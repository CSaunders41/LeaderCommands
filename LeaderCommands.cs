using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace LeaderCommands
{
    public class LeaderCommands : BaseSettingsPlugin<LeaderCommandsSettings>
    {
        private Dictionary<string, DateTime> _lastCommandTimes = new Dictionary<string, DateTime>();
        
        // Network components
        private TcpListener _tcpListener;
        private UdpClient _discoveryBroadcaster;
        private List<TcpClient> _connectedFollowers = new List<TcpClient>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _serverTask;
        private Task _discoveryTask;
        private readonly object _followersLock = new object();

        public override bool Initialise()
        {
            Name = "LeaderCommands";
            
            // Register hotkeys
            Input.RegisterKey(Settings.StashItemsKey.Value);
            Input.RegisterKey(Settings.SellItemsKey.Value);
            Input.RegisterKey(Settings.AcceptTradeKey.Value);
            Input.RegisterKey(Settings.EmergencyStopKey.Value);
            
            Settings.StashItemsKey.OnValueChanged += () => { Input.RegisterKey(Settings.StashItemsKey.Value); };
            Settings.SellItemsKey.OnValueChanged += () => { Input.RegisterKey(Settings.SellItemsKey.Value); };
            Settings.AcceptTradeKey.OnValueChanged += () => { Input.RegisterKey(Settings.AcceptTradeKey.Value); };
            Settings.EmergencyStopKey.OnValueChanged += () => { Input.RegisterKey(Settings.EmergencyStopKey.Value); };

            // Start network services
            StartNetworkServices();
            
            return base.Initialise();
        }

        private void StartNetworkServices()
        {
            try
            {
                if (!Settings.EnableNetworkCommunication.Value)
                    return;

                // Start TCP server
                _serverTask = Task.Run(() => StartTcpServer(_cancellationTokenSource.Token));
                
                // Start auto-discovery broadcaster
                if (Settings.EnableAutoDiscovery.Value)
                {
                    _discoveryTask = Task.Run(() => StartDiscoveryBroadcaster(_cancellationTokenSource.Token));
                }
                
                LogMessage("Network services started successfully", 4);
            }
            catch (Exception ex)
            {
                LogMessage($"Failed to start network services: {ex.Message}", 1);
            }
        }

        private async Task StartTcpServer(CancellationToken cancellationToken)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, Settings.ServerPort.Value);
                _tcpListener.Start();
                
                LogMessage($"TCP server listening on port {Settings.ServerPort.Value}", 4);
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                        
                        lock (_followersLock)
                        {
                            _connectedFollowers.Add(tcpClient);
                        }
                        
                        LogMessage($"Follower connected: {tcpClient.Client.RemoteEndPoint}", 4);
                        
                        // Handle client in separate task
                        _ = Task.Run(() => HandleFollowerConnection(tcpClient, cancellationToken));
                    }
                    catch (ObjectDisposedException)
                    {
                        // Server was stopped
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error accepting client: {ex.Message}", 1);
                        await Task.Delay(1000, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"TCP server error: {ex.Message}", 1);
            }
        }

        private async Task HandleFollowerConnection(TcpClient client, CancellationToken cancellationToken)
        {
            try
            {
                var networkStream = client.GetStream();
                var buffer = new byte[4096];
                
                while (!cancellationToken.IsCancellationRequested && client.Connected)
                {
                    try
                    {
                        var bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0)
                        {
                            // Client disconnected
                            break;
                        }
                        
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        
                        // Handle follower messages (like status updates, confirmations, etc.)
                        await ProcessFollowerMessage(message, client);
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error handling follower message: {ex.Message}", 1);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error in follower connection: {ex.Message}", 1);
            }
            finally
            {
                lock (_followersLock)
                {
                    _connectedFollowers.Remove(client);
                }
                
                client?.Close();
                LogMessage($"Follower disconnected: {client?.Client?.RemoteEndPoint}", 4);
            }
        }

        private async Task ProcessFollowerMessage(string message, TcpClient client)
        {
            try
            {
                var followerMessage = JsonConvert.DeserializeObject<FollowerMessage>(message);
                
                if (followerMessage?.Type == "STATUS")
                {
                    LogMessage($"Follower status: {followerMessage.Data}", 4);
                }
                else if (followerMessage?.Type == "COMMAND_COMPLETE")
                {
                    LogMessage($"Command completed: {followerMessage.Data}", 4);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error processing follower message: {ex.Message}", 1);
            }
        }

        private async Task StartDiscoveryBroadcaster(CancellationToken cancellationToken)
        {
            try
            {
                _discoveryBroadcaster = new UdpClient();
                _discoveryBroadcaster.EnableBroadcast = true;
                
                var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, Settings.DiscoveryPort.Value);
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var localIp = GetLocalIPAddress();
                        var characterName = GameController?.Game?.IngameState?.Data?.LocalPlayer?.GetComponent<Player>()?.PlayerName ?? "Unknown";
                        
                        var discoveryMessage = new DiscoveryMessage
                        {
                            Type = "LEADER_DISCOVERY",
                            LeaderName = characterName,
                            IpAddress = localIp?.ToString() ?? "Unknown",
                            Port = Settings.ServerPort.Value,
                            Timestamp = DateTime.Now
                        };
                        
                        var jsonMessage = JsonConvert.SerializeObject(discoveryMessage);
                        var data = Encoding.UTF8.GetBytes(jsonMessage);
                        
                        await _discoveryBroadcaster.SendAsync(data, data.Length, broadcastEndpoint);
                        
                        // Broadcast every 5 seconds
                        await Task.Delay(5000, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error broadcasting discovery: {ex.Message}", 1);
                        await Task.Delay(5000, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Discovery broadcaster error: {ex.Message}", 1);
            }
        }

        private IPAddress GetLocalIPAddress()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up && 
                        networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var ipProperties = networkInterface.GetIPProperties();
                        foreach (var ipAddress in ipProperties.UnicastAddresses)
                        {
                            if (ipAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ipAddress.Address;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error getting local IP: {ex.Message}", 1);
            }
            
            return IPAddress.Loopback;
        }

        public override Job Tick()
        {
            if (!Settings.Enable.Value)
                return null;
                
            // Handle hotkey presses
            HandleHotkeys();
            
            return null;
        }

        private void HandleHotkeys()
        {
            try
            {
                if (Input.GetKeyState(Settings.StashItemsKey.Value) && CanSendCommand("STASH_ITEMS"))
                {
                    _ = SendCommandToFollowers("STASH_ITEMS");
                }
                
                if (Input.GetKeyState(Settings.SellItemsKey.Value) && CanSendCommand("SELL_ITEMS"))
                {
                    _ = SendCommandToFollowers("SELL_ITEMS");
                }
                
                if (Input.GetKeyState(Settings.AcceptTradeKey.Value) && CanSendCommand("ACCEPT_TRADE"))
                {
                    _ = SendCommandToFollowers("ACCEPT_TRADE");
                }
                
                if (Input.GetKeyState(Settings.EmergencyStopKey.Value))
                {
                    _ = SendCommandToFollowers("EMERGENCY_STOP");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling hotkeys: {ex.Message}", 1);
            }
        }

        private async Task SendCommandToFollowers(string command)
        {
            try
            {
                if (!IsCommandEnabled(command))
                {
                    LogMessage($"Command {command} is disabled", 3);
                    return;
                }
                
                var commandMessage = new CommandMessage
                {
                    Type = "COMMAND",
                    Command = command,
                    Data = GetCommandData(command),
                    Timestamp = DateTime.Now
                };
                
                var jsonMessage = JsonConvert.SerializeObject(commandMessage);
                var data = Encoding.UTF8.GetBytes(jsonMessage);
                
                List<TcpClient> followersToRemove = new List<TcpClient>();
                
                lock (_followersLock)
                {
                    foreach (var follower in _connectedFollowers.ToList())
                    {
                        try
                        {
                            if (follower.Connected)
                            {
                                var stream = follower.GetStream();
                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync();
                            }
                            else
                            {
                                followersToRemove.Add(follower);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"Error sending command to follower: {ex.Message}", 1);
                            followersToRemove.Add(follower);
                        }
                    }
                    
                    // Remove disconnected followers
                    foreach (var follower in followersToRemove)
                    {
                        _connectedFollowers.Remove(follower);
                        follower?.Close();
                    }
                }
                
                _lastCommandTimes[command] = DateTime.Now;
                LogMessage($"Sent command {command} to {_connectedFollowers.Count} followers", 4);
            }
            catch (Exception ex)
            {
                LogMessage($"Error sending command to followers: {ex.Message}", 1);
            }
        }

        private Dictionary<string, object> GetCommandData(string command)
        {
            var data = new Dictionary<string, object>();
            
            switch (command)
            {
                case "STASH_ITEMS":
                    data = GetStashSettings();
                    break;
                case "SELL_ITEMS":
                    data = GetSellSettings();
                    break;
                case "ACCEPT_TRADE":
                    data["TradeWhitelist"] = GetTradeWhitelist();
                    break;
            }
            
            return data;
        }

        private bool CanSendCommand(string command)
        {
            if (!_lastCommandTimes.ContainsKey(command))
                return true;
                
            var timeSinceLastCommand = DateTime.Now - _lastCommandTimes[command];
            return timeSinceLastCommand.TotalMilliseconds >= Settings.CommandCooldown.Value;
        }

        public bool IsCommandEnabled(string command)
        {
            return command switch
            {
                "STASH_ITEMS" => Settings.EnableStashCommands.Value,
                "SELL_ITEMS" => Settings.EnableSellCommands.Value,
                "ACCEPT_TRADE" => Settings.EnableTradeCommands.Value,
                "EMERGENCY_STOP" => true,
                _ => false
            };
        }

        public List<string> GetTradeWhitelist()
        {
            if (string.IsNullOrEmpty(Settings.TradeWhitelist.Value))
                return new List<string>();
                
            return Settings.TradeWhitelist.Value
                .Split(',')
                .Select(name => name.Trim().ToLower())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }

        public Dictionary<string, object> GetStashSettings()
        {
            return new Dictionary<string, object>
            {
                ["StashCurrency"] = Settings.StashCurrency.Value,
                ["StashMaps"] = Settings.StashMaps.Value,
                ["StashGems"] = Settings.StashGems.Value,
                ["StashUniques"] = Settings.StashUniques.Value,
                ["StashRares"] = Settings.StashRares.Value,
                ["StashDivCards"] = Settings.StashDivCards.Value,
                ["StashFragments"] = Settings.StashFragments.Value,
                ["StashEssences"] = Settings.StashEssences.Value,
                ["StashFossils"] = Settings.StashFossils.Value,
                ["StashResonators"] = Settings.StashResonators.Value,
                ["StashScarabs"] = Settings.StashScarabs.Value,
                ["StashIncubators"] = Settings.StashIncubators.Value,
                ["StashFlasks"] = Settings.StashFlasks.Value
            };
        }

        public Dictionary<string, object> GetSellSettings()
        {
            return new Dictionary<string, object>
            {
                ["SellWhiteItems"] = Settings.SellWhiteItems.Value,
                ["SellBlueItems"] = Settings.SellBlueItems.Value,
                ["SellYellowItems"] = Settings.SellYellowItems.Value,
                ["SellMinLevel"] = Settings.SellMinLevel.Value,
                ["SellMaxLevel"] = Settings.SellMaxLevel.Value,
                ["SellWeapons"] = Settings.SellWeapons.Value,
                ["SellArmor"] = Settings.SellArmor.Value,
                ["SellAccessories"] = Settings.SellAccessories.Value
            };
        }

        public override void Dispose()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                
                lock (_followersLock)
                {
                    foreach (var follower in _connectedFollowers)
                    {
                        follower?.Close();
                    }
                    _connectedFollowers.Clear();
                }
                
                _tcpListener?.Stop();
                _discoveryBroadcaster?.Close();
                
                _serverTask?.Wait(1000);
                _discoveryTask?.Wait(1000);
                
                _cancellationTokenSource?.Dispose();
            }
            catch (Exception ex)
            {
                LogMessage($"Error disposing network services: {ex.Message}", 1);
            }
            
            base.Dispose();
        }

        public override void Render()
        {
            try
            {
                if (!Settings.Enable.Value || !Settings.ShowDebugInfo.Value)
                    return;

                var pos = new Vector2(Settings.DebugPosition.Value.X, Settings.DebugPosition.Value.Y);
                var textColor = Settings.DebugTextColor.Value;
                
                int connectedFollowers;
                lock (_followersLock)
                {
                    connectedFollowers = _connectedFollowers.Count;
                }
                
                Graphics.DrawText($"LeaderCommands Status:", pos, textColor);
                pos.Y += 20;
                Graphics.DrawText($"Network: {(Settings.EnableNetworkCommunication.Value ? "Enabled" : "Disabled")}", pos, textColor);
                pos.Y += 15;
                Graphics.DrawText($"Connected Followers: {connectedFollowers}", pos, textColor);
                pos.Y += 15;
                Graphics.DrawText($"Server Port: {Settings.ServerPort.Value}", pos, textColor);
                pos.Y += 15;
                Graphics.DrawText($"Auto-Discovery: {(Settings.EnableAutoDiscovery.Value ? "Enabled" : "Disabled")}", pos, textColor);
            }
            catch (Exception ex)
            {
                LogMessage($"Error in render: {ex.Message}", 1);
            }
        }
    }

    // Message classes for network communication
    public class CommandMessage
    {
        public string Type { get; set; }
        public string Command { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class FollowerMessage
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DiscoveryMessage
    {
        public string Type { get; set; }
        public string LeaderName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;
using System.Windows.Forms;

namespace LeaderCommands
{
    public class LeaderCommandsSettings : ISettings
    {
        [Menu("Enable Plugin")]
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Network Communication", "Network Communication Settings")]
        public EmptyNode NetworkSettings { get; set; } = new EmptyNode();

        [Menu("Enable Network Communication", "Enable TCP communication with followers", parentIndex = 0)]
        public ToggleNode EnableNetworkCommunication { get; set; } = new ToggleNode(true);

        [Menu("Server Port", "TCP server port for follower connections", parentIndex = 0)]
        public RangeNode<int> ServerPort { get; set; } = new RangeNode<int>(7777, 1024, 65535);

        [Menu("Enable Auto-Discovery", "Enable UDP broadcast for automatic follower discovery", parentIndex = 0)]
        public ToggleNode EnableAutoDiscovery { get; set; } = new ToggleNode(true);

        [Menu("Discovery Port", "UDP port for auto-discovery broadcasts", parentIndex = 0)]
        public RangeNode<int> DiscoveryPort { get; set; } = new RangeNode<int>(7778, 1024, 65535);

        [Menu("Hotkeys", "Hotkey Configuration")]
        public EmptyNode HotkeySettings { get; set; } = new EmptyNode();

        [Menu("Stash Items Key", "Hotkey to send stash items command", parentIndex = 1)]
        public HotkeyNode StashItemsKey { get; set; } = new HotkeyNode(Keys.F5);

        [Menu("Sell Items Key", "Hotkey to send sell items command", parentIndex = 1)]
        public HotkeyNode SellItemsKey { get; set; } = new HotkeyNode(Keys.F6);

        [Menu("Accept Trade Key", "Hotkey to send accept trade command", parentIndex = 1)]
        public HotkeyNode AcceptTradeKey { get; set; } = new HotkeyNode(Keys.F7);

        [Menu("Emergency Stop Key", "Hotkey to send emergency stop command", parentIndex = 1)]
        public HotkeyNode EmergencyStopKey { get; set; } = new HotkeyNode(Keys.F12);

        [Menu("Command Settings", "Command Configuration")]
        public EmptyNode CommandSettings { get; set; } = new EmptyNode();

        [Menu("Enable Stash Commands", "Allow sending stash commands to followers", parentIndex = 2)]
        public ToggleNode EnableStashCommands { get; set; } = new ToggleNode(true);

        [Menu("Enable Sell Commands", "Allow sending sell commands to followers", parentIndex = 2)]
        public ToggleNode EnableSellCommands { get; set; } = new ToggleNode(true);

        [Menu("Enable Trade Commands", "Allow sending trade commands to followers", parentIndex = 2)]
        public ToggleNode EnableTradeCommands { get; set; } = new ToggleNode(true);

        [Menu("Command Cooldown (ms)", "Minimum time between same commands", parentIndex = 2)]
        public RangeNode<int> CommandCooldown { get; set; } = new RangeNode<int>(2000, 500, 10000);

        [Menu("Stash Settings", "Stash Configuration")]
        public EmptyNode StashSettings { get; set; } = new EmptyNode();

        [Menu("Stash Currency", "Stash currency items", parentIndex = 3)]
        public ToggleNode StashCurrency { get; set; } = new ToggleNode(true);

        [Menu("Stash Maps", "Stash maps", parentIndex = 3)]
        public ToggleNode StashMaps { get; set; } = new ToggleNode(true);

        [Menu("Stash Gems", "Stash gems", parentIndex = 3)]
        public ToggleNode StashGems { get; set; } = new ToggleNode(true);

        [Menu("Stash Uniques", "Stash unique items", parentIndex = 3)]
        public ToggleNode StashUniques { get; set; } = new ToggleNode(true);

        [Menu("Stash Rares", "Stash rare items", parentIndex = 3)]
        public ToggleNode StashRares { get; set; } = new ToggleNode(false);

        [Menu("Stash Divination Cards", "Stash divination cards", parentIndex = 3)]
        public ToggleNode StashDivCards { get; set; } = new ToggleNode(true);

        [Menu("Stash Fragments", "Stash fragments", parentIndex = 3)]
        public ToggleNode StashFragments { get; set; } = new ToggleNode(true);

        [Menu("Stash Essences", "Stash essences", parentIndex = 3)]
        public ToggleNode StashEssences { get; set; } = new ToggleNode(true);

        [Menu("Stash Fossils", "Stash fossils", parentIndex = 3)]
        public ToggleNode StashFossils { get; set; } = new ToggleNode(true);

        [Menu("Stash Resonators", "Stash resonators", parentIndex = 3)]
        public ToggleNode StashResonators { get; set; } = new ToggleNode(true);

        [Menu("Stash Scarabs", "Stash scarabs", parentIndex = 3)]
        public ToggleNode StashScarabs { get; set; } = new ToggleNode(true);

        [Menu("Stash Incubators", "Stash incubators", parentIndex = 3)]
        public ToggleNode StashIncubators { get; set; } = new ToggleNode(true);

        [Menu("Stash Flasks", "Stash flasks", parentIndex = 3)]
        public ToggleNode StashFlasks { get; set; } = new ToggleNode(false);

        [Menu("Sell Settings", "Sell Configuration")]
        public EmptyNode SellSettings { get; set; } = new EmptyNode();

        [Menu("Sell White Items", "Sell white (normal) items", parentIndex = 4)]
        public ToggleNode SellWhiteItems { get; set; } = new ToggleNode(true);

        [Menu("Sell Blue Items", "Sell blue (magic) items", parentIndex = 4)]
        public ToggleNode SellBlueItems { get; set; } = new ToggleNode(true);

        [Menu("Sell Yellow Items", "Sell yellow (rare) items", parentIndex = 4)]
        public ToggleNode SellYellowItems { get; set; } = new ToggleNode(false);

        [Menu("Sell Min Level", "Minimum item level to sell", parentIndex = 4)]
        public RangeNode<int> SellMinLevel { get; set; } = new RangeNode<int>(1, 1, 100);

        [Menu("Sell Max Level", "Maximum item level to sell", parentIndex = 4)]
        public RangeNode<int> SellMaxLevel { get; set; } = new RangeNode<int>(60, 1, 100);

        [Menu("Sell Weapons", "Sell weapons", parentIndex = 4)]
        public ToggleNode SellWeapons { get; set; } = new ToggleNode(true);

        [Menu("Sell Armor", "Sell armor", parentIndex = 4)]
        public ToggleNode SellArmor { get; set; } = new ToggleNode(true);

        [Menu("Sell Accessories", "Sell accessories (rings, amulets, belts)", parentIndex = 4)]
        public ToggleNode SellAccessories { get; set; } = new ToggleNode(true);

        [Menu("Trade Settings", "Trade Configuration")]
        public EmptyNode TradeSettings { get; set; } = new EmptyNode();

        [Menu("Trade Whitelist", "Comma-separated list of allowed trader names", parentIndex = 5)]
        public TextNode TradeWhitelist { get; set; } = new TextNode("");

        [Menu("Debug Settings", "Debug Configuration")]
        public EmptyNode DebugSettings { get; set; } = new EmptyNode();

        [Menu("Show Debug Info", "Show debug information overlay", parentIndex = 6)]
        public ToggleNode ShowDebugInfo { get; set; } = new ToggleNode(false);

        [Menu("Debug Position", "Position of debug information", parentIndex = 6)]
        public RangeNode<Vector2> DebugPosition { get; set; } = new RangeNode<Vector2>(new Vector2(10, 300), new Vector2(0, 0), new Vector2(2000, 2000));

        [Menu("Debug Text Color", "Color of debug text", parentIndex = 6)]
        public ColorNode DebugTextColor { get; set; } = new ColorNode(Color.White);
    }
} 
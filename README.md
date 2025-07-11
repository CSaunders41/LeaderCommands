# LeaderCommands Plugin for ExileCore

A leader-to-follower command system for Path of Exile that enables **cross-PC communication** for automated stashing, selling, and trade acceptance.

## Overview

The LeaderCommands plugin creates a TCP server that communicates with Follower plugins running on **different PCs** across your network. Send commands via hotkeys that are received and executed by follower characters in real-time, enabling seamless multi-PC gameplay.

## Features

- **Cross-PC Communication**: TCP server enables communication between different computers
- **Auto-Discovery**: Followers automatically find and connect to leaders on the network
- **Real-time Commands**: Instant command transmission via hotkeys
- **Configurable Settings**: Extensive customization for stashing, selling, and trading
- **Connection Management**: Automatic reconnection and heartbeat monitoring
- **Debug Information**: Visual connection status and command monitoring
- **Emergency Stop**: Instant halt of all follower activities

## Installation

1. **Copy** the `LeaderCommands` folder to your `ExileCore/Plugins/Source/` directory
2. **Ensure** the enhanced Follower plugin is installed on follower PCs
3. **Restart** ExileCore to load the plugin
4. **Configure** network settings via F12 menu

## Network Setup

### Leader PC Setup

1. **Load** the LeaderCommands plugin in ExileCore
2. **Configure network settings** (F12 → LeaderCommands → Network Communication):
   - **Enable Network Communication**: ✓ (checked)
   - **Server Port**: 7777 (default, configurable)
   - **Enable Auto-Discovery**: ✓ (recommended)
   - **Discovery Port**: 7778 (default)
3. **Configure hotkeys** and command preferences
4. **Windows Firewall**: Allow ExileCore through firewall when prompted

### Follower PC Setup

1. **Load** the enhanced Follower plugin in ExileCore
2. **Configure network settings** (F12 → Follower → Network Communication):
   - **Enable Network Communication**: ✓ (checked)
   - **Enable Auto Discovery**: ✓ (recommended for automatic connection)
   - **Leader IP Address**: Leave blank for auto-discovery, or enter manually
   - **Leader Port**: 7777 (must match leader's port)
3. **Configure** follower behavior preferences

### Network Requirements

- **Same Network**: Both PCs must be on the same local network (WiFi/LAN)
- **Firewall**: Windows may prompt to allow network access - click "Allow"
- **Ports**: Default ports 7777 (TCP) and 7778 (UDP) must be accessible

## Auto-Discovery

Auto-discovery eliminates the need for manual IP configuration:

1. **Leader broadcasts** its presence every 5 seconds
2. **Followers listen** for leader broadcasts and connect automatically
3. **Character-specific**: Uses actual character names for identification
4. **Fallback**: Manual IP configuration available if auto-discovery fails

## Available Commands

### Stash Items (F5)
- **Hotkey**: F5 (configurable)
- **Function**: Followers organize inventory items into stash tabs
- **Settings**: Configure which item types to stash (currency, maps, gems, etc.)
- **Execution**: Followers find nearby stash and sort items automatically

### Sell Items (F6)
- **Hotkey**: F6 (configurable)  
- **Function**: Followers sell items to vendors based on criteria
- **Settings**: Configure item rarity, level ranges, and item types to sell
- **Execution**: Followers find nearby vendor and sell matching items

### Accept Trade (F7)
- **Hotkey**: F7 (configurable)
- **Function**: Followers accept trade requests from whitelisted players
- **Settings**: Configure whitelist of trusted trader names
- **Execution**: Followers check incoming trades and auto-accept if approved

### Emergency Stop (F12)
- **Hotkey**: F12 (configurable)
- **Function**: Immediately halt all follower activities
- **Execution**: Stops all commands, clears task queues, pauses following

## Configuration

### Network Settings
- **Server Port**: TCP port for follower connections (default: 7777)
- **Discovery Port**: UDP port for auto-discovery broadcasts (default: 7778)
- **Max Connected Followers**: Limit simultaneous connections
- **Connection Monitoring**: Track follower connection status

### Command Settings
- **Command Cooldown**: Minimum time between identical commands (default: 2s)
- **Enable/Disable**: Individual command type controls
- **Execution Timeout**: Maximum time for command completion

### Stash Settings
- **Item Types**: Currency, Maps, Gems, Uniques, Rares, Divination Cards, etc.
- **Tab Organization**: Automatic stash tab detection and sorting
- **Quality Filtering**: Minimum quality thresholds for gems/flasks

### Sell Settings
- **Item Rarity**: White, Blue, Yellow (rare) items
- **Level Ranges**: Minimum/maximum item levels
- **Item Categories**: Weapons, Armor, Accessories
- **Value Thresholds**: Minimum sale values

### Trade Settings
- **Whitelist**: Comma-separated list of trusted trader names
- **Proximity Requirements**: Trade only when leader is nearby
- **Timeout Settings**: Maximum time to wait for trade completion

## Network Troubleshooting

### Connection Issues
- ✅ **Verify same network**: Both PCs on same WiFi/LAN
- ✅ **Check firewall**: Allow ExileCore through Windows Firewall
- ✅ **Port accessibility**: Ensure ports 7777/7778 are not blocked
- ✅ **Plugin enabled**: Network communication enabled on both PCs

### Auto-Discovery Problems
- ✅ **Character names**: Ensure follower knows leader's character name
- ✅ **UDP port**: Check Discovery Port setting (default 7778)
- ✅ **Manual fallback**: Enter leader's IP address manually
- ✅ **Network broadcast**: Some networks block UDP broadcasts

### Command Execution Issues
- ✅ **Connection status**: Verify follower shows "Connected" in debug info
- ✅ **Command cooldowns**: Check if commands are on cooldown
- ✅ **Execution settings**: Verify command types are enabled
- ✅ **Proximity requirements**: Ensure followers are near stash/vendor

## Debug Information

Enable **Show Debug Info** to monitor:
- **Connection Status**: Number of connected followers
- **Network State**: Server port and auto-discovery status
- **Command History**: Recently sent commands and responses
- **Performance**: Command execution timing and success rates

## Technical Details

### Network Architecture
- **Protocol**: TCP for reliable command transmission
- **Discovery**: UDP broadcast for automatic leader detection
- **Message Format**: JSON serialization for cross-platform compatibility
- **Connection Management**: Automatic reconnection and heartbeat monitoring

### Performance Optimization
- **Asynchronous Processing**: Non-blocking network operations
- **Connection Pooling**: Efficient handling of multiple followers
- **Command Queuing**: Ordered execution with timeout protection
- **Heartbeat Monitoring**: Automatic detection of disconnected followers

### Security Considerations
- **Local Network Only**: Communication restricted to LAN/WiFi
- **No External Access**: No internet connectivity required
- **Character Validation**: Commands tied to specific character names
- **Timeout Protection**: Commands automatically expire to prevent abuse

## Advanced Usage

### Multiple Followers
- **Simultaneous Connections**: Support for multiple follower PCs
- **Broadcast Commands**: Single hotkey affects all connected followers
- **Individual Monitoring**: Track each follower's connection status
- **Load Balancing**: Distribute commands across available followers

### Custom Configuration
- **Port Customization**: Change default ports for multiple leaders
- **Network Interfaces**: Specify which network adapter to use
- **Connection Limits**: Control maximum concurrent connections
- **Logging Levels**: Adjust debug output verbosity

### Integration with Other Plugins
- **PickItV2**: Followers yield control during item pickup
- **Stashie**: Compatible with existing stash organization
- **Trade Macros**: Works alongside trading automation
- **Performance Monitoring**: Minimal impact on game performance

## Future Enhancements

- **Command Scripting**: Complex command sequences and macros
- **Conditional Logic**: Execute commands based on game state
- **Mobile Control**: Remote command sending from mobile devices
- **Voice Commands**: Speech recognition for hands-free control
- **Web Interface**: Browser-based follower management
- **Multi-Leader Support**: Followers can switch between multiple leaders

## Version History

- **v3.0**: Cross-PC TCP communication with auto-discovery
- **v2.0**: Enhanced PluginBridge system (deprecated)
- **v1.0**: Initial leader-follower command system

## Support

For issues or questions:
1. **Check Debug Info**: Enable debug overlay for connection status
2. **Review Logs**: Check ExileCore logs for error messages
3. **Network Diagnostics**: Verify network connectivity and firewall settings
4. **Community Support**: Share configuration and troubleshooting tips

---

**Note**: This system requires network communication between PCs. Ensure both leader and follower PCs are on the same network and have appropriate firewall permissions. 
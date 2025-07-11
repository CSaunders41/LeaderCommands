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

        // Network Communication Settings
        [Menu("Enable Network Communication", "Enable TCP communication with followers")]
        public ToggleNode EnableNetworkCommunication { get; set; } = new ToggleNode(true);

        [Menu("Server Port", "TCP server port for follower connections")]
        public RangeNode<int> ServerPort { get; set; } = new RangeNode<int>(7777, 1024, 65535);

        [Menu("Enable Auto-Discovery", "Enable UDP broadcast for automatic follower discovery")]
        public ToggleNode EnableAutoDiscovery { get; set; } = new ToggleNode(true);

        [Menu("Discovery Port", "UDP port for auto-discovery broadcasts")]
        public RangeNode<int> DiscoveryPort { get; set; } = new RangeNode<int>(7778, 1024, 65535);

        // Hotkey Settings
        [Menu("Stash Items Key", "Hotkey to send stash items command")]
        public HotkeyNode StashItemsKey { get; set; } = new HotkeyNode(Keys.F5);

        [Menu("Sell Items Key", "Hotkey to send sell items command")]
        public HotkeyNode SellItemsKey { get; set; } = new HotkeyNode(Keys.F6);

        [Menu("Accept Trade Key", "Hotkey to send accept trade command")]
        public HotkeyNode AcceptTradeKey { get; set; } = new HotkeyNode(Keys.F7);

        [Menu("Emergency Stop Key", "Hotkey to send emergency stop command")]
        public HotkeyNode EmergencyStopKey { get; set; } = new HotkeyNode(Keys.F12);

        // Command Settings
        [Menu("Enable Stash Commands", "Allow sending stash commands to followers")]
        public ToggleNode EnableStashCommands { get; set; } = new ToggleNode(true);

        [Menu("Enable Sell Commands", "Allow sending sell commands to followers")]
        public ToggleNode EnableSellCommands { get; set; } = new ToggleNode(true);

        [Menu("Enable Trade Commands", "Allow sending trade commands to followers")]
        public ToggleNode EnableTradeCommands { get; set; } = new ToggleNode(true);

        [Menu("Command Cooldown (ms)", "Minimum time between same commands")]
        public RangeNode<int> CommandCooldown { get; set; } = new RangeNode<int>(2000, 500, 10000);

        // Stash Settings
        [Menu("Stash Currency", "Stash currency items")]
        public ToggleNode StashCurrency { get; set; } = new ToggleNode(true);

        [Menu("Stash Maps", "Stash maps")]
        public ToggleNode StashMaps { get; set; } = new ToggleNode(true);

        [Menu("Stash Gems", "Stash gems")]
        public ToggleNode StashGems { get; set; } = new ToggleNode(true);

        [Menu("Stash Uniques", "Stash unique items")]
        public ToggleNode StashUniques { get; set; } = new ToggleNode(true);

        [Menu("Stash Rares", "Stash rare items")]
        public ToggleNode StashRares { get; set; } = new ToggleNode(false);

        [Menu("Stash Divination Cards", "Stash divination cards")]
        public ToggleNode StashDivCards { get; set; } = new ToggleNode(true);

        [Menu("Stash Fragments", "Stash fragments")]
        public ToggleNode StashFragments { get; set; } = new ToggleNode(true);

        [Menu("Stash Essences", "Stash essences")]
        public ToggleNode StashEssences { get; set; } = new ToggleNode(true);

        [Menu("Stash Fossils", "Stash fossils")]
        public ToggleNode StashFossils { get; set; } = new ToggleNode(true);

        [Menu("Stash Resonators", "Stash resonators")]
        public ToggleNode StashResonators { get; set; } = new ToggleNode(true);

        [Menu("Stash Scarabs", "Stash scarabs")]
        public ToggleNode StashScarabs { get; set; } = new ToggleNode(true);

        [Menu("Stash Incubators", "Stash incubators")]
        public ToggleNode StashIncubators { get; set; } = new ToggleNode(true);

        [Menu("Stash Flasks", "Stash flasks")]
        public ToggleNode StashFlasks { get; set; } = new ToggleNode(false);

        // Sell Settings
        [Menu("Sell White Items", "Sell white (normal) items")]
        public ToggleNode SellWhiteItems { get; set; } = new ToggleNode(true);

        [Menu("Sell Blue Items", "Sell blue (magic) items")]
        public ToggleNode SellBlueItems { get; set; } = new ToggleNode(true);

        [Menu("Sell Yellow Items", "Sell yellow (rare) items")]
        public ToggleNode SellYellowItems { get; set; } = new ToggleNode(false);

        [Menu("Sell Min Level", "Minimum item level to sell")]
        public RangeNode<int> SellMinLevel { get; set; } = new RangeNode<int>(1, 1, 100);

        [Menu("Sell Max Level", "Maximum item level to sell")]
        public RangeNode<int> SellMaxLevel { get; set; } = new RangeNode<int>(60, 1, 100);

        [Menu("Sell Weapons", "Sell weapons")]
        public ToggleNode SellWeapons { get; set; } = new ToggleNode(true);

        [Menu("Sell Armor", "Sell armor")]
        public ToggleNode SellArmor { get; set; } = new ToggleNode(true);

        [Menu("Sell Accessories", "Sell accessories (rings, amulets, belts)")]
        public ToggleNode SellAccessories { get; set; } = new ToggleNode(true);

        // Trade Settings
        [Menu("Trade Whitelist", "Comma-separated list of allowed trader names")]
        public TextNode TradeWhitelist { get; set; } = new TextNode("");

        // Debug Settings
        [Menu("Show Debug Info", "Show debug information overlay")]
        public ToggleNode ShowDebugInfo { get; set; } = new ToggleNode(false);

        [Menu("Debug Position", "Position of debug information")]
        public RangeNode<Vector2> DebugPosition { get; set; } = new RangeNode<Vector2>(new Vector2(10, 300), new Vector2(0, 0), new Vector2(2000, 2000));

        [Menu("Debug Text Color", "Color of debug text")]
        public ColorNode DebugTextColor { get; set; } = new ColorNode(Color.White);
    }
} 
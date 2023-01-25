using ServersDataAggregation.Common.Enums;

namespace ServersDataAggregation.Common.Model
{
    [Flags]
    public enum PlayerSnapshotFeatureFlags : short
    {
        Clothes,
        PlayerType
    }

    /// <summary>
    /// Contains information reported by server queries
    /// </summary>
    public class PlayerSnapshot
    {
        public PlayerSnapshotFeatureFlags FeatureFlags { get; set; }
        /// <summary>
        /// Player's Name (utf8)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Player's Name Byte Value (for extended ascii)
        /// </summary>
        public byte[] NameRaw { get; set; }
        /// <summary>
        /// Player's Number
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Shirt Color is using
        /// </summary>
        public int ShirtColor { get; set; }
        /// <summary>
        /// PantColor is using
        /// </summary>
        public int  PantColor { get; set; }
        /// <summary>
        /// Skin player is using
        /// </summary>
        public string SkinName { get; set; }
        /// <summary>
        /// Model Player is using
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// Ping as reported by the server
        /// </summary>
        public int Ping { get; set; }
        /// <summary>
        /// Current score
        /// </summary>
        public int Frags { get; set; }
        /// <summary>
        /// Current server reported playing time of player
        /// </summary>
        public TimeSpan PlayTime { get; set; }
        public PlayerType PlayerType { get; set; }
    }
}

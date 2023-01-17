namespace ServersDataAggregation.Common.Model
{
    public class Player
    {
        /// <summary>
        /// Text friendly name of Player
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Raw byte representation of Player's name
        /// </summary>
        public byte[] NameBytes { get; private set; }
        /// <summary>
        /// How long Player has been on the server
        /// </summary>
        public TimeSpan UpTime { get; private set; }
        /// <summary>
        /// Current frag count of Player
        /// </summary>
        public int CurrentFrags { get; private set; }
        /// <summary>
        /// Total frags earned by Player while Connected
        /// </summary>
        public int TotalFrags { get; private set; }
        /// <summary>
        /// Player's Unique Identifier
        /// </summary>
        public int PlayerId { get; private set; }
        /// <summary>
        /// If supported, Player's shirt color
        /// </summary>
        public int ShirtColor { get; private set; }
        /// <summary>
        /// If supported, Player's pant color
        /// </summary>
        public int PantColor { get; private set; }
        /// <summary>
        /// If supported, current skin used by player
        /// </summary>
        public string Skin { get; private set; }
        /// <summary>
        /// If supported, current model used by player
        /// </summary>
        public string Model { get; private set; }
    }
}

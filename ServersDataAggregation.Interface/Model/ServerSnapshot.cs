using ServersDataAggregation.Common.Enums;

namespace ServersDataAggregation.Common.Model
{
    public class ServerSnapshot
    {
        /// <summary>
        /// IpAddress of server
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// TCP/IP listening port of server
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Server's name
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// Current map server is hosting
        /// </summary>
        public string Map { get; set; }
        /// <summary>
        /// Maximum number of players
        /// </summary>
        public int MaxPlayerCount { get; set; }
        /// <summary>
        /// Server version
        /// </summary>
        public string ServerVersion { get; set; }
        /// <summary>
        /// Current server status
        /// </summary>
        public ServerStatus Status { get; set; }
        /// <summary>
        /// ServerSettings in Key/Value format
        /// </summary>
        public ServerSetting[] ServerSettings { get; set; }
        /// <summary>
        /// Collection of PlayerInfos representing player entities
        /// currently on the server
        /// </summary>
        public PlayerSnapshot[] Players { get; set; }
        /// <summary>
        /// Current Mod server is hosting
        /// </summary>
        public string Mod { get; set; }
        /// <summary>
        /// Current Mode server is hosting (Ie: CRx may run CTF or FFA or Match mode)
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// Current Status of Match (Pickup, Waiting for Game, Match in Progress)
        /// </summary>
        public MatchStatus MatchStatus { get; set; }
        /// <summary>
        /// Frag Limit of game/match
        /// </summary>
        public int  Timelimit { get; set; }
        /// <summary>
        /// Time Limit of game/match
        /// </summary>
        public int Fraglimit { get; set; }
    }
}

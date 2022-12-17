using ServersDataAggregation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Common.Model
{
    public class Server
    {
        /// <summary>
        /// Internal server Identification
        /// </summary>
        public int ServerId { get; private set; }
        /// <summary>
        /// Server Name received from server query
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Server Name specified inside of the database defined at setup
        /// </summary>
        public string CustomName { get; private set; }
        /// <summary>
        /// Server Name specified inside of the database defined at setup (short version)
        /// </summary>
        public string CustomNameShort { get; private set; }
        /// <summary>
        /// Modification Name inside of the database defined at setup
        /// </summary>
        public string CustomModificationName { get; private set; }
        /// <summary>
        /// Internet reachable HostName of server used by querying process
        /// </summary>
        public string DNS { get; private set; }
        /// <summary>
        /// Port server instance listens on
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// IP Address resolved at query time
        /// </summary>
        public string IpAddress { get; private set; }
        /// <summary>
        /// Game type of server instance
        /// </summary>
        public Game GameId { get; private set; }
        /// <summary>
        /// Public server website
        /// </summary>
        public string PublicSiteUrl { get; private set; }
        /// <summary>
        /// Public location of custom maps (if any)
        /// </summary>
        public string MapDownloadUrl { get; private set; }
        /// <summary>
        /// Specific geographic location of server
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// More generic geographic location of server
        /// </summary>
        public string Region { get; private set; }
        /// <summary>
        /// Modification Code or Type defined at setup
        /// </summary>
        public string ModificationCode { get; private set; }
        /// <summary>
        /// Grouping identifier
        /// </summary>
        public string Category { get; private set; }
        /// <summary>
        /// Current running map on server
        /// </summary>
        public string Map { get; private set; }
        /// <summary>
        /// JSON String representing Settings received from server
        /// </summary>
        public string ServerSettings { get; private set; }
        /// <summary>
        /// Modification type received from server
        /// </summary>
        public string Modification { get; private set; }
        /// <summary>
        /// Timestamp of last query
        /// </summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// Number of maximum players the server supports received from server
        /// </summary>
        public int MaxPlayers { get; private set; }
        /// <summary>
        /// Status of server from last query
        /// </summary>
        public ServerStatus CurrentStatus { get; private set; }
    }
}

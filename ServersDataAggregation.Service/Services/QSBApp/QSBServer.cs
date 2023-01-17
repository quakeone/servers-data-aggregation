using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Services.QSBApp
{
    internal class QSBServer
    {
        public int ServerId { get; set; }
        public int GameId { get; set; }
        public bool? AntiWallHack { get; set; }
        public int Port { get; set; }
        public string DNS { get; set; }
        public string? Region { get; set; }
        /// <summary>
        /// Physical location of server
        /// </summary>
        public string? Location { get; set; }
        /// <summary>
        /// Seconds between each query interval (Minimum of 10 seconds)
        /// </summary>
        public int QueryInterval { get; set; }
        public int FailedQueryAttempts { get; set; }
        public DateTime? LastQuery { get; set; }
        public int QueryResult { get; set; }
        public DateTime? NextQuery { get; set; }
        /// <summary>
        /// Last time a server was online or successfully queried
        /// </summary>
        public DateTime? LastQuerySuccess { get; set; }

        public string? MapDownloadUrl { get; set; }
        public string? PublicSiteUrl { get; set; }
        public string? ModificationCode { get; set; }
        public string? CustomName { get; set; }
        public string? CustomNameShort { get; set; }

        public int Active { get; set; }
        public string? ApiKey { get; set; }
        public string? Parameters { get; set; }

    }
}

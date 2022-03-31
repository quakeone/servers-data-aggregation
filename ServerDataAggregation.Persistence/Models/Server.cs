using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Table("server")]
    public class Server
    {
        [Column("server_id")]
        public virtual int ServerId { get; set; }
        [Column("game_id")]
        public virtual int GameId { get; set; }
        [Column("port")]
        public virtual int Port { get; set; }
        [Column("address")]
        public virtual string Address { get; set; }
        [Column("location")]
        /// <summary>
        /// Physical location of server
        /// </summary>
        public virtual string Location { get; set; }
        [Column("query_interval")]
        /// <summary>
        /// Seconds between each query interval (Minimum of 10 seconds)
        /// </summary>
        public virtual int QueryInterval { get; set; }
        [Column("failed_query_attempts")]
        public virtual int FailedQueryAttempts { get; set; }
        [Column("last_query")]
        public virtual DateTime? LastQuery { get; set; }
        [Column("query_result")]
        public virtual int QueryResult { get; set; }
        [Column("next_query")]
        public virtual DateTime? NextQuery { get; set; }
        [Column("last_query_success")]
        /// <summary>
        /// Last time a server was online or successfully queried
        /// </summary>
        public virtual DateTime? LastQuerySuccess { get; set; }

        [Column("modification_code")]
        public virtual string ModificationCode { get; set; }

        [Column("hostname")]
        public virtual string Hostname { get; set; }

        [Column("active")]
        public virtual bool Active { get; set; }

        [Column("api_key")]
        public virtual string ApiKey { get; set; }

        [Column("parameters")]
        public virtual string Parameters { get; set; }
        [Column("metadata")]
        public virtual string Metadata { get; set; }

        public override string ToString()
        {
            return Address + ":" + Port;
        }
    }
}

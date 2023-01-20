using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Table("server")]
    public class Server
    {
        [Key]
        [Column("server_id")]
        public virtual int ServerId { get; set; }
        [Column("game_id")]
        public virtual int GameId { get; set; }
        [Column("port")]
        public virtual int Port { get; set; }
        [Column("address")]
        public virtual string Address { get; set; }
        /// <summary>
        /// Country of server
        /// </summary>
        [Column("country")]
        public virtual string? Country { get; set; }
        /// <summary>
        /// Locality - location within country
        /// </summary>
        [Column("locality")]
        public virtual string? Locality { get; set; }
        [Column("query_interval")]
        /// <summary>
        /// Seconds between each query interval (Minimum of 10 seconds)
        /// </summary>
        public virtual int QueryInterval { get; set; }
        [Column("mod")]
        public virtual string? Mod { get; set; }

        [Column("active")]
        public virtual bool Active { get; set; }

        [Column("api_key")]
        public virtual string ApiKey { get; set; }

        [Column("parameters")]
        public virtual string? Parameters { get; set; }
        [Column("metadata")]
        public virtual string? Metadata { get; set; }

        public override string ToString()
        {
            return Address + ":" + Port;
        }
    }
}

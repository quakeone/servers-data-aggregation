using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Table("server_status")]
    public class ServerStatus
    {
        [Column("server_status_id")]
        [Key]
        public virtual int ServerStatusId { get; set; }
        [Column("server_id")]
        public virtual int ServerId { get; set; }
        public Server Server { get; set; }
        [Column("timestamp")]
        public virtual DateTime TimeStamp { get; set; }
        [Column("map")]
        public virtual string Map { get; set; }
        [Column("modification")]
        public virtual string Modification { get; set; }
        [Column("mode")]
        public virtual string Mode { get; set; }
        [Column("players")]
        public virtual string Players { get; set; }
        [Column("hostname")]
        public virtual string Hostname { get; set; }
        [Column("ip_address")]
        public virtual string IpAddress { get; set; }
        [Column("max_players")]
        public virtual int MaxPlayers { get; set; }
        [Column("server_settings")]
        public virtual string ServerSettings { get; set; }
    }

}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Table("server_state")]
    public class ServerState
    {
        [Key]
        [Column("server_state_id")]
        public virtual int ServerStateId { get; set; }
        [ForeignKey("server_id")]
        public virtual Server ServerDefinition { get; set; }
        /// <summary>
        /// The timestamp of this data (last query success)
        /// </summary>
        [Column("timestamp")]
        public virtual DateTime? TimeStamp { get; set; }
        [Column("hostname")]
        public virtual string? Hostname { get; set; }
        [Column("map")]
        public virtual string? Map { get; set; }
        [Column("mod")]
        public virtual string? Mod { get; set; }
        [Column("mode")]
        public virtual string? Mode { get; set; }
        [Column("ip_address")]
        public virtual string? IpAddress { get; set; }
        [Column("max_players")]
        public virtual int MaxPlayers { get; set; }
        [Column("server_settings")]
        public virtual string? ServerSettings { get; set; }
        [Column("players")]
        public List<PlayerState>? Players { get; set; }
        [Column("failed_query_attempts")]
        public virtual int FailedQueryAttempts { get; set; }
        /// <summary>
        /// The last time this server was queried, regardless of result
        /// </summary>
        [Column("last_query")]
        public virtual DateTime? LastQuery { get; set; }
        [Column("query_result")]
        public virtual int LastQueryResult { get; set; }

        public override string ToString()
        {
            if (ServerDefinition == null) {
                return "New Server";
            }
            return ServerDefinition.Address + ":" + ServerDefinition.Port;
        }
    }

    [NotMapped]
    public class PlayerState
    {
        /// <summary>
        /// Player's Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Player's Name (in base64)
        /// </summary>
        public string NameRaw { get; set; }
        /// <summary>
        /// Player's Number
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Color of shirt
        /// </summary>
        public int ShirtColor { get; set; }
        /// <summary>
        /// Color of pants
        /// </summary>
        public int PantColor { get; set; }
        /// <summary>
        /// Skin player is using
        /// </summary>
        public string? Skin { get; set; }
        /// <summary>
        /// Model Player is using
        /// </summary>
        public string? Model { get; set; }
        /// <summary>
        /// Ping as reported by the server
        /// </summary>
        public int Ping { get; set; }
        /// <summary>
        /// Current score
        /// </summary>
        public int Frags { get; set; }
        /// <summary>
        /// Total score
        /// </summary>
        public int TotalFrags { get; set; }
        /// <summary>
        /// Join Timestamp of Player
        /// </summary>
        public DateTime JoinTime { get; set; }
        /// <summary>
        /// Join Timestamp of Player
        /// </summary>
        public int PlayerType { get; set; }
    }

    public class ServerStateConfiguration : IEntityTypeConfiguration<ServerState>
    {
        public void Configure(EntityTypeBuilder<ServerState> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(e => e.Players).HasJsonConversion();
        }
    }
}


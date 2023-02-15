using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Index(nameof(TimeStamp))]
    [Table("server_snapshot")]
    public class ServerSnapshot
    {
        [Column("server_snapshot_id")]
        public int ServerSnapshotId { get; set; }
        [Column("server_id")]
        public int ServerId { get; set; }
        [Column("timestamp")]
        public virtual DateTime TimeStamp { get; set; }
        [Column("hostname")]
        public virtual string Hostname { get; set; }
        [Column("map")]
        public virtual string Map { get; set; }
        [Column("mod")]
        public virtual string? Mod { get; set; }
        [Column("mode")]
        public virtual string? Mode { get; set; }
        [Column("ip_address")]
        public virtual string IpAddress { get; set; }
        [Column("max_players")]
        public virtual int MaxPlayers { get; set; }
        [Column("server_settings")]
        public virtual string ServerSettings { get; set; }
        [Column("players")]
        public List<PlayerSnapshot> Players { get; set; }
    }

    [NotMapped]
    public class PlayerSnapshot
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
        /// Player's Name
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
        public string Skin { get; set; }
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
        /// Current server reported playing time of player
        /// </summary>
        public TimeSpan PlayTime { get; set; }
        /// <summary>
        /// Join Timestamp of Player
        /// </summary>
        public int PlayerType { get; set; }
    }

    public class ServerSnapshotConfiguration : IEntityTypeConfiguration<ServerSnapshot>
    {
        public void Configure(EntityTypeBuilder<ServerSnapshot> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(e => e.Players).HasJsonConversion();
        }
    }
}

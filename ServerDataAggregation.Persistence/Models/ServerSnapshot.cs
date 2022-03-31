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
    [Table("server_snapshot")]
    public class ServerSnapshot
    {
        [Column("server_snapshot_id")]
        public int ServerSnapshotId { get; set; }
        [Column("server_id")]
        public int ServerId { get; set; }   
        [Column("players")]
        public List<PlayerSnapshot> Players { get; set; }
    }

    public class PlayerSnapshot
    {
        /// <summary>
        /// Player's Name (text)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Players'Name (bytes)
        /// </summary>
        public byte[] NameBytes { get; set; }
        /// <summary>
        /// Skin player is using
        /// </summary>
        public string Skin { get; set; }
        /// <summary>
        /// Model Player is using
        /// </summary>
        public string Model { get; set; }
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

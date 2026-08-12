using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Index(nameof(MatchEnd), nameof(MatchStart))]
    [Table("server_match")]
    public class ServerMatch
    {
        [Key]
        [Column("server_match_id")]
        public  int ServerMatchId { get; set; }
        [ForeignKey("server_id")]
        public virtual Server Server { get; set; }
        public virtual ICollection<PlayerMatch> PlayerMatches { get; set; }
        [Column("map")]
        public string Map { get; set; }
        [Column("mod")]
        public string? Mod { get; set; }
        [Column("mode")]
        public string? Mode { get; set; }
        [Column("timelimit")]
        public int Timelimit { get; set; }
        [Column("fraglimit")]
        public int Fraglimit { get; set; }
        [Column("match_start")]
        public DateTime MatchStart { get; set; }
        [Column("match_end")]
        public DateTime? MatchEnd { get; set; }
    }

    public class ServerMatchConfiguration : IEntityTypeConfiguration<ServerMatch>
    {
        public void Configure(EntityTypeBuilder<ServerMatch> builder)
        {
            // Declared explicitly: the filtered index below would otherwise suppress
            // EF's conventional FK index, which cascade deletes still need.
            builder.HasIndex(new[] { "server_id" }, "IX_server_match_server_id");

            // Serves the open-match lookup done on every poll; ~220 open of 83k rows.
            builder.HasIndex(new[] { "server_id" }, "IX_server_match_server_id_open")
                .HasFilter("match_end IS NULL");
        }
    }
}

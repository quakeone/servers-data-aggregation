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
}

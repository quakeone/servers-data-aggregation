using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Table("player_match_progress")]
    public class PlayerMatchProgress
    {
        [Key]
        [Column("player_match_progress_id")]
        public int PlayerMatchProgressId { get; set; }
        [ForeignKey("player_match_id")]
        public virtual PlayerMatch PlayerMatch { get; set; }
        [ForeignKey("server_match_id")]
        public virtual ServerMatch ServerMatch { get; set; }
        [Column("timestamp")]
        public virtual DateTime Timestamp { get; set; }
        [Column("shirt_color")]
        public virtual int ShirtColor { get; set; }
        [Column("pant_color")]
        public virtual int PantColor { get; set; }
        [Column("model")]
        public virtual string? Model { get; set; }
        [Column("skin")]
        public virtual string? Skin { get; set; }
        [Column("frags")]
        public virtual int Frags { get; set; }
    }
}

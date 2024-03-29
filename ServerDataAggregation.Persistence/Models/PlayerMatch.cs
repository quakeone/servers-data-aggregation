﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence.Models
{
    [Index(nameof(NameRaw))]
    [Index(nameof(Frags))]
    [Index(nameof(PlayerMatchEnd), nameof(PlayerMatchStart))]
    [Table("player_match")]
    public class PlayerMatch
    {
        [Key]
        [Column("player_match_id")]
        public int PlayerMatchId { get; set; }
        [ForeignKey("server_match_id")]
        public virtual ServerMatch ServerMatch { get; set; }
        [Column("name")]
        public virtual string Name { get; set; }
        [Column("name_raw")]
        public virtual string NameRaw { get; set; }
        [Column("number")]
        public virtual int Number { get; set; }
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
        [Column("type")]
        public virtual int Type { get; set; }
        [Column("player_match_start")]
        public virtual DateTime? PlayerMatchStart { get; set; }
        [Column("player_match_end")]
        public virtual DateTime? PlayerMatchEnd { get; set; }
    }
}

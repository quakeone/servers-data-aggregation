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
    [Index(nameof(TimeStamp))]
    [Table("server_match_actual")]
    public class ServerMatchActual
    {
        [Key]
        [Column("server_match_actual_id")]
        public int ServerMatchActualId { get; set; }
        [Column("server_id")]
        public int ServerId { get; set; }
        [Column("timestamp")]
        public virtual DateTime TimeStamp { get; set; }
        [Column("payload")]
        public virtual string Payload { get; set; }
    }
}
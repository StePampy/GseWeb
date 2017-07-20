using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("v_work_calculate")]
    public class HourResult : IHour
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Break { get; set; }
        public TimeSpan Travel { get; set; }

        [ForeignKey("WorkType")]
        public int WorkTypeId { get; set; }
        public string Note { get; set; }
        public bool OffSite { get; set; }
        public TimeSpan WorkTime { get; set; }
        public TimeSpan OrdinaryHour { get; set; }
        public TimeSpan DiffTime { get; set; }
        public bool Festivity { get; set; }

        public virtual WorkType WorkType { get; set; }

        [NotMapped]
        public virtual HourResult Results { get; set; }
    }
}
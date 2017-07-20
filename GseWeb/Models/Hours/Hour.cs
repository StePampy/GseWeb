using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("work_hours")]
    public class Hour : IHour
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Results")]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Results")]
        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Break { get; set; }
        public TimeSpan Travel { get; set; }

        [ForeignKey("WorkType")]
        public int WorkTypeId { get; set; }
        public string Note { get; set; }
        public bool OffSite { get; set; }

        [NotMapped]
        public TimeSpan WorkTime { get; set; }
        [NotMapped]
        public TimeSpan OrdinaryHour { get; set; }
        [NotMapped]
        public TimeSpan DiffTime { get; set; }
        [NotMapped]
        public bool Festivity { get; set; }

        public virtual WorkType WorkType { get; set; }

        public virtual HourResult Results { get; set; }
    }
}
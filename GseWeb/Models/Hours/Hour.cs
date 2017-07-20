using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("work_hours")]
    public class Hour
    {
        [NotMapped]
        public long DateTicks { get { return Date.Ticks; } set { Date = new DateTime(value); } }

        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Break { get; set; }
        public TimeSpan Travel { get; set; }

        public WorkType WorkType { get; set; }
        public string Note { get; set; }
        public bool OffSite { get; set; }

        [NotMapped]
        public TimeSpan WorkTime { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Account
{
    [Table("hours_profile")]
    public class Profile
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Display(Name = "Profilo Ore")]
        public string Description { get; set; }
        public TimeSpan Mon { get; set; }
        public TimeSpan Tue { get; set; }
        public TimeSpan Wed { get; set; }
        public TimeSpan Thu { get; set; }
        public TimeSpan Fri { get; set; }
        public TimeSpan Sat { get; set; }
        public TimeSpan Sun { get; set; }
        public bool CostDaily { get; set; }
        public TimeSpan ExtraWeek { get; set; }
        public TimeSpan ExtraYear { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
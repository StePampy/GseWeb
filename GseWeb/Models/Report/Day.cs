using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GseWeb.Models.Hours;

namespace GseWeb.Models.Report
{
    public class Day
    {
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        public bool Festivity { get; set; }
        public IHour Hour { get; set; }
        public Orders.OrderVM OrderVM { get; set; }
    }
}
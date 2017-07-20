using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Holiday
{
    public class HolidayDay
    {
        public int Day { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Holiday Holiday { get; set; }
    }
}
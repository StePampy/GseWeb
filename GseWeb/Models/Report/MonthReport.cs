using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Report
{
    public class MonthReport
    {
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<Day> Dates { get; set; }
        public IEnumerable<MonthTotal> Totals { get; set; }
    }
}
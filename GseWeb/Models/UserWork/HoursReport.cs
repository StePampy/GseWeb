using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.UserWork
{
    public class HoursReport
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
    }
}
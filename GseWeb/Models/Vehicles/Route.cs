using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Vehicles
{
    public class Route
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Km { get; set; }
    }
}
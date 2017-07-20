using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Vehicles
{
    public class VehicleVM
    {
        public Account.User User { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public UserVehicle[] UserVehicles { get; set; }
        public WorkVehicle[] WorkVehicles { get; set; }
        public int Km_Total { get; set; }
        public double Cost_Total { get; set; }
    }
}
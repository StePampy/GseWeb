using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Report
{
    public class ReportLabor
    {
        public Account.User User { get; set; }
        public Orders.Order Order { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public ReportLabor_Day[] Days
        {
            get
            {
                var wOrders = Order.WorkOrders.Where(x => x.UserId == User.UserId && x.Date.Year == Year && x.Date.Month == Month).ToArray();
                var wVehicles = Order.WorkVehicles.Where(x => x.UserId == User.UserId && x.Date.Year == Year && x.Date.Month == Month).ToArray();
                var Dates = wOrders.Select(x => x.Date).Union(wVehicles.Select(x => x.Date)).ToArray();
                return Dates.Select(x => new ReportLabor_Day
                {
                    Date = x,
                    WorkOrder = wOrders.FirstOrDefault(o => o.Date == x),
                    WorkVehicle = wVehicles.FirstOrDefault(v => v.Date == x)
                })
                .ToArray();
            }
        }
    }

    public class ReportLabor_Day
    {
        public DateTime Date { get; set; }
        public Orders.WorkOrder WorkOrder { get; set; }
        public Vehicles.WorkVehicle WorkVehicle { get; set; }

        // Expense
    }
}
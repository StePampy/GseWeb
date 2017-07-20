using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Report
{
    public class ReportWorkOrder
    {
        public Account.User User { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Orders.WorkOrder[] WorkOrders { get; set; }
    }
}
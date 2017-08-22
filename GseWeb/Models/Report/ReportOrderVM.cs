using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GseWeb.Models.Orders;
using GseWeb.Models.Materials;

namespace GseWeb.Models.Report
{
    public class ReportOrderVM
    {
        public ReportOrderDetails Order { get; set; }
        public IEnumerable<ReportOrderLabor_User> Users { get; set; }
        public IEnumerable<ReportOrderLabor_Month> Months { get; set; }
        public IEnumerable<Material> Materials { get; set; }
        public static ReportOrderVM GetFromOrder(Order order)
        {
            return new ReportOrderVM
            {
                Order = ReportOrderDetails.FromOrder(order),
                Users = ReportOrderLabor_User.GetFromOrder(order),
                Months = ReportOrderLabor_Month.GetFromOrder(order),
                Materials = order.Materials.ToArray().OrderBy(x => x.Supplier).ThenByDescending(x => x.Date),
            };
        }
    }



    public class ReportOrderLabor_Month
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public TimeSpan HoursApproved { get; set; }
        public TimeSpan HoursNotApproved { get; set; }
        public double Cost_Hours { get; set; }
        public double Cost_Vehicles { get; set; }
        public double Cost_Expenses { get; set; }
        public IEnumerable<ReportOrderLabor_User> Users { get; set; }

        public static IEnumerable<ReportOrderLabor_Month> GetFromOrder(Order order)
        {
            var YearsMonths = order.WorkOrders.Select(x => new { Year = x.Date.Year, Month = x.Date.Month })
                .Union(order.WorkVehicles.Select(x => new { Year = x.Date.Year, Month = x.Date.Month }));
            return YearsMonths.Select(x => {
                var rep = new ReportOrderLabor_Month();
                rep.Year = x.Year;
                rep.Month = x.Month;
                rep.Users = ReportOrderLabor_User.GetFromOrder(order, x.Year, x.Month);
                rep.HoursApproved = TimeSpan.FromSeconds(rep.Users.Sum(u => u.HoursApproved.TotalSeconds));
                rep.HoursNotApproved = TimeSpan.FromSeconds(rep.Users.Sum(u => u.HoursNotApproved.TotalSeconds));
                rep.Cost_Hours = rep.Users.Sum(u => u.Cost_Hours);
                rep.Cost_Vehicles = rep.Users.Sum(u => u.Cost_Vehicles);
                rep.Cost_Expenses = rep.Users.Sum(u => u.Cost_Expenses);
                return rep;
            })
            .ToArray()
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month);
        }
    }

    

    public class ReportOrderLabor_User
    {
        public string UserId { get; set; }
        public string UserInfo { get; set; }
        public TimeSpan HoursApproved { get; set; }
        public TimeSpan HoursNotApproved { get; set; }
        public double Cost_Hours { get; set; }
        public double Cost_Vehicles { get; set; }
        public double Cost_Expenses { get; set; }

        public static IEnumerable<ReportOrderLabor_User> GetFromOrder(Order order, int Year = 0, int Month = 0)
        {
            var wOrders = (Year == 0 || Month == 0) ? order.WorkOrders : order.WorkOrders.Where(x => x.Date.Year == Year && x.Date.Month == Month).ToArray();
            var wVehicles = (Year == 0 || Month == 0) ? order.WorkVehicles : order.WorkVehicles.Where(x => x.Date.Year == Year && x.Date.Month == Month).ToArray();
            var users = wOrders.Select(x => x.User).Union(wVehicles.Select(x => x.User)).ToArray();

            return users.Select(u => {
                var rep = new ReportOrderLabor_User();
                rep.UserId = u.UserId;
                rep.UserInfo = u.LastName + " " + u.FirstName;
                var woUsr = wOrders.Where(wo => wo.UserId == u.UserId).ToArray();
                rep.HoursApproved = TimeSpan.FromSeconds(woUsr.Where(x => x.Approved).Sum(wo => wo.Hours.TotalSeconds));
                rep.HoursNotApproved = TimeSpan.FromSeconds(woUsr.Where(x => ! x.Approved).Sum(wo => wo.Hours.TotalSeconds));
                rep.Cost_Hours = woUsr.Where(x => x.Approved).Sum(x => x.Cost);
                rep.Cost_Vehicles = wVehicles.Where(wv => wv.UserId == u.UserId && wv.Approved).Sum(wv => wv.Cost_Total);
                rep.Cost_Expenses = 0;
                return rep;
            }).ToArray()
            .OrderBy(x => x.UserInfo);
        }
    }
}
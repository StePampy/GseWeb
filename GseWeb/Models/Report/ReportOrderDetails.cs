using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GseWeb.Models.Orders;

namespace GseWeb.Models.Report
{
    public class ReportOrderDetails
    {
        [Display(Name = "Commessa")]
        public string OrderId { get; set; }

        [Display(Name = "Cliente")]
        public string Client { get; set; }

        [Display(Name = "Descrizione")]
        public string Description { get; set; }

        [Display(Name = "Responsabile")]
        public string TeamLeaderInfo { get; set; }

        [Display(Name = "Valore Commessa")]
        public double ExpectedCost { get; set; }

        [Display(Name = "Costo Totale")]
        public double Cost_Total { get; set; }

        [Display(Name = "Costo Ore")]
        public double Cost_Hours { get; set; }

        [Display(Name = "Rimborsi Chilometrici")]
        public double Cost_Vehicles { get; set; }

        [Display(Name = "Rimborsi Spese")]
        public double Cost_Expenses { get; set; }

        [Display(Name = "Costo Manodopera")]
        public double Cost_Labor { get; set; }

        [Display(Name = "Costi Vari")]
        public double Cost_Various { get; set; }

        [Display(Name = "Costo Materiali")]
        public double Cost_Materials { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Scadenza")]
        public DateTime FinishDate { get; set; }

        [Display(Name = "Utenti")]
        public string[] UsersInfo { get; set; }

        public static ReportOrderDetails[] FromOrders(Order[] orders)
        {
            return orders.Select(x => FromOrder(x)).ToArray();
        }

        public static ReportOrderDetails FromOrder(Order order)
        {
            var details = new ReportOrderDetails
            {
                OrderId = order.OrderId,
                Client = order.Client,
                Description = order.Description,
                ExpectedCost = order.Cost,
                Cost_Hours = order.WorkOrders.Where(o => o.UserId == order.TeamLeaderId || o.UserApprove != null).Sum(o => o.Cost),
                Cost_Vehicles = order.WorkVehicles.Where(v => v.UserId == order.TeamLeaderId || v.UserApprove != null).Sum(v => v.Cost_Total),
                Cost_Expenses = 0,
                Cost_Materials = order.Materials.Sum(m => m.Total_Used),
                Cost_Various = 0,
                FinishDate = order.FinishDate,
                TeamLeaderInfo = order.TeamLeader.LastName + ' ' + order.TeamLeader.FirstName,
                UsersInfo = order.Users.Where(x => x.UserId != order.TeamLeaderId).Select(x => x.User.LastName + " " + x.User.FirstName).OrderBy(x => x).ToArray(),
            };
            // calcolo costo totale
            details.Cost_Labor = details.Cost_Hours + details.Cost_Vehicles + details.Cost_Expenses;
            details.Cost_Total = details.Cost_Labor + details.Cost_Materials + details.Cost_Various;
            return details;
        }
    }
}
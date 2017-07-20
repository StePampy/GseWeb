using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    public class OrderVM
    {
        public long DateTicks { get; set; }
        public string UserId { get; set; }

        public WorkOrder AddOrder { get; set; }
        public WorkOrder EditOrder { get; set; }
        public IEnumerable<WorkOrder> Orders { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore Disponibili")]
        public TimeSpan AvaiableHours { get; set; }

        public static OrderVM Get(DateTime Date, string UserId)
        {
            var db = new DAL.GestionaleDB();

            // View Model dell'ordine
            var ord = new OrderVM();
            ord.DateTicks = Date.Date.Ticks;
            ord.UserId = UserId;
            ord.EditOrder = null;
            

            // Ordini di oggi configurati
            ord.Orders = db.WorkOrders.Where(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(Date)).ToArray();

            // Ore configurate
            var hrcfg = new TimeSpan(ord.Orders.Sum(x => x.Hours.Ticks));

            // Giornata segnata
            var day = new TimeSpan(0);//db.HourResults.Find(UserId, Date);

            // Sommo ore totali giornata con ore viaggio
            var hrday = new TimeSpan(0);//day.WorkTime.Add(day.Travel);

            // Ore disponibili
            ord.AvaiableHours = hrday.Subtract(hrcfg);

            // Creo un nuovo add order e gli metto a disposizione le ore disponibili
            ord.AddOrder = new WorkOrder() { Date = Date, UserId = UserId, Hours = ord.AvaiableHours };

            return ord;
        }
    }
}
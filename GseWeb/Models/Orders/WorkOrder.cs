using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    [Table("work_orders")]
    public class WorkOrder
    {
        [NotMapped]
        public long DateTicks { get { return Date.Ticks; } set { Date = new DateTime(value); } }

        [Key, Column(Order = 0)]
        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [Display(Name = "Commessa")]
        public string OrderId { get; set; }

        [Key, Column(Order = 2)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore")]

        public TimeSpan Hours { get; set; }

        public double Cost { get; set; }

        [Display(Name = "Utente Approvatore")]
        public string UserApprove { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("UserId")]
        public virtual Account.User User { get; set; }

        [NotMapped]
        [Display(Name = "Approvazione")]
        public bool Approved
        {
            get
            {
                return (Order != null && Order.TeamLeaderId == UserId)? true : (UserApprove != null) ? true : false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    [Table("users_orders")]
    public class UserOrder
    {
        [Key, Column(Order = 0)]
        [Display(Name = "Commessa")]
        [ForeignKey("Order")]
        public string OrderId { get; set; }

        [Key, Column(Order = 1)]
        [Display(Name = "Utente")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Account.User User { get; set; }
    }
}
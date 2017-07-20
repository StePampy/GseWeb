using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    public class UsersOrderVM
    {
        [Display(Name = "Commessa")]
        public string OrderId { get; set; }
        public string OrderClient { get; set; }
        public string OrderDescription { get; set; }
        public IEnumerable<UserOrderVM> Users { get; set; }
    }
}
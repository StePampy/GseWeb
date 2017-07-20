using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    public class UserOrderVM
    {
        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [Display(Name = "Utente")]
        public string UserInfo { get; set; }
        [Display(Name = "Seleziona")]
        public bool Selected { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    public class ApproveHour
    {
        [Display(Name = "Commessa")]
        public string OrderId { get; set; }

        [Display(Name = "Cliente")]
        public string OrderClient { get; set; }

        [Display(Name = "Descrizione")]
        public string OrderDescription { get; set; }

        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [Display(Name = "Utente")]
        public string UserInfo { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Ore")]
        [DataType(DataType.Time)]
        public TimeSpan Hours { get; set; }
    }
}

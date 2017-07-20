using GseWeb.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Orders
{
    [Table("orders")]
    public class Order
    {
        [Key, Column(Order = 0)]
        [Required(ErrorMessage = "Please Enter OrderId")]
        [Display(Name = "Commessa")]
        [RegularExpression(@"^[^\s\,]*$", ErrorMessage = "Il codice commessa non può contenere spazi")]
        public string OrderId { get; set; }

        [Required(ErrorMessage = "Please Enter Client")]
        [Display(Name = "Cliente")]
        public string Client { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        [Display(Name = "Descrizione")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter Hours")]
        [Display(Name = "Ore")]
        //[DataType(DataType.Time)]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Inserire Costo")]
        [Display(Name = "Costo Previsto €")]
        public int Cost { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Finish Date")]
        [Display(Name = "Data Scadenza")]
        public DateTime FinishDate { get; set; }

        [Required(ErrorMessage = "Please select Team Leader")]
        [Display(Name = "Responsabile")]
        public string TeamLeaderId { get; set; }

        [Display(Name = "Chiusa")]
        public bool Closed { get; set; }

        [ForeignKey("TeamLeaderId")]
        public virtual Account.User TeamLeader { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<UserOrder> Users { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<WorkVehicle> WorkVehicles { get; set; }
    }
}
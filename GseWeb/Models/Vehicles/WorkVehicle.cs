using GseWeb.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Vehicles
{
    [Table("work_vehicles")]
    public class WorkVehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [Display(Name = "Commessa")]
        [Required(ErrorMessage = "Selezionare Commessa")]
        public string OrderId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Inserire Data")]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Inserire Partenza")]
        [Display(Name = "Partenza")]
        public string From { get; set; }

        [Required(ErrorMessage = "Inserire Destinazione")]
        [Display(Name = "Destinazione")]
        public string To { get; set; }

        [Required(ErrorMessage = "Inserire Km")]
        [Display(Name = "Chilometri")]
        public int Km { get; set; }

        [Display(Name = "Tragitto Casa")]
        public bool StartHome { get; set; }

        [Display(Name = "Veicolo")]
        public string VehicleDescription { get; set; }

        [Display(Name = "Casa/GSE")]
        public int KmHomeWork { get; set; }

        [Display(Name = "Costo Km")]
        public double Cost_Km { get; set; }

        [Display(Name = "Andata Ritorno")]
        public bool RoundTrip { get; set; }

        [Display(Name = "Utente Approvazione")]
        public string UserApprove { get; set; }

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
                return (UserApprove != null) ? true : false;
            }
        }

        [NotMapped]
        [Display(Name = "Totale Km")]
        public int Km_Total
        {
            get
            {
                var km = (StartHome) ? Km - KmHomeWork : Km;
                return (RoundTrip) ? km * 2 : km;
            }
        }

        [NotMapped]
        [Display(Name = "Totale Costo")]
        public double Cost_Total
        {
            get
            {
                return (!Approved) ? 0 : Km_Total * Cost_Km;
            }
        }

        [NotMapped]
        [Display(Name = "Totale Costo")]
        public double Cost_Total_All
        {
            get
            {
                return Km_Total * Cost_Km; ;
            }
        }
    }
}
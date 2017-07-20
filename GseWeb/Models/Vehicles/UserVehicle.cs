using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Vehicles
{
    [Table("users_vehicles")]
    public class UserVehicle
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [Display(Name = "Veicolo")]
        [Required(ErrorMessage = "Inserire Veicolo")]
        public string VehicleDescription { get; set; }

        [Display(Name = "Costo Km")]
        [Required(ErrorMessage = "Inserire Costo")]
        public double Cost { get; set; }
        [Display(Name = "Km Casa/GSE")]
        [Required(ErrorMessage = "Inserire Km Casa/GSE")]
        public int KmHomeWork { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
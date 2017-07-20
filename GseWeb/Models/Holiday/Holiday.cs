using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Holiday
{
    [Table("holidays")]
    public class Holiday
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Utente")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Giorno Inizio (Compreso)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date_From { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Giorno Fine (Compreso)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date_To { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ora Inizio (Compresa)")]
        public TimeSpan Time_From { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ora Fine (Compresa)")]
        public TimeSpan Time_To { get; set; }

        [Display(Name = "Tipologia")]
        public Holiday_Type Type { get; set; }

        [Display(Name = "Note")]
        public string Request_Note { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Richiesta")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Request_Date { get; set; }

        [Display(Name = "Stato Richesta")]
        public Status_Request Status { get; set; }

        [Display(Name = "Utente Approvazione")]
        public string UserApproveId { get; set; }

        [Display(Name = "Nota")]
        public string Response_Note { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Risposta")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Response_Date { get; set; }

        [ForeignKey("UserId")]
        public virtual Account.User User { get; set; }

        [ForeignKey("UserApproveId")]
        public virtual Account.User UserApprove { get; set; }


        [NotMapped]
        public double Days
        {
            get
            {
                return (Date_To - Date_From).TotalDays +1;
            }
        }

        [NotMapped]
        public TimeSpan Hours
        {
            get
            {
                return Time_To.Subtract(Time_From);
            }
        }
    }

    public enum Holiday_Type
    {
        Ferie,
        Permesso
    }

    public enum Status_Request
    {
        Attesa,
        Approvata,
        Negata
    }
}
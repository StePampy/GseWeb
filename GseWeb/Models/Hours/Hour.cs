using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("work_hours")]
    public class Hour
    {
        [NotMapped]
        public long DateTicks { get { return Date.Ticks; } set { Date = new DateTime(value); } }

        [Key, Column(Order = 0)]
        [Display(Name = "Utente")]
        [Required(ErrorMessage = "Utente non puo essere nullo")]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [Required(ErrorMessage = "Inserire data")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Inizio")]
        [Required(ErrorMessage = "Inserire Inizio")]
        public TimeSpan Start { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Fine")]
        [Required(ErrorMessage = "Inserire Fine")]
        public TimeSpan End { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Pausa")]
        [Required(ErrorMessage = "Inserire Pausa")]
        public TimeSpan Break { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore Viaggio")]
        [Required(ErrorMessage = "Inserire Ore Viaggio")]
        public TimeSpan Travel { get; set; }

        [Display(Name = "Causale")]
        [Required(ErrorMessage = "Selezioneare Causale")]
        public WorkType WorkType { get; set; }

        //[DataType(DataType.Text)]
        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Trasferta")]
        public bool OffSite { get; set; }

        [NotMapped]
        [DataType(DataType.Time)]
        [Display(Name = "Totale Lavorativo")]
        public TimeSpan WorkTime { get; set; }
    }
}
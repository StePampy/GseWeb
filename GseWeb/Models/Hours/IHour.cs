using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GseWeb.Models.Hours
{
    public interface IHour
    {
        [Display(Name = "Utente")]
        string UserId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Inizio")]
        TimeSpan Start { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Fine")]
        TimeSpan End { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Pausa")]
        TimeSpan Break { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore Viaggio")]
        TimeSpan Travel { get; set; }

        [Display(Name = "Causale")]
        int WorkTypeId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Note")]
        string Note { get; set; }

        [Display(Name = "Trasferta")]
        bool OffSite { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Totale Lav.")]
        TimeSpan WorkTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore Ordinarie")]
        TimeSpan OrdinaryHour { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Differenza")]
        TimeSpan DiffTime { get; set; }

        [Display(Name = "Festività")]
        bool Festivity { get; set; }

        WorkType WorkType { get; set; }
        HourResult Results { get; set; }
    }
}

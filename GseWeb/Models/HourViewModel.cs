using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GseWeb.Models
{
    public class MonthReport
    {
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<DateReport> Dates { get; set; }
        public IEnumerable<MonthTotal> Totals { get; set; }
    }

    public class MonthTotal
    {
        public int WorkTypeId { get; set; }
        [Display(Name = "Lavoro")]
        public string Description { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Valore")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH\\:mm}")]
        public TimeSpan Value { get; set; }
    }

    public class DateReport
    {

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        public bool Festivity { get; set; }
        public Hour Hour { get; set; }
    }

    public class Festivity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public class Hour
    {
        public Hour()
        {
            var worktypes = DAL.Hours.GetWorkTypes();
            WorkTypeList = worktypes.Select(x => new System.Web.Mvc.SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Description
            });
        }


        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        public long DateTicks { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Inizio")]
        public TimeSpan Start { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Fine")]
        public TimeSpan End { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Pausa")]
        public TimeSpan Break { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ore Viaggio")]
        public TimeSpan Travel { get; set; }

        [Display(Name = "Causale")]
        public int WorkTypeId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Trasferta")]
        public bool OffSite { get; set; }
        public HourResult Results { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> WorkTypeList { get; set; }

        [Display(Name = "Causale")]
        public string WorkTypeDesc { get { return WorkTypeList.Where(x => x.Value == WorkTypeId.ToString()).Select(x => x.Text).FirstOrDefault(); } }
    }

    public class HourResult
    {
        [DataType(DataType.Time)]
        [Display(Name = "Totale Lav.")]
        public TimeSpan WorkTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Differenza")]
        public TimeSpan DiffTime { get; set; }
    }
}
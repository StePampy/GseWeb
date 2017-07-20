using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GseWeb.Models.Hours;

namespace GseWeb.Models.Report
{
    public class MonthTotal
    {
        public int WorkTypeId { get; set; }

        [Display(Name = "Lavoro")]
        public string Description { get; set; }

        public int DaysCount { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Valore")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH\\:mm}")]
        public TimeSpan Value { get; set; }


        public static MonthTotal[] Calculate(HourResult[] hours)
        {
            // Ore totali mese
            var tmpTOT = new TimeSpan(hours.Sum(x => x.WorkTime.Ticks));

            // Ore totali Viaggio 
            var tmpTRAV = new TimeSpan(hours.Sum(x => x.Travel.Ticks));

            // Altri gruppi
            var tmpGRP = hours.Where(x => x.WorkTypeId > 1)
                .GroupBy(x => x.WorkType)
                .Select(x => new MonthTotal
                {
                    WorkTypeId = x.Key.Id,
                    Description = x.Key.Description,
                    DaysCount = x.Count(),
                    Value = new TimeSpan(Math.Abs(x.Sum(h => h.DiffTime.Ticks)))
                }).ToList();

            // Ore totali straordinario
            var tmpEXTRA = tmpGRP.FirstOrDefault(x => x.WorkTypeId == 2)?.Value ?? new TimeSpan();

            // Ore ordinarie
            var tmpORD = tmpTOT.Subtract(tmpEXTRA);

            // Aggiungo ordinario
            tmpGRP.Add(new MonthTotal { WorkTypeId = 0, DaysCount = 0, Description = "Ordinario", Value = tmpORD });

            // Aggiungo totale
            tmpGRP.Add(new MonthTotal { WorkTypeId = 1000, DaysCount = 0, Description = "Totale Lavorativo", Value = tmpTOT });

            // Aggiungo i viaggi se ci sono
            if (tmpTRAV > new TimeSpan())
                tmpGRP.Add(new MonthTotal { WorkTypeId = 999, DaysCount = 0, Description = "Totale Viaggio", Value = tmpTRAV });

            return tmpGRP.OrderBy(x => x.WorkTypeId).ToArray();
        }
    }
}
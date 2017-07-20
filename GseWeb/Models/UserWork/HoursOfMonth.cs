using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.UserWork
{
    public class HoursOfMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public HoursOfYear HoursOfYear { get; set; }
        public Account.User User { get { return HoursOfYear.User; } }
        public IEnumerable<HoursOfDay> Hours { get { return HoursOfYear.Hours.Where(x => x.Date.Month == Month); } }

        public TimeSpan Amount
        {
            get
            {
                return TimeSpan.FromSeconds(Hours.Sum(x => x.AmountFestivity.TotalSeconds) + Hours.Sum(x => x.AmountWorking.TotalSeconds));
            }
        }

        public HoursReport[] Report { get; set; }

        public HoursOfMonth(string UserId, int Year, int Month)
        {
            this.Year = Year;
            this.Month = Month;
            this.HoursOfYear = new HoursOfYear(UserId, Year);

            var report = Hours.Where(x => x.WorkTypeRegular != WorkType.Default && x.WorkTypeRegular != WorkType.Lavoro && x.WorkTypeRegular != WorkType.Straordinario && x.WorkTypeRegular != WorkType.Viaggio)
                    .GroupBy(x => x.WorkTypeRegular)
                    .Select(x => new HoursReport
                    {
                        Id = (int)x.Key,
                        Description = x.Key.Description(),
                        Value = GetReport(x)
                    }).ToList();
            // Aggiungo ordinario previsto
            report.Add(new HoursReport
            {
                Id = 0,
                Description = "Ordinario Previsto",
                Value = TimeSpan.FromSeconds(Hours.Sum(x => x.OrdinaryRegular.TotalSeconds))
            });
            // Aggiungo ordinario effettuato
            report.Add(new HoursReport
            {
                Id = 1,
                Description = "Ordinario Svolto",
                Value = TimeSpan.FromSeconds(Hours.Sum(x => x.WorkTimeRegular.TotalSeconds))
            });
            // Aggiungo Straordinario
            if (Hours.Any(x => x.ExtraRegular > new TimeSpan(0)))
            {
                report.Add(new HoursReport
                {
                    Id = 2,
                    Description = "Straordinario",
                    Value = TimeSpan.FromSeconds(Hours.Sum(x => x.ExtraRegular.TotalSeconds))
                });
            }
            
            // Aggiungo le Presenze
            report.Add(new HoursReport
            {
                Id = 997,
                Description = "Presenze",
                Value = Hours.Count(x => x.Presence)
            });
            // Aggiungo la Trasferta
            var trasf = Hours.Where(x => x.OffSite);
            if (trasf.Any())
            {
                report.Add(new HoursReport
                {
                    Id = 997,
                    Description = "Trasferta",
                    Value = trasf.Count()
                });
            }
            // Aggiungo Viaggio
            if (Hours.Any(x => x.OrdersTimeTravel > new TimeSpan(0)))
            {
                report.Add(new HoursReport
                {
                    Id = 999,
                    Description = "Viaggio",
                    Value = TimeSpan.FromSeconds(Hours.Sum(x => x.OrdersTimeTravel.TotalSeconds))
                });
            }
            Report = report.OrderBy(x => x.Id).ToArray();
        }

        private static TimeSpan GetReport(IGrouping<WorkType, HoursOfDay> values)
        {
            switch (values.Key)
            {
                case WorkType.Ferie:
                case WorkType.PermessoRetribuito:
                    return TimeSpan.FromSeconds(values.Sum(x => x.HolidayTime.TotalSeconds));
                case WorkType.NonGiustificato:
                case WorkType.PermessoNonRetribuito:
                case WorkType.Malattia:
                case WorkType.Infortunio:
                case WorkType.Lutto:
                case WorkType.Recupero:
                    return TimeSpan.FromSeconds(values.Sum(x => x.LessTime.TotalSeconds));
                case WorkType.NoCommesse:
                    return TimeSpan.FromSeconds(values.Sum(x => x.WorkTime.TotalSeconds));
                case WorkType.NoApproved:
                    return TimeSpan.FromSeconds(values.Sum(x => x.OrdersTimeComplete.TotalSeconds) - values.Sum(x => x.OrdersTimeApproved.TotalSeconds));
                default:
                    return new TimeSpan(0);
            }
        }
    }
}
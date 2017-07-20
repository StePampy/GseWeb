using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.UserWork
{
    public class HoursOfYear
    {
        public Account.User User { get; set; }
        public int Year { get; set; }
        public IEnumerable<HoursOfWeek> HoursOfWeek { get; set; }
        public TimeSpan ExtraExpected { get; set; }
        public TimeSpan ExtraDone
        {
            get
            {
                return TimeSpan.FromSeconds(HoursOfWeek.Sum(x => x.ExtraDone.TotalSeconds));
            }
        }

        public TimeSpan AmountWorking
        {
            get
            {
                return TimeSpan.FromSeconds(HoursOfWeek.Sum(x => x.AmountWorking.TotalSeconds));
            }
        }

        public TimeSpan AmountFestivity
        {
            get
            {
                return TimeSpan.FromSeconds(HoursOfWeek.Sum(x => x.AmountFestivity.TotalSeconds));
            }
        }

        public IEnumerable<HoursOfDay> Hours
        {
            get
            {
                return HoursOfWeek.SelectMany(x => x.Hours);
            }
        }

        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        public HoursOfYear(string UserId, int Year)
        {
            this.Year = Year;
            this.User = db.Users.Find(UserId);
            this.ExtraExpected = User.HoursProfile.ExtraYear;
            var ExtraWeek = User.HoursProfile.ExtraWeek;

            // Raghruppo le ore in base alla settimana
            this.HoursOfWeek = db.UserWork(Year, UserId)
                .GroupBy(x => x.Week)
                .Select(x => new HoursOfWeek
                {
                    Week = x.Key,
                    Hours = x,
                    ExtraExpected = ExtraWeek
                });
            // Calcoli delle ore rimaneti di straordinario
            foreach (var w in HoursOfWeek)
            {
                foreach (var h in w.Hours)
                {
                    var exYear = ExtraExpected - ExtraDone;
                    var exWeek = w.ExtraExpected - w.ExtraDone;
                    var exExp = (h.IsFestivity || h.Date.DayOfWeek == DayOfWeek.Sunday || h.Date.DayOfWeek == DayOfWeek.Saturday) ? new TimeSpan(8, 0, 0) : new TimeSpan(2, 0, 0);
                    // Verifico di non aver superato la settimana
                    exExp = (exExp > exWeek) ? exWeek : exExp;
                    // Verifico di non aver superato l'anno
                    exExp = (exExp > exYear) ? exYear : exExp;
                    // Setto Lo straordinario previsto
                    h.ExtraExpected = exExp;
                }
            }

        }

    }
}
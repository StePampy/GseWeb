using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.UserWork
{
    public class HoursOfWeek
    {
        public int Week { get; set; }
        public IEnumerable<HoursOfDay> Hours { get; set; }
        public TimeSpan ExtraExpected { get; set; }
        public TimeSpan ExtraDone
        {
            get
            {
                return TimeSpan.FromSeconds(Hours.Sum(x => x.ExtraRegular.TotalSeconds));
            }
        }

        public TimeSpan AmountWorking
        {
            get
            {
                return TimeSpan.FromSeconds(Hours.Sum(x => x.AmountWorking.TotalSeconds));
            }
        }

        public TimeSpan AmountFestivity
        {
            get
            {
                return TimeSpan.FromSeconds(Hours.Sum(x => x.AmountFestivity.TotalSeconds));
            }
        }
    }
}
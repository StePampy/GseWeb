﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
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

        public TimeSpan ExtraAmount
        {
            get
            {
                return TimeSpan.FromSeconds(Hours.Sum(x => x.ExtraAmount.TotalSeconds));
            }
        }
    }
}
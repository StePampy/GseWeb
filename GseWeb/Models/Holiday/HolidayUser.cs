using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Holiday
{
    public class HolidayYear_User
    {
        public int Year { get; set; }
        public Account.User User { get; set; }
        public Holiday[] Holidays { get; set; }

        public HolidayMonth_User[] Months
        {
            get
            {
                return Enumerable.Range(1, 12)
                            .Select(month => new HolidayMonth_User {
                                Month = month,
                                Days = Enumerable.Range(1, DateTime.DaysInMonth(Year, month))
                                .Select(day =>
                                {
                                    var dt = new DateTime(Year, month, day);
                                    return new HolidayDay
                                    {
                                        Day = dt.Day,
                                        DayOfWeek = dt.DayOfWeek,
                                        Holiday = Holidays.FirstOrDefault(x => x.Date_From <= dt && x.Date_To >= dt)
                                    };
                                }).ToArray()
                            }).ToArray();
            }
        }
    }

    public class HolidayMonth_User
    {
        public int Month { get; set; }
        public HolidayDay[] Days { get; set; }
    }
}
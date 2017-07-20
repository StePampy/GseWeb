using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Holiday
{
    public class HolidayYear_Manager
    {
        public int Year { get; set; }
        public Holiday[] Holidays { get; set; }
        public Account.User[] Users { get; set; }
        public HolidayMonth_Manager[] Months
        {
            get
            {
                return Enumerable.Range(1, 12)
                            .Select(month => new HolidayMonth_Manager
                            {
                                Month = month,
                                Users = Users
                                .Select(u => {
                                    var holidaysUser = Holidays.Where(x => x.UserId == u.UserId).ToArray();
                                    return new HolidayUser_Manager {
                                        User = u,
                                        Days = Enumerable.Range(1, DateTime.DaysInMonth(Year, month))
                                        .Select(day => {
                                            var dt = new DateTime(Year, month, day);
                                            return new HolidayDay
                                            {
                                                Day = dt.Day,
                                                DayOfWeek = dt.DayOfWeek,
                                                Holiday = holidaysUser.FirstOrDefault(x => x.Date_From <= dt && x.Date_To >= dt)
                                            };
                                        }).ToArray()
                                    };
                                }).ToArray()
                            }).ToArray();
            }
        }
    }

    public class HolidayMonth_Manager
    {
        public int Month { get; set; }
        public HolidayUser_Manager[] Users { get; set; }
    }

    public class HolidayUser_Manager
    {
        public Account.User User { get; set; }
        public HolidayDay[] Days { get; set; }
    }
}
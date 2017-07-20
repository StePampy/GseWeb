using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GseWeb.Models
{
    public static class Extensions
    {
        public static TimeSpan FloorHalfHour(this TimeSpan value)
        {
            return TimeSpan.FromSeconds(Math.Floor(value.TotalSeconds / 1800) * 1800);
        }

        public static TimeSpan CeilingHalfHour(this TimeSpan value)
        {
            return TimeSpan.FromSeconds(Math.Ceiling(value.TotalSeconds / 1800) * 1800);
        }

        public static string Description(this Enum value)
        {
            // variables  
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // return  
            return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
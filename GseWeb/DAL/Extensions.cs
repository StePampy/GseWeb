using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GseWeb.DAL
{
    public static class Extensions
    {
        public static string MySqlExMessage(this Exception ex)
        {
            var m = ex.InnerException?.InnerException?.Message;
            return (m == null) ? ex.Message : m;
        }
    }
}
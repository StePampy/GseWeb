using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models;

namespace GseWeb.DAL
{
    public static class SelectList
    {
        public static IEnumerable<SelectListItem> Users()
        {
            using (var db = new GestionaleDB())
            {
                return db.Users.Where(x => x.UserId != "Admin")
                    .ToArray()
                    .OrderBy(x => x.RoleId)
                    .ThenBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.UserId,
                        Text = x.LastName + " " + x.FirstName,
                    });
                //.ToArray()
                //.OrderBy(x => x.Text);
            }
        }

        public static IEnumerable<SelectListItem> UserRoles()
        {
            using (var db = new GestionaleDB())
            {
                return db.UserRoles.OrderBy(x => x.Id).ToArray()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Value
                });
            }
        }
        public static IEnumerable<SelectListItem> HoursProfiles()
        {
            using (var db = new GestionaleDB())
            {
                return db.HoursProfiles.OrderBy(x => x.Id).ToArray()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Description
                });
            }
        }

        public static IEnumerable<SelectListItem> WorkTypesNotJustified()
        {
            var lst = new Models.Hours.WorkType[]
            {
                Models.Hours.WorkType.PermessoNonRetribuito,
                Models.Hours.WorkType.Malattia,
                Models.Hours.WorkType.Infortunio,
                Models.Hours.WorkType.Lutto,
                Models.Hours.WorkType.Recupero,
                Models.Hours.WorkType.Viaggio,
            };
            return lst.Select(x => new SelectListItem
            {
                Value = ((int)x).ToString(),
                Text = x.Description()
            });
        }

        //public static IEnumerable<SelectListItem> WorkTypes()
        //{
        //    using (var db = new GestionaleDB())
        //    {
        //        return db.WorkTypes.Where(x => x.Id > 2)
        //            .OrderBy(x => x.Id).ToArray()
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.Id.ToString(),
        //            Text = x.Description
        //        });
        //    }
        //}

        public static IEnumerable<SelectListItem> UserOrders(string UserId)
        {
            using (var db = new GestionaleDB())
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == UserId);
                if (user == null)
                    return new SelectListItem[0];

                var ordUsr = user.UserOrders.Where(x => !x.Order.Closed)
                    .Select(x => new SelectListItem
                    {
                        Value = x.OrderId,
                        Text = x.Order.Description
                    }).ToArray();

                var ordTL = db.Orders.Where(x => !x.Closed && x.TeamLeaderId == UserId)
                    .Select(x => new SelectListItem
                    {
                        Value = x.OrderId,
                        Text = x.Description
                    }).ToArray();

                return ordUsr.Union(ordTL, new ComparerSLI());
            }
        }

        private class ComparerSLI : IEqualityComparer<SelectListItem>
        {
            public bool Equals(SelectListItem x, SelectListItem y)
            {
                return x.Value == y.Value;
            }

            public int GetHashCode(SelectListItem obj)
            {
                return obj.Value.GetHashCode();
            }
        }

    }
}
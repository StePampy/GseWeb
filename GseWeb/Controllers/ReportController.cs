using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.DAL;
using GseWeb.Models.Report;
using System.Globalization;
using System.Data.Entity;

namespace GseWeb.Controllers
{
    public class ReportController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult ReportOrders(bool closed = false)
        {
            var user = db.Users.Find(User.Identity.Name);

            var orders = (user.RoleId == 1) ?
                db.Orders.Where(x => x.Closed == closed).ToArray()
                : db.Orders.Where(x => x.TeamLeaderId == user.UserId && x.Closed == closed).ToArray();
            var dett = ReportOrderDetails.FromOrders(orders);
            return View(dett);
        }

        [HttpGet]
        public ActionResult ReportOrder(string OrderId)
        {
            var order = db.Orders.Find(OrderId);
            if (order == null)
                return HttpNotFound();

            // Modello da ritornare
            var model = ReportOrderVM.GetFromOrder(order);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult ReportUsers(int year = 0, int month = 0)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            month = (month == 0) ? DateTime.Now.Month : month;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.MonthFrozen = db.MonthsFrozen.Any(x => x.Year == year && x.Month == month);
            var rep = db.Users.Where(x => x.UserId != "Admin")
                .Select(x => new ReportUser
                {
                    UserId = x.UserId,
                    UserInfo = x.LastName + " " + x.FirstName
                }).ToArray()
            .OrderBy(x => x.UserInfo);
            return View(rep);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult FreezeMonth(int year = 0, int month = 0)
        {
            try
            {
                var entity = db.MonthsFrozen.Find(year, month);
                if (entity == null)
                {
                    // Aggiungo
                    entity = new Models.MonthFrozen { Year = year, Month = month };
                    db.MonthsFrozen.Add(entity);
                    TempData["AlertMessage"] = "Mese Bloccato con Successo";
                }
                else
                {
                    // Elimino
                    db.Entry(entity).State = EntityState.Deleted;
                    TempData["AlertMessage"] = "Mese Sbloccato con Successo";
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = string.Format("Error: {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("ReportUsers", new { year = year, month = month });
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult ReportWorkOrders(int year = 0, int month = 0,string UserId = null)
        {
            ViewBag.IsManager = (UserId == null) ? false : true;
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var model = new ReportWorkOrder()
            {
                User = db.Users.Find(UserId),
                Year = (year == 0) ? DateTime.Now.Year : year,
                Month = (month == 0) ? DateTime.Now.Month : month,
            };
            model.WorkOrders =  db.WorkOrders.Where(x => x.UserId == UserId && x.Date.Year == model.Year && x.Date.Month == model.Month).OrderBy(x => x.Date).ToArray();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public PartialViewResult ReportLabor(int year, int month, string UserId, string OrderId)
        {
            ViewBag.Title = "Manodopera Utente";
            var model = new ReportLabor
            {
                User = db.Users.Find(UserId),
                Order = db.Orders.Find(OrderId),
                Year = year,
                Month = month
            };
            return PartialView("_ReportLabor", model);
        }
    }
}
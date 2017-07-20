using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Hours;
using System.Globalization;
using GseWeb.DAL;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace GseWeb.Controllers
{
    public class HoursController : Controller
    {
        // GET: UserWork
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Index(int year = 0, int month = 0, string UserId = null)
        {
            // Se l'utente non e nullo signifa che chi sta visualizzando e il manager quindi faccio sparire i bottoni
            ViewBag.IsManager = (UserId == null) ? false : true;

            year = (year == 0) ? DateTime.Now.Year : year;
            month = (month == 0) ? DateTime.Now.Month : month;
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            var model = new HoursOfMonth(UserId, year, month);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult GetDay(int year = 0, int month = 0, int day = 0)
        {
            var UserId = User.Identity.Name;
            year = (year == 0) ? DateTime.Now.Year : year;
            month = (month == 0) ? DateTime.Now.Month : month;
            day = (day == 0) ? DateTime.Now.Day : day;

            var hYear = new HoursOfYear(UserId, year);
            var model = hYear.Hours.FirstOrDefault(x => x.Date.Month == month && x.Date.Day == day);
            // Aggiungo le liste WorkType e Ordini
            if (model.WorkTypeRegular == WorkType.NonGiustificato || ((int)model.WorkTypeRegular >= (int)WorkType.PermessoNonRetribuito && (int)model.WorkTypeRegular <= (int)WorkType.Viaggio))
                ViewBag.lstWorkType = DAL.SelectList.WorkTypesNotJustified(model.Travel > new TimeSpan(0), hYear.ExtraAmount > new TimeSpan(0));

            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(UserId);
            // Titolo del Popup
            ViewBag.PopupTitle = "Modifica Giorno";
            ViewBag.Title = GetTitle(model.Date);
            return PartialView("_ManageDay",model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateEditHours(Hour model)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                return PartialView("_ManageHours", model);
            }
            try
            {
                var entity = db.Hours.Find(model.UserId, model.Date);
                // Verifico se ci sono ordini e se sono state modificate le ore
                var bOrders = db.WorkOrders.Any(x => x.UserId == model.UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(model.Date));
                if (bOrders && (entity.Start != model.Start || entity.End != model.End || entity.Break != model.Break || entity.Travel != model.Travel))
                    throw new Exception("Impossibile Modificare le Ore di Giorni con commesse associate");

                // Sistemo le note
                model.Note = (model.Note == null) ? "" : model.Note;
                // Aggiungo o modifico del db
                db.Hours.AddOrUpdate(m => new { m.UserId, m.Date }, model);
                db.SaveChanges();
                ViewBag.Success = "La modifica è stata eseguita";
                return GetDay(model.Date.Year, model.Date.Month, model.Date.Day);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                Response.StatusCode = 500;
                return PartialView("_ManageHours", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult DeleteHour(long DateTicks = 0, string UserId = null)
        {
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var date = new DateTime(DateTicks);
            try
            {
                // Verifico gli ordini di quel giorno che per quel giorno
                if (db.WorkOrders.Where(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Count() > 0)
                    throw new Exception("Impossibile Eliminare Giorni con commesse associate");
                // Delete user into DB
                var entity = db.Hours.Find(UserId, date);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["Error"] = "Giorno Eliminato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.MySqlExMessage();
            }
            return RedirectToAction("Index", new { year = date.Year, month = date.Month });
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult GetWorkOrder(long DateTicks, string OrderId, string UserId = null)
        {
            var date = new DateTime(DateTicks);
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            var model = db.WorkOrders.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) && x.OrderId == OrderId);

            // Aggiungo gli ordini dell'utente
            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(UserId);
            return PartialView("_ManageWorkOrder", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateEditWorkOrder(Models.Orders.WorkOrder model)
        {
            // Aggiungo gli ordini dell'utente
            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(model.UserId);

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                return PartialView("_ManageWorkOrder", model);
            }
            try
            {
                // Verifico che non siano troppe le ore inserite
                var hYear = new HoursOfYear(model.UserId, model.Date.Year);
                var hDay = hYear.Hours.FirstOrDefault(x => x.Date.Month == model.Date.Month && x.Date.Day == model.Date.Day);
                var avaiableHours = hDay.WorkTime + hDay.Travel - hDay.OrdersTimeComplete;
                if (model.Hours > avaiableHours)
                    throw new Exception("Ore impostate maggiori delle ore disponibili");

                // verifico se l'utente e teamleader
                model.UserApprove = (db.Orders.Any(x => x.OrderId == model.OrderId && x.TeamLeaderId == model.UserId)) ? model.UserId : null;
                
                // aggiungo il costo orario e verifico se il tipo e dirigente
                var user = hYear.User;
                if (user.HoursProfile.CostDaily)
                {
                    // Sabato
                    if (model.Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        model.Cost = 0;
                    }
                    // Festivo e Domenica
                    else if (hDay.Ordinary == new TimeSpan(0) && (hDay.WorkTime + hDay.Travel) < new TimeSpan(8,0,0))
                    {
                        model.Cost = (user.Cost / new TimeSpan(8, 0, 0).TotalHours) * model.Hours.TotalHours;
                    }
                    // In settimana minore dell'ordinario
                    else if ((hDay.WorkTime + hDay.Travel) < hDay.Ordinary)
                    {
                        model.Cost = (user.Cost / hDay.Ordinary.TotalHours) * model.Hours.TotalHours;
                    }
                    else
                    {
                        model.Cost = (user.Cost / hDay.WorkTime.TotalHours) * model.Hours.TotalHours;
                    }
                }
                else
                {
                    model.Cost = model.Hours.TotalHours * user.Cost;
                }
                // Aggiungo o modifico del db
                db.WorkOrders.AddOrUpdate(m => new { m.UserId,m.OrderId, m.Date }, model);
                db.SaveChanges();
                ViewBag.Success = "La modifica è stata eseguita";
                return GetDay(model.Date.Year, model.Date.Month, model.Date.Day);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                Response.StatusCode = 500;
                return PartialView("_ManageWorkOrder", model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult DeleteWorkOrder(long DateTicks, string OrderId, string UserId = null)
        {
            var date = new DateTime(DateTicks);
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            try
            {
                // Cerco quello gia configurato nel DB
                var entity = db.WorkOrders.Find(UserId, OrderId, date);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.MySqlExMessage();
            }
            return GetDay(date.Year, date.Month, date.Day);
        }

        private string GetTitle(DateTime date)
        {
            var CInfIT = CultureInfo.CreateSpecificCulture("it-IT");
            var DayName = CInfIT.TextInfo.ToTitleCase(CInfIT.DateTimeFormat.GetDayName(date.DayOfWeek));
            var MonthName = CInfIT.TextInfo.ToTitleCase(CInfIT.DateTimeFormat.GetMonthName(date.Month));
            return string.Format("{0} {1} {2} {3}", DayName, date.Day, MonthName, date.Year);
        }

    }
}
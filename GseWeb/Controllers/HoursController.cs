using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Hours;
using System.Globalization;
using GseWeb.DAL;
using System.Data.Entity.Migrations;

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
            if (model.WorkTypeRegular == WorkType.NonGiustificato)
                ViewBag.lstWorkType = DAL.SelectList.WorkTypesNotJustified();

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
                // Aggiungo o modifico del db
                db.Hours.AddOrUpdate(m => new { m.UserId, m.Date }, model);
                db.SaveChanges();
                ViewBag.Success = "La modifica è stata esguita";
                return GetDay(model.Date.Year, model.Date.Month, model.Date.Day);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                Response.StatusCode = 500;
                return PartialView("_ManageHours", model);
            }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.UserWork;

namespace GseWeb.Controllers
{
    public class UserWorkController : Controller
    {
        // GET: UserWork
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        public ActionResult Index()
        {
            //var u = new IEnumerable<Models.UserWork.Hour>()
            var m = new Models.UserWork.HoursOfMonth("ste", 2017, 6);
            var hy = new Models.UserWork.HoursOfYear("ste", 2017);
            //var u = db.UserWork(2017, "ste");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult ReportMonth(int year = 0, int month = 0, string UserId = null)
        {
            // Se l'utente non e nullo signifa che chi sta visualizzando e il manager quindi faccio sparire i bottoni
            ViewBag.IsManager = (UserId == null) ? false : true;

            year = (year == 0) ? DateTime.Now.Year : year;
            month = (month == 0) ? DateTime.Now.Month : month;
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            var model = new Models.UserWork.HoursOfMonth(UserId, year, month);
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
            // Se l'utente non e nullo signifa che chi sta visualizzando e il manager quindi faccio sparire i bottoni
            var hYear = new HoursOfYear(UserId, year);
            var model = hYear.Hours.FirstOrDefault(x => x.Date.Month == month && x.Date.Year == year);
            // Aggiungo le liste
            return PartialView("_ManagerDay",model);
        }

    }
}
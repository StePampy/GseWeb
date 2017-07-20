using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Hours;
using GseWeb.DAL;

namespace GseWeb.Controllers
{
    public class HoursOldController : Controller
    {
        /*
        private DAL.GestionaleDB db = new DAL.GestionaleDB();

        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult Hour(string UserId = null)
        {
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var date = DateTime.Now.Date;
            var hr = db.Hours.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date))
                ?? new Hour() { UserId = UserId, Date = date };

            // Lista tipo causale
            Session.Add("lstWorkTypes", DAL.SelectList.WorkTypes());

            return PartialView("_Hour", hr);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Hour(long DateTicks, string UserId = null)
        {
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var date = new DateTime(DateTicks);
            var hr = db.Hours.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date))
               ?? new Hour() { UserId = UserId, Date = date };

            // Lista tipo causale
            Session.Add("lstWorkTypes", DAL.SelectList.WorkTypes());

            return PartialView("_Hour", hr);
        }


        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Create(long DateTicks = 0, string UserId = null, string returnUrl = "")
        {
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var date = new DateTime(DateTicks);
            var hr = db.Hours.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date))
               ?? new Hour() { UserId = UserId, Date = date };

            // Lista tipo causale
            Session.Add("lstWorkTypes", DAL.SelectList.WorkTypes());
            ViewBag.ReturnUrl = returnUrl;
            return View("Hour", hr);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Create(Hour model, string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                if (returnUrl == "Report/ReportMonth")
                    return View("Hour", model);
                return PartialView("_Hour", model);
            }
            try
            {
                model.Note = (model.Note == null) ? "" : model.Note;
                db.Hours.Add(model);
                db.SaveChanges();
                db = new DAL.GestionaleDB();
                model = db.Hours.FirstOrDefault(x => x.UserId == model.UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(model.Date))
                    ?? new Hour() { UserId = model.UserId, Date = model.Date };
                if (returnUrl == "Report/ReportMonth")
                    return View("Hour", model);
                //return RedirectToAction("Edit", new { DateTicks = model.Date.Date.Ticks, returnUrl = returnUrl });
                return PartialView("_Hour", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                if (returnUrl == "Report/ReportMonth")
                    return View("Hour", model);
                return PartialView("_Hour", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Edit(long DateTicks = 0, string UserId = null, string returnUrl = "")
        {
            UserId = (UserId == null) ? User.Identity.Name : UserId;
            var date = new DateTime(DateTicks);
            var hr = db.Hours.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date))
               ?? new Hour() { UserId = UserId, Date = date };

            // Lista tipo causale
            Session.Add("lstWorkTypes", DAL.SelectList.WorkTypes());
            ViewBag.ReturnUrl = returnUrl;
            return View("Hour", hr);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Edit(HourResult model, string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                var hour = new Hour() { UserId = model.UserId, Date = model.Date };
                hour.Results = model;
                if (returnUrl == "Report/ReportMonth")
                    return View("Hour", hour);
                return PartialView("_Hour", model);
            }
            try
            {
                var entity = db.Hours.Find(model.UserId, model.Date);

                // Verifico se ci sono ordini e se sono state modificate le ore
                var bOrders = db.WorkOrders.Any(x => x.UserId == model.UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(model.Date));
                if(bOrders && (entity.Start != model.Start || entity.End != model.End || entity.Break != model.Break || entity.Travel != model.Travel))
                    throw new Exception("Impossibile Modificare le Ore di Giorni con commesse associate");
                
                // Edit hour into DB
                model.Note = (model.Note == null) ? "" : model.Note;
                db.Entry(entity).CurrentValues.SetValues(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
            }
            // Lista tipo causale
            Session.Add("lstWorkTypes", DAL.SelectList.WorkTypes());
            // Select new result
            db = new DAL.GestionaleDB();
            var h2 = db.Hours.FirstOrDefault(x => x.UserId == model.UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(model.Date))
                ?? new Hour() { UserId = model.UserId, Date = model.Date };
            if (returnUrl == "Report/ReportMonth")
                return View("Hour", h2);
            return PartialView("_Hour", h2);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Delete(long DateTicks = 0, string UserId = null, string returnUrl = "")
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                TempData["ErrorHours"] = ex.MySqlExMessage();
            }
            
            ViewBag.ReturnUrl = returnUrl;
            if (returnUrl == "Report/ReportMonth")
                return RedirectToAction("ReportMonth","Report", new { year = date.Date.Year, month = date.Date.Month});
            return View();
        }
        */
    }
}
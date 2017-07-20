using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.DAL;
using System.Data.Entity;

namespace GseWeb.Controllers
{
    public class FestivityController : Controller
    {
        private GestionaleDB db = new GestionaleDB();
        
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Index(int year = 0)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            ViewBag.Year = year;
            var res = db.Festivities.Where(x => x.Date.Year == year).ToArray().OrderBy(x => x.Date);
            return View(res);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Create(int year)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            ViewBag.Year = year;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Create(Models.Hours.Festivity model, int year)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            ViewBag.Year = year;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                db.Festivities.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
            return RedirectToAction("Index", new { year = year});
        }

        [HttpGet]
        public ActionResult Delete(DateTime date, int year)
        {
            try
            {
                var entity = db.Festivities.Find(date);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.MySqlExMessage();
            }
            return RedirectToAction("Index", new { year = year });
        }
    }
}
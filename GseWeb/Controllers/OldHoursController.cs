using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models;

namespace GseWeb.Controllers
{
    public class OldHoursController : Controller
    {
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Hour()
        //{
        //    var date = DateTime.Now;
        //    var hr = DAL.Hours.GetHour(User.Identity.Name, date);
        //    hr = (hr == null) ? new Hour() { Date = date } : hr;
        //    hr.DateTicks = date.Ticks;
        //    return PartialView("_Hour", hr);
        //}

        //[HttpPost]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Hour(DateTime date)
        //{
        //    var hr = DAL.Hours.GetHour(User.Identity.Name, date);
        //    hr = (hr == null) ? new Hour() { Date = date } : hr;
        //    hr.DateTicks = date.Ticks;
        //    return PartialView("_Hour", hr);
        //}

        //[HttpGet]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Create(long DateTicks = 0, string returnUrl = "")
        //{
        //    var date = new DateTime(DateTicks);
        //    var hr = DAL.Hours.GetHour(User.Identity.Name, date);
        //    hr = (hr == null) ? new Hour() { Date = date } : hr;
        //    hr.DateTicks = date.Ticks;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View("Hour", hr);
        //}

        //[HttpPost]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Create(Models.Hour hour, string returnUrl = "")
        //{
        //    hour.Date = new DateTime(hour.DateTicks);
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Add user into DB
        //            DAL.Hours.CreateHour(User.Identity.Name, hour);
        //            hour = DAL.Hours.GetHour(User.Identity.Name, hour.Date);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", (ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
        //        }
        //    }
        //    if (returnUrl == "Hours/ReportMonth")
        //        return RedirectToAction("Edit", new { DateTicks = hour.Date.Date.Ticks, returnUrl = returnUrl });
        //    return PartialView("_Hour", hour);
        //}

        //[HttpGet]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Edit(long DateTicks = 0, string returnUrl = "")
        //{
        //    var date = new DateTime(DateTicks);
        //    var hr = DAL.Hours.GetHour(User.Identity.Name, date);
        //    hr = (hr == null) ? new Hour() { Date = date } : hr;
        //    hr.DateTicks = date.Ticks;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View("Hour", hr);
        //}

        //[HttpPost]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Edit(Models.Hour hour, string returnUrl = "")
        //{
        //    hour.Date = new DateTime(hour.DateTicks);
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Edit user into DB
        //            DAL.Hours.EditHour(User.Identity.Name, hour);
        //            hour = DAL.Hours.GetHour(User.Identity.Name, hour.Date);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", (ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
        //        }
               
        //    }
        //    if(returnUrl == "Hours/ReportMonth")
        //        return RedirectToAction("ReportMonth", new { year = hour.Date.Year, month = hour.Date.Month });
        //    return PartialView("_Hour", hour);
        //}

        

        //[HttpGet]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult Delete(long DateTicks = 0, string returnUrl = "")
        //{
        //    var date = new DateTime(DateTicks);
        //    try
        //    {
        //        DAL.Hours.DeleteHour(User.Identity.Name, date.Date);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", (ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
        //    }
        //    ViewBag.ReturnUrl = returnUrl;
        //    if (returnUrl == "Hours/ReportMonth")
        //        return RedirectToAction("ReportMonth", new { year = date.Date.Year, month = date.Date.Month });
        //    return View();
        //}

        //[HttpGet]
        //[Authorize(Roles = "Manager, TeamLeader, Employee")]
        //public ActionResult ReportMonth(int year = 0, int month = 0)
        //{

        //    year = (year == 0) ? DateTime.Now.Year : year;
        //    month = (month == 0) ? DateTime.Now.Month : month;
        //    var dates = GetDates(year, month);
        //    var hours = DAL.Hours.GetReportMonth(User.Identity.Name, year, month);
        //    var fest = DAL.Hours.GetFestivity(year, month);
        //    var monthtot = DAL.Hours.GetReportMonthTotal(User.Identity.Name, year, month);

        //    var user = new Models.Account.User(); //DAL.Users.GetUser(User.Identity.Name);

        //    var hourrep = dates.Select(x => new DateReport {
        //        Date = x,
        //        Festivity = fest.Any(f => f.Date == x),
        //        Hour = hours.FirstOrDefault(h => h.Date == x), 
        //    });
        //    var rep = new MonthReport
        //    {
        //        UserId = user.UserId,
        //        UserFirstName = user.FirstName,
        //        UserLastName = user.LastName,
        //        Year = year,
        //        Month = month,
        //        Dates = hourrep,
        //        Totals = monthtot,
        //    };
        //    ViewBag.ReturnUrl = "Hours/ReportMonth";
        //    return View(rep);
        //}

        //public static DateTime[] GetDates(int year, int month)
        //{
        //    return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
        //                     .Select(day => new DateTime(year, month, day)) // Map each day to a date
        //                     .ToArray(); // Load dates into a list
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

    }
}
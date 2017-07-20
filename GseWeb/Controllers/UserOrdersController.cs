using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Orders;
using System.Data.Entity;
using System.Net;
using GseWeb.DAL;

namespace GseWeb.Controllers
{
    public class UserOrdersController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Orders(long DateTicks, string UserId = null)
        {
            var date = new DateTime(DateTicks);
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            // Prendo gli ordini
            var ordVM = OrderVM.Get(date, UserId);

            // Ordini configurabili
            var lstord = DAL.SelectList.UserOrders(UserId).Where(x => ! ordVM.Orders.Select(o => o.OrderId).ToList().Contains(x.Value));
            Session.Add("lstUserOrders", lstord);

            return PartialView("_Orders", ordVM);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult GetEdit(long DateTicks, string OrderId, string UserId = null)
        {
            var date = new DateTime(DateTicks);
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            // Prendo gli ordini
            var ordVM = OrderVM.Get(date, UserId);

            // Prendo l'ordine da modificare
            db = new DAL.GestionaleDB();
            ordVM.EditOrder = db.WorkOrders.FirstOrDefault(x => x.UserId == UserId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) && x.OrderId == OrderId);
            return PartialView("_Orders", ordVM);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Add(WorkOrder model)
        {
            var ordVM = OrderVM.Get(model.Date, model.UserId);
            // Approssimo alla mezz'ora per difetto
            model.Hours = TimeSpan.FromSeconds(Math.Floor(model.Hours.TotalSeconds / 1800) * 1800);
            if (!ModelState.IsValid)
            {
                ordVM.AddOrder = model;
                return PartialView("_Orders", ordVM);
            }
            if (model.Hours > ordVM.AvaiableHours)
            {
                ordVM.AddOrder = model;
                ViewBag.ErrorOrder = "Ore impostate maggiori di ore disponibili";
                return PartialView("_Orders", ordVM);
            }
            // estraggo le info utente
            var user = db.Users.Find(model.UserId);
            try
            {
                // verifico se l'utente e teamleader
                model.UserApprove = (db.Orders.Any(x => x.OrderId == model.OrderId && x.TeamLeaderId == model.UserId)) ? model.UserId : null;
                // aggiungo il costo orario
                model.Cost = model.Hours.TotalHours * user.Cost;
                // Add into db
                db.WorkOrders.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ordVM.AddOrder = model;
                ViewBag.ErrorOrder = ex.MySqlExMessage();
                return PartialView("_Orders", ordVM);
            }
            return Orders(model.Date.Ticks);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Edit(WorkOrder model)
        {
            var ordVM = OrderVM.Get(model.Date, model.UserId);
            // Approssimo alla mezz'ora per difetto
            model.Hours = TimeSpan.FromSeconds(Math.Floor(model.Hours.TotalSeconds / 1800) * 1800);
            if (!ModelState.IsValid)
            {
                ordVM.AddOrder = model;
                return PartialView("_Orders", ordVM);
            }
            // Cerco quello gia configurato nel DB
            var entity = db.WorkOrders.Find(model.UserId, model.OrderId,model.Date);
            // do come ore disponibili anche quelle dello stesso ordine
            var AvaiableHours = ordVM.AvaiableHours.Add(entity.Hours);
            if (model.Hours > AvaiableHours)
            {
                ordVM.AddOrder = model;
                ViewBag.ErrorOrder = "Ore impostate maggiori di ore disponibili";
                return PartialView("_Orders", ordVM);
            }
            // estraggo le info utente
            var user = db.Users.Find(model.UserId);
            try
            {
                // verifico se l'utente e teamleader
                model.UserApprove = (db.Orders.Any(x => x.OrderId == model.OrderId && x.TeamLeaderId == model.UserId)) ? model.UserId : null;
                // aggiungo il costo orario
                model.Cost = model.Hours.TotalHours * user.Cost;
                // Edit into db
                db.Entry(entity).CurrentValues.SetValues(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ordVM.AddOrder = model;
                ViewBag.ErrorOrder = ex.MySqlExMessage();
                return PartialView("_Orders", ordVM);
            }
            return Orders(model.Date.Ticks);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Delete(long DateTicks, string OrderId, string UserId = null)
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
                ViewBag.ErrorOrder = ex.MySqlExMessage();
            }
            return Orders(DateTicks);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult ApproveHours()
        {
            var user = db.Users.Find(User.Identity.Name);
            if(user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var hours = user.TeamLeaderOrders
                .SelectMany(x => x.WorkOrders.Where(o => o.UserId != x.TeamLeaderId && o.UserApprove == null))
                .OrderByDescending(x => x.Date)
                .Select(x => new ApproveHour {
                    OrderId = x.OrderId,
                    OrderClient = x.Order.Client,
                    OrderDescription = x.Order.Description,
                    Date = x.Date,
                    UserId = x.UserId,
                    UserInfo = x.User.LastName + " " + x.User.FirstName,
                    Hours = x.Hours,
                }).ToArray();
            return View(hours);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult ApproveHour(string UserId, string OrderId, DateTime Date)
        {
            try
            {
                var entity = db.WorkOrders.Find(UserId, OrderId, Date);
                if (entity == null)
                    return HttpNotFound();
                entity.UserApprove = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Approvato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("ApproveHours");
        }

    }
}
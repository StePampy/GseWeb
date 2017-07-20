using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Vehicles;
using GseWeb.DAL;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Net;

namespace GseWeb.Controllers
{
    public class VehicleController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        // GET: Vehicle

        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Index(int month = 0, int year = 0, string UserId = null)
        {
            // Se l'utente non e nullo signifa che chi sta visualizzando e il manager quindi faccio sparire i bottoni
            ViewBag.IsManager = (UserId == null) ? false : true;
            UserId = (UserId == null) ? User.Identity.Name : UserId;

            var user = db.Users.Find(UserId);
            var model = new VehicleVM {
                User = user,
                Year = (year == 0) ? DateTime.Now.Year : year,
                Month = (month == 0) ? DateTime.Now.Month : month,
            };
            model.UserVehicles = db.UsersVehicles.Where(x => x.UserId == user.UserId).ToArray();
            model.WorkVehicles = db.WorkVehicles.Where(x => x.UserId == user.UserId && x.Date.Year == model.Year && x.Date.Month == model.Month)
                .OrderBy(x => x.Date)
                .ToArray();
            model.Km_Total = model.WorkVehicles.Sum(x => x.Km_Total);
            model.Cost_Total = model.WorkVehicles.Sum(x => x.Cost_Total);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateUserVehicle()
        {
            var user = db.Users.Find(User.Identity.Name);
            var model = new UserVehicle() { Id = -1};
            model.UserId = user.UserId;
            ViewBag.Title = "Aggiungi Veicolo";
            return PartialView("_CreateEditUserVehicle", model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult EditUserVehicle(int Id)
        {
            var model = db.UsersVehicles.Find(Id);
            ViewBag.Title = "Modifica Veicolo";
            return PartialView("_CreateEditUserVehicle", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateEditUserVehicle(UserVehicle model)
        {
            ViewBag.Title = (model.Id == -1) ? "Aggiungi Veicolo" : "Modifica Veicolo";
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEditUserVehicle", model);
            }
            // Inserimento/Modifica in database
            try
            {
                model.UpdateDate = DateTime.Now;
                db.UsersVehicles.AddOrUpdate(m => m.Id, model);
                db.SaveChanges();
                ViewBag.Message = (model.Id == -1) ? "Aggiunto con Successo!" : "Modificato con Successo!";
                ViewBag.ReturnUrl = string.Format("/Vehicle/Index?year={0}&month={1}", DateTime.Now.Year, DateTime.Now.Month);
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_CreateEditUserVehicle", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult DeleteUserVehicle(int id, int year, int month)
        {
            try
            {
                var entity = db.UsersVehicles.Find(id);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["DeleteMessage"] = "Eliminato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("Index", new { year = year, month = month });
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateWorkVehicle(int VehicleId)
        {
            var user = db.Users.Find(User.Identity.Name);
            var vehicle = db.UsersVehicles.Find(VehicleId);
            // Se il veicolo e piu vecchio di tre mesi chiedo la modifica
            if (DateTime.Now > vehicle.UpdateDate.AddMonths(3))
            {
                ViewBag.Title = "Modifica Veicolo";
                ModelState.AddModelError("", "Aggiornare costo veicolo");
                return PartialView("_CreateEditUserVehicle", vehicle);
            }
            var model = new WorkVehicle();
            model.Id = -1;
            model.Date = DateTime.Now;
            model.UserId = user.UserId;
            model.VehicleDescription = vehicle.VehicleDescription;
            model.Cost_Km = vehicle.Cost;
            model.KmHomeWork = vehicle.KmHomeWork;
            ViewBag.Title = "Aggiungi Percorso";
            ViewBag.Routes = GetRoutes(User.Identity.Name);

            // Ordini Configurabili
            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(user.UserId);

            return PartialView("_CreateEditWorkVehicle", model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult EditWorkVehicle(int Id)
        {
            ViewBag.Title = "Modifica Percorso";
            ViewBag.Routes = GetRoutes(User.Identity.Name);
            var model = db.WorkVehicles.Find(Id);

            // Ordini Configurabili
            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(model.UserId);

            return PartialView("_CreateEditWorkVehicle", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateEditWorkVehicle(WorkVehicle model)
        {
            ViewBag.Title = (model.Id == -1) ? "Aggiungi Percorso" : "Modifica Percorso";
            ViewBag.Routes = GetRoutes(User.Identity.Name);
            // Ordini Configurabili
            ViewBag.lstUserOrders = DAL.SelectList.UserOrders(model.UserId);
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEditWorkVehicle", model);
            }
            // Inserimento/Modifica in database
            try
            {
                // verifico se l'utente e teamleader
                model.UserApprove = (db.Orders.Any(x => x.OrderId == model.OrderId && x.TeamLeaderId == model.UserId)) ? model.UserId : null;
                db.WorkVehicles.AddOrUpdate(m => m.Id, model);
                db.SaveChanges();
                ViewBag.Message = (model.Id == -1) ? "Aggiunto con Successo!" : "Modificato con Successo!";
                ViewBag.ReturnUrl = string.Format("/Vehicle/Index?year={0}&month={1}", model.Date.Year, model.Date.Month);
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_CreateEditWorkVehicle", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult DeleteWorkVehicle(int id, int year, int month)
        {
            try
            {
                var entity = db.WorkVehicles.Find(id);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["DeleteMessage"] = "Eliminato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("Index", new { year = year, month = month });
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult Approve()
        {
            var user = db.Users.Find(User.Identity.Name);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var toapprove = user.TeamLeaderOrders
                .SelectMany(x => x.WorkVehicles.Where(o => o.UserId != x.TeamLeaderId && o.UserApprove == null))
                .OrderByDescending(x => x.Date).ToArray();
            return View(toapprove);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult ApproveWorkVehicle(int id)
        {
            try
            {
                var entity = db.WorkVehicles.Find(id);
                entity.UserApprove = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Approvato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("Approve");
        }

        private Route[] GetRoutes(string UserId)
        {
            return db.WorkVehicles
                .Where(x => x.UserId == UserId)
                .GroupBy(x => new {
                    x.From,
                    x.To
                }).Select(x => new Route
                {
                    From = x.Key.From,
                    To = x.Key.To,
                    Km = x.Min(v => v.Km)
                }).ToArray();
        }
    }
}
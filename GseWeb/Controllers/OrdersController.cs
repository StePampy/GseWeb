using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Orders;
using System.Data.Entity;
using GseWeb.DAL;
using System.Net;

namespace GseWeb.Controllers
{
    public class OrdersController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        // GET: Orders

        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var ord = db.Orders.OrderBy(x => new { x.Closed, x.FinishDate }).ToArray();
            return View(ord);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            // Lista Utenti

            Session.Add("lstUsers", DAL.SelectList.Users());
            // Creo l'ordine e inserico enrico default

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Create(Order model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                db.Orders.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(string id)
        {
            // Lista Utenti
            Session.Add("lstUsers", DAL.SelectList.Users());
            return View(db.Orders.Find(id));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(Order model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var entity = db.Orders.Find(model.OrderId);
                db.Entry(entity).CurrentValues.SetValues(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(string id)
        {
            try
            {
                var entity = db.Orders.Find(id);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.MySqlExMessage();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult Users(string id, string returnUrl = "")
        {
            var order = db.Orders.Find(id);
            if(order == null)
            {
                return HttpNotFound();
            }
            var model = new UsersOrderVM();
            model.OrderId = order.OrderId;
            model.OrderClient = order.Client;
            model.OrderDescription = order.Description;
            model.Users = db.Users.Where(x => x.UserId != order.TeamLeaderId && x.RoleId != 1)
                .Select(x => new UserOrderVM {
                    UserId = x.UserId,
                    UserInfo = x.FirstName + " " + x.LastName,
                    Selected = db.UsersOrders.Any(o => o.UserId == x.UserId && o.OrderId == order.OrderId)
                });
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult Users(UsersOrderVM model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Update Tabella users_orders
            foreach (var item in model.Users)
            {
                using (var _db = new DAL.GestionaleDB())
                {
                    var entity = db.UsersOrders.Find(model.OrderId, item.UserId);
                    if(entity == null && item.Selected) //Aggiungo
                    {
                        db.UsersOrders.Add(new UserOrder { OrderId = model.OrderId, UserId = item.UserId});
                        db.SaveChanges();
                    }
                    else if (entity != null && ! item.Selected)
                    {
                        db.Entry(entity).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public PartialViewResult GetEditOrderId(string OrderId)
        {
            ViewBag.Title =  "Modifica Codice Commessa";
            var ord = db.Orders.Find(OrderId);
            var model = new EditOrderId { OldOrderId = ord.OrderId };
            return PartialView("_EditOrderId", model);
        }

        [HttpPost]
        public PartialViewResult EditOrderId(EditOrderId model)
        {
            ViewBag.Title = "Modifica Codice Commessa";
            if (!ModelState.IsValid)
            {
                return PartialView("_EditOrderId", model);
            }
            try
            {
                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
                {
                    conn.Open();
                    using (var comm = new MySql.Data.MySqlClient.MySqlCommand("UPDATE orders SET OrderId = @NewOrderId WHERE OrderId = @OldOrderId", conn))
                    {
                        comm.CommandType = System.Data.CommandType.Text;
                        comm.CommandTimeout = 3000;
                        comm.Parameters.AddWithValue("@NewOrderId", model.NewOrderId);
                        comm.Parameters.AddWithValue("@OldOrderId", model.OldOrderId);
                        comm.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                ViewBag.Message = "Modificato con Successo!";
                ViewBag.ReturnUrl = "/Orders/Index";
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_EditOrderId", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult ApproveHours()
        {
            var user = db.Users.Find(User.Identity.Name);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var hours = user.TeamLeaderOrders
                .SelectMany(x => x.WorkOrders.Where(o => o.UserId != x.TeamLeaderId && o.UserApprove == null))
                .OrderByDescending(x => x.Date)
                .Select(x => new ApproveHour
                {
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
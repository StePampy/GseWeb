using GseWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GseWeb.Models.Account;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using GseWeb.DAL;

namespace GseWeb.Controllers
{
    public class AccountController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        // GET: Account
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var usrs = db.Users.Where(x => x.UserId != "Admin").ToArray();
            return View(usrs);
        }

        /*Login*/

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Login model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.FirstOrDefault(x => x.UserId == model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not exist. Please try again.");
                return View(model);
            }
            // Verifico che non sia null
            if (user.Password != model.Password)
            {
                ModelState.AddModelError("", "Invalid password. Please try again.");
                return View(model);
            }

            // Add user into session
            Session.Add("UserLogged", user);
            FormsAuthentication.Initialize();
            // Create a new ticket used for authentication
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
               version: 1, // Ticket version
               name: user.UserId, // Username associated with ticket
               issueDate: DateTime.Now, // Date/time issued
               expiration: DateTime.Now.AddDays(30), // Date/time to expire
               isPersistent: model.RememberMe, // "true" for a persistent user cookie
               userData: (user.RoleId == 1) ? user.UserRole.Value : (db.Orders.Any(x => x.TeamLeaderId == user.UserId)) ? "TeamLeader" : user.UserRole.Value, // User-data, in this case the roles
               cookiePath: FormsAuthentication.FormsCookiePath);// Path cookie valid for

            // Encrypt the cookie using the machine key for secure transport
            string hash = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(
               FormsAuthentication.FormsCookieName, // Name of auth cookie
               hash); // Hashed ticket

            // Set the cookie's expiration time to the tickets expiration time
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

            // Add the cookie to the list for outgoing response
            Response.Cookies.Add(cookie);
            return RedirectToLocal(returnUrl);
        }

        /*Register*/

        // GET: /Account/Login
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            var usr = new User();
            // Liste di anagrafica
            Session.Add("lstRoles", DAL.SelectList.UserRoles());
            Session.Add("lstProfiles", DAL.SelectList.HoursProfiles());
            return View(usr);
        }

        // POST: /Account/Login
        [HttpPost]
        [Authorize(Roles = "Manager")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(User model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The password and confirmation password do not match.");
                return View(model);
            }
            try
            {
                // Add user into DB
                db.Users.Add(model);
                db.SaveChanges();
                TempData["Success"] = "User has been added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(string UserId = "", string returnUrl = "")
        {
            var user = db.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.ConfirmPassword = user.Password;
            ViewBag.returnUrl = returnUrl;
            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(string UserId = "")
        {
            var user = db.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.ConfirmPassword = user.Password;
            // Liste di anagrafica
            Session.Add("lstRoles", DAL.SelectList.UserRoles());
            Session.Add("lstProfiles", DAL.SelectList.HoursProfiles());
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(User model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The password and confirmation password do not match.");
                return View(model);
            }
            try
            {
                // Edit user into DB
                var entity = db.Users.Find(model.UserId);
                db.Entry(entity).CurrentValues.SetValues(model);
                db.SaveChanges();
                TempData["Success"] = "User has been updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
        }



        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(string UserId = "")
        {
            var user = db.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(User model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                // Delete user into DB
                var entity = db.Users.Find(model.UserId);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["Success"] = "User has been deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Manage(string UserId = "")
        {
            var user = db.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.ConfirmPassword = user.Password;
            return View(user);
        }



        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(User model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The password and confirmation password do not match.");
                return View(model);
            }
            try
            {
                // Edit user into DB
                var entity = db.Users.Find(model.UserId);
                entity.Password = model.Password;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Password has been changed successfully!";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return View(model);
            }
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GseWeb.Models.Holiday;
using GseWeb.DAL;
using System.Data.Entity;

namespace GseWeb.Controllers
{
    public class HolidayController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Index(int year = 0)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            var user = db.Users.Find(User.Identity.Name);
            var holidays = db.Holidays.Where(x => x.UserId == user.UserId && (x.Date_From.Year == year || x.Date_To.Year == year)).OrderBy(x => x.Date_From).ToArray();
            var model = new HolidayYear_User
            {
                Year = year,
                User = user,
                Holidays = holidays
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Manage(int year = 0)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            var user = db.Users.Find(User.Identity.Name);
            // Se non sei amministratore allora sei ridirezionato alla pagina principale
            if (user.RoleId != 1)
                return RedirectToAction("Index", new { year = year });
            var users = db.Users.Where(x => x.UserId != "Admin").ToArray();
            var holidays = db.Holidays.Where(x => x.Date_From.Year == year || x.Date_To.Year == year).OrderBy(x => x.Status).ThenBy(x => x.Date_From).ToArray();
            var model = new HolidayYear_Manager
            {
                Year = year,
                Users = users,
                Holidays = holidays
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateHoliday()
        {
            var user = db.Users.Find(User.Identity.Name);
            var model = new Holiday()
            {
                Id = -1,
                UserId = user.UserId,
                Date_From = DateTime.Now,
                Date_To = DateTime.Now,
            };
            ViewBag.Title = "Nuova Richiesta";
            return PartialView("_CreateHoliday", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public PartialViewResult CreateHoliday(Holiday model)
        {
            ViewBag.Title = "Nuova Richiesta";
            
            // verifiche
            if (model !=null && model.Type == Holiday_Type.Ferie)
            {
                if(model.Date_To < model.Date_From)
                    ModelState.AddModelError("", "Il Giorno di Fine non può essere minore del Giorno di Inizio");
                if (db.Holidays.Any(x => x.UserId == model.UserId && (model.Date_From >= x.Date_From && model.Date_From <= x.Date_To || model.Date_To >= x.Date_From && model.Date_To <= x.Date_To)))
                    ModelState.AddModelError("", "Esiste già una richiesta per le date selezionate");
            }
            if (model != null && model.Type == Holiday_Type.Permesso)
            {
                model.Date_To = model.Date_From;
                // Veriica Ore
                if(model.Time_From >= model.Time_To)
                    ModelState.AddModelError("", "Il permesso deve essre maggiore di 00:00:00 ore");
                // Verifica motivazione
                if (string.IsNullOrWhiteSpace(model.Request_Note))
                    ModelState.AddModelError("", "Inserire Motivazione");
                // Verifica Data
                if (db.Holidays.Any(x => x.UserId == model.UserId & model.Date_From >= x.Date_From && model.Date_From <= x.Date_To))
                    ModelState.AddModelError("", "Esiste già una richiesta per la data selezionata");
                // Verifica Ore Ordinarie
                var ord = db.GetOrdinaryHours(model.UserId, model.Date_From.DayOfWeek);
                if((model.Time_To - model.Time_From) > ord)
                    ModelState.AddModelError("", "Il permesso non può essere maggiore delle ore ordinarie");
            }
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateHoliday", model);
            }
            var user = db.Users.Find(model.UserId);
            model.User = user;
            model.Status = Status_Request.Attesa;
            model.Request_Date = DateTime.Now;
            model.Response_Date = new DateTime();
            model.UserApproveId = null;
            // Invio Email
            try
            {
                SendEmailHoliday(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", string.Format("Impossibile Inviare la Richiesta: {0}", ex.Message));
                return PartialView("_CreateHoliday", model);
            }
            // Inserimento/Modifica in database
            try
            {
                db.Holidays.Add(model);
                db.SaveChanges();
                ViewBag.Message = "Aggiunta con Successo!";
                ViewBag.ReturnUrl = string.Format("/Holiday/Index?year={0}", model.Date_From.Year);
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_CreateHoliday", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult DeleteHoliday(int id, int year)
        {
            try
            {
                var entity = db.Holidays.Find(id);
                if (entity.Status == Status_Request.Approvata)
                    throw new Exception("Impossibile Eliminare Ferie Approvate");
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["DeleteMessage"] = "Annullata con Successo!";
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return RedirectToAction("Index", new { year = year });
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public PartialViewResult GetApproveHoliday(int id, bool status)
        {
            var model = db.Holidays.Find(id);
            if (model.Status == Status_Request.Approvata)
                ModelState.AddModelError("", "ATTENZIONE!! La richiesta era già approvata");
            model.Status = (status) ? Status_Request.Approvata : Status_Request.Negata;
            ViewBag.Title = (status) ? "Approva Richiesta" : "Nega Richiesta";
            return PartialView("_ApproveHoliday", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult ApproveHoliday(Holiday model)
        {
            var user = db.Users.Find(model.UserId);
            model.User = user;
            ViewBag.Title = (model.Status == Status_Request.Approvata) ? "Approva Richiesta" : "Nega Richiesta";
            if (!ModelState.IsValid)
            {
                return PartialView("_ApproveHoliday", model);
            }
            //var entity = db.Holidays.Find(model.Id);
            //entity.Status = model.Status;
            model.UserApproveId = User.Identity.Name;
            model.UserApprove = db.Users.Find(User.Identity.Name);
            model.Response_Date = DateTime.Now;
            // Invio Email
            try
            {
                SendEmailHoliday(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", string.Format("Impossibile Inviare la Richiesta: {0}", ex.Message));
                return PartialView("_ApproveHoliday", model);
            }
            // Modifica database
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = (model.Status == Status_Request.Approvata) ? "Approvata con Successo!" : "Negata con Successo!";
                ViewBag.ReturnUrl = string.Format("/Holiday/Manage?year={0}", model.Date_From.Year);
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_ApproveHoliday", model);
            }
        }

        private void SendEmailHoliday(Holiday request)
        {
            var message = this.RenderView("Email", request);
            var mail = new MailMessage();
            mail.From = new MailAddress("gestionale@gseautomation.it", "Gestionale (GSE)");

            if (request.Status == 0)
            {
                // Aggiungo le email degli amministratori
                foreach (var u in db.Users.Where(x => x.UserId != "Admin" && x.RoleId == 1).ToArray())
                {
                    mail.To.Add(u.Email);
                }
                //Metto in copia il richiedente
                mail.CC.Add(request.User.Email);
                // Oggetto Email
                mail.Subject = (request.Type == Models.Holiday.Holiday_Type.Ferie)
                    ? string.Format("Richiesta Ferie {0} {1} {2} - {3}", request.User.LastName, request.User.FirstName, request.Date_From.ToString("dd/MM/yyyy"), request.Date_To.ToString("dd/MM/yyyy"))
                    : string.Format("Richiesta Permesso {0} {1} {2} dalle {3} alle {4}", request.User.LastName, request.User.FirstName, request.Date_From.ToString("dd/MM/yyyy"), request.Time_From, request.Time_To);
            }
            else
            {
                //Invio al richiedente
                mail.CC.Add(request.User.Email);
                // Metto in copia gli amministratori
                foreach (var u in db.Users.Where(x => x.UserId != "Admin" && x.RoleId == 1).ToArray())
                {
                    mail.To.Add(u.Email);
                }
                // Oggetto Email
                mail.Subject = (request.Type == Models.Holiday.Holiday_Type.Ferie)
                    ? string.Format("Re: Richiesta Ferie {0} {1} {2} - {3}", request.User.LastName, request.User.FirstName, request.Date_From.ToString("dd/MM/yyyy"), request.Date_To.ToString("dd/MM/yyyy"))
                    : string.Format("Re: Richiesta Permesso {0} {1} {2} dalle {3} alle {4}", request.User.LastName, request.User.FirstName, request.Date_From.ToString("dd/MM/yyyy"), request.Time_From, request.Time_To);
            }

            mail.Body = message;
            mail.Priority = MailPriority.High;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            var server = new SmtpClient();
            server.Timeout = 10000;
            server.Send(mail);
        }
    }
}
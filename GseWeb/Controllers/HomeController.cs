using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GseWeb.Controllers
{
    public class HomeController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader, Employee")]
        public ActionResult Index()
        {

            //var r = db.getHours(2017, 5);
            //var o = db.Orders.Find("ENG04101601");
            //var u1 = Models.Report.ReportOrderLabor_User.GetFromOrder(o, 2017, 4);
            //var m = Models.Report.ReportOrderLabor_Month.GetFromOrder(o);
            //var r = Models.Report.ReportOrderLabor_User.GetFromOrder(o);


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
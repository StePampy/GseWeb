using GseWeb.DAL;
using GseWeb.Models.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace GseWeb.Controllers
{
    public class MaterialController : Controller
    {
        private DAL.GestionaleDB db = new DAL.GestionaleDB();
        
        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public PartialViewResult GetMaterial(int Id = -1, string OrderId = "", string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = (Id == -1) ? "Aggiungi Materiale" : "Modifica Materiale";
            var model = (Id == -1) ? new Material { Id = Id, OrderId = OrderId } : db.Materials.Find(Id);
            return PartialView("_CreateEditMaterial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TeamLeader")]
        public PartialViewResult CreateEditMaterial(Material model, string returnUrl = "")
        {
            ViewBag.Title = (model.Id == -1) ? "Aggiungi Materiale" : "Modifica Materiale";
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEditMaterial", model);
            }
            // Inserimento/Modifica in database
            try
            {
                model.Date = DateTime.Now;
                db.Materials.AddOrUpdate(m => m.Id, model);
                db.SaveChanges();
                ViewBag.Message = (model.Id == -1) ? "Aggiunto con Successo!" : "Modificato con Successo!";
                ViewBag.ReturnUrl = returnUrl;
                return PartialView("../Shared/_Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.MySqlExMessage());
                return PartialView("_CreateEditMaterial", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult DeleteMaterial(int Id, string returnUrl = "")
        {
            try
            {
                var entity = db.Materials.Find(Id);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["DeleteMessage"] = "Eliminato con Successo!";
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = string.Format("[Errore] {0}", ex.MySqlExMessage());
            }
            return Redirect(returnUrl);
        }

        [HttpGet]
        [Authorize(Roles = "Manager, TeamLeader")]
        public ActionResult Export(string OrderId, string returnUrl)
        {
            // Prelevo i materiali dal database
            var materials = db.Orders.Find(OrderId)
                .Materials.ToArray().OrderBy(x => x.Supplier).ThenByDescending(x => x.Date);
            var excludeProp = new string[] { "Id", "OrderId", "Order" };
            var dt = ToDataTable(materials, true, excludeProp);
            // Export Excel
            using (XLWorkbook wb = new XLWorkbook())
            {

                //Aggiungo il foglio del report
                wb.Worksheets.Add(dt, "Materiali_" + OrderId);

                //Export the Excel file.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/vnd.ms-excel";

                // Costruisco il nome del file in base alle informazioni
                var filename = string.Format("{0}_Materiali_{1}", OrderId, DateTime.Now.ToString("yyyyMMddHHmmss"));

                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xlsx", filename));
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                return View();
            }
        }

        private DataTable ToDataTable<T>(IEnumerable<T> collection,bool DisplayNameColumn = false, string[] exclude = null)
        {
            // Filtro le proprietà escludendo quelle nella lista
            var pia = (exclude == null) ? typeof(T).GetProperties() : typeof(T).GetProperties().Where(x => !exclude.Contains(x.Name));
            // Estraggo le colonne della tabella
            var dca = pia.Select(pi => {
                return (pi.PropertyType.IsGenericType)
                ? new DataColumn(pi.Name, pi.PropertyType.GetGenericArguments()[0])
                : new DataColumn(pi.Name, pi.PropertyType);
            }).ToArray();
            // nuovo datatable
            var dt = new DataTable();
            // aggiungo le colonne
            dt.Columns.AddRange(dca);
            // Estraggo le righe dai dati
            var dra = collection.Select(x => {
                var dr = dt.NewRow();
                foreach (PropertyInfo pi in pia)
                {
                    dr[pi.Name] = pi.GetValue(x, null);
                }
                return dr;
            }).ToArray();
            // Estraggo la tabella
            dt = dra.CopyToDataTable();
            // Rinomino le colonne con DisplayName
            if (DisplayNameColumn)
            {
                foreach (PropertyInfo pi in pia)
                {
                    var name = GetDisplayAttributeName(pi);
                    dt.Columns[pi.Name].ColumnName = name;
                }
            }
            return dt;
        }

        private string GetDisplayAttributeName(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length == 0 ? pi.Name : ((DisplayAttribute)attributes[0]).Name;
        }

    }
}
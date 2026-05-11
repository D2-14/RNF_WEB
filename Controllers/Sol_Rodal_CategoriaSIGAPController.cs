using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Sol_Rodal_CategoriaSIGAPController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        public ActionResult Index()
        {
            return View(db.Vw_Sol_Rodal_CategoriaSIGAP.ToList());
        }

        // GET: Seg_Rol/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Seg_Rol/Create
        public ActionResult Create()
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Rodal_CategoriaSIGAP tbl_Sol_Rodal_CategoriaSIGAP = new Tbl_Sol_Rodal_CategoriaSIGAP();

            tbl_Sol_Rodal_CategoriaSIGAP.swcreatedby = objUs.intUsuario_id;
            tbl_Sol_Rodal_CategoriaSIGAP.swdatecreated = DateTime.Now;
            tbl_Sol_Rodal_CategoriaSIGAP.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_Rodal_CategoriaSIGAP.swdateupdated = DateTime.Now;

            return View(tbl_Sol_Rodal_CategoriaSIGAP);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Rodal_CategoriaSIGAP tbl_Sol_Rodal_CategoriaSIGAP)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Sol_Rodal_CategoriaSIGAP.Max(u => u.CategoriaSIGAP_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Sol_Rodal_CategoriaSIGAP.CategoriaSIGAP_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(obj => obj.Descripcion == tbl_Sol_Rodal_CategoriaSIGAP.Descripcion).Max(u => u.CategoriaSIGAP_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["CategoriaSIGAPerror"] = "Error: La categoria SIGAP ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Sol_Rodal_CategoriaSIGAP.Descripcion.Trim() == "")
            {
                Error = 1;
                TempData["CategoriaSIGAPerror"] = "Error: No puede grabar una categoría SIGAP vacia";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Sol_Rodal_CategoriaSIGAP.Add(tbl_Sol_Rodal_CategoriaSIGAP);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Sol_Rodal_CategoriaSIGAP);


        }


        // GET: Seg_Rol/Edit/5
        public ActionResult Edit(int id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Rodal_CategoriaSIGAP tbl_Sol_Rodal_CategoriaSIGAP = db.Tbl_Sol_Rodal_CategoriaSIGAP.Find(id);
            if (tbl_Sol_Rodal_CategoriaSIGAP == null)
            {
                return HttpNotFound();
            }

            tbl_Sol_Rodal_CategoriaSIGAP.swdateupdated = DateTime.Now;
            tbl_Sol_Rodal_CategoriaSIGAP.swupdatedby = objUs.intUsuario_id;

            return View(tbl_Sol_Rodal_CategoriaSIGAP);

        }

        // POST: Seg_Rol/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Sol_Rodal_CategoriaSIGAP tbl_Sol_Rodal_CategoriaSIGAP)
        {
            int Error = 0;

            try
            {


                if (Error == 0)
                {
                    db.Entry(tbl_Sol_Rodal_CategoriaSIGAP).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Sol_Rodal_CategoriaSIGAP);
            }
            catch
            {
                return View(tbl_Sol_Rodal_CategoriaSIGAP);
            }
        }

        // GET: Seg_Rol/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Seg_Rol/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

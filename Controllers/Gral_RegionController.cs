using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gral_RegionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();


        // GET: Gral_Region
        public ActionResult Index()
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

            var tbl_Gral_Region = db.Tbl_Gral_Region.Where(Obj=> Obj.Id_Region>0);
            return View(tbl_Gral_Region);

        }


        // GET: Gral_Region/Edit/5
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

            Tbl_Gral_Region tbl_Gral_Region = db.Tbl_Gral_Region.Find(id);

            return View(tbl_Gral_Region);
        }

        // POST: Gral_Region/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Region tbl_Gral_Region)
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


            tbl_Gral_Region.swupdatedby = objUs.intUsuario_id;
            tbl_Gral_Region.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_Gral_Region.swupdatedbyinterno = false;
            }
            else
            {
                tbl_Gral_Region.swupdatedbyinterno = true;
            }


            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tbl_Gral_Region).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                return View(tbl_Gral_Region);
            }
            catch
            {
                return View(tbl_Gral_Region);
            }
        }

        // GET: Gral_Region/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Gral_Region/Delete/5
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

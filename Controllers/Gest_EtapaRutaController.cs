using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gest_EtapaRutaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Gest_EtapaRuta
        public ActionResult Index()
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            return View(db.Tbl_Gest_EtapaRuta.Where(Obj=> Obj.EtapaRuta_id>0).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_EtapaRuta NuevaRuta = new Tbl_Gest_EtapaRuta();

            NuevaRuta.EtapaRuta_id = decimal.Parse(collection["R_NewCodigo"]);
            NuevaRuta.Descripcion = collection["R_NewNombre"];

            try
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gest_EtapaRuta.Add(NuevaRuta);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: No esperado" + ex.Data.ToString();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


    }
}

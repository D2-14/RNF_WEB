using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Rpt_Seg_UsuariosPorRegionController : Controller
    {
        // GET: Rpt_Seg_UsuariosPorRegion

        public ActionResult Busqueda()
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
                return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            return View();
        }


        public ActionResult Index(string Region, string SubRegion)
        {
            return View();
        }
    }
}
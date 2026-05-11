using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Finca_ProcedenciaExternaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        public ActionResult Probosque_EspeciesForestales(string No_Registro)
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


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();


            List<Tbl_RNF_Finca_Probosque_EspeciesForestales> tbl_RNF_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Probosque_EspeciesForestales
                                                                                                           where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                                           orderby d.Finca_id, d.Correlativo_id
                                                                                                           select d).ToList();

            if (tbl_RNF_Finca_Probosque_EspeciesForestales == null)
            {
                tbl_RNF_Finca_Probosque_EspeciesForestales = new List<Tbl_RNF_Finca_Probosque_EspeciesForestales>();
            }

            return View(tbl_RNF_Finca_Probosque_EspeciesForestales);
        }

        public ActionResult PinpepOld_EspeciesForestales(string No_Registro)
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


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();


            List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales> tbl_RNF_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesForestales
                                                                                                           where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_RNF_Finca_PinpepOld_EspeciesForestales == null)
            {
                tbl_RNF_Finca_PinpepOld_EspeciesForestales = new List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales>();
            }


            return View(tbl_RNF_Finca_PinpepOld_EspeciesForestales);
        }

        public ActionResult PinpepOld_EspeciesProteger(string No_Registro)
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



            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();



            List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger> tbl_RNF_Finca_PinpepOld_EspeciesProteger = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesProteger
                                                                                                       where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                                       orderby d.Rodal_id
                                                                                                       select d).ToList();

            if (tbl_RNF_Finca_PinpepOld_EspeciesProteger == null)
            {
                tbl_RNF_Finca_PinpepOld_EspeciesProteger = new List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger>();
            }


            return View(tbl_RNF_Finca_PinpepOld_EspeciesProteger);
        }
        public ActionResult Secorf_EspeciesForestales(string No_Registro)
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



            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();


            List<Tbl_RNF_Finca_Secorf_EspeciesForestales> tbl_RNF_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Secorf_EspeciesForestales
                                                                                                        where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_RNF_Finca_Secorf_EspeciesForestales == null)
            {
                tbl_RNF_Finca_Secorf_EspeciesForestales = new List<Tbl_RNF_Finca_Secorf_EspeciesForestales>();
            }


            return View(tbl_RNF_Finca_Secorf_EspeciesForestales);
        }
    }
}
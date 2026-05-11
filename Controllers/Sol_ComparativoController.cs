using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net;

namespace RNF_Web.Controllers
{
    public class Sol_ComparativoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Comparativo
        Tbl_Sol_Solicitud solicitud = new Tbl_Sol_Solicitud();
        Tbl_Sol_Solicitud datos_Solicitud(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
            tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                 where d.Solicitud_id == Solicitud_id
                                 select d).FirstOrDefault();
            solicitud = tbl_Sol_Solicitud;
            return solicitud;
        }
        public ActionResult Index(long? Solicitud_id)
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

            ViewBag.solicitud = Solicitud_id;

            return View();
        }

        public ActionResult Index_Comparativo(long? Solicitud_id)
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

            ViewBag.solicitud = Solicitud_id;

            return View();
        }

        public ActionResult Index_RNF(long? Solicitud_id)
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

            ViewBag.solicitud = Solicitud_id;

            return View();
        }

        public ActionResult PropietarioPersonaIndividual(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_PropietarioPersonaIndividual> lst = new List<Tbl_RNF_PropietarioPersonaIndividual>();
            if (tbl_Sol_Solicitud != null)
            {
                lst = (from d in db.Tbl_RNF_PropietarioPersonaIndividual
                        where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                        orderby d.PersonaIndividual_id
                        select d).ToList();
            }
            return View(lst);
        }
        public ActionResult PropietarioPersonaJuridica(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_PropietarioPersonaJuridica> lst = new List<Tbl_RNF_PropietarioPersonaJuridica>();
            if (tbl_Sol_Solicitud != null)
            {
            lst = (from d in db.Tbl_RNF_PropietarioPersonaJuridica
                                                            where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                            orderby d.PersonaJuridica_id
                                                            select d).ToList();
            }
            return View(lst);
        }

        public ActionResult ArrendatarioPersonaIndividual(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_ArrendatarioPersonaIndividual> lst = new List<Tbl_RNF_ArrendatarioPersonaIndividual>();
            if (tbl_Sol_Solicitud != null)
            {
                lst = (from d in db.Tbl_RNF_ArrendatarioPersonaIndividual
                       where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                       orderby d.PersonaIndividual_id
                       select d).ToList();
            }
            return View(lst);
        }
        public ActionResult ArrendatarioPersonaJuridica(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_ArrendatarioPersonaJuridica> lst = new List<Tbl_RNF_ArrendatarioPersonaJuridica>();
            if (tbl_Sol_Solicitud != null)
            {
                lst = (from d in db.Tbl_RNF_ArrendatarioPersonaJuridica
                       where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                       orderby d.PersonaJuridica_id
                       select d).ToList();
            }
            return View(lst);
        }

        public ActionResult RepresentanteLegal(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_RepresentanteLegal> lst = new List<Tbl_RNF_RepresentanteLegal>();
            if (tbl_Sol_Solicitud != null)
            {
            lst = (from d in db.Tbl_RNF_RepresentanteLegal
                                                    where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                    orderby d.RepresentanteLegal_id
                                                    select d).ToList();
            }
            return View(lst);
        }

        public ActionResult Finca(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_Finca> lst = new List<Tbl_RNF_Finca>();
            if(tbl_Sol_Solicitud != null)
            {
                lst = (from d in db.Tbl_RNF_Finca
                                           where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                           orderby d.Finca_Id
                                           select d).ToList();
            }
            return View(lst);
        }

        public ActionResult MotoSierra(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_Motosierra> lst = new List<Tbl_RNF_Motosierra>();
            if (tbl_Sol_Solicitud != null)
            {
            lst = (from d in db.Tbl_RNF_Motosierra
                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                   orderby d.Motosierra_id
                   select d).ToList();
            }

            return View(lst);
        }

        public ActionResult Empresa_Entidad(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = datos_Solicitud(Solicitud_id);
            List<Tbl_RNF_Empresa_Entidad> lst = new List<Tbl_RNF_Empresa_Entidad>();
            if (tbl_Sol_Solicitud != null)
            {
            lst = (from d in db.Tbl_RNF_Empresa_Entidad
                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                   select d).ToList();
            }
            return View(lst);
        }

        long sp_actualizacion_rnfsolicitud(string No_RegistroLiteral, int No_RegistroCorrelativo, decimal solicitudtipoid, long usuarioid, int esinterno)
        {

            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_RNF_Registro_Solicitud @NoRegistroLiteral , @NoRegistroCorrelativo , @SolicitudTipo_id , @UsuarioID , @EsInterno";

            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@NoRegistroLiteral", Value = No_RegistroLiteral, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@NoRegistroCorrelativo", Value = No_RegistroCorrelativo, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@SolicitudTipo_id", Value = solicitudtipoid, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@UsuarioID", Value = usuarioid, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EsInterno", Value = esinterno, Direction = ParameterDirection.Input }
            };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure> {
                new ResultFromStoreProcedure { id = 0, mensaje = "Fallo Desconocido", respuesta = 0 }
            };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            long idsolicitud = resultado[0].id;

            return idsolicitud;
        }


        public JsonResult PrimerActualizacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.01);

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_RegistroLiteral == model.No_RegistroLiteral && d.No_RegistroCorrelativo == model.No_RegistroCorrelativo
                                                 select d).FirstOrDefault();

            long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
            tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Solicitud_id == idsolicitud
                                                   select d).FirstOrDefault();
            string TxtMostrar = "{\"NumeroTemporal\":\""+tbl_Sol_Solicitud.Solicitud_NumeroTemporal+"\"}";
            return Json(TxtMostrar);
        }

        public JsonResult SegundaActualizacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.02);

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_RegistroLiteral == model.No_RegistroLiteral && d.No_RegistroCorrelativo == model.No_RegistroCorrelativo
                                                 select d).FirstOrDefault();

            long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
            tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Solicitud_id == idsolicitud
                                                   select d).FirstOrDefault();

            string TxtMostrar = "{\"NumeroTemporal\":\"" + tbl_Sol_Solicitud.Solicitud_NumeroTemporal + "\"}";
            return Json(TxtMostrar);
        }
    }
}
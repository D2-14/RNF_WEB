using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class Gral_SolicitudConfiguracionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        class Respuesta
        {
            public string Resultado { get; set; }
        }

        public ActionResult Edit(decimal id)
        {
            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = db.Tbl_Gral_SolicitudConfiguracionTipo.Find(id);

            return View(tbl_Gral_SolicitudConfiguracionTipo);

        }


        public ActionResult Index()
        {
            Usuario objUs = new Usuario();
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
            ViewBag.ConfiguracionTipoSolicitud_id = new SelectList(db.Tbl_Gral_SolicitudConfiguracionTipo, "SolicitudTipo_id", "Descripcion");

            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = new Tbl_Gral_SolicitudConfiguracionTipo();
            TempData["ConfiMessage"] = "";

            return View(tbl_Gral_SolicitudConfiguracionTipo);
        }


        [HttpPost]
        public ActionResult Index(Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo)
        {
            Usuario objUs = new Usuario();
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


            if (ModelState.IsValid)
            {
                db.Entry(tbl_Gral_SolicitudConfiguracionTipo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ConfiMessage"] = "Registro actualizado con éxito";
            }

            return View(tbl_Gral_SolicitudConfiguracionTipo);
        }

        public ActionResult PermisosAsignados(decimal SolicitudTipo_id)
        {
            Usuario objUs = new Usuario();
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
            Tbl_Gral_SolicitudConfiguracion valida = db.Tbl_Gral_SolicitudConfiguracion.Find(SolicitudTipo_id);
            if (valida == null)
            {
                Tbl_Gral_SolicitudConfiguracion configuracion = new Tbl_Gral_SolicitudConfiguracion();
                configuracion.SolicitudTipo_id = SolicitudTipo_id;
                db.Tbl_Gral_SolicitudConfiguracion.Add(configuracion);
                db.SaveChanges();
            }
            return View(db.Tbl_Gral_SolicitudConfiguracion.Find(SolicitudTipo_id));
        }

        public ActionResult PermisosRecibidos(long Solicitud_id, string controllerName, int EsInterno)
        {
            bool boolEsInterno = false;
            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.controllerName = controllerName;
            ViewBag.EsInterno = EsInterno;
            if (EsInterno == 1)
            {
                boolEsInterno = true;
            }
            ViewBag.boolEsInterno = boolEsInterno;

            fc_Gral_Sol_Configuracion_Result fc_Gral_Sol_Configuracion_Result = (from d in db.fc_Gral_Sol_Configuracion(Solicitud_id, controllerName, boolEsInterno)
                                                                                 select d).FirstOrDefault();

            return View(fc_Gral_Sol_Configuracion_Result);
        }

        public JsonResult GrabarCambiosfiguracionTipo(decimal SolicitudTipo_id, string FormatoSolicitud_Codigo, DateTime FormatoSolicitud_Fecha, string FormatoSolicitud_Version, string BoletaDeDecision_Codigo, DateTime BoletaDeDecision_Fecha, string BoletaDeDecision_Version, string InformeTecnico_Codigo, DateTime InformeTecnico_Fecha, string InformeTecnico_Version)
        {

            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = db.Tbl_Gral_SolicitudConfiguracionTipo.Find(SolicitudTipo_id);

            tbl_Gral_SolicitudConfiguracionTipo.FormatoSolicitud_Codigo = FormatoSolicitud_Codigo;
            tbl_Gral_SolicitudConfiguracionTipo.FormatoSolicitud_Fecha = FormatoSolicitud_Fecha;
            tbl_Gral_SolicitudConfiguracionTipo.FormatoSolicitud_Version = FormatoSolicitud_Version;

            tbl_Gral_SolicitudConfiguracionTipo.BoletaDeDecision_Codigo = BoletaDeDecision_Codigo;
            tbl_Gral_SolicitudConfiguracionTipo.BoletaDeDecision_Fecha = BoletaDeDecision_Fecha;
            tbl_Gral_SolicitudConfiguracionTipo.BoletaDeDecision_Version = BoletaDeDecision_Version;

            tbl_Gral_SolicitudConfiguracionTipo.InformeTecnico_Codigo = InformeTecnico_Codigo;
            tbl_Gral_SolicitudConfiguracionTipo.InformeTecnico_Fecha = InformeTecnico_Fecha;
            tbl_Gral_SolicitudConfiguracionTipo.InformeTecnico_Version = InformeTecnico_Version;

            db.Entry(tbl_Gral_SolicitudConfiguracionTipo).State = EntityState.Modified;
            db.SaveChanges();

            Respuesta respuesta = new Respuesta();
            respuesta.Resultado = "Registro Actualizado";
            return Json(JsonConvert.SerializeObject(respuesta));

        }

        public JsonResult GrabarCambiosPermisos(Tbl_Gral_SolicitudConfiguracion model)
        {
            Tbl_Gral_SolicitudConfiguracion configuracion = db.Tbl_Gral_SolicitudConfiguracion.Find(model.SolicitudTipo_id);

            if (configuracion != null)
            {
                configuracion.Usuario_CopiarDesdeRNF = model.Usuario_CopiarDesdeRNF;
                configuracion.Usuario_Actualizar = model.Usuario_Actualizar;
                configuracion.Usuario_Borrar = model.Usuario_Borrar;
                configuracion.Usuario_Agregar = model.Usuario_Agregar;
                configuracion.Propietario_CopiarDesdeRNF = model.Propietario_CopiarDesdeRNF;
                configuracion.Propietario_Editar = model.Propietario_Editar;
                configuracion.Propietario_Borrar = model.Propietario_Borrar;
                configuracion.Propietario_Agregar = model.Propietario_Agregar;
                configuracion.Representante_CopiarDesdeRNF = model.Representante_CopiarDesdeRNF;
                configuracion.Representante_Editar = model.Representante_Editar;
                configuracion.Representante_Borrar = model.Representante_Borrar;
                configuracion.Representante_Agregar = model.Representante_Agregar;
                configuracion.Finca_CopiarDesdeRNF = model.Finca_CopiarDesdeRNF;
                configuracion.Finca_Editar = model.Finca_Editar;
                configuracion.Finca_Borrar = model.Finca_Borrar;
                configuracion.Finca_Agregar = model.Finca_Agregar;
                configuracion.Rodal_CopiarDesdeRNF = model.Rodal_CopiarDesdeRNF;
                configuracion.Rodal_Editar = model.Rodal_Editar;
                configuracion.Rodal_Borrar = model.Rodal_Borrar;
                configuracion.Rodal_Agregar = model.Rodal_Agregar;
                configuracion.Poligono_CopiarDesdeRNF = model.Poligono_CopiarDesdeRNF;
                configuracion.Poligono_Agregar = model.Poligono_Agregar;
                configuracion.Cultivo_CopiarDesdeRNF = model.Cultivo_CopiarDesdeRNF;
                configuracion.Cultivo_Editar = model.Cultivo_Editar;
                configuracion.Cultivo_Editar = model.Cultivo_Editar;
                configuracion.Cultivo_Borrar = model.Cultivo_Borrar;
                configuracion.Cultivo_Agregar = model.Cultivo_Agregar;
                configuracion.Dasometricos_CopiarDesdeRNF = model.Dasometricos_CopiarDesdeRNF;
                configuracion.Dasometricos_Agregar = model.Dasometricos_Agregar;
                configuracion.SubirDocumentos = model.SubirDocumentos;
                configuracion.Motosierra_CopiarDesdeRNF = model.Motosierra_CopiarDesdeRNF;
                configuracion.Motosierra_Editar = model.Motosierra_Editar;
                configuracion.Motosierra_Borrar = model.Motosierra_Borrar;
                configuracion.Motosierra_Agregar = model.Motosierra_Agregar;
                configuracion.Empresa_Entidad_CopiarDesdeRNF = model.Empresa_Entidad_CopiarDesdeRNF;
                configuracion.Empresa_Entidad_Editar = model.Empresa_Entidad_Editar;
                configuracion.Empresa_Entidad_Borrar = model.Empresa_Entidad_Borrar;
                configuracion.Empresa_Entidad_Agregar = model.Empresa_Entidad_Agregar;
                db.Entry(configuracion).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.Tbl_Gral_SolicitudConfiguracion.Add(model);
                db.SaveChanges();
            }
            Respuesta respuesta = new Respuesta();
            respuesta.Resultado = "Registro Actualizado";
            return Json(JsonConvert.SerializeObject(respuesta));
        }


        public JsonResult VerificarPermisosSolicitudTipo(decimal SolicitudTipo_id)
        {
            int solicitudtipoid = int.Parse(SolicitudTipo_id.ToString("0"));
            Tbl_Gral_SolicitudConfiguracionCategoria tbl_Gral_SolicitudConfiguracionCategoria = db.Tbl_Gral_SolicitudConfiguracionCategoria.Find(solicitudtipoid);

            return Json(tbl_Gral_SolicitudConfiguracionCategoria);
        }

    }
}

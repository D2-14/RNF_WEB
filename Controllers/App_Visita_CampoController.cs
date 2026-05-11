using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
namespace RNF_Web.Controllers
{
    public class App_Visita_CampoController : Controller
    {
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();
        db_RNFEntities db = new db_RNFEntities();
        AppAudit oAudit = new AppAudit();

        // GET: App_Visita_Campo
        public ActionResult Index(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();


            TempData["Mensaje"] = "";

            ViewBag.guidid = Guid_id;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            ViewBag.Categoria_id = tbl_Sol_Solicitud.Categoria_id;

            return View();

        }

        public ActionResult Historial(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.guidid = Guid_id;
            List<fc_Reporte_Auditorias_Result> lst = new List<fc_Reporte_Auditorias_Result>();
            lst = (from Obj in db_API.fc_Reporte_Auditorias(Guid_id)
                   orderby Obj.Solicitud_id, Obj.swdatetransfered descending
                   select Obj).ToList();

            ViewBag.guidid = Guid_id;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View(lst);
        }

        public ActionResult Fincas_API(long Solicitud_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            List<Tbl_API_Sol_Finca> tbl_API_Sol_Fincas = db_API.Tbl_API_Sol_Finca.Where(Obj => Obj.Solicitud_id == Solicitud_id).ToList();
            if (tbl_API_Sol_Fincas == null)
            {
                tbl_API_Sol_Fincas = new List<Tbl_API_Sol_Finca>();
            }
            return View(tbl_API_Sol_Fincas);
        }

        public ActionResult EstimacionVolumen_Tecnico(long Solicitud_id, long Finca_Id)
        {
            List<fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db_API.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica(Solicitud_id, Finca_Id).ToList()
                                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.Clase
                                                                                               select d).ToList();

            int CantidadEspecies = db_API.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_API_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Solicitud_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_Id = Finca_Id;
            return View(validacionesDiametrica);
        }


        public ActionResult ResumenPV_FS(long Solicitud_id)
        {
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + Solicitud_id + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV>();
            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado == null)
            {
                Resultado = new List<ClassResumenPV>();
            }

            return View(Resultado);
        }


        public ActionResult Auditoria(string Auditoria_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.auditoriaid = Auditoria_id;

            ViewBag.guidid = Guid_id;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View();
        }

        public ActionResult PonderacionDescuento(string Guid_id)
        {
            string guidid;
            guidid = Guid_id;
            ViewBag.guidid = Guid_id;
            Tbl_Sol_Solicitud oPonderacion = (from d in db.Tbl_Sol_Solicitud
                                              where d.Guid_id == guidid
                                              select d).FirstOrDefault();
            if (oPonderacion.PonderacionEvaluacionTecnicaDeArea_id == null)
            {
                oPonderacion.PonderacionEvaluacionTecnicaDeArea_id = 0;
            }
            ViewBag.PonderacionEvaluacionTecnicaDeArea_id = new SelectList(db.Tbl_Gral_EvaluacionPonderacion, "PonderacionEvaluacion_id", "Descripcion", oPonderacion.PonderacionEvaluacionTecnicaDeArea_id);

            return View(oPonderacion);
        }

        public ActionResult PonderacionDasometricos(string Guid_id)
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

            string guidid;
            guidid = Guid_id;
            ViewBag.guidid = Guid_id;

            Tbl_Sol_Solicitud oPonderacion = (from d in db.Tbl_Sol_Solicitud
                                              where d.Guid_id == guidid
                                              select d).FirstOrDefault();
            if (oPonderacion.PonderacionEvaluacionTecnicaDasometrica_id == null)
            {
                oPonderacion.PonderacionEvaluacionTecnicaDasometrica_id = 0;
            }
            ViewBag.PonderacionEvaluacionTecnicaDasometrica_id = new SelectList(db.Tbl_Gral_EvaluacionPonderacion, "PonderacionEvaluacion_id", "Descripcion", oPonderacion.PonderacionEvaluacionTecnicaDasometrica_id);


            ViewBag.Owner = 1;

            if (oPonderacion.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }


            return View(oPonderacion);
        }

        public ActionResult PanelAsignacion(string Guid_id)
        {
            string guidid;
            guidid = Guid_id;
            ViewBag.guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == guidid
                                            select d).FirstOrDefault();

            return View(oSolicitud);
        }

        public ActionResult PermitirCambios(string Guid_id)
        {
            string guidid;
            guidid = Guid_id;
            ViewBag.guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == guidid
                                            select d).FirstOrDefault();

            if (oSolicitud.PermitirSubirDocumentos == null)
            {
                oSolicitud.PermitirSubirDocumentos = false;
            }

            if (oSolicitud.PermitirCambioDePropietarioRepresentante == null)
            {
                oSolicitud.PermitirCambioDePropietarioRepresentante = false;
            }

            if (oSolicitud.PermitirCambioFincaRodalesDasometricos == null)
            {
                oSolicitud.PermitirCambioFincaRodalesDasometricos = false;
            }

            if (oSolicitud.PermitirCambiosEnDatosMotosierra == null)
            {
                oSolicitud.PermitirCambiosEnDatosMotosierra = false;
            }

            return View(oSolicitud);
        }

        public ActionResult ArbolesDiferenteEspecie(string Auditoria_id)
        {
            string auditoriaid;
            auditoriaid = Auditoria_id;

            List<fc_Reporte_Auditorias_Arboles_DiferenteEspecie_Result> lst = new List<fc_Reporte_Auditorias_Arboles_DiferenteEspecie_Result>();
            lst = (from Obj in db_API.fc_Reporte_Auditorias_Arboles_DiferenteEspecie(auditoriaid)
                   orderby Obj.Finca_id, Obj.Rodal_id, Obj.No_Parcela, Obj.Dasometrico_id
                   select Obj).ToList();

            return View(lst);
        }

        public ActionResult ArbolesDiferenteDAPyAltura(string Auditoria_id)
        {
            string auditoriaid;
            auditoriaid = Auditoria_id;

            List<fc_Reporte_Auditorias_Arboles_DiferenteDAP_y_Altura_Result> lst = new List<fc_Reporte_Auditorias_Arboles_DiferenteDAP_y_Altura_Result>();
            lst = (from Obj in db_API.fc_Reporte_Auditorias_Arboles_DiferenteDAP_y_Altura(auditoriaid)
                   orderby Obj.Finca_id, Obj.Rodal_id, Obj.No_Parcela, Obj.Dasometrico_id
                   select Obj).ToList();

            return View(lst);
        }

        public ActionResult ArbolesDiferenteAnioEstablecimiento(string Auditoria_id)
        {
            string auditoriaid;
            auditoriaid = Auditoria_id;

            List<fc_Reporte_Auditorias_Arboles_DiferenteAnio_Establecimiento_Result> lst = new List<fc_Reporte_Auditorias_Arboles_DiferenteAnio_Establecimiento_Result>();
            lst = (from Obj in db_API.fc_Reporte_Auditorias_Arboles_DiferenteAnio_Establecimiento(auditoriaid)
                   orderby Obj.Finca_id, Obj.Rodal_id, Obj.No_Parcela, Obj.Dasometrico_id
                   select Obj).ToList();

            return View(lst);
        }


        public ActionResult Arboles_x_Clase(string Auditoria_id)
        {
            string auditoriaid;
            int claseid;
            auditoriaid = Auditoria_id;
            Guid auddi = Guid.Parse(auditoriaid);
            List<fc_Reporte_Auditorias_Arboles_DiferenteClase_y_EstadoFitosanitario_Result> lst = null;
            Tbl_API_Sol_Rodal_Dasometrico tbl_API_Sol_Rodal_Dasometrico = db_API.Tbl_API_Sol_Rodal_Dasometrico.Where(Obj => Obj.Auditoria_id == auddi).FirstOrDefault();
            if (tbl_API_Sol_Rodal_Dasometrico != null)
            { 
                lst = new List<fc_Reporte_Auditorias_Arboles_DiferenteClase_y_EstadoFitosanitario_Result>();
                lst = (from Obj in db_API.fc_Reporte_Auditorias_Arboles_DiferenteClase_y_EstadoFitosanitario(auditoriaid)
                       orderby Obj.Finca_id, Obj.Rodal_id, Obj.No_Parcela, Obj.Dasometrico_id
                       select Obj).ToList();

                Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_API_Sol_Rodal_Dasometrico.Solicitud_id);
                ViewBag.tbl_Sol_Solicitud = tbl_Sol_Solicitud;
            }
            return View(lst);
        }

        public ActionResult Promedio_DAP_Altura(string Auditoria_id)
        {
            string auditoriaid;
            auditoriaid = Auditoria_id;

            List<App_PromedioDAP_Altura> lst = new List<App_PromedioDAP_Altura>();
            lst = (from d in db_API.fc_Reporte_Auditorias_Arboles_PromedioDAP_y_Altura(auditoriaid)
                   orderby d.Finca_id, d.Rodal_id, d.No_Parcela, d.Cantidad, d.Tipo_de_Area_id
                   select new App_PromedioDAP_Altura
                   {
                       Cantidad = d.Cantidad ?? 0,
                       Finca_id = d.Finca_id,
                       Rodal_id = d.Rodal_id,
                       Tipo_de_Area = d.Tipo_de_Area,
                       Tipo_de_Area_id = d.Tipo_de_Area_id,
                       No_Parcela = d.No_Parcela,
                       Especie_Indicada = d.Especie_Indicada,
                       DAP_Promedio = d.DAP_Promedio ?? 0,
                       DAP_Promedio_Medido = d.DAP_Promedio_Medido ?? 0,
                       Promedio_Altura = d.Promedio_Altura ?? 0,
                       Promedio_Altura_Medido = d.Promedio_Altura_Medido ?? 0,
                       Diferencia_DAP = d.Diferencia_DAP ?? 0,
                       Diferencia_Altura = d.Diferencia_Altura ?? 0,
                       Observacion = d.Observacion
                   }).ToList();

            return View(lst);
        }

        public ActionResult AreasDeDescuentoNuevas(string Auditoria_id)
        {
            Guid auditoriaid;
            auditoriaid = Guid.Parse(Auditoria_id);
            List<Tbl_API_Sol_Rodal_Descuento_Local> lst = new List<Tbl_API_Sol_Rodal_Descuento_Local>();
            lst = (from Obj in db_API.Tbl_API_Sol_Rodal_Descuento_Local
                   where Obj.Auditoria_id == auditoriaid
                   orderby Obj.Auditoria_id, Obj.Solicitud_id, Obj.Finca_id, Obj.Rodal_Id, Obj.Rodal_Descuento_Id
                   select Obj).ToList();


            return View(lst);
        }

        public ActionResult EspeciesAgrupadas(string Auditoria_id)
        {
            string auditoriaid;
            auditoriaid = Auditoria_id;


            return View();
        }

        public ActionResult EspeciesNuevasAgrupadas(string Auditoria_id)
        {
            Guid guidauditoriaid;
            string auditoriaid;
            auditoriaid = Auditoria_id;
            guidauditoriaid = Guid.Parse(auditoriaid);

            List<fc_Reporte_Auditorias_ArbolesNuevosAgrupados_PromedioDAP_y_Altura_Result> lst = new List<fc_Reporte_Auditorias_ArbolesNuevosAgrupados_PromedioDAP_y_Altura_Result>();
            lst = (from d in db_API.fc_Reporte_Auditorias_ArbolesNuevosAgrupados_PromedioDAP_y_Altura(auditoriaid)
                   orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela, d.Arboles, d.Especie
                   select d).ToList();

            return View(lst);
        }

        public ActionResult EspeciesNuevasAgregadas(string Auditoria_id)
        {
            Guid guidauditoriaid;
            string auditoriaid;
            auditoriaid = Auditoria_id;
            guidauditoriaid = Guid.Parse(auditoriaid);

            List<fc_Reporte_Auditorias_ArbolesNuevos_PromedioDAP_y_Altura_Result> lst = new List<fc_Reporte_Auditorias_ArbolesNuevos_PromedioDAP_y_Altura_Result>();
            lst = (from d in db_API.fc_Reporte_Auditorias_ArbolesNuevos_PromedioDAP_y_Altura(auditoriaid)
                   orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela, d.Arboles, d.Especie
                   select d).ToList();

            return View(lst);
        }

        public ActionResult PoligonoDescuentoFinca(string Auditoria_id, long Finca_id, long Rodal_id = 0)
        {
            string auditoriaid;
            Guid guidauditoriaid;
            long fincaid, rodalid;
            auditoriaid = Auditoria_id;
            guidauditoriaid = Guid.Parse(auditoriaid);
            fincaid = Finca_id;
            rodalid = Rodal_id;
            ViewBag.auditoriaid = Auditoria_id;
            IEnumerable<fc_Reporte_Auditorias_Descuento_RodalPoligonoPlano_Result> lst = (from Obj in db_API.fc_Reporte_Auditorias_Descuento_RodalPoligonoPlano(auditoriaid, fincaid, rodalid)
                                                                                          select Obj).ToList();

            Tbl_API_Sol_Solicitud oSolicitante = new Tbl_API_Sol_Solicitud();
            oSolicitante = (from Obj in db_API.Tbl_API_Sol_Solicitud
                            where Obj.Auditoria_id == guidauditoriaid
                            select Obj).FirstOrDefault();

            Tbl_API_Sol_Finca oFinca = new Tbl_API_Sol_Finca();
            oFinca = (from Obj in db_API.Tbl_API_Sol_Finca
                      where Obj.Auditoria_id == guidauditoriaid && Obj.Finca_Id == fincaid
                      select Obj).FirstOrDefault();

            ViewBag.oSolicitante = oSolicitante;
            ViewBag.oFinca = oFinca;

            return View(lst);
        }

        [HttpPost]
        public JsonResult PermitirDocumentos(string Guid_id, bool Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PermitirSubirDocumentos = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult PermitirPropietarioRepresentante(string Guid_id, bool Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PermitirCambioDePropietarioRepresentante = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult PermitirFincaRodalesDasometricos(string Guid_id, bool Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PermitirCambioFincaRodalesDasometricos = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult PermitirCambiosEnDatosMotosierra(string Guid_id, bool Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PermitirCambiosEnDatosMotosierra = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult EvaluacionPoligono(string Guid_id, int Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PonderacionEvaluacionTecnica_id = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult EvaluacionDescuento(string Guid_id, int Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PonderacionEvaluacionTecnicaDeArea_id = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        [HttpPost]
        public JsonResult EvaluacionDasometrico(string Guid_id, int Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PonderacionEvaluacionTecnicaDasometrica_id = Opcion;
            db.SaveChanges();
            return Json(null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;


namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoLocationController : Controller
    {

        AppAudit oAudit = new AppAudit();
        db_RNFEntities db = new db_RNFEntities();
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();

        // GET: App_Visita_Campo_Locacion
        public ActionResult Index(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.guidid = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View();
        }



        LocacionDatosSolicitante_API ObtenerDatosSolicitante(long Solicitud_id, long Finca_id = 0, int Tipo_de_Area = 0, long Rodal_id = 0)
        {

            LocacionDatosSolicitante_API oResultado = new LocacionDatosSolicitante_API();

            /*
                *******Reglas*******
            
                Todos los rodales

                fc_Sol_Finca_Rodal_Planos   Solicitud, 0, 0

                Todos los rodales de la finca 1

                fc_Sol_Finca_Rodal_Planos   Solicitud, 1, 0

                Todos los rodales de la rodal 1, finca 1

                fc_Sol_Finca_Rodal_Planos   Solicitud, 1, 1
            
            */

            long fincaid = 0;


            IEnumerable<Tbl_Sol_PropietarioPersonaJuridica> tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                                  where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                  select d);

            IEnumerable<Tbl_Sol_PropietarioPersonaIndividual> tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                                      where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                      select d);

            oResultado.Nombre = "";

            if (tbl_Sol_PropietarioPersonaIndividual != null)
            {

                foreach (var personasIndividuales in tbl_Sol_PropietarioPersonaIndividual)
                {
                    if (oResultado.Nombre != "")
                    {
                        oResultado.Nombre = oResultado.Nombre + ", ";
                    }

                    oResultado.Nombre = oResultado.Nombre + personasIndividuales.Nombres + " " + personasIndividuales.Apellidos;

                }

            }

            if (tbl_Sol_PropietarioPersonaJuridica != null)
            {


                foreach (var personasjuridica in tbl_Sol_PropietarioPersonaJuridica)
                {
                    if (oResultado.Nombre != "")
                    {
                        oResultado.Nombre = oResultado.Nombre + ", ";
                    }

                    oResultado.Nombre = oResultado.Nombre + personasjuridica.Nombre;
                }

            }

            oResultado.Direccion = "";

            if (Finca_id == 0)
            {

                foreach (var finca in db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == Solicitud_id))
                {
                    if (oResultado.Direccion != "")
                    {
                        oResultado.Direccion = oResultado.Direccion + ", ";
                    }
                    oResultado.Direccion = finca.Ubicacion ?? "" + " " + finca.Tbl_Gral_Departamento.Departamento + ", " + finca.Tbl_Gral_Municipio.Municipio;
                }


            }

            List<fc_API_Sol_Finca_Rodal_Planos_Result> oDatosPlanos = (from d in db_API.fc_API_Sol_Finca_Rodal_Planos(Solicitud_id, Finca_id, Tipo_de_Area, Rodal_id)
                                                                       select d).ToList();

            for (int i = 0; i < oDatosPlanos.Count(); i++)
            {
                oResultado.FechaDia = oDatosPlanos[i].FechaDia;
                oResultado.Registro = oDatosPlanos[i].Categoria_Registro;
                oResultado.AreaEfectiva += oDatosPlanos[i].AreaEfectiva ?? 0;
                oResultado.LongitudEfectiva += oDatosPlanos[i].Longitud_Total ?? 0;
                if ((fincaid != oDatosPlanos[i].Finca_id) && (fincaid != 0))
                {
                    fincaid = oDatosPlanos[i].Finca_id;
                    oResultado.Fincas += $", {fincaid}";
                }
                if ((fincaid != oDatosPlanos[i].Finca_id) && (fincaid == 0))
                {
                    fincaid = oDatosPlanos[i].Finca_id;
                    oResultado.Fincas = $"{fincaid}";
                }

            }

            oResultado.Solicitud_id = Solicitud_id;
            oResultado.Finca_id = Finca_id;
            oResultado.Rodal_id = Rodal_id;

            return oResultado;
        }


        public ActionResult IndexRodal(string Guid_id, long Rodal_id)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.Guid_id = Guid_id;

            //int CantidadRodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.Guid_id == Guid_id).Count();
            //ViewBag.CantidadRodales = CantidadRodales;

            //Session[Constants.session_Rodal] = (long)Rodal_id;

            //IEnumerable<fc_RNF_Sel_RodalPoligono_Result> lista = (from c in db.fc_RNF_Sel_RodalPoligono(Guid_id, Rodal_id)
            //                                                      select c);

            //ViewBag.Rodal_id = Rodal_id;
            //return View(lista);

            return View();

        }

        public ActionResult IndexRodalPlano(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            if (tbl_Sol_Solicitud == null) 
            { 
                return RedirectToAction("../Home/AccesoDenegado");
            }

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            LocacionDatosSolicitante_API oDatosSolicitante = ObtenerDatosSolicitante(Solicitud_id: (long)tbl_Sol_Solicitud.Solicitud_id, Tipo_de_Area: Tipo_de_Area, Rodal_id: Rodal_id);

            ViewBag.LlenarCampos = oDatosSolicitante;

            Tbl_Gral_VersionesDocumetoISO VersionesDocumetoISO = db.Tbl_Gral_VersionesDocumetoISO.Where(Obj => Obj.Documento == "PlanoTecnico" && Obj.estado_id == true).FirstOrDefault();

            ViewBag.VersionesDocumetoISO = VersionesDocumetoISO;

            ViewBag.VersionesDocumetoISO.Codigo = "---";
            ViewBag.VersionesDocumetoISO.Version = "---";
            ViewBag.VersionesDocumetoISO.Fecha = "---";

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;
            Session[Constants.session_Finca] = (long)Finca_id;
            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Result> lista = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlano((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area)
                                                                                      select d);
            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen_Result> listaResumen = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area)
                                                                                              select d);

            ViewBag.Finca_id = Finca_id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Tipo_de_Area = Tipo_de_Area;

            ViewBag.listaResumen = listaResumen;



            return View(lista);

        }

        public ActionResult IndexRodalPlanoTecnico(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            LocacionDatosSolicitante_API oDatosSolicitante = ObtenerDatosSolicitante(Solicitud_id: (long)tbl_Sol_Solicitud.Solicitud_id, Tipo_de_Area: Tipo_de_Area, Rodal_id: Rodal_id);

            ViewBag.LlenarCampos = oDatosSolicitante;

            Tbl_Gral_VersionesDocumetoISO VersionesDocumetoISO = db.Tbl_Gral_VersionesDocumetoISO.Where(Obj => Obj.Documento == "PlanoTecnico" && Obj.estado_id == true).FirstOrDefault();

            ViewBag.VersionesDocumetoISO = VersionesDocumetoISO;

            ViewBag.VersionesDocumetoISO.Codigo = "---";
            ViewBag.VersionesDocumetoISO.Version = "---";
            ViewBag.VersionesDocumetoISO.Fecha = "---";

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;
            Session[Constants.session_Finca] = (long)Finca_id;
            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario_Result> lista = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area, true, false)
                                                                                      select d);

            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen_Result> listaResumen = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area)
                                                                                                     select d);

            ViewBag.Finca_id = Finca_id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Tipo_de_Area = Tipo_de_Area;

            ViewBag.listaResumen = listaResumen;



            return View(lista);

        }
    

        public ActionResult IndexRodalPlanoUsuario(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            LocacionDatosSolicitante_API oDatosSolicitante = ObtenerDatosSolicitante(Solicitud_id: (long)tbl_Sol_Solicitud.Solicitud_id, Tipo_de_Area: Tipo_de_Area, Rodal_id: Rodal_id);

            ViewBag.LlenarCampos = oDatosSolicitante;

            Tbl_Gral_VersionesDocumetoISO VersionesDocumetoISO = db.Tbl_Gral_VersionesDocumetoISO.Where(Obj => Obj.Documento == "PlanoTecnico" && Obj.estado_id == true).FirstOrDefault();

            ViewBag.VersionesDocumetoISO = VersionesDocumetoISO;

            ViewBag.VersionesDocumetoISO.Codigo = "---";
            ViewBag.VersionesDocumetoISO.Version = "---";
            ViewBag.VersionesDocumetoISO.Fecha = "---";

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;
            Session[Constants.session_Finca] = (long)Finca_id;
            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario_Result> lista = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area, false, true)
                                                                                                     select d);

            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen_Result> listaResumen = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Resumen((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area)
                                                                                                     select d);

            ViewBag.Finca_id = Finca_id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Tipo_de_Area = Tipo_de_Area;

            ViewBag.listaResumen = listaResumen;



            return View(lista);

        }


        public JsonResult ObtenerCoordenadas()
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }


            IEnumerable<TAB_LOCATION> lst = (from c in db.TAB_LOCATION
                                             select c);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRodalCoordenadasPlano(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlano_Result> lst = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlano((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area)
                                                                                    select d);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRodalCoordenadasPlanoTecnico(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario_Result> lst = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area, true,false)
                                                                                    select d);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRodalCoordenadasPlanoUsuario(string Guid_id, long Finca_id, long Rodal_id, int Tipo_de_Area)
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            IEnumerable<fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario_Result> lst = (from d in db_API.fc_Reporte_Auditorias_Sel_RodalPoligonoPlanoTecnicoOUsuario((long)tbl_Sol_Solicitud.Solicitud_id, Finca_id, Rodal_id, Tipo_de_Area, false, true)
                                                                                                   select d);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

    }
}

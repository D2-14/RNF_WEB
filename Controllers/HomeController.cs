using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using RNF_Web.Models;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net.Mail;
using iTextSharp.text.html;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using static Common.Logging.Configuration.ArgUtils;
using static System.Net.WebRequestMethods;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Net;

namespace RNF_Web.Controllers
{

    public class HomeController : Controller
    {


private db_RNFEntities db = new db_RNFEntities();

        public object ExcelPackage { get; private set; }

        private string strCurrentStep = "";

        private string strsection_0 = "";
        private bool boolsection_0 = true;

        private string strsection_I = "";
        private bool boolsection_I = false;

        private string strsection_II = "";
        private bool boolsection_II = false;

        private string strsection_III = "";
        private bool boolsection_III = false;

        private string strsection_IV = "";
        private bool boolsection_IV = false;

        private string strsection_V = "";
        private bool boolsection_V = false;

        private string strsection_VI = "";
        private bool boolsection_VI = false;

        private string strsection_VII = "";
        private bool boolsection_VII = false;

        private string strsection_VIII = "";
        private bool boolsection_VIII = false;

        private string strsection_IX = "";
        private bool boolsection_IX = false;

        int intTbl_PersoneriaIndividual;
        int intTbl_PersoneriaJuridica;
        int intTbl_Sol_RepresentanteLegal;
        int intTbl_Sol_Motosierra;
        int intTbl_Sol_Finca;
        int intTbl_Sol_Rodal;
        int intTbl_Sol_Rodal_Dasometrico;

        int intTbl_Sol_Empresa_Entidad;
        int intTbl_Sol_Empresa_Entidad_Tipo_Registro;

        int intRegion_id;
        long lngSolicitud_id;

        bool boolProcedencia_Probosque = false;
        bool boolProcedencia_PinpepOld = false;
        bool boolProcedencia_PinpepNew = false;
        bool boolProcedencia_Secorf = false;
        bool boolProcedencia_Externa = false;

        public ActionResult GuiaEmpresa()
        {
            return View();
        }

        public ActionResult FalloFirmaElectronica(string FalloFe)
        {
            ViewBag.FalloFe = FalloFe;
            return View();
        }

        public ActionResult TestFirmaElectronica()
        {
            return View();
        }

        public ActionResult carrousel()
        {
            return View();
        }

        public JsonResult GestionaWhatsapp()
        {
            string TxtMostrar;

            Whatsapper whatsapper = new Whatsapper();
            if (Constants.EnviarWhatsApp == 1)
            {
                whatsapper.EnviarWhatsappPendiente();
            }
            TxtMostrar = "{\"Resultado\":\"Sí\"}";
            return Json(TxtMostrar);
        }

        public JsonResult GestionaCorreos()
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            string TxtMostrar;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                TxtMostrar = "{\"Resultado\":\"No\"}";
                return Json(TxtMostrar);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Mailer mailer = new Mailer();
            _ = mailer.EnviarCorreoPendiente(objUs);
            Whatsapper whatsapper = new Whatsapper();
            if (Constants.EnviarWhatsApp == 1)
            {
                whatsapper.EnviarWhatsappPendiente();
            }
            TxtMostrar = "{\"Resultado\":\"Sí\"}";
            return Json(TxtMostrar);
        }

        // GET: Sol_PersonaJuridica/Create
        public ActionResult RegistroAgregado()
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

            @TempData["MensajeHome"] = "";
            @TempData["MensajeHomeTwo"] = "";

            return View();
        }

        public ActionResult RegistroNoAgregado()
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

        public ActionResult FeedHistoryRSS(string Guidid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            TempData["Mensaje"] = "";



            string Query;

            //Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guidid).First();

            Query = $" Select \n";
            Query += $"     * \n";
            Query += $" From \n";
            Query += $"     Tbl_RSS_Usuario \n";
            Query += $" Where \n";
            Query += $"     Guid_Solicitud = '{Guidid}' \n";
            Query += $"  and Msg_ParaInterno = 1 order by \n";
            Query += $"     swdatecreated desc \n";

            List<Tbl_RSS_Usuario> tbl_RSS_Usuarios = new List<Tbl_RSS_Usuario>();

            tbl_RSS_Usuarios = db.Tbl_RSS_Usuario.SqlQuery(Query).ToList();

            return View(tbl_RSS_Usuarios);
        }

        public ActionResult ListarSolicitudUsuario(int limit = 0)
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

            List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = new List<Tbl_Sol_Solicitud>();


            string Query, QueryComplementoBusqueda;
            DateTime swdatecreated, ahora;
            ahora = DateTime.Now;
            QueryComplementoBusqueda = "";

            if (objUs.intUsuario_id == 583) //Usuario motosierra, no se debe cargar solicitudes en la pantalla de inicio ya que sobrecarga el load de la misma
            {

                Query = $" Select \n";
                Query += $"     * \n";
                Query += $" From \n";
                Query += $"     Tbl_Sol_Solicitud Sol \n";
                Query += $" where exists ( \n";
                Query += $"     select * \n";
                Query += $"     from Tbl_RSS_Usuario RSS \n";
                Query += $"     where RSS.Solicitud_id = Sol.Solicitud_id \n";
                Query += $"     and Datediff(MONTH, RSS.swdatecreated,getdate()) <= 6  \n";
                Query += $"     and RSS.UsuarioExterno_id = -1 \n";
                Query += $"     ) \n";
                Query += $" Union \n";
                Query += $" Select 0 Solicitud_id, Region_id, SubRegion_id, Categoria_id, Sub_Categoria_id, Sub_Sub_Categoria_id, No_Registro Guid_id, Estado_id, AreaTotalFincas, swdatecreated, swcreatedby, swcreatedbyinterno, swdateupdated, swupdatedby, swupdatedbyinterno, No_Registro Solicitud_NumeroTemporal, Expediente Solicitud_NumeroExpediente, '' FacturaSerie, null FacturaNumero, 0 CantidadFolios, getdate() FechaRecepcionExpedienteFisico, 1 RecepcionExpedienteFisicoby, 1 SecretariaAsignada_id, 1 TecnicoAsignado_id, 1 JuridicoAsignado_id, null JuridicoAsignadoNombramiento, null JuridicoAsignadoFecha, getdate() TecnicoAsignadoFecha,  null JuridicoAsignadoPorSubRegional_id, null TecnicoAsignadoPorSubRegional_id, 0 PonderacionEvaluacionTecnica_id,0 PonderacionEvaluacionTecnicaFormulario_id,0 PonderacionEvaluacionTecnicaDeArea_id, 0 PonderacionEvaluacionTecnicaDasometrica_id, 0 PonderacionEvaluacionJuridica_id, 1 TipoGestion_id, null PermitirCambioDePropietarioRepresentante, null PermitirCambioFincaRodalesDasometricos, null PermitirSubirDocumentos, No_Registro, No_RegistroLiteral, No_RegistroCorrelativo, ResolucionInscripcion, ResolucionInscripcionFecha, getdate() InscripcionFecha, SolicitudTipo_id, null EnmiendasRecibidas, TipoInactivacion_id, Descripcion_Inactivacion, null PermitirCambiosEnDatosMotosierra, null DepartamentoSolicitud_id, null MunicipioSolicitud_id, null PermitirCambioEnDatosTecnicoProfesional, DPI_Titular, '' DocumentoVinculado, Procedencia_Probosque, Procedencia_PinpepOld, Procedencia_PinpepNew, Procedencia_secorf, Procedencia_Expediente, Procedencia_POA, Procedencia_Licencia, Procedencia_Modalidad, Procedencia_Fase, Procedencia_NombreSolicitante, Procedencia_TipoProyecto, Procedencia_InformeTecnico, Procedencia_FechaInicioPeriodo, Procedencia_FechaFinPeriodo, Fecha_Inscripcion Fecha_De_Inscripcion_RNF, ResolucionInscripcion Resolucion_De_Inscripcion_RNF, Fecha_Inscripcion Fecha_De_ResolucionInscripcion_RNF, Fecha_Inscripcion FechaResolucionSolicitud, ResolucionInscripcion ResolucionSolicitud, InactivacionTecnicoTipo_id, Descripcion_InactivacionTecnico, Bitacora_id, Notificacion_Direccion, Notificacion_Municipio_id, Notificacion_Departamento_id \n";
                Query += $"   From Tbl_RNF_Registro \n";
                Query += $"    where exists( \n";
                Query += $"      select * \n";
                Query += $"      from Tbl_RSS_Usuario RSS  \n";
                Query += $"      where Datediff(MONTH, RSS.swdatecreated, getdate()) <= 6  \n";
                Query += $"      and RSS.UsuarioExterno_id = 36  \n";
                Query += $"      and RSS.EtapaSolicitud_GUID_id = No_Registro  \n";
                Query += $"      )   \n";

                Console.WriteLine("Query Tbl_RNF_Registro--> " + Query);

                tbl_Sol_Solicituds = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();
            }
            else
            {

                Query = $" Select \n";
                Query += $"     * \n";
                Query += $" From \n";
                Query += $"     Tbl_Sol_Solicitud Sol \n";
                Query += $" where exists ( \n";
                Query += $"     select * \n";
                Query += $"     from Tbl_RSS_Usuario RSS \n";
                Query += $"     where RSS.Solicitud_id = Sol.Solicitud_id \n";
                Query += $"     and Datediff(MONTH, RSS.swdatecreated,getdate()) <= 6  \n";
                Query += $"     and RSS.UsuarioExterno_id = {objUs.intUsuario_id} \n";
                Query += $"     ) \n";
                Query += $" Union \n";
                Query += $" Select 0 Solicitud_id, Region_id, SubRegion_id, Categoria_id, Sub_Categoria_id, Sub_Sub_Categoria_id, No_Registro Guid_id, Estado_id, AreaTotalFincas, swdatecreated, swcreatedby, swcreatedbyinterno, swdateupdated, swupdatedby, swupdatedbyinterno, No_Registro Solicitud_NumeroTemporal, Expediente Solicitud_NumeroExpediente, '' FacturaSerie, null FacturaNumero, 0 CantidadFolios, getdate() FechaRecepcionExpedienteFisico, 1 RecepcionExpedienteFisicoby, 1 SecretariaAsignada_id, 1 TecnicoAsignado_id, 1 JuridicoAsignado_id, null JuridicoAsignadoNombramiento, null JuridicoAsignadoFecha, getdate() TecnicoAsignadoFecha,  null JuridicoAsignadoPorSubRegional_id, null TecnicoAsignadoPorSubRegional_id, 0 PonderacionEvaluacionTecnica_id,0 PonderacionEvaluacionTecnicaFormulario_id,0 PonderacionEvaluacionTecnicaDeArea_id, 0 PonderacionEvaluacionTecnicaDasometrica_id, 0 PonderacionEvaluacionJuridica_id, 1 TipoGestion_id, null PermitirCambioDePropietarioRepresentante, null PermitirCambioFincaRodalesDasometricos, null PermitirSubirDocumentos, No_Registro, No_RegistroLiteral, No_RegistroCorrelativo, ResolucionInscripcion, ResolucionInscripcionFecha, getdate() InscripcionFecha, SolicitudTipo_id, null EnmiendasRecibidas, TipoInactivacion_id, Descripcion_Inactivacion, null PermitirCambiosEnDatosMotosierra, null DepartamentoSolicitud_id, null MunicipioSolicitud_id, null PermitirCambioEnDatosTecnicoProfesional, DPI_Titular, '' DocumentoVinculado, Procedencia_Probosque, Procedencia_PinpepOld, Procedencia_PinpepNew, Procedencia_secorf, Procedencia_Expediente, Procedencia_POA, Procedencia_Licencia, Procedencia_Modalidad, Procedencia_Fase, Procedencia_NombreSolicitante, Procedencia_TipoProyecto, Procedencia_InformeTecnico, Procedencia_FechaInicioPeriodo, Procedencia_FechaFinPeriodo, Fecha_Inscripcion Fecha_De_Inscripcion_RNF, ResolucionInscripcion Resolucion_De_Inscripcion_RNF, Fecha_Inscripcion Fecha_De_ResolucionInscripcion_RNF, Fecha_Inscripcion FechaResolucionSolicitud, ResolucionInscripcion ResolucionSolicitud, InactivacionTecnicoTipo_id, Descripcion_InactivacionTecnico, Bitacora_id, Notificacion_Direccion, Notificacion_Municipio_id, Notificacion_Departamento_id \n";
                Query += $"   From Tbl_RNF_Registro \n";
                Query += $"    where exists( \n";
                Query += $"      select * \n";
                Query += $"      from Tbl_RSS_Usuario RSS  \n";
                Query += $"      where Datediff(MONTH, RSS.swdatecreated, getdate()) <= 6  \n";
                Query += $"      and RSS.UsuarioExterno_id = 36  \n";
                Query += $"      and RSS.EtapaSolicitud_GUID_id = No_Registro  \n";
                Query += $"      )   \n";

                Console.WriteLine("Query Tbl_RNF_Registro--> " + Query);

            tbl_Sol_Solicituds = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();

            }
            if (tbl_Sol_Solicituds == null)
            {
                tbl_Sol_Solicituds = new List<Tbl_Sol_Solicitud>();
            }

            if (tbl_Sol_Solicituds.Count() > 0)
            {
                if (limit != 0)
                {
                    tbl_Sol_Solicituds = (from d in tbl_Sol_Solicituds
                                          orderby d.Solicitud_id descending
                                          select d).Take(limit).ToList();
                }
            }
            ViewBag.limit = limit;

            return View(tbl_Sol_Solicituds);
        }

        public class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }

        public JsonResult ListaSolicitudUsuarioHome(Solicitud_ListaFeedRSSRequest model)
        {
            RespuestaJSON respuestaJSON = new RespuestaJSON()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(respuestaJSON);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            //List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = (from d in db.Tbl_Sol_Solicitud
            //                                              where d.swcreatedby == objUs.intUsuario_id
            //                                              orderby d.Solicitud_id descending
            //                                              select d).ToList();



            List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = new List<Tbl_Sol_Solicitud>();


            string Query, QueryComplementoBusqueda;
            DateTime swdatecreated, ahora;
            ahora = DateTime.Now;
            QueryComplementoBusqueda = "";

           

                Query = $" Select \n";
                Query += $"     * \n";
                Query += $" From \n";
                Query += $"     Tbl_Sol_Solicitud Sol \n";
                Query += $" where exists ( \n";
                Query += $"     select * \n";
                Query += $"     from Tbl_RSS_Usuario RSS \n";
                Query += $"     where RSS.Solicitud_id = Sol.Solicitud_id \n";
                Query += $"     and Datediff(MONTH, RSS.swdatecreated,getdate()) <= 6  \n";
                Query += $"     and RSS.UsuarioExterno_id = {objUs.intUsuario_id} \n";
                Query += $"     ) \n";
                Query += $" Union \n";
                Query += $" Select 0 Solicitud_id, Region_id, SubRegion_id, Categoria_id, Sub_Categoria_id, Sub_Sub_Categoria_id, No_Registro Guid_id, Estado_id, AreaTotalFincas, swdatecreated, swcreatedby, swcreatedbyinterno, swdateupdated, swupdatedby, swupdatedbyinterno, No_Registro Solicitud_NumeroTemporal, Expediente Solicitud_NumeroExpediente, '' FacturaSerie, null FacturaNumero, 0 CantidadFolios, getdate() FechaRecepcionExpedienteFisico, 1 RecepcionExpedienteFisicoby, 1 SecretariaAsignada_id, 1 TecnicoAsignado_id, 1 JuridicoAsignado_id, null JuridicoAsignadoNombramiento, null JuridicoAsignadoFecha, getdate() TecnicoAsignadoFecha,  null JuridicoAsignadoPorSubRegional_id, null TecnicoAsignadoPorSubRegional_id, 0 PonderacionEvaluacionTecnica_id,0 PonderacionEvaluacionTecnicaFormulario_id,0 PonderacionEvaluacionTecnicaDeArea_id, 0 PonderacionEvaluacionTecnicaDasometrica_id, 0 PonderacionEvaluacionJuridica_id, 1 TipoGestion_id, null PermitirCambioDePropietarioRepresentante, null PermitirCambioFincaRodalesDasometricos, null PermitirSubirDocumentos, No_Registro, No_RegistroLiteral, No_RegistroCorrelativo, ResolucionInscripcion, ResolucionInscripcionFecha, getdate() InscripcionFecha, SolicitudTipo_id, null EnmiendasRecibidas, TipoInactivacion_id, Descripcion_Inactivacion, null PermitirCambiosEnDatosMotosierra, null DepartamentoSolicitud_id, null MunicipioSolicitud_id, null PermitirCambioEnDatosTecnicoProfesional, DPI_Titular, '' DocumentoVinculado, Procedencia_Probosque, Procedencia_PinpepOld, Procedencia_PinpepNew, Procedencia_secorf, Procedencia_Expediente, Procedencia_POA, Procedencia_Licencia, Procedencia_Modalidad, Procedencia_Fase, Procedencia_NombreSolicitante, Procedencia_TipoProyecto, Procedencia_InformeTecnico, Procedencia_FechaInicioPeriodo, Procedencia_FechaFinPeriodo, Fecha_Inscripcion Fecha_De_Inscripcion_RNF, ResolucionInscripcion Resolucion_De_Inscripcion_RNF, Fecha_Inscripcion Fecha_De_ResolucionInscripcion_RNF, Fecha_Inscripcion FechaResolucionSolicitud, ResolucionInscripcion ResolucionSolicitud, InactivacionTecnicoTipo_id, Descripcion_InactivacionTecnico, Bitacora_id, Notificacion_Direccion, Notificacion_Municipio_id, Notificacion_Departamento_id \n";
                Query += $"   From Tbl_RNF_Registro \n";
                Query += $"    where exists( \n";
                Query += $"      select * \n";
                Query += $"      from Tbl_RSS_Usuario RSS  \n";
                Query += $"      where Datediff(MONTH, RSS.swdatecreated, getdate()) <= 6  \n";
                Query += $"      and RSS.UsuarioExterno_id = 36  \n";
                Query += $"      and RSS.EtapaSolicitud_GUID_id = No_Registro  \n";
                Query += $"      )   \n";

          
            tbl_Sol_Solicituds = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();


            if (tbl_Sol_Solicituds == null)
            {
                tbl_Sol_Solicituds = new List<Tbl_Sol_Solicitud>();
            }

            List<Solicitud_ListaFeedRSS> solicitud_ListaFeedRSSes = new List<Solicitud_ListaFeedRSS>();

            if (tbl_Sol_Solicituds.Count() > 0)
            {
                if (model.limit != 0)
                {
                    tbl_Sol_Solicituds = (from d in tbl_Sol_Solicituds
                                          orderby d.Solicitud_id descending
                                          select d).Take(model.limit).ToList();
                }

                solicitud_ListaFeedRSSes = (from d in tbl_Sol_Solicituds
                                            select new Solicitud_ListaFeedRSS
                                            {
                                                Solicitud_id = d.Solicitud_id,
                                                Guid_Solicitud = d.Guid_id
                                            }).ToList();


                respuestaJSON = new RespuestaJSON()
                {
                    Result = 1,
                    Mensaje = "Solicitudes en cola",
                    data = solicitud_ListaFeedRSSes
                };

                return Json(respuestaJSON);

            }
            else
            {
                respuestaJSON = new RespuestaJSON()
                {
                    Result = 2,
                    Mensaje = "No hay solicitudes pendientes"
                };

                return Json(respuestaJSON);
            }

        }


        public ActionResult FeedRSS_Solicitud(long Solicitud_id, string Guid_Solicitud)
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

            string Query, QueryComplementoBusqueda;


            Query = $" Select \n";
            Query += $"     * \n";
            Query += $" From \n";
            Query += $"     Tbl_RSS_Usuario \n";
            Query += $" Where \n";
            Query += $"     UsuarioExterno_id = {objUs.intUsuario_id} \n";
            Query += $"     and Solicitud_id = '{Solicitud_id}' \n";
            Query += $" order by \n";
            Query += $"     swdatecreated desc \n";

            ViewBag.Guid_Solicitud = Guid_Solicitud;
            ViewBag.Solicitud_id = Solicitud_id;

            List<Tbl_RSS_Usuario> tbl_RSS_Usuarios = new List<Tbl_RSS_Usuario>();

            tbl_RSS_Usuarios = db.Tbl_RSS_Usuario.SqlQuery(Query).ToList();


            return View(tbl_RSS_Usuarios);
        }





        public ActionResult FeedRSS(string strDatoBusqueda = null)
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
            TempData["Mensaje"] = "";

            ViewBag.strDatoBusqueda = "";
            ViewBag.cantResult = "";
            if (strDatoBusqueda != null)
            {
                ViewBag.strDatoBusqueda = strDatoBusqueda;
            }

            string Query, QueryComplementoBusqueda;
            DateTime swdatecreated, ahora;
            ahora = DateTime.Now;
            QueryComplementoBusqueda = "";
            if ((strDatoBusqueda != null) && (strDatoBusqueda != "") && (strDatoBusqueda != "0"))
            {
                int monthts = int.Parse(strDatoBusqueda);
                swdatecreated = ahora.AddMonths(-monthts);
                QueryComplementoBusqueda = $" and swdatecreated between Convert(datetime, '{swdatecreated}', 103) and getdate() \n";
            }

            Query = $" Select \n";
            Query += $"     * \n";
            Query += $" From \n";
            Query += $"     Tbl_RSS_Usuario \n";
            Query += $" Where \n";
            Query += $"     UsuarioExterno_id = {objUs.intUsuario_id} \n";
            Query += $" {QueryComplementoBusqueda} \n";
            Query += $" order by \n";
            Query += $"     swdatecreated desc \n";

            List<Tbl_RSS_Usuario> tbl_RSS_Usuarios = new List<Tbl_RSS_Usuario>();

            tbl_RSS_Usuarios = db.Tbl_RSS_Usuario.SqlQuery(Query).ToList();
            //tbl_RSS_Usuarios = (from d in db.Tbl_RSS_Usuario
            //                    where d.UsuarioExterno_id == objUs.intUsuario_id
            //                    orderby d.RSS_id
            //                    select d).ToList();

            return View(tbl_RSS_Usuarios);
        }

        [HttpPost]
        public JsonResult UpdateRSS(int esinterno, long usuario_id = 0)
        {

            bool leidointerno, leidoexterno, interno;
            leidointerno = leidoexterno = false;
            List<Tbl_RSS_Usuario> update_Leido = new List<Tbl_RSS_Usuario>();
            interno = false;

            if (esinterno != 0)
            {
                interno = true;
            }

            if (usuario_id != 0)
            {

                if (interno)
                {
                    leidointerno = true;
                    update_Leido = (from Obj in db.Tbl_RSS_Usuario
                                    where Obj.UsuarioExterno_id == usuario_id && Obj.InternoLeido == false && Obj.Msg_ParaInterno == true
                                    select Obj).ToList();
                }
                else
                {
                    leidoexterno = true;
                    update_Leido = (from Obj in db.Tbl_RSS_Usuario
                                    where Obj.UsuarioExterno_id == usuario_id && Obj.ExternoLeido == false && Obj.Msg_ParaExterno == true
                                    select Obj).ToList();
                }

                if (update_Leido.Count() > 0)
                {
                    foreach (var item in update_Leido)
                    {
                        if (leidointerno == true)
                        {
                            item.InternoLeido = leidointerno;
                        }
                        else
                        {
                            item.ExternoLeido = leidoexterno;
                        }
                    }
                    db.SaveChanges();
                }

            }

            return Json(null);
        }

        public ActionResult FirmaElectronicaConProblemas(long Form_Formulario_id)
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

            String Query;

            Query = "Select Top 5 * ";
            Query += "from Tbl_Form_Formulario_FirmaElectronica_Bitacora ";
            Query += "Where Form_Formulario_id = " + Form_Formulario_id.ToString();
            Query += "Order by swdatecreated desc";


            List<Tbl_Form_Formulario_FirmaElectronica_Bitacora> tbl_Form_Formulario_FirmaElectronica_Bitacora = new List<Tbl_Form_Formulario_FirmaElectronica_Bitacora>();

            tbl_Form_Formulario_FirmaElectronica_Bitacora = db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.SqlQuery(Query).ToList();


            return View(tbl_Form_Formulario_FirmaElectronica_Bitacora.ToList());
        }

        public ActionResult EliminarSolicitudProcesada()
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

        public ActionResult EliminarSolicitud(int intEliminar, long id, string firma)
        {

            ViewBag.id = id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            if (tbl_sol_solicitud.Estado_id != 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            /// Es int porque 0 significa pendiente de BoVo.   Null significa proceda.
            /// No lo utilizo para enviar la solicitud id

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

            if (intEliminar == 0)
            {
                return View();
            }
            else
            {

                int Etapa_EnCreacion = Constants.Etapa_EnCreacion;
                int Etapa_Incompleta = Constants.Etapa_Incompleta;

                if ((tbl_sol_solicitud.Estado_id == Etapa_EnCreacion) || (tbl_sol_solicitud.Estado_id == Etapa_Incompleta))
                {
                    tbl_sol_solicitud.swupdatedby = objUs.intUsuario_id;
                    tbl_sol_solicitud.swdateupdated = DateTime.Now;


                    if (objUs.EsInterno != 1)
                    {
                        tbl_sol_solicitud.swupdatedbyinterno = false;
                    }
                    else
                    {
                        tbl_sol_solicitud.swupdatedbyinterno = true;
                    }

                    tbl_sol_solicitud.Estado_id = Constants.Etapa_EliminadaPorElUsuario;

                    db.Entry(tbl_sol_solicitud).State = EntityState.Modified;
                    db.SaveChanges();

                }

                return RedirectToAction("../Home/EliminarSolicitudProcesada");
            }
        }

        public ActionResult EnviarSolicitudProcesada()
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

        public ActionResult CorreccionesRequeridas(long solicitud_id, string firma)
        {
            long lngSolicitud_id = solicitud_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);
  

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.PuntosBloqueantes = db.fc_Sol_Sel_Revision(lngSolicitud_id).Where(Obj => Obj.Bloqueante == true);

            ViewBag.DocumentosBloqueantes = db.Tbl_Sol_DocumentosSolicitarAuxiliar.Where(Obj => Obj.Solicitud_id == lngSolicitud_id && Obj.Bloqueante == true);

            return View();
        }

        public ActionResult EnviarSolicitud(int intEnviar, long solicitud_id, string firma)
        {

            ViewBag.MensajeError = "";
            ViewBag.id = solicitud_id;
            ViewBag.firma = firma;

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);
      

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            string sqlQuery;
            SqlParameter[] sqlParams;


            if (intEnviar == 0)  // Cero significa realizarla revision... requerido.
            {

                sqlQuery = "Exec SP_Sol_Sel_RevisionDocumentosNoSubidos @Solicitud_id";

                sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input }
                    };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };


                // Importante: Verifica los documentos requeridos y deba el dato
                //             en una tabla auxiliar Tbl_Sol_DocumentosSolicitarAuxiliar

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();
                Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad_ = db.Tbl_Sol_Empresa_Entidad.Find(solicitud_id);
           

                int intPuntosBloqueantes = db.fc_Sol_Sel_Revision(tbl_sol_solicitud.Solicitud_id).Where(Obj => Obj.Bloqueante == true).Count();

                int intDocumentosBloqueantes = db.Tbl_Sol_DocumentosSolicitarAuxiliar.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Bloqueante == true).Count();

              
                decimal TipoGestion = tbl_sol_solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_solicitud.SolicitudTipo_id);               
                decimal terminacioninactivacion = 0.06M;

             

                    if ((tbl_Sol_Empresa_Entidad_ != null) && (TipoGestion != terminacioninactivacion))
                {
                    if ((intDocumentosBloqueantes > 0) || (intPuntosBloqueantes > 0) || (tbl_Sol_Empresa_Entidad_.email == null))
                    {
                        return RedirectToAction("../Home/CorreccionesRequeridas", new { solicitud_id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                    return View();
                }
                else
                {
                    if ((intDocumentosBloqueantes > 0) || (intPuntosBloqueantes > 0) && (TipoGestion != terminacioninactivacion))
                    {
                        return RedirectToAction("../Home/CorreccionesRequeridas", new { solicitud_id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                    return View();
                }


             
              
            }
            else
            {

                //////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                //////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                int Etapa_EnCreacion = Constants.Etapa_EnCreacion;
                int Etapa_Incompleta = Constants.Etapa_Incompleta;

                if ((tbl_sol_solicitud.Estado_id == Etapa_EnCreacion) || (tbl_sol_solicitud.Estado_id == Etapa_Incompleta))
                {

                    sqlQuery = "Exec SP_Sol_Enviar_Solicitud @Solicitud_id, @swupdatedby, @swupdatedbyinterno";

                    sqlParams = new SqlParameter[]
                        {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                        };

                    ResultFromStoreProcedure2 resultado = new ResultFromStoreProcedure2()
                    {
                        id = 0,
                        id2 = 0,
                        mensaje = "Fallo desconocido.",
                        respuesta = 0
                    };


                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure2>(sqlQuery, sqlParams).FirstOrDefault();

                    if (resultado.respuesta != 1)
                    {
                        ViewBag.MensajeError = resultado.mensaje + "<br/> Favor de presentar sus enmiendas en la oficina subregional del INAB, donde inicio su gestión.";

                        return View();
                    }

                    if (tbl_sol_solicitud.Procedencia_secorf && (resultado.id != resultado.id2))
                    {
                        long Solicitud_id2 = resultado.id2;
                        List<Tbl_Sol_DocumentoSubido> tbl_Sol_DocumentoSubidos = (from d in db.Tbl_Sol_DocumentoSubido
                                                                                  where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                                                  select d).ToList();



                        string PathArchivo;
                        string NombreArchivo;
                        string ArchivoSolicitudInicial;
                        string partialpath;
                        string UbicacionSistema = Server.MapPath("~/");
                        if (tbl_Sol_DocumentoSubidos.Count() > 0)
                        {
                            foreach (var item in tbl_Sol_DocumentoSubidos)
                            {
                                NombreArchivo = item.FileName;
                                ArchivoSolicitudInicial = "Archivos_Subidos/" + item.Solicitud_id + "/" + item.Tipo_Documento_id + "/" + item.FileName;
                                partialpath = "~/Archivos_Subidos/" + Solicitud_id2 + "/" + item.Tipo_Documento_id + "/";
                                PathArchivo = Server.MapPath(partialpath);
                                NombreArchivo = Path.Combine(PathArchivo, NombreArchivo);
                                ArchivoSolicitudInicial = Path.Combine(UbicacionSistema, ArchivoSolicitudInicial);
                                if (!Directory.Exists(Server.MapPath(partialpath)))
                                {
                                    Directory.CreateDirectory(Server.MapPath(partialpath));
                                }
                                //Copy(ArchivoDescargado, NombreArchivo, true);
                                if (System.IO.File.Exists(NombreArchivo))
                                {
                                    System.IO.File.Delete(NombreArchivo);
                                }
                                System.IO.File.Copy(ArchivoSolicitudInicial, NombreArchivo);
                            }

                        }
                    }

                    if (resultado.respuesta == 10)
                    {

                        TempData["Expediente_Secorf"] = "Se han creado dos solicitud, favor darles continuidad, Expediente SECORF fragmentado.";
                        return RedirectToAction(actionName: "Index", controllerName: "Expediente_Secorf", new { NoExpediente = tbl_sol_solicitud.Procedencia_Expediente, NoLicencia = tbl_sol_solicitud.Procedencia_Licencia, NoPOA = tbl_sol_solicitud.Procedencia_POA, NoDPI = "", SolicitanteNombre = "", LayouActivo = "No" });
                    }

                    //-------------------------- Enviar correo    ------------------------------------

                    string strSubject = "";

                    int CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

                    string EtapaSolicitud_GUIDid = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1 && Obj.Respuesta_id == 0).First().EtapaSolicitud_GUID_id;

                    Tbl_Gest_Etapa tbl_gest_Etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.EtapaRuta_id == tbl_sol_solicitud.SolicitudTipo_id && Obj.EtapaInicialRuta == true).First();

                    strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - PRIMERA REVISIÓN -";

                    if (CantidadEtapasIniciales == 2)
                    {
                        strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - SEGUNDA REVISIÓN -";
                    }

                    if (CantidadEtapasIniciales == 3)
                    {
                        strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - TERCERA REVISIÓN -";
                    }

                    string Mensaje = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Mail_SecretariaNotificacionElectronica_a_Secretaria(" + tbl_sol_solicitud.Solicitud_id.ToString() + ")").FirstOrDefault();

                    Tbl_Seg_UsuarioExterno tbl_seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_solicitud.swcreatedby);

                    string strCorreoSecretaria = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_EmailsXRoles('Secretaria'," + tbl_sol_solicitud.Region_id.ToString() + "," + tbl_sol_solicitud.SubRegion_id.ToString() + ")").FirstOrDefault();


                }

                //////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                //////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                return RedirectToAction("../Home/EnviarSolicitudProcesada");
            }
        }

        public void EnvioCorreo(string tipoDeCorreo, string Mail, string Motivo, string Mensaje, bool Publico, string EtapaSolicitud_GUID_id)
        {

            Tbl_Mail_History Tbl_mail_History = new Tbl_Mail_History();
            Tbl_mail_History.Guid_id = Guid.NewGuid().ToString();
            Tbl_mail_History.para = Mail;
            Tbl_mail_History.De = "Sistema SERNAF";
            Tbl_mail_History.TipoDeCorreo = tipoDeCorreo;
            Tbl_mail_History.CC = Motivo;
            Tbl_mail_History.Publico = Publico;
            Tbl_mail_History.Cuerpo = Mensaje;
            Tbl_mail_History.EtapaSolicitud_GUID_id = EtapaSolicitud_GUID_id;

            db.Tbl_Mail_History.Add(Tbl_mail_History);
            db.SaveChanges();



            System.Net.Mail.MailMessage Correo = new System.Net.Mail.MailMessage();
            Correo.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], "INAB Administrador");
            Correo.To.Add(new MailAddress(Mail));
            Correo.Subject = Motivo;
            AlternateView HTMLConImagenes = default(AlternateView);
            HTMLConImagenes = AlternateView.CreateAlternateViewFromString(Mensaje, null, "text/html");

            //HTMLConImagenes.LinkedResources.Add(imagen);
            Correo.AlternateViews.Add(HTMLConImagenes);
            Correo.IsBodyHtml = true;
            Correo.Priority = System.Net.Mail.MailPriority.High;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["Host"].ToString(), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Puerto"]));
            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], System.Configuration.ConfigurationManager.AppSettings["Clave"]);
            smtp.Send(Correo);
            return;
        }

        public ActionResult RegistroActualizado()
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

        public ActionResult RegistroInexistente()
        {
            return View();
        }

        public ActionResult RegistroNoActualizado()
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

        public ActionResult RegistroActivoNoGenerado()
        {

            return View();
        }

        public ActionResult RegistroInactivoNoGenerado()
        {

            return View();
        }



        public ActionResult BorrarFincasPosteriores()
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


        public ActionResult RegistroRodalNoEliminado()
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

        public ActionResult RegistroEliminado()
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

        public ActionResult RegistroFincaNoEliminado()
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


        public JsonResult EnviarSolicitud_Evaluacion(long solicitud_id, string firma)
        {
            long strSolicitud_id = solicitud_id;


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {

                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = "Acceso denegado";
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");

            }

            if (tbl_sol_solicitud.Guid_id != firma)
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = "Acceso denegado";
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Sol_Enviar_Solicitud @Solicitud_id, @swupdatedby, @swupdatedbyinterno";

            sqlParams = new SqlParameter[]
                {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = strSolicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            string strNombre = resultado[0].mensaje;
            return Json(strNombre);

        }

        public ActionResult Seguimiento(string id)
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


            TempData["Mensaje"] = "";

            if (objUs.EsInterno == 1)
            {
                return RedirectToAction("../Login/AccesoColaborador");
            }


            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

            if (tbl_Seg_UsuarioExterno.No_CasilleroElectronico == null)
            {
                return RedirectToAction("RegistroCasillero", "CasilleroElectronico");
            }


            return View();
        }

        void Uno_SetUp(long? id, string firma)
        {
            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            if ((id ?? 0) == 0)
            {
                strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            }
            else
            {
                strsection_I = "/Sol_Solicitud/visualizar?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal))
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

                    strCurrentStep = "form-total-t-" + "3";

                }
            }


        }

        void Uno_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }

                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

                    strCurrentStep = "form-total-t-" + "3";

                }
            }


        }

        void Dos_SetUp(long lngSolicitud_id, string firma)
        {
            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;

            if (lngSolicitud_id == 0)
            {
                strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            }
            else
            {
                strsection_I = "/Sol_Solicitud/visualizar?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

            }


            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }

                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal))
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";
                }

            }





        }

        void Dos_SetUpView(long lngSolicitud_id, string firma)  
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;



            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)               
                {                
                    boolsection_IV = true;               
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";
                }
            }





        }

        void Tres_SetUp(long lngSolicitud_id, string firma)
        {
            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;

            if (lngSolicitud_id == 0)
            {
                strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            }
            else
            {
                strsection_I = "/Sol_Solicitud/visualizar?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

            }


            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal))
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";

                }
            }




        }

        void Tres_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";

                }
            }




        }

        void Cuatro_SetUp(long lngSolicitud_id, string firma)
        {
            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            if (lngSolicitud_id == 0)
            {
                strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            }
            else
            {
                strsection_I = "/Sol_Solicitud/Visualizar?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";




            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal))
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";
                }
                else
                {
                    TempData["Mensaje"] = "Para continuar debe de completar al menos un datos dasometrico por cada rodal indicado";
                }
            }



        }

        void Cuatro_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";
                }
            }



        }

        void Cinco_SetUp(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            long id = lngSolicitud_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            fc_Sol_EmpresasForestales_Result PermisosEmpresasForestales = (from d in db.fc_Sol_EmpresasForestales(tbl_Sol_Solicitud.Solicitud_id)
                                                                           select d).FirstOrDefault();

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Find(tbl_Sol_Solicitud.Solicitud_id);

            bool Valido = true;
            TempData["MensajeEmpresa"] = "";

            if ((intTbl_Sol_Empresa_Entidad == 0) && (intTbl_Sol_Empresa_Entidad_Tipo_Registro == 0))
            {
                TempData["MensajeEmpresa"] += "Debe indicar el registro con el que cuenta la empresa, para continuar \n";
                Valido = false;
            }

            if (tbl_Sol_Empresa_Entidad_Tipo_Registro == null)
            {
                TempData["MensajeEmpresa"] += "Debe indicar el tipo registro con el que cuenta la empresa, para continuar \n";
                Valido = false;
            }

            if ((bool)PermisosEmpresasForestales.Actividades_Empresa)
            {
                int countActividad = db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countActividad == 0)
                {
                    TempData["MensajeEmpresa"] += "Debe indicar al menos una actividad para continuar \n";
                    Valido = false;
                }

            }

            if ((bool)PermisosEmpresasForestales.Materia_Prima)
            {
                int countMateriaPrima = db.Tbl_Sol_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countMateriaPrima == 0)
                {

                    TempData["MensajeEmpresa"] += "Debe indicar la materia prima para continuar \n";
                    Valido = false;
                }

            }

            if ((bool)PermisosEmpresasForestales.Maquinaria)
            {
                int countMaquinaria = db.Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countMaquinaria == 0)
                {
                    TempData["MensajeEmpresa"] += "Debe indicar la maquinaria para continuar \n";
                    Valido = false;
                }

            }

            if ((bool)PermisosEmpresasForestales.Vivero_Forestal)
            {
                int countVivero_Forestal = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countVivero_Forestal == 0)
                {
                    TempData["MensajeEmpresa"] += "Debe indicar las plantaciones para continuar \n";
                    Valido = false;
                }

            }

            if ((bool)PermisosEmpresasForestales.Motosierras)
            {
                int countMotosierras = db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countMotosierras == 0)
                {
                    TempData["MensajeEmpresa"] += "Debe indicar los datos de marcas y modelos de motosierras para continuar \n";
                    Valido = false;
                }

            }


            if (tbl_Sol_Empresa_Entidad != null)
            {

                if ((bool)PermisosEmpresasForestales.Capacidad_Instalada)
                {
                    if (tbl_Sol_Empresa_Entidad.CapacidadInstalada == null)
                    {
                        TempData["MensajeEmpresa"] += "Debe indicar la capacidad, presionar el boton grabar, para continuar \n";
                        Valido = false;
                    }

                }

                if ((bool)PermisosEmpresasForestales.Inscripcion_Vinculada)
                {
                    if (tbl_Sol_Empresa_Entidad.RNF_Inscripcion_Vinculada == null)
                    {
                        TempData["MensajeEmpresa"] += "Debe indicar la inscripcion vinculada y grabar para continuar \n";
                        Valido = false;
                    }

                }

            }


            if (Valido)
            {
                TempData["MensajeEmpresa"] = "";
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }


        }

        void Cinco_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Seis_SetUp(long? id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;

            if ((id ?? 0) == 0)
            {
                strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;
            }
            else
            {
                strsection_I = "/Sol_Solicitud/visualizar?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal))
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";

                }
            }




        }

        void Seis_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Finca/IndexFincaRodalDasometricos?Id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
                if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";

                }
            }




        }

        void Siete_SetUp(long lngSolicitud_id, string firma)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Sol_TecnicoProfesional_replicarDatos  @UsuarioExterno_id, @Solicitud_id";

            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@UsuarioExterno_id", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@Solicitud_id", Value = lngSolicitud_id, Direction = System.Data.ParameterDirection.Input }
            };

            db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            boolsection_II = true;
            strsection_II = "/Sol_TecnicoProfesional/EditProfesional?solicitud_id=" + lngSolicitud_id + "&firma=" + firma; ;
            strCurrentStep = "form-total-t-" + "1";

            Tbl_Sol_TecnicoProfesional tbl_Sol_TecnicoProfesional = db.Tbl_Sol_TecnicoProfesional.Find(lngSolicitud_id);

            if ((tbl_Sol_TecnicoProfesional.Grado_Academico_Profesional != false) || (tbl_Sol_TecnicoProfesional.Grado_Academico_Tecnico != false))
            {
                strCurrentStep = "form-total-t-" + "2";
                boolsection_III = true;
                strsection_III = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
            }


        }

        void Siete_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_TecnicoProfesional/EditProfesional?solicitud_id=" + lngSolicitud_id + "&firma=" + firma; ;
            strCurrentStep = "form-total-t-" + "1";

            boolsection_III = true;
            strsection_III = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

            strCurrentStep = "form-total-t-" + "1";

        }

        void Ocho_SetUp(long lngSolicitud_id, string firma)
        {
            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {

                    boolsection_III = true;
                    strsection_III = "/Sol_Motosierra/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Motosierra/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Motosierra > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Ocho_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {

                    boolsection_III = true;
                    strsection_III = "/Sol_Motosierra/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;

                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Motosierra/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Motosierra > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Nueve_SetUp(long lngSolicitud_id, string firma)
        {
            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Find(lngSolicitud_id);

            bool Valido = true;
            TempData["MensajeEmpresa"] = "";

            if ((intTbl_Sol_Empresa_Entidad == 0) && (intTbl_Sol_Empresa_Entidad_Tipo_Registro == 0))
            {
                TempData["MensajeEmpresa"] += "Debe indicar el registro con el que cuenta la empresa, para continuar \n";
                Valido = false;
            }

            if (tbl_Sol_Empresa_Entidad_Tipo_Registro == null)
            {
                TempData["MensajeEmpresa"] += "Debe indicar el tipo registro con el que cuenta la empresa, para continuar \n";
                Valido = false;
            }

            if (Valido)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Nueve_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Once_SetUp(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/Create?id=" + lngSolicitud_id + "&firma=" + firma;

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            long id = lngSolicitud_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            fc_Sol_EmpresasForestales_Result PermisosEmpresasForestales = (from d in db.fc_Sol_EmpresasForestales(tbl_Sol_Solicitud.Solicitud_id)
                                                                           select d).FirstOrDefault();

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Find(tbl_Sol_Solicitud.Solicitud_id);

            bool Valido = true;
            TempData["MensajeEmpresa"] = "";

            //if ((intTbl_Sol_Empresa_Entidad == 0) && (intTbl_Sol_Empresa_Entidad_Tipo_Registro == 0))
            //{
            //    TempData["MensajeEmpresa"] += "Debe indicar el registro con el que cuenta la empresa, para continuar \n";
            //    Valido = false;
            //}

            //if (tbl_Sol_Empresa_Entidad_Tipo_Registro == null)
            //{
            //    TempData["MensajeEmpresa"] += "Debe indicar el tipo registro con el que cuenta la empresa, para continuar \n";
            //    Valido = false;
            //}

            //if ((bool)PermisosEmpresasForestales.Actividades_Empresa)
            //{
            //    int countActividad = db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            //    if (countActividad == 0)
            //    {
            //        TempData["MensajeEmpresa"] += "Debe indicar al menos una actividad para continuar \n";
            //        Valido = false;
            //    }

            //}

            //if ((bool)PermisosEmpresasForestales.Materia_Prima)
            //{
            //    int countMateriaPrima = db.Tbl_Sol_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            //    if (countMateriaPrima == 0)
            //    {

            //        TempData["MensajeEmpresa"] += "Debe indicar la materia prima para continuar \n";
            //        Valido = false;
            //    }

            //}

            //if ((bool)PermisosEmpresasForestales.Maquinaria)
            //{
            //    int countMaquinaria = db.Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            //    if (countMaquinaria == 0)
            //    {
            //        TempData["MensajeEmpresa"] += "Debe indicar la maquinaria para continuar \n";
            //        Valido = false;
            //    }

            //}

            if ((bool)PermisosEmpresasForestales.Vivero_Forestal)
            {
                int countVivero_Forestal = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

                if (countVivero_Forestal == 0)
                {
                    TempData["MensajeEmpresa"] += "Debe indicar las plantaciones para continuar \n";
                    Valido = false;
                }

            }

            //if ((bool)PermisosEmpresasForestales.Motosierras)
            //{
            //    int countMotosierras = db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Count();

            //    if (countMotosierras == 0)
            //    {
            //        TempData["MensajeEmpresa"] += "Debe indicar los datos de marcas y modelos de motosierras para continuar \n";
            //        Valido = false;
            //    }

            //}


            if (tbl_Sol_Empresa_Entidad != null)
            {

                if ((bool)PermisosEmpresasForestales.Capacidad_Instalada)
                {
                    if (tbl_Sol_Empresa_Entidad.CapacidadInstalada == null)
                    {
                        TempData["MensajeEmpresa"] += "Debe indicar la capacidad, presionar el boton grabar, para continuar \n";
                        Valido = false;
                    }

                }

                //if ((bool)PermisosEmpresasForestales.Inscripcion_Vinculada)
                //{
                //    if (tbl_Sol_Empresa_Entidad.RNF_Inscripcion_Vinculada == null)
                //    {
                //        TempData["MensajeEmpresa"] += "Debe indicar la inscripcion vinculada y grabar para continuar \n";
                //        Valido = false;
                //    }

                //}

            }


            if (Valido)
            {
                TempData["MensajeEmpresa"] = "";
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }


        }

        void Once_SetUpView(long lngSolicitud_id, string firma)
        {

            strsection_I = "/Sol_Solicitud/CreateConRegion?id=" + lngSolicitud_id + "&firma=" + firma;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/Sol_Solicitud/IndexPropietarioRepresentante?Id=" + lngSolicitud_id + "&firma=" + firma;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/Sol_Empresa_Entidad/Create?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "3";
            }

        }




        public ActionResult NoPuedeAgregarRodal()
        {
            return View();

        }


        public ActionResult SolicitudEtapaView(string guid_id, int etapaid, decimal etaparutaid, int correlativo)
        {


            int intContador = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guid_id).Count();

            if (intContador == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            long solicitudid = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guid_id).First().Solicitud_id;
            long id = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guid_id).First().Solicitud_id;
            ViewBag.solicitudid = id;
            Session[Constants.session_Solicitud] = id;


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

            //	0	-- Seleccione una categoría ---
            //	1	Bosques naturales
            //	2	Plantaciones forestales
            //	3	Plantaciones de arboles frutales
            //	4	Sistemas agroforestales
            //	5	Empresas forestales
            //	6	Fuentes semilleras y material vegetativo
            //	7	Profesionales del área forestal
            //	8	Motosierras
            //	9	Entidades relacionadas con investigación, extensión y capacitación

            ViewBag.lngSolicitud = solicitudid;

            ViewBag.section_0 = true;
            ViewBag.section_I = false;
            ViewBag.section_II = false;
            ViewBag.section_III = false;
            ViewBag.section_IV = false;
            ViewBag.section_V = false;
            ViewBag.section_VI = false;

            boolsection_0 = true;
            boolsection_I = false;
            boolsection_II = false;
            boolsection_III = false;
            boolsection_IV = false;
            boolsection_V = false;
            boolsection_VI = false;

            strsection_I = "";
            strsection_II = "";
            strsection_III = "";
            strsection_IV = "";
            strsection_V = "";
            strsection_VI = "";

            /// Si no es el propietario no deja trabajar la solicitud.
            /// 
            //try
            //{
            //    Tbl_Gest_EtapaSolicitud tbl_gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj=> Obj.Solicitud_id == solicitudid && Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.CorrelativoEtapa_id == correlativo).First();

            //}
            //catch (Exception ex)
            //{
            //    return RedirectToAction("../Home/AccesoDenegado");
            //}


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);



            EdicionSolicitudGrants EdicionSolicitudGrant = new EdicionSolicitudGrants();

            if (tbl_sol_solicitud.Estado_id == 0)
            {
                EdicionSolicitudGrant.boolEditarSolicitud = true;
                EdicionSolicitudGrant.boolEditarDasometrico = true;
                EdicionSolicitudGrant.boolEditarFinca = true;
                EdicionSolicitudGrant.boolEditarMotosierra = true;
                EdicionSolicitudGrant.boolEditarEntidad = true;
                EdicionSolicitudGrant.boolEditarRodal = true;
                EdicionSolicitudGrant.boolEditarDasometrico = true;
                EdicionSolicitudGrant.boolEditarPropietario = true;
                EdicionSolicitudGrant.boolEditarRepresentante = true;
                EdicionSolicitudGrant.boolSubirDocumentos = true;

                if ((tbl_sol_solicitud.Region_id ?? 0) != 0)
                {
                    
                    EdicionSolicitudGrant.boolEditarSolicitud = false;
                }

            }
            else
            {
                EdicionSolicitudGrant.boolEditarSolicitud = false;
                EdicionSolicitudGrant.boolEditarDasometrico = false;
                EdicionSolicitudGrant.boolEditarFinca = false;
                EdicionSolicitudGrant.boolEditarMotosierra = false;
                EdicionSolicitudGrant.boolEditarEntidad = false;
                EdicionSolicitudGrant.boolEditarRodal = false;
                EdicionSolicitudGrant.boolEditarDasometrico = false;
                EdicionSolicitudGrant.boolEditarPropietario = false;
                EdicionSolicitudGrant.boolEditarRepresentante = false;
                EdicionSolicitudGrant.boolSubirDocumentos = false;
            }

            EdicionSolicitudGrant.boolEtapaView = true;
            Session[Constants.session_EdicionSolicitudGrants] = EdicionSolicitudGrant;


            ///*************************************************************************************///
            // Si se detecta que ya han subido documentos se puede enviar a analisis la solicitud   ///
            int intDocumentosSubidos = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == id).Count();

            //Acá se define si es procedencia externa para que permita visualizar todos los campos registrados de parte de la migración
            boolProcedencia_Probosque = tbl_sol_solicitud.Procedencia_Probosque;
            boolProcedencia_PinpepOld = tbl_sol_solicitud.Procedencia_PinpepOld;
            boolProcedencia_PinpepNew = tbl_sol_solicitud.Procedencia_PinpepNew;
            boolProcedencia_Secorf = tbl_sol_solicitud.Procedencia_secorf;

             if (boolProcedencia_Probosque || boolProcedencia_PinpepOld || boolProcedencia_PinpepNew || boolProcedencia_Secorf)
            {
                boolProcedencia_Externa = true;
            }

            intTbl_PersoneriaIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == id).Count();
            intTbl_PersoneriaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == id).Count();
            intTbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id).Count();
            intTbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == id).Count();
            intTbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == id).Count();

            intTbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == id).Count();
            intTbl_Sol_Rodal_Dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == id).Count();




            intTbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).Count();
            intRegion_id = tbl_sol_solicitud.Region_id ?? 0;

            if ((intTbl_Sol_Finca > 0) || (intTbl_Sol_Empresa_Entidad > 0) || (intTbl_Sol_Motosierra > 0))
            {
                Session[Constants.session_SolicitudLista] = 1;
                ViewBag.EnviaraEvaluacion = 1;
            }
            else
            {
                Session[Constants.session_SolicitudLista] = 0;
                ViewBag.EnviaraEvaluacion = 0;
            }

            ///***********************************************************************************


            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_solicitud.swcreatedby);

            strsection_0 = "/UsuarioExterno/UsuarioExView?id=" + tbl_sol_solicitud.swcreatedby + "&email=" + tbl_Seg_UsuarioExterno.Correo;

            strsection_I = "/Sol_Solicitud/Create?id=" + id.ToString() + "&firma=" + tbl_sol_solicitud.Guid_id;


            strCurrentStep = "form-total-t-" + "0";
            boolsection_I = true;

            if (tbl_sol_solicitud.Categoria_id == 1)
                Uno_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 2)
                Dos_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 3)
                Tres_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 4)
                Cuatro_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 5)
                Cinco_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 6)
                Seis_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 7)
                Siete_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            //if (tbl_sol_solicitud.Categoria_id == 8)
            //    Ocho_SetUpView(id);

            if ((tbl_sol_solicitud.Categoria_id == 8) && (tbl_sol_solicitud.Sub_Categoria_id == 1))
                Ocho_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if ((tbl_sol_solicitud.Categoria_id == 8) && (tbl_sol_solicitud.Sub_Categoria_id == 2))
                Cinco_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

            if (tbl_sol_solicitud.Categoria_id == 9)
                Nueve_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);


            if (tbl_sol_solicitud.Categoria_id == 11)
                Once_SetUpView(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);



            ViewBag.lngSolicitud_id = tbl_sol_solicitud.Solicitud_id;
            ViewBag.CurrentStep = strCurrentStep;

            ViewBag.section_I = boolsection_I;
            ViewBag.section_II = boolsection_II;
            ViewBag.section_III = boolsection_III;
            ViewBag.section_IV = boolsection_IV;
            ViewBag.section_V = boolsection_V;
            ViewBag.section_VI = boolsection_VI;

            ViewBag.section_Link_0 = strsection_0;
            ViewBag.section_Link_I = strsection_I;
            ViewBag.section_Link_II = strsection_II;
            ViewBag.section_Link_III = strsection_III;
            ViewBag.section_Link_IV = strsection_IV;
            ViewBag.section_Link_V = strsection_V;
            ViewBag.section_Link_VI = strsection_VI;

            return View(tbl_sol_solicitud);
        }




        public ActionResult test()
        {
            return RedirectToAction("../Home/AccesoDenegado");
            //return View();
        }

        public ActionResult SolicitudInsertUpdate(long? id, string firma)
        {
            lngSolicitud_id = (long)id;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            ViewBag.lngSolicitud_id = lngSolicitud_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = new Tbl_Sol_Solicitud();

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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

            //	0	-- Seleccione una categoría ---
            //	1	Bosques naturales
            //	2	Plantaciones forestales
            //	3	Plantaciones de arboles frutales
            //	4	Sistemas agroforestales
            //	5	Empresas forestales
            //	6	Fuentes semilleras y material vegetativo
            //	7	Profesionales del área forestal
            //	8	Motosierras
            //	9	Entidades relacionadas con investigación, extensión y capacitación

            ViewBag.lngSolicitud = id;

            ViewBag.section_I = false;
            ViewBag.section_II = false;
            ViewBag.section_III = false;
            ViewBag.section_IV = false;
            ViewBag.section_V = false;
            ViewBag.section_VI = false;

            boolsection_I = false;
            boolsection_II = false;
            boolsection_III = false;
            boolsection_IV = false;
            boolsection_V = false;
            boolsection_VI = false;

            strsection_I = "";
            strsection_II = "";
            strsection_III = "";
            strsection_IV = "";
            strsection_V = "";
            strsection_VI = "";

            if ((id == null) || (id == 0))
            {

                tbl_sol_solicitud.Solicitud_id = 0;
                tbl_sol_solicitud.Guid_id = null;

                EdicionSolicitudGrants EdicionSolicitudGrant = new EdicionSolicitudGrants();

                EdicionSolicitudGrant.boolEditarSolicitud = true;
                EdicionSolicitudGrant.boolEditarDasometrico = true;
                EdicionSolicitudGrant.boolEditarFinca = true;
                EdicionSolicitudGrant.boolEditarMotosierra = true;
                EdicionSolicitudGrant.boolEditarEntidad = true;
                EdicionSolicitudGrant.boolEditarRodal = true;
                EdicionSolicitudGrant.boolEditarDasometrico = true;
                EdicionSolicitudGrant.boolEditarPropietario = true;
                EdicionSolicitudGrant.boolEditarRepresentante = true;
                EdicionSolicitudGrant.boolSubirDocumentos = true;

                strsection_I = "/Sol_Solicitud/Create?id=" + 0;
                boolsection_I = true;

                Session[Constants.session_EdicionSolicitudGrants] = EdicionSolicitudGrant;
                Session[Constants.session_Solicitud] = null;

            }
            else
            {
                /// Si no es el propietario no deja trabajar la solicitud.

                tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

                EdicionSolicitudGrants EdicionSolicitudGrant = new EdicionSolicitudGrants();


                if (tbl_sol_solicitud == null)
                {
                    return RedirectToAction("../Home/SolicitudNoCreada");

                }
                else
                {
                    if (firma != tbl_sol_solicitud.Guid_id)
                    {
                        return RedirectToAction("../Home/AccesoDenegado");
                    }
                }

                if (tbl_sol_solicitud.Estado_id == 0)
                {
                    EdicionSolicitudGrant.boolEditarSolicitud = true;
                    EdicionSolicitudGrant.boolEditarDasometrico = true;
                    EdicionSolicitudGrant.boolEditarFinca = true;
                    EdicionSolicitudGrant.boolEditarMotosierra = true;
                    EdicionSolicitudGrant.boolEditarEntidad = true;
                    EdicionSolicitudGrant.boolEditarRodal = true;
                    EdicionSolicitudGrant.boolEditarDasometrico = true;
                    EdicionSolicitudGrant.boolEditarPropietario = true;
                    EdicionSolicitudGrant.boolEditarRepresentante = true;
                    EdicionSolicitudGrant.boolSubirDocumentos = true;

                    if ((tbl_sol_solicitud.Region_id ?? 0) != 0)
                    {
                        EdicionSolicitudGrant.boolEditarSolicitud = false;
                    }

                }
                else
                {
                    EdicionSolicitudGrant.boolEditarSolicitud = false;
                    EdicionSolicitudGrant.boolEditarDasometrico = false;
                    EdicionSolicitudGrant.boolEditarFinca = false;
                    EdicionSolicitudGrant.boolEditarMotosierra = false;
                    EdicionSolicitudGrant.boolEditarEntidad = false;
                    EdicionSolicitudGrant.boolEditarRodal = false;
                    EdicionSolicitudGrant.boolEditarDasometrico = false;
                    EdicionSolicitudGrant.boolEditarPropietario = false;
                    EdicionSolicitudGrant.boolEditarRepresentante = false;
                    EdicionSolicitudGrant.boolSubirDocumentos = false;
                }
                Session[Constants.session_EdicionSolicitudGrants] = EdicionSolicitudGrant;
                EdicionSolicitudGrant.boolEtapaView = false;

                try
                {
                    if ((objUs.EsInterno == 0) && (tbl_sol_solicitud.swcreatedby != objUs.intUsuario_id) && (tbl_sol_solicitud.DPI_Titular != tbl_Seg_UsuarioExterno.No_Documento))
                    {
                        return RedirectToAction("../Home/AccesoDenegado");

                    }
                    else
                    {
                        Session[Constants.session_Solicitud] = id;
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("../Home/AccesoDenegado");
                }






                ///*************************************************************************************///
                // Si se detecta que ya han subido documentos se puede enviar a analisis la solicitud   ///
                int intDocumentosSubidos = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == id).Count();

                intTbl_PersoneriaIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == true).Count();
                intTbl_PersoneriaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == true).Count();
                intTbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == 1).Count();
                intTbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == id).Count();
                intTbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == id).Count();

                intTbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == id).Count();
                intTbl_Sol_Rodal_Dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == id).Count();

                intTbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).Count();
                intTbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).Count();
                intRegion_id = tbl_sol_solicitud.Region_id ?? 0;

                if ((intTbl_Sol_Finca > 0) || (intTbl_Sol_Empresa_Entidad > 0) || (intTbl_Sol_Motosierra > 0))
                {
                    Session[Constants.session_SolicitudLista] = 1;
                    ViewBag.EnviaraEvaluacion = 1;
                }
                else
                {
                    Session[Constants.session_SolicitudLista] = 0;
                    ViewBag.EnviaraEvaluacion = 0;
                }

                ///***********************************************************************************

                strsection_I = "/Sol_Solicitud/Create?id=" + id.ToString() + "&firma=" + firma;
                strCurrentStep = "form-total-t-" + "0";
                boolsection_I = true;

                if (tbl_sol_solicitud.Categoria_id == 1)
                    Uno_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 2)
                    Dos_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 3)
                    Tres_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 4)
                    Cuatro_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 5)
                    Cinco_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 6)
                    Seis_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 7)
                    Siete_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if ((tbl_sol_solicitud.Categoria_id == 8) && (tbl_sol_solicitud.Sub_Categoria_id == 1))
                    Ocho_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if ((tbl_sol_solicitud.Categoria_id == 8) && (tbl_sol_solicitud.Sub_Categoria_id == 2))
                    Cinco_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 9)
                    Nueve_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);

                if (tbl_sol_solicitud.Categoria_id == 11)
                    Once_SetUp(tbl_sol_solicitud.Solicitud_id, tbl_sol_solicitud.Guid_id);


                //Cancelacion
                decimal TipoGestion = tbl_sol_solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_solicitud.SolicitudTipo_id);
                decimal terminacioninactivacion = 0.06M;

                if (TipoGestion == terminacioninactivacion)
                {
                    boolsection_IV = true;
                    strsection_IV = "/UploadFiles/UploadedFiles?solicitud_id=" + lngSolicitud_id + "&firma=" + firma;
                    strCurrentStep = "form-total-t-" + "3";
                }

            }


            ViewBag.CurrentStep = strCurrentStep;

            ViewBag.section_I = boolsection_I;
            ViewBag.section_II = boolsection_II;
            ViewBag.section_III = boolsection_III;
            ViewBag.section_IV = boolsection_IV;
            ViewBag.section_V = boolsection_V;
            ViewBag.section_VI = boolsection_VI;

            ViewBag.section_Link_I = strsection_I;
            ViewBag.section_Link_II = strsection_II;
            ViewBag.section_Link_III = strsection_III;
            ViewBag.section_Link_IV = strsection_IV;
            ViewBag.section_Link_V = strsection_V;
            ViewBag.section_Link_VI = strsection_VI;

            return View(tbl_sol_solicitud);
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }

        public ActionResult SolicitudNoCreada()
        {
            return View();
        }


        public JsonResult ExportarExcel(long id, string firma)
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
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Boolean ErrorEncontrado = false;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                ErrorEncontrado = true;
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                ErrorEncontrado = true;
            }

            string strNombre = "";

            if (ErrorEncontrado == false)
            {
                string strDir = "Documentos\\";
                string strFolder = Server.MapPath("~/") + strDir;
                DateTime hoy = DateTime.Now;
                string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
                strNombre = @"LibroFiscalizacionRegionVI" + fecha + ".xlsx";
                string strDirArchivo = strFolder + strNombre;

                if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);

                FileInfo newFile = new FileInfo(strDirArchivo);
                if (newFile.Exists)
                {
                    newFile.Delete();
                    newFile = new FileInfo(strDirArchivo);
                }

                ExcelPackage Mi_Excel = new ExcelPackage(newFile);
                System.Drawing.Image image = System.Drawing.Image.FromFile(Server.MapPath("~/Content/images/logoInabExcel.jpg"));


                //    #region MADERA MEDIDA EN TRANSPORTE O PATIO
                ExcelWorksheet HojaExcel_Solicitud = Mi_Excel.Workbook.Worksheets.Add("Solicitud");
                var excelImage = HojaExcel_Solicitud.Drawings.AddPicture("My Logo", image);
                excelImage.SetPosition(0, 0, 0, 0);

                HojaExcel_Solicitud.Cells["A1:J1"].Merge = true;
                HojaExcel_Solicitud.Cells["A1:J1"].Value = Constants.Entidad;
                HojaExcel_Solicitud.Cells["A1:J1"].Style.Font.Name = "Calibri";
                HojaExcel_Solicitud.Cells["A1:J1"].Style.Font.Bold = true;
                HojaExcel_Solicitud.Cells["A1:J1"].Style.Font.Size = 18;
                HojaExcel_Solicitud.Cells["A1:J1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                HojaExcel_Solicitud.Cells["C2:I2"].Merge = true;
                HojaExcel_Solicitud.Cells["C2:I2"].Value = "SOLICITUD";
                HojaExcel_Solicitud.Cells["C2:I2"].Style.Font.Name = "Calibri";
                HojaExcel_Solicitud.Cells["C2:I2"].Style.Font.Bold = true;
                HojaExcel_Solicitud.Cells["C2:I2"].Style.Font.Size = 12;
                HojaExcel_Solicitud.Cells["C2:I2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                HojaExcel_Solicitud.Cells["A3:J3"].Merge = true;
                HojaExcel_Solicitud.Cells["A3:J3"].Value = Constants.Entidad;
                HojaExcel_Solicitud.Cells["A3:J3"].Style.Font.Name = "Calibri";
                HojaExcel_Solicitud.Cells["A3:J3"].Style.Font.Bold = true;
                HojaExcel_Solicitud.Cells["A2:J2"].Style.Font.Size = 18;
                HojaExcel_Solicitud.Cells["A3:J3"].Style.Font.Italic = true;
                HojaExcel_Solicitud.Cells["A3:J3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                HojaExcel_Solicitud.Cells["A5:J5"].Merge = true;
                HojaExcel_Solicitud.Cells["A5:J5"].Value = "SOLICITANTE";
                HojaExcel_Solicitud.Cells["A5:J5"].Style.Font.Name = "Calibri";
                HojaExcel_Solicitud.Cells["A5:J5"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightTrellis;
                HojaExcel_Solicitud.Cells["A5:J5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSeaGreen); HojaExcel_Solicitud.Cells["A5:J5"].Style.Font.Bold = true;
                HojaExcel_Solicitud.Cells["A5:J5"].Style.Font.Size = 12;
                HojaExcel_Solicitud.Cells["A5:J5"].Style.Font.Italic = true;
                HojaExcel_Solicitud.Cells["A5:J5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                Mi_Excel.Workbook.Properties.Title = "ExcelInforme";
                Mi_Excel.Workbook.Properties.Author = "INTECNOVA";

                Mi_Excel.Save();
            }
            else
            { strNombre = ""; }

            return Json(strNombre);

        }


        public JsonResult AgregarmeComoPropietarioRepresentante(int tipo, long solicitud_id, string firma)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            Boolean error = false;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                error = true;
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                error = true;
            }


            string strRespuesta;

            if (error == false)
            {
                strRespuesta = "";

                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec Sp_Sol_InsMe_PropietarioRepresentate  @Tipo_id, @Solicitud_id, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                {
                new SqlParameter { ParameterName = "@Tipo_id",  Value = tipo, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = solicitud_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedby", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedbyinterno", Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };


                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                strRespuesta = resultado[0].respuesta.ToString();
            }
            else
            {
                strRespuesta = "Acceso denegado";
            }
            return Json(strRespuesta);
        }

        public JsonResult AgregarmeComoArrendatarioRepresentante(int tipo, long solicitud_id, string firma)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }
            bool ErrorEncontrado = false;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                ErrorEncontrado = true;
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                ErrorEncontrado = true;
            }

            string strRespuesta;

            if (ErrorEncontrado == false)
            {

                strRespuesta = "";

                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec Sp_Sol_InsMe_ArrendatarioRepresentate  @Tipo_id, @Solicitud_id, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                {
                new SqlParameter { ParameterName = "@Tipo_id",  Value = tipo, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = solicitud_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedby", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedbyinterno", Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };


                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                strRespuesta = resultado[0].respuesta.ToString();
            }
            else
            {
                strRespuesta = "Error encontrado, acceso denegado";
            }
            return Json(strRespuesta);
        }


        public ActionResult EdicionPropietarioDenegada()
        {
            return View();
        }



        public string GetBearer()
        {
            try
            {
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                var client = new RestClient(Constants.Address_Bearer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string body = @"{
                        " + "\n" +
                            @"""Username"":""RNFUser"",
                        " + "\n" +
                            @"""Password"":""RNF_Password""
                        " + "\n" +
                            @"}
                        " + "\n" +
                            @"";

                body = body.Replace("RNFUser", Constants.RNF_Username);
                body = body.Replace("RNF_Password", Constants.RNF_Password);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //ServicePointManager.ServerCertificateValidationCallback = null;

                if (response.Content.ToString().Replace("\"", "").Length < 250)
                {
                    return "Error en credenciales RNF User  y RNF Password en la solicitud de bearer";
                }

                if (response.Content.ToString().Replace("\"", "").Length > 270)
                {
                    return "Error: " + response.Content.ToString().Replace("\"", "").Substring(1, 100);
                }
                else
                {
                    return "Bearer " + response.Content.ToString().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message.Substring(1, 100);
            }
        }


        class RNFCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public string CallCORS(string strSign, string strPath)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveUpload);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", strSign);
                //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
                request.AddFile("nombre", strPath, "application/pdf");
                request.AddParameter("parentGoogleDriveId", Constants.strParentGoogleDrive);
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                return joResponse["GoogleDriveId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error: No se logró subir el archivo a Google Drive, reporte a informatica. " + ex.Message.ToString();
            }
        }

        //public string firmarFile(string bearer, string strUsuarioFirma, string strUsuarioPassword, string strDocumento)
        //{


        //    try
        //    {

        //        //45,75,190,120
        //        //30,30,250,150
        //        //400,30,570,150
        //        var client = new RestClient(Constants.Address_FirmaElectronica);
        //        client.Timeout = -1;
        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("Authorization", bearer);
        //        request.AddHeader("Content-Type", "application/json");
        //        string body = @"{
        //        " + "\n" +
        //                        @"  ""User"": ""strUsuarioFirma"",
        //        " + "\n" +
        //                        @"  ""Password"": ""strUsuarioPassword"",
        //        " + "\n" +
        //                        @"  ""parentGoogleDriveId"":""strparentGoogleDriveId"",
        //        " + "\n" +
        //                        @"  ""Documentos"":[
        //        " + "\n" +
        //                        @"    {
        //        " + "\n" +
        //                        @"      ""GoogleDriveId"": ""strDocumento"",
        //        " + "\n" +
        //                        @"      ""Coordenadas"": ""70,650,180,710"",
        //        " + "\n" +
        //                        @"      ""NumeroPagina"": 1
        //        " + "\n" +
        //                        @"    }
        //        " + "\n" +
        //                        @" ]
        //        " + "\n" +
        //        @"}";

        //        //@"      ""Coordenadas"": ""350,730,570,830"",
        //        body = body.Replace("strUsuarioFirma", strUsuarioFirma).Replace("strUsuarioPassword", strUsuarioPassword).Replace("strparentGoogleDriveId", Constants.parentGoogleDriveId);
        //        body = body.Replace("strDocumento", strDocumento);

        //        request.AddParameter("application/json", body, ParameterType.RequestBody);
        //        IRestResponse response = client.Execute(request);

        //        if (response.Content.ToString().IndexOf("Error") > 0)
        //        {
        //            return response.Content.ToString() + "  Usuario ó Password erroneo en firma.";
        //        }

        //        if (response.Content.ToString().IndexOf("connection") > 0)
        //        {
        //            return response.Content.ToString() + "  No hay conexion con el servidor de firmas." + response.Content.ToString();
        //        }


        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        try
        //        {
        //            Rootobject myDeserializedClass = JsonConvert.DeserializeObject<Rootobject>(response.Content.ToString());
        //            return myDeserializedClass.Data[0].GoogleDriveIdNuevo;
        //        }
        //        catch (Exception ex)
        //        {
        //            return strDocumento;
        //        }


        //        // Descomentar return myDeserializedClass.Data[0].GoogleDriveIdNuevo;

        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica

        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error al intentar firmar el archivo." + ex.Message.ToString();
        //    }


        //}

        public bool getFile(string strBearer, string strFile, string path)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveDownLoad + strFile);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", strBearer);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                byte[] buffer = client.DownloadData(request);

                MemoryStream ms = new MemoryStream(buffer);
                FileStream file = new FileStream(@path + strFile + ".pdf", FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }

            // IRestResponse response = client.Execute(request);

        }

        public JsonResult JsonProcesarFirmaElectronica(string Guid_id, string Guidetapa_id, string UsuarioFE, string PasswordFE)
        {
            // Bitacora activa.
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;


            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            DateTime hoy = DateTime.Now;

            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            string rootpdf = rootpath + "U" + Guidetapa_id + fecha + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

            rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;
            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);

            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 0, "A.- Inicia proceso de firma electronica Home-JsonProcesarFirmaElectronica");

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

            try
            {

                
                Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = null;

                if ((Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado != null) && (Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado.ToString() != ""))
                {
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 0, "A.- El documento ya ha sido firmado antes Home-JsonProcesarFirmaElectronica ");

                    Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado;
                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "Este documento ya ha sido firmado. No puede firmarlo nuevamente." + "\"}";

                    return Json(jsonResultUsr);

                }

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 2, "B.- Se busca obtener el bearer");

                strBearer = GetBearer();

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 3, "C.- Bearer obtenido");

                if (strBearer.Length < 125)
                {

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 4, "D.- Bearer erroneo, menor a 125 caracteres");


                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                    return Json(jsonResultUsr);

                }

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
                {

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 5, "N.- Error al subir el documento.");

                    intRespuesta = 0;

                    strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                    return Json(jsonResultUsr);


                }

                //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

                RequestUtil requestUtil = new RequestUtil();

                string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);


                if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
                {


                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 6, "R.- Error al firmar el documento, Usuario o Password Erroneos.");

                    intRespuesta = 0;

                    strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                    return Json(jsonResultUsr);


                }

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 7, "W.- Intentando obtener archivo firmado.");

                if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
                {

                    string SP_SqlQuery = "EXEC [dbo].[SP_GestEtapaSolicitud_ActualizaDocumentoFirmado] @Solicitud_id, @Firmante, @GUID_id, @EtapaSolicitud_GUID_id, @NombreDocumentoFirmado";
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@Solicitud_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Firmante", Value = strEnc, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@GUID_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_Guid_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@EtapaSolicitud_GUID_id", Value = Tbl_Gest_etapaSolicitud.EtapaSolicitud_GUID_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@NombreDocumentoFirmado", Value = strDocumentofirmado + ".pdf", Direction = System.Data.ParameterDirection.Input },
                    };

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();

                    //Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                    //db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                    //db.SaveChanges();

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 8, "X.- El archivo se obtuvo con exito.");

                }
                else
                {
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 9, "X.- No se logró obtener el archivo firmado.");

                }

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 10, "Z.- FE generada con éxito.");

                intRespuesta = resultFromStoreProcedure.respuesta;

                if (intRespuesta == 1)
                {

                    string ip = Request.UserHostAddress;
                    if (ip == "::1")
                    {
                        ip = "127.0.0.1";
                    }


                    Usuario objUs = new Usuario();
                    objUs.intUsuario_id = 0;
                    objUs = (Usuario)Session["User"];


                    //************  BITACORA AUDITORIA *******************

                    string SP_SqlQuery = "EXEC [dbo].[Sp_BitacoraRegresoOficioNoProcedente] @Solicitud_id, @UsrIng, @GUID_id, @IP";
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@Solicitud_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@UsrIng", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@GUID_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_Guid_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@IP", Value = ip, Direction = System.Data.ParameterDirection.Input },
                        
                    };

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();

                    //***************************************************


                    Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

                }
                else
                {
                    jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + 0 + "\","
                                      + "\"strRespuesta\":" + "\"" + resultFromStoreProcedure.mensaje + "\"}";

                }

                return Json(jsonResultUsr);

            }
            catch (Exception ex)
            {

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 10, "Z.- No se pudo generar la FE" + ex.Message.ToString() + "..." + ex.InnerException.ToString());


                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + $"No se logró realizar la firma electrónica. " + ex.Message.ToString() + "\"}";

                return Json(jsonResultUsr);

            }
        }

        public class RespuestaJSON_Bearer
        {
            public int CodRespuesta { get; set; }
            public string strRespuesta { get; set; }
        }

        public JsonResult GetBearerCasillero(string usuario, string password)
        {
            int intRespuesta = 0;
            string strRespuesta = "";

            RespuestaJSON_Bearer respuestaJSON_Bearer = new RespuestaJSON_Bearer();

            Tbl_Gral_ParametrosGenerales tbl_Gral_ParametrosGenerales = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault();

            if ((usuario != Constants.CasilleroElectronicoUser_Out) || (password != Constants.CasilleroElectronicoPassword_Out))
            {
                respuestaJSON_Bearer = new RespuestaJSON_Bearer
                {
                    CodRespuesta = intRespuesta,
                    strRespuesta = "No se logró obtener el bearer. Usuario y Password invalidos",
                };
            }

            try
            {
                strRespuesta = GenerarToken(usuario);
                intRespuesta = 1;
            }
            catch (Exception ex)
            {
                intRespuesta = 0;
                strRespuesta = "Error:" + ex.Message.Substring(1, 100);
            }

            respuestaJSON_Bearer = new RespuestaJSON_Bearer
            {
                CodRespuesta = intRespuesta,
                strRespuesta = strRespuesta,
            };

            return Json(respuestaJSON_Bearer);
        }

        public async Task<FileStreamResult> MiMetodo(string Codigo)
        {
            // Obtener token de la cabecera Authorization
            var tokenString = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Deserializar token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Obtener datos del token
            var sub = token.Claims.First(claim => claim.Type == "sub").Value;
            var jti = token.Claims.First(claim => claim.Type == "jti").Value;
            var exp = token.Claims.First(claim => claim.Type == "exp").Value;

            DateTime exp_dt = ConversorFechas.FromUnixTime(long.Parse(exp.ToString()));
            DateTime swdatenow = DateTime.Now;

            if (exp_dt > swdatenow)
            {
                var filePath = Path.Combine(Server.MapPath("~/"), "Archivos_ConFirmaElectronica", "1AtkIci1JLJy4pddv7_M21r1wrwXOeOTp.pdf");

                // Leer archivo PDF en segundo plano
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await fileStream.CopyToAsync(memoryStream);
                            var pdfBytes = memoryStream.ToArray();
                            var base64String = Convert.ToBase64String(pdfBytes);
                            var response = new { pdf = base64String };

                            // Crear nuevo FileStream a partir de MemoryStream
                            var newFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                            var fileStreamResult = new FileStreamResult(newFileStream, "application/pdf");
                            return fileStreamResult;
                        }
                    }
                    catch (Exception e)
                    {
                        // Manejar la excepción como se desee
                        Console.WriteLine(e.Message);
                        return new FileStreamResult(new MemoryStream(), "application/pdf");
                    }
                }
            }
            else
            {
                // Devolver un archivo PDF vacío en caso de que el token haya expirado
                return new FileStreamResult(new MemoryStream(), "application/pdf");
            }
        }

        private string GenerarToken(string username)
        {
            // Definir las claves de cifrado y firma
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clave_secreta_aqui"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definir las claims del usuario
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: "tu_empresa_aqui",
                audience: "tu_cliente_aqui",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            // Devolver el token como cadena de texto
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JsonResult VerificarWhatsApp()
        {
            Reply reply = new Reply();

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if ((Constants.EnviarWhatsApp == 1) && (Constants.WhatsAppMeta == 1))
            {
                string sqlQuery = "exec [SP_Meta_VerificarWhatsappUsuario] @Usuario_id, @EsInterno";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input },
                };
                ResultFromStoreProcedureWhatsapp resultFromStoreProcedureWhatsapp = db.Database.SqlQuery<ResultFromStoreProcedureWhatsapp>(sqlQuery, sqlParams).FirstOrDefault();

                reply = new Reply
                {
                    result = resultFromStoreProcedureWhatsapp.result,
                    message = resultFromStoreProcedureWhatsapp.message,
                    data = resultFromStoreProcedureWhatsapp.data,
                    details = resultFromStoreProcedureWhatsapp.details,
                };
            }
            else
            {

                reply = new Reply
                {
                    result = 1,
                    message = "",
                };
            }

            return Json(reply);
        }


    }
}



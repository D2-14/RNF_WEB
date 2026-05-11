using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Net.Mail;
using System.Data.SqlClient;
using Newtonsoft.Json;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSubRegionalController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableBanner = new PdfPTable(1);

        // GET: Form_FormularioSubRegional
        public ActionResult Index()
        {


            return View();
        }




        [HttpPost]
        public JsonResult AplicaNombramiento(long juridico)
        {
            string TextoMostrar, AplicaNombramiento;

            int intCantidad = db.Database.SqlQuery<int>("Select dbo.Fnc_Seg_AplicaNombramiento(@p0)", juridico).FirstOrDefault();

            if (intCantidad > 0)
            {
                AplicaNombramiento = "Si";
            }
            else
            {
                AplicaNombramiento = "No";
            }


            TextoMostrar = "{ \"AplicaNombramiento\" : \"" + AplicaNombramiento + "\"}";

            return Json(TextoMostrar);

        }




        public class Usuario_Rol
        {
            // Permiso de edición //
            public long Usuario_id;
            public string NombreCompleto;

        }

        public ActionResult AsignacionJuridica(string Guid_id, int? etapa_id, decimal? etaparuta_id, int? correlativoetapa_id)
        {

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;

            if (tbl_sol_Solicitud.JuridicoAsignado_id == null)
            {
                ViewBag.JuridicoAsigando = " ** Debe seleccionar a la personal jurídico responsable de la providencia.";
                ViewBag.JuridicoAsignadoNombramiento = "";
            }
            else
            {
                Tbl_Seg_Usuario JuridicoAsigando_ = db.Tbl_Seg_Usuario.Find(tbl_sol_Solicitud.JuridicoAsignado_id);

                ViewBag.JuridicoAsigando = "Juridico asignado actualmente: " + JuridicoAsigando_.Nombre + " " + JuridicoAsigando_.Apellidos;
                ViewBag.JuridicoAsignadoNombramiento = tbl_sol_Solicitud.JuridicoAsignadoNombramiento;
            }

            ViewBag.Juridico = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Juridico, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_sol_Solicitud.JuridicoAsignado_id);

            ViewBag.Guid_id = Guid_id;

            return View();
        }

        public ActionResult AsignacionJuridicaRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.CantidadEtapas = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id).Count();

            //  Opciones posibles                                                                         //
            //  ==================                                                                        //
            //  1    Asignación jurídica realizada                                                        //


            return View(tbl_gest_EtapaSolicitud);

        }


        public ActionResult GestionDeEnmiendas(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            return View();
        }


        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud
    (
        long solicitud_id,
        int etapa_id,
        decimal etaparuta_id,
        int correlativoetapa_id,
        string motivo,
        int respuestaid,
        string EtapaSolicitud_GUIDid
    )
        {

            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "El usuario no se encuentra logueado. Ingrese de nuevo al sistema.";

                string jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResultUsr);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            int CountJuridico = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.Nombre_Etapa.Contains("jurídica")).Count();
            int CountTecnico = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.Nombre_Etapa.Contains("técnico")).Count();
            int CountDocumento = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.RequiereDocumentos == true).Count();

            int CountDocumentoSubido = db.Tbl_Gest_EtapaSolicitud_Documento.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).Count();

            if ((CountDocumento > 0) && (CountDocumentoSubido == 0))
            {

                codRespuesta = 0;
                strRespuesta = "Error: Debe subir el documento requerido para poder continuar.";

                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


            }

            //Asignación jurídica y/o técnica deben contar con la palabra Asignación en el nombre de la etapa
            // Para poder hacer obligatoria la asignación.



            Tbl_Gest_Etapa Tbl_Gest_Etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.Nombre_Etapa.Contains("Asignación")).FirstOrDefault();

            if (Tbl_Gest_Etapa != null)
            {

                if ((respuestaid == 1) && (CountJuridico > 0) && (tbl_sol_solicitud.JuridicoAsignado_id == null))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede procesar la solicitud sin juridico asignado.";


                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

                if ((respuestaid == 1) && (CountTecnico > 0) && (tbl_sol_solicitud.TecnicoAsignado_id == null))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede procesar la solicitud sin técnico asignado.";


                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

            }

            Gest_EtapaModel gest_EtapaModel = new Gest_EtapaModel();
            ResultFromStoreProcedure Respuesta = gest_EtapaModel.ConfirmarRespuesta(objUs, solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid);

            //if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
            if (Respuesta.respuesta == 1)
            {

                codRespuesta = 1;
                strRespuesta = "Actualización realizada.";


                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


            }
            else
            {
                codRespuesta = 0;
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
                if ((Respuesta.mensaje ?? "").Trim() != "")
                {
                    strRespuesta = Respuesta.mensaje;
                }
            }

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);

        }
        [HttpPost]
        public JsonResult MensajeSeleccione
    (
        long solicitud_id,
        int etapa_id,
        decimal etaparuta_id,
        int correlativoetapa_id,
        string motivo,
        int respuestaid,
        string EtapaSolicitud_GUIDid
    )
        {

            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Prueba.";

                string jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResultUsr);

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

            try
            {
                smtp.Send(Correo);
            }
            catch (Exception ex)
            {
                return;
            }

            return;
        }


        [HttpPost]
        public JsonResult AsignacionJurista(
         long juridico,
         string nombramiento,
         string Guid_id)
        {

            string strRespuesta = "";
            string jsonResult = "";
            int codRespuesta = 0;

            Usuario objUs = new Usuario();

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            tbl_sol_Solicitud.JuridicoAsignado_id = juridico;
            tbl_sol_Solicitud.JuridicoAsignadoNombramiento = nombramiento;


            tbl_sol_Solicitud.JuridicoAsignadoPorSubRegional_id = objUs.intUsuario_id;
            tbl_sol_Solicitud.JuridicoAsignadoFecha = DateTime.Now;

            tbl_sol_Solicitud.swupdatedby = objUs.intUsuario_id;
            tbl_sol_Solicitud.swdateupdated = DateTime.Now;

            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            codRespuesta = 1;

            jsonResult = "{\"CodRespuesta\":"
                      + "\"" + codRespuesta + "\","
                      + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }


            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Se ha realizado la asignación jurídica";
            tbl_RSS_Usuario.Cuerpo = "El sub regional (" + db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault() + ") ha asignado un técnico. El abogado asignado fué: " + db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", juridico).FirstOrDefault();
            tbl_RSS_Usuario.Pasos_a_seguir = "";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "";
            tbl_RSS_Usuario.link_A_descripcion = "";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();



            return Json(jsonResult);
        }

        [HttpPost]
        public JsonResult AsignacionTecnicoForestal(
             long tecnico,
             string Guid_id)
        {

            string strRespuesta = "";
            string jsonResult = "";
            int codRespuesta = 0;


            Usuario objUs = new Usuario();

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            tbl_sol_Solicitud.TecnicoAsignado_id = tecnico;

            tbl_sol_Solicitud.TecnicoAsignadoPorSubRegional_id = objUs.intUsuario_id;
            tbl_sol_Solicitud.TecnicoAsignadoFecha = DateTime.Now;

            tbl_sol_Solicitud.swupdatedby = objUs.intUsuario_id;
            tbl_sol_Solicitud.swdateupdated = DateTime.Now;

            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Se ha realizado la asignación técnica";
            tbl_RSS_Usuario.Cuerpo = "El sub regional (" + db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault() + ") ha asignado un técnico. El técnico asignado fué: " + db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", tecnico).FirstOrDefault();
            tbl_RSS_Usuario.Pasos_a_seguir = "";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "";
            tbl_RSS_Usuario.link_A_descripcion = "";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();



            codRespuesta = 1;

            jsonResult = "{\"CodRespuesta\":"
                      + "\"" + codRespuesta + "\","
                      + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        public ActionResult AsignacionTecnica(string Guid_id, int? etapa_id, decimal? etaparuta_id, int? correlativoetapa_id)
        {

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;


            if (tbl_sol_Solicitud.TecnicoAsignado_id == null)
            {
                ViewBag.TecnicoAsigando = " ** Debe seleccionar a la personal técnico responsable de la providencia.";
            }
            else
            {
                Tbl_Seg_Usuario TecnicoAsigando_ = db.Tbl_Seg_Usuario.Find(tbl_sol_Solicitud.TecnicoAsignado_id);

                ViewBag.TecnicoAsigando = "Técnico asignado actualmente: " + TecnicoAsigando_.Nombre + " " + TecnicoAsigando_.Apellidos;
            }

            ViewBag.Tecnico = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Tecnico, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_sol_Solicitud.TecnicoAsignado_id);

            ViewBag.Guid_id = Guid_id;


            return View();
        }

        public ActionResult AsignacionTecnicaRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.CantidadEtapas = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id).Count();

            //  Opciones posibles                                                                         //
            //  ==================                                                                        //
            //  1    Asignación técnica realizada                                                        //


            return View(tbl_gest_EtapaSolicitud);

        }

        public int ConfirmarRespuesta_Old(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid)
        {
            Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.Respuesta_id != 0)
            {
                return 0;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return 0;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapasolicitud.Motivo = strMotivo;

            if (ModelState.IsValid)
            {

                tbl_gest_etapasolicitud.swupdatedby = objUs.intUsuario_id;
                tbl_gest_etapasolicitud.swdateupdated = DateTime.Now;

                if (objUs.EsInterno != 1)
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = true;

                }

                db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                db.SaveChanges();

            }


            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return resultado[0].respuesta;

        }

        public int ConfirmarRespuesta(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid)
        {
            Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.Respuesta_id != 0)
            {
                return 0;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return 0;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapasolicitud.Motivo = strMotivo;

            if (ModelState.IsValid)
            {

                tbl_gest_etapasolicitud.swupdatedby = objUs.intUsuario_id;
                tbl_gest_etapasolicitud.swdateupdated = DateTime.Now;

                if (objUs.EsInterno != 1)
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = true;

                }

                db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                db.SaveChanges();

            }


            string sqlQuery;
            SqlParameter[] sqlParams;

            // Inserta en la cola de casillero el documento.
            //   Respuesta = 0  significa que no informa a casillero
            //   Respesta = 1   Informar a casillero.
            //   MensajeId contiene el registro ID, con ese registro hay que consultar el casillero. Si aplica.

            sqlQuery = "Exec SP_Gest_EtapaCasillero_EVO  @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input }
                };


            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            if (resultado[0].respuesta == 1)
            {
                //Aplica enviar a casillero y verificar si fue enviado.
                CasilleroElectronicoSender casilleroElectronicoSender = new CasilleroElectronicoSender();
                Reply reply = casilleroElectronicoSender.EnviarCasilleroPendiente();
                Whatsapper whatsapper = new Whatsapper();
                _ = whatsapper.EnviarWhatsappPendiente();
                Models.Mailer mailer = new Models.Mailer();
                _ = mailer.EnviarCorreoPendiente(objUs);


                Tbl_Cas_CasilleroElectronicoCola tbl_Cas_CasilleroElectronicoCola = db.Tbl_Cas_CasilleroElectronicoCola.Find(resultado[0].mensaje);

                if ((tbl_Cas_CasilleroElectronicoCola.Estado ?? false) == false)
                {
                    return 0;
                }

            }

            sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return resultado[0].respuesta;

        }
        public ActionResult ResolucionInscripcion(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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



            ViewBag.Guid_id = Guid_id;
            ViewBag.EtapaSolicitudGuid = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;
            

            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = GuidEtapa_id;

            //string resultado = GenerarResolucion(guidsolicitud, guidetapasolicitud);
            return View();
        }



        public ActionResult ResolucionInscripcionRespuesta(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            return View();
        }




        public ActionResult ResolucionSubRegional(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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



            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = GuidEtapa_id;


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == guidsolicitud
                                            select d).FirstOrDefault();

            Tbl_Gest_EtapaSolicitud oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                                                   where
                                                      d.Solicitud_Guid_id == guidsolicitud
                                                   && d.EtapaSolicitud_GUID_id == guidetapasolicitud
                                                   && d.CorrelativoEtapa_id == correlativoetapa_id
                                                   select d).FirstOrDefault();

            if (oResolucion.Resolucion_Aprobada == null)
            {
                oResolucion.Resolucion_Aprobada = true;
            }

            if (oResolucion.Resolucion_Denegada == null)
            {
                oResolucion.Resolucion_Denegada = false;
            }

            ViewBag.Tbl_Gest_Etapa_respuestaEnmienda = db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == oResolucion.Etapa_id && Obj.EtapaRuta_id == oResolucion.EtapaRuta_id).ToList().Where(s => s.Descripcion.ToUpper().Contains("ENMIEN"));
            List<Tbl_RNF_Registro_InactivacionTecnico_Tipo> tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in db.Tbl_RNF_Registro_InactivacionTecnico_Tipo
                                                                                                          select d).ToList();

            //Resolución SubRegional *Tipo de inactivacion*
            tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                              //where d.InactivacionTecnicoTipo_id != 0
                                                          where d.InactivacionTecnicoTipo_id != 0 && d.InactivacionTecnicoTipo_id != 1
                                                          select d).ToList();


            if (oSolicitud.No_Registro != null)
            {
                List<Tbl_RNF_Registro_Bitacora> tbl_RNF_Registro_Bitacoras = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                              where d.No_Registro == oSolicitud.No_Registro
                                                                              select d).ToList();

                if (tbl_RNF_Registro_Bitacoras == null)
                {
                    tbl_RNF_Registro_Bitacoras = new List<Tbl_RNF_Registro_Bitacora>();
                }

                foreach (var item in tbl_RNF_Registro_Bitacoras)
                {
                    item.InactivacionTemporal = item.InactivacionTemporal ?? false;
                    item.InactivacionDefinitiva = item.InactivacionDefinitiva ?? false;
                }

            }

            ViewBag.TipoInactivacion_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == oSolicitud.Categoria_id && Obj.UsuarioInterno == true && Obj.TipoInactivacion_id != 3 && Obj.TipoInactivacion_id != 8).OrderBy(Obj => Obj.TipoInactivacion_id).ToList(), "TipoInactivacion_id", "Descripcion", (oSolicitud.TipoInactivacion_id ?? 0));

            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion", (oSolicitud.InactivacionTecnicoTipo_id ?? tbl_RNF_Registro_InactivacionTecnico_Tipos.FirstOrDefault().InactivacionTecnicoTipo_id));

            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == oSolicitud.Categoria_id).ToList();
            if (oSolicitud.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == oSolicitud.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");


            return View(oResolucion);
        }


        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        public string GenerarResolucion(string Guid_id, string EtapaSolicitudGuid, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Gest_EtapaSolicitud oResolucion = new Tbl_Gest_EtapaSolicitud();
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();

            oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                           where d.Solicitud_Guid_id == Guid_id && d.EtapaSolicitud_GUID_id == EtapaSolicitudGuid
                           select d).FirstOrDefault();

            oParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                         where d.Solicitud_Guid_id == Guid_id
                         select d).FirstOrDefault();



            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime fecharesolucion;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            if (oResolucion.swdatecreated == null)
            {
                fecharesolucion = hoy;
            }
            else
            {
                fecharesolucion = (DateTime)oResolucion.swdatecreated;
            }

            string strNombre = EtapaSolicitudGuid + fecha + ".pdf";
            string strDirArchivo = strFolder + strNombre;

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);


            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            var Enter = new Paragraph(" ");

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }



            CrearBanner crearBanner = new CrearBanner();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroResolucionSubRegional(Solicitud_id: oParrafos.Solicitud_id, Etapa_id: etapa_id, EtapaRuta_id: etaparuta_id, CorrelativoEtapa_id: correlativoetapa_id, Usuario_id: objUs.intUsuario_id);

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(oResolucion.Solicitud_id);

            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = (from d in db.Tbl_Gral_SolicitudConfiguracionTipo
                                                                                       where d.SolicitudTipo_id == tbl_sol_Solicitud.SolicitudTipo_id
                                                                                       select d).FirstOrDefault();

            string varTitulo = "" + tbl_Gral_SolicitudConfiguracionTipo.DescripcionEnOficio.ToUpper() + "";


            varTitulo = varTitulo.Replace("SOLICITUD", "RESOLUCIÓN");

            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            string No_Resolucion = resultsp.Identificador;

            LlenaBanner("Resolución No." + No_Resolucion, "Derecha", "Blanco");
            doc.Add(tableBanner);

            //string strFecha = db.Database.SqlQuery<string>("SELECT dbo.[Fnc_Gral_FechaSolicitudTxt]('" + SQLDate(oResolucion.swdatecreated ?? DateTime.Now) + "','" + oResolucion.Solicitud_id + "')").FirstOrDefault();

            //LlenaBanner(strFecha, "Derecha", "Blanco");
            //doc.Add(tableBanner);

            tbl_sol_Solicitud.ResolucionSolicitud = resultsp.Identificador;
            tbl_sol_Solicitud.FechaResolucionSolicitud = DateTime.Now;

            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

            doc.Add(Enter);

            Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);

            Tbl_Gral_Departamento tbl_Gral_Departamento = db.Tbl_Gral_Departamento.Find(tbl_seg_usuarioExterno.Departamento_id);

            Tbl_Gral_Municipio tbl_Gral_Municipio = db.Tbl_Gral_Municipio.Find(tbl_seg_usuarioExterno.Municipio_id);

            Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);

            Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();

          

            Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();
            var añoActual = DateTime.Now.Year;

            //CrearBanner banner = new CrearBanner();

            LlenaBanner(oParrafos.Parrafo_1.ToUpper(), "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            if (tbl_sol_Solicitud_OficioDictamenJuridico != null)
            {
            string NoDictamenJuridico = tbl_sol_Solicitud.Solicitud_NumeroExpediente.Substring(0, 3).Replace("-", "").Replace(".", "")+ "-DJ-"+ tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico + "-"+ tbl_sol_Solicitud_OficioDictamenJuridico.Numero +"-"+ añoActual;

            }

            

            if ((oParrafos.Parrafo_2 ?? "") != "")
            {
                LlenaBanner(oParrafos.Parrafo_2, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((oParrafos.Parrafo_3 ?? "") != "")
            {
                LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(oParrafos.Parrafo_3, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((oParrafos.Parrafo_4 ?? "") != "")
            {
                LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(oParrafos.Parrafo_4, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((oParrafos.Parrafo_5 ?? "") != "")
            {

                LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(oParrafos.Parrafo_5, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((oParrafos.Parrafo_6 ?? "") != "")
            {
                LlenaBanner("POR TANTO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(oParrafos.Parrafo_6, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((oParrafos.Parrafo_7 ?? "") != "")
            {
                LlenaBanner("RESUELVE", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(oParrafos.Parrafo_7, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            LlenaBanner(DatosSubRegional.NombreCompleto, "Centro", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Dirección SubRegional  " + tbl_Gral_SubRegion.No_SubRegion, "Centro", "Blanco");
            doc.Add(tableBanner);


            doc.Close();
            writer.Close();


            oResolucion.NombreDocumentoNoFirmado = strNombre;
            oResolucion.NombreDocumentoFirmado = null;

            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.Entry(oResolucion).State = EntityState.Modified;
            db.SaveChanges();

            return strNombre;
            //return null;
        }



        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
        }

        private void LlenaBanner(String Leyenda, string alineacion, string Color)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.Border = 0;

            if (Color == "Blanco")
            {
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            }

            if (Color == "Gris")
            {
                c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;
            }

            if (alineacion == "Centro")
            {
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
            }

            if (alineacion == "Derecha")
            {
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            }

            if (alineacion == "Izquierda")
            {
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
            }

            if (alineacion == "Justificado")
            {
                c1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }

            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        public JsonResult GrabarResolucion(Tbl_Gest_EtapaSolicitud model)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            DateTime swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud oResolucion = new Tbl_Gest_EtapaSolicitud();
            oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                           where d.Solicitud_id == model.Solicitud_id && d.Etapa_id == model.Etapa_id && d.EtapaRuta_id == model.EtapaRuta_id && d.CorrelativoEtapa_id == model.CorrelativoEtapa_id
                           select d).FirstOrDefault();

            if (oResolucion != null)
            {
                model.swdateupdated = swdatecreated;
                model.swupdatedby = objUs.intUsuario_id;
                model.swupdatedbyinterno = true;
                oResolucion.Resolucion_Aprobada = model.Resolucion_Aprobada;
                oResolucion.Resolucion_Denegada = model.Resolucion_Denegada;
                oResolucion.swdateupdated = model.swdateupdated;
                oResolucion.swupdatedby = model.swupdatedby;
                oResolucion.swupdatedbyinterno = model.swupdatedbyinterno;

            }
            else
            {
                model.swdatecreated = swdatecreated;
                model.swcreatedby = objUs.intUsuario_id;
                model.swcreatedbyinterno = true;
                db.Tbl_Gest_EtapaSolicitud.Add(model);
            }

            db.SaveChanges();
            string txtMostrar = "{\"Resultado\":\"Listo\"}";
            return Json(txtMostrar);
        }

        public class ListaParrafos
        {
            public string Parrafo1 { get; set; }
            public string Parrafo2 { get; set; }
            public string Parrafo3 { get; set; }
            public string Parrafo4 { get; set; }
            public string Parrafo5 { get; set; }
            public string Parrafo6 { get; set; }
            public string Parrafo7 { get; set; }
        }
        public JsonResult VistaPreviaResolucion(Tbl_Gest_EtapaSolicitud model)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            //Cancelacion
            decimal TipoGestion = tbl_Sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            decimal terminacioninactivacion = 0.06M;

            ListaParrafos oParrafos = new ListaParrafos();
            string query, validacionParrafos;

            if (TipoGestion == terminacioninactivacion)
            {
                                
                validacionParrafos = model.Solicitud_Guid_id + "','" + model.Etapa_id + "','" + model.EtapaRuta_id + "','" + model.CorrelativoEtapa_id + "','" + model.Motivo ?? "";

                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 1 + "')";
                oParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 2 + "')";
                oParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 3 + "')";
                oParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 4 + "')";
                oParrafos.Parrafo4 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 5 + "')";
                oParrafos.Parrafo5 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 6 + "')";
                oParrafos.Parrafo6 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Cancelacion('" + validacionParrafos + "','" + 7 + "')";
                oParrafos.Parrafo7 = db.Database.SqlQuery<string>(query).FirstOrDefault();

            }
            else
            {
                validacionParrafos = model.Solicitud_Guid_id + "','" + model.Etapa_id + "','" + model.EtapaRuta_id + "','" + model.CorrelativoEtapa_id;

                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 1 + "')";
            oParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 2 + "')";
            oParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 3 + "')";
            oParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 4 + "')";
            oParrafos.Parrafo4 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 5 + "')";
            oParrafos.Parrafo5 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 6 + "')";
            oParrafos.Parrafo6 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 7 + "')";
            oParrafos.Parrafo7 = db.Database.SqlQuery<string>(query).FirstOrDefault();

            }

            return Json(JsonConvert.SerializeObject(oParrafos));
        }

        public JsonResult GeneraResolucionSubRegional(Tbl_Gest_EtapaSolicitud model)
        {

                string archivo = GenerarResolucion(model.Solicitud_Guid_id, model.EtapaSolicitud_GUID_id, model.Etapa_id, model.EtapaRuta_id, model.CorrelativoEtapa_id);
                string txtMostrar = "{\"Ubicacion\":\"" + archivo + "\"}";
                return Json(txtMostrar);
         
        } 
        
        public JsonResult AprobarDenegarSeleccione()
        {
            
            ViewBag.Mensaje = "Bienvenido a mi aplicación ASP.NET MVC";

            return ViewBag;
            }

        public JsonResult GrabaParrafos(Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos model)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            DateTime swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();
            oParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                         where d.Solicitud_id == model.Solicitud_id
                         select d).FirstOrDefault();

            if (oParrafos != null)
            {
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Remove(oParrafos);
                db.SaveChanges();

                model.swdateupdated = swdatecreated;
                model.swupdatedby = objUs.intUsuario_id;
                model.swupdatedbyinterno = true;
                oParrafos.Parrafo_1 = model.Parrafo_1;
                oParrafos.Parrafo_2 = model.Parrafo_2;
                oParrafos.Parrafo_3 = model.Parrafo_3;
                oParrafos.Parrafo_4 = model.Parrafo_4;
                oParrafos.Parrafo_5 = model.Parrafo_5;
                oParrafos.Parrafo_6 = model.Parrafo_6;
                oParrafos.Parrafo_7 = model.Parrafo_7;
                oParrafos.swdateupdated = model.swdateupdated;
                oParrafos.swupdatedby = model.swupdatedby;
                oParrafos.swupdatedbyinterno = model.swupdatedbyinterno;
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Add(model);
            }
            else
            {
                model.swdatecreated = swdatecreated;
                model.swcreatedby = objUs.intUsuario_id;
                model.swcreatedbyinterno = true;
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Add(model);
            }

            db.SaveChanges();
            string txtMostrar = "{\"Resultado\":\"Listo\"}";
            return Json(txtMostrar);
        }

        public JsonResult ObtenerParrafos(Tbl_Gest_EtapaSolicitud model)
        {
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oGetParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();
            oGetParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                            where d.Solicitud_id == model.Solicitud_id
                            select d).FirstOrDefault();
            ListaParrafos oParrafos = new ListaParrafos();

            if (oGetParrafos != null)
            {

                oParrafos.Parrafo1 = oGetParrafos.Parrafo_1;
                oParrafos.Parrafo2 = oGetParrafos.Parrafo_2;
                oParrafos.Parrafo3 = oGetParrafos.Parrafo_3;
                oParrafos.Parrafo4 = oGetParrafos.Parrafo_4;
                oParrafos.Parrafo5 = oGetParrafos.Parrafo_5;
                oParrafos.Parrafo6 = oGetParrafos.Parrafo_6;
                oParrafos.Parrafo7 = oGetParrafos.Parrafo_7;
                return Json(JsonConvert.SerializeObject(oParrafos));
            }
            else
            {
                string query, validacionParrafos;
                validacionParrafos = model.Solicitud_Guid_id + "','" + model.Etapa_id + "','" + model.EtapaRuta_id + "','" + model.CorrelativoEtapa_id;
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 1 + "')";
                oParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 2 + "')";
                oParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 3 + "')";
                oParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 4 + "')";
                oParrafos.Parrafo4 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 5 + "')";
                oParrafos.Parrafo5 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 6 + "')";
                oParrafos.Parrafo6 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo('" + validacionParrafos + "','" + 7 + "')";
                oParrafos.Parrafo7 = db.Database.SqlQuery<string>(query).FirstOrDefault();


                return Json(JsonConvert.SerializeObject(oParrafos));
            }
        }


        public class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }

        public JsonResult ActualizarTipoInactivacion(Tbl_Sol_Solicitud model)
        {
            RespuestaJSON respuestaJSON = new RespuestaJSON()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };

            Usuario objUs = new Usuario();
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
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_Sol_Solicitud == null)
            {
                respuestaJSON = new RespuestaJSON()
                {
                    Result = 2,
                    Mensaje = "No pudo encontrarse la solicitud"
                };
            }
            else
            {
                try
                {
                    tbl_Sol_Solicitud.TipoInactivacion_id = model.TipoInactivacion_id;
                    db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
                    db.SaveChanges();
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 1,
                        Mensaje = "Tipo de inactivación asignado exitosamente"
                    };
                }
                catch (Exception ex)
                {
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 3,
                        Mensaje = "Ocurrió un error: " + ex.Message + " " + ex.InnerException,
                        data = ex
                    };

                }
            }

            return Json(respuestaJSON);
        }



    }
}
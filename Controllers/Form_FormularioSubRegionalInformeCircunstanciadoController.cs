using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;
using Font = iTextSharp.text.Font;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.SqlClient;
using RestSharp;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSubRegionalInformeCircunstanciadoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        PdfPTable tableTitulo = new PdfPTable(3);
        PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        PdfPTable tableDatosInstrucciones = new PdfPTable(numColumns: 8);
        PdfPTable tableDatosEvaluacion = new PdfPTable(numColumns: 8);
        PdfPTable tablePreguntasRespuestas = new PdfPTable(numColumns: 8);
        PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        PdfPTable tablePersoneria = new PdfPTable(1);
        PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        PdfPTable tableBanner = new PdfPTable(1);
        int Al_Izquierda = Element.ALIGN_LEFT;
        int Al_Centro = Element.ALIGN_CENTER;
        int Al_Derecha = Element.ALIGN_RIGHT;
        int Al_Justificado = Element.ALIGN_JUSTIFIED;
        int Al_Arriba = Element.ALIGN_TOP;
        int Al_Abajo = Element.ALIGN_BOTTOM;
        int Al_Medio = Element.ALIGN_MIDDLE;
        int Al_JustificadoTodo = Element.ALIGN_JUSTIFIED_ALL;
        int Al_NoDefinido = Element.ALIGN_UNDEFINED;
        BaseColor GrisClaro = BaseColor.LIGHT_GRAY;
        BaseColor Blanco = BaseColor.WHITE;

        iTextSharp.text.Font fntTituloTabla_10 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_11 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_12 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_13 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_14 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_15 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);

        iTextSharp.text.Font fntTituloTabla_10B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_11B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_12B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_13B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_14B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_15B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);


        public ActionResult InformeCircunstanciado(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;


            return View();
        }



        public ActionResult InformeCircunstanciado_Generar(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
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

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado tbl_Gest_EtapaSolicitud_InformeCircunstanciado = ParrafosSolicitud(GuidEtapa_id, Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_InformeCircunstanciado);
        }

        public ActionResult Procedimientos_Create(long Solicitud_id)
        {
            Usuario objUs = new Usuario();
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
            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = new Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos();
            tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Solicitud_id = Solicitud_id;

            return View(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos);
        }

        public ActionResult Procedimientos_Edit(long Solicitud_id, long Procedimiento_id)
        {
            Usuario objUs = new Usuario();
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
            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Find(Solicitud_id, Procedimiento_id);

            if(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos);
        }

        [HttpPost]
        public ActionResult Procedimientos_Edit(Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos model)
        {
            Usuario objUs = new Usuario();
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
            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Find(model.Solicitud_id, model.Procedimiento_id);

            if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            if (ModelState.IsValid)
            {
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Procedimiento = model.Procedimiento;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swupdatedby = objUs.intUsuario_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swdateupdated = DateTime.Now;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swupdatedbyinterno = Convert.ToBoolean(objUs.EsInterno);
                db.Entry(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RegistroActualizado", "Home");
            }

            return View(model);
        }



        public ActionResult Procedimientos_Lista(long Solicitud_id)
        {
            Usuario objUs = new Usuario();
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

            List<Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos> tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos
                                                                                                                                                 where d.Solicitud_id == Solicitud_id
                                                                                                                                                 orderby d.Procedimiento_id
                                                                                                                                                 select d).ToList();


            return View(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos);
        }



        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud(long solicitud_id,int etapa_id,decimal etaparuta_id,int correlativoetapa_id,string motivo,int respuestaid,string EtapaSolicitud_GUIDid)
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


            Gest_EtapaModel gest_EtapaModel = new Gest_EtapaModel();
            ResultFromStoreProcedure Respuesta = gest_EtapaModel.ConfirmarRespuesta(objUs, solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid);
            //if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
            if (Respuesta.respuesta == 1)
            {


                codRespuesta = 1;
                strRespuesta = "Se ha notificado la respuesta.";


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


        public string GetBearer()
        {
            try
            {

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
        //                        @"      ""Coordenadas"": ""70,700,180,760"",
        //        " + "\n" +
        //                        @"      ""NumeroPagina"": 1
        //        " + "\n" +
        //                        @"    }
        //        " + "\n" +
        //                        @" ]
        //        " + "\n" +
        //        @"}";

        //        //@"      ""Coordenadas"": ""260,1500,60,120"",


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
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;
            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);


            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

            /// bbarillas
            rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;
            /// bbarillas

            strBearer = GetBearer();

            if (strBearer.Length < 125)
            {
                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                return Json(jsonResultUsr);

            }

            string strDocumentoSubido = CallCORS(strBearer, rootpdf);

            if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
            {

                intRespuesta = 0;

                strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                return Json(jsonResultUsr);


            }

            RequestUtil requestUtil = new RequestUtil();

            string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);

            //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

            if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
            {


                // se cambio de 0 a 1 para pruebas de emananuel

                intRespuesta = 1;

                strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                return Json(jsonResultUsr);


            }


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

            }


            intRespuesta = resultFromStoreProcedure.respuesta;

            if (intRespuesta == 1)
            {
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



            //intRespuesta = 1;

            //jsonResultUsr = "{\"CodRespuesta\":"
            //                      + "\"" + intRespuesta + "\","
            //                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

            return Json(jsonResultUsr);
        }







        public Tbl_Gest_EtapaSolicitud_InformeCircunstanciado ParrafosSolicitud(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, Usuario objUs)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsInterno = false;
            if (objUs.EsInterno != 0)
            {
                boolEsInterno = true;
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();
            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado tbl_Gest_EtapaSolicitud_InformeCircunstanciado = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado
                                                                                                             where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                                           select d).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado == null)
            {
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado = new Tbl_Gest_EtapaSolicitud_InformeCircunstanciado();
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
                Parrafos parrafos = ListarParrafos(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id);
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_1 = parrafos.Parrafo_1;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_2 = parrafos.Parrafo_2;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_3 = parrafos.Parrafo_3;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_4 = parrafos.Parrafo_4;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_5 = parrafos.Parrafo_5;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_6 = parrafos.Parrafo_6;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_7 = parrafos.Parrafo_7;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swcreatedby = objUs.intUsuario_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swdatecreated = swdatecreated;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swcreatedbyinterno = boolEsInterno;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.EtapaSolicitud_Guid_id = GuidEtapa_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Solicitud_Guid_id = Guid_id;

                db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Add(tbl_Gest_EtapaSolicitud_InformeCircunstanciado);
                db.SaveChanges();


            }

            return tbl_Gest_EtapaSolicitud_InformeCircunstanciado;
        }
        public class Parrafos
        {
            public string Parrafo_1 { get; set; }
            public string Parrafo_2 { get; set; }
            public string Parrafo_3 { get; set; }
            public string Parrafo_4 { get; set; }
            public string Parrafo_5 { get; set; }
            public string Parrafo_6 { get; set; }
            public string Parrafo_7 { get; set; }
            public string Parrafo_8 { get; set; }
            public string Parrafo_9 { get; set; }
            public string Parrafo_10 { get; set; }
        }

        public Parrafos ListarParrafos(string Solicitud_Guid_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id)
        {
            int countParrafos = 0;
            Parrafos parrafos = new Parrafos();

            string sqlQuery, validaParams, strSelect;

            strSelect = "select [dbo].[Fcn_Gest_EtapaSolicitud_Inactivacion_Parrafo_Tipo]";
            validaParams = $"'{Solicitud_Guid_id}','{Etapa_id}','{EtapaRuta_id}','{CorrelativoEtapa_id}','2'";

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_1 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_2 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_3 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_4 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_5 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_6 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_7 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_8 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_9 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_10 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            return parrafos;
        }

        public JsonResult ReiniciarParrafos(string Solicitud_Guid_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id)
        {
            Parrafos parrafos = ListarParrafos(Solicitud_Guid_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id);

            return Json(parrafos);
        }

        public JsonResult GrabarParrafos(Tbl_Gest_EtapaSolicitud_InformeCircunstanciado model)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                Tbl_Gest_EtapaSolicitud_InformeCircunstanciado tbl_Gest_EtapaSolicitud_InformeCircunstanciado = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado
                                                                                                                 where d.Solicitud_id == model.Solicitud_id
                                                                                                                 select d).FirstOrDefault();

                if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado != null)
                {

                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_1 = model.Parrafo_1;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_2 = model.Parrafo_2;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_3 = model.Parrafo_3;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_4 = model.Parrafo_4;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_5 = model.Parrafo_5;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_6 = model.Parrafo_6;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_7 = model.Parrafo_7;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swupdatedby = objUs.intUsuario_id;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swupdatedbyinterno = boolEsInterno;
                    tbl_Gest_EtapaSolicitud_InformeCircunstanciado.swdateupdated = swdatecreated;
                    db.Entry(tbl_Gest_EtapaSolicitud_InformeCircunstanciado).State = EntityState.Modified;
                }
                else
                {

                    model.swcreatedbyinterno = boolEsInterno;
                    model.swcreatedby = objUs.intUsuario_id;
                    model.swdatecreated = swdatecreated;
                    db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Add(model);

                }

                db.SaveChanges();

                jsonResupesta.Result = 1;
                jsonResupesta.Mensaje = "Registro agregado exitosamente";

            }

            return Json(jsonResupesta);
        }



        private void LlenaBanner(String Leyenda, string Color = "", int alineacionhorizontal = 0, int alineacionvertical = 5, int borde = 0, string estilotexto = "Normal")
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);
            Font fntTituloBold = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            Font estiloFont = FontFactory.GetFont("HELVETICA", size: 10, FontColour);
            if (estilotexto == "Normal")
            {
                estiloFont = fntTituloTabla;
            }
            else if (estilotexto == "Bold")
            {
                estiloFont = fntTituloBold;
            }

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, estiloFont));

            c1.Border = borde;

            if (Color == "Blanco")
            {
                c1.BackgroundColor = Blanco;
            }
            else if (Color == "GrisClaro")
            {
                c1.BackgroundColor = GrisClaro;
            }
            else
            {

            }

            c1.HorizontalAlignment = alineacionhorizontal;

            c1.VerticalAlignment = alineacionvertical;

            tableBanner.AddCell(c1);

            return;
        }
        private void LlenaDos_UnoTextosDosResaltadoUnidos(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTablaSimple = fntTituloTabla_11;
            iTextSharp.text.Font fntTituloTablaResaltado = fntTituloTabla_11B;

            tableTitulo = new PdfPTable(3);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTablaSimple));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTablaResaltado));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        public string GenerarInformeCircunstanciado_PDF(Tbl_Sol_Solicitud tbl_Sol_Solicitud, Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud)
        {
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

            Tbl_Gest_EtapaSolicitud_InformeCircunstanciado tbl_Gest_EtapaSolicitud_InformeCircunstanciado = db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Find(tbl_Sol_Solicitud.Solicitud_id);

            if(tbl_Gest_EtapaSolicitud_InformeCircunstanciado == null)
            {
                return null;
            }

            List<Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos> tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos
                                                                                                                                                 where d.Solicitud_id == tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Solicitud_id
                                                                                                                                                 select d).ToList();

            if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos == null)
            {
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = new List<Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos>();
            }

            string sqlQuery;

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();

            if(tbl_RNF_Registro_Bitacora == null)
            {
                return null;
            }

            List<Tbl_RNF_Registro_BitacoraHallazgo> tbl_RNF_Registro_BitacoraHallazgos = (from d in db.Tbl_RNF_Registro_BitacoraHallazgo
                                                                                          where d.Solicitud_id == tbl_RNF_Registro_Bitacora.Solicitud_id
                                                                                          && d.No_Registro == tbl_RNF_Registro_Bitacora.No_Registro
                                                                                          && d.Bitacora_id == tbl_RNF_Registro_Bitacora.Bitacora_id
                                                                                          orderby d.Correlativo_id
                                                                                          select d).ToList();

            if(tbl_RNF_Registro_BitacoraHallazgos == null)
            {
                tbl_RNF_Registro_BitacoraHallazgos = new List<Tbl_RNF_Registro_BitacoraHallazgo>();
            }

            Constants constants = new Constants();
            CrearBanner crearBanner = new CrearBanner();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = new Result_SP_IdentificadorOficialGestion();
            
            resultsp = identificadorOficialGestion.ObtenerNumeroOficio(16, tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);
            
            //resultsp = new Result_SP_IdentificadorOficialGestion()
            //{
            //    respuesta = 1,
            //    Identificador = "Identificador",
            //    Version = "Version",
            //    Fecha = DateTime.Now,
            //    strFecha = "strFecha",
            //    Codigo = "Codigo",
            //    id = 1
            //};

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            string fechaDocumento = "";
            decimal SolicitudTipo_id = 0;
            var Enter = new Paragraph(" ");
            long contador = 0;

            SolicitudTipo_id = tbl_Sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);



            strNombre = "U" + tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id + ".pdf";
            strDirArchivo = strFolder + strNombre;


            fechaDocumento = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_FechaTxtSinGuatemala](getdate())").FirstOrDefault();


            //Regla para concatenar el nombre del usuario


            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 50f, 50f);

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }


            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();

            try
            {

                // imagen de fondo

                string imagenMarcaAgua = Server.MapPath("~/Content/images/logoInab_VerticalSello.png");
                float posX, posY;
                Image image = Image.GetInstance(imagenMarcaAgua);
                PdfGState pdfGState = new PdfGState();
                pdfGState.FillOpacity = 0.23f;
                PdfContentByte pdfContentByte = writer.DirectContentUnder;
                //posX = (writer.PageSize.Right / 2) - (image.Width / 2);
                //posY = (writer.PageSize.Top / 2) - (image.Height / 2);
                //image.SetAbsolutePosition(posX, posY);
                image.SetAbsolutePosition(0, 0);

                //  imagen de fondo


                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    crearBanner.LlenaBanner(this.Url.Action(), "Izquierda", "Gris");
                    doc.Add(crearBanner.tableBanner);
                }

                string varTitulo = "INFORME CIRCUNSTANCIADO";

                crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);

                crearBanner.LlenaBanner(fechaDocumento, "Derecha", "");
                doc.Add(crearBanner.tableBanner);
                crearBanner.LlenaBanner("Informe No." + resultsp.Identificador, "Derecha", "");
                doc.Add(crearBanner.tableBanner);
                doc.Add(Enter);

                if((tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_1 != null) && (tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_1.Trim() != ""))
                {
                    crearBanner.LlenaBanner(tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_1, "Izquierda", "");
                    doc.Add(crearBanner.tableBanner);
                }


                if ((tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_2 != null) && (tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_2.Trim() != ""))
                {
                    crearBanner.LlenaBanner(tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_2, "Izquierda", "");
                    doc.Add(crearBanner.tableBanner);
                    doc.Add(Enter);
                }

                if ((tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_3 != null) && (tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_3.Trim() != ""))
                {
                    crearBanner.LlenaBanner(tbl_Gest_EtapaSolicitud_InformeCircunstanciado.Parrafo_3, "Justificado", "");
                    doc.Add(crearBanner.tableBanner);
                    doc.Add(Enter);
                }


                if (tbl_RNF_Registro_BitacoraHallazgos.Count() > 0)
                {
                    contador = 0;
                    foreach (var item in tbl_RNF_Registro_BitacoraHallazgos)
                    {
                        if((item.DescripcionHallazgo != null) && (item.DescripcionHallazgo.Trim() != ""))
                        {
                            contador++;
                            crearBanner.LlenaBanner(contador + ". " + item.DescripcionHallazgo, "Justificado", "");
                            doc.Add(crearBanner.tableBanner);
                            doc.Add(Enter);
                        }
                    }
                }


                if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Count() > 0)
                {
                    contador = 0;
                    LlenaBanner(Leyenda: "Contenido del expediente:", estilotexto: "Bold");
                    doc.Add(tableBanner);
                    foreach (var item in tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos)
                    {
                        if ((item.Procedimiento != null) && (item.Procedimiento.Trim() != ""))
                        {
                            contador++;
                            crearBanner.LlenaBanner(contador + ". " + item.Procedimiento, "Justificado", "");
                            doc.Add(crearBanner.tableBanner);
                            doc.Add(Enter);
                        }
                    }
                }



                doc.Close();
                writer.Close();
            }
            catch (Exception ex)
            {

                doc.Close();
                writer.Close();
            }


            return strNombre;
        }

        public class JsonResupesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }
        public JsonResult ObtenerDocumentoInactivacion(Tbl_Gest_EtapaSolicitud model)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                try
                {
                    Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                                       where d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id
                                                                       select d).FirstOrDefault();

                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_Gest_EtapaSolicitud.Solicitud_id);
                    List<Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos> tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos
                                                                                                                                                         where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                                                                                         select d).ToList();
                    if(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Count() == 0)
                    {
                        jsonResupesta.Result = 2;
                        jsonResupesta.Mensaje = "Debe registrar procedimientos para el expediente";
                        return Json(jsonResupesta);

                    }
                    jsonResupesta.Ubicacion = GenerarInformeCircunstanciado_PDF(tbl_Sol_Solicitud, tbl_Gest_EtapaSolicitud);
                    if (jsonResupesta.Ubicacion != null)
                    {
                        jsonResupesta.Result = 1;
                        jsonResupesta.Mensaje = "Archivo generado exitosamente";
                        tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = jsonResupesta.Ubicacion;
                        tbl_Gest_EtapaSolicitud.swdateupdated = swdatecreated;
                        tbl_Gest_EtapaSolicitud.swupdatedby = objUs.intUsuario_id;
                        tbl_Gest_EtapaSolicitud.swupdatedbyinterno = boolEsInterno;

                        db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        jsonResupesta.Result = 2;
                        jsonResupesta.Mensaje = "No se pudo generar el archivo, no se encontró el documento requerido";
                    }
                }
                catch (Exception ex)
                {
                    jsonResupesta.Result = 3;
                    jsonResupesta.Mensaje = "Ocurrió un error: " + ex.Message;
                }
            }
            return Json(jsonResupesta);
        }

        public JsonResult AgregarProcedimiento(Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos model)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                long Procedimiento_id = 0;
                try
                {
                    Procedimiento_id = db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Where(Obj => Obj.Solicitud_id == model.Solicitud_id).Max(Obj => Obj.Procedimiento_id);
                }
                catch (Exception ex)
                {
                    Procedimiento_id = 0;
                }
                Procedimiento_id++;

                Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = new Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos();
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Solicitud_id = model.Solicitud_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Procedimiento_id = Procedimiento_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Procedimiento = model.Procedimiento;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Solicitud_Guid_id = model.Solicitud_Guid_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.EtapaSolicitud_Guid_id = model.EtapaSolicitud_Guid_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swcreatedby = objUs.intUsuario_id;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swcreatedbyinterno = boolEsInterno;
                tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.swdatecreated = swdatecreated;

                db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Add(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos);
                db.SaveChanges();

                jsonResupesta.Result = 1;
                jsonResupesta.Mensaje = "Registro agregado exitosamente";

            }

            return Json(jsonResupesta);
        }

        public JsonResult EliminarProcedimiento(Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos model)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos = (from d in db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos
                                                                                                                                               where d.Solicitud_id == model.Solicitud_id && d.Procedimiento_id == model.Procedimiento_id
                                                                                                                                               select d).FirstOrDefault();

                if (tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos != null)
                {
                    db.Tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos.Remove(tbl_Gest_EtapaSolicitud_InformeCircunstanciado_Procedimientos);
                    db.SaveChanges();

                    jsonResupesta.Result = 1;
                    jsonResupesta.Mensaje = "Procedimiento eliminado";

                }
                else
                {

                    jsonResupesta.Result = 2;
                    jsonResupesta.Mensaje = "Procedimiento no encontrado";

                }

            }

            return Json(jsonResupesta);
        }

    }
}
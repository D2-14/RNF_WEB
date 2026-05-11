using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GemBox.Spreadsheet;
using RNF_Web.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace RNF_Web.Controllers
{
    public class Form_FormularioController : Controller
    {

        string strParentGoogleDrive = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";
        string Address_Bearer = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/login/authenticate";
        string Address_GoogleDriveUpload = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive";
        string Address_FirmaElectronica = "http://" + Constants.IP_FirmaElectronica +  "/Api_RNF/api/FirmaElectronica";
        string Address_GoogleDriveDownLoad = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive/";


        string RNF_Username = "rnf";
        string RNF_Password = "NWJjEr9Q.3+w_rM=";

        string parentGoogleDriveId = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";

        private db_RNFEntities db = new db_RNFEntities();


        public class Rootobject
        {
            public string Estado { get; set; }
            public Datum[] Data { get; set; }
        }

        public class Datum
        {
            public string GoogleDriveIdAnterior { get; set; }
            public string GoogleDriveIdNuevo { get; set; }
        }


        // GET: Form_Formulario
        public ActionResult EditarFormulario(long solicitud_id, int etapa_Respuesta_id, int etapa_id, decimal etapaRuta_id, int correlativoEtapa_id, int tipoFormulario_id)
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

            Tbl_Gest_EtapaSolicitud_RespuestaFormulario tbl_Gest_EtapaSolicitud_RespuestaFormulario = db.Tbl_Gest_EtapaSolicitud_RespuestaFormulario.Find(solicitud_id, etapa_Respuesta_id, etapa_id, etapaRuta_id, correlativoEtapa_id, tipoFormulario_id);
            Tbl_Form_Formulario tbl_form_formulario;

            if (tbl_Gest_EtapaSolicitud_RespuestaFormulario.Form_Formulario_id == null || tbl_Gest_EtapaSolicitud_RespuestaFormulario.Form_Formulario_id == 0)
            {
                tbl_form_formulario = new Tbl_Form_Formulario();

                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Form_Formulario.Max(u => u.Form_Formulario_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_form_formulario.Form_Formulario_id = lngIdt;

                tbl_form_formulario.swdatecreated = DateTime.Now;
                tbl_form_formulario.swcreatedby = objUs.intUsuario_id;
                tbl_form_formulario.swdateupdated = DateTime.Now;
                tbl_form_formulario.swupdatedby = objUs.intUsuario_id;
                tbl_form_formulario.TipoFormulario_id = tipoFormulario_id;
                tbl_form_formulario.Estado_id = false;
                tbl_form_formulario.ArchivoNombre = "";

                if (objUs.EsInterno != 1)
                {
                    tbl_form_formulario.swcreatedbyinterno = false;

                }
                else
                {
                    tbl_form_formulario.swcreatedbyinterno = true;

                }

                tbl_form_formulario.swupdatedbyinterno = tbl_form_formulario.swcreatedbyinterno;

                DateTime hoy = DateTime.Now;

                string fecha = hoy.Year + @"/" + hoy.Month;

                string PathCrear = "~/Archivos_ConFirmaElectronica/" + fecha;

                if (!Directory.Exists(Server.MapPath(PathCrear)))
                    Directory.CreateDirectory(Server.MapPath(PathCrear));


                db.Tbl_Form_Formulario.Add(tbl_form_formulario);
                db.SaveChanges();

                tbl_Gest_EtapaSolicitud_RespuestaFormulario.Form_Formulario_id = tbl_form_formulario.Form_Formulario_id;

                db.Entry(tbl_Gest_EtapaSolicitud_RespuestaFormulario).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                tbl_form_formulario = db.Tbl_Form_Formulario.Find(tbl_Gest_EtapaSolicitud_RespuestaFormulario.Form_Formulario_id);

                tbl_form_formulario.swdateupdated = DateTime.Now;
                tbl_form_formulario.swupdatedby = objUs.intUsuario_id;


                if (objUs.EsInterno != 1)
                {
                    tbl_form_formulario.swupdatedbyinterno = false;
                }
                else
                {
                    tbl_form_formulario.swupdatedbyinterno = true;
                }


                db.Entry(tbl_form_formulario).State = EntityState.Modified;
                db.SaveChanges();

            }

            if (tbl_form_formulario.TipoFormulario_id == 1)
            {
                return RedirectToAction("../Form_Formulario/Formulario_DenegarSolicitud", new { form_formulario_id = tbl_form_formulario.Form_Formulario_id });
            }

            return View();

        }

        // GET: Form_Formulario
        public ActionResult Formulario_DenegarSolicitud(long form_formulario_id)
        {
            Tbl_Form_Formulario tbl_form_formulario = db.Tbl_Form_Formulario.Find(form_formulario_id);

            return View(tbl_form_formulario);

        }

        private int FirmaElectronica(Tbl_Form_Formulario tbl_form_formulario)
        {
            Tbl_Form_Formulario_FirmaElectronica_Bitacora tbl_Form_Formulario_FirmaElectronica_Bitacora;
            Tbl_Gest_Etapa_RespuestaFormulario_Tipo tbl_gest_Etapa_RespuestaFormulario_Tipo = db.Tbl_Gest_Etapa_RespuestaFormulario_Tipo.Find(tbl_form_formulario.TipoFormulario_id);

            return 1;
        }

        public void AgregarBitacora(long Form_Formulario_id, string firma, int estado, string strMsg)
        {
            Tbl_Form_Formulario_FirmaElectronica_Bitacora tbl_Form_Formulario_FirmaElectronica_Bitacora = new Tbl_Form_Formulario_FirmaElectronica_Bitacora();

            tbl_Form_Formulario_FirmaElectronica_Bitacora.Form_Formulario_id = Form_Formulario_id;
            tbl_Form_Formulario_FirmaElectronica_Bitacora.Firmante = firma;
            tbl_Form_Formulario_FirmaElectronica_Bitacora.FirmaElectronica_Estado_id = estado;
            tbl_Form_Formulario_FirmaElectronica_Bitacora.swdatecreated = DateTime.Now;
            tbl_Form_Formulario_FirmaElectronica_Bitacora.Resultado = strMsg;

            db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.Add(tbl_Form_Formulario_FirmaElectronica_Bitacora);
            db.SaveChanges();


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Formulario_DenegarSolicitud(Tbl_Form_Formulario tbl_form_formulario)
        {

            if (tbl_form_formulario.FirmaElectronica_Estado_id == null)
            {
                tbl_form_formulario.FirmaElectronica_Estado_id = 0;
            }

            if (tbl_form_formulario.FirmaElectronica_Estado_id == 0)
            {

                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 0, "Iniciado");

                tbl_form_formulario.ArchivoNombre = Generar_Formulario_DenegarSolicitud();
                tbl_form_formulario.FirmaElectronica_Estado_id = 1;

                db.Entry(tbl_form_formulario).State = EntityState.Modified;
                db.SaveChanges();

                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 1, "Archivo generado en PDF :" + tbl_form_formulario.ArchivoNombre);

            }

            if (ProcesarFirmaElectronica(tbl_form_formulario) == true)
            {
                Tbl_Form_Formulario formularioFirmado = db.Tbl_Form_Formulario.Find(tbl_form_formulario.Form_Formulario_id);

                return RedirectToAction("../Archivos_ConFirmaElectronica/"+ formularioFirmado.ArchivoNombre+".pdf");
            }
            else
            {
                return RedirectToAction("../Home/FirmaElectronicaConProblemas", new { Form_Formulario_id = tbl_form_formulario.Form_Formulario_id });

            }


        }


        public bool ProcesarFirmaElectronica(Tbl_Form_Formulario tbl_form_formulario)
        {
            bool bl_resultado = true;
            string strBearer;
            string rootpath = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";
            string rootpdf = rootpath + tbl_form_formulario.ArchivoNombre;

            if (tbl_form_formulario.FirmaElectronica_Estado_id == 1)
            {
                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 2, "Inicia la gestion de Firma electronica");
                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 3, "Gestionando Bearer");
                strBearer = GetBearer();
                if (strBearer.Length < 125)
                {
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 3, "Error. Fallo al tratar de conseguir Bearer" + strBearer);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 4, "Subidendo archivo a Google");

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length > 30) && (strDocumentoSubido.Length < 36))
                {
                    tbl_form_formulario.FirmaElectronica_Estado_id = 4;
                    tbl_form_formulario.ArchivoNombre = strDocumentoSubido;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 4, "Archivo subido a Google de forma exitosa");

                }
                else
                {
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 4, strDocumentoSubido);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 5, "Se intentara firmar el documento");
                //string strDocumentofirmado = firmarFile(strBearer, tbl_form_formulario.Usuario_FirmaElectronica, tbl_form_formulario.Password_FirmaElectronica, tbl_form_formulario.ArchivoNombre);

                RequestUtil requestUtil = new RequestUtil();

                string strDocumentofirmado = requestUtil.firmarFile(tbl_form_formulario.Usuario_FirmaElectronica, tbl_form_formulario.Password_FirmaElectronica, tbl_form_formulario.ArchivoNombre);



                if ((strDocumentofirmado.Length > 30) && (strDocumentofirmado.Length < 36))
                {
                    tbl_form_formulario.FirmaElectronica_Estado_id = 5;
                    tbl_form_formulario.ArchivoNombre = strDocumentofirmado;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 5, "Documento firmado de forma exitosa.");

                }
                else
                {
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 5, strDocumentofirmado);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 6, "Intentando guardar archivo de forma local.");

                if (getFile(strBearer, tbl_form_formulario.ArchivoNombre, rootpath) == true)
                {
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 6, "Archivo grabado en server local.");
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 7, "Fin de la firma electronica.");
                    AgregarBitacora(tbl_form_formulario.Form_Formulario_id, tbl_form_formulario.Usuario_FirmaElectronica, 10, "Documento con firma electronica.");

                    tbl_form_formulario.FirmaElectronica_Estado_id = 10;
                    tbl_form_formulario.ArchivoNombre = strDocumentofirmado;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return bl_resultado;
        }

        public bool getFile(string strBearer, string strFile, string path)
        {
            try
            {
                var client = new RestClient(Address_GoogleDriveDownLoad + strFile);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", strBearer);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                byte[] buffer = client.DownloadData(request);

                MemoryStream ms = new MemoryStream(buffer);
                FileStream file = new FileStream(@path+ strFile+".pdf", FileMode.Create, FileAccess.Write);
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



        //public string firmarFile(string bearer, string strUsuarioFirma, string strUsuarioPassword, string strDocumento)
        //{
        //    string strRespuesta = "";

        //    try
        //    {
        //        var client = new RestClient(Address_FirmaElectronica);
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

        //        //@"      ""Coordenadas"": ""60,60,120,120"",


        //        body = body.Replace("strUsuarioFirma", strUsuarioFirma).Replace("strUsuarioPassword", strUsuarioPassword).Replace("strparentGoogleDriveId", parentGoogleDriveId);
        //        body = body.Replace("strDocumento", strDocumento);

        //        request.AddParameter("application/json", body, ParameterType.RequestBody);
        //        IRestResponse response = client.Execute(request);

        //        if (response.Content.ToString().IndexOf("Error") > 0)
        //        {
        //            return response.Content.ToString()+ "  Usuario ó Password erroneo en firma.";
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
        //        return "Error al intentar firmar el archivo."+ex.Message.ToString();
        //    }

            
        //}



        public string CallCORS(string strSign, string strPath)
        {
            try
            {
                var client = new RestClient(Address_GoogleDriveUpload);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", strSign);
                //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
                request.AddFile("nombre", strPath, "application/pdf");
                request.AddParameter("parentGoogleDriveId", strParentGoogleDrive);
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                return joResponse["GoogleDriveId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error: No se logró subir el archivo a Google Drive, reporte a informatica. " + ex.Message.ToString();
            }
        }

        public string GetBearer()
        {
            try
            {

                var client = new RestClient(Address_Bearer);
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

                body = body.Replace("RNFUser", RNF_Username);
                body = body.Replace("RNF_Password", RNF_Password);

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

        private string Generar_Formulario_DenegarSolicitud()
        {


            string rootpath = Server.MapPath("~/");
            string escribirxlsx = rootpath + "Archivos_Machotes/Denegacion_De_Solicitud.xlsx";
            string rootdest = rootpath + "Archivos_Generados_Que_Pueden_Borrar/";
            string rootdestpdf = rootpath + "Archivos_ConFirmaElectronica/";

            DateTime hoy = DateTime.Now;
            string fecha = hoy.Millisecond + "_" + hoy.Minute + "_" + hoy.Day + "_" + hoy.Month + "_" + hoy.Year;
            //string correo = emailEnviar.Substring(0, 3).ToString();
            string newfilexlsx = @"Denegacion_Solicitud_" + fecha;

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            ExcelFile LibroExcel = ExcelFile.Load(escribirxlsx);
            GemBox.Spreadsheet.ExcelWorksheet sheet = LibroExcel.Worksheets[0];

            ExcelCell cell;


            cell = sheet.Cells["B34"];
            cell.Value = "test estes";


            //sheet.Rows.InsertEmpty(29);   /*Las agrega arriba del D30*/
            //sheet.Rows.InsertEmpty(29);

            LibroExcel.Save(rootdest + newfilexlsx + ".xlsx");



            var workbook = ExcelFile.Load(rootdest + newfilexlsx + ".xlsx");

            foreach (var worksheet in workbook.Worksheets)
            {
                var printOptions = worksheet.PrintOptions;
                printOptions.LeftMargin =
                printOptions.RightMargin =
                printOptions.TopMargin =
                printOptions.BottomMargin = .5;

                printOptions.AutomaticPageBreakScalingFactor = 100;
            }

            var saveOptions = new PdfSaveOptions();
            saveOptions.SelectionType = SelectionType.EntireFile;

            workbook.Save(rootdestpdf + newfilexlsx + ".pdf", saveOptions);

            return newfilexlsx + ".pdf";
        }

        public ActionResult Formulario_MotivoDenegacion(long form_formulario_id)
        {
            var tbl_form_formulario = db.Tbl_Form_Formulario_MotivoDenegacion.Where(Obj => Obj.Form_Formulario_id == form_formulario_id && Obj.Estado_id == true).ToList();

            return View(tbl_form_formulario);

        }

        public JsonResult Eliminar_DenegacionDetalle(long form_formulario_id, int correlativo)
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

            Tbl_Form_Formulario_MotivoDenegacion tbl_form_formulario_MotivoDenegacion = db.Tbl_Form_Formulario_MotivoDenegacion.Find(form_formulario_id, correlativo);

            tbl_form_formulario_MotivoDenegacion.swupdatedby = objUs.intUsuario_id;
            tbl_form_formulario_MotivoDenegacion.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_form_formulario_MotivoDenegacion.swupdatedbyinterno = false;

            }
            else
            {
                tbl_form_formulario_MotivoDenegacion.swupdatedbyinterno = true;

            }

            tbl_form_formulario_MotivoDenegacion.Estado_id = false;

            if (ModelState.IsValid)
            {

                db.Entry(tbl_form_formulario_MotivoDenegacion).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json("");
        }

        public JsonResult Agregar_DenegacionDetalle(long form_formulario_id, string descripcion)
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

            int intKey = 0;

            try
            {
                intKey = db.Tbl_Form_Formulario_MotivoDenegacion.Where(MD => MD.Form_Formulario_id == form_formulario_id).Max(u => u.Correlativo);
                intKey++;
            }
            catch
            {
                intKey = 1;
            }

            Tbl_Form_Formulario_MotivoDenegacion tbl_form_formulario_MotivoDenegacion = new Tbl_Form_Formulario_MotivoDenegacion();

            tbl_form_formulario_MotivoDenegacion.Form_Formulario_id = form_formulario_id;
            tbl_form_formulario_MotivoDenegacion.Correlativo = intKey;

            tbl_form_formulario_MotivoDenegacion.Descripcion = descripcion;

            tbl_form_formulario_MotivoDenegacion.swcreatedby = objUs.intUsuario_id;
            tbl_form_formulario_MotivoDenegacion.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_form_formulario_MotivoDenegacion.swcreatedbyinterno = false;

            }
            else
            {
                tbl_form_formulario_MotivoDenegacion.swcreatedbyinterno = true;
            }


            tbl_form_formulario_MotivoDenegacion.swupdatedby = objUs.intUsuario_id;
            tbl_form_formulario_MotivoDenegacion.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_form_formulario_MotivoDenegacion.swupdatedbyinterno = false;

            }
            else
            {
                tbl_form_formulario_MotivoDenegacion.swupdatedbyinterno = true;

            }

            tbl_form_formulario_MotivoDenegacion.Estado_id = true;

            if (ModelState.IsValid)
            {
                db.Tbl_Form_Formulario_MotivoDenegacion.Add(tbl_form_formulario_MotivoDenegacion);
                db.SaveChanges();
            }
            return Json("");
        }

        // POST: Form_Formulario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }

}

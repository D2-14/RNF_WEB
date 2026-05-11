using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.IO;
using GemBox.Spreadsheet;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Form_FormularioJuridicoController : Controller
    {


        string strParentGoogleDrive = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";
        string Address_Bearer = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/login/authenticate";
        string Address_GoogleDriveUpload = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive";
        string Address_FirmaElectronica = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/FirmaElectronica";
        string Address_GoogleDriveDownLoad = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive/";


        string RNF_Username = "rnf";
        string RNF_Password = "NWJjEr9Q.3+w_rM=";

        string parentGoogleDriveId = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";


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


        private db_RNFEntities db = new db_RNFEntities();

        private string glNombreSolicitante;
        private string glNombreSecretaria;


        // GET: Form_FormularioSecretaria
        public ActionResult Index(string Guid_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            long solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosRequeridos(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            ViewBag.DocumentoASubir = new SelectList(db.fc_Sol_DocumentosRequeridos(solicitud_id), "Tipo_Documento_id", "Nombre");

            return View();
        }

        public ActionResult IndexRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            return View(tbl_gest_EtapaSolicitud);
        }

        public ActionResult CreacionOficio(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();
            Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_solicitud_OficioDictamenJuridico;
            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            int cantidad = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id).Count();

            ViewBag.Guid_id = Guid_id;


            if (cantidad > 0)
            {

                tbl_sol_solicitud_OficioDictamenJuridico = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id).First();
            }
            else
            {

                tbl_sol_solicitud_OficioDictamenJuridico = new Tbl_Sol_Solicitud_OficioDictamenJuridico();
                Tbl_Sol_Solicitud_OficioDictamenJuridico temp_tbl_Sol_Solicitud_OficioDictamenJuridico;
                try
                {
                    int maxCorrelativoEtapa_id = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(Obj => Obj.CorrelativoEtapa_id);
                    temp_tbl_Sol_Solicitud_OficioDictamenJuridico = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.CorrelativoEtapa_id == maxCorrelativoEtapa_id).First();
                }
                catch
                {
                    temp_tbl_Sol_Solicitud_OficioDictamenJuridico = null;
                }

                if (temp_tbl_Sol_Solicitud_OficioDictamenJuridico != null)
                {

                    tbl_sol_solicitud_OficioDictamenJuridico.Numero = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.Numero;
                    tbl_sol_solicitud_OficioDictamenJuridico.InicialesDelJuridico = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico;
                    tbl_sol_solicitud_OficioDictamenJuridico.Antecedentes = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.Antecedentes;
                    tbl_sol_solicitud_OficioDictamenJuridico.FundamentoLegal = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.FundamentoLegal;
                    tbl_sol_solicitud_OficioDictamenJuridico.Analisis = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.Analisis;
                    tbl_sol_solicitud_OficioDictamenJuridico.Observaciones = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.Observaciones;
                    tbl_sol_solicitud_OficioDictamenJuridico.OpinionJuridica = temp_tbl_Sol_Solicitud_OficioDictamenJuridico.OpinionJuridica;

                }

                tbl_sol_solicitud_OficioDictamenJuridico.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

                tbl_sol_solicitud_OficioDictamenJuridico.Dictamen = false;

                tbl_sol_solicitud_OficioDictamenJuridico.Oficio = true;

                tbl_sol_solicitud_OficioDictamenJuridico.Etapa_id = tbl_gest_EtapaSolicitud.Etapa_id;

                tbl_sol_solicitud_OficioDictamenJuridico.EtapaRuta_id = tbl_gest_EtapaSolicitud.EtapaRuta_id;

                tbl_sol_solicitud_OficioDictamenJuridico.CorrelativoEtapa_id = tbl_gest_EtapaSolicitud.CorrelativoEtapa_id;

                tbl_sol_solicitud_OficioDictamenJuridico.EtapaSolicitud_GUID_id = tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id;
            }


            return View(tbl_sol_solicitud_OficioDictamenJuridico);
        }



        public ActionResult CreacionDictamen(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();
            Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_solicitud_OficioDictamenJuridico;
            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            int cantidad = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id).Count();

            ViewBag.Guid_id = Guid_id;


            if (cantidad > 0)
            {
                tbl_sol_solicitud_OficioDictamenJuridico = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id).First();
            }
            else
            {

                tbl_sol_solicitud_OficioDictamenJuridico = new Tbl_Sol_Solicitud_OficioDictamenJuridico();

                tbl_sol_solicitud_OficioDictamenJuridico.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

                tbl_sol_solicitud_OficioDictamenJuridico.Dictamen = true;

                tbl_sol_solicitud_OficioDictamenJuridico.Oficio = false;

                tbl_sol_solicitud_OficioDictamenJuridico.Etapa_id = tbl_gest_EtapaSolicitud.Etapa_id;

                tbl_sol_solicitud_OficioDictamenJuridico.EtapaRuta_id = tbl_gest_EtapaSolicitud.EtapaRuta_id;

                tbl_sol_solicitud_OficioDictamenJuridico.CorrelativoEtapa_id = tbl_gest_EtapaSolicitud.CorrelativoEtapa_id;

                tbl_sol_solicitud_OficioDictamenJuridico.EtapaSolicitud_GUID_id = tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id;
            }


            return View(tbl_sol_solicitud_OficioDictamenJuridico);
        }



        [HttpPost]
        public JsonResult CambiarEstadoArchivoVerificadoNotario(Tbl_Sol_DocumentoSubido model)
        {
            long Solicitud_id;
            int Documento_id, Tipo_Documento_id;
            Solicitud_id = model.Solicitud_id;
            Documento_id = model.Documento_id;
            Tipo_Documento_id = model.Tipo_Documento_id;
            bool DocumentoVerificado;
            if (model.DocumentoVerificado == null)
            {
                DocumentoVerificado = true;
            }
            else
            {
                DocumentoVerificado = (bool)model.DocumentoVerificado;
            }
            string result;
            result = $"";

            /*  Tbl_Sol_DocumentoTipo.ValidacionJuridica   */


            try
            {
                Tbl_Sol_DocumentoSubido oTbl_Sol_DocumentoSubido = (from d in db.Tbl_Sol_DocumentoSubido
                                                                    where d.Solicitud_id == Solicitud_id && d.Documento_id == Documento_id && d.Tipo_Documento_id == Tipo_Documento_id
                                                                    select d).FirstOrDefault();

                oTbl_Sol_DocumentoSubido.DocumentoVerificadoNotario = DocumentoVerificado;
                result = $"Correcto";

                db.SaveChanges();
                return Json(result);
            }
            catch
            {
                return Json(result);
            }
        }



        public ActionResult IndexEspecificoGUID(int id, string GUID)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == GUID).First();


            long Id = tbl_sol_solicitud.Solicitud_id;

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == Id && Obj.Tipo_Documento_id == id);

            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }



        [HttpPost]
        public JsonResult GuardarOficio(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico, int generarDoc)
        {

            int codRespuesta = 0;
            string strRespuesta = "";

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



            tbl_sol_Solicitud_OficioDictamenJuridico.Dictamen = false;

            tbl_sol_Solicitud_OficioDictamenJuridico.Oficio = true;

            tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated = DateTime.Now;
            tbl_sol_Solicitud_OficioDictamenJuridico.swcreatedby = objUs.intUsuario_id;

            //  Opcion de ser un documento nuevo para ello verificamos dicha teoria.
            //  Grabar
            //  Crear PDF
            //  Firmar PDF
            int cantidadExistente = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id && Obj.Etapa_id == tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id && Obj.EtapaRuta_id == tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id && Obj.CorrelativoEtapa_id == tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id).Count();


            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_sol_Solicitud_OficioDictamenJuridico.EtapaSolicitud_GUID_id).First();


            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(1, tbl_Gest_EtapaSolicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);



            if (cantidadExistente == 0)
            {
                //  Opcion de ser un documento nuevo para ello verificamos dicha teoria.
                //  Grabar
                //  Crear PDF
                //  Firmar PDF

                db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Add(tbl_sol_Solicitud_OficioDictamenJuridico);
                db.SaveChanges();




                tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado = GenerarOficio_PDF(tbl_sol_Solicitud_OficioDictamenJuridico, "OFICIO");

                db.Entry(tbl_sol_Solicitud_OficioDictamenJuridico).State = EntityState.Modified;
                db.SaveChanges();

                //ProcesarFirmaElectronica(tbl_sol_Solicitud_OficioDictamenJuridico);

                strRespuesta = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;

            }
            else
            {
                try
                {
                    List<string> sqlQuery = db.Database.SqlQuery<List<string>>("delete from Tbl_Sol_Solicitud_OficioDictamenJuridico where Solicitud_id = @p0", tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                if (generarDoc != 0)
                {
                    tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado = GenerarOficio_PDF(tbl_sol_Solicitud_OficioDictamenJuridico, "OFICIO");

                }

                db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Add(tbl_sol_Solicitud_OficioDictamenJuridico);
                //db.Entry(tbl_sol_Solicitud_OficioDictamenJuridico).State = EntityState.Modified;
                db.SaveChanges();

                strRespuesta = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;

            }


            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            codRespuesta = 1;

            string jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }


        [HttpPost]
        public JsonResult GuardarDictamen(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico)
        {

            int codRespuesta = 0;
            string strRespuesta = "";

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


            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);

            //NOTA: Inicialmente el código registraba en este campo si era Dictamen u Oficio, bajo instrucción de Nancy se coloca que ahora dirá: Dictamen
            //Este dato venía de los JsonResult: GuardarDictamen y GuardarOficio
            //LlenaBanner($"Dictamen No.{tbl_sol_Solicitud.Solicitud_NumeroExpediente.Substring(0, 2)}-DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");

            //if (tbl_sol_Solicitud.Solicitud_NumeroExpediente == null)

            //{
            //    strRespuesta = "No se puede generar la emisión de dictamen jurídico ya que no se registro número de expediente.";
            //    string jsonResultUsr = "{\"CodRespuesta\":"
            //             + "\"" + codRespuesta + "\","
            //             + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            //    return Json(jsonResultUsr);
            //}



                tbl_sol_Solicitud_OficioDictamenJuridico.Dictamen = true;

            tbl_sol_Solicitud_OficioDictamenJuridico.Oficio = false;

            tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated = DateTime.Now;
            tbl_sol_Solicitud_OficioDictamenJuridico.swcreatedby = objUs.intUsuario_id;

            //  Opcion de ser un documento nuevo para ello verificamos dicha teoria.
            //  Grabar
            //  Crear PDF
            //  Firmar PDF
            int cantidadExistente = db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id && Obj.Etapa_id == tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id && Obj.EtapaRuta_id == tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id && Obj.CorrelativoEtapa_id == tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id).Count();

            if (cantidadExistente == 0)
            {
                //  Opcion de ser un documento nuevo para ello verificamos dicha teoria.
                //  Grabar
                //  Crear PDF
                //  Firmar PDF

                db.Tbl_Sol_Solicitud_OficioDictamenJuridico.Add(tbl_sol_Solicitud_OficioDictamenJuridico);
                db.SaveChanges();

                tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado = GenerarDictamen_PDF(tbl_sol_Solicitud_OficioDictamenJuridico, "Dictamen");

                db.Entry(tbl_sol_Solicitud_OficioDictamenJuridico).State = EntityState.Modified;
                db.SaveChanges();


                // ProcesarFirmaElectronica(tbl_sol_Solicitud_OficioDictamenJuridico);

                strRespuesta = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;


            }
            else
            {

                tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado = GenerarDictamen_PDF(tbl_sol_Solicitud_OficioDictamenJuridico, "Dictamen");

                db.Entry(tbl_sol_Solicitud_OficioDictamenJuridico).State = EntityState.Modified;
                db.SaveChanges();

                strRespuesta = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;


            }


            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == tbl_sol_Solicitud_OficioDictamenJuridico.EtapaSolicitud_GUID_id).First();

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(13, tbl_Gest_EtapaSolicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);



            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            codRespuesta = 1;

            string jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }


        private PdfPTable tableBanner = new PdfPTable(1);

        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
        }

        public String SQLDateH(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Day.ToString("D2") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Year.ToString("D4") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

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

        public void AgregarBitacora(long Form_Formulario_id, string firma, int estado, string strMsg)
        {
            //Tbl_Form_Formulario_FirmaElectronica_Bitacora tbl_Form_Formulario_FirmaElectronica_Bitacora = new Tbl_Form_Formulario_FirmaElectronica_Bitacora();

            //tbl_Form_Formulario_FirmaElectronica_Bitacora.Form_Formulario_id = Form_Formulario_id;
            //tbl_Form_Formulario_FirmaElectronica_Bitacora.Firmante = firma;
            //tbl_Form_Formulario_FirmaElectronica_Bitacora.FirmaElectronica_Estado_id = estado;
            //tbl_Form_Formulario_FirmaElectronica_Bitacora.swdatecreated = DateTime.Now;
            //tbl_Form_Formulario_FirmaElectronica_Bitacora.Resultado = strMsg;

            //db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.Add(tbl_Form_Formulario_FirmaElectronica_Bitacora);
            //db.SaveChanges();


        }


        class Personerias
        {
            public List<string> PropietariosIndividuales { get; set; }
            public List<string> PropietariosJuridicos { get; set; }
            public List<string> RepresentatnteLegal { get; set; }
            public List<string> Mandatario { get; set; }
            public List<string> ArrendatariosIndividuales { get; set; }
            public List<string> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias(long Solicitud_id)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<string>();
            personerias.PropietariosJuridicos = new List<string>();
            personerias.RepresentatnteLegal = new List<string>();
            personerias.Mandatario = new List<string>();
            personerias.ArrendatariosIndividuales = new List<string>();
            personerias.ArrendatariosJuridicos = new List<string>();
            List<fc_Sol_Sel_ListadoDePropietario_Result> PropietariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                     where d.Personeria_id == 1
                                                                                     select d).ToList();

            List<fc_Sol_Sel_ListadoDePropietario_Result> ProietariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                 where d.Personeria_id == 2
                                                                                 select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> RepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false)
                                                                                    select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> Mandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true)
                                                                            select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                       where d.Personeria_id == 1
                                                                                       select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                    where d.Personeria_id == 2
                                                                                    select d).ToList();

            if (PropietariosIndividuales.Count() > 0)
            {
                foreach (var item in PropietariosIndividuales)
                {
                    personerias.PropietariosIndividuales.Add(item.Nombre);
                }
            }

            if (ProietariosJuridicos.Count() > 0)
            {
                foreach (var item in ProietariosJuridicos)
                {
                    personerias.PropietariosJuridicos.Add(item.Nombre);
                }
            }

            if (RepresentanteLegal.Count() > 0)
            {
                foreach (var item in RepresentanteLegal)
                {
                    personerias.RepresentatnteLegal.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (Mandatario.Count() > 0)
            {
                foreach (var item in Mandatario)
                {
                    personerias.Mandatario.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (ArrendatariosIndividuales.Count() > 0)
            {
                foreach (var item in ArrendatariosIndividuales)
                {
                    personerias.ArrendatariosIndividuales.Add(item.Nombre);
                }
            }

            if (ArrendatariosJuridicos.Count() > 0)
            {
                foreach (var item in ArrendatariosJuridicos)
                {
                    personerias.ArrendatariosJuridicos.Add(item.Nombre);
                }
            }

            return personerias;
        }


        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        public string GenerarOficio_PDF(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico, string OficioDictamen)
        {


            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            string strNombre = OficioDictamen + fecha + ".pdf";
            string strDirArchivo = strFolder + strNombre;


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
            doc.Open();

            doc.Open();
            var Enter = new Paragraph(" ");
            doc.Add(Enter);
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }


            var tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id && Obj.Estado_id == true);


            CrearBanner crearBanner = new CrearBanner();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(1, tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id, Etapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id, EtapaRuta_id: tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id, CorrelativoEtapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
            //Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroResolucionSubRegional(Solicitud_id: tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id, Etapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id, EtapaRuta_id: tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id, CorrelativoEtapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
            string varTitulo = "";


            varTitulo = "DICTAMEN JURIDICO CON ENMIENDAS";




            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            var añoActual = DateTime.Now.Year;
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);

            //NOTA: Inicialmente el código registraba en este campo si era Dictamen u Oficio, bajo instrucción de Nancy se coloca que ahora dirá: Dictamen
            //Este dato venía de los JsonResult: GuardarDictamen y GuardarOficio
            LlenaBanner($"Dictamen No.{tbl_sol_Solicitud.Solicitud_NumeroExpediente.Substring(0, 3).Replace("-", "").Replace(".", "")}-DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");
            

            //LlenaBanner($"{OficioDictamen} No.{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}/{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}", "Derecha", "Blanco");
            //LlenaBanner($"{OficioDictamen} No.{No_OficioJuridico}", "Derecha", "Blanco");
            doc.Add(tableBanner);




            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now) + "')").FirstOrDefault();

            LlenaBanner(strFecha, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            //Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);



            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;

            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

            //Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
                                                     where d.Region_id == tbl_sol_Solicitud.Region_id
                                                        && d.SubRegion_id == tbl_sol_Solicitud.SubRegion_id
                                                     select d).FirstOrDefault();






            var DireccionNotificacion = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Usuario").ToList();

            string strDireccionNotificacion = "";

            if (DireccionNotificacion.Count() > 0)
            {
                strDireccionNotificacion = "";

                if ((DireccionNotificacion[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionNotificacion += " " + DireccionNotificacion[0].Direccion;
                }
                if ((DireccionNotificacion[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", aldea " + DireccionNotificacion[0].Aldea;
                }
                if ((DireccionNotificacion[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", Departamento de " + DireccionNotificacion[0].Departamento;
                }
                if ((DireccionNotificacion[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", Municipio de " + DireccionNotificacion[0].Municipio;
                }

            }


            var DireccionFinca = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Finca").ToList();
            string strDireccionFinca = "";

            if (DireccionFinca.Count() > 0)
            {
                strDireccionFinca = "";


                if ((DireccionFinca[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionFinca += " " + DireccionFinca[0].Direccion;
                }
                if ((DireccionFinca[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionFinca += ", aldea " + DireccionFinca[0].Aldea;
                }
                if ((DireccionFinca[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionFinca += ", Municipio de " + DireccionFinca[0].Municipio;
                }
                if ((DireccionFinca[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionFinca += ", Departamento de " + DireccionFinca[0].Departamento;
                }

            }

            var DireccionEmpresa = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa").ToList();

            string strDireccionEmpresa = "";

            if (DireccionEmpresa.Count() > 0)
            {
                strDireccionEmpresa = "";


                if ((DireccionEmpresa[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionEmpresa += " " + DireccionEmpresa[0].Direccion;
                }
                if ((DireccionEmpresa[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", aldea " + DireccionEmpresa[0].Aldea;
                }
                if ((DireccionEmpresa[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", Municipio de " + DireccionEmpresa[0].Municipio;
                }
                if ((DireccionEmpresa[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", Departamento de " + DireccionEmpresa[0].Departamento;
                }

            }

            var DireccionEmpresaMovil = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa Movil").ToList();

            string strDireccionEmpresaMovil = "";

            if (DireccionEmpresaMovil.Count() > 0)
            {
                strDireccionEmpresaMovil = "";


                if ((DireccionEmpresaMovil[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += " " + DireccionEmpresaMovil[0].Direccion;
                }
                if ((DireccionEmpresaMovil[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", aldea " + DireccionEmpresaMovil[0].Aldea;
                }
                if ((DireccionEmpresaMovil[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", Municipio de " + DireccionEmpresaMovil[0].Municipio;
                }
                if ((DireccionEmpresaMovil[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", Departamento de " + DireccionEmpresaMovil[0].Departamento;
                }

            }



            string strDireccionMovil = ".";

            if (tbl_sol_Solicitud.Categoria_id == 5)
            {
                if (DireccionNotificacion.Count() > 1)
                {
                    strDireccionMovil = "";

                    if ((DireccionNotificacion[1].Direccion ?? "").Trim() != "")
                    {
                        strDireccionMovil += " para la industria movil ubicada en " + DireccionNotificacion[1].Direccion;
                    }
                    if ((DireccionNotificacion[1].Aldea ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", aldea " + DireccionNotificacion[1].Aldea;
                    }
                    if ((DireccionNotificacion[1].Municipio ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", Municipio de " + DireccionNotificacion[1].Municipio;
                    }
                    if ((DireccionNotificacion[1].Departamento ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", Departamento de " + DireccionNotificacion[1].Departamento;
                    }
                    strDireccionMovil += ".";
                }
            }

            doc.Add(Enter);
            doc.Add(Enter);

            LlenaBanner(DatosSubRegional.NombreCompleto, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Director Subregional " + tbl_Gral_SubRegion.No_SubRegion + ", " + tbl_Gral_SubRegion.Nombre_SubRegion, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("Instituto Nacional de Bosques -INAB-", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            Personerias personerias = ObtenerPersonerias(tbl_sol_Solicitud.Solicitud_id);
            string datoreemplazar = "";
            List<string> datolistreemplazar = new List<string>();
            datolistreemplazar = personerias.PropietariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " solicitado por: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.PropietariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " solicitado por: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.RepresentatnteLegal;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Representante Legal: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.Mandatario;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Mandatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.ArrendatariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Arrendatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.ArrendatariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Arrendatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }

            Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);


            Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);

            Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();
            string ParrafoNo_1 = "";

            Tbl_Sol_Solicitud_Sub_Sub_Categoria tbl_Sol_Solicitud_Sub_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_sol_Solicitud.Sub_Sub_Categoria_id).FirstOrDefault();

            decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);
            string strTipoSolicitud;

            if (TipoGestion == (decimal)0.00)
            {
                strTipoSolicitud = "inscripción";
            }
            else
            {
                strTipoSolicitud = "actualización";
            }

            string strAdendumRegistro = "";

            if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
            {
                strAdendumRegistro = " con No. de Registro : " + tbl_sol_Solicitud.No_Registro + ";";
            }

            if (strDireccionEmpresa == "")
            {
                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante quien se ubica en @DireccionSolicitante.";
            }

            if (strDireccionEmpresa != "")
            {
                Tbl_Sol_Empresa_Entidad Tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).First();

                if (tbl_sol_Solicitud.Categoria_id == 8)
                {
                    ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la comercializadora de motosierras " + Tbl_Sol_Empresa_Entidad.Nombre + " ubicada en " + strDireccionEmpresa;
                }
                else
                {
                    if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
                    {
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para el " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicado en " + strDireccionEmpresa;
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
                    }
                    else
                    {
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicada en " + strDireccionEmpresa;
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
                    }
                }

                if ((tbl_sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tipo_Industria_id == 2) || (tbl_sol_Solicitud.SolicitudTipo_id >= 5 && tbl_sol_Solicitud.SolicitudTipo_id <= 6 && tbl_sol_Solicitud.Sub_Categoria_id == 3) && (strDireccionEmpresaMovil != ""))
                {
                    if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
                    {
                        ParrafoNo_1 += ", cuya dirección de funcionamiento es " + strDireccionEmpresaMovil;
                    }
                    else
                    {
                        ParrafoNo_1 += ", cuya dirección de trabajo para la Industria Forestal Movil es " + strDireccionEmpresaMovil;
                    }
                }

                ParrafoNo_1 += ".";
            }

            if (strDireccionFinca != "")
            {
                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionFinca;
            }

            Tbl_RNF_Registro tbl_RNF_Registro;

            tbl_RNF_Registro = db.Tbl_RNF_Registro.FirstOrDefault(r => r.No_Registro == tbl_sol_Solicitud.No_Registro);


            if (tbl_sol_Solicitud.Solicitud_NumeroExpediente!=null)
            {
            ParrafoNo_1 = ParrafoNo_1.Replace("@NoExpediente", tbl_sol_Solicitud.Solicitud_NumeroExpediente);

            }
            else if (tbl_RNF_Registro.Expediente!=null)
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@NoExpediente", tbl_RNF_Registro.Expediente);
            }


            if ((tbl_sol_Solicitud_Categoria.Categoria_id == 6) && (tbl_Sol_Solicitud_Sub_Sub_Categoria != null))
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría de " + tbl_Sol_Solicitud_Sub_Sub_Categoria.Descripcion);
            }
            else
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion);
            }
            //ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", tbl_seg_usuarioExterno.Nombres + " " + tbl_seg_usuarioExterno.Apellidos);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", datoreemplazar);

            ParrafoNo_1 = ParrafoNo_1.Replace("@DireccionSolicitante", strDireccionNotificacion);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Departamento", DireccionNotificacion[0].Departamento);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Municipio", DireccionNotificacion[0].Municipio);



            LlenaBanner(ParrafoNo_1, "Justificado", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Después de realizar el análisis de los documentos legales para este tipo de Registro y de la lectura del expediente sometido a la consideración de esta Delegación Jurídica establece lo siguiente:", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            string[] separador = { "\n" };

            if ((tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes.Split(separador, StringSplitOptions.None);
                LlenaBanner("ANTECEDENTES", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);


                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }

            if ((tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal.Split(separador, StringSplitOptions.None);
                LlenaBanner("FUNDAMENTO LEGAL", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);


                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }


            if ((tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.Analisis.Split(separador, StringSplitOptions.None);

                LlenaBanner("ANÁLISIS", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);


                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }

            if ((tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica.Split(separador, StringSplitOptions.None);

                LlenaBanner("OPINIÓN JURÍDICA", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);


                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }



            if (tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Count() > 0)
            {

                LlenaBanner("Previo a continuar con el trámite administrativo del expediente el solicitante debe presentar lo siguiente:", "Izquierda", "Blanco");
                doc.Add(tableBanner);

                doc.Add(Enter);
            }

            int intcontador = 0;

            foreach (var Item in tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda)
            {
                intcontador = intcontador + 1;

                LlenaBanner(intcontador.ToString() + ". " + Item.Descripcion, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }


            if (tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Count() > 0)
            {

                LlenaBanner("Por lo anterior, solicito a su persona, requerir la información al solicitante. Para continuar con el trámite del expediente administrativo.", "Izquierda", "Blanco");
                doc.Add(tableBanner);

                doc.Add(Enter);
            }

            //LlenaBanner("Atentamente", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            //doc.Add(Enter);


            LlenaBanner(objUs.strNombre_Usuario, "Centro", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Delegada(o) Jurídico(a)", "Centro", "Blanco");
            doc.Add(tableBanner);

            //LlenaBanner("cc Archivo", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            //LlenaBanner("// Expediente", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            DateTime Fecha = tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now;

            string Fch_String = Fecha.Day.ToString("D2") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Year.ToString("D4") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            LlenaBanner("Fecha y hora " + Fch_String, "Izquierda", "Blanco");
            doc.Add(tableBanner);


            doc.Close();
            writer.Close();

            return strNombre;
        }

        public string GenerarDictamen_PDF(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico, string OficioDictamen)
        {


            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            string strNombre = OficioDictamen + fecha + ".pdf";
            string strDirArchivo = strFolder + strNombre;


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
            doc.Open();

            doc.Open();
            var Enter = new Paragraph(" ");
            doc.Add(Enter);
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }


            var tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id && Obj.Estado_id == true);


            CrearBanner crearBanner = new CrearBanner();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            //Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroResolucionSubRegional(Solicitud_id: tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id, Etapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id, EtapaRuta_id: tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id, CorrelativoEtapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);

            //Cristian Rejo

            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(13, tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id, Etapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id, EtapaRuta_id: tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id, CorrelativoEtapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
            string varTitulo = "";


            varTitulo = "DICTAMEN JURIDICO";




            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            var añoActual = DateTime.Now.Year;
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);

            Tbl_RNF_Registro tbl_RNF_Registro;

            if (tbl_sol_Solicitud.No_Registro != null)
            {

                tbl_RNF_Registro = db.Tbl_RNF_Registro.FirstOrDefault(r => r.No_Registro == tbl_sol_Solicitud.No_Registro);
            }
            else
            {

            tbl_RNF_Registro= db.Tbl_RNF_Registro.FirstOrDefault(r => r.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);
            }

            //NOTA: Inicialmente el código registraba en este campo si era Dictamen u Oficio, bajo instrucción de Nancy se coloca que ahora dirá: Dictamen
            //Este dato venía de los JsonResult: GuardarDictamen y GuardarOficio
            //LlenaBanner($"Dictamen No.{tbl_sol_Solicitud.Solicitud_NumeroExpediente.Substring(0, 2)}-DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");

            if (tbl_sol_Solicitud.Solicitud_NumeroExpediente!= null)
            {

            LlenaBanner($"Dictamen No. {tbl_sol_Solicitud.Solicitud_NumeroExpediente.Substring(0, 3).Replace("-", "").Replace(".", "")}-DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");
            }
            else if (tbl_RNF_Registro.Expediente != null)
            {
                LlenaBanner($"Dictamen No. {tbl_RNF_Registro.Expediente.Substring(0, 3).Replace("-", "").Replace(".", "")}-DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");
            }
            else
            {
            LlenaBanner($"Dictamen No. DJ-{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}-{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}-{añoActual}", "Derecha", "Blanco");

            }


            //LlenaBanner($"{OficioDictamen} No.{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}/{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}", "Derecha", "Blanco");
            //LlenaBanner($"{OficioDictamen} No.{No_OficioJuridico}", "Derecha", "Blanco");
            doc.Add(tableBanner);




            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now) + "')").FirstOrDefault();

            LlenaBanner(strFecha, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            //Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);



            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;

            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

            //Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
                                                     where d.Region_id == tbl_sol_Solicitud.Region_id
                                                        && d.SubRegion_id == tbl_sol_Solicitud.SubRegion_id
                                                     select d).FirstOrDefault();






            var DireccionNotificacion = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Usuario").ToList();

            string strDireccionNotificacion = "";

            if (DireccionNotificacion.Count() > 0)
            {
                strDireccionNotificacion = "";

                if ((DireccionNotificacion[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionNotificacion += " " + DireccionNotificacion[0].Direccion;
                }
                if ((DireccionNotificacion[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", aldea " + DireccionNotificacion[0].Aldea;
                }
                if ((DireccionNotificacion[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", Departamento de " + DireccionNotificacion[0].Departamento;
                }
                if ((DireccionNotificacion[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionNotificacion += ", Municipio de " + DireccionNotificacion[0].Municipio;
                }

            }


            var DireccionFinca = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Finca").ToList();
            string strDireccionFinca = "";

            if (DireccionFinca.Count() > 0)
            {
                strDireccionFinca = "";


                if ((DireccionFinca[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionFinca += " " + DireccionFinca[0].Direccion;
                }
                if ((DireccionFinca[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionFinca += ", aldea " + DireccionFinca[0].Aldea;
                }
                if ((DireccionFinca[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionFinca += ", Municipio de " + DireccionFinca[0].Municipio;
                }
                if ((DireccionFinca[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionFinca += ", Departamento de " + DireccionFinca[0].Departamento;
                }

            }

            var DireccionEmpresa = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa").ToList();

            string strDireccionEmpresa = "";

            if (DireccionEmpresa.Count() > 0)
            {
                strDireccionEmpresa = "";


                if ((DireccionEmpresa[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionEmpresa += " " + DireccionEmpresa[0].Direccion;
                }
                if ((DireccionEmpresa[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", aldea " + DireccionEmpresa[0].Aldea;
                }
                if ((DireccionEmpresa[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", Municipio de " + DireccionEmpresa[0].Municipio;
                }
                if ((DireccionEmpresa[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionEmpresa += ", Departamento de " + DireccionEmpresa[0].Departamento;
                }

            }

            var DireccionEmpresaMovil = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa Movil").ToList();

            string strDireccionEmpresaMovil = "";

            if (DireccionEmpresaMovil.Count() > 0)
            {
                strDireccionEmpresaMovil = "";


                if ((DireccionEmpresaMovil[0].Direccion ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += " " + DireccionEmpresaMovil[0].Direccion;
                }
                if ((DireccionEmpresaMovil[0].Aldea ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", aldea " + DireccionEmpresaMovil[0].Aldea;
                }
                if ((DireccionEmpresaMovil[0].Municipio ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", Municipio de " + DireccionEmpresaMovil[0].Municipio;
                }
                if ((DireccionEmpresaMovil[0].Departamento ?? "").Trim() != "")
                {
                    strDireccionEmpresaMovil += ", Departamento de " + DireccionEmpresaMovil[0].Departamento;
                }

            }



            string strDireccionMovil = ".";

            if (tbl_sol_Solicitud.Categoria_id == 5)
            {
                if (DireccionNotificacion.Count() > 1)
                {
                    strDireccionMovil = "";

                    if ((DireccionNotificacion[1].Direccion ?? "").Trim() != "")
                    {
                        strDireccionMovil += " para la industria movil ubicada en " + DireccionNotificacion[1].Direccion;
                    }
                    if ((DireccionNotificacion[1].Aldea ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", aldea " + DireccionNotificacion[1].Aldea;
                    }
                    if ((DireccionNotificacion[1].Municipio ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", Municipio de " + DireccionNotificacion[1].Municipio;
                    }
                    if ((DireccionNotificacion[1].Departamento ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", Departamento de " + DireccionNotificacion[1].Departamento;
                    }
                    strDireccionMovil += ".";
                }
            }

            doc.Add(Enter);
            doc.Add(Enter);

            LlenaBanner(DatosSubRegional.NombreCompleto, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Director Subregional " + tbl_Gral_SubRegion.No_SubRegion + ", " + tbl_Gral_SubRegion.Nombre_SubRegion, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("Instituto Nacional de Bosques -INAB-", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            Personerias personerias = ObtenerPersonerias(tbl_sol_Solicitud.Solicitud_id);
            string datoreemplazar = "";
            List<string> datolistreemplazar = new List<string>();
            datolistreemplazar = personerias.PropietariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " solicitado por: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.PropietariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " solicitado por: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.RepresentatnteLegal;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Representante Legal: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.Mandatario;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Mandatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.ArrendatariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Arrendatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }
            datolistreemplazar = personerias.ArrendatariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                datoreemplazar += " a través de Arrendatario: ";
                for (int i = 0; i < datolistreemplazar.Count(); i++)
                {
                    datoreemplazar += datolistreemplazar[i];
                    if (i != datolistreemplazar.Count() - 1)
                    {
                        if (i == datolistreemplazar.Count() - 2)
                        {
                            datoreemplazar += " y ";
                        }
                        else
                        {
                            datoreemplazar += ", ";
                        }
                    }
                }
            }

            Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);


            Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);

            Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();
            string ParrafoNo_1 = "";

            Tbl_Sol_Solicitud_Sub_Sub_Categoria tbl_Sol_Solicitud_Sub_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_sol_Solicitud.Sub_Sub_Categoria_id).FirstOrDefault();

            decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);
            string strTipoSolicitud;

            if (TipoGestion == (decimal)0.00)
            {
                strTipoSolicitud = "inscripción";
            }
            else
            {
                strTipoSolicitud = "actualización";
            }

            string strAdendumRegistro = "";

            if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
            {
                strAdendumRegistro = " con No. de Registro : " + tbl_sol_Solicitud.No_Registro + ";";
            }

            if (strDireccionEmpresa == "")
            {
                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante quien se ubica en @DireccionSolicitante.";
            }

            if (strDireccionEmpresa != "")
            {
                Tbl_Sol_Empresa_Entidad Tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).First();

                if (tbl_sol_Solicitud.Categoria_id == 8)
                {
                    ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la comercializadora de motosierras " + Tbl_Sol_Empresa_Entidad.Nombre + " ubicada en " + strDireccionEmpresa;
                }
                else
                {
                    if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
                    {
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para el " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicado en " + strDireccionEmpresa;
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
                    }
                    else
                    {
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicada en " + strDireccionEmpresa;
                        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
                    }
                }

                if ((tbl_sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tipo_Industria_id == 2) || (tbl_sol_Solicitud.SolicitudTipo_id >= 5 && tbl_sol_Solicitud.SolicitudTipo_id <= 6 && tbl_sol_Solicitud.Sub_Categoria_id == 3) && (strDireccionEmpresaMovil != ""))
                {
                    if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
                    {
                        ParrafoNo_1 += ", cuya dirección de funcionamiento es " + strDireccionEmpresaMovil;
                    }
                    else
                    {
                        ParrafoNo_1 += ", cuya dirección de trabajo para la Industria Forestal Movil es " + strDireccionEmpresaMovil;
                    }
                }

                ParrafoNo_1 += ".";
            }

            if (strDireccionFinca != "")
            {
                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionFinca;
            }

            if (tbl_sol_Solicitud.Solicitud_NumeroExpediente != null)
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@NoExpediente", tbl_sol_Solicitud.Solicitud_NumeroExpediente);

            }
            else if (tbl_RNF_Registro.Expediente != null)
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@NoExpediente", tbl_RNF_Registro.Expediente);
            }


            if ((tbl_sol_Solicitud_Categoria.Categoria_id == 6) && (tbl_Sol_Solicitud_Sub_Sub_Categoria != null))
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría de " + tbl_Sol_Solicitud_Sub_Sub_Categoria.Descripcion);
            }
            else
            {
                ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion);
            }
            //ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", tbl_seg_usuarioExterno.Nombres + " " + tbl_seg_usuarioExterno.Apellidos);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", datoreemplazar);

            ParrafoNo_1 = ParrafoNo_1.Replace("@DireccionSolicitante", strDireccionNotificacion);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Departamento", DireccionNotificacion[0].Departamento);

            ParrafoNo_1 = ParrafoNo_1.Replace("@Municipio", DireccionNotificacion[0].Municipio);



            LlenaBanner(ParrafoNo_1, "Justificado", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Después de realizar el análisis de los documentos legales para este tipo de Registro y de la lectura del expediente sometido a la consideración de esta Delegación Jurídica establece lo siguiente:", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            string[] separador = { "\n" };


            if ((tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes.Split(separador, StringSplitOptions.None);
                LlenaBanner("ANTECEDENTES", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }

            if ((tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal.Split(separador, StringSplitOptions.None);
                LlenaBanner("FUNDAMENTO LEGAL", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }


            if ((tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != ""))
            {
                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.Analisis.Split(separador, StringSplitOptions.None);

                LlenaBanner("ANÁLISIS", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);
            }

            if ((tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != ""))
            {

                string[] coord = tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica.Split(separador, StringSplitOptions.None);
                LlenaBanner("OPINIÓN JURÍDICA", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                foreach (var item in coord)
                {
                    if (item.Trim() == "")
                    {
                        doc.Add(Enter);
                    }
                    LlenaBanner(item, "Justificado", "Blanco");
                    doc.Add(tableBanner);
                }
                doc.Add(Enter);

            }





            LlenaBanner("Envio el presente documento para continuar con el trámite del expediente administrativo.", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            //LlenaBanner("Atentamente", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            //doc.Add(Enter);


            LlenaBanner(objUs.strNombre_Usuario, "Centro", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Delegada(o) Jurídico(a)", "Centro", "Blanco");
            doc.Add(tableBanner);

            //LlenaBanner("cc Archivo", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            //LlenaBanner("// Expediente", "Izquierda", "Blanco");
            //doc.Add(tableBanner);

            DateTime Fecha = tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now;

            string Fch_String = Fecha.Day.ToString("D2") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Year.ToString("D4") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            LlenaBanner("Fecha y hora " + Fch_String, "Izquierda", "Blanco");
            doc.Add(tableBanner);


            doc.Close();
            writer.Close();

            return strNombre;
        }



        //public string GenerarOficioDictame_PDF(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_sol_Solicitud_OficioDictamenJuridico, string OficioDictamen)
        //{


        //    string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
        //    string strFolder = Server.MapPath("~/") + strDir;
        //    DateTime hoy = DateTime.Now;
        //    string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

        //    string strNombre = OficioDictamen + fecha + ".pdf";
        //    string strDirArchivo = strFolder + strNombre;


        //    Document doc = new Document(PageSize.LETTER);
        //    doc.SetMargins(1f, 1f, 25f, 50f);

        //    Usuario objUs = new Usuario();
        //    objUs.intUsuario_id = 0;

        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];

        //    }




        //    strDirArchivo = strFolder + strNombre;

        //    if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);


        //    FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
        //    PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
        //    doc.Open();

        //    doc.Open();
        //    var Enter = new Paragraph(" ");
        //    doc.Add(Enter);
        //    doc.Add(Enter);
        //    if (Constants.VisualizarInformacionDesarrollo == 1)
        //    {
        //        string urlact = this.Url.Action();
        //        LlenaBanner(urlact);
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }


        //    var tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id && Obj.Estado_id == true);


        //    CrearBanner crearBanner = new CrearBanner();
        //    IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
        //    Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroResolucionSubRegional(Solicitud_id: tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id, Etapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.Etapa_id, EtapaRuta_id: tbl_sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id, CorrelativoEtapa_id: tbl_sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
        //    string varTitulo = "";

        //    if (tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Count() > 0)
        //    {
        //        varTitulo = "DICTAMEN JURIDICO CON ENMIENDAS";
        //    }
        //    else
        //    {
        //        varTitulo = "DICTAMEN JURIDICO";
        //    }





        //    crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
        //    doc.Add(crearBanner.tableTitulo);
        //    doc.Add(Enter);


        //    //NOTA: Inicialmente el código registraba en este campo si era Dictamen u Oficio, bajo instrucción de Nancy se coloca que ahora dirá: Dictamen
        //    //Este dato venía de los JsonResult: GuardarDictamen y GuardarOficio
        //    LlenaBanner($"Dictamen No.{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}/{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}", "Derecha", "Blanco");

        //    //LlenaBanner($"{OficioDictamen} No.{tbl_sol_Solicitud_OficioDictamenJuridico.Numero}/{tbl_sol_Solicitud_OficioDictamenJuridico.InicialesDelJuridico}", "Derecha", "Blanco");
        //    //LlenaBanner($"{OficioDictamen} No.{No_OficioJuridico}", "Derecha", "Blanco");
        //    doc.Add(tableBanner);




        //    string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now) + "')").FirstOrDefault();

        //    LlenaBanner(strFecha, "Izquierda", "Blanco");
        //    doc.Add(tableBanner);

        //    Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Solicitud_OficioDictamenJuridico.Solicitud_id);



        //    int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;

        //    fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

        //    //Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
        //    Tbl_Gral_SubRegion tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
        //                                             where d.Region_id == tbl_sol_Solicitud.Region_id
        //                                                && d.SubRegion_id == tbl_sol_Solicitud.SubRegion_id
        //                                             select d).FirstOrDefault();






        //    var DireccionNotificacion = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Usuario").ToList();

        //    string strDireccionNotificacion = "";

        //    if (DireccionNotificacion.Count() > 0)
        //    {
        //        strDireccionNotificacion = "";

        //        if (DireccionNotificacion[0].Direccion.Trim() != "")
        //        {
        //            strDireccionNotificacion += " " + DireccionNotificacion[0].Direccion;
        //        }
        //        if (DireccionNotificacion[0].Aldea.Trim() != "")
        //        {
        //            strDireccionNotificacion += ", aldea " + DireccionNotificacion[0].Aldea;
        //        }
        //        if (DireccionNotificacion[0].Departamento.Trim() != "")
        //        {
        //            strDireccionNotificacion += ", Departamento de " + DireccionNotificacion[0].Departamento;
        //        }
        //        if (DireccionNotificacion[0].Municipio.Trim() != "")
        //        {
        //            strDireccionNotificacion += ", Municipio de " + DireccionNotificacion[0].Municipio;
        //        }

        //    }


        //    var DireccionFinca = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Finca").ToList();
        //    string strDireccionFinca = "";

        //    if (DireccionFinca.Count() > 0)
        //    {
        //        strDireccionFinca = "";


        //        if (DireccionFinca[0].Direccion.Trim() != "")
        //        {
        //            strDireccionFinca += " " + DireccionFinca[0].Direccion;
        //        }
        //        if (DireccionFinca[0].Aldea.Trim() != "")
        //        {
        //            strDireccionFinca += ", aldea " + DireccionFinca[0].Aldea;
        //        }
        //        if (DireccionFinca[0].Municipio.Trim() != "")
        //        {
        //            strDireccionFinca += ", Municipio de " + DireccionFinca[0].Municipio;
        //        }
        //        if (DireccionFinca[0].Departamento.Trim() != "")
        //        {
        //            strDireccionFinca += ", Departamento de " + DireccionFinca[0].Departamento;
        //        }

        //    }

        //    var DireccionEmpresa = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa").ToList();

        //    string strDireccionEmpresa = "";

        //    if (DireccionEmpresa.Count() > 0)
        //    {
        //        strDireccionEmpresa = "";


        //        if (DireccionEmpresa[0].Direccion.Trim() != "")
        //        {
        //            strDireccionEmpresa += " " + DireccionEmpresa[0].Direccion;
        //        }
        //        if (DireccionEmpresa[0].Aldea.Trim() != "")
        //        {
        //            strDireccionEmpresa += ", aldea " + DireccionEmpresa[0].Aldea;
        //        }
        //        if (DireccionEmpresa[0].Municipio.Trim() != "")
        //        {
        //            strDireccionEmpresa += ", Municipio de " + DireccionEmpresa[0].Municipio;
        //        }
        //        if (DireccionEmpresa[0].Departamento.Trim() != "")
        //        {
        //            strDireccionEmpresa += ", Departamento de " + DireccionEmpresa[0].Departamento;
        //        }

        //    }

        //    var DireccionEmpresaMovil = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).Where(Obj => Obj.TipoDireccion == "Empresa Movil").ToList();

        //    string strDireccionEmpresaMovil = "";

        //    if (DireccionEmpresaMovil.Count() > 0)
        //    {
        //        strDireccionEmpresaMovil = "";


        //        if (DireccionEmpresaMovil[0].Direccion.Trim() != "")
        //        {
        //            strDireccionEmpresaMovil += " " + DireccionEmpresaMovil[0].Direccion;
        //        }
        //        if (DireccionEmpresaMovil[0].Aldea.Trim() != "")
        //        {
        //            strDireccionEmpresaMovil += ", aldea " + DireccionEmpresaMovil[0].Aldea;
        //        }
        //        if (DireccionEmpresaMovil[0].Municipio.Trim() != "")
        //        {
        //            strDireccionEmpresaMovil += ", Municipio de " + DireccionEmpresaMovil[0].Municipio;
        //        }
        //        if (DireccionEmpresaMovil[0].Departamento.Trim() != "")
        //        {
        //            strDireccionEmpresaMovil += ", Departamento de " + DireccionEmpresaMovil[0].Departamento;
        //        }

        //    }



        //    string strDireccionMovil = ".";

        //    if (tbl_sol_Solicitud.Categoria_id == 5)
        //    {
        //        if (DireccionNotificacion.Count() > 1)
        //        {
        //            strDireccionMovil = "";

        //            if (DireccionNotificacion[1].Direccion.Trim() != "")
        //            {
        //                strDireccionMovil += " para la industria movil ubicada en " + DireccionNotificacion[1].Direccion;
        //            }
        //            if (DireccionNotificacion[1].Aldea.Trim() != "")
        //            {
        //                strDireccionMovil += ", aldea " + DireccionNotificacion[1].Aldea;
        //            }
        //            if (DireccionNotificacion[1].Municipio.Trim() != "")
        //            {
        //                strDireccionMovil += ", Municipio de " + DireccionNotificacion[1].Municipio;
        //            }
        //            if (DireccionNotificacion[1].Departamento.Trim() != "")
        //            {
        //                strDireccionMovil += ", Departamento de " + DireccionNotificacion[1].Departamento;
        //            }
        //            strDireccionMovil += ".";
        //        }
        //    }

        //    doc.Add(Enter);
        //    doc.Add(Enter);

        //    LlenaBanner(DatosSubRegional.NombreCompleto, "Izquierda", "Blanco");
        //    doc.Add(tableBanner);

        //    LlenaBanner("Director Subregional " + tbl_Gral_SubRegion.No_SubRegion + ", " + tbl_Gral_SubRegion.Nombre_SubRegion, "Izquierda", "Blanco");
        //    doc.Add(tableBanner);
        //    LlenaBanner("Instituto Nacional de Bosques -INAB-", "Izquierda", "Blanco");
        //    doc.Add(tableBanner);

        //    doc.Add(Enter);

        //    Personerias personerias = ObtenerPersonerias(tbl_sol_Solicitud.Solicitud_id);
        //    string datoreemplazar = "";
        //    List<string> datolistreemplazar = new List<string>();
        //    datolistreemplazar = personerias.PropietariosIndividuales;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " solicitado por: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }
        //    datolistreemplazar = personerias.PropietariosJuridicos;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " solicitado por: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }
        //    datolistreemplazar = personerias.RepresentatnteLegal;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " a través de Representante Legal: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }
        //    datolistreemplazar = personerias.Mandatario;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " a través de Mandatario: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }
        //    datolistreemplazar = personerias.ArrendatariosIndividuales;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " a través de Arrendatario: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }
        //    datolistreemplazar = personerias.ArrendatariosJuridicos;
        //    if (datolistreemplazar.Count() > 0)
        //    {
        //        datoreemplazar += " a través de Arrendatario: ";
        //        for (int i = 0; i < datolistreemplazar.Count(); i++)
        //        {
        //            datoreemplazar += datolistreemplazar[i];
        //            if (i != datolistreemplazar.Count() - 1)
        //            {
        //                if (i == datolistreemplazar.Count() - 2)
        //                {
        //                    datoreemplazar += " y ";
        //                }
        //                else
        //                {
        //                    datoreemplazar += ", ";
        //                }
        //            }
        //        }
        //    }

        //    Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);


        //    Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);

        //    Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();
        //    string ParrafoNo_1 = "";

        //    Tbl_Sol_Solicitud_Sub_Sub_Categoria tbl_Sol_Solicitud_Sub_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_sol_Solicitud.Sub_Sub_Categoria_id).FirstOrDefault();

        //    decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);
        //    string strTipoSolicitud;

        //    if (TipoGestion == (decimal)0.00)
        //    {
        //        strTipoSolicitud = "inscripción";
        //    }
        //    else
        //    {
        //        strTipoSolicitud = "actualización";
        //    }

        //    string strAdendumRegistro = "";

        //    if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
        //    {
        //        strAdendumRegistro = " con No. de Registro : " + tbl_sol_Solicitud.No_Registro + ";";
        //    }

        //    if (strDireccionEmpresa == "")
        //    {
        //        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante quien se ubica en @DireccionSolicitante.";
        //    }

        //    if (strDireccionEmpresa != "")
        //    {
        //        Tbl_Sol_Empresa_Entidad Tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).First();

        //        if (tbl_sol_Solicitud.Categoria_id == 8)
        //        {
        //            ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la comercializadora de motosierras " + Tbl_Sol_Empresa_Entidad.Nombre + " ubicada en " + strDireccionEmpresa;
        //        }
        //        else
        //        {
        //            if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
        //            {
        //                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para el " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicado en " + strDireccionEmpresa;
        //                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
        //            }
        //            else
        //            {
        //                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, para la " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion + " ubicada en " + strDireccionEmpresa;
        //                ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionEmpresa;
        //            }
        //        }

        //        if (strDireccionEmpresaMovil != "")
        //        {
        //            if (tbl_sol_Solicitud.Sub_Categoria_id == 3)
        //            {
        //                ParrafoNo_1 += ", cuya dirección de funcionamiento es " + strDireccionEmpresaMovil;
        //            }
        //            else
        //            {
        //                ParrafoNo_1 += ", cuya dirección de trabajo para la Industria Forestal Movil es " + strDireccionEmpresaMovil;
        //            }
        //        }

        //        ParrafoNo_1 += ".";
        //    }

        //    if (strDireccionFinca != "")
        //    {
        //        ParrafoNo_1 = "Por este medio informo que se realizó un análisis de los documentos legales presentados en el expediente @NoExpediente " + strAdendumRegistro + " según solicitud de " + strTipoSolicitud + " en el Registro Nacional Forestal, en la categoría de @Categoria, @Solicitante, ubicado en " + strDireccionFinca;
        //    }

        //    ParrafoNo_1 = ParrafoNo_1.Replace("@NoExpediente", tbl_sol_Solicitud.Solicitud_NumeroExpediente);

        //    if ((tbl_sol_Solicitud_Categoria.Categoria_id == 6) && (tbl_Sol_Solicitud_Sub_Sub_Categoria != null))
        //    {
        //        ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría de " + tbl_Sol_Solicitud_Sub_Sub_Categoria.Descripcion);
        //    }
        //    else
        //    {
        //        ParrafoNo_1 = ParrafoNo_1.Replace("@Categoria", tbl_sol_Solicitud_Categoria.Descripcion + " subcategoría " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion);
        //    }
        //    //ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", tbl_seg_usuarioExterno.Nombres + " " + tbl_seg_usuarioExterno.Apellidos);

        //    ParrafoNo_1 = ParrafoNo_1.Replace("@Solicitante", datoreemplazar);

        //    ParrafoNo_1 = ParrafoNo_1.Replace("@DireccionSolicitante", strDireccionNotificacion);

        //    ParrafoNo_1 = ParrafoNo_1.Replace("@Departamento", DireccionNotificacion[0].Departamento);

        //    ParrafoNo_1 = ParrafoNo_1.Replace("@Municipio", DireccionNotificacion[0].Municipio);



        //    LlenaBanner(ParrafoNo_1, "Justificado", "Blanco");
        //    doc.Add(tableBanner);

        //    doc.Add(Enter);

        //    LlenaBanner("Después de realizar el análisis de los documentos legales para este tipo de Registro y de la lectura del expediente sometido a la consideración de esta Delegación Jurídica establece lo siguiente:", "Izquierda", "Blanco");
        //    doc.Add(tableBanner);

        //    doc.Add(Enter);


        //    if ((tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes != ""))
        //    {
        //        LlenaBanner("ANTECEDENTES", "Centro", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);

        //        LlenaBanner(tbl_sol_Solicitud_OficioDictamenJuridico.Antecedentes, "Justificado", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }

        //    if ((tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal != ""))
        //    {
        //        LlenaBanner("FUNDAMENTO LEGAL", "Centro", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);

        //        LlenaBanner(tbl_sol_Solicitud_OficioDictamenJuridico.FundamentoLegal, "Justificado", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }


        //    if ((tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.Analisis != ""))
        //    {

        //        LlenaBanner("ANÁLISIS", "Centro", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);

        //        LlenaBanner(tbl_sol_Solicitud_OficioDictamenJuridico.Analisis, "Justificado", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }

        //    if ((tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != null) && (tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica != ""))
        //    {

        //        LlenaBanner("OPINIÓN JURÍDICA", "Centro", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);

        //        LlenaBanner(tbl_sol_Solicitud_OficioDictamenJuridico.OpinionJuridica, "Justificado", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);

        //    }



        //    if (tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Count() > 0)
        //    {

        //        LlenaBanner("Previo a continuar con el trámite administrativo del expediente el solicitante debe presentar lo siguiente:", "Izquierda", "Blanco");
        //        doc.Add(tableBanner);

        //        doc.Add(Enter);
        //    }

        //    int intcontador = 0;

        //    foreach (var Item in tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda)
        //    {
        //        intcontador = intcontador + 1;

        //        LlenaBanner(intcontador.ToString() + ". " + Item.Descripcion, "Izquierda", "Blanco");
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }


        //    if (tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Count() > 0)
        //    {

        //        LlenaBanner("Por lo anterior, solicito a su persona, requerir la información al solicitante. Para continuar con el trámite del expediente administrativo.", "Izquierda", "Blanco");
        //        doc.Add(tableBanner);

        //        doc.Add(Enter);
        //    }
        //    else
        //    {
        //        LlenaBanner("Envio el presente documento para continuar con el trámite del expediente administrativo.", "Izquierda", "Blanco");
        //        doc.Add(tableBanner);

        //        doc.Add(Enter);

        //    }

        //    //LlenaBanner("Atentamente", "Izquierda", "Blanco");
        //    //doc.Add(tableBanner);

        //    //doc.Add(Enter);


        //    LlenaBanner(objUs.strNombre_Usuario, "Centro", "Blanco");
        //    doc.Add(tableBanner);

        //    LlenaBanner("Delegada(o) Jurídico(a)", "Centro", "Blanco");
        //    doc.Add(tableBanner);

        //    //LlenaBanner("cc Archivo", "Izquierda", "Blanco");
        //    //doc.Add(tableBanner);

        //    //LlenaBanner("// Expediente", "Izquierda", "Blanco");
        //    //doc.Add(tableBanner);

        //    DateTime Fecha = tbl_sol_Solicitud_OficioDictamenJuridico.swdatecreated ?? DateTime.Now;

        //    string Fch_String = Fecha.Day.ToString("D2") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Year.ToString("D4") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

        //    LlenaBanner("Fecha y hora " + Fch_String, "Izquierda", "Blanco");
        //    doc.Add(tableBanner);

        //    doc.Add(Enter);

        //    doc.Close();
        //    writer.Close();

        //    return strNombre;
        //}

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

        //        //                @"      ""Coordenadas"": ""60,60,120,120"",


        //        body = body.Replace("strUsuarioFirma", strUsuarioFirma).Replace("strUsuarioPassword", strUsuarioPassword).Replace("strparentGoogleDriveId", parentGoogleDriveId);
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

        //    return strRespuesta;
        //}

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


        public bool ProcesarFirmaElectronica(Tbl_Sol_Solicitud_OficioDictamenJuridico tbl_form_formulario)
        {
            bool bl_resultado = true;
            string strBearer;
            string rootpath = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";
            string rootpdf = rootpath + tbl_form_formulario.documentoNoFirmado;

            tbl_form_formulario.FirmaElectronica_Estado_id = tbl_form_formulario.FirmaElectronica_Estado_id ?? 1;

            if (tbl_form_formulario.FirmaElectronica_Estado_id == 1)
            {
                AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 2, "Inicia la gestion de Firma electronica");
                AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 3, "Gestionando Bearer");

                strBearer = GetBearer();

                if (strBearer.Length < 125)
                {
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 3, "Error. Fallo al tratar de conseguir Bearer" + strBearer);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 4, "Subidendo archivo a Google");

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length > 30) && (strDocumentoSubido.Length < 36))
                {
                    tbl_form_formulario.FirmaElectronica_Estado_id = 4;
                    tbl_form_formulario.documentoNoFirmado = strDocumentoSubido;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 4, "Archivo subido a Google de forma exitosa");

                }
                else
                {
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 4, strDocumentoSubido);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 5, "Se intentara firmar el documento");

                RequestUtil requestUtil = new RequestUtil();

                string strDocumentofirmado = requestUtil.firmarFile(tbl_form_formulario.FirmaElectronicaUsuario, tbl_form_formulario.FirmaElectronicaPassword, tbl_form_formulario.documentoNoFirmado);

                //string strDocumentofirmado = firmarFile(strBearer, tbl_form_formulario.FirmaElectronicaUsuario, tbl_form_formulario.FirmaElectronicaPassword, tbl_form_formulario.documentoNoFirmado);

                if ((strDocumentofirmado.Length > 30) && (strDocumentofirmado.Length < 36))
                {
                    tbl_form_formulario.FirmaElectronica_Estado_id = 5;
                    tbl_form_formulario.documentoFirmado = strDocumentofirmado;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 5, "Documento firmado de forma exitosa.");

                }
                else
                {
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 5, strDocumentofirmado);
                    bl_resultado = false;
                    return bl_resultado;
                }

                AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 6, "Intentando guardar archivo de forma local.");

                if (getFile(strBearer, tbl_form_formulario.documentoFirmado, rootpath) == true)
                {
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 6, "Archivo grabado en server local.");
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 7, "Fin de la firma electronica.");
                    AgregarBitacora(tbl_form_formulario.Solicitud_id, tbl_form_formulario.FirmaElectronicaUsuario, 10, "Documento con firma electronica.");

                    tbl_form_formulario.FirmaElectronica_Estado_id = 10;
                    tbl_form_formulario.documentoFirmado = strDocumentofirmado;

                    db.Entry(tbl_form_formulario).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return bl_resultado;
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

        public JsonResult JsonProcesarFirmaElectronica(string Guid_id, string Guidetapa_id, string UsuarioFE, string PasswordFE)
        {
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;

            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioJuridicoController-JsonProcesarFirmaElectronica");

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };
            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);

            try
            {
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


                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 2, "B.- Se busca obtener el bearer");

                strBearer = GetBearer();

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 3, "C.- Bearer obtenido");

                //            strBearer = GetBearerNeftafiufiu();
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

                RequestUtil requestUtil = new RequestUtil();

                string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);

                //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

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


                if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
                {
                    //Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                    //db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                    //db.SaveChanges();


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

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 10, "Z.- No se pudo generar la FE" + ex.Message.ToString());


                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró realizar la firma electrónica. " + ex.Message.ToString() + "\"}";

                return Json(jsonResultUsr);

            }



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


        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitudDocumentosRecibidos
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


            int intCantidadVerificados = (from d in db.fc_Sol_DocumentosSubido(solicitud_id)
                                          select d).Where(Obj => Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificadoNotario == true).Count();

            int intCantidadDebeVerificar = (from d in db.fc_Sol_DocumentosSubido(solicitud_id)
                                            from c in db.Tbl_Sol_DocumentoTipo
                                            where c.Tipo_Documento_id == d.Tipo_Documento_id
                                            && c.ValidacionJuridica == true
                                            select d).Where(Obj => Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificadoNotario == true).Count();

            //int intCantidadVerificados = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificadoNotario == true).Count();

            //int intCantidadDebeVerificar = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.Tbl_Sol_DocumentoTipo.ValidacionJuridica == true).Count();


            if ((intCantidadVerificados != intCantidadDebeVerificar))
            {
                codRespuesta = 0;
                strRespuesta = "Error: Debe marcar como verificados los documentos, para continuar.";

                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);

            }


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


        public JsonResult ConsultarDocumentosRequeridos(Tbl_Sol_Solicitud model)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == model.Guid_id).FirstOrDefault();


            List<fc_Sol_DocumentosRequeridos_Result> fc_Sol_DocumentosRequeridos_Results = new List<fc_Sol_DocumentosRequeridos_Result>();

            if (tbl_Sol_Solicitud != null)
            {
                fc_Sol_DocumentosRequeridos_Results = ((from d in db.fc_Sol_DocumentosRequeridos(tbl_Sol_Solicitud.Solicitud_id)
                                                        where d.Tipo_Documento_id > 0
                                                        orderby d.Tipo_Documento_id
                                                        select d).ToList() ?? new List<fc_Sol_DocumentosRequeridos_Result>());
            }


            return Json(fc_Sol_DocumentosRequeridos_Results);
        }

    }

}

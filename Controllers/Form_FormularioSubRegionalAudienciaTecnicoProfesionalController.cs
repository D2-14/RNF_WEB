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
    public class Form_FormularioSubRegionalAudienciaTecnicoProfesionalController : Controller
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



        public ActionResult AvisoDeAudiencia(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            int Doc_TecPro_Tipo_id = 2;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;
            ViewBag.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            ViewBag.Doc_TecPro_Tipo_id = Doc_TecPro_Tipo_id;

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                && d.Etapa_id == etapa_id
                                                                && d.EtapaRuta_id == etaparuta_id
                                                                && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();

            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }


            return View(tbl_Gest_EtapaSolicitud);
            //Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            //return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
            //return View();
        }










        public class JsonResupesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }



        public Parrafos ListarParrafos(string Solicitud_Guid_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id)
        {
            int countParrafos = 0;
            Parrafos parrafos = new Parrafos();

            string sqlQuery, validaParams, strSelect;

            strSelect = "select [dbo].[Fcn_Gest_EtapaSolicitud_Inactivacion_Parrafo_Tipo]";
            //4 es el default para Aviso de audiencia para el técnico o profesional que se dedica a la actividad forestal
            validaParams = $"'{Solicitud_Guid_id}','{Etapa_id}','{EtapaRuta_id}','{CorrelativoEtapa_id}','4'";

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


        private void LlenaBanner(String Leyenda, string Color = "", int alineacionhorizontal = 0, int alineacionvertical = 5, int borde = 0, string estilotexto = "Normal", int tamaniotexto = 10)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: tamaniotexto, FontColour);
            Font fntTituloBold = FontFactory.GetFont("HELVETICA", size: tamaniotexto, Font.BOLD);
            Font estiloFont = FontFactory.GetFont("HELVETICA", size: tamaniotexto, FontColour);
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

        public string GenerarDocumentoAudiencia_PDF(Tbl_Sol_Solicitud tbl_Sol_Solicitud, Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud, Usuario objUs)
        {
            Tbl_Sol_IdentificadorOficial tbl_Sol_IdentificadorOficial = new Tbl_Sol_IdentificadorOficial();
            try
            {
                tbl_Sol_IdentificadorOficial = (from d in db.Tbl_Sol_IdentificadorOficial
                                                where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                && d.GestionTipo_id == 4
                                                orderby d.CorrelativoEtapa_id descending
                                                select d).FirstOrDefault();
            }
            catch (Exception ex)
            {
                tbl_Sol_IdentificadorOficial = new Tbl_Sol_IdentificadorOficial();
            }
            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora == null)
            {
                return null;
            }

            List<Tbl_RNF_Registro_BitacoraHallazgo> tbl_RNF_Registro_BitacoraHallazgos = (from d in db.Tbl_RNF_Registro_BitacoraHallazgo
                                                                                          where d.Solicitud_id == tbl_RNF_Registro_Bitacora.Solicitud_id
                                                                                          && d.No_Registro == tbl_RNF_Registro_Bitacora.No_Registro
                                                                                          && d.Bitacora_id == tbl_RNF_Registro_Bitacora.Bitacora_id
                                                                                          orderby d.Correlativo_id
                                                                                          select d).ToList();

            if (tbl_RNF_Registro_BitacoraHallazgos == null)
            {
                tbl_RNF_Registro_BitacoraHallazgos = new List<Tbl_RNF_Registro_BitacoraHallazgo>();
            }


            Parrafos parrafos = ListarParrafos(tbl_Gest_EtapaSolicitud.Solicitud_Guid_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id);

            Constants constants = new Constants();
            CrearBanner crearBanner = new CrearBanner();
            Result_SP_IdentificadorOficialGestion resultsp = new Result_SP_IdentificadorOficialGestion();
            //resultsp = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);
            resultsp = new Result_SP_IdentificadorOficialGestion()
            {
                respuesta = 1,
                Identificador = "Identificador",
                Version = "Version",
                Fecha = DateTime.Now,
                strFecha = "strFecha",
                Codigo = "Codigo",
                id = 1
            };

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            string strNombreTecnicoProfesional = "";
            string fechaDocumento = "";
            decimal SolicitudTipo_id = 0;
            string valorbusqueda, valorreemplazar;
            long contador = 0;

            var Enter = new Paragraph(" ");
            strNombre = "U" + tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id + ".pdf";
            strDirArchivo = strFolder + strNombre;

            fechaDocumento = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_FechaTxtSinGuatemala](getdate())").FirstOrDefault();

            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_Sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_Sol_Solicitud.SubRegion_id).First();
            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).First();

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 50f, 50f);

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            var strTitulo = "AVISO DE AUDIENCIA PARA EL TÉCNICO O PROFESIONAL QUE SE DEDICA A LA ACTIVIDAD FORESTAL";

            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();

            try
            {
                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    crearBanner.LlenaBanner(this.Url.Action(), "Izquierda", "Gris");
                    doc.Add(crearBanner.tableBanner);
                }

                crearBanner.LlenaTituloRevision(strTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);

                LlenaBanner(Leyenda: fechaDocumento, alineacionhorizontal: Al_Derecha, estilotexto: "Bold");
                doc.Add(tableBanner);
                doc.Add(Enter);


                if (strNombreTecnicoProfesional.Trim() != "")
                {

                }
                LlenaBanner(Leyenda: "Señor(a)", alineacionhorizontal: Al_Izquierda);
                doc.Add(tableBanner);
                LlenaBanner(Leyenda: parrafos.Parrafo_1, alineacionhorizontal: Al_Izquierda);
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner(Leyenda: parrafos.Parrafo_2, alineacionhorizontal: Al_Izquierda);
                doc.Add(tableBanner);
                LlenaBanner(Leyenda: parrafos.Parrafo_3, alineacionhorizontal: Al_Izquierda);
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner(Leyenda: parrafos.Parrafo_4, alineacionhorizontal: Al_Justificado);
                doc.Add(tableBanner);
                doc.Add(Enter);

                //Sección de bitácora
                if (tbl_RNF_Registro_BitacoraHallazgos.Count() > 0)
                {
                    contador = 0;
                    foreach (var item in tbl_RNF_Registro_BitacoraHallazgos)
                    {
                        if ((item.DescripcionHallazgo != null) && (item.DescripcionHallazgo.Trim() != ""))
                        {
                            contador++;
                            crearBanner.LlenaBanner(contador + ". " + item.DescripcionHallazgo, "Izquierda", "");
                            doc.Add(crearBanner.tableBanner);
                            doc.Add(Enter);
                        }
                    }
                }



                LlenaBanner(Leyenda: parrafos.Parrafo_5, alineacionhorizontal: Al_Justificado);
                doc.Add(tableBanner);
                doc.Add(Enter);
                doc.Add(Enter);
                doc.Add(Enter);

                LlenaBanner(Leyenda: DatosSubRegional.NombreCompleto, alineacionhorizontal: Al_Centro, estilotexto: "Bold");
                doc.Add(tableBanner);
                LlenaBanner(Leyenda: "Dirección SubRegional  " + tbl_Gral_SubRegion.No_SubRegion, alineacionhorizontal: Al_Centro, estilotexto: "Bold");
                doc.Add(tableBanner);

                doc.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                doc.Close();
                writer.Close();
                return null;

            }





            return strNombre;
        }
        public JsonResult GenerarDocumentoAudiencia(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            JsonResupesta jsonResupesta = new JsonResupesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(jsonResupesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                && d.Etapa_id == etapa_id
                                                                && d.EtapaRuta_id == etaparuta_id
                                                                && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();
            string NombreArchivo = "";

            try
            {
                NombreArchivo = GenerarDocumentoAudiencia_PDF(tbl_Sol_Solicitud, tbl_Gest_EtapaSolicitud, objUs);

                if (NombreArchivo != null)
                {
                    tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = NombreArchivo;
                    db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
                    db.SaveChanges();
                    jsonResupesta = new JsonResupesta()
                    {
                        Result = 1,
                        Mensaje = "Archivo generado exitosamente",
                        Ubicacion = NombreArchivo
                    };
                }
                else
                {
                    jsonResupesta = new JsonResupesta()
                    {
                        Result = 2,
                        Mensaje = "No se pudo generar el documento"
                    };
                }

            }
            catch (Exception ex)
            {
                jsonResupesta = new JsonResupesta()
                {
                    Result = 3,
                    Mensaje = "Ocurrió un error, " + ex.Message
                };
            }

            return Json(jsonResupesta);
        }




        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string motivo, int respuestaid, string EtapaSolicitud_GUIDid)
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


            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioSubRegionalAudienciaTecnicoProfesionalController-JsonProcesarFirmaElectronica");

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

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 2, "B.- Se busca obtener el bearer");

                strBearer = GetBearer();

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 3, "C.- Bearer obtenido");

                if (strBearer.Length < 125)
                {


                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 4, "D.- Bearer erroneo, menor a 125 caracteres");

                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                    return Json(jsonResultUsr);

                }

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
                {

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 5, "N.- Error al subir el documento.");

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

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 6, "R.- Error al firmar el documento, Usuario o Password Erroneos.");


                    // se cambio de 0 a 1 para pruebas de emananuel

                    intRespuesta = 1;

                    strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                    return Json(jsonResultUsr);


                }


                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 7, "W.- Intentando obtener archivo firmado.");

                if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
                {
                    Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                    db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                    db.SaveChanges();
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 8, "X.- El archivo se obtuvo con exito.");

                }
                else
                {
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 9, "X.- No se logró obtener el archivo firmado.");

                }



                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 10, "Z.- FE generada con éxito.");

                intRespuesta = 1;

                Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + intRespuesta + "\","
                                  + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

                return Json(jsonResultUsr);
            }
            catch (Exception ex)
            {

                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, UsuarioFE, 1, 10, "Z.- No se pudo generar la FE" + ex.Message.ToString());


                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró realizar la firma electrónica. " + ex.Message.ToString() + "\"}";

                return Json(jsonResultUsr);

            }


        }


    }
}
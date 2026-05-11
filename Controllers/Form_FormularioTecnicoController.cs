using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelDataReader;
using System.Data;
using System.Data.SqlClient;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Font = iTextSharp.text.Font;
using System.Data.Entity;
using DotSpatial.Topology;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoController : Controller
    {

        db_RNFEntities db = new db_RNFEntities();
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();
        
        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosInstrucciones = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosEvaluacion = new PdfPTable(numColumns: 8);
        private PdfPTable tablePreguntasRespuestas = new PdfPTable(numColumns: 8);
        private PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        private PdfPTable tablePersoneria = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);
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
        // GET: EvaluacionTecnicos


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

        public ActionResult Index(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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
            ViewBag.Session = objUs;


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();



            ViewBag.Owner = 1;

            if (oSolicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            ViewBag.GuidEtapa_id = GuidEtapa_id;

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            ViewBag.Firmado = false;

            if ((tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado != null) && (tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado.ToString() != ""))
            {
                ViewBag.NombreArchivo = tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado;
                ViewBag.Firmado = true;

            }



            return View(oSolicitud);

        }


        public ActionResult AnalisisEmpresa(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            return RedirectToAction("Create", "Sol_Empresa_Entidad", new { solicitud_id = oSolicitud.Solicitud_id, firma = Guid_id });
        }

        public ActionResult AnalisisRodal(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Loginx");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            ViewBag.Session = objUs;


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();


            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View(oSolicitud);

        }


        public ActionResult AnalisisCentroParcela(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Loginx");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            ViewBag.Session = objUs;


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();


            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View(oSolicitud);

        }

        public ActionResult CentroParcelaTecnico(long Solicitud_id)
        {
            List<Tbl_API_Sol_Rodal_Dasometrico_CentroParcela_Local> tbl_API_Sol_Rodal_Dasometrico_CentroParcela_Locals = db_API.Tbl_API_Sol_Rodal_Dasometrico_CentroParcela_Local.Where(Obj => Obj.Solicitud_id == Solicitud_id).ToList();
            return View(tbl_API_Sol_Rodal_Dasometrico_CentroParcela_Locals);
        }

        public string GeneraDatosPoligonos(long Solicitud_id)
        {
            long solicitudid;
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            //string fecha = "" + hoy.Day + "_" + hoy.Month + "_" + hoy.Year;
            string fecha = hoy.Ticks.ToString();
            string strNombre, strDirArchivo;
            solicitudid = Solicitud_id;
            string rootbase, rootpath, rootdest, rootnew, machote, destfinal, nombrereporte, destfile, destxlsx, tipodeareadescripcion;
            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            ExcelPackage oEPP;


            #region Preparación de Datos

            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Content/Machotes/";
            rootnew = $"{rootbase}Archivos_Generados_Que_Pueden_Borrar/";
            machote = $"{rootpath}DatosPoligono_En_Blanco.xlsx";
            rootdest = $"{rootpath}";
            nombrereporte = $"DatosPoligono_{fecha}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";

            List<Tbl_Sol_Rodal_Poligono> oPoligonos = new List<Tbl_Sol_Rodal_Poligono>();
            oPoligonos = (from d in db.Tbl_Sol_Rodal_Poligono
                          where d.Solicitud_id == solicitudid
                          orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Id, d.Correlativo_id
                          select d).ToList();

            List<Tbl_Sol_Rodal_Descuento_Poligono> oAreaDescuento = new List<Tbl_Sol_Rodal_Descuento_Poligono>();
            oAreaDescuento = (from d in db.Tbl_Sol_Rodal_Descuento_Poligono
                              where d.Solicitud_id == solicitudid
                              orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Descuento_Id, d.Correlativo_id
                              select d).ToList();
            #endregion


            //strNombre = $"..//..//Archivos_Generados_Que_Pueden_Borrar//{nombrereporte}.xlsx";
            strNombre = $"/Archivos_Generados_Que_Pueden_Borrar/{nombrereporte}.xlsx";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            #region Manipulacion de Archivo XLSX
            oEPP = new ExcelPackage(new FileInfo(machote));
            ExcelWorksheet wSheet1, wSheet2;
            wSheet1 = oEPP.Workbook.Worksheets[0];

            iniciofila = finalfila = 13;
            iniciocolumna = finalcolumna = 1;
            int startrow, startcol;

            for (int i = 0; i < oPoligonos.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;
                wSheet1.InsertRow(rowFrom: startrow, 1);
                tipodearea = oPoligonos[i].Tipo_de_Area;
                Tbl_Sol_Rodal_Tipo oTipoDeArea = (from d in db.Tbl_Sol_Rodal_Tipo
                                                  where d.Tipo_de_Area == tipodearea
                                                  select d).FirstOrDefault();
                tipodeareadescripcion = oTipoDeArea.Descripcion;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse($"{oPoligonos[i].Finca_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse($"{oPoligonos[i].Rodal_Id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = tipodeareadescripcion;
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {

                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (oPoligonos[i].GTMX ?? 0);

                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (oPoligonos[i].GTMY ?? 0);

                }

                iniciofila += 1;
            }



            iniciofila += 16;
            iniciocolumna = 1;

            for (int i = 0; i < oAreaDescuento.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;
                tipodearea = oAreaDescuento[i].Tipo_de_Area;
                Tbl_Sol_Rodal_Tipo oTipoDeArea = (from d in db.Tbl_Sol_Rodal_Tipo
                                                  where d.Tipo_de_Area == tipodearea
                                                  select d).FirstOrDefault();
                tipodeareadescripcion = oTipoDeArea.Descripcion;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{oAreaDescuento[i].Finca_id}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{oAreaDescuento[i].Rodal_Id}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{oAreaDescuento[i].Rodal_Descuento_Id}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = tipodeareadescripcion;
                }

                startcol += 1;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (oAreaDescuento[i].GTMX ?? 0).ToString();

                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (oAreaDescuento[i].GTMY ?? 0).ToString();
                }


                iniciofila += 1;
            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            //oEPP.SaveAs(new FileInfo($"{strNombre}"));
            oEPP.SaveAs(new FileInfo($"{destxlsx}"));


            #endregion

            return strNombre;
        }


        public JsonResult DescargaDatosPoligonos(string Guid_id)
        {
            string TextoMostrar, guidid;
            long solicitudid;
            guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            string archivo = GeneraDatosPoligonos(oSolicitud.Solicitud_id);
            TextoMostrar = "{\"Ubicacion\":\"" + archivo + "\"}";

            return Json(TextoMostrar);
        }

        public ActionResult GenerarPreguntas(long Solicitud_id, int Categoria_id, int Sub_Categoria_id)
        {
            List<Tbl_Tecnico_Pregunta> oPregunta = (from d in db.Tbl_Tecnico_Pregunta
                                                    where d.Categoria_id == Categoria_id && d.Sub_Categoria_id == Sub_Categoria_id
                                                    select d).OrderBy(d => d.Pregunta_id).ToList();

            ViewBag.Categoria_id = Categoria_id;
            ViewBag.Sub_Categoria_id = Sub_Categoria_id;
            ViewBag.Solicitud_id = Solicitud_id;
            return View(oPregunta);
        }
        public ActionResult GenerarRespuestas(long Solicitud_id, int Categoria_id, int Sub_Categoria_id, int Pregunta_id)
        {
            List<Tbl_Tecnico_PreguntaRespuesta> oRespuesta = (from d in db.Tbl_Tecnico_PreguntaRespuesta
                                                              where d.Categoria_id == Categoria_id && d.Sub_Categoria_id == Sub_Categoria_id && d.Pregunta_id == Pregunta_id
                                                              select d).OrderBy(d => d.Respuesta_id).ToList();

            ViewBag.Categoria_id = Categoria_id;
            ViewBag.Sub_Categoria_id = Sub_Categoria_id;
            ViewBag.Pregunta_id = Pregunta_id;
            ViewBag.Solicitud_id = Solicitud_id;
            return View(oRespuesta);
        }
        private void LlenaTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

            tableTitulo = new PdfPTable(7);

            //// Imagen superior
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/images/logoInabExcel.jpg"));

            logo.ScalePercent(80f);

            PdfPCell c1 = new PdfPCell(logo);


            c1.Colspan = 2;
            c1.Rowspan = 4;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nBOLETA DE DECISION DE PLANTACION VOLUNTARIA \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("REV-0.1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Agosto 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }


        private void LlenaDatosSolicitud(Tbl_Sol_Solicitud tbl_Sol_Solicitud, string No_InformeTecnico)
        {
            PdfPCell c1 = new PdfPCell();
            tableDatosEvaluacion = new PdfPTable(11);
            Font fntInstrucciones = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase("Expediente No.", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Solicitud_NumeroExpediente, fntTituloTabla));
            c1.BackgroundColor = Blanco;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Subregión", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            Constants constants = new Constants();

            c1 = new PdfPCell(new Phrase(constants.initCapPalabras(tbl_Sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion), fntTituloTabla));
            c1.BackgroundColor = Blanco;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);
        }
        private void LlenaDatosEvaluacion(Tbl_Sol_Finca tbl_Sol_Fincas)
        {
            PdfPCell c1 = new PdfPCell();
            tableDatosEvaluacion = new PdfPTable(11);
            Font fntInstrucciones = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase("Dirección de Ubicación del Área", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Fincas.Ubicacion + ", " + tbl_Sol_Fincas.Tbl_Gral_Municipio.Municipio + ", " + tbl_Sol_Fincas.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            c1.BackgroundColor = Blanco;
            c1.Colspan = 8;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Nombre de la Finca", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Fincas.NombreFinca, fntTituloTabla));
            c1.BackgroundColor = Blanco;
            c1.Colspan = 8;
            c1.HorizontalAlignment = Al_Izquierda;
            c1.VerticalAlignment = Al_Abajo;
            tableDatosEvaluacion.AddCell(c1);

            return;
        }

        private void LlenaInstrucciones(string[] instrucciones)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosInstrucciones = new PdfPTable(1);
            Font fntInstrucciones = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            for (int i = 0; i < instrucciones.Count(); i++)
            {
                c1 = new PdfPCell(new Phrase($"{instrucciones[i]}", fntTituloTabla));
                c1.BackgroundColor = GrisClaro;
                c1.Colspan = 1;
                c1.HorizontalAlignment = Al_Izquierda;
                c1.VerticalAlignment = Al_Abajo;
                tableDatosInstrucciones.AddCell(c1);
            }

            tableBanner.AddCell(c1);
            return;
        }
        private void LlenaPreguntasRespuestas(Tbl_Sol_Solicitud tbl_sol_Solicitud)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tablePreguntasRespuestas = new PdfPTable(14);
            Font fntPreguntas = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntRespuestas = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntObservaciones = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            int preguntaid, respuestaid;
            string observacionestext;

            c1 = new PdfPCell(new Phrase("Lineamiento", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Al_Centro;
            c1.VerticalAlignment = Al_Centro;
            tablePreguntasRespuestas.AddCell(c1);
            c1 = new PdfPCell(new Phrase("Parámetro", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 5;
            c1.HorizontalAlignment = Al_Centro;
            c1.VerticalAlignment = Al_Centro;
            tablePreguntasRespuestas.AddCell(c1);
            c1 = new PdfPCell(new Phrase("Respuesta", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Al_Centro;
            c1.VerticalAlignment = Al_Centro;
            tablePreguntasRespuestas.AddCell(c1);
            c1 = new PdfPCell(new Phrase("Observaciones", fntTituloTabla));
            c1.BackgroundColor = GrisClaro;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Al_Centro;
            c1.VerticalAlignment = Al_Centro;
            tablePreguntasRespuestas.AddCell(c1);




            List<Tbl_Tecnico_Pregunta> oPreguntas = (from d in db.Tbl_Tecnico_Pregunta
                                                     where d.Categoria_id == tbl_sol_Solicitud.Categoria_id && d.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id
                                                     select d).OrderBy(d => d.Pregunta_id).ToList();

            for (int i = 0; i < oPreguntas.Count(); i++)
            {
                preguntaid = oPreguntas[i].Pregunta_id;
                List<Tbl_Tecnico_PreguntaRespuesta> oRespuestas = (from d in db.Tbl_Tecnico_PreguntaRespuesta
                                                                   where d.Categoria_id == tbl_sol_Solicitud.Categoria_id && d.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id && d.Pregunta_id == preguntaid
                                                                   select d).OrderBy(d => d.Respuesta_id).ToList();

                c1 = new PdfPCell(new Phrase($"{oPreguntas[i].Pregunta_id}", fntTituloTabla));
                c1.BackgroundColor = Blanco;
                c1.Colspan = 1;
                c1.Rowspan = oRespuestas.Count();
                c1.HorizontalAlignment = Al_Centro;
                c1.VerticalAlignment = Al_Medio;
                tablePreguntasRespuestas.AddCell(c1);
                c1 = new PdfPCell(new Phrase($"{oPreguntas[i].Descripcion}", fntTituloTabla));
                c1.BackgroundColor = Blanco;
                c1.Colspan = 3;
                c1.Rowspan = oRespuestas.Count();
                c1.HorizontalAlignment = Al_Izquierda;
                c1.VerticalAlignment = Al_Medio;
                tablePreguntasRespuestas.AddCell(c1);

                for (int j = 0; j < oRespuestas.Count(); j++)
                {
                    preguntaid = oRespuestas[j].Pregunta_id;
                    respuestaid = oRespuestas[j].Respuesta_id;

                    Tbl_Tecnico_PreguntaRespuesta_Evaluacion oEvaluacion = (from d in db.Tbl_Tecnico_PreguntaRespuesta_Evaluacion
                                                                            where d.Solicitud_id == tbl_sol_Solicitud.Solicitud_id
                                                                                && d.Categoria_id == tbl_sol_Solicitud.Categoria_id
                                                                                && d.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id
                                                                                && d.Pregunta_id == preguntaid
                                                                                && d.Respuesta_id == respuestaid
                                                                            select d).FirstOrDefault();

                    c1 = new PdfPCell(new Phrase($"{oRespuestas[j].Descripcion}", fntTituloTabla));
                    c1.BackgroundColor = Blanco;
                    c1.Colspan = 5;
                    c1.HorizontalAlignment = Al_Centro;
                    c1.VerticalAlignment = Al_Centro;
                    tablePreguntasRespuestas.AddCell(c1);

                    if (oEvaluacion != null)
                    {
                        c1 = new PdfPCell(new Phrase($"X", fntTituloTabla));
                        c1.BackgroundColor = Blanco;
                        c1.Colspan = 2;
                        c1.HorizontalAlignment = Al_Centro;
                        c1.VerticalAlignment = Al_Centro;
                        tablePreguntasRespuestas.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{oEvaluacion.Observaciones}", fntTituloTabla));
                        c1.BackgroundColor = Blanco;
                        c1.Colspan = 3;
                        c1.HorizontalAlignment = Al_Arriba;
                        c1.VerticalAlignment = Al_Izquierda;
                        tablePreguntasRespuestas.AddCell(c1);


                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                        c1.BackgroundColor = Blanco;
                        c1.Colspan = 2;
                        c1.HorizontalAlignment = Al_Centro;
                        c1.VerticalAlignment = Al_Centro;
                        tablePreguntasRespuestas.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                        c1.BackgroundColor = Blanco;
                        c1.Colspan = 3;
                        c1.HorizontalAlignment = Al_Arriba;
                        c1.VerticalAlignment = Al_Izquierda;
                        tablePreguntasRespuestas.AddCell(c1);

                    }



                }
            }

            tableBanner.AddCell(c1);
            return;
        }
        private void FirmaSolicitante(Usuario objUs)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 7);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            c1 = new PdfPCell(new Phrase($"{objUs.strNombre_Usuario}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"NOMBRE DEL TÉCNICO", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }
        public string GenerarEvaluacion(long Solicitud_id, string GuidEtapa_id)
        {
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
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

            var Enter = new Paragraph(" ");

            string[] Texto_Romano = new string[11];
            int intTexto_Romano = 1;
            {
                Texto_Romano[1] = "I";
                Texto_Romano[2] = "II";
                Texto_Romano[3] = "III";
                Texto_Romano[4] = "IV";
                Texto_Romano[5] = "V";
                Texto_Romano[6] = "VI";
                Texto_Romano[7] = "VII";
                Texto_Romano[8] = "VIII";
                Texto_Romano[9] = "IX";
                Texto_Romano[10] = "X";
            }
            string[] ListaInstrucciones =
            {
                //"Para uso del Técncio forestal evaluador",
                //"Observe el flujograma de decisión Formato FR-PR-37 para determinar los lineamientos y parámetros de evaluación según el escenario que se le presente",
                "Marque con una \"X\" la respuesta"
            };

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);
            strNombre = @"U" + GuidEtapa_id + ".pdf";
            strDirArchivo = strFolder + strNombre;

            //4	                Informe técnico
            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).FirstOrDefault();
            string No_InformeTecnico = "";

            List<Tbl_Sol_Finca> tbl_Sol_Fincas = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Finca_Id).ToList();

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }


            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(12, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            CrearBanner crearBanner = new CrearBanner();

            string complementoTitulo = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_CategoriaNombreDescripcionEnOficio(@p0)", tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

            string varTitulo = "BOLETA DE DECISION PARA " + complementoTitulo.ToUpper();

            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);
            doc.Add(Enter);

            LlenaDatosSolicitud(tbl_sol_Solicitud, No_InformeTecnico);
            doc.Add(tableDatosEvaluacion);
            doc.Add(Enter);

            for (int i = 0; i < tbl_Sol_Fincas.Count(); i++)
            {
                LlenaDatosEvaluacion(tbl_Sol_Fincas[i]);
                doc.Add(tableDatosEvaluacion);
                doc.Add(Enter);
            }

            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. INSTRUCCIONES");
            doc.Add(tableBanner);
            intTexto_Romano += 1;
            LlenaInstrucciones(ListaInstrucciones);
            doc.Add(tableDatosInstrucciones);
            doc.Add(Enter);

            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DESCRIPCIÓN DEL ÁREA");
            doc.Add(tableBanner);
            intTexto_Romano += 1;

            LlenaPreguntasRespuestas(tbl_sol_Solicitud);
            doc.Add(tablePreguntasRespuestas);
            doc.Add(Enter);


            FirmaSolicitante(objUs);
            doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();
            return strNombre;
        }

        [HttpPost]
        public JsonResult AgregarPreguntasRespuestas(Tbl_Tecnico_PreguntaRespuesta_Evaluacion model)
        {
            String TextoMostrar;
            TextoMostrar = "Realizado";

            Tbl_Tecnico_PreguntaRespuesta_Evaluacion oEvaluacion = new Tbl_Tecnico_PreguntaRespuesta_Evaluacion();
            oEvaluacion = (from d in db.Tbl_Tecnico_PreguntaRespuesta_Evaluacion
                           where d.Solicitud_id == model.Solicitud_id
                                && d.Categoria_id == model.Categoria_id
                                && d.Sub_Categoria_id == model.Sub_Categoria_id
                                && d.Pregunta_id == model.Pregunta_id
                           select d).FirstOrDefault();

            if (oEvaluacion != null)
            {
                db.Tbl_Tecnico_PreguntaRespuesta_Evaluacion.Remove(oEvaluacion);
                db.SaveChanges();
            }

            Tbl_Tecnico_PreguntaRespuesta_Evaluacion oEvaluacionNueva = new Tbl_Tecnico_PreguntaRespuesta_Evaluacion();
            oEvaluacionNueva.Solicitud_id = model.Solicitud_id;
            oEvaluacionNueva.Categoria_id = model.Categoria_id;
            oEvaluacionNueva.Sub_Categoria_id = model.Sub_Categoria_id;
            oEvaluacionNueva.Pregunta_id = model.Pregunta_id;
            oEvaluacionNueva.Respuesta_id = model.Respuesta_id;
            oEvaluacionNueva.Observaciones = model.Observaciones;
            oEvaluacionNueva.swcreatedby = model.swcreatedby;
            oEvaluacionNueva.swcreatedbyinterno = model.swcreatedbyinterno;
            oEvaluacionNueva.swdatecreated = DateTime.Now;
            db.Tbl_Tecnico_PreguntaRespuesta_Evaluacion.Add(oEvaluacionNueva);
            db.SaveChanges();

            return Json(new { success = TextoMostrar }, JsonRequestBehavior.AllowGet);

            //return Json(null);
        }

        [HttpPost]
        public JsonResult DescargarEvaluacion(long Solicitud_id, string GuidEtapa_id)
        {
            string TextoMostrar, documento;
            documento = GenerarEvaluacion(Solicitud_id, GuidEtapa_id);
            TextoMostrar = "{ \"Ubicacion\" : \"" + documento + "\"}";

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = documento;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            return Json(TextoMostrar);
        }


        public string GeneraDasometricos(long Solicitud_id)
        {
            long solicitudid;
            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre, strDirArchivo;
            solicitudid = Solicitud_id;
            string rootbase, rootpath, rootdest, rootnew, machote, destfinal, nombrereporte, destfile, destxlsx;
            int iniciofila, iniciocolumna, finalfila, finalcolumna;
            ExcelPackage oEPP;


            #region Preparación de Datos

            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Content/Machotes/";
            rootnew = $"{rootbase}Documentos/";
            machote = $"{rootpath}DatosDasometricos_En_Blanco_Tecnico.xlsx";
            rootdest = $"{rootpath}";
            nombrereporte = $"DatosDasometricos_{Solicitud_id}_{fecha}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";

            List<Tbl_Sol_Rodal_Dasometrico> oDasometricos = new List<Tbl_Sol_Rodal_Dasometrico>();
            oDasometricos = (from d in db.Tbl_Sol_Rodal_Dasometrico
                             where d.Solicitud_id == solicitudid
                             orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela, d.Dasometrico_id
                             select d).ToList();

            #endregion


            strNombre = $"..//..//Documentos//{nombrereporte}.xlsx";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            #region Manipulacion de Archivo XLSX
            oEPP = new ExcelPackage(new FileInfo(machote));
            ExcelWorksheet wSheet1;
            wSheet1 = oEPP.Workbook.Worksheets[0];

            iniciofila = finalfila = 11;
            iniciocolumna = finalcolumna = 1;
            int startrow, startcol;
            for (int i = 0; i < oDasometricos.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse($"{oDasometricos[i].Finca_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse($"{oDasometricos[i].Rodal_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{oDasometricos[i].Tbl_Sol_Rodal_Tipo.Descripcion}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{oDasometricos[i].Area_Efectiva_Rodal}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oDasometricos[i].No_Parcela}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{oDasometricos[i].Area_Muestreada ?? 0}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oDasometricos[i].Dasometrico_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{oDasometricos[i].Tbl_Gral_Especie.NombreCientifico}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oDasometricos[i].Anio_Establecimiento}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{oDasometricos[i].DAP_Promedio}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{oDasometricos[i].Altura_Promedio}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    if ((oDasometricos[i].Clase_id ?? 0).ToString() == "0")
                    {
                        rango.Value = null;
                    }
                    else
                    {
                        rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rango.Value = Int32.Parse((oDasometricos[i].Clase_id ?? 0).ToString());

                    }
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    if ((oDasometricos[i].Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Descripcion.Contains("No Aplica")) == false)
                    {
                        rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rango.Value = oDasometricos[i].Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Descripcion ?? "";
                    }
                }


                iniciofila += 1;
            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            //oEPP.SaveAs(new FileInfo($"{strNombre}"));
            oEPP.SaveAs(new FileInfo($"{destxlsx}"));


            #endregion

            return strNombre;
        }




        public string GeneraDatosPoligonosyDescuento(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            long solicitudid;
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre, strDirArchivo;
            string rootbase, rootpath, rootdest, rootnew, machote, destfinal, nombrereporte, destfile, destxlsx, tipodeareadescripcion;
            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            ExcelPackage oEPP;


            #region Preparación de Datos

            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Content/Machotes/";
            rootnew = $"{rootbase}Archivos_Generados_Que_Pueden_Borrar/";
            machote = $"{rootpath}Datos_Poligono_y_Descuento.xlsx";
            rootdest = $"{rootpath}";
            nombrereporte = $"DatosPoligonoyDescuento_{fecha}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";



            List<Tbl_Sol_Rodal_Poligono> oPoligonos = new List<Tbl_Sol_Rodal_Poligono>();
            oPoligonos = (from d in db.Tbl_Sol_Rodal_Poligono
                          where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                          orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Id, d.Correlativo_id
                          select d).ToList();

            List<Tbl_Sol_Rodal_Descuento_Poligono> oAreaDescuento = new List<Tbl_Sol_Rodal_Descuento_Poligono>();
            oAreaDescuento = (from d in db.Tbl_Sol_Rodal_Descuento_Poligono
                              where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                              orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Descuento_Id, d.Correlativo_id
                              select d).ToList();
            #endregion


            strNombre = $"../../../Archivos_Generados_Que_Pueden_Borrar/{nombrereporte}.xlsx";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            #region Manipulacion de Archivo XLSX
            oEPP = new ExcelPackage(new FileInfo(machote));
            ExcelWorksheet wSheet1, wSheet2;
            wSheet1 = oEPP.Workbook.Worksheets[0];


            using (ExcelRange rango = wSheet1.Cells[5, 5, 5, 5])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = tbl_Sol_Solicitud.Solicitud_NumeroTemporal;
            }

            using (ExcelRange rango = wSheet1.Cells[7, 5, 7, 5])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = tbl_Sol_Solicitud.Solicitud_NumeroExpediente;
            }

            iniciofila = finalfila = 12;
            iniciocolumna = finalcolumna = 1;
            int startrow, startcol;

            for (int i = 0; i < oPoligonos.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;
                wSheet1.InsertRow(rowFrom: startrow, 1);
                tipodearea = oPoligonos[i].Tipo_de_Area;
                Tbl_Sol_Rodal_Tipo oTipoDeArea = (from d in db.Tbl_Sol_Rodal_Tipo
                                                  where d.Tipo_de_Area == tipodearea
                                                  select d).FirstOrDefault();
                tipodeareadescripcion = oTipoDeArea.Descripcion;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oPoligonos[i].Finca_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oPoligonos[i].Rodal_Id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = tipodeareadescripcion;
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse(Int64.Parse((oPoligonos[i].GTMX ?? 0).ToString("0")).ToString());
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse(Int64.Parse((oPoligonos[i].GTMY ?? 0).ToString("0")).ToString());
                }


                iniciofila += 1;
            }



            iniciofila += 6;
            iniciocolumna = 1;

            for (int i = 0; i < oAreaDescuento.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;
                tipodearea = oAreaDescuento[i].Tipo_de_Area;
                Tbl_Sol_Rodal_Tipo oTipoDeArea = (from d in db.Tbl_Sol_Rodal_Tipo
                                                  where d.Tipo_de_Area == tipodearea
                                                  select d).FirstOrDefault();
                tipodeareadescripcion = oTipoDeArea.Descripcion;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oAreaDescuento[i].Finca_id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oAreaDescuento[i].Rodal_Id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{oAreaDescuento[i].Rodal_Descuento_Id}");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = tipodeareadescripcion;
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse(Int64.Parse((oAreaDescuento[i].GTMX ?? 0).ToString("0")).ToString());

                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int64.Parse(Int64.Parse((oAreaDescuento[i].GTMY ?? 0).ToString("0")).ToString());
                }


                iniciofila += 1;
            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            //oEPP.SaveAs(new FileInfo($"{strNombre}"));
            oEPP.SaveAs(new FileInfo($"{destxlsx}"));


            #endregion

            return strNombre;
        }


        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Ubicacion { get; set; }
            public string Mensaje { get; set; }
        }

        public JsonResult DescargaDatosPoligonosyDescuento(long solicitud_id, string Guid_id)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            string TextoMostrar, guidid;
            long solicitudid;
            guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            string archivo = "";

            if (oSolicitud == null)
            {
                jsonRespuesta.Result = 0;
                jsonRespuesta.Ubicacion = null;
                jsonRespuesta.Mensaje = "Acceso denegado";
                return Json(JsonConvert.SerializeObject(jsonRespuesta));
            }
            if (oSolicitud.Guid_id != Guid_id)
            {
                jsonRespuesta.Result = 0;
                jsonRespuesta.Ubicacion = null;
                jsonRespuesta.Mensaje = "Acceso denegado";
                return Json(JsonConvert.SerializeObject(jsonRespuesta));
            }

            jsonRespuesta.Result = 1;
            jsonRespuesta.Ubicacion = GeneraDatosPoligonosyDescuento(oSolicitud); ;
            jsonRespuesta.Mensaje = "Archivo generado exitosamente";

            return Json(JsonConvert.SerializeObject(jsonRespuesta));

        }


        public Double DecimalToSgl_Dbl(decimal argument)
        {
            object SingleValue;
            Double DoubleValue;

            // Convert the argument to a float value.
            SingleValue = decimal.ToSingle(argument);

            // Convert the argument to a double value.
            DoubleValue = decimal.ToDouble(argument);

            return DoubleValue;
        }


        public void APIEvaluacion_AreaRodalDescuento(long finca_id, string tipo_area_id, long rodal_id, long rodal_descuento_id)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_API_Sol_Rodal_Descuento_Local sol_sol_rodal = db_API.Tbl_API_Sol_Rodal_Descuento_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id).First();

            IEnumerable<Tbl_API_Sol_Rodal_Descuento_Poligono_Local> tbl_sol_rodal_poligono = db_API.Tbl_API_Sol_Rodal_Descuento_Poligono_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;

            sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
            sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;

            try
            {
                db_API.Entry(sol_sol_rodal).State = EntityState.Modified;
                db_API.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;

        }

        public void APIEvaluacion_AreaRodal(long finca_id, string tipo_area_id, long rodal_id)
        {

            if (tipo_area_id != "Rodal")
            {
                return;
            }

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_API_Sol_Rodal_Local sol_sol_rodal = db_API.Tbl_API_Sol_Rodal_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_API_Sol_Rodal_Poligono_Local> tbl_sol_rodal_poligono = db_API.Tbl_API_Sol_Rodal_Poligono_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            //sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            //sol_sol_rodal.AreaTotalCalculadaSistema = Math.Round((decimal)(polyRodal.Area / 10000), 2);
            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;
            sol_sol_rodal.Longitud_Total = 0;

            sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
            sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;

            try
            {
                db_API.Entry(sol_sol_rodal).State = EntityState.Modified;
                db_API.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }

        public ActionResult DownloadDatosPoligonoTecnico(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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



            string guidid;
            guidid = Guid_id;


            ViewBag.Guid_id = guidid;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;


            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();

            ViewBag.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.Owner = 1;

            if (tbl_sol_solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            return View();
        }

        public ActionResult UploadDatosPoligonoTecnico(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        
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



            string guidid;
            guidid = Guid_id;


            ViewBag.Guid_id = guidid;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;


            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();

            ViewBag.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.Owner = 1;

            if (tbl_sol_solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            return View();
        }
        [HttpPost]
        public ActionResult UploadDatosPoligonoTecnico(HttpPostedFileBase uploadPoligono, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {


            bool ErrorEncontrado = false;
            TempData["MensajeFile"] = "";


            int intTipoCarga = 7;

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();


            if (ModelState.IsValid)
            {
                if (uploadPoligono != null && uploadPoligono.ContentLength > 0)
                {
                    Stream stream = uploadPoligono.InputStream;
                    IExcelDataReader reader = null;

                    if (uploadPoligono.FileName.EndsWith(".xls") || uploadPoligono.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();
                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA POLÍGONO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                            ErrorEncontrado = true;

                        }

                        DatoDeCampo = dt.Rows[8][3].ToString();


                        List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure> {
                                new ResultFromStoreProcedure {
                                    id = 0, mensaje= "Fallo desconocido.", respuesta = 0
                                }
                            };

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador = 0;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                            {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                            };



                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 12;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = System.Data.ParameterDirection.Input}
                                           };


                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        intContador = intContador + 1;
                                    }


                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";

                                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFile"] = resultado[0].mensaje;
                                            TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";
                                        }
                                        else
                                        {


                                            Tbl_Gral_Carga tbl_Gral_CargaB = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == intTipoCarga & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                                            if (tbl_Gral_CargaB != null)
                                            {
                                                TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>" + tbl_Gral_CargaB.Observaciones ?? "";
                                            }
                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFile"] = resultado[0].mensaje;
                                            TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                        }

                                    }
                                }

                                Tbl_Gral_Carga tbl_Gral_CargaC = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == intTipoCarga & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                                if (tbl_Gral_CargaC != null)
                                {
                                    TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" + tbl_Gral_CargaC.Observaciones ?? "";
                                }
                                else
                                {
                                    TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                }

                            }


                        }

                        //Inicio calculo de areas rodasles y áreas de descuento

                        if (resultado[0].respuesta == 1)
                        {
                            var Rodal_Api = db_API.Tbl_API_Sol_Rodal_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                            foreach (var ItemRodal in Rodal_Api)
                            {
                                APIEvaluacion_AreaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);
                            }

                            var Tbl_Sol_Rodales_Descuento = db_API.Tbl_API_Sol_Rodal_Descuento_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                            foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                            {
                                APIEvaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                            }


                        }


                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFile"] = "Ocurrió una excepción: " + Ex.InnerException.ToString() + " Mensaje: " + Ex.Message.ToString();//" El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";

                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }

            return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }

        public ActionResult UploadDatosPoligonoDescuentoTecnico(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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



            string guidid;
            guidid = Guid_id;


            ViewBag.Guid_id = guidid;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;


            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();

            ViewBag.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.Owner = 1;

            if (tbl_sol_solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            return View();
        }
        [HttpPost]
        public ActionResult UploadDatosPoligonoDescuentoTecnico(HttpPostedFileBase uploadDescuento, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {


            bool ErrorEncontrado = false;
            TempData["MensajeFile"] = "";


            int intTipoCarga = 7;

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                if (uploadDescuento != null && uploadDescuento.ContentLength > 0)
                {
                    Stream stream = uploadDescuento.InputStream;
                    IExcelDataReader reader = null;

                    if (uploadDescuento.FileName.EndsWith(".xls") || uploadDescuento.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();
                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA POLÍGONO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                            ErrorEncontrado = true;

                        }

                        DatoDeCampo = dt.Rows[8][3].ToString();


                        List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure> {
                                new ResultFromStoreProcedure {
                                    id = 0, mensaje= "Fallo desconocido.", respuesta = 0
                                }
                            };

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador = 0;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                            {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                            };



                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 12;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        intContador = intContador + 1;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }


                            }


                            int lastintContador = intContador + 100;

                            bool verificarValor = false;
                            try
                            {
                                while ((dt.Rows[intContador][0].ToString() != "No. Finca") && (!verificarValor))
                                {
                                    intContador = intContador + 1;
                                    if (intContador >= lastintContador)
                                    {
                                        verificarValor = true;
                                    }

                                }

                                if (dt.Rows[intContador][0].ToString() == "No. Finca")
                                {

                                    intTipoCarga = 8;
                                    sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                                    sqlParams = new SqlParameter[]
                                    {
                                    new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                    new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                                    new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                    new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                    new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                                    new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                                    };


                                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();
                                    if (resultado[0].respuesta == 1)
                                    {
                                        intContador += 1;
                                        lnCarga_id = resultado[0].id;

                                        try
                                        {
                                            while (dt.Rows[intContador][0].ToString() != "")
                                            {
                                                sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                                sqlParams = new SqlParameter[]
                                                   {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2].ToString()??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3].ToString()??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4].ToString()??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5].ToString()??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = System.Data.ParameterDirection.Input}
                                                   };


                                                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                                intContador = intContador + 1;
                                            }


                                            sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                            sqlParams = new SqlParameter[]
                                           {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                           };

                                        }
                                        catch (Exception ex)
                                        {
                                            if (intContador < 4)
                                            {
                                                TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                                //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                            }
                                            else
                                            {
                                                sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                                sqlParams = new SqlParameter[]
                                               {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                               };

                                                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                                if (resultado[0].respuesta != 1)
                                                {
                                                    TempData["MensajeFile"] = resultado[0].mensaje;
                                                }

                                            }
                                        }

                                        Tbl_Gral_Carga tbl_Gral_CargaB = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == intTipoCarga & Obj.Carga_id == lnCarga_id).FirstOrDefault();


                                        if (tbl_Gral_CargaB != null)
                                        {
                                            TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>" + tbl_Gral_CargaB.Observaciones ?? "";
                                        }
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                intContador = intContador + 1;
                                intContador = intContador - 1;
                            }
                        }

                        //Inicio calculo de areas rodasles y áreas de descuento

                        if (resultado[0].respuesta == 1)
                        {
                            var Rodal_Api = db_API.Tbl_API_Sol_Rodal_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                            foreach (var ItemRodal in Rodal_Api)
                            {
                                APIEvaluacion_AreaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);
                            }

                            var Tbl_Sol_Rodales_Descuento = db_API.Tbl_API_Sol_Rodal_Descuento_Local.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                            foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                            {
                                APIEvaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                            }


                        }


                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFile"] = "Ocurrió una excepción: " + Ex.InnerException.ToString() + " Mensaje: " + Ex.Message.ToString();//" El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";

                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                        return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }

            return RedirectToAction("AnalisisRodal", "Form_FormularioTecnico", new { Guid_id = Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }


        private void EstimacionVolumen_API(Tbl_API_Sol_Finca tbl_Sol_Finca)
        {

            //List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
            //                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
            //                                                                               select d).ToList();

            List<fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db_API.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
                                                                                               select d).ToList();


            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();
           



            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(11);
            decimal Total_areabasalha, volumenha, volumenrodal, Total_Areabasalm2, Total_Volumenlineam2;
            decimal SubTotal_Areabasalha, SubTotal_Volumenha, SubTotal_Volumenrodal, SubTotal_Areabasalm2, SubTotal_Volumenlineam2, SubTotal_Densidadha, Total_Densidadha, SubTotal_CantArboles, Total_CantArboles;
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            if (validacionesDiametrica.Count() > 0)
            {
                string varEspecie = "";
                string varEstimacion = "";
                long rodalid = 0;

                SubTotal_Areabasalha = 0;
                SubTotal_Volumenha = 0;
                SubTotal_Volumenrodal = 0;
                SubTotal_Areabasalm2 = 0;
                SubTotal_Volumenlineam2 = 0;
                SubTotal_Densidadha = 0;
                SubTotal_CantArboles = 0;

                Total_areabasalha = 0;
                volumenha = 0;
                volumenrodal = 0;
                Total_Areabasalm2 = 0;
                Total_Volumenlineam2 = 0;
                Total_Densidadha = 0;
                Total_CantArboles = 0;

                for (int i = 0; i < validacionesDiametrica.Count(); i++)
                {

                    if (validacionesDiametrica[i].Tipo_de_Area == 1)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea = 0;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;

                            Tbl_Sol_Rodal_Tipo tbl_Sol_Rodal_Tipo = (from d in db.Tbl_Sol_Rodal_Tipo
                                                                     where d.Tipo_de_Area == tipodearea
                                                                     select d).FirstOrDefault();
                            //Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();
                            Tbl_API_Sol_Rodal_Local tbl_Sol_Rodal = db_API.Tbl_API_Sol_Rodal_Local.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tbl_Sol_Rodal_Tipo.Descripcion).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? 0).ToString("0");
                            }

                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización de la plantación en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(gtmx, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(gtmy, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


                            // c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"ESPECIES A REGISTRAR", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 10;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Rodal", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Área ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Densidad por ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por Rodal", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.Border = 0;
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Area_Efectiva_Rodal ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);
                            #endregion
                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Densidad_ha).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_ha).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_Rodal).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                        c1.Border = 0;
                        c1.BackgroundColor = BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);
                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        SubTotal_Densidadha += (decimal)validacionesDiametrica[i].Densidad_ha;

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha += SubTotal_Areabasalha;
                                volumenha += SubTotal_Volumenha;
                                volumenrodal += SubTotal_Volumenrodal;
                                Total_Densidadha += SubTotal_Densidadha;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_Densidadha = 0;
                                SubTotal_CantArboles = 0;

                            }
                        }
                        catch
                        {

                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            Total_areabasalha += SubTotal_Areabasalha;
                            volumenha += SubTotal_Volumenha;
                            volumenrodal += SubTotal_Volumenrodal;
                            Total_Densidadha += SubTotal_Densidadha;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_Densidadha = 0;
                            SubTotal_CantArboles = 0;

                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(volumenha.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(volumenrodal.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;

                                Total_Densidadha = 0;
                                Total_CantArboles = 0;

                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(volumenha.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(volumenrodal.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;

                            Total_Densidadha = 0;
                            Total_CantArboles = 0;

                        }
                    }
                    else if (validacionesDiametrica[i].Tipo_de_Area == 2)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea = 0;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;

                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;

                            Tbl_Sol_Rodal_Tipo tbl_Sol_Rodal_Tipo = (from d in db.Tbl_Sol_Rodal_Tipo
                                                                     where d.Tipo_de_Area == tipodearea
                                                                     select d).FirstOrDefault();
                            //Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();
                            Tbl_API_Sol_Rodal_Local tbl_Sol_Rodal = db_API.Tbl_API_Sol_Rodal_Local.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tbl_Sol_Rodal_Tipo.Descripcion).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? 0).ToString("0");
                            }


                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización de la plantación en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(gtmx, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(gtmy, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


//                            c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"ESPECIES A REGISTRAR", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id. de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Longitud (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de Plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Cantidad de Árboles", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Longitud_Total ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            #endregion
                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Cantidad_Arboles ?? 0).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_X_Linea).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);
                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        SubTotal_CantArboles += (decimal)validacionesDiametrica[i].Cantidad_Arboles;

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                                Total_Areabasalm2 += SubTotal_Areabasalm2;
                                Total_CantArboles += SubTotal_CantArboles;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_CantArboles = 0;
                                SubTotal_Densidadha = 0;
                            }
                        }
                        catch
                        {
                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                            Total_Areabasalm2 += SubTotal_Areabasalm2;
                            Total_CantArboles += SubTotal_CantArboles;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_CantArboles = 0;
                            SubTotal_Densidadha = 0;
                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);
                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;
                                Total_CantArboles = 0;
                                Total_Densidadha = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);
                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;
                            Total_CantArboles = 0;
                            Total_Densidadha = 0;
                        }
                    }


                }


                tableEstimacion.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }



        public ActionResult Revisar(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            db_RNF_APIEntities db_Api = new db_RNF_APIEntities();

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

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();
            try
            {
                ViewBag.CountRodalesDescuento = db_Api.Tbl_API_Sol_Rodal_Descuento_Local.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).Count();
            }
            catch
            {
                ViewBag.CountRodalesDescuento = 0;
            }

            ViewBag.listModelB = db_Api.Tbl_API_Sol_Rodal_Descuento_Local.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).ToList();

            List<Tbl_API_Sol_Rodal_Local> lst = new List<Tbl_API_Sol_Rodal_Local>();

            string sqlQuery;
            SqlParameter[] sqlParams;
            sqlQuery = " select \n"; 
            sqlQuery += "   * \n";
            sqlQuery += " from \n";
            sqlQuery += "   Tbl_API_Sol_Rodal_Local \n";
            sqlQuery += " where Solicitud_id = @Solicitud_id \n";

            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_Solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input }
            };

            lst = db_API.Database.SqlQuery<Tbl_API_Sol_Rodal_Local>(sqlQuery, sqlParams).ToList();
            //db_API.Tbl_API_Sol_Rodal_Poligono.Where(Obj => Obj.Solicitud_id == tbl_API_Sol_Solicitud.Solicitud_id).ToList();

            ViewBag.RodalesDescuento = db_API.Tbl_API_Sol_Rodal_Descuento_Local.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id);

            return View(lst);

        }




        [HttpPost]
        public JsonResult DescargarDasometricos(string Guid_id)
        {

           

            string TextoMostrar, documento, guidid;
            long solicitudid;
            guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            documento = GeneraDasometricos(solicitudid);
            TextoMostrar = "{ \"Ubicacion\" : \"" + documento + "\"}";
            return Json(TextoMostrar);
        }

        public ActionResult UploadDatosDasometricosTecnico(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            ViewBag.Guid_id = guidid;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            ViewBag.Categoria_id = tbl_Sol_Solicitud.Categoria_id;

            ViewBag.solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.Owner = 1;

            if (tbl_Sol_Solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadDatosDasometricosTecnico(HttpPostedFileBase upload, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

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

            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);
            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                  where d.Guid_id == guidid
                                                  select d).FirstOrDefault();


            ViewBag.Owner = 1;

            if (tbl_sol_solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        //return RedirectToAction("../Form_FormularioTecnico/UploadDatosDasometricosTecnico");
                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DASOMETRICOS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                        }


                        DatoDeCampo = dt.Rows[8][4].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            string @strCampo12 = "0";
                            string @strCampo13 = "0";
                            string @strCampo5 = "0";
                            string @strCampo6 = "0";

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";


                                        if (tbl_sol_solicitud.Categoria_id == 6)
                                        {
                                            try
                                            {
                                                @strCampo12 = (dt.Rows[intContador][11] ?? "").ToString().Trim();
                                            }
                                            catch (Exception Ex)
                                            {
                                                @strCampo12 = "0";
                                            }

                                            try
                                            {
                                                @strCampo13 = (dt.Rows[intContador][12] ?? "").ToString().Trim();
                                            }
                                            catch (Exception Ex)
                                            {
                                                @strCampo13 = "0";
                                            }


                                        }

                                        try
                                        {
                                            @strCampo5 = (dt.Rows[intContador][4] ?? "").ToString().Trim();
                                            if (@strCampo5 == "")
                                            {
                                                @strCampo5 = "0";
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            @strCampo5 = "0";
                                        }

                                        try
                                        {
                                            @strCampo6 = (dt.Rows[intContador][5] ?? "").ToString().Trim();
                                            if (@strCampo6 == "")
                                            {
                                                @strCampo6 = "0";
                                            }

                                        }
                                        catch (Exception Ex)
                                        {
                                            @strCampo6 = "0";
                                        }



                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = @strCampo5, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = @strCampo6, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = (dt.Rows[intContador][6]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = (dt.Rows[intContador][7]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = (dt.Rows[intContador][8]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = (dt.Rows[intContador][9]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = (dt.Rows[intContador][10]??"").ToString().Trim(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = @strCampo12, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = @strCampo13, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = ParameterDirection.Input}
                                           };


                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        intContador = intContador + 1;
                                    }
                                } 
                                catch (Exception ex)
                                {
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = ParameterDirection.Input}
                                       };
                                        db.Database.CommandTimeout = 3000;

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                       

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                        else
                                        {

                                            Tbl_Gral_Carga tbl_Gral_Carga = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == 6 & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                                            if (tbl_Gral_Carga != null)
                                            {
                                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" + tbl_Gral_Carga.Observaciones ?? "";
                                            }
                                            else
                                            {
                                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                            }


                                            //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";

                                return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                            }
                        }
                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";

                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = "Ocurrió una excepción: " + (Ex.InnerException.ToString()??"") + " Mensaje: " + Ex.Message.ToString();//TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";

                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
            //return RedirectToAction("../Sol_Rodal/UploadExcel");
            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadDatosDasometricosTecnico_(HttpPostedFileBase upload, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

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

            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);
            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                  where d.Guid_id == guidid
                                                  select d).FirstOrDefault();


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        //return RedirectToAction("../Form_FormularioTecnico/UploadDatosDasometricosTecnico");
                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DASOMETRICOS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                        }


                        DatoDeCampo = dt.Rows[8][4].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = dt.Rows[intContador][0].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = dt.Rows[intContador][1].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = dt.Rows[intContador][2].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = dt.Rows[intContador][3].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = dt.Rows[intContador][4].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = dt.Rows[intContador][5].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = dt.Rows[intContador][6].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = dt.Rows[intContador][7].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = dt.Rows[intContador][8].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = dt.Rows[intContador][9].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = dt.Rows[intContador][10].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = dt.Rows[intContador][11].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = dt.Rows[intContador][12].ToString(), Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = ParameterDirection.Input}
                                           };


                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        intContador = intContador + 1;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 6, Direction = ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                        else
                                        {
                                            //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                            }
                        }
                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                        return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            //return RedirectToAction("../Sol_Rodal/UploadExcel");
            return RedirectToAction(actionName: "Index", controllerName: "App_Visita_Campo", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }

        public ActionResult UploadDatosCentroParcelaTecnico(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            ViewBag.Guid_id = guidid;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            ViewBag.solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.Owner = 1;

            if (tbl_Sol_Solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadDatosCentroParcelaTecnico(HttpPostedFileBase upload, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Session[Constants.session_Tabulador] = "defaultParcela";

            bool ErrorEncontrado = false;
            int intTipoCarga = 13;
            TempData["MensajeFileCentroParcela"] = "";

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

            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);
            Tbl_Sol_Solicitud tbl_sol_solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == guidid
                                                   select d).FirstOrDefault();




            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DE CENTRO DE PARCELAS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                        }

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            string @strCampo12 = "0";
                            string @strCampo13 = "0";

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = dt.Rows[intContador][0].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = dt.Rows[intContador][1].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = dt.Rows[intContador][2].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = dt.Rows[intContador][3].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = dt.Rows[intContador][4].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = 0, Direction = System.Data.ParameterDirection.Input},

                                                 new SqlParameter { ParameterName = "@Campo11",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = System.Data.ParameterDirection.Input},

                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = System.Data.ParameterDirection.Input}

                                           };


                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        intContador = intContador + 1;
                                    }


                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFileCentroParcela"] = "Error: La cantidad de muestras es muy pequeña.";
                                        return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileCentroParcela"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileCentroParcela"] = "Carga realizada con exito.";
                                            return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                        else
                                        {
                                            return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });

                            }
                        }
                        return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });


                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction(actionName: "AnalisisCentroParcela", controllerName: "Form_FormularioTecnico", routeValues: new { Guid_id = guidid, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }



        public ActionResult NoAplicaEvaluacionTecnicaFormularioRodalDasometricos()
        {

            return View();
        }

        public ActionResult EvaluacionTecnicaFormularioEmpresa(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string ModoVista)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                return RedirectToAction("../Home/AccesoDenegado");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();


            Session[Constants.session_Solicitud] = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            ViewBag.GuidEtapa_id = GuidEtapa_id;

            byte[] newQR;
            string text = tbl_Sol_Solicitud.Guid_id;
            newQR = qrCodeBytes(text);
            long ticks = DateTime.Now.Ticks;
            string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");
            ViewBag.BytesQR = newQR;
            System.IO.File.WriteAllBytes(rootqr, newQR);

            ViewBag.Owner = 1;

            if (tbl_Sol_Solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {

                ViewBag.Owner = 0;

            }

            ViewBag.ModoVista = ModoVista;

            return View(tbl_Sol_Solicitud);

        }



        public ActionResult EvaluacionTecnicaFormularioEmpresaRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

            //  Opciones posibles                                                                         //
            //  ==================                                                                        //
            //  1    Aprobación                                                          //


            return View(tbl_gest_EtapaSolicitud);

        }



        public ActionResult EvaluacionTecnicaFormularioRodalDasometricos(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string ModoVista)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                return RedirectToAction("../Home/AccesoDenegado");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj=> Obj.Guid_id == Guid_id).First();

            int[] Categoria_id = { 5, 7, 8, 9, 11 };

            if (Categoria_id.Contains((int)tbl_Sol_Solicitud.Categoria_id))
            {

                if ((tbl_Sol_Solicitud.Categoria_id == 5) || (tbl_Sol_Solicitud.Categoria_id == 8) && (tbl_Sol_Solicitud.Sub_Categoria_id == 2) || (tbl_Sol_Solicitud.Categoria_id == 9) || (tbl_Sol_Solicitud.Categoria_id == 11))
                {
                    return RedirectToAction("EvaluacionTecnicaFormularioEmpresa", "Form_FormularioTecnico", new { GuidEtapa_id= GuidEtapa_id, Guid_id= Guid_id, etapa_id = etapa_id, etaparuta_id= etaparuta_id, correlativoetapa_id = correlativoetapa_id, ModoVista = ModoVista });
                }

                return RedirectToAction("../Form_FormularioTecnico/NoAplicaEvaluacionTecnicaFormularioRodalDasometricos");
            }

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            ViewBag.GuidEtapa_id = GuidEtapa_id;

            byte[] newQR;
            string text = tbl_Sol_Solicitud.Guid_id;
            newQR = qrCodeBytes(text);
            long ticks = DateTime.Now.Ticks;
            string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");
            ViewBag.BytesQR = newQR;
            System.IO.File.WriteAllBytes(rootqr, newQR);

            ViewBag.Owner = 1;

            if (tbl_Sol_Solicitud.TecnicoAsignado_id != objUs.intUsuario_id)
            {

                ViewBag.Owner = 0;

            }

            ViewBag.ModoVista = ModoVista;

            return View(tbl_Sol_Solicitud);

        }

        public ActionResult EvaluacionTecnicaFormularioRodalDasometricosRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

            //  Opciones posibles                                                                         //
            //  ==================                                                                        //
            //  1    Aprobación                                                          //


            return View(tbl_gest_EtapaSolicitud);

        }



        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaFormularioRodalDasometricos
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

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == EtapaSolicitud_GUIDid).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado = tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado ?? "";

            bool bl_AplicaFirmaFormulario = true;
            bool bl_AplicaAreas = true;
            bool bl_AplicaDasometricos = true;
            bool bl_AplicaCultivoEnAsocio = false;

            int intVerificar = 0;
            int intIngresado = 0;
            int intVivero = 0;

            try
            {
                intVerificar = tbl_sol_solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Sol_Empresa_Entidad_Actividad.Count();
                intIngresado = db.Tbl_Sol_Empresa_Entidad_TecnicoActividad.Where(Obt => Obt.Solicitud_id == solicitud_id).Count();
                intVivero = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Categoria_id == 11).Count();  // VIVEROS
            }
            catch
            {
                intVerificar = 0;
                intIngresado = 0;
            }

            if ((tbl_sol_solicitud.Categoria_id == 5) && (tbl_sol_solicitud.Sub_Categoria_id == 1))
            {


                if ((intVerificar > 0) && (intIngresado == 0) && (intVivero == 0))
                {
                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin realizar el ingreso de confirmación de actividad por el lado técnico.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

            }

            try
            {
                intVerificar = tbl_sol_solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada.Count();
                intIngresado = db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Where(Obt => Obt.Solicitud_id == solicitud_id).Count();

            }
            catch
            {
                intVerificar = 0;
                intIngresado = 0;
            }

            if ((tbl_sol_solicitud.Categoria_id == 5) && (tbl_sol_solicitud.Sub_Categoria_id == 1))
            {

                if ((intVerificar > 0) && (intIngresado == 0))
                {
                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin realizar el ingreso de confirmación de la maquinaria utilizada por el lado técnico.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }
            }


            try
            {
                intVerificar = tbl_sol_solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Sol_Empresa_Entidad_Materia_Prima.Count();
                intIngresado = db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Where(Obt => Obt.Solicitud_id == solicitud_id).Count();
            }
            catch
            {
                intVerificar = 0;
                intIngresado = 0;
            }



            if ((intVerificar > 0) && (intIngresado == 0))
            {
                codRespuesta = 0;
                strRespuesta = "No puede avanzar sin realizar el ingreso de confirmación de la materia prima utilizada por el lado técnico.";


                jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);

            }


            try
            {
                intVerificar = tbl_sol_solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Count();
                intIngresado = db.Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal.Where(Obt => Obt.Solicitud_id == solicitud_id).Count();
            }
            catch
            {
                intVerificar = 0;
                intIngresado = 0;
            }
        


            if ((intVerificar > 0) && (intIngresado == 0))
            {
                codRespuesta = 0;
                strRespuesta = "No puede avanzar sin realizar el ingreso de confirmación de los detalles del vivero forestal por el lado técnico.";


                jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);

            }


            if ((tbl_sol_solicitud.SolicitudTipo_id == (decimal)1.00) || (tbl_sol_solicitud.SolicitudTipo_id == (decimal)5.00) || (tbl_sol_solicitud.SolicitudTipo_id == (decimal)11.00))
            {
                bl_AplicaFirmaFormulario = false;
                bl_AplicaAreas = false;
                bl_AplicaDasometricos = false;

            }

            if ((tbl_sol_solicitud.SolicitudTipo_id == (decimal)3.00) || (tbl_sol_solicitud.SolicitudTipo_id == (decimal)6.00))
            {
                bl_AplicaFirmaFormulario = false;
            }

            if ((tbl_sol_solicitud.SolicitudTipo_id >= (decimal)4.00) && (tbl_sol_solicitud.SolicitudTipo_id < (decimal)4.04))
            {
                bl_AplicaCultivoEnAsocio = true;
            }



            if ((tbl_sol_solicitud.SolicitudTipo_id - tbl_sol_solicitud.Categoria_id) > 0)
            {
                bl_AplicaFirmaFormulario = false;
                bl_AplicaAreas = false;
                bl_AplicaDasometricos = false;
            }


            if (bl_AplicaDasometricos == true && intVivero ==0)
            {
                if ((db_API.Tbl_API_Sol_Rodal_Dasometrico_Local.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Count() == 0) && (bl_AplicaDasometricos == true))
                {

                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin realizar el análisis de datos dasométricos.";

                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);
                }
            }

            if (bl_AplicaAreas == true && intVivero == 0)
            {

                if ((db_API.Tbl_API_Sol_Rodal_Local.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Count() == 0) && (bl_AplicaAreas == true))
                {

                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin realizar el análisis de áreas.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);
                }

                if ((bl_AplicaAreas == true) && (tbl_sol_solicitud.PonderacionEvaluacionTecnicaDeArea_id == null))


                {

                    //Crejo  Funcion que indica cuantos registros vienen diferentes entre el Area de los poligonos con el Area Dasometrico
                    int CantidadEspecies = db_API.Database.SqlQuery<int>("SELECT  dbo.fc_Sol_CompararAreaDasomePoligono(@p0)", tbl_sol_solicitud.Solicitud_id).FirstOrDefault();
                   

                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin indicar la valoración proporcionada en el análisis de áreas .";

                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }
            }

            if ((bl_AplicaDasometricos == true) && (tbl_sol_solicitud.PonderacionEvaluacionTecnicaDasometrica_id == null) && (intVivero == 0))
            {
                codRespuesta = 0;
                strRespuesta = "No puede avanzar sin indicar la valoración en el área de dasométricos.";

                jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);

            }

            if ((bl_AplicaAreas == true) && (bl_AplicaDasometricos == true) && (tbl_sol_solicitud.PonderacionEvaluacionTecnicaDasometrica_id != null) && (tbl_sol_solicitud.PonderacionEvaluacionTecnicaDeArea_id != null) &&  (intVivero == 0))
            {

                //Crejo  Funcion que indica cuantos registros vienen diferentes entre el Area de los poligonos con el Area Dasometrico

                //int ContadorComparacion = db_API.Database.SqlQuery<int>("SELECT  dbo.SP_Sol_CompararAreaDasomePoligono(@p0)", tbl_sol_solicitud.Solicitud_id).FirstOrDefault();

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure> {
                                new ResultFromStoreProcedure {
                                    id = 0, mensaje= "Fallo desconocido.", respuesta = 0
                                }
                            };


                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Sol_CompararAreaDasomePoligono @Solicitud_id";

                sqlParams = new SqlParameter[]
               {
                                             new SqlParameter { ParameterName = "@Solicitud_id", Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input}
                                         
               };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                if (resultado[0].respuesta > 0)
                    
                {
                    codRespuesta = 0;
                    strRespuesta = "El area en Dasometricos no concuerda con el Area en Rodales.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);
                }


            }

            if (bl_AplicaCultivoEnAsocio == true && intVivero == 0)
            {
                if ((db.Tbl_Sol_Rodal_CultivoTecnico.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Count() == 0) && (bl_AplicaCultivoEnAsocio == true))
                {

                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin realizar el ingreso de cultivos en asocio. En el área de datos dasométricos.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);
                }
            }

            if (bl_AplicaFirmaFormulario == true && intVivero == 0)
            { 
                if ((tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado.ToString().Trim() == "") && (bl_AplicaFirmaFormulario == true))
                {

                    codRespuesta = 0;
                    strRespuesta = "No puede avanzar sin firmar el formulario de análisis.";


                    jsonResult = "{\"CodRespuesta\":"
                                    + "\"" + codRespuesta + "\","
                                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);
                }
            }
            // bl_AplicaAreas

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



        public Byte[] qrCodeBytes(string Guid_id)
        {
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
            byte[] newcode;
            QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(Guid_id, QRCodeGenerator.ECCLevel.Q);

            BitmapByteQRCode bitmapByteQRCode = new BitmapByteQRCode(qRCodeData);
            QRCode qrCode = new QRCode(qRCodeData);

            Bitmap bitmap = new Bitmap(qrCode.GetGraphic(20));
            MemoryStream ms = new MemoryStream();

            bitmap.Save(ms, ImageFormat.Png);

            byte[] byteImage = ms.ToArray();
            newcode = byteImage;

            return newcode;
        }


        public ActionResult PonderacionEvaluacionTecnico(string Guid_id)
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


            string guidid;
            guidid = Guid_id;
            ViewBag.guidid = Guid_id;
            Tbl_Sol_Solicitud oPonderacion = (from d in db.Tbl_Sol_Solicitud
                                              where d.Guid_id == guidid
                                              select d).FirstOrDefault();
            if (oPonderacion.PonderacionEvaluacionTecnicaFormulario_id == null)
            {
                oPonderacion.PonderacionEvaluacionTecnicaFormulario_id = 0;
            }
            ViewBag.PonderacionEvaluacionTecnicaFormulario_id = new SelectList(db.Tbl_Gral_EvaluacionPonderacion, "PonderacionEvaluacion_id", "Descripcion", oPonderacion.PonderacionEvaluacionTecnicaFormulario_id);




            ViewBag.Owner = 1;

            if (oPonderacion.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }


            return View(oPonderacion);
        }

        [HttpPost]
        public JsonResult EvaluacionFormularioTecnico(string Guid_id, int Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PonderacionEvaluacionTecnicaFormulario_id = Opcion;
            db.SaveChanges();
            return Json(null);
        }

        public ActionResult PonderacionEvaluacionArea(string Guid_id)
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

            Session[Constants.session_Solicitud] = oPonderacion.Solicitud_id;


            ViewBag.Owner = 1;

            if (oPonderacion.TecnicoAsignado_id != objUs.intUsuario_id)
            {
                ViewBag.Owner = 0;
            }

            ViewBag.solicitud_id = oPonderacion.Solicitud_id;

            ViewBag.guidid = oPonderacion.Guid_id;

            return View(oPonderacion);
        }

        [HttpPost]
        public JsonResult EvaluacionArea(string Guid_id, int Opcion)
        {
            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            oSolicitud.PonderacionEvaluacionTecnicaDeArea_id = Opcion;
            db.SaveChanges();
            return Json(null);
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




        public string GeneraCentroParcela(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            long solicitudid;
            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre, strDirArchivo;
            solicitudid = tbl_Sol_Solicitud.Solicitud_id;
            string rootbase, rootpath, rootdest, rootnew, machote, destfinal, nombrereporte, destfile, destxlsx;
            int iniciofila, iniciocolumna, finalfila, finalcolumna;
            ExcelPackage oEPP;


            #region Preparación de Datos

            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Content/Machotes/";
            rootnew = $"{rootbase}Documentos/";
            machote = $"{rootpath}DatosCentroParcela_Vacio.xlsx";
            rootdest = $"{rootpath}";
            nombrereporte = $"DatosCentroParcela_{tbl_Sol_Solicitud.Guid_id}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";


            List<Tbl_Sol_Rodal_Dasometrico_CentroParcela> tbl_Sol_Rodal_Dasometrico_CentroParcelas = new List<Tbl_Sol_Rodal_Dasometrico_CentroParcela>();
            tbl_Sol_Rodal_Dasometrico_CentroParcelas = (from d in db.Tbl_Sol_Rodal_Dasometrico_CentroParcela
                                                        where d.Solicitud_id == solicitudid
                                                        orderby d.Finca_id, d.Rodal_id, d.No_Parcela
                                                        select d).ToList();

            //List<Tbl_Sol_Rodal_Dasometrico> oDasometricos = new List<Tbl_Sol_Rodal_Dasometrico>();
            //oDasometricos = (from d in db.Tbl_Sol_Rodal_Dasometrico
            //                 where d.Solicitud_id == solicitudid
            //                 orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela, d.Dasometrico_id
            //                 select d).ToList();

            #endregion


            strNombre = $"..//..//Documentos//{nombrereporte}.xlsx";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            #region Manipulacion de Archivo XLSX
            oEPP = new ExcelPackage(new FileInfo(machote));
            ExcelWorksheet wSheet1;
            wSheet1 = oEPP.Workbook.Worksheets[0];

            iniciofila = finalfila = 11;
            iniciocolumna = finalcolumna = 1;
            int startrow, startcol;
            for (int i = 0; i < tbl_Sol_Rodal_Dasometrico_CentroParcelas.Count(); i++)
            {
                startcol = iniciocolumna;
                startrow = iniciofila;

                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{tbl_Sol_Rodal_Dasometrico_CentroParcelas[i].Finca_id}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{tbl_Sol_Rodal_Dasometrico_CentroParcelas[i].Rodal_id}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{tbl_Sol_Rodal_Dasometrico_CentroParcelas[i].No_Parcela}";
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (tbl_Sol_Rodal_Dasometrico_CentroParcelas[i].GTMX ?? 0).ToString("0");
                }

                startcol += 1;
                using (ExcelRange rango = wSheet1.Cells[startrow, startcol, startrow, startcol])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = (tbl_Sol_Rodal_Dasometrico_CentroParcelas[i].GTMY ?? 0).ToString("0");
                }



                iniciofila += 1;
            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            //oEPP.SaveAs(new FileInfo($"{strNombre}"));
            oEPP.SaveAs(new FileInfo($"{destxlsx}"));

            #endregion

            return strNombre;
        }

        public JsonResult DescargarCentroParcela(string Guid_id)
        {
            string TextoMostrar, documento, guidid;
            long solicitudid;
            guidid = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            documento = GeneraCentroParcela(oSolicitud);
            TextoMostrar = "{ \"Ubicacion\" : \"" + documento + "\"}";
            return Json(TextoMostrar);
        }




        public JsonResult DefinirTipoDeAreaSIGAP(long Solicitud_id, int Finca_id, string Tipo_de_Area, int Rodal_Id, int TipoDeAreaSigap)
        {

            JsonRespuesta jsonRespuesta = new JsonRespuesta
            {
                Result = 0,
                Mensaje = "No se ha realizado ninguna gestión",
            };

            try
            {
            Tbl_API_Sol_Rodal_Local tbl_API_Sol_Rodal_Local = db_API.Tbl_API_Sol_Rodal_Local.Where(Obj => Obj.Solicitud_id == Solicitud_id && Obj.Finca_id == Finca_id && Obj.Tipo_de_Area == Tipo_de_Area && Obj.Rodal_Id == Rodal_Id).First();

     
                tbl_API_Sol_Rodal_Local.CategoriaSIGAP_Id = TipoDeAreaSigap;

                db_API.Entry(tbl_API_Sol_Rodal_Local).State = EntityState.Modified;
                db_API.SaveChanges();

                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = "Registro actualizado";


            }
            catch (Exception ex)
            {

                jsonRespuesta.Mensaje = ex.Message;

            }


            return Json(jsonRespuesta);

        }



        [HttpPost]
        public JsonResult DefinirAreaSIGAP(long Solicitud_id, int Finca_id, string Tipo_de_Area, int Rodal_Id, decimal Medida)
        {

            JsonRespuesta jsonRespuesta = new JsonRespuesta
            {
                Result = 0,
                Mensaje = "No se ha realizado ninguna gestión"
            };

            Tbl_API_Sol_Rodal_Local tbl_API_Sol_Rodal_Local = db_API.Tbl_API_Sol_Rodal_Local.Where(Obj => Obj.Solicitud_id == Solicitud_id && Obj.Finca_id == Finca_id && Obj.Tipo_de_Area == Tipo_de_Area && Obj.Rodal_Id == Rodal_Id ).First();

            if ((tbl_API_Sol_Rodal_Local.CategoriaSIGAP_Id??0) == 0)
             {
                jsonRespuesta.Result = 0;
                jsonRespuesta.Mensaje = "No puede asignar valor, sin haber definido el área SIGAP";

            }
            else
            {
                try
                {

                tbl_API_Sol_Rodal_Local.CategoriaSIGAP_AreaLongitud = Medida;

                db_API.Entry(tbl_API_Sol_Rodal_Local).State = EntityState.Modified;
                db_API.SaveChanges();

                    jsonRespuesta.Result = 1;
                    jsonRespuesta.Mensaje = "Registro actualizado con éxito";

                }
                catch (Exception ex)
                {

                    jsonRespuesta.Result = 0;
                    jsonRespuesta.Mensaje = ex.Message;

                }

            }


            return Json(jsonRespuesta);

        }


    }
}


using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Image = iTextSharp.text.Image;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Data.SqlClient;
using OfficeOpenXml.Drawing.Chart;
using Microsoft.Office.Interop.Word;
using System.Windows.Media;

namespace RNF_Web.Controllers
{
    public class Form_FormularioDirectorRegionalController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();

        private PdfPTable tableCarnetMotosierra = new PdfPTable(7);

        private string StrMotosierraPropietario;

        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosNotificacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosFinca = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosPlantacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        private PdfPTable tablePersoneria = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);

        private iTextSharp.text.Font fntTituloTabla_MTS = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_MT = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_10 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_11 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_12 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_13 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_14 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_15 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_10B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_11B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_12B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_13B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_14B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_15B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);


        public ActionResult SolicitudAutorizar(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where
                                                                  d.Solicitud_Guid_id == Guid_id
                                                               && d.EtapaSolicitud_GUID_id == GuidEtapa_id
                                                               select d).FirstOrDefault();


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();

            List<Tbl_RNF_Registro_InactivacionTecnico_Tipo> tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in db.Tbl_RNF_Registro_InactivacionTecnico_Tipo select d).ToList();

            tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                          where d.InactivacionTecnicoTipo_id != 1
                                                          select d).ToList();
                                                          

            ViewBag.TipoInactivacion_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == oSolicitud.Categoria_id && Obj.UsuarioInterno == true).OrderBy(Obj => Obj.TipoInactivacion_id).ToList(), "TipoInactivacion_id", "Descripcion", (oSolicitud.TipoInactivacion_id ?? 0));

            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion", (oSolicitud.InactivacionTecnicoTipo_id ?? tbl_RNF_Registro_InactivacionTecnico_Tipos.FirstOrDefault().InactivacionTecnicoTipo_id));

            bool EsInactivacion = false;
            decimal TipoGestion = oSolicitud.SolicitudTipo_id - Math.Truncate(oSolicitud.SolicitudTipo_id);
            decimal solicitudtipoentero = Math.Truncate(oSolicitud.SolicitudTipo_id);
            decimal terminacioninactivacion = 0.04M;

            ViewBag.tbl_Sol_Solicitud = oSolicitud;

            if (TipoGestion == terminacioninactivacion)
            {
                EsInactivacion = true;
            }

            ViewBag.EsInactivacion = EsInactivacion;

            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == oSolicitud.Categoria_id).ToList();
            if (oSolicitud.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == oSolicitud.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");


            return View(tbl_Gest_EtapaSolicitud);

        }

        public ActionResult SolicitudAutorizarRespuesta(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            return SolicitudAutorizar(GuidEtapa_id, Guid_id, etapa_id, etaparuta_id, correlativoetapa_id);
        }


        public ActionResult SolicitudAutorizarMotosierra(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where
                                                                  d.Solicitud_Guid_id == Guid_id
                                                               && d.EtapaSolicitud_GUID_id == GuidEtapa_id
                                                               select d).FirstOrDefault();
            return View(tbl_Gest_EtapaSolicitud);
        }



        private string GenerarQR(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            string linkServer = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault().Direccion_URL;

            string encRegistro = SecurEncryptDecrypt.EncryptString(tbl_RNF_Registro.No_Registro);

            string actionServer = Url.Action("VisualizarInscripcion", "RNF_Registro", new { encRegistro = encRegistro });
            //Uri queryparams = new Uri(linkServer + actionServer);
            Uri queryparams = new Uri(new Uri(linkServer), actionServer);
            //Uri queryparams = new Uri("https://consultarnf.inab.gob.gt/");


            byte[] newQR;
            //string ippuerto = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault().Direccion_URL;
            string text = queryparams.AbsoluteUri;// ippuerto + "/Documentos/" + tbl_RNF_Registro.Guid_id + ".pdf";
            newQR = qrCodeBytes(text);
            long ticks = DateTime.Now.Ticks;
            string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");

            System.IO.File.WriteAllBytes(rootqr, newQR);

            return rootqr;
        }



        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 15, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }


        private void LlenaBanner(String Leyenda, string alineacion, string Color)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 15, FontColour);

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

        private void LlenaSubTituloRevision_RNF(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_10;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_10;

            tableTitulo = new PdfPTable(7);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));

            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("REGISTRO NACIONAL FORESTAL", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);




            //    RNF_Registro VisualizarInscripcion
            string rootqr = GenerarQR(tbl_RNF_Registro);




            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rootqr);

            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);

            c1.Colspan = 1;
            c1.Rowspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            // Ventana emergente

          

            ///// Primera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("GUATEMALA, C.A.", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            ///// Segunda Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("CONSTANCIA DE REGISTRO", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

        

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            //// Tercera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("" + tbl_RNF_Registro.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            //c1.HorizontalAlignment = Element.ALIGN_CENTER;
            //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            //c1.Colspan = 2;
            //c1.Rowspan = 1;
            //c1.Border = 0;


            //tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            //// Tercera Linea

            //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            //c1.HorizontalAlignment = Element.ALIGN_CENTER;
            //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            //c1.Colspan = 2;
            //c1.Rowspan = 1;
            //c1.Border = 0;


            //tableTitulo.AddCell(c1);


          




            return;
        }





        //private void LlenaBannerMotosierra(Tbl_RNF_Registro tbl_RNF_Registro)
        //{
        //    iTextSharp.text.Font fntTituloTabla = fntTituloTabla_MT;
        //    iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_MT;

        //    tableCarnetMotosierra = new PdfPTable(11);

        //    PdfPCell c1 = new PdfPCell();


        //    string rootqr = GenerarQR(tbl_RNF_Registro);



        //    c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

        //    c1.Border = 1;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;
        //    c1.BorderWidthTop = 1;

        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;
        //    c1.BorderWidthTop = 1;
        //    c1.Colspan = 5;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;
        //    c1.BorderWidthTop = 1;


        //    tableCarnetMotosierra.AddCell(c1);



        //    /// Linea No. 3


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;

        //    c1.Border = 0;

        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("REGISTRO NACIONAL FORESTAL", fntTituloTabla));


        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;
        //    c1.BorderWidthTop = 1;
        //    c1.BorderWidthLeft = 1;
        //    c1.BorderWidthRight = 1;
        //    c1.Colspan = 5;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);



        //    /// Linea No. 3


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;

        //    c1.Border = 0;

        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase(tbl_RNF_Registro.No_Registro, fntTituloTabla));


        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;
        //    c1.BorderWidthLeft = 1;
        //    c1.BorderWidthRight = 1;
        //    c1.Colspan = 5;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);



        //    /// Linea No. 2


        //    /// Linea No. 4



        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 4;

        //    c1.Border = 0;

        //    tableCarnetMotosierra.AddCell(c1);

        //    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rootqr);

        //    logo.ScalePercent(7f);

        //    c1 = new PdfPCell(logo);

        //    c1.Border = 0;
        //    c1.BorderWidthLeft = 1;
        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 4;


        //    tableCarnetMotosierra.AddCell(c1);

        //    DateTime fecha = (DateTime)tbl_RNF_Registro.Fecha_De_Vencimiento;

        //    c1 = new PdfPCell(new Phrase("Vence:" + fecha.ToString("dd/MM/yyyy"), fntTituloTabla));

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;
        //    c1.BorderWidthLeft = 0;
        //    c1.BorderWidthRight = 1;
        //    c1.Colspan = 2;
        //    c1.Rowspan = 4;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 4;


        //    tableCarnetMotosierra.AddCell(c1);


        //    /// Linea No. 2


        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;

        //    c1.Border = 0;

        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("DECRETO 122-96, LEY REGULADORA DEL REGISTRO, AUTORIZACION", fntTituloTabla));


        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;
        //    c1.BorderWidthBottom = 1;
        //    c1.BorderWidthLeft = 1;
        //    c1.BorderWidthRight = 1;
        //    c1.Colspan = 5;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);




        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;

        //    c1.Border = 0;

        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("Recortar y adherir a la herramienta", fntTituloTabla));


        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Border = 0;

        //    c1.Colspan = 5;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);


        //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));

        //    c1.Border = 0;

        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    c1.Colspan = 3;
        //    c1.Rowspan = 1;


        //    tableCarnetMotosierra.AddCell(c1);




        //    //byte[] newQR;
        //    //string ippuerto = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault().Direccion_URL;
        //    //string text = ippuerto + "/Documentos/" + tbl_RNF_Registro.Guid_id + ".pdf";
        //    //newQR = qrCodeBytes(text);
        //    //long ticks = DateTime.Now.Ticks;
        //    //string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");

        //    //System.IO.File.WriteAllBytes(rootqr, newQR);



        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 1;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;

        //    //tableCarnetMotosierra.AddCell(c1);


        //    //c1.Colspan = 1;
        //    //c1.Rowspan = 4;

        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    //c1.Border = 0;

        //    //tableCarnetMotosierra.AddCell(c1);

        //    /////// Primera Linea

        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);


        //    //c1 = new PdfPCell(new Phrase("GUATEMALA, C.A.", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 3;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);


        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);

        //    /////// Segunda Linea

        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);

        //    //c1 = new PdfPCell(new Phrase("CONSTANCIA DE REGISTRO", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 3;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);

        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);

        //    ////// Tercera Linea

        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);


        //    //c1 = new PdfPCell(new Phrase("" + tbl_RNF_Registro.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));

        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 3;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);


        //    //c1 = new PdfPCell(new Phrase("", fntTituloTabla));


        //    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    //c1.Colspan = 2;
        //    //c1.Rowspan = 1;
        //    //c1.Border = 0;


        //    //tableCarnetMotosierra.AddCell(c1);

        //    return;
        //}


        private void LlenaSubTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_10;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_10;

            tableTitulo = new PdfPTable(7);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));

            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("PROCESO: REGISTRO NACIONAL FORESTAL", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);



            byte[] newQR;
            string text = "http://localhost:61244/Documentos/" + tbl_Sol_Solicitud.Guid_id + ".pdf";
            newQR = qrCodeBytes(text);
            long ticks = DateTime.Now.Ticks;
            string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");

            System.IO.File.WriteAllBytes(rootqr, newQR);



            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rootqr);

            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);

            c1.Colspan = 1;
            c1.Rowspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            ///// Primera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("GUATEMALA, C.A.", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            ///// Segunda Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("CERTIFICADO DE INSCRIPCION", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            //// Tercera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("" + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaSubTituloRevisionBck(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_10;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_10;

            tableTitulo = new PdfPTable(7);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));

            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("PROCESO: REGISTRO NACIONAL FORESTAL", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            //bbarillas


            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();

            QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode("http://localhost:61244/Documentos/" + tbl_Sol_Solicitud.Guid_id + ".pdf", QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qRCodeData);
            String base64String = "";
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    base64String = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }

            }

            //String base64Image = base64String.Split(",")[1];
            //byte[] decodedString = Base64.decode(base64Image, Base64.DEFAULT);
            //Bitmap decodedByte = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);
            //imageView.setImageBitmap(decodedByte);


            //bbarillas

            //// Imagen superior


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/icons/images.png"));

            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);

            c1.Colspan = 1;
            c1.Rowspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            ///// Primera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("GUATEMALA, C.A.", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            ///// Segunda Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("CERTIFICADO DE INSCRIPCION", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            //// Tercera Linea

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("" + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            return;
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


        private void LlenaUnTextos(string TextoA)
        {
            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_13B;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_13B;

            tableTitulo = new PdfPTable(1);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            return;
        }

        private void LlenaUnTextoResaltado(string TextoA)
        {
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 30, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 30, iTextSharp.text.Font.BOLD);

            tableTitulo = new PdfPTable(1);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


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


        private void LlenaDosTextos(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_10;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_10;

            tableTitulo = new PdfPTable(2);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaDosTextosPorCuatro(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);

            tableTitulo = new PdfPTable(3);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTablasCeldas));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaDosTextosPorCuatroHuge(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 14);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 22, iTextSharp.text.Font.BOLD);

            tableTitulo = new PdfPTable(3);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTablasCeldas));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaCincoTextos(string TextoA, string TextoB, string TextoC, string TextoD,string TextoE)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 9);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 9);

            tableTitulo = new PdfPTable(5);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(TextoE, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }
        private void LlenaCuatroTextos_(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 09);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 09);

            tableTitulo = new PdfPTable(5);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_RIGHT;

            c1.Colspan = 2;
            c1.Rowspan = 2;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }
        private void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 11);

            tableTitulo = new PdfPTable(5);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaCuatroTextosResaltado(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 30, iTextSharp.text.Font.BOLD);

            tableTitulo = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 11);

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

            c1 = new PdfPCell(new Phrase("\nCERTIFICADO DE INSCRIPCION DE " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("RF-RE-051", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Noviembre 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\n PROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaDatosSolicitante(long Solicitud_id)
        {

            string Nombre, No_Nit;

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            if (tbl_Sol_PropietarioPersonaIndividual != null)
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaIndividual.Nombres} {tbl_Sol_PropietarioPersonaIndividual.Apellidos}";
                No_Nit = $"{tbl_Sol_PropietarioPersonaIndividual.No_NIT}";
            }
            else
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaJuridica.Nombre}";
                No_Nit = $"{tbl_Sol_PropietarioPersonaJuridica.No_NIT}";
            }

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();

            DateTime fecha;
            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(11);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{Nombre}", fntTituloTabla));

            c1.Colspan = 4;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"NIT: {No_Nit}", fntTitulo2));

            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Teléfono Celular", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            if (oRepresentanteLegal != null)
            {
                for (int i = 0; i < oRepresentanteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].Nombres} {oRepresentanteLegal[i].Apellidos}", fntTituloTabla));

                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].RepresentanteNo_Documento}", fntTituloTabla));

                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                }
            }

            if (oMandatario != null)
            {
                for (int i = 0; i < oMandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 3;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].RepresentanteNo_Documento}", fntTituloTabla));

                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].Nombres} {oMandatario[i].Apellidos}", fntTituloTabla));

                    c1.Colspan = 9;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oMandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oMandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));

                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase($"", fntTituloTabla));

                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }
            }


            tableBanner.AddCell(c1);

            return;
        }
        private void LlenaDatosNotificacion(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {


            Tbl_Gral_Municipio oMunicipio = (from d in db.Tbl_Gral_Municipio
                                             where d.Municipio_id == tbl_Seg_UsuarioExterno.Municipio_id
                                             select d).FirstOrDefault();

            Tbl_Gral_Departamento oDepartamento = (from d in db.Tbl_Gral_Departamento
                                                   where d.Departamento_id == oMunicipio.Departamento_id
                                                   select d).FirstOrDefault();

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(11);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Dirección:", fntTitulo2));

            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Direccion}", fntTituloTabla));

            c1.Colspan = 9;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTitulo2));

            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oMunicipio.Municipio}", fntTituloTabla));

            c1.Colspan = 6;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTitulo2));

            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oDepartamento.Departamento}", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Correo Electrónico:", fntTitulo2));

            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Correo}", fntTituloTabla));

            c1.Colspan = 5;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"No. de Celular", fntTitulo2));

            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Telefono_Celular}, {tbl_Seg_UsuarioExterno.Telefono_Oficina}, {tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension}", fntTituloTabla));

            c1.Colspan = 3;
            tableDatosNotificacion.AddCell(c1);

            tableBanner.AddCell(c1);

            return;
        }
        private void LlenaDatosNotificacionDetallada(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);


            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************


            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("NIT: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_NIT, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Fecha Nacimiento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Fecha_Nacimiento.ToString("dd/MM/yyyy"), fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Tipo de Documento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Número de documento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_Documento, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Departamento emisión: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Municipio emisión: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio, fntTituloTabla));

            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Observaciones: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.Colspan = 3;

            tableDatosNotificacion.AddCell(c1);

            return;
        }
        private void LlenaDatosFinca(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            var Enter = new iTextSharp.text.Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(11);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Nombre de la finca:", fntTituloTabla));

            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));

            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));

            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));

            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 11;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Registro de la Propiedad:", fntTituloTabla));

            c1.Colspan = 2;
            c1.Rowspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número", fntTituloTabla));

            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegNumero}", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Libro", fntTituloTabla));

            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegLibro}", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Folio", fntTituloTabla));

            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegFolio}", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));

            c1.Colspan = 7;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Dirección de la Finca", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Ubicacion}", fntTituloTabla));

            c1.Colspan = 9;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTituloTabla));

            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Municipio.Municipio}", fntTituloTabla));

            c1.Colspan = 6;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));

            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }
        private void LlenaDatosDeLaFinca(Tbl_Sol_Finca tbl_sol_finca)
        {
            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();


            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombre de la finca: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.NombreFinca, fntTituloTabla));

            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Acreditada por medio de: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Sol_FincaConstanciaDePropiedad.Descripcion, fntTituloTabla));

            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 1)
            {
                c1 = new PdfPCell(new Phrase("Numero: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegNumero, fntTituloTabla));

                tablePersoneria.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Folio: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegFolio, fntTituloTabla));

                tablePersoneria.AddCell(c1);



                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Libro: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegLibro, fntTituloTabla));

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Departamento de Registro: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_DepartamentoRegistro.Departamento, fntTituloTabla));

                tablePersoneria.AddCell(c1);


                //************************************************************************************************************************************


            }

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 2)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);

            }

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 3)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Nombre de quien certifica: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialConCertificacion_NombreDeCertificador, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);

            }


            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 4)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Número de escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialDeEscrituraPublica_Numero, fntTituloTabla));

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha de la escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_sol_finca.ActaNotarialDeEscrituraPublica_Fecha ?? DateTime.Now).ToString("dd/MM/yyyy"), fntTituloTabla));

                tablePersoneria.AddCell(c1);

            }



            if (tbl_sol_finca.ConstanciaDePorpiedad_id >= 5)
            {
                c1 = new PdfPCell(new Phrase("Descripcion del respaldo de adquisicion: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ConstanciaPropiedadDescripcion, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Número de escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialDeEscrituraPublica_Numero, fntTituloTabla));

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha de la escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_sol_finca.ActaNotarialDeEscrituraPublica_Fecha ?? DateTime.Now).ToString("dd/MM/yyyy"), fntTituloTabla));

                tablePersoneria.AddCell(c1);

            }

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Ubicacion : ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Ubicacion, fntTituloTabla));
            c1.Colspan = 3;

            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_Departamento.Departamento, fntTituloTabla));

            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_Municipio.Municipio, fntTituloTabla));

            tablePersoneria.AddCell(c1);


            if (tbl_sol_finca.ObjetivoDeLaPlantacion != null)
            {
                c1 = new PdfPCell(new Phrase("Objetivo de la plantación : ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Sol_FincaObjetivoDeLaPlantacion.Descripcion, fntTituloTabla));
                c1.Colspan = 3;

                tablePersoneria.AddCell(c1);
            }

            if (tbl_sol_finca.OtrosRegistrosRNF == true || (tbl_sol_finca.Registros != null && tbl_sol_finca.Registros != ""))
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("SI", fntTituloTabla));

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Registro", fntTituloTabla));

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.Registros, fntTituloTabla));

                tablePersoneria.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("NO", fntTituloTabla));
                c1.Colspan = 2;

                tablePersoneria.AddCell(c1);

            }

            //Datos que agrega Nefta
            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_sol_finca.AreaTotal}", fntTituloTabla));

            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_sol_finca.AreaARegistrar}", fntTituloTabla));

            tablePersoneria.AddCell(c1);


            //************************************************************************************************************************************


            return;
        }



        public class ClassResumenPV
        {

            public long Solicitud_id { get; set; }
            public long Finca_id { get; set; }
            public string NombreFinca { get; set; }
            public long Rodal_id { get; set; }
            public int Tipo_de_Area { get; set; }
            public string Tipo_de_Area_Desc { get; set; }
            public decimal Longitud_Total { get; set; }
            public int Cantidad_Total_Arboles { get; set; }
            public string Especie { get; set; }
            public decimal Area_Efectiva_Rodal { get; set; }
            public int Anio_Establecimiento { get; set; }
            public string EstimacionPorMedioDe { get; set; }
            public decimal Cantidad_Arboles { get; set; }
            public decimal Densidad_ha { get; set; }
            public decimal AlturaPromedio { get; set; }
            public decimal DAPPromedio { get; set; }
            public decimal AreaBasal_ha { get; set; }
            public decimal Volumen_ha { get; set; }
            public decimal Volumen_Rodal { get; set; }
            public decimal Area_Basa_MetroCuadrado { get; set; }
            public decimal Volumen_X_Linea { get; set; }
            public decimal CoordenadaX { get; set; }
            public decimal CoordenadaY { get; set; }
            public int Clase { get; set; }

        }


        private void ResumenPV(long solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(11);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY";
            sqlQuery += " From fc_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id.ToString() + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            for (int i = 0; i < Resultado.Count(); i++)
            {
                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTitulo));
                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    varArea = Resultado[i].Tipo_de_Area_Desc;

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("No. área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area efectiva rodal (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad/ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura Promedio", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("No. área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad/ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura Promedio", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

            }
            return;
        }

        private void ResumenPV_API(long solicitud_id)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(11);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (decLongitudLinea != Resultado[i].Longitud_Total)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }
        private void ResumenPV_API_FS(long solicitud_id)
        {

            string sylfaenpath = "C:\\Windows\\fonts\\arialbd.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            //iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntSubTitulo = new iTextSharp.text.Font(sylfaen, size: 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(12);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 12;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 12;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP Prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (decLongitudLinea != Resultado[i].Longitud_Total)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }


                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_RNF_NoVolumen(string No_Registro)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(9);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);


                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }
        private void ResumenPV_RNF(string No_Registro)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell(); 

            tableEstimacion = new PdfPTable(10);
            string sqlQuery="";

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;
            bool mostrartotal = false;
            decimal volumenrodal = 0;
            decimal volumenlinea = 0;

            for (int i = 0; i < Resultado.Count(); i++)
            {
                mostrartotal = false;
                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("ID", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        //c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1 = new PdfPCell(new Phrase("Arboles por Rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);


                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("ID", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Arboles por rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        
                        c1 = new PdfPCell(new Phrase("Volumen linea", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                        //c1 = new PdfPCell(new Phrase($"{Resultado[i].Finca_id}", fntSubTitulo));
                    }
                    else
                    {
                        //c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {

                        if ((Resultado[i].Area_Efectiva_Rodal.ToString() == "0.00") || (Resultado[i].Area_Efectiva_Rodal.ToString() == "0.00"))
                        {
                            c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                        }
                        else
                        {

                            c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                        }
                        
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].EstimacionPorMedioDe == "Censo")
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Cantidad_Arboles.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                   
                    c1 = new PdfPCell(new Phrase($"{Math.Truncate(Resultado[i].Densidad_ha).ToString()}", fntSubTitulo));
                    //Redondear
                    //c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);
                    }

                                       

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                        //c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);



                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                   
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Cantidad_Arboles.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                   


                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_X_Linea.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                try
                {
                    if ((varArea == Resultado[i].Tipo_de_Area_Desc) && (varArea != Resultado[i+1].Tipo_de_Area_Desc))
                    {
                        mostrartotal = true;
                    }
                }
                catch
                {
                    mostrartotal = true;
                }

                volumenrodal += Resultado[i].Volumen_Rodal;
                volumenlinea += Resultado[i].Volumen_X_Linea;

                if (mostrartotal)
                {
                    decimal volumenmostrar = 0;

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodales  */
                    {
                        volumenmostrar = volumenrodal;
                    }
                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                    {
                        volumenmostrar = volumenlinea;
                    }

                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
                    c1.Colspan = 8;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);
                    c1 = new PdfPCell(new Phrase("Total", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(volumenmostrar.ToString("0.00"), fntSubTitulo));
                    c1.Colspan = 8;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    volumenrodal = 0;
                    volumenlinea = 0;
                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_RNF_20230825(string No_Registro)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(10);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);


                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id)))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_X_Linea.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_RNF_FS_NoVolumen(string No_Registro)
        {

            string sylfaenpath = "C:\\Windows\\fonts\\arialbd.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            //iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntSubTitulo = new iTextSharp.text.Font(sylfaen, size: 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(10);
            string sqlQuery= "";

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP Prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_RNF_FS(string No_Registro)
        {

            string sylfaenpath = "C:\\Windows\\fonts\\arialbd.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            //iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntSubTitulo = new iTextSharp.text.Font(sylfaen, size: 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(10);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;
            bool mostrartotal = false;
            decimal volumenrodal = 0;
            decimal volumenlinea = 0;

            for (int i = 0; i < Resultado.Count(); i++)
            {
                mostrartotal = false;

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP Prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                try
                {
                    if ((varArea == Resultado[i].Tipo_de_Area_Desc) && (varArea != Resultado[i + 1].Tipo_de_Area_Desc))
                    {
                        mostrartotal = true;
                    }
                }
                catch
                {
                    mostrartotal = true;
                }

                volumenrodal += Resultado[i].Volumen_Rodal;
                volumenlinea += Resultado[i].Volumen_X_Linea;

                //if (mostrartotal)
                //{
                //    decimal volumenmostrar = 0;

                //    if (Resultado[i].Tipo_de_Area == 1)   /* Rodales  */
                //    {
                //        volumenmostrar = volumenrodal;
                //    }
                //    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                //    {
                //        volumenmostrar = volumenlinea;
                //    }

                //    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
                //    c1.Colspan = 8;
                //    c1.Border = 1;
                //    tableEstimacion.AddCell(c1);
                //    c1 = new PdfPCell(new Phrase("Total", fntSubTitulo));
                //    c1.Colspan = 1;
                //    c1.Border = 1;
                //    tableEstimacion.AddCell(c1);

                //    c1 = new PdfPCell(new Phrase(volumenmostrar.ToString("0.00"), fntSubTitulo));
                //    c1.Colspan = 8;
                //    c1.Border = 1;
                //    tableEstimacion.AddCell(c1);

                //    volumenrodal = 0;
                //    volumenlinea = 0;
                //}

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_RNF_FS_20230825(string No_Registro)
        {

            string sylfaenpath = "C:\\Windows\\fonts\\arialbd.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            //iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntSubTitulo = new iTextSharp.text.Font(sylfaen, size: 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(10);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 12;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP Prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((decLongitudLinea != Resultado[i].Longitud_Total) || (lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private string CapitalizarNombre(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            texto = texto.Trim();
            return char.ToUpper(texto[0]) + texto.Substring(1).ToLower();
        }

        private void EspeciesForestales_Probosque(string No_Registro)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 6;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_RNF_Finca_Probosque_EspeciesForestales> tbl_Sol_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Probosque_EspeciesForestales
                                                                                                           where d.No_Registro == No_Registro
                                                                                                           orderby d.Finca_id , d.Area, d.NombreCientifico,d.Correlativo_id
                                                                                                           select d)
                                                                                                          .GroupBy(d => new { d.Finca_id,d.Area,d.NombreCientifico,d.Correlativo_id })
                                                                                                                 .Select(g => g.FirstOrDefault()).ToList();

            Tbl_RNF_Registro tbl_RNF_Registro;

            if (Session["Cancelacion"] == "SI")
            {

                 tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == No_Registro                                                     
                                                     select d).FirstOrDefault();
                tbl_RNF_Registro.SolicitudTipo_id = 1.06M;

            }
            else
            {
                 tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == No_Registro
                                                     && d.Solicitud_id != 0
                                                     select d).FirstOrDefault();
                tbl_RNF_Registro.SolicitudTipo_id = tbl_RNF_Registro.SolicitudTipo_id ?? 0;
            }


          
            decimal RegistroTipo = (decimal)(tbl_RNF_Registro.SolicitudTipo_id - Math.Truncate((decimal)tbl_RNF_Registro.SolicitudTipo_id));

            //List<Tbl_RNF_Finca_Probosque_EspeciesForestales> tbl_Sol_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Probosque_EspeciesForestales
            //                                                                                               where d.No_Registro == No_Registro
            //                                                                                               orderby d.Finca_id, d.Area, d.NombreCientifico
            //                                                                                               select d)
            //                                                                                        .GroupBy(d => new { d.Finca_id, d.Area, d.NombreCientifico })
            //                                                                                               .Select(g => g.FirstOrDefault())
            //                                                                                               .ToList()
            //                                                                                              .Select(d => {
            //                                                                                                  d.NombreCientifico = CapitalizarNombre(d.NombreCientifico);
            //                                                                                                  return d;
            //                                                                                              }).ToList();




            if (tbl_Sol_Finca_Probosque_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Probosque_EspeciesForestales = new List<Tbl_RNF_Finca_Probosque_EspeciesForestales>();
            }

            if (RegistroTipo != 0.02M)
            {


            if (tbl_Sol_Finca_Probosque_EspeciesForestales.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies forestales", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Referencia", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha plantación", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Densidad actual", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);




                for (int i = 0; i < tbl_Sol_Finca_Probosque_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].Referencia_id ==0)
                    {
                        c1 = new PdfPCell(new Phrase($"------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Referencia_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    }


                    //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Area.ToString().ToLower()}", fntSubTitulo));
                    //c1.Colspan = 1;
                    //c1.Border = 1;
                    //tableEstimacion.AddCell(c1);

                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].Area != null)
                    {

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Area.ToString().ToLower()}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"Especie en Asocio", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].NombreCientifico.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].FechaPlantacion != null)
                    {
                        c1 = new PdfPCell(new Phrase($"{((DateTime)tbl_Sol_Finca_Probosque_EspeciesForestales[i].FechaPlantacion).ToString("yyyy")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].DensidadActual != null)
                    {

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_Probosque_EspeciesForestales[i].DensidadActual).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);

                    }





                }


            }

            }

            return;
        }

        private void EspeciesForestales_PinpepOld(string No_Registro)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 5;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);

            List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales> tbl_Sol_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesForestales
                                                                                                           where d.No_Registro == No_Registro
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesForestales = new List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales>();
            }


            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies forestales", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Arboles Ha", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);



                for (int i = 0; i < tbl_Sol_Finca_PinpepOld_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].NombreEspecie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].ArbolesPorHa).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Area}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }


            }


            return;
        }

        private void EspeciesProteger_PinpepOld(string No_Registro)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 5;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);

            List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger> tbl_Sol_Finca_PinpepOld_EspeciesProtegers = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesProteger
                                                                                                        where d.No_Registro == No_Registro
                                                                                                        orderby d.Rodal_id
                                                                                                        select d).ToList();


            if (tbl_Sol_Finca_PinpepOld_EspeciesProtegers == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesProtegers = new List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger>();
            }


            if (tbl_Sol_Finca_PinpepOld_EspeciesProtegers.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies a proteger", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especies", fntSubTitulo));
                c1.Colspan = 3;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_Sol_Finca_PinpepOld_EspeciesProtegers.Count(); i++)
                {

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].EspeciesProteger.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 3;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].Area).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


            }


            return;
        }

        private void EspeciesForestales_Secorf(string No_Registro)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 8;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_RNF_Finca_Secorf_EspeciesForestales> tbl_Sol_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Secorf_EspeciesForestales
                                                                                                     where d.No_Registro == No_Registro
                                                                                                     orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.Correlativo_id
                                                                                                     select d).ToList();

            if (tbl_Sol_Finca_Secorf_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Secorf_EspeciesForestales = new List<Tbl_RNF_Finca_Secorf_EspeciesForestales>();
            }
            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long fincaid = 0;
            long lngRodal = 0;
            long rodalid = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            if (tbl_Sol_Finca_Secorf_EspeciesForestales.Count() > 0)
            {

                for (int i = 0; i < tbl_Sol_Finca_Secorf_EspeciesForestales.Count(); i++)
                {
                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {

                        if (lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id)
                        {
                            fincaid = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id;
                            Tbl_RNF_Finca tbl_RNF_Finca = (from d in db.Tbl_RNF_Finca
                                                           where d.No_Registro == No_Registro && d.Finca_Id == fincaid
                                                           select d).FirstOrDefault();

                            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
                            c1.Colspan = maxColumnas;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("Finca: " + tbl_RNF_Finca.Finca_Id + ", nombre de la finca : " + tbl_RNF_Finca.NombreFinca, fntSubTituloBig));
                            c1.Colspan = maxColumnas;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            varArea = "None";
                        }
                        rodalid = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id;
                        Tbl_RNF_Rodal tbl_RNF_Rodal = (from d in db.Tbl_RNF_Rodal
                                                       where d.No_Registro == No_Registro && d.Finca_id == fincaid && d.Rodal_Id == rodalid
                                                       select d).FirstOrDefault();

                        c1 = new PdfPCell(new Phrase($"Especies forestales, GTMX: {(((decimal)tbl_RNF_Rodal.GTMX).ToString("0"))} GTMY: {(((decimal)tbl_RNF_Rodal.GTMY).ToString("0"))} ", fntSubTitulo));
                        c1.Colspan = maxColumnas;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año Establ.", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Vol m3", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);
                    }

                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Area_ha ?? 0).ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Especie_id.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].DAP ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Altura ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Densidad_ha ?? 0).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento != null)
                    {
                        if (tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento == 0)
                        {
                            c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento}", fntSubTitulo));
                        }
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                    lngFinca = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id;
                    lngRodal = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id;


                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Volumen ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }



            }


            return;
        }


        private void ResumenPV_API_20221130(long solicitud_id)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(11);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY";
            sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 11;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 11;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("No. área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area efectiva rodal (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad/ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura Promedio", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("No. área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad/ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura Promedio", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Volumen por rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if (lngRodal != Resultado[i].Rodal_id)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if (lngRodal != Resultado[i].Rodal_id)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (decLongitudLinea != Resultado[i].Longitud_Total)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_ha.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Volumen_Rodal.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void Coordenadas_Rodal(long solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);
            List<Tbl_Sol_Rodal> tbl_Sol_Rodals = (from d in db.Tbl_Sol_Rodal
                                                  where d.Solicitud_id == solicitud_id
                                                  orderby d.Solicitud_id, d.Finca_id, d.Rodal_Id, d.Tipo_de_Area
                                                  select d).ToList();


            if (tbl_Sol_Rodals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas de Rodales", fntSubTitulo));
                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Tipo de Área", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMX", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMY", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
            }


            string Finca = "None";
            string varArea = "None";

            foreach (var item in tbl_Sol_Rodals)
            {

                c1 = new PdfPCell(new Phrase($"{item.Finca_id}. {item.Tbl_Sol_Finca.NombreFinca}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Rodal_Id}", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Tbl_Sol_Rodal_Tipo.Tipo_de_Area}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMX ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMY ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

            }

            return;
        }
        private void Coordenadas_Rodal_Poligono(long solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);
            List<Tbl_Sol_Rodal_Poligono> tbl_Sol_Rodals = (from d in db.Tbl_Sol_Rodal_Poligono
                                                           where d.Solicitud_id == solicitud_id
                                                           orderby d.Solicitud_id, d.Finca_id, d.Rodal_Id, d.Tipo_de_Area
                                                           select d).ToList();


            if (tbl_Sol_Rodals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas de Rodales", fntSubTitulo));
                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Tipo de Área", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMX", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMY", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
            }


            string Finca = "None";
            string varArea = "None";

            foreach (var item in tbl_Sol_Rodals)
            {

                c1 = new PdfPCell(new Phrase($"{item.Finca_id}. {item.Tbl_Sol_Rodal.Tbl_Sol_Finca.NombreFinca}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Rodal_Id}", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Tbl_Sol_Rodal.Tbl_Sol_Rodal_Tipo.Tipo_de_Area}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMX ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMY ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

            }

            return;
        }
        private void Coordenadas_Rodal_Poligono_Tecnico_API(long solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);
            //string sqlQuery;

            //sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY";
            //sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id.ToString() + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";
            //List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };
            //Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();
            List<Tbl_API_Sol_Rodal_Poligono_Local> tbl_API_Sol_Rodal_Locals = (from d in db_API.Tbl_API_Sol_Rodal_Poligono_Local
                                                                               where d.Solicitud_id == solicitud_id
                                                                               orderby d.Solicitud_id, d.Finca_id, d.Rodal_Id, d.Tipo_de_Area
                                                                               select d).ToList();


            if (tbl_API_Sol_Rodal_Locals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas de Rodales", fntSubTitulo));
                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Tipo de Área", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMX", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMY", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
            }


            string Finca = "None";
            string varArea = "None";

            foreach (var item in tbl_API_Sol_Rodal_Locals)
            {

                c1 = new PdfPCell(new Phrase($"{item.Finca_id}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Rodal_Id}", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Tipo_de_Area}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMX ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMY ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

            }

            return;
        }
        private void Coordenadas_Rodal_Tecnico_API(long solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);
            //string sqlQuery;

            //sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY";
            //sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id.ToString() + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";
            //List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };
            //Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();
            List<Tbl_API_Sol_Rodal_Local> tbl_API_Sol_Rodal_Locals = (from d in db_API.Tbl_API_Sol_Rodal_Local
                                                                      where d.Solicitud_id == solicitud_id
                                                                      orderby d.Solicitud_id, d.Finca_id, d.Rodal_Id, d.Tipo_de_Area
                                                                      select d).ToList();


            if (tbl_API_Sol_Rodal_Locals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas de Rodales", fntSubTitulo));
                c1.Colspan = 5;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Tipo de Área", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMX", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase("GTMY", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);
            }


            string Finca = "None";
            string varArea = "None";

            foreach (var item in tbl_API_Sol_Rodal_Locals)
            {

                c1 = new PdfPCell(new Phrase($"{item.Finca_id}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Rodal_Id}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Tipo_de_Area}", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMX ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((item.GTMY ?? 0).ToString("0")), fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                tableEstimacion.AddCell(c1);

            }

            return;
        }

        private void DatosEmpresas_RNF(string No_Registro, fc_RNF_Sel_Direccion_Result DatosDireccion, fc_RNF_Sel_Direccion_Result DatosDireccionMovil)
        {

            //Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(solicitud_id);
            Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad = (from d in db.Tbl_RNF_Empresa_Entidad
                                                               where d.No_Registro == No_Registro
                                                               select d).FirstOrDefault();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            string DireccionMostrar = "";
            string DireccionMovilMostrar = "";
            if (DatosDireccionMovil != null)
            {
                if ((DatosDireccionMovil.Direccion != null) && (DatosDireccionMovil.Direccion.Trim() != ""))
                {
                    DireccionMovilMostrar += DatosDireccionMovil.Direccion + ", ";
                }
                if ((DatosDireccionMovil.Aldea != null) && (DatosDireccionMovil.Aldea.Trim() != ""))
                {
                    DireccionMovilMostrar += DatosDireccionMovil.Aldea + ", ";
                }
                DireccionMovilMostrar += DatosDireccionMovil.Municipio + ", " + DatosDireccionMovil.Departamento;
            }


            if ((DatosDireccion.Direccion != null) && (DatosDireccion.Direccion.Trim() != ""))
            {
                DireccionMostrar += DatosDireccion.Direccion + ", ";
            }
            if (DatosDireccion.Aldea.Trim() != "")
            {
                DireccionMostrar += DatosDireccion.Aldea + ", ";
            }
            DireccionMostrar += DatosDireccion.Municipio + ", " + DatosDireccion.Departamento;



            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_11B;
            iTextSharp.text.Font fntSubTitulo = fntTituloTabla_11;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);


            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

            c1.Colspan = 5;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);


            if ((tbl_RNF_Empresa_Entidad.Tipo_Industria_id != null) && (tbl_RNF_Empresa_Entidad.Tipo_Industria_id != 0))
            {
                c1 = new PdfPCell(new Phrase("Tipo de Industria :", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombre_Tecnico ?? "", fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);
            }
            if (tbl_RNF_Registro.Categoria_id == 9)
            {
                c1 = new PdfPCell(new Phrase("Nombre de la entidad :", fntSubTitulo));
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Nombre comercial :", fntSubTitulo));
            }

            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad.Nombre ?? "", fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Dirección de ubicación :", fntSubTitulo));
            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(DireccionMostrar, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            if (((tbl_RNF_Empresa_Entidad.Tipo_Industria_id ?? 1) == 2) && ((DireccionMovilMostrar != null) && (DireccionMovilMostrar.Trim() != "")))
            {
                c1 = new PdfPCell(new Phrase("Dirección móvil :", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase(DireccionMovilMostrar, fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }



            c1 = new PdfPCell(new Phrase("Ubicación geográfica en coordenadas GTM :", fntSubTitulo));
            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"GTMX : {(tbl_RNF_Empresa_Entidad.GTMX ?? 0).ToString("0")}, GTMY : {(tbl_RNF_Empresa_Entidad.GTMY ?? 0).ToString("0")}", fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            return;
        }

        private void DatosEmpresas(long solicitud_id, fc_Sol_Sel_Direccion_Result DatosDireccion)
        {

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(solicitud_id);

            string DireccionMostrar = "";

            if (DatosDireccion.Direccion.Trim() != "")
            {
                DireccionMostrar += DatosDireccion.Direccion + ", ";
            }
            if (DatosDireccion.Aldea.Trim() != "")
            {
                DireccionMostrar += DatosDireccion.Aldea + ", ";
            }
            DireccionMostrar += DatosDireccion.Municipio + ", " + DatosDireccion.Departamento;



            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(5);


            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

            c1.Colspan = 5;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);


            if ((tbl_Sol_Empresa_Entidad.Tipo_Industria_id != null) && (tbl_Sol_Empresa_Entidad.Tipo_Industria_id != 0))
            {
                c1 = new PdfPCell(new Phrase("Tipo de Industria :", fntSubTitulo));
                c1.Colspan = 2;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombre_Tecnico, fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);
            }

            c1 = new PdfPCell(new Phrase("Nombre comercial :", fntSubTitulo));
            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad.Nombre, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Dirección de ubicación :", fntSubTitulo));
            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(DireccionMostrar, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Ubicación geográfica en coordenadas GTM :", fntSubTitulo));
            c1.Colspan = 2;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.GTMX}, {tbl_Sol_Empresa_Entidad.GTMY}", fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            return;
        }

        private void DatosTecnicoProfesional(Tbl_RNF_Registro tbl_RNF_Registro)
        {

            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(6);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 9, iTextSharp.text.Font.NORMAL);

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_RNF_Registro.UsuarioExterno_id);
            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault(); ;

            int categoriaid = (int)tbl_RNF_Registro.Categoria_id;
            int subcategoriaid = (int)tbl_RNF_Registro.Sub_Categoria_id;


            string profesional = (tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion ?? "");
            string posgrado = (tbl_RNF_TecnicoProfesional.PostGradoEspecialidad ?? "");
            string colegiado = (tbl_RNF_TecnicoProfesional.No_Colegiado ?? "");
            string universidad = (tbl_RNF_TecnicoProfesional.Universidad ?? "");
            string diplomaobtenido = (tbl_RNF_TecnicoProfesional.EspecificarCarrera ?? "");
            string nodediploma = (tbl_RNF_TecnicoProfesional.No_De_Diploma ?? "");

            c1 = new PdfPCell(new Phrase("Profesional", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(profesional, fntTituloTabla));
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Posgrado", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(posgrado, fntTituloTabla));
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Colegiado No", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(colegiado, fntTituloTabla));
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Universidad que lo Avala", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(universidad, fntTituloTabla));
            c1.Colspan = 5;
            tableDatosGenerales.AddCell(c1);

            if (((categoriaid == 7) && (subcategoriaid == 3)) || ((categoriaid == 7) && (subcategoriaid == 4)))
            {

                c1 = new PdfPCell(new Phrase("Diploma Obtenido", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);
                c1 = new PdfPCell(new Phrase(diplomaobtenido, fntTituloTabla));
                c1.Colspan = 3;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Código del Diploma", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);
                c1 = new PdfPCell(new Phrase(nodediploma, fntTituloTabla));
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

            }
        }


        private void DatosTecnicoProfesional_RNF(Tbl_RNF_Registro tbl_RNF_Registro)
        {

            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(3);
            iTextSharp.text.Font fntTituloTablaSimple = FontFactory.GetFont("HELVETICA", size: 13);
            iTextSharp.text.Font fntTituloTablaResaltado = FontFactory.GetFont("HELVETICA", size: 15, iTextSharp.text.Font.BOLD);

            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault(); ;

            string NombreProfesional = ((tbl_RNF_TecnicoProfesional.Nombres ?? "") + " " + (tbl_RNF_TecnicoProfesional.Apellidos ?? "")).Trim();
            string DireccionProfesional = tbl_RNF_TecnicoProfesional.Direccion ?? "";
            string MunicipioProfesional = tbl_RNF_TecnicoProfesional.Tbl_Gral_Municipio.Municipio;
            string DepartamentoProfesional = tbl_RNF_TecnicoProfesional.Tbl_Gral_Departamento1.Departamento;
            string direccionnuevaprofesional = "";
            string profesion = "";
            string gradoacademico = "";

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Tecnico)
            {
                gradoacademico = "Técnico";
            }

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional)
            {
                gradoacademico = "Profesional";
            }

            if (tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion.ToUpper().Contains("OTRA") == true)
            {
                profesion = tbl_RNF_TecnicoProfesional.UniversidadEspecificarCarrera;
            }
            else
            {
                profesion = tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion;

            }
            if ((DireccionProfesional != null) && (DireccionProfesional != ""))
            {
                direccionnuevaprofesional += DireccionProfesional + ", ";
            }

            direccionnuevaprofesional += MunicipioProfesional + ", " + DepartamentoProfesional;

            PdfPCell enter = new PdfPCell(new Phrase(" ", fntTituloTablaSimple));
            enter.Colspan = 3;
            enter.Border = 0;

            tableDatosGenerales.AddCell(enter);

            c1 = new PdfPCell(new Phrase("Nombre", fntTituloTablaSimple));
            c1.Colspan = 1;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(NombreProfesional, fntTituloTablaResaltado));
            c1.Colspan = 2;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);

            tableDatosGenerales.AddCell(enter);

            c1 = new PdfPCell(new Phrase("Documento de Identificación " + tbl_RNF_TecnicoProfesional.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTablaSimple));
            c1.Colspan = 1;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.No_Documento, fntTituloTablaResaltado));
            c1.Colspan = 2;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);

            tableDatosGenerales.AddCell(enter);

            c1 = new PdfPCell(new Phrase("Dirección de Ubicación", fntTituloTablaSimple));
            c1.Colspan = 1;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(direccionnuevaprofesional, fntTituloTablaResaltado));
            c1.Colspan = 2;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);

            tableDatosGenerales.AddCell(enter);

            c1 = new PdfPCell(new Phrase("Profesión", fntTituloTablaSimple));
            c1.Colspan = 1;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(profesion, fntTituloTablaResaltado));
            c1.Colspan = 2;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);

            tableDatosGenerales.AddCell(enter);

            c1 = new PdfPCell(new Phrase("Categoría", fntTituloTablaSimple));
            c1.Colspan = 1;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);
            c1 = new PdfPCell(new Phrase(gradoacademico, fntTituloTablaResaltado));
            c1.Colspan = 2;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);

            tableDatosGenerales.AddCell(enter);

        }


        private void LlenaDatosProfesionalPersonales(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);


            //******************************************

            c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);
            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de nacimiento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Fecha_Nacimiento.ToString("dd/MM/yyyy"), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_Documento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("No. de NIT: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_NIT, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }

        private void LlenaDatosProfesionalDireccion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);


            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            GlobalUtils globalUtils = new GlobalUtils();

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono celular: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono oficina: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Oficina, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }

        private void DatosMotosierras(long solicitud_id)
        {

            Tbl_Sol_Motosierra tbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == solicitud_id).FirstOrDefault();

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);



            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(4);


            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
            c1.Colspan = 4;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            string marca, modelo, cilindraje, potencia, noserie;
            marca = modelo = cilindraje = potencia = noserie = "";

            if ((tbl_Sol_Motosierra.Marca != null) && (tbl_Sol_Motosierra.Marca.Trim() != ""))
            {
                marca = tbl_Sol_Motosierra.Marca;
            }
            if ((tbl_Sol_Motosierra.Modelo != null) && (tbl_Sol_Motosierra.Modelo.Trim() != ""))
            {
                modelo = tbl_Sol_Motosierra.Modelo;
            }
            if ((tbl_Sol_Motosierra.Cilindraje != null) && (tbl_Sol_Motosierra.Cilindraje.Trim() != ""))
            {
                cilindraje = tbl_Sol_Motosierra.Cilindraje;
            }
            if ((tbl_Sol_Motosierra.Potencia != null) && (tbl_Sol_Motosierra.Potencia.Trim() != ""))
            {
                potencia = tbl_Sol_Motosierra.Potencia;
            }
            if ((tbl_Sol_Motosierra.No_SerieMotosierra != null) && (tbl_Sol_Motosierra.No_SerieMotosierra.Trim() != ""))
            {
                noserie = tbl_Sol_Motosierra.No_SerieMotosierra;
            }

            c1 = new PdfPCell(new Phrase("Marca :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(marca, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Modelo :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(modelo, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Cilindraje :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(cilindraje, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Potencia : ", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(potencia, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Número de Serie :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(noserie, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);


            return;
        }

        private void DatosMotosierras_RNF(string No_Registro)
        {

            //Tbl_Sol_Motosierra tbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == solicitud_id).FirstOrDefault();
            Tbl_RNF_Motosierra tbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(4);


            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
            c1.Colspan = 4;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            string marca, modelo, cilindraje, potencia, noserie;
            marca = modelo = cilindraje = potencia = noserie = "";

            if ((tbl_Sol_Motosierra.Marca != null) && (tbl_Sol_Motosierra.Marca.Trim() != ""))
            {
                marca = tbl_Sol_Motosierra.Marca;
            }
            if ((tbl_Sol_Motosierra.Modelo != null) && (tbl_Sol_Motosierra.Modelo.Trim() != ""))
            {
                modelo = tbl_Sol_Motosierra.Modelo;
            }
            if ((tbl_Sol_Motosierra.Cilindraje != null) && (tbl_Sol_Motosierra.Cilindraje.Trim() != ""))
            {
                cilindraje = tbl_Sol_Motosierra.Cilindraje;
            }
            if ((tbl_Sol_Motosierra.Potencia != null) && (tbl_Sol_Motosierra.Potencia.Trim() != ""))
            {
                potencia = tbl_Sol_Motosierra.Potencia;
            }
            if ((tbl_Sol_Motosierra.No_SerieMotosierra != null) && (tbl_Sol_Motosierra.No_SerieMotosierra.Trim() != ""))
            {
                noserie = tbl_Sol_Motosierra.No_SerieMotosierra;
            }

            c1 = new PdfPCell(new Phrase("Marca :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(marca, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Modelo :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(modelo, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Cilindraje :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(cilindraje, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Potencia :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(potencia, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Número de Serie :", fntSubTitulo));
            c1.Colspan = 1;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase(noserie, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            return;
        }

        private void EstimacionVolumen(Tbl_Sol_Finca tbl_Sol_Finca)
        {
            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();



            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(11);
            decimal Total_areabasalha, volumenha, volumenrodal, Total_Areabasalm2, Total_Volumenlineam2;
            decimal SubTotal_Areabasalha, SubTotal_Volumenha, SubTotal_Volumenrodal, SubTotal_Areabasalm2, SubTotal_Volumenlineam2;
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

                Total_areabasalha = 0;
                volumenha = 0;
                volumenrodal = 0;
                Total_Areabasalm2 = 0;
                Total_Volumenlineam2 = 0;

                for (int i = 0; i < validacionesDiametrica.Count(); i++)
                {

                    if (validacionesDiametrica[i].Tipo_de_Area == 1)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización de la plantación en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMX}", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMY}", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


                            c1 = new PdfPCell(new Phrase($"Especies a registrar (método utilizado {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()})", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
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

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));

                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));

                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Area_Efectiva_Rodal}", fntTituloTabla));

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

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Cantidad_Arboles}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Densidad_ha}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].AlturaPromedio}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].AreaBasal_ha}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].Volumen_Rodal}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);
                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        try
                        {
                            if (varEspecie != validacionesDiametrica[i + 1].Especie)
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));

                                c1.Colspan = 6;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString(), fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString(), fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha += SubTotal_Areabasalha;
                                volumenha += SubTotal_Volumenha;
                                volumenrodal += SubTotal_Volumenrodal;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;

                            }
                        }
                        catch
                        {

                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));

                            c1.Colspan = 6;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString(), fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString(), fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha += SubTotal_Areabasalha;
                            volumenha += SubTotal_Volumenha;
                            volumenrodal += SubTotal_Volumenrodal;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;

                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 5;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{volumenha}", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{volumenrodal}", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;

                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 5;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{volumenha}", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{volumenrodal}", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;

                        }
                    }
                    else if (validacionesDiametrica[i].Tipo_de_Area == 2)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización de la plantación en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMX}", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMY}", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


                            //c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"ESPECIES A REGISTRAR", fntTituloTabla)); c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id. de la finca", fntTituloTabla));
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

                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));

                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));

                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Area_Efectiva_Rodal}", fntTituloTabla));

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

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Cantidad_Arboles}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].AlturaPromedio}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].Volumen_X_Linea}", fntTituloTabla));

                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                        c1.Colspan = 2;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);
                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        try
                        {
                            if (varEspecie != validacionesDiametrica[i + 1].Especie)
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));

                                c1.Colspan = 5;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{SubTotal_Volumenlineam2}", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                                Total_Areabasalm2 += SubTotal_Areabasalm2;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;

                            }
                        }
                        catch
                        {
                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));

                            c1.Colspan = 5;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{SubTotal_Volumenlineam2}", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                            Total_Areabasalm2 += SubTotal_Areabasalm2;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;

                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 4;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{Total_Volumenlineam2}", fntTituloTabla));

                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);
                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;

                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 4;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{Total_Volumenlineam2}", fntTituloTabla));

                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);
                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));

                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;

                        }
                    }


                }


                tableEstimacion.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }

        private void FormulasCalculoVolumen(long Solicitud_id)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");
            tableFormulas = new PdfPTable(11);
            List<Tbl_Sol_Rodal_Dasometrico_Especie_Formula> oEspecieFormula;
            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 2;
            tableFormulas.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"Fórmula", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 9;
            tableFormulas.AddCell(c1);

            oEspecieFormula = (from d in db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula
                               where d.Solicitud_id == Solicitud_id
                               select d).ToList();

            for (int i = 0; i < oEspecieFormula.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].Especie_id, fntTituloTabla));

                c1.Colspan = 2;
                tableFormulas.AddCell(c1);
                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].strFormulario, fntTituloTabla));

                c1.Colspan = 9;
                tableFormulas.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }

        private void LlenaDatosPlantacion(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            Tbl_Sol_FincaObjetivoDeLaPlantacion objetivoPlantacion = (from d in db.Tbl_Sol_FincaObjetivoDeLaPlantacion
                                                                      where d.ObjetivoDeLaPlantacion == tbl_Sol_Finca.ObjetivoDeLaPlantacion
                                                                      select d).FirstOrDefault();
            Tbl_Sol_Rodal_CategoriaSIGAP categoriaSIGAP = (from d in db.Tbl_Sol_Rodal_CategoriaSIGAP
                                                           where d.CategoriaSIGAP_Id == tbl_Sol_Finca.CategoriaSIGAP_Id
                                                           select d).FirstOrDefault();

            var Enter = new iTextSharp.text.Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosPlantacion = new PdfPTable(11);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            string DescripcionPlantacion, DescripcionCategoriaSIGAP, CategoriaSIGAP, strCONAP;

            if (tbl_Sol_Finca.Area_SIGAP == true)
            {
                CategoriaSIGAP = $"Sí";
            }
            else
            {
                CategoriaSIGAP = $"No";
            }

            if (tbl_Sol_Finca.Reforestacion_CONAP == true)
            {
                strCONAP = $"Sí";
            }
            else
            {
                strCONAP = $"No";
            }


            if (categoriaSIGAP != null)
            {
                DescripcionCategoriaSIGAP = $"{categoriaSIGAP.Descripcion}";
            }
            else
            {
                DescripcionCategoriaSIGAP = $"";
            }
            if (objetivoPlantacion != null)
            {
                DescripcionPlantacion = $"{objetivoPlantacion.Descripcion}";
            }
            else
            {
                DescripcionPlantacion = $"";
            }

            c1 = new PdfPCell(new Phrase($"¿La plantación es producto de reforestación con CONAP?   {strCONAP}", fntTituloTabla));

            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"¿La plantación se encuentra dentro de área SIGAP?   {CategoriaSIGAP}", fntTituloTabla));

            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Nombre de la Categoría SIGAP", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{DescripcionCategoriaSIGAP}", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 5;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Objetivo de la plantación", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{DescripcionPlantacion}", fntTituloTabla));

            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 5;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }


        private void FirmaSolicitante_Bck()
        {
            var Enter = new iTextSharp.text.Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);

            tableFirmaSolicitante = new PdfPTable(numColumns: 11);

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            iTextSharp.text.Image logo;

            try
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/FirmaDigital_" + objUs.intUsuario_id.ToString() + ".png"));
            }
            catch (Exception excep)
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/rubrica.png"));
            }


            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 1;
            c1.Rowspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            ///   Firma


            logo.ScaleAbsolute(75.0F, 50.0F);

            c1 = new PdfPCell(logo);

            c1.Colspan = 4;
            c1.Rowspan = 3;
            c1.Border = 0;
            c1.Border = PdfPCell.BOTTOM_BORDER;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableFirmaSolicitante.AddCell(c1);

            ///   Firma

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 6;
            c1.Rowspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            string strDirectorRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{strDirectorRegional}", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            tableBanner.AddCell(c1);
            return;
        }
        private void FirmaSolicitante()
        {
            var Enter = new iTextSharp.text.Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);

            tableFirmaSolicitante = new PdfPTable(numColumns: 11);

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            iTextSharp.text.Image logo;

            try
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/FirmaDigital_" + objUs.intUsuario_id.ToString() + ".png"));
            }
            catch (Exception excep)
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/rubrica.png"));
            }


            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            string strDirectorRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{strDirectorRegional}", fntTituloTabla));

            c1.Colspan = 4;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);




            c1 = new PdfPCell(new Phrase("Director regional", fntTituloTabla));

            c1.Colspan = 4;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            tableBanner.AddCell(c1);
            return;
        }



        public class DatosInscripcion
        {

            public string Categoria { get; set; }
            public string SubCategoria { get; set; }
            public string Region { get; set; }
            public string SubRegion { get; set; }
            public string Propietario { get; set; }
            public string ConstanciaPropiedad { get; set; }
            public string Direccion { get; set; }
            public decimal AreaInscritaha { get; set; }
            public string procedencia { get; set; }
            public string PeriodoIncentivadoDel { get; set; }
            public string PeriodoIncentivadoAl { get; set; }
            public string ResolucionInscripcion { get; set; }
            public string FechaResolucion { get; set; }
            public string FechaRegistro { get; set; }
            public string FechaVencimiento { get; set; }
        }


        class Personerias
        {
            public List<fc_RNF_Sel_ListadoDePropietario_Result> PropietariosIndividuales { get; set; }
            public List<fc_RNF_Sel_ListadoDePropietario_Result> PropietariosJuridicos { get; set; }
            public List<fc_RNF_Rodal_RepresentanteMandatario_Result> RepresentatnteLegal { get; set; }
            public List<fc_RNF_Rodal_RepresentanteMandatario_Result> Mandatario { get; set; }
            public List<fc_RNF_Sel_ListadoDeArrendatario_Result> ArrendatariosIndividuales { get; set; }
            public List<fc_RNF_Sel_ListadoDeArrendatario_Result> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias_RNF(string No_Registro)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<fc_RNF_Sel_ListadoDePropietario_Result>();
            personerias.PropietariosJuridicos = new List<fc_RNF_Sel_ListadoDePropietario_Result>();
            personerias.RepresentatnteLegal = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>();
            personerias.Mandatario = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>();
            personerias.ArrendatariosIndividuales = new List<fc_RNF_Sel_ListadoDeArrendatario_Result>();
            personerias.ArrendatariosJuridicos = new List<fc_RNF_Sel_ListadoDeArrendatario_Result>();

            List<fc_RNF_Sel_ListadoDePropietario_Result> PropietariosIndividuales = (from d in db.fc_RNF_Sel_ListadoDePropietario(No_Registro)
                                                                                     where d.Personeria_id == 1
                                                                                     select d).ToList();

            List<fc_RNF_Sel_ListadoDePropietario_Result> ProietariosJuridicos = (from d in db.fc_RNF_Sel_ListadoDePropietario(No_Registro)
                                                                                 where d.Personeria_id == 2
                                                                                 select d).ToList();

            List<fc_RNF_Rodal_RepresentanteMandatario_Result> RepresentanteLegal = (from d in db.fc_RNF_Rodal_RepresentanteMandatario(No_Registro, false)
                                                                                    select d).ToList();

            List<fc_RNF_Rodal_RepresentanteMandatario_Result> Mandatario = (from d in db.fc_RNF_Rodal_RepresentanteMandatario(No_Registro, true)
                                                                            select d).ToList();

            List<fc_RNF_Sel_ListadoDeArrendatario_Result> ArrendatariosIndividuales = (from d in db.fc_RNF_Sel_ListadoDeArrendatario(No_Registro)
                                                                                       where d.Personeria_id == 1
                                                                                       select d).ToList();

            List<fc_RNF_Sel_ListadoDeArrendatario_Result> ArrendatariosJuridicos = (from d in db.fc_RNF_Sel_ListadoDeArrendatario(No_Registro)
                                                                                    where d.Personeria_id == 2
                                                                                    select d).ToList();

            if (PropietariosIndividuales == null) { PropietariosIndividuales = new List<fc_RNF_Sel_ListadoDePropietario_Result>(); }
            if (ProietariosJuridicos == null) { ProietariosJuridicos = new List<fc_RNF_Sel_ListadoDePropietario_Result>(); }
            if (RepresentanteLegal == null) { RepresentanteLegal = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>(); }
            if (Mandatario == null) { Mandatario = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>(); }
            if (ArrendatariosIndividuales == null) { ArrendatariosIndividuales = new List<fc_RNF_Sel_ListadoDeArrendatario_Result>(); }
            if (ArrendatariosJuridicos == null) { ArrendatariosJuridicos = new List<fc_RNF_Sel_ListadoDeArrendatario_Result>(); }

            if (PropietariosIndividuales.Count() > 0) { personerias.PropietariosIndividuales = PropietariosIndividuales; }
            if (ProietariosJuridicos.Count() > 0) { personerias.PropietariosJuridicos = ProietariosJuridicos; }
            if (RepresentanteLegal.Count() > 0) { personerias.RepresentatnteLegal = RepresentanteLegal; }
            if (Mandatario.Count() > 0) { personerias.Mandatario = Mandatario; }
            if (ArrendatariosIndividuales.Count() > 0) { personerias.ArrendatariosIndividuales = ArrendatariosIndividuales; }
            if (ArrendatariosJuridicos.Count() > 0) { personerias.ArrendatariosJuridicos = ArrendatariosJuridicos; }

            return personerias;
        }

        private PdfPTable tableCultivosEnAsocio = new PdfPTable(numColumns: 5);


        private void LlenarCultivosEnAsocio(List<ClassCultivosEnAsocio> Resultado)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            int maxcolumnas = 7;
            tableCultivosEnAsocio = new PdfPTable(maxcolumnas);

            c1 = new PdfPCell(new Phrase("Finca", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.Colspan = maxcolumnas;
            tableCultivosEnAsocio.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Finca", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Área", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Tipo de área", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Cultivo", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Año Establecimiento", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Volumen Rodal", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Volumen ha", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            for (int i = 0; i < Resultado.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase(Resultado[i].Finca, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Area, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Tipo_De_Area, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Cultivo, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Anio_Establecimiento, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Volumen_Rodal.ToString("0.00"), fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Volumen_ha.ToString("0.00"), fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

            }

            return;
        }



        public class ClassCultivosEnAsocio
        {
            public string Finca { get; set; }
            public string Area { get; set; }
            public string Tipo_De_Area { get; set; }
            public string Cultivo { get; set; }
            public string Anio_Establecimiento { get; set; }
            public decimal Volumen_Rodal { get; set; }
            public decimal Volumen_ha { get; set; }
        }


        public static string GenerarLink(string texto, string url)
        {
            return $"<a href='{url}' target='_blank'>{texto}</a>";
        }
        public string EmitirDocumento_PDF_API(string No_Registro)
        {
            Tbl_RNF_Registro tbl_RNF_Registro;

            if (Session["Cancelacion"] == "SI")
                {
                tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == No_Registro                                                    
                                                     select d).FirstOrDefault();
            }
            else
            {

             tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 && d.Solicitud_id !=0
                                                 select d).FirstOrDefault();
            }

            bool boolFechaInscripcion = false;
            bool boolFechaActualizacion = false;
            bool boolFechaRatificacion = false;
            bool boolFechaVencimiento = false;
            bool boolProcedenciaExterna = false;
            bool boolFechaInactivacion = false;

            string strProcedenciaExterna = "";
            if (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld || tbl_RNF_Registro.Procedencia_Probosque || tbl_RNF_Registro.Procedencia_secorf)
            {
                boolProcedenciaExterna = true;
                if (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld)
                {
                    strProcedenciaExterna = "PINPEP";
                }
                if (tbl_RNF_Registro.Procedencia_Probosque)
                {
                    strProcedenciaExterna = "PROBOSQUE";
                }
                if (tbl_RNF_Registro.Procedencia_secorf)
                {
                    strProcedenciaExterna = "Obligación de Repoblación Forestal";
                }
            }

            tbl_RNF_Registro.SolicitudTipo_id = tbl_RNF_Registro.SolicitudTipo_id ?? 0;

            decimal RegistroTipo = (decimal)(tbl_RNF_Registro.SolicitudTipo_id - Math.Truncate((decimal)tbl_RNF_Registro.SolicitudTipo_id));
            decimal[] RegistroTipoInscripcion = { 0M };
            decimal[] RegistroTipoActualizacion = { 0.01M, 0.02M };
            decimal[] RegistroTipoRatificacion = { 0.03M };
            decimal[] RegistroTipoInactivacion = { 0.4M };
            decimal[] RegistroTipoActivacion = { 0.05M };
            decimal[] RegistroTipoCancelacion = { 0.06M };

            if (RegistroTipoInscripcion.Contains(RegistroTipo))
            {
                if (tbl_RNF_Registro.Fecha_Inscripcion != null)
                {
                    boolFechaInscripcion = true;
                }
                if (tbl_RNF_Registro.Fecha_De_Vencimiento != null)
                {
                    boolFechaVencimiento = true;
                }
            }

            if (RegistroTipoActualizacion.Contains(RegistroTipo))
            {
                if (tbl_RNF_Registro.Fecha_Inscripcion != null)
                {
                    boolFechaInscripcion = true;
                }
                if (tbl_RNF_Registro.Fecha_Actualizacion != null)
                {
                    boolFechaActualizacion = true;
                }
                if (tbl_RNF_Registro.Fecha_De_Vencimiento != null)
                {
                    boolFechaVencimiento = true;
                }
            }


            if (RegistroTipoRatificacion.Contains(RegistroTipo))
            {
                if (tbl_RNF_Registro.Fecha_Inscripcion != null)
                {
                    boolFechaInscripcion = true;
                }
                if (tbl_RNF_Registro.Fecha_Actualizacion != null)
                {
                    boolFechaActualizacion = true;
                }
                if (tbl_RNF_Registro.Fecha_De_Vencimiento != null)
                {
                    boolFechaVencimiento = true;
                }
                boolFechaRatificacion = true;
            }

            if (RegistroTipoInactivacion.Contains(RegistroTipo))
            {
                if (tbl_RNF_Registro.Fecha_Inscripcion != null)
                {
                    boolFechaInscripcion = true;
                }
                if (tbl_RNF_Registro.Fecha_Inactivacion != null)
                {
                    boolFechaInactivacion = true;
                }
            }

            string sqlQuery;
            Constants constants = new Constants();



            fc_RNF_DatosInscripcion_Result Resultado;
            

            if (Session["Cancelacion"] == "SI")
            {

                Resultado = (from d in db.fc_RNF_DatosInscripcion(tbl_RNF_Registro.No_Registro)                            
                             select d).FirstOrDefault();
            }
            else
            {

                Resultado = (from d in db.fc_RNF_DatosInscripcion(tbl_RNF_Registro.No_Registro)
                             where d.Solicitud_id != 0
                             select d).FirstOrDefault();
            }

               

            sqlQuery = "Select dbo.[Fnc_RNF_Sel_DocumentosPropiedad]('" + tbl_RNF_Registro.No_Registro + "')";
            string documentospropiedad = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER); 
            doc.SetMargins(1f, 1f, 50f, 50f);

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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Enter = new iTextSharp.text.Paragraph(" ");

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

             Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == tbl_RNF_Registro.UsuarioExterno_id
                                                             select d).FirstOrDefault();

            fc_RNF_Sel_Direccion_Result fc_RNF_Sel_Direccion_Result = (from d in db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro)
                                                                       select d).FirstOrDefault();

            fc_RNF_Sel_Direccion_Result fc_RNF_Sel_Direccion_ResultMovil = (from d in db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro)
                                                                            where d.TipoDireccion == "Empresa Movil"
                                                                            select d).FirstOrDefault();



            List<fc_RNF_Sel_Direccion_Result> fc_RNF_Sel_Direccion_Results = (from d in db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro)
                                                                              select d).ToList();

            strNombre = tbl_RNF_Registro.No_Registro + tbl_RNF_Registro.Expediente + ".pdf";

            strNombre = strNombre.Replace("/", "-");

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();

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


            Personerias personerias = ObtenerPersonerias_RNF(tbl_RNF_Registro.No_Registro);

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 1))
            {


                try
                {

                    if (Constants.VisualizarInformacionDesarrollo == 1)
                    {
                        string urlact = this.Url.Action();
                        LlenaBanner(urlact);
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }


                    StrMotosierraPropietario = "";

                    if (personerias.PropietariosIndividuales.Count() > 0)
                    {
                        foreach (var item in personerias.PropietariosIndividuales)
                        {
                            StrMotosierraPropietario = StrMotosierraPropietario + "Propietario:" + item.Nombre + "\n";
                            StrMotosierraPropietario = StrMotosierraPropietario + item.TipoIdentificacion + item.NoIdentificacion + "\n";

                        }
                    }
                    if (personerias.PropietariosJuridicos.Count() > 0)
                    {
                        foreach (var item in personerias.PropietariosJuridicos)
                        {

                            StrMotosierraPropietario = StrMotosierraPropietario + "Propietario:" + item.Nombre + "\n";
                            StrMotosierraPropietario = StrMotosierraPropietario + item.TipoIdentificacion + item.NoIdentificacion + "\n";

                        }
                    }
                    if (personerias.RepresentatnteLegal.Count() > 0)
                    {
                        foreach (var item in personerias.RepresentatnteLegal)
                        {

                            StrMotosierraPropietario = StrMotosierraPropietario + "Representante Legal:" + item.Nombres + " " + item.Apellidos + "\n";
                            StrMotosierraPropietario = StrMotosierraPropietario + item.DocumentoTipo + item.RepresentanteNo_Documento + "\n";

                        }
                    }

                    if (personerias.Mandatario.Count() > 0)
                    {
                        foreach (var item in personerias.Mandatario)
                        {

                            StrMotosierraPropietario = StrMotosierraPropietario + "Mandatario:" + item.Nombres + " " + item.Apellidos + "\n";
                            StrMotosierraPropietario = StrMotosierraPropietario + item.DocumentoTipo + item.RepresentanteNo_Documento + "\n";

                        }
                    }


                    LlenaBannerMotosierra(tbl_RNF_Registro, Resultado);
                    doc.Add(tableCarnetMotosierra);

                    //pdfContentByte.SetGState(pdfGState);
                    //pdfContentByte.AddImage(image);

                    doc.Close();
                    writer.Close();

                }
                catch
                {
                    doc.Close();
                    writer.Close();
                }
                return strNombre;
            }



            try
            {

                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlact = this.Url.Action();
                    LlenaBanner(urlact);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }

                LlenaSubTituloRevision_RNF(tbl_RNF_Registro);
                doc.Add(tableTitulo);
                //doc.Add(Enter);

                //bbarillas

                LlenaCuatroTextos_("", "", "","* Previo a escanear el código QR, debe");
                doc.Add(tableTitulo);

                LlenaCuatroTextos_("", "", "", "habilitar el uso de ventanas emergentes");
                doc.Add(tableTitulo);
                
                LlenaCuatroTextos_("", "", ""," ");
                doc.Add(tableTitulo);

                LlenaCuatroTextos_("", "", "", "Verificación de Constancia: https://consultarnf.inab.gob.gt/");
                doc.Add(tableTitulo);

                LlenaCuatroTextos_("", "", ""," ");
                doc.Add(tableTitulo);

                LlenaCuatroTextos_("", "", ""," ");
                doc.Add(tableTitulo);

                LlenaCuatroTextos("", "", "", "Registro No. " + tbl_RNF_Registro.No_Registro);
                doc.Add(tableTitulo);

                LlenaCuatroTextos("", "", "", "Región " + Resultado.Region + " Subregión " + Resultado.SubRegion);
                doc.Add(tableTitulo);

                LlenaDos_UnoTextosDosResaltadoUnidos("Subcategoría", Resultado.SubCategoria);
                //LlenaCuatroTextos("Subcategoría", "", "", "");
                doc.Add(tableTitulo);

                LlenaDos_UnoTextosDosResaltadoUnidos("Número de expediente RNF", tbl_RNF_Registro.Expediente);
                doc.Add(tableTitulo);

                if (boolProcedenciaExterna)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Origen", strProcedenciaExterna);
                    doc.Add(tableTitulo);

                    if ((tbl_RNF_Registro.Procedencia_Expediente != null) && (tbl_RNF_Registro.Procedencia_Expediente.Trim() != ""))
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Número de expediente Origen", tbl_RNF_Registro.Procedencia_Expediente);
                        doc.Add(tableTitulo);
                    }

                    if (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld)
                    {
                        if ((tbl_RNF_Registro.Procedencia_Fase != null) && (tbl_RNF_Registro.Procedencia_Fase.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Ultima fase", (tbl_RNF_Registro.Procedencia_Fase ?? ""));
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_Modalidad != null) && (tbl_RNF_Registro.Procedencia_Modalidad.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Procedencia", (tbl_RNF_Registro.Procedencia_Modalidad ?? ""));
                            doc.Add(tableTitulo);
                        }
                    }
                    if (tbl_RNF_Registro.Procedencia_Probosque)
                    {
                        if ((tbl_RNF_Registro.Procedencia_Fase != null) && (tbl_RNF_Registro.Procedencia_Fase.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Ultima fase", (tbl_RNF_Registro.Procedencia_Fase ?? ""));
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_Modalidad != null) && (tbl_RNF_Registro.Procedencia_Modalidad.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Procedencia", (tbl_RNF_Registro.Procedencia_Modalidad ?? ""));
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_TipoProyecto != null) && (tbl_RNF_Registro.Procedencia_TipoProyecto.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Tipo de proyecto", (tbl_RNF_Registro.Procedencia_TipoProyecto ?? ""));
                            doc.Add(tableTitulo);
                        }
                    }
                    if (tbl_RNF_Registro.Procedencia_secorf)
                    {
                        if ((tbl_RNF_Registro.Procedencia_Modalidad != null) && (tbl_RNF_Registro.Procedencia_Modalidad.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Procedencia", (tbl_RNF_Registro.Procedencia_Modalidad ?? ""));
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_FechaInicioPeriodo != null) && (tbl_RNF_Registro.Procedencia_FechaFinPeriodo != null))
                        {
                            DateTime FechaInicio = (DateTime)tbl_RNF_Registro.Procedencia_FechaInicioPeriodo;
                            DateTime FechaFin = (DateTime)tbl_RNF_Registro.Procedencia_FechaFinPeriodo;
                            string StrFechaInicio = FechaInicio.ToString("dd/MM/yyyy");
                            string StrFechaFin = FechaFin.ToString("dd/MM/yyyy");
                            LlenaDos_UnoTextosDosResaltadoUnidos("Periodo del incentivo", StrFechaInicio + " - " + StrFechaFin);
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_Licencia != null) && (tbl_RNF_Registro.Procedencia_Licencia.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Número de licencia", (tbl_RNF_Registro.Procedencia_Licencia ?? ""));
                            doc.Add(tableTitulo);
                        }
                        if ((tbl_RNF_Registro.Procedencia_POA != null) && (tbl_RNF_Registro.Procedencia_POA.Trim() != ""))
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Número de POA", (tbl_RNF_Registro.Procedencia_POA ?? ""));
                            doc.Add(tableTitulo);
                        }
                    }


                }


                string subModalidad = "";
                string Modalidad = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id && Obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id).First().Modalidad;
              
                try
                {
                           
                    subModalidad += db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id && Obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_RNF_Registro.Sub_Sub_Categoria_id).First().Modalidad;
                }
                catch (Exception ex)
                {
                    subModalidad = "";
                }


                Constants Const = new Constants();

                if (Modalidad.ToUpper().Contains("HULE"))
                {
                    Modalidad = "Hule";
                }

                if (Modalidad.ToUpper().Contains("PINABETE"))
                {
                    Modalidad = "Pinabete";
                }

                if (subModalidad.ToUpper().Contains("HULE"))
                {
                    subModalidad = "Hule";
                }

                if (subModalidad.ToUpper().Contains("PINABETE"))
                {
                    subModalidad = "Pinabete";
                }

                if ((Modalidad != "Hule") && (Modalidad != "Pinabete"))
                {
                    Modalidad = "";
                }

                if (Modalidad != "")
                {
                    Modalidad = Const.initCapTexto(Modalidad);

                    LlenaDos_UnoTextosDosResaltadoUnidos(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id).First().Modalidad_Categoria, Modalidad);
                    doc.Add(tableTitulo);

                }

                if (subModalidad != "")
                {
                    subModalidad = Const.initCapTexto(subModalidad);

                    LlenaDos_UnoTextosDosResaltadoUnidos(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id).First().Modalidad_SubCategoria, subModalidad);
                    doc.Add(tableTitulo);

                }


                string datoreemplazar = "";
                List<string> datolistreemplazar = new List<string>();

                personerias = ObtenerPersonerias_RNF(tbl_RNF_Registro.No_Registro);
                StrMotosierraPropietario = "";

                 if (personerias.PropietariosIndividuales.Count() > 0)
                {
                    foreach (var item in personerias.PropietariosIndividuales)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Propietario:", item.Nombre);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.TipoIdentificacion, item.NoIdentificacion);
                        doc.Add(tableTitulo);

                        StrMotosierraPropietario = StrMotosierraPropietario + "Propietario:" + item.Nombre + "\n";
                        StrMotosierraPropietario = StrMotosierraPropietario + item.TipoIdentificacion + item.NoIdentificacion + "\n";

                    }
                }
                if (personerias.PropietariosJuridicos.Count() > 0)
                {
                    foreach (var item in personerias.PropietariosJuridicos)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Propietario:", item.Nombre);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.TipoIdentificacion, item.NoIdentificacion);
                        doc.Add(tableTitulo);

                        StrMotosierraPropietario = StrMotosierraPropietario + "Propietario:" + item.Nombre + "\n";
                        StrMotosierraPropietario = StrMotosierraPropietario + item.TipoIdentificacion + item.NoIdentificacion + "\n";

                    }
                }
                if (personerias.RepresentatnteLegal.Count() > 0)
                {
                    foreach (var item in personerias.RepresentatnteLegal)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Representante Legal:", item.Nombres + " " + item.Apellidos);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.DocumentoTipo, item.RepresentanteNo_Documento);
                        doc.Add(tableTitulo);

                        StrMotosierraPropietario = StrMotosierraPropietario + "Representante Legal:" + item.Nombres + " " + item.Apellidos + "\n";
                        StrMotosierraPropietario = StrMotosierraPropietario + item.DocumentoTipo + item.RepresentanteNo_Documento + "\n";

                    }
                }

                if (personerias.Mandatario.Count() > 0)
                {
                    foreach (var item in personerias.Mandatario)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Mandatario:", item.Nombres + " " + item.Apellidos);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.DocumentoTipo, item.RepresentanteNo_Documento);
                        doc.Add(tableTitulo);

                        StrMotosierraPropietario = StrMotosierraPropietario + "Mandatario:" + item.Nombres + " " + item.Apellidos + "\n";
                        StrMotosierraPropietario = StrMotosierraPropietario + item.DocumentoTipo + item.RepresentanteNo_Documento + "\n";

                    }
                }

                if (personerias.ArrendatariosIndividuales.Count() > 0)
                {
                    foreach (var item in personerias.ArrendatariosIndividuales)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Arrendatario:", item.Nombre);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.TipoIdentificacion, item.NoIdentificacion);
                        doc.Add(tableTitulo);
                    }
                }
                if (personerias.ArrendatariosJuridicos.Count() > 0)
                {
                    foreach (var item in personerias.ArrendatariosJuridicos)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Arrendatario:", item.Nombre);
                        doc.Add(tableTitulo);
                        LlenaDos_UnoTextosDosResaltadoUnidos(item.TipoIdentificacion, item.NoIdentificacion);
                        doc.Add(tableTitulo);
                    }
                }


                List<fc_RNF_Sel_Documentospropiedad_Result> fc_RNF_Sel_Documentospropiedads = (from d in db.fc_RNF_Sel_Documentospropiedad(tbl_RNF_Registro.No_Registro)
                                                                                               select d).ToList();

                int intContador = 0;
                if (fc_RNF_Sel_Documentospropiedads.Count() > 0)
                {
                    foreach (var item in fc_RNF_Sel_Documentospropiedads)
                    {
                        intContador = intContador + 1;

                        if (intContador == 1)
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Documento de propiedad:", item.Registro);
                        }
                        else
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("", item.Registro);
                        }

                        doc.Add(tableTitulo);
                    }
                }

                if ((tbl_RNF_Registro.Categoria_id == 1) || (tbl_RNF_Registro.Categoria_id == 2) || (tbl_RNF_Registro.Categoria_id == 3) || (tbl_RNF_Registro.Categoria_id == 4) || (tbl_RNF_Registro.Categoria_id == 6))
                {

                    LlenaDos_UnoTextosDosResaltadoUnidos("Lugar de ubicación del terreno : ", Resultado.Direccion);
                    doc.Add(tableTitulo);

                    //if (boolProcedenciaExterna)=false
                    if (boolProcedenciaExterna)
                    {
                        decimal solicitudtipoid = (decimal)(tbl_RNF_Registro.SolicitudTipo_id??0);
                        if(Math.Truncate(solicitudtipoid) == 100)
                        {
                            decimal areaaregistrarha = 0;
                            try
                            {
                                List<Tbl_RNF_Finca> tbl_RNF_Fincas = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList() ?? new List<Tbl_RNF_Finca>();
                                if(tbl_RNF_Fincas.Count() > 0)
                                {
                                    foreach(Tbl_RNF_Finca item in tbl_RNF_Fincas)
                                    {
                                        areaaregistrarha += (decimal)(item.AreaARegistrar??0);

                                    }
                                }
                            }
                            catch
                            {
                                areaaregistrarha = 0;
                            }

                            if (areaaregistrarha > 0)
                            {
                                LlenaDos_UnoTextosDosResaltadoUnidos("Área inscrita ha.:", Resultado.AreaARegistrarha.ToString() + " ha ");
                                doc.Add(tableTitulo);
                            }

                        }
                        else
                        {
                            if (Resultado.AreaARegistrarha > 0)
                            {
                                LlenaDos_UnoTextosDosResaltadoUnidos("Área inscrita ha.:", Resultado.AreaARegistrarha.ToString() + " ha ");
                                doc.Add(tableTitulo);
                            }
                        }
                    }
                    else
                    {
                        if (Resultado.AreaInscritaha > 0)
                        {
                            LlenaDos_UnoTextosDosResaltadoUnidos("Área inscrita ha.:", Resultado.AreaInscritaha.ToString() + " ha ");
                            doc.Add(tableTitulo);
                        }
                    }


                    if (Resultado.LongitudInscrita > 0)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Longitud inscrita m.:", Resultado.LongitudInscrita.ToString() + " m ");
                        doc.Add(tableTitulo);
                    }

                    if (Resultado.SIGAP_Area > 0)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Area SIGAP:", Resultado.SIGAP_Area.ToString() + " ha ");
                        doc.Add(tableTitulo);
                    }

                    if (Resultado.SIGAP_Longitud > 0)
                    {
                        LlenaDos_UnoTextosDosResaltadoUnidos("Longitud SIGAP:", Resultado.SIGAP_Longitud.ToString() + " m ");
                        doc.Add(tableTitulo);
                    }

                    if (tbl_RNF_Registro.Categoria_id == 6)
                    {
                        ResumenPV_RNF_FS(tbl_RNF_Registro.No_Registro);
                        doc.Add(tableEstimacion);
                    }
                    else
                    {
                        ResumenPV_RNF(tbl_RNF_Registro.No_Registro);
                        doc.Add(tableEstimacion);
                    }


                    EspeciesForestales_Probosque(tbl_RNF_Registro.No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesForestales_PinpepOld(tbl_RNF_Registro.No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesProteger_PinpepOld(tbl_RNF_Registro.No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesForestales_Secorf(tbl_RNF_Registro.No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }


                    //Coordenadas_Rodal_Tecnico_API(tbl_RNF_Registro.Solicitud_id);
                    //doc.Add(tableEstimacion);

                }
                if ((tbl_RNF_Registro.Categoria_id == 11) || (tbl_RNF_Registro.Categoria_id == 5) || (tbl_RNF_Registro.Categoria_id == 9) || ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2)))
                {

                    DatosEmpresas_RNF(No_Registro, fc_RNF_Sel_Direccion_Result, fc_RNF_Sel_Direccion_ResultMovil);
                    doc.Add(tableEstimacion);

                }
                if (tbl_RNF_Registro.Categoria_id == 7)
                {
                    //LlenaBanner("Formación Académica", "Izquierda", "");
                    //doc.Add(tableBanner);
                    //DatosTecnicoProfesional(tbl_RNF_Registro);
                    DatosTecnicoProfesional_RNF(tbl_RNF_Registro);
                    doc.Add(tableDatosGenerales);


                }
                if ((tbl_RNF_Registro.Categoria_id == 8) && tbl_RNF_Registro.Sub_Categoria_id == 1)
                {
                    DatosMotosierras_RNF(No_Registro);
                    doc.Add(tableEstimacion);
                }


                doc.Add(Enter);



                sqlQuery = " Select convert(varchar(10),Finca_id) +'. ' + \n";
                sqlQuery += "        (Select ISNULL(NombreFinca,'')  \n";
                sqlQuery += "        From Tbl_RNF_Finca \n";
                sqlQuery += "        Where No_Registro = Cultivo.No_Registro\n";
                sqlQuery += "        and Finca_id = Cultivo.Finca_id) Finca, \n";
                sqlQuery += "        convert(varchar(10),Rodal_id) Area, \n";
                sqlQuery += "        (case when Tipo_de_Area = 1 then 'Rodal' else 'Arboles en línea' end) Tipo_De_Area, \n";
                sqlQuery += "        (SElect Nombres_Comunes from Tbl_Gral_Cultivo Where Cultivo_id = Cultivo.Cultivo_id) Cultivo, \n";
                sqlQuery += "        convert(varchar(10),Anio_Establecimiento) Anio_Establecimiento, \n";
                sqlQuery += "        ISNULL(Volumen_Rodal,0) Volumen_Rodal, \n";
                sqlQuery += "        ISNULL(Volumen_ha,0) Volumen_ha\n";
                sqlQuery += "        From Tbl_RNF_Rodal_Cultivo Cultivo \n";
                sqlQuery += $"       Where Cultivo.No_Registro = '{tbl_RNF_Registro.No_Registro}' \n";

                List<ClassCultivosEnAsocio> ResultadoCultivo = new List<ClassCultivosEnAsocio> { };

                ResultadoCultivo = db.Database.SqlQuery<ClassCultivosEnAsocio>(sqlQuery).ToList();


                if (ResultadoCultivo.Count() > 0)
                {
                    LlenarCultivosEnAsocio(ResultadoCultivo);
                    doc.Add(tableCultivosEnAsocio);
                    doc.Add(Enter);
                }


                if (Resultado.ResolucionInscripcion.Contains("No asignada") == false)
                {
                    // Hasta la fecha 26-03-25 se utilizaba GETDATE para concatenar la fecha
                    //string fechaimpresion = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_FechaTxtSinGuatemala](getdate())").FirstOrDefault();                  
                    //LlenaUnTextos("Inscripción según resolución No. " + Resultado.ResolucionInscripcion + ", de fecha " + fechaimpresion);
                    LlenaUnTextos("Inscripción según resolución No. " + Resultado.ResolucionInscripcion + ", de fecha " + Resultado.FechaResolucionInscripcion);
                    doc.Add(tableTitulo);

                }


                if (boolFechaInscripcion)
                {
                    if ((tbl_RNF_Registro.ConstanciaFirmada != null) && (tbl_RNF_Registro.Fecha_Inscripcion != null))                    
                    {
                        LlenaDosTextosPorCuatro("Fecha de Inscripción : ", constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.Fecha_Inscripcion));
                        doc.Add(tableTitulo);
                    }
                }

                if (boolFechaActualizacion)
                {
                    if (tbl_RNF_Registro.ResolucionUltimaActualizacionFecha != null)
                    {
                        LlenaUnTextos("Actualización según resolución No. " + Resultado.ResolucionUltimaActualizacion + ", de fecha " + constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.ResolucionUltimaActualizacionFecha));
                        doc.Add(tableTitulo);
                    }
                    if (tbl_RNF_Registro.Fecha_Actualizacion != null)
                    {
                        LlenaDosTextosPorCuatro("Fecha de actualización : ", constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.Fecha_Actualizacion));
                        doc.Add(tableTitulo);
                    }
                }

                if (boolFechaRatificacion)
                {
                    if (tbl_RNF_Registro.Fecha_Actualizacion != null)
                    {
                        LlenaDosTextosPorCuatro("Fecha de ratificación : ", constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.Fecha_Actualizacion));
                        doc.Add(tableTitulo);
                    }
                }
                if (boolFechaInactivacion)
                {
                    if (tbl_RNF_Registro.ResolucionUltimaInactivacionFecha != null)
                    {
                        LlenaUnTextos("Inactivación según resolución No. " + Resultado.ResolucionInactivacion + ", de fecha " + constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.ResolucionUltimaInactivacionFecha));
                        doc.Add(tableTitulo);
                    }
                    if (tbl_RNF_Registro.Fecha_Inactivacion != null)
                    {
                        LlenaDosTextosPorCuatro("Fecha de inactivación : ", constants.fechaTxtSinGuatemala((DateTime)tbl_RNF_Registro.Fecha_Inactivacion));
                        doc.Add(tableTitulo);
                    }



                    if ((tbl_RNF_Registro.TipoInactivacion_id != null) && (tbl_RNF_Registro.TipoInactivacion_id != 0))
                    {
                        LlenaDosTextosPorCuatro("Motivo : ", tbl_RNF_Registro.Tbl_RNF_Registro_Inactivacion_Tipo.Descripcion);
                        doc.Add(tableTitulo);
                    }
                    if ((tbl_RNF_Registro.Descripcion_Inactivacion != null) && (tbl_RNF_Registro.Descripcion_Inactivacion.Trim() != ""))
                    {
                        LlenaDosTextosPorCuatro("Descripión : ", tbl_RNF_Registro.Descripcion_Inactivacion.Trim());
                        doc.Add(tableTitulo);
                    }
                    if ((tbl_RNF_Registro.Descripcion_InactivacionTecnico != null) && (tbl_RNF_Registro.Descripcion_InactivacionTecnico.Trim() != ""))
                    {
                        LlenaDosTextosPorCuatro("Descripión : ", tbl_RNF_Registro.Descripcion_InactivacionTecnico.Trim());
                        doc.Add(tableTitulo);
                    }


                }



                if (boolFechaVencimiento)
                {
                    LlenaDosTextosPorCuatro("Fecha de vencimiento: ", Resultado.FechaVencimiento);
                    doc.Add(tableTitulo);
                }



                string strEstado = "Activo";

                if ((tbl_RNF_Registro.Estado_id == 2) || (tbl_RNF_Registro.Estado_id == 3))
                {
                    strEstado = "Inactivo";
                }

                if ((tbl_RNF_Registro.Estado_id == 6))
                {
                    strEstado = "Cancelado";
                }

                //LlenaDosTextosPorCuatro("Estado del registro: ", tbl_RNF_Registro.Tbl_RNF_Registro_Estado.Descripcion);

                if ((strEstado == "Inactivo") || (strEstado == "Cancelado"))
                {
                    doc.Add(Enter);
                    LlenaDosTextosPorCuatroHuge("Estado del registro: ", strEstado);
                }
                else
                {
                    LlenaDosTextosPorCuatro("Estado del registro: ", strEstado);
                }

                doc.Add(tableTitulo);


                doc.Add(Enter);

                if ((tbl_RNF_Registro.Categoria_id == 5) && (tbl_RNF_Registro.Sub_Categoria_id == 3))
                {
                    LlenaBanner("Nota: Centro de acopio", "Centro", "Blanco");
                    doc.Add(tableBanner);
                    LlenaBanner("no autorizado, para la venta de productos forestales.", "Centro", "Blanco");
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                }

                if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 1) && (tbl_RNF_Registro.Fecha_De_Vencimiento > DateTime.Now) && ((tbl_RNF_Registro.Estado_id == 1) || (tbl_RNF_Registro.Estado_id == 4)))
                {
                    doc.Add(Enter);
                    LlenaBannerMotosierra(tbl_RNF_Registro, Resultado);
                    doc.Add(tableCarnetMotosierra);

                }

                //if ((tbl_RNF_Registro.Procedencia_PinpepNew == true) || (tbl_RNF_Registro.Procedencia_PinpepOld == true) || (tbl_RNF_Registro.Procedencia_Probosque == true) || ((tbl_RNF_Registro.Procedencia_secorf == true) && (tbl_RNF_Registro.Procedencia_Modalidad.Contains("Con"))))
                //{

                //    LlenaBanner("Nota: Se prohíbe el cambio de uso de la tierra sin autorización", "Centro", "Blanco");
                //    doc.Add(tableBanner);

                //    LlenaBanner("Ley Forestal, Decreto 101 - 96, Artículo 98.", "Centro", "Blanco");
                //    doc.Add(tableBanner);

                //}

                if (boolFechaRatificacion)
                {
                    LlenaBanner("Los datos consignados fueron ratificados por el propietario, lo cual tiene vigencia por 3 años a partir de la presente fecha.", "Centro", "");
                    doc.Add(tableBanner);

                }



                //FirmaSolicitante();
                //doc.Add(tableFirmaSolicitante);

                //pdfContentByte.SetGState(pdfGState);
                //pdfContentByte.AddImage(image);

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



        [HttpPost]
        public JsonResult EmitirDocumentoDeInscripcionDeRegistro(int Solicitud_id, string Guid_id, string EtapaSolicitudGuid, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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


            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == Solicitud_id && d.EtapaSolicitud_GUID_id == EtapaSolicitudGuid
                                                               select d).FirstOrDefault();
            string TextoMostrar, Nombre;
            TextoMostrar = "";

            try
            {
                Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

                decimal TipoGestion = tbl_Sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
                decimal solicitudtipoentero = Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
                decimal terminacionCancelacion = 0.06M;
                Session["Cancelacion"] = null;

                //SI NO es CANCELACIÓN
                if (TipoGestion == terminacionCancelacion)
                {
                    Session["Cancelacion"] = "SI";
                }
                else
                {
                    Session["Cancelacion"] = "NO";

                    ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure()
                    {
                        id = 0,
                        mensaje = "Fallo Desconocido",
                        respuesta = 0
                    };

                    string sqlQuery = "Exec SP_Sol_Solicitud_Registro @Solicitud_id, @Usuario_id, @EsInterno";
                    TempData["CantidadItems"] = "";
                    SqlParameter[] sqlParams;
                    sqlParams = new SqlParameter[]
                    {
                                 new SqlParameter { ParameterName = "@Solicitud_id",  Value = Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id , Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                    };

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                }

                Tbl_Sol_Solicitud tbl_Sol_Solicitud_ = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

                //Nombre = EmitirDocumento_PDF(Solicitud_id, EtapaSolicitudGuid);
                Nombre = EmitirDocumento_PDF_API(tbl_Sol_Solicitud_.No_Registro);


                tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = Nombre;

                db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;

                db.SaveChanges();



                TextoMostrar = "{ \"Ubicacion\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(Nombre) + "}";



            }



            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.InnerException?.InnerException?.Message); // a veces está más adentro
                Nombre = ex.Message;
                TextoMostrar = "{ \"Ubicacion\" : " + Newtonsoft.Json.JsonConvert.SerializeObject(Nombre) + "}";
            }

            return Json(TextoMostrar);
        }


        public ActionResult Index(long Solicitud_id)
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


            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Session = objUs;

            return View();
        }
        public ActionResult Fincas(long Solicitud_id)
        {
            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();
            ViewBag.Solicitud_id = Solicitud_id;
            return View(tbl_Sol_Finca);
        }

        public ActionResult EstimacionVolumen(long Solicitud_id, long Finca_Id)
        {
            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(Solicitud_id, Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Solicitud_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_Id = Finca_Id;

            return View(validacionesDiametrica);
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

        //        //@"      ""Coordenadas"": ""50,730,250,830"",

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
            string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);
            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioDirectorRegionalController-JsonProcesarFirmaElectronica");


            partialrootDest = $"/Archivos_ConFirmaElectronica/";

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();


            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Tbl_Gest_etapaSolicitud.Solicitud_id);

            decimal TipoGestion1 = tbl_Sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            decimal solicitudtipoentero = Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            decimal terminacionCancelacion = 0.06M;
            Session["Cancelacion"] = null;

            //SI NO es CANCELACIÓN
            if (TipoGestion1 == terminacionCancelacion)
            {
                Session["Cancelacion"] = "SI";
            }
            else
            {
                Session["Cancelacion"] = "NO";

            }

            Tbl_RNF_Registro tbl_RNF_Registro;


            if (Session["Cancelacion"] == "SI")
            {
                 tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == tbl_sol_solicitud.No_Registro
                                                     select d).FirstOrDefault();
            }
            else
            {
                 tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.GuidSolicitud_id == Guid_id
                                                     select d).FirstOrDefault();           
            }

                

               

            tbl_RNF_Registro.ConstanciaFirmada = ".pdf";

            //Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            decimal TipoGestion = (decimal)(tbl_RNF_Registro.SolicitudTipo_id - Math.Truncate((decimal)tbl_RNF_Registro.SolicitudTipo_id));
            decimal[] RegistroTipoInscripcion = { 0M };
            decimal[] RegistroTipoActualizacion = { 0.01M, 0.02M };
            decimal[] RegistroTipoRatificacion = { 0.03M };
            decimal[] RegistroTipoInactivacion = { 0.4M };
            decimal[] RegistroTipoActivacion = { 0.05M };
            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

            try
            {
                RequestUtil requestUtil = new RequestUtil();

                if (RegistroTipoInscripcion.Contains(TipoGestion))
                {
                    tbl_RNF_Registro.Fecha_Inscripcion = DateTime.Now;
                }

                if ((RegistroTipoActualizacion.Contains(TipoGestion)) || (RegistroTipoRatificacion.Contains(TipoGestion)))
                {
                    tbl_RNF_Registro.Fecha_Actualizacion = DateTime.Now;
                }

                if (RegistroTipoInactivacion.Contains(TipoGestion))
                {
                    tbl_RNF_Registro.Fecha_Inactivacion = DateTime.Now;
                }


                EmitirDocumento_PDF_API(tbl_RNF_Registro.No_Registro);

                /// bbarillas
                rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;
                /// bbarillas

                Usuario objUs = new Usuario();
                RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

                if (!objSesion.getBlSession())
                {
                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "Su sesión ha expirado, inicie sesión nuevamente." + "\"}";

                    return Json(jsonResultUsr);
                }
                else
                {
                    objUs = (Usuario)Session["User"];
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

                FirmarArchivos_Documentos_Request newParams = null;
                if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 1))
                {
                    newParams = new FirmarArchivos_Documentos_Request
                    {
                        Coordenadas = "290,670,480,700",
                    };
                }

                string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido, newParams);

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

                    if (resultFromStoreProcedure.respuesta == 1)
                    {
                        tbl_RNF_Registro.ConstanciaFirmada = strDocumentofirmado + ".pdf";
                        if ((TipoGestion == (decimal)0.00) || (TipoGestion == (decimal)0.01) || (TipoGestion == (decimal)0.02) || (TipoGestion == (decimal)0.03))
                        {
                            tbl_RNF_Registro.Estado_id = 4;
                        }

                        if (TipoGestion == (decimal)0.04)
                        {
                            tbl_RNF_Registro.Estado_id = 2;
                        }
                        if ((TipoGestion == (decimal)0.04) && (Math.Truncate((decimal)tbl_RNF_Registro.SolicitudTipo_id) >= 90))
                        {
                            tbl_RNF_Registro.Estado_id = 3;
                        }

                        tbl_RNF_Registro.ResolucionInscripcionFecha = DateTime.Now;
                        db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                        db.SaveChanges();

                        Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 8, "X.- El archivo se obtuvo con exito.");
                        Gestion_SEINEF_IECAI(tbl_RNF_Registro);
                    }



                }
                else
                {
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 9, "X.- No se logró obtener el archivo firmado.");

                }


                /* ------------------------------- Generar  RNF   ------------------------------------------------- */


                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 10, "Z.- FE generada con éxito.");

                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Sol_Solicitud_Registro  @Solicitud_id, @Usuario_id, @EsInterno";

                sqlParams = new SqlParameter[]
                {
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@Usuario_id", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EsInterno", Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };


                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                intRespuesta = resultado[0].respuesta;


                if (intRespuesta == 0)
                {

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró crear el Registro." + "\"}";

                    return Json(jsonResultUsr);
                }


                /* ------------------------------- FIN Generar  RNF   ------------------------------------------------- */

                intRespuesta = 1;



                Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

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



        Reply Gestion_SEINEF_IECAI(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            Reply respuesta = new Reply { };
            Reply existeIECAI = ExisteIECAI(tbl_RNF_Registro);


            if (existeIECAI.result != 1)
            {
                respuesta = existeIECAI;
            }
            else
            {

                Reply registrarIECAI = RegistrarIECAI(tbl_RNF_Registro);
                if (registrarIECAI.result != 1)
                {

                    respuesta = registrarIECAI;

                }
                else
                {

                    if (registrarIECAI.data != null)
                    {

                        string ClaveEncriptada = registrarIECAI.data.ToString();

                        string Clave = SecurEncryptDecrypt.DecryptString(ClaveEncriptada);


                        string sqlQuery = "EXEC SP_SEINEF_RegistroEmpresa @No_Registro, @Password";

                        SqlParameter[] sqlParameters = new SqlParameter[]
                        {
                            new SqlParameter{ParameterName = "@No_Registro", Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                            new SqlParameter{ParameterName = "@Password", Value = Clave, Direction = System.Data.ParameterDirection.Input}
                        };

                        ResultFromStoreProcedure resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParameters).FirstOrDefault();


                    }

                }

            }


            return respuesta;
        }

        public class RNF_Request
        {
            public string No_Registro { get; set; }
        }

        Reply RegistrarIECAI(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            RequestUtil requestUtil = new RequestUtil();

            Reply reply = new Reply { };
            RNF_Request rnf_Request = new RNF_Request
            {
                No_Registro = tbl_RNF_Registro.No_Registro,
            };

            string url = Constants.Direccion_IP_SEINEF + "/SEINEF_API/api/Gestion_SEINEF/RegistrarIECAI";

            reply = requestUtil.Execute_Gestion_SEINEF<RNF_Request>(url, "POST", rnf_Request);

            return reply;
        }

        Reply ExisteIECAI(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            RequestUtil requestUtil = new RequestUtil();
            Reply reply = new Reply { };
            RNF_Request rnf_Request = new RNF_Request
            {
                No_Registro = tbl_RNF_Registro.No_Registro,
            };
            string url = Constants.Direccion_IP_SEINEF + "/SEINEF_API/api/Gestion_SEINEF/ExisteIECAI";

            reply = requestUtil.Execute_Gestion_SEINEF<RNF_Request>(url, "POST", rnf_Request);

            return reply;
        }


        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

        [HttpPost]
        public JsonResult EmitirConstanciaNuevaRegistro(string No_Registro)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
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
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            string TextoMostrar, Nombre;
            TextoMostrar = "";


            try
            {
                Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == No_Registro
                                                     select d).FirstOrDefault();
                Nombre = EmitirDocumento_PDF_API(tbl_RNF_Registro.No_Registro);

                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Documento generado exitosamente",
                    Ubicacion = Nombre
                };

            }
            catch (Exception ex)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Ocurrió un error " + ex.Message
                };
            }
            return Json(jsonRespuesta);
        }

        public JsonResult JsonProcesarFirmaElectronicaConstanciaNueva(string No_Registro, string UsuarioFE, string PasswordFE)
        {
            // Bitacora activa.
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;
            string Guidetapa_id = Guid.NewGuid().ToString();
            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";
            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);
            try
            {
                Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                     where d.No_Registro == No_Registro
                                                     select d).FirstOrDefault();

                tbl_RNF_Registro.ConstanciaFirmada = ".pdf";
                if (tbl_RNF_Registro.Fecha_Inscripcion == null)
                {
                    tbl_RNF_Registro.Fecha_Inscripcion = DateTime.Now;
                }

                string archivonuevo = EmitirDocumento_PDF_API(tbl_RNF_Registro.No_Registro);

                /// bbarillas
                rootpdf = rootpath + archivonuevo;
                /// bbarillas

                Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioDirectorRegionalController-JsonProcesarFirmaElectronicaConstanciaNueva");


                Usuario objUs = new Usuario();
                RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

                if (!objSesion.getBlSession())
                {
                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró generar bearer, su sesión ha expirado." + "\"}";

                    return Json(jsonResultUsr);
                }
                else
                {
                    objUs = (Usuario)Session["User"];
                }

                Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 2, "B.- Se busca obtener el bearer");

                strBearer = GetBearer();

                Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 3, "C.- Bearer obtenido");

                //            strBearer = GetBearerNeftafiufiu();
                if (strBearer.Length < 125)
                {

                    Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 4, "D.- Bearer erroneo, menor a 125 caracteres");

                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                    return Json(jsonResultUsr);

                }

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
                {

                    Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 5, "N.- Error al subir el documento.");

                    intRespuesta = 0;

                    strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                    return Json(jsonResultUsr);


                }

                RequestUtil requestUtil = new RequestUtil();
                FirmarArchivos_Documentos_Request newParams = null;
                if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 1))
                {
                    newParams = new FirmarArchivos_Documentos_Request
                    {
                        Coordenadas = "290,670,480,700",
                    };
                }

                string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido, newParams);
                //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

                if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
                {

                    Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 6, "R.- Error al firmar el documento, Usuario o Password Erroneos.");

                    intRespuesta = 0;

                    strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                    return Json(jsonResultUsr);


                }

                Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 7, "W.- Intentando obtener archivo firmado.");

                if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
                {

                    string SP_SqlQuery = "EXEC [dbo].[SP_RNF_Registro_ActualizaDocumentoFirmado_ConstanciaNueva] @No_Registro, @Firmante, @NombreDocumentoFirmado";
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@No_Registro", Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Firmante", Value = strEnc, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@NombreDocumentoFirmado", Value = strDocumentofirmado + ".pdf", Direction = System.Data.ParameterDirection.Input },
                    };

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();


                    //tbl_RNF_Registro.ConstanciaFirmada = strDocumentofirmado + ".pdf";
                    ////tbl_RNF_Registro.Estado_id = 4;
                    //if (tbl_RNF_Registro.Fecha_Inscripcion == null)
                    //{
                    //    tbl_RNF_Registro.Fecha_Inscripcion = DateTime.Now;
                    //}
                    //tbl_RNF_Registro.ResolucionInscripcionFecha = DateTime.Now;
                    //db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                    //db.SaveChanges();

                    Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 8, "X.- El archivo se obtuvo con exito.");

                }


                intRespuesta = 1;

                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

                return Json(jsonResultUsr);


            }
            catch (Exception ex)
            {

                Constants.FirmaElectronicaInsertarBitacora(No_Registro, No_Registro, strEnc, 1, 10, "Z.- No se pudo generar la FE" + ex.Message.ToString());


                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró realizar la firma electrónica. " + ex.Message.ToString() + "\"}";

                return Json(jsonResultUsr);

            }

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

            //Procedimiento para: Cancelaciones

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
                strRespuesta = "Actualización realizada.";

                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


            }
            else
            {
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
                if((Respuesta.mensaje ??"").Trim() != "")
                {
                    strRespuesta = Respuesta.mensaje;
                }
                codRespuesta = 0;
            }

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + Newtonsoft.Json.JsonConvert.SerializeObject(strRespuesta) + "}";

            return Json(jsonResult);

        }


        private void LlenaBannerMotosierra(Tbl_RNF_Registro tbl_RNF_Registro, fc_RNF_DatosInscripcion_Result Resultado)
        {

            /// Titulo

            iTextSharp.text.Font fntTituloTabla = fntTituloTabla_MTS;
            iTextSharp.text.Font fntTablasCeldas = fntTituloTabla_MTS;

            bool boolFechaInscripcion = false;
            bool boolFechaActualizacion = false;
            bool boolFechaRatificacion = false;
            bool boolFechaVencimiento = false;
            bool boolProcedenciaExterna = false;
            bool boolFechaInactivacion = false;

            decimal tipogestion = ((decimal)tbl_RNF_Registro.SolicitudTipo_id) - Math.Truncate(((decimal)tbl_RNF_Registro.SolicitudTipo_id));

            //20231103 - Nefta: Ante la posibilidad de que le cambien estos textos al tipo de registro, es mejor validarlo según el tipo de solicitudtipoid

            if (tipogestion == 0)
            {
                boolFechaInscripcion = true;
                boolFechaVencimiento = true;
            }

            if (tipogestion == 0.01M)
            {
                boolFechaInscripcion = true;
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
            }

            if (tipogestion == 0.02M)
            {
                boolFechaInscripcion = true;
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
            }


            if (tipogestion == 0.03M)
            {
                boolFechaInscripcion = true;
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
                boolFechaRatificacion = true;
            }

            if (tipogestion == 0.04M)
            {
                boolFechaInscripcion = true;
                boolFechaInactivacion = true;
            }

            Tbl_RNF_Motosierra tbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();

            string marca, modelo, cilindraje, potencia, noserie;
            marca = modelo = cilindraje = potencia = noserie = "";

            if ((tbl_Sol_Motosierra.Marca != null) && (tbl_Sol_Motosierra.Marca.Trim() != ""))
            {
                marca = tbl_Sol_Motosierra.Marca;
            }
            if ((tbl_Sol_Motosierra.Modelo != null) && (tbl_Sol_Motosierra.Modelo.Trim() != ""))
            {
                modelo = tbl_Sol_Motosierra.Modelo;
            }
            if ((tbl_Sol_Motosierra.Cilindraje != null) && (tbl_Sol_Motosierra.Cilindraje.Trim() != ""))
            {
                cilindraje = tbl_Sol_Motosierra.Cilindraje;
            }
            if ((tbl_Sol_Motosierra.Potencia != null) && (tbl_Sol_Motosierra.Potencia.Trim() != ""))
            {
                potencia = tbl_Sol_Motosierra.Potencia;
            }
            if ((tbl_Sol_Motosierra.No_SerieMotosierra != null) && (tbl_Sol_Motosierra.No_SerieMotosierra.Trim() != ""))
            {
                noserie = tbl_Sol_Motosierra.No_SerieMotosierra;
            }



            tableCarnetMotosierra = new PdfPTable(12);

            PdfPCell c1 = new PdfPCell();

            /// Linea 
            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.Border = 0;
            c1.BorderWidthRight = 1;
            c1.BorderWidthLeft = 1;
            c1.BorderWidthTop = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 12;
            c1.Rowspan = 1;

            tableCarnetMotosierra.AddCell(c1);
            /// Linea 

            /// Linea top  bbarillas
            //// 5  Lineas ///////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Border = 0;
            c1.Colspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Rowspan = 3;

            tableCarnetMotosierra.AddCell(c1);


            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/images/logoInabExcel.jpg"));

            logo.ScalePercent(80f);

            c1 = new PdfPCell(logo);


            c1.Colspan = 1;
            c1.Rowspan = 3;
            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase("GUATEMALA, C.A.", fntTituloTabla));

            c1.Border = 1;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableCarnetMotosierra.AddCell(c1);


            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);


            c1.Colspan = 1;
            c1.Rowspan = 3;
            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableCarnetMotosierra.AddCell(c1);

            c1.Border = 1;
            c1.BorderWidthRight = 1;

            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);


            c1.Colspan = 1;
            c1.Rowspan = 3;
            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("REGISTRO No.", fntTituloTabla));

            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;

            tableCarnetMotosierra.AddCell(c1);


            logo.ScalePercent(40f);

            c1 = new PdfPCell(logo);


            c1.Colspan = 1;
            c1.Rowspan = 3;
            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableCarnetMotosierra.AddCell(c1);



            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.Border = 0;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.BorderWidthRight = 1;

            c1.Colspan = 1;
            c1.Rowspan = 3;

            tableCarnetMotosierra.AddCell(c1);

            //// 5  Lineas ///////


            c1 = new PdfPCell(new Phrase("CONSTANCIA DE REGISTRO", fntTituloTabla));

            c1.Border = 1;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_RNF_Registro.No_Registro, fntTituloTabla));

            c1.Border = 1;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableCarnetMotosierra.AddCell(c1);



            c1 = new PdfPCell(new Phrase("MOTOSIERRAS", fntTituloTabla));

            c1.Border = 1;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.Border = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableCarnetMotosierra.AddCell(c1);
            /// Titulo

            //BBARILLAS

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ////////////////////////////////////////////////////////////////////////////////


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(StrMotosierraPropietario, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 4;
            c1.Rowspan = 2;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////

            if (boolFechaInscripcion == true)
            {

                c1 = new PdfPCell(new Phrase("Fecha Inscripcion : ", fntTituloTabla));


                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_RNF_Registro.Fecha_Inscripcion ?? DateTime.Now).ToString("dd/MM/yyyy"), fntTituloTabla));

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);

            }







            ///////////////////////////////////////////////


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);




            string strTitulo = "";
            string strFecha = "";

            if (boolFechaVencimiento == true)
            {
                strTitulo = "Fecha Vencimiento";
                strFecha = (tbl_RNF_Registro.Fecha_De_Vencimiento ?? DateTime.Now).ToString("dd/MM/yyyy");

            }

            if (boolFechaInactivacion == true)
            {
                strTitulo = "Fecha de Inactivación";
                strFecha = (tbl_RNF_Registro.Fecha_Inactivacion ?? DateTime.Now).ToString("dd/MM/yyyy");

            }


            c1 = new PdfPCell(new Phrase(strTitulo, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(strFecha, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);




            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);


            ////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            if (boolFechaActualizacion == true)
            {
                strTitulo = "Fecha Actualización";
                strFecha = (tbl_RNF_Registro.Fecha_Actualizacion ?? DateTime.Now).ToString("dd/MM/yyyy");

            }

            if (boolFechaRatificacion == true)
            {
                strTitulo = "Fecha de Actualización: ";
                strFecha = (tbl_RNF_Registro.Fecha_Actualizacion ?? DateTime.Now).ToString("dd/MM/yyyy");

            }


            if (boolFechaInactivacion == true)
            {
                strTitulo = "Fecha de Inactivación: ";
                strFecha = (tbl_RNF_Registro.Fecha_Inactivacion ?? DateTime.Now).ToString("dd/MM/yyyy");

            }

            if ((boolFechaActualizacion == false) && (boolFechaActualizacion == false) && (boolFechaActualizacion == false))
            {
                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);


                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);
            }
            else
            {

                c1 = new PdfPCell(new Phrase(strTitulo, fntTituloTabla));

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);


                c1 = new PdfPCell(new Phrase(strFecha, fntTituloTabla));


                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;

                c1.Colspan = 2;
                c1.Rowspan = 1;
                c1.Border = 0;
                tableCarnetMotosierra.AddCell(c1);
            }


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);


            ////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("No. DE SERIE :", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.BorderWidthRight = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(noserie, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.BorderWidthRight = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthRight = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            //byte[] newQR;
            //string ippuerto = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault().Direccion_URL;
            //string text = ippuerto + "/Documentos/" + tbl_RNF_Registro.Guid_id + ".pdf";
            //newQR = qrCodeBytes(text);
            //long ticks = DateTime.Now.Ticks;
            //string rootqr = Server.MapPath($"~/Archivos_Generados_Que_Pueden_Borrar/QR_{ticks}.png");

            //System.IO.File.WriteAllBytes(rootqr, newQR);


            string rootqr = GenerarQR(tbl_RNF_Registro);
            logo = iTextSharp.text.Image.GetInstance(rootqr);

            logo = iTextSharp.text.Image.GetInstance(rootqr);

            logo.ScalePercent(5f);

            c1 = new PdfPCell(logo);

            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 4;

            tableCarnetMotosierra.AddCell(c1);

            //


            c1 = new PdfPCell(new Phrase("Fecha de impresión:", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.BorderWidthRight = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Tipo: Motosierra", fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Marca :", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(marca, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);



            c1 = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);



            c1 = new PdfPCell(new Phrase("Modelo :", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(modelo, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Cilindraje:", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(cilindraje, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("No. RESOLUCIÓN", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            tableCarnetMotosierra.AddCell(c1);



            c1 = new PdfPCell(new Phrase("Potencia :", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(potencia, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            tableCarnetMotosierra.AddCell(c1);

            string TextoResolucion = "";
            if (Resultado.ResolucionInscripcion.Contains("No asignada") == false)
            {
                TextoResolucion = Resultado.ResolucionInscripcion;
            }
            if (Resultado.ResolucionUltimaActualizacion.Contains("No asignada") == false)
            {
                TextoResolucion = Resultado.ResolucionUltimaActualizacion;
            }

            c1 = new PdfPCell(new Phrase(TextoResolucion, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.BorderWidthLeft = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            tableCarnetMotosierra.AddCell(c1);


            ///////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
           
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 5;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
        

            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("* Previo a escanear el código QR, debe", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
         

            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////
            ///

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;

            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 5;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("habilitar las ventanas emergentes. ", fntTituloTabla));


            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;

            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 5;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);




            c1 = new PdfPCell(new Phrase("Validación de Constancia: https://consultarnf.inab.gob.gt/", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;

            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 5;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;


            tableCarnetMotosierra.AddCell(c1);

            ///////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthLeft = 1;
            c1.BorderWidthBottom = 1;
            tableCarnetMotosierra.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Se recomienda escaner el QR para validar el presente documento ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 5;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            c1.BorderWidthBottom = 1;

            tableCarnetMotosierra.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Decreto 122-96 Ley Reguladora del Registro, autorización y uso de motosierras. ", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 6;
            c1.Rowspan = 1;
            c1.Border = 0;
            c1.BorderWidthRight = 1;
            c1.BorderWidthBottom = 1;

            tableCarnetMotosierra.AddCell(c1);

        

            return;
        }


    }
}
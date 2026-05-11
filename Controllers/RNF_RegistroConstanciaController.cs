using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using RNF_Web.Models;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Image = iTextSharp.text.Image;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class RNF_RegistroConstanciaController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();
        CrearBanner crearBanner = new CrearBanner();


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

        // GET: RNF_RegistroConstancia
        public ActionResult Index()
        {
            return View();
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

        public void LlenaUnTextos(string TextoA)
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

        public void LlenaUnTextoResaltado(string TextoA)
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

        public void LlenaDos_UnoTextosDosResaltadoUnidos(string TextoA, string TextoB)
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

        public void LlenaDosTextos(string TextoA, string TextoB)
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

        public void LlenaDosTextosPorCuatro(string TextoA, string TextoB)
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

        public void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 11);

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

        public void LlenaCuatroTextosResaltado(string TextoA, string TextoB, string TextoC, string TextoD)
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

            //bbarillas


            byte[] newQR;
            string ippuerto = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault().Direccion_URL;
            string text = ippuerto + "/Documentos/" + tbl_RNF_Registro.Guid_id + ".pdf";
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


            c1 = new PdfPCell(new Phrase("", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;


            tableTitulo.AddCell(c1);

            return;
        }
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
            public List<string> PropietariosIndividuales { get; set; }
            public List<string> PropietariosJuridicos { get; set; }
            public List<string> RepresentatnteLegal { get; set; }
            public List<string> Mandatario { get; set; }
            public List<string> ArrendatariosIndividuales { get; set; }
            public List<string> ArrendatariosJuridicos { get; set; }
        }


        Personerias ObtenerPersonerias_RNF(string No_Registro)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<string>();
            personerias.PropietariosJuridicos = new List<string>();
            personerias.RepresentatnteLegal = new List<string>();
            personerias.Mandatario = new List<string>();
            personerias.ArrendatariosIndividuales = new List<string>();
            personerias.ArrendatariosJuridicos = new List<string>();

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



        private void ResumenPV_RNF(string No_Registro)
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
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

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
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.000")}", fntSubTitulo));
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
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.000")}", fntSubTitulo));
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

            tableEstimacion = new PdfPTable(12);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

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
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.000")}", fntSubTitulo));
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
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.000")}", fntSubTitulo));
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


        private void DatosEmpresas_RNF(string No_Registro, fc_RNF_Sel_Direccion_Result DatosDireccion)
        {

            //Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(solicitud_id);
            Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad = (from d in db.Tbl_RNF_Empresa_Entidad
                                                               where d.No_Registro == No_Registro
                                                               select d).FirstOrDefault();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
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
                c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombre_Tecnico, fntTituloTabla));
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
            c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad.Nombre, fntTituloTabla));
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
            c1 = new PdfPCell(new Phrase($"{(tbl_RNF_Empresa_Entidad.GTMX ?? 0).ToString("0")}, {(tbl_RNF_Empresa_Entidad.GTMY ?? 0).ToString("0")}", fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = 0;
            tableEstimacion.AddCell(c1);

            return;
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

        private void DatosMotosierras_RNF(string No_Registro)
        {

            //Tbl_Sol_Motosierra tbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == solicitud_id).FirstOrDefault();
            Tbl_RNF_Motosierra tbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 15, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 15, iTextSharp.text.Font.BOLD);
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



        public string EmitirDocumento_PDF_RNF(string No_Registro)
        {


            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            bool boolFechaInscripcion = false;
            bool boolFechaActualizacion = false;
            bool boolFechaVencimiento = false;

            if (tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro == "Inscripción")
            {
                boolFechaInscripcion = true;
                boolFechaVencimiento = true;
            }

            if (tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro == "Primer Actualización")
            {
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
            }

            if (tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro == "Segunda Actualización")
            {
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
            }

            if (tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro == "Ratificación")
            {
                boolFechaActualizacion = true;
                boolFechaVencimiento = true;
            }

            if (tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro == "Inactivación")
            {

            }

            string sqlQuery;

            sqlQuery = " Select * From dbo.fc_RNF_DatosInscripcion('" + tbl_RNF_Registro.No_Registro + "')";
            DatosInscripcion Resultado = new DatosInscripcion();
            Resultado = db.Database.SqlQuery<DatosInscripcion>(sqlQuery).FirstOrDefault();

            sqlQuery = "Select dbo.[Fnc_RNF_Sel_DocumentosPropiedad]('" + tbl_RNF_Registro.No_Registro + "')";
            string documentospropiedad = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == tbl_RNF_Registro.UsuarioExterno_id
                                                             select d).FirstOrDefault();

            fc_RNF_Sel_Direccion_Result fc_RNF_Sel_Direccion_Result = (from d in db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro)
                                                                       select d).FirstOrDefault();
            List<fc_RNF_Sel_Direccion_Result> fc_RNF_Sel_Direccion_Results = (from d in db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro)
                                                                              select d).ToList();

            strNombre = tbl_RNF_Registro.No_Registro + tbl_RNF_Registro.Expediente + ".pdf";
            strNombre = tbl_RNF_Registro.Guid_id + tbl_RNF_Registro.Expediente + ".pdf";


            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();

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

            //LlenaTituloRevision(tbl_sol_Solicitud);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);

            LlenaSubTituloRevision_RNF(tbl_RNF_Registro);
            doc.Add(tableTitulo);
            doc.Add(Enter);

            //bbarillas

            LlenaCuatroTextos("", "", "", "Registro No. " + tbl_RNF_Registro.No_Registro);
            doc.Add(tableTitulo);

            LlenaCuatroTextos("", "", "", "Región " + Resultado.Region + " Subregión " + Resultado.SubRegion);
            doc.Add(tableTitulo);

            //LlenaCuatroTextos("Subcategoría :", Resultado.SubCategoria, "", "");
            //doc.Add(tableTitulo);

            //LlenaUnTextoResaltado(Resultado.SubCategoria);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);

            LlenaDos_UnoTextosDosResaltadoUnidos("Subcategoría", Resultado.SubCategoria);
            //LlenaCuatroTextos("Subcategoría", "", "", "");
            doc.Add(tableTitulo);

            string Modalidad = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id && Obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id).First().Modalidad;
            string subModalidad = "";
            try
            {
                subModalidad += db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id && Obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_RNF_Registro.Sub_Sub_Categoria_id).First().Modalidad;
            }
            catch (Exception ex)
            {
                subModalidad = "";
            }

            Modalidad = Modalidad + subModalidad;


            Constants Const = new Constants();

            if (Modalidad.ToUpper().Contains("HULE"))
            {
                Modalidad = "Hule";
            }

            if (Modalidad.ToUpper().Contains("PINABETE"))
            {
                Modalidad = "Pinabete";
            }

            if ((Modalidad != "Hule") && (Modalidad != "Pinabete"))
            {
                Modalidad = "";
            }

            if (Modalidad != "")
            {
                Modalidad = Const.initCapTexto(Modalidad);

                LlenaDos_UnoTextosDosResaltadoUnidos("Modalidad :", Modalidad);
                doc.Add(tableTitulo);

            }


            Personerias personerias = ObtenerPersonerias_RNF(tbl_RNF_Registro.No_Registro);

            string datoreemplazar = "";
            List<string> datolistreemplazar = new List<string>();

            datolistreemplazar = personerias.PropietariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Propietario:", item);
                    doc.Add(tableTitulo);
                }
            }
            datolistreemplazar = personerias.PropietariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Propietario:", item);
                    doc.Add(tableTitulo);
                }
            }
            datolistreemplazar = personerias.RepresentatnteLegal;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Representante Legal:", item);
                    doc.Add(tableTitulo);
                }
            }
            datolistreemplazar = personerias.Mandatario;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Mandatario:", item);
                    doc.Add(tableTitulo);
                }
            }
            datolistreemplazar = personerias.ArrendatariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Arrendatario:", item);
                    doc.Add(tableTitulo);
                }
            }
            datolistreemplazar = personerias.ArrendatariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    LlenaDos_UnoTextosDosResaltadoUnidos("Arrendatario:", item);
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

                LlenaDos_UnoTextosDosResaltadoUnidos("Área inscrita ha.:", Resultado.AreaInscritaha.ToString() + " ha ");
                doc.Add(tableTitulo);

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


                //Coordenadas_Rodal_Tecnico_API(tbl_RNF_Registro.Solicitud_id);
                //doc.Add(tableEstimacion);

            }
            if ((tbl_RNF_Registro.Categoria_id == 5) || (tbl_RNF_Registro.Categoria_id == 9) || ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2)))
            {

                DatosEmpresas_RNF(No_Registro, fc_RNF_Sel_Direccion_Result);
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

            string fechaimpresion = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_FechaTxtSinGuatemala](getdate())").FirstOrDefault();
            LlenaUnTextos("Inscripción según resolución No. " + Resultado.ResolucionInscripcion + ", de fecha " + fechaimpresion);
            doc.Add(tableTitulo);

            if ((tbl_RNF_Registro.ConstanciaFirmada != null) && boolFechaInscripcion)
            {
                LlenaDosTextosPorCuatro("Fecha de Inscripción : ", Resultado.FechaResolucion);
                doc.Add(tableTitulo);
            }

            if (boolFechaActualizacion)
            {
                LlenaDosTextosPorCuatro("Fecha de actualización : ", Resultado.FechaRegistro);
                doc.Add(tableTitulo);
            }

            if (boolFechaVencimiento)
            {
                LlenaDosTextosPorCuatro("Fecha de vencimiento: ", Resultado.FechaVencimiento);
                doc.Add(tableTitulo);
            }

            LlenaDosTextosPorCuatro("Estado del registro: ", tbl_RNF_Registro.Tbl_RNF_Registro_Estado.Descripcion);
            doc.Add(tableTitulo);

            doc.Add(Enter);

            if ((tbl_RNF_Registro.Categoria_id == 5) && (tbl_RNF_Registro.Sub_Categoria_id == 3))
            {
                LlenaBanner("Nota: Centro de acopia", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner("no autorizado, para la venta de productos forestales.", "Centro", "Blanco");
                doc.Add(tableBanner);

                doc.Add(Enter);
            }


            //FirmaSolicitante();
            //doc.Add(tableFirmaSolicitante);

            pdfContentByte.SetGState(pdfGState);
            pdfContentByte.AddImage(image);

            doc.Close();
            writer.Close();

            return strNombre;
        }

        public JsonResult GenerarPDFRNF(string No_Registro)
        {
            string TextoMostrar, Nombre;
            TextoMostrar = "";
            Nombre = EmitirDocumento_PDF_RNF(No_Registro);
            TextoMostrar = "{ \"Ubicacion\" : \"" + Nombre + "\"}";
            return Json(TextoMostrar);
        }

    }
}
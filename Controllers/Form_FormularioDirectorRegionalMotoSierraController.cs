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

namespace RNF_Web.Controllers
{
    public class Form_FormularioDirectorRegionalMotoSierraController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosNotificacion = new PdfPTable(numColumns: 8);
        private PdfPTable tablePersoneria = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);

        public ActionResult SolicitudAutorizar(string Guid_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            ViewBag.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.Guid_id = Guid_id;

            return View();

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

        private void LlenaSubTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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


            c1 = new PdfPCell(new Phrase("Guatemala, C.A.", fntTituloTabla));


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
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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

        private void LlenaDosTextos(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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

        private void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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

            c1 = new PdfPCell(new Phrase("\nPROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
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

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, iTextSharp.text.Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{Nombre}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 4;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"NIT: {No_Nit}", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Teléfono Celular", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].Nombres} {oMandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, iTextSharp.text.Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Dirección:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Direccion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oMunicipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oDepartamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Correo Electrónico:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Correo}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 5;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"No. de Celular", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Telefono_Celular}, {tbl_Seg_UsuarioExterno.Telefono_Oficina}, {tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            tableDatosNotificacion.AddCell(c1);

            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaDatosNotificacionDetallada(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);


            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************


            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("NIT: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_NIT, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Fecha Nacimiento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Fecha_Nacimiento.ToString("dd/MM/yyyy"), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Tipo de Documento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Número de documento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_Documento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Departamento emisión: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Municipio emisión: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Observaciones: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }

        private void FirmaSolicitante(Tbl_Sol_Solicitud tbl_sol_Solicitud)
        {

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


            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 11);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.Rowspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            ///   Firma

            iTextSharp.text.Image logo;

            ///   Firma
            try
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/FirmaDigital_" + objUs.intUsuario_id.ToString() + ".png"));

            }
            catch (Exception excep)
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/rubrica.png"));
            }

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
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.Rowspan = 2;
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


            string strDirectorRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{strDirectorRegional}", fntTituloTabla));
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



            tableBanner.AddCell(c1);
            return;
        }

        public class DatosInscripcionMotosierra
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

        }

        public string EmitirDocumento_PDF(long Solicitud_id, string GuidEtapa_id)
        {

            string sqlQuery;

            sqlQuery = " Select * From dbo.fc_SolRNF_DatosInscripcion(" + Solicitud_id.ToString() + ")";


            List<DatosInscripcionMotosierra> Resultado = new List<DatosInscripcionMotosierra> { };

            Resultado = db.Database.SqlQuery<DatosInscripcionMotosierra>(sqlQuery).ToList();

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
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == objUs.intUsuario_id
                                                             select d).FirstOrDefault();

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();


            strNombre = GuidEtapa_id + ".pdf";

            if (tbl_Sol_PropietarioPersonaJuridica != null)
            {
                strNombrePersona = $"{tbl_Sol_PropietarioPersonaJuridica.Nombre.ToUpper()}";
            }
            else
            {
                strNombrePersona = $"{tbl_Sol_PropietarioPersonaIndividual.Nombres.ToUpper()} {tbl_Sol_PropietarioPersonaIndividual.Apellidos.ToUpper()}";
            }

            strDirArchivo = strFolder + strNombre;

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

            // imagen de fondo

            string imagenMarcaAgua = Server.MapPath("~/Content/images/logoInab_VerticalSello.png");
            float posX, posY;
            Image image = Image.GetInstance(imagenMarcaAgua);
            PdfGState pdfGState = new PdfGState();
            pdfGState.FillOpacity = 0.3f;
            PdfContentByte pdfContentByte = writer.DirectContentUnder;
            posX = (writer.PageSize.Right / 2) - (image.Width / 2);
            posY = (writer.PageSize.Top / 2) - (image.Height / 2);
            image.SetAbsolutePosition(posX, posY);

            //  imagen de fondo

            LlenaTituloRevision(tbl_sol_Solicitud);
            doc.Add(tableTitulo);
            doc.Add(Enter);


            LlenaSubTituloRevision(tbl_sol_Solicitud);
            doc.Add(tableTitulo);
            doc.Add(Enter);


            LlenaCuatroTextos("", "", "Registro No. ", tbl_sol_Solicitud.No_Registro);
            doc.Add(tableTitulo);

            LlenaCuatroTextos("", "", "", "Región " + Resultado[0].Region + " Subregión " + Resultado[0].SubRegion);
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Subcategoría :", Resultado[0].SubCategoria, "", "");
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Propietario :", Resultado[0].Propietario, "", "");
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Documento de propiedad :", Resultado[0].ConstanciaPropiedad, "", "");
            doc.Add(tableTitulo);

            LlenaUnTextos("Lugar de ubicación del terreno : " + Resultado[0].Direccion);
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Área inscrita ha.:", Resultado[0].AreaInscritaha.ToString() + " ha ", "", "");
            doc.Add(tableTitulo);

    
            LlenaUnTextos("Inscripción según resolución No. " + Resultado[0].ResolucionInscripcion + ", de fecha " + Resultado[0].FechaResolucion + ".");
            doc.Add(tableTitulo);

            LlenaUnTextos("Fecha de inscripción: " + Resultado[0].FechaRegistro);
            doc.Add(tableTitulo);

            LlenaUnTextos("Fecha de vencimiento: ");
            doc.Add(tableTitulo);

            LlenaUnTextos("Estado del registro: Vigente ");
            doc.Add(tableTitulo);

            FirmaSolicitante(tbl_sol_Solicitud);
            doc.Add(tableFirmaSolicitante);

            pdfContentByte.SetGState(pdfGState);
            pdfContentByte.AddImage(image);

            doc.Close();
            writer.Close();

            return strNombre;
        }

        [HttpPost]
        public JsonResult EmitirDocumento(int Solicitud_id, string Guid_id, string EtapaSolicitudGuid, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == Solicitud_id && d.EtapaSolicitud_GUID_id == EtapaSolicitudGuid
                                                               select d).FirstOrDefault();

            string TextoMostrar, Nombre;
            Nombre = EmitirDocumento_PDF(Solicitud_id, EtapaSolicitudGuid);

            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = Nombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            TextoMostrar = "{ \"Ubicacion\" : \"" + Nombre + "\"}";
            return Json(TextoMostrar);
        }
    }
}

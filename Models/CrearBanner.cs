using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RNF_Web.Models
{
    public class CrearBanner
    {
        public PdfPTable tableBanner = new PdfPTable(1);
        public PdfPTable tableTitulo = new PdfPTable(1);
        public Paragraph Enter = new Paragraph(" ");

        public void LlenaTituloRevision(string Titulo, string Codigo, string Version, string Fecha_Implementacion, string strlogo)
        {

            //Codigo = "---";
            //Version = "---";
            //Fecha_Implementacion = "---";

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 9);

            tableTitulo = new PdfPTable(8);

            //// Imagen superior
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(strlogo);

            logo.ScalePercent(50f);

            PdfPCell c1 = new PdfPCell(logo);

            c1.Colspan = 2;
            c1.Rowspan = 3;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Titulo.ToUpper(), fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 2;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 2;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Codigo, fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            c1.Colspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Version, fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("PROCESO: REGISTRO NACIONAL FORESTAL", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Colspan = 2;
            c1.Rowspan = 1;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Fecha_Implementacion, fntTablasCeldas));
            c1.Rowspan = 1;
            tableTitulo.AddCell(c1);


            return;
        }

        public void LlenaBanner(String Leyenda, string alineacion, string Color)
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

    }
}
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
using RestSharp;
using Newtonsoft.Json;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoReporteDenegacionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
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

        public ActionResult GenerarAprobacion(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud Tbl_Sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            //http://localhost:61244/Form_FormularioTecnicoReporteAprobacion/GenerarPVJuridico_PDF?solicitud_id=7

            return View(Tbl_Sol_solicitud);

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

            c1 = new PdfPCell(new Phrase("\nINFORME TÉCNICO PARA INSCRIPCIÓN DE " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("RF-RE-031", fntTablasCeldas));
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

        private void LlenaDatosGenerales(Tbl_Sol_Solicitud tbl_sol_Solicitud, int CantidadFincas, int CantidadEmpresas, Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud, long Usuario_id)
        {


            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            string Const_No_Informe = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_sol_Solicitud.Solicitud_id, tbl_gest_EtapaSolicitud.Etapa_id, tbl_gest_EtapaSolicitud.EtapaRuta_id, tbl_gest_EtapaSolicitud.CorrelativoEtapa_id, Usuario_id).Identificador;

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();

            string strNombreDelDirectorRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Gral_NombreDirectorRegional(@p0,@p1)", tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).FirstOrDefault();
            string strNombreDelDirectorSubRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Gral_NombreSubDirectorRegional(@p0,@p1)", tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).FirstOrDefault();

            tableDatosGenerales = new PdfPTable(12);

            PdfPCell c1 = new PdfPCell();

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);
            Font fntTitulo3 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Informe No.", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Const_No_Informe, fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Fecha", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(strFecha, fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            //-------- Inicio Enter --------
            c1 = new PdfPCell(new Phrase("  ", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            tableDatosGenerales.AddCell(c1);



            c1 = new PdfPCell(new Phrase($"Nombre del Director Sub-Regional", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(strNombreDelDirectorSubRegional, fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Dirección Subregional ", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegionCompleto, fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosGenerales.AddCell(c1);


            //-------- Inicio Enter --------
            c1 = new PdfPCell(new Phrase("  ", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            tableDatosGenerales.AddCell(c1);

            //-------- fin Enter --------

            if (CantidadFincas > 1)
            {
                c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguientes fincas.", fntTitulo3));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                tableDatosGenerales.AddCell(c1);
            }

            if (CantidadFincas == 1)
            {
                c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguiente finca.", fntTitulo3));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                tableDatosGenerales.AddCell(c1);
            }

            if ((CantidadEmpresas == 1) && (tbl_sol_Solicitud.Categoria_id == 5))
            {
                c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguiente empresa.", fntTitulo3));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                tableDatosGenerales.AddCell(c1);
            }

            return;
        }

        private void LlenaDatosFincaMapaEvaluar(Tbl_Sol_Finca tbl_Sol_Finca, int FincaId)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(12);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"3. Se adjunta mapa del área evaluada.", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 12;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);



            c1 = new PdfPCell(new Phrase($"FINCA " + FincaId.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            ///////////////////////////////////////

            c1 = new PdfPCell(new Phrase($"4. La plantación se encuentra en área protegida-SIGAP ? ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            if (tbl_Sol_Finca.Area_SIGAP == true)
            {
                c1 = new PdfPCell(new Phrase($"SI", fntTituloTabla));
            }
            else
            {
                c1 = new PdfPCell(new Phrase($"NO", fntTituloTabla));
            }

            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;


            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Nombre del área", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreAreaSIGAP}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);


            ///////////////////////////////////////


        }

        private void LlenaDatosFincaResultado(Tbl_Sol_Finca tbl_Sol_Finca, int FincaId)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(12);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"FINCA " + FincaId.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            ///////////////////////////////////////

            c1 = new PdfPCell(new Phrase($"1. El objetivo de la plantacion es ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Sol_FincaObjetivoDeLaPlantacion.Descripcion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Su procedencia es de ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);


            string strNombreProcedencia = db.Database.SqlQuery<string>("Select dbo.Fnc_Gral_SubSubCategoriaNombre(@p0,@p1,@p2)", tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id, tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Categoria_id, tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Sub_Categoria_id).FirstOrDefault();

            c1 = new PdfPCell(new Phrase($"{strNombreProcedencia}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);


            ///////////////////////////////////////

            c1 = new PdfPCell(new Phrase($"2. El área a total a registrar es de ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Hectareas", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

        }

        private void LlenaDatosFinca(Tbl_Sol_Finca tbl_Sol_Finca, int FincaId)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(12);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"FINCA " + FincaId.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

        }

        private void LlenaDatosFincaDireccion(Tbl_Sol_Finca tbl_Sol_Finca, int FincaId)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(12);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"FINCA " + FincaId.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"Dirección de la finca ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Ubicacion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);



            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Municipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            tableDatosFinca.AddCell(c1);

            return;
        }

        public class PersonaJuridica
        {
            public string Tipo { get; set; }
            public string Nombre { get; set; }
            public string TipoDocto { get; set; }
            public string No_Documento { get; set; }
        }

        private void LlenaDatosPersonaInividualJuridica(long solicitud_id)
        {
            string sqlQuery;

            sqlQuery = " Select 'Propietario jurídico' Tipo, Nombre, 'NIT' TipoDocto, No_Nit No_Documento ";
            sqlQuery += " From Tbl_Sol_PropietarioPersonaJuridica ";
            sqlQuery += " Where Solicitud_id = " + solicitud_id.ToString();
            sqlQuery += " Union ";
            sqlQuery += " Select 'Propietario individual' Tipo, Nombres + ' ' + Apellidos Nombre, dbo.Fnc_Gral_DocumentoIDNombre(DocumentoID_Tipo) TipoDocto, No_Documento ";
            sqlQuery += " From Tbl_Sol_PropietarioPersonaIndividual ";
            sqlQuery += " Where Solicitud_id = " + solicitud_id.ToString();
            sqlQuery += " Union ";
            sqlQuery += " Select dbo.Fnc_Gral_RepresentanteLegalTipoNombre(RepresentanteLegatTipo_id) Tipo, Nombres + ' ' + Apellidos Nombre, dbo.Fnc_Gral_DocumentoIDNombre(RepresententeDocumentoID_Tipo) TipoDocto, RepresentanteNo_Documento ";
            sqlQuery += " From Tbl_Sol_RepresentanteLegal ";
            sqlQuery += " Where Solicitud_id = " + solicitud_id.ToString();

            List<PersonaJuridica> Resultado = new List<PersonaJuridica> { };


            Resultado = db.Database.SqlQuery<PersonaJuridica>(sqlQuery).ToList();

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tablePersoneria = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);



            for (int i = 0; i < Resultado.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{Resultado[i].Nombre}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{Resultado[i].TipoDocto}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{Resultado[i].No_Documento}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tablePersoneria.AddCell(c1);


            }
        }

        private void EstimacionVolumen(Tbl_Sol_Finca tbl_Sol_Finca)
        {
            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();



            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
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
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMX}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMY}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
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
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
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
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Area_Efectiva_Rodal}", fntTituloTabla));
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

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Cantidad_Arboles}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Densidad_ha}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].AlturaPromedio}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].AreaBasal_ha}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].Volumen_Rodal}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 6;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString(), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString(), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 6;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString(), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString(), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                                c1.Colspan = 5;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{volumenha}", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{volumenrodal}", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
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
                            c1.Colspan = 5;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{volumenha}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{volumenrodal}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
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

                        }
                    }
                    else if (validacionesDiametrica[i].Tipo_de_Area == 2)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {
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
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMX}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.GTMY}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
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

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Area_Efectiva_Rodal}", fntTituloTabla));
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

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Cantidad_Arboles}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].AlturaPromedio}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{(decimal)validacionesDiametrica[i].Volumen_X_Linea}", fntTituloTabla));
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

                        try
                        {
                            if (varEspecie != validacionesDiametrica[i + 1].Especie)
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 5;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{SubTotal_Volumenlineam2}", fntTituloTabla));
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
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 5;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{SubTotal_Volumenlineam2}", fntTituloTabla));
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
                                c1.Colspan = 4;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"{Total_Volumenlineam2}", fntTituloTabla));
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
                            c1.Colspan = 4;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{Total_Volumenlineam2}", fntTituloTabla));
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

                        }
                    }


                }


                tableEstimacion.AddCell(c1);

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

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosPlantacion = new PdfPTable(11);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            string DescripcionPlantacion, DescripcionCategoriaSIGAP, CategoriaSIGAP;

            if (tbl_Sol_Finca.Area_SIGAP == true)
            {
                CategoriaSIGAP = $"Sí";
            }
            else
            {
                CategoriaSIGAP = $"No";
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


            string DescripcionPlantacionCONAP;
            if (tbl_Sol_Finca.Reforestacion_CONAP == true)
            {
                DescripcionPlantacionCONAP = $"¿La plantación es producto de una reforestación con CONAP ? Sí";
            }
            else
            {
                DescripcionPlantacionCONAP = $"¿La plantación es producto de una reforestación con CONAP ? No";
            }

            c1 = new PdfPCell(new Phrase(DescripcionPlantacionCONAP, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"¿La plantación se encuentra dentro de área SIGAP?   {CategoriaSIGAP}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Nombre de la Categoría SIGAP", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{DescripcionCategoriaSIGAP}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 5;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Objetivo de la plantación", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{DescripcionPlantacion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 5;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }


        private void PreFirma(Tbl_Sol_Solicitud tbl_sol_Solicitud)
        {

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 12);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($"Basado en el anális del expediente No. ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 5;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_sol_Solicitud.Solicitud_NumeroExpediente}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 7;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" y  su evaluación en campo se recomienda  ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 5;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"NO APROBAR LA INSCRIPCIÓN", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 7;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            ///////////////////////////////////////////



            c1 = new PdfPCell(new Phrase($"del área como " + $"{tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion}" + " con los datos que se consignan en el presente informe. ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

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
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
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


            //c1 = new PdfPCell(logo);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Colspan = 4;
            c1.Rowspan = 3;
            c1.Border = 0;


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


            string strNombreDelTecnico = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", tbl_sol_Solicitud.TecnicoAsignado_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{strNombreDelTecnico}", fntTituloTabla));
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

            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        private void LlenaDatosDeLaEmpresa(Tbl_Sol_Solicitud tbl_Sol_Solicitud, fc_Sol_Sel_Direccion_Result DireccionSolicitud)
        {
            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            string Direccion = "";
            string DireccionMovil = "";

            if (DireccionSolicitud.Direccion.Trim() != "")
            {
                Direccion += DireccionSolicitud.Direccion.Trim() + ", ";
            }
            if (DireccionSolicitud.Aldea.Trim() != "")
            {
                Direccion += "Aldea " + DireccionSolicitud.Aldea.Trim() + ", ";
            }
            Direccion += DireccionSolicitud.Municipio + ", " + DireccionSolicitud.Departamento;


            if (!string.IsNullOrWhiteSpace(tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil))
            {
                DireccionMovil = tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";

            }

            //    if (tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() != "")
            //{
            //    DireccionMovil = tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";
            //}
            DireccionMovil += tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio1.Municipio + ", " + tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento1.Departamento;


            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            if (tbl_Sol_Empresa_Entidad != null)
            {
                Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Find(tbl_Sol_Solicitud.Solicitud_id);

                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Nombre Comercial: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Nombre}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                if (tbl_Sol_Empresa_Entidad.Objeto_Empresa != null && tbl_Sol_Empresa_Entidad.Objeto_Empresa.Trim() != "")
                {

                    c1 = new PdfPCell(new Phrase("Objeto de la Empresa: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Objeto_Empresa}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                c1 = new PdfPCell(new Phrase("Número de NIT: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.No_NIT}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Dirección de la Empresa Forestal: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Direccion, fntTituloTabla));
                //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Aldea}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio.Municipio}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tipo_Industria_id == 2) || (tbl_Sol_Solicitud.SolicitudTipo_id >= 5 && tbl_Sol_Solicitud.SolicitudTipo_id <= 6 && tbl_Sol_Solicitud.Sub_Categoria_id == 3))
                {
                    c1 = new PdfPCell(new Phrase("Dirección de trabajo de la industria Forestal Movil: ", fntTituloTabla));
                    c1 = new PdfPCell(new Phrase("Dirección de funcionamiento: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(DireccionMovil, fntTituloTabla));
                    //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Aldea}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio.Municipio}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);
                }


                if (tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria != null)
                {

                    c1 = new PdfPCell(new Phrase("Tipo de Industria: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                if (tbl_Sol_Empresa_Entidad_Tipo_Registro != null)
                {

                    string repeju = "";
                    string nopartida = "";
                    string nofolio = "";
                    string nolibro = "";
                    string noacta = "";
                    string fechaacta = "";

                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.REPEJU_De_Id != 0)
                    {
                        repeju = tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_REPEJU_De.Descripcion;
                    }
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Partida != null)
                    {
                        nopartida = tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Partida;
                    }
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Folio != null)
                    {
                        nofolio = tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Folio;
                    }
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Libro != null)
                    {
                        nolibro = tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Libro;
                    }
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Acta != null)
                    {
                        noacta = tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Acta;
                    }
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta != null)
                    {
                        DateTime Fecha_Acta = (DateTime)tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta;
                        fechaacta = Fecha_Acta.ToString("dd/MM/yyyy");
                    }
                    c1 = new PdfPCell(new Phrase("Tipo de Registro: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Número de Registro: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(nopartida, fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Número de Folio: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(nofolio, fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Número de Libro: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(nolibro, fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);


                    if (repeju != "")
                    {
                        c1 = new PdfPCell(new Phrase("Tipo de REPEJU: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(repeju, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                        c1.Colspan = 2;
                        c1.Border = 0;
                        tablePersoneria.AddCell(c1);

                    }

                    if ((noacta.Trim() != "") || (fechaacta.Trim() != ""))
                    {
                        c1 = new PdfPCell(new Phrase("Número de Acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(noacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Fecha de Acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }


                    c1 = new PdfPCell(new Phrase("Empresa Forestal a Registrar: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);
                }


            }

        }


        public string GenerarPVJuridico_PDF(long Solicitud_id, string Guidetapa, string AnalisisExpediente, Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud)
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

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == Solicitud_id).FirstOrDefault();

            if (tbl_Sol_PropietarioPersonaJuridica != null)
            {
                strNombre = Guidetapa + ".pdf";
                strNombrePersona = $"{tbl_Sol_PropietarioPersonaJuridica.Nombre.ToUpper()}";
            }
            else
            {
                strNombre = Guidetapa + ".pdf";
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

            LlenaTituloRevision(tbl_sol_Solicitud);
            doc.Add(tableTitulo);
            doc.Add(Enter);

            // tbl_Gest_EtapaSolicitud

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            string varNo_Informe = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id).Identificador;

            int intVarEntidad = 0;

            if (tbl_Sol_Empresa_Entidad != null)
            {
                intVarEntidad = 1;
            }

            LlenaDatosGenerales(tbl_sol_Solicitud, tbl_Sol_Finca.Count(), intVarEntidad, tbl_Gest_EtapaSolicitud, objUs.intUsuario_id);
            doc.Add(tableDatosGenerales);
            doc.Add(Enter);


            fc_Sol_Sel_Direccion_Result DireccionSolicitud = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

            if ((tbl_sol_Solicitud.Categoria_id == 5) || ((tbl_sol_Solicitud.Categoria_id == 8) && (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE LA EMPRESA");
                doc.Add(tableBanner);
                intTexto_Romano = intTexto_Romano + 1;

                LlenaDatosDeLaEmpresa(tbl_sol_Solicitud, DireccionSolicitud);
                doc.Add(tablePersoneria);
                doc.Add(Enter);

                if (tbl_Sol_Empresa_Entidad.CapacidadInstalada != null)
                {
                    var CapacidadInstalada = new Paragraph("                  Capacidad instalada para producción mensual: " + tbl_Sol_Empresa_Entidad.CapacidadInstalada.ToString() + " metros cubicos.");
                    doc.Add(CapacidadInstalada);
                    doc.Add(Enter);
                }

            }



            if (tbl_Sol_Finca.Count() > 0)
            {
                if (tbl_Sol_Finca.Count() > 1)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]} DATOS DE LA FINCAS");
                }
                if (tbl_Sol_Finca.Count() == 1)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]} DATOS DE LA FINCA");
                }

                doc.Add(tableBanner);

                for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                {

                    LlenaDatosFinca(tbl_Sol_Finca[i], i + 1);

                    doc.Add(tableDatosFinca);
                }
                doc.Add(Enter);

                intTexto_Romano = intTexto_Romano + 1;

                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. ÁNALISIS DEL EXPEDIENTE", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner(AnalisisExpediente, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                intTexto_Romano = intTexto_Romano + 1;
            }

            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DICTAMEN");
            doc.Add(tableBanner);


            doc.Add(Enter);

            PreFirma(tbl_sol_Solicitud);
            doc.Add(tableFirmaSolicitante);


            FirmaSolicitante(tbl_sol_Solicitud);
            doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();

            return strNombre;

        }

        [HttpPost]
        public JsonResult GenerarPlantacionVoluntariaJuridico(string Guidid, string Guidetapa, string AnalisisExpediente)
        {

            string TextoMostrar, Ubicacion;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guidid).First();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa).First();


            Ubicacion = GenerarPVJuridico_PDF(tbl_sol_solicitud.Solicitud_id, Guidetapa, AnalisisExpediente, tbl_Gest_EtapaSolicitud);
            TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";


            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = Guidetapa + ".pdf";


            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


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
        //                        @"      ""Coordenadas"": ""70,650,180,710"",
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

            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioTecnicoReporteDenegacionController-JsonProcesarFirmaElectronica");

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
                    intRespuesta = 0;

                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 4, "D.- Bearer erroneo, menor a 125 caracteres");

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



                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 8, "X.- El archivo se obtuvo con exito.");

                }
                else
                {
                    Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 9, "X.- No se logró obtener el archivo firmado.");

                }



                Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 10, "Z.- FE generada con éxito.");

                //intRespuesta = 1;

                //Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                //jsonResultUsr = "{\"CodRespuesta\":"
                //                  + "\"" + intRespuesta + "\","
                //                  + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";




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

    }
}

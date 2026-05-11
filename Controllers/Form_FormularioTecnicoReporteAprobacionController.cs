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
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net;

namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoReporteAprobacionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();
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

        private void LlenaDatosGenerales(Tbl_Sol_Solicitud tbl_sol_Solicitud, int CantidadFincas, int CantidadEmpresas, Result_SP_IdentificadorOficialGestion resultsp)
        {

            string Const_No_Informe = resultsp.Identificador;

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxtSinGuatemala(getdate())").FirstOrDefault();

            string strNombreDelDirectorRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Gral_NombreDirectorRegional(@p0,@p1)", tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).FirstOrDefault();
            string strNombreDelDirectorSubRegional = db.Database.SqlQuery<string>("Select dbo.Fnc_Gral_NombreSubDirectorRegional(@p0,@p1)", tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).FirstOrDefault();

            tableDatosGenerales = new PdfPTable(12);

            PdfPCell c1 = new PdfPCell();

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
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

            decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);

            string varTitulo;





            if (CantidadFincas > 1)
            {
                if (TipoGestion == (decimal)0.00)
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguientes fincas.", fntTitulo3));
                }

                if ((TipoGestion == (decimal)0.01) || (TipoGestion == (decimal)0.02) || (TipoGestion == (decimal)0.03))
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + ", número de registro: " + tbl_sol_Solicitud.No_Registro + "; con solicitud de actualización en El Registro Nacional Forestal de las siguientes fincas.", fntTitulo3));
                }

                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                tableDatosGenerales.AddCell(c1);
            }

            if (CantidadFincas == 1)
            {

                if (TipoGestion == (decimal)0.00)
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguiente finca.", fntTitulo3));
                }

                if ((TipoGestion == (decimal)0.01) || (TipoGestion == (decimal)0.02))
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + ", número de registro " + tbl_sol_Solicitud.No_Registro + "; con solicitud de actualización en El Registro Nacional Forestal de las siguiente finca.", fntTitulo3));
                }


                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                tableDatosGenerales.AddCell(c1);
            }

            if ((CantidadEmpresas == 1) && (tbl_sol_Solicitud.Categoria_id == 5))
            {

                if (TipoGestion == (decimal)0.00)
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con solicitud de inscripcion en El Registro Nacional Forestal de las siguiente empresa.", fntTitulo3));
                }

                if ((TipoGestion == (decimal)0.01) || (TipoGestion == (decimal)0.02))
                {
                    c1 = new PdfPCell(new Phrase("Por  este  medio  informo  sobre  el  análisis  e  inspección  de  campo  realizado  al  expediente  No. " + tbl_sol_Solicitud.Solicitud_NumeroExpediente + ", número de registro  " + tbl_sol_Solicitud.No_Registro + "; con solicitud de actualización en El Registro Nacional Forestal de las siguiente empresa.", fntTitulo3));
                }



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

            //c1 = new PdfPCell(new Phrase($"4. La plantación se encuentra en área protegida-SIGAP ? ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 4;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            //tableDatosFinca.AddCell(c1);

            //if (tbl_Sol_Finca.Area_SIGAP == true)
            //{
            //    c1 = new PdfPCell(new Phrase($"Si", fntTituloTabla));
            //}
            //else
            //{
            //    c1 = new PdfPCell(new Phrase($"No", fntTituloTabla));
            //}

            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //c1.Colspan = 2;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_BOTTOM;


            //tableDatosFinca.AddCell(c1);

            //c1 = new PdfPCell(new Phrase($"Nombre del área", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 2;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            //tableDatosFinca.AddCell(c1);


            //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreAreaSIGAP}", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //c1.Colspan = 4;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            //tableDatosFinca.AddCell(c1);


            ///////////////////////////////////////


        }

        private void LlenaDatosFincaResultado_Old(Tbl_Sol_Finca tbl_Sol_Finca, int FincaId)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(12);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 7, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"FINCA " + FincaId.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 12;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            c1.Rowspan = 2;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.Rowspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            string Modalidad = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Categoria_id).First().Modalidad;
            string subModalidad = "";
            try
            {
                subModalidad += db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Sub_Categoria_id).First().Modalidad;
            }
            catch (Exception ex)
            {
                subModalidad = "";
            }

            Constants Const = new Constants();

            if (Modalidad != "")
            {
                if (subModalidad != "")
                {
                    Modalidad = Const.initCapTexto(Modalidad);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_Categoria + " ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Modalidad}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);
                }
                else
                {
                    Modalidad = Const.initCapTexto(Modalidad);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_Categoria + " ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Modalidad}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    c1.Rowspan = 2;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);


                }

            }

            if (subModalidad != "")
            {

                c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_SubCategoria + " ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{subModalidad}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);

            }


            ///////////////////////////////////////

            c1 = new PdfPCell(new Phrase($"1. El área a total a registrar es de ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            decimal areafincatotal = db_API.Database.SqlQuery<decimal>("SELECT dbo.fn_sumar_area_rodal_local('" + tbl_Sol_Finca.Solicitud_id + "','" + tbl_Sol_Finca.Finca_Id + "')").FirstOrDefault();
            //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1 = new PdfPCell(new Phrase(areafincatotal.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"ha", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            if (Modalidad != "")
            {

                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 6;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosFinca.AddCell(c1);
            }

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
            c1.Colspan = 12;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Denominada ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            c1.Rowspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;

            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.Rowspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            string Modalidad = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Categoria_id).First().Modalidad;
            string subModalidad = "";
            try
            {
                subModalidad += db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Sub_Sub_Categoria_id).First().Modalidad;
            }
            catch (Exception ex)
            {
                subModalidad = "";
            }

            Constants Const = new Constants();

            if (Modalidad != "")
            {
                if (subModalidad != "")
                {
                    Modalidad = Const.initCapTexto(Modalidad);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_Categoria + " ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Modalidad}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);
                }
                else
                {
                    Modalidad = Const.initCapTexto(Modalidad);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_Categoria + " ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Modalidad}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;

                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                    tableDatosFinca.AddCell(c1);


                }

            }

            if (subModalidad != "")
            {

                c1 = new PdfPCell(new Phrase(tbl_Sol_Finca.Tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Modalidad_SubCategoria + " ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{subModalidad}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);



            }
            else
            {

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;

                tableDatosFinca.AddCell(c1);

            }


            ///////////////////////////////////////

            c1 = new PdfPCell(new Phrase($"El área total a registrar es de ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            decimal areafincatotal = db_API.Database.SqlQuery<decimal>("SELECT dbo.fn_sumar_area_rodal_local('" + tbl_Sol_Finca.Solicitud_id + "','" + tbl_Sol_Finca.Finca_Id + "')").FirstOrDefault();
            //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1 = new PdfPCell(new Phrase(areafincatotal.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"ha", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"La longitud a registrar ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            decimal longitudfincatotal = db_API.Database.SqlQuery<decimal>("SELECT dbo.fn_sumar_longitud_rodal_local('" + tbl_Sol_Finca.Solicitud_id + "','" + tbl_Sol_Finca.Finca_Id + "')").FirstOrDefault();
            //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1 = new PdfPCell(new Phrase(longitudfincatotal.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"m", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);


            //if (Modalidad != "")
            //{

            //    c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            //    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //    c1.Colspan = 6;
            //    c1.Border = 0;
            //    c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            //    tableDatosFinca.AddCell(c1);
            //}

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


            decimal areafincatotal = db_API.Database.SqlQuery<decimal>("SELECT dbo.fn_sumar_area_rodal_local('" + tbl_Sol_Finca.Solicitud_id + "','0')").FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{areafincatotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

        }

        private void LlenaDatosFinca_API(Tbl_API_Sol_Finca tbl_Sol_Finca, int FincaId)
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

        private void LlenaDatosFincaDireccion_API(Tbl_API_Sol_Finca tbl_Sol_Finca, int FincaId)
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

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Departamento_Registro}", fntTituloTabla));
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

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Municipio}", fntTituloTabla));
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

        public class Personerias
        {
            public List<Tbl_Sol_PropietarioPersonaIndividual> PropietariosIndividuales { get; set; }
            public List<Tbl_Sol_PropietarioPersonaJuridica> PropietariosJuridicos { get; set; }
            public List<fc_Sol_Rodal_RepresentanteMandatario_Result> RepresentatnteLegal { get; set; }
            public List<fc_Sol_Rodal_RepresentanteMandatario_Result> Mandatario { get; set; }
            public List<Tbl_Sol_ArrendatarioPersonaIndividual> ArrendatariosIndividuales { get; set; }
            public List<Tbl_Sol_ArrendatarioPersonaJuridica> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias(long Solicitud_id)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<Tbl_Sol_PropietarioPersonaIndividual>();
            personerias.PropietariosJuridicos = new List<Tbl_Sol_PropietarioPersonaJuridica>();
            personerias.RepresentatnteLegal = new List<fc_Sol_Rodal_RepresentanteMandatario_Result>();
            personerias.Mandatario = new List<fc_Sol_Rodal_RepresentanteMandatario_Result>();
            personerias.ArrendatariosIndividuales = new List<Tbl_Sol_ArrendatarioPersonaIndividual>();
            personerias.ArrendatariosJuridicos = new List<Tbl_Sol_ArrendatarioPersonaJuridica>();

            List<Tbl_Sol_ArrendatarioPersonaJuridica> tbl_Sol_ArrendatarioPersonaJuridica = (from d in db.Tbl_Sol_ArrendatarioPersonaJuridica
                                                                                             where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                             select d).ToList();

            List<Tbl_Sol_ArrendatarioPersonaIndividual> tbl_Sol_ArrendatarioPersonaIndividual = (from d in db.Tbl_Sol_ArrendatarioPersonaIndividual
                                                                                                 where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                 select d).ToList();


            List<Tbl_Sol_PropietarioPersonaJuridica> tbl_Sol_PropietarioPersonaJuridicas = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                            where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                            select d).ToList();

            List<Tbl_Sol_PropietarioPersonaIndividual> tbl_Sol_PropietarioPersonaIndividuals = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                                where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();


            if (tbl_Sol_PropietarioPersonaIndividuals.Count() > 0)
            {
                personerias.PropietariosIndividuales = tbl_Sol_PropietarioPersonaIndividuals;
            }

            if (tbl_Sol_PropietarioPersonaJuridicas.Count() > 0)
            {
                personerias.PropietariosJuridicos = tbl_Sol_PropietarioPersonaJuridicas;
            }

            if (oRepresentanteLegal.Count() > 0)
            {
                personerias.RepresentatnteLegal = oRepresentanteLegal;
            }

            if (oMandatario.Count() > 0)
            {
                personerias.Mandatario = oMandatario;
            }

            if (tbl_Sol_ArrendatarioPersonaIndividual.Count() > 0)
            {
                personerias.ArrendatariosIndividuales = tbl_Sol_ArrendatarioPersonaIndividual;
            }

            if (tbl_Sol_ArrendatarioPersonaJuridica.Count() > 0)
            {
                personerias.ArrendatariosJuridicos = tbl_Sol_ArrendatarioPersonaJuridica;
            }

            return personerias;
        }

        private void LlenaDatosSolicitante(long Solicitud_id, Personerias personerias)
        {


            DateTime fecha;
            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (personerias.PropietariosIndividuales.Count() > 0)
            {
                for (int i = 0; i < personerias.PropietariosIndividuales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].Nombres + " " + personerias.PropietariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (personerias.PropietariosJuridicos.Count() > 0)
            {
                for (int i = 0; i < personerias.PropietariosJuridicos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosJuridicos[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosJuridicos[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                }
            }

            if (personerias.RepresentatnteLegal.Count() > 0)
            {
                for (int i = 0; i < personerias.RepresentatnteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Datos del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{personerias.RepresentatnteLegal[i].Nombres} {personerias.RepresentatnteLegal[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.RepresentatnteLegal[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.RepresentatnteLegal[i].RepresentanteNo_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        fecha = (DateTime)personerias.RepresentatnteLegal[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    }
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vence", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        if (personerias.RepresentatnteLegal[i].VigenciaIndefinida == true)
                        {
                            c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)personerias.RepresentatnteLegal[i].Fecha_FinNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }
                }
            }

            if (personerias.Mandatario.Count() > 0)
            {

                for (int i = 0; i < personerias.Mandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{personerias.Mandatario[i].Nombres} {personerias.Mandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase(personerias.Mandatario[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{personerias.Mandatario[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia del mandato", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)personerias.Mandatario[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vence", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (personerias.Mandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)personerias.Mandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                }
            }

            if (personerias.ArrendatariosIndividuales.Count() > 0)
            {
                for (int i = 0; i < personerias.ArrendatariosIndividuales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].Nombres + " " + personerias.ArrendatariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (personerias.ArrendatariosJuridicos.Count() > 0)
            {
                for (int i = 0; i < personerias.ArrendatariosJuridicos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosJuridicos[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosJuridicos[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);
                }
            }

            tableBanner.AddCell(c1);

            return;
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

        private void DatosPoligonos_API(Tbl_API_Sol_Finca tbl_API_Sol_Finca)
        {
            tableEstimacion = new PdfPTable(6);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            List<Tbl_API_Sol_Rodal_Poligono_Local> tbl_API_Sol_Rodal_Poligono_Locals = new List<Tbl_API_Sol_Rodal_Poligono_Local>();
            tbl_API_Sol_Rodal_Poligono_Locals = (from d in db_API.Tbl_API_Sol_Rodal_Poligono_Local
                                                 where d.Solicitud_id == tbl_API_Sol_Finca.Solicitud_id && d.Finca_id == tbl_API_Sol_Finca.Finca_Id
                                                 orderby d.Finca_id, d.Rodal_Id, d.Tipo_de_Area descending, d.RegistroLocal_id
                                                 select d).ToList();

            if (tbl_API_Sol_Rodal_Poligono_Locals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Finca", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Tipo de Area", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Correlativo", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMX", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;

                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMY", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;

                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_API_Sol_Rodal_Poligono_Locals.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Poligono_Locals[i].Finca_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Poligono_Locals[i].Tipo_de_Area, fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Poligono_Locals[i].Rodal_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Poligono_Locals[i].RegistroLocal_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_API_Sol_Rodal_Poligono_Locals[i].GTMX ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_API_Sol_Rodal_Poligono_Locals[i].GTMY ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);
                }

                tableEstimacion.AddCell(c1);
            }

            tableBanner.AddCell(c1);
            return;
        }

        private void DatosPoligonoDescuento_API(Tbl_API_Sol_Finca tbl_API_Sol_Finca)
        {
            tableEstimacion = new PdfPTable(7);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            List<Tbl_API_Sol_Rodal_Descuento_Poligono_Local> tbl_API_Sol_Rodal_Descuento_Poligono_Locals = new List<Tbl_API_Sol_Rodal_Descuento_Poligono_Local>();
            tbl_API_Sol_Rodal_Descuento_Poligono_Locals = (from d in db_API.Tbl_API_Sol_Rodal_Descuento_Poligono_Local
                                                           where d.Solicitud_id == tbl_API_Sol_Finca.Solicitud_id && d.Finca_id == tbl_API_Sol_Finca.Finca_Id
                                                           orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Id, d.Rodal_Descuento_Id, d.RegistroLocal_id
                                                           select d).ToList();

            if (tbl_API_Sol_Rodal_Descuento_Poligono_Locals.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Finca", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Tipo de Area", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area de Descuento", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Correlativo", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMX", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMY", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_API_Sol_Rodal_Descuento_Poligono_Locals.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].Finca_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Area de Descuento", fntSubTotal));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].Rodal_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].Rodal_Descuento_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].RegistroLocal_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].GTMX ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_API_Sol_Rodal_Descuento_Poligono_Locals[i].GTMY ?? 0).ToString("0"), fntTituloTabla));
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);
                }

                tableEstimacion.AddCell(c1);
            }

            tableBanner.AddCell(c1);
            return;

        }

        private void DatosPoligonos(Tbl_Sol_Finca tbl_Sol_Finca)
        {
            tableEstimacion = new PdfPTable(6);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();

            List<Tbl_Sol_Rodal_Poligono> tbl_Sol_Rodal_Poligonos = new List<Tbl_Sol_Rodal_Poligono>();
            tbl_Sol_Rodal_Poligonos = (from d in db.Tbl_Sol_Rodal_Poligono
                                       where d.Solicitud_id == tbl_Sol_Finca.Solicitud_id && d.Finca_id == tbl_Sol_Finca.Finca_Id
                                       orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Id, d.Carga_id, d.Correlativo_id
                                       select d).ToList();

            if (tbl_Sol_Rodal_Poligonos.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Finca", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Tipo de Area", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Correlativo", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMX", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;

                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMY", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;

                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_Sol_Rodal_Poligonos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Poligonos[i].Finca_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Poligonos[i].Tbl_Sol_Rodal.Tbl_Sol_Rodal_Tipo.Descripcion, fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Poligonos[i].Rodal_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Poligonos[i].Correlativo_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Poligonos[i].GTMX ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Poligonos[i].GTMY ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);
                }

                tableEstimacion.AddCell(c1);
            }

            tableBanner.AddCell(c1);
            return;
        }

        private void DatosPoligonoDescuento(Tbl_Sol_Finca tbl_Sol_Finca)
        {
            tableEstimacion = new PdfPTable(7);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
            PdfPCell c1 = new PdfPCell();


            List<Tbl_Sol_Rodal_Descuento_Poligono> tbl_Sol_Rodal_Descuento_Poligonos = new List<Tbl_Sol_Rodal_Descuento_Poligono>();
            tbl_Sol_Rodal_Descuento_Poligonos = (from d in db.Tbl_Sol_Rodal_Descuento_Poligono
                                                 where d.Solicitud_id == tbl_Sol_Finca.Solicitud_id && d.Finca_id == tbl_Sol_Finca.Finca_Id
                                                 orderby d.Finca_id, d.Tipo_de_Area, d.Rodal_Id, d.Rodal_Descuento_Id, d.Carga_id, d.Correlativo_id
                                                 select d).ToList();

            if (tbl_Sol_Rodal_Descuento_Poligonos.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Finca", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Tipo de Area", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area de Descuento", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Correlativo", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMX", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("GTMY", fntSubTotal));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Border = 15;
                c1.Colspan = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_Sol_Rodal_Descuento_Poligonos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Descuento_Poligonos[i].Finca_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Area de Descuento", fntSubTotal));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Descuento_Poligonos[i].Rodal_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Descuento_Poligonos[i].Rodal_Descuento_Id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Descuento_Poligonos[i].Correlativo_id.ToString(), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Descuento_Poligonos[i].GTMX ?? 0).ToString("0"), fntTituloTabla));
                    c1.Border = 15;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Descuento_Poligonos[i].GTMY ?? 0).ToString("0"), fntTituloTabla));
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;
                    c1.Border = 15;
                    c1.Colspan = 1;
                    tableEstimacion.AddCell(c1);
                }

                tableEstimacion.AddCell(c1);
            }

            tableBanner.AddCell(c1);
            return;

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
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Finca.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Finca.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


                            //c1 = new PdfPCell(new Phrase($"Estimación de volumen por {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"Especie a registrar", fntTituloTabla));
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

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Finca.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Finca.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);


                            // c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"Especies a registrar", fntTituloTabla));
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
                            c1 = new PdfPCell(new Phrase($"RODAL", fntTituloTabla));
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

        private void EstimacionVolumen_API(Tbl_API_Sol_Finca tbl_Sol_Finca)
        {

            //List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
            //                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
            //                                                                               select d).ToList();

            List<fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db_API.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Longitud_Total, d.EstimacionPorMedioDe, d.Especie, d.LongitudMinima
                                                                                               //orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
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
                            Tbl_Sol_Rodal tbl_Sol_Rodal1 = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";
                            string area = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? (tbl_Sol_Rodal1.GTMX ?? 0)).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? (tbl_Sol_Rodal1.GTMY ?? 0)).ToString("0");
                                area = (validacionesDiametrica[i].Area_Efectiva_Rodal ?? (tbl_Sol_Rodal1.AreaTotal ?? 0)).ToString("0.00");
                            }
                            else
                            {
                                gtmx = (tbl_Sol_Rodal1.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal1.GTMY ?? 0).ToString("0");
                                area = (tbl_Sol_Rodal1.AreaTotal ?? 0).ToString("0.00");
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
                            if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                            {

                                c1 = new PdfPCell(new Phrase($"Arboles por ha", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Arboles por Rodal", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                            else
                            {
                            c1 = new PdfPCell(new Phrase($"Densidad por ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            }
                            if (validacionesDiametrica[i].EstimacionPorMedioDe != "Censo")
                            {
                                c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }

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

                            c1 = new PdfPCell(new Phrase(area, fntTituloTabla));
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
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {

                            c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Cantidad_Arboles).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                        }
                        else
                        {

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Densidad_ha).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }

                        if (validacionesDiametrica[i].EstimacionPorMedioDe != "Censo")
                        {
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                        }

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {
                            c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        { 
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_ha).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);
                        }

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_Rodal).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {

                            SubTotal_Densidadha += (decimal)validacionesDiametrica[i].Cantidad_Arboles;
                        }
                        else
                        {

                            SubTotal_Densidadha += (decimal)validacionesDiametrica[i].Densidad_ha;
                        }


                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                tableEstimacion.AddCell(c1);
                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);

                                    c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                if (validacionesDiametrica[i].EstimacionPorMedioDe != "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    tableEstimacion.AddCell(c1);
                                }

                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                { 
                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                                }

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
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
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

                                c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.Border = 0;
                                    tableEstimacion.AddCell(c1);

                                    c1 = new PdfPCell(new Phrase("—", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.Border = 0;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);

                                    c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {

                                    c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.Border = 0;
                                    tableEstimacion.AddCell(c1);

                                    c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);

                                }


                                if (validacionesDiametrica[i].EstimacionPorMedioDe != "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase($" ", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.Border = 0;
                                    tableEstimacion.AddCell(c1);
                                }
                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                { 

                                c1 = new PdfPCell(new Phrase(volumenha.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                                }

                                c1 = new PdfPCell(new Phrase(volumenrodal.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
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
                            Tbl_Sol_Rodal tbl_Sol_Rodal1 = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";
                            string longitud = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? (tbl_Sol_Rodal1.GTMX ?? 0)).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? (tbl_Sol_Rodal1.GTMY ?? 0)).ToString("0");
                                longitud = (validacionesDiametrica[i].Longitud_Total ?? (tbl_Sol_Rodal1.Longitud_Total ?? 0)).ToString("0.00");
                            }
                            else
                            {
                                gtmx = (tbl_Sol_Rodal1.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal1.GTMY ?? 0).ToString("0");
                                longitud = (tbl_Sol_Rodal1.Longitud_Total ?? 0).ToString("0.00");
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

                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Cantidad de Árboles", fntTituloTabla));
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

                            c1 = new PdfPCell(new Phrase(longitud, fntTituloTabla));
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
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Cantidad_Arboles ?? 0).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);
                        

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_X_Linea).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
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
                                

                                c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                                                       

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                

                                c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

                           
                            c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                                                       

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                            {

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("—", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                            }
                            else
                            {
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                            }

                            //if (validacionesDiametrica[i].EstimacionPorMedioDe != "Censo")
                            //{
                            //    c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            //    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            //    c1.Colspan = 2;
                            //    c1.Border = 0;
                            //    tableEstimacion.AddCell(c1);
                            //}

                            c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
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

        private void EstimacionVolumen_API_FS(Tbl_API_Sol_Finca tbl_Sol_Finca)
        {

            //List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
            //                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
            //                                                                               select d).ToList();

            List<fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db_API.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                               orderby d.Rodal_id, d.Tipo_de_Area, d.Longitud_Total, d.EstimacionPorMedioDe, d.Especie, d.LongitudMinima
                                                                                               //orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.Clase
                                                                                               select d).ToList();


            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();



            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(9);
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
                            Tbl_Sol_Rodal tbl_Sol_Rodal1 = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";
                            string area = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? (tbl_Sol_Rodal1.GTMX ?? 0)).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? (tbl_Sol_Rodal1.GTMY ?? 0)).ToString("0");
                                area = (validacionesDiametrica[i].Area_Efectiva_Rodal ?? (tbl_Sol_Rodal1.AreaTotal ?? 0)).ToString("0.00");
                            }
                            else
                            {
                                gtmx = (tbl_Sol_Rodal1.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal1.GTMY ?? 0).ToString("0");
                                area = (tbl_Sol_Rodal1.AreaTotal ?? 0).ToString("0.00");
                            }

                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 9;
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
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
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
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(gtmy, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
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


                            c1 = new PdfPCell(new Phrase($"DATOS DE LA FUENTE SEMILLERA", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
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
                            c1 = new PdfPCell(new Phrase("Clase", fntTituloTabla));
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

                            c1 = new PdfPCell(new Phrase(area, fntTituloTabla));
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


                        c1 = new PdfPCell(new Phrase(validacionesDiametrica[i].Clase.ToString(), fntTituloTabla));
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


                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = BaseColor.WHITE;
                                c1.Colspan = 2;
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


                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 2;
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


                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 9;
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
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                            c1.BackgroundColor = BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 9;
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
                            Tbl_Sol_Rodal tbl_Sol_Rodal1 = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).FirstOrDefault();
                            string gtmx, gtmy;
                            gtmx = gtmy = "0";
                            string area = "0";

                            if (tbl_Sol_Rodal != null)
                            {
                                gtmx = (tbl_Sol_Rodal.GTMX ?? (tbl_Sol_Rodal1.GTMX ?? 0)).ToString("0");
                                gtmy = (tbl_Sol_Rodal.GTMY ?? (tbl_Sol_Rodal1.GTMY ?? 0)).ToString("0");
                                area = (validacionesDiametrica[i].Area_Efectiva_Rodal ?? (tbl_Sol_Rodal1.AreaTotal ?? 0)).ToString("0.00");
                            }
                            else
                            {
                                gtmx = (tbl_Sol_Rodal1.GTMX ?? 0).ToString("0");
                                gtmy = (tbl_Sol_Rodal1.GTMY ?? 0).ToString("0");
                                area = (tbl_Sol_Rodal1.AreaTotal ?? 0).ToString("0.00");
                            }



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
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
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
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(gtmy, fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
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



                            c1 = new PdfPCell(new Phrase($"DATOS DE LA FUENTE SEMILLERA", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
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
                            c1 = new PdfPCell(new Phrase($"Clase", fntTituloTabla));
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

                        c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Clase).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
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
                                c1.Colspan = 4;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
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
                            c1.Colspan = 4;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
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
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 9;
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
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 9;
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

            tableEstimacion = new PdfPTable(10);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From db_RNF_API.dbo.fc_API_Sol_Sel_Rodal_ValidacionesDiametrica_PV(" + solicitud_id + ") Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos de la fuente semillera", fntSubTitulo));
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

        private void LlenaDatosCultivoEnAsocio(List<Tbl_Sol_Rodal_CultivoTecnico> tbl_sol_Rodal_Cultivo)
        {
            tableDatosPlantacion = new PdfPTable(7);
            var Enter = new Paragraph(" ");
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Cultivos en asocio", fntTituloTabla));
            c1.Colspan = 5;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"No. rodal", fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Nombre común", fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Año de plantación", fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Volúmen / ha", fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Volúmen / rodal", fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosPlantacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);
            decimal volumenha = 0;
            decimal volumenrodal = 0;

            foreach (Tbl_Sol_Rodal_CultivoTecnico item in tbl_sol_Rodal_Cultivo)
            {
                volumenha = (item.Volumen_ha ?? 0);
                volumenrodal = (item.Volumen_Rodal ?? 0);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Rodal_Id}", fntTituloTabla));
                c1.Colspan = 1;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Cultivo.Nombres_Comunes}", fntTituloTabla));
                c1.Colspan = 1;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{item.Anio_Establecimiento}", fntTituloTabla));
                c1.Colspan = 1;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{volumenha.ToString("0.00")}", fntTituloTabla));
                c1.Colspan = 1;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{volumenrodal.ToString("0.00")}", fntTituloTabla));
                c1.Colspan = 1;
                tableDatosPlantacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);



            }
            return;
        }

        private void LlenaDatosPlantacion_20230901(Tbl_Sol_Finca tbl_Sol_Finca)
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

            if (CategoriaSIGAP != "No")
            {
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
            }
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

        private void LlenaDatosPlantacion(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            Tbl_Sol_FincaObjetivoDeLaPlantacion objetivoPlantacion = (from d in db.Tbl_Sol_FincaObjetivoDeLaPlantacion
                                                                      where d.ObjetivoDeLaPlantacion == tbl_Sol_Finca.ObjetivoDeLaPlantacion
                                                                      select d).FirstOrDefault();

            fc_API_GetResumenFinca_DictamenTecnico_Result ResumenFinca_DictamenTecnico = db_API.fc_API_GetResumenFinca_DictamenTecnico(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosPlantacion = new PdfPTable(11);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            string DescripcionPlantacion, DescripcionCategoriaSIGAP, CategoriaSIGAP;

            Tbl_API_Sol_Finca tbl_API_Sol_Finca = (from d in db_API.Tbl_API_Sol_Finca
                                                   where d.Solicitud_id == tbl_Sol_Finca.Solicitud_id
                                                   && d.Finca_Id == tbl_Sol_Finca.Finca_Id
                                                   select d).FirstOrDefault();

            bool Area_SIGAP = false;

            if ((ResumenFinca_DictamenTecnico != null) && (ResumenFinca_DictamenTecnico.Area_SIGAP == true))
            {
                Area_SIGAP = true;
                CategoriaSIGAP = $"Sí";
            }
            else
            {
                CategoriaSIGAP = $"No";
            }

            if ((Area_SIGAP) && ((ResumenFinca_DictamenTecnico.CategoriaSIGAP ?? "") != ""))
            {
                DescripcionCategoriaSIGAP = $"{ResumenFinca_DictamenTecnico.CategoriaSIGAP}";
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

            if ((Area_SIGAP) && (ResumenFinca_DictamenTecnico.CategoriaSIGAP_AreaLongitud > 0))
            {
                c1 = new PdfPCell(new Phrase($"Area/Longitud dentro de Area SIGAP:   {ResumenFinca_DictamenTecnico.CategoriaSIGAP_AreaLongitud}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 11;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);


            }

            if (DescripcionCategoriaSIGAP != "")
            {
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
            }

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

        private void AgregarArgumentacion(string Argumentacion)
        {
            tableFirmaSolicitante = new PdfPTable(numColumns: 12);
            PdfPCell c1 = new PdfPCell();


            var Enter = new Paragraph(" ");
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            if (Argumentacion.ToString().Trim() == "")
            {
                c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 12;
                c1.Rowspan = 1;

                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableFirmaSolicitante.AddCell(c1);

                return;
            }


            c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            c1.Rowspan = 1;


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Argumentacion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            c1.Rowspan = 1;


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



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
            c1 = new PdfPCell(new Phrase($"Resultados: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 12;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"Basado en el anális del expediente No. ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 5;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            if ((tbl_sol_Solicitud.No_Registro ?? "") == "")
            {
                c1 = new PdfPCell(new Phrase($"{tbl_sol_Solicitud.Solicitud_NumeroExpediente}", fntTituloTabla));
            }
            else
            {
                c1 = new PdfPCell(new Phrase(tbl_sol_Solicitud.Solicitud_NumeroExpediente + " con número de registro " + tbl_sol_Solicitud.No_Registro, fntTituloTabla));
            }


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

            decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);

            if (TipoGestion == (decimal)0.00)
            {
                c1 = new PdfPCell(new Phrase($"APROBAR LA INSCRIPCIÓN", fntTituloTabla));
            }
            else
            {
                c1 = new PdfPCell(new Phrase($"APROBAR LA ACTUALIZACIÓN", fntTituloTabla));
            }

            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 7;
            c1.Rowspan = 1;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            ///////////////////////////////////////////



            c1 = new PdfPCell(new Phrase($"del área como " + $"{tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion}" + " con los datos que se consignan en el presente informe. Por las razones siguientes :", fntTituloTabla));
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
            //c1.Border = PdfPCell.BOTTOM_BORDER;

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

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" Técnico forestal", fntTituloTabla));
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

            return;
        }

        private void LlenaDatosEmpresaEntidadActividad(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_TecnicoActividad> tbl_Sol_Empresa_Entidad_TecnicoActividads = db.Tbl_Sol_Empresa_Entidad_TecnicoActividad.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Actividad_id).ToList();

            if (tbl_Sol_Empresa_Entidad_TecnicoActividads.Count() > 0)
            {
                LlenaBanner($"ACTIVIDADES DE LA EMPRESA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"ACTIVIDAD", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Empresa_Entidad_TecnicoActividads)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Actividad.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadMateriaPrima(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima> tbl_Sol_Empresa_Entidad_TecnicoMateria_Primas = db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Materia_Prima_id).ToList();
            if (tbl_Sol_Empresa_Entidad_TecnicoMateria_Primas.Count() > 0)
            {

                if ((tbl_Sol_Solicitud.Sub_Categoria_id == 3) || (tbl_Sol_Solicitud.Sub_Categoria_id == 6))
                {
                    LlenaBanner($"PRODUCTOS");
                }
                else
                {
                    LlenaBanner($"MATERIA PRIMA UTILIZADA EN EL PROCESO DE PRODUCCIÓN");
                }

                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"PRODUCTO", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Empresa_Entidad_TecnicoMateria_Primas)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Materia_Prima.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadMaquinariaUtilizada(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada> tbl_Sol_TecnicoEmpresa_Entidad_TecnicoMaquinaria_Utilizadas = db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Maquinaria_Utilizada_id).ToList();
            if (tbl_Sol_TecnicoEmpresa_Entidad_TecnicoMaquinaria_Utilizadas.Count() > 0)
            {
                LlenaBanner($"EQUIPO Y MAQUINARIA UTILIZADA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"EQUIPO - MAQUINARIA", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_TecnicoEmpresa_Entidad_TecnicoMaquinaria_Utilizadas)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Maquinaria_Utilizada.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadViveroForestal(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(8);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal> tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals = db.Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Vivero_Forestal_id).ToList();
            if (tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals.Count() > 0)
            {
                LlenaBanner($"PRINCIPALES ESPECIES EN PRODUCCION");
                doc.Add(tableBanner);

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Nombre Científico / Nombre Común", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Producción Anual de Plantas", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Procedencia de la Semilla", fntTituloTabla));
                c1.Colspan = 4;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Nota de Control de Semilla Certificada", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Finca, Municipio, Departamento, País", fntTituloTabla));
                c1.Colspan = 3;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"No. RNF Origen", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                int contador = 0;
                decimal sumatoria = 0;
                foreach (var item in tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals)
                {
                    contador++;

                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Especie.NombreCientifico}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    sumatoria += item.Produccion_Anual_Plantas;
                    c1 = new PdfPCell(new Phrase($"{item.Produccion_Anual_Plantas}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Nombre_Finca}, {item.Tbl_Gral_Municipio.Municipio}, {item.Tbl_Gral_Departamento.Departamento}, {item.Tbl_Gral_Pais.Pais}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                    if ((item.Codigo_RNF ?? "") == "")
                    {
                        c1 = new PdfPCell(new Phrase($"No indica procedencia", fntTituloTabla));
                        c1.Colspan = 2;
                        tablePersoneria.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{item.Codigo_RNF}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{item.Numero_Nota_Control_Semilla_Certificada}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }
                }

                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                c1.Border = 0;
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{sumatoria}", fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                c1.Border = 0;
                c1.Colspan = 5;
                tablePersoneria.AddCell(c1);

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadMotosierraMarcaModelo(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Motosierra_Marca_Modelo> tbl_Sol_Motosierra_Marca_Modelos = db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Motosierra_id).ToList();
            if (tbl_Sol_Motosierra_Marca_Modelos.Count() > 0)
            {
                LlenaBanner($"MAQUINARIA UTILIZADA EN EL PROCESO DE PRODUCCION");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Marca", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Modelo", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Motosierra_Marca_Modelos)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Marca}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Modelo}", fntTituloTabla));
                    c1.Colspan = 2;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        public string GenerarReporteDeAprobacion(long Solicitud_id, string Guidetapa, string Argumentacion)
        {

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-A" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == objUs.intUsuario_id
                                                             select d).FirstOrDefault();

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();
            List<Tbl_API_Sol_Finca> tbl_API_Sol_Fincas = (from d in db_API.Tbl_API_Sol_Finca
                                                          where d.Solicitud_id == tbl_sol_Solicitud.Solicitud_id
                                                          orderby d.Finca_Id
                                                          select d).ToList();

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == Solicitud_id).FirstOrDefault();


            strNombre = Guidetapa + ".pdf";

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = strNombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            try
            {

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
                Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);

                CrearBanner crearBanner = new CrearBanner();

                decimal TipoGestion = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);

                string varTitulo = "";

                varTitulo = "INFORME TÉCNICO PARA INSCRIPCIÓN DE " + tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToUpper();

                if (TipoGestion == (decimal)0.00)
                {
                    varTitulo = "INFORME TÉCNICO PARA INSCRIPCIÓN DE " + tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToUpper();
                }

                if ((TipoGestion == (decimal)0.01) || (TipoGestion == (decimal)0.02))
                {
                    varTitulo = "INFORME TÉCNICO PARA ACTUALIZACIÓN DE " + tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToUpper();
                }

                if ((TipoGestion == (decimal)0.04))
                {
                    varTitulo = "INFORME TÉCNICO PARA INACTIVACIÓN DE " + tbl_sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToUpper();
                }

                crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);


                //LlenaTituloRevision(tbl_sol_Solicitud);
                //doc.Add(tableTitulo);
                //doc.Add(Enter);

                int intVarEntidad = 0;

                if (tbl_Sol_Empresa_Entidad != null)
                {
                    intVarEntidad = 1;
                }

                doc.Add(Enter);
                LlenaDatosGenerales(tbl_sol_Solicitud, tbl_Sol_Finca.Count(), intVarEntidad, resultsp);
                doc.Add(tableDatosGenerales);
                doc.Add(Enter);

                fc_Sol_Sel_Direccion_Result DireccionSolicitud = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();
                if (tbl_Sol_Finca.Count() > 0)
                {
                    if (tbl_API_Sol_Fincas.Count() > 1)
                    {
                        LlenaBanner($"{Texto_Romano[intTexto_Romano]} DATOS DE LA FINCAS");
                        doc.Add(tableBanner);
                        intTexto_Romano = intTexto_Romano + 1;
                    }

                    if (tbl_API_Sol_Fincas.Count() == 1)
                    {
                        LlenaBanner($"{Texto_Romano[intTexto_Romano]} DATOS DE LA FINCA");
                        doc.Add(tableBanner);
                        intTexto_Romano = intTexto_Romano + 1;
                    }

                    for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                    {
                        LlenaDatosFinca(tbl_Sol_Finca[i], i + 1);
                        doc.Add(tableDatosFinca);
                    }
                    doc.Add(Enter);


                    if (tbl_Sol_Finca.Count() > 1)
                    {
                        LlenaBanner($"{Texto_Romano[intTexto_Romano]}.1. DIRECCIÓN DE LA FINCAS");
                        intTexto_Romano = intTexto_Romano + 1;
                        doc.Add(tableBanner);
                    }
                    if (tbl_Sol_Finca.Count() == 1)
                    {
                        LlenaBanner($"{Texto_Romano[intTexto_Romano]}.1. DIRECCIÓN DE LA FINCA");
                        intTexto_Romano = intTexto_Romano + 1;
                        doc.Add(tableBanner);
                    }


                    for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                    {

                        LlenaDatosFincaDireccion(tbl_Sol_Finca[i], i + 1);

                        doc.Add(tableDatosFinca);
                    }
                    doc.Add(Enter);

                }


                if ((tbl_sol_Solicitud.Categoria_id == 5) || (tbl_sol_Solicitud.Categoria_id == 11) || ((tbl_sol_Solicitud.Categoria_id == 8) && (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE LA EMPRESA");
                    doc.Add(tableBanner);
                    intTexto_Romano = intTexto_Romano + 1;

                    LlenaDatosDeLaEmpresa(tbl_sol_Solicitud, DireccionSolicitud);
                    doc.Add(tablePersoneria);
                    doc.Add(Enter);


                }

                Personerias personerias = ObtenerPersonerias(Solicitud_id);
                int countPersonerias = personerias.PropietariosIndividuales.Count() + personerias.PropietariosJuridicos.Count() + personerias.RepresentatnteLegal.Count() + personerias.Mandatario.Count() + personerias.ArrendatariosIndividuales.Count() + personerias.ArrendatariosJuridicos.Count();

                if (countPersonerias > 0)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. PERSONA INDIVIDUAL O JURIDICA");
                    doc.Add(tableBanner);

                    intTexto_Romano = intTexto_Romano + 1;

                    //LlenaDatosPersonaInividualJuridica(Solicitud_id);
                    LlenaDatosSolicitante(Solicitud_id, personerias);
                    doc.Add(tableDatosGenerales);

                    doc.Add(Enter);

                }

                if ((tbl_sol_Solicitud.Categoria_id == 5) || (tbl_sol_Solicitud.Categoria_id == 11) || ((tbl_sol_Solicitud.Categoria_id == 8) && (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
                {

                    //if (tbl_Sol_Empresa_Entidad.CapacidadInstalada != null)
                    //{
                    //    var CapacidadInstalada = new Paragraph("                  Capacidad instalada para producción mensual: " + tbl_Sol_Empresa_Entidad.CapacidadInstalada.ToString() + " metros cubicos.");
                    //    doc.Add(CapacidadInstalada);
                    //    doc.Add(Enter);
                    //}

                    LlenaDatosEmpresaEntidadActividad(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMaquinariaUtilizada(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMateriaPrima(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadViveroForestal(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMotosierraMarcaModelo(doc, tbl_sol_Solicitud);

                }


                if (tbl_Sol_Finca.Count() > 0)
                {

                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. RESULTADOS");
                    doc.Add(tableBanner);

                    intTexto_Romano = intTexto_Romano + 1;

                    for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                    {

                        LlenaDatosFincaResultado(tbl_Sol_Finca[i], i + 1);

                        doc.Add(tableDatosFinca);

                    }
                    doc.Add(Enter);

                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. MAPA DEL ÁREA A EVALUAR ");
                    doc.Add(tableBanner);
                    intTexto_Romano = intTexto_Romano + 1;


                    for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                    {

                        LlenaDatosFincaMapaEvaluar(tbl_Sol_Finca[i], i + 1);

                        doc.Add(tableDatosFinca);

                    }
                    doc.Add(Enter);


                    if (tbl_Sol_Finca.Count() > 0)
                    {
                        LlenaBanner($"{Texto_Romano[4]}. DATOS DE LA PLANTACIÓN");
                        doc.Add(tableBanner);
                    }

                    long solid = 0;
                    long fincaid = 0;

                    for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                    {
                        LlenaDatosPlantacion(tbl_Sol_Finca[i]);
                        doc.Add(tableDatosPlantacion);
                        doc.Add(Enter);
                        solid = tbl_Sol_Finca[i].Solicitud_id;
                        fincaid = tbl_Sol_Finca[i].Finca_Id;
                        List<Tbl_Sol_Rodal_CultivoTecnico> tbl_sol_Rodal_CultivoTecnico = db.Tbl_Sol_Rodal_CultivoTecnico.Where(Obj => Obj.Solicitud_id == solid && Obj.Finca_id == fincaid).ToList();

                        if (tbl_sol_Rodal_CultivoTecnico == null)
                        {
                            tbl_sol_Rodal_CultivoTecnico = new List<Tbl_Sol_Rodal_CultivoTecnico>();
                        }
                        if (tbl_sol_Rodal_CultivoTecnico.Count > 0)
                        {
                            LlenaDatosCultivoEnAsocio(tbl_sol_Rodal_CultivoTecnico);
                            doc.Add(tableDatosPlantacion);

                        }

                    }

                    if (tbl_API_Sol_Fincas.Count() > 0)
                    {
                        if (tbl_sol_Solicitud.Categoria_id == 6)
                        {
                            for (int i = 0; i < tbl_API_Sol_Fincas.Count(); i++)
                            {
                                EstimacionVolumen_API_FS(tbl_API_Sol_Fincas[i]);
                                doc.Add(tableEstimacion);
                                doc.Add(Enter);
                            }

                            ResumenPV_API_FS(tbl_sol_Solicitud.Solicitud_id);
                            doc.Add(tableEstimacion);
                            doc.Add(Enter);

                        }
                        else
                        {
                            for (int i = 0; i < tbl_API_Sol_Fincas.Count(); i++)
                            {
                                EstimacionVolumen_API(tbl_API_Sol_Fincas[i]);
                                doc.Add(tableEstimacion);
                                doc.Add(Enter);
                            }

                        }




                        for (int i = 0; i < tbl_API_Sol_Fincas.Count(); i++)
                        {
                            DatosPoligonos_API(tbl_API_Sol_Fincas[i]);
                            doc.Add(tableEstimacion);
                            doc.Add(Enter);

                            DatosPoligonoDescuento_API(tbl_API_Sol_Fincas[i]);
                            doc.Add(tableEstimacion);
                            doc.Add(Enter);
                        }
                    }


                }


                PreFirma(tbl_sol_Solicitud);
                doc.Add(tableFirmaSolicitante);

                AgregarArgumentacion(Argumentacion);
                doc.Add(tableFirmaSolicitante);

                doc.Add(Enter);
                FirmaSolicitante(tbl_sol_Solicitud);
                doc.Add(tableFirmaSolicitante);

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

        private void LlenaDatosDeLaEmpresa(Tbl_Sol_Solicitud tbl_Sol_Solicitud, fc_Sol_Sel_Direccion_Result DireccionSolicitud)
        {
            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            string Direccion = "";
            string DireccionMovil = "";

            if ((DireccionSolicitud.Direccion != null) && (DireccionSolicitud.Direccion.Trim() != ""))
            {
                Direccion += DireccionSolicitud.Direccion.Trim() + ", ";
            }
            if ((DireccionSolicitud.Aldea != null) && (DireccionSolicitud.Aldea.Trim() != ""))
            {
                Direccion += "Aldea " + DireccionSolicitud.Aldea.Trim() + ", ";
            }
            Direccion += DireccionSolicitud.Municipio + ", " + DireccionSolicitud.Departamento;


            if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil != null) && (tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() != ""))
            {
                DireccionMovil = tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";
            }

            if((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio1 != null) && (tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento1 != null))
            {
                DireccionMovil += tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio1.Municipio + ", " + tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento1.Departamento;
            }


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

                //if (tbl_Sol_Empresa_Entidad.Objeto_Empresa != null && tbl_Sol_Empresa_Entidad.Objeto_Empresa.Trim() != "")
                //{

                //    c1 = new PdfPCell(new Phrase("Objeto de la Empresa: ", fntTituloTabla));
                //    c1.Colspan = 1;
                //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                //    tablePersoneria.AddCell(c1);

                //    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Objeto_Empresa}", fntTituloTabla));
                //    c1.Colspan = 3;
                //    tablePersoneria.AddCell(c1);

                //}

                //c1 = new PdfPCell(new Phrase("Número de NIT: ", fntTituloTabla));
                //c1.Colspan = 1;
                //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                //tablePersoneria.AddCell(c1);

                //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.No_NIT}", fntTituloTabla));
                //c1.Colspan = 3;
                //tablePersoneria.AddCell(c1);

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

                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.REPEJU_De_Id != 0 && tbl_Sol_Empresa_Entidad_Tipo_Registro.REPEJU_De_Id != null)
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

                    //c1 = new PdfPCell(new Phrase("Tipo de Registro: ", fntTituloTabla));
                    //c1.Colspan = 1;
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //tablePersoneria.AddCell(c1);
                    //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                    //c1.Colspan = 3;
                    //tablePersoneria.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase("Número de Registro: ", fntTituloTabla));
                    //c1.Colspan = 1;
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //tablePersoneria.AddCell(c1);
                    //c1 = new PdfPCell(new Phrase(nopartida, fntTituloTabla));
                    //c1.Colspan = 1;
                    //tablePersoneria.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase("Número de Folio: ", fntTituloTabla));
                    //c1.Colspan = 1;
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //tablePersoneria.AddCell(c1);
                    //c1 = new PdfPCell(new Phrase(nofolio, fntTituloTabla));
                    //c1.Colspan = 1;
                    //tablePersoneria.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase("Número de Libro: ", fntTituloTabla));
                    //c1.Colspan = 1;
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //tablePersoneria.AddCell(c1);
                    //c1 = new PdfPCell(new Phrase(nolibro, fntTituloTabla));
                    //c1.Colspan = 1;
                    //tablePersoneria.AddCell(c1);


                    //if (repeju != "")
                    //{
                    //    c1 = new PdfPCell(new Phrase("Tipo de REPEJU: ", fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //    tablePersoneria.AddCell(c1);
                    //    c1 = new PdfPCell(new Phrase(repeju, fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    tablePersoneria.AddCell(c1);
                    //}
                    //else
                    //{
                    //    c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                    //    c1.Colspan = 2;
                    //    c1.Border = 0;
                    //    tablePersoneria.AddCell(c1);

                    //}

                    //if ((noacta.Trim() != "") || (fechaacta.Trim() != ""))
                    //{
                    //    c1 = new PdfPCell(new Phrase("Número de Acta: ", fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //    tablePersoneria.AddCell(c1);
                    //    c1 = new PdfPCell(new Phrase(noacta, fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    tablePersoneria.AddCell(c1);
                    //    c1 = new PdfPCell(new Phrase("Fecha de Acta: ", fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //    tablePersoneria.AddCell(c1);
                    //    c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                    //    c1.Colspan = 1;
                    //    tablePersoneria.AddCell(c1);
                    //}


                    //c1 = new PdfPCell(new Phrase("Tipo de empresa Forestal a Registrar: ", fntTituloTabla));
                    //c1.Colspan = 1;
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //tablePersoneria.AddCell(c1);
                    //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion}", fntTituloTabla));
                    //c1.Colspan = 3;
                    //tablePersoneria.AddCell(c1);


                    c1 = new PdfPCell(new Phrase("GTM X: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Empresa_Entidad.GTMX_Tecnico ?? 0).ToString("0")}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;

                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("GTM Y: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Empresa_Entidad.GTMY_Tecnico ?? 0).ToString("0")}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_CENTER;
                    tablePersoneria.AddCell(c1);

                }


            }

        }

        class JsonRespuesta
        {
            public int CodRespuesta { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

        [HttpPost]
        public JsonResult GenerarReporteDeAprobacionTecnica(string Guidid, string Guidetapa, string Argumentacion)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            string TextoMostrar, Ubicacion; 
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.Mensaje = "";
            jsonRespuesta.Ubicacion = "";
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guidid).First();

            ViewBag.CantidadSubidos = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Tipo_Documento_id == 19).Count();

            if ((Argumentacion ?? "") == "")
            {
                Ubicacion = "";
                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.Mensaje = "Debe llenar el recuadro de argumentación para poder continuar el proceso";
                jsonRespuesta.Ubicacion = "";
                return Json(JsonConvert.SerializeObject(jsonRespuesta));
            }

            if (ViewBag.CantidadSubidos == 0)
            {
                Ubicacion = "";
                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.Mensaje = "Debe subir los anexos para poder continuar el proceso";
                jsonRespuesta.Ubicacion = "";
                return Json(JsonConvert.SerializeObject(jsonRespuesta));
            }

            try
            {
                Ubicacion = GenerarReporteDeAprobacion(tbl_sol_solicitud.Solicitud_id, Guidetapa, Argumentacion);
                jsonRespuesta.CodRespuesta = 1;
                jsonRespuesta.Mensaje = "Archivo generado exitosamente";
                jsonRespuesta.Ubicacion = Ubicacion;
            }
            catch (Exception ex)
            {
                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.Mensaje = "hubo un error acá " + ex.InnerException;
                jsonRespuesta.Ubicacion = null;
            }
            //TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";
            return Json(JsonConvert.SerializeObject(jsonRespuesta));
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
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

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
                //ServicePointManager.ServerCertificateValidationCallback = null;

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


        //        //Rootobject myDeserializedClass = JsonConvert.DeserializeObject<Rootobject>(response.Content.ToString());

        //        //return myDeserializedClass.Data[0].GoogleDriveIdNuevo;



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

            Constants.FirmaElectronicaInsertarBitacora(Guid_id, Guidetapa_id, strEnc, 1, 0, "A.- Inicia proceso de firma electronica Form_FormularioTecnicoReporteAprobacionController-JsonProcesarFirmaElectronica");
            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

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

                    // se cambio de 0 a 1 para pruebas de emananuel

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
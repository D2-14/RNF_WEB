using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

using Newtonsoft.Json;
using OfficeOpenXml;
using System.IO;

using Font = iTextSharp.text.Font;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using RestSharp;
using System.Data.Entity;
using iTextSharp.text.pdf.codec.wmf;
using static QRCoder.PayloadGenerator;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using ExcelDataReader.Log;

namespace RNF_Web.Controllers
{
    public class EncResp_EncuestaConstanciaController : Controller
    {
        db_RNF_SurveyEntities db_Survey = new db_RNF_SurveyEntities();

        db_RNFEntities db = new db_RNFEntities();


        private int intModalidadImpresa = 0;


        PdfPTable tableTitulo = new PdfPTable(3);
        PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        PdfPTable tableDatosInstrucciones = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosNotificacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosPlantacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosFinca = new PdfPTable(numColumns: 8);

        private PdfPTable tableCultivosEnAsocio = new PdfPTable(numColumns: 5);

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
        int Bordes_No = Rectangle.NO_BORDER;

        BaseColor Gris = BaseColor.GRAY;

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




        private void EspeciesForestales_Probosque(string No_Registro)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 5;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_RNF_Finca_Probosque_EspeciesForestales> tbl_RNF_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Probosque_EspeciesForestales
                                                                                                           where d.No_Registro == No_Registro
                                                                                                           orderby d.Finca_id, d.Correlativo_id
                                                                                                           select d).ToList();

            if (tbl_RNF_Finca_Probosque_EspeciesForestales == null)
            {
                tbl_RNF_Finca_Probosque_EspeciesForestales = new List<Tbl_RNF_Finca_Probosque_EspeciesForestales>();
            }



            if (tbl_RNF_Finca_Probosque_EspeciesForestales.Count() > 0)
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

                c1 = new PdfPCell(new Phrase("Correlativo", fntSubTitulo));
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




                for (int i = 0; i < tbl_RNF_Finca_Probosque_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Probosque_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Probosque_EspeciesForestales[i].Correlativo_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Probosque_EspeciesForestales[i].NombreCientifico.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (tbl_RNF_Finca_Probosque_EspeciesForestales[i].FechaPlantacion != null)
                    {
                        c1 = new PdfPCell(new Phrase($"{((DateTime)tbl_RNF_Finca_Probosque_EspeciesForestales[i].FechaPlantacion).ToString("dd/MM/yyyy")}", fntSubTitulo));
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

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_Probosque_EspeciesForestales[i].DensidadActual).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);





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

            List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales> tbl_RNF_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesForestales
                                                                                                           where d.No_Registro == No_Registro
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_RNF_Finca_PinpepOld_EspeciesForestales == null)
            {
                tbl_RNF_Finca_PinpepOld_EspeciesForestales = new List<Tbl_RNF_Finca_PinpepOld_EspeciesForestales>();
            }


            if (tbl_RNF_Finca_PinpepOld_EspeciesForestales.Count() > 0)
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



                for (int i = 0; i < tbl_RNF_Finca_PinpepOld_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesForestales[i].NombreEspecie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_PinpepOld_EspeciesForestales[i].ArbolesPorHa).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesForestales[i].Area}", fntSubTitulo));
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

            List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger> tbl_RNF_Finca_PinpepOld_EspeciesProtegers = (from d in db.Tbl_RNF_Finca_PinpepOld_EspeciesProteger
                                                                                                        where d.No_Registro == No_Registro
                                                                                                        orderby d.Rodal_id
                                                                                                        select d).ToList();


            if (tbl_RNF_Finca_PinpepOld_EspeciesProtegers == null)
            {
                tbl_RNF_Finca_PinpepOld_EspeciesProtegers = new List<Tbl_RNF_Finca_PinpepOld_EspeciesProteger>();
            }


            if (tbl_RNF_Finca_PinpepOld_EspeciesProtegers.Count() > 0)
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

                for (int i = 0; i < tbl_RNF_Finca_PinpepOld_EspeciesProtegers.Count(); i++)
                {

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesProtegers[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_PinpepOld_EspeciesProtegers[i].EspeciesProteger.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 3;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_PinpepOld_EspeciesProtegers[i].Area).ToString("0.00")}", fntSubTitulo));
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

            int maxColumnas = 7;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_RNF_Finca_Secorf_EspeciesForestales> tbl_RNF_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_RNF_Finca_Secorf_EspeciesForestales
                                                                                                     where d.No_Registro == No_Registro
                                                                                                     orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.Correlativo_id
                                                                                                     select d).ToList();

            if (tbl_RNF_Finca_Secorf_EspeciesForestales == null)
            {
                tbl_RNF_Finca_Secorf_EspeciesForestales = new List<Tbl_RNF_Finca_Secorf_EspeciesForestales>();
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

            if (tbl_RNF_Finca_Secorf_EspeciesForestales.Count() > 0)
            {

                for (int i = 0; i < tbl_RNF_Finca_Secorf_EspeciesForestales.Count(); i++)
                {
                    if ((lngFinca != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {

                        if (lngFinca != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id)
                        {
                            fincaid = tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id;
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
                        rodalid = tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id;
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

                        c1 = new PdfPCell(new Phrase("Año Establecimiento", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if ((lngFinca != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((lngFinca != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_Secorf_EspeciesForestales[i].Area_ha).ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Secorf_EspeciesForestales[i].Especie_id.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_Secorf_EspeciesForestales[i].DAP).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_Secorf_EspeciesForestales[i].Altura).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_RNF_Finca_Secorf_EspeciesForestales[i].Densidad_ha).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (tbl_RNF_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento != null)
                    {
                        if (tbl_RNF_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento == 0)
                        {
                            c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento}", fntSubTitulo));
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

                    lngFinca = tbl_RNF_Finca_Secorf_EspeciesForestales[i].Finca_id;
                    lngRodal = tbl_RNF_Finca_Secorf_EspeciesForestales[i].Rodal_id;




                }



            }


            return;
        }


        private void FormulasCalculoVolumen(string No_Registro)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");
            tableFormulas = new PdfPTable(11);
            List<Tbl_RNF_Rodal_Dasometrico_Especie_Formula> oEspecieFormula;
            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 2;
            tableFormulas.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"Fórmula", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 9;
            tableFormulas.AddCell(c1);

            oEspecieFormula = (from d in db.Tbl_RNF_Rodal_Dasometrico_Especie_Formula
                               where d.No_Registro == No_Registro
                               select d).ToList();

            for (int i = 0; i < oEspecieFormula.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].Especie_id, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 2;
                tableFormulas.AddCell(c1);
                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].strFormulario, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 9;
                tableFormulas.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }



        private void LlenaDatosPlantacion(Tbl_RNF_Finca tbl_RNF_Finca)
        {

            Tbl_Sol_FincaObjetivoDeLaPlantacion objetivoPlantacion = (from d in db.Tbl_Sol_FincaObjetivoDeLaPlantacion
                                                                      where d.ObjetivoDeLaPlantacion == tbl_RNF_Finca.ObjetivoDeLaPlantacion
                                                                      select d).FirstOrDefault();
            Tbl_Sol_Rodal_CategoriaSIGAP categoriaSIGAP = (from d in db.Tbl_Sol_Rodal_CategoriaSIGAP
                                                           where d.CategoriaSIGAP_Id == tbl_RNF_Finca.CategoriaSIGAP_Id
                                                           select d).FirstOrDefault();

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosPlantacion = new PdfPTable(11);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            string DescripcionPlantacion, DescripcionCategoriaSIGAP, CategoriaSIGAP;

            if (tbl_RNF_Finca.Area_SIGAP == true)
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

            c1 = new PdfPCell(new Phrase($"¿La plantación se encuentra dentro de área SIGAP?   {CategoriaSIGAP}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);


            if (CategoriaSIGAP == "Sí")
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

            if (objetivoPlantacion != null)
            {
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
            }

            tableBanner.AddCell(c1);
            return;
        }

        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Ubicacion { get; set; }
            public string Mensaje { get; set; }
        }


        // GET: EncResp_EncuestaConstancia
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FirmarEncuesta(int id_encuesta, long swcreatedby, int id_Correlativo)
        {
            ViewBag.id_encuesta = id_encuesta;
            ViewBag.swcreatedby = swcreatedby;
            ViewBag.id_Correlativo = id_Correlativo;

            return View();
        }


        public class Personerias
        {
            public List<Tbl_RNF_PropietarioPersonaIndividual> PropietariosIndividuales { get; set; }
            public List<Tbl_RNF_PropietarioPersonaJuridica> PropietariosJuridicos { get; set; }
            public List<fc_RNF_Rodal_RepresentanteMandatario_Result> RepresentatnteLegal { get; set; }
            public List<fc_RNF_Rodal_RepresentanteMandatario_Result> Mandatario { get; set; }
            public List<Tbl_RNF_ArrendatarioPersonaIndividual> ArrendatariosIndividuales { get; set; }
            public List<Tbl_RNF_ArrendatarioPersonaJuridica> ArrendatariosJuridicos { get; set; }
        }


        Personerias ObtenerPersonerias(string No_Registro)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<Tbl_RNF_PropietarioPersonaIndividual>();
            personerias.PropietariosJuridicos = new List<Tbl_RNF_PropietarioPersonaJuridica>();
            personerias.RepresentatnteLegal = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>();
            personerias.Mandatario = new List<fc_RNF_Rodal_RepresentanteMandatario_Result>();
            personerias.ArrendatariosIndividuales = new List<Tbl_RNF_ArrendatarioPersonaIndividual>();
            personerias.ArrendatariosJuridicos = new List<Tbl_RNF_ArrendatarioPersonaJuridica>();

            List<Tbl_RNF_ArrendatarioPersonaJuridica> tbl_RNF_ArrendatarioPersonaJuridicas = (from d in db.Tbl_RNF_ArrendatarioPersonaJuridica
                                                                                              where d.No_Registro == No_Registro && d.Estado_id == true
                                                                                              select d).ToList();

            List<Tbl_RNF_ArrendatarioPersonaIndividual> tbl_RNF_ArrendatarioPersonaIndividuals = (from d in db.Tbl_RNF_ArrendatarioPersonaIndividual
                                                                                                  where d.No_Registro == No_Registro && d.Estado_id == true
                                                                                                  select d).ToList();


            List<Tbl_RNF_PropietarioPersonaJuridica> tbl_RNF_PropietarioPersonaJuridicas = (from d in db.Tbl_RNF_PropietarioPersonaJuridica
                                                                                            where d.No_Registro == No_Registro && d.Estado_id == true
                                                                                            select d).ToList();

            List<Tbl_RNF_PropietarioPersonaIndividual> tbl_RNF_PropietarioPersonaIndividuals = (from d in db.Tbl_RNF_PropietarioPersonaIndividual
                                                                                                where d.No_Registro == No_Registro && d.Estado_id == true
                                                                                                select d).ToList();

            List<fc_RNF_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_RNF_Rodal_RepresentanteMandatario(No_Registro, false).ToList()
                                                                                     select d).ToList();

            List<fc_RNF_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_RNF_Rodal_RepresentanteMandatario(No_Registro, true).ToList()
                                                                             select d).ToList();


            if (tbl_RNF_PropietarioPersonaIndividuals.Count() > 0)
            {
                personerias.PropietariosIndividuales = tbl_RNF_PropietarioPersonaIndividuals;
            }

            if (tbl_RNF_PropietarioPersonaJuridicas.Count() > 0)
            {
                personerias.PropietariosJuridicos = tbl_RNF_PropietarioPersonaJuridicas;
            }

            if (oRepresentanteLegal.Count() > 0)
            {
                personerias.RepresentatnteLegal = oRepresentanteLegal;
            }

            if (oMandatario.Count() > 0)
            {
                personerias.Mandatario = oMandatario;
            }

            if (tbl_RNF_ArrendatarioPersonaIndividuals.Count() > 0)
            {
                personerias.ArrendatariosIndividuales = tbl_RNF_ArrendatarioPersonaIndividuals;
            }

            if (tbl_RNF_ArrendatarioPersonaJuridicas.Count() > 0)
            {
                personerias.ArrendatariosJuridicos = tbl_RNF_ArrendatarioPersonaJuridicas;
            }

            return personerias;
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



        private void LlenaTresTextos(string TextoA, string TextoB, string TextoC)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

            tableTitulo = new PdfPTable(6);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
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

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            return;
        }

        private void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);


            tableTitulo = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);

            return;
        }



        private void LlenaDatosSolicitante(string No_Registro, Personerias personerias)
        {


            DateTime fecha;
            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

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

                    if ((tbl_RNF_Registro.No_Registro == null) && (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld || tbl_RNF_Registro.Procedencia_Probosque))
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

                    if ((tbl_RNF_Registro.No_Registro == null) && (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld || tbl_RNF_Registro.Procedencia_Probosque))
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

        public class ClassCultivosEnAsocio
        {
            public string Finca { get; set; }
            public string Area { get; set; }
            public string Tipo_De_Area { get; set; }
            public string Cultivo { get; set; }
            public string Anio_Establecimiento { get; set; }
        }


        private void LlenarCultivosEnAsocio(string No_Registro, List<ClassCultivosEnAsocio> Resultado)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            tableCultivosEnAsocio = new PdfPTable(5);


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



            }

            return;
        }


        private PdfPTable tableRespuesta = new PdfPTable(7);
        PdfPCell c1 = new PdfPCell();
        int Contador = 0;
        Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
        Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

        public JsonRespuesta GenerarConstancia_PDF(Tbl_EncResp_Encuesta model)
        {

            string No_Registro = model.No_Registro;

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            CrearBanner crearBanner = new CrearBanner();

            string sqlQuery;
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            string guidid = Guid.NewGuid().ToString();

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
            strNombre = $"U{guidid}.pdf";

            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            Rectangle tamanioDocumento = PageSize.LETTER;
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            Document doc = new Document(tamanioDocumento);
            doc.SetMargins(1f, 1f, 25f, 50f);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            //var wartermark = new Wat8ermark();
            //writer.PageEvent = wartermark;
            doc.Open();
            try
            {
                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlact = this.Url.Action();
                    LlenaBanner(Leyenda: urlact, Color: "Gris", alineacionhorizontal: Al_Centro, borde: 15);
                    doc.Add(tableBanner);
                }



                //Por el momento este documento no se encuentra vinculado a una etapa o una solicitud como tal, por lo que el encabezado que se suele usar comúnmente, esta vez estará con los datos quemados, está pendiente verificar qué datos o de dónde se tomará esta info
                string varTitulo = "Boleta de monitoreo";
                string codigo = "---";
                string version = "---";
                string strfecha = "---";
                doc.Add(Enter);
                doc.Add(Enter);
                crearBanner.LlenaTituloRevision(varTitulo, codigo, version, strfecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);
                doc.Add(Enter);

                //Copiado de registro

                Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();



                Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = null;
                if (tbl_RNF_Registro.UsuarioExterno_id != null)
                {

                    tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                              where d.Usuario_id == tbl_RNF_Registro.UsuarioExterno_id
                                              select d).FirstOrDefault();

                }

                List<Tbl_RNF_Finca> tbl_RNF_Finca = (from d in db.Tbl_RNF_Finca
                                                     where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                     select d).OrderBy(d => d.Finca_Id).ToList();

                fc_RNF_Sel_Direccion_Result DireccionSolicitud = db.fc_RNF_Sel_Direccion(tbl_RNF_Registro.No_Registro).FirstOrDefault();

                Personerias personerias = ObtenerPersonerias(No_Registro);
                int countPersonerias = personerias.PropietariosIndividuales.Count() + personerias.PropietariosJuridicos.Count() + personerias.RepresentatnteLegal.Count() + personerias.Mandatario.Count() + personerias.ArrendatariosIndividuales.Count() + personerias.ArrendatariosJuridicos.Count();

                string nombre = tbl_RNF_Registro.No_Registro.Replace("-", "_").Replace(" ", "");
                nombre = SecurEncryptDecrypt.EncryptString(nombre);
                nombre = nombre.Replace("-", "_").Replace(" ", "").Replace("/", "_").Replace(".", "_").Replace("+", "_").Replace("¿", "_").Replace("?", "_").Replace(";", "_").Replace(",", "_").Replace("=", "_");


                try
                {
                    doc.Add(Enter);
                    if (Constants.VisualizarInformacionDesarrollo == 1)
                    {
                        string urlact = this.Url.Action();
                        LlenaBanner(urlact);
                        doc.Add(tableBanner);
                        doc.Add(Enter);

                    }

                    string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();

                    LlenaBanner(strFecha, "Derecha", "Blanco");
                    doc.Add(tableBanner);


                    string strNumero;

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "Datos Registrados", Color: "GrisClaro", alineacionhorizontal: Al_Centro, tamaniotexto: 25);
                    doc.Add(tableBanner);
                    doc.Add(Enter);

                    if ((tbl_RNF_Registro.Expediente != null) && (tbl_RNF_Registro.Expediente != ""))
                    {
                        strNumero = tbl_RNF_Registro.Expediente;
                        LlenaTresTextos("", "", "Número de expediente : " + strNumero);
                        doc.Add(tableTitulo);

                    }


                    if (tbl_RNF_Registro.Procedencia_PinpepNew || tbl_RNF_Registro.Procedencia_PinpepOld || tbl_RNF_Registro.Procedencia_Probosque || tbl_RNF_Registro.Procedencia_secorf)
                    {
                        if ((tbl_RNF_Registro.Procedencia_Expediente != null) && (tbl_RNF_Registro.Procedencia_Expediente.Trim() != ""))
                        {
                            var NumeroDeExpedienteOrigen = new Paragraph("Número de expediente de origen: " + tbl_RNF_Registro.Procedencia_Expediente);
                            LlenaBanner("Número de expediente de origen: " + tbl_RNF_Registro.Procedencia_Expediente, "Derecha", "Blanco");
                            doc.Add(tableBanner);
                        }
                    }

                    if ((tbl_RNF_Registro.No_Registro != null) && (tbl_RNF_Registro.No_Registro != ""))
                    {

                        strNumero = tbl_RNF_Registro.No_Registro;
                        LlenaTresTextos("", "", "Número de registro       : " + strNumero);
                        doc.Add(tableTitulo);


                    }

                    Constants constant = new Constants();

                    LlenaCuatroTextos("Región: ", DireccionSolicitud.NoRegion + " " + constant.initCapPalabras(DireccionSolicitud.NombreRegion), "Sub Región:", DireccionSolicitud.NoSubRegion + " " + constant.initCapPalabras(DireccionSolicitud.NombreSubRegion));
                    doc.Add(tableTitulo);
                    doc.Add(Enter);

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

                    Modalidad = Modalidad + " " + subModalidad;

                    Constants Const = new Constants();
                    if ((Modalidad != "") && (Modalidad != " "))
                    {
                        Modalidad = Const.initCapTexto(Modalidad);
                    }
                    else
                    {
                        Modalidad = "";
                    }

                    #region Sub categoría o modalidad para profesional
                    if (tbl_RNF_Registro.Categoria_id == 7)
                    {
                        if (Modalidad != "")
                        {
                            LlenaBanner("Subcategoría : " + ' ' + Modalidad, "Izquierda", "Blanco");
                            doc.Add(tableBanner);
                            doc.Add(Enter);
                        }
                    }
                    #endregion



                    if (countPersonerias > 0)
                    {
                        LlenaBanner($"DATOS DEL PROPIETARIO");
                        doc.Add(tableBanner);
                        LlenaDatosSolicitante(No_Registro, personerias);
                        doc.Add(tableDatosGenerales);
                        doc.Add(Enter);
                    }


                    if (tbl_RNF_Registro.Categoria_id == 7)
                    {
                        LlenaBanner($" DATOS DEL SOLICITANTE");
                        doc.Add(tableBanner);
                        LlenaDatosProfesionalPersonales(tbl_RNF_Registro);
                        doc.Add(tableDatosNotificacion);
                        doc.Add(Enter);

                        LlenaBanner($"DATOS DE UBICACIÓN");
                        doc.Add(tableBanner);
                        LlenaDatosProfesionalDireccion(tbl_RNF_Registro);
                        doc.Add(tableDatosNotificacion);
                        doc.Add(Enter);


                    }

                    var Tbl_RNFMotosiera = db.Tbl_RNF_Motosierra.Where(obj => obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();

                    foreach (var itemMotosierra in Tbl_RNFMotosiera)
                    {
                        LlenaBanner( ". DATOS DE LA MOTOSIERRA");
                        doc.Add(tableBanner);
                        LlenaDatosDeLaMotosierra(itemMotosierra);
                        doc.Add(tablePersoneria);
                    }


                    if (tbl_RNF_Registro.Categoria_id == 7)
                    {
                        LlenaBanner($"FORMACION ACADEMICA");
                        doc.Add(tableBanner);
                        LlenaDatosProfesional(tbl_RNF_Registro);
                        doc.Add(tableDatosGenerales);
                        doc.Add(Enter);

                    }


                    if ((tbl_RNF_Registro.Categoria_id == 5) || (tbl_RNF_Registro.Categoria_id == 9) || ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2)))
                    {

                        if (tbl_RNF_Registro.Categoria_id == 9)
                        {
                            LlenaBanner($"DATOS DE LA ENTIDAD");
                            doc.Add(tableBanner);
                        }

                        LlenaDatosDeLaEmpresa(tbl_RNF_Registro, DireccionSolicitud);
                        doc.Add(tablePersoneria);
                        doc.Add(Enter);

                        LlenaDatosEmpresaEntidadActividad(doc, tbl_RNF_Registro);

                        LlenaDatosEmpresaEntidadMaquinariaUtilizada(doc, tbl_RNF_Registro);

                        LlenaDatosEmpresaEntidadMateriaPrima(doc, tbl_RNF_Registro);

                        LlenaDatosEmpresaEntidadViveroForestal(doc, tbl_RNF_Registro);

                        LlenaDatosEmpresaEntidadMotosierraMarcaModelo(doc, tbl_RNF_Registro);

                        Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();

                        if (tbl_RNF_Empresa_Entidad != null)
                        {
                            if (tbl_RNF_Empresa_Entidad.CapacidadInstalada != null)
                            {
                                var CapacidadInstalada = new Paragraph("                  Capacidad instalada para producción mensual: " + tbl_RNF_Empresa_Entidad.CapacidadInstalada.ToString() + " metros cubicos.");
                                doc.Add(CapacidadInstalada);
                                doc.Add(Enter);
                            }
                        }

                    }

                    if (Constants.VisualizarInformacionDesarrollo == 1)
                    {
                        LlenaBanner("ReportePV_RNFController/GenerarPVJuridico_PDF  Datos de la Finca");
                        doc.Add(tableBanner);
                    }

                    for (int ji = 0; ji < tbl_RNF_Finca.Count(); ji++)
                    {
                        LlenaBanner($"DATOS DE LA FINCA");
                        doc.Add(tableBanner);
                        //LlenaDatosFinca(tbl_Sol_Finca[i]);
                        //doc.Add(tableDatosFinca);
                        LlenaDatosDeLaFinca(tbl_RNF_Finca[ji]);
                        doc.Add(tablePersoneria);
                        doc.Add(Enter);

                        //ClassCultivosEnAsocio

                        sqlQuery = " Select convert(varchar(10),Finca_id) +'. ' + \n";
                        sqlQuery += "        (Select NombreFinca  \n";
                        sqlQuery += "        From Tbl_RNF_Finca \n";
                        sqlQuery += "        Where No_Registro = Cultivo.No_Registro \n";
                        sqlQuery += "        and Finca_id = Cultivo.Finca_id) Finca, \n";
                        sqlQuery += "        convert(varchar(10),Rodal_id) Area, \n";
                        sqlQuery += "        (case when Tipo_de_Area = 1 then 'Rodal' else 'Arboles en línea' end) Tipo_De_Area, \n";
                        sqlQuery += "        (SElect Nombres_Comunes from Tbl_Gral_Cultivo Where Cultivo_id = Cultivo.Cultivo_id) Cultivo, \n";
                        sqlQuery += "        convert(varchar(10),Anio_Establecimiento) Anio_Establecimiento \n";
                        sqlQuery += "        From Tbl_RNF_Rodal_Cultivo Cultivo \n";
                        sqlQuery += $"       Where Cultivo.No_Registro = '{tbl_RNF_Registro.No_Registro}' \n";
                        sqlQuery += $"         and Cultivo.Finca_id = {tbl_RNF_Finca[ji].Finca_Id} \n";




                        List<ClassCultivosEnAsocio> Resultado = new List<ClassCultivosEnAsocio> { };

                        Resultado = db.Database.SqlQuery<ClassCultivosEnAsocio>(sqlQuery).ToList();


                        if (Resultado.Count() > 0)
                        {
                            LlenaBanner("CULTIVOS EN ASOCIO");
                            doc.Add(tableBanner);
                            LlenarCultivosEnAsocio(No_Registro, Resultado);
                            doc.Add(tableCultivosEnAsocio);
                            doc.Add(Enter);
                        }

                        /// fin Class

                        if ((tbl_RNF_Registro.No_Registro == null) && (!tbl_RNF_Registro.Procedencia_PinpepNew && !tbl_RNF_Registro.Procedencia_PinpepOld && !tbl_RNF_Registro.Procedencia_Probosque && !tbl_RNF_Registro.Procedencia_secorf))
                        {
                            LlenaBanner($" DATOS DE LA PLANTACION");
                            doc.Add(tableBanner);
                            LlenaDatosPlantacion(tbl_RNF_Finca[ji]);
                            doc.Add(tableDatosPlantacion);
                            if (tableDatosPlantacion.Rows.Count() > 0)
                            {
                                doc.Add(Enter);
                            }
                        }

                        if (tbl_RNF_Registro.Categoria_id != 6)
                        {
                            EstimacionVolumen(tbl_RNF_Finca[ji], Modalidad);
                            doc.Add(tableEstimacion);
                        }

                        if (tbl_RNF_Registro.Categoria_id == 6)
                        {
                            EstimacionVolumenFS(tbl_RNF_Finca[ji], Modalidad);
                            doc.Add(tableEstimacion);
                        }

                       

                    }


                    if (tbl_RNF_Registro.Categoria_id == 6)
                    {
                        ResumenPV_FS(tbl_RNF_Registro.No_Registro);
                        doc.Add(tableEstimacion);
                    }

                    int cantTbl_RNF_Rodal_Dasometrico_Especie_Formula = db.Tbl_RNF_Rodal_Dasometrico_Especie_Formula.Where(Obj => Obj.No_Registro == No_Registro).Count();

                    if (cantTbl_RNF_Rodal_Dasometrico_Especie_Formula > 0)
                    {
                        LlenaBanner($"FORMULAS UTILIZADAS PARA EL CÁLCULO DE VOLUMEN POR ESPECIE");
                        doc.Add(tableBanner);
                        FormulasCalculoVolumen(No_Registro);
                        doc.Add(tableFormulas);
                    }


                    EspeciesForestales_Probosque(No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesForestales_PinpepOld(No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesProteger_PinpepOld(No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }

                    EspeciesForestales_Secorf(No_Registro);
                    doc.Add(tableEstimacion);
                    if (tableEstimacion.Rows.Count() > 0)
                    {
                        doc.Add(Enter);
                    }



                }
                catch (Exception ex)
                {
                    doc.Close();
                    writer.Close();
                    jsonRespuesta.Ubicacion = null;
                    jsonRespuesta.Result = 2;
                    jsonRespuesta.Mensaje = ex.Message;
                }



                LlenaBanner(Leyenda: "Datos del monitoreo", Color: "GrisClaro", alineacionhorizontal: Al_Centro, tamaniotexto: 25);
                doc.Add(tableBanner);
                doc.Add(Enter);
                // Copiado de registro
                List<Tbl_EncResp_Encuesta_Pregunta> preguntas = model.Tbl_EncResp_Encuesta_Pregunta.ToList();
                List<Tbl_EncResp_Encuesta_Documento> documentos = db_Survey.Tbl_EncResp_Encuesta_Documento.Where(Obj => Obj.id_encuesta == model.id_encuesta && Obj.swcreatedby == model.swcreatedby && Obj.id_Correlativo == model.id_Correlativo).ToList();
                List<Tbl_EncResp_Encuesta_PreguntaRespuesta> preguntaRespuestas = new List<Tbl_EncResp_Encuesta_PreguntaRespuesta>();
                List<Tbl_EncResp_Encuesta_Documento> documentos_pregunta = new List<Tbl_EncResp_Encuesta_Documento>();
                int countrespuestas = 0;
                int countdocumentos = 0;
                int i = 0;
                string titulopregunta = "";
                string archivoadjuntar = "";
                string respuestapregunta = "";
                string rootbase = Server.MapPath("~/");
                string Ext = "";

                foreach (var item in preguntas)
                {

                    preguntaRespuestas = (from d in item.Tbl_EncResp_Encuesta_PreguntaRespuesta
                                          where d.id_pregunta == item.id_pregunta
                                          select d).ToList();

                    documentos_pregunta = (from d in documentos
                                           where d.id_pregunta == item.id_pregunta
                                           orderby d.id_CorrelativoDocumento
                                           select d).ToList();

                    if (preguntaRespuestas == null)
                    {
                        preguntaRespuestas = new List<Tbl_EncResp_Encuesta_PreguntaRespuesta>();
                    }

                    if (documentos_pregunta == null)
                    {
                        documentos_pregunta = new List<Tbl_EncResp_Encuesta_Documento>();
                    }


                    //La idea es llegar al título de la pregunta a través de los parámetros heredados

                    titulopregunta = item.Tbl_EncResp_Encuesta.Tbl_Enc_Encuesta.Tbl_Enc_Encuesta_Pregunta.Where(Obj => Obj.id_encuesta == item.id_encuesta && Obj.id_pregunta == item.id_pregunta).FirstOrDefault().descripcion;


                    LlenaBanner(Leyenda: titulopregunta, Color: "GrisClaro", alineacionhorizontal: Al_Izquierda, borde: 15, tamaniotexto: 13);
                    doc.Add(tableBanner);


                    if (preguntaRespuestas.Count() > 0)
                    {
                        foreach (var respuesta in preguntaRespuestas)
                        {

                            tableRespuesta = new PdfPTable(7);

                            Contador = 2;
                            c1 = new PdfPCell(new Phrase("", fntTitulo2));
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableRespuesta.AddCell(c1);

                            if ((respuesta.Tbl_Enc_Encuesta_Respuesta.respuesta != null) && (respuesta.Tbl_Enc_Encuesta_Respuesta.respuesta.Trim() != ""))
                            {
                                c1 = new PdfPCell(new Phrase(respuesta.Tbl_Enc_Encuesta_Respuesta.respuesta, fntTitulo2));

                                if ((respuesta.Respuesta_Observaciones == null) || (respuesta.Respuesta_Observaciones.Trim() == ""))
                                {
                                    Contador = Contador + 5;
                                    c1.Colspan = 5;
                                }
                                else
                                {
                                    Contador = Contador + 1;
                                    c1.Colspan = 1;
                                }
                                tableRespuesta.AddCell(c1);
                            }

                            if ((respuesta.Respuesta_Observaciones != null) && (respuesta.Respuesta_Observaciones.Trim() != ""))
                            {
                                Contador = Contador + 2;
                                c1 = new PdfPCell(new Phrase(respuesta.Tbl_Enc_Encuesta_Respuesta.ExplicacionTexto, fntTitulo2));
                                c1.Colspan = 2;
                                tableRespuesta.AddCell(c1);

                                Contador = Contador + 2;
                                c1 = new PdfPCell(new Phrase(respuesta.Respuesta_Observaciones, fntTitulo2));
                                c1.Colspan = 2;
                                tableRespuesta.AddCell(c1);
                            }

                            if (Contador < 7)
                            {
                                c1 = new PdfPCell(new Phrase("", fntTitulo2));
                                c1.Colspan = 7 - Contador;
                                c1.Border = 0;
                                tableRespuesta.AddCell(c1);
                            }

                            doc.Add(tableRespuesta);

                        }

                    }
                    if (documentos_pregunta.Count() > 0)
                    {
                        foreach (var documento in documentos_pregunta)
                        {
                            doc.Add(Enter);

                            tableRespuesta = new PdfPTable(7);

                            c1 = new PdfPCell(new Phrase("", fntTitulo2));
                            c1.Colspan = 1;
                            c1.Rowspan = 7;
                            c1.Border = 0;
                            tableRespuesta.AddCell(c1);


                            archivoadjuntar = $"{rootbase}Encuesta/{documento.id_encuesta}/{documento.swcreatedby}/{documento.id_Correlativo}/{documento.id_pregunta}/{documento.Documento}";
                            Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();
                            
                            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(archivoadjuntar);

                            if  (image.Width > 400)
                            { 

                                float Equivalencia = 400/image.Width;

                                float ConversionHeigth = image.Height * Equivalencia;

                                image.ScaleAbsolute(400.0F, ConversionHeigth);
                            }

                            c1 = new PdfPCell(image);

                            c1.Colspan = 5;
                            c1.Rowspan = 7;

                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.Border = 0;
                            tableRespuesta.AddCell(c1);


                            Contador = 1;
                            c1 = new PdfPCell(new Phrase("", fntTitulo2));
                            c1.Colspan = 1;
                            c1.Rowspan = 7;
                            c1.Border = 0;
                            tableRespuesta.AddCell(c1);

                            doc.Add(tableRespuesta);

                        }
                    }

                    if ((item.Respuesta_Observaciones != null) && (item.Respuesta_Observaciones.Trim() != ""))
                    {
                        tableRespuesta = new PdfPTable(7);

                        Contador = 2;
                        c1 = new PdfPCell(new Phrase("", fntTitulo2));
                        c1.Colspan = 2;
                        c1.Border = 0;
                        tableRespuesta.AddCell(c1);
                        if (item.Tbl_Enc_Encuesta_Pregunta.titulo_observacion == null)
                        {
                            Contador = 2 + 3;
                            c1 = new PdfPCell(new Phrase(item.Respuesta_Observaciones, fntTitulo2));
                            c1.Colspan = 5;
                            tableRespuesta.AddCell(c1);
                            doc.Add(tableRespuesta);
                        }
                        else
                        { 
                            Contador = Contador + 2;
                            c1 = new PdfPCell(new Phrase(item.Tbl_Enc_Encuesta_Pregunta.titulo_observacion, fntTitulo2));
                            c1.Colspan = 2;
                            tableRespuesta.AddCell(c1);

                            Contador = Contador + 3;
                            c1 = new PdfPCell(new Phrase(item.Respuesta_Observaciones, fntTitulo2));
                            c1.Colspan = 3;
                            tableRespuesta.AddCell(c1);
                            doc.Add(tableRespuesta);
                        }
                    }

                    doc.Add(Enter);

                }
                doc.Close();
                writer.Close();

                model.NombreDocumentoNoFirmado = strNombre;
                db_Survey.Entry(model).State = EntityState.Modified;
                db_Survey.SaveChanges();

                jsonRespuesta.Ubicacion = strNombre;
                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = "Ok";
            }
            catch (Exception ex)
            {
                doc.Close();
                writer.Close();
                jsonRespuesta.Ubicacion = null;
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = ex.Message;
            }
            return jsonRespuesta;
        }
        public JsonResult GenerarConstancia(Tbl_EncResp_Encuesta model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            Tbl_EncResp_Encuesta tbl_EncResp_Encuesta = db_Survey.Tbl_EncResp_Encuesta.Find(model.id_encuesta,model.swcreatedby,model.id_Correlativo);

            if(tbl_EncResp_Encuesta== null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Mensaje = "No se encontró la encuesta",
                    Result = 0,
                };
            }
            else
            {
                jsonRespuesta = GenerarConstancia_PDF(tbl_EncResp_Encuesta);
            }



            return Json(jsonRespuesta);
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


        public JsonResult JsonProcesarFirmaElectronica(Tbl_EncResp_Encuesta model, string UsuarioFE, string PasswordFE)
        {
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;

            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";

            Tbl_EncResp_Encuesta tbl_EncResp_Encuesta = db_Survey.Tbl_EncResp_Encuesta.Find(model.id_encuesta, model.swcreatedby, model.id_Correlativo);

            if(tbl_EncResp_Encuesta == null)
            {
                return Json(null);
            }

            //string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";
            string rootpdf = rootpath + tbl_EncResp_Encuesta.NombreDocumentoNoFirmado;

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";


            /// bbarillas
            rootpdf = rootpath + tbl_EncResp_Encuesta.NombreDocumentoNoFirmado;
            /// bbarillas

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

            strBearer = GetBearer();

            //            strBearer = GetBearerNeftafiufiu();
            if (strBearer.Length < 125)
            {
                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                return Json(jsonResultUsr);

            }

            string strDocumentoSubido = CallCORS(strBearer, rootpdf);

            if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
            {

                intRespuesta = 0;

                strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                return Json(jsonResultUsr);


            }

            RequestUtil requestUtil = new RequestUtil();

            string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);

            if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
            {

                // se cambio de 0 a 1 para pruebas de emananuel

                intRespuesta = 1;

                strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                return Json(jsonResultUsr);


            }


            if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
            {
                tbl_EncResp_Encuesta.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                db_Survey.Entry(tbl_EncResp_Encuesta).State = EntityState.Modified;
                db_Survey.SaveChanges();

            }

            intRespuesta = 1;

            jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + intRespuesta + "\","
                                  + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";
            return Json(jsonResultUsr);


        }


        //En este método solo es obligatorio enviar la Leyenda, que viene siendo el texto necesario para hacer toda la resolución del documento
        private void LlenaBanner(String Leyenda, string Color = "", int alineacionhorizontal = Element.ALIGN_LEFT, int alineacionvertical = Element.ALIGN_MIDDLE, int borde = Rectangle.NO_BORDER, string estilotexto = "Normal", int tamaniotexto = 10)
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
            else if (Color == "Gris")
            {
                c1.BackgroundColor = Gris;
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

        private void LlenaDatosDeLaMotosierra(Tbl_RNF_Motosierra tbl_RNF_motosierra)
        {
            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();


            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Marca: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.Marca, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Modelo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.Modelo, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);
            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Cilindraje: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.Cilindraje, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Potencia: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.Potencia, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Serie de la motosierra: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            if ((tbl_RNF_motosierra.EmpresaEmisoraFactura == null) || (tbl_RNF_motosierra.EmpresaEmisoraFactura == ""))
            {
                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.No_SerieMotosierra, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.No_SerieMotosierra, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Empresa emisora de la factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.EmpresaEmisoraFactura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            //************************************************************************************************************************************

            if (tbl_RNF_motosierra.No_Factura != "")
            {
                c1 = new PdfPCell(new Phrase("Serie de la factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.No_SerieFactura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Número de factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.No_Factura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            //************************************************************************************************************************************

            if (tbl_RNF_motosierra.OtroDocumentoRespaldo != "")
            {
                c1 = new PdfPCell(new Phrase("Otro documento de respaldo", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_motosierra.OtroDocumentoRespaldo, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            return;
        }


        private void LlenaDatosProfesionalPersonales(Tbl_RNF_Registro tbl_RNF_Registro)
        {

            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();


            if (tbl_RNF_TecnicoProfesional != null)
            {

                PdfPCell c1 = new PdfPCell();

                tableDatosNotificacion = new PdfPTable(4);

                Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


                //******************************************

                c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);
                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha de nacimiento: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(((DateTime)tbl_RNF_TecnicoProfesional.Fecha_Nacimiento).ToString("dd/MM/yyyy"), fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Nombres, fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Apellidos, fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.No_Documento, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("No. de NIT: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.No_NIT, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

            }



            return;
        }

        private void LlenaDatosProfesionalDireccion(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();

            if (tbl_RNF_TecnicoProfesional != null)
            {

            }

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            GlobalUtils globalUtils = new GlobalUtils();

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_RNF_TecnicoProfesional.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_RNF_TecnicoProfesional.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Correo, fntTituloTabla));
            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono celular: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono oficina: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Telefono_Oficina, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }



        private void LlenaDatosProfesional(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            PdfPCell c1 = new PdfPCell();
            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).First();

            tableDatosGenerales = new PdfPTable(4);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            #region  Grado académico

            c1 = new PdfPCell(new Phrase("Categoría:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Tecnico == true)
            {
                c1 = new PdfPCell(new Phrase("Técnico", fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Profesional", fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }
            #endregion

            #region Universidad
            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == true)
            {

                c1 = new PdfPCell(new Phrase("Profesión", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                if (tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion.ToUpper().Contains("OTRA") == true)
                {
                    c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.UniversidadEspecificarCarrera, fntTitulo2));
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }
                else
                {
                    c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion, fntTitulo2));
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }

            }
            else
            {
                c1 = new PdfPCell(new Phrase("Profesión", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion, fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }

            #endregion


            #region Universidad
            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == true)
            {
                c1 = new PdfPCell(new Phrase("Universidad", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.Universidad, fntTitulo2));
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase("No. de colegiado", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.No_Colegiado, fntTitulo2));
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                if (tbl_RNF_TecnicoProfesional.PostGradoMateriaForestal == true)
                {
                    c1 = new PdfPCell(new Phrase("Universidad de post-grado", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.PostGradoUniversidad, fntTitulo2));
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Post-grado obtenido", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.PostGradoEspecialidad, fntTitulo2));
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                }

            }

            #endregion

            #region Titulo INAB
            if ((tbl_RNF_Registro.Sub_Categoria_id == 3) || (tbl_RNF_Registro.Sub_Categoria_id == 4))
            {

                c1 = new PdfPCell(new Phrase("Código de aprobación del curso emitido por INAB", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_TecnicoProfesional.No_De_Diploma, fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);



            }
            #endregion


        }


        private void LlenaDatosEmpresaEntidadActividad(Document doc, Tbl_RNF_Registro tbl_RNF_Registro)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_RNF_Empresa_Entidad_Actividad> tbl_RNF_Empresa_Entidad_Actividads = db.Tbl_RNF_Empresa_Entidad_Actividad.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).OrderBy(Obj => Obj.Actividad_id).ToList();

            if (tbl_RNF_Empresa_Entidad_Actividads.Count() > 0)
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

                foreach (var item in tbl_RNF_Empresa_Entidad_Actividads)
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
        private void LlenaDatosEmpresaEntidadMateriaPrima(Document doc, Tbl_RNF_Registro tbl_RNF_Registro)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_RNF_Empresa_Entidad_Materia_Prima> tbl_RNF_Empresa_Entidad_Materia_Primas = db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).OrderBy(Obj => Obj.Materia_Prima_id).ToList();
            if (tbl_RNF_Empresa_Entidad_Materia_Primas.Count() > 0)
            {
                if ((tbl_RNF_Registro.Sub_Categoria_id == 3) || (tbl_RNF_Registro.Sub_Categoria_id == 6))
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

                foreach (var item in tbl_RNF_Empresa_Entidad_Materia_Primas)
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
        private void LlenaDatosEmpresaEntidadMaquinariaUtilizada(Document doc, Tbl_RNF_Registro tbl_RNF_Registro)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada> tbl_RNF_Empresa_Entidad_Maquinaria_Utilizadas = db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).OrderBy(Obj => Obj.Maquinaria_Utilizada_id).ToList();
            if (tbl_RNF_Empresa_Entidad_Maquinaria_Utilizadas.Count() > 0)
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

                foreach (var item in tbl_RNF_Empresa_Entidad_Maquinaria_Utilizadas)
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
        private void LlenaDatosEmpresaEntidadViveroForestal(Document doc, Tbl_RNF_Registro tbl_RNF_Registro)
        {
            tablePersoneria = new PdfPTable(8);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_RNF_Empresa_Entidad_Vivero_Forestal> tbl_RNF_Empresa_Entidad_Vivero_Forestals = db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).OrderBy(Obj => Obj.Vivero_Forestal_id).ToList();
            if (tbl_RNF_Empresa_Entidad_Vivero_Forestals.Count() > 0)
            {
                LlenaBanner($"PRINCIPALES ESPECIES EN PRODUCCIÓN");
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

                c1 = new PdfPCell(new Phrase($"(Finca, Municipio, Departamento, País)", fntTituloTabla));
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
                foreach (var item in tbl_RNF_Empresa_Entidad_Vivero_Forestals)
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

                    c1 = new PdfPCell(new Phrase($"{item.Codigo_RNF}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Numero_Nota_Control_Semilla_Certificada}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

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
        private void LlenaDatosEmpresaEntidadMotosierraMarcaModelo(Document doc, Tbl_RNF_Registro tbl_RNF_Registro)
        {
            tablePersoneria = new PdfPTable(3);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_RNF_Motosierra_Marca_Modelo> tbl_RNF_Motosierra_Marca_Modelos = db.Tbl_RNF_Motosierra_Marca_Modelo.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).OrderBy(Obj => Obj.Motosierra_id).ToList();
            if (tbl_RNF_Motosierra_Marca_Modelos.Count() > 0)
            {
                LlenaBanner($"MARCAS Y MODELOS DE LAS MOTOSIERRAS");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"Marca", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Modelo", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_RNF_Motosierra_Marca_Modelos)
                {

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


        private void LlenaDatosNotificacionDetallada(Tbl_RNF_Registro tbl_RNF_Registro)
        {

            if (tbl_RNF_Registro.UsuarioExterno_id != null)
            {
                Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_RNF_Registro.UsuarioExterno_id);


                PdfPCell c1 = new PdfPCell();

                tableDatosNotificacion = new PdfPTable(4);

                Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);




                //************************************************************************************************************************************

                //************************************************************************************************************************************

                //************************************************************************************************************************************
                c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);


                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Direccion, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Teléfono: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosNotificacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tableDatosNotificacion.AddCell(c1);



            }



            return;
        }
        private void LlenaDatosFinca(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Nombre de la finca:", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
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
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 11;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Registro de la Propiedad:", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.Rowspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegNumero}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Libro", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegLibro}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Folio", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegFolio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 7;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Dirección de la Finca", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Ubicacion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Municipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }
        private void LlenaDatosDeLaFinca(Tbl_RNF_Finca tbl_RNF_finca)
        {
            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();


            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombre de la finca: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_finca.NombreFinca, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Acreditada por medio de: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_finca.Tbl_Sol_FincaConstanciaDePropiedad.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Ubicación : ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_finca.Ubicacion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_finca.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_finca.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);




            if (tbl_RNF_finca.OtrosRegistrosRNF == true || (tbl_RNF_finca.Registros != null && tbl_RNF_finca.Registros != ""))
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Si", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Registro", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_RNF_finca.Registros, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("No", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

            }


            //Datos que agrega Nefta
            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_RNF_finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);


            var tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_finca.No_Registro && Obj.Finca_id == tbl_RNF_finca.Finca_Id).ToList();
            var sumAreaTotal = tbl_RNF_Rodal.Select(c => c.AreaTotalCalculadaSistema).Sum();

            var tbl_RNF_RodalDescuento = db.Tbl_RNF_Rodal_Descuento.Where(Obj => Obj.No_Registro == tbl_RNF_finca.No_Registro && Obj.Finca_id == tbl_RNF_finca.Finca_Id).ToList();
            var sumAreaTotalDescuento = tbl_RNF_RodalDescuento.Select(c => c.AreaTotalCalculadaSistema).Sum();


            c1 = new PdfPCell(new Phrase($"{tbl_RNF_finca.AreaARegistrar}", fntTituloTabla));
            //c1 = new PdfPCell(new Phrase($"{sumAreaTotal - sumAreaTotalDescuento}", fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            //************************************************************************************************************************************
            //************************************************************Datos de fuentes semilleras ******************************************** ///



            //************************************************************Datos de fuentes semilleras ******************************************** ///

            return;
        }


        private void EstimacionVolumen(Tbl_RNF_Finca tbl_RNF_Finca, string Modalidad)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).FirstOrDefault();

            List<fc_RNF_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_RNF_Sel_Rodal_ValidacionesDiametrica(tbl_RNF_Finca.No_Registro, tbl_RNF_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_RNF_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_RNF_Finca.No_Registro, tbl_RNF_Finca.Finca_Id).FirstOrDefault();



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
                            string noregistro;
                            long solicitudid, fincaid, rodalidd;
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                if (tbl_RNF_Registro.Categoria_id == 4)
                                {
                                    c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase("Modalidad : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }
                                intModalidadImpresa = intModalidadImpresa + 1;

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
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            // c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"ESPECIES A REGISTRAR", fntTituloTabla));
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
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca.Finca_Id}", fntTituloTabla));
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
                        c1.Colspan = 2;
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
                                c1.Colspan = 2;
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
                            c1.Colspan = 2;
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
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {
                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);


                                if (tbl_RNF_Registro.Categoria_id == 4)
                                {
                                    c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase("Modalidad : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }

                                intModalidadImpresa = intModalidadImpresa + 1;

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
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 11;
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
                            c1.Colspan = 3;
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

                            c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca.Finca_Id}", fntTituloTabla));
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
                        c1.Colspan = 3;
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
                                c1.Colspan = 3;
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
                            c1.Colspan = 3;
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


        private void EstimacionVolumenFS(Tbl_RNF_Finca tbl_RNF_Finca, string Modalidad)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).FirstOrDefault();

            List<fc_RNF_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_RNF_Sel_Rodal_ValidacionesDiametrica(tbl_RNF_Finca.No_Registro, tbl_RNF_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.Clase, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_RNF_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_RNF_Finca.No_Registro, tbl_RNF_Finca.Finca_Id).FirstOrDefault();



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
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 5;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 1;

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
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
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
                            c1 = new PdfPCell(new Phrase($"Clase", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca.Finca_Id}", fntTituloTabla));
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

                        c1 = new PdfPCell(new Phrase(((int)validacionesDiametrica[i].Clase).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
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
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 5;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 1;

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
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);



                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_RNF_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
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

                            c1 = new PdfPCell(new Phrase($"{tbl_RNF_Finca.Finca_Id}", fntTituloTabla));
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


        private void ResumenPV(long solicitud_id)
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
            sqlQuery += " From dbo.[fc_Sol_Sel_Rodal_ValidacionesDiametrica_PV]('" + solicitud_id + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

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

        private void ResumenPV_FS(string No_Registro)
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

            sqlQuery = " Select No_Registro, Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_RNF_Sel_Rodal_ValidacionesDiametrica_PV]('" + No_Registro + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV_RNF> Resultado = new List<ClassResumenPV_RNF> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV_RNF>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Resumen de la fuente semillera", fntSubTitulo));
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


        private void LlenaDatosDeLaEmpresa(Tbl_RNF_Registro tbl_RNF_Registro, fc_RNF_Sel_Direccion_Result DireccionSolicitud)
        {
            Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();
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

            if (tbl_RNF_Empresa_Entidad != null)
            {
                if ((tbl_RNF_Empresa_Entidad.DireccionEmpresaMovil != null) && (tbl_RNF_Empresa_Entidad.DireccionEmpresaMovil.Trim() != ""))
                {
                    DireccionMovil = tbl_RNF_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";
                    DireccionMovil += tbl_RNF_Empresa_Entidad.Tbl_Gral_Municipio1.Municipio + ", " + tbl_RNF_Empresa_Entidad.Tbl_Gral_Departamento1.Departamento;
                }

            }



            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            if (tbl_RNF_Empresa_Entidad != null)
            {
                Tbl_RNF_Empresa_Entidad_Tipo_Registro tbl_RNF_Empresa_Entidad_Tipo_Registro = db.Tbl_RNF_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).FirstOrDefault();

                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Nombre Comercial: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad.Nombre}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                if (tbl_RNF_Empresa_Entidad.Objeto_Empresa != null && tbl_RNF_Empresa_Entidad.Objeto_Empresa.Trim() != "")
                {

                    c1 = new PdfPCell(new Phrase("Objeto de la Empresa: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad.Objeto_Empresa}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                c1 = new PdfPCell(new Phrase("Número de NIT: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad.No_NIT}", fntTituloTabla));
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

                if (tbl_RNF_Registro.Tbl_RNF_Empresa_Entidad.Tipo_Industria_id == 2)
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


                if (tbl_RNF_Empresa_Entidad.Tbl_Gral_Tipo_Industria != null)
                {

                    c1 = new PdfPCell(new Phrase("Tipo de Industria: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                if (tbl_RNF_Empresa_Entidad_Tipo_Registro != null)
                {
                    string fechaacta;
                    //1   Registro Mercantil
                    //2   REPEJU
                    //3   INACOP
                    //4   Comunal
                    //5   Municipal

                    // 1 Registro mercantil
                    if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 1)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);



                        c1 = new PdfPCell(new Phrase("Número de folio: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Folio, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de libro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Libro, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                    }

                    // 2 REPEJU
                    if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 2)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de partida: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);



                        c1 = new PdfPCell(new Phrase("Número de folio: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Folio, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de libro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Libro, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Tipo de REPEJU: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_REPEJU_De.Descripcion, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                    }

                    // 3 INACOP
                    if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 3)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de partida: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                    }

                    // 4 Comunal
                    if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 4)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Acta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Fecha del acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);


                        if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Fecha_Acta != null)
                        {
                            DateTime Fecha_Acta = (DateTime)tbl_RNF_Empresa_Entidad_Tipo_Registro.Fecha_Acta;
                            fechaacta = Fecha_Acta.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            fechaacta = "Debe indicar la fecha del acta";
                        }

                        c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }

                    // 5 Municipal
                    if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 5)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Acta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Fecha del acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);


                        if (tbl_RNF_Empresa_Entidad_Tipo_Registro.Fecha_Acta != null)
                        {
                            DateTime Fecha_Acta = (DateTime)tbl_RNF_Empresa_Entidad_Tipo_Registro.Fecha_Acta;
                            fechaacta = Fecha_Acta.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            fechaacta = "Debe indicar la fecha del acta";
                        }

                        c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }


                    c1 = new PdfPCell(new Phrase("Empresa Forestal a Registrar: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Registro.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);
                }


                if (tbl_RNF_Registro.Sub_Categoria_id == 3)
                {

                    c1 = new PdfPCell(new Phrase("No. de Registro de Empresa Forestal Vinculada : ", fntTituloTabla));
                    c1.Colspan = 2;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_RNF_Empresa_Entidad.RNF_Inscripcion_Vinculada}", fntTituloTabla));
                    c1.Colspan = 2;
                    tablePersoneria.AddCell(c1);

                }




                c1 = new PdfPCell(new Phrase("Coordenadas GTM X: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_RNF_Empresa_Entidad.GTMX ?? 0).ToString("0"), fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas GTM Y: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_RNF_Empresa_Entidad.GTMY ?? 0).ToString("0"), fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

            }

        }


    }
}
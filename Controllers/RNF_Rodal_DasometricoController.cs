using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Rodal_DasometricoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        public class Especie_Formula
        {
            public string Especie_id { get; set; }
            public string Formula_Volumen_MetrosCubicos { get; set; }
        }

        // GET: RNF_Rodal_Dasometrico
        public ActionResult Index(string Guid_id)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            ViewBag.Guid_id = tbl_RNF_Registro.No_Registro;

            string sqlQuery;


            sqlQuery = " Select Especie_id, Formula_Volumen_MetrosCubicos";
            sqlQuery += " From Tbl_RNF_Rodal_Dasometrico";
            sqlQuery += " Where No_Registro = '" + tbl_RNF_Registro.No_Registro + "'";
            sqlQuery += " Group by Especie_id, Formula_Volumen_MetrosCubicos ";

            List<Especie_Formula> Resultado = new List<Especie_Formula> { };

            Resultado = db.Database.SqlQuery<Especie_Formula>(sqlQuery).ToList();

            ViewBag.FormulasUtilizadas = Resultado;

            var tbl_RNF_rodal_dasometrico = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.Guid_id);
            return View(tbl_RNF_rodal_dasometrico.ToList());
        }

        public ActionResult DatosDasometricosCargados(string Guid_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            string sqlQuery;

            sqlQuery = " Select Especie_id, Formula_Volumen_MetrosCubicos";
            sqlQuery += " From Tbl_RNF_Rodal_Dasometrico";
            sqlQuery += " Where No_Registro = '" + tbl_RNF_Registro.No_Registro.ToString() + "'";
            sqlQuery += " Group by Especie_id, Formula_Volumen_MetrosCubicos ";

            List<Especie_Formula> Resultado = new List<Especie_Formula> { };

            Resultado = db.Database.SqlQuery<Especie_Formula>(sqlQuery).ToList();

            ViewBag.FormulasUtilizadas = Resultado;

            var tbl_RNF_rodal_dasometrico = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro);
            return View(tbl_RNF_rodal_dasometrico.ToList());
        }



        class JsonRespuesta
        {
            public int CodRespuesta { get; set; }
            public string StrUbicacion { get; set; }
            public string StrMensaje { get; set; }
        }
        string Generar_Excel(string No_Registro)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            long Id = tbl_RNF_Registro.Solicitud_id;


            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            string strNombre, strDirArchivo;
            string rootbase, rootpath, rootdest, rootnew, destfinal, nombrereporte, destfile, destxlsx, tipodeareadescripcion;
            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Content/Machotes/";
            rootnew = $"{rootbase}Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootpath}";
            nombrereporte = $"DasometricosCargados_{No_Registro}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";
            List<Especie_Formula> FormulasUtilizadas = (from d in db.Tbl_RNF_Rodal_Dasometrico
                                                        where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                        group d by new { d.Especie_Id, d.Formula_Volumen_MetrosCubicos }
                                                        into grp
                                                        select new Especie_Formula
                                                        {
                                                            Especie_id = grp.Key.Especie_Id,
                                                            Formula_Volumen_MetrosCubicos = grp.Key.Formula_Volumen_MetrosCubicos
                                                        }).ToList();

            List<Tbl_RNF_Rodal_Dasometrico> tbl_RNF_Rodal_Dasometricos = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();

            tbl_RNF_Rodal_Dasometricos = (from d in tbl_RNF_Rodal_Dasometricos
                                          orderby d.Finca_id, d.Tipo_de_Area, d.No_Parcela, d.Dasometrico_id
                                          select d).ToList();


            strNombre = $"..//..//Archivos_Generados_Que_Pueden_Borrar//{nombrereporte}.xlsx";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            Color ColorVerdeBosque = Color.ForestGreen;
            Color ColorBlanco = Color.White;


            ExcelPackage oEPP = new ExcelPackage();
            oEPP.Workbook.Worksheets.Add("DasometricosCargados");
            ExcelWorksheet wSheet1 = oEPP.Workbook.Worksheets[0];
            iniciofila = finalfila = 1;
            iniciocolumna = finalcolumna = 1;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 4])
            {
                //rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Style.Font.Bold = true;
                rango.Value = "Detalle de datos dasométricos cargados";
                rango.Merge = true;
            }

            iniciofila = finalfila = 3;
            iniciocolumna = finalcolumna = 1;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 4])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Style.Font.Bold = true;
                rango.Value = "Fórmula utilizada para cálculo de volúmen";
                rango.Merge = true;
            }


            iniciofila = finalfila = 4;
            iniciocolumna = finalcolumna = 1;
            foreach (var item in FormulasUtilizadas)
            {

                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = item.Especie_id;
                }

                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna + 1, finalfila, finalcolumna + 4])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = item.Formula_Volumen_MetrosCubicos;
                    rango.Merge = true;
                }
                iniciofila++;
                iniciofila = finalfila = iniciofila;

            }

            iniciofila++;
            iniciofila = finalfila = iniciofila;

            iniciocolumna = finalcolumna = 1;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Finca";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Rodal/Línea";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Tipo de Area";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Area Efectiva";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "No. Parcela";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Area Muestreada";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "No. Arbol";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }


            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Especie";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Año de Plantación";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "DAP (cm)";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Altura (m)";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciocolumna++;
            iniciocolumna = finalcolumna = iniciocolumna;
            using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                rango.Value = "Volumen (m3)";
                rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
            }

            iniciofila++;
            iniciofila = finalfila = iniciofila;

            foreach (var item in tbl_RNF_Rodal_Dasometricos)
            {

                iniciocolumna = 1;
                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Finca_id}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Rodal_id}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Tbl_Sol_Rodal_Tipo.Descripcion}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{(item.Area_Efectiva_Rodal ?? 0).ToString("0.00")}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.No_Parcela}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Area_Muestreada}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Dasometrico_id}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Tbl_Gral_Especie.NombreCientifico}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Anio_Establecimiento}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.DAP_Promedio}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{item.Altura_Promedio}";
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = $"{Math.Abs((decimal)(item.Volumen_MetrosCubicos??0))}";
                }

                iniciofila++;

            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            oEPP.SaveAs(new FileInfo($"{destxlsx}"));



            return strNombre;
        }

        public JsonResult Exportar_Excel(string No_Registro)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrUbicacion = null;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna gestión";

            try
            {
                jsonRespuesta.StrUbicacion = Generar_Excel(No_Registro);
                if (jsonRespuesta.StrUbicacion != null)
                {

                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Se ha generado el documento exitosamente";

                }
                else
                {

                    jsonRespuesta.CodRespuesta = 2;
                    jsonRespuesta.StrMensaje = "No se pudo generar el documento";

                }

            }
            catch (Exception ex)
            {

                jsonRespuesta.CodRespuesta = 3;
                jsonRespuesta.StrUbicacion = null;
                jsonRespuesta.StrMensaje = "Ocurrió un error " + ex.ToString();

            }


            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

    }
}
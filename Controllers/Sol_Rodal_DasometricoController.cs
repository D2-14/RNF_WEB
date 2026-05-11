using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_Rodal_DasometricoController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        public class Especie_Formula
        {
            public string Especie_id { get; set; }
            public string Formula_Volumen_MetrosCubicos { get; set; }
        }

        // GET: Sol_Rodal_Dasometrico
        public ActionResult Index(long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            ViewBag.Solicitudid = tbl_sol_solicitud.Solicitud_id;



            ViewBag.Guid = tbl_sol_solicitud.Guid_id;
            
            long Id = tbl_sol_solicitud.Solicitud_id;

            string sqlQuery;


            sqlQuery = " Select Especie_id, Formula_Volumen_MetrosCubicos";
            sqlQuery += " From Tbl_Sol_Rodal_Dasometrico";
            sqlQuery += " Where Solicitud_id = " + Id.ToString();
            sqlQuery += " Group by Especie_id, Formula_Volumen_MetrosCubicos ";

            List<Especie_Formula> Resultado = new List<Especie_Formula> { };
            
            Resultado = db.Database.SqlQuery<Especie_Formula>(sqlQuery).ToList();

            ViewBag.FormulasUtilizadas = Resultado;

            var tbl_sol_rodal_dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == Id);
            return View(tbl_sol_rodal_dasometrico.ToList());

            

        }


        // GET: Sol_Rodal_Dasometrico
        public ActionResult DatosDasometricosCargados(long Solicitud_id, string firma)
        {

            ViewBag.Guid = firma;
            ViewBag.Solicitud_id = Solicitud_id;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            long Id = tbl_Sol_Solicitud.Solicitud_id;

            string sqlQuery;

            sqlQuery = " Select Especie_id, Formula_Volumen_MetrosCubicos";
            sqlQuery += " From Tbl_Sol_Rodal_Dasometrico";
            sqlQuery += " Where Solicitud_id = " + Id.ToString();
            sqlQuery += " Group by Especie_id, Formula_Volumen_MetrosCubicos ";

            List<Especie_Formula> Resultado = new List<Especie_Formula> { };

            Resultado = db.Database.SqlQuery<Especie_Formula>(sqlQuery).ToList();

            ViewBag.FormulasUtilizadas = Resultado;

            var tbl_sol_rodal_dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == Id);
            tbl_sol_rodal_dasometrico = (from d in tbl_sol_rodal_dasometrico
                                         orderby d.Solicitud_id, d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela
                                         select d);

            return View(tbl_sol_rodal_dasometrico.ToList());
        }




        // GET: Sol_Rodal_Dasometrico/Create
        public ActionResult Create(long solicitud_id, string firma)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            Tbl_Sol_Rodal_Dasometrico sol_rodal_dasometrico = new Tbl_Sol_Rodal_Dasometrico();

            sol_rodal_dasometrico.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            int intCategoria = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoria == 1)
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie.Where(Obj=>Obj.ArbolFrutal==true), "Especie_Id", "NombreCientifico");
            }
            else
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico");
            }



            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj=> Obj.Solicitud_id== tbl_sol_solicitud.Solicitud_id), "Rodal_Id", "Rodal_Id");
            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario, "EstadoFitosanitario_id", "Descripcion");

            sol_rodal_dasometrico.swcreatedby = objUs.intUsuario_id;
            sol_rodal_dasometrico.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                sol_rodal_dasometrico.swcreatedbyinterno = false;

            }
            else
            {
                sol_rodal_dasometrico.swcreatedbyinterno = true;
                
            }

            ViewBag.Perimetro_Dimensional = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_PerimetroDimensional, "Perimetro_Dimensional", "Descripcion", sol_rodal_dasometrico.Perimetro_Dimensional);

            sol_rodal_dasometrico.Densidad_ha = 0;
            sol_rodal_dasometrico.Altura_Promedio = 0;
            sol_rodal_dasometrico.Volumen = 0;
            sol_rodal_dasometrico.DAP_Promedio = 0;

            sol_rodal_dasometrico.Clase_I_DAP_Promedio = 0;
            sol_rodal_dasometrico.Clase_I_Altura_Promedio = 0;
            sol_rodal_dasometrico.Clase_I_Densidad_Promedio = 0;

            sol_rodal_dasometrico.Clase_II_DAP_Promedio = 0;
            sol_rodal_dasometrico.Clase_II_Altura_Promedio = 0;
            sol_rodal_dasometrico.Clase_II_Densidad_Promedio = 0;

            sol_rodal_dasometrico.Clase_III_DAP_Promedio = 0;
            sol_rodal_dasometrico.Clase_III_Altura_Promedio = 0;
            sol_rodal_dasometrico.Clase_III_Densidad_Promedio = 0;

            return View(sol_rodal_dasometrico);

        }

        [HttpPost]
        public JsonResult GetTipoArea(int rodal_id)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            long lnSolicitud_ID = Convert.ToInt64(Session[Constants.session_Solicitud]);

            Tbl_Sol_Rodal tbl_Sol_Rodal = (from c in db.Tbl_Sol_Rodal
                                                         where c.Solicitud_id == lnSolicitud_ID
                                                         &&  c.Rodal_Id == rodal_id
                                                         select c).First();


            return Json(tbl_Sol_Rodal.Tbl_Sol_Rodal_Tipo.Descripcion);

        }

        // POST: Sol_Rodal_Dasometrico/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Rodal_Dasometrico tbl_Sol_Rodal_Dasometrico)
        {

            if (tbl_Sol_Rodal_Dasometrico.Densidad_ha == null)
            {
                tbl_Sol_Rodal_Dasometrico.Densidad_ha = 0;
            }

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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Rodal_Dasometrico.Solicitud_id, "Sol_Rodal_Dasometricos", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos de rodales.";
                return RedirectToAction("../Home/RegistroAgregado");
            }


            if (ModelState.IsValid)
            {
                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Sol_Rodal_Dasometrico.Where(RodalDasometrico => RodalDasometrico.Solicitud_id == tbl_Sol_Rodal_Dasometrico.Solicitud_id && RodalDasometrico.Rodal_id == tbl_Sol_Rodal_Dasometrico.Rodal_id).Max(u => u.Dasometrico_id);
                    lngIdt++;
                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Sol_Rodal_Dasometrico.Dasometrico_id = lngIdt;

                db.Tbl_Sol_Rodal_Dasometrico.Add(tbl_Sol_Rodal_Dasometrico);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            int intCategoria = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoria == 1)
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie.Where(Obj => Obj.ArbolFrutal == true), "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }
            else
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }

            ViewBag.Perimetro_Dimensional = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_PerimetroDimensional, "Perimetro_Dimensional", "Descripcion", tbl_Sol_Rodal_Dasometrico.Perimetro_Dimensional);

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal, "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Dasometrico.Rodal_id);
            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario, "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Dasometrico.EstadoFitosanitario_id);

            return View(tbl_Sol_Rodal_Dasometrico);

        }

        public ActionResult Edit(int rodal_id, int dasometrico_id, long solicitud_id, string firma)
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


            long varSolicitudid = solicitud_id;

            Tbl_Sol_Rodal_Dasometrico tbl_Sol_Rodal_Dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Rodal_id == rodal_id && Obj.Dasometrico_id == dasometrico_id).First();

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            int intCategoria = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoria == 1)
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie.Where(Obj => Obj.ArbolFrutal == true), "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }
            else
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }
            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj=> Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Rodal_Id == tbl_Sol_Rodal_Dasometrico.Rodal_id), "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Dasometrico.Rodal_id);
            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario, "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Dasometrico.EstadoFitosanitario_id);
            ViewBag.Perimetro_Dimensional = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_PerimetroDimensional, "Perimetro_Dimensional", "Descripcion", tbl_Sol_Rodal_Dasometrico.Perimetro_Dimensional);

            return View(tbl_Sol_Rodal_Dasometrico);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_Sol_Rodal_Dasometrico tbl_Sol_Rodal_Dasometrico)
        {

            if (tbl_Sol_Rodal_Dasometrico.Densidad_ha == null)
            {
                tbl_Sol_Rodal_Dasometrico.Densidad_ha = 0;
            }

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Rodal_Dasometrico.Solicitud_id, "Sol_Rodal_Dasometricos", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos de rodales.";
                return RedirectToAction("../Home/RegistroActualizado");
            }


            if (ModelState.IsValid)
            {
                db.Entry(tbl_Sol_Rodal_Dasometrico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            int intCategoria = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoria == 1)
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie.Where(Obj => Obj.ArbolFrutal == true), "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }
            else
            {
                ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_Sol_Rodal_Dasometrico.Especie_Id);
            }
            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal, "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Dasometrico.Rodal_id);
            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario, "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Dasometrico.EstadoFitosanitario_id);
            ViewBag.Perimetro_Dimensional = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_PerimetroDimensional, "Perimetro_Dimensional", "Descripcion", tbl_Sol_Rodal_Dasometrico.Perimetro_Dimensional);

            return View(tbl_Sol_Rodal_Dasometrico);

        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        class JsonRespuesta
        {
            public int CodRespuesta { get; set; }
            public string StrUbicacion { get; set; }
            public string StrMensaje { get; set; }
        }
        string Generar_Excel(long Solicitud_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);
            long Id = tbl_Sol_Solicitud.Solicitud_id;


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
            nombrereporte = $"DasometricosCargados_{Id}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{rootnew}{nombrereporte}";
            destxlsx = $"{destfinal}.xlsx";
            List<Especie_Formula> FormulasUtilizadas = (from d in db.Tbl_Sol_Rodal_Dasometrico
                                                        where d.Solicitud_id == Id
                                                        group d by new { d.Especie_Id, d.Formula_Volumen_MetrosCubicos }
                                                        into grp
                                                        select new Especie_Formula
                                                        {
                                                            Especie_id = grp.Key.Especie_Id,
                                                            Formula_Volumen_MetrosCubicos = grp.Key.Formula_Volumen_MetrosCubicos
                                                        }).ToList();
            List<Tbl_Sol_Rodal_Dasometrico> tbl_sol_rodal_dasometrico = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == Id).ToList();
            tbl_sol_rodal_dasometrico = (from d in tbl_sol_rodal_dasometrico
                                         orderby d.Solicitud_id, d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.No_Parcela, d.Dasometrico_id
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

            if (tbl_Sol_Solicitud.Categoria_id == 6)
            {
                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = "Clase";
                    rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
                }
                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = "Estado Fitosanitario";
                    rango.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rango.Style.Fill.BackgroundColor.SetColor(ColorVerdeBosque);
                }
            }

            iniciofila++;
            iniciofila = finalfila = iniciofila;

            foreach (var item in tbl_sol_rodal_dasometrico)
            {

                iniciocolumna = 1;
                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{item.Finca_id}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{item.Rodal_id}");
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
                    rango.Value = Decimal.Parse($"{(item.Area_Efectiva_Rodal ?? 0).ToString("0.00")}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{item.No_Parcela}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{item.Area_Muestreada ?? 0}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Int32.Parse($"{item.Dasometrico_id}");
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
                    rango.Value = Int32.Parse($"{item.Anio_Establecimiento}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{item.DAP_Promedio}");
                }

                iniciocolumna++;
                iniciocolumna = finalcolumna = iniciocolumna;
                using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rango.Value = Decimal.Parse($"{item.Altura_Promedio}");
                }

                if (tbl_Sol_Solicitud.Categoria_id == 6)
                {
                    iniciocolumna++;
                    iniciocolumna = finalcolumna = iniciocolumna;
                    using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rango.Value = (item.Clase_id).ToString();
                    }
                    iniciocolumna++;
                    iniciocolumna = finalcolumna = iniciocolumna;
                    using (ExcelRange rango = wSheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        rango.Value = (item.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Descripcion);
                    }

                }

                iniciofila++;

            }

            wSheet1.Protection.IsProtected = false;
            wSheet1.Protection.AllowSelectLockedCells = false;

            oEPP.SaveAs(new FileInfo($"{destxlsx}"));



            return strNombre;
        }


        public JsonResult Exportar_Excel(long Solicitud_id, string Guid)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrUbicacion = null;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna gestión";

            try
            {
                jsonRespuesta.StrUbicacion = Generar_Excel(Solicitud_id);
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

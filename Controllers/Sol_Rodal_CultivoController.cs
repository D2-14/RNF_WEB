using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{


    public class Sol_Rodal_CultivoController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_Rodal_Cultivo
        public ActionResult Index(string Guid)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid).First();

            ViewBag.solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            return View(db.Tbl_Sol_Rodal_Cultivo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList());
        }


        public ActionResult EmptyView()
        {
            return View();
        }


        public class Cultivo
        {
            public int Cultivo_id { get; set; }
            public string Nombres_Comunes { get; set; }
        }


        public ActionResult Create(string Guid)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid).First();
            ViewBag.solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            ViewBag.Categoria = tbl_Sol_Solicitud.Categoria_id;
            ViewBag.SubCategoria = tbl_Sol_Solicitud.Sub_Categoria_id;
            ViewBag.SubSubCategoria = tbl_Sol_Solicitud.Sub_Sub_Categoria_id;

            if ((tbl_Sol_Solicitud.Categoria_id != 3) && (tbl_Sol_Solicitud.Categoria_id != 4))
            {
                return RedirectToAction("../Sol_Rodal_Cultivo/EmptyView");
            }

            ViewBag.Guid = Guid;

            Tbl_Sol_Rodal_Cultivo tbl_Sol_Rodal_Cultivo = new Tbl_Sol_Rodal_Cultivo();

            tbl_Sol_Rodal_Cultivo.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.Finca_id = new SelectList(db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id), "Finca_Id", "Finca_Id");

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == -5), "Rodal_id", "Rodal_id");

            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 100), "Tipo_de_Area", "Descripcion", 1);


            string sqlQuery;

            sqlQuery = " Select Cultivo_id, Nombre_Tecnico + ' - '+Nombres_Comunes Nombres_Comunes";
            sqlQuery += " From Tbl_Gral_Cultivo ";

            if (tbl_Sol_Solicitud.Categoria_id == 3)
            {
                sqlQuery += " Where  Arbol_Frutales = 1 ";
            }

            if ((tbl_Sol_Solicitud.Categoria_id == 4) && (tbl_Sol_Solicitud.Sub_Categoria_id == 1)) // Perenes
            {
                sqlQuery += " Where  Especies_Agricolas_Permanentes = 1 ";
            }

            if ((tbl_Sol_Solicitud.Categoria_id == 4) && (tbl_Sol_Solicitud.Sub_Categoria_id == 2)) // Anuales o bianuales
            {
                sqlQuery += " Where  Especies_Agricolas_Bianuales = 1 ";
            }

            if ((tbl_Sol_Solicitud.Categoria_id == 4) && (tbl_Sol_Solicitud.Sub_Categoria_id == 3)) // Silvopastoril
            {
                sqlQuery += " Where  Cultivo_SilvoPastoril = 1 ";
            }

            if ((tbl_Sol_Solicitud.Categoria_id == 4) && (tbl_Sol_Solicitud.Sub_Categoria_id == 4)) // Huertos
            {
                sqlQuery += " Where  Especies_Mixtas = 1 ";
            }

            List<Cultivo> Resultado = new List<Cultivo> { };

            Resultado = db.Database.SqlQuery<Cultivo>(sqlQuery).ToList();

            ViewBag.Cultivo_id = new SelectList(Resultado, "Cultivo_id", "Nombres_Comunes");

            return View(tbl_Sol_Rodal_Cultivo);

        }

        [HttpPost]
        public JsonResult GetRodal(long solicitud_id, long finca_id)
        {

            IEnumerable<Tbl_Sol_Rodal> RodalesSelected = (from c in db.Tbl_Sol_Rodal
                                                          where c.Solicitud_id == solicitud_id
                                                          && c.Finca_id == finca_id
                                                          && c.Tipo_de_Area != 3
                                                          select c);

            var Rodales = new SelectList(RodalesSelected, "Rodal_id", "Rodal_id");

            return Json(new SelectList(Rodales, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetRodalTipo(long solicitud_id, long finca_id, long rodal_id)
        {

            IEnumerable<Tbl_Sol_Rodal_Tipo> RodalesAreaSelected = (from c in db.Tbl_Sol_Rodal
                                                                   from d in db.Tbl_Sol_Rodal_Tipo
                                                                   where c.Solicitud_id == solicitud_id
                                                                   && c.Finca_id == finca_id
                                                                   && c.Rodal_Id == rodal_id
                                                                   && c.Tipo_de_Area != 3
                                                                   && d.Tipo_de_Area == c.Tipo_de_Area
                                                                   select d);

            var RodalArea = new SelectList(RodalesAreaSelected, "Tipo_de_Area", "Descripcion");

            return Json(new SelectList(RodalArea, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult AgregarCultivo(long solicitud_id, long finca_id, long rodal_id, int area_id, int cultivo_id, int anioestablecimiento, decimal Volumen_Rodal, decimal Volumen_ha)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            int encontrado = db.Tbl_Sol_Rodal_Cultivo.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Finca_id == finca_id && Obj.Rodal_Id == rodal_id && Obj.Tipo_de_Area == area_id && Obj.Cultivo_id == cultivo_id && Obj.Anio_Establecimiento == anioestablecimiento).Count();

            if (anioestablecimiento > DateTime.Now.Year)
            {
                encontrado = 1;
                codRespuesta = 0;
                strRespuesta = "Error: No puede seleccionar una año de plantación superior al año actual";

                jsonResultUsr = "{\"CodRespuesta\":"
                         + "\"" + codRespuesta + "\","
                         + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResultUsr);
            }

            if (encontrado == 0)
            {
                Tbl_Sol_Rodal_Cultivo tbl_Sol_Rodal_Cultivo = new Tbl_Sol_Rodal_Cultivo();

                tbl_Sol_Rodal_Cultivo.Solicitud_id = solicitud_id;
                tbl_Sol_Rodal_Cultivo.Finca_id = finca_id;
                tbl_Sol_Rodal_Cultivo.Rodal_Id = rodal_id;
                tbl_Sol_Rodal_Cultivo.Tipo_de_Area = area_id;
                tbl_Sol_Rodal_Cultivo.Cultivo_id = cultivo_id;
                tbl_Sol_Rodal_Cultivo.Anio_Establecimiento = anioestablecimiento;
                tbl_Sol_Rodal_Cultivo.Volumen_ha = Volumen_ha;
                tbl_Sol_Rodal_Cultivo.Volumen_Rodal = Volumen_Rodal;

                db.Tbl_Sol_Rodal_Cultivo.Add(tbl_Sol_Rodal_Cultivo);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarCultivo(long solicitud_id, long finca_id, long rodal_id, int area_id, int cultivo_id)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal_Cultivo tbl_Sol_Rodal_Cultivo = db.Tbl_Sol_Rodal_Cultivo.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Finca_id == finca_id && Obj.Rodal_Id == rodal_id && Obj.Tipo_de_Area == area_id && Obj.Cultivo_id == cultivo_id).First();

            db.Tbl_Sol_Rodal_Cultivo.Remove(tbl_Sol_Rodal_Cultivo);
            db.SaveChanges();

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }


    }
}
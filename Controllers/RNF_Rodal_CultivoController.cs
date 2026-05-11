using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Rodal_CultivoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Rodal_Cultivo
        public ActionResult Index(string Guid_id)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            return View(db.Tbl_RNF_Rodal_Cultivo.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList());
        }

        public class Cultivo
        {
            public int Cultivo_id { get; set; }
            public string Nombres_Comunes { get; set; }
        }

        public ActionResult Create(string No_Registro)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid).First();

            ViewBag.Categoria = tbl_RNF_Registro.Categoria_id;
            ViewBag.SubCategoria = tbl_RNF_Registro.Sub_Categoria_id;
            ViewBag.SubSubCategoria = tbl_RNF_Registro.Sub_Sub_Categoria_id;

            if ((tbl_RNF_Registro.Categoria_id != 3) && (tbl_RNF_Registro.Categoria_id != 4))
            {
                return RedirectToAction("../Sol_Rodal_Cultivo/EmptyView");
            }

            ViewBag.Guid = No_Registro;

            Tbl_RNF_Rodal_Cultivo tbl_RNF_Rodal_Cultivo = new Tbl_RNF_Rodal_Cultivo();

            tbl_RNF_Rodal_Cultivo.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_Rodal_Cultivo.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_Rodal_Cultivo.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_Rodal_Cultivo.No_Registro = tbl_RNF_Registro.No_Registro;

            ViewBag.Finca_id = new SelectList(db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro), "Finca_Id", "Finca_Id");

            ViewBag.Rodal_id = new SelectList(db.Tbl_RNF_Rodal.Where(Obj => Obj.Solicitud_id == -5), "Rodal_id", "Rodal_id");

            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 100), "Tipo_de_Area", "Descripcion", 1);


            string sqlQuery;

            sqlQuery = " Select Cultivo_id, Nombre_Tecnico + ' - '+Nombres_Comunes Nombres_Comunes";
            sqlQuery += " From Tbl_Gral_Cultivo ";

            if (tbl_RNF_Registro.Categoria_id == 3)
            {
                sqlQuery += " Where  Arbol_Frutales = 1 ";
            }

            if ((tbl_RNF_Registro.Categoria_id == 4) && (tbl_RNF_Registro.Sub_Categoria_id == 1)) // Perenes
            {
                sqlQuery += " Where  Especies_Agricolas_Permanentes = 1 ";
            }

            if ((tbl_RNF_Registro.Categoria_id == 4) && (tbl_RNF_Registro.Sub_Categoria_id == 2)) // Anuales o bianuales
            {
                sqlQuery += " Where  Especies_Agricolas_Bianuales = 1 ";
            }

            if ((tbl_RNF_Registro.Categoria_id == 4) && (tbl_RNF_Registro.Sub_Categoria_id == 3)) // Silvopastoril
            {
                sqlQuery += " Where  Cultivo_SilvoPastoril = 1 ";
            }

            if ((tbl_RNF_Registro.Categoria_id == 4) && (tbl_RNF_Registro.Sub_Categoria_id == 4)) // Huertos
            {
                sqlQuery += " Where  Especies_Mixtas = 1 ";
            }

            List<Cultivo> Resultado = new List<Cultivo> { };

            Resultado = db.Database.SqlQuery<Cultivo>(sqlQuery).ToList();

            ViewBag.Cultivo_id = new SelectList(Resultado, "Cultivo_id", "Nombres_Comunes");

            return View(tbl_RNF_Rodal_Cultivo);

        }


        [HttpPost]
        public JsonResult GetRodal(string No_Registro, long finca_id)
        {

            IEnumerable<Tbl_RNF_Rodal> RodalesSelected = (from c in db.Tbl_RNF_Rodal
                                                          where c.No_Registro == No_Registro
                                                          && c.Finca_id == finca_id
                                                          && c.Tipo_de_Area != 3
                                                          select c);

            var Rodales = new SelectList(RodalesSelected, "Rodal_id", "Rodal_id");

            return Json(new SelectList(Rodales, "Value", "Text"));

        }



        [HttpPost]
        public JsonResult GetRodalTipo(string No_Registro, long finca_id, long rodal_id)
        {

            IEnumerable<Tbl_Sol_Rodal_Tipo> RodalesAreaSelected = (from c in db.Tbl_RNF_Rodal
                                                                   from d in db.Tbl_Sol_Rodal_Tipo
                                                                   where c.No_Registro == No_Registro
                                                                   && c.Finca_id == finca_id
                                                                   && c.Rodal_Id == rodal_id
                                                                   && c.Tipo_de_Area != 3
                                                                   && d.Tipo_de_Area == c.Tipo_de_Area
                                                                   select d);

            var RodalArea = new SelectList(RodalesAreaSelected, "Tipo_de_Area", "Descripcion");

            return Json(new SelectList(RodalArea, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult AgregarCultivo(string No_Registro, long finca_id, long rodal_id, int area_id, int cultivo_id, int anioestablecimiento, decimal Volumen_Rodal, decimal Volumen_ha)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int encontrado = db.Tbl_RNF_Rodal_Cultivo.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == finca_id && Obj.Rodal_Id == rodal_id && Obj.Tipo_de_Area == area_id && Obj.Cultivo_id == cultivo_id).Count();

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            if ((encontrado == 0) && (objRNFGrants.Agregar))// && ((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4))
            {
                Tbl_RNF_Rodal_Cultivo tbl_RNF_Rodal_Cultivo = new Tbl_RNF_Rodal_Cultivo();
                //Tbl_Sol_Rodal_Cultivo tbl_Sol_Rodal_Cultivo = new Tbl_Sol_Rodal_Cultivo();

                tbl_RNF_Rodal_Cultivo.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_Rodal_Cultivo.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Rodal_Cultivo.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Rodal_Cultivo.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Rodal_Cultivo.Finca_id = finca_id;
                tbl_RNF_Rodal_Cultivo.Rodal_Id = rodal_id;
                tbl_RNF_Rodal_Cultivo.Tipo_de_Area = area_id;
                tbl_RNF_Rodal_Cultivo.Cultivo_id = cultivo_id;
                tbl_RNF_Rodal_Cultivo.Anio_Establecimiento = anioestablecimiento;
                tbl_RNF_Rodal_Cultivo.Volumen_ha = Volumen_ha;
                tbl_RNF_Rodal_Cultivo.Volumen_Rodal = Volumen_Rodal;

                db.Tbl_RNF_Rodal_Cultivo.Add(tbl_RNF_Rodal_Cultivo);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarCultivo(string No_Registro, long finca_id, long rodal_id, int area_id, int cultivo_id)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Tbl_RNF_Rodal_Cultivo tbl_RNF_Rodal_Cultivo = db.Tbl_RNF_Rodal_Cultivo.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == finca_id && Obj.Rodal_Id == rodal_id && Obj.Tipo_de_Area == area_id && Obj.Cultivo_id == cultivo_id).FirstOrDefault();

            if (objRNFGrants.Borrar)
            {
                db.Tbl_RNF_Rodal_Cultivo.Remove(tbl_RNF_Rodal_Cultivo);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Empresa_Entidad_MateriaPrimaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Empresa_Entidad_MateriaPrima
        public ActionResult Index(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View(db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.No_Registro== No_Registro).ToList());
        }

        public ActionResult Create(string No_Registro)
        {
            ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj=> Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            ViewBag.No_Registro = No_Registro;

            return View();
        }

        [HttpPost]
        public JsonResult AgregarMateriaPrima(string No_Registro, int MateriaPrimaId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int encontrado = db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.No_Registro == No_Registro && Obj.Materia_Prima_id == MateriaPrimaId).Count();

            if ((encontrado == 0) && (objRNFGrants.Agregar))
            {
                Tbl_RNF_Empresa_Entidad_Materia_Prima tbl_RNF_Empresa_Entidad_Materia_Prima = new Tbl_RNF_Empresa_Entidad_Materia_Prima();

                tbl_RNF_Empresa_Entidad_Materia_Prima.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Empresa_Entidad_Materia_Prima.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Empresa_Entidad_Materia_Prima.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Empresa_Entidad_Materia_Prima.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_Empresa_Entidad_Materia_Prima.Materia_Prima_id = MateriaPrimaId;

                db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Add(tbl_RNF_Empresa_Entidad_Materia_Prima);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarMateriaPrima(string No_Registro, int MateriaPrimaId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Empresa_Entidad_Materia_Prima tbl_RNF_Empresa_Entidad_Materia_Prima = db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.No_Registro == No_Registro && Obj.Materia_Prima_id == MateriaPrimaId).FirstOrDefault();

            if (objRNFGrants.Borrar)
            {
                db.Tbl_RNF_Empresa_Entidad_Materia_Prima.Remove(tbl_RNF_Empresa_Entidad_Materia_Prima);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Empresa_Entidad_MaquinariaUtilizadaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Empresa_Entidad_MaquinariaUtilizada
        public ActionResult Index(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View(db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.No_Registro == No_Registro).ToList());
        }

        public ActionResult Create(string No_Registro)
        {
            ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj=> Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            ViewBag.No_Registro = No_Registro;

            return View();
        }

        [HttpPost]
        public JsonResult AgregarMaquinariaUtilizada(string No_Registro, int MaquinariaUtilizadaId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int encontrado = db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.No_Registro == No_Registro && Obj.Maquinaria_Utilizada_id == MaquinariaUtilizadaId).Count();

            if ((encontrado == 0) && (objRNFGrants.Agregar))
            {
                Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada = new Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada();

                tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Maquinaria_Utilizada_id = MaquinariaUtilizadaId;

                db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Add(tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarMaquinariaUtilizada(string No_Registro, int MaquinariaUtilizadaId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada = db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.No_Registro == No_Registro && Obj.Maquinaria_Utilizada_id == MaquinariaUtilizadaId).FirstOrDefault();

            if (objRNFGrants.Borrar)
            {
                db.Tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada.Remove(tbl_RNF_Empresa_Entidad_Maquinaria_Utilizada);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Empresa_Entidad_ActividadController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Empresa_Entidad_Actividad
        public ActionResult Index(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View(db.Tbl_RNF_Empresa_Entidad_Actividad.Where(Obj => Obj.No_Registro == No_Registro).ToList());
        }

        public ActionResult Create(string No_Registro)
        {
            ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            ViewBag.No_Registro = No_Registro;

            return View();
        }

        [HttpPost]
        public JsonResult AgregarActividad(string No_Registro, int ActividadId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int encontrado = db.Tbl_RNF_Empresa_Entidad_Actividad.Where(Obj => Obj.No_Registro == No_Registro && Obj.Actividad_id == ActividadId).Count();

            if ((encontrado == 0) && (objRNFGrants.Agregar))
            {
                Tbl_RNF_Empresa_Entidad_Actividad tbl_RNF_Empresa_Entidad_Actividad = new Tbl_RNF_Empresa_Entidad_Actividad();

                tbl_RNF_Empresa_Entidad_Actividad.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Empresa_Entidad_Actividad.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Empresa_Entidad_Actividad.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Empresa_Entidad_Actividad.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_Empresa_Entidad_Actividad.Actividad_id = ActividadId;

                db.Tbl_RNF_Empresa_Entidad_Actividad.Add(tbl_RNF_Empresa_Entidad_Actividad);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarActividad(string No_Registro, int ActividadId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Empresa_Entidad_Actividad tbl_RNF_Empresa_Entidad_Actividad = db.Tbl_RNF_Empresa_Entidad_Actividad.Where(Obj => Obj.No_Registro == No_Registro && Obj.Actividad_id == ActividadId).FirstOrDefault();

            if (objRNFGrants.Borrar)
            {
                db.Tbl_RNF_Empresa_Entidad_Actividad.Remove(tbl_RNF_Empresa_Entidad_Actividad);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }
    }
}
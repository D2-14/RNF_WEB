using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Sol_Empresa_Entidad_ActividadController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Empresa_Entidad_Actividad
        public ActionResult Index(int SolicitudId)
        {
            return View(db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == SolicitudId).ToList());
        }

        public ActionResult Create(int SolicitudId)
        {

            ViewBag.SolicitudId = SolicitudId;
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);


            if (tbl_sol_solicitud.Sub_Categoria_id == 1)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Industria_Forestal == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 2)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Deposito_Forestal == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 3)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Centro_Acopio == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 4)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Viveros_Forestales == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 5)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Exporta_Importa_Producto_Forestal == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 6)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Producto_Forestal_No_Maderable == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 7)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Repobladoras_Forestales == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 8)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Consultora_Forestal == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            }

           // ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad, "Actividad_Id", "Nombres_Comunes");

            return View();
        }

        public ActionResult CreateTecnico(int SolicitudId)
        {
            ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Estado_id == true && Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
            ViewBag.SolicitudId = SolicitudId;

            return View();
        }



        [HttpPost]
        public JsonResult AgregarActividad(long SolicitudId, int ActividadId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            int encontrado = db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Actividad_id == ActividadId).Count();

            string controllerName = "Sol_Empresa_Entidad";
            Usuario objUs = (Usuario)Session["User"];
            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Solicitud.Solicitud_id, controllerName, boolEsInterno).FirstOrDefault();

            if ((encontrado == 0) && ((bool)permisos.Agregar))
            {
                Tbl_Sol_Empresa_Entidad_Actividad tbl_Sol_Empresa_Entidad_Actividad = new Tbl_Sol_Empresa_Entidad_Actividad();

                tbl_Sol_Empresa_Entidad_Actividad.Solicitud_id = SolicitudId;
                tbl_Sol_Empresa_Entidad_Actividad.Actividad_id = ActividadId;

                db.Tbl_Sol_Empresa_Entidad_Actividad.Add(tbl_Sol_Empresa_Entidad_Actividad);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarActividad(long SolicitudId, int ActividadId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            Tbl_Sol_Empresa_Entidad_Actividad tbl_Sol_Empresa_Entidad_Actividad = db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Actividad_id == ActividadId).First();

            string controllerName = "Sol_Empresa_Entidad";
            Usuario objUs = (Usuario)Session["User"];
            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Solicitud.Solicitud_id, controllerName, boolEsInterno).FirstOrDefault();

            if ((bool)permisos.Borrar)
            {
                db.Tbl_Sol_Empresa_Entidad_Actividad.Remove(tbl_Sol_Empresa_Entidad_Actividad);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
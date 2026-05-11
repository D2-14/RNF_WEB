using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Sol_Empresa_Entidad_TecnicoMaquinariaUtilizadaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Empresa_Entidad_TecnicoMaquinariaUtilizada
        public ActionResult Index(int SolicitudId)
        {
            return View(db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == SolicitudId).ToList());
        }

        public ActionResult Create(int SolicitudId)
        {
            //ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada, "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            ViewBag.SolicitudId = SolicitudId;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);


            if (tbl_sol_solicitud.Sub_Categoria_id == 1)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Industria_Forestal == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 2)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Deposito_Forestal == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 3)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Centro_Acopio == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 4)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Viveros_Forestales == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 5)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Exporta_Importa_Producto_Forestal == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 6)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Producto_Forestal_No_Maderable == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 7)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Repobladoras_Forestales == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 8)
            {
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Consultora_Forestal == true && Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            // ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad, "Actividad_Id", "Nombres_Comunes");

            return View();
        }

        [HttpPost]
        public JsonResult AgregarMaquinariaUtilizada(long SolicitudId, int MaquinariaUtilizadaId)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            int codRespuesta = 1;
            string strRespuesta = "Registro agregado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            int encontrado = db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Maquinaria_Utilizada_id == MaquinariaUtilizadaId).Count();

            if ((encontrado == 0) && ((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4) || (objUs.EsInterno == 1)))
            {
                Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada Tbl_sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada = new Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada();

                Tbl_sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Solicitud_id = SolicitudId;
                Tbl_sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Maquinaria_Utilizada_id = MaquinariaUtilizadaId;

                db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Add(Tbl_sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarMaquinariaUtilizada(long SolicitudId, int MaquinariaUtilizadaId)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada = db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Maquinaria_Utilizada_id == MaquinariaUtilizadaId).First();

            if (((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4) || (objUs.EsInterno == 1)))
            {
                db.Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada.Remove(Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
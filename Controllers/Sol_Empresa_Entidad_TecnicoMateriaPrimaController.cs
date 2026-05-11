using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RNF_Web.Controllers
{
    public class Sol_Empresa_Entidad_TecnicoMateriaPrimaController : Controller
    {
        // GET: Sol_Empresa_Entidad_TecnicoMateriaPrima
       
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Empresa_Entidad_MateriaPrima
        public ActionResult Index(int SolicitudId)
        {
            ViewBag.SolicitudId = SolicitudId;
            return View(db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Where(Obj => Obj.Solicitud_id == SolicitudId).ToList());
        }

        public ActionResult Create(int SolicitudId)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            if (tbl_sol_solicitud.Sub_Categoria_id == 1)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Industria_Forestal == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 2)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Deposito_Forestal == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 3)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Centro_Acopio == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 4)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Viveros_Forestales == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 5)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Exporta_Importa_Producto_Forestal == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 6)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Producto_Forestal_No_Maderable == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 7)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Repobladoras_Forestales == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            if (tbl_sol_solicitud.Sub_Categoria_id == 8)
            {
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Consultora_Forestal == true && Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
            }

            ViewBag.SolicitudId = SolicitudId;

            return View();
        }

        [HttpPost]
        public JsonResult AgregarMateriaPrima(long SolicitudId, int MateriaPrimaId)
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

            int encontrado = db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Materia_Prima_id == MateriaPrimaId).Count();

            if ((encontrado == 0) && ((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4) || (objUs.EsInterno == 1)))
            {
                Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima = new Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima();

                tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Solicitud_id = SolicitudId;
                tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Materia_Prima_id = MateriaPrimaId;

                db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Add(tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima);
                db.SaveChanges();

            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);

        }

        [HttpPost]
        public JsonResult EliminarMateriaPrima(long SolicitudId, int MateriaPrimaId)
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

            Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima tbl_Sol_Empresa_Entidad_tecnicoMateria_Prima = db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Materia_Prima_id == MateriaPrimaId).First();

            if (((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4) || (objUs.EsInterno == 1)))
            {
                db.Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima.Remove(tbl_Sol_Empresa_Entidad_tecnicoMateria_Prima);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
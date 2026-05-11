using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Sol_TipoRegistroController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_TipoRegistro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
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

            long id = (long)Session[Constants.session_Solicitud];

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            int intTbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).Count();
            Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_sol_empresa_entidad_tipo_registro;

            if (intTbl_Sol_Empresa_Entidad_Tipo_Registro > 0)
            {
                tbl_sol_empresa_entidad_tipo_registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).First();

                ViewBag.TieneEntidadTipoRegistro = 1;
            }
            else
            {

                tbl_sol_empresa_entidad_tipo_registro = new Tbl_Sol_Empresa_Entidad_Tipo_Registro();

                tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id = (long)Session[Constants.session_Solicitud];

            }


            if (Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] != null)
            {
                tbl_sol_empresa_entidad_tipo_registro = (Tbl_Sol_Empresa_Entidad_Tipo_Registro)Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro];
            }

            //if ((tbl_sol_solicitud.Categoria_id == 5) && (tbl_sol_solicitud.Sub_Categoria_id == 4))
            //{
            //    ViewBag.Tipo_Registro_Id = new SelectList(db.Tbl_Gral_Tipo_Registro, "Tipo_Registro_Id", "Tipo_Registro", tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id);
            //}
            //else
            //{
                ViewBag.Tipo_Registro_Id = new SelectList(db.Tbl_Gral_Tipo_Registro, "Tipo_Registro_Id", "Tipo_Registro", tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id);
            //}

            ViewBag.REPEJU_De_Id = new SelectList(db.Tbl_REPEJU_De, "REPEJU_De_Id", "Descripcion", tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id);


            return View(tbl_sol_empresa_entidad_tipo_registro);
        }


        public ActionResult CreateTecnico()
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

            long id = (long)Session[Constants.session_Solicitud];

            int intTbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).Count();
            Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_sol_empresa_entidad_tipo_registro;

            if (intTbl_Sol_Empresa_Entidad_Tipo_Registro > 0)
            {
                tbl_sol_empresa_entidad_tipo_registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).First();

                ViewBag.TieneEntidadTipoRegistro = 1;
            }
            else
            {

                tbl_sol_empresa_entidad_tipo_registro = new Tbl_Sol_Empresa_Entidad_Tipo_Registro();

                tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id = (long)Session[Constants.session_Solicitud];

            }


            if (Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] != null)
            {
                tbl_sol_empresa_entidad_tipo_registro = (Tbl_Sol_Empresa_Entidad_Tipo_Registro)Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro];
            }

            ViewBag.Tipo_Registro_Id = new SelectList(db.Tbl_Gral_Tipo_Registro, "Tipo_Registro_Id", "Tipo_Registro", tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id);
            ViewBag.REPEJU_De_Id = new SelectList(db.Tbl_REPEJU_De, "REPEJU_De_Id", "Descripcion", tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id);


            return View(tbl_sol_empresa_entidad_tipo_registro);
        }


        [HttpPost]
        public ActionResult Create(Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_sol_empresa_entidad_tipo_registro)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id);


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

            if ((tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 4) || (tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 5))
            {
                tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id = 0;
                tbl_sol_empresa_entidad_tipo_registro.No_Partida
                    = tbl_sol_empresa_entidad_tipo_registro.No_Libro
                    = tbl_sol_empresa_entidad_tipo_registro.No_Folio = null;
            }
            else if (tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 1)
            {
                tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id = 0;
                tbl_sol_empresa_entidad_tipo_registro.Fecha_Acta = null;
                tbl_sol_empresa_entidad_tipo_registro.No_Acta = null;
            }
            else if (tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 2)
            {
                tbl_sol_empresa_entidad_tipo_registro.Fecha_Acta = null;
                tbl_sol_empresa_entidad_tipo_registro.No_Acta = null;

            }
            else if (tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 3)
            {
                tbl_sol_empresa_entidad_tipo_registro.Fecha_Acta = null;
                tbl_sol_empresa_entidad_tipo_registro.No_Libro = tbl_sol_empresa_entidad_tipo_registro.No_Folio = tbl_sol_empresa_entidad_tipo_registro.No_Acta = null;
                tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id = 0;

            }

            string controllerName = "Sol_Empresa_Entidad";
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id, controllerName, boolEsInterno).FirstOrDefault();
            TempData["MensajeREPEJU"] = "";

            if (!(bool)permisos.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
            }
            else
            {
                Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] = null;

                if ((tbl_sol_empresa_entidad_tipo_registro.Tipo_Registro_Id == 2) && (tbl_sol_empresa_entidad_tipo_registro.REPEJU_De_Id == 0))
                {
                    TempData["MensajeREPEJU"] = "Error: Debe seleccionar el Tipo REPEJU al que corresponde su Registro REPEJU.";
                    Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] = (Tbl_Sol_Empresa_Entidad_Tipo_Registro)tbl_sol_empresa_entidad_tipo_registro;

                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });

                }


                if (ModelState.IsValid)
                {
                    long id = tbl_sol_solicitud.Solicitud_id;

                    int intTbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.Solicitud_Id == id).Count();

                    if (intTbl_Sol_Empresa_Entidad_Tipo_Registro == 0)
                    {
                        tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id = id;
                        db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Add(tbl_sol_empresa_entidad_tipo_registro);
                        db.SaveChanges();
                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                    else
                    {
                        tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id = id;
                        db.Entry(tbl_sol_empresa_entidad_tipo_registro).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";

                Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] = (Tbl_Sol_Empresa_Entidad_Tipo_Registro)tbl_sol_empresa_entidad_tipo_registro;
            }

            ViewBag.SolicitudId = tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id;
            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });

        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class RNF_TipoRegistroController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_TipoRegistro
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(string No_Registro)
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

            ViewBag.No_Registro = No_Registro;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            int intTbl_RNF_Empresa_Entidad_Tipo_Registro = db.Tbl_RNF_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.No_Registro == No_Registro).Count();
            Tbl_RNF_Empresa_Entidad_Tipo_Registro tbl_RNF_Empresa_Entidad_Tipo_Registro;

            if (intTbl_RNF_Empresa_Entidad_Tipo_Registro > 0)
            {
                tbl_RNF_Empresa_Entidad_Tipo_Registro = db.Tbl_RNF_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

                ViewBag.TieneEntidadTipoRegistro = 1;
            }
            else
            {

                tbl_RNF_Empresa_Entidad_Tipo_Registro = new Tbl_RNF_Empresa_Entidad_Tipo_Registro();

                tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Empresa_Entidad_Tipo_Registro.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Empresa_Entidad_Tipo_Registro.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Empresa_Entidad_Tipo_Registro.Solicitud_Id = tbl_RNF_Registro.Solicitud_id;

            }


            if (Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] != null)
            {
                tbl_RNF_Empresa_Entidad_Tipo_Registro = (Tbl_RNF_Empresa_Entidad_Tipo_Registro)Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro];
            }

            ViewBag.Tipo_Registro_Id = new SelectList(db.Tbl_Gral_Tipo_Registro, "Tipo_Registro_Id", "Tipo_Registro", tbl_RNF_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id);
            ViewBag.REPEJU_De_Id = new SelectList(db.Tbl_REPEJU_De, "REPEJU_De_Id", "Descripcion", tbl_RNF_Empresa_Entidad_Tipo_Registro.REPEJU_De_Id);


            return View(tbl_RNF_Empresa_Entidad_Tipo_Registro);
        }

        [HttpPost]
        public ActionResult Create(Tbl_RNF_Empresa_Entidad_Tipo_Registro tbl_RNF_Empresa_Entidad_Tipo_Registro)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
            }
            else
            {
                Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] = null;

                if (ModelState.IsValid)
                {
                    Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro).FirstOrDefault();

                    int intTbl_RNF_Empresa_Entidad_Tipo_Registro = db.Tbl_RNF_Empresa_Entidad_Tipo_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro).Count();

                    if (intTbl_RNF_Empresa_Entidad_Tipo_Registro == 0)
                    {
                        db.Tbl_RNF_Empresa_Entidad_Tipo_Registro.Add(tbl_RNF_Empresa_Entidad_Tipo_Registro);
                        db.SaveChanges();
                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro });
                    }
                    else
                    {
                        db.Entry(tbl_RNF_Empresa_Entidad_Tipo_Registro).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";

                Session[Constants.session_Tbl_Sol_Empresa_Entidad_Tipo_Registro] = (Tbl_RNF_Empresa_Entidad_Tipo_Registro)tbl_RNF_Empresa_Entidad_Tipo_Registro;
            }

            ViewBag.SolicitudId = tbl_RNF_Empresa_Entidad_Tipo_Registro.Solicitud_Id;
            ViewBag.NoRegistro = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro;
            ViewBag.NoRegistroLiteral = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_RegistroLiteral;
            ViewBag.NoRegistroCorrelativo = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_RegistroCorrelativo;
            return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad_Tipo_Registro.No_Registro });
            //return RedirectToAction("/Sol_Empresa_Entidad_Tipo_Registro/Create", new { id = tbl_sol_empresa_entidad_tipo_registro.Solicitud_Id });

        }
    }
}
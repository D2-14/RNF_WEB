using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class InactivacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        // GET: Inactivacion
        public ActionResult Index()
        {
            Usuario objUs = new Usuario();
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

            return View(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id != 0).ToList());
        }

        public ActionResult Categoria(int Categoria_id)
        {
            Usuario objUs = new Usuario();
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

            ViewBag.Categoria_id = Categoria_id;

            return View(db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == Categoria_id).OrderBy(Obj => Obj.TipoInactivacion_id).ToList());
        }

        public ActionResult AgregarTipo(int Categoria_id)
        {
            Usuario objUs = new Usuario();
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
            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }

            Tbl_Sol_Solicitud_Categoria tbl_Sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(Categoria_id);

            if(tbl_Sol_Solicitud_Categoria== null)
            {

            }

            ViewBag.Categoria_id = Categoria_id;
            ViewBag.AlertaTipoInactivacion = "";

            Tbl_RNF_Registro_Inactivacion_Tipo tbl_RNF_Registro_Inactivacion_Tipo = new Tbl_RNF_Registro_Inactivacion_Tipo()
            {
                Categoria_id = tbl_Sol_Solicitud_Categoria.Categoria_id,
                UsuarioInterno= true,
                UsuarioExterno = true,
            };
            return View(tbl_RNF_Registro_Inactivacion_Tipo);
        }

        [HttpPost]
        public ActionResult AgregarTipo(Tbl_RNF_Registro_Inactivacion_Tipo model)
        {
            Usuario objUs = new Usuario();
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
            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }

            if((model.Descripcion == null) || (model.Descripcion.Trim() == ""))
            {
                ViewBag.AlertaTipoInactivacion = "Debe agregar la descripción al tipo de inactivación";
                return View(model);
            }
            model.Descripcion = model.Descripcion.Trim();
            ViewBag.Categoria_id = model.Categoria_id;
            model.swcreatedby = objUs.intUsuario_id;
            model.swdateupdated = DateTime.Now;
            model.swupdatedbyinterno = EsInterno;

            int tipoinactivacionid = 0;

            try
            {
                tipoinactivacionid = db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == model.Categoria_id).Max(Obj => Obj.TipoInactivacion_id);
            }
            catch
            {
                tipoinactivacionid = 0;
            }
            tipoinactivacionid++;

            model.TipoInactivacion_id = tipoinactivacionid;

            if(ModelState.IsValid)
            {
                db.Tbl_RNF_Registro_Inactivacion_Tipo.Add(model);
                db.SaveChanges();
                return RedirectToAction("RegistroAgregado", "Home");
            }


            return View(model);
        }

        public class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }

        public JsonResult CambiarEstadoInactivacion(Tbl_RNF_Registro_Inactivacion_Tipo model)
        {
            RespuestaJSON respuestaJSON = new RespuestaJSON()
            {
                Result = 0,
                Mensaje = "No se encontró una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(respuestaJSON);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno= true;
            }

            Tbl_RNF_Registro_Inactivacion_Tipo tbl_RNF_Registro_Inactivacion_Tipo = db.Tbl_RNF_Registro_Inactivacion_Tipo.Find(model.Categoria_id, model.TipoInactivacion_id);
            if(tbl_RNF_Registro_Inactivacion_Tipo == null)
            {
                respuestaJSON = new RespuestaJSON()
                {
                    Result = 2,
                    Mensaje = "No se encontró el tipo de inactivación indiciado"
                };
            }
            else
            {
                try
                {
                    tbl_RNF_Registro_Inactivacion_Tipo.UsuarioInterno = model.UsuarioInterno;
                    tbl_RNF_Registro_Inactivacion_Tipo.UsuarioExterno = model.UsuarioExterno;
                    tbl_RNF_Registro_Inactivacion_Tipo.swdateupdated = DateTime.Now;
                    tbl_RNF_Registro_Inactivacion_Tipo.swupdatedby = objUs.intUsuario_id;
                    tbl_RNF_Registro_Inactivacion_Tipo.swupdatedbyinterno = EsInterno;
                    db.Entry(tbl_RNF_Registro_Inactivacion_Tipo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 1,
                        Mensaje = "Cambio realizado exitsamente"
                    };
                }
                catch (Exception ex)
                {
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 3,
                        Mensaje = "Ocurrió un error: " + ex.Message + " " + ex.InnerException.ToString(),
                        data = ex
                    };
                }
            }


            return Json(respuestaJSON);
        }

        public JsonResult EliminarTipoInactivacion(Tbl_RNF_Registro_Inactivacion_Tipo model)
        {
            RespuestaJSON respuestaJSON = new RespuestaJSON()
            {
                Result = 0,
                Mensaje = "No se encontró una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(respuestaJSON);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }

            Tbl_RNF_Registro_Inactivacion_Tipo tbl_RNF_Registro_Inactivacion_Tipo = db.Tbl_RNF_Registro_Inactivacion_Tipo.Find(model.Categoria_id, model.TipoInactivacion_id);
            if (tbl_RNF_Registro_Inactivacion_Tipo == null)
            {
                respuestaJSON = new RespuestaJSON()
                {
                    Result = 2,
                    Mensaje = "No se encontró el tipo de inactivación indiciado"
                };
            }
            else
            {
                try
                {
                    db.Tbl_RNF_Registro_Inactivacion_Tipo.Remove(tbl_RNF_Registro_Inactivacion_Tipo);
                    db.SaveChanges();
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 1,
                        Mensaje = "Registro eliminado"
                    };
                }
                catch (Exception ex)
                {
                    respuestaJSON = new RespuestaJSON()
                    {
                        Result = 3,
                        Mensaje = "Ocurrió un error: " + ex.Message + " " + ex.InnerException.ToString(),
                        data = ex
                    };
                }
            }


            return Json(respuestaJSON);
        }
    }
}
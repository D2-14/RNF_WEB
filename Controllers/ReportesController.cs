using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Index(string Control, string Key)
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

            ViewBag.Control = Control;
            ViewBag.Key = Key;


            return View();
        }

        public class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }

        public class DatosSession
        {
            public string Nombre_Usuario { get; set; }
            public string EsInterno { get; set; }
            public string Usuario_id { get; set; }
            public long ts { get; set; }
        }

        public JsonResult ObtenerDatos()
        {
            RespuestaJSON respuestaJSON = new RespuestaJSON()
            {
                Result = 0,
                Mensaje = "No se ha encontrado una sesión activa"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(respuestaJSON);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno != 1)
            {
                respuestaJSON = new RespuestaJSON()
                {
                    Result = 2,
                    Mensaje = "No cuenta con los permisos necesarios para acceder a esta función"
                };
                return Json(respuestaJSON);
            }

            DatosSession datosSession = new DatosSession()
            {
                Nombre_Usuario = SecurEncryptDecrypt.EncryptString(objUs.strNombre_Usuario),
                EsInterno = SecurEncryptDecrypt.EncryptString(objUs.EsInterno.ToString()),
                Usuario_id = SecurEncryptDecrypt.EncryptString(objUs.intUsuario_id.ToString()),
                ts = DateTime.Now.Ticks,
            };
            respuestaJSON = new RespuestaJSON()
            {
                Result = 1,
                Mensaje = "Datos de validación generados exitosamente",
                data = datosSession
            };


            return Json(respuestaJSON);
        }

        public JsonResult VerificarSession(DatosSession model)
        {
            DateTime swdatenow = DateTime.Now;
            DateTime Dts = new DateTime(model.ts);
            long ticksnow = swdatenow.Ticks;
            long diferencia = ticksnow - model.ts;

            Reply reply = new Reply()
            {
                result = 0,
                message = "Tiempo de verificación excedida"
            };
            if (diferencia > 600000000)
            {
                return Json(reply);
            }



            long usuarioid = 0;
            try
            {
                usuarioid = long.Parse(SecurEncryptDecrypt.DecryptString(model.Usuario_id));
            }
            catch (Exception ex)
            {
                reply = new Reply()
                {
                    result = 3,
                    message = "Llave Usuario no válida"
                };
                return Json(reply);
            }

            int esinterno = 0;
            try
            {
                esinterno = int.Parse(SecurEncryptDecrypt.DecryptString(model.EsInterno));
            }
            catch (Exception ex)
            {
                reply = new Reply()
                {
                    result = 4,
                    message = "Llave de Interno no válida"
                };
                return Json(reply);
            }

            if (esinterno != 1)
            {
                reply = new Reply()
                {
                    result = 5,
                    message = "No cuenta con los permisos necesarios para acceder a esta función"
                };
                return Json(reply);
            }


            reply = new Reply()
            {
                result = 1,
                message = "Sesión válida"
            };
            return Json(reply);
        }


    }
}
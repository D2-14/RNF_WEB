using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gral_Mapa_Vocacion_ForestalController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerMapa()
        {
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gral_Mapa_Vocacion_Forestal oMapa = new Tbl_Gral_Mapa_Vocacion_Forestal();

            oMapa = (from d in db.Tbl_Gral_Mapa_Vocacion_Forestal
                     where d.Estado == true
                     select d).FirstOrDefault();



            return View(oMapa);
        }

        public ActionResult AgregarMapa()
        {
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            List<Tbl_Gral_Mapa_Vocacion_Forestal> oMapa = new List<Tbl_Gral_Mapa_Vocacion_Forestal>();

            //oMapa = (from d in db.Tbl_Gral_Mapa_Vocacion_Forestal
            //         select d).ToList();



            return View(db.Tbl_Gral_Mapa_Vocacion_Forestal.ToList());
        }

        [HttpPost]
        public ActionResult AgregarMapa(HttpPostedFileBase Archivo)
        {
            string TextoMostrar, Ubicacion;
            int Mapa_id;
            DateTime swdate = DateTime.Now;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            List<Tbl_Gral_Mapa_Vocacion_Forestal> oMapa = new List<Tbl_Gral_Mapa_Vocacion_Forestal>();

            oMapa = (from d in db.Tbl_Gral_Mapa_Vocacion_Forestal
                     select d).ToList();


            Ubicacion = AgregarMapaNuevo(Archivo);
            if (Ubicacion != null)
            {
                try
                {
                    Mapa_id = ((db.Tbl_Gral_Mapa_Vocacion_Forestal.Max(d => d.Mapa_id)) + 1);
                }
                catch (Exception ex)
                {
                    Mapa_id = 1;
                }

                Tbl_Gral_Mapa_Vocacion_Forestal oMapasAnteriores = (from d in db.Tbl_Gral_Mapa_Vocacion_Forestal
                                                                    where d.Estado == true
                                                                    select d).FirstOrDefault();
                if (oMapasAnteriores != null)
                {
                    oMapasAnteriores.Estado = false;
                    oMapasAnteriores.swdateupdated = swdate;
                    oMapasAnteriores.swupdatedby = objUs.intUsuario_id;
                }


                Tbl_Gral_Mapa_Vocacion_Forestal oVocacion = new Tbl_Gral_Mapa_Vocacion_Forestal();
                oVocacion.Nombre = Ubicacion;
                oVocacion.Estado = true;
                oVocacion.swdatecreated = swdate;
                oVocacion.swcreatedby = objUs.intUsuario_id;
                oVocacion.Mapa_id = Mapa_id;

                db.Tbl_Gral_Mapa_Vocacion_Forestal.Add(oVocacion);

                db.SaveChanges();
                TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";

            }


            return View(oMapa);
        }

        public string AgregarMapaNuevo(HttpPostedFileBase archivo)
        {
            DateTime FechaCarga = DateTime.Now;
            string formatoarchivo, path, filename, PathCrear, partialpath, partialdirectory;
            string FormatoFechaCarga = FechaCarga.ToString("yyyyMMdd_HHmmss");
            string NombreArchivo = "Vocacion_Forestal_";
            partialdirectory = "/Archivos_Machotes/Mapa/";
            path = Server.MapPath($"~{partialdirectory}");
            formatoarchivo = Path.GetExtension(archivo.FileName);
            partialpath = $"{NombreArchivo}{FormatoFechaCarga}{formatoarchivo}";
            if (archivo != null)
            {
                var data = new byte[archivo.ContentLength];
                archivo.InputStream.Read(data, 0, archivo.ContentLength);
                filename = Path.Combine(path, $"{partialpath}");

                if ((System.IO.File.Exists(path)) == true)
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    PathCrear = path;

                    if (!Directory.Exists(PathCrear))
                    {
                        Directory.CreateDirectory(PathCrear);
                    }
                }
                System.IO.File.WriteAllBytes(Path.Combine(path, filename), data);
                archivo.SaveAs(filename);
                return $"{partialdirectory}{partialpath}";
            }
            else
            {
                return null;
            }
        }
        public JsonResult CargarMapa(HttpPostedFileBase archivo)
        {
            string TextoMostrar, Ubicacion;
            int Mapa_id;
            DateTime swdate = DateTime.Now;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            Ubicacion = AgregarMapaNuevo(archivo);
            if (Ubicacion != null)
            {
                try
                {
                    Mapa_id = ((db.Tbl_Gral_Mapa_Vocacion_Forestal.Max(d => d.Mapa_id)) + 1);
                }
                catch (Exception ex)
                {
                    Mapa_id = 1;
                }

                Tbl_Gral_Mapa_Vocacion_Forestal oMapasAnteriores = (from d in db.Tbl_Gral_Mapa_Vocacion_Forestal
                                                                    where d.Estado == true
                                                                    select d).FirstOrDefault();
                if (oMapasAnteriores != null)
                {
                    oMapasAnteriores.Estado = false;
                    oMapasAnteriores.swdateupdated = swdate;
                    oMapasAnteriores.swupdatedby = objUs.intUsuario_id;
                }


                Tbl_Gral_Mapa_Vocacion_Forestal oVocacion = new Tbl_Gral_Mapa_Vocacion_Forestal();
                oVocacion.Nombre = Ubicacion;
                oVocacion.Estado = true;
                oVocacion.swdatecreated = swdate;
                oVocacion.swcreatedby = objUs.intUsuario_id;
                oVocacion.Mapa_id = Mapa_id;

                db.Tbl_Gral_Mapa_Vocacion_Forestal.Add(oVocacion);

                db.SaveChanges();
                TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";
                return Json(TextoMostrar);

            }
            else
            {
                return Json(null);
            }


        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Gest_EtapaSolicitud_DocumentoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();


        //GuidEtapa_id
        //Guid_id
        //etapa_id
        //etaparuta_id
        //correlativoetapa_id
        //

        public ActionResult DocumentoCreate(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();

            if(tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                               && d.Etapa_id == etapa_id
                                                               && d.EtapaRuta_id == etaparuta_id
                                                               && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();


            ViewBag.Title = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id).First().Documentos_RequeridosDescripcion;


            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_Gest_EtapaSolicitud_Documento tbl_Gest_EtapaSolicitud_Documento = new Tbl_Gest_EtapaSolicitud_Documento()
            {
                Solicitud_id = tbl_Gest_EtapaSolicitud.Solicitud_id,
                Etapa_id = tbl_Gest_EtapaSolicitud.Etapa_id,
                EtapaRuta_id = tbl_Gest_EtapaSolicitud.EtapaRuta_id,
                CorrelativoEtapa_id = tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id
            };

            tbl_Gest_EtapaSolicitud_Documento.Tbl_Gest_EtapaSolicitud = tbl_Gest_EtapaSolicitud;

            return View(tbl_Gest_EtapaSolicitud_Documento);
        }

        public ActionResult DocumentoLista(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                               && d.Etapa_id == etapa_id
                                                               && d.EtapaRuta_id == etaparuta_id
                                                               && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            List<Tbl_Gest_EtapaSolicitud_Documento> tbl_Gest_EtapaSolicitud_Documentos = (from d in db.Tbl_Gest_EtapaSolicitud_Documento
                                                                                          where d.Solicitud_id == tbl_Gest_EtapaSolicitud.Solicitud_id
                                                                                          && d.Etapa_id == tbl_Gest_EtapaSolicitud.Etapa_id
                                                                                          && d.EtapaRuta_id == tbl_Gest_EtapaSolicitud.EtapaRuta_id
                                                                                          && d.CorrelativoEtapa_id == tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id
                                                                                          select d).ToList();

            if(tbl_Gest_EtapaSolicitud_Documentos == null)
            {
                tbl_Gest_EtapaSolicitud_Documentos = new List<Tbl_Gest_EtapaSolicitud_Documento>();
            }

            return View(tbl_Gest_EtapaSolicitud_Documentos);
        }

        public ActionResult DocumentoEtapaSolicitud(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                               && d.Etapa_id == etapa_id
                                                               && d.EtapaRuta_id == etaparuta_id
                                                               && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            List<Tbl_Gest_EtapaSolicitud_Documento> tbl_Gest_EtapaSolicitud_Documentos = (from d in db.Tbl_Gest_EtapaSolicitud_Documento
                                                                                          where d.Solicitud_id == tbl_Gest_EtapaSolicitud.Solicitud_id
                                                                                          && d.Etapa_id == tbl_Gest_EtapaSolicitud.Etapa_id
                                                                                          && d.EtapaRuta_id == tbl_Gest_EtapaSolicitud.EtapaRuta_id
                                                                                          && d.CorrelativoEtapa_id == tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id
                                                                                          select d).ToList();

            if (tbl_Gest_EtapaSolicitud_Documentos == null)
            {
                tbl_Gest_EtapaSolicitud_Documentos = new List<Tbl_Gest_EtapaSolicitud_Documento>();
            }

            return View(tbl_Gest_EtapaSolicitud_Documentos);
        }




        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }
        [HttpPost]
        public JsonResult AgregarDocumento(Tbl_Gest_EtapaSolicitud_Documento model, HttpPostedFileBase upload)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
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
            bool ErrorDetectado = false;

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == model.Solicitud_id
                                                               && d.Etapa_id == model.Etapa_id
                                                               && d.EtapaRuta_id == model.EtapaRuta_id
                                                               && d.CorrelativoEtapa_id == model.CorrelativoEtapa_id
                                                               select d).FirstOrDefault();

            if(tbl_Gest_EtapaSolicitud == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "No existe esta etapa"
                };
            }

            int Documento_id = 0;

            try
            {
                Documento_id = db.Tbl_Gest_EtapaSolicitud_Documento.Where(Obj =>
                    Obj.Solicitud_id == model.Solicitud_id
                    && Obj.Etapa_id == model.Etapa_id
                    && Obj.EtapaRuta_id == model.EtapaRuta_id
                    && Obj.CorrelativoEtapa_id == model.CorrelativoEtapa_id)
                    .Max(Obj => Obj.Documento_id);
            }
            catch(Exception ex)
            {
                Documento_id = 0;
            }

            Documento_id++;

            string partialpath = "~/Gest_EtapaSolicitud_Documento/" + model.Solicitud_id + "/" + model.Etapa_id + "/" + model.EtapaRuta_id.ToString("0.00") + "/" + model.CorrelativoEtapa_id + "/";
            string Nombre_Archivo = "";
            try
            {
                Nombre_Archivo = upload.FileName;
                Nombre_Archivo = Nombre_Archivo.Replace("-", "");
                Nombre_Archivo = Nombre_Archivo.Replace("(", "");
                Nombre_Archivo = Nombre_Archivo.Replace(")", "");
                Nombre_Archivo = Nombre_Archivo.Replace(" ", "");

                partialpath = partialpath.Replace(".", "_");


                string PathArchivo = Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);

                if (!System.IO.File.Exists(PathArchivo))
                {
                    string PathCrear = partialpath;
                    if (!Directory.Exists(Server.MapPath(PathCrear)))
                    {
                        Directory.CreateDirectory(Server.MapPath(PathCrear));
                    }
                    upload.SaveAs(PathArchivo);
                }

                model.FileName = Nombre_Archivo;
                ViewBag.MensajeDocumento += "Archivo subido con éxito";
            }
            catch
            {
                ViewBag.MensajeDocumento += "Hubo un error al subir el archivo";
                ErrorDetectado = true;
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 3,
                    Mensaje = "Hubo un error al subir el archivo"
                };
            }

            if (!ErrorDetectado)
            {
                model.Documento_id = Documento_id;
                model.swcreatedby = objUs.intUsuario_id;
                model.swdatecreated = DateTime.Now;
                model.swcreatedbyinterno = EsInterno;
                db.Tbl_Gest_EtapaSolicitud_Documento.Add(model);
                db.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje="Documento agregado exitosamente"
                };
            }



            return Json(jsonRespuesta);
        }

        public JsonResult EliminarDocumento(Tbl_Gest_EtapaSolicitud_Documento model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_EtapaSolicitud_Documento tbl_Gest_EtapaSolicitud_Documento = (from d in db.Tbl_Gest_EtapaSolicitud_Documento
                                                                                   where d.Solicitud_id == model.Solicitud_id
                                                                                   && d.Etapa_id == model.Etapa_id
                                                                                   && d.EtapaRuta_id == model.EtapaRuta_id
                                                                                   && d.CorrelativoEtapa_id == model.CorrelativoEtapa_id
                                                                                   && d.Documento_id == model.Documento_id
                                                                                   select d).FirstOrDefault();

            if(tbl_Gest_EtapaSolicitud_Documento != null)
            {
                db.Tbl_Gest_EtapaSolicitud_Documento.Remove(tbl_Gest_EtapaSolicitud_Documento);
                db.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Documento eliminado exitosamente"
                };
            }
            else
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "No se ha encontrado el documento, verifique nuevamente por favor"
                };
            }

            return Json(jsonRespuesta);
        }

        public JsonResult Obtenerhref(Tbl_Gest_EtapaSolicitud_Documento model)
        {

            Tbl_Gest_EtapaSolicitud_Documento img = db.Tbl_Gest_EtapaSolicitud_Documento.Where(Obj => Obj.Solicitud_id == model.Solicitud_id && Obj.Etapa_id == model.Etapa_id && Obj.EtapaRuta_id == model.EtapaRuta_id && Obj.CorrelativoEtapa_id == model.CorrelativoEtapa_id && Obj.Documento_id == model.Documento_id).FirstOrDefault();

            string partialpath = "/Gest_EtapaSolicitud_Documento/" + model.Solicitud_id + "/" + model.Etapa_id + "/" + model.EtapaRuta_id.ToString("0.00") + "/" + model.CorrelativoEtapa_id + "/";
            partialpath = partialpath.Replace(".", "_");

            string extension = Path.GetExtension(img.FileName).ToLower();


            if (extension == ".pdf" || extension == ".jpg" || extension == ".bmp" || extension == ".png" || extension == ".gif")
            {

                string fileLocation = partialpath + img.FileName;

                return Json(fileLocation);


            }
            else
            {
                string fileLocation = partialpath + img.FileName;

                return Json(fileLocation);
            }
        }




    }
}
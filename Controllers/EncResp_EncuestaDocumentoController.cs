using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;


namespace RNF_Web.Controllers
{
    public class EncResp_EncuestaDocumentoController : Controller
    {
        db_RNF_SurveyEntities db_Survey = new db_RNF_SurveyEntities();
        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }





        public ActionResult ArchivosSubir(int id_encuesta, long swcreatedby, int id_Correlativo, int id_pregunta)
        {
            Tbl_EncResp_Encuesta_Documento tbl_EncResp_Encuesta_Documento = new Tbl_EncResp_Encuesta_Documento()
            {
                id_encuesta = id_encuesta,
                swcreatedby = swcreatedby,
                id_Correlativo = id_Correlativo,
                id_pregunta = id_pregunta,
            };
            return View(tbl_EncResp_Encuesta_Documento);
        }

        public ActionResult ArchivosSubidos(int id_encuesta, long swcreatedby, int id_Correlativo, int id_pregunta)
        {
            List<Tbl_EncResp_Encuesta_Documento> tbl_EncResp_Encuesta_Documentos = (from d in db_Survey.Tbl_EncResp_Encuesta_Documento
                                                                                    where d.id_encuesta == id_encuesta
                                                                                    && d.swcreatedby == swcreatedby
                                                                                    && d.id_Correlativo == id_Correlativo
                                                                                    && d.id_pregunta == id_pregunta
                                                                                    orderby d.id_CorrelativoDocumento
                                                                                    select d).ToList();

            if (tbl_EncResp_Encuesta_Documentos == null)
            {
                tbl_EncResp_Encuesta_Documentos = new List<Tbl_EncResp_Encuesta_Documento>();
            }
            return View(tbl_EncResp_Encuesta_Documentos);
        }










        public string GenerarNombreDocumento(string Nombre_Archivo, string Ext, int id_CorrelativoDocumento)
        {
            return Nombre_Archivo.Replace(Ext, id_CorrelativoDocumento + Ext);
        }

        public void GuardarArchivo(HttpPostedFileBase upload, string PathCrear, string PathArchivo)
        {
            if (!Directory.Exists(Server.MapPath(PathCrear)))
            {
                Directory.CreateDirectory(Server.MapPath(PathCrear));
            }
            upload.SaveAs(PathArchivo);
        }


        public string ParsearUbicacionArchivo(string partialpath, string Nombre_Archivo)
        {
            return Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);
        }

        public JsonResult ArchivoAgregar(Tbl_EncResp_Encuesta_Documento model, HttpPostedFileBase upload)
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


            bool ErrorDetectado = false;
            int id_CorrelativoDocumento = 0;


            try
            {
                id_CorrelativoDocumento = db_Survey.Tbl_EncResp_Encuesta_Documento.Where(Obj =>
                    Obj.id_encuesta == model.id_encuesta
                    && Obj.swcreatedby == model.swcreatedby
                    && Obj.id_Correlativo == model.id_Correlativo
                    && Obj.id_pregunta == model.id_pregunta)
                    .Max(Obj => Obj.id_CorrelativoDocumento);
            }
            catch (Exception ex)
            {
                id_CorrelativoDocumento = 0;
            }

            id_CorrelativoDocumento++;


            string partialpath = "~/Encuesta/" + model.id_encuesta + "/" + model.swcreatedby + "/" + model.id_Correlativo + "/" + model.id_pregunta + "/";
            string Nombre_Archivo = "";
            string Ext = "";
            try
            {
                Nombre_Archivo = upload.FileName;
                Ext = Path.GetExtension(Nombre_Archivo).ToLower();
                Nombre_Archivo = Nombre_Archivo.Replace("-", "");
                Nombre_Archivo = Nombre_Archivo.Replace("(", "");
                Nombre_Archivo = Nombre_Archivo.Replace(")", "");
                Nombre_Archivo = Nombre_Archivo.Replace(" ", "");
                Nombre_Archivo = GenerarNombreDocumento(Nombre_Archivo, Ext, id_CorrelativoDocumento);//Nombre_Archivo.Replace(Ext, id_CorrelativoDocumento + Ext);


                partialpath = partialpath.Replace(".", "_");


                string PathArchivo = ParsearUbicacionArchivo(partialpath, Nombre_Archivo);//Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);
                string PathCrear = partialpath;

                if (!System.IO.File.Exists(PathArchivo))
                {
                    GuardarArchivo(upload, PathCrear, PathArchivo);
                    //if (!Directory.Exists(Server.MapPath(PathCrear)))
                    //{
                    //    Directory.CreateDirectory(Server.MapPath(PathCrear));
                    //}
                    //upload.SaveAs(PathCrear);
                }
                else
                {
                    System.IO.File.Delete(PathArchivo);
                    //La razón por la que el nombre se reescribe si el sistema detecta que aún existe este documento es simplemente para que el archivo agregado no tena inconvenientes a nivel de caché en el navegador, mucho cuidado con este dato
                    Nombre_Archivo = GenerarNombreDocumento(Nombre_Archivo, Ext, id_CorrelativoDocumento);
                    PathArchivo = ParsearUbicacionArchivo(partialpath, Nombre_Archivo);
                    PathCrear = partialpath;
                    GuardarArchivo(upload, PathCrear, PathArchivo);
                }



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
                model.id_CorrelativoDocumento = id_CorrelativoDocumento;
                model.Documento = Nombre_Archivo;
                model.NombreBase = upload.FileName;
                model.swdatecreated = DateTime.Now;
                model.Estado = true;
                db_Survey.Tbl_EncResp_Encuesta_Documento.Add(model);
                db_Survey.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Documento agregado exitosamente"
                };
            }



            return Json(jsonRespuesta);
        }

        public JsonResult ArchivoEliminar(Tbl_EncResp_Encuesta_Documento model)
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

            Tbl_EncResp_Encuesta_Documento tbl_EncResp_Encuesta_Documento = (from d in db_Survey.Tbl_EncResp_Encuesta_Documento
                                                                                where d.id_encuesta == model.id_encuesta
                                                                                   && d.swcreatedby == model.swcreatedby
                                                                                   && d.id_Correlativo == model.id_Correlativo
                                                                                   && d.id_pregunta == model.id_pregunta
                                                                                   && d.id_CorrelativoDocumento == model.id_CorrelativoDocumento
                                                                             select d).FirstOrDefault();

            if (tbl_EncResp_Encuesta_Documento != null)
            {

                string Nombre_Archivo = tbl_EncResp_Encuesta_Documento.Documento;

                string partialpath = "~/Encuesta/" + tbl_EncResp_Encuesta_Documento.id_encuesta + "/" + tbl_EncResp_Encuesta_Documento.swcreatedby + "/" + tbl_EncResp_Encuesta_Documento.id_Correlativo + "/" + tbl_EncResp_Encuesta_Documento.id_pregunta + "/";

                partialpath = partialpath.Replace(".", "_");


                string PathArchivo = ParsearUbicacionArchivo(partialpath, Nombre_Archivo);


                if (System.IO.File.Exists(PathArchivo))
                {
                    System.IO.File.Delete(PathArchivo);
                }




                db_Survey.Tbl_EncResp_Encuesta_Documento.Remove(tbl_EncResp_Encuesta_Documento);
                db_Survey.SaveChanges();



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



    }
}
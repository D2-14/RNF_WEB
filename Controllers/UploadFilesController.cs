using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class UploadFilesController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        // GET: UploadFiles
        public ActionResult UploadedFiles(long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosRequeridos(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            return View();

        }


        public ActionResult UploadedEnmiendas(string Guid_id)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            long solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosAnexoSecretaria(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            ViewBag.DocumentoASubir = new SelectList(db.fc_Sol_DocumentosAnexoSecretaria(solicitud_id), "Tipo_Documento_id", "Nombre");

            return View(tbl_Sol_Solicitud);


        }

        public ActionResult UploadedEnmiendasRespuesta(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            return View(tbl_gest_EtapaSolicitud);

        }


        public ActionResult UploadedAnexos(string Guid_id)
        {


            TempData["MensajeFile"] = "";

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            long solicitud_id = tbl_Sol_Solicitud.Solicitud_id;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosAnexoTecnico(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            ViewBag.DocumentoASubir = new SelectList(db.fc_Sol_DocumentosAnexoTecnico(solicitud_id), "Tipo_Documento_id", "Nombre");

            return View(tbl_Sol_Solicitud);


        }

        public ActionResult Index()
        {
            long Id = (long)Session[Constants.session_Solicitud];

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == Id);


            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }

        public ActionResult IndexEspecifico(int id, long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id == id);

            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = solicitud_id;

            return View(tbl_DoctoSubido.ToList());

        }

        public ActionResult IndexEspecificoAnexo(int id, long solicitudid)
        {
            long Id = solicitudid;

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Tipo_Documento_id == id);


            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }

        public ActionResult IndexEspecificoGUID(int id, string GUID)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == GUID).First();


            long Id = tbl_sol_solicitud.Solicitud_id; ;

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == Id && Obj.Tipo_Documento_id == id);


            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }

        [HttpPost]
        public Boolean UploadFile(HttpPostedFileBase archivo_a_grabar, int Tipo_De_Archivo, string Nombre_Archivo, long solicitud_id, string firma)
        {
            bool ErrorDetectado = false;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                ErrorDetectado = true;
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                ErrorDetectado = true;
            }


            Nombre_Archivo = Nombre_Archivo.Replace("-", "");
            Nombre_Archivo = Nombre_Archivo.Replace("(", "");
            Nombre_Archivo = Nombre_Archivo.Replace(")", "");
            Nombre_Archivo = Nombre_Archivo.Replace(" ", "");

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                ErrorDetectado = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            long idSol = solicitud_id;

            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

            if ((System.IO.File.Exists(PathArchivo)) == false)
            {
                if (ErrorDetectado == true)
                {
                    return true;
                }

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };



                string PathCrear = "~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo;

                string fileLocation = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

                if (!Directory.Exists(Server.MapPath(PathCrear)))
                    Directory.CreateDirectory(Server.MapPath(PathCrear));

                archivo_a_grabar.SaveAs(fileLocation);


                // Grabar en base de datosw

                int intIdt = 0;

                try
                {
                    intIdt = db.Tbl_Sol_DocumentoSubido.Where(DS => DS.Solicitud_id == idSol).Max(u => u.Documento_id);
                    intIdt++;

                }
                catch
                {
                    intIdt = 1;
                }

                Tbl_Sol_DocumentoSubido tbl_sol_DocumentoSubido = new Tbl_Sol_DocumentoSubido();

                tbl_sol_DocumentoSubido.Solicitud_id = idSol;
                tbl_sol_DocumentoSubido.Documento_id = intIdt;
                tbl_sol_DocumentoSubido.Tipo_Documento_id = Tipo_De_Archivo;
                tbl_sol_DocumentoSubido.FileName = Nombre_Archivo;


                tbl_sol_DocumentoSubido.swcreatedby = objUs.intUsuario_id;
                tbl_sol_DocumentoSubido.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_sol_DocumentoSubido.swcreatedbyinterno = false;
                }
                else
                {
                    tbl_sol_DocumentoSubido.swcreatedbyinterno = true;
                }

                db.Tbl_Sol_DocumentoSubido.Add(tbl_sol_DocumentoSubido);
                db.SaveChanges();

            }

            return true;

        }


        [HttpPost]
        public Boolean UploadFileAnexo(HttpPostedFileBase archivo_a_grabar, int Tipo_De_Archivo, string Nombre_Archivo, long solicitudid)
        {
            bool ErrorDetectado = false;

            Nombre_Archivo = Nombre_Archivo.Replace("-", "");
            Nombre_Archivo = Nombre_Archivo.Replace("(", "");
            Nombre_Archivo = Nombre_Archivo.Replace(")", "");
            Nombre_Archivo = Nombre_Archivo.Replace(" ", "");

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                ErrorDetectado = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            long idSol = solicitudid;

            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

            if ((System.IO.File.Exists(PathArchivo)) == false)
            {
                if (ErrorDetectado == true)
                {
                    return true;
                }

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };



                string PathCrear = "~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo;

                string fileLocation = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

                if (!Directory.Exists(Server.MapPath(PathCrear)))
                    Directory.CreateDirectory(Server.MapPath(PathCrear));

                archivo_a_grabar.SaveAs(fileLocation);


                // Grabar en base de datosw

                int intIdt = 0;

                try
                {
                    intIdt = db.Tbl_Sol_DocumentoSubido.Where(DS => DS.Solicitud_id == idSol).Max(u => u.Documento_id);
                    intIdt++;

                }
                catch
                {
                    intIdt = 1;
                }

                Tbl_Sol_DocumentoSubido tbl_sol_DocumentoSubido = new Tbl_Sol_DocumentoSubido();

                tbl_sol_DocumentoSubido.Solicitud_id = idSol;
                tbl_sol_DocumentoSubido.Documento_id = intIdt;
                tbl_sol_DocumentoSubido.Tipo_Documento_id = Tipo_De_Archivo;
                tbl_sol_DocumentoSubido.FileName = Nombre_Archivo;


                tbl_sol_DocumentoSubido.swcreatedby = objUs.intUsuario_id;
                tbl_sol_DocumentoSubido.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_sol_DocumentoSubido.swcreatedbyinterno = false;
                }
                else
                {
                    tbl_sol_DocumentoSubido.swcreatedbyinterno = true;
                }

                db.Tbl_Sol_DocumentoSubido.Add(tbl_sol_DocumentoSubido);
                db.SaveChanges();

            }

            return true;

        }




        public string ParsearUbicacionArchivo(string partialpath, string Nombre_Archivo)
        {
            return Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);
        }


        [HttpPost]
        public JsonResult Obtenerhref(int id, string varfilename, long solicitud_id, string firma)
        {

            long idSol = solicitud_id;
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(idSol);
            Tbl_Sol_DocumentoSubido img = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == varfilename).First();

            if (img != null)
            {

                string path = "";

                if ((tbl_Sol_Solicitud.No_Registro ?? "").Trim() != "")
                {
                    path = "/Archivos_Subidos/" + tbl_Sol_Solicitud.No_Registro + "/" + img.Tipo_Documento_id + "/";
                    string revisar = "";
                    revisar = ParsearUbicacionArchivo(path, img.FileName);
                    if (!System.IO.File.Exists(revisar))
                    {
                        path = "/Archivos_Subidos/" + tbl_Sol_Solicitud.Solicitud_id.ToString() + "/" + img.Tipo_Documento_id + "/";
                    }

                }
                else
                {
                    path = "/Archivos_Subidos/" + tbl_Sol_Solicitud.Solicitud_id.ToString() + "/" + img.Tipo_Documento_id + "/";
                }

                string PathArchivo = path;
                string fileLocation = PathArchivo + img.FileName;

                return Json(fileLocation);

            }
            else
            {
                return Json(null);
            }
        }



        [HttpPost]
        public JsonResult ObtenerhrefAnexo(int id, string varfilename, long solicitudid)
        {

            long idSol = solicitudid;

            Tbl_Sol_DocumentoSubido img = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == varfilename).First();


            string PathArchivo = "/Archivos_Subidos/" + idSol.ToString() + "/" + img.Tipo_Documento_id + "/";


            if (img.FileName.Split('.')[1] == "pdf" || img.FileName.Split('.')[1] == "PDF" ||
                img.FileName.Split('.')[1] == "jpg" || img.FileName.Split('.')[1] == "JPG" ||
                img.FileName.Split('.')[1] == "bmp" || img.FileName.Split('.')[1] == "BMP" ||
                img.FileName.Split('.')[1] == "png" || img.FileName.Split('.')[1] == "PNG" ||
                img.FileName.Split('.')[1] == "gif" || img.FileName.Split('.')[1] == "GIF")
            {

                string fileLocation = PathArchivo + img.FileName;

                return Json(fileLocation);


            }
            else
            {
                string fileLocation = PathArchivo + img.FileName;

                return Json(fileLocation);
            }
        }


        //  filename

        [HttpPost]
        public JsonResult BorrarArchivo(int id, string filename, long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return Json("");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return Json("");
            }

            long idSol = solicitud_id;

            string PathArchivo = "~/Archivos_Subidos/" + idSol.ToString() + "/" + id + "/" + filename;

            PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + id + "/"), filename);
            try
            {
                if (System.IO.File.Exists(PathArchivo))
                {
                    Tbl_Sol_DocumentoSubido DocumentoSubido = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == filename).First();
                    db.Tbl_Sol_DocumentoSubido.Remove(DocumentoSubido);
                    System.IO.File.Delete(PathArchivo);
                    db.SaveChanges();

                }
                else
                {
                    Tbl_Sol_DocumentoSubido DocumentoSubido = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == filename).First();
                    db.Tbl_Sol_DocumentoSubido.Remove(DocumentoSubido);
                    db.SaveChanges();
                    //System.IO.File.Delete(PathArchivo);

                }
                Task.WaitAll(Task.Delay(1000));

            }
            catch (Exception ex)
            {
                PathArchivo = "";
            }
            return Json(PathArchivo);
        }


        [HttpPost]
        public JsonResult BorrarArchivoAnexo(long solicitudid, int id, string filename)
        {

            long idSol = solicitudid;

            string PathArchivo = "~/Archivos_Subidos/" + idSol.ToString() + "/" + id + "/" + filename;

            PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + idSol.ToString() + "/" + id + "/"), filename);
            try
            {
                if (System.IO.File.Exists(PathArchivo))
                {
                    Tbl_Sol_DocumentoSubido DocumentoSubido = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == filename).First();
                    db.Tbl_Sol_DocumentoSubido.Remove(DocumentoSubido);
                    System.IO.File.Delete(PathArchivo);
                    db.SaveChanges();

                }
                else
                {
                    Tbl_Sol_DocumentoSubido DocumentoSubido = db.Tbl_Sol_DocumentoSubido.Where(obj => obj.Solicitud_id == idSol && obj.Tipo_Documento_id == id && obj.FileName == filename).First();
                    db.Tbl_Sol_DocumentoSubido.Remove(DocumentoSubido);
                    db.SaveChanges();
                    //System.IO.File.Delete(PathArchivo);

                }
                Task.WaitAll(Task.Delay(1000));

            }
            catch (Exception ex)
            {
                PathArchivo = "";
            }
            return Json(PathArchivo);
        }


        public ActionResult RetunKMZ(long Rodal_id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Session[Constants.session_Rodal] = (long)Rodal_id;

            ViewBag.KLM = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_KML(@p0,@p1)", Session[Constants.session_Solicitud], Rodal_id).FirstOrDefault();

            DateTime hoy = DateTime.Now;
            string fecha = hoy.Millisecond + "_" + hoy.Minute + "_" + hoy.Day + "_" + hoy.Month + "_" + hoy.Year;
            string strNombre = @"KLM_Rodal_" + fecha + ".kml";

            ViewBag.FileName = strNombre;
            return View();
        }

        public ActionResult UploadFile()
        {
            return View();
        }


        public JsonResult ConsultarDocumentosRequeridos(Tbl_Sol_Solicitud model)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == model.Guid_id).FirstOrDefault();


            List<fc_Sol_DocumentosRequeridos_Result> fc_Sol_DocumentosRequeridos_Results = new List<fc_Sol_DocumentosRequeridos_Result>();

            if (tbl_Sol_Solicitud != null)
            {
                fc_Sol_DocumentosRequeridos_Results = ((from d in db.fc_Sol_DocumentosRequeridos(tbl_Sol_Solicitud.Solicitud_id)
                                                        where d.Tipo_Documento_id > 0
                                                        orderby d.Tipo_Documento_id
                                                        select d).ToList() ?? new List<fc_Sol_DocumentosRequeridos_Result>());
            }


            return Json(fc_Sol_DocumentosRequeridos_Results);
        }


    }
}
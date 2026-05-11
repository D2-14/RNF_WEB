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
    public class RNF_UploadFilesController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_UploadFiles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadedFiles(string Guid_id)
        {

            ViewBag.Guid_id = Guid_id;

            ViewBag.DocumentosPendientes = db.fc_RNF_DocumentosRequeridos(Guid_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            ViewBag.DocumentoASubir = new SelectList(db.fc_RNF_DocumentosRequeridos(Guid_id), "Tipo_Documento_id", "Nombre");

            return View();


        }

        public ActionResult IndexEspecifico(string Guid_id, int id)
        {
            var tbl_DoctoSubido = db.Tbl_RNF_DocumentoSubido.Where(Obj => Obj.No_Registro == Guid_id && Obj.Tipo_Documento_id == id);

            ViewBag.Guid_id = Guid_id;
            return View(tbl_DoctoSubido.ToList());

        }


        [HttpPost]
        public Boolean UploadFile(HttpPostedFileBase archivo_a_grabar, string No_Registro, int Tipo_De_Archivo, string Nombre_Archivo)
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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + tbl_RNF_Registro.No_Registro + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

            if ((System.IO.File.Exists(PathArchivo)) == false)
            {
                if (ErrorDetectado == true)
                {
                    return true;
                }

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };



                string PathCrear = "~/Archivos_Subidos/" + tbl_RNF_Registro.No_Registro + "/" + Tipo_De_Archivo;

                string fileLocation = Path.Combine(Server.MapPath("~/Archivos_Subidos/" + tbl_RNF_Registro.No_Registro + "/" + Tipo_De_Archivo + "/"), Nombre_Archivo);

                if (!Directory.Exists(Server.MapPath(PathCrear)))
                    Directory.CreateDirectory(Server.MapPath(PathCrear));

                archivo_a_grabar.SaveAs(fileLocation);


                // Grabar en base de datosw

                int intIdt = 0;

                try
                {
                    intIdt = db.Tbl_RNF_DocumentoSubido.Where(DS => DS.No_Registro == No_Registro).Max(u => u.Documento_id);
                    intIdt++;

                }
                catch
                {
                    intIdt = 1;
                }

                Tbl_RNF_DocumentoSubido tbl_RNF_DocumentoSubido = new Tbl_RNF_DocumentoSubido();

                tbl_RNF_DocumentoSubido.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_DocumentoSubido.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_DocumentoSubido.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_DocumentoSubido.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_DocumentoSubido.Documento_id = intIdt;
                tbl_RNF_DocumentoSubido.Tipo_Documento_id = Tipo_De_Archivo;
                tbl_RNF_DocumentoSubido.FileName = Nombre_Archivo;


                tbl_RNF_DocumentoSubido.swcreatedby = objUs.intUsuario_id;
                tbl_RNF_DocumentoSubido.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_RNF_DocumentoSubido.swcreatedbyinterno = false;
                }
                else
                {
                    tbl_RNF_DocumentoSubido.swcreatedbyinterno = true;
                }

                db.Tbl_RNF_DocumentoSubido.Add(tbl_RNF_DocumentoSubido);
                db.SaveChanges();

            }

            return true;

        }

        public string ParsearUbicacionArchivo(string partialpath, string Nombre_Archivo)
        {
            return Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);
        }

        [HttpPost]
        public JsonResult Obtenerhref(string Guid_id, int id, string varfilename)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();
            Tbl_RNF_DocumentoSubido img = db.Tbl_RNF_DocumentoSubido.Where(obj => obj.No_Registro == Guid_id && obj.Tipo_Documento_id == id && obj.FileName == varfilename).FirstOrDefault();

            bool procedenciaexterna = false;

            if (img != null)
            {

                bool swcreatedbyinterno = img.swcreatedbyinterno ?? false;

                string path = "";


                path = "/Archivos_Subidos/" + img.No_Registro + "/" + img.Tipo_Documento_id + "/";
                string revisar = "";
                revisar = ParsearUbicacionArchivo(path, img.FileName);
                if (!System.IO.File.Exists(revisar))
                {
                    path = "/Archivos_Subidos/" + img.Solicitud_id.ToString() + "/" + img.Tipo_Documento_id + "/";
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
        public JsonResult BorrarArchivo(string No_Registro, int id, string filename)
        {

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_DocumentoSubido DocumentoSubido = db.Tbl_RNF_DocumentoSubido.Where(obj => obj.No_Registro == No_Registro && obj.Tipo_Documento_id == id && obj.FileName == filename).FirstOrDefault();

            string path = "";
            if (DocumentoSubido != null)
            {
                bool swcreatedbyinterno = (bool)DocumentoSubido.swcreatedbyinterno;
                if (swcreatedbyinterno)
                {
                    path = "~/Archivos_Subidos/" + tbl_RNF_Registro.No_Registro + "/" + id + "/" + filename;
                }
                else
                {
                    path = "~/Archivos_Subidos/" + tbl_RNF_Registro.Solicitud_id.ToString() + "/" + id + "/" + filename;
                }
            }

            string PathArchivo = path;

            PathArchivo = Path.Combine(Server.MapPath(PathArchivo), filename);
            try
            {
                if (System.IO.File.Exists(PathArchivo))
                {
                    System.IO.File.Delete(PathArchivo);
                }
                db.Tbl_RNF_DocumentoSubido.Remove(DocumentoSubido);
                db.SaveChanges();
                //System.IO.File.Delete(PathArchivo);
                Task.WaitAll(Task.Delay(1000));

            }
            catch (Exception ex)
            {
                PathArchivo = "";
            }





            return Json(PathArchivo);
        }

        public JsonResult ConsultarDocumentosRequeridos(Tbl_RNF_Registro model)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();


            List<fc_RNF_DocumentosRequeridos_Result> fc_RNF_DocumentosRequeridos_Results = new List<fc_RNF_DocumentosRequeridos_Result>();

            if (tbl_RNF_Registro != null)
            {
                fc_RNF_DocumentosRequeridos_Results = ((from d in db.fc_RNF_DocumentosRequeridos(tbl_RNF_Registro.No_Registro)
                                                        where d.Tipo_Documento_id > 0
                                                        orderby d.Tipo_Documento_id
                                                        select d).ToList() ?? new List<fc_RNF_DocumentosRequeridos_Result>());
            }


            return Json(fc_RNF_DocumentosRequeridos_Results);
        }



    }
}
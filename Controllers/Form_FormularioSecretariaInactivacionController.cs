using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSecretariaInactivacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AgregarHallazgo(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();


            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            if ((tbl_Sol_Solicitud.No_Registro == null) || (tbl_Sol_Solicitud.No_Registro.Trim() == ""))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.GuidEtapa_id = tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id;
            ViewBag.No_Registro = tbl_Sol_Solicitud.No_Registro;
            ViewBag.Bitacora_id = tbl_Sol_Solicitud.Bitacora_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                 select d).FirstOrDefault();

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();


            if (tbl_RNF_Registro_Bitacora == null)
            {
                long Bitacora_id = 0;
                try
                {
                    Bitacora_id = db.Tbl_RNF_Registro_Bitacora.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).Max(Obj => Obj.Bitacora_id);
                }
                catch
                {
                    Bitacora_id = 0;
                }
                Bitacora_id++;

                tbl_RNF_Registro_Bitacora = new Tbl_RNF_Registro_Bitacora()
                {
                    No_Registro = tbl_Sol_Solicitud.No_Registro,
                    No_RegistroLiteral = tbl_Sol_Solicitud.No_RegistroLiteral,
                    No_RegistroCorrelativo = (int)tbl_Sol_Solicitud.No_RegistroCorrelativo,
                    Solicitud_id = tbl_Sol_Solicitud.Solicitud_id,
                    Bitacora_id = Bitacora_id,
                    Motivo = ""
                };
                db.Tbl_RNF_Registro_Bitacora.Add(tbl_RNF_Registro_Bitacora);
                db.SaveChanges();

                tbl_Sol_Solicitud.Bitacora_id = Bitacora_id;
                db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Motivo = tbl_RNF_Registro_Bitacora.Motivo;


            Tbl_RNF_Registro_BitacoraHallazgo tbl_RNF_Registro_BitacoraHallazgo = new Tbl_RNF_Registro_BitacoraHallazgo()
            {
                No_Registro = tbl_RNF_Registro_Bitacora.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro_Bitacora.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro_Bitacora.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro_Bitacora.Solicitud_id,
                Bitacora_id = tbl_RNF_Registro_Bitacora.Bitacora_id
            };



            List<Tbl_RNF_Registro_InactivacionTecnico_Tipo> tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in db.Tbl_RNF_Registro_InactivacionTecnico_Tipo
                                                                                                          select d).ToList();

            tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                          where d.InactivacionTecnicoTipo_id != 0
                                                          select d).ToList();

            List<Tbl_RNF_Registro_Bitacora> tbl_RNF_Registro_Bitacoras = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                          where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                          select d).ToList();

            if (tbl_RNF_Registro_Bitacoras == null)
            {
                tbl_RNF_Registro_Bitacoras = new List<Tbl_RNF_Registro_Bitacora>();
            }

            int CountInactivacionTemporal = 0;
            int CountInactivacionDefinitiva = 0;

            foreach (var item in tbl_RNF_Registro_Bitacoras)
            {
                item.InactivacionTemporal = item.InactivacionTemporal ?? false;
                item.InactivacionDefinitiva = item.InactivacionDefinitiva ?? false;
            }

            CountInactivacionTemporal = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionTemporal == true).Count();
            CountInactivacionDefinitiva = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionDefinitiva == true).Count();


            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion", (tbl_Sol_Solicitud.InactivacionTecnicoTipo_id ?? tbl_RNF_Registro_InactivacionTecnico_Tipos.FirstOrDefault().InactivacionTecnicoTipo_id));

            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id).ToList();
            if (tbl_Sol_Solicitud.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");

            return View(tbl_RNF_Registro_BitacoraHallazgo);
        }


        public ActionResult DocumentosCambioEstado(string No_Registro, long Bitacora_id)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            ViewBag.No_Registro = No_Registro;
            ViewBag.Bitacora_id = Bitacora_id;

            List<Tbl_RNF_Registro_BitacoraDocumento> tbl_RNF_Registro_BitacoraDocumentos = (from d in db.Tbl_RNF_Registro_BitacoraDocumento
                                                                                            where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                            && d.Bitacora_id == Bitacora_id
                                                                                            orderby d.Bitacora_id, d.Documento_id
                                                                                            select d).ToList();
            if (tbl_RNF_Registro_BitacoraDocumentos == null)
            {
                tbl_RNF_Registro_BitacoraDocumentos = new List<Tbl_RNF_Registro_BitacoraDocumento>();
            }

            ViewBag.MensajeDocumento = "";

            return View(tbl_RNF_Registro_BitacoraDocumentos);
        }



        [HttpPost]
        public JsonResult AgregarDocumentoBitacora(Tbl_RNF_Registro_BitacoraDocumento model, HttpPostedFileBase upload)
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



            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == model.No_Registro
                                                                   && d.Bitacora_id == model.Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Bitácora no se ha encontrado"
                };
                return Json(jsonRespuesta);
            }

            ViewBag.No_Registro = model.No_Registro;
            ViewBag.Bitacora_id = model.Bitacora_id;
            ViewBag.MensajeDocumento = "";

            long Documento_id = 0;

            try
            {
                Documento_id = db.Tbl_RNF_Registro_BitacoraDocumento.Where(Obj => Obj.No_Registro == tbl_RNF_Registro_Bitacora.No_Registro && Obj.Solicitud_id == tbl_RNF_Registro_Bitacora.Solicitud_id && Obj.Bitacora_id == model.Bitacora_id).Max(Obj => Obj.Documento_id);
            }
            catch (Exception ex)
            {
                Documento_id = 0;
            }

            Documento_id++;

            model.Documento_id = Documento_id;
            model.swcreatedby = objUs.intUsuario_id;
            model.swcreatedbyinterno = EsInterno;
            model.swdatecreated = DateTime.Now;

            string partialpath = "~/Archivos_Subidos/" + model.No_Registro + "/Bitacora_" + model.Bitacora_id + "/";

            string Nombre_Archivo = "";



            if (!ErrorDetectado)
            {
                try
                {
                    Nombre_Archivo = upload.FileName;
                    Nombre_Archivo = Nombre_Archivo.Replace("-", "");
                    Nombre_Archivo = Nombre_Archivo.Replace("(", "");
                    Nombre_Archivo = Nombre_Archivo.Replace(")", "");
                    Nombre_Archivo = Nombre_Archivo.Replace(" ", "");
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

                    model.NombreArchivo = Nombre_Archivo;
                    ViewBag.MensajeDocumento += "Archivo subido con éxito";
                }
                catch (Exception ex)
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 3,
                        Mensaje = "No se pudo registrar el documento" + ex.Message
                    };
                    return Json(jsonRespuesta);
                }

            }


            if (ModelState.IsValid && !ErrorDetectado)
            {
                model.No_RegistroLiteral = tbl_RNF_Registro_Bitacora.No_RegistroLiteral;
                model.No_RegistroCorrelativo = tbl_RNF_Registro_Bitacora.No_RegistroCorrelativo;
                model.Solicitud_id = tbl_RNF_Registro_Bitacora.Solicitud_id;



                db.Tbl_RNF_Registro_BitacoraDocumento.Add(model);
                db.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Documento agregado exitosamente"
                };

            }
            else
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 4,
                    Mensaje = "Ocurrió un error"
                };
            }

            return Json(jsonRespuesta);
        }

        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

    }
}
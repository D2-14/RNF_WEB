using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Font = iTextSharp.text.Font;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using RNF_Web.Models;
using System.IO;
using System.Data.SqlClient;
using RestSharp;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoInactivacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        PdfPTable tableTitulo = new PdfPTable(3);
        PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        PdfPTable tableDatosInstrucciones = new PdfPTable(numColumns: 8);
        PdfPTable tableDatosEvaluacion = new PdfPTable(numColumns: 8);
        PdfPTable tablePreguntasRespuestas = new PdfPTable(numColumns: 8);
        PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        PdfPTable tablePersoneria = new PdfPTable(1);
        PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        PdfPTable tableBanner = new PdfPTable(1);
        int Al_Izquierda = Element.ALIGN_LEFT;
        int Al_Centro = Element.ALIGN_CENTER;
        int Al_Derecha = Element.ALIGN_RIGHT;
        int Al_Justificado = Element.ALIGN_JUSTIFIED;
        int Al_Arriba = Element.ALIGN_TOP;
        int Al_Abajo = Element.ALIGN_BOTTOM;
        int Al_Medio = Element.ALIGN_MIDDLE;
        int Al_JustificadoTodo = Element.ALIGN_JUSTIFIED_ALL;
        int Al_NoDefinido = Element.ALIGN_UNDEFINED;
        BaseColor GrisClaro = BaseColor.LIGHT_GRAY;
        BaseColor Blanco = BaseColor.WHITE;

        iTextSharp.text.Font fntTituloTabla_10 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_11 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_12 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_13 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_14 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        iTextSharp.text.Font fntTituloTabla_15 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);

        iTextSharp.text.Font fntTituloTabla_10B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_11B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_12B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_13B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_14B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        iTextSharp.text.Font fntTituloTabla_15B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);



        // GET: Form_FormularioTecnicoInactivacion
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
                                                          where d.InactivacionTecnicoTipo_id != 0 & d.InactivacionTecnicoTipo_id !=1
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

            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion", (tbl_Sol_Solicitud.InactivacionTecnicoTipo_id ?? tbl_RNF_Registro_InactivacionTecnico_Tipos.FirstOrDefault().InactivacionTecnicoTipo_id));

            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id).ToList();
            if(tbl_Sol_Solicitud.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");


            List<Tbl_RNF_Registro_Inactivacion_Tipo> TipoInactivacion_id = db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && Obj.UsuarioExterno == true).ToList();


            ViewBag.TipoInactivacion_id = new SelectList(TipoInactivacion_id, "TipoInactivacion_id", "Descripcion", tbl_Sol_Solicitud.TipoInactivacion_id ?? 0);
            decimal tipogestion, procesoinactivacion;
            tipogestion = 0;
            procesoinactivacion = 0.04M;
            bool mostrar = false;
            decimal truncsolicitudtipoid = Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            decimal solicitudtipoid = tbl_Sol_Solicitud.SolicitudTipo_id - truncsolicitudtipoid;

            if (solicitudtipoid == 0.04M)
            {
                mostrar = true;
            }
            ViewBag.mostrar = mostrar;

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


        class InactivarRNF
        {
            public int result { get; set; }
            public string message { get; set; }
            public string ubicacion { get; set; }
        }
        public JsonResult GrabarMotivoInactivacion(Tbl_Sol_Solicitud model)
        {

            InactivarRNF inactivarRNF = new InactivarRNF();
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (model.Guid_id != tbl_Sol_Solicitud.Guid_id)
            {
                inactivarRNF.result = 0;
                inactivarRNF.message = "Acceso denegado";
                inactivarRNF.ubicacion = "";
                return Json(JsonConvert.SerializeObject(inactivarRNF));

            }

            tbl_Sol_Solicitud.TipoInactivacion_id = model.TipoInactivacion_id;
            tbl_Sol_Solicitud.Descripcion_Inactivacion = model.Descripcion_Inactivacion;
            db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            inactivarRNF.result = 1;
            inactivarRNF.message = "Se ha realizado la inactivación";
            return Json(inactivarRNF);
        }




        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud
    (
        long solicitud_id,
        int etapa_id,
        decimal etaparuta_id,
        int correlativoetapa_id,
        string motivo,
        int respuestaid,
        string EtapaSolicitud_GUIDid
    )
        {

            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "El usuario no se encuentra logueado. Ingrese de nuevo al sistema.";

                string jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResultUsr);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);


            Gest_EtapaModel gest_EtapaModel = new Gest_EtapaModel();
            ResultFromStoreProcedure Respuesta = gest_EtapaModel.ConfirmarRespuesta(objUs, solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid);
            //if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
            if (Respuesta.respuesta == 1)
            {


                codRespuesta = 1;
                strRespuesta = "Se ha notificado la respuesta.";


            }
            else
            {
                codRespuesta = 0;
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
                if ((Respuesta.mensaje ?? "").Trim() != "")
                {
                    strRespuesta = Respuesta.mensaje;
                }
            }

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);

        }



        public int ConfirmarRespuesta(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid)
        {
            Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.Respuesta_id != 0)
            {
                return 0;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return 0;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapasolicitud.Motivo = strMotivo;

            if (ModelState.IsValid)
            {

                tbl_gest_etapasolicitud.swupdatedby = objUs.intUsuario_id;
                tbl_gest_etapasolicitud.swdateupdated = DateTime.Now;

                if (objUs.EsInterno != 1)
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = true;

                }

                db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                db.SaveChanges();

            }


            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return resultado[0].respuesta;

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

        private void LlenaDos_UnoTextosDosResaltadoUnidos(string TextoA, string TextoB)
        {
            iTextSharp.text.Font fntTituloTablaSimple = fntTituloTabla_11;
            iTextSharp.text.Font fntTituloTablaResaltado = fntTituloTabla_11B;

            tableTitulo = new PdfPTable(3);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTablaSimple));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTablaResaltado));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);

            return;
        }

        private void FirmaSolicitante(Tbl_Sol_Solicitud tbl_sol_Solicitud)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 11);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.Rowspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            iTextSharp.text.Image logo;

            ///   Firma
            try
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/FirmaDigital_" + objUs.intUsuario_id.ToString() + ".png"));

            }
            catch (Exception excep)
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/rubrica.png"));
            }

            logo.ScaleAbsolute(75.0F, 50.0F);

            //c1 = new PdfPCell(logo);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Colspan = 4;
            c1.Rowspan = 3;
            c1.Border = 0;
            //c1.Border = PdfPCell.BOTTOM_BORDER;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableFirmaSolicitante.AddCell(c1);

            ///   Firma

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.Rowspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            string strNombreDelTecnico = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", tbl_sol_Solicitud.TecnicoAsignado_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase($"{strNombreDelTecnico}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" Técnico forestal", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            return;
        }


        private void FirmaUsuarioSession(Tbl_Sol_Solicitud tbl_sol_Solicitud, string InactivacionSolicitado)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 11);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            //Linea No. 0

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.Rowspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            iTextSharp.text.Image logo;

            ///   Firma
            try
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/FirmaDigital_" + objUs.intUsuario_id.ToString() + ".png"));

            }
            catch (Exception excep)
            {
                logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Archivos_ConFirmaElectronica/FirmaDigital/rubrica.png"));
            }

            logo.ScaleAbsolute(75.0F, 50.0F);

            //c1 = new PdfPCell(logo);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));

            c1.Colspan = 4;
            c1.Rowspan = 3;
            c1.Border = 0;
            //c1.Border = PdfPCell.BOTTOM_BORDER;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableFirmaSolicitante.AddCell(c1);

            ///   Firma

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.Rowspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);



            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            string strNombreDelTecnico = db.Database.SqlQuery<string>("Select dbo.Fnc_Seg_UsuarioNombre(@p0)", objUs.intUsuario_id).FirstOrDefault();


            c1 = new PdfPCell(new Phrase(objUs.strNombre_Usuario, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 6;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase(InactivacionSolicitado, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            return;
        }


        public string GeneraInformeTecnicoInactivacion_PDF(Tbl_Sol_Solicitud tbl_Sol_Solicitud, Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            string sqlQuery;
            Constants constants = new Constants();
            CrearBanner crearBanner = new CrearBanner();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = new Result_SP_IdentificadorOficialGestion();
            resultsp = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);

            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            string fechaDocumento = "";
            string nombreSubRegional = "";
            var Enter = new Paragraph(" ");


            strNombre = tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id + ".pdf";
            strDirArchivo = strFolder + strNombre;

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 50f, 50f);

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            decimal SolicitudTipo = tbl_Sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            int SolicitudTipoEntero = (int)Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            if (tbl_Sol_Solicitud.No_Registro == null)
            {
                return null;
            }

            if ((tbl_Sol_Solicitud.Bitacora_id == null) || (tbl_Sol_Solicitud.Bitacora_id == 0))
            {
                return null;
            }

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                   && d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora == null)
            {
                return null;
            }

            List<Tbl_RNF_Registro_BitacoraHallazgo> tbl_RNF_Registro_BitacoraHallazgos = (from d in db.Tbl_RNF_Registro_BitacoraHallazgo
                                                                                          where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                                          && d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                          && d.Bitacora_id == tbl_RNF_Registro_Bitacora.Bitacora_id
                                                                                          select d).ToList();

            if (tbl_RNF_Registro_BitacoraHallazgos == null)
            {
                tbl_RNF_Registro_BitacoraHallazgos = new List<Tbl_RNF_Registro_BitacoraHallazgo>();
            }

            fechaDocumento = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_FechaTxtSinGuatemala](getdate())").FirstOrDefault();
            nombreSubRegional = db.Database.SqlQuery<string>("select isnull([dbo].[Fnc_Gral_NombreSubDirectorRegional]('" + tbl_Sol_Solicitud.Region_id + "','" + tbl_Sol_Solicitud.SubRegion_id + "'),'')").FirstOrDefault();
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();

            try
            {

                // imagen de fondo

                string imagenMarcaAgua = Server.MapPath("~/Content/images/logoInab_VerticalSello.png");
                float posX, posY;
                Image image = Image.GetInstance(imagenMarcaAgua);
                PdfGState pdfGState = new PdfGState();
                pdfGState.FillOpacity = 0.23f;
                PdfContentByte pdfContentByte = writer.DirectContentUnder;
                //posX = (writer.PageSize.Right / 2) - (image.Width / 2);
                //posY = (writer.PageSize.Top / 2) - (image.Height / 2);
                //image.SetAbsolutePosition(posX, posY);
                image.SetAbsolutePosition(0, 0);

                //  imagen de fondo

                //LlenaTituloRevision(tbl_sol_Solicitud);
                //doc.Add(tableTitulo);
                //doc.Add(Enter);

                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    crearBanner.LlenaBanner(this.Url.Action(), "Izquierda", "Gris");
                    doc.Add(crearBanner.tableBanner);
                }
                string titulo = "INFORME TÉCNICO DE CANCELACIÓN";
                string codigo = "RF-RE-043";
                string version = "1";
                string fecha_implementacion = "Septiembre 2022";
                crearBanner.LlenaTituloRevision(titulo, codigo, version, fecha_implementacion, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);



                crearBanner.LlenaBanner("Informe No." + resultsp.Identificador, "Derecha", "");
                doc.Add(crearBanner.tableBanner);
                doc.Add(Enter);

                crearBanner.LlenaBanner(fechaDocumento, "Derecha", "");
                doc.Add(crearBanner.tableBanner);
                doc.Add(Enter);

                LlenaDos_UnoTextosDosResaltadoUnidos("Nombre del Director Subregional:", nombreSubRegional);
                doc.Add(tableTitulo);
                LlenaDos_UnoTextosDosResaltadoUnidos("Dirección Subregional:", tbl_Sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion);
                doc.Add(tableTitulo);
                doc.Add(Enter);

                crearBanner.LlenaBanner("Estimado Director Subregional, por medio del presente informe hago de su conocimiento lo siguiente: \n" + "", "Izquierda", "");
                doc.Add(crearBanner.tableBanner);
                doc.Add(Enter);

                LlenaDos_UnoTextosDosResaltadoUnidos("Código de Registro evaluado: ", tbl_Sol_Solicitud.No_Registro);
                doc.Add(tableTitulo);
                LlenaDos_UnoTextosDosResaltadoUnidos("Expediente  No:", tbl_Sol_Solicitud.Solicitud_NumeroExpediente);
                doc.Add(tableTitulo);
                doc.Add(Enter);

                if (tbl_RNF_Registro_BitacoraHallazgos.Count() > 0)
                {
                    long countHallazgos = 0;
                    crearBanner.LlenaBanner("Resultado:", "Izquierda", "");
                    doc.Add(crearBanner.tableBanner);

                    foreach (var item in tbl_RNF_Registro_BitacoraHallazgos)
                    {
                        if ((item.DescripcionHallazgo != null) && (item.DescripcionHallazgo.Trim() != ""))
                        {
                            countHallazgos++;
                            crearBanner.LlenaBanner(countHallazgos + ". " + item.DescripcionHallazgo.Trim(), "Izquierda", "");
                            doc.Add(crearBanner.tableBanner);
                        }
                    }
                    doc.Add(Enter);

                }

                //crearBanner.LlenaBanner("Dado el resultado se recomienda la " + tbl_Sol_Solicitud.Tbl_RNF_Registro_InactivacionTecnico_Tipo.Descripcion + " del Número de Registro: " + tbl_Sol_Solicitud.No_Registro + ", correspondiente a la subcategoria de: " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion, "Izquierda", "");
                //crearBanner.LlenaBanner("Dado el resultado se recomienda la " + (tbl_Sol_Solicitud.Tbl_RNF_Registro_InactivacionTecnico_Tipo.Descripcion ?? "") + " a causa de: " + (tbl_Sol_Solicitud.Tbl_RNF_Registro_Inactivacion_Tipo.Descripcion ?? "") + " del Número de Registro: " + (tbl_Sol_Solicitud.No_Registro ?? "") + ", correspondiente a la subcategoria de: " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion, "Izquierda", "");
                crearBanner.LlenaBanner("Dado el resultado se recomienda la " + (tbl_Sol_Solicitud.Tbl_RNF_Registro_InactivacionTecnico_Tipo.Descripcion ?? "") + " del Número de Registro: " + (tbl_Sol_Solicitud.No_Registro ?? "") + ", correspondiente a la subcategoria de: " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion, "Izquierda", "");
                doc.Add(crearBanner.tableBanner);
                doc.Add(Enter);
                doc.Add(Enter);


                //FirmaSolicitante(tbl_Sol_Solicitud);

                string SolicitadoPor = "";
                if((SolicitudTipoEntero >= 95) || (SolicitudTipoEntero <= 99))
                {
                    if(SolicitudTipoEntero == 95)
                    {
                        SolicitadoPor = "Administrador";
                    }
                    if (SolicitudTipoEntero == 96)
                    {
                        SolicitadoPor = "Director Regional";
                    }
                    if (SolicitudTipoEntero == 97)
                    {
                        SolicitadoPor = "Director SubRegional";
                    }
                    if (SolicitudTipoEntero == 98)
                    {
                        SolicitadoPor = "Secretaria";
                    }
                    if (SolicitudTipoEntero == 99)
                    {
                        SolicitadoPor = "Técnico Forestal";
                    }
                    //FirmaUsuarioSession(tbl_Sol_Solicitud, SolicitadoPor);
                    //doc.Add(tableFirmaSolicitante);
                }
                else
                {
                    FirmaSolicitante(tbl_Sol_Solicitud);
                    doc.Add(tableFirmaSolicitante);
                }



                doc.Close();
                writer.Close();
            }
            catch (Exception ex)
            {

                doc.Close();
                writer.Close();
            }


            return strNombre;

        }

        public JsonResult InformeInactivacion(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).FirstOrDefault();

            if (tbl_Sol_Solicitud != null)
            {
                if ((tbl_Sol_Solicitud.InactivacionTecnicoTipo_id == null) || (tbl_Sol_Solicitud.InactivacionTecnicoTipo_id == 0))
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 2,
                        Mensaje = "No se ha indicado el tipo de inactivación"
                    };
                    return Json(jsonRespuesta);
                }

                if ((tbl_Sol_Solicitud.Descripcion_InactivacionTecnico == null) || (tbl_Sol_Solicitud.Descripcion_InactivacionTecnico.Trim() == ""))
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 2,
                        Mensaje = "No se ha indicado el motivo de inactivación"
                    };
                    return Json(jsonRespuesta);

                }

                Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                       where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                       && d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                       && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                       select d).FirstOrDefault();

                if(tbl_RNF_Registro_Bitacora == null)
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 2,
                        Mensaje = "No se encontró la bitácora"
                    };
                    return Json(jsonRespuesta);

                }

                if(tbl_RNF_Registro_Bitacora.Tbl_RNF_Registro_BitacoraHallazgo.Count() == 0)
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 2,
                        Mensaje = "Debe registrar al menos una causal a la bitácora"
                    };
                    return Json(jsonRespuesta);
                }

                if (tbl_RNF_Registro_Bitacora.Tbl_RNF_Registro_BitacoraDocumento.Count() == 0)
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 2,
                        Mensaje = "Debe registrar al menos un documento a la bitácora"
                    };
                    return Json(jsonRespuesta);
                }

                Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).FirstOrDefault();
                if (tbl_Gest_EtapaSolicitud != null)
                {
                    string nombrearchivo = GeneraInformeTecnicoInactivacion_PDF(tbl_Sol_Solicitud, tbl_Gest_EtapaSolicitud);
                    if (nombrearchivo != null)
                    {
                        tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = nombrearchivo;
                        db.Entry(tbl_Gest_EtapaSolicitud).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        jsonRespuesta = new JsonRespuesta()
                        {
                            Result = 1,
                            Mensaje = "Documento generado exitosamente",
                            Ubicacion = tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado
                        };
                    }
                    else
                    {
                        jsonRespuesta = new JsonRespuesta()
                        {
                            Result = 4,
                            Mensaje = "Datos insuficientes"
                        };
                    }
                    return Json(jsonRespuesta);
                }
                else
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 3,
                        Mensaje = "Etapa no encontrada"
                    };
                    return Json(jsonRespuesta);
                }
            }
            else
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Solicitud no encontrada"
                };
                return Json(jsonRespuesta);
            }

            return Json(null);
        }


        class ResultRegistro
        {
            public int CodRespuesta { get; set; }
            public string StrRespuesta { get; set; }
            public string StrRegistro { get; set; }
        }
        public JsonResult GeneraInactivacionTecnico(long Solicitud_id, int InactivacionTecnico_Tipo_id, string Motivo, int TiempoInactivacion = 0)
        {
            ResultRegistro resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 0,
                StrRespuesta = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(resultRegistro);
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);


            if (tbl_Sol_Solicitud == null)
            {
                resultRegistro = new ResultRegistro()
                {
                    CodRespuesta = 2,
                    StrRespuesta = "No se encontró la solicitud"
                };
                return Json(resultRegistro);
            }

            tbl_Sol_Solicitud.InactivacionTecnicoTipo_id = InactivacionTecnico_Tipo_id;
            tbl_Sol_Solicitud.Descripcion_InactivacionTecnico = (Motivo ?? "");
            tbl_Sol_Solicitud.swdateupdated = DateTime.Now;
            tbl_Sol_Solicitud.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_Solicitud.swupdatedbyinterno = boolEsInterno;
            db.Entry(tbl_Sol_Solicitud).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                   && d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora != null)
            {
                tbl_RNF_Registro_Bitacora.InactivacionTemporal = false;
                tbl_RNF_Registro_Bitacora.InactivacionDefinitiva = false;

                if(InactivacionTecnico_Tipo_id == 1)
                {
                    tbl_RNF_Registro_Bitacora.InactivacionTemporal = true;
                }
                if (InactivacionTecnico_Tipo_id == 2)
                {
                    tbl_RNF_Registro_Bitacora.InactivacionDefinitiva = true;
                }

                if(TiempoInactivacion != 0)
                {
                    tbl_RNF_Registro_Bitacora.FechaInicioInactivacionTemporal = DateTime.Now;
                    tbl_RNF_Registro_Bitacora.FechaFinInactivacionTemporal = DateTime.Now.AddDays(TiempoInactivacion);
                }

                tbl_RNF_Registro_Bitacora.Motivo = Motivo;
                tbl_RNF_Registro_Bitacora.swdateupdated = DateTime.Now;
                tbl_RNF_Registro_Bitacora.swupdatedby = objUs.intUsuario_id;
                tbl_RNF_Registro_Bitacora.swupdatedbyinterno = boolEsInterno;
                db.Entry(tbl_RNF_Registro_Bitacora).State = EntityState.Modified;
                db.SaveChanges();
            }


            resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 1,
                StrRespuesta = "Se ha registrado el motivo y tipo"
            };



            return Json(resultRegistro);
        }








        public string GetBearer()
        {
            try
            {

                var client = new RestClient(Constants.Address_Bearer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string body = @"{
                        " + "\n" +
                            @"""Username"":""RNFUser"",
                        " + "\n" +
                            @"""Password"":""RNF_Password""
                        " + "\n" +
                            @"}
                        " + "\n" +
                            @"";

                body = body.Replace("RNFUser", Constants.RNF_Username);
                body = body.Replace("RNF_Password", Constants.RNF_Password);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.Content.ToString().Replace("\"", "").Length < 250)
                {
                    return "Error en credenciales RNF User  y RNF Password en la solicitud de bearer";
                }

                if (response.Content.ToString().Replace("\"", "").Length > 270)
                {
                    return "Error: " + response.Content.ToString().Replace("\"", "").Substring(1, 100);
                }
                else
                {
                    return "Bearer " + response.Content.ToString().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message.Substring(1, 100);
            }
        }

        class RNFCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public string CallCORS(string strSign, string strPath)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveUpload);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", strSign);
                //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
                request.AddFile("nombre", strPath, "application/pdf");
                request.AddParameter("parentGoogleDriveId", Constants.strParentGoogleDrive);
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                return joResponse["GoogleDriveId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error: No se logró subir el archivo a Google Drive, reporte a informatica. " + ex.Message.ToString();
            }
        }

        //public string firmarFile(string bearer, string strUsuarioFirma, string strUsuarioPassword, string strDocumento)
        //{


        //    try
        //    {
        //        var client = new RestClient(Constants.Address_FirmaElectronica);
        //        client.Timeout = -1;
        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("Authorization", bearer);
        //        request.AddHeader("Content-Type", "application/json");
        //        string body = @"{
        //        " + "\n" +
        //                        @"  ""User"": ""strUsuarioFirma"",
        //        " + "\n" +
        //                        @"  ""Password"": ""strUsuarioPassword"",
        //        " + "\n" +
        //                        @"  ""parentGoogleDriveId"":""strparentGoogleDriveId"",
        //        " + "\n" +
        //                        @"  ""Documentos"":[
        //        " + "\n" +
        //                        @"    {
        //        " + "\n" +
        //                        @"      ""GoogleDriveId"": ""strDocumento"",
        //        " + "\n" +
        //                        @"      ""Coordenadas"": ""70,700,180,760"",
        //        " + "\n" +
        //                        @"      ""NumeroPagina"": 1
        //        " + "\n" +
        //                        @"    }
        //        " + "\n" +
        //                        @" ]
        //        " + "\n" +
        //        @"}";

        //        //@"      ""Coordenadas"": ""260,1500,60,120"",


        //        body = body.Replace("strUsuarioFirma", strUsuarioFirma).Replace("strUsuarioPassword", strUsuarioPassword).Replace("strparentGoogleDriveId", Constants.parentGoogleDriveId);
        //        body = body.Replace("strDocumento", strDocumento);

        //        request.AddParameter("application/json", body, ParameterType.RequestBody);
        //        IRestResponse response = client.Execute(request);

        //        if (response.Content.ToString().IndexOf("Error") > 0)
        //        {
        //            return response.Content.ToString() + "  Usuario ó Password erroneo en firma.";
        //        }

        //        if (response.Content.ToString().IndexOf("connection") > 0)
        //        {
        //            return response.Content.ToString() + "  No hay conexion con el servidor de firmas." + response.Content.ToString();
        //        }

        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        try
        //        {
        //            Rootobject myDeserializedClass = JsonConvert.DeserializeObject<Rootobject>(response.Content.ToString());
        //            return myDeserializedClass.Data[0].GoogleDriveIdNuevo;
        //        }
        //        catch (Exception ex)
        //        {
        //            return strDocumento;
        //        }


        //        // Descomentar return myDeserializedClass.Data[0].GoogleDriveIdNuevo;

        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica

        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error al intentar firmar el archivo." + ex.Message.ToString();
        //    }


        //}

        public bool getFile(string strBearer, string strFile, string path)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveDownLoad + strFile);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", strBearer);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                byte[] buffer = client.DownloadData(request);

                MemoryStream ms = new MemoryStream(buffer);
                FileStream file = new FileStream(@path + strFile + ".pdf", FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }

            // IRestResponse response = client.Execute(request);

        }

        public JsonResult JsonProcesarFirmaElectronica(string Guid_id, string Guidetapa_id, string UsuarioFE, string PasswordFE)
        {
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;


            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };
            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);

            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

            /// bbarillas
            rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;
            /// bbarillas

            strBearer = GetBearer();

            if (strBearer.Length < 125)
            {
                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                return Json(jsonResultUsr);

            }

            string strDocumentoSubido = CallCORS(strBearer, rootpdf);

            if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
            {

                intRespuesta = 0;

                strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                return Json(jsonResultUsr);


            }

            RequestUtil requestUtil = new RequestUtil();

            string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);

            //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

            if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
            {


                // se cambio de 0 a 1 para pruebas de emananuel

                intRespuesta = 1;

                strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                return Json(jsonResultUsr);


            }


            if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
            {
                //Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                //db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                //db.SaveChanges();

                string SP_SqlQuery = "EXEC [dbo].[SP_GestEtapaSolicitud_ActualizaDocumentoFirmado] @Solicitud_id, @Firmante, @GUID_id, @EtapaSolicitud_GUID_id, @NombreDocumentoFirmado";
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                        new SqlParameter { ParameterName = "@Solicitud_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Firmante", Value = strEnc, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@GUID_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_Guid_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@EtapaSolicitud_GUID_id", Value = Tbl_Gest_etapaSolicitud.EtapaSolicitud_GUID_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@NombreDocumentoFirmado", Value = strDocumentofirmado + ".pdf", Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();



            }



            intRespuesta = resultFromStoreProcedure.respuesta;

            if (intRespuesta == 1)
            {
                Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + intRespuesta + "\","
                                  + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

            }
            else
            {
                jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + 0 + "\","
                                  + "\"strRespuesta\":" + "\"" + resultFromStoreProcedure.mensaje + "\"}";

            }


            //intRespuesta = 1;

            //jsonResultUsr = "{\"CodRespuesta\":"
            //                      + "\"" + intRespuesta + "\","
            //                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

            return Json(jsonResultUsr);
        }



    }
}
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
using System.Net.Mail;
using Newtonsoft.Json;
using iTextSharp.text.pdf.draw;
using System.Windows.Media;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSecretariaController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        private string glNombrePropietario;
        private string glNombreSolicitante;
        private string glNombreSecretaria;
        private static Dictionary<string,string> diccionarioVarialbes=new Dictionary<string, string>();

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


            //if (tbl_sol_solicitud.Solicitud_NumeroExpediente==null)
            //{
            //    strRespuesta = "Debe de Imprimir la constancia para generar correctamente el número de expediente.";

            //    string jsonResultUsr = "{\"CodRespuesta\":"
            //              + "\"" + codRespuesta + "\","
            //              + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            //    return Json(jsonResultUsr);
            //}

            bool ValidarRevisionSecretaria = false;

            if (tbl_sol_solicitud.SolicitudTipo_id == (decimal)2.00)
            {
                ValidarRevisionSecretaria = true;

                if ((etapa_id == 21) && (tbl_sol_solicitud.EnmiendasRecibidas != true))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede enviar la confirmación, sin imprimir la constancia de recepción al usuario.";

                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

            }

            if (
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)2.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)3.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)4.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)5.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)6.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)7.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)8.00) ||
                    (tbl_sol_solicitud.SolicitudTipo_id == (decimal)9.00)
               )
            {
                ValidarRevisionSecretaria = true;
            }

            if (ValidarRevisionSecretaria == true)
            {

                if (etapa_id == 1)
                {
                    // No considerar los Anexos y Enmiendas
                    // int intCantidadNoVerificados = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificado != true).Count();

                    int intCantidadNoVerificados = (from d in db.fc_Sol_DocumentosSubido(solicitud_id)
                                                    select d).Where(Obj => Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificado != true).Count();

                    if ((respuestaid == 1) && (intCantidadNoVerificados > 0))
                    {
                        codRespuesta = 0;
                        strRespuesta = "Error: Debe marcar como verificados los documentos, para continuar.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }

                    int intCantidadTodoList = db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id && Obj.Observaciones != null && Obj.Observaciones.Length > 1).Count();

                    if ((respuestaid == 2) && (intCantidadTodoList == 0))
                    {

                        codRespuesta = 0;
                        strRespuesta = "Error: En el área de observaciones debe indicar los errores encontrados, para poder guiar al usuario en la corrección.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }



                }

                if (etapa_id == 2)
                {
                    // No considerar los Anexos y Enmiendas

                    int intCantidadNoRecibidos = (from d in db.fc_Sol_DocumentosSubido(solicitud_id)
                                                  select d).Where(Obj => Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoRecibido != true).Count();

                    if ((respuestaid == 1) && (intCantidadNoRecibidos > 0))
                    {
                        codRespuesta = 0;
                        strRespuesta = "Error: Debe marcar los documentos como recibidos para continuar.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }
                }

                if ((etapa_id == 3) && (tbl_sol_solicitud.Solicitud_NumeroExpediente == null))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede enviar ningun expediente sin generar la constancia de recepción de documentos al usuario.";


                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

                if ((etapa_id == 3) && (tbl_sol_solicitud.Solicitud_NumeroExpediente == ""))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede enviar ningun expediente sin generar la constancia de recepción de documentos al usuario.";

                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

            }

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

        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitudDoctosVeificados
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

            bool ValidarRevisionSecretaria = false;

            //if (tbl_sol_solicitud.SolicitudTipo_id == (decimal)2.00)
            //{
            //    ValidarRevisionSecretaria = true;

            //    if ((etapa_id == 21) && (tbl_sol_solicitud.EnmiendasRecibidas != true))
            //    {

            //        codRespuesta = 0;
            //        strRespuesta = "Error: No puede enviar la confirmación, sin imprimir la constancia de recepción al usuario.";

            //        jsonResult = "{\"CodRespuesta\":"
            //        + "\"" + codRespuesta + "\","
            //        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            //        return Json(jsonResult);

            //    }

            //}

            //if (
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)2.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)3.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)4.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)5.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)6.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)7.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)8.00) ||
            //        (tbl_sol_solicitud.SolicitudTipo_id == (decimal)9.00)
            //   )
            //{
            //    ValidarRevisionSecretaria = true;
            //}

            ValidarRevisionSecretaria = true;

            if (ValidarRevisionSecretaria == true)
            {

                if (etapa_id == 1)
                {
                    // No considerar los Anexos y Enmiendas

                    //int intCantidadNoVerificados = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificado == false).Count();
                    //    intCantidadNoVerificados = intCantidadNoVerificados + db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoVerificado == null).Count();


                    int intCantidadNoVerificados = (from d in db.fc_Sol_DocumentosSubido(solicitud_id)
                                                    select d).Where(Obj => Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && (Obj.DocumentoVerificado == false || Obj.DocumentoVerificado == null)).Count();

                    if ((respuestaid == 1) && (intCantidadNoVerificados > 0))
                    {
                        codRespuesta = 0;
                        strRespuesta = "Error: Debe marcar como verificados los documentos, para continuar.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }

                    int intCantidadTodoList = db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id && Obj.Observaciones != null && Obj.Observaciones.Length > 1).Count();

                    if ((respuestaid == 2) && (intCantidadTodoList == 0))
                    {

                        codRespuesta = 0;
                        strRespuesta = "Error: En el área de observaciones debe indicar los errores encontrados, para poder guiar al usuario en la corrección.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }



                }

                if (etapa_id == 2)
                {
                    // No considerar los Anexos y Enmiendas

                    int intCantidadNoRecibidos = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Tipo_Documento_id != 19 && Obj.Tipo_Documento_id != 20 && Obj.DocumentoRecibido != true).Count();

                    if ((respuestaid == 1) && (intCantidadNoRecibidos > 0))
                    {
                        codRespuesta = 0;
                        strRespuesta = "Error: Debe marcar los documentos como recibidos para continuar.";

                        jsonResult = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                        return Json(jsonResult);

                    }
                }

                if ((etapa_id == 3) && (tbl_sol_solicitud.Solicitud_NumeroExpediente == null))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede enviar ningun expediente sin generar la constancia de recepción de documentos al usuario.";


                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

                if ((etapa_id == 3) && (tbl_sol_solicitud.Solicitud_NumeroExpediente == ""))
                {

                    codRespuesta = 0;
                    strRespuesta = "Error: No puede enviar ningun expediente sin generar la constancia de recepción de documentos al usuario.";

                    jsonResult = "{\"CodRespuesta\":"
                    + "\"" + codRespuesta + "\","
                    + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                    return Json(jsonResult);

                }

            }

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

        public ActionResult SubirEnmiendas(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;
            return View();
        }
        // GET: Form_FormularioSecretaria
        public ActionResult Index(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            long solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosRequeridos(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            //ViewBag.DocumentoASubir = new SelectList(db.fc_Sol_DocumentosRequeridos(solicitud_id), "Tipo_Documento_id", "Nombre");

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.Respuesta_id = tbl_gest_EtapaSolicitud.Respuesta_id;

            return View();


        }

        public ActionResult IndexRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

            //  Opciones posibles                                                                         //
            //  ==================                                                                        //
            //  1    Expediente virtual completo                                                          //
            //  2    Expediente virtual incompleto                                                        //
            //  3    Expediente ha superado la cantidad máxima de revisiones por parte de secretaría      //

            // Reglas  //
            //  Si  ViewBag.CantidadEtapasIniciales > 3    Denegada por exceder la cantidad de revisiones //

            ViewBag.BotonNombre = "Cuarta revisión (Inconvenientes encontrados)";

            if (ViewBag.CantidadEtapasIniciales == 1)
            {
                ViewBag.BotonNombre = "Primera revisión (Inconvenientes encontrados)";
            }

            if (ViewBag.CantidadEtapasIniciales == 2)
            {
                ViewBag.BotonNombre = "Segunda revisión (Inconvenientes encontrados)";
            }

            if (ViewBag.CantidadEtapasIniciales == 3)
            {
                ViewBag.BotonNombre = "Tercera revisión (Inconvenientes encontrados)";
            }
            return View(tbl_gest_EtapaSolicitud);

        }

        public ActionResult DocumentosRecibidos(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            long solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.DocumentosPendientes = db.fc_Sol_DocumentosRequeridos(solicitud_id).Where(ObjDocto => ObjDocto.Tipo_Documento_id > 0);

            ViewBag.DocumentoASubir = new SelectList(db.fc_Sol_DocumentosRequeridos(solicitud_id), "Tipo_Documento_id", "Nombre");

            return View();
        }

        public ActionResult DocumentosRecibidosRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {


            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();


            return View(tbl_gest_EtapaSolicitud);

        }

        [HttpPost]
        public JsonResult Obtenerhref(string GUID, int id, string varfilename)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == GUID).First();

            long idSol = tbl_sol_solicitud.Solicitud_id;

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

        [HttpPost]
        public JsonResult CambiarEstadoArchivoVerificado(Tbl_Sol_DocumentoSubido model)
        {
            long Solicitud_id;
            int Documento_id, Tipo_Documento_id;
            Solicitud_id = model.Solicitud_id;
            Documento_id = model.Documento_id;
            Tipo_Documento_id = model.Tipo_Documento_id;
            bool DocumentoVerificado;
            if (model.DocumentoVerificado == null)
            {
                DocumentoVerificado = true;
            }
            else
            {
                DocumentoVerificado = (bool)model.DocumentoVerificado;
            }
            string result;
            result = $"";

            try
            {
                Tbl_Sol_DocumentoSubido oTbl_Sol_DocumentoSubido = (from d in db.Tbl_Sol_DocumentoSubido
                                                                    where d.Solicitud_id == Solicitud_id && d.Documento_id == Documento_id && d.Tipo_Documento_id == Tipo_Documento_id
                                                                    select d).FirstOrDefault();

                oTbl_Sol_DocumentoSubido.DocumentoVerificado = DocumentoVerificado;
                result = $"Correcto";

                db.SaveChanges();
                return Json(result);
            }
            catch
            {
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CambiarEstadoArchivoRecibido(Tbl_Sol_DocumentoSubido model)
        {
            long Solicitud_id;
            int Documento_id, Tipo_Documento_id;
            Solicitud_id = model.Solicitud_id;
            Documento_id = model.Documento_id;
            Tipo_Documento_id = model.Tipo_Documento_id;
            bool DocumentoVerificado;
            if (model.DocumentoVerificado == null)
            {
                DocumentoVerificado = true;
            }
            else
            {
                DocumentoVerificado = (bool)model.DocumentoVerificado;
            }
            string result;
            result = $"";

            try
            {
                Tbl_Sol_DocumentoSubido oTbl_Sol_DocumentoSubido = (from d in db.Tbl_Sol_DocumentoSubido
                                                                    where d.Solicitud_id == Solicitud_id && d.Documento_id == Documento_id && d.Tipo_Documento_id == Tipo_Documento_id
                                                                    select d).FirstOrDefault();

                oTbl_Sol_DocumentoSubido.DocumentoRecibido = DocumentoVerificado;
                result = $"Correcto";

                db.SaveChanges();
                return Json(result);
            }
            catch
            {
                return Json(result);
            }
        }

        public ActionResult IndexEspecificoGUID(int id, string GUID)
        {


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == GUID).First();


            long Id = tbl_sol_solicitud.Solicitud_id;

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == Id && Obj.Tipo_Documento_id == id);

            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }

        public ActionResult IndexEspecificoRecepcionGUID(int id, string GUID)
        {


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == GUID).First();


            long Id = tbl_sol_solicitud.Solicitud_id;

            var tbl_DoctoSubido = db.Tbl_Sol_DocumentoSubido.Where(Obj => Obj.Solicitud_id == Id && Obj.Tipo_Documento_id == id);

            ViewBag.SolicitudLista = Session[Constants.session_SolicitudLista];

            ViewBag.lngSolicitud = Id;
            return View(tbl_DoctoSubido.ToList());

        }

        public ActionResult ImpresionConstancia(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)  /*(string Guid_id)*/
        {

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            if (tbl_sol_Solicitud.CantidadFolios == null)
            {
                tbl_sol_Solicitud.CantidadFolios = 0;
            }

            ViewBag.Guid_id = Guid_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            return View(tbl_sol_Solicitud);
        }

        //public ActionResult ImpresionConstanciaRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        //{

        //    ViewBag.Guid_id = Guid_id;

        //    Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

        //    Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

        //    ViewBag.CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

        //    //  Opciones posibles                                                                         //
        //    //  ==================                                                                        //
        //    //  1    Expediente listo para rivision de director Sub Regional                              //

        //    // Reglas  //
        //    //  Si  Debe tener asignado un número de expediente                                          //

        //    return View(tbl_gest_EtapaSolicitud);
        //}

        [HttpPost]
        public JsonResult GenerarConstanciaImprimirFormulario(string guidid, string GuidEtapa_id, string facturaserie, string facturanumero, int cantidadfolios)
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

            int RolSecretaria = db.Tbl_Gral_PerfilesRol.First().Secretaria ?? 0;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            string ExpedienteAsignado = identificadorOficialGestion.ObtenerNumeroDeExpediente(tbl_Sol_Solicitud.Solicitud_id, objUs.intUsuario_id).Identificador;

            tbl_Sol_Solicitud.Solicitud_NumeroExpediente = ExpedienteAsignado;

            db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            string TextoMostrar, Ubicacion;
            Ubicacion = GenerarReportePDF(guidid, GuidEtapa_id, facturaserie, facturanumero, cantidadfolios);

            TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";
            return Json(TextoMostrar);

        }

        [HttpPost]
        public JsonResult GenerarConstanciaImprimirFormularioSinCobro(string guidid, string GuidEtapa_id, string facturaserie, string facturanumero, int cantidadfolios,string Titular,string TitularRecepcionista)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

                    IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            // Ok

            string ExpedienteAsignado = identificadorOficialGestion.ObtenerNumeroDeExpediente(tbl_Sol_Solicitud.Solicitud_id, objUs.intUsuario_id).Identificador;

            tbl_Sol_Solicitud.Solicitud_NumeroExpediente = ExpedienteAsignado;

            db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            string TextoMostrar, Ubicacion;
            Ubicacion = GenerarReporteSinCobroPDF(guidid, GuidEtapa_id, cantidadfolios,Titular, TitularRecepcionista);

            TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";
            return Json(TextoMostrar);

        }

        [HttpPost]
        public JsonResult GenerarConstanciaImprimirFormularioConExoneracion(string guidid, string GuidEtapa_id, string facturaserie, string facturanumero, int cantidadfolios)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            string ExpedienteAsignado = identificadorOficialGestion.ObtenerNumeroDeExpediente(tbl_Sol_Solicitud.Solicitud_id, objUs.intUsuario_id).Identificador;

            tbl_Sol_Solicitud.Solicitud_NumeroExpediente = ExpedienteAsignado;

            db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            string TextoMostrar, Ubicacion;
            Ubicacion = GenerarReporteExoneradoPDF(guidid, GuidEtapa_id, cantidadfolios);

            TextoMostrar = "{ \"Ubicacion\" : \"" + Ubicacion + "\"}";
            return Json(TextoMostrar);

        }


        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosNotificacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);


        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

            tableTitulo = new PdfPTable(7);

            //// Imagen superior
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/images/logoInabExcel.jpg"));

            logo.ScalePercent(80f);

            PdfPCell c1 = new PdfPCell(logo);


            c1.Colspan = 2;
            c1.Rowspan = 4;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nCONSTANCIA DE RECEPCIÓN DE DOCUMENTOS" + " \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("REV-0.1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Agosto 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO:REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaTituloRevisionExoneracion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

            tableTitulo = new PdfPTable(7);

            //// Imagen superior
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/images/logoInabExcel.jpg"));

            logo.ScalePercent(80f);

            PdfPCell c1 = new PdfPCell(logo);


            c1.Colspan = 2;
            c1.Rowspan = 4;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nCONSTANCIA DE RECEPCIÓN DE DOCUMENTOS\n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("REV-0.1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Agosto 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO:REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaDatosSolicitante(long Solicitud_id, string NoExpediente)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            string Nombre, No_Nit, No_DPI, textoNITDPI;
            textoNITDPI = "";
            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            if (tbl_Sol_PropietarioPersonaIndividual != null)
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaIndividual.Nombres} {tbl_Sol_PropietarioPersonaIndividual.Apellidos}";
                No_DPI = $"{tbl_Sol_PropietarioPersonaIndividual.No_Documento}";
                if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 1)
                {
                    textoNITDPI = "DPI";
                }
                else if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 2)
                {
                    textoNITDPI = "Pasaporte";

                }
                else if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 3)
                {
                    textoNITDPI = "Cédula";
                }
                No_Nit = No_DPI;
            }
            else
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaJuridica.Nombre}";
                No_Nit = $"{tbl_Sol_PropietarioPersonaJuridica.No_NIT}";
                textoNITDPI = "NIT";
            }

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();

            DateTime fecha;
            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //---------------------------------------  Tipo de solicitud  --------------

            c1 = new PdfPCell(new Phrase($"Inscripción de :", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper(), fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 4;

            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"En la categoría de :", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToString().ToUpper(), fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);


            //---------------------------------------  Tipo de solicitud  --------------




            c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            glNombrePropietario = Nombre;

            c1 = new PdfPCell(new Phrase($"{Nombre}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 4;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{textoNITDPI}: {No_Nit}", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número de expediente", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(NoExpediente, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            if (oRepresentanteLegal != null)
            {
                for (int i = 0; i < oRepresentanteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].Nombres} {oRepresentanteLegal[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                }
            }

            if (oMandatario != null)
            {
                for (int i = 0; i < oMandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 3;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].Nombres} {oMandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 9;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oMandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                }
            }


            tableBanner.AddCell(c1);


            //tbl_Sol_Solicitud


            c1 = new PdfPCell(new Phrase($"Serie de la factura", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.FacturaSerie, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.FacturaNumero, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Cantidad de folios", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.CantidadFolios.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);


            return;
        }

        private void LlenaDatosSolicitanteSinCobro(long Solicitud_id, string NoExpediente)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            string Nombre, No_Nit, No_DPI, textoNITDPI;
            textoNITDPI = "";

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            if (tbl_Sol_PropietarioPersonaIndividual != null)
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaIndividual.Nombres} {tbl_Sol_PropietarioPersonaIndividual.Apellidos}";
                No_DPI = $"{tbl_Sol_PropietarioPersonaIndividual.No_Documento}";
                if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 1)
                {
                    textoNITDPI = "DPI";
                }
                else if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 2)
                {
                    textoNITDPI = "Pasaporte";

                }
                else if (tbl_Sol_PropietarioPersonaIndividual.Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo == 3)
                {
                    textoNITDPI = "Cédula";
                }
                No_Nit = No_DPI;
            }
            else
            {
                Nombre = $"{tbl_Sol_PropietarioPersonaJuridica.Nombre}";
                No_Nit = $"{tbl_Sol_PropietarioPersonaJuridica.No_NIT}";
                textoNITDPI = "NIT";
            }

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();

            DateTime fecha;
            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //---------------------------------------  Tipo de solicitud  --------------

            c1 = new PdfPCell(new Phrase($"Inscripción de :", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper(), fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 4;

            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"En la categoría de :", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToString().ToUpper(), fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            tableDatosGenerales.AddCell(c1);


            //---------------------------------------  Tipo de solicitud  --------------




            c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{Nombre}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 4;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{textoNITDPI}: {No_Nit}", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número de expediente", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(NoExpediente, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            if (oRepresentanteLegal != null)
            {
                for (int i = 0; i < oRepresentanteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].Nombres} {oRepresentanteLegal[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oRepresentanteLegal[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                }
            }

            if (oMandatario != null)
            {
                for (int i = 0; i < oMandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 3;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"No. DPI", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 6;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].Nombres} {oMandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 9;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (oMandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)oMandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                }
            }


            tableBanner.AddCell(c1);


            //tbl_Sol_Solicitud


            c1 = new PdfPCell(new Phrase($"Cantidad de folios", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.CantidadFolios.ToString(), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 7;
            c1.Border = 0;
            tableDatosGenerales.AddCell(c1);


            return;
        }



        private void LlenaDatosSolicitante(long Solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            List<Tbl_Sol_ArrendatarioPersonaJuridica> tbl_Sol_ArrendatarioPersonaJuridica = (from d in db.Tbl_Sol_ArrendatarioPersonaJuridica
                                                                                             where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                             select d).ToList();

            List<Tbl_Sol_ArrendatarioPersonaIndividual> tbl_Sol_ArrendatarioPersonaIndividual = (from d in db.Tbl_Sol_ArrendatarioPersonaIndividual
                                                                                                 where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                 select d).ToList();


            List<Tbl_Sol_PropietarioPersonaJuridica> tbl_Sol_PropietarioPersonaJuridicas = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                            where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                            select d).ToList();

            List<Tbl_Sol_PropietarioPersonaIndividual> tbl_Sol_PropietarioPersonaIndividuals = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                                where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();

            DateTime fecha;
            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            if (tbl_Sol_PropietarioPersonaIndividuals.Count() > 0)
            {
                for (int i = 0; i < tbl_Sol_PropietarioPersonaIndividuals.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    glNombrePropietario = tbl_Sol_PropietarioPersonaIndividuals[i].Nombres + " " + tbl_Sol_PropietarioPersonaIndividuals[i].Apellidos;

                    c1 = new PdfPCell(new Phrase(tbl_Sol_PropietarioPersonaIndividuals[i].Nombres + " " + tbl_Sol_PropietarioPersonaIndividuals[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_PropietarioPersonaIndividuals[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_PropietarioPersonaIndividuals[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (tbl_Sol_PropietarioPersonaJuridicas.Count() > 0)
            {
                for (int i = 0; i < tbl_Sol_PropietarioPersonaJuridicas.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_PropietarioPersonaJuridicas[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_PropietarioPersonaJuridicas[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                }
            }

            if (tbl_Sol_ArrendatarioPersonaJuridica.Count() > 0)
            {
                for (int i = 0; i < tbl_Sol_ArrendatarioPersonaJuridica.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_ArrendatarioPersonaJuridica[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_ArrendatarioPersonaJuridica[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                }
            }


            if (tbl_Sol_ArrendatarioPersonaIndividual.Count() > 0)
            {
                for (int i = 0; i < tbl_Sol_ArrendatarioPersonaIndividual.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_ArrendatarioPersonaIndividual[i].Nombres + " " + tbl_Sol_ArrendatarioPersonaIndividual[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_ArrendatarioPersonaIndividual[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_ArrendatarioPersonaIndividual[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (oRepresentanteLegal.Count() > 0)
            {
                for (int i = 0; i < oRepresentanteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Datos del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oRepresentanteLegal[i].Nombres} {oRepresentanteLegal[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(oRepresentanteLegal[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(oRepresentanteLegal[i].RepresentanteNo_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                        {
                            fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)oRepresentanteLegal[i].Fecha_InicioNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        if (oRepresentanteLegal[i].VigenciaIndefinida == true)
                        {
                            c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)oRepresentanteLegal[i].Fecha_FinNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }
                }
            }

            if (oMandatario.Count() > 0)
            {
                for (int i = 0; i < oMandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].Nombres} {oMandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase(oMandatario[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{oMandatario[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia del mandato", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        if (oMandatario[i].VigenciaIndefinida == true)
                        {
                            fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)oMandatario[i].Fecha_InicioNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }

                    c1 = new PdfPCell(new Phrase($"Vencimiento", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        if (oMandatario[i].VigenciaIndefinida == true)
                        {
                            c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)oMandatario[i].Fecha_FinNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }

                    c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }
            }


            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaDatosNotificacion(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {


            Tbl_Gral_Municipio oMunicipio = (from d in db.Tbl_Gral_Municipio
                                             where d.Municipio_id == tbl_Seg_UsuarioExterno.Municipio_id
                                             select d).FirstOrDefault();

            Tbl_Gral_Departamento oDepartamento = (from d in db.Tbl_Gral_Departamento
                                                   where d.Departamento_id == oMunicipio.Departamento_id
                                                   select d).FirstOrDefault();

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Dirección:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Direccion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oMunicipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oDepartamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Correo Electrónico:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Correo}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 5;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"No. de Celular", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Telefono_Celular}, {tbl_Seg_UsuarioExterno.Telefono_Oficina}, {tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            tableDatosNotificacion.AddCell(c1);

            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaDatosNotificacionDetallada(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);




            //************************************************************************************************************************************

            //************************************************************************************************************************************

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Notificacion_Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            GlobalUtils globalUtils = new GlobalUtils();

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);




            return;
        }


        private void LlenaDatosProfesionalPersonales(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //******************************************

            c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);
            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de nacimiento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Fecha_Nacimiento.ToString("dd/MM/yyyy"), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_Documento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("No. de NIT: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_NIT, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }

        private void LlenaDatosProfesionalDireccion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            GlobalUtils globalUtils = new GlobalUtils();

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono celular: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono oficina: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Oficina, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }




        private void FirmaSolicitante()
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 11);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
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
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
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

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            if (glNombreSolicitante == "Usuario Default")
            {
                c1 = new PdfPCell(new Phrase("Propietario/Representante Legal/Poseedor", fntTituloTabla));
            }
            else
            {
                c1 = new PdfPCell(new Phrase(glNombreSolicitante, fntTituloTabla));
            }

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


            Constants constant = new Constants();

            c1 = new PdfPCell(new Phrase(constant.initCapPalabras(glNombreSecretaria), fntTituloTabla));
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

            tableBanner.AddCell(c1);
            return;
        }

        private void LlenaDatosDocumentosSubidos(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(7);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 5;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("SI", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("NO", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            var revions = db.fc_Sol_Sel_RevisionDocumentosSubidos(tbl_Sol_Solicitud.Solicitud_id);

            foreach (var item in revions)
            {
                c1 = new PdfPCell(new Phrase(item.Descripcion, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 5;
                tableDatosGenerales.AddCell(c1);


                c1 = new PdfPCell(new Phrase(item.Realizado, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(item.No_Realizado, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

            }


            tableBanner.AddCell(c1);

            return;
        }


        private void LlenaDatosDocumentosNoSubidos(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosGenerales = new PdfPTable(7);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 5;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("SI", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            c1 = new PdfPCell(new Phrase("NO", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosGenerales.AddCell(c1);

            var revions = db.fc_Sol_Sel_RevisionDocumentosNoSubidos(tbl_Sol_Solicitud.Solicitud_id);

            foreach (var item in revions)
            {
                c1 = new PdfPCell(new Phrase(item.Descripcion, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 5;
                tableDatosGenerales.AddCell(c1);


                c1 = new PdfPCell(new Phrase(item.Realizado, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(item.No_Realizado, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

            }


            tableBanner.AddCell(c1);

            return;
        }


        //    Public Shared Function SQLdate(ByRef Fecha As DateTime) As String
        //    Dim Fch_String As String
        //    Fch_String = Format(Fecha, "yyyy").ToString + "-" + Format(Fecha, "MM").ToString + "-" + Format(Fecha, "dd").ToString + " " + Hour(Fecha).ToString + ":" + Minute(Fecha).ToString + ":" + Second(Fecha).ToString
        //    Return Fch_String
        //End Function


        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
        }


        private void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);


            tableTitulo = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);

            return;
        }



        private void LlenaSeisTextos(string TextoA, string TextoB, string TextoC, string TextoD, string TextoE, string TextoF)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);


            tableTitulo = new PdfPTable(6);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoE, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoF, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            return;
        }

        public string GenerarReportePDF(string guidid, string GuidEtapa_id, string facturaserie, string facturanumero, int cantidadfolios)
        {
            long Solicitud_id;

            string strDir = "Archivos_ConFirmaElectronica\\";   
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

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

            glNombreSecretaria = objUs.strNombre_Usuario;

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Enter = new Paragraph(" ");

            string[] Texto_Romano = new string[11];

            int intTexto_Romano = 1;
            {
                Texto_Romano[1] = "I";
                Texto_Romano[2] = "II";
                Texto_Romano[3] = "III";
                Texto_Romano[4] = "IV";
                Texto_Romano[5] = "V";
                Texto_Romano[6] = "VI";
                Texto_Romano[7] = "VII";
                Texto_Romano[8] = "VIII";
                Texto_Romano[9] = "IX";
                Texto_Romano[10] = "X";
            }
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            Solicitud_id = tbl_sol_Solicitud.Solicitud_id;

            tbl_sol_Solicitud.FacturaSerie = facturaserie;
            tbl_sol_Solicitud.FacturaNumero = facturanumero;
            tbl_sol_Solicitud.CantidadFolios = cantidadfolios;
            tbl_sol_Solicitud.FechaRecepcionExpedienteFisico = DateTime.Now;
            tbl_sol_Solicitud.RecepcionExpedienteFisicoby = objUs.intUsuario_id;


            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == tbl_sol_Solicitud.swcreatedby
                                                             select d).FirstOrDefault();



            glNombreSolicitante = tbl_Seg_UsuarioExterno.Nombres + ' ' + tbl_Seg_UsuarioExterno.Apellidos;

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();

            strNombre = "U" + GuidEtapa_id + ".pdf";

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }



            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(9, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            CrearBanner crearBanner = new CrearBanner();

            string varTitulo = "CONSTANCIA DE RECEPCIÓN DE DOCUMENTOS";


            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            //LlenaTituloRevision(tbl_sol_Solicitud);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud.FechaRecepcionExpedienteFisico ?? DateTime.Now) + "')").FirstOrDefault();

            crearBanner.LlenaBanner(strFecha, "Derecha", "");
            doc.Add(crearBanner.tableBanner);

            var NumeroDeExpediente = new Paragraph("                  Número de expediente: " + tbl_sol_Solicitud.Solicitud_NumeroExpediente);
            doc.Add(NumeroDeExpediente);

            if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
            {
                NumeroDeExpediente = new Paragraph("                  Número de registro: " + tbl_sol_Solicitud.No_Registro);
                doc.Add(NumeroDeExpediente);
            }

            doc.Add(Enter);

            if (tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf)
            {
                if ((tbl_sol_Solicitud.Procedencia_Expediente != null) && (tbl_sol_Solicitud.Procedencia_Expediente.Trim() != ""))
                {
                    var NumeroDeExpedienteOrigen = new Paragraph("                  Número de expediente de origen: " + tbl_sol_Solicitud.Procedencia_Expediente);
                    doc.Add(NumeroDeExpedienteOrigen);
                    doc.Add(Enter);
                }
            }

            Constants constant = new Constants();

            LlenaCuatroTextos("Región : ", tbl_sol_Solicitud.Tbl_Gral_Region.No_Region + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_Region.Nombre_Region), "Sub-región:", tbl_sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion));
            doc.Add(tableTitulo);

            LlenaSeisTextos("Serie de la factura: ", tbl_sol_Solicitud.FacturaSerie, "Número:", tbl_sol_Solicitud.FacturaNumero, "Cantidad de folios :", (tbl_sol_Solicitud.CantidadFolios ?? 0).ToString());
            doc.Add(tableTitulo);
            doc.Add(Enter);

            if (tbl_sol_Solicitud.Categoria_id != 7)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL PROPIETARIO");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
            }
            LlenaDatosSolicitante(Solicitud_id);
            doc.Add(tableDatosGenerales);
            doc.Add(Enter);


            //if (tbl_sol_Solicitud.Categoria_id != 7)
            //{
            //    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL SOLICITANTE");
            //    intTexto_Romano = intTexto_Romano + 1;
            //    doc.Add(tableBanner);
            //    LlenaDatosProfesionalPersonales(tbl_sol_Solicitud);
            //    doc.Add(tableDatosNotificacion);
            //    doc.Add(Enter);

            //    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE UBICACIÓN");
            //    intTexto_Romano = intTexto_Romano + 1;
            //    doc.Add(tableBanner);
            //    LlenaDatosProfesionalDireccion(tbl_sol_Solicitud);
            //    doc.Add(tableDatosNotificacion);
            //    doc.Add(Enter);

            //}
            //else
            //{ 


            if (!tbl_sol_Solicitud.Procedencia_PinpepNew && !tbl_sol_Solicitud.Procedencia_PinpepOld && !tbl_sol_Solicitud.Procedencia_Probosque && !tbl_sol_Solicitud.Procedencia_secorf)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE NOTIFICACIÓN");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
                LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);
                doc.Add(tableDatosNotificacion);
                doc.Add(Enter);
            }
            //}






            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DOCUMENTOS RECIBIDOS / NO RECIBIDOS ");
            intTexto_Romano = intTexto_Romano + 1;
            doc.Add(tableBanner);
            LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);

            LlenaBanner("DOCUMENTOS RECIBIDOS");
            doc.Add(tableBanner);
            LlenaDatosDocumentosSubidos(tbl_sol_Solicitud);
            doc.Add(tableDatosGenerales);

            int cantidad = db.fc_Sol_Sel_RevisionDocumentosNoSubidos(tbl_sol_Solicitud.Solicitud_id).Count();
            if (cantidad > 0)
            {
                LlenaBanner("DOCUMENTOS NO RECIBIDOS");
                doc.Add(tableBanner);
                LlenaDatosDocumentosNoSubidos(tbl_sol_Solicitud);
                doc.Add(tableDatosGenerales);
            }
            doc.Add(Enter);

            //FirmaSolicitante();
            //PdfPCell celdaFirmante = new PdfPCell();
            //celdaFirmante = FirmanteSolicitanteV2("quien Firma");
            
             doc.Add(FirmanteSolicitanteV2());
            // doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado = strNombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Impresion de documento de recepción de documentos y recibo asociado";
            tbl_RSS_Usuario.Cuerpo = "Los documentos fueron recibidos y foliados.";
            tbl_RSS_Usuario.Pasos_a_seguir = "Puede visualizar el documento asociado.";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "/Archivos_ConFirmaElectronica/" + strNombre;
            tbl_RSS_Usuario.link_A_descripcion = "<<Click aquí para abrir la documento>>";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();


            return strNombre;
        }

        public string GenerarReporteSinCobroPDF(string guidid, string GuidEtapa_id, int cantidadfolios,string Titular,string TitularRecepcionista)
        {

            long Solicitud_id;

            string strDir = "Archivos_ConFirmaElectronica\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

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

            glNombreSecretaria = objUs.strNombre_Usuario;

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Enter = new Paragraph(" ");

            string[] Texto_Romano = new string[11];

            int intTexto_Romano = 1;
            {
                Texto_Romano[1] = "I";
                Texto_Romano[2] = "II";
                Texto_Romano[3] = "III";
                Texto_Romano[4] = "IV";
                Texto_Romano[5] = "V";
                Texto_Romano[6] = "VI";
                Texto_Romano[7] = "VII";
                Texto_Romano[8] = "VIII";
                Texto_Romano[9] = "IX";
                Texto_Romano[10] = "X";
            }
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            Solicitud_id = tbl_sol_Solicitud.Solicitud_id;

            tbl_sol_Solicitud.EnmiendasRecibidas = true;
            tbl_sol_Solicitud.CantidadFolios = cantidadfolios;
            tbl_sol_Solicitud.FechaRecepcionExpedienteFisico = DateTime.Now;
            tbl_sol_Solicitud.RecepcionExpedienteFisicoby = objUs.intUsuario_id;


            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            string NoOficioSubRegiona = "";

            try
            {
                int RolSubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;
                Tbl_Sol_IdentificadorOficial tbl_Sol_IdentificadorOficials = db.Tbl_Sol_IdentificadorOficial.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id && Obj.Rol_id == RolSubRegional).FirstOrDefault();
                NoOficioSubRegiona = tbl_Sol_IdentificadorOficials.IdentificadorAsignado;

                //NoOficioSubRegiona = db.Tbl_Sol_IdentificadorOficial.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id && Obj.Rol_id == RolSubRegional).Last().IdentificadorAsignado;
            }
            catch
            {
                NoOficioSubRegiona = "Oficial";
            }

            ViewBag.NoOficioSubRegiona = NoOficioSubRegiona;

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                            where d.Usuario_id == tbl_sol_Solicitud.swcreatedby                                                            
                                                             select d).FirstOrDefault();



            if (Titular == "block")
          
            {

                glNombreSolicitante = TitularRecepcionista;
            }
            else
            {
                glNombreSolicitante = tbl_Seg_UsuarioExterno.Nombres + ' ' + tbl_Seg_UsuarioExterno.Apellidos;
            }


            
           

               
           

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();

            strNombre = "U" + GuidEtapa_id + ".pdf";

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            CrearBanner crearBanner = new CrearBanner();

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(9, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            string varTitulo = "CONSTANCIA DE RECEPCIÓN DE DOCUMENTOS";


            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(tableTitulo);
            doc.Add(Enter);


            //LlenaTituloRevision(tbl_sol_Solicitud);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud.FechaRecepcionExpedienteFisico ?? DateTime.Now) + "')").FirstOrDefault();

            crearBanner.LlenaBanner(strFecha, "Derecha", "");
            doc.Add(crearBanner.tableBanner);



            var NumeroDeExpediente = new Paragraph("                  Número de expediente: " + tbl_sol_Solicitud.Solicitud_NumeroExpediente);
            doc.Add(NumeroDeExpediente);
            doc.Add(Enter);

            if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
            {
                NumeroDeExpediente = new Paragraph("                  Número de registro: " + tbl_sol_Solicitud.No_Registro);
                doc.Add(NumeroDeExpediente);
            }

            if (tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf)
            {
                if ((tbl_sol_Solicitud.Procedencia_Expediente != null) && (tbl_sol_Solicitud.Procedencia_Expediente.Trim() != ""))
                {
                    var NumeroDeExpedienteOrigen = new Paragraph("                  Número de expediente de origen: " + tbl_sol_Solicitud.Procedencia_Expediente);
                    doc.Add(NumeroDeExpedienteOrigen);
                    doc.Add(Enter);
                }
            }

            NumeroDeExpediente = new Paragraph("                  Tipo de gestión: Recepción de enmiendas");
            doc.Add(NumeroDeExpediente);
            doc.Add(Enter);

            NumeroDeExpediente = new Paragraph("                  Se recibieron los documentos solicitados en el oficio de enmiendas número : " + NoOficioSubRegiona);
            doc.Add(NumeroDeExpediente);
            doc.Add(Enter);

            LlenaCuatroTextos("Región: ", tbl_sol_Solicitud.Tbl_Gral_Region.No_Region + " " + tbl_sol_Solicitud.Tbl_Gral_Region.Nombre_Region, "Sub-región:", tbl_sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + tbl_sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion);
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Cantidad de folios :", (tbl_sol_Solicitud.CantidadFolios ?? 0).ToString(), " Tipo de documentos :", "Anexos");
            doc.Add(tableTitulo);
            doc.Add(Enter);


            if (tbl_sol_Solicitud.Categoria_id != 7)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL PROPIETARIO");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
            }

            LlenaDatosSolicitante(Solicitud_id);
            doc.Add(tableDatosGenerales);
            doc.Add(Enter);

            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE NOTIFICACIÓN");
            intTexto_Romano = intTexto_Romano + 1;
            doc.Add(tableBanner);
            LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);
            doc.Add(tableDatosNotificacion);
            doc.Add(Enter);

            doc.Add(Enter);

            FirmaSolicitante();
            doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado = strNombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Impresion de documento de recepción de documentos y recibo asociado";
            tbl_RSS_Usuario.Cuerpo = "Los documentos fueron recibidos y foliados.";
            tbl_RSS_Usuario.Pasos_a_seguir = "Puede visualizar el documento asociado.";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "/Archivos_ConFirmaElectronica/" + strNombre;
            tbl_RSS_Usuario.link_A_descripcion = "<<Click aquí para abrir la documento>>";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();


            return strNombre;
        }



        public string GenerarReporteExoneradoPDF(string guidid, string GuidEtapa_id, int cantidadfolios)
        {

            long Solicitud_id;

            string strDir = "Archivos_ConFirmaElectronica\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

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

            glNombreSecretaria = objUs.strNombre_Usuario;

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Enter = new Paragraph(" ");

            string[] Texto_Romano = new string[11];

            int intTexto_Romano = 1;
            {
                Texto_Romano[1] = "I";
                Texto_Romano[2] = "II";
                Texto_Romano[3] = "III";
                Texto_Romano[4] = "IV";
                Texto_Romano[5] = "V";
                Texto_Romano[6] = "VI";
                Texto_Romano[7] = "VII";
                Texto_Romano[8] = "VIII";
                Texto_Romano[9] = "IX";
                Texto_Romano[10] = "X";
            }
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            Solicitud_id = tbl_sol_Solicitud.Solicitud_id;

            tbl_sol_Solicitud.EnmiendasRecibidas = true;
            tbl_sol_Solicitud.CantidadFolios = cantidadfolios;
            tbl_sol_Solicitud.FechaRecepcionExpedienteFisico = DateTime.Now;
            tbl_sol_Solicitud.RecepcionExpedienteFisicoby = objUs.intUsuario_id;


            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();



            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == tbl_sol_Solicitud.swcreatedby
                                                             select d).FirstOrDefault();



            glNombreSolicitante = tbl_Seg_UsuarioExterno.Nombres + ' ' + tbl_Seg_UsuarioExterno.Apellidos;

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();

            strNombre = "U" + GuidEtapa_id + ".pdf";

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(9, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            CrearBanner crearBanner = new CrearBanner();

            string varTitulo = "CONSTANCIA DE RECEPCIÓN DE DOCUMENTOS";


            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            //LlenaTituloRevisionExoneracion(tbl_sol_Solicitud);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);



            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud.FechaRecepcionExpedienteFisico ?? DateTime.Now) + "')").FirstOrDefault();


            crearBanner.LlenaBanner(strFecha, "Derecha", "");
            doc.Add(crearBanner.tableBanner);

            var NumeroDeExpediente = new Paragraph("                  Número de expediente: " + tbl_sol_Solicitud.Solicitud_NumeroExpediente);
            doc.Add(NumeroDeExpediente);

            if ((tbl_sol_Solicitud.No_Registro ?? "") != "")
            {
                NumeroDeExpediente = new Paragraph("                  Número de registro: " + tbl_sol_Solicitud.No_Registro);
                doc.Add(NumeroDeExpediente);
            }

            doc.Add(Enter);

            if (tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf)
            {
                if ((tbl_sol_Solicitud.Procedencia_Expediente != null) && (tbl_sol_Solicitud.Procedencia_Expediente.Trim() != ""))
                {
                    var NumeroDeExpedienteOrigen = new Paragraph("                  Número de expediente de origen: " + tbl_sol_Solicitud.Procedencia_Expediente);
                    doc.Add(NumeroDeExpedienteOrigen);
                    doc.Add(Enter);
                }
            }

            Constants constant = new Constants();

            LlenaCuatroTextos("Región: ", tbl_sol_Solicitud.Tbl_Gral_Region.No_Region + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_Region.Nombre_Region), "Sub-región:", tbl_sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion));
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Cantidad de folios :", (tbl_sol_Solicitud.CantidadFolios ?? 0).ToString(), " Tipo de cobro :", "Exonerado");
            doc.Add(tableTitulo);
            doc.Add(Enter);



            if (tbl_sol_Solicitud.Categoria_id != 7)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL PROPIETARIO");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
            }

            LlenaDatosSolicitante(Solicitud_id);
            doc.Add(tableDatosGenerales);
            doc.Add(Enter);



            if (!tbl_sol_Solicitud.Procedencia_PinpepNew && !tbl_sol_Solicitud.Procedencia_PinpepOld && !tbl_sol_Solicitud.Procedencia_Probosque && !tbl_sol_Solicitud.Procedencia_secorf)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE NOTIFICACIÓN");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
                LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);
                doc.Add(tableDatosNotificacion);
                doc.Add(Enter);
            }


            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DOCUMENTOS RECIBIDOS / NO RECIBIDOS ");
            intTexto_Romano = intTexto_Romano + 1;
            doc.Add(tableBanner);
            LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);



            LlenaBanner("DOCUMENTOS RECIBIDOS");
            doc.Add(tableBanner);
            LlenaDatosDocumentosSubidos(tbl_sol_Solicitud);
            doc.Add(tableDatosGenerales);


            int cantidad = db.fc_Sol_Sel_RevisionDocumentosNoSubidos(tbl_sol_Solicitud.Solicitud_id).Count();
            if (cantidad > 0)
            {
                LlenaBanner("DOCUMENTOS NO RECIBIDOS");
                doc.Add(tableBanner);
                LlenaDatosDocumentosNoSubidos(tbl_sol_Solicitud);
                doc.Add(tableDatosGenerales);
            }
            doc.Add(Enter);
            doc.Add(Enter);

            FirmaSolicitante();
            doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado = strNombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Impresion de documento de recepción de documentos y recibo asociado";
            tbl_RSS_Usuario.Cuerpo = "Los documentos fueron recibidos y foliados.";
            tbl_RSS_Usuario.Pasos_a_seguir = "Puede visualizar el documento asociado.";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "/Archivos_ConFirmaElectronica/" + strNombre;
            tbl_RSS_Usuario.link_A_descripcion = "<<Click aquí para abrir la documento>>";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();


            return strNombre;
        }



        public string GenerarReporteExoneradoPDF__(string guidid, string GuidEtapa_id, int cantidadfolios)
        {

            long Solicitud_id;

            string strDir = "Archivos_ConFirmaElectronica\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

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

            glNombreSecretaria = objUs.strNombre_Usuario;

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Enter = new Paragraph(" ");

            string[] Texto_Romano = new string[11];

            int intTexto_Romano = 1;
            {
                Texto_Romano[1] = "I";
                Texto_Romano[2] = "II";
                Texto_Romano[3] = "III";
                Texto_Romano[4] = "IV";
                Texto_Romano[5] = "V";
                Texto_Romano[6] = "VI";
                Texto_Romano[7] = "VII";
                Texto_Romano[8] = "VIII";
                Texto_Romano[9] = "IX";
                Texto_Romano[10] = "X";
            }
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == guidid).First();

            Solicitud_id = tbl_sol_Solicitud.Solicitud_id;

            tbl_sol_Solicitud.EnmiendasRecibidas = true;
            tbl_sol_Solicitud.CantidadFolios = cantidadfolios;
            tbl_sol_Solicitud.FechaRecepcionExpedienteFisico = DateTime.Now;
            tbl_sol_Solicitud.RecepcionExpedienteFisicoby = objUs.intUsuario_id;


            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();



            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d).FirstOrDefault();

            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == tbl_sol_Solicitud.swcreatedby
                                                             select d).FirstOrDefault();



            glNombreSolicitante = tbl_Seg_UsuarioExterno.Nombres + ' ' + tbl_Seg_UsuarioExterno.Apellidos;

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();

            strNombre = "U" + GuidEtapa_id + ".pdf";

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(9, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            CrearBanner crearBanner = new CrearBanner();

            string varTitulo = "CONSTANCIA DE RECEPCIÓN DE DOCUMENTOS";


            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            //LlenaTituloRevisionExoneracion(tbl_sol_Solicitud);
            //doc.Add(tableTitulo);
            //doc.Add(Enter);



            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt('" + SQLDate(tbl_sol_Solicitud.FechaRecepcionExpedienteFisico ?? DateTime.Now) + "')").FirstOrDefault();


            crearBanner.LlenaBanner(strFecha, "Derecha", "");
            doc.Add(crearBanner.tableBanner);

            var NumeroDeExpediente = new Paragraph("                  Número de expediente: " + tbl_sol_Solicitud.Solicitud_NumeroExpediente);
            doc.Add(NumeroDeExpediente);
            doc.Add(Enter);

            Constants constant = new Constants();

            LlenaCuatroTextos("Región: ", tbl_sol_Solicitud.Tbl_Gral_Region.No_Region + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_Region.Nombre_Region), "Sub-región:", tbl_sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + constant.initCapPalabras(tbl_sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion));
            doc.Add(tableTitulo);

            LlenaCuatroTextos("Cantidad de folios :", (tbl_sol_Solicitud.CantidadFolios ?? 0).ToString(), " Tipo de cobro :", "Exonerado");
            doc.Add(tableTitulo);
            doc.Add(Enter);



            if (tbl_sol_Solicitud.Categoria_id != 7)
            {
                LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL PROPIETARIO");
                intTexto_Romano = intTexto_Romano + 1;
                doc.Add(tableBanner);
            }

            LlenaDatosSolicitante(Solicitud_id);
            doc.Add(tableDatosGenerales);
            doc.Add(Enter);



            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE NOTIFICACIÓN");
            intTexto_Romano = intTexto_Romano + 1;
            doc.Add(tableBanner);
            LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);
            doc.Add(tableDatosNotificacion);
            doc.Add(Enter);


            LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DOCUMENTOS RECIBIDOS / NO RECIBIDOS ");
            intTexto_Romano = intTexto_Romano + 1;
            doc.Add(tableBanner);
            LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);



            LlenaBanner("DOCUMENTOS RECIBIDOS");
            doc.Add(tableBanner);
            LlenaDatosDocumentosSubidos(tbl_sol_Solicitud);
            doc.Add(tableDatosGenerales);


            int cantidad = db.fc_Sol_Sel_RevisionDocumentosNoSubidos(tbl_sol_Solicitud.Solicitud_id).Count();
            if (cantidad > 0)
            {
                LlenaBanner("DOCUMENTOS NO RECIBIDOS");
                doc.Add(tableBanner);
                LlenaDatosDocumentosNoSubidos(tbl_sol_Solicitud);
                doc.Add(tableDatosGenerales);
            }
            doc.Add(Enter);
            doc.Add(Enter);

            FirmaSolicitante();
            doc.Add(tableFirmaSolicitante);

            doc.Close();
            writer.Close();

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == GuidEtapa_id).First();

            tbl_Gest_EtapaSolicitud.NombreDocumentoFirmado = strNombre;

            db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RSS_Usuario.Max(u => u.RSS_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            Tbl_RSS_Usuario tbl_RSS_Usuario = new Tbl_RSS_Usuario();

            tbl_RSS_Usuario.RSS_id = lngIdt;
            tbl_RSS_Usuario.UsuarioExterno_id = tbl_sol_Solicitud.swcreatedby;
            tbl_RSS_Usuario.Guid_Solicitud = tbl_sol_Solicitud.Guid_id;
            tbl_RSS_Usuario.imagen_Link = "/Content/icons/DocumentoRecibirDocumentosPago.jpg";
            tbl_RSS_Usuario.imagen_url = "#";
            tbl_RSS_Usuario.EtapaSolicitud_GUID_id = "NA";
            tbl_RSS_Usuario.Titulo = "Impresion de documento de recepción de documentos y recibo asociado";
            tbl_RSS_Usuario.Cuerpo = "Los documentos fueron recibidos y foliados.";
            tbl_RSS_Usuario.Pasos_a_seguir = "Puede visualizar el documento asociado.";
            tbl_RSS_Usuario.Link_Pasos_a_seguir = "";
            tbl_RSS_Usuario.link_A = "/Archivos_ConFirmaElectronica/" + strNombre;
            tbl_RSS_Usuario.link_A_descripcion = "<<Click aquí para abrir la documento>>";
            tbl_RSS_Usuario.link_A_Externo = false;
            tbl_RSS_Usuario.link_A_Interno = true;
            tbl_RSS_Usuario.link_A_Target_Blank = true;

            tbl_RSS_Usuario.link_B = "";
            tbl_RSS_Usuario.link_B_descripcion = "";
            tbl_RSS_Usuario.link_B_Externo = true;
            tbl_RSS_Usuario.link_B_Target_Blank = true;

            tbl_RSS_Usuario.link_C = "";
            tbl_RSS_Usuario.link_C_descripcion = "";
            tbl_RSS_Usuario.link_C_Externo = false;
            tbl_RSS_Usuario.link_C_Target_Blank = false;

            tbl_RSS_Usuario.link_D = "";
            tbl_RSS_Usuario.link_D_descripcion = "";
            tbl_RSS_Usuario.link_D_Externo = false;
            tbl_RSS_Usuario.link_D_Target_Blank = false;

            tbl_RSS_Usuario.InternoLeido = false;
            tbl_RSS_Usuario.ExternoLeido = false;

            tbl_RSS_Usuario.Msg_ParaExterno = false;
            tbl_RSS_Usuario.Msg_ParaInterno = true;
            tbl_RSS_Usuario.swdatecreated = DateTime.Now;

            db.Tbl_RSS_Usuario.Add(tbl_RSS_Usuario);
            db.SaveChanges();


            return strNombre;
        }


        class ArchivoGenerado
        {
            public int result { get; set; }
            public string message { get; set; }
            public string ubicacion { get; set; }
        }


        public void DatosExpedienteRegistro(string Expediente, string ExpedienteOrigen, string No_Registro, string No_RegistroLiteral, int No_RegistroCorrelativo)
        {
            tableBanner = new PdfPTable(6);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            //Fila 1
            PdfPCell c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            string correlativo = "";
            if (No_RegistroCorrelativo != 0)
            {
                correlativo = No_RegistroCorrelativo.ToString();
            }

            if ((Expediente != null) && (Expediente.Trim() != ""))
            {
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Expediente", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Expediente, fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);
            }

            if ((ExpedienteOrigen != null) && (ExpedienteOrigen.Trim() != ""))
            {
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Expediente Origen", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(ExpedienteOrigen, fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);
            }

            //Fila 2
            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código de Registro", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;
            tableBanner.AddCell(c1);

            if ((No_RegistroLiteral ?? "") != "")
            {

                c1 = new PdfPCell(new Phrase((No_RegistroLiteral ?? "") + "-" + correlativo, fntTituloTabla));
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);


            }
            else
            {
                c1 = new PdfPCell(new Phrase((""), fntTituloTabla));
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);


            }
            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);
            return;
        }

        public void DatosExpedienteRegistro__(string Expediente, string No_Registro, string No_RegistroLiteral, int No_RegistroCorrelativo)
        {
            tableBanner = new PdfPTable(6);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            //Fila 1
            PdfPCell c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));

            string correlativo = "";
            if (No_RegistroCorrelativo != 0)
            {
                correlativo = No_RegistroCorrelativo.ToString();
            }

            if (Expediente != null)
            {
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Expediente", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_CENTER;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Expediente, fntTituloTabla));
                c1.Colspan = 3;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);

                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                c1.Colspan = 1;
                c1.Border = 0;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);
            }

            //Fila 2
            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código de Registro", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;
            tableBanner.AddCell(c1);

            if ((No_RegistroLiteral ?? "") != "")
            {

                c1 = new PdfPCell(new Phrase((No_RegistroLiteral ?? "") + "-" + correlativo, fntTituloTabla));
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);


            }
            else
            {
                c1 = new PdfPCell(new Phrase((""), fntTituloTabla));
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableBanner.AddCell(c1);


            }
            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);
            return;
        }

        public void DatosPropietarioRepresentante(string titulo, string nombre)
        {
            tableBanner = new PdfPTable(4);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(titulo, fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(nombre, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }

        public void DatosTecnicoProfesional(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            Tbl_Sol_TecnicoProfesional tbl_Sol_TecnicoProfesional = db.Tbl_Sol_TecnicoProfesional.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).First();

            tableBanner = new PdfPTable(4);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Solicitante :", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.Nombres + " " + tbl_Sol_TecnicoProfesional.Apellidos, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }


        public void DatosUbicacion(string Direccion, string Municipio, string Departamento)
        {
            tableBanner = new PdfPTable(8);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Dirección de ubicación", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Direccion, fntTituloTabla));
            c1.Colspan = 6;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            PdfPCell Enter = new PdfPCell(new Phrase(" ", fntTituloTabla));
            Enter.Colspan = 8;
            Enter.Border = 0;
            tableBanner.AddCell(Enter);


            c1 = new PdfPCell(new Phrase("Municipio", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Municipio, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Departamento, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;

        }

        public void DatosUbicacionEmpresa(string Direccion, string Municipio, string Departamento)
        {
            tableBanner = new PdfPTable(8);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Ubicación Empresa", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Direccion, fntTituloTabla));
            c1.Colspan = 6;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            PdfPCell Enter = new PdfPCell(new Phrase(" ", fntTituloTabla));
            Enter.Colspan = 8;
            Enter.Border = 0;
            tableBanner.AddCell(Enter);


            c1 = new PdfPCell(new Phrase("Municipio", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Municipio, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Departamento, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;

        }

        public void DatosUbicacionMovil(string Direccion, string Municipio, string Departamento)
        {
            tableBanner = new PdfPTable(8);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Ubicación de funcionamiento", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Direccion, fntTituloTabla));
            c1.Colspan = 6;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            PdfPCell Enter = new PdfPCell(new Phrase(" ", fntTituloTabla));
            Enter.Colspan = 8;
            Enter.Border = 0;
            tableBanner.AddCell(Enter);


            c1 = new PdfPCell(new Phrase("Municipio", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Municipio, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Departamento, fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;

        }

        public void DatosCategoria(string Categoria)
        {
            tableBanner = new PdfPTable(4);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Categoría de Registro", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Categoria, fntTituloTabla));
            c1.Colspan = 3;
            c1.Border = PdfPCell.BOTTOM_BORDER;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }

        class Personerias
        {
            public List<string> PropietariosIndividuales { get; set; }
            public List<string> PropietariosJuridicos { get; set; }
            public List<string> RepresentatnteLegal { get; set; }
            public List<string> Mandatario { get; set; }
            public List<string> ArrendatariosIndividuales { get; set; }
            public List<string> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias(long Solicitud_id)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<string>();
            personerias.PropietariosJuridicos = new List<string>();
            personerias.RepresentatnteLegal = new List<string>();
            personerias.Mandatario = new List<string>();
            personerias.ArrendatariosIndividuales = new List<string>();
            personerias.ArrendatariosJuridicos = new List<string>();
            List<fc_Sol_Sel_ListadoDePropietario_Result> PropietariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                     where d.Personeria_id == 1
                                                                                     select d).ToList();

            List<fc_Sol_Sel_ListadoDePropietario_Result> ProietariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                 where d.Personeria_id == 2
                                                                                 select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> RepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false)
                                                                                    select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> Mandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true)
                                                                            select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                       where d.Personeria_id == 1
                                                                                       select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                    where d.Personeria_id == 2
                                                                                    select d).ToList();

            if (PropietariosIndividuales.Count() > 0)
            {
                foreach (var item in PropietariosIndividuales)
                {
                    personerias.PropietariosIndividuales.Add(item.Nombre);
                }
            }

            if (ProietariosJuridicos.Count() > 0)
            {
                foreach (var item in ProietariosJuridicos)
                {
                    personerias.PropietariosJuridicos.Add(item.Nombre);
                }
            }

            if (RepresentanteLegal.Count() > 0)
            {
                foreach (var item in RepresentanteLegal)
                {
                    personerias.RepresentatnteLegal.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (Mandatario.Count() > 0)
            {
                foreach (var item in Mandatario)
                {
                    personerias.Mandatario.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (ArrendatariosIndividuales.Count() > 0)
            {
                foreach (var item in ArrendatariosIndividuales)
                {
                    personerias.ArrendatariosIndividuales.Add(item.Nombre);
                }
            }

            if (ArrendatariosJuridicos.Count() > 0)
            {
                foreach (var item in ArrendatariosJuridicos)
                {
                    personerias.ArrendatariosJuridicos.Add(item.Nombre);
                }
            }

            return personerias;
        }

        public string GenerarCaratulaSolicitud(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            long Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;

            strNombre = @"Caratula_" + tbl_Sol_Solicitud.Solicitud_NumeroTemporal + fecha + ".pdf";
            strDirArchivo = strFolder + strNombre;

            var Enter = new Paragraph(" ");
            string No_Registro, No_RegistroLiteral, Expediente, Direccion, Municipio, Departamento, ExpedienteOrigen;
            int No_RegistroCorrelativo;
            No_Registro = tbl_Sol_Solicitud.No_Registro;
            No_RegistroLiteral = tbl_Sol_Solicitud.No_RegistroLiteral;
            No_RegistroCorrelativo = (tbl_Sol_Solicitud.No_RegistroCorrelativo ?? 0);

            string[] Texto_Romano = new string[11];

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();
            string strDirectorRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_NombreSubDirectorRegional](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();
            string strDireccionSubRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_SubRegionCodigo](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();

            fc_SolRNF_DatosInscripcion_Result datosInscripcion = new fc_SolRNF_DatosInscripcion_Result();
            var direccion_Result = (from d in db.fc_Sol_Sel_Direccion(tbl_Sol_Solicitud.Solicitud_id)
                                    select d).ToList();
            string strDireccion = "";
            //if(direccion_Result[0].NombreFinca.Trim() != "")
            //{
            //    strDireccion += direccion_Result[0].NombreFinca + ", ";
            //}
            if (direccion_Result.Count() > 0)
            {
                if ((direccion_Result[0].Direccion ?? "").Trim() != "")
                {
                    strDireccion += direccion_Result[0].Direccion;
                }
                if ((direccion_Result[0].Aldea ?? "").Trim() != "")
                {
                    strDireccion += ", " + direccion_Result[0].Aldea;
                }
            }

            string strDireccionMovil = "";

            if (tbl_Sol_Solicitud.Categoria_id == 5)
            {
                if (direccion_Result.Count() > 1)
                {
                    if ((direccion_Result[1].NombreFinca ?? "").Trim() != "")
                    {
                        strDireccionMovil += direccion_Result[1].NombreFinca + ", ";
                    }
                    if ((direccion_Result[1].Direccion ?? "").Trim() != "")
                    {
                        strDireccionMovil += direccion_Result[1].Direccion;
                    }
                    if ((direccion_Result[1].Aldea ?? "").Trim() != "")
                    {
                        strDireccionMovil += ", " + direccion_Result[1].Aldea;
                    }
                }
            }

            Personerias personerias = ObtenerPersonerias(Solicitud_id);
            string datoreemplazar = "";
            List<string> datolistreemplazar = new List<string>();

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

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            CrearBanner crearBanner = new CrearBanner();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(11, tbl_Sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);
            string varTitulo = "CARATULA\nINSCRIPCIÓN A PETICIÓN DE OFICIO".ToUpper();

            Expediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente;
            ExpedienteOrigen = tbl_Sol_Solicitud.Procedencia_Expediente;
            //Document doc = new Document(PageSize.LETTER);
            Rectangle rectangle = new Rectangle(792, 612);
            Document doc = new Document(rectangle);
            doc.SetMargins(1f, 1f, 25f, 50f);

            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            doc.Open();
            doc.Add(Enter);

            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            //crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            //doc.Add(crearBanner.tableTitulo);
            //doc.Add(Enter);


            DatosExpedienteRegistro(Expediente, ExpedienteOrigen, No_Registro, No_RegistroLiteral, No_RegistroCorrelativo);
            doc.Add(tableBanner);
            doc.Add(Enter);


            #region Datos Solicitante
            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_Sol_Solicitud.swcreatedby);

            #endregion

            #region Datos Personerias
            datolistreemplazar = personerias.PropietariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Propietario", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }
            datolistreemplazar = personerias.PropietariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Propietario", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }
            datolistreemplazar = personerias.RepresentatnteLegal;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Representante Legal", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }
            datolistreemplazar = personerias.Mandatario;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Mandatario", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }
            datolistreemplazar = personerias.ArrendatariosIndividuales;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Arrendatario", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }
            datolistreemplazar = personerias.ArrendatariosJuridicos;
            if (datolistreemplazar.Count() > 0)
            {
                foreach (var item in datolistreemplazar)
                {
                    DatosPropietarioRepresentante("Arrendatario", item);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
            }

            #endregion

            if (tbl_Sol_Solicitud.Categoria_id == 7)
            {
                DatosTecnicoProfesional(tbl_Sol_Solicitud);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            if ((tbl_Sol_Solicitud.Categoria_id == 5) || (tbl_Sol_Solicitud.Categoria_id == 8 && tbl_Sol_Solicitud.Categoria_id == 2))
            {
                DatosUbicacionEmpresa(strDireccion, direccion_Result[0].Municipio, direccion_Result[0].Departamento);
                doc.Add(tableBanner);
                doc.Add(Enter);

                if (strDireccionMovil != "")
                {

                    Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).First();

                    if ((tbl_Sol_Empresa_Entidad.Tipo_Industria_id == 2) || ((tbl_Sol_Solicitud.Categoria_id == 5) && (tbl_Sol_Solicitud.Sub_Categoria_id == 3)))
                    {
                        DatosUbicacionMovil(strDireccionMovil, direccion_Result[1].Municipio, direccion_Result[1].Departamento);
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }
                }
            }
            else
            {
                DatosUbicacion(strDireccion, direccion_Result[0].Municipio, direccion_Result[0].Departamento);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }




            DatosCategoria(tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion);
            doc.Add(tableBanner);
            doc.Add(Enter);


            doc.Add(Enter);

            doc.Close();
            writer.Close();

            return "/" + strDir + strNombre;

        }

        public JsonResult DescargarCaratulaRegistro(long Solicitud_id)
        {



            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Solicitud_id == Solicitud_id
                                                   select d).FirstOrDefault();

            ArchivoGenerado archivoGenerado = new ArchivoGenerado();
            archivoGenerado.result = 0;
            archivoGenerado.message = "No se ha encontrado ningún registro asociado";
            if (tbl_Sol_Solicitud != null)
            {
                archivoGenerado.result = 1;
                archivoGenerado.message = "Archivo generado con éxito";
                try
                {
                    archivoGenerado.ubicacion = GenerarCaratulaSolicitud(tbl_Sol_Solicitud);
                }
                catch (Exception ex)
                {
                    archivoGenerado.result = 2;
                    archivoGenerado.message = ex.ToString();
                }
            }

            return Json(JsonConvert.SerializeObject(archivoGenerado));
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

        public ActionResult SetFirmaSolicitante(string nombreFirmante, string comoActua)
        {
            // accedenmos al valor del input y lo seteamos al diccionario de parametros globales
            if ("".Equals(nombreFirmante)){
                //si es vacio borrar 
                diccionarioVarialbes.Remove("firmante");
                diccionarioVarialbes.Remove("comoActua");
            }
            else
            { 
                //si no es vacio asignarlo
                diccionarioVarialbes.Add("firmante", nombreFirmante);
                diccionarioVarialbes.Add("comoActua", comoActua);
            }
            
            return View();
        } 
        
        //public ActionResult SetNombreSolicitante(string Titular)
        //{
        //    // accedenmos al valor del input y lo seteamos al diccionario de parametros globales
        //    if (Titular==""){
        //        //si es vacio borrar 
        //        diccionarioVarialbes.Remove("Titular");
        //        Session["Titular"] = "Si";


        //    }
        //    else
        //    {
        //        Session["Titular"] = "No";
        //        //si no es vacio asignarlo
        //        diccionarioVarialbes.Remove("Titular");
        //        diccionarioVarialbes.Add("Titular", Titular);
           
        //    }
            
        //    return View();
        //}

        public PdfPTable FirmanteSolicitanteV2()
        {
            // Crear una tabla con 9 columnas
            PdfPTable tableFirmaSolicitante1 = new PdfPTable(9);


            var colorTexto = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, colorTexto);

            PdfPCell cellHeader = new PdfPCell(new Phrase("\n\n\n\n\n\n", fntTituloTabla));
            cellHeader.BorderWidth = 0;
            cellHeader.Colspan = 9;
            tableFirmaSolicitante1.AddCell(cellHeader);

            // Añadir las celdas correspondientes a cada columna
            PdfPCell c1 = new PdfPCell(new Phrase("", fntTituloTabla));
            if (glNombreSolicitante == "Usuario Default")
            {
                if (diccionarioVarialbes.ContainsKey("firmante") && diccionarioVarialbes.ContainsKey("comoActua"))
                {
                    c1 = new PdfPCell(new Phrase(diccionarioVarialbes["firmante"] + "\n" + diccionarioVarialbes["comoActua"], fntTituloTabla));
                }
                else
                {
                    c1 = new PdfPCell(new Phrase("Propietario/Representante Legal/Poseedor", fntTituloTabla));
                }
            }
            else
            {
                if (diccionarioVarialbes.ContainsKey("firmante") && diccionarioVarialbes.ContainsKey("comoActua"))
                {
                    c1 = new PdfPCell(new Phrase(diccionarioVarialbes["firmante"] + "\n" + diccionarioVarialbes["comoActua"], fntTituloTabla));
                }
                else
                {
                    c1 = new PdfPCell(new Phrase(glNombreSolicitante, fntTituloTabla));
                }

            }



            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.BorderWidth = 0;
            c1.BorderWidthTop = 1;

            tableFirmaSolicitante1.AddCell(c1);

            PdfPCell c2 = new PdfPCell(new Phrase("", fntTituloTabla));
            c2.BorderWidth = 0;
            tableFirmaSolicitante1.AddCell(c2);

            Constants constant = new Constants();
            PdfPCell c3 = new PdfPCell(new Phrase(constant.initCapPalabras(glNombreSecretaria), fntTituloTabla));
            c3.BorderWidth = 0;
            c3.Colspan = 4;
            c3.HorizontalAlignment = Element.ALIGN_CENTER;
            c3.VerticalAlignment = Element.ALIGN_MIDDLE;
            c3.BorderWidthTop = 1;
            tableFirmaSolicitante1.AddCell(c3);

            return tableFirmaSolicitante1;
        }
    }
}
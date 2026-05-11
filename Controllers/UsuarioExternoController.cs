using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class UsuarioExternoController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        [HttpPost]
        public JsonResult ActualizaEtapaRespuesta
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

            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            //tbl_sol_solicitud.PermitirSubirDocumentos = false;
            //tbl_sol_solicitud.PermitirCambiosEnDatosMotosierra = false;
            //tbl_sol_solicitud.PermitirCambioDePropietarioRepresentante = false;
            //tbl_sol_solicitud.PermitirCambioFincaRodalesDasometricos = false;

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
                try
                {

                    db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception exec)
                {
                    Console.WriteLine(exec.Message);

                }

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



        public bool ConsultarCasilleroElectronico(CasilleroElectronicoRequest model)
        {
            if (((model.No_CasilleroElectronico ?? "").Trim() == "") || ((model.No_Documento ?? "") == ""))
            {
                return false;
            }
            RequestUtil requestUtil = new RequestUtil();

            return requestUtil.Execute_Consultar_CasilleroElectronico(model); ;
        }



        public ActionResult ListadoUsuarios(string idx)
        {
            ViewBag.idx = idx;
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

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Login/Index");
            }
            if ((idx != null) && (idx != ""))
            {
                return View(db.Tbl_Seg_UsuarioExterno.Where(x => ((x.Correo.Contains(idx)) || (x.No_Documento.Contains(idx)) || (x.Nombres.Contains(idx)) || (x.Apellidos.Contains(idx)))).ToList());
            }
            return View(db.Tbl_Seg_UsuarioExterno.ToList().Take(100));
        }

        public ActionResult Confirmacion(long id, long idx, string Pw)
        {
            string sqlQuery;
            SqlParameter[] sqlParams;
            int intDay;

            idx = idx / 3;
            idx = idx / id;

            intDay = DateTime.Now.Day;

            if (idx != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sqlQuery = "Exec SP_Seg_Upd_UsuarioExternoActivar  @Usuario_id, @Clave";

            sqlParams = new SqlParameter[]
                    {
                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Clave",  Value = Pw, Direction = System.Data.ParameterDirection.Input }
                    };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            if (resultado[0].respuesta == 1)
            {


                String Query;

                Query = "Select * ";
                Query += "from Tbl_Seg_UsuarioExterno ";
                Query += "Where  Usuario_id = " + id.ToString();

                List<Tbl_Seg_UsuarioExterno> LstUsuario = new List<Tbl_Seg_UsuarioExterno>();

                LstUsuario = db.Tbl_Seg_UsuarioExterno.SqlQuery(Query).ToList();

                if (LstUsuario.Count() > 0)
                {
                    Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = LstUsuario.First();

                    ViewBag.Usuario = tbl_Seg_UsuarioExterno.Correo;
                    ViewBag.Password = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Desencriptar(@p0)", tbl_Seg_UsuarioExterno.Clave).FirstOrDefault();
                    ViewBag.idk = tbl_Seg_UsuarioExterno.Usuario_id;
                }

                ViewBag.Mensaje = resultado[0].mensaje;
                @ViewBag.Mensaje_II = "Su usuario ha sido activado.";
                ViewBag.RegistroGrabado = 1;
            }
            else
            {
                ViewBag.Mensaje = resultado[0].mensaje;
                ViewBag.RegistroGrabado = 0;
            }


            return View();
        }

        public ActionResult ConfirmacionGuid(string guidid)
        {
            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Seg_Upd_UsuarioExternoActivarGuid @guidid";

            sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@guidid",  Value = guidid, Direction = System.Data.ParameterDirection.Input }
                    };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            if (resultado[0].respuesta == 1)
            {


                String Query;

                Query = "Select * ";
                Query += "from Tbl_Seg_UsuarioExterno ";
                Query += "Where  Usuario_id = " + resultado[0].id.ToString();

                List<Tbl_Seg_UsuarioExterno> LstUsuario = new List<Tbl_Seg_UsuarioExterno>();

                LstUsuario = db.Tbl_Seg_UsuarioExterno.SqlQuery(Query).ToList();

                if (LstUsuario.Count() > 0)
                {
                    Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = LstUsuario.First();

                    ViewBag.Usuario = tbl_Seg_UsuarioExterno.Correo;
                    ViewBag.Password = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Desencriptar(@p0)", tbl_Seg_UsuarioExterno.Clave).FirstOrDefault();
                    ViewBag.idk = tbl_Seg_UsuarioExterno.Usuario_id;
                }

                ViewBag.Mensaje = resultado[0].mensaje;
                @ViewBag.Mensaje_II = "Su usuario ha sido activado.";
                ViewBag.RegistroGrabado = 1;
            }
            else
            {
                ViewBag.Mensaje = resultado[0].mensaje;
                ViewBag.RegistroGrabado = 0;
            }

            return View();
        }

        // GET: UsuarioExterno/Limpiar
        public ActionResult Limpiar()
        {
            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec Sp_Limpiar @Usuario_id";

            sqlParams = new SqlParameter[] {
            new SqlParameter { ParameterName = "@Usuario_id", Value =1, Direction = System.Data.ParameterDirection.Input }
            };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return RedirectToAction("Index", "Login");


        }

        public ActionResult ReenviarContrasenia(string mail, string clave)
        {

            if (mail != null)
            {
                //if (Session[Constants.session_Captcha].ToString().Equals(clave) == true)
                if (true == true)
                {

                    string sqlQuery;
                    SqlParameter[] sqlParams;

                    sqlQuery = "Exec SP_Seg_Upd_UsuarioExternoReenviarClave @Email";

                    List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>

                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };
                    sqlParams = new SqlParameter[] {
                                            new SqlParameter { ParameterName = "@Email", Value =mail, Direction = System.Data.ParameterDirection.Input }
                                                    };

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    if (resultado[0].respuesta == 0)
                    {
                        TempData["Error"] = resultado[0].mensaje;

                    }
                    else
                    {
                        TempData["Error"] = " Se ha enviado correo electrónico y confirmación por Whatsapp.";
                        EnvioCorreo(mail, "Reenvío de contraseña para el sistema -SERNAF-  Sistema electrónico de Registro Nacional Forestal", resultado[0].mensaje);
                    }
                }
                else
                {
                    TempData["Error"] = " La contraseña no fue ingresada de forma correcta.";
                }
            }
            return View();
        }

        // GET: UsuarioExterno/Create
        public ActionResult Create()
        {
            int Usuario_id = 0;

            ViewBag.RegistroGrabado = 0;
            @ViewBag.Mensaje = "";

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                Usuario_id = 0;
            }
            else
            {
                Usuario objUs = (Usuario)Session["User"];
                Usuario_id = 0;
            }

            ViewBag.Estado_id = new SelectList(db.Tbl_Seg_UsuarioExterno_Estado, "Estado_id", "Descripcion");

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = new Tbl_Seg_UsuarioExterno();

            tbl_Seg_UsuarioExterno.swdatecreated = DateTime.Now;
            tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
            tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
            tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

            tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 1;
            tbl_Seg_UsuarioExterno.No_Documento = "";
            tbl_Seg_UsuarioExterno.No_NIT = "";
            tbl_Seg_UsuarioExterno.DepartamentoDPI_id = 7;
            tbl_Seg_UsuarioExterno.MunicipioDPI_id = 74;
            tbl_Seg_UsuarioExterno.Direccion = "";
            tbl_Seg_UsuarioExterno.Municipio_id = 74;
            tbl_Seg_UsuarioExterno.Departamento_id = 7;
            tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 1;

            tbl_Seg_UsuarioExterno.Telefono_Oficina = "";
            tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension = "";
            tbl_Seg_UsuarioExterno.Sexo_id = 1;

            tbl_Seg_UsuarioExterno.Fecha_Nacimiento = DateTime.Now;
            tbl_Seg_UsuarioExterno.Grado_Academico_Tecnico = false;
            tbl_Seg_UsuarioExterno.Grado_Academico_Profesional = false;
            tbl_Seg_UsuarioExterno.No_Colegiado = "---";
            tbl_Seg_UsuarioExterno.PostGradoMateriaForestal = false;
            tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "---";
            tbl_Seg_UsuarioExterno.Universidad = "---";
            tbl_Seg_UsuarioExterno.Profesion_id = 0;
            tbl_Seg_UsuarioExterno.CambioPW = true;

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);


            return View(tbl_Seg_UsuarioExterno);
        }

        // POST: UsuarioExterno/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno, FormCollection Collection)
        {
            bool ErrorDetectado = false;
            string ClaveConfirmacion = Collection["ClaveConfirmacion"];

            ViewBag.ClaveConfirmacion = ClaveConfirmacion;

            TempData["Mensaje"] = "";

            if (ClaveConfirmacion != tbl_Seg_UsuarioExterno.Clave)
            {
                ErrorDetectado = true;
                TempData["Mensaje"] = "No concuerda la contraseña con la confirmación de la misma.";
                ViewBag.RegistroGrabado = 0;
            }

            var UsuarioExternoEncontrado = db.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.Correo == tbl_Seg_UsuarioExterno.Correo);

            if (UsuarioExternoEncontrado.Count() > 0)
            {
                ErrorDetectado = true;
                TempData["Mensaje"] = "El correo ha sido registrado previamente, seleccione la opción de reenvio de contraseña para poder ingresar. Este error puede ser originado cuando no ha procedido a activar su usuario dando Click sobre el link enviado.";
                ViewBag.RegistroGrabado = 0;

            }

            UsuarioExternoEncontrado = db.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.No_Documento == tbl_Seg_UsuarioExterno.No_Documento);


            if (UsuarioExternoEncontrado.Count() > 0)
            {

                string strmail = db.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.No_Documento == tbl_Seg_UsuarioExterno.No_Documento).First().Correo;

                ErrorDetectado = true;
                TempData["Mensaje"] = "El usuario ha sido registrado previamente. Con la siguiente dirección de correo : " + strmail + ", debe ir al menú de login, la opción de reenvio de contraseña.";
                ViewBag.RegistroGrabado = 0;

            }

            if ((!DPI_Valido(tbl_Seg_UsuarioExterno.No_Documento)) && (tbl_Seg_UsuarioExterno.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }

            if ((!No_NIT_Valido(tbl_Seg_UsuarioExterno.No_NIT)) && (tbl_Seg_UsuarioExterno.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }

            if ((!DPI_Valido(tbl_Seg_UsuarioExterno.No_Documento)) && (tbl_Seg_UsuarioExterno.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }


            if (DiferenciaAnio(tbl_Seg_UsuarioExterno.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }

            if (ErrorDetectado == false)
            {


                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Seg_UsuarioExterno.Max(u => u.Usuario_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                if (ModelState.IsValid)
                {

                    tbl_Seg_UsuarioExterno.Usuario_id = lngIdt;

                    tbl_Seg_UsuarioExterno.UsuarioExternoGuid_id = Guid.NewGuid();

                    db.Tbl_Seg_UsuarioExterno.Add(tbl_Seg_UsuarioExterno);
                    db.SaveChanges();

                    string sqlQuery;
                    SqlParameter[] sqlParams;

                    sqlQuery = "Exec SP_Seg_InsUpd_UsuarioExterno  @Usuario_id, @Correo, @Clave, @Nombres, @Apellidos, @CambioPW, @Telefono_Celular, @Telefono_Oficina,@Telefono_Oficina_Extension, @Estado_id, @UsuarioCreaActualiza_id, @Usuario_interno";

                    tbl_Seg_UsuarioExterno.Correo = (tbl_Seg_UsuarioExterno.Correo == null) ? "" : tbl_Seg_UsuarioExterno.Correo;
                    tbl_Seg_UsuarioExterno.Clave = (tbl_Seg_UsuarioExterno.Clave == null) ? "" : tbl_Seg_UsuarioExterno.Clave;
                    tbl_Seg_UsuarioExterno.Nombres = (tbl_Seg_UsuarioExterno.Nombres == null) ? "" : tbl_Seg_UsuarioExterno.Nombres;
                    tbl_Seg_UsuarioExterno.Apellidos = (tbl_Seg_UsuarioExterno.Apellidos == null) ? "" : tbl_Seg_UsuarioExterno.Apellidos;
                    tbl_Seg_UsuarioExterno.Telefono_Celular = (tbl_Seg_UsuarioExterno.Telefono_Celular == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Celular;
                    tbl_Seg_UsuarioExterno.Telefono_Oficina = (tbl_Seg_UsuarioExterno.Telefono_Oficina == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina;
                    tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension = (tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension;
                    tbl_Seg_UsuarioExterno.swupdatedby = 0;

                    int CambioPW = 1;
                    int Usuario_interno = (tbl_Seg_UsuarioExterno.swupdateUsuarioInterno == true) ? 1 : 0;

                    sqlParams = new SqlParameter[]
                     {

                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = tbl_Seg_UsuarioExterno.Usuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Correo",  Value = tbl_Seg_UsuarioExterno.Correo, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Clave",  Value = tbl_Seg_UsuarioExterno.Clave, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Nombres",  Value = tbl_Seg_UsuarioExterno.Nombres, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Apellidos",  Value = tbl_Seg_UsuarioExterno.Apellidos, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@CambioPW",  Value = CambioPW, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Celular",  Value = tbl_Seg_UsuarioExterno.Telefono_Celular, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina_Extension", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Estado_id", Value = tbl_Seg_UsuarioExterno.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@UsuarioCreaActualiza_id", Value = tbl_Seg_UsuarioExterno.swupdatedby, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Usuario_interno", Value = Usuario_interno, Direction = System.Data.ParameterDirection.Input }
                     };

                    List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    if (resultado[0].respuesta == 1)
                    {

                        EnvioCorreo(tbl_Seg_UsuarioExterno.Correo, "Proceso de activación en el sistema -SERNAF-  Sistema electrónico de Registro Nacional Forestal", resultado[0].mensaje);

                        TempData["Mensaje"] = "Registro grabado de forma exitosa.";
                        @ViewBag.Mensaje_II = "IMPORTANTE: Se ha enviado un correo a la dirección " + tbl_Seg_UsuarioExterno.Correo + ", Favor dar Click en el link para activar la cuenta, o bien a través de Whatsapp.";
                        ViewBag.RegistroGrabado = 1;
                    }
                    else
                    {
                        TempData["Mensaje"] = resultado[0].mensaje;
                        ViewBag.RegistroGrabado = 0;
                    }

                }
                else
                {


                    ViewBag.RegistroGrabado = 0;
                }
            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);

            ViewBag.Estado_id = new SelectList(db.Tbl_Seg_UsuarioExterno_Estado, "Estado_id", "Descripcion", tbl_Seg_UsuarioExterno.Estado_id);

            return View(tbl_Seg_UsuarioExterno);
        }

        // GET: UsuarioExterno/Edit/5
        public ActionResult Edit()
        {
            long Usuario_id = 0;

            ViewBag.RegistroGrabado = 0;
            @ViewBag.Mensaje = "";

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                Usuario_id = 0;
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                Usuario objUs = (Usuario)Session["User"];
                Usuario_id = objUs.intUsuario_id;
            }

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(Usuario_id);

            tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
            tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
            tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

            tbl_Seg_UsuarioExterno.Clave = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Desencriptar(@p0)", tbl_Seg_UsuarioExterno.Clave).FirstOrDefault();

            if (tbl_Seg_UsuarioExterno.No_Documento == "0")
            {
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 0;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.DepartamentoDPI_id = 7;
                tbl_Seg_UsuarioExterno.MunicipioDPI_id = 74;
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.Municipio_id = 74;
                tbl_Seg_UsuarioExterno.Departamento_id = 7;
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 0;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 0;


                //temp
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 1;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 1;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 1;

                //temp


            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);

            tbl_Seg_UsuarioExterno.CambioPW = true;

            return View(tbl_Seg_UsuarioExterno);
        }

        // GET: UsuarioExterno/Edit/5
        public ActionResult UsuarioExView(long id, string email)
        {
            long Usuario_id = id;

            ViewBag.RegistroGrabado = 0;
            @ViewBag.Mensaje = "";

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                Usuario_id = 0;
            }
            else
            {
                Usuario objUs = (Usuario)Session["User"];

            }

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(Usuario_id);

            if (tbl_Seg_UsuarioExterno.Correo != email)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
            tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
            tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

            tbl_Seg_UsuarioExterno.Clave = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Desencriptar(@p0)", tbl_Seg_UsuarioExterno.Clave).FirstOrDefault();

            if (tbl_Seg_UsuarioExterno.No_Documento == "0")
            {
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 0;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.DepartamentoDPI_id = 7;
                tbl_Seg_UsuarioExterno.MunicipioDPI_id = 74;
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.Municipio_id = 74;
                tbl_Seg_UsuarioExterno.Departamento_id = 7;
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 0;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 0;


                //temp
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 1;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 1;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 1;

                //temp


            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);

            tbl_Seg_UsuarioExterno.CambioPW = true;

            return View(tbl_Seg_UsuarioExterno);
        }

        // POST: UsuarioExterno/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno, FormCollection Collection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            string ClaveConfirmacion = Collection["ClaveConfirmacion"];
            string ClaveNueva = Collection["ClaveNueva"];


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


            TempData["Mensaje"] = "";
            bool ErrorDetectado = false;


            if (ClaveConfirmacion != null && ClaveConfirmacion != "")
            {
                bool Passwordmatched = Regex.Match(ClaveConfirmacion, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$").Success;

                if (Passwordmatched == false)
                {
                    ErrorDetectado = true;
                    TempData["Mensaje"] = TempData["Mensaje"] + " La Contraseña debe contener mayusculas, minusculas, numeros y al menos un caracter especial.";
                }
                if ((ClaveConfirmacion.Length < 8) && (ClaveConfirmacion.Length > 25))
                {
                    ErrorDetectado = true;
                    TempData["Mensaje"] = TempData["Mensaje"] + " La longitud del password debe estar entre 8 y 25 caracteres. No se actualizo el registro.";
                }


                if (ClaveConfirmacion != ClaveNueva)
                {
                    ErrorDetectado = true;
                    TempData["Mensaje"] = TempData["Mensaje"] + " No concuerda la contraseña de confirmación. No se actualizó la contraseña.";
                }
                else
                {
                    tbl_Seg_UsuarioExterno.Clave = ClaveConfirmacion;
                }
            }

            
            tbl_Seg_UsuarioExterno.No_Documento = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_Documento_Formateado]('{(tbl_Seg_UsuarioExterno.No_Documento ?? "")}')").FirstOrDefault() ?? "";

            tbl_Seg_UsuarioExterno.No_CasilleroElectronico = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_NoCasillero_Formateado]('{(tbl_Seg_UsuarioExterno.No_CasilleroElectronico ?? "")}')").FirstOrDefault() ?? "";



            if (!ConsultarCasilleroElectronico(new CasilleroElectronicoRequest { No_CasilleroElectronico = tbl_Seg_UsuarioExterno.No_CasilleroElectronico, No_Documento = tbl_Seg_UsuarioExterno.No_Documento }))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "** Número de casillero invalido";
                ErrorDetectado = true;
            }

            if ((!No_NIT_Valido(tbl_Seg_UsuarioExterno.No_NIT)) && (tbl_Seg_UsuarioExterno.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_Seg_UsuarioExterno.No_Documento)) && (tbl_Seg_UsuarioExterno.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es valido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_Seg_UsuarioExterno.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }


            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_Seg_UsuarioExterno).State = EntityState.Modified;
                db.SaveChanges();
                // SP_Seg_UsuarioExterno_EncriptarContraseña


                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Seg_UsuarioExterno_EncriptarContraseña  @Usuario_id, @Correo, @Clave, @Nombres, @Apellidos, @CambioPW, @Telefono_Celular, @Telefono_Oficina,@Telefono_Oficina_Extension, @Estado_id, @UsuarioCreaActualiza_id, @Usuario_interno";

                tbl_Seg_UsuarioExterno.Telefono_Oficina = (tbl_Seg_UsuarioExterno.Telefono_Oficina == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina;
                tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension = (tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension;


                sqlParams = new SqlParameter[]
                 {

                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = tbl_Seg_UsuarioExterno.Usuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Correo",  Value = tbl_Seg_UsuarioExterno.Correo, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Clave",  Value = tbl_Seg_UsuarioExterno.Clave, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Nombres",  Value = tbl_Seg_UsuarioExterno.Nombres, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Apellidos",  Value = tbl_Seg_UsuarioExterno.Apellidos, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@CambioPW",  Value = 1, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Celular",  Value = tbl_Seg_UsuarioExterno.Telefono_Celular, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina_Extension", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Estado_id", Value = tbl_Seg_UsuarioExterno.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@UsuarioCreaActualiza_id", Value = tbl_Seg_UsuarioExterno.swupdatedby, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Usuario_interno", Value = 0, Direction = System.Data.ParameterDirection.Input },


                 };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                objUs.CambioPW = true;

                Session[Constants.session_User] = objUs;

                TempData["MensajeHome"] = "     Datos actualizados con éxito.";
                TempData["MensajeHomeTwo"] = "      Puede utilizar las distintas opciones según la gestión requerida.";

                return RedirectToAction("../Home/Index");

            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);



            return View(tbl_Seg_UsuarioExterno);
        }



        // GET: UsuarioExterno/Edit/5
        public ActionResult EditProfesional()
        {
            long Usuario_id = 0;

            ViewBag.RegistroGrabado = 0;
            @ViewBag.Mensaje = "";

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                Usuario_id = 0;
            }
            else
            {
                Usuario objUs = (Usuario)Session["User"];
                Usuario_id = objUs.intUsuario_id;
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            ViewBag.SubCategoria = tbl_Sol_Solicitud.Sub_Categoria_id;

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_Sol_Solicitud.swcreatedby);

            tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
            tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
            tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

            tbl_Seg_UsuarioExterno.Clave = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Desencriptar(@p0)", tbl_Seg_UsuarioExterno.Clave).FirstOrDefault();

            if (tbl_Seg_UsuarioExterno.No_Documento == "0")
            {
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 0;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.DepartamentoDPI_id = 7;
                tbl_Seg_UsuarioExterno.MunicipioDPI_id = 74;
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.Municipio_id = 74;
                tbl_Seg_UsuarioExterno.Departamento_id = 7;
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 0;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 0;


                //temp
                tbl_Seg_UsuarioExterno.swdateupdated = DateTime.Now;
                tbl_Seg_UsuarioExterno.swupdatedby = Usuario_id;
                tbl_Seg_UsuarioExterno.swupdateUsuarioInterno = false;

                tbl_Seg_UsuarioExterno.DocumentoID_Tipo = 1;
                tbl_Seg_UsuarioExterno.No_Documento = "";
                tbl_Seg_UsuarioExterno.No_NIT = "";
                tbl_Seg_UsuarioExterno.Direccion = "";
                tbl_Seg_UsuarioExterno.PuebloPertenencia_id = 1;
                tbl_Seg_UsuarioExterno.No_Colegiado = "";
                tbl_Seg_UsuarioExterno.PostGradoEspecialidad = "";
                tbl_Seg_UsuarioExterno.Universidad = "";
                tbl_Seg_UsuarioExterno.Profesion_id = 0;

                tbl_Seg_UsuarioExterno.Sexo_id = 1;

                //temp


            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);

            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_Seg_UsuarioExterno.Profesion_id);

            if ((tbl_Seg_UsuarioExterno.Grado_Academico_Tecnico == false) && (tbl_Seg_UsuarioExterno.Grado_Academico_Profesional == false))
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Profesion_id == 0), "Profesion_id", "Descripcion", tbl_Seg_UsuarioExterno.Profesion_id);
            }

            if (tbl_Seg_UsuarioExterno.Grado_Academico_Tecnico == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Tecnico == true), "Profesion_id", "Descripcion", tbl_Seg_UsuarioExterno.Profesion_id);
            }

            if (tbl_Seg_UsuarioExterno.Grado_Academico_Profesional == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Profesional == true), "Profesion_id", "Descripcion", tbl_Seg_UsuarioExterno.Profesion_id);
            }



            tbl_Seg_UsuarioExterno.CambioPW = true;

            return View(tbl_Seg_UsuarioExterno);
        }

        // POST: UsuarioExterno/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfesional(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno, FormCollection Collection)
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


            TempData["Mensaje"] = "";
            bool ErrorDetectado = false;


            if ((!No_NIT_Valido(tbl_Seg_UsuarioExterno.No_NIT)) && (tbl_Seg_UsuarioExterno.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_Seg_UsuarioExterno.No_Documento)) && (tbl_Seg_UsuarioExterno.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_Seg_UsuarioExterno.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }

            if (tbl_Seg_UsuarioExterno.Grado_Academico_Profesional == false && tbl_Seg_UsuarioExterno.Grado_Academico_Tecnico == false)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar su grado profesional ó tecnico.";
                ErrorDetectado = true;
            }

            if (tbl_Seg_UsuarioExterno.Profesion_id == 0)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: No ha seleccionado su profesion";
                ErrorDetectado = true;
            }

            if ((tbl_Seg_UsuarioExterno.Grado_Academico_Profesional == true) && ((tbl_Seg_UsuarioExterno.Universidad == "") || (tbl_Seg_UsuarioExterno.No_Colegiado == "")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y número de colegiado.";
                ErrorDetectado = true;
            }

            if ((tbl_Seg_UsuarioExterno.PostGradoMateriaForestal == true) && ((tbl_Seg_UsuarioExterno.PostGradoEspecialidad == "") || (tbl_Seg_UsuarioExterno.PostGradoUniversidad == "")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y detalles del postgrado obtenido.";
                ErrorDetectado = true;
            }


            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_Seg_UsuarioExterno).State = EntityState.Modified;
                db.SaveChanges();
                // SP_Seg_UsuarioExterno_EncriptarContraseña


                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Seg_UsuarioExterno_EncriptarContraseña  @Usuario_id, @Correo, @Clave, @Nombres, @Apellidos, @CambioPW, @Telefono_Celular, @Telefono_Oficina,@Telefono_Oficina_Extension, @Estado_id, @UsuarioCreaActualiza_id, @Usuario_interno";

                tbl_Seg_UsuarioExterno.Telefono_Oficina = (tbl_Seg_UsuarioExterno.Telefono_Oficina == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina;
                tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension = (tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension == null) ? "" : tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension;


                sqlParams = new SqlParameter[]
                 {

                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = tbl_Seg_UsuarioExterno.Usuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Correo",  Value = tbl_Seg_UsuarioExterno.Correo, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Clave",  Value = tbl_Seg_UsuarioExterno.Clave, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Nombres",  Value = tbl_Seg_UsuarioExterno.Nombres, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Apellidos",  Value = tbl_Seg_UsuarioExterno.Apellidos, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@CambioPW",  Value = 1, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Celular",  Value = tbl_Seg_UsuarioExterno.Telefono_Celular, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Telefono_Oficina_Extension", Value = tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Estado_id", Value = tbl_Seg_UsuarioExterno.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@UsuarioCreaActualiza_id", Value = tbl_Seg_UsuarioExterno.swupdatedby, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Usuario_interno", Value = 0, Direction = System.Data.ParameterDirection.Input },


                 };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                objUs.CambioPW = true;

                Session[Constants.session_User] = objUs;

                TempData["MensajeHome"] = "     Datos actualizados con éxito.";
                TempData["MensajeHomeTwo"] = "      Puede utilizar las distintas opciones según la gestión requerida.";

                return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = (long)Session[Constants.session_Solicitud] });

            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.Departamento_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.Municipio_id);


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Seg_UsuarioExterno.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Seg_UsuarioExterno.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Seg_UsuarioExterno.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Seg_UsuarioExterno.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Seg_UsuarioExterno.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Seg_UsuarioExterno.Sexo_id);
            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_Seg_UsuarioExterno.Profesion_id);



            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = (long)Session[Constants.session_Solicitud] });
        }


        public int DiferenciaAnio(DateTime Fecha)
        {
            int intDias;
            TimeSpan dias = DateTime.Now.Subtract(Fecha);

            intDias = dias.Days;

            return intDias / 364;
        }


        public bool No_NIT_Valido(string No_NIT)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_NIT_Valido(@p0)", No_NIT).FirstOrDefault(); ;
        }

        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }


        public FileResult WriteTextAsImage(string id, int? width, int? height)
        {
            int textSize = 20;
            if (!width.HasValue)
            {
                width = 245;
            }

            if (!height.HasValue)
            {
                height = 40;
            }

            Response.ContentType = "image/jpeg";
            string textToWrite = id;
            string[] s = textToWrite.Split('|');
            textToWrite = textToWrite.Replace("|", "\n");


            if (textToWrite.Trim().Length > 0)
            {
                Bitmap image = new Bitmap(width.Value, height.Value);
                Graphics g = null;
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        g = Graphics.FromImage(image);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        Font f = new Font("Verdana", textSize, FontStyle.Regular);
                        SolidBrush b = new SolidBrush(Color.White);
                        g.FillRectangle(b, 0, 0, width.Value, height.Value);
                        g.DrawString(textToWrite, f, Brushes.Blue, 2, 3);
                        g.DrawLine(new Pen(Color.Black, 1), new Point(5, 25), new Point(100, 0));

                        f.Dispose();
                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return File(stream.ToArray(), "image/jpeg");
                    }
                }
                catch (Exception exe)
                {
                    throw;
                }
                finally
                {
                    image.Dispose();
                    g.Dispose();
                }
            }
            else
            {
                return File("", "");
            }
        }


        public ActionResult VisualizarMapa()
        {
            ViewBag.Fotografia = "/Content/images/Guatemala.png";

            string Nombre_Archivo = "TierraVocacionForestal.jpg";

            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Machotes/Mapa/"), Nombre_Archivo);
            if ((System.IO.File.Exists(PathArchivo)) == true)
            {
                ViewBag.Fotografia = "~/Archivos_Generados_Que_Pueden_Borrar/Mapa/" + Nombre_Archivo;
            }
            return View();
        }

        public ActionResult Mapa()
        {
            ViewBag.Fotografia = "/Content/images/Guatemala.png";

            string Nombre_Archivo = "TierraVocacionForestal.jpg";

            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Generados_Que_Pueden_Borrar/Mapa/"), Nombre_Archivo);
            if ((System.IO.File.Exists(PathArchivo)) == true)
            {
                ViewBag.Fotografia = "/Archivos_Generados_Que_Pueden_Borrar/Mapa/" + Nombre_Archivo;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Mapa(HttpPostedFileBase postedFiles)
        {

            if (postedFiles.ContentType.Contains("image") == false)
            {
                TempData["MessageFoto"] = "Solamente se aceptan fotografias.";
                return RedirectToAction("Fotografia", "UsuarioExterno");

            }

            string Nombre_Archivo = "TierraVocacionForestal.jpg";


            string PathArchivo = Path.Combine(Server.MapPath("~/Archivos_Generados_Que_Pueden_Borrar/Mapa/"), Nombre_Archivo);
            if ((System.IO.File.Exists(PathArchivo)) == true)
            {
                System.IO.File.Delete(PathArchivo);
            }
            else
            {
                string PathCrear = "~/Archivos_Generados_Que_Pueden_Borrar/Mapa";

                if (!Directory.Exists(Server.MapPath(PathCrear)))
                    Directory.CreateDirectory(Server.MapPath(PathCrear));

            }

            postedFiles.SaveAs(PathArchivo);

            ViewBag.Fotografia = PathArchivo;
            return RedirectToAction("Mapa", "UsuarioExterno");

        }



        public JsonResult PasswordTemporal(long usuario_id, string no_documento)
        {

            string sqlQuery;


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "", respuesta = 0 }  };


                sqlQuery = "Exec SP_Seg_PW_Temporal_UsuarioExterno @Usuario_id, @No_Documento, @swupdatedby, @swupdatebyinterno";
                SqlParameter[] sqlParams;


                sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = usuario_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@No_Documento",  Value = no_documento, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@swupdatebyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
               };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            }


            return Json("");
        }


        public JsonResult UpdateCorreo(long usuario_id, string no_documento, string correo)
        {

            string sqlQuery;


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "", respuesta = 0 }  };


                sqlQuery = "Exec SP_Seg_UsuarioExterno_UpdCorreo @Usuario_id, @No_Documento, @correo, @swupdatedby, @swupdatebyinterno";
                SqlParameter[] sqlParams;


                sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = usuario_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@No_Documento",  Value = no_documento, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@correo",  Value = correo, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@swupdatebyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
               };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            }


            return Json("");
        }




        public void EnvioCorreo(string Mail, string Motivo, string Mensaje)
        {
            System.Net.Mail.MailMessage Correo = new System.Net.Mail.MailMessage();
            Correo.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], "INAB Administrador");
            Correo.To.Add(new MailAddress(Mail));
            Correo.Subject = Motivo;
            AlternateView HTMLConImagenes = default(AlternateView);
            HTMLConImagenes = AlternateView.CreateAlternateViewFromString(Mensaje, null, "text/html");

            //HTMLConImagenes.LinkedResources.Add(imagen);
            Correo.AlternateViews.Add(HTMLConImagenes);
            Correo.IsBodyHtml = true;
            Correo.Priority = System.Net.Mail.MailPriority.High;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["Host"].ToString(), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Puerto"]));
            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], System.Configuration.ConfigurationManager.AppSettings["Clave"]);
            smtp.Send(Correo);
            return;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

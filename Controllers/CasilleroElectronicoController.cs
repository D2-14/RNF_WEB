using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using System.Data.SqlClient;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace RNF_Web.Controllers
{
    public class CasilleroElectronicoController : Controller
    {
        //Alta de Casillero electronico
        private db_RNFEntities db = new db_RNFEntities();
        RequestUtil requestUtil = new RequestUtil();


        public bool ConsultarCasilleroElectronico(CasilleroElectronicoRequest model)
        {
            if (((model.No_CasilleroElectronico ?? "").Trim() == "") || ((model.No_Documento ?? "") == ""))
            {
                return false;
            }
            RequestUtil requestUtil = new RequestUtil();

            return requestUtil.Execute_Consultar_CasilleroElectronico(model); ;
        }


        public ActionResult RegistroCasillero()
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                return RedirectToAction("AccesoDenegado", "Home");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.MensajeError = "";

            return View();
        }



        [HttpPost]
        public ActionResult RegistroCasillero(FormCollection Collection)
        {
            ViewBag.MensajeError = "";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                return RedirectToAction("AccesoDenegado", "Home");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            CasilleroElectronicoRequest model = new CasilleroElectronicoRequest();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

            model.No_Documento = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_Documento_Formateado]('{tbl_Seg_UsuarioExterno.No_Documento}')").FirstOrDefault() ?? "";

            model.No_CasilleroElectronico = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_NoCasillero_Formateado]('{Collection["NoCasillero"]}')").FirstOrDefault() ?? "";

            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                if (Collection["NoCasillero"] == "2")
                {
                    model.No_Documento = "1987910620101";
                }
            }

            if (ConsultarCasilleroElectronico(model) == true)
            {
                try

                {

                    string sqlQuery;
                    SqlParameter[] sqlParams;

                    sqlQuery = "Exec SP_Seg_UsuarioExterno_ActualizaCasillero  @Usuario_id, @NoCasillero";

                    sqlParams = new SqlParameter[]
                     {
                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = tbl_Seg_UsuarioExterno.Usuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@NoCasillero",  Value = model.No_CasilleroElectronico, Direction = System.Data.ParameterDirection.Input }

                     };


                    List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


                }
                catch (Exception exep)
                {
                    Console.WriteLine(exep.Message);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.MensajeError = "** Número de casillero invalido";
            return View();
        }



        public ActionResult ActualizarCasillero()
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                return RedirectToAction("AccesoDenegado", "Home");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.MensajeError = "";

            return View();
        }



        [HttpPost]
        public ActionResult ActualizarCasillero(FormCollection Collection)
        {
            ViewBag.MensajeError = "";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                return RedirectToAction("AccesoDenegado", "Home");

            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 1)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            CasilleroElectronicoRequest model = new CasilleroElectronicoRequest();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

            model.No_Documento = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_Documento_Formateado]('{tbl_Seg_UsuarioExterno.No_Documento}')").FirstOrDefault() ?? "";

            model.No_CasilleroElectronico = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_NoCasillero_Formateado]('{Collection["NoCasillero"]}')").FirstOrDefault() ?? "";

            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                if (Collection["NoCasillero"] == "2")
                {
                    model.No_Documento = "1987910620101";
                }
            }

            if (ConsultarCasilleroElectronico(model) == true)
            {
                try

                {

                    string sqlQuery;
                    SqlParameter[] sqlParams;

                    sqlQuery = "Exec SP_Seg_UsuarioExterno_ActualizaCasillero  @Usuario_id, @NoCasillero";

                    sqlParams = new SqlParameter[]
                     {
                                        new SqlParameter { ParameterName = "@Usuario_id",  Value = tbl_Seg_UsuarioExterno.Usuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@NoCasillero",  Value = model.No_CasilleroElectronico, Direction = System.Data.ParameterDirection.Input }

                     };


                    List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


                }
                catch (Exception exep)
                {
                    Console.WriteLine(exep.Message);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.MensajeError = "** Número de casillero invalido";
            return View();
        }




        class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }



        public async Task<ActionResult> EnviarNotificacion_CasilleroElectronico()
        {

            RespuestaJSON respuestaJSON = new RespuestaJSON
            {
                Result = 0,
                Mensaje = "No se realizó ninguna gestión",
            };

            CasilleroElectronicoSender casilleroElectronicoSender = new CasilleroElectronicoSender();

            _ = casilleroElectronicoSender.EnviarCasilleroPendiente();


            return Json(respuestaJSON);
        }


        public class RespuestaJSON_Bearer
        {
            public int CodRespuesta { get; set; }
            public string strRespuesta { get; set; }
        }

        public JsonResult GetBearerCasillero(string username, string password)
        {
            int intRespuesta = 0;
            string strRespuesta = "";

            RespuestaJSON_Bearer respuestaJSON_Bearer = new RespuestaJSON_Bearer();

            Tbl_Gral_ParametrosGenerales tbl_Gral_ParametrosGenerales = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault();

            if ((username != Constants.CasilleroElectronicoUser_Out) || (password != Constants.CasilleroElectronicoPassword_Out))
            {
                respuestaJSON_Bearer = new RespuestaJSON_Bearer
                {
                    CodRespuesta = intRespuesta,
                    strRespuesta = "No se logró obtener el bearer. Usuario y Password invalidos",
                };
            }

            try
            {
                strRespuesta = GenerarToken(username);
                intRespuesta = 1;
            }
            catch (Exception ex)
            {
                intRespuesta = 0;
                strRespuesta = "Error:" + ex.Message.Substring(1, 100);
            }

            respuestaJSON_Bearer = new RespuestaJSON_Bearer
            {
                CodRespuesta = intRespuesta,
                strRespuesta = strRespuesta,
            };

            return Json(strRespuesta);
        }

        public async Task<ActionResult> Documento(string Codigo)
        {
            // Obtener token de la cabecera Authorization
            var tokenString = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Deserializar token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Obtener datos del token
            var sub = token.Claims.First(claim => claim.Type == "sub").Value;
            var jti = token.Claims.First(claim => claim.Type == "jti").Value;
            var exp = token.Claims.First(claim => claim.Type == "exp").Value;

            DateTime exp_dt = ConversorFechas.FromUnixTime(long.Parse(exp.ToString()));
            DateTime swdatenow = DateTime.Now;

            if (exp_dt > swdatenow)
            {
                try
                {

                    string archivobase64 = requestUtil.ObtieneArchivoFirmado<Credentials_GetBearer>(Constants.Address_GoogleDriveDownLoad, "GET", null, Codigo);
                    byte[] bytes = Convert.FromBase64String(archivobase64);

                    if ((bytes ?? new byte[] { }).Length > 1000)
                    {
                        string nombrearchivo = Codigo;
                        if (!nombrearchivo.ToLower().EndsWith(".pdf"))
                        {
                            nombrearchivo = nombrearchivo + ".pdf";
                        }
                        var contentResult = new FileContentResult(bytes, "application/pdf")
                        {
                            FileDownloadName = nombrearchivo
                        };
                        return contentResult;
                    }
                    else
                    {
                        string filePath = Path.Combine(Server.MapPath("~/"), "Content/Machotes/", "DOCUMENTO NO ENCONTRADO.pdf");
                        // Leer archivo PDF en segundo plano
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                        {
                            try
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await fileStream.CopyToAsync(memoryStream);
                                    var pdfBytes = memoryStream.ToArray();
                                    var base64String = Convert.ToBase64String(pdfBytes);
                                    var response = new { pdf = base64String };

                                    // Crear nuevo FileStream a partir de MemoryStream
                                    var newFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

                                    //Ahora el contenttype estará automatizado en caso de que se utilice otro formato de archivo
                                    string contenttype = MimeMapping.GetMimeMapping(filePath);
                                    var fileStreamResult = new FileStreamResult(newFileStream, contenttype);
                                    return fileStreamResult;
                                }
                            }
                            catch (Exception e)
                            {
                                // Manejar la excepción como se desee
                                Console.WriteLine(e.Message);
                                return new FileStreamResult(new MemoryStream(), "application/pdf");
                            }
                        }

                    }

                }
                catch
                {

                    var filePath = Path.Combine(Server.MapPath("~/"), "Archivos_ConFirmaElectronica", Codigo);

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePath = Path.Combine(Server.MapPath("~/"), "Content/Machotes/", "DOCUMENTO NO ENCONTRADO.pdf");
                    }

                    // Leer archivo PDF en segundo plano
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                    {
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await fileStream.CopyToAsync(memoryStream);
                                var pdfBytes = memoryStream.ToArray();
                                var base64String = Convert.ToBase64String(pdfBytes);
                                var response = new { pdf = base64String };

                                // Crear nuevo FileStream a partir de MemoryStream
                                var newFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

                                //Ahora el contenttype estará automatizado en caso de que se utilice otro formato de archivo
                                string contenttype = MimeMapping.GetMimeMapping(filePath);
                                var fileStreamResult = new FileStreamResult(newFileStream, contenttype);
                                return fileStreamResult;
                            }
                        }
                        catch (Exception e)
                        {
                            // Manejar la excepción como se desee
                            Console.WriteLine(e.Message);
                            return new FileStreamResult(new MemoryStream(), "application/pdf");
                        }
                    }

                }
            }
            else
            {
                // Devolver un archivo PDF vacío en caso de que el token haya expirado
                return new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
            }
        }

        private string GenerarToken(string username)
        {
            // Definir las claves de cifrado y firma
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clave_secreta_aqui"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definir las claims del usuario
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: "INSTITUTO NACIONAL DE BOSQUES",
                audience: "CASILLERO ELECTRONICO",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            // Devolver el token como cadena de texto
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public ActionResult CasillerosEntrampados()
        {

            List<fc_Cas_CasillerosEntrampados_Result> Mandatario = (from d in db.fc_Cas_CasillerosEntrampados()
                                                                    select d).ToList() ?? new List<fc_Cas_CasillerosEntrampados_Result>();
            return View(Mandatario);
        }


        [HttpPost]
        public JsonResult ReiniciarIntentos()
        {

            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Cas_CasilleroElectronicoReiniciar ";

            sqlParams = new SqlParameter[]
             {
             };


            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            return Json(resultado);

        }





    }
}

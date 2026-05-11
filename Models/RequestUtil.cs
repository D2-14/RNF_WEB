using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace RNF_Web.Models
{
    public class Credentials_GetBearer
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Credentials_GetBearerCasillero
    {
        public string usuario { get; set; }
        public string claveUsuario { get; set; }
    }

    public class DatosAPI
    {
        public string codigo { get; set; }
    }

    public class RequestUtil
    {
        public Api_Probosque_Get Reply_Probosque_Get { get; set; }
        public Api_Probosque_Post Reply_Probosque_Post { get; set; }
        public Api_PinpepOld_Get Reply_PinpepOld_Get { get; set; }
        public Api_PinpepOld_Post Reply_PinpepOld_Post { get; set; }
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

        private db_RNF_IntermediaEntities db_Intermedia = new db_RNF_IntermediaEntities();


        public RequestUtil()
        {
            Reply_Probosque_Get = new Api_Probosque_Get();
            Reply_Probosque_Post = new Api_Probosque_Post();
            Reply_PinpepOld_Get = new Api_PinpepOld_Get();
            Reply_PinpepOld_Post = new Api_PinpepOld_Post();
        }


        //public Api_Probosque_Get getExpedienteProbosque(string url, string codigo)
        //{
        //    string bearer = GetBearer();

        //    var client = new RestClient(url);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.GET);
        //    request.AddHeader("Authorization", bearer);
        //    //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
        //    request.AddParameter("codigo", codigo);
        //    IRestResponse response = client.Execute(request);
        //    JObject joResponse = JObject.Parse(response.Content);
        //    Reply_Probosque_Get = JsonConvert.DeserializeObject<Api_Probosque_Get>(JsonConvert.SerializeObject(joResponse));
        //    return Reply_Probosque_Get;

        //}

        string RegistroOK = "Registro encontrado";
        string RegistroNO = "No se encontró la información";
        int TimeOutRest = 300000;

        #region Central ConsultaAPIs

        private string GetBearerCasillero()
        {
            var client = new RestClient(Constants.Address_BearerCasillero);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");


            Credentials_GetBearerCasillero credenciales_bearer = new Credentials_GetBearerCasillero
            {
                usuario = "c2lzdGVtYXJuZg==",
                claveUsuario = "U2lzdDNtYXJuZjI0Nw==",
            };


            request.AddParameter("application/json", JsonConvert.SerializeObject(credenciales_bearer), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            CasilleroElectronicoResponse respuesta = JsonConvert.DeserializeObject<CasilleroElectronicoResponse>(response.Content.ToString());
            return "Bearer " + respuesta.data.ToString();

        }

        public string GetBearer()
        {
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var client = new RestClient(Constants.Address_Bearer);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            Credentials_GetBearer credenciales_bearer = new Credentials_GetBearer();
            credenciales_bearer.Username = Constants.RNF_Username;
            credenciales_bearer.Password = Constants.RNF_Password;
            request.AddParameter("application/json", JsonConvert.SerializeObject(credenciales_bearer), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            // Restablecer la validación del certificado para otras solicitudes (no recomendado en producción).
            //ServicePointManager.ServerCertificateValidationCallback = null;

            //string respuesta = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(response));
            return "Bearer " + response.Content.ToString().Replace("\"", "");

        }
        private string GetAbsoluteUri(string url, string parametros = "")
        {
            if (parametros == null)
            {
                parametros = "";
            }

            Uri queryparams = new Uri(url + parametros);
            return queryparams.AbsoluteUri;
        }
        private string ResultRequestAPICasillero<T>(string url, string method, T objectRequest, string parametros = "")
        {
            //string bearer = GetBearer();

            StreamWriter oStreamWriter;
            StreamReader oStreamReader;
            string result = "";

            method = method.ToUpper();

            //string link = GetAbsoluteUri(url, parametros);

            string json = JsonConvert.SerializeObject(objectRequest);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest request = WebRequest.Create(GetAbsoluteUri(url, parametros));
            request.Headers.Add("Authorization", GetBearerCasillero());
            request.Method = method;
            request.PreAuthenticate = true;
            request.ContentType = $"application/json;charset=utf-8";
            request.Timeout = TimeOutRest;

            if (method == "POST")
            {
                oStreamWriter = new StreamWriter(request.GetRequestStream());
                oStreamWriter.Write(json);
                oStreamWriter.Flush();
            }

            try
            {

                WebResponse oHttpResponse = request.GetResponse();
                oStreamReader = new StreamReader(oHttpResponse.GetResponseStream());
                result = oStreamReader.ReadToEnd();

            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex);
            }
            return result;
        }
        private string ResultRequestAPI<T>(string url, string method, T objectRequest, string parametros = "")
        {
            //string bearer = GetBearer();

            StreamWriter oStreamWriter;
            StreamReader oStreamReader;
            string result = "";

            method = method.ToUpper();

            //string link = GetAbsoluteUri(url, parametros);

            string json = JsonConvert.SerializeObject(objectRequest);
            WebRequest request = WebRequest.Create(GetAbsoluteUri(url, parametros));
            request.Headers.Add("Authorization", GetBearer());
            request.Method = method;
            request.PreAuthenticate = true;
            request.ContentType = $"application/json;charset=utf-8";
            request.Timeout = TimeOutRest;

            if (method == "POST")
            {
                oStreamWriter = new StreamWriter(request.GetRequestStream());
                oStreamWriter.Write(json);
                oStreamWriter.Flush();
            }

            var path = System.AppDomain.CurrentDomain.BaseDirectory;
            SqlServerTypes.Utilities.LoadNativeAssemblies(path);

            try
            {

                WebResponse oHttpResponse = request.GetResponse();
                oStreamReader = new StreamReader(oHttpResponse.GetResponseStream());
                result = oStreamReader.ReadToEnd();

            }
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //    Console.WriteLine(ex);
            //}

            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd();
                        Console.WriteLine(errorResponse);
                    }
                }

                Console.WriteLine(ex.Message);
                throw;
            }
            return result;
        }
        private string DescargarArchivosGenericos(string url, string rootpath)
        {
            string guidid = Guid.NewGuid().ToString();
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            byte[] buffer = client.DownloadData(request);

            if (buffer.Length < 1000)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream(buffer);
            string partialroot = "Archivos_Generados_Que_Pueden_Borrar/";
            string ubicacion = rootpath + partialroot + guidid + ".pdf";
            string ubicacionparcial = "../../" + partialroot + guidid + ".pdf";
            FileStream file = new FileStream(ubicacion, FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);
            file.Close();
            ms.Close();

            return ubicacionparcial;
        }

        private string ResultRequestAPIMeta<T>(string url, string method, T objectRequest, Tbl_Meta_ParametrosGenerales tbl_Meta_ParametrosGenerales, string parametros = "")
        {
            //string bearer = GetBearer();

            string result = "";

            method = method.ToUpper();

            string BearerMeta = tbl_Meta_ParametrosGenerales.WhatsappToken;

            //string link = GetAbsoluteUri(url, parametros);

            string json = JsonConvert.SerializeObject(objectRequest);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest request = WebRequest.Create(GetAbsoluteUri(url, parametros));
            request.Headers.Add("Authorization", $"Bearer {BearerMeta}");
            request.Method = method;
            request.PreAuthenticate = true;
            request.ContentType = $"application/json;charset=utf-8";
            request.Timeout = TimeOutRest;

            if (method == "POST")
            {
                using (StreamWriter oStreamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    oStreamWriter.Write(json);
                    oStreamWriter.Flush();
                }
            }

            try
            {

                using (WebResponse oHttpResponse = request.GetResponse())
                {
                    using (StreamReader oStreamReader = new StreamReader(oHttpResponse.GetResponseStream()))
                    {
                        result = oStreamReader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex);
            }
            return result;
        }

        private string ResultRequestAPI_Gestion_SEINEF<T>(string url, string method, T objectRequest, string parametros = "")
        {
            //string bearer = GetBearer();

            string result = "";

            method = method.ToUpper();

            //string link = GetAbsoluteUri(url, parametros);

            string json = JsonConvert.SerializeObject(objectRequest);
            WebRequest request = WebRequest.Create(GetAbsoluteUri(url, parametros));
            request.Method = method;
            request.PreAuthenticate = true;
            request.ContentType = $"application/json;charset=utf-8";
            request.Timeout = TimeOutRest;

            if (method == "POST")
            {
                using (StreamWriter oStreamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    oStreamWriter.Write(json);
                    oStreamWriter.Flush();
                }
            }

            try
            {

                using (WebResponse oHttpResponse = request.GetResponse())
                {
                    using (StreamReader oStreamReader = new StreamReader(oHttpResponse.GetResponseStream()))
                    {
                        result = oStreamReader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex);
            }
            return result;
        }

        private byte[] ResultRequestFile<T>(string url, string method, T objectRequest, string parametros = "")
        {
            //string bearer = GetBearer();
            MemoryStream resultStream = new MemoryStream();
            StreamWriter oStreamWriter;
            StreamReader oStreamReader;
            string result = "";

            method = method.ToUpper();

            //string link = GetAbsoluteUri(url, parametros);

            string json = JsonConvert.SerializeObject(objectRequest);

            WebRequest request = WebRequest.Create(GetAbsoluteUri(url, parametros));
            request.Headers.Add("Authorization", GetBearer());
            request.Method = method;
            request.PreAuthenticate = true;
            request.ContentType = $"application/json;charset=utf-8";
            request.Timeout = TimeOutRest;

            if (method == "POST")
            {
                oStreamWriter = new StreamWriter(request.GetRequestStream());
                oStreamWriter.Write(json);
                oStreamWriter.Flush();
            }

            try
            {

                WebResponse oHttpResponse = request.GetResponse();
                oStreamReader = new StreamReader(oHttpResponse.GetResponseStream());
                Stream stream = oHttpResponse.GetResponseStream();
                stream.CopyTo(resultStream);
                result = oStreamReader.ReadToEnd();

            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex);
            }
            return resultStream.ToArray();
        }

        #endregion


        #region Casillero Electrónico

        public bool Execute_Consultar_CasilleroElectronico(CasilleroElectronicoRequest model)
        {
            bool respuesta = false;
            string url = Constants.Address_ConsultarCasillero;
            string result = "";

            //if(model.No_CasilleroElectronico == "2")
            //{
            //    return true;
            //}

            if (model.No_CasilleroElectronico == null)
            {
                return false;
            }

            CasilleroElectronicoResponse casillero = new CasilleroElectronicoResponse
            {
                status = -10,
                message = "No se ha ejecutado",
            };
            DatosCasilleroElectronico datosCasilleroElectronico = new DatosCasilleroElectronico();

            try
            {
                result = ResultRequestAPICasillero<CasilleroElectronicoResponse>(url, "GET", null, model.No_CasilleroElectronico);

                casillero = JsonConvert.DeserializeObject<CasilleroElectronicoResponse>(result);

                List<CasilleroElectronicoResponse_data> data = JsonConvert.DeserializeObject<List<CasilleroElectronicoResponse_data>>(JsonConvert.SerializeObject(casillero.data));

                foreach (var item in data)
                {
                    //Si ya encontró que se encuentra vinculado al casillero electrónico, ya no seguirá consultando el foreach
                    if (respuesta)
                    {
                        return respuesta;
                    }
                    else
                    {
                        datosCasilleroElectronico = new DatosCasilleroElectronico
                        {
                            No_CasilleroElectronico = item.userLockerId.ToString(),
                            No_Documento = item.tcUser.documentNumber.ToString(),
                            Nombres = item.tcUser.fullname,
                            Apellidos = item.tcUser.fullname,
                            Fecha_Nacimiento = item.tcUser.birthday,
                            No_NIT = item.tcUser.nit,
                            Telefono_Celular = item.tcUser.phone.ToString(),
                        };

                        //El número de casillero debe ser el que le aparece en el sistema de casillero electrónico
                        //El documento de identificación del usuario debe aparecer vinculado al casillero
                        //Esa es la forma en que se certifica que una persona tiene ese casillero electrónico vinculado

                        if(model.No_CasilleroElectronico == "2")
                        {
                            respuesta = true;
                        }

                        if ((datosCasilleroElectronico.No_Documento != model.No_Documento))
                        {
                            casillero.status = 2;
                            casillero.message = "El número de casillero electrónico o documento indicado no coincide";
                            respuesta = false;
                        }
                        else
                        {
                            respuesta = true;
                        }
                    }
                }



            }
            catch (TimeoutException e)
            {
                respuesta = false;
                casillero = new CasilleroElectronicoResponse
                {
                    status = 3,
                    message = "Servidor sin respuesta, " + e.Message,
                };
            }
            catch (Exception e)
            {
                respuesta = false;
                casillero = new CasilleroElectronicoResponse
                {
                    status = 4,
                    message = "Ocurrió un error, " + e.Message,
                };
            }


            return respuesta;
        }

        public CasilleroElectronicoResponse Execute_CasilleroElectronicoGlobal<T>(string url, string method, T objectRequest)
        {
            CasilleroElectronicoResponse casilleroElectronicoResponse = new CasilleroElectronicoResponse
            {
                status = -10,
                message = "No se ha ejecutado"
            };

            string result = "";

            try
            {
                result = ResultRequestAPICasillero<T>(url, method, objectRequest);

                casilleroElectronicoResponse = JsonConvert.DeserializeObject<CasilleroElectronicoResponse>(result);


            }
            catch (TimeoutException e)
            {
                casilleroElectronicoResponse = new CasilleroElectronicoResponse
                {
                    status = 3,
                    message = "Servidor sin respuesta, " + e.Message,
                };
            }
            catch (Exception e)
            {
                casilleroElectronicoResponse = new CasilleroElectronicoResponse
                {
                    status = 4,
                    message = "Ocurrió un error, " + e.Message,
                };
            }


            return casilleroElectronicoResponse;
        }

        public string ObtieneArchivoFirmado<T>(string url, string method, T objectRequest, string codigo)
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
            string result = "";

            if (codigo.ToLower().EndsWith(".pdf"))
            {
                codigo = codigo.Replace(Path.GetExtension(codigo),"");

            }

            string parametros = codigo;
            try
            {
                result = Convert.ToBase64String(ResultRequestFile<T>(url, method, objectRequest, parametros));
            }
            catch (TimeoutException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            return result;
        }

        #endregion

        #region Firmar Archivos (Nuevo)
        public FirmarArchivos_Response Execute_FirmarArchivos(FirmarArchivos_Request objectRequest, FirmarArchivos_Documentos_Request newParams = null)
        {
            FirmarArchivos_Response firmarArchivos_Response = new FirmarArchivos_Response();
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
            string result = "";
            foreach (var item in objectRequest.Documentos)
            {

                //Coordenadas default en caso de que no se hayan asignado
                //Hay que tener en cuenta que los documentos se están generando en su mayoría tamaño carta, lo que significa que la coordenada máxima es de 612,792, de allí es que se tomó en cuenta las coordenadas default
                //Para esto es recomendable que el documento generado previamente, tenga el margen sigiente:
                //  -> doc.SetMargins(1f, 1f, 25f, 50f);
                //           Los primeros 3 márgenes corresponden a Izquierda, Derecha y Arriba suscesivamente
                //           La importante a tener en cuenta es el 4to margen que corresponde abajo, la altura ideal para las coordenadas default es de 50 para que no afecte el cuadro blanco que genera la firma electrónica
                if ((item.Coordenadas ?? "") == "")
                {
                    item.Coordenadas = "200,20,400,60";
                }

                if (newParams != null)
                {
                    if ((newParams.Coordenadas ?? "").Trim() != "")
                    {
                        item.Coordenadas = newParams.Coordenadas;
                    }
                    if (newParams.NumeroPagina != 0)
                    {
                        item.NumeroPagina = newParams.NumeroPagina;
                    }
                }


                ////Actualmente genera inconveniente en el API de firma electrónica si está en 0, por lo cual si actualmente viene declarado como 0, se cambiará a la 1er página la firma
                //if (item.NumeroPagina == 0)
                //{
                //    item.NumeroPagina = 1;
                //}
            }


            firmarArchivos_Response.Result = 0;
            firmarArchivos_Response.Mensaje = RegistroNO;
            try
            {
                var client = new RestClient(Constants.Address_FirmaElectronica);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", GetBearer());
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(objectRequest), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                FirmarArchivos_Response_Error firmarArchivos_Response_Error = new FirmarArchivos_Response_Error();

                if (response.Content.ToString().IndexOf("Error") > 0)
                {
                    firmarArchivos_Response_Error = new FirmarArchivos_Response_Error
                    {
                        Message = response.Content.ToString() + "  Usuario ó Password erroneo en firma."
                    };
                    firmarArchivos_Response.Result = 2;
                }
                if (response.Content.ToString().IndexOf("connection") > 0)
                {
                    firmarArchivos_Response_Error = new FirmarArchivos_Response_Error
                    {
                        Message = response.Content.ToString() + "  No hay conexion con el servidor de firmas." + response.Content.ToString()
                    };
                    firmarArchivos_Response.Result = 2;
                }

                if (firmarArchivos_Response.Result == 2)
                {
                    firmarArchivos_Response.Respuesta = firmarArchivos_Response_Error;
                }
                else
                {
                    try
                    {
                        FirmarArchivos_Response_OK firmarArchivos_Response_OK = JsonConvert.DeserializeObject<FirmarArchivos_Response_OK>(response.Content.ToString());
                        firmarArchivos_Response.Respuesta = firmarArchivos_Response_OK;
                        firmarArchivos_Response.Result = 1;
                        firmarArchivos_Response.Estado = "Archivos firmados";
                        firmarArchivos_Response.Mensaje = "Archivos firmados";
                    }
                    catch (Exception ex)
                    {
                    }

                }

            }
            catch (TimeoutException e)
            {
                firmarArchivos_Response.Estado = "Servidor sin respuesta, " + e.Message;
                firmarArchivos_Response.Result = 3;
            }
            catch (Exception e)
            {
                firmarArchivos_Response.Estado = "Ocurrió un error, " + e.Message;
                firmarArchivos_Response.Result = 4;
                firmarArchivos_Response.Respuesta = JsonConvert.DeserializeObject<FirmarArchivos_Response_Error>(result, jsonSettings);
            }
            return firmarArchivos_Response;
        }


        public string firmarFile(string strUsuarioFirma, string strUsuarioPassword, string strDocumento, FirmarArchivos_Documentos_Request newParams = null)
        {

            string parentGoogleDriveId = Constants.parentGoogleDriveId;

            RequestUtil requestUtil = new RequestUtil();

            try
            {


                //Lista de archivos que se agregarán al proceso de firmado electrónico
                List<FirmarArchivos_Documentos_Request> firmarArchivos_Documentos_Requests = new List<FirmarArchivos_Documentos_Request>
                {
                    new FirmarArchivos_Documentos_Request
                    {
                        GoogleDriveId = strDocumento,
                    }
                };

                //Credenciales y documentos a anexar al proveso de firmado electrónico
                FirmarArchivos_Request firmarArchivos_Request = new FirmarArchivos_Request
                {
                    User = strUsuarioFirma,
                    Password = strUsuarioPassword,
                    parentGoogleDriveId = parentGoogleDriveId,
                    Documentos = firmarArchivos_Documentos_Requests,
                };

                //Proceso encargado de la firma, se centraliza en RequestUtil
                FirmarArchivos_Response responseUtil = requestUtil.Execute_FirmarArchivos(firmarArchivos_Request, newParams);




                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica

                if (responseUtil.Result == 1)
                {
                    try
                    {
                        FirmarArchivos_Response_OK firmarArchivos_Response_OK = JsonConvert.DeserializeObject<FirmarArchivos_Response_OK>(JsonConvert.SerializeObject(responseUtil.Respuesta));
                        return firmarArchivos_Response_OK.Data.FirstOrDefault().GoogleDriveIdNuevo;
                    }
                    catch (Exception ex)
                    {
                        string respuestarecibida = responseUtil.Respuesta.ToString() ?? "";
                        return "Ha ocurrido un error durante la firma electrónca... \n Respuesta recibida:" + respuestarecibida + "\n ErrorMessage: " + ex.Message;
                    }

                }
                else
                {
                    try
                    {
                        FirmarArchivos_Response_Error firmarArchivos_Response_Error = JsonConvert.DeserializeObject<FirmarArchivos_Response_Error>(JsonConvert.SerializeObject(responseUtil.Respuesta));
                        return firmarArchivos_Response_Error.Message;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }



                // Descomentar return myDeserializedClass.Data[0].GoogleDriveIdNuevo;

                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica
                //  Temporal por fallo en firma electronica


            }
            catch (Exception ex)
            {
                return "Error al intentar firmar el archivo." + ex.Message.ToString();
            }

        }


        #endregion


        #region Probosque
        public Api_Probosque_Get Execute_Probosque_Get<T>(string url, string method, T objectRequest, string codigo = null)
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
            string result = "";
            string parametros = "";
            if (codigo != null)
            {
                parametros = $"?codigo={codigo}";
            }

            Reply_Probosque_Get.Result = 0;
            Reply_Probosque_Get.Mensaje = RegistroNO;
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                result = ResultRequestAPI<T>(url, method, objectRequest, parametros);
                Reply_Probosque_Get = JsonConvert.DeserializeObject<Api_Probosque_Get>(result, jsonSettings);
                if (Reply_Probosque_Get.Data.proyecto != null)
                {
                    Reply_Probosque_Get.Result = 1;
                    Reply_Probosque_Get.Mensaje = RegistroOK;
                }
                else
                {
                    Reply_Probosque_Get.Result = 2;
                }
            }
            catch (TimeoutException e)
            {
                Reply_Probosque_Get.Estado = "Servidor sin respuesta, " + e.Message;
                Reply_Probosque_Get.Result = 3;
            }
            catch (Exception e)
            {
                Reply_Probosque_Get.Estado = "Ocurrió un error, " + e.Message;
                Reply_Probosque_Get.Result = 4;
            }
            return Reply_Probosque_Get;
        }
        public Api_Probosque_Post Execute_Probosque_Post<T>(string url, string method, T objectRequest, string codigo = null, string rootpath = "")
        {
            //jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
            string result = "";
            string parametros = "";
            if (codigo != null)
            {
                parametros = $"?codigo={codigo}";
            }

            Reply_Probosque_Post.Result = 0;
            Reply_Probosque_Post.Mensaje = RegistroNO;
            try
            {

                result = ResultRequestAPI<T>(url, method, objectRequest, parametros);
                if ((result != null) & (result != ""))
                {
                    Reply_Probosque_Post = JsonConvert.DeserializeObject<Api_Probosque_Post>(result);
                    string expediente = Reply_Probosque_Post.Data.proyecto.Expediente;
                    Tbl_API_Probosque_Expediente tbl_API_Probosque_Expediente = (from d in db_Intermedia.Tbl_API_Probosque_Expediente
                                                                                 where d.Expediente == expediente
                                                                                 select d).FirstOrDefault();
                    Reply_Probosque_Post.Data.proyecto.FincaRegistroPropiedad.Fecha = (DateTime)Reply_Probosque_Post.Data.proyecto.FincaRegistroPropiedad.Fecha;

                    if (tbl_API_Probosque_Expediente == null)
                    {
                        if (Reply_Probosque_Post.Data.proyecto != null)
                        {
                            Reply_Probosque_Post.Result = 1;
                            Reply_Probosque_Post.Mensaje = RegistroOK;




                            Reply_Probosque_Post.ArchivosDescargados = new List<string>();
                            if (Reply_Probosque_Post.Data.proyecto.InformeTecnico.PDFInforme != null)
                            {
                                try
                                {

                                    string nombrearchivo = DescargarArchivosGenericos(Reply_Probosque_Post.Data.proyecto.InformeTecnico.PDFInforme, rootpath);
                                    if (nombrearchivo != null)
                                    {
                                        Reply_Probosque_Post.Data.proyecto.InformeTecnico.PDFInforme_Local = nombrearchivo;
                                        Reply_Probosque_Post.ArchivosDescargados.Add(nombrearchivo);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }

                            if (Reply_Probosque_Post.Data.proyecto.InformeTecnico.DocumentosPoligonos.Count() > 0)
                            {
                                int contador = Reply_Probosque_Post.Data.proyecto.InformeTecnico.DocumentosPoligonos.Count();

                                for (int i = 0; i < contador; i++)
                                {
                                    try
                                    {
                                        string nombrearchivo = DescargarArchivosGenericos(Reply_Probosque_Post.Data.proyecto.InformeTecnico.DocumentosPoligonos[i].PDFPol, rootpath);
                                        if (nombrearchivo != null)
                                        {
                                            Reply_Probosque_Post.Data.proyecto.InformeTecnico.DocumentosPoligonos[i].PDFPol_Local = nombrearchivo;
                                            Reply_Probosque_Post.ArchivosDescargados.Add(nombrearchivo);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }
                            }

                        }
                        else
                        {
                            Reply_Probosque_Post.Result = 2;
                        }
                    }
                    else
                    {
                        Reply_Probosque_Post.Result = 1;
                        Reply_Probosque_Post.Mensaje = RegistroOK;
                    }
                }
            }
            catch (TimeoutException e)
            {
                Reply_Probosque_Post.Estado = "Servidor sin respuesta, " + e.Message;
                Reply_Probosque_Post.Result = 3;
            }
            catch (Exception e)
            {
                Reply_Probosque_Post.Estado = "Ocurrió un error, " + e.Message;
                Reply_Probosque_Post.Result = 4;
            }
            return Reply_Probosque_Post;
        }
        #endregion


        #region PinpepOld
        public Api_PinpepOld_Get Execute_PinpepOld_Get<T>(string url, string method, T objectRequest, string codigo = null)
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!

            string result = "";

            string parametros = "";
            if (codigo != null)
            {
                parametros = $"?codigo={codigo}";
            }

            Reply_PinpepOld_Get.Result = 0;
            Reply_PinpepOld_Get.Mensaje = RegistroNO;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                result = ResultRequestAPI<T>(url, method, objectRequest, parametros);

                Reply_PinpepOld_Get = JsonConvert.DeserializeObject<Api_PinpepOld_Get>(result, jsonSettings);

                if (Reply_PinpepOld_Get.Data.proyecto != null)
                {
                    Reply_PinpepOld_Get.Result = 1;
                    Reply_PinpepOld_Get.Mensaje = RegistroOK;
                }
                else
                {
                    Reply_PinpepOld_Get.Result = 2;
                }
            }
            catch (TimeoutException e)
            {
                Reply_PinpepOld_Get.Estado = "Servidor sin respuesta, " + e.Message;
                Reply_PinpepOld_Get.Result = 3;
            }
            catch (Exception e)
            {
                Reply_PinpepOld_Get.Estado = "Ocurrió un error, " + e.Message;
                Reply_PinpepOld_Get.Result = 4;
            }
            return Reply_PinpepOld_Get;
        }
        public Api_PinpepOld_Post Execute_PinpepOld_Post<T>(string url, string method, T objectRequest, string codigo = null, string rootpath = "")
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!

            string result = "";

            string parametros = "";
            if (codigo != null)
            {
                parametros = $"?codigo={codigo}";
            }

            Reply_PinpepOld_Post.Result = 0;
            Reply_PinpepOld_Post.Mensaje = RegistroNO;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                result = ResultRequestAPI<T>(url, method, objectRequest, parametros);

                Reply_PinpepOld_Post = JsonConvert.DeserializeObject<Api_PinpepOld_Post>(result, jsonSettings);
                if (Reply_PinpepOld_Post.Data.proyecto != null)
                {
                    Reply_PinpepOld_Post.Result = 1;
                    Reply_PinpepOld_Post.Mensaje = RegistroOK;
                    Reply_PinpepOld_Post.ArchivosDescargados = new List<string>();
                    if (Reply_PinpepOld_Post.Data.proyecto.InformeTecnico.PDFInforme != null)
                    {
                        try
                        {
                            string nombrearchivo = DescargarArchivosGenericos(Reply_PinpepOld_Post.Data.proyecto.InformeTecnico.PDFInforme, rootpath);
                            if (nombrearchivo != null)
                            {
                                Reply_PinpepOld_Post.Data.proyecto.InformeTecnico.PDFInforme_Local = nombrearchivo;
                                Reply_PinpepOld_Post.ArchivosDescargados.Add(nombrearchivo);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }

                    if (Reply_PinpepOld_Post.Data.proyecto.Poligonos.Count() > 0)
                    {
                        int contador = Reply_PinpepOld_Post.Data.proyecto.Poligonos.Count();
                        for (int i = 0; i < contador; i++)
                        {
                            try
                            {
                                string nombrearchivo = DescargarArchivosGenericos(Reply_PinpepOld_Post.Data.proyecto.Poligonos[i].PDFPol, rootpath);
                                if (nombrearchivo != null)
                                {
                                    Reply_PinpepOld_Post.Data.proyecto.Poligonos[i].PDFPol_Local = nombrearchivo;
                                    Reply_PinpepOld_Post.ArchivosDescargados.Add(nombrearchivo);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }

                }
                else
                {
                    Reply_PinpepOld_Post.Result = 2;
                }
            }
            catch (TimeoutException e)
            {
                Reply_Probosque_Post.Estado = "Servidor sin respuesta, " + e.Message;
                Reply_PinpepOld_Post.Result = 3;
            }
            catch (Exception e)
            {
                Reply_Probosque_Post.Estado = "Ocurrió un error, " + e.Message;
                Reply_PinpepOld_Post.Result = 4;
            }
            return Reply_PinpepOld_Post;
        }
        #endregion



        #region PinpepNew
        #endregion

        #region Whatsapp Meta
        public Reply Execute_EnviarMensajeWhatsAppMeta<T>(string url, string method, T objectRequest, Tbl_Meta_ParametrosGenerales tbl_Meta_ParametrosGenerales)
        {
            Reply reply = new Reply
            {
                result = 0,
                message = "No se ha realizado ninguna acción"
            };
            jsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
            };
            string result = "";


            try
            {
                result = ResultRequestAPIMeta<T>(url, method, objectRequest, tbl_Meta_ParametrosGenerales);
                try
                {
                    EnviarWhatsApp_MetaOK_Response RespuestaOK = JsonConvert.DeserializeObject<EnviarWhatsApp_MetaOK_Response>(result, jsonSettings);
                    reply = new Reply
                    {
                        result = 1,
                        message = "Envío exitoso",
                        data = RespuestaOK,
                    };
                }
                catch (Exception ex)
                {
                    try
                    {
                        EnviarWhatsApp_MetaBAD_Response RespuestaBAD = JsonConvert.DeserializeObject<EnviarWhatsApp_MetaBAD_Response>(result, jsonSettings);
                        reply = new Reply
                        {
                            result = 2,
                            message = "Error al enviar",
                            data = RespuestaBAD,
                        };
                    }
                    catch (Exception e)
                    {
                        reply = new Reply
                        {
                            result = 3,
                            message = "Ocurrió un error",
                            data = result.ToString(),
                        };
                    }
                }


            }
            catch (Exception e)
            {
                reply = new Reply
                {
                    result = 3,
                    message = "Ocurrió un error, " + e.Message
                };
            }

            return reply;
        }
        #endregion

        #region SEINEF API
        public Reply Execute_Gestion_SEINEF<T>(string url, string method, T objectRequest, string parametros = "")
        {
            Reply reply = new Reply
            {
                result = 0,
                message = "No se ha realizado ninguna acción"
            };
            jsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
            };
            string result = "";


            try
            {

                result = ResultRequestAPI_Gestion_SEINEF<T>(url, method, objectRequest, parametros);
                reply = JsonConvert.DeserializeObject<Reply>(result, jsonSettings);
            }
            catch (Exception e)
            {
                reply = new Reply
                {
                    result = 3,
                    message = "Ocurrió un error, " + e.Message
                };
            }

            return reply;
        }
        #endregion



        //public class Rootobject
        //{
        //    public string Estado { get; set; }
        //    public Data Data { get; set; }
        //}

        //public class Data
        //{
        //    public Proyecto proyecto { get; set; }
        //}

        //public class Proyecto
        //{
        //    public int ProyectoId { get; set; }
        //    public string Expediente { get; set; }
        //    public string Modalidad { get; set; }
        //    public Propietarios Propietarios { get; set; }
        //    public Propietariosgrupal[] PropietariosGrupal { get; set; }
        //    public Representante Representante { get; set; }
        //    public string Region { get; set; }
        //    public string SubRegion { get; set; }
        //    public string FincaDepartamento { get; set; }
        //    public string FincaMunicipio { get; set; }
        //    public string FincaUbicacion { get; set; }
        //    public string FincaLugar { get; set; }
        //    public float FincaRefX { get; set; }
        //    public float FincaRefY { get; set; }
        //    public float AreaAprobada { get; set; }
        //    public float UltimaAreaCertificada { get; set; }
        //    public string UltimaFaseCertificada { get; set; }
        //    public Informetecnico InformeTecnico { get; set; }
        //    public Poligono[] Poligonos { get; set; }
        //}

        //public class Propietarios
        //{
        //    public string NoDPI { get; set; }
        //    public string Nit { get; set; }
        //    public string NombreCompleto { get; set; }
        //}

        //public class Representante
        //{
        //    public string NoDPI { get; set; }
        //    public string NombreCompleto { get; set; }
        //}

        //public class Informetecnico
        //{
        //    public string UltimaFaseCertificada { get; set; }
        //    public string NumeroInforme { get; set; }
        //    public string PDFInforme { get; set; }
        //    public Rodale[] Rodales { get; set; }
        //}

        //public class Rodale
        //{
        //    public int Id { get; set; }
        //    public float Area { get; set; }
        //    public string EspeciesProteger { get; set; }
        //    public Especiesforestale[] EspeciesForestales { get; set; }
        //}

        //public class Especiesforestale
        //{
        //    public string NombreEspecie { get; set; }
        //    public int ArbolesPorHa { get; set; }
        //    public float Area { get; set; }
        //}

        //public class Propietariosgrupal
        //{
        //    public string NombreCompleto { get; set; }
        //    public string NoDPI { get; set; }
        //}

        //public class Poligono
        //{
        //    public int Correlativo { get; set; }
        //    public Geometriagtm GeometriaGTM { get; set; }
        //    public object[] PoligonosDescuento { get; set; }
        //    public string PDFPol { get; set; }
        //}

        //public class Geometriagtm
        //{
        //    public Geometry Geometry { get; set; }
        //}

        //public class Geometry
        //{
        //    public string WellKnownText { get; set; }
        //}


    }
}
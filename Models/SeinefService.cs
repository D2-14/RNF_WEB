using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace RNF_Web.Models
{
    /// <summary>
    /// Servicio de integración con la API externa de SEINEF (Sistema de Empresas del INAB).
    /// Sincroniza los datos de una empresa registrada en RNF hacia SEINEF mediante REST/JSON.
    /// </summary>
    /// <remarks>
    /// Configuración requerida en Web.config (appSettings):
    /// <list type="bullet">
    ///   <item><term>Seinef_API_BaseUrl</term><description>URL base, p.ej. https://seinefcapacitacion.inab.gob.gt</description></item>
    ///   <item><term>Seinef_API_Usuario</term><description>Usuario para autenticación (sernafsys)</description></item>
    ///   <item><term>Seinef_API_Clave</term><description>Contraseña del usuario SEINEF</description></item>
    /// </list>
    /// Los logs de diagnóstico se emiten con el prefijo <c>[RNF-SEINEF]</c> hacia Debug Output.
    /// </remarks>
    public static class SeinefService
    {
        /// <summary>
        /// Envía los datos de una empresa a SEINEF. Realiza login (JWT) y luego POST al endpoint
        /// <c>/api/user/sernaf/add</c>. Se invoca desde <see cref="Gest_EtapaModel.ConfirmarRespuesta"/>
        /// cuando se autoriza la etapa 12 (<c>etapa_id == 12 &amp;&amp; respuesta == 1</c>).
        /// </summary>
        /// <param name="No_Registro">
        /// Número de registro RNF de la empresa, p.ej. <c>IF-4389</c>.
        /// Se usa para localizar <c>Tbl_RNF_Registro</c> y <c>Tbl_RNF_Empresa_Entidad</c>.
        /// </param>
        /// <param name="solicitud_id">
        /// ID de la solicitud asociada (opcional). Cuando se suministra, se usa para resolver
        /// el representante legal mediante la siguiente cadena de fallbacks:
        /// <list type="number">
        ///   <item><c>fc_Sol_Rodal_RepresentanteMandatario</c> (misma fuente que el PDF y el correo)</item>
        ///   <item><c>Tbl_Sol_PropietarioPersonaIndividual</c> (Estado_id = true)</item>
        ///   <item><c>Tbl_Sol_PropietarioPersonaJuridica</c> (Estado_id = true)</item>
        ///   <item>Usuario del casillero (<c>Tbl_Seg_UsuarioExterno</c> vía <c>swcreatedby</c>)</item>
        /// </list>
        /// Sin <paramref name="solicitud_id"/> usa <c>Tbl_RNF_RepresentanteLegal</c> (último registro).
        /// </param>
        /// <returns>
        /// <c>true</c> si SEINEF responde con HTTP 2xx; <c>false</c> en cualquier error
        /// (login fallido, datos no encontrados, excepción de red, etc.).
        /// </returns>
        /// <remarks>
        /// Nota: el error <c>FK_Users_Roles_RoleId</c> que devuelve SEINEF es un bug del backend
        /// externo; los datos de <c>Companies</c> SÍ se crean correctamente a pesar del error.
        /// <para>
        /// La función <c>fc_API_RNF_SEINEF</c> NO se usa porque referencia
        /// <c>db_RNF.dbo.Tbl_RNF_Registro</c> que no existe en este ambiente. Depto/municipio/
        /// dirección se obtienen directamente desde <c>Tbl_RNF_Empresa_Entidad</c> con Include.
        /// </para>
        /// </remarks>
        public static bool EnviarEmpresa(string No_Registro, long? solicitud_id = null)
        {
            System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Inicio EnviarEmpresa | No_Registro={No_Registro} | solicitud_id={solicitud_id} | {DateTime.Now:HH:mm:ss.fff}");
            try
            {
                using (db_RNFEntities db = new db_RNFEntities())
                {
                    Tbl_RNF_Registro registro = db.Tbl_RNF_Registro
                        .Include("Tbl_Gral_Region")
                        .Include("Tbl_RNF_Registro_Estado")
                        .Where(r => r.No_Registro == No_Registro)
                        .FirstOrDefault();

                    if (registro == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] ERROR: No se encontró registro para No_Registro={No_Registro}");
                        return false;
                    }
                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Registro encontrado | Estado_id={registro.Estado_id} | SubRegion_id={registro.SubRegion_id}");

                    Tbl_RNF_Empresa_Entidad empresa = db.Tbl_RNF_Empresa_Entidad
                        .Include("Tbl_Gral_Departamento")
                        .Include("Tbl_Gral_Municipio")
                        .Where(e => e.No_Registro == No_Registro)
                        .FirstOrDefault();

                    if (empresa == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] ERROR: No se encontró empresa para No_Registro={No_Registro}");
                        return false;
                    }
                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Empresa encontrada | Nombre={empresa.Nombre} | NIT={empresa.No_NIT}");

                    string romanoRegion = registro.Tbl_Gral_Region != null ? registro.Tbl_Gral_Region.No_Region : "";

                    string nombreEstado = registro.Tbl_RNF_Registro_Estado != null ? registro.Tbl_RNF_Registro_Estado.Descripcion : "";
                    int estadoActivo = (nombreEstado.ToLower() == "activo") ? 1 : 0;
                    int estadoInactivo = 1;

                    // Representante: usa fc_Sol_Rodal_RepresentanteMandatario (misma fuente que el correo)
                    // si hay solicitud_id; si no, cae al fallback de Tbl_RNF_RepresentanteLegal.
                    string representanteNombre = "";
                    string representanteCedula = "";
                    if (solicitud_id.HasValue)
                    {
                        // 1. Representante legal de la solicitud (misma fuente que el PDF y el correo)
                        fc_Sol_Rodal_RepresentanteMandatario_Result repFc = db
                            .fc_Sol_Rodal_RepresentanteMandatario(solicitud_id.Value, false)
                            .FirstOrDefault();
                        if (repFc != null)
                        {
                            representanteNombre = (repFc.Nombres + " " + repFc.Apellidos).Trim();
                            representanteCedula = repFc.RepresentanteNo_Documento ?? "";
                            System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante (fc_Sol_Rodal) | Nombre={representanteNombre} | Cedula={representanteCedula}");
                        }

                        // 2. Fallback: propietario persona individual
                        if (string.IsNullOrWhiteSpace(representanteNombre))
                        {
                            Tbl_Sol_PropietarioPersonaIndividual propInd = db.Tbl_Sol_PropietarioPersonaIndividual
                                .Where(p => p.Solicitud_id == solicitud_id.Value && p.Estado_id == true)
                                .FirstOrDefault();
                            if (propInd != null)
                            {
                                representanteNombre = (propInd.Nombres + " " + propInd.Apellidos).Trim();
                                representanteCedula = propInd.No_Documento ?? "";
                                System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante (PropietarioIndividual) | Nombre={representanteNombre} | Cedula={representanteCedula}");
                            }
                        }

                        // 3. Fallback: propietario persona jurídica
                        if (string.IsNullOrWhiteSpace(representanteNombre))
                        {
                            Tbl_Sol_PropietarioPersonaJuridica propJur = db.Tbl_Sol_PropietarioPersonaJuridica
                                .Where(p => p.Solicitud_id == solicitud_id.Value && p.Estado_id == true)
                                .FirstOrDefault();
                            if (propJur != null)
                            {
                                representanteNombre = propJur.Nombre ?? "";
                                System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante (PropietarioJuridico) | Nombre={representanteNombre}");
                            }
                        }

                        // 4. Fallback: usuario del casillero (mismo nombre que aparece en el correo de confirmación)
                        if (string.IsNullOrWhiteSpace(representanteNombre))
                        {
                            Tbl_Sol_Solicitud solCasillero = db.Tbl_Sol_Solicitud.Find(solicitud_id.Value);
                            if (solCasillero != null)
                            {
                                Tbl_Seg_UsuarioExterno usrCasillero = db.Tbl_Seg_UsuarioExterno
                                    .Where(u => u.Usuario_id == solCasillero.swcreatedby)
                                    .FirstOrDefault();
                                if (usrCasillero != null)
                                {
                                    representanteNombre = (usrCasillero.Nombres + " " + usrCasillero.Apellidos).Trim();
                                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante (UsuarioCasillero) | Nombre={representanteNombre}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Tbl_RNF_RepresentanteLegal representante = db.Tbl_RNF_RepresentanteLegal
                            .Where(r => r.No_Registro == No_Registro)
                            .OrderByDescending(r => r.RepresentanteLegal_id)
                            .FirstOrDefault();
                        representanteNombre = representante != null ? (representante.Nombres + " " + representante.Apellidos).Trim() : "";
                        representanteCedula = representante != null ? representante.RepresentanteNo_Documento ?? "" : "";
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante (Tbl_RNF_RepresentanteLegal) | Nombre={representanteNombre} | Cedula={representanteCedula}");
                    }

                    // Email: usar empresa.email si tiene valor; si no, usar el correo del usuario del casillero
                    string emailEmpresa = empresa.email ?? "";
                    if (string.IsNullOrWhiteSpace(emailEmpresa))
                    {
                        Tbl_Sol_Solicitud solicitud = solicitud_id.HasValue
                            ? db.Tbl_Sol_Solicitud.Find(solicitud_id.Value)
                            : db.Tbl_Sol_Solicitud.Where(s => s.No_Registro == No_Registro).OrderByDescending(s => s.Solicitud_id).FirstOrDefault();

                        if (solicitud != null)
                        {
                            Tbl_Seg_UsuarioExterno usuarioExterno = db.Tbl_Seg_UsuarioExterno
                                .Where(u => u.Usuario_id == solicitud.swcreatedby)
                                .FirstOrDefault();
                            emailEmpresa = usuarioExterno?.Correo ?? "";
                        }
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Email empresa vacío → usando correo del usuario del casillero: {emailEmpresa}");
                    }

                    // DireccionPlanta, DepartamentoEmpresa, MunicipioEmpresa desde la empresa directamente
                    string direccionPlanta = empresa.DireccionEmpresa ?? "";
                    string municipioEmpresa = empresa.Tbl_Gral_Municipio?.Municipio ?? "";
                    string deptoRaw = empresa.Tbl_Gral_Departamento?.Departamento ?? "";
                    var deptoNormalizado = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
                    {
                        { "Peten",          "Petén" },
                        { "Quiche",         "Quiché" },
                        { "Sacatepequez",   "Sacatepéquez" },
                        { "Solola",         "Sololá" },
                        { "Suchitepequez",  "Suchitepéquez" },
                        { "Totonicapan",    "Totonicapán" },
                    };
                    string departamentoEmpresa = deptoNormalizado.TryGetValue(deptoRaw, out string deptoCorr) ? deptoCorr : deptoRaw;

                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] DireccionPlanta={direccionPlanta} | Depto={departamentoEmpresa} | Municipio={municipioEmpresa}");
                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Representante={representanteNombre} | Region={romanoRegion} | Estado={nombreEstado} | Email={emailEmpresa}");

                    string seinefBaseUrl = System.Configuration.ConfigurationManager.AppSettings["Seinef_API_BaseUrl"] ?? "";
                    string seinefUsuario = System.Configuration.ConfigurationManager.AppSettings["Seinef_API_Usuario"] ?? "";
                    string seinefClave = System.Configuration.ConfigurationManager.AppSettings["Seinef_API_Clave"] ?? "";

                    System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] BaseUrl={seinefBaseUrl} | Usuario={seinefUsuario}");

                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);

                        var loginBody = new { username = seinefUsuario, password = seinefClave };
                        string loginJson = JsonConvert.SerializeObject(loginBody);
                        HttpContent loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] POST login → {seinefBaseUrl}/api/Auth/login");
                        HttpResponseMessage loginResponse = client
                            .PostAsync(seinefBaseUrl + "/api/Auth/login", loginContent)
                            .ConfigureAwait(false).GetAwaiter().GetResult();

                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Login response | StatusCode={(int)loginResponse.StatusCode} {loginResponse.StatusCode}");

                        if (!loginResponse.IsSuccessStatusCode)
                        {
                            string loginError = loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                            System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] ERROR login fallido | Body={loginError}");
                            return false;
                        }

                        string loginResult = loginResponse.Content.ReadAsStringAsync()
                            .ConfigureAwait(false).GetAwaiter().GetResult();

                        dynamic loginObj = JsonConvert.DeserializeObject(loginResult);
                        string token = loginObj?.token ?? loginObj?.access_token ?? "";

                        if (string.IsNullOrEmpty(token))
                        {
                            System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] ERROR: token vacío | LoginBody={loginResult}");
                            return false;
                        }
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Token obtenido OK | Token[..20]={token.Substring(0, Math.Min(20, token.Length))}...");

                        var seinefBody = new
                        {
                            SERNAF = true,
                            No_Registro = registro.No_Registro,
                            RegistroId = registro.Old_RegistroId_Identity ?? 0,
                            Industrias = registro.No_Registro,
                            IndustriasFisc = "'0'",
                            IndustriasFisc2 = "'0'",
                            ID_RNF_S = registro.SubRegion_id?.ToString() ?? "",
                            TipoRegistroId = registro.Old_TipoRegistroId?.ToString() ?? registro.Sub_Categoria_id?.ToString() ?? "",
                            Fecha1 = registro.Fecha_Inscripcion.HasValue ? registro.Fecha_Inscripcion.Value.ToString("yyyy-MM-dd") : "",
                            Fecha2 = registro.Fecha_Actualizacion.HasValue
                                ? registro.Fecha_Actualizacion.Value.ToString("yyyy-MM-dd")
                                : (registro.Fecha_De_Vencimiento.HasValue ? registro.Fecha_De_Vencimiento.Value.ToString("yyyy-MM-dd") : ""),
                            NombreEstado = nombreEstado,
                            EstadoActivo = estadoActivo,
                            EstadoInactivo = estadoInactivo,
                            No_NIT = empresa.No_NIT ?? "",
                            RomanoRegion = romanoRegion,
                            SqlQuery = "",
                            RepresentanteNombre = representanteNombre,
                            NombreComercial = empresa.Nombre ?? "",
                            EMail = emailEmpresa,
                            RepresentanteCedula = representanteCedula,
                            DireccionPlanta = direccionPlanta,
                            DepartamentoEmpresa = departamentoEmpresa,
                            MunicipioEmpresa = municipioEmpresa
                        };

                        string seinefJson = JsonConvert.SerializeObject(seinefBody);
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] POST empresa → {seinefBaseUrl}/api/user/sernaf/add");
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Payload={seinefJson}");

                        HttpContent seinefContent = new StringContent(seinefJson, Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);

                        HttpResponseMessage seinefResponse = client
                            .PostAsync(seinefBaseUrl + "/api/user/sernaf/add", seinefContent)
                            .ConfigureAwait(false).GetAwaiter().GetResult();

                        string seinefResponseBody = seinefResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Respuesta SEINEF | StatusCode={(int)seinefResponse.StatusCode} {seinefResponse.StatusCode} | Body={seinefResponseBody}");

                        bool exito = seinefResponse.IsSuccessStatusCode;
                        System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] Resultado final EnviarEmpresa={exito} | No_Registro={No_Registro}");
                        return exito;
                    }
                }
            }
            catch (Exception ex)
            {
                string innerMsg = ex.InnerException?.Message ?? "";
                string inner2Msg = ex.InnerException?.InnerException?.Message ?? "";
                System.Diagnostics.Debug.WriteLine($"[RNF-SEINEF] EXCEPCION en EnviarEmpresa | No_Registro={No_Registro} | {ex.GetType().Name}: {ex.Message} | Inner: {innerMsg} | Inner2: {inner2Msg}");
            }
            return false;
        }
    }
}

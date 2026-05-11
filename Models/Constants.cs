using System;
using System.Net.Mail;
using System.Configuration;
using System.Linq;

namespace RNF_Web.Models
{
    public class Constants
    {

        public static string varkey = ConfigurationManager.AppSettings["varkey"];
        public static string variv = ConfigurationManager.AppSettings["variv"];

        public const string session_User = "User";

        public const string session_Tabulador = "Tabulador";

        public const string session_Solicitud = "Solicitud";

        public static int VisualizarInformacionDesarrollo = Convert.ToInt32(ConfigurationManager.AppSettings["VisualizarInformacionDesarrollo"]);
        public static string Url_Reportes_RNF = ConfigurationManager.AppSettings["Url_Reportes_RNF"];
        //public static string IP_Pinpep_Probosque = ConfigurationManager.AppSettings["Direccion_IP_PinPepOld"];
        public static string IP_Pinpep_Probosque = ConfigurationManager.AppSettings["Direccion_IP_PinPepv2"];

        public static string IP_FirmaElectronica = ConfigurationManager.AppSettings["Direccion_IP_FirmaElectronica"];

        //public static long LimitadorArchivo_FirmaElectronica = long.Parse(ConfigurationManager.AppSettings["LimitadorArchivo_FirmaElectronica"]);

        public static int codigoPaisGuatemala = 1;

        public const int Etapa_EnCreacion = 0;
        public const int Etapa_EliminadaPorElUsuario = 6;
        public const int Etapa_Incompleta = 4;

        public const string session_CantidadItems = "CantidadItems";

        public const string session_Rodal = "Rodal";

        public const string session_Finca = "Finca";

        public const string session_SolicitudLista = "SolicitudLista";

        public const string session_Tbl_Sol_Solicitud = "Tbl_Sol_Solicitud";

        public const string session_Tbl_Sol_Motosierra = "Tbl_Sol_Motosierra";

        public const string session_Tbl_Sol_Empresa_Entidad = "Tbl_Sol_Empresa_Entidad";

        public const string session_Tbl_Sol_Empresa_Entidad_Tipo_Registro = "Tbl_Sol_Empresa_Entidad_Tipo_Registro";

        public const string session_Tbl_Sol_PersonaIndividual = "Tbl_Sol_PersonaIndividual";

        public const string session_Tbl_Sol_PersonaJuridica = "Tbl_Sol_PersonaJuridica";

        public const string session_Captcha = "Captcha";

        public const string session_EdicionSolicitudGrants = "SolicitudGrants";
        public const string session_EdicionRNFGrants = "RNFGrants";

        //-------------------------------------------------------

        public const string strEmpresa = "INAB";

        public const string Login_Logo = "/Content/images/logoInab_Vertical.png";

        public const string Login_Titulo = " Ingreso al sistema de Registro <br/>Nacional Forestal";

        public const string Layout_Titulo = "Sistema virtual INAB";

        public const string Menu_Logo = "/Content/assets/img/logo_01.png";

        public const string Entidad = "PROCESO: REGISTRO NACIONAL FORESTAL";

        public const string MensajeAlerta = "MensajeAlerta";

        public const string Titulo_Rodal_Linea = "Datos del polígono";

        public static string Direccion_IP_SEINEF = (ConfigurationManager.AppSettings["Direccion_IP_SEINEF"]);

        public const string strParentGoogleDrive = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";

        public static string Direccion_IP_FirmaElectronica = (ConfigurationManager.AppSettings["Direccion_IP_FirmaElectronica"]);
        public static string Address_Bearer = Direccion_IP_FirmaElectronica + "/Api_RNF/api/login/authenticate";
        public static string Address_GoogleDriveUpload = Direccion_IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive";
        public static string Address_FirmaElectronica = Direccion_IP_FirmaElectronica + "/Api_RNF/api/FirmaElectronica";
        public static string Address_GoogleDriveDownLoad = Direccion_IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive/";

        public static string Direccion_IP_CasilleroElectronico = (ConfigurationManager.AppSettings["Direccion_IP_CasilleroElectronico"]);
        public static string Address_BearerCasillero = Direccion_IP_CasilleroElectronico + "/user/token/guest";
        public static string Address_ConsultarCasillero = Direccion_IP_CasilleroElectronico + "/user/find/locker/";
        public static string Address_AgregarNotificacionCasillero = Direccion_IP_CasilleroElectronico + "/notification/add";
        public const int CasilleroElectronicoSistemaId = 4;

        public const string RNF_Username = "rnf";
        public const string RNF_Password = "NWJjEr9Q.3+w_rM=";

        public const string CasilleroElectronicoUser_In = "rnf";
        public const string CasilleroElectronicoUser_Out = "NWJjEr9Q.3+w_rM=";
        public const string CasilleroElectronicoPassword_In = "rnf";
        public const string CasilleroElectronicoPassword_Out = "NWJjEr9Q.3+w_rM=";

        public const string parentGoogleDriveId = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";


        //Sección Meta Whatsapp
        public static string UrlGraphFacebook = Obtener_UrlGraphFacebook();
        //Habilitar el envío de mensajes de Whatsapp a nivel global
        public static int EnviarWhatsApp = int.Parse(ConfigurationManager.AppSettings["EnviarWhatsApp"]);
        //Usar el método Meta o usar el Método anterior de envío de whatsapp
        public static int WhatsAppMeta = int.Parse(ConfigurationManager.AppSettings["WhatsAppMeta"]);


        public string initCapPalabras(string texto)
        {
            // mayuscula en cada palabra  HASTA LA VISTA ===> Hasta La Vista
            db_RNFEntities db = new db_RNFEntities();

            string Respuesta = db.Database.SqlQuery<string>("SELECT dbo.InitCap(@p0)", texto).FirstOrDefault();

            return Respuesta;

        }




        public static void FirmaElectronicaInsertarBitacoraDelete(string Guid_id, string Guidetapa_id, string UsuarioFE)
        {
            try
            {
                using (db_RNFEntities db = new db_RNFEntities())
                {


                    var BitacoraABorrar = from c in db.Tbl_Form_Formulario_FirmaElectronica_Bitacora
                                          where c.Guid_id == Guid_id
                                            && c.GuidEtapa_id == Guidetapa_id
                                            && c.Firmante == UsuarioFE
                                          select c;

                    foreach (var Bitacora in BitacoraABorrar)
                    {
                        db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.Remove(Bitacora);
                    }

                    db.SaveChanges();

                }
            }
            catch
            {

            }

        }



        public static void FirmaElectronicaInBitacoraDelResolucionesArchivo(string Guid_id, string Solicitud_Id, string UsuarioFE)
        {
            try
            {
                using (db_RNFEntities db = new db_RNFEntities())
                {


                    var BitacoraABorrar = from c in db.Tbl_Form_Formulario_FirmaElectronica_Bitacora
                                          where c.Guid_id == Guid_id
                                            && c.GuidEtapa_id == Solicitud_Id
                                            && c.Firmante == UsuarioFE
                                          select c;

                    foreach (var Bitacora in BitacoraABorrar)
                    {
                        db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.Remove(Bitacora);
                    }

                    db.SaveChanges();

                }
            }
            catch
            {

            }

        }


        private static string Obtener_UrlGraphFacebook()
        {
            try
            {
                using (db_WhatsappEntities db_wp = new db_WhatsappEntities())
                {
                    Tbl_Meta_ParametrosGenerales params_Meta = db_wp.Tbl_Meta_ParametrosGenerales.FirstOrDefault();
                    return $"https://graph.facebook.com/{params_Meta.WhatsappVersion}/{params_Meta.WhatsappID}/messages";
                }
            }
            catch
            {
                return "";
            }
        }

        public static void FirmaElectronicaInsertarBitacora(string Guid_id, string Guidetapa_id, string UsuarioFE, int Form_Formulario_id, int FirmaElectronica_Estado_id, string Observaciones)
        {

            try
            {

                using (db_RNFEntities db = new db_RNFEntities())
                {
                    Tbl_Form_Formulario_FirmaElectronica_Bitacora tbl_Form_Formulario_FirmaElectronica_Bitacora = new Tbl_Form_Formulario_FirmaElectronica_Bitacora();
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.Form_Formulario_id = Form_Formulario_id;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.Firmante = UsuarioFE;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.FirmaElectronica_Estado_id = FirmaElectronica_Estado_id;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.swdatecreated = DateTime.Now;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.Guid_id = Guid_id;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.GuidEtapa_id = Guidetapa_id;
                    tbl_Form_Formulario_FirmaElectronica_Bitacora.Resultado = Observaciones;
                    db.Tbl_Form_Formulario_FirmaElectronica_Bitacora.Add(tbl_Form_Formulario_FirmaElectronica_Bitacora);
                    db.SaveChanges();
                }

            }
            catch(Exception ex)
            {

            }

        }




        public static void InEtapaSolResolucionesArchivo(long Solicitud_Id, int Etapa_Id, decimal EtapaRuta_Id, int CorrelativoEtapa_id,int EtapaOrigen_Correlativo,long Usuario,string Solicitud_Gui_id,string NombreDocumentoNofirmado)
        {

            try
            {

                using (db_RNFEntities db = new db_RNFEntities())
                {
                    Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = new Tbl_Gest_EtapaSolicitud();

                    tbl_Gest_EtapaSolicitud.Solicitud_id = Solicitud_Id;
                    tbl_Gest_EtapaSolicitud.Etapa_id = Etapa_Id;
                    tbl_Gest_EtapaSolicitud.EtapaRuta_id = EtapaRuta_Id;
                    tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id = CorrelativoEtapa_id;
                    tbl_Gest_EtapaSolicitud.EtapaOrigen_Correlativo_id = EtapaOrigen_Correlativo;
                    tbl_Gest_EtapaSolicitud.Respuesta_id = 0;
                    tbl_Gest_EtapaSolicitud.EtapaSolicitudEstado_id = 1;
                    tbl_Gest_EtapaSolicitud.Escalamiento_A = false;
                    tbl_Gest_EtapaSolicitud.Escalamiento_B = false;
                    tbl_Gest_EtapaSolicitud.Escalamiento_C = false;
                    tbl_Gest_EtapaSolicitud.swcreatedby = Usuario;
                    tbl_Gest_EtapaSolicitud.swcreatedbyinterno = true;
                    tbl_Gest_EtapaSolicitud.swdatecreated = DateTime.Now;
                    tbl_Gest_EtapaSolicitud.swViewedby = Usuario;
                    tbl_Gest_EtapaSolicitud.swLastViewedbyinterno = true;
                    tbl_Gest_EtapaSolicitud.swdateViewed = DateTime.Now;
                    tbl_Gest_EtapaSolicitud.swLastViewedby = Usuario;
                    tbl_Gest_EtapaSolicitud.swLastViewedbyinterno = true;
                    tbl_Gest_EtapaSolicitud.swdateLastViewed = DateTime.Now;
                    tbl_Gest_EtapaSolicitud.Solicitud_Guid_id = Solicitud_Gui_id;
                    tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = NombreDocumentoNofirmado;
                    db.Tbl_Gest_EtapaSolicitud.Add(tbl_Gest_EtapaSolicitud);
                    db.SaveChanges();
                }

            }
            catch(Exception ex)
            {

            }

        }

        public string initCapTexto(string texto)
        {
            // mayuscula en cada palabra  HASTA LA VISTA ===> Hasta la vista

            db_RNFEntities db = new db_RNFEntities();

            string Respuesta = db.Database.SqlQuery<string>("SELECT dbo.InitCapTexto(@p0)", texto).FirstOrDefault();

            return Respuesta;


        }

        public string fechaTxtSinGuatemala(DateTime fecha)
        {
            db_RNFEntities db = new db_RNFEntities();
            if (fecha == null)
            {
                fecha = DateTime.Now;
            }

            string Respuesta = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxtSinGuatemala(Convert(datetime, '" + fecha + "', 103))").FirstOrDefault();

            return Respuesta;

        }

        public int ValidarSession(Usuario objUsuario)
        {
            if (objUsuario == null)
            {
                return 0;
            }
            return 1;
        }


        //public class ResultFromStoreProcedure
        //{
        //    public int respuesta { get; set; }

        //    public string mensaje { get; set; }

        //    public int id { get; set; }
        //}


        public bool No_NIT_Valido(string No_NIT)
        {
            if (No_NIT == "asdf")
            {
                return false;
            }
            return true;
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



        public static int MaximoRol(long intUsuario_id)
        {
            using (db_RNFEntities db = new db_RNFEntities())
            {
                int intSubRegionaRol = db.Database.SqlQuery<int>("select max(SubRegional) from Tbl_Gral_PerfilesRol").FirstOrDefault();
                int intDirectorRegionalRol = db.Database.SqlQuery<int>("select max(Regional) from Tbl_Gral_PerfilesRol").FirstOrDefault();
                int intAdministradorRol = db.Database.SqlQuery<int>("select max(AdministradorRNF) from Tbl_Gral_PerfilesRol").FirstOrDefault();
                int intTecnicoRol = db.Database.SqlQuery<int>("select max(TecnicoForestal) from Tbl_Gral_PerfilesRol").FirstOrDefault();
                int intSecretariaRol = db.Database.SqlQuery<int>("select max(Secretaria) from Tbl_Gral_PerfilesRol").FirstOrDefault();

                fc_Seg_Sel_RolUsuario_Result fc_RolSubRegional = db.fc_Seg_Sel_RolUsuario(intUsuario_id, 1).Where(Obj => Obj.Rol_id == intSubRegionaRol).FirstOrDefault();
                fc_Seg_Sel_RolUsuario_Result fc_RolDirectorRegional = db.fc_Seg_Sel_RolUsuario(intUsuario_id, 1).Where(Obj => Obj.Rol_id == intDirectorRegionalRol).FirstOrDefault();
                fc_Seg_Sel_RolUsuario_Result fc_RolAdminsitrador = db.fc_Seg_Sel_RolUsuario(intUsuario_id, 1).Where(Obj => Obj.Rol_id == intAdministradorRol).FirstOrDefault();
                fc_Seg_Sel_RolUsuario_Result fc_RolTecnicoForestal = db.fc_Seg_Sel_RolUsuario(intUsuario_id, 1).Where(Obj => Obj.Rol_id == intTecnicoRol).FirstOrDefault();
                fc_Seg_Sel_RolUsuario_Result fc_RolSecretaria = db.fc_Seg_Sel_RolUsuario(intUsuario_id, 1).Where(Obj => Obj.Rol_id == intSecretariaRol).FirstOrDefault();

                if (fc_RolAdminsitrador != null)
                {
                    return intAdministradorRol;
                }
                if (fc_RolDirectorRegional != null)
                {
                    return intDirectorRegionalRol;
                }
                if (fc_RolSubRegional != null)
                {
                    return intSubRegionaRol;
                }
                if (fc_RolTecnicoForestal != null)
                {
                    return intTecnicoRol;
                }
                return intSecretariaRol;
            }


        }

        public static string Gral_Telefono_LimpiarNumero(string Telefono, int LimpiarCodigoDeArea, string CodigoDeArea, int Longitud = 8)
        {
            try
            {
                using (db_RNFEntities db = new db_RNFEntities())
                {
                    return db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Telefono_LimpiarNumero('" + Telefono + "'," + LimpiarCodigoDeArea + ",'" + CodigoDeArea + "'," + Longitud + ")").FirstOrDefault();
                }
            }
            catch
            {
                return Telefono;
            }
        }

        public static void Gral_Bitacora(Tbl_Gral_Bitacora model)
        {
            try
            {
                using (db_RNFEntities db = new db_RNFEntities())
                {

                    long Id_Bitacora = 0;
                    try
                    {
                        Id_Bitacora = db.Tbl_Gral_Bitacora.Max(Obj => Obj.Id_Bitacora);
                    }
                    catch
                    {
                        Id_Bitacora = 0;
                    }
                    Id_Bitacora++;

                    model.Id_Bitacora = Id_Bitacora;

                    db.Tbl_Gral_Bitacora.Add(model);
                    db.SaveChanges();

                }
            }
            catch
            {

            }
        }

    }
}
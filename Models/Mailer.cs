using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RNF_Web.Models
{
    public class Mailer
    {

        db_RNFEntities db = new db_RNFEntities();

        public async Task<bool> EnviarCorreoPendiente(Usuario usuario)
        {
            bool buscarpendientes;
            if (usuario.EsInterno == 0)
            {
                buscarpendientes = BuscarPendientes(usuario);
                if(buscarpendientes)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            else
            {
                buscarpendientes = BuscarPendientes(usuario);
                if (buscarpendientes)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
        }
        public async Task<bool> EnvioCorreoNuevo(Tbl_Mail_History model)
        {

            model.Guid_id = Guid.NewGuid().ToString();
            model.De = "Sistema SERNAF";

            Tbl_Mail_History Tbl_mail_History = new Tbl_Mail_History();
            Tbl_mail_History.Guid_id = model.Guid_id;
            Tbl_mail_History.Publico = model.Publico;
            Tbl_mail_History.De = model.De;
            Tbl_mail_History.para = model.para;
            Tbl_mail_History.CC = model.CC;
            Tbl_mail_History.Cuerpo = model.Cuerpo;
            Tbl_mail_History.EtapaSolicitud_GUID_id = model.EtapaSolicitud_GUID_id;
            Tbl_mail_History.TipoDeCorreo = model.TipoDeCorreo;
            Tbl_mail_History.UsuarioExterno_id = model.UsuarioExterno_id;
            Tbl_mail_History.Usuario_id = model.Usuario_id;
            Tbl_mail_History.Subject = model.Subject;
            Tbl_mail_History.Estado_id = 0;
            try
            {
                bool agregardb = Agregar_DB(Tbl_mail_History);
                if (agregardb)
                {
                    bool enviar = Enviar(Tbl_mail_History);
                    if (enviar)
                    {
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        return await Task.FromResult(false);
                    }
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }
        private bool BuscarPendientes(Usuario usuario)
        {
            int contador = 0;
            bool respuesta;

            string sqlQuery = "SELECT * FROM fc_Mail_History_Enviar()";

            List<Tbl_Mail_History> oCorreosPendientes = (db.Database.SqlQuery<Tbl_Mail_History>(sqlQuery).ToList() ?? new List<Tbl_Mail_History>());

            if (oCorreosPendientes.Count() > 0)
            {
                for (int i = 0; i < oCorreosPendientes.Count(); i++)
                {
                    respuesta = Enviar(oCorreosPendientes[i]);
                    if (respuesta)
                    {
                        contador += 1;
                    }
                }
                if (contador > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private bool BuscarPendientes_Externo(Usuario usuario)
        {
            int contador = 0;
            bool respuesta;
 
            List<Tbl_Mail_History> oCorreosPendientes = new List<Tbl_Mail_History>();
            oCorreosPendientes = (from d in db.Tbl_Mail_History
                                  where d.UsuarioExterno_id == usuario.intUsuario_id
                                  && d.Estado_id == 0
                                  select d).ToList();

            if (oCorreosPendientes.Count() > 0)
            {
                for (int i = 0; i < oCorreosPendientes.Count(); i++)
                {
                    respuesta = Enviar(oCorreosPendientes[i]);
                    if (respuesta)
                    {
                        contador += 1;
                    }
                }
                if(contador > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        private bool BuscarPendientes_Interno(Usuario usuario)
        {
            int contador = 0;
            bool respuesta;
            Tbl_Seg_Usuario oUsuario = (from d in db.Tbl_Seg_Usuario
                                        where d.Usuario_id == usuario.intUsuario_id
                                        select d).FirstOrDefault();

            List<Tbl_Mail_History> oCorreosPendientes = new List<Tbl_Mail_History>();
            oCorreosPendientes = (from d in db.Tbl_Mail_History
                                  where d.Usuario_id == oUsuario.Usuario_id && d.Estado_id == 0
                                  select d).ToList();

            if (oCorreosPendientes.Count() > 0)
            {
                for (int i = 0; i < oCorreosPendientes.Count(); i++)
                {
                    respuesta = Enviar(oCorreosPendientes[i]);
                    if (respuesta)
                    {
                        contador += 1;
                    }
                }
                if (contador > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool EnvioCorreoPendiente(Tbl_Mail_History model)
        {
            Tbl_Mail_History correo_pendiente = (from d in db.Tbl_Mail_History
                                                 where d.Guid_id == model.Guid_id && d.Estado_id == 0
                                                 select d).FirstOrDefault();
            if(correo_pendiente != null)
            {
                Enviar(correo_pendiente);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Agregar_DB(Tbl_Mail_History model)
        {
            try
            {
                db.Tbl_Mail_History.Add(model);
                db.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }


        private bool Enviar(Tbl_Mail_History model)
        {
            try
            {

                string host = ConfigurationManager.AppSettings["Host"];
                int puerto = Convert.ToInt32(ConfigurationManager.AppSettings["Puerto"]);
                string cred_cuenta = ConfigurationManager.AppSettings["Cuenta"];
                string cred_clave = ConfigurationManager.AppSettings["Clave"];
                string mailfrom = ConfigurationManager.AppSettings["Cuenta"];
                string displayname = "INAB Administrador";

                string correopara = model.para;
                string correocc = model.CC;
                string correoasunto = model.Subject;
                string correocuerpo = (model.Cuerpo??"");


                using (MailMessage Correo = new MailMessage())
                {

                    Correo.From = new MailAddress(mailfrom, displayname);
                    Correo.To.Add(new MailAddress(correopara));
                    if (correocc != null) 
                    {
                        if (correocc.Trim() != "")
                        {
                            Correo.CC.Add(new MailAddress(correocc));
                        }
                    }
                    Correo.Subject = correoasunto;

                    AlternateView HTMLConImagenes = default(AlternateView);
                    HTMLConImagenes = AlternateView.CreateAlternateViewFromString(correocuerpo, null, "text/html");

                    //HTMLConImagenes.LinkedResources.Add(imagen);
                    Correo.AlternateViews.Add(HTMLConImagenes);
                    Correo.IsBodyHtml = true;
                    Correo.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient(host, puerto);
                    smtp.Credentials = new NetworkCredential(cred_cuenta, cred_clave);

                    try
                    {
                        smtp.Send(Correo);
                        model.Estado_id = 1;

                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        model.Estado_id = 3;

                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();


                        Constants.Gral_Bitacora(new Tbl_Gral_Bitacora
                        {
                            Descripcion = "Fallo al enviar correo... " + ex.Message,
                            swdatecreated = DateTime.Now,
                            Referencia = "Tbl_Mail_History: " + model.Guid_id
                        });


                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                model.Estado_id = 2;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                Constants.Gral_Bitacora(new Tbl_Gral_Bitacora
                {
                    Descripcion = "Fallo al enviar correo... " + ex.Message,
                    swdatecreated = DateTime.Now,
                    Referencia = "Tbl_Mail_History: " + model.Guid_id
                });

                return false;
            }

        }
    }
}
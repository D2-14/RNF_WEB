using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RNF_Web.Models
{

    public class Whatsapper
    {
        db_RNFEntities db = new db_RNFEntities();
        db_WhatsappEntities db_wp = new db_WhatsappEntities();
        RequestUtil requestUtil = new RequestUtil();

        public Task<bool> EnviarWhatsappPendiente()
        {
            int contador = 0;
            bool respuesta;
            if (Constants.EnviarWhatsApp == 1)
            {
                List<Tbl_Whatsapp_Mensajes> oWPPendientes = new List<Tbl_Whatsapp_Mensajes>();

                //Se verifica que el parámetro de WhatsAppMeta se encuentre en estado 1, para que de esta manera se trabaje según los parámetros de Meta, caso contrario, el sistema se apegará a los datos que sean necesarios
                if (Constants.WhatsAppMeta == 1)
                {
                    oWPPendientes = db_wp.Tbl_Whatsapp_Mensajes.SqlQuery("SELECT * FROM Fc_Meta_MensajesPendientesAutorizados()").ToList() ?? new List<Tbl_Whatsapp_Mensajes>();
                }
                else
                {
                    oWPPendientes = (from d in db_wp.Tbl_Whatsapp_Mensajes
                                     where d.estado == false
                                     select d).ToList();
                }

                if (oWPPendientes.Count() > 0)
                {
                    Tbl_Meta_ParametrosGenerales tbl_Meta_ParametrosGenerales = db_wp.Tbl_Meta_ParametrosGenerales.FirstOrDefault();

                    if (Constants.WhatsAppMeta == 1)
                    {
                        for (int i = 0; i < oWPPendientes.Count(); i++)
                        {
                            respuesta = Enviar_Meta(oWPPendientes[i], tbl_Meta_ParametrosGenerales);
                            if (respuesta)
                            {
                                contador += 1;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < oWPPendientes.Count(); i++)
                        {
                            respuesta = Enviar(oWPPendientes[i]);
                            if (respuesta)
                            {
                                contador += 1;
                            }
                        }
                    }
                    if (contador > 0)
                    {
                        return Task.FromResult(true);
                    }
                    else
                    {
                        return Task.FromResult(false);
                    }
                }

            }
            return Task.FromResult(false);
        }

        private bool Enviar(Tbl_Whatsapp_Mensajes model)
        {
            Tbl_Seg_Parametros tbl_Seg_Parametros = new Tbl_Seg_Parametros();
            tbl_Seg_Parametros = (from d in db_wp.Tbl_Seg_Parametros
                                  select d).FirstOrDefault();
            if (tbl_Seg_Parametros != null)
            {
                model.Mensaje = " -- INAB Registro Nacional Forestal informa -- " + model.Mensaje;

                string instance, token;
                instance = tbl_Seg_Parametros.InstanceId;
                token = tbl_Seg_Parametros.Token;
                string paramsundefined = $"token={token}&to=%2B{model.Destino}&body={model.Mensaje}&priority=1&referenceId=";
                string url = $"https://api.ultramsg.com/{instance}/messages/chat?{paramsundefined}";
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("undefined", paramsundefined, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                model.swdateupdated = DateTime.Now;
                model.estado = true;
                db_wp.Entry(model).State = EntityState.Modified;
                db_wp.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }


        }

        private bool Enviar_Meta(Tbl_Whatsapp_Mensajes model, Tbl_Meta_ParametrosGenerales tbl_Meta_ParametrosGenerales)
        {

            //Tbl_Meta_ValidarTelefono tbl_Meta_ValidarTelefono = (from d in db_wp.Tbl_Meta_ValidarTelefono
            //                                                     where d.Telefono_id == model.Destino && d.Verificado
            //                                                     select d).FirstOrDefault();

            //if(tbl_Meta_ValidarTelefono != null) { }
            EnviarWhatsApp_Meta_Request enviarWhatsApp_Request = new EnviarWhatsApp_Meta_Request
            {
                messaging_product = "whatsapp",
                type = "text",
                to = model.Destino,
                text = new EnviarWhatsApp_Meta_Request_Text
                {
                    body = model.Mensaje
                },
            };

            Reply reply = requestUtil.Execute_EnviarMensajeWhatsAppMeta<EnviarWhatsApp_Meta_Request>(Constants.UrlGraphFacebook, "POST", enviarWhatsApp_Request, tbl_Meta_ParametrosGenerales);

            if (reply.result == 1)
            {

                model.swdateupdated = DateTime.Now;
                model.estado = true;
                db_wp.Entry(model).State = EntityState.Modified;
                db_wp.SaveChanges();

                return true;
            }

            return false;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    #region Solicitud para Enviar Mensajes
    public class EnviarWhatsApp_Meta_Request
    {
        public string messaging_product { get; set; }
        public EnviarWhatsApp_Request_Context context { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public EnviarWhatsApp_Meta_Request_Text text { get; set; }
        public EnviarWhatsApp_Meta_Request_Template template { get; set; }
    }

    public class EnviarWhatsApp_Request_Context
    {
        public string message_id { get; set; }
    }

    public class EnviarWhatsApp_Meta_Request_Text
    {
        public bool preview_url = false;
        public string body { get; set; }
    }

    public class EnviarWhatsApp_Meta_Request_Template
    {
        public string name { get; set; }
        public EnviarWhatsApp_Meta_Request_Template_Language language { get; set; }
    }
    public class EnviarWhatsApp_Meta_Request_Template_Language
    {
        public string code { get; set; }
    }
    #endregion

    #region Respuesta OK tras enviar mensajes
    public class EnviarWhatsApp_MetaOK_Response
    {
        public string messaging_product { get; set; }
        public List<EnviarWhatsApp_MetaOK_Response_Contacts> contacts { get; set; }
        public List<EnviarWhatsApp_MetaOK_Response_Messages> messages { get; set; }
    }
    public class EnviarWhatsApp_MetaOK_Response_Contacts
    {
        public string input { get; set; }
        public string wa_id { get; set; }
    }
    public class EnviarWhatsApp_MetaOK_Response_Messages
    {
        public string id { get; set; }
    }
    #endregion

    #region Respuesta BAD tras enviar mensajes
    public class EnviarWhatsApp_MetaBAD_Response
    {
        public EnviarWhatsApp_MetaBAD_Response_Error error { get; set; }
    }
    public class EnviarWhatsApp_MetaBAD_Response_Error
    {
        public string message { get; set; }
        public string type { get; set; }
        public long code { get; set; }
        public EnviarWhatsApp_MetaBAD_Response_Error_ErrorData error_data { get; set; }
        public string fbtrace_id { get; set; }
    }
    public class EnviarWhatsApp_MetaBAD_Response_Error_ErrorData
    {
        public string messaging_product { get; set; }
        public string details { get; set; }
    }
    #endregion


}
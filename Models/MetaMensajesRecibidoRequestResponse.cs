using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    #region Encabezado de Mensaje Entrante
    public class MensajeRecibido_Meta_Response
    {
        public string Object { get; set; }
        public List<MensajeRecibido_Meta_Entry_Response> entry { get; set; }

    }

    public class MensajeRecibido_Meta_Entry_Response
    {
        public string id { get; set; }
        public MensajeRecibido_Meta_Entry_Change[] changes { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change
    {
        public MensajeRecibido_Meta_Entry_Change_Value value { get; set; }
        public string field { get; set; }
    }
    #endregion

    #region Detalle de Mensaje Entrante
    public class MensajeRecibido_Meta_Entry_Change_Value
    {
        public string messaging_product { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Metadata metadata { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Contact[] contacts { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Message[] messages { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Metadata
    {
        public string display_phone_number { get; set; }
        public string phone_number_id { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Contact
    {
        public Meta_Profile profile { get; set; }
        public string wa_id { get; set; }
    }

    public class Meta_Profile
    {
        public string name { get; set; }
    }
    #endregion

    #region Cuerpo y Tipo de Mensaje
    public class MensajeRecibido_Meta_Entry_Change_Value_Message
    {
        public string from { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Message_Text text { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Message_Location location { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Message_Image image { get; set; }
        public MensajeRecibido_Meta_Entry_Change_Value_Message_Contact[] contacts { get; set; }
        public string type { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Message_Text
    {
        public string body { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Message_Location
    {
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Message_Image
    {
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string id { get; set; }
        public string caption { get; set; }
    }

    public class MensajeRecibido_Meta_Entry_Change_Value_Message_Contact
    {
        public Meta_Name name { get; set; }
        public Meta_Phone[] phones { get; set; }
    }
    public class Meta_Name
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string formatted_name { get; set; }
    }

    public class Meta_Phone
    {
        public string phone { get; set; }
        public string wa_id { get; set; }
        public string type { get; set; }
    }
    #endregion



}
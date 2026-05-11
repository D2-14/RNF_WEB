using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class Gest_EtapaSolicitudHistorial
    {
        public string Solicitud_NumeroExpediente { get; set; }
        public string Solicitud_NumeroTemporal { get; set; }
        public Nullable<DateTime> swdateupdated { get; set; }
        public Nullable<DateTime> swdatecreated { get; set; }
        public string Nombre_Etapa { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public string Nombres { get; set; }
        public string UsuarioAsignado { get; set; }
        public string EtapaSolicitudEstado { get; set; }
        public string Respuesta { get; set; }
        public string RespuestaJuridica { get; set; }
        public long Solicitud_id { get; set; }
        public string EtapaSolicitud_GUID_id { get; set; }
        public string Solicitud_Guid_id { get; set; }
        public int Etapa_id { get; set; }
        public decimal EtapaRuta_id { get; set; }
        public int CorrelativoEtapa_id { get; set; }
        public int EtapaSolicitudEstado_id { get; set; }
        public string NombreDocumentoFirmado { get; set; }
        public string NombreDocumentoNoFirmado { get; set; }
        public string Documentos { get; set; }
        public int Firmado { get; set; }
        public int DocumentosEtapa { get; set; }
    }
}
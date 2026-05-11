using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class FirmarArchivos_Request{
        public string User { get; set; }
        public string Password { get; set; }
        public string parentGoogleDriveId { get; set; }
        public List<FirmarArchivos_Documentos_Request> Documentos { get; set; }
    }

    public class FirmarArchivos_Documentos_Request
    {
        public string GoogleDriveId { get; set; }
        public string Coordenadas { get; set; }
        public int NumeroPagina { get; set; }
    }

    public class FirmarArchivos_Response
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public object Respuesta { get; set; }
    }


    public class FirmarArchivos_Response_Error
    {
        public string Message { get; set; }
    }
    public class FirmarArchivos_Response_OK
    {
        public string Estado { get; set; }
        public List<FirmarArchivos_Response_Ok_Data> Data { get; set; }
    }
    
    public class FirmarArchivos_Response_Ok_Data
    {
        public string GoogleDriveIdAnterior { get; set; }
        public string GoogleDriveIdNuevo { get; set; }
    }


}
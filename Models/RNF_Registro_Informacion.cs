using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class RNF_Registro_Informacion
    {
        public string No_Registro { get; set; }
        public string No_RegistroLiteral { get; set; }
        public int No_RegistroCorrelativo { get; set; }
        public string Expediente { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string No_Region { get; set; }
        public string No_SubRegion { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public Nullable<DateTime> Fecha_De_Vencimiento { get; set; }
        public Nullable<long> UsuarioExterno_id { get; set; }
        public long Solicitud_id { get; set; }
        public string GuidSolicitud_id { get; set; }
        public string DPI_Titular { get; set; }
        public string Titular { get; set; }
        public string InscripcionTipo { get; set; }
        public int Estado_id { get; set; }
        public string Estado { get; set; }
        public bool MismaRegion { get; set; }
        public int  Categoria_id { get; set; }
    }
}
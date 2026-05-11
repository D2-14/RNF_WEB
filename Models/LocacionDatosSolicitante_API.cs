using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class LocacionDatosSolicitante_API
    {
        public string Auditoria_id { get; set; }
        public string Guid_id { get; set; }
        public long Solicitud_id { get; set; }
        public long Finca_id { get; set; }
        public long Rodal_id { get; set; }
        public string FechaDia { get; set; }
        public string Nombre { get; set; }
        public string Registro { get; set; }
        public string Direccion { get; set; }
        public string Fincas { get; set; }
        public decimal AreaEfectiva { get; set; }
        public decimal LongitudEfectiva { get; set; }
    }
}
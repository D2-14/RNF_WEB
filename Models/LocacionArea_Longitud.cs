using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class LocacionArea_Longitud
    {
        public long Finca_id { get; set; }
        public long Rodal_id { get; set; }
        public long Rodal_Descuento_Id { get; set; }
        public string Tipo_de_AreaDescripcion { get; set; }
        public decimal AreaLUsuario { get; set; }
        public decimal AreaLTecnico { get; set; }
        public string Metrica { get; set; }

    }
}
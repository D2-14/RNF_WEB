using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class App_PromedioDAP_Altura
    {
        public int Cantidad { get; set; }
        public long Finca_id { get; set; }
        public long Rodal_id { get; set; }
        public string Tipo_de_Area { get; set; }
        public int Tipo_de_Area_id { get; set; }
        public int No_Parcela { get; set; }
        public string Especie_Indicada { get; set; }
        public decimal DAP_Promedio { get; set; }
        public decimal DAP_Promedio_Medido { get; set; }
        public decimal Promedio_Altura { get; set; }
        public decimal Promedio_Altura_Medido { get; set; }
        public decimal Diferencia_DAP { get; set; }
        public decimal Diferencia_Altura { get; set; }
        public string Observacion { get; set; }
    }
}
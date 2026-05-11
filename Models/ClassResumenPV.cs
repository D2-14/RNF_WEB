using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class ClassResumenPV
    {
        public long Solicitud_id { get; set; }
        public long Finca_id { get; set; }
        public string NombreFinca { get; set; }
        public long Rodal_id { get; set; }
        public int Tipo_de_Area { get; set; }
        public string Tipo_de_Area_Desc { get; set; }
        public decimal Longitud_Total { get; set; }
        public int Cantidad_Total_Arboles { get; set; }
        public string Especie { get; set; }
        public decimal Area_Efectiva_Rodal { get; set; }
        public int Anio_Establecimiento { get; set; }
        public string EstimacionPorMedioDe { get; set; }
        public decimal Cantidad_Arboles { get; set; }
        public decimal Densidad_ha { get; set; }
        public decimal AlturaPromedio { get; set; }
        public decimal DAPPromedio { get; set; }
        public decimal AreaBasal_ha { get; set; }
        public decimal Volumen_ha { get; set; }
        public decimal Volumen_Rodal { get; set; }
        public decimal Area_Basa_MetroCuadrado { get; set; }
        public decimal Volumen_X_Linea { get; set; }
        public decimal CoordenadaX { get; set; }
        public decimal CoordenadaY { get; set; }
        public int Clase { get; set; }
    }
    public class ClassResumenPV_RNF
    {
        public long Solicitud_id { get; set; }
        public long Finca_id { get; set; }
        public string NombreFinca { get; set; }
        public long Rodal_id { get; set; }
        public int Tipo_de_Area { get; set; }
        public string Tipo_de_Area_Desc { get; set; }
        public decimal Longitud_Total { get; set; }
        public int Cantidad_Total_Arboles { get; set; }
        public string Especie { get; set; }
        public decimal Area_Efectiva_Rodal { get; set; }
        public int Anio_Establecimiento { get; set; }
        public string EstimacionPorMedioDe { get; set; }
        public decimal Cantidad_Arboles { get; set; }
        public decimal Densidad_ha { get; set; }
        public decimal AlturaPromedio { get; set; }
        public decimal DAPPromedio { get; set; }
        public decimal AreaBasal_ha { get; set; }
        public decimal Volumen_ha { get; set; }
        public decimal Volumen_Rodal { get; set; }
        public decimal Area_Basa_MetroCuadrado { get; set; }
        public decimal Volumen_X_Linea { get; set; }
        public decimal CoordenadaX { get; set; }
        public decimal CoordenadaY { get; set; }
        public int Clase { get; set; }
    }
}
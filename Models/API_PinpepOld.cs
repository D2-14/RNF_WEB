using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using DotSpatial.Topology;
//using System.Windows.Shapes;

namespace RNF_Web.Models
{
    public class Api_PinpepOld_Get
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public Api_PinpepOld_Data_Get Data { get; set; }
    }
    public class Api_PinpepOld_Post
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public string Guid_id { get; set; }
        public bool Migrado { get; set; }
        public long Solicitud_id { get; set; }
        public Api_PinpepOld_Data_Post Data { get; set; }
        public List<string> ArchivosDescargados { get; set; }
    }


    public class Api_PinpepOld_Data_Get
    {
        public Api_PinpepOld_Data_Proyecto_Get proyecto { get; set; }
    }
    public class Api_PinpepOld_Data_Post
    {
        public Api_PinpepOld_Data_Proyecto_Post proyecto { get; set; }
    }

    public class Api_PinpepOld_Data_Proyecto_Get
    {
        public long ProyectoId { get; set; }
        public string Expediente { get; set; }
        public string Modalidad { get; set; }
        public string UltimaFaseCertificada { get; set; }
        public decimal UltimaAreaCertificada { get; set; }
        public Api_PinpepOld_Data_Proyecto_Propietarios Propietarios { get; set; }
        public string FincaLugar { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_Post
    {
        public long ProyectoId { get; set; }
        public string Expediente { get; set; }
        public string Modalidad { get; set; }
        public Api_PinpepOld_Data_Proyecto_Propietarios Propietarios { get; set; }

        public List<Api_PinpepOld_Data_Proyecto_PropietariosGrupal_Post> PropietariosGrupal { get; set; }
        public Api_PinpepOld_Data_Proyecto_Representante_Post Representante { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string FincaDepartamento { get; set; }
        public string FincaMunicipio { get; set; }
        public string FincaUbicacion { get; set; }
        public string FincaLugar { get; set; }
        public string FincaRefX { get; set; }
        public string FincaRefY { get; set; }
        public decimal AreaAprobada { get; set; }
        public decimal UltimaAreaCertificada { get; set; }
        public string UltimaFaseCertificada { get; set; }
        public Api_PinpepOld_Data_Proyecto_InformeTecnico_Post InformeTecnico { get; set; }
        public List<Api_PinpepOld_Data_Proyecto_Poligonos_Post> Poligonos { get; set; }
    }


    public class Api_PinpepOld_Data_Proyecto_Propietarios
    {
        public string NoDPI { get; set; }
        public string Nit { get; set; }
        public string NombreCompleto { get; set; }
    }



    public class Api_PinpepOld_Data_Proyecto_PropietariosGrupal_Post
    {
        public string NombreCompleto { get; set; }
        public string NoDPI { get; set; }
    }

    public class Api_PinpepOld_Data_Proyecto_Representante_Post
    {
        public string NoDPI { get; set; }
        public string NombreCompleto { get; set; }
    }


    public class Api_PinpepOld_Data_Proyecto_InformeTecnico_Post
    {
        public string UltimaFaseCertificada { get; set; }
        public string NumeroInforme { get; set; }
        public string PDFInforme { get; set; }
        public string PDFInforme_Local { get; set; }
        public List<Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_Post> Rodales { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_Post
    {
        public long Id { get; set; }
        public decimal Area { get; set; }
        public string EspeciesProteger { get; set; }
        public List<Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post> EspeciesForestales { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post
    {
        public string NombreEspecie { get; set; }
        public long? ArbolesPorHa { get; set; } = 0;
        public decimal? Area { get; set; } = 0;
    }


    public class Api_PinpepOld_Data_Proyecto_Poligonos_Post
    {
        public long Correlativo { get; set; }
        public Api_PinpepOld_Data_Proyecto_Poligonos_GeometriaGTM_Post GeometriaGTM { get; set; }
        public List<Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_Post> PoligonosDescuento { get; set; }
        public string PDFPol { get; set; }
        public string PDFPol_Local { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_Poligonos_GeometriaGTM_Post
    {
        public Api_PinpepOld_Data_Proyecto_Poligonos_GeometriaGTM_Geometry_Post Geometry { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_Poligonos_GeometriaGTM_Geometry_Post
    {
        public string WellKnownText { get; set; }
        //public Polygon WellKnownText { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_GeometriaGTM_Geometry_Post
    {
        public string WellKnownText { get; set; }
        //public Polygon WellKnownText { get; set; }
    }
    public class Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_Post
    {
        public Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_GeometriaGTM_Geometry_Post Geometry { get; set; }
    }









}
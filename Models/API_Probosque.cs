using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class Api_Probosque_Get
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public Api_Probosque_Data_Get Data { get; set; }
    }
    public class Api_Probosque_Post
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; }
        public string Guid_id { get; set; }
        public bool Migrado { get; set; }
        public long Solicitud_id { get; set; }
        public string TipoProyecto { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public Api_Probosque_Data_Post Data { get; set; }
        public List<string> ArchivosDescargados { get; set; }
    }



    public class Api_Probosque_Data_Get
    {
        public Api_Probosque_Data_Proyecto_Get proyecto { get; set; }
    }
    public class Api_Probosque_Data_Post
    {
        public Api_Probosque_Data_Proyecto_Post proyecto { get; set; }
    }



    public class Api_Probosque_Data_Proyecto_Get
    {
        public long ProyectoId { get; set; }
        public string Expediente { get; set; }
        public string Modalidad { get; set; }
        public string TipoProyecto { get; set; }
        public string UltimaFaseCertificada { get; set; }
        public string TipoPropietario { get; set; }
        public decimal UltimaAreaCertificada { get; set; }
        public List<Api_Probosque_Data_Proyecto_personasIndividuales_Get> personasIndividuales { get; set; }
        public List<Api_Probosque_Data_Proyecto_personasJuridicas_Get> personasJuridicas { get; set; }
        public string FincaNombre { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_Post
    {
        public long ProyectoId { get; set; }
        public string Expediente { get; set; }
        public string Modalidad { get; set; }
        public string TipoProyecto { get; set; }
        public string TipoPropietario { get; set; }
        public List<Api_Probosque_Data_Proyecto_personasIndividuales_Post> personasIndividuales { get; set; }
        public List<Api_Probosque_Data_Proyecto_personasJuridicas_Post> personasJuridicas { get; set; }
        public List<Api_Probosque_Data_Proyecto_Representante_Post> Representante { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string FincaNombre { get; set; }
        public string FincaMunicipio { get; set; }
        public string FincaDepartamento { get; set; }
        public string FincaUbicacion { get; set; }
        public decimal FincaRefX { get; set; }
        public decimal FincaRefY { get; set; }
        public int FincaTipoPropiedad { get; set; }
        public Api_Probosque_Data_Proyecto_FincaRegistroPropiedad_Post FincaRegistroPropiedad { get; set; }
        public string FincaActaMunicipal { get; set; }
        public string FincaArrendamiento { get; set; }
        public string FincaActaNotarial { get; set; }
        public decimal AreaFinca { get; set; }
        public decimal AreaAprobada { get; set; }
        public decimal UltimaAreaCertificada { get; set; }
        public string UltimaFaseCertificada { get; set; }
        public Api_Probosque_Data_Proyecto_InformeTecnico_Post InformeTecnico { get; set; }
    }



    public class Api_Probosque_Data_Proyecto_personasIndividuales_Get
    {
        public int EtniaId { get; set; }
        public int ComunidadLinguisticaId { get; set; }
        public int OcupacionId { get; set; }
        public int EstadoCivilId { get; set; }
        public string Nit { get; set; }
        public long Id { get; set; }
        public int MunicipioId { get; set; }
        public string CUI { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Direccion { get; set; }
        public string CorreoElectronico { get; set; }
        public string Observaciones { get; set; }
        public bool InformacionConfirmada { get; set; }
        public string Iniciales { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_personasJuridicas_Get
    {
        public int EtniaId { get; set; }
        public int ComunidadLinguisticaId { get; set; }
        public int OcupacionId { get; set; }
        public int EstadoCivilId { get; set; }
        public string Nit { get; set; }
        public long Id { get; set; }
        public int MunicipioId { get; set; }
        public string CUI { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Direccion { get; set; }
        public string CorreoElectronico { get; set; }
        public string Observaciones { get; set; }
        public bool InformacionConfirmada { get; set; }
        public string Iniciales { get; set; }
    }

    public class Api_Probosque_Data_Proyecto_personasIndividuales_Post
    {
        public string CUI { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_personasJuridicas_Post
    {
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public object TipoPersonaJ { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_Representante_Post
    {
        public string CUI { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_FincaRegistroPropiedad_Post
    {
        public long Id { get; set; }
        public string Numero { get; set; }
        public string Folio { get; set; }
        public string Libro { get; set; }
        public string Municipalidad { get; set; }
        public DateTime Fecha { get; set; }
        //public string Fecha_String { get; set; }
        public int TipoRegistroPropiedad { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Post
    {
        public string UltimaFaseCertificada { get; set; }
        public string NumeroInforme { get; set; }
        public string PDFInforme { get; set; }
        public string PDFInforme_Local { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_Post> Rodales { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Post>[] Poligonos { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_DocumentosPoligonos_Post> DocumentosPoligonos { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_Post
    {
        public long Id { get; set; }
        public decimal Area { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post> EspeciesForestales { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Post
    {
        public long RodalId { get; set; }
        public long PoligonoId { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Coordenadas_Post> Coordenadas { get; set; }
        public List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_PoligonosDescuento_Post>[] PoligonosDescuento { get; set; }
        public decimal AreaAprobada { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post
    {
        public string NombreCientifico { get; set; }
        public string Informe { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Informe_Post
    {
        public Nullable<DateTime> FechaPlantacion { get; set; }
        public int CicloDeCorta { get; set; }
        public Nullable<decimal> DensidadFinal { get; set; }
        public Nullable<decimal> DensidadInicial { get; set; }
        public Nullable<decimal> Mixtaje { get; set; }
        public Nullable<decimal> DistanciaES { get; set; }
        public Nullable<decimal> DistanciaEP { get; set; }
        public Nullable<decimal> DensidadActual { get; set; }
        public Nullable<decimal> Supervivencia { get; set; }
        public Nullable<decimal> PlantasSanas { get; set; }
        public Nullable<decimal> PlantacionDPA { get; set; }
        public Nullable<decimal> PlantacionAltura { get; set; }
        public Nullable<decimal> PlantasAfectadas { get; set; }
        public Nullable<decimal> PlantasEnfermedad { get; set; }
        public Nullable<decimal> PlantasFuego { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Coordenadas_Post
    {
        public long Id { get; set; }
        public long Orden { get; set; }
        public decimal GTMX { get; set; }
        public decimal GTMY { get; set; }

    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_PoligonosDescuento_Post
    {
        public long Id { get; set; }
        public long Orden { get; set; }
        public decimal GTMX { get; set; }
        public decimal GTMY { get; set; }
    }
    public class Api_Probosque_Data_Proyecto_InformeTecnico_DocumentosPoligonos_Post
    {
        public string PDFPol { get; set; }
        public string PDFPol_Local { get; set; }
    }



}
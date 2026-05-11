using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Finca
    {

        public class urlmetadata
        {
            [Display(Name = "Nombre de la Finca")]
            [Required(ErrorMessage = "Debe indicar el nombre de la finca.")]
            //[RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü.,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string NombreFinca { get; set; }

            [Display(Name = "Constancia de propiedad")]
            [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de documento que acredita la propiedad de la finca.")]
            public string ConstanciaDePorpiedad_id { get; set; }

            [Display(Name = "Describa la constancia")]
            public string ConstanciaPropiedadDescripcion { get; set; }

            [Display(Name = "Número de Finca")]
            public string RegNumero { get; set; }

            [Display(Name = "Folio de la Finca")]
            public string RegFolio { get; set; }

            [Display(Name = "Número de libro")]
            public string RegLibro { get; set; }

            [Display(Name = "Departamento de registro")]
            public string RegDepartamento_id { get; set; }

            [Display(Name = "Departamento de la finca")]
            public string DepartamentoFinca_Id { get; set; }

            [Display(Name = "Municipio de la finca")]
            [Required(ErrorMessage = "Debe indicar el municipio")]
            public string MunicipioFinca_Id { get; set; }

            [Display(Name = "Ubicación de la finca")]
            public string Ubicacion { get; set; }

            [Required(ErrorMessage = "Debe ingresar el area total de la finca según documentos")]
            [Display(Name = "Área total de la finca según documentos (ha)")]
            [DisplayFormat(DataFormatString = "{0:0.0000}")]
            [RegularExpression(@"^\d+(\.\d{1,4})?$", ErrorMessage = "Formato invalido en area total se espera valor con hasta cuatro decimales.")]
            [Range(0.01, int.MaxValue, ErrorMessage = "Indique el área total.")]
            public string AreaTotal { get; set; }

            [Required(ErrorMessage = "Debe ingresar el area registrada.")]
            [Display(Name = "Área a registrar (ha)")]
            [DisplayFormat(DataFormatString = "{0:0.00}")]
            [RegularExpression(@"^\d+(\.\d{1,3})?$", ErrorMessage = "Formato invalido en area registrada se espera valor con hasta tres decimales.")]
            [Range(0.01, int.MaxValue, ErrorMessage = "Indique el área registrada")]
            public string AreaARegistrar { get; set; }

            //[Required(ErrorMessage = "Debe ingresar las coordenadas GTMX.")]
            //[Display(Name = "Coordenada GTMX")]
            //[Range(312456, 754988, ErrorMessage = "Indique las coordenadas GTMX validas entre [312456, 754988]")]
            //[RegularExpression(@"^\d+(\.\d{1,2})?$")]
            //public string GTMX { get; set; }

            //[Required(ErrorMessage = "Debe ingresar las coordenadas GTMY.")]
            //[Display(Name = "Coordenada GTMY")]
            //[Range(1519248, 2046762, ErrorMessage = "Indique las coordenadas GTMY validas entre [1519248, 2046762]")]
            //[RegularExpression(@"^\d+(\.\d{1,2})?$")]
            //public string GTMY { get; set; }
        }
    }
}



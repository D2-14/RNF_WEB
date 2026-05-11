using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Empresa_Entidad
    {

        public class urlmetadata
        {
            [Display(Name = "Nombre")]
            [Required(ErrorMessage = "Debe indicar el nombre de la entidad.")]
            public string Nombre { get; set; }

            [Display(Name = "Número de NIT")]
            [Required(ErrorMessage = "El número de NIT es requerido.")] 
            public string No_NIT { get; set; }

            [Display(Name = "Dirección")]
//            [Required(ErrorMessage = "La dirección de la entidad debe ser especificada.")]
            public string DireccionEmpresa { get; set; }

            [Display(Name = "Tipo de Industria")]
            [Range(0, int.MaxValue, ErrorMessage = "Seleccione el tipo de industria")]
            public int Tipo_Industria_id { get; set; }

            [Required(ErrorMessage = "Debe ingresar las coordenadas GTMX.")]
            [Display(Name = "Coordenada GTMX")]
          // [Range(312456, 754988, ErrorMessage = "Indique las coordenadas GTMX validas entre [312456, 754988]")]
            [RegularExpression(@"^\d+(\.\d{1,4})?$")]
            public string GTMX { get; set; }

            [Required(ErrorMessage = "Debe ingresar las coordenadas GTMY.")]
          //  [Display(Name = "Coordenada GTMY")]
          //  [Range(1519248, 2046762, ErrorMessage = "Indique las coordenadas GTMY validas entre [1519248, 2046762]")]
            [RegularExpression(@"^\d+(\.\d{1,4})?$")]
            public string GTMY { get; set; }

            [StringLength(8, MinimumLength = 8, ErrorMessage = "La longitud esperada del teléfono es de 8 dígitos..")]
            [RegularExpression(@"\d*", ErrorMessage = "El teléfono solamente debe contener números")]
            public string Telefono { get; set; }

            [Required(ErrorMessage = "Debe ingresar la dirección de email_Cr.")]
            [EmailAddress(ErrorMessage = "Direccion de email invalida_Cr")]
            public string email { get; set; }

            [Display(Name = "Departamento")]
            [Required(ErrorMessage = "Debe indicar el departamento")]
            public int DepartamentoEmpresaEntidad_id { get; set; }

            [Display(Name = "Municipio")]
            [Required(ErrorMessage = "Debe indicar el municipio")] 
            public int MunicipioEmpresaEntidad_id { get; set; }

            [Display(Name = "Capacidad Instalada")]
            public decimal CapacidadInstalada { get; set; }

            [Display(Name = "No. de Registro de Empresa Forestal Vinculada")]
            public string RNF_Inscripcion_Vinculada { get; set; }

        }
    }
}
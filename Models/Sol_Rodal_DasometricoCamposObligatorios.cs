using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Rodal_Dasometrico
    {

        public class urlmetadata
        {

            [Display(Name = "Año de la plantación")]
            [Range(1500, 2050, ErrorMessage = "Indique el año de establecimiento entre 1500 y Año actual.")]
            public string Anio_Establecimiento { get; set; }

            [Required(ErrorMessage = "Debe ingresar la densidad(ha).")]
            [Display(Name = "Densidad(ha)")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique las densidad(ha).")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Densidad_ha { get; set; }

            [Required(ErrorMessage = "Debe ingresar la DAP Promedio.")]
            [Display(Name = "DAP Promedio(cm)")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique las DAP Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string DAP_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la altura promedio.")]
            [Display(Name = "Altura promedio(m)")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la altura Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Altura_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar el volumen.")]
            [Display(Name = "Volumen por área (metros³)")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique el volumen por área.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Volumen { get; set; }

            [Display(Name = "Estado Fitosanitario")]
            [Range(1, int.MaxValue, ErrorMessage = "Seleccione el estado fitosanitario.")]
            public string EstadoFitosanitario_id { get; set; }



            [Required(ErrorMessage = "Debe ingresar el DAP Promedio.")]
            [Display(Name = "DAP Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique el DAP Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_I_DAP_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Altura Promedio.")]
            [Display(Name = "Altura Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Altura Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_I_Altura_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Densidad Promedio.")]
            [Display(Name = "Densidad Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Densidad Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_I_Densidad_Promedio { get; set; }


            [Required(ErrorMessage = "Debe ingresar el DAP Promedio.")]
            [Display(Name = "DAP Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique el DAP Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_II_DAP_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Altura Promedio.")]
            [Display(Name = "Altura Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Altura Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_II_Altura_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Densidad Promedio.")]
            [Display(Name = "Densidad Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Densidad Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_II_Densidad_Promedio { get; set; }


            [Required(ErrorMessage = "Debe ingresar el DAP Promedio.")]
            [Display(Name = "DAP Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique el DAP Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_III_DAP_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Altura Promedio.")]
            [Display(Name = "Altura Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Altura Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_III_Altura_Promedio { get; set; }

            [Required(ErrorMessage = "Debe ingresar la Densidad Promedio.")]
            [Display(Name = "Densidad Promedio")]
            [Range(0, int.MaxValue, ErrorMessage = "Indique la Densidad Promedio.")]
            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo unicamente acepta dos decimales.")]
            public string Clase_III_Densidad_Promedio { get; set; }

            [Display(Name = "Especie")]
            public string Especie_Id { get; set; }

        }
    }
}

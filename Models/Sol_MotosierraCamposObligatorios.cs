using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Motosierra
    {

        public class urlmetadata
        {

            [Display(Name = "Marca")]
            [Required(ErrorMessage = "Debe indicar la marca de la motosierra.")]
            public string Marca { get; set; }

            [Display(Name = "Empresa emisora de la factura")]
            public string EmpesaEmisoraFactura { get; set; }

            [Display(Name = "Modelo")]
            [Required(ErrorMessage = "Debe indicar el modelo de la motosierra.")]
            public string Modelo { get; set; }

            [Display(Name = "Cilindraje")]
            [Required(ErrorMessage = "Debe indicar el Cilindraje de la motosierra.")]
            public string Cilindraje { get; set; }

            [Display(Name = "Potencia")]
            [Required(ErrorMessage = "Debe indicar la potencia de la motosierra.")]
            public string Potencia { get; set; }

            [Display(Name = "Número de serie")]
            [Required(ErrorMessage = "Debe indicar el número de serie, guiones si no lo tiene")]
            public string No_SerieMotosierra { get; set; }

            [Display(Name = "Número de lote")]
            public string No_Lote { get; set; }

        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;
//using System.Web.DynamicData;

//namespace RNF_Web.Models
//{
//    [MetadataType(typeof(urlmetadata))]
//    public partial class Tbl_Sol_Finca
//    {

//        public class urlmetadata
//        {
//            [Display(Name = "Nombre de la Finca")]
//            [Required(ErrorMessage = "Debe indicar el nombre de la finca.")]
//            [RegularExpression(@"^[a-zA-Zá-ú.,\s]+$", ErrorMessage = "No se aceptan números en el nombre de la finca o caracteres especiales.")]
//            public string NombreFinca { get; set; }

//            [Display(Name = "Constancia de propiedad")]
//            [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de documento que acredita la propiedad de la finca.")]
//            public string ConstanciaDePorpiedad_id { get; set; }

//            [Display(Name = "Describa la constancia")]
//            public string ConstanciaPropiedadDescripcion { get; set; }

//            [Display(Name = "Número de Finca")]
//            public string RegNumero { get; set; }

//            [Display(Name = "Folio de la Finca")]
//            public string RegFolio { get; set; }

//            [Display(Name = "Número de libro")]
//            public string RegLibro { get; set; }

//            [Display(Name = "Departamento de registro")]
//            public string RegDepartamento_id { get; set; }

//            [Display(Name = "Departamento de la finca")]
//            public string DepartamentoFinca_Id { get; set; }

//            [Display(Name = "Municipio de la finca")]
//            public string MunicipioFinca_Id { get; set; }

//            [Display(Name = "Ubicación de la finca")]
//            public string Ubicacion { get; set; }

//            [Required(ErrorMessage = "Debe ingresar el area total.")]
//            [Display(Name = "Area total")]
//            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato invalido en area total se espera valor con dos decimales.")]
//            [Range(1, int.MaxValue, ErrorMessage = "Indique el área total.")]
//            public string AreaTotal { get; set; }

//            [Required(ErrorMessage = "Debe ingresar el area registrada.")]
//            [Display(Name = "Area registrada")]
//            [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato invalido en area registrada se espera valor con dos decimales.")]
//            [Range(1, int.MaxValue, ErrorMessage = "Indique el área registrada")]
//            public string AreaRegistrada { get; set; }

//            [Required(ErrorMessage = "Debe ingresar las coordenadas GTMX.")]
//            [Display(Name = "Coordenada GTMX")]
//            [Range(1, int.MaxValue, ErrorMessage = "Indique las coordenadas GTMX")]
//            [RegularExpression(@"^\d+(\.\d{1,2})?$")]
//            public string GTMX { get; set; }


//            [Required(ErrorMessage = "Debe ingresar las coordenadas GTMY.")]
//            [Display(Name = "Coordenada GTMY")]
//            [Range(1, int.MaxValue, ErrorMessage = "Indique las coordenadas GTMY")]
//            [RegularExpression(@"^\d+(\.\d{1,2})?$")]
//            public string GTMY { get; set; }

//        }
//    }
//}
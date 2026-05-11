using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Gral_SubRegion
    {
        public string Nombre_SubRegionCompleto { get { return No_SubRegion + " " + Nombre_SubRegion; } }

        public class urlmetadata
        {

            [Display(Name = "Código de Sub Region")]
            [Required(ErrorMessage = "Indique el código de sub region.")]
            public string No_SubRegion { get; set; }

            [Display(Name = "Nombre de Sub Region")]
            [Required(ErrorMessage = "Indique el nombre de sub region.")]
            public string Nombre_SubRegion { get; set; }


           [Display(Name = "Codigo RNF_S")]
            [Required(ErrorMessage = "Indique el número de RNF_S.")]
            public int Id_RNF_S { get; set; }

            [StringLength(8, MinimumLength = 8, ErrorMessage = "La longitud esperada del teléfono es de 8 dígitos..")]
            [RegularExpression(@"\d*", ErrorMessage = "El telefono solamente debe contener números")]
            public string Telefono { get; set; }

            [Display(Name = "Dirección de la sub region")]
            [Required(ErrorMessage = "La dirección de la sub region.")]
            public string Direccion { get; set; }

       

        }
    }
}
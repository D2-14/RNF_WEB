using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_PropietarioPersonaJuridica
    {

        public class urlmetadata
        {

            [Display(Name = "Nombre(s) de de la entidad jurídica")]
            [Required(ErrorMessage = "Debe indicar el nombre(s) del propietario.")]
            public string Nombre { get; set; }

            [Display(Name = "No. de NIT")]
            [Required(ErrorMessage = "El número de NIT es requerido.")]
            public string No_NIT { get; set; }

            [Display(Name = "Dirección de la entidad")]
            [Required(ErrorMessage = "La dirección de la entidad debe ser especificada.")]
            public string DireccionEmpresa { get; set; }

            [Display(Name = "Tipo de personería")]
            [Required(ErrorMessage = "Indique la personería juridica.")]
            [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de personería jurídica")]
            public int PersonaJuridicaTipo_Id { get; set; }

        

        }

    }
}


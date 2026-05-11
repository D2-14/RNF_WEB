using System.ComponentModel.DataAnnotations;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Finca_Secorf_EspeciesForestales
    {
        public class urlmetadata
        {
            [Required(ErrorMessage = "Debe indicar el area.")]
            public string Area_ha { get; set; }

            [Required(ErrorMessage = "Debe indicar la especie.")]
            public string Especie_id { get; set; }

            [Required(ErrorMessage = "Debe indicar el DAP.")]
            public string DAP { get; set; }

            [Required(ErrorMessage = "Debe indicar la Altura.")]
            public string Altura { get; set; }

            [Required(ErrorMessage = "Debe indicar la Densidad ha.")]
            public string Densidad_ha { get; set; }

            [Required(ErrorMessage = "Debe indicar el Volumen.")]
            public string Volumen { get; set; }

            [Required(ErrorMessage = "Debe indicar Año de Establecimiento.")]
            public string Anio_Establecimiento { get; set; }
        }
    }
}
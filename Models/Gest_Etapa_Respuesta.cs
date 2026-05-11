using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Gest_Etapa_Respuesta
    {

        public class urlmetadata
        {
            [Display(Name = "Ruta Id.")]
            public int EtapaRuta_id { get; set; }

            [Display(Name = "Respuesta default")]
            public bool Respuesta_default { get; set; }

            [Display(Name = "Solicitud nuevo estado")]
            public bool Solicitud_Nuevo_Estado_id { get; set; }


        }
    }
}


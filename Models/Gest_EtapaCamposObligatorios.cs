using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{ 
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Gest_Etapa
    {

        public class urlmetadata
        {
            [Display(Name = "Area responsable")]
            public int Rol_id { get; set; }

            [Display(Name = "Nombre")]
            public string Nombre_Etapa { get; set; }

            [Display(Name = "Horas 1er Escalamiento")]
            public string EscalamientoA_Horas { get; set; }

            [Display(Name = "email 1er Escalamiento")]
            public string EscalamientoA_Email { get; set; }

            [Display(Name = "Telefono 1er Escalamiento")]
            public string EscalamientoA_Telefono { get; set; }

            [Display(Name = "Horas 2er Escalamiento")]
            public string EscalamientoB_Horas { get; set; }

            [Display(Name = "email 2er Escalamiento")]
            public string EscalamientoB_Email { get; set; }

            [Display(Name = "Telefono 2er Escalamiento")]
            public string EscalamientoB_Telefono { get; set; }


            [Display(Name = "Horas 3er Escalamiento")]
            public string EscalamientoC_Horas { get; set; }

            [Display(Name = "email 3er Escalamiento")]
            public string EscalamientoC_Email { get; set; }

            [Display(Name = "Telefono 3er Escalamiento")]
            public string EscalamientoC_Telefono { get; set; }


        }
    }
}


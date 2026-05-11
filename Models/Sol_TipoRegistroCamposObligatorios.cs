using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Empresa_Entidad_Tipo_Registro
    {
        public class urlmetadata
        {

            [Display(Name = "Tipo de Registro")]
            public int Tipo_Registro_Id { get; set; }

            [Display(Name = "Número de registro")]
            public string No_Partida { get; set; }

            [Display(Name = "Número de Folio")]
            public string No_Folio { get; set; }

            [Display(Name = "Número de Libro")]
            public string No_Libro { get; set; }

            [Display(Name = "Tipo REPEJU")]
            public int REPEJU_De_Id { get; set; }

            [Display(Name = "Número de Acta")]
            public string No_Acta { get; set; }

            [Display(Name = "Fecha de Acta")]
            public DateTime Fecha_Acta { get; set; }





        }
    }
}
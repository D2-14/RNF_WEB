using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{

    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_Solicitud
    {

        public class urlmetadata
        {

            [Display(Name = "Categoría")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique la categoría")]
            public int Categoria_id { get; set; }

            [Display(Name = "Sub Categoría")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique la Subcategoría")]
            public int Sub_Categoria_id { get; set; }


        }
    }

}


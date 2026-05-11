using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace RNF_Web.Models
{
    [MetadataType(typeof(Tbl_Gral_Region))]
    public partial class Tbl_Gral_Region
    {
        public string Nombre_RegionCompleto { get { return No_Region + " " + Nombre_Region; } }
    }
}
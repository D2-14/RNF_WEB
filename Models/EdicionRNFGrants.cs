using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class EdicionRNFGrants
    {
        public bool Agregar { get; set; }
        public bool Editar { get; set; }
        public bool Borrar { get; set; }
        public int Estado_id { get; set; }
        public string Estado_Registro { get; set; }
        public string Usuario { get; set; }
        public bool GenerarConstancia { get; set; }
    }
}
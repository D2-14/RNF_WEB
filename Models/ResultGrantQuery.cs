using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class ResultGrantQuery
    {
        public bool crear { get; set; }
        public bool consultar { get; set; }
        public bool anular { get; set; }
        public bool actualizar { get; set; }
        public bool imprimir { get; set; }
    }
}

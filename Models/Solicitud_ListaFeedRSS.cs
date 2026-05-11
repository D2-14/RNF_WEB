using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{

    public class Solicitud_ListaFeedRSSRequest
    {
        public int limit { get; set; }
    }

    public class Solicitud_ListaFeedRSS
    {
        public long Solicitud_id { get; set; }
        public string Guid_Solicitud { get; set; }
    }
}
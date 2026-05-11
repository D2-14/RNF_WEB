using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class Datum
    {
        public string GoogleDriveIdAnterior { get; set; }
        public string GoogleDriveIdNuevo { get; set; }
    }

    public class Rootobject
    {
        public string Estado { get; set; }
        public Datum[] Data { get; set; }
    }
}
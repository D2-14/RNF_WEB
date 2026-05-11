using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class Reply
    {
        public string message { get; set; }
        public int result { get; set; }
        public object data { get; set; }
        public object details { get; set; }
    }
}
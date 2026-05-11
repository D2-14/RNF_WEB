using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RNF_Web.Models;

namespace RNF_Web.Jobs
{
    public class JobCargaAreaProtegida : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            HttpContext.Current.Session[Constants.session_CantidadItems] = "Fecha "+ DateTime.Now.ToString();
        }
    }
}
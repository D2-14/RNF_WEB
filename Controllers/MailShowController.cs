using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class MailShowController : Controller
    {
        // GET: MailShow
        public ActionResult Index(string Guid_id)
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gest_EtapaRSSController : Controller
    {

        db_RNFEntities db = new db_RNFEntities();

        // GET: Gest_EtapaRSS
        public ActionResult Index()
        {
            return View(db.Tbl_gest_EtapaRSS);
        }

    }
}

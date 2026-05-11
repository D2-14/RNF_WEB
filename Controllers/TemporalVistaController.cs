using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class TemporalVistaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: TemporalVista
        public ActionResult Index()
        {

            return View(db.Tbl_Gral_SubRegion );
        }

    }
}

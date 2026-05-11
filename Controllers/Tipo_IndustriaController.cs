using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Tipo_IndustriaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Tipo_Industria
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Tipo_Industria.ToList());
        }

        public ActionResult Create()
        {
            Tbl_Gral_Tipo_Industria tbl_Gral_Tipo_Industria = new Tbl_Gral_Tipo_Industria();
            tbl_Gral_Tipo_Industria.swdatecreated = DateTime.Now;
            tbl_Gral_Tipo_Industria.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Tipo_Industria);
        }

        [HttpPost]
        public ActionResult Create(Tbl_Gral_Tipo_Industria tbl_Gral_Tipo_Industria)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Gral_Tipo_Industria.Max(u => u.Tipo_Industria_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Tipo_Industria.Tipo_Industria_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Tipo_Industria.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Tipo_Industria.Nombre_Tecnico).Max(u => u.Tipo_Industria_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de industria ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Tipo_Industria.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de industria vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Tipo_Industria.Add(tbl_Gral_Tipo_Industria);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Tipo_Industria);
        }

        public ActionResult Edit(int id)
        {
            Tbl_Gral_Tipo_Industria tbl_Gral_Tipo_Industria = db.Tbl_Gral_Tipo_Industria.Find(id);

            if (tbl_Gral_Tipo_Industria == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Tipo_Industria.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Tipo_Industria);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Tipo_Industria tbl_Gral_Tipo_Industria)
        {
            int Error = 0;

            try
            {
                if (tbl_Gral_Tipo_Industria.Tipo_Industria_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Tipo_Industria).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Tipo_Industria);
            }
            catch
            {
                return View(tbl_Gral_Tipo_Industria);
            }
        }

    }
}
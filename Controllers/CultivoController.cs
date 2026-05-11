using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class CultivoController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Cultivo
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Cultivo.ToList());
        }

        public ActionResult Create()
        {
            Tbl_Gral_Cultivo tbl_Gral_Cultivo = new Tbl_Gral_Cultivo();
            tbl_Gral_Cultivo.swdatecreated = DateTime.Now;
            tbl_Gral_Cultivo.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Cultivo);
        }

        [HttpPost]
        public ActionResult Create(Tbl_Gral_Cultivo tbl_Gral_Cultivo)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Gral_Cultivo.Max(u => u.Cultivo_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Cultivo.Cultivo_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Cultivo.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Cultivo.Nombre_Tecnico).Max(u => u.Cultivo_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de cultivo ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Cultivo.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de cultivo vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Cultivo.Add(tbl_Gral_Cultivo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Cultivo);
        }

        public ActionResult Edit(int id)
        {
            Tbl_Gral_Cultivo tbl_Gral_Cultivo = db.Tbl_Gral_Cultivo.Find(id);

            if (tbl_Gral_Cultivo == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Cultivo.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Cultivo);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Cultivo tbl_Gral_Cultivo)
        {
            int Error = 0;

            try
            {
                if (tbl_Gral_Cultivo.Cultivo_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Cultivo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Cultivo);
            }
            catch
            {
                return View(tbl_Gral_Cultivo);
            }
        }
    }
}
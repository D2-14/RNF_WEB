using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Materia_PrimaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Materia_Prima
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Materia_Prima.ToList());
        }

        public ActionResult Create()
        {
            Tbl_Gral_Materia_Prima tbl_Gral_Materia_Prima = new Tbl_Gral_Materia_Prima();
            tbl_Gral_Materia_Prima.swdatecreated = DateTime.Now;
            tbl_Gral_Materia_Prima.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Materia_Prima);
        }

        [HttpPost]
        public ActionResult Create(Tbl_Gral_Materia_Prima tbl_Gral_Materia_Prima)
        {
            int intIdt = 0;
            int Error = 0;

            tbl_Gral_Materia_Prima.Estado_id = true;

            tbl_Gral_Materia_Prima.Nombre_Tecnico = tbl_Gral_Materia_Prima.Nombres_Comunes;

            try
            {
                intIdt = db.Tbl_Gral_Materia_Prima.Max(u => u.Materia_Prima_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Materia_Prima.Materia_Prima_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Materia_Prima.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Materia_Prima.Nombre_Tecnico).Max(u => u.Materia_Prima_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de materia prima ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Materia_Prima.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de materia prima vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Materia_Prima.Add(tbl_Gral_Materia_Prima);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Materia_Prima);
        }

        public ActionResult Edit(int id)
        {
            Tbl_Gral_Materia_Prima tbl_Gral_Materia_Prima = db.Tbl_Gral_Materia_Prima.Find(id);

            if (tbl_Gral_Materia_Prima == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Materia_Prima.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Materia_Prima);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Materia_Prima tbl_Gral_Materia_Prima)
        {
            int Error = 0;

            tbl_Gral_Materia_Prima.Nombre_Tecnico = tbl_Gral_Materia_Prima.Nombres_Comunes;

            try
            {
                if (tbl_Gral_Materia_Prima.Materia_Prima_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Materia_Prima).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Materia_Prima);
            }
            catch
            {
                return View(tbl_Gral_Materia_Prima);
            }
        }

    }
}
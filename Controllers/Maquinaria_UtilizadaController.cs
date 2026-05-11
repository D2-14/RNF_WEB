using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Maquinaria_UtilizadaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Maquinaria_Utilizada
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Maquinaria_Utilizada.ToList());
        }

        public ActionResult Create()
        {
            Tbl_Gral_Maquinaria_Utilizada tbl_Gral_Maquinaria_Utilizada = new Tbl_Gral_Maquinaria_Utilizada();
            tbl_Gral_Maquinaria_Utilizada.swdatecreated = DateTime.Now;
            tbl_Gral_Maquinaria_Utilizada.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Maquinaria_Utilizada);
        }

        [HttpPost]
        public ActionResult Create(Tbl_Gral_Maquinaria_Utilizada tbl_Gral_Maquinaria_Utilizada)
        {

            tbl_Gral_Maquinaria_Utilizada.Nombre_Tecnico = tbl_Gral_Maquinaria_Utilizada.Nombres_Comunes;

            tbl_Gral_Maquinaria_Utilizada.Estado_id = true;
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Gral_Maquinaria_Utilizada.Max(u => u.Maquinaria_Utilizada_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Maquinaria_Utilizada.Maquinaria_Utilizada_Id= intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Maquinaria_Utilizada.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Maquinaria_Utilizada.Nombre_Tecnico).Max(u => u.Maquinaria_Utilizada_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de maquinaria utilizada ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Maquinaria_Utilizada.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de maquinaria utilizada vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Maquinaria_Utilizada.Add(tbl_Gral_Maquinaria_Utilizada);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Maquinaria_Utilizada);
        }

        public ActionResult Edit(int id)
        {
            Tbl_Gral_Maquinaria_Utilizada tbl_Gral_Maquinaria_Utilizada = db.Tbl_Gral_Maquinaria_Utilizada.Find(id);

            if (tbl_Gral_Maquinaria_Utilizada == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Maquinaria_Utilizada.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Maquinaria_Utilizada);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Maquinaria_Utilizada tbl_Gral_Maquinaria_Utilizada)
        {

            tbl_Gral_Maquinaria_Utilizada.Nombre_Tecnico = tbl_Gral_Maquinaria_Utilizada.Nombres_Comunes;


            int Error = 0;

            try
            {
                if (tbl_Gral_Maquinaria_Utilizada.Maquinaria_Utilizada_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Maquinaria_Utilizada).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Maquinaria_Utilizada);
            }
            catch
            {
                return View(tbl_Gral_Maquinaria_Utilizada);
            }
        }



        public ActionResult Editar()
        {
            Tbl_Gral_Maquinaria_Utilizada tbl_Gral_Maquinaria_Utilizada = new Tbl_Gral_Maquinaria_Utilizada();
            return View(tbl_Gral_Maquinaria_Utilizada);
        }

    }
}
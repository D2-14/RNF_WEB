using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class ActividadController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Actividad
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Actividad.ToList());
        }

        public ActionResult Create()
        {
            Tbl_Gral_Actividad tbl_Gral_Actividad = new Tbl_Gral_Actividad();
            tbl_Gral_Actividad.swdatecreated = DateTime.Now;
            tbl_Gral_Actividad.swdateupdated = DateTime.Now;
            tbl_Gral_Actividad.Estado_id = true;
            return View(tbl_Gral_Actividad);
        }

        [HttpPost]
        public ActionResult Create(Tbl_Gral_Actividad tbl_Gral_Actividad)
        {
            int intIdt = 0;
            int Error = 0;
            tbl_Gral_Actividad.Estado_id = true;

            if((tbl_Gral_Actividad.Nombre_Tecnico == null) && (tbl_Gral_Actividad.Nombres_Comunes != null))
            {
                tbl_Gral_Actividad.Nombre_Tecnico = tbl_Gral_Actividad.Nombres_Comunes;
            }
            else if ((tbl_Gral_Actividad.Nombres_Comunes == null) && (tbl_Gral_Actividad.Nombre_Tecnico != null))
            {
                tbl_Gral_Actividad.Nombres_Comunes= tbl_Gral_Actividad.Nombre_Tecnico;
            }
            else
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No se encontró nombre de la actividad";
            }

            try
            {
                intIdt = db.Tbl_Gral_Actividad.Max(u => u.Actividad_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Actividad.Actividad_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Actividad.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Actividad.Nombre_Tecnico).Max(u => u.Actividad_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de actividad ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Actividad.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de actividad vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Actividad.Add(tbl_Gral_Actividad);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Actividad);
        }

        public ActionResult Edit(int id)
        {
            Tbl_Gral_Actividad tbl_Gral_Actividad = db.Tbl_Gral_Actividad.Find(id);

            if (tbl_Gral_Actividad == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Actividad.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Actividad);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Actividad tbl_Gral_Actividad)
        {
            int Error = 0;

            tbl_Gral_Actividad.Nombre_Tecnico = tbl_Gral_Actividad.Nombres_Comunes;

            try
            {
                if (tbl_Gral_Actividad.Actividad_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Actividad).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Actividad);
            }
            catch
            {
                return View(tbl_Gral_Actividad);
            }
        }

    }
}
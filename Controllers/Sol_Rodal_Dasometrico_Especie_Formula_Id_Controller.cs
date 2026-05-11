using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_Rodal_Dasometrico_Especie_Formula_Id_Controller : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        public ActionResult Index()
        {
            return View(db.Vw_Sol_Rodal_Dasometrico_Especie_Formula_Id.ToList());
        }

        // GET: Seg_Rol/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Seg_Rol/Create
        public ActionResult Create()
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id tbl_sol_Rodal_Dasometrico_Especie_Formula_Id = new Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id();

            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swcreatedby = objUs.intUsuario_id;
            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swdatecreated = DateTime.Now;
            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swupdatedby = objUs.intUsuario_id;
            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swdateupdated = DateTime.Now;

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico");

            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id tbl_sol_Rodal_Dasometrico_Especie_Formula_Id)
        {
            int intIdt = 0;
            int Error = 0;



            try
            {
                intIdt = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Max(u => u.RodalDasometricoEspecieFormula_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.RodalDasometricoEspecieFormula_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(obj => obj.Descripcion == tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Descripcion).Max(u => u.RodalDasometricoEspecieFormula_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["FormulaMessage"] = "Error: La formula ya existe, no debe ingresarla de nuevo";
            }

            if (tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Descripcion.Trim() == "")
            {
                Error = 1;
                TempData["FormulaMessage"] = "Error: No puede grabar una formula vacia.";
            }

            if (tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.FormulaSistemas.Trim() == "")
            {
                Error = 1;
                TempData["FormulaMessage"] = "Error: No puede grabar una formula interna vacia.";
            }

            if (Error == 0)
            {

                int cantidad = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Especie_id).Count();

                if (cantidad == 1)
                {
                    Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Especie_id).First();

                    EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Descripcion = tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Descripcion;

                    EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.FormulaSistemas = tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.FormulaSistemas;

                    EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.swupdatedby = tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swupdatedby;
                    EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.swdateupdated = tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swdatecreated;

                    db.Entry(EncontradoTbl_Sol_Rodal_Dasometrico_Especie_Formula_Id).State = EntityState.Modified;
                    db.SaveChanges();


                }
                else
                { 
                        if (ModelState.IsValid)
                        {
                            db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Add(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                }
            }

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Especie_id);

            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);

        }


        // GET: Seg_Rol/Edit/5
        public ActionResult Edit(int id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id tbl_sol_Rodal_Dasometrico_Especie_Formula_Id = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Find(id);

            if (tbl_sol_Rodal_Dasometrico_Especie_Formula_Id == null)
            {
                return HttpNotFound();
            }

            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swdateupdated = DateTime.Now;
            tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.swupdatedby = objUs.intUsuario_id;

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Especie_id);

            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);

        }

        // POST: Seg_Rol/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id tbl_sol_Rodal_Dasometrico_Especie_Formula_Id)
        {
            int Error = 0;

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.Especie_id);

            try
            {

                if (tbl_sol_Rodal_Dasometrico_Especie_Formula_Id.RodalDasometricoEspecieFormula_Id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar la opcion de NO aplica";

                }


                if (Error == 0)
                {
                    db.Entry(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);
            }
            catch
            {
                return View(tbl_sol_Rodal_Dasometrico_Especie_Formula_Id);
            }
        }

        // GET: Seg_Rol/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Seg_Rol/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

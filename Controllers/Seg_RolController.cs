using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Seg_RolController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();


        // GET: Seg_Rol
        public ActionResult Index()
        {
            return View(db.Vw_Seg_Sel_Rol.ToList());
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

            Tbl_Seg_Rol tbl_seg_rol = new Tbl_Seg_Rol();

            tbl_seg_rol.swcreatedby = objUs.intUsuario_id;
            tbl_seg_rol.swdatecreated = DateTime.Now;
            tbl_seg_rol.swUpdatedby = objUs.intUsuario_id;
            tbl_seg_rol.swdateUpdated = DateTime.Now;

            return View(tbl_seg_rol);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Seg_Rol tbl_seg_rol)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Seg_Rol.Max(u => u.Rol_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_seg_rol.Rol_id = intIdt;

            try
            {
                intIdt = db.Tbl_Seg_Rol.Where(obj=> obj.Nombre == tbl_seg_rol.Nombre).Max(u => u.Rol_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El rol ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_seg_rol.Nombre.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un roll vacio";
            }

            if ( Error == 0 ) 
            { 
                if (ModelState.IsValid)
                {
                    db.Tbl_Seg_Rol.Add(tbl_seg_rol);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_seg_rol);


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


            Tbl_Seg_Rol tbl_seg_rol = db.Tbl_Seg_Rol.Find(id);
            if (tbl_seg_rol == null)
            {
                return HttpNotFound();
            }

            tbl_seg_rol.swdateUpdated = DateTime.Now;
            tbl_seg_rol.swUpdatedby = objUs.intUsuario_id;

            return View(tbl_seg_rol);

        }

        // POST: Seg_Rol/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Seg_Rol tbl_seg_rol)
        {
            int Error = 0;

            try
            {
                if (tbl_seg_rol.Rol_id == 0)
                {
                    Error = 1;
                    TempData["RollMessage"] = "No puede editar el roll Administrador";

                }

                if (Error == 0)
                {
                    db.Entry(tbl_seg_rol).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_seg_rol);
            }
            catch
            {
                return View(tbl_seg_rol);
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

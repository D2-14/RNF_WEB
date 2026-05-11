using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gral_ProfesionController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();


        // GET: Gral_Profesion
        public ActionResult Index()
        {
            var tbl_gral_Profesion = db.Tbl_Gral_Profesion.Where(Obj=>Obj.Profesion_id>0);
            return View(tbl_gral_Profesion.ToList());

        }

  
        // GET: Gral_Profesion/Create
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

            Tbl_Gral_Profesion tbl_gral_Profesion = new Tbl_Gral_Profesion();

            tbl_gral_Profesion.Profesion_id = 0;
            tbl_gral_Profesion.swcreatedby = objUs.intUsuario_id;
            tbl_gral_Profesion.swdatecreated = DateTime.Now;
            tbl_gral_Profesion.swUpdatedby = objUs.intUsuario_id;
            tbl_gral_Profesion.swdateUpdated = DateTime.Now;

            return View(tbl_gral_Profesion); ;
        }

        // POST: Gral_Profesion/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gral_Profesion tbl_gral_Profesion)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Gral_Profesion.Max(u => u.Profesion_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_gral_Profesion.Profesion_id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Profesion.Where(obj => obj.Descripcion == tbl_gral_Profesion.Descripcion).Max(u => u.Profesion_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: La profesion ya existe. No puede agregarla nuevamente";
            }

            if (tbl_gral_Profesion.Descripcion.Trim() == "")
            {
                Error = 1;
                TempData["ProfesionMessage"] = "Error: No puede grabar una profesion vacia";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Profesion.Add(tbl_gral_Profesion);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_gral_Profesion);

        }

        // GET: Gral_Profesion/Edit/5
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


            Tbl_Gral_Profesion tbl_gral_Profesion = db.Tbl_Gral_Profesion.Find(id);
            if (tbl_gral_Profesion == null)
            {
                return HttpNotFound();
            }

            tbl_gral_Profesion.swdateUpdated = DateTime.Now;
            tbl_gral_Profesion.swUpdatedby = objUs.intUsuario_id;

            return View(tbl_gral_Profesion);

        }

        // POST: Gral_Profesion/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Profesion tbl_gral_Profesion)
        {
            int Error = 0;

            try
            {
                if (tbl_gral_Profesion.Profesion_id == 0)
                {
                    Error = 1;
                    TempData["ProfesionMessage"] = "No puede editar la profesion default";

                }

                if (Error == 0)
                {
                    db.Entry(tbl_gral_Profesion).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_gral_Profesion);
            }
            catch
            {
                return View(tbl_gral_Profesion);
            }
        }


        [HttpPost]
        public JsonResult GetProfesion(long Solicitud_id, bool GAT, bool GAP)
        {
            IEnumerable<Tbl_Gral_Profesion> Profesiones = null;

            if ((GAT == false) && (GAP == false))
            {
                Profesiones = (from c in db.Tbl_Gral_Profesion
                               where c.Profesion_id == 0
                               select c);
            }
            else
            {
                Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

                if (tbl_Sol_Solicitud == null)
                {
                    Profesiones = (from c in db.Tbl_Gral_Profesion
                                   where c.Profesion_id == 0
                                   select c);

                }
                else
                {

                    if (tbl_Sol_Solicitud.Sub_Categoria_id == 1) //Regente
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.Regente == true)
                                       select c);

                    }
                    if (tbl_Sol_Solicitud.Sub_Categoria_id == 2) //Elaborador de planes de manejo forestal
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.ElaboradorDePlanesManejoForestal == true)
                                       select c);
                    }
                    if (tbl_Sol_Solicitud.Sub_Categoria_id == 3) //Elaborador de estudios de capacidad de uso del suelo
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.ElaboradorDeEstudiosUsodelSuelo == true)
                                       select c);

                    }
                    if (tbl_Sol_Solicitud.Sub_Categoria_id == 4) //Certificador de fuentes semilleras
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.CertificadorDeFuentesSemilleras == true)
                                       select c);
                    }

                }

            }



            var Municipios = new SelectList(Profesiones, "Profesion_id", "Descripcion");

            return Json(new SelectList(Municipios, "Value", "Text"));

        }
    }
}

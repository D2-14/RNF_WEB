using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;
using System.Data.SqlClient;



namespace RNF_Web.Controllers
{
    public class Sol_Rodal_InventarioController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();




        // GET: Sol_Rodal_Inventario/Create
        public ActionResult Index(long Id)
        {

            var tbl_Sol_Rodal_Inventario = db.Tbl_Sol_Rodal_Inventario.Where(Obj => Obj.Solicitud_id == Id);
            return View(tbl_Sol_Rodal_Inventario.ToList());

        }

        public ActionResult Create(long solicitud_id, string firma)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Rodal_Inventario tbl_Sol_Rodal_Inventario = new Tbl_Sol_Rodal_Inventario();

            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Rodal_Inventario.Where(Arbol => Arbol.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Arbol.Rodal_id == 1).Max(u => u.Arbol_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_Rodal_Inventario.Arbol_id = lngIdt;

            tbl_Sol_Rodal_Inventario.Solicitud_id = tbl_sol_solicitud.Solicitud_id;


            tbl_Sol_Rodal_Inventario.swcreatedby = objUs.intUsuario_id;
            tbl_Sol_Rodal_Inventario.swdatecreated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_Rodal_Inventario.swcreatedbyinterno = false;
            }
            else
            {
                tbl_Sol_Rodal_Inventario.swcreatedbyinterno = true;
            }

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", 1);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", 1);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id");

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj=> Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id), "Rodal_Id", "Rodal_Id");

            tbl_Sol_Rodal_Inventario.DAP = 0;

            tbl_Sol_Rodal_Inventario.Altura = 0;

            tbl_Sol_Rodal_Inventario.InventarioClase_id = 0;

            tbl_Sol_Rodal_Inventario.EstadoFitosanitario_id = 0;

            tbl_Sol_Rodal_Inventario.Especie_Id = "";

            return View(tbl_Sol_Rodal_Inventario);


        }

        public ActionResult Edit(long varRodalid, int varArbolid, long solicitud_id, string firma)
        {
            long varSolicitudid = solicitud_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Rodal_Inventario tbl_Sol_Rodal_Inventario = db.Tbl_Sol_Rodal_Inventario.Where(Obj=> Obj.Solicitud_id == varSolicitudid && Obj.Rodal_id == varRodalid && Obj.Arbol_id == varArbolid).First();

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_Sol_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_Sol_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Rodal_Id == tbl_Sol_Rodal_Inventario.Rodal_id), "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Inventario.Rodal_id);

            return View(tbl_Sol_Rodal_Inventario);
        }


        public ActionResult Borrar(long varRodalid, int varArbolid, long solicitud_id, string firma)
        {
            long varSolicitudid = solicitud_id;



            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            int intInventarioMayor = db.Tbl_Sol_Rodal_Inventario.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Rodal_id == varRodalid && Obj.Arbol_id > varArbolid).Count();


            if (intInventarioMayor > 0)
            {
                return RedirectToAction("../Sol_Rodal_Inventario/BorradoFallido");
            }

            Tbl_Sol_Rodal_Inventario tbl_Sol_Rodal_Inventario = db.Tbl_Sol_Rodal_Inventario.Where(Obj=> Obj.Solicitud_id == varSolicitudid && Obj.Rodal_id == varRodalid && Obj.Arbol_id == varArbolid).First();
            db.Tbl_Sol_Rodal_Inventario.Remove(tbl_Sol_Rodal_Inventario);
            db.SaveChanges();

            return View();

        }

        public ActionResult BorradoFallido()
        {

            return View();

        }



        [HttpPost]
        public ActionResult Edit(Tbl_Sol_Rodal_Inventario tbl_Sol_Rodal_Inventario)
        {

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Rodal_Inventario.Solicitud_id, "Sol_Rodal_Dasometricos", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos.";
                return RedirectToAction("../Home/RegistroNoActualizado");
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                db.Entry(tbl_Sol_Rodal_Inventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");
            }

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_Sol_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_Sol_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Rodal_Id == tbl_Sol_Rodal_Inventario.Rodal_id), "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Inventario.Rodal_id);

            return View(tbl_Sol_Rodal_Inventario);

        }

            [HttpPost]
        public JsonResult GetNextTree(int rodal_id)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Rodal_Inventario.Where(Arbol => Arbol.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Arbol.Rodal_id == rodal_id).Max(u => u.Arbol_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }




            return Json(lngIdt);

        }

 


// POST: Sol_Rodal_Inventario/Create
[HttpPost]
        public ActionResult Create(Tbl_Sol_Rodal_Inventario tbl_Sol_Rodal_Inventario)
        {
            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            if (objGrant.boolEditarRepresentante == false)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos de rodales.";
                return RedirectToAction("../Home/RegistroAgregado");
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                int lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Sol_Rodal_Inventario.Where(RodalInventario => RodalInventario.Solicitud_id == tbl_sol_solicitud.Solicitud_id && RodalInventario.Rodal_id == tbl_Sol_Rodal_Inventario.Rodal_id).Max(u => u.Arbol_id);
                    lngIdt++;
                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Sol_Rodal_Inventario.Arbol_id = lngIdt;

                db.Tbl_Sol_Rodal_Inventario.Add(tbl_Sol_Rodal_Inventario);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }


            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_Sol_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_Sol_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_Sol_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id), "Rodal_Id", "Rodal_Id", tbl_Sol_Rodal_Inventario.Rodal_id);

            return View(tbl_Sol_Rodal_Inventario);

        }


        // GET: Sol_Rodal_Inventario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sol_Rodal_Inventario/Delete/5
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

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
    public class RNF_Rodal_InventarioController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        // GET: RNF_Rodal_Inventario
        public ActionResult Index(string Guid_id)
        {
            ViewBag.Guid_id = Guid_id;
            var tbl_RNF_Rodal_Inventario = db.Tbl_RNF_Rodal_Inventario.Where(Obj => Obj.No_Registro == Guid_id);
            return View(tbl_RNF_Rodal_Inventario.ToList());
        }

        public ActionResult Create(string No_Registro)
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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Rodal_Inventario tbl_RNF_Rodal_Inventario = new Tbl_RNF_Rodal_Inventario();

            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_Rodal_Inventario.Where(Arbol => Arbol.No_Registro == tbl_RNF_Registro.No_Registro && Arbol.Rodal_id == 1).Max(u => u.Arbol_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_RNF_Rodal_Inventario.Arbol_id = lngIdt;

            tbl_RNF_Rodal_Inventario.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_Rodal_Inventario.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_Rodal_Inventario.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_Rodal_Inventario.Solicitud_id = tbl_RNF_Registro.Solicitud_id;


            tbl_RNF_Rodal_Inventario.swcreatedby = objUs.intUsuario_id;
            tbl_RNF_Rodal_Inventario.swdatecreated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Rodal_Inventario.swcreatedbyinterno = false;
            }
            else
            {
                tbl_RNF_Rodal_Inventario.swcreatedbyinterno = true;
            }

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", 1);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", 1);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id");

            ViewBag.Rodal_id = new SelectList(db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro), "Rodal_Id", "Rodal_Id");

            tbl_RNF_Rodal_Inventario.DAP = 0;

            tbl_RNF_Rodal_Inventario.Altura = 0;

            tbl_RNF_Rodal_Inventario.InventarioClase_id = 0;

            tbl_RNF_Rodal_Inventario.EstadoFitosanitario_id = 0;

            tbl_RNF_Rodal_Inventario.Especie_Id = "";

            return View(tbl_RNF_Rodal_Inventario);


        }

        public ActionResult Edit(string No_Registro, long varRodalid, int varArbolid)
        {

            Tbl_RNF_Rodal_Inventario tbl_RNF_Rodal_Inventario = db.Tbl_RNF_Rodal_Inventario.Where(Obj => Obj.No_Registro == No_Registro && Obj.Rodal_id == varRodalid && Obj.Arbol_id == varArbolid).FirstOrDefault();

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_RNF_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_RNF_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_RNF_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == No_Registro && Obj.Rodal_Id == tbl_RNF_Rodal_Inventario.Rodal_id), "Rodal_Id", "Rodal_Id", tbl_RNF_Rodal_Inventario.Rodal_id);

            return View(tbl_RNF_Rodal_Inventario);
        }


        public ActionResult Borrar(string No_Registro, long varRodalid, int varArbolid)
        {

            int intInventarioMayor = db.Tbl_RNF_Rodal_Inventario.Where(Obj => Obj.No_Registro == No_Registro && Obj.Rodal_id == varRodalid && Obj.Arbol_id > varArbolid).Count();


            if (intInventarioMayor > 0)
            {
                return RedirectToAction("../Sol_Rodal_Inventario/BorradoFallido");
            }

            Tbl_RNF_Rodal_Inventario tbl_RNF_Rodal_Inventario = db.Tbl_RNF_Rodal_Inventario.Where(Obj => Obj.No_Registro == No_Registro && Obj.Rodal_id == varRodalid && Obj.Arbol_id == varArbolid).FirstOrDefault();
            db.Tbl_RNF_Rodal_Inventario.Remove(tbl_RNF_Rodal_Inventario);
            db.SaveChanges();

            return View();

        }



        [HttpPost]
        public ActionResult Edit(Tbl_RNF_Rodal_Inventario tbl_RNF_Rodal_Inventario)
        {

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos.";
                return RedirectToAction("../Home/RegistroNoActualizado");
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Rodal_Inventario.No_Registro).FirstOrDefault();


            if (ModelState.IsValid)
            {
                db.Entry(tbl_RNF_Rodal_Inventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");
            }

            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_RNF_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_RNF_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_RNF_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Rodal_Id == tbl_RNF_Rodal_Inventario.Rodal_id), "Rodal_Id", "Rodal_Id", tbl_RNF_Rodal_Inventario.Rodal_id);

            return View(tbl_RNF_Rodal_Inventario);

        }

        [HttpPost]
        public JsonResult GetNextTree(string No_Registro, int rodal_id)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_Rodal_Inventario.Where(Arbol => Arbol.Solicitud_id == tbl_RNF_Registro.Solicitud_id && Arbol.Rodal_id == rodal_id).Max(u => u.Arbol_id);
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
        public ActionResult Create(Tbl_RNF_Rodal_Inventario tbl_RNF_Rodal_Inventario)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos de rodales.";
                return RedirectToAction("../Home/RegistroAgregado");
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Rodal_Inventario.No_Registro).FirstOrDefault();

            if (ModelState.IsValid)
            {
                int lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_RNF_Rodal_Inventario.Where(RodalInventario => RodalInventario.No_Registro == tbl_RNF_Registro.No_Registro && RodalInventario.Rodal_id == tbl_RNF_Rodal_Inventario.Rodal_id).Max(u => u.Arbol_id);
                    lngIdt++;
                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_RNF_Rodal_Inventario.Arbol_id = lngIdt;

                db.Tbl_RNF_Rodal_Inventario.Add(tbl_RNF_Rodal_Inventario);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }


            ViewBag.InventarioClase_id = new SelectList(db.Tbl_Sol_Rodal_Inventario_Clase, "InventarioClase_id", "Descripcion", tbl_RNF_Rodal_Inventario.InventarioClase_id);

            ViewBag.EstadoFitosanitario_id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_EstadoFitosanitario.Where(Obj => Obj.EstadoFitosanitario_id > 0), "EstadoFitosanitario_id", "Descripcion", tbl_RNF_Rodal_Inventario.EstadoFitosanitario_id);

            ViewBag.Especie_Id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "Especie_Id", tbl_RNF_Rodal_Inventario.Especie_Id);

            ViewBag.Rodal_id = new SelectList(db.Tbl_RNF_Rodal.Where(Obj => Obj.Solicitud_id == tbl_RNF_Rodal_Inventario.Solicitud_id), "Rodal_Id", "Rodal_Id", tbl_RNF_Rodal_Inventario.Rodal_id);

            return View(tbl_RNF_Rodal_Inventario);

        }


    }
}
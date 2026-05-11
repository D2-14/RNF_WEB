using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gral_EspecieController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();


        public ActionResult ListadoEspecies(string strBusqueda)
        {
            if (strBusqueda == null || strBusqueda == "")
            {
                return View(db.Tbl_Gral_Especie.Where(Obj=>Obj.Estado_id == true).ToList().Take(100));
            }

            ViewBag.strBusqueda = strBusqueda;

            return View(db.Tbl_Gral_Especie.Where(Obj => Obj.Estado_id == true && (Obj.Especie_Id.ToLower().Contains(strBusqueda.ToLower()) || Obj.NombreCientifico.ToLower().Contains(strBusqueda.ToLower()))).ToList());

        }

        // GET: Gral_Especie
        public ActionResult Index(string strBusqueda)
        {
            if (strBusqueda == null || strBusqueda == "")
            {
                return View(db.Tbl_Gral_Especie.ToList().Take(100));
            }

            ViewBag.strBusqueda = strBusqueda;

            return View(db.Tbl_Gral_Especie.Where(Obj=> Obj.Especie_Id.ToLower().Contains(strBusqueda.ToLower()) || Obj.NombreCientifico.ToLower().Contains(strBusqueda.ToLower())).ToList());

        }


        // GET: Sol_Finca/Create
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

            Tbl_Gral_Especie tbl_gral_especie = new Tbl_Gral_Especie();

            tbl_gral_especie.swcreatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swcreatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swcreatedbyinterno = true;

            }
            tbl_gral_especie.Estado_id = true;

            return View(tbl_gral_especie);
        }

        // POST: Sol_Finca/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gral_Especie tbl_gral_especie)
        {
            tbl_gral_especie.Especie_Id = tbl_gral_especie.Especie_Id.Trim();

            int cantidad = db.Tbl_Gral_Especie.Where(obj => obj.Especie_Id == tbl_gral_especie.Especie_Id).Count();

            if (cantidad > 0)
            {

                db.Entry(tbl_gral_especie).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("../Home/RegistroActualizado");
            }
            tbl_gral_especie.Estado_id = true;
            db.Tbl_Gral_Especie.Add(tbl_gral_especie);
            db.SaveChanges();
            return RedirectToAction("../Home/RegistroAgregado");


            
        }



        // GET: Sol_Finca/Create
        public ActionResult Edit(string especieid)
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

            Tbl_Gral_Especie tbl_gral_especie = db.Tbl_Gral_Especie.Find(especieid);

            tbl_gral_especie.swupdatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swupdatedbyinterno = true;

            }

            return View(tbl_gral_especie);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Especie tbl_gral_especie)
        {

                db.Entry(tbl_gral_especie).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("../Home/RegistroActualizado");



        }

        public JsonResult SetArbolFrutal(string especieid, bool arbolfrutal)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gral_Especie tbl_gral_especie = db.Tbl_Gral_Especie.Find(especieid);



            tbl_gral_especie.swupdatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swupdatedbyinterno = true;
            }

            tbl_gral_especie.ArbolFrutal = arbolfrutal;


            db.Entry(tbl_gral_especie).State = EntityState.Modified;
            db.SaveChanges();


            return Json("");
        }


        public JsonResult SetEspecieEstado(string especieid, bool intestado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gral_Especie tbl_gral_especie = db.Tbl_Gral_Especie.Find(especieid);



            tbl_gral_especie.swupdatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swupdatedbyinterno = true;
            }

            tbl_gral_especie.Estado_id = intestado;


            db.Entry(tbl_gral_especie).State = EntityState.Modified;
            db.SaveChanges();


            return Json("");
        }


        public JsonResult SetEspecieConifera(string especieid, bool conifera)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gral_Especie tbl_gral_especie = db.Tbl_Gral_Especie.Find(especieid);



            tbl_gral_especie.swupdatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swupdatedbyinterno = true;
            }

            if (conifera == true)
            {
                tbl_gral_especie.Latifoliada = false;

            }
            else
            {
                tbl_gral_especie.Latifoliada = true;
            }




            tbl_gral_especie.Conifera = conifera;


            db.Entry(tbl_gral_especie).State = EntityState.Modified;
            db.SaveChanges();


            return Json("");
        }


        public JsonResult SetEspecieLatifoliada(string especieid, bool latifoliada)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gral_Especie tbl_gral_especie = db.Tbl_Gral_Especie.Find(especieid);



            tbl_gral_especie.swupdatedby = objUs.intUsuario_id;
            tbl_gral_especie.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gral_especie.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gral_especie.swupdatedbyinterno = true;
            }

            if (latifoliada == true)
            {
                tbl_gral_especie.Conifera = false;

            }
            else
            {
                tbl_gral_especie.Conifera = true;
            }

            tbl_gral_especie.Latifoliada = latifoliada;


            db.Entry(tbl_gral_especie).State = EntityState.Modified;
            db.SaveChanges();


            return Json("");
        }


    }
}
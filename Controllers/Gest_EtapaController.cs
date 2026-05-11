using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gest_EtapaController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        public ActionResult TodoList(int Etapaid, decimal EtapaRutaid)
        {
            Usuario objUs = new Usuario(); 
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();


                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.Etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == Etapaid && Obj.EtapaRuta_id == EtapaRutaid).First();

            ViewBag.etapaid = Etapaid;

            ViewBag.etaparutaid = EtapaRutaid;

            return View(db.Tbl_Gest_Etapa_To_doList.Where(Obj => Obj.Etapa_id == Etapaid && Obj.EtapaRuta_id == EtapaRutaid).ToList());
        }

        // GET: Gest_Etapa
        public ActionResult Index(decimal rutaid)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.rutaid = rutaid;

            ViewBag.NombreRuta = db.Database.SqlQuery<string>("Select Descripcion From Tbl_Gest_EtapaRuta Where EtapaRuta_id =@p0", rutaid).FirstOrDefault();


            return View(db.Tbl_Gest_Etapa.Where(Obj => Obj.EtapaRuta_id == rutaid).ToList());
        }

        public ActionResult ConfigurarEtapa(int etapaid, decimal EtapaRutaid)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_Etapa tbl_gest_etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.EtapaRuta_id == EtapaRutaid && Obj.Etapa_id == etapaid).First();


            tbl_gest_etapa.swupdatedby = objUs.intUsuario_id;
            tbl_gest_etapa.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gest_etapa.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gest_etapa.swupdatedbyinterno = true;

            }


            ViewBag.Rol_id = new SelectList(db.Tbl_Seg_Rol, "Rol_id", "Nombre", tbl_gest_etapa.Rol_id);



            return View(tbl_gest_etapa);
        }

        [HttpPost]
        public ActionResult ConfigurarEtapa(Tbl_Gest_Etapa tbl_Gest_EtapaActualizar)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            tbl_Gest_EtapaActualizar.swupdatedby = objUs.intUsuario_id;
            tbl_Gest_EtapaActualizar.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Gest_EtapaActualizar.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Gest_EtapaActualizar.swupdatedbyinterno = true;

            }


            ViewBag.Rol_id = new SelectList(db.Tbl_Seg_Rol, "Rol_id", "Nombre", tbl_Gest_EtapaActualizar.Rol_id);

            if (ModelState.IsValid)
            {

                db.Entry(tbl_Gest_EtapaActualizar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Gest_Etapa/ConfigurarEtapa", new { etapaid = tbl_Gest_EtapaActualizar.Etapa_id, EtapaRutaid = tbl_Gest_EtapaActualizar.EtapaRuta_id });

            }

            ViewBag.Gest_EtapaNuevaError = "No se puede grabar la etapa, favor revisar los datos";
            return RedirectToAction("../Gest_Etapa/ConfigurarEtapa", new { etapaid = tbl_Gest_EtapaActualizar.Etapa_id, EtapaRutaid = tbl_Gest_EtapaActualizar.EtapaRuta_id });

        }


        // GET: Gest_Etapa
        public ActionResult Create(decimal rutaid)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_Etapa tbl_Gest_EtapaNueva = new Tbl_Gest_Etapa();


            ViewBag.Rol_id = new SelectList(db.Tbl_Seg_Rol, "Rol_id", "Nombre");


            tbl_Gest_EtapaNueva.EtapaRuta_id = rutaid;
            tbl_Gest_EtapaNueva.Tbl_Gest_EtapaRuta = db.Tbl_Gest_EtapaRuta.Find(rutaid);
            tbl_Gest_EtapaNueva.Nombre_Etapa = "";
            tbl_Gest_EtapaNueva.EscalamientoA_Horas = "0";
            tbl_Gest_EtapaNueva.EscalamientoA_Email = "";
            tbl_Gest_EtapaNueva.EscalamientoA_Telefono = "";

            tbl_Gest_EtapaNueva.EscalamientoB_Horas = "0";
            tbl_Gest_EtapaNueva.EscalamientoB_Email = "";
            tbl_Gest_EtapaNueva.EscalamientoB_Telefono = "";

            tbl_Gest_EtapaNueva.EscalamientoC_Horas = "0";
            tbl_Gest_EtapaNueva.EscalamientoC_Email = "";
            tbl_Gest_EtapaNueva.EscalamientoC_Telefono = "";



            ViewBag.rutaid = rutaid;
            return View(tbl_Gest_EtapaNueva);
        }

        // POST: Sol_Finca/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gest_Etapa tbl_Gest_EtapaNueva)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_Gest_EtapaNueva.swcreatedby = objUs.intUsuario_id;
            tbl_Gest_EtapaNueva.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Gest_EtapaNueva.swcreatedbyinterno = false;

            }
            else
            {
                tbl_Gest_EtapaNueva.swcreatedbyinterno = true;
            }


            tbl_Gest_EtapaNueva.swupdatedby = objUs.intUsuario_id;
            tbl_Gest_EtapaNueva.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Gest_EtapaNueva.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Gest_EtapaNueva.swupdatedbyinterno = true;

            }


            ViewBag.Rol_id = new SelectList(db.Tbl_Seg_Rol, "Rol_id", "Nombre", tbl_Gest_EtapaNueva.Rol_id);


            int intKey = 0;

            try
            {
                intKey = db.Tbl_Gest_Etapa.Where(Etapa => Etapa.EtapaRuta_id == tbl_Gest_EtapaNueva.EtapaRuta_id).Max(u => u.Etapa_id);
                intKey++;
            }
            catch
            {
                intKey = 1;
            }
            tbl_Gest_EtapaNueva.Etapa_id = intKey; 


            if (ModelState.IsValid)
            {
                db.Tbl_Gest_Etapa.Add(tbl_Gest_EtapaNueva);
                db.SaveChanges();
                return RedirectToAction("../Gest_Etapa/ConfigurarEtapa", new { etapaid = tbl_Gest_EtapaNueva.Etapa_id, EtapaRutaid= tbl_Gest_EtapaNueva.EtapaRuta_id } );

            }

            ViewBag.Gest_EtapaNuevaError = "No se puede grabar la etapa, favor revisar los datos";
            return View(tbl_Gest_EtapaNueva);

        }


        public JsonResult Agregar_To_Do_Item(int etapaid, decimal etaparutaid, string descripcion)
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


            Tbl_Gest_Etapa_To_doList Tbl_Gest_Etapa_To_doListNueva = new Tbl_Gest_Etapa_To_doList();

            Tbl_Gest_Etapa_To_doListNueva.Etapa_id = etapaid;
            Tbl_Gest_Etapa_To_doListNueva.EtapaRuta_id = etaparutaid;
            Tbl_Gest_Etapa_To_doListNueva.Descripcion = descripcion;


            int intKey = 0;

            try
            {
                intKey = db.Tbl_Gest_Etapa_To_doList.Where(Etapa => Etapa.Etapa_id == Tbl_Gest_Etapa_To_doListNueva.Etapa_id && Etapa.EtapaRuta_id == Tbl_Gest_Etapa_To_doListNueva.EtapaRuta_id).Max(u => u.To_do_id);
                intKey++;
            }
            catch
            {
                intKey = 1;
            }

            Tbl_Gest_Etapa_To_doListNueva.To_do_id = intKey;

            Tbl_Gest_Etapa_To_doListNueva.swcreatedby = objUs.intUsuario_id;
            Tbl_Gest_Etapa_To_doListNueva.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                Tbl_Gest_Etapa_To_doListNueva.swcreatedbyinterno = false;

            }
            else
            {
                Tbl_Gest_Etapa_To_doListNueva.swcreatedbyinterno = true;
            }


            Tbl_Gest_Etapa_To_doListNueva.swupdatedby = objUs.intUsuario_id;
            Tbl_Gest_Etapa_To_doListNueva.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                Tbl_Gest_Etapa_To_doListNueva.swupdatedbyinterno = false;

            }
            else
            {
                Tbl_Gest_Etapa_To_doListNueva.swupdatedbyinterno = true;

            }

            Tbl_Gest_Etapa_To_doListNueva.Estado = true;

            if (ModelState.IsValid)
            {
                db.Tbl_Gest_Etapa_To_doList.Add(Tbl_Gest_Etapa_To_doListNueva);
                db.SaveChanges();
            }
                return Json("");
        }


        public JsonResult CambiarEstado_To_Do_Item(int etapaid, decimal etaparutaid, int Todoid, bool estado)
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

            Tbl_Gest_Etapa_To_doList Tbl_Gest_Etapa_To_doListChange = db.Tbl_Gest_Etapa_To_doList.Where(Obj=> Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.To_do_id == Todoid).First();

            Tbl_Gest_Etapa_To_doListChange.Estado = estado;


            db.Entry(Tbl_Gest_Etapa_To_doListChange).State = EntityState.Modified;
            db.SaveChanges();

            return Json("");
        }

    }
}

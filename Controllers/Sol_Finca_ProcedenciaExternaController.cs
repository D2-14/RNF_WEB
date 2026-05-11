using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_Finca_ProcedenciaExternaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
        }
        public ActionResult Probosque_EspeciesForestales(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;

            List<Tbl_Sol_Finca_Probosque_EspeciesForestales> tbl_Sol_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Probosque_EspeciesForestales
                                                                                                           where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                                                                           orderby d.Finca_id, d.Correlativo_id
                                                                                                           select d).ToList();

            if (tbl_Sol_Finca_Probosque_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Probosque_EspeciesForestales = new List<Tbl_Sol_Finca_Probosque_EspeciesForestales>();
            }

            return View(tbl_Sol_Finca_Probosque_EspeciesForestales);
        }

        public ActionResult Probosque_EspeciesForestalesAgregar(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;


            Tbl_Sol_Finca tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                           where d.Solicitud_id == Solicitud_id
                                           select d).FirstOrDefault();

            long fincaid = 0;
            decimal area = 0;
            if (tbl_Sol_Finca != null)
            {
                fincaid = tbl_Sol_Finca.Finca_Id;
                area = (tbl_Sol_Finca.AreaARegistrar??0);
            }

            Tbl_Sol_Finca_Probosque_EspeciesForestales tbl_Sol_Finca_Probosque_EspeciesForestales = new Tbl_Sol_Finca_Probosque_EspeciesForestales()
            {
                Solicitud_id = tbl_sol_solicitud.Solicitud_id,
                Finca_id = fincaid,
                Referencia_id = 0,
                Area = area,
            };

            return View(tbl_Sol_Finca_Probosque_EspeciesForestales);
        }

        [HttpPost]
        public ActionResult Probosque_EspeciesForestalesAgregar(Tbl_Sol_Finca_Probosque_EspeciesForestales model)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            long Correlativo_id = 0;
            try
            {
                Correlativo_id = db.Tbl_Sol_Finca_Probosque_EspeciesForestales.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Finca_id == model.Finca_id).Max(Obj => Obj.Correlativo_id);
            }
            catch (Exception ex)
            {
                Correlativo_id = 0;
            }
            Correlativo_id++;

            model.Correlativo_id = Correlativo_id;
            model.Referencia_id = model.Referencia_id;
            model.Area = model.Area ?? 0;
            model.NombreCientifico = model.NombreCientifico ?? "spp";
            model.CicloDeCorta = model.CicloDeCorta ?? 0;
            model.DensidadActual = model.DensidadActual ?? 0;
            model.DensidadFinal = model.DensidadFinal ?? 0;
            model.DensidadInicial = model.DensidadInicial ?? 0;
            model.Mixtaje = model.Mixtaje ?? 0;
            model.DistanciaES = model.DistanciaES ?? 0;
            model.DistanciaEP = model.DistanciaEP ?? 0;
            model.Supervivencia = model.Supervivencia ?? 0;
            model.PlantasSanas = model.PlantasSanas ?? 0;
            model.PlantacionDPA = model.PlantacionDPA ?? 0;
            model.PlantacionAltura = model.PlantacionAltura ?? 0;
            model.PlantasAfectadas = model.PlantasAfectadas ?? 0;
            model.PlantasEnfermedad = model.PlantasEnfermedad ?? 0;
            model.PlantasFuego = model.PlantasFuego ?? 0;
            model.RegistroExterno = false;
            if ((model.FechaPlantacion == null) || ((DateTime)model.FechaPlantacion != null))
            {
                DateTime fechalantacion = (DateTime)model.FechaPlantacion;
                DateTime anioactual = DateTime.Now;

            }

            if (ModelState.IsValid)
            {
                db.Tbl_Sol_Finca_Probosque_EspeciesForestales.Add(model);
                db.SaveChanges();
                return RedirectToAction("RegistroAgregado", "Home");

            }

            return View(model);
        }

        public ActionResult Probosque_EspeciesForestales_Borrar(Tbl_Sol_Finca_Probosque_EspeciesForestales model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            Tbl_Sol_Finca_Probosque_EspeciesForestales tbl_Sol_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Probosque_EspeciesForestales
                                                                                                     where d.Solicitud_id == model.Solicitud_id
                                                                                                     && d.Finca_id == model.Finca_id
                                                                                                     && d.Correlativo_id == model.Correlativo_id
                                                                                                     select d).FirstOrDefault();

            if (tbl_Sol_Finca_Probosque_EspeciesForestales != null)
            {
                db.Tbl_Sol_Finca_Probosque_EspeciesForestales.Remove(tbl_Sol_Finca_Probosque_EspeciesForestales);
                db.SaveChanges();
                return RedirectToAction("RegistroEliminado", "Home");

            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

        public ActionResult PinpepOld_EspeciesForestales(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;

            List<Tbl_Sol_Finca_PinpepOld_EspeciesForestales> tbl_Sol_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales
                                                                                                           where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesForestales = new List<Tbl_Sol_Finca_PinpepOld_EspeciesForestales>();
            }


            return View(tbl_Sol_Finca_PinpepOld_EspeciesForestales);
        }

        public ActionResult PinpepOld_EspeciesForestales_Agregar(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;


            Tbl_Sol_Finca tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                           where d.Solicitud_id == Solicitud_id
                                           select d).FirstOrDefault();

            long fincaid = 0;
            if (tbl_Sol_Finca != null)
            {
                fincaid = tbl_Sol_Finca.Finca_Id;
            }

            Tbl_Sol_Finca_PinpepOld_EspeciesForestales tbl_Sol_Finca_PinpepOld_EspeciesForestales = new Tbl_Sol_Finca_PinpepOld_EspeciesForestales()
            {
                Solicitud_id = tbl_sol_solicitud.Solicitud_id,
                Finca_id = fincaid
            };


            return View(tbl_Sol_Finca_PinpepOld_EspeciesForestales);
        }

        [HttpPost]
        public ActionResult PinpepOld_EspeciesForestales_Agregar(Tbl_Sol_Finca_PinpepOld_EspeciesForestales model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            long countEspeciesForestales = 0;
            try
            {
                countEspeciesForestales = db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(Obj => Obj.Correlativo_id);
            }
            catch {
                countEspeciesForestales = 0;
            }
            countEspeciesForestales++;
            model.Correlativo_id = countEspeciesForestales;
            model.RegistroExterno = false;
            if (ModelState.IsValid)
            {
                db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales.Add(model);
                db.SaveChanges();
                return RedirectToAction("RegistroAgregado", "Home");

            }

            return View(model);
        }

        public ActionResult PinpepOld_EspeciesForestales_Borrar(Tbl_Sol_Finca_PinpepOld_EspeciesForestales model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            Tbl_Sol_Finca_PinpepOld_EspeciesForestales tbl_Sol_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales
                                                                                                     where d.Solicitud_id == model.Solicitud_id
                                                                                                     && d.Finca_id == model.Finca_id
                                                                                                     && d.Rodal_id == model.Rodal_id
                                                                                                     && d.Correlativo_id == model.Correlativo_id
                                                                                                     select d).FirstOrDefault();

            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales != null)
            {
                db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales.Remove(tbl_Sol_Finca_PinpepOld_EspeciesForestales);
                db.SaveChanges();
                return RedirectToAction("RegistroEliminado", "Home");

            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

        public ActionResult PinpepOld_EspeciesProteger(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;

            List<Tbl_Sol_Finca_PinpepOld_EspeciesProteger> tbl_Sol_Finca_PinpepOld_EspeciesProteger = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesProteger
                                                                                                       where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                                                                       orderby d.Rodal_id
                                                                                                       select d).ToList();

            if (tbl_Sol_Finca_PinpepOld_EspeciesProteger == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesProteger = new List<Tbl_Sol_Finca_PinpepOld_EspeciesProteger>();
            }


            return View(tbl_Sol_Finca_PinpepOld_EspeciesProteger);
        }

        public ActionResult PinpepOld_EspeciesProteger_Agregar(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.Solicitud_id = Solicitud_id;


            Tbl_Sol_Finca tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                           where d.Solicitud_id == Solicitud_id
                                           select d).FirstOrDefault();

            long fincaid = 0;
            if (tbl_Sol_Finca != null)
            {
                fincaid = tbl_Sol_Finca.Finca_Id;
            }

            Tbl_Sol_Finca_PinpepOld_EspeciesProteger tbl_Sol_Finca_PinpepOld_EspeciesProteger = new Tbl_Sol_Finca_PinpepOld_EspeciesProteger()
            {
                Solicitud_id = tbl_sol_solicitud.Solicitud_id
            };


            return View(tbl_Sol_Finca_PinpepOld_EspeciesProteger);
        }

        [HttpPost]
        public ActionResult PinpepOld_EspeciesProteger_Agregar(Tbl_Sol_Finca_PinpepOld_EspeciesProteger model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            model.Area = model.Area ?? 0;
            model.EspeciesProteger = model.EspeciesProteger ?? "spp";
            model.RegistroExterno = false;

            if (ModelState.IsValid)
            {
                db.Tbl_Sol_Finca_PinpepOld_EspeciesProteger.Add(model);
                db.SaveChanges();
                return RedirectToAction("RegistroAgregado", "Home");

            }

            return View(model);
        }

        public ActionResult PinpepOld_EspeciesProteger_Borrar(Tbl_Sol_Finca_PinpepOld_EspeciesProteger model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            Tbl_Sol_Finca_PinpepOld_EspeciesProteger tbl_Sol_Finca_PinpepOld_EspeciesProteger = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesProteger
                                                                                                 where d.Solicitud_id == model.Solicitud_id
                                                                                                   && d.Rodal_id == model.Rodal_id
                                                                                                 select d).FirstOrDefault();

            if (tbl_Sol_Finca_PinpepOld_EspeciesProteger != null)
            {
                db.Tbl_Sol_Finca_PinpepOld_EspeciesProteger.Remove(tbl_Sol_Finca_PinpepOld_EspeciesProteger);
                db.SaveChanges();
                return RedirectToAction("RegistroEliminado", "Home");

            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

        public ActionResult Secorf_EspeciesForestales(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            List<Tbl_Sol_Finca_Secorf_EspeciesForestales> tbl_Sol_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Secorf_EspeciesForestales
                                                                                                     where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                                                                     orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.Correlativo_id
                                                                                                     select d).ToList();

            if (tbl_Sol_Finca_Secorf_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Secorf_EspeciesForestales = new List<Tbl_Sol_Finca_Secorf_EspeciesForestales>();
            }

            return View(tbl_Sol_Finca_Secorf_EspeciesForestales);
        }

        public ActionResult Secorf_Rodal(long Solicitud_id, string firma)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            List<Tbl_Sol_Rodal> tbl_Sol_Rodals = (from d in db.Tbl_Sol_Rodal
                                                  where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                  orderby d.Finca_id, d.Rodal_Id, d.Tipo_de_Area
                                                  select d).ToList();

            if (tbl_Sol_Rodals == null)
            {
                tbl_Sol_Rodals = new List<Tbl_Sol_Rodal>();
            }

            return View(tbl_Sol_Rodals);
        }

        public ActionResult Secorf_Editar(long Solicitud_id, long Finca_id, long Rodal_id, int Tipo)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Rodal tbl_Sol_Rodal = (from d in db.Tbl_Sol_Rodal
                                           where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                           && d.Finca_id == Finca_id
                                           && d.Rodal_Id == Rodal_id
                                           && d.Tipo_de_Area == Tipo
                                           select d).FirstOrDefault();

            if (tbl_Sol_Rodal == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            ViewBag.Secorf_TipoRegistro_id = new SelectList(db.Tbl_Gral_Secorf_TipoRegistro, "Secorf_TipoRegistro_id", "Descripcion", (tbl_Sol_Rodal.Secorf_TipoRegistro_id ?? 0));

            return View(tbl_Sol_Rodal);
        }
        public JsonResult Secorf_EditarRodal(Tbl_Sol_Rodal model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool swcreatedbyinterno = false;
            if (objUs.EsInterno == 1)
            {
                swcreatedbyinterno = true;
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Número de solicitud no existe"
                };
                return Json(jsonRespuesta);
            }


            Tbl_Sol_Rodal tbl_Sol_Rodal = (from d in db.Tbl_Sol_Rodal
                                           where d.Solicitud_id == model.Solicitud_id
                                           && d.Finca_id == model.Finca_id
                                           && d.Rodal_Id == model.Rodal_Id
                                           && d.Tipo_de_Area == model.Tipo_de_Area
                                           select d).FirstOrDefault();

            if (tbl_Sol_Rodal == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 3,
                    Mensaje = "Rodal o tipo de área no existe"
                };
                return Json(jsonRespuesta);
            }

            tbl_Sol_Rodal.Secorf_TipoRegistro_id = model.Secorf_TipoRegistro_id;
            tbl_Sol_Rodal.swdateupdated = DateTime.Now;
            tbl_Sol_Rodal.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_Rodal.swupdatedbyinterno = swcreatedbyinterno;

            try
            {
                db.Entry(tbl_Sol_Rodal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Registro actualizado"
                };
            }
            catch (Exception ex)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 4,
                    Mensaje = "Error: " + ex.Message
                };
            }
            return Json(jsonRespuesta);
        }
        public ActionResult Secorf_EspeciesForestales_Create(long Solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            ViewBag.Finca_id = new SelectList(db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id), "Finca_Id", "Finca_Id");

            ViewBag.Rodal_id = new SelectList(db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == -5), "Rodal_id", "Rodal_id");

            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 100), "Tipo_de_Area", "Descripcion", 1);


            Tbl_Sol_Finca_Secorf_EspeciesForestales tbl_Sol_Finca_Secorf_EspeciesForestales = new Tbl_Sol_Finca_Secorf_EspeciesForestales()
            {
                Solicitud_id = tbl_sol_solicitud.Solicitud_id
            };


            return View(tbl_Sol_Finca_Secorf_EspeciesForestales);
        }
        public ActionResult Secorf_EspeciesForestales_Agregar(long Solicitud_id, string firma, long Finca_id, long Rodal_id, int Tipo_de_Area)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            Tbl_Sol_Finca tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                           where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id && d.Finca_Id == Finca_id
                                           select d).FirstOrDefault();

            if (tbl_Sol_Finca == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Rodal tbl_Sol_Rodal = (from d in db.Tbl_Sol_Rodal
                                           where d.Solicitud_id == tbl_Sol_Finca.Solicitud_id
                                           && d.Finca_id == tbl_Sol_Finca.Finca_Id
                                           && d.Tipo_de_Area == Tipo_de_Area
                                           && d.Rodal_Id == Rodal_id
                                           select d).FirstOrDefault();

            if (tbl_Sol_Rodal == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Finca_Secorf_EspeciesForestales tbl_Sol_Finca_Secorf_EspeciesForestales = new Tbl_Sol_Finca_Secorf_EspeciesForestales()
            {
                Solicitud_id = tbl_Sol_Rodal.Solicitud_id,
                Finca_id = tbl_Sol_Rodal.Finca_id,
                Rodal_id = tbl_Sol_Rodal.Rodal_Id,
                Tipo_de_Area = tbl_Sol_Rodal.Tipo_de_Area,
                Area_ha = tbl_Sol_Rodal.AreaTotalCalculadaSistema
            };


            return View(tbl_Sol_Finca_Secorf_EspeciesForestales);
        }

        [HttpPost]
        public ActionResult Secorf_EspeciesForestales_Agregar(Tbl_Sol_Finca_Secorf_EspeciesForestales model)
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

            bool swcreatedbyinterno = false;

            if (objUs.EsInterno == 1)
            {
                swcreatedbyinterno = true;
            }



            if (ModelState.IsValid)
            {

                int correlativoid = 0;
                try
                {
                    correlativoid = db.Tbl_Sol_Finca_Secorf_EspeciesForestales.Where(Obj => Obj.Solicitud_id == model.Solicitud_id && Obj.Finca_id == model.Finca_id && Obj.Tipo_de_Area == model.Tipo_de_Area && Obj.Rodal_id == model.Rodal_id).Max(Obj => Obj.Correlativo_id);
                }
                catch (Exception ex)
                {
                    correlativoid = 0;
                }
                correlativoid++;
                model.Correlativo_id = correlativoid;
                model.swdatecreated = DateTime.Now;
                model.swcreatedbyinterno = swcreatedbyinterno;
                model.swcreatedby = objUs.intUsuario_id;

                db.Tbl_Sol_Finca_Secorf_EspeciesForestales.Add(model);
                db.SaveChanges();
                return RedirectToAction("RegistroAgregado", "Home");

            }

            return View(model);
        }

        public ActionResult Secorf_EspeciesForestales_Borrar(long Solicitud_id, string firma, long Finca_id, long Rodal_id, int Tipo_de_Area, int Correlativo_id)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            Tbl_Sol_Finca_Secorf_EspeciesForestales tbl_Sol_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Secorf_EspeciesForestales
                                                                                               where d.Solicitud_id == Solicitud_id
                                                                                               && d.Finca_id == Finca_id
                                                                                               && d.Rodal_id == Rodal_id
                                                                                               && d.Tipo_de_Area == Tipo_de_Area
                                                                                               && d.Correlativo_id == d.Correlativo_id
                                                                                               select d).FirstOrDefault();

            if (tbl_Sol_Finca_Secorf_EspeciesForestales != null)
            {
                db.Tbl_Sol_Finca_Secorf_EspeciesForestales.Remove(tbl_Sol_Finca_Secorf_EspeciesForestales);
                db.SaveChanges();
                return RedirectToAction("RegistroEliminado", "Home");
            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_RepresentanteLegalController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();


        public ActionResult IndexMandatario(long id, string firma)
        {


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            //int cantidad = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.RepresentanteLegatTipo_id == 1 &&  Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).Count();

            //if (cantidad > 0)
            //{
            //    Tbl_Sol_RepresentanteLegal Tbl_Sol_RepresentanteLegalborrar = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.RepresentanteLegatTipo_id == 1 && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).First();

            //    db.Tbl_Sol_RepresentanteLegal.Remove(Tbl_Sol_RepresentanteLegalborrar);
            //    db.SaveChanges();

            //}

            var tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.RepresentanteLegatTipo_id == 1 && Obj.Estado_id == 1);
            ViewBag.solicitud_id = id;
            return View(tbl_Sol_RepresentanteLegal.ToList());
        }


        // GET: Sol_RepresentanteLegal
        public ActionResult Index(long id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            //int cantidad = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).Count();

            //if (cantidad > 0)
            //{
            //    Tbl_Sol_RepresentanteLegal Tbl_Sol_RepresentanteLegalborrar = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).First();

            //    db.Tbl_Sol_RepresentanteLegal.Remove(Tbl_Sol_RepresentanteLegalborrar);
            //    db.SaveChanges();


            //}

            int cantidadJ = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == true).Count();

            var tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id != Obj.Solicitud_id );

            if (cantidadJ > 0)
            {
                 tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == 1 && Obj.RepresentanteLegatTipo_id == 3 );

            }
            else
            {
                 tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == id && Obj.Estado_id == 1);
            }
            ViewBag.solicitud_id = id; 
            return View(tbl_Sol_RepresentanteLegal.ToList());
        }



        // GET: Sol_RepresentanteLegal/Create
        public ActionResult Create(long solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = new Tbl_Sol_RepresentanteLegal();
            ViewBag.solicitud_id = solicitud_id;
            tbl_Sol_RepresentanteLegal.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj=> Obj.Estado ==true), "DocumentoID_Tipo", "Descripcion");
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion");

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj=> Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2 ), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }


            return View(tbl_Sol_RepresentanteLegal);
        }


        // GET: Sol_RepresentanteLegal/Create
        public ActionResult CreateM(long solicitud_id, string firma)
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            ViewBag.solicitud_id = solicitud_id;

            Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = new Tbl_Sol_RepresentanteLegal();

            tbl_Sol_RepresentanteLegal.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion");
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion");

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Estado_id == true).Count();

            ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 ), "RepresentanteLegatTipo_id", "Descripcion");

            return View(tbl_Sol_RepresentanteLegal);
        }


        // POST: Sol_RepresentanteLegal/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateM(Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal, FormCollection Collection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            bool ErrorDetectado = false;

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];


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


            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_RepresentanteLegal.Solicitud_id, "Sol_RepresentanteLegal", EsInterno).FirstOrDefault();

            if (!((bool)permisos.Agregar))
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }




            tbl_Sol_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_Sol_RepresentanteLegal.Estado_id = 1;


            long lngIdt = 0;

            tbl_Sol_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;

            try
            {
                lngIdt = db.Tbl_Sol_RepresentanteLegal.Where(Propietario => Propietario.Solicitud_id == tbl_Sol_RepresentanteLegal.Solicitud_id).Max(u => u.RepresentanteLegal_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_RepresentanteLegal.RepresentanteLegal_id = lngIdt;


            //if (DiferenciaAnio(tbl_Sol_RepresentanteLegal.Fecha_Nacimiento?? DateTime.Now) < 18)
            //{
            //    TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
            //    ErrorDetectado = true;
            //}

            if (tbl_Sol_RepresentanteLegal.VigenciaIndefinida == false)
            {
                if ((tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                    ErrorDetectado = true;
                }
            }
            else
            {
                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = (tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);

                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = null;

                tbl_Sol_RepresentanteLegal.VigenciaIndefinida = true;
            }

            if ((tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_Sol_RepresentanteLegal.Add(tbl_Sol_RepresentanteLegal);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_Sol_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_Sol_RepresentanteLegal.Solicitud_id && Obj.Estado_id == true).Count();

            ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 ), "RepresentanteLegatTipo_id", "Descripcion");

            return View(tbl_Sol_RepresentanteLegal);

        }






        // POST: Sol_RepresentanteLegal/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal, FormCollection Collection)
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


            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }
            bool ErrorDetectado = false;

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_RepresentanteLegal.Solicitud_id, "Sol_RepresentanteLegal", EsInterno).FirstOrDefault();

            if (!((bool)permisos.Agregar))
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }



            tbl_Sol_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_Sol_RepresentanteLegal.Estado_id = 1;


            long lngIdt = 0;

            tbl_Sol_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;

            try
            {
                lngIdt = db.Tbl_Sol_RepresentanteLegal.Where(Propietario => Propietario.Solicitud_id == tbl_Sol_RepresentanteLegal.Solicitud_id).Max(u => u.RepresentanteLegal_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_RepresentanteLegal.RepresentanteLegal_id = lngIdt;


            //if (DiferenciaAnio(tbl_Sol_RepresentanteLegal.Fecha_Nacimiento?? DateTime.Now) < 18)
            //{
            //    TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
            //    ErrorDetectado = true;
            //}

            if (tbl_Sol_RepresentanteLegal.VigenciaIndefinida == false)
            { 
                    if ((tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                    {
                        TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                        ErrorDetectado = true;
                    }
            }
            else
            {
                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = (tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);

                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = null;

                tbl_Sol_RepresentanteLegal.VigenciaIndefinida = true;

            }

            if ((tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_Sol_RepresentanteLegal.Add(tbl_Sol_RepresentanteLegal);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_Sol_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_Sol_RepresentanteLegal.Solicitud_id && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }
            return View(tbl_Sol_RepresentanteLegal);

        }



 public int DiferenciaAnio(DateTime Fecha)
 {
            int intDias;
            TimeSpan dias = DateTime.Now.Subtract(Fecha);

            intDias = dias.Days;

            return intDias / 364;
  }


        // GET: Sol_PersonaJuridica/Create
public ActionResult RegistroAgregado()
    {
        return View();
    }

        public ActionResult Eliminar(long solicitud_id, long RepresentanteLegal_id, string firma)
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


            Tbl_Sol_RepresentanteLegal tbl_sol_representantelegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.RepresentanteLegal_id == RepresentanteLegal_id).First();


            if (tbl_sol_solicitud.Solicitud_id != tbl_sol_representantelegal.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");

            }


            tbl_sol_representantelegal.swupdatedby = objUs.intUsuario_id;
            tbl_sol_representantelegal.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_sol_representantelegal.swupdatedbyinterno = false;

            }
            else
            {
                tbl_sol_representantelegal.swupdatedbyinterno = true;

            }

            tbl_sol_representantelegal.Estado_id = 0;

  

            db.Tbl_Sol_RepresentanteLegal.Remove(tbl_sol_representantelegal);
            db.SaveChanges();

    


            return RedirectToAction("../Home/RegistroEliminado");

        }

        // GET: Sol_RepresentanteLegal/Edit/5
        public ActionResult Edit(long solicitud_id, string firma, long RepresentanteLegal_id)
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
            ViewBag.solicitud_id = solicitud_id;


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            Tbl_Sol_RepresentanteLegal tbl_sol_representantelegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.RepresentanteLegal_id == RepresentanteLegal_id).First();


            if (tbl_sol_solicitud.Solicitud_id != tbl_sol_representantelegal.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");

            }


            tbl_sol_representantelegal.swupdatedby = objUs.intUsuario_id;
            tbl_sol_representantelegal.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_sol_representantelegal.swupdatedbyinterno = false;

            }
            else
            {
                tbl_sol_representantelegal.swupdatedbyinterno = true;
                
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_sol_representantelegal.RepresententeDocumentoID_Tipo);

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3 || Obj.RepresentanteLegatTipo_id == tbl_sol_representantelegal.RepresentanteLegatTipo_id), "RepresentanteLegatTipo_id", "Descripcion");
            }

            return View(tbl_sol_representantelegal);

        }

        // POST: Sol_RepresentanteLegal/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal, FormCollection Collection)
        {
            ViewBag.solicitud_id = tbl_Sol_RepresentanteLegal.Solicitud_id;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            bool ErrorDetectado = false;

            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            //if (objGrant.boolEditarRepresentante == false)
            //{
            //    TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
            //    ErrorDetectado = true;
            //}


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

            bool EsInterno = false;

            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_RepresentanteLegal.Solicitud_id, "Sol_RepresentanteLegal", EsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }


            tbl_Sol_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_Sol_RepresentanteLegal.Estado_id = 1;

            tbl_Sol_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;


            if (tbl_Sol_RepresentanteLegal.VigenciaIndefinida == false)
            {
                if ((tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                    ErrorDetectado = true;
                }
            }
            else
            {
                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = (tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);

                tbl_Sol_RepresentanteLegal.Fecha_FinNombramiento = null;

                tbl_Sol_RepresentanteLegal.VigenciaIndefinida = true;
            }

            if ((tbl_Sol_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {

                db.Entry(tbl_Sol_RepresentanteLegal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_Sol_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_Sol_RepresentanteLegal.Solicitud_id && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }

            return View(tbl_Sol_RepresentanteLegal);
        }

        //ViewBag.solicitud_id

        // GET: Sol_RepresentanteLegal/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Find(id);
            if (tbl_Sol_RepresentanteLegal == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Sol_RepresentanteLegal);
        }




        // POST: Sol_RepresentanteLegal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Find(id);
            db.Tbl_Sol_RepresentanteLegal.Remove(tbl_Sol_RepresentanteLegal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

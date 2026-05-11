using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_RepresentanteLegalController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        public ActionResult IndexMandatario(string Guid_id)
        {

            int cantidad = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.RepresentanteLegatTipo_id == 1 && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).Count();

            if (cantidad > 0)
            {
                Tbl_RNF_RepresentanteLegal Tbl_RNF_RepresentanteLegalborrar = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.RepresentanteLegatTipo_id == 1 && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).First();

                db.Tbl_RNF_RepresentanteLegal.Remove(Tbl_RNF_RepresentanteLegalborrar);
                db.SaveChanges();


            }

            var tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.RepresentanteLegatTipo_id == 1 && Obj.Estado_id == 1);
            ViewBag.Guid_id = Guid_id;
            return View(tbl_RNF_RepresentanteLegal.ToList());
        }

        public ActionResult Index(string Guid_id)
        {

            int cantidad = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).Count();

            if (cantidad > 0)
            {
                Tbl_RNF_RepresentanteLegal Tbl_RNF_RepresentanteLegalborrar = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.Estado_id == 1 && Obj.Fecha_InicioNombramiento == null).First();

                db.Tbl_RNF_RepresentanteLegal.Remove(Tbl_RNF_RepresentanteLegalborrar);
                db.SaveChanges();


            }

            int cantidadJ = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == Guid_id && Obj.Estado_id == true).Count();

            var tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro != Obj.No_Registro);

            if (cantidadJ > 0)
            {
                tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.Estado_id == 1 && Obj.RepresentanteLegatTipo_id == 3);

            }
            else
            {
                tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == Guid_id && Obj.Estado_id == 1);
            }
            ViewBag.Guid_id = Guid_id;
            return View(tbl_RNF_RepresentanteLegal.ToList());
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
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal = new Tbl_RNF_RepresentanteLegal();
            //Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = new Tbl_Sol_RepresentanteLegal();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            tbl_RNF_RepresentanteLegal.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_RepresentanteLegal.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_RepresentanteLegal.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_RepresentanteLegal.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion");
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion");

            int CantidadPropietarioIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }


            return View(tbl_RNF_RepresentanteLegal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal, FormCollection Collection)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            bool ErrorDetectado = false;

            if (!objRNFGrants.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }


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



            tbl_RNF_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_RNF_RepresentanteLegal.Estado_id = 1;


            long lngIdt = 0;

            tbl_RNF_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;

            try
            {
                lngIdt = db.Tbl_RNF_RepresentanteLegal.Where(Propietario => Propietario.No_Registro == tbl_RNF_RepresentanteLegal.No_Registro).Max(u => u.RepresentanteLegal_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_RNF_RepresentanteLegal.RepresentanteLegal_id = lngIdt;


            //if (DiferenciaAnio(tbl_Sol_RepresentanteLegal.Fecha_Nacimiento?? DateTime.Now) < 18)
            //{
            //    TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
            //    ErrorDetectado = true;
            //}

            if (tbl_RNF_RepresentanteLegal.VigenciaIndefinida == false)
            {
                if ((tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                    ErrorDetectado = true;
                }
            }
            else
            {
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = (tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = null;
                tbl_RNF_RepresentanteLegal.VigenciaIndefinida = true;


            }

            if ((tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_RNF_RepresentanteLegal.Add(tbl_RNF_RepresentanteLegal);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_RNF_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_RepresentanteLegal.No_Registro && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }
            return View(tbl_RNF_RepresentanteLegal);

        }

        public ActionResult CreateM(string No_Registro)
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


            Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal = new Tbl_RNF_RepresentanteLegal();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            tbl_RNF_RepresentanteLegal.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_RepresentanteLegal.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_RepresentanteLegal.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_RepresentanteLegal.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion");
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion");

            int CantidadPropietarioIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Estado_id == true).Count();

            ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1), "RepresentanteLegatTipo_id", "Descripcion");

            return View(tbl_RNF_RepresentanteLegal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateM(Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal, FormCollection Collection)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            bool ErrorDetectado = false;

            if (!objRNFGrants.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }


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



            tbl_RNF_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_RNF_RepresentanteLegal.Estado_id = 1;


            long lngIdt = 0;

            tbl_RNF_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;

            try
            {
                lngIdt = db.Tbl_RNF_RepresentanteLegal.Where(Propietario => Propietario.No_Registro == tbl_RNF_RepresentanteLegal.No_Registro).Max(u => u.RepresentanteLegal_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_RNF_RepresentanteLegal.RepresentanteLegal_id = lngIdt;


            //if (DiferenciaAnio(tbl_Sol_RepresentanteLegal.Fecha_Nacimiento?? DateTime.Now) < 18)
            //{
            //    TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
            //    ErrorDetectado = true;
            //}

            if (tbl_RNF_RepresentanteLegal.VigenciaIndefinida == false)
            {
                if ((tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                    ErrorDetectado = true;
                }
            }
            else
            {
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = (tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = null;
                tbl_RNF_RepresentanteLegal.VigenciaIndefinida = true;


            }


            if ((tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_RNF_RepresentanteLegal.Add(tbl_RNF_RepresentanteLegal);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_RNF_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_RepresentanteLegal.No_Registro && Obj.Estado_id == true).Count();

            ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1), "RepresentanteLegatTipo_id", "Descripcion");

            return View(tbl_RNF_RepresentanteLegal);

        }

        public ActionResult Eliminar(string No_Registro, long RepresentanteLegal_id)
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

            Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == No_Registro && Obj.RepresentanteLegal_id == RepresentanteLegal_id).FirstOrDefault();
            //Tbl_Sol_RepresentanteLegal tbl_sol_representantelegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.RepresentanteLegal_id == RepresentanteLegal_id).First();


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_RepresentanteLegal.No_Registro)
            {
                return RedirectToAction("../Login/Index");

            }


            tbl_RNF_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_RepresentanteLegal.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_RepresentanteLegal.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_RepresentanteLegal.swupdatedbyinterno = true;

            }

            tbl_RNF_RepresentanteLegal.Estado_id = 0;

            db.Entry(tbl_RNF_RepresentanteLegal).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("../Home/RegistroEliminado");

        }

        public ActionResult Edit(string No_Registro, long RepresentanteLegal_id)
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

            Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == No_Registro && Obj.RepresentanteLegal_id == RepresentanteLegal_id).FirstOrDefault();
            //Tbl_Sol_RepresentanteLegal tbl_sol_representantelegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.RepresentanteLegal_id == RepresentanteLegal_id).First();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_RepresentanteLegal.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }


            tbl_RNF_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_RepresentanteLegal.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_RepresentanteLegal.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_RepresentanteLegal.swupdatedbyinterno = true;

            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_RepresentanteLegal.RepresententeDocumentoID_Tipo);

            int CantidadPropietarioIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3 || Obj.RepresentanteLegatTipo_id == tbl_RNF_RepresentanteLegal.RepresentanteLegatTipo_id), "RepresentanteLegatTipo_id", "Descripcion");
            }

            return View(tbl_RNF_RepresentanteLegal);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_RNF_RepresentanteLegal tbl_RNF_RepresentanteLegal, FormCollection Collection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            bool ErrorDetectado = false;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del representante.";
                ErrorDetectado = true;
            }


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



            tbl_RNF_RepresentanteLegal.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_RepresentanteLegal.swdateupdated = DateTime.Now;

            tbl_RNF_RepresentanteLegal.Estado_id = 1;

            tbl_RNF_RepresentanteLegal.VigenciaIndefinida = (Collection["ChkVigenciaIndefinida"] == null) ? false : true;


            if (tbl_RNF_RepresentanteLegal.VigenciaIndefinida == false)
            {
                if ((tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento ?? DateTime.Now) <= DateTime.Now)
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de fin de nombramiento debe ser mayor a la fecha actual.";
                    ErrorDetectado = true;
                }
            }
            else
            {
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = (tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now).AddYears(10);
                tbl_RNF_RepresentanteLegal.Fecha_FinNombramiento = null;

                tbl_RNF_RepresentanteLegal.VigenciaIndefinida = true;
            }

            if ((tbl_RNF_RepresentanteLegal.Fecha_InicioNombramiento ?? DateTime.Now) >= DateTime.Now)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "La fecha de inicio de nombramiento debe ser menor a la fecha actual.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {

                db.Entry(tbl_RNF_RepresentanteLegal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");
            }

            ViewBag.RepresententeDocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_RepresentanteLegal.RepresententeDocumentoID_Tipo);
            ViewBag.Estado_id = new SelectList(db.Tbl_Sol_RepresentanteLegal_Estado, "Estado_id", "Descripcion", tbl_RNF_RepresentanteLegal.Estado_id);

            int CantidadPropietarioIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_RNF_RepresentanteLegal.Solicitud_id && Obj.Estado_id == true).Count();

            if (CantidadPropietarioIndividual > 0)
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 1 || Obj.RepresentanteLegatTipo_id == 2), "RepresentanteLegatTipo_id", "Descripcion");
            }
            else
            {
                ViewBag.RepresentanteLegatTipo_id = new SelectList(db.Tbl_Sol_RepresentanteLegalTipo.Where(Obj => Obj.RepresentanteLegatTipo_id == 3), "RepresentanteLegatTipo_id", "Descripcion");
            }

            return View(tbl_RNF_RepresentanteLegal);
        }



    }
}
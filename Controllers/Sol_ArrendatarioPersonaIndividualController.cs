using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_ArrendatarioPersonaIndividualController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_ArrendatarioPersonaIndividual
        public ActionResult Index()
        {
            return View(db.Tbl_Sol_ArrendatarioPersonaIndividual.ToList());
        }

        public ActionResult Create(long solicitud_id, string firma)
        {

            ViewBag.solicitud_id = solicitud_id;

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



            Tbl_Sol_ArrendatarioPersonaIndividual tbl_Sol_PersonaIndividual = new Tbl_Sol_ArrendatarioPersonaIndividual();


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Propietario => Propietario.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(u => u.PersonaIndividual_id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }


            tbl_Sol_PersonaIndividual.Solicitud_id = tbl_sol_solicitud.Solicitud_id;
            tbl_Sol_PersonaIndividual.PersonaIndividual_id = lngIdt;



            tbl_Sol_PersonaIndividual.Fecha_Nacimiento = DateTime.Now;

            tbl_Sol_PersonaIndividual.swdatecreated = DateTime.Now;
            tbl_Sol_PersonaIndividual.swcreatedbyinterno = (objUs.EsInterno == 1);

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaIndividual.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaIndividual.swcreatedbyinterno = true;

            }




            tbl_Sol_PersonaIndividual.swcreatedby = objUs.intUsuario_id;

            tbl_Sol_PersonaIndividual.DepartamentoDPI_id = 7;
            tbl_Sol_PersonaIndividual.MunicipioDPI_id = 74;
            tbl_Sol_PersonaIndividual.DocumentoID_Tipo = 0;

            tbl_Sol_PersonaIndividual.PuebloPertenencia_id = 0;
            tbl_Sol_PersonaIndividual.Sexo_id = 0;



            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_PersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_PersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Sol_PersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Sol_PersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Sol_PersonaIndividual.Sexo_id);

            return View(tbl_Sol_PersonaIndividual);
        }

        public ActionResult Eliminar(long solicitud_id, long personaIndividual_id, string firma)
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


            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Arrendatario", boolEsInterno).FirstOrDefault();
            if (!(bool)permisos.Borrar)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Sol_ArrendatarioPersonaIndividual tbl_Sol_PersonaIndividual = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaIndividual_id == personaIndividual_id).First();



            if (tbl_sol_solicitud.Solicitud_id != tbl_Sol_PersonaIndividual.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");
            }


            tbl_Sol_PersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PersonaIndividual.swdateupdated = DateTime.Now;





            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaIndividual.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaIndividual.swupdatedbyinterno = true;

            }

            tbl_Sol_PersonaIndividual.Estado_id = false;

      

            db.Tbl_Sol_ArrendatarioPersonaIndividual.Remove(tbl_Sol_PersonaIndividual);
            db.SaveChanges();


            return RedirectToAction("../Home/RegistroEliminado");

        }



        public ActionResult Edit(long solicitud_id, long personaIndividual_id, string firma)
        {

            ViewBag.solicitud_id = solicitud_id;

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


            Tbl_Sol_ArrendatarioPersonaIndividual tbl_Sol_PersonaIndividual = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaIndividual_id == personaIndividual_id).First();

            Tbl_Seg_UsuarioExterno tbl_seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.Usuario_id == objUs.intUsuario_id && objUs.EsInterno == 0).FirstOrDefault();

            //No se permite que el usuario modifique sus datos desde aca.

            if (tbl_seg_UsuarioExterno != null)
            {
                if (tbl_seg_UsuarioExterno.No_Documento == tbl_Sol_PersonaIndividual.No_Documento)
                {
                    return RedirectToAction("../Home/EdicionPropietarioDenegada");

                }

            }


            if (tbl_sol_solicitud.Solicitud_id != tbl_Sol_PersonaIndividual.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");
            }

            tbl_Sol_PersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PersonaIndividual.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaIndividual.swupdatedbyinterno = false;
            }
            else
            {
                tbl_Sol_PersonaIndividual.swupdatedbyinterno = true;
            }

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_PersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_PersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Sol_PersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Sol_PersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Sol_PersonaIndividual.Sexo_id);

            return View(tbl_Sol_PersonaIndividual);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_Sol_ArrendatarioPersonaIndividual tbl_Sol_PersonaIndividual)
        {
            bool ErrorDetectado = false;
            ViewBag.solicitud_id = tbl_Sol_PersonaIndividual.Solicitud_id;


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
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_PersonaIndividual.Solicitud_id, "Sol_Arrendatario", boolEsInterno).FirstOrDefault();
            if (!(bool)permisos.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
                ErrorDetectado = true;
            }

            tbl_Sol_PersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PersonaIndividual.swdateupdated = DateTime.Now;

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_PersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_PersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Sol_PersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Sol_PersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Sol_PersonaIndividual.Sexo_id);
            tbl_Sol_PersonaIndividual.Estado_id = true;

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaIndividual.swcreatedbyinterno = true;

            }


            if ((!No_NIT_Valido(tbl_Sol_PersonaIndividual.No_NIT)) && (tbl_Sol_PersonaIndividual.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_Sol_PersonaIndividual.No_Documento)) && (tbl_Sol_PersonaIndividual.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_Sol_PersonaIndividual.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_Sol_PersonaIndividual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");

            }
            return View(tbl_Sol_PersonaIndividual);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_ArrendatarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual)
        {
            TempData["Mensaje"] = "";
            ViewBag.solicitud_id = tbl_Sol_PropietarioPersonaIndividual.Solicitud_id;

            bool ErrorDetectado = false;

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            int cantidad = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_Sol_PropietarioPersonaIndividual.Solicitud_id && Obj.No_Documento == tbl_Sol_PropietarioPersonaIndividual.No_Documento && Obj.Estado_id == true).Count();

            if (cantidad > 0)
            {
                TempData["Mensaje"] = "Error: Este propietario ya ha sido registrado previamente. No puede volverlo a asociar a esta solicitud.";
                ErrorDetectado = true;
            }


            if (tbl_Sol_PropietarioPersonaIndividual.No_NIT != null)
            {
                cantidad = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_Sol_PropietarioPersonaIndividual.Solicitud_id && Obj.No_NIT == tbl_Sol_PropietarioPersonaIndividual.No_NIT && Obj.Estado_id == true).Count();

                if (cantidad > 0)
                {
                    TempData["Mensaje"] = "Error: Este propietario ya ha sido registrado previamente. No puede volverlo a asociar a esta solicitud.";
                    ErrorDetectado = true;
                }

            }



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






            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_Solicitud.Solicitud_id, "Sol_Arrendatario", boolEsInterno).FirstOrDefault();
            if (!(bool)permisos.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
                ErrorDetectado = true;
            }

            tbl_Sol_PropietarioPersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PropietarioPersonaIndividual.swdateupdated = DateTime.Now;

            tbl_Sol_PropietarioPersonaIndividual.Estado_id = true;

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(Propietario => Propietario.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).Max(u => u.PersonaIndividual_id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_PropietarioPersonaIndividual.PersonaIndividual_id = lngIdt;


            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PropietarioPersonaIndividual.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PropietarioPersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PropietarioPersonaIndividual.swcreatedbyinterno = true;

            }

            if ((!No_NIT_Valido(tbl_Sol_PropietarioPersonaIndividual.No_NIT)) && (tbl_Sol_PropietarioPersonaIndividual.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_Sol_PropietarioPersonaIndividual.No_Documento)) && (tbl_Sol_PropietarioPersonaIndividual.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_Sol_PropietarioPersonaIndividual.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }


            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_Sol_ArrendatarioPersonaIndividual.Add(tbl_Sol_PropietarioPersonaIndividual);
                db.SaveChanges();

                return RedirectToAction("../Home/RegistroAgregado");

            }


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PropietarioPersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_Sol_PropietarioPersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_PropietarioPersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_Sol_PropietarioPersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_Sol_PropietarioPersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_Sol_PropietarioPersonaIndividual.Sexo_id);

            return View(tbl_Sol_PropietarioPersonaIndividual);


        }




        public int DiferenciaAnio(DateTime Fecha)
        {
            int intDias;
            TimeSpan dias = DateTime.Now.Subtract(Fecha);

            intDias = dias.Days;

            return intDias / 364;
        }

        public bool No_NIT_Valido(string No_NIT)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_NIT_Valido(@p0)", No_NIT).FirstOrDefault(); ;
        }

        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }

        public ActionResult RegistroAgregado()
        {
            return View();
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
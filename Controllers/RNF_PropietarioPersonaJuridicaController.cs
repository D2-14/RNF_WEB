using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class RNF_PropietarioPersonaJuridicaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_PropietarioPersonaJuridica
        public ActionResult Index()
        {
            return View();
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


            Tbl_RNF_PropietarioPersonaJuridica tbl_RNF_PropietarioPersonaJuridica = new Tbl_RNF_PropietarioPersonaJuridica();
            //Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = new Tbl_Sol_PropietarioPersonaJuridica();


            tbl_RNF_PropietarioPersonaJuridica.swdatecreated = DateTime.Now;
            tbl_RNF_PropietarioPersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);


            tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id = 7;
            tbl_RNF_PropietarioPersonaJuridica.MunicipioEmpresa_id = 74;
            tbl_RNF_PropietarioPersonaJuridica.PersonaJuridicaTipo_Id = 0;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            tbl_RNF_PropietarioPersonaJuridica.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_PropietarioPersonaJuridica.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_PropietarioPersonaJuridica.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_PropietarioPersonaJuridica.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            // tbl_Sol_PersonaJuridica = db.Tbl_Sol_PersonaJuridica.Find(1);


            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_PropietarioPersonaJuridica.MunicipioEmpresa_id);


            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_PropietarioPersonaJuridica.PersonaJuridicaTipo_Id);




            tbl_RNF_PropietarioPersonaJuridica.swdatecreated = DateTime.Now;
            tbl_RNF_PropietarioPersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_PropietarioPersonaJuridica.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_PropietarioPersonaJuridica.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_PropietarioPersonaJuridica.swcreatedbyinterno = true;

            }




            return View(tbl_RNF_PropietarioPersonaJuridica);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_RNF_PropietarioPersonaJuridica tbl_RNF_PropietarioPersonaJuridica)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            TempData["Mensaje"] = "";

            bool ErrorDetectado = false;

            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            int cantidad = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == tbl_RNF_PropietarioPersonaJuridica.No_Registro && Obj.No_NIT == tbl_RNF_PropietarioPersonaJuridica.No_NIT && Obj.Estado_id == true).Count();

            if (cantidad > 0)
            {
                TempData["Mensaje"] = "Error: El propietario jurídico ya ha sido ingresado en esta solicitud. No puede agregarlo nuevamente.";
                ErrorDetectado = true;
            }



            if (!objRNFGrants.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
                ErrorDetectado = true;
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


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_PropietarioPersonaJuridica.No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);



            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Propietario => Propietario.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.PersonaJuridica_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }


            tbl_RNF_PropietarioPersonaJuridica.PersonaJuridica_id = lngIdt;
            tbl_RNF_PropietarioPersonaJuridica.Estado_id = true;

            if (!No_NIT_Valido(tbl_RNF_PropietarioPersonaJuridica.No_NIT))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_RNF_PropietarioPersonaJuridica.Add(tbl_RNF_PropietarioPersonaJuridica);
                db.SaveChanges();

                return RedirectToAction("../Sol_PropietarioPersonaIndividual/RegistroAgregado");

            }


            TempData["Mensaje"] = TempData["Mensaje"] + " Error detectado";

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_PropietarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_PropietarioPersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_RNF_PropietarioPersonaJuridica);


        }
        public ActionResult Edit(string No_Registro, long personaJuridica_id)
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


            Tbl_RNF_PropietarioPersonaJuridica tbl_RNF_PropietarioPersonaJuridica = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaJuridica_id == personaJuridica_id).FirstOrDefault();
            //Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaJuridica_id == personaJuridica_id).First();


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_PropietarioPersonaJuridica.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }





            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_PropietarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_PropietarioPersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_RNF_PropietarioPersonaJuridica);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_RNF_PropietarioPersonaJuridica tbl_RNF_PropietarioPersonaJuridica)
        {

            bool ErrorDetectado = false;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
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

            tbl_RNF_PropietarioPersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_PropietarioPersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_PropietarioPersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_PropietarioPersonaJuridica.swupdatedbyinterno = true;

            }

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_PropietarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_PropietarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_PropietarioPersonaJuridica.PersonaJuridicaTipo_Id);



            if (!No_NIT_Valido(tbl_RNF_PropietarioPersonaJuridica.No_NIT))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_RNF_PropietarioPersonaJuridica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");

            }
            return View(tbl_RNF_PropietarioPersonaJuridica);
        }

        public ActionResult Eliminar(string No_Registro, long personaJuridica_id)
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


            Tbl_RNF_PropietarioPersonaJuridica tbl_RNF_PropietarioPersonaJuridica = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaJuridica_id == personaJuridica_id).FirstOrDefault();
            //Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaJuridica_id == personaJuridica_id).First();


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_PropietarioPersonaJuridica.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }



            tbl_RNF_PropietarioPersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_PropietarioPersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_PropietarioPersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_PropietarioPersonaJuridica.swupdatedbyinterno = true;

            }

            tbl_RNF_PropietarioPersonaJuridica.Estado_id = false;



            db.Tbl_RNF_PropietarioPersonaJuridica.Remove(tbl_RNF_PropietarioPersonaJuridica);
            db.SaveChanges();


            return RedirectToAction("../Home/RegistroEliminado");

        }




        public bool No_NIT_Valido(string No_NIT)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_NIT_Valido(@p0)", No_NIT).FirstOrDefault(); ;
        }
        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }
    }
}
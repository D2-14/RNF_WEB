using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class RNF_ArrendatarioPersonaJuridicaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
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


            Tbl_RNF_ArrendatarioPersonaJuridica tbl_RNF_ArrendatarioPersonaJuridica = new Tbl_RNF_ArrendatarioPersonaJuridica();


            tbl_RNF_ArrendatarioPersonaJuridica.swdatecreated = DateTime.Now;
            tbl_RNF_ArrendatarioPersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);


            tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id = 7;
            tbl_RNF_ArrendatarioPersonaJuridica.MunicipioEmpresa_id = 74;
            tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridicaTipo_Id = 0;


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();


            tbl_RNF_ArrendatarioPersonaJuridica.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_ArrendatarioPersonaJuridica.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_ArrendatarioPersonaJuridica.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_ArrendatarioPersonaJuridica.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            // tbl_Sol_PersonaJuridica = db.Tbl_Sol_PersonaJuridica.Find(1);


            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaJuridica.MunicipioEmpresa_id);


            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridicaTipo_Id);




            tbl_RNF_ArrendatarioPersonaJuridica.swdatecreated = DateTime.Now;
            tbl_RNF_ArrendatarioPersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaJuridica.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swcreatedbyinterno = true;

            }




            return View(tbl_RNF_ArrendatarioPersonaJuridica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_RNF_ArrendatarioPersonaJuridica tbl_RNF_ArrendatarioPersonaJuridica)
        {

            TempData["Mensaje"] = "";

            bool ErrorDetectado = false;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            int cantidad = db.Tbl_RNF_ArrendatarioPersonaJuridica.Where(Obj => Obj.No_Registro == tbl_RNF_ArrendatarioPersonaJuridica.No_Registro && Obj.No_NIT == tbl_RNF_ArrendatarioPersonaJuridica.No_NIT && Obj.Estado_id == true).Count();

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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_ArrendatarioPersonaJuridica.No_Registro).FirstOrDefault();


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_ArrendatarioPersonaJuridica.Where(Propietario => Propietario.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.PersonaJuridica_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }


            tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridica_id = lngIdt;
            tbl_RNF_ArrendatarioPersonaJuridica.Estado_id = true;

            if (!No_NIT_Valido(tbl_RNF_ArrendatarioPersonaJuridica.No_NIT))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_RNF_ArrendatarioPersonaJuridica.Add(tbl_RNF_ArrendatarioPersonaJuridica);
                db.SaveChanges();

                return RedirectToAction("../Sol_ArrendatarioPersonaIndividual/RegistroAgregado");

            }


            TempData["Mensaje"] = TempData["Mensaje"] + " Error detectado";

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_RNF_ArrendatarioPersonaJuridica);


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

            Tbl_RNF_ArrendatarioPersonaJuridica tbl_RNF_ArrendatarioPersonaJuridica = db.Tbl_RNF_ArrendatarioPersonaJuridica.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaJuridica_id == personaJuridica_id).FirstOrDefault();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_ArrendatarioPersonaJuridica.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }



            tbl_RNF_ArrendatarioPersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swupdatedbyinterno = true;

            }

            tbl_RNF_ArrendatarioPersonaJuridica.Estado_id = false;

            db.Entry(tbl_RNF_ArrendatarioPersonaJuridica).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("../Home/RegistroEliminado");

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

            Tbl_RNF_ArrendatarioPersonaJuridica tbl_RNF_ArrendatarioPersonaJuridica = db.Tbl_RNF_ArrendatarioPersonaJuridica.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaJuridica_id == personaJuridica_id).FirstOrDefault();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_ArrendatarioPersonaJuridica.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }





            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_RNF_ArrendatarioPersonaJuridica);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_RNF_ArrendatarioPersonaJuridica tbl_RNF_ArrendatarioPersonaJuridica)
        {

            bool ErrorDetectado = false;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            tbl_RNF_ArrendatarioPersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaJuridica.swupdatedbyinterno = true;

            }

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_RNF_ArrendatarioPersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_RNF_ArrendatarioPersonaJuridica.PersonaJuridicaTipo_Id);



            if (!No_NIT_Valido(tbl_RNF_ArrendatarioPersonaJuridica.No_NIT))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_RNF_ArrendatarioPersonaJuridica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");

            }
            return View(tbl_RNF_ArrendatarioPersonaJuridica);
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

    }
}
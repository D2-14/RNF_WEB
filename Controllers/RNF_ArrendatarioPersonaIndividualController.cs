using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_ArrendatarioPersonaIndividualController : Controller
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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_ArrendatarioPersonaIndividual tbl_RNF_ArrendatarioPersonaIndividual = new Tbl_RNF_ArrendatarioPersonaIndividual();


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Propietario => Propietario.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.PersonaIndividual_id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }


            tbl_RNF_ArrendatarioPersonaIndividual.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_ArrendatarioPersonaIndividual.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_ArrendatarioPersonaIndividual.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_ArrendatarioPersonaIndividual.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
            tbl_RNF_ArrendatarioPersonaIndividual.PersonaIndividual_id = lngIdt;



            tbl_RNF_ArrendatarioPersonaIndividual.Fecha_Nacimiento = DateTime.Now;

            tbl_RNF_ArrendatarioPersonaIndividual.swdatecreated = DateTime.Now;
            tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = (objUs.EsInterno == 1);

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = true;

            }




            tbl_RNF_ArrendatarioPersonaIndividual.swcreatedby = objUs.intUsuario_id;

            tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id = 7;
            tbl_RNF_ArrendatarioPersonaIndividual.MunicipioDPI_id = 74;
            tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo = 0;

            tbl_RNF_ArrendatarioPersonaIndividual.PuebloPertenencia_id = 0;
            tbl_RNF_ArrendatarioPersonaIndividual.Sexo_id = 0;



            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.Sexo_id);

            return View(tbl_RNF_ArrendatarioPersonaIndividual);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_RNF_ArrendatarioPersonaIndividual tbl_RNF_ArrendatarioPersonaIndividual)
        {
            TempData["Mensaje"] = "";

            bool ErrorDetectado = false;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            int cantidad = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_ArrendatarioPersonaIndividual.No_Registro && Obj.No_Documento == tbl_RNF_ArrendatarioPersonaIndividual.No_Documento && Obj.Estado_id == true).Count();

            if (cantidad > 0)
            {
                TempData["Mensaje"] = "Error: Este propietario ya ha sido registrado previamente. No puede volverlo a asociar a esta solicitud.";
                ErrorDetectado = true;
            }


            if (tbl_RNF_ArrendatarioPersonaIndividual.No_NIT != null)
            {
                cantidad = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Obj => Obj.No_Registro == tbl_RNF_ArrendatarioPersonaIndividual.No_Registro && Obj.No_NIT == tbl_RNF_ArrendatarioPersonaIndividual.No_NIT && Obj.Estado_id == true).Count();

                if (cantidad > 0)
                {
                    TempData["Mensaje"] = "Error: Este propietario ya ha sido registrado previamente. No puede volverlo a asociar a esta solicitud.";
                    ErrorDetectado = true;
                }

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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_ArrendatarioPersonaIndividual.No_Registro).FirstOrDefault();

            tbl_RNF_ArrendatarioPersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaIndividual.swdateupdated = DateTime.Now;

            tbl_RNF_ArrendatarioPersonaIndividual.Estado_id = true;

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Propietario => Propietario.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.PersonaIndividual_id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }

            tbl_RNF_ArrendatarioPersonaIndividual.PersonaIndividual_id = lngIdt;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.UsuarioExterno_id = objUs.intUsuario_id;

            }

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = true;

            }

            if ((!No_NIT_Valido(tbl_RNF_ArrendatarioPersonaIndividual.No_NIT)) && (tbl_RNF_ArrendatarioPersonaIndividual.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_RNF_ArrendatarioPersonaIndividual.No_Documento)) && (tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_RNF_ArrendatarioPersonaIndividual.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }


            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_RNF_ArrendatarioPersonaIndividual.Add(tbl_RNF_ArrendatarioPersonaIndividual);
                db.SaveChanges();

                return RedirectToAction("../Home/RegistroAgregado");

            }


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.Sexo_id);

            return View(tbl_RNF_ArrendatarioPersonaIndividual);


        }

        public ActionResult Eliminar(string No_Registro, long personaIndividual_id)
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


            Tbl_RNF_ArrendatarioPersonaIndividual tbl_RNF_ArrendatarioPersonaIndividual = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaIndividual_id == personaIndividual_id).First();

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_ArrendatarioPersonaIndividual.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }


            tbl_RNF_ArrendatarioPersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaIndividual.swdateupdated = DateTime.Now;





            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swupdatedbyinterno = true;

            }

            tbl_RNF_ArrendatarioPersonaIndividual.Estado_id = false;

            db.Tbl_RNF_ArrendatarioPersonaIndividual.Remove(tbl_RNF_ArrendatarioPersonaIndividual);
            db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");

        }

        public ActionResult Edit(string No_Registro, long personaIndividual_id)
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


            Tbl_RNF_ArrendatarioPersonaIndividual tbl_RNF_ArrendatarioPersonaIndividual = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(Obj => Obj.No_Registro == No_Registro && Obj.PersonaIndividual_id == personaIndividual_id).First();

            Tbl_Seg_UsuarioExterno tbl_seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.Usuario_id == objUs.intUsuario_id && objUs.EsInterno == 0).FirstOrDefault();

            //No se permite que el usuario modifique sus datos desde aca.

            if (tbl_seg_UsuarioExterno != null)
            {
                if (tbl_seg_UsuarioExterno.No_Documento == tbl_RNF_ArrendatarioPersonaIndividual.No_Documento)
                {
                    return RedirectToAction("../Home/EdicionPropietarioDenegada");

                }

            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro.No_Registro != tbl_RNF_ArrendatarioPersonaIndividual.No_Registro)
            {
                return RedirectToAction("../Login/Index");
            }

            tbl_RNF_ArrendatarioPersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaIndividual.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swupdatedbyinterno = false;
            }
            else
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swupdatedbyinterno = true;
            }

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.Sexo_id);

            return View(tbl_RNF_ArrendatarioPersonaIndividual);
        }
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_RNF_ArrendatarioPersonaIndividual tbl_RNF_ArrendatarioPersonaIndividual)
        {
            bool ErrorDetectado = false;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
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

            tbl_RNF_ArrendatarioPersonaIndividual.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_ArrendatarioPersonaIndividual.swdateupdated = DateTime.Now;

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_ArrendatarioPersonaIndividual.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_ArrendatarioPersonaIndividual.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_ArrendatarioPersonaIndividual.Sexo_id);
            tbl_RNF_ArrendatarioPersonaIndividual.Estado_id = true;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_ArrendatarioPersonaIndividual.swcreatedbyinterno = true;

            }


            if ((!No_NIT_Valido(tbl_RNF_ArrendatarioPersonaIndividual.No_NIT)) && (tbl_RNF_ArrendatarioPersonaIndividual.No_NIT != null))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_RNF_ArrendatarioPersonaIndividual.No_Documento)) && (tbl_RNF_ArrendatarioPersonaIndividual.DocumentoID_Tipo == 1))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }


            if (DiferenciaAnio(tbl_RNF_ArrendatarioPersonaIndividual.Fecha_Nacimiento) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_RNF_ArrendatarioPersonaIndividual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroAgregado");

            }
            return View(tbl_RNF_ArrendatarioPersonaIndividual);
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
    }
}
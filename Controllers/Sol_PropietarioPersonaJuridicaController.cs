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
    public class Sol_PropietarioPersonaJuridicaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_PersonaJuridica
        public ActionResult Index()
        {
            return View(db.Tbl_Sol_PropietarioPersonaJuridica.ToList());
        }

        // GET: Sol_PersonaJuridica/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Find(id);
            if (tbl_Sol_PersonaJuridica == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Sol_PersonaJuridica);
        }

        // GET: Sol_PersonaJuridica/Create
        public ActionResult Create(long solicitud_id, string firma)
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

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = new Tbl_Sol_PropietarioPersonaJuridica();


            tbl_Sol_PersonaJuridica.swdatecreated = DateTime.Now;
            tbl_Sol_PersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);


            tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id = 7;
            tbl_Sol_PersonaJuridica.MunicipioEmpresa_id = 74;
            tbl_Sol_PersonaJuridica.PersonaJuridicaTipo_Id = 0;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            tbl_Sol_PersonaJuridica.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            // tbl_Sol_PersonaJuridica = db.Tbl_Sol_PersonaJuridica.Find(1);


            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_Sol_PersonaJuridica.MunicipioEmpresa_id);


            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_Sol_PersonaJuridica.PersonaJuridicaTipo_Id);




            tbl_Sol_PersonaJuridica.swdatecreated = DateTime.Now;
            tbl_Sol_PersonaJuridica.swcreatedbyinterno = (objUs.EsInterno == 1);

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaJuridica.UsuarioExterno_id = objUs.intUsuario_id;
                
            }

            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaJuridica.swcreatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaJuridica.swcreatedbyinterno = true;

            }




            return View(tbl_Sol_PersonaJuridica);
        }

        // POST: Sol_PersonaJuridica/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica)
        {

            TempData["Mensaje"] = "";

            bool ErrorDetectado = false;
            ViewBag.solicitud_id = tbl_Sol_PersonaJuridica.Solicitud_id;


            bool boolEsInterno = false;

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
                if (objUs.EsInterno == 1)
                {
                    boolEsInterno = true;
                }
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_PersonaJuridica.Solicitud_id, "Sol_PropietarioPersonaJuridica", boolEsInterno).FirstOrDefault();

            int cantidad = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == tbl_Sol_PersonaJuridica.Solicitud_id && Obj.No_NIT == tbl_Sol_PersonaJuridica.No_NIT && Obj.Estado_id == true).Count();

            if (cantidad > 0)
            {
                TempData["Mensaje"] = "Error: El propietario jurídico ya ha sido ingresado en esta solicitud. No puede agregarlo nuevamente.";
                ErrorDetectado = true;
            }



            if (!(bool)permisos.Agregar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
                ErrorDetectado = true;
            }


            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Propietario => Propietario.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).Max(u => u.PersonaJuridica_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }


            tbl_Sol_PersonaJuridica.PersonaJuridica_id = lngIdt;
            tbl_Sol_PersonaJuridica.Estado_id = true;

            if (!No_NIT_Valido(tbl_Sol_PersonaJuridica.No_NIT))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }




            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Tbl_Sol_PropietarioPersonaJuridica.Add(tbl_Sol_PersonaJuridica);
                db.SaveChanges();

                return RedirectToAction("../Sol_PropietarioPersonaIndividual/RegistroAgregado");

            }


            TempData["Mensaje"] = TempData["Mensaje"] + " Error detectado";

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj=>obj.Departamento_id == tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_Sol_PersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_Sol_PersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_Sol_PersonaJuridica);


        }



        // GET: Sol_PersonaJuridica/Eliminar
        public ActionResult Eliminar(long solicitud_id, long personaJuridica_id, string firma)
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

            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaJuridica_id == personaJuridica_id).First();


            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Propietario", boolEsInterno).FirstOrDefault();
            if (!(bool)permisos.Borrar)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }


            if (tbl_sol_solicitud.Solicitud_id != tbl_Sol_PersonaJuridica.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");
            }



            tbl_Sol_PersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaJuridica.swupdatedbyinterno = true;

            }

            tbl_Sol_PersonaJuridica.Estado_id = false;

            db.Entry(tbl_Sol_PersonaJuridica).State = EntityState.Modified;
            db.SaveChanges();



            return RedirectToAction("../Home/RegistroEliminado");

        }


        // GET: Sol_PersonaJuridica/Edit
        public ActionResult Edit(long solicitud_id, long personaJuridica_id, string firma)
        {


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


            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.PersonaJuridica_id == personaJuridica_id).First();

            if (tbl_sol_solicitud.Solicitud_id != tbl_Sol_PersonaJuridica.Solicitud_id)
            {
                return RedirectToAction("../Login/Index");
            }





            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_Sol_PersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_Sol_PersonaJuridica.PersonaJuridicaTipo_Id);


            return View(tbl_Sol_PersonaJuridica);
        }




        [HttpPost]
        public ActionResult Edit(Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PersonaJuridica)
        {

            bool ErrorDetectado = false;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            ViewBag.solicitud_id = tbl_Sol_PersonaJuridica.Solicitud_id;



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
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_PersonaJuridica.Solicitud_id, "Sol_Propietario", boolEsInterno).FirstOrDefault();
            if (!(bool)permisos.Editar)
            {
                TempData["Mensaje"] = "Error: El estatus de la solicitud no permite editar datos del propietario.";
                ErrorDetectado = true;
            }

            tbl_Sol_PersonaJuridica.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_PersonaJuridica.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Sol_PersonaJuridica.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Sol_PersonaJuridica.swupdatedbyinterno = true;

            }

            ViewBag.DepartamentoEmpresa_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id);
            ViewBag.MunicipioEmpresa_id = new SelectList(db.Tbl_Gral_Municipio.Where(obj => obj.Departamento_id == tbl_Sol_PersonaJuridica.DepartamentoEmpresa_id), "Municipio_id", "Municipio", tbl_Sol_PersonaJuridica.MunicipioEmpresa_id);

            ViewBag.PersonaJuridicaTipo_Id = new SelectList(db.Tbl_Sol_PersonaJuridicaTipo, "PersonaJuridicaTipo_Id", "Descripcion", tbl_Sol_PersonaJuridica.PersonaJuridicaTipo_Id);



            if (!No_NIT_Valido(tbl_Sol_PersonaJuridica.No_NIT))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_Sol_PersonaJuridica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroActualizado");
            }

            return View(tbl_Sol_PersonaJuridica);
        }



        public bool No_NIT_Valido(string No_NIT)
        {
            bool blResultado;

            No_NIT= No_NIT.Trim();

            blResultado = db.Database.SqlQuery<bool>($"SELECT dbo.Fnc_Gral_NIT_Valido('{No_NIT}')").FirstOrDefault();

            return blResultado;
        }

        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }




        // GET: Sol_PersonaJuridica/Create
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

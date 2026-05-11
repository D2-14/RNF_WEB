using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;


namespace RNF_Web.Controllers
{
    public class LocacionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Locacion
        public ActionResult Index()
        {
            IEnumerable<TAB_LOCATION> lista = (from c in db.TAB_LOCATION
                                               select c);
            return View(lista);
        }

        LocacionDatosSolicitante ObtenerDatosSolicitante(long Solicitud_id, long Finca_id = 0, int Tipo_de_Area = 0, long Rodal_id = 0)
        {
            LocacionDatosSolicitante oResultado = new LocacionDatosSolicitante();

            /*
                *******Reglas*******
            
                Todos los rodales

                fc_Sol_Finca_Rodal_Planos   Solicitud, 0, 0

                Todos los rodales de la finca 1

                fc_Sol_Finca_Rodal_Planos   Solicitud, 1, 0

                Todos los rodales de la rodal 1, finca 1

                fc_Sol_Finca_Rodal_Planos   Solicitud, 1, 1
            
            */

            long fincaid = 0;


            IEnumerable<Tbl_Sol_PropietarioPersonaJuridica>  tbl_Sol_PropietarioPersonaJuridica = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                     where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                     select d);

            IEnumerable<Tbl_Sol_PropietarioPersonaIndividual> tbl_Sol_PropietarioPersonaIndividual = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                         where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                         select d);

            oResultado.Nombre = "";

            if (tbl_Sol_PropietarioPersonaIndividual != null)
            {

                foreach (var personasIndividuales in tbl_Sol_PropietarioPersonaIndividual)
                {
                    if (oResultado.Nombre != "")
                    {
                        oResultado.Nombre = oResultado.Nombre + ", ";
                    }

                    oResultado.Nombre = oResultado.Nombre + personasIndividuales.Nombres + " " + personasIndividuales.Apellidos;

                }

            }

            if (tbl_Sol_PropietarioPersonaJuridica != null)
            {
               

                foreach (var personasjuridica in tbl_Sol_PropietarioPersonaJuridica)
                {
                    if (oResultado.Nombre != "")
                    {
                        oResultado.Nombre = oResultado.Nombre + ", ";
                    }

                    oResultado.Nombre = oResultado.Nombre + personasjuridica.Nombre;
                }

            }

            oResultado.Direccion = "";

            if (Finca_id == 0)
            {

                foreach (var finca in db.Tbl_Sol_Finca.Where(Obj=> Obj.Solicitud_id == Solicitud_id))
                {
                    if (oResultado.Direccion != "")
                    {
                        oResultado.Direccion = oResultado.Direccion + ", ";
                    }
                    oResultado.Direccion = finca.Ubicacion??"" + " " + finca.Tbl_Gral_Departamento.Departamento + ", " + finca.Tbl_Gral_Municipio.Municipio;
                }


            }

            List<fc_Sol_Finca_Rodal_Planos_Result> oDatosPlanos = (from d in db.fc_Sol_Finca_Rodal_Planos(Solicitud_id, Finca_id, Tipo_de_Area, Rodal_id)
                                                                   select d).ToList();

            for (int i = 0; i < oDatosPlanos.Count(); i++)
            {
                oResultado.FechaDia = oDatosPlanos[i].FechaDia;
                oResultado.Registro = oDatosPlanos[i].Categoria_Registro;
                oResultado.AreaEfectiva += oDatosPlanos[i].AreaEfectiva ?? 0;
                oResultado.LongitudEfectiva += oDatosPlanos[i].Longitud_Total ?? 0;
                if ((fincaid != oDatosPlanos[i].Finca_id) && (fincaid != 0))
                {
                    fincaid = oDatosPlanos[i].Finca_id;
                    oResultado.Fincas += $", {fincaid}";
                }
                if ((fincaid != oDatosPlanos[i].Finca_id) && (fincaid == 0))
                {
                    fincaid = oDatosPlanos[i].Finca_id;
                    oResultado.Fincas = $"{fincaid}";
                }

            }

            oResultado.Solicitud_id = Solicitud_id;
            oResultado.Finca_id = Finca_id;
            oResultado.Rodal_id = Rodal_id;

            return oResultado;
        }

        public ActionResult IndexRodal(long solicitud_id, string firma, long Rodal_id)
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

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            long lngSolicitud = (long)Session[Constants.session_Solicitud];

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == lngSolicitud).Count();
            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;

            IEnumerable<fc_Sol_Sel_RodalPoligono_Result> lista= (from c in db.fc_Sol_Sel_RodalPoligono((long)Session[Constants.session_Solicitud], Rodal_id)
                                               select c);
            ViewBag.Rodal_id = Rodal_id;
            return View(lista);

        }

        public ActionResult IndexRodalPlanoVertical(long solicitud_id, string firma, long Finca_id, long Rodal_id, string Tipo)
        {
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

            ViewBag.Tipo = Tipo;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            long lngSolicitud = solicitud_id;

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == lngSolicitud).Count();

            int Tipo_de_Area = 0;
            if (Tipo != null)
            {
                Tipo_de_Area = int.Parse(Tipo);
            }

            LocacionDatosSolicitante oDatosSolicitante = ObtenerDatosSolicitante(Solicitud_id: lngSolicitud, Finca_id: Finca_id, Tipo_de_Area: Tipo_de_Area, Rodal_id: Rodal_id);

            ViewBag.LlenarCampos = oDatosSolicitante;

            Tbl_Gral_VersionesDocumetoISO VersionesDocumetoISO = db.Tbl_Gral_VersionesDocumetoISO.Where(Obj => Obj.Documento == "PlanoVertical" && Obj.estado_id == true).FirstOrDefault();

            ViewBag.VersionesDocumetoISO = VersionesDocumetoISO;

            ViewBag.CantidadRodales = CantidadRodales;
            
            Session[Constants.session_Rodal] = (long)Rodal_id;

            if (Tipo == null)
            {
                Tipo = "0";
            }    

            IEnumerable<fc_Sol_Sel_RodalPoligonoPlano_Result> lista = (from c in db.fc_Sol_Sel_RodalPoligonoPlano((long)Session[Constants.session_Solicitud],Finca_id, Rodal_id, Tipo)
                                                                  select c);
            ViewBag.Finca_id = Finca_id;
            ViewBag.Rodal_id = Rodal_id;

            return View(lista);

        }

        public ActionResult IndexRodalGoogleMaps(long solicitud_id, string firma, long Rodal_id)
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

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            Session[Constants.session_Rodal] = (long)Rodal_id;

            IEnumerable<fc_Sol_Sel_RodalPoligono_Result> lista = (from c in db.fc_Sol_Sel_RodalPoligono((long)Session[Constants.session_Solicitud], Rodal_id)
                                                                  select c);

            return View(lista);

        }

        public JsonResult ObtenerCoordenadas()
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }


            IEnumerable<TAB_LOCATION> lst = (from c in db.TAB_LOCATION
                                             select c);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRodalCoordenadas()
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            //IEnumerable<fc_Sol_Sel_RodalPoligono_Result> lst = (from c in db.fc_Sol_Sel_RodalPoligono((long)Session[Constants.session_Solicitud], (long)Session[Constants.session_Rodal])
            //                                                      select c);

            IEnumerable<fc_Sol_Sel_RodalPoligonoPlano_Result> lst = (from c in db.fc_Sol_Sel_RodalPoligonoPlano((long)Session[Constants.session_Solicitud], (long)0, (long)Session[Constants.session_Rodal], "0")
                                                                     select c);


            return Json(lst, JsonRequestBehavior.AllowGet);
        }

   

        public JsonResult ObtenerRodalCoordenadasPlanoDatos(long Finca_id, string Tipo)
        {

            if ((Tipo == "") || (Tipo == null))
            {
                Tipo = "0";
            }


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            IEnumerable<fc_Sol_Sel_RodalPoligonoPlano_Result> lst = (from c in db.fc_Sol_Sel_RodalPoligonoPlano((long)Session[Constants.session_Solicitud], Finca_id, (long)Session[Constants.session_Rodal], Tipo)
                                                                     select c);


            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public ActionResult IndexFincaPlano(string Guidid, long Finca_id)
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
                return RedirectToAction("../Home/AccesoDenegado");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj=> Obj.Guid_id == Guidid).First();

            long lngSolicitud = (long)Session[Constants.session_Solicitud];

            int CantidadRodales = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == lngSolicitud).Count();

            LocacionDatosSolicitante oDatosSolicitante = ObtenerDatosSolicitante(Solicitud_id: lngSolicitud, Finca_id: Finca_id);

            ViewBag.LlenarCampos = oDatosSolicitante;

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Finca] = (long)Finca_id;

            IEnumerable<fc_Sol_Sel_FincaPoligonoPlano_Result> lista = (from c in db.fc_Sol_Sel_FincaPoligonoPlano(tbl_Sol_Solicitud.Solicitud_id, Finca_id)
                                                                       select c);

            ViewBag.Finca_id = Finca_id;
            return View(lista);

        }



        public JsonResult ObtenerFincaCoordenadasPlano()
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
                //  return Json("");

                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            IEnumerable<fc_Sol_Sel_FincaPoligonoPlano_Result> lst = (from c in db.fc_Sol_Sel_FincaPoligonoPlano((long)Session[Constants.session_Solicitud], (long)Session[Constants.session_Finca])
                                                                     select c);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

    }
}
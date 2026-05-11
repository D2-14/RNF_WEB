using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_LocacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        // GET: RNF_Locacion
        public ActionResult Index()
        {
            return View();
        }

        LocacionDatosRegistro ObtenerDatosRegistro(string Guid_id, long Finca_id = 0, int Tipo_de_Area = 0, long Rodal_id = 0)
        {
            LocacionDatosRegistro oResultado = new LocacionDatosRegistro();

            /*
            *******Reglas*******
            
             Todos los rodales

            fc_RNF_Finca_Rodal_Planos   Solicitud, 0, 0

            Todos los rodales de la finca 1

            fc_RNF_Finca_Rodal_Planos   Solicitud, 1, 0

            Todos los rodales de la rodal 1, finca 1

            fc_RNF_Finca_Rodal_Planos   Solicitud, 1, 1
             */

            long fincaid = 0;

            IEnumerable<Tbl_RNF_PropietarioPersonaIndividual> tbl_RNF_PropietarioPersonaIndividuals = (from d in db.Tbl_RNF_PropietarioPersonaIndividual
                                                                                                       where d.No_Registro == Guid_id && d.Estado_id == true
                                                                                                       select d);

            IEnumerable<Tbl_RNF_PropietarioPersonaJuridica> tbl_RNF_PropietarioPersonaJuridicas = (from d in db.Tbl_RNF_PropietarioPersonaJuridica
                                                                                                   where d.No_Registro == Guid_id && d.Estado_id == true
                                                                                                   select d);



            if (tbl_RNF_PropietarioPersonaIndividuals != null)
            {
                foreach (var personasIndividuales in tbl_RNF_PropietarioPersonaIndividuals)
                {
                    if (oResultado.Nombre != "")
                    {
                        oResultado.Nombre = oResultado.Nombre + ", ";
                    }

                    oResultado.Nombre = oResultado.Nombre + personasIndividuales.Nombres + " " + personasIndividuales.Apellidos;

                }
            }

            if (tbl_RNF_PropietarioPersonaJuridicas != null)
            {


                foreach (var personasjuridica in tbl_RNF_PropietarioPersonaJuridicas)
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

                foreach (var finca in db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id))
                {
                    if (oResultado.Direccion != "")
                    {
                        oResultado.Direccion = oResultado.Direccion + ", ";
                    }
                    oResultado.Direccion = finca.Ubicacion ?? "" + " " + finca.Tbl_Gral_Departamento.Departamento + ", " + finca.Tbl_Gral_Municipio.Municipio;
                }


            }

            List<fc_RNF_Finca_Rodal_Planos_Result> oDatosPlanos = (from d in db.fc_RNF_Finca_Rodal_Planos(Guid_id, Finca_id, Tipo_de_Area, Rodal_id)
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

            oResultado.Guid_id = Guid_id;
            oResultado.Finca_id = Finca_id;
            oResultado.Rodal_id = Rodal_id;

            return oResultado;
        }

        public ActionResult IndexRodal(string Guid_id, long Rodal_id)
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

            ViewBag.Guid_id = Guid_id;

            int CantidadRodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == Guid_id).Count();
            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;

            IEnumerable<fc_RNF_Sel_RodalPoligono_Result> lista = (from c in db.fc_RNF_Sel_RodalPoligono(Guid_id, Rodal_id)
                                                                  select c);

            ViewBag.Rodal_id = Rodal_id;
            return View(lista);

        }

        public ActionResult IndexRodalPlano(string Guid_id, long Rodal_id)
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

            ViewBag.Guid_id = Guid_id;

            int CantidadRodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == Guid_id).Count();

            LocacionDatosRegistro oDatosSolicitante = ObtenerDatosRegistro(Guid_id: Guid_id, Rodal_id: Rodal_id);
            ViewBag.LlenarCampos = oDatosSolicitante;

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;

            IEnumerable<fc_RNF_Sel_RodalPoligonoPlano_Result> lista = (from c in db.fc_RNF_Sel_RodalPoligonoPlano(Guid_id, Rodal_id,0,"0")
                                                                       select c);

            ViewBag.Rodal_id = Rodal_id;
            return View(lista);

        }

        public ActionResult IndexRodalGoogleMaps(string Guid_id, long Rodal_id)
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

            ViewBag.Guid_id = Guid_id;

            Session[Constants.session_Rodal] = (long)Rodal_id;

            IEnumerable<fc_RNF_Sel_RodalPoligono_Result> lista = (from c in db.fc_RNF_Sel_RodalPoligono(Guid_id, Rodal_id)
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

        public JsonResult ObtenerRodalCoordenadas(string Guid_id)
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

            IEnumerable<fc_RNF_Sel_RodalPoligono_Result> lst = (from c in db.fc_RNF_Sel_RodalPoligono(Guid_id, (long)Session[Constants.session_Rodal])
                                                                select c);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRodalCoordenadasPlano(string Guid_id, long Finca_id, long Rodal_id, string Tipo)
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

            IEnumerable<fc_RNF_Sel_RodalPoligonoPlano_Result> lst = (from c in db.fc_RNF_Sel_RodalPoligonoPlano(Guid_id, Finca_id, Rodal_id, Tipo)
                                                                     select c);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexFincaPlano(string Guid_id, long Finca_id)
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

            ViewBag.Guid_id = Guid_id;

            int CantidadRodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == Guid_id).Count();

            LocacionDatosRegistro oDatosSolicitante = ObtenerDatosRegistro(Guid_id: Guid_id, Finca_id: Finca_id);
            ViewBag.LlenarCampos = oDatosSolicitante;

            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Finca] = (long)Finca_id;

            IEnumerable<fc_RNF_Sel_FincaPoligonoPlano_Result> lista = (from c in db.fc_RNF_Sel_FincaPoligonoPlano(Guid_id, Finca_id)
                                                                       select c);

            ViewBag.Finca_id = Finca_id;
            return View(lista);

        }

        public ActionResult IndexRodalPlanoVertical(string No_Registro, long Finca_id, long Rodal_id, string Tipo)
        {

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro == null)
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

            //long lngSolicitud = solicitud_id;

            int CantidadRodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == No_Registro).Count();

            int Tipo_de_Area = 0;
            if (Tipo != null)
            {
                Tipo_de_Area = int.Parse(Tipo);
            }

            //LocacionDatosRegistro oDatosSolicitante = ObtenerDatosRegistro(Guid_id: No_Registro, Rodal_id: Rodal_id);
            LocacionDatosRegistro oDatosSolicitante = ObtenerDatosRegistro(Guid_id: No_Registro, Tipo_de_Area: Tipo_de_Area, Rodal_id: Rodal_id);
            ViewBag.LlenarCampos = oDatosSolicitante;


            ViewBag.CantidadRodales = CantidadRodales;

            Session[Constants.session_Rodal] = (long)Rodal_id;

            if (Tipo == null)
            {
                Tipo = "0";
            }

            IEnumerable<fc_RNF_Sel_RodalPoligonoPlano_Result> lista = (from c in db.fc_RNF_Sel_RodalPoligonoPlano(No_Registro, Finca_id, Rodal_id, Tipo)
                                                                       select c);

            ViewBag.No_Registro = No_Registro;
            ViewBag.Finca_id = Finca_id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Tipo = Tipo;

            return View(lista);

        }


        public JsonResult ObtenerFincaCoordenadasPlano(string Guid_id)
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

            IEnumerable<fc_RNF_Sel_FincaPoligonoPlano_Result> lst = (from c in db.fc_RNF_Sel_FincaPoligonoPlano(Guid_id, (long)Session[Constants.session_Finca])
                                                                     select c);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;
using DotSpatial.Topology;
using System.Data.Entity;
using PagedList;

namespace RNF_Web.Controllers
{
    public class API_ProbosqueController : Controller
    {

        RequestUtil request = new RequestUtil();
        GlobalUtils globalUtils = new GlobalUtils();
        DatosAPI datosAPI = new DatosAPI();
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
        db_RNF_IntermediaEntities db_Intermedia = new db_RNF_IntermediaEntities();
        db_RNFEntities db = new db_RNFEntities();


        bool ValidarFaseExpediente(string Fase)
        {

            return db_Intermedia.Database.SqlQuery<bool>($"SELECT dbo.Fcn_Intermedia_ValidarFaseExpediente('{Fase.Trim()}')").FirstOrDefault();
        }

        public JsonResult ValidarFaseCertificada(string Fase)
        {
            bool result = ValidarFaseExpediente(Fase ?? "");
            return Json(result);
        }


        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetExpedienteProbosque(string Expediente)
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
             datosAPI.codigo = Expediente;
            string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataProbosque";
            string method = "get";

            Api_Probosque_Get response = request.Execute_Probosque_Get<DatosAPI>(url, method, datosAPI, Expediente);

            if (response.Result == 1)
            {
                Api_Probosque_Data_Proyecto_Get proyecto = response.Data.proyecto;
            }

            response.Mensaje = globalUtils.InitCap(response.Mensaje);

            return Json(JsonConvert.SerializeObject(response));
            //return Json(response);
        }
        public JsonResult PostExpedienteProbosque(string Expediente)
        {
            datosAPI.codigo = Expediente;
            string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataProbosque";
            string method = "post";
            string rootpath = Server.MapPath("~/");

            Api_Probosque_Post response = new Api_Probosque_Post();
            Tbl_API_Probosque_Expediente ExpedienteExistente = (from d in db_Intermedia.Tbl_API_Probosque_Expediente
                                                                where d.Expediente == Expediente
                                                                select d).FirstOrDefault();
            if (ExpedienteExistente != null)
            {
                response.Result = 5;
                response.Guid_id = ExpedienteExistente.Expediente;
                response.Migrado = (ExpedienteExistente.Migrado ?? false);
                response.Solicitud_id = (ExpedienteExistente.Solicitud_id ?? 0);
                response.TipoProyecto = ExpedienteExistente.TipoProyecto;
                response.Region = ExpedienteExistente.Region;
                response.SubRegion = ExpedienteExistente.SubRegion;
                response.Mensaje = "Expediente ya ha sido agregado con anterioridad";
            }
            else
            {

                try
                {
                    response = request.Execute_Probosque_Post<DatosAPI>(url, method, datosAPI, Expediente, rootpath);
                    if (response.Result == 1)
                    {
                        string fase = response.Data.proyecto.UltimaFaseCertificada;
                        if (ValidarFaseExpediente(fase))
                        {


                            string ExpedienteRegistrado = RegistrarNuevoExpedienteProbosque(response.Data.proyecto);

                            Tbl_API_Probosque_Expediente ExpedienteNuevo = (from d in db_Intermedia.Tbl_API_Probosque_Expediente
                                                                            where d.Guid_id == ExpedienteRegistrado
                                                                            select d).FirstOrDefault();
                            response.Guid_id = ExpedienteNuevo.Expediente;
                            response.Migrado = (ExpedienteNuevo.Migrado ?? false);
                            response.Solicitud_id = (ExpedienteNuevo.Solicitud_id ?? 0);
                            response.TipoProyecto = ExpedienteNuevo.TipoProyecto;
                            response.Region = ExpedienteNuevo.Region;
                            response.SubRegion = ExpedienteNuevo.SubRegion;                            
                            response.Mensaje = "Expediente registrado exitosamente";
                        }

                    }
                    else
                    {
                        response.Mensaje = "No se ha registrado debido a que la fase no se encuentra en una opción válida para registrarse";
                    }
                }
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex);
                //    response.Result = 6;
                //    response.Mensaje = "Ocurrió un error al registrar el expediente... Mensaje: " + ex.Message;

                //}

                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            //Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            response.Result = 6;
                            response.Mensaje = "Ocurrió un error al registrar el expediente... Mensaje: " + validationError.ErrorMessage;
                        }
                    }
                }
            }

            response.Mensaje = globalUtils.InitCap(response.Mensaje);

            return Json(response);
        }

        public JsonResult GetExpedientePinpepOld(string Expediente)
        {
            jsonSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss.fffZ"; //this won't help much for the 'date' only field!
            datosAPI.codigo = Expediente;
            //string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataPinpepOld";
            string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataPinpepNew";
            string method = "get";


            Api_PinpepOld_Get response = request.Execute_PinpepOld_Get<DatosAPI>(url, method, datosAPI, Expediente);

            if (response.Result == 1)
            {
                Api_PinpepOld_Data_Proyecto_Get proyecto = response.Data.proyecto;
            }

            response.Mensaje = globalUtils.InitCap(response.Mensaje);

            return Json(JsonConvert.SerializeObject(response));
        }

        //********************* Solucion para seguir editando una solicitud CERP*********************

        public JsonResult PostExpedientePinpepOld(string Expediente)
        {
            datosAPI.codigo = Expediente;
            //string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataPinpepOld";
            string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataPinpepNew";
            string method = "post";
            string rootpath = Server.MapPath("~/");

            Tbl_API_PinpepOld_Expediente ExpedienteExistente = (from d in db_Intermedia.Tbl_API_PinpepOld_Expediente
                                                                where d.Expediente == Expediente
                                                                select d).FirstOrDefault();
            Api_PinpepOld_Post response = new Api_PinpepOld_Post();

            //if (ExpedienteExistente != null)
            //{
            //    response.Result = 5;
            //    response.Guid_id = ExpedienteExistente.Expediente;
            //    response.Mensaje = "Expediente ya ha sido agregado con anterioridad";
            //}
            //else
            //{

            try
            {
                response = request.Execute_PinpepOld_Post<DatosAPI>(url, method, datosAPI, Expediente, rootpath);
                if (response.Result == 1)
                {
                    string fase = response.Data.proyecto.UltimaFaseCertificada;
                    if (ValidarFaseExpediente(fase))
                    {
                        response.Guid_id = RegistrarNuevoExpedientePinpepOld(response.Data.proyecto);
                        response.Mensaje = "Expediente registrado exitosamente";
                    }
                    else
                    {
                        response.Mensaje = "No se ha registrado debido a que la fase no se encuentra en una opción válida para registrarse";
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    response.Result = 6;
            //    response.Mensaje = "Ocurrió un error al registrar el expediente... Mensaje: " + ex.Message;
            //    Console.WriteLine(ex);
            //}

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Entidad: {validationErrors.Entry.Entity.GetType().Name}");
                        Console.WriteLine($"Propiedad: {error.PropertyName} - Error: {error.ErrorMessage}");
                    }
                }

                throw; // Opcional: relanza la excepción para detener la ejecución
            }

            //}



            response.Mensaje = globalUtils.InitCap(response.Mensaje);

            return Json(response);
        }


        //public JsonResult PostExpedientePinpepOld(string Expediente)
        //{
        //    datosAPI.codigo = Expediente;
        //    string url = Constants.IP_Pinpep_Probosque + "/Api_RNF/api/DataPinpepOld";
        //    string method = "post";
        //    string rootpath = Server.MapPath("~/");

        //    Tbl_API_PinpepOld_Expediente ExpedienteExistente = (from d in db_Intermedia.Tbl_API_PinpepOld_Expediente
        //                                                        where d.Expediente == Expediente
        //                                                        select d).FirstOrDefault();
        //    Api_PinpepOld_Post response = new Api_PinpepOld_Post();
        //    if (ExpedienteExistente != null)
        //    {
        //        response.Result = 5;
        //        response.Guid_id = ExpedienteExistente.Expediente;
        //        response.Mensaje = "Expediente ya ha sido agregado con anterioridad";
        //    }
        //    else
        //    {

        //        try
        //        {
        //            response = request.Execute_PinpepOld_Post<DatosAPI>(url, method, datosAPI, Expediente, rootpath);
        //            if (response.Result == 1)
        //            {
        //                string fase = response.Data.proyecto.UltimaFaseCertificada;
        //                if (ValidarFaseExpediente(fase))
        //                {
        //                    response.Guid_id = RegistrarNuevoExpedientePinpepOld(response.Data.proyecto);
        //                    response.Mensaje = "Expediente registrado exitosamente";
        //                }
        //                else
        //                {
        //                    response.Mensaje = "No se ha registrado debido a que la fase no se encuentra en una opción válida para registrarse";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Result = 6;
        //            response.Mensaje = "Ocurrió un error al registrar el expediente... Mensaje: " + ex.Message;
        //            Console.WriteLine(ex);
        //        }

        //    }



        //    response.Mensaje = globalUtils.InitCap(response.Mensaje);

        //    return Json(response);
        //}

        public ActionResult BuscarProbosque()
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


            return View();
        }
        public ActionResult BuscarPinpepOld()
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


            return View();
        }

        public ActionResult ResultadoProbosque(string Expediente)
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


            return View(model: Expediente);
        }
        public ActionResult ResultadoPinpepOld(string Expediente)
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


            return View(model: Expediente);
        }


        public ActionResult ListarSolicitud()
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

            List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = (from d in db.Tbl_Sol_Solicitud
                                                          where d.Procedencia_Probosque == true
                                                          || d.Procedencia_PinpepOld == true
                                                          || d.Procedencia_PinpepNew == true
                                                          || d.Procedencia_secorf == true
                                                          orderby d.swdatecreated descending
                                                          select d).ToList();

            return View(tbl_Sol_Solicituds);
        }
        public ActionResult ListarProbosque(string sortOrder, int? page, string Expediente = null, string Modalidad = null, string TipoProyecto = null, string Fase = null, string DPI_Titular = null)
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


            string sqlQuery = "";
            sqlQuery = $"Select Sol.* from Tbl_Sol_Solicitud Sol\n";
            sqlQuery += $"where Sol.Procedencia_Probosque = 1\n";


            if ((DPI_Titular != null) && (DPI_Titular.Trim() != ""))
            {
                string sDPI_Titular = DPI_Titular;
                sDPI_Titular = sDPI_Titular.Trim();
                sDPI_Titular = sDPI_Titular.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.DPI_Titular Like '%{sDPI_Titular}%'\n";
            }

            if ((Expediente != null) && (Expediente.Trim() != ""))
            {
                string sExpediente = Expediente;
                sExpediente = sExpediente.Trim();
                sExpediente = sExpediente.Replace(" ", "%");
                sqlQuery += $"and Sol.Procedencia_Expediente Like '%{sExpediente}%'\n";
            }

            if ((Modalidad != null) && (Modalidad.Trim() != ""))
            {
                string sModalidad = Modalidad;
                sModalidad = sModalidad.Trim();
                sModalidad = sModalidad.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.Procedencia_Modalidad Like '%{sModalidad}%'\n";
            }

            if ((Fase != null) && (Fase.Trim() != ""))
            {
                string sFase = Fase;
                sFase = sFase.Trim();
                sFase = sFase.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.Procedencia_Fase Like '%{sFase}%'\n";
            }

            if ((TipoProyecto != null) && (TipoProyecto.Trim() != ""))
            {
                string sTipoProyecto = TipoProyecto;
                sTipoProyecto = sTipoProyecto.Trim();
                sTipoProyecto = sTipoProyecto.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.Procedencia_TipoProyecto Like '%{sTipoProyecto}%'\n";
            }

            List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = db.Tbl_Sol_Solicitud.SqlQuery(sqlQuery).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DPI_Titular = DPI_Titular;
            ViewBag.Expediente = Expediente;
            ViewBag.Modalidad = Modalidad;
            ViewBag.Fase = Fase;
            ViewBag.TipoProyecto = TipoProyecto;


            ViewBag.CurrentSort = sortOrder;
            ViewBag.sortswdatecreated = sortOrder == "swdatecreated" ? "swdatecreated_desc" : "swdatecreated";
            ViewBag.sortDPI_Titular = sortOrder == "DPI_Titular" ? "dpi_titular_desc" : "DPI_Titular";
            ViewBag.sortExpediente = sortOrder == "Expediente" ? "expediente_desc" : "Expediente";
            ViewBag.sortModalidad = sortOrder == "Modalidad" ? "modalidad_desc" : "Modalidad";
            ViewBag.sortFase = sortOrder == "Fase" ? "fase_desc" : "Fase";
            ViewBag.sortTipoProyecto = sortOrder == "TipoProyecto" ? "tipoproyecto_desc" : "TipoProyecto";


            var ListarSolicitudes = (from d in tbl_Sol_Solicituds select d);

            switch (sortOrder)
            {
                case "swdatecreated":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.swdatecreated);
                    break;
                case "swdatecreated_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.swdatecreated);
                    break;
                case "DPI_Titular":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.DPI_Titular);
                    break;
                case "dpi_titular_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.DPI_Titular);
                    break;
                case "Expediente":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Expediente);
                    break;
                case "expediente_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Expediente);
                    break;
                case "Modalidad":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Modalidad);
                    break;
                case "modalidad_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Modalidad);
                    break;
                case "Fase":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Fase);
                    break;
                case "fase_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Fase);
                    break;
                case "TipoProyecto":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_TipoProyecto);
                    break;
                case "tipoproyecto_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_TipoProyecto);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);


            return View(ListarSolicitudes.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ListarPinpep(string sortOrder, int? page, string Expediente = null, string Modalidad = null, string Fase = null, string DPI_Titular = null)
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


            string sqlQuery = "";
            sqlQuery = $"Select Sol.* from Tbl_Sol_Solicitud Sol\n";
            sqlQuery += $"where Sol.Procedencia_PinpepOld = 1 or Sol.Procedencia_PinpepNew = 1 \n";


            if ((DPI_Titular != null) && (DPI_Titular.Trim() != ""))
            {
                string sDPI_Titular = DPI_Titular;
                sDPI_Titular = sDPI_Titular.Trim();
                sDPI_Titular = sDPI_Titular.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.DPI_Titular Like '%{sDPI_Titular}%'\n";
            }

            if ((Expediente != null) && (Expediente.Trim() != ""))
            {
                string sExpediente = Expediente;
                sExpediente = sExpediente.Trim();
                sExpediente = sExpediente.Replace(" ", "%");
                sqlQuery += $"and Sol.Procedencia_Expediente Like '%{sExpediente}%'\n";
            }

            if ((Modalidad != null) && (Modalidad.Trim() != ""))
            {
                string sModalidad = Modalidad;
                sModalidad = sModalidad.Trim();
                sModalidad = sModalidad.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.Procedencia_Modalidad Like '%{sModalidad}%'\n";
            }

            if ((Fase != null) && (Fase.Trim() != ""))
            {
                string sFase = Fase;
                sFase = sFase.Trim();
                sFase = sFase.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                sqlQuery += $"and Sol.Procedencia_Fase Like '%{sFase}%'\n";
            }

            List<Tbl_Sol_Solicitud> tbl_Sol_Solicituds = db.Tbl_Sol_Solicitud.SqlQuery(sqlQuery).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DPI_Titular = DPI_Titular;
            ViewBag.Expediente = Expediente;
            ViewBag.Modalidad = Modalidad;
            ViewBag.Fase = Fase;


            ViewBag.CurrentSort = sortOrder;
            ViewBag.sortswdatecreated = sortOrder == "swdatecreated" ? "swdatecreated_desc" : "swdatecreated";
            ViewBag.sortDPI_Titular = sortOrder == "DPI_Titular" ? "dpi_titular_desc" : "DPI_Titular";
            ViewBag.sortExpediente = sortOrder == "Expediente" ? "expediente_desc" : "Expediente";
            ViewBag.sortModalidad = sortOrder == "Modalidad" ? "modalidad_desc" : "Modalidad";
            ViewBag.sortFase = sortOrder == "Fase" ? "fase_desc" : "Fase";
            ViewBag.sortPinpepOrigen = sortOrder == "PinpepOld" ? "PinpepNew" : "PinpepOld";

            var ListarSolicitudes = (from d in tbl_Sol_Solicituds select d);

            switch (sortOrder)
            {
                case "swdatecreated":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.swdatecreated);
                    break;
                case "swdatecreated_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.swdatecreated);
                    break;
                case "DPI_Titular":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.DPI_Titular);
                    break;
                case "dpi_titular_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.DPI_Titular);
                    break;
                case "Expediente":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Expediente);
                    break;
                case "expediente_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Expediente);
                    break;
                case "Modalidad":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Modalidad);
                    break;
                case "modalidad_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Modalidad);
                    break;
                case "Fase":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Fase);
                    break;
                case "fase_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Fase);
                    break;
                case "PinpepOld":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_PinpepOld);
                    break;
                case "PinpepNew":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_PinpepNew);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);


            return View(ListarSolicitudes.ToPagedList(pageNumber, pageSize));
        }


        public string RegistrarNuevoExpedientePinpepOld(Api_PinpepOld_Data_Proyecto_Post proyecto)
        {
            Tbl_API_PinpepOld_Expediente ExpedienteExistente = (from d in db_Intermedia.Tbl_API_PinpepOld_Expediente
                                                                where d.Expediente == proyecto.Expediente
                                                                select d).FirstOrDefault();
            string Guid_id = Guid.NewGuid().ToString();

            try
            {



                if (ExpedienteExistente != null)
                {
                    Guid_id = ExpedienteExistente.Guid_id;
                }
                else
                {

                    Api_PinpepOld_Data_Proyecto_Propietarios Propietarios;
                    List<Api_PinpepOld_Data_Proyecto_PropietariosGrupal_Post> PropietariosGrupal;
                    Api_PinpepOld_Data_Proyecto_Representante_Post Representante;
                    Api_PinpepOld_Data_Proyecto_InformeTecnico_Post InformeTecnico;
                    List<Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_Post> Rodales;
                    List<Api_PinpepOld_Data_Proyecto_Poligonos_Post> Poligonos;
                    List<Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_Post> PoligonosDescuento;
                    List<Api_PinpepOld_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post> EspeciesForestales;

                    Tbl_API_PinpepOld_Expediente tbl_API_PinpepOld_Expediente;
                    Tbl_API_PinpepOld_Finca tbl_API_PinpepOld_Finca;
                    Tbl_API_PinpepOld_Propietarios tbl_API_PinpepOld_Propietarios;
                    Tbl_API_PinpepOld_PropietariosGrupal tbl_API_PinpepOld_PropietariosGrupal;
                    Tbl_API_PinpepOld_Representante tbl_API_PinpepOld_Representante;
                    Tbl_API_PinpepOld_InformeTecnico tbl_API_PinpepOld_InformeTecnico;
                    Tbl_API_PinpepOld_Rodales tbl_API_PinpepOld_Rodales;
                    Tbl_API_PinpepOld_EspeciesForestales tbl_API_PinpepOld_EspeciesForestales;
                    Tbl_API_PinpepOld_Rodales_Poligonos tbl_API_PinpepOld_Rodales_Poligonos;
                    Tbl_API_PinpepOld_Rodales_PoligonosDescuento tbl_API_PinpepOld_Rodales_PoligonosDescuento;

                    tbl_API_PinpepOld_Expediente = new Tbl_API_PinpepOld_Expediente();
                    tbl_API_PinpepOld_Expediente.Guid_id = Guid_id;
                    tbl_API_PinpepOld_Expediente.ProyectoId = proyecto.ProyectoId;
                    tbl_API_PinpepOld_Expediente.Expediente = proyecto.Expediente;
                    tbl_API_PinpepOld_Expediente.Modalidad = proyecto.Modalidad;
                    tbl_API_PinpepOld_Expediente.Region = proyecto.Region;
                    tbl_API_PinpepOld_Expediente.SubRegion = proyecto.SubRegion;
                    tbl_API_PinpepOld_Expediente.UltimaFaseCertificada = proyecto.UltimaFaseCertificada;
                    tbl_API_PinpepOld_Expediente.UltimaAreaCertificada = proyecto.UltimaAreaCertificada;
                    db_Intermedia.Tbl_API_PinpepOld_Expediente.Add(tbl_API_PinpepOld_Expediente);

                    tbl_API_PinpepOld_Finca = new Tbl_API_PinpepOld_Finca();
                    tbl_API_PinpepOld_Finca.Guid_id = Guid_id;
                    tbl_API_PinpepOld_Finca.FincaMunicipio = proyecto.FincaMunicipio;
                    tbl_API_PinpepOld_Finca.FincaDepartamento = proyecto.FincaDepartamento;
                    tbl_API_PinpepOld_Finca.FincaUbicacion = proyecto.FincaUbicacion;
                    tbl_API_PinpepOld_Finca.FincaLugar = proyecto.FincaLugar;
                    tbl_API_PinpepOld_Finca.FincaRefX = proyecto.FincaRefX;
                    tbl_API_PinpepOld_Finca.FincaRefY = proyecto.FincaRefY;
                    tbl_API_PinpepOld_Finca.AreaAprobada = proyecto.AreaAprobada;
                    db_Intermedia.Tbl_API_PinpepOld_Finca.Add(tbl_API_PinpepOld_Finca);

                    if (proyecto.Propietarios != null)
                    {
                        Propietarios = proyecto.Propietarios;

                        tbl_API_PinpepOld_Propietarios = new Tbl_API_PinpepOld_Propietarios();
                        tbl_API_PinpepOld_Propietarios.Guid_id = Guid_id;
                        tbl_API_PinpepOld_Propietarios.NoDPI = Propietarios.NoDPI;
                        tbl_API_PinpepOld_Propietarios.Nit = Propietarios.Nit;
                        tbl_API_PinpepOld_Propietarios.NombreCompleto = Propietarios.NombreCompleto;

                        db_Intermedia.Tbl_API_PinpepOld_Propietarios.Add(tbl_API_PinpepOld_Propietarios);

                    }

                    if (proyecto.PropietariosGrupal.Count() > 0)
                    {
                        PropietariosGrupal = proyecto.PropietariosGrupal;

                        long countPropietariosGrupal = 0;
                        foreach (var item in PropietariosGrupal)
                        {
                            countPropietariosGrupal++;
                            tbl_API_PinpepOld_PropietariosGrupal = new Tbl_API_PinpepOld_PropietariosGrupal();
                            tbl_API_PinpepOld_PropietariosGrupal.Guid_id = Guid_id;
                            tbl_API_PinpepOld_PropietariosGrupal.Id_PropietarioGrupal = countPropietariosGrupal;
                            tbl_API_PinpepOld_PropietariosGrupal.NoDPI = HttpUtility.HtmlDecode((item.NoDPI ?? ""));
                            tbl_API_PinpepOld_PropietariosGrupal.NombreCompleto = item.NombreCompleto;

                            db_Intermedia.Tbl_API_PinpepOld_PropietariosGrupal.Add(tbl_API_PinpepOld_PropietariosGrupal);
                        }

                    }

                    if (proyecto.Representante != null)
                    {
                        Representante = proyecto.Representante;

                        tbl_API_PinpepOld_Representante = new Tbl_API_PinpepOld_Representante();
                        tbl_API_PinpepOld_Representante.Guid_id = Guid_id;
                        tbl_API_PinpepOld_Representante.NoDPI = Representante.NoDPI;
                        tbl_API_PinpepOld_Representante.NombreCompleto = Representante.NombreCompleto;

                        db_Intermedia.Tbl_API_PinpepOld_Representante.Add(tbl_API_PinpepOld_Representante);
                    }

                    if (proyecto.InformeTecnico != null)
                    {
                        InformeTecnico = proyecto.InformeTecnico;

                        tbl_API_PinpepOld_InformeTecnico = new Tbl_API_PinpepOld_InformeTecnico();
                        tbl_API_PinpepOld_InformeTecnico.Guid_id = Guid_id;
                        tbl_API_PinpepOld_InformeTecnico.NumeroInforme = InformeTecnico.NumeroInforme;
                        tbl_API_PinpepOld_InformeTecnico.PDFInforme = InformeTecnico.PDFInforme;
                        tbl_API_PinpepOld_InformeTecnico.PDFInforme_Local = InformeTecnico.PDFInforme_Local;
                        tbl_API_PinpepOld_InformeTecnico.UltimaFaseCertificada = InformeTecnico.UltimaFaseCertificada;
                        db_Intermedia.Tbl_API_PinpepOld_InformeTecnico.Add(tbl_API_PinpepOld_InformeTecnico);

                    }

                    if (proyecto.InformeTecnico.Rodales.Count() > 0)
                    {
                        Rodales = proyecto.InformeTecnico.Rodales;

                        long countEspeciesForestales = 0;
                        foreach (var item in Rodales)
                        {
                            tbl_API_PinpepOld_Rodales = new Tbl_API_PinpepOld_Rodales();
                            tbl_API_PinpepOld_Rodales.Guid_id = Guid_id;
                            tbl_API_PinpepOld_Rodales.Id = item.Id;
                            tbl_API_PinpepOld_Rodales.Area = item.Area;
                            tbl_API_PinpepOld_Rodales.EspeciesProteger = item.EspeciesProteger;
                            db_Intermedia.Tbl_API_PinpepOld_Rodales.Add(tbl_API_PinpepOld_Rodales);

                            if (item.EspeciesForestales.Count() > 0)
                            {
                                EspeciesForestales = item.EspeciesForestales;

                                foreach (var item2 in EspeciesForestales)
                                {

                                    countEspeciesForestales++;

                                    tbl_API_PinpepOld_EspeciesForestales = new Tbl_API_PinpepOld_EspeciesForestales();
                                    tbl_API_PinpepOld_EspeciesForestales.Guid_id = Guid_id;
                                    tbl_API_PinpepOld_EspeciesForestales.RodalId = item.Id;
                                    tbl_API_PinpepOld_EspeciesForestales.Correlativo_id = countEspeciesForestales;
                                    tbl_API_PinpepOld_EspeciesForestales.NombreEspecie = item2.NombreEspecie;
                                    tbl_API_PinpepOld_EspeciesForestales.ArbolesPorHa = item2.ArbolesPorHa;
                                    tbl_API_PinpepOld_EspeciesForestales.Area = item2.Area;
                                    db_Intermedia.Tbl_API_PinpepOld_EspeciesForestales.Add(tbl_API_PinpepOld_EspeciesForestales);

                                }

                            }

                        }


                    }

                    if (proyecto.Poligonos.Count() > 0)
                    {
                        Poligonos = proyecto.Poligonos;

                        foreach (var item in Poligonos)
                        {

                            tbl_API_PinpepOld_Rodales_Poligonos = new Tbl_API_PinpepOld_Rodales_Poligonos();
                            tbl_API_PinpepOld_Rodales_Poligonos.Guid_id = Guid_id;
                            tbl_API_PinpepOld_Rodales_Poligonos.Correlativo = item.Correlativo;
                            tbl_API_PinpepOld_Rodales_Poligonos.GeometriaGTM = item.GeometriaGTM.Geometry.WellKnownText;
                            tbl_API_PinpepOld_Rodales_Poligonos.PDFPol = item.PDFPol;
                            tbl_API_PinpepOld_Rodales_Poligonos.PODPol_Local = item.PDFPol_Local;
                            db_Intermedia.Tbl_API_PinpepOld_Rodales_Poligonos.Add(tbl_API_PinpepOld_Rodales_Poligonos);
                            int Id_Correlativo = 0;
                            if ((item.PoligonosDescuento ?? new List<Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_Post>()).Count() > 0)
                            {
                                PoligonosDescuento = item.PoligonosDescuento ?? new List<Api_PinpepOld_Data_Proyecto_Poligonos_PoligonosDescuento_Post>();

                                foreach (var item2 in PoligonosDescuento)
                                {
                                    Id_Correlativo++;
                                    tbl_API_PinpepOld_Rodales_PoligonosDescuento = new Tbl_API_PinpepOld_Rodales_PoligonosDescuento();
                                    tbl_API_PinpepOld_Rodales_PoligonosDescuento.Guid_id = Guid_id;
                                    tbl_API_PinpepOld_Rodales_PoligonosDescuento.Correlativo = item.Correlativo;
                                    tbl_API_PinpepOld_Rodales_PoligonosDescuento.Id_Correlativo = Id_Correlativo;
                                    tbl_API_PinpepOld_Rodales_PoligonosDescuento.GeometriaGTM = item2.Geometry.WellKnownText;
                                    db_Intermedia.Tbl_API_PinpepOld_Rodales_PoligonosDescuento.Add(tbl_API_PinpepOld_Rodales_PoligonosDescuento);

                                }

                            }

                        }

                    }

                    db_Intermedia.SaveChanges();

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    var entityName = validationErrors.Entry.Entity.GetType().Name;
                    Console.WriteLine($"❌ Entidad con errores: {entityName}");

                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"   - Propiedad: {error.PropertyName}");
                        Console.WriteLine($"     Error: {error.ErrorMessage}");
                    }
                }

                throw; // Opcional: relanzar la excepción si necesitas
            }
            return Guid_id;

        }
        public string RegistrarNuevoExpedienteProbosque(Api_Probosque_Data_Proyecto_Post proyecto)
        {
            string Guid_id = Guid.NewGuid().ToString();
            DateTime fechadefault = new DateTime(2000, 1, 1, 0, 0, 0);
            DateTime fechaminima = new DateTime(1980, 1, 1, 0, 0, 0);
            long contareaid = 0;
            long countpoligono = 0;

            Api_Probosque_Data_Proyecto_FincaRegistroPropiedad_Post FincaRegistroPropiedad;
            List<Api_Probosque_Data_Proyecto_personasIndividuales_Post> PersonasIndividuales;
            List<Api_Probosque_Data_Proyecto_Representante_Post> RepresentanteLegal;
            List<Api_Probosque_Data_Proyecto_personasJuridicas_Post> PersonasJuridicas;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_Post> Rodales;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Post> EspeciesForestales;
            Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Informe_Post Informe;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Post>[] PoligonosArray;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Post> Poligonos;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_PoligonosDescuento_Post>[] PoligonosDescuentoArray;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_PoligonosDescuento_Post> PoligonosDescuento;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_Poligonos_Coordenadas_Post> Coordenadas;
            List<Api_Probosque_Data_Proyecto_InformeTecnico_DocumentosPoligonos_Post> DocumentosPoligonos;
            Api_Probosque_Data_Proyecto_InformeTecnico_Post InformeTecnico;

            Tbl_API_Probosque_Expediente tbl_API_Probosque_Expediente;
            Tbl_API_Probosque_Finca tbl_API_Probosque_Finca;
            Tbl_API_Probosque_FincaRegistroPropiedad tbl_API_Probosque_FincaRegistroPropiedad;
            Tbl_API_Probosque_Propietario_PersonaIndividual tbl_API_Probosque_Propietario_PersonaIndividual;
            Tbl_API_Probosque_Propietario_PersonaJuridica tbl_API_Probosque_Propietario_PersonaJuridica;
            Tbl_API_Probosque_RepresentanteLegal tbl_API_Probosque_RepresentanteLegal;
            Tbl_API_Probosque_InformeTecnico tbl_API_Probosque_InformeTecnico;
            Tbl_API_Probosque_EspeciesForestales tbl_API_Probosque_EspeciesForestales;
            Tbl_API_Probosque_Rodales tbl_API_Probosque_Rodales;
            Tbl_API_Probosque_Rodales_Poligonos tbl_API_Probosque_Rodales_Poligonos;
            Tbl_API_Probosque_Rodales_PoligonosDescuento tbl_API_Probosque_Rodales_PoligonosDescuento;
            Tbl_API_Probosque_Rodales_Documentos tbl_API_Probosque_Rodales_Documentos;

            tbl_API_Probosque_Expediente = new Tbl_API_Probosque_Expediente();
            tbl_API_Probosque_Expediente.Guid_id = Guid_id;
            tbl_API_Probosque_Expediente.ProyectoId = proyecto.ProyectoId;
            tbl_API_Probosque_Expediente.Expediente = proyecto.Expediente;
            tbl_API_Probosque_Expediente.Modalidad = proyecto.Modalidad;
            tbl_API_Probosque_Expediente.TipoProyecto = proyecto.TipoProyecto;
            tbl_API_Probosque_Expediente.Region = proyecto.Region;
            tbl_API_Probosque_Expediente.SubRegion = proyecto.SubRegion;
            tbl_API_Probosque_Expediente.UltimaFaseCertificada = proyecto.UltimaFaseCertificada;
            tbl_API_Probosque_Expediente.UltimaAreaCertificada = proyecto.UltimaAreaCertificada;
             tbl_API_Probosque_Expediente.Migrado = false;
            tbl_API_Probosque_Expediente.Solicitud_id = 0;
            db_Intermedia.Tbl_API_Probosque_Expediente.Add(tbl_API_Probosque_Expediente);

            tbl_API_Probosque_Finca = new Tbl_API_Probosque_Finca();
            tbl_API_Probosque_Finca.Guid_id = Guid_id;
            tbl_API_Probosque_Finca.FincaNombre = proyecto.FincaNombre;
            tbl_API_Probosque_Finca.FincaMunicipio = proyecto.FincaMunicipio;
            tbl_API_Probosque_Finca.FincaDepartamento = proyecto.FincaDepartamento;
            tbl_API_Probosque_Finca.FincaUbicacion = proyecto.FincaUbicacion;
            tbl_API_Probosque_Finca.FincaRefX = proyecto.FincaRefX;
            tbl_API_Probosque_Finca.FincaRefY = proyecto.FincaRefY;
            tbl_API_Probosque_Finca.FincaTipoPropiedad = proyecto.FincaTipoPropiedad;
            tbl_API_Probosque_Finca.FincaActaMunicipal = proyecto.FincaActaMunicipal;
            tbl_API_Probosque_Finca.FincaArrendamiento = proyecto.FincaArrendamiento;
            tbl_API_Probosque_Finca.FincaActaNotarial = proyecto.FincaActaNotarial;
            tbl_API_Probosque_Finca.AreaFinca = proyecto.AreaFinca;
            tbl_API_Probosque_Finca.AreaAprobada = proyecto.AreaAprobada;
            db_Intermedia.Tbl_API_Probosque_Finca.Add(tbl_API_Probosque_Finca);

            if (proyecto.FincaRegistroPropiedad != null)
            {
                FincaRegistroPropiedad = proyecto.FincaRegistroPropiedad;
                tbl_API_Probosque_FincaRegistroPropiedad = new Tbl_API_Probosque_FincaRegistroPropiedad();
                tbl_API_Probosque_FincaRegistroPropiedad.Guid_id = Guid_id;
                tbl_API_Probosque_FincaRegistroPropiedad.Id = FincaRegistroPropiedad.Id;
                tbl_API_Probosque_FincaRegistroPropiedad.Numero = FincaRegistroPropiedad.Numero;
                tbl_API_Probosque_FincaRegistroPropiedad.Folio = FincaRegistroPropiedad.Folio;
                tbl_API_Probosque_FincaRegistroPropiedad.Libro = FincaRegistroPropiedad.Libro;
                tbl_API_Probosque_FincaRegistroPropiedad.Municipalidad = FincaRegistroPropiedad.Municipalidad;
                tbl_API_Probosque_FincaRegistroPropiedad.Fecha = FincaRegistroPropiedad.Fecha;
                tbl_API_Probosque_FincaRegistroPropiedad.TipoRegistroPropiedad = FincaRegistroPropiedad.TipoRegistroPropiedad;
                db_Intermedia.Tbl_API_Probosque_FincaRegistroPropiedad.Add(tbl_API_Probosque_FincaRegistroPropiedad);
            }

            if (proyecto.InformeTecnico != null)
            {
                InformeTecnico = proyecto.InformeTecnico;
                tbl_API_Probosque_InformeTecnico = new Tbl_API_Probosque_InformeTecnico();
                tbl_API_Probosque_InformeTecnico.Guid_id = Guid_id;
                tbl_API_Probosque_InformeTecnico.UltimaFaseCertificada = InformeTecnico.UltimaFaseCertificada;
                tbl_API_Probosque_InformeTecnico.NumeroInforme = InformeTecnico.NumeroInforme;
                tbl_API_Probosque_InformeTecnico.PDFInforme = InformeTecnico.PDFInforme;
                tbl_API_Probosque_InformeTecnico.PDFInforme_Local = InformeTecnico.PDFInforme_Local;

                db_Intermedia.Tbl_API_Probosque_InformeTecnico.Add(tbl_API_Probosque_InformeTecnico);

                if (InformeTecnico.Rodales.Count() > 0)
                {
                    Rodales = InformeTecnico.Rodales;

                    foreach (var item in Rodales)
                    {

                        tbl_API_Probosque_Rodales = new Tbl_API_Probosque_Rodales();
                        tbl_API_Probosque_Rodales.Guid_id = Guid_id;
                        tbl_API_Probosque_Rodales.Id = item.Id;
                        tbl_API_Probosque_Rodales.Area = item.Area;

                        db_Intermedia.Tbl_API_Probosque_Rodales.Add(tbl_API_Probosque_Rodales);

                        if (item.EspeciesForestales.Count() > 0)
                        {

                            EspeciesForestales = item.EspeciesForestales;
                            long countEspeciesForestales = 0;

                            foreach (var item2 in EspeciesForestales)
                            {
                                if ((item2.Informe != null) && (item2.Informe != "{}"))
                                {
                                    countEspeciesForestales++;
                                    Informe = JsonConvert.DeserializeObject<Api_Probosque_Data_Proyecto_InformeTecnico_Rodales_EspeciesForestales_Informe_Post>(item2.Informe);
                                    tbl_API_Probosque_EspeciesForestales = new Tbl_API_Probosque_EspeciesForestales();
                                    tbl_API_Probosque_EspeciesForestales.Guid_id = Guid_id;
                                    tbl_API_Probosque_EspeciesForestales.Id = item.Id;
                                    tbl_API_Probosque_EspeciesForestales.Correlativo_id = countEspeciesForestales;
                                    tbl_API_Probosque_EspeciesForestales.Area = item.Area;
                                    tbl_API_Probosque_EspeciesForestales.NombreCientifico = item2.NombreCientifico;
                                    //if (Informe.FechaPlantacion < fechaminima)
                                    //{
                                    //    tbl_API_Probosque_EspeciesForestales.FechaPlantacion = fechadefault;
                                    //}
                                    //else
                                    //{
                                    //}
                                    tbl_API_Probosque_EspeciesForestales.FechaPlantacion = Informe.FechaPlantacion;
                                    tbl_API_Probosque_EspeciesForestales.CicloDeCorta = Informe.CicloDeCorta;
                                    tbl_API_Probosque_EspeciesForestales.DensidadFinal = Informe.DensidadFinal;
                                    tbl_API_Probosque_EspeciesForestales.DensidadInicial = Informe.DensidadInicial;
                                    tbl_API_Probosque_EspeciesForestales.Mixtaje = Informe.Mixtaje;
                                    tbl_API_Probosque_EspeciesForestales.DistanciaES = Informe.DistanciaES;
                                    tbl_API_Probosque_EspeciesForestales.DistanciaEP = Informe.DistanciaEP;
                                    tbl_API_Probosque_EspeciesForestales.DensidadActual = Informe.DensidadActual;
                                    tbl_API_Probosque_EspeciesForestales.Supervivencia = Informe.Supervivencia;
                                    tbl_API_Probosque_EspeciesForestales.PlantasSanas = Informe.PlantasSanas;
                                    tbl_API_Probosque_EspeciesForestales.PlantacionDPA = Informe.PlantacionDPA;
                                    tbl_API_Probosque_EspeciesForestales.PlantacionAltura = Informe.PlantacionAltura;
                                    tbl_API_Probosque_EspeciesForestales.PlantasAfectadas = Informe.PlantasAfectadas;
                                    tbl_API_Probosque_EspeciesForestales.PlantasEnfermedad = Informe.PlantasEnfermedad;
                                    tbl_API_Probosque_EspeciesForestales.PlantasFuego = Informe.PlantasFuego;


                                    db_Intermedia.Tbl_API_Probosque_EspeciesForestales.Add(tbl_API_Probosque_EspeciesForestales);
                                }
                            }

                        }

                    }

                }

                if (InformeTecnico.Poligonos.Length > 0)
                {

                    PoligonosArray = InformeTecnico.Poligonos;
                    for (int z = 0; z < PoligonosArray.Length; z++)
                    {

                        if (PoligonosArray[z].Count() > 0)
                        {

                            Poligonos = PoligonosArray[z];
                            foreach (var item in Poligonos)
                            {

                                if (item.Coordenadas.Count() > 0)
                                {

                                    Coordenadas = item.Coordenadas;
                                    foreach (var item2 in Coordenadas)
                                    {

                                        tbl_API_Probosque_Rodales_Poligonos = new Tbl_API_Probosque_Rodales_Poligonos();
                                        tbl_API_Probosque_Rodales_Poligonos.Guid_id = Guid_id;
                                        tbl_API_Probosque_Rodales_Poligonos.RodalId = item.RodalId;
                                        tbl_API_Probosque_Rodales_Poligonos.PoligonoId = item.PoligonoId;
                                        tbl_API_Probosque_Rodales_Poligonos.Id = item2.Id;
                                        tbl_API_Probosque_Rodales_Poligonos.Orden = item2.Orden;
                                        tbl_API_Probosque_Rodales_Poligonos.GTMX = item2.GTMX;
                                        tbl_API_Probosque_Rodales_Poligonos.GTMY = item2.GTMY;
                                        tbl_API_Probosque_Rodales_Poligonos.Area = item.AreaAprobada;
                                        db_Intermedia.Tbl_API_Probosque_Rodales_Poligonos.Add(tbl_API_Probosque_Rodales_Poligonos);

                                    }

                                }

                                if (item.PoligonosDescuento.Length > 0)
                                {
                                    PoligonosDescuentoArray = item.PoligonosDescuento;
                                    for (int y = 0; y < PoligonosDescuentoArray.Length; y++)
                                    {
                                        contareaid++;
                                        if (PoligonosDescuentoArray[y].Count() > 0)
                                        {
                                            countpoligono = 0;
                                            PoligonosDescuento = PoligonosDescuentoArray[y];
                                            foreach (var item2 in PoligonosDescuento)
                                            {
                                                countpoligono++;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento = new Tbl_API_Probosque_Rodales_PoligonosDescuento();
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.Guid_id = Guid_id;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.RodalId = item.RodalId;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.PoligonoId = item.PoligonoId;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.Id = item2.Id;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.Area_id = contareaid;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.Orden = countpoligono;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.GTMX = item2.GTMX;
                                                tbl_API_Probosque_Rodales_PoligonosDescuento.GTMY = item2.GTMY;
                                                db_Intermedia.Tbl_API_Probosque_Rodales_PoligonosDescuento.Add(tbl_API_Probosque_Rodales_PoligonosDescuento);
                                            }

                                        }
                                    }
                                }


                                //tbl_API_Probosque_Rodales_Poligonos.Id = item.Id
                            }

                        }


                    }

                }

                if (InformeTecnico.DocumentosPoligonos.Count() > 0)
                {
                    long countDocumentosPoligonos = 0;
                    DocumentosPoligonos = InformeTecnico.DocumentosPoligonos;
                    foreach (var item in DocumentosPoligonos)
                    {
                        countDocumentosPoligonos++;
                        tbl_API_Probosque_Rodales_Documentos = new Tbl_API_Probosque_Rodales_Documentos();

                        tbl_API_Probosque_Rodales_Documentos.Guid_id = Guid_id;
                        tbl_API_Probosque_Rodales_Documentos.Correlativo_id = countDocumentosPoligonos;
                        tbl_API_Probosque_Rodales_Documentos.PDFPol = item.PDFPol;
                        tbl_API_Probosque_Rodales_Documentos.PDFPol_Local = item.PDFPol_Local;
                        db_Intermedia.Tbl_API_Probosque_Rodales_Documentos.Add(tbl_API_Probosque_Rodales_Documentos);


                    }


                }





            }

            string propind = "PersonaIndividual";
            string propjur = "PersonaJuridica";
            string tipoprop = proyecto.TipoPropietario;
            if (tipoprop == propind)
            {
                PersonasIndividuales = proyecto.personasIndividuales;

                foreach (var item in PersonasIndividuales)
                {
                    tbl_API_Probosque_Propietario_PersonaIndividual = new Tbl_API_Probosque_Propietario_PersonaIndividual();
                    tbl_API_Probosque_Propietario_PersonaIndividual.Guid_id = Guid_id;
                    tbl_API_Probosque_Propietario_PersonaIndividual.CUI = item.CUI;
                    tbl_API_Probosque_Propietario_PersonaIndividual.Nombres = item.Nombres;
                    tbl_API_Probosque_Propietario_PersonaIndividual.Apellidos = item.Apellidos;

                    db_Intermedia.Tbl_API_Probosque_Propietario_PersonaIndividual.Add(tbl_API_Probosque_Propietario_PersonaIndividual);
                }

            }

            if (tipoprop == propjur)
            {
                PersonasJuridicas = proyecto.personasJuridicas;

                foreach (var item in PersonasJuridicas)
                {
                    tbl_API_Probosque_Propietario_PersonaJuridica = new Tbl_API_Probosque_Propietario_PersonaJuridica();
                    tbl_API_Probosque_Propietario_PersonaJuridica.Guid_id = Guid_id;
                    tbl_API_Probosque_Propietario_PersonaJuridica.Nit = item.Nit;
                    tbl_API_Probosque_Propietario_PersonaJuridica.Nombre = item.Nombre;
                    tbl_API_Probosque_Propietario_PersonaJuridica.TipoPersonaJ = (string)item.TipoPersonaJ;

                    db_Intermedia.Tbl_API_Probosque_Propietario_PersonaJuridica.Add(tbl_API_Probosque_Propietario_PersonaJuridica);
                }
            }

            if (proyecto.Representante.Count() > 0)
            {
                RepresentanteLegal = proyecto.Representante;

                foreach (var item in RepresentanteLegal)
                {
                    tbl_API_Probosque_RepresentanteLegal = new Tbl_API_Probosque_RepresentanteLegal();
                    tbl_API_Probosque_RepresentanteLegal.Guid_id = Guid_id;
                    tbl_API_Probosque_RepresentanteLegal.CUI = item.CUI;
                    tbl_API_Probosque_RepresentanteLegal.Nombres = item.Nombres;
                    tbl_API_Probosque_RepresentanteLegal.Apellidos = item.Apellidos;

                    db_Intermedia.Tbl_API_Probosque_RepresentanteLegal.Add(tbl_API_Probosque_RepresentanteLegal);

                }
            }

            //db_Intermedia.SaveChanges();
            //return Guid_id;

            try
            {
                db_Intermedia.SaveChanges();
            return Guid_id;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                Console.WriteLine("ERROR REAL:");
                Console.WriteLine(ex.Message);

                throw;
            }



        }


        public ActionResult CreateConRegionProbosque()
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

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Procedencia_Probosque == true), "Categoria_id", "Descripcion", 0);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == 0 && obj.Procedencia_Probosque == true), "Sub_Categoria_id", "Descripcion", 0);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == 0 && obj.Procedencia_Probosque == true && obj.Sub_Categoria_id == 0) || (obj.Procedencia_Probosque == true && obj.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", 0);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", 0);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == 0 && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", 0);



            return View();
        }
        public ActionResult CreateConRegionPinpepOld()
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

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Procedencia_PinpepOld == true), "Categoria_id", "Descripcion", 0);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == 0 && obj.Procedencia_PinpepOld == true), "Sub_Categoria_id", "Descripcion", 0);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == 0 && obj.Procedencia_PinpepOld == true && obj.Sub_Categoria_id == 0) || (obj.Procedencia_PinpepOld == true && obj.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", 0);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", 0);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == 0 && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", 0);



            return View();
        }


        [HttpPost]
        public JsonResult GetSubCategoriasProbosque(int Categoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                 where c.Categoria_id == Categoria
                                                                                 && c.Procedencia_Probosque == true
                                                                                 select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");

            return Json(new SelectList(SubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubSubCategoriasProbosque(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                           && c.Procedencia_Probosque == true
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubCategoriasPinpepOld(int Categoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                 where c.Categoria_id == Categoria
                                                                                 && c.Procedencia_PinpepOld == true
                                                                                 select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");

            return Json(new SelectList(SubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubSubCategoriasPinpepOld(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                           && c.Procedencia_PinpepOld == true
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubCategoriasPinpepNew(int Categoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                 where c.Categoria_id == Categoria
                                                                                 && c.Procedencia_PinpepNew == true
                                                                                 select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");

            return Json(new SelectList(SubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubSubCategoriasPinpepNew(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                           && c.Procedencia_PinpepNew == true
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

        }



        class JsonRegionSubRegion
        {
            public int Region_id { get; set; }
            public int SubRegion_id { get; set; }
        }
        public JsonResult VerificarRegionSubRegion(string No_Region, string No_SubRegion)
        {
            JsonRegionSubRegion jsonRegionSubRegion = new JsonRegionSubRegion()
            {
                Region_id = 0,
                SubRegion_id = 0
            };

            Tbl_Gral_SubRegion tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
                                                     where d.No_SubRegion == No_SubRegion && d.Estado_id == true
                                                     select d).FirstOrDefault();

            if (tbl_Gral_SubRegion != null)
            {
                jsonRegionSubRegion.Region_id = tbl_Gral_SubRegion.Region_id;
                jsonRegionSubRegion.SubRegion_id = tbl_Gral_SubRegion.SubRegion_id;
                return Json(jsonRegionSubRegion);
            }

            Tbl_Gral_Region tbl_Gral_Region = (from d in db.Tbl_Gral_Region
                                               where d.No_Region == No_Region
                                               select d).FirstOrDefault();

            if (tbl_Gral_Region != null)
            {
                tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
                                      where d.Region_id == tbl_Gral_Region.Id_Region && d.Estado_id == true
                                      select d).FirstOrDefault();

                jsonRegionSubRegion.Region_id = tbl_Gral_SubRegion.Region_id;
                jsonRegionSubRegion.SubRegion_id = tbl_Gral_SubRegion.SubRegion_id;

                return Json(jsonRegionSubRegion);
            }

            return Json(jsonRegionSubRegion);
        }


        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public long Solicitud_id { get; set; }
            public string guid_id { get; set; }
            public int etapaid { get; set; }
            public decimal etaparutaid { get; set; }
            public int correlativo { get; set; }
        }
        public JsonResult CrearSolicitudProbosque(string Expediente, int Categoria_id, int Sub_Categoria_id, int Sub_Sub_Categoria_id, int Region_id, int SubRegion_id, string DPI_Titular)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha realizado ninguna gestión",
                Solicitud_id = 0
            };

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure();

            try
            {

                string sqlQuery = "exec [SP_MigrarProbosque] @Expediente, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id, @SubRegion_id, @DPI_Titular, @Usuario_id";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                   new SqlParameter { ParameterName = "@Expediente",  Value = Expediente, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Categoria_id",  Value = Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = Sub_Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Region_id",  Value = Region_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@SubRegion_id",  Value = SubRegion_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@DPI_Titular",  Value = DPI_Titular, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db_Intermedia.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                jsonRespuesta.Mensaje = resultFromStoreProcedure.mensaje;
                jsonRespuesta.Result = resultFromStoreProcedure.respuesta;
                jsonRespuesta.Solicitud_id = resultFromStoreProcedure.id;

                if (jsonRespuesta.Result == 1)
                {

                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(jsonRespuesta.Solicitud_id);
                    jsonRespuesta.guid_id = tbl_Sol_Solicitud.Guid_id;
                    jsonRespuesta.etapaid = 1;
                    jsonRespuesta.etaparutaid = 1;
                    jsonRespuesta.correlativo = 1;

                    if (tbl_Sol_Solicitud != null)
                    {


                        Tbl_API_Probosque_Expediente tbl_API_Probosque_Expediente = (from d in db_Intermedia.Tbl_API_Probosque_Expediente
                                                                                     where d.Solicitud_id == jsonRespuesta.Solicitud_id
                                                                                     select d).FirstOrDefault();

                        List<Tbl_API_Probosque_InformeTecnico> tbl_API_Probosque_InformeTecnicos = new List<Tbl_API_Probosque_InformeTecnico>();
                        tbl_API_Probosque_InformeTecnicos = (from d in db_Intermedia.Tbl_API_Probosque_InformeTecnico
                                                             where d.Guid_id == tbl_API_Probosque_Expediente.Guid_id
                                                             select d).ToList();

                        List<Tbl_API_Probosque_Rodales_Documentos> tbl_API_Probosque_Rodales_Documentos = new List<Tbl_API_Probosque_Rodales_Documentos>();
                        tbl_API_Probosque_Rodales_Documentos = (from d in db_Intermedia.Tbl_API_Probosque_Rodales_Documentos
                                                                where d.Guid_id == tbl_API_Probosque_Expediente.Guid_id
                                                                select d).ToList();


                        string PathArchivo = "";
                        string NombreArchivo = "";
                        string ArchivoDescargado = "";
                        string partialpath = "";
                        string almacenamientodefault = "../../Archivos_Generados_Que_Pueden_Borrar/";

                        if (tbl_API_Probosque_InformeTecnicos.Count() > 0)
                        {
                            foreach (var item in tbl_API_Probosque_InformeTecnicos)
                            {
                                if ((item.PDFInforme_Local != null) && (item.PDFInforme_Local != ""))
                                {

                                    ArchivoDescargado = item.PDFInforme_Local;
                                    ArchivoDescargado = item.PDFInforme_Local.Replace(almacenamientodefault, Server.MapPath("~/Archivos_Generados_Que_Pueden_Borrar/"));
                                    NombreArchivo = item.PDFInforme_Local;
                                    NombreArchivo = NombreArchivo.Replace(almacenamientodefault, "");
                                    partialpath = "~/Archivos_Subidos/" + jsonRespuesta.Solicitud_id + "/" + 19 + "/";
                                    PathArchivo = Server.MapPath(partialpath);
                                    NombreArchivo = Path.Combine(PathArchivo, NombreArchivo);
                                    if (!Directory.Exists(Server.MapPath(partialpath)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(partialpath));
                                    }
                                    //Copy(ArchivoDescargado, NombreArchivo, true);
                                    if (System.IO.File.Exists(NombreArchivo))
                                    {
                                        System.IO.File.Delete(NombreArchivo);
                                    }
                                    System.IO.File.Copy(ArchivoDescargado, NombreArchivo);

                                }
                            }
                        }

                        if (tbl_API_Probosque_Rodales_Documentos.Count() > 0)
                        {
                            foreach (var item in tbl_API_Probosque_Rodales_Documentos)
                            {
                                if ((item.PDFPol_Local != null) && (item.PDFPol_Local != ""))
                                {
                                    ArchivoDescargado = item.PDFPol_Local;
                                    ArchivoDescargado = item.PDFPol_Local.Replace(almacenamientodefault, Server.MapPath("~/Archivos_Generados_Que_Pueden_Borrar/"));
                                    NombreArchivo = item.PDFPol_Local;
                                    NombreArchivo = NombreArchivo.Replace(almacenamientodefault, "");
                                    partialpath = "~/Archivos_Subidos/" + jsonRespuesta.Solicitud_id + "/" + 19 + "/";
                                    PathArchivo = Server.MapPath(partialpath);
                                    NombreArchivo = Path.Combine(PathArchivo, NombreArchivo);
                                    if (!Directory.Exists(Server.MapPath(partialpath)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(partialpath));
                                    }
                                    if (System.IO.File.Exists(NombreArchivo))
                                    {
                                        System.IO.File.Delete(NombreArchivo);
                                    }
                                    System.IO.File.Copy(ArchivoDescargado, NombreArchivo);
                                }
                            }
                        }


                        var Tbl_Sol_Rodales = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList();
                        foreach (var ItemRodal in Tbl_Sol_Rodales)
                        {

                            if (ItemRodal.Procedencia_Probosque != true)
                            {
                            Evaluacion_AreaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            }
                            Evaluacion_RegionRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            Evaluacion_AreaProtegidaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            Evaluacion_Colisiones(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                        }
                        var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList();
                        foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                        {
                            Evaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id, tbl_Sol_Solicitud.Solicitud_id);
                        }




                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                jsonRespuesta.Solicitud_id = 0;
                jsonRespuesta.Mensaje = "Error: " + ex;
                jsonRespuesta.Result = 3;
            }

            return Json(jsonRespuesta);
        }
        public JsonResult CrearSolicitudPinpepOld(string Expediente, int Categoria_id, int Sub_Categoria_id, int Sub_Sub_Categoria_id, int Region_id, int SubRegion_id, string DPI_Titular)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha realizado ninguna gestión",
                Solicitud_id = 0
            };

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure();

            try
            {

                string sqlQuery = "exec [SP_MigrarPinpepOld] @Expediente, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id, @SubRegion_id, @DPI_Titular, @Usuario_id";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                   new SqlParameter { ParameterName = "@Expediente",  Value = Expediente, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Categoria_id",  Value = Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = Sub_Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Region_id",  Value = Region_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@SubRegion_id",  Value = SubRegion_id, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@DPI_Titular",  Value = DPI_Titular, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db_Intermedia.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                jsonRespuesta.Mensaje = resultFromStoreProcedure.mensaje;
                jsonRespuesta.Result = resultFromStoreProcedure.respuesta;
                jsonRespuesta.Solicitud_id = resultFromStoreProcedure.id;

                if (jsonRespuesta.Result == 1)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(jsonRespuesta.Solicitud_id);
                    jsonRespuesta.guid_id = tbl_Sol_Solicitud.Guid_id;
                    jsonRespuesta.etapaid = 1;
                    jsonRespuesta.etaparutaid = 1;
                    jsonRespuesta.correlativo = 1;

                    if (tbl_Sol_Solicitud != null)
                    {

                        //De momento solo se llama al expediente pero aún no se ha generado el proceso de mover documentos a la solicitud
                        //debido a que los documentos de Pinpep han presentado fallos en el link y no han permitido la visualización
                        //pendiente de consultar con Samuel para verificar el porqué de este inconveniente
                        //, posible tema de validación de sesión al abrir los enlaces de los documentos
                        Tbl_API_PinpepOld_Expediente tbl_API_PinpepOld_Expediente = (from d in db_Intermedia.Tbl_API_PinpepOld_Expediente
                                                                                     where d.Solicitud_id == jsonRespuesta.Solicitud_id
                                                                                     select d).FirstOrDefault();

                        List<Tbl_API_PinpepOld_InformeTecnico> tbl_API_PinpepOld_InformeTecnicos = new List<Tbl_API_PinpepOld_InformeTecnico>();
                        tbl_API_PinpepOld_InformeTecnicos = (from d in db_Intermedia.Tbl_API_PinpepOld_InformeTecnico
                                                             where d.Guid_id == tbl_API_PinpepOld_Expediente.Guid_id
                                                             select d).ToList();


                        string PathArchivo = "";
                        string NombreArchivo = "";
                        string ArchivoDescargado = "";
                        string partialpath = "";
                        string almacenamientodefault = "../../Archivos_Generados_Que_Pueden_Borrar/";

                        if (tbl_API_PinpepOld_InformeTecnicos.Count() > 0)
                        {
                            foreach (var item in tbl_API_PinpepOld_InformeTecnicos)
                            {
                                if ((item.PDFInforme_Local != null) && (item.PDFInforme_Local != ""))
                                {

                                    ArchivoDescargado = item.PDFInforme_Local;
                                    ArchivoDescargado = item.PDFInforme_Local.Replace(almacenamientodefault, Server.MapPath("~/Archivos_Generados_Que_Pueden_Borrar/"));
                                    NombreArchivo = item.PDFInforme_Local;
                                    NombreArchivo = NombreArchivo.Replace(almacenamientodefault, "");
                                    partialpath = "~/Archivos_Subidos/" + jsonRespuesta.Solicitud_id + "/" + 19 + "/";
                                    PathArchivo = Server.MapPath(partialpath);
                                    NombreArchivo = Path.Combine(PathArchivo, NombreArchivo);
                                    if (!Directory.Exists(Server.MapPath(partialpath)))
                                    {
                                        Directory.CreateDirectory(Server.MapPath(partialpath));
                                    }
                                    //Copy(ArchivoDescargado, NombreArchivo, true);
                                    if (System.IO.File.Exists(NombreArchivo))
                                    {
                                        System.IO.File.Delete(NombreArchivo);
                                    }
                                    System.IO.File.Copy(ArchivoDescargado, NombreArchivo);

                                }
                            }
                        }





                        var Tbl_Sol_Rodales = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList();
                        foreach (var ItemRodal in Tbl_Sol_Rodales)
                        {
                            Evaluacion_AreaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            Evaluacion_RegionRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            Evaluacion_AreaProtegidaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                            Evaluacion_Colisiones(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, tbl_Sol_Solicitud.Solicitud_id);
                        }
                        var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList();
                        foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                        {
                            Evaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id, tbl_Sol_Solicitud.Solicitud_id);
                        }







                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                jsonRespuesta.Solicitud_id = 0;
                jsonRespuesta.Mensaje = "Error: " + ex;
                jsonRespuesta.Result = 3;
            }

            return Json(jsonRespuesta);
        }




        public void Evaluacion_AreaRodalDescuento(long finca_id, int tipo_area_id, long rodal_id, long rodal_descuento_id, long solicitud_id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal_Descuento sol_sol_rodal = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id).First();

            IEnumerable<Tbl_Sol_Rodal_Descuento_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Descuento_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;

            sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
            sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;


            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;

        }

        public void Evaluacion_AreaRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            //ViewBag.Area = "";
            //ViewBag.Region = "Región no encontrada. El Rodal no pertenecea a Guatemala o existe un problema para identificarlo.";
            //ViewBag.AreaProtegida = "El rodal no está sobre ningún área protegida.";
            //ViewBag.Centroide = "No se encontró el centroide del rodal.";
            //ViewBag.Invasion = "";

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };
            int IsPrimera = 0;
            decimal PrimeraX = (Decimal)0.000;
            decimal PrimeraY = (Decimal)0.000;

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                if (IsPrimera == 0)
                {
                    PrimeraX = (Decimal)Item.GTMX;
                    PrimeraY = (Decimal)Item.GTMY;


                }
                IsPrimera = IsPrimera + 1;
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;

            if (sol_sol_rodal.Tipo_de_Area == 2)
            {
                sol_sol_rodal.AreaTotalCalculadaSistema = 0;
                sol_sol_rodal.AreaTotal = 0;
                sol_sol_rodal.GTMX = PrimeraX;
                sol_sol_rodal.GTMY = PrimeraY;
            }
            else
            {
                sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }


            sol_sol_rodal.Longitud_Total = sol_sol_rodal.Longitud_Total ?? 0;




            Tbl_Sol_Finca Tbl_Sol_FincaUpdate = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Finca_Id == finca_id).First();

            Tbl_Sol_FincaUpdate.GTMX = sol_sol_rodal.GTMX;
            Tbl_Sol_FincaUpdate.GTMY = sol_sol_rodal.GTMY;
            try
            {
                if ((Tbl_Sol_FincaUpdate.ConstanciaDePorpiedad_id ?? 0) == 0)
                {
                    Tbl_Sol_FincaUpdate.ConstanciaDePorpiedad_id = 1;
                    Tbl_Sol_FincaUpdate.RegDepartamento_id = 1;
                }
                db.Entry(Tbl_Sol_FincaUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }



            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }


        public void Evaluacion_RegionRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalRegionPoligonoEvaluar(" + sol_sol_rodal.GTMX.ToString() + ", " + sol_sol_rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaRegion> LstRegiones = new List<Tbl_Gral_AreaRegion>();

            LstRegiones = db.Tbl_Gral_AreaRegion.SqlQuery(Query).ToList();

            var coordinatesRegion = new List<Coordinate>() { };

            Polygon polyRegion;

            sol_sol_rodal.AreaRegion_Id = 0;

            foreach (var ItemRegion in LstRegiones)
            {
                IEnumerable<Tbl_Gral_AreaRegion_Poligono> tbl_gral_arearegion_poligono = db.Tbl_Gral_AreaRegion_Poligono.Where(Rodal => Rodal.AreaRegion_id == ItemRegion.AreaRegion_Id);

                coordinatesRegion = new List<Coordinate>() { };

                foreach (var ItemRegionPolig in tbl_gral_arearegion_poligono)
                {
                    coordinatesRegion.Add(new Coordinate(DecimalToSgl_Dbl(ItemRegionPolig.Longitud ?? 0), DecimalToSgl_Dbl(ItemRegionPolig.Latitud ?? 0)));
                }


                polyRegion = new Polygon(coordinatesRegion);

                var Result = polyRegion.Intersection(polyRodal);

                if (Result.Centroid != null)
                {
                    sol_sol_rodal.AreaRegion_Id = ItemRegion.AreaRegion_Id;

                    ViewBag.Region = "Area asociada a la región " + ItemRegion.CodReg + "  Sub_Region: " + ItemRegion.CodSubReg;

                }

            }

            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }

        public void Evaluacion_AreaProtegidaRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalAreaProtegidaPoligonoEvaluar(" + sol_sol_rodal.GTMX.ToString() + ", " + sol_sol_rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaProtegida> LstAreasProtegidas = new List<Tbl_Gral_AreaProtegida>();

            LstAreasProtegidas = db.Tbl_Gral_AreaProtegida.SqlQuery(Query).ToList();

            var coordinatesAreaProtegida = new List<Coordinate>() { };

            Polygon polyAreaProtegida;

            sol_sol_rodal.AreaProtegida_Id = 0;

            if (LstAreasProtegidas.Count() > 0)
            {
                foreach (var ItemAreaProtegida in LstAreasProtegidas)
                {
                    IEnumerable<Tbl_Gral_AreaProtegida_Poligono> tbl_gral_areaprotegida_poligono = db.Tbl_Gral_AreaProtegida_Poligono.Where(Rodal => Rodal.AreaProtegida_Id == ItemAreaProtegida.AreaProtegida_Id);

                    coordinatesAreaProtegida = new List<Coordinate>() { };

                    foreach (var ItemAreaProtegidaPolig in tbl_gral_areaprotegida_poligono)
                    {
                        coordinatesAreaProtegida.Add(new Coordinate(DecimalToSgl_Dbl(ItemAreaProtegidaPolig.Longitud ?? 0), DecimalToSgl_Dbl(ItemAreaProtegidaPolig.Latitud ?? 0)));
                    }

                    polyAreaProtegida = new Polygon(coordinatesAreaProtegida);

                    var Result = polyAreaProtegida.Intersection(polyRodal);

                    sol_sol_rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                    if (Result.Centroid != null)
                    {
                        sol_sol_rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                        ViewBag.Region = "El rodal esta vinculado a un área protegida ";

                    }

                }
            }
            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }

            return;
        }


        public void Evaluacion_Colisiones(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            //Colisión de rodales en la misma solicitud

            IEnumerable<Tbl_Sol_Rodal> RodalesSolicitud = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Tipo_de_Area == tipo_area_id && (Rodal.Rodal_Id != rodal_id || Rodal.Finca_id != finca_id));

            var coordinatesOtrosRodales = new List<Coordinate>() { };
            Polygon polyOtrosRodales;

            foreach (var ItemRodalesSolicitud in RodalesSolicitud)
            {
                IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodalotros_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == ItemRodalesSolicitud.Finca_id && Rodal.Tipo_de_Area == ItemRodalesSolicitud.Tipo_de_Area && Rodal.Rodal_Id == ItemRodalesSolicitud.Rodal_Id);

                coordinatesOtrosRodales = new List<Coordinate>() { };

                foreach (var ItemRodalOtrosPolig in tbl_sol_rodalotros_poligono)
                {
                    coordinatesOtrosRodales.Add(new Coordinate(DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMX ?? 0), DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMY ?? 0)));
                }

                polyOtrosRodales = new Polygon(coordinatesOtrosRodales);


                var Result = polyOtrosRodales.Intersection(polyRodal);

                if (Result.Centroid != null)
                {
                    if (Result.Area > (polyRodal.Area * 0.001))
                    {
                        ViewBag.Invasion = "El rodal esta colisionando o invadiendo área de otro rodal en la misma solicitud.";
                    }
                }
            }

            return;
        }
        public Double DecimalToSgl_Dbl(decimal argument)
        {
            object SingleValue;
            Double DoubleValue;

            // Convert the argument to a float value.
            SingleValue = decimal.ToSingle(argument);

            // Convert the argument to a double value.
            DoubleValue = decimal.ToDouble(argument);

            return DoubleValue;
        }


    }
}
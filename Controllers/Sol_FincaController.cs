using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_FincaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_Finca
        public ActionResult Index(long solicitud_id, string firma)
        {
            decimal TotalArea, TotalAreaSegunFincas;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            var tbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == solicitud_id);


            ViewBag.AreaTotalFincasSegunFincas = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == solicitud_id).Sum(Obj => Obj.AreaTotal);

            ViewBag.TotalAreaARegistrar = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == solicitud_id).Sum(Obj => Obj.AreaARegistrar);

            ViewBag.AreaTotalFincasSegunFincas = ViewBag.AreaTotalFincasSegunFincas ?? 0;


            TotalArea = tbl_sol_solicitud.AreaTotalFincas ?? 0;
            ViewBag.TotalAreaARegistrar = ViewBag.TotalAreaARegistrar ?? 0;

            TotalAreaSegunFincas = ViewBag.AreaTotalFincasSegunFincas;

            ViewBag.FincaMensaje = "";

            if (TotalArea == 0)
            {
                ViewBag.FincaMensaje = "Debe definir el área total según documentos para las fincas a ingresar en la solicitud";
            }
            else
            {
                if ((TotalArea - (Decimal)0.01) > TotalAreaSegunFincas)
                {
                    ViewBag.FincaMensaje = "El total de las áreas según documentos es menor al ingresado en el encabezado.";

                }
                if ((TotalArea + (Decimal)0.01) < TotalAreaSegunFincas)
                {
                    ViewBag.FincaMensaje = "El total de las áreas según documentos es mayor al ingresado en el encabezado.";
                }

            }

            return View(tbl_Sol_Finca.ToList());

        }

        public ActionResult listadoFincas(long solicitud_id, string firma)
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

            var tbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == solicitud_id);

            return View(tbl_Sol_Finca.ToList());

        }

        // GET: Sol_Finca
        public ActionResult IndexFincaRodalDasometricos(long Id, string firma)
        {
            var tbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == Id);

            ViewBag.CantidadFincas = tbl_Sol_Finca.Count();

            ViewBag.Solicitud_id = Id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.SolicitudGuid = tbl_sol_solicitud.Guid_id;

            if (ViewBag.CantidadFincas > 0)
            {

                var tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == Id).First();

                List<Tbl_Gral_Region> tbl_Gral_Regions = (from R in db.Tbl_Gral_Region
                                                          from SR in db.Tbl_Gral_SubRegion
                                                          from RD in db.Tbl_Gral_SubRegionDepartamento
                                                          where RD.Id_Departamento == tbl_Sol_FincaFirst.DepartamentoFinca_Id
                                                            && RD.Id_Municipio == tbl_Sol_FincaFirst.MunicipioFinca_Id
                                                            && SR.SubRegion_id == RD.SubRegion_id
                                                            && R.Id_Region == SR.Region_id
                                                          select R).ToList();

                SelectList Sl_RegionFinca_id = new SelectList(tbl_Gral_Regions, nameof(Tbl_Gral_Region.Id_Region), nameof(Tbl_Gral_Region.Nombre_RegionCompleto), tbl_sol_solicitud.Region_id);


                List<SelectListItem> selectListItems = Sl_RegionFinca_id.GroupBy(x => x.Value)
                                                 .Select(x => x.First())
                                                 .ToList();

                ViewBag.RegionFinca_id = selectListItems;

                List<Tbl_Gral_SubRegion> tbl_Gral_SubRegions = (from R in db.Tbl_Gral_SubRegion
                                                                from RD in db.Tbl_Gral_SubRegionDepartamento
                                                                where RD.Id_Departamento == tbl_Sol_FincaFirst.DepartamentoFinca_Id
                                                                   && RD.Id_Municipio == tbl_Sol_FincaFirst.MunicipioFinca_Id
                                                                   && R.SubRegion_id == RD.SubRegion_id
                                                                select R).ToList();


                var tbl_Gral_SubRegionsFirst = tbl_Gral_SubRegions.First();
                SelectList Sl_SubRegionFinca_id = new SelectList(tbl_Gral_SubRegions, "SubRegion_id", "Nombre_SubRegionCompleto", tbl_sol_solicitud.SubRegion_id);

                ViewBag.SubRegionFinca_id = Sl_SubRegionFinca_id;

                if (tbl_Gral_SubRegions.Count() > 0)
                {
                    try
                    {
                        tbl_sol_solicitud.Region_id = Int32.Parse(Sl_RegionFinca_id.SelectedValue.ToString());
                        tbl_sol_solicitud.SubRegion_id = Int32.Parse(Sl_SubRegionFinca_id.SelectedValue.ToString());
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Mensaje = ex.Message.ToString();
                    }


                }
                else
                {
                    TempData["NombeDepartamento"] = tbl_Sol_FincaFirst.Tbl_Gral_Departamento.Departamento;
                    TempData["NombeMunicipio"] = tbl_Sol_FincaFirst.Tbl_Gral_Municipio.Municipio;
                    return RedirectToAction("../Sol_Finca/SubRegionNoAsignada");
                }


                //var query = from D in db.Tbl_Gral_Departamento
                //            from R in db.Tbl_Gral_Region
                //            from SR in db.Tbl_Gral_SubRegion
                //            from RD in db.Tbl_Gral_SubRegionDepartamento
                //            where RD.Id_Departamento == tbl_Sol_FincaFirst.DepartamentoFinca_Id
                //                && RD.Id_Municipio == tbl_Sol_FincaFirst.MunicipioFinca_Id
                //                && SR.SubRegion_id == RD.SubRegion_id
                //                && R.Id_Region == SR.Region_id
                //                && D.Departamento_id == RD.Id_Departamento
                //            group D by new { D.Departamento_id, D.Departamento } into g
                //            select new
                //            {
                //                Departamento_id = g.Key.Departamento_id,
                //                Departamento = g.Key.Departamento,

                //            };  var query = from D in db.Tbl_Gral_Departamento

                
                var query = from D in db.Tbl_Gral_Departamento  
                            group D by new { D.Departamento_id, D.Departamento } into g
                            select new
                            {
                                Departamento_id = g.Key.Departamento_id,
                                Departamento = g.Key.Departamento,
                          
                            };

                List<Tbl_Gral_Departamento> tbl_Gral_Departamento = query.AsEnumerable()
                    .Select(d => new Tbl_Gral_Departamento
                    {
                        Departamento_id = d.Departamento_id,
                        Departamento = d.Departamento,
                        // include other properties as needed
                    })
                    .ToList();

                ViewBag.Notificacion_Departamento_id = new SelectList(tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_solicitud.Notificacion_Departamento_id);


                //List<Tbl_Gral_Municipio> tbl_Gral_Municipios = (from M in db.Tbl_Gral_Municipio
                //                                                from R in db.Tbl_Gral_SubRegion
                //                                                from RD in db.Tbl_Gral_SubRegionDepartamento
                //                                                where RD.Id_Departamento == tbl_Sol_FincaFirst.DepartamentoFinca_Id
                //                                                   && R.SubRegion_id == RD.SubRegion_id
                //                                                   && M.Municipio_id == RD.Id_Municipio
                //                                                   && M.Departamento_id == RD.Id_Departamento
                //                                                   && RD.SubRegion_id == tbl_Gral_SubRegionsFirst.SubRegion_id
                //                                                select M).ToList();




                List<Tbl_Gral_Municipio> tbl_Gral_Municipios = (from M in db.Tbl_Gral_Municipio                                                               
                                                                select M).ToList();




                ViewBag.Notificacion_Municipio_id = new SelectList(tbl_Gral_Municipios, "Municipio_id", "Municipio", tbl_sol_solicitud.Notificacion_Municipio_id);
                ViewBag.Notificacion_Direccion = tbl_sol_solicitud.Notificacion_Direccion;

                db.Entry(tbl_sol_solicitud).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_solicitud.Notificacion_Departamento_id);
                ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_sol_solicitud.Notificacion_Municipio_id);
                ViewBag.Notificacion_Direccion = tbl_sol_solicitud.Notificacion_Direccion;


                ViewBag.RegionFinca_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_sol_solicitud.Region_id);
                ViewBag.SubRegionFinca_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_sol_solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_sol_solicitud.SubRegion_id);
            }

            return View();

        }

        public ActionResult SubRegionNoAsignada()
        {
            return View();

        }

      

        [HttpPost]
        public JsonResult actualizarArea(decimal areafinca, long solicitud_id, string firma)
        {
            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            if (areafinca <= 0)
            {

                strRespuesta = "El área no puede ser menor o igual a cero.";

                jsonResult = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);
            }


            if (areafinca <= 0)
            {

                strRespuesta = "El área no puede ser menor o igual a cero.";

                jsonResult = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);
            }



            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {


                strRespuesta = "Acceso denegado.";

                jsonResult = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {

                strRespuesta = "Acceso denegado.";

                jsonResult = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);
            }




            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;



            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            //Crejo Math.Round permite redondear a los decimales que le indiques, en este caso 2 decimales
            //tbl_sol_solicitud.AreaTotalFincas = Math.Round(areafinca, 5) ;
            tbl_sol_solicitud.AreaTotalFincas = areafinca;

            tbl_sol_solicitud.swupdatedby = objUs.intUsuario_id;
            tbl_sol_solicitud.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_sol_solicitud.swcreatedbyinterno = false;

            }
            else
            {
                tbl_sol_solicitud.swcreatedbyinterno = true;
            }

            if ((tbl_sol_solicitud.Estado_id == 0) || (tbl_sol_solicitud.Estado_id == 4))
            {
                db.Entry(tbl_sol_solicitud).State = EntityState.Modified;
                db.SaveChanges();
                strRespuesta = "Actualización realizada.";
            }
            else
            {
                strRespuesta = "No se puede actualizar sub-region en solicitud creada.";
            }
            codRespuesta = 1;

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        [HttpPost]
        public JsonResult actualizarregion(int regionid, int subregionid, string Notificacion_Direccion, int Notificacion_Departamento_id, int Notificacion_Municipio_id)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            int codRespuesta = 0;
            string strRespuesta = "";

            string jsonResult;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            tbl_sol_solicitud.Region_id = regionid;
            tbl_sol_solicitud.SubRegion_id = subregionid;

            tbl_sol_solicitud.Notificacion_Direccion = Notificacion_Direccion;
            tbl_sol_solicitud.Notificacion_Departamento_id = Notificacion_Departamento_id;
            tbl_sol_solicitud.Notificacion_Municipio_id = Notificacion_Municipio_id;

            tbl_sol_solicitud.swupdatedby = objUs.intUsuario_id;
            tbl_sol_solicitud.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_sol_solicitud.swcreatedbyinterno = false;
            }
            else
            {
                tbl_sol_solicitud.swcreatedbyinterno = true;
            }

            //No pasa por función ya que solo se requiere validar este estado para correcciones de ser necesario
            if ((tbl_sol_solicitud.Estado_id == 0) || (tbl_sol_solicitud.Estado_id == 4))
            {
                db.Entry(tbl_sol_solicitud).State = EntityState.Modified;
                db.SaveChanges();
                strRespuesta = "Actualización realizada.";
            }
            else
            {
                strRespuesta = "No se permite actualizar la región.";

            }


            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        // GET: Sol_Finca/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Sol_Finca/Create
        public ActionResult Create(string mensaje, long solicitud_id, string firma)
        {

            if (mensaje == "<< Ingrese datos de la siguiente finca a incluir en la solicitud>>")
            {
                ViewBag.Siguiente = mensaje;
                mensaje = "";
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            ViewBag.firma = firma;

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

            Tbl_Sol_Finca sol_sol_finca = new Tbl_Sol_Finca();

            sol_sol_finca.Tbl_Sol_Solicitud = tbl_sol_solicitud;

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Finca.Where(Finca => Finca.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(u => u.Finca_Id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }
            sol_sol_finca.Finca_Id = lngIdt;

            sol_sol_finca.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            sol_sol_finca.LongitudDeLineasTotal = 0;

            sol_sol_finca.DepartamentoFinca_Id = 7;
            sol_sol_finca.RegDepartamento_id = 7;

            sol_sol_finca.MunicipioFinca_Id = 74;
            sol_sol_finca.ConstanciaDePorpiedad_id = 0;


            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", sol_sol_finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", sol_sol_finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == 7), "Municipio_id", "Municipio", sol_sol_finca.MunicipioFinca_Id);


            var tbl_Sol_FincaG = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id);

            ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

            if (ViewBag.CantidadFincas > 0)
            {
                var tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == sol_sol_finca.Solicitud_id).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", sol_sol_finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", sol_sol_finca.MunicipioFinca_Id);

            }


            ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", sol_sol_finca.ConstanciaDePorpiedad_id);

            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", sol_sol_finca.ObjetivoDeLaPlantacion);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);


            sol_sol_finca.swcreatedby = objUs.intUsuario_id;
            sol_sol_finca.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                sol_sol_finca.swcreatedbyinterno = false;

            }
            else
            {
                sol_sol_finca.swcreatedbyinterno = true;

            }

            sol_sol_finca.NoOtrosRegistrosRNF = true;

            ViewBag.mensaje = mensaje;

            return View(sol_sol_finca);
        }

        // POST: Sol_Finca/Create
        [HttpPost]
        public ActionResult Create(Tbl_Sol_Finca tbl_Sol_Finca)
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

            bool blError = false;
            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            ViewBag.Mensaje = "";



            var tbl_Sol_FincaG = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id);

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_Sol_Finca.Solicitud_id);

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Finca", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Agregar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar o adicionar datos de la finca.";
            }

            if (((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 1) || (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 6)) && ((tbl_Sol_Finca.RegFolio ?? "") == "" || (tbl_Sol_Finca.RegLibro ?? "") == "" || (tbl_Sol_Finca.RegNumero ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar folio, finca, libro y departamento del registro.";
            }

            if ((tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha != null) && (tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha > DateTime.Now))
            {
                blError = true;
                ViewBag.Mensaje = "Error: La fecha de la constancia de propiedad debe ser menor a la fecha actual.";
            }

            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 2) && ((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 3) && (((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 4) && (((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario, numero de escritura y fecha.";
            }

            if (((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 5) || (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 7)) && ((tbl_Sol_Finca.ConstanciaPropiedadDescripcion ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar los documentos que constaten la acreditación de la propiedad.";
            }


            if (tbl_Sol_Finca.AreaTotal < tbl_Sol_Finca.AreaARegistrar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: No puede registrar un área mayor al área de la finca.";
            }

            if (blError == false)
            {
                if (tbl_Sol_Finca.NoOtrosRegistrosRNF == true)
                {
                    tbl_Sol_Finca.Registros = "";
                }

                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 1)
                {
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 2)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 3)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 4)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 5)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    //tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 6)
                {
                    tbl_Sol_Finca.RegDepartamento_id = 31;
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }

                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 7)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                }
                try
                {
                    if (ModelState.IsValid)
                    {

                        long lngIdt = 0;

                        try
                        {
                            lngIdt = db.Tbl_Sol_Finca.Where(Finca => Finca.Solicitud_id == tbl_Sol_Finca.Solicitud_id).Max(u => u.Finca_Id);
                            lngIdt++;
                        }
                        catch
                        {
                            lngIdt = 1;
                        }

                        tbl_Sol_Finca.Finca_Id = lngIdt;

                        db.Tbl_Sol_Finca.Add(tbl_Sol_Finca);
                        db.SaveChanges();

                        if (tbl_sol_solicitud.Categoria_id == 6)
                        {
                            return RedirectToAction("Edit", "Sol_Finca", new { fincaId = lngIdt, solicitud_id = tbl_Sol_Finca.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                        }

                        return RedirectToAction("../Sol_Finca/Create", new { mensaje = "<< Ingrese datos de la siguiente finca a incluir en la solicitud>>", solicitud_id = tbl_Sol_Finca.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });


                    }

                }
                catch
                {
                    ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_Sol_Finca.RegDepartamento_id);
                    ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                    ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);


                    ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

                    if (ViewBag.CantidadFincas > 0)
                    {
                        Tbl_Sol_Finca tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id).First();

                        ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                        ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);

                    }

                    ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", tbl_Sol_Finca.ConstanciaDePorpiedad_id);
                    ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_Sol_Finca.ObjetivoDeLaPlantacion);

                    ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);

                    return View(tbl_Sol_Finca);
                }
            }
            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_Sol_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);

            ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

            if (ViewBag.CantidadFincas > 0)
            {
                Tbl_Sol_Finca tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);

            }




            ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", tbl_Sol_Finca.ConstanciaDePorpiedad_id);

            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_Sol_Finca.ObjetivoDeLaPlantacion);
            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);

            return View(tbl_Sol_Finca);

        }


        public ActionResult Edit(long fincaId, long solicitud_id, string firma)
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


            long varSolicitudid = solicitud_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            Tbl_Sol_Finca sol_sol_finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_Id == fincaId).First();

            var tbl_Sol_FincaG = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == sol_sol_finca.Solicitud_id);


            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", sol_sol_finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", sol_sol_finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == sol_sol_finca.DepartamentoFinca_Id), "Municipio_id", "Municipio", sol_sol_finca.MunicipioFinca_Id); ;


            ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

            if (ViewBag.CantidadFincas > 1)
            {
                Tbl_Sol_Finca tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == sol_sol_finca.Solicitud_id).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", sol_sol_finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", sol_sol_finca.MunicipioFinca_Id);

            }

            ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", sol_sol_finca.ConstanciaDePorpiedad_id);
            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", sol_sol_finca.ObjetivoDeLaPlantacion);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", sol_sol_finca.CategoriaSIGAP_Id);

            sol_sol_finca.swcreatedby = objUs.intUsuario_id;
            sol_sol_finca.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                sol_sol_finca.swcreatedbyinterno = false;
            }
            else
            {
                sol_sol_finca.swcreatedbyinterno = true;
            }

            return View(sol_sol_finca);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_Sol_Finca tbl_Sol_Finca)
        {
            bool blError = false;
            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            ViewBag.Mensaje = "";

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
            var tbl_Sol_FincaG = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id);


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_Sol_Finca.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Finca", boolEsInterno).FirstOrDefault();


            if (!(bool)permisos.Editar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar o adicionar datos de la finca.";
            }

            if (((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 1) || (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 6)) && ((tbl_Sol_Finca.RegFolio ?? "") == "" || (tbl_Sol_Finca.RegLibro ?? "") == "" || (tbl_Sol_Finca.RegNumero ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar folio, finca, libro y departamento del registro.";
            }

            if ((tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha != null) && (tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha > DateTime.Now))
            {
                blError = true;
                ViewBag.Mensaje = "Error: La fecha de la constancia de propiedad debe ser menor a la fecha actual.";
            }


            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 2) && ((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 3) && (((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 4) && (((tbl_Sol_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario, numero de escritura y fecha.";
            }

            if (((tbl_Sol_Finca.ConstanciaDePorpiedad_id == 5) || (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 7)) && ((tbl_Sol_Finca.ConstanciaPropiedadDescripcion ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar los documentos que constaten la acreditación de la propiedad.";
            }


            if (tbl_Sol_Finca.AreaTotal < tbl_Sol_Finca.AreaARegistrar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: No puede registrar un área mayor al área de la finca.";
            }


            tbl_Sol_Finca.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_Finca.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_Sol_Finca.swupdatedbyinterno = false;

            }
            else
            {
                tbl_Sol_Finca.swupdatedbyinterno = true;

            }




            if (blError == false)
            {

                if (tbl_Sol_Finca.NoOtrosRegistrosRNF == true)
                {
                    tbl_Sol_Finca.Registros = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 1)
                {
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 2)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 3)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 4)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 5)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    //tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 6)
                {
                    tbl_Sol_Finca.RegDepartamento_id = 31;
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_Sol_Finca.ActaNotarial_Notario = "";
                    tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_Sol_Finca.ConstanciaDePorpiedad_id == 7)
                {
                    tbl_Sol_Finca.RegFolio = "";
                    tbl_Sol_Finca.RegNumero = "";
                    tbl_Sol_Finca.RegLibro = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    //tbl_Sol_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_Sol_Finca.ActaNotarial_Notario = "";
                    //tbl_Sol_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_Sol_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }



                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(tbl_Sol_Finca).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["FincaMessage"] = "Registro actualizado con éxito";
                        return RedirectToAction("Edit", "Sol_Finca", new { fincaId = tbl_Sol_Finca.Finca_Id, solicitud_id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });

                    }

                }
                catch
                {
                    ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_Sol_Finca.RegDepartamento_id);
                    ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                    ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);



                    ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

                    if (ViewBag.CantidadFincas > 1)
                    {
                        Tbl_Sol_Finca tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id).First();

                        ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                        ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);

                    }



                    ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", tbl_Sol_Finca.ConstanciaDePorpiedad_id);
                    ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_Sol_Finca.ObjetivoDeLaPlantacion);
                    ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_Sol_Finca.CategoriaSIGAP_Id);

                    return View(tbl_Sol_Finca);
                }
            }
            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_Sol_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);



            ViewBag.CantidadFincas = tbl_Sol_FincaG.Count();

            if (ViewBag.CantidadFincas > 1)
            {
                Tbl_Sol_Finca tbl_Sol_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_Sol_Finca.Solicitud_id).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_Sol_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_Sol_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_Sol_Finca.MunicipioFinca_Id);

            }

            ViewBag.ConstanciaDePorpiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad.Where(Obj => Obj.ConstanciaDePorpiedad_id != 7 || tbl_sol_solicitud.Categoria_id == 6), "ConstanciaDePorpiedad_id", "Descripcion", tbl_Sol_Finca.ConstanciaDePorpiedad_id);
            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_Sol_Finca.ObjetivoDeLaPlantacion);
            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_Sol_Finca.CategoriaSIGAP_Id);

            return View(tbl_Sol_Finca);

        }


        public ActionResult Borrar(long fincaId, long solicitud_id, string firma)
        {

            long varSolicitudid = solicitud_id;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            int FincasMayores = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_Id > fincaId).Count();

            if (FincasMayores > 0)
            {
                return RedirectToAction("../Home/BorrarFincasPosteriores");
            }


            try
            {
                Tbl_Sol_Finca tbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_Id == fincaId).First();
                db.Tbl_Sol_Finca.Remove(tbl_Sol_Finca);
                db.SaveChanges();
                return RedirectToAction("../Home/RegistroEliminado");
            }
            catch (Exception exe)
            {
                return RedirectToAction("../Home/RegistroFincaNoEliminado");
            }


        }


    }
}




//foreach (var Item in tbl_sol_rodal_poligono)
//{
//    coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
//}


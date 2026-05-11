using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_RegionSubRegionController : Controller
    {

        db_RNFEntities db = new db_RNFEntities();
        List<Tbl_Gral_Region> Region = new List<Tbl_Gral_Region>();
        List<Tbl_Gral_SubRegion> SubRegion = new List<Tbl_Gral_SubRegion>();
        // GET: Sol_RegionSubRegion
        public ActionResult Index()
        {


            return View();
        }



        void ObtenerRegionSubRegion(int Departamento_id, int Municipio_id)
        {
            Region = (from R in db.Tbl_Gral_Region
                      from SR in db.Tbl_Gral_SubRegion
                      from RD in db.Tbl_Gral_SubRegionDepartamento
                      where RD.Id_Departamento == Departamento_id
                         && RD.Id_Municipio == Municipio_id
                         && SR.SubRegion_id == RD.SubRegion_id
                         && R.Id_Region == SR.Region_id
                      select R).ToList();
            SubRegion = (from R in db.Tbl_Gral_SubRegion
                         from RD in db.Tbl_Gral_SubRegionDepartamento
                         where RD.Id_Departamento == Departamento_id
                            && RD.Id_Municipio == Municipio_id
                            && R.SubRegion_id == RD.SubRegion_id
                         select R).ToList();
        }

        public ActionResult IndexEmpresaEntidad()
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_sol_solicitud.Solicitud_id);

            if (((tbl_Sol_Empresa_Entidad.Tipo_Industria_id??0) == 2) || ((tbl_sol_solicitud.Categoria_id == 5) && (tbl_sol_solicitud.Sub_Categoria_id == 3)))
            {
                ObtenerRegionSubRegion((int)tbl_Sol_Empresa_Entidad.DepartamentoEmpresaMovilEntidad_id, (int)tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidadMovil_id);
            }
            else
            {
                ObtenerRegionSubRegion((int)tbl_Sol_Empresa_Entidad.DepartamentoEmpresaEntidad_id, (int)(tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidad_id??0));
            }

            SelectList SL_RegionEmpresa_id = new SelectList(Region, "Id_Region", "Nombre_Region", tbl_sol_solicitud.Region_id);
            SelectList SL_SubRegionEmpresa_id = new SelectList(SubRegion, "SubRegion_id", "Nombre_SubRegion", tbl_sol_solicitud.SubRegion_id);
            ViewBag.RegionEmpresa_id = SL_RegionEmpresa_id;

            ViewBag.SubRegionEmpresa_id = SL_SubRegionEmpresa_id;


            if (Region.Count() > 0)
            {
                tbl_sol_solicitud.Region_id = int.Parse(SL_RegionEmpresa_id.SelectedValue.ToString());
                tbl_sol_solicitud.SubRegion_id = int.Parse(SL_SubRegionEmpresa_id.SelectedValue.ToString());

            }

            db.Entry(tbl_sol_solicitud).State = EntityState.Modified;
            db.SaveChanges();

            if ((tbl_Sol_Empresa_Entidad.Tipo_Industria_id != 2) && (tbl_sol_solicitud.Categoria_id == 5 && tbl_sol_solicitud.Sub_Categoria_id != 3 )) // Movil
            {
                tbl_Sol_Empresa_Entidad.DepartamentoEmpresaMovilEntidad_id = tbl_Sol_Empresa_Entidad.DepartamentoEmpresaEntidad_id;
                tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidadMovil_id = tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidad_id;
            }

            //List<Tbl_Gral_Departamento> tbl_Gral_Departamento = (from D in db.Tbl_Gral_Departamento
            //                                                         from R in db.Tbl_Gral_Region
            //                                                         from SR in db.Tbl_Gral_SubRegion
            //                                                         from RD in db.Tbl_Gral_SubRegionDepartamento
            //                                                         where RD.Id_Departamento == (tbl_Sol_Empresa_Entidad.DepartamentoEmpresaMovilEntidad_id ?? tbl_Sol_Empresa_Entidad.DepartamentoEmpresaEntidad_id)
            //                                                            && RD.Id_Municipio == (tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidadMovil_id ??tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidad_id)
            //                                                            && SR.SubRegion_id == RD.SubRegion_id
            //                                                            && R.Id_Region == SR.Region_id
            //                                                            && D.Departamento_id == RD.Id_Departamento
            //                                                         select D).ToList();

            //    List<Tbl_Gral_Municipio> tbl_Gral_Municipios = (from M in db.Tbl_Gral_Municipio
            //                                                    from R in db.Tbl_Gral_SubRegion
            //                                                    from RD in db.Tbl_Gral_SubRegionDepartamento
            //                                                    where RD.Id_Departamento == (tbl_Sol_Empresa_Entidad.DepartamentoEmpresaMovilEntidad_id ?? tbl_Sol_Empresa_Entidad.DepartamentoEmpresaEntidad_id)
            //                                                       && R.SubRegion_id == RD.SubRegion_id
            //                                                       && M.Municipio_id == RD.Id_Municipio
            //                                                       && M.Departamento_id == RD.Id_Departamento
            //                                                    select M).ToList(); 



            List<Tbl_Gral_Departamento> tbl_Gral_Departamento = (from D in db.Tbl_Gral_Departamento
                                                                 select D).ToList();


            List<Tbl_Gral_Municipio> tbl_Gral_Municipios = (from M in db.Tbl_Gral_Municipio
                                                            select M).ToList();







            ViewBag.Notificacion_Departamento_id = new SelectList(tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(tbl_Gral_Municipios, "Municipio_id", "Municipio", tbl_sol_solicitud.Notificacion_Municipio_id);
            ViewBag.Notificacion_Direccion = tbl_sol_solicitud.Notificacion_Direccion;


            return View();
        }

        public ActionResult IndexRegionSubRegionGlobal()
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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_sol_solicitud.Solicitud_id);


            ObtenerRegionSubRegion((int)tbl_Sol_Empresa_Entidad.DepartamentoEmpresaEntidad_id, (int)tbl_Sol_Empresa_Entidad.MunicipioEmpresaEntidad_id);

            ViewBag.RegionGlobal_id = new SelectList(Region, "Id_Region", "Nombre_Region", tbl_sol_solicitud.Region_id);

            ViewBag.SubRegionGlobal_id = new SelectList(SubRegion, "SubRegion_id", "Nombre_SubRegion", tbl_sol_solicitud.SubRegion_id);

            return View();
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


    }
}
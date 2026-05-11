using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace RNF_Web.Controllers
{
    public class Sol_Empresa_EntidadController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();
        long lngSolicitud_id;
        int intSubCategoria;

        private string strsection_0 = "";
        private string strsection_1 = "";
        private string strsection_2 = "";
        private string strsection_3 = "";
        private string strsection_4 = "";
        private string strsection_5 = "";

        private bool boolsection_0 = true;
        private bool boolsection_1 = false;
        private bool boolsection_2 = false;
        private bool boolsection_3 = false;
        private bool boolsection_4 = false;
        private bool boolsection_5 = false;

        // GET: Sol_Empresa_Entida/Create
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
            ViewBag.Session = objUs;
            long id = solicitud_id;


            int intTbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).Count();
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            intSubCategoria = (int)db.Tbl_Sol_Solicitud.Where(Obj => Obj.Solicitud_id == id).First().Sub_Categoria_id;

            Tbl_Sol_Empresa_Entidad tbl_sol_empresa_entidad;            
            ViewBag.TieneEntidad = 0;
            ViewBag.SubCategoria = 0;


                    


                if (intTbl_Sol_Empresa_Entidad > 0)
                {
                    tbl_sol_empresa_entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).First();


                    tbl_sol_empresa_entidad.swupdatedby = objUs.intUsuario_id;
                    tbl_sol_empresa_entidad.swdateupdated = DateTime.Now;
                    ViewBag.TieneEntidad = 1;
                    ViewBag.SubCategoria = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Solicitud_id == id).First().Sub_Categoria_id;


                    if (objUs.EsInterno != 1)
                    {
                        tbl_sol_empresa_entidad.swupdatedbyinterno = false;

                    }
                    else
                    {
                        tbl_sol_empresa_entidad.swupdatedbyinterno = true;
                    }
                }
                else
                {

                    tbl_sol_empresa_entidad = new Tbl_Sol_Empresa_Entidad();

                    tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id = tbl_Sol_Solicitud.DepartamentoSolicitud_id ?? 7;

                    tbl_sol_empresa_entidad.MunicipioEmpresaEntidad_id = tbl_Sol_Solicitud.MunicipioSolicitud_id ?? 74;

                    tbl_sol_empresa_entidad.Solicitud_id = solicitud_id;

                    tbl_sol_empresa_entidad.swcreatedby = objUs.intUsuario_id;
                    tbl_sol_empresa_entidad.swdatecreated = DateTime.Now;

                    tbl_sol_empresa_entidad.GTMX = (decimal?)TempData["GR_GTMX"];
                    tbl_sol_empresa_entidad.GTMY = (decimal?)TempData["GR_GTMY"];

                    if (objUs.EsInterno != 1)
                    {
                        tbl_sol_empresa_entidad.swcreatedbyinterno = false;
                    }
                    else
                    {
                        tbl_sol_empresa_entidad.swcreatedbyinterno = true;
                    }

                }

                //if (Session[Constants.session_Tbl_Sol_Empresa_Entidad] != null)
                //{
                //    tbl_sol_empresa_entidad = (Tbl_Sol_Empresa_Entidad)Session[Constants.session_Tbl_Sol_Empresa_Entidad];
                //}

                ViewBag.Tipo_Industria_id = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes", tbl_sol_empresa_entidad.Tipo_Industria_id);
                ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id);
                ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id), "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidad_id);

                ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaMovilEntidad_id);
                ViewBag.MunicipioEmpresaEntidadMovil_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidadMovil_id);

                ViewBag.lstTipoIndustria = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes");

                if (intSubCategoria == 1)
                {
                    ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
                    ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
                    ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
                }

                setViews(id);

                ViewBag.section_0 = boolsection_0;
                ViewBag.section_1 = boolsection_1;
                ViewBag.section_2 = boolsection_2;
                ViewBag.section_3 = boolsection_3;
                ViewBag.section_4 = boolsection_4;
                ViewBag.section_5 = boolsection_5;

                ViewBag.section_Link_0 = strsection_0;
                ViewBag.section_Link_1 = strsection_1;
                ViewBag.section_Link_2 = strsection_2;
                ViewBag.section_Link_3 = strsection_3;
                ViewBag.section_Link_4 = strsection_4;
                ViewBag.section_Link_5 = strsection_5;

                List<fc_RNF_Inscripcion_Vinculada_Result> fc_RNF_Inscripcion_Vinculada_Results = db.fc_RNF_Inscripcion_Vinculada().ToList() ?? new List<fc_RNF_Inscripcion_Vinculada_Result>();
                ViewBag.RNF_Inscripcion_Vinculada = new SelectList(fc_RNF_Inscripcion_Vinculada_Results, nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), tbl_sol_empresa_entidad.RNF_Inscripcion_Vinculada);

           

            return View(tbl_sol_empresa_entidad);
          



        }
        // POST: Sol_Empresa_Entida/Create
        [HttpPost]
        public ActionResult Create(Tbl_Sol_Empresa_Entidad tbl_sol_empresa_entidad)
        {
            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

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
            string controllerName = Request.RequestContext.RouteData.Values["controller"].ToString();

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_empresa_entidad.Solicitud_id);

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, controllerName, boolEsInterno).FirstOrDefault();

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            if ((permisos.Agregar == false) || (permisos.Editar == false))
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
            }
            else
            {
                //Session[Constants.session_Tbl_Sol_Empresa_Entidad] = null;

                if (ModelState.IsValid)
                {

                    long id = (long)Session[Constants.session_Solicitud];

                    int intTbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).Count();

                    if (intTbl_Sol_Empresa_Entidad == 0)
                    {
                        db.Tbl_Sol_Empresa_Entidad.Add(tbl_sol_empresa_entidad);
                        db.SaveChanges();
                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });


                    }
                    else
                    {

                        db.Entry(tbl_sol_empresa_entidad).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        ViewBag.TieneEntidad = 1;
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";

                //Session[Constants.session_Tbl_Sol_Empresa_Entidad] = (Tbl_Sol_Empresa_Entidad)tbl_sol_empresa_entidad;
            }

            ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id);
            ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidad_id);

            ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaMovilEntidad_id);
            ViewBag.MunicipioEmpresaEntidadMovil_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidadMovil_id);


            decimal SolicitudTipo_id = tbl_sol_solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_solicitud.SolicitudTipo_id);


            //if ((SolicitudTipo_id > 0) && ((tbl_sol_empresa_entidad.RNF_Inscripcion_Vinculada??"").Trim() != ""))
            //{


            //}

            List<fc_RNF_Inscripcion_Vinculada_Result> fc_RNF_Inscripcion_Vinculada_Results = db.fc_RNF_Inscripcion_Vinculada().ToList() ?? new List<fc_RNF_Inscripcion_Vinculada_Result>();
            ViewBag.RNF_Inscripcion_Vinculada = new SelectList(fc_RNF_Inscripcion_Vinculada_Results, nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), tbl_sol_empresa_entidad.RNF_Inscripcion_Vinculada);

            ViewBag.SolicitudId = tbl_sol_empresa_entidad.Solicitud_id;
            //return RedirectToAction("/Sol_Empresa_Entidad/Create", new { id = tbl_sol_empresa_entidad.Solicitud_id });
            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });


        }

        // GET: Sol_Empresa_Entida/Create
        public ActionResult CreateTecnico(long solicitud_id, string firma, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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

            long id = solicitud_id;

            int intTbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).Count();
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.firma = tbl_Sol_Solicitud.Guid_id;


            Session[Constants.session_Solicitud] = tbl_Sol_Solicitud.Solicitud_id;

            intSubCategoria = (int)db.Tbl_Sol_Solicitud.Where(Obj => Obj.Solicitud_id == id).First().Sub_Categoria_id;

            Tbl_Sol_Empresa_Entidad tbl_sol_empresa_entidad;
            ViewBag.TieneEntidad = 0;
            ViewBag.SubCategoria = 0;


            tbl_sol_empresa_entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == id).First();


            ViewBag.Tipo_Industria_id = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes", tbl_sol_empresa_entidad.Tipo_Industria_id);
            ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id);
            ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidad_id);

            ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaMovilEntidad_id);
            ViewBag.MunicipioEmpresaEntidadMovil_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidadMovil_id);

            ViewBag.lstTipoIndustria = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes");

            if (intSubCategoria == 1)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            setViewsTecnico(id);

            ViewBag.section_0 = boolsection_0;
            ViewBag.section_1 = boolsection_1;
            ViewBag.section_2 = boolsection_2;
            ViewBag.section_3 = boolsection_3;
            ViewBag.section_4 = boolsection_4;
            ViewBag.section_5 = boolsection_5;

            ViewBag.section_Link_0 = strsection_0;
            ViewBag.section_Link_1 = strsection_1;
            ViewBag.section_Link_2 = strsection_2;
            ViewBag.section_Link_3 = strsection_3;
            ViewBag.section_Link_4 = strsection_4;
            ViewBag.section_Link_5 = strsection_5;

            Session[Constants.session_Solicitud] = tbl_Sol_Solicitud.Solicitud_id;

            ConvertirGTMModel ConvertirGTMmodel = new ConvertirGTMModel();

            ViewBag.Coordenada = ConvertirGTMmodel.ObtenerDireccionGoogleMaps(tbl_sol_empresa_entidad.GTMX ?? 0, tbl_sol_empresa_entidad.GTMY ?? 0);

            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View(tbl_sol_empresa_entidad);

        }

        [HttpPost]
        public ActionResult CreateTecnico(Tbl_Sol_Empresa_Entidad tbl_sol_empresa_entidad, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {


            ViewBag.etapa_id = etapa_id;

            ViewBag.etaparuta_id = etaparuta_id;

            ViewBag.correlativoetapa_id = correlativoetapa_id;

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_empresa_entidad.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            ViewBag.solicitud_id = tbl_sol_solicitud.Solicitud_id;

            //Session[Constants.session_Tbl_Sol_Empresa_Entidad] = null;

            Tbl_Sol_Empresa_Entidad tbl_sol_empresa_entidadGrabar = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_sol_empresa_entidad.Solicitud_id).First();

            tbl_sol_empresa_entidadGrabar.GTMX_Tecnico = tbl_sol_empresa_entidad.GTMX_Tecnico;
            tbl_sol_empresa_entidadGrabar.GTMY_Tecnico = tbl_sol_empresa_entidad.GTMY_Tecnico;

            tbl_sol_empresa_entidadGrabar.email = "pruebaEntidad@inab.gob.gt";
            
            try
            {
                db.Entry(tbl_sol_empresa_entidadGrabar).State = EntityState.Modified;
                db.SaveChanges();
            }
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}




            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    var entityName = entityError.Entry.Entity.GetType().Name;
                    Console.WriteLine($"Entidad: {entityName}");

                    foreach (var validationError in entityError.ValidationErrors)
                    {
                        Console.WriteLine($" - Propiedad: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }

                throw; // Opcional: relanza la excepción si quieres detener la ejecución
            }


            ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaEntidad_id);
            ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidad_id);

            ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_empresa_entidad.DepartamentoEmpresaMovilEntidad_id);
            ViewBag.MunicipioEmpresaEntidadMovil_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_sol_empresa_entidad.MunicipioEmpresaEntidadMovil_id);

            ViewBag.SolicitudId = tbl_sol_empresa_entidad.Solicitud_id;
            return RedirectToAction("CreateTecnico", "Sol_Empresa_Entidad", new { solicitud_id = tbl_sol_empresa_entidad.Solicitud_id, firma = tbl_sol_solicitud.Guid_id, etapa_id = etapa_id, etaparuta_id = etaparuta_id, correlativoetapa_id = correlativoetapa_id });
        }

        //int etapa_id, decimal etaparuta_id, int correlativoetapa_id
        void setViewsTecnico(long? id)
        {
            lngSolicitud_id = (long)id;

            ViewBag.lngSolicitud = id;
            ViewBag.section_I = false;

            //	0	-- Seleccione una sub-categoría ---
            //	1	Industria Forestal
            //	2	Deposito Forestal
            //	3	Centro de Acopio
            //	4	Viveros Forestales
            //	5	Exportadoras e Importadoras de Productos Forestales
            //	6	Productos Forestales no Maderables
            //	7	Repobladoras Forestales
            //	8	Consultoras Forestales

            boolsection_1 = false;
            strsection_1 = "";

            boolsection_2 = false;
            strsection_2 = "";

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);
            if ((id == null) || (id == 0))
            {

                strsection_0 = "/Sol_TipoRegistro/CreateTecnico";
                boolsection_0 = true;

            }
            else
            {
                strsection_0 = "/Sol_TipoRegistro/CreateTecnico";
                boolsection_0 = true;
            }

            fc_Sol_EmpresasForestales_Result PermisosEmpresasForestales = (from d in db.fc_Sol_EmpresasForestales(tbl_sol_solicitud.Solicitud_id)
                                                                           select d).FirstOrDefault();
            if (PermisosEmpresasForestales != null)
            {
                boolsection_1 = (bool)PermisosEmpresasForestales.Actividades_Empresa;
                boolsection_2 = (bool)PermisosEmpresasForestales.Materia_Prima;
                boolsection_3 = (bool)PermisosEmpresasForestales.Maquinaria;
                boolsection_4 = (bool)PermisosEmpresasForestales.Vivero_Forestal;
                boolsection_5 = (bool)PermisosEmpresasForestales.Motosierras;
                if (boolsection_1)
                {
                    strsection_1 = "/Sol_Empresa_Entidad_TecnicoActividad/Create?SolicitudId=" + id;
                }
                if (boolsection_2)
                {
                    strsection_2 = "/Sol_Empresa_Entidad_TecnicoMateriaPrima/Create?SolicitudId=" + id;
                }
                if (boolsection_3)
                {
                    strsection_3 = "/Sol_Empresa_Entidad_TecnicoMaquinariaUtilizada/Create?SolicitudId=" + id;
                }
                if (boolsection_4)
                {
                    strsection_4 = "/Sol_Empresa_Entidad_TecnicoViveroForestal/Index?SolicitudId=" + id;
                }
                if (boolsection_5)
                {
                    strsection_5 = "/Sol_Motosierra_MarcaModelo/Create?Solicitud_id=" + id;
                }

            }
        }

        void setViews(long? id)
        {
            lngSolicitud_id = (long)id;

            ViewBag.lngSolicitud = id;
            ViewBag.section_I = false;

            //	0	-- Seleccione una sub-categoría ---
            //	1	Industria Forestal
            //	2	Deposito Forestal
            //	3	Centro de Acopio
            //	4	Viveros Forestales
            //	5	Exportadoras e Importadoras de Productos Forestales
            //	6	Productos Forestales no Maderables
            //	7	Repobladoras Forestales
            //	8	Consultoras Forestales

            boolsection_1 = false;
            strsection_1 = "";

            boolsection_2 = false;
            strsection_2 = "";

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);
            if ((id == null) || (id == 0))
            {

                strsection_0 = "/Sol_TipoRegistro/Create";
                boolsection_0 = true;

            }
            else
            {
                strsection_0 = "/Sol_TipoRegistro/Create";
                boolsection_0 = true;
            }

            fc_Sol_EmpresasForestales_Result PermisosEmpresasForestales = (from d in db.fc_Sol_EmpresasForestales(tbl_sol_solicitud.Solicitud_id)
                                                                           select d).FirstOrDefault();
            if (PermisosEmpresasForestales != null)
            {
                boolsection_1 = (bool)PermisosEmpresasForestales.Actividades_Empresa;
                boolsection_2 = (bool)PermisosEmpresasForestales.Materia_Prima;
                boolsection_3 = (bool)PermisosEmpresasForestales.Maquinaria;
                boolsection_4 = (bool)PermisosEmpresasForestales.Vivero_Forestal;
                boolsection_5 = (bool)PermisosEmpresasForestales.Motosierras;
                if (boolsection_1)
                {
                    strsection_1 = "/Sol_Empresa_Entidad_Actividad/Create?SolicitudId=" + id;
                }
                if (boolsection_2)
                {
                    strsection_2 = "/Sol_Empresa_Entidad_MateriaPrima/Create?SolicitudId=" + id;
                }
                if (boolsection_3)
                {
                    strsection_3 = "/Sol_Empresa_Entidad_MaquinariaUtilizada/Create?SolicitudId=" + id;
                }
                if (boolsection_4)
                {
                    strsection_4 = "/Sol_Empresa_Entidad_ViveroForestal/Index?SolicitudId=" + id;
                }
                if (boolsection_5)
                {
                    strsection_5 = "/Sol_Motosierra_MarcaModelo/Create?Solicitud_id=" + id;
                }

            }
        }

        [HttpPost]
        public JsonResult GrabarTipoIndustria(int SolicitudId,
                                                int TipoIndustriaId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Tipo Industria Registrado";

            //Tbl_Sol_Empresa_Entidad_Tipo_Industria tbl_Sol_Empresa_Entidad_Tipo_Industria;

            //int intTbl_Sol_Empresa_Entidad_Tipo_Industria = db.Tbl_Sol_Empresa_Entidad_Tipo_Industria.Where(Obj => Obj.Solicitud_Id == SolicitudId).Count();

            //if(intTbl_Sol_Empresa_Entidad_Tipo_Industria == 0)
            //{
            //    tbl_Sol_Empresa_Entidad_Tipo_Industria = new Tbl_Sol_Empresa_Entidad_Tipo_Industria();

            //    tbl_Sol_Empresa_Entidad_Tipo_Industria.Solicitud_Id = SolicitudId;
            //    tbl_Sol_Empresa_Entidad_Tipo_Industria.Tipo_Industria_Id = TipoIndustriaId;

            //    db.Tbl_Sol_Empresa_Entidad_Tipo_Industria.Add(tbl_Sol_Empresa_Entidad_Tipo_Industria);
            //    db.SaveChanges();

            //}
            //else
            //{
            //    tbl_Sol_Empresa_Entidad_Tipo_Industria = db.Tbl_Sol_Empresa_Entidad_Tipo_Industria.Where(Obj => Obj.Solicitud_Id == SolicitudId).First();

            //    tbl_Sol_Empresa_Entidad_Tipo_Industria.Tipo_Industria_Id = TipoIndustriaId;

            //    db.Entry(tbl_Sol_Empresa_Entidad_Tipo_Industria).State = EntityState.Modified;
            //    db.SaveChanges();

            //}

            string jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
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

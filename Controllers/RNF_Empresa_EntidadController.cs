using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Empresa_EntidadController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        string strNo_Registro;
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

        public ActionResult Create(string Guid_id)
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

            ViewBag.Guid_id = Guid_id;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            int intTbl_Sol_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == Guid_id).Count();

            Tbl_RNF_Empresa_Entidad tbl_RNF_empresa_entidad;

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                tbl_RNF_empresa_entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();


                tbl_RNF_empresa_entidad.swupdatedby = objUs.intUsuario_id;
                tbl_RNF_empresa_entidad.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_RNF_empresa_entidad.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_RNF_empresa_entidad.swupdatedbyinterno = true;
                }
            }
            else
            {

                tbl_RNF_empresa_entidad = new Tbl_RNF_Empresa_Entidad();

                tbl_RNF_empresa_entidad.DepartamentoEmpresaEntidad_id = 7;

                tbl_RNF_empresa_entidad.MunicipioEmpresaEntidad_id = 74;

                tbl_RNF_empresa_entidad.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

                tbl_RNF_empresa_entidad.swcreatedby = objUs.intUsuario_id;
                tbl_RNF_empresa_entidad.swdatecreated = DateTime.Now;

                tbl_RNF_empresa_entidad.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_empresa_entidad.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_empresa_entidad.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;


                if (objUs.EsInterno != 1)
                {
                    tbl_RNF_empresa_entidad.swcreatedbyinterno = false;
                }
                else
                {
                    tbl_RNF_empresa_entidad.swcreatedbyinterno = true;
                }

            }

            if (Session[Constants.session_Tbl_Sol_Empresa_Entidad] != null)
            {
                tbl_RNF_empresa_entidad = (Tbl_RNF_Empresa_Entidad)Session[Constants.session_Tbl_Sol_Empresa_Entidad];
            }


            ViewBag.Tipo_Industria_id = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes", tbl_RNF_empresa_entidad.Tipo_Industria_id);
            ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", (tbl_RNF_empresa_entidad.DepartamentoEmpresaEntidad_id ?? 7));
            ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == (tbl_RNF_empresa_entidad.DepartamentoEmpresaEntidad_id ?? 7)), "Municipio_id", "Municipio", tbl_RNF_empresa_entidad.MunicipioEmpresaEntidad_id);

            ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", (tbl_RNF_empresa_entidad.DepartamentoEmpresaMovilEntidad_id ?? 7));
            ViewBag.MunicipioEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == (tbl_RNF_empresa_entidad.DepartamentoEmpresaMovilEntidad_id ?? 7)), "Municipio_id", "Municipio", tbl_RNF_empresa_entidad.MunicipioEmpresaMovilEntidad_id);

            ViewBag.lstTipoIndustria = new SelectList(db.Tbl_Gral_Tipo_Industria, "Tipo_Industria_Id", "Nombres_Comunes");

            if (intSubCategoria == 1)
            {
                ViewBag.lstActividad = new SelectList(db.Tbl_Gral_Actividad.Where(Obj => Obj.Estado_id == true), "Actividad_Id", "Nombres_Comunes");
                ViewBag.lstMateriaPrima = new SelectList(db.Tbl_Gral_Materia_Prima.Where(Obj => Obj.Estado_id == true), "Materia_Prima_Id", "Nombres_Comunes");
                ViewBag.lstMaquinariaUtilizada = new SelectList(db.Tbl_Gral_Maquinaria_Utilizada.Where(Obj => Obj.Estado_id == true), "Maquinaria_Utilizada_Id", "Nombres_Comunes");
            }

            List<fc_RNF_Inscripcion_Vinculada_Result> fc_RNF_Inscripcion_Vinculada_Results = db.fc_RNF_Inscripcion_Vinculada().ToList() ?? new List<fc_RNF_Inscripcion_Vinculada_Result>();
            ViewBag.RNF_Inscripcion_Vinculada = new SelectList(fc_RNF_Inscripcion_Vinculada_Results, nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), tbl_RNF_empresa_entidad.RNF_Inscripcion_Vinculada);
            setViews(Guid_id);

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


            return View(tbl_RNF_empresa_entidad);
        }

        [HttpPost]
        public ActionResult Create(Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
            }
            else
            {
                Session[Constants.session_Tbl_Sol_Empresa_Entidad] = null;

                if (ModelState.IsValid)
                {

                    Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad.No_Registro).FirstOrDefault();

                    int intTbl_RNF_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad.No_Registro).Count();

                    if (intTbl_RNF_Empresa_Entidad == 0)
                    {
                        db.Tbl_RNF_Empresa_Entidad.Add(tbl_RNF_Empresa_Entidad);
                        db.SaveChanges();
                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad.No_Registro });
                    }
                    else
                    {

                        db.Entry(tbl_RNF_Empresa_Entidad).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.MensajeEmpresaEntidad = "Ultima actualizacion : " + DateTime.Now.ToString();
                        return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad.No_Registro });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";

                Session[Constants.session_Tbl_Sol_Empresa_Entidad] = (Tbl_RNF_Empresa_Entidad)tbl_RNF_Empresa_Entidad;
            }

            ViewBag.DepartamentoEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Empresa_Entidad.DepartamentoEmpresaEntidad_id);
            ViewBag.MunicipioEmpresaEntidad_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Empresa_Entidad.MunicipioEmpresaEntidad_id);

            ViewBag.DepartamentoEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Empresa_Entidad.DepartamentoEmpresaMovilEntidad_id);
            ViewBag.MunicipioEmpresaMovilEntidad_id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Empresa_Entidad.MunicipioEmpresaMovilEntidad_id);



            List<fc_RNF_Inscripcion_Vinculada_Result> fc_RNF_Inscripcion_Vinculada_Results = db.fc_RNF_Inscripcion_Vinculada().ToList() ?? new List<fc_RNF_Inscripcion_Vinculada_Result>();
            ViewBag.RNF_Inscripcion_Vinculada = new SelectList(fc_RNF_Inscripcion_Vinculada_Results, nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), nameof(fc_RNF_Inscripcion_Vinculada_Result.No_Registro), tbl_RNF_Empresa_Entidad.RNF_Inscripcion_Vinculada);

            return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_Empresa_Entidad.No_Registro });

        }

        class JsonRespuesta
        {
            public int CodRespuesta { get; set; }
            public string StrRespuesta { get; set; }
        }

        public JsonResult AgregarEditarEmpresaEntidad(Tbl_RNF_Empresa_Entidad tbl_RNF_Empresa_Entidad)
        {
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrRespuesta = "";
            if (!objRNFGrants.Editar)
            {
                jsonRespuesta.StrRespuesta = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar los datos de la entidad.";
            }
            else
            {
                Session[Constants.session_Tbl_Sol_Empresa_Entidad] = null;

                if (ModelState.IsValid)
                {

                    Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad.No_Registro).FirstOrDefault();

                    int intTbl_RNF_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == tbl_RNF_Empresa_Entidad.No_Registro).Count();

                    if (intTbl_RNF_Empresa_Entidad == 0)
                    {
                        db.Tbl_RNF_Empresa_Entidad.Add(tbl_RNF_Empresa_Entidad);
                        db.SaveChanges();
                        jsonRespuesta.CodRespuesta = 1;
                        jsonRespuesta.StrRespuesta = "Registro agregado con éxito";
                    }
                    else
                    {

                        db.Entry(tbl_RNF_Empresa_Entidad).State = EntityState.Modified;
                        db.SaveChanges();

                        jsonRespuesta.CodRespuesta = 1;
                        jsonRespuesta.StrRespuesta = "Registro actualizado con éxito";
                    }
                }


                Session[Constants.session_Tbl_Sol_Empresa_Entidad] = (Tbl_RNF_Empresa_Entidad)tbl_RNF_Empresa_Entidad;
            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        void setViews(string No_Registro)
        {
            strNo_Registro = No_Registro;

            ViewBag.strNo_Registro = No_Registro;
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


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).First();

            string viewTipoRegistro, viewEmpresaEntidadActividad, viewEmpresaEntidadMateriaPrima, viewEmpresaEntidadMaquinariaUtilizada, viewEmpresaEntidadViveroForestal, viewMotosierraMarcaModelo, viewComplementoView;
            viewComplementoView = "?No_Registro=" + No_Registro;
            viewTipoRegistro = "/RNF_TipoRegistro/Create" + viewComplementoView;
            viewEmpresaEntidadActividad = "/RNF_Empresa_Entidad_Actividad/Create" + viewComplementoView;
            viewEmpresaEntidadMateriaPrima = "/RNF_Empresa_Entidad_MateriaPrima/Create" + viewComplementoView;
            viewEmpresaEntidadMaquinariaUtilizada = "/RNF_Empresa_Entidad_MaquinariaUtilizada/Create" + viewComplementoView;
            viewEmpresaEntidadViveroForestal = "/RNF_Empresa_Entidad_ViveroForestal/Index" + viewComplementoView;
            viewMotosierraMarcaModelo = "/RNF_Motosierra_MarcaModelo/Create" + viewComplementoView;


            if ((No_Registro == null) || (No_Registro == ""))
            {

                strsection_0 = viewTipoRegistro;
                boolsection_0 = true;

            }
            else
            {
                strsection_0 = viewTipoRegistro;
                boolsection_0 = true;
            }


            if (tbl_RNF_Registro.Categoria_id == 5)
            {
                if (tbl_RNF_Registro.Sub_Categoria_id == 1) //Industria Forestal
                {
                    strsection_1 = viewEmpresaEntidadActividad;
                    boolsection_1 = true;

                    strsection_2 = viewEmpresaEntidadMateriaPrima;
                    boolsection_2 = true;

                    strsection_3 = viewEmpresaEntidadMaquinariaUtilizada;
                    boolsection_3 = true;
                }
                else if (tbl_RNF_Registro.Sub_Categoria_id == 2) //Deposito Forestal
                {
                    strsection_1 = viewEmpresaEntidadActividad;
                    boolsection_1 = true;

                    strsection_2 = viewEmpresaEntidadMateriaPrima;
                    boolsection_2 = true;

                    strsection_3 = "";
                    boolsection_3 = false;
                }
                else if (tbl_RNF_Registro.Sub_Categoria_id == 3) //Centro de Acopio
                {
                    strsection_1 = "";
                    boolsection_1 = false;

                    strsection_2 = "";
                    boolsection_2 = false;

                    strsection_3 = "";
                    boolsection_3 = false;
                }
                else if (tbl_RNF_Registro.Sub_Categoria_id == 4) //Viveros Forestales
                {
                    strsection_1 = "";
                    boolsection_1 = false;

                    strsection_2 = "";
                    boolsection_2 = false;

                    strsection_3 = "";
                    boolsection_3 = false;

                    strsection_4 = viewEmpresaEntidadViveroForestal;
                    boolsection_4 = true;
                }
                else if (tbl_RNF_Registro.Sub_Categoria_id == 5) //Exportadoras e Importadoras de Productos Forestales
                {
                    strsection_1 = viewEmpresaEntidadActividad;
                    boolsection_1 = true;

                    strsection_2 = "";
                    boolsection_2 = false;

                    strsection_3 = "";
                    boolsection_3 = false;
                }
                else if (tbl_RNF_Registro.Sub_Categoria_id == 6 || tbl_RNF_Registro.Sub_Categoria_id == 7 || tbl_RNF_Registro.Sub_Categoria_id == 8) //Productos Forestales no Maderables. Repobladoras Forestales. Consultoras Forestales.
                {
                    strsection_1 = "";
                    boolsection_1 = false;

                    strsection_2 = "";
                    boolsection_2 = false;

                    strsection_3 = "";
                    boolsection_3 = false;
                }
            }

            if (tbl_RNF_Registro.Categoria_id == 8)
            {
                if (tbl_RNF_Registro.Sub_Categoria_id == 2)
                {
                    strsection_1 = viewEmpresaEntidadActividad;
                    boolsection_1 = true;

                    strsection_2 = viewEmpresaEntidadMateriaPrima;
                    boolsection_2 = true;

                    strsection_3 = viewEmpresaEntidadMaquinariaUtilizada;
                    boolsection_3 = true;

                    strsection_4 = viewEmpresaEntidadViveroForestal;
                    boolsection_4 = true;

                    strsection_5 = viewMotosierraMarcaModelo;
                    boolsection_5 = true;
                }
            }
        }
    }
}
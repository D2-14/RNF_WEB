using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using RNF_Web.Models;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Data.Entity;
using iTextSharp.text.html;
using Newtonsoft.Json;
using System.Data;

namespace RNF_Web.Controllers
{

    public class RNF_RegistroController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        public object ExcelPackage { get; private set; }

        private string strCurrentStep = "";

        private string strsection_0 = "";
        private bool boolsection_0 = true;

        private string strsection_I = "";
        private bool boolsection_I = false;

        private string strsection_II = "";
        private bool boolsection_II = false;

        private string strsection_III = "";
        private bool boolsection_III = false;

        private string strsection_IV = "";
        private bool boolsection_IV = false;

        private string strsection_V = "";
        private bool boolsection_V = false;

        private string strsection_VI = "";
        private bool boolsection_VI = false;

        private string strsection_VII = "";
        private bool boolsection_VII = false;

        private string strsection_VIII = "";
        private bool boolsection_VIII = false;

        private string strsection_IX = "";
        private bool boolsection_IX = false;

        int intTbl_PersoneriaIndividual;
        int intTbl_PersoneriaJuridica;
        int intTbl_Sol_RepresentanteLegal;
        int intTbl_Sol_Motosierra;
        int intTbl_Sol_Finca;
        int intTbl_Sol_Rodal;
        int intTbl_Sol_Rodal_Dasometrico;

        int intTbl_Sol_Empresa_Entidad;
        int intRegion_id;
        long lngSolicitud_id;

        int intCategoria_id;

        int intProcedenciaPinfor;
        int intProcedenciaPinpep;
        int intProcedenciaProbosque;
        int intProcedenciaExterna;


        bool boolProcedencia_Probosque = false;
        bool boolProcedencia_PinpepOld = false;
        bool boolProcedencia_PinpepNew = false;
        bool boolProcedencia_Secorf = false;
        bool boolProcedencia_Externa = false;

        private PdfPTable tableBanner = new PdfPTable(1);
        private PdfPTable tableTitulo = new PdfPTable(3);

        // GET: RNF_Registro
        public ActionResult Index()
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

        public ActionResult ListaRegistros(string No_Registro, string Expediente)
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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

            string sqlQuery = "";
            List<Tbl_RNF_Registro> tbl_RNF_Registros = new List<Tbl_RNF_Registro>();


            try
            {
                sqlQuery = "SELECT *\n";
                sqlQuery += "FROM Tbl_RNF_Registro\n";
                sqlQuery += "WHERE (UsuarioExterno_id = '" + tbl_Seg_UsuarioExterno.Usuario_id + "'\n";
                sqlQuery += "OR DPI_Titular LIKE '%" + tbl_Seg_UsuarioExterno.No_Documento + "%')\n";
                if((No_Registro??"").Trim() != "")
                {
                    sqlQuery += "AND No_Registro LIKE '%"+No_Registro+"%'\n";
                }
                if ((Expediente ?? "").Trim() != "")
                {
                    sqlQuery += "AND Expediente LIKE '%"+Expediente+"%'\n";
                }
                sqlQuery += "ORDER BY swdatecreated DESC\n";
                sqlQuery += "\n";
                //tbl_RNF_Registros = (from Obj in db.Tbl_RNF_Registro
                //                     where Obj.UsuarioExterno_id == tbl_Seg_UsuarioExterno.Usuario_id || Obj.DPI_Titular.Contains(tbl_Seg_UsuarioExterno.No_Documento)
                //                     orderby Obj.swdatecreated descending
                //                     select Obj).ToList() ?? new List<Tbl_RNF_Registro>();
                tbl_RNF_Registros = db.Tbl_RNF_Registro.SqlQuery(sqlQuery).ToList() ?? new List<Tbl_RNF_Registro>();

                return View(tbl_RNF_Registros);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("AccesoDenegado", "Home");
            }
        }

        void Uno_SetUpView(string Guid_id)
        {
            strsection_I = "/RNF_Registro/Create/?Guid_id=" + Guid_id;

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal)) || boolProcedencia_Externa)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";

            }

        }

        void Dos_SetUpView(string Guid_id)
        {

            //strsection_I = "/Sol_Solicitud/CreateConRegion/" + lngSolicitud_id;
            //if (intRegion_id == 0)
            //{
            //    return;
            //}
            strsection_I = "/RNF_Registro/Create/?Guid_id=" + Guid_id;



            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";

            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }


            if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";
            }



        }

        void Tres_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }



            if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";

            }

        }

        void Cuatro_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }


            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";




            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }


            if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Cinco_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Empresa_Entidad/Create?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Empresa_Entidad/Create?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Seis_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0) || boolProcedencia_Externa)
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";

                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Finca/IndexFincaRodalDasometricos?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }


            if ((intTbl_Sol_Rodal > 0) && (intTbl_Sol_Rodal_Dasometrico >= intTbl_Sol_Rodal) || boolProcedencia_Externa)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";

            }


        }

        void Siete_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_TecnicoProfesional/EditProfesional/?No_Registro=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";

            boolsection_III = true;
            strsection_III = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";

        }

        void Ocho_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";



            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {

                    boolsection_III = true;
                    strsection_III = "/RNF_Motosierra/Create?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Motosierra/Create?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Motosierra > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        void Nueve_SetUpView(string Guid_id)
        {

            strsection_I = "/RNF_Registro/CreateConRegion/?Guid_id=" + Guid_id;
            if (intRegion_id == 0)
            {
                return;
            }

            boolsection_II = true;
            strsection_II = "/RNF_Registro/IndexPropietarioRepresentante?Guid_id=" + Guid_id;
            strCurrentStep = "form-total-t-" + "1";


            if ((intTbl_PersoneriaIndividual > 0) || (intTbl_PersoneriaJuridica > 0))
            {
                if (intTbl_PersoneriaJuridica == 0)
                {
                    boolsection_III = true;
                    strsection_III = "/RNF_Empresa_Entidad/Create?Guid_id=" + Guid_id;
                    strCurrentStep = "form-total-t-" + "2";
                }
                else
                {
                    if (intTbl_Sol_RepresentanteLegal > 0)
                    {
                        boolsection_III = true;
                        strsection_III = "/RNF_Empresa_Entidad/Create?Guid_id=" + Guid_id;
                        strCurrentStep = "form-total-t-" + "2";
                    }
                }
            }

            if (intTbl_Sol_Empresa_Entidad > 0)
            {
                boolsection_IV = true;
                strsection_IV = "/RNF_UploadFiles/UploadedFiles?Guid_id=" + Guid_id;
                strCurrentStep = "form-total-t-" + "3";
            }

        }

        public ActionResult RegistroView(string No_Registro)//(string No_RegistroLiteral, int No_RegistroCorrelativo)
        {
            Tbl_RNF_Registro registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == registro.UsuarioExterno_id
                                                             select d).FirstOrDefault();
            ViewBag.No_Registro = No_Registro;
            int intContador = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();

            if (intContador == 0)
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

            EdicionRNFGrants objRNFGrants = new EdicionRNFGrants();
            objRNFGrants = (from d in db.fc_Gral_RNF_Configuracion(No_Registro, objUs.intUsuario_id, boolEsInterno)
                            select new EdicionRNFGrants
                            {
                                Agregar = (bool)d.Agregar,
                                Editar = (bool)d.Editar,
                                Borrar = (bool)d.Borrar,
                                Estado_id = (int)d.Estado_id,
                                Estado_Registro = d.Estado_Registro,
                                Usuario = d.Usuario,
                                GenerarConstancia = (bool)d.GenerarConstancia
                            }).FirstOrDefault();

            Session[Constants.session_EdicionRNFGrants] = objRNFGrants;
            //var permisosRNF = db.fc_Gral_RNF_Configuracion(No_Registro, objUs.intUsuario_id, boolEsInterno).FirstOrDefault();
            //	0	-- Seleccione una categoría ---
            //	1	Bosques naturales
            //	2	Plantaciones forestales
            //	3	Plantaciones de arboles frutales
            //	4	Sistemas agroforestales
            //	5	Empresas forestales
            //	6	Fuentes semilleras y material vegetativo
            //	7	Profesionales del área forestal
            //	8	Motosierras
            //	9	Entidades relacionadas con investigación, extensión y capacitación

            ViewBag.section_0 = true;
            ViewBag.section_I = false;
            ViewBag.section_II = false;
            ViewBag.section_III = false;
            ViewBag.section_IV = false;
            ViewBag.section_V = false;
            ViewBag.section_VI = false;

            boolsection_0 = true;
            boolsection_I = false;
            boolsection_II = false;
            boolsection_III = false;
            boolsection_IV = false;
            boolsection_V = false;
            boolsection_VI = false;

            strsection_I = "";
            strsection_II = "";
            strsection_III = "";
            strsection_IV = "";
            strsection_V = "";
            strsection_VI = "";

            EdicionSolicitudGrants EdicionSolicitudGrant = new EdicionSolicitudGrants();

            EdicionSolicitudGrant.boolEditarSolicitud = false;
            EdicionSolicitudGrant.boolEditarDasometrico = false;
            EdicionSolicitudGrant.boolEditarFinca = false;
            EdicionSolicitudGrant.boolEditarMotosierra = false;
            EdicionSolicitudGrant.boolEditarEntidad = false;
            EdicionSolicitudGrant.boolEditarRodal = false;
            EdicionSolicitudGrant.boolEditarDasometrico = false;
            EdicionSolicitudGrant.boolEditarPropietario = false;
            EdicionSolicitudGrant.boolEditarRepresentante = false;
            EdicionSolicitudGrant.boolSubirDocumentos = false;
            EdicionSolicitudGrant.boolEtapaView = true;


            Session[Constants.session_EdicionSolicitudGrants] = EdicionSolicitudGrant;


            ///*************************************************************************************///
            // Si se detecta que ya han subido documentos se puede enviar a analisis la solicitud   ///
            int intDocumentosSubidos = db.Tbl_RNF_DocumentoSubido.Where(Obj => Obj.No_Registro == No_Registro).Count();

            intTbl_PersoneriaIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_PersoneriaJuridica = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();

            intTbl_Sol_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Rodal_Dasometrico = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();


            //Acá se define si es procedencia externa para que permita visualizar todos los campos registrados de parte de la migración
            boolProcedencia_Probosque = registro.Procedencia_Probosque;
            boolProcedencia_PinpepOld = registro.Procedencia_PinpepOld;
            boolProcedencia_PinpepNew = registro.Procedencia_PinpepNew;
            boolProcedencia_Secorf = registro.Procedencia_secorf;

            if (boolProcedencia_Probosque || boolProcedencia_PinpepOld || boolProcedencia_PinpepNew || boolProcedencia_Secorf)
            {
                boolProcedencia_Externa = true;
            }


            intTbl_Sol_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intRegion_id = registro.Region_id ?? 0;

            if ((intTbl_Sol_Finca > 0) || (intTbl_Sol_Empresa_Entidad > 0) || (intTbl_Sol_Motosierra > 0))
            {
                Session[Constants.session_SolicitudLista] = 1;
                ViewBag.EnviaraEvaluacion = 1;
            }
            else
            {
                Session[Constants.session_SolicitudLista] = 0;
                ViewBag.EnviaraEvaluacion = 0;
            }

            ///***********************************************************************************


            strsection_0 = "/UsuarioExterno/UsuarioExView?id=" + tbl_Seg_UsuarioExterno.Usuario_id + "&email=" + tbl_Seg_UsuarioExterno.Correo;

            strsection_I = "/RNF_Registro/Create/?Guid_id=" + registro.No_Registro;


            strCurrentStep = "form-total-t-" + "0";
            boolsection_I = true;

            if (registro.Categoria_id == 1) { Uno_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 2) { Dos_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 3) { Tres_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 4) { Cuatro_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 5) { Cinco_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 6) { Seis_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 7) { Siete_SetUpView(registro.No_Registro); }

            if ((registro.Categoria_id == 8) && (registro.Sub_Categoria_id == 1)) { Ocho_SetUpView(registro.No_Registro); }

            if ((registro.Categoria_id == 8) && (registro.Sub_Categoria_id == 2)) { Cinco_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 9) { Nueve_SetUpView(registro.No_Registro); }


            ViewBag.CurrentStep = strCurrentStep;

            ViewBag.section_I = boolsection_I;
            ViewBag.section_II = boolsection_II;
            ViewBag.section_III = boolsection_III;
            ViewBag.section_IV = boolsection_IV;
            ViewBag.section_V = boolsection_V;
            ViewBag.section_VI = boolsection_VI;

            ViewBag.section_Link_0 = strsection_0;
            ViewBag.section_Link_I = strsection_I;
            ViewBag.section_Link_II = strsection_II;
            ViewBag.section_Link_III = strsection_III;
            ViewBag.section_Link_IV = strsection_IV;
            ViewBag.section_Link_V = strsection_V;
            ViewBag.section_Link_VI = strsection_VI;

            return View();
        }
        public ActionResult RegistroEdit(string No_Registro)//(string No_RegistroLiteral, int No_RegistroCorrelativo)
        {
            Tbl_RNF_Registro registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            ViewBag.registro = registro;
            int intContador = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();

            if (intContador == 0)
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

            EdicionRNFGrants objRNFGrants = new EdicionRNFGrants();
            objRNFGrants = (from d in db.fc_Gral_RNF_Configuracion(No_Registro, objUs.intUsuario_id, boolEsInterno)
                            select new EdicionRNFGrants
                            {
                                Agregar = (bool)d.Agregar,
                                Editar = (bool)d.Editar,
                                Borrar = (bool)d.Borrar,
                                Estado_id = (int)d.Estado_id,
                                Estado_Registro = d.Estado_Registro,
                                Usuario = d.Usuario
                            }).FirstOrDefault();

            Session[Constants.session_EdicionRNFGrants] = objRNFGrants;
            //	0	-- Seleccione una categoría ---
            //	1	Bosques naturales
            //	2	Plantaciones forestales
            //	3	Plantaciones de arboles frutales
            //	4	Sistemas agroforestales
            //	5	Empresas forestales
            //	6	Fuentes semilleras y material vegetativo
            //	7	Profesionales del área forestal
            //	8	Motosierras
            //	9	Entidades relacionadas con investigación, extensión y capacitación

            ViewBag.section_0 = true;
            ViewBag.section_I = false;
            ViewBag.section_II = false;
            ViewBag.section_III = false;
            ViewBag.section_IV = false;
            ViewBag.section_V = false;
            ViewBag.section_VI = false;

            boolsection_0 = true;
            boolsection_I = false;
            boolsection_II = false;
            boolsection_III = false;
            boolsection_IV = false;
            boolsection_V = false;
            boolsection_VI = false;

            strsection_I = "";
            strsection_II = "";
            strsection_III = "";
            strsection_IV = "";
            strsection_V = "";
            strsection_VI = "";

            EdicionSolicitudGrants EdicionSolicitudGrant = new EdicionSolicitudGrants();

            EdicionSolicitudGrant.boolEditarSolicitud = true;
            EdicionSolicitudGrant.boolEditarDasometrico = true;
            EdicionSolicitudGrant.boolEditarFinca = true;
            EdicionSolicitudGrant.boolEditarMotosierra = true;
            EdicionSolicitudGrant.boolEditarEntidad = true;
            EdicionSolicitudGrant.boolEditarRodal = true;
            EdicionSolicitudGrant.boolEditarDasometrico = true;
            EdicionSolicitudGrant.boolEditarPropietario = true;
            EdicionSolicitudGrant.boolEditarRepresentante = true;
            EdicionSolicitudGrant.boolSubirDocumentos = true;

            EdicionSolicitudGrant.boolEtapaView = true;


            Session[Constants.session_EdicionSolicitudGrants] = EdicionSolicitudGrant;


            ///*************************************************************************************///
            // Si se detecta que ya han subido documentos se puede enviar a analisis la solicitud   ///
            int intDocumentosSubidos = db.Tbl_RNF_DocumentoSubido.Where(Obj => Obj.No_Registro == No_Registro).Count();

            intTbl_PersoneriaIndividual = db.Tbl_RNF_PropietarioPersonaIndividual.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_PersoneriaJuridica = db.Tbl_RNF_PropietarioPersonaJuridica.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_RepresentanteLegal = db.Tbl_RNF_RepresentanteLegal.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();

            intTbl_Sol_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intTbl_Sol_Rodal_Dasometrico = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();




            intTbl_Sol_Empresa_Entidad = db.Tbl_RNF_Empresa_Entidad.Where(Obj => Obj.No_Registro == registro.No_Registro).Count();
            intRegion_id = registro.Region_id ?? 0;

            if ((intTbl_Sol_Finca > 0) || (intTbl_Sol_Empresa_Entidad > 0) || (intTbl_Sol_Motosierra > 0))
            {
                Session[Constants.session_SolicitudLista] = 1;
                ViewBag.EnviaraEvaluacion = 1;
            }
            else
            {
                Session[Constants.session_SolicitudLista] = 0;
                ViewBag.EnviaraEvaluacion = 0;
            }

            ///***********************************************************************************


            strsection_0 = "/UsuarioExterno/UsuarioExView/" + registro.swcreatedby;

            strsection_I = "/RNF_Registro/Create/?Guid_id=" + registro.No_Registro;


            strCurrentStep = "form-total-t-" + "0";
            boolsection_I = true;

            if (registro.Categoria_id == 1) { Uno_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 2) { Dos_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 3) { Tres_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 4) { Cuatro_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 5) { Cinco_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 6) { Seis_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 7) { Siete_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 8) { Ocho_SetUpView(registro.No_Registro); }

            if (registro.Categoria_id == 9) { Nueve_SetUpView(registro.No_Registro); }


            ViewBag.CurrentStep = strCurrentStep;

            ViewBag.section_I = boolsection_I;
            ViewBag.section_II = boolsection_II;
            ViewBag.section_III = boolsection_III;
            ViewBag.section_IV = boolsection_IV;
            ViewBag.section_V = boolsection_V;
            ViewBag.section_VI = boolsection_VI;

            ViewBag.section_Link_0 = strsection_0;
            ViewBag.section_Link_I = strsection_I;
            ViewBag.section_Link_II = strsection_II;
            ViewBag.section_Link_III = strsection_III;
            ViewBag.section_Link_IV = strsection_IV;
            ViewBag.section_Link_V = strsection_V;
            ViewBag.section_Link_VI = strsection_VI;

            return View();
        }

        public ActionResult DatosRegistro(string No_Registro)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("AccesoColaborador", "Login");
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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(x => x.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(tbl_RNF_Registro);
        }


        public JsonResult ObtieneInfoRNF(Tbl_RNF_Registro model)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.Guid_id == model.Guid_id
                                                 select d).FirstOrDefault();
            if (tbl_RNF_Registro == null)
            {
                return Json("");
            }

            string enc = SecurEncryptDecrypt.EncryptString(tbl_RNF_Registro.No_Registro);


            return Json(enc);
        }

        public ActionResult VisualizarInscripcion(string encRegistro)
        {
            string No_Registro = SecurEncryptDecrypt.DecryptString(encRegistro);
           

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                return RedirectToAction("../Home/RegistroInexistente");
            }


            if ((tbl_RNF_Registro.Estado_id != 2) && (tbl_RNF_Registro.Estado_id != 3) && (tbl_RNF_Registro.Fecha_De_Vencimiento > DateTime.Now))
            {
                if (tbl_RNF_Registro.ConstanciaFirmada == null)
                {
                    return RedirectToAction("../Home/RegistroActivoNoGenerado");
                }
                else
                {
                    return RedirectToAction("DocumentoOficial", "VisorDeDocumentos", new { Documento = tbl_RNF_Registro.ConstanciaFirmada });
                }
            }

            if ((tbl_RNF_Registro.Estado_id != 2) && (tbl_RNF_Registro.Estado_id != 3) && (tbl_RNF_Registro.Fecha_De_Vencimiento < DateTime.Now))
            {
                return RedirectToAction("../Content/Machotes/RegistroVencido.pdf");
            }

            if ((tbl_RNF_Registro.Estado_id == 2) || (tbl_RNF_Registro.Estado_id == 3))
            {
                if (tbl_RNF_Registro.ConstanciaFirmada == null)
                {
                    return RedirectToAction("../Home/RegistroInactivoNoGenerado");
                }
                else
                {
                    return RedirectToAction("DocumentoOficial", "VisorDeDocumentos", new { Documento = tbl_RNF_Registro.ConstanciaFirmada });
                }
            }


            return View();
        }

        public ActionResult Buscar()
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            return View();
        }

        public ActionResult BuscarParams(string No_Registro, string Expediente, string DPI_Titular)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            if (No_Registro == null)
            {
                No_Registro = "";
            }
            if (Expediente == null)
            {
                Expediente = "";
            }
            if (DPI_Titular == "")
            {
                DPI_Titular = "";
            }

            No_Registro = No_Registro.Replace(" ", "%");
            No_Registro = No_Registro.Replace("-", "%");
            No_Registro = No_Registro.Replace("_", "%");
            No_Registro = No_Registro.Replace(".", "%");
            No_Registro = No_Registro.Replace(",", "%");
            No_Registro = No_Registro.Replace("/", "%");

            Expediente = Expediente.Replace(" ", "%");
            Expediente = Expediente.Replace("-", "%");
            Expediente = Expediente.Replace("_", "%");
            Expediente = Expediente.Replace(".", "%");
            Expediente = Expediente.Replace(",", "%");
            Expediente = Expediente.Replace("/", "%");

            DPI_Titular = DPI_Titular.Replace(" ", "%");
            DPI_Titular = DPI_Titular.Replace("-", "%");
            DPI_Titular = DPI_Titular.Replace("_", "%");
            DPI_Titular = DPI_Titular.Replace(".", "%");
            DPI_Titular = DPI_Titular.Replace(",", "%");
            DPI_Titular = DPI_Titular.Replace("/", "%");


            List<RNF_Registro_Informacion> rNF_Registro_Informacions = new List<RNF_Registro_Informacion>();
            string sqlQuery, sqlQueryComplementoBusqueda;
            sqlQueryComplementoBusqueda = "";
            sqlQuery = " Select top 100 \n";
            sqlQuery += " RNF_Reg.[No_Registro] \n";
            sqlQuery += " ,RNF_Reg.[No_RegistroLiteral] \n";
            sqlQuery += " ,RNF_Reg.[No_RegistroCorrelativo] \n";
            sqlQuery += " ,isnull(RNF_Reg.[Expediente] ,'')[Expediente] \n";
            sqlQuery += " ,(select GralReg.Nombre_Region from Tbl_Gral_Region GralReg where GralReg.Id_Region = RNF_Reg.Region_id) Region \n";
            sqlQuery += " ,(select Nombre_SubRegion from Tbl_Gral_SubRegion GralSubReg where GralSubReg.Region_id = RNF_Reg.Region_id and GralSubReg.SubRegion_id = RNF_Reg.SubRegion_id) SubRegion \n";
            sqlQuery += " ,(Select Cat.Descripcion from Tbl_Sol_Solicitud_Categoria Cat where Cat.Categoria_id = RNF_Reg.Categoria_id) Categoria \n";
            sqlQuery += " ,(Select SubCat.Descripcion from Tbl_Sol_Solicitud_Sub_Categoria SubCat where SubCat.Categoria_id = RNF_Reg.Sub_Categoria_id and SubCat.Sub_Categoria_id = RNF_Reg.Sub_Categoria_id) SubCategoria \n";
            sqlQuery += " ,RNF_Reg.[Fecha_De_Vencimiento] \n";
            sqlQuery += " ,RNF_Reg.[UsuarioExterno_id] \n";
            sqlQuery += " ,RNF_Reg.[Solicitud_id] \n";
            sqlQuery += " ,RNF_Reg.[GuidSolicitud_id] \n";
            sqlQuery += " ,isnull(convert(varchar(50),RNF_Reg.[DPI_Titular]),'') [DPI_Titular] \n";
            sqlQuery += " ,isnull((select Nombres + ' ' + Apellidos from Tbl_Seg_UsuarioExterno SegUs where No_Documento = isnull([DPI_Titular],'') ),'') Titular, Categoria_id \n";
            sqlQuery += " From \n";
            sqlQuery += " [dbo].[Tbl_RNF_Registro] RNF_Reg \n";
            sqlQueryComplementoBusqueda += " where \n";
            sqlQueryComplementoBusqueda += " (isnull('" + No_Registro + "','')like'' or (RNF_Reg.No_Registro Like '%" + No_Registro + "%')) \n";
            sqlQueryComplementoBusqueda += " and (isnull('" + Expediente + "','')like'' or (RNF_Reg.Expediente Like '%" + Expediente + "%')) \n";
            sqlQueryComplementoBusqueda += " and (isnull('" + DPI_Titular + "','') like '' or (RNF_Reg.DPI_Titular Like '%" + DPI_Titular + "%'))  \n";
            sqlQuery += sqlQueryComplementoBusqueda;

            rNF_Registro_Informacions = db.Database.SqlQuery<RNF_Registro_Informacion>(sqlQuery).ToList();


            return View(rNF_Registro_Informacions);
        }




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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_RNF_Registro.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_RNF_Registro.Categoria_id && obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_RNF_Registro.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Sub_Categoria_id);


            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_Region", tbl_RNF_Registro.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_RNF_Registro.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegion", tbl_RNF_Registro.SubRegion_id);

            return View(tbl_RNF_Registro);
        }
        public ActionResult CreateConRegion(string Guid_id)
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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_RNF_Registro.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_RNF_Registro.Categoria_id && obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_RNF_Registro.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_Region", tbl_RNF_Registro.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_RNF_Registro.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegion", tbl_RNF_Registro.SubRegion_id);


            if ((tbl_RNF_Registro.Region_id == 0) && (tbl_RNF_Registro.Categoria_id == 8))
            { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la región y dirección en la que realizará la gestión, para continuar. "; }


            return View(tbl_RNF_Registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConRegion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
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

            /* Si es una nueva solicitud el estado es Cero */
            if (tbl_Sol_Solicitud.Solicitud_id == 0)
            {
                tbl_Sol_Solicitud.Estado_id = 0;

                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Sol_Solicitud.Max(u => u.Solicitud_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Sol_Solicitud.Solicitud_id = lngIdt;
            }

            if (ModelState.IsValid)
            {
                string sqlQuery;
                SqlParameter[] sqlParams;


                sqlQuery = "Exec SP_Sol_InsUpd_Solicitud @Solicitud_id, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id ,@SubRegion_id, @Estado_id, @AreaTotalFincas, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                        {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_Sol_Solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Categoria_id",  Value = tbl_Sol_Solicitud.Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Region_id",  Value = tbl_Sol_Solicitud.Region_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@SubRegion_id",  Value = tbl_Sol_Solicitud.SubRegion_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Estado_id",  Value = tbl_Sol_Solicitud.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@AreaTotalFincas",  Value = tbl_Sol_Solicitud.AreaTotalFincas, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swcreatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swcreatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                        };


                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                if (resultado[0].respuesta == 1)
                {

                    ViewBag.Mensaje_II = "Paso completado.";
                    ViewBag.RegistroGrabado = 1;
                    Session[Constants.session_Solicitud] = resultado[0].id;

                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = Session[Constants.session_Solicitud] });

                }
                else
                {
                    TempData["Mensaje"] = resultado[0].mensaje;
                    ViewBag.Mensaje = resultado[0].mensaje;
                    ViewBag.RegistroGrabado = 0;
                }
            }
            else
            {
                if (tbl_Sol_Solicitud.Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = " Seleccione la categoría de la solicitud "; }
                if (tbl_Sol_Solicitud.Sub_Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la sub categoría de la solicitud "; }

                TempData["MensajeSolicitudFail"] = @TempData["MensajeSolicitudFail"] + "-- Favor corregir --";
                ViewBag.RegistroGrabado = 0;
            }




            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);


            Session[Constants.session_Tbl_Sol_Solicitud] = (Tbl_Sol_Solicitud)tbl_Sol_Solicitud;


            if ((tbl_Sol_Solicitud.Region_id == 0) && (tbl_Sol_Solicitud.Categoria_id == 8))
            { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la región en la que realizará la gestión, para continuar. "; }



            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
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

            /* Si es una nueva solicitud el estado es Cero */
            if (tbl_Sol_Solicitud.Solicitud_id == 0)
            {
                tbl_Sol_Solicitud.Estado_id = 0;
            }



            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Solicitud.Max(u => u.Solicitud_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_Solicitud.Solicitud_id = lngIdt;


            if (ModelState.IsValid)
            {
                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Sol_InsUpd_Solicitud @Solicitud_id, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id ,@SubRegion_id, @Estado_id, @AreaTotalFincas, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                        {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_Sol_Solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Categoria_id",  Value = tbl_Sol_Solicitud.Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Sub_Categoria_id??0, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Region_id",  Value = tbl_Sol_Solicitud.Region_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@SubRegion_id",  Value = tbl_Sol_Solicitud.SubRegion_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Estado_id",  Value = tbl_Sol_Solicitud.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@AreaTotalFincas",  Value = tbl_Sol_Solicitud.AreaTotalFincas, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@swcreatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swcreatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                        };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                if (resultado[0].respuesta == 1)
                {

                    ViewBag.Mensaje_II = "Paso completado.";
                    ViewBag.RegistroGrabado = 1;
                    Session[Constants.session_Solicitud] = resultado[0].id;

                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = Session[Constants.session_Solicitud] });

                }
                else
                {
                    TempData["Mensaje"] = resultado[0].mensaje;
                    ViewBag.Mensaje = resultado[0].mensaje;
                    ViewBag.RegistroGrabado = 0;
                }
            }
            else
            {
                if (tbl_Sol_Solicitud.Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = " Seleccione la categoría de la solicitud "; }
                if (tbl_Sol_Solicitud.Sub_Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la sub categoría de la solicitud "; }

                TempData["MensajeSolicitudFail"] = @TempData["MensajeSolicitudFail"] + "-- Favor corregir --";
                ViewBag.RegistroGrabado = 0;
            }



            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);


            Session[Constants.session_Tbl_Sol_Solicitud] = (Tbl_Sol_Solicitud)tbl_Sol_Solicitud;

            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id });


        }


        public ActionResult IndexPropietarioRepresentante(string Guid_id)
        {
            ViewBag.Guid_id = Guid_id;
            return View();
        }
        public ActionResult IndexPropietario(string Guid_id)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            int PropietariosIndividuales = db.Tbl_RNF_PropietarioPersonaIndividual.Where(obj => obj.No_Registro == tbl_RNF_Registro.No_Registro && obj.Estado_id == true).Count();

            if (PropietariosIndividuales > 0)
            {
                ViewBag.Personeria = new SelectList(db.Tbl_Sol_Solicitud_Personeria.Where(Obj => Obj.PersoneriaTipo_id == 1), "PersoneriaTipo_id", "Descripcion", 0);
            }
            else
            {
                ViewBag.Personeria = new SelectList(db.Tbl_Sol_Solicitud_Personeria, "PersoneriaTipo_id", "Descripcion", 0);
            }

            var ListodoDePropietariosRegistrados = db.fc_RNF_Sel_ListadoDePropietario(tbl_RNF_Registro.No_Registro);
            ViewBag.Guid_id = Guid_id;
            return View(ListodoDePropietariosRegistrados);

        }
        public ActionResult IndexArrendatario(string No_Registro)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            int PropietariosIndividuales = db.Tbl_RNF_ArrendatarioPersonaIndividual.Where(obj => obj.No_Registro == tbl_RNF_Registro.No_Registro && obj.Estado_id == true).Count();

            if (PropietariosIndividuales > 0)
            {
                ViewBag.PersoneriaArrendatario = new SelectList(db.Tbl_Sol_Solicitud_Personeria.Where(Obj => Obj.PersoneriaTipo_id == 1), "PersoneriaTipo_id", "Descripcion", 0);
            }
            else
            {
                ViewBag.PersoneriaArrendatario = new SelectList(db.Tbl_Sol_Solicitud_Personeria, "PersoneriaTipo_id", "Descripcion", 0);
            }

            var ListodoDePropietariosRegistrados = db.fc_RNF_Sel_ListadoDeArrendatario(No_Registro);
            ViewBag.No_Registro = No_Registro;
            return View(ListodoDePropietariosRegistrados);
        }
        long sp_actualizacion_rnfsolicitud(string No_RegistroLiteral, int No_RegistroCorrelativo, decimal solicitudtipoid, long usuarioid, int esinterno)
        {

            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_RNF_Registro_Solicitud @NoRegistroLiteral , @NoRegistroCorrelativo , @SolicitudTipo_id , @UsuarioID , @EsInterno";

            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@NoRegistroLiteral", Value = No_RegistroLiteral, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@NoRegistroCorrelativo", Value = No_RegistroCorrelativo, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@SolicitudTipo_id", Value = solicitudtipoid, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@UsuarioID", Value = usuarioid, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EsInterno", Value = esinterno, Direction = ParameterDirection.Input }
            };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure> {
                new ResultFromStoreProcedure { id = 0, mensaje = "Fallo Desconocido", respuesta = 0 }
            };

            long idsolicitud = 0;
            try
            {
                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();
                idsolicitud = resultado[0].id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                idsolicitud = 0;
            }


            return idsolicitud;
        }
        int SolicitudesEnProceso(string No_RegistroLiteral, int No_RegistroCorrelativo)
        {
            return db.Database.SqlQuery<int>("select count(*) from Tbl_Sol_Solicitud where No_RegistroLiteral = @p0 and No_RegistroCorrelativo = @p1 and Estado_id < 6 ", No_RegistroLiteral, No_RegistroCorrelativo).FirstOrDefault();
        }

        class JsonRespuesta
        {
            public int CodRespuesta { get; set; }
            public string StrMensaje { get; set; }
            public string NumeroTemporal { get; set; }
            public string NumeroExpediente { get; set; }
            public long Solicitud { get; set; }
            public string firma { get; set; }
            public int etapa_id { get; set; }
            public decimal etaparuta_id { get; set; }
            public int correlativoetapa_id { get; set; }
        }
        public JsonResult PrimerActualizacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }

            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.01);

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);

            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);

                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();
                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;
                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));

        }

        public JsonResult SegundaActualizacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }

            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.02);

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";


            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);

            

            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);

                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();
                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                   
                        jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                        jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                        jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                        jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;
                    
                  
                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudRatificacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }

            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.03);

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);

            SolicitudesProceso = 0;

            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);

                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();
                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;
                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudCancelacion_(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }


            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.04);
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Cancelación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudCancelacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }

            //.06  --> Cancelación
            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.06);
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Cancelación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }
        public JsonResult SolicitudInactivacion(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }


            decimal solicitudtipoid = (decimal)(model.Categoria_id + 0.04);
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudInactivacionInterno(Tbl_RNF_Registro model, decimal SolicitudTipoID)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }


            decimal solicitudtipoid = SolicitudTipoID;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudActivacionInterno(Tbl_RNF_Registro model, decimal SolicitudTipoID)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }


            decimal solicitudtipoid = ((decimal)model.Categoria_id) + SolicitudTipoID;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Activación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }

        public JsonResult SolicitudInactivacionTecnico(Tbl_RNF_Registro model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            bool esinterno = false;

            if (objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_RegistroLiteral == model.No_RegistroLiteral && Obj.No_RegistroCorrelativo == model.No_RegistroCorrelativo).FirstOrDefault();

            if ((tbl_RNF_Registro.Categoria_id == 8) && (tbl_RNF_Registro.Sub_Categoria_id == 2))
            {
                model.Categoria_id = 10;
            }


            decimal solicitudtipoid = 99.04M;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.CodRespuesta = 0;
            jsonRespuesta.StrMensaje = "No se ha realizado ninguna acción";

            int SolicitudesProceso = SolicitudesEnProceso(model.No_RegistroLiteral, model.No_RegistroCorrelativo);
            if (SolicitudesProceso == 0)
            {

                long idsolicitud = sp_actualizacion_rnfsolicitud(model.No_RegistroLiteral, model.No_RegistroCorrelativo, solicitudtipoid, objUs.intUsuario_id, objUs.EsInterno);
                if (idsolicitud != 0)
                {
                    Tbl_Sol_Solicitud tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
                    tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                         where d.Solicitud_id == idsolicitud
                                         select d).FirstOrDefault();


                    jsonRespuesta.CodRespuesta = 1;
                    jsonRespuesta.StrMensaje = "Solicitud de Inactivación iniciada con éxito";
                    jsonRespuesta.NumeroTemporal = tbl_Sol_Solicitud.Solicitud_NumeroTemporal ?? "";
                    jsonRespuesta.NumeroExpediente = tbl_Sol_Solicitud.Solicitud_NumeroExpediente ?? "";
                    jsonRespuesta.Solicitud = tbl_Sol_Solicitud.Solicitud_id;
                    jsonRespuesta.firma = tbl_Sol_Solicitud.Guid_id;

                }
                else
                {
                    jsonRespuesta.CodRespuesta = 3;
                    jsonRespuesta.StrMensaje = "Ocurrió un error durante la gestión";
                }

            }
            else
            {

                jsonRespuesta.CodRespuesta = 2;
                jsonRespuesta.StrMensaje = "Ya hay una solicitud en proceso";

            }

            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }


        public JsonResult IniciarGestEstapaSolicitud(Tbl_Sol_Solicitud model, bool AsignarTecnico = false)
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
                CodRespuesta = 0,
                StrMensaje = "No se ha realizado ninguna gestión",
                Solicitud = 0
            };

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure();
            string sqlQuery = "";

            SqlParameter[] sqlParams = new SqlParameter[] { };

            sqlQuery = "Exec SP_Sol_Enviar_Solicitud @Solicitud_id, @swupdatedby, @swupdatedbyinterno";

            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = model.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swupdatedby",  Value = tbl_Sol_Solicitud.swcreatedby, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = 0, Direction = System.Data.ParameterDirection.Input }
            };

            resultFromStoreProcedure = new ResultFromStoreProcedure
            {
                id = 0,
                mensaje = "Fallo desconocido.",
                respuesta = 0
            };


            resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();



            string strSubject = "";

            try
            {
                Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == 1 && Obj.Respuesta_id == 0).FirstOrDefault();

                if (tbl_Gest_EtapaSolicitud != null)
                {

                    int CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == 1).Count();

                    string EtapaSolicitud_GUIDid = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == 1 && Obj.Respuesta_id == 0).First().EtapaSolicitud_GUID_id;


                    Tbl_Gest_Etapa tbl_gest_Etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.EtapaRuta_id == tbl_Sol_Solicitud.SolicitudTipo_id && Obj.EtapaInicialRuta == true).First();
                    jsonRespuesta = new JsonRespuesta()
                    {
                        CodRespuesta = 1,
                        StrMensaje = "Proceso de inactivación realizado",
                        Solicitud = tbl_Sol_Solicitud.Solicitud_id,
                        firma = tbl_Sol_Solicitud.Guid_id,
                        etaparuta_id = tbl_Gest_EtapaSolicitud.EtapaRuta_id,
                        etapa_id = tbl_Gest_EtapaSolicitud.Etapa_id,
                        correlativoetapa_id = tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id
                    };

                    if (AsignarTecnico == null)
                    {
                        AsignarTecnico = false;
                    }

                    if (AsignarTecnico)
                    {
                        tbl_Sol_Solicitud.TecnicoAsignado_id = objUs.intUsuario_id;
                        tbl_Sol_Solicitud.TecnicoAsignadoFecha = DateTime.Now;
                        db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        CodRespuesta = 3,
                        StrMensaje = "No se ha contrado una ruta para este tigo de gestión"
                    };

                }
            }
            catch (Exception ex)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    CodRespuesta = 2,
                    StrMensaje = "No existe una ruta definida para esta gestión",
                    Solicitud = 0
                };
            }


            //strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - PRIMERA REVISIÓN -";

            //if (CantidadEtapasIniciales == 2)
            //{
            //    strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - SEGUNDA REVISIÓN -";
            //}

            //if (CantidadEtapasIniciales == 3)
            //{
            //    strSubject = "AVISO ELECTRÓNICO - SOLICITUD DE INSCRIPCIÓN - TERCERA REVISIÓN -";
            //}

            //string Mensaje = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Mail_SecretariaNotificacionElectronica_a_Secretaria(" + tbl_Sol_Solicitud.Solicitud_id.ToString() + ")").FirstOrDefault();


            return Json(jsonRespuesta);
        }

        private void LlenaTituloRevision(Tbl_RNF_Registro tbl_RNF_Registro)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 8);

            tableTitulo = new PdfPTable(7);

            //// Imagen superior
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/images/logoInabExcel.jpg"));

            logo.ScalePercent(80f);

            PdfPCell c1 = new PdfPCell(logo);


            c1.Colspan = 2;
            c1.Rowspan = 4;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nSOLICITUD DE CANCELACIÓN DE " + tbl_RNF_Registro.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("REV-0.1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Version", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Agosto 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\n REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }

        private void LlenaBanner(String Leyenda, string alineacion, string Color)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.Border = 0;

            if (Color == "Blanco")
            {
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            }

            if (Color == "Gris")
            {
                c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;
            }

            if (alineacion == "Centro")
            {
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
            }

            if (alineacion == "Derecha")
            {
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
            }

            if (alineacion == "Izquierda")
            {
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
            }

            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }

        private void CodigoRegistroInactivar(Tbl_RNF_Registro tbl_RNF_Registro)
        {

            tableBanner = new PdfPTable(4);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Código de Registro a Inactivar", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_RNF_Registro.No_Registro, fntTituloTabla));
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
            c1.Colspan = 2;
            c1.Border = 0;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }
        private void MotivoRegistroInactivar(string motivoInactivacion)
        {
            if (motivoInactivacion == null)
            {
                motivoInactivacion = "";
            }
            tableBanner = new PdfPTable(4);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase("Motivo de Inactivación", fntTituloTabla));
            c1.Colspan = 1;
            c1.Border = 0;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            c1 = new PdfPCell(new Phrase(motivoInactivacion, fntTituloTabla));
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }


        public string InactivacionRegistroProceso(Tbl_RNF_Registro model, string motivoInactivacion)
        {


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();
            if (tbl_RNF_Registro != null)
            {

                string strDir = "Documentos\\";
                string strFolder = Server.MapPath("~/") + strDir;
                DateTime hoy = DateTime.Now;
                string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
                string strNombre;
                string strDirArchivo;
                string strNombrePersona;

                strNombre = @"Inactivacion_" + tbl_RNF_Registro.No_Registro + fecha + ".pdf";
                strDirArchivo = strFolder + strNombre;

                var Enter = new Paragraph(" ");

                string[] Texto_Romano = new string[11];

                int intTexto_Romano = 1;
                {
                    Texto_Romano[1] = "I";
                    Texto_Romano[2] = "II";
                    Texto_Romano[3] = "III";
                    Texto_Romano[4] = "IV";
                    Texto_Romano[5] = "V";
                    Texto_Romano[6] = "VI";
                    Texto_Romano[7] = "VII";
                    Texto_Romano[8] = "VIII";
                    Texto_Romano[9] = "IX";
                    Texto_Romano[10] = "X";
                }

                string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();
                string strDirectorRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_NombreSubDirectorRegional](@p0,@p1)", tbl_RNF_Registro.Region_id, tbl_RNF_Registro.SubRegion_id).FirstOrDefault();
                string strDireccionSubRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_SubRegionCodigo](@p0,@p1)", tbl_RNF_Registro.Region_id, tbl_RNF_Registro.SubRegion_id).FirstOrDefault();

                Document doc = new Document(PageSize.LETTER);
                doc.SetMargins(1f, 1f, 25f, 50f);

                FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

                doc.Open();
                doc.Add(Enter);
                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlact = this.Url.Action();
                    LlenaBanner(urlact);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }


                LlenaTituloRevision(tbl_RNF_Registro);
                doc.Add(tableTitulo);
                doc.Add(Enter);


                LlenaBanner("Nombre del Director Subregional: " + strDirectorRegional, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("Dirección Subregional: " + strDireccionSubRegional, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                CodigoRegistroInactivar(tbl_RNF_Registro);
                doc.Add(tableBanner);
                doc.Add(Enter);

                MotivoRegistroInactivar(motivoInactivacion);
                doc.Add(tableBanner);
                doc.Add(Enter);
                doc.Add(Enter);

                LlenaBanner("F._________________________________________________________", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("Nombre del Propietario/Representante Legal", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("DPI", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                doc.Close();
                writer.Close();

                return "/" + strDir + strNombre;

            }
            else
            {

                return null;

            }
        }

        public string ActivarRegistroProceso(Tbl_RNF_Registro model, string motivoInactivacion)
        {


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();
            if (tbl_RNF_Registro != null)
            {

                string strDir = "Documentos\\";
                string strFolder = Server.MapPath("~/") + strDir;
                DateTime hoy = DateTime.Now;
                string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
                string strNombre;
                string strDirArchivo;
                string strNombrePersona;

                strNombre = @"Inactivacion_" + tbl_RNF_Registro.No_Registro + fecha + ".pdf";
                strDirArchivo = strFolder + strNombre;

                var Enter = new Paragraph(" ");

                string[] Texto_Romano = new string[11];

                int intTexto_Romano = 1;
                {
                    Texto_Romano[1] = "I";
                    Texto_Romano[2] = "II";
                    Texto_Romano[3] = "III";
                    Texto_Romano[4] = "IV";
                    Texto_Romano[5] = "V";
                    Texto_Romano[6] = "VI";
                    Texto_Romano[7] = "VII";
                    Texto_Romano[8] = "VIII";
                    Texto_Romano[9] = "IX";
                    Texto_Romano[10] = "X";
                }

                string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();
                string strDirectorRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_NombreSubDirectorRegional](@p0,@p1)", tbl_RNF_Registro.Region_id, tbl_RNF_Registro.SubRegion_id).FirstOrDefault();
                string strDireccionSubRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_SubRegionCodigo](@p0,@p1)", tbl_RNF_Registro.Region_id, tbl_RNF_Registro.SubRegion_id).FirstOrDefault();

                Document doc = new Document(PageSize.LETTER);
                doc.SetMargins(1f, 1f, 25f, 50f);

                FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

                doc.Open();
                doc.Add(Enter);
                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlact = this.Url.Action();
                    LlenaBanner(urlact);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }


                LlenaTituloRevision(tbl_RNF_Registro);
                doc.Add(tableTitulo);
                doc.Add(Enter);


                LlenaBanner("Nombre del Director Subregional: " + strDirectorRegional, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("Dirección Subregional: " + strDireccionSubRegional, "Izquierda", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                CodigoRegistroInactivar(tbl_RNF_Registro);
                doc.Add(tableBanner);
                doc.Add(Enter);

                MotivoRegistroInactivar(motivoInactivacion);
                doc.Add(tableBanner);
                doc.Add(Enter);
                doc.Add(Enter);

                LlenaBanner("F._________________________________________________________", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("Nombre del Propietario/Representante Legal", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                LlenaBanner("DPI", "Centro", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

                doc.Close();
                writer.Close();

                return "/" + strDir + strNombre;

            }
            else
            {

                return null;

            }
        }

        class InactivarRNF
        {
            public int result { get; set; }
            public string message { get; set; }
            public string ubicacion { get; set; }
        }
        [HttpPost]
        public JsonResult InactivarRegistro(Tbl_RNF_Registro model, string motivoInactivacion)
        {
            string ubicacion = InactivacionRegistroProceso(model, motivoInactivacion);
            InactivarRNF inactivarRNF = new InactivarRNF();
            inactivarRNF.result = 1;
            inactivarRNF.message = "Se ha realizado la inactivación";
            inactivarRNF.ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(inactivarRNF));
        }

        [HttpPost]
        public JsonResult ActivarRegistro(Tbl_RNF_Registro model, string motivoInactivacion)
        {
            string ubicacion = ActivarRegistroProceso(model, motivoInactivacion);
            InactivarRNF inactivarRNF = new InactivarRNF();
            inactivarRNF.result = 1;
            inactivarRNF.message = "Se ha realizado la activación";
            inactivarRNF.ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(inactivarRNF));
        }



        class RespuestasJSON
        {
            public int CodRespuesta { get; set; }
            public string strRespuesta { get; set; }
        }

        public ActionResult ActualizarCategoriaRegion_RNF(Tbl_RNF_Registro model)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();
            RespuestasJSON respuestasJSON = new RespuestasJSON();
            respuestasJSON.CodRespuesta = 0;
            respuestasJSON.strRespuesta = "No se ha efectuado ninguna gestión";
            if (tbl_RNF_Registro != null)
            {
                tbl_RNF_Registro.Region_id = model.Region_id;
                tbl_RNF_Registro.SubRegion_id = model.SubRegion_id;
                tbl_RNF_Registro.Categoria_id = model.Categoria_id;
                tbl_RNF_Registro.Sub_Categoria_id = model.Sub_Categoria_id;
                tbl_RNF_Registro.Sub_Sub_Categoria_id = model.Sub_Sub_Categoria_id;

                db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                db.SaveChanges();
                respuestasJSON.CodRespuesta = 1;
                respuestasJSON.strRespuesta = "Registro actualizado exitosamente";

            }


            return Json(respuestasJSON);
        }




        [HttpPost]
        public JsonResult GetSubCategorias(int Categoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                 where c.Categoria_id == Categoria
                                                                                 select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");

            return Json(new SelectList(SubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubSubCategorias(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

        }


        [HttpPost]
        public JsonResult GetMunicipios(int Departamento)
        {

            IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                                                         where c.Departamento_id == Departamento
                                                         select c);

            var Municipios = new SelectList(Municipio, "Municipio_id", "Municipio");

            return Json(new SelectList(Municipios, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubRegion(int Region)
        {

            IEnumerable<Tbl_Gral_SubRegion> SubRegion = (from c in db.Tbl_Gral_SubRegion
                                                         where c.Region_id == Region
                                                               && c.Estado_id == true
                                                         select c);

            var SubRegiones = new SelectList(SubRegion, "SubRegion_id", "Nombre_SubRegion");

            return Json(new SelectList(SubRegiones, "Value", "Text"));

        }


    }
}
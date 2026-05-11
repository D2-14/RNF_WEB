using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using RNF_Web.Models;
using PagedList;
using DotSpatial.Topology.Utilities;

namespace RNF_Web.Controllers
{
    public class Expediente_SecorfController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        class ResultRegistro
        {
            public int CodRespuesta { get; set; }
            public string StrRespuesta { get; set; }
            public string StrRegistro { get; set; }
            public string StrFirma { get; set; }
            public string StrSolicitud_id { get; set; }
        }


        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }

        public ActionResult Index(int? page, string sortOrder, string NoExpediente = null, string NoLicencia = null, string NoPOA = null, string NoDPI = null, string SolicitanteNombre = null)
        {


            ViewBag.NoExpediente = NoExpediente;
            ViewBag.NoLicencia = NoLicencia;
            ViewBag.NoPOA = NoPOA;
            ViewBag.NoDPI = NoDPI;
            ViewBag.SolicitanteNombre = SolicitanteNombre;


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            string Query;

            Query = $"  Select * \n";
            Query += $" From Tbl_Sol_Solicitud Solicitud \n";
            Query += $" Where (Solicitud.Procedencia_secorf = 1 or Solicitud.Procedencia_PinpepOld = 1 or Solicitud.Procedencia_PinpepNew = 1 or Solicitud.Procedencia_Probosque = 1) \n";

            if ((NoDPI != null) && (NoDPI.Trim() != ""))
            {
                string sDPI_Titular = NoDPI;
                sDPI_Titular = sDPI_Titular.Trim();
                sDPI_Titular = sDPI_Titular.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                Query += $"and Solicitud.DPI_Titular Like '%{sDPI_Titular}%'\n";
            }

            if ((NoExpediente != null) && (NoExpediente.Trim() != ""))
            {
                string sExpediente = NoExpediente;
                sExpediente = sExpediente.Trim();
                sExpediente = sExpediente.Replace(" ", "%");
                Query += $"and Solicitud.Procedencia_Expediente Like '%{sExpediente}%'\n";
            }

            if ((NoPOA != null) && (NoPOA.Trim() != ""))
            {
                string sProcedencia_POA = NoPOA;
                sProcedencia_POA = sProcedencia_POA.Trim();
                sProcedencia_POA = sProcedencia_POA.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                Query += $"and Solicitud.Procedencia_POA Like '%{sProcedencia_POA}%'\n";
            }

            if ((NoLicencia != null) && (NoLicencia.Trim() != ""))
            {
                string sNoLicencia = NoLicencia;
                sNoLicencia = sNoLicencia.Trim();
                sNoLicencia = sNoLicencia.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                Query += $"and Solicitud.Procedencia_Licencia Like '%{sNoLicencia}%'\n";
            }

            if ((SolicitanteNombre != null) && (SolicitanteNombre.Trim() != ""))
            {
                string sSolicitanteNombre = SolicitanteNombre;
                sSolicitanteNombre = sSolicitanteNombre.Trim();
                sSolicitanteNombre = sSolicitanteNombre.Replace(" ", "%").Replace("-", "%").Replace(".", "%").Replace("_", "%");
                Query += $"and Solicitud.Procedencia_NombreSolicitante Like '%{sSolicitanteNombre}%'\n";
            }


            List<Tbl_Sol_Solicitud> LstSolicitudes = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();


            ViewBag.SortNoExpediente = sortOrder == "Solicitud_NumeroExpediente" ? "Solicitud_NumeroExpediente_desc" : "Solicitud_NumeroExpediente";
            ViewBag.SortNoLicencia = sortOrder == "Solicitud_NumeroLicencia" ? "Solicitud_NumeroLicencia_desc" : "Solicitud_NumeroLicencia";
            ViewBag.SortNoPOA = sortOrder == "Solicitud_NumeroPOA" ? "Solicitud_NumeroPOA_desc" : "Solicitud_NumeroPOA";
            ViewBag.SortNoDPI = sortOrder == "Solicitud_NumeroDPI" ? "Solicitud_NumeroDPI_desc" : "Solicitud_NumeroDPI";
            ViewBag.SortNombre = sortOrder == "Solicitud_Nombre" ? "Solicitud_Nombre_desc" : "Solicitud_Nombre";
            ViewBag.SortModalidad = sortOrder == "Modalidad" ? "Modalidad_desc" : "Modalidad";
            ViewBag.SortFase = sortOrder == "Fase" ? "Fase_desc" : "Fase";
            ViewBag.SortTipoProyecto = sortOrder == "TipoProyecto" ? "TipoProyecto_desc" : "TipoProyecto";
            ViewBag.Sortswdatecreated = sortOrder == "swdatecreated" ? "swdatecreated_desc" : "swdatecreated";

            var ListarSolicitudes = (from d in LstSolicitudes select d);

            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "Solicitud_NumeroExpediente":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Expediente);
                    break;
                case "Solicitud_NumeroExpediente_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Expediente);
                    break;

                case "Solicitud_NumeroLicencia":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_Licencia);
                    break;
                case "Solicitud_NumeroLicencia_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_Licencia);
                    break;

                case "Solicitud_NumeroPOA":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_POA);
                    break;
                case "Solicitud_NumeroPOA_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_POA);
                    break;

                case "Solicitud_NumeroDPI":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.DPI_Titular);
                    break;
                case "Solicitud_NumeroDPI_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.DPI_Titular);
                    break;

                case "Solicitud_Nombre":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Procedencia_NombreSolicitante);
                    break;
                case "Solicitud_Nombre_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Procedencia_NombreSolicitante);
                    break;

                case "swdatecreated":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.swdatecreated);
                    break;
                case "swdatecreated_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.swdatecreated);
                    break;

            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            string sqlquerytecnicos = "SELECT *\n";
            sqlquerytecnicos += "FROM Tbl_Seg_Usuario Usuario\n";
            sqlquerytecnicos += "WHERE EXISTS (\n";
            sqlquerytecnicos += "\tSELECT *\n";
            sqlquerytecnicos += "\tFROM Tbl_Seg_UsuarioRol URol\n";
            sqlquerytecnicos += "\tWHERE URol.Usuario_id = Usuario.Usuario_id\n";
            sqlquerytecnicos += "\tAND URol.Estado_id = 1\n";
            sqlquerytecnicos += "\tAND URol.Rol_id IN (\n";
            sqlquerytecnicos += "\t\tSELECT MAX(TecnicoForestal) TecnicoForestal\n";
            sqlquerytecnicos += "\t\tFROM Tbl_Gral_PerfilesRol Perfil\n";
            sqlquerytecnicos += "\t)\n";
            sqlquerytecnicos += ")\n";
            sqlquerytecnicos += "AND Usuario.Estado = 1\n";
            sqlquerytecnicos += "ORDER BY email\n";
            sqlquerytecnicos += "\n";

            List<Tbl_Seg_Usuario> tbl_Seg_Usuarios = db.Tbl_Seg_Usuario.SqlQuery(sqlquerytecnicos).ToList();
            ViewBag.tbl_Seg_Usuarios = tbl_Seg_Usuarios;



            return View(ListarSolicitudes.ToPagedList(pageNumber, pageSize));
        }


        // GET: Sol_Solicitud/Create
        public ActionResult Crear()
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud;
            tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();
            tbl_Sol_Solicitud.swcreatedbyinterno = objUs.EsInterno == 1 ? true : false;
            tbl_Sol_Solicitud.Categoria_id = 0;
            tbl_Sol_Solicitud.Sub_Categoria_id = 0;
            tbl_Sol_Solicitud.Sub_Sub_Categoria_id = 0;
            tbl_Sol_Solicitud.AreaTotalFincas = 0;
            tbl_Sol_Solicitud.Region_id = 0;
            tbl_Sol_Solicitud.SubRegion_id = 0;

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Procedencia_Secorf == true), "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Procedencia_Secorf == true), "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Procedencia_Secorf == true && obj.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id) || (obj.Procedencia_Secorf == true && obj.Sub_Sub_Categoria_id == 0 && tbl_Sol_Solicitud.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, nameof(Tbl_Gral_Region.Id_Region), nameof(Tbl_Gral_Region.Nombre_RegionCompleto), tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), nameof(Tbl_Gral_SubRegion.SubRegion_id), nameof(Tbl_Gral_SubRegion.Nombre_SubRegionCompleto), tbl_Sol_Solicitud.SubRegion_id);



            return View(tbl_Sol_Solicitud);

        }


        [HttpPost]
        public JsonResult CrearNuevoRegistro(Tbl_Sol_Solicitud model)
        {
            //Tbl_Sol_Solicitud model = new Tbl_Sol_Solicitud
            //{
            //    Region_id = modelo.Region_id,
            //    SubRegion_id = modelo.SubRegion_id,
            //    Categoria_id = modelo.Categoria_id,
            //    Sub_Categoria_id = modelo.Sub_Categoria_id,
            //    Sub_Sub_Categoria_id = modelo.Sub_Sub_Categoria_id,
            //    DPI_Titular = modelo.DPI_Titular,
            //    Procedencia_POA = modelo.Procedencia_POA,
            //    Procedencia_Licencia = modelo.Procedencia_Licencia,
            //    Procedencia_Modalidad = modelo.Procedencia_Modalidad,
            //    Procedencia_NombreSolicitante = modelo.Procedencia_NombreSolicitante,
            //    Procedencia_Expediente = modelo.Procedencia_Expediente,
            //    Procedencia_InformeTecnico = modelo.Procedencia_InformeTecnico,
            //    Procedencia_FechaInicioPeriodo = modelo.Procedencia_FechaInicioPeriodo,
            //    Procedencia_FechaFinPeriodo = modelo.Procedencia_FechaFinPeriodo,
            //};

            model.AreaTotalFincas = 0;
            model.Region_id = 0;
            model.SubRegion_id = 0;
            string errores = "";

            ResultRegistro resultRegistro = new ResultRegistro();
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

            bool boolEsInterno = false;

            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            bool erroresencontrados = false;

            if (!DPI_Valido(model.DPI_Titular))
            {
                erroresencontrados = true;
                errores = "\n -> DPI no es valido";
            }

            if (!erroresencontrados)
            {
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

                model.Solicitud_id = lngIdt;
                model.SolicitudTipo_id = 100;
                model.Solicitud_NumeroTemporal = $"{lngIdt}-{DateTime.Now.ToString("yyyy")}";

                model.Estado_id = 0;
                model.Guid_id = Guid.NewGuid().ToString();

                model.Procedencia_secorf = true;

                model.swdatecreated = DateTime.Now;
                model.swcreatedby = 0;
                model.swcreatedbyinterno = false;

                model.swdateupdated = DateTime.Now;
                model.swupdatedbyinterno = true;
                model.swupdatedby = objUs.intUsuario_id;

                model.Notificacion_Direccion = "";
                model.Notificacion_Departamento_id = 7;
                model.Notificacion_Municipio_id = 74;
                model.TecnicoAsignado_id = objUs.intUsuario_id;

                db.Tbl_Sol_Solicitud.Add(model);
                db.SaveChanges();

                resultRegistro.StrFirma = model.Guid_id;
                resultRegistro.StrSolicitud_id = model.Solicitud_id.ToString();

                resultRegistro.CodRespuesta = 1;
                resultRegistro.StrRespuesta = "Se ha generado el registro ";
                resultRegistro.StrRegistro = "";

            }
            else
            {
                resultRegistro.CodRespuesta = 2;
                resultRegistro.StrRespuesta = "No se ha podido registrar " + errores;
            }


            return Json(JsonConvert.SerializeObject(resultRegistro));
        }




        [HttpPost]
        public JsonResult GetSubCategorias(int Categoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                 where c.Categoria_id == Categoria &&
                                                                                       c.Procedencia_Secorf == true
                                                                                 select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");

            return Json(new SelectList(SubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubSubCategorias(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Procedencia_Secorf == true
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult GetSubRegion(int Region)
        {
            // Done  
            var SubRegiones = new SelectList(db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == Region && Obj.Estado_id == true), nameof(Tbl_Gral_SubRegion.SubRegion_id), nameof(Tbl_Gral_SubRegion.Nombre_SubRegionCompleto));
            return Json(new SelectList(SubRegiones, "Value", "Text"));

        }


        public JsonResult ValidarDPI(string DPI)
        {
            DPI = DPI.Replace(" ", "").Replace("_", "").Replace("-", "").Replace(".", "");

            ResultRegistro resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 1,
                StrRespuesta = "DPI Valido",
                StrRegistro = DPI,
            };


            if (!DPI_Valido(DPI))
            {
                resultRegistro = new ResultRegistro()
                {
                    CodRespuesta = 2,
                    StrRespuesta = "DPI Invalido"
                };
            }

            return Json(resultRegistro);
        }


        public JsonResult AsignarTecnicoExpedienteOficio(Tbl_Sol_Solicitud model)
        {
            Reply reply = new Reply
            {
                result = 0,
                message = "No se ha realizado ninguna gestión"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(reply);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure()
            {
                respuesta = 0,
                mensaje = "No se realizó ninguna gestión",
                id = 0
            };
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if(tbl_Sol_Solicitud != null)
            {
                try
                {
                    string strSqlQuery = "EXEC SP_Sol_AsignarTecnico_ExpedienteOficio @Solicitud_id, @TecnicoAsignado_id, @Usuario_id, @esinterno";

                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter {ParameterName = "@Solicitud_id", Value = model.Solicitud_id, Direction = ParameterDirection.Input},
                        new SqlParameter {ParameterName = "@TecnicoAsignado_id", Value = model.TecnicoAsignado_id, Direction = ParameterDirection.Input},
                        new SqlParameter {ParameterName = "@Usuario_id", Value = objUs.intUsuario_id, Direction = ParameterDirection.Input},
                        new SqlParameter {ParameterName = "@esinterno", Value = objUs.EsInterno, Direction = ParameterDirection.Input},
                    };
                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(strSqlQuery, sqlParameters).FirstOrDefault();


                }catch(Exception ex)
                {
                    resultFromStoreProcedure = new ResultFromStoreProcedure()
                    {
                        respuesta = 2,
                        mensaje = "Ocurrió un error... " + ex.Message,
                        id = 0
                    };
                }
            }

            reply.result = resultFromStoreProcedure.respuesta;
            reply.message = resultFromStoreProcedure.mensaje;
            reply.data = resultFromStoreProcedure.id;


            return Json(reply);

        }


    }
}

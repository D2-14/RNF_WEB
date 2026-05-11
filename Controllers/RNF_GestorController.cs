using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_GestorController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Gestor
        public ActionResult Index(string No_Registro)
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

            Tbl_Gral_ParametrosGenerales tbl_Gral_ParametrosGenerales = db.Tbl_Gral_ParametrosGenerales.FirstOrDefault();
            fc_Gral_RNF_Configuracion_Result permisos = (from d in db.fc_Gral_RNF_Configuracion(No_Registro, objUs.intUsuario_id, boolEsInterno)
                                                         select d).FirstOrDefault();
            EdicionRNFGrants objRNFGrants = new EdicionRNFGrants
            {
                Agregar = (bool)permisos.Agregar,
                Editar = (bool)permisos.Editar,
                Borrar = (bool)permisos.Borrar,
                Estado_id = (int)permisos.Estado_id,
                Estado_Registro = permisos.Estado_Registro,
                Usuario = permisos.Usuario,
                GenerarConstancia = (bool)permisos.GenerarConstancia
            };

            Session[Constants.session_EdicionRNFGrants] = objRNFGrants;

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

            bool boolPropietario = false;
            bool boolFinca = false;
            bool boolEmpresaEntidad = false;
            bool boolMotosierra = false;
            bool boolTecnicoProfesional = false;
            bool boolDocumentos = false;


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(x => x.No_Registro == No_Registro).FirstOrDefault();
            ViewBag.No_Registro = No_Registro;

            int categoriaid = (int)tbl_RNF_Registro.Categoria_id;
            int subcategoriaid = (int)tbl_RNF_Registro.Sub_Categoria_id;

            if ((categoriaid == 1) || (categoriaid == 2) || (categoriaid == 3) || (categoriaid == 4) || (categoriaid == 6))
            {
                boolPropietario = boolFinca = boolDocumentos = true;
            }

            if ((categoriaid == 5) || ((categoriaid == 8) && (subcategoriaid == 2)) || (categoriaid == 9))
            {
                boolPropietario = boolEmpresaEntidad = boolDocumentos = true;
            }

            if (categoriaid == 7)
            {
                boolTecnicoProfesional = boolDocumentos = true;
            }

            if ((categoriaid == 8) && (subcategoriaid == 1))
            {
                boolPropietario = boolMotosierra = boolDocumentos = true;
            }

            ViewBag.boolPropietario = boolPropietario;
            ViewBag.boolFinca = boolFinca;
            ViewBag.boolEmpresaEntidad = boolEmpresaEntidad;
            ViewBag.boolMotosierra = boolMotosierra;
            ViewBag.boolTecnicoProfesional = boolTecnicoProfesional;
            ViewBag.boolDocumentos = boolDocumentos;



            return View(tbl_RNF_Registro);
        }

        public ActionResult Create()
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

            EdicionRNFGrants objRNFGrants = new EdicionRNFGrants();
            objRNFGrants.Agregar = true;
            objRNFGrants.Editar = true;
            objRNFGrants.Borrar = true;
            objRNFGrants.Estado_id = 1;
            objRNFGrants.Estado_Registro = "";
            objRNFGrants.Usuario = "";

            Session[Constants.session_EdicionRNFGrants] = objRNFGrants;

            Tbl_RNF_Registro tbl_RNF_Registro = new Tbl_RNF_Registro();
            tbl_RNF_Registro.Categoria_id = 0;
            tbl_RNF_Registro.Sub_Categoria_id = 0;
            tbl_RNF_Registro.Sub_Sub_Categoria_id = 0;
            tbl_RNF_Registro.Region_id = 0;
            tbl_RNF_Registro.SubRegion_id = 0;


            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_RNF_Registro.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_RNF_Registro.Categoria_id && obj.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_RNF_Registro.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_RNF_Registro.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_RNF_Registro.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_RNF_Registro.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_RNF_Registro.SubRegion_id);


            return View(tbl_RNF_Registro);
        }

        public ActionResult InactivarRegistro(string No_Registro, string Tipo)
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
            ViewBag.Tipo = SecurEncryptDecrypt.DecryptString(Tipo);
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

            List<Tbl_RNF_Registro_InactivacionTecnico_Tipo> tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in db.Tbl_RNF_Registro_InactivacionTecnico_Tipo
                                                                                                          where d.InactivacionTecnicoTipo_id == 2
                                                                                                          select d).ToList();

            List<Tbl_RNF_Registro_Bitacora> tbl_RNF_Registro_Bitacoras = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                          where d.No_Registro == No_Registro
                                                                          select d).ToList();

            if (tbl_RNF_Registro_Bitacoras == null)
            {
                tbl_RNF_Registro_Bitacoras = new List<Tbl_RNF_Registro_Bitacora>();
            }

            int CountInactivacionTemporal = 0;
            int CountInactivacionDefinitiva = 0;

            foreach (var item in tbl_RNF_Registro_Bitacoras)
            {
                item.InactivacionTemporal = item.InactivacionTemporal ?? false;
                item.InactivacionDefinitiva = item.InactivacionDefinitiva ?? false;
            }

            CountInactivacionTemporal = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionTemporal == true).Count();
            CountInactivacionDefinitiva = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionDefinitiva == true).Count();

            tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                          where d.InactivacionTecnicoTipo_id != 0
                                                          select d).ToList();


            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion");


            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == tbl_RNF_Registro.Categoria_id).ToList();
            if (tbl_RNF_Registro.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == tbl_RNF_Registro.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");

            return View(tbl_RNF_Registro);
        }

        public ActionResult ActivarRegistro(string No_Registro, string Tipo)
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
            ViewBag.Tipo = SecurEncryptDecrypt.DecryptString(Tipo);

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


        public JsonResult GeneraInactivacionInterno(string No_Registro, int InactivacionTecnico_Tipo_id, string Motivo, int TiempoInactivacion, decimal SolicituTipo_id)
        {
            ResultRegistro resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 0,
                StrRespuesta = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(resultRegistro);
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                resultRegistro = new ResultRegistro()
                {
                    CodRespuesta = 2,
                    StrRespuesta = "No se encontró el número de registro"
                };
                return Json(resultRegistro);
            }



            tbl_RNF_Registro.InactivacionTecnicoTipo_id = InactivacionTecnico_Tipo_id;
            tbl_RNF_Registro.Descripcion_InactivacionTecnico = Motivo;
            tbl_RNF_Registro.swdateupdated = DateTime.Now;
            tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Registro.swupdatedbyinterno = boolEsInterno;
            db.Entry(tbl_RNF_Registro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 1,
                StrRespuesta = "Inactivación técnica solicitada exitosamente"
            };



            return Json(resultRegistro);
        }

        public JsonResult GeneraInactivacionTecnico(string No_Registro, int InactivacionTecnico_Tipo_id, string Motivo, int TiempoInactivacion)
        {

            ResultRegistro resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 0,
                StrRespuesta = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(resultRegistro);
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                resultRegistro = new ResultRegistro()
                {
                    CodRespuesta = 2,
                    StrRespuesta = "No se encontró el número de registro"
                };
                return Json(resultRegistro);
            }



            tbl_RNF_Registro.InactivacionTecnicoTipo_id = InactivacionTecnico_Tipo_id;
            tbl_RNF_Registro.Descripcion_InactivacionTecnico = Motivo;
            tbl_RNF_Registro.swdateupdated = DateTime.Now;
            tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Registro.swupdatedbyinterno = boolEsInterno;
            db.Entry(tbl_RNF_Registro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 1,
                StrRespuesta = "Inactivación técnica solicitada exitosamente"
            };



            return Json(resultRegistro);
        }

        public JsonResult GeneraActivacion(string No_Registro, string Motivo)
        {

            ResultRegistro resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 0,
                StrRespuesta = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(resultRegistro);
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                resultRegistro = new ResultRegistro()
                {
                    CodRespuesta = 2,
                    StrRespuesta = "No se encontró el número de registro"
                };
                return Json(resultRegistro);
            }

            tbl_RNF_Registro.Descripcion_InactivacionTecnico = Motivo;
            tbl_RNF_Registro.swdateupdated = DateTime.Now;
            tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Registro.swupdatedbyinterno = boolEsInterno;
            db.Entry(tbl_RNF_Registro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            resultRegistro = new ResultRegistro()
            {
                CodRespuesta = 1,
                StrRespuesta = "Inactivación técnica solicitada exitosamente"
            };

            return Json(resultRegistro);
        }

        class ResultRegistro
        {
            public int CodRespuesta { get; set; }
            public string StrRespuesta { get; set; }
            public string StrRegistro { get; set; }
        }


        public int DiferenciaAnio(DateTime Fecha)
        {
            int intDias;
            TimeSpan dias = DateTime.Now.Subtract(Fecha);

            intDias = dias.Days;

            return intDias / 364;
        }

        public bool No_NIT_Valido(string No_NIT)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_NIT_Valido(@p0)", No_NIT).FirstOrDefault(); ;
        }

        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }
        public JsonResult CrearNuevoRegistro(Tbl_RNF_Registro model)
        {

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
            }

            //Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
            //                                                 where d.No_Documento == model.DPI_Titular && d.DocumentoID_Tipo == 1
            //                                                 select d).FirstOrDefault();

            //if((tbl_Seg_UsuarioExterno == null) && (model.Categoria_id == 7))
            //{
            //    erroresencontrados = true;
            //    resultRegistro = new ResultRegistro()
            //    {
            //        CodRespuesta = 2,
            //        StrRespuesta = "Para crear este tipo de registro, debe existir el usuario"
            //    };
            //    return Json(JsonConvert.SerializeObject(resultRegistro));
            //}


            if (!erroresencontrados)
            {
                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                {
                    new ResultFromStoreProcedure {
                        id = 0,
                        mensaje= "Fallo desconocido.",
                        respuesta = 0
                    }
                };
                SqlParameter[] sqlParams;
                string sqlQuery;
                sqlQuery = "Exec SP_RNF_InsUpd_Registro @Fecha_De_Vencimiento, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id, @Sub_Region_id, @DPI_Titular, @Usuario_id, @EsInterno ";

                sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@Fecha_De_Vencimiento",  Value = model.Fecha_De_Vencimiento, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Categoria_id",  Value = model.Categoria_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = model.Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = model.Sub_Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Region_id",  Value = model.Region_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Sub_Region_id",  Value = model.SubRegion_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@DPI_Titular",  Value = model.DPI_Titular, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                resultRegistro.CodRespuesta = 1;
                resultRegistro.StrRespuesta = "Se ha generado el registro " + resultado[0].mensaje;
                resultRegistro.StrRegistro = resultado[0].mensaje;

                if (model.Categoria_id == 7)
                {
                    sqlQuery = "Exec SP_RNF_TecnicoProfesional_replicarDatos @UsuarioExterno_id, @No_Registro ";
                    sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@UsuarioExterno_id",  Value = 0, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@No_Registro",  Value = resultado[0].mensaje, Direction = System.Data.ParameterDirection.Input }
                    };

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();
                }

            }
            else
            {
                resultRegistro.CodRespuesta = 2;
                resultRegistro.StrRespuesta = "No se ha podido registrar ";
            }


            return Json(JsonConvert.SerializeObject(resultRegistro));
        }

        class CategoriaRegionRegistroActual
        {
            public int regionid { get; set; }
            public int subregionid { get; set; }
            public int categoriaid { get; set; }
            public int subcategoriaid { get; set; }
            public int subsubcategoriaid { get; set; }
        }

        class JsonRespuestaFecha
        {
            public int CodResult { get; set; }
            public string StrMensaje { get; set; }
            public DateTime Fecha_De_Vencimiento { get; set; }
            public string StrFecha_De_Vencimiento { get; set; }
        }

        public JsonResult ConsultaFechaCambioRegistro(string No_Registro)
        {
            DateTime swdatecreated = DateTime.Now;
            JsonRespuestaFecha jsonRespuestaFecha = new JsonRespuestaFecha();
            jsonRespuestaFecha.CodResult = 0;
            jsonRespuestaFecha.StrMensaje = "No se ha encontrado ninguna fecha";
            jsonRespuestaFecha.Fecha_De_Vencimiento = swdatecreated;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            if (tbl_RNF_Registro != null)
            {

                jsonRespuestaFecha.CodResult = 1;
                jsonRespuestaFecha.StrMensaje = "Fecha encontrada";
                jsonRespuestaFecha.Fecha_De_Vencimiento = tbl_RNF_Registro.Fecha_De_Vencimiento ?? swdatecreated;

            }

            int No_Mes = jsonRespuestaFecha.Fecha_De_Vencimiento.Month;
            string sqlQuery;
            SqlParameter[] sqlParams;
            sqlQuery = "select [dbo].[Fnc_Gral_MesNombre](@p0)";
            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@p0",  Value = No_Mes, Direction = System.Data.ParameterDirection.Input }
            };
            string FechaTexto = db.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault();

            string FechaReal = $"{jsonRespuestaFecha.Fecha_De_Vencimiento.Day} de {FechaTexto} de {jsonRespuestaFecha.Fecha_De_Vencimiento.Year}";

            jsonRespuestaFecha.StrFecha_De_Vencimiento = FechaReal;
            return Json(JsonConvert.SerializeObject(jsonRespuestaFecha));
        }

        public JsonResult ConsultaRegistroActual(Tbl_RNF_Registro model)
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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            CategoriaRegionRegistroActual categoriaRegionRegistroActual = (from d in db.Tbl_RNF_Registro
                                                                           where d.No_Registro == model.No_Registro
                                                                           select new CategoriaRegionRegistroActual
                                                                           {
                                                                               regionid = (int)d.Region_id,
                                                                               subregionid = (int)d.SubRegion_id,
                                                                               categoriaid = (int)d.Categoria_id,
                                                                               subcategoriaid = (int)d.Sub_Categoria_id,
                                                                               subsubcategoriaid = (int)d.Sub_Sub_Categoria_id
                                                                           }).FirstOrDefault();

            return Json(JsonConvert.SerializeObject(categoriaRegionRegistroActual));
        }

        public JsonResult ActualizarFechaVencimiento(Tbl_RNF_Registro model)
        {
            Reply reply = new Reply
            {
                result = 0,
                message = "No se ha realizado ninguna gestión",
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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == model.No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                reply = new Reply
                {
                    result = 2,
                    message = "No se ha encontrado el registro indicado",
                };
            }
            else
            {
                try
                {
                    tbl_RNF_Registro.Fecha_De_Vencimiento = model.Fecha_De_Vencimiento;
                    tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
                    tbl_RNF_Registro.swupdatedbyinterno = boolEsInterno;
                    tbl_RNF_Registro.swdateupdated = DateTime.Now;
                    db.Entry(tbl_RNF_Registro).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    reply = new Reply
                    {
                        result = 1,
                        message = "Se ha actualizado correctamente la fecha de vencimiento",
                    };
                }
                catch (Exception ex)
                {
                    reply = new Reply
                    {
                        result = 3,
                        message = "Ocurrió un error... Exc." + ex.Message,
                        data = ex,
                    };
                }

            }

            return Json(reply);
        }

    }
}
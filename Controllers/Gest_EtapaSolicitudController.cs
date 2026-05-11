using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Net.Mail;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace RNF_Web.Controllers
{
    public class Gest_EtapaSolicitudController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        public JsonResult JsonReturnEtapa(long varSolicitud_id, int VarCorrelativoId, int SelectedEtapaId)
        {

            string jsonResult;

            jsonResult = "{\"CodRespuesta\":"
                      + "\"" + 0 + "\","
                      + "\"strRespuesta\":" + "\"" + "No se logró retornar la etapa." + "\"}";


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);


            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                             { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            string sqlQuery = "Exec SP_Gest_EtapaSolicitudReturnForzado @Solicitud_id, @Correlativo_id, @Selected_Etapa_id, @Usuario_id";

            SqlParameter[] sqlParams = new SqlParameter[]
                   {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = varSolicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Correlativo_id",  Value = VarCorrelativoId, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Selected_Etapa_id",  Value = SelectedEtapaId, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input }
                   };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            jsonResult = "{\"CodRespuesta\":"
                        + "\"" + resultado[0].respuesta + "\","
                        + "\"strRespuesta\":" + "\"" + resultado[0].mensaje + "\"}";


            return Json(jsonResult);



        }


        public JsonResult JsonReturnEtapaAud(long varSolicitud_id, int VarCorrelativoId, int SelectedEtapaId,string varSelectEtapa, string varSolicitante, string varFecha, string varMotivo)
        {

            string jsonResult;

            jsonResult = "{\"CodRespuesta\":"
                      + "\"" + 0 + "\","
                      + "\"strRespuesta\":" + "\"" + "No se logró retornar la etapa." + "\"}";


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);


            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                             { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            string ip = Request.UserHostAddress;

            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }


            // Procedimiento Anterior sin auditoria de regreso de tarea
            //string sqlQuery = "Exec SP_Gest_EtapaSolicitudReturnForzado @Solicitud_id, @Correlativo_id, @Selected_Etapa_id, @Usuario_id";

            //SqlParameter[] sqlParams = new SqlParameter[]
            //       {
            //           new SqlParameter { ParameterName = "@Solicitud_id",  Value = varSolicitud_id, Direction = System.Data.ParameterDirection.Input },
            //           new SqlParameter { ParameterName = "@Correlativo_id",  Value = VarCorrelativoId, Direction = System.Data.ParameterDirection.Input },
            //           new SqlParameter { ParameterName = "@Selected_Etapa_id",  Value = SelectedEtapaId, Direction = System.Data.ParameterDirection.Input },
            //           new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input }
            //       };


            string sqlQuery = "Exec SP_Gest_EtapaSolicitudReturnForzado_ @Solicitud_id, @Correlativo_id, @Selected_Etapa_id, @Usuario_id,@Solicitante,@FechaSolicita,@Motivo,@IP,@NombreEtapa";

            SqlParameter[] sqlParams = new SqlParameter[]
                   {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = varSolicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Correlativo_id",  Value = VarCorrelativoId, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Selected_Etapa_id",  Value = SelectedEtapaId, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Solicitante",  Value = varSolicitante, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@FechaSolicita",  Value =varFecha, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Motivo",  Value = varMotivo, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@IP",  Value = ip, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@NombreEtapa",  Value = varSelectEtapa, Direction = System.Data.ParameterDirection.Input }
                   };


            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            jsonResult = "{\"CodRespuesta\":"
                        + "\"" + resultado[0].respuesta + "\","
                        + "\"strRespuesta\":" + "\"" + resultado[0].mensaje + "\"}";


            return Json(jsonResult);



        }

        public ActionResult IndexHistorico(string Guid)
        {
            
            String Query;

            Query = "Select EtapaSolicitud.* ";
            Query += "From Tbl_Gest_EtapaSolicitud EtapaSolicitud,Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud ";
            Query += "Where Solicitud.Guid_id = '" + Guid + "' ";
            Query += "and   Solicitud.Solicitud_id = EtapaSolicitud.Solicitud_id ";
            Query += "and   Etapa.Etapa_id = EtapaSolicitud.Etapa_id ";
            Query += "and   Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id ";


            List<Tbl_Gest_EtapaSolicitud> LstSolicitudes = db.Tbl_Gest_EtapaSolicitud.SqlQuery(Query).ToList();

            return View(LstSolicitudes);
        }

        public List<Tbl_Gest_EtapaSolicitud> VerificarSolicitudesReturn(Usuario objUs, DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "", string Expediente=null)
        {
            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;

            String Query, QueryComplementoBusqueda1, QueryComplementoBusqueda2, QueryComplementoBusquedaExpediente;
            QueryComplementoBusqueda1 = "";
            QueryComplementoBusqueda2 = "";
            QueryComplementoBusquedaExpediente = "";

            QueryComplementoBusqueda1 = "and  EtapaSolicitud.EtapaSolicitudEstado_id in (1,2) \n";

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;

            if (string.IsNullOrEmpty(Expediente))
            {
                if ((fecha_inicio != null) && (fecha_fin != null))
                {
                    swdateinicio = (DateTime)fecha_inicio;
                    swdatefinal = (DateTime)fecha_fin;
                    swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    //QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
                    QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
                }
                else

                {
                    swdateinicio = swdatenow.AddDays(-30).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                    swdatefinal = swdatenow;
                    //QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
                    QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
                }

            }
            


            Query = "Select top 1000 EtapaSolicitud.* \n";
            Query += "From Tbl_Gest_EtapaSolicitud EtapaSolicitud,Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud \n";
            Query += "Where Solicitud.Solicitud_id = EtapaSolicitud.Solicitud_id \n";


            Query += "and  Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "and  Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "and (Etapa.Nombre_Etapa collate SQL_Latin1_General_Cp1_CI_AI = '" + Nombre_Etapa_Param + "' or '" + Nombre_Etapa_Param + "' = '') \n";
            Query += QueryComplementoBusqueda1;
            Query += QueryComplementoBusqueda2;
            //Query += "and  EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "and  (EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "or  EtapaSolicitud.EtapaSolicitudEstado_id = 3) \n";


            //Query += " and  ((Etapa.Rol_id not in (" + Rol_Juridico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Juridico.ToString() + ") and  Solicitud.JuridicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            //Query += " and  ((Etapa.Rol_id not in (" + Rol_Tecnico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Tecnico.ToString() + ") and  Solicitud.TecnicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += "and  exists \n";
            Query += "( \n";
            Query += "	  Select * \n";
            Query += "	  From dbo.fc_Seg_Sel_RegionSubRegionUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	  Where Region_id = Solicitud.Region_id \n";
            Query += "	  and   SubRegion_id = Solicitud.SubRegion_id \n";
            Query += ") \n";
            //Query += "and exists  \n";
            //Query += "( \n";
            //Query += "	Select * \n";
            //Query += "	From dbo.fc_Seg_Sel_RolUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            //Query += "	Where Rol_id = Etapa.Rol_id \n";
            //Query += ") \n";

            if (!string.IsNullOrEmpty(Expediente))
            {
                QueryComplementoBusquedaExpediente = $" and Solicitud.Solicitud_NumeroExpediente='" + Expediente + "' ";
              
            }
            Query += QueryComplementoBusquedaExpediente;
            Query += "AND EtapaSolicitud.Solicitud_id<>16704 order by EtapaSolicitud.swdatecreated desc \n";

            return db.Tbl_Gest_EtapaSolicitud.SqlQuery(Query).ToList();
        }


        public List<Tbl_Gest_EtapaSolicitud> VerificarSolicitudes(Usuario objUs, DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "")
        {

            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;


            String Query, QueryComplementoBusqueda1, QueryComplementoBusqueda2;
            QueryComplementoBusqueda1 = "";
            QueryComplementoBusqueda2 = "";

            if (((Pendientes != null) && (Finalizados != null)) || ((Pendientes == null) && (Finalizados == null)))
            {
                Pendientes = 1;
                Finalizados = 1;
                QueryComplementoBusqueda1 = "and  EtapaSolicitud.EtapaSolicitudEstado_id in (1,2,3) \n";
            }

            if ((Pendientes != null) && (Finalizados == null))
            {
                QueryComplementoBusqueda1 = "and  EtapaSolicitud.EtapaSolicitudEstado_id in (1,2) \n";
            }

            if ((Pendientes == null) && (Finalizados != null))
            {
                QueryComplementoBusqueda1 = "and  EtapaSolicitud.EtapaSolicitudEstado_id in (3) \n";
            }

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;


            if ((fecha_inicio != null) && (fecha_fin != null))
            {
                swdateinicio = (DateTime)fecha_inicio;
                swdatefinal = (DateTime)fecha_fin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
            }
            else
            {
                swdateinicio = swdatenow.AddDays(-30).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
                QueryComplementoBusqueda2 += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
            }



            Query = "DECLARE @ContadadorRecepcion AS int SET @ContadadorRecepcion=(SELECT count (*) FROM dbo.Tbl_Gest_EtapaSolicitud es  JOIN dbo.Tbl_Gest_Etapa e ON e.EtapaRuta_id=es.EtapaRuta_id AND es.Etapa_id=e.Etapa_id \n";
            Query += "WHERE   es.EtapaSolicitudEstado_id=1  AND e.Nombre_Etapa='Revisión de expediente') \n";
            Query += "IF @ContadadorRecepcion>0 BEGIN \n";            
            
            Query += "Select EtapaSolicitud.* \n";
            Query += "From Tbl_Gest_EtapaSolicitud EtapaSolicitud,Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud \n";
            Query += "Where Solicitud.Solicitud_id = EtapaSolicitud.Solicitud_id \n";


            Query += "and  Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "and  Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "and (Etapa.Nombre_Etapa collate SQL_Latin1_General_Cp1_CI_AI = '" + Nombre_Etapa_Param + "' or '" + Nombre_Etapa_Param + "' = '') \n";
            Query += QueryComplementoBusqueda1;
            Query += QueryComplementoBusqueda2;
            //Query += "and  EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "and  (EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "or  EtapaSolicitud.EtapaSolicitudEstado_id = 3) \n";


            Query += " and  ((Etapa.Rol_id not in (" + Rol_Juridico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Juridico.ToString() + ") and  Solicitud.JuridicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += " and  ((Etapa.Rol_id not in (" + Rol_Tecnico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Tecnico.ToString() + ") and  Solicitud.TecnicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += " AND EtapaSolicitud.UsuarioResponsable_id='" + objUs.intUsuario_id.ToString() + "' \n";
            Query += " AND EtapaSolicitud.Etapa_id <>26 \n";

            Query += "and  exists \n";
            Query += "( \n";
            Query += "	  Select * \n";
            Query += "	  From dbo.fc_Seg_Sel_RegionSubRegionUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	  Where Region_id = Solicitud.Region_id \n";
            Query += "	  and   SubRegion_id = Solicitud.SubRegion_id \n";
            Query += ") \n";
            Query += "and exists  \n";
            Query += "( \n";
            Query += "	Select * \n";
            Query += "	From dbo.fc_Seg_Sel_RolUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	Where Rol_id = Etapa.Rol_id \n";
            Query += ") \n";
            Query += "UNION ALL \n";

            Query += "Select EtapaSolicitud.* \n";
            Query += "From Tbl_Gest_EtapaSolicitud EtapaSolicitud,Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud \n";
            Query += "Where Solicitud.Solicitud_id = EtapaSolicitud.Solicitud_id \n";


            Query += "and  Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "and  Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "and (Etapa.Nombre_Etapa collate SQL_Latin1_General_Cp1_CI_AI = '" + Nombre_Etapa_Param + "' or '" + Nombre_Etapa_Param + "' = '') \n";
            Query += QueryComplementoBusqueda1;
            Query += QueryComplementoBusqueda2;
            //Query += "and  EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "and  (EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "or  EtapaSolicitud.EtapaSolicitudEstado_id = 3) \n";


            Query += " and  ((Etapa.Rol_id not in (" + Rol_Juridico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Juridico.ToString() + ") and  Solicitud.JuridicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += " and  ((Etapa.Rol_id not in (" + Rol_Tecnico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Tecnico.ToString() + ") and  Solicitud.TecnicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += " AND ISNULL(EtapaSolicitud.UsuarioResponsable_id,0)='0' \n";
            Query += " AND EtapaSolicitud.Etapa_id <>26 \n";

            Query += "and  exists \n";
            Query += "( \n";
            Query += "	  Select * \n";
            Query += "	  From dbo.fc_Seg_Sel_RegionSubRegionUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	  Where Region_id = Solicitud.Region_id \n";
            Query += "	  and   SubRegion_id = Solicitud.SubRegion_id \n";
            Query += ") \n";
            Query += "and exists  \n";
            Query += "( \n";
            Query += "	Select * \n";
            Query += "	From dbo.fc_Seg_Sel_RolUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	Where Rol_id = Etapa.Rol_id \n";
            Query += ") \n";
            Query += "order by EtapaSolicitud.swdatecreated desc \n";
            
            Query += "END \n";
            Query += "ELSE \n";
            Query += "BEGIN \n";
            Query += "Select EtapaSolicitud.* \n";
            Query += "From Tbl_Gest_EtapaSolicitud EtapaSolicitud,Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud \n";
            Query += "Where Solicitud.Solicitud_id = EtapaSolicitud.Solicitud_id \n";


            Query += "and  Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "and  Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "and (Etapa.Nombre_Etapa collate SQL_Latin1_General_Cp1_CI_AI = '" + Nombre_Etapa_Param + "' or '" + Nombre_Etapa_Param + "' = '') \n";
            Query += QueryComplementoBusqueda1;
            Query += QueryComplementoBusqueda2;
            //Query += "and  EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "and  (EtapaSolicitud.EtapaSolicitudEstado_id != 3 \n";
            //Query += "or  EtapaSolicitud.EtapaSolicitudEstado_id = 3) \n";


            Query += " and  ((Etapa.Rol_id not in (" + Rol_Juridico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Juridico.ToString() + ") and  Solicitud.JuridicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += " and  ((Etapa.Rol_id not in (" + Rol_Tecnico.ToString() + ")  )  or ( (Etapa.Rol_id in (" + Rol_Tecnico.ToString() + ") and  Solicitud.TecnicoAsignado_id = " + objUs.intUsuario_id.ToString() + " ))) \n";

            Query += "and  exists \n";
            Query += "( \n";
            Query += "	  Select * \n";
            Query += "	  From dbo.fc_Seg_Sel_RegionSubRegionUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	  Where Region_id = Solicitud.Region_id \n";
            Query += "	  and   SubRegion_id = Solicitud.SubRegion_id \n";
            Query += ") \n";
            Query += "and exists  \n";
            Query += "( \n";
            Query += "	Select * \n";
            Query += "	From dbo.fc_Seg_Sel_RolUsuario(" + objUs.intUsuario_id.ToString() + ",1) \n";
            Query += "	Where Rol_id = Etapa.Rol_id \n";
            Query += ") \n";
            Query += "order by EtapaSolicitud.swdatecreated desc \n";
            Query += "END \n";




            return db.Tbl_Gest_EtapaSolicitud.SqlQuery(Query).ToList();
        }

        // GET: Gest_EtapaSolicitud
        public ActionResult Index(DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "")
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");

            }

            if (((Pendientes != null) && (Finalizados != null)) || ((Pendientes == null) && (Finalizados == null)))
            {
                Pendientes = 1;
                Finalizados = 1;
                if (fecha_inicio == null)
                {
                    Finalizados = null;
                }
            }

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;


            if ((fecha_inicio != null) && (fecha_fin != null))
            {
                swdateinicio = (DateTime)fecha_inicio;
                swdatefinal = (DateTime)fecha_fin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                swdateinicio = swdatenow.AddDays(-30).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }

            List<Tbl_Gest_EtapaSolicitud> LstSolicitudes = VerificarSolicitudes(objUs, swdateinicio, swdatefinal, Pendientes, Finalizados, Nombre_Etapa_Param);

            var etapas = (from m in LstSolicitudes
                          group m.Tbl_Gest_Etapa.Nombre_Etapa by m.Tbl_Gest_Etapa.Nombre_Etapa into g
                          select new { Name = g.Key, KeyCols = g.Key }).ToList();
            var etapa = new { Name = "", KeyCols = "Todos" };
            etapas.Add(etapa);
            etapas = etapas.OrderBy(x => x.Name).ToList();

            //List<string> etapas = new List<string>();
            //if (LstSolicitudes.Count() > 0)
            //{
            //    foreach(var item in LstSolicitudes)
            //    {
            //        if (!etapas.Contains(item.Tbl_Gest_Etapa.Nombre_Etapa))
            //        {
            //            etapas.Add(item.Tbl_Gest_Etapa.Nombre_Etapa);
            //        }
            //    }
            //}

            ViewBag.swdateinicio = swdateinicio;
            ViewBag.swdatefinal = swdatefinal;
            ViewBag.Pendientes = Pendientes;
            ViewBag.Finalizados = Finalizados;
            ViewBag.Nombre_Etapa = Nombre_Etapa_Param;
            ViewBag.Nombre_Etapa_Param = new SelectList(etapas.ToList(), "Name", "KeyCols", Nombre_Etapa_Param);

            return View(LstSolicitudes);
        }

        public ActionResult IndexParams(DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "")
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");

            }

            if (((Pendientes != null) && (Finalizados != null)) || ((Pendientes == null) && (Finalizados == null)))
            {
                Pendientes = 1;
                Finalizados = 1;
            }

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;


            if ((fecha_inicio != null) && (fecha_fin != null))
            {
                swdateinicio = (DateTime)fecha_inicio;
                swdatefinal = (DateTime)fecha_fin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                swdateinicio = swdatenow.AddMonths(-1).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }

            List<Tbl_Gest_EtapaSolicitud> LstSolicitudes = VerificarSolicitudes(objUs, swdateinicio, swdatefinal, Pendientes, Finalizados, Nombre_Etapa_Param);

            var etapas = (from m in LstSolicitudes
                          group m.Tbl_Gest_Etapa.Nombre_Etapa by m.Tbl_Gest_Etapa.Nombre_Etapa into g
                          select new { Name = g.Key, KeyCols = g.Key }).ToList();
            var etapa = new { Name = "", KeyCols = "Todos" };
            etapas.Add(etapa);
            etapas = etapas.OrderBy(x => x.Name).ToList();

            ViewBag.swdateinicio = swdateinicio;
            ViewBag.swdatefinal = swdatefinal;
            ViewBag.Pendientes = Pendientes;
            ViewBag.Finalizados = Finalizados;

            ViewBag.Nombre_Etapa_Param = new SelectList(etapas.ToList(), "Name", "KeyCols", Nombre_Etapa_Param);

            return View();
        }

        public ActionResult IndexReturn(DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "", string Expediente = null)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            if (((Pendientes != null) && (Finalizados != null)) || ((Pendientes == null) && (Finalizados == null)))
            {
                Pendientes = 1;
                Finalizados = 1;
                if (fecha_inicio == null)
                {
                    Finalizados = null;
                }
            }

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;


            if ((fecha_inicio != null) && (fecha_fin != null))
            {
                swdateinicio = (DateTime)fecha_inicio;
                swdatefinal = (DateTime)fecha_fin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                swdateinicio = swdatenow.AddDays(-30).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }
            string exped;
            exped = Expediente;

            List<Tbl_Gest_EtapaSolicitud> LstSolicitudes = VerificarSolicitudesReturn(objUs, swdateinicio, swdatefinal, 1, 0, Nombre_Etapa_Param,exped);

            var etapas = (from m in LstSolicitudes
                          group m.Tbl_Gest_Etapa.Nombre_Etapa by m.Tbl_Gest_Etapa.Nombre_Etapa into g
                          select new { Name = g.Key, KeyCols = g.Key }).ToList();
            var etapa = new { Name = "", KeyCols = "Todos" };
            etapas.Add(etapa);
            etapas = etapas.OrderBy(x => x.Name).ToList();

            ViewBag.swdateinicio = swdateinicio;
            ViewBag.swdatefinal = swdatefinal;
            ViewBag.Pendientes = Pendientes;
            ViewBag.Finalizados = Finalizados;
            ViewBag.Nombre_Etapa = Nombre_Etapa_Param;
            ViewBag.Nombre_Etapa_Param = new SelectList(etapas.ToList(), "Name", "KeyCols", Nombre_Etapa_Param);
            ViewBag.Expediente = exped;

            return View(LstSolicitudes);
        }

        public ActionResult IndexReturnParams(DateTime? fecha_inicio, DateTime? fecha_fin, int? Pendientes, int? Finalizados, string Nombre_Etapa_Param = "",string Expediente=null)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");

            }

            if (((Pendientes != null) && (Finalizados != null)) || ((Pendientes == null) && (Finalizados == null)))
            {
                Pendientes = 1;
                Finalizados = 1;
            }

            DateTime swdatenow = DateTime.Now;
            DateTime swdateinicio = swdatenow;
            DateTime swdatefinal = swdatenow;


            if ((fecha_inicio != null) && (fecha_fin != null))
            {
                swdateinicio = (DateTime)fecha_inicio;
                swdatefinal = (DateTime)fecha_fin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                swdateinicio = swdatenow.AddMonths(-1).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }
            string exped;
            exped = Expediente;

            List<Tbl_Gest_EtapaSolicitud> LstSolicitudes = VerificarSolicitudesReturn(objUs, swdateinicio, swdatefinal, Pendientes, Finalizados, Nombre_Etapa_Param, exped);

            var etapas = (from m in LstSolicitudes
                          group m.Tbl_Gest_Etapa.Nombre_Etapa by m.Tbl_Gest_Etapa.Nombre_Etapa into g
                          select new { Name = g.Key, KeyCols = g.Key }).ToList();
            var etapa = new { Name = "", KeyCols = "Todos" };
            etapas.Add(etapa);
            etapas = etapas.OrderBy(x => x.Name).ToList();

            ViewBag.swdateinicio = swdateinicio;
            ViewBag.swdatefinal = swdatefinal;
            ViewBag.Pendientes = Pendientes;
            ViewBag.Finalizados = Finalizados;

            ViewBag.Nombre_Etapa_Param = new SelectList(etapas.ToList(), "Name", "KeyCols", Nombre_Etapa_Param);

            return View();
        }

        public ActionResult Historial(string Guid_id, string Layout)
        {

            if (Layout == null)
            { 
                 Layout = "Si"; 
            }


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            DateTime swdatenow, swdateinicio, swdatefinal;
            swdatenow = DateTime.Now;

            swdateinicio = swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
            swdatefinal = swdatenow;

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();
            ViewBag.FechaInicio = swdateinicio;
            ViewBag.FechaFin = swdatefinal;


            ViewBag.Layout = Layout;

            return View(oSolicitud);
        }

        public ActionResult IndexHistorial(DateTime? FechaInicio, DateTime? FechaFin, string Expediente )
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;

            DateTime swdatenow, swdateinicio, swdatefinal;
            string exped;

            swdatenow = DateTime.Now;
           
            string Query, QueryComplementoBusqueda;
            QueryComplementoBusqueda = "";

            QueryComplementoBusqueda = " and exists ( select  * from Tbl_Seg_UsuarioSubRegion Where Usuario_id = " + objUs.intUsuario_id.ToString() + " and Estado_id = 1 and Region_id = Solicitud.Region_id and SubRegion_id = Solicitud.SubRegion_id  ) ";


            if ((FechaInicio != null) && (FechaFin == null))
            {
                swdateinicio = (DateTime)FechaInicio;
                QueryComplementoBusqueda += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and getdate() ";
            }
            else if ((FechaInicio != null) && (FechaFin != null))
            {
                swdateinicio = (DateTime)FechaInicio;
                swdatefinal = (DateTime)FechaFin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103) ";
            }
            else
            {
                QueryComplementoBusqueda += $" and EtapaSolicitud.swdatecreated between Convert(datetime, '{swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second)}', 103) and Convert(datetime, '{swdatenow}', 103) ";
            }

            Query = " Select Solicitud.Solicitud_NumeroExpediente Solicitud_NumeroExpediente, \n";
            Query += "	    Solicitud.Solicitud_NumeroTemporal   Solicitud_NumeroTemporal, \n";
            Query += "		Solicitud.swdateupdated              swdateupdated, \n";
            Query += "      EtapaSolicitud.swdatecreated         swdatecreated, \n";
            Query += "		Etapa.Nombre_Etapa                   Nombre_Etapa, \n";
            Query += "		dbo.Fnc_Gral_CategoriaNombre(Solicitud.Categoria_id) Categoria, \n";
            Query += "		dbo.Fnc_Gral_SubCategoriaNombre(Solicitud.Categoria_id, Solicitud.Sub_Categoria_id) SubCategoria, \n";
            Query += "		dbo.fc_Seg_Sel_UsuarioResponsable(EtapaSolicitud.Solicitud_id, EtapaSolicitud.Etapa_id, EtapaSolicitud.EtapaRuta_id, EtapaSolicitud.CorrelativoEtapa_id)  UsuarioAsignado, \n";
            Query += "		dbo.Fnc_Seg_UsuarioExternoNombre(Solicitud.swcreatedby) Nombres , \n";
            Query += "		(select SolEstado.Descripcion from Tbl_Gest_EtapaSolicitudEstado SolEstado where SolEstado.EtapaSolicitudEstado_id = EtapaSolicitud.EtapaSolicitudEstado_id) EtapaSolicitudEstado, \n";
            Query += "		isnull((Select Tbl_Gest_Etapa_Respuesta.Descripcion From Tbl_Gest_Etapa_Respuesta Where Tbl_Gest_Etapa_Respuesta.Etapa_id = EtapaSolicitud.Etapa_id and Tbl_Gest_Etapa_Respuesta.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id and Tbl_Gest_Etapa_Respuesta.Etapa_Respuesta_id = EtapaSolicitud.Respuesta_id and Tbl_Gest_Etapa_Respuesta.Estado = 1 ),'En proceso') Respuesta,  \n";
            Query += "	    isnull((   \n";
            Query += "	     Select '<h5>  Observaciones jurídicas : </h5>' + Observaciones  \n";
            Query += "	     From Tbl_Sol_Solicitud_OficioDictamenJuridico  \n";
            Query += "	    	Where  Tbl_Sol_Solicitud_OficioDictamenJuridico.Solicitud_id = EtapaSolicitud.Solicitud_id  \n";
            Query += "	 	    and Tbl_Sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id  \n";
            Query += "		     and  Tbl_Sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id = EtapaSolicitud.CorrelativoEtapa_id  \n";
            Query += "		     and Tbl_Sol_Solicitud_OficioDictamenJuridico.Etapa_id = EtapaSolicitud.Etapa_id  \n";
            Query += "	     ),'')  \n";
            Query += "		    RespuestaJuridica,  \n";
            Query += "		 EtapaSolicitud.Solicitud_id,  \n";
            Query += "		 EtapaSolicitud.EtapaSolicitud_GUID_id, \n";
            Query += "		 EtapaSolicitud.Solicitud_Guid_id,  \n";
            Query += "		 EtapaSolicitud.Etapa_id,  \n";
            Query += "		 EtapaSolicitud.EtapaRuta_id,  \n";
            Query += "		 EtapaSolicitud.CorrelativoEtapa_id, \n";
            Query += "		 EtapaSolicitud.EtapaSolicitudEstado_id,  \n";
            Query += "		 EtapaSolicitud.NombreDocumentoFirmado, \n";
            Query += "		 EtapaSolicitud.NombreDocumentoNoFirmado, \n";
            Query += "		 (Select   \n";
            Query += "			 case when isnull(EtapaSolicitud.NombreDocumentoFirmado,'') !=''  \n";
            Query += "			 then  '/Archivos_ConFirmaElectronica/'+ EtapaSolicitud.NombreDocumentoFirmado  \n";
            Query += "			 else  \n";
            Query += "			 case when isnull(EtapaSolicitud.NombreDocumentoFirmado,'') != ''  \n";
            Query += "			 then   \n";
            Query += "			 '/Archivos_Generados_Que_Pueden_Borrar/'+ EtapaSolicitud.NombreDocumentoNoFirmado  \n";
            Query += "			 else  \n";
            Query += "			 '#'  \n";
            Query += "			 end  \n";
            Query += "		 end ) Documentos, \n";
            Query += "		 (Select   \n";
            Query += "			 case when ((LTRIM(RTRIM(isnull(EtapaSolicitud.NombreDocumentoFirmado,''))) !='') AND (LTRIM(RTRIM(ISNULL(EtapaSolicitud.NombreDocumentoNoFirmado,''))) != ''))  \n";
            Query += "			 then  1 \n";
            Query += "			 else 0 \n";
            Query += "		 end ) Firmado, \n";
            Query += "			(Select count(*) \n";
            Query += "			from Tbl_Gest_EtapaSolicitud_Documento EtapaDoc \n";
            Query += "			Where EtapaDoc.Solicitud_id = EtapaSolicitud.Solicitud_id \n";
            Query += "			and EtapaDoc.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "			and EtapaDoc.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "			and EtapaDoc.CorrelativoEtapa_id = EtapaSolicitud.CorrelativoEtapa_id) DocumentosEtapa \n";
            Query += " From Tbl_Gest_EtapaSolicitud EtapaSolicitud, Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud   \n";
            Query += " Where Solicitud.Guid_id = '" + Expediente + "' \n";
            Query += " and   EtapaSolicitud.Solicitud_id = Solicitud.Solicitud_id \n";
            Query += " and   Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += " and   Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += QueryComplementoBusqueda;
            Query += " order by EtapaSolicitud.swdatecreated --desc \n";

            List<Gest_EtapaSolicitudHistorial> LstSolicitudes = new List<Gest_EtapaSolicitudHistorial>();
            LstSolicitudes = db.Database.SqlQuery<Gest_EtapaSolicitudHistorial>(Query).ToList();

            return View(LstSolicitudes);
        }

        public ActionResult HistorialSolicitud(string Guid_id, string Layout)
        {

            if (Layout == null)
            {
                Layout = "Si";
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            DateTime swdatenow, swdateinicio, swdatefinal;
            swdatenow = DateTime.Now;

            swdateinicio = swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
            swdatefinal = swdatenow;

            ViewBag.Guid_id = Guid_id;

            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == Guid_id
                                            select d).FirstOrDefault();
            ViewBag.FechaInicio = swdateinicio;
            ViewBag.FechaFin = swdatefinal;


            ViewBag.Layout = Layout;

            return View(oSolicitud);
        }

        public ActionResult HistorialSolicitudDetalle(string Expediente)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;

            string exped;


            string Query, QueryComplementoBusqueda;
            QueryComplementoBusqueda = "";


            QueryComplementoBusqueda = " and exists ( select  * from Tbl_Seg_UsuarioSubRegion Where Usuario_id = " + objUs.intUsuario_id.ToString() + " and Estado_id = 1 and Region_id = Solicitud.Region_id and SubRegion_id = Solicitud.SubRegion_id  ) ";


            Query = " Select Solicitud.Solicitud_NumeroExpediente Solicitud_NumeroExpediente, \n";
            Query += "	    Solicitud.Solicitud_NumeroTemporal   Solicitud_NumeroTemporal, \n";
            Query += "		Solicitud.swdateupdated              swdateupdated, \n";
            Query += "      EtapaSolicitud.swdatecreated         swdatecreated, \n";
            Query += "		Etapa.Nombre_Etapa                   Nombre_Etapa, \n";
            Query += "		dbo.Fnc_Gral_CategoriaNombre(Solicitud.Categoria_id) Categoria, \n";
            Query += "		dbo.Fnc_Gral_SubCategoriaNombre(Solicitud.Categoria_id, Solicitud.Sub_Categoria_id) SubCategoria, \n";
            Query += "		dbo.fc_Seg_Sel_UsuarioResponsable(EtapaSolicitud.Solicitud_id, EtapaSolicitud.Etapa_id, EtapaSolicitud.EtapaRuta_id, EtapaSolicitud.CorrelativoEtapa_id) UsuarioAsignado, \n";
            Query += "		dbo.Fnc_Seg_UsuarioExternoNombre(Solicitud.swcreatedby) Nombres , \n";
            Query += "		(select SolEstado.Descripcion from Tbl_Gest_EtapaSolicitudEstado SolEstado where SolEstado.EtapaSolicitudEstado_id = EtapaSolicitud.EtapaSolicitudEstado_id) EtapaSolicitudEstado, \n";
            Query += "		isnull((Select Tbl_Gest_Etapa_Respuesta.Descripcion From Tbl_Gest_Etapa_Respuesta Where Tbl_Gest_Etapa_Respuesta.Etapa_id = EtapaSolicitud.Etapa_id and Tbl_Gest_Etapa_Respuesta.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id and Tbl_Gest_Etapa_Respuesta.Etapa_Respuesta_id = EtapaSolicitud.Respuesta_id and Tbl_Gest_Etapa_Respuesta.Estado = 1 ),'En proceso') Respuesta,  \n";
            Query += "	    isnull((   \n";
            Query += "	     Select '<h5>  Observaciones jurídicas : </h5>' + Observaciones  \n";
            Query += "	     From Tbl_Sol_Solicitud_OficioDictamenJuridico  \n";
            Query += "	    	Where  Tbl_Sol_Solicitud_OficioDictamenJuridico.Solicitud_id = EtapaSolicitud.Solicitud_id  \n";
            Query += "	 	    and Tbl_Sol_Solicitud_OficioDictamenJuridico.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id  \n";
            Query += "		     and  Tbl_Sol_Solicitud_OficioDictamenJuridico.CorrelativoEtapa_id = EtapaSolicitud.CorrelativoEtapa_id  \n";
            Query += "		     and Tbl_Sol_Solicitud_OficioDictamenJuridico.Etapa_id = EtapaSolicitud.Etapa_id  \n";
            Query += "	     ),'')  \n";
            Query += "		    RespuestaJuridica,  \n";
            Query += "		 EtapaSolicitud.Solicitud_id,  \n";
            Query += "		 EtapaSolicitud.EtapaSolicitud_GUID_id, \n";
            Query += "		 EtapaSolicitud.Solicitud_Guid_id,  \n";
            Query += "		 EtapaSolicitud.Etapa_id,  \n";
            Query += "		 EtapaSolicitud.EtapaRuta_id,  \n";
            Query += "		 EtapaSolicitud.CorrelativoEtapa_id, \n";
            Query += "		 EtapaSolicitud.EtapaSolicitudEstado_id,  \n";
            Query += "		 EtapaSolicitud.NombreDocumentoFirmado, \n";
            Query += "		 EtapaSolicitud.NombreDocumentoNoFirmado, \n";
            Query += "		 (Select   \n";
            Query += "			 case when isnull(EtapaSolicitud.NombreDocumentoFirmado,'') !=''  \n";
            Query += "			 then  '/Archivos_ConFirmaElectronica/'+ EtapaSolicitud.NombreDocumentoFirmado  \n";
            Query += "			 else  \n";
            Query += "			 case when isnull(EtapaSolicitud.NombreDocumentoFirmado,'') != ''  \n";
            Query += "			 then   \n";
            Query += "			 '/Archivos_Generados_Que_Pueden_Borrar/'+ EtapaSolicitud.NombreDocumentoNoFirmado  \n";
            Query += "			 else  \n";
            Query += "			 '#'  \n";
            Query += "			 end  \n";
            Query += "		 end ) Documentos, \n";
            Query += "			(Select count(*) \n";
            Query += "			from Tbl_Gest_EtapaSolicitud_Documento EtapaDoc \n";
            Query += "			Where EtapaDoc.Solicitud_id = EtapaSolicitud.Solicitud_id \n";
            Query += "			and EtapaDoc.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += "			and EtapaDoc.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += "			and EtapaDoc.CorrelativoEtapa_id = EtapaSolicitud.CorrelativoEtapa_id) DocumentosEtapa \n";
            Query += " From Tbl_Gest_EtapaSolicitud EtapaSolicitud, Tbl_Gest_Etapa Etapa, Tbl_Sol_Solicitud Solicitud   \n";
            Query += " Where Solicitud.Guid_id = '" + Expediente + "' \n";
            Query += " and   EtapaSolicitud.Solicitud_id = Solicitud.Solicitud_id \n";
            Query += " and   Etapa.EtapaRuta_id = EtapaSolicitud.EtapaRuta_id \n";
            Query += " and   Etapa.Etapa_id = EtapaSolicitud.Etapa_id \n";
            Query += QueryComplementoBusqueda;
            Query += " order by EtapaSolicitud.swdatecreated \n";

            List<Gest_EtapaSolicitudHistorial> LstSolicitudes = new List<Gest_EtapaSolicitudHistorial>();
            LstSolicitudes = db.Database.SqlQuery<Gest_EtapaSolicitudHistorial>(Query).ToList();

            return View(LstSolicitudes);
        }

        public ActionResult BuscarSolicitud(string sortOrder, int? page, DateTime? FechaInicio, DateTime? FechaFin, string Expediente = null, string SolicitanteNombre = null, string SolicitanteApellido = null)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            DateTime swdatenow, swdateinicio, swdatefinal;
            string exped;

            swdatenow = DateTime.Now;
            exped = Expediente;

            swdateinicio = swdatefinal = swdatenow;

            string Query, QueryComplementoBusqueda;
            QueryComplementoBusqueda = "";

            if ((FechaInicio != null) && (FechaFin == null))
            {
                swdateinicio = (DateTime)FechaInicio;
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and getdate()) ";
            }
            else if ((FechaInicio != null) && (FechaFin != null))
            {
                swdateinicio = (DateTime)FechaInicio;
                swdatefinal = (DateTime)FechaFin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103)) ";
            }
            else if ((FechaInicio == null) && (FechaFin == null))
            {
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second)}', 103) and Convert(datetime, '{swdatenow}', 103)) ";
                swdateinicio = swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }
            else
            {
                swdateinicio = (DateTime)FechaInicio;
                swdatefinal = (DateTime)FechaFin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103)) ";
            }

            ViewBag.Expediente = Expediente;
            ViewBag.SolicitanteNombre = SolicitanteNombre;
            ViewBag.SolicitanteApellido = SolicitanteApellido;
            ViewBag.FechaInicio = swdateinicio;
            ViewBag.FechaFin = swdatefinal;

            Query = $"Select \n";
            Query += $"    Solicitud.* \n";
            Query += $"From \n";
            Query += $"    Tbl_Sol_Solicitud Solicitud \n";
            Query += $"Where \n";
            Query += $"    ((isnull('{Expediente}','')='' or (Solicitud.Solicitud_NumeroExpediente like '%{Expediente}%')) \n";
            Query += $"    or (isnull('{Expediente}','')='' or (Solicitud.Solicitud_NumeroTemporal like '%{Expediente}%'))) \n";
            Query += $"    and Solicitud.swcreatedby in \n";
            Query += $"    	(select  Usuario_id from Tbl_Seg_UsuarioExterno Ext \n";
            Query += $"    	where (isnull('{SolicitanteNombre}','')= '' or (Ext.Nombres like '%{SolicitanteNombre}%')) \n";
            Query += $"    	and (isnull('{SolicitanteApellido}','')= '' or (Ext.Apellidos like '%{SolicitanteApellido}%'))) \n";

            Query += QueryComplementoBusqueda;
            Query += $"    	Order by Solicitud.swdatecreated desc, Solicitud.Solicitud_id desc \n";

            List<Tbl_Sol_Solicitud> LstSolicitudes = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();


            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NTE = sortOrder == "Numero" ? "numero_desc" : "Numero";
            ViewBag.Estado = sortOrder == "Estado" ? "estado_desc" : "Estado";
            ViewBag.FechaSolicitud = sortOrder == "FechaSolicitud" ? "fechasol_desc" : "FechaSolicitud";
            ViewBag.FechaEtapa = sortOrder == "FechaEtapa" ? "fechaetapa_desc" : "FechaEtapa";
            ViewBag.Tipo = sortOrder == "Tipo" ? "tipo_desc" : "Tipo";
            ViewBag.Solicitante = sortOrder == "Solicitante" ? "solicitante_desc" : "Solicitante";

            var ListarSolicitudes = (from d in LstSolicitudes select d);

            switch (sortOrder)
            {
                case "Estado":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Estado_id);
                    break;
                case "estado_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Estado_id);
                    break;
                case "FechaSolicitud":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.swdatecreated);
                    break;
                case "fechasol_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.swdatecreated);
                    break;
                case "FechaEtapa":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.swdateupdated);
                    break;
                case "fechaetapa_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.swdateupdated);
                    break;
                case "Tipo":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Tbl_Sol_Solicitud_Categoria.Descripcion).ThenBy(Obj => Obj.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion);
                    break;
                case "tipo_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Tbl_Sol_Solicitud_Categoria.Descripcion).ThenByDescending(Obj => Obj.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion);
                    break;
                case "Solicitante":
                    ListarSolicitudes = ListarSolicitudes.OrderBy(Obj => Obj.Tbl_Seg_UsuarioExterno.Nombres);
                    break;
                case "solicitante_desc":
                    ListarSolicitudes = ListarSolicitudes.OrderByDescending(Obj => Obj.Tbl_Seg_UsuarioExterno.Nombres);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);


            return View(ListarSolicitudes.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult BuscarSolicitudParametros(DateTime? FechaInicio, DateTime? FechaFin, string Expediente = null, string SolicitanteNombre = null, string SolicitanteApellido = null)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            DateTime swdatenow, swdateinicio, swdatefinal;
            string exped;

            swdatenow = DateTime.Now;
            exped = Expediente;

            swdateinicio = swdatefinal = swdatenow;

            string Query, QueryComplementoBusqueda;
            QueryComplementoBusqueda = "";

            QueryComplementoBusqueda = " and exists ( select  * from Tbl_Seg_UsuarioSubRegion Where Usuario_id = " + objUs.intUsuario_id.ToString() + " and Estado_id = 1 and Region_id = Solicitud.Region_id and SubRegion_id = Solicitud.SubRegion_id  ) ";


            if ((FechaInicio != null) && (FechaFin == null))
            {
                swdateinicio = (DateTime)FechaInicio;
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and getdate()) ";
            }
            else if ((FechaInicio != null) && (FechaFin != null))
            {
                swdateinicio = (DateTime)FechaInicio;
                swdatefinal = (DateTime)FechaFin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103)) ";
            }
            else if ((FechaInicio == null) && (FechaFin == null))
            {
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second)}', 103) and Convert(datetime, '{swdatenow}', 103)) ";
                swdateinicio = swdatenow.AddMonths(-6).AddHours(-swdatenow.Hour).AddMinutes(-swdatenow.Minute).AddSeconds(-swdatenow.Second);
                swdatefinal = swdatenow;
            }
            else
            {
                swdateinicio = (DateTime)FechaInicio;
                swdatefinal = (DateTime)FechaFin;
                swdatefinal = swdatefinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                QueryComplementoBusqueda += $" and (Solicitud.swdatecreated between Convert(datetime, '{swdateinicio}', 103) and Convert(datetime, '{swdatefinal}', 103)) ";
            }

            Query = $"Select \n";
            Query += $"    Solicitud.* \n";
            Query += $"From \n";
            Query += $"    Tbl_Sol_Solicitud Solicitud \n";
            Query += $"Where \n";
            Query += $"    ((isnull('{Expediente}','')='' or (Solicitud.Solicitud_NumeroExpediente like '%{Expediente}%')) \n";
            Query += $"    or (isnull('{Expediente}','')='' or (Solicitud.Solicitud_NumeroTemporal like '%{Expediente}%'))) \n";
            Query += $"    and Solicitud.swcreatedby in \n";
            Query += $"    	(select  Usuario_id from Tbl_Seg_UsuarioExterno Ext \n";
            Query += $"    	where (isnull('{SolicitanteNombre}','')= '' or (Ext.Nombres like '%{SolicitanteNombre}%')) \n";
            Query += $"    	and (isnull('{SolicitanteApellido}','')= '' or (Ext.Apellidos like '%{SolicitanteApellido}%'))) \n";

            Query += QueryComplementoBusqueda;

            List<Tbl_Sol_Solicitud> LstSolicitudes = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();

            return View(LstSolicitudes);
        }





        public ActionResult Solicitud_Reasignacion(long Solicitud_id)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");

            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);
            if(tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);

            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);

            ViewBag.DepartamentoSolicitud_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.DepartamentoSolicitud_id);
            ViewBag.MunicipioSolicitud_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.DepartamentoSolicitud_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.MunicipioSolicitud_id);

            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            ViewBag.JuridicoAsignado_id = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Juridico, -5, tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_Sol_Solicitud.JuridicoAsignado_id);


            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;
            ViewBag.TecnicoAsignado_id = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Tecnico, -5, tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_Sol_Solicitud.TecnicoAsignado_id);

            return View(tbl_Sol_Solicitud);
        }



        [HttpPost]
        public ActionResult Solicitud_Reasignacion(Tbl_Sol_Solicitud model)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno == 0)
            {
                return RedirectToAction("../Home/AccesoDenegado");

            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);
            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if ((model.Categoria_id??0) !=0)
            {
                tbl_Sol_Solicitud.Categoria_id = model.Categoria_id;
            }
            if((model.Sub_Categoria_id??0) != 0)
            {
                tbl_Sol_Solicitud.Sub_Categoria_id = model.Sub_Categoria_id;
            }
            if ((model.Sub_Sub_Categoria_id ?? 0) != 0)
            {
                tbl_Sol_Solicitud.Sub_Sub_Categoria_id = model.Sub_Sub_Categoria_id;
            }

            if ((model.Region_id ?? 0) != 0)
            {
                tbl_Sol_Solicitud.Region_id = model.Region_id;
            }
            if ((model.SubRegion_id ?? 0) != 0)
            {
                tbl_Sol_Solicitud.SubRegion_id= model.SubRegion_id;
            }

            if ((model.Notificacion_Departamento_id != 0) && (model.Notificacion_Municipio_id != 0))
            {
                tbl_Sol_Solicitud.Notificacion_Departamento_id = model.Notificacion_Departamento_id;
                tbl_Sol_Solicitud.Notificacion_Municipio_id= model.Notificacion_Municipio_id;
            }

            if ((model.Notificacion_Direccion??"") != "")
            {
                tbl_Sol_Solicitud.Notificacion_Direccion = model.Notificacion_Direccion;
            }
            if (((model.DepartamentoSolicitud_id ?? 0) != 0) && ((model.MunicipioSolicitud_id ?? 0) != 0))
            {
                tbl_Sol_Solicitud.DepartamentoSolicitud_id= model.DepartamentoSolicitud_id;
                tbl_Sol_Solicitud.MunicipioSolicitud_id = model.MunicipioSolicitud_id;
            }
            tbl_Sol_Solicitud.DPI_Titular = (model.DPI_Titular??"");


            if((model.TecnicoAsignado_id??0) != 0)
            {
                tbl_Sol_Solicitud.TecnicoAsignado_id = model.TecnicoAsignado_id;

            }

            if ((model.JuridicoAsignado_id ?? 0) != 0)
            {
                tbl_Sol_Solicitud.JuridicoAsignado_id = model.JuridicoAsignado_id;
            }



            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);

            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);

            ViewBag.DepartamentoSolicitud_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.DepartamentoSolicitud_id);
            ViewBag.MunicipioSolicitud_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.DepartamentoSolicitud_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.MunicipioSolicitud_id);


            int Rol_Juridico = db.Tbl_Gral_PerfilesRol.First().JuridicoRegional ?? 0;
            ViewBag.JuridicoAsignado_id = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Juridico, -5, tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_Sol_Solicitud.JuridicoAsignado_id);


            int Rol_Tecnico = db.Tbl_Gral_PerfilesRol.First().TecnicoForestal ?? 0;
            ViewBag.TecnicoAsignado_id = new SelectList(db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_Tecnico, -5, tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id), "Usuario_id", "NombreCompleto", tbl_Sol_Solicitud.TecnicoAsignado_id);

            bool esinterno = false;
            if(objUs.EsInterno == 1)
            {
                esinterno = true;
            }

            try
            {

                if (ModelState.IsValid)
                {
                    tbl_Sol_Solicitud.swupdatedby = objUs.intUsuario_id;
                    tbl_Sol_Solicitud.swupdatedbyinterno = esinterno;
                    tbl_Sol_Solicitud.swdateupdated = DateTime.Now;
                    db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("RegistroActualizado","Home");
                }

            }
            catch
            {

            }
            return View(tbl_Sol_Solicitud);
        }

        public ActionResult email(string EtapaSolicitud_Guid_id, string tipoDeCorreo)
        {
            string sqlQuery = $"SELECT * FROM fc_Mail_History('{EtapaSolicitud_Guid_id}','{tipoDeCorreo}')";

            //Tbl_Mail_History tbl_Mail_history = db.Tbl_Mail_History.Where(Obj=> Obj.EtapaSolicitud_GUID_id == EtapaSolicitud_Guid_id && Obj.TipoDeCorreo == tipoDeCorreo).LastOrDefault();
            Tbl_Mail_History tbl_Mail_history = db.Tbl_Mail_History.SqlQuery(sqlQuery).FirstOrDefault();

            return View(tbl_Mail_history);        
        }



        //DocumentosRecibidos

        public ActionResult EstatusActual(string Guid)
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
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid).First();

            List<Tbl_Gest_EtapaSolicitud> LstGest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).ToList();

            return View(LstGest_EtapaSolicitud);

        }

            // GET: Gest_EtapaSolicitud
        public ActionResult Edit(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            ViewBag.RespuestaExplicita = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id).First().Respuesta_Explicita;


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            var tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj=> Obj.Solicitud_id == solicitud_id
            && Obj.Etapa_id == etapa_id &&  Obj.EtapaRuta_id == etaparuta_id &&  Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.EtapaSolicitudEstado_id < 3)     /* Estado 3 significa finalizada  */
            {

                if (tbl_gest_etapasolicitud.swViewedby == null)
                {
                        tbl_gest_etapasolicitud.swViewedby = objUs.intUsuario_id;
                        tbl_gest_etapasolicitud.swdateViewed = DateTime.Now;
                        if (objUs.EsInterno != 1)
                        {
                            tbl_gest_etapasolicitud.swViewedbyinterno = false;

                        }
                        else
                        {
                            tbl_gest_etapasolicitud.swViewedbyinterno = true;
                        }
                }

                tbl_gest_etapasolicitud.swLastViewedby = objUs.intUsuario_id;
                tbl_gest_etapasolicitud.swdateLastViewed = DateTime.Now;
                if (objUs.EsInterno != 1)
                {
                    tbl_gest_etapasolicitud.swLastViewedbyinterno = false;

                }
                else
                {
                    tbl_gest_etapasolicitud.swLastViewedbyinterno = true;
                }

                db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                db.SaveChanges();

                //ViewBag.Respuesta_id = new SelectList(db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == tbl_gest_etapasolicitud.Etapa_id && Obj.EtapaRuta_id == etaparuta_id), "Etapa_Respuesta_id", "Descripcion");

                ViewBag.intCantidadRespuestas = db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == tbl_gest_etapasolicitud.Etapa_id && Obj.EtapaRuta_id == etaparuta_id).Count();

                ViewBag.CantidadRepeticionesEtapa = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id &&  Obj.Etapa_id == tbl_gest_etapasolicitud.Etapa_id && Obj.EtapaRuta_id == etaparuta_id).Count();


                ViewBag.Respuestas = db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == tbl_gest_etapasolicitud.Etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.ButtonColor !=null && Obj.ButtonColor !="" && Obj.Estado == true );


                ViewBag.RespuestaDescripcion = "";
            }
            else
            {
                ViewBag.Respuestas = null;
                ViewBag.RespuestaDescripcion = "Respuesta enviada : " + new SelectList(db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == tbl_gest_etapasolicitud.Etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.Etapa_Respuesta_id == tbl_gest_etapasolicitud.Respuesta_id), "Etapa_Respuesta_id", "Descripcion").First().Value;
            }

            return View(tbl_gest_etapasolicitud);
        }
                          
        public JsonResult ActualizaTodoListEstatus(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, int to_do_id, string opcion, int valor)
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
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();


            if (Tbl_Gest_etapasolicitud.Respuesta_id == 0)
            {

                Tbl_Gest_EtapaSolicitud_To_doList tbl_gest_EtapaSolicitud_To_doList = db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id && Obj.To_do_id == to_do_id).First();

                tbl_gest_EtapaSolicitud_To_doList.Si = false;
                tbl_gest_EtapaSolicitud_To_doList.No = false;
                tbl_gest_EtapaSolicitud_To_doList.No_Aplica = false;

                if (opcion == "No")
                    tbl_gest_EtapaSolicitud_To_doList.No = true;

                if (opcion == "No Aplica")
                    tbl_gest_EtapaSolicitud_To_doList.No_Aplica = true;

                if (opcion == "Si")
                    tbl_gest_EtapaSolicitud_To_doList.Si = true;


                tbl_gest_EtapaSolicitud_To_doList.swupdatedby = objUs.intUsuario_id;
                tbl_gest_EtapaSolicitud_To_doList.swdateupdated = DateTime.Now;
                tbl_gest_EtapaSolicitud_To_doList.swupdatedbyinterno = true;

                db.Entry(tbl_gest_EtapaSolicitud_To_doList).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json("");
        }

        public JsonResult ActualizaTodoListObservacion(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, int to_do_id, string observacion)
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
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();


            if (Tbl_Gest_etapasolicitud.Respuesta_id == 0)
            { 

                Tbl_Gest_EtapaSolicitud_To_doList tbl_gest_EtapaSolicitud_To_doList = db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id && Obj.To_do_id == to_do_id).First();

                tbl_gest_EtapaSolicitud_To_doList.swupdatedby = objUs.intUsuario_id;
                tbl_gest_EtapaSolicitud_To_doList.swdateupdated = DateTime.Now;
                tbl_gest_EtapaSolicitud_To_doList.swupdatedbyinterno = true;
                tbl_gest_EtapaSolicitud_To_doList.Observaciones = observacion;


                db.Entry(tbl_gest_EtapaSolicitud_To_doList).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json("");
        }


        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud
    (
        long solicitud_id,
        int etapa_id,
        decimal etaparuta_id,
        int correlativoetapa_id,
        string motivo,
        int respuestaid,
        string EtapaSolicitud_GUIDid
    )
        {

            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "El usuario no se encuentra logueado. Ingrese de nuevo al sistema.";


                string jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResultUsr);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Gest_EtapaModel gest_EtapaModel = new Gest_EtapaModel();
            ResultFromStoreProcedure Respuesta = gest_EtapaModel.ConfirmarRespuesta(objUs, solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid);
            //ResultFromStoreProcedure resultFromStoreProcedure = ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid);
            if (Respuesta.respuesta == 1)
            {

                codRespuesta = 1;
                strRespuesta = "Actualización realizada.";


                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


            }

            codRespuesta = 0;

            if (Respuesta.mensaje.ToLower().Contains("Tiene actividades en procesos".ToLower()))
            {
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
            }
            else
            {
                strRespuesta = Respuesta.mensaje;
            }

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);

        }



        //-------------------------- Etapa revision  // Respuesta Expediente incompleto    ------------------------------------

        //if ((tbl_gest_etapasolicitudCh.Etapa_id == 1) && (tbl_gest_etapasolicitudCh.Respuesta_id == 2))
        //{
        //    string strSubject = "";
        //    int CantidadEtapasIniciales = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_gest_etapasolicitudCh.Solicitud_id && Obj.Etapa_id == 1).Count();

        //    if (CantidadEtapasIniciales == 1)
        //    {
        //        strSubject = "AVISO ELECTRÓNICO. PRIMERA REVISIÓN DE SOLICITUD DE INSCRIPCIÓN ";
        //    }

        //    if (CantidadEtapasIniciales == 2)
        //    {
        //        strSubject = "AVISO ELECTRÓNICO. SEGUNDA REVISIÓN DE SOLICITUD DE INSCRIPCIÓN ";
        //    }

        //    if (CantidadEtapasIniciales == 3)
        //    {
        //        strSubject = "AVISO ELECTRÓNICO. TERCERA REVISIÓN DE SOLICITUD DE INSCRIPCIÓN ";
        //    }

        //    Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_gest_etapasolicitud.Solicitud_id);

        //    string Mensaje = db.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_NotificacionElectronicaCorreccionesRequeridas(" + tbl_sol_solicitud.Solicitud_id.ToString() + ")").FirstOrDefault();

        //    Tbl_Seg_UsuarioExterno tbl_seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_solicitud.swcreatedby);

        //    EnvioCorreo("Denegación",tbl_seg_UsuarioExterno.Correo, strSubject, Mensaje, true, tbl_gest_etapasolicitud.EtapaSolicitud_GUID_id);

        //}


        //-------------------------- Enviar correo    ------------------------------------





        public ResultFromStoreProcedure ConfirmarRespuesta(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid)
        {
            Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.Respuesta_id != 0)
            {
                return new ResultFromStoreProcedure
                {
                    respuesta = 0,
                    mensaje = "No se encontró la etapa"
                };
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return new ResultFromStoreProcedure
                {
                    respuesta = 0,
                    mensaje = "Su sesión ha expirado, inicie sesión nuevamente por favor"
                };
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapasolicitud.Motivo = strMotivo;

            if (ModelState.IsValid)
            {

                tbl_gest_etapasolicitud.swupdatedby = objUs.intUsuario_id;
                tbl_gest_etapasolicitud.swdateupdated = DateTime.Now;

                if (objUs.EsInterno != 1)
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_gest_etapasolicitud.swupdatedbyinterno = true;

                }

                db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                db.SaveChanges();

            }


            string sqlQuery;
            SqlParameter[] sqlParams;

            sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return resultado.FirstOrDefault();

        }



        public void EnvioCorreo(string tipoDeCorreo, string Mail, string Motivo, string Mensaje, bool Publico, string EtapaSolicitud_GUID_id)
        {

            Tbl_Mail_History Tbl_mail_History = new Tbl_Mail_History();
            Tbl_mail_History.Guid_id = Guid.NewGuid().ToString();
            Tbl_mail_History.para = Mail;
            Tbl_mail_History.De = "Sistema SERNAF";
            Tbl_mail_History.TipoDeCorreo = tipoDeCorreo;
            Tbl_mail_History.CC = Motivo;
            Tbl_mail_History.Publico = Publico;
            Tbl_mail_History.Cuerpo = Mensaje;
            Tbl_mail_History.EtapaSolicitud_GUID_id = EtapaSolicitud_GUID_id;

            db.Tbl_Mail_History.Add(Tbl_mail_History);
            db.SaveChanges();



            System.Net.Mail.MailMessage Correo = new System.Net.Mail.MailMessage();
            Correo.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], "INAB Administrador");
            Correo.To.Add(new MailAddress(Mail));
            Correo.Subject = Motivo;
            AlternateView HTMLConImagenes = default(AlternateView);
            HTMLConImagenes = AlternateView.CreateAlternateViewFromString(Mensaje, null, "text/html");

            //HTMLConImagenes.LinkedResources.Add(imagen);
            Correo.AlternateViews.Add(HTMLConImagenes);
            Correo.IsBodyHtml = true;
            Correo.Priority = System.Net.Mail.MailPriority.High;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["Host"].ToString(), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Puerto"]));
            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Cuenta"], System.Configuration.ConfigurationManager.AppSettings["Clave"]);
            smtp.Send(Correo);
            return;
        }




        public ActionResult EtapaSolicitud_To_DoList(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            var tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id
            && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.EtapaSolicitudEstado_id < 3)
            {
                string sqlQuery;
                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };

                sqlQuery = "Exec SP_Gest_Ins_EtapaSolicitud_To_DoList @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Usuario";
                SqlParameter[] sqlParams;


                sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = solicitud_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Etapa_id",  Value = etapa_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = etaparuta_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = correlativoetapa_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input}
               };



                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();



                ViewBag.PermitirActualizacion = true;

            }
            else
            {
                ViewBag.PermitirActualizacion = false;
            }


            ViewBag.Title = db.Tbl_Gest_Etapa.Where(Obj => Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id).First().CheckList_Titulo;


            if (db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).ToList().Count() > 0)
            {


                return View(db.Tbl_Gest_EtapaSolicitud_To_doList.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).ToList());

            }
            else
            {
                return RedirectToAction("emptyView", "Gest_EtapaSolicitud");

            }

        }


        public ActionResult emptyView()
        {
            return View();

        }

        public ActionResult EtapaSolicitud_Formularios(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, int respuestaid)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            var tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id
            && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.EtapaSolicitudEstado_id < 3)  /* No Procesada */
            {
                string sqlQuery;
                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };

                sqlQuery = "Exec SP_Gest_Ins_EtapaSolicitud_Formularios @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @Usuario";
                SqlParameter[] sqlParams;

               sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = solicitud_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Etapa_id",  Value = etapa_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = etaparuta_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = correlativoetapa_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Respuesta_id",  Value = respuestaid, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input}
               };



                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                ViewBag.PermitirActualizacion = true;

            }
            else
            {
                ViewBag.PermitirActualizacion = false;
            }


            return View(db.Tbl_Gest_EtapaSolicitud_RespuestaFormulario.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id && Obj.Etapa_Respuesta_id == respuestaid).ToList());
        }


    }
}

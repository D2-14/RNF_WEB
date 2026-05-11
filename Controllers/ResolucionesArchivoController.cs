using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.IO;
using GemBox.Spreadsheet;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Data.SqlClient;
using PagedList;

using RNF_Web.Models;  //Permite utilizar todas las clases de modelos que usan el namespace "RNF_Web"


namespace RNF_Web.Controllers
{
    public class ResolucionesArchivoController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableBanner = new PdfPTable(1);



        public class RespuestaJSON
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public object data { get; set; }
        }

        // GET: ResolucionesArchivo
        public ActionResult Index(int? page, DateTime? FechaInicio, DateTime? FechaFin, string Expediente = null)
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




            ViewBag.Expediente = Expediente;


          

            string Query, QueryComplementoBusqueda;
            QueryComplementoBusqueda = "";



            Query = @"CREATE TABLE #SolicitudesTemp
                        (
                            Solicitud_Id BIGINT
                        );
                        DECLARE @Usuario AS int=" + objUs.intUsuario_id + " ";

            if (Expediente != null && Expediente !="")
            {

                Query += @"DECLARE @Expediente AS NVARCHAR(50)= '" + Expediente + "' ";

                Query += @"DECLARE @Region int,@Subregion int
						SELECT TOP 1 @Region=Region_id,@Subregion=SubRegion_id
						FROM dbo.Tbl_Seg_UsuarioSubRegion WHERE Usuario_id=@Usuario AND Estado_id=1 ";

                Query += @"INSERT INTO #SolicitudesTemp
                        SELECT  s.Solicitud_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id 
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1  and  s.Solicitud_id NOT IN (
                        SELECT Solicitud_id from dbo.Tbl_Gest_EtapaSolicitud ee WHERE ee.EtapaSolicitudEstado_id=1
						AND ee.Etapa_id NOT IN (20,21)
                        )
                        -- GROUP BY --
                        GROUP BY s.Solicitud_id
                        --, s.Solicitud_NumeroExpediente,se.Descripcion
                        HAVING MAX(es.Etapa_id) IN(20,21)						                        
                        
      --                  SELECT  s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Guid_id,s.Estado_id
						--,sub.swdatecreated,s.Solicitud_NumeroExpediente,s.No_Registro,s.SolicitudTipo_id,s.DPI_Titular,s.DocumentoVinculado,s.FechaResolucionSolicitud
						--,s.ResolucionSolicitud,s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id 
						SELECT  *
                        FROM (
                        				SELECT  *,
                        			           ROW_NUMBER() OVER (PARTITION BY e.Solicitud_id ORDER BY e.swdatecreated DESC) AS rn
                        			    FROM dbo.Tbl_Gest_EtapaSolicitud e	WHERE e.Etapa_id IN (20,21)		
                        			 ) AS sub		
                        		JOIN #SolicitudesTemp st ON st.Solicitud_Id=sub.Solicitud_id
                        		JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=st.Solicitud_Id
                        		WHERE rn =1 
                        		and
                          (DATEDIFF(MONTH, sub.swdatecreated, GETDATE()) +
                            (DAY(GETDATE()) - DAY(sub.swdatecreated)) * 1.0 / 
                            DAY(DATEADD(DAY, -DAY(DATEADD(MONTH, 1, sub.swdatecreated)), DATEADD(MONTH, 1, sub.swdatecreated)))) > 6
                        --AND sub.swcreatedby=@Usuario // la validacion no se realiza por USUARIO sino por la region de la solicitud 
                        AND s.Solicitud_NumeroExpediente =@Expediente
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
            UNION ALL 
						SELECT s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,'1' AS m,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id  AND es.EtapaRuta_id=e.EtapaRuta_id
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1 
                        	--AND Es.swcreatedby=@Usuario  // la validacion no se realiza por USUARIO sino por la region de la solicitud 
                        AND s.Solicitud_NumeroExpediente=@Expediente
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
						AND es.Etapa_id IN (11)
						UNION ALL 
						SELECT s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,'1' AS m,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id 
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1 
                        --AND Es.swcreatedby=@Usuario 
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
						GROUP BY 	
                        s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
						HAVING MAX(es.Etapa_id) IN(26)                        
						UNION ALL 
						SELECT s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,'1' AS m,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id 
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1 
                        	--AND Es.swcreatedby=@Usuario  // la validacion no se realiza por USUARIO sino por la region de la solicitud 
                        AND s.Solicitud_NumeroExpediente=@Expediente
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
						GROUP BY 	
                        s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
						HAVING MAX(es.Etapa_id) IN(26) 
                        DROP TABLE #SolicitudesTemp";
            }
            else
            {
                Query += @"DECLARE @Region int,@Subregion int
						SELECT TOP 1 @Region=Region_id,@Subregion=SubRegion_id
						FROM dbo.Tbl_Seg_UsuarioSubRegion WHERE Usuario_id=@Usuario AND Estado_id=1";

                Query += @"INSERT INTO #SolicitudesTemp
                        SELECT  s.Solicitud_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id 
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1  and  s.Solicitud_id NOT IN (
                        SELECT Solicitud_id from dbo.Tbl_Gest_EtapaSolicitud ee WHERE ee.EtapaSolicitudEstado_id=1
						AND ee.Etapa_id NOT IN (20,21)
                        )
                        -- GROUP BY --
                        GROUP BY s.Solicitud_id
                        --, s.Solicitud_NumeroExpediente,se.Descripcion
                        HAVING MAX(es.Etapa_id) IN(20,21)						                        
                        
      --                  SELECT  s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Guid_id,s.Estado_id
						--,sub.swdatecreated,s.Solicitud_NumeroExpediente,s.No_Registro,s.SolicitudTipo_id,s.DPI_Titular,s.DocumentoVinculado,s.FechaResolucionSolicitud
						--,s.ResolucionSolicitud,s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id 
						SELECT  *
                        FROM (
                        				SELECT  *,
                        			           ROW_NUMBER() OVER (PARTITION BY e.Solicitud_id ORDER BY e.swdatecreated DESC) AS rn
                        			    FROM dbo.Tbl_Gest_EtapaSolicitud e	WHERE e.Etapa_id IN (20,21)		
                        			 ) AS sub		
                        		JOIN #SolicitudesTemp st ON st.Solicitud_Id=sub.Solicitud_id
                        		JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=st.Solicitud_Id
                        		WHERE rn =1 
                        		and
                          (DATEDIFF(MONTH, sub.swdatecreated, GETDATE()) +
                            (DAY(GETDATE()) - DAY(sub.swdatecreated)) * 1.0 / 
                            DAY(DATEADD(DAY, -DAY(DATEADD(MONTH, 1, sub.swdatecreated)), DATEADD(MONTH, 1, sub.swdatecreated)))) > 6
                        --AND sub.swcreatedby=@Usuario						
	                    AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
            UNION ALL 
						SELECT s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,'1' AS m,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id  AND es.EtapaRuta_id=e.EtapaRuta_id
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1 
                        	--AND Es.swcreatedby=@Usuario  // la validacion no se realiza por USUARIO sino por la region de la solicitud                         
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
						AND es.Etapa_id IN (11)
						UNION ALL 
						SELECT s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,'1' AS m,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
                        FROM dbo.Tbl_Gest_EtapaSolicitud es
                        JOIN dbo.Tbl_Sol_Solicitud s ON s.Solicitud_id=es.Solicitud_id
                        JOIN dbo.Tbl_Gest_Etapa E ON E.Etapa_id=ES.Etapa_id 
                        JOIN Tbl_Sol_Solicitud_Estado se ON se.Estado_id=s.Estado_id
                        WHERE es.EtapaSolicitudEstado_id=1 
                        --AND Es.swcreatedby=@Usuario 
                        AND s.Region_id=@Region AND s.SubRegion_id=@Subregion
						GROUP BY 	
                        s.Solicitud_id,E.Etapa_id,E.EtapaRuta_id,es.CorrelativoEtapa_id,es.EtapaOrigen_Correlativo_id
						,es.Respuesta_id,es.Motivo,es.EtapaSolicitudEstado_id,es.Escalamiento_A,es.Escalamiento_B,es.Escalamiento_C,
						es.swcreatedby,es.swcreatedbyinterno,es.swdatecreated,es.swViewedby,es.swViewedbyinterno,es.swdateViewed,es.swLastViewedby,es.swLastViewedbyinterno,
						es.swdateLastViewed,es.swdateupdated,es.swupdatedby,es.swupdatedbyinterno,es.UsuarioResponsable_id,es.EtapaSolicitud_GUID_id,es.Solicitud_Guid_id,
						es.TecnicoOficio,es.NombreDocumentoFirmado,es.NombreDocumentoNoFirmado,es.NoInformeTecnico,es.Resolucion_Aprobada,es.Resolucion_Denegada,
						es.NoResolucion,es.Resolucion_Abandono,s.Solicitud_id,s.Solicitud_id,s.Region_id,s.SubRegion_id,s.Categoria_id,s.Sub_Categoria_id,s.Sub_Sub_Categoria_id,
						s.Guid_id,s.Estado_id,s.AreaTotalFincas,s.swdatecreated,s.swcreatedby,s.swcreatedbyinterno,s.swdateupdated,s.swupdatedby,s.swupdatedbyinterno,
						s.Solicitud_NumeroTemporal,s.Solicitud_NumeroExpediente,
						s.FacturaSerie,s.FacturaNumero,s.CantidadFolios,s.FechaRecepcionExpedienteFisico,s.RecepcionExpedienteFisicoby,
						s.SecretariaAsignada_id,s.TecnicoAsignado_id,s.JuridicoAsignado_id,s.JuridicoAsignadoNombramiento,s.JuridicoAsignadoFecha,
						s.TecnicoAsignadoFecha,s.JuridicoAsignadoPorSubRegional_id,s.TecnicoAsignadoPorSubRegional_id,s.PonderacionEvaluacionTecnica_id,
						s.PonderacionEvaluacionTecnicaFormulario_id,s.PonderacionEvaluacionTecnicaDeArea_id,s.PonderacionEvaluacionTecnicaDasometrica_id,
						s.PonderacionEvaluacionJuridica_id,s.TipoGestion_id,s.PermitirCambioDePropietarioRepresentante,s.PermitirCambioFincaRodalesDasometricos,
						s.PermitirSubirDocumentos,s.No_Registro,s.No_RegistroLiteral,s.No_RegistroCorrelativo,s.ResolucionInscripcion,s.ResolucionInscripcionFecha,
						s.InscripcionFecha,s.SolicitudTipo_id,s.EnmiendasRecibidas,s.TipoInactivacion_id,s.Descripcion_Inactivacion,s.PermitirCambiosEnDatosMotosierra,
						s.DepartamentoSolicitud_id,s.MunicipioSolicitud_id,s.PermitirCambioEnDatosTecnicoProfesional,s.DPI_Titular,s.DocumentoVinculado,
						s.Procedencia_Probosque,s.Procedencia_PinpepOld,s.Procedencia_PinpepNew,s.Procedencia_secorf,s.Procedencia_Expediente,s.Procedencia_POA,
						s.Procedencia_Licencia,s.Procedencia_Modalidad,s.Procedencia_Fase,s.Procedencia_NombreSolicitante,s.Procedencia_TipoProyecto,
						s.Procedencia_InformeTecnico,s.Procedencia_FechaInicioPeriodo,s.Procedencia_FechaFinPeriodo,s.Fecha_De_Inscripcion_RNF,s.Resolucion_De_Inscripcion_RNF
						,s.Fecha_De_ResolucionInscripcion_RNF,s.FechaResolucionSolicitud,s.ResolucionSolicitud,s.InactivacionTecnicoTipo_id,s.Descripcion_InactivacionTecnico,s.Bitacora_id,
						s.Notificacion_Direccion,s.Notificacion_Municipio_id,s.Notificacion_Departamento_id
						HAVING MAX(es.Etapa_id) IN(26) 
                        DROP TABLE #SolicitudesTemp";
            }


            List<Tbl_Sol_Solicitud> LstSolicitudes = db.Tbl_Sol_Solicitud.SqlQuery(Query).ToList();

            var ListarSolicitudes = (from d in LstSolicitudes select d);

            int pageSize = 20;
            int pageNumber = (page ?? 1);


            return View(ListarSolicitudes.ToPagedList(pageNumber, pageSize));
            //return View();
        }

        [HttpPost]
        public JsonResult GeneraVistaResolucion(int Solicitud_Guid_id, string NumeroExpediente,string numeroOficio)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            string archivo = GenerarDocumentoResolucion(Solicitud_Guid_id, NumeroExpediente, numeroOficio);

           

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_Guid_id);


            //Ver si ya se inserto la ETAPA DE Resolucion de archivo y de lo contrario INSERTAR
            int ContadorEtapaResolucion = db.Tbl_Gest_EtapaSolicitud
                 .Count(e => e.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && e.Etapa_id == 26);

            if (ContadorEtapaResolucion == 0)
            {


                int maxCorrelativo = db.Tbl_Gest_EtapaSolicitud
                     .Where(e => e.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id)
                     .Max(e => e.CorrelativoEtapa_id);


                Constants.InEtapaSolResolucionesArchivo(tbl_Sol_Solicitud.Solicitud_id, 26, 1.00m, maxCorrelativo + 1, maxCorrelativo, objUs.intUsuario_id, tbl_Sol_Solicitud.Guid_id, archivo);
            }

            string txtMostrar = "{\"Ubicacion\":\"" + archivo + "\"}";
            return Json(txtMostrar);

        }

        public string GenerarDocumentoResolucion(int Solicitud_Guid_id, string NumeroExpediente,string numeroOficio)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_Guid_id);

            if (numeroOficio ==null || numeroOficio =="")
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = "Debe ingresar el número de Oficio correspondiente.";

                return null;

            }
            else
            {
          
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();

            oParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                         where d.Etapa_id == 26
                         select d).FirstOrDefault();


            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime fecharesolucion;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            var fechaFormateada1 = DateTime.Now.ToString("dd 'de' MMMM 'del' yyyy", new System.Globalization.CultureInfo("es-ES"));
            ViewBag.FechaActual = fechaFormateada1;


            string strNombre = Solicitud_Guid_id + fecha + ".pdf";
            string strDirArchivo = strFolder + strNombre;


            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            Document doc = new Document(PageSize.LETTER);
                doc.SetMargins(1f, 1f, 25f, 50f);
                //doc.SetMargins(10f, 1f, 141f, 50f);

                Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }


            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);

            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

            var Enter = new Paragraph(" ");

            doc.Open();
            doc.Add(Enter);
                //if (Constants.VisualizarInformacionDesarrollo == 1)
                //{
                //    string urlact = this.Url.Action();
                //    LlenaBanner(urlact);
                //    doc.Add(tableBanner);
                //    doc.Add(Enter);
                //}


                CrearBanner crearBanner = new CrearBanner();



            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroResolucionArchivo(Solicitud_id: tbl_Sol_Solicitud.Solicitud_id, Usuario_id: objUs.intUsuario_id);

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_Guid_id);


            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = (from d in db.Tbl_Gral_SolicitudConfiguracionTipo
                                                                                       where d.SolicitudTipo_id == tbl_sol_Solicitud.SolicitudTipo_id
                                                                                       select d).FirstOrDefault();


                //UBICACION EMPRESA
                var resultadoUbicacionEmpresa = db.Tbl_Sol_Empresa_Entidad.Join(db.Tbl_Gral_Departamento,
                                                                                empresa => empresa.DepartamentoEmpresaEntidad_id,
                                                                                depto => depto.Departamento_id,
                                                                                    (empresa, depto)
                                                                                    => new
                                                                                    {

                                                                                        depto.Departamento,
                                                                                        empresa.Solicitud_id,
                                                                                        empresa.DepartamentoEmpresaEntidad_id,
                                                                                        empresa.MunicipioEmpresaEntidad_id
                                                                                    }
                                                                                    ).Join(db.Tbl_Gral_Municipio,
                                                                                    empresa1 => empresa1.MunicipioEmpresaEntidad_id,
                                                                                    municipio => municipio.Municipio_id,
                                                                                     (empresa1, municipio) => new
                                                                                     {
                                                                                         empresa1.Departamento,
                                                                                         municipio.Municipio,
                                                                                         empresa1.Solicitud_id,
                                                                                         municipio.Codigo_Municipal
                                                                                     }
                                                                                    )

                                                                                        .Where(x => x.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

                //UBICACION FINCA
                var resultadoUbicacionFinca = db.Tbl_Sol_Finca.Join(db.Tbl_Gral_Departamento,
                                                                                finca => finca.DepartamentoFinca_Id,
                                                                                depto => depto.Departamento_id,
                                                                                    (finca, depto)
                                                                                    => new
                                                                                    {

                                                                                        depto.Departamento,
                                                                                        finca.Solicitud_id,
                                                                                        finca.DepartamentoFinca_Id,
                                                                                        finca.MunicipioFinca_Id
                                                                                    }
                                                                                    ).Join(db.Tbl_Gral_Municipio,
                                                                                    finca_ => finca_.MunicipioFinca_Id,
                                                                                    municipio => municipio.Municipio_id,
                                                                                     (finca_, municipio) => new
                                                                                     {
                                                                                         finca_.Departamento,
                                                                                         municipio.Municipio,
                                                                                         finca_.Solicitud_id,
                                                                                         municipio.Codigo_Municipal
                                                                                     }
                                                                                    )

                                                                                        .Where(x => x.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

            Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();

                string varTitulo;

            varTitulo = "RESOLUCIÓN DE ARCHIVO DE EXPEDIENTE";

            DateTime fechaFormateada = DateTime.Parse(resultsp.strFecha);

                string fechaFinal = fechaFormateada.ToString("MMMM yyyy", new CultureInfo("es-ES"));

                crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, fechaFinal, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);


                //Correlativo
                Result_SP_CorrelativoResolucionesArchivo correlativo = identificadorOficialGestion.Correlativo(tbl_sol_Solicitud.Solicitud_id, objUs.intUsuario_id);


                string No_Resolucion = resultsp.Identificador;

                if (resultadoUbicacionEmpresa==null)
                {
            LlenaBanner("Resolución No. " + NumeroExpediente.Substring(0, 3).Replace("-", "").Replace(".", "") + "-" + resultadoUbicacionFinca.Codigo_Municipal + "-" + correlativo.strCorrelativo + "-" + tbl_Sol_Solicitud_Sub_Categoria.CodigoServicioINAB + "-" + hoy.Year ,"Derecha", "Blanco");
            doc.Add(tableBanner);

                }
                else
                {
                    LlenaBanner("Resolución No. " + NumeroExpediente.Substring(0, 3).Replace("-", "").Replace(".", "") + "-" + resultadoUbicacionEmpresa.Codigo_Municipal +"-" + correlativo.strCorrelativo + "-"+ tbl_Sol_Solicitud_Sub_Categoria.CodigoServicioINAB + "-" + hoy.Year ,"Derecha", "Blanco");
            doc.Add(tableBanner);

                }


            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

            doc.Add(Enter);

            Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);

            //Tbl_Gral_Departamento tbl_Gral_Departamento = db.Tbl_Gral_Departamento.Find(tbl_seg_usuarioExterno.Departamento_id);
            Tbl_Gral_Departamento tbl_Gral_Departamento = db.Tbl_Gral_Departamento.Find(tbl_Gral_SubRegion.DepartamentoSubRegion_id);

            //Tbl_Gral_Municipio tbl_Gral_Municipio = db.Tbl_Gral_Municipio.Find(tbl_seg_usuarioExterno.Municipio_id);
            Tbl_Gral_Municipio tbl_Gral_Municipio = db.Tbl_Gral_Municipio.Find(tbl_Gral_SubRegion.MunicipioSubRegion_id);

            Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);


            Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda = db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id && Obj.Estado_id == true).FirstOrDefault();

            // Departamento Municipio |UBICACION|
            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();
            Tbl_Sol_Finca tbl_Sol_Finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();


            Tbl_Sol_PropietarioPersonaIndividual tbl_Sol_PropietarioPersonaIndividual = db.Tbl_Sol_PropietarioPersonaIndividual.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();
            Tbl_Sol_PropietarioPersonaJuridica tbl_Sol_PropietarioPersonaJuridica = db.Tbl_Sol_PropietarioPersonaJuridica.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();
            String NombrePropietario = $"{tbl_Sol_PropietarioPersonaIndividual?.Nombres ?? ""} {tbl_Sol_PropietarioPersonaIndividual?.Apellidos ?? ""} {tbl_Sol_PropietarioPersonaJuridica?.Nombre ?? ""}".Trim();
            String CUI_PersonaIndividual = $"{tbl_Sol_PropietarioPersonaIndividual?.No_Documento ?? ""}".Trim();
            

            var resultadoCategoria = db.Tbl_Sol_Solicitud.Join(db.Tbl_Sol_Solicitud_Categoria,
                                                                 solicitud => solicitud.Categoria_id,
                                                                 categoria => categoria.Categoria_id,
                                                                 (solicitud, categoria)
                                                                  => new
                                                                  {
                                                                      solicitud.Solicitud_id,
                                                                      categoria.Descripcion

                                                                  }
                                                                  ).Where(x => x.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

          
            //********************  PARRAFO NO 1 **********************

            String Parrafo1 = "DIRECCIÓN SUB-REGIONAL " + tbl_Gral_SubRegion.No_SubRegion + ", CON SEDE EN EL MUNICIPIO DE " + tbl_Gral_Municipio.Municipio + " DEL DEPARTAMENTO DE " + tbl_Gral_Departamento.Departamento + ", INSTITUTO NACIONAL DE BOSQUES -INAB-, " + ViewBag.FechaActual + ".";

            LlenaBanner(Parrafo1.ToUpper(), "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            //********************  PARRAFO NO 2 **********************
            Tbl_Sol_RepresentanteLegal tbl_Sol_RepresentanteLegal = db.Tbl_Sol_RepresentanteLegal.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

            string Parrafo2;
            if (tbl_Sol_PropietarioPersonaJuridica?.Nombre != null)
            {

                if (tbl_sol_Solicitud_Categoria.Categoria_id==5 || tbl_sol_Solicitud_Categoria.Categoria_id == 11)
                {
                     Parrafo2 = "Se tiene a la vista el expediente administrativo número " + NumeroExpediente + ", el cual contiene la solicitud de " + tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro.Replace("Primer","").Replace("Segunda","") + " en el Registro Nacional Forestal en la Categoría de " + resultadoCategoria.Descripcion.ToLower() + " subcategoria de " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToLower() + ", ubicada en el municipio de " + resultadoUbicacionEmpresa.Municipio +
                        " del departamento de " + resultadoUbicacionEmpresa.Departamento + ", realizada por: " + NombrePropietario + " representada legalmente por: " + tbl_Sol_RepresentanteLegal.Nombres + " " + tbl_Sol_RepresentanteLegal.Apellidos + " identificado (a) con el Código Unico de Identificación número: " + tbl_Sol_RepresentanteLegal.RepresentanteNo_Documento + " extendido por el Registro Nacional de las Personas -RENAP-.";
               

                }
                else
                {
                     Parrafo2 = "Se tiene a la vista el expediente administrativo número " + NumeroExpediente + ", el cual contiene la solicitud de " + tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro.Replace("Primer", "").Replace("Segunda", "") + " en el Registro Nacional Forestal en la Categoría de " + resultadoCategoria.Descripcion.ToLower() + " subcategoria de " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToLower() + ", ubicada en el municipio de " + resultadoUbicacionFinca.Municipio +
                                      " del departamento de " + resultadoUbicacionFinca.Departamento +", realizada por: " + NombrePropietario + " representada legalmente por: " + tbl_Sol_RepresentanteLegal.Nombres + " " + tbl_Sol_RepresentanteLegal.Apellidos + " identificado (a) con el Código Unico de Identificación número: " + tbl_Sol_RepresentanteLegal.RepresentanteNo_Documento + " extendido por el Registro Nacional de las Personas -RENAP-.";

                  
                }
            }
            else
            {

                if (tbl_sol_Solicitud_Categoria.Categoria_id == 5 || tbl_sol_Solicitud_Categoria.Categoria_id == 11)
                {

                     Parrafo2 = "Se tiene a la vista el expediente administrativo número " + NumeroExpediente + ", el cual contiene la solicitud de " + tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro.Replace("Primer", "").Replace("Segunda", "") + " en el Registro Nacional Forestal en la categoria de " + resultadoCategoria.Descripcion.ToLower() + " subcategoria de " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToLower() + ", ubicada en el municipio de " + resultadoUbicacionEmpresa.Municipio +
                    " del departamento de " + resultadoUbicacionEmpresa.Departamento +", realizada por: " + NombrePropietario + " identificado (a) con el Código Unico de Identificación número: " + CUI_PersonaIndividual + " extendido por el Registro Nacional de las Personas -RENAP-.";
                }
                else
                {
                 Parrafo2 = "Se tiene a la vista el expediente administrativo número " + NumeroExpediente + ", el cual contiene la solicitud de " + tbl_Gral_SolicitudConfiguracionTipo.TipoRegistro.Replace("Primer", "").Replace("Segunda", "") + " en el Registro Nacional Forestal en la categoria de " + resultadoCategoria.Descripcion.ToLower() + " subcategoria de " + tbl_Sol_Solicitud_Sub_Categoria.Descripcion.ToLower() + ", ubicada en el municipio de " + resultadoUbicacionFinca.Municipio +
                    " del departamento de " + resultadoUbicacionFinca.Departamento +", realizada por: " + NombrePropietario + " identificado (a) con el Código Unico de Identificación número: " + CUI_PersonaIndividual + " extendido por el Registro Nacional de las Personas -RENAP-.";

                }

            }

            LlenaBanner(Parrafo2, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);


            //********************  PARRAFO NO 3 **********************

            string Parrafo3 = "Que el artículo quinto del Decreto Legislativo 101-96 establece que el Instituto Nacional de Bosques -INAB- es una entidad estatal, autónoma, descentralizada con personalidad jurídica, patrimonio propio e independencia administrativa, es el órgano de dirección y autoridad competente del Sector Público Agrícola en materia forestal.";

            LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(Parrafo3, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            //********************  PARRAFO NO 4 **********************

            string Parrafo4 = "Que el reglamento del Registro Nacional Forestal, Resolución de Junta Directiva, acta JD.01.19.2023, en el artículo número 16, establece que el Director de la oficina Subregional del INAB correspondiente, con base al análisis legal y técnico de la documentación respectiva debe emitir la resolución correspondiente, el artículo 13 establece los requisitos de inscripción, el artículo 19 establece que la  actualización es periódica y que el INAB establecerá los montos que se deben cobrar para la inscripción o actualización en el RNF; siendo el inciso J del Acta  No. JD.02.16.2024, y su manual de procedimientos y formularios vigentes.";

            LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(Parrafo4, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);


            //********************  PARRAFO NO 5 **********************


            DateTime fechaFormateada_ = DateTime.Parse(tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.swdatecreated.ToString());

            if (tbl_Sol_PropietarioPersonaJuridica?.Nombre != null)
            {




                string Parrafo5 = "Que con fecha " + fechaFormateada_.ToString("dd/MM/yyyy") + " esta Dirección Subregional notificó a " + NombrePropietario + " el oficio número " + numeroOficio + " mediante el cual se le hizo el requerimiento de documentos o enmiendas técnicas y/o juridicas, como previo para continuar con el trámite del" +
                " expediente administrativo número " + NumeroExpediente + ".  " + tbl_Sol_RepresentanteLegal.Nombres + " " + tbl_Sol_RepresentanteLegal.Apellidos + " no ha realizado acción alguna para subsanar dicho requerimiento, dejando de accionar por más de seis(6) meses despúes de su notificación";

                LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(Parrafo5, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

            }
            else
            {

                string Parrafo5 = "Que con fecha " + fechaFormateada_.ToString("dd/MM/yyyy") + " esta Dirección Subregional notificó a " + NombrePropietario + " el oficio número " + numeroOficio + " mediante el cual se le hizo el requerimiento de documentos o enmiendas técnicas y/o juridicas, como previo para continuar con el trámite del" +
              " expediente administrativo número " + NumeroExpediente + ". " + NombrePropietario + " no ha realizado acción alguna para subsanar dicho requerimiento, dejando de accionar por más de seis(6) meses despúes de su notificación";

                LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(Parrafo5, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);

            }



            //********************  PARRAFO NO 6 **********************

            string Parrafo6 = "Esta Dirección Subregional, con base en lo considerado y a lo preceptuado en los artículos 1,2,6,88 del " +
                    "Decreto Legislativo número 101 - 96, Ley Forestal, en lo establecido en los artículos " +
                    "1,2,3,6,7,8,10,11,12,13,15,16,17,18,19,20,24 y 26 del Reglamento del Registro Nacional Forestal vigente " +
                    "y articulo 5, de la Ley de lo contencioso administrativo, Decreto 119-96.";


            LlenaBanner("POR TANTO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(Parrafo6, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);


            //********************  PARRAFO NO 7 **********************

            if (tbl_Sol_PropietarioPersonaJuridica?.Nombre != null)
            {
                string Parrafo7 = "I. Denegar la solicitud de: " + NombrePropietario + " contenida en el expediente administrativo número " + NumeroExpediente + ".";
                string Parrafo8 = "II. Archivar el expediente administrativo número " + NumeroExpediente + " a nombre de: " + NombrePropietario + " por haberse dejado de accionar por más de seis meses en la gestión del mismo.";

                LlenaBanner("RESUELVE", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(Parrafo7, "Justificado", "Blanco");
                doc.Add(tableBanner);
                          
                
                LlenaBanner(Parrafo8, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }
            else
            {
                string Parrafo7 = "I. Denegar la solicitud de: " + NombrePropietario + " contenida en el expediente administrativo número " + NumeroExpediente + ".";
                string Parrafo8 = "II. Archivar el expediente administrativo número " + NumeroExpediente + " a nombre de: " + NombrePropietario + " por haberse dejado de accionar por más de seis meses en la gestión del mismo.";


                LlenaBanner("RESUELVE", "Centro", "Blanco");
                doc.Add(tableBanner);
                LlenaBanner(Parrafo7, "Justificado", "Blanco");
                doc.Add(tableBanner);             
                               
                LlenaBanner(Parrafo8, "Justificado", "Blanco");
                doc.Add(tableBanner);
                doc.Add(Enter);
            }



            LlenaBanner(DatosSubRegional.NombreCompleto, "Centro", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Dirección SubRegional  " + tbl_Gral_SubRegion.No_SubRegion, "Centro", "Blanco");
            doc.Add(tableBanner);



            doc.Close();
            writer.Close();


            db.Entry(tbl_sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();


            return strNombre;

            }

        }



        public JsonResult VistaPreviaResolucion()
        {


            int codRespuesta = 0;
            string strRespuesta = "";



            //tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado = GenerarDictamen_PDF(tbl_sol_Solicitud_OficioDictamenJuridico, "Dictamen");

            //db.Entry(tbl_sol_Solicitud_OficioDictamenJuridico).State = EntityState.Modified;
            //db.SaveChanges();


            //strRespuesta = tbl_sol_Solicitud_OficioDictamenJuridico.documentoNoFirmado;





            codRespuesta = 1;

            string jsonResult = "{\"CodRespuesta\":"
                           + "\"" + codRespuesta + "\","
                           + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }


        public ActionResult ResolucionesArchivo()
        {


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            select d).FirstOrDefault();



            return View(oSolicitud);

        }


        //public string GenerarResolucion_PDF()
        //{

        //    string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
        //    string strFolder = Server.MapPath("~/") + strDir;
        //    DateTime hoy = DateTime.Now;
        //    string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

        //    string strNombre = "Resolucion" + fecha + ".pdf";
        //    string strDirArchivo = strFolder + strNombre;


        //    Document doc = new Document(PageSize.LETTER);
        //    doc.SetMargins(1f, 1f, 25f, 50f);

        //    Usuario objUs = new Usuario();
        //    objUs.intUsuario_id = 0;

        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];

        //    }


        //    strDirArchivo = strFolder + strNombre;


        //    if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);


        //    FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
        //    PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
        //    doc.Open();

        //    doc.Open();
        //    var Enter = new Paragraph(" ");
        //    doc.Add(Enter);
        //    doc.Add(Enter);
        //    if (Constants.VisualizarInformacionDesarrollo == 1)
        //    {
        //        string urlact = this.Url.Action();
        //        LlenaBanner(urlact);
        //        doc.Add(tableBanner);
        //        doc.Add(Enter);
        //    }




        //    CrearBanner crearBanner = new CrearBanner();



        //}

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

            if (alineacion == "Justificado")
            {
                c1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }

            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        private void LlenaBanner(String Leyenda)
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }


        public JsonResult JsonProcesarFirmaElectronica(long Solicitud_Id, string UsuarioFE, string PasswordFE)
        {


            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }


            // Bitacora activa.
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;


            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            DateTime hoy = DateTime.Now;

            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            string rootpdf = rootpath + "U" + fecha + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";


            Tbl_Sol_IdentificadorOficial tbl_Sol_IdentificadorOficial = db.Tbl_Sol_IdentificadorOficial.Where(Obj => Obj.Solicitud_id == Solicitud_Id && Obj.GestionTipo_id==17).First();
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Solicitud_id == Solicitud_Id).First();

           


            rootpdf = rootpath + Solicitud_Id;

            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);

            Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 0, "A.- Inicia proceso de firma electronica ResolucionesArchivo-JsonProcesarFirmaElectronica");


         

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == 26).First();
            rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;

            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };

            try
            {
                Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = null;

                if ((Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado != null) && (Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado.ToString() != ""))
                {
                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 0, "A.- El documento ya ha sido firmado antes Home-JsonProcesarFirmaElectronica ");

                    Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado;
                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "Este documento ya ha sido firmado. No puede firmarlo nuevamente." + "\"}";

                    return Json(jsonResultUsr);

                }

                Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 2, "B.- Se busca obtener el bearer");

                strBearer = GetBearer();

                Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 3, "C.- Bearer obtenido");

                if (strBearer.Length < 125)
                {

                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 4, "D.- Bearer erroneo, menor a 125 caracteres");


                    intRespuesta = 0;

                    jsonResultUsr = "{\"CodRespuesta\":"
                              + "\"" + intRespuesta + "\","
                              + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                    return Json(jsonResultUsr);

                }

                string strDocumentoSubido = CallCORS(strBearer, rootpdf);

                if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
                {

                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 5, "N.- Error al subir el documento.");

                    intRespuesta = 0;

                    strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                    return Json(jsonResultUsr);


                }

                //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

                RequestUtil requestUtil = new RequestUtil();

                string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);


                if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
                {


                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 6, "R.- Error al firmar el documento, Usuario o Password Erroneos.");

                    intRespuesta = 0;

                    strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                          + "\"" + intRespuesta + "\","
                                          + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                    return Json(jsonResultUsr);


                }

                Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 7, "W.- Intentando obtener archivo firmado.");

                if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
                {

                    string SP_SqlQuery = "EXEC [dbo].[SP_GestEtapaSolicitud_ActualizaDocumentoFirmadoResolucionesArchivo] @Solicitud_id, @Firmante, @GUID_id, @NombreDocumentoFirmado";
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@Solicitud_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Firmante", Value = strEnc, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@GUID_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_Guid_id, Direction = System.Data.ParameterDirection.Input },                        
                        new SqlParameter { ParameterName = "@NombreDocumentoFirmado", Value = strDocumentofirmado + ".pdf", Direction = System.Data.ParameterDirection.Input },
                    };

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();

                    //Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                    //db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                    //db.SaveChanges();

                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 8, "X.- El archivo se obtuvo con exito.");


                 
                    if (Tbl_Gest_etapaSolicitud.Etapa_id==26)
                    {


                        var Etapa_Solicitud = new List<int> {20,21,26};

                        var etapasol = db.Tbl_Gest_EtapaSolicitud
                                              .Where(u => Etapa_Solicitud.Contains((int)u.Etapa_id) && u.Solicitud_id==Tbl_Gest_etapaSolicitud.Solicitud_id)
                                              .ToList();


                        foreach (var etapasol_ in etapasol)
                        {
                            etapasol_.EtapaSolicitudEstado_id = 3;
                            
                        }

                        db.SaveChanges();


                    }

                }
                else
                {
                    Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 9, "X.- No se logró obtener el archivo firmado.");

                }

                Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 10, "Z.- FE generada con éxito.");

                intRespuesta = resultFromStoreProcedure.respuesta;

                if (intRespuesta == 1)
                {
                    Constants.FirmaElectronicaInBitacoraDelResolucionesArchivo(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), UsuarioFE);


                    jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

                }
                else
                {
                    jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + 0 + "\","
                                      + "\"strRespuesta\":" + "\"" + resultFromStoreProcedure.mensaje + "\"}";

                }

                return Json(jsonResultUsr);
            }
            catch (Exception ex)
            {
                Constants.FirmaElectronicaInsertarBitacora(tbl_Sol_Solicitud.Guid_id, tbl_Sol_Solicitud.Solicitud_id.ToString(), strEnc, 1, 10, "Z.- No se pudo generar la FE" + ex.Message.ToString() + "..." + ex.InnerException.ToString());


                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + $"No se logró realizar la firma electrónica. " + ex.Message.ToString() + "\"}";


                return Json(jsonResultUsr);

            }         


        }



        public JsonResult ActualizaEtapaRespuestaResolucionesArchivo(long solicitud_id, int etapa_id, decimal etaparuta_id, string motivo, int respuestaid,string NoOficio)
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
            ResultFromStoreProcedure Respuesta = gest_EtapaModel.ConfirmarRespuesta(objUs, solicitud_id, etapa_id, etaparuta_id, 0, motivo, respuestaid,NoOficio);
            //if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
            if (Respuesta.respuesta == 1)
            {


                codRespuesta = 1;
                strRespuesta = "Se ha notificado exitosamente.";


            }
            else
            {
                codRespuesta = 0;
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
                if ((Respuesta.mensaje ?? "").Trim() != "")
                {
                    strRespuesta = Respuesta.mensaje;
                }
            }

            jsonResult =   "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";


            return Json(jsonResult);

        }


        public string GetBearer()
        {
            try
            {

                var client = new RestClient(Constants.Address_Bearer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string body = @"{
                        " + "\n" +
                            @"""Username"":""RNFUser"",
                        " + "\n" +
                            @"""Password"":""RNF_Password""
                        " + "\n" +
                            @"}
                        " + "\n" +
                            @"";

                body = body.Replace("RNFUser", Constants.RNF_Username);
                body = body.Replace("RNF_Password", Constants.RNF_Password);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.Content.ToString().Replace("\"", "").Length < 250)
                {
                    return "Error en credenciales RNF User  y RNF Password en la solicitud de bearer";
                }

                if (response.Content.ToString().Replace("\"", "").Length > 270)
                {
                    return "Error: " + response.Content.ToString().Replace("\"", "").Substring(1, 100);
                }
                else
                {
                    return "Bearer " + response.Content.ToString().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message.Substring(1, 100);
            }
        }


        public string CallCORS(string strSign, string strPath)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveUpload);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", strSign);
                //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
                request.AddFile("nombre", strPath, "application/pdf");
                request.AddParameter("parentGoogleDriveId", Constants.strParentGoogleDrive);
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                return joResponse["GoogleDriveId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error: No se logró subir el archivo a Google Drive, reporte a informatica. " + ex.Message.ToString();
            }
        }


        public bool getFile(string strBearer, string strFile, string path)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveDownLoad + strFile);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", strBearer);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                byte[] buffer = client.DownloadData(request);

                MemoryStream ms = new MemoryStream(buffer);
                FileStream file = new FileStream(@path + strFile + ".pdf", FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }

            // IRestResponse response = client.Execute(request);

        }


    }

}


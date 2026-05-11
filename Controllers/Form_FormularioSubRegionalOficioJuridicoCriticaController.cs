using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;
using IOPWord = Microsoft.Office.Interop.Word;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSubRegionalOficioJuridicoCriticaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: OficioJuridico_EnmiendaCritica
        public ActionResult Index(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            string guidsolicitud = Guid_id;
            string guidetapasolicitud = tbl_gest_EtapaSolicitud.EtapaSolicitud_GUID_id;

            Usuario objUs = new Usuario();
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

        
            ViewBag.guidsolicitud = guidsolicitud;
            ViewBag.guidetapasolicitud = guidetapasolicitud;

            return View(tbl_gest_EtapaSolicitud);
        }




        public ActionResult Create(string Guid_id, string EtapaSolicitudGuid)
        {
        //    Usuario objUs = new Usuario();
        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
        //        ViewBag.Mensaje = objSesion.getStrMensaje();
        //        return RedirectToAction("../Login/Index");
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];
        //    }

        //    string guidsolicitud, guidetapasolicitud;
        //    guidsolicitud = Guid_id;
        //    guidetapasolicitud = EtapaSolicitudGuid;

        //    Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


        //    oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
        //                       where d.EtapaSolicitud_GUID_id == guidetapasolicitud && d.Solicitud_Guid_id == guidsolicitud
        //                       select d).FirstOrDefault();




        //    Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda_Critica oCriticasOficioJuridico_Enmienda = new Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda_Critica();
        //    oCriticasOficioJuridico_Enmienda.Solicitud_Guid_id = guidsolicitud;
        //    oCriticasOficioJuridico_Enmienda.EtapaSolicitud_Guid_id = guidetapasolicitud;
        //    oCriticasOficioJuridico_Enmienda.Solicitud_id = oEtapaSolicitud.Solicitud_id;
        //    oCriticasOficioJuridico_Enmienda.Etapa_id = oEtapaSolicitud.Etapa_id;
        //    oCriticasOficioJuridico_Enmienda.EtapaRuta_id = oEtapaSolicitud.EtapaRuta_id;
        //    oCriticasOficioJuridico_Enmienda.CorrelativoEtapa_id = oEtapaSolicitud.CorrelativoEtapa_id;
        //    oCriticasOficioJuridico_Enmienda.swcreatedby = objUs.intUsuario_id;
        //    oCriticasOficioJuridico_Enmienda.swcreatedbyinterno = true;

        //    return View(oCriticasOficioJuridico_Enmienda);
         return View();
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
            //if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
            if (Respuesta.respuesta == 1)

            {
                codRespuesta = 1;
                strRespuesta = "Respuesta enviada.";
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

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);

        }


        public int ConfirmarRespuesta(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid)
        {
            Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            if (tbl_gest_etapasolicitud.Respuesta_id != 0)
            {
                return 0;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return 0;
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

            return resultado[0].respuesta;

        }

        public ActionResult ListaCriticasOficioJuridico(string Guid_id, string EtapaSolicitudGuid)
        {
            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;

            List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda> oEtapaOficioJuridico_Enmienda = new List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda>();
            oEtapaOficioJuridico_Enmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                                   where d.Solicitud_Guid_id == Guid_id && d.Estado_id == true
                                   orderby d.Enmienda_id
                                   select d).ToList();

            return View(oEtapaOficioJuridico_Enmienda);

        }

        public ActionResult Edit()
        {
            return View();
        }

        void ReemplazaPalabraWord(IOPWord.Application appWord, string datobusqueda, string datoreemplazar)
        {
            string datoinsertar;

            #region valores para reemplazar
            object matchCase = true;

            object matchwholeWord = true;

            object matchwildCards = false;

            object matchSoundLike = false;

            object nmatchAllforms = false;

            object forward = true;

            object format = false;

            object matchKashida = false;

            object matchDiactitics = false;

            object matchAlefHamza = false;

            object matchControl = false;

            object read_only = false;

            object visible = true;

            object replace = -2;
            object replaceAll = IOPWord.WdReplace.wdReplaceAll;
            object wrap = 1;
            #endregion

            datoinsertar = validareemplazo(datobusqueda, datoreemplazar);
            appWord.Selection.Collapse();
            appWord.Selection.Find.ClearFormatting();
            appWord.Selection.Find.Execute(FindText: datobusqueda, ReplaceWith: datoinsertar, Replace: replaceAll);

        }

        void ReemplazaListaWord(IOPWord.Application appWord, string datobusqueda, List<string> datoreemplazar)
        {
            string datoinsertar, reemplazaenlist;
            reemplazaenlist = "";
            int contador;
            contador = 0;
            #region valores para reemplazar
            object matchCase = true;

            object matchwholeWord = true;

            object matchwildCards = false;

            object matchSoundLike = false;

            object nmatchAllforms = false;

            object forward = true;

            object format = false;

            object matchKashida = false;

            object matchDiactitics = false;

            object matchAlefHamza = false;

            object matchControl = false;

            object read_only = false;

            object visible = true;

            object replace = -2;
            object replaceAll = IOPWord.WdReplace.wdReplaceAll;
            object wrap = 1;
            #endregion

            datoinsertar = validareemplazolista(datobusqueda, datoreemplazar);
            if (datoinsertar == "Ok")
            {

                for (int i = 0; i < datoreemplazar.Count(); i++)
                {
                    contador += 1;
                    reemplazaenlist += contador + ". " + datoreemplazar[i] + Environment.NewLine;

                }

                appWord.Selection.Collapse();
                appWord.Selection.Find.ClearFormatting();
                appWord.Selection.Find.Execute(FindText: datobusqueda, ReplaceWith: reemplazaenlist, Replace: replaceAll);

            }

        }

        string validareemplazo(string valorbase, string valorreemplazo)
        {
            if (
                    (valorreemplazo == null)
                || (valorreemplazo == "")
                //|| (valorreemplazo == "0")
                )
            {
                return valorbase;
            }
            else
            {
                return valorreemplazo;
            }
        }

        string validareemplazolista(string valorbase, List<string> valorreemplazo)
        {
            if (valorreemplazo.Count() == 0)
            {
                return valorbase;
            }
            else
            {
                return "Ok";
            }
        }

        string GenerarEnmiendaOficioJuridico(string Guid_id, string EtapaSolicitudGuid)
        {

            IOPWord.Application appWord;
            IOPWord.Document docWord;

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated, strvalidar;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioSubRegional.docx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioSubRegional_{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destpdf = $"{destfinal}.pdf";
            docreturn = $"/{partialdestpath}{nombrereporte}";
            #endregion


            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where d.EtapaSolicitud_GUID_id == guidetapasol && d.Solicitud_Guid_id == guidid
                               select d).FirstOrDefault();

            string nombredia, nombremes;
            nombredia = swdatecreated.ToString("dddd", CultureInfo.CreateSpecificCulture("es-MX"));
            nombremes = swdatecreated.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));

            strswdatecreated = swdatecreated.ToString("dd/MM/yyyy HH:mm");
            stronlydatecreated = $"{nombredia} {swdatecreated.ToString("dd")} de {nombremes} de {swdatecreated.ToString("yyyy")}";


            if (oEtapaSolicitud != null)
            {
                List<string> oEnmiendasOficio = new List<string>();
                List<string> oObjecionesOficio = new List<string>();
                List<string> datolistareemplazar = new List<string>();
                Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                                where d.Guid_id == guidid
                                                select d).FirstOrDefault();

                Tbl_Seg_UsuarioExterno oSolicitante = (from d in db.Tbl_Seg_UsuarioExterno
                                                       where d.Usuario_id == oSolicitud.swcreatedby
                                                       select d).FirstOrDefault();

                Tbl_Seg_Usuario oJuridicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.JuridicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda> oOficioJuridico_Enmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                                                                              where d.EtapaSolicitud_GUID_id == guidetapasol && d.Solicitud_Guid_id == guidid && d.Estado_id == true
                                                                              select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                string[] paramsQuery = new string[]
                {
                    oSolicitud.Region_id.ToString(),
                    oSolicitud.SubRegion_id.ToString()
                };
                string NombreSubRegional = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_NombreSubDirectorRegional](@p0, @p1) ", paramsQuery).FirstOrDefault();

                if (oOficioJuridico_Enmienda.Count() > 0)
                {

                    oOficioJuridico_Enmienda.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oOficioJuridico_Enmienda.Count(); i++)
                    {

                        if((oOficioJuridico_Enmienda[i].Descripcion != null) && (oOficioJuridico_Enmienda[i].Descripcion != ""))
                        {
                            strvalidar = oOficioJuridico_Enmienda[i].Descripcion;
                            if(strvalidar.Trim() != "")
                            {
                                oEnmiendasOficio.Add($"{oOficioJuridico_Enmienda[i].Descripcion.Trim()}");
                            }
                        }

                        if ((oOficioJuridico_Enmienda[i].Objecion != null) && (oOficioJuridico_Enmienda[i].Objecion != ""))
                        {
                            strvalidar = oOficioJuridico_Enmienda[i].Objecion;
                            if (strvalidar.Trim() != "")
                            {
                                oObjecionesOficio.Add($"{oOficioJuridico_Enmienda[i].Objecion.Trim()}");
                            }
                        }

                    }

                }


                if (System.IO.File.Exists(destdocx))
                {
                    System.IO.File.Delete(destdocx);
                }

                if (System.IO.File.Exists(destpdf))
                {
                    System.IO.File.Delete(destpdf);
                }



                System.IO.File.Copy(machotedocx, destdocx);

                appWord = new IOPWord.Application();
                appWord.DisplayAlerts = IOPWord.WdAlertLevel.wdAlertsNone;
                appWord.Visible = false;

                docWord = appWord.Documents.Open(FileName: destdocx, ReadOnly: false);
                docWord.Activate();

                try
                {

                    datobusqueda = "{No_Oficio}";
                    datoreemplazar = "2022-" + oEtapaSolicitud.Solicitud_id.ToString();

                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Nombre_SubRegional}";
                    datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Director_SubRegional}";
                    datoreemplazar = NombreSubRegional;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Fecha}";
                    datoreemplazar = stronlydatecreated;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{No_Expediente}";
                    datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{SubCategoria}";
                    datoreemplazar = oSolicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{SubRegion}";
                    datoreemplazar = oSolDatosDescripcion.SubRegion;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Solicitante}";
                    datoreemplazar = oSolDatosDescripcion.Propietario;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Propietario_RepresentanteLegal}";
                    datoreemplazar = oSolDatosDescripcion.Propietario;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Direccion}";
                    datoreemplazar = oSolDatosDescripcion.Direccion;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Municipio}";
                    datoreemplazar = oSolicitante.Tbl_Gral_Municipio.Municipio;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Departamento}";
                    datoreemplazar = oSolicitante.Tbl_Gral_Departamento.Departamento;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Lista_EnmiendasTecnicas}";
                    datolistareemplazar = oEnmiendasOficio;
                    ReemplazaListaWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datolistareemplazar);

                    datobusqueda = "{Lista_EnmiendasJuridicas}";
                    datolistareemplazar = oObjecionesOficio;
                    ReemplazaListaWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datolistareemplazar);

                    datobusqueda = "{Nombre_Juridico}";
                    datoreemplazar = (oJuridicoAsignado.Nombre + " " + oJuridicoAsignado.Apellidos).Trim();
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{SubRegion_Juridico}";
                    datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    datobusqueda = "{Fecha_Hora}";
                    datoreemplazar = strswdatecreated;
                    ReemplazaPalabraWord(appWord: appWord, datobusqueda: datobusqueda, datoreemplazar: datoreemplazar);

                    docWord.Save();
                    //docWord.SaveAs2(destpdf, IOPWord.WdSaveFormat.wdFormatPDF);
                    docWord.Close(SaveChanges: true);
                    appWord.Quit(SaveChanges: true);

                }
                catch (Exception ex)
                {

                    docWord.Save();
                    //docWord.SaveAs2(destpdf, IOPWord.WdSaveFormat.wdFormatPDF);
                    docWord.Close(SaveChanges: true);
                    appWord.Quit(SaveChanges: true);

                }


                return docreturn;

            }
            else
            {
                return null;
            }

            return null;
        }

        [HttpPost]
        public JsonResult GrabarObjecion(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                         where d.Enmienda_id == model.Enmienda_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
                         select d).FirstOrDefault();

            if (oEnmienda != null)
            {
                oEnmienda.swcriticadoby = model.swcriticadoby;
                oEnmienda.swcriticadobyinterno = model.swcriticadobyinterno;
                oEnmienda.swdatecriticadoby = swdatecreated;
                oEnmienda.Objecion = model.Objecion;

                db.SaveChanges();
            }

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        class UbicacionArchivo
        {
            public string Ubicacion { get; set; }
        }

        public JsonResult ObtenerEnmiendaOficioJuridico(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            string guidsolicitud, guidetapasolicitud, ubicacion;
            guidsolicitud = model.Solicitud_Guid_id;
            guidetapasolicitud = model.EtapaSolicitud_GUID_id;
            ubicacion = GenerarEnmiendaOficioJuridico(guidsolicitud, guidetapasolicitud);
            UbicacionArchivo oUbi = new UbicacionArchivo();
            oUbi.Ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(oUbi));
        }








    }
}
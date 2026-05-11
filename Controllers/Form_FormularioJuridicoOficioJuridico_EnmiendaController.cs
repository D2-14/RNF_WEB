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
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class Form_FormularioJuridicoOficioJuridico_EnmiendaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: OficioJuridico_Enmienda
        public ActionResult Index(string EtapaSolicitudGuid, long Solicitudid)
        {
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitudid);

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.EtapaSolicitud_GUID_id == EtapaSolicitudGuid).First();

            string guidsolicitud = tbl_sol_solicitud.Guid_id;
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

           

            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where d.EtapaSolicitud_GUID_id == EtapaSolicitudGuid
                               select d).FirstOrDefault();




            Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda oEnmiendasOficioJuridico_Enmienda = new Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda();
            oEnmiendasOficioJuridico_Enmienda.Solicitud_Guid_id = oEtapaSolicitud.Solicitud_Guid_id;
            oEnmiendasOficioJuridico_Enmienda.EtapaSolicitud_GUID_id = oEtapaSolicitud.EtapaSolicitud_GUID_id;
            oEnmiendasOficioJuridico_Enmienda.Solicitud_id = oEtapaSolicitud.Solicitud_id;
            oEnmiendasOficioJuridico_Enmienda.Etapa_id = oEtapaSolicitud.Etapa_id;
            oEnmiendasOficioJuridico_Enmienda.EtapaRuta_id = oEtapaSolicitud.EtapaRuta_id;
            oEnmiendasOficioJuridico_Enmienda.CorrelativoEtapa_id = oEtapaSolicitud.CorrelativoEtapa_id;
            oEnmiendasOficioJuridico_Enmienda.swcreatedby = objUs.intUsuario_id;
            oEnmiendasOficioJuridico_Enmienda.swcreatedbyinterno = true;


            return View(oEnmiendasOficioJuridico_Enmienda);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult ListaEnmiendasOficioJuridico_Enmienda(string Guid_id, string EtapaSolicitudGuid)
        {
            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;

            List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda> oEtapaOficioJuridico_Enmienda = new List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda>();
            oEtapaOficioJuridico_Enmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                                   where d.Solicitud_Guid_id == guidsolicitud
                                   orderby d.Enmienda_id
                                   select d).ToList();

            return View(oEtapaOficioJuridico_Enmienda);
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
                    reemplazaenlist = contador + ". " + datoreemplazar[i] + Environment.NewLine;

                    appWord.Selection.Collapse();
                    appWord.Selection.Find.ClearFormatting();

                    if (i < (datoreemplazar.Count() - 1))
                    {
                        appWord.Selection.Find.Execute(FindText: datobusqueda, ReplaceWith: reemplazaenlist + datobusqueda, Replace: replaceAll);
                    }
                    else
                    {
                        appWord.Selection.Find.Execute(FindText: datobusqueda, ReplaceWith: reemplazaenlist, Replace: replaceAll);
                    }

                }

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

        string GenerarEnmiendaOficioJuridico_Enmienda(string Guid_id, string EtapaSolicitudGuid)
        {

            IOPWord.Application appWord;
            IOPWord.Document docWord;

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioJuridico_Enmienda.docx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioJuridico_Enmienda_{guidetapasol}";
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


            oEtapaSolicitud.NombreDocumentoNoFirmado = nombrereporte + ".pdf";



            db.Entry(oEtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            string nombredia, nombremes;
            nombredia = swdatecreated.ToString("dddd", CultureInfo.CreateSpecificCulture("es-MX"));
            nombremes = swdatecreated.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));

            strswdatecreated = swdatecreated.ToString("dd/MM/yyyy HH:mm");
            stronlydatecreated = $"{nombredia} {swdatecreated.ToString("dd")} de {nombremes} de {swdatecreated.ToString("yyyy")}";


            if (oEtapaSolicitud != null)
            {
                List<string> oEnmiendasOficio = new List<string>();
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

                        if ((oOficioJuridico_Enmienda[i].Descripcion.Trim() != null) && (oOficioJuridico_Enmienda[i].Descripcion.Trim() != ""))
                        {
                            oEnmiendasOficio.Add($"{oOficioJuridico_Enmienda[i].Descripcion.Trim()}");
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

                    datobusqueda = "{Solicitante}";
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

                    datobusqueda = "{Lista_Enmiendas}";
                    datolistareemplazar = oEnmiendasOficio;
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


                return nombrereporte + ".pdf";

            }
            else
            {
                return null;
            }

            
        }

        class UbicacionArchivo
        {
            public string Ubicacion { get; set; }
        }

        [HttpPost]
        public JsonResult GrabarEnmienda(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            DateTime swdatecreated;
            int contEnmienda;

            swdatecreated = DateTime.Now;


            //contEnmienda = 0;
            //lstContEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
            //                   where d.Solicitud_Guid_id == model.Solicitud_Guid_id
            //                   orderby d.Enmienda_id
            //                   select d).ToList();

            //if (lstContEnmienda.Count() > 0)
            //{
            //    for (int i = 0; i < lstContEnmienda.Count(); i++)
            //    {
            //        contEnmienda = lstContEnmienda[i].Enmienda_id;
            //    }
            //}


            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_Guid_id == model.Solicitud_Guid_id).Max(u => u.Enmienda_id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }

            contEnmienda = lngIdt;

            model.swdatecreated = swdatecreated;
            model.Enmienda_id = contEnmienda;
            model.Estado_id = true;

            db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Add(model);
            db.SaveChanges();

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        [HttpPost]
        public JsonResult ActualizaEstadoEnmienda(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                         where d.Enmienda_id == model.Enmienda_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
                         select d).FirstOrDefault();

            if (oEnmienda != null)
            {
                oEnmienda.swupdatedby = model.swupdatedby;
                oEnmienda.swupdatedbyinterno = true;
                oEnmienda.swdateupdated = swdatecreated;
                oEnmienda.Estado_id = model.Estado_id;

                db.SaveChanges();
            }

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        [HttpPost]
        public JsonResult EliminarEnmienda(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                         where d.Enmienda_id == model.Enmienda_id && d.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
                         select d).FirstOrDefault();

            if (oEnmienda != null)
            {
                db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Remove(oEnmienda);
                db.SaveChanges();
            }

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        public JsonResult ObtenerEnmiendaOficioJuridico_Enmienda(Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda model)
        {
            string guidsolicitud, guidetapasolicitud, ubicacion;
            guidsolicitud = model.Solicitud_Guid_id;
            guidetapasolicitud = model.EtapaSolicitud_GUID_id;
            ubicacion = GenerarEnmiendaOficioJuridico_Enmienda(guidsolicitud, guidetapasolicitud);
            UbicacionArchivo oUbi = new UbicacionArchivo();
            oUbi.Ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(oUbi));
        }
    }
}
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Image = iTextSharp.text.Image;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using System.Data.SqlClient;
using OfficeOpenXml.Drawing.Chart;
using System.Windows.Media;

using iTextSharp.text;
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
using System.Data.SqlClient;
using OfficeOpenXml;
using GPDF = GemBox.Spreadsheet;


namespace RNF_Web.Controllers
{
    public class Form_FormularioSubRegionalOficioEnmiendasController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        class UbicacionArchivo
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }



        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosNotificacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosFinca = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosPlantacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        private PdfPTable tablePersoneria = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);

        private iTextSharp.text.Font fntTituloTabla_MTS = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_MT = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_10 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_11 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_12 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_13 = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_14 = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.NORMAL);
        private iTextSharp.text.Font fntTituloTabla_15 = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.NORMAL);

        private iTextSharp.text.Font fntTituloTabla_10B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_11B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_12B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_13B = FontFactory.GetFont("HELVETICA", size: 10, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_14B = FontFactory.GetFont("HELVETICA", size: 11, iTextSharp.text.Font.BOLD);
        private iTextSharp.text.Font fntTituloTabla_15B = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);



        // GET: OficioTecnicoCritica
        public ActionResult Index(string Guid_id, string EtapaSolicitudGuid)
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

            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;
            ViewBag.guidsolicitud = guidsolicitud;
            ViewBag.guidetapasolicitud = guidetapasolicitud;

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == EtapaSolicitudGuid).First();

            return View(tbl_gest_EtapaSolicitud);
        }

     

        public ActionResult CreateEnmienda(string Guid_id, string EtapaSolicitudGuid)
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

            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;

            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where d.EtapaSolicitud_GUID_id == guidetapasolicitud && d.Solicitud_Guid_id == guidsolicitud
                               select d).FirstOrDefault();


            Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda oEnmiendaSubRegional = new Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda();
            oEnmiendaSubRegional.Solicitud_Guid_id = guidsolicitud;
            oEnmiendaSubRegional.EtapaSolicitud_GUID_id = guidetapasolicitud;
            oEnmiendaSubRegional.Solicitud_id = oEtapaSolicitud.Solicitud_id;
            oEnmiendaSubRegional.Etapa_id = oEtapaSolicitud.Etapa_id;
            oEnmiendaSubRegional.EtapaRuta_id = oEtapaSolicitud.EtapaRuta_id;
            oEnmiendaSubRegional.CorrelativoEtapa_id = oEtapaSolicitud.CorrelativoEtapa_id;
            oEnmiendaSubRegional.swcreatedby = objUs.intUsuario_id;
            oEnmiendaSubRegional.swcreatedbyinterno = true;


            return View(oEnmiendaSubRegional);
        }


        public ActionResult ListaEnmiendasOficioSubRegional(string Guid_id, string EtapaSolicitudGuid)
        {
            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;

            List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEtapaOficioTecnico = new List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda>();
            oEtapaOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                   where d.Solicitud_Guid_id == guidsolicitud && d.Estado_id == true
                                   orderby d.Enmienda_id
                                   select d).ToList();
            return View(oEtapaOficioTecnico);
        }

        //public ActionResult ListaCriticasOficioTecnico(string Guid_id, string EtapaSolicitudGuid)
        //{
        //    string guidsolicitud, guidetapasolicitud;
        //    guidsolicitud = Guid_id;
        //    guidetapasolicitud = EtapaSolicitudGuid;

        //    List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica> oEtapaOficioTecnico = new List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica>();
        //    oEtapaOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica
        //                           where d.EtapaSolicitud_Guid_id == guidetapasolicitud && d.Solicitud_Guid_id == guidsolicitud
        //                           orderby d.Critica_id
        //                           select d).ToList();
        //    return View(oEtapaOficioTecnico);
        //}

        public ActionResult ListaEnmiendasTecnico(string Guid_id, string EtapaSolicitudGuid)
        {
            List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oEnmiendas = new List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda>();
            oEnmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                          where d.Solicitud_Guid_id == Guid_id && d.Estado_id == true
                          select d).ToList();
            return View(oEnmiendas);
        }

        public ActionResult ListaEnmiendasJuridico(string Guid_id, string EtapaSolicitudGuid)
        {
            List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda> oEnmiendas = new List<Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda>();
            oEnmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda
                          where d.Solicitud_Guid_id == Guid_id && d.Estado_id == true
                          select d).ToList();
            return View(oEnmiendas);
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

        void ReemplazaListaWord(IOPWord.Application appWord, string datobusqueda, List<string> datoreemplazar, string datobusquedatitulo = null)
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
            else if (datoinsertar == "")
            {
                appWord.Selection.Collapse();
                appWord.Selection.Find.ClearFormatting();
                appWord.Selection.Find.Execute(FindText: datobusqueda, ReplaceWith: "", Replace: replaceAll);
                appWord.Selection.Collapse();
                appWord.Selection.Find.ClearFormatting();
                appWord.Selection.Find.Execute(FindText: datobusquedatitulo, ReplaceWith: "", Replace: replaceAll);
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
                return "";
            }
            else
            {
                return "Ok";
            }
        }


        class Personerias
        {
            public List<string> PropietariosIndividuales { get; set; }
            public List<string> PropietariosJuridicos { get; set; }
            public List<string> RepresentatnteLegal { get; set; }
            public List<string> Mandatario { get; set; }
            public List<string> ArrendatariosIndividuales { get; set; }
            public List<string> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias(long Solicitud_id)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<string>();
            personerias.PropietariosJuridicos = new List<string>();
            personerias.RepresentatnteLegal = new List<string>();
            personerias.Mandatario = new List<string>();
            personerias.ArrendatariosIndividuales = new List<string>();
            personerias.ArrendatariosJuridicos = new List<string>();
            List<fc_Sol_Sel_ListadoDePropietario_Result> PropietariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                     where d.Personeria_id == 1
                                                                                     select d).ToList();

            List<fc_Sol_Sel_ListadoDePropietario_Result> ProietariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDePropietario(Solicitud_id)
                                                                                 where d.Personeria_id == 2
                                                                                 select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> RepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false)
                                                                                    select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> Mandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true)
                                                                            select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosIndividuales = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                       where d.Personeria_id == 1
                                                                                       select d).ToList();

            List<fc_Sol_Sel_ListadoDeArrendatario_Result> ArrendatariosJuridicos = (from d in db.fc_Sol_Sel_ListadoDeArrendatario(Solicitud_id)
                                                                                    where d.Personeria_id == 2
                                                                                    select d).ToList();

            if (PropietariosIndividuales.Count() > 0)
            {
                foreach (var item in PropietariosIndividuales)
                {
                    personerias.PropietariosIndividuales.Add(item.Nombre);
                }
            }

            if (ProietariosJuridicos.Count() > 0)
            {
                foreach (var item in ProietariosJuridicos)
                {
                    personerias.PropietariosJuridicos.Add(item.Nombre);
                }
            }

            if (RepresentanteLegal.Count() > 0)
            {
                foreach (var item in RepresentanteLegal)
                {
                    personerias.RepresentatnteLegal.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (Mandatario.Count() > 0)
            {
                foreach (var item in Mandatario)
                {
                    personerias.Mandatario.Add(item.Nombres + " " + item.Apellidos);
                }
            }

            if (ArrendatariosIndividuales.Count() > 0)
            {
                foreach (var item in ArrendatariosIndividuales)
                {
                    personerias.ArrendatariosIndividuales.Add(item.Nombre);
                }
            }

            if (ArrendatariosJuridicos.Count() > 0)
            {
                foreach (var item in ArrendatariosJuridicos)
                {
                    personerias.ArrendatariosJuridicos.Add(item.Nombre);
                }
            }

            return personerias;
        }


        string GenerarEnmiendaOficioJuridico(string Guid_id, string EtapaSolicitudGuid)
        {

            IOPWord.Application appWord;
            IOPWord.Document docWord;
            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated, strvalidar;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto,
                nombresubregional;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioSubRegional.docx";
            machotexlsx = $"{rootpath}Machote_OficioSubRegional.xlsx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioSubRegional_{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destpdf = $"{destfinal}.pdf";
            destxlsx = $"{destfinal}.xlsx";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
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

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                                                                                       where d.Solicitud_Guid_id == guidid && d.Estado_id == true
                                                                                       select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                Personerias personerias = ObtenerPersonerias(oSolicitud.Solicitud_id);

                string[] paramsQuery = new string[]
                {
                    oSolicitud.Region_id.ToString(),
                    oSolicitud.SubRegion_id.ToString()
                };
                string NombreSubRegional = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_NombreSubDirectorRegional](@p0, @p1) ", paramsQuery).FirstOrDefault();

                if (oOficioTecnico.Count() > 0)
                {

                    oOficioTecnico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oOficioTecnico.Count(); i++)
                    {

                        if ((oOficioTecnico[i].Descripcion != null) && (oOficioTecnico[i].Descripcion != ""))
                        {
                            strvalidar = oOficioTecnico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasOficio.Add($"{oOficioTecnico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (System.IO.File.Exists(destdocx))
                {
                    System.IO.File.Delete(destdocx);
                }

                if (System.IO.File.Exists(destxlsx))
                {
                    System.IO.File.Delete(destxlsx);
                }

                if (System.IO.File.Exists(destpdf))
                {
                    System.IO.File.Delete(destpdf);
                }


                oEPP = new ExcelPackage(new FileInfo(machotexlsx));
                ExcelWorksheet wsheet1;
                wsheet1 = oEPP.Workbook.Worksheets[0];

                int startrow, startcol;

                Usuario objUs = new Usuario();
                objUs.intUsuario_id = 0;

                RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
                if (objSesion.getBlSession())
                {
                    objUs = (Usuario)Session["User"];
                }
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                //1	                Oficio Jurídico
                //2	                Oficio Técnico
                //3	                Oficio Sub-regional
                string No_OficioJuridico = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 1, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;


                datobusqueda = "{No_Oficio}";
                datoreemplazar = No_OficioJuridico;
                iniciofila = finalfila = 3;
                iniciocolumna = finalcolumna = 8;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Fecha}";
                datoreemplazar = stronlydatecreated;
                iniciofila = finalfila = 4;
                iniciocolumna = finalcolumna = 6;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 3])
                {
                    rango.Value = "Guatemala, " + datoreemplazar;
                }



                datobusqueda = "{Propietario_RepresentanteLegal}";
                datoreemplazar = oSolDatosDescripcion.Propietario;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 1;
                datolistareemplazar = personerias.PropietariosIndividuales;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Propietario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.PropietariosJuridicos;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Propietario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.RepresentatnteLegal;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Representante Legal:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.Mandatario;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Mandatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.ArrendatariosIndividuales;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Arrendatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.ArrendatariosJuridicos;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Arrendatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }

                iniciofila = iniciofila + 3;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion}";
                datoreemplazar = oSolicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + oSolicitud.Tbl_Gral_SubRegion.Nombre_SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Municipio}";
                datoreemplazar = oSolicitante.Tbl_Gral_Municipio.Municipio;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Departamento}";
                datoreemplazar = oSolicitante.Tbl_Gral_Departamento.Departamento;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }

                iniciofila = iniciofila + 5;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                if ((oSolicitud.No_Registro != null) && (oSolicitud.No_Registro.Trim() != ""))
                {
                    datoreemplazar += " con número de registro " + oSolicitud.No_Registro;
                }
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }

                iniciofila = iniciofila + 4;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasJuridicas}";
                datolistareemplazar = oObjecionesOficio;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmienda Jurídica";
                        rango.Style.Font.Bold = true;
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;



                    }


                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasTecnicas}";
                datolistareemplazar = oEnmiendasOficio;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmienda Técnica";
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = finalcolumna = 1;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;

                    }
                }
                else
                {
                }

                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = iniciofila + 10;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }



                datobusqueda = "{Fecha_Hora}";
                datoreemplazar = strswdatecreated;
                iniciofila = finalfila = iniciofila + 4;
                iniciocolumna = finalcolumna = 7;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                wsheet1.Protection.IsProtected = false;
                wsheet1.Protection.AllowSelectLockedCells = false;
                oEPP.SaveAs(new FileInfo(destxlsx));

                try
                {
                    GPDF.ExcelFile LibroExcel = GPDF.ExcelFile.Load(destxlsx);
                    GPDF.ExcelWorksheet sheet = LibroExcel.Worksheets[0];

                    var saveOptions = new GPDF.PdfSaveOptions();
                    saveOptions.SelectionType = GPDF.SelectionType.EntireFile;

                    LibroExcel.Save(destpdf, saveOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return docreturn;

            }
            else
            {
                return null;
            }
        }


        string GenerarEnmiendaOficioJuridico_Borrar(string Guid_id, string EtapaSolicitudGuid)
        {

            IOPWord.Application appWord;
            IOPWord.Document docWord;
            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated, strvalidar;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto,
                nombresubregional;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioSubRegional.docx";
            machotexlsx = $"{rootpath}Machote_OficioSubRegional.xlsx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioSubRegional_{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destpdf = $"{destfinal}.pdf";
            destxlsx = $"{destfinal}.xlsx";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
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

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                                                                                       where d.Solicitud_Guid_id == guidid && d.Estado_id == true
                                                                                       select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();


                string[] paramsQuery = new string[]
                {
                    oSolicitud.Region_id.ToString(),
                    oSolicitud.SubRegion_id.ToString()
                };
                string NombreSubRegional = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_NombreSubDirectorRegional](@p0, @p1) ", paramsQuery).FirstOrDefault();

                if (oOficioTecnico.Count() > 0)
                {

                    oOficioTecnico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oOficioTecnico.Count(); i++)
                    {

                        if ((oOficioTecnico[i].Descripcion != null) && (oOficioTecnico[i].Descripcion != ""))
                        {
                            strvalidar = oOficioTecnico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasOficio.Add($"{oOficioTecnico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (System.IO.File.Exists(destdocx))
                {
                    System.IO.File.Delete(destdocx);
                }

                if (System.IO.File.Exists(destxlsx))
                {
                    System.IO.File.Delete(destxlsx);
                }

                if (System.IO.File.Exists(destpdf))
                {
                    System.IO.File.Delete(destpdf);
                }


                oEPP = new ExcelPackage(new FileInfo(machotexlsx));
                ExcelWorksheet wsheet1;
                wsheet1 = oEPP.Workbook.Worksheets[0];

                int startrow, startcol;

                Usuario objUs = new Usuario();
                objUs.intUsuario_id = 0;

                RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
                if (objSesion.getBlSession())
                {
                    objUs = (Usuario)Session["User"];
                }
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                //1	                Oficio Jurídico
                //2	                Oficio Técnico
                //3	                Oficio Sub-regional
                string No_OficioJuridico = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 1, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;


                datobusqueda = "{No_Oficio}";
                datoreemplazar = No_OficioJuridico;
                iniciofila = finalfila = 3;
                iniciocolumna = finalcolumna = 8;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Fecha}";
                datoreemplazar = stronlydatecreated;
                iniciofila = finalfila = 4;
                iniciocolumna = finalcolumna = 6;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 3])
                {
                    rango.Value = "Guatemala, " + datoreemplazar;
                }


                datobusqueda = "{Propietario_RepresentanteLegal}";
                datoreemplazar = oSolDatosDescripcion.Propietario;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                iniciofila = finalfila = 11;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion}";
                datoreemplazar = oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Municipio}";
                datoreemplazar = oSolicitante.Tbl_Gral_Municipio.Municipio;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Departamento}";
                datoreemplazar = oSolicitante.Tbl_Gral_Departamento.Departamento;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }


                iniciofila = finalfila = 16;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }


                iniciofila = finalfila = 20;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasJuridicas}";
                datolistareemplazar = oObjecionesOficio;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmieda Jurídica";
                        rango.Style.Font.Bold = true;
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        wsheet1.Row(iniciofila).Height = 80;

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }

                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;


                    }


                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasTecnicas}";
                datolistareemplazar = oEnmiendasOficio;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmieda Técnica";
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = finalcolumna = 1;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                        }

                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = finalcolumna = 1;

                    }
                }
                else
                {
                }

                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = iniciofila + 10;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }



                datobusqueda = "{Fecha_Hora}";
                datoreemplazar = strswdatecreated;
                iniciofila = finalfila = iniciofila + 4;
                iniciocolumna = finalcolumna = 7;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                wsheet1.Protection.IsProtected = false;
                wsheet1.Protection.AllowSelectLockedCells = false;
                oEPP.SaveAs(new FileInfo(destxlsx));

                try
                {
                    GPDF.ExcelFile LibroExcel = GPDF.ExcelFile.Load(destxlsx);
                    GPDF.ExcelWorksheet sheet = LibroExcel.Worksheets[0];

                    var saveOptions = new GPDF.PdfSaveOptions();
                    saveOptions.SelectionType = GPDF.SelectionType.EntireFile;

                    LibroExcel.Save(destpdf, saveOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return docreturn;

            }
            else
            {
                return null;
            }
        }

        string GenerarEnmiendaOficioSubRegional_borrar(string Guid_id, string EtapaSolicitudGuid)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            IOPWord.Application appWord;
            IOPWord.Document docWord;
            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated, strvalidar;
            string rootbase, rootpath, partialdestpath, rootdest, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";

            machotexlsx = $"{rootpath}Machote_OficioSubRegional.xlsx";

            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where d.EtapaSolicitud_GUID_id == guidetapasol && d.Solicitud_Guid_id == guidid
                               select d).FirstOrDefault();

            int CantidadEnmiendasTecnico = db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            int CantidadEnmiendasJuridicas = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            int CantidadEnmiendasSubRegional = db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            if ((CantidadEnmiendasTecnico + CantidadEnmiendasJuridicas + CantidadEnmiendasSubRegional) == 0)
            {
                return "";
            }

            if ((oEtapaSolicitud.EtapaRuta_id == (decimal)8.00) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.01) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.02) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.03) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.04) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.05))
            {
                machotexlsx = $"{rootpath}Machote_OficioSubRegionalMotoSierra.xlsx";

            }


            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioSubRegional_{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destpdf = $"{destfinal}.pdf";
            destxlsx = $"{destfinal}.xlsx";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
            #endregion

            oEtapaSolicitud.NombreDocumentoNoFirmado = nombrereporte + ".pdf";

            db.Entry(oEtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            string strTipoDeSolicitud = db.Database.SqlQuery<string>("Select dbo.fnc_Sol_TipoDeGestionDesc(@p0) + ' ' + dbo.Fcn_Gral_SubCategoriaMailRSS(@p1)", oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.Solicitud_id).FirstOrDefault();

            string nombredia, nombremes;
            nombredia = swdatecreated.ToString("dddd", CultureInfo.CreateSpecificCulture("es-MX"));
            nombremes = swdatecreated.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));

            strswdatecreated = swdatecreated.ToString("dd/MM/yyyy HH:mm");
            stronlydatecreated = $"{nombredia} {swdatecreated.ToString("dd")} de {nombremes} de {swdatecreated.ToString("yyyy")}";


            if (oEtapaSolicitud != null)
            {
                List<string> oEnmiendasTecnico = new List<string>();
                List<string> oEnmiendasJuridico = new List<string>();
                List<string> oEnmiendasSubRegional = new List<string>();
                List<string> datolistareemplazar = new List<string>();
                Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                                where d.Guid_id == guidid
                                                select d).FirstOrDefault();

                Tbl_Seg_UsuarioExterno oSolicitante = (from d in db.Tbl_Seg_UsuarioExterno
                                                       where d.Usuario_id == oSolicitud.swcreatedby
                                                       select d).FirstOrDefault();

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_Tecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                               where  d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaTecnico_id == true
                                                                                               select d).ToList();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_Juridico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                                where  d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaJuridico_id == true
                                                                                                select d).ToList();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_SubRegional = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                                   where d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaSubRegional_id == true
                                                                                                   select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                string[] paramsQuery = new string[]
                {
                    oSolicitud.Region_id.ToString(),
                    oSolicitud.SubRegion_id.ToString()
                };
                string NombreSubRegional = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_NombreSubDirectorRegional](@p0, @p1) ", paramsQuery).FirstOrDefault();

                if (oEnmiendas_Tecnico.Count() > 0)
                {

                    oEnmiendas_Tecnico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_Tecnico.Count(); i++)
                    {

                        if ((oEnmiendas_Tecnico[i].Descripcion != null) && (oEnmiendas_Tecnico[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_Tecnico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasTecnico.Add($"{oEnmiendas_Tecnico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                //string No_OficioSubRegional = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 3, Solicitud_id: oEtapaSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, CorrelativoEtapa_id: oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;
                Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 3, Solicitud_id: oEtapaSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, CorrelativoEtapa_id: oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);


                if (oEnmiendas_Juridico.Count() > 0)
                {

                    oEnmiendas_Juridico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_Juridico.Count(); i++)
                    {

                        if ((oEnmiendas_Juridico[i].Descripcion != null) && (oEnmiendas_Juridico[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_Juridico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasJuridico.Add($"{oEnmiendas_Juridico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (oEnmiendas_SubRegional.Count() > 0)
                {

                    oEnmiendas_SubRegional.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_SubRegional.Count(); i++)
                    {

                        if ((oEnmiendas_SubRegional[i].Descripcion != null) && (oEnmiendas_SubRegional[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_SubRegional[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasSubRegional.Add($"{oEnmiendas_SubRegional[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (System.IO.File.Exists(destdocx))
                {
                    System.IO.File.Delete(destdocx);
                }

                if (System.IO.File.Exists(destxlsx))
                {
                    System.IO.File.Delete(destxlsx);
                }

                if (System.IO.File.Exists(destpdf))
                {
                    System.IO.File.Delete(destpdf);
                }


                oEPP = new ExcelPackage(new FileInfo(machotexlsx));
                ExcelWorksheet wsheet1;
                wsheet1 = oEPP.Workbook.Worksheets[0];

                int startrow, startcol;

                datobusqueda = "{No_Oficio}";
                datoreemplazar = resultsp.Codigo; //oEtapaSolicitud.TecnicoOficio;
                iniciofila = finalfila = 3;
                iniciocolumna = finalcolumna = 8;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Fecha}";
                datoreemplazar = stronlydatecreated;
                iniciofila = finalfila = 4;
                iniciocolumna = finalcolumna = 6;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 3])
                {
                    rango.Value = "Guatemala, " + datoreemplazar;
                }


                datobusqueda = "{Propietario_RepresentanteLegal}";
                datoreemplazar = oSolDatosDescripcion.Propietario;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                iniciofila = finalfila = 11;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion}";
                datoreemplazar = oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Municipio}";
                datoreemplazar = oSolicitante.Tbl_Gral_Municipio.Municipio;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Departamento}";
                datoreemplazar = oSolicitante.Tbl_Gral_Departamento.Departamento;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }


                iniciofila = finalfila = 16;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                datobusqueda = "{TipoDeSolicitud}";
                datoreemplazar = strTipoDeSolicitud;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }


                iniciofila = finalfila = 20;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasJuridicas}";
                datolistareemplazar = oEnmiendasJuridico;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmieda Jurídica";
                        rango.Style.Font.Bold = true;
                    }


                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;


                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;


                    }


                    //iniciofila = finalfila = iniciofila + 1;
                    //iniciocolumna = finalcolumna = 1;

                    //for (int i = 0; i < datolistareemplazar.Count(); i++)
                    //{

                    //    contador += 1;
                    //    valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                    //    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    //    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    //    {
                    //        rango.Value = valornuevo;
                    //    }

                    //    iniciofila = finalfila = iniciofila + 1;
                    //    iniciocolumna = finalcolumna = 1;

                    //}
                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasTecnicas}";
                datolistareemplazar = oEnmiendasTecnico;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmieda Técnica";
                        rango.Style.Font.Bold = true;
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;


                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;


                    }

                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasSubRegional}";
                datolistareemplazar = oEnmiendasSubRegional;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmieda SubRegional";
                        rango.Style.Font.Bold = true;
                    }


                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        wsheet1.Row(iniciofila).Height = 80;

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }

                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                    }


                }
                else
                {
                }


                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = iniciofila + 10;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }




                datobusqueda = "{Fecha_Hora}";
                datoreemplazar = strswdatecreated;
                iniciofila = finalfila = iniciofila + 4;
                iniciocolumna = finalcolumna = 7;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                wsheet1.Protection.IsProtected = false;
                wsheet1.Protection.AllowSelectLockedCells = false;
                oEPP.SaveAs(new FileInfo(destxlsx));

                try
                {
                    GPDF.ExcelFile LibroExcel = GPDF.ExcelFile.Load(destxlsx);
                    GPDF.ExcelWorksheet sheet = LibroExcel.Worksheets[0];

                    var saveOptions = new GPDF.PdfSaveOptions();
                    saveOptions.SelectionType = GPDF.SelectionType.EntireFile;

                    LibroExcel.Save(destpdf, saveOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return nombrereporte + ".pdf";

            }
            else
            {
                return null;
            }
        }

        string GenerarEnmiendaOficioSubRegional_PDF(string Guid_id, string EtapaSolicitudGuid)
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


            IOPWord.Application appWord;
            IOPWord.Document docWord;

            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated, strvalidar;
            string rootbase, rootpath, partialdestpath, rootdest, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";

            machotexlsx = $"{rootpath}Machote_OficioSubRegional.xlsx";

            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();


            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where d.EtapaSolicitud_GUID_id == guidetapasol && d.Solicitud_Guid_id == guidid
                               select d).FirstOrDefault();

            int CantidadEnmiendasTecnico = db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            int CantidadEnmiendasJuridicas = db.Tbl_Gest_EtapaSolicitud_OficioJuridico_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            int CantidadEnmiendasSubRegional = db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.Where(Obj => Obj.Solicitud_id == oEtapaSolicitud.Solicitud_id && Obj.Estado_id == true).Count();

            if ((CantidadEnmiendasTecnico + CantidadEnmiendasJuridicas + CantidadEnmiendasSubRegional) == 0)
            {
                return "";
            }

            if ((oEtapaSolicitud.EtapaRuta_id == (decimal)8.00) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.01) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.02) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.03) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.04) || (oEtapaSolicitud.EtapaRuta_id == (decimal)8.05))
            {
                machotexlsx = $"{rootpath}Machote_OficioSubRegionalMotoSierra.xlsx";

            }


            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"OficioSubRegional_{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destpdf = $"{destfinal}.pdf";
            destxlsx = $"{destfinal}.xlsx";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
            #endregion

            oEtapaSolicitud.NombreDocumentoNoFirmado = nombrereporte + ".pdf";

            db.Entry(oEtapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            string strTipoDeSolicitud = db.Database.SqlQuery<string>("Select dbo.fnc_Sol_TipoDeGestionDesc(@p0) + ' ' + dbo.Fcn_Gral_SubCategoriaMailRSS(@p1)", oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.Solicitud_id).FirstOrDefault();

            string nombredia, nombremes;
            nombredia = swdatecreated.ToString("dddd", CultureInfo.CreateSpecificCulture("es-MX"));
            nombremes = swdatecreated.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));

            strswdatecreated = swdatecreated.ToString("dd/MM/yyyy HH:mm");
            stronlydatecreated = $"{nombredia} {swdatecreated.ToString("dd")} de {nombremes} de {swdatecreated.ToString("yyyy")}";

            stronlydatecreated = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaSolicitudTxt(getdate(), @p0)", oEtapaSolicitud.Solicitud_id).FirstOrDefault();



            if (oEtapaSolicitud != null)
            {
                List<string> oEnmiendasTecnico = new List<string>();
                List<string> oEnmiendasJuridico = new List<string>();
                List<string> oEnmiendasSubRegional = new List<string>();
                List<string> datolistareemplazar = new List<string>();
                Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                                where d.Guid_id == guidid
                                                select d).FirstOrDefault();

                Tbl_Gral_SubRegion tbl_Gral_SubRegion = (from d in db.Tbl_Gral_SubRegion
                                                         where d.Region_id == oSolicitud.Region_id
                                                            && d.SubRegion_id == oSolicitud.SubRegion_id
                                                         select d).FirstOrDefault();

                Tbl_Seg_UsuarioExterno oSolicitante = (from d in db.Tbl_Seg_UsuarioExterno
                                                       where d.Usuario_id == oSolicitud.swcreatedby
                                                       select d).FirstOrDefault();

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_Tecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                               where d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaTecnico_id == true
                                                                                               select d).ToList();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_Juridico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                                where d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaJuridico_id == true
                                                                                                select d).ToList();

                List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> oEnmiendas_SubRegional = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                                                                                   where d.Solicitud_Guid_id == guidid && d.Estado_id == true && d.EstadoEnmiendaSubRegional_id == true
                                                                                                   select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                CrearBanner crearBanner = new CrearBanner();
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();             
                Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 3, Solicitud_id: oEtapaSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, CorrelativoEtapa_id: oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
                



                Personerias personerias = ObtenerPersonerias(oSolicitud.Solicitud_id);

                string[] paramsQuery = new string[]
                {
                    oSolicitud.Region_id.ToString(),
                    oSolicitud.SubRegion_id.ToString()
                };

                string NombreSubRegional = db.Database.SqlQuery<string>("select [dbo].[Fnc_Gral_NombreSubDirectorRegional](@p0, @p1) ", paramsQuery).FirstOrDefault();

                if (oEnmiendas_Tecnico.Count() > 0)
                {

                    oEnmiendas_Tecnico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_Tecnico.Count(); i++)
                    {

                        if ((oEnmiendas_Tecnico[i].Descripcion != null) && (oEnmiendas_Tecnico[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_Tecnico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasTecnico.Add($"{oEnmiendas_Tecnico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }

                if (oEnmiendas_Juridico.Count() > 0)
                {

                    oEnmiendas_Juridico.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_Juridico.Count(); i++)
                    {

                        if ((oEnmiendas_Juridico[i].Descripcion != null) && (oEnmiendas_Juridico[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_Juridico[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasJuridico.Add($"{oEnmiendas_Juridico[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (oEnmiendas_SubRegional.Count() > 0)
                {

                    oEnmiendas_SubRegional.OrderBy(Obj => Obj.Enmienda_id);

                    for (int i = 0; i < oEnmiendas_SubRegional.Count(); i++)
                    {

                        if ((oEnmiendas_SubRegional[i].Descripcion != null) && (oEnmiendas_SubRegional[i].Descripcion != ""))
                        {
                            strvalidar = oEnmiendas_SubRegional[i].Descripcion;
                            if (strvalidar.Trim() != "")
                            {
                                oEnmiendasSubRegional.Add($"{oEnmiendas_SubRegional[i].Descripcion.Trim()}");
                            }
                        }
                    }

                }


                if (System.IO.File.Exists(destdocx))
                {
                    System.IO.File.Delete(destdocx);
                }

                if (System.IO.File.Exists(destxlsx))
                {
                    System.IO.File.Delete(destxlsx);
                }

                if (System.IO.File.Exists(destpdf))
                {
                    System.IO.File.Delete(destpdf);
                }



                oEPP = new ExcelPackage(new FileInfo(machotexlsx));
                ExcelWorksheet wsheet1;
                wsheet1 = oEPP.Workbook.Worksheets[0];

                int startrow, startcol;


                datobusqueda = "{Codigo}";
                datoreemplazar = resultsp.Codigo; //oEtapaSolicitud.TecnicoOficio;
                iniciofila = finalfila = 2;
                iniciocolumna = finalcolumna = 10;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }
                

                datobusqueda = "{Version}";
                datoreemplazar = resultsp.Version; //oEtapaSolicitud.TecnicoOficio;
                iniciofila = finalfila = 3;
                iniciocolumna = finalcolumna = 10;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }
                

                datobusqueda = "{FechaImplementacion}";
                datoreemplazar = resultsp.strFecha; //oEtapaSolicitud.TecnicoOficio;
                iniciofila = finalfila = 4;
                iniciocolumna = finalcolumna = 10;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }


                datobusqueda = "{No_Oficio}";
                datoreemplazar = resultsp.Identificador; //oEtapaSolicitud.TecnicoOficio;
                iniciofila = finalfila = 6;
                iniciocolumna = finalcolumna = 8;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Fecha}";
                datoreemplazar = stronlydatecreated;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 7;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna + 3])
                {
                    rango.Value = datoreemplazar;
                }


                datobusqueda = "{Propietario_RepresentanteLegal}";
                datoreemplazar = oSolDatosDescripcion.Propietario;
                iniciofila = finalfila = 9;
                iniciocolumna = finalcolumna = 1;
                datolistareemplazar = personerias.PropietariosIndividuales;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Propietario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.PropietariosJuridicos;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Propietario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.RepresentatnteLegal;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Representante Legal:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.Mandatario;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Mandatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.ArrendatariosIndividuales;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Arrendatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }
                datolistareemplazar = personerias.ArrendatariosJuridicos;
                if (datolistareemplazar.Count() > 0)
                {
                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Arrendatario:";
                        rango.Style.WrapText = true;
                        rango.Merge = true;
                        rango.Style.Font.Bold = true;
                        rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    iniciofila++;
                    iniciofila = finalfila = iniciofila;
                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        valornuevo = "- " + datolistareemplazar[i];

                        using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                        {
                            rango.Value = valornuevo;
                            rango.Style.WrapText = true;
                            rango.Merge = true;
                            rango.AutoFitColumns();
                            rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }


                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;
                        wsheet1.InsertRow(rowFrom: iniciofila, 1);


                    }
                }

                iniciofila = iniciofila + 3;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion}";
                datoreemplazar = oSolicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + oSolicitud.Tbl_Gral_SubRegion.Nombre_SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Municipio}";
                datoreemplazar = (tbl_Gral_SubRegion.Tbl_Gral_Municipio.Municipio ?? "");// oSolicitante.Tbl_Gral_Municipio.Municipio;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Departamento}";
                datoreemplazar = (tbl_Gral_SubRegion.Tbl_Gral_Departamento.Departamento ?? "");// oSolicitante.Tbl_Gral_Departamento.Departamento;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }

                iniciofila = iniciofila + 3;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                if ((oSolicitud.No_Registro != null) && (oSolicitud.No_Registro.Trim() != ""))
                {
                    datoreemplazar += " con número de registro " + oSolicitud.No_Registro;
                }
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                datobusqueda = "{TipoDeSolicitud}";
                datoreemplazar = strTipoDeSolicitud;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }

                iniciofila = iniciofila + 4;

                iniciofila = finalfila = iniciofila;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasJuridicas}";
                datolistareemplazar = oEnmiendasJuridico;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmienda Jurídica";
                        rango.Style.Font.Bold = true;
                    }


                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;


                    for (int i = 0;                                                                                                                                       i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;


                    }


                    //iniciofila = finalfila = iniciofila + 1;
                    //iniciocolumna = finalcolumna = 1;

                    //for (int i = 0; i < datolistareemplazar.Count(); i++)
                    //{

                    //    contador += 1;
                    //    valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                    //    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    //    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    //    {
                    //        rango.Value = valornuevo;
                    //    }

                    //    iniciofila = finalfila = iniciofila + 1;
                    //    iniciocolumna = finalcolumna = 1;

                    //}
                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasTecnicas}";
                datolistareemplazar = oEnmiendasTecnico;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmienda Técnica";
                        rango.Style.Font.Bold = true;
                    }

                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;


                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;


                    }

                }
                else
                {
                }


                wsheet1.InsertRow(rowFrom: iniciofila, 1);
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;

                datobusqueda = "{Lista_EnmiendasSubRegional}";
                datolistareemplazar = oEnmiendasSubRegional;
                if (datolistareemplazar.Count() > 0)
                {

                    wsheet1.InsertRow(rowFrom: iniciofila, 1);
                    using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                    {
                        rango.Value = "Enmienda SubRegional";
                        rango.Style.Font.Bold = true;
                    }


                    iniciofila = finalfila = iniciofila + 1;
                    iniciocolumna = 1;
                    finalcolumna = iniciocolumna + 8;

                    for (int i = 0; i < datolistareemplazar.Count(); i++)
                    {

                        contador += 1;
                        valornuevo = "    " + contador + ". " + datolistareemplazar[i] + Environment.NewLine;

                        wsheet1.InsertRow(rowFrom: iniciofila, 1);
                        AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, valornuevo);
                        iniciofila = finalfila = iniciofila + 1;
                        iniciocolumna = 1;
                        finalcolumna = iniciocolumna + 8;

                    }


                }
                else
                {
                }


                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = iniciofila + 9;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{SubRegion_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = obtenertexto;
                }




                datobusqueda = "{Fecha_Hora}";
                datoreemplazar = strswdatecreated;
                iniciofila = finalfila = iniciofila + 3;
                iniciocolumna = finalcolumna = 9;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                wsheet1.Protection.IsProtected = false;
                wsheet1.Protection.AllowSelectLockedCells = false;
                oEPP.SaveAs(new FileInfo(destxlsx));

                try
                {
                    GPDF.ExcelFile LibroExcel = GPDF.ExcelFile.Load(destxlsx);
                    GPDF.ExcelWorksheet sheet = LibroExcel.Worksheets[0];

                    var saveOptions = new GPDF.PdfSaveOptions();
                    saveOptions.SelectionType = GPDF.SelectionType.EntireFile;

                    LibroExcel.Save(destpdf, saveOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return nombrereporte + ".pdf";

            }
            else
            {
                return null;
            }
        }


        void AgregarTextoDinamicoCeldasCombinadas(ExcelWorksheet wsheet, int iniciofila, int iniciocolumna, int finalfila, int finalcolumna, string valornuevo)
        {
            int altoInicial = 20;

            double altofinal = 0;

            altofinal = valornuevo.Length / 100;
            altofinal = Math.Round(altofinal);
            altofinal = altofinal * 20;
            altofinal += altoInicial;

            wsheet.Row(iniciofila).Height = altofinal;
            using (ExcelRange rango = wsheet.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
            {
                rango.Value = valornuevo;
                rango.Style.WrapText = true;
                rango.Merge = true;
                rango.AutoFitColumns();
                rango.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                rango.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }

        }

        [HttpPost]
        public JsonResult GrabarObjecion(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                         where d.Enmienda_id == model.Enmienda_id && d.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
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

        public JsonResult ObtenerEnmiendaOficioJuridico(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            string guidsolicitud, guidetapasolicitud, ubicacion;
            guidsolicitud = model.Solicitud_Guid_id;
            guidetapasolicitud = model.EtapaSolicitud_GUID_id;
            ubicacion = GenerarEnmiendaOficioJuridico(guidsolicitud, guidetapasolicitud);
            UbicacionArchivo oUbi = new UbicacionArchivo();
            oUbi.Ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(oUbi));
        }

        public JsonResult GenerarEnmiendaOficioSubRegional(Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda model)
        {
            string guidsolicitud, guidetapasolicitud, ubicacion;
            guidsolicitud = model.Solicitud_Guid_id;
            guidetapasolicitud = model.EtapaSolicitud_GUID_id;
            UbicacionArchivo oUbi = new UbicacionArchivo();


            int Cant_oEnmiendas_Tecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                           where d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.Estado_id == true && d.EstadoEnmiendaTecnico_id == true
                                           select d).Count();

            int Cant_Enmiendas_Juridico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                           where d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.Estado_id == true && d.EstadoEnmiendaJuridico_id == true
                                           select d).Count();

            int Cant_oEnmiendas_SubRegional = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                               where d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.Estado_id == true && d.EstadoEnmiendaSubRegional_id == true
                                               select d).Count();

          
            if ((Cant_oEnmiendas_Tecnico == 0) && (Cant_Enmiendas_Juridico==0) && (Cant_oEnmiendas_SubRegional==0))
            {
                oUbi.Mensaje = "Debe ingresar las enmiendas para poder proceder.\nPuede ingresarlas manual o presionando el boton agregar, en cada enmienda manifiesta.";
                oUbi.Result = 2;
            }
            else
            {
                ubicacion = GenerarEnmiendaOficioSubRegional_PDF(guidsolicitud, guidetapasolicitud);
                if(ubicacion != null)
                {
                    oUbi.Result = 1;
                    oUbi.Mensaje = "Documento generado exitósamente";
                }
                else
                {
                    oUbi.Result = 3;
                    oUbi.Mensaje = "Ocurrió un error";

                }

                oUbi.Ubicacion = ubicacion;
            }

            return Json(JsonConvert.SerializeObject(oUbi));
        }

        [HttpPost]
        public JsonResult ActualizaEstadoEnmienda(Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda model)
        {
            Usuario objUs = new Usuario();
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

            bool esinterno = true;

            if (objUs.EsInterno != 1)
            {
                esinterno = false;
            }

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                         where d.Solicitud_Guid_id == model.Solicitud_Guid_id
                         && d.CorrelativoEtapa_id == model.CorrelativoEtapa_id
                         && d.Enmienda_id == model.Enmienda_id
                         select d).FirstOrDefault();

            if (oEnmienda != null)
            {
                try
                {
                    oEnmienda.swupdatedby = model.swupdatedby;
                    oEnmienda.swupdatedbyinterno = esinterno;
                    oEnmienda.swdateupdated = swdatecreated;
                    oEnmienda.Estado_id = false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

                db.SaveChanges();
            }

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        public JsonResult GrabarEnmienda(Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda model)
        {
            Usuario objUs = new Usuario();
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

            bool esinterno = true;

            if (objUs.EsInterno != 1)
            {
                esinterno = false;
            }

            DateTime swdatecreated;
            int contEnmienda;

            swdatecreated = DateTime.Now;
            contEnmienda = 0;

            try
            {
                contEnmienda = db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                    .Where(Obj =>
                                    Obj.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id
                                && Obj.Solicitud_Guid_id == model.Solicitud_Guid_id)
                                    .Max(Obj => Obj.Enmienda_id);
            }
            catch (Exception ex)
            {
                contEnmienda = 0;
            }

            contEnmienda += 1;

            model.swcreatedby = objUs.intUsuario_id;
            model.swcreatedbyinterno = esinterno;
            model.swdatecreated = swdatecreated;
            model.Enmienda_id = contEnmienda;
            model.Estado_id = true;
            db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.Add(model);
            db.SaveChanges();



            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        //public JsonResult GrabarCritica(Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica model)
        //{
        //    Usuario objUs = new Usuario();
        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
        //        ViewBag.Mensaje = objSesion.getStrMensaje();
        //        return Json(null);
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];
        //    }

        //    bool esinterno = true;

        //    if (objUs.EsInterno != 1)
        //    {
        //        esinterno = false;
        //    }

        //    DateTime swdatecreated;
        //    int contCritica;

        //    swdatecreated = DateTime.Now;
        //    contCritica = 0;

        //    try
        //    {
        //        contCritica = db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica
        //                            .Where(Obj =>
        //                            Obj.EtapaSolicitud_Guid_id == model.EtapaSolicitud_Guid_id
        //                        && Obj.Solicitud_Guid_id == model.Solicitud_Guid_id)
        //                            .Max(Obj => Obj.Critica_id);
        //    }
        //    catch (Exception ex)
        //    {
        //        contCritica = 0;
        //    }

        //    contCritica += 1;

        //    model.swcreatedby = objUs.intUsuario_id;
        //    model.swcreatedbyinterno = esinterno;
        //    model.swdatecreated = swdatecreated;
        //    model.Critica_id = contCritica;

        //    db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica.Add(model);
        //    db.SaveChanges();



        //    string TextoMostrar;
        //    TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

        //    return Json(TextoMostrar);
        //}

        //public JsonResult EliminarCritica(Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica model)
        //{
        //    Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica oCritica = new Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica();
        //    oCritica = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica
        //                where d.EtapaSolicitud_Guid_id == model.EtapaSolicitud_Guid_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.Critica_id == model.Critica_id
        //                select d).FirstOrDefault();

        //    if(oCritica != null)
        //    {
        //        db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Critica.Remove(oCritica);
        //        db.SaveChanges();
        //    }

        //    string TextoMostrar;
        //    TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

        //    return Json(TextoMostrar);
        //}

        public JsonResult AgregarEnmiendaSubRegional(Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda model)
        {
            Usuario objUs = new Usuario();
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

            bool esinterno = true;

            if(objUs.EsInterno != 1)
            {
                esinterno = false;
            }

            int contEmienda;

            contEmienda = 0;

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud oEtapa = new Tbl_Gest_EtapaSolicitud();
            oEtapa = (from d in db.Tbl_Gest_EtapaSolicitud
                      where d.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
                      select d).FirstOrDefault();

            try
            {
                contEmienda = db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                                    .Where(Obj =>
                                    Obj.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id
                                && Obj.Solicitud_Guid_id == model.Solicitud_Guid_id)
                                    .Max(Obj => Obj.Enmienda_id);
            }
            catch (Exception ex)
            {
                contEmienda = 0;
            }

            contEmienda += 1;

            Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda();
            oEnmienda.EtapaSolicitud_GUID_id = oEtapa.EtapaSolicitud_GUID_id;
            oEnmienda.Solicitud_Guid_id = oEtapa.Solicitud_Guid_id;
            oEnmienda.Solicitud_id = oEtapa.Solicitud_id;
            oEnmienda.Etapa_id = oEtapa.Etapa_id;
            oEnmienda.EtapaRuta_id = oEtapa.EtapaRuta_id;
            oEnmienda.CorrelativoEtapa_id = oEtapa.CorrelativoEtapa_id;
            oEnmienda.Enmienda_id = contEmienda;
            oEnmienda.Descripcion = model.Descripcion;
            oEnmienda.Estado_id = true;
            oEnmienda.EstadoEnmiendaJuridico_id = model.EstadoEnmiendaJuridico_id;
            oEnmienda.EstadoEnmiendaTecnico_id = model.EstadoEnmiendaTecnico_id;
            oEnmienda.swdatecreated = swdatecreated;
            oEnmienda.swcreatedby = objUs.intUsuario_id;
            oEnmienda.swcreatedbyinterno = esinterno;

            db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda.Add(oEnmienda);
            db.SaveChanges();

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }


        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaDicotomica
        (
            long solicitud_id,
            int etapa_id,
            decimal etaparuta_id,
            int correlativoetapa_id,
            string motivo,
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


            if (ConfirmarRespuestaDicotomica(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo) == 1)
            {

                codRespuesta = 1;
                strRespuesta = "Se ha notificado la respuesta.";

            }
            else
            {
                codRespuesta = 0;
                strRespuesta = "Error: No se ha logrado notificar la respuesta.";
            }

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);

        }


        public int ConfirmarRespuestaDicotomica(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo)
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

            sqlQuery = "Exec SP_Gest_EtapaRespuestaSubRegionalDicotomicaEnmienda @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @swupdatedby, @swupdatedbyinterno	";

            sqlParams = new SqlParameter[]
                {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            return resultado[0].respuesta;

        }



        public ActionResult EnmiendasSubRegionalSolicitud(long Solicitud_id, string firma )
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda> listaPendientes = new List<Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda>();
            listaPendientes = (from d in db.Tbl_Gest_EtapaSolicitud_OficioSubRegional_Enmienda
                               where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                               orderby d.Enmienda_id
                               select d).ToList();

            return View(listaPendientes);
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
                strRespuesta = "Actualización realizada.";

                jsonResult = "{\"CodRespuesta\":"
                + "\"" + codRespuesta + "\","
                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);


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


        private void LlenaTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 11);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 11);

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

            c1 = new PdfPCell(new Phrase("\nCERTIFICADO DE INSCRIPCION DE " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("RF-RE-051", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Noviembre 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\n PROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }

    }
}
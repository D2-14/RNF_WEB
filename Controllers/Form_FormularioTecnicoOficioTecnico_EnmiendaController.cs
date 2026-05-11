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
using OfficeOpenXml;
using GPDF = GemBox.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;


namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoOficioTecnico_EnmiendaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();


        private PdfPTable tableBanner = new PdfPTable(1);
        int Al_Izquierda = Element.ALIGN_LEFT;
        int Al_Centro = Element.ALIGN_CENTER;
        int Al_Derecha = Element.ALIGN_RIGHT;
        int Al_Justificado = Element.ALIGN_JUSTIFIED;
        int Al_Arriba = Element.ALIGN_TOP;
        int Al_Abajo = Element.ALIGN_BOTTOM;
        int Al_Medio = Element.ALIGN_MIDDLE;
        int Al_JustificadoTodo = Element.ALIGN_JUSTIFIED_ALL;
        int Al_NoDefinido = Element.ALIGN_UNDEFINED;
        iTextSharp.text.BaseColor GrisClaro = iTextSharp.text.BaseColor.LIGHT_GRAY;
        iTextSharp.text.BaseColor Blanco = iTextSharp.text.BaseColor.WHITE;

        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
        }

        private void LlenaBanner(String Leyenda, string Color, int alineacionhorizontal = 0, int alineacionvertical = 5, int borde = 0, string estilotexto = "Normal")
        {

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(0, 0, 0);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);
            Font fntTituloBold = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            Font estiloFont = FontFactory.GetFont("HELVETICA", size: 10, FontColour);
            if (estilotexto == "Normal")
            {
                estiloFont = fntTituloTabla;
            }
            else if (estilotexto == "Bold")
            {
                estiloFont = fntTituloBold;
            }

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, estiloFont));

            c1.Border = borde;

            if (Color == "Blanco")
            {
                c1.BackgroundColor = Blanco;
            }
            else if (Color == "GrisClaro")
            {
                c1.BackgroundColor = GrisClaro;
            }
            else
            {

            }

            c1.HorizontalAlignment = alineacionhorizontal;

            c1.VerticalAlignment = alineacionvertical;

            tableBanner.AddCell(c1);

            return;
        }


        // GET: OficioTecnico_Enmienda
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




            Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda oEnmiendasOficioTecnico_Enmienda = new Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda();
            oEnmiendasOficioTecnico_Enmienda.Solicitud_Guid_id = guidsolicitud;
            oEnmiendasOficioTecnico_Enmienda.EtapaSolicitud_GUID_id = guidetapasolicitud;
            oEnmiendasOficioTecnico_Enmienda.Solicitud_id = oEtapaSolicitud.Solicitud_id;
            oEnmiendasOficioTecnico_Enmienda.Etapa_id = oEtapaSolicitud.Etapa_id;
            oEnmiendasOficioTecnico_Enmienda.EtapaRuta_id = oEtapaSolicitud.EtapaRuta_id;
            oEnmiendasOficioTecnico_Enmienda.CorrelativoEtapa_id = oEtapaSolicitud.CorrelativoEtapa_id;
            oEnmiendasOficioTecnico_Enmienda.swcreatedby = objUs.intUsuario_id;
            oEnmiendasOficioTecnico_Enmienda.swcreatedbyinterno = true;


            return View(oEnmiendasOficioTecnico_Enmienda);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult ListaEnmiendasOficioTecnico_Enmienda(string Guid_id, string EtapaSolicitudGuid)
        {
            string guidsolicitud, guidetapasolicitud;
            guidsolicitud = Guid_id;
            guidetapasolicitud = EtapaSolicitudGuid;

            List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oEtapaOficioTecnico_Enmienda = new List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda>();
            oEtapaOficioTecnico_Enmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                                   where d.Solicitud_Guid_id == guidsolicitud
                                   orderby d.Enmienda_id
                                   select d).ToList();

            return View(oEtapaOficioTecnico_Enmienda);
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

        string GenerarEnmiendaOficioTecnicoborrar(string Guid_id, string EtapaSolicitudGuid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (objSesion.getBlSession())
            {
                objUs = (Usuario)Session["User"];
            }

            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioTecnico.docx";
            machotexlsx = $"{rootpath}Machote_OficioTecnico.xlsx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destxlsx = $"{destfinal}.xlsx";
            destpdf = $"{destfinal}.pdf";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
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

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                                                                                       where d.Solicitud_Guid_id == guidid && d.Estado_id == true
                                                                                       select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                //1	                Oficio Jurídico
                //2	                Oficio Técnico
                //3	                Oficio Sub-regional
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                string No_OficioTecnico = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 2, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;

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

                        if ((oOficioTecnico[i].Descripcion.Trim() != null) && (oOficioTecnico[i].Descripcion.Trim() != ""))
                        {
                            oEnmiendasOficio.Add($"{oOficioTecnico[i].Descripcion.Trim()}");
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
                //obtenertexto = wsheet1.Cells[iniciofila, iniciocolumna].Value;

                int startrow, startcol;

                datobusqueda = "{No_Oficio}";
                datoreemplazar = No_OficioTecnico;
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


                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                iniciofila = finalfila = 6;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Director_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = 10;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{SubCategoria}";
                datoreemplazar = oSolicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Solicitante}";
                datoreemplazar = oSolDatosDescripcion.Propietario;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Direccion}";
                datoreemplazar = oSolDatosDescripcion.Direccion;
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


                iniciofila = finalfila = 20;
                iniciocolumna = finalcolumna = 1;


                datobusqueda = "{Lista_Enmiendas}";
                datolistareemplazar = oEnmiendasOficio;

                contador = 0;
                if (datolistareemplazar.Count() > 0)
                {

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
                iniciofila = finalfila = iniciofila + 7;
                iniciocolumna = finalcolumna = 1;
                datobusqueda = "{Nombre_Tecnico}";
                datoreemplazar = (oTecnicoAsignado.Nombre + " " + oTecnicoAsignado.Apellidos).Trim();
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{SubRegion_Tecnico}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;
                obtenertexto = (string)datos;
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

        string GenerarEnmiendasOficioTecnico(string Guid_id, string EtapaSolicitudGuid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (objSesion.getBlSession())
            {
                objUs = (Usuario)Session["User"];
            }

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto,
                parrafo1, parrafo2, parrafo3;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioTecnico.docx";
            machotexlsx = $"{rootpath}Machote_OficioTecnico.xlsx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destxlsx = $"{destfinal}.xlsx";
            destpdf = $"{destfinal}.pdf";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
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
            parrafo1 = "Por este medio informo que se realizó un análisis e inspección de campo para el expediente {No_Expediente} según solicitud de {Descripcion_Gestion} en el Registro Nacional Forestal, en la subcategoría de {SubCategoria} solicitado {Solicitante}, ubicado en {Direccion} del municipio de {Municipio} del Departamento de {Departamento}.";
            parrafo2 = "Para continuar con la gestión de {Descripcion_Gestion}, el solicitante debe enmendar lo siguiente.";
            parrafo3 = "Por lo anterior, solicito a su persona requerir la información al solicitante. Para continuar con el trámite del expediente administrativo.";

            if (oEtapaSolicitud != null)
            {
                Document doc = new Document(PageSize.LETTER);
                doc.SetMargins(1f, 1f, 25f, 50f);
                if (!Directory.Exists(rootdest)) Directory.CreateDirectory(rootdest);
                FileStream _stream = new FileStream(destpdf, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, _stream);

                var Enter = new Paragraph(" ");
                doc.Open();

                try
                {
                    List<string> oEnmiendasOficio = new List<string>();
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

                    //1	                Oficio Jurídico
                    //2	                Oficio Técnico
                    //3	                Oficio Sub-regional
                    CrearBanner crearBanner = new CrearBanner();
                    IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                    Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(GestionTipo_id: 2, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id);
                    string No_OficioTecnico = resultsp.Identificador;
                    //string No_OficioTecnico = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 2, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;

                    Personerias personerias = ObtenerPersonerias(oSolicitud.Solicitud_id);
                    fc_Sol_Sel_Direccion_Result Sol_Sel_Direccion = db.fc_Sol_Sel_Direccion(oSolicitud.Solicitud_id).FirstOrDefault();

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

                            if ((oOficioTecnico[i].Descripcion.Trim() != null) && (oOficioTecnico[i].Descripcion.Trim() != ""))
                            {
                                oEnmiendasOficio.Add($"{oOficioTecnico[i].Descripcion.Trim()}");
                            }

                        }



                    }


                    string descripciongestion = "";
                    decimal tiposolicitud = 0;
                    tiposolicitud = oSolicitud.SolicitudTipo_id - Math.Truncate(oSolicitud.SolicitudTipo_id);

                    if (tiposolicitud == 0)
                    {
                        descripciongestion = "inscripción";
                    }
                    if ((tiposolicitud == (decimal)0.01) || (tiposolicitud == (decimal)0.02) || (tiposolicitud == (decimal)0.03))
                    {
                        descripciongestion = "actualización";
                    }
                    if (tiposolicitud == (decimal) 0.04)
                    {
                        descripciongestion = "inactivación";
                    }

                    obtenertexto = parrafo1;
                    datobusqueda = "{No_Expediente}";
                    datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                    if ((oSolicitud.No_Registro != null) && (oSolicitud.No_Registro.Trim() != ""))
                    {
                        datoreemplazar += " con número de registro " + oSolicitud.No_Registro;
                    }
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{SubCategoria}";
                    datoreemplazar = oSolicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{Descripcion_Gestion}";
                    datoreemplazar = descripciongestion;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{Solicitante}";
                    datoreemplazar = "";
                    List<string> datolistreemplazar = new List<string>();
                    datolistreemplazar = personerias.PropietariosIndividuales;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Propietario: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    datolistreemplazar = personerias.PropietariosJuridicos;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Propietario: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    datolistreemplazar = personerias.RepresentatnteLegal;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Representante Legal: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    datolistreemplazar = personerias.Mandatario;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Mandatario: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    datolistreemplazar = personerias.ArrendatariosIndividuales;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Arrendatario: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    datolistreemplazar = personerias.ArrendatariosJuridicos;
                    if (datolistreemplazar.Count() > 0)
                    {
                        datoreemplazar += " a través de Arrendatario: ";
                        for (int i = 0; i < datolistreemplazar.Count(); i++)
                        {
                            datoreemplazar += datolistreemplazar[i];
                            if (i != datolistreemplazar.Count() - 1)
                            {
                                if (i == datolistreemplazar.Count() - 2)
                                {
                                    datoreemplazar += " y ";
                                }
                                else
                                {
                                    datoreemplazar += ", ";
                                }
                            }
                        }
                    }
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{Direccion}";
                    datoreemplazar = Sol_Sel_Direccion.Direccion;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{Municipio}";
                    datoreemplazar = Sol_Sel_Direccion.Municipio;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    datobusqueda = "{Departamento}";
                    datoreemplazar = Sol_Sel_Direccion.Departamento;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    parrafo1 = obtenertexto;


                    obtenertexto = parrafo2;
                    datobusqueda = "{Descripcion_Gestion}";
                    datoreemplazar = descripciongestion;
                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                    parrafo2 = obtenertexto;


                    datobusqueda = "{Nombre_Tecnico}";
                    datoreemplazar = (oTecnicoAsignado.Nombre + " " + oTecnicoAsignado.Apellidos).Trim();

                    datobusqueda = "{SubRegion_Tecnico}";
                    datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;

                    obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                    datobusqueda = "{Fecha_Hora}";
                    datoreemplazar = strswdatecreated;


                    string varTitulo = "OFICIO DE ENMIENDAS TECNICAS";
                    crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                    doc.Add(crearBanner.tableTitulo);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner("Oficio No. " + No_OficioTecnico, "Transparente", Al_Derecha, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);

                    string strFecha = db.Database.SqlQuery<string>("SELECT dbo.[Fnc_Gral_FechaSolicitudTxt]('" + SQLDate(DateTime.Now) + "','" + oSolicitud.Solicitud_id + "')").FirstOrDefault();
                    LlenaBanner(strFecha, "Transparente", Al_Derecha, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);

                    LlenaBanner("SubRegion " + oSolDatosDescripcion.SubRegion, "Transparente", Al_Izquierda, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);

                    LlenaBanner(NombreSubRegional, "Transparente", Al_Izquierda, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(parrafo1, "Transparente", Al_Justificado, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(parrafo2, "Transparente", Al_Justificado, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);


                    datobusqueda = "{Lista_Enmiendas}";
                    datolistareemplazar = oEnmiendasOficio;

                    contador = 0;
                    if (datolistareemplazar.Count() > 0)
                    {
                        for (int i = 0; i < datolistareemplazar.Count(); i++)
                        {

                            contador += 1;
                            valornuevo = "    " + contador + ". " + datolistareemplazar[i];

                            LlenaBanner(valornuevo, "Transparente", Al_Izquierda, Al_Medio, 0, "Normal");
                            doc.Add(tableBanner);

                        }
                    }
                    else { }

                    doc.Add(Enter);

                    LlenaBanner(parrafo3, "Transparente", Al_Izquierda, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);
                    doc.Add(Enter);

                    LlenaBanner("Atentamente", "Transparente", Al_Izquierda, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);
                    doc.Add(Enter);


                    LlenaBanner((oTecnicoAsignado.Nombre + " " + oTecnicoAsignado.Apellidos).Trim(), "Transparente", Al_Centro, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);
                    LlenaBanner("Técnico(a) Forestal - SubRegion " + oSolDatosDescripcion.SubRegion, "Transparente", Al_Centro, Al_Medio, 0, "Normal");
                    doc.Add(tableBanner);


                    doc.Close();
                    writer.Close();


                }
                catch (Exception ex)
                {


                    doc.Close();
                    writer.Close();

                    return null;
                }

            }

            return nombrereporte + ".pdf";
        }


        string GenerarEnmiendaOficioTecnico(string Guid_id, string EtapaSolicitudGuid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (objSesion.getBlSession())
            {
                objUs = (Usuario)Session["User"];
            }

            ExcelPackage oEPP;
            GPDF.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            DateTime swdatecreated;
            swdatecreated = DateTime.Now;

            string guidid, guidetapasol, strswdatecreated, stronlydatecreated;
            string rootbase, rootpath, partialdestpath, rootdest, machotedocx, machotexlsx, destfinal,
                nombrereporte, destfile, destdocx, destxlsx,
                destpdf, datobusqueda, datoreemplazar, docreturn, obtenertexto;

            guidid = Guid_id;
            guidetapasol = EtapaSolicitudGuid;

            #region Rutas Predeterminadas de Archivos
            rootbase = Server.MapPath("~/");
            rootpath = $"{rootbase}Archivos_Machotes/";
            machotedocx = $"{rootpath}Machote_OficioTecnico.docx";
            machotexlsx = $"{rootpath}Machote_OficioTecnico.xlsx";
            partialdestpath = $"Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialdestpath}";
            nombrereporte = $"{guidetapasol}";
            destfile = $"{rootdest}{nombrereporte}";
            destfinal = $"{destfile}";
            destdocx = $"{destfinal}.docx";
            destxlsx = $"{destfinal}.xlsx";
            destpdf = $"{destfinal}.pdf";
            docreturn = $"/{partialdestpath}{nombrereporte}";

            int iniciofila, iniciocolumna, finalfila, finalcolumna, tipodearea;
            int contador = 0;
            string valornuevo;
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

                Tbl_Seg_Usuario oTecnicoAsignado = (from d in db.Tbl_Seg_Usuario
                                                    where d.Usuario_id == oSolicitud.TecnicoAsignado_id
                                                    select d).FirstOrDefault();

                List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> oOficioTecnico = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                                                                                       where d.Solicitud_Guid_id == guidid && d.Estado_id == true
                                                                                       select d).ToList();

                fc_SolRNF_DatosInscripcion_Result oSolDatosDescripcion = (from d in db.fc_SolRNF_DatosInscripcion(oSolicitud.Solicitud_id)
                                                                          select d).FirstOrDefault();

                //1	                Oficio Jurídico
                //2	                Oficio Técnico
                //3	                Oficio Sub-regional
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                string No_OficioTecnico = identificadorOficialGestion.ObtenerNumeroOficio(GestionTipo_id: 2, Solicitud_id: oSolicitud.Solicitud_id, Etapa_id: oEtapaSolicitud.Etapa_id, EtapaRuta_id: oEtapaSolicitud.EtapaRuta_id, oEtapaSolicitud.CorrelativoEtapa_id, Usuario_id: objUs.intUsuario_id).Identificador;

                Personerias personerias = ObtenerPersonerias(oSolicitud.Solicitud_id);
                fc_Sol_Sel_Direccion_Result Sol_Sel_Direccion = db.fc_Sol_Sel_Direccion(oSolicitud.Solicitud_id).FirstOrDefault();

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

                        if ((oOficioTecnico[i].Descripcion.Trim() != null) && (oOficioTecnico[i].Descripcion.Trim() != ""))
                        {
                            oEnmiendasOficio.Add($"{oOficioTecnico[i].Descripcion.Trim()}");
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
                //obtenertexto = wsheet1.Cells[iniciofila, iniciocolumna].Value;

                int startrow, startcol;

                datobusqueda = "{No_Oficio}";
                datoreemplazar = No_OficioTecnico;
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


                datobusqueda = "{Nombre_SubRegional}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                iniciofila = finalfila = 6;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{Director_SubRegional}";
                datoreemplazar = NombreSubRegional;
                iniciofila = finalfila = 7;
                iniciocolumna = finalcolumna = 1;
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }


                iniciofila = finalfila = 10;
                iniciocolumna = finalcolumna = 1;
                var datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;

                obtenertexto = (string)datos;

                datobusqueda = "{No_Expediente}";
                datoreemplazar = oSolicitud.Solicitud_NumeroExpediente;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{SubCategoria}";
                datoreemplazar = oSolicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Solicitante}";
                datoreemplazar = "";
                List<string> datolistreemplazar = new List<string>();
                datolistreemplazar = personerias.PropietariosIndividuales;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Propietario: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }
                datolistreemplazar = personerias.PropietariosJuridicos;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Propietario: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }
                datolistreemplazar = personerias.RepresentatnteLegal;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Representante Legal: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }
                datolistreemplazar = personerias.Mandatario;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Mandatario: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }
                datolistreemplazar = personerias.ArrendatariosIndividuales;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Arrendatario: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }
                datolistreemplazar = personerias.ArrendatariosJuridicos;
                if (datolistreemplazar.Count() > 0)
                {
                    datoreemplazar += " a través de Arrendatario: ";
                    for (int i = 0; i < datolistreemplazar.Count(); i++)
                    {
                        datoreemplazar += datolistreemplazar[i];
                        if (i != datolistreemplazar.Count() - 1)
                        {
                            if (i == datolistreemplazar.Count() - 2)
                            {
                                datoreemplazar += " y ";
                            }
                            else
                            {
                                datoreemplazar += ", ";
                            }
                        }
                    }
                }

                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Direccion}";
                datoreemplazar = Sol_Sel_Direccion.Direccion;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Municipio}";
                datoreemplazar = Sol_Sel_Direccion.Municipio;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);
                datobusqueda = "{Departamento}";
                datoreemplazar = Sol_Sel_Direccion.Departamento;
                obtenertexto = obtenertexto.Replace(datobusqueda, datoreemplazar);

                iniciofila = finalfila = 10;
                iniciocolumna = 1;
                finalcolumna = iniciocolumna + 8;
                AgregarTextoDinamicoCeldasCombinadas(wsheet1, iniciofila, iniciocolumna, finalfila, finalcolumna, obtenertexto);


                iniciofila = finalfila = 15;
                iniciocolumna = finalcolumna = 1;


                datobusqueda = "{Lista_Enmiendas}";
                datolistareemplazar = oEnmiendasOficio;

                contador = 0;
                if (datolistareemplazar.Count() > 0)
                {

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
                iniciofila = finalfila = iniciofila + 7;
                iniciocolumna = finalcolumna = 1;
                datobusqueda = "{Nombre_Tecnico}";
                datoreemplazar = (oTecnicoAsignado.Nombre + " " + oTecnicoAsignado.Apellidos).Trim();
                using (ExcelRange rango = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna])
                {
                    rango.Value = datoreemplazar;
                }

                datobusqueda = "{SubRegion_Tecnico}";
                datoreemplazar = "SubRegion " + oSolDatosDescripcion.SubRegion;
                iniciofila = finalfila = iniciofila + 1;
                iniciocolumna = finalcolumna = 1;
                datos = wsheet1.Cells[iniciofila, iniciocolumna, finalfila, finalcolumna].Value;
                obtenertexto = (string)datos;
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


        class UbicacionArchivo
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

        [HttpPost]
        public JsonResult GrabarEnmienda(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda> lstContEnmienda = new List<Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda>();
            DateTime swdatecreated;
            int contEnmienda;

            swdatecreated = DateTime.Now;
           

            int lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda.Where(Obj => Obj.Solicitud_Guid_id == model.Solicitud_Guid_id).Max(u => u.Enmienda_id);
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

            db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda.Add(model);
            db.SaveChanges();

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        [HttpPost]
        public JsonResult ActualizaEstadoEnmienda(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
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
        public JsonResult EliminarEnmienda(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda oEnmienda = new Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda();
            oEnmienda = (from d in db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda
                         where d.Enmienda_id == model.Enmienda_id && d.Solicitud_Guid_id == model.Solicitud_Guid_id
                         select d).FirstOrDefault();

            if (oEnmienda != null)
            {
                db.Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda.Remove(oEnmienda);
                db.SaveChanges();
            }

            string TextoMostrar;
            TextoMostrar = "{ \"Resultado\": \"Realizado\" }";

            return Json(TextoMostrar);
        }

        public JsonResult ObtenerEnmiendaOficioTecnico_Enmienda(Tbl_Gest_EtapaSolicitud_OficioTecnico_Enmienda model)
        {
            string guidsolicitud, guidetapasolicitud, ubicacion;
            guidsolicitud = model.Solicitud_Guid_id;
            guidetapasolicitud = model.EtapaSolicitud_GUID_id;
            UbicacionArchivo oUbi = new UbicacionArchivo();
            try
            {
                oUbi.Result = 1;
                oUbi.Mensaje = "Archivo generado exitosamente";
                ubicacion = GenerarEnmiendasOficioTecnico(guidsolicitud, guidetapasolicitud);
                oUbi.Ubicacion = ubicacion;
            }catch(Exception ex)
            {
                oUbi.Result = 2;
                oUbi.Mensaje = "Error: " + ex.Message;
            }
            return Json(JsonConvert.SerializeObject(oUbi));
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

    }
 }
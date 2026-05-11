using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using RNF_Web.Models;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class UnificarController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        // GET: Unificar
        public ActionResult Index(string Guid_id)
        {
            string guidid;
            guidid = "AC30AF3B-6C49-4CF2-870A-A71140DC359E";

            if(Guid_id != null)
            {
                guidid = Guid_id;
            }

            ViewBag.guidid = guidid;

            List<Tbl_Sol_DocumentoTipo> lst = (from d in db.Tbl_Sol_DocumentoTipo
                                               orderby d.Tipo_Documento_id
                                               select d).ToList();
            return View(lst);
        }


        public JsonResult jsonUnificar(string Guid_id, string Guidetapa_id, int TipoDocumento_id)
        {

            string guidid, rootbase, rootdest, archivoadjuntar, Ext, extdest, partialroot, archivodefault;
            long solicitudid;
            List<string> documentos = new List<string>();
            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            List<Tbl_Sol_DocumentoSubido> oDocSubidos = new List<Tbl_Sol_DocumentoSubido>();
            guidid = Guid_id;
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            //long ticks = swdatecreated.Ticks;
            string ticks = "U" + Guidetapa_id + ".pdf";
            extdest = ".pdf";
            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialroot}{ticks}";
            archivodefault = $"{rootbase}{partialroot}{Guidetapa_id + ".pdf"}";
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

                List<Tbl_Sol_DocumentoSubido> oAgregarSubidos = (from d in db.Tbl_Sol_DocumentoSubido
                                                                 where d.Solicitud_id == solicitudid && (d.Tipo_Documento_id == TipoDocumento_id )
                                                                 orderby d.Documento_id
                                                                 select d).ToList();
                if (oAgregarSubidos.Count() > 0)
                {
                    for (int i = 0; i < oAgregarSubidos.Count(); i++)
                    {
                        oDocSubidos.Add(oAgregarSubidos[i]);
                    }
                }


            Document doc = new Document(PageSize.LEGAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path: rootdest, mode: FileMode.Create));

            PdfImportedPage page = null;
            var Enter = new Paragraph(" ");

            doc.Open();
            //doc.Add(Enter);
            //if (Constants.VisualizarInformacionDesarrollo == 1)
            //{
            //    string urlact = this.Url.Action();
            //    LlenaBanner(urlact + "GenerarCaratulaSolicitud");
            //    doc.Add(tableBanner);
            //    doc.Add(Enter);
            //}

            PdfContentByte cb = pdfWriter.DirectContent;

            AddToPDF(archivodefault, pdfWriter, page, doc, cb);

            for (int i = 0; i < oDocSubidos.Count(); i++)
            {
                archivoadjuntar = $"{rootbase}Archivos_Subidos/{solicitudid}/{oDocSubidos[i].Tipo_Documento_id}/{oDocSubidos[i].FileName}";
                Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();
                doc.NewPage();
                if (Ext == "jpg" || Ext == "png" || Ext == "bmp")
                {
                    iTextSharp.text.Paragraph anchos = new iTextSharp.text.Paragraph();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(archivoadjuntar);
                    doc.Add(image);
                }

                if (Ext == "pdf")
                {
                    AddToPDF(archivoadjuntar, pdfWriter, page, doc, cb);
                }

            }

            doc.Close();

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

            Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado = "U" + Guidetapa_id + ".pdf";

            db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
            db.SaveChanges();


            string jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + 1 + "\","
                                  + "\"strRespuesta\":" + "\"" + partialroot + "U" + Guidetapa_id + ".pdf" + "\"}";

            return Json(jsonResultUsr);
        }


        public JsonResult jsonUnificarBitacora(string Guid_id, string Guidetapa_id, long Bitacora_id)
        {

            string guidid, rootbase, rootdest, archivoadjuntar, Ext, extdest, partialroot, archivodefault;
            long solicitudid;
            List<string> documentos = new List<string>();
            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            List<Tbl_RNF_Registro_BitacoraDocumento> oDocSubidos = new List<Tbl_RNF_Registro_BitacoraDocumento>();
            guidid = Guid_id;
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            //long ticks = swdatecreated.Ticks;
            string ticks = "U" + Guidetapa_id + ".pdf";
            extdest = ".pdf";
            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialroot}{ticks}";
            archivodefault = $"{rootbase}{partialroot}{Guidetapa_id + ".pdf"}";
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            List<Tbl_RNF_Registro_BitacoraDocumento> tbl_RNF_Registro_BitacoraDocumentos = (from d in db.Tbl_RNF_Registro_BitacoraDocumento
                                                                                            where d.No_Registro == oSolicitud.No_Registro
                                                                                            && d.Solicitud_id == oSolicitud.Solicitud_id
                                                                                            && d.Bitacora_id == Bitacora_id
                                                                                            orderby d.Documento_id
                                                                                            select d).ToList();

            if (tbl_RNF_Registro_BitacoraDocumentos.Count() > 0)
            {
                for (int i = 0; i < tbl_RNF_Registro_BitacoraDocumentos.Count(); i++)
                {
                    oDocSubidos.Add(tbl_RNF_Registro_BitacoraDocumentos[i]);
                }
            }


            Document doc = new Document(PageSize.LEGAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path: rootdest, mode: FileMode.Create));

            PdfImportedPage page = null;
            var Enter = new Paragraph(" ");

            doc.Open();
            //doc.Add(Enter);
            //if (Constants.VisualizarInformacionDesarrollo == 1)
            //{
            //    string urlact = this.Url.Action();
            //    LlenaBanner(urlact + "GenerarCaratulaSolicitud");
            //    doc.Add(tableBanner);
            //    doc.Add(Enter);
            //}

            PdfContentByte cb = pdfWriter.DirectContent;


            string jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + 1 + "\","
                                  + "\"strRespuesta\":" + "\"" + partialroot + "U" + Guidetapa_id + ".pdf" + "\"}";


            try
            {

                AddToPDF(archivodefault, pdfWriter, page, doc, cb);

                for (int i = 0; i < oDocSubidos.Count(); i++)
                {
                    archivoadjuntar = $"{rootbase}Archivos_Subidos/{oDocSubidos[i].No_Registro}/Bitacora_{oDocSubidos[i].Bitacora_id}/{oDocSubidos[i].NombreArchivo}";
                    Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();
                    doc.NewPage();
                    if (Ext == "jpg" || Ext == "png" || Ext == "bmp")
                    {
                        iTextSharp.text.Paragraph anchos = new iTextSharp.text.Paragraph();
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(archivoadjuntar);
                        doc.Add(image);
                    }

                    if (Ext == "pdf")
                    {
                        AddToPDF(archivoadjuntar, pdfWriter, page, doc, cb);
                    }

                }

                doc.Close();

                Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

                Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado = "U" + Guidetapa_id + ".pdf";

                db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + 2 + "\","
                                      + "\"strRespuesta\":" + "\"No se pudo generar, " + ex.Message + "\"}";
                doc.Close();

            }

            return Json(jsonResultUsr);
        }

        public JsonResult jsonUnificarGestEtapaSolicitud(string Guidetapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            string guidid, rootbase, rootdest, archivoadjuntar, Ext, extdest, partialroot, archivodefault;
            long solicitudid;
            List<string> documentos = new List<string>();
            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            List<Tbl_Gest_EtapaSolicitud_Documento> oDocSubidos = new List<Tbl_Gest_EtapaSolicitud_Documento>();
            guidid = Guid_id;
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            //long ticks = swdatecreated.Ticks;
            string ticks = "U" + Guidetapa_id + ".pdf";
            extdest = ".pdf";
            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialroot}{ticks}";
            archivodefault = $"{rootbase}{partialroot}{Guidetapa_id + ".pdf"}";
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == oSolicitud.Solicitud_id
                                                               && d.Etapa_id == etapa_id
                                                               && d.EtapaRuta_id == etaparuta_id
                                                               && d.CorrelativoEtapa_id == correlativoetapa_id
                                                               select d).FirstOrDefault();


            List<Tbl_Gest_EtapaSolicitud_Documento> tbl_Gest_EtapaSolicitud_Documentos = (from d in db.Tbl_Gest_EtapaSolicitud_Documento
                                                                                          where d.Solicitud_id == oSolicitud.Solicitud_id
                                                                                          && d.Etapa_id == etapa_id
                                                                                          && d.EtapaRuta_id == etaparuta_id
                                                                                          && d.CorrelativoEtapa_id == correlativoetapa_id
                                                                                          select d).ToList();

            if (tbl_Gest_EtapaSolicitud_Documentos == null)
            {
                tbl_Gest_EtapaSolicitud_Documentos = new List<Tbl_Gest_EtapaSolicitud_Documento>();
            }

            if (tbl_Gest_EtapaSolicitud_Documentos.Count() > 0)
            {
                for (int i = 0; i < tbl_Gest_EtapaSolicitud_Documentos.Count(); i++)
                {
                    oDocSubidos.Add(tbl_Gest_EtapaSolicitud_Documentos[i]);
                }
            }


            Document doc = new Document(PageSize.LEGAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path: rootdest, mode: FileMode.Create));

            PdfImportedPage page = null;
            var Enter = new Paragraph(" ");

            doc.Open();
            //doc.Add(Enter);
            //if (Constants.VisualizarInformacionDesarrollo == 1)
            //{
            //    string urlact = this.Url.Action();
            //    LlenaBanner(urlact + "GenerarCaratulaSolicitud");
            //    doc.Add(tableBanner);
            //    doc.Add(Enter);
            //}

            PdfContentByte cb = pdfWriter.DirectContent;


            string jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + 1 + "\","
                                  + "\"strRespuesta\":" + "\"" + partialroot + "U" + Guidetapa_id + ".pdf" + "\"}";


            try
            {

                AddToPDF(archivodefault, pdfWriter, page, doc, cb);

                for (int i = 0; i < oDocSubidos.Count(); i++)
                {
                    archivoadjuntar = $"{rootbase}Gest_EtapaSolicitud_Documento/{oDocSubidos[i].Solicitud_id}/{oDocSubidos[i].Etapa_id}/{oDocSubidos[i].EtapaRuta_id.ToString("0.00")}/{oDocSubidos[i].CorrelativoEtapa_id}";
                    archivoadjuntar = archivoadjuntar.Replace(".", "_");
                    archivoadjuntar = $"{archivoadjuntar}/{oDocSubidos[i].FileName}";
                    Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();
                    doc.NewPage();
                    if (Ext == "jpg" || Ext == "png" || Ext == "bmp")
                    {
                        iTextSharp.text.Paragraph anchos = new iTextSharp.text.Paragraph();
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(archivoadjuntar);
                        doc.Add(image);
                    }

                    if (Ext == "pdf")
                    {
                        AddToPDF(archivoadjuntar, pdfWriter, page, doc, cb);
                    }

                }

                doc.Close();

                Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

                Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado = "U" + Guidetapa_id + ".pdf";

                db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + 2 + "\","
                                      + "\"strRespuesta\":" + "\"No se pudo generar, " + ex.Message + "\"}";
                doc.Close();

            }

            return Json(jsonResultUsr);
        }

        public ActionResult Unificar(string Guid_id, string Guidetapa_id, int[] TipoDocumento_id)
        {

            string guidid, rootbase, rootdest, archivoadjuntar, Ext, extdest, partialroot, archivodefault;
            long solicitudid;
            List<string> documentos = new List<string>();
            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            List<Tbl_Sol_DocumentoSubido> oDocSubidos = new List<Tbl_Sol_DocumentoSubido>();
            guidid = Guid_id;
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            //long ticks = swdatecreated.Ticks;
            string ticks = "U"+Guidetapa_id + ".pdf";
            extdest = ".pdf";
            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            rootdest = $"{rootbase}{partialroot}{ticks}";
            archivodefault = $"{rootbase}{partialroot}{Guidetapa_id+".pdf"}";
            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            foreach(int tipodoc in TipoDocumento_id)
            {
                List<Tbl_Sol_DocumentoSubido> oAgregarSubidos = (from d in db.Tbl_Sol_DocumentoSubido
                                                                 where d.Solicitud_id == solicitudid && d.Tipo_Documento_id == tipodoc
                                                                 orderby d.Documento_id
                                                                 select d).ToList();
                if(oAgregarSubidos.Count() > 0)
                {
                    for(int i = 0; i < oAgregarSubidos.Count(); i++)
                    {
                        oDocSubidos.Add(oAgregarSubidos[i]);
                    }
                }
            }


            Document doc = new Document(PageSize.LEGAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path: rootdest, mode: FileMode.Create));

            PdfImportedPage page = null;
            var Enter = new Paragraph(" ");

            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            PdfContentByte cb = pdfWriter.DirectContent;

            AddToPDF(archivodefault, pdfWriter, page, doc, cb);

            for (int i = 0; i < oDocSubidos.Count(); i++)
            {
                archivoadjuntar = $"{rootbase}Archivos_Subidos/{solicitudid}/{oDocSubidos[i].Tipo_Documento_id}/{oDocSubidos[i].FileName}";
                Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();

                if (Ext == "jpg" || Ext == "png" || Ext == "bmp")
                {
                    iTextSharp.text.Paragraph anchos = new iTextSharp.text.Paragraph();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(archivoadjuntar);
                    doc.Add(image);
                    doc.NewPage();
                }

                if (Ext == "pdf")
                {
                    AddToPDF(archivoadjuntar, pdfWriter, page, doc, cb);
                }

            }

            doc.Close();
            ViewBag.direccion = partialroot + "U" +Guidetapa_id + ".pdf"; ;
            return View();
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

        private PdfPTable tableBanner = new PdfPTable(1);

        public string CrearPDF(string Guid_id, int[] TipoDocumento_id)
        {
            string guidid, rootbase, rootdest, archivoadjuntar, Ext, extdest;
            long solicitudid;
            List<string> documentos = new List<string>();
            Tbl_Sol_Solicitud oSolicitud = new Tbl_Sol_Solicitud();
            List<Tbl_Sol_DocumentoSubido> oDocSubidos = new List<Tbl_Sol_DocumentoSubido>();
            guidid = Guid_id;
            DateTime swdatecreated;
            swdatecreated = DateTime.Now;
            long ticks = swdatecreated.Ticks;
            extdest = ".pdf";
            rootbase = Server.MapPath("~/");
            rootdest = $"{rootbase}/{ticks}{extdest}";

            oSolicitud = (from d in db.Tbl_Sol_Solicitud
                          where d.Guid_id == guidid
                          select d).FirstOrDefault();

            solicitudid = oSolicitud.Solicitud_id;

            foreach (int tipodoc in TipoDocumento_id)
            {
                List<Tbl_Sol_DocumentoSubido> oAgregarSubidos = (from d in db.Tbl_Sol_DocumentoSubido
                                                                 where d.Solicitud_id == solicitudid && d.Tipo_Documento_id == tipodoc
                                                                 orderby d.Documento_id
                                                                 select d).ToList();
                if (oAgregarSubidos.Count() > 0)
                {
                    for (int i = 0; i < oAgregarSubidos.Count(); i++)
                    {
                        oDocSubidos.Add(oAgregarSubidos[i]);
                    }
                }
            }


            Document doc = new Document(PageSize.LEGAL);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path: rootdest, mode: FileMode.Create));

            PdfImportedPage page = null;
            var Enter = new Paragraph(" ");
            doc.Open();
            doc.Add(Enter);
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            PdfContentByte cb = pdfWriter.DirectContent;

            for (int i = 0; i < oDocSubidos.Count(); i++)
            {
                archivoadjuntar = $"{rootbase}Archivos_Subidos/{solicitudid}/{oDocSubidos[i].Tipo_Documento_id}/{oDocSubidos[i].FileName}";
                Ext = Path.GetExtension(archivoadjuntar).Substring(1).ToLower();
                if (Ext == "pdf")
                {
                    AddToPDF(archivoadjuntar, pdfWriter, page, doc, cb);
                }

            }

            doc.Close();
            return null;
        }

        public string AddToPDF(string path, PdfWriter pdfWriter, PdfImportedPage page, iTextSharp.text.Document doc, PdfContentByte cb)
        {
            PdfReader reader = new PdfReader(path);
            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                page = pdfWriter.GetImportedPage(reader, i + 1);

                var width = page.Width
                + doc.RightMargin
                + doc.LeftMargin
                ;
                var height = page.Height
                  + doc.TopMargin
                  + doc.BottomMargin
                ;
                if (page.Width > page.Height)
                {
                    doc.SetPageSize(PageSize.LEGAL_LANDSCAPE.Rotate());
                    doc.NewPage();
                    cb.AddTemplate(page, 0, -50);
                }
                else
                {
                    Rectangle r = width > PageSize.LETTER.Width && height > PageSize.A4.Height
                          ? new Rectangle(width, height)
                          : PageSize.LEGAL
                        ;
                    doc.SetPageSize(r);
                    doc.NewPage();
                    cb.AddTemplate(page, 0, 0);
                }
            }
            return path;
        }

        [HttpPost]
        public JsonResult Procesar(string Guid_id, int[] TipoDocumento_id)
        {


            return Json(null);
        }

       
    }
}
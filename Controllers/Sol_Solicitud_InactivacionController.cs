using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity;

namespace RNF_Web.Controllers
{
    public class Sol_Solicitud_InactivacionController : Controller
    {
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableBanner = new PdfPTable(1);
        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
        db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Solicitud_Inactivacion
        public ActionResult Index(long Solicitud_id, string firma)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            List<Tbl_RNF_Registro_Inactivacion_Tipo> TipoInactivacion_id = db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && Obj.UsuarioExterno == true).ToList();
            decimal tipogestion, procesoinactivacion;
            tipogestion = 0;
            procesoinactivacion = 0.04M;
            bool mostrar = false;
            if(tbl_Sol_Solicitud != null)
            {

                decimal truncsolicitudtipoid = Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
                decimal solicitudtipoid = tbl_Sol_Solicitud.SolicitudTipo_id - truncsolicitudtipoid;

                if((solicitudtipoid == 0.04M)|| (solicitudtipoid == 0.06M))
                {
                    mostrar = true;
                }
            }
            else
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.mostrar = mostrar;

            ViewBag.TipoInactivacion_id = new SelectList(TipoInactivacion_id, "TipoInactivacion_id", "Descripcion", tbl_Sol_Solicitud.TipoInactivacion_id ?? 0);
            return View(tbl_Sol_Solicitud);
        }


        public class Personerias
        {
            public List<Tbl_Sol_PropietarioPersonaIndividual> PropietariosIndividuales { get; set; }
            public List<Tbl_Sol_PropietarioPersonaJuridica> PropietariosJuridicos { get; set; }
            public List<fc_Sol_Rodal_RepresentanteMandatario_Result> RepresentatnteLegal { get; set; }
            public List<fc_Sol_Rodal_RepresentanteMandatario_Result> Mandatario { get; set; }
            public List<Tbl_Sol_ArrendatarioPersonaIndividual> ArrendatariosIndividuales { get; set; }
            public List<Tbl_Sol_ArrendatarioPersonaJuridica> ArrendatariosJuridicos { get; set; }
        }

        Personerias ObtenerPersonerias(long Solicitud_id)
        {
            Personerias personerias = new Personerias();
            personerias.PropietariosIndividuales = new List<Tbl_Sol_PropietarioPersonaIndividual>();
            personerias.PropietariosJuridicos = new List<Tbl_Sol_PropietarioPersonaJuridica>();
            personerias.RepresentatnteLegal = new List<fc_Sol_Rodal_RepresentanteMandatario_Result>();
            personerias.Mandatario = new List<fc_Sol_Rodal_RepresentanteMandatario_Result>();
            personerias.ArrendatariosIndividuales = new List<Tbl_Sol_ArrendatarioPersonaIndividual>();
            personerias.ArrendatariosJuridicos = new List<Tbl_Sol_ArrendatarioPersonaJuridica>();

            List<Tbl_Sol_ArrendatarioPersonaJuridica> tbl_Sol_ArrendatarioPersonaJuridica = (from d in db.Tbl_Sol_ArrendatarioPersonaJuridica
                                                                                             where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                             select d).ToList();

            List<Tbl_Sol_ArrendatarioPersonaIndividual> tbl_Sol_ArrendatarioPersonaIndividual = (from d in db.Tbl_Sol_ArrendatarioPersonaIndividual
                                                                                                 where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                 select d).ToList();


            List<Tbl_Sol_PropietarioPersonaJuridica> tbl_Sol_PropietarioPersonaJuridicas = (from d in db.Tbl_Sol_PropietarioPersonaJuridica
                                                                                            where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                            select d).ToList();

            List<Tbl_Sol_PropietarioPersonaIndividual> tbl_Sol_PropietarioPersonaIndividuals = (from d in db.Tbl_Sol_PropietarioPersonaIndividual
                                                                                                where d.Solicitud_id == Solicitud_id && d.Estado_id == true
                                                                                                select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oRepresentanteLegal = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, false).ToList()
                                                                                     select d).ToList();

            List<fc_Sol_Rodal_RepresentanteMandatario_Result> oMandatario = (from d in db.fc_Sol_Rodal_RepresentanteMandatario(Solicitud_id, true).ToList()
                                                                             select d).ToList();


            if (tbl_Sol_PropietarioPersonaIndividuals.Count() > 0)
            {
                personerias.PropietariosIndividuales = tbl_Sol_PropietarioPersonaIndividuals;
            }

            if (tbl_Sol_PropietarioPersonaJuridicas.Count() > 0)
            {
                personerias.PropietariosJuridicos = tbl_Sol_PropietarioPersonaJuridicas;
            }

            if (oRepresentanteLegal.Count() > 0)
            {
                personerias.RepresentatnteLegal = oRepresentanteLegal;
            }

            if (oMandatario.Count() > 0)
            {
                personerias.Mandatario = oMandatario;
            }

            if (tbl_Sol_ArrendatarioPersonaIndividual.Count() > 0)
            {
                personerias.ArrendatariosIndividuales = tbl_Sol_ArrendatarioPersonaIndividual;
            }

            if (tbl_Sol_ArrendatarioPersonaJuridica.Count() > 0)
            {
                personerias.ArrendatariosJuridicos = tbl_Sol_ArrendatarioPersonaJuridica;
            }

            return personerias;
        }



        private void LlenaTituloRevision(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

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

            c1 = new PdfPCell(new Phrase("\nSOLICITUD DE INACTIVACION DE " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));


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

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Agosto 2021", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
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

        private void CodigoRegistroInactivar(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
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

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.No_Registro, fntTituloTabla));
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableBanner.AddCell(c1);

            return;
        }
        private void FirmaSolicitante(long Solicitud_id, Personerias personerias)
        {
            bool printed = false;

            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(5);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            PdfPCell Enter = new PdfPCell();

            Enter = new PdfPCell(new Phrase($" ", fntTitulo2));
            Enter.Colspan = 5;
            Enter.Border = 0;
            Enter.HorizontalAlignment = Element.ALIGN_CENTER;
            Enter.VerticalAlignment = Element.ALIGN_MIDDLE;


            if ((personerias.Mandatario.Count()) > 0 && (printed == false))
            {
                printed = true;

                for (int i = 0; i < personerias.Mandatario.Count(); i++)
                {
                    tableDatosGenerales.AddCell(Enter);
                    tableDatosGenerales.AddCell(Enter);

                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Colspan = 1;
                    c1.Border = 0;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.) Mandatario", fntTitulo2));
                    c1.Colspan = 1;
                    c1.Border = 0;

                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{personerias.Mandatario[i].Nombres} {personerias.Mandatario[i].Apellidos}", fntTituloTabla));
                    c1.Border = 0;
                    c1.Border = PdfPCell.TOP_BORDER;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Colspan = 1;
                    c1.Border = 0;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                }
            }

            if ((personerias.RepresentatnteLegal.Count() > 0) && (printed == false))
            {
                printed = true;
                for (int i = 0; i < personerias.RepresentatnteLegal.Count(); i++)
                {

                    tableDatosGenerales.AddCell(Enter);
                    tableDatosGenerales.AddCell(Enter);

                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.) Representante", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{personerias.RepresentatnteLegal[i].Nombres} {personerias.RepresentatnteLegal[i].Apellidos}", fntTituloTabla));
                    c1.Border = 0;
                    c1.Colspan = 2;
                    c1.Border = PdfPCell.TOP_BORDER;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if ((personerias.PropietariosIndividuales.Count() > 0) && (printed == false))
            {
                printed = true;
                for (int i = 0; i < personerias.PropietariosIndividuales.Count(); i++)
                {
                    tableDatosGenerales.AddCell(Enter);
                    tableDatosGenerales.AddCell(Enter);


                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.)", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].Nombres + " " + personerias.PropietariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.Border = 0;
                    c1.Border = PdfPCell.TOP_BORDER;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if ((personerias.ArrendatariosIndividuales.Count() > 0) && (printed == false))
            {
                for (int i = 0; i < personerias.ArrendatariosIndividuales.Count(); i++)
                {

                    tableDatosGenerales.AddCell(Enter);
                    tableDatosGenerales.AddCell(Enter);

                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.)", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].Nombres + " " + personerias.ArrendatariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.Border = 0;
                    c1.Colspan = 2;
                    c1.Border = PdfPCell.TOP_BORDER;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if ((personerias.ArrendatariosJuridicos.Count() > 0) && (printed == false))
            {
                for (int i = 0; i < personerias.ArrendatariosJuridicos.Count(); i++)
                {

                    tableDatosGenerales.AddCell(Enter);
                    tableDatosGenerales.AddCell(Enter);

                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Colspan = 1;
                    c1.Border = 0;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.) representante legal de arrendadora", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 3;
                    c1.Border = PdfPCell.TOP_BORDER;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 1;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                }
            }


            return;
        }

        private void FirmaSolicitanteExterno(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 7);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"f.", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Nombres + " " + tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }

        private void NombreExterno(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 7);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");


            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Nombres + " " + tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

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

            PdfPCell c1 = new PdfPCell(new Phrase("Razón", fntTituloTabla));
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
        private void TipoInactivacion(string motivoInactivacion)
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


        public string InactivacionSolicitudProceso(Tbl_Sol_Solicitud tbl_Sol_Solicitud, string NombreArchivo)
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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == objUs.intUsuario_id
                                                             select d).FirstOrDefault();
            Personerias personerias = ObtenerPersonerias(tbl_Sol_Solicitud.Solicitud_id);

            CrearBanner crearBanner = new CrearBanner();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();


            //GestionTipoId  19 --> Cancelacion
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(19, tbl_Sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

            Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = (from d in db.Tbl_Gral_SolicitudConfiguracionTipo
                                                                                       where d.SolicitudTipo_id == tbl_Sol_Solicitud.SolicitudTipo_id
                                                                                       select d).FirstOrDefault();


            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;

            strNombre = NombreArchivo;
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
            string strDirectorRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_NombreSubDirectorRegional](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();
            string strDireccionSubRegional = db.Database.SqlQuery<string>("select dbo.[Fnc_Gral_SubRegionCodigo](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();

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

            string varTitulo = "" + tbl_Gral_SolicitudConfiguracionTipo.DescripcionEnOficio.ToUpper() + "";

            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);


            LlenaBanner("Nombre del Director Subregional: " + strDirectorRegional, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("Dirección Subregional: " + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion + " " + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            if ((tbl_Sol_Solicitud.No_Registro != null) && (tbl_Sol_Solicitud.No_Registro.Trim() != ""))
            {
                CodigoRegistroInactivar(tbl_Sol_Solicitud);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            //TipoInactivacion(tbl_Sol_Solicitud.Tbl_RNF_Registro_Inactivacion_Tipo.Descripcion);
            //doc.Add(tableBanner);
            //doc.Add(Enter);

            MotivoRegistroInactivar(tbl_Sol_Solicitud.Descripcion_Inactivacion);
            doc.Add(tableBanner);
            doc.Add(Enter);
            doc.Add(Enter);


            if (tbl_Sol_Solicitud.Categoria_id == 7)
            {
                FirmaSolicitanteExterno(tbl_Seg_UsuarioExterno);
                doc.Add(tableFirmaSolicitante);
            }
            else if ((tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque || tbl_Sol_Solicitud.Procedencia_secorf) && (objUs.EsInterno == 1))
            {
                NombreExterno(tbl_Seg_UsuarioExterno);
                doc.Add(tableFirmaSolicitante);
            }
            else
            {
                FirmaSolicitante(tbl_Sol_Solicitud.Solicitud_id, personerias);
                doc.Add(tableDatosGenerales);
            }



            doc.Close();
            writer.Close();

            return "/" + strDir + strNombre;

        }




        class InactivarRNF
        {
            public int result { get; set; }
            public string message { get; set; }
            public string ubicacion { get; set; }
            public string NombreArchivo { get; set; }
        }

        public JsonResult GrabarMotivoInactivacion(Tbl_Sol_Solicitud model)
        {

            InactivarRNF inactivarRNF = new InactivarRNF();
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

            if (model.Guid_id != tbl_Sol_Solicitud.Guid_id)
            {
                inactivarRNF.result = 0;
                inactivarRNF.message = "Acceso denegado";
                inactivarRNF.ubicacion = "";
                return Json(JsonConvert.SerializeObject(inactivarRNF));

            }

            tbl_Sol_Solicitud.TipoInactivacion_id = model.TipoInactivacion_id;
            tbl_Sol_Solicitud.Descripcion_Inactivacion = model.Descripcion_Inactivacion;
            db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
            db.SaveChanges();

            long ticks = DateTime.Now.Ticks;
            inactivarRNF.NombreArchivo = $"Frm_{ticks}.pdf";

            string ubicacion = InactivacionSolicitudProceso(tbl_Sol_Solicitud, inactivarRNF.NombreArchivo);
            inactivarRNF.result = 1;
            inactivarRNF.message = "Se ha realizado la inactivación";
            inactivarRNF.ubicacion = ubicacion;
            return Json(JsonConvert.SerializeObject(inactivarRNF));
        }


    }
}
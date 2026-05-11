using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using OfficeOpenXml;
using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace RNF_Web.Controllers
{
    public class ReportePVController : Controller
    {

        private int intModalidadImpresa = 0;

        private db_RNFEntities db = new db_RNFEntities();
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

        private PdfPTable tableCultivosEnAsocio = new PdfPTable(numColumns: 5);


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

        private void LlenaDatosSolicitante(long Solicitud_id, Personerias personerias)
        {


            DateTime fecha;
            PdfPCell c1 = new PdfPCell();
            tableDatosGenerales = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (personerias.PropietariosIndividuales.Count() > 0)
            {
                for (int i = 0; i < personerias.PropietariosIndividuales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].Nombres + " " + personerias.PropietariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosIndividuales[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (personerias.PropietariosJuridicos.Count() > 0)
            {
                for (int i = 0; i < personerias.PropietariosJuridicos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Propietario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosJuridicos[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.PropietariosJuridicos[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                }
            }

            if (personerias.RepresentatnteLegal.Count() > 0)
            {
                for (int i = 0; i < personerias.RepresentatnteLegal.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Datos del Representante Legal", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{personerias.RepresentatnteLegal[i].Nombres} {personerias.RepresentatnteLegal[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.RepresentatnteLegal[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.RepresentatnteLegal[i].RepresentanteNo_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia de la Representación", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));

                    }
                    else
                    {
                        fecha = (DateTime)personerias.RepresentatnteLegal[i].Fecha_InicioNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    }
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vence", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if ((tbl_Sol_Solicitud.No_Registro == null) && (tbl_Sol_Solicitud.Procedencia_PinpepNew || tbl_Sol_Solicitud.Procedencia_PinpepOld || tbl_Sol_Solicitud.Procedencia_Probosque))
                    {
                        c1 = new PdfPCell(new Phrase($"-----", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        if (personerias.RepresentatnteLegal[i].VigenciaIndefinida == true)
                        {
                            c1 = new PdfPCell(new Phrase($"Indefinido", fntTitulo2));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                        else
                        {
                            fecha = (DateTime)personerias.RepresentatnteLegal[i].Fecha_FinNombramiento;
                            c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableDatosGenerales.AddCell(c1);
                        }
                    }
                }
            }

            if (personerias.Mandatario.Count() > 0)
            {

                for (int i = 0; i < personerias.Mandatario.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Mandatario", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    c1.Rowspan = 2;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"{personerias.Mandatario[i].Nombres} {personerias.Mandatario[i].Apellidos}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase(personerias.Mandatario[i].DocumentoTipo + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{personerias.Mandatario[i].RepresentanteNo_Documento}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"Vigencia del mandato", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Fecha de Inicio", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    fecha = (DateTime)personerias.Mandatario[i].Fecha_InicioNombramiento;
                    c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"Vence", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    if (personerias.Mandatario[i].VigenciaIndefinida == true)
                    {
                        c1 = new PdfPCell(new Phrase($"Indefinido", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }
                    else
                    {
                        fecha = (DateTime)personerias.Mandatario[i].Fecha_FinNombramiento;
                        c1 = new PdfPCell(new Phrase($"{fecha.ToString("dd/MM/yyyy")}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        tableDatosGenerales.AddCell(c1);
                    }

                }
            }

            if (personerias.ArrendatariosIndividuales.Count() > 0)
            {
                for (int i = 0; i < personerias.ArrendatariosIndividuales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del Arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].Nombres + " " + personerias.ArrendatariosIndividuales[i].Apellidos, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].Tbl_Gral_DocumentoID_Tipo.Descripcion + ": ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosIndividuales[i].No_Documento, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);

                }
            }

            if (personerias.ArrendatariosJuridicos.Count() > 0)
            {
                for (int i = 0; i < personerias.ArrendatariosJuridicos.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"Nombre del arrendatario:", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosJuridicos[i].Nombre, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 4;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"NIT: ", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(personerias.ArrendatariosJuridicos[i].No_NIT, fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    c1.Colspan = 3;
                    tableDatosGenerales.AddCell(c1);
                }
            }

            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaDatosNotificacion(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {


            Tbl_Gral_Municipio oMunicipio = (from d in db.Tbl_Gral_Municipio
                                             where d.Municipio_id == tbl_Seg_UsuarioExterno.Municipio_id
                                             select d).FirstOrDefault();

            Tbl_Gral_Departamento oDepartamento = (from d in db.Tbl_Gral_Departamento
                                                   where d.Departamento_id == oMunicipio.Departamento_id
                                                   select d).FirstOrDefault();

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Dirección:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Direccion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oMunicipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{oDepartamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Correo Electrónico:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Correo}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 5;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"No. de Celular", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Seg_UsuarioExterno.Telefono_Celular}, {tbl_Seg_UsuarioExterno.Telefono_Oficina}, {tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            tableDatosNotificacion.AddCell(c1);

            tableBanner.AddCell(c1);

            return;
        }

        private void LlenaDatosNotificacionDetallada(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);




            //************************************************************************************************************************************

            //************************************************************************************************************************************

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            //************************************************************************************************************************************

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTablaBanner = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            //c1 = new PdfPCell(new Phrase("Dirección de domicilio", fntTituloTablaBanner));

            //c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            //c1.Colspan = 4;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            //tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Direccion, fntTituloTabla));
            //c1.Colspan = 3;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);


            //c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            //c1.Colspan = 1;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
            //c1.Colspan = 1;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Datos de notificación", fntTituloTablaBanner));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            c1.Colspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Notificacion_Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************




            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);




            return;
        }
        private void LlenaDatosFinca(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosFinca = new PdfPTable(11);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            c1 = new PdfPCell(new Phrase($"Nombre de la finca:", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.NombreFinca}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 3;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.AreaARegistrar}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 8;
            tableDatosFinca.AddCell(c1);


            c1 = new PdfPCell(new Phrase($"Longitud total de árboles en línea (m)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.LongitudDeLineasTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 11;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Registro de la Propiedad:", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.Rowspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Número", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegNumero}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Libro", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegLibro}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Folio", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.RegFolio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 7;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Dirección de la Finca", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Ubicacion}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 9;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Municipio", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 1;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Municipio.Municipio}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 6;
            tableDatosFinca.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Departamento", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            tableDatosFinca.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }
        private void LlenaDatosDeLaFinca(Tbl_Sol_Finca tbl_sol_finca, Tbl_Sol_Solicitud tbl_sol_Solicitud)
        {
            bool ProcedenciaExterna = false;
            decimal GestionTIpoId = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);
            if ((tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf) && ((GestionTIpoId == 0) && (Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id) == 100)))
            {
                ProcedenciaExterna = true;
            }

            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();


            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Nombre de la finca: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.NombreFinca, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Acreditada por medio de: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Sol_FincaConstanciaDePropiedad.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************

            //************************************************************************************************************************************

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 1)
            {
                c1 = new PdfPCell(new Phrase("Numero: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegNumero, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);


                c1 = new PdfPCell(new Phrase("Folio: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegFolio, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);



                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Libro: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.RegLibro, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Departamento de Registro: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_DepartamentoRegistro.Departamento, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);


                //************************************************************************************************************************************


            }

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 2)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

            }

            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 3)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Nombre de quien certifica: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialConCertificacion_NombreDeCertificador, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

            }


            if (tbl_sol_finca.ConstanciaDePorpiedad_id == 4)
            {

                c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Número de escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialDeEscrituraPublica_Numero, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha de la escritura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_sol_finca.ActaNotarialDeEscrituraPublica_Fecha ?? DateTime.Now).ToString("dd/MM/yyyy"), fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

            }

            if (tbl_sol_finca.ConstanciaDePorpiedad_id >= 5)
            {

                if ((tbl_sol_finca.ConstanciaPropiedadDescripcion ?? "") != "")
                {
                    c1 = new PdfPCell(new Phrase("Descripcion del respaldo de adquisicion: ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_sol_finca.ConstanciaPropiedadDescripcion, fntTituloTabla));
                    c1.Colspan = 3;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    tablePersoneria.AddCell(c1);
                }


                if ((tbl_sol_finca.ActaNotarial_Notario ?? "") != "")
                {

                    c1 = new PdfPCell(new Phrase("Nombre del notario: ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarial_Notario, fntTituloTabla));
                    c1.Colspan = 3;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    tablePersoneria.AddCell(c1);
                }

                if ((tbl_sol_finca.ActaNotarialDeEscrituraPublica_Numero ?? "") != "")
                {
                    c1 = new PdfPCell(new Phrase("Número de escritura: ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_sol_finca.ActaNotarialDeEscrituraPublica_Numero, fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Fecha de la escritura: ", fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase((tbl_sol_finca.ActaNotarialDeEscrituraPublica_Fecha ?? DateTime.Now).ToString("dd/MM/yyyy"), fntTituloTabla));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    tablePersoneria.AddCell(c1);
                }

            }

            //************************************************************************************************************************************



            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Ubicacion : ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Ubicacion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_Departamento.Departamento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_finca.Tbl_Gral_Municipio.Municipio, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);




            if (tbl_sol_finca.OtrosRegistrosRNF == true || (tbl_sol_finca.Registros != null && tbl_sol_finca.Registros != ""))
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Si", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Registro", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_finca.Registros, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Existe otro registro (RNF) en la misma finca ? ", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("No", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

            }
            c1.Colspan = 1;

            //Datos que agrega Nefta
            c1 = new PdfPCell(new Phrase($"Área de la finca, según documento de propiedad (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"{tbl_sol_finca.AreaTotal}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"Área a registrar (ha)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);


            var tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == tbl_sol_finca.Solicitud_id && Obj.Finca_id == tbl_sol_finca.Finca_Id).ToList();
            var sumAreaTotal = tbl_Sol_Rodal.Select(c => c.AreaTotalCalculadaSistema).Sum();

            var tbl_Sol_RodalDescuento = db.Tbl_Sol_Rodal_Descuento.Where(Obj => Obj.Solicitud_id == tbl_sol_finca.Solicitud_id && Obj.Finca_id == tbl_sol_finca.Finca_Id).ToList();
            var sumAreaTotalDescuento = tbl_Sol_RodalDescuento.Select(c => c.AreaTotalCalculadaSistema).Sum();

            c1 = new PdfPCell(new Phrase($"{sumAreaTotal - sumAreaTotalDescuento}", fntTituloTabla));
            if (ProcedenciaExterna)
            {
                c1 = new PdfPCell(new Phrase($"{tbl_sol_finca.AreaARegistrar}", fntTituloTabla));
            }
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);




            c1 = new PdfPCell(new Phrase($"Longitud total de árboles en línea (m)", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase($"{tbl_sol_finca.LongitudDeLineasTotal ?? 0}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************
            //************************************************************Datos de fuentes semilleras ******************************************** ///



            //************************************************************Datos de fuentes semilleras ******************************************** ///

            return;
        }


        private void EstimacionVolumen(Tbl_Sol_Finca tbl_Sol_Finca, string Modalidad, string SubModalidad)
        {

            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Longitud_Total, d.EstimacionPorMedioDe, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();



            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(11);
            decimal Total_areabasalha, volumenha, volumenrodal, Total_Areabasalm2, Total_Volumenlineam2;
            decimal SubTotal_Areabasalha, SubTotal_Volumenha, SubTotal_Volumenrodal, SubTotal_Areabasalm2, SubTotal_Volumenlineam2, SubTotal_Densidadha, Total_DensidadRodal, Total_Densidadha, SubTotal_CantArboles, Total_CantArboles,Subtotal_DensidadRodal;
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            if (validacionesDiametrica.Count() > 0)
            {
                string varEspecie = "";
                string varEstimacion = "";
                long rodalid = 0;

                SubTotal_Areabasalha = 0;
                SubTotal_Volumenha = 0;
                SubTotal_Volumenrodal = 0;
                SubTotal_Areabasalm2 = 0;
                SubTotal_Volumenlineam2 = 0;
                SubTotal_Densidadha = 0;
                SubTotal_CantArboles = 0;

                Subtotal_DensidadRodal = 0;

                Total_areabasalha = 0;
                volumenha = 0;
                volumenrodal = 0;
                Total_Areabasalm2 = 0;
                Total_Volumenlineam2 = 0;
                Total_Densidadha = 0;
                Total_CantArboles = 0;
                Total_DensidadRodal = 0;



                decimal DensidadRodal = 0;

                for (int i = 0; i < validacionesDiametrica.Count(); i++)
                {

                    if (validacionesDiametrica[i].Tipo_de_Area == 1)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea;
                            string estimacion;

                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            estimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                            Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();



                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {
                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id).First().Modalidad_Categoria + " " + Modalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 7;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 1;
                            }
                            if ((SubModalidad != "") && (intModalidadImpresa <= 1) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 11;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id).First().Modalidad_SubCategoria + " " + SubModalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 7;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 2;
                            }


                            #region Encabezado de la Tabla

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Localización del bosque en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            // c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1 = new PdfPCell(new Phrase($"ESPECIES A REGISTRAR", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Rodal", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Área ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            if (estimacion == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase($"Arboles por ha", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                            }
                            else
                            {
                            c1 = new PdfPCell(new Phrase($"Densidad por ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            }

                            if (estimacion == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase($"Arboles por Rodal", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                            }
                            else
                            {
                                c1 = new PdfPCell(new Phrase($"Densidad por Rodal", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                            }

                            //c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            //c1.BackgroundColor = fondoVerde;
                            //c1.Colspan = 1;
                            //tableEstimacion.AddCell(c1);


                            c1 = new PdfPCell(new Phrase($"Volumen por ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por Rodal", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Area_Efectiva_Rodal ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            #endregion
                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {
                            //c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Cantidad_Arboles).ToString("0"), fntTituloTabla));
                            c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase((Math.Truncate((decimal)validacionesDiametrica[i].Densidad_ha).ToString()), fntTituloTabla));                            
                            //c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Densidad_ha).ToString("0"), fntTituloTabla));                            
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Cantidad_Arboles).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        {
                            decimal densidad = Math.Truncate((decimal)validacionesDiametrica[i].Densidad_ha);
                            decimal area = Math.Round(validacionesDiametrica[i].Area_Efectiva_Rodal ?? 0, 2);

                            DensidadRodal = densidad * area;
                            

                            //c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Densidad_ha).ToString("0"), fntTituloTabla));
                            c1 = new PdfPCell(new Phrase(Math.Truncate(DensidadRodal).ToString(), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }

                        //c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        //c1.Colspan = 1;
                        //tableEstimacion.AddCell(c1);

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {
                            c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_ha).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                        }
                        

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_Rodal).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 2;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);


                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        { 
                        
                            SubTotal_Densidadha += Math.Truncate((decimal)validacionesDiametrica[i].Cantidad_Arboles);
                        }
                        else
                        {
                            decimal densidad = Math.Truncate((decimal)validacionesDiametrica[i].Densidad_ha);
                            decimal area = Math.Round(validacionesDiametrica[i].Area_Efectiva_Rodal ?? 0, 2);

                            DensidadRodal = densidad * area;



                            SubTotal_Densidadha += Math.Truncate((decimal)validacionesDiametrica[i].Densidad_ha);

                            Subtotal_DensidadRodal += Math.Truncate(DensidadRodal);


                        }

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;                                
                                tableEstimacion.AddCell(c1);

                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    c1.VerticalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                                }
                                
                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString(), fntSubTotal));                                    
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase(Subtotal_DensidadRodal.ToString(), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }

                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }                                    

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha += SubTotal_Areabasalha;
                                volumenha += SubTotal_Volumenha;
                                volumenrodal += SubTotal_Volumenrodal;
                                Total_Densidadha += SubTotal_Densidadha;
                                Total_DensidadRodal += Subtotal_DensidadRodal;
                                Total_CantArboles += SubTotal_CantArboles;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_Densidadha = 0;                               
                                SubTotal_CantArboles = 0;
                                Subtotal_DensidadRodal = 0;

                            }
                        }
                        catch
                        {

                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                            }
                            else //ver
                            {
                                c1 = new PdfPCell(new Phrase(Subtotal_DensidadRodal.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                            }
                               

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenha.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenrodal.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha += SubTotal_Areabasalha;
                            volumenha += SubTotal_Volumenha;
                            volumenrodal += SubTotal_Volumenrodal;
                            Total_Densidadha += SubTotal_Densidadha;
                            Total_CantArboles += SubTotal_CantArboles;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_Densidadha = 0;
                            SubTotal_CantArboles = 0;

                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);
                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    c1.VerticalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                { 
                                    c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                                }

                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase(Total_DensidadRodal.ToString(), fntSubTotal));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    //c1.Border = 0;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }


                                if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase("—", fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {


                                    c1 = new PdfPCell(new Phrase(volumenha.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);
                                }

                                c1 = new PdfPCell(new Phrase(volumenrodal.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;

                                Total_Densidadha = 0;
                                Total_CantArboles = 0;

                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(volumenha.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(volumenrodal.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;

                            Total_Densidadha = 0;
                            Total_CantArboles = 0;

                        }
                    }
                    else if (validacionesDiametrica[i].Tipo_de_Area == 2)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                           
                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {
                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                

                                if (tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id == 4)
                                {
                                    c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase("Modalidad : " + " " + Modalidad, fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Border = 0;
                                    c1.Colspan = 7;
                                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    tableEstimacion.AddCell(c1);
                                }

                                intModalidadImpresa = intModalidadImpresa + 1;

                            }

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización del bosque en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"ESTIMACIÓN DE VOLUMEN POR {validacionesDiametrica[i].EstimacionPorMedioDe.ToUpper()}", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 11;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id. de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Longitud (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 3;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de Plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Cantidad de Árboles", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Longitud_Total ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            #endregion

                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 3;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Cantidad_Arboles ?? 0).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_X_Linea).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tableEstimacion.AddCell(c1);

                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        SubTotal_CantArboles += (decimal)validacionesDiametrica[i].Cantidad_Arboles;

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 4;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                                Total_Areabasalm2 += SubTotal_Areabasalm2;
                                Total_CantArboles += SubTotal_CantArboles;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_CantArboles = 0;
                                SubTotal_Densidadha = 0;
                            }
                        }
                        catch
                        {
                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                            Total_Areabasalm2 += SubTotal_Areabasalm2;
                            Total_CantArboles += SubTotal_CantArboles;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_CantArboles = 0;
                            SubTotal_Densidadha = 0;
                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 11;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;
                                Total_CantArboles = 0;
                                Total_Densidadha = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);


                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 11;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;
                            Total_CantArboles = 0;
                            Total_Densidadha = 0;
                        }
                    }
                }

                tableEstimacion.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }


        private void EstimacionVolumenFS(Tbl_Sol_Finca tbl_Sol_Finca, string Modalidad, string SubModalidad)
        {

            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Longitud_Total, d.EstimacionPorMedioDe, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", tbl_Sol_Finca.Solicitud_id, tbl_Sol_Finca.Finca_Id).FirstOrDefault();



            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(9);
            decimal Total_areabasalha, volumenha, volumenrodal, Total_Areabasalm2, Total_Volumenlineam2;
            decimal SubTotal_Areabasalha, SubTotal_Volumenha, SubTotal_Volumenrodal, SubTotal_Areabasalm2, SubTotal_Volumenlineam2, SubTotal_Densidadha, Total_Densidadha, SubTotal_CantArboles, Total_CantArboles;
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            if (validacionesDiametrica.Count() > 0)
            {
                string varEspecie = "";
                string varEstimacion = "";
                long rodalid = 0;

                SubTotal_Areabasalha = 0;
                SubTotal_Volumenha = 0;
                SubTotal_Volumenrodal = 0;
                SubTotal_Areabasalm2 = 0;
                SubTotal_Volumenlineam2 = 0;
                SubTotal_Densidadha = 0;
                SubTotal_CantArboles = 0;

                Total_areabasalha = 0;
                volumenha = 0;
                volumenrodal = 0;
                Total_Areabasalm2 = 0;
                Total_Volumenlineam2 = 0;
                Total_Densidadha = 0;
                Total_CantArboles = 0;

                for (int i = 0; i < validacionesDiametrica.Count(); i++)
                {

                    if (validacionesDiametrica[i].Tipo_de_Area == 1)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id).First().Modalidad_Categoria + " " + Modalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 5;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 1;
                            }
                            if ((SubModalidad != "") && (intModalidadImpresa <= 1) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {
                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_Sol_Finca.Tbl_Sol_Solicitud.Categoria_id).First().Modalidad_SubCategoria + " " + SubModalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 5;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 2;
                            }

                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización del bosque en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);




                            c1 = new PdfPCell(new Phrase($"DATOS DE LA FUENTE SEMILLERA", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Rodal", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Área ha", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);


                            if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase($"Cantidad de Arboles", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                            else
                            {
                                c1 = new PdfPCell(new Phrase($"Densidad", fntTituloTabla));
                                c1.BackgroundColor = fondoVerde;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                                

                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Area_Efectiva_Rodal ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);
                            #endregion
                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        if (validacionesDiametrica[i].EstimacionPorMedioDe == "Censo")
                        {
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Cantidad_Arboles).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Densidad_ha).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                        }

                        

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((int)validacionesDiametrica[i].Clase).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);
                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        SubTotal_Densidadha += (decimal)validacionesDiametrica[i].Densidad_ha;
                        SubTotal_CantArboles += (decimal)validacionesDiametrica[i].Cantidad_Arboles;

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha += SubTotal_Areabasalha;
                                volumenha += SubTotal_Volumenha;
                                volumenrodal += SubTotal_Volumenrodal;
                                Total_Densidadha += SubTotal_Densidadha;
                                Total_CantArboles += SubTotal_CantArboles;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_Densidadha = 0;
                                SubTotal_CantArboles = 0;

                            }
                        }
                        catch
                        {

                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            tableEstimacion.AddCell(c1);

                            if (validacionesDiametrica[1].EstimacionPorMedioDe == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase(SubTotal_CantArboles.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                            else
                            {
                                c1 = new PdfPCell(new Phrase(SubTotal_Densidadha.ToString("0"), fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                              

                            c1 = new PdfPCell(new Phrase("", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            tableEstimacion.AddCell(c1);



                            Total_areabasalha += SubTotal_Areabasalha;
                            volumenha += SubTotal_Volumenha;
                            volumenrodal += SubTotal_Volumenrodal;
                            Total_Densidadha += SubTotal_Densidadha;
                            Total_CantArboles += SubTotal_CantArboles;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_Densidadha = 0;
                            SubTotal_CantArboles = 0;

                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                if (validacionesDiametrica[1].EstimacionPorMedioDe == "Censo")
                                {
                                    c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    tableEstimacion.AddCell(c1);
                                }
                                else
                                {
                                    c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntTituloTabla));
                                    c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                    c1.Colspan = 1;
                                    tableEstimacion.AddCell(c1);
                                }

                                   

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 2;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 9;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;

                                Total_Densidadha = 0;
                                Total_CantArboles = 0;

                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            if (validacionesDiametrica[1].EstimacionPorMedioDe == "Censo")
                            {
                                c1 = new PdfPCell(new Phrase(Total_CantArboles.ToString("0"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                            else
                            {
                                c1 = new PdfPCell(new Phrase(Total_Densidadha.ToString("0"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);
                            }
                               

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);



                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 9;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;

                            Total_Densidadha = 0;
                            Total_CantArboles = 0;

                        }
                    }
                    else if (validacionesDiametrica[i].Tipo_de_Area == 2)
                    {
                        if ((varEstimacion != validacionesDiametrica[i].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i].Rodal_id))
                        {

                            long solicitudid, fincaid, rodalidd;
                            int tipodearea;
                            solicitudid = validacionesDiametrica[i].Solicitud_id;
                            fincaid = validacionesDiametrica[i].Finca_id;
                            rodalidd = validacionesDiametrica[i].Rodal_id;
                            tipodearea = validacionesDiametrica[i].Tipo_de_Area;
                            Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == fincaid && Obj.Rodal_Id == rodalidd && Obj.Tipo_de_Area == tipodearea).First();


                            if ((Modalidad != "") && (intModalidadImpresa == 0) && (Modalidad.ToUpper().Contains("FRUTAL") != true))
                            {

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 4;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("Subcategoría : " + " " + Modalidad, fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Border = 0;
                                c1.Colspan = 5;
                                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                                tableEstimacion.AddCell(c1);
                                intModalidadImpresa = intModalidadImpresa + 1;

                            }


                            #region Encabezado de la Tabla
                            c1 = new PdfPCell(new Phrase($"Localización del bosque en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Border = 0;
                            c1.Colspan = 4;
                            c1.Rowspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);



                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMX ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal.GTMY ?? 0).ToString("0"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 2;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_CENTER;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);



                            c1 = new PdfPCell(new Phrase($"DATOS DE LA FUENTE SEMILLERA", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 9;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Id. de la finca", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Longitud (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Año de Plantación", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase diamétrica", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Altura Promedio (m)", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Volumen por línea", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase($"Clase", fntTituloTabla));
                            c1.BackgroundColor = fondoVerde;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca.Finca_Id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Rodal_id}", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Longitud_Total ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Rowspan = (int)(validacionesDiametrica[i].Filas_PDF_Estimacion + 1);
                            tableEstimacion.AddCell(c1);

                            #endregion

                        }

                        rodalid = validacionesDiametrica[i].Rodal_id;
                        varEstimacion = validacionesDiametrica[i].EstimacionPorMedioDe;
                        varEspecie = validacionesDiametrica[i].Especie;

                        #region Recorrido de datos de la tabla
                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Especie}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].Anio_Establecimiento}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{validacionesDiametrica[i].ClaseDiametrica}", fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].AlturaPromedio).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(((decimal)validacionesDiametrica[i].Volumen_X_Linea).ToString("0.00"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase((validacionesDiametrica[i].Clase).ToString("0"), fntTituloTabla));
                        c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                        c1.Colspan = 1;
                        tableEstimacion.AddCell(c1);

                        #endregion

                        SubTotal_Areabasalm2 += (decimal)validacionesDiametrica[i].Area_Basal_MetroCuadrado;
                        SubTotal_Volumenlineam2 += (decimal)validacionesDiametrica[i].Volumen_X_Linea;

                        SubTotal_Areabasalha += (decimal)validacionesDiametrica[i].AreaBasal_ha;
                        SubTotal_Volumenha += (decimal)validacionesDiametrica[i].Volumen_ha;
                        SubTotal_Volumenrodal += (decimal)validacionesDiametrica[i].Volumen_Rodal;

                        SubTotal_CantArboles += (decimal)validacionesDiametrica[i].Cantidad_Arboles;

                        try
                        {
                            if ((varEspecie != validacionesDiametrica[i + 1].Especie) || (validacionesDiametrica[i].Rodal_id != validacionesDiametrica[i + 1].Rodal_id))
                            {

                                c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 4;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                                Total_Areabasalm2 += SubTotal_Areabasalm2;
                                Total_CantArboles += SubTotal_CantArboles;

                                SubTotal_Areabasalha = 0;
                                SubTotal_Volumenha = 0;
                                SubTotal_Volumenrodal = 0;
                                SubTotal_Areabasalm2 = 0;
                                SubTotal_Volumenlineam2 = 0;
                                SubTotal_CantArboles = 0;
                                SubTotal_Densidadha = 0;
                            }
                        }
                        catch
                        {
                            c1 = new PdfPCell(new Phrase("Subtotal:", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 4;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(SubTotal_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);


                            Total_Volumenlineam2 += SubTotal_Volumenlineam2;
                            Total_Areabasalm2 += SubTotal_Areabasalm2;
                            Total_CantArboles += SubTotal_CantArboles;

                            SubTotal_Areabasalha = 0;
                            SubTotal_Volumenha = 0;
                            SubTotal_Volumenrodal = 0;
                            SubTotal_Areabasalm2 = 0;
                            SubTotal_Volumenlineam2 = 0;
                            SubTotal_CantArboles = 0;
                            SubTotal_Densidadha = 0;
                        }

                        try
                        {
                            if ((varEstimacion != validacionesDiametrica[i + 1].EstimacionPorMedioDe) || (rodalid != validacionesDiametrica[i + 1].Rodal_id))
                            {
                                #region Ultimas filas de cada tabla
                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 3;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                tableEstimacion.AddCell(c1);

                                c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 1;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);


                                #endregion
                                c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                                c1.Colspan = 9;
                                c1.Border = 0;
                                tableEstimacion.AddCell(c1);

                                Total_areabasalha = 0;
                                volumenha = 0;
                                volumenrodal = 0;
                                Total_Areabasalm2 = 0;
                                Total_Volumenlineam2 = 0;
                                Total_CantArboles = 0;
                                Total_Densidadha = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            #region Ultimas filas de cada tabla
                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 3;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Total_Volumenlineam2.ToString("0.00"), fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("", fntTituloTabla));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 1;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            #endregion
                            c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                            c1.Colspan = 9;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);


                            Total_areabasalha = 0;
                            volumenha = 0;
                            volumenrodal = 0;
                            Total_Areabasalm2 = 0;
                            Total_Volumenlineam2 = 0;
                            Total_CantArboles = 0;
                            Total_Densidadha = 0;
                        }
                    }

                }

                tableEstimacion.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }


        private void ResumenPV(long solicitud_id)
        {
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(9);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_Sol_Sel_Rodal_ValidacionesDiametrica_PV]('" + solicitud_id + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Datos Dasométricos", fntSubTitulo));
                c1.Colspan = 9;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 9;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.000")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[1].EstimacionPorMedioDe == "Censo")
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Cantidad_Arboles.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                    

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (decLongitudLinea != Resultado[i].Longitud_Total)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.000")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }

        private void ResumenPV_FS(long solicitud_id)
        {

            string sylfaenpath = "C:\\Windows\\fonts\\arialbd.ttf";
            BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            //iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntSubTitulo = new iTextSharp.text.Font(sylfaen, size: 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string fuente = "c:/windows/fonts/arialbd.ttf";
            BaseFont bf = BaseFont.CreateFont(fuente, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(10);
            string sqlQuery;

            sqlQuery = " Select Solicitud_id, Finca_id, NombreFinca, Rodal_id, Tipo_de_Area, Tipo_de_Area_Desc, Longitud_Total, Cantidad_Total_Arboles, Especie, Area_Efectiva_Rodal, Anio_Establecimiento, EstimacionPorMedioDe, Cantidad_Arboles, Densidad_ha, AlturaPromedio, DAPPromedio, AreaBasal_ha, Volumen_ha, Volumen_Rodal, Area_Basa_MetroCuadrado, Volumen_X_Linea, CoordenadaX, CoordenadaY, Clase";
            sqlQuery += " From dbo.[fc_Sol_Sel_Rodal_ValidacionesDiametrica_PV]('" + solicitud_id + "') Order by  Solicitud_id, finca_id, Tipo_De_Area ";

            List<ClassResumenPV> Resultado = new List<ClassResumenPV> { };

            Resultado = db.Database.SqlQuery<ClassResumenPV>(sqlQuery).ToList();

            if (Resultado.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Resumen de la fuente semillera", fntSubTitulo));
                c1.Colspan = 10;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

            }

            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long lngRodal = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            for (int i = 0; i < Resultado.Count(); i++)
            {

                if (Finca != Resultado[i].NombreFinca)
                {
                    c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));

                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre de la finca : " + Resultado[i].NombreFinca, fntSubTituloBig));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    Finca = Resultado[i].NombreFinca;
                    varArea = "None";
                }

                if (varArea != Resultado[i].Tipo_de_Area_Desc)
                {

                    varArea = Resultado[i].Tipo_de_Area_Desc;
                    c1 = new PdfPCell(new Phrase("Tipo de área :" + Resultado[i].Tipo_de_Area_Desc + "          Coordendas    GTMX:" + Resultado[i].CoordenadaX.ToString("0") + " GTMY :" + Resultado[i].CoordenadaY.ToString("0"), fntSubTitulo));
                    c1.Colspan = 10;
                    c1.Border = 0;
                    tableEstimacion.AddCell(c1);

                    if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area (ha)", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año de plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        if (Resultado[i].EstimacionPorMedioDe == "Censo")
                        {
                            c1 = new PdfPCell(new Phrase("Cantidad de Arboles", fntSubTitulo));
                            c1.Colspan = 1;
                            c1.Border = 1;
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            tableEstimacion.AddCell(c1);
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase("Densidad ha", fntSubTitulo));
                            c1.Colspan = 1;
                            c1.Border = 1;
                            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                            tableEstimacion.AddCell(c1);
                        }

                           

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP Prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea */
                    {

                        c1 = new PdfPCell(new Phrase("Área", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Tipo", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Longitud", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 2;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año plantación", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP prom", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Clase", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }
                }

                if (Resultado[i].Tipo_de_Area == 1)   /* Rodal */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }


                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Area_Efectiva_Rodal.ToString("0.000")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (Resultado[i].EstimacionPorMedioDe == "Censo")
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Cantidad_Arboles.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Densidad_ha.ToString("0")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                   

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }

                if (Resultado[i].Tipo_de_Area == 2)   /* Arboles en línea  */
                {

                    if ((lngRodal != Resultado[i].Rodal_id) || (lngFinca != Resultado[i].Finca_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Rodal_id}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Tipo_de_Area_Desc}", fntSubTitulo));
                    }

                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (decLongitudLinea != Resultado[i].Longitud_Total)
                    {
                        c1 = new PdfPCell(new Phrase($"{Resultado[i].Longitud_Total.ToString("0.000")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase("", fntSubTitulo));

                    }

                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Especie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 2;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Anio_Establecimiento}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].AlturaPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].DAPPromedio.ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{Resultado[i].Clase.ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }

                if (lngFinca != Resultado[i].Finca_id)
                {
                    lngFinca = Resultado[i].Finca_id;
                }

                if (lngRodal != Resultado[i].Rodal_id)
                {
                    lngRodal = Resultado[i].Rodal_id;
                }

                if (strTipoArea != Resultado[i].Tipo_de_Area_Desc)
                {
                    strTipoArea = Resultado[i].Tipo_de_Area_Desc;
                }

                if (decArea_Efectiva_Rodal != Resultado[i].Area_Efectiva_Rodal)
                {
                    decArea_Efectiva_Rodal = Resultado[i].Area_Efectiva_Rodal;
                }

                if (decLongitudLinea != Resultado[i].Longitud_Total)
                {
                    decLongitudLinea = Resultado[i].Longitud_Total;
                }


            }
            return;
        }



        private void EspeciesForestales_Probosque(long Solicitud_id)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 6;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_Sol_Finca_Probosque_EspeciesForestales> tbl_Sol_Finca_Probosque_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Probosque_EspeciesForestales
                                                                                                           where d.Solicitud_id == Solicitud_id
                                                                                                           orderby d.Finca_id, d.Correlativo_id
                                                                                                           select d).ToList();

            if (tbl_Sol_Finca_Probosque_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Probosque_EspeciesForestales = new List<Tbl_Sol_Finca_Probosque_EspeciesForestales>();
            }



            if (tbl_Sol_Finca_Probosque_EspeciesForestales.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies forestales", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Referencia", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Fecha plantación", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Densidad actual", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);




                for (int i = 0; i < tbl_Sol_Finca_Probosque_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Referencia_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].Area.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Probosque_EspeciesForestales[i].NombreCientifico.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].FechaPlantacion != null)
                    {
                        c1 = new PdfPCell(new Phrase($"{((DateTime)tbl_Sol_Finca_Probosque_EspeciesForestales[i].FechaPlantacion).ToString("yyyy")}", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    if (tbl_Sol_Finca_Probosque_EspeciesForestales[i].DensidadActual != null)
                    {
                        c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_Probosque_EspeciesForestales[i].DensidadActual).ToString("0")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1 = new PdfPCell(new Phrase($"{((tbl_Sol_Finca_Probosque_EspeciesForestales[i].DensidadActual ?? 0)).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);





                }


            }


            return;
        }

        private void EspeciesForestales_PinpepOld(long Solicitud_id)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 5;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);

            List<Tbl_Sol_Finca_PinpepOld_EspeciesForestales> tbl_Sol_Finca_PinpepOld_EspeciesForestales = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesForestales
                                                                                                           where d.Solicitud_id == Solicitud_id
                                                                                                           orderby d.Finca_id, d.Rodal_id
                                                                                                           select d).ToList();

            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesForestales = new List<Tbl_Sol_Finca_PinpepOld_EspeciesForestales>();
            }


            if (tbl_Sol_Finca_PinpepOld_EspeciesForestales.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies forestales", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Finca", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Arboles Ha", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);



                for (int i = 0; i < tbl_Sol_Finca_PinpepOld_EspeciesForestales.Count(); i++)
                {
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Finca_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].NombreEspecie.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].ArbolesPorHa).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesForestales[i].Area}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }


            }


            return;
        }

        private void EspeciesProteger_PinpepOld(long Solicitud_id)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 5;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);

            List<Tbl_Sol_Finca_PinpepOld_EspeciesProteger> tbl_Sol_Finca_PinpepOld_EspeciesProtegers = (from d in db.Tbl_Sol_Finca_PinpepOld_EspeciesProteger
                                                                                                        where d.Solicitud_id == Solicitud_id
                                                                                                        orderby d.Rodal_id
                                                                                                        select d).ToList();


            if (tbl_Sol_Finca_PinpepOld_EspeciesProtegers == null)
            {
                tbl_Sol_Finca_PinpepOld_EspeciesProtegers = new List<Tbl_Sol_Finca_PinpepOld_EspeciesProteger>();
            }


            if (tbl_Sol_Finca_PinpepOld_EspeciesProtegers.Count() > 0)
            {
                c1 = new PdfPCell(new Phrase("Especies a proteger", fntSubTitulo));
                c1.Colspan = maxColumnas;
                c1.Border = 0;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Especies", fntSubTitulo));
                c1.Colspan = 3;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                c1.Colspan = 1;
                c1.Border = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tableEstimacion.AddCell(c1);

                for (int i = 0; i < tbl_Sol_Finca_PinpepOld_EspeciesProtegers.Count(); i++)
                {

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].EspeciesProteger.ToString().ToLower()}", fntSubTitulo));
                    c1.Colspan = 3;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{((decimal)tbl_Sol_Finca_PinpepOld_EspeciesProtegers[i].Area).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                }


            }


            return;
        }

        private void EspeciesForestales_Secorf(long Solicitud_id)
        {

            iTextSharp.text.Font fntSubTituloBig = FontFactory.GetFont("HELVETICA", size: 12, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fntSubTitulo = FontFactory.GetFont("HELVETICA", size: 8, iTextSharp.text.Font.BOLD);

            int maxColumnas = 8;
            PdfPCell c1 = new PdfPCell();

            tableEstimacion = new PdfPTable(maxColumnas);
            string sqlQuery;

            List<Tbl_Sol_Finca_Secorf_EspeciesForestales> tbl_Sol_Finca_Secorf_EspeciesForestales = (from d in db.Tbl_Sol_Finca_Secorf_EspeciesForestales
                                                                                                     where d.Solicitud_id == Solicitud_id
                                                                                                     orderby d.Finca_id, d.Rodal_id, d.Tipo_de_Area, d.Correlativo_id
                                                                                                     select d).ToList();

            if (tbl_Sol_Finca_Secorf_EspeciesForestales == null)
            {
                tbl_Sol_Finca_Secorf_EspeciesForestales = new List<Tbl_Sol_Finca_Secorf_EspeciesForestales>();
            }
            string Finca = "None";
            string varArea = "None";

            long lngFinca = 0;
            long fincaid = 0;
            long lngRodal = 0;
            long rodalid = 0;
            string strTipoArea = "";

            decimal decArea_Efectiva_Rodal = -1;
            decimal decLongitudLinea = -1;

            if (tbl_Sol_Finca_Secorf_EspeciesForestales.Count() > 0)
            {

                for (int i = 0; i < tbl_Sol_Finca_Secorf_EspeciesForestales.Count(); i++)
                {
                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {

                        if (lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id)
                        {
                            fincaid = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id;
                            Tbl_Sol_Finca tbl_RNF_Finca = (from d in db.Tbl_Sol_Finca
                                                           where d.Solicitud_id == Solicitud_id && d.Finca_Id == fincaid
                                                           select d).FirstOrDefault();

                            c1 = new PdfPCell(new Phrase(" ", fntSubTitulo));
                            c1.Colspan = maxColumnas;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("Finca: " + tbl_RNF_Finca.Finca_Id + ", nombre de la finca : " + tbl_RNF_Finca.NombreFinca, fntSubTituloBig));
                            c1.Colspan = maxColumnas;
                            c1.Border = 0;
                            tableEstimacion.AddCell(c1);

                            varArea = "None";
                        }
                        rodalid = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id;
                        Tbl_Sol_Rodal tbl_RNF_Rodal = (from d in db.Tbl_Sol_Rodal
                                                       where d.Solicitud_id == Solicitud_id && d.Finca_id == fincaid && d.Rodal_Id == rodalid
                                                       select d).FirstOrDefault();

                        c1 = new PdfPCell(new Phrase($"Especies forestales, GTMX: {(((decimal)tbl_RNF_Rodal.GTMX).ToString("0"))} GTMY: {(((decimal)tbl_RNF_Rodal.GTMY).ToString("0"))} ", fntSubTitulo));
                        c1.Colspan = maxColumnas;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Rodal", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Area", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Especie", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("DAP", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Altura", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Densidad", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Año Establ.", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Vol m3", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tableEstimacion.AddCell(c1);

                    }

                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id.ToString().ToLower()}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    if ((lngFinca != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id) || (lngRodal != tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id))
                    {
                        c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Area_ha ?? 0).ToString("0.00")}", fntSubTitulo));
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"", fntSubTitulo));
                    }
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Especie_id.ToLower()}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].DAP ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Altura ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);

                   
                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Densidad_ha ?? 0).ToString("0")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                    if (tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento != null)
                    {
                        if (tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento == 0)
                        {
                            c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        }
                        else
                        {
                            c1 = new PdfPCell(new Phrase($"{tbl_Sol_Finca_Secorf_EspeciesForestales[i].Anio_Establecimiento}", fntSubTitulo));
                        }
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }
                    else
                    {
                        c1 = new PdfPCell(new Phrase($"-------", fntSubTitulo));
                        c1.Colspan = 1;
                        c1.Border = 1;
                        tableEstimacion.AddCell(c1);
                    }

                    lngFinca = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Finca_id;
                    lngRodal = tbl_Sol_Finca_Secorf_EspeciesForestales[i].Rodal_id;


                    c1 = new PdfPCell(new Phrase($"{(tbl_Sol_Finca_Secorf_EspeciesForestales[i].Volumen ?? 0).ToString("0.00")}", fntSubTitulo));
                    c1.Colspan = 1;
                    c1.Border = 1;
                    tableEstimacion.AddCell(c1);


                }



            }


            return;
        }

        private void DasometricosClase(Tbl_Sol_Finca tbl_Sol_Finca, int Clase_id)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntSubTotal = FontFactory.GetFont("HELVETICA", size: 10, Font.BOLD);
            PdfPCell c1 = new PdfPCell();
            tableEstimacion = new PdfPTable(7);




            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            int countfilas;
            long solicitudid, fincaid, rodalid;
            decimal dap_promedio, altura_promedio, densidad_ha, dap, altura, densidad;
            dap = 0;
            dap_promedio = 0;
            altura_promedio = 0;
            densidad_ha = 0;
            altura = 0;
            densidad = 0;

            solicitudid = tbl_Sol_Finca.Solicitud_id;
            fincaid = tbl_Sol_Finca.Finca_Id;
            List<Tbl_Sol_Rodal> tbl_Sol_Rodals = new List<Tbl_Sol_Rodal>();
            tbl_Sol_Rodals = (from d in db.Tbl_Sol_Rodal
                              where d.Solicitud_id == solicitudid && d.Finca_id == fincaid
                              select d).ToList();


            if (tbl_Sol_Rodals.Count() > 0)
            {



                for (int i = 0; i < tbl_Sol_Rodals.Count(); i++)
                {
                    dap = 0;
                    dap_promedio = 0;
                    altura_promedio = 0;
                    densidad_ha = 0;
                    altura = 0;
                    densidad = 0;

                    rodalid = tbl_Sol_Rodals[i].Rodal_Id;


                    //#region Localizacion
                    //c1 = new PdfPCell(new Phrase($"Localización de la plantación en coordenadas GTM: (punto centro del rodal)", fntTituloTabla));
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    //c1.Border = 0;
                    //c1.Colspan = 3;
                    //c1.Rowspan = 2;
                    //c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                    //tableEstimacion.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase($"GTMX", fntTituloTabla));
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //c1.Colspan = 2;
                    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //c1.VerticalAlignment = Element.ALIGN_CENTER;
                    //tableEstimacion.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase($"GTMY", fntTituloTabla));
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    //c1.Colspan = 2;
                    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //c1.VerticalAlignment = Element.ALIGN_CENTER;
                    //tableEstimacion.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase((tbl_Sol_Rodals[i].GTMX ?? 0).ToString("0"), fntTituloTabla));
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    //c1.Colspan = 2;
                    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //c1.VerticalAlignment = Element.ALIGN_CENTER;
                    //tableEstimacion.AddCell(c1);

                    //c1 = new PdfPCell(new Phrase((tbl_Sol_Rodals[i].GTMY ?? 0).ToString("0"), fntTituloTabla));
                    //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    //c1.Colspan = 2;
                    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //c1.VerticalAlignment = Element.ALIGN_CENTER;
                    //tableEstimacion.AddCell(c1);

                    //#endregion



                    countfilas = db.Database.SqlQuery<int>
                        ("SELECT count(*) from Tbl_Sol_Rodal_Dasometrico where Solicitud_id = @p0 and Finca_id = @p1 and Rodal_id = @p2 and Clase_id = @p3 ",
                        solicitudid, fincaid, rodalid, Clase_id).FirstOrDefault();

                    List<Tbl_Sol_Rodal_Dasometrico> tbl_Sol_Rodal_Dasometricos = new List<Tbl_Sol_Rodal_Dasometrico>();
                    tbl_Sol_Rodal_Dasometricos = (from d in db.Tbl_Sol_Rodal_Dasometrico
                                                  where d.Solicitud_id == solicitudid && d.Finca_id == fincaid && d.Rodal_id == rodalid && d.Clase_id == Clase_id
                                                  select d).ToList();

                    if (tbl_Sol_Rodal_Dasometricos.Count() > 0)
                    {
                        c1 = new PdfPCell(new Phrase("CLASE " + Clase_id, fntSubTotal));
                        c1.Colspan = 7;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(" ", fntSubTotal));
                        c1.Colspan = 7;
                        c1.Border = 0;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Finca", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Rodal", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Especie", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Año de Plantación", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("DAP", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Altura", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase("Densidad (ha)", fntSubTotal));
                        c1.BackgroundColor = fondoVerde;
                        c1.Colspan = 1;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Rodals[i].Finca_id.ToString(), fntTituloTabla));
                        c1.Colspan = 1;
                        c1.Rowspan = countfilas;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(tbl_Sol_Rodals[i].Rodal_Id.ToString(), fntTituloTabla));
                        c1.Colspan = 1;
                        c1.Rowspan = countfilas;
                        c1.Border = 15;
                        tableEstimacion.AddCell(c1);

                        for (int j = 0; j < tbl_Sol_Rodal_Dasometricos.Count(); j++)
                        {

                            dap += (tbl_Sol_Rodal_Dasometricos[j].DAP_Promedio ?? 0);
                            altura += (tbl_Sol_Rodal_Dasometricos[j].Altura_Promedio ?? 0);
                            densidad = 1; // Estefany 21/11/2022

                            c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Dasometricos[j].Tbl_Gral_Especie.NombreCientifico.ToString(), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(tbl_Sol_Rodal_Dasometricos[j].Anio_Establecimiento.ToString(), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Dasometricos[j].DAP_Promedio ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((tbl_Sol_Rodal_Dasometricos[j].Altura_Promedio ?? 0).ToString("0.00"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase((densidad).ToString("0.00"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);

                            densidad_ha += densidad;

                        }

                        if (tbl_Sol_Rodal_Dasometricos.Count() > 0)
                        {

                            dap_promedio = dap / countfilas;
                            altura_promedio = altura / countfilas;
                            //densidad_ha = (tbl_Sol_Rodal_Dasometricos[0].Area_Efectiva_Rodal ?? 0) / countfilas;

                            c1 = new PdfPCell(new Phrase("Promedio", fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(" ", fntTituloTabla));
                            c1.Colspan = 3;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(dap_promedio.ToString("0.00"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(altura_promedio.ToString("0.00"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);
                            c1 = new PdfPCell(new Phrase(densidad_ha.ToString("0"), fntTituloTabla));
                            c1.Colspan = 1;
                            c1.Border = 15;
                            tableEstimacion.AddCell(c1);

                        }


                    }

                }

            }

            tableBanner.AddCell(c1);
            return;
        }

        private void LlenarCultivosEnAsocio(long Solicitud_id, List<ClassCultivosEnAsocio> Resultado)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");
            int maxcolumnas = 5;
            tableCultivosEnAsocio = new PdfPTable(maxcolumnas);


            c1 = new PdfPCell(new Phrase("Finca", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Área", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Tipo de área", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Cultivo", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Volúmen / Rodal", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableCultivosEnAsocio.AddCell(c1);





            for (int i = 0; i < Resultado.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase(Resultado[i].Finca, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Area, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Tipo_De_Area, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Cultivo, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Resultado[i].Volumen_Rodal.ToString("0.00"), fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tableCultivosEnAsocio.AddCell(c1);



            }

            return;
        }


        private void FormulasCalculoVolumen(long Solicitud_id)
        {
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            PdfPCell c1 = new PdfPCell();
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");
            tableFormulas = new PdfPTable(11);
            List<Tbl_Sol_Rodal_Dasometrico_Especie_Formula> oEspecieFormula;
            c1 = new PdfPCell(new Phrase($"Especie", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 2;
            tableFormulas.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"Fórmula", fntTituloTabla));
            c1.BackgroundColor = fondoVerde;
            c1.Colspan = 9;
            tableFormulas.AddCell(c1);

            oEspecieFormula = (from d in db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula
                               where d.Solicitud_id == Solicitud_id
                               select d).ToList();

            for (int i = 0; i < oEspecieFormula.Count(); i++)
            {

                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].Especie_id, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 2;
                tableFormulas.AddCell(c1);
                c1 = new PdfPCell(new Phrase(oEspecieFormula[i].strFormulario, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 9;
                tableFormulas.AddCell(c1);

            }

            tableBanner.AddCell(c1);
            return;
        }

        private void LlenaDatosPlantacion(Tbl_Sol_Finca tbl_Sol_Finca)
        {

            Tbl_Sol_FincaObjetivoDeLaPlantacion objetivoPlantacion = (from d in db.Tbl_Sol_FincaObjetivoDeLaPlantacion
                                                                      where d.ObjetivoDeLaPlantacion == tbl_Sol_Finca.ObjetivoDeLaPlantacion
                                                                      select d).FirstOrDefault();

            Tbl_Sol_Rodal_CategoriaSIGAP categoriaSIGAP = (from d in db.Tbl_Sol_Rodal_CategoriaSIGAP
                                                           where d.CategoriaSIGAP_Id == tbl_Sol_Finca.CategoriaSIGAP_Id
                                                           select d).FirstOrDefault();

            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            tableDatosPlantacion = new PdfPTable(11);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            string DescripcionPlantacion, DescripcionCategoriaSIGAP, CategoriaSIGAP;


            if (tbl_Sol_Finca.Area_SIGAP == true)
            {
                CategoriaSIGAP = $"Sí";
            }
            else
            {
                CategoriaSIGAP = $"No";
            }

            if (categoriaSIGAP != null)
            {
                DescripcionCategoriaSIGAP = $"{categoriaSIGAP.Descripcion}";
            }
            else
            {
                DescripcionCategoriaSIGAP = $"";
            }
            if (objetivoPlantacion != null)
            {
                DescripcionPlantacion = $"{objetivoPlantacion.Descripcion}";
            }
            else
            {
                DescripcionPlantacion = $"";
            }

            string DescripcionPlantacionCONAP;
            if (tbl_Sol_Finca.Reforestacion_CONAP == true)
            {
                DescripcionPlantacionCONAP = $"¿La plantación es producto de una reforestación con CONAP ? Sí";
            }
            else
            {
                DescripcionPlantacionCONAP = $"¿La plantación es producto de una reforestación con CONAP ? No";
            }

            c1 = new PdfPCell(new Phrase(DescripcionPlantacionCONAP, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);



            c1 = new PdfPCell(new Phrase($"¿La plantación se encuentra dentro de área SIGAP?   {CategoriaSIGAP}", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Colspan = 11;
            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_BOTTOM;
            tableDatosPlantacion.AddCell(c1);


            if (CategoriaSIGAP == "Sí")
            {
                c1 = new PdfPCell(new Phrase($"Nombre de la Categoría SIGAP", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase($"{DescripcionCategoriaSIGAP}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 5;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);

            }


            if (objetivoPlantacion != null)
            {
                c1 = new PdfPCell(new Phrase($"Objetivo de la plantación", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase($"{DescripcionPlantacion}", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);
                c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Border = 0;
                c1.Colspan = 5;
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_BOTTOM;
                tableDatosPlantacion.AddCell(c1);
            }

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


                    //c1 = new PdfPCell(new Phrase($"", fntTitulo2));
                    //c1.Border = 0;
                    //c1.Colspan = 1;
                    //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //tableDatosGenerales.AddCell(c1);


                    c1 = new PdfPCell(new Phrase($"(f.) Propietario/poseedor", fntTitulo2));
                    c1.Border = 0;
                    c1.Colspan = 2;
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


                    c1 = new PdfPCell(new Phrase($"(f.) Arrendatario ", fntTitulo2));
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

        private void NombreSession(Usuario objUs)
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
            c1 = new PdfPCell(new Phrase(objUs.strNombre_Usuario, fntTituloTabla));
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



            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 2;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase("Técnico forestal responsable", fntTituloTabla));
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

        private void FirmaSolicitante_(Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno)
        {
            var Enter = new Paragraph(" ");
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            tableFirmaSolicitante = new PdfPTable(numColumns: 11);
            BaseColor fondoVerde = WebColors.GetRGBColor("#92D050");

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = PdfPCell.BOTTOM_BORDER;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase(tbl_Seg_UsuarioExterno.Nombres + " " + tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($"FIRMA DEL SOLICITANTE", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 4;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);
            c1 = new PdfPCell(new Phrase($" ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            c1.Border = 0;
            c1.Colspan = 1;
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableFirmaSolicitante.AddCell(c1);

            tableBanner.AddCell(c1);
            return;
        }

        private void LlenaCuatroTextos(string TextoA, string TextoB, string TextoC, string TextoD)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);


            tableTitulo = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoD, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;

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

            if (alineacion == "Justificado")
            {
                c1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }

            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
        }


        public class ClassCultivosEnAsocio
        {
            public string Finca { get; set; }
            public string Area { get; set; }
            public string Tipo_De_Area { get; set; }
            public string Cultivo { get; set; }
            public string Anio_Establecimiento { get; set; }
            public decimal Volumen_Rodal { get; set; }
            public decimal Volumen_ha { get; set; }
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




        private void LlenaDatosDeLaMotosierra(Tbl_Sol_Motosierra tbl_sol_motosierra)
        {
            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();


            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Marca: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.Marca, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Modelo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.Modelo, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);
            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Cilindraje: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.Cilindraje, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Potencia: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.Potencia, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tablePersoneria.AddCell(c1);

            //************************************************************************************************************************************
            c1 = new PdfPCell(new Phrase("Serie de la motosierra: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tablePersoneria.AddCell(c1);

            if (tbl_sol_motosierra.EmpesaEmisoraFactura == "")
            {
                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.No_SerieMotosierra, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.No_SerieMotosierra, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Empresa emisora de la factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.EmpesaEmisoraFactura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            //************************************************************************************************************************************

            if (tbl_sol_motosierra.No_Factura != "")
            {
                c1 = new PdfPCell(new Phrase("Serie de la factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.No_SerieFactura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Número de factura: ", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.No_Factura, fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            //************************************************************************************************************************************

            if (tbl_sol_motosierra.OtroDocumentoRespaldo != "")
            {
                c1 = new PdfPCell(new Phrase("Otro documento de respaldo", fntTituloTabla));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_sol_motosierra.OtroDocumentoRespaldo, fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                tablePersoneria.AddCell(c1);
            }

            return;
        }


        private void LlenaDatosProfesionalPersonales(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //******************************************

            c1 = new PdfPCell(new Phrase("Pueblo de pertenencia: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_PuebloPertenencia.Descripcion, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);
            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Sexo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Sexo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de nacimiento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Fecha_Nacimiento.ToString("dd/MM/yyyy"), fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Nombres: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Nombres, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);


            c1 = new PdfPCell(new Phrase("Apellidos: ", fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Apellidos, fntTituloTabla));
            c1.Colspan = 2;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_DocumentoID_Tipo.Descripcion, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_Documento, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("No. de NIT: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.No_NIT, fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }

        private void LlenaDatosProfesionalDireccion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            PdfPCell c1 = new PdfPCell();

            tableDatosNotificacion = new PdfPTable(4);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);


            //************************************************************************************************************************************

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTablaBanner = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            //c1 = new PdfPCell(new Phrase("Dirección de domicilio", fntTituloTablaBanner));

            //c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            //c1.Colspan = 4;

            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
            //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            //tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            //c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Direccion, fntTituloTabla));
            //c1.Colspan = 3;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);


            //c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //GlobalUtils globalUtils = new GlobalUtils();

            //c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            //c1.Colspan = 1;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosNotificacion.AddCell(c1);

            //c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            //c1.Colspan = 1;
            //c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Datos de notificación", fntTituloTablaBanner));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;

            c1.Colspan = 4;

            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Dirección: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Notificacion_Direccion, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Departamento: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            GlobalUtils globalUtils = new GlobalUtils();

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Gral_Departamento.Departamento), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Municipio: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(globalUtils.InitCap(tbl_Sol_Solicitud.Tbl_Gral_Municipio.Municipio), fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            //************************************************************************************************************************************

            c1 = new PdfPCell(new Phrase("Correo: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Correo, fntTituloTabla));
            c1.Colspan = 3;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono celular: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Celular, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Teléfono oficina: ", fntTituloTabla));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 1;
            tableDatosNotificacion.AddCell(c1);

            c1 = new PdfPCell(new Phrase(tbl_Sol_Solicitud.Tbl_Seg_UsuarioExterno.Telefono_Oficina, fntTituloTabla));
            c1.Colspan = 1;
            c1.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            tableDatosNotificacion.AddCell(c1);

            return;
        }



        private void LlenaDatosProfesional(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            PdfPCell c1 = new PdfPCell();
            Tbl_Sol_TecnicoProfesional tbl_Sol_TecnicoProfesional = db.Tbl_Sol_TecnicoProfesional.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).First();

            tableDatosGenerales = new PdfPTable(4);
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            Font fntTitulo2 = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            #region  Grado académico

            c1 = new PdfPCell(new Phrase("Categoría:", fntTitulo2));
            c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            c1.Colspan = 2;
            tableDatosGenerales.AddCell(c1);

            if (tbl_Sol_TecnicoProfesional.Grado_Academico_Tecnico == true)
            {
                c1 = new PdfPCell(new Phrase("Técnico", fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }
            else
            {
                c1 = new PdfPCell(new Phrase("Profesional", fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }
            #endregion

            #region Universidad
            if (tbl_Sol_TecnicoProfesional.Grado_Academico_Profesional == true)
            {

                c1 = new PdfPCell(new Phrase("Profesión", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                if (tbl_Sol_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion.ToUpper().Contains("OTRA") == true)
                {
                    c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.UniversidadEspecificarCarrera, fntTitulo2));
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }
                else
                {
                    c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion, fntTitulo2));
                    c1.Colspan = 2;
                    tableDatosGenerales.AddCell(c1);
                }

            }
            else
            {
                c1 = new PdfPCell(new Phrase("Profesión", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion, fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);
            }

            #endregion


            #region Universidad
            if (tbl_Sol_TecnicoProfesional.Grado_Academico_Profesional == true)
            {
                c1 = new PdfPCell(new Phrase("Universidad", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.Universidad, fntTitulo2));
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase("No. de colegiado", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.No_Colegiado, fntTitulo2));
                c1.Colspan = 1;
                tableDatosGenerales.AddCell(c1);

                if (tbl_Sol_TecnicoProfesional.PostGradoMateriaForestal == true)
                {
                    c1 = new PdfPCell(new Phrase("Universidad de post-grado", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.PostGradoUniversidad, fntTitulo2));
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Post-grado obtenido", fntTitulo2));
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.PostGradoEspecialidad, fntTitulo2));
                    c1.Colspan = 1;
                    tableDatosGenerales.AddCell(c1);

                }

            }

            #endregion

            #region Titulo INAB
            if ((tbl_Sol_Solicitud.Sub_Categoria_id == 3) || (tbl_Sol_Solicitud.Sub_Categoria_id == 4))
            {

                c1 = new PdfPCell(new Phrase("Código de aprobación del curso emitido por INAB", fntTitulo2));
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);

                c1 = new PdfPCell(new Phrase(tbl_Sol_TecnicoProfesional.No_De_Diploma, fntTitulo2));
                c1.Colspan = 2;
                tableDatosGenerales.AddCell(c1);



            }
            #endregion


            //Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_Sol_Solicitud.swcreatedby);
            //Tbl_Sol_TecnicoProfesional tbl_Sol_TecnicoProfesional = db.Tbl_Sol_TecnicoProfesional.Where(Obj=> Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).First();


            //string profesional = (tbl_Sol_TecnicoProfesional.Tbl_Gral_Profesion.Descripcion ?? "");
            //string posgrado = (tbl_Sol_TecnicoProfesional.PostGradoEspecialidad ?? "");
            //string colegiado = (tbl_Sol_TecnicoProfesional.No_Colegiado ?? "");
            //string universidad = (tbl_Sol_TecnicoProfesional.Universidad ?? "");
            //string diplomaobtenido = (tbl_Sol_TecnicoProfesional.EspecificarCarrera ?? "");
            //string nodediploma = (tbl_Sol_TecnicoProfesional.No_De_Diploma ?? "");

            //c1 = new PdfPCell(new Phrase("Profesional", fntTitulo2));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);
            //c1 = new PdfPCell(new Phrase(profesional, fntTituloTabla));
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);

            //c1 = new PdfPCell(new Phrase("Posgrado", fntTitulo2));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);
            //c1 = new PdfPCell(new Phrase(posgrado, fntTituloTabla));
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);

            //c1 = new PdfPCell(new Phrase("Colegiado No", fntTitulo2));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);
            //c1 = new PdfPCell(new Phrase(colegiado, fntTituloTabla));
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);

            //c1 = new PdfPCell(new Phrase("Universidad que lo Avala", fntTitulo2));
            //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //c1.Colspan = 1;
            //tableDatosGenerales.AddCell(c1);
            //c1 = new PdfPCell(new Phrase(universidad, fntTituloTabla));
            //c1.Colspan = 5;
            //tableDatosGenerales.AddCell(c1);

            //if (((tbl_Sol_Solicitud.Categoria_id == 7) && (tbl_Sol_Solicitud.Sub_Categoria_id == 3)) || ((tbl_Sol_Solicitud.Categoria_id == 7) && (tbl_Sol_Solicitud.Sub_Categoria_id == 4)))
            //{

            //    c1 = new PdfPCell(new Phrase("Diploma Obtenido", fntTitulo2));
            //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //    c1.Colspan = 1;
            //    tableDatosGenerales.AddCell(c1);
            //    c1 = new PdfPCell(new Phrase(diplomaobtenido, fntTituloTabla));
            //    c1.Colspan = 3;
            //    tableDatosGenerales.AddCell(c1);

            //    c1 = new PdfPCell(new Phrase("Código del Diploma", fntTitulo2));
            //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //    c1.Colspan = 1;
            //    tableDatosGenerales.AddCell(c1);
            //    c1 = new PdfPCell(new Phrase(nodediploma, fntTituloTabla));
            //    c1.Colspan = 1;
            //    tableDatosGenerales.AddCell(c1);

            //}

        }


        private void LlenaDatosEmpresaEntidadActividad(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_Actividad> tbl_Sol_Empresa_Entidad_Actividads = db.Tbl_Sol_Empresa_Entidad_Actividad.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Actividad_id).ToList();

            if (tbl_Sol_Empresa_Entidad_Actividads.Count() > 0)
            {
                LlenaBanner($"ACTIVIDADES DE LA EMPRESA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"ACTIVIDAD", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Empresa_Entidad_Actividads)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Actividad.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }
        private void LlenaDatosEmpresaEntidadMateriaPrima(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_Materia_Prima> tbl_Sol_Empresa_Entidad_Materia_Primas = db.Tbl_Sol_Empresa_Entidad_Materia_Prima.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Materia_Prima_id).ToList();
            if (tbl_Sol_Empresa_Entidad_Materia_Primas.Count() > 0)
            {
                if ((tbl_Sol_Solicitud.Sub_Categoria_id == 3) || (tbl_Sol_Solicitud.Sub_Categoria_id == 6))
                {
                    LlenaBanner($"PRODUCTOS");
                }
                else
                {
                    LlenaBanner($"MATERIA PRIMA UTILIZADA EN EL PROCESO DE PRODUCCIÓN");
                }

                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"PRODUCTO", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Empresa_Entidad_Materia_Primas)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Materia_Prima.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }
        private void LlenaDatosEmpresaEntidadMaquinariaUtilizada(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(4);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada> tbl_Sol_Empresa_Entidad_Maquinaria_Utilizadas = db.Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Maquinaria_Utilizada_id).ToList();
            if (tbl_Sol_Empresa_Entidad_Maquinaria_Utilizadas.Count() > 0)
            {
                LlenaBanner($"EQUIPO Y MAQUINARIA UTILIZADA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"EQUIPO - MAQUINARIA", fntTituloTabla));
                c1.Colspan = 3;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Empresa_Entidad_Maquinaria_Utilizadas)
                {

                    contador++;
                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Maquinaria_Utilizada.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }
        private void LlenaDatosEmpresaEntidadViveroForestal(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(8);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_Vivero_Forestal> tbl_Sol_Empresa_Entidad_Vivero_Forestals = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Vivero_Forestal_id).ToList();
            if (tbl_Sol_Empresa_Entidad_Vivero_Forestals.Count() > 0)
            {
                LlenaBanner($"PRINCIPALES ESPECIES EN PRODUCCIÓN");
                doc.Add(tableBanner);

                c1 = new PdfPCell(new Phrase($"No.", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Nombre Científico / Nombre Común", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Producción Anual de Plantas", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Procedencia de la Semilla", fntTituloTabla));
                c1.Colspan = 4;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Nota de Control de Semilla Certificada", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"(Finca, Municipio, Departamento, País)", fntTituloTabla));
                c1.Colspan = 3;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"No. RNF Origen", fntTituloTabla));
                c1.Colspan = 1;
                c1.Rowspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                int contador = 0;
                decimal sumatoria = 0;
                foreach (var item in tbl_Sol_Empresa_Entidad_Vivero_Forestals)
                {
                    contador++;

                    c1 = new PdfPCell(new Phrase($"{contador}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Tbl_Gral_Especie.NombreCientifico}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    sumatoria += item.Produccion_Anual_Plantas;
                    c1 = new PdfPCell(new Phrase($"{item.Produccion_Anual_Plantas}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Nombre_Finca}, {item.Tbl_Gral_Municipio.Municipio}, {item.Tbl_Gral_Departamento.Departamento}, {item.Tbl_Gral_Pais.Pais}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Codigo_RNF}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Numero_Nota_Control_Semilla_Certificada}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                }

                c1 = new PdfPCell(new Phrase($"Total", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                c1.Border = 0;
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{sumatoria}", fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"", fntTituloTabla));
                c1.Border = 0;
                c1.Colspan = 5;
                tablePersoneria.AddCell(c1);

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }
        private void LlenaDatosEmpresaEntidadMotosierraMarcaModelo(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(3);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Motosierra_Marca_Modelo> tbl_Sol_Motosierra_Marca_Modelos = db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Motosierra_id).ToList();
            if (tbl_Sol_Motosierra_Marca_Modelos.Count() > 0)
            {
                LlenaBanner($"MARCAS Y MODELOS DE LAS MOTOSIERRAS");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"Marca", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Modelo", fntTituloTabla));
                c1.Colspan = 2;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                foreach (var item in tbl_Sol_Motosierra_Marca_Modelos)
                {

                    c1 = new PdfPCell(new Phrase($"{item.Marca}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{item.Modelo}", fntTituloTabla));
                    c1.Colspan = 2;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosDeLaEmpresa(Tbl_Sol_Solicitud tbl_Sol_Solicitud, fc_Sol_Sel_Direccion_Result DireccionSolicitud)
        {
            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            string Direccion = "";
            string DireccionMovil = "";

            if ((DireccionSolicitud.Direccion != null) && (DireccionSolicitud.Direccion.Trim() != ""))
            {
                Direccion += DireccionSolicitud.Direccion.Trim() + ", ";
            }
            if ((DireccionSolicitud.Aldea != null) && (DireccionSolicitud.Aldea.Trim() != ""))
            {
                Direccion += "Aldea " + DireccionSolicitud.Aldea.Trim() + ", ";
            }
            Direccion += DireccionSolicitud.Municipio + ", " + DireccionSolicitud.Departamento;


            if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil != null) && (tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.Trim() != ""))
            {
                DireccionMovil = tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";
                DireccionMovil += tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio1.Municipio + ", " + tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento1.Departamento;
            }


            tablePersoneria = new PdfPTable(4);

            PdfPCell c1 = new PdfPCell();

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);

            if (tbl_Sol_Empresa_Entidad != null)
            {
                Tbl_Sol_Empresa_Entidad_Tipo_Registro tbl_Sol_Empresa_Entidad_Tipo_Registro = db.Tbl_Sol_Empresa_Entidad_Tipo_Registro.Find(tbl_Sol_Solicitud.Solicitud_id);

                //************************************************************************************************************************************

                c1 = new PdfPCell(new Phrase("Nombre Comercial: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Nombre}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                if (tbl_Sol_Empresa_Entidad.Objeto_Empresa != null && tbl_Sol_Empresa_Entidad.Objeto_Empresa.Trim() != "")
                {

                    c1 = new PdfPCell(new Phrase("Objeto de la Empresa: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Objeto_Empresa}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                c1 = new PdfPCell(new Phrase("Número de NIT: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.No_NIT}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Dirección de la Empresa Forestal: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase(Direccion, fntTituloTabla));
                //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Aldea}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio.Municipio}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
                c1.Colspan = 3;
                tablePersoneria.AddCell(c1);

                if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tipo_Industria_id == 2) || (tbl_Sol_Solicitud.SolicitudTipo_id >= 5 && tbl_Sol_Solicitud.SolicitudTipo_id <= 6 && tbl_Sol_Solicitud.Sub_Categoria_id == 3))
                {
                    c1 = new PdfPCell(new Phrase("Dirección de trabajo de la industria Forestal Movil: ", fntTituloTabla));
                    c1 = new PdfPCell(new Phrase("Dirección de funcionamiento: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(DireccionMovil, fntTituloTabla));
                    //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Aldea}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio.Municipio}, {tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento.Departamento}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);
                }


                if (tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria != null)
                {

                    c1 = new PdfPCell(new Phrase("Tipo de Industria: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombres_Comunes}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);

                }

                if (tbl_Sol_Solicitud.Categoria_id != 11)
                {
                if (tbl_Sol_Empresa_Entidad_Tipo_Registro != null)
                {
                    string fechaacta;
                    //1   Registro Mercantil
                    //2   REPEJU
                    //3   INACOP
                    //4   Comunal
                    //5   Municipal

                    // 1 Registro mercantil
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 1)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);



                        c1 = new PdfPCell(new Phrase("Número de folio: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Folio, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de libro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Libro, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                    }

                    // 2 REPEJU
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 2)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);



                        c1 = new PdfPCell(new Phrase("Número de folio: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Folio, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de libro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Libro, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Tipo de REPEJU: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_REPEJU_De.Descripcion, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                    }

                    // 3 INACOP
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 3)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Partida, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);

                    }

                    // 4 Comunal
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 4)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Acta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Fecha del acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);


                        if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta != null)
                        {
                            DateTime Fecha_Acta = (DateTime)tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta;
                            fechaacta = Fecha_Acta.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            fechaacta = "Debe indicar la fecha del acta";
                        }

                        c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }

                    // 5 Municipal
                    if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Tipo_Registro_Id == 5)
                    {

                        c1 = new PdfPCell(new Phrase("Tipo de registro: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad_Tipo_Registro.Tbl_Gral_Tipo_Registro.Tipo_Registro}", fntTituloTabla));
                        c1.Colspan = 3;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase("Número de acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(tbl_Sol_Empresa_Entidad_Tipo_Registro.No_Acta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);


                        c1 = new PdfPCell(new Phrase("Fecha del acta: ", fntTituloTabla));
                        c1.Colspan = 1;
                        c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                        tablePersoneria.AddCell(c1);


                        if (tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta != null)
                        {
                            DateTime Fecha_Acta = (DateTime)tbl_Sol_Empresa_Entidad_Tipo_Registro.Fecha_Acta;
                            fechaacta = Fecha_Acta.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            fechaacta = "Debe indicar la fecha del acta";
                        }

                        c1 = new PdfPCell(new Phrase(fechaacta, fntTituloTabla));
                        c1.Colspan = 1;
                        tablePersoneria.AddCell(c1);
                    }


                    c1 = new PdfPCell(new Phrase("Empresa Forestal a Registrar: ", fntTituloTabla));
                    c1.Colspan = 1;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);
                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion}", fntTituloTabla));
                    c1.Colspan = 3;
                    tablePersoneria.AddCell(c1);
                }
                }


                if (tbl_Sol_Solicitud.Sub_Categoria_id == 3 && tbl_Sol_Solicitud.Categoria_id != 11 )
                {

                    c1 = new PdfPCell(new Phrase("No. de Registro de Empresa Forestal Vinculada : ", fntTituloTabla));
                    c1.Colspan = 2;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.RNF_Inscripcion_Vinculada}", fntTituloTabla));
                    c1.Colspan = 2;
                    tablePersoneria.AddCell(c1);

                }




                c1 = new PdfPCell(new Phrase("Coordenadas GTM X: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_Sol_Empresa_Entidad.GTMX ?? 0).ToString("0"), fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase("Coordenadas GTM Y: ", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase((tbl_Sol_Empresa_Entidad.GTMY ?? 0).ToString("0"), fntTituloTabla));
                c1.Colspan = 1;
                tablePersoneria.AddCell(c1);

            }

        }

        private void LlenaTresTextos(string TextoA, string TextoB, string TextoC)
        {
            iTextSharp.text.Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10);
            iTextSharp.text.Font fntTablasCeldas = FontFactory.GetFont("HELVETICA", size: 10);

            tableTitulo = new PdfPTable(6);

            PdfPCell c1 = new PdfPCell();

            c1 = new PdfPCell(new Phrase(TextoA, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 2;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoB, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 1;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            c1 = new PdfPCell(new Phrase(TextoC, fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_LEFT;
            c1.VerticalAlignment = Element.ALIGN_CENTER;

            c1.Colspan = 3;
            c1.Rowspan = 1;
            c1.Border = 0;

            tableTitulo.AddCell(c1);


            return;
        }

        public JsonRespuesta GenerarFormularioDeSolicitud_PDF(long Solicitud_id)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;

            decimal[] tiposRegistro = new decimal[] { 0.00M, 0.01M, 0.02M, 0.03M };

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            string sqlQuery = "Exec SP_Sol_Enviar_Solicitud_CorregirTexto @Solicitud_id";

            SqlParameter[] sqlParams = new SqlParameter[]
            {
                   new SqlParameter { ParameterName = "@Solicitud_id",  Value = Solicitud_id, Direction = System.Data.ParameterDirection.Input }
            };


            db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return null;
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            decimal solicitudtipo = tbl_sol_Solicitud.SolicitudTipo_id - Math.Truncate(tbl_sol_Solicitud.SolicitudTipo_id);


            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = (from d in db.Tbl_Seg_UsuarioExterno
                                                             where d.Usuario_id == objUs.intUsuario_id
                                                             select d).FirstOrDefault();

            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();

            fc_Sol_Sel_Direccion_Result DireccionSolicitud = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();

            Personerias personerias = ObtenerPersonerias(Solicitud_id);
            int countPersonerias = personerias.PropietariosIndividuales.Count() + personerias.PropietariosJuridicos.Count() + personerias.RepresentatnteLegal.Count() + personerias.Mandatario.Count() + personerias.ArrendatariosIndividuales.Count() + personerias.ArrendatariosJuridicos.Count();

            strNombre = $"Formulario_{tbl_sol_Solicitud.Guid_id.ToString()}.pdf";

            strDirArchivo = strFolder + strNombre;

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }

            FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
            doc.Open();
            try
            {
                doc.Add(Enter);
                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlact = this.Url.Action();
                    LlenaBanner(urlact);
                    doc.Add(tableBanner);
                    doc.Add(Enter);
                }
                CrearBanner crearBanner = new CrearBanner();
                IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
                Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(10, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);

                Tbl_Gral_SolicitudConfiguracionTipo tbl_Gral_SolicitudConfiguracionTipo = (from d in db.Tbl_Gral_SolicitudConfiguracionTipo
                                                                                           where d.SolicitudTipo_id == tbl_sol_Solicitud.SolicitudTipo_id
                                                                                           select d).FirstOrDefault();

                string varTitulo = "" + tbl_Gral_SolicitudConfiguracionTipo.DescripcionEnOficio.ToUpper() + "";

                crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);

                string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();

                LlenaBanner(strFecha, "Derecha", "Blanco");
                doc.Add(tableBanner);

                string strNumero;

                doc.Add(Enter);

                if ((tbl_sol_Solicitud.Solicitud_NumeroExpediente != null) && (tbl_sol_Solicitud.Solicitud_NumeroExpediente != ""))
                {
                    strNumero = tbl_sol_Solicitud.Solicitud_NumeroExpediente;
                    LlenaTresTextos("", "", "Número de expediente : " + strNumero);
                    doc.Add(tableTitulo);
                }
                else
                {
                    strNumero = tbl_sol_Solicitud.Solicitud_NumeroTemporal;
                    LlenaTresTextos("", "", "Número temporal     :  " + strNumero);
                    doc.Add(tableTitulo);

                }

                if (tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf)
                {
                    if ((tbl_sol_Solicitud.Procedencia_Expediente != null) && (tbl_sol_Solicitud.Procedencia_Expediente.Trim() != ""))
                    {
                        var NumeroDeExpedienteOrigen = new Paragraph("Número de expediente de origen: " + tbl_sol_Solicitud.Procedencia_Expediente);
                        LlenaBanner("Número de expediente de origen: " + tbl_sol_Solicitud.Procedencia_Expediente, "Derecha", "Blanco");
                        doc.Add(tableBanner);
                    }
                }

                if ((tbl_sol_Solicitud.No_Registro != null) && (tbl_sol_Solicitud.No_Registro != ""))
                {

                    strNumero = tbl_sol_Solicitud.No_Registro;
                    LlenaTresTextos("", "", "Número de registro       : " + strNumero);
                    doc.Add(tableTitulo);


                }

                Constants constant = new Constants();

                LlenaCuatroTextos("Región: ", DireccionSolicitud.NoRegion + " " + constant.initCapPalabras(DireccionSolicitud.NombreRegion), "Sub Región:", DireccionSolicitud.NoSubRegion + " " + constant.initCapPalabras(DireccionSolicitud.NombreSubRegion));
                doc.Add(tableTitulo);
                doc.Add(Enter);

                string Modalidad = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First().Modalidad;

                string subModalidad = "";

                try
                {
                    subModalidad += db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id && Obj.Sub_Sub_Categoria_id == tbl_sol_Solicitud.Sub_Sub_Categoria_id).First().Modalidad;
                }
                catch (Exception ex)
                {
                    subModalidad = "";
                }


                Constants Const = new Constants();
                if ((Modalidad != "") && (Modalidad != " "))
                {
                    Modalidad = Const.initCapTexto(Modalidad);
                }
                else
                {
                    Modalidad = "";
                }

                #region Sub categoría o modalidad para profesional
                if (tbl_sol_Solicitud.Categoria_id == 7)
                {
                    if (Modalidad != "")
                    {
                        LlenaBanner(db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id).First().Modalidad_Categoria + " " + Modalidad, "Izquierda", "Blanco");
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }
                }
                #endregion



                if (countPersonerias > 0)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL PROPIETARIO");
                    intTexto_Romano = intTexto_Romano + 1;
                    doc.Add(tableBanner);
                    LlenaDatosSolicitante(Solicitud_id, personerias);
                    doc.Add(tableDatosGenerales);
                    doc.Add(Enter);
                }


                if (tbl_sol_Solicitud.Categoria_id == 7)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DEL SOLICITANTE");
                    intTexto_Romano = intTexto_Romano + 1;
                    doc.Add(tableBanner);
                    LlenaDatosProfesionalPersonales(tbl_sol_Solicitud);
                    doc.Add(tableDatosNotificacion);
                    doc.Add(Enter);

                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE UBICACIÓN");
                    intTexto_Romano = intTexto_Romano + 1;
                    doc.Add(tableBanner);
                    LlenaDatosProfesionalDireccion(tbl_sol_Solicitud);
                    doc.Add(tableDatosNotificacion);
                    doc.Add(Enter);


                }
                else
                {
                    if ((objUs.EsInterno == 1) && (tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf))
                    {
                    }
                    else
                    {

                        LlenaBanner($"{Texto_Romano[intTexto_Romano]}. DATOS DE NOTIFICACIÓN");
                        intTexto_Romano = intTexto_Romano + 1;
                        doc.Add(tableBanner);
                        LlenaDatosNotificacionDetallada(tbl_sol_Solicitud);
                        doc.Add(tableDatosNotificacion);
                        doc.Add(Enter);
                    }
                }

                //if ((Modalidad != "") && (tbl_sol_Solicitud.Categoria_id==4) && ((tbl_sol_Solicitud.Sub_Categoria_id == 1) || (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
                //{
                //    LlenaBanner("Sub-categoría : " + ' ' + Modalidad, "Izquierda", "Blanco");
                //    doc.Add(tableBanner);
                //    doc.Add(Enter);
                //}


                var Tbl_SolMotosiera = db.Tbl_Sol_Motosierra.Where(obj => obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).ToList();

                foreach (var itemMotosierra in Tbl_SolMotosiera)
                {
                    LlenaBanner(Texto_Romano[intTexto_Romano] + ". DATOS DE LA MOTOSIERRA");
                    intTexto_Romano = intTexto_Romano + 1;
                    doc.Add(tableBanner);
                    LlenaDatosDeLaMotosierra(itemMotosierra);
                    doc.Add(tablePersoneria);
                }


                if (tbl_sol_Solicitud.Categoria_id == 7)
                {
                    LlenaBanner($"{Texto_Romano[intTexto_Romano]}. FORMACION ACADEMICA");
                    intTexto_Romano = intTexto_Romano + 1;
                    doc.Add(tableBanner);
                    LlenaDatosProfesional(tbl_sol_Solicitud);
                    doc.Add(tableDatosGenerales);
                    doc.Add(Enter);

                }


                if ((tbl_sol_Solicitud.Categoria_id == 5) || (tbl_sol_Solicitud.Categoria_id == 9) || ((tbl_sol_Solicitud.Categoria_id == 8) && (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
                {

                    if (tbl_sol_Solicitud.Categoria_id == 9)
                    {
                        LlenaBanner($"{Texto_Romano[3]}. DATOS DE LA ENTIDAD");
                        doc.Add(tableBanner);
                    }

                    LlenaDatosDeLaEmpresa(tbl_sol_Solicitud, DireccionSolicitud);
                    doc.Add(tablePersoneria);
                    doc.Add(Enter);

                    LlenaDatosEmpresaEntidadActividad(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMaquinariaUtilizada(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMateriaPrima(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadViveroForestal(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadMotosierraMarcaModelo(doc, tbl_sol_Solicitud);

                    Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_sol_Solicitud.Solicitud_id);
                    if (tbl_Sol_Empresa_Entidad.CapacidadInstalada != null)
                    {
                        var CapacidadInstalada = new Paragraph("                  Capacidad instalada para producción mensual: " + tbl_Sol_Empresa_Entidad.CapacidadInstalada.ToString() + " metros cubicos.");
                        doc.Add(CapacidadInstalada);
                        doc.Add(Enter);
                    }

                }

                if ((tbl_sol_Solicitud.Categoria_id == 11))
                {
                    LlenaDatosDeLaEmpresa(tbl_sol_Solicitud, DireccionSolicitud);
                    doc.Add(tablePersoneria);
                    doc.Add(Enter);

                  
                    LlenaDatosEmpresaEntidadViveroForestal(doc, tbl_sol_Solicitud);

                }


                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    LlenaBanner("ReportePVController/GenerarPVJuridico_PDF  Datos de la Finca");
                    doc.Add(tableBanner);
                }

                for (int i = 0; i < tbl_Sol_Finca.Count(); i++)
                {
                    LlenaBanner($"{Texto_Romano[3]}. DATOS DE LA FINCA");
                    doc.Add(tableBanner);
                    //LlenaDatosFinca(tbl_Sol_Finca[i]);
                    //doc.Add(tableDatosFinca);
                    LlenaDatosDeLaFinca(tbl_Sol_Finca[i], tbl_sol_Solicitud);
                    doc.Add(tablePersoneria);
                    doc.Add(Enter);

                    //ClassCultivosEnAsocio

                    sqlQuery = " Select convert(varchar(10),Finca_id) +'. ' + \n";
                    sqlQuery += "        (Select NombreFinca  \n";
                    sqlQuery += "        From Tbl_Sol_Finca \n";
                    sqlQuery += "        Where Solicitud_id = Cultivo.Solicitud_id \n";
                    sqlQuery += "        and Finca_id = Cultivo.Finca_id) Finca, \n";
                    sqlQuery += "        convert(varchar(10),Rodal_id) Area, \n";
                    sqlQuery += "        (case when Tipo_de_Area = 1 then 'Rodal' else 'Arboles en línea' end) Tipo_De_Area, \n";
                    sqlQuery += "        (SElect Nombres_Comunes from Tbl_Gral_Cultivo Where Cultivo_id = Cultivo.Cultivo_id) Cultivo, \n";
                    sqlQuery += "        convert(varchar(10),Anio_Establecimiento) Anio_Establecimiento, \n";
                    sqlQuery += "        ISNULL(Volumen_Rodal,0) Volumen_Rodal, \n";
                    sqlQuery += "        ISNULL(Volumen_ha,0) Volumen_ha\n";
                    sqlQuery += "        From Tbl_Sol_Rodal_Cultivo Cultivo \n";
                    sqlQuery += $"       Where Cultivo.Solicitud_id = {tbl_sol_Solicitud.Solicitud_id} \n";
                    sqlQuery += $"         and Cultivo.Finca_id = {tbl_Sol_Finca[i].Finca_Id} \n";




                    List<ClassCultivosEnAsocio> Resultado = new List<ClassCultivosEnAsocio> { };

                    Resultado = db.Database.SqlQuery<ClassCultivosEnAsocio>(sqlQuery).ToList();


                    if (Resultado.Count() > 0)
                    {
                        LlenaBanner("CULTIVOS EN ASOCIO");
                        doc.Add(tableBanner);
                        LlenarCultivosEnAsocio(Solicitud_id, Resultado);
                        doc.Add(tableCultivosEnAsocio);
                        doc.Add(Enter);
                    }

                    /// fin Class

                    //if ((tbl_sol_Solicitud.No_Registro == null) && (!tbl_sol_Solicitud.Procedencia_PinpepNew && !tbl_sol_Solicitud.Procedencia_PinpepOld && !tbl_sol_Solicitud.Procedencia_Probosque && !tbl_sol_Solicitud.Procedencia_secorf))
                    {
                        LlenaBanner($"{Texto_Romano[4]}. DATOS DE LA PLANTACION");
                        doc.Add(tableBanner);
                        LlenaDatosPlantacion(tbl_Sol_Finca[i]);
                        doc.Add(tableDatosPlantacion);
                        if (tableDatosPlantacion.Rows.Count() > 0)
                        {
                            doc.Add(Enter);
                        }
                    }

                    if (tbl_sol_Solicitud.Categoria_id != 6)
                    {
                        EstimacionVolumen(tbl_Sol_Finca[i], Modalidad, subModalidad);
                        doc.Add(tableEstimacion);
                    }

                    if (tbl_sol_Solicitud.Categoria_id == 6)
                    {
                        EstimacionVolumenFS(tbl_Sol_Finca[i], Modalidad, subModalidad);
                        doc.Add(tableEstimacion);
                    }

                    //if (tbl_sol_Solicitud.Categoria_id == 6)
                    //{

                    //    DasometricosClase(tbl_Sol_Finca[i], 1);
                    //    doc.Add(tableEstimacion);
                    //    doc.Add(Enter);

                    //    DasometricosClase(tbl_Sol_Finca[i], 2);
                    //    doc.Add(tableEstimacion);
                    //    doc.Add(Enter);

                    //    DasometricosClase(tbl_Sol_Finca[i], 3);
                    //    doc.Add(tableEstimacion);
                    //    doc.Add(Enter);

                    //}

                }


                if (tbl_sol_Solicitud.Categoria_id == 6)
                {
                    ResumenPV_FS(tbl_sol_Solicitud.Solicitud_id);
                    doc.Add(tableEstimacion);
                }

                int cantTbl_Sol_Rodal_Dasometrico_Especie_Formula = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj => Obj.Solicitud_id == Solicitud_id).Count();

                if (cantTbl_Sol_Rodal_Dasometrico_Especie_Formula > 0)
                {
                    LlenaBanner($"{Texto_Romano[6]}. FORMULAS UTILIZADAS PARA EL CÁLCULO DE VOLUMEN POR ESPECIE");
                    doc.Add(tableBanner);
                    FormulasCalculoVolumen(Solicitud_id);
                    doc.Add(tableFormulas);
                }


                EspeciesForestales_Probosque(Solicitud_id);
                doc.Add(tableEstimacion);
                if (tableEstimacion.Rows.Count() > 0)
                {
                    doc.Add(Enter);
                }

                EspeciesForestales_PinpepOld(Solicitud_id);
                doc.Add(tableEstimacion);
                if (tableEstimacion.Rows.Count() > 0)
                {
                    doc.Add(Enter);
                }

                EspeciesProteger_PinpepOld(Solicitud_id);
                doc.Add(tableEstimacion);
                if (tableEstimacion.Rows.Count() > 0)
                {
                    doc.Add(Enter);
                }

                EspeciesForestales_Secorf(Solicitud_id);
                doc.Add(tableEstimacion);
                if (tableEstimacion.Rows.Count() > 0)
                {
                    doc.Add(Enter);
                }

                if ((tiposRegistro.Contains(solicitudtipo)) && tbl_sol_Solicitud.Procedencia_PinpepNew != true && tbl_sol_Solicitud.Procedencia_PinpepOld != true && tbl_sol_Solicitud.Procedencia_Probosque != true && tbl_sol_Solicitud.Procedencia_secorf != true)
                {
                    doc.Add(Enter);
                    LlenaBanner("Declaro y juro que he revisado los datos y que los mismos son correctos y que conozco de la pena correspondiente de perjurio.", "Justificado", "Blanco");
                    doc.Add(tableBanner);
                    //LlenaBanner("que conozco de la pena correspondiente de perjurio.            ", "Centro", "Blanco");
                    //doc.Add(tableBanner);
                    LlenaBanner("                                    Artículo 459 del Código Penal.", "Izquierda", "Blanco");
                    doc.Add(tableBanner);

                }
                else
                {
                    if (solicitudtipo == 0)
                    {
                        LlenaBanner("Todas las solicitudes de inscripción de oficio no llevarán firma del usuario excepto las inscripciones de plantaciones voluntarias y sistemas agroforestales según reglamento correspondiente.", "Izquierda", "Blanco");
                        doc.Add(tableBanner);

                    }
                    else if ((solicitudtipo == 0.01M) || (solicitudtipo == 0.02M))
                    {
                        doc.Add(Enter);
                        LlenaBanner("Declaro y juro que he revisado los datos y que los mismos son correctos y que conozco de la pena correspondiente de perjurio.", "Justificado", "Blanco");
                        doc.Add(tableBanner);
                        //LlenaBanner("que conozco de la pena correspondiente de perjurio.            ", "Centro", "Blanco");
                        //doc.Add(tableBanner);
                        LlenaBanner("                                    Artículo 459 del Código Penal.", "Izquierda", "Blanco");
                        doc.Add(tableBanner);
                    }

                }


                doc.Add(Enter);
                doc.Add(Enter);

                if (tbl_sol_Solicitud.Categoria_id == 7)
                {
                    FirmaSolicitanteExterno(tbl_Seg_UsuarioExterno);
                    doc.Add(tableFirmaSolicitante);
                }
                else if ((tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque || tbl_sol_Solicitud.Procedencia_secorf))
                {

                    if (solicitudtipo == 0)
                    {
                        if ((tbl_sol_Solicitud.Procedencia_PinpepNew || tbl_sol_Solicitud.Procedencia_PinpepOld || tbl_sol_Solicitud.Procedencia_Probosque))
                        {
                            FirmaSolicitante(Solicitud_id, personerias);
                            doc.Add(tableDatosGenerales);
                            doc.Add(Enter);
                            doc.Add(Enter);
                            NombreSession(objUs);
                            doc.Add(tableFirmaSolicitante);

                        }
                        else
                        {
                            NombreSession(objUs);
                            doc.Add(tableFirmaSolicitante);

                        }


                    }
                    else
                    {
                        FirmaSolicitante(Solicitud_id, personerias);
                        doc.Add(tableDatosGenerales);

                    }


                }
                else
                {
                    FirmaSolicitante(Solicitud_id, personerias);
                    doc.Add(tableDatosGenerales);
                }



                doc.Close();
                writer.Close();
                jsonRespuesta.Ubicacion = strNombre;
                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = "Ok";
            }
            catch (Exception ex)
            {
                doc.Close();
                writer.Close();
                jsonRespuesta.Ubicacion = null;
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = ex.Message;
            }

            return jsonRespuesta;
        }


        public ActionResult Index(long Solicitud_id)
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


            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Session = objUs;

            return View();
        }

        public ActionResult Fincas(long Solicitud_id)
        {
            List<Tbl_Sol_Finca> tbl_Sol_Finca = (from d in db.Tbl_Sol_Finca
                                                 where d.Solicitud_id == Solicitud_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();
            ViewBag.Solicitud_id = Solicitud_id;
            return View(tbl_Sol_Finca);
        }

        public ActionResult EstimacionVolumen(long Solicitud_id, long Finca_Id)
        {
            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(Solicitud_id, Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.LongitudMinima
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Solicitud_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_Id = Finca_Id;

            return View(validacionesDiametrica);

        }

        public ActionResult EstimacionVolumen_FuenteSemillera(long Solicitud_id, long Finca_Id)
        {
            List<fc_Sol_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_Sol_Sel_Rodal_ValidacionesDiametrica(Solicitud_id, Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.Clase
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_Sol_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Solicitud_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_Id = Finca_Id;

            return View(validacionesDiametrica);

        }


        public ActionResult Dasometricos_Rodales(long Solicitud_id, long Finca_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_id = Finca_id;

            List<Tbl_Sol_Rodal> lst = (from d in db.Tbl_Sol_Rodal
                                       where d.Solicitud_id == Solicitud_id && d.Finca_id == Finca_id
                                       select d).ToList();
            return View(lst);
        }

        public ActionResult Dasometricos_Clase(long Solicitud_id, long Finca_Id, long Rodal_id, int Clase_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Finca_Id = Finca_Id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Clase_id = Clase_id;
            List<Tbl_Sol_Rodal_Dasometrico> lst = (from d in db.Tbl_Sol_Rodal_Dasometrico
                                                   where d.Solicitud_id == Solicitud_id && d.Finca_id == Finca_Id && d.Rodal_id == Rodal_id && d.Clase_id == Clase_id
                                                   select d).ToList();

            return View(lst);

        }


        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Ubicacion { get; set; }
            public string Mensaje { get; set; }
        }

        [HttpPost]
        public JsonResult GenerarFormularioDeSolicitud(long Solicitud_id, string firma)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            string TextoMostrar, Ubicacion;
            jsonRespuesta.Result = 0;
            jsonRespuesta.Mensaje = "No se ha realizado ninguna operación";

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            { 
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = "Error: Acceso denegado";

                return Json(JsonConvert.SerializeObject(jsonRespuesta));

            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = "Error: Acceso denegado";

                return Json(JsonConvert.SerializeObject(jsonRespuesta));
            }

            try
            {
                JsonRespuesta Archivo = GenerarFormularioDeSolicitud_PDF(Solicitud_id);   //GenerarPVJuridico_PDF
                if (Archivo.Result == 1)
                {
                    jsonRespuesta.Result = 1;
                    jsonRespuesta.Ubicacion = Archivo.Ubicacion;
                    jsonRespuesta.Mensaje = "Documento generado con éxito";
                }
                else
                {
                    jsonRespuesta = Archivo;
                }
            }
            catch (Exception ex)
            {
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = "Error: " + ex.Message;
            }
            return Json(JsonConvert.SerializeObject(jsonRespuesta));
        }


    }
}
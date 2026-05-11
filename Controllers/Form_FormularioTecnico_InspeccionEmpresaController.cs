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
    public class Form_FormularioTecnico_InspeccionEmpresaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);

        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Ubicacion { get; set; }
            public string Mensaje { get; set; }
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


        private void LlenaBannerTransparenteBorder_1(String Leyenda)
        {

            Leyenda = "B";

            tableBanner = new PdfPTable(1);

            var FontColour = new BaseColor(255, 255, 255);

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 20, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
            c1.Rowspan = 3;
            tableBanner.AddCell(c1);

            return;
        }


        private PdfPTable tableBanner = new PdfPTable(1);
        private PdfPTable tablePersoneria = new PdfPTable(1);


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

        private void LlenaDatosDeLaEmpresa(Tbl_Sol_Solicitud tbl_Sol_Solicitud, fc_Sol_Sel_Direccion_Result DireccionSolicitud)
        {
            Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_Sol_Solicitud.Solicitud_id);
            string Direccion = "";
            string DireccionMovil = "";

            if ((DireccionSolicitud.Direccion.Trim() != "")|| (DireccionSolicitud.Direccion != null))
            {
                Direccion += DireccionSolicitud.Direccion.Trim() + ", ";
            }
            if (DireccionSolicitud.Aldea.Trim() != "")
            {
                Direccion += "Aldea " + DireccionSolicitud.Aldea.Trim() + ", ";
            }
            Direccion += DireccionSolicitud.Municipio + ", " + DireccionSolicitud.Departamento;


            if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil ?? "").Trim() != "")
            {
                DireccionMovil = tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.DireccionEmpresaMovil.ToString().Trim() + ", ";
            }

            if ((tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Municipio1 != null) && (tbl_Sol_Solicitud.Tbl_Sol_Empresa_Entidad.Tbl_Gral_Departamento1 != null))
            {
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

                //if (tbl_Sol_Empresa_Entidad.Objeto_Empresa != null && tbl_Sol_Empresa_Entidad.Objeto_Empresa.Trim() != "")
                //{

                //    c1 = new PdfPCell(new Phrase("Objeto de la Empresa: ", fntTituloTabla));
                //    c1.Colspan = 1;
                //    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                //    tablePersoneria.AddCell(c1);

                //    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.Objeto_Empresa}", fntTituloTabla));
                //    c1.Colspan = 3;
                //    tablePersoneria.AddCell(c1);

                //}

                //c1 = new PdfPCell(new Phrase("Número de NIT: ", fntTituloTabla));
                //c1.Colspan = 1;
                //c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                //tablePersoneria.AddCell(c1);

                //c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.No_NIT}", fntTituloTabla));
                //c1.Colspan = 3;
                //tablePersoneria.AddCell(c1);

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

                if (tbl_Sol_Solicitud.Sub_Categoria_id == 3)
                {
                    c1 = new PdfPCell(new Phrase("No. de Registro de Empresa Forestal Vinculada : ", fntTituloTabla));
                    c1.Colspan = 2;
                    c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{tbl_Sol_Empresa_Entidad.RNF_Inscripcion_Vinculada}", fntTituloTabla));
                    c1.Colspan = 2;
                    tablePersoneria.AddCell(c1);
                }
            }

        }

        public class Entidad_Actividad
        {
            public string Actividad_Usuario { get; set; }
            public string Actividad_Tecnico { get; set; }
        }

        private IEnumerable<Entidad_Actividad> ActividadesXSolicitud(long solicitud_id)
        {
            string strsolicitud_id = solicitud_id.ToString();

            string sqlQuery;


            sqlQuery = " Select ";
            sqlQuery += " isnull(( ";
            sqlQuery += " Select dbo.Fnc_Gral_ActividadNombre(Actividad_id) ";
            sqlQuery += " From Tbl_Sol_Empresa_Entidad_Actividad ";
            sqlQuery += " Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += " and Actividad_id = EA.Actividad_Id ";
            sqlQuery += " ),'') Actividad_Usuario, ";
            sqlQuery += " isnull(( ";
            sqlQuery += " Select dbo.Fnc_Gral_ActividadNombre(Actividad_id) ";
            sqlQuery += " From Tbl_Sol_Empresa_Entidad_TecnicoActividad ";
            sqlQuery += " Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += " and Actividad_id = EA.Actividad_Id ";
            sqlQuery += " ),'') Actividad_Tecnico ";
            sqlQuery += " From Tbl_Gral_Actividad EA ";
            sqlQuery += " Where Actividad_id in ";
            sqlQuery += " ( ";
            sqlQuery += " Select Actividad_id ";
            sqlQuery += " From Tbl_Sol_Empresa_Entidad_Actividad ";
            sqlQuery += " Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += " ) ";
            sqlQuery += " or Actividad_id in ";
            sqlQuery += " ( ";
            sqlQuery += " Select Actividad_id ";
            sqlQuery += " From Tbl_Sol_Empresa_Entidad_TecnicoActividad ";
            sqlQuery += " Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += " ) ";

            List<Entidad_Actividad> Resultado = new List<Entidad_Actividad> { };

            Resultado = db.Database.SqlQuery<Entidad_Actividad>(sqlQuery).ToList();

            return Resultado;
        }

        public class Entidad_MateriaPrima
        {
            public string MateriaPrima_Usuario { get; set; }
            public string MateriaPrima_Tecnico { get; set; }
        }

        private IEnumerable<Entidad_MateriaPrima> MateriaPrimaXSolicitud(long solicitud_id)
        {
            string strsolicitud_id = solicitud_id.ToString();

            string sqlQuery;

            sqlQuery = " Select ";
            sqlQuery += " isnull(( ";
            sqlQuery += "     Select dbo.Fnc_Gral_MateriaPrimaNombre(Materia_Prima_id) ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_Materia_Prima ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "     and Materia_Prima_id = EA.Materia_Prima_id ";
            sqlQuery += " ),'') MateriaPrima_Usuario, ";
            sqlQuery += " isnull(( ";
            sqlQuery += "     Select dbo.Fnc_Gral_MateriaPrimaNombre(Materia_Prima_id) ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "     and Materia_Prima_id = EA.Materia_Prima_id ";
            sqlQuery += " ),'') MateriaPrima_Tecnico ";
            sqlQuery += " From Tbl_Gral_Materia_Prima EA ";
            sqlQuery += " Where Materia_Prima_id in ";
            sqlQuery += "    ( ";
            sqlQuery += "     Select Materia_Prima_id ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_Materia_Prima ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "    ) ";
            sqlQuery += " or Materia_Prima_id in ";
            sqlQuery += "    ( ";
            sqlQuery += "         Select Materia_Prima_id ";
            sqlQuery += "         From Tbl_Sol_Empresa_Entidad_TecnicoMateria_Prima ";
            sqlQuery += "         Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "    ) ";


            List<Entidad_MateriaPrima> Resultado = new List<Entidad_MateriaPrima> { };

            Resultado = db.Database.SqlQuery<Entidad_MateriaPrima>(sqlQuery).ToList();

            return Resultado;
        }

        public class Entidad_Maquinaria
        {
            public string Maquinaria_Usuario { get; set; }
            public string Maquinaria_Tecnico { get; set; }
        }

        private IEnumerable<Entidad_Maquinaria> MaquinariaXSolicitud(long solicitud_id)
        {
            string strsolicitud_id = solicitud_id.ToString();

            string sqlQuery;

            sqlQuery = " Select ";
            sqlQuery += " isnull(( ";
            sqlQuery += "     Select dbo.Fnc_Gral_MaquinariaNombre(Maquinaria_Utilizada_Id) ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "     and Maquinaria_Utilizada_Id = EA.Maquinaria_Utilizada_Id ";
            sqlQuery += " ),'') Maquinaria_Usuario, ";
            sqlQuery += " isnull(( ";
            sqlQuery += "     Select dbo.Fnc_Gral_MaquinariaNombre(Maquinaria_Utilizada_Id) ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "     and Maquinaria_Utilizada_Id = EA.Maquinaria_Utilizada_Id ";
            sqlQuery += " ),'') Maquinaria_Tecnico ";
            sqlQuery += " From Tbl_Gral_Maquinaria_Utilizada EA ";
            sqlQuery += " Where Maquinaria_Utilizada_Id in ";
            sqlQuery += "    ( ";
            sqlQuery += "     Select Maquinaria_Utilizada_Id ";
            sqlQuery += "     From Tbl_Sol_Empresa_Entidad_Maquinaria_Utilizada ";
            sqlQuery += "     Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "    ) ";
            sqlQuery += " or Maquinaria_Utilizada_Id in ";
            sqlQuery += "    ( ";
            sqlQuery += "         Select Maquinaria_Utilizada_Id ";
            sqlQuery += "         From Tbl_Sol_Empresa_Entidad_TecnicoMaquinaria_Utilizada ";
            sqlQuery += "         Where Solicitud_id = " + strsolicitud_id;
            sqlQuery += "    ) ";


            List<Entidad_Maquinaria> Resultado = new List<Entidad_Maquinaria> { };

            Resultado = db.Database.SqlQuery<Entidad_Maquinaria>(sqlQuery).ToList();

            return Resultado;
        }


        private void LlenaDatosEmpresaEntidadActividad(Document doc, IEnumerable<Entidad_Actividad> Actividades)
        {
            tablePersoneria = new PdfPTable(2);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");


            if (Actividades.Count() > 0)
            {
                LlenaBanner($"ACTIVIDADES DE LA EMPRESA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"Actividad ingresada por el usuario", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Actividad ingresada por el técnico", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);


                foreach (Entidad_Actividad itemactividad in Actividades)
                {
                    c1 = new PdfPCell(new Phrase($"{itemactividad.Actividad_Usuario}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{itemactividad.Actividad_Tecnico}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadMateriaPrima(Document doc, IEnumerable<Entidad_MateriaPrima> MateriaPrima)
        {
            tablePersoneria = new PdfPTable(2);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");


            if (MateriaPrima.Count() > 0)
            {
                LlenaBanner($"MATERIA PRIMA UTILIZAD POR LA EMPRESA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"Materia prima ingresada por el usuario", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Actividad ingresada por el técnico", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);


                foreach (Entidad_MateriaPrima itemactividad in MateriaPrima)
                {
                    c1 = new PdfPCell(new Phrase($"{itemactividad.MateriaPrima_Usuario}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{itemactividad.MateriaPrima_Tecnico}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                }

                doc.Add(tablePersoneria);
                doc.Add(Enter);
            }

        }

        private void LlenaDatosEmpresaEntidadMaquinaria(Document doc, IEnumerable<Entidad_Maquinaria> Maquinaria)
        {
            tablePersoneria = new PdfPTable(2);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");


            if (Maquinaria.Count() > 0)
            {
                LlenaBanner($"MATERIA PRIMA UTILIZAD POR LA EMPRESA");
                doc.Add(tableBanner);

                int contador = 0;

                c1 = new PdfPCell(new Phrase($"Maquinaria ingresada por el usuario", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);

                c1 = new PdfPCell(new Phrase($"Maquinaria ingresada por el técnico", fntTituloTabla));
                c1.Colspan = 1;
                c1.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                tablePersoneria.AddCell(c1);


                foreach (Entidad_Maquinaria itemMaquinaria in Maquinaria)
                {
                    c1 = new PdfPCell(new Phrase($"{itemMaquinaria.Maquinaria_Usuario}", fntTituloTabla));
                    c1.Colspan = 1;
                    tablePersoneria.AddCell(c1);

                    c1 = new PdfPCell(new Phrase($"{itemMaquinaria.Maquinaria_Tecnico}", fntTituloTabla));
                    c1.Colspan = 1;
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
                LlenaBanner($"PRINCIPALES ESPECIES EN PRODUCCIÓN INGRESADAS POR EL USUARIO");
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

                c1 = new PdfPCell(new Phrase($"Finca, Municipio, Departamento, País", fntTituloTabla));
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


        private void LlenaDatosEmpresaEntidadViveroForestalTecnico(Document doc, Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {
            tablePersoneria = new PdfPTable(8);
            PdfPCell c1 = new PdfPCell();
            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, Font.NORMAL);
            var Enter = new Paragraph(" ");

            List<Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal> tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals = db.Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).OrderBy(Obj => Obj.Vivero_Forestal_id).ToList();
            if (tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals.Count() > 0)
            {
                LlenaBanner($"PRINCIPALES ESPECIES EN PRODUCCIÓN INGRESADAS POR EL TÉCNICO");
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

                c1 = new PdfPCell(new Phrase($"Finca, Municipio, Departamento, País", fntTituloTabla));
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
                foreach (var item in tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestals)
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





        public JsonRespuesta GenerarFormularioDeInspeccionEntidad_PDF(long Solicitud_id)
        {

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            string strDir = "Documentos\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            int intImprimir = 0;

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

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            strNombre = $"FormularioTecnicoEntidad_{tbl_sol_Solicitud.Guid_id.ToString()}.pdf";

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
                //Result_SP_IdentificadorOficialGestion resultsp = new Result_SP_IdentificadorOficialGestion();

                                                         
                Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerDatosDocumentos(17, tbl_sol_Solicitud.Solicitud_id, 1, 1, 1, objUs.intUsuario_id);


                string varTitulo = "BOLETA DE INSPECCIÓN PARA INSCRIPCIÓN DE EMPRESAS FORESTALES";

                //resultsp.Codigo = "----";
                //resultsp.Version = "----";
                //resultsp.strFecha = "----";

                crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                doc.Add(crearBanner.tableTitulo);
                doc.Add(Enter);


                fc_Sol_Sel_Direccion_Result DireccionSolicitud = db.fc_Sol_Sel_Direccion(tbl_sol_Solicitud.Solicitud_id).FirstOrDefault();


                if ((tbl_sol_Solicitud.Categoria_id == 5) || (tbl_sol_Solicitud.Categoria_id == 11) || ((tbl_sol_Solicitud.Categoria_id == 8) && (tbl_sol_Solicitud.Sub_Categoria_id == 2)))
                {
                    Tbl_Sol_Empresa_Entidad tbl_Sol_Empresa_Entidad = db.Tbl_Sol_Empresa_Entidad.Find(tbl_sol_Solicitud.Solicitud_id);

                    string strNumero = tbl_sol_Solicitud.Solicitud_NumeroExpediente;
                    LlenaBanner("Número de expediente : " + strNumero, "Derecha", "Blanco");
                    doc.Add(tableBanner);

                    LlenaBanner("Fecha de inspección :  ______________________", "Derecha", "Blanco");
                    doc.Add(tableBanner);
                    doc.Add(Enter);

                    if (tbl_sol_Solicitud.Sub_Categoria_id == 1)
                    {
                        LlenaBanner("Subcategoría                                         Observaciones del técnico forestal", "Derecha", "Blanco");
                        doc.Add(tableBanner);
                        string tipoindustria = "";
                        try
                        {
                            tipoindustria = (tbl_Sol_Empresa_Entidad.Tbl_Gral_Tipo_Industria.Nombres_Comunes ?? "");
                        }
                        catch { tipoindustria = ""; }
                        LlenaBanner(tbl_sol_Solicitud.Tbl_Sol_Solicitud_Sub_Categoria.Descripcion + "        " + tipoindustria + "       SI  ___________________________", "Derecha", "Blanco");
                        doc.Add(tableBanner);
                        LlenaBanner("  NO ___________________________", "Derecha", "Blanco");
                        doc.Add(tableBanner);
                        doc.Add(Enter);

                    }

                    LlenaBanner("DATOS DE LA EMPRESA");
                    doc.Add(tableBanner);

                    LlenaDatosDeLaEmpresa(tbl_sol_Solicitud, DireccionSolicitud);
                    doc.Add(tablePersoneria);
                    doc.Add(Enter);


                    string UbicacionGeografica = "Ubicación geográfica:                                                Dato del técnico forestal";
                    LlenaBanner(UbicacionGeografica, "Izquierda", "Blanco");
                    doc.Add(tableBanner);

                    string GTMX_Cliente = (tbl_Sol_Empresa_Entidad.GTMX ?? 0).ToString("0");
                    string GTMY_Cliente = (tbl_Sol_Empresa_Entidad.GTMY ?? 0).ToString("0");

                    string GTMX_Tecnico = (tbl_Sol_Empresa_Entidad.GTMX_Tecnico ?? 0).ToString("0");
                    string GTMY_Tecnico = (tbl_Sol_Empresa_Entidad.GTMY_Tecnico ?? 0).ToString("0");

                    if (GTMX_Tecnico == "0")
                    {
                        GTMX_Tecnico = "___________________";
                    }
                    if (GTMY_Tecnico == "0")
                    {
                        GTMY_Tecnico = "___________________";
                    }

                    string CoordendaX = "         Coordenada GTM X          " + GTMX_Cliente + "               " + GTMX_Tecnico;
                    LlenaBanner(CoordendaX, "Izquierda", "Blanco");
                    doc.Add(tableBanner);
                    string CoordendaY = "         Coordenada GTM Y       " + GTMY_Cliente + "               " + GTMY_Tecnico;
                    LlenaBanner(CoordendaY, "Izquierda", "Blanco");
                    doc.Add(tableBanner);
                    doc.Add(Enter);

                    LlenaBanner("Observaciones del técnico forestal");
                    doc.Add(tableBanner);
                    LlenaBannerTransparenteBorder_1(" ...");
                    doc.Add(tableBanner);
                    LlenaBannerTransparenteBorder_1(" ...");
                    doc.Add(tableBanner);


                    IEnumerable<Entidad_Actividad> ActividadesPorSolicitud = ActividadesXSolicitud(Solicitud_id);

                    if (ActividadesPorSolicitud.Count() > 0)
                    {
                        doc.Add(Enter);
                        LlenaDatosEmpresaEntidadActividad(doc, ActividadesPorSolicitud);
                        intImprimir = intImprimir + 1;

                        doc.Add(Enter);
                        LlenaBanner("Observaciones del técnico forestal");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);

                    }


                    IEnumerable<Entidad_MateriaPrima> MateriaPorSolicitud = MateriaPrimaXSolicitud(Solicitud_id);
                    if (MateriaPorSolicitud.Count() > 0)
                    {
                        doc.Add(Enter);
                        LlenaDatosEmpresaEntidadMateriaPrima(doc, MateriaPorSolicitud);
                        intImprimir = intImprimir + 1;

                        doc.Add(Enter);
                        LlenaBanner("Observaciones del técnico forestal");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);

                    }

                    IEnumerable<Entidad_Maquinaria> MaquinariaPorSolicitud = MaquinariaXSolicitud(Solicitud_id);
                    if (MaquinariaPorSolicitud.Count() > 0)
                    {
                        doc.Add(Enter);
                        LlenaDatosEmpresaEntidadMaquinaria(doc, MaquinariaPorSolicitud);
                        intImprimir = intImprimir + 1;

                        doc.Add(Enter);
                        LlenaBanner("Observaciones del técnico forestal");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);


                    }


                    LlenaDatosEmpresaEntidadViveroForestal(doc, tbl_sol_Solicitud);

                    LlenaDatosEmpresaEntidadViveroForestalTecnico(doc, tbl_sol_Solicitud);

                    int Count_Vivero_Forestals = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).Count();
                    int Count_Vivero_ForestalsTecnico = db.Tbl_Sol_Empresa_Entidad_TecnicoVivero_Forestal.Where(Obj => Obj.Solicitud_id == tbl_sol_Solicitud.Solicitud_id).Count();

                    if ((Count_Vivero_Forestals + Count_Vivero_ForestalsTecnico) > 0)
                    {
                        intImprimir = intImprimir + 1;
                    }

                    if (intImprimir > 0)
                    {
                        doc.Add(Enter);
                        LlenaBanner("Observaciones adicionales del técnico forestal");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);
                        LlenaBannerTransparenteBorder_1(" ...");
                        doc.Add(tableBanner);
                    }

                    doc.Add(Enter);

                    string strfirma = "  f.___________________________________________  ";
                    LlenaBanner(strfirma, "Centro", "Blanco");
                    doc.Add(tableBanner);

                    strfirma = "   " + objUs.strNombre_Usuario + "    ";
                    LlenaBanner(strfirma, "Centro", "Blanco");

                    doc.Add(tableBanner);

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

        [HttpPost]
        public JsonResult GenerarFormularioDeInspeccionEntidad(long Solicitud_id, string firma)
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
                JsonRespuesta Archivo = GenerarFormularioDeInspeccionEntidad_PDF(Solicitud_id);   //GenerarPVJuridico_PDF
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

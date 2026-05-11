using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using RNF_Web.Models;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Form_FormularioRegionalOficioDenegacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
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

        // GET: Form_FormularioRegionalOficioDenegacion
        public ActionResult Index(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            ViewBag.Solicitud_id = tbl_sol_Solicitud.Solicitud_id;
            ViewBag.Etapa_id = etapa_id;
            ViewBag.EtapaRuta_id = etaparuta_id;
            ViewBag.CorrelativoEtapa_id = correlativoetapa_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
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



            if (ConfirmarRespuesta(solicitud_id, etapa_id, etaparuta_id, correlativoetapa_id, motivo, respuestaid) == 1)
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

        public ActionResult GenerarEnmiendas(long Solicitud_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            return View();
        }

        public ActionResult ListarEnmiendas(long Solicitud_id)
        {
            List<Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas> lst = (from d in db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas
                                                                                        where d.Solicitud_id == Solicitud_id
                                                                                        orderby d.Estado_id descending, d.Enmienda_id
                                                                                        select d).ToList();
            return View(lst);
        }

        public ActionResult Oficio_Registro_NoProcedente(long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, string GuidEtapa_id)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsinterno = false;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno != 0)
            {
                boolEsinterno = true;
            }


            ViewBag.Solicitud_id = Solicitud_id;
            ViewBag.Etapa_id = Etapa_id;
            ViewBag.EtapaRuta_id = EtapaRuta_id;
            ViewBag.CorrelativoEtapa_id = CorrelativoEtapa_id;
            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ListaParrafos listaParrafos = new ListaParrafos();
            Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente = db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Find(Solicitud_id);

            if (tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente == null)
            {
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente = new Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente();
                listaParrafos = GetParrafos(Solicitud_id);
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Solicitud_id = Solicitud_id;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Parrafo_1 = listaParrafos.Parrafo1;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Parrafo_2 = listaParrafos.Parrafo2;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Parrafo_3 = listaParrafos.Parrafo3;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.swdatecreated = swdatecreated;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.swcraetedbyinterno = boolEsinterno;
                tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.swcreatedby = objUs.intUsuario_id;
                db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Add(tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente);
                db.SaveChanges();
            }

            return View(tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente);
        }


        public class ParametrosMetodos
        {
            public long Solicitud_id { get; set; }
            public int Etapa_id { get; set; }
            public decimal EtapaRuta_id { get; set; }
            public int CorrelativoEtapa_id { get; set; }
            public long Usuario_id { get; set; }
            public int GenerarDocumento { get; set; }
        }
        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }
        public class ListaParrafos
        {
            public string Parrafo1 { get; set; }
            public string Parrafo2 { get; set; }
            public string Parrafo3 { get; set; }
            public string Parrafo4 { get; set; }
            public string Parrafo5 { get; set; }
            public string Parrafo6 { get; set; }
            public string Parrafo7 { get; set; }
        }

        private ListaParrafos GetParrafos(long Solicitud_id)
        {
            ListaParrafos listaParrafos = new ListaParrafos();
            string query;
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Parrafo]('" + Solicitud_id + "','" + 1 + "')";
            listaParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Parrafo]('" + Solicitud_id + "','" + 2 + "')";
            listaParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Parrafo]('" + Solicitud_id + "','" + 3 + "')";
            listaParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();

            return listaParrafos;
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

            c1 = new PdfPCell(new Phrase("\n OFICIO DE REGISTRO NO PROCEDENTE \n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("RF-RE-033", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("1", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Septiembre 2022", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO:REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
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



        public string GenerarDocumentoDenegacion_Regional(ParametrosMetodos parametros, Usuario objUs)
        {
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(parametros.Solicitud_id);
            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                               where d.Solicitud_id == parametros.Solicitud_id
                                                               && d.Etapa_id == parametros.Etapa_id
                                                               && d.EtapaRuta_id == parametros.EtapaRuta_id
                                                               && d.CorrelativoEtapa_id == parametros.CorrelativoEtapa_id
                                                               select d).FirstOrDefault();
            List<Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas> Enmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas
                                                                                              where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                              && d.Estado_id == true
                                                                                              orderby d.Enmienda_id
                                                                                              select d).ToList();

            Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente ParrafosOficioRegistro = (from d in db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente
                                                                                           where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                           select d).FirstOrDefault();
            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            CrearBanner crearBanner = new CrearBanner();
            Result_SP_IdentificadorOficialGestion resultsp = identificadorOficialGestion.ObtenerNumeroOficio(14, tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();
            string strDirectorRegional = db.Database.SqlQuery<string>("SELECT dbo.[Fnc_Gral_NombreDirectorRegional](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();
            string strSubDirectorRegional = db.Database.SqlQuery<string>("SELECT dbo.[Fnc_Gral_NombreSubDirectorRegional](@p0,@p1)", tbl_Sol_Solicitud.Region_id, tbl_Sol_Solicitud.SubRegion_id).FirstOrDefault();

            string personeriaConIdentificacion = db.Database.SqlQuery<string>($"select [dbo].[Fnc_Sol_Sel_Personeria_y_Documentos]('{tbl_Sol_Solicitud.Solicitud_id}')").FirstOrDefault() ?? "";
            string personeria = db.Database.SqlQuery<string>($"select [dbo].[Fnc_Sol_Sel_Personeria]('{tbl_Sol_Solicitud.Solicitud_id}')").FirstOrDefault() ?? "";


            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);
            var Enter = new Paragraph(" ");
            strNombre = $"U{tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id}.pdf";
            strDirArchivo = strFolder + strNombre;
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
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


            string varTitulo = "OFICIO DE REGISTRO NO PROCEDENTE";
            crearBanner.LlenaTituloRevision(varTitulo, resultsp.Codigo, resultsp.Version, resultsp.strFecha, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
            doc.Add(crearBanner.tableTitulo);
            doc.Add(Enter);

            LlenaBanner("Oficio No. " + resultsp.Identificador, "Derecha", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(strFecha, "Derecha", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Ing. " + strSubDirectorRegional, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("Subregión " + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.Nombre_SubRegion ?? "" + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.No_SubRegion ?? "", "Izquierda", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("Instituto Nacional de Bosques -INAB-", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Estimado Ing. " + strSubDirectorRegional, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Reciba un atento y cordial saludo, por parte de la Direccion Regional " + tbl_Sol_Solicitud.Tbl_Gral_Region.Nombre_Region ?? "" + tbl_Sol_Solicitud.Tbl_Gral_Region.No_Region ?? "" + ", INAB " + tbl_Sol_Solicitud.Tbl_Gral_SubRegion.Direccion + ".", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);


            LlenaBanner(ParrafosOficioRegistro.Parrafo_1, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("De lo anterior se le informa que se procedió a revisar el expediente administrativo, así como el sistema Electrónico del Registro Nacional Forestal, encontrando lo siguiente:", "Izquierda", "Blanco");
            doc.Add(tableBanner);
            //Inicia parte de Enmienda
            if (Enmiendas.Count() > 0)
            {
                int countEnmiendas = 0;
                foreach (var item in Enmiendas)
                {
                    countEnmiendas++;
                    LlenaBanner(countEnmiendas + ". " + item.Enmienda, "Izquierda", "Blanco");
                    doc.Add(tableBanner);
                }
            }           

            //Finaliza parte de Enmiendas

            doc.Add(Enter);

            LlenaBanner(ParrafosOficioRegistro.Parrafo_2, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner(ParrafosOficioRegistro.Parrafo_3, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);



            LlenaBanner("Agradeciendo se sirva atender lo solicitado.", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            LlenaBanner("Atentamente,", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);
            doc.Add(Enter);

            LlenaBanner(strDirectorRegional, "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("Director Regional", "Centro", "Blanco");
            doc.Add(tableBanner);

            doc.Close();
            writer.Close();
            return strNombre;
        }
        public JsonResult ObtenerDocumentoDenegacion_Regional(ParametrosMetodos parametros)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(parametros.Solicitud_id);

            List<Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas> Enmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas
                                                                                              where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                                              && d.Estado_id == true
                                                                                              orderby d.Enmienda_id
                                                                                              select d).ToList();

            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.Result = 0;
            jsonRespuesta.Mensaje = "No se ha realizado ninguna gestión";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                jsonRespuesta.Result = 3;
                jsonRespuesta.Mensaje = "No posee una sesión válida";
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
                parametros.Usuario_id = objUs.intUsuario_id;
            }

          if (Enmiendas.Count() > 0)
            {
                
            if (parametros.GenerarDocumento == 1)
            {
                try
                {
                    jsonRespuesta.Ubicacion = GenerarDocumentoDenegacion_Regional(parametros, objUs);
                    jsonRespuesta.Result = 1;
                    jsonRespuesta.Mensaje = "Generación de documento realizada con éxito";

                    Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == parametros.Solicitud_id && Obj.EtapaRuta_id == parametros.EtapaRuta_id && Obj.Etapa_id == parametros.Etapa_id && Obj.CorrelativoEtapa_id == parametros.CorrelativoEtapa_id).First();

                    tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = jsonRespuesta.Ubicacion;

                    db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    jsonRespuesta.Result = 2;
                    jsonRespuesta.Mensaje = "Hubo un error: " + ex.Message;
                }
            }
            else
            {
                jsonRespuesta.Result = 4;
                jsonRespuesta.Mensaje = "No se ha generado documento";
            }

          }
            else
                {
                    jsonRespuesta.Result = 4;
                    jsonRespuesta.Mensaje = "Debe ingresar las observaciones correspondientes.";
                }

            
            return Json(jsonRespuesta);
        }


        public JsonResult ObtenerParrafos_Oficio_Registro_NoProcedente(Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente Datos_Oficio)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsinterno = false;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.Result = 0;
            jsonRespuesta.Mensaje = "No se ha realizado ninguna gestión";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                jsonRespuesta.Result = 3;
                jsonRespuesta.Mensaje = "No posee una sesión válida";
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno != 0)
            {
                boolEsinterno = true;
            }




            Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente OficioNoProcedente = db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Find(Datos_Oficio.Solicitud_id);
            ListaParrafos listaParrafos = new ListaParrafos();
            if (OficioNoProcedente == null)
            {
                OficioNoProcedente = new Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente();

                listaParrafos = GetParrafos(Datos_Oficio.Solicitud_id);

                OficioNoProcedente.Solicitud_id = Datos_Oficio.Solicitud_id;
                OficioNoProcedente.Parrafo_1 = listaParrafos.Parrafo1;
                OficioNoProcedente.Parrafo_2 = listaParrafos.Parrafo2;
                OficioNoProcedente.Parrafo_3 = listaParrafos.Parrafo3;
                OficioNoProcedente.swdatecreated = swdatecreated;
                OficioNoProcedente.swcraetedbyinterno = boolEsinterno;
                OficioNoProcedente.swcreatedby = objUs.intUsuario_id;

                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = JsonConvert.SerializeObject(listaParrafos);

                db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente.Add(Datos_Oficio);
            }
            else
            {

                OficioNoProcedente.Parrafo_1 = Datos_Oficio.Parrafo_1;
                OficioNoProcedente.Parrafo_2 = Datos_Oficio.Parrafo_2;
                OficioNoProcedente.Parrafo_3 = Datos_Oficio.Parrafo_3;
                OficioNoProcedente.swdateupdated = swdatecreated;
                OficioNoProcedente.swupdatedbyinterno = boolEsinterno;
                OficioNoProcedente.swupdatedby = objUs.intUsuario_id;

                listaParrafos.Parrafo1 = OficioNoProcedente.Parrafo_1;
                listaParrafos.Parrafo2 = OficioNoProcedente.Parrafo_2;
                listaParrafos.Parrafo1 = OficioNoProcedente.Parrafo_3;
                db.Entry(OficioNoProcedente).State = EntityState.Modified;

                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = JsonConvert.SerializeObject(listaParrafos);
            }

            db.SaveChanges();


            return Json(jsonRespuesta);
        }


        public JsonResult AgregarEnmienda(Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas model)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsinterno = false;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.Result = 0;
            jsonRespuesta.Mensaje = "No se ha realizado ninguna gestión";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                jsonRespuesta.Result = 3;
                jsonRespuesta.Mensaje = "No posee una sesión válida";
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno != 0)
            {
                boolEsinterno = true;
            }

            int countEnmiendas = 0;
            try
            {
                countEnmiendas = db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas.Where(Obj => Obj.Solicitud_id == model.Solicitud_id).Max(Obj => Obj.Enmienda_id);
            }
            catch (Exception ex)
            {
                countEnmiendas = 0;
            }

            countEnmiendas++;

            Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas Enmiendas = new Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas();
            Enmiendas.Solicitud_id = model.Solicitud_id;
            Enmiendas.Enmienda_id = countEnmiendas;
            Enmiendas.Enmienda = model.Enmienda;
            Enmiendas.Estado_id = true;
            Enmiendas.swcreatedby = objUs.intUsuario_id;
            Enmiendas.swdatecreated = swdatecreated;
            Enmiendas.swcreatedbyinterno = boolEsinterno;
            db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas.Add(Enmiendas);
            db.SaveChanges();

            jsonRespuesta.Result = 1;
            jsonRespuesta.Mensaje = "Registro agregado exitosamente";

            return Json(jsonRespuesta);
        }
        public JsonResult CambiarEstadoEnmienda(Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas model)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsinterno = false;
            JsonRespuesta jsonRespuesta = new JsonRespuesta();
            jsonRespuesta.Result = 0;
            jsonRespuesta.Mensaje = "No se ha realizado ninguna gestión";
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                jsonRespuesta.Result = 3;
                jsonRespuesta.Mensaje = "No posee una sesión válida";
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (objUs.EsInterno != 0)
            {
                boolEsinterno = true;
            }

            Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas Enmienda = db.Tbl_Gest_EtapaSolicitud_Oficio_Registro_NoProcedente_Enmiendas.Where(Obj => Obj.Solicitud_id == model.Solicitud_id && Obj.Enmienda_id == model.Enmienda_id).FirstOrDefault();

            if (Enmienda != null)
            {
                Enmienda.Estado_id = false;

                jsonRespuesta.Result = 1;
                jsonRespuesta.Mensaje = "Se cambió el estado del registro";

                db.Entry(Enmienda).State = EntityState.Modified;

                db.SaveChanges();
            }
            else
            {
                jsonRespuesta.Result = 2;
                jsonRespuesta.Mensaje = "No se encontró";
            }


            return Json(jsonRespuesta);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSubRegionalOficioRechazoController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableBanner = new PdfPTable(1);
        // GET: Form_FormularioSubRegionalOficioRechazo
        public ActionResult Index(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            ViewBag.Guid_id = Guid_id;
            ViewBag.EtapaSolicitudGuid = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;
            return View();
        }

        public ActionResult ResolucionRechazo(string Guid_id, string GuidEtapa_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
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
            guidetapasolicitud = GuidEtapa_id;


            Tbl_Sol_Solicitud oSolicitud = (from d in db.Tbl_Sol_Solicitud
                                            where d.Guid_id == guidsolicitud
                                            select d).FirstOrDefault();

            Tbl_Gest_EtapaSolicitud oEtapaSolicitud = new Tbl_Gest_EtapaSolicitud();

            oEtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                               where
                                   d.Solicitud_Guid_id == guidsolicitud
                                && d.EtapaSolicitud_GUID_id == guidetapasolicitud
                               select d).FirstOrDefault();

            Tbl_Gest_EtapaSolicitud oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                                                   where
                                                      d.Solicitud_Guid_id == guidsolicitud
                                                   && d.EtapaSolicitud_GUID_id == guidetapasolicitud
                                                   select d).FirstOrDefault();

            if (oResolucion.Resolucion_Aprobada == null)
            {
                oResolucion.Resolucion_Aprobada = false;
            }

            if (oResolucion.Resolucion_Denegada == null)
            {
                oResolucion.Resolucion_Denegada = false;
            }
            ViewBag.Guid_id = Guid_id;
            ViewBag.EtapaSolicitudGuid = GuidEtapa_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            return View(oResolucion);
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

        public String SQLDate(DateTime Fecha)
        {
            string Fch_String;

            Fch_String = Fecha.Year.ToString("D4") + "-" + Fecha.Month.ToString("D2") + "-" + Fecha.Day.ToString("D2") + " " + Fecha.Hour.ToString("D2") + ":" + Fecha.Minute.ToString("D2");

            return Fch_String;
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

            Font fntTituloTabla = FontFactory.GetFont("HELVETICA", size: 10, FontColour);

            PdfPCell c1 = new PdfPCell(new Phrase(Leyenda, fntTituloTabla));

            c1.BackgroundColor = iTextSharp.text.BaseColor.GRAY;


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableBanner.AddCell(c1);

            return;
        }



        public string GenerarResolucion(string Guid_id, string EtapaSolicitudGuid, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Tbl_Gest_EtapaSolicitud oResolucion = new Tbl_Gest_EtapaSolicitud();
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();

            oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                           where d.Solicitud_Guid_id == Guid_id && d.EtapaSolicitud_GUID_id == EtapaSolicitudGuid
                           select d).FirstOrDefault();

            oParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                         where d.Solicitud_Guid_id == Guid_id
                         select d).FirstOrDefault();



            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime fecharesolucion;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year + "-" + hoy.Hour + "-" + hoy.Minute + "-" + hoy.Second;

            if (oResolucion.swdatecreated == null)
            {
                fecharesolucion = hoy;
            }
            else
            {
                fecharesolucion = (DateTime)oResolucion.swdatecreated;
            }

            string strNombre = EtapaSolicitudGuid + fecha + ".pdf";
            string strDirArchivo = strFolder + strNombre;

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

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
            if (Constants.VisualizarInformacionDesarrollo == 1)
            {
                string urlact = this.Url.Action();
                LlenaBanner(urlact);
                doc.Add(tableBanner);
                doc.Add(Enter);
            }

            string No_Resolucion = identificadorOficialGestion.ObtenerNumeroResolucionSubRegional(Solicitud_id: oParrafos.Solicitud_id, Etapa_id: etapa_id, EtapaRuta_id: etaparuta_id, CorrelativoEtapa_id: correlativoetapa_id, Usuario_id: objUs.intUsuario_id).Identificador;
            LlenaBanner("Resolución No." + No_Resolucion, "Derecha", "Blanco");
            doc.Add(tableBanner);

            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.[Fnc_Gral_FechaSolicitudTxt]('" + SQLDate(oResolucion.swdatecreated ?? DateTime.Now) + "','" + oResolucion.Solicitud_id + "')").FirstOrDefault();

            LlenaBanner(strFecha, "Derecha", "Blanco");
            doc.Add(tableBanner);

            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(oResolucion.Solicitud_id);

            int Rol_SubRegional = db.Tbl_Gral_PerfilesRol.First().SubRegional ?? 0;
            Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_sol_Solicitud.Region_id && Obj.SubRegion_id == tbl_sol_Solicitud.SubRegion_id).First();
            fc_Seg_Sel_UsuarioXRolyRegion_Result DatosSubRegional = db.fc_Seg_Sel_UsuarioXRolyRegion(Rol_SubRegional, -5, tbl_sol_Solicitud.Region_id, tbl_sol_Solicitud.SubRegion_id).First();

            doc.Add(Enter);
            doc.Add(Enter);

            LlenaBanner(DatosSubRegional.NombreCompleto, "Izquierda", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Director Subregional " + tbl_Gral_SubRegion.No_SubRegion, "Izquierda", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner("INAB, Guatemala, Guatemala ", "Izquierda", "Blanco");
            doc.Add(tableBanner);

            doc.Add(Enter);

            Tbl_Seg_UsuarioExterno tbl_seg_usuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(tbl_sol_Solicitud.swcreatedby);

            Tbl_Gral_Departamento tbl_Gral_Departamento = db.Tbl_Gral_Departamento.Find(tbl_seg_usuarioExterno.Departamento_id);

            Tbl_Gral_Municipio tbl_Gral_Municipio = db.Tbl_Gral_Municipio.Find(tbl_seg_usuarioExterno.Municipio_id);

            Tbl_Sol_Solicitud_Categoria tbl_sol_Solicitud_Categoria = db.Tbl_Sol_Solicitud_Categoria.Find(tbl_sol_Solicitud.Categoria_id);

            Tbl_Sol_Solicitud_Sub_Categoria tbl_Sol_Solicitud_Sub_Categoria = db.Tbl_Sol_Solicitud_Sub_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_Solicitud.Categoria_id && Obj.Sub_Categoria_id == tbl_sol_Solicitud.Sub_Categoria_id).First();

            //CrearBanner banner = new CrearBanner();

            LlenaBanner(oParrafos.Parrafo_1, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner(oParrafos.Parrafo_2, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(oParrafos.Parrafo_3, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(oParrafos.Parrafo_4, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("CONSIDERANDO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(oParrafos.Parrafo_5, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner("POR TANTO", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(oParrafos.Parrafo_6, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);


            LlenaBanner("RESUELVE", "Centro", "Blanco");
            doc.Add(tableBanner);
            LlenaBanner(oParrafos.Parrafo_7, "Justificado", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);


            LlenaBanner("Atentamente", "Izquierda", "Blanco");
            doc.Add(tableBanner);
            doc.Add(Enter);

            LlenaBanner(DatosSubRegional.NombreCompleto, "Centro", "Blanco");
            doc.Add(tableBanner);

            LlenaBanner("Dirección SubRegional  " + tbl_Gral_SubRegion.No_SubRegion, "Centro", "Blanco");
            doc.Add(tableBanner);


            doc.Close();
            writer.Close();


            oResolucion.NombreDocumentoNoFirmado = strNombre;

            db.Entry(oResolucion).State = EntityState.Modified;
            db.SaveChanges();

            return strNombre;
            //return null;
        }
        public JsonResult VistaPreviaResolucion(Tbl_Gest_EtapaSolicitud model)
        {
            ListaParrafos oParrafos = new ListaParrafos();
            string query, validacionParrafos;
            validacionParrafos = model.Solicitud_Guid_id + "','" + model.Etapa_id + "','" + model.EtapaRuta_id + "','" + model.CorrelativoEtapa_id;
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 1 + "')";
            oParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 2 + "')";
            oParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 3 + "')";
            oParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 4 + "')";
            oParrafos.Parrafo4 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 5 + "')";
            oParrafos.Parrafo5 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 6 + "')";
            oParrafos.Parrafo6 = db.Database.SqlQuery<string>(query).FirstOrDefault();
            query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 7 + "')";
            oParrafos.Parrafo7 = db.Database.SqlQuery<string>(query).FirstOrDefault();

            return Json(JsonConvert.SerializeObject(oParrafos));
        }

        public JsonResult GeneraResolucion(Tbl_Gest_EtapaSolicitud model)
        {
            string archivo = GenerarResolucion(model.Solicitud_Guid_id, model.EtapaSolicitud_GUID_id, model.Etapa_id, model.EtapaRuta_id, model.CorrelativoEtapa_id);
            string txtMostrar = "{\"Ubicacion\":\"" + archivo + "\"}";
            return Json(txtMostrar);
        }

        public JsonResult GrabaParrafos(Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos model)
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


            DateTime swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();
            oParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                         where d.Solicitud_id == model.Solicitud_id
                         select d).FirstOrDefault();

            if (oParrafos != null)
            {
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Remove(oParrafos);
                db.SaveChanges();

                model.swdateupdated = swdatecreated;
                model.swupdatedby = objUs.intUsuario_id;
                model.swupdatedbyinterno = true;
                oParrafos.Parrafo_1 = model.Parrafo_1;
                oParrafos.Parrafo_2 = model.Parrafo_2;
                oParrafos.Parrafo_3 = model.Parrafo_3;
                oParrafos.Parrafo_4 = model.Parrafo_4;
                oParrafos.Parrafo_5 = model.Parrafo_5;
                oParrafos.Parrafo_6 = model.Parrafo_6;
                oParrafos.Parrafo_7 = model.Parrafo_7;
                oParrafos.swdateupdated = model.swdateupdated;
                oParrafos.swupdatedby = model.swupdatedby;
                oParrafos.swupdatedbyinterno = model.swupdatedbyinterno;
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Add(model);
            }
            else
            {
                model.swdatecreated = swdatecreated;
                model.swcreatedby = objUs.intUsuario_id;
                model.swcreatedbyinterno = true;
                db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos.Add(model);
            }

            db.SaveChanges();
            string txtMostrar = "{\"Resultado\":\"Listo\"}";
            return Json(txtMostrar);
        }

        public JsonResult ObtenerParrafos(Tbl_Gest_EtapaSolicitud model)
        {
            Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos oGetParrafos = new Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos();
            oGetParrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Resolucion_Parrafos
                            where d.Solicitud_id == model.Solicitud_id
                            select d).FirstOrDefault();
            ListaParrafos oParrafos = new ListaParrafos();

            if (oGetParrafos != null)
            {

                oParrafos.Parrafo1 = oGetParrafos.Parrafo_1;
                oParrafos.Parrafo2 = oGetParrafos.Parrafo_2;
                oParrafos.Parrafo3 = oGetParrafos.Parrafo_3;
                oParrafos.Parrafo4 = oGetParrafos.Parrafo_4;
                oParrafos.Parrafo5 = oGetParrafos.Parrafo_5;
                oParrafos.Parrafo6 = oGetParrafos.Parrafo_6;
                oParrafos.Parrafo7 = oGetParrafos.Parrafo_7;
                return Json(JsonConvert.SerializeObject(oParrafos));
            }
            else
            {
                string query, validacionParrafos;
                validacionParrafos = model.Solicitud_Guid_id + "','" + model.Etapa_id + "','" + model.EtapaRuta_id + "','" + model.CorrelativoEtapa_id;
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 1 + "')";
                oParrafos.Parrafo1 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 2 + "')";
                oParrafos.Parrafo2 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 3 + "')";
                oParrafos.Parrafo3 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 4 + "')";
                oParrafos.Parrafo4 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 5 + "')";
                oParrafos.Parrafo5 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 6 + "')";
                oParrafos.Parrafo6 = db.Database.SqlQuery<string>(query).FirstOrDefault();
                query = "SELECT dbo.[Fcn_Gest_EtapaSolicitud_Resolucion_Parrafo_Rechazo]('" + validacionParrafos + "','" + 7 + "')";
                oParrafos.Parrafo7 = db.Database.SqlQuery<string>(query).FirstOrDefault();


                return Json(JsonConvert.SerializeObject(oParrafos));
            }
        }
        public JsonResult GrabarResolucion(Tbl_Gest_EtapaSolicitud model)
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

            DateTime swdatecreated = DateTime.Now;
            Tbl_Gest_EtapaSolicitud oResolucion = new Tbl_Gest_EtapaSolicitud();
            oResolucion = (from d in db.Tbl_Gest_EtapaSolicitud
                           where d.Solicitud_id == model.Solicitud_id && d.Etapa_id == model.Etapa_id && d.EtapaRuta_id == model.EtapaRuta_id && d.CorrelativoEtapa_id == model.CorrelativoEtapa_id
                           select d).FirstOrDefault();

            if (oResolucion != null)
            {
                model.swdateupdated = swdatecreated;
                model.swupdatedby = objUs.intUsuario_id;
                model.swupdatedbyinterno = true;
                oResolucion.Resolucion_Aprobada = model.Resolucion_Aprobada;
                oResolucion.Resolucion_Denegada = model.Resolucion_Denegada;
                oResolucion.swdateupdated = model.swdateupdated;
                oResolucion.swupdatedby = model.swupdatedby;
                oResolucion.swupdatedbyinterno = model.swupdatedbyinterno;

            }
            else
            {
                model.swdatecreated = swdatecreated;
                model.swcreatedby = objUs.intUsuario_id;
                model.swcreatedbyinterno = true;
                db.Tbl_Gest_EtapaSolicitud.Add(model);
            }

            db.SaveChanges();
            string txtMostrar = "{\"Resultado\":\"Listo\"}";
            return Json(txtMostrar);
        }

    }
}
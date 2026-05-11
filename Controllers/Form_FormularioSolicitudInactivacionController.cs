using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace RNF_Web.Controllers
{
    public class Form_FormularioSolicitudInactivacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        private PdfPTable tableTitulo = new PdfPTable(3);
        private PdfPTable tableDatosGenerales = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosInstrucciones = new PdfPTable(numColumns: 8);
        private PdfPTable tableDatosEvaluacion = new PdfPTable(numColumns: 8);
        private PdfPTable tablePreguntasRespuestas = new PdfPTable(numColumns: 8);
        private PdfPTable tableEstimacion = new PdfPTable(numColumns: 8);
        private PdfPTable tableFormulas = new PdfPTable(numColumns: 8);
        private PdfPTable tablePersoneria = new PdfPTable(1);
        private PdfPTable tableFirmaSolicitante = new PdfPTable(numColumns: 8);
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


        private void LlenaBanner(String Leyenda, string Color = "", int alineacionhorizontal = 0, int alineacionvertical = 5, int borde = 0, string estilotexto = "Normal")
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
        private void LlenaTituloRevision(string Titulo, string Codigo, string Version, string Fecha_Implementacion)
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

            //c1 = new PdfPCell(new Phrase("\nREVISION DE SOLITUD DE INSCRIPCION DE " + tbl_Sol_Solicitud.Tbl_Sol_Solicitud_Categoria.Descripcion.ToString().ToUpper() + " \n\n\n", fntTituloTabla));
            c1 = new PdfPCell(new Phrase("\n" + Titulo.ToUpper() + "\n\n\n", fntTituloTabla));


            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            c1.Colspan = 3;
            c1.Rowspan = 3;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Código", fntTablasCeldas));
            c1.Colspan = 1;
            c1.Rowspan = 1;


            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Codigo, fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Versión", fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Version, fntTablasCeldas));
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("Fecha de implementación:", fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase(Fecha_Implementacion, fntTablasCeldas));
            c1.Rowspan = 2;
            tableTitulo.AddCell(c1);

            c1 = new PdfPCell(new Phrase("\nPROCESO: REGISTRO NACIONAL FORESTAL \n\n\n", fntTablasCeldas));
            c1.Colspan = 3;

            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.VerticalAlignment = Element.ALIGN_MIDDLE;

            tableTitulo.AddCell(c1);

            return;
        }


        // GET: Form_FormularioSolicitudInactivacion
        //public ActionResult Index(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        //{

        //    Usuario objUs = new Usuario();
        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
        //        ViewBag.Mensaje = objSesion.getStrMensaje();
        //        return RedirectToAction("../Login/AccesoColaborador");
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];
        //    }

        //    ViewBag.GuidEtapa_id = GuidEtapa_id;
        //    ViewBag.Guid_id = Guid_id;
        //    ViewBag.etapa_id = etapa_id;
        //    ViewBag.etaparuta_id = etaparuta_id;
        //    ViewBag.correlativoetapa_id = correlativoetapa_id;

        //    return View();
        //}


        public ActionResult AgregarHallazgo(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();


            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            if ((tbl_Sol_Solicitud.No_Registro == null) || (tbl_Sol_Solicitud.No_Registro.Trim() == ""))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.GuidEtapa_id = tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id;
            ViewBag.No_Registro = tbl_Sol_Solicitud.No_Registro;
            ViewBag.Bitacora_id = tbl_Sol_Solicitud.Bitacora_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                 select d).FirstOrDefault();

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();


            if (tbl_RNF_Registro_Bitacora == null)
            {
                long Bitacora_id = 0;
                try
                {
                    Bitacora_id = db.Tbl_RNF_Registro_Bitacora.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).Max(Obj => Obj.Bitacora_id);
                }
                catch
                {
                    Bitacora_id = 0;
                }
                Bitacora_id++;

                tbl_RNF_Registro_Bitacora = new Tbl_RNF_Registro_Bitacora()
                {
                    No_Registro = tbl_Sol_Solicitud.No_Registro,
                    No_RegistroLiteral = tbl_Sol_Solicitud.No_RegistroLiteral,
                    No_RegistroCorrelativo = (int)tbl_Sol_Solicitud.No_RegistroCorrelativo,
                    Solicitud_id = tbl_Sol_Solicitud.Solicitud_id,
                    Bitacora_id = Bitacora_id,
                    Motivo = ""
                };
                db.Tbl_RNF_Registro_Bitacora.Add(tbl_RNF_Registro_Bitacora);
                db.SaveChanges();

                tbl_Sol_Solicitud.Bitacora_id = Bitacora_id;
                db.Entry(tbl_Sol_Solicitud).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Motivo = tbl_RNF_Registro_Bitacora.Motivo;

            Tbl_RNF_Registro_BitacoraHallazgo tbl_RNF_Registro_BitacoraHallazgo = new Tbl_RNF_Registro_BitacoraHallazgo()
            {
                No_Registro = tbl_RNF_Registro_Bitacora.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro_Bitacora.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro_Bitacora.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro_Bitacora.Solicitud_id,
                Bitacora_id = tbl_RNF_Registro_Bitacora.Bitacora_id
            };



            List<Tbl_RNF_Registro_InactivacionTecnico_Tipo> tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in db.Tbl_RNF_Registro_InactivacionTecnico_Tipo
                                                                                                          select d).ToList();

            tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                          where d.InactivacionTecnicoTipo_id != 0
                                                          select d).ToList();

            List<Tbl_RNF_Registro_Bitacora> tbl_RNF_Registro_Bitacoras = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                          where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                          select d).ToList();

            if (tbl_RNF_Registro_Bitacoras == null)
            {
                tbl_RNF_Registro_Bitacoras = new List<Tbl_RNF_Registro_Bitacora>();
            }

            int CountInactivacionTemporal = 0;
            int CountInactivacionDefinitiva = 0;

            foreach (var item in tbl_RNF_Registro_Bitacoras)
            {
                item.InactivacionTemporal = item.InactivacionTemporal ?? false;
                item.InactivacionDefinitiva = item.InactivacionDefinitiva ?? false;
            }

            CountInactivacionTemporal = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionTemporal == true).Count();
            CountInactivacionDefinitiva = tbl_RNF_Registro_Bitacoras.Where(Obj => Obj.InactivacionDefinitiva == true).Count();

            if ((CountInactivacionTemporal >= 2) || (CountInactivacionDefinitiva >= 1))
            {
                //1: Inactivación temporal
                tbl_RNF_Registro_InactivacionTecnico_Tipos = (from d in tbl_RNF_Registro_InactivacionTecnico_Tipos
                                                              where d.InactivacionTecnicoTipo_id != 1
                                                              select d).ToList();
            }

            ViewBag.InactivacionTecnico_Tipo = new SelectList(tbl_RNF_Registro_InactivacionTecnico_Tipos, "InactivacionTecnicoTipo_id", "Descripcion", (tbl_Sol_Solicitud.InactivacionTecnicoTipo_id ?? tbl_RNF_Registro_InactivacionTecnico_Tipos.FirstOrDefault().InactivacionTecnicoTipo_id));

            List<Tbl_RNF_Registro_InactivacionTiempo> tbl_RNF_Registro_InactivacionTiempos = db.Tbl_RNF_Registro_InactivacionTiempo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id).ToList();
            if (tbl_Sol_Solicitud.Categoria_id == 8)
            {
                tbl_RNF_Registro_InactivacionTiempos = (from d in tbl_RNF_Registro_InactivacionTiempos
                                                        where d.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id
                                                        select d).ToList();
            }
            ViewBag.TiempoInactivacion = new SelectList(tbl_RNF_Registro_InactivacionTiempos, "Dias", "Descripcion");


            List<Tbl_RNF_Registro_Inactivacion_Tipo> TipoInactivacion_id = db.Tbl_RNF_Registro_Inactivacion_Tipo.Where(Obj => Obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && Obj.UsuarioExterno == true).ToList();


            ViewBag.TipoInactivacion_id = new SelectList(TipoInactivacion_id, "TipoInactivacion_id", "Descripcion", tbl_Sol_Solicitud.TipoInactivacion_id ?? 0);
            decimal tipogestion, procesoinactivacion;
            tipogestion = 0;
            procesoinactivacion = 0.04M;
            bool mostrar = false;
            decimal truncsolicitudtipoid = Math.Truncate(tbl_Sol_Solicitud.SolicitudTipo_id);
            decimal solicitudtipoid = tbl_Sol_Solicitud.SolicitudTipo_id - truncsolicitudtipoid;

            if (solicitudtipoid == 0.04M)
            {
                mostrar = true;
            }
            ViewBag.mostrar = mostrar;

            return View(tbl_RNF_Registro_BitacoraHallazgo);
        }




        public ActionResult InactivacionTipo001(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            int Doc_TecPro_Tipo_id = 2;

            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
        }

        public ActionResult InactivacionTipo002(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            int Doc_TecPro_Tipo_id = 2;

            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
        }

        public ActionResult InactivacionTipo003(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            int Doc_TecPro_Tipo_id = 3;

            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
        }

        public ActionResult InactivacionTipo004(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            int Doc_TecPro_Tipo_id = 4;

            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
        }


        public ActionResult OficioDeTraslado(string GuidEtapa_id, string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            ViewBag.GuidEtapa_id = GuidEtapa_id;
            ViewBag.Guid_id = Guid_id;
            ViewBag.etapa_id = etapa_id;
            ViewBag.etaparuta_id = etaparuta_id;
            ViewBag.correlativoetapa_id = correlativoetapa_id;

            int Doc_TecPro_Tipo_id = 3;

            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = ParrafosSolicitud(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id, objUs);

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
        }



        public Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos ParrafosSolicitud(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, int Doc_TecPro_Tipo_id, Usuario objUs)
        {
            DateTime swdatecreated = DateTime.Now;
            bool boolEsInterno = false;
            if (objUs.EsInterno != 0)
            {
                boolEsInterno = true;
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == Guid_id
                                                   select d).FirstOrDefault();
            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos
                                                                                                           where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && d.Doc_TecPro_Tipo_id == Doc_TecPro_Tipo_id
                                                                                                           select d).FirstOrDefault();
            if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos == null)
            {
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = new Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos();
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Doc_TecPro_Tipo_id = Doc_TecPro_Tipo_id;
                Parrafos parrafos = ListarParrafos(Guid_id, etapa_id, etaparuta_id, correlativoetapa_id, Doc_TecPro_Tipo_id);
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1 = parrafos.Parrafo_1;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2 = parrafos.Parrafo_2;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3 = parrafos.Parrafo_3;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4 = parrafos.Parrafo_4;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5 = parrafos.Parrafo_5;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6 = parrafos.Parrafo_6;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7 = parrafos.Parrafo_7;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_8 = parrafos.Parrafo_8;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_9 = parrafos.Parrafo_9;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_10 = parrafos.Parrafo_10;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swcreatedby = objUs.intUsuario_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swdatecreated = swdatecreated;
                tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swcreatedbyinterno = boolEsInterno;

                db.Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Add(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos);
                db.SaveChanges();


            }

            return tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos;
        }

        public ActionResult Enmiendas_Create(long Solicitud_id, int Doc_TecPro_Tipo_id)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas = new Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas();
            tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Solicitud_id = Solicitud_id;
            tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Doc_TecPro_Tipo_id = Doc_TecPro_Tipo_id;

            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas);
        }

        public ActionResult Enmiendas_Lista(long Solicitud_id, int Doc_TecPro_Tipo_id)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            List<Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas> tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas
                                                                                                                   where d.Solicitud_id == Solicitud_id && d.Doc_TecPro_Tipo_id == Doc_TecPro_Tipo_id
                                                                                                                   orderby d.Estado_id descending, d.Enmienda_id
                                                                                                                   select d).ToList();


            return View(tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas);
        }

        public string GenerarDocumentoInactivacion(Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud, int Doc_TecPro_Tipo_id, Usuario objUs)
        {
            var Enter = new Paragraph(" ");
            string strDir = "Archivos_Generados_Que_Pueden_Borrar\\";
            string strFolder = Server.MapPath("~/") + strDir;
            DateTime hoy = DateTime.Now;
            string fecha = "-" + hoy.Day + "-" + hoy.Month + "-" + hoy.Year;
            string strNombre;
            string strDirArchivo;
            string strNombrePersona;

            CrearBanner crearBanner = new CrearBanner();

            IdentificadorOficialGestion identificadorOficialGestion = new IdentificadorOficialGestion();
            SqlParameter[] sqlParams;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = (from d in db.Tbl_Sol_Solicitud
                                                   where d.Guid_id == tbl_Gest_EtapaSolicitud.Solicitud_Guid_id
                                                   select d).FirstOrDefault();
            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_Sol_Solicitud.No_Registro
                                                                   && d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id
                                                                   && d.Bitacora_id == tbl_Sol_Solicitud.Bitacora_id
                                                                   select d).FirstOrDefault();
            if (tbl_RNF_Registro_Bitacora == null)
            {
                return null;
            }



            Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos
                                                                                                           where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && d.Doc_TecPro_Tipo_id == Doc_TecPro_Tipo_id
                                                                                                           select d).FirstOrDefault();
            strNombre = $"U{tbl_Gest_EtapaSolicitud.EtapaSolicitud_GUID_id}.pdf";

            strDirArchivo = strFolder + strNombre;

            int countEnmiendas = 0;
            string @p0, @p1;
            @p0 = "@Region_id";
            @p1 = "@SubRegion_id";
            string sqlQuery = $"select [dbo].[Fnc_Gral_NombreSubDirectorRegional]({@p0},{@p1})";
            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = @p0,  Value = tbl_Sol_Solicitud.Region_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = @p1,  Value = tbl_Sol_Solicitud.SubRegion_id, Direction = System.Data.ParameterDirection.Input }
            };
            string NombreSubDirectorRegional = db.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault();

            sqlQuery = "SELECT dbo.Fnc_Gral_FechaTxt(getdate())";
            string strFecha = db.Database.SqlQuery<string>("SELECT dbo.Fnc_Gral_FechaTxt(getdate())").FirstOrDefault();

            string JuridicoAsignado = "";
            if (tbl_Sol_Solicitud.JuridicoAsignado_id != null)
            {

                @p0 = "@JuridicoAsignado_id";
                sqlQuery = $"select Nombre + ' ' + Apellidos from Tbl_Seg_Usuario where Usuario_id = {@p0}";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = @p0,  Value = tbl_Sol_Solicitud.JuridicoAsignado_id, Direction = System.Data.ParameterDirection.Input }
                };
                JuridicoAsignado = db.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault();

            }

            string TecnicoAsignado = "";
            if (tbl_Sol_Solicitud.TecnicoAsignado_id != null)
            {
                @p0 = "@TecnicoAsignado_id";
                sqlQuery = $"select Nombre + ' ' + Apellidos from Tbl_Seg_Usuario where Usuario_id = {@p0}";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = @p0,  Value = tbl_Sol_Solicitud.TecnicoAsignado_id, Direction = System.Data.ParameterDirection.Input }
                };
                TecnicoAsignado = db.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault();
            }


            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(1f, 1f, 25f, 50f);

            if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos != null)
            {
                List<Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas> Enmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas
                                                                                  where d.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id && d.Doc_TecPro_Tipo_id == Doc_TecPro_Tipo_id && d.Estado_id == true
                                                                                  orderby d.Enmienda_id
                                                                                  select d).ToList();

                FileStream _stream = new FileStream(strDirArchivo, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, _stream);
                doc.Open();


                if (Constants.VisualizarInformacionDesarrollo == 1)
                {
                    string urlAction = this.Url.Action();
                    LlenaBanner(Leyenda: urlAction, Color: "GrisClaro", alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                }

                if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Doc_TecPro_Tipo_id == 1)
                {
                    Result_SP_IdentificadorOficialGestion result_SP_IdentificadorOficialGestion = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id);
                    string InformeTecnico = result_SP_IdentificadorOficialGestion.Identificador;
                    string titulo = tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Tbl_Gest_EtapaSolicitud_Inactivacion_Tipo.Descripcion;
                    string codigo = result_SP_IdentificadorOficialGestion.Codigo;
                    string version = result_SP_IdentificadorOficialGestion.Version;
                    string fecha_implementacion = result_SP_IdentificadorOficialGestion.strFecha;

                    LlenaTituloRevision(titulo, codigo, version, fecha_implementacion);
                    doc.Add(tableTitulo);
                    doc.Add(Enter);


                    LlenaBanner(Leyenda: strFecha, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Informe No." + InformeTecnico, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: NombreSubDirectorRegional, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Dirección Subregional", Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Instituto Nacional de Bosques -INAB-", Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "F. _____________________________", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: TecnicoAsignado, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Director Subregional " + NombreSubDirectorRegional, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);


                }
                else if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Doc_TecPro_Tipo_id == 2)
                {
                    string InformeTecnico = identificadorOficialGestion.ObtenerNumeroInformeTecnico(tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id).Identificador;
                    string titulo = tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Tbl_Gest_EtapaSolicitud_Inactivacion_Tipo.Descripcion;
                    string codigo = "RF-RE-042";
                    string version = "1";
                    string fecha_implementacion = "Septiembre 2022";
                    LlenaTituloRevision(titulo, codigo, version, fecha_implementacion);
                    doc.Add(tableTitulo);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: strFecha, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    LlenaBanner(Leyenda: "Informe No." + InformeTecnico, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "Señor:", Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0, estilotexto: "Bold");
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0, estilotexto: "Bold");
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2, Color: null, alineacionhorizontal: Al_Justificado, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    if (Enmiendas.Count() > 0)
                    {
                        countEnmiendas++;
                        foreach (var item in Enmiendas)
                        {
                            LlenaBanner(Leyenda: countEnmiendas + ". " + item.Descripcion, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                            doc.Add(tableBanner);
                            countEnmiendas++;
                        }
                    }

                    doc.Add(Enter);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "F. _____________________________", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Nombre, Firma y Seelo", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Director Subregional " + NombreSubDirectorRegional, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                }
                else if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Doc_TecPro_Tipo_id == 3)
                {
                    string OficioSubRegional = identificadorOficialGestion.ObtenerNumeroOficio(3, tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id).Identificador;
                    string titulo = tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Tbl_Gest_EtapaSolicitud_Inactivacion_Tipo.Descripcion;
                    string codigo = "RF-RE-043";
                    string version = "1";
                    string fecha_implementacion = "Septiembre 2022";
                    crearBanner.LlenaTituloRevision(titulo, codigo, version, fecha_implementacion, Server.MapPath("~/Content/images/logoInabExcel.jpg"));
                    doc.Add(crearBanner.tableTitulo);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: strFecha, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    LlenaBanner(Leyenda: "Oficio No." + OficioSubRegional, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);


                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }

                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }

                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }

                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4.Trim() != ""))
                    {

                        doc.Add(Enter); LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }
                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                        doc.Add(Enter);
                    }
                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6, Color: null, alineacionhorizontal: Al_Justificado, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }

                    if (tbl_RNF_Registro_Bitacora.Tbl_RNF_Registro_BitacoraHallazgo.Count() > 0)
                    {
                        long countHallazgos = 0;
                        doc.Add(Enter);

                        foreach (var item in tbl_RNF_Registro_Bitacora.Tbl_RNF_Registro_BitacoraHallazgo)
                        {
                            if ((item.DescripcionHallazgo != null) && (item.DescripcionHallazgo.Trim() != ""))
                            {
                                countHallazgos++;
                                crearBanner.LlenaBanner(countHallazgos + ". " + item.DescripcionHallazgo.Trim(), "Izquierda", "");
                                doc.Add(crearBanner.tableBanner);
                            }
                        }
                        doc.Add(Enter);
                    }


                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }
                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_8 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_8.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_8, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }
                    if ((tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_9 != null) && (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_9.Trim() != ""))
                    {
                        LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_9, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                        doc.Add(tableBanner);
                    }

                    doc.Add(Enter);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "F. _____________________________", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Nombre, Firma y Seelo", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Director Subregional " + NombreSubDirectorRegional, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                }
                else if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Doc_TecPro_Tipo_id == 4)
                {
                    string OficioSubRegional = identificadorOficialGestion.ObtenerNumeroOficio(3, tbl_Sol_Solicitud.Solicitud_id, tbl_Gest_EtapaSolicitud.Etapa_id, tbl_Gest_EtapaSolicitud.EtapaRuta_id, tbl_Gest_EtapaSolicitud.CorrelativoEtapa_id, objUs.intUsuario_id).Identificador;
                    string titulo = tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Tbl_Gest_EtapaSolicitud_Inactivacion_Tipo.Descripcion;
                    string codigo = "RF-RE-044";
                    string version = "1";
                    string fecha_implementacion = "Septiembre 2022";
                    LlenaTituloRevision(titulo, codigo, version, fecha_implementacion);
                    doc.Add(tableTitulo);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: strFecha, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    LlenaBanner(Leyenda: "Oficio No." + OficioSubRegional, Color: null, alineacionhorizontal: Al_Derecha, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "Señor(a):", Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3, Color: null, alineacionhorizontal: Al_Justificado, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);

                    if (Enmiendas.Count() > 0)
                    {
                        countEnmiendas++;
                        foreach (var item in Enmiendas)
                        {
                            LlenaBanner(Leyenda: item.Descripcion, Color: null, alineacionhorizontal: Al_Izquierda, alineacionvertical: Al_Abajo, borde: 0);
                            doc.Add(tableBanner);
                            countEnmiendas++;
                        }
                        doc.Add(Enter);
                    }

                    LlenaBanner(Leyenda: tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4, Color: null, alineacionhorizontal: Al_Justificado, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                    doc.Add(Enter);
                    doc.Add(Enter);
                    doc.Add(Enter);

                    LlenaBanner(Leyenda: "F. _____________________________", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Nombre, Firma y Seelo", Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);
                    LlenaBanner(Leyenda: "Director Subregional " + NombreSubDirectorRegional, Color: null, alineacionhorizontal: Al_Centro, alineacionvertical: Al_Abajo, borde: 0);
                    doc.Add(tableBanner);

                }

                doc.Close();
                writer.Close();

                return strNombre;
            }
            else
            {
                return null;
            }
        }

        public class Parrafos
        {
            public string Parrafo_1 { get; set; }
            public string Parrafo_2 { get; set; }
            public string Parrafo_3 { get; set; }
            public string Parrafo_4 { get; set; }
            public string Parrafo_5 { get; set; }
            public string Parrafo_6 { get; set; }
            public string Parrafo_7 { get; set; }
            public string Parrafo_8 { get; set; }
            public string Parrafo_9 { get; set; }
            public string Parrafo_10 { get; set; }
        }

        public Parrafos ListarParrafos(string Solicitud_Guid_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, int Doc_Tecpro_Tipo_id)
        {
            int countParrafos = 0;
            Parrafos parrafos = new Parrafos();

            string sqlQuery, validaParams, strSelect;

            strSelect = "select [dbo].[Fcn_Gest_EtapaSolicitud_Inactivacion_Parrafo_Tipo]";
            validaParams = $"'{Solicitud_Guid_id}','{Etapa_id}','{EtapaRuta_id}','{CorrelativoEtapa_id}','{Doc_Tecpro_Tipo_id}'";

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_1 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_2 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_3 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_4 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_5 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_6 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_7 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_8 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_9 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            countParrafos++;
            sqlQuery = strSelect + "(" + validaParams + "," + countParrafos + ")";
            parrafos.Parrafo_10 = db.Database.SqlQuery<string>(sqlQuery).FirstOrDefault();

            return parrafos;
        }

        public JsonResult ReiniciarParrafos(string Solicitud_Guid_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, int Doc_Tecpro_Tipo_id)
        {
            Parrafos parrafos = ListarParrafos(Solicitud_Guid_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Doc_Tecpro_Tipo_id);

            return Json(parrafos);
        }

        public JsonResult GrabarParrafos(Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos model)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos
                                                                                                               where d.Solicitud_id == model.Solicitud_id && d.Doc_TecPro_Tipo_id == model.Doc_TecPro_Tipo_id
                                                                                                               select d).FirstOrDefault();

                if (tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos != null)
                {

                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_1 = model.Parrafo_1;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_2 = model.Parrafo_2;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_3 = model.Parrafo_3;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_4 = model.Parrafo_4;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_5 = model.Parrafo_5;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_6 = model.Parrafo_6;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_7 = model.Parrafo_7;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_8 = model.Parrafo_8;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_9 = model.Parrafo_9;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Parrafo_10 = model.Parrafo_10;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swupdatedby = objUs.intUsuario_id;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swupdatedbyinterno = boolEsInterno;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.swdateupdated = swdatecreated;
                    db.Entry(tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos).State = EntityState.Modified;
                }
                else
                {

                    model.swcreatedbyinterno = boolEsInterno;
                    model.swcreatedby = objUs.intUsuario_id;
                    model.swdatecreated = swdatecreated;
                    db.Tbl_Gest_EtapaSolicitud_Inactivacion_Parrafos.Add(model);

                }

                db.SaveChanges();

                jsonResupesta.Result = 1;
                jsonResupesta.Mensaje = "Registro agregado exitosamente";

            }

            return Json(jsonResupesta);
        }

        public class JsonResupesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

        public JsonResult AgregarEnmienda(long Solicitud_id, int Doc_Tecpro_Tipo_id, string Descripcion)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                int Enmienda_id = 0;
                try
                {
                    Enmienda_id = db.Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Where(Obj => Obj.Solicitud_id == Solicitud_id && Obj.Doc_TecPro_Tipo_id == Doc_Tecpro_Tipo_id).Max(Obj => Obj.Enmienda_id);
                }
                catch (Exception ex)
                {
                    Enmienda_id = 0;
                }
                Enmienda_id++;

                Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas = new Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas();
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Solicitud_id = Solicitud_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Doc_TecPro_Tipo_id = Doc_Tecpro_Tipo_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Enmienda_id = Enmienda_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Descripcion = Descripcion;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Estado_id = true;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swcreatedby = objUs.intUsuario_id;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swcreatedbyinterno = boolEsInterno;
                tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swdatecreated = swdatecreated;

                db.Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Add(tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas);
                db.SaveChanges();

                jsonResupesta.Result = 1;
                jsonResupesta.Mensaje = "Registro agregado exitosamente";

            }

            return Json(jsonResupesta);
        }

        public JsonResult CambiarEstadoEnmienda(long Solicitud_id, int Doc_TecPro_Tipo_id, int Enmienda_id)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas = (from d in db.Tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas
                                                                                                                 where d.Solicitud_id == Solicitud_id && d.Doc_TecPro_Tipo_id == Doc_TecPro_Tipo_id && d.Enmienda_id == Enmienda_id
                                                                                                                 select d).FirstOrDefault();

                if (tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas != null)
                {

                    tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Estado_id = !tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.Estado_id;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swupdatedby = objUs.intUsuario_id;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swcreatedbyinterno = boolEsInterno;
                    tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas.swdateupdated = swdatecreated;

                    db.Entry(tbl_Gest_EtapaSolicitud_Inactivacion_Enmiendas).State = EntityState.Modified;
                    db.SaveChanges();

                    jsonResupesta.Result = 1;
                    jsonResupesta.Mensaje = "Registro actualizado";

                }
                else
                {

                    jsonResupesta.Result = 2;
                    jsonResupesta.Mensaje = "Registro no encontrado";

                }

            }

            return Json(jsonResupesta);
        }

        public JsonResult ObtenerDocumentoInactivacion(Tbl_Gest_EtapaSolicitud model, int Doc_Tecpro_Tipo_id)
        {
            JsonResupesta jsonResupesta = new JsonResupesta();
            bool ErroresEncontrados = false;
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                jsonResupesta.Result = 0;
                jsonResupesta.Mensaje = "No posee una sesión válida";
                ErroresEncontrados = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (!ErroresEncontrados)
            {
                DateTime swdatecreated = DateTime.Now;
                bool boolEsInterno = false;

                if (objUs.EsInterno != 0)
                {
                    boolEsInterno = true;
                }

                try
                {
                    Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = (from d in db.Tbl_Gest_EtapaSolicitud
                                                                       where d.Solicitud_Guid_id == model.Solicitud_Guid_id && d.EtapaSolicitud_GUID_id == model.EtapaSolicitud_GUID_id
                                                                       select d).FirstOrDefault();

                    jsonResupesta.Ubicacion = GenerarDocumentoInactivacion(tbl_Gest_EtapaSolicitud, Doc_Tecpro_Tipo_id, objUs);
                    if (jsonResupesta.Ubicacion != null)
                    {
                        jsonResupesta.Result = 1;
                        jsonResupesta.Mensaje = "Archivo generado exitosamente";
                        tbl_Gest_EtapaSolicitud.NombreDocumentoNoFirmado = jsonResupesta.Ubicacion;
                        tbl_Gest_EtapaSolicitud.swdateupdated = swdatecreated;
                        tbl_Gest_EtapaSolicitud.swupdatedby = objUs.intUsuario_id;
                        tbl_Gest_EtapaSolicitud.swupdatedbyinterno = boolEsInterno;

                        db.Entry(tbl_Gest_EtapaSolicitud).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        jsonResupesta.Result = 2;
                        jsonResupesta.Mensaje = "No se pudo generar el archivo, no se encontró el documento requerido";
                    }
                }
                catch (Exception ex)
                {
                    jsonResupesta.Result = 3;
                    jsonResupesta.Mensaje = "Ocurrió un error: " + ex.Message;
                }
            }
            return Json(jsonResupesta);
        }










        [HttpPost]
        public JsonResult ActualizaEtapaRespuestaIndexSolicitud(long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string motivo, int respuestaid, string EtapaSolicitud_GUIDid)
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
                strRespuesta = "Se ha notificado la respuesta.";


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


        public string GetBearer()
        {
            try
            {

                var client = new RestClient(Constants.Address_Bearer);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string body = @"{
                        " + "\n" +
                            @"""Username"":""RNFUser"",
                        " + "\n" +
                            @"""Password"":""RNF_Password""
                        " + "\n" +
                            @"}
                        " + "\n" +
                            @"";

                body = body.Replace("RNFUser", Constants.RNF_Username);
                body = body.Replace("RNF_Password", Constants.RNF_Password);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.Content.ToString().Replace("\"", "").Length < 250)
                {
                    return "Error en credenciales RNF User  y RNF Password en la solicitud de bearer";
                }

                if (response.Content.ToString().Replace("\"", "").Length > 270)
                {
                    return "Error: " + response.Content.ToString().Replace("\"", "").Substring(1, 100);
                }
                else
                {
                    return "Bearer " + response.Content.ToString().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message.Substring(1, 100);
            }
        }

        class RNFCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public string CallCORS(string strSign, string strPath)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveUpload);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", strSign);
                //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
                request.AddFile("nombre", strPath, "application/pdf");
                request.AddParameter("parentGoogleDriveId", Constants.strParentGoogleDrive);
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                return joResponse["GoogleDriveId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error: No se logró subir el archivo a Google Drive, reporte a informatica. " + ex.Message.ToString();
            }
        }

        //public string firmarFile(string bearer, string strUsuarioFirma, string strUsuarioPassword, string strDocumento)
        //{


        //    try
        //    {
        //        var client = new RestClient(Constants.Address_FirmaElectronica);
        //        client.Timeout = -1;
        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("Authorization", bearer);
        //        request.AddHeader("Content-Type", "application/json");
        //        string body = @"{
        //        " + "\n" +
        //                        @"  ""User"": ""strUsuarioFirma"",
        //        " + "\n" +
        //                        @"  ""Password"": ""strUsuarioPassword"",
        //        " + "\n" +
        //                        @"  ""parentGoogleDriveId"":""strparentGoogleDriveId"",
        //        " + "\n" +
        //                        @"  ""Documentos"":[
        //        " + "\n" +
        //                        @"    {
        //        " + "\n" +
        //                        @"      ""GoogleDriveId"": ""strDocumento"",
        //        " + "\n" +
        //                        @"      ""Coordenadas"": ""70,700,180,760"",
        //        " + "\n" +
        //                        @"      ""NumeroPagina"": 1
        //        " + "\n" +
        //                        @"    }
        //        " + "\n" +
        //                        @" ]
        //        " + "\n" +
        //        @"}";

        //        //@"      ""Coordenadas"": ""260,1500,60,120"",


        //        body = body.Replace("strUsuarioFirma", strUsuarioFirma).Replace("strUsuarioPassword", strUsuarioPassword).Replace("strparentGoogleDriveId", Constants.parentGoogleDriveId);
        //        body = body.Replace("strDocumento", strDocumento);

        //        request.AddParameter("application/json", body, ParameterType.RequestBody);
        //        IRestResponse response = client.Execute(request);

        //        if (response.Content.ToString().IndexOf("Error") > 0)
        //        {
        //            return response.Content.ToString() + "  Usuario ó Password erroneo en firma.";
        //        }

        //        if (response.Content.ToString().IndexOf("connection") > 0)
        //        {
        //            return response.Content.ToString() + "  No hay conexion con el servidor de firmas." + response.Content.ToString();
        //        }


        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        try
        //        {
        //            Rootobject myDeserializedClass = JsonConvert.DeserializeObject<Rootobject>(response.Content.ToString());
        //            return myDeserializedClass.Data[0].GoogleDriveIdNuevo;
        //        }
        //        catch (Exception ex)
        //        {
        //            return strDocumento;
        //        }


        //        // Descomentar return myDeserializedClass.Data[0].GoogleDriveIdNuevo;

        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica
        //        //  Temporal por fallo en firma electronica

        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error al intentar firmar el archivo." + ex.Message.ToString();
        //    }


        //}

        public bool getFile(string strBearer, string strFile, string path)
        {
            try
            {
                var client = new RestClient(Constants.Address_GoogleDriveDownLoad + strFile);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", strBearer);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                byte[] buffer = client.DownloadData(request);

                MemoryStream ms = new MemoryStream(buffer);
                FileStream file = new FileStream(@path + strFile + ".pdf", FileMode.Create, FileAccess.Write);
                ms.WriteTo(file);
                file.Close();
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }

            // IRestResponse response = client.Execute(request);

        }

        public JsonResult JsonProcesarFirmaElectronica(string Guid_id, string Guidetapa_id, string UsuarioFE, string PasswordFE)
        {
            string strBearer;
            int intRespuesta;
            string rootbase, partialroot, partialrootDest;
            string jsonResultUsr;



            rootbase = Server.MapPath("~/");
            partialroot = $"/Archivos_Generados_Que_Pueden_Borrar/";
            string rootpath = Server.MapPath("~/") + "Archivos_Generados_Que_Pueden_Borrar/";
            string rootpdf = rootpath + "U" + Guidetapa_id + ".pdf";

            string rootpathDest = Server.MapPath("~/") + "Archivos_ConFirmaElectronica/";

            partialrootDest = $"/Archivos_ConFirmaElectronica/";

            Tbl_Gest_EtapaSolicitud Tbl_Gest_etapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.EtapaSolicitud_GUID_id == Guidetapa_id).First();

            /// bbarillas
            rootpdf = rootpath + Tbl_Gest_etapaSolicitud.NombreDocumentoNoFirmado;
            /// bbarillas

            strBearer = GetBearer();
            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure { respuesta = 0, mensaje = "No se ha realizado ninguna gestión", };
            string strEnc = UsuarioFE + " " + SecurEncryptDecrypt.EncryptString(UsuarioFE + " ___ " + PasswordFE);
            if (strBearer.Length < 125)
            {
                intRespuesta = 0;

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + intRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + "No se logró generar bearer de firma electrónica. Servicio de firma electrónica no disponible." + "\"}";

                return Json(jsonResultUsr);

            }

            string strDocumentoSubido = CallCORS(strBearer, rootpdf);

            if ((strDocumentoSubido.Length <= 30) || (strDocumentoSubido.Length >= 36))
            {

                intRespuesta = 0;

                strDocumentoSubido = strDocumentoSubido.Substring(strDocumentoSubido.IndexOf("}") + 1);

                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + "No se logró subir el documento para firma." + strDocumentoSubido + "\"}";

                return Json(jsonResultUsr);


            }

            RequestUtil requestUtil = new RequestUtil();

            string strDocumentofirmado = requestUtil.firmarFile(UsuarioFE, PasswordFE, strDocumentoSubido);

            //string strDocumentofirmado = firmarFile(strBearer, UsuarioFE, PasswordFE, strDocumentoSubido);

            if ((strDocumentofirmado.Length <= 30) || (strDocumentofirmado.Length >= 36))
            {


                // se cambio de 0 a 1 para pruebas de emananuel

                intRespuesta = 1;

                strDocumentofirmado = strDocumentofirmado.Substring(strDocumentofirmado.IndexOf("}") + 1);


                jsonResultUsr = "{\"CodRespuesta\":"
                                      + "\"" + intRespuesta + "\","
                                      + "\"strRespuesta\":" + "\"" + strDocumentofirmado + "\"}";

                return Json(jsonResultUsr);

            }


            if (getFile(strBearer, strDocumentofirmado, rootpathDest) == true)
            {
                //Tbl_Gest_etapaSolicitud.NombreDocumentoFirmado = strDocumentofirmado + ".pdf";

                //db.Entry(Tbl_Gest_etapaSolicitud).State = EntityState.Modified;
                //db.SaveChanges();




                string SP_SqlQuery = "EXEC [dbo].[SP_GestEtapaSolicitud_ActualizaDocumentoFirmado] @Solicitud_id, @Firmante, @GUID_id, @EtapaSolicitud_GUID_id, @NombreDocumentoFirmado";
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                        new SqlParameter { ParameterName = "@Solicitud_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Firmante", Value = strEnc, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@GUID_id", Value = Tbl_Gest_etapaSolicitud.Solicitud_Guid_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@EtapaSolicitud_GUID_id", Value = Tbl_Gest_etapaSolicitud.EtapaSolicitud_GUID_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@NombreDocumentoFirmado", Value = strDocumentofirmado + ".pdf", Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(SP_SqlQuery, sqlParameters).FirstOrDefault();

            }

            //intRespuesta = 1;

            //jsonResultUsr = "{\"CodRespuesta\":"
            //                      + "\"" + intRespuesta + "\","
            //                      + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

            intRespuesta = resultFromStoreProcedure.respuesta;

            if (intRespuesta == 1)
            {
                Constants.FirmaElectronicaInsertarBitacoraDelete(Guid_id, Guidetapa_id, UsuarioFE);


                jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + intRespuesta + "\","
                                  + "\"strRespuesta\":" + "\"" + partialrootDest + strDocumentofirmado + ".pdf" + "\"}";

            }
            else
            {
                jsonResultUsr = "{\"CodRespuesta\":"
                                  + "\"" + 0 + "\","
                                  + "\"strRespuesta\":" + "\"" + resultFromStoreProcedure.mensaje + "\"}";

            }

            return Json(jsonResultUsr);
        }


    }
}
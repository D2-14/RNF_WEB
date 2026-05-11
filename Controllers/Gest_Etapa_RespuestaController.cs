using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Gest_Etapa_RespuestaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: Gest_Etapa_Respuesta
        public ActionResult Index(int Etapaid, decimal EtapaRutaid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            @ViewBag.Nombre_Etapa = db.Tbl_Gest_Etapa.Where(Obj => Obj.EtapaRuta_id == EtapaRutaid && Obj.Etapa_id == Etapaid).First().Nombre_Etapa;

            ViewBag.NombreRuta = db.Database.SqlQuery<string>("Select Descripcion From Tbl_Gest_EtapaRuta Where EtapaRuta_id =@p0", EtapaRutaid).FirstOrDefault();


            ViewBag.Etapaid = Etapaid;

            ViewBag.EtapaRutaid = EtapaRutaid;

            return View(db.Tbl_Gest_Etapa_Respuesta.Where(Obj=>Obj.Etapa_id == Etapaid && Obj.EtapaRuta_id == EtapaRutaid).ToList());

        }
   
        public ActionResult Edit(int EtapaRespuestaid, int Etapaid, decimal EtapaRutaid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            return View();
        }

        // GET: Gest_Etapa_Respuesta/Create
        public ActionResult Create(int Etapaid, decimal EtapaRutaid)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_Gest_Etapa_Respuesta Tbl_Gest_Etapa_RespuestaNueva = new Tbl_Gest_Etapa_Respuesta();

            Tbl_Gest_Etapa_RespuestaNueva.Etapa_id = Etapaid;
            Tbl_Gest_Etapa_RespuestaNueva.EtapaRuta_id = EtapaRutaid;
            Tbl_Gest_Etapa_RespuestaNueva.Etapa_Respuesta_id = 0;
            Tbl_Gest_Etapa_RespuestaNueva.EtapaRSS_id = 0;

            Tbl_Gest_Etapa_RespuestaNueva.Descripcion = "";
            Tbl_Gest_Etapa_RespuestaNueva.Respuesta_default = false;
            Tbl_Gest_Etapa_RespuestaNueva.Solicitud_Nuevo_Estado_id = 1; 
            Tbl_Gest_Etapa_RespuestaNueva.Estado = true;

            ViewBag.Solicitud_Nuevo_Estado_id = new SelectList(db.Tbl_Sol_Solicitud_Estado, "Estado_id", "Descripcion", Tbl_Gest_Etapa_RespuestaNueva.Solicitud_Nuevo_Estado_id);

            ViewBag.EtapaRSS_id = new SelectList(db.Tbl_gest_EtapaRSS, "EtapaRSS_id", "Descripcion", Tbl_Gest_Etapa_RespuestaNueva.EtapaRSS_id);


            return View(Tbl_Gest_Etapa_RespuestaNueva);
        }

        // POST: Gest_Etapa_Respuesta/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gest_Etapa_Respuesta tbl_gest_etapa_respuestaNueva, FormCollection frmCollection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapa_respuestaNueva.RespuestaDeFinalizacion = (frmCollection["ChkRespuestaDeFinalizacion"] == null) ? false : true;
            tbl_gest_etapa_respuestaNueva.Estado = (frmCollection["ChkEstado"] == null) ? false : true;
            tbl_gest_etapa_respuestaNueva.Respuesta_default = (frmCollection["ChkRespuesta_default"] == null) ? false : true;


            tbl_gest_etapa_respuestaNueva.swcreatedby = objUs.intUsuario_id;
            tbl_gest_etapa_respuestaNueva.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gest_etapa_respuestaNueva.swcreatedbyinterno = false;
            }
            else
            {
                tbl_gest_etapa_respuestaNueva.swcreatedbyinterno = true;
            }


            tbl_gest_etapa_respuestaNueva.swupdatedby = objUs.intUsuario_id;
            tbl_gest_etapa_respuestaNueva.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gest_etapa_respuestaNueva.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gest_etapa_respuestaNueva.swupdatedbyinterno = true;

            }

            ViewBag.Solicitud_Nuevo_Estado_id = new SelectList(db.Tbl_Sol_Solicitud_Estado, "Estado_id", "Descripcion", tbl_gest_etapa_respuestaNueva.Solicitud_Nuevo_Estado_id);
            ViewBag.EtapaRSS_id = new SelectList(db.Tbl_gest_EtapaRSS, "EtapaRSS_id", "Descripcion", tbl_gest_etapa_respuestaNueva.EtapaRSS_id);

            int intKey = 0;

            try
            {
                intKey = db.Tbl_Gest_Etapa_Respuesta.Where(EtapaRespuesta => EtapaRespuesta.EtapaRuta_id == tbl_gest_etapa_respuestaNueva.EtapaRuta_id && EtapaRespuesta.Etapa_id == tbl_gest_etapa_respuestaNueva.Etapa_id ).Max(u => u.Etapa_Respuesta_id);
                intKey++;
            }
            catch
            {
                intKey = 1;
            }
            tbl_gest_etapa_respuestaNueva.Etapa_Respuesta_id = intKey;

            if (ModelState.IsValid)
            {
                db.Tbl_Gest_Etapa_Respuesta.Add(tbl_gest_etapa_respuestaNueva);
                db.SaveChanges();
                return RedirectToAction("../Gest_Etapa_Respuesta/ConfigurarEtapaRespuesta", new { etapaid = tbl_gest_etapa_respuestaNueva.Etapa_id, EtapaRutaid = tbl_gest_etapa_respuestaNueva.EtapaRuta_id, EtapaRespuestaid = tbl_gest_etapa_respuestaNueva.Etapa_Respuesta_id });

            }

            ViewBag.Gest_EtapaNuevaError = "No se puede grabar la etapa, favor revisar los datos";
            return View(tbl_gest_etapa_respuestaNueva);

        }
      
        public ActionResult ConfigurarEtapaRespuesta(int etapaid, decimal EtapaRutaid, int EtapaRespuestaid)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

 
            Tbl_Gest_Etapa_Respuesta Tbl_Gest_Etapa_RespuestaEditar = db.Tbl_Gest_Etapa_Respuesta.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == EtapaRutaid && Obj.Etapa_Respuesta_id == EtapaRespuestaid).First();

            Tbl_Gest_Etapa_RespuestaEditar.swupdatedby = objUs.intUsuario_id;
            Tbl_Gest_Etapa_RespuestaEditar.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                Tbl_Gest_Etapa_RespuestaEditar.swupdatedbyinterno = false;

            }
            else
            {
                Tbl_Gest_Etapa_RespuestaEditar.swupdatedbyinterno = true;

            }

            ViewBag.Solicitud_Nuevo_Estado_id = new SelectList(db.Tbl_Sol_Solicitud_Estado, "Estado_id", "Descripcion", Tbl_Gest_Etapa_RespuestaEditar.Solicitud_Nuevo_Estado_id);
            ViewBag.EtapaRSS_id = new SelectList(db.Tbl_gest_EtapaRSS, "EtapaRSS_id", "Descripcion", Tbl_Gest_Etapa_RespuestaEditar.EtapaRSS_id);

            return View(Tbl_Gest_Etapa_RespuestaEditar);
        }
        
        [HttpPost]
        public ActionResult ConfigurarEtapaRespuesta(Tbl_Gest_Etapa_Respuesta tbl_gest_etapa_respuestaNueva, FormCollection frmCollection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            tbl_gest_etapa_respuestaNueva.RespuestaDeFinalizacion = (frmCollection["ChkRespuestaDeFinalizacion"] == null) ? false : true;
            tbl_gest_etapa_respuestaNueva.Estado = (frmCollection["ChkEstado"] == null) ? false : true;
            tbl_gest_etapa_respuestaNueva.Respuesta_default = (frmCollection["ChkRespuesta_default"] == null) ? false : true;


            tbl_gest_etapa_respuestaNueva.swupdatedby = objUs.intUsuario_id;
            tbl_gest_etapa_respuestaNueva.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_gest_etapa_respuestaNueva.swupdatedbyinterno = false;

            }
            else
            {
                tbl_gest_etapa_respuestaNueva.swupdatedbyinterno = true;

            }

            ViewBag.Solicitud_Nuevo_Estado_id = new SelectList(db.Tbl_Sol_Solicitud_Estado, "Estado_id", "Descripcion", tbl_gest_etapa_respuestaNueva.Solicitud_Nuevo_Estado_id);
            ViewBag.EtapaRSS_id = new SelectList(db.Tbl_gest_EtapaRSS, "EtapaRSS_id", "Descripcion", tbl_gest_etapa_respuestaNueva.EtapaRSS_id);

            if (ModelState.IsValid)
            {
                db.Entry(tbl_gest_etapa_respuestaNueva).State = EntityState.Modified;
                db.SaveChanges();
                return View(tbl_gest_etapa_respuestaNueva);
            }

            ViewBag.Gest_EtapaNuevaError = "No se puede actualizar la etapa, favor revisar los datos";
            return View(tbl_gest_etapa_respuestaNueva);

        }

        public ActionResult ListadoEtapas(int etapaid, decimal etaparutaid, int etaparespuestaid)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            IQueryable<fc_Gest_Sel_Etapa_RespuestaRuta_Result> EtapaRespuestaRuta = db.fc_Gest_Sel_Etapa_RespuestaRuta(etapaid, etaparutaid, etaparespuestaid);

            return View(EtapaRespuestaRuta);

        }

        public ActionResult ListadoDocumentos(int etapaid, decimal etaparutaid, int etaparespuestaid)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            IQueryable<fc_Gest_Sel_Etapa_RespuestaDocumentoTipo_Result> DocumentoRespuestaRuta = db.fc_Gest_Sel_Etapa_RespuestaDocumentoTipo(etapaid, etaparutaid, etaparespuestaid);

            return View(DocumentoRespuestaRuta);

        }

        public ActionResult ListadoFormularios(int etapaid, decimal etaparutaid, int etaparespuestaid)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            IQueryable<fc_Gest_Sel_Etapa_RespuestaFormulario_Result> DocumentoRespuestaRuta = db.fc_Gest_Sel_Etapa_RespuestaFormulario(etapaid, etaparutaid, etaparespuestaid);


            return View(DocumentoRespuestaRuta);

        }




        public JsonResult CambiarEtapaRespuestaRuta(int etapaid, decimal etaparutaid, int etaparespuestaid, int nuevaetapaid, bool estado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int intEtapaRespuestaRuta = db.Tbl_Gest_Etapa_RespuestaRuta.Where(Obj => Obj.Etapa_Id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.NuevaEtapa_id == nuevaetapaid).Count();
            /// Update
            if (intEtapaRespuestaRuta > 0)
            {
                Tbl_Gest_Etapa_RespuestaRuta Tbl_Gest_Etapa_RespuestaRutaActualizar = db.Tbl_Gest_Etapa_RespuestaRuta.Where(Obj => Obj.Etapa_Id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.NuevaEtapa_id == nuevaetapaid).First();



                Tbl_Gest_Etapa_RespuestaRutaActualizar.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaRutaActualizar.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaRutaActualizar.swupdatedbyinterno= false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaRutaActualizar.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaRutaActualizar.Estado = estado;


                db.Entry(Tbl_Gest_Etapa_RespuestaRutaActualizar).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                Tbl_Gest_Etapa_RespuestaRuta Tbl_Gest_Etapa_RespuestaRutaNueva = new Tbl_Gest_Etapa_RespuestaRuta();

                Tbl_Gest_Etapa_RespuestaRutaNueva.Etapa_Id = etapaid;
                Tbl_Gest_Etapa_RespuestaRutaNueva.EtapaRuta_id = etaparutaid;
                Tbl_Gest_Etapa_RespuestaRutaNueva.Etapa_Respuesta_id = etaparespuestaid;
                Tbl_Gest_Etapa_RespuestaRutaNueva.NuevaEtapa_id = nuevaetapaid;


                Tbl_Gest_Etapa_RespuestaRutaNueva.swcreatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaRutaNueva.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaRutaNueva.swcreatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaRutaNueva.swcreatedbyinterno = true;

                }


                Tbl_Gest_Etapa_RespuestaRutaNueva.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaRutaNueva.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaRutaNueva.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaRutaNueva.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaRutaNueva.Estado = estado;

                db.Tbl_Gest_Etapa_RespuestaRuta.Add(Tbl_Gest_Etapa_RespuestaRutaNueva);
                db.SaveChanges();
            }

            return Json("");
        }

        public JsonResult CambiarEtapaRespuestaDocumentoObligatorio(int etapaid, decimal etaparutaid, int etaparespuestaid, int TipoDocumentoid, bool estado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int intEtapaRespuestaRuta = db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.Tipo_Documento_id == TipoDocumentoid).Count();
            /// Update
            if (intEtapaRespuestaRuta > 0)
            {
                Tbl_Gest_Etapa_RespuestaDocumentoTipo Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar = db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.Tipo_Documento_id == TipoDocumentoid).First();



                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.Obligatorio = estado;


                db.Entry(Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                Tbl_Gest_Etapa_RespuestaDocumentoTipo Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo = new Tbl_Gest_Etapa_RespuestaDocumentoTipo();

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Etapa_id = etapaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.EtapaRuta_id = etaparutaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Etapa_Respuesta_id = etaparespuestaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Tipo_Documento_id = TipoDocumentoid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedbyinterno = true;

                }


                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Estado = false;

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Obligatorio = estado;

                db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Add(Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo);
                db.SaveChanges();
            }

            return Json("");
        }


        public JsonResult CambiarEtapaRespuestaFormularioObligatorio(int etapaid, decimal etaparutaid, int etaparespuestaid, int TipoFormularioid, bool estado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int intEtapaRespuestaRuta = db.Tbl_Gest_Etapa_RespuestaFormulario.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.TipoFormulario_id == TipoFormularioid).Count();
            /// Update
            if (intEtapaRespuestaRuta > 0)
            {
                Tbl_Gest_Etapa_RespuestaFormulario Tbl_Gest_Etapa_RespuestaFormularioActualizar = db.Tbl_Gest_Etapa_RespuestaFormulario.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.TipoFormulario_id == TipoFormularioid).First();

                Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioActualizar.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaFormularioActualizar.Obligatorio = estado;


                db.Entry(Tbl_Gest_Etapa_RespuestaFormularioActualizar).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                Tbl_Gest_Etapa_RespuestaFormulario Tbl_Gest_Etapa_RespuestaFormularioNuevo = new Tbl_Gest_Etapa_RespuestaFormulario();

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Etapa_id = etapaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.EtapaRuta_id = etaparutaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Etapa_Respuesta_id = etaparespuestaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.TipoFormulario_id = TipoFormularioid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedbyinterno = true;

                }


                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Estado = false;

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Obligatorio = estado;

                db.Tbl_Gest_Etapa_RespuestaFormulario.Add(Tbl_Gest_Etapa_RespuestaFormularioNuevo);
                db.SaveChanges();
            }

            return Json("");
        }



        public JsonResult CambiarEtapaRespuestaFormularioEstatus(int etapaid, decimal etaparutaid, int etaparespuestaid, int TipoFormularioid, bool estado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int intEtapaRespuestaRuta = db.Tbl_Gest_Etapa_RespuestaFormulario.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.TipoFormulario_id == TipoFormularioid).Count();
            /// Update
            if (intEtapaRespuestaRuta > 0)
            {
                Tbl_Gest_Etapa_RespuestaFormulario Tbl_Gest_Etapa_RespuestaFormularioActualizar = db.Tbl_Gest_Etapa_RespuestaFormulario.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.TipoFormulario_id == TipoFormularioid).First();



                Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioActualizar.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioActualizar.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaFormularioActualizar.Estado = estado;


                db.Entry(Tbl_Gest_Etapa_RespuestaFormularioActualizar).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                Tbl_Gest_Etapa_RespuestaFormulario Tbl_Gest_Etapa_RespuestaFormularioNuevo = new Tbl_Gest_Etapa_RespuestaFormulario();

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Etapa_id = etapaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.EtapaRuta_id = etaparutaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Etapa_Respuesta_id = etaparespuestaid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.TipoFormulario_id = TipoFormularioid;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swcreatedbyinterno = true;

                }


                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaFormularioNuevo.swupdatedbyinterno = true;

                }
                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaFormularioNuevo.Estado = estado;

                db.Tbl_Gest_Etapa_RespuestaFormulario.Add(Tbl_Gest_Etapa_RespuestaFormularioNuevo);
                db.SaveChanges();
            }

            return Json("");
        }



        public JsonResult CambiarEtapaRespuestaDocumentoEstatus(int etapaid, decimal etaparutaid, int etaparespuestaid, int TipoDocumentoid, bool estado)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return Json("");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int intEtapaRespuestaRuta = db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.Tipo_Documento_id == TipoDocumentoid).Count();
            /// Update
            if (intEtapaRespuestaRuta > 0)
            {
                Tbl_Gest_Etapa_RespuestaDocumentoTipo Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar = db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Where(Obj => Obj.Etapa_id == etapaid && Obj.EtapaRuta_id == etaparutaid && Obj.Etapa_Respuesta_id == etaparespuestaid && Obj.Tipo_Documento_id == TipoDocumentoid).First();



                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.swupdatedbyinterno = true;

                }

                Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar.Estado = estado;


                db.Entry(Tbl_Gest_Etapa_RespuestaDocumentoTipoActualizar).State = EntityState.Modified;
                db.SaveChanges();


            }
            else
            {
                Tbl_Gest_Etapa_RespuestaDocumentoTipo Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo = new Tbl_Gest_Etapa_RespuestaDocumentoTipo();

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Etapa_id = etapaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.EtapaRuta_id = etaparutaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Etapa_Respuesta_id = etaparespuestaid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Tipo_Documento_id = TipoDocumentoid;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swcreatedbyinterno = true;

                }


                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedby = objUs.intUsuario_id;
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedbyinterno = false;

                }
                else
                {
                    Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.swupdatedbyinterno = true;

                }
                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Obligatorio = false;

                Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo.Estado = estado;

                db.Tbl_Gest_Etapa_RespuestaDocumentoTipo.Add(Tbl_Gest_Etapa_RespuestaDocumentoTipoNuevo);
                db.SaveChanges();
            }

            return Json("");
        }


    }
}

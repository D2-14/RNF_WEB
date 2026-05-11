using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using Newtonsoft.Json;

namespace RNF_Web.Controllers
{
    public class Sol_Motosierra_MarcaModeloController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Motosierrra_MarcaModelo
        public ActionResult Index(long Solicitud_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            return View(db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == Solicitud_id).OrderBy(Obj => Obj.Motosierra_id).ToList());
        }

        public ActionResult Create (long Solicitud_id)
        {
            ViewBag.Solicitud_id = Solicitud_id;
            return View();
        }

        class RespuestaJson
        {
            public int CodRespuesta { get; set; }
            public string strRespuesta { get; set; }
        }

        public JsonResult AgregarMotosierraMarcaModelo(Tbl_Sol_Motosierra_Marca_Modelo model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            bool ErrorDetectado = false;
            bool boolEsInterno = false;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                ErrorDetectado = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            DateTime swdatecreated = DateTime.Now;
            RespuestaJson respuestaJson = new RespuestaJson();


            if (ErrorDetectado)
            {

                respuestaJson.CodRespuesta = 0;
                respuestaJson.strRespuesta = "No posee una sesión válida";

            }
            else
            {
                Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(model.Solicitud_id);

                if(tbl_Sol_Solicitud != null)
                {

                    fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Solicitud.Solicitud_id, "Sol_Motosierra_MarcaModelo", boolEsInterno).FirstOrDefault();

                    if ((bool)permisos.Agregar)
                    {

                        int intIdt = 0;

                        try
                        {
                            intIdt = db.Tbl_Sol_Motosierra_Marca_Modelo.Where(Obj => Obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).Max(u => u.Motosierra_id);
                            intIdt++;

                        }
                        catch(Exception ex)
                        {
                            intIdt = 1;
                        }

                        Tbl_Sol_Motosierra_Marca_Modelo tbl_Sol_Motosierra_Marca_Modelo = new Tbl_Sol_Motosierra_Marca_Modelo();
                        tbl_Sol_Motosierra_Marca_Modelo.Solicitud_id = tbl_Sol_Solicitud.Solicitud_id;
                        tbl_Sol_Motosierra_Marca_Modelo.Motosierra_id = intIdt;
                        tbl_Sol_Motosierra_Marca_Modelo.Marca = model.Marca;
                        tbl_Sol_Motosierra_Marca_Modelo.Modelo = model.Modelo;
                        tbl_Sol_Motosierra_Marca_Modelo.swdatecreated = swdatecreated;
                        tbl_Sol_Motosierra_Marca_Modelo.swcreatedbyinterno = boolEsInterno;
                        tbl_Sol_Motosierra_Marca_Modelo.swcreatedby = objUs.intUsuario_id;
                        db.Tbl_Sol_Motosierra_Marca_Modelo.Add(tbl_Sol_Motosierra_Marca_Modelo);
                        db.SaveChanges();
                        respuestaJson.CodRespuesta = 1;
                        respuestaJson.strRespuesta = "Registro agregado exitosamente";

                    }

                }
            }



            return Json(JsonConvert.SerializeObject(respuestaJson));
        }
   
        public JsonResult EliminarMotosierraMarcaModelo(
            long Solicitud_id,
            int Motosierra_id,
            string firma
        )
        {
            RespuestaJson respuestaJson = new RespuestaJson();


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                respuestaJson.CodRespuesta = 0;
                respuestaJson.strRespuesta = "No posee una firma válida";
                return Json(JsonConvert.SerializeObject(respuestaJson));
            }

            if (tbl_sol_solicitud.Guid_id != firma)
            {
                respuestaJson.CodRespuesta = 0;
                respuestaJson.strRespuesta = "No posee una firma válida";
                return Json(JsonConvert.SerializeObject(respuestaJson));
            }



            Tbl_Sol_Motosierra_Marca_Modelo model = new Tbl_Sol_Motosierra_Marca_Modelo();

            model.Solicitud_id = Solicitud_id;
            model.Motosierra_id = Motosierra_id;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            bool ErrorDetectado = false;
            bool boolEsInterno = false;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                ErrorDetectado = true;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            DateTime swdatecreated = DateTime.Now;


            if (ErrorDetectado)
            {

                respuestaJson.CodRespuesta = 0;
                respuestaJson.strRespuesta = "No posee una sesión válida";

            }
            else
            {

                fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(model.Solicitud_id, "Sol_Motosierra_MarcaModelo", boolEsInterno).FirstOrDefault();

                if ((bool)permisos.Borrar)
                {

                    Tbl_Sol_Motosierra_Marca_Modelo tbl_Sol_Motosierra_Marca_Modelo = (from d in db.Tbl_Sol_Motosierra_Marca_Modelo
                                                                                       where d.Solicitud_id == model.Solicitud_id && d.Motosierra_id == model.Motosierra_id
                                                                                       select d).FirstOrDefault();

                    if(tbl_Sol_Motosierra_Marca_Modelo != null)
                    {
                        db.Tbl_Sol_Motosierra_Marca_Modelo.Remove(tbl_Sol_Motosierra_Marca_Modelo);
                        db.SaveChanges();
                        respuestaJson.CodRespuesta = 1;
                        respuestaJson.strRespuesta = "Registro elminado exitosamente";
                    }
                    else
                    {
                        respuestaJson.CodRespuesta = 2;
                        respuestaJson.strRespuesta = "Error, este registro ya no existe";
                    }

                }
                else
                {
                    respuestaJson.CodRespuesta = 3;
                    respuestaJson.strRespuesta = "Error, no tiene permiso para eliminar este registro";
                }

            }

            return Json(JsonConvert.SerializeObject(respuestaJson));
        }
    }
}
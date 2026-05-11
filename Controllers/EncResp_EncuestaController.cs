using Microsoft.Office.Interop.Word;
using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace RNF_Web.Controllers
{
    public class EncResp_EncuestaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        private db_RNF_SurveyEntities db_Survey = new db_RNF_SurveyEntities(); 

        // GET: EncResp_Encuesta
        public ActionResult Index(string No_Registro, string Guid_id)
        {

            ViewBag.No_Registro = No_Registro;
            ViewBag.Guid_id = Guid_id;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int tbl_RNF_RegistroCount = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro && Obj.Guid_id == Guid_id).Count();

            if (tbl_RNF_RegistroCount == 0)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(db_Survey.Tbl_EncResp_Encuesta.Where(Obj=>Obj.No_Registro == No_Registro).ToList());

        }


        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
        }

        public JsonResult RealizarEncuesta(string No_Registro, string Guid_id)
        {


            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha realizado ninguna gestión",
            };
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                jsonRespuesta.Mensaje = "Sesión finalizada, favor loguearse de nuevo.";
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro && Obj.Guid_id == Guid_id).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                jsonRespuesta.Mensaje = "Número de registro invalido.";
                return Json(jsonRespuesta);
            }


            Tbl_EncResp_Encuesta tbl_EncResp_encuesta = new Tbl_EncResp_Encuesta();
            try
            {

                tbl_EncResp_encuesta.No_Registro = No_Registro;
                tbl_EncResp_encuesta.id_encuesta = db_Survey.Tbl_Enc_EncuestaXCategoria.Where(Obj => Obj.id_categoria == tbl_RNF_Registro.Categoria_id).First().id_encuesta;
                tbl_EncResp_encuesta.swcreatedby = objUs.intUsuario_id;
                tbl_EncResp_encuesta.swdatecreated = DateTime.Now;
                tbl_EncResp_encuesta.id_estado = 0;
                tbl_EncResp_encuesta.fecha_inicio = DateTime.Now;
                tbl_EncResp_encuesta.fecha_final = DateTime.Now;
                tbl_EncResp_encuesta.swupdatedby = objUs.intUsuario_id;
                tbl_EncResp_encuesta.swdateupdated = DateTime.Now;

                int lngIdt = 0;

                try
                {
                    lngIdt = db_Survey.Tbl_EncResp_Encuesta.Max(u => u.id_Correlativo);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_EncResp_encuesta.id_Correlativo = lngIdt;


                db_Survey.Tbl_EncResp_Encuesta.Add(tbl_EncResp_encuesta);
                db_Survey.SaveChanges();

                jsonRespuesta.Mensaje = "Registro ingresado de forma exitosa.";
                jsonRespuesta.Result = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                jsonRespuesta.Mensaje = "Error: " + ex;
                jsonRespuesta.Result = 0;
            }

            return Json(jsonRespuesta);

        }

    }
}





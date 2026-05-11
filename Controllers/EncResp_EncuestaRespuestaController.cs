using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RNF_Web.Models.Constants;

namespace RNF_Web.Controllers
{
    public class EncResp_EncuestaRespuestaController : Controller
    {

        private db_RNF_SurveyEntities db_Survey = new db_RNF_SurveyEntities();
        private db_RNFEntities  db = new db_RNFEntities();

        // GET: EncResp_EncuestaRespuesta
        public ActionResult Index(int id_encuesta, long swcreatedby, int id_correlativo)
        {

            IEnumerable<Tbl_EncResp_Encuesta_Pregunta> tbl_EncResp_Encuesta_Pregunta = db_Survey.Tbl_EncResp_Encuesta_Pregunta.Where(Obj => Obj.id_encuesta == id_encuesta && Obj.swcreatedby == swcreatedby && Obj.id_Correlativo == id_correlativo);

            return View(tbl_EncResp_Encuesta_Pregunta);
        }

        public ActionResult PreguntaActual(int id_encuesta, long swcreatedby, int id_correlativo)
        {

            Tbl_EncResp_Encuesta tbl_EncResp_Encuesta = db_Survey.Tbl_EncResp_Encuesta.Where(Obj => Obj.id_encuesta == id_encuesta && Obj.swcreatedby == swcreatedby && Obj.id_Correlativo == id_correlativo).FirstOrDefault();

            if (tbl_EncResp_Encuesta.id_estado == 2)
            {
                return RedirectToAction("EncuestaFinalizada", "EncResp_EncuestaRespuesta", new { id_encuesta = id_encuesta,  swcreatedby = swcreatedby,  id_correlativo = id_correlativo });

            }

            ViewBag.id_encuesta = id_encuesta;
            ViewBag.swcreatedby = swcreatedby;
            ViewBag.id_correlativo = id_correlativo;
            int MaxPregunta = 0;
            try
            {
                MaxPregunta = db_Survey.Tbl_EncResp_Encuesta_Pregunta.Where(Obj => Obj.id_encuesta == id_encuesta && Obj.swcreatedby == swcreatedby && Obj.id_Correlativo == id_correlativo).Max(u => u.id_pregunta);
            }
            catch
            {
                MaxPregunta = 0;
            }
            Tbl_Enc_Encuesta_Pregunta Tbl_Enc_Encuesta_Pregunta;

            Tbl_Enc_Encuesta_Pregunta = db_Survey.Tbl_Enc_Encuesta_Pregunta.Where(Obj => Obj.id_encuesta == id_encuesta).First();

            if (MaxPregunta != 0)
            {
                Tbl_Enc_Encuesta_Pregunta = db_Survey.Tbl_Enc_Encuesta_Pregunta.Where(Obj => Obj.id_encuesta == id_encuesta && Obj.id_pregunta == MaxPregunta).First();
            }

            return View(Tbl_Enc_Encuesta_Pregunta);

        }

        public ActionResult EncuestaFinalizada(int id_encuesta, long swcreatedby, int id_correlativo)
        {
            ViewBag.id_encuesta = id_encuesta;
            ViewBag.swcreatedby = swcreatedby;
            ViewBag.id_correlativo = id_correlativo;

            return View();
        }

        public class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
        }



        public JsonResult EnviarRespuesta(int id_encuesta, long swcreatedby, int id_correlativo, int id_pregunta, string RespuestaObservaciones)
        {

            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha grabado la respuesta",
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

            try
            {
                ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure();

                string sqlQuery = "exec Sp_EncResp_Encuesta_Pregunta @id_encuesta, @swcreatedby, @id_Correlativo, @id_pregunta, @Respuesta_Observaciones, @swupdatedby";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                   new SqlParameter { ParameterName = "@id_encuesta",  Value = id_encuesta, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@swcreatedby",  Value = swcreatedby, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@id_Correlativo",  Value = id_correlativo, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@id_pregunta",  Value = id_pregunta, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Respuesta_Observaciones",  Value = RespuestaObservaciones, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db_Survey.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                jsonRespuesta.Mensaje = resultFromStoreProcedure.mensaje;
                jsonRespuesta.Result = resultFromStoreProcedure.respuesta;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                jsonRespuesta.Mensaje = "Error: " + ex;
                jsonRespuesta.Result = 0;
            }

            return Json(jsonRespuesta);

        }


        public JsonResult EnviarRespuestaEspecifica(int id_encuesta, long swcreatedby, int id_correlativo, int id_pregunta, int id_respuesta, Boolean Aplica, string RespuestaObservaciones)
        {

            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha grabado la respuesta",
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

            try
            {
                ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure();

                string sqlQuery = "exec Sp_EncResp_Encuesta_PreguntaEspecifica @id_encuesta, @swcreatedby, @id_Correlativo, @id_pregunta, @id_respuesta, @Aplica, @Respuesta_Observaciones, @swupdatedby";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                   new SqlParameter { ParameterName = "@id_encuesta",  Value = id_encuesta, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@swcreatedby",  Value = swcreatedby, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@id_Correlativo",  Value = id_correlativo, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@id_pregunta",  Value = id_pregunta, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@id_respuesta", Value = id_respuesta, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Aplica", Value = Aplica, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@Respuesta_Observaciones",  Value = RespuestaObservaciones, Direction = System.Data.ParameterDirection.Input },
                   new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                };

                resultFromStoreProcedure = db_Survey.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                jsonRespuesta.Mensaje = resultFromStoreProcedure.mensaje;
                jsonRespuesta.Result = resultFromStoreProcedure.respuesta;

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
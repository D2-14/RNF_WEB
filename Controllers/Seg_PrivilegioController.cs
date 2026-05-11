using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Seg_PrivilegioController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        // GET: Seg_Privilegio
        public ActionResult Index(int rol_id)
        {

            List<fc_Seg_Privilegio_Result>  Lst_seg_Privilegio = db.fc_Seg_Privilegio(rol_id).ToList();

            return View(Lst_seg_Privilegio);

        }

        //var data = { empresa_id: varEmpresa_id, modulo_id: varModulo_id, opciontipo_id: varOpcionTipo_id, opcion_id: varOpcion_id, rol_id: varRol_id, acceso : varAcceso };

        //public JsonResult CambioPrivilegios(int empresa_id, int modulo_id, int opciontipo_id, int Opcion_id, int Rol_id, int Acceder)

        [HttpPost]
        public JsonResult CambioPrivilegios(int empresa_id, int modulo_id, int opciontipo_id, int Opcion_id, int Rol_id, int Acceder, int Tipo)
        {


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            Usuario objUs = (Usuario)Session["User"];

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                       { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };


            string sqlQuery = "Exec SP_Seg_InsUpd_Privilegio @Empresa_id, @Modulo_id, @OpcionTipo_id, @Opcion_id, @Rol_id, @Acceder, @Tipo, @Usuario_id";
            SqlParameter[] sqlParams;

            bool boolAcceder = false;
            if (Acceder == 1)
            {
                boolAcceder = true;
            }


            sqlParams = new SqlParameter[]
           {
                               new SqlParameter { ParameterName = "@Empresa_id",  Value = empresa_id, Direction = System.Data.ParameterDirection.Input },
                               new SqlParameter { ParameterName = "@Modulo_id",  Value = modulo_id, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@OpcionTipo_id",  Value = opciontipo_id, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@Opcion_id",  Value = Opcion_id, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@Rol_id",  Value = Rol_id, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@Acceder",  Value =  boolAcceder, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@Tipo",  Value =  Tipo, Direction = System.Data.ParameterDirection.Input},
                               new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input}
           };



            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            string TextoMostrar = resultado[0].mensaje;


            return Json(new { success = TextoMostrar }, JsonRequestBehavior.AllowGet);

        }


    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class ExpedienteOficio_GestorController : Controller
    {
        db_RNF_IntermediaEntities db_Intermedia = new db_RNF_IntermediaEntities();
        RequestUtil RequestUtil = new RequestUtil();
        // GET: ExpedienteOficio_Gestor
        public ActionResult Index()
        {
            string respuesta = RequestUtil.ObtieneArchivoFirmado <Credentials_GetBearer> (Constants.Address_GoogleDriveDownLoad, "GET", null, "1QcMsdlvKzWVMJTWwXGAMNsA74I7cc7Q8");

            return View(model: respuesta);
        }

        public ActionResult Expediente_PinpepOld_Listar(string Expediente)
        {
            string strSqlQuery = $"SELECT *\n";
            strSqlQuery += $"FROM Tbl_API_PinpepOld_Expediente\n";
            if((Expediente ?? "").Trim() != "")
            {
                strSqlQuery += $"WHERE Expediente LIKE '%{Expediente}%'\n";
            }
            List<Tbl_API_PinpepOld_Expediente> tbl_API_PinpepOld_Expedientes = db_Intermedia.Database.SqlQuery<Tbl_API_PinpepOld_Expediente>(strSqlQuery).ToList();

            return View(tbl_API_PinpepOld_Expedientes);
        }
        public ActionResult Expediente_Probosque_Listar(string Expediente)
        {
            string strSqlQuery = $"SELECT *\n";
            strSqlQuery += $"FROM Tbl_API_Probosque_Expediente\n";
            if ((Expediente ?? "").Trim() != "")
            {
                strSqlQuery += $"WHERE Expediente LIKE '%{Expediente}%'\n";
            }
            List<Tbl_API_Probosque_Expediente> tbl_API_Probosque_Expedientes = db_Intermedia.Database.SqlQuery<Tbl_API_Probosque_Expediente>(strSqlQuery).ToList();

            return View(tbl_API_Probosque_Expedientes);
        }




    }
}
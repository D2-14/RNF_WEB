using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_Finca_FuenteSemilleraController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_Solicitud_FuenteSemillera/Edit/5
        public ActionResult Edit_DatosFuenteSemillera(long solicitudid, long finca_id)
        {
            Tbl_Sol_Solicitud tbl_sol_Solicitud = db.Tbl_Sol_Solicitud.Find(solicitudid);

            ViewBag.Sub_Categoria_id = tbl_sol_Solicitud.Sub_Categoria_id;

            ViewBag.Sub_Sub_Categoria_id = tbl_sol_Solicitud.Sub_Categoria_id;

            int intEncontrado = db.Tbl_Sol_Finca_FuenteSemillera.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == finca_id).Count();

            Tbl_Sol_Finca_FuenteSemillera tbl_Sol_Solicitud_FuenteSemillera;

            if (intEncontrado == 0)
            {
                tbl_Sol_Solicitud_FuenteSemillera = new Tbl_Sol_Finca_FuenteSemillera();
                tbl_Sol_Solicitud_FuenteSemillera.Solicitud_id = solicitudid;
                tbl_Sol_Solicitud_FuenteSemillera.Finca_id = finca_id;
                tbl_Sol_Solicitud_FuenteSemillera.PresentaAlgunaPlaga = false;
                tbl_Sol_Solicitud_FuenteSemillera.Procedencia_id = 0;
                tbl_Sol_Solicitud_FuenteSemillera.BosqueIntervenido = false;
                tbl_Sol_Solicitud_FuenteSemillera.AclareoGenetico = false;
                tbl_Sol_Solicitud_FuenteSemillera.TipoDeEnsayo_id = 0;
                tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaDepartamento_id = 7;
                tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaMunicipio_id = 74;

                tbl_Sol_Solicitud_FuenteSemillera.swcreatedby = 1;
                tbl_Sol_Solicitud_FuenteSemillera.swdatecreated = DateTime.Now;
                tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaPais_id = Constants.codigoPaisGuatemala;
            }
            else
            {
                 tbl_Sol_Solicitud_FuenteSemillera = db.Tbl_Sol_Finca_FuenteSemillera.Where(Obj => Obj.Solicitud_id == solicitudid && Obj.Finca_id == finca_id).FirstOrDefault();
            }

            tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaDepartamento_id = tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaDepartamento_id??7;
            tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaMunicipio_id = tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaMunicipio_id??74;
            tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaPais_id = tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaPais_id??Constants.codigoPaisGuatemala;


            ViewBag.TipoDeEnsayo_id = new SelectList(db.Tbl_Gral_FuenteSemillera_TipoEnsayo, "TipoDeEnsayo_id", "Descripcion", tbl_Sol_Solicitud_FuenteSemillera.TipoDeEnsayo_id);

            ViewBag.Procedencia_id = new SelectList(db.Tbl_Gral_FuenteSemillera_Procedencia, "Procedencia_id", "Descripcion", tbl_Sol_Solicitud_FuenteSemillera.Procedencia_id);

            ViewBag.FincaProcedenciaDepartamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaDepartamento_id);

            ViewBag.FincaProcedenciaMunicipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaDepartamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaMunicipio_id);

            ViewBag.FincaProcedenciaPais_id = new SelectList(db.Tbl_Gral_Pais, "Pais_id", "Pais", tbl_Sol_Solicitud_FuenteSemillera.FincaProcedenciaPais_id?? Constants.codigoPaisGuatemala);

            return View(tbl_Sol_Solicitud_FuenteSemillera);

        }

       [HttpPost]
        public JsonResult Edit_DatosFuenteSemillera(Tbl_Sol_Finca_FuenteSemillera modelo)
        {

            int codRespuesta = 1;
            string strRespuesta = "Actualización de datos realizada.";
            try
            {
                Tbl_Sol_Finca_FuenteSemillera ModelBorrar = db.Tbl_Sol_Finca_FuenteSemillera.Where(Obj => Obj.Solicitud_id == modelo.Solicitud_id && Obj.Finca_id == modelo.Finca_id).First();

                db.Tbl_Sol_Finca_FuenteSemillera.Remove(ModelBorrar);
            }
            catch (Exception ex)
            {
                codRespuesta = 1;
            }

            db.Tbl_Sol_Finca_FuenteSemillera.Add(modelo);
            db.SaveChanges();


            string jsonResultUsr = "{\"CodRespuesta\":"
                        + "\"" + codRespuesta + "\","
                        + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);
        }




    }
}

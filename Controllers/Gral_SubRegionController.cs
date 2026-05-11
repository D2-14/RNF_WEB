using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using RNF_Web.Models;
namespace RNF_Web.Controllers
{
    public class Gral_SubRegionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Gral_SubRegion
        public ActionResult Index(int id)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            var tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == id && Obj.SubRegion_id > 0);
            return View(tbl_Gral_SubRegion);

        }


        // GET: Gral_SubRegion/Create
        public ActionResult Create(int id)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            Tbl_Gral_SubRegion tbl_Gral_SubRegion = new Tbl_Gral_SubRegion();
            tbl_Gral_SubRegion.Region_id = id;
            int intIdt = 0;

            try
            {
                intIdt = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == id).Max(u => u.SubRegion_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }
            tbl_Gral_SubRegion.SubRegion_id = intIdt;

            tbl_Gral_SubRegion.DepartamentoSubRegion_id = 7;

            tbl_Gral_SubRegion.MunicipioSubRegion_id = 74;

            ViewBag.DepartamentoSubRegion_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Gral_SubRegion.DepartamentoSubRegion_id);
            ViewBag.MunicipioSubRegion_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == 7), "Municipio_id", "Municipio", tbl_Gral_SubRegion.MunicipioSubRegion_id);



            return View(tbl_Gral_SubRegion);

        }

        // POST: Gral_SubRegion/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gral_SubRegion tbl_Gral_SubRegion)
        {
            int intIdt = 0;
      
            try
            {
                intIdt = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_Gral_SubRegion.Region_id).Max(u => u.SubRegion_id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            ViewBag.DepartamentoSubRegion_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Gral_SubRegion.DepartamentoSubRegion_id);
            ViewBag.MunicipioSubRegion_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == 7), "Municipio_id", "Municipio", tbl_Gral_SubRegion.MunicipioSubRegion_id);

            tbl_Gral_SubRegion.SubRegion_id = intIdt;
            tbl_Gral_SubRegion.Estado_id = true;
                try
                {
                    db.Tbl_Gral_SubRegion.Add(tbl_Gral_SubRegion);
                    db.SaveChanges();

                return RedirectToAction("Edit", "Gral_Region", new { id = tbl_Gral_SubRegion.Region_id });
                }
                catch
                {
                    return View(tbl_Gral_SubRegion);
                }
            
        }

        // GET: Gral_SubRegion/Edit/5
        public ActionResult Edit(int region_id, int subregionid)
        {

            Tbl_Gral_SubRegion tbl_Gral_SubRegion = db.Tbl_Gral_SubRegion.Where(SubRegion => SubRegion.Region_id == region_id && SubRegion.SubRegion_id == subregionid).First();


            ViewBag.DepartamentoSubRegion_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Gral_SubRegion.DepartamentoSubRegion_id);
            ViewBag.MunicipioSubRegion_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Gral_SubRegion.DepartamentoSubRegion_id), "Municipio_id", "Municipio", tbl_Gral_SubRegion.MunicipioSubRegion_id);


            return View(tbl_Gral_SubRegion);
        }

        // POST: Gral_SubRegion/Edit/5

        [HttpPost]
        public JsonResult GrabarSubRegion(Tbl_Gral_SubRegion tbl_Gral_SubRegionCambios)
        {
          
            string jsonResult;
            string strRespuesta = "Error: No se logró realizar la actualización.";
            int codRespuesta = 0;
            long strUsuarioid = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;


            jsonResult = "{\"CodRespuesta\":"
                      + "\"" + codRespuesta + "\","
                      + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                      + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

            if (!objSesion.getBlSession())
            {

                strRespuesta = "Error: Debe estar dentro del sistema para poder actualizar datos.";

                jsonResult = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                          + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

                return Json(jsonResult);


            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_Gral_SubRegion tbl_gral_subRegion = db.Tbl_Gral_SubRegion.Where(Obj => Obj.Region_id == tbl_Gral_SubRegionCambios.Region_id && Obj.SubRegion_id == tbl_Gral_SubRegionCambios.SubRegion_id).FirstOrDefault();


            tbl_gral_subRegion.swdateupdated = DateTime.Now;
            tbl_gral_subRegion.swupdatedby = objUs.intUsuario_id;

            tbl_gral_subRegion.No_SubRegion = tbl_Gral_SubRegionCambios.No_SubRegion;
            tbl_gral_subRegion.Nombre_SubRegion = tbl_Gral_SubRegionCambios.Nombre_SubRegion;
            tbl_gral_subRegion.Id_RNF_S = tbl_Gral_SubRegionCambios.Id_RNF_S;
            tbl_gral_subRegion.Direccion = tbl_Gral_SubRegionCambios.Direccion;
            tbl_gral_subRegion.DepartamentoSubRegion_id = tbl_Gral_SubRegionCambios.DepartamentoSubRegion_id;
            tbl_gral_subRegion.MunicipioSubRegion_id = tbl_Gral_SubRegionCambios.MunicipioSubRegion_id;
            tbl_gral_subRegion.Telefono = tbl_Gral_SubRegionCambios.Telefono;
            tbl_gral_subRegion.Estado_id = tbl_Gral_SubRegionCambios.Estado_id;

            try
            {
                db.Entry(tbl_gral_subRegion).State = EntityState.Modified;
                db.SaveChanges();


                strRespuesta = "Actualización realizada.";

                jsonResult = "{\"CodRespuesta\":"
                          + "\"" + 1 + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                          + "\"strUsuarioid\":" + "\"" + 1 + "\"}";


                return Json(jsonResult);
            }
            catch
            {
                return Json(jsonResult); 
            }

        }

        // GET: Gral_SubRegion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Gral_SubRegion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

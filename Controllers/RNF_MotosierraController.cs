using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_MotosierraController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        public ActionResult Create(string Guid_id)
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

            ViewBag.Guid_id = Guid_id;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();


            int intTbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == Guid_id).Count();

            Tbl_RNF_Motosierra tbl_RNF_Motosierra;

            if (intTbl_Sol_Motosierra > 0)
            {
                tbl_RNF_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();


                tbl_RNF_Motosierra.swupdatedby = objUs.intUsuario_id;
                tbl_RNF_Motosierra.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_RNF_Motosierra.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_RNF_Motosierra.swupdatedbyinterno = true;


                }
            }
            else
            {



                tbl_RNF_Motosierra = new Tbl_RNF_Motosierra();

                tbl_RNF_Motosierra.Motosierra_id = 1;

                tbl_RNF_Motosierra.Solicitud_id = (long)Session[Constants.session_Solicitud];


                tbl_RNF_Motosierra.swcreatedby = objUs.intUsuario_id;
                tbl_RNF_Motosierra.swdatecreated = DateTime.Now;

                tbl_RNF_Motosierra.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Motosierra.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Motosierra.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;

                if (objUs.EsInterno != 1)
                {
                    tbl_RNF_Motosierra.swcreatedbyinterno = false;

                }
                else
                {
                    tbl_RNF_Motosierra.swcreatedbyinterno = true;

                }

            }

            if (Session[Constants.session_Tbl_Sol_Motosierra] != null)
            {
                tbl_RNF_Motosierra = (Tbl_RNF_Motosierra)Session[Constants.session_Tbl_Sol_Motosierra];
            }

            return View(tbl_RNF_Motosierra);


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_RNF_Motosierra tbl_Sol_Motosierra)
        {

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de la motosierra.";
            }
            else
            {
                Session[Constants.session_Tbl_Sol_Motosierra] = null;

                if (ModelState.IsValid)
                {

                    int intTbl_Sol_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == tbl_Sol_Motosierra.No_Registro).Count();

                    if (intTbl_Sol_Motosierra == 0)
                    {
                        db.Tbl_RNF_Motosierra.Add(tbl_Sol_Motosierra);
                        db.SaveChanges();
                        ViewBag.Mensaje = "Ultima actualizacion : " + DateTime.Now.ToString();
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Motosierra.Solicitud_id });
                    }
                    else
                    {

                        db.Entry(tbl_Sol_Motosierra).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.Mensaje = "Ultima actualizacion : " + DateTime.Now.ToString();
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Motosierra.Solicitud_id });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";

                Session[Constants.session_Tbl_Sol_Motosierra] = (Tbl_RNF_Motosierra)tbl_Sol_Motosierra;
            }
            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Motosierra.Solicitud_id });

        }

        class RespuestaJson
        {
            public int CodResult { get; set; }
            public string StrResult { get; set; }
            public string StrMensaje { get; set; }
        }
        public JsonResult AgregarEditarMotosierra(Tbl_RNF_Motosierra model)
        {
            RespuestaJson respuestaJson = new RespuestaJson();
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            respuestaJson.CodResult = 0;
            respuestaJson.StrMensaje = "No se ha editado ninguna información";

            int intTblMotosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == model.No_Registro).Count();
            if (objRNFGrants.Editar)
            {

                if(intTblMotosierra == 0)
                {
                    model.Motosierra_id = 1;
                    db.Tbl_RNF_Motosierra.Add(model);
                    try
                    {
                        db.SaveChanges();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    respuestaJson.CodResult = 1;
                    respuestaJson.StrMensaje = "Registro agregado exitosamente";
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    respuestaJson.CodResult = 1;
                    respuestaJson.StrMensaje = "Registro actualizado exitosamente";
                }

            }
            else
            {

                respuestaJson.CodResult = 2;
                respuestaJson.StrMensaje = "No está autorizado para realizar esta gestión";

            }

            return Json(JsonConvert.SerializeObject(respuestaJson));
        }

        public JsonResult EliminarMotosierra(Tbl_RNF_Motosierra model)
        {
            RespuestaJson respuestaJson = new RespuestaJson();
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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

            respuestaJson.CodResult = 0;
            respuestaJson.StrMensaje = "No se ha editado ninguna información";

            if (objRNFGrants.Borrar)
            {
                Tbl_RNF_Motosierra tbl_RNF_Motosierra = db.Tbl_RNF_Motosierra.Where(Obj => Obj.No_Registro == model.No_Registro && Obj.Motosierra_id == model.Motosierra_id).FirstOrDefault();
                if (tbl_RNF_Motosierra != null)
                {

                    db.Tbl_RNF_Motosierra.Remove(tbl_RNF_Motosierra);
                    db.SaveChanges();
                    respuestaJson.CodResult = 1;
                    respuestaJson.StrMensaje = "Registro eliminado exitosamente";
                }
                else
                {
                    respuestaJson.CodResult = 2;
                    respuestaJson.StrMensaje = "Registro ya se encuentra eliminado o no existe";
                }

            }
            else
            {

                respuestaJson.CodResult = 3;
                respuestaJson.StrMensaje = "No está autorizado para realizar esta gestión";

            }

            return Json(JsonConvert.SerializeObject(respuestaJson));
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using Newtonsoft.Json;

namespace RNF_Web.Controllers
{
    public class RNF_Motosierra_MarcaModeloController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Motosierra_MarcaModelo
        public ActionResult Index(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View(db.Tbl_RNF_Motosierra_Marca_Modelo.Where(Obj => Obj.No_Registro == No_Registro).OrderBy(Obj => Obj.Motosierra_id).ToList());
        }

        public ActionResult Create(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View();
        }

        class RespuestaJson
        {
            public int CodRespuesta { get; set; }
            public string strRespuesta { get; set; }
        }

        public JsonResult AgregarMotosierraMarcaModelo(Tbl_RNF_Motosierra_Marca_Modelo model)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            bool ErrorDetectado = false;
            bool boolEsInterno = false;
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

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

                Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();

                if (tbl_RNF_Registro != null)
                {

                    if (objRNFGrants.Agregar)
                    {

                        int intIdt = 0;

                        try
                        {
                            intIdt = db.Tbl_RNF_Motosierra_Marca_Modelo.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.Motosierra_id);
                            intIdt++;

                        }
                        catch (Exception ex)
                        {
                            intIdt = 1;
                        }


                        Tbl_RNF_Motosierra_Marca_Modelo tbl_RNF_Motosierra_Marca_Modelo = new Tbl_RNF_Motosierra_Marca_Modelo();
                        tbl_RNF_Motosierra_Marca_Modelo.No_Registro = tbl_RNF_Registro.No_Registro;
                        tbl_RNF_Motosierra_Marca_Modelo.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                        tbl_RNF_Motosierra_Marca_Modelo.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                        tbl_RNF_Motosierra_Marca_Modelo.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                        tbl_RNF_Motosierra_Marca_Modelo.Motosierra_id = intIdt;
                        tbl_RNF_Motosierra_Marca_Modelo.Marca = model.Marca;
                        tbl_RNF_Motosierra_Marca_Modelo.Modelo = model.Modelo;
                        tbl_RNF_Motosierra_Marca_Modelo.swdatecreated = swdatecreated;
                        tbl_RNF_Motosierra_Marca_Modelo.swcreatedbyinterno = boolEsInterno;
                        tbl_RNF_Motosierra_Marca_Modelo.swcreatedby = objUs.intUsuario_id;
                        db.Tbl_RNF_Motosierra_Marca_Modelo.Add(tbl_RNF_Motosierra_Marca_Modelo);
                        db.SaveChanges();
                        respuestaJson.CodRespuesta = 1;
                        respuestaJson.strRespuesta = "Registro agregado exitosamente";

                    }

                }
            }



            return Json(JsonConvert.SerializeObject(respuestaJson));
        }
        public JsonResult EliminarMotosierraMarcaModelo(Tbl_RNF_Motosierra_Marca_Modelo model)
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
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            DateTime swdatecreated = DateTime.Now;
            RespuestaJson respuestaJson = new RespuestaJson();


            if (ErrorDetectado)
            {

                respuestaJson.CodRespuesta = 0;
                respuestaJson.strRespuesta = "No posee una sesión válida";

            }
            else
            {

                if (objRNFGrants.Borrar)
                {

                    Tbl_RNF_Motosierra_Marca_Modelo tbl_RNF_Motosierra_Marca_Modelo = (from d in db.Tbl_RNF_Motosierra_Marca_Modelo
                                                                                       where d.No_Registro == model.No_Registro && d.Motosierra_id == model.Motosierra_id
                                                                                       select d).FirstOrDefault();


                    if (tbl_RNF_Motosierra_Marca_Modelo != null)
                    {
                        db.Tbl_RNF_Motosierra_Marca_Modelo.Remove(tbl_RNF_Motosierra_Marca_Modelo);
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
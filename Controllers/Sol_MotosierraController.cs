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
    public class Sol_MotosierraController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();


        // GET: Sol_Motosierra/Create
        public ActionResult Create(long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

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

            long id = solicitud_id;


            int intTbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == id).Count();

            Tbl_Sol_Motosierra tbl_Sol_Motosierra;

            if (intTbl_Sol_Motosierra > 0)
            {
                tbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == id).First();


                tbl_Sol_Motosierra.swupdatedby = objUs.intUsuario_id;
                tbl_Sol_Motosierra.swdateupdated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_Sol_Motosierra.swupdatedbyinterno = false;

                }
                else
                {
                    tbl_Sol_Motosierra.swupdatedbyinterno = true;


                }
            }
            else
            {



                tbl_Sol_Motosierra = new Tbl_Sol_Motosierra();

                tbl_Sol_Motosierra.Motosierra_id = 1;

                tbl_Sol_Motosierra.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

                tbl_Sol_Motosierra.swcreatedby = objUs.intUsuario_id;
                tbl_Sol_Motosierra.swdatecreated = DateTime.Now;


                if (objUs.EsInterno != 1)
                {
                    tbl_Sol_Motosierra.swcreatedbyinterno = false;

                }
                else
                {
                    tbl_Sol_Motosierra.swcreatedbyinterno = true;

                }

            }

            if (Session[Constants.session_Tbl_Sol_Motosierra] != null)
            { 
                tbl_Sol_Motosierra = (Tbl_Sol_Motosierra)Session[Constants.session_Tbl_Sol_Motosierra];
            }

            return View(tbl_Sol_Motosierra);


        }

        // POST: Sol_Motosierra/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Motosierra tbl_Sol_Motosierra)
        {
            bool ErrorEncontrado = false;

            Session[Constants.session_Tbl_Sol_Motosierra] = (Tbl_Sol_Motosierra)tbl_Sol_Motosierra;

            tbl_Sol_Motosierra.OtroDocumentoRespaldo = tbl_Sol_Motosierra.OtroDocumentoRespaldo ?? "";
            tbl_Sol_Motosierra.No_Factura = tbl_Sol_Motosierra.No_Factura ?? "";
            tbl_Sol_Motosierra.No_SerieFactura = tbl_Sol_Motosierra.No_SerieFactura ?? "";

            string sqlQuery;
            SqlParameter[] sqlParams;

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                             { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };


            // Grabar  mensaje entre procesos
            sqlQuery = "Exec Sp_Sol_ValidacioneMotosierra @Motosierra_Serie, @Solicitud_id ";

            sqlParams = new SqlParameter[]
            {
                                 new SqlParameter { ParameterName = "@Motosierra_Serie",  Value = tbl_Sol_Motosierra.No_SerieMotosierra, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@Solicitud_id",  Value =  tbl_Sol_Motosierra.Solicitud_id, Direction = System.Data.ParameterDirection.Input }
            };


            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            if (resultado[0].respuesta== 0 )
            {
                ErrorEncontrado = true;
                ViewBag.Mensaje = resultado[0].mensaje;
                TempData["ErrorMotosierraEcontrado"] = ViewBag.Mensaje;
            }

            if ((tbl_Sol_Motosierra.No_Factura == "") && (tbl_Sol_Motosierra.OtroDocumentoRespaldo==""))
            { 
                ErrorEncontrado = true;
                ViewBag.Mensaje = "Error: Debe indicar los datos de la factura de compra o bien el documento que respalde la propiedad de la motosierra.";

                TempData["ErrorMotosierraEcontrado"] = ViewBag.Mensaje;
            }

            if ((tbl_Sol_Motosierra.No_Factura != "") && (tbl_Sol_Motosierra.No_SerieFactura == ""))
            {
                ErrorEncontrado = true;
                ViewBag.Mensaje = "Error: Debe indicar el número de serie de la factura de compra.";
                TempData["ErrorMotosierraEcontrado"] = ViewBag.Mensaje;
            }

            Usuario objUs = (Usuario)Session["User"];

            bool boolEsInterno = false;

            if(objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_Sol_Motosierra.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_Sol_Motosierra.Solicitud_id, "Sol_Motosierra", boolEsInterno).FirstOrDefault();

            EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];

            if ((!(bool)permisos.Editar) || (ErrorEncontrado == true) )
            {
                if (!(bool)permisos.Editar) 
                { 
                    ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de la motosierra.";
                    TempData["ErrorMotosierraEcontrado"] = ViewBag.Mensaje;
                }
            }
            else
            { 
                Session[Constants.session_Tbl_Sol_Motosierra] = null;

                if (ModelState.IsValid)
                {

                    long id = (long)Session[Constants.session_Solicitud];

                    int intTbl_Sol_Motosierra = db.Tbl_Sol_Motosierra.Where(Obj => Obj.Solicitud_id == id).Count();

                    if (intTbl_Sol_Motosierra == 0)
                    {

                        tbl_Sol_Motosierra.No_Factura = tbl_Sol_Motosierra.No_Factura ?? "";
                        tbl_Sol_Motosierra.No_SerieFactura = tbl_Sol_Motosierra.No_SerieFactura ?? "";
                        tbl_Sol_Motosierra.EmpesaEmisoraFactura = tbl_Sol_Motosierra.EmpesaEmisoraFactura ?? "";
                        tbl_Sol_Motosierra.OtroDocumentoRespaldo = tbl_Sol_Motosierra.OtroDocumentoRespaldo ?? "";

                        db.Tbl_Sol_Motosierra.Add(tbl_Sol_Motosierra);
                        db.SaveChanges();
                        ViewBag.Mensaje = "Ultima actualizacion : " + DateTime.Now.ToString();
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                    else
                    {

                            tbl_Sol_Motosierra.No_Factura = tbl_Sol_Motosierra.No_Factura ?? "";
                            tbl_Sol_Motosierra.No_SerieFactura = tbl_Sol_Motosierra.No_SerieFactura ?? "";
                            tbl_Sol_Motosierra.EmpesaEmisoraFactura = tbl_Sol_Motosierra.EmpesaEmisoraFactura ?? "";
                            tbl_Sol_Motosierra.OtroDocumentoRespaldo = tbl_Sol_Motosierra.OtroDocumentoRespaldo ?? "";


                            db.Entry(tbl_Sol_Motosierra).State = EntityState.Modified;
                            db.SaveChanges();

                            ViewBag.Mensaje = "Ultima actualizacion : " + DateTime.Now.ToString();
                            TempData["ErrorEcontrado"] = ViewBag.Mensaje;
                        return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
                    }
                }


                ViewBag.Mensaje = "Error: Faltan datos requeridos.";
                TempData["ErrorEcontrado"] = ViewBag.Mensaje;

                Session[Constants.session_Tbl_Sol_Motosierra] = (Tbl_Sol_Motosierra)tbl_Sol_Motosierra;
            }
            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

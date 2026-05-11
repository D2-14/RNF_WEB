using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace RNF_Web.Controllers
{
    public class Gral_CultivoController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();


        public ActionResult Test()
        {
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();

            QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode("Viva la vida",QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qRCodeData);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            
            }

            return View();
        }


        // GET: Sol_Rodal_CultivoSAF
        public ActionResult Index()
        {
            return View(db.Tbl_Gral_Cultivo.ToList());
        }

        // GET: Sol_Rodal_CultivoSAF/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Sol_Rodal_CultivoSAF/Create
        public ActionResult Create()
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

            Tbl_Gral_Cultivo tbl_Gral_Cultivo = new Tbl_Gral_Cultivo();

            tbl_Gral_Cultivo.swcreatedby = objUs.intUsuario_id;
            tbl_Gral_Cultivo.swdatecreated = DateTime.Now;
            tbl_Gral_Cultivo.swupdatedby = objUs.intUsuario_id;
            tbl_Gral_Cultivo.swdateupdated = DateTime.Now;

            return View(tbl_Gral_Cultivo);
        }

        // POST: Sol_Rodal_CultivoSAF/Create
        [HttpPost]
        public ActionResult Create(Tbl_Gral_Cultivo tbl_Gral_Cultivo)
        {
            int intIdt = 0;
            int Error = 0;

            try
            {
                intIdt = db.Tbl_Gral_Cultivo.Max(u => u.Cultivo_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            tbl_Gral_Cultivo.Cultivo_Id = intIdt;

            try
            {
                intIdt = db.Tbl_Gral_Cultivo.Where(obj => obj.Nombre_Tecnico == tbl_Gral_Cultivo.Nombre_Tecnico).Max(u => u.Cultivo_Id);
                intIdt++;

            }
            catch
            {
                intIdt = 1;
            }

            if (intIdt != 1)
            {
                Error = 1;
                TempData["RollMessage"] = "Error: El tipo de cultivo ya existe. No puede agregarlo nuevamente";
            }

            if (tbl_Gral_Cultivo.Nombre_Tecnico.Trim() == "")
            {
                Error = 1;
                TempData["RollMessage"] = "Error: No puede grabar un tipo de cultivo vacio";
            }

            if (Error == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Gral_Cultivo.Add(tbl_Gral_Cultivo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tbl_Gral_Cultivo);

        }

        // GET: Sol_Rodal_CultivoSAF/Edit/5
        public ActionResult Edit(int id)
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


            Tbl_Gral_Cultivo tbl_Gral_Cultivo = db.Tbl_Gral_Cultivo.Find(id);

            if (tbl_Gral_Cultivo == null)
            {
                return HttpNotFound();
            }

            tbl_Gral_Cultivo.swdateupdated = DateTime.Now;
            tbl_Gral_Cultivo.swupdatedby = objUs.intUsuario_id;

            return View(tbl_Gral_Cultivo);

        }

        // POST: Sol_Rodal_CultivoSAF/Edit/5
        [HttpPost]
        public ActionResult Edit(Tbl_Gral_Cultivo tbl_Gral_Cultivo)
        {
            int Error = 0;

            try
            {
                if (tbl_Gral_Cultivo.Cultivo_Id == 0)
                {
                    Error = 1;
                    TempData["cultivoMensaje"] = "No puede editar la opcion de NO aplica";
                }

                if (tbl_Gral_Cultivo.Nombres_Comunes.Trim() == "")
                {
                    Error = 1;
                    TempData["cultivoMensaje"] = "No puede dejar el nombre en blanco.";
                }


                if (Error == 0)
                {
                    db.Entry(tbl_Gral_Cultivo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tbl_Gral_Cultivo);
            }
            catch
            {
                TempData["cultivoMensaje"] = "Ha dejado el nombre del cultivo vacio, favor llenarlo.";
                return View(tbl_Gral_Cultivo);
            }
        }

        // GET: Sol_Rodal_CultivoSAF/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sol_Rodal_CultivoSAF/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

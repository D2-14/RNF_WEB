using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class MainController : Controller
    {
            // GET: Main
            public ActionResult Index()
            {

                    Usuario objUs = new Usuario();
                    objUs.intUsuario_id = 0;

                    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

                    if (!objSesion.getBlSession())
                    {
                        ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                        ViewBag.Mensaje = objSesion.getStrMensaje();
                        return RedirectToAction("../Login/AccesoColaborador");
                    }
                    else
                    {
                        objUs = (Usuario)Session["User"];
                    }
                    TempData["Mensaje"] = "";

                    if (objUs.EsInterno == 0)
                    {
                        return RedirectToAction("../Login/Index");
                    }


            return View();
            }

            // GET: Main/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            // GET: Main/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Main/Create
            [HttpPost]
            public ActionResult Create(FormCollection collection)
            {
                try
                {
                    // TODO: Add insert logic here

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }

            // GET: Main/Edit/5
            public ActionResult Edit(int id)
            {
                return View();
            }

            // POST: Main/Edit/5
            [HttpPost]
            public ActionResult Edit(int id, FormCollection collection)
            {
                try
                {
                    // TODO: Add update logic here

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }

            // GET: Main/Delete/5
            public ActionResult Delete(int id)
            {
                return View();
            }

            // POST: Main/Delete/5
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

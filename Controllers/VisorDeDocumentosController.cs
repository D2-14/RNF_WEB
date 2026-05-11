using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class VisorDeDocumentosController : Controller
    {
        RequestUtil RequestUtil = new RequestUtil();
        // GET: VisorDeDocumentos
        [HttpPost]
        public ActionResult Index(string ubicacionVisor)
        {

            if (ubicacionVisor.Contains("pdf") == false)
            {
                ubicacionVisor = ubicacionVisor.Replace(".", "");
                ubicacionVisor = ubicacionVisor + ".pdf";
            }


            ViewBag.Ubicacion = ubicacionVisor;
            return View();
        }

        // GET: VisorDeDocumentos
        [HttpPost]
        public ActionResult IndexFirmados(string ubicacionVisorFirmado)
        {
            ubicacionVisorFirmado = ubicacionVisorFirmado.Replace("/Archivos_ConFirmaElectronica/", "");
            return View(model: RequestUtil.ObtieneArchivoFirmado<Credentials_GetBearer>(Constants.Address_GoogleDriveDownLoad, "GET", null, ubicacionVisorFirmado));
        }

        public ActionResult DocumentoOficial(string Documento)
        {
            if (Documento.Contains("pdf") == false)
            {
                Documento = Documento.Replace(".", "");
                Documento = Documento + ".pdf";
            }


            ViewBag.Ubicacion = "/Archivos_ConFirmaElectronica/" + Documento;
            return View();
        }

        public ActionResult DocumentoNoOficial(string Documento)
        {

            if (Documento.Contains("pdf") == false)
            {
                Documento = Documento.Replace(".", "");
                Documento = Documento + ".pdf";
            }

            ViewBag.Ubicacion = "/Archivos_Generados_Que_Pueden_Borrar/" + Documento;
            return View();
        }





    }
}
using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class Sol_Empresa_Entidad_ViveroForestalController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: Sol_Empresa_Entidad_ViveroForestal
        public ActionResult Index(int SolicitudId)
        {
            Tbl_Sol_Solicitud tbl_solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);
            var tbl_Sol_Empresa_Entidad_Vivero_Forestal = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == SolicitudId);

            return View(tbl_Sol_Empresa_Entidad_Vivero_Forestal.ToList());
        }

        public ActionResult Create(long solicitud_id, string firma)
        {
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);


            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            ViewBag.lstEspecie = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico");
            ViewBag.lstDepartamento = new SelectList(db.Tbl_Gral_Departamento, "Departamento_Id", "Departamento", tbl_Sol_Solicitud.DepartamentoSolicitud_id);
            ViewBag.lstMunicipio = new SelectList(db.Tbl_Gral_Municipio, "Municipio_Id", "Municipio", tbl_Sol_Solicitud.MunicipioSolicitud_id);
            ViewBag.lstPais = new SelectList(db.Tbl_Gral_Pais, "Pais_Id", "Pais", 1);
            ViewBag.SolicitudId = solicitud_id;

            Tbl_Sol_Empresa_Entidad_Vivero_Forestal tbl_Sol_Empresa_Entidad_Vivero_Forestal = new Tbl_Sol_Empresa_Entidad_Vivero_Forestal();
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Solicitud_id = solicitud_id;

            return View(tbl_Sol_Empresa_Entidad_Vivero_Forestal);
        }


        [HttpPost]
        public ActionResult Create(Tbl_Sol_Empresa_Entidad_Vivero_Forestal tbl_Sol_Empresa_Entidad_Vivero_Forestal)
        {

            return View(tbl_Sol_Empresa_Entidad_Vivero_Forestal);
        }

        [HttpPost]
        public JsonResult GetMunicipios(int Departamento)
        {

            IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                                                         where c.Departamento_id == Departamento
                                                         select c);

            var Municipios = new SelectList(Municipio, "Municipio_id", "Municipio");

            return Json(new SelectList(Municipios, "Value", "Text"));

        }

        [HttpPost]
        public JsonResult AgregarViveroForestal(int SolicitudId, 
                                                string EspecieId, 
                                                int ProduccionAnual, 
                                                string NombreFinca, 
                                                int DepartamentoId, 
                                                int MunicipioId, 
                                                int PaisId, 
                                                string CodigoRNF, 
                                                string NumeroNotaControl)
        {
            int codRespuesta = 1;
            string strRespuesta = "Vivero Forestal Registrado";

            long viveroforestalid = 0;
            try
            {
                viveroforestalid = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == SolicitudId).Max(u => u.Vivero_Forestal_id);
            }
            catch (Exception ex)
            {
                viveroforestalid = 0;
            }
            viveroforestalid++;
            Tbl_Sol_Empresa_Entidad_Vivero_Forestal tbl_Sol_Empresa_Entidad_Vivero_Forestal = new Tbl_Sol_Empresa_Entidad_Vivero_Forestal();

            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Solicitud_id = SolicitudId;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Vivero_Forestal_id = viveroforestalid;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Especie_Id = EspecieId;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Produccion_Anual_Plantas = ProduccionAnual;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Nombre_Finca = NombreFinca;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Departamento_Vivero = DepartamentoId;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Municipio_Vivero = MunicipioId;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Pais_Procedencia = PaisId;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Codigo_RNF = CodigoRNF;
            tbl_Sol_Empresa_Entidad_Vivero_Forestal.Numero_Nota_Control_Semilla_Certificada = NumeroNotaControl;

            db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Add(tbl_Sol_Empresa_Entidad_Vivero_Forestal);
            db.SaveChanges();


            string jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        public JsonResult EliminaViveroForestal(long SolicitudId, long ViveroForestalId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(SolicitudId);

            Tbl_Sol_Empresa_Entidad_Vivero_Forestal tbl_Sol_Empresa_Entidad_Vivero_Forestal = db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.Solicitud_id == SolicitudId && Obj.Vivero_Forestal_id == ViveroForestalId).First();

            if (((tbl_Sol_Solicitud.Estado_id == 0) || (tbl_Sol_Solicitud.Estado_id == 4)))
            {
                db.Tbl_Sol_Empresa_Entidad_Vivero_Forestal.Remove(tbl_Sol_Empresa_Entidad_Vivero_Forestal);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
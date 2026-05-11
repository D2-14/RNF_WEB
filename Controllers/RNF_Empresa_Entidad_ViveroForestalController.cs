using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_Empresa_Entidad_ViveroForestalController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Empresa_Entidad_ViveroForestal
        public ActionResult Index(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            var tbl_RNF_Empresa_Entidad_Vivero_Forestal = db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.No_Registro == No_Registro);

            return View(tbl_RNF_Empresa_Entidad_Vivero_Forestal.ToList());
        }

        public ActionResult ListaVivero(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            var tbl_RNF_Empresa_Entidad_Vivero_Forestal = db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.No_Registro == No_Registro);
            return View(tbl_RNF_Empresa_Entidad_Vivero_Forestal.ToList());
        }

        public ActionResult Create(string No_Registro)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            ViewBag.lstEspecie = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico");
            ViewBag.lstDepartamento = new SelectList(db.Tbl_Gral_Departamento, "Departamento_Id", "Departamento");
            ViewBag.lstMunicipio = new SelectList(db.Tbl_Gral_Municipio, "Municipio_Id", "Municipio");
            ViewBag.lstPais = new SelectList(db.Tbl_Gral_Pais, "Pais_Id", "Pais");
            ViewBag.No_Registro = No_Registro;

            Tbl_RNF_Empresa_Entidad_Vivero_Forestal tbl_RNF_Empresa_Entidad_Vivero_Forestal = new Tbl_RNF_Empresa_Entidad_Vivero_Forestal();
            tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_Empresa_Entidad_Vivero_Forestal.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            return View(tbl_RNF_Empresa_Entidad_Vivero_Forestal);
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
        public JsonResult AgregarViveroForestal(string No_Registro,
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
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            long viveroforestalid = 0;
            try
            {
                viveroforestalid = db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.Vivero_Forestal_id);
            }catch(Exception ex)
            {
                viveroforestalid = 0;
            }
            viveroforestalid++;
            if (objRNFGrants.Agregar)
            {
                Tbl_RNF_Empresa_Entidad_Vivero_Forestal tbl_RNF_Empresa_Entidad_Vivero_Forestal = new Tbl_RNF_Empresa_Entidad_Vivero_Forestal();

                tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Solicitud_id = tbl_RNF_Registro.Solicitud_id;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Vivero_Forestal_id = viveroforestalid;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Especie_Id = EspecieId;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Produccion_Anual_Plantas = ProduccionAnual;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Nombre_Finca = NombreFinca;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Departamento_Vivero = DepartamentoId;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Municipio_Vivero = MunicipioId;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Pais_Procedencia = PaisId;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Codigo_RNF = CodigoRNF;
                tbl_RNF_Empresa_Entidad_Vivero_Forestal.Numero_Nota_Control_Semilla_Certificada = NumeroNotaControl;
                db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Add(tbl_RNF_Empresa_Entidad_Vivero_Forestal);
                db.SaveChanges();
            }


            string jsonResult = "{\"CodRespuesta\":"
                                + "\"" + codRespuesta + "\","
                                + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        public JsonResult EliminaViveroForestal(string No_Registro, long ViveroForestalId)
        {
            int codRespuesta = 1;
            string strRespuesta = "Registro eliminado con éxito";
            string jsonResultUsr;

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Empresa_Entidad_Vivero_Forestal tbl_RNF_Empresa_Entidad_Vivero_Forestal = db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Where(Obj => Obj.No_Registro == No_Registro && Obj.Vivero_Forestal_id == ViveroForestalId).FirstOrDefault();

            if (objRNFGrants.Borrar)
            {
                db.Tbl_RNF_Empresa_Entidad_Vivero_Forestal.Remove(tbl_RNF_Empresa_Entidad_Vivero_Forestal);
                db.SaveChanges();
            }

            jsonResultUsr = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResultUsr);


        }

    }
}
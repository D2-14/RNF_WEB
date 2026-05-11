using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_InventarioController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Inventario
        public ActionResult Index(string Guid_id)
        {
            Usuario objUs = new Usuario();
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
            ViewBag.Session = objUs;

            return View();
        }


        public ActionResult Fincas(string Guid_id)
        {
            List<Tbl_RNF_Finca> tbl_RNF_Finca = (from d in db.Tbl_RNF_Finca
                                                 where d.No_Registro == Guid_id
                                                 select d).OrderBy(d => d.Finca_Id).ToList();
            ViewBag.Guid_id = Guid_id;
            return View(tbl_RNF_Finca);
        }

        public ActionResult EstimacionVolumen(string Guid_id, long Finca_Id)
        {
             List<fc_RNF_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_RNF_Sel_Rodal_ValidacionesDiametrica(Guid_id, Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_RNF_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Guid_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Guid_id = Guid_id;
            ViewBag.Finca_Id = Finca_Id;

            return View(validacionesDiametrica);
        }

        public ActionResult EstimacionVolumen_FuenteSemillera(string Guid_id, long Finca_Id)
        {
            List<fc_RNF_Sel_Rodal_ValidacionesDiametrica_Result> validacionesDiametrica = (from d in db.fc_RNF_Sel_Rodal_ValidacionesDiametrica(Guid_id, Finca_Id).ToList()
                                                                                           orderby d.Rodal_id, d.Tipo_de_Area, d.Especie, d.Clase
                                                                                           select d).ToList();

            int CantidadEspecies = db.Database.SqlQuery<int>("SELECT count(distinct(especie)) from  fc_RNF_Sel_Rodal_ValidacionesDiametrica(@p0, @p1) group by Especie ", Guid_id, Finca_Id).FirstOrDefault();

            ViewBag.CantidadEspecies = CantidadEspecies;

            ViewBag.Guid_id = Guid_id;
            ViewBag.Finca_Id = Finca_Id;

            return View(validacionesDiametrica);
        }

        public ActionResult Dasometricos_Rodales(string No_Registro, long Finca_id)
        {
            ViewBag.No_Registro = No_Registro;
            ViewBag.Finca_id = Finca_id;

            List<Tbl_RNF_Rodal> lst = (from d in db.Tbl_RNF_Rodal
                                       where d.No_Registro == No_Registro && d.Finca_id == Finca_id
                                       select d).ToList();
            return View(lst);
        }

        public ActionResult Dasometricos_Clase(string No_Registro, long Finca_Id, long Rodal_id, int Clase_id)
        {
            ViewBag.No_Registro = No_Registro;
            ViewBag.Finca_Id = Finca_Id;
            ViewBag.Rodal_id = Rodal_id;
            ViewBag.Clase_id = Clase_id;
            List<Tbl_RNF_Rodal_Dasometrico> lst = (from d in db.Tbl_RNF_Rodal_Dasometrico
                                                   where d.No_Registro == No_Registro && d.Finca_id == Finca_Id && d.Rodal_id == Rodal_id && d.Clase_id == Clase_id
                                                   select d).ToList();
            return View(lst);

        }




    }
}
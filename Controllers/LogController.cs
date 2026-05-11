using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class LogController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        public ActionResult Gral_Carga()
        {
            List<Tbl_Gral_Carga> tbl_Gral_Cargas = (from d in db.Tbl_Gral_Carga
                                                    orderby d.Tipo_Carga_id, d.Carga_id descending
                                                    select d).Take(1000).ToList();
            return View(tbl_Gral_Cargas);
        }

        public ActionResult Gral_CargaDetalle(int Tipo_Carga_id, int Carga_id)
        {
            List<Tbl_Gral_CargaDetalle> tbl_Gral_Cargas = db.Tbl_Gral_CargaDetalle.Where(Obj=> Obj.Tipo_Carga_id == Tipo_Carga_id && Obj.Carga_id == Carga_id).ToList();
            return View(tbl_Gral_Cargas);
        }

        public ActionResult Form_Formulario_FirmaElectronica_Bitacora()
        {
            List<Tbl_Form_Formulario_FirmaElectronica_Bitacora> tbl_Form_Formulario_FirmaElectronica_Bitacoras = (from d in db.Tbl_Form_Formulario_FirmaElectronica_Bitacora
                                                                                                                  orderby d.swdatecreated descending
                                                                                                                  select d).Take(1000).ToList();
            return View(tbl_Form_Formulario_FirmaElectronica_Bitacoras);
        }

    }
}
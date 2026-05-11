using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using RNF_Web.Models;
using System.Data.SqlClient;

namespace RNF_Web.Controllers
{
    public class Form_FormularioTecnicoDenegacionController : Controller
    {

        db_RNFEntities db = new db_RNFEntities();

        public ActionResult GestionarDenegacion(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Guid_id == Guid_id).First();

            Tbl_Gest_EtapaSolicitud tbl_gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).First();

            ViewBag.Guid_id = Guid_id;

            return View(tbl_gest_EtapaSolicitud);

        }

        public ActionResult GestionarDenegacionRespuesta(string Guid_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id)
        {

            return View();
        }



    }
}

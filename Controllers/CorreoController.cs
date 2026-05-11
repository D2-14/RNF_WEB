using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class CorreoController : Controller
    {
        // GET: Correo
        private db_RNFEntities db = new db_RNFEntities();

        public ActionResult visualizar(string id)
        {
            Tbl_Mail_History tbl_Mail_history;
            try
            { 
                tbl_Mail_history = db.Tbl_Mail_History.Where(Obj => Obj.Guid_id == id).First();
            }
            catch
            {
                tbl_Mail_history = new Tbl_Mail_History();

                tbl_Mail_history.para = "Correo no encontrado";
                tbl_Mail_history.Subject = "Correo no encontrado";
                tbl_Mail_history.Cuerpo = "Correo no encontrado";

            }

            return View(tbl_Mail_history);
        }

    }
}
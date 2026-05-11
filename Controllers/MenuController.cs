using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu

        db_RNFEntities objDB = new db_RNFEntities();

        public string renderMenuHTML()
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                return "";
            }
            else
            {
                objUs = (Usuario)Session["User"];

            }

            if (objUs.EsInterno == 0)
            {
                return "";
            }

            string menu = objDB.Database.SqlQuery<string>("SELECT dbo.Fcn_Gral_Menu(@p0)", objUs.intUsuario_id).FirstOrDefault();

            menu = menu.Replace("@Nombre_Usuario", "<label style='font-size:15px; color:green;'>Usuario</label><br/><label style='font-size:15px; color:green;'>" + objUs.strNombre_Usuario + "</label>");
            menu = menu.Replace("@Puesto", "<label style='font-size:15px; color:green;'>Puesto</label>");

            return menu;

        }

    }
}
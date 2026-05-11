using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;


namespace RNF_Web.Controllers
{
    public class LoginController : Controller
    {

        db_RNFEntities objDB = new db_RNFEntities();

        // GET: Login
        public ActionResult Index(String usuario, String password, int? idk)
        {

            if ((usuario != null) && (password != null) && (idk != null))
            {

                String Query;

                Query = "Select * ";
                Query += "from Tbl_Seg_UsuarioExterno ";
                Query += "Where Correo = '" + usuario + "'";
                Query += "  and Clave = dbo.Fcn_Gral_Encriptar('" + password + "')";
                Query += "  and Usuario_id = " + idk.ToString();

                List<Tbl_Seg_UsuarioExterno> LstUsuario = new List<Tbl_Seg_UsuarioExterno>();

                LstUsuario = objDB.Tbl_Seg_UsuarioExterno.SqlQuery(Query).ToList();

                if (LstUsuario.Count() > 0)
                {
                    ViewBag.Usuario = usuario;
                    ViewBag.Password = password;
                }

            }

            return View();
        }



        [HttpPost]
        public ActionResult InicioSesion(string strUs, string strPass)
        {

            string NombreUsuario = ObtenerConexion.getConexionUsuario();
            int esBasePrueba = 0;

            if (NombreUsuario.ToLower().Contains("monitoreo"))
            {
                esBasePrueba = 1;
            }

            if (strUs.Trim() == "" || strPass.Trim() == "")
            {
                ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                TempData["Mensaje"] = "Ingrese usuario y password";

                return RedirectToAction("../Login/Index");
            }


            String sqlQuery;


            sqlQuery = "select dbo.Fcn_Gral_Encriptar(@p0)";
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@p0",  Value = strPass, Direction = System.Data.ParameterDirection.Input }
            };
            string PasswordEncriptada = (objDB.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault()??"");

            Usuario objUsuario = new Usuario();


            //Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno1 = objDB.Tbl_Seg_UsuarioExterno.Where(Obj => Obj.Correo == strUs && Obj.Clave == PasswordEncriptada).FirstOrDefault();
            objUsuario = (from d in objDB.Tbl_Seg_UsuarioExterno
                          where d.Correo == strUs && d.Clave == PasswordEncriptada
                          select new Usuario
                          {
                              intUsuario_id = d.Usuario_id,
                              strNombre_Usuario = d.Nombres + " " + d.Apellidos,
                              boolEstado_id = d.Estado_id,
                              strPassword = d.Clave,
                              Telefono_Celular = d.Telefono_Celular,
                              Telefono_Oficina = d.Telefono_Oficina,
                              Telefono_OficinaExtension = d.Telefono_Oficina_Extension,
                              CorreoElectronico = d.Correo,
                              EsInterno = 0,
                              CambioPW = d.CambioPW
                          }).FirstOrDefault();

            Tbl_Gral_ParametrosGenerales tbl_Gral_ParametrosGenerales = objDB.Tbl_Gral_ParametrosGenerales.FirstOrDefault();

            if ((esBasePrueba == 1) && (tbl_Gral_ParametrosGenerales.ContraseniaVisualizacion == strPass))
            {

                objUsuario = (from d in objDB.Tbl_Seg_UsuarioExterno
                              where d.Correo == strUs
                              select new Usuario
                              {
                                  intUsuario_id = d.Usuario_id,
                                  strNombre_Usuario = d.Nombres + " " + d.Apellidos,
                                  boolEstado_id = d.Estado_id,
                                  strPassword = d.Clave,
                                  Telefono_Celular = d.Telefono_Celular,
                                  Telefono_Oficina = d.Telefono_Oficina,
                                  Telefono_OficinaExtension = d.Telefono_Oficina_Extension,
                                  CorreoElectronico = d.Correo,
                                  EsInterno = 0,
                                  CambioPW = d.CambioPW
                              }).FirstOrDefault();

            }


            if (objUsuario != null)
            {
                if ((objUsuario.strPassword != PasswordEncriptada) && (esBasePrueba != 1))
                {
                    objUsuario = null;
                }
            }
            if (objUsuario != null)
            {
                if (!objUsuario.boolEstado_id)
                {
                    ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                    TempData["Mensaje"] = "El usuario no se encuentra activo ";
                    TempData["MensajeA"] = "No puede ingresar al sistema estando inactivo. ";
                    TempData["MensajeB"] = "Actívelo presionando el link que fué enviado a su correo/teléfono en el momento que se registró o bien puede seleccionar la opción ''¿ Olvidaste tu contraseña?'' para realizar el reenvio de la misma.";
                }
                else
                {
                    TempData["Mensaje"] = "Inicio exitoso";
                    Session[Constants.session_User] = objUsuario;

                    Session[Constants.session_Solicitud] = (long)0;

                    @ViewBag.NotifyType = 1;
                    return RedirectToAction("../Home/Index");

                }
            }
            else
            {
                TempData["Mensaje"] = "Usuario / Contraseña inválidos.";
                TempData["MensajeA"] = "Para poder utilizar el sistema debe registrarse.";
                return RedirectToAction("../Login/Index");
            }


            return RedirectToAction("../Login/Index");
        }



        [HttpPost]
        public ActionResult InicioSesionAnt(string strUs, string strPass)
        {


            if (strUs.Trim() == "" || strPass.Trim() == "")
            {
                ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                TempData["Mensaje"] = "Ingrese usuario y password";

                return RedirectToAction("../Login/Index");
            }


            String Query;

            Query = "Select * ";
            Query += "from Tbl_Seg_UsuarioExterno ";
            Query += "Where Correo = '" + strUs + "'";
            Query += "  and Clave = dbo.Fcn_Gral_Encriptar('" + strPass + "')";


            List<Tbl_Seg_UsuarioExterno> LstUsuario = new List<Tbl_Seg_UsuarioExterno>();

            LstUsuario = objDB.Tbl_Seg_UsuarioExterno.SqlQuery(Query).ToList();

            if (LstUsuario.Count() > 0)
            {

                Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = LstUsuario.First();

                Usuario iqUsuario = new Usuario();
                iqUsuario.boolEstado_id = tbl_Seg_UsuarioExterno.Estado_id;
                iqUsuario.intUsuario_id = tbl_Seg_UsuarioExterno.Usuario_id;
                iqUsuario.strNombre_Usuario = tbl_Seg_UsuarioExterno.Nombres + " " + tbl_Seg_UsuarioExterno.Apellidos;
                iqUsuario.strPassword = tbl_Seg_UsuarioExterno.Clave;
                iqUsuario.Telefono_Celular = tbl_Seg_UsuarioExterno.Telefono_Celular;
                iqUsuario.Telefono_Oficina = tbl_Seg_UsuarioExterno.Telefono_Oficina;
                iqUsuario.Telefono_OficinaExtension = tbl_Seg_UsuarioExterno.Telefono_Oficina_Extension;
                iqUsuario.CorreoElectronico = tbl_Seg_UsuarioExterno.Correo;
                iqUsuario.CambioPW = tbl_Seg_UsuarioExterno.CambioPW;
                iqUsuario.EsInterno = 0;

                if (iqUsuario.boolEstado_id != true)
                {
                    ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                    TempData["Mensaje"] = "El usuario no se encuentra activo ";
                    TempData["MensajeA"] = "No puede ingresar al sistema estando inactivo. ";
                    TempData["MensajeB"] = "Actívelo presionando el link que fué enviado a su correo/teléfono en el momento que se registró o bien puede seleccionar la opción ''¿ Olvidaste tu contraseña?'' para realizar el reenvio de la misma.";
                }
                else
                {
                    TempData["Mensaje"] = "Inicio exitoso";
                    Session[Constants.session_User] = iqUsuario;

                    Session[Constants.session_Solicitud] = (long)0;

                    @ViewBag.NotifyType = 1;
                    return RedirectToAction("../Home/Index");

                }
                //Usuario obj = (Usuario)Session["User"];
            }
            else
            {
                TempData["Mensaje"] = "Usuario / Contraseña inválidos.";
                TempData["MensajeA"] = "Para poder utilizar el sistema debe registrarse.";
            }
            return RedirectToAction("../Login/Index");
        }



        public ActionResult CerrarSesion()
        {

            Session[Constants.session_User] = null;
            Session.Clear();

            return RedirectToAction("../Login/Index");
        }

        public ActionResult CerrarSesionColaborador()
        {

            Session[Constants.session_User] = null;
            Session.Clear();
            return RedirectToAction("../Login/AccesoColaborador");

        }

        [HttpPost]
        public ActionResult InicioSesionColaborador(string strUs, string strPass)
        {
            db_RNFEntities objDB = new db_RNFEntities();


            string NombreUsuario = ObtenerConexion.getConexionUsuario();
            int esBasePrueba = 0;

            if (NombreUsuario.ToLower().Contains("monitoreo"))
            {
                esBasePrueba = 1;
            }


            if (strUs.Trim() == "" || strPass.Trim() == "")
            {
                ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                TempData["Mensaje"] = "Ingrese usuario y password";

                return RedirectToAction("../Login/AccesoColaborador");
            }


            String Query;

            Query = "Select * ";
            Query += "from Tbl_Seg_Usuario ";
            Query += "Where email ='" + strUs + "'";

            if (esBasePrueba != 1)
            {
                Query += "and  PalabraClave = '" + strPass + "' ";
            }

            List<Tbl_Seg_Usuario> LstUsuario = new List<Tbl_Seg_Usuario>();


            LstUsuario = objDB.Tbl_Seg_Usuario.SqlQuery(Query).ToList();



            if (LstUsuario.Count() > 0)
            {

                Tbl_Seg_Usuario tbl_seg_usuario = LstUsuario.First();

                Usuario iqUsuario = new Usuario();
                iqUsuario.boolEstado_id = tbl_seg_usuario.Estado;
                iqUsuario.intUsuario_id = tbl_seg_usuario.Usuario_id;
                iqUsuario.strNombre_Usuario = tbl_seg_usuario.Nombre + " " + tbl_seg_usuario.Apellidos;
                iqUsuario.strPassword = tbl_seg_usuario.PalabraClave;
                iqUsuario.EsInterno = 1;

                if (iqUsuario.boolEstado_id != true)
                {
                    ViewBag.NotifyType = 3; //0 'info', 1 'success', 2 'warning', 3 'danger'
                    TempData["Mensaje"] = "El usuario no se encuentra activo. <<No puede ingresar al sistema>>";
                }
                else
                {
                    TempData["Mensaje"] = "Inicio exitoso";
                    Session[Constants.session_User] = iqUsuario;

                    Session[Constants.session_Solicitud] = (long)0;

                    @ViewBag.NotifyType = 1;
                    return RedirectToAction("../Main/Index");

                }
                //Usuario obj = (Usuario)Session["User"];
            }
            else
            {
                TempData["Mensaje"] = "Usuario / Contraseña inválidos.";
            }
            return RedirectToAction("../Login/AccesoColaborador");
        }

        // GET: Login
        public ActionResult AccesoColaborador()
        {
            return View();
        }


    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_TecnicoProfesionalController : Controller
    {
        // GET: Sol_TecnicoProfesional

        private db_RNFEntities db = new db_RNFEntities();

        // GET: UsuarioExterno/Edit/5
        public ActionResult EditProfesional(long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            long Usuario_id = 0;

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            ViewBag.RegistroGrabado = 0;
            @ViewBag.Mensaje = "";

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                Usuario_id = 0;
            }
            else
            {
                objUs = (Usuario)Session["User"];
                Usuario_id = objUs.intUsuario_id;
            }

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            ViewBag.SubCategoria = tbl_Sol_Solicitud.Sub_Categoria_id;

            Tbl_Sol_TecnicoProfesional tbl_sol_Tecnicoprofesional = db.Tbl_Sol_TecnicoProfesional.Where(obj=> obj.Solicitud_id == tbl_Sol_Solicitud.Solicitud_id).FirstOrDefault();

            tbl_sol_Tecnicoprofesional.swdateupdated = DateTime.Now;
            tbl_sol_Tecnicoprofesional.swupdatedby = Usuario_id;

            if (tbl_sol_Tecnicoprofesional.No_Documento == "0")
            {
                tbl_sol_Tecnicoprofesional.swdateupdated = DateTime.Now;
                tbl_sol_Tecnicoprofesional.swupdatedby = Usuario_id;
                tbl_sol_Tecnicoprofesional.swupdatedbyinterno = false;

                tbl_sol_Tecnicoprofesional.DocumentoID_Tipo = 0;
                tbl_sol_Tecnicoprofesional.No_Documento = "";
                tbl_sol_Tecnicoprofesional.No_NIT = "";
                tbl_sol_Tecnicoprofesional.DepartamentoDPI_id = 7;
                tbl_sol_Tecnicoprofesional.MunicipioDPI_id = 74;
                tbl_sol_Tecnicoprofesional.Direccion = "";
                tbl_sol_Tecnicoprofesional.Municipio_id = 74;
                tbl_sol_Tecnicoprofesional.Departamento_id = 7;
                tbl_sol_Tecnicoprofesional.PuebloPertenencia_id = 0;
                tbl_sol_Tecnicoprofesional.No_Colegiado = "";
                tbl_sol_Tecnicoprofesional.PostGradoEspecialidad = "";
                tbl_sol_Tecnicoprofesional.Universidad = "";
                tbl_sol_Tecnicoprofesional.Profesion_id = 0;

                tbl_sol_Tecnicoprofesional.Sexo_id = 0;

                //temp
                tbl_sol_Tecnicoprofesional.swdateupdated = DateTime.Now;
                tbl_sol_Tecnicoprofesional.swupdatedby = Usuario_id;

                tbl_sol_Tecnicoprofesional.DocumentoID_Tipo = 1;
                tbl_sol_Tecnicoprofesional.No_Documento = "";
                tbl_sol_Tecnicoprofesional.No_NIT = "";
                tbl_sol_Tecnicoprofesional.Direccion = "";
                tbl_sol_Tecnicoprofesional.PuebloPertenencia_id = 1;
                tbl_sol_Tecnicoprofesional.No_Colegiado = "";
                tbl_sol_Tecnicoprofesional.PostGradoEspecialidad = "";
                tbl_sol_Tecnicoprofesional.Universidad = "";
                tbl_sol_Tecnicoprofesional.Profesion_id = 0;

                tbl_sol_Tecnicoprofesional.Sexo_id = 1;

                //temp

            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_Tecnicoprofesional.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_Tecnicoprofesional.Departamento_id), "Municipio_id", "Municipio", tbl_sol_Tecnicoprofesional.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_Tecnicoprofesional.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_sol_Tecnicoprofesional.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_Tecnicoprofesional.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_sol_Tecnicoprofesional.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_sol_Tecnicoprofesional.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_sol_Tecnicoprofesional.Sexo_id);

            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_sol_Tecnicoprofesional.Profesion_id);

            if ((tbl_sol_Tecnicoprofesional.Grado_Academico_Tecnico == false) && (tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == false))
            {
                tbl_sol_Tecnicoprofesional.Grado_Academico_Tecnico = true;
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Tecnico == true), "Profesion_id", "Descripcion", tbl_sol_Tecnicoprofesional.Profesion_id);
            }

            if (tbl_sol_Tecnicoprofesional.Grado_Academico_Tecnico == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Tecnico == true), "Profesion_id", "Descripcion", tbl_sol_Tecnicoprofesional.Profesion_id);
            }

            if (tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Profesional == true), "Profesion_id", "Descripcion", tbl_sol_Tecnicoprofesional.Profesion_id);
            }

            return View(tbl_sol_Tecnicoprofesional);
        }


        // POST: UsuarioExterno/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult EditProfesional(Tbl_Sol_TecnicoProfesional tbl_sol_Tecnicoprofesional, FormCollection Collection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

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



            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(tbl_sol_Tecnicoprofesional.Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            TempData["Mensaje"] = "";
            bool ErrorDetectado = false;

            if ((!No_NIT_Valido(tbl_sol_Tecnicoprofesional.No_NIT)) && (tbl_sol_Tecnicoprofesional.No_NIT != null))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_sol_Tecnicoprofesional.No_Documento)) && (tbl_sol_Tecnicoprofesional.DocumentoID_Tipo == 1))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }

            if (DiferenciaAnio(tbl_sol_Tecnicoprofesional.Fecha_Nacimiento?? DateTime.Now) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }

            if (tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == false && tbl_sol_Tecnicoprofesional.Grado_Academico_Tecnico == false)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar su grado profesional ó tecnico.";
                ErrorDetectado = true;
            }

            if (tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == true && (tbl_sol_Tecnicoprofesional.No_Colegiado??"").Length < 2)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar el No. de colegiado.";
                ErrorDetectado = true;
            }


            if ((tbl_sol_solicitud.Categoria_id == 7) && ((tbl_sol_solicitud.Sub_Categoria_id == 3) || (tbl_sol_solicitud.Sub_Categoria_id == 4)))
            {
                if ((tbl_sol_Tecnicoprofesional.No_De_Diploma??"") == "" )
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar en los datos profesionales el número de diploma obtenido en INAB.";
                    ErrorDetectado = true;
                }
            }

            if (tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == true) 
            {
                Tbl_Gral_Profesion Tbl_gral_Profesion = db.Tbl_Gral_Profesion.Find(tbl_sol_Tecnicoprofesional.Profesion_id);

                if ((Tbl_gral_Profesion.Descripcion ?? "").ToUpper().Contains("OTRA") == true)
                {
                    if ((tbl_sol_Tecnicoprofesional.UniversidadEspecificarCarrera ?? "") == "")
                    { 
                        TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar la carrera universitaria.";
                        ErrorDetectado = true;
                    }
                }
            }

            if (tbl_sol_Tecnicoprofesional.PostGradoMateriaForestal == true)
            {
                if ((tbl_sol_Tecnicoprofesional.PostGradoUniversidad ?? "") == "")
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar la universitaria del postgrado.";
                    ErrorDetectado = true;
                }

                if ((tbl_sol_Tecnicoprofesional.PostGradoEspecialidad ?? "") == "")
                {
                    TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar el postgrado obtenido.";
                    ErrorDetectado = true;
                }

            }


            if (tbl_sol_Tecnicoprofesional.Profesion_id == 0)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: No ha seleccionado su profesion";
                ErrorDetectado = true;
            }

            if ((tbl_sol_Tecnicoprofesional.Grado_Academico_Profesional == true) && ((tbl_sol_Tecnicoprofesional.Universidad.Length <4) || (tbl_sol_Tecnicoprofesional.No_Colegiado == "---")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y número de colegiado.";
                ErrorDetectado = true;
            }

            if ((tbl_sol_Tecnicoprofesional.PostGradoMateriaForestal == true) && (((tbl_sol_Tecnicoprofesional.PostGradoEspecialidad ?? "") == "---")|| ((tbl_sol_Tecnicoprofesional.PostGradoEspecialidad ?? "") == "") || ((tbl_sol_Tecnicoprofesional.PostGradoUniversidad??"") == "")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y detalles del postgrado obtenido.";
                ErrorDetectado = true;
            }

            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_sol_Tecnicoprofesional).State = EntityState.Modified;
                db.SaveChanges();
                // SP_Seg_UsuarioExterno_EncriptarContraseña

                Session[Constants.session_User] = objUs;

                TempData["MensajeHome"] = "     Datos actualizados con éxito.";
                TempData["MensajeHomeTwo"] = "      Puede utilizar las distintas opciones según la gestión requerida.";

                return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });

            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_Tecnicoprofesional.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_Tecnicoprofesional.Departamento_id), "Municipio_id", "Municipio", tbl_sol_Tecnicoprofesional.Municipio_id);


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_Tecnicoprofesional.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_sol_Tecnicoprofesional.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_Tecnicoprofesional.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_sol_Tecnicoprofesional.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_sol_Tecnicoprofesional.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_sol_Tecnicoprofesional.Sexo_id);
            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_sol_Tecnicoprofesional.Profesion_id);

            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_sol_solicitud.Solicitud_id, firma = tbl_sol_solicitud.Guid_id });
        }


        public int DiferenciaAnio(DateTime Fecha)
        {
            int intDias;
            TimeSpan dias = DateTime.Now.Subtract(Fecha);

            intDias = dias.Days;

            return intDias / 364;
        }

        public bool No_NIT_Valido(string No_NIT)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_NIT_Valido(@p0)", No_NIT).FirstOrDefault(); ;
        }

        public bool DPI_Valido(string No_DPI)
        {
            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
        }

    }
}

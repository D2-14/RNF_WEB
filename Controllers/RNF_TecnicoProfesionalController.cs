using RNF_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Controllers
{
    public class RNF_TecnicoProfesionalController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_TecnicoProfesional

        public ActionResult EditProfesional(string No_Registro)
        {


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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();


            ViewBag.SubCategoria = tbl_RNF_Registro.Sub_Categoria_id;

            Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional = db.Tbl_RNF_TecnicoProfesional.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            if (tbl_RNF_TecnicoProfesional == null)
            {
                tbl_RNF_TecnicoProfesional = new Tbl_RNF_TecnicoProfesional();

                tbl_RNF_TecnicoProfesional.No_Registro = tbl_RNF_Registro.No_Registro;
                tbl_RNF_TecnicoProfesional.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                tbl_RNF_TecnicoProfesional.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                tbl_RNF_TecnicoProfesional.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

                tbl_RNF_TecnicoProfesional.swdateupdated = DateTime.Now;
                tbl_RNF_TecnicoProfesional.swupdatedby = Usuario_id;
                tbl_RNF_TecnicoProfesional.swupdatedbyinterno = false;

                tbl_RNF_TecnicoProfesional.DocumentoID_Tipo = 0;
                tbl_RNF_TecnicoProfesional.No_Documento = "";
                tbl_RNF_TecnicoProfesional.No_NIT = "";
                tbl_RNF_TecnicoProfesional.DepartamentoDPI_id = 7;
                tbl_RNF_TecnicoProfesional.MunicipioDPI_id = 74;
                tbl_RNF_TecnicoProfesional.Direccion = "";
                tbl_RNF_TecnicoProfesional.Municipio_id = 74;
                tbl_RNF_TecnicoProfesional.Departamento_id = 7;
                tbl_RNF_TecnicoProfesional.PuebloPertenencia_id = 0;
                tbl_RNF_TecnicoProfesional.No_Colegiado = "";
                tbl_RNF_TecnicoProfesional.PostGradoEspecialidad = "";
                tbl_RNF_TecnicoProfesional.Universidad = "";
                tbl_RNF_TecnicoProfesional.Profesion_id = 0;

                tbl_RNF_TecnicoProfesional.Sexo_id = 0;

                //temp
                tbl_RNF_TecnicoProfesional.swdateupdated = DateTime.Now;
                tbl_RNF_TecnicoProfesional.swupdatedby = Usuario_id;

                tbl_RNF_TecnicoProfesional.DocumentoID_Tipo = 1;
                tbl_RNF_TecnicoProfesional.No_Documento = "";
                tbl_RNF_TecnicoProfesional.No_NIT = "";
                tbl_RNF_TecnicoProfesional.Direccion = "";
                tbl_RNF_TecnicoProfesional.PuebloPertenencia_id = 1;
                tbl_RNF_TecnicoProfesional.No_Colegiado = "";
                tbl_RNF_TecnicoProfesional.PostGradoEspecialidad = "";
                tbl_RNF_TecnicoProfesional.Universidad = "";
                tbl_RNF_TecnicoProfesional.Profesion_id = 0;

                tbl_RNF_TecnicoProfesional.Sexo_id = 1;

                //temp


            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_TecnicoProfesional.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_TecnicoProfesional.Departamento_id), "Municipio_id", "Municipio", tbl_RNF_TecnicoProfesional.Municipio_id);

            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_TecnicoProfesional.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_TecnicoProfesional.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_TecnicoProfesional.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_TecnicoProfesional.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_TecnicoProfesional.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_TecnicoProfesional.Sexo_id);

            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_RNF_TecnicoProfesional.Profesion_id);

            if ((tbl_RNF_TecnicoProfesional.Grado_Academico_Tecnico == false) && (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == false))
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Profesion_id == 0), "Profesion_id", "Descripcion", tbl_RNF_TecnicoProfesional.Profesion_id);
            }

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Tecnico == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Tecnico == true), "Profesion_id", "Descripcion", tbl_RNF_TecnicoProfesional.Profesion_id);
            }

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == true)
            {
                ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion.Where(Obj => Obj.Profesional == true), "Profesion_id", "Descripcion", tbl_RNF_TecnicoProfesional.Profesion_id);
            }


            return View(tbl_RNF_TecnicoProfesional);
        }


        // POST: UsuarioExterno/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult EditProfesional(Tbl_RNF_TecnicoProfesional tbl_RNF_TecnicoProfesional, FormCollection Collection)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            TempData["Mensaje"] = "";
            bool ErrorDetectado = false;

            if ((!No_NIT_Valido(tbl_RNF_TecnicoProfesional.No_NIT)) && (tbl_RNF_TecnicoProfesional.No_NIT != null))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El número de NIT proporcionado es invalido.";
                ErrorDetectado = true;
            }


            if ((!DPI_Valido(tbl_RNF_TecnicoProfesional.No_Documento)) && (tbl_RNF_TecnicoProfesional.DocumentoID_Tipo == 1))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
            }

            if (DiferenciaAnio(tbl_RNF_TecnicoProfesional.Fecha_Nacimiento ?? DateTime.Now) < 18)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "El propietario no puede ser menor de edad.";
                ErrorDetectado = true;
            }

            if (tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == false && tbl_RNF_TecnicoProfesional.Grado_Academico_Tecnico == false)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Debe especificar su grado profesional ó tecnico.";
                ErrorDetectado = true;
            }

            if (tbl_RNF_TecnicoProfesional.Profesion_id == 0)
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: No ha seleccionado su profesion";
                ErrorDetectado = true;
            }

            if ((tbl_RNF_TecnicoProfesional.Grado_Academico_Profesional == true) && ((tbl_RNF_TecnicoProfesional.Universidad == "") || (tbl_RNF_TecnicoProfesional.No_Colegiado == "")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y número de colegiado.";
                ErrorDetectado = true;
            }

            if ((tbl_RNF_TecnicoProfesional.PostGradoMateriaForestal == true) && ((tbl_RNF_TecnicoProfesional.PostGradoEspecialidad == "") || (tbl_RNF_TecnicoProfesional.PostGradoUniversidad == "")))
            {
                TempData["Mensaje"] = TempData["Mensaje"] + "Error: Debe completar sus datos de universidad y detalles del postgrado obtenido.";
                ErrorDetectado = true;
            }


            if ((ModelState.IsValid) && ErrorDetectado == false)
            {
                db.Entry(tbl_RNF_TecnicoProfesional).State = EntityState.Modified;
                db.SaveChanges();
                // SP_Seg_UsuarioExterno_EncriptarContraseña

                Session[Constants.session_User] = objUs;

                TempData["MensajeHome"] = "     Datos actualizados con éxito.";
                TempData["MensajeHomeTwo"] = "      Puede utilizar las distintas opciones según la gestión requerida.";

                return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_TecnicoProfesional.No_Registro });

            }

            ViewBag.Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_TecnicoProfesional.Departamento_id);
            ViewBag.Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_TecnicoProfesional.Departamento_id), "Municipio_id", "Municipio", tbl_RNF_TecnicoProfesional.Municipio_id);


            ViewBag.DepartamentoDPI_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_TecnicoProfesional.DepartamentoDPI_id);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.Where(Obj => Obj.Estado == true), "DocumentoID_Tipo", "Descripcion", tbl_RNF_TecnicoProfesional.DocumentoID_Tipo);
            ViewBag.MunicipioDPI_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_TecnicoProfesional.DepartamentoDPI_id), "Municipio_id", "Municipio", tbl_RNF_TecnicoProfesional.MunicipioDPI_id);
            ViewBag.PuebloPertenencia_id = new SelectList(db.Tbl_Gral_PuebloPertenencia, "Pueblo_id", "Descripcion", tbl_RNF_TecnicoProfesional.PuebloPertenencia_id);
            ViewBag.Sexo_id = new SelectList(db.Tbl_Gral_Sexo, "Sexo_id", "Descripcion", tbl_RNF_TecnicoProfesional.Sexo_id);
            ViewBag.Profesion_id = new SelectList(db.Tbl_Gral_Profesion, "Profesion_id", "Descripcion", tbl_RNF_TecnicoProfesional.Profesion_id);

            return RedirectToAction("../RNF_Gestor/Index", new { No_Registro = tbl_RNF_TecnicoProfesional.No_Registro});
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



        [HttpPost]
        public JsonResult GetProfesion(string No_Registro, bool GAT, bool GAP)
        {
            IEnumerable<Tbl_Gral_Profesion> Profesiones = null;

            if ((GAT == false) && (GAP == false))
            {
                Profesiones = (from c in db.Tbl_Gral_Profesion
                               where c.Profesion_id == 0
                               select c);
            }
            else
            {
                Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

                if (tbl_RNF_Registro == null)
                {
                    Profesiones = (from c in db.Tbl_Gral_Profesion
                                   where c.Profesion_id == 0
                                   select c);

                }
                else
                {

                    if (tbl_RNF_Registro.Sub_Categoria_id == 1) //Regente
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.Regente == true)
                                       select c);

                    }
                    if (tbl_RNF_Registro.Sub_Categoria_id == 2) //Elaborador de planes de manejo forestal
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.ElaboradorDePlanesManejoForestal == true)
                                       select c);
                    }
                    if (tbl_RNF_Registro.Sub_Categoria_id == 3) //Elaborador de estudios de capacidad de uso del suelo
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.ElaboradorDeEstudiosUsodelSuelo == true)
                                       select c);

                    }
                    if (tbl_RNF_Registro.Sub_Categoria_id == 4) //Certificador de fuentes semilleras
                    {
                        Profesiones = (from c in db.Tbl_Gral_Profesion
                                       where (c.Tecnico == GAT || GAT == false)
                                             &&
                                             (c.Profesional == GAP || GAP == false)
                                             &&
                                             (c.CertificadorDeFuentesSemilleras == true)
                                       select c);
                    }

                }

            }



            var Municipios = new SelectList(Profesiones, "Profesion_id", "Descripcion");

            return Json(new SelectList(Municipios, "Value", "Text"));

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;
using System.Data.SqlClient;


namespace RNF_Web.Controllers
{
    public class Sol_Rodal_Dasometrico_Especie_FormulaController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_Rodal_Dasometrico_Especie_Formula
        public ActionResult Index(long solicitud_id, string firma)
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

            var tbl_Sol_Rodal_Dasometrico_Especie_Formula = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj => Obj.Solicitud_id == solicitud_id);
            return View(tbl_Sol_Rodal_Dasometrico_Especie_Formula.ToList());

        }

        // GET: Sol_Rodal_Dasometrico_Especie_Formula/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Sol_Rodal_Dasometrico_Especie_Formula/Create
        public ActionResult Create(long solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            Tbl_Sol_Rodal_Dasometrico_Especie_Formula tbl_sol_Rodal_Dasometrico_Especie_Formula = new Tbl_Sol_Rodal_Dasometrico_Especie_Formula();

            tbl_sol_Rodal_Dasometrico_Especie_Formula.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            tbl_sol_Rodal_Dasometrico_Especie_Formula.swcreatedby = objUs.intUsuario_id;
            tbl_sol_Rodal_Dasometrico_Especie_Formula.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_sol_Rodal_Dasometrico_Especie_Formula.swcreatedbyinterno = false;

            }
            else
            {
                tbl_sol_Rodal_Dasometrico_Especie_Formula.swcreatedbyinterno = true;
            }

            ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                 from d in db.Tbl_Sol_Rodal_Dasometrico
                                                  where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                  &&  c.Especie_Id == d.Especie_Id 
                                                  select c).ToList(), "Especie_Id", "NombreCientifico");

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", "0");


            int intCategoriaPlantacionesForestales = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoriaPlantacionesForestales > 0)
            { 
                    ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                         from d in db.Tbl_Sol_Rodal_Dasometrico
                                                         where c.ArbolFrutal == true
                                                         && d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                         && c.Especie_Id == d.Especie_Id
                                                         select c).ToList(), "Especie_Id", "NombreCientifico");

                    ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", "0");

            }

            ViewBag.RodalDasometricoEspecieFormula_Id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id == null || Obj.Especie_id == "0" ), "RodalDasometricoEspecieFormula_Id", "Descripcion", "0");

            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula);
        }




        // POST: Sol_Rodal_Dasometrico_Especie_Formula/Create
        [HttpPost]
        public ActionResult Create(Tbl_Sol_Rodal_Dasometrico_Especie_Formula tbl_sol_Rodal_Dasometrico_Especie_Formula)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                 from d in db.Tbl_Sol_Rodal_Dasometrico
                                                 where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                 && c.Especie_Id == d.Especie_Id
                                                 select c).ToList(), "Especie_Id", "NombreCientifico");

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id);

            int intCategoriaPlantacionesForestales = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoriaPlantacionesForestales > 0)
            {
                ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                     from d in db.Tbl_Sol_Rodal_Dasometrico
                                                     where c.ArbolFrutal == true
                                                     && d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                     && c.Especie_Id == d.Especie_Id
                                                     select c).ToList(), "Especie_Id", "NombreCientifico");

                ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", "0");

            }

            ViewBag.RodalDasometricoEspecieFormula_Id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id), "RodalDasometricoEspecieFormula_Id", "Descripcion", tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id);
            if (ModelState.IsValid)
            {

                int Cantidad = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj=> Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.RodalDasometricoEspecieFormula_Id == tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id && Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id).Count();
                string TempFormula = tbl_sol_Rodal_Dasometrico_Especie_Formula.strFormulario;

//--------------------------- Inicio SP  -----------------------------------------
                tbl_sol_Rodal_Dasometrico_Especie_Formula.strFormulario = "";

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };


                string sqlQuery = "Exec Sp_Sol_Upd_Rodal_Dasometrico_Especie_Formula @Solicitud_id, @Especie_id, @RodalDasometricoEspecieFormula_Id,@strFormulario";
                SqlParameter[] sqlParams;


                sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Especie_id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@RodalDasometricoEspecieFormula_Id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@strFormulario",  Value =TempFormula, Direction = System.Data.ParameterDirection.Input}
               };


                //--------------------------- Fin SP  -----------------------------------------



                if (Cantidad > 0)
                {
                    db.Entry(tbl_sol_Rodal_Dasometrico_Especie_Formula).State = EntityState.Modified;
                    db.SaveChanges();

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    return RedirectToAction("../Home/RegistroActualizado");
                }
                else
                {
                    try
                    {
                        db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Add(tbl_sol_Rodal_Dasometrico_Especie_Formula);
                        db.SaveChanges();

                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    }
                    catch (Exception ex)
                    {
                        ViewBag.Mensaje = ex.ToString();

                    }


                    return RedirectToAction("../Home/RegistroAgregado");
                }

            }
            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula);

        }


        public ActionResult Edit(int rodalDasometricoEspecieFormula_Id, string especie_id, long solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            Tbl_Sol_Rodal_Dasometrico_Especie_Formula tbl_sol_Rodal_Dasometrico_Especie_Formula = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj=> Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.RodalDasometricoEspecieFormula_Id == rodalDasometricoEspecieFormula_Id && Obj.Especie_id == especie_id).First();

            tbl_sol_Rodal_Dasometrico_Especie_Formula.swupdatedby = objUs.intUsuario_id;
            tbl_sol_Rodal_Dasometrico_Especie_Formula.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_sol_Rodal_Dasometrico_Especie_Formula.swupdatedbyinterno = false;

            }
            else
            {
                tbl_sol_Rodal_Dasometrico_Especie_Formula.swupdatedbyinterno = true;
            }

            ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                 from d in db.Tbl_Sol_Rodal_Dasometrico
                                                 where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                 && c.Especie_Id == d.Especie_Id
                                                 select c).ToList(), "Especie_Id", "NombreCientifico");

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id);


            int intCategoriaPlantacionesForestales = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoriaPlantacionesForestales > 0)
            {
                ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                     from d in db.Tbl_Sol_Rodal_Dasometrico
                                                     where c.ArbolFrutal == true
                                                     && d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                     && c.Especie_Id == d.Especie_Id
                                                     select c).ToList(), "Especie_Id", "NombreCientifico");

                ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", "0");

            }

            ViewBag.RodalDasometricoEspecieFormula_Id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id.Contains(tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id)), "RodalDasometricoEspecieFormula_Id", "Descripcion", tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id);


            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula);
        }



        // POST: Sol_Rodal_Dasometrico_Especie_Formula/Create
        [HttpPost]
        public ActionResult Edit(Tbl_Sol_Rodal_Dasometrico_Especie_Formula tbl_sol_Rodal_Dasometrico_Especie_Formula)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                 from d in db.Tbl_Sol_Rodal_Dasometrico
                                                 where d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                 && c.Especie_Id == d.Especie_Id
                                                 select c).ToList(), "Especie_Id", "NombreCientifico");

            ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id);

            int intCategoriaPlantacionesForestales = db.Tbl_Sol_Solicitud_Categoria.Where(Obj => Obj.Categoria_id == tbl_sol_solicitud.Categoria_id && (Obj.Descripcion.Contains("Plantaciones de arboles frutales"))).Count();

            if (intCategoriaPlantacionesForestales > 0)
            {
                ViewBag.Especie_id = new SelectList((from c in db.Tbl_Gral_Especie
                                                     from d in db.Tbl_Sol_Rodal_Dasometrico
                                                     where c.ArbolFrutal == true
                                                     && d.Solicitud_id == tbl_sol_solicitud.Solicitud_id
                                                     && c.Especie_Id == d.Especie_Id
                                                     select c).ToList(), "Especie_Id", "NombreCientifico");

                ViewBag.Especie_id = new SelectList(db.Tbl_Gral_Especie, "Especie_Id", "NombreCientifico", "0");

            }

            ViewBag.RodalDasometricoEspecieFormula_Id = new SelectList(db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id.Where(Obj => Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id), "RodalDasometricoEspecieFormula_Id", "Descripcion", tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id);
            if (ModelState.IsValid)
            {

                int Cantidad = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.RodalDasometricoEspecieFormula_Id == tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id && Obj.Especie_id == tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id).Count();
                string TempFormula = tbl_sol_Rodal_Dasometrico_Especie_Formula.strFormulario;

                //--------------------------- Inicio SP  -----------------------------------------
                tbl_sol_Rodal_Dasometrico_Especie_Formula.strFormulario = "";

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };


                string sqlQuery = "Exec Sp_Sol_Upd_Rodal_Dasometrico_Especie_Formula @Solicitud_id, @Especie_id, @RodalDasometricoEspecieFormula_Id,@strFormulario";
                SqlParameter[] sqlParams;


                sqlParams = new SqlParameter[]
               {
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Especie_id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.Especie_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@RodalDasometricoEspecieFormula_Id",  Value = tbl_sol_Rodal_Dasometrico_Especie_Formula.RodalDasometricoEspecieFormula_Id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@strFormulario",  Value =TempFormula, Direction = System.Data.ParameterDirection.Input}
               };


                //--------------------------- Fin SP  -----------------------------------------



                if (Cantidad > 0)
                {
                    db.Entry(tbl_sol_Rodal_Dasometrico_Especie_Formula).State = EntityState.Modified;
                    db.SaveChanges();

                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    return RedirectToAction("../Home/RegistroActualizado");
                }
                else
                {
                    try
                    {
                        db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Add(tbl_sol_Rodal_Dasometrico_Especie_Formula);
                        db.SaveChanges();

                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                    }
                    catch (Exception ex)
                    {
                        ViewBag.Mensaje = ex.ToString();

                    }


                    return RedirectToAction("../Home/RegistroAgregado");
                }

            }
            return View(tbl_sol_Rodal_Dasometrico_Especie_Formula);

        }


        public ActionResult Borrar(int rodalDasometricoEspecieFormula_Id, string especie_id, long solicitud_id, string firma)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal_Dasometrico_Especie_Formula tbl_sol_Rodal_Dasometrico_Especie_Formula = db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.RodalDasometricoEspecieFormula_Id == rodalDasometricoEspecieFormula_Id && Obj.Especie_id == especie_id).First();


            db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula.Remove(tbl_sol_Rodal_Dasometrico_Especie_Formula);
            db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");
        }



        [HttpPost]
        public JsonResult GetFormulas(string especie_id)
        {

            IEnumerable<Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id> Formulas = (from c in db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id
                                                         where c.Especie_id.Contains(especie_id)
                                                         select c);

            if (Formulas.Count() == 0)
            {
                Formulas = (from c in db.Tbl_Sol_Rodal_Dasometrico_Especie_Formula_Id
                            where c.Especie_id == "0"
                            select c);

            }

            var formulas = new SelectList(Formulas, "RodalDasometricoEspecieFormula_Id", "Descripcion");

            return Json(new SelectList(formulas, "Value", "Text"));

        }




    }
}

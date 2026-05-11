using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_SolicitudController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Sol_Solicitud
        public ActionResult Index()
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

            Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = db.Tbl_Seg_UsuarioExterno.Find(objUs.intUsuario_id);

           

            if (tbl_Seg_UsuarioExterno.Usuario_id == 583)
            {
               var  tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Estado_id != 6 && Obj.Estado_id != 10 && (Obj.swcreatedby == objUs.intUsuario_id || Obj.DPI_Titular == tbl_Seg_UsuarioExterno.No_Documento)).OrderByDescending(x => x.swdatecreated);
                return View(tbl_Sol_Solicitud.ToList());
            }
            else
            {
               var  tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Where(Obj => Obj.Estado_id != 6  && (Obj.swcreatedby == objUs.intUsuario_id || Obj.DPI_Titular == tbl_Seg_UsuarioExterno.No_Documento)).OrderByDescending(x => x.swdatecreated);

            return View(tbl_Sol_Solicitud.ToList());
            }

        }


        public ActionResult IndexPropietarioRepresentante(long id, string firma)
        {
            ViewBag.Sol_id = id;
            ViewBag.Guid_id = firma;

            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_Sol_Solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_Sol_Solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            return View(tbl_Sol_Solicitud);
        }

        // GET: Sol_Solicitud
        public ActionResult IndexPropietario(long id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            int PropietariosIndividuales = db.Tbl_Sol_PropietarioPersonaIndividual.Where(obj => obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && obj.Estado_id == true).Count();
            int PropietariosJuridicos = db.Tbl_Sol_PropietarioPersonaJuridica.Where(obj => obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && obj.Estado_id == true).Count();

                ViewBag.Personeria = new SelectList(db.Tbl_Sol_Solicitud_Personeria, "PersoneriaTipo_id", "Descripcion", 0);
            if (PropietariosIndividuales > 0)
            {
                ViewBag.Personeria = new SelectList(db.Tbl_Sol_Solicitud_Personeria.Where(Obj=> Obj.PersoneriaTipo_id==1), "PersoneriaTipo_id", "Descripcion", 0);
            }
            if(PropietariosJuridicos > 0)
            {
                ViewBag.Personeria = new SelectList(db.Tbl_Sol_Solicitud_Personeria.Where(Obj => Obj.PersoneriaTipo_id == 2), "PersoneriaTipo_id", "Descripcion", 0);
            }

            var ListodoDePropietariosRegistrados = db.fc_Sol_Sel_ListadoDePropietario(id);

            ViewBag.Sol_id = id;
            ViewBag.Guid_id = firma;
            ViewBag.Solicitud_id = id;
            return View(ListodoDePropietariosRegistrados);

        }

        public ActionResult IndexArrendatario(long id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            int PropietariosIndividuales = db.Tbl_Sol_ArrendatarioPersonaIndividual.Where(obj => obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && obj.Estado_id == true).Count();

            if (PropietariosIndividuales > 0)
            {
                ViewBag.PersoneriaArrendatario = new SelectList(db.Tbl_Sol_Solicitud_Personeria.Where(Obj => Obj.PersoneriaTipo_id == 1), "PersoneriaTipo_id", "Descripcion", 0);
            }
            else
            {
                ViewBag.PersoneriaArrendatario = new SelectList(db.Tbl_Sol_Solicitud_Personeria, "PersoneriaTipo_id", "Descripcion", 0);
            }

            var ListodoDePropietariosRegistrados = db.fc_Sol_Sel_ListadoDeArrendatario(id);
            ViewBag.Solicitud_id = id;
            return View(ListodoDePropietariosRegistrados);

        }

        // GET: Sol_Solicitud/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Sol_Solicitud tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);
            if (tbl_Sol_Solicitud == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Sol_Solicitud);
        }

        // GET: Sol_Solicitud/Create
        public ActionResult Create(long? id, string firma)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud;

            if ((id == null) || (id == 0))
            {

                tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();

                tbl_Sol_Solicitud.Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Sub_Categoria_id = 0;
                tbl_Sol_Solicitud.AreaTotalFincas = 0;
                tbl_Sol_Solicitud.Region_id = 0;
                tbl_Sol_Solicitud.SubRegion_id = 0;

                tbl_Sol_Solicitud.Notificacion_Municipio_id = 74;
                tbl_Sol_Solicitud.Notificacion_Departamento_id = 7;
                tbl_Sol_Solicitud.Notificacion_Direccion = "";

                Session[Constants.session_Solicitud] = (long) 0;

            }
            else
            {
                tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

                    if (tbl_Sol_Solicitud == null)
                    {
                        return HttpNotFound();
                    }

                    if (tbl_Sol_Solicitud.Guid_id != firma)
                    {
                        return HttpNotFound();
                    }

                Session[Constants.session_Solicitud] = (long) id;

            }


            if (Session[Constants.session_Tbl_Sol_Solicitud] != null)
            {
                tbl_Sol_Solicitud = (Tbl_Sol_Solicitud) Session[Constants.session_Tbl_Sol_Solicitud];
            }

            if (objUs.CorreoElectronico == "motosierra@inab.gob.gt")
            {
                //crejo  si ingresan con el correo de motosierra, solo les despliega categoria "motosierra"  Id == 8
                ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria.Where(obj => obj.Categoria_id == 0 || obj.Categoria_id == 8), "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            }
            else
            {
                            
                ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);

            }

            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj=> obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj=> (obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id==0 && tbl_Sol_Solicitud.Sub_Sub_Categoria_id == 0 )), "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);


            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);



            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);

            return View(tbl_Sol_Solicitud);
        }


        // POST: Sol_Solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
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

            /* Si es una nueva solicitud el estado es Cero */
            if (tbl_Sol_Solicitud.Solicitud_id == 0)
            {
                tbl_Sol_Solicitud.Estado_id = 0;
            }



            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Solicitud.Max(u => u.Solicitud_id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }


            tbl_Sol_Solicitud.Solicitud_id = lngIdt;

            if (tbl_Sol_Solicitud.Notificacion_Direccion == null)
            {
                tbl_Sol_Solicitud.Notificacion_Direccion = "";
                tbl_Sol_Solicitud.Notificacion_Municipio_id = 74;
                tbl_Sol_Solicitud.Notificacion_Departamento_id = 7;
                tbl_Sol_Solicitud.DepartamentoSolicitud_id = 7;
                tbl_Sol_Solicitud.MunicipioSolicitud_id = 74;
            }


            if (tbl_Sol_Solicitud.Region_id == null)
            {
                tbl_Sol_Solicitud.Region_id = 0;
                tbl_Sol_Solicitud.SubRegion_id = 0;
            }



            if (ModelState.IsValid)
            {
                string sqlQuery;
                SqlParameter[] sqlParams;

                sqlQuery = "Exec SP_Sol_InsUpd_Solicitud @Solicitud_id, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id ,@SubRegion_id, @Estado_id, @AreaTotalFincas, @Notificacion_Direccion, @Notificacion_Municipio_id, @Notificacion_Departamento_id, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]

                        {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_Sol_Solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Categoria_id",  Value = tbl_Sol_Solicitud.Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Sub_Categoria_id??0, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Region_id",  Value = tbl_Sol_Solicitud.Region_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@SubRegion_id",  Value = tbl_Sol_Solicitud.SubRegion_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Estado_id",  Value = tbl_Sol_Solicitud.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@AreaTotalFincas",  Value = tbl_Sol_Solicitud.AreaTotalFincas, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Notificacion_Direccion",  Value = tbl_Sol_Solicitud.Notificacion_Direccion, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Notificacion_Municipio_id",  Value = tbl_Sol_Solicitud.Notificacion_Municipio_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Notificacion_Departamento_id",  Value = tbl_Sol_Solicitud.Notificacion_Departamento_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@swcreatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swcreatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                        };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                if (resultado[0].respuesta == 1)
                {

                    ViewBag.Mensaje_II = "Paso completado.";
                    ViewBag.RegistroGrabado = 1;
                    Session[Constants.session_Solicitud] = resultado[0].id;

                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = resultado[0].mensaje });

                }
                else
                {
                    TempData["Mensaje"] = resultado[0].mensaje;
                    ViewBag.Mensaje = resultado[0].mensaje;
                    ViewBag.RegistroGrabado = 0;
                }
            }
            else
            {
                if (tbl_Sol_Solicitud.Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = " Seleccione la categoría de la solicitud "; }
                if (tbl_Sol_Solicitud.Sub_Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la sub categoría de la solicitud "; }

                TempData["MensajeSolicitudFail"] = @TempData["MensajeSolicitudFail"] + "-- Favor corregir --";
                ViewBag.RegistroGrabado = 0;
            }



            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);


            Session[Constants.session_Tbl_Sol_Solicitud] = (Tbl_Sol_Solicitud)tbl_Sol_Solicitud;

            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = 0, firma = tbl_Sol_Solicitud.Guid_id });


        }


        public ActionResult visualizar(long solicitud_id, string firma)
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

            if ((solicitud_id == 0))
            {

                tbl_sol_solicitud = new Tbl_Sol_Solicitud();

                tbl_sol_solicitud.Categoria_id = 0;
                tbl_sol_solicitud.Sub_Categoria_id = 0;
                tbl_sol_solicitud.Sub_Sub_Categoria_id = 0;

                tbl_sol_solicitud.Region_id = 0;
                tbl_sol_solicitud.SubRegion_id = 0;


                Session[Constants.session_Solicitud] = (long)0;

            }

            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_sol_solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_sol_solicitud.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_sol_solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_sol_solicitud.Categoria_id && obj.Sub_Categoria_id == tbl_sol_solicitud.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_sol_solicitud.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_sol_solicitud.Sub_Sub_Categoria_id);


            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_sol_solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_sol_solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_sol_solicitud.SubRegion_id);

            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_sol_solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_sol_solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_sol_solicitud.Notificacion_Municipio_id);

            // return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = tbl_Sol_Solicitud.Guid_id });

            return View(tbl_sol_solicitud);
        }


        [HttpPost]
        public ActionResult visualizar(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud_Current = db.Tbl_Sol_Solicitud.Find(tbl_Sol_Solicitud.Solicitud_id);



            int tbl_Sol_Solicitud_Estado_id = tbl_Sol_Solicitud_Current.Estado_id ?? 0;

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


            tbl_Sol_Solicitud_Current.Sub_Sub_Categoria_id = tbl_Sol_Solicitud.Sub_Sub_Categoria_id;
            tbl_Sol_Solicitud_Current.swupdatedby = objUs.intUsuario_id;
            tbl_Sol_Solicitud_Current.swdateupdated = DateTime.Now;

            db.Entry(tbl_Sol_Solicitud_Current).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud_Current.Solicitud_id, firma = tbl_Sol_Solicitud_Current.Guid_id });


        }



        public ActionResult CreateConRegion(long? id, string firma)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud;

            if ((id == null) || (id == 0))
            {

                tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();

                tbl_Sol_Solicitud.Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Sub_Categoria_id = 0;

                tbl_Sol_Solicitud.Region_id = 0;
                tbl_Sol_Solicitud.SubRegion_id = 0;

                tbl_Sol_Solicitud.Notificacion_Municipio_id = 74;
                tbl_Sol_Solicitud.Notificacion_Departamento_id = 7;
                tbl_Sol_Solicitud.Notificacion_Direccion = "";

                Session[Constants.session_Solicitud] = (long)0;

            }
            else
            {
                tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

                if (tbl_Sol_Solicitud == null)
                {
                    return HttpNotFound();
                }

                if (tbl_Sol_Solicitud.Guid_id != firma)
                {
                    return HttpNotFound();
                }

                Session[Constants.session_Solicitud] = (long)id;
            }

            //if (Session[Constants.session_Tbl_Sol_Solicitud] != null)
            //{
            //    tbl_Sol_Solicitud = (Tbl_Sol_Solicitud)Session[Constants.session_Tbl_Sol_Solicitud];
            //}


            if (objUs.CorreoElectronico == "motosierra@inab.gob.gt")
            {
                ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria.Where(obj => obj.Categoria_id == 0 || obj.Categoria_id == 8), "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Sub_Categoria_id == 1), "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);

            }
            else
            {

            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
                ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);

            }



            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_Sol_Solicitud.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);


            string sqlQuery;


            //sqlQuery = " Select D.*";
            //sqlQuery += " From Tbl_Gral_Departamento D, Tbl_Gral_Region R, Tbl_Gral_SubRegion SR, Tbl_Gral_SubRegionDepartamento RD ";
            //sqlQuery += " where SR.SubRegion_id =" + tbl_Sol_Solicitud.SubRegion_id;
            //sqlQuery += "   and SR.SubRegion_id = RD.SubRegion_id ";
            //sqlQuery += "   and R.Id_Region = SR.Region_id ";
            //sqlQuery += "   and D.Departamento_id = RD.Id_Departamento ";
            //sqlQuery += "   group by D.Departamento_id, D.Departamento, D.OldDepartamento_Id ";
            

            sqlQuery = " Select D.*";
            sqlQuery += " From Tbl_Gral_Departamento D";            
            sqlQuery += "   group by D.Departamento_id, D.Departamento,d.OldDepartamento_Id";

            List<Tbl_Gral_Departamento> tbl_Gral_Departamento = new List<Tbl_Gral_Departamento> { };

            tbl_Gral_Departamento = db.Database.SqlQuery<Tbl_Gral_Departamento>(sqlQuery).ToList();

            ViewBag.Notificacion_Departamento_id = new SelectList(tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);

            if (tbl_Gral_Departamento.Count() != 0)
            { 
            int QueryDeptoId = tbl_Gral_Departamento.FirstOrDefault().Departamento_id;

                //IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                //                                             from sr in db.Tbl_Gral_SubRegionDepartamento
                //                                             where c.Departamento_id == QueryDeptoId
                //                                               && sr.SubRegion_id == tbl_Sol_Solicitud.SubRegion_id
                //                                               && c.Municipio_id == sr.Id_Municipio
                //                                             select c);

                IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio                                                            
                                                             //where c.Departamento_id == QueryDeptoId                                                             
                                                             select c);

                //ViewBag.Notificacion_Municipio_id = new SelectList(Municipio.Where(Obj => Obj.Departamento_id == QueryDeptoId), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);
                //CR_24/07/24
                ViewBag.Notificacion_Municipio_id = new SelectList(Municipio, "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);

            }
            else
            {
                ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj=> Obj.Municipio ==" No vale "), "Municipio_id", "Municipio");

            }
            if ((tbl_Sol_Solicitud.Region_id == 0) && (tbl_Sol_Solicitud.Categoria_id == 8))
            { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la región y dirección en la que realizará la gestión, para continuar. "; }


            return View(tbl_Sol_Solicitud);
        }

        public ActionResult CreateSoloRegion(long? id)
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

            Tbl_Sol_Solicitud tbl_Sol_Solicitud;

            if ((id == null) || (id == 0))
            {

                tbl_Sol_Solicitud = new Tbl_Sol_Solicitud();

                tbl_Sol_Solicitud.Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Categoria_id = 0;
                tbl_Sol_Solicitud.Sub_Sub_Categoria_id = 0;

                tbl_Sol_Solicitud.Region_id = 0;
                tbl_Sol_Solicitud.SubRegion_id = 0;

                tbl_Sol_Solicitud.Notificacion_Municipio_id = 74;
                tbl_Sol_Solicitud.Notificacion_Departamento_id = 7;
                tbl_Sol_Solicitud.Notificacion_Direccion = "";

                Session[Constants.session_Solicitud] = (long)0;

            }
            else
            {
                tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

                if (tbl_Sol_Solicitud == null)
                {
                    return HttpNotFound();
                }

                Session[Constants.session_Solicitud] = (long)id;


            }


            if (Session[Constants.session_Tbl_Sol_Solicitud] != null)
            {
                tbl_Sol_Solicitud = (Tbl_Sol_Solicitud)Session[Constants.session_Tbl_Sol_Solicitud];
            }


            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria.Where(obj => obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id), "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria.Where(obj => (obj.Categoria_id == tbl_Sol_Solicitud.Categoria_id && obj.Sub_Categoria_id == tbl_Sol_Solicitud.Sub_Categoria_id) || (obj.Sub_Sub_Categoria_id == 0 && tbl_Sol_Solicitud.Sub_Sub_Categoria_id == 0)), "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);



            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);


            if ((tbl_Sol_Solicitud.Region_id == 0) && (tbl_Sol_Solicitud.Categoria_id == 8))
            { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la región y dirección en la que realizará la gestión, para continuar. "; }


            return View(tbl_Sol_Solicitud);
        }

        [HttpPost]
        public ActionResult CreateConRegion(Tbl_Sol_Solicitud tbl_Sol_Solicitud)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud_Current = db.Tbl_Sol_Solicitud.Find(tbl_Sol_Solicitud.Solicitud_id);

            int tbl_Sol_Solicitud_Estado_id = tbl_Sol_Solicitud_Current.Estado_id??0;

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

            /* Si es una nueva solicitud el estado es Cero */
            if (tbl_Sol_Solicitud.Solicitud_id == 0)
            {
                tbl_Sol_Solicitud.Estado_id = 0;
            
                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Sol_Solicitud.Max(u => u.Solicitud_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Sol_Solicitud.Solicitud_id = lngIdt;
            }

            if ((tbl_Sol_Solicitud_Estado_id != 0) && (tbl_Sol_Solicitud_Estado_id != 4))
            {
                return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = tbl_Sol_Solicitud.Guid_id });

            }

            if ((ModelState.IsValid) && (tbl_Sol_Solicitud.Notificacion_Direccion != null))
            {
                string sqlQuery;
                SqlParameter[] sqlParams;


                sqlQuery = "Exec SP_Sol_InsUpd_Solicitud @Solicitud_id, @Categoria_id, @Sub_Categoria_id, @Sub_Sub_Categoria_id, @Region_id ,@SubRegion_id, @Estado_id, @AreaTotalFincas, @Notificacion_Direccion, @Notificacion_Municipio_id, @Notificacion_Departamento_id, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                        {
                                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_Sol_Solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Categoria_id",  Value = tbl_Sol_Solicitud.Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Sub_Sub_Categoria_id",  Value = tbl_Sol_Solicitud.Sub_Sub_Categoria_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Region_id",  Value = tbl_Sol_Solicitud.Region_id ?? tbl_Sol_Solicitud_Current.Region_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@SubRegion_id",  Value = tbl_Sol_Solicitud.SubRegion_id ?? tbl_Sol_Solicitud_Current.SubRegion_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Estado_id",  Value = tbl_Sol_Solicitud.Estado_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@AreaTotalFincas",  Value = (tbl_Sol_Solicitud.AreaTotalFincas??tbl_Sol_Solicitud_Current.AreaTotalFincas)??0, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@Notificacion_Direccion",  Value = tbl_Sol_Solicitud.Notificacion_Direccion, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Notificacion_Municipio_id",  Value = tbl_Sol_Solicitud.Notificacion_Municipio_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@Notificacion_Departamento_id",  Value = tbl_Sol_Solicitud.Notificacion_Departamento_id, Direction = System.Data.ParameterDirection.Input },

                                        new SqlParameter { ParameterName = "@swcreatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                                        new SqlParameter { ParameterName = "@swcreatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                        };


                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                if (resultado[0].respuesta == 1)
                {

                    ViewBag.Mensaje_II = "Paso completado.";
                    ViewBag.RegistroGrabado = 1;
                    Session[Constants.session_Solicitud] = resultado[0].id;

                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = tbl_Sol_Solicitud.Guid_id });

                }
                else
                {
                    TempData["Mensaje"] = resultado[0].mensaje;
                    ViewBag.Mensaje = resultado[0].mensaje;
                    ViewBag.RegistroGrabado = 0;
                    return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = tbl_Sol_Solicitud.Guid_id });

                }
            }
            else
            {
                if (tbl_Sol_Solicitud.Notificacion_Direccion == null)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Es obligatorio ingresar la dirección de notificación. "; }
                if (tbl_Sol_Solicitud.Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la categoría de la solicitud "; }
                if (tbl_Sol_Solicitud.Sub_Categoria_id == 0)
                { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la sub categoría de la solicitud "; }

                TempData["MensajeSolicitudFail"] = @TempData["MensajeSolicitudFail"] + "-- Favor corregir --";
                ViewBag.RegistroGrabado = 0;
            }




            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_Sol_Solicitud.Categoria_id);
            ViewBag.Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Categoria_id);
            ViewBag.Sub_Sub_Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Sub_Categoria, "Sub_Sub_Categoria_id", "Descripcion", tbl_Sol_Solicitud.Sub_Sub_Categoria_id);

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_Sol_Solicitud.Region_id);
            ViewBag.SubRegion_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_Sol_Solicitud.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_Sol_Solicitud.SubRegion_id);



            ViewBag.Notificacion_Departamento_id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_Sol_Solicitud.Notificacion_Departamento_id);
            ViewBag.Notificacion_Municipio_id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_Sol_Solicitud.Notificacion_Departamento_id), "Municipio_id", "Municipio", tbl_Sol_Solicitud.Notificacion_Municipio_id);

            Session[Constants.session_Tbl_Sol_Solicitud] = (Tbl_Sol_Solicitud)tbl_Sol_Solicitud;


            if ((tbl_Sol_Solicitud.Region_id == 0) && (tbl_Sol_Solicitud.Categoria_id == 8))
            { TempData["MensajeSolicitudFail"] = TempData["MensajeSolicitudFail"] + " Seleccione la región en la que realizará la gestión, para continuar. "; }



            return RedirectToAction("../Home/SolicitudInsertUpdate", new { id = tbl_Sol_Solicitud.Solicitud_id, firma = tbl_Sol_Solicitud.Guid_id });


        }

        // GET: Sol_Solicitud/Create
        public ActionResult PersoneriaJuridica(long id)
        {

            Tbl_Sol_Solicitud tbl_Sol_Solicitud;

            tbl_Sol_Solicitud = db.Tbl_Sol_Solicitud.Find(id);

            if (tbl_Sol_Solicitud == null)
            {
                return HttpNotFound();
            }


            return View(tbl_Sol_Solicitud);


        }


        public ActionResult ConversorGTM()
        {
            return View();
        }
        public ActionResult ConversorGR(decimal? GTMX, decimal? GTMY)
        {
            ViewBag.GTMX = GTMX;
            ViewBag.GTMY = GTMY;
            CoordenadasResponse coordenadasResponse = new CoordenadasResponse();
            ConvertirGTMModel convertirGTMModel = new ConvertirGTMModel();
            ConvertirGTMModel.GR_GTM coordinates = new ConvertirGTMModel.GR_GTM()
            {
                GR_Latitud = 0,
                GR_Longitud = 0,
                GTM_Y = 0,
                GTM_X = 0,
            };
            coordenadasResponse.Coordenadas = coordinates;

            if ((GTMX != null) && (GTMY != null))
            {
                try
                {
                    coordenadasResponse.Coordenadas = convertirGTMModel.Convertir_GTM_2_GR((decimal)GTMX, (decimal)GTMY);
                    coordenadasResponse.Result = 1;
                    coordenadasResponse.Mensaje = "Coordenadas convertidas";
                }
                catch (Exception ex)
                {
                }

            }

            ViewBag.JSONCoordenadasRazor = coordenadasResponse; return View();
        }

        public JsonResult ConvertGRJunto(CoordenadasRequest model)
        {
            CoordenadasResponse coordenadasResponse = new CoordenadasResponse();
            string[] separador = { "," };
            string[] coord = model.Coordenadas.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            if ((coord.Length < 2) || (coord.Length > 2))
            {
                coordenadasResponse.Result = 2;
                coordenadasResponse.Mensaje = "La cantidad de datos no coincide, por favor revise que las coordenadas estén escritas como en el ejemplo: 14.59651062320446, -90.52819352681975";
                coordenadasResponse.Coordenadas = null;
            }
            else
            {
                string latitud = coord[0];
                string longitud = coord[1];
                model.Latitud = decimal.Parse(latitud);
                model.Longitud = decimal.Parse(longitud);

                ConvertirGTMModel convertirGTMModel = new ConvertirGTMModel();
                coordenadasResponse.Coordenadas = convertirGTMModel.Convertir_GR_2_GTM(model.Longitud, model.Latitud);
                coordenadasResponse.Coordenadas.GTM_X = decimal.Parse(coordenadasResponse.Coordenadas.GTM_X.ToString("0"));
                coordenadasResponse.Coordenadas.GTM_Y = decimal.Parse(coordenadasResponse.Coordenadas.GTM_Y.ToString("0"));
                TempData["GR_GTMX"] = coordenadasResponse.Coordenadas.GTM_X;
                TempData["GR_GTMY"] = coordenadasResponse.Coordenadas.GTM_Y;
                coordenadasResponse.Result = 1;
                coordenadasResponse.Mensaje = "Coordenadas convertidas";
            }

            return Json(coordenadasResponse);
        }

        public JsonResult ConvertGR(CoordenadasRequest model)
        {
            CoordenadasResponse coordenadasResponse = new CoordenadasResponse();
            ConvertirGTMModel convertirGTMModel = new ConvertirGTMModel();
            coordenadasResponse.Coordenadas = convertirGTMModel.Convertir_GR_2_GTM(model.Longitud, model.Latitud);
            coordenadasResponse.Coordenadas.GTM_X = decimal.Parse(coordenadasResponse.Coordenadas.GTM_X.ToString("0"));
            coordenadasResponse.Coordenadas.GTM_Y = decimal.Parse(coordenadasResponse.Coordenadas.GTM_Y.ToString("0"));
            TempData["GR_GTMX"] = coordenadasResponse.Coordenadas.GTM_X;
            TempData["GR_GTMY"] = coordenadasResponse.Coordenadas.GTM_Y;
            coordenadasResponse.Result = 1;
            coordenadasResponse.Mensaje = "Coordenadas convertidas";
            return Json(coordenadasResponse);
        }

        public JsonResult ConvertGTM(CoordenadasRequest model)
        {
            CoordenadasResponse coordenadasResponse = new CoordenadasResponse();
            ConvertirGTMModel convertirGTMModel = new ConvertirGTMModel();
            coordenadasResponse.Coordenadas = convertirGTMModel.Convertir_GTM_2_GR(model.GTMX, model.GTMY);
            //coordenadasResponse.Coordenadas = convertirGTMModel.Convertir_GTM_SQL_GR(model.GTMX, model.GTMY);
            coordenadasResponse.Result = 1;
            coordenadasResponse.Mensaje = "Coordenadas convertidas";
            return Json(coordenadasResponse);
        }



        [HttpPost]
        public JsonResult GetSubCategorias(int Categoria)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            objUs = (Usuario)Session["User"];

            //crejo  si ingresan con el correo de motosierra, solo les despliega la subcateria == motosierra
            if (objUs.CorreoElectronico == "motosierra@inab.gob.gt")
            {
                IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                     where c.Categoria_id == Categoria
                                                                                     && c.Sub_Categoria_id == 1
                                                                                       && c.Visible == true
                                                                                     select c);
            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");
            return Json(new SelectList(SubCategoria, "Value", "Text"));

            }

            else
            {

                IEnumerable<Tbl_Sol_Solicitud_Sub_Categoria> SubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Categoria
                                                                                     where c.Categoria_id == Categoria
                                                                                       && c.Visible == true
                                                                                     select c);

            var SubCategoria = new SelectList(SubCategoriaSelected, "Sub_Categoria_id", "Descripcion");
            return Json(new SelectList(SubCategoria, "Value", "Text"));
            }

        }

        [HttpPost]
        public JsonResult GetSubSubCategorias(int Categoria, int SubCategoria)
        {

            IEnumerable<Tbl_Sol_Solicitud_Sub_Sub_Categoria> SubSubCategoriaSelected = (from c in db.Tbl_Sol_Solicitud_Sub_Sub_Categoria
                                                                                        where c.Categoria_id == Categoria
                                                                                           && c.Sub_Categoria_id == SubCategoria
                                                                                        select c);

            var SubSubCategoria = new SelectList(SubSubCategoriaSelected, "Sub_Sub_Categoria_id", "Descripcion");


            return Json(new SelectList(SubSubCategoria, "Value", "Text"));

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
        public JsonResult GetMunicipiosRegion(int Departamento, int SubRegionId)
        {

            IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                                                         from sr in db.Tbl_Gral_SubRegionDepartamento
                                                         where c.Departamento_id == Departamento
                                                           &&  sr.SubRegion_id == SubRegionId
                                                           && c.Municipio_id == sr.Id_Municipio
                                                         select c);

            var Municipios = new SelectList(Municipio, "Municipio_id", "Municipio");

            return Json(new SelectList(Municipios, "Value", "Text"));

        }


  
        [HttpPost]
        public JsonResult GetDepartamento(int SubRegionId)
        {

                string sqlQuery;


                sqlQuery = " Select D.*";
                sqlQuery += " From Tbl_Gral_Departamento D, Tbl_Gral_Region R, Tbl_Gral_SubRegion SR, Tbl_Gral_SubRegionDepartamento RD ";
                sqlQuery += " where SR.SubRegion_id =" + SubRegionId;
                sqlQuery += "   and SR.SubRegion_id = RD.SubRegion_id ";
                sqlQuery += "   and R.Id_Region = SR.Region_id ";
                sqlQuery += "   and D.Departamento_id = RD.Id_Departamento ";
                sqlQuery += "   group by D.Departamento_id, D.Departamento, D.OldDepartamento_Id ";

                List<Tbl_Gral_Departamento> tbl_Gral_Departamento = new List<Tbl_Gral_Departamento> { };

                tbl_Gral_Departamento = db.Database.SqlQuery<Tbl_Gral_Departamento>(sqlQuery).ToList();

                var Departamento = new SelectList(tbl_Gral_Departamento, "Departamento_id", "Departamento");



            return Json(new SelectList(Departamento, "Value", "Text"));

        }



        public ContentResult Get()
        {
            return new System.Web.Mvc.ContentResult
            {
                Content = "Hi there! ☺",
                ContentType = "text/plain; charset=utf-8"
            };
        }

        public ContentResult Gets()
        {

            IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                                                         where c.Departamento_id == 1
                                                         select c);

            var Municipios = new SelectList(Municipio, "Municipio_id", "Municipio");


            return new System.Web.Mvc.ContentResult
            {
                Content = Municipios.ToString(),
                ContentType = "text/plain; charset=utf-8"
            };
        }


        public JsonResult GetAjaxValue()
        {
            return Json("string value", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSubRegiones()
        {
            IEnumerable<Tbl_Gral_Municipio> Municipio = (from c in db.Tbl_Gral_Municipio
                                                         where c.Departamento_id == 1
                                                         select c);

            var Municipios = new SelectList(Municipio, "Municipio_id", "Municipio");



            return Json( Municipios, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public JsonResult GetSubRegion(int Region)
        {
            // Done  
            var SubRegiones = new SelectList(db.Tbl_Gral_SubRegion.Where(Obj=> Obj.Region_id == Region && Obj.Estado_id == true ), "SubRegion_id", "Nombre_SubRegionCompleto");
            return Json(new SelectList(SubRegiones, "Value", "Text"));

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

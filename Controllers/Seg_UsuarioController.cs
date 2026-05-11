using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using PagedList;
using System.Threading;
using ExcelDataReader;

namespace RNF_Web.Controllers
{
    public class Seg_UsuarioController : Controller
    {

        private db_RNFEntities db = new db_RNFEntities();


        // GET: Carga_AreaProtegida
        public ActionResult Carga()
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                //  "../Login/Index"
                //  "../Login/AccesoColaborador"
                return RedirectToAction("../Login/AccesoColaborador");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadUsuarios(HttpPostedFileBase upload)
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


            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

            int CargaTipo = 14; // Carga de usuarios internos 
            string RedirectTo = "../Seg_Usuario/Carga";
            bool ErrorEncontrado = false;

            TempData["MensajeFile"] = "";


            if (upload != null && upload.ContentLength > 0)
            {

                DateTime hoy = DateTime.Now;
                string fecha = "_" + hoy.Year + "_" + hoy.Month + "_" + hoy.Day + "_" + hoy.Hour + "_" + hoy.Minute + "_" + hoy.Second + "_" + hoy.Millisecond;

                Stream stream = upload.InputStream;
                IExcelDataReader reader = null;

                if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                }
                else
                {

                    TempData["MensajeFile"] = TempData["MensajeFile"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                    return RedirectToAction(RedirectTo);
                }

                try
                {
                    DataSet datDatosExcel = reader.AsDataSet();


                    DataTable dt = datDatosExcel.Tables[0];

                    string DatoDeCampo = dt.Rows[5][0].ToString();

                    if (DatoDeCampo != "Carga de usuarios internos")
                    {
                        ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                        ErrorEncontrado = true;
                    }


                    DatoDeCampo = dt.Rows[6][0].ToString();

                    if (DatoDeCampo != "Nombres")
                    {
                        ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                        ErrorEncontrado = true;
                    }


                    DatoDeCampo = dt.Rows[6][6].ToString();

                    if (DatoDeCampo != "Sub-Región")
                    {
                        ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                        ErrorEncontrado = true;
                    }
                    if (ErrorEncontrado == false)
                    {
                        string sqlQuery;
                        sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                        SqlParameter[] sqlParams;
                        int intContador = 0;
                        long lnCarga_id;

                        sqlParams = new SqlParameter[]
                        {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                        };

                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                        //Cargar detalle
                        if (resultado[0].respuesta == 1)
                        {
                            lnCarga_id = resultado[0].id;
                            intContador = 7;
                            try
                            {
                                while (dt.Rows[intContador][0].ToString() != "")
                                {
                                    sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                    sqlParams = new SqlParameter[]
                                       {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = dt.Rows[intContador][0].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = dt.Rows[intContador][1].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = dt.Rows[intContador][2].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = dt.Rows[intContador][3].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = dt.Rows[intContador][4].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = dt.Rows[intContador][5].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = dt.Rows[intContador][6].ToString(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo14",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo15",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo16",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo17",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo18",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo19",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo20",  Value = 0, Direction = System.Data.ParameterDirection.Input}
                                       };


                                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                    intContador = intContador + 1;
                                }


                                if (intContador < 1)
                                {
                                    TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";

                                    TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                    //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                }
                                else
                                {
                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                    {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                    };

                                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                    if (resultado[0].respuesta != 1)
                                    {
                                        TempData["MensajeFile"] = resultado[0].mensaje;
                                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";
                                    }
                                    else
                                    {

                                        Tbl_Gral_Carga tbl_Gral_CargaB = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == CargaTipo & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                                        if (tbl_Gral_CargaB != null)
                                        {
                                            TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>" + tbl_Gral_CargaB.Observaciones ?? "";
                                        }
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                if (intContador < 1)
                                {
                                    TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                    TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                    //return RedirectToAction("../Sol_Rodal/UploadExcel");
                                }
                                else
                                {
                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };

                                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                    if (resultado[0].respuesta != 1)
                                    {
                                        TempData["MensajeFile"] = resultado[0].mensaje;
                                        TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";

                                    }

                                }
                            }

                            Tbl_Gral_Carga tbl_Gral_CargaC = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == CargaTipo & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                            if (tbl_Gral_CargaC != null)
                            {
                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" + tbl_Gral_CargaC.Observaciones ?? "";
                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                            }

                        }



                    }

                    return RedirectToAction(RedirectTo);
                }
                catch (Exception Ex)
                {
                    TempData["MensajeFile"] = TempData["MensajeFile"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente." + Ex.ToString();
                    return RedirectToAction(RedirectTo);
                }
            }
            else
            {
                TempData["MensajeFile"] = "Por favor seleccione el archivo con los usuarios, roles y permisos que desee proporcionar.";
            }

            return RedirectToAction(RedirectTo);
        }




        // GET: Seg_Usuario
        public ActionResult Index(string sortOrder, string idx, int? page)
        {
            //Se reemplazó el IQueryable, por List debido a que dio problema de compatibilidad con SQL Server 2008 R2
            List<Tbl_Seg_Usuario> ListarUsuarios = (from d in db.Tbl_Seg_Usuario
                                                          orderby d.Usuario_id
                                                          select d).ToList();

            var lst = (from d in ListarUsuarios select d);

            if ((idx != null) && (idx.Trim() != ""))
            {
                idx = idx.ToLower();
                //lst = lst.Where(x => ((x.email.Contains(idx)) || (x.Nombre.Contains(idx)) || (x.Apellidos.Contains(idx))));
                ListarUsuarios = (from d in ListarUsuarios
                                  where d.email.ToLower().Contains(idx) || d.Nombre.ToLower().Contains(idx) || d.Apellidos.ToLower().Contains(idx)
                                  select d).ToList();
            }



            //if ((idx != null) && (idx != ""))
            //{
            //    return View(db.Tbl_Seg_Usuario.Where(x => ((x.email.Contains(idx)) || (x.Nombre.Contains(idx)) || (x.Apellidos.Contains(idx)))).ToList());
            //}
            //return View(db.Tbl_Seg_Usuario.ToList().Take(100));

            ViewBag.CurrentSort = sortOrder;
            ViewBag.idx = idx;
            ViewBag.Nombre = sortOrder == "Nombre" ? "nombre_desc" : "Nombre";
            ViewBag.email = sortOrder == "email" ? "email_desc" : "email";

            switch (sortOrder)
            {
                case "Nombre":
                    ListarUsuarios = ListarUsuarios.OrderBy(Obj => Obj.Nombre).ToList();
                    break;
                case "nombre_desc":
                    ListarUsuarios = ListarUsuarios.OrderByDescending(Obj => Obj.Nombre).ToList();
                    break;
                case "email":
                    ListarUsuarios = ListarUsuarios.OrderBy(Obj => Obj.email).ToList();
                    break;
                case "email_desc":
                    ListarUsuarios = ListarUsuarios.OrderByDescending(Obj => Obj.email).ToList();
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(ListarUsuarios.ToPagedList(pageNumber, pageSize));

        }

        public string AgregarFirmaNueva(HttpPostedFileBase archivo, int id)
        {
            DateTime FechaCarga = DateTime.Now;
            string formatoarchivo, path, filename, PathCrear, partialpath, partialdirectory;
            string FormatoFechaCarga = FechaCarga.ToString("yyyyMMdd_HHmmss");
            string NombreArchivo = "FirmaDigital_";
            partialdirectory = "/Archivos_ConFirmaElectronica/FirmaDigital/";
            path = Server.MapPath($"~{partialdirectory}");
            formatoarchivo = Path.GetExtension(archivo.FileName);
            partialpath = $"{NombreArchivo}{id}{formatoarchivo.ToLower()}";
            if (archivo != null)
            {
                var data = new byte[archivo.ContentLength];
                archivo.InputStream.Read(data, 0, archivo.ContentLength);
                filename = Path.Combine(path, $"{partialpath}");

                if ((System.IO.File.Exists(path)) == true)
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    PathCrear = path;

                    if (!Directory.Exists(PathCrear))
                    {
                        Directory.CreateDirectory(PathCrear);
                    }
                }
                System.IO.File.WriteAllBytes(Path.Combine(path, filename), data);
                archivo.SaveAs(filename);
                return $"{partialdirectory}{partialpath}";
            }
            else
            {
                return null;
            }
        }

        public ActionResult FirmaDigital(int id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            string DireccionFirma;
            ViewBag.idUsuario = id;
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
            string partialpath, path, partialdirectory;
            string NombreArchivo = "FirmaDigital_";
            partialdirectory = "/Archivos_ConFirmaElectronica/FirmaDigital/"; 
            partialpath = $"{partialdirectory}{NombreArchivo}{id}.png";
            path = Server.MapPath($"~{partialpath}");

            DireccionFirma = $"{partialpath}";

            if ((System.IO.File.Exists(path)) == true)
            {
                ViewBag.DireccionFirma = DireccionFirma;
            }
            else
            {
                ViewBag.DireccionFirma = null;
            }



            return View();
        }

        [HttpPost]
        public ActionResult FirmaDigital(HttpPostedFileBase Archivo, int id)
        {
            ViewBag.idUsuario = id;

            string archivo = AgregarFirmaNueva(Archivo, id);

            Console.WriteLine(id);
            return View();
        }


        public ActionResult Create()
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

            Tbl_Seg_Usuario tbl_seg_usuario = new Tbl_Seg_Usuario();

            tbl_seg_usuario.swcreatedby = objUs.intUsuario_id;
            tbl_seg_usuario.swdatecreated = DateTime.Now;
            tbl_seg_usuario.swUpdatedby = objUs.intUsuario_id;
            tbl_seg_usuario.swdateUpdated = DateTime.Now;

            return View(tbl_seg_usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Seg_Usuario tbl_Seg_Usuario)
        {
            tbl_Seg_Usuario.Estado = true;

            if (ModelState.IsValid)
            {
                long lngIdt = 0;

                try
                {
                    lngIdt = db.Tbl_Seg_Usuario.Max(u => u.Usuario_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Seg_Usuario.Usuario_id = lngIdt;

                db.Tbl_Seg_Usuario.Add(tbl_Seg_Usuario);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = tbl_Seg_Usuario.Usuario_id });
            }

            return View(tbl_Seg_Usuario);
        }

        // GET: Tbl_Seg_Usuario/Edit/5
        public ActionResult Edit(long? id)
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tbl_Seg_Usuario tbl_Seg_Usuario = db.Tbl_Seg_Usuario.Find(id);
            if (tbl_Seg_Usuario == null)
            {
                return HttpNotFound();
            }

            tbl_Seg_Usuario.swdateUpdated = DateTime.Now;
            tbl_Seg_Usuario.swUpdatedby = objUs.intUsuario_id;

            ViewBag.Estado = new SelectList(db.Tbl_Seg_UsuarioEstado, "Estado_id", "Descripcion", tbl_Seg_Usuario.Estado);
            ViewBag.DocumentoID_Tipo = new SelectList(db.Tbl_Gral_DocumentoID_Tipo.ToList(), nameof(Tbl_Gral_DocumentoID_Tipo.DocumentoID_Tipo), nameof(Tbl_Gral_DocumentoID_Tipo.Descripcion));

            return View(tbl_Seg_Usuario);
        }

        public ActionResult ActualizarMiPalabraClave()
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

            long id = objUs.intUsuario_id;


            Tbl_Seg_Usuario tbl_Seg_Usuario = db.Tbl_Seg_Usuario.Find(id);
            if (tbl_Seg_Usuario == null)
            {
                return HttpNotFound();
            }

            tbl_Seg_Usuario.swdateUpdated = DateTime.Now;
            tbl_Seg_Usuario.swUpdatedby = objUs.intUsuario_id;

            ViewBag.Estado = new SelectList(db.Tbl_Seg_UsuarioEstado, "Estado_id", "Descripcion", tbl_Seg_Usuario.Estado);

            return View(tbl_Seg_Usuario);
        }

        // POST: Tbl_Seg_Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public JsonResult GrabarUsuario(Tbl_Seg_Usuario tbl_Seg_Usuario)
        {
            string jsonResultUsr;

            int codRespuesta = 0;
            long strUsuarioid = 0;
            string strRespuesta = "Error: Al intentar grabar los datos del usuario.";
            long lngIdt = 0;
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;
            tbl_Seg_Usuario.Usuario = "";
            tbl_Seg_Usuario.Empleado_id = 0;

            bool Passwordmatched = Regex.Match(tbl_Seg_Usuario.PalabraClave, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$").Success;

            if (!Passwordmatched)
            {
                strRespuesta = "Error: La contraseña debe conterner números, letras minusculas, mayusculas y algún caracter especial.";

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                          + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

                return Json(jsonResultUsr);

            }

            tbl_Seg_Usuario.No_Documento = db.Database.SqlQuery<string>($"SELECT [dbo].[Fnc_CasilleroElectronico_Documento_Formateado]('{(tbl_Seg_Usuario.No_Documento ?? "")}')").FirstOrDefault() ?? "";

            if (ModelState.IsValid == false)
            {
                strRespuesta = "Error: Los datos que desea grabar no cumplen con lo solicitado. Quitar guiones o caracteres especiales.";

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                          + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

                return Json(jsonResultUsr);

            }


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {

                strRespuesta = "Error: Debe estar dentro del sistema para poder actualizar datos.";

                jsonResultUsr = "{\"CodRespuesta\":"
                          + "\"" + codRespuesta + "\","
                          + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                          + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

                return Json(jsonResultUsr);


            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            if (tbl_Seg_Usuario.Usuario_id != 0)
            {
                tbl_Seg_Usuario.swUpdatedby = objUs.intUsuario_id;
                tbl_Seg_Usuario.swdateUpdated = DateTime.Now;

                db.Entry(tbl_Seg_Usuario).State = EntityState.Modified;
                db.SaveChanges();

                strUsuarioid = lngIdt;

            }
            else
            {

                int Encontrado = db.Tbl_Seg_Usuario.Where(Obj => Obj.email == tbl_Seg_Usuario.email && Obj.Estado ==true).Count();

                codRespuesta = 0;
                strRespuesta = "Error: El usuario ya se encuentra registrado. No puede registrarlo nuevamente.";

                if (Encontrado > 0)
                {


                    jsonResultUsr = "{\"CodRespuesta\":"
                                     + "\"" + codRespuesta + "\","
                                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
                                     + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";
                    return Json(jsonResultUsr);

                }

                tbl_Seg_Usuario.swcreatedby = objUs.intUsuario_id;
                tbl_Seg_Usuario.swdatecreated = DateTime.Now;
                tbl_Seg_Usuario.swUpdatedby = objUs.intUsuario_id;
                tbl_Seg_Usuario.swdateUpdated = DateTime.Now;

                try
                {
                    lngIdt = db.Tbl_Seg_Usuario.Max(u => u.Usuario_id);
                    lngIdt++;

                }
                catch
                {
                    lngIdt = 1;
                }

                tbl_Seg_Usuario.Usuario_id = lngIdt;


                db.Tbl_Seg_Usuario.Add(tbl_Seg_Usuario);
                db.SaveChanges();

                strUsuarioid = lngIdt;


            }


            codRespuesta = 1;
            strRespuesta = "Registro actualizado con éxito.";


            jsonResultUsr = "{\"CodRespuesta\":"
             + "\"" + codRespuesta + "\","
             + "\"strRespuesta\":" + "\"" + strRespuesta + "\","
             + "\"strUsuarioid\":" + "\"" + strUsuarioid + "\"}";

            return Json(jsonResultUsr);


        }

        public ActionResult UsuarioPermisos(int id)
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

            ViewBag.RolesAsignados = db.fc_Seg_Sel_RolUsuario(id, 1);
            ViewBag.RolesNoAsignados = db.fc_Seg_Sel_RolUsuario(id, 0);

            return View();
        }

        [HttpPost]
        public JsonResult CambioAsignacion(int usuario_id, int rol_id, int nuevoestado)
        {
            string sqlQuery;
            String TextoMostrar;


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            Usuario objUs = (Usuario)Session["User"];

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };


            sqlQuery = "Exec SP_Seg_InsUpd_UsuarioRol @Usuario_id, @Rol_id, @Estado_id, @UsuarioSolicitante";
            SqlParameter[] sqlParams;


            sqlParams = new SqlParameter[]
           {
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = usuario_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Rol_id",  Value = rol_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Estado_id",  Value = nuevoestado, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@UsuarioSolicitante",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input}
           };



            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            TextoMostrar = "Realizado";

            return Json(new { success = TextoMostrar }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult CambioAsignacionRegion(int usuario_id, int region_id, int subregion_id, int nuevoestado)
        {
            string sqlQuery;
            String TextoMostrar;


            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);

            Usuario objUs = (Usuario)Session["User"];

            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                     { new ResultFromStoreProcedure { id = 0, mensaje= "El password no concuerda con el password de confirmación", respuesta = 0 }  };


            sqlQuery = "Exec SP_Seg_InsUpd_UsuarioRegionSubRegion @Usuario_id, @Region_id, @SubRegion_id, @Estado_id, @UsuarioSolicitante";
            SqlParameter[] sqlParams;


            sqlParams = new SqlParameter[]
           {
                              new SqlParameter { ParameterName = "@Usuario_id",  Value = usuario_id, Direction = System.Data.ParameterDirection.Input },
                              new SqlParameter { ParameterName = "@Region_id",  Value = region_id, Direction = System.Data.ParameterDirection.Input},
                              new SqlParameter { ParameterName = "@SubRegion_id",  Value = subregion_id, Direction = System.Data.ParameterDirection.Input},
                              new SqlParameter { ParameterName = "@Estado_id",  Value = nuevoestado, Direction = System.Data.ParameterDirection.Input},
                              new SqlParameter { ParameterName = "@UsuarioSolicitante",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input}
           };



            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();


            TextoMostrar = "Realizado";

            return Json(new { success = TextoMostrar }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UsuarioPermisosRegion(int id)
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

            ViewBag.SubRegionesAsignadas = db.fc_Seg_Sel_RegionSubRegionUsuario(id, 1).OrderBy(s => s.NombreRegion);
            ViewBag.SubRegionesNoAsignadas = db.fc_Seg_Sel_RegionSubRegionUsuario(id, 0).OrderBy(s => s.NombreRegion);

            return View();
        }

    }
}

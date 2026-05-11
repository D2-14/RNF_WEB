using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;
using RNF_Web.Jobs;
using System.Threading;

namespace RNF_Web.Controllers
{
    public class Carga_AreaProtegidaController : Controller
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


        public ActionResult CargaContador()
        {

            ViewBag.CantidadItems = db.Database.SqlQuery<string>("SELECT dbo.fnc_Gral_MensajeEntreProcesos()").FirstOrDefault();

            return View();
        }


        public class Rootobject_sigap
        {
            public string type { get; set; }
            public Crs_sigap crs { get; set; }
            public Feature_sigap[] features { get; set; }
        }

        public class Crs_sigap
        {
            public string type { get; set; }
            public Properties_sigap properties { get; set; }
        }

        public class Properties_sigap
        {
            public string name { get; set; }
        }

        public class Feature_sigap
        {
            public string type { get; set; }
            public int id { get; set; }
            public Geometry_sigap geometry { get; set; }
            public Properties1_sigap properties { get; set; }
        }

        public class Geometry_sigap
        {
            public string type { get; set; }
            public object[][][] coordinates { get; set; }
        }

        public class Properties1_sigap
        {
            public int OBJECTID { get; set; }
            public int codigo_gra { get; set; }
            public int codigo_esp { get; set; }
            public int CÓDIGO { get; set; }
            public string NOMBRE_Gra { get; set; }
            public string Categor_1 { get; set; }
            public int CÓDIGO_1 { get; set; }
            public string NOMBRE_esp { get; set; }
            public string Categor_12 { get; set; }
            public float hectareas { get; set; }
            public float Shape_Length { get; set; }
            public float Shape_Area { get; set; }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadGeojson(HttpPostedFileBase upload)
        {



            String strCantidadTotal;
            int strCantidadActual;

            int CargaTipo = 2;
            string RedirectTo = "../Carga_AreaProtegida/Carga";


            String strLatitud, strLongitud;

            strLatitud = "";
            strLongitud = "";
            string replaceWithNothing = "";

            string Texto_completo;
            string[] Texto_Dividido;

            TempData["MensajeFile"] = "";
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




            if (upload != null && upload.ContentLength > 0)
            {

                DateTime hoy = DateTime.Now;
                string fecha = "_" + hoy.Year + "_" + hoy.Month + "_" + hoy.Day + "_" + hoy.Hour + "_" + hoy.Minute + "_" + hoy.Second + "_" + hoy.Millisecond;

                Stream stream = upload.InputStream;


                if (upload.FileName.EndsWith(".geojson"))
                {

                    String FileName = "Carga_AreaProgegida_" + fecha;

                    string strDir = "FileCargaAreaProgida\\";
                    string strFolder = Server.MapPath("~/") + strDir;

                    upload.SaveAs(Path.Combine(strFolder, FileName));

                    var webClient = new WebClient();
                    var json = webClient.DownloadString(strFolder + FileName);

                    try
                    {

                       new Thread(() => {

                        Rootobject_sigap myDeserializedClass = JsonConvert.DeserializeObject<Rootobject_sigap>(json);

                        string sqlQuery;
                        SqlParameter[] sqlParams;
                        long lnCarga_id;

                           List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                             { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };


                           // Grabar  mensaje entre procesos
                           sqlQuery = "Exec SP_Gral_Ins_MensajeEntreProcesos @Mensaje";
                           TempData["CantidadItems"] = "";

                           sqlParams = new SqlParameter[]
                           {
                                 new SqlParameter { ParameterName = "@Mensaje",  Value = TempData["CantidadItems"], Direction = System.Data.ParameterDirection.Input }
                           };
                           resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                           // Grabar  mensaje entre procesos


                           sqlQuery = "Exec SP_Gral_Baja_AreaProtegida @Tipo_Carga_id";

                        TempData["MensajeFile"] = "Baja realizada con éxito.";

                        sqlParams = new SqlParameter[]
                        {
                            new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input }
                        };

                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                        strCantidadTotal = myDeserializedClass.features.Count().ToString();
                        strCantidadActual = 0;

                        foreach (var GeoReferencia in myDeserializedClass.features)
                        {
                            strCantidadActual = strCantidadActual + 1;


                            // Grabar  mensaje entre procesos
                               sqlQuery = "Exec SP_Gral_Ins_MensajeEntreProcesos @Mensaje";
                               TempData["CantidadItems"] = "Procesando :" + strCantidadActual.ToString() + " de " + strCantidadTotal + " areas.";

                               sqlParams = new SqlParameter[]
                               {
                                 new SqlParameter { ParameterName = "@Mensaje",  Value = TempData["CantidadItems"], Direction = System.Data.ParameterDirection.Input }
                               };
                               resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            // Grabar  mensaje entre procesos




                            sqlQuery = "Exec SP_Gral_Ins_Carga_AreaProtegida @Tipo_Carga_id, @AreaProtegida_id, @Usuario_id, @EsInterno";

                            sqlParams = new SqlParameter[]
                            {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@AreaProtegida_id",  Value = GeoReferencia.properties.OBJECTID, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                            };


                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            lnCarga_id = resultado[0].id;

                            foreach (var GeoPosition in GeoReferencia.geometry.coordinates[0])
                            {

                                if (GeoPosition.Count() == 2)
                                {
                                    strLatitud = GeoPosition[0].ToString();
                                    strLongitud = GeoPosition[1].ToString();

                                    sqlQuery = "Exec SP_Gral_Ins_CargaDetalle_Poligono @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02";

                                    sqlParams = new SqlParameter[]
                                    {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                             new SqlParameter { ParameterName = "@Campo01",  Value = strLatitud, Direction = System.Data.ParameterDirection.Input},
                                             new SqlParameter { ParameterName = "@Campo02",  Value = strLongitud.ToString(), Direction = System.Data.ParameterDirection.Input}
                                    };

                                    resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                }
                                else
                                {

                                    foreach (var GeoPositionInterior in GeoPosition)
                                    {
                                        Texto_completo = GeoPositionInterior.ToString();
                                        Texto_completo = Texto_completo.Replace("\r\n", replaceWithNothing).Replace("\n", replaceWithNothing).Replace("\r", replaceWithNothing).Replace("[", replaceWithNothing).Replace("]", replaceWithNothing).Replace(" ", replaceWithNothing);

                                        Texto_Dividido = Texto_completo.Split(',');
                                        strLatitud = Texto_Dividido[0];
                                        strLongitud = Texto_Dividido[1];

                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle_Poligono @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02";

                                        sqlParams = new SqlParameter[]
                                        {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                             new SqlParameter { ParameterName = "@Campo01",  Value = strLatitud, Direction = System.Data.ParameterDirection.Input},
                                             new SqlParameter { ParameterName = "@Campo02",  Value = strLongitud.ToString(), Direction = System.Data.ParameterDirection.Input}
                                        };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                    }
                                }


                            }

                            sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                            sqlParams = new SqlParameter[]
                           {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                           };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                        }


                           // Grabar  mensaje entre procesos
                           sqlQuery = "Exec SP_Gral_Ins_MensajeEntreProcesos @Mensaje";
                           TempData["CantidadItems"] = "";

                           sqlParams = new SqlParameter[]
                           {
                                 new SqlParameter { ParameterName = "@Mensaje",  Value = TempData["CantidadItems"], Direction = System.Data.ParameterDirection.Input }
                           };
                           resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                           // Grabar  mensaje entre procesos



                       }).Start();  //new Thread(()

                    }
                    catch
                    {
                        TempData["MensajeFile"] = " Error: El Modelo no cumple con la estructura SIGAP, puede utilizar un nuevo geojson.";
                        TempData["CantidadItems"] = "";

                        return RedirectToAction(RedirectTo);


                    }

                }
                else
                {
                    TempData["MensajeFile"] = " El formato de archivo no es soportado. Unicamente archivos GeoJson son soportados. ";
                    TempData["CantidadItems"] = "";
                    return RedirectToAction(RedirectTo);

                }

            }
            else
            {
                TempData["MensajeFile"] = "Por favor seleccione el archivo con los polígonos.";
            }

            return RedirectToAction(RedirectTo);
        }




    }
}
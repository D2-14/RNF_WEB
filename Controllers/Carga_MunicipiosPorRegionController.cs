using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Carga_MunicipiosPorRegionController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        //Crer el controlador
        //Generar las clases  Editar -->> Pegado Especial -->> Json clases

        public ActionResult CargaContador()
        {

            ViewBag.CantidadItems = db.Database.SqlQuery<string>("SELECT dbo.fnc_Gral_MensajeEntreProcesos()").FirstOrDefault();

            return View();
        }

        public class Rootobject
        {
            public string displayFieldName { get; set; }
            public Fieldaliases fieldAliases { get; set; }
            public string geometryType { get; set; }
            public Spatialreference spatialReference { get; set; }
            public Field[] fields { get; set; }
            public Feature[] features { get; set; }
        }

        public class Fieldaliases
        {
            public string FID { get; set; }
            public string FID_DAD_20 { get; set; }
            public string CodReg { get; set; }
            public string NomOfic { get; set; }
            public string FID_DAD_21 { get; set; }
            public string CodSubreg { get; set; }
            public string NomSubreg { get; set; }
            public string FID_DAD_22 { get; set; }
            public string CODIGO { get; set; }
            public string MUNICIPIO { get; set; }
            public string ID { get; set; }
            public string Depto { get; set; }
            public string kfw { get; set; }
            public string FIP { get; set; }
            public string OrdFIP { get; set; }
            public string Shape_Leng { get; set; }
            public string Shape_Area { get; set; }
        }

        public class Spatialreference
        {
            public int wkid { get; set; }
            public int latestWkid { get; set; }
        }

        public class Field
        {
            public string name { get; set; }
            public string type { get; set; }
            public string alias { get; set; }
            public int length { get; set; }
        }

        public class Feature
        {
            public Attributes attributes { get; set; }
            public Geometry geometry { get; set; }
        }

        public class Attributes
        {
            public int FID { get; set; }
            public int FID_DAD_20 { get; set; }
            public string CodReg { get; set; }
            public string NomOfic { get; set; }
            public int FID_DAD_21 { get; set; }
            public string CodSubreg { get; set; }
            public string NomSubreg { get; set; }
            public int FID_DAD_22 { get; set; }
            public int CODIGO { get; set; }
            public string MUNICIPIO { get; set; }
            public int ID { get; set; }
            public string Depto { get; set; }
            public int kfw { get; set; }
            public int FIP { get; set; }
            public int OrdFIP { get; set; }
            public float Shape_Leng { get; set; }
            public float Shape_Area { get; set; }
        }

        public class Geometry
        {
            public float[][][] rings { get; set; }
        }


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
        public ActionResult UploadGeojson(HttpPostedFileBase upload)
        {
            String strCantidadTotal;
            int strCantidadActual;
            int CargaTipo = 3;
            string RedirectTo = "../Carga_MunicipiosPorRegion/Carga";

            String strLatitud, strLongitud;

            strLatitud = "";
            strLongitud = "";

            TempData["MensajeFile"] = "";
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


            if (upload != null && upload.ContentLength > 0)
            {

                DateTime hoy = DateTime.Now;
                string fecha = "_" + hoy.Year + "_" + hoy.Month + "_" + hoy.Day + "_" + hoy.Hour + "_" + hoy.Minute + "_" + hoy.Second + "_" + hoy.Millisecond;

                Stream stream = upload.InputStream;


                if (upload.FileName.EndsWith(".json"))
                {

                    String FileName = "Carga_MunicipioPorRegion_" + fecha;

                    string strDir = "FileCargaAreaProgida\\";
                    string strFolder = Server.MapPath("~/") + strDir;

                    upload.SaveAs(Path.Combine(strFolder, FileName));


                    var webClient = new WebClient();
                    var json = webClient.DownloadString(strFolder + FileName);

                    try
                    {

                        new Thread(() => {


                        Rootobject myDeserializedClass = JsonConvert.DeserializeObject<Rootobject>(json);

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


                            sqlQuery = "Exec SP_Gral_Baja_AreaRegiones @Tipo_Carga_id";
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



                                sqlQuery = "Exec SP_Gral_Ins_Carga_AreaRegiones @Tipo_Carga_id, @CODIGO, @CodReg, @CodSubreg, @Depto, @Usuario_id, @EsInterno";

                                sqlParams = new SqlParameter[]
                                {
                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@CODIGO",  Value = GeoReferencia.attributes.CODIGO, Direction = System.Data.ParameterDirection.Input},
                                 new SqlParameter { ParameterName = "@CodReg",  Value = GeoReferencia.attributes.CodReg, Direction = System.Data.ParameterDirection.Input},
                                 new SqlParameter { ParameterName = "@CodSubreg",  Value = GeoReferencia.attributes.CodSubreg, Direction = System.Data.ParameterDirection.Input},
                                 new SqlParameter { ParameterName = "@Depto",  Value = GeoReferencia.attributes.Depto, Direction = System.Data.ParameterDirection.Input},
                                 new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                                 new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                                };

                                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                lnCarga_id = resultado[0].id;


                                foreach (var GeoPosition in GeoReferencia.geometry.rings[0])
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
                                        //            Texto_completo = GeoPositionInterior.ToString();
                                        //            Texto_completo = Texto_completo.Replace("\r\n", replaceWithNothing).Replace("\n", replaceWithNothing).Replace("\r", replaceWithNothing).Replace("[", replaceWithNothing).Replace("]", replaceWithNothing).Replace(" ", replaceWithNothing);

                                        //            Texto_Dividido = Texto_completo.Split(',');
                                        //            strLatitud = Texto_Dividido[0];
                                        //            strLongitud = Texto_Dividido[1];


                                        //            sqlQuery = "Exec SP_Gral_Ins_CargaDetalle_AreaProgeda @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02";


                                        //            sqlParams = new SqlParameter[]
                                        //           {
                                        //                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = CargaTipo, Direction = System.Data.ParameterDirection.Input },
                                        //                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                        //                 new SqlParameter { ParameterName = "@Campo01",  Value = strLatitud, Direction = System.Data.ParameterDirection.Input},
                                        //                 new SqlParameter { ParameterName = "@Campo02",  Value = strLongitud.ToString(), Direction = System.Data.ParameterDirection.Input}
                                        //           };

                                        //            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

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

                        return RedirectToAction(RedirectTo);

                    }

                }
                else
                {
                    TempData["MensajeFile"] = " El formato de archivo no es soportado. Unicamente archivos de json son soportados. ";

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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotSpatial.Topology;
using ExcelDataReader;
using Newtonsoft.Json;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_RodalController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        // GET: RNF_Rodal
        public ActionResult Index(string Guid_id)
        {
            ViewBag.Guid_id = Guid_id;

            var tbl_rnf_rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == Guid_id);

            ViewBag.Inconvenientes = db.fc_RNF_Sel_Revision(Guid_id).Where(Obj => Obj.Bloqueante == true);

            return View(tbl_rnf_rodal.ToList());
        }


        public ActionResult IndexDescuento(string Guid_id)
        {
            ViewBag.Guid_id = Guid_id;
            var tbl_rnf_rodal = db.Tbl_RNF_Rodal_Descuento.Where(Obj => Obj.No_Registro == Guid_id);
            return View(tbl_rnf_rodal.ToList());
        }

        public JsonResult SetArea(string Guid_id, int Rodal_id, decimal decimal_area)
        {
            if (Rodal_id == 0)
            {
                return Json("");
            }

            if (decimal_area == 0)
            {
                return Json("");
            }

            Tbl_RNF_Rodal rnf_rodal = db.Tbl_RNF_Rodal.Where(Rodal => Rodal.No_Registro == Guid_id && Rodal.Rodal_Id == Rodal_id).First();

            rnf_rodal.AreaTotalCalculadaSistema = decimal_area;

            db.Entry(rnf_rodal).State = EntityState.Modified;
            db.SaveChanges();

            return Json("");

        }


        public void Evaluacion_AreaRodalDescuento(string No_Registro, long finca_id, int tipo_area_id, long rodal_id, long rodal_descuento_id)
        {

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_RNF_Rodal_Descuento tbl_RNF_Rodal = db.Tbl_RNF_Rodal_Descuento.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id).FirstOrDefault();
            //Tbl_Sol_Rodal_Descuento sol_sol_rodal = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id).First();

            IEnumerable<Tbl_RNF_Rodal_Descuento_Poligono> tbl_rnf_rodal_poligono = db.Tbl_RNF_Rodal_Descuento_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id);
            //IEnumerable<Tbl_Sol_Rodal_Descuento_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Descuento_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_rnf_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            tbl_RNF_Rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            tbl_RNF_Rodal.AreaTotal = tbl_RNF_Rodal.AreaTotalCalculadaSistema;

            tbl_RNF_Rodal.GTMX = (decimal)polyRodal.Centroid.X;
            tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;


            try
            {
                db.Entry(tbl_RNF_Rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;

        }

        public void Evaluacion_AreaRodal(string No_Registro, long finca_id, int tipo_area_id, long rodal_id)
        {

            //ViewBag.Area = "";
            //ViewBag.Region = "Región no encontrada. El Rodal no pertenecea a Guatemala o existe un problema para identificarlo.";
            //ViewBag.AreaProtegida = "El rodal no está sobre ningún área protegida.";
            //ViewBag.Centroide = "No se encontró el centroide del rodal.";
            //ViewBag.Invasion = "";

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                return;
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Finca_id == finca_id && Obj.Tipo_de_Area == tipo_area_id && Obj.Rodal_Id == rodal_id).FirstOrDefault();
            //Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_RNF_Rodal_Poligono> tbl_rnf_rodal_poligono = db.Tbl_RNF_Rodal_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);
            //IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };
            int IsPrimera = 0;
            decimal PrimeraX = (Decimal)0.000;
            decimal PrimeraY = (Decimal)0.000;

            foreach (var Item in tbl_rnf_rodal_poligono)
            {
                if (IsPrimera == 0)
                {
                    PrimeraX = (Decimal)Item.GTMX;
                    PrimeraY = (Decimal)Item.GTMY;


                }
                IsPrimera = IsPrimera + 1;
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            tbl_RNF_Rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            tbl_RNF_Rodal.AreaTotal = tbl_RNF_Rodal.AreaTotalCalculadaSistema;

            if (tbl_RNF_Rodal.Tipo_de_Area == 2)
            {
                tbl_RNF_Rodal.AreaTotal = 0;
                tbl_RNF_Rodal.GTMX = PrimeraX;
                tbl_RNF_Rodal.GTMY = PrimeraY;
            }
            else
            {
                tbl_RNF_Rodal.GTMX = (decimal)polyRodal.Centroid.X;
                tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }


            tbl_RNF_Rodal.Longitud_Total = tbl_RNF_Rodal.Longitud_Total ?? 0;




            Tbl_RNF_Finca Tbl_RNF_FincaUpdate = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Rodal.No_Registro && Obj.Finca_Id == finca_id).FirstOrDefault();

            Tbl_RNF_FincaUpdate.GTMX = tbl_RNF_Rodal.GTMX;
            Tbl_RNF_FincaUpdate.GTMY = tbl_RNF_Rodal.GTMY;

            db.Entry(Tbl_RNF_FincaUpdate).State = EntityState.Modified;
            db.SaveChanges();

            try
            {
                db.Entry(tbl_RNF_Rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }
        public void Evaluacion_RegionRodal(string No_Registro, long finca_id, int tipo_area_id, long rodal_id)
        {

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).FirstOrDefault();
            //Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_RNF_Rodal_Poligono> tbl_rnf_rodal_poligono = db.Tbl_RNF_Rodal_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);
            //IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_rnf_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalRegionPoligonoEvaluar(" + tbl_RNF_Rodal.GTMX.ToString() + ", " + tbl_RNF_Rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaRegion> LstRegiones = new List<Tbl_Gral_AreaRegion>();

            LstRegiones = db.Tbl_Gral_AreaRegion.SqlQuery(Query).ToList();

            var coordinatesRegion = new List<Coordinate>() { };

            Polygon polyRegion;

            tbl_RNF_Rodal.AreaRegion_Id = 0;

            foreach (var ItemRegion in LstRegiones)
            {
                IEnumerable<Tbl_Gral_AreaRegion_Poligono> tbl_gral_arearegion_poligono = db.Tbl_Gral_AreaRegion_Poligono.Where(Rodal => Rodal.AreaRegion_id == ItemRegion.AreaRegion_Id);

                coordinatesRegion = new List<Coordinate>() { };

                foreach (var ItemRegionPolig in tbl_gral_arearegion_poligono)
                {
                    coordinatesRegion.Add(new Coordinate(DecimalToSgl_Dbl(ItemRegionPolig.Longitud ?? 0), DecimalToSgl_Dbl(ItemRegionPolig.Latitud ?? 0)));
                }


                polyRegion = new Polygon(coordinatesRegion);

                var Result = polyRegion.Intersection(polyRodal);

                if (Result.Centroid != null)
                {
                    tbl_RNF_Rodal.AreaRegion_Id = ItemRegion.AreaRegion_Id;

                    ViewBag.Region = "Area asociada a la región " + ItemRegion.CodReg + "  Sub_Region: " + ItemRegion.CodSubReg;

                }

            }

            try
            {
                db.Entry(tbl_RNF_Rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }
        public void Evaluacion_AreaProtegidaRodal(string No_Registro, long finca_id, int tipo_area_id, long rodal_id)
        {

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).FirstOrDefault();
            //Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_RNF_Rodal_Poligono> tbl_rnf_rodal_poligono = db.Tbl_RNF_Rodal_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);
            //IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_rnf_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalAreaProtegidaPoligonoEvaluar(" + tbl_RNF_Rodal.GTMX.ToString() + ", " + tbl_RNF_Rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaProtegida> LstAreasProtegidas = new List<Tbl_Gral_AreaProtegida>();

            LstAreasProtegidas = db.Tbl_Gral_AreaProtegida.SqlQuery(Query).ToList();

            var coordinatesAreaProtegida = new List<Coordinate>() { };

            Polygon polyAreaProtegida;

            tbl_RNF_Rodal.AreaProtegida_Id = 0;

            if (LstAreasProtegidas.Count() > 0)
            {
                foreach (var ItemAreaProtegida in LstAreasProtegidas)
                {
                    IEnumerable<Tbl_Gral_AreaProtegida_Poligono> tbl_gral_areaprotegida_poligono = db.Tbl_Gral_AreaProtegida_Poligono.Where(Rodal => Rodal.AreaProtegida_Id == ItemAreaProtegida.AreaProtegida_Id);

                    coordinatesAreaProtegida = new List<Coordinate>() { };

                    foreach (var ItemAreaProtegidaPolig in tbl_gral_areaprotegida_poligono)
                    {
                        coordinatesAreaProtegida.Add(new Coordinate(DecimalToSgl_Dbl(ItemAreaProtegidaPolig.Longitud ?? 0), DecimalToSgl_Dbl(ItemAreaProtegidaPolig.Latitud ?? 0)));
                    }

                    polyAreaProtegida = new Polygon(coordinatesAreaProtegida);

                    var Result = polyAreaProtegida.Intersection(polyRodal);

                    tbl_RNF_Rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                    if (Result.Centroid != null)
                    {
                        tbl_RNF_Rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                        ViewBag.Region = "El rodal esta vinculado a un área protegida ";

                    }

                }
            }
            try
            {
                db.Entry(tbl_RNF_Rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                tbl_RNF_Rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }

            return;
        }
        public void Evaluacion_Colisiones(string No_Registro, long finca_id, int tipo_area_id, long rodal_id)
        {

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).FirstOrDefault();
            //Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_RNF_Rodal_Poligono> tbl_rnf_rodal_poligono = db.Tbl_RNF_Rodal_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);
            //IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_rnf_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            //Colisión de rodales en la misma solicitud

            IEnumerable<Tbl_RNF_Rodal> RodalesRNF = db.Tbl_RNF_Rodal.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Tipo_de_Area == tipo_area_id && (Rodal.Rodal_Id != rodal_id || Rodal.Finca_id != finca_id));
            //IEnumerable<Tbl_Sol_Rodal> RodalesSolicitud = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Tipo_de_Area == tipo_area_id && (Rodal.Rodal_Id != rodal_id || Rodal.Finca_id != finca_id));

            var coordinatesOtrosRodales = new List<Coordinate>() { };
            Polygon polyOtrosRodales;

            foreach (var ItemRodalesSolicitud in RodalesRNF)
            {
                IEnumerable<Tbl_RNF_Rodal_Poligono> tbl_rnf_rodalotros_poligono = db.Tbl_RNF_Rodal_Poligono.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro && Rodal.Finca_id == ItemRodalesSolicitud.Finca_id && Rodal.Tipo_de_Area == ItemRodalesSolicitud.Tipo_de_Area && Rodal.Rodal_Id == ItemRodalesSolicitud.Rodal_Id);
                //IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodalotros_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == ItemRodalesSolicitud.Finca_id && Rodal.Tipo_de_Area == ItemRodalesSolicitud.Tipo_de_Area && Rodal.Rodal_Id == ItemRodalesSolicitud.Rodal_Id);

                coordinatesOtrosRodales = new List<Coordinate>() { };

                foreach (var ItemRodalOtrosPolig in tbl_rnf_rodalotros_poligono)
                {
                    coordinatesOtrosRodales.Add(new Coordinate(DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMX ?? 0), DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMY ?? 0)));
                }

                polyOtrosRodales = new Polygon(coordinatesOtrosRodales);


                var Result = polyOtrosRodales.Intersection(polyRodal);

                if (Result.Centroid != null)
                {
                    if (Result.Area > (polyRodal.Area * 0.001))
                    {
                        ViewBag.Invasion = "El rodal esta colisionando o invadiendo área de otro rodal en el mismo registro.";
                    }
                }
            }

            return;
        }



        public Double DecimalToSgl_Dbl(decimal argument)
        {
            object SingleValue;
            Double DoubleValue;

            // Convert the argument to a float value.
            SingleValue = decimal.ToSingle(argument);

            // Convert the argument to a double value.
            DoubleValue = decimal.ToDouble(argument);

            return DoubleValue;
        }

        public ActionResult UploadExcelAreas(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View();
        }

        public ActionResult UploadExcel(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View();
        }

        public ActionResult UploadExcelAreasDescuento(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View();
        }

        public ActionResult UploadDatosDasometricos(string No_Registro)
        {
            ViewBag.No_Registro = No_Registro;
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadDatosDasometricos(HttpPostedFileBase upload, string No_Registro)
        {
            string columna12, columna13;
            columna12 = columna13 = "";
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

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

            int tipocargaid = 0;
            tipocargaid = 11;
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DASOMETRICOS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                        }


                        //DatoDeCampo = dt.Rows[8][4].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec [SP_Gral_Ins_Carga_RNF] @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        try
                                        {
                                            columna12 = dt.Rows[intContador][11].ToString();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna12 = "0";
                                        }

                                        try
                                        {
                                            columna13 = dt.Rows[intContador][12].ToString();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna13 = "0";
                                        }

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = (dt.Rows[intContador][6]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = (dt.Rows[intContador][7]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = (dt.Rows[intContador][8]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = (dt.Rows[intContador][9]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = (dt.Rows[intContador][10]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = columna12, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = columna13, Direction = System.Data.ParameterDirection.Input},
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


                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                        else
                                        {
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                            }
                        }
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadExcelDescuento(HttpPostedFileBase upload, string No_Registro)
        {
            bool ErrorEncontrado = false;
            TempData["MensajeFileDesc"] = "";

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

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


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            int tipocargaid = 0;
            tipocargaid = 10;

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction("../RNF_Rodal/UploadExcelDescuento", new { No_Registro = No_Registro });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA AREAS DE DESCUENTO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;

                        }


                        //DatoDeCampo = dt.Rows[8][3].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        DatoDeCampo = dt.Rows[6][3].ToString();

                        Tbl_RNF_Finca tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Finca_Id.ToString() == DatoDeCampo).FirstOrDefault();
                        //Tbl_Sol_Finca tbl_sol_finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Finca_Id.ToString() == DatoDeCampo).First();

                        if (tbl_RNF_Finca == null)
                        {
                            ModelState.AddModelError("Carga", "El número de finca no fue encontrado, debe crear la finca.");
                            TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El número de finca no fue encontrado, debe crear la finca. ";
                            ErrorEncontrado = true;
                        }

                        if (!ErrorEncontrado)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga_RNF @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = tbl_RNF_Finca.Finca_Id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
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
                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDesc"] = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../RNF_Rodal/UploadExcelDescuento", new { No_Registro = No_Registro });
                                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDesc"] = resultado[0].mensaje;
                                        }


                                        if (resultado[0].respuesta == 1)
                                        {
                                            var Tbl_RNF_Rodales_Descuento = db.Tbl_RNF_Rodal_Descuento.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();
                                            //var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_RNF_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.No_Registro, ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                                            }

                                            TempData["MensajeFileDesc"] = "Carga realizada con exito.";
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                                        }
                                        else
                                        {
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }


                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                            }
                        }
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadExcel(HttpPostedFileBase upload, string No_Registro)
        {
            bool ErrorEncontrado = false;
            TempData["MensajeFile"] = "";


            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

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

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            int tipocargaid = 0;
            tipocargaid = 9;

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFile"] = TempData["MensajeFile"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA POLÍGONO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;

                        }

                        DatoDeCampo = dt.Rows[7][3].ToString();
                        if (DatoDeCampo != "COORDENADAS GTM X")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;

                        }


                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga_RNF @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 8;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
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
                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFile"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            var Tbl_RNF_Rodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();
                                            //var Tbl_Sol_Rodales = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodal in Tbl_RNF_Rodales)
                                            {
                                                Evaluacion_AreaRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                Evaluacion_RegionRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                Evaluacion_AreaProtegidaRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                //Evaluacion_Colisiones(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                            }


                                            var Tbl_RNF_Rodales_Descuento = db.Tbl_RNF_Rodal_Descuento.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_RNF_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.No_Registro, ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                                            }





                                            //Evaluacion_AreaRodal(1, 1, 1);


                                            TempData["MensajeFile"] = "Carga realizada con exito.";
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                        else
                                        {
                                            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFile"] = "Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                            }
                        }
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFile"] = " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
        }


        class RespuestaCargaArchivos
        {
            public int CodResult { get; set; }
            public string StrMensaje { get; set; }
        }

        [HttpPost]
        public JsonResult UploadExcelRodales(HttpPostedFileBase upload, string No_Registro)
        {
            RespuestaCargaArchivos respuestaCargaArchivos = new RespuestaCargaArchivos();
            respuestaCargaArchivos.CodResult = 0;
            respuestaCargaArchivos.StrMensaje = "";

            bool ErrorEncontrado = false;
            TempData["MensajeFile"] = "";


            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {

                respuestaCargaArchivos.StrMensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                respuestaCargaArchivos.CodResult = 2;
                respuestaCargaArchivos.StrMensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            int tipocargaid = 0;
            tipocargaid = 9;

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        ErrorEncontrado = true;
                        respuestaCargaArchivos.CodResult = 2;
                        respuestaCargaArchivos.StrMensaje = "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.";

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA POLÍGONO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            respuestaCargaArchivos.CodResult = 3;
                            respuestaCargaArchivos.StrMensaje = "El encabezado del archivo no concuerda con el formato solicitado.";
                            ErrorEncontrado = true;

                        }

                        DatoDeCampo = dt.Rows[7][3].ToString();
                        if (DatoDeCampo != "COORDENADAS GTM X")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFile"] = TempData["MensajeFile"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                            respuestaCargaArchivos.CodResult = 3;
                            respuestaCargaArchivos.StrMensaje = "El encabezado del archivo no concuerda con el formato solicitado.";

                        }


                        if (!ErrorEncontrado)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga_RNF @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 8;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
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
                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                        respuestaCargaArchivos.CodResult = 4;
                                        respuestaCargaArchivos.StrMensaje = "Error: La cantidad de muestras es muy pequeña.";
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFile"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            var Tbl_RNF_Rodales = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();
                                            //var Tbl_Sol_Rodales = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodal in Tbl_RNF_Rodales)
                                            {
                                                Evaluacion_AreaRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                Evaluacion_RegionRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                Evaluacion_AreaProtegidaRodal(ItemRodal.No_Registro, ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                                //Evaluacion_Colisiones(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                            }


                                            var Tbl_RNF_Rodales_Descuento = db.Tbl_RNF_Rodal_Descuento.Where(Rodal => Rodal.No_Registro == tbl_RNF_Registro.No_Registro).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_RNF_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.No_Registro, ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                                            }





                                            //Evaluacion_AreaRodal(1, 1, 1);


                                            TempData["MensajeFile"] = "Carga realizada con exito.";
                                            respuestaCargaArchivos.CodResult = 1;
                                            respuestaCargaArchivos.StrMensaje = "Carga realizada con exito.";
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFile"] = "Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                respuestaCargaArchivos.CodResult = 5;
                                respuestaCargaArchivos.StrMensaje = "Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";

                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        respuestaCargaArchivos.CodResult = 6;
                        respuestaCargaArchivos.StrMensaje = "El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        TempData["MensajeFile"] = " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return Json(JsonConvert.SerializeObject(respuestaCargaArchivos));
        }

        [HttpPost]
        public JsonResult UploadExcelRodalesDescuento(HttpPostedFileBase upload, string No_Registro)
        {
            RespuestaCargaArchivos respuestaCargaArchivos = new RespuestaCargaArchivos();
            respuestaCargaArchivos.CodResult = 0;
            respuestaCargaArchivos.StrMensaje = "";

            bool ErrorEncontrado = false;
            TempData["MensajeFileDesc"] = "";

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                respuestaCargaArchivos.CodResult = 2;
                respuestaCargaArchivos.StrMensaje = "Error: El estatus del registro no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                respuestaCargaArchivos.CodResult = 2;
                respuestaCargaArchivos.StrMensaje = objSesion.getStrMensaje();
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }


            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            int tipocargaid = 0;
            tipocargaid = 10;

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        respuestaCargaArchivos.CodResult = 3;
                        respuestaCargaArchivos.StrMensaje = "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.";

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE COORDENADAS PARA AREAS DE DESCUENTO")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                            respuestaCargaArchivos.CodResult = 4;
                            respuestaCargaArchivos.StrMensaje = "El encabezado del archivo no concuerda con el formato solicitado.";

                        }


                        //DatoDeCampo = dt.Rows[8][3].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        //DatoDeCampo = dt.Rows[6][3].ToString();

                        //Tbl_RNF_Finca tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Finca_Id.ToString() == DatoDeCampo).FirstOrDefault();
                        ////Tbl_Sol_Finca tbl_sol_finca = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Finca_Id.ToString() == DatoDeCampo).First();

                        //if (tbl_RNF_Finca == null)
                        //{
                        //    ModelState.AddModelError("Carga", "El número de finca no fue encontrado, debe crear la finca.");
                        //    TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El número de finca no fue encontrado, debe crear la finca. ";
                        //    respuestaCargaArchivos.CodResult = 5;
                        //    respuestaCargaArchivos.StrMensaje = "El número de finca no fue encontrado, debe crear la finca.";
                        //    ErrorEncontrado = true;
                        //}

                        if (!ErrorEncontrado)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga_RNF @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = 0, Direction = System.Data.ParameterDirection.Input},
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
                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDesc"] = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../RNF_Rodal/UploadExcelDescuento", new { No_Registro = No_Registro });
                                        respuestaCargaArchivos.CodResult = 6;
                                        respuestaCargaArchivos.StrMensaje = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDesc"] = resultado[0].mensaje;
                                        }


                                        if (resultado[0].respuesta == 1)
                                        {
                                            var Tbl_RNF_Rodales_Descuento = db.Tbl_RNF_Rodal_Descuento.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).ToList();
                                            //var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_RNF_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.No_Registro, ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id);
                                            }

                                            TempData["MensajeFileDesc"] = "Carga realizada con exito.";
                                            respuestaCargaArchivos.CodResult = 1;
                                            respuestaCargaArchivos.StrMensaje = "Carga realizada con exito.";
                                            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                                        }

                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                respuestaCargaArchivos.CodResult = 7;
                                respuestaCargaArchivos.StrMensaje = "Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                            }
                        }
                        //return RedirectToAction("../Sol_Rodal/UploadExcel");
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        respuestaCargaArchivos.CodResult = 8;
                        respuestaCargaArchivos.StrMensaje = "El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
            return Json(JsonConvert.SerializeObject(respuestaCargaArchivos));
        }

        [HttpPost]
        public JsonResult UploadExcelDatosDasometricos(HttpPostedFileBase upload, string No_Registro)
        {
            RespuestaCargaArchivos respuestaCargaArchivos = new RespuestaCargaArchivos();
            respuestaCargaArchivos.CodResult = 0;
            respuestaCargaArchivos.StrMensaje = "";

            string columna12, columna13;
            columna12 = columna13 = "";
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                respuestaCargaArchivos.CodResult = 2;
                respuestaCargaArchivos.StrMensaje = objSesion.getStrMensaje();
                //return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int tipocargaid = 0;
            tipocargaid = 11;
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        respuestaCargaArchivos.CodResult = 3;
                        respuestaCargaArchivos.StrMensaje = "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.";
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DASOMETRICOS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                            respuestaCargaArchivos.CodResult = 4;
                            respuestaCargaArchivos.StrMensaje = "El encabezado del archivo no concuerda con el formato solicitado.";
                        }


                        //DatoDeCampo = dt.Rows[8][4].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec [SP_Gral_Ins_Carga_RNF] @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        try
                                        {
                                            columna12 = (dt.Rows[intContador][11] ?? "").ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna12 = "0";
                                        }

                                        try
                                        {
                                            columna13 = (dt.Rows[intContador][12] ?? "").ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna13 = "0";
                                        }

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = (dt.Rows[intContador][6]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = (dt.Rows[intContador][7]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = (dt.Rows[intContador][8]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = (dt.Rows[intContador][9]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = (dt.Rows[intContador][10]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = columna12, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = columna13, Direction = System.Data.ParameterDirection.Input},
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


                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        respuestaCargaArchivos.CodResult = 5;
                                        respuestaCargaArchivos.StrMensaje = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                            respuestaCargaArchivos.CodResult = 6;
                                            respuestaCargaArchivos.StrMensaje = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            respuestaCargaArchivos.CodResult = 1;
                                            respuestaCargaArchivos.StrMensaje = "Carga realizada con exito.";
                                            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                respuestaCargaArchivos.CodResult = 7;
                                respuestaCargaArchivos.StrMensaje = "Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                            }
                        }
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        respuestaCargaArchivos.CodResult = 8;
                        respuestaCargaArchivos.StrMensaje = "El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

            return Json(JsonConvert.SerializeObject(respuestaCargaArchivos));
        }

        public ActionResult Borrar(string No_Registro, long varFinca_id, int varTipoArea, long varRodalid)
        {
            int intDosometricos = db.Tbl_RNF_Rodal_Dasometrico.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == varFinca_id && Obj.Tipo_de_Area == varTipoArea && Obj.Rodal_id == varRodalid).Count();


            if (intDosometricos > 0)
            {
                return RedirectToAction("../Sol_Rodal_Inventario/BorradoFallido");
            }


            IEnumerable<Tbl_RNF_Rodal_Poligono> poligonoList = db.Tbl_RNF_Rodal_Poligono.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == varFinca_id && Obj.Tipo_de_Area == varTipoArea && Obj.Rodal_Id == varRodalid);
            //Now you have everything you need.
            foreach (var Item in poligonoList)
            {
                //Remove
                db.Tbl_RNF_Rodal_Poligono.Remove(Item);
            }

            db.SaveChanges();



            Tbl_RNF_Rodal tbl_RNF_Rodal = db.Tbl_RNF_Rodal.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == varFinca_id && Obj.Tipo_de_Area == varTipoArea && Obj.Rodal_Id == varRodalid).First();
            db.Tbl_RNF_Rodal.Remove(tbl_RNF_Rodal);
            db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");

        }

        public ActionResult BorrarDescuento(string No_Registro, long varFinca_id, int varTipoArea, long varRodalid, long varAreaDescuento_id)
        {
            IEnumerable<Tbl_RNF_Rodal_Descuento_Poligono> poligonoDescuentoList = db.Tbl_RNF_Rodal_Descuento_Poligono.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == varFinca_id && Obj.Tipo_de_Area == varTipoArea && Obj.Rodal_Id == varRodalid && Obj.Rodal_Descuento_Id == varAreaDescuento_id);

            //Now you have everything you need.
            foreach (var Item in poligonoDescuentoList)
            {
                //Remove
                db.Tbl_RNF_Rodal_Descuento_Poligono.Remove(Item);
            }

            db.SaveChanges();


            Tbl_RNF_Rodal_Descuento tbl_RNF_Rodal_Descuento = db.Tbl_RNF_Rodal_Descuento.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == varFinca_id && Obj.Tipo_de_Area == varTipoArea && Obj.Rodal_Id == varRodalid && Obj.Rodal_Descuento_Id == varAreaDescuento_id).FirstOrDefault();
            db.Tbl_RNF_Rodal_Descuento.Remove(tbl_RNF_Rodal_Descuento);
            db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");

        }

        [HttpPost]
        public JsonResult UploadExcelDatosDasometricos2(HttpPostedFileBase upload, string No_Registro)
        {
            RespuestaCargaArchivos respuestaCargaArchivos = new RespuestaCargaArchivos();
            respuestaCargaArchivos.CodResult = 0;
            respuestaCargaArchivos.StrMensaje = "";

            string columna12, columna13;
            columna12 = columna13 = "";
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];

            if (!objRNFGrants.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();
                respuestaCargaArchivos.CodResult = 2;
                respuestaCargaArchivos.StrMensaje = objSesion.getStrMensaje();
                //return RedirectToAction("../Login/Index");
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            int tipocargaid = 0;
            tipocargaid = 11;
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {

                        ModelState.AddModelError("Archivo", "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.");
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        respuestaCargaArchivos.CodResult = 3;
                        respuestaCargaArchivos.StrMensaje = "El formato de archivo no es soportado. Unicamente archivos de Excel son soportados.";
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DASOMETRICOS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                            respuestaCargaArchivos.CodResult = 4;
                            respuestaCargaArchivos.StrMensaje = "El encabezado del archivo no concuerda con el formato solicitado.";
                        }


                        //DatoDeCampo = dt.Rows[8][4].ToString();

                        //if (DatoDeCampo != tbl_sol_solicitud.Solicitud_id.ToString())
                        //{
                        //    ModelState.AddModelError("Carga", "El número de solicitud no concuerda con el esperado.");
                        //    TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El número de solicitud no concuerda con el esperado. ";
                        //    ErrorEncontrado = true;
                        //}

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec [SP_Gral_Ins_Carga_RNF] @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno, @No_Registro, @No_RegistroLiteral, @No_RegistroCorrelativo";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_RNF_Registro.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_Registro",  Value = tbl_RNF_Registro.No_Registro, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroLiteral",  Value = tbl_RNF_Registro.No_RegistroLiteral, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@No_RegistroCorrelativo",  Value = tbl_RNF_Registro.No_RegistroCorrelativo, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {
                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle2 @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07";

                                        try
                                        {
                                            columna12 = (dt.Rows[intContador][11] ?? "").ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna12 = "0";
                                        }

                                        try
                                        {
                                            columna13 = (dt.Rows[intContador][12] ?? "").ToString().Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            columna13 = "0";
                                        }

                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = (dt.Rows[intContador][6]??"").ToString().Trim(), Direction = System.Data.ParameterDirection.Input}
                                                 
                                           };


                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        intContador = intContador + 1;
                                    }


                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 4)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        respuestaCargaArchivos.CodResult = 5;
                                        respuestaCargaArchivos.StrMensaje = "Error: La cantidad de muestras es muy pequeña.";
                                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga2 @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = tipocargaid, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                            respuestaCargaArchivos.CodResult = 6;
                                            respuestaCargaArchivos.StrMensaje = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            respuestaCargaArchivos.CodResult = 1;
                                            respuestaCargaArchivos.StrMensaje = "Carga realizada con exito.";
                                            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                respuestaCargaArchivos.CodResult = 7;
                                respuestaCargaArchivos.StrMensaje = "Error desconocido en la carga de archivos Ref:RNF_RodalController_001001.";
                                //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                            }
                        }
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        respuestaCargaArchivos.CodResult = 8;
                        respuestaCargaArchivos.StrMensaje = "El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            //return RedirectToAction("../RNF_Rodal/UploadExcel", new { No_Registro = No_Registro });

            return Json(JsonConvert.SerializeObject(respuestaCargaArchivos));
        }


    }
}
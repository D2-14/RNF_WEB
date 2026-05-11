using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotSpatial.Topology;
using ExcelDataReader;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Sol_RodalController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();
        private SqlParameter[] sqlParams;

        // GET: Sol_Rodal
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

            var tbl_sol_rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == solicitud_id);
            
            ViewBag.Inconvenientes = db.fc_Sol_Sel_Revision(solicitud_id).Where(Obj=>Obj.Bloqueante==true);


            ViewBag.solicitud_id = tbl_sol_solicitud.Solicitud_id;

            ViewBag.guidid = tbl_sol_solicitud.Guid_id;

            return View(tbl_sol_rodal.ToList());
        }

        public ActionResult IndexDescuento(long solicitud_id, string firma)
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

            var tbl_sol_rodal = db.Tbl_Sol_Rodal_Descuento.Where(Obj => Obj.Solicitud_id == solicitud_id);

            ViewBag.guidid = tbl_sol_solicitud.Guid_id;

            return View(tbl_sol_rodal.ToList());

        }

        public ActionResult CentroParcela(long Solicitud_id, string firma)
        {
            List<Tbl_Sol_Rodal_Dasometrico_CentroParcela> tbl_Sol_Rodal_Dasometrico_CentroParcelas = db.Tbl_Sol_Rodal_Dasometrico_CentroParcela.Where(Obj => Obj.Solicitud_id == Solicitud_id).ToList();
            ViewBag.CantidadCentrosParcela = tbl_Sol_Rodal_Dasometrico_CentroParcelas.Count();
            return View(tbl_Sol_Rodal_Dasometrico_CentroParcelas);
        }

        public JsonResult SetArea(int Rodal_id, decimal decimal_area, long solicitud_id, string firma)
        {
            if (Rodal_id == 0)
            {
                return Json("");
            }

            if (decimal_area == 0)
            {
                return Json("");
            }

            long lngSolicitud = solicitud_id;

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == lngSolicitud && Rodal.Rodal_Id == Rodal_id).First();

            sol_sol_rodal.AreaTotalCalculadaSistema = decimal_area;

            db.Entry(sol_sol_rodal).State = EntityState.Modified;
            db.SaveChanges();

            return Json("");

        }

        // GET: Sol_Rodal/Create
        public ActionResult Create(string mensajerodal, long solicitud_id, string firma)
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


            Tbl_Sol_Rodal sol_sol_rodal = new Tbl_Sol_Rodal();

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(u => u.Rodal_Id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }

            sol_sol_rodal.Rodal_Id = lngIdt;

            sol_sol_rodal.Solicitud_id = tbl_sol_solicitud.Solicitud_id;

            sol_sol_rodal.AreaProtegida_Id = 0;

            sol_sol_rodal.swcreatedby = objUs.intUsuario_id;
            sol_sol_rodal.swdatecreated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                sol_sol_rodal.swcreatedbyinterno = false;
            }
            else
            {
                sol_sol_rodal.swcreatedbyinterno = true;
            }

            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj=> Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", 1);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);

            sol_sol_rodal.Colecta_De_Datos_X_Censo = true;

            sol_sol_rodal.Longitud_Total = sol_sol_rodal.Longitud_Total ?? 0;


            ViewBag.mensajerodal = mensajerodal;

            return View(sol_sol_rodal);

        }

        // POST: Sol_Rodal/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Sol_Rodal tbl_Sol_Rodal, long solicitud_id, string firma)
        {

            long lngIdt = 0;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            if (tbl_Sol_Rodal.Area_SIGAP == false)
            {
                tbl_Sol_Rodal.CategoriaSIGAP_Id = 0;
            }

            try
            {
                lngIdt = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).Max(u => u.Rodal_Id);
                lngIdt++;
            }
            catch
            {
                lngIdt = 1;
            }

            tbl_Sol_Rodal.Rodal_Id = lngIdt;

            try
            {

               if (ModelState.IsValid)
               {
                    db.Tbl_Sol_Rodal.Add(tbl_Sol_Rodal);
                    db.SaveChanges();
                    return RedirectToAction("../Sol_Rodal/Create", new { mensajerodal  = "Ingrese los datos del siguiente rodal", solicitud_id = solicitud_id,  firma = firma});
               }

            }
            catch
            {

                ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", 1);

                ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);


                return View(tbl_Sol_Rodal);
            }



            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", 1);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);



            return View(tbl_Sol_Rodal);
        }


        //// GET: Sol_Rodal/Create
        //public ActionResult Edit(long solicitud_id, string firma)
        //{

        //    Usuario objUs = new Usuario();
        //    objUs.intUsuario_id = 0;

        //    RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
        //    if (!objSesion.getBlSession())
        //    {
        //        ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

        //        ViewBag.Mensaje = objSesion.getStrMensaje();
        //        return RedirectToAction("../Login/Index");
        //    }
        //    else
        //    {
        //        objUs = (Usuario)Session["User"];
        //    }


        //    Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);

        //    Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal=> Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Rodal_Id == id).First();

        //    ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", sol_sol_rodal.Tipo_de_Area);

        //    ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", sol_sol_rodal.CategoriaSIGAP_Id);

        //    return View(sol_sol_rodal);

        //}


        public void Evaluacion_AreaRodalDescuento(long finca_id, int tipo_area_id, long rodal_id, long rodal_descuento_id, long solicitud_id)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal_Descuento sol_sol_rodal = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id).First();

            IEnumerable<Tbl_Sol_Rodal_Descuento_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Descuento_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id && Rodal.Rodal_Descuento_Id == rodal_descuento_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;

            sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
            sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;


            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;

        }

        public void Evaluacion_AreaRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
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


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };
            int IsPrimera = 0;
            decimal PrimeraX = (Decimal) 0.000;
            decimal PrimeraY = (Decimal) 0.000;

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                if (IsPrimera == 0)
                {
                    PrimeraX = (Decimal) Item.GTMX;
                    PrimeraY = (Decimal) Item.GTMY;


                }
                IsPrimera = IsPrimera + 1;
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            sol_sol_rodal.AreaTotalCalculadaSistema = (decimal)(polyRodal.Area / 10000);
            sol_sol_rodal.AreaTotal = sol_sol_rodal.AreaTotalCalculadaSistema;

            if (sol_sol_rodal.Tipo_de_Area == 2)
            {
                sol_sol_rodal.AreaTotalCalculadaSistema = 0;
                sol_sol_rodal.AreaTotal = 0;
                sol_sol_rodal.GTMX = PrimeraX;
                sol_sol_rodal.GTMY = PrimeraY;
            }
            else
            {
                sol_sol_rodal.GTMX = (decimal)polyRodal.Centroid.X;
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }


            sol_sol_rodal.Longitud_Total = sol_sol_rodal.Longitud_Total ?? 0;



 
            Tbl_Sol_Finca Tbl_Sol_FincaUpdate = db.Tbl_Sol_Finca.Where(Obj=> Obj.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Obj.Finca_Id == finca_id).First();

            Tbl_Sol_FincaUpdate.GTMX = sol_sol_rodal.GTMX;
            Tbl_Sol_FincaUpdate.GTMY = sol_sol_rodal.GTMY;
            try
            {
                if ((Tbl_Sol_FincaUpdate.ConstanciaDePorpiedad_id ??0) == 0)
                {
                    Tbl_Sol_FincaUpdate.ConstanciaDePorpiedad_id =  1;
                    Tbl_Sol_FincaUpdate.RegDepartamento_id = 1;
                }
                db.Entry(Tbl_Sol_FincaUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }



            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }


        public void Evaluacion_RegionRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalRegionPoligonoEvaluar(" + sol_sol_rodal.GTMX.ToString() + ", " + sol_sol_rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaRegion> LstRegiones = new List<Tbl_Gral_AreaRegion>();

            LstRegiones = db.Tbl_Gral_AreaRegion.SqlQuery(Query).ToList();

            var coordinatesRegion = new List<Coordinate>() { };

            Polygon polyRegion;

            sol_sol_rodal.AreaRegion_Id = 0;

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
                    sol_sol_rodal.AreaRegion_Id = ItemRegion.AreaRegion_Id;

                    ViewBag.Region = "Area asociada a la región " + ItemRegion.CodReg + "  Sub_Region: " + ItemRegion.CodSubReg;

                }

            }

            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }
            return;
        }

        public void Evaluacion_AreaProtegidaRodal(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }


            Polygon polyRodal = new Polygon(coordinatesRodal);

            String Query;

            Query = "Select * ";
            Query += "from fc_Sol_Sel_RodalAreaProtegidaPoligonoEvaluar(" + sol_sol_rodal.GTMX.ToString() + ", " + sol_sol_rodal.GTMY.ToString() + ") ";

            List<Tbl_Gral_AreaProtegida> LstAreasProtegidas = new List<Tbl_Gral_AreaProtegida>();

            LstAreasProtegidas = db.Tbl_Gral_AreaProtegida.SqlQuery(Query).ToList();

            var coordinatesAreaProtegida = new List<Coordinate>() { };

            Polygon polyAreaProtegida;

            sol_sol_rodal.AreaProtegida_Id = 0;

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

                    sol_sol_rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                    if (Result.Centroid != null)
                    {
                        sol_sol_rodal.AreaProtegida_Id = ItemAreaProtegida.AreaProtegida_Id;

                        ViewBag.Region = "El rodal esta vinculado a un área protegida ";

                    }

                }
            }
            try
            {
                db.Entry(sol_sol_rodal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exeption)
            {
                sol_sol_rodal.GTMY = (decimal)polyRodal.Centroid.Y;
            }

            return;
        }


        public void Evaluacion_Colisiones(long finca_id, int tipo_area_id, long rodal_id, long solicitud_id, string firma)
        {

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Sol_Rodal sol_sol_rodal = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id).First();

            IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodal_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == finca_id && Rodal.Tipo_de_Area == tipo_area_id && Rodal.Rodal_Id == rodal_id);

            var coordinatesRodal = new List<Coordinate>() { };

            foreach (var Item in tbl_sol_rodal_poligono)
            {
                coordinatesRodal.Add(new Coordinate(DecimalToSgl_Dbl(Item.GTMX ?? 0), DecimalToSgl_Dbl(Item.GTMY ?? 0)));
            }

            Polygon polyRodal = new Polygon(coordinatesRodal);

            //Colisión de rodales en la misma solicitud

            IEnumerable<Tbl_Sol_Rodal> RodalesSolicitud = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Tipo_de_Area == tipo_area_id && (Rodal.Rodal_Id != rodal_id || Rodal.Finca_id != finca_id ));

            var coordinatesOtrosRodales = new List<Coordinate>() { };
            Polygon polyOtrosRodales;

            foreach (var ItemRodalesSolicitud in RodalesSolicitud)
            {
                IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodalotros_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Finca_id == ItemRodalesSolicitud.Finca_id && Rodal.Tipo_de_Area == ItemRodalesSolicitud.Tipo_de_Area && Rodal.Rodal_Id == ItemRodalesSolicitud.Rodal_Id);

                coordinatesOtrosRodales = new List<Coordinate>() { };

                foreach (var ItemRodalOtrosPolig in tbl_sol_rodalotros_poligono)
                {
                    coordinatesOtrosRodales.Add(new Coordinate(DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMX ?? 0), DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMY ?? 0)));
                }

                polyOtrosRodales = new Polygon(coordinatesOtrosRodales);


                var Result = polyOtrosRodales.Intersection(polyRodal);

                if (Result.Centroid != null)
                {
                    if (Result.Area > (polyRodal.Area * 0.001))
                    {
                        ViewBag.Invasion = "El rodal esta colisionando o invadiendo área de otro rodal en la misma solicitud.";
                    }
                }
            }

            return;
        }

        ////Colisión de rodales en la misma solicitud

        //IEnumerable<Tbl_Sol_Rodal> RodalesSolicitud = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Rodal_Id != Rodal_id);

        //var coordinatesOtrosRodales = new List<Coordinate>() { };
        //Polygon polyOtrosRodales;

        //foreach (var ItemRodalesSolicitud in RodalesSolicitud)
        //{
        //    IEnumerable<Tbl_Sol_Rodal_Poligono> tbl_sol_rodalotros_poligono = db.Tbl_Sol_Rodal_Poligono.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id && Rodal.Rodal_Id == ItemRodalesSolicitud.Rodal_Id && Rodal.Estado_id == true );

        //    coordinatesOtrosRodales = new List<Coordinate>() { };

        //    foreach (var ItemRodalOtrosPolig in tbl_sol_rodalotros_poligono)
        //    {
        //        coordinatesOtrosRodales.Add(new Coordinate(DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMX ?? 0), DecimalToSgl_Dbl(ItemRodalOtrosPolig.GTMY ?? 0)));
        //    }

        //    polyOtrosRodales = new Polygon(coordinatesOtrosRodales);


        //    var Result = polyOtrosRodales.Intersection(polyRodal);

        //    if (Result.Centroid != null)
        //    {
        //        if (Result.Area> (polyRodal.Area*0.001))
        //        { 
        //            ViewBag.Invasion = "El rodal esta colisionando o invadiendo área de otro rodal en la misma solicitud.";
        //        }
        //    }
        //}





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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tbl_Sol_Rodal tbl_Sol_Rodal)
        {
            if (tbl_Sol_Rodal.Area_SIGAP == false)
            {
                tbl_Sol_Rodal.CategoriaSIGAP_Id = 0;
            }

            if (ModelState.IsValid)
            {
                db.Entry(tbl_Sol_Rodal).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Mensaje = "Ultima actualizacion : " + DateTime.Now.ToString();

                ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", tbl_Sol_Rodal.Tipo_de_Area);

                ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_Sol_Rodal.CategoriaSIGAP_Id);


                return View(tbl_Sol_Rodal);
            }

            ViewBag.Mensaje = "Error: No se pudo actualizar." + DateTime.Now.ToString();

            ViewBag.Tipo_de_Area = new SelectList(db.Tbl_Sol_Rodal_Tipo.Where(Obj => Obj.Tipo_de_Area > 0), "Tipo_de_Area", "Descripcion", tbl_Sol_Rodal.Tipo_de_Area);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_Sol_Rodal.CategoriaSIGAP_Id);

            return View(tbl_Sol_Rodal);
        }

        public ActionResult UploadExcelAreas(long solicitud_id, string firma)
        {

            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;


            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            return View();
        }

        public ActionResult UploadExcel(long solicitud_id, string firma)
        {
            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            return View();
        }

        public ActionResult UploadExcelDasometrico(long solicitud_id, string firma)
        {
            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            return View();
        }

        public ActionResult UploadExcelAreasDescuento(long solicitud_id, string firma)
        {
            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            return View();
        }

        public ActionResult UploadDatosDasometricos(long solicitud_id, string firma)
        {
            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            return View();
        }

        public ActionResult UploadCentroDeParcela(long solicitud_id, string firma)
        {


            ViewBag.Solicitud_id = solicitud_id;
            ViewBag.firma = firma;

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            return View();
        }


        [HttpPost]
        public ActionResult UploadExcelCentroParcelas(HttpPostedFileBase upload, long Solicitud_id, string firma)
        {
            Session[Constants.session_Tabulador] = "defaultParcela";

            bool ErrorEncontrado = false;
            int intTipoCarga = 12;
            TempData["MensajeFileCentroParcela"] = "";

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }



            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Rodal_Dasometricos", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de las parcelas";
                ErrorEncontrado = true;
            }


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
                        TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                        ErrorEncontrado = true;
                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                    }

                    try
                    {

                        DataSet datDatosExcel = reader.AsDataSet();

                        DataTable dt = datDatosExcel.Tables[0];

                        string DatoDeCampo = dt.Rows[1][0].ToString();

                        if (DatoDeCampo != "CARGA DE DATOS DE CENTRO DE PARCELAS")
                        {
                            ModelState.AddModelError("Carga", "El encabezado del archivo no concuerda con el formato solicitado.");
                            TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El encabezado del archivo no concuerda con el formato solicitado. ";
                            ErrorEncontrado = true;
                        }

                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            string @strCampo12 = "0";
                            string @strCampo13 = "0";

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
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
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


                                    sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                    sqlParams = new SqlParameter[]
                                   {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFileCentroParcela"] = "Error: La cantidad de muestras es muy pequeña.";
                                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipoCarga, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileCentroParcela"] = resultado[0].mensaje;
                                        }

                                        if (resultado[0].respuesta == 1)
                                        {
                                            TempData["MensajeFileCentroParcela"] = "Carga realizada con exito.";
                                            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                        else
                                        {
                                            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                            }
                        }
                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileCentroParcela"] = TempData["MensajeFileCentroParcela"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });


                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadDatosDasometricos(HttpPostedFileBase upload, long Solicitud_id, string firma)
        {
            Session[Constants.session_Tabulador] = "defaultDasometricos";
            int intTipo_Carga_id = 5;
            bool ErrorEncontrado = false;
            TempData["MensajeFileDasom"] = "";

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

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Rodal_Dasometricos", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }


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
                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { Solicitud_id = Solicitud_id, firma = firma });

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
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
                           };

                            List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                           { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                            resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                            string @strCampo12 = "0";
                            string @strCampo13 = "0";
                            int claseCat6 = 0;

                            //Cargar detalle
                            if (resultado[0].respuesta == 1)
                            {
                                lnCarga_id = resultado[0].id;
                                intContador = 10;
                                try
                                {

                                    if (((dt.Rows[intContador][0] == null) || (dt.Rows[intContador][0].ToString() == "")) && (intContador == 10))
                                    {
                                        TempData["MensajeFileDasom"] = "Error: No se ha encontrado el número de finca en la línea " + (intContador + 1).ToString() + ".";
                                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                    }


                                    while (dt.Rows[intContador][0].ToString() != "")
                                    {
                                        sqlQuery = "Exec SP_Gral_Ins_CargaDetalle @Tipo_Carga_id, @Carga_id, @Campo01, @Campo02, @Campo03, @Campo04, @Campo05, @Campo06, @Campo07, @Campo08, @Campo09, @Campo10, @Campo11, @Campo12, @Campo13, @Campo14, @Campo15, @Campo16, @Campo17, @Campo18, @Campo19, @Campo20";

                                        if ((dt.Rows[intContador][1] == null) || (dt.Rows[intContador][1].ToString() == ""))
                                        {
                                            TempData["MensajeFileDasom"] = "Error: No se ha encontrado el número de rodal en la línea " + (intContador + 1).ToString() + ".";
                                            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                        }


                                        if (tbl_sol_solicitud.Categoria_id == 6)
                                        {
                                            try
                                            {
                                                @strCampo12 = (dt.Rows[intContador][11] ?? "").ToString().Trim();
                                            }
                                            catch (Exception Ex)
                                            {
                                                @strCampo12 = "0";
                                            }


                                            claseCat6 = int.Parse(@strCampo12);

                                            if (claseCat6 > 3)
                                            {
                                                TempData["MensajeFileDasom"] = "Error: La clase es mayor a 3 en la linea " + (intContador + 1).ToString() + ".";
                                                return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                            }

                                            try
                                            {
                                                @strCampo13 = (dt.Rows[intContador][12] ?? "").ToString().Trim();
                                            }
                                            catch (Exception Ex)
                                            {
                                                @strCampo13 = "0";
                                            }
                                        }


                                        sqlParams = new SqlParameter[]
                                           {
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo06",  Value = (dt.Rows[intContador][5] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo07",  Value = (dt.Rows[intContador][6] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo08",  Value = (dt.Rows[intContador][7] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo09",  Value = (dt.Rows[intContador][8] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo10",  Value = (dt.Rows[intContador][9] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo11",  Value = (dt.Rows[intContador][10] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo12",  Value = @strCampo12, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo13",  Value = @strCampo13, Direction = System.Data.ParameterDirection.Input},

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
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                   };


                                }
                                catch (Exception ex)
                                {
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFileDasom"] = "Error: La cantidad de muestras es muy pequeña.";
                                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        db.Database.CommandTimeout = 3000;

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 0)
                                        {
                                            TempData["MensajeFileDasom"] = resultado[0].mensaje;
                                            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" ;
                                        }

                                        if (resultado[0].respuesta != 0)
                                        {
                                            TempData["MensajeFileDasom"] = "Carga realizada con exito.";
                                            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                        else
                                        {

                                            Tbl_Gral_Carga tbl_Gral_Carga = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == intTipo_Carga_id & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                                            if (tbl_Gral_Carga != null)
                                            {
                                                if (resultado[0].mensaje != null)
                                                {
                                                    TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" + "¡Error Inesperado!  " + resultado[0].mensaje  ?? "";
                                                }
                                                else
                                                {

                                                    TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>" + "¡Error Inesperado!  "  + tbl_Gral_Carga.Observaciones ?? "";
                                                }
                                            }
                                            else
                                            {
                                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                                            }

                                            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                    }
                                }

                            }
                            else
                            {

                                TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";

                                return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                            }
                        }
                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";

                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDasom"] = TempData["MensajeFileDasom"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
                        return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            TempData["MensajeFileDasom"] = "<table><tr><td>" + TempData["MensajeFileDasom"] + "</td></tr></table>";
            return RedirectToAction("../Sol_Rodal/UploadExcelDasometrico", new { solicitud_id = Solicitud_id, firma = firma });
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadExcelDescuento(HttpPostedFileBase upload, long Solicitud_id, string firma)
        {
            Session[Constants.session_Tabulador] = "defaultDescuento";

            bool ErrorEncontrado = false;
            TempData["MensajeFileDesc"] = "";

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
            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }
            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Rodal_Poligono", boolEsInterno).FirstOrDefault();

            if (!(bool)permisos.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }




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
                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

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


                        if (ErrorEncontrado == false)
                        {
                            string sqlQuery;
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            long lnCarga_id;

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 4, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
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
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 4, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
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
                                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                        {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = 4, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                        };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        {
                                            TempData["MensajeFileDesc"] = resultado[0].mensaje;
                                        }


                                        if (resultado[0].respuesta == 1)
                                        {

                                            var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id, Solicitud_id);
                                            }

                                            TempData["MensajeFileDesc"] = "Carga realizada con exito.";
                                            return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

                                        }
                                        else
                                        {
                                            return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                                        }


                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                            }
                        }
                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFileDesc"] = TempData["MensajeFileDesc"] + " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                }
            }
            return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadExcelAreas(HttpPostedFileBase upload, long Solicitud_id, string firma)
        {

            Session[Constants.session_Tabulador] = "defaultArea";


            long lnCarga_id = 0;
            int intTipo_Carga_id = 1;
            bool ErrorEncontrado = false;
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

            bool boolEsInterno = false;
            if (objUs.EsInterno == 1)
            {
                boolEsInterno = true;
            }

            Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Solicitud_id);

            if (tbl_sol_solicitud == null)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }
            if (tbl_sol_solicitud.Guid_id != firma)
            {
                return RedirectToAction("../Home/AccesoDenegado");
            }


            fc_Gral_Sol_Configuracion_Result permisos = db.fc_Gral_Sol_Configuracion(tbl_sol_solicitud.Solicitud_id, "Sol_Rodal", boolEsInterno).FirstOrDefault();


            if (!(bool)permisos.Editar)
            {
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar datos de los rodales";
                ErrorEncontrado = true;
            }

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
                         TempData["MensajeFile"] = TempData["MensajeFile"] +  " El formato de archivo no es soportado. Unicamente archivos de Excel son soportados. ";
                         ErrorEncontrado = true;
                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

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
                            sqlQuery = "Exec SP_Gral_Ins_Carga @Tipo_Carga_id, @Solicitud_id, @Finca_id, @Rodal_id, @Usuario_id, @EsInterno";
                            SqlParameter[] sqlParams;
                            int intContador;
                            

                            sqlParams = new SqlParameter[]
                           {
                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                             new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_sol_solicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Finca_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Rodal_id",  Value = 0, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@Usuario_id",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
                             new SqlParameter { ParameterName = "@EsInterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input}
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
                                                 new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                                                 new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo01",  Value = (dt.Rows[intContador][0] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo02",  Value = (dt.Rows[intContador][1] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo03",  Value = (dt.Rows[intContador][2] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo04",  Value = (dt.Rows[intContador][3] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
                                                 new SqlParameter { ParameterName = "@Campo05",  Value = (dt.Rows[intContador][4] ?? "").ToString().Trim(), Direction = System.Data.ParameterDirection.Input},
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
                                    if (intContador < 1)
                                    {
                                        TempData["MensajeFile"] = "Error: La cantidad de muestras es muy pequeña.";
                                        return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                                    }
                                    else
                                    {
                                        sqlQuery = "Exec SP_Gral_Proc_Carga @Tipo_Carga_id, @Carga_id";

                                        sqlParams = new SqlParameter[]
                                       {
                                             new SqlParameter { ParameterName = "@Tipo_Carga_id",  Value = intTipo_Carga_id, Direction = System.Data.ParameterDirection.Input },
                                             new SqlParameter { ParameterName = "@Carga_id",  Value = lnCarga_id, Direction = System.Data.ParameterDirection.Input}
                                       };

                                        resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                                        if (resultado[0].respuesta != 1)
                                        { 
                                            TempData["MensajeFile"] = resultado[0].mensaje;
                                        }

                                        if  (resultado[0].respuesta ==1)
                                        {

                                            var Tbl_Sol_Rodales = db.Tbl_Sol_Rodal.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodal in Tbl_Sol_Rodales)
                                            {
                                                Evaluacion_AreaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, Solicitud_id);

                                                Evaluacion_RegionRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, Solicitud_id);

                                                Evaluacion_AreaProtegidaRodal(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id, Solicitud_id);

                                                //Evaluacion_Colisiones(ItemRodal.Finca_id, ItemRodal.Tipo_de_Area, ItemRodal.Rodal_Id);

                                            }

                                            var Tbl_Sol_Rodales_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Rodal => Rodal.Solicitud_id == tbl_sol_solicitud.Solicitud_id).ToList();

                                            foreach (var ItemRodalDescuento in Tbl_Sol_Rodales_Descuento)
                                            {
                                                Evaluacion_AreaRodalDescuento(ItemRodalDescuento.Finca_id, ItemRodalDescuento.Tipo_de_Area, ItemRodalDescuento.Rodal_Id, ItemRodalDescuento.Rodal_Descuento_Id, Solicitud_id);
                                            }


                                            //Evaluacion_AreaRodal(1, 1, 1);


                                            TempData["MensajeFile"] = "Carga realizada con exito.";
                                            //return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                        else
                                        {
                                            //return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                                        }
                                    }
                                }

                            }
                            else
                            {
                                TempData["MensajeFile"] = "Error desconocido en la carga de archivos Ref:Sol_RodalController_001001.";
                                //return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

                            }
                        }
                        //return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
                    }
                    catch (Exception Ex)
                    {
                        TempData["MensajeFile"] = " El archivo contiene datos vacios en una posición en la que se esperaba información. Corregir e intentar nuevamente.";
                        //return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });

                    }
                }
                    else
                    {
                        ModelState.AddModelError("File", "Por favor seleccione el archivo con los polígonos.");
                    }


                Tbl_Gral_Carga tbl_Gral_Carga = db.Tbl_Gral_Carga.Where(Obj => Obj.Tipo_Carga_id == intTipo_Carga_id & Obj.Carga_id == lnCarga_id).FirstOrDefault();

                if (tbl_Gral_Carga != null)
                {
                    TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>" + tbl_Gral_Carga.Observaciones ?? "";
                }
                else
                {
                    TempData["MensajeFile"] = "<table><tr><td>" + TempData["MensajeFile"] + "</td></tr></table>";
                }
            }
            return RedirectToAction("../Sol_Rodal/UploadExcel", new { solicitud_id = Solicitud_id, firma = firma });
        }


        public ActionResult Borrar(int Fincaid, int TipodeArea, long varRodalid, long solicitud_id, string firma)
        {

            //long varSolicitudid = solicitud_id;

            //int intDosometricos = db.Tbl_Sol_Rodal_Dasometrico.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_id == Fincaid && Obj.Tipo_de_Area == TipodeArea &&  Obj.Rodal_id == varRodalid).Count();

            //if (intDosometricos > 0)
            //{
            //    return RedirectToAction("../Sol_Rodal_Inventario/BorradoFallido");
            //}

            // Grabar  mensaje entre procesos
            string sqlQuery = "Exec Sp_Sol_BorrarRodal @Solicitud_id, @Finca_id, @Tipo_de_Area, @Rodal_id";

            sqlParams = new SqlParameter[]
            {
                                 new SqlParameter { ParameterName = "@Solicitud_id",  Value = solicitud_id, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@Finca_id",  Value = Fincaid, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@Tipo_de_Area",  Value = TipodeArea, Direction = System.Data.ParameterDirection.Input },
                                 new SqlParameter { ParameterName = "@Rodal_id",  Value = varRodalid, Direction = System.Data.ParameterDirection.Input }
            };
            
            db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

            //IEnumerable<Tbl_Sol_Rodal_Poligono> poligonoList = db.Tbl_Sol_Rodal_Poligono.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_id == Fincaid && Obj.Tipo_de_Area == TipodeArea && Obj.Rodal_Id == varRodalid);
            ////Now you have everything you need.
            //foreach (var Item in poligonoList)
            //{
            //    //Remove
            //    db.Tbl_Sol_Rodal_Poligono.Remove(Item);
            //}

            //db.SaveChanges();

            //Tbl_Sol_Rodal tbl_Sol_Rodal = db.Tbl_Sol_Rodal.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_id == Fincaid && Obj.Tipo_de_Area == TipodeArea && Obj.Rodal_Id == varRodalid).First();
            //db.Tbl_Sol_Rodal.Remove(tbl_Sol_Rodal);
            //db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");

        }


        public ActionResult BorrarAreaDescuento(long Fincaid, long PadreRodal_id, long DescuentoRodalid, long solicitud_id, string firma)
        {
            long varSolicitudid = solicitud_id;

            IEnumerable<Tbl_Sol_Rodal_Descuento_Poligono> poligonoList = db.Tbl_Sol_Rodal_Descuento_Poligono.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_id == Fincaid && Obj.Rodal_Id == PadreRodal_id && Obj.Rodal_Descuento_Id == DescuentoRodalid);
            //Now you have everything you need.
            foreach (var Item in poligonoList)
            {
                //Remove
                db.Tbl_Sol_Rodal_Descuento_Poligono.Remove(Item);
            }

            db.SaveChanges();


            Tbl_Sol_Rodal_Descuento tbl_Sol_Rodal_Descuento = db.Tbl_Sol_Rodal_Descuento.Where(Obj => Obj.Solicitud_id == varSolicitudid && Obj.Finca_id == Fincaid && Obj.Rodal_Id == PadreRodal_id && Obj.Rodal_Descuento_Id == DescuentoRodalid).First();
            db.Tbl_Sol_Rodal_Descuento.Remove(tbl_Sol_Rodal_Descuento);
            db.SaveChanges();

            return RedirectToAction("../Home/RegistroEliminado");

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

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class RNF_FincaController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();
        // GET: RNF_Finca
        public ActionResult Index(string Guid_id)
        {
            ViewBag.Guid_id = Guid_id;
            decimal TotalArea, TotalAreaSegunFincas;

            var tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            var tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id);


            ViewBag.AreaTotalFincasSegunFincas = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id).Sum(Obj => Obj.AreaTotal);

            ViewBag.TotalAreaARegistrar = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id).Sum(Obj => Obj.AreaARegistrar);

            ViewBag.AreaTotalFincasSegunFincas = ViewBag.AreaTotalFincasSegunFincas ?? 0;


            TotalArea = tbl_RNF_Registro.AreaTotalFincas ?? 0;
            ViewBag.TotalAreaARegistrar = ViewBag.TotalAreaARegistrar ?? 0;

            TotalAreaSegunFincas = ViewBag.AreaTotalFincasSegunFincas;

            ViewBag.FincaMensaje = "";

            if (TotalArea == 0)
            {
                ViewBag.FincaMensaje = "Debe definir el área total según documentos para las fincas a ingresar en la solicitud";
            }
            else
            {
                if ((TotalArea - (decimal)0.01) > TotalAreaSegunFincas)
                {
                    ViewBag.FincaMensaje = "El total de las áreas según documentos es menor al ingresado en el encabezado.";

                }
                if ((TotalArea + (decimal)0.01) < TotalAreaSegunFincas)
                {
                    ViewBag.FincaMensaje = "El total de las áreas según documentos es mayor al ingresado en el encabezado.";
                }

            }

            return View(tbl_RNF_Finca.ToList());

        }


        public class Class_Region
        {
            public int Id_Region { get; set; }
            public string Nombre_Region { get; set; }
        }

        public ActionResult IndexFincaRodalDasometricos(string Guid_id)
        {

            var tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id);

            ViewBag.CantidadFincas = tbl_RNF_Finca.Count();

            ViewBag.Guid_id = Guid_id;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();
            string sqlQuery;

            if (ViewBag.CantidadFincas > 0)
            {

                var tbl_RNF_FincaFirst = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == Guid_id).First();

                List<Tbl_Gral_Region> tbl_Gral_Regions = (from R in db.Tbl_Gral_Region
                                                          from SR in db.Tbl_Gral_SubRegion
                                                          from RD in db.Tbl_Gral_SubRegionDepartamento
                                                          where RD.Id_Departamento == tbl_RNF_FincaFirst.DepartamentoFinca_Id
                                                            && RD.Id_Municipio == tbl_RNF_FincaFirst.MunicipioFinca_Id
                                                            && SR.SubRegion_id == RD.SubRegion_id
                                                            && R.Id_Region == SR.Region_id
                                                          select R).ToList();

                SelectList Sl_RegionFinca_id = new SelectList(tbl_Gral_Regions, nameof(Tbl_Gral_Region.Id_Region), nameof(Tbl_Gral_Region.Nombre_RegionCompleto), tbl_RNF_Registro.Region_id);


                List<SelectListItem> selectListItems = Sl_RegionFinca_id.GroupBy(x => x.Value)
                                                 .Select(x => x.First())
                                                 .ToList();

                ViewBag.RegionFinca_id = selectListItems;

                List<Tbl_Gral_SubRegion> tbl_Gral_SubRegions = (from R in db.Tbl_Gral_SubRegion
                                                                from RD in db.Tbl_Gral_SubRegionDepartamento
                                                                where RD.Id_Departamento == tbl_RNF_FincaFirst.DepartamentoFinca_Id
                                                                   && RD.Id_Municipio == tbl_RNF_FincaFirst.MunicipioFinca_Id
                                                                   && R.SubRegion_id == RD.SubRegion_id
                                                                select R).ToList();


                var tbl_Gral_SubRegionsFirst = tbl_Gral_SubRegions.First();
                SelectList Sl_SubRegionFinca_id = new SelectList(tbl_Gral_SubRegions, "SubRegion_id", "Nombre_SubRegionCompleto", tbl_RNF_Registro.SubRegion_id);

                ViewBag.SubRegionFinca_id = Sl_SubRegionFinca_id;

                if (tbl_Gral_SubRegions.Count() > 0)
                {
                    try
                    {
                        tbl_RNF_Registro.Region_id = Int32.Parse(Sl_RegionFinca_id.SelectedValue.ToString());
                        tbl_RNF_Registro.SubRegion_id = Int32.Parse(Sl_SubRegionFinca_id.SelectedValue.ToString());
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Mensaje = ex.Message.ToString();
                    }

                }
                else
                {
                    TempData["NombeDepartamento"] = tbl_RNF_FincaFirst.Tbl_Gral_Departamento.Departamento;
                    TempData["NombeMunicipio"] = tbl_RNF_FincaFirst.Tbl_Gral_Municipio.Municipio;
                    return RedirectToAction("../Sol_Finca/SubRegionNoAsignada");
                }

                //Actualmente del lado interno no hay dirección de notificación, por lo que no se agregó en comparación con Sol_FincaController donde sí existe dirección de notificación

            }
            else
            {

                ViewBag.RegionFinca_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "Nombre_RegionCompleto", tbl_RNF_Registro.Region_id);
                ViewBag.SubRegionFinca_id = new SelectList(db.Tbl_Gral_SubRegion.Where(objeto => objeto.Region_id == tbl_RNF_Registro.Region_id && objeto.Estado_id == true), "SubRegion_id", "Nombre_SubRegionCompleto", tbl_RNF_Registro.SubRegion_id);

            }




            return View();

        }

        [HttpPost]
        public JsonResult actualizarArea(decimal areafinca, string Guid_id)
        {
            int codRespuesta = 0;
            string strRespuesta = "";
            string jsonResult;

            if (areafinca < 0)
            {

                strRespuesta = "El área no puede ser menor a cero.";

                jsonResult = "{\"CodRespuesta\":"
                     + "\"" + codRespuesta + "\","
                     + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

                return Json(jsonResult);
            }

            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;



            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            tbl_RNF_Registro.AreaTotalFincas = areafinca;

            tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Registro.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Registro.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_Registro.swcreatedbyinterno = true;
            }

            db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
            db.SaveChanges();
            strRespuesta = "Actualización realizada.";
            codRespuesta = 1;

            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        [HttpPost]
        public JsonResult actualizarregion(int regionid, int subregionid, string Guid_id)
        {
            Usuario objUs = new Usuario();
            objUs.intUsuario_id = 0;

            int codRespuesta = 0;
            string strRespuesta = "";

            string jsonResult;

            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'

                ViewBag.Mensaje = objSesion.getStrMensaje();

                strRespuesta = "Usuario no encontrado.";

                jsonResult = "{\"CodRespuesta\":"
                 + "\"" + codRespuesta + "\","
                 + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";
                return Json(jsonResult);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == Guid_id).FirstOrDefault();

            tbl_RNF_Registro.Region_id = regionid;
            tbl_RNF_Registro.SubRegion_id = subregionid;

            tbl_RNF_Registro.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Registro.swdateupdated = DateTime.Now;

            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Registro.swcreatedbyinterno = false;
            }
            else
            {
                tbl_RNF_Registro.swcreatedbyinterno = true;
            }

            //if (tbl_RNF_Registro.Estado_id == 0)
            {
                db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                db.SaveChanges();
                strRespuesta = "Actualización realizada.";
            }
            //else
            //{
            //    strRespuesta = "No se permite actualizar la región.";

            //}


            jsonResult = "{\"CodRespuesta\":"
                            + "\"" + codRespuesta + "\","
                            + "\"strRespuesta\":" + "\"" + strRespuesta + "\"}";

            return Json(jsonResult);
        }

        public ActionResult Edit(string No_Registro, long fincaId)
        {
            ViewBag.No_Registro = No_Registro;
            ViewBag.fincaId = fincaId;

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


            //long varSolicitudid = Int64.Parse(Session[Constants.session_Solicitud].ToString());


            Tbl_RNF_Finca tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_Id == fincaId).FirstOrDefault();

            var tbl_RNF_FincaG = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro);


            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_Finca.DepartamentoFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id); ;


            ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

            if (ViewBag.CantidadFincas > 1)
            {
                Tbl_RNF_Finca tbl_RNF_FincaFirst = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

            }

            ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);
            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_RNF_Finca.CategoriaSIGAP_Id);

            tbl_RNF_Finca.swcreatedby = objUs.intUsuario_id;
            tbl_RNF_Finca.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Finca.swcreatedbyinterno = false;
            }
            else
            {
                tbl_RNF_Finca.swcreatedbyinterno = true;
            }

            return View(tbl_RNF_Finca);
        }

        [HttpPost]
        public ActionResult Edit(Tbl_RNF_Finca tbl_RNF_Finca)
        {
            bool blError = false;
            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            ViewBag.Mensaje = "";

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

            var tbl_RNF_FincaG = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro);



            if (!objRNFGrants.Editar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar o adicionar datos de la finca.";
            }

            if (((tbl_RNF_Finca.ConstanciaDePropiedad_id == 1) || (tbl_RNF_Finca.ConstanciaDePropiedad_id == 6)) && ((tbl_RNF_Finca.RegFolio ?? "") == "" || (tbl_RNF_Finca.RegLibro ?? "") == "" || (tbl_RNF_Finca.RegNumero ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar folio, finca, libro y departamento del registro.";
            }

            if ((tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha != null) && (tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha > DateTime.Now))
            {
                blError = true;
                ViewBag.Mensaje = "Error: La fecha de la constancia de propiedad debe ser menor a la fecha actual.";
            }


            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 2) && ((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 3) && (((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 4) && (((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario, numero de escritura y fecha.";
            }

            if (((tbl_RNF_Finca.ConstanciaDePropiedad_id == 5) || (tbl_RNF_Finca.ConstanciaDePropiedad_id == 7)) && ((tbl_RNF_Finca.ConstanciaPropiedadDescripcion ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar los documentos que constaten la acreditación de la propiedad.";
            }


            if (tbl_RNF_Finca.AreaTotal < tbl_RNF_Finca.AreaARegistrar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: No puede registrar un área mayor al área de la finca.";
            }


            tbl_RNF_Finca.swupdatedby = objUs.intUsuario_id;
            tbl_RNF_Finca.swdateupdated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Finca.swupdatedbyinterno = false;

            }
            else
            {
                tbl_RNF_Finca.swupdatedbyinterno = true;

            }




            if (blError == false)
            {

                if (tbl_RNF_Finca.NoOtrosRegistrosRNF == true)
                {
                    tbl_RNF_Finca.Registros = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 1)
                {
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 2)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 3)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 4)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 5)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 6)
                {
                    tbl_RNF_Finca.RegDepartamento_id = 31;
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 7)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    //tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }



                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(tbl_RNF_Finca).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("../Home/RegistroActualizado");
                    }

                }
                catch
                {
                    ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
                    ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                    ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);



                    ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

                    if (ViewBag.CantidadFincas > 1)
                    {
                        Tbl_Sol_Finca tbl_RNF_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_RNF_Finca.Solicitud_id).First();

                        ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                        ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

                    }



                    ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);
                    ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);
                    ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_RNF_Finca.CategoriaSIGAP_Id);

                    return View(tbl_RNF_Finca);
                }
            }
            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);



            ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

            if (ViewBag.CantidadFincas > 1)
            {
                Tbl_Sol_Finca tbl_RNF_FincaFirst = db.Tbl_Sol_Finca.Where(Obj => Obj.Solicitud_id == tbl_RNF_Finca.Solicitud_id).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

            }



            ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);
            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);
            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", tbl_RNF_Finca.CategoriaSIGAP_Id);

            return View(tbl_RNF_Finca);

        }



        public ActionResult Borrar(string No_Registro, long fincaId)
        {

            int countrodales = 0;
            int countrodalesdescuento = 0;
            int countdasometricos = 0;
            int counttotal = 0;
            countrodales = db.Tbl_RNF_Rodal.Count(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == fincaId);
            countrodalesdescuento = db.Tbl_RNF_Rodal_Descuento.Count(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == fincaId);
            countdasometricos = db.Tbl_RNF_Rodal_Dasometrico.Count(Obj => Obj.No_Registro == No_Registro && Obj.Finca_id == fincaId);
            string vistareturn = "../Home/AccesoDenegado";

            counttotal = countrodales + countrodalesdescuento + countdasometricos;
            if (counttotal == 0)
            {
                Tbl_RNF_Finca tbl_RNF_Finca = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == No_Registro && Obj.Finca_Id == fincaId).FirstOrDefault();
                if (tbl_RNF_Finca != null)
                {
                    db.Tbl_RNF_Finca.Remove(tbl_RNF_Finca);
                    db.SaveChanges();
                }

                vistareturn = "../Home/RegistroEliminado";

            }

            return RedirectToAction(vistareturn);

        }
        public ActionResult Create(string No_Registro, string mensaje)
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

            ViewBag.No_Registro = No_Registro;
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == No_Registro).FirstOrDefault();

            Tbl_RNF_Finca tbl_RNF_Finca = new Tbl_RNF_Finca();
            tbl_RNF_Finca.No_Registro = tbl_RNF_Registro.No_Registro;
            tbl_RNF_Finca.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
            tbl_RNF_Finca.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
            tbl_RNF_Finca.Solicitud_id = tbl_RNF_Registro.Solicitud_id;

            long lngIdt = 0;

            try
            {
                lngIdt = db.Tbl_RNF_Finca.Where(Finca => Finca.No_Registro == tbl_RNF_Registro.No_Registro).Max(u => u.Finca_Id);
                lngIdt++;

            }
            catch
            {
                lngIdt = 1;
            }
            tbl_RNF_Finca.Finca_Id = lngIdt;

            tbl_RNF_Finca.DepartamentoFinca_Id = 7;
            tbl_RNF_Finca.RegDepartamento_id = 7;

            tbl_RNF_Finca.MunicipioFinca_Id = 74;
            tbl_RNF_Finca.ConstanciaDePropiedad_id = 0;


            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == 7), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);


            var tbl_RNF_FincaG = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro);

            ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

            if (ViewBag.CantidadFincas > 0)
            {
                var tbl_RNF_FincaFirst = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

            }


            ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);
            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);

            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);


            tbl_RNF_Finca.swcreatedby = objUs.intUsuario_id;
            tbl_RNF_Finca.swdatecreated = DateTime.Now;


            if (objUs.EsInterno != 1)
            {
                tbl_RNF_Finca.swcreatedbyinterno = false;

            }
            else
            {
                tbl_RNF_Finca.swcreatedbyinterno = true;

            }

            tbl_RNF_Finca.NoOtrosRegistrosRNF = true;

            ViewBag.mensaje = mensaje;

            return View(tbl_RNF_Finca);
        }

        // POST: Sol_Finca/Create
        [HttpPost]
        public ActionResult Create(Tbl_RNF_Finca tbl_RNF_Finca)
        {
            bool blError = false;
            //EdicionSolicitudGrants objGrant = (EdicionSolicitudGrants)Session[Constants.session_EdicionSolicitudGrants];
            EdicionRNFGrants objRNFGrants = (EdicionRNFGrants)Session[Constants.session_EdicionRNFGrants];
            ViewBag.Mensaje = "";


            var tbl_RNF_FincaG = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro);

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).FirstOrDefault();
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);
            //Tbl_Sol_Solicitud tbl_sol_solicitud = db.Tbl_Sol_Solicitud.Find(Session[Constants.session_Solicitud]);



            if (!objRNFGrants.Agregar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: El estatus de la solicitud no permite editar o adicionar datos de la finca.";
            }

            if (((tbl_RNF_Finca.ConstanciaDePropiedad_id == 1) || (tbl_RNF_Finca.ConstanciaDePropiedad_id == 6)) && ((tbl_RNF_Finca.RegFolio ?? "") == "" || (tbl_RNF_Finca.RegLibro ?? "") == "" || (tbl_RNF_Finca.RegNumero ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar folio, finca, libro y departamento del registro.";
            }

            if ((tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha != null) && (tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha > DateTime.Now))
            {
                blError = true;
                ViewBag.Mensaje = "Error: La fecha de la constancia de propiedad debe ser menor a la fecha actual.";
            }



            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 2) && ((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 3) && (((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario.";
            }

            if ((tbl_RNF_Finca.ConstanciaDePropiedad_id == 4) && (((tbl_RNF_Finca.ActaNotarial_Notario ?? "") == "") || ((tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero ?? "") == "")))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar el nombre del notario, numero de escritura y fecha.";
            }

            if (((tbl_RNF_Finca.ConstanciaDePropiedad_id == 5) || (tbl_RNF_Finca.ConstanciaDePropiedad_id == 7)) && ((tbl_RNF_Finca.ConstanciaPropiedadDescripcion ?? "") == ""))
            {
                blError = true;
                ViewBag.Mensaje = "Error: Debe especificar los documentos que constaten la acreditación de la propiedad.";
            }


            if (tbl_RNF_Finca.AreaTotal < tbl_RNF_Finca.AreaARegistrar)
            {
                blError = true;
                ViewBag.Mensaje = "Error: No puede registrar un área mayor al área de la finca.";
            }

            if (blError == false)
            {
                if (tbl_RNF_Finca.NoOtrosRegistrosRNF == true)
                {
                    tbl_RNF_Finca.Registros = "";
                }

                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 1)
                {
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 2)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 3)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 4)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 5)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    //tbl_RNF_Finca.ActaNotarial_Notario = "";
                    //tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    //tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }


                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 6)
                {
                    tbl_RNF_Finca.RegDepartamento_id = 31;
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Fecha = null;
                    tbl_RNF_Finca.ConstanciaPropiedadDescripcion = null;
                    tbl_RNF_Finca.ActaNotarial_Notario = "";
                    tbl_RNF_Finca.ActaNotarialConCertificacion_NombreDeCertificador = "";
                    tbl_RNF_Finca.ActaNotarialDeEscrituraPublica_Numero = "";
                }

                if (tbl_RNF_Finca.ConstanciaDePropiedad_id == 7)
                {
                    tbl_RNF_Finca.RegFolio = "";
                    tbl_RNF_Finca.RegNumero = "";
                    tbl_RNF_Finca.RegLibro = "";
                }
                try
                {
                    if (ModelState.IsValid)
                    {

                        long lngIdt = 0;

                        try
                        {
                            lngIdt = db.Tbl_RNF_Finca.Where(Finca => Finca.No_Registro == tbl_RNF_Finca.No_Registro).Max(u => u.Finca_Id);
                            lngIdt++;
                        }
                        catch
                        {
                            lngIdt = 1;
                        }

                        tbl_RNF_Finca.Finca_Id = lngIdt;

                        db.Tbl_RNF_Finca.Add(tbl_RNF_Finca);
                        db.SaveChanges();

                        if (tbl_RNF_Registro.Categoria_id == 6)
                        {
                            return RedirectToAction("Edit", "RNF_Finca", new { No_Registro = tbl_RNF_Registro.No_Registro, fincaId = lngIdt });
                        }


                        return RedirectToAction("../RNF_Finca/Create", new { No_Registro = tbl_RNF_Registro.No_Registro, mensaje = "<< Ingrese los datos de esta otra finca >> " });


                    }

                }
                catch
                {
                    ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
                    ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                    ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);


                    ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

                    if (ViewBag.CantidadFincas > 0)
                    {
                        Tbl_RNF_Finca tbl_RNF_FincaFirst = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).First();

                        ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                        ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

                    }


                    ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);
                    ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);

                    ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);

                    return View(tbl_RNF_Finca);
                }
            }
            ViewBag.RegDepartamento_id = new SelectList(db.Tbl_Gral_DepartamentoRegistro, "Departamento_id", "Departamento", tbl_RNF_Finca.RegDepartamento_id);
            ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento, "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
            ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio, "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

            ViewBag.CantidadFincas = tbl_RNF_FincaG.Count();

            if (ViewBag.CantidadFincas > 0)
            {
                Tbl_RNF_Finca tbl_RNF_FincaFirst = db.Tbl_RNF_Finca.Where(Obj => Obj.No_Registro == tbl_RNF_Finca.No_Registro).First();

                ViewBag.DepartamentoFinca_Id = new SelectList(db.Tbl_Gral_Departamento.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id), "Departamento_id", "Departamento", tbl_RNF_Finca.DepartamentoFinca_Id);
                ViewBag.MunicipioFinca_Id = new SelectList(db.Tbl_Gral_Municipio.Where(Obj => Obj.Departamento_id == tbl_RNF_FincaFirst.DepartamentoFinca_Id && Obj.Municipio_id == tbl_RNF_FincaFirst.MunicipioFinca_Id), "Municipio_id", "Municipio", tbl_RNF_Finca.MunicipioFinca_Id);

            }




            ViewBag.ConstanciaDePropiedad_id = new SelectList(db.Tbl_Sol_FincaConstanciaDePropiedad, "ConstanciaDePorpiedad_id", "Descripcion", tbl_RNF_Finca.ConstanciaDePropiedad_id);

            ViewBag.ObjetivoDeLaPlantacion = new SelectList(db.Tbl_Sol_FincaObjetivoDeLaPlantacion, "ObjetivoDeLaPlantacion", "Descripcion", tbl_RNF_Finca.ObjetivoDeLaPlantacion);
            ViewBag.CategoriaSIGAP_Id = new SelectList(db.Tbl_Sol_Rodal_CategoriaSIGAP.Where(Obj => Obj.CategoriaSIGAP_Id > 0), "CategoriaSIGAP_Id", "Descripcion", 1);

            return View(tbl_RNF_Finca);

        }




    }
}
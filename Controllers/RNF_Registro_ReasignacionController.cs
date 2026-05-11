using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using Newtonsoft.Json;
using PagedList;
using System.IO;

namespace RNF_Web.Controllers
{
    public class RNF_Registro_ReasignacionController : Controller
    {
        db_RNFEntities db = new db_RNFEntities();

        class Respuesta
        {
            public string resultado { get; set; }
        }

        class Consulta_DPI
        {
            public int cod_respuesta { get; set; }
            public datos_DPI datos_dpi { get; set; }
        }

        class datos_DPI
        {
            public string DPI { get; set; }
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string Telefono_Oficina { get; set; }
            public string Telefono_Oficina_Extension { get; set; }
            public string No_Nit { get; set; }
            public string Departamento { get; set; }
            public string Municipio { get; set; }
            public string Direccion { get; set; }

        }

        // GET: RNF_Registro_Reasignacion
        public ActionResult Index(string No_Registro, string Expediente, string DPI_Titular, string Motosierra, string NombrePropietario, string ApellidoPropietario, string sortOrder, int? page)
        {
            Usuario objUs = new Usuario();
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
            ViewBag.strNo_Registro = No_Registro;
            ViewBag.strExpediente = Expediente;
            ViewBag.strDPI_Titular = DPI_Titular;
            ViewBag.strMotosierra = Motosierra;
            ViewBag.strNombrePropietario = NombrePropietario;
            ViewBag.strApellidoPropietario = ApellidoPropietario;


            //Se optó por usar una clase para generar la lista de registros,
            //debido a que de este modo se hace más práctico el aplicar los filtros de búsqueda en base a los parámetros que se envíen
            List<RNF_Registro_Informacion> rNF_Registro_Informacions = new List<RNF_Registro_Informacion>();


            //rNF_Registro_Informacions = db.Database.SqlQuery<RNF_Registro_Informacion>(sqlQuery).ToList();
            string sqlQuery = "EXEC SP_RNF_Buscar_RegistroInscrito @No_Registro,@Expediente,@DPI_Titular,@Motosierra,@NombrePropietario,@ApellidoPropietario,@Usuario_id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter{ParameterName = "@No_Registro", Value = (@No_Registro??""), Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@Expediente", Value = (@Expediente??""), Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@DPI_Titular", Value = (@DPI_Titular??""), Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@Motosierra ", Value = (@Motosierra??"") , Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@NombrePropietario", Value = (@NombrePropietario??""), Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@ApellidoPropietario", Value = (@ApellidoPropietario ?? ""), Direction = System.Data.ParameterDirection.Input},
                new SqlParameter{ParameterName = "@Usuario_id", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input},
            };
            //List<fc_RNF_Buscar_RegistroInscrito_Result> fc_RNF_Buscar_RegistroInscrito_Results = db.fc_RNF_Buscar_RegistroInscrito(No_Registro, Expediente, DPI_Titular, Motosierra, NombrePropietario, ApellidoPropietario, objUs.intUsuario_id).ToList();
            List<fc_RNF_Buscar_RegistroInscrito_Result> fc_RNF_Buscar_RegistroInscrito_Results = db.Database.SqlQuery<fc_RNF_Buscar_RegistroInscrito_Result>(sqlQuery, sqlParameters).ToList();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NoRegistro = sortOrder == "NoRegistro" ? "noregistro_desc" : "NoRegistro";
            ViewBag.Expediente = sortOrder == "Expediente" ? "expediente_desc" : "Expediente";
            ViewBag.FechaVencimiento = sortOrder == "FechaVencimiento" ? "fechavencimiento_desc" : "FechaVencimiento";
            ViewBag.DPITitular = sortOrder == "DPITitular" ? "dpititular_desc" : "DPITitular";
            ViewBag.Titular = sortOrder == "Titular" ? "titular_desc" : "Titular";

            var lst = (from d in fc_RNF_Buscar_RegistroInscrito_Results select d);

            switch (sortOrder)
            {
                case "NoRegistro":
                    lst = lst.OrderBy(Obj => Obj.No_Registro);
                    break;
                case "noregistro_desc":
                    lst = lst.OrderByDescending(Obj => Obj.No_Registro);
                    break;
                case "Expediente":
                    lst = lst.OrderBy(Obj => Obj.Expediente);
                    break;
                case "expediente_desc":
                    lst = lst.OrderByDescending(Obj => Obj.Expediente);
                    break;
                case "FechaVencimiento":
                    lst = lst.OrderBy(Obj => Obj.Fecha_De_Vencimiento);
                    break;
                case "fechavencimiento_desc":
                    lst = lst.OrderByDescending(Obj => Obj.Fecha_De_Vencimiento);
                    break;
                case "DPITitular":
                    lst = lst.OrderBy(Obj => Obj.DPI_Titular);
                    break;
                case "dpititular_desc":
                    lst = lst.OrderByDescending(Obj => Obj.DPI_Titular);
                    break;
                case "Titular":
                    lst = lst.OrderBy(Obj => Obj.Titular);
                    break;
                case "titular_desc":
                    lst = lst.OrderByDescending(Obj => Obj.Titular);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult HistoricoRNF(string No_Registro)
        {
            List<Tbl_RNF_RegistroHistory> tbl_RNF_RegistroHistories = (from d in db.Tbl_RNF_RegistroHistory
                                                                       where d.No_Registro == No_Registro
                                                                       orderby d.Solicitud_id descending
                                                                       select d).ToList();


            if (tbl_RNF_RegistroHistories == null)
            {
                tbl_RNF_RegistroHistories = new List<Tbl_RNF_RegistroHistory>();
            };

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            Tbl_RNF_RegistroHistory tbl_RNF_RegistroHistory = new Tbl_RNF_RegistroHistory()
            {
                No_Registro = tbl_RNF_Registro.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro.Solicitud_id,
                SolicitudTipo_id = tbl_RNF_Registro.SolicitudTipo_id,
                GuidSolicitud_id = tbl_RNF_Registro.GuidSolicitud_id,
                Guid_id = tbl_RNF_Registro.Guid_id,
                swdatecreated = tbl_RNF_Registro.swdatecreated,
                swcreatedby = tbl_RNF_Registro.swcreatedby,
                swcreatedbyinterno = tbl_RNF_Registro.swcreatedbyinterno,
                swdateupdated = tbl_RNF_Registro.swdateupdated,
                swupdatedby = tbl_RNF_Registro.swupdatedby,
                swupdatedbyinterno = tbl_RNF_Registro.swupdatedbyinterno,
                Expediente = tbl_RNF_Registro.Expediente,
                Region_id = tbl_RNF_Registro.Region_id,
                SubRegion_id = tbl_RNF_Registro.SubRegion_id,
                Categoria_id = tbl_RNF_Registro.Categoria_id,
                Sub_Categoria_id = tbl_RNF_Registro.Sub_Categoria_id,
                Sub_Sub_Categoria_id = tbl_RNF_Registro.Sub_Sub_Categoria_id,
                Estado_id = tbl_RNF_Registro.Estado_id,
                Fecha_De_Vencimiento = tbl_RNF_Registro.Fecha_De_Vencimiento,
                Tbl_Gral_SolicitudConfiguracionTipo = tbl_RNF_Registro.Tbl_Gral_SolicitudConfiguracionTipo
            };

            tbl_RNF_RegistroHistories.Add(tbl_RNF_RegistroHistory);


            return View(tbl_RNF_RegistroHistories);
        }

        public ActionResult Lista_Registros(string No_Registro, string Expediente, string DPI_Titular)
        {
            Usuario objUs = new Usuario();
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
            if (No_Registro == null)
            {
                No_Registro = "";
            }
            if (Expediente == null)
            {
                Expediente = "";
            }
            if (DPI_Titular == "")
            {
                DPI_Titular = "";
            }

            No_Registro = No_Registro.Replace(" ", "%");

            Expediente = Expediente.Replace(" ", "%");
            Expediente = Expediente.Replace("-", "%");
            Expediente = Expediente.Replace("_", "%");
            Expediente = Expediente.Replace(".", "%");
            Expediente = Expediente.Replace(",", "%");
            Expediente = Expediente.Replace("/", "%");

            DPI_Titular = DPI_Titular.Replace(" ", "%");
            DPI_Titular = DPI_Titular.Replace("-", "%");
            DPI_Titular = DPI_Titular.Replace("_", "%");
            DPI_Titular = DPI_Titular.Replace(".", "%");
            DPI_Titular = DPI_Titular.Replace(",", "%");
            DPI_Titular = DPI_Titular.Replace("/", "%");


            List<RNF_Registro_Informacion> rNF_Registro_Informacions = new List<RNF_Registro_Informacion>();
            string sqlQuery, sqlQueryComplementoBusqueda;
            sqlQueryComplementoBusqueda = "";
            sqlQuery = " Select top 100 \n";
            sqlQuery += " RNF_Reg.[No_Registro] \n";
            sqlQuery += " ,RNF_Reg.[No_RegistroLiteral] \n";
            sqlQuery += " ,RNF_Reg.[No_RegistroCorrelativo] \n";
            sqlQuery += " ,isnull(RNF_Reg.[Expediente] ,'')[Expediente] \n";
            sqlQuery += " ,(select GralReg.Nombre_Region from Tbl_Gral_Region GralReg where GralReg.Id_Region = RNF_Reg.Region_id) Region \n";
            sqlQuery += " ,(select Nombre_SubRegion from Tbl_Gral_SubRegion GralSubReg where GralSubReg.Region_id = RNF_Reg.Region_id and GralSubReg.SubRegion_id = RNF_Reg.SubRegion_id) SubRegion \n";
            sqlQuery += " ,(Select Cat.Descripcion from Tbl_Sol_Solicitud_Categoria Cat where Cat.Categoria_id = RNF_Reg.Categoria_id) Categoria \n";
            sqlQuery += " ,(Select SubCat.Descripcion from Tbl_Sol_Solicitud_Sub_Categoria SubCat where SubCat.Categoria_id = RNF_Reg.Sub_Categoria_id and SubCat.Sub_Categoria_id = RNF_Reg.Sub_Categoria_id) SubCategoria \n";
            sqlQuery += " ,RNF_Reg.[Fecha_De_Vencimiento] \n";
            sqlQuery += " ,RNF_Reg.[UsuarioExterno_id] \n";
            sqlQuery += " ,RNF_Reg.[Solicitud_id] \n";
            sqlQuery += " ,RNF_Reg.[GuidSolicitud_id] \n";
            sqlQuery += " ,isnull(convert(varchar(50),RNF_Reg.[DPI_Titular]),'') [DPI_Titular] \n";
            sqlQuery += " ,isnull((select Nombres + ' ' + Apellidos from Tbl_Seg_UsuarioExterno SegUs where No_Documento = isnull([DPI_Titular],'') ),'') Titular \n";
            sqlQuery += " From \n";
            sqlQuery += " [dbo].[Tbl_RNF_Registro] RNF_Reg \n";
            sqlQueryComplementoBusqueda += " where \n";
            sqlQueryComplementoBusqueda += " (isnull('" + No_Registro + "','')like'' or (RNF_Reg.No_Registro Like '%" + No_Registro + "%')) \n";
            sqlQueryComplementoBusqueda += " and (isnull('" + Expediente + "','')like'' or (RNF_Reg.Expediente Like '%" + Expediente + "%')) \n";
            sqlQueryComplementoBusqueda += " and (isnull('" + DPI_Titular + "','') like '' or (RNF_Reg.DPI_Titular Like '%" + DPI_Titular + "%'))  \n";
            sqlQuery += sqlQueryComplementoBusqueda;
            sqlQuery += " order by \n";
            sqlQuery += " RNF_Reg.No_Registro \n";
            //sqlQuery += " ,RNF_Reg.Expediente \n";
            //sqlQuery += " ,RNF_Reg.Fecha_De_Vencimiento \n";

            rNF_Registro_Informacions = db.Database.SqlQuery<RNF_Registro_Informacion>(sqlQuery).ToList();


            return View(rNF_Registro_Informacions);
        }

        public ActionResult CambiarTitular(string No_Registro)
        {
            Usuario objUs = new Usuario();
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

            Tbl_RNF_Registro tbl_RNF_Registro = new Tbl_RNF_Registro();
            tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                where d.No_Registro == No_Registro
                                select d).FirstOrDefault();

            return View(tbl_RNF_Registro);
        }

        public ActionResult CambiarEstado(string No_Registro)
        {
            Usuario objUs = new Usuario();
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = new Tbl_RNF_Registro_Bitacora()
            {
                No_Registro = tbl_RNF_Registro.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro.Solicitud_id,
            };

            ViewBag.No_Registro = No_Registro;

            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado.Where(Obj => Obj.Estado_id != 0 && Obj.Estado_id != 1).ToList(), "Estado_id", "Descripcion", tbl_RNF_Registro.Estado_id);


            return View(tbl_RNF_Registro_Bitacora);
        }

        public ActionResult GenerarNuevaConstancia(string No_Registro)
        {
            Usuario objUs = new Usuario();
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

            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }
            bool ErrorDetectado = false;

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }


            return View(tbl_RNF_Registro);
        }


        public ActionResult AgregarHallazgo(string No_Registro, long Bitacora_id, int ActivarLayout = 0)
        {
            Usuario objUs = new Usuario();
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
            ViewBag.No_Registro = No_Registro;
            ViewBag.Bitacora_id = Bitacora_id;
            ViewBag.ActivarLayout = ActivarLayout;

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                   && d.Bitacora_id == Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_RNF_Registro_BitacoraHallazgo tbl_RNF_Registro_BitacoraHallazgo = new Tbl_RNF_Registro_BitacoraHallazgo()
            {
                No_Registro = tbl_RNF_Registro_Bitacora.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro_Bitacora.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro_Bitacora.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro_Bitacora.Solicitud_id,
                Bitacora_id = tbl_RNF_Registro_Bitacora.Bitacora_id
            };




            return View(tbl_RNF_Registro_BitacoraHallazgo);
        }
        public ActionResult HallazgoBitacora(string No_Registro, long Bitacora_id)
        {
            Usuario objUs = new Usuario();
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            ViewBag.No_Registro = No_Registro;
            ViewBag.Bitacora_id = Bitacora_id;

            List<Tbl_RNF_Registro_BitacoraHallazgo> tbl_RNF_Registro_BitacoraHallazgos = (from d in db.Tbl_RNF_Registro_BitacoraHallazgo
                                                                                          where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                          && d.Bitacora_id == Bitacora_id
                                                                                          orderby d.Correlativo_id
                                                                                          select d).ToList();

            if (tbl_RNF_Registro_BitacoraHallazgos == null)
            {
                tbl_RNF_Registro_BitacoraHallazgos = new List<Tbl_RNF_Registro_BitacoraHallazgo>();
            }

            return View(tbl_RNF_Registro_BitacoraHallazgos);
        }

        public ActionResult DocumentosCambioEstado(string No_Registro, long Bitacora_id)
        {
            Usuario objUs = new Usuario();
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

            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == No_Registro
                                                 select d).FirstOrDefault();

            ViewBag.No_Registro = No_Registro;
            ViewBag.Bitacora_id = Bitacora_id;

            List<Tbl_RNF_Registro_BitacoraDocumento> tbl_RNF_Registro_BitacoraDocumentos = (from d in db.Tbl_RNF_Registro_BitacoraDocumento
                                                                                            where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                                            && d.Bitacora_id == Bitacora_id
                                                                                            orderby d.Bitacora_id, d.Documento_id
                                                                                            select d).ToList();
            if (tbl_RNF_Registro_BitacoraDocumentos == null)
            {
                tbl_RNF_Registro_BitacoraDocumentos = new List<Tbl_RNF_Registro_BitacoraDocumento>();
            }

            ViewBag.MensajeDocumento = "";

            return View(tbl_RNF_Registro_BitacoraDocumentos);
        }
        [HttpPost]
        public ActionResult DocumentosCambioEstado(Tbl_RNF_Registro_BitacoraDocumento model, HttpPostedFileBase upload)
        {
            Usuario objUs = new Usuario();
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

            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }
            bool ErrorDetectado = false;


            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == model.No_Registro
                                                 select d).FirstOrDefault();

            if (tbl_RNF_Registro == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == tbl_RNF_Registro.No_Registro
                                                                   && d.Bitacora_id == model.Bitacora_id
                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_Bitacora == null)
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            ViewBag.No_Registro = model.No_Registro;
            ViewBag.Bitacora_id = model.Bitacora_id;
            ViewBag.MensajeDocumento = "";

            long Documento_id = 0;

            try
            {
                Documento_id = db.Tbl_RNF_Registro_BitacoraDocumento.Where(Obj => Obj.No_Registro == model.No_Registro && Obj.Bitacora_id == model.Bitacora_id).Max(Obj => Obj.Documento_id);
            }
            catch (Exception ex)
            {
                Documento_id = 0;
            }

            Documento_id++;

            model.Documento_id = Documento_id;
            model.swcreatedby = objUs.intUsuario_id;
            model.swcreatedbyinterno = EsInterno;
            model.swdatecreated = DateTime.Now;

            string partialpath = "~/Archivos_Subidos/" + model.No_Registro + "/Bitacora_" + model.Bitacora_id + "/";

            string Nombre_Archivo = "";



            if (!ErrorDetectado)
            {
                try
                {
                    Nombre_Archivo = upload.FileName;
                    Nombre_Archivo = Nombre_Archivo.Replace("-", "");
                    Nombre_Archivo = Nombre_Archivo.Replace("(", "");
                    Nombre_Archivo = Nombre_Archivo.Replace(")", "");
                    Nombre_Archivo = Nombre_Archivo.Replace(" ", "");
                    string PathArchivo = Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);

                    if (!System.IO.File.Exists(PathArchivo))
                    {
                        string PathCrear = partialpath;
                        if (!Directory.Exists(Server.MapPath(PathCrear)))
                        {
                            Directory.CreateDirectory(Server.MapPath(PathCrear));
                        }
                        upload.SaveAs(PathArchivo);
                    }
                    else
                    {
                        System.IO.File.Delete(PathArchivo);
                        upload.SaveAs(PathArchivo);

                    }

                    model.NombreArchivo = Nombre_Archivo;
                    ViewBag.MensajeDocumento += "Archivo subido con éxito";
                }
                catch
                {
                    ViewBag.MensajeDocumento += "Hubo un error al subir el archivo";
                    return View(model);
                }

            }


            if (ModelState.IsValid && !ErrorDetectado)
            {
                model.No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral;
                model.No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo;
                model.Solicitud_id = tbl_RNF_Registro.Solicitud_id;



                db.Tbl_RNF_Registro_BitacoraDocumento.Add(model);
                db.SaveChanges();
                return RedirectToAction("DocumentosCambioEstado", "RNF_Registro_Reasignacion", new { No_Registro = model.No_Registro, Bitacora_id = model.Bitacora_id });

            }



            return View(model);
        }

        public JsonResult Obtenerhref(Tbl_RNF_Registro_BitacoraDocumento model)
        {

            Tbl_RNF_Registro_BitacoraDocumento img = db.Tbl_RNF_Registro_BitacoraDocumento.Where(Obj => Obj.No_Registro == model.No_Registro && Obj.Bitacora_id == model.Bitacora_id && Obj.Documento_id == model.Documento_id).FirstOrDefault();

            string partialpath = "/Archivos_Subidos/" + model.No_Registro + "/Bitacora_" + model.Bitacora_id + "/";



            if (img.NombreArchivo.Split('.')[1] == "pdf" || img.NombreArchivo.Split('.')[1] == "PDF" ||
                img.NombreArchivo.Split('.')[1] == "jpg" || img.NombreArchivo.Split('.')[1] == "JPG" ||
                img.NombreArchivo.Split('.')[1] == "bmp" || img.NombreArchivo.Split('.')[1] == "BMP" ||
                img.NombreArchivo.Split('.')[1] == "png" || img.NombreArchivo.Split('.')[1] == "PNG" ||
                img.NombreArchivo.Split('.')[1] == "gif" || img.NombreArchivo.Split('.')[1] == "GIF")
            {

                string fileLocation = partialpath + img.NombreArchivo;

                return Json(fileLocation);


            }
            else
            {
                string fileLocation = partialpath + img.NombreArchivo;

                return Json(fileLocation);
            }
        }

        public ActionResult RNF_Bitacora(string No_Registro)
        {
            Usuario objUs = new Usuario();
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

            List<Tbl_RNF_Registro_Bitacora> tbl_RNF_Registro_Bitacoras = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                          where d.No_Registro == No_Registro
                                                                          orderby d.swdatecreated descending
                                                                          select d).ToList();

            if (tbl_RNF_Registro_Bitacoras == null)
            {
                tbl_RNF_Registro_Bitacoras = new List<Tbl_RNF_Registro_Bitacora>();
            }

            ViewBag.No_Registro = No_Registro;

            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado.ToList(), "Estado_id", "Descripcion");

            return View(tbl_RNF_Registro_Bitacoras);
        }

        public ActionResult HabilitarEdicion_RNF(string No_Registro)
        {
            Usuario objUs = new Usuario();
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

            Tbl_RNF_Registro tbl_RNF_Registro = new Tbl_RNF_Registro();
            tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                where d.No_Registro == No_Registro
                                select d).FirstOrDefault();

            return View(tbl_RNF_Registro);
        }

        public bool DPI_Valido(string No_DPI)
        {
            if (No_DPI.Length.ToString() == "9")
            {
                return true;
            }
            else
            {

            return db.Database.SqlQuery<bool>("SELECT dbo.Fnc_Gral_DPI_Valido(@p0)", No_DPI).FirstOrDefault(); ;
            }
        }

        [HttpPost]
        public JsonResult CambiarNuevoTitular(string No_Registro, string DPI_Nuevo)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            bool ErrorDetectado = false;
            string strRespuesta;

            strRespuesta = "";


            if (!DPI_Valido(DPI_Nuevo))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }


            if (!ErrorDetectado)
            {

                string sqlQuery;
                SqlParameter[] sqlParams;
                sqlQuery = "Exec [SP_RNF_Cambio_Titular_DPI]  @No_Registro, @DPI_Nuevo, @swcreatedby, @swcreatedbyinterno";

                sqlParams = new SqlParameter[]
                {
                new SqlParameter { ParameterName = "@No_Registro",  Value = No_Registro, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@DPI_Nuevo",  Value = DPI_Nuevo, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedby", Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@swcreatedbyinterno", Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                };

                List<ResultFromStoreProcedure> resultado = new List<ResultFromStoreProcedure>
                    { new ResultFromStoreProcedure { id = 0, mensaje= "Fallo desconocido.", respuesta = 0 }  };

                resultado = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).ToList();

                strRespuesta = resultado[0].respuesta.ToString();
                Respuesta respuesta = new Respuesta();
                respuesta.resultado = "Se ha realizado la transferencia de Titular";
                return Json(JsonConvert.SerializeObject(respuesta));

            }
            else
            {
                Respuesta respuesta = new Respuesta();
                respuesta.resultado = "El DPI proporcionado no es válido, por favor revisar nuevamente";
                return Json(JsonConvert.SerializeObject(respuesta));
            }
        }

        [HttpPost]
        public JsonResult CambiarEstadoRegistro(Tbl_RNF_Registro_Bitacora model, bool AsignarTecnico = false, int TiempoInactivacion = 0)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }



            bool EsInterno = false;

            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }
            bool ErrorDetectado = false;
            string strRespuesta;

            strRespuesta = "";
            Tbl_RNF_Registro tbl_RNF_Registro = (from d in db.Tbl_RNF_Registro
                                                 where d.No_Registro == model.No_Registro
                                                 select d).FirstOrDefault();

            long bitacora_id = 0;
            try
            {
                bitacora_id = db.Tbl_RNF_Registro_Bitacora.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro).Max(Obj => Obj.Bitacora_id);
            }
            catch
            {
                bitacora_id = 0;
            }
            bitacora_id++;

            model.InactivacionTemporal = model.InactivacionTemporal ?? false;
            model.InactivacionDefinitiva = model.InactivacionDefinitiva ?? false;




            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = new Tbl_RNF_Registro_Bitacora()
            {
                No_Registro = tbl_RNF_Registro.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro.Solicitud_id,
                Bitacora_id = bitacora_id,
                Estado_id = model.Estado_id,
                Motivo = model.Motivo,
                swdatecreated = DateTime.Now,
                swcreatedby = objUs.intUsuario_id,
                swcreatedbyinterno = EsInterno,
                InactivacionTemporal = model.InactivacionTemporal,
                InactivacionDefinitiva = model.InactivacionDefinitiva,
                Rol_Id_Del_Solicitante = Constants.MaximoRol(objUs.intUsuario_id),
                FechaInicioInactivacionTemporal = model.FechaInicioInactivacionTemporal,
                FechaFinInactivacionTemporal = model.FechaFinInactivacionTemporal
            };
            if (model.InactivacionTemporal == true)
            {
                if (TiempoInactivacion != 0)
                {
                    DateTime fechainicioinactivacion = DateTime.Now;
                    DateTime fechafininactivacion = fechainicioinactivacion.AddDays(TiempoInactivacion);
                    tbl_RNF_Registro_Bitacora.FechaInicioInactivacionTemporal = fechainicioinactivacion;
                    tbl_RNF_Registro_Bitacora.FechaFinInactivacionTemporal = fechafininactivacion;
                }
            }

            db.Tbl_RNF_Registro_Bitacora.Add(tbl_RNF_Registro_Bitacora);
            db.SaveChanges();

            tbl_RNF_Registro.Estado_id = model.Estado_id;
            tbl_RNF_Registro.Bitacora_id = bitacora_id;
            tbl_RNF_Registro.Descripcion_InactivacionTecnico = model.Motivo;
            db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
            db.SaveChanges();
            jsonRespuesta = new JsonRespuesta()
            {
                Result = 1,
                Mensaje = "Registro actualizado"
            };

            return Json(jsonRespuesta);
        }

        class JsonRespuesta
        {
            public int Result { get; set; }
            public string Mensaje { get; set; }
            public string Ubicacion { get; set; }
        }

        [HttpPost]
        public JsonResult CambiarTiempoEdicion_RNF(Tbl_RNF_Registro model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();
            if (tbl_RNF_Registro != null)
            {
                DateTime FechaModificacionDisponible = model.FechaModificacionDisponible ?? DateTime.Now;
                FechaModificacionDisponible = FechaModificacionDisponible.AddHours(23).AddMinutes(59).AddSeconds(59);
                tbl_RNF_Registro.FechaModificacionDisponible = FechaModificacionDisponible;
                db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                db.SaveChanges();
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Asignación realizada con éxito, la disponibilidad de cambios del registro estarán disponibles hasta " + FechaModificacionDisponible.ToString()
                };
            }
            else
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Registro no encontrado, verifique nuevamente"
                };
            }

            return Json(jsonRespuesta);
        }

        public JsonResult ConsultarDPINuevoTitular(string DPI_Nuevo)
        {
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(null);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            bool ErrorDetectado = false;
            string strRespuesta;

            strRespuesta = "";

            Consulta_DPI consulta_DPI = new Consulta_DPI();
            if (!DPI_Valido(DPI_Nuevo))
            {

                TempData["Mensaje"] = TempData["Mensaje"] + "El número de DPI proporcionado no es invalido.";
                ErrorDetectado = true;
                ViewBag.RegistroGrabado = 0;
            }


            if (!ErrorDetectado)
            {

                datos_DPI datos_dpi = new datos_DPI();
                Tbl_Seg_UsuarioExterno tbl_Seg_UsuarioExterno = new Tbl_Seg_UsuarioExterno();
                datos_dpi = (from d in db.Tbl_Seg_UsuarioExterno
                             where d.No_Documento == DPI_Nuevo && d.DocumentoID_Tipo == 1
                             select new datos_DPI
                             {
                                 DPI = d.No_Documento,
                                 Nombre = d.Nombres + " " + d.Apellidos,
                                 Telefono = d.Telefono_Celular,
                                 Telefono_Oficina = d.Telefono_Oficina,
                                 Telefono_Oficina_Extension = d.Telefono_Oficina_Extension,
                                 No_Nit = d.No_NIT,
                                 Departamento = d.Tbl_Gral_Departamento.Departamento,
                                 Municipio = d.Tbl_Gral_Municipio.Municipio,
                                 Direccion = d.Direccion
                             }).FirstOrDefault();
                if (datos_dpi != null)
                {
                    consulta_DPI.cod_respuesta = 1;
                    consulta_DPI.datos_dpi = datos_dpi;
                }
                else
                {
                    consulta_DPI.cod_respuesta = 0;
                }

            }
            else
            {
                consulta_DPI.cod_respuesta = 2;
            }
            return Json(JsonConvert.SerializeObject(consulta_DPI));
        }

        public JsonResult RegistrarHallazgo(Tbl_RNF_Registro_BitacoraHallazgo model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No posee una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }
            bool EsInterno = false;
            if (objUs.EsInterno == 1)
            {
                EsInterno = true;
            }
            bool ErroresEncontrados = false;

            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Where(Obj => Obj.No_Registro == model.No_Registro).FirstOrDefault();
            if (tbl_RNF_Registro == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "Registro no encontrado"
                };
                return Json(jsonRespuesta);
            }


            Tbl_RNF_Registro_Bitacora tbl_RNF_Registro_Bitacora = (from d in db.Tbl_RNF_Registro_Bitacora
                                                                   where d.No_Registro == model.No_Registro
                                                                   && d.Solicitud_id == model.Solicitud_id
                                                                   && d.Bitacora_id == model.Bitacora_id
                                                                   select d).FirstOrDefault();
            if (tbl_RNF_Registro == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 3,
                    Mensaje = "Bitácora no encontrada"
                };
                return Json(jsonRespuesta);
            }


            if ((model.DescripcionHallazgo == null) || (model.DescripcionHallazgo.Trim() == ""))
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 4,
                    Mensaje = "La descripción del hallazgo no puede estar en blanco"
                };
                return Json(jsonRespuesta);
            }

            int correlativoid = 0;
            try
            {
                correlativoid = db.Tbl_RNF_Registro_BitacoraHallazgo.Where(Obj => Obj.No_Registro == tbl_RNF_Registro.No_Registro && Obj.Bitacora_id == tbl_RNF_Registro_Bitacora.Bitacora_id).Max(Obj => Obj.Correlativo_id);
            }
            catch (Exception ex)
            {
                correlativoid = 0;
            }
            correlativoid++;


            Tbl_RNF_Registro_BitacoraHallazgo tbl_RNF_Registro_BitacoraHallazgo = new Tbl_RNF_Registro_BitacoraHallazgo()
            {
                No_Registro = tbl_RNF_Registro_Bitacora.No_Registro,
                No_RegistroLiteral = tbl_RNF_Registro_Bitacora.No_RegistroLiteral,
                No_RegistroCorrelativo = tbl_RNF_Registro_Bitacora.No_RegistroCorrelativo,
                Solicitud_id = tbl_RNF_Registro_Bitacora.Solicitud_id,
                Bitacora_id = tbl_RNF_Registro_Bitacora.Bitacora_id,
                Correlativo_id = correlativoid,
                DescripcionHallazgo = model.DescripcionHallazgo,
                swdatecreated = DateTime.Now,
                swcreatedby = objUs.intUsuario_id,
                swupdatedbyinterno = EsInterno
            };

            db.Tbl_RNF_Registro_BitacoraHallazgo.Add(tbl_RNF_Registro_BitacoraHallazgo);
            db.SaveChanges();
            jsonRespuesta = new JsonRespuesta()
            {
                Result = 1,
                Mensaje = "Hallazgo registrado exitosamente"
            };

            return Json(jsonRespuesta);
        }

        public JsonResult EliminarHallazgo(Tbl_RNF_Registro_BitacoraHallazgo model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se ha encontrado el hallazgo, verifique nuevamente por favor"
            };
            Tbl_RNF_Registro_BitacoraHallazgo tbl_RNF_Registro_BitacoraHallazgo = (from d in db.Tbl_RNF_Registro_BitacoraHallazgo
                                                                                   where d.No_Registro == model.No_Registro
                                                                                   && d.Bitacora_id == model.Bitacora_id
                                                                                   && d.Correlativo_id == model.Correlativo_id
                                                                                   select d).FirstOrDefault();

            if (tbl_RNF_Registro_BitacoraHallazgo != null)
            {
                db.Tbl_RNF_Registro_BitacoraHallazgo.Remove(tbl_RNF_Registro_BitacoraHallazgo);
                db.SaveChanges();

                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 1,
                    Mensaje = "Hallazgo eliminado"
                };
            }

            return Json(jsonRespuesta);
        }

        public JsonResult EliminarDocumentosCambioEstado(Tbl_RNF_Registro_BitacoraDocumento model)
        {
            JsonRespuesta jsonRespuesta = new JsonRespuesta()
            {
                Result = 0,
                Mensaje = "No se encontró una sesión válida"
            };
            Usuario objUs = new Usuario();
            RespuestaValidaSesion objSesion = new RespuestaValidaSesion(Session[Constants.session_User]);
            if (!objSesion.getBlSession())
            {
                ViewBag.NotifyType = 2; //0 'info', 1 'success', 2 'warning', 3 'danger'
                ViewBag.Mensaje = objSesion.getStrMensaje();
                return Json(jsonRespuesta);
            }
            else
            {
                objUs = (Usuario)Session["User"];
            }

            Tbl_RNF_Registro_BitacoraDocumento tbl_RNF_Registro_BitacoraDocumento = (from d in db.Tbl_RNF_Registro_BitacoraDocumento
                                                                                     where d.No_Registro == model.No_Registro
                                                                                     && d.Bitacora_id == model.Bitacora_id
                                                                                     && d.Documento_id == model.Documento_id
                                                                                     select d).FirstOrDefault();

            if (tbl_RNF_Registro_BitacoraDocumento == null)
            {
                jsonRespuesta = new JsonRespuesta()
                {
                    Result = 2,
                    Mensaje = "No se encontró el documento en la bitácora"
                };
                return Json(jsonRespuesta);
            }
            else
            {
                try
                {
                    string partialpath = "~/Archivos_Subidos/" + tbl_RNF_Registro_BitacoraDocumento.No_Registro + "/Bitacora_" + tbl_RNF_Registro_BitacoraDocumento.Bitacora_id + "/";
                    string Nombre_Archivo = tbl_RNF_Registro_BitacoraDocumento.NombreArchivo;
                    string PathArchivo = Path.Combine(Server.MapPath(partialpath), Nombre_Archivo);
                    if (System.IO.File.Exists(PathArchivo))
                    {
                        System.IO.File.Delete(PathArchivo);
                    }
                    db.Tbl_RNF_Registro_BitacoraDocumento.Remove(tbl_RNF_Registro_BitacoraDocumento);
                    db.SaveChanges();
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 1,
                        Mensaje = "Documento eliminado exitosamente"
                    };
                }
                catch (Exception ex)
                {
                    jsonRespuesta = new JsonRespuesta()
                    {
                        Result = 3,
                        Mensaje = "Ocurrió un error: " + ex.Message
                    };
                }
            }



            return Json(jsonRespuesta);
        }

    }
}
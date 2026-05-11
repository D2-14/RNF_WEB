using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class Tbl_RNF_RegistroController : Controller
    {
        private db_RNFEntities db = new db_RNFEntities();

        // GET: Tbl_RNF_Registro
        public ActionResult Index()
        {
            var tbl_RNF_Registro = db.Tbl_RNF_Registro.Include(t => t.Tbl_Gral_Region).Include(t => t.Tbl_Gral_SolicitudConfiguracionTipo).Include(t => t.Tbl_Gral_SubRegion).Include(t => t.Tbl_RNF_Empresa_Entidad).Include(t => t.Tbl_RNF_Registro_Estado).Include(t => t.Tbl_RNF_Registro_Inactivacion_Tipo).Include(t => t.Tbl_RNF_Registro_InactivacionTecnico_Tipo).Include(t => t.Tbl_Sol_Solicitud_Categoria).Include(t => t.Tbl_Sol_Solicitud_Sub_Categoria).Include(t => t.Tbl_RNF_TecnicoProfesional);
            return View(tbl_RNF_Registro.ToList());
        }

        // GET: Tbl_RNF_Registro/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Find(id);
            if (tbl_RNF_Registro == null)
            {
                return HttpNotFound();
            }
            return View(tbl_RNF_Registro);
        }

        // GET: Tbl_RNF_Registro/Create
        public ActionResult Create()
        {
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "No_Region");
            ViewBag.SolicitudTipo_id = new SelectList(db.Tbl_Gral_SolicitudConfiguracionTipo, "SolicitudTipo_id", "Descripcion");
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_SubRegion, "Region_id", "No_SubRegion");
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_Empresa_Entidad, "No_Registro", "Nombre");
            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado, "Estado_id", "Descripcion");
            ViewBag.Categoria_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo, "Categoria_id", "Descripcion");
            ViewBag.InactivacionTecnicoTipo_id = new SelectList(db.Tbl_RNF_Registro_InactivacionTecnico_Tipo, "InactivacionTecnicoTipo_id", "Descripcion");
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion");
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Categoria_id", "Descripcion");
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_TecnicoProfesional, "No_Registro", "Nombres");
            return View();
        }

        // POST: Tbl_RNF_Registro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "No_Registro,No_RegistroLiteral,No_RegistroCorrelativo,Expediente,Region_id,SubRegion_id,Categoria_id,Sub_Categoria_id,Sub_Sub_Categoria_id,Guid_id,Estado_id,Fecha_De_Vencimiento,swdatecreated,swcreatedby,swcreatedbyinterno,swdateupdated,swupdatedby,swupdatedbyinterno,Old_RegistroId_Identity,Old_TipoRegistroId,Old_RegistradorId,UsuarioExterno_id,Solicitud_id,GuidSolicitud_id,DPI_Titular,AreaTotalFincas,TipoInactivacion_id,Descripcion_Inactivacion,ResolucionInscripcion,ResolucionInscripcionFecha,ResolucionUltimaActualizacion,ResolucionUltimaActualizacionFecha,ResolucionUltimaInactivacion,ResolucionUltimaInactivacionFecha,ConstanciaFirmada,SolicitudTipo_id,FechaModificacionDisponible,Procedencia_Probosque,Procedencia_PinpepOld,Procedencia_PinpepNew,Procedencia_secorf,Procedencia_Expediente,Procedencia_POA,Procedencia_Licencia,Procedencia_Modalidad,Procedencia_Fase,Procedencia_NombreSolicitante,Procedencia_TipoProyecto,Procedencia_InformeTecnico,Procedencia_FechaInicioPeriodo,Procedencia_FechaFinPeriodo,Fecha_Inscripcion,Fecha_Actualizacion,Fecha_Inactivacion,InactivacionTecnicoTipo_id,Descripcion_InactivacionTecnico,Bitacora_id,Notificacion_Direccion,Notificacion_Municipio_id,Notificacion_Departamento_id")] Tbl_RNF_Registro tbl_RNF_Registro)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_RNF_Registro.Add(tbl_RNF_Registro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "No_Region", tbl_RNF_Registro.Region_id);
            ViewBag.SolicitudTipo_id = new SelectList(db.Tbl_Gral_SolicitudConfiguracionTipo, "SolicitudTipo_id", "Descripcion", tbl_RNF_Registro.SolicitudTipo_id);
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_SubRegion, "Region_id", "No_SubRegion", tbl_RNF_Registro.Region_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_Empresa_Entidad, "No_Registro", "Nombre", tbl_RNF_Registro.No_Registro);
            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado, "Estado_id", "Descripcion", tbl_RNF_Registro.Estado_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.InactivacionTecnicoTipo_id = new SelectList(db.Tbl_RNF_Registro_InactivacionTecnico_Tipo, "InactivacionTecnicoTipo_id", "Descripcion", tbl_RNF_Registro.InactivacionTecnicoTipo_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_TecnicoProfesional, "No_Registro", "Nombres", tbl_RNF_Registro.No_Registro);
            return View(tbl_RNF_Registro);
        }

        // GET: Tbl_RNF_Registro/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Find(id);
            if (tbl_RNF_Registro == null)
            {
                return HttpNotFound();
            }
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "No_Region", tbl_RNF_Registro.Region_id);
            ViewBag.SolicitudTipo_id = new SelectList(db.Tbl_Gral_SolicitudConfiguracionTipo, "SolicitudTipo_id", "Descripcion", tbl_RNF_Registro.SolicitudTipo_id);
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_SubRegion, "Region_id", "No_SubRegion", tbl_RNF_Registro.Region_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_Empresa_Entidad, "No_Registro", "Nombre", tbl_RNF_Registro.No_Registro);
            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado, "Estado_id", "Descripcion", tbl_RNF_Registro.Estado_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.InactivacionTecnicoTipo_id = new SelectList(db.Tbl_RNF_Registro_InactivacionTecnico_Tipo, "InactivacionTecnicoTipo_id", "Descripcion", tbl_RNF_Registro.InactivacionTecnicoTipo_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_TecnicoProfesional, "No_Registro", "Nombres", tbl_RNF_Registro.No_Registro);
            return View(tbl_RNF_Registro);
        }

        // POST: Tbl_RNF_Registro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "No_Registro,No_RegistroLiteral,No_RegistroCorrelativo,Expediente,Region_id,SubRegion_id,Categoria_id,Sub_Categoria_id,Sub_Sub_Categoria_id,Guid_id,Estado_id,Fecha_De_Vencimiento,swdatecreated,swcreatedby,swcreatedbyinterno,swdateupdated,swupdatedby,swupdatedbyinterno,Old_RegistroId_Identity,Old_TipoRegistroId,Old_RegistradorId,UsuarioExterno_id,Solicitud_id,GuidSolicitud_id,DPI_Titular,AreaTotalFincas,TipoInactivacion_id,Descripcion_Inactivacion,ResolucionInscripcion,ResolucionInscripcionFecha,ResolucionUltimaActualizacion,ResolucionUltimaActualizacionFecha,ResolucionUltimaInactivacion,ResolucionUltimaInactivacionFecha,ConstanciaFirmada,SolicitudTipo_id,FechaModificacionDisponible,Procedencia_Probosque,Procedencia_PinpepOld,Procedencia_PinpepNew,Procedencia_secorf,Procedencia_Expediente,Procedencia_POA,Procedencia_Licencia,Procedencia_Modalidad,Procedencia_Fase,Procedencia_NombreSolicitante,Procedencia_TipoProyecto,Procedencia_InformeTecnico,Procedencia_FechaInicioPeriodo,Procedencia_FechaFinPeriodo,Fecha_Inscripcion,Fecha_Actualizacion,Fecha_Inactivacion,InactivacionTecnicoTipo_id,Descripcion_InactivacionTecnico,Bitacora_id,Notificacion_Direccion,Notificacion_Municipio_id,Notificacion_Departamento_id")] Tbl_RNF_Registro tbl_RNF_Registro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_RNF_Registro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_Region, "Id_Region", "No_Region", tbl_RNF_Registro.Region_id);
            ViewBag.SolicitudTipo_id = new SelectList(db.Tbl_Gral_SolicitudConfiguracionTipo, "SolicitudTipo_id", "Descripcion", tbl_RNF_Registro.SolicitudTipo_id);
            ViewBag.Region_id = new SelectList(db.Tbl_Gral_SubRegion, "Region_id", "No_SubRegion", tbl_RNF_Registro.Region_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_Empresa_Entidad, "No_Registro", "Nombre", tbl_RNF_Registro.No_Registro);
            ViewBag.Estado_id = new SelectList(db.Tbl_RNF_Registro_Estado, "Estado_id", "Descripcion", tbl_RNF_Registro.Estado_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_RNF_Registro_Inactivacion_Tipo, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.InactivacionTecnicoTipo_id = new SelectList(db.Tbl_RNF_Registro_InactivacionTecnico_Tipo, "InactivacionTecnicoTipo_id", "Descripcion", tbl_RNF_Registro.InactivacionTecnicoTipo_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.Categoria_id = new SelectList(db.Tbl_Sol_Solicitud_Sub_Categoria, "Categoria_id", "Descripcion", tbl_RNF_Registro.Categoria_id);
            ViewBag.No_Registro = new SelectList(db.Tbl_RNF_TecnicoProfesional, "No_Registro", "Nombres", tbl_RNF_Registro.No_Registro);
            return View(tbl_RNF_Registro);
        }

        // GET: Tbl_RNF_Registro/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Find(id);
            if (tbl_RNF_Registro == null)
            {
                return HttpNotFound();
            }
            return View(tbl_RNF_Registro);
        }

        // POST: Tbl_RNF_Registro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Tbl_RNF_Registro tbl_RNF_Registro = db.Tbl_RNF_Registro.Find(id);
            db.Tbl_RNF_Registro.Remove(tbl_RNF_Registro);
            db.SaveChanges();
            return RedirectToAction("Index");
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

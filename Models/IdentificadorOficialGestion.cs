using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class Result_SP_IdentificadorOficialGestion
    {
        public int respuesta { get; set; }
        public string Identificador { get; set; }
        public string Version { get; set; }
        public DateTime Fecha { get; set; }
        public string strFecha { get; set; }
        public string Codigo { get; set; }
        public long id { get; set; }

    } 
    public class Result_SP_CorrelativoResolucionesArchivo
    {
        public int respuesta { get; set; }
        public string strCorrelativo { get; set; }

    }

    public class IdentificadorOficialGestion
    {
        private db_RNFEntities db = new db_RNFEntities();

        public class Params_SP_Sol_IdentificadorOficialGestion
        {
            public int GestionTipo_id { get; set; }
            public long Solicitud_id { get; set; }
            public int Etapa_id { get; set; }
            public decimal EtapaRuta_id { get; set; }
            public int CorrelativoEtapa_id { get; set; }
            public long Usuario_id { get; set; }
        }

        public class Params_SP_CorrelativoResolucionesArchivo
        {
      
            public long Solicitud_id { get; set; }           
            public long Usuario_id { get; set; }
        }

        private Params_SP_Sol_IdentificadorOficialGestion model = new Params_SP_Sol_IdentificadorOficialGestion();

        private Params_SP_CorrelativoResolucionesArchivo model1 = new Params_SP_CorrelativoResolucionesArchivo();

        private Result_SP_IdentificadorOficialGestion Exec_SP_Sol_IdentificadorOficialGestion(Params_SP_Sol_IdentificadorOficialGestion model)
        {
            string sqlQuery;
            SqlParameter[] sqlParams;

            Result_SP_IdentificadorOficialGestion result_SP_IdentificadorOficialGestion = new Result_SP_IdentificadorOficialGestion()
            {
                id = 0,
                Identificador = "Fallo desconocido",
                respuesta = 0,
                Version = null,
                Fecha = DateTime.Now,
                strFecha = null,
                Codigo = null
            };

            sqlQuery = "Exec SP_Sol_IdentificadorOficialGestion @GestionTipo_id, @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Usuario_id";
            sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@GestionTipo_id",  Value = model.GestionTipo_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = model.Solicitud_id, Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@Etapa_id",  Value = model.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = model.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = model.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@Usuario_id",  Value = model.Usuario_id, Direction = System.Data.ParameterDirection.Input}
            };
            try
            {
                result_SP_IdentificadorOficialGestion = db.Database.SqlQuery<Result_SP_IdentificadorOficialGestion>(sqlQuery, sqlParams).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result_SP_IdentificadorOficialGestion;
        }


        private Result_SP_CorrelativoResolucionesArchivo Exec_SP_CorrelativoResolucionesArchivo(Params_SP_CorrelativoResolucionesArchivo model1)
        {
            string sqlQuery;
            SqlParameter[] sqlParams;

            Result_SP_CorrelativoResolucionesArchivo result_SP_CorrelativoResolucionesArchivo = new Result_SP_CorrelativoResolucionesArchivo()
            {
               
                respuesta = 0,
                strCorrelativo = null
            };

            sqlQuery = "Exec SP_CorrelativoResolucionesArchivo  @Solicitud_id, @Usuario_id";
            sqlParams = new SqlParameter[]
            {
               
                new SqlParameter { ParameterName = "@Solicitud_id",  Value = model1.Solicitud_id, Direction = System.Data.ParameterDirection.Input},                
                new SqlParameter { ParameterName = "@Usuario_id",  Value = model1.Usuario_id, Direction = System.Data.ParameterDirection.Input}
            };
            try
            {
                result_SP_CorrelativoResolucionesArchivo = db.Database.SqlQuery<Result_SP_CorrelativoResolucionesArchivo>(sqlQuery, sqlParams).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result_SP_CorrelativoResolucionesArchivo;
        }

        private Params_SP_Sol_IdentificadorOficialGestion ConvertParams_Model(int GestionTipo_id, long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            model.GestionTipo_id = GestionTipo_id;
            model.Solicitud_id = Solicitud_id;
            model.Etapa_id = Etapa_id;
            model.EtapaRuta_id = EtapaRuta_id;
            model.CorrelativoEtapa_id = CorrelativoEtapa_id;
            model.Usuario_id = Usuario_id;
            return model;
        }

        private Params_SP_CorrelativoResolucionesArchivo ConvertParams_Model(long Solicitud_id, long Usuario_id)
        {

            model1.Solicitud_id = Solicitud_id;
            model1.Usuario_id = Usuario_id;
            return model1;
        }
        public Result_SP_IdentificadorOficialGestion ObtenerNumeroOficio(int GestionTipo_id, long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            //1	                Oficio Jurídico
            //2	                Oficio Técnico
            //3	                Oficio Sub-regional
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(GestionTipo_id, Solicitud_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerNumeroInformeTecnico(long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            //4	                Informe técnico
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(4, Solicitud_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerNumeroResolucionSubRegional(long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            //5	                Resolución SubRegional de Aprobación
            //6	                Resolución SubRegional de Denegación
            Tbl_Gest_EtapaSolicitud tbl_Gest_EtapaSolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == Solicitud_id && Obj.Etapa_id == Etapa_id && Obj.EtapaRuta_id == EtapaRuta_id && Obj.CorrelativoEtapa_id == CorrelativoEtapa_id).FirstOrDefault();
            int gestiontipoid = 0;
            bool resolucionaprobada = true;
            bool resoluciondenegada = false;
            decimal parteDecimal = EtapaRuta_id - Math.Truncate(EtapaRuta_id);

            if (tbl_Gest_EtapaSolicitud != null)
            {
                resolucionaprobada = tbl_Gest_EtapaSolicitud.Resolucion_Aprobada ?? true;
                resoluciondenegada = tbl_Gest_EtapaSolicitud.Resolucion_Denegada ?? false;
                
               if (parteDecimal == 0.06M)
               {
                   gestiontipoid = 19;
               }
               else
                { 
                    if (resolucionaprobada && !resoluciondenegada)
                    {
                        gestiontipoid = 5;
                    }
                    else if (!resolucionaprobada && resoluciondenegada)
                    {
                        gestiontipoid = 6;
                    }
               }
            }
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(gestiontipoid, Solicitud_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerNumeroResolucionSubRegionalAbandono(long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            //7	                Resolución por abandono
            int gestiontipoid = 7;
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(gestiontipoid, Solicitud_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerNumeroResolucionArchivo(long Solicitud_id, long Usuario_id)
        {
            //18    Resolucion de Archivo  CR
            int gestiontipoid = 18;
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(gestiontipoid, Solicitud_id, 0, 0, 0, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerNumeroDeExpediente(long Solicitud_id, long Usuario_id)
        {
            //8	                Constancia de Recepcion de Documentos
            int gestiontipoid = 8;
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(gestiontipoid, Solicitud_id, 0, 0, 0, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_IdentificadorOficialGestion ObtenerDatosDocumentos(int GestionTipo_id, long Solicitud_id, int Etapa_id, decimal EtapaRuta_id, int CorrelativoEtapa_id, long Usuario_id)
        {
            //9	                Constancia de Recepcion de Documentos
            Params_SP_Sol_IdentificadorOficialGestion privateModel = ConvertParams_Model(GestionTipo_id, Solicitud_id, Etapa_id, EtapaRuta_id, CorrelativoEtapa_id, Usuario_id);
            return Exec_SP_Sol_IdentificadorOficialGestion(privateModel);
        }

        public Result_SP_CorrelativoResolucionesArchivo Correlativo(long Solicitud_id, long Usuario_id)
        {

            Params_SP_CorrelativoResolucionesArchivo privateModel = ConvertParams_Model(Solicitud_id, Usuario_id);
            return Exec_SP_CorrelativoResolucionesArchivo(privateModel);
        }

    }
}

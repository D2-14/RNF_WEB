using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RNF_Web.Models
{
    public class Gest_EtapaModel : Controller
    {

        public ResultFromStoreProcedure ConfirmarRespuesta(Usuario objUs, long solicitud_id, int etapa_id, decimal etaparuta_id, int correlativoetapa_id, string strMotivo, int Respuestaid, string NoOficio = null)
        {
            ResultFromStoreProcedure resultFromStoreProcedure = new ResultFromStoreProcedure
            {
                respuesta = 0,
                mensaje = "No se ha realizado ninguna gestión",
                id = -1,
            };
             db_RNFEntities db_ = new db_RNFEntities();
            Tbl_Sol_Solicitud tbl_sol_solicitud = db_.Tbl_Sol_Solicitud.Find(solicitud_id);

            Tbl_Gest_Etapa Tbl_Gest_Etapa = db_.Tbl_Gest_Etapa.Where(obj => obj.EtapaRuta_id==etaparuta_id && obj.Etapa_id==etapa_id ).FirstOrDefault();


          
            if ((tbl_sol_solicitud.Solicitud_NumeroExpediente == null) && (Tbl_Gest_Etapa.Nombre_Etapa == "Impresión de constancia y entrega de la misma al usuario."))

            {
                return resultFromStoreProcedure = new ResultFromStoreProcedure
                {
                    respuesta = 0,
                    mensaje = "No se ha impreso la constancia",
                    id = 0,
                };
            }

            try
            {

                Tbl_Gest_EtapaSolicitud tbl_gest_etapasolicitud = new Tbl_Gest_EtapaSolicitud();


                using (db_RNFEntities db = new db_RNFEntities())
                {
                    if (etapa_id == 26)
                    {
                        tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id).FirstOrDefault();

                    }
                    else
                    {
                     tbl_gest_etapasolicitud = db.Tbl_Gest_EtapaSolicitud.Where(Obj => Obj.Solicitud_id == solicitud_id && Obj.Etapa_id == etapa_id && Obj.EtapaRuta_id == etaparuta_id && Obj.CorrelativoEtapa_id == correlativoetapa_id).FirstOrDefault();

                    }

                    if (tbl_gest_etapasolicitud.Respuesta_id != 0)
                    {
                        return resultFromStoreProcedure = new ResultFromStoreProcedure
                        {
                            respuesta = 0,
                            mensaje = "No hay respuesta aún",
                            id = 0,
                        };

                    }

                    tbl_gest_etapasolicitud.Motivo = strMotivo;

                    if (ModelState.IsValid)
                    {

                        tbl_gest_etapasolicitud.swupdatedby = objUs.intUsuario_id;
                        tbl_gest_etapasolicitud.swdateupdated = DateTime.Now;

                        if (objUs.EsInterno != 1)
                        {
                            tbl_gest_etapasolicitud.swupdatedbyinterno = false;

                        }
                        else
                        {
                            tbl_gest_etapasolicitud.swupdatedbyinterno = true;

                        }

                        db.Entry(tbl_gest_etapasolicitud).State = EntityState.Modified;
                        db.SaveChanges();
                    }



                    string sqlQuery;
                    SqlParameter[] sqlParams;

                    // Inserta en la cola de casillero el documento.
                    //   Respuesta = 0  significa que no informa a casillero
                    //   Respesta = 1   Informar a casillero.
                    //   MensajeId contiene el registro ID, con ese registro hay que consultar el casillero. Si aplica.


                  

                    sqlQuery = "Exec SP_Gest_EtapaCasillero_EVO    @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id	";

                        sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                        new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input }
                    };


                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();



                    //if (resultFromStoreProcedure.respuesta == 1 && Respuestaid != 1)
                    if (resultFromStoreProcedure.respuesta == 1)
                    {
                        //Aplica enviar a casillero y verificar si fue enviado.
                        CasilleroElectronicoSender casilleroElectronicoSender = new CasilleroElectronicoSender();
                        Reply reply = casilleroElectronicoSender.EnviarCasilleroPendiente();

                        Tbl_Cas_CasilleroElectronicoCola tbl_Cas_CasilleroElectronicoCola = db.Tbl_Cas_CasilleroElectronicoCola.Find(resultFromStoreProcedure.mensaje);

                        if ((tbl_Cas_CasilleroElectronicoCola.Estado ?? false) == false)
                        {
                            return new ResultFromStoreProcedure
                            {
                                respuesta = 0,
                                mensaje = tbl_Cas_CasilleroElectronicoCola.Respuesta,
                                id = Convert.ToInt64(tbl_Cas_CasilleroElectronicoCola.Estado),
                            };
                        }

                    }


                    if (etapa_id == 26)
                    {
                        sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno,@NoOficio";
                        sqlParams = new SqlParameter[]
                      {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@NoOficio",  Value = NoOficio, Direction = System.Data.ParameterDirection.Input }
                      };
                    }
                    else
                    {

                        sqlQuery = "Exec SP_Gest_EtapaRespuesta @Solicitud_id, @Etapa_id, @EtapaRuta_id, @CorrelativoEtapa_id, @Respuesta_id, @swupdatedby, @swupdatedbyinterno	";

                        sqlParams = new SqlParameter[]
                            {
                       new SqlParameter { ParameterName = "@Solicitud_id",  Value = tbl_gest_etapasolicitud.Solicitud_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Etapa_id",  Value = tbl_gest_etapasolicitud.Etapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@EtapaRuta_id",  Value = tbl_gest_etapasolicitud.EtapaRuta_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@CorrelativoEtapa_id",  Value = tbl_gest_etapasolicitud.CorrelativoEtapa_id, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@Respuesta_id",  Value = Respuestaid, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedby",  Value = objUs.intUsuario_id, Direction = System.Data.ParameterDirection.Input },
                       //new SqlParameter { ParameterName = "@swupdatedby",  Value = 1014, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@swupdatedbyinterno",  Value = objUs.EsInterno, Direction = System.Data.ParameterDirection.Input }
                            };
                    }

                    // SP_Gest_EtapaCasillero_EVO (llamado arriba) y SP_Gest_EtapaCasillero (llamado
                    // internamente por SP_Gest_EtapaRespuesta) ambos insertan en
                    // Tbl_Cas_CasilleroElectronicoCola con el mismo registroId, causando violación
                    // de llave primaria. Este DELETE elimina la entrada previa para que
                    // SP_Gest_EtapaRespuesta pueda insertarla correctamente.
                    // Corrección definitiva: restaurar la condición AND @Contador = 0 en
                    // SP_Gest_EtapaCasillero (bloque ELSE), que fue comentada accidentalmente.
                    try
                    {
                        db.Database.ExecuteSqlCommand(
                            "DELETE FROM Tbl_Cas_CasilleroElectronicoCola WHERE registroId = @p0 OR Solicitud_id = @p1",
                            new SqlParameter("@p0", tbl_sol_solicitud.No_Registro ?? ""),
                            new SqlParameter("@p1", solicitud_id));
                    }
                    catch (Exception exClean)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] Limpieza casillero cola falló (no crítico): {exClean.Message}");
                    }

                    resultFromStoreProcedure = db.Database.SqlQuery<ResultFromStoreProcedure>(sqlQuery, sqlParams).FirstOrDefault();

                    System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] SP_Gest_EtapaRespuesta resultado | etapa_id={etapa_id} | respuesta={resultFromStoreProcedure?.respuesta} | mensaje={resultFromStoreProcedure?.mensaje}");

                    // *** LLAMADA A SEINEF: al autorizar en etapa 12, se sincronizan los datos de la empresa con la API de SEINEF ***
                    if (etapa_id == 12 && resultFromStoreProcedure != null && resultFromStoreProcedure.respuesta == 1)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] Etapa 12 autorizada → iniciando sincronización SEINEF | No_Registro={tbl_sol_solicitud.No_Registro}");
                        try
                        {
                            bool seinefOk = SeinefService.EnviarEmpresa(tbl_sol_solicitud.No_Registro, solicitud_id);
                            System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] SEINEF resultado={seinefOk} | No_Registro={tbl_sol_solicitud.No_Registro}");
                        }
                        catch (Exception exSeinef)
                        {
                            System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] SEINEF EXCEPCION | {exSeinef.GetType().Name}: {exSeinef.Message}");
                        }
                    }
                    // *** FIN LLAMADA A SEINEF ***

                    try
                    {
                        Whatsapper whatsapper = new Whatsapper();
                        _ = whatsapper.EnviarWhatsappPendiente();
                    }
                    catch { }
                    try
                    {
                        Models.Mailer mailer = new Models.Mailer();
                        _ = mailer.EnviarCorreoPendiente(objUs);
                    }
                    catch { }

                    if ((resultFromStoreProcedure.respuesta != 0) && (resultFromStoreProcedure.respuesta != 1))
                    {
                        resultFromStoreProcedure.respuesta = 0;
                    }

                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RNF-ETAPA] EXCEPCION OUTER | {ex.GetType().Name}: {ex.Message} | Inner: {ex.InnerException?.Message}");
                resultFromStoreProcedure = new ResultFromStoreProcedure
                {
                    respuesta = 0,
                    mensaje = "Ocurrió un error... " + ex.Message,
                    id = -1,
                };
            }

            //catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            //{
            //    foreach (var eve in ex.EntityValidationErrors)
            //    {
            //        Console.WriteLine($"Entidad: {eve.Entry.Entity.GetType().Name} - Estado: {eve.Entry.State}");
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine($"  Propiedad: {ve.PropertyName}, Error: {ve.ErrorMessage}");
            //        }
            //    }
            //    throw; // opcional, relanza la excepción
            //}

            return resultFromStoreProcedure;
        }

    }
}
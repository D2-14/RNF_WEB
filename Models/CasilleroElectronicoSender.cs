using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class CasilleroElectronicoSender
    {
        public Reply EnviarCasilleroPendiente()
        {
            Reply reply = new Reply();

            using (db_RNFEntities db = new db_RNFEntities())
            {
                RequestUtil requestUtil = new RequestUtil();
                bool?[] estatusPendiente = new bool?[] { null, false };


                List<Tbl_Cas_CasilleroElectronicoCola> tbl_Cas_CasilleroElectronicoColas = ((from d in db.Tbl_Cas_CasilleroElectronicoCola
                                                                                             where estatusPendiente.Contains(d.Estado)
                                                                                             && (d.registroId ?? "").Trim() != ""
                                                                                             && (d.expediente ?? "").Trim() != ""
                                                                                             && (d.casillero ?? -1) != -1
                                                                                             && (d.tipoNotificacionId ?? -1) != -1
                                                                                             && (d.sistemaId ?? -1) != -1
                                                                                             && (d.cui ?? "").Trim() != ""
                                                                                             && (d.subregionId ?? -1) != -1
                                                                                             && (d.regionId ?? -1) != -1
                                                                                             && (d.Intentos ?? 0) < 3
                                                                                             select d).ToList() ?? new List<Tbl_Cas_CasilleroElectronicoCola>());
                if (tbl_Cas_CasilleroElectronicoColas.Count() > 0)
                {
                    int count = 0;
                    foreach (var item in tbl_Cas_CasilleroElectronicoColas)
                    {
                        item.Intentos = (item.Intentos ?? 0);
                        item.Respuesta = (item.Respuesta ?? "");



                        CasilleroElectronicoResponse casilleroElectronicoResponse = requestUtil.Execute_CasilleroElectronicoGlobal(Constants.Address_AgregarNotificacionCasillero, "POST", item);

                        if (casilleroElectronicoResponse.status == 1)
                        {
                            count++;
                            item.Estado = true;
                        }
                        else
                        {
                            item.Estado = false;
                        }

                        item.Intentos = item.Intentos + 1;
                        item.Respuesta = "id status: " + casilleroElectronicoResponse.status + " respuesta: " + casilleroElectronicoResponse.message;

                        item.swdateupdated = DateTime.Now;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    if ((count > 0))
                    {
                        reply = new Reply
                        {
                            result = 1,
                            message = "Se actualizaron " + count + " de " + tbl_Cas_CasilleroElectronicoColas.Count() + " notificaciones de casillero electrónico pendientes",
                        };
                    }
                    else
                    {
                        reply = new Reply
                        {
                            result = 2,
                            message = "No se pudo actualizar las notificaciones pendientes",
                        };
                    }
                }
            }

            return reply;
        }
    }
}
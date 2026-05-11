using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class ObtenerConexion
    {
        public static string getConexionUsuario()
        {
            //Obtenemos los datos de conexión de la base de datos que necesitemos
            String cadena_conexion =
                        ConfigurationManager.ConnectionStrings["db_RNFEntities"].ToString();

            SqlConnectionStringBuilder builder = new
            SqlConnectionStringBuilder(cadena_conexion.Split('"')[1]);
            String servidor = builder.DataSource;
            string base_de_dato = builder.InitialCatalog;
            bool autentificacionWindows = builder.IntegratedSecurity;
            String usuario = builder.UserID;
            String cadenaValida = builder.ConnectionString;


            return usuario;

        }

    }
}
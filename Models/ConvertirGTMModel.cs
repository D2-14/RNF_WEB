using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class CoordenadasRequest
    {
        public string Coordenadas { get; set; }
        public decimal Longitud { get; set; }
        public decimal Latitud { get; set; }
        public decimal GTMX { get; set; }
        public decimal GTMY { get; set; }
    }
    public class CoordenadasResponse
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public ConvertirGTMModel.GR_GTM Coordenadas { get; set; }
    }
    public class ConvertirGTMModel
    {
        db_RNF_APIEntities db_API = new db_RNF_APIEntities();
        db_RNFEntities db = new db_RNFEntities();

        public class GR_GTM
        {
            public decimal GR_Longitud { get; set; }
            public decimal GR_Latitud { get; set; }
            public decimal GTM_X { get; set; }
            public decimal GTM_Y { get; set; }
        }

        GR_GTM oGR_GTM = new GR_GTM();
        public GR_GTM Convertir_GTM_2_GR(decimal GTM_X, decimal GTM_Y)
        {
            string iResultado;
            int iLongGrad, iLongMin, iLongSec, iLatGrad, iLatMin, iLatSec;

            /* CONSTANTES */
            decimal e, c, mc;

            e = 0.00673949674227624M;
            c = 6399593.62575849M;
            mc = -90.5M;

            /* CALCULOS */
            decimal fi, Ni, a, A1, A2, J2, J4, J6, Alfa, Beta, Gamma, Bfi, b, Zeta, Xi, Eta, SenHXi, DeltaLambda, Tau, fiRad, LongDec, LatDec;

            fi = GTM_Y / (6366197.724M * 0.9998M);
            Ni = (c / DecimalMath.Power((1M + e * DecimalMath.Power(DecimalMath.Cos(fi), 2M)), 0.5M)) * 0.9998M;

            a = (GTM_X - 500000M) / Ni;
            A1 = DecimalMath.Sin(2M * fi);

            A2 = A1 * DecimalMath.Power(DecimalMath.Cos(fi), 2M);
            J2 = fi + (A1 / 2M);
            J4 = (3M * J2 + A2) / 4M;
            J6 = (5M * J4 + A2 * DecimalMath.Power(DecimalMath.Cos(fi), 2M)) / 3M;
            Alfa = (3M / 4M) * e;
            Beta = (5M / 3M) * DecimalMath.Power(Alfa, 2M);
            Gamma = (35M / 27M) * DecimalMath.Power(Alfa, 3M);
            Bfi = 0.9998M * c * (fi - (Alfa * J2) + (Beta * J4) - (Gamma * J6));
            b = (GTM_Y - Bfi) / Ni;
            Zeta = (e * DecimalMath.Power(a, 2M) / 2M) * DecimalMath.Power(DecimalMath.Cos(fi), 2M);
            Xi = a * (1M - (Zeta / 3M));

            Eta = b * (1M - Zeta) + fi;
            SenHXi = (DecimalMath.Exp(Xi) - DecimalMath.Exp(-Xi)) / 2M;
            DeltaLambda = DecimalMath.ATan(SenHXi / DecimalMath.Cos(Eta));
            Tau = DecimalMath.ATan(DecimalMath.Cos(DeltaLambda) * DecimalMath.Tan(Eta));
            fiRad = fi + (1M + e * DecimalMath.Power(DecimalMath.Cos(fi), 2M) - (decimal)(1.5) * e * DecimalMath.Sin(fi) * DecimalMath.Cos(fi) * (Tau - fi)) * (Tau - fi);
            LongDec = (DeltaLambda / DecimalMath.Pi) * 180M + mc;
            LatDec = (fiRad / DecimalMath.Pi) * 180M;

            oGR_GTM.GTM_X = GTM_X;
            oGR_GTM.GTM_Y = GTM_Y;
            oGR_GTM.GR_Latitud = LatDec;
            oGR_GTM.GR_Longitud = LongDec;

            return oGR_GTM;
        }
        public GR_GTM Convertir_GR_2_GTM(decimal grLongitud, decimal grLatitud)
        {
            //Constantes en Hoja 5
            decimal
                Hoja5_a_semiejemayor,
                Hoja5_b_semiejemenor,
                Hoja5_Excentricidad,
                Hoja5_2Excentricidad_e,
                Hoja5_e2,
                Hoja5_c_radio_polar_de_curvatura;

            Hoja5_a_semiejemayor = 6378137;
            Hoja5_b_semiejemenor = (decimal)6356752.314;
            Hoja5_Excentricidad = DecimalMath.Sqrt((DecimalMath.Power(Hoja5_a_semiejemayor, 2) - DecimalMath.Power(Hoja5_b_semiejemenor, 2)), 2) / Hoja5_a_semiejemayor;
            Hoja5_2Excentricidad_e = DecimalMath.Sqrt((DecimalMath.Power(Hoja5_a_semiejemayor, 2) - DecimalMath.Power(Hoja5_b_semiejemenor, 2)), 2) / Hoja5_b_semiejemenor;
            Hoja5_e2 = DecimalMath.Power(Hoja5_2Excentricidad_e, 2);
            Hoja5_c_radio_polar_de_curvatura = (DecimalMath.Power(Hoja5_a_semiejemayor, 2)) / Hoja5_b_semiejemenor;
            //Variables en Hoja 5
            decimal
                Hoja5_Longitud, Hoja5_Latitud, Hoja5_LongitudRadianes, Hoja5_LatitudRadianes,
                Hoja5_Calculo_Huso, Hoja5_Meridiano_Huso,
                Hoja5_Delta_Lambda, Hoja5_A, Hoja5_Xi, Hoja5_Eta, Hoja5_Ni, Hoja5_Zeta,
                Hoja5_A1, Hoja5_A2, Hoja5_J2, Hoja5_J4, Hoja5_J6,
                Hoja5_Alfa, Hoja5_Beta, Hoja5_Gamma, Hoja5_B_fi,
                Hoja5_UTM_Este_X, Hoja5_UTM_Norte_Y;
            //Operación en Hoja 5
            Hoja5_Longitud = grLongitud;
            Hoja5_Latitud = grLatitud;

            Hoja5_LongitudRadianes = Hoja5_Longitud * DecimalMath.Pi / 180;
            Hoja5_LatitudRadianes = Hoja5_Latitud * DecimalMath.Pi / 180;

            Hoja5_Calculo_Huso = Math.Truncate((Hoja5_Longitud / 6) + 31);
            Hoja5_Meridiano_Huso = 6 * Hoja5_Calculo_Huso - 183;

            Hoja5_Delta_Lambda = Hoja5_LongitudRadianes - ((Hoja5_Meridiano_Huso * DecimalMath.Pi) / 180);
            Hoja5_A = DecimalMath.Cos(Hoja5_LatitudRadianes) * DecimalMath.Sin(Hoja5_Delta_Lambda);
            Hoja5_Xi = (decimal)(0.5) * DecimalMath.Log((1 + Hoja5_A) / (1 - Hoja5_A));
            Hoja5_Eta = DecimalMath.ATan((DecimalMath.Tan(Hoja5_LatitudRadianes)) / (DecimalMath.Cos(Hoja5_Delta_Lambda))) - Hoja5_LatitudRadianes;
            Hoja5_Ni = (Hoja5_c_radio_polar_de_curvatura / (DecimalMath.Power(1 + Hoja5_e2 * (DecimalMath.Power((DecimalMath.Cos(Hoja5_LatitudRadianes)), 2)), (decimal)(0.5)))) * (decimal)0.9996;
            Hoja5_Zeta = (Hoja5_e2 / 2) * DecimalMath.Power(Hoja5_Xi, 2) * DecimalMath.Power((DecimalMath.Cos(Hoja5_LatitudRadianes)), 2);
            Hoja5_A1 = DecimalMath.Sin(2 * Hoja5_LatitudRadianes);
            Hoja5_A2 = Hoja5_A1 * DecimalMath.Power((DecimalMath.Cos(Hoja5_LatitudRadianes)), 2);
            Hoja5_J2 = Hoja5_LatitudRadianes + (Hoja5_A1 / 2);
            Hoja5_J4 = (decimal)((3 * Hoja5_J2) + Hoja5_A2) / 4;
            Hoja5_J6 = (decimal)(5 * Hoja5_J4 + Hoja5_A2 * (DecimalMath.Power((DecimalMath.Cos(Hoja5_LatitudRadianes)), 2))) / 3;
            Hoja5_Alfa = (decimal)(0.75) * Hoja5_e2;
            Hoja5_Beta = (decimal)(1.6666666666666666666666666666667) * DecimalMath.Power(Hoja5_Alfa, 2);
            Hoja5_Gamma = (decimal)(1.2962962962962962962962962962963) * DecimalMath.Power(Hoja5_Alfa, 3);
            Hoja5_B_fi = (decimal)0.9996 * Hoja5_c_radio_polar_de_curvatura * (Hoja5_LatitudRadianes - (Hoja5_Alfa * Hoja5_J2) + (Hoja5_Beta * Hoja5_J4) - (Hoja5_Gamma * Hoja5_J6));

            Hoja5_UTM_Este_X = Hoja5_Xi * Hoja5_Ni * (decimal)(1 + Hoja5_Zeta / 3) + 500000;
            Hoja5_UTM_Norte_Y = Hoja5_Eta * Hoja5_Ni * (1 + Hoja5_Zeta) + Hoja5_B_fi;

            //Constantes en Hoja WGS84-GTM
            decimal
                WGS84_GTM_a_semiejemayor,
                WGS84_GTM_b_semiejemenor,
                WGS84_GTM_Excentricidad,
                WGS84_GTM_2Excentricidad_e,
                WGS84_GTM_e2,
                WGS84_GTM_c_radio_polar_de_curvatura;

            WGS84_GTM_a_semiejemayor = 6378137;
            WGS84_GTM_b_semiejemenor = (decimal)6356752.31424518;
            WGS84_GTM_Excentricidad = (decimal)0.0818191908426203;
            WGS84_GTM_2Excentricidad_e = (decimal)0.0820944379496945;
            WGS84_GTM_e2 = (decimal)0.00673949674227624;
            WGS84_GTM_c_radio_polar_de_curvatura = (decimal)6399593.62575849;

            //Variables en Hoja WGS84-GTM
            decimal
                WGS84_GTM_UTM_Este_X,
                WGS84_GTM_UTM_Norte_Y,
                WGS84_GTM_Huso, WGS84_GTM_Meridiano_Central, WGS84_GTM_Y_AlSurDelEcuador,
                WGS84_GTM_Fi, WGS84_GTM_Ni1, WGS84_GTM_a, WGS84_GTM_A11, WGS84_GTM_A21, WGS84_GTM_J21, WGS84_GTM_J41, WGS84_GTM_J61,
                WGS84_GTM_Alfa1, WGS84_GTM_Beta1, WGS84_GTM_Gamma1, WGS84_GTM_B_fi1, WGS84_GTM_b, WGS84_GTM_Zeta1,
                WGS84_GTM_Xi1, WGS84_GTM_Eta1, WGS84_GTM_Sen_h_Xi,
                WGS84_GTM_Delta_Lambda1, WGS84_GTM_Tau, WGS84_GTM_Fi_Radianes,
                WGS84_GTM_Longitud_Sexas_Decimales, WGS84_GTM_Latitud_Sexas_Decimales,
                WGS84_GTM_Longitud_Grados, WGS84_GTM_Longitud_Minutos, WGS84_GTM_Longitud_Segundos,
                WGS84_GTM_Latitud_Grados, WGS84_GTM_Latitud_Minutos, WGS84_GTM_Latitud_Segundos,
                WGS84_GTM_Longitud_Radianes, WGS84_GTM_Latitud_Radianes,
                WGS84_GTM_Calculo_Huso, WGS84_GTM_Meridiano_Huso,
                WGS84_GTM_Delta_Lambda2,
                WGS84_GTM_A,
                WGS84_GTM_Xi2, WGS84_GTM_Eta2, WGS84_GTM_Ni2,
                WGS84_GTM_Zeta2, WGS84_GTM_A12, WGS84_GTM_A22, WGS84_GTM_J22, WGS84_GTM_J42, WGS84_GTM_J62,
                WGS84_GTM_Alfa2, WGS84_GTM_Beta2, WGS84_GTM_Gamma2, WGS84_GTM_B_fi2,
                WGS84_GTM_GTM_Este_X,
                WGS84_GTM_GTM_Norte_Y;


            //Operación en Hoja WGS84-GTM
            WGS84_GTM_UTM_Este_X = Hoja5_UTM_Este_X;
            WGS84_GTM_UTM_Norte_Y = Hoja5_UTM_Norte_Y;
            WGS84_GTM_Huso = 15;
            WGS84_GTM_Meridiano_Central = 6 * WGS84_GTM_Huso - 183;
            WGS84_GTM_Y_AlSurDelEcuador = WGS84_GTM_UTM_Norte_Y;
            WGS84_GTM_Fi = (WGS84_GTM_Y_AlSurDelEcuador) / (decimal)(6366197.724 * 0.9996);
            WGS84_GTM_Ni1 = (WGS84_GTM_c_radio_polar_de_curvatura / (DecimalMath.Power(1 + WGS84_GTM_e2 * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Fi), 2)), (decimal)(0.5)))) * (decimal)0.9996;
            WGS84_GTM_a = (WGS84_GTM_UTM_Este_X - 500000) / WGS84_GTM_Ni1;
            WGS84_GTM_A11 = DecimalMath.Sin(2 * WGS84_GTM_Fi);
            WGS84_GTM_A21 = WGS84_GTM_A11 * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Fi), 2));
            WGS84_GTM_J21 = WGS84_GTM_Fi + (WGS84_GTM_A11 / 2M);
            WGS84_GTM_J41 = (3 * WGS84_GTM_J21 + WGS84_GTM_A21) / 4;
            WGS84_GTM_J61 = (5 * WGS84_GTM_J41 + WGS84_GTM_A21 * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Fi), 2))) / 3;
            WGS84_GTM_Alfa1 = (decimal)(0.75) * WGS84_GTM_e2;
            WGS84_GTM_Beta1 = (decimal)(1.6666666666666666666666666666667) * (DecimalMath.Power(WGS84_GTM_Alfa1, 2));
            WGS84_GTM_Gamma1 = (decimal)(1.2962962962962962962962962962963) * (DecimalMath.Power(WGS84_GTM_Alfa1, 3));
            WGS84_GTM_B_fi1 = (decimal)0.9996 * WGS84_GTM_c_radio_polar_de_curvatura * (WGS84_GTM_Fi - (WGS84_GTM_Alfa1 * WGS84_GTM_J21) + (WGS84_GTM_Beta1 * WGS84_GTM_J41) - (WGS84_GTM_Gamma1 * WGS84_GTM_J61));
            WGS84_GTM_b = (WGS84_GTM_Y_AlSurDelEcuador - WGS84_GTM_B_fi1) / WGS84_GTM_Ni1;
            WGS84_GTM_Zeta1 = ((WGS84_GTM_e2 * DecimalMath.Power(WGS84_GTM_a, 2)) / 2) * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Fi), 2));
            WGS84_GTM_Xi1 = WGS84_GTM_a * (1 - (WGS84_GTM_Zeta1 / 3));
            WGS84_GTM_Eta1 = WGS84_GTM_b * (1 - WGS84_GTM_Zeta1) + WGS84_GTM_Fi;
            WGS84_GTM_Sen_h_Xi = (DecimalMath.Exp(WGS84_GTM_Xi1) - DecimalMath.Exp(-WGS84_GTM_Xi1)) / 2;
            WGS84_GTM_Delta_Lambda1 = DecimalMath.ATan(WGS84_GTM_Sen_h_Xi / DecimalMath.Cos(WGS84_GTM_Eta1));
            WGS84_GTM_Tau = DecimalMath.ATan(DecimalMath.Cos(WGS84_GTM_Delta_Lambda1) * DecimalMath.Tan(WGS84_GTM_Eta1));
            WGS84_GTM_Fi_Radianes = WGS84_GTM_Fi + (1 + WGS84_GTM_e2 * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Fi), 2) - (decimal)(1.5) * WGS84_GTM_e2 * DecimalMath.Sin(WGS84_GTM_Fi) * DecimalMath.Cos(WGS84_GTM_Fi) * (WGS84_GTM_Tau - WGS84_GTM_Fi))) * (WGS84_GTM_Tau - WGS84_GTM_Fi);
            WGS84_GTM_Longitud_Sexas_Decimales = (WGS84_GTM_Delta_Lambda1 / DecimalMath.Pi) * 180 + WGS84_GTM_Meridiano_Central;
            WGS84_GTM_Latitud_Sexas_Decimales = (WGS84_GTM_Fi_Radianes / DecimalMath.Pi) * 180;
            WGS84_GTM_Longitud_Grados = Math.Truncate(WGS84_GTM_Longitud_Sexas_Decimales);
            WGS84_GTM_Longitud_Minutos = Math.Truncate((WGS84_GTM_Longitud_Sexas_Decimales - WGS84_GTM_Longitud_Grados) * 60);
            WGS84_GTM_Longitud_Segundos = (((WGS84_GTM_Longitud_Sexas_Decimales - WGS84_GTM_Longitud_Grados) * 60) - WGS84_GTM_Longitud_Minutos) * 60;
            WGS84_GTM_Latitud_Grados = Math.Truncate(WGS84_GTM_Latitud_Sexas_Decimales);
            WGS84_GTM_Latitud_Minutos = Math.Truncate((WGS84_GTM_Latitud_Sexas_Decimales - WGS84_GTM_Latitud_Grados) * 60);
            WGS84_GTM_Latitud_Segundos = (((WGS84_GTM_Latitud_Sexas_Decimales - WGS84_GTM_Latitud_Grados) * 60) - WGS84_GTM_Latitud_Minutos) * 60;
            WGS84_GTM_Longitud_Radianes = WGS84_GTM_Longitud_Sexas_Decimales * DecimalMath.Pi / 180;
            WGS84_GTM_Latitud_Radianes = WGS84_GTM_Latitud_Sexas_Decimales * DecimalMath.Pi / 180;
            WGS84_GTM_Calculo_Huso = Math.Truncate((WGS84_GTM_Longitud_Sexas_Decimales / 6) + 31);
            WGS84_GTM_Meridiano_Huso = -(decimal)90.5;
            WGS84_GTM_Delta_Lambda2 = WGS84_GTM_Longitud_Radianes - ((WGS84_GTM_Meridiano_Huso * DecimalMath.Pi) / 180);
            WGS84_GTM_A = DecimalMath.Cos(WGS84_GTM_Latitud_Radianes) * DecimalMath.Sin(WGS84_GTM_Delta_Lambda2);
            WGS84_GTM_Xi2 = (decimal)(0.5) * DecimalMath.Log((1 + WGS84_GTM_A) / (1 - WGS84_GTM_A));
            WGS84_GTM_Eta2 = DecimalMath.ATan((DecimalMath.Tan(WGS84_GTM_Latitud_Radianes)) / (DecimalMath.Cos(WGS84_GTM_Delta_Lambda2))) - WGS84_GTM_Latitud_Radianes;
            WGS84_GTM_Ni2 = (WGS84_GTM_c_radio_polar_de_curvatura / (DecimalMath.Power(1 + WGS84_GTM_e2 * ((DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Latitud_Radianes), 2))), (decimal)(0.5)))) * (decimal)0.9998;
            WGS84_GTM_Zeta2 = (WGS84_GTM_e2 / 2) * DecimalMath.Power(WGS84_GTM_Xi2, 2) * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Latitud_Radianes), 2));
            WGS84_GTM_A12 = DecimalMath.Sin(2 * WGS84_GTM_Latitud_Radianes);
            WGS84_GTM_A22 = WGS84_GTM_A12 * (DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Latitud_Radianes), 2));
            WGS84_GTM_J22 = WGS84_GTM_Latitud_Radianes + (WGS84_GTM_A12 / 2);
            WGS84_GTM_J42 = ((3 * WGS84_GTM_J22) + WGS84_GTM_A22) / 4;
            WGS84_GTM_J62 = (5 * WGS84_GTM_J42 + WGS84_GTM_A22 * ((DecimalMath.Power(DecimalMath.Cos(WGS84_GTM_Latitud_Radianes), 2)))) / 3;
            WGS84_GTM_Alfa2 = (decimal)(0.75) * WGS84_GTM_e2;
            WGS84_GTM_Beta2 = (decimal)(1.6666666666666666666666666666667) * DecimalMath.Power(WGS84_GTM_Alfa2, 2);
            WGS84_GTM_Gamma2 = (decimal)(1.2962962962962962962962962962963) * DecimalMath.Power(WGS84_GTM_Alfa2, 3);
            WGS84_GTM_B_fi2 = (decimal)0.9998 * WGS84_GTM_c_radio_polar_de_curvatura * (WGS84_GTM_Latitud_Radianes - (WGS84_GTM_Alfa2 * WGS84_GTM_J22) + (WGS84_GTM_Beta2 * WGS84_GTM_J42) - (WGS84_GTM_Gamma2 * WGS84_GTM_J62));
            WGS84_GTM_GTM_Este_X = WGS84_GTM_Xi2 * WGS84_GTM_Ni2 * (1 + WGS84_GTM_Zeta2 / 3) + 500000;
            WGS84_GTM_GTM_Norte_Y = WGS84_GTM_Eta2 * WGS84_GTM_Ni2 * (1 + WGS84_GTM_Zeta2) + WGS84_GTM_B_fi2;

            oGR_GTM.GR_Longitud = Hoja5_Longitud;
            oGR_GTM.GR_Latitud = Hoja5_Latitud;
            oGR_GTM.GTM_X = WGS84_GTM_GTM_Este_X;
            oGR_GTM.GTM_Y = WGS84_GTM_GTM_Norte_Y;
            return oGR_GTM;

        }

        public GR_GTM Convertir_GTM_SQL_GR(decimal GTM_X, decimal GTM_Y)
        {
            fc_API_GetLongitudLatitud_Result gtmtogr = (from d in db_API.fc_API_GetLongitudLatitud(GTM_X, GTM_Y)
                                                        select d).FirstOrDefault();
            oGR_GTM.GTM_X = GTM_X;
            oGR_GTM.GTM_Y = GTM_Y;
            oGR_GTM.GR_Latitud = (decimal)gtmtogr.Latitud;
            oGR_GTM.GR_Longitud = (decimal)gtmtogr.Longitud;
            return oGR_GTM;
        }


        public string ObtenerDireccionGoogleMaps(decimal GTM_X, decimal GTM_Y)
        {
            string sqlQuery;
            SqlParameter[] sqlParams;
            string url;

            sqlQuery = "select dbo.[Fnc_generar_direcciongooglemaps](@GTM_X, @GTM_Y)";

            sqlParams = new SqlParameter[]
            {
                       new SqlParameter { ParameterName = "@GTM_X",  Value = GTM_X, Direction = System.Data.ParameterDirection.Input },
                       new SqlParameter { ParameterName = "@GTM_Y",  Value = GTM_Y, Direction = System.Data.ParameterDirection.Input }
            };


            url = db.Database.SqlQuery<string>(sqlQuery, sqlParams).FirstOrDefault();
            if (url == null)
            {
                url = "";
            }
            return url;
        }

    }
}
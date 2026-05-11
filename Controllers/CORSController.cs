using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using RNF_Web.Models;

namespace RNF_Web.Controllers
{
    public class CORSController : Controller
    {
        string strParentGoogleDrive = "1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB";
        string Address_Bearer = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/login/authenticate";
        string Address_GoogleDriveUpload = "http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive";
        string RNF_Username = "rnf";
        string RNF_Password = "NWJjEr9Q.3+w_rM=";
        
        // GET: CORS
        //public ActionResult Index()
        //{
        //    //nothing();
        //    // "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJuZiIsIm5iZiI6MTY0NTQ2NzMyMiwiZXhwIjoxNjQ1NDk2MTIyLCJpYXQiOjE2NDU0NjczMjIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAyIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDIifQ.XZ_wnJUrak0MIOXQuXsoivAtlWKnPb1oPLMk_Rgw7Tk",
        //    getFile();

        //    firmarFile();

        //    getFile();

        //    string strBearer = GetBearer();

        //    string strDocumentoSubido = CallCORS(strBearer, @"C:\Temporal_II\PDF Test.pdf");

        //    return View();
        //}

        public string GetBearer()
        {
            var client = new RestClient(Address_Bearer);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
                        " + "\n" +
                        @"""Username"":""rnf"",
                        " + "\n" +
                        @"""Password"":""NWJjEr9Q.3+w_rM=""
                        " + "\n" +
                        @"}
                        " + "\n" +
                        @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return "Bearer " + response.Content.ToString().Replace("\"", "");

        }

        public string CallCORS(string strSign, string strPath)
        {
            var client = new RestClient(Address_GoogleDriveUpload);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", strSign);
            //request.AddFile("nombre", "/C:/Temporal_II/PDF Test.pdf");
            request.AddFile("nombre", strPath, "application/pdf");
            request.AddParameter("parentGoogleDriveId", strParentGoogleDrive);
            IRestResponse response = client.Execute(request);
            JObject joResponse = JObject.Parse(response.Content);

            return joResponse["GoogleDriveId"].ToString();

        }

        public void getFile()
        {
            var client = new RestClient("http://" + Constants.IP_FirmaElectronica + "/Api_RNF/api/GoogleDrive/1MEF1HbEosWdgqnF614s6ttZ09S72e6r6");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJuZiIsIm5iZiI6MTY0NTU3NDk0MCwiZXhwIjoxNjQ1NjAzNzQwLCJpYXQiOjE2NDU1NzQ5NDAsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAyIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDIifQ.1WMvQJFQf0eiaqi86O36Frm_ChJheNTLW6PwpN5toUo");
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            byte[] buffer = client.DownloadData(request);


            MemoryStream ms = new MemoryStream(buffer);
            FileStream file = new FileStream(@"C:\temp2022.pdf", FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);
            file.Close();
            ms.Close();


            // IRestResponse response = client.Execute(request);

        }

        //public void firmarFile()
        //{

        //    var client = new RestClient("http://170.239.56.109/Api_RNF/api/FirmaElectronica");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJuZiIsIm5iZiI6MTY0NTU3NDk0MCwiZXhwIjoxNjQ1NjAzNzQwLCJpYXQiOjE2NDU1NzQ5NDAsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAyIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDIifQ.1WMvQJFQf0eiaqi86O36Frm_ChJheNTLW6PwpN5toUo");
        //    request.AddHeader("Content-Type", "application/json");
        //    var body = @"{
        //        " + "\n" +
        //                    @"  ""User"": """",
        //        " + "\n" +
        //                    @"  ""Password"": """",
        //        " + "\n" +
        //                    @"  ""parentGoogleDriveId"":""1tbsVY5xJOQYDWf00lJtYxG8keNlUeiGB"",
        //        " + "\n" +
        //                    @"  ""Documentos"":[
        //        " + "\n" +
        //                    @"    {
        //        " + "\n" +
        //                    @"      ""GoogleDriveId"": ""1WTUjj_rrv2KF2m4ov8MHXrJ46g1n4MTE"",
        //        " + "\n" +
        //                        @"      ""Coordenadas"": ""70,650,180,710"",

        //        " + "\n" +
        //                    @"      ""NumeroPagina"": 1
        //        " + "\n" +
        //                    @"    }
        //        " + "\n" +
        //                    @" ]
        //        " + "\n" +
        //    @"}";

        //    //@"      ""Coordenadas"": ""30,30,60,60"",

        //    request.AddParameter("application /json", body, ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);
        //    Console.WriteLine(response.Content);

        //}


    }
}

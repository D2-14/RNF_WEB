namespace RNF_Web.Models
{
    public class ResultFromStoreProcedure
    {
        public int respuesta { get; set; }
        public string mensaje { get; set; }
        public long id { get; set; }
    }

    public class ResultFromStoreProcedure2
    {
        public int respuesta { get; set; }
        public string mensaje { get; set; }
        public long id { get; set; }
        public long id2 { get; set; }
    }

    public class ResultFromStoreProcedureWhatsapp
    {
        public int result { get; set; }
        public string message { get; set; }
        public string data { get; set; }
        public bool details { get; set; }
    }
}
namespace RNF_Web.Models
{
    public class RespuestaValidaSesion
    {
        private bool blSession = false;
        private string strDireccion;
        private string strMensaje = null;

        private Usuario objUsuario;

        public RespuestaValidaSesion(object objUsuario)
        {
            this.objUsuario = (Usuario)objUsuario;

            ValidarSession();
        }

        private void AsignarRespuesta(bool blSession, string strDireccion, string strMensaje)
        {
            this.blSession = blSession;
            this.strDireccion = strDireccion;
            this.strMensaje = strMensaje;
        }

        private void ValidarSession()
        {
            Usuario objUs = (Usuario)objUsuario;

            if (objUs != null)
            {
                AsignarRespuesta(true, null, null);
            }
            else
            {
                AsignarRespuesta(false, "../Login/Index", null);
            }
        }

        public bool getBlSession()
        {
            return blSession;
        }

        public string getStrDireccion()
        {
            return strDireccion;
        }

        public string getStrMensaje()
        {
            return strMensaje;
        }

    }
}
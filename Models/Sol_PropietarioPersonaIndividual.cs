using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_PropietarioPersonaIndividual
    {

        public class urlmetadata
        {
            [Display(Name = "Nombre(s) del propietario")]
            [Required(ErrorMessage = "Debe indicar el nombre(s) del propietario.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Nombres { get; set; }

            [Display(Name = "Apellido(s) del propietario")]
            [Required(ErrorMessage = "Debe indicar el apellido(s) del propietario.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Apellidos { get; set; }



            [Display(Name = "Número de celular")]
            [Required(ErrorMessage = "Debe el número de celular del propietario.")]
            [RegularExpression(@"\d*", ErrorMessage = "El celular solamente debe contener números")]
            public string Celular { get; set; }

            [Display(Name = "Número de teléfono")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "El teléfono debe contener solamente números.")]
            public string Telefono { get; set; }

            //[RegularExpression("^[0-9]{10,19}$", ErrorMessage = "El número de documento no debe contener guiones o espacios en blanco. La longitud no es la esperada.")]
            [Display(Name = "Número de documento")]
            [Required(ErrorMessage = "Debe el número de documento de identificación.")]
            public string No_Documento { get; set; }


            [Display(Name = "Correo electrónico")]
            [EmailAddress(ErrorMessage = "Direccion de email invalida")]
            public string Email { get; set; }

            [Display(Name = "Pueblo de pertenencia")]
            [Range(1, int.MaxValue, ErrorMessage = "Por favor indique el pueblo de pertenencia")]
            public int PuebloPertenencia_id { get; set; }

            [Display(Name = "Sexo")]
            [Range(1, int.MaxValue, ErrorMessage = "Por favor indique el sexo del propietario")]
            public int Sexo_id { get; set; }

            [Display(Name = "Número de NIT ó DPI")]
            public string No_NIT { get; set; }


            [Display(Name = "Municipio")]
            public string MunicipioDPI_id { get; set; }

            [Display(Name = "Departamento de emisión del documento de identificación")]
            public string DepartamentoDPI_id { get; set; }


            [Display(Name = "Documento identificación")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique el tipo de documento de identificación")]
            public int DocumentoID_Tipo { get; set; }


        }

    }
}




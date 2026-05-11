using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{
    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Sol_RepresentanteLegal
    {

        public class urlmetadata
        {

            [Display(Name = "Nombre(s) del representante legal")]
            [Required(ErrorMessage = "Debe indicar el nombre(s) del propietario.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s-]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Nombres { get; set; }

            [Display(Name = "Apellido(s) del representante legal")]
            [Required(ErrorMessage = "Debe indicar el apellido(s) del propietario.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s-]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Apellidos { get; set; }

            [Display(Name = "Tipo de representante")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique el tipo de representación legal.")]
            public int RepresentanteLegatTipo_id { get; set; }

            [Display(Name = "Documento de identificación")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique el documento de identificación.")]
            public int RepresententeDocumentoID_Tipo { get; set; }

            //[RegularExpression("^[0-9]{5,13}$", ErrorMessage = "El número de documento no debe contener guiones o espacios en blanco. La longitud esperada es de 13 dígitos.")]
            [Display(Name = "Número de documento")]
            [Required(ErrorMessage = "Debe indicar el número de documento.")]
            public string RepresentanteNo_Documento { get; set; }

            [Display(Name = "Número de NIT")]
            public int No_NIT { get; set; }

            [Display(Name = "Fecha de Nacimiento")]
            public int Fecha_Nacimiento { get; set; }

            [Display(Name = "Fecha de Inicio de Nombramiento")]
            public int Fecha_InicioNombramiento { get; set; }

            [Display(Name = "Fecha de Fin de Nombramiento")]
            public int Fecha_FinNombramiento { get; set; }

        }
    }
}








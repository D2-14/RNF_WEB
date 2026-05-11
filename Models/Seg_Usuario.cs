using System;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RNF_Web.Models
{

    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Seg_Usuario
    {

        public class urlmetadata
        {

            [Display(Name = "Correo(*) : ")]
            [Required(ErrorMessage = "Debe ingresar la dirección de email.")]
            [EmailAddress(ErrorMessage = "Direccion de email invalida")]
            public string email { get; set; }


            [Display(Name = "Nombres (*)")]
            [Required(ErrorMessage = "Debe indicar su nombre, no acepta dígitos o carácteres especiales.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Nombre { get; set; }

            [Display(Name = "Apellidos (*)")]
            [Required(ErrorMessage = "Debe indicar su nombre, no acepta dígitos o carácteres especiales.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Apellidos { get; set; }



            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            //[StringLength(25, ErrorMessage = "La Contraseña debe tener entre 8 y 25 caracteres de longitud.", MinimumLength = 8)]
            // Sin caracter especial [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Debe contener mayusculas, minusculas, numeros y al menos un caracter especial.")]
            // Complejidad alta, más de un caracter especial "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\]*+\\/|!\"£$%^&*()#[@~'?><,.=_-]).{6,}$"
            // Unicamente un caracter especial @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$"
            // Letras espacio, coma y punto, tildes      [RegularExpression(@"^[a-zA-Zá-ú.,\s]+$", ErrorMessage = "No se aceptan números en la razon social")]

            //[RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "La Contraseña debe contener mayusculas, minusculas, numeros y al menos un caracter especial.")]
            //[DataType(DataType.Password)]
            [Display(Name = "Clave")]
            public string PalabraClave { get; set; }

            [StringLength(8, MinimumLength = 8, ErrorMessage = "La longitud esperada del teléfono celular es de 8 dígitos..")]
            [RegularExpression(@"\d*", ErrorMessage = "El celular solamente debe contener números")]
            [Display(Name = "Teléfono celular")]
            public string whatsapp { get; set; }


        }
    }

}


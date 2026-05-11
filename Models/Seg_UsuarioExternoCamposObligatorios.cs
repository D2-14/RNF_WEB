using System;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;


namespace RNF_Web.Models
{

    [MetadataType(typeof(urlmetadata))]
    public partial class Tbl_Seg_UsuarioExterno
    {
      
        public class urlmetadata
        {

            [Display(Name = "Correo (*)")]
            [Required(ErrorMessage = "Debe ingresar la dirección de email.")]
            [EmailAddress(ErrorMessage = "Direccion de email invalida")]
            public string Correo { get; set; }


            [Display(Name = "Nombres (*)")]
            [Required(ErrorMessage = "Debe indicar su nombre, no acepta dígitos o carácteres especiales.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Nombres{ get; set; }

            [Display(Name = "Apellidos (*)")]
            [Required(ErrorMessage = "Debe indicar su nombre, no acepta dígitos o carácteres especiales.")]
            [RegularExpression(@"^[a-zñáéíóúÑÁÉÍÓÚÜü,A-Z\s]+$", ErrorMessage = "En este campo, no se aceptan números ni caracteres especiales.")]
            public string Apellidos { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(25, ErrorMessage = "La Contraseña debe tener entre 8 y 25 caracteres de longitud.", MinimumLength = 8)]
            // Sin caracter especial [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Debe contener mayusculas, minusculas, numeros y al menos un caracter especial.")]
            // Complejidad alta, más de un caracter especial "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\]*+\\/|!\"£$%^&*()#[@~'?><,.=_-]).{6,}$"
            // Unicamente un caracter especial @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$"
            // Letras espacio, coma y punto, tildes      [RegularExpression(@"^[a-zA-Zá-ú.,\s]+$", ErrorMessage = "No se aceptan números en la razon social")]

            [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "La Contraseña debe contener mayusculas, minusculas, numeros y al menos un caracter especial.")]
            [DataType(DataType.Password)]
            [Display(Name = "Clave (*)")]
            public string Clave { get; set; }

            [StringLength(8, MinimumLength = 8, ErrorMessage = "La longitud esperada del teléfono celular es de 8 dígitos..")]
            [RegularExpression(@"\d*", ErrorMessage = "El celular solamente debe contener números")]
            [Display(Name = "Teléfono celular (*)")]
            public string Telefono_Celular { get; set; }

            [RegularExpression("^[0-9]*$", ErrorMessage = "El telefono debe contener solamente números.")]
            [Display(Name = "Teléfono de oficina")]
            public string Telefono_Oficina { get; set; }

            [RegularExpression("^[0-9]*$", ErrorMessage = "El número de extension debe contener solamente números.")]
            [Display(Name = "Número de extensión")]
            public string Telefono_Oficina_Extension { get; set; }

            [Display(Name = "Pueblo de pertenencia (*)")]
            [Range(1, int.MaxValue, ErrorMessage = "Por favor indique el pueblo de pertenencia")]
            public int PuebloPertenencia_id { get; set; }

            [Display(Name = "Sexo (*)")]
            [Range(1, int.MaxValue, ErrorMessage = "Por favor indique el sexo del propietario")]
            public int Sexo_id { get; set; }

            [Display(Name = "Documento identificación (*)")]
            [Range(1, int.MaxValue, ErrorMessage = "Indique el tipo de documento de identificación")]
            public int DocumentoID_Tipo { get; set; }

            [Display(Name = "Número de documento")]
            [Required(ErrorMessage = "Debe el número de documento de identificación.")]
            public string No_Documento { get; set; }


            [Display(Name = "Número de NIT")]
            public string No_NIT { get; set; }

            [Display(Name = "Vecindad (Departamento) (*)")]
            public string DepartamentoDPI_id { get; set; }

            [Display(Name = "Vecindad (Municipio) (*)")]
            public string MunicipioDPI_id { get; set; }

            [Display(Name = "Dirección (*)")]
            [Required(ErrorMessage = "Debe ingresar su dirección.")]
            public string Direccion { get; set; }

            [Display(Name = "Municipio (*)")]
            public string Municipio_id { get; set; }

            [Display(Name = "Departamento (*)")]
            public string Departamento_id { get; set; }


            [Display(Name = "Grado Académico Técnico")]
            public bool Grado_Academico_Tecnico { get; set; }
            
            [Display(Name = "Grado Académico Profesional")]
            public bool Grado_Academico_Profesional { get; set; }
           
            [Display(Name = "No. Colegiado")]
            public string No_Colegiado { get; set; }
            
            [Display(Name = "Post-Grado en MateriaForestal")]
            public bool PostGradoMateriaForestal { get; set; }

            [Display(Name = "Especialidad del Post-Grado")]
            public string PostGradoEspecialidad { get; set; }

            [Display(Name = "Universidad")]
            public string Universidad { get; set; }

            [Display(Name = "Profesión")]
            public int Profesion_id { get; set; }

            [Display(Name = "Universidad en donde estudió el Post-grado ")]
            public int PostGradoUniversidad { get; set; }


            [Display(Name = "Especificar carrera")]
            public int EspecificarCarrera { get; set; }

            [Display(Name = "Número de diploma")]
            public int No_De_Diploma { get; set; }


        }
    }

}


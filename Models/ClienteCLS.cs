using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiPrimerEntityFramework.Models
{
    public class ClienteCLS
    {
        [Display (Name ="Id. Cliente")]
        public int iidcliente { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        [StringLength(100,ErrorMessage ="Longitud Máxima 100")]
        public string nombre { get; set; }
        [Display(Name = "Apellido Paterno")]
        [Required]
        [StringLength(150, ErrorMessage = "Longitud Máxima 150")]
        public string appaterno { get; set; }
        [Display(Name = "Apellido Materno")]
        [Required]
        [StringLength(150, ErrorMessage = "Longitud Máxima 150")]
        public string apmaterno { get; set; }
        [Display(Name = "Email")]
        [Required]
        [StringLength(200, ErrorMessage = "Longitud Máxima 200")]
        [EmailAddress(ErrorMessage ="Ingresa un email valido")]
        public string email { get; set; }

        [Display(Name = "Dirección")]
        [DataType(DataType.MultilineText)]
        [Required]
        [StringLength(200, ErrorMessage = "Longitud Máxima 200")]
        public string direccion { get; set; }

        [Display(Name = "Sexo")]
        [Required]

        public int iidsexo { get; set; }
        
        [Display(Name = "Teléfono Fijo")]
        [Required]
        [StringLength(10, ErrorMessage = "Longitud Máxima 10")]
        public string telefonofijo { get; set; }
        [Display(Name = "Teléfono Celular")]
        [Required]
        [StringLength(10, ErrorMessage = "Longitud Máxima 10")]
        public string telefonocelular { get; set; }

        public int bhabilitado { get; set; }

        //Propiedad Adicional

        public string mensajeError { get; set; }

    }
}
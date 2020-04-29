using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiPrimerEntityFramework.Models
{
    public class tipoUsuarioCLS
    {
        [Display(Name ="Id tipo usuario")]
        public int iidtipousuario { get; set; }
        [Display(Name = "Nombre tipo usuario")]
        [Required]
        [StringLength(150,ErrorMessage ="Longitud máxima 150")]
        public string nombre { get; set; }
        [Display(Name = "Descripción tipo usuario")]
        [Required]
        [StringLength(250, ErrorMessage = "Longitud máxima 250")]
        public string descripcion { get; set; }
        public string bhabilitado { get; set; }
    }
}
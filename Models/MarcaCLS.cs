using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiPrimerEntityFramework.Models
{
    public class MarcaCLS
    {
        [Display(Name ="Id Marca")]
        public int iidmarca { get; set; }
        [Display(Name = "Nombre Marca")]
        [Required]
        [StringLength(100,ErrorMessage ="La longitud máxima es de 100")]
        public string nombre { get; set; }
        [Display(Name ="Descripción Marca")]
        [Required]
        [StringLength(200,ErrorMessage ="La longitud máxima del campo son 200")]
        public string descripcion { get; set; }
        public int bhabilitado { get; set; }

        //Añadiendo propiedad (Errores de validación)
        public string MensajeError{ get; set; }
    }
}
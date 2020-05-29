using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiPrimerEntityFramework.Models
{
    public class PaginaCLS
    {
        [Display(Name ="Id Página")]
        public int iidpagin { get; set; }
        [Required]
        [Display(Name = "Título del link")]
        public string mensaje { get; set; }
        [Required]
        [Display(Name = "Nombre de la acción")]
        public string accion { get; set; }
        [Required]
        [Display(Name = "Nombre del controlador")]
        public string controlador { get; set; }
        public int bhabilitado { get; set; }

        public string mensajeFiltrar { get; set; }
    }
}
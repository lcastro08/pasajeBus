using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class tipoUsuarioController : Controller
    {
        private tipoUsuarioCLS otipoVal;
        private bool buscarTipoUsuario(tipoUsuarioCLS otipoUsuarioCLS)
        {
            bool busquedaId = true;
            bool busquedaNombre = true;
            bool busquedaDescripcion = true;

            if (otipoVal.iidtipousuario > 0)
                busquedaId = otipoUsuarioCLS.iidtipousuario.ToString().Contains(otipoVal.iidtipousuario.ToString());

            if (otipoVal.nombre != null)
               busquedaNombre = otipoUsuarioCLS.nombre.ToString().Contains(otipoVal.nombre);

            if (otipoVal.descripcion != null)
                busquedaDescripcion = otipoUsuarioCLS.descripcion.ToString().Contains(otipoVal.descripcion);

            return (busquedaId && busquedaNombre && busquedaDescripcion);
        }
        // GET: tipoUsuario
        public ActionResult Index(tipoUsuarioCLS otipousuario)
        {
            otipoVal = otipousuario;
            List<tipoUsuarioCLS> listaTipousuario = null;
            List<tipoUsuarioCLS> listaFiltrado;
            using (var bd = new BDPasajeEntities())
            {
                listaTipousuario = (from tipoUsuario in bd.TipoUsuario
                                    where tipoUsuario.BHABILITADO == 1
                                    select new tipoUsuarioCLS
                                    {
                                        iidtipousuario = tipoUsuario.IIDTIPOUSUARIO,
                                        nombre = tipoUsuario.NOMBRE,
                                        descripcion = tipoUsuario.DESCRIPCION
                                    }).ToList();
                if (otipousuario.iidtipousuario == 0 && otipousuario.nombre == null
                    && otipousuario.descripcion == null)
                    listaFiltrado = listaTipousuario;
                else
                {
                    Predicate<tipoUsuarioCLS> pred = new Predicate<tipoUsuarioCLS>(buscarTipoUsuario);
                    listaFiltrado = listaTipousuario.FindAll(pred);
                }
            }
                return View(listaFiltrado);
        }
    }
}
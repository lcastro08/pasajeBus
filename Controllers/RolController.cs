using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimerEntityFramework.Controllers
{
    public class RolController : Controller
    {
        // GET: Rol
        public ActionResult Index()
        {
            List<RolCLS> listaRol = new List<RolCLS>();
            using (var bd = new BDPasajeEntities())
            {
                listaRol = (from rol in bd.Rol
                            where rol.BHABILITADO == 1
                            select new RolCLS
                            {
                                iidRol = rol.IIDROL,
                                nombre = rol.NOMBRE,
                                descripcion = rol.DESCRIPCION
                            }).ToList();
            }
                return View(listaRol);
        }
        public ActionResult Filtro (string nombre)
        {
            List<RolCLS> listaRol = new List<RolCLS>();
            using (var bd = new BDPasajeEntities())
            {
                if (nombre == null)
                    listaRol = (from rol in bd.Rol
                                where rol.BHABILITADO == 1
                                select new RolCLS
                                {
                                    iidRol = rol.IIDROL,
                                    nombre = rol.NOMBRE,
                                    descripcion = rol.DESCRIPCION
                                }).ToList();
                else
                {
                    listaRol = (from rol in bd.Rol
                                where rol.BHABILITADO == 1
                                && rol.NOMBRE.Contains(nombre)
                                select new RolCLS
                                {
                                    iidRol = rol.IIDROL,
                                    nombre = rol.NOMBRE,
                                    descripcion = rol.DESCRIPCION
                                }).ToList();
                }
                return PartialView("_tablaRol", listaRol);
            }
        }
        public int Guardar(RolCLS oRolCLS, int titulo)
        {
            int respuesta = 0;
            using (var bd = new BDPasajeEntities())
            { 
                if(titulo.Equals(1))
                {
                    Rol oRol = new Rol();
                    oRol.NOMBRE = oRolCLS.nombre;
                    oRol.DESCRIPCION = oRolCLS.descripcion;
                    oRol.BHABILITADO = 1;
                    bd.Rol.Add(oRol);
                    respuesta = bd.SaveChanges();
                }
            }

            return respuesta;
        }
    }
}
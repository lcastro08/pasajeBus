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
        public ActionResult Filtro (string nombreRol)
        {
            List<RolCLS> listaRol = new List<RolCLS>();
            using (var bd = new BDPasajeEntities())
            {
                if (nombreRol == null)
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
                                && rol.NOMBRE.Contains(nombreRol)
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
        public string Guardar(RolCLS oRolCLS, int titulo)
        {
            string respuesta = "";
            try { 
            if (!ModelState.IsValid) 
            {
                //Vamos a obtener los estados de cada propiedad y los mensajes de error (si es que hay).
                var query = (from state in ModelState.Values
                             from error in state.Errors
                             select error.ErrorMessage).ToList();
                respuesta += "<ul class='list-group'>";
                foreach(var item in query)
                {
                    respuesta += "<li class = 'list-group-item'>"+item+"</li>";
                }
                respuesta += "</ul>";
            }
            else
            { 
                using (var bd = new BDPasajeEntities())
                {
                    int cantidad = 0;
                    if(titulo.Equals(-1))
                    {
                        cantidad = bd.Rol.Where(p => p.NOMBRE == oRolCLS.nombre).Count();
                            //-1 el registro ya existe en la base de datos
                            if (cantidad >= 1)
                            {
                                respuesta = "-1";
                            }
                            else
                            { 
                                Rol oRol = new Rol();
                                oRol.NOMBRE = oRolCLS.nombre;
                                oRol.DESCRIPCION = oRolCLS.descripcion;
                                oRol.BHABILITADO = 1;
                                bd.Rol.Add(oRol);
                                respuesta = bd.SaveChanges().ToString();
                                if (respuesta == "0") respuesta = "";
                            }
                            
                    }
                    else
                    {
                            cantidad = bd.Rol.Where(p => p.NOMBRE == oRolCLS.nombre && p.IIDROL != titulo).Count();
                            if (cantidad >= 1)
                            {
                                respuesta = "-1";
                            }
                            else
                            { 
                                Rol oRol = bd.Rol.Where(p => p.IIDROL == titulo).First();
                                oRol.NOMBRE = oRolCLS.nombre;
                                oRol.DESCRIPCION = oRolCLS.descripcion;
                                respuesta = bd.SaveChanges().ToString();
                            }

                        }
                    }
                }

            }
            catch(Exception ex)
            {
                respuesta = "";
            }

            return respuesta;
        }

        public JsonResult recuperarDatos(int titulo)
        {
            RolCLS oRolCLS = new RolCLS();
            using (var bd = new BDPasajeEntities())
            {
                Rol oRol = bd.Rol.Where(p => p.IIDROL == titulo).First();
                oRolCLS.nombre = oRol.NOMBRE;
                oRolCLS.descripcion = oRol.DESCRIPCION;
            }
            return Json(oRolCLS,JsonRequestBehavior.AllowGet);
        }

        public string Eliminar (RolCLS oRolCLS)
        {
            //Error
            string respuesta = "";

            try
            { 
                int idRol = oRolCLS.iidRol;
                using (var bd = new BDPasajeEntities())
                {
                    Rol oRol = bd.Rol.Where(p => p.IIDROL == idRol).First();
                    oRol.BHABILITADO = 0;
                    respuesta = bd.SaveChanges().ToString();

                }
            }
            catch(Exception ex)
            {
                respuesta = "";
            }
            return respuesta;
        }
    }
}
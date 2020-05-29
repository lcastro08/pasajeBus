using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Filters;

namespace MiPrimerEntityFramework.Controllers
{
    [Acceder]
    public class RolPaginaController : Controller
    {
        // GET: RolPagina
        public ActionResult Index()
        {
            listarComboRol();
            listarComboPagina();
            List<RolPaginaCLS> listaRol = new List<RolPaginaCLS>();
            using (var bd = new BDPasajeEntities())
            {
                listaRol = (from RolPagina in bd.RolPagina
                            join rol in bd.Rol
                            on RolPagina.IIDROL equals
                            rol.IIDROL
                            join pagina in bd.Pagina
                            on RolPagina.IIDPAGINA equals
                            pagina.IIDPAGINA
                            where RolPagina.BHABILITADO == 1
                            select new RolPaginaCLS
                            {
                                iidrolpagina = RolPagina.IIDROLPAGINA,
                                nombreRol = rol.NOMBRE,
                                nombreMensaje = pagina.MENSAJE
                            }).ToList();
            }
            return View(listaRol);

        }
        public void listarComboRol()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.Rol
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.NOMBRE,
                             Value = item.IIDROL.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
                ViewBag.listaRol = lista;
            }
        }

        public void listarComboPagina()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.Pagina
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.MENSAJE,
                             Value = item.IIDPAGINA.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
                ViewBag.listaPagina = lista;
            }
        }
        public ActionResult Filtrar(int? iidrolFiltro) //int? acepta null
        {
            List<RolPaginaCLS> listaRol = new List<RolPaginaCLS>();
            using (var bd = new BDPasajeEntities())
            {
                if(iidrolFiltro == null) { 
                listaRol = (from RolPagina in bd.RolPagina
                            join rol in bd.Rol
                            on RolPagina.IIDROL equals
                            rol.IIDROL
                            join pagina in bd.Pagina
                            on RolPagina.IIDPAGINA equals
                            pagina.IIDPAGINA
                            where RolPagina.BHABILITADO == 1
                            select new RolPaginaCLS
                            {
                                iidrolpagina = RolPagina.IIDROLPAGINA,
                                nombreRol = rol.NOMBRE,
                                nombreMensaje = pagina.MENSAJE
                            }).ToList();
                }
                else
                {
                    listaRol = (from RolPagina in bd.RolPagina
                                join rol in bd.Rol
                                on RolPagina.IIDROL equals
                                rol.IIDROL
                                join pagina in bd.Pagina
                                on RolPagina.IIDPAGINA equals
                                pagina.IIDPAGINA
                                where RolPagina.BHABILITADO == 1
                                && RolPagina.IIDROL == iidrolFiltro
                                select new RolPaginaCLS
                                {
                                    iidrolpagina = RolPagina.IIDROLPAGINA,
                                    nombreRol = rol.NOMBRE,
                                    nombreMensaje = pagina.MENSAJE
                                }).ToList();
                }
            }
            return PartialView("_tableRolPagina", listaRol);

        }

        public string Guardar (RolPaginaCLS oRolPaginaCLS, int titulo)
        {
            //error
            string respuesta = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    //Vamos a obtener los estados de cada propiedad y los mensajes de error (si es que hay).
                    var query = (from state in ModelState.Values
                                 from error in state.Errors
                                 select error.ErrorMessage).ToList();
                    respuesta += "<ul class='list-group'>";
                    foreach (var item in query)
                    {
                        respuesta += "<li class = 'list-group-item'>" + item + "</li>";
                    }
                    respuesta += "</ul>";
                }
                else 
                {
            
            
                    using (var bd = new BDPasajeEntities())
                    {
                        int cantidad = 0;
                        //agregar
                        if(titulo == -1)
                        {
                            cantidad = bd.RolPagina.Where(p => p.IIDROL == oRolPaginaCLS.iidrol
                            && p.IIDPAGINA == oRolPaginaCLS.iidpagina).Count();
                            if(cantidad >= 1)
                            {
                                respuesta = "-1";
                            }
                            else
                            {
                                RolPagina oRolPagina = new RolPagina();
                                oRolPagina.IIDROL = oRolPaginaCLS.iidrol;
                                oRolPagina.IIDPAGINA = oRolPaginaCLS.iidpagina;
                                oRolPagina.BHABILITADO = 1;
                                bd.RolPagina.Add(oRolPagina);
                                respuesta = bd.SaveChanges().ToString();
                                if (respuesta == "0") respuesta = "";
                            }

                        }
                        else
                        {
                            cantidad = bd.RolPagina.Where(p => p.IIDROL == oRolPaginaCLS.iidrol
                            && p.IIDPAGINA == oRolPaginaCLS.iidpagina
                            && p.IIDROLPAGINA != titulo).Count();
                            if(cantidad >= 1)
                            {
                                respuesta = "-1";
                            }
                            else
                            { 
                                RolPagina oRolpagina = bd.RolPagina.Where(p => p.IIDROLPAGINA == titulo).First();
                                oRolpagina.IIDROL = oRolPaginaCLS.iidrol;
                                oRolpagina.IIDPAGINA = oRolPaginaCLS.iidpagina;
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

        public JsonResult recuperarInfo(int idRolPagina)
        {
            RolPaginaCLS oRolPaginaCLS = new RolPaginaCLS();
            using (var bd = new BDPasajeEntities())
            {
                RolPagina oRolPagina = bd.RolPagina.Where(p => p.IIDROLPAGINA == idRolPagina).First();
                oRolPaginaCLS.iidrol = (int)oRolPagina.IIDROL;
                oRolPaginaCLS.iidpagina = (int)oRolPagina.IIDROLPAGINA;
            }
            return Json(oRolPaginaCLS, JsonRequestBehavior.AllowGet);
        }

        public int EliminarRolPagina(int iidRolPagina)
        {
            int respuesta = 0;
            try
            {
                using (var bd = new BDPasajeEntities())
                {
                    RolPagina oRolPagina = bd.RolPagina.Where(p => p.IIDROLPAGINA == iidRolPagina).First();
                    oRolPagina.BHABILITADO = 0;
                    respuesta = bd.SaveChanges();

                }
            }
            catch
            {
                respuesta = 0;
            }
            return respuesta;
        }

    }
}
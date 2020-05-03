using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimerEntityFramework.Controllers
{
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
        public ActionResult Filtrar(int? iidrol) //int? acepta null
        {
            List<RolPaginaCLS> listaRol = new List<RolPaginaCLS>();
            using (var bd = new BDPasajeEntities())
            {
                if(iidrol == null) { 
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
                                && RolPagina.IIDROL == iidrol
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

        public int Guardar (RolPaginaCLS oRolPaginaCLS, int titulo)
        {
            int respuesta = 0;
            using (var bd = new BDPasajeEntities())
            {
                if(titulo == 1)
                {
                    RolPagina oRolPagina = new RolPagina();
                    oRolPagina.IIDROL = oRolPaginaCLS.iidrol;
                    oRolPagina.IIDPAGINA = oRolPaginaCLS.iidpagina;
                    oRolPagina.BHABILITADO = 1;
                    bd.RolPagina.Add(oRolPagina);
                    respuesta = bd.SaveChanges();
                }
            }
            return respuesta;
        }


    }
}
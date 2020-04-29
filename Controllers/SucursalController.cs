using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class SucursalController : Controller
    {
        // GET: Sucursal
        public ActionResult Index(SucursalCLS oSucursalCLS)
        {
            List<SucursalCLS> listaSucursal = null;
            string nombreSucursal = oSucursalCLS.nombre;
                using (var bd = new BDPasajeEntities())
                {
                    if (oSucursalCLS.nombre == null)
                    {
                        listaSucursal = (from Sucursal in bd.Sucursal
                                 where Sucursal.BHABILITADO == 1
                                 select new SucursalCLS
                                 {
                                     iidsucursal = Sucursal.IIDSUCURSAL,
                                     nombre = Sucursal.NOMBRE,
                                     telefono = Sucursal.TELEFONO,
                                     email = Sucursal.EMAIL
                                 }).ToList();
                    }
                    else
                    {
                        listaSucursal = (from Sucursal in bd.Sucursal
                                     where Sucursal.BHABILITADO == 1
                                     && Sucursal.NOMBRE.Contains(nombreSucursal)
                                     select new SucursalCLS
                                     {
                                         iidsucursal = Sucursal.IIDSUCURSAL,
                                         nombre = Sucursal.NOMBRE,
                                         telefono = Sucursal.TELEFONO,
                                         email = Sucursal.EMAIL
                                     }).ToList();
                }
                }
            return View(listaSucursal);
        }

        public ActionResult Editar(int id)
        {
            SucursalCLS oSucursalCLS = new SucursalCLS();
            using(var bd = new BDPasajeEntities())
            {
                Sucursal oSucural = bd.Sucursal.Where(p => p.IIDSUCURSAL.Equals(id)).First(); //El .Where siempre nos devuelve una lista, si queremos que nos devuelva un objeto ponemos .first()
                oSucursalCLS.iidsucursal = oSucural.IIDSUCURSAL;
                oSucursalCLS.nombre = oSucural.NOMBRE;
                oSucursalCLS.direccion = oSucural.DIRECCION;
                oSucursalCLS.telefono = oSucural.TELEFONO;
                oSucursalCLS.email = oSucural.EMAIL;
                oSucursalCLS.fechaapertura =(DateTime) oSucural.FECHAAPERTURA;
            }
            return View(oSucursalCLS);

        }
        [HttpPost]
        public ActionResult Editar(SucursalCLS oSucursalCLS)
        {
            int registrosEncontrados = 0;
            string nombreSucursal = oSucursalCLS.nombre;
            int idSucursal = oSucursalCLS.iidsucursal;
            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Sucursal.Where(p => p.NOMBRE.Equals(nombreSucursal) && !p.IIDSUCURSAL.Equals(idSucursal)).Count();

            }
                if (!ModelState.IsValid || registrosEncontrados>=1)
                {
                if (registrosEncontrados >= 1) oSucursalCLS.mensajeError = "Ya existe la sucursal";
                    return View(oSucursalCLS);
                }
            using (var bd = new BDPasajeEntities())
            {
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IIDSUCURSAL.Equals(idSucursal)).First();
                oSucursal.NOMBRE = oSucursalCLS.nombre;
                oSucursal.DIRECCION = oSucursalCLS.direccion;
                oSucursal.TELEFONO = oSucursalCLS.telefono;
                oSucursal.EMAIL = oSucursalCLS.email;
                oSucursal.FECHAAPERTURA =oSucursalCLS.fechaapertura;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
            
        }

        public ActionResult Agregar()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Agregar(SucursalCLS oSucursalCLS)
        {
            int registrosEncontrados = 0;
            string nombreSucursal = oSucursalCLS.nombre;
            using (var bd = new BDPasajeEntities()) 
            {
                registrosEncontrados = bd.Sucursal.Where(p => p.NOMBRE.Equals(nombreSucursal)).Count();

            }
                if (!ModelState.IsValid || registrosEncontrados>=1)
                {
                if (registrosEncontrados >= 1) oSucursalCLS.mensajeError = "Ya existe la sucursal";
                    return View(oSucursalCLS);
                }
                else
                {
                    using (var bd = new BDPasajeEntities())
                    {
                        Sucursal oSucursal = new Sucursal();
                        oSucursal.NOMBRE = oSucursalCLS.nombre;
                        oSucursal.DIRECCION = oSucursalCLS.direccion;
                        oSucursal.TELEFONO = oSucursalCLS.telefono;
                        oSucursal.EMAIL = oSucursalCLS.email;
                        oSucursal.FECHAAPERTURA = oSucursalCLS.fechaapertura;
                        oSucursal.BHABILITADO = 1;
                        bd.Sucursal.Add(oSucursal);
                        bd.SaveChanges();

                    }
                    return RedirectToAction("Index");
                }
        }

        public ActionResult Eliminar (int id)
        {
            using (var bd = new BDPasajeEntities())
            {
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IIDSUCURSAL.Equals(id)).First();
                oSucursal.BHABILITADO = 0;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
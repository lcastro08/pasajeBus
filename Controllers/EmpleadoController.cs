using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        public ActionResult Index()
        {
            List<EmpleadoCLS> listaEmpleado = null;
            using (var bd = new BDPasajeEntities())
            {
                listaEmpleado = (from empleado in bd.Empleado
                                 join tipoUsuario in bd.TipoUsuario
                                 on empleado.IIDTIPOUSUARIO equals tipoUsuario.IIDTIPOUSUARIO
                                 join tipoContrato in bd.TipoContrato
                                 on empleado.IIDTIPOCONTRATO equals tipoContrato.IIDTIPOCONTRATO
                                 where empleado.BHABILITADO ==1
                                 select new EmpleadoCLS
                                 {
                                     iidEmpleado = empleado.IIDEMPLEADO,
                                     Nombre = empleado.NOMBRE,
                                     apPaterno = empleado.APPATERNO,
                                     nombreTipoUsuario = tipoUsuario.NOMBRE,
                                     nombreTipoContrato = tipoContrato.NOMBRE
                                 }).ToList();
            }
                return View(listaEmpleado);
        }

        public void listarComboSexo()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from sexo in bd.Sexo
                         where sexo.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = sexo.NOMBRE,
                             Value = sexo.IIDSEXO.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione", Value = "" });
                ViewBag.listaSexo = lista;
            }
        }

        public void listarTipoContrato()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.TipoContrato
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.NOMBRE,
                             Value = item.IIDTIPOCONTRATO.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione", Value = "" });
                ViewBag.listaTipoContrato = lista;
            }
        }

        public void listarTipoUsuario()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.TipoUsuario
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.NOMBRE,
                             Value = item.IIDTIPOUSUARIO.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione", Value = "" });
                ViewBag.listaTipoUsuario = lista;
            }
        }

        public void listarCombos()
        {
            listarTipoUsuario();
            listarTipoContrato();
            listarComboSexo();
        }

        public ActionResult Agregar()
        {
            listarCombos();
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(EmpleadoCLS oEmpleadoCls)
        {
            int registrosEncontrados = 0;
            string nombre = oEmpleadoCls.Nombre;
            string apPaterno = oEmpleadoCls.apPaterno;
            string apMaterno = oEmpleadoCls.apMaterno;
            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Empleado.Where(p => p.NOMBRE.Equals(nombre) && p.APPATERNO.Equals(apPaterno)
                && p.APMATERNO.Equals(apMaterno)).Count();
            }
                if (!ModelState.IsValid || registrosEncontrados >=1)
                {
                if (registrosEncontrados >= 1) oEmpleadoCls.mensajeError = "Ya existe el empleado";
                    listarCombos();
                    return View(oEmpleadoCls);
                }
            using (var bd = new BDPasajeEntities())
            {
                Empleado oEmpleado = new Empleado();
                oEmpleado.NOMBRE = oEmpleadoCls.Nombre;
                oEmpleado.APPATERNO = oEmpleadoCls.apPaterno;
                oEmpleado.APMATERNO = oEmpleadoCls.apMaterno;
                oEmpleado.FECHACONTRATO = oEmpleadoCls.fechaContrato;
                oEmpleado.SUELDO = oEmpleadoCls.sueldo;
                oEmpleado.IIDTIPOUSUARIO = oEmpleadoCls.iidtipoUsuario;
                oEmpleado.IIDTIPOCONTRATO = oEmpleadoCls.iidtipoContrato;
                oEmpleado.IIDSEXO = oEmpleadoCls.iidSexo;
                oEmpleado.BHABILITADO = 1;
                bd.Empleado.Add(oEmpleado);
                bd.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult Editar( int id)
        {
            listarCombos();
            EmpleadoCLS oEmpleadoCLS = new EmpleadoCLS();
            using (var bd = new BDPasajeEntities())
            {
                Empleado oEmpleado = bd.Empleado.Where(p => p.IIDEMPLEADO.Equals(id)).First();
                oEmpleadoCLS.iidEmpleado = oEmpleado.IIDEMPLEADO;
                oEmpleadoCLS.Nombre = oEmpleado.NOMBRE;
                oEmpleadoCLS.apPaterno = oEmpleado.APPATERNO;
                oEmpleadoCLS.apMaterno = oEmpleado.APMATERNO;
                oEmpleadoCLS.fechaContrato =(DateTime) oEmpleado.FECHACONTRATO;
                oEmpleadoCLS.sueldo =(decimal) oEmpleado.SUELDO;
                oEmpleadoCLS.iidtipoUsuario =(int) oEmpleado.IIDTIPOUSUARIO;
                oEmpleadoCLS.iidtipoContrato = (int)oEmpleado.IIDTIPOCONTRATO;
                oEmpleadoCLS.iidSexo = (int)oEmpleado.IIDSEXO;
            }
                return View(oEmpleadoCLS);
        }
        [HttpPost]
        public ActionResult Editar(EmpleadoCLS oEmpleadoCLS)
        {
            int registrosEncontrados = 0;
            int idEmpleado = oEmpleadoCLS.iidEmpleado;
            string nombre = oEmpleadoCLS.Nombre;
            string apPaterno = oEmpleadoCLS.apPaterno;
            string apMaterno = oEmpleadoCLS.apMaterno;
            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Empleado.Where(p => p.NOMBRE.Equals(nombre) && p.APPATERNO.Equals(apPaterno)
                && p.APMATERNO.Equals(apMaterno) && !p.IIDEMPLEADO.Equals(idEmpleado)).Count();
            }

                if (!ModelState.IsValid || registrosEncontrados>= 1)
                {
                if (registrosEncontrados >= 1) oEmpleadoCLS.mensajeError = "Ya existe el empleado";
                listarCombos();    
                return View(oEmpleadoCLS);
                }
            using (var bd = new BDPasajeEntities())
            {
                Empleado oEmpleado = bd.Empleado.Where(p => p.IIDEMPLEADO.Equals(idEmpleado)).First();
                oEmpleado.NOMBRE = oEmpleadoCLS.Nombre;
                oEmpleado.APPATERNO = oEmpleadoCLS.apPaterno;
                oEmpleado.APMATERNO = oEmpleadoCLS.apMaterno;
                oEmpleado.FECHACONTRATO = oEmpleadoCLS.fechaContrato;
                oEmpleado.SUELDO = oEmpleadoCLS.sueldo;
                oEmpleado.IIDTIPOUSUARIO = oEmpleadoCLS.iidtipoUsuario;
                oEmpleado.IIDTIPOCONTRATO = oEmpleadoCLS.iidtipoContrato;
                oEmpleado.IIDSEXO = oEmpleadoCLS.iidSexo;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Eliminar( int txtidEmpleado)
        {
            using (var bd = new BDPasajeEntities())
            {
                Empleado emp = bd.Empleado.Where(p => p.IIDEMPLEADO.Equals(txtidEmpleado)).First();
                emp.BHABILITADO = 0;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index(ClienteCLS oClienteCLS)
        {
            List<ClienteCLS> listaCliente = null;
            int idsexo = oClienteCLS.iidsexo;
            llenarSexo();
            ViewBag.lista = listaSexo;

            using (var bd = new BDPasajeEntities())
            {
                if(oClienteCLS.iidsexo == 0)
                { 
                    listaCliente = (from Cliente in bd.Cliente
                                where Cliente.BHABILITADO == 1
                                select new ClienteCLS
                                {
                                    iidcliente = Cliente.IIDCLIENTE,
                                    nombre = Cliente.NOMBRE,
                                    appaterno = Cliente.APPATERNO,
                                    apmaterno = Cliente.APMATERNO,
                                    telefonofijo = Cliente.TELEFONOFIJO
                                }).ToList();
                }
                else
                {
                    listaCliente = (from Cliente in bd.Cliente
                                    where Cliente.BHABILITADO == 1
                                    && Cliente.IIDSEXO == idsexo
                                    select new ClienteCLS
                                    {
                                        iidcliente = Cliente.IIDCLIENTE,
                                        nombre = Cliente.NOMBRE,
                                        appaterno = Cliente.APPATERNO,
                                        apmaterno = Cliente.APMATERNO,
                                        telefonofijo = Cliente.TELEFONOFIJO
                                    }).ToList();
                }
            }

                return View(listaCliente);
        }

        public ActionResult Editar( int id)
        {
            ClienteCLS oClienteCLS = new ClienteCLS();
            using(var bd = new BDPasajeEntities())
            {
                llenarSexo();
                ViewBag.lista = listaSexo;

                Cliente oCliente = bd.Cliente.Where(p => p.IIDCLIENTE.Equals(id)).First();
                oClienteCLS.iidcliente = oCliente.IIDCLIENTE;
                oClienteCLS.nombre = oCliente.NOMBRE;
                oClienteCLS.appaterno = oCliente.APPATERNO;
                oClienteCLS.apmaterno = oCliente.APMATERNO;
                oClienteCLS.direccion = oCliente.DIRECCION;
                oClienteCLS.email = oCliente.EMAIL;
                oClienteCLS.iidsexo =(int) oCliente.IIDSEXO;
                oClienteCLS.telefonocelular = oCliente.TELEFONOCELULAR;
                oClienteCLS.telefonofijo = oCliente.TELEFONOFIJO;
            }
            return View(oClienteCLS);
        }
        [HttpPost]
        public ActionResult Editar (ClienteCLS oClienteCLS)
        {
            int registrosEncontrados = 0;
            int idCliente = oClienteCLS.iidcliente;
            string nombre = oClienteCLS.nombre;
            string apPaterno = oClienteCLS.appaterno;
            string apMaterno = oClienteCLS.apmaterno;

            using (var bd = new BDPasajeEntities())
            {
                bd.Cliente.Where(p => p.NOMBRE.Equals(nombre) && p.APPATERNO.Equals(apPaterno) && p.APMATERNO.Equals(apMaterno)
                && !p.IIDCLIENTE.Equals(idCliente)).Count();
            }

                if (!ModelState.IsValid || registrosEncontrados >= 1)
                {
                if (registrosEncontrados >= 1) oClienteCLS.mensajeError = "Ya existe el cliente";
                llenarSexo();    
                return View(oClienteCLS);
                }
            using (var bd = new BDPasajeEntities())
            {
                Cliente oCliente = bd.Cliente.Where(p => p.IIDCLIENTE.Equals(idCliente)).First();
                oCliente.NOMBRE = oClienteCLS.nombre;
                oCliente.APPATERNO = oClienteCLS.appaterno;
                oCliente.APMATERNO = oClienteCLS.apmaterno;
                oCliente.DIRECCION = oClienteCLS.direccion;
                oCliente.EMAIL = oClienteCLS.email;
                oCliente.IIDSEXO = oClienteCLS.iidsexo;
                oCliente.TELEFONOCELULAR = oClienteCLS.telefonocelular;
                oCliente.TELEFONOFIJO = oClienteCLS.telefonofijo;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
                
        }
            

        List<SelectListItem> listaSexo;
        private void llenarSexo()
        {
            using(var bd = new BDPasajeEntities())
            {
                listaSexo = (from sexo in bd.Sexo
                             where sexo.BHABILITADO == 1
                             select new SelectListItem
                             {
                                 Text = sexo.NOMBRE,
                                 Value = sexo.IIDSEXO.ToString()
                             }).ToList();
                listaSexo.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            }
        }
        public ActionResult Agregar()
        {
            llenarSexo();
            ViewBag.lista = listaSexo;
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(ClienteCLS oClienteCLS)
        {
            int registrosEncontrados = 0;
            string nombre = oClienteCLS.nombre;
            string apPaterno=oClienteCLS.appaterno;
            string apMaterno=oClienteCLS.apmaterno;
            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Cliente.Where(p => p.NOMBRE.Equals(nombre) && p.APPATERNO.Equals(apPaterno) &&
                p.APMATERNO.Equals(apMaterno)).Count();
            }
                if (!ModelState.IsValid || registrosEncontrados>=1)
                {
                if (registrosEncontrados >= 1) oClienteCLS.mensajeError = "Ya existe el cliente";
                    llenarSexo();
                    ViewBag.lista = listaSexo;
                    return View(oClienteCLS);
                }
            using (var bd = new BDPasajeEntities())
            {
                Cliente oCliente = new Cliente(); // USamos la clase Cliente del Entity Framework.
                oCliente.NOMBRE = oClienteCLS.nombre;
                oCliente.APPATERNO = oClienteCLS.appaterno;
                oCliente.APMATERNO = oClienteCLS.apmaterno;
                oCliente.EMAIL = oClienteCLS.email;
                oCliente.DIRECCION = oClienteCLS.direccion;
                oCliente.IIDSEXO = oClienteCLS.iidsexo;
                oCliente.TELEFONOCELULAR = oClienteCLS.telefonocelular;
                oCliente.TELEFONOFIJO = oClienteCLS.telefonofijo;
                oCliente.BHABILITADO = 1;
                bd.Cliente.Add(oCliente);
                bd.SaveChanges();
            }

                return RedirectToAction("Index");
           
        }

        public ActionResult Eliminar(int iidcliente)
        {
            using (var bd = new BDPasajeEntities())
            {
                Cliente oCliente = bd.Cliente.Where(p => p.IIDCLIENTE.Equals(iidcliente)).First();
                oCliente.BHABILITADO = 0;
                bd.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Security.Cryptography;
using System.Text;
using System.Web.Razor.Text;

namespace MiPrimerEntityFramework.Controllers
{
    public class UsuarioController : Controller
    {
        public void listaPersonas()
        {
            List<SelectListItem> listaPersonas = new List<SelectListItem>();
            using (var bd = new BDPasajeEntities())
            {
                //Lista de clientes que estan habilitados y que no tienen usuario
                List<SelectListItem> listaClientes = (from item in bd.Cliente
                                                      where item.BHABILITADO == 1
                                                      && item.bTieneUsuario != 1
                                                      select new SelectListItem
                                                      {
                                                          Text = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + "(C)",
                                                          Value = item.IIDCLIENTE.ToString()
                                                      }).ToList();
                //Lista de empleados que estan habilitados y que no tienen usuario
                List<SelectListItem> listaEmpleados = (from item in bd.Empleado
                                                       where item.BHABILITADO == 1
                                                       && item.bTieneUsuario != 1
                                                       select new SelectListItem
                                                       {
                                                           Text = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + "(E)",
                                                           Value = item.IIDEMPLEADO.ToString()
                                                       }).ToList();
                listaPersonas.AddRange(listaClientes);
                listaPersonas.AddRange(listaEmpleados);
                listaPersonas = listaPersonas.OrderBy(p => p.Text).ToList();
                listaPersonas.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
                ViewBag.listaPersona = listaPersonas;
            }

        }

        public void listaRol()
        {
            List<SelectListItem> listaRol;
            using (var bd = new BDPasajeEntities())
            {
                listaRol = (from item in bd.Rol
                            where item.BHABILITADO == 1
                            select new SelectListItem
                            {
                                Text = item.NOMBRE,
                                Value = item.IIDROL.ToString()
                            }).ToList();
            }
            listaRol.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            ViewBag.listaRoll = listaRol;
        }
        // GET: Usuario
        public ActionResult Index()
        {
            listaPersonas();
            listaRol();
            List<usuarioCLS> listaUsuario = new List<usuarioCLS>();
            using (var bd = new BDPasajeEntities())
            {
                List<usuarioCLS> listaUsuarioCliente = (from usuario in bd.Usuario
                                                        join cliente in bd.Cliente
                                                        on usuario.IID equals cliente.IIDCLIENTE
                                                        join rol in bd.Rol
                                                        on usuario.IIDROL equals rol.IIDROL
                                                        where usuario.bhabilitado == 1
                                                        && usuario.TIPOUSUARIO == "C"
                                                        select new usuarioCLS
                                                        {
                                                            iidusuario = usuario.IIDUSUARIO,
                                                            nombrePersona = cliente.NOMBRE + " " + cliente.APPATERNO + " " + cliente.APMATERNO,
                                                            nombreusuario = usuario.NOMBREUSUARIO,
                                                            nombreRol = rol.NOMBRE,
                                                            nombreTipoEmpleado = "Cliente"

                                                        }).ToList();
                List<usuarioCLS> listaUsuarioEmpleados = (from usuario in bd.Usuario
                                                          join empleado in bd.Empleado
                                                          on usuario.IID equals empleado.IIDEMPLEADO
                                                          join rol in bd.Rol
                                                          on usuario.IIDROL equals rol.IIDROL
                                                          where usuario.bhabilitado == 1
                                                          && usuario.TIPOUSUARIO == "E"
                                                          select new usuarioCLS
                                                          {
                                                              iidusuario = usuario.IIDUSUARIO,
                                                              nombrePersona = empleado.NOMBRE + " " + empleado.APPATERNO + " " + empleado.APMATERNO,
                                                              nombreusuario = usuario.NOMBREUSUARIO,
                                                              nombreRol = rol.NOMBRE,
                                                              nombreTipoEmpleado = "Empleado"

                                                          }).ToList();
                listaUsuario.AddRange(listaUsuarioCliente);
                listaUsuario.AddRange(listaUsuarioEmpleados);
                listaUsuario = listaUsuario.OrderBy(p => p.iidusuario).ToList();
            }
            return View(listaUsuario);
        }

        public ActionResult Filtrar(usuarioCLS oUsuarioCLS)
        {
            string nombrePersona = oUsuarioCLS.nombrePersona;
            listaPersonas();
            listaRol();
            List<usuarioCLS> listaUsuario = new List<usuarioCLS>();
            using (var bd = new BDPasajeEntities())
            {
                if (oUsuarioCLS.nombrePersona == null)
                {
                    List<usuarioCLS> listaUsuarioCliente = (from usuario in bd.Usuario
                                                            join cliente in bd.Cliente
                                                            on usuario.IID equals cliente.IIDCLIENTE
                                                            join rol in bd.Rol
                                                            on usuario.IIDROL equals rol.IIDROL
                                                            where usuario.bhabilitado == 1
                                                            && usuario.TIPOUSUARIO == "C"
                                                            select new usuarioCLS
                                                            {
                                                                iidusuario = usuario.IIDUSUARIO,
                                                                nombrePersona = cliente.NOMBRE + " " + cliente.APPATERNO + " " + cliente.APMATERNO,
                                                                nombreusuario = usuario.NOMBREUSUARIO,
                                                                nombreRol = rol.NOMBRE,
                                                                nombreTipoEmpleado = "Cliente"

                                                            }).ToList();
                    List<usuarioCLS> listaUsuarioEmpleados = (from usuario in bd.Usuario
                                                              join empleado in bd.Empleado
                                                              on usuario.IID equals empleado.IIDEMPLEADO
                                                              join rol in bd.Rol
                                                              on usuario.IIDROL equals rol.IIDROL
                                                              where usuario.bhabilitado == 1
                                                              && usuario.TIPOUSUARIO == "E"
                                                              select new usuarioCLS
                                                              {
                                                                  iidusuario = usuario.IIDUSUARIO,
                                                                  nombrePersona = empleado.NOMBRE + " " + empleado.APPATERNO + " " + empleado.APMATERNO,
                                                                  nombreusuario = usuario.NOMBREUSUARIO,
                                                                  nombreRol = rol.NOMBRE,
                                                                  nombreTipoEmpleado = "Empleado"

                                                              }).ToList();
                    listaUsuario.AddRange(listaUsuarioCliente);
                    listaUsuario.AddRange(listaUsuarioEmpleados);
                    listaUsuario = listaUsuario.OrderBy(p => p.iidusuario).ToList();
                }
                else
                {
                    List<usuarioCLS> listaUsuarioCliente = (from usuario in bd.Usuario
                                                            join cliente in bd.Cliente
                                                            on usuario.IID equals cliente.IIDCLIENTE
                                                            join rol in bd.Rol
                                                            on usuario.IIDROL equals rol.IIDROL
                                                            where usuario.bhabilitado == 1
                                                            && (
                                                            cliente.NOMBRE.Contains(nombrePersona)
                                                            || cliente.APPATERNO.Contains(nombrePersona)
                                                            || cliente.APMATERNO.Contains(nombrePersona)
                                                            )
                                                            && usuario.TIPOUSUARIO == "C"
                                                            select new usuarioCLS
                                                            {
                                                                iidusuario = usuario.IIDUSUARIO,
                                                                nombrePersona = cliente.NOMBRE + " " + cliente.APPATERNO + " " + cliente.APMATERNO,
                                                                nombreusuario = usuario.NOMBREUSUARIO,
                                                                nombreRol = rol.NOMBRE,
                                                                nombreTipoEmpleado = "Cliente"

                                                            }).ToList();
                    List<usuarioCLS> listaUsuarioEmpleados = (from usuario in bd.Usuario
                                                              join empleado in bd.Empleado
                                                              on usuario.IID equals empleado.IIDEMPLEADO
                                                              join rol in bd.Rol
                                                              on usuario.IIDROL equals rol.IIDROL
                                                              where usuario.bhabilitado == 1
                                                              && usuario.TIPOUSUARIO == "E"
                                                              && (
                                                                empleado.NOMBRE.Contains(nombrePersona)
                                                                || empleado.APPATERNO.Contains(nombrePersona)
                                                                || empleado.APMATERNO.Contains(nombrePersona)
                                                                )
                                                              select new usuarioCLS
                                                              {
                                                                  iidusuario = usuario.IIDUSUARIO,
                                                                  nombrePersona = empleado.NOMBRE + " " + empleado.APPATERNO + " " + empleado.APMATERNO,
                                                                  nombreusuario = usuario.NOMBREUSUARIO,
                                                                  nombreRol = rol.NOMBRE,
                                                                  nombreTipoEmpleado = "Empleado"

                                                              }).ToList();
                    listaUsuario.AddRange(listaUsuarioCliente);
                    listaUsuario.AddRange(listaUsuarioEmpleados);
                    listaUsuario = listaUsuario.OrderBy(p => p.iidusuario).ToList();
                }
            }
            return PartialView("_tablaUsuario", listaUsuario);
        }

        public string Guardar(usuarioCLS oUsuarioCLS, int titulo)
        {

            //vacio es error
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
                        using (var transaccion = new TransactionScope())
                            if (titulo.Equals(-1))
                            {
                                cantidad = bd.Usuario.Where(p => p.NOMBREUSUARIO == oUsuarioCLS.nombreusuario).Count();
                                if (cantidad >= 1)
                                {
                                    respuesta = "-1";
                                }
                                else
                                {
                                    Usuario oUsuario = new Usuario();
                                    oUsuario.NOMBREUSUARIO = oUsuarioCLS.nombreusuario;
                                    SHA256Managed sha = new SHA256Managed();
                                    byte[] byteContra = Encoding.Default.GetBytes(oUsuarioCLS.contra);
                                    byte[] byteContraCifrada = sha.ComputeHash(byteContra);
                                    string cadenaContraCifrada = BitConverter.ToString(byteContraCifrada).Replace("-", "");
                                    oUsuario.CONTRA = cadenaContraCifrada;
                                    //Obteniendo la penúltima posición del string para identificar si el usuario es empleado o cliente
                                    oUsuario.TIPOUSUARIO = oUsuarioCLS.nombrePersonaEnviar.Substring(oUsuarioCLS.nombrePersonaEnviar.Length - 2, 1);
                                    oUsuario.IID = oUsuarioCLS.iid;
                                    oUsuario.IIDROL = oUsuarioCLS.iidrol;
                                    oUsuario.bhabilitado = 1;
                                    bd.Usuario.Add(oUsuario);
                                    if (oUsuario.TIPOUSUARIO.Equals("C"))
                                    {
                                        Cliente oCliente = bd.Cliente.Where(p => p.IIDCLIENTE.Equals(oUsuarioCLS.iid)).First();
                                        oCliente.bTieneUsuario = 1;

                                    }
                                    else
                                    {
                                        Empleado oEmpleado = bd.Empleado.Where(p => p.IIDEMPLEADO.Equals(oUsuarioCLS.iid)).First();
                                        oEmpleado.bTieneUsuario = 1;
                                    }
                                    respuesta = bd.SaveChanges().ToString();
                                    if (respuesta == "0") respuesta = "";
                                    transaccion.Complete();
                                }
                            }
                            else
                            {
                                cantidad = bd.Usuario.Where(p => p.NOMBREUSUARIO == oUsuarioCLS.nombreusuario
                                && p.IIDUSUARIO != titulo).Count();
                                if (cantidad >= 1)
                                {
                                    respuesta = "-1";
                                }
                                else
                                {
                                    //Editar
                                    Usuario oUsuario = bd.Usuario.Where(p => p.IIDUSUARIO == titulo).First();
                                    oUsuario.IIDROL = oUsuarioCLS.iidrol;
                                    oUsuario.NOMBREUSUARIO = oUsuarioCLS.nombreusuario;
                                    respuesta = bd.SaveChanges().ToString();
                                    transaccion.Complete();
                                }
                            }
                    }
                }
            
            }
    
            catch (Exception ex)
            {
                respuesta = "";

            }
            return respuesta;

        }

        public int Eliminar(int idUsuario)
        {
            int respuesta = 0;
            try { 
            using (BDPasajeEntities bd = new BDPasajeEntities())
            {
                Usuario oUsuario = bd.Usuario.Where(p => p.IIDUSUARIO == idUsuario).First();
                oUsuario.bhabilitado = 0;
                respuesta =  bd.SaveChanges();

            }
            }
            catch (Exception e)
            {
                respuesta = 0;
            }
            return respuesta;
        }
        public JsonResult recuperarInfo(int iidusuario)
        {
            usuarioCLS oUsuarioCLS = new usuarioCLS();
            using (var bd = new BDPasajeEntities())
            {
                Usuario oUsuario = bd.Usuario.Where(p => p.IIDUSUARIO == iidusuario).First();
                oUsuarioCLS.nombreusuario = oUsuario.NOMBREUSUARIO;
                oUsuarioCLS.iidrol =(int) oUsuario.IIDROL;
            }
            return Json(oUsuarioCLS, JsonRequestBehavior.AllowGet);
        }

    }  
}
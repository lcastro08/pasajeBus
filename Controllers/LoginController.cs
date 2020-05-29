using MiPrimerEntityFramework.ClasesAuxiliares;
using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MiPrimerEntityFramework.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult cerrarSesion()
        {
            Session["Usuario"] = null;
            Session["Rol"] = null;
            return RedirectToAction("Index");
        }

        public string Login(usuarioCLS oUsuario)
        {
            string mensaje = "";
            
            //error
            if (!ModelState.IsValid)
            {
                //Vamos a obtener los estados de cada propiedad y los mensajes de error (si es que hay).
                var query = (from state in ModelState.Values
                             from error in state.Errors
                             select error.ErrorMessage).ToList();
                mensaje += "<ul class='list-group'>";
                foreach (var item in query)
                {
                    mensaje += "<li class = 'list-group-item'>" + item + "</li>";
                }
                mensaje += "</ul>";
            }
            //bien
            else
            {
                string nombreUsuario = oUsuario.nombreusuario;
                string password = oUsuario.contra;
                //Cifrar
                SHA256Managed sha = new SHA256Managed();
                byte[] byteContra = Encoding.Default.GetBytes(password);
                byte[] byteContraCifrada = sha.ComputeHash(byteContra);
                string cadenaContraCifrada = BitConverter.ToString(byteContraCifrada).Replace("-", "");
                using (var bd = new BDPasajeEntities())
                {
                    int numeroVeces = bd.Usuario.Where(p => p.NOMBREUSUARIO == nombreUsuario
                    && p.CONTRA == cadenaContraCifrada).Count();
                    mensaje = numeroVeces.ToString();
                    if (mensaje == "0") mensaje = "Usuario o contraseña incorrecta";
                    else
                    {
                        Usuario ousuario = bd.Usuario.Where(p => p.NOMBREUSUARIO == nombreUsuario
                        && p.CONTRA == cadenaContraCifrada).First();
                        //Todo el objeto usuario
                        Session["Usuario"] = ousuario;

                        if(ousuario.TIPOUSUARIO == "C")
                        {
                            Cliente oCliente = bd.Cliente.Where(p => p.IIDCLIENTE == ousuario.IID).First();
                            Session["nombreCompleto"] = oCliente.NOMBRE + " " + oCliente.APPATERNO + " " + oCliente.APMATERNO;

                        }
                        else
                        {
                            Empleado oEmpleado = bd.Empleado.Where(p => p.IIDEMPLEADO == ousuario.IID).First();
                            Session["nombreCompleto"] = oEmpleado.NOMBRE + " " + oEmpleado.APPATERNO + " " + oEmpleado.APMATERNO;
                        }

                        List<menuCLS> listaMenu = (from usuario in bd.Usuario
                                                   join rol in bd.Rol
                                                   on usuario.IIDROL equals rol.IIDROL
                                                   join rolpag in bd.RolPagina
                                                   on rol.IIDROL equals rolpag.IIDROL
                                                   join Pagina in bd.Pagina
                                                   on rolpag.IIDPAGINA equals Pagina.IIDPAGINA
                                                   where rol.IIDROL == ousuario.IIDROL && rolpag.IIDROL == usuario.IIDROL
                                                   && usuario.IIDUSUARIO == ousuario.IIDUSUARIO
                                                   select new menuCLS
                                                   {
                                                       nombreAccion = Pagina.ACCION,
                                                       nombreControlador = Pagina.CONTROLADOR,
                                                       mensaje = Pagina.MENSAJE
                                                   }).ToList();


                                        Session["Rol"] = listaMenu;
                    }
                }
            }
            return mensaje;
        }

        public string recuperarContra( string IIDtipo, string correo, string telefonoCelular)
        {
            string mensaje = "";
            using (var bd = new BDPasajeEntities())
            {
                
                int cantidad = 0;
                int iidcliente;
                if(IIDtipo == "C")
                {
                    //Existe un cliente con esa información.
                  cantidad =   bd.Cliente.Where(p => p.EMAIL == correo && p.TELEFONOCELULAR == telefonoCelular).Count();
                
                }
                if (cantidad == 0) mensaje = "No existe una persona registrada con esa información";
                else
                {
                    iidcliente = bd.Cliente.Where(p => p.EMAIL == correo && p.TELEFONOCELULAR == telefonoCelular).First().IIDCLIENTE;
                    //Verificar si tiene usuario
                    int nveces = bd.Usuario.Where(p => p.IID == iidcliente && p.TIPOUSUARIO == "C").Count();
                    if(nveces == 0)
                    {
                        mensaje = "No tiene usuario";
                    }
                    else
                    {
                        //Obtener id usuario
                        Usuario ousuario = bd.Usuario.Where(p => p.IID == iidcliente && p.TIPOUSUARIO == "C").First();

                        //Modificar su clave
                        Random ra = new Random();
                        int n1 = ra.Next(0, 9);
                        int n2 = ra.Next(0, 9);
                        int n3 = ra.Next(0, 9);
                        int n4 = ra.Next(0, 9);
                        string nuevaContra = n1.ToString() + n2 + n3 + n4;
                        //Cifrar Clave

                        SHA256Managed sha = new SHA256Managed();
                        byte[] byteContra = Encoding.Default.GetBytes(nuevaContra);
                        byte[] byteContraCifrada = sha.ComputeHash(byteContra);
                        string cadenaContraCifrada = BitConverter.ToString(byteContraCifrada).Replace("-", "");

                        ousuario.CONTRA = cadenaContraCifrada;
                        mensaje = bd.SaveChanges().ToString();
                        Correo.EnviarCorreo(correo, "Recuperar Clave", "Se generó de nuevo su clave. Ahora es: "+nuevaContra,"");
                    }
                    
                }
            }
            return mensaje;
        }
    }
}
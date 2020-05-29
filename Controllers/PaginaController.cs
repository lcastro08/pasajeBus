using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Filters;

namespace MiPrimerEntityFramework.Controllers
{
    [Acceder]
    public class PaginaController : Controller
    {
        // GET: Pagina
        public ActionResult Index()
        {
            List<PaginaCLS> listaPagina = new List<PaginaCLS>();
            using (var bd = new BDPasajeEntities())
            {
                listaPagina = (from pagina in bd.Pagina
                               where pagina.BHABILITADO == 1
                               select new PaginaCLS
                               {
                                   iidpagin = pagina.IIDPAGINA,
                                   mensaje = pagina.MENSAJE,
                                   controlador = pagina.CONTROLADOR,
                                   accion = pagina.ACCION
                               }).ToList();
            }
                return View(listaPagina);
        }
        public ActionResult Filtrar(PaginaCLS oPaginaCLS)
        {
            List<PaginaCLS> listaPagina = new List<PaginaCLS>();
            string mensaje = oPaginaCLS.mensajeFiltrar;
            using (var bd = new BDPasajeEntities())
            {
                if (mensaje == null)
                {
                    listaPagina = (from pagina in bd.Pagina
                                   where pagina.BHABILITADO == 1
                                   select new PaginaCLS
                                   {
                                       iidpagin = pagina.IIDPAGINA,
                                       mensaje = pagina.MENSAJE,
                                       controlador = pagina.CONTROLADOR,
                                       accion = pagina.ACCION
                                   }).ToList();
                }
                else
                {
                    listaPagina = (from pagina in bd.Pagina
                                   where pagina.BHABILITADO == 1
                                   && pagina.MENSAJE.Contains(mensaje)
                                   select new PaginaCLS
                                   {
                                       iidpagin = pagina.IIDPAGINA,
                                       mensaje = pagina.MENSAJE,
                                       controlador = pagina.CONTROLADOR,
                                       accion = pagina.ACCION
                                   }).ToList();
                }

            }
            return PartialView("_tablaPagina", listaPagina);
        }

        public int EliminarPagina(int iidPagina)
        {
            int respuesta = 0;
            try
            {
                using (var bd = new BDPasajeEntities())
                {
                    Pagina oPagina = bd.Pagina.Where(p => p.IIDPAGINA == iidPagina).First();
                    oPagina.BHABILITADO = 0;
                    respuesta = bd.SaveChanges();

                }
            }
            catch
            {
                respuesta = 0;
            }
            return respuesta;
        }
        public string Guardar (PaginaCLS oPaginaCLS,int titulo)
        {
            string respuesta = "";
            try 
            {
                if (!ModelState.IsValid)
                {
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
                        if (titulo == -1)
                        {
                            cantidad = bd.Pagina.Where(p => p.MENSAJE == oPaginaCLS.mensaje).Count();
                            if (cantidad>= 1 )
                            {
                                respuesta = "-1";
                            }
                            else
                            { 
                            Pagina oPagina = new Pagina();
                            oPagina.MENSAJE = oPaginaCLS.mensaje;
                            oPagina.ACCION = oPaginaCLS.accion;
                            oPagina.CONTROLADOR = oPaginaCLS.controlador;
                            oPagina.BHABILITADO = 1;
                            bd.Pagina.Add(oPagina);
                            respuesta = bd.SaveChanges().ToString();
                                if (respuesta == "0") respuesta = "";

                            }

                        }
                        //Editar
                        else
                        {
                            cantidad = bd.Pagina.Where(p => p.MENSAJE == oPaginaCLS.mensaje 
                            && p.IIDPAGINA != titulo).Count();
                            if(cantidad >= 1)
                            {
                                respuesta = "-1";
                            }
                            else
                            { 
                                Pagina oPagina = bd.Pagina.Where(p => p.IIDPAGINA == titulo).First();
                                oPagina.MENSAJE = oPaginaCLS.mensaje;
                                oPagina.CONTROLADOR = oPaginaCLS.controlador;
                                oPagina.ACCION = oPaginaCLS.accion;
                                respuesta = bd.SaveChanges().ToString();
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

        public JsonResult recuperarInformacion (int idpagina)
        {
            PaginaCLS oPaginaCLS = new PaginaCLS();
            using (var bd = new BDPasajeEntities())
            {
                Pagina oPagina = bd.Pagina.Where(p => p.IIDPAGINA == idpagina).First();
                oPaginaCLS.mensaje = oPagina.MENSAJE;
                oPaginaCLS.accion = oPagina.ACCION;
                oPaginaCLS.controlador = oPagina.CONTROLADOR;
            }
            return Json(oPaginaCLS, JsonRequestBehavior.AllowGet);
        }
    }
}
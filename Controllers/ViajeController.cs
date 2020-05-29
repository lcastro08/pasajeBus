using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class ViajeController : Controller
    {
        // GET: Viaje
        public ActionResult Index()
        {
            List<ViajeCLS> listaViaje = null;
            listarCombos();
            using (var bd = new BDPasajeEntities())
            {
                listaViaje = (from viaje in bd.Viaje
                              join lugarOrigen in bd.Lugar
                              on viaje.IIDLUGARORIGEN equals lugarOrigen.IIDLUGAR
                              join lugarDestino in bd.Lugar
                              on viaje.IIDLUGARDESTINO equals lugarDestino.IIDLUGAR
                              join bus in bd.Bus
                              on viaje.IIDBUS equals bus.IIDBUS
                              where viaje.BHABILITADO == 1
                              select new ViajeCLS
                              {
                                  iidViaje = viaje.IIDVIAJE,
                                  nombreBus = bus.PLACA,
                                  nombreLugarOrigen = lugarOrigen.NOMBRE,
                                  nombreLugarDestino = lugarDestino.NOMBRE,

                              }).ToList();
            }

            return View(listaViaje);
        }
        public void listarLugar()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.Lugar
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.NOMBRE,
                             Value = item.IIDLUGAR.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione", Value = "" });
                ViewBag.listaLugar = lista;
            }
        }
        public void listarBus()
        {
            //Agregar
            List<SelectListItem> lista;
            using (var bd = new BDPasajeEntities())
            {
                lista = (from item in bd.Bus
                         where item.BHABILITADO == 1
                         select new SelectListItem
                         {
                             Text = item.PLACA,
                             Value = item.IIDBUS.ToString()
                         }).ToList();
                lista.Insert(0, new SelectListItem { Text = "--Seleccione", Value = "" });
                ViewBag.listaBus = lista;
            }
        }
        public void listarCombos ()
        {
            listarLugar();
            listarBus();
        }
        public ActionResult Agregar()
        {
            listarCombos();
            return View();
        }

        public ActionResult Filtrar(int? lugarDestinoParametro)
        {
            List<ViajeCLS> listaViaje = new List<ViajeCLS>();

            using (var bd = new BDPasajeEntities())
            {
                if(lugarDestinoParametro == null)
                {
                    listaViaje = (from viaje in bd.Viaje
                                  join lugarOrigen in bd.Lugar
                                  on viaje.IIDLUGARORIGEN equals lugarOrigen.IIDLUGAR
                                  join lugarDestino in bd.Lugar
                                  on viaje.IIDLUGARDESTINO equals lugarDestino.IIDLUGAR
                                  join bus in bd.Bus
                                  on viaje.IIDBUS equals bus.IIDBUS
                                  where viaje.BHABILITADO == 1
                                  select new ViajeCLS
                                  {
                                      iidViaje = viaje.IIDVIAJE,
                                      nombreBus = bus.PLACA,
                                      nombreLugarOrigen = lugarOrigen.NOMBRE,
                                      nombreLugarDestino = lugarDestino.NOMBRE,

                                  }).ToList();
                }
                else
                {
                    listaViaje = (from viaje in bd.Viaje
                                  join lugarOrigen in bd.Lugar
                                  on viaje.IIDLUGARORIGEN equals lugarOrigen.IIDLUGAR
                                  join lugarDestino in bd.Lugar
                                  on viaje.IIDLUGARDESTINO equals lugarDestino.IIDLUGAR
                                  join bus in bd.Bus
                                  on viaje.IIDBUS equals bus.IIDBUS
                                  where viaje.BHABILITADO == 1
                                  && viaje.IIDLUGARDESTINO == lugarDestinoParametro
                                  select new ViajeCLS
                                  {
                                      iidViaje = viaje.IIDVIAJE,
                                      nombreBus = bus.PLACA,
                                      nombreLugarOrigen = lugarOrigen.NOMBRE,
                                      nombreLugarDestino = lugarDestino.NOMBRE,

                                  }).ToList();
                }
            }
            return PartialView("_tablaViaje", listaViaje);
            
        }
        public string Guardar( ViajeCLS oViajeCLS, HttpPostedFileBase foto, int titulo)
        {
            string mensaje = "";
            try
            {
                if(!ModelState.IsValid || (foto == null && titulo == -1))
                {
                    //Vamos a obtener los estados de cada propiedad y los mensajes de error (si es que hay).
                    var query = (from state in ModelState.Values
                                 from error in state.Errors
                                 select error.ErrorMessage).ToList();
                   
                    if (foto == null && titulo == -1)
                    {
                        oViajeCLS.mensaje = "La foto es obligatoria";
                        mensaje += "<ul><li> Debe ingresar la foto </li></ul>";
                    }   
                       
                    mensaje += "<ul class='list-group'>";
                    foreach (var item in query)
                    {
                        mensaje += "<li class = 'list-group-item'>" + item + "</li>";
                    }
                    mensaje += "</ul>";
                }
                else
                {
                    byte[] fotoBD = null;
                    if(foto!= null)
                    {
                        BinaryReader lector = new BinaryReader(foto.InputStream);
                        fotoBD = lector.ReadBytes((int)foto.ContentLength);
                    }
                    using(var bd = new BDPasajeEntities())
                    {
                        if (titulo == -1)
                        {
                            Viaje oViaje = new Viaje();
                            oViaje.IIDBUS = oViajeCLS.iidBus;
                            oViaje.IIDLUGARDESTINO = oViajeCLS.iidLugarDestino;
                            oViaje.IIDLUGARORIGEN = oViajeCLS.iidLugarOrigen;
                            oViaje.PRECIO = oViajeCLS.precio;
                            oViaje.FECHAVIAJE = oViajeCLS.fechaViaje;
                            oViaje.NUMEROASIENTOSDISPONIBLES = oViajeCLS.numeroAsientosDisponibles;
                            oViaje.FOTO = fotoBD;
                            oViaje.nombrefoto = oViajeCLS.nombreFoto;
                            oViaje.BHABILITADO = 1;
                            bd.Viaje.Add(oViaje);
                            mensaje = bd.SaveChanges().ToString();
                            if (mensaje == "0") mensaje = "";
                        }
                        else
                        {
                            Viaje oViaje = bd.Viaje.Where(p => p.IIDVIAJE == titulo).First();
                            oViaje.IIDLUGARDESTINO = oViajeCLS.iidLugarDestino;
                            oViaje.IIDLUGARORIGEN = oViajeCLS.iidLugarOrigen;
                            oViaje.PRECIO = oViajeCLS.iidLugarOrigen;
                            oViaje.FECHAVIAJE = oViajeCLS.fechaViaje;
                            oViaje.IIDBUS = oViajeCLS.iidBus;
                            oViaje.NUMEROASIENTOSDISPONIBLES = oViajeCLS.numeroAsientosDisponibles;
                            oViaje.IIDLUGARDESTINO = oViajeCLS.iidLugarDestino;
                            if (foto != null) oViaje.FOTO = fotoBD;
                            mensaje = bd.SaveChanges().ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = "";
            }
            return mensaje;
        }

        public JsonResult recuperarInfo(int idViaje)
        {
            ViajeCLS oViajeCLS = new ViajeCLS();
            using (var bd = new BDPasajeEntities())
            {
                Viaje oViaje = bd.Viaje.Where(p => p.IIDVIAJE == idViaje).First();
                oViajeCLS.iidViaje = oViaje.IIDVIAJE;
                oViajeCLS.iidBus = (int)oViaje.IIDBUS;
                oViajeCLS.iidLugarDestino = (int)oViaje.IIDLUGARDESTINO;
                oViajeCLS.iidLugarOrigen = (int)oViaje.IIDLUGARORIGEN;
                oViajeCLS.precio = (int)oViaje.PRECIO;
                //año-mes-dia (así la pide)
                //en la bd viene: día-mes-año
                oViajeCLS.fechaViajeCadena = ((DateTime) oViaje.FECHAVIAJE).ToString("yyyy-MM-dd");
                oViajeCLS.numeroAsientosDisponibles = (int)oViaje.NUMEROASIENTOSDISPONIBLES;
                oViajeCLS.nombreFoto = oViaje.nombrefoto;
                oViajeCLS.extension = Path.GetExtension(oViaje.nombrefoto);
                oViajeCLS.fotoRecuperarCadena = Convert.ToBase64String(oViaje.FOTO);

            }
            return Json(oViajeCLS, JsonRequestBehavior.AllowGet);
        }

        public int EliminarViaje (int idViaje)
        {
            int registrosAfectados = 0;
            try
            {
                using (var bd = new BDPasajeEntities())
                {
                    Viaje oViaje = bd.Viaje.Where(p => p.IIDVIAJE == idViaje).First();
                    oViaje.BHABILITADO = 0;
                    registrosAfectados = bd.SaveChanges();
                    
                }
            }
            catch(Exception ex)
            {
                registrosAfectados = 0;
            }
            return registrosAfectados;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiPrimerEntityFramework.Models;

namespace MiPrimerEntityFramework.Controllers
{
    public class MarcaController : Controller
    {
        // GET: Marca
        public ActionResult Index(MarcaCLS oMarcaCLS)
        {
            string nombreMarca = oMarcaCLS.nombre;
            List < MarcaCLS > listaMarca = null;
            using (var bd = new BDPasajeEntities())
            {
                if(oMarcaCLS.nombre == null) 
                { 
                listaMarca = (from marca in bd.Marca
                              where marca.BHABILITADO == 1
                                             select new MarcaCLS
                                             {
                                                 iidmarca = marca.IIDMARCA,
                                                 nombre = marca.NOMBRE,
                                                 descripcion = marca.DESCRIPCION
                                             }).ToList();
                }
                else
                {
                    listaMarca = (from marca in bd.Marca
                                  where marca.BHABILITADO == 1 
                                  && marca.NOMBRE.Contains(nombreMarca)
                                  select new MarcaCLS
                                  {
                                      iidmarca = marca.IIDMARCA,
                                      nombre = marca.NOMBRE,
                                      descripcion = marca.DESCRIPCION
                                  }).ToList();
                }
            }
            return View(listaMarca);
        }

        public ActionResult Agregar()
        {
            return View();
        }

        public ActionResult Editar(int id)
        {
            MarcaCLS oMarcaCLS = new MarcaCLS();
            using (var bd = new BDPasajeEntities())
            {
                Marca oMarca = bd.Marca.Where(p => p.IIDMARCA.Equals(id)).First();
                oMarcaCLS.iidmarca = oMarca.IIDMARCA;
                oMarcaCLS.nombre = oMarca.NOMBRE;
                oMarcaCLS.descripcion = oMarca.DESCRIPCION;
            }
                return View(oMarcaCLS);
        }

        [HttpPost]
        public ActionResult Editar(MarcaCLS oMarcaCLS)
        {
            int registrosEncontrados = 0;
            string nombreMarca = oMarcaCLS.nombre;
            int iidMarca = oMarcaCLS.iidmarca;

            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Marca.Where(p => p.NOMBRE.Equals(nombreMarca) && !p.IIDMARCA.Equals(iidMarca)).Count();
            }
                if (!ModelState.IsValid || registrosEncontrados>= 1)
                {
                if (registrosEncontrados >= 1) oMarcaCLS.MensajeError = "Ya se encuentra registrada la marca";
                    return View(oMarcaCLS);
                }

            int idMarca = oMarcaCLS.iidmarca;
            using (var bd = new BDPasajeEntities())
            {
                Marca oMarca = bd.Marca.Where(p => p.IIDMARCA.Equals(idMarca)).First();
                oMarca.NOMBRE = oMarcaCLS.nombre;
                oMarca.DESCRIPCION = oMarcaCLS.descripcion;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Agregar(MarcaCLS oMarcaCLS)
        {
            int registrosEncontrados = 0;
            string nombreMarca = oMarcaCLS.nombre;
            using (var bd = new BDPasajeEntities())
            {
                registrosEncontrados = bd.Marca.Where(p => p.NOMBRE.Equals(nombreMarca)).Count();

            }
            //////////////////////////////////////////////
                if (!ModelState.IsValid || registrosEncontrados>=1)
                {
                    if (registrosEncontrados >= 1) oMarcaCLS.MensajeError = "El nombre de la marca ya existe";
                    return View(oMarcaCLS);
                }
                else
                {
                    using (var bd = new BDPasajeEntities())
                    {
                        Marca oMarca = new Marca(); //Primero ponemos el modelo "Marca"
                        oMarca.NOMBRE = oMarcaCLS.nombre;
                        oMarca.DESCRIPCION = oMarcaCLS.descripcion;
                        oMarca.BHABILITADO = 1;
                        bd.Marca.Add(oMarca);
                        bd.SaveChanges();
                    }
                }
            return RedirectToAction("Index");
            
        }

        public ActionResult Eliminar(int id)
        {
            using (var bd = new BDPasajeEntities())
            {
                Marca oMarca = bd.Marca.Where(p => p.IIDMARCA.Equals(id)).First();
                oMarca.BHABILITADO = 0;
                bd.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        
    }
}
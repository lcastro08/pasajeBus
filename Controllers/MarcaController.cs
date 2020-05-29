using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MiPrimerEntityFramework.Models;
using OfficeOpenXml;
using MiPrimerEntityFramework.Filters;
namespace MiPrimerEntityFramework.Controllers
{
    [Acceder]
    public class MarcaController : Controller
    {

        public FileResult generarExcelByte()
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                //Todo el documento excel
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage ep = new ExcelPackage();
                //Crear una hoja
                ep.Workbook.Worksheets.Add("Reporte");
                ExcelWorksheet ew = ep.Workbook.Worksheets[0];

                //Nombre de las columnas
                ew.Cells[1, 1].Value = "Id Marca";
                ew.Cells[1, 2].Value = "Nombre";
                ew.Cells[1, 3].Value = "Descripción";
                ew.Column(1).Width = 20;
                ew.Column(2).Width = 40;
                ew.Column(3).Width = 180;

                using (var range = ew.Cells[1,1,1,3])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkRed);
                }
                List<MarcaCLS> lista = (List<MarcaCLS>)Session["lista"];
                int numeroRegistros = lista.Count;

                for (int i = 0; i<numeroRegistros;i++)
                {
                    ew.Cells[i + 2, 1].Value = lista[i].iidmarca;
                    ew.Cells[i + 2, 2].Value = lista[i].nombre;
                    ew.Cells[i + 2, 3].Value = lista[i].descripcion;
                }
                ep.SaveAs(ms);
                buffer =  ms.ToArray();
                return File(buffer, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            }
        }

        public FileResult generarPDF()
        {
            Document doc = new Document();
            byte[] buffer;

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter.GetInstance(doc, ms);

                doc.Open();
                Paragraph title = new Paragraph("Lista Marca");
                title.Alignment = Element.ALIGN_CENTER;
                
                doc.Add(title);

                Paragraph espacio = new Paragraph(" ");
                doc.Add(espacio);
                //Columnas
                PdfPTable table = new PdfPTable(3); //Entre parentesis va el número de columnas en nuestro documento
                float[] values = new float[3] { 30, 40, 80 };
               //asignando los anchos a la tabla
                table.SetWidths(values);
                //Creando celdas (Poniendo contenido)-color-alineado el contenido al centro
                PdfPCell celda1 = new PdfPCell(new Phrase("Id Marca"));
                celda1.BackgroundColor = new BaseColor(130, 130, 130);
                celda1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table.AddCell(celda1);

                PdfPCell celda2 = new PdfPCell(new Phrase("Nombre"));
                celda2.BackgroundColor = new BaseColor(130, 130, 130);
                celda2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table.AddCell(celda2);

                PdfPCell celda3 = new PdfPCell(new Phrase("Descripción"));
                celda3.BackgroundColor = new BaseColor(130, 130, 130);
                celda3.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table.AddCell(celda3);

                List<MarcaCLS> lista = (List < MarcaCLS >) Session["lista"];
                int registros = lista.Count;
                for (int i = 0; i<registros;i++)
                {
                    table.AddCell(lista[i].iidmarca.ToString());
                    table.AddCell(lista[i].nombre);
                    table.AddCell(lista[i].descripcion);
                }

                doc.Add(table);
                doc.Close();

                buffer = ms.ToArray();
            }
            return File(buffer,"application/pdf");
        }

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
                    Session["lista"] = listaMarca;
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
                    Session["lista"] = listaMarca;
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
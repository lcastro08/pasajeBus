using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MiPrimerEntityFramework.ClasesAuxiliares
{
    public class Correo
    {
        public static int EnviarCorreo(string nombreCorreo,string asunto,string contenido, string rutaError)
        {
            int respuesta = 0;
            try
            {
                string correo = ConfigurationManager.AppSettings["correo"];
                string clave = ConfigurationManager.AppSettings["clave"];
                string servidor = ConfigurationManager.AppSettings["servidor"];
                int puerto =int.Parse(ConfigurationManager.AppSettings["puerto"]);


                //Data del correo(definición)
                MailMessage mail = new MailMessage();
                mail.Subject = asunto;
                mail.IsBodyHtml = true;
                mail.Body = contenido;
                mail.From = new MailAddress(correo);
                mail.To.Add(new MailAddress(nombreCorreo));
                //Envío de correo
                SmtpClient smtp = new SmtpClient();
                smtp.Host = servidor;
                smtp.EnableSsl = true;
                smtp.Port = puerto;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(correo, clave);
                smtp.Send(mail);
                respuesta = 1;
            }
            catch(Exception ex)
            {
                respuesta = 0;
            }
            return respuesta;
        }
    }
}
using MiPrimerEntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimerEntityFramework.Filters
{
    public class Acceder:ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Si session es null , entonces retorna al login
            var usuario = HttpContext.Current.Session["Usuario"];
            List<menuCLS> roles = (List<menuCLS>) HttpContext.Current.Session["Rol"];
            string nombreControlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string accion = filterContext.ActionDescriptor.ActionName;
            int cantidad = roles.Where(p=> p.nombreControlador == nombreControlador).Count();
            if(usuario == null || cantidad == 0)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcExtension
{
    public class CheckVenderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session["VenderUser"] != null)
            //if (true)
            {
                return;
            }
            else
            {
                filterContext.Result = new RedirectResult("/Vender/Login");
            }
        }
    }
}

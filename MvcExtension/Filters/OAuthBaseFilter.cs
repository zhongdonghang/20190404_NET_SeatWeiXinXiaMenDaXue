using System;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Common;
using Senparc.Weixin.MP;

namespace MvcExtension
{
    public class OAuthBaseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);

            if (filterContext.HttpContext.Session["OpenID"] != null)
            {
                return;
            }
            else
            {
                string url = OAuthApi.GetAuthorizeUrl(GetAppSettings.AppID, GetAppSettings.SysURL + "/OAuth/BaseCallback", filterContext.HttpContext.Request.Url.ToString().Replace(":84", ""), OAuthScope.snsapi_userinfo);
                filterContext.Result = new RedirectResult(url);
            }
            
        }
    }

    public class OAuthGetSeatNowStatusFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.HttpContext.Session["OpenID"] != null)
            {
                return;
            }
            else
            {
                //string url = filterContext.HttpContext.Request.Url.ToString().Replace(":84", "");
                string p = filterContext.HttpContext.Request.QueryString["param"];
                //filterContext.Result = new ContentResult() { Content = p};
                string url = OAuthApi.GetAuthorizeUrl(GetAppSettings.AppID, GetAppSettings.SysURL + "/OAuth/BaseCallbackGetSeatNow", p, OAuthScope.snsapi_userinfo);
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}

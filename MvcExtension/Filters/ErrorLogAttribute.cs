using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcExtension
{
    /// <summary>
    /// 表示一个特性，该特性用于处理由操作方法引发的异常。
    /// </summary>
    public class ErrorLogAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            filterContext.Result =
                new ContentResult() {
                    Content = filterContext.Exception.Message
                };
                //new ContentResult().Content = filterContext.Exception.Message + filterContext.Exception.StackTrace;

            //记录错误日志文件
            //LogHelper.Error(filterContext.Exception.Message + filterContext.Exception.StackTrace);
            filterContext.ExceptionHandled = true;
        }
    }
}
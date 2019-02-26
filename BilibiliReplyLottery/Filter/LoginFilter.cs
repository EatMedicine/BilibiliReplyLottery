using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BilibiliReplyLottery.Filter
{
    public class LoginFilter : ActionFilterAttribute
    {
        /// <summary>
        /// OnActionExecuting:正要准备执行Action的时候但还未执行时执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie loginNameCookie = HttpContext.Current.Request.Cookies.Get("LoginName");
            HttpCookie CerCookie = HttpContext.Current.Request.Cookies.Get("Certification");
            string js = "<script language=javascript>alert('{0}');window.location.replace('{1}')</script>";
            if (Tools.IsCookieEmpty(loginNameCookie)==false||
                Tools.IsCookieEmpty(CerCookie) == false)
            {
                filterContext.HttpContext.Response.Redirect("/Home/Jump");
                return;
            }
            string name = loginNameCookie.Value;
            string cer = CerCookie.Value;
            if (Tools.verifyCertification(name, cer) == false)
            {
                filterContext.HttpContext.Response.Redirect("/Home/Jump");
                return;
            }
        }
    }
}
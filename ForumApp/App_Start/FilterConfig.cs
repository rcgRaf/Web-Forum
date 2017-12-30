using ForumApp.Models;
using NLog;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ForumApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        private static Logger logger = LogManager.GetLogger("dbloger");

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null || filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                
                var user = (User)filterContext.HttpContext.Session["Username"];

                var logedOutUser = user?.Username ?? "Anonymus";

                logger.Info("User tried direct acces to url, logged out user:" + logedOutUser);
                filterContext.HttpContext.Session.Abandon();

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login",
                    area = "Main"
                }));
            }
        }
    }





}

using ForumApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumApp.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomAuthorizationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
          var user =  (User)filterContext.Controller.ControllerContext.HttpContext.Session["User"];
            if (user == null)
                throw new Exception("You have to login first");

            if (!user.IsAdmin)
                throw new UnauthorizedAccessException();

        }

       
    }
}
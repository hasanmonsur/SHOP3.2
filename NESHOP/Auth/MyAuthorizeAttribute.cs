using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace NESHOP.Auth
{
    public class MyAuth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jsonLogin = filterContext.HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                //all good, add optional code if you want. Or don't
            }
            else
            {
                //DENIED!
                var redirectTarget = new RouteValueDictionary
                {{"action", "Logon"}, {"controller", "Home"}};
                //context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme);
                filterContext.Result = new RedirectToRouteResult(redirectTarget);
                //return RedirectToAction("Index", "Home");
            }
        }
    }


}

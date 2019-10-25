using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WA_HyJ
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = GetAllControllersAsRegex() }
            );

            routes.MapRoute(
               name: "404-catch-all",
               url: "{*catchall}",
               defaults: new { controller = "Error", action = "NotFound" }
           );
        }

        private static string GetAllControllersAsRegex()
        {
            var controllers = typeof(MvcApplication).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Controller))); var controllerNames = controllers.Select(c => c.Name.Replace("Controller", "")); return string.Format("({0})", string.Join("|", controllerNames));
        }
    }
}

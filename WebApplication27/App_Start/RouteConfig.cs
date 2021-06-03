using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication27
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Accounts", action = "Login", id = UrlParameter.Optional }
            );


        routes.MapRoute(
        name: "Requesters",
        url: "{Requesters}/{GetID}/{id}",
        defaults: new { controller = "Requesters", action = "GetID", id = UrlParameter.Optional},
        constraints: new { id = @"\d+" });


        }
    }
}

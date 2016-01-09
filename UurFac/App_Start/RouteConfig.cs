using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UurFac
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Klanten",
                url: "Klanten/{action}/{id}",
                defaults: new { controller = "Klants", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Tarieven",
                url: "Tarieven/{action}/{id}",
                defaults: new { controller = "Tariefs", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Departementen",
                url: "Departementen/{action}/{id}",
                defaults: new { controller = "Departements", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
               name: "GebruikerKlanten",
               url: "GebruikerKlanten/{action}/{id}",
               defaults: new { controller = "GebruikerKlants", action = "Index", id = UrlParameter.Optional }
               );

            routes.MapRoute(
               name: "Facturen",
               url: "Facturen/{action}/{id}",
               defaults: new { controller = "Factuurs", action = "Index", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );           
        }
    }
}

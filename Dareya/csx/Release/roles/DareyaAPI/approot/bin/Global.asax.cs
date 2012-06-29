using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DareyaAPI.Formatters;
using Microsoft.WindowsAzure.Diagnostics;

namespace DareyaAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new Filters.DaremetoResponseError());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public static void Configure(HttpConfiguration config)
        {
            var matches = config.Formatters
                            .Where(f => f.SupportedMediaTypes
                                         .Where(m => m.MediaType.ToString() == "application/xml" ||
                                                     m.MediaType.ToString() == "text/xml" || 
                                                     m.MediaType.ToString() == "application/json" || 
                                                     m.MediaType.ToString() == "text/json")
                                         .Count() > 0)
                            .ToList();
            foreach (var match in matches)
                config.Formatters.Remove(match);  

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            config.Formatters.Add(new JsonNetFormatter(serializerSettings));
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //BundleTable.Bundles.RegisterTemplateBundles();
            BundleTable.Bundles.EnableDefaultBundles();

            Configure(GlobalConfiguration.Configuration);
        }
    }
}
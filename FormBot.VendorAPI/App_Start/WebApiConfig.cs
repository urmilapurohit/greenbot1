using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FormBot.VendorAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //FormUrlEncodedMediaTypeFormatter f;

            //f = new FormUrlEncodedMediaTypeFormatter();
            //f.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //f.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-www-form-urlencoded"));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FormBot.Main
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapMvcAttributeRoutes();

            //URL for Reseller section
            routes.MapRoute(
          name: "Default1",
          url: "{id}",
          defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional });

            //set routes for Reseller SignUp
            routes.MapRoute(
                name: "ResellerSignUp",
                url: "Account/ResellerSignUp/{CompanyName}/{id}",
                defaults: new { controller = "Account", action = "ResellerSignUp", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "GreenbotSignUp",
                url: "Account/GreenbotSignUp/{id}/{flg}",
                defaults: new { controller = "Account", action = "GreenbotSignUp", id = UrlParameter.Optional, flg = UrlParameter.Optional });

            routes.MapRoute(
                name: "ViewDetail",
                url: "User/ViewDetail/{id}/{flg}",
                defaults: new { controller = "User", action = "ViewDetail", id = UrlParameter.Optional, flg = UrlParameter.Optional });

            routes.MapRoute(
               name: "OwnerSign",
               url: "Job/OwnerVerification/{Id}",
               defaults: new { controller = "Job", action = "_OwnerVerification", id = UrlParameter.Optional });


            //Default URL
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional });

           

        }
    }
}

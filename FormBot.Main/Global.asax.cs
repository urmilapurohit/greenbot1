using FormBot.Main.App_Start;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FormBot.Helper;
using System.Globalization;
using System.Threading;
using FormBot.Main.Infrastructure;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

namespace FormBot.Main
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly ILogger _log;

        public MvcApplication()
        {
            //_log = new Logger();
            _log = new Logger();
        }


        protected void Application_Start()
        {

            CheckApp.IsStart = "Last started : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
            //RECRegistryHelper.WriteToLogFile("Application Start " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff"));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Run();
            log4net.Config.XmlConfigurator.Configure();
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new DisallowHtmlMetadataValidationProvider());
            if (RedisCacheConfiguration.Connection == null)
            {
                RedisCacheConfiguration.InitializeAsync().Wait();
            }
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {

                var httpContext = ((MvcApplication)sender).Context;
                var currentController = " ";
                var currentAction = " ";
                var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                if (currentRouteData != null)
                {
                    if (currentRouteData.Values["controller"] != null &&
                        !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                        currentController = currentRouteData.Values["controller"].ToString();

                    if (currentRouteData.Values["action"] != null &&
                        !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                        currentAction = currentRouteData.Values["action"].ToString();
                }

                var controller = new FormBot.Main.Controllers.ErrorController();
                var routeData = new RouteData();

                _log.LogException(FormBot.Helper.SystemEnums.Severity.Error,
                        "\n Controller : " + currentController + "\n Action: " + currentAction + "\n", httpContext.Error);

                // Code that runs when an unhandled error occurs
                string ErrorPagePath = Context.Request.Url.ToString();
                Exception ErrorInfo = Server.GetLastError().GetBaseException();
                var st = new StackTrace(ErrorInfo, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                //FormBot.Email.Log.WriteLine("", "Error Date: " + DateTime.Now.ToString("yyyy-MM-dd") + " Controller: " + currentController + " Action: " + currentAction + " Error Message:" + ErrorInfo.Message);            
                //RECRegistryHelper.WriteToLogFile("Error Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:s:ss") + " Controller: " + currentController + " Action: " + currentAction + "Error Line No:" + line + " Error Message:" + ErrorInfo.Message);
                //FormBot.Email.Log.WriteLine("", "Error Date: " + DateTime.Now.ToString("yyyy-MM-dd") + " Controller: " + currentController + " Action: " + currentAction + " Error Message:" + ErrorInfo.Message);            
                //RECRegistryHelper.WriteToLogFile("Error Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Controller: " + currentController + " Action: " + currentAction + " Error Line No:" + line + " Error Message:" + ErrorInfo.Message);
                _log.LogException("Error Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Controller: " + currentController + " Action: " + currentAction + " Error Line No:" + line + " Error Message:" + ErrorInfo.Message,null);
                httpContext.ClearError();
                httpContext.Response.Clear();

                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = "Error";

                controller.ViewData.Model = new HandleErrorInfo(ErrorInfo, currentController, currentAction);
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
                Response.Redirect(String.Format("~/Error/Error"));
                //Response.Redirect("~/Views/Shared/Error.cshtml");
            }
            catch (Exception ex)
            {

            }
        }

        protected void Application_BeginREquest(Object sender, EventArgs e)
        {
            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            newCulture.DateTimeFormat.DateSeparator = "-";
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }


    }
}

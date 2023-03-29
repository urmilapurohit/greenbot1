using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using FormBot.Helper;
using FormBot.SPVMain.App_Start;

namespace FormBot.SPVMain
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly Logger _logger;

        public MvcApplication()
        {
            _logger = new Logger();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Run();
        }
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

                var controller = new FormBot.SPVMain.Controllers.ErrorController();
                var routeData = new RouteData();

                _logger.LogException(FormBot.Helper.SystemEnums.Severity.Error,
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
                WriteToLogFile("Error Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Controller: " + currentController + " Action: " + currentAction + " Error Line No:" + line + " Error Message:" + ErrorInfo.Message);
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
        public static void WriteToLogFile(string content)
        {
            FileStream fs = null;
            try
            {
                content = Environment.NewLine + content;
                //set up a filestream
                fs = new FileStream(HttpContext.Current.Server.MapPath("~/test.txt"), FileMode.OpenOrCreate, FileAccess.Write);
                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //add the text
                    sw.WriteLine(content);
                    //add the text to the underlying filestream
                    sw.Flush();
                    //close the writer
                    sw.Close();
                }
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                //throw;
            }
            //StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream            
        }
    }
}

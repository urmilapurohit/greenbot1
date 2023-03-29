using System.Web;
using System.Web.Optimization;

namespace FormBot.SPVMain
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/reset.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/datepicker.css",
                      "~/Content/style.min.css",
                      "~/Content/theme.css",
                      "~/Content/Formbot.css",
                      "~/Content/Site.css",
                      "~/Content/easy-responsive-tabs.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/timeout-dialog.css",
                      "~/Content/lightbox.css",
                      "~/Content/InvoiceDetail.css",
                      "~/Content/jquery.mCustomScrollbar.css"));

            bundles.Add(new ScriptBundle("~/bundles/CommonJS").Include(
               "~/Scripts/jquery-1.10.2.min.js",
               "~/Scripts/JqueryUi.min.js",
               "~/Scripts/jquery.validate-vsdoc.js",
               "~/Scripts/jquery.validate.min.js",
               "~/Scripts/jquery.validate.unobtrusive.min.js",
               "~/Scripts/bootstrap.min.js",
               "~/Scripts/bootstrap-datepicker.js",

               "~/Scripts/general.js",
              //"~/Scripts/Calendar/moment.min.js",
              "~/Scripts/dropdown.js",

              "~/Scripts/jquery.fileupload.js",
              //"~/Scripts/FormBot.min.js",

               "~/Scripts/jquery.dataTables-1.10.10.min.js",
               "~/Scripts/dataTables.responsive.min.js"));

            //Apply Bundling using this flag
            BundleTable.EnableOptimizations = true;

        }
    }
}

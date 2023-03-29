using FormBot.Helper;
using System.Web;
using System.Web.Optimization;

namespace FormBot.Main
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-1.10.2.min.js",
            //"~/Scripts/JqueryUi.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JobDetailNewScreen").Include(
                  //"~/Scripts/View/STCTradeModule.js",
                  "~/Scripts/CommonModularAndTabularJob.min.js",
                  //"~/Scripts/View/Job/_JobPhotosNew.min.js",
                  "~/Scripts/View/Job/_JobSystemDetails.min.js",
                  //"~/Scripts/View/Job/_InstallerDesignerElectrician.min.js",
                  "~/Scripts/View/Job/_DocumentManager.min.js"
           ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate-vsdoc.js", "~/Scripts/jquery.validate.min.js", "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.min.js",
            //          "~/Scripts/bootstrap-datepicker.js",
            //          "~/Scripts/respond.min.js",
            //          "~/Scripts/fastclick.min.js",
            //          "~/Scripts/general.js"));

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
                      "~/Content/jquery.mCustomScrollbar.css",
                      "~/Content/js-snackbar.css"

                      )); // Add timeout css (By Rajnikant serasiya)

            //bundles.Add(new ScriptBundle("~/datatableJs").Include(
            //          "~/Scripts/jquery.dataTables-1.10.10.min.js",
            //          "~/Scripts/dataTables.responsive.min.js"));

            bundles.Add(new StyleBundle("~/datatableCSS").Include(
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/responsive.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/kendoCSS").Include(
                      "~/Content/kendo/styles/kendo.common-material.min.css" ,
                      "~/Content/kendo/styles/kendo.material.min.css" ,
                      "~/Content/kendo/styles/kendo.material.mobile.min.css" ,
                      "~/Content/kendo/styles/kendo.custom.css", 
                      "~/Content/KendoGrid.css" ));

            bundles.Add(new StyleBundle("~/JobIndexBootstrapCSS").Include(
                    "~/Content/ColReorder.datatables.min.css" ,
                    "~/Content/excel-bootstrap-table-filter-style.css" ));

            bundles.Add(new ScriptBundle("~/ChartJs").Include(
                      "~/Scripts/Charts/d3.v3.min.js",
                      "~/Scripts/Charts/c3.js"));

           

            //bundles.Add(new ScriptBundle("~/Timeout_MomentJS").Include(
            //          "~/Scripts/Calendar/moment.min.js"));

            bundles.Add(new ScriptBundle("~/Report_ExportJS").Include(
                   "~/Scripts/ReportJS/dataTables.buttons.min.js",
                   "~/Scripts/ReportJS/buttons.flash.min.js",
                   "~/Scripts/ReportJS/buttons.html5.min.js",
                   "~/Scripts/ReportJS/jszip.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile").Include(
                    "~/Scripts/easyResponsiveTabs.js",
                    "~/Areas/Email/js/ckeditor/ckeditor.js",
                    "~/Areas/Email/js/ckeditor/samples/js/sample.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/CommonJS").Include(

              "~/Scripts/jquery-1.10.2.min.js",
               "~/Scripts/JqueryUi.min.js",

               "~/Scripts/jquery.validate-vsdoc.js",
               "~/Scripts/jquery.validate.min.js",
               "~/Scripts/jquery.validate.unobtrusive.min.js",

               "~/Scripts/bootstrap.min.js",
               "~/Scripts/bootstrap-datepicker.js",
               "~/Scripts/respond.min.js",
               "~/Scripts/fastclick.min.js",
               "~/Scripts/general.js",

              "~/Scripts/jquery.dataTables-1.10.10.min.js",
              "~/Scripts/dataTables.responsive.min.js",
              "~/Scripts/Calendar/moment.min.js",
              "~/Scripts/dropdown.js",

              "~/Scripts/FormBot.min.js",
              "~/Scripts/jquery.fileupload.js"
            
              //"~/Scripts/datetime.min.js",
              //"~/Scripts/View/Shared/_Layout.min.js",
              //"~/url/https://www.googletagmanager.com/gtag/js?id=UA-114902803-1"
              ));

            bundles.Add(new ScriptBundle("~/bundles/JobDetailsExtra").Include(
                   "~/Scripts/jqueryrotate.min.js",
                  "~/Scripts/set-number.min.js",
                  "~/Scripts/jquery.autosize.min.js"
                  //"~/Scripts/View/Job/ViewAndEditNewJob.min.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/STCSubmission").Include(
                 "~/Scripts/jquery.nicescroll.min.js",
                 "~/Scripts/datetime.min.js"
                
                ));
            bundles.Add(new ScriptBundle("~/bundles/SAASInvoiceBuilder").Include(
                "~/Scripts/kendo/js/kendo.all.min.js",
                 "~/Scripts/jquery.nicescroll.min.js",
                 "~/Scripts/datetime.min.js",
                 "~/Scripts/colResizable-1.6.min.js",
                 "~/Scripts/dataTables.colReorder.min.js",
                 "~/Scripts/View/SAASInvoiceBuilder.min.js"

                ));
            bundles.Add(new ScriptBundle("~/bundles/JobIndex").Include(
                    "~/Scripts/jquery.nicescroll.min.js",
                    "~/Scripts/colResizable-1.6.min.js" ,
                     "~/Scripts/dataTables.colReorder.min.js", 
                     "~/Scripts/dataTables.buttons.min.js", 
                     "~/Scripts/jszip.min.js" ,
                     "~/Scripts/buttons.html5.min.js" ,
                     "~/Scripts/excel-bootstrap-table-filter-bundle.js" 

                ));

            bundles.Add(new ScriptBundle("~/bundles/UserDetail").Include(
                "~/Scripts/kendo/js/kendo.all.min.js",
                "~/Scripts/User/CommonRulesForUser.js",
                "~/Scripts/User/UserDetail.min.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/LayoutDetail").Include(
                "~/Scripts/View/Shared/_Layout.min.js",
                "~/Scripts/GeneralFunction.js",
                "~/Scripts/View/Job/js-snackbar.js",
                "~/Scripts/Layout/googleTagManager.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/UserProfile").Include(
                "~/Scripts/easyResponsiveTabs.js",
                "~/Scripts/User/Profile.min.js"
              ));

            bundles.Add(new ScriptBundle("~/bundles/CKEditor").Include(
               "~/Areas/Email/js/ckeditor/ckeditor.js",
              "~/Areas/Email/js/ckeditor/samples/js/sample.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/UserViewDetail").Include(
               "~/Scripts/easyResponsiveTabs.js",
              "~/Scripts/jquery.fileupload.js",
              "~/Scripts/User/ViewDetail.dev.js",
              "~/Scripts/Formbot.min.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/UserCreate").Include(
              "~/Scripts/easyResponsiveTabs.js",
              "~/Scripts/User/CommonRulesForUser.js",
              "~/Scripts/jquery.sumoselect.js",
              "~/Scripts/User/Create.min.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/UserEdit").Include(
              "~/Scripts/easyResponsiveTabs.js",
              "~/Scripts/jquery.sumoselect.js",
              "~/Scripts/jquery.fileupload.js",
              "~/Scripts/User/Edit.min.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/UserMyProfile").Include(
             "~/Scripts/easyResponsiveTabs.js",
             "~/Scripts/jquery.fileupload.js",
             "~/Scripts/User/MyProfile.dev.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/UserView").Include(
              "~/Scripts/easyResponsiveTabs.js",
              "~/Scripts/jquery.sumoselect.js",
              "~/Scripts/jquery.fileupload.js",
              "~/Scripts/User/View.dev.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/JobModularSubViews").Include(
              //"~/Scripts/View/Job/PreApprovalConnectionSTC.min.js",
              //"~/Scripts/View/Job/_JobPhotosNew.min.js",
              "~/Scripts/jquery.validate-vsdoc.js", 
              "~/Scripts/jquery.validate.min.js",
              "~/Scripts/jquery.validate.unobtrusive.min.js",
              "~/Scripts/View/Job/JobVisit.min.js",
              "~/Scripts/View/STCTradeModule.min.js"
             //"~/Scripts/View/Job/PricingSettlementTerm.min.js"
             ));

            //Apply Bundling using this flag
            BundleTable.EnableOptimizations = true;
        }
    }
}

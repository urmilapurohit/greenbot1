using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using FormBot.Entity.Dashboard;

namespace FormBot.Main.Controllers
{
    public class FormBotReportController : Controller
    {
        #region Properties
        private readonly IFormBotReportBAL _formBotReportService;
        private readonly IResellerBAL _resellerService;
        #endregion

        #region Constructor
        public FormBotReportController(IFormBotReportBAL formBotReportService, IResellerBAL resellerService)
        {
            this._formBotReportService = formBotReportService;
            this._resellerService = resellerService;
        }
        #endregion

        #region Event
        [UserAuthorization]
        public ActionResult Index()
        {
            FormBotReport model = new FormBotReport();
            var lstReport = _formBotReportService.GetFormBotReportDrp();
            lstReport = lstReport.Where(s => s.ReportName != "SE User Level Report" && s.ReportName != "SSC, SE Jobs Detail Report").ToList();
            model.lstReport = lstReport;
            return View(model);
        }
        public ActionResult _NonTradeJobs()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// get solar company.
        /// </summary>
        /// <param name="hdnResellers">hdnResellers.</param>
        /// <returns>action result</returns>
        public ActionResult _SolarCompany(string hdnResellers = "")
        {
            FormBotReport model = new FormBotReport();
            var lstSolarCompany = _formBotReportService.GetSolarCompany("",hdnResellers);
            model.LstSolarCompany = lstSolarCompany;
            model.LstSolarCompanyAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// get solar company For Active.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _SolarCompanyForActive()
        {
            FormBotReport model = new FormBotReport();
            var lstSolarCompany = _formBotReportService.GetSolarCompany();
            model.LstSolarCompany = lstSolarCompany;
            model.LstSolarCompanyAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// get preapproval status.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _Preapproval()
        {
            FormBotReport model = new FormBotReport();
            var lstPreapprovalStatus = _formBotReportService.GetPreapprovalStatus();
            model.LstPreapprovalStatus = lstPreapprovalStatus;
            model.LstPreapprovalStatusAssigned = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// get connection status.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _Connection()
        {
            FormBotReport model = new FormBotReport();
            var lstConnectionStatus = _formBotReportService.GetConnectionStatus();
            model.LstConnectionStatus = lstConnectionStatus;
            model.LstConnectionStatusAssigned = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// _STCSubmission
        /// </summary>
        /// <param name="report">report</param>
        /// <returns>ActionResult</returns>
        public ActionResult _STCSubmission(string report = "")
        {
            FormBotReport model = new FormBotReport();
            var lstStcSubmissionStatus = _formBotReportService.GetSTCSubmissionStatus();
            if (!string.IsNullOrEmpty(report) && report.ToLower() == "jobstatusbardetailsreport")
                model.LstSTCSubmissionStatus = lstStcSubmissionStatus.Where(s => s.Text.ToLower() != "not yet submitted" && s.Text.ToLower() != "submit to trade").ToList();
            else
                model.LstSTCSubmissionStatus = lstStcSubmissionStatus;
            model.LstSTCSubmissionStatusAssigned = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the users list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _UsersList(string id)
        {
            FormBotReport model = new FormBotReport();
            if (id == "4")
            {
                model.LstUsers = _formBotReportService.GetSolarCompany();
            }
            else
            {
                model.LstUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id));
            }
            model.LstSolarCompanyAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the SSC view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _SSCView(string id)
        {
            FormBotReport model = new FormBotReport();
            model.LstSSCUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id));
            model.LstSSCAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the sc view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _SCView(string id)
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 9)
                model.LstSCUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id), string.Empty, ProjectSession.LoggedInUserId);
            else
                model.LstSCUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id));

            //model.LstSCAssignedUser = new List<SelectListItem>();
            model.LstSCAssignedUser = model.LstSCUsers.Where(x => x.Value == ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            model.LstSCUsers = model.LstSCUsers.Where(x => x.Value != ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            return View(model);
        }

        /// <summary>
        /// the se view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _SEView(string id)
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 7)
                model.LstSEUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id), string.Empty, ProjectSession.LoggedInUserId);
            else
                model.LstSEUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id)); 

            //model.LstSEAssignedUser = new List<SelectListItem>();
            model.LstSEAssignedUser = model.LstSEUsers.Where(x => x.Value == ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            model.LstSEUsers = model.LstSEUsers.Where(x => x.Value != ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            return View(model);
        }

        public ActionResult _SWHView(string id)
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 10)
                model.LstSWHUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id), string.Empty, ProjectSession.LoggedInUserId);
            else
                model.LstSWHUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id)); // FOR SE - 7
            
            model.LstSWHAssignedUser = model.LstSWHUsers.Where(x => x.Value == ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            model.LstSWHUsers = model.LstSWHUsers.Where(x => x.Value != ProjectSession.LoggedInUserId.ToString()).Select(X => X).ToList();
            return View(model);
        }

        /// <summary>
        /// the reseller details.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _Reseller()
        {
            FormBotReport model = new FormBotReport();
            model.LstResellerUsers = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            model.LstResellerAssignedUser = new List<SelectListItem>();
            return View(model);
        }
        /// <summary>
        /// the reseller For Active.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult ResellerForActive()
        {
            FormBotReport model = new FormBotReport();
            model.LstResellerUsers = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            model.LstResellerAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the sco view
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="hdnSolarCompanies">hdnSolarCompanies</param>
        /// <returns>ActionResult</returns>
        public ActionResult _SCOView(string id, string hdnSolarCompanies = "")
        {
            FormBotReport model = new FormBotReport();
            model.LstSCOUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id), (string.IsNullOrEmpty(hdnSolarCompanies) || hdnSolarCompanies == "undefined") ? "" : hdnSolarCompanies, ProjectSession.LoggedInUserId);
            model.LstSCOAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// _RAMView
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="assignedRAM">assignedRAM</param>
        /// <param name="hdnResellers">hdnResellers</param>
        /// <returns>ActionResult</returns>
        public ActionResult _RAMView(string id = "", int assignedRAM = 0, string hdnResellers = "")
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 2)
            {
                model.ResellerID = ProjectSession.ResellerId;
            }
            else
                model.ResellerID = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            model.hdnResellers = (string.IsNullOrEmpty(hdnResellers) || hdnResellers == "undefined") ? "" : hdnResellers;

            model.LstRAMUsers = _formBotReportService.GetFormBotRAMUser(model);

            if (assignedRAM == 0)
                model.LstRAMAssignedUser = new List<SelectListItem>();

            return View(model);
        }

        /// <summary>
        /// the fco view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _FCOView(string id)
        {
            FormBotReport model = new FormBotReport();
            model.LstFCOUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id));
            model.LstFCOAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the fsa view.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>action result</returns>
        public ActionResult _FSAView(string id)
        {
            FormBotReport model = new FormBotReport();
            model.LstFSAUsers = _formBotReportService.GetFormBotUser(Convert.ToInt32(id));
            model.LstFSAAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the invoice view.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _InvoiceView()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the invoice status.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _InvoiceStatus()
        {
            FormBotReport model = new FormBotReport();
            model.LstStatusUsers = new List<SelectListItem>() 
            { 
                new SelectListItem() { Value = "1", Text = "Paid in Full" },
                new SelectListItem() { Value = "2", Text = "Cancelled" },
                new SelectListItem() { Value = "3", Text = "Partial Payment" },
                new SelectListItem() { Value = "4", Text = "Outstanding" }
            };
            model.LstStatusAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// the job status bar total reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _JobStatusBarTotalReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the job status detail reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _JobStatusDetailReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the job stages report.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _JobStagesReport()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the total jobs reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _TotalJobsReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the system user reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _SystemUserReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the allocation report.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _AllocationReport()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the active user report.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _ActiveUserReport()
        {
            FormBotReport model = new FormBotReport();
            model.LoginUserType = ProjectSession.UserTypeId;
            return View(model);
        }

        /// <summary>
        /// the se user level reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _SEUserLevelReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// the record failure reasons reports.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _RECFailureReasonsReports()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// Get sold by who report
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _SoldByWhoReport()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// Get job stage
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _JobStages()
        {
            FormBotReport model = new FormBotReport();
            model.LstJobStages = _formBotReportService.GetJobStages();
            model.LstJobStagesAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// _SalesAgent
        /// </summary>
        /// <param name="hdnSolarCompanies">hdnSolarCompanies</param>
        /// <returns>ActionResult</returns>
        public ActionResult _SalesAgent(string hdnSolarCompanies = "")
        {
            hdnSolarCompanies = (string.IsNullOrEmpty(hdnSolarCompanies) || hdnSolarCompanies == "undefined") ? "" : hdnSolarCompanies;
            FormBotReport model = new FormBotReport();
            model.LstSalesAgent = _formBotReportService.GetSalesAgent(hdnSolarCompanies);
            model.LstSalesAgentAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// Get SSC/SE job detail report
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _SSCSEJobsDetailReport()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// Get STC general report
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _STCGeneralReport()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        /// <summary>
        /// Get STC general report
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult _STCGeneralReportForRAM()
        {
            FormBotReport model = new FormBotReport();
            return View(model);
        }

        [HttpGet]
        public JsonResult GetSolarCompanyOfSelectedReseller(string resellerIds = "")
        {
            IEnumerable<SelectListItem> items = _formBotReportService.GetSolarCompany("",resellerIds);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the job status bar total reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetJobStatusBarTotalReportsList(FormBotReport formbotReportModel)
        {
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            IList<FormBotReport> lstRecord = _formBotReportService.GetJobStatusBarTotalReportsList(formbotReportModel);
            return Json(lstRecord, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Total Active Users Report Data
        /// </summary>
        /// <param name="formbotReportModel">formbot ReportModel</param>
        /// <returns>json result</returns>
        public JsonResult GetTotalActiveUsersReport(FormBotReport formbotReportModel)
        {
            //if (ProjectSession.UserTypeId == 4)
            //{
            //    formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            //}
            IList<FormBotReport> lstRecord = _formBotReportService.GetTotalActiveUsersReport(formbotReportModel);
            return Json(lstRecord, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the job stages report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetJobStagesReport(FormBotReport formbotReportModel)
        {
            JobStagesReport obj = new JobStagesReport();
            formbotReportModel.LogginUserID = ProjectSession.LoggedInUserId;
            formbotReportModel.LoginUserType = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            IList<FormBotReport> lstRecord = new List<FormBotReport>();
            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
            {
                DataSet dsReport = _formBotReportService.GetJobStagesReport(formbotReportModel);
                obj.lstRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
                //lstRecord = _formBotReportService.GetJobStagesReport(formbotReportModel);
            }
            else
            {
                DataSet dsReport = _formBotReportService.GetJobStagesReport(formbotReportModel);
                obj.lstRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
                obj.lstChartRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            }


            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the job status detail reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetJobStatusDetailReportsList(FormBotReport formbotReportModel)
        {
            JobStatusDetailReport objJobStatusDetailReport = new JobStatusDetailReport();
            formbotReportModel.LogginUserID = ProjectSession.LoggedInUserId;
            formbotReportModel.LoginUserType = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }

            DataSet dsReport = _formBotReportService.GetJobStatusDetailReportsList(formbotReportModel);
            objJobStatusDetailReport.lstJobDetailsRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            objJobStatusDetailReport.lstJob_StatuswiseRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            return Json(objJobStatusDetailReport, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the system user reports reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetSystemUserReportsReportsList(FormBotReport formbotReportModel)
        {
            SystemUserReport obj = new SystemUserReport();

            if (formbotReportModel.IsAllSelected == true)
            {
                formbotReportModel.hdnFormBotResellerAssignedUser = null;
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = null;
            }
            if (ProjectSession.UserTypeId == 2)
            {
                formbotReportModel.hdnFormBotResellerAssignedUser = Convert.ToString(ProjectSession.ResellerId);
            }
            DataSet dsReport = _formBotReportService.GetSystemUserReportsReportsList(formbotReportModel);
            obj.lstRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.lstChartRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the total j ob report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        [HttpPost]
        public JsonResult GetTotalJObReport(FormBotReport formbotReportModel)
        {
            List<FormBotReport> lstRecord = _formBotReportService.GetTotalJobList(formbotReportModel);
            return Json(lstRecord, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the invoice report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        [HttpPost]
        public JsonResult GetInvoiceReport(FormBotReport formbotReportModel)
        {
            if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
            {
                formbotReportModel.ResellerID = ProjectSession.ResellerId;
                formbotReportModel.hdnFormBotResellerAssignedUser = Convert.ToString(ProjectSession.ResellerId);
            }
            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
            {
                formbotReportModel.hdnFormBotSEAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
            }
            if (ProjectSession.UserTypeId == 6)
            {
                formbotReportModel.hdnFormBotSSCAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
            }
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            DataSet dataset = _formBotReportService.GetJobInvoiceReport(formbotReportModel);
            if (dataset != null && dataset.Tables.Count > 0)
            {
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    if (dataset.Tables[0].Rows[0].ItemArray[0].ToString() != "0")
                    {
                        return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dataset.Tables[0]), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the allocation report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        [HttpPost]
        public JsonResult GetAllocationReportList(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            DataSet dsReport = _formBotReportService.GetAllocationReportList(formbotReportModel);
            //if (ProjectSession.UserTypeId == 7)
            //{
            //    obj.lstTotalJobRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            //    obj.lstJobDetailsRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            //}
            //if (ProjectSession.UserTypeId == 9 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8 || ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 5 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 4)
            //{
            obj.lstTotalJobRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.lstAllocationReportChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            //}
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the se user level reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetSEUserLevelReportsList(FormBotReport formbotReportModel)
        {
            if (ProjectSession.UserTypeId == 1)
            {
                formbotReportModel.LogginUserID = 0;
            }
            else if (ProjectSession.UserTypeId == 7)
            {
                formbotReportModel.LogginUserID = ProjectSession.LoggedInUserId;
            }
            List<FormBotReport> lstRecord = _formBotReportService.GetSEUserLevelReportsList(formbotReportModel);
            return Json(lstRecord, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetRECFailureReasonsReportsList(FormBotReport formbotReportModel)
        {
            RecFailureReasonReport obj = new RecFailureReasonReport();
            if (ProjectSession.UserTypeId == 2)
            {
                formbotReportModel.hdnFormBotResellerAssignedUser = Convert.ToString(ProjectSession.ResellerId);
            }
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }

            DataSet dsReport = _formBotReportService.GetRECFailureReasonsReportsList(formbotReportModel);
            obj.lstRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.lstChartRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            //List<FormBotReport> lstRecord = _formBotReportService.GetRECFailureReasonsReportsList(formbotReportModel);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the sold by who report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetSoldByWhoReportList(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            //TODO:
            DataSet dsReport = _formBotReportService.GetSoldByWhoReportList(formbotReportModel);
            obj.dsSoldByWho = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSoldByWhoInnerTable = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate CSV For Sold By Who Grouping report
        /// </summary>
        /// <param name="StartDate">Start Date</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="hdnFormBotResellerAssignedUser">hdnFormBotResellerAssignedUser</param>
        /// <param name="hdnFormBotSolarCompanyAssignedUser">hdnFormBotSolarCompanyAssignedUser</param>
        /// <param name="hdnJobStagesAssignedUser">hdnJobStagesAssignedUser</param>
        /// <param name="hdnSalesAgentAssignedUser">hdnSalesAgentAssignedUser</param>
        public void GenerateSoldByWhoReportCSV(string StartDate, string EndDate, string hdnFormBotResellerAssignedUser, string hdnFormBotSolarCompanyAssignedUser, string hdnJobStagesAssignedUser, string hdnSalesAgentAssignedUser)
        {
            FormBotReport formbotReportModel = new FormBotReport();
            if (!string.IsNullOrEmpty(StartDate))
                formbotReportModel.StartDate = Convert.ToDateTime(StartDate);
            if (!string.IsNullOrEmpty(EndDate))
                formbotReportModel.EndDate = Convert.ToDateTime(EndDate);
            formbotReportModel.hdnFormBotResellerAssignedUser = hdnFormBotResellerAssignedUser;
            formbotReportModel.hdnFormBotSolarCompanyAssignedUser = hdnFormBotSolarCompanyAssignedUser;
            formbotReportModel.hdnJobStagesAssignedUser = hdnJobStagesAssignedUser;
            formbotReportModel.hdnSalesAgentAssignedUser = hdnSalesAgentAssignedUser;

            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }

            DataSet dsReport = _formBotReportService.GetSoldByWhoReportList(formbotReportModel);
            obj.dsSoldByWho = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSoldByWhoInnerTable = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            DataTable dt = dsReport.Tables[0];

            StringBuilder csv = new StringBuilder();
            csv.Append(@"Sold By,Reference Number,Stage Name,Sold By Date");
            csv.Append("\r\n");

            string SoldBy = string.Empty;
            string StageName = string.Empty;
            var New_StageCount = 0;
            var NewInstallation_StageCount = 0;
            var Preapproval_StageCount = 0;
            var InProgress_StageCount = 0;
            var Complete_StageCount = 0;
            var STCTrade_StageCount = 0;
            var Aftersales_StageCount = 0;
            var Cancellations_StageCount = 0;
            var InstallationCompleted_StageCount = 0;
            string SoldByDate = string.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object value = dt.Rows[i]["SoldByDate"];
                if (value != DBNull.Value)
                    SoldByDate = Convert.ToDateTime(dt.Rows[i]["SoldByDate"]).ToString("yyyy-MM-dd");

                if (SoldBy == Convert.ToString(dt.Rows[i]["SoldBy"]) || string.IsNullOrEmpty(SoldBy))
                {
                    SoldBy = Convert.ToString(dt.Rows[i]["SoldBy"]);
                    StageName = Convert.ToString(dt.Rows[i]["StageName"]);

                    if (StageName == "New")
                        New_StageCount += 1;
                    if (StageName == "New Installation")
                        NewInstallation_StageCount += 1;
                    if (StageName == "Preapproval")
                        Preapproval_StageCount += 1;
                    if (StageName == "In Progress")
                        InProgress_StageCount += 1;
                    if (StageName == "Complete")
                        Complete_StageCount += 1;
                    if (StageName == "STC Trade")
                        STCTrade_StageCount += 1;
                    if (StageName == "Aftersales")
                        Aftersales_StageCount += 1;
                    if (StageName == "Cancellations")
                        Cancellations_StageCount += 1;
                    if (StageName == "Installation Completed")
                        InstallationCompleted_StageCount += 1;

                    csv.Append(Convert.ToString(dt.Rows[i]["SoldBy"]) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + "," + Convert.ToString(dt.Rows[i]["StageName"]) + "," + SoldByDate + ",");
                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                else
                {
                    csv.Append("New(" + New_StageCount + ")" + "Preapproval(" + Preapproval_StageCount + "),New Installation(" + NewInstallation_StageCount + ")" + "In Progress(" + InProgress_StageCount + ")," +
                             "Complete(" + Complete_StageCount + ")" + "STC Trade(" + STCTrade_StageCount + "),Aftersales(" + Aftersales_StageCount + ")" + "Cancellations(" + Cancellations_StageCount + ")," +
                             "Installation Completed(" + InstallationCompleted_StageCount + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                    csv.Append("\r\n");

                    SoldBy = Convert.ToString(dt.Rows[i]["SoldBy"]);
                    StageName = Convert.ToString(dt.Rows[i]["StageName"]);

                    New_StageCount = 0;
                    NewInstallation_StageCount = 0;
                    Preapproval_StageCount = 0;
                    InProgress_StageCount = 0;
                    Complete_StageCount = 0;
                    STCTrade_StageCount = 0;
                    Aftersales_StageCount = 0;
                    Cancellations_StageCount = 0;
                    InstallationCompleted_StageCount = 0;

                    if (StageName == "New")
                        New_StageCount += 1;
                    if (StageName == "New Installation")
                        NewInstallation_StageCount += 1;
                    if (StageName == "Preapproval")
                        Preapproval_StageCount += 1;
                    if (StageName == "In Progress")
                        InProgress_StageCount += 1;
                    if (StageName == "Complete")
                        Complete_StageCount += 1;
                    if (StageName == "STC Trade")
                        STCTrade_StageCount += 1;
                    if (StageName == "Aftersales")
                        Aftersales_StageCount += 1;
                    if (StageName == "Cancellations")
                        Cancellations_StageCount += 1;
                    if (StageName == "Installation Completed")
                        InstallationCompleted_StageCount += 1;

                    csv.Append(Convert.ToString(dt.Rows[i]["SoldBy"]) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + "," + Convert.ToString(dt.Rows[i]["StageName"]) + "," + SoldByDate + ",");
                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                if (i == dt.Rows.Count - 1)
                {
                    csv.Append("New(" + New_StageCount + ")" + "Preapproval(" + Preapproval_StageCount + "),New Installation(" + NewInstallation_StageCount + ")" + "In Progress(" + InProgress_StageCount + ")," +
                                "Complete(" + Complete_StageCount + ")" + "STC Trade(" + STCTrade_StageCount + "),Aftersales(" + Aftersales_StageCount + ")" + "Cancellations(" + Cancellations_StageCount + ")," +
                                "Installation Completed(" + InstallationCompleted_StageCount + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=SoldByWho_" + DateTime.Now.Ticks.ToString() + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Gets the sscse jobs detail report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult GetSSCSEJobsDetailReportList(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 6)
            {
                formbotReportModel.hdnFormBotSSCAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
            }
            else if (ProjectSession.UserTypeId == 7)
            {
                formbotReportModel.hdnFormBotSEAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            DataSet dsReport = _formBotReportService.GetSSCSEJobsDetailReportList(formbotReportModel);
            obj.dsSSCSEJobsDetail = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSSCSEJobsDetailInnerTable = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate CSV For SSE SC Job detail Grouping report
        /// </summary>
        /// <param name="hdnFormBotSSCAssignedUser">hdnFormBot SSCAssignedUser</param>
        /// <param name="hdnFormBotSEAssignedUser">hdnFormBot SEAssignedUser</param>
        /// <param name="hdnFormBotSolarCompanyAssignedUser">hdnFormBot SolarCompanyAssignedUser</param>
        /// <param name="JobStageID">JobStage ID</param>
        public void Generate_SSE_SC_JobDetail_CSV(string hdnFormBotSSCAssignedUser, string hdnFormBotSEAssignedUser, string hdnFormBotSolarCompanyAssignedUser, int JobStageID)
        {
            FormBotReport formbotReportModel = new FormBotReport();
            formbotReportModel.hdnFormBotSSCAssignedUser = hdnFormBotSSCAssignedUser;
            formbotReportModel.hdnFormBotSEAssignedUser = hdnFormBotSEAssignedUser;
            formbotReportModel.hdnFormBotSolarCompanyAssignedUser = hdnFormBotSolarCompanyAssignedUser;
            formbotReportModel.JobStageID = JobStageID;

            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 6)
            {
                formbotReportModel.hdnFormBotSSCAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
            }
            else if (ProjectSession.UserTypeId == 7)
            {
                formbotReportModel.hdnFormBotSEAssignedUser = Convert.ToString(ProjectSession.LoggedInUserId);
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            DataSet dsReport = _formBotReportService.GetSSCSEJobsDetailReportList(formbotReportModel);
            obj.dsSSCSEJobsDetail = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSSCSEJobsDetailInnerTable = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);

            DataTable dt = dsReport.Tables[0];

            StringBuilder csv = new StringBuilder();
            csv.Append(@"Reference Number,Owner Name,Visit Date,Invoice Sent?,Stage Name,Company Name");
            csv.Append("\r\n");

            string SolarCompanyName = string.Empty;
            string StageName = string.Empty;
            var Completed_Job = 0;
            var Outstanding_Job = 0;
            var Final_Total_Job = 0;
            var Final_Completed_Job = 0;
            var Final_Outstanding_Job = 0;
            var New_StageCount = 0;
            var NewInstallation_StageCount = 0;
            var Preapproval_StageCount = 0;
            var InProgress_StageCount = 0;
            var Complete_StageCount = 0;
            var STCTrade_StageCount = 0;
            var Aftersales_StageCount = 0;
            var Cancellations_StageCount = 0;
            var InstallationCompleted_StageCount = 0;
            string VisitDate = string.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object value = dt.Rows[i]["visitdate"];
                if (value != DBNull.Value)
                    VisitDate = Convert.ToDateTime(dt.Rows[i]["visitdate"]).ToString("yyyy-MM-dd");

                if (SolarCompanyName == Convert.ToString(dt.Rows[i]["CompanyName"]) || string.IsNullOrEmpty(SolarCompanyName))
                {
                    SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);
                    StageName = Convert.ToString(dt.Rows[i]["StageName"]);

                    if (StageName == "New")
                        New_StageCount += 1;
                    if (StageName == "Preapproval")
                        Preapproval_StageCount += 1;
                    if (StageName == "New Installation")
                        NewInstallation_StageCount += 1;
                    if (StageName == "In Progress")
                        InProgress_StageCount += 1;
                    if (StageName == "Complete")
                        Complete_StageCount += 1;
                    if (StageName == "STC Trade")
                        STCTrade_StageCount += 1;
                    if (StageName == "Aftersales")
                        Aftersales_StageCount += 1;
                    if (StageName == "Cancellations")
                        Cancellations_StageCount += 1;
                    if (StageName == "Installation Completed")
                        InstallationCompleted_StageCount += 1;

                    if (StageName == "New Installation" || StageName == "In Progress")
                    {
                        Outstanding_Job += 1;
                        Final_Outstanding_Job += Outstanding_Job;
                    }
                    else if (StageName == "New" || StageName == "Preapproval" || StageName == "Complete" || StageName == "STC Trade" || StageName == "Aftersales" || StageName == "Cancellations" || StageName == "Installation Completed")
                    {
                        Completed_Job += 1;
                        Final_Completed_Job += Completed_Job;
                    }

                    csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["RefNumber"]) + "," + Convert.ToString(dt.Rows[i]["Owner_Name"]) + ","
                                + VisitDate + "," + Convert.ToString(dt.Rows[i]["InvoiceSent"]) + "," + Convert.ToString(dt.Rows[i]["StageName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])));
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                else
                {

                    csv.Append(",,,Total Jobs(" + (Completed_Job + Outstanding_Job) + ")," + "Completed Jobs(" + Completed_Job + "),Outstanding Jobs(" + Outstanding_Job + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                    csv.Append("\r\n");

                    SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);
                    StageName = Convert.ToString(dt.Rows[i]["StageName"]);

                    New_StageCount = 0;
                    NewInstallation_StageCount = 0;
                    Preapproval_StageCount = 0;
                    InProgress_StageCount = 0;
                    Complete_StageCount = 0;
                    STCTrade_StageCount = 0;
                    Aftersales_StageCount = 0;
                    Cancellations_StageCount = 0;
                    InstallationCompleted_StageCount = 0;

                    if (StageName == "New")
                        New_StageCount += 1;
                    if (StageName == "New Installation")
                        NewInstallation_StageCount += 1;
                    if (StageName == "Preapproval")
                        Preapproval_StageCount += 1;
                    if (StageName == "In Progress")
                        InProgress_StageCount += 1;
                    if (StageName == "Complete")
                        Complete_StageCount += 1;
                    if (StageName == "STC Trade")
                        STCTrade_StageCount += 1;
                    if (StageName == "Aftersales")
                        Aftersales_StageCount += 1;
                    if (StageName == "Cancellations")
                        Cancellations_StageCount += 1;
                    if (StageName == "Installation Completed")
                        InstallationCompleted_StageCount += 1;

                    if (StageName == "New Installation" || StageName == "In Progress")
                    {
                        Outstanding_Job += 1;
                        Final_Outstanding_Job += Outstanding_Job;
                    }
                    else if (StageName == "New" || StageName == "Preapproval" || StageName == "Complete" || StageName == "STC Trade" || StageName == "Aftersales" || StageName == "Cancellations" || StageName == "Installation Completed")
                    {
                        Completed_Job += 1;
                        Final_Completed_Job += Completed_Job;
                    }

                    csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["RefNumber"]) + "," + Convert.ToString(dt.Rows[i]["Owner_Name"]) + ","
                               + VisitDate + "," + Convert.ToString(dt.Rows[i]["InvoiceSent"]) + "," + Convert.ToString(dt.Rows[i]["StageName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])));
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                if (i == dt.Rows.Count - 1)
                {

                    csv.Append(",,,Total Jobs(" + (Completed_Job + Outstanding_Job) + ")," + "Completed Jobs(" + Completed_Job + "),Outstanding Jobs(" + Outstanding_Job + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                    csv.Append("\r\n");
                    csv.Append(",,,Total Jobs(" + (Final_Completed_Job + Final_Outstanding_Job) + ")," + "Completed Jobs(" + Final_Completed_Job + "),Outstanding Jobs(" + Final_Outstanding_Job + ")");
                }
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=SSESCDetailReport_" + DateTime.Now.Ticks.ToString() + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// STCs the general report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult STCGeneralReport(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            try
            {
                if (ProjectSession.UserTypeId == 4)
                    formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);

                DataSet dsReport = _formBotReportService.GetSTCGeneralReport(formbotReportModel);
                obj.dsSTCGeneralGrid = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);

                if (dsReport.Tables[1] != null && dsReport.Tables[1].Rows.Count > 0)
                {
                    obj.dsSTCGeneralChart = new List<FormBotReport>();
                    DataColumnCollection columns = dsReport.Tables[1].Columns;
                    DataTable dt = dsReport.Tables[1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        FormBotReport lstobj = new FormBotReport();
                        lstobj.Name = Convert.ToString(dt.Rows[i]["NAME"]);
                        lstobj.NotYetSubmitted = columns.Contains("Not Yet Submitted") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Not Yet Submitted"])) ? Convert.ToInt64(dt.Rows[i]["Not Yet Submitted"]) : 0) : 0;
                        lstobj.ReSubmission = columns.Contains("Re-submission") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Re-submission"])) ? Convert.ToInt64(dt.Rows[i]["Re-submission"]) : 0) : 0;
                        lstobj.ReadyToTrade = columns.Contains("Ready to Trade") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Ready to Trade"])) ? Convert.ToInt64(dt.Rows[i]["Ready to Trade"]) : 0) : 0;
                        lstobj.AwaitingAuthorization = columns.Contains("Awaiting Authorization") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Awaiting Authorization"])) ? Convert.ToInt64(dt.Rows[i]["Awaiting Authorization"]) : 0) : 0;
                        lstobj.CERFailed = columns.Contains("CER Failed") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["CER Failed"])) ? Convert.ToInt64(dt.Rows[i]["CER Failed"]) : 0) : 0;
                        lstobj.CannotReCreate = columns.Contains("Cannot re-create") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Cannot re-create"])) ? Convert.ToInt64(dt.Rows[i]["Cannot re-create"]) : 0) : 0;
                        lstobj.UnderReview = columns.Contains("Under Review") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Under Review"])) ? Convert.ToInt64(dt.Rows[i]["Under Review"]) : 0) : 0;
                        lstobj.ComplianceIssues = columns.Contains("Compliance Issues") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Compliance Issues"])) ? Convert.ToInt64(dt.Rows[i]["Compliance Issues"]) : 0) : 0;
                        lstobj.RequiresCallBack = columns.Contains("Requires Call Back") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Requires Call Back"])) ? Convert.ToInt64(dt.Rows[i]["Requires Call Back"]) : 0) : 0;
                        lstobj.ReadyToCreate = columns.Contains("Ready To Create") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Ready To Create"])) ? Convert.ToInt64(dt.Rows[i]["Ready To Create"]) : 0) : 0;
                        lstobj.PendingApproval = columns.Contains("Pending Approval") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Pending Approval"])) ? Convert.ToInt64(dt.Rows[i]["Pending Approval"]) : 0) : 0;
                        lstobj.NewSubmission = columns.Contains("New Submission") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["New Submission"])) ? Convert.ToInt64(dt.Rows[i]["New Submission"]) : 0) : 0;
                        lstobj.CERApproved = columns.Contains("CER Approved") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["CER Approved"])) ? Convert.ToInt64(dt.Rows[i]["CER Approved"]) : 0) : 0;
                        obj.dsSTCGeneralChart.Add(lstobj);
                    }

                }
                //obj.dsSTCGeneralChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            }
            catch (Exception ex)
            {

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// STCs the general report for RAM.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>json result</returns>
        public JsonResult STCGeneralReportForRAM(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            try
            {
                if (ProjectSession.UserTypeId == 4)
                    formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);

                DataSet dsReport = _formBotReportService.GetSTCGeneralReportForRAM(formbotReportModel);
                obj.dsSTCGeneralGrid = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);

                if (dsReport.Tables[1] != null && dsReport.Tables[1].Rows.Count > 0)
                {
                    obj.dsSTCGeneralChart = new List<FormBotReport>();
                    DataColumnCollection columns = dsReport.Tables[1].Columns;
                    DataTable dt = dsReport.Tables[1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        FormBotReport lstobj = new FormBotReport();
                        lstobj.Name = Convert.ToString(dt.Rows[i]["NAME"]);
                        lstobj.NotYetSubmitted = columns.Contains("Not Yet Submitted") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Not Yet Submitted"])) ? Convert.ToInt64(dt.Rows[i]["Not Yet Submitted"]) : 0) : 0;
                        lstobj.ReSubmission = columns.Contains("Re-submission") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Re-submission"])) ? Convert.ToInt64(dt.Rows[i]["Re-submission"]) : 0) : 0;
                        lstobj.ReadyToTrade = columns.Contains("Ready to Trade") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Ready to Trade"])) ? Convert.ToInt64(dt.Rows[i]["Ready to Trade"]) : 0) : 0;
                        lstobj.AwaitingAuthorization = columns.Contains("Awaiting Authorization") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Awaiting Authorization"])) ? Convert.ToInt64(dt.Rows[i]["Awaiting Authorization"]) : 0) : 0;
                        lstobj.CERFailed = columns.Contains("CER Failed") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["CER Failed"])) ? Convert.ToInt64(dt.Rows[i]["CER Failed"]) : 0) : 0;
                        lstobj.CannotReCreate = columns.Contains("Cannot re-create") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Cannot re-create"])) ? Convert.ToInt64(dt.Rows[i]["Cannot re-create"]) : 0) : 0;
                        lstobj.UnderReview = columns.Contains("Under Review") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Under Review"])) ? Convert.ToInt64(dt.Rows[i]["Under Review"]) : 0) : 0;
                        lstobj.ComplianceIssues = columns.Contains("Compliance Issues") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Compliance Issues"])) ? Convert.ToInt64(dt.Rows[i]["Compliance Issues"]) : 0) : 0;
                        lstobj.RequiresCallBack = columns.Contains("Requires Call Back") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Requires Call Back"])) ? Convert.ToInt64(dt.Rows[i]["Requires Call Back"]) : 0) : 0;
                        lstobj.ReadyToCreate = columns.Contains("Ready To Create") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Ready To Create"])) ? Convert.ToInt64(dt.Rows[i]["Ready To Create"]) : 0) : 0;
                        lstobj.PendingApproval = columns.Contains("Pending Approval") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Pending Approval"])) ? Convert.ToInt64(dt.Rows[i]["Pending Approval"]) : 0) : 0;
                        lstobj.NewSubmission = columns.Contains("New Submission") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["New Submission"])) ? Convert.ToInt64(dt.Rows[i]["New Submission"]) : 0) : 0;
                        lstobj.CERApproved = columns.Contains("CER Approved") ? (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["CER Approved"])) ? Convert.ToInt64(dt.Rows[i]["CER Approved"]) : 0) : 0;
                        obj.dsSTCGeneralChart.Add(lstobj);
                    }

                }

                //obj.dsSTCGeneralChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);
            }
            catch (Exception ex)
            {

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate CSV For STC General Grouping report
        /// </summary>
        /// <param name="hdnFormBotResellerAssignedUser">hdnFormBot ResellerAssignedUser</param>
        /// <param name="hdnFormBotSolarCompanyAssignedUser">hdnFormBot SolarCompanyAssignedUser</param>
        /// <param name="hdnFormBotRAMAssignedUser">hdnFormBot RAMAssignedUser</param>
        /// <param name="StartDate">Start Date</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="hdnSTCSubmissionStatusAssigned">hdnSTCSubmission StatusAssigned</param>
        /// <param name="IsAllSelected">IsAll Selected</param>
        public void Generate_STCGeneralReport_CSV(string hdnFormBotResellerAssignedUser, string hdnFormBotSolarCompanyAssignedUser, string hdnFormBotRAMAssignedUser, string StartDate, string EndDate, string hdnSTCSubmissionStatusAssigned, bool IsAllSelected = false)
        {
            FormBotReport formbotReportModel = new FormBotReport();
            formbotReportModel.hdnFormBotResellerAssignedUser = hdnFormBotResellerAssignedUser;
            formbotReportModel.hdnFormBotSolarCompanyAssignedUser = hdnFormBotSolarCompanyAssignedUser;
            formbotReportModel.hdnFormBotRAMAssignedUser = hdnFormBotRAMAssignedUser;
            if (!string.IsNullOrEmpty(StartDate))
                formbotReportModel.StartDate = Convert.ToDateTime(StartDate);
            if (!string.IsNullOrEmpty(EndDate))
                formbotReportModel.EndDate = Convert.ToDateTime(EndDate);
            formbotReportModel.hdnSTCSubmissionStatusAssigned = hdnSTCSubmissionStatusAssigned;
            formbotReportModel.IsAllSelected = IsAllSelected;

            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 4)
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);

            DataSet dsReport = _formBotReportService.GetSTCGeneralReport(formbotReportModel);
            obj.dsSTCGeneralGrid = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSTCGeneralChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);

            DataTable dt = dsReport.Tables[0];

            StringBuilder csv = new StringBuilder();
            csv.Append(@"Reseller Name,Company Name,Reference Number,Client Name,Submission Date,Status,No Of STC,STC Price,Total");
            csv.Append("\r\n");

            string resellerName = string.Empty;
            string SolarCompanyName = string.Empty;
            string StageName = string.Empty;
            decimal TotalNoOfSTC = 0;
            decimal TotalPrice = 0;
            string VisitDate = string.Empty;
            string submissionDate = string.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {



                if (SolarCompanyName == Convert.ToString(dt.Rows[i]["CompanyName"]) || string.IsNullOrEmpty(SolarCompanyName))
                {
                    SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);
                    TotalNoOfSTC += Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]);

                    resellerName = Convert.ToString(dt.Rows[i]["ResellerName"]);
                    submissionDate = Convert.ToString(dt.Rows[i]["STCSubmissionDate"]);

                    TotalPrice += Convert.ToDecimal(dt.Rows[i]["Total"]);

                    csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["ResellerName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + ","
                                 + Convert.ToString(dt.Rows[i]["ClientName"]) + "," + Convert.ToString(dt.Rows[i]["STCSubmissionDate"]) + "," + Convert.ToString(dt.Rows[i]["Status"]) + "," + Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]) + ","
                                 + Convert.ToDecimal(dt.Rows[i]["STCPrice"]) + "," + Convert.ToDecimal(dt.Rows[i]["Total"]));
                    csv.Append("\r\n");
                }
                else
                {

                    csv.Append(",,,,,,Total No of STC(" + TotalNoOfSTC + "),," + " Total(" + TotalPrice + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                    csv.Append("\r\n");

                    TotalNoOfSTC = 0;
                    TotalPrice = 0;
                    SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);

                    csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["ResellerName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + ","
                                 + Convert.ToString(dt.Rows[i]["ClientName"]) + "," + Convert.ToString(dt.Rows[i]["STCSubmissionDate"]) + "," + Convert.ToString(dt.Rows[i]["Status"]) + "," + Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]) + ","
                                 + Convert.ToDecimal(dt.Rows[i]["STCPrice"]) + "," + Convert.ToDecimal(dt.Rows[i]["Total"]));
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                if (i == dt.Rows.Count - 1)
                {
                    csv.Append(",,,,,,Total No of STC(" + TotalNoOfSTC + "),," + " Total(" + TotalPrice + ")");
                    //csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=STCGeneralReports" + DateTime.Now.Ticks.ToString() + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Get SCA Trade report data 
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public JsonResult GetSCATradedChartData(string StartDate,string EndDate,int Type)
        {
            List<SCATradeReport> lstTardedCountTypeWise = _formBotReportService.GetSCATradedChartData(StartDate, EndDate,Type);
            return Json(lstTardedCountTypeWise, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get SCA Status report data 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public JsonResult GetSCAStatusChartData(int Type)
        {
            List<SCAStatusReport> lstStatusCountTypeWise = _formBotReportService.GetSCAStatusChartData(Type);
            return Json(lstStatusCountTypeWise, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate CSV For STC General Grouping report
        /// </summary>
        /// <param name="hdnFormBotResellerAssignedUser">hdnFormBotResellerAssignedUser</param>
        /// <param name="hdnFormBotSolarCompanyAssignedUser">hdnFormBotSolarCompanyAssignedUser</param>
        /// <param name="hdnFormBotRAMAssignedUser">hdnFormBotRAMAssignedUser</param>
        /// <param name="StartDate">StartDate</param>
        /// <param name="EndDate">EndDate</param>
        /// <param name="hdnSTCSubmissionStatusAssigned">hdnSTCSubmissionStatusAssigned</param>
        /// <param name="IsDetail">IsDetail</param>
        public void Generate_STCGeneralReportRAM_CSV(string hdnFormBotResellerAssignedUser, string hdnFormBotSolarCompanyAssignedUser, string hdnFormBotRAMAssignedUser, string StartDate, string EndDate, string hdnSTCSubmissionStatusAssigned, int IsDetail = 0)
        {
            FormBotReport formbotReportModel = new FormBotReport();
            formbotReportModel.hdnFormBotResellerAssignedUser = hdnFormBotResellerAssignedUser;
            formbotReportModel.hdnFormBotSolarCompanyAssignedUser = hdnFormBotSolarCompanyAssignedUser;
            formbotReportModel.hdnFormBotRAMAssignedUser = hdnFormBotRAMAssignedUser;
            if (!string.IsNullOrEmpty(StartDate))
                formbotReportModel.StartDate = Convert.ToDateTime(StartDate);
            if (!string.IsNullOrEmpty(EndDate))
                formbotReportModel.EndDate = Convert.ToDateTime(EndDate);
            formbotReportModel.hdnSTCSubmissionStatusAssigned = hdnSTCSubmissionStatusAssigned;
            formbotReportModel.IsDetail = IsDetail;

            AllocationReport obj = new AllocationReport();
            if (ProjectSession.UserTypeId == 4)
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);

            DataSet dsReport = _formBotReportService.GetSTCGeneralReportForRAM(formbotReportModel);
            obj.dsSTCGeneralGrid = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            obj.dsSTCGeneralChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);

            DataTable dt = dsReport.Tables[0];



            string resellerName = string.Empty;
            string SolarCompanyName = string.Empty;
            string StageName = string.Empty;
            decimal TotalNoOfSTC = 0;
            decimal TotalPrice = 0;
            string VisitDate = string.Empty;
            string submissionDate = string.Empty;
            StringBuilder csv = new StringBuilder();

            if (IsDetail == 0)
            {
                csv.Append(@"Reseller,Account Manager,Company Name,Status,STCs,Average Price,Amount,%");
                csv.Append("\r\n");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TotalNoOfSTC += Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]);
                    TotalPrice += Convert.ToDecimal(dt.Rows[i]["Total"]);

                    csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["ResellerName"]) + "," + Convert.ToString(dt.Rows[i]["RAMName"])) + "," + Convert.ToString(dt.Rows[i]["CompanyName"]) + ","
                        + Convert.ToString(dt.Rows[i]["Status"]) + "," + Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]) + ","
                                 + Convert.ToDecimal(dt.Rows[i]["STCPrice"]) + "," + Convert.ToDecimal(dt.Rows[i]["Total"]) + "," + Convert.ToDecimal(dt.Rows[i]["percentageAmount"]));
                    csv.Append("\r\n");

                    if (i == dt.Rows.Count - 1)
                    {
                        csv.Append(",,,,,,Total No of STC(" + TotalNoOfSTC + "),," + " Total(" + TotalPrice + ")");
                        csv.Append("\r\n");
                    }
                }
            }
            else
            {
                csv.Append(@"Reseller,Account Manager,Company Name,Reference Number,Client Name,Submission Date,Status,No Of STC,STC Price,Total");
                csv.Append("\r\n");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (SolarCompanyName == Convert.ToString(dt.Rows[i]["CompanyName"]) || string.IsNullOrEmpty(SolarCompanyName))
                    {
                        SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);
                        TotalNoOfSTC += Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]);
                        resellerName = Convert.ToString(dt.Rows[i]["ResellerName"]);
                        submissionDate = Convert.ToString(dt.Rows[i]["STCSubmissionDate"]);

                        TotalPrice += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["ResellerName"]) + "," + Convert.ToString(dt.Rows[i]["RAMName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + ","
                                     + Convert.ToString(dt.Rows[i]["ClientName"]) + "," + Convert.ToString(dt.Rows[i]["STCSubmissionDate"]) + "," + Convert.ToString(dt.Rows[i]["Status"]) + "," + Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]) + ","
                                     + Convert.ToDecimal(dt.Rows[i]["STCPrice"]) + "," + Convert.ToDecimal(dt.Rows[i]["Total"]));
                        csv.Append("\r\n");
                    }
                    else
                    {

                        csv.Append(",,,,,,Total No of STC(" + TotalNoOfSTC + "),," + " Total(" + TotalPrice + ")");
                        csv.Append("\r\n");
                        csv.Append("\r\n");

                        TotalNoOfSTC = 0;
                        TotalPrice = 0;
                        SolarCompanyName = Convert.ToString(dt.Rows[i]["CompanyName"]);

                        csv.Append(Convert.ToString(Convert.ToString(dt.Rows[i]["ResellerName"]) + "," + Convert.ToString(dt.Rows[i]["RAMName"]) + "," + Convert.ToString(dt.Rows[i]["CompanyName"])) + "," + Convert.ToString(dt.Rows[i]["RefNumber"]) + ","
                                     + Convert.ToString(dt.Rows[i]["ClientName"]) + "," + Convert.ToString(dt.Rows[i]["STCSubmissionDate"]) + "," + Convert.ToString(dt.Rows[i]["Status"]) + "," + Convert.ToDecimal(dt.Rows[i]["NoOfSTC"]) + ","
                                     + Convert.ToDecimal(dt.Rows[i]["STCPrice"]) + "," + Convert.ToDecimal(dt.Rows[i]["Total"]));
                        csv.Append("\r\n");
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        csv.Append(",,,,,,Total No of STC(" + TotalNoOfSTC + "),," + " Total(" + TotalPrice + ")");
                        csv.Append("\r\n");
                    }
                }
            }



            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=STCGeneralReports" + DateTime.Now.Ticks.ToString() + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }


        public JsonResult GetNonTradeJobReport(FormBotReport formbotReportModel)
        {
            NonTradeJobReport nonTradeJobReport = new NonTradeJobReport();
            formbotReportModel.LogginUserID = ProjectSession.LoggedInUserId;
            formbotReportModel.LoginUserType = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            DataSet dsReport = _formBotReportService.GetNonTradeJobReport(formbotReportModel);

            nonTradeJobReport.lstRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            //nonTradeJobReport.lstChartRecord = DBClient.ToListof<FormBotReport>(dsReport.Tables[1]);

            return Json(nonTradeJobReport, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}
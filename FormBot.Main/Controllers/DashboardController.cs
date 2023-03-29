using FormBot.BAL.Service.Dashboard;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL;
using FormBot.Entity;
using FormBot.BAL.Service.Job;
using FormBot.BAL.Service;
using FormBot.Entity.Dashboard;
using Newtonsoft.Json;
using FormBot.Helper.Helper;
using FormBot.Entity.SolarElectrician;

namespace FormBot.Main.Controllers
{
    public class DashboardController : Controller
    {
        #region Properties
        private readonly IDashboardBAL _dashboard;
        private readonly IJobSchedulingBAL _jobScheduling;
        private readonly ICreateJobBAL _jobBAL;
        private readonly ICreateJobHistoryBAL _jobHistory;
        private readonly IResellerBAL _resellerService;
        private readonly IFormBotReportBAL _formBotReportService;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly ILogger _log;
        #endregion

        #region Constructor

        public DashboardController(IDashboardBAL dashboard, IJobSchedulingBAL jobScheduling, ICreateJobBAL jobBAL, ICreateJobHistoryBAL jobHistory, IResellerBAL resellerService, IFormBotReportBAL formBotReportService, IUserBAL userBAL, IEmailBAL emailBAL, ILogger log)
        {
            this._dashboard = dashboard;
            this._jobScheduling = jobScheduling;
            this._jobBAL = jobBAL;
            this._jobHistory = jobHistory;
            this._resellerService = resellerService;
            this._formBotReportService = formBotReportService;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
            this._log = log;
        }

        #endregion

        #region Events

        [HttpGet]
        [UserAuthorization]
        public ActionResult Index()
        {
            if ((ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8) && ProjectSession.IsSubContractor)
            {
                DataSet ds = _dashboard.GetSSCDashboardDetails();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.NewInstallationCount = Convert.ToString(ds.Tables[0].Rows[0]["NewInstallationCount"]);
                    ViewBag.InProgressCount = Convert.ToString(ds.Tables[0].Rows[0]["InProgressCount"]);
                    ViewBag.InstallationCompletedCount = Convert.ToString(ds.Tables[0].Rows[0]["InstallationCompletedCount"]);
                    ViewBag.CancellationsCount = Convert.ToString(ds.Tables[0].Rows[0]["CancellationsCount"]);
                    ViewBag.CompleteCount = Convert.ToString(ds.Tables[0].Rows[0]["CompleteCount"]);
                    ViewBag.NotYetInvoiced = Convert.ToString(ds.Tables[0].Rows[0]["NotYetInvoiced"]);
                    ViewBag.SERequestPending = Convert.ToString(ds.Tables[0].Rows[0]["SERequestPending"]);
                }
                return View("SSC");
            }
            else if ((ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8) && !ProjectSession.IsSubContractor)
            {
                DataSet ds = _dashboard.GetSCADashboardDetails();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.FullName = Convert.ToString(ds.Tables[0].Rows[0]["FullName"]);
                    ViewBag.Email = Convert.ToString(ds.Tables[0].Rows[0]["Email"]);
                    ViewBag.Phone = Convert.ToString(ds.Tables[0].Rows[0]["Phone"]);
                    ViewBag.IsWholeSaler = Convert.ToString(ds.Tables[0].Rows[0]["IsWholeSaler"]);
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NotYetApplied = Convert.ToString(ds.Tables[1].Rows[0]["NotYetAppliedCount"]);
                    ViewBag.PreApprovalIssues = Convert.ToString(ds.Tables[1].Rows[0]["PreApprovalIssuesCount"]);
                    ViewBag.PreApprovalSent = Convert.ToString(ds.Tables[1].Rows[0]["PreApprovalSentCount"]);
                    ViewBag.Incomplete = Convert.ToString(ds.Tables[1].Rows[0]["IncompleteCount"]);
                    ViewBag.ConnectionIssues = Convert.ToString(ds.Tables[1].Rows[0]["ConnectionIssuesCount"]);
                    ViewBag.ConnectionSent = Convert.ToString(ds.Tables[1].Rows[0]["ConnectionSentCount"]);
                    ViewBag.UnscheduledJobs = Convert.ToString(ds.Tables[1].Rows[0]["UnscheduledJobsCount"]);
                    ViewBag.ComplianceIssues = Convert.ToString(ds.Tables[1].Rows[0]["ComplianceIssuesCount"]);
                    ViewBag.RECFailure = Convert.ToString(ds.Tables[1].Rows[0]["RECFailureCount"]);
                    ViewBag.NotYetInvoiced = Convert.ToString(ds.Tables[1].Rows[0]["NotYetInvoiced"]);
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    ViewBag.isDashboardWS_SCASettlementTermView = Convert.ToString(Convert.ToBoolean(ds.Tables[2].Rows[0][0]));
                }
                if(ds != null && ds.Tables.Count > 0 && ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                {
                    ViewBag.CompanyName = Convert.ToString(ds.Tables[3].Rows[0]["CompanyName"]);
                    ViewBag.ProfilePhoto = Convert.ToString(ds.Tables[3].Rows[0]["ProfilePhoto"]);
                    ViewBag.SCAName = Convert.ToString(ds.Tables[3].Rows[0]["SCAName"]);
                }                
                ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                InstallerDesignerView installerDesigner = new InstallerDesignerView();

                return View("SCA",installerDesigner);
            }
            else if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
            {
                JobScheduling jobScheduling = new JobScheduling();
                int solarCompanyId = ProjectSession.SolarCompanyId;
                int userTypeId = ProjectSession.UserTypeId;
                int userId = ProjectSession.LoggedInUserId;

                if ((userTypeId != 2 && userTypeId != 5 && userTypeId != 1 && userTypeId != 3))
                {
                    JobSchedulingController jobSchedulingController = new JobSchedulingController(_jobScheduling, _jobBAL, _jobHistory, _userBAL, _emailBAL, _log);
                    jobScheduling = jobSchedulingController.SetCalendarData(jobScheduling, userId, userTypeId, solarCompanyId);
                }
                else
                {
                    jobScheduling.solarElectrician = new List<SolarElectricianView>();
                    jobScheduling.job = new List<JobDetails>();
                    jobScheduling.jobSchedulingData = string.Empty;
                }
                jobScheduling.IsDashboard = true;

                DataSet dsSESC = _dashboard.GetSESCDashboard();
                if (dsSESC != null && dsSESC.Tables.Count > 0 && dsSESC.Tables[0] != null && dsSESC.Tables[0].Rows.Count > 0)
                {
                    ViewBag.NewInstallation = Convert.ToString(dsSESC.Tables[0].Rows[0]["NewInstallationCount"]);
                    ViewBag.InProgress = Convert.ToString(dsSESC.Tables[0].Rows[0]["InProgressCount"]);
                    ViewBag.InstallationCompleted = Convert.ToString(dsSESC.Tables[0].Rows[0]["InstallationCompletedCount"]);
                    ViewBag.NotYetInvoiced = Convert.ToString(dsSESC.Tables[0].Rows[0]["NotYetInvoiced"]);
                    ViewBag.Cancelled = Convert.ToString(dsSESC.Tables[0].Rows[0]["CancelledCount"]);
                }

                return View("SeSc", jobScheduling);
            }
            else if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {

                DataSet ds = _dashboard.GetFSADashboardDetails();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.SSCRemovalRequest = Convert.ToString(ds.Tables[0].Rows[0]["SSCRemovalRequest"]);
                    ViewBag.PendingSolarElectrician = Convert.ToString(ds.Tables[0].Rows[0]["PendingSolarElectrician"]);
                }

                DataSet databaselinkDs = _dashboard.GetDatabaseLinkUpdate();
                if (databaselinkDs != null && databaselinkDs.Tables.Count > 0 && databaselinkDs.Tables[0] != null && databaselinkDs.Tables[0].Rows.Count > 0)
                {
                    ViewBag.PVModules = Convert.ToString(databaselinkDs.Tables[0].Rows[0]["PVModules"]);
                    ViewBag.Inverters = Convert.ToString(databaselinkDs.Tables[1].Rows[0]["Inverters"]);
                    ViewBag.Installers = Convert.ToString(databaselinkDs.Tables[2].Rows[0]["Installers"]);
                    ViewBag.SWH = Convert.ToString(databaselinkDs.Tables[3].Rows[0]["SWH"]);
                }
                else
                {
                    ViewBag.PVModules = string.Empty;
                    ViewBag.Inverters = string.Empty;
                    ViewBag.Installers = string.Empty;
                    ViewBag.SWH = string.Empty;
                }
                ViewBag.FCO = _dashboard.GetFCOByReseller();
                //Dashboard dashboard = new Dashboard();
                return View("FSA");
            }
            else if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
            {
                DataSet ds = _dashboard.GetRADashboardDetails();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.PendingSolarCompany = Convert.ToString(ds.Tables[0].Rows[0]["PendingSolarCompany"]);
                }

                DataSet dsPaymentCount = _dashboard.GetPaymentForRARAM((ProjectSession.UserTypeId == 2 ? ProjectSession.ResellerId : 0), (ProjectSession.UserTypeId == 5 ? ProjectSession.LoggedInUserId : 0));
                if (dsPaymentCount != null && dsPaymentCount.Tables.Count > 0 && dsPaymentCount.Tables[0] != null && dsPaymentCount.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalPaid = Convert.ToString(dsPaymentCount.Tables[0].Rows[0]["TotalPaid"]);
                    ViewBag.PayOnApproval = Convert.ToString(dsPaymentCount.Tables[0].Rows[0]["PayOnApproval"]);
                    ViewBag.PartialPayment = Convert.ToString(dsPaymentCount.Tables[0].Rows[0]["PartialPayment"]);
                    ViewBag.Outstanding = Convert.ToString(dsPaymentCount.Tables[0].Rows[0]["Outstanding"]);
                }
                else
                {
                    ViewBag.TotalPaid = 0;
                    ViewBag.PayOnApproval = 0;
                    ViewBag.PartialPayment = 0;
                    ViewBag.Outstanding = 0;
                }

                ViewBag.RAM = _dashboard.GetAllRAMByRA(ProjectSession.ResellerId);

                return View("RA");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Get SCA Dashboard Details
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SCA()
        {
            DataSet ds = new DataSet();
            if (ProjectSession.UserTypeId == 8 && ProjectSession.IsSubContractor)
            {
                ds = _dashboard.GetSSCDashboardDetails();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.NewInstallationCount = Convert.ToString(ds.Tables[0].Rows[0]["NewInstallationCount"]);
                    ViewBag.InProgressCount = Convert.ToString(ds.Tables[0].Rows[0]["InProgressCount"]);
                    ViewBag.InstallationCompletedCount = Convert.ToString(ds.Tables[0].Rows[0]["InstallationCompletedCount"]);
                    ViewBag.CancellationsCount = Convert.ToString(ds.Tables[0].Rows[0]["CancellationsCount"]);
                    ViewBag.CompleteCount = Convert.ToString(ds.Tables[0].Rows[0]["CompleteCount"]);
                    ViewBag.NotYetInvoiced = Convert.ToString(ds.Tables[0].Rows[0]["NotYetInvoiced"]);
                    ViewBag.SERequestPending = Convert.ToString(ds.Tables[0].Rows[0]["SERequestPending"]);
                }
                return View("SSC");
            }
            ds = _dashboard.GetSCADashboardDetails();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ViewBag.FullName = Convert.ToString(ds.Tables[0].Rows[0]["FullName"]);
                ViewBag.Email = Convert.ToString(ds.Tables[0].Rows[0]["Email"]);
                ViewBag.Phone = Convert.ToString(ds.Tables[0].Rows[0]["Phone"]);
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                ViewBag.NotYetApplied = Convert.ToString(ds.Tables[1].Rows[0]["NotYetAppliedCount"]);
                ViewBag.PreApprovalIssues = Convert.ToString(ds.Tables[1].Rows[0]["PreApprovalIssuesCount"]);
                ViewBag.PreApprovalSent = Convert.ToString(ds.Tables[1].Rows[0]["PreApprovalSentCount"]);
                ViewBag.Incomplete = Convert.ToString(ds.Tables[1].Rows[0]["IncompleteCount"]);
                ViewBag.ConnectionIssues = Convert.ToString(ds.Tables[1].Rows[0]["ConnectionIssuesCount"]);
                ViewBag.ConnectionSent = Convert.ToString(ds.Tables[1].Rows[0]["ConnectionSentCount"]);
                ViewBag.UnscheduledJobs = Convert.ToString(ds.Tables[1].Rows[0]["UnscheduledJobsCount"]);
                ViewBag.ComplianceIssues = Convert.ToString(ds.Tables[1].Rows[0]["ComplianceIssuesCount"]);
                ViewBag.RECFailure = Convert.ToString(ds.Tables[1].Rows[0]["RECFailureCount"]);
                ViewBag.NotYetInvoiced = Convert.ToString(ds.Tables[1].Rows[0]["NotYetInvoiced"]);
            }
            return View();
        }

        /// <summary>
        /// Get Se/Sc Dashboard Details
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        [ValidateInput(false)]
        [UserAuthorization]
        public ActionResult SeSc()
        {
            JobScheduling jobScheduling = new JobScheduling();
            int solarCompanyId = ProjectSession.SolarCompanyId;
            int userTypeId = ProjectSession.UserTypeId;
            int userId = ProjectSession.LoggedInUserId;

            if ((userTypeId != 2 && userTypeId != 5 && userTypeId != 1 && userTypeId != 3))
            {
                JobSchedulingController jobSchedulingController = new JobSchedulingController(_jobScheduling, _jobBAL, _jobHistory, _userBAL, _emailBAL, _log);
                jobScheduling = jobSchedulingController.SetCalendarData(jobScheduling, userId, userTypeId, solarCompanyId);
            }
            else
            {
                jobScheduling.solarElectrician = new List<SolarElectricianView>();
                jobScheduling.job = new List<JobDetails>();
                jobScheduling.jobSchedulingData = string.Empty;
            }
            jobScheduling.IsDashboard = true;

            DataSet dsSESC = _dashboard.GetSESCDashboard();
            if (dsSESC != null && dsSESC.Tables.Count > 0 && dsSESC.Tables[0] != null && dsSESC.Tables[0].Rows.Count > 0)
            {
                ViewBag.NewInstallation = Convert.ToString(dsSESC.Tables[0].Rows[0]["NewInstallationCount"]);
                ViewBag.InProgress = Convert.ToString(dsSESC.Tables[0].Rows[0]["InProgressCount"]);
                ViewBag.InstallationCompleted = Convert.ToString(dsSESC.Tables[0].Rows[0]["InstallationCompletedCount"]);
                ViewBag.NotYetInvoiced = Convert.ToString(dsSESC.Tables[0].Rows[0]["NotYetInvoiced"]);
                ViewBag.Cancelled = Convert.ToString(dsSESC.Tables[0].Rows[0]["CancelledCount"]);
            }

            return View(jobScheduling);
        }

        /// <summary>
        /// Get SCA Dashboard News Feed
        /// </summary>
        /// <param name="categoryID">Category ID</param>
        /// <param name="from">From Date</param>
        /// <param name="to">To Date</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GetNewsFeed(int categoryID, DateTime? from, DateTime? to, string pageIndex, string pageSize)
        {
            if (pageIndex == "1")
                ViewBag.AllTagsShow = 1;
            else if (string.IsNullOrEmpty(pageIndex))
                ViewBag.AllTagsShow = 1;
            else
                ViewBag.AllTagsShow = 0;
            return this.PartialView("~/Views/JobHistory/_HistoryList.cshtml", GetHistory(categoryID, from, to, pageIndex, pageSize));
        }

        /// <summary>
        /// Get History for SCA Dashboard News Feed
        /// </summary>
        /// <param name="categoryID">Category ID</param>
        /// <param name="from">From Date</param>
        /// <param name="to">To Date</param>
        /// <returns>List of Job history</returns>
        public List<JobHistory> GetHistory(int? categoryID, DateTime? from, DateTime? to, string pageIndex, string pageSize)
        {
            //GridParam gridParam = Grid.ParseParams(HttpContext.Request);

            List<JobHistory> jobHistory = new List<JobHistory>();
            DataSet objDs = _dashboard.GetNewsFeedList(categoryID, from, to, !string.IsNullOrEmpty(pageIndex) ? Convert.ToInt32(pageIndex) : 0, !string.IsNullOrEmpty(pageSize) ? Convert.ToInt32(pageSize) : 0);
            if (objDs != null && objDs.Tables.Count > 0)
            {
                jobHistory = objDs.Tables[0].AsEnumerable().Select(
                    p => new JobHistory
                    {
                        CategoryID = (p.Field<int>("CategoryID")),
                        HistoryCategory = (p.Field<string>("HistoryCategory")),
                        HistoryMessage = (p.Field<string>("HistoryMessage")),
                        IsSSC = (p.Field<bool>("IsSSC")),
                        JobHistoryID = (p.Field<int>("JobHistoryID")),
                        JobID = (p.Field<int>("JobID")),
                        ModifiedBy = (p.Field<int>("ModifiedBy")),
                        CreateDate = (p.Field<string>("CreateDate")),
                        ModifiedDate = (p.Field<DateTime>("ModifiedDate")),
                        Modifier = (p.Field<string>("Modifier")),
                        Title = (p.Field<string>("Title")),
                        RefNumber = (p.Field<string>("RefNumber"))
                    }).ToList();
            }
            return jobHistory;
        }

        /// <summary>
        /// Get SCA Dashboard Job STC Price for STC Ready for Treade Section
        /// </summary>
        /// <param name="IsShowInDashboard">Is Show In Dashboard</param>
        /// <param name="IsGridView">Is GridView</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GetDashboardJobSTCPrice(bool IsShowInDashboard, bool IsGridView = false)
        {
            FormBot.Entity.PricingManager result = _dashboard.GetDashboardJobSTCPrice();
            if (result.CustomSettlementTerm > 0)
            {
                result.CustomTermText = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
                result.CustomSubDescription = Common.GetSubDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
            }
            result.IsGridView = IsGridView;
            result.IsShowInDashboard = IsShowInDashboard;
            result.IsDashboardPricing = true;
            //Here Only That Jobs will come whose status is submit to trade
            ViewBag.CurrentSTCJobStatus = SystemEnums.STCJobStatus.SubmittoTrade.GetHashCode();
            result.STCStatus = SystemEnums.STCJobStatus.SubmittoTrade.GetHashCode();
            return this.PartialView("~/Views/Job/_SettlementBlock.cshtml", result);
        }
        /// <summary>
        /// Get RA/RAM Dashboard Job STC Price for STC Ready for Treade Section
        /// </summary>
        /// <param name="IsShowInDashboard">Is Show In Dashboard</param>
        /// <param name="IsGridView">Is GridView</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetDashboardJobSTCPriceForRA(bool IsShowInDashboard, bool IsGridView = false)
        {
            PricingManager result = _dashboard.GetDashboardJobSTCPriceForRA();
            if (result.CustomSettlementTerm > 0)
            {
                result.CustomTermText = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
                result.CustomSubDescription = Common.GetSubDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
            }
            result.IsShowInDashboard = IsShowInDashboard;
            result.IsGridView = IsGridView;
            result.IsDashboardPricing = true;
            ViewBag.CurrentSTCJobStatus = SystemEnums.STCJobStatus.SubmittoTrade.GetHashCode();
            result.STCStatus = SystemEnums.STCJobStatus.SubmittoTrade.GetHashCode();
            result.IsForDashboardPricingWholesaler = true;
            return this.PartialView("~/Views/Job/_SettlementBlock.cshtml", result);

        }

        /// <summary>
        /// Redirect to Job from Dashboard
        /// </summary>
        /// <param name="status">string status</param>
        /// <returns>Action Result</returns>
        public ActionResult RedirectToJob(string status)
        {
            if (status == "NotYetApplied")
            {
                TempData["PreApprovalsDashboardStatus"] = 1;
            }
            else if (status == "PreApprovalIssues")
            {
                TempData["PreApprovalsDashboardStatus"] = 4;
            }
            else if (status == "PreApprovalSent")
            {
                TempData["PreApprovalsDashboardStatus"] = 3;
            }
            else if (status == "Incomplete")
            {
                TempData["ConnectionsDashboardStatus"] = 6;
            }
            else if (status == "ConnectionIssues")
            {
                TempData["ConnectionsDashboardStatus"] = 8;
            }
            else if (status == "ConnectionSent")
            {
                TempData["ConnectionsDashboardStatus"] = 7;
            }
            else if (status == "NotYetInvoiced")
            {
                TempData["NotYetInvoicedDashboardStatus"] = "NotYetInvoiced";
            }
            else if (status == "NewInstallation")
            {
                TempData["JobStage"] = 3;
            }
            else if (status == "InProgress")
            {
                TempData["JobStage"] = 4;
            }
            else if (status == "InstallationCompleted")
            {
                TempData["JobStage"] = 9;
            }
            else if (status == "Cancellations")
            {
                TempData["JobStage"] = 8;
            }
            else if (status == "Complete")
            {
                TempData["JobStage"] = 5;
            }

            return RedirectToAction("Index", "Job");
        }

        /// <summary>
        /// Redirect to STC Submission
        /// </summary>
        /// <param name="status">String Status</param>
        /// <returns>Action Result</returns>
        public ActionResult RedirectToSTCSubmission(string status)
        {
            if (status == "ComplianceIssues")
            {
                TempData["ComplianceIssuesDashboardStatus"] = 17;
            }
            else if (status == "RECFailure")
            {
                TempData["ComplianceIssuesDashboardStatus"] = 14;
            }
            return RedirectToAction("STCSubmission", "Job");
        }

        /// <summary>
        /// Redirect to Job from Dashboard
        /// </summary>
        /// <param name="SearchId">SearchId</param>
        /// <param name="status">status</param>
        /// <returns>ActionResult</returns>
        public ActionResult RedirectToJobSTCSubmission(string SearchId = "", string status = "")
        {
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                TempData["STCSubmission_ResellerId"] = SearchId;
                TempData["STCSubmission_SolarCompanyId"] = null;
            }
            else if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
            {
                TempData["STCSubmission_ResellerId"] = null;
                TempData["STCSubmission_SolarCompanyId"] = SearchId;
            }

            TempData["STCSubmission_Status"] = status;
            return RedirectToAction("STCSubmission", "Job");
        }

        /// <summary>
        /// Redirect to Solar Electrician
        /// </summary>
        /// <param name="status">String Status</param>
        /// <returns>Action Result</returns>
        public ActionResult RedirectToSolarElectrician(string status)
        {
            if (status == "SERequestPending")
            {
                TempData["SERequestPending"] = "Request_Send";
            }
            return RedirectToAction("SE", "User");
        }

        /// <summary>
        /// Display current job scheduling
        /// </summary>
        /// <returns>Partial View</returns>
        public PartialViewResult _Currentjobs()
        {
            return PartialView("~/Views/Dashboard/_Currentjobs.cshtml");
        }

        /// <summary>
        /// Display complete job scheduling
        /// </summary>
        /// <returns>Partial View</returns>
        public PartialViewResult _Completedjobs()
        {

            return PartialView("_Completedjobs");
        }

        /// <summary>
        /// Get list of job scheduling
        /// </summary>
        /// <param name="isCurrent">Is current</param>
        public void GetListForCurrentJobs(int isCurrent)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            DataSet dsJobs = _dashboard.GetSESCDashboardDetails(ProjectSession.LoggedInUserId, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir);
            List<FormBot.Entity.Dashboard.Dashboard> lstJobs = new List<Entity.Dashboard.Dashboard>();
            if (isCurrent == 1)
                lstJobs = DBClient.ToListof<FormBot.Entity.Dashboard.Dashboard>(dsJobs.Tables[0]);
            else
                lstJobs = DBClient.ToListof<FormBot.Entity.Dashboard.Dashboard>(dsJobs.Tables[1]);

            if (lstJobs.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstJobs.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstJobs.FirstOrDefault().TotalRecords;
            }
            HttpContext.Response.Write(Grid.PrepareDataSet(lstJobs, gridParam));
        }

        /// <summary>
        /// Get New Job Request List for SSC
        /// </summary>
        public string GetNewJobRequestList(int pageIndex = 1, int pageSize = 10)
        {
            DataSet dsJobs = _dashboard.GetNewJobRequestList(ProjectSession.LoggedInUserId, "", "", pageIndex, pageSize);
            List<FormBot.Entity.Dashboard.DashboardJobList> lstJobs = new List<Entity.Dashboard.DashboardJobList>();
            lstJobs = DBClient.ToListof<FormBot.Entity.Dashboard.DashboardJobList>(dsJobs.Tables[0]);
            if (lstJobs.Any())
            {
                return JsonConvert.SerializeObject(lstJobs);
            }
            else
                return "";
        }
        //public void GetNewJobRequestList()
        //{
        //	GridParam gridParam = Grid.ParseParams(HttpContext.Request);
        //	int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
        //	DataSet dsJobs = _dashboard.GetNewJobRequestList(ProjectSession.LoggedInUserId, gridParam.SortCol, gridParam.SortDir);
        //	List<FormBot.Entity.Dashboard.DashboardJobList> lstJobs = new List<Entity.Dashboard.DashboardJobList>();
        //	lstJobs = DBClient.ToListof<FormBot.Entity.Dashboard.DashboardJobList>(dsJobs.Tables[0]);
        //	HttpContext.Response.Write(Grid.PrepareDataSet(lstJobs, gridParam));
        //}

        /// <summary>
        /// Get SSC Dashboard Details
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult SSC()
        {
            DataSet ds = _dashboard.GetSSCDashboardDetails();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ViewBag.NewInstallationCount = Convert.ToString(ds.Tables[0].Rows[0]["NewInstallationCount"]);
                ViewBag.InProgressCount = Convert.ToString(ds.Tables[0].Rows[0]["InProgressCount"]);
                ViewBag.InstallationCompletedCount = Convert.ToString(ds.Tables[0].Rows[0]["InstallationCompletedCount"]);
                ViewBag.CancellationsCount = Convert.ToString(ds.Tables[0].Rows[0]["CancellationsCount"]);
                ViewBag.CompleteCount = Convert.ToString(ds.Tables[0].Rows[0]["CompleteCount"]);

                ViewBag.NotYetInvoiced = Convert.ToString(ds.Tables[0].Rows[0]["NotYetInvoiced"]);
                ViewBag.SERequestPending = Convert.ToString(ds.Tables[0].Rows[0]["SERequestPending"]);
            }

            return View();
        }

        /// <summary>
        /// FSA this instance.
        /// </summary>
        /// <returns>Action Result</returns>
        [UserAuthorization]
        public ActionResult FSA()
        {
            DataSet ds = _dashboard.GetFSADashboardDetails();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ViewBag.SSCRemovalRequest = Convert.ToString(ds.Tables[0].Rows[0]["SSCRemovalRequest"]);
                ViewBag.PendingSolarElectrician = Convert.ToString(ds.Tables[0].Rows[0]["PendingSolarElectrician"]);
            }
            return View();
        }

        /// <summary>
        /// the reseller details.
        /// </summary>
        /// <returns>Json Result</returns>
        public JsonResult Reseller()
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 3)
            {
                model.LstResellerUsers = _resellerService.GetData(ProjectSession.LoggedInUserId).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            }
            else
            {
                model.LstResellerUsers = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            }
            return new JsonResult()
            {
                Data = model.LstResellerUsers,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// the ram view.
        /// </summary>
        /// <returns>Json Result</returns>
        public JsonResult RAM()
        {
            FormBotReport model = new FormBotReport();
            if (ProjectSession.UserTypeId == 2)
            {
                model.ResellerID = ProjectSession.ResellerId;
            }
            model.LstRAMUsers = _formBotReportService.GetFormBotRAMUser(model);
            return new JsonResult()
            {
                Data = model.LstRAMUsers,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// RA this instance.
        /// </summary>
        /// <returns>Action Result</returns>
        [UserAuthorization]
        public ActionResult RA()
        {
            DataSet ds = _dashboard.GetRADashboardDetails();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ViewBag.PendingSolarCompany = Convert.ToString(ds.Tables[0].Rows[0]["PendingSolarCompany"]);
            }
            return View();
        }

        /// <summary>
        /// get solar company.
        /// </summary>
        /// <returns>Action Result</returns>
        public ActionResult SolarCompany(string isAllScaJobView = "")
        {
            FormBotReport model = new FormBotReport();
            var lstSolarCompany = _formBotReportService.GetSolarCompany(isAllScaJobView);
            model.LstSolarCompany = lstSolarCompany;

            return new JsonResult()
            {
                Data = model.LstSolarCompany,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        //[UserAuthorization]
        //public ActionResult FCO()
        //{
        //    return View();
        //}

        /// <summary>
        /// Redirects to se pending.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>Actioin Result</returns>
        public ActionResult RedirectToSEPending(string status)
        {
            if (status == "Pending")
            {
                TempData["PendingDashboardStatus"] = "Pending";
            }
            return RedirectToAction("SE", "User");
        }

        /// <summary>
        /// Redirects to SCA pending.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>Action Result</returns>
        public ActionResult RedirectToSCAPending(string status)
        {
            if (status == "Pending")
            {
                TempData["PendingSCADashboardStatus"] = "Pending";
            }
            return RedirectToAction("SCA", "User");
        }

        /// <summary>
        /// STCs the general report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>Json Result</returns>
        public JsonResult STCGeneralReport(FormBotReport formbotReportModel)
        {
            AllocationReport obj = new AllocationReport();
            //if (ProjectSession.UserTypeId == 4)
            //    formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);

            DataSet dsReport = _formBotReportService.GetSTCGeneralDashboardReport(formbotReportModel);
            obj.dsSTCGeneralDashboardChart = DBClient.ToListof<FormBotReport>(dsReport.Tables[0]);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>Json result</returns>
        public JsonResult GetRECFailureReasonsReportsList(FormBotReport formbotReportModel)
        {
            if (ProjectSession.UserTypeId == 2)
            {
                formbotReportModel.hdnFormBotResellerAssignedUser = Convert.ToString(ProjectSession.ResellerId);
            }
            if (ProjectSession.UserTypeId == 4)
            {
                formbotReportModel.hdnFormBotSolarCompanyAssignedUser = Convert.ToString(ProjectSession.SolarCompanyId);
            }
            List<FormBotReport> lstRecord = _formBotReportService.GetRECFailureReasonsDashboardReportsList(formbotReportModel);
            return Json(lstRecord, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get STC job parts for FSA/FCO users.
        /// </summary>
        /// <param name="resellerIds"></param>
        /// <param name="isAllReseller"></param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetSTCJobParts(string resellerIds, int isAllReseller)
        {
            List<DashboardSTCJobStaus> lstSTCJobStatus = new List<DashboardSTCJobStaus>();
            //try
            //{
            lstSTCJobStatus = _dashboard.GetFSAFCO_STCJobStaus(resellerIds, (isAllReseller == 1 ? true : false));
            //}
            //catch (Exception e)
            //{

            //}
            return PartialView("_FSASCOSTCJobStatus", lstSTCJobStatus);
        }

        /// <summary>
        /// Get reseller by FCO.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Json Result</returns>
        public JsonResult ResellerByFCO(int userId)
        {
            List<FormBot.Entity.Reseller> lstReseller = new List<FormBot.Entity.Reseller>();
            lstReseller = _dashboard.GetResellerByFCO(userId);
            return new JsonResult()
            {
                Data = lstReseller,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <returns>String Json</returns>
        public string GetComplianceTeam()
        {
            DataSet ds = _dashboard.GetComplianceTeam();
            return JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
        }

        /// <summary>
        /// Get STC job parts for RA/RAM  users.
        /// </summary>
        /// <param name="solarCompanyIds">solar Company Ids</param>
        /// <param name="sortCol">sort Column</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetRARAMSTCJobParts(string solarCompanyIds, string sortCol)
        {
            List<DashboardSTCJobStaus> lstSTCJobStatus = new List<DashboardSTCJobStaus>();
            //try
            //{
            lstSTCJobStatus = _dashboard.GetRARAM_STCJobStaus(solarCompanyIds, sortCol);
            //}
            //catch (Exception e)
            //{
            //}
            return PartialView("_FSASCOSTCJobStatus", lstSTCJobStatus);
        }

        /// <summary>
        /// Get solar company by RAM
        /// </summary>
        /// <param name="ramIds">Ram Ids</param>
        /// <returns>Json Result</returns>
        public JsonResult GetSolarCompanyByRAM(string ramIds,string isAllScaJobView="")
        {
            List<FormBot.Entity.SolarCompany> lstSolarCompany = new List<FormBot.Entity.SolarCompany>();
            lstSolarCompany = _dashboard.GetSolarCompanyByRAM(ramIds,isAllScaJobView);
            return new JsonResult()
            {
                Data = lstSolarCompany,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public void GetRequestingSCAandSSCForSEandSC()
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);

            IList<FormBot.Entity.User> lstSCASSCUser = _dashboard.GetRequestingSCAandSSCForSEandSC(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, gridParam.SortCol, gridParam.SortDir);
            if (lstSCASSCUser.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSCASSCUser.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSCASSCUser.FirstOrDefault().TotalRecords;

            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSCASSCUser, gridParam));
        }

        [HttpGet]
        [ValidateInput(false)]

        public ActionResult GetDashboardJobCERApproved(bool IsShowInDashboard, bool IsGridView = false)
        {
            FormBot.Entity.PricingManager result = _dashboard.GetDashboardJobCERApproved();
            result.IsGridView = IsGridView;
            result.IsShowInDashboard = IsShowInDashboard;
            //Here Only That Jobs will come whose status is submit to trade
            ViewBag.CurrentSTCJobStatus = SystemEnums.STCJobStatus.SubmittoTrade.GetHashCode();
            return this.PartialView("~/Views/Job/_CERApprovedSummary.cshtml", result);
        }

        //[HttpGet]
        //public string GetNotifications(DateTime dt)
        //{
        //    DataSet ds = _dashboard.GetNotifications(dt);
        //    return JsonConvert.SerializeObject(ds.Tables[0]);
        //}
        //[HttpGet]
        //public string GetNotifications(DateTime dt)
        //{
        //    //DataSet ds = _dashboard.GetNotifications(dt);
        //    //return JsonConvert.SerializeObject(ds.Tables[0]);
        //    if (ProjectSession.SnackbarId != 0)
        //    {
        //        DataSet ds = _dashboard.GetNotificationsById(ProjectSession.SnackbarId, dt);
        //        return JsonConvert.SerializeObject(ds.Tables[0]);
        //    }
        //    else
        //    {
        //        DataSet ds1 = _dashboard.GetNotifications(dt);
        //        return JsonConvert.SerializeObject(ds1.Tables[0]);
        //    }


        //}
        //#endregion
        //[HttpGet]
        //public JsonResult SetsnackbarSession(int SnackbarId)
        //{
        //    DateTime date = DateTime.Today;
        //    ProjectSession.SnackbarId = SnackbarId;
        //    DataSet ds = _dashboard.GetNotificationsById(ProjectSession.SnackbarId, date);
        //    //return JsonConvert.SerializeObject(ds.Tables[0]);
        //    return new JsonResult() { Data = true };
        //}

        #endregion
    }
}
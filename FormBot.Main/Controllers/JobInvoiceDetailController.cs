using FormBot.BAL.Service.Job;
using FormBot.BAL.Service;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Main.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.OAuth;
using FormBot.BAL;
using xero = Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;
using Xero.Api.Core.Model.Status;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Xml;
using FormBot.Entity;
using FormBot.Entity.Settings;
using Xero.Api.Infrastructure.Exceptions;
using FormBot.Entity.Email;
using FormBot.BAL.Service;
using System.Text;
using FormBot.Main.Infrastructure;
using Xero.Api.Core.Model;
using Xero.NetStandard.OAuth2.Api;
using oauthModel = Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Token;
using System.Threading.Tasks;
using System.Xml.Linq;
using FormBot.Helper.Helper;

namespace FormBot.Main.Controllers
{
    public class JobInvoiceDetailController : Controller
    {
        #region Properties

        private readonly IJobPartsBAL _jobParts;
        private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
        //private IMvcAuthenticator _authenticator;
        private readonly IEmailBAL _emailBAL;
        private readonly ICreateJobBAL _createJobBAL;
        //private ISettingsBAL _settingsBAL;
        //private ApiUser _user;
        //private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
        private readonly IJobInvoiceBAL _jobInvoice;
        private readonly ICreateJobHistoryBAL _jobHistory;
        private readonly IEmailBAL _emailService;
        private ApplicationSettingsTest xeroApiHelper;
        private readonly ILogger _log;

        //public bool isAllPart = false;

        #endregion

        #region Constructor

        /// <summary>
        /// JobInvoiceDetailController
        /// </summary>
        /// <param name="jobInvoiceDetail">jobInvoiceDetail</param>
        /// <param name="jobInvoice">jobInvoice</param>
        /// <param name="emailBAL">emailBAL</param>
        /// <param name="createJobBAL">createJobBAL</param>
        /// <param name="jobParts">jobParts</param>
        /// <param name="jobHistory">jobHistory</param>
        /// <param name="emailService">emailService</param>
        public JobInvoiceDetailController(IJobInvoiceDetailBAL jobInvoiceDetail, IJobInvoiceBAL jobInvoice, IEmailBAL emailBAL, ICreateJobBAL createJobBAL, IJobPartsBAL jobParts, ICreateJobHistoryBAL jobHistory, IEmailBAL emailService, ILogger log)
        {
            this._jobInvoiceDetail = jobInvoiceDetail;
            this._jobInvoice = jobInvoice;
            this._emailBAL = emailBAL;
            this._createJobBAL = createJobBAL;
            //this._settingsBAL = settingsBAL;
            this._jobParts = jobParts;
            this._jobHistory = jobHistory;
            this._emailService = emailService;
            this._log = log;
        }

        #endregion

        #region method

        /// <summary>
        /// Sync job parts using xero api
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult SyncParts()
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            JobParts jobParts = new JobParts();
            jobParts.isAllPart = false;
            try
            {
                if (!TokenUtilities.TokenExists())
                {
                    //XeroConnectController xcc = new XeroConnectController();
                    //xcc.Connect();
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }

                DataSet dsJobParts = SyncJobPartItems();
                if (dsJobParts == null)
                {
                    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //TempData["SyncMsg"] = "Success";
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                //jobParts.SyncValue = 1;
                //return PartialView("_JobParts", jobParts);
            }
            catch (RenewTokenException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));
                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Sync job parts using xero api
        /// </summary>
        /// <returns>dataset</returns>
        public DataSet SyncJobPartItems()
        {
            DataSet dsJobParts = null;
           
            var token = TokenUtilities.GetStoredToken();
            string accessToken = token.AccessToken;
            string xeroTenantId = token.Tenants[0].TenantId.ToString();
            var api = new AccountingApi();

            AccountingApi accountingApi = new AccountingApi();
            var result = Task.Run(async () => await accountingApi.GetItemsAsync(accessToken, xeroTenantId));
            result.Wait();
            var items = result.Result._Items;
            // var items = api.Items;
            var objJobParts = items.ToList();
            int? solarCompanyId = ProjectSession.SolarCompanyId;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;

            List<JobParts> lstJobParts = objJobParts.AsEnumerable().Select(row =>
            new JobParts
            {
                Description = row.Description,
                ItemCode = row.Code,
                Purchase = Convert.ToDecimal(row.PurchaseDetails.UnitPrice),
                Sale = Convert.ToDecimal(row.SalesDetails.UnitPrice),
                IsDeleted = false,
                XeroPartId = row.ItemID.ToString(),
                TaxType = row.SalesDetails.TaxType,
                SaleAccountCode = row.SalesDetails.AccountCode
            }).ToList();

            string totalJobPartsIds = string.Empty;

            if (lstJobParts.Count > 0)
            {
                //strUnitId = string.Join("_", dsUnitGroup.Tables[0].Rows.OfType<DataRow>().Where(n => Array.IndexOf(strDistinctUnitGroupIds.Split('_'), Convert.ToString(n.Field<int>("intUnitGroupId"))) > -1).Select(r => r.Field<int>("intUnitId")));
                totalJobPartsIds = string.Join(",", lstJobParts.AsEnumerable().Select(r => r.XeroPartId));
            }

            string jobPartsJson = string.Empty;
            jobPartsJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstJobParts);

            //JobPartsBAL jobParts = new JobPartsBAL();
            dsJobParts = _jobParts.InsertJobPartsUsingSyncXero(jobPartsJson, ProjectSession.LoggedInUserId, DateTime.Now, solarCompanyId, ProjectSession.UserTypeId, totalJobPartsIds);

            return dsJobParts;
        }

        /// <summary>
        /// Get job parts
        /// </summary>
        /// <param name="isAllPart">isAllPart</param>
        /// <returns>action result</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult GetJobParts(bool isAllPart = false)
        {
            JobParts jobParts = new JobParts();
            jobParts.UserTypeId = ProjectSession.UserTypeId;
            jobParts.isAllPart = isAllPart;
            return View("GetJobParts", jobParts);
        }

        /// <summary>
        /// Get job parts list
        /// </summary>
        /// <param name="itemCodeOrDescription">itemCodeOrDescription</param>
        /// <param name="isAllPart">isAllPart</param>
        public void GetJobPartsList(string itemCodeOrDescription = "", bool isAllPart = false)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            bool isXeroParts = !(isAllPart);

            //if (isAllPart)
            //    isXeroParts = false;
            //else
            //    isXeroParts = true;

            //JobPartsBAL jobParts = new JobPartsBAL();
            IList<JobParts> lstJobParts = _jobParts.GetJobPartsList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, ProjectSession.SolarCompanyId, itemCodeOrDescription, isXeroParts);
            if (lstJobParts.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstJobParts.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstJobParts.FirstOrDefault().TotalRecords;

            }

            //isAllPart = false;

            HttpContext.Response.Write(Grid.PrepareDataSet(lstJobParts, gridParam));
        }

        /// <summary>
        /// Get job parts for partial view
        /// </summary>
        /// <param name="isAllParts">isAllParts</param>
        /// <returns>partial result</returns>
        [HttpGet]
        public PartialViewResult GetJobPartsPartialView(string isAllParts)
        {
            JobParts jobParts = new JobParts();
            jobParts.isAllPart = Convert.ToBoolean(isAllParts);
            return PartialView("_JobParts", jobParts);
        }

        /// <summary>
        /// Get job parts by id on click
        /// </summary>
        /// <param name="jobPartId">jobPartId</param>
        /// <returns>string</returns>
        [HttpGet]
        public string GetJobPartsByIdOnClick(string jobPartId)
        {
            string jobPartsJson = "[]";

            DataSet dsJobParts = _jobParts.GetJobPartsById(Convert.ToInt32(jobPartId));
            if (dsJobParts != null && dsJobParts.Tables[0] != null && dsJobParts.Tables[0].Rows.Count > 0)
            {
                jobPartsJson = Newtonsoft.Json.JsonConvert.SerializeObject(dsJobParts.Tables[0]);
            }

            return jobPartsJson;
        }

        /// <summary>
        /// Get job parts by jobInvoiceDetailId and jobId
        /// </summary>
        /// <param name="jobInvoiceDetailID">jobInvoiceDetailID</param>
        /// <param name="jobId">jobId</param>
        /// <returns>action result</returns>
        [HttpGet]
        public ActionResult GetJobPartsById(string jobInvoiceDetailID, string jobId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetJobPart(jobInvoiceDetailID, jobId);
            if (jobInvoiceDetail.JobInvoiceDetailID > 0)
            {
                // no line
            }
            else
            {
                jobInvoiceDetail.Tax = ProjectSession.PartAccountTax;
                jobInvoiceDetail.IsTaxInclusive = ProjectSession.IsTaxInclusive;
            }
            return PartialView("_AddEditJobPart", jobInvoiceDetail);
        }

        /// <summary>
        /// Gets the job time by identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <returns>action result</returns>
        [HttpGet]
        public ActionResult GetJobTimeById(string jobId, string jobInvoiceDetailID)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            List<SelectListItem> lstList = new List<SelectListItem>();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetTimePart(jobInvoiceDetailID, jobId, ref lstList);
            ViewBag.JobVisit = lstList;
            jobInvoiceDetail.Guid = Guid.NewGuid().ToString();
            ViewBag.Guid = Guid.NewGuid().ToString();
            return PartialView("_AddTime", jobInvoiceDetail);
        }

        /// <summary>
        /// Gets the job payment by identifier.
        /// </summary>
        /// <param name="jobInvoiceId">The job invoice identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="InvoiceNumber">The invoice number.</param>
        /// <param name="JobInvoiceDetailId">The job invoice detail identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetJobPaymentById(int jobInvoiceId, string jobId, string InvoiceNumber, string JobInvoiceDetailId)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            FormBot.Main.Infrastructure.InvoiceDetailsHelper objInvoiceDetails = new Infrastructure.InvoiceDetailsHelper(new JobInvoiceDetailBAL());
            jobInvoiceDetail = objInvoiceDetails.GetJobPayment(jobInvoiceId, jobId, InvoiceNumber, JobInvoiceDetailId);

            return PartialView("_AddPayment", jobInvoiceDetail);
        }

        /// <summary>
        /// Insert job invoice parts 
        /// </summary>
        /// <param name="jobInvoiceDetail">jobInvoiceDetail</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public JsonResult InsertJobInvoiceDetailAndJobParts(JobInvoiceDetail jobInvoiceDetail)
        {
            int jobInvoiceDetailID = 0;
            int jobPartId = 0;
            int jobInvoiceID = 0;

            jobInvoiceDetail.JobInvoiceType = Convert.ToByte(SystemEnums.JobInvoiceType.Part);

            try
            {
                RequiredValidationField(jobInvoiceDetail);
                if (ModelState.IsValid)
                {

                    if (jobInvoiceDetail.Signature != null && jobInvoiceDetail.Signature != "")
                        jobInvoiceDetail.FileName = jobInvoiceDetail.Signature;

                    if (jobInvoiceDetail.OldFileName != jobInvoiceDetail.FileName)
                    {
                        DeleteFile(Convert.ToString(jobInvoiceDetail.JobId), jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.OldFileName);
                    }

                    //JobInvoiceDetailBAL jobInvoiceDetailBAL = new JobInvoiceDetailBAL();
                    DataSet dsJobInvoiceDetail = new DataSet();
                    DateTime dtStartDateTime = Convert.ToDateTime(jobInvoiceDetail.DateAdded) + TimeSpan.Parse(jobInvoiceDetail.TimeAdded);

                    int? solarCompanyId;

                    if (ProjectSession.UserTypeId == 9 || ProjectSession.UserTypeId == 7)
                        solarCompanyId = null;
                    else
                        solarCompanyId = ProjectSession.SolarCompanyId;

                    SettingsBAL settingsBAL = new SettingsBAL();
                    Entity.Settings.Settings settings = new Entity.Settings.Settings();
                    settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId,
                        ProjectSession.UserTypeId, solarCompanyId, ProjectSession.ResellerId);
                    if (jobInvoiceDetail.JobInvoiceDetailID > 0)
                    {
                        //update

                        dsJobInvoiceDetail = _jobInvoiceDetail.InsertJobInvoiceDetail(jobInvoiceDetail.JobInvoiceDetailID, jobInvoiceDetail.JobInvoiceID,
                            jobInvoiceDetail.JobPartID, jobInvoiceDetail.IsBillable, jobInvoiceDetail.JobScheduleID, ProjectSession.LoggedInUserId, jobInvoiceDetail.Sale, dtStartDateTime, jobInvoiceDetail.ItemCode, jobInvoiceDetail.Quantity, jobInvoiceDetail.Purchase, jobInvoiceDetail.Margin, jobInvoiceDetail.Description
                             , jobInvoiceDetail.FileName, null, null, null, jobInvoiceDetail.JobInvoiceType, false, ProjectSession.LoggedInUserId,
                             ProjectSession.LoggedInUserId, false, solarCompanyId, true, jobInvoiceDetail.InvoiceNumber, jobInvoiceDetail.JobId,
                             jobInvoiceDetail.OwnerUsername, jobInvoiceDetail.SentTo, settings, jobInvoiceDetail.TaxRateId, jobInvoiceDetail.SaleAccountCode);
                    }
                    else
                    {
                        //insert

                        dsJobInvoiceDetail = _jobInvoiceDetail.InsertJobInvoiceDetail(jobInvoiceDetail.JobInvoiceDetailID, jobInvoiceDetail.JobInvoiceID,
                            jobInvoiceDetail.JobPartID, jobInvoiceDetail.IsBillable, jobInvoiceDetail.JobScheduleID, ProjectSession.LoggedInUserId, jobInvoiceDetail.Sale
                             , dtStartDateTime, jobInvoiceDetail.ItemCode, jobInvoiceDetail.Quantity, jobInvoiceDetail.Purchase, jobInvoiceDetail.Margin, jobInvoiceDetail.Description
                             , jobInvoiceDetail.FileName, null, null, null, jobInvoiceDetail.JobInvoiceType, false, ProjectSession.LoggedInUserId,
                             ProjectSession.LoggedInUserId, false, solarCompanyId, true, jobInvoiceDetail.InvoiceNumber, jobInvoiceDetail.JobId,
                             jobInvoiceDetail.OwnerUsername, jobInvoiceDetail.SentTo, settings, jobInvoiceDetail.TaxRateId, jobInvoiceDetail.SaleAccountCode);
                    }

                    if (dsJobInvoiceDetail != null && dsJobInvoiceDetail.Tables.Count > 0)
                    {
                        if (dsJobInvoiceDetail.Tables[0] != null && dsJobInvoiceDetail.Tables[0].Rows.Count > 0)
                            jobPartId = Convert.ToInt32(dsJobInvoiceDetail.Tables[0].Rows[0]["JobPartID"]);
                        if (dsJobInvoiceDetail.Tables[1] != null && dsJobInvoiceDetail.Tables[1].Rows.Count > 0)
                            jobInvoiceDetailID = Convert.ToInt32(dsJobInvoiceDetail.Tables[1].Rows[0]["JobInvoiceDetailID"]);
                        if (dsJobInvoiceDetail.Tables[2] != null && dsJobInvoiceDetail.Tables[2].Rows.Count > 0)
                            jobInvoiceID = Convert.ToInt32(dsJobInvoiceDetail.Tables[2].Rows[0]["JobInvoiceID"]);
                    }

                    if (jobInvoiceDetail.JobInvoiceDetailID == 0 || jobInvoiceDetail.JobInvoiceDetailID == null)
                    {
                        string guid = jobInvoiceDetail.Guid;

                        string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobInvoiceDetail.JobId + "\\" + "Invoice" + "\\" + jobInvoiceDetail.Guid + "_" + jobInvoiceDetail.FileName;
                        string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobInvoiceDetail.JobId + "\\" + "Invoice" + "\\" + jobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;

                        if (!string.IsNullOrEmpty(jobInvoiceDetail.Guid) && System.IO.File.Exists(sourceDirectory))
                        {
                            System.IO.File.Move(sourceDirectory, destinationDirectory);
                        }
                    }
                    //return Json("successs" + "#" + jobInvoiceDetailID + "#" + jobPartId + "#" + jobInvoiceID, JsonRequestBehavior.AllowGet);
                    return Json(new { status = "successs" + "#" + jobInvoiceDetailID + "#" + jobPartId + "#" + jobInvoiceID, fileName = jobInvoiceDetail.FileName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg += d.Errors[0].ErrorMessage;
                    });
                    return Json("error" + "#" + msg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("error" + "#" + "Job invoice detail has not been saved.", JsonRequestBehavior.AllowGet);
            }
            //return Json(jobInvoiceDetailID + "^" + jobPartId, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View download invoice file
        /// </summary>
        /// <param name="FileName">FileName</param>
        /// <param name="JobID">JobID</param>
        /// <returns>Action result</returns>
        [HttpGet]
        public ActionResult ViewDownloadInvoiceFile(string FileName, string JobID)
        {
            var path = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + JobID + "\\" + "Invoice", FileName);
            //var path=@"\\azureexamplestorage.file.core.windows.net\test\UserDocuments\27\examples.png";

            var fileData = System.IO.File.ReadAllBytes(path);
            //FileName="examples.png";
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/octet-stream";

            if (FileName.Split('_').Length > 1)
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + FileName));
            Response.BinaryWrite(fileData);

            //    Response.AddHeader("content-disposition", "attachment;  filename=\"" + FileName + "\"");
            //Response.BinaryWrite(fileData);
            //Response.End();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View download invoice report file
        /// </summary>
        /// <param name="FileName">FileName</param>
        /// <param name="JobID">JobID</param>
        /// <returns>Action result</returns>
        public ActionResult ViewDownloadInvoiceReportFile(string FileName, string JobID)
        {
            var path = Path.Combine(ProjectSession.ProofDocumentsURL.Replace("/", "") + "\\" + "JobDocuments" + "\\" + JobID + "\\" + "Invoice\\Report", FileName);
            //var path=@"\\azureexamplestorage.file.core.windows.net\test\UserDocuments\27\examples.png";

            if (System.IO.File.Exists(path))
            {
                //    return Json("false", JsonRequestBehavior.AllowGet);
                var fileData = System.IO.File.ReadAllBytes(path);
                //FileName="examples.png";

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + FileName));
                Response.BinaryWrite(fileData);

                //Response.ContentType = "application/octet-stream";
                //Response.AddHeader("content-disposition", "attachment;  filename=\"" + FileName + "\"");
                //Response.BinaryWrite(fileData);
                //Response.End();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert job invoice payment
        /// </summary>
        /// <param name="jobInvoiceDetail">jobInvoiceDetail</param>
        /// <returns>Action result</returns>
        [HttpPost]
        public JsonResult InsertJobInvoicePayment(JobInvoiceDetail jobInvoiceDetail)
        {
            int jobInvoiceDetailID = 0;
            int jobInvoiceID = 0;
            jobInvoiceDetail.JobInvoiceType = Convert.ToByte(SystemEnums.JobInvoiceType.Payment);
            RequiredValidationField(jobInvoiceDetail);
            if (ModelState.IsValid)
            {

                if (jobInvoiceDetail.Signature != null && jobInvoiceDetail.Signature != "")
                    jobInvoiceDetail.FileName = jobInvoiceDetail.Signature;

                if (jobInvoiceDetail.OldFileName != jobInvoiceDetail.FileName)
                {
                    DeleteFile(Convert.ToString(jobInvoiceDetail.JobId), jobInvoiceDetail.JobInvoiceDetailID + "_" + jobInvoiceDetail.OldFileName);
                }

                //JobInvoiceDetailBAL jobInvoiceDetailBAL = new JobInvoiceDetailBAL();
                DataSet dsJobInvoiceDetail = new DataSet();
                DateTime dtStartDateTime = Convert.ToDateTime(jobInvoiceDetail.DateAdded) + TimeSpan.Parse(jobInvoiceDetail.TimeAdded);

                int? solarCompanyId;

                if (ProjectSession.UserTypeId == 9 || ProjectSession.UserTypeId == 7)
                    solarCompanyId = null;
                else
                    solarCompanyId = ProjectSession.SolarCompanyId;

                SettingsBAL settingsBAL = new SettingsBAL();
                Entity.Settings.Settings settings = new Entity.Settings.Settings();
                settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId,
                    ProjectSession.UserTypeId, solarCompanyId, ProjectSession.ResellerId);

                if (jobInvoiceDetail.JobInvoiceDetailID > 0)
                {
                    //update

                    dsJobInvoiceDetail = _jobInvoiceDetail.InsertJobInvoiceDetail(jobInvoiceDetail.JobInvoiceDetailID, jobInvoiceDetail.JobInvoiceID,
                        jobInvoiceDetail.JobPartID, true, jobInvoiceDetail.JobScheduleID, ProjectSession.LoggedInUserId, jobInvoiceDetail.Sale
                         , dtStartDateTime, jobInvoiceDetail.ItemCode, jobInvoiceDetail.Quantity, jobInvoiceDetail.Purchase, jobInvoiceDetail.Margin, jobInvoiceDetail.Description
                         , jobInvoiceDetail.FileName, null, jobInvoiceDetail.PaymentType, jobInvoiceDetail.PaymentAmount, jobInvoiceDetail.JobInvoiceType,
                         false, ProjectSession.LoggedInUserId, ProjectSession.LoggedInUserId, false, ProjectSession.SolarCompanyId, false,
                         jobInvoiceDetail.InvoiceNumber, jobInvoiceDetail.JobId, jobInvoiceDetail.OwnerUsername, jobInvoiceDetail.SentTo, settings);
                }
                else
                {
                    //insert

                    dsJobInvoiceDetail = _jobInvoiceDetail.InsertJobInvoiceDetail(jobInvoiceDetail.JobInvoiceDetailID, jobInvoiceDetail.JobInvoiceID,
                        jobInvoiceDetail.JobPartID, true, jobInvoiceDetail.JobScheduleID, ProjectSession.LoggedInUserId, jobInvoiceDetail.Sale
                         , dtStartDateTime, jobInvoiceDetail.ItemCode, jobInvoiceDetail.Quantity, jobInvoiceDetail.Purchase, jobInvoiceDetail.Margin, jobInvoiceDetail.Description
                         , jobInvoiceDetail.FileName, null, jobInvoiceDetail.PaymentType, jobInvoiceDetail.PaymentAmount, jobInvoiceDetail.JobInvoiceType,
                         false, ProjectSession.LoggedInUserId, ProjectSession.LoggedInUserId, false, ProjectSession.SolarCompanyId, false,
                         jobInvoiceDetail.InvoiceNumber, jobInvoiceDetail.JobId, jobInvoiceDetail.OwnerUsername, jobInvoiceDetail.SentTo, settings);
                }

                if (dsJobInvoiceDetail != null && dsJobInvoiceDetail.Tables.Count > 0)
                {

                    if (dsJobInvoiceDetail.Tables[0] != null && dsJobInvoiceDetail.Tables[0].Rows.Count > 0)
                        jobInvoiceDetailID = Convert.ToInt32(dsJobInvoiceDetail.Tables[0].Rows[0]["JobInvoiceDetailID"]);
                    if (dsJobInvoiceDetail.Tables[1] != null && dsJobInvoiceDetail.Tables[1].Rows.Count > 0)
                        jobInvoiceID = Convert.ToInt32(dsJobInvoiceDetail.Tables[1].Rows[0]["JobInvoiceID"]);
                }

                if (jobInvoiceDetail.JobInvoiceDetailID == 0 || jobInvoiceDetail.JobInvoiceDetailID == null)
                {
                    string guid = jobInvoiceDetail.Guid;
                    string sourceDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobInvoiceDetail.JobId + "\\" + "Invoice" + "\\" + jobInvoiceDetail.Guid + "_" + jobInvoiceDetail.FileName;
                    string destinationDirectory = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobInvoiceDetail.JobId + "\\" + "Invoice" + "\\" + jobInvoiceDetailID + "_" + jobInvoiceDetail.FileName;
                    if (!string.IsNullOrEmpty(jobInvoiceDetail.Guid) && System.IO.File.Exists(sourceDirectory))
                    {
                        System.IO.File.Move(sourceDirectory, destinationDirectory);
                    }
                }

                return Json(new { status = "successs" + "#" + jobInvoiceDetailID + "#" + jobInvoiceID, fileName = jobInvoiceDetail.FileName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string msg = string.Empty;
                ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                {
                    if (d.Errors.Count > 0)
                        msg += d.Errors[0].ErrorMessage;
                });
                return Json("error" + "#" + msg, JsonRequestBehavior.AllowGet);
            }
            //return Json(jobInvoiceDetailID + "^" + jobPartId, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get PaymentMethods
        /// </summary>
        /// <returns>Json Result</returns>
        [HttpGet]
        public JsonResult GetPaymentMethods()
        {
            List<SelectListItem> Items = new List<SelectListItem>(); // { new SelectListItem() { Text = "High", Value = "High" }, new SelectListItem() { Text = "Normal", Value = "Normal" }, new SelectListItem() { Text = "Low", Value = "Low" } };
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.ManualCreditCard.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.ManualCreditCard) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.Cash.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.Cash) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.Cheque.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.Cheque) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.Credit.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.Credit) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.BankTransfer.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.BankTransfer) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.Eftpos.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.Eftpos) });
            Items.Add(new SelectListItem() { Text = Helper.SystemEnums.PaymentType.Debit.ToString(), Value = Convert.ToString((int)Helper.SystemEnums.PaymentType.Debit) });
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Requireds the validation field.
        /// </summary>
        /// <param name="jobInvoiceDetail">jobInvoiceDetail</param>
        public void RequiredValidationField(JobInvoiceDetail jobInvoiceDetail)
        {
            if (jobInvoiceDetail.JobInvoiceType == 3)
            {
                ModelState.Remove("TimeStart");
                ModelState.Remove("ItemCode");
                ModelState.Remove("InvoiceStartDate");
                ModelState.Remove("InvoiceStartTime");
                if (jobInvoiceDetail.JobInvoiceDetailID == 0)
                {
                    ModelState.Remove("JobInvoiceDetailID");
                }
            }

            if (jobInvoiceDetail.JobInvoiceType == 1)
            {
                ModelState.Remove("TimeStart");
                ModelState.Remove("PaymentType");
                ModelState.Remove("PaymentAmount");
                ModelState.Remove("InvoiceStartDate");
                ModelState.Remove("InvoiceStartTime");
                if (jobInvoiceDetail.JobInvoiceDetailID == 0)
                    ModelState.Remove("JobInvoiceDetailID");
            }
        }

        /// <summary>
        /// Delete uploaded file from invoice folder
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="FolderName">FolderName</param>
        /// <param name="OldInvoiceFile">OldInvoiceFile</param>
        /// <returns>Action result</returns>
        [AllowAnonymous]
        public JsonResult DeleteFileFromInvoiceFolder(string fileName, string FolderName, string OldInvoiceFile)
        {
            if (OldInvoiceFile != fileName && fileName != null)
            {
                DeleteFile(FolderName, fileName);
            }
            this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FolderName">FolderName</param>
        /// <param name="fileName">fileName</param>
        private void DeleteFile(string FolderName, string fileName)
        {
            DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + FolderName + "\\" + "Invoice", fileName));
        }

        /// <summary>
        /// Get settings data 
        /// </summary>
        /// <returns>Action result</returns>
        public Entity.Settings.Settings GetSettingsData()
        {
            SettingsBAL settingsBAL = new SettingsBAL();
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            int? solarCompanyId = 0;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;
            else
                solarCompanyId = ProjectSession.SolarCompanyId;

            settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, solarCompanyId, ProjectSession.ResellerId);
            return settings;
        }

        #endregion

        #region Invoice details

        /// <summary>
        /// Get RandomString
        /// </summary>
        /// <param name="length">length</param>
        /// <returns>string</returns>
        [NonAction]
        public static string GetRandomString(int length)
        {
            var numArray = new byte[length];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(numArray);
            return SanitiseBase64String(Convert.ToBase64String(numArray), length);
        }

        /// <summary>
        /// SanitiseBase64 String
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="maxLength">maxLength</param>
        /// <returns>string</returns>
        [NonAction]
        private static string SanitiseBase64String(string input, int maxLength)
        {
            input = input.Replace("-", "");
            input = input.Replace("=", "");
            input = input.Replace("/", "");
            input = input.Replace("+", "");
            input = input.Replace(" ", "");
            while (input.Length < maxLength)
                input = input + GetRandomString(maxLength);
            return input.Length <= maxLength ?
                input.ToUpper() :
                input.ToUpper().Substring(0, maxLength);
        }

        /// <summary>
        /// Get Job invoice detail partial view
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <param name="jobId">jobId</param>
        /// <returns>Action Result</returns>
        public ActionResult GetJobInvoiceDetail(string invoiceID = "", int jobId = 0)
        {
            GetJobInvoiceDetailsList(invoiceID);
            BindInvoiceTo(jobId);

            JobInvoice jobInvoice = GetInvoice(Convert.ToInt32(invoiceID));

            bool IsFileExists = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report\\" + jobInvoice.InvoiceNumber + ".pdf")) ? true : false;
            if (IsFileExists)
            {
                ViewBag.PDFFound = true;
                ViewBag.Filename = jobInvoice.InvoiceNumber + ".pdf";
            }

            Entity.Settings.Settings setting = GetSettingsData();
            if (setting != null)
            {
                ViewBag.isXero = setting.IsXeroAccount;
            }
            else
            {
                ViewBag.isXero = false;
            }
            if (string.IsNullOrEmpty(jobInvoice.InvoiceNumber))
            {
                jobInvoice.InvoiceNumber = string.Format("INV-{0}", GetRandomString(4));
            }

            return View("_JobInvoiceDetail", jobInvoice);
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <returns>model</returns>
        [NonAction]
        public JobInvoice GetInvoice(int invoiceID = 0)
        {
            JobInvoice jobInvoice = new JobInvoice();
            try
            {
                if (invoiceID != 0)
                    jobInvoice = _jobInvoice.GetJobInvoiceById(Convert.ToInt32(invoiceID));
            }
            catch (Exception ex)
            {

            }
            return jobInvoice;
        }

        /// <summary>
        /// BindInvoiceTo
        /// </summary>
        /// <param name="jobId">jobId</param>
        [NonAction]
        public void BindInvoiceTo(int jobId = 0)
        {
            try
            {
                if (ProjectSession.UserTypeId == 4)
                {
                    IList<JobOwnerDetails> lstJobOwnerDetails =
                        _jobInvoice.GetInvoiceSendToDetails<JobOwnerDetails>(ProjectSession.LoggedInUserId,
                        ProjectSession.UserTypeId, jobId);
                    if (lstJobOwnerDetails.Count > 0)
                    {
                        ViewBag.UserList = new SelectList(lstJobOwnerDetails.ToList(), "FirstName", "Fullname");
                        ViewBag.UserType = "2";
                        ViewBag.InvoiceToUserId = lstJobOwnerDetails.FirstOrDefault().JobOwnerID;
                        //ViewBag.InvoiceToUserDetail = lstJobOwnerDetails.FirstOrDefault().CompanyName;
                        ViewBag.InvoiceToUserDetail = lstJobOwnerDetails.FirstOrDefault().OwnerAddress;
                        ViewBag.InvoiceToEmailId = lstJobOwnerDetails.FirstOrDefault().Email;
                    }
                }
                else if (ProjectSession.UserTypeId == 6 ||
                            ProjectSession.UserTypeId == 7 ||
                            ProjectSession.UserTypeId == 9)
                {
                    IList<FormBot.Entity.User> lstUser = _jobInvoice.GetInvoiceSendToDetails<FormBot.Entity.User>(ProjectSession.LoggedInUserId,
                        ProjectSession.UserTypeId, jobId);
                    if (lstUser.Count > 0)
                    {
                        ViewBag.UserList = new SelectList(lstUser.ToList(), "UserId", "Fullname");
                        ViewBag.UserType = "1";
                        ViewBag.InvoiceToUserId = lstUser.FirstOrDefault().UserId;
                        //ViewBag.InvoiceToUserDetail = lstUser.First().CompanyName;
                        ViewBag.InvoiceToUserDetail = lstUser.FirstOrDefault().OwnerAddress;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Delete JobInvoiceDetail
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult DeleteJobInvoiceDetail(string ids = "")
        {
            //try
            //{
            if (!string.IsNullOrEmpty(ids) && ids.Trim(',').Length > 0)
            {
                _jobInvoiceDetail.DeleteInvoiceDetail(ids.Trim(','));
            }
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Send Invoice
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="invoiceId">invoiceId</param>
        /// <param name="userType">userType</param>
        /// <param name="emailId">emailId</param>
        /// <param name="invoiceSendUserID">invoiceSendUserID</param>
        /// <param name="jobID">jobID</param>
        /// <param name="invoiceNumber">invoiceNumber</param>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult SendInvoice(string id = "", string invoiceId = "", string userType = "",
            string emailId = "", string invoiceSendUserID = "", string jobID = "", string invoiceNumber = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(invoiceId) && !string.IsNullOrEmpty(userType))
                {
                    if (userType == "2")
                    {
                        IList<JobOwnerDetails> lstJobOwnerDetails =
                       _jobInvoice.GetInvoiceSendToDetails<JobOwnerDetails>(ProjectSession.LoggedInUserId,
                       ProjectSession.UserTypeId, Convert.ToInt32(jobID));
                        JobOwnerDetails jobOwnerDetails = lstJobOwnerDetails.FirstOrDefault();

                        ////EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(24, null, null, null, null);
                        IEmailBAL emailService = null;
                        FormBot.BAL.Service.IUserBAL userBAL = null;
                        FormBot.BAL.Service.ICreateJobBAL job = null;

                        Areas.Email.Controllers.EmailController objEmail = new Areas.Email.Controllers.EmailController(emailService, userBAL, job, null, null, null);

                        string fileName = invoiceNumber;

                        //            bool IsFileExists = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report\\" + jobInvoice.InvoiceNumber + ".pdf")) ? true : false;
                        //if (IsFileExists)
                        //{

                        //if (System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report\\" + fileName)))
                        if (System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report\\" + fileName)))
                        {
                            string filePath = System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report\\" + fileName);
                            //RECRegistryHelper.WriteToLogFile("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " From Invoice details controller");
                            _log.LogException("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " From Invoice details controller",null);
                            var result = objEmail.UploadPhysicalFileAsAnAttachmentInEmail(fileName, filePath);

                            if (result != null)
                            {
                                var myJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                                var fileTmpDetails = Newtonsoft.Json.Linq.JObject.Parse(myJsonString);
                                var fileTmpName = fileTmpDetails["Data"]["tmp_name"].ToString();

                                if (fileTmpName != null)
                                {
                                    //Email.ComposeEmail composeEmail = new Email.ComposeEmail();
                                    //composeEmail.Attachments = Attachments;
                                    //_emailBAL.GeneralComposeAndSendMail(eTemplate, jobOwnerDetails.Email, composeEmail);
                                    //_jobInvoice.SendInvoice(id, invoiceId, userType);

                                    EmailInfo emailInfo = new EmailInfo();
                                    emailInfo.TemplateID = 24;
                                    List<FormBot.Email.AttachmentData> Attachments = new List<FormBot.Email.AttachmentData>
                                    {
                                        new FormBot.Email.AttachmentData
                                        {
                                            GeneratedName = fileTmpName,
                                            FileName = fileName
                                        }
                                    };

                                    _emailService.ComposeAndSendEmail(emailInfo, jobOwnerDetails.Email, Attachments, null, default(Guid), jobID);
                                    _jobInvoice.SendInvoice(id, invoiceId, userType);
                                }
                            }
                        }
                        InvoiceHistory objInvoiceHistory = new InvoiceHistory()
                        {
                            InvoiceNumber = invoiceNumber,
                            InvoiceTo = id,
                            JobID = Convert.ToInt32(jobID)
                        };
                        //bool isHistorySaved = _jobHistory.LogJobHistory(objInvoiceHistory, HistoryCategory.InvoiceSent);
                        string JobHistoryMessage = "has just sent invoice <b class=\"blue-title\">"+ objInvoiceHistory.InvoiceNumber +"</b> to " +objInvoiceHistory.InvoiceTo;
                        Common.SaveJobHistorytoXML(objInvoiceHistory.JobID, JobHistoryMessage, "Notifications", "InvoiceSent", ProjectSession.LoggedInName, false);
                    }

                }
                return this.Json(new { success = true });
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message });
            }
        }

        /// <summary>
        /// update InvoiceNumber
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <param name="invoiceNumber">invoiceNumber</param>
        /// <param name="status">status</param>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult updateInvoiceNumber(string invoiceId = "", string invoiceNumber = "", int status = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(invoiceId) && !string.IsNullOrEmpty(invoiceNumber))
                {
                    string result = _jobInvoice.UpdateInvoiceNumber(invoiceId, invoiceNumber, status);
                    return this.Json(new { success = true, result = result });
                }
                return this.Json(new { success = false });

            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message });
            }
        }

        /// <summary>
        /// Get JobInvoiceDetails List
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <param name="jobInvoiceType">jobInvoiceType</param>
        /// <param name="isInvoiced">isInvoiced</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <returns>list</returns>
        public IList<JobInvoiceDetail> GetJobInvoiceDetailsList(string invoiceID = "", int jobInvoiceType = 0,
            int isInvoiced = 0, string sortCol = "", string sortDir = "")
        {
            IList<JobInvoiceDetail> lstJobInvoiceDetail =
                _jobInvoiceDetail.GetJobInvoiceDetail(invoiceID, jobInvoiceType, isInvoiced,
                                    sortCol, sortDir);
            try
            {

                if (lstJobInvoiceDetail.Count > 0)
                {

                    ViewBag.SubTotal = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.SubTotal);
                    ViewBag.Cost = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.cost);
                    ViewBag.Profit = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Profit);

                    if (ViewBag.SubTotal != 0 && ViewBag.Profit != 0)
                        ViewBag.Margin = Math.Round(Convert.ToDecimal((100 * Convert.ToDecimal(ViewBag.Profit)) / Convert.ToDecimal(ViewBag.SubTotal)), MidpointRounding.AwayFromZero);
                    else
                        ViewBag.Margin = 0;

                    ViewBag.Tax = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Tax);

                    decimal totalWithOutTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                        c.IsBillable == true && c.IsTaxInclusive == false).Sum(c => c.SubTotal + c.Tax));
                    decimal totalWithTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                        c.IsBillable == true && c.IsTaxInclusive == true).Sum(c => c.SubTotal));
                    //ViewBag.SummaryTotal = Math.Round(Convert.ToDecimal(totalWithOutTax + totalWithTax), 2);

                    ViewBag.SummaryTotal = Math.Round(Convert.ToDecimal(ViewBag.SubTotal + ViewBag.Tax), 2);

                    ViewBag.Payments = lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3).Sum(c => c.PaymentAmount);
                    ViewBag.Ramaning = Math.Round(Convert.ToDecimal(ViewBag.SummaryTotal - ViewBag.Payments), 2);
                    //if (lstJobInvoiceDetail.Any(c => c.IsInvoiced == true))
                    //{
                    //    ViewBag.PDFFound = true;
                    //}
                }
                else
                {
                    ViewBag.Ramaning = ViewBag.Payments = ViewBag.SummaryTotal = ViewBag.Profit = ViewBag.Cost = ViewBag.SubTotal = 0;
                    ViewBag.Margin = 0;
                }
            }
            catch (Exception ex)
            {
                FormBot.Helper.Log.WriteError(ex);
            }
            TempData["lstJobInvoiceDetail"] = lstJobInvoiceDetail;
            return lstJobInvoiceDetail;
        }

        /// <summary>
        /// JobInvoiceTotalPartialView
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <param name="jobInvoiceType">jobInvoiceType</param>
        /// <param name="isInvoiced">isInvoiced</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <returns>Action Result</returns>
        public ActionResult JobInvoiceTotalPartialView(string invoiceID = "", int jobInvoiceType = 0,
            int isInvoiced = 0, string sortCol = "", string sortDir = "")
        {

            IList<JobInvoiceDetail> lstJobInvoiceDetail =
                _jobInvoiceDetail.GetJobInvoiceDetail(invoiceID, jobInvoiceType, isInvoiced,
                                    sortCol, sortDir);
            InvoicePaymentDetails obj = new InvoicePaymentDetails();
            if (lstJobInvoiceDetail.Count > 0)
            {
                obj.SubTotal = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.SubTotal);
                obj.Cost = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.cost));
                obj.Profit = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Profit));

                if (obj.SubTotal != 0 && obj.Profit != 0)
                    obj.Margin = Math.Round(Convert.ToDecimal((100 * Convert.ToDecimal(obj.Profit)) / Convert.ToDecimal(obj.SubTotal)), MidpointRounding.AwayFromZero);
                else
                    obj.Margin = 0;

                obj.Tax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).Sum(c => c.Tax));

                decimal totalWithOutTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                    c.IsBillable == true && c.IsTaxInclusive == false).Sum(c => c.SubTotal + c.Tax));
                decimal totalWithTax = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 &&
                    c.IsBillable == true && c.IsTaxInclusive == true).Sum(c => c.SubTotal));
                obj.SummaryTotal = Math.Round(Convert.ToDecimal(totalWithOutTax + totalWithTax), 2);
                obj.Payments = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3).Sum(c => c.PaymentAmount));
                obj.Ramaning = Math.Round(Convert.ToDecimal(obj.SummaryTotal - obj.Payments), 2);

            }
            else
            {
                obj.Tax = obj.Ramaning = obj.Payments = obj.SummaryTotal = obj.Profit = obj.Cost = obj.SubTotal = 0;
                obj.Margin = 0;
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get job invoice details based on invoice ID
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <param name="jobInvoiceType">jobInvoiceType</param>
        /// <param name="isInvoiced">isInvoiced</param>
        /// <param name="jobID">jobID</param>
        public void GetInvoiceDetailListing(string invoiceID = "", int jobInvoiceType = 0,
            int isInvoiced = 0, int jobID = 0)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            try
            {

                IList<JobInvoiceDetail> lstJobInvoiceDetail = new List<JobInvoiceDetail>();

                if (TempData["lstJobInvoiceDetail"] == null)
                {
                    lstJobInvoiceDetail = GetJobInvoiceDetailsList(invoiceID, jobInvoiceType, isInvoiced,
                        gridParam.SortCol, gridParam.SortDir);
                }

                if (TempData["lstJobInvoiceDetail"] != null && !lstJobInvoiceDetail.Any())
                {
                    lstJobInvoiceDetail = (IList<JobInvoiceDetail>)TempData["lstJobInvoiceDetail"];
                }

                //if (lstJobInvoiceDetail.Count > 0)
                //{
                //    JobInvoice jobInvoice = GetInvoice(Convert.ToInt32(invoiceID));
                //    bool IsFileExists = System.IO.File.Exists(System.IO.Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report\\" + jobInvoice.InvoiceNumber + ".pdf")) ? true : false;
                //    if (IsFileExists)
                //    {
                //        lstJobInvoiceDetail.ToList().ForEach(s => s.IsInvoiced = true);
                //    }
                //    gridParam.TotalDisplayRecords = lstJobInvoiceDetail.FirstOrDefault().TotalRecords;
                //    gridParam.TotalRecords = lstJobInvoiceDetail.FirstOrDefault().TotalRecords;
                //}

                HttpContext.Response.Write(Grid.PrepareDataSet(lstJobInvoiceDetail, gridParam));
            }
            catch (Exception ex)
            {
                FormBot.Helper.Log.WriteError(ex);
                HttpContext.Response.Write(Grid.PrepareDataSet(new List<JobInvoiceDetail>(), gridParam));
            }
        }

        /// <summary>
        /// GetSettingDescription
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <returns>return model</returns>
        [NonAction]
        public Entity.Settings.Settings GetSettingDescription(string invoiceID)
        {
            try
            {
                StringBuilder jobDetails = new StringBuilder();
                SettingsBAL settingsBAL = new SettingsBAL();
                Entity.Settings.Settings settings = new Entity.Settings.Settings();
                int? solarCompanyId = 0;
                if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                    solarCompanyId = null;
                else
                    solarCompanyId = ProjectSession.SolarCompanyId;

                settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, solarCompanyId, ProjectSession.ResellerId);

                return settings;

                //if (settings != null)
                //{
                //    var description = _jobInvoice.GetInvoiceExtraDescription(Convert.ToInt32(invoiceID),
                //        settings.IsJobAddress, settings.IsJobDate, settings.IsJobDescription,
                //        settings.IsTitle, settings.IsName);

                //    if (description != null)
                //    {
                //        if (!string.IsNullOrEmpty(Convert.ToString(description.Item1))
                //            && Convert.ToDateTime(description.Item1) != DateTime.MinValue)
                //            jobDetails.Append(Convert.ToDateTime(description.Item1).ToString("dd/MM/yyyy") + "\n\r");

                //        if (!string.IsNullOrEmpty(Convert.ToString(description.Item3)))
                //            jobDetails.Append(description.Item3 + "\n\r");

                //        if (!string.IsNullOrEmpty(Convert.ToString(description.Item2)))
                //            jobDetails.Append(description.Item2 + "\n\r");

                //        if (!string.IsNullOrEmpty(Convert.ToString(description.Item4)))
                //            jobDetails.Append(description.Item4 + "\n\r");
                //    }
                //}

                //return jobDetails.ToString();
            }
            catch (Exception err)
            {

            }
            return new Entity.Settings.Settings();
            //return "";
        }

        /// <summary>
        /// submitToZero
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        /// <param name="invoiceNumber">invoiceNumber</param>
        /// <param name="invoiceStatus">invoiceStatus</param>
        /// <param name="jobID">jobID</param>
        /// <param name="contactName">contactName</param>
        /// <param name="invoicePaidStatus">invoicePaidStatus</param>
        /// <returns>action result</returns>
        [HttpPost]
        public ActionResult submitToZero(string invoiceID, string invoiceNumber, string invoiceStatus,
            string jobID, string contactName, int invoicePaidStatus)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            xeroApiHelper = new ApplicationSettingsTest();

            //Task<XeroOAuth2Token> xeroToken = Task.Run(async () => await xeroApiHelper.CheckToken());
            //xeroToken.Wait();
            if (!TokenUtilities.TokenExists())
            {
                return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
            }
            var token = TokenUtilities.GetStoredToken();
            string accessToken = token.AccessToken;
            string xeroTenantId = token.Tenants[0].TenantId.ToString();
            if (token == null)
            {
                return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var api = new AccountingApi();
                AccountingApi accountingApi = new AccountingApi();
                try
                {

                    var jobInvoice = _jobInvoice.GetJobInvoiceById(Convert.ToInt32(invoiceID));
                    IList<JobInvoiceDetail> lstJobInvoiceDetail = GetJobInvoiceDetailsList(invoiceID);

                    if (jobInvoice.XeroDraftDate != null)
                        lstJobInvoiceDetail = lstJobInvoiceDetail.ToList();

                    var dueDate = DateTime.Now;
                    var accountCode = "200";
                    var paymrntAccountCode = "881";
                    decimal invoiceTax = 0;
                    var invoiceSetting = _jobInvoiceDetail.GetInvoiceSetting();
                    bool isTaxInclusive = false;
                    if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
                    {
                        dueDate = invoiceSetting.Item1;
                    }
                    if (invoiceSetting.Item2 != null && invoiceSetting.Item2 != "")
                    {
                        accountCode = invoiceSetting.Item2;
                    }
                    if (invoiceSetting.Item3 != null && invoiceSetting.Item3 != "")
                    {
                        paymrntAccountCode = invoiceSetting.Item3;
                    }
                    if (invoiceSetting.Item4 != null && invoiceSetting.Item4 != 0)
                    {
                        invoiceTax = invoiceSetting.Item4;
                    }
                    if (invoiceSetting.Item6 != null)
                    {
                        isTaxInclusive = invoiceSetting.Item6;
                    }

                    var paymentDate = jobInvoice.CreatedDate == null ? DateTime.Now.Date : jobInvoice.CreatedDate;
                    List<oauthModel.LineItem> lineItems = new List<oauthModel.LineItem>();
                    if (lstJobInvoiceDetail != null && lstJobInvoiceDetail.Any())
                    {
                        oauthModel.LineItem lineItem = null;
                        StringBuilder jobDetails = new StringBuilder();
                        Entity.Settings.Settings getSettings = new Entity.Settings.Settings();

                        getSettings = GetSettingDescription(invoiceID);

                        if (getSettings != null)
                        {
                            var description = _jobInvoice.GetInvoiceExtraDescription(Convert.ToInt32(invoiceID),
                                getSettings.IsJobAddress, getSettings.IsJobDate, getSettings.IsJobDescription,
                                getSettings.IsTitle, getSettings.IsName);

                            if (description != null)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(description.Item1))
                                    && Convert.ToDateTime(description.Item1) != DateTime.MinValue)
                                {
                                    lineItem = new oauthModel.LineItem
                                    {
                                        AccountCode = null,
                                        Description = Convert.ToDateTime(description.Item1).ToString("dd/MM/yyyy"),
                                    };
                                    lineItems.Add(lineItem);
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(description.Item3)))
                                {
                                    lineItem = new oauthModel.LineItem
                                    {
                                        AccountCode = null,
                                        Description = Convert.ToString(description.Item3),
                                    };
                                    lineItems.Add(lineItem);
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(description.Item2)))
                                {
                                    lineItem = new oauthModel.LineItem
                                    {
                                        AccountCode = null,
                                        Description = Convert.ToString(description.Item2),
                                    };
                                    lineItems.Add(lineItem);
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(description.Item4)))
                                {
                                    lineItem = new oauthModel.LineItem
                                    {
                                        AccountCode = null,
                                        Description = Convert.ToString(description.Item4),
                                    };
                                    lineItems.Add(lineItem);
                                }
                            }
                        }

                        IList<JobInvoiceDetail> lstJobInvoiceDetail2 = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3 && c.IsBillable == true).ToList();

                        for (int i = 0; i < lstJobInvoiceDetail2.Count(); i++)
                        {
                            if (lstJobInvoiceDetail2[i].IsXeroId != "0")
                            {
                                lineItem = new oauthModel.LineItem
                                {
                                    //mandatory
                                    AccountCode = !string.IsNullOrEmpty(lstJobInvoiceDetail2[i].SaleAccountCode) ? lstJobInvoiceDetail2[i].SaleAccountCode : ProjectSession.SaleAccountCode,
                                    Description = jobDetails + lstJobInvoiceDetail2[i].Description,
                                    UnitAmount = Convert.ToInt64(lstJobInvoiceDetail2[i].SubTotal / lstJobInvoiceDetail2[i].Quantity),
                                    Quantity = Convert.ToInt64(lstJobInvoiceDetail2[i].Quantity),
                                    ItemCode = Convert.ToString(lstJobInvoiceDetail2[i].ItemCode),
                                    TaxType = (lstJobInvoiceDetail2[i].TaxRateId != null && lstJobInvoiceDetail2[i].TaxRateId > 0) ? Convert.ToString((FormBot.Helper.SystemEnums.TaxType)lstJobInvoiceDetail2[i].TaxRateId) : null
                                    //optional
                                };
                            }
                            else
                            {

                                lineItem = new oauthModel.LineItem
                                {
                                    //mandatory
                                    AccountCode = !string.IsNullOrEmpty(lstJobInvoiceDetail2[i].SaleAccountCode) ? lstJobInvoiceDetail2[i].SaleAccountCode : ProjectSession.SaleAccountCode,
                                    Description = jobDetails + lstJobInvoiceDetail2[i].Description,
                                    UnitAmount = Convert.ToInt64(lstJobInvoiceDetail2[i].SubTotal / lstJobInvoiceDetail2[i].Quantity),
                                    Quantity = Convert.ToInt64(lstJobInvoiceDetail2[i].Quantity),
                                    TaxType = (lstJobInvoiceDetail2[i].TaxRateId != null && lstJobInvoiceDetail2[i].TaxRateId > 0) ? Convert.ToString((FormBot.Helper.SystemEnums.TaxType)lstJobInvoiceDetail2[i].TaxRateId) : null
                                    //optional
                                };
                            }

                            lineItems.Add(lineItem);
                        }

                    }
                    var invoices = (List<oauthModel.Invoice>)null;
                    List<oauthModel.Address> lstAddress = new List<oauthModel.Address>();
                    oauthModel.Address address = new oauthModel.Address();

                    DataSet dsAddress = _jobInvoiceDetail.GetInvoiceDetsilForReport(Convert.ToInt32(invoiceID), true, true, true, true, true);
                    if (dsAddress != null && dsAddress.Tables.Count > 0)
                    {
                        if (dsAddress.Tables[4] != null && dsAddress.Tables[4].Rows.Count > 0)
                        {
                            address.AddressLine1 = !string.IsNullOrEmpty(dsAddress.Tables[5].Rows[0]["FromAddressLine1"].ToString()) ? dsAddress.Tables[5].Rows[0]["FromAddressLine1"].ToString() : string.Empty;
                            address.AddressLine2 = !string.IsNullOrEmpty(dsAddress.Tables[5].Rows[0]["FromAddressLine2"].ToString()) ? dsAddress.Tables[5].Rows[0]["FromAddressLine2"].ToString() : string.Empty;
                            address.AddressLine3 = !string.IsNullOrEmpty(dsAddress.Tables[5].Rows[0]["FromAddressLine3"].ToString()) ? dsAddress.Tables[5].Rows[0]["FromAddressLine3"].ToString() : string.Empty;
                        }
                    }
                    lstAddress.Add(address);

                    if (string.IsNullOrEmpty(jobInvoice.JobInvoiceIDXero))
                    {
                        oauthModel.Invoices invoicesCreate = new oauthModel.Invoices();
                        List<oauthModel.Invoice> lstInv = new List<oauthModel.Invoice>();
                        oauthModel.Invoice inv = new oauthModel.Invoice();
                        //mandatory
                        inv.Type = oauthModel.Invoice.TypeEnum.ACCREC;
                        inv.Contact = new oauthModel.Contact { Name = contactName, Addresses = lstAddress };
                        inv.Date = paymentDate;
                        inv.DueDate = dueDate;
                        inv.ExpectedPaymentDate = paymentDate;
                        inv.LineAmountTypes = oauthModel.LineAmountTypes.Exclusive;
                        //optional
                        inv.Status = invoiceStatus == "1" ? oauthModel.Invoice.StatusEnum.DRAFT : oauthModel.Invoice.StatusEnum.AUTHORISED;
                        inv.InvoiceNumber = invoiceNumber;
                        inv.LineItems = lineItems;
                        lstInv.Add(inv);
                        invoicesCreate._Invoices = lstInv;
                        var invResponse = Task.Run(async () => await accountingApi.CreateInvoicesAsync(accessToken, xeroTenantId, invoicesCreate));
                        invResponse.Wait();
                        invoices = invResponse.Result._Invoices.ToList();

                    }
                    else
                    {
                        oauthModel.Invoices invoicesCreate = new oauthModel.Invoices();
                        List<oauthModel.Invoice> lstInv = new List<oauthModel.Invoice>();
                        oauthModel.Invoice inv = new oauthModel.Invoice();
                        //mandatory
                        inv.Type = oauthModel.Invoice.TypeEnum.ACCREC;
                        inv.Contact = new oauthModel.Contact { Name = contactName, Addresses = lstAddress };
                        inv.Date = paymentDate;
                        inv.DueDate = dueDate;
                        inv.ExpectedPaymentDate = paymentDate;
                        inv.LineAmountTypes = oauthModel.LineAmountTypes.Exclusive;
                        //optional
                        inv.Status = invoiceStatus == "1" ? oauthModel.Invoice.StatusEnum.DRAFT : oauthModel.Invoice.StatusEnum.AUTHORISED;
                        inv.InvoiceNumber = invoiceNumber;
                        inv.LineItems = lineItems;
                        var invResponse = Task.Run(async () => await accountingApi.UpdateInvoiceAsync(accessToken, xeroTenantId, new Guid(invoiceID), invoicesCreate));
                        invResponse.Wait();
                        invoices = invResponse.Result._Invoices.ToList();
                    }

                    if (!string.IsNullOrEmpty(invoiceID) && !string.IsNullOrEmpty(invoiceStatus) && invoices != null)
                    {
                        List<IEnumerable<oauthModel.Payment>> listCreatePayment = new List<IEnumerable<oauthModel.Payment>>();
                        List<IEnumerable<oauthModel.Payment>> listUpdatePayment = new List<IEnumerable<oauthModel.Payment>>();
                        List<IEnumerable<oauthModel.Payment>> listFinalPayment = new List<IEnumerable<oauthModel.Payment>>();

                        if (invoiceStatus == "2")
                        {
                            oauthModel.Payment objPayment = null;
                            IList<JobInvoiceDetail> lstJobInvoiceDetail2 = lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3 && c.XeroId == null).ToList();
                            for (int i = 0; i < lstJobInvoiceDetail2.Count(); i++)
                            {
                                objPayment = new oauthModel.Payment
                                {
                                    Invoice = new oauthModel.Invoice { InvoiceID = new Guid(invoices[0].InvoiceID.ToString()) },
                                    Account = new oauthModel.Account { Code = paymrntAccountCode },
                                    //Date = lstJobInvoiceDetail2[i].CreatedDate == null ? DateTime.Now : lstJobInvoiceDetail2[i].CreatedDate,
                                    Date = lstJobInvoiceDetail2[i].TimeStart == null ? DateTime.Now : Convert.ToDateTime(lstJobInvoiceDetail2[i].TimeStart),
                                    Amount = Convert.ToDouble(lstJobInvoiceDetail2[i].PaymentAmount)

                                };

                                var paymentApi = Task.Run(async () => await accountingApi.CreatePaymentAsync(accessToken, xeroTenantId, objPayment));
                                paymentApi.Wait();
                                listCreatePayment.Add(paymentApi.Result._Payments);
                            }
                            lstJobInvoiceDetail2.Clear();
                            lstJobInvoiceDetail2 = lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3 && c.XeroId != null).ToList();
                            for (int i = 0; i < lstJobInvoiceDetail2.Count(); i++)
                            {
                                objPayment = new oauthModel.Payment
                                {
                                    PaymentID = new Guid(lstJobInvoiceDetail[i].XeroId.ToString()),
                                    Invoice = new oauthModel.Invoice { InvoiceID = new Guid(invoices[0].InvoiceID.ToString()) },
                                    Account = new oauthModel.Account { Code = paymrntAccountCode },
                                    Date = lstJobInvoiceDetail2[i].TimeStart == null ? DateTime.Now : Convert.ToDateTime(lstJobInvoiceDetail2[i].TimeStart),
                                    Amount = Convert.ToDouble(lstJobInvoiceDetail2[i].PaymentAmount)

                                };
                                oauthModel.Payments payments = new oauthModel.Payments();
                                payments._Payments.Add(objPayment);
                                var paymentApi = Task.Run(async () => await accountingApi.DeletePaymentAsync(accessToken, xeroTenantId, objPayment.PaymentID.Value, payments));
                                paymentApi.Wait();
                                var paymentCreateApi = Task.Run(async () => await accountingApi.CreatePaymentAsync(accessToken, xeroTenantId, objPayment));
                                paymentApi.Wait();
                                listUpdatePayment.Add(paymentCreateApi.Result._Payments);
                            }

                        }

                        XmlDocument docConfig = new XmlDocument();
                        XmlNode xmlNode = docConfig.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                        XmlElement rootElement = docConfig.CreateElement("Invoice");
                        docConfig.AppendChild(rootElement);

                        IList<JobInvoiceDetail> lstJobInvoiceDetail3 = lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3).ToList();
                        for (int i = 0; i < lstJobInvoiceDetail3.Count(); i++)
                        {
                            XmlElement hedder = docConfig.CreateElement("InvoiceDetail");
                            docConfig.DocumentElement.PrependChild(hedder);
                            docConfig.ChildNodes.Item(0).AppendChild(hedder);

                            XmlElement invoiceDetailIDElement = docConfig.CreateElement("InvoiceDetailID");
                            XmlText text = docConfig.CreateTextNode(Convert.ToString(lstJobInvoiceDetail3[i].JobInvoiceDetailID));
                            invoiceDetailIDElement.AppendChild(text);
                            hedder.PrependChild(invoiceDetailIDElement);

                            XmlElement XeroIDElement = docConfig.CreateElement("XeroID");

                            string lineItemID = Convert.ToString(invoices.FirstOrDefault().LineItems.FirstOrDefault(y => y.Description == lstJobInvoiceDetail3[i].Description).LineItemID);
                            XmlText text2 = docConfig.CreateTextNode(lineItemID);
                            XeroIDElement.AppendChild(text2);
                            hedder.PrependChild(XeroIDElement);

                        }

                        lstJobInvoiceDetail3.Clear();
                        lstJobInvoiceDetail3 = lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3).ToList();
                        listFinalPayment = listFinalPayment.Concat(listCreatePayment).Concat(listUpdatePayment).ToList();
                        for (int i = 0; i < listFinalPayment.Count(); i++)
                        {
                            XmlElement hedder = docConfig.CreateElement("InvoiceDetail");
                            docConfig.DocumentElement.PrependChild(hedder);
                            docConfig.ChildNodes.Item(0).AppendChild(hedder);

                            XmlElement invoiceDetailIDElement = docConfig.CreateElement("InvoiceDetailID");
                            XmlText text = docConfig.CreateTextNode(Convert.ToString(lstJobInvoiceDetail3[i].JobInvoiceDetailID));
                            invoiceDetailIDElement.AppendChild(text);
                            hedder.PrependChild(invoiceDetailIDElement);

                            XmlElement XeroIDElement = docConfig.CreateElement("XeroID");
                            XmlText text2 = docConfig.CreateTextNode(Convert.ToString(listCreatePayment[i].FirstOrDefault().PaymentID));
                            XeroIDElement.AppendChild(text2);
                            hedder.PrependChild(XeroIDElement);
                        }

                        if (invoicePaidStatus != Convert.ToInt32(SystemEnums.InvoiceStatus.Cancelled))
                        {
                            decimal partPayment = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType != 3).Sum(c => (c.Quantity * c.Sale)));
                            decimal paidPayment = Convert.ToDecimal(lstJobInvoiceDetail.Where(c => c.JobInvoiceType == 3).Sum(c => (c.PaymentAmount)));
                            if (partPayment == paidPayment)
                            {
                                invoicePaidStatus = Convert.ToInt32(SystemEnums.InvoiceStatus.OutStanding);
                            }
                            else if (partPayment >= paidPayment)
                            {
                                invoicePaidStatus = Convert.ToInt32(SystemEnums.InvoiceStatus.PartialPayment);
                            }
                            else if ((partPayment - paidPayment) == 0)
                            {
                                invoicePaidStatus = Convert.ToInt32(SystemEnums.InvoiceStatus.PaidFull);
                            }
                        }
                        _jobInvoice.UpdateInvoiceXeroDetail(invoiceStatus, Convert.ToInt32(invoiceID),
                            invoices[0].InvoiceID.ToString(), docConfig.InnerXml, invoicePaidStatus, invoiceNumber);
                        var jsonSample = downlaodPDF(invoices[0].InvoiceID.ToString(), jobID, invoiceNumber);
                    }
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                catch (RenewTokenException e)
                {
                    return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
                }
                catch (UnauthorizedException e)
                {
                    return Json(new { status = false, error = "UnauthorizedException" }, JsonRequestBehavior.AllowGet);
                }
                catch (XeroApiException e)
                {
                    int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                    string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

                    return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    //Response.Write(e.Message);
                    return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        public ActionResult downlaodPDF(string xeroInvoiceID, string jobID, string invoiceID)
        {
            //if (XeroApiHelper.xeroApiHelperSession == null)
            //{
            //    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            // var api = XeroApiHelper.xeroApiHelperSession.CoreApi();
            try
            {
                var xeroApiHelper = new ApplicationSettingsTest();
                Task<XeroOAuth2Token> xeroToken = Task.Run(async () => await xeroApiHelper.CheckToken());
                xeroToken.Wait();

                if (xeroToken.Result != null)
                {
                    var api = new AccountingApi();
                    string accessToken = xeroToken.Result.AccessToken;
                    string xeroTenantId = xeroToken.Result.Tenants[0].TenantId.ToString();

                    AccountingApi accountingApi = new AccountingApi();
                    var id = new Guid(xeroInvoiceID);
                    var result = Task.Run(async () => await accountingApi.GetInvoiceAsPdfAsync(accessToken, xeroTenantId, id));
                    result.Wait();
                    MemoryStream ms = new MemoryStream();
                    var pdf = result.Result;
                    pdf.CopyTo(ms);
                    byte[] bytes = ms.ToArray();
                    //var pdf = api.PdfFiles.Get(PdfEndpointType.Invoices, id);
                    var expected = invoiceID + ".pdf";
                    ByteArrayToFile(expected, bytes, jobID);

                }
                else
                {
                    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);

            // }
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray, string jobID)
        {
            try
            {
                string physicalPath = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;

                if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(physicalPath, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        #endregion

        #region create reports

        /// <summary>
        /// Create Report for job invoice
        /// </summary>
        /// <param name="Filename">Filename</param>
        /// <param name="ExportType">ExportType</param>
        /// <param name="jobInvoiceID">jobInvoiceID</param>
        /// <param name="jobID">jobID</param>
        /// <returns>Action Result</returns>
        public ActionResult CreateReport(string Filename, string ExportType, int jobInvoiceID, string jobID)
        {
            JobInvoiceDetail jobInvoiceDetail = new JobInvoiceDetail();
            clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
            try
            {
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                ReportViewer viewer = new ReportViewer();
                XmlDocument oXD = new XmlDocument();
                //rdlc path.
                //oXD.Load("D://Projects//FormBot//SourceCode//FormBot.Main//Reports//Invoice1.rdlc");
                oXD.Load(Server.MapPath("/Reports/Invoice1.rdlc"));

                DataSet ds = new DataSet();
                int report = 1;
                string RefNumber = string.Empty;
                string CompanyABN = string.Empty;
                string InoviceDate = string.Empty;
                string InvoiceNumber = string.Empty;
                string AmountDue = string.Empty;
                string Total = string.Empty;
                string DueDate = string.Empty;
                string ToAddressLine1 = string.Empty;
                string ToAddressLine2 = string.Empty;
                string ToAddressLine3 = string.Empty;
                string FromAddressLine1 = string.Empty;
                string FromAddressLine2 = string.Empty;
                string FromAddressLine3 = string.Empty;
                string LogoPath = string.Empty;
                string InvoiceFooter = string.Empty;
                string JobDescription = string.Empty;
                string JobDate = string.Empty;
                string JobAddress = string.Empty;
                string JobTitle = string.Empty;
                string Logo = string.Empty;
                string ItemCode = string.Empty;
                string ToName = string.Empty;
                string FromName = string.Empty;
                string FromCompanyName = string.Empty;
                string ToCompanyName = string.Empty;
                bool IsJobDescription = false;
                bool IsJobAddress = false;
                bool IsJobDate = false;
                bool IsTitle = false;
                bool IsName = false;
                int SettingUserId = 0;
                string IsStcInvoice = "1";
                //sp
                Entity.Settings.Settings settings = new Entity.Settings.Settings();
                settings = GetSettingsData();
                Logo = settings.Logo;
                InvoiceFooter = settings.InvoiceFooter;
                IsJobDescription = settings.IsJobDescription;
                IsJobAddress = settings.IsJobAddress;
                IsJobDate = settings.IsJobDate;
                IsTitle = settings.IsTitle;
                IsName = settings.IsName;
                SettingUserId = settings.UserId;
                //SettingUserId = 544;
                //Logo = "gogreen.jpg";

                var invoiceSetting = _jobInvoiceDetail.GetInvoiceSetting();
                if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
                {
                    DueDate = invoiceSetting.Item1.ToString("dd MMMM yyyy");
                }

                if (Logo != "" && Logo != null)
                {
                    var LogoP = Path.Combine(ProjectSession.UploadedDocumentPath + "UserDocuments" + "\\" + SettingUserId, Logo);
                    LogoPath = new Uri(LogoP).AbsoluteUri;
                }
                else
                {
                    LogoPath = "";
                }
                ds = _jobInvoiceDetail.GetInvoiceDetsilForReport(jobInvoiceID, IsJobAddress, IsJobDate, IsJobDescription, IsTitle, IsName);
                DataTable dt = new DataTable();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Count >= 1)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            RefNumber = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["RefNumber"].ToString()) ? ds.Tables[0].Rows[0]["RefNumber"].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 2)
                    {
                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            CompanyABN = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABN"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABN"].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 3)
                    {
                        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        {
                            //InoviceDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["InoviceDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["InoviceDate"]).ToString("dd MMMM yyyy") : string.Empty;
                            InoviceDate = Convert.ToDateTime(DateTime.Now).ToString("dd MMMM yyyy");
                            InvoiceNumber = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["InvoiceNumber"].ToString()) ? ds.Tables[2].Rows[0]["InvoiceNumber"].ToString() : string.Empty;
                            //DueDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["DueDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["DueDate"]).ToString("dd MMMM yyyy") : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 4)
                    {
                        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                        {
                            dt = ds.Tables[3];
                        }
                    }
                    if (ds.Tables.Count >= 5)
                    {
                        if (ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
                        {
                            ToAddressLine1 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine1"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
                            ToAddressLine2 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine2"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
                            ToAddressLine3 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine3"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
                            ToName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToName"].ToString()) ? ds.Tables[4].Rows[0]["ToName"].ToString() : string.Empty;
                            ToCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToCompanyName"].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 6)
                    {
                        if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
                        {
                            FromAddressLine1 = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["FromAddressLine1"].ToString()) ? ds.Tables[5].Rows[0]["FromAddressLine1"].ToString() : string.Empty;
                            FromAddressLine2 = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["FromAddressLine2"].ToString()) ? ds.Tables[5].Rows[0]["FromAddressLine2"].ToString() : string.Empty;
                            FromAddressLine3 = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["FromAddressLine3"].ToString()) ? ds.Tables[5].Rows[0]["FromAddressLine3"].ToString() : string.Empty;
                            FromName = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["FromName"].ToString()) ? ds.Tables[5].Rows[0]["FromName"].ToString() : string.Empty;
                            FromCompanyName = !string.IsNullOrEmpty(ds.Tables[5].Rows[0]["FromCompanyname"].ToString()) ? ds.Tables[5].Rows[0]["FromCompanyname"].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 7)
                    {
                        if (ds.Tables[6] != null && ds.Tables[6].Rows.Count > 0)
                        {
                            JobDate = !string.IsNullOrEmpty(ds.Tables[6].Rows[0][0].ToString()) ? Convert.ToDateTime(ds.Tables[6].Rows[0][0]).ToString("dd MMMM yyyy") : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 8)
                    {
                        if (ds.Tables[7] != null && ds.Tables[7].Rows.Count > 0)
                        {
                            JobDescription = !string.IsNullOrEmpty(ds.Tables[7].Rows[0][0].ToString()) ? ds.Tables[7].Rows[0][0].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 9)
                    {
                        if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
                        {
                            JobTitle = !string.IsNullOrEmpty(ds.Tables[8].Rows[0][0].ToString()) ? ds.Tables[8].Rows[0][0].ToString() : string.Empty;
                        }
                    }
                    if (ds.Tables.Count >= 10)
                    {
                        if (ds.Tables[9] != null && ds.Tables[9].Rows.Count > 0)
                        {
                            JobAddress = !string.IsNullOrEmpty(ds.Tables[9].Rows[0][0].ToString()) ? ds.Tables[9].Rows[0][0].ToString() : string.Empty;
                        }
                    }
                    viewer.LocalReport.ReportPath = @"Reports//Invoice1.rdlc";
                    viewer.LocalReport.EnableExternalImages = true;
                    // LocalReport.EnableExternalImages = true;
                    ReportDataSource rds1 = new ReportDataSource("dt", dt);
                    viewer.LocalReport.DataSources.Add(rds1);

                    viewer.LocalReport.SetParameters(new ReportParameter("RefNumber", RefNumber));
                    viewer.LocalReport.SetParameters(new ReportParameter("CompanyABN", CompanyABN));
                    viewer.LocalReport.SetParameters(new ReportParameter("InoviceDate", InoviceDate));
                    viewer.LocalReport.SetParameters(new ReportParameter("InvoiceNumber", InvoiceNumber));
                    viewer.LocalReport.SetParameters(new ReportParameter("AmountDue", AmountDue));
                    viewer.LocalReport.SetParameters(new ReportParameter("Total", Total));
                    viewer.LocalReport.SetParameters(new ReportParameter("DueDate", DueDate));
                    viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
                    viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
                    viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine1", FromAddressLine1));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine2", FromAddressLine2));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine3", FromAddressLine3));
                    viewer.LocalReport.SetParameters(new ReportParameter("JobDate", JobDate));
                    viewer.LocalReport.SetParameters(new ReportParameter("JobDescription", JobDescription));
                    viewer.LocalReport.SetParameters(new ReportParameter("JobTitle", JobTitle));
                    viewer.LocalReport.SetParameters(new ReportParameter("JobAddress", JobAddress));
                    viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));
                    viewer.LocalReport.SetParameters(new ReportParameter("InvoiceFooter", InvoiceFooter));
                    viewer.LocalReport.SetParameters(new ReportParameter("ToName", ToName));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromName", FromName));
                    viewer.LocalReport.SetParameters(new ReportParameter("IsStcInvoice", IsStcInvoice));
                    viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
                    viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));
                    viewer.LocalReport.Refresh();

                    byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    //Save Report
                    Filename = InvoiceNumber;
                    ByteArrayToFile(Convert.ToString(InvoiceNumber + ".pdf"), bytes, jobID);
                    _jobInvoice.UpdateIsInvoicedByInvoiceID(jobInvoiceID);

                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                    JsonResponseObj.status = "Completed";
                    JsonResponseObj.strErrors = "<ul><li>Invoice generated successfully</li></ul>";

                    //return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, Filename); 
                    // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                    //Response.Buffer = true;
                    //Response.Clear();
                    //Response.ClearHeaders();
                    //Response.ContentType = mimeType;
                    //Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "." + extension);
                    //Response.BinaryWrite(bytes); // create the file
                    //Response.End(); // send it to the client to download
                }
            }
            catch (Exception ex)
            {
                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
            return Json(JsonResponseObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create csv for job invoice.
        /// </summary>
        /// <param name="jobInvoiceID">jobInvoiceID</param>
        public void CreateCSV(int jobInvoiceID)
        {
            string DueDate = string.Empty;
            var invoiceSetting = _jobInvoiceDetail.GetInvoiceSetting();
            if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
            {
                DueDate = invoiceSetting.Item1.ToString("dd MMMM yyyy");
            }

            bool IsTaxInclusive = false;
            Entity.Settings.Settings settings = new Entity.Settings.Settings();
            settings = GetSettingsData();
            IsTaxInclusive = settings.IsTaxInclusive;
            DataSet ds = _jobInvoiceDetail.GetInvoiceAllDetailForCSV(jobInvoiceID, IsTaxInclusive, DueDate);
            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }

            //Build the CSV file data as a Comma separated string.
            string csv = string.Empty;

            foreach (DataColumn column in dt.Columns)
            {
                //Add the Header row for CSV file.
                csv += column.ColumnName + ',';
            }

            //Add new line.
            csv += "\r\n";

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //Add the Data rows.
                    csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
                }

                //Add new line.
                csv += "\r\n";
            }

            //Download the CSV file.
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=SalesInvoice.csv");
            //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv);
            Response.Flush();
            Response.End();
        }

        #endregion
    }
}

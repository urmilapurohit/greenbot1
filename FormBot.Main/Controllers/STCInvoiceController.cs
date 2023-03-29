using FormBot.Helper;
using FormBot.Entity;
using FormBot.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL.Service;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Example.Applications.Public;
using FormBot.Main.Helpers;
using System.Data;
using FormBot.BAL.Service.Job;
using xero = Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;
using Xero.Api.Infrastructure.Exceptions;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Xml;
using FormBot.Main.Infrastructure;
using System.Text;
using Ionic.Zip;
using GenericParsing;
using FormBot.Entity.Email;
using FormBot.Email;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.EnterpriseServices;
using System.Reflection;
using Xero.Api.Core.Endpoints;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using FormBot.Helper.Helper;
using FormBot.BAL.Service.CommonRules;
using FileHelpers.Options;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using FormBot.Entity.KendoGrid;
using FormBot.Entity.Job;
using LumenWorks.Framework.IO.Csv;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using oauthModel = Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Model;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Globalization;
using FormBot.BAL.Service.RECRegistry;

namespace FormBot.Main.Controllers
{
    public class STCInvoiceController : Controller
    {
        #region Properties
        private readonly ISTCInvoiceBAL _stcInvoiceServiceBAL;
        //private ApiUser _user;
        //private IMvcAuthenticator _authenticator;
        private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
        private readonly IResellerBAL _reseller;
        private readonly ICreateJobBAL _createJob;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly IGenerateStcReportBAL _generateStcReportBAL;
        private readonly ILogger _log;
        private ApplicationSettingsTest xeroApiHelper;
        private readonly ICreateJobHistoryBAL _jobHistory;
        //CommonRECMethodsBAL commonRECMethodsBAL = null;
        private readonly IJobRulesBAL _jobRules;
        private readonly ICommonBulkUploadToCERBAL _commonBulkUploadToCER;
        #endregion

        #region Constructor

        public STCInvoiceController(ISTCInvoiceBAL stcInvoice, IJobInvoiceDetailBAL jobInvoiceDetail, IResellerBAL reseller, ICreateJobBAL createJob, IUserBAL userBAL, IEmailBAL emailBAL, IGenerateStcReportBAL generateStcReportBAL, ICreateJobHistoryBAL jobHistory, ILogger log, IJobRulesBAL jobRules, ICommonBulkUploadToCERBAL commonBulkUploadToCER)
        {
            this._stcInvoiceServiceBAL = stcInvoice;
            //_user = XeroApiHelper.User();
            //_authenticator = XeroApiHelper.MvcAuthenticator();
            this._jobInvoiceDetail = jobInvoiceDetail;
            this._reseller = reseller;
            this._createJob = createJob;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
            this._generateStcReportBAL = generateStcReportBAL;
            this._jobHistory = jobHistory;
            this._log = log;
            this._jobRules = jobRules;
            this._commonBulkUploadToCER = commonBulkUploadToCER;
            //commonRECMethodsBAL = new CommonRECMethodsBAL();
        }

        //public STCInvoiceController(ISTCInvoiceBAL stcInvoice, ICreateJobBAL createJob)
        //{
        //    this._stcInvoiceServiceBAL = stcInvoice;
        //    this._createJob = createJob;
        //    commonRECMethodsBAL = new CommonRECMethodsBAL();
        //}

        #endregion

        #region Events

        /// <summary>
        /// Get STC Invoice Kendo Grid Page
        /// </summary>
        /// <param name="IsOldStcInvoicePage"></param>
        /// <returns></returns>
        [UserAuthorization]
        public ActionResult Index()
        {
            STCInvoice stcInvoice = new STCInvoice();
            stcInvoice.UserTypeID = ProjectSession.UserTypeId;
            if (ProjectSession.UserTypeId == 8)
            {
                stcInvoice.lstSTCInvoiceStages = _stcInvoiceServiceBAL.GetSTCInvoiceStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 0, 0, ProjectSession.SolarCompanyId);
            }
            else
            {
                stcInvoice.lstSTCInvoiceStages = _stcInvoiceServiceBAL.GetSTCInvoiceStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 0, 0, 0);
            }

            Type enumType = typeof(FormBot.Helper.SystemEnums.STCSettlementTerm);
            Type descriptionAttributeType = typeof(System.ComponentModel.DescriptionAttribute);
            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (string memberName in Enum.GetNames(enumType))
            {
                MemberInfo member = enumType.GetMember(memberName).Single();

                string memberDescription = ((System.ComponentModel.DescriptionAttribute)Attribute.GetCustomAttribute(member, descriptionAttributeType)).Description;
                dict.Add((int)Enum.Parse(typeof(FormBot.Helper.SystemEnums.STCSettlementTerm), memberName), memberDescription);

            }
            stcInvoice.DictSettlementTerm = dict;

            // get the previous url and store it in tempdata (It's value is used inside /Authorization/Callback method to redirect to views based on visited page)
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.Url;
            TempData.Keep("PreviousUrl");

            return View("Index", stcInvoice);
        }
        #endregion

        #region Methods

        /// <summary>
        /// GetSTCInvoiceList For Selected Invoice
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="reseller"></param>
        /// <param name="ramid"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="invoicenumber"></param>
        /// <param name="RefJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionfromdate"></param>
        /// <param name="submissiontodate"></param>
        /// <param name="settlementfromdate"></param>
        /// <param name="settlementtodate"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="iscreditnotes"></param>
        /// <param name="issentinvoice"></param>
        /// <param name="isunsentinvoice"></param>
        /// <param name="SettlementTermId"></param>
        public void GetSTCInvoiceList(string stageid, string reseller = "", string ramid = "", string solarcompanyid = "", string invoicenumber = "", string RefJobId = "", string ownername = "", string installationaddress = "", string submissionfromdate = "", string submissiontodate = "", string settlementfromdate = "", string settlementtodate = "", bool isstcinvoice = true, bool iscreditnotes = true, bool issentinvoice = true, bool isunsentinvoice = true, string SettlementTermId = "", string isAllScaJobView = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            dynamic objlstSTCInvoice = ExportStcInvoice(stageid, reseller, ramid, solarcompanyid, invoicenumber, RefJobId, ownername, installationaddress, submissionfromdate, submissiontodate, settlementfromdate, settlementtodate, isstcinvoice, iscreditnotes, issentinvoice, isunsentinvoice, SettlementTermId, false, gridParam, isAllScaJobView);
            decimal TotalAmount = 0;
            IList<STCInvoice> lstSTCInvoice = objlstSTCInvoice;
            lstSTCInvoice.Where(a => a.SettlementTerms == "10").Select(a => a.CustomTermLabel = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)a.CustomSettlementTerm, "")).ToList();

            if (lstSTCInvoice.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstSTCInvoice.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstSTCInvoice.FirstOrDefault().TotalRecords;
                gridParam.TotalAmount = TotalAmount;
            }
            HttpContext.Response.Write(Grid.PrepareDataSet(lstSTCInvoice, gridParam));
        }

        /// <summary>
        /// Get All STC Invoice
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="resellerId"></param>
        /// <param name="ramid"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="invoicenumber"></param>
        /// <param name="RefJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionfromdate"></param>
        /// <param name="submissiontodate"></param>
        /// <param name="settlementfromdate"></param>
        /// <param name="settlementtodate"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="iscreditnotes"></param>
        /// <param name="issentinvoice"></param>
        /// <param name="isunsentinvoice"></param>
        /// <param name="SettlementTermId"></param>

        public void ExportAllCSV(string stageid, string resellerId = "", string ramid = "", string solarcompanyid = "", string invoicenumber = "", string RefJobId = "", string ownername = "", string installationaddress = "", string submissionfromdate = "", string submissiontodate = "", string settlementfromdate = "", string settlementtodate = "", bool isstcinvoice = true, bool iscreditnotes = true, bool issentinvoice = true, bool isunsentinvoice = true, string SettlementTermId = "")
        {
            dynamic objlstSTCInvoice = ExportStcInvoice(stageid, resellerId, ramid, solarcompanyid, invoicenumber, RefJobId, ownername, installationaddress, submissionfromdate, submissiontodate, settlementfromdate, settlementtodate, isstcinvoice, iscreditnotes, issentinvoice, isunsentinvoice, SettlementTermId, true, null);

        }

        /// <summary>
        /// Export STC invoice 
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="reseller"></param>
        /// <param name="ramid"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="invoicenumber"></param>
        /// <param name="RefJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionfromdate"></param>
        /// <param name="submissiontodate"></param>
        /// <param name="settlementfromdate"></param>
        /// <param name="settlementtodate"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="iscreditnotes"></param>
        /// <param name="issentinvoice"></param>
        /// <param name="isunsentinvoice"></param>
        /// <param name="SettlementTermId"></param>
        /// <param name="IsAllExportCSv"></param>
        /// <param name="gridParam"></param>
        /// <returns></returns>
        public dynamic ExportStcInvoice(string stageid, string reseller = "", string ramid = "", string solarcompanyid = "", string invoicenumber = "", string RefJobId = "", string ownername = "", string installationaddress = "", string submissionfromdate = "", string submissiontodate = "", string settlementfromdate = "", string settlementtodate = "", bool isstcinvoice = true, bool iscreditnotes = true, bool issentinvoice = true, bool isunsentinvoice = true, string SettlementTermId = "", bool IsAllExportCSv = false, GridParam gridParam = null, string isAllScaJobView = "")
        {
            int pageNumber = 0;
            if (!IsAllExportCSv)
            {
                pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            }

            int StageId = Convert.ToInt32(stageid);
            int ResellerId = 0;
            int RamId = 0;
            int SolarCompanyId = 0;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
                RamId = !string.IsNullOrEmpty(ramid) ? Convert.ToInt32(ramid) : 0;
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }
            else if (ProjectSession.UserTypeId == 2)
            {
                ResellerId = ProjectSession.ResellerId;
                RamId = !string.IsNullOrEmpty(ramid) ? Convert.ToInt32(ramid) : 0;
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }
            else if (ProjectSession.UserTypeId == 5)
            {
                ResellerId = ProjectSession.ResellerId;
                RamId = ProjectSession.LoggedInUserId;
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }
            else
            {
                SolarCompanyId = ProjectSession.SolarCompanyId;
            }

            DateTime? SubmissionFromDate = null, SubmissionToDate = null, SettlementFromDate = null, SettlementToDate = null;
            if (!string.IsNullOrEmpty(submissionfromdate) && !string.IsNullOrEmpty(submissiontodate))
            {
                SubmissionFromDate = Convert.ToDateTime(submissionfromdate);
                SubmissionToDate = Convert.ToDateTime(submissiontodate);
            }
            if (!string.IsNullOrEmpty(settlementfromdate) && !string.IsNullOrEmpty(settlementtodate))
            {
                SettlementFromDate = Convert.ToDateTime(settlementfromdate);
                SettlementToDate = Convert.ToDateTime(settlementtodate);
            }

            int sTermId = !string.IsNullOrEmpty(SettlementTermId) ? Convert.ToInt32(SettlementTermId) : 0;
            dynamic objlstSTCInvoice;
            if (IsAllExportCSv)
            {
                objlstSTCInvoice = _stcInvoiceServiceBAL.GetSTCInvoiceList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 1, 10, "", "", StageId, ResellerId, RamId, SolarCompanyId, invoicenumber, RefJobId, ownername, installationaddress, SubmissionFromDate, SubmissionToDate, SettlementFromDate, SettlementToDate, isstcinvoice, iscreditnotes, issentinvoice, isunsentinvoice, sTermId, IsAllExportCSv);
                IList<Int64> allInvoiceId = objlstSTCInvoice;
                GenerateCSVForSelectedInvoice(Convert.ToString(ResellerId), string.Join(",", allInvoiceId.Select(m => m.ToString()).ToArray()));
            }
            else
            {
                objlstSTCInvoice = _stcInvoiceServiceBAL.GetSTCInvoiceList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, StageId, ResellerId, RamId, SolarCompanyId, invoicenumber, RefJobId, ownername, installationaddress, SubmissionFromDate, SubmissionToDate, SettlementFromDate, SettlementToDate, isstcinvoice, iscreditnotes, issentinvoice, isunsentinvoice, sTermId, IsAllExportCSv, isAllScaJobView);
            }
            return objlstSTCInvoice;
        }

        /// <summary>
        /// Get STCInvoiceStageCount
        /// </summary>
        /// <param name="reseller"></param>
        /// <param name="ram"></param>
        /// <param name="sId"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult GetSTCInvoiceStageCount(string reseller, string ram, string sId, string isAllScaJobView = "")
        {
            int ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            int RamId = !string.IsNullOrEmpty(ram) ? Convert.ToInt32(ram) : 0;
            int SolarCompanyId = !string.IsNullOrEmpty(sId) ? Convert.ToInt32(sId) : 0;

            //try
            //{
            var lstSTCInvoiceStagesCount = _stcInvoiceServiceBAL.GetSTCInvoiceStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResellerId, RamId, SolarCompanyId, isAllScaJobView);
            return this.Json(new { lstSTCInvoiceStagesCount, success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Generate STCInvoiceForSelectedJobs
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="jobs"></param>
        /// <param name="isstcinvoice"></param>
        /// <param name="solarCompanyId"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult> GenerateSTCInvoiceForSelectedJobs(string resellerId, string jobs, string isstcinvoice, string solarCompanyId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            //try
            //{
            int IsSTCInvoice = Convert.ToInt32(isstcinvoice);
            //if (IsSTCInvoice == 0 && (XeroApiHelper.xeroApiHelperSession == null))
            //{
            //    return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
            //}
            var result = await SaveSTCInvoice(resellerId, jobs, IsSTCInvoice, solarCompanyId, InoviceTermID: 1);
            return this.Json(new { success = result });
            //}
            //catch (RenewTokenException e)
            //{
            //    return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            //}
            //catch (XeroApiException e)
            //{
            //    int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
            //    string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));

            //    return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Save STC Invoice
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="jobs"></param>
        /// <param name="IsSTCInvoice"></param>
        /// <param name="solarCompanyId"></param>
        /// <returns>bool</returns>
        public async Task<bool> SaveSTCInvoice(string resellerId, string jobs, int IsSTCInvoice = 1, string solarCompanyId = "", int UserTypeId = 0, int userId = 0, bool IsBackgroundRecProcess = false, int InoviceTermID = 1)
        {
            //try
            //{
            string Days = string.Empty;
            DateTime? STCSettlementDateForInvoiceSTC = Common.GetSettlementDate(1, ref Days);

            int ResellerId = 0;

            if ((UserTypeId == 1 || UserTypeId == 3) || (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3))
            {
                ResellerId = Convert.ToInt32(resellerId);
            }
            else
            {
                ResellerId = ProjectSession.ResellerId;
            }

            string[] sID = jobs.Split(',');

            string stcjobids = string.Empty;
            if (sID != null && sID.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("STCJobDetailsID", typeof(int));
                dt.Columns.Add("STCInvoiceNumber", typeof(string));
                dt.Columns.Add("STCInvoiceFilePath", typeof(string));

                //int invNumber = GetInvoiceNumberByResellerWise(ResellerId) + 1;

                foreach (string str in sID)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        DataRow dr = dt.NewRow();
                        dr["STCJobDetailsID"] = str.Split('_')[0];
                        dr["STCInvoiceNumber"] = string.Empty;
                        dr["STCInvoiceFilePath"] = string.Empty;
                        dt.Rows.Add(dr);
                        stcjobids = stcjobids + str.Split('_')[0] + ",";

                    }
                }
                stcjobids = stcjobids.Remove(stcjobids.Length - 1);

                if (dt.Rows.Count > 0)
                {
                    DataSet dsSTCInvoice = _stcInvoiceServiceBAL.GenerateSTCInvoiceForSelectedJobs(UserTypeId == 0 ? ProjectSession.LoggedInUserId : userId, UserTypeId == 0 ? ProjectSession.UserTypeId : UserTypeId, ResellerId, IsSTCInvoice, dt, STCSettlementDateForInvoiceSTC);
                    DataTable dtSTCInvoice = dsSTCInvoice != null ? dsSTCInvoice.Tables[0] : new DataTable();
                    List<DataRow> stcJobIds = dtSTCInvoice.AsEnumerable().ToList();
                    if (dtSTCInvoice != null && dtSTCInvoice.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtSTCInvoice.Rows.Count; i++)
                        {
                            _generateStcReportBAL.CreateStcReportNew("FormbotSTCReport", "Pdf", Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"]), Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"]), solarCompanyId, "4", UserTypeId == 0 ? ProjectSession.LoggedInUserId : userId, UserTypeId == 0 ? ProjectSession.ResellerId : ResellerId, false, IsBackgroundRecProcess: IsBackgroundRecProcess);
                            Common.Log("saveSTCInvoice: sID[" + i + "]: " + sID[i]);
                            //await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"]), 0);
                            Common.Log("saveSTCInvoice after jobid set: sID[" + i + "]: " + sID[i] + "jobid from db: " + dtSTCInvoice.Rows[i]["JobId"].ToString());
                            STCInvoice objSTCInvoice = new STCInvoice();
                            //objSTCInvoice.JobID = Convert.ToInt32(sID[i].Split('_')[2]);
                            objSTCInvoice.JobID = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["JobId"].ToString()) ? Convert.ToInt32(dtSTCInvoice.Rows[i]["JobId"].ToString()) : 0;
                            objSTCInvoice.InoviceTermID = InoviceTermID;
                            objSTCInvoice.STCInvoiceNumber = Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"]);
                            objSTCInvoice.UserName = ProjectSession.LoggedInName;
                            Common.Log("saveSTCInvoice after set object stc invoice:+IsSTCInvoice:"+IsSTCInvoice.ToString());
                            if (IsSTCInvoice == 1)
                            {
                                //_jobHistory.LogJobHistory(objSTCInvoice, HistoryCategory.SendToSTCInovice);
                                string JobHistoryMessage = "has been created invoice for JobID " + objSTCInvoice.JobID + " and STCInvoiceNumber is " + objSTCInvoice.STCInvoiceNumber;
                                if (objSTCInvoice.InoviceTermID == 1)
                                {
                                    JobHistoryMessage += " (Generated from STC Inoive Creation for jobs)";
                                }
                                else if (objSTCInvoice.InoviceTermID == 2)
                                {
                                    JobHistoryMessage += " (Generated from Inserted Entry In REC)";
                                }
                                else if (objSTCInvoice.InoviceTermID == 3)
                                {
                                    JobHistoryMessage += " (Generated from Updated PVD code from REC)";
                                }
                                else
                                {
                                    JobHistoryMessage += " (Generated from Imported Bulk Upload ID From REC)";
                                }

                                Common.SaveJobHistorytoXML(objSTCInvoice.JobID, JobHistoryMessage, "Invoicing", "SendToSTCInovice", ProjectSession.LoggedInName, false);
                                Common.Log("after saveJobHistory in stc invoice");
                                Common.Log("saveSTCInvoice after jobid set: sID[" + i + "]: " + sID[i] + "jobid from db: " + dtSTCInvoice.Rows[i]["JobId"].ToString());
                            }
                            else
                            {
                                //_jobHistory.LogJobHistory(objSTCInvoice, HistoryCategory.SendCreditNotesToXero);
                                string JobHistoryMessage = "has been sent credit note " + objSTCInvoice.STCInvoiceNumber + " successfully with JobID " + objSTCInvoice.JobID + " for adjustment in Xero " + objSTCInvoice.SolarCompany;
                                Common.SaveJobHistorytoXML(objSTCInvoice.JobID, JobHistoryMessage, "Invoicing", "SendCreditNotesToXero", ProjectSession.LoggedInName, false);
                                Common.Log("after saveJobHistory from else in stc invoice credit note");
                            }
                        }
                        DataTable countInvoice = _createJob.GetInvoiceCount(stcjobids);
                        Common.Log("CountInvoice: "+countInvoice.Rows.Count.ToString());
                        // DataTable dtSTCDetails = _createJob.GetSTCDetailsAndJobDataForCache(stcjobids,null);
                        if (countInvoice.Rows.Count > 0)
                        {
                            Common.Log("inside count invoice row count");
                            for (int j = 0; j < countInvoice.Rows.Count; j++)
                            {
                                SortedList<string, string> data = new SortedList<string, string>();
                                string IsInvoiced = countInvoice.Rows[j]["IsInvoiced"].ToString();
                                string IsCreditNote = countInvoice.Rows[j]["IsCreditNote"].ToString();
                                string STCInvoiceCount = countInvoice.Rows[j]["STCInvoiceCount"].ToString();
                                string IsPartialValidForSTCInvoice = countInvoice.Rows[j]["IsPartialValidForSTCInvoice"].ToString();
                                //string stcPrice = dtSTCDetails.Rows[j]["STCPrice"].ToString();
                                //string stcSettlementDate = dtSTCDetails.Rows[j]["STCSettlementDate"].ToString();
                                data.Add("IsInvoiced", IsInvoiced);
                                data.Add("IsCreditNote", IsCreditNote);
                                data.Add("STCInvoiceCount", STCInvoiceCount);
                                data.Add("IsPartialValidForSTCInvoice", IsPartialValidForSTCInvoice);
                                await CommonBAL.SetCacheDataForSTCSubmission(null, Convert.ToInt32(countInvoice.Rows[j]["jobid"]), data, true);

                                data = new SortedList<string, string>();
                                data.Add("STCInvoiceCnt", STCInvoiceCount);
                                data.Add("PaymentDate", Convert.ToString(countInvoice.Rows[j]["STCInvoiceCreatedDate"]));
                                data.Add("IsInvoiced", Convert.ToString(countInvoice.Rows[j]["InvoiceStatus"]));
                                await CommonBAL.SetCacheDataForPeakPay(null, Convert.ToInt32(countInvoice.Rows[j]["jobid"]), data);
                            }

                        }
                    }

                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    //CreateStcReport("FormbotSTCReport", "Pdf", Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), Convert.ToString(dt.Rows[i]["STCInvoiceNumber"]), solarCompanyId);
                    //    _generateStcReportBAL.CreateStcReport("FormbotSTCReport", "Pdf", Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), Convert.ToString(dt.Rows[i]["STCInvoiceNumber"]), solarCompanyId, "4", UserTypeId == 0 ? ProjectSession.LoggedInUserId : userId, UserTypeId == 0 ? ProjectSession.ResellerId : ResellerId, false, IsBackgroundRecProcess: IsBackgroundRecProcess);
                    //    CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), 0);
                    //}
                    return true;
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    //throw;
            //}
            return false;
        }

        public bool SetCacheDataFroSTCSubmissionURL(int STCJobDetailsID)
        {
            try
            {
                //await CommonBAL.SetCacheDataForSTCSubmission(STCJobDetailsID, 0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        ///// <summary>
        ///// CreateEntryInRECForSelectedJobs
        ///// </summary>
        ///// <param name="resellerId"></param>
        ///// <param name="jobs"></param>
        ///// <param name="JobsSerialNumbers"></param>
        ///// <param name="JobIDs"></param>
        ///// <param name="JobType"></param>
        ///// <returns>ActionResult</returns>
        //[HttpPost]
        //public ActionResult CreateEntryInRECForSelectedJobs(string resellerId, string jobs, string[] JobsSerialNumbers, string JobIDs, string JobType)
        //{

        //    //string strFindTerm = "123112;45678;787878";
        //    //strFindTerm = strFindTerm.Replace(";", "\r\n");
        //    //STCInvoiceBAL obj = new STCInvoiceBAL();
        //    //DataTable dt = obj.GetJobRefNumberBySerialNumbers(strFindTerm).Tables[0];            

        //    clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
        //    try
        //    {
        //        string FilePath = string.Empty;
        //        string UploadURL = string.Empty;
        //        string referer = string.Empty;
        //        string paramname = string.Empty;
        //        bool IsPVDJob = false;
        //        DataTable dtCSV_JobID = new DataTable();

        //        #region Generate CSV File Based on Selected Job IDs

        //        // Generate CSV Files based on selected job ids and get file path to upload file into REC Registry                

        //        FilePath = Server.MapPath("~/UserDocuments/" + DateTime.Now.Ticks + ".csv");
        //        if (JobType == "1") // PVD Jobs
        //            GetBulkUploadCSV_PVD(JobIDs, FilePath, ref dtCSV_JobID);
        //        else  // SWH Jobs
        //            GetBulkUploadSWHCSV(JobIDs, FilePath, ref dtCSV_JobID);

        //        //FilePath = @"C:\Users\pci36\Downloads\SWHJobs636142081497141328.csv";
        //        #endregion

        //        #region REC Regsitry call and get response from REC
        //        // PVD Jobs
        //        if (JobType == "1")
        //        {
        //            UploadURL = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/sgu/register-bulk";
        //            referer = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/register-bulk-small-generation-unit";
        //            paramname = "sguBulkUploadFile";
        //            IsPVDJob = true;
        //        }
        //        else
        //        {
        //            // SWH Jobs (Still not confirmed this SWH URL)
        //            UploadURL = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/swh/register-bulk";
        //            referer = "https://www.rec-registry.gov.au/rec-registry/app/smallunits/register-bulk-solar-water-heater";
        //            paramname = "bulkUploadFile";
        //            IsPVDJob = false;
        //        }

        //        // Get Reseller user REC Credentials from User table
        //        FormBot.Entity.User objResellerUser = new Entity.User();
        //        objResellerUser = _userBAL.GetResellerUserRECCredentialsByResellerID(Convert.ToInt32(resellerId));
        //        //objResellerUser.RecUserName = "LAMH53041";
        //        //objResellerUser.RecPassword = "Tatva1412!!!";

        //        if (objResellerUser != null && !string.IsNullOrEmpty(objResellerUser.RecUserName) && !string.IsNullOrEmpty(objResellerUser.RecPassword))
        //        {
        //            RECRegistryHelper.AuthenticateUser_UploadFileForREC(FilePath, JobsSerialNumbers, ref JsonResponseObj, ref dtCSV_JobID, UploadURL, referer, paramname, IsPVDJob, objResellerUser);
        //            if (JsonResponseObj.status == "Completed")
        //            {
        //                bool IsSuccess = true;

        //                if (!string.IsNullOrEmpty(jobs))
        //                {
        //                    IsSuccess = SaveSTCInvoice(resellerId, jobs);
        //                }
        //                CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(Convert.ToInt32(resellerId));
        //                //if (IsInvoiced.ToLower() == "true")
        //                //    IsSuccess = true;
        //                //else
        //                //{
        //                //    if (!string.IsNullOrEmpty(STCSettlementTerm) && Convert.ToInt32(STCSettlementTerm) != 4)
        //                //    {
        //                //        IsSuccess = SaveSTCInvoice(resellerId, jobs);
        //                //    }
        //                //    else
        //                //    {
        //                //        IsSuccess = true;
        //                //    }
        //                //}

        //                if (IsSuccess)
        //                {
        //                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
        //                    JsonResponseObj.status = "Completed";
        //                    JsonResponseObj.strErrors = "<ul><li>File uploaded successfully</li></ul>";
        //                }
        //                else
        //                {
        //                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
        //                    JsonResponseObj.status = "Failed";
        //                    JsonResponseObj.strErrors = "<ul><li>Error while saving data into Invoice table</li></ul>";
        //                }
        //                return this.Json(JsonResponseObj);
        //            }
        //        }
        //        else
        //        {
        //            JsonResponseObj = new clsUploadedFileJsonResponseObject();
        //            JsonResponseObj.status = "Failed";
        //            JsonResponseObj.strErrors = "<ul><li>Reseller REC Credentails not found.</li></ul>";
        //        }
        //        return Json(JsonResponseObj);

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        // It will execute when throws error before REC Registry                 
        //        JsonResponseObj = new clsUploadedFileJsonResponseObject();
        //        JsonResponseObj.status = "Failed";
        //        JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
        //        return this.Json(JsonResponseObj);
        //    }
        //}

        /// <summary>
        /// CreateEntryInRECForSelectedJobs
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="jobs"></param>
        /// <param name="JobsSerialNumbers"></param>
        /// <param name="JobIDs"></param>
        /// <param name="JobType"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult> CreateEntryInRECForSelectedJobs(string resellerId, string jobs, string JobIDs, string JobType, decimal TotalSTC = 0, string CERLoginId = "", string CERPassword = "", string RECAccName = "", string LoginType = "", string RECCompanyName = "")
        {
            clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
            try
            {
                int UserId = ProjectSession.LoggedInUserId;
                int UserTypeId = ProjectSession.UserTypeId;
                if ((JobType == "1" && _createJob.CheckInstallationDate(JobIDs)) || JobType == "2")
                {
                    await InsertEntryInRec(resellerId, jobs, JobIDs, JobType, UserId, UserTypeId, TotalSTC, CERLoginId, CERPassword, RECAccName, LoginType, RECCompanyName);
                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                    JsonResponseObj.status = "Completed";
                    JsonResponseObj.strErrors = "<ul><li>Jobs are sent for REC creation at " + DateTime.Now + "</li></ul>";
                }
                else
                {
                    JsonResponseObj = new clsUploadedFileJsonResponseObject();
                    JsonResponseObj.status = "Failed";
                    JsonResponseObj.strErrors = "<ul><li>" + "Select jobs which have installation date either before april or from april." + "</li></ul>";
                }
            }
            catch (Exception ex)
            {
                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
            return this.Json(JsonResponseObj);
        }

        public async Task InsertEntryInRec(string resellerId, string jobs, string JobIDs, string JobType, int UserId, int UserTypeId, decimal TotalSTC = 0, string CERLoginId = "", string CERPassword = "", string RECAccName = "", string LoginType = "", string RECCompanyName = "")
        {
            string FilePath = string.Empty;
            string UploadURL = string.Empty;
            string referer = string.Empty;
            string paramname = string.Empty;
            string spvParamName = string.Empty;
            string spvFilePath = string.Empty;
            bool IsPVDJob = false;
            string sguBulkUploadDocumentsParamName = string.Empty;
            string sguBulkUploadDocumentsFilePath = string.Empty;
            string JobIdForException = string.Empty;
            clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
            try
            {
                List<string> lstJobIds = JobIDs.Split(',').ToList();
                int BatchRecUploadCount = Convert.ToInt32(ProjectConfiguration.BatchRecUploadCount);

                List<string> lstBatches = SplitList<string>(lstJobIds, BatchRecUploadCount);
                // DataTable dtBatches = ListToDataTable(lstBatches);
                // For caching purpose

                #region comment
                //DataSet dsStcJobDetailsId = _createJob.InsertGBBatchRECUploadId(dtBatches, Convert.ToInt32(resellerId), dateTimeTicks.ToString(), TotalSTC, UserId, CERLoginId, CERPassword, RECAccName, LoginType);
                //string stcjobids = string.Empty;
                //string gbBatchRecUploadId = "";
                //if (dsStcJobDetailsId.Tables.Count > 0)
                //{
                //    if (dsStcJobDetailsId.Tables[0].Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in dsStcJobDetailsId.Tables[0].Rows)
                //        {
                //            stcjobids = stcjobids + dr["StcJobDetailsId"].ToString() + ",";
                //        }
                //        stcjobids = stcjobids.Remove(stcjobids.Length - 1);
                //        DataTable dtGetSTCData = _createJob.GetSTCDetailsAndJobDataForCache(stcjobids, null);

                //        if (dtGetSTCData.Rows.Count > 0)
                //        {
                //            for (int i = 0; i < dtGetSTCData.Rows.Count; i++)
                //            {
                //                SortedList<string, string> data = new SortedList<string, string>();
                //                gbBatchRecUploadId = dtGetSTCData.Rows[i]["GBBatchRECUploadId"].ToString();
                //                data.Add("GBBatchRECUploadId", gbBatchRecUploadId);
                //                CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                //                Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from insert entry in rec: " + (dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()) + " BulkUploadId: " + gbBatchRecUploadId);
                //                string descriptionLog = "Insert entry in REC from create in rec button: BulkUploadId- "+gbBatchRecUploadId+" from Account- "+RECAccName+" with CERLoginId- "+CERLoginId;
                //                _createJob.SaveSTCJobHistory(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), 19, UserId, descriptionLog, DateTime.Now, ProjectSession.LoggedInUserId);
                //            }
                //        }
                //    }

                //}
                #endregion

                foreach (var JobIds in lstBatches)
                {
                    List<string> jids = new List<string>();
                    jids.Add(JobIds);
                    DataTable dtBatches = ListToDataTable(jids);
                    long dateTimeTicks = DateTime.Now.Ticks;
                    DataSet dsStcJobDetailsId = _createJob.InsertGBBatchRECUploadId(dtBatches, Convert.ToInt32(resellerId), dateTimeTicks.ToString(), TotalSTC, UserId, CERLoginId, CERPassword, RECAccName, LoginType, RECCompanyName);
                    string stcjobids = string.Empty;
                    string gbBatchRecUploadId = "";
                    if (dsStcJobDetailsId.Tables.Count > 0)
                    {
                        if (dsStcJobDetailsId.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsStcJobDetailsId.Tables[0].Rows)
                            {
                                stcjobids = stcjobids + dr["StcJobDetailsId"].ToString() + ",";
                            }
                            stcjobids = stcjobids.Remove(stcjobids.Length - 1);
                            DataTable dtGetSTCData = _createJob.GetSTCDetailsAndJobDataForCache(stcjobids, null);

                            if (dtGetSTCData.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtGetSTCData.Rows.Count; i++)
                                {
                                    SortedList<string, string> data = new SortedList<string, string>();
                                    gbBatchRecUploadId = dtGetSTCData.Rows[i]["GBBatchRECUploadId"].ToString();
                                    data.Add("GBBatchRECUploadId", gbBatchRecUploadId);
                                    await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                                    Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from insert entry in rec: " + (dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()) + " BulkUploadId: " + gbBatchRecUploadId);
                                    string descriptionLog = "Insert entry in REC from create in rec button: BulkUploadId- " + gbBatchRecUploadId + " from Account- " + RECAccName + " with CERLoginId- " + CERLoginId;
                                    _createJob.SaveSTCJobHistory(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), 19, UserId, descriptionLog, DateTime.Now, ProjectSession.LoggedInUserId);
                                }
                            }
                        }

                    }

                    JobIdForException = string.Empty;
                    JobIdForException = JobIds;
                    DataSet ds = _createJob.GetJobsForRecInsertOrUpdateNew(JobIds);

                    if (ds.Tables.Count > 0)
                    {
                        // Get Reseller user REC Credentials from User table                    

                        string NotInRecSTCJobDetailsID = "";
                        if (ds.Tables[0].Rows.Count > 0 || ds.Tables[1].Rows.Count > 0)
                        {
                            //string NotInRecJobIds = string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                            //NotInRecSTCJobDetailsID = string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());
                            string NotInRecJobIds = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                            NotInRecSTCJobDetailsID = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());
                            DataSet dsCSV_JobID = new DataSet();
                            DataTable dtSPVXmlPath = new DataTable();


                            #region Generate CSV File Based on Selected Job IDs
                            // Generate CSV Files based on selected job ids and get file path to upload file into REC Registry     

                            FilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".csv";

                            if (JobType == "1") // PVD Jobs
                                _commonBulkUploadToCER.GetBulkUploadCSV_PVD(NotInRecJobIds, FilePath, ref dsCSV_JobID, ref dtSPVXmlPath, true);
                            else  // SWH Jobs
                                _commonBulkUploadToCER.GetBulkUploadSWHCSV(NotInRecJobIds, FilePath, ref dsCSV_JobID, true);

                            if (JobType == "1")
                            {
                                if (dsCSV_JobID.Tables.Count > 0 && dsCSV_JobID.Tables[0] != null)

                                    foreach (DataRow dr in dsCSV_JobID.Tables[0].Rows)
                                    {
                                        string srcPath = ProjectSession.ProofDocuments + "\\JobDocuments\\" + Convert.ToString(dr["JobId"]) + "\\" + dr["Documents Zip File"].ToString();
                                        string fileName = System.IO.Path.GetFileName(srcPath);
                                        string destFolder = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;
                                        string destPath = Path.Combine(destFolder + "\\" + fileName);

                                        if (!Directory.Exists(destFolder))
                                        {
                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();
                                            Directory.CreateDirectory(destFolder);
                                        }

                                        if (System.IO.File.Exists(srcPath))
                                        {
                                            System.IO.File.Copy(srcPath, destPath, true);
                                        }
                                    }

                                #region Creating zip file

                                string InputDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;

                                sguBulkUploadDocumentsFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + "_REC.zip";

                                using (Stream zipStream = new FileStream(Path.GetFullPath(sguBulkUploadDocumentsFilePath), FileMode.Create, FileAccess.Write))
                                {
                                    using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create))
                                    {
                                        bool IsAnyFileExists = false;
                                        foreach (var filePath in System.IO.Directory.GetFiles(InputDirectory, "*.*", System.IO.SearchOption.AllDirectories))
                                        {
                                            var relativePath = Path.GetFileName(filePath);//filePath.Replace(InputDirectory, string.Empty);
                                            using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                            using (Stream fileStreamInZip = archive.CreateEntry(relativePath).Open())
                                                fileStream.CopyTo(fileStreamInZip);

                                            IsAnyFileExists = true;
                                        }
                                        if (!IsAnyFileExists)
                                            sguBulkUploadDocumentsFilePath = "";
                                    }
                                }
                                #endregion Creating zip file

                                #endregion
                            }

                            #region REC Regsitry call and get response from REC
                            // PVD Jobs
                            if (JobType == "1")
                            {
                                // Zip files for SPV Jobs
                                int spvDataCount = 0;
                                if (dsCSV_JobID.Tables[0] != null)
                                {
                                    spvDataCount = dsCSV_JobID.Tables[0].Select("[Signed data package] <> ''").Count();
                                }
                                if (spvDataCount > 0)
                                {
                                    spvParamName = "sguBulkUploadSdpZip";
                                    spvFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".zip";
                                    //WriteToLogFile("spvfilePath: " + spvFilePath);
                                    _log.LogException("spvfilePath: " + spvFilePath, null);
                                    using (ZipFile zip = new ZipFile())
                                    {
                                        foreach (DataRow dr in dtSPVXmlPath.Rows)
                                        {
                                            zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, dr["Path"].ToString()), "");
                                            zip.Save(spvFilePath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _stcInvoiceServiceBAL.UpdateQueuedSubmissionStatus(gbBatchRecUploadId, "In Queue");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _stcInvoiceServiceBAL.RemoveFromQueuedRecSubmission(JobIdForException);
                //WriteToLogFile(DateTime.Now + " Exception in insert in rec:" + ex.Message);
                _log.LogException("InsertEntryInRec Error", ex);
                // It will execute when throws error before REC Registry                 
                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }

        }


        public List<T> SplitList<T>(List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            var lst = new List<T>();

            var lstWithBatchId = new List<T>();
            var lstWithoutBatchId = new List<T>();


            foreach (var item in me)
            {
                if (item.ToString().Contains('_'))
                    lstWithBatchId.Add(item);
                else
                    lstWithoutBatchId.Add(item);

            }
            var DistinctItems = lstWithBatchId.GroupBy(x => x.ToString().Split('_')[1]).Select(y => y.First().ToString().Split('_')[1]).Distinct();

            foreach (var distinctItem in DistinctItems)
            {
                var lstItem = string.Join(",", lstWithBatchId.Where(x => x.ToString().Contains(distinctItem)));
                lstItem = lstItem.Replace("_" + distinctItem, "");
                lst.Add((T)Convert.ChangeType(lstItem, typeof(T)));
            }

            for (int i = 0; i < lstWithoutBatchId.Count; i += size)
            {
                list.Add(lstWithoutBatchId.GetRange(i, Math.Min(size, lstWithoutBatchId.Count - i)));
                lst.Add((T)Convert.ChangeType(string.Join(",", lstWithoutBatchId.GetRange(i, Math.Min(size, lstWithoutBatchId.Count - i))), typeof(T)));
            }

            //for (int i = 0; i < me.Count; i += size)
            //{
            //    list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            //    lst.Add((T)Convert.ChangeType(string.Join(",", me.GetRange(i, Math.Min(size, me.Count - i))), typeof(T)));
            //}

            //DataTable table = new DataTable();
            //table.Columns.Add("StcJobDetailsId", typeof(string));
            //foreach (var item in lst)
            //{
            //    DataRow row = table.NewRow();
            //    row["StcJobDetailsId"] = item.ToString();
            //    table.Rows.Add(row);
            //}

            return lst;
        }

        public DataTable ListToDataTable<T>(List<T> lst)
        {
            DataTable table = new DataTable();
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("JobId", typeof(string));
            int i = 1;
            foreach (var item in lst)
            {
                DataRow row = table.NewRow();
                row["JobId"] = item.ToString();
                row["SrNo"] = i;
                table.Rows.Add(row);
                i++;
            }
            return table;
        }

        /// <summary>
        /// Generate CSV for selected invoice
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="jobs"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult GenerateCSVForSelectedInvoice(string resellerId, string jobs)
        {
            try
            {
                int ResellerId = 0;
                if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
                {
                    ResellerId = Convert.ToInt32(resellerId);
                }
                else
                {
                    ResellerId = ProjectSession.ResellerId;
                }

                if (!string.IsNullOrEmpty(jobs))
                {
                    CreateCSV(jobs);

                    //return this.Json(new { success = true });
                }
                else
                {
                    //return this.Json(new { success = false, errormessage = "Please select valid jobs." });
                }

            }
            catch (Exception ex)
            {
                //return this.Json(new { success = false, errormessage = ex.Message });
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate invoice number
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="str"></param>
        /// <param name="invNumber"></param>
        /// <param name="IsSTCInvoice"></param>
        /// <returns>ActionResult</returns>
        [NonAction]
        public string GenerateInvoiceNumber(int ResellerId, string str, int invNumber, int IsSTCInvoice)
        {
            string prefix = IsSTCInvoice == 1 ? "STC" : "CR";
            if (str.Split('_')[1] == "5")
            {
                return string.Format(prefix + ResellerId + "-IPP01-{0}", invNumber.ToString().PadLeft(6, '0'));
            }
            else
            {
                return string.Format(prefix + ResellerId + "-IFP-{0}", invNumber.ToString().PadLeft(6, '0'));
            }
        }

        /// <summary>
        /// Get invoice number by reseller wise
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns>ActionResult</returns>
        [NonAction]
        private int GetInvoiceNumberByResellerWise(int ResellerId)
        {
            return _stcInvoiceServiceBAL.GetInvoiceNumberByResellerWise(ResellerId);
        }

        /// <summary>
        /// Generate invoice file
        /// </summary>
        /// <param name="str"></param>
        /// <returns>ActionResult</returns>
        [NonAction]
        public string GenerateInvoiceFile(string str)
        {
            return string.Empty;
        }

        /// <summary>
        /// Get selected ids record
        /// </summary>
        /// <param name="stcInvoiceIds"></param>
        /// <param name="solarCompanyId"></param>
        /// <param name="drafts"></param>
        /// <param name="invoices"></param>
        /// <param name="resellerId"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult GetSelectedIdsRecord(string stcInvoiceIds, int solarCompanyId, int drafts, string invoices, string resellerId)
        {
            // get the previous url and store it in tempdata (It's value is used inside /Authorization/Callback method to redirect to views based on visited page)
            TempData["PreviousUrl"] = System.Web.HttpContext.Current.Request.UrlReferrer;
            TempData.Keep("PreviousUrl");

            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                DataSet dsSTCInvoice = _stcInvoiceServiceBAL.GetSelectdSTCInvoice(stcInvoiceIds, Convert.ToInt32(resellerId));
                if (dsSTCInvoice != null && dsSTCInvoice.Tables.Count > 0 && dsSTCInvoice.Tables[0] != null)
                {
                    List<STCInvoiceDetail> STCInvoiceDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoiceDetail>>(invoices);
                    int response = SendDraftsToXero(dsSTCInvoice.Tables[0], solarCompanyId, drafts, STCInvoiceDetail, Convert.ToInt32(resellerId));
                    if (response == 0)
                    {
                        return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //List<string> lstBatches = stcInvoiceIds.Split(',').ToList();
                        //DataTable dtBatches = ListToDataTable(lstBatches);
                        //_stcInvoiceServiceBAL.InsertGbInvoiceBatchId(dtBatches, Convert.ToInt32(resellerId));

                        STCInvoice sTCInvoice = new STCInvoice();
                        sTCInvoice.UserName = ProjectSession.LoggedInName;
                        for (int i = 0; i < dsSTCInvoice.Tables[0].Rows.Count; i++)
                        {
                            decimal quantity = 0;
                            decimal price = 0;
                            bool isGST = false;
                            quantity = !string.IsNullOrEmpty(dsSTCInvoice.Tables[0].Rows[i]["Quantity"].ToString()) ? Convert.ToDecimal(dsSTCInvoice.Tables[0].Rows[i]["Quantity"]) : 0;
                            price = !string.IsNullOrEmpty(dsSTCInvoice.Tables[0].Rows[i]["Price"].ToString()) ? Convert.ToDecimal(dsSTCInvoice.Tables[0].Rows[i]["Price"]) : 0;

                            isGST = Convert.ToBoolean(dsSTCInvoice.Tables[0].Rows[i]["IsGst"]);

                            sTCInvoice.JobID = Convert.ToInt32(dsSTCInvoice.Tables[0].Rows[i]["JobId"]);
                            sTCInvoice.STC = quantity + "STC@" + price + (isGST ? "+GST" : "");
                            sTCInvoice.SolarCompany = dsSTCInvoice.Tables[0].Rows[i]["CompanyName"].ToString();
                            sTCInvoice.CreatedDate = DateTime.Now;
                            sTCInvoice.STCInvoiceNumber = dsSTCInvoice.Tables[0].Rows[i]["STCInvoiceNumber"].ToString();

                            if (Convert.ToBoolean(dsSTCInvoice.Tables[0].Rows[i]["IsCreditNote"]))
                            {
                                //_jobHistory.LogJobHistory(sTCInvoice, HistoryCategory.SendCreditNotesToXero);
                                string JobHistoryMessage = "has been sent credit note " + sTCInvoice.STCInvoiceNumber + " successfully with JobID " + sTCInvoice.JobID + " for adjustment in Xero " + sTCInvoice.SolarCompany;
                                Common.SaveJobHistorytoXML(sTCInvoice.JobID, JobHistoryMessage, "Invoicing", "SendCreditNotesToXero", ProjectSession.LoggedInName, false);
                            }
                            else
                            {
                                //_jobHistory.LogJobHistory(sTCInvoice, HistoryCategory.SentToXero);
                                string JobHistoryMessage = "has been sent invoice " + sTCInvoice.STCInvoiceNumber.ToString() + " successfully with JobID " + sTCInvoice.JobID + " for payment in Xero (" + sTCInvoice.STC.ToString() + ") - " + sTCInvoice.SolarCompany;
                                Common.SaveJobHistorytoXML(sTCInvoice.JobID, JobHistoryMessage, "Invoicing", "SentToXero", ProjectSession.LoggedInName, false);
                            }
                        }
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { status = false, error = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (UnauthorizedException e)
            {
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (RateExceededException e)
            {
                return Json(new { status = false, error = "Rate limit exceeded, Please try after 1 minute." }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationException e)
            {
                int errorCount = ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.Count;
                string errorMsg = string.Join(", ", ((Xero.Api.Infrastructure.Exceptions.ValidationException)(e)).ValidationErrors.AsEnumerable().Select(a => a.Message));
                return Json(new { status = false, error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = false, error = e.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public int SendDraftsToXero(DataTable dtSTCInvoice, int solarCompanyId, int drafts, List<STCInvoiceDetail> STCInvoiceDetail, int resellerId)
        {
            int responseUpdateXeroInvoiceId = 0;
            if (!TokenUtilities.TokenExists())
            {
                XeroConnectController xcc = new XeroConnectController();
                xcc.Connect();
                return 0;
            }
            var token = TokenUtilities.GetStoredToken();
            string accessToken = token.AccessToken;
            string xeroTenantId = token.Tenants[0].TenantId.ToString();
            var response = Task.Run(async () => await SendDraftsToXero(dtSTCInvoice, solarCompanyId, drafts, STCInvoiceDetail, resellerId, accessToken, xeroTenantId));
            response.Wait();
            responseUpdateXeroInvoiceId = Convert.ToInt32(response.Result);
            return responseUpdateXeroInvoiceId;
        }

        /// <summary>
        /// Send drafts to xero
        /// </summary>
        /// <param name="dtSTCInvoice"></param>
        /// <param name="solarCompanyId"></param>
        /// <param name="drafts"></param>
        /// <param name="STCInvoiceDetail"></param>
        /// <param name="resellerId"></param>
        /// <returns>ActionResult</returns>
        public async Task<int> SendDraftsToXero(DataTable dtSTCInvoice, int solarCompanyId, int drafts, List<STCInvoiceDetail> STCInvoiceDetail, int resellerId, string accessToken, string xeroTenantId)
        {
            int responseUpdateXeroInvoiceId = 0;
            var api = new AccountingApi();
            string description = string.Empty;
            bool isTaxInclusive = false;
            DateTime dueDate = DateTime.Now;

            string companyName = string.Empty;
            string clientNumber = string.Empty;
            string CompanyABN = string.Empty;
            string STCInvoiceNumAsRefXERO = string.Empty;
            string CompanyNumber = string.Empty;

            string STCInvoiceNum = string.Empty;
            decimal quantity = 0;
            decimal price = 0;
            decimal taxRate = 0;
            decimal taxAmount = 0;
            bool isGST = false;
            string refNumberOwnerNameAddress = string.Empty;
            string STCPVDCode = string.Empty;
            bool isCreditNote = false;

            bool isPeakPay = false;
            bool isPeakPayGst = false;
            decimal peakPayTaxRate = 0;
            decimal peakPayFee = 0;
            decimal peakPayAmount = 0;


            List<KeyValuePair<int, string>> lstSTCInvoice = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < dtSTCInvoice.Rows.Count; i++)
            {
                if (dtSTCInvoice != null && dtSTCInvoice.Rows.Count > 0)
                {
                    description = Convert.ToString(dtSTCInvoice.Rows[i]["description"]);
                    companyName = Convert.ToString(dtSTCInvoice.Rows[i]["CompanyName"]);
                    clientNumber = Convert.ToString(dtSTCInvoice.Rows[i]["ClientNumber"]);
                    CompanyABN = Convert.ToString(dtSTCInvoice.Rows[i]["CompanyABN"]);
                    CompanyNumber = Convert.ToString(dtSTCInvoice.Rows[i]["CompanyNumber"]);
                    if (!string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["InvoiceDueDateId"].ToString()))
                        dueDate = FormBot.Helper.Helper.Common.SettingDueDate(Convert.ToInt32(dtSTCInvoice.Rows[i]["InvoiceDueDateId"]));
                    else
                        dueDate = DateTime.Now.AddDays(Convert.ToDouble(ProjectConfiguration.XeroDraftsDueDate));

                    STCInvoiceNum = Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"]);
                    quantity = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["Quantity"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["Quantity"]) : 0;
                    price = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["Price"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["Price"]) : 0;
                    taxRate = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["TaxRate"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["TaxRate"]) : 0;
                    isGST = Convert.ToBoolean(dtSTCInvoice.Rows[i]["IsGst"]);
                    refNumberOwnerNameAddress = Convert.ToString(dtSTCInvoice.Rows[i]["description"]);
                    STCPVDCode = Convert.ToString(dtSTCInvoice.Rows[i]["STCPVDCode"]);
                    isCreditNote = Convert.ToBoolean(dtSTCInvoice.Rows[i]["IsCreditNote"]);

                    isTaxInclusive = isGST == true ? true : false;
                    taxAmount = taxRate != 0 ? ((quantity * price) * taxRate) / 100 : 0;

                    STCInvoiceNumAsRefXERO = STCInvoiceNum + " - " + quantity + "STC@" + price + (isGST ? "+GST" : "") + " - " + refNumberOwnerNameAddress + ", " + STCPVDCode;

                    isPeakPay = Convert.ToBoolean(dtSTCInvoice.Rows[i]["IsPeakPay"]);
                    isPeakPayGst = Convert.ToBoolean(dtSTCInvoice.Rows[i]["IsPeakPayGst"]);
                    peakPayFee = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["PeakPayFee"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["PeakPayFee"]) : 0;
                    peakPayTaxRate = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["PeakPayGstRate"].ToString()) ? Convert.ToDecimal(dtSTCInvoice.Rows[i]["PeakPayGstRate"]) : 0;
                    peakPayAmount = peakPayTaxRate != 0 ? ((quantity * peakPayFee) * peakPayTaxRate) / 100 : 0;

                }

                List<oauthModel.LineItem> lineItems = new List<oauthModel.LineItem>();

                //decimal? quantity = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.Quantity).FirstOrDefault();
                //decimal? price = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.Price).FirstOrDefault();
                //isTaxInclusive = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.IsTax).FirstOrDefault() == 1 ? true : false;
                //decimal? taxRate = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.TaxRate).FirstOrDefault();
                //decimal? taxAmount = taxRate != null ? ((quantity * price) * taxRate) / 100 : 0;
                //string STCPVDCode = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.STCPVDCode).FirstOrDefault();
                //bool isCreditNote = STCInvoiceDetail.AsEnumerable().Where(c => c.Id == Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"])).Select(a => a.IsCreditNote).FirstOrDefault();

                oauthModel.LineItem lineItem = null;

                List<oauthModel.LineItemTracking> objTracking = new List<oauthModel.LineItemTracking>();
                oauthModel.LineItemTracking objItemTrackingCategory = new oauthModel.LineItemTracking();
                objItemTrackingCategory.Name = "Trader";
                objItemTrackingCategory.Option = Convert.ToString(dtSTCInvoice.Rows[i]["TrackingName1"]);
                objTracking.Add(objItemTrackingCategory);

                lineItem = new oauthModel.LineItem
                {
                    //mandatory
                    //AccountCode = accountCode,
                    Description = !string.IsNullOrEmpty(STCPVDCode) ? description + ", " + STCPVDCode : description,
                    UnitAmount = (float)price,
                    Quantity = (float)quantity,
                    //ItemCode = (isTaxInclusive == true ? "STC+GST" : "STC"),
                    ItemCode = "STC",
                    TaxAmount = (isTaxInclusive == true ? ((taxAmount) != 0 ? Convert.ToDouble(taxAmount) : 0) : 0),
                    TaxType = (isTaxInclusive == true ? "INPUT" : "EXEMPTEXPENSES"),
                    Tracking = objTracking
                };

                lineItems.Add(lineItem);

                if (isPeakPay)
                {
                    lineItem = new oauthModel.LineItem
                    {
                        //mandatory
                        //AccountCode = "",
                        Description = "PeakPay Charges",
                        UnitAmount = (float)-peakPayFee,
                        Quantity = (float)quantity,
                        ItemCode = "PeakFees",
                        TaxAmount = -(isPeakPayGst == true ? (peakPayAmount != 0 ? Convert.ToDouble(peakPayAmount) : 0) : 0),
                        TaxType = (isPeakPayGst == true ? "OUTPUT" : "EXEMPTOUTPUT"),
                        Tracking = objTracking
                    };
                    lineItems.Add(lineItem);
                }
                var invoices = (List<oauthModel.Invoice>)null;
                var creditNotes = (List<oauthModel.CreditNote>)null;
                var paymentDate = !string.IsNullOrEmpty(dtSTCInvoice.Rows[i]["createdDate"].ToString()) ? Convert.ToDateTime(dtSTCInvoice.Rows[i]["createdDate"]) : DateTime.Now.Date;
                AccountingApi accountingApi = new AccountingApi();

                if (!isCreditNote)
                {
                    IEnumerable<oauthModel.Contact> lstContactWithAccountNumber = new List<oauthModel.Contact>();

                    if (!string.IsNullOrEmpty(CompanyNumber))
                    {
                        var response = Task.Run(async () => await accountingApi.GetContactsAsync(accessToken, xeroTenantId, where: string.Format("AccountNumber == \"{0}\"", CompanyNumber)));
                        response.Wait();
                        lstContactWithAccountNumber = response.Result._Contacts.ToList();
                    }
                    if (lstContactWithAccountNumber.Any())
                    {
                        if (lstContactWithAccountNumber.FirstOrDefault().Name != companyName)
                            lstContactWithAccountNumber.FirstOrDefault().Name = companyName;
                    }
                    oauthModel.Invoices invoicesCreate = new oauthModel.Invoices();
                    List<oauthModel.Invoice> lstInv = new List<oauthModel.Invoice>();
                    oauthModel.Invoice inv = new oauthModel.Invoice();
                    inv.Type = oauthModel.Invoice.TypeEnum.ACCPAY;
                    inv.Contact = (lstContactWithAccountNumber.Any() ? lstContactWithAccountNumber.FirstOrDefault() : (new oauthModel.Contact { Name = companyName, TaxNumber = CompanyABN }));
                    inv.Date = paymentDate;
                    inv.DueDate = dueDate;
                    inv.LineAmountTypes = oauthModel.LineAmountTypes.Exclusive;
                    inv.InvoiceNumber = STCInvoiceNumAsRefXERO;
                    inv.LineItems = lineItems;
                    lstInv.Add(inv);
                    invoicesCreate._Invoices = lstInv;

                    var invResponse = Task.Run(async () => await accountingApi.CreateInvoicesAsync(accessToken, xeroTenantId, invoicesCreate));
                    invResponse.Wait();
                    invoices = invResponse.Result._Invoices.ToList();
                }
                else
                {
                    oauthModel.CreditNotes invoicesCreate = new oauthModel.CreditNotes();
                    List<oauthModel.CreditNote> lstInv = new List<oauthModel.CreditNote>();
                    oauthModel.CreditNote inv = new oauthModel.CreditNote();
                    inv.Type = oauthModel.CreditNote.TypeEnum.ACCPAYCREDIT;
                    inv.Contact = new oauthModel.Contact { Name = companyName };
                    inv.Date = paymentDate;
                    inv.LineAmountTypes = oauthModel.LineAmountTypes.Exclusive;
                    inv.CreditNoteNumber = STCInvoiceNumAsRefXERO;
                    //optional
                    inv.LineItems = lineItems;
                    lstInv.Add(inv);
                    invoicesCreate._CreditNotes = lstInv;

                    var invResponse = Task.Run(async () => await accountingApi.CreateCreditNotesAsync(accessToken, xeroTenantId, invoicesCreate));
                    invResponse.Wait();
                    creditNotes = invResponse.Result._CreditNotes.ToList();
                }

                string xeroInvoiceId = string.Empty;
                if (!isCreditNote)
                    xeroInvoiceId = invoices[0].InvoiceID.ToString();
                else
                    xeroInvoiceId = creditNotes[0].CreditNoteID.ToString();

                responseUpdateXeroInvoiceId = _stcInvoiceServiceBAL.UpdateXeroInvoiceId(Convert.ToInt32(dtSTCInvoice.Rows[i]["STCInvoiceID"]), xeroInvoiceId);
            }
            return responseUpdateXeroInvoiceId;
        }

        /// <summary>
        /// Get all payment detail while sync with xero
        /// </summary>
        /// <param name="resellerId"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public JsonResult SyncWithXero(string resellerId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                if (!TokenUtilities.TokenExists())
                {
                    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                }
                var token = TokenUtilities.GetStoredToken();
                string accessToken = token.AccessToken;
                string xeroTenantId = token.Tenants[0].TenantId.ToString();
                int userId = ProjectSession.LoggedInUserId;
                var response = Task.Run(async () => await SyncWithXero(resellerId, accessToken, xeroTenantId, userId));
                response.Wait();
                List<Remittance> remittanceData = new List<Remittance>();
                remittanceData = response.Result;
                if (remittanceData != null && remittanceData.Count > 0)
                {
                    for (int i = 0; i < remittanceData.Count; i++)
                    {
                        _generateStcReportBAL.GenerateRemittanceNew(remittanceData[i], resellerId);

                        var stcInvoiceId = !string.IsNullOrEmpty(Convert.ToString(remittanceData[i].STCInvoiceID)) ? Convert.ToInt32(remittanceData[i].STCInvoiceID) : 0;
                        if (Convert.ToInt32(resellerId) != 0)
                        {
                            //_reseller.SyncToXeroWithReseller(Convert.ToInt32(resellerId));
                            if (Convert.ToInt32(remittanceData[i].StillOwing) == 0)
                            {
                                //_jobHistory.LogJobHistory(remittanceData[i], HistoryCategory.SyncWithXero);
                                string JobHistoryMessage = "has try to settlement of invoice for JobID " + remittanceData[i].JobId + " and invoice " + remittanceData[i].STCInvoiceNumber + " has been settled and moved in PAID status successfully from Xero ";
                                Common.SaveJobHistorytoXML(remittanceData[i].JobId, JobHistoryMessage, "Invoicing", "SyncWithXero", ProjectSession.LoggedInName, false);
                            }
                            else
                            {
                                //_jobHistory.LogJobHistory(remittanceData[i], HistoryCategory.PartiallySettledWithXero);
                                string JobHistoryMessage = "has try to settle invoice for JobID " + remittanceData[i].JobId + " and invoice " + remittanceData[i].STCInvoiceNumber + " has been partially settled(" + remittanceData[i].AmountPaid.ToString() + ") successfully from Xero.";
                                Common.SaveJobHistorytoXML(remittanceData[i].JobId, JobHistoryMessage, "Invoicing", "PartiallySettledWithXero", ProjectSession.LoggedInName, false);
                            }
                        }
                    }
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
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

        public async Task<List<Remittance>> SyncWithXero(string resellerId, string accessToken, string xeroTenantId, int userId)
        {
            try
            {
                var AccountingApi = new AccountingApi();
                Reseller reseller = _reseller.GetResellerByResellerID(Convert.ToInt32(resellerId));
                List<Xero.NetStandard.OAuth2.Model.Payment> payment = new List<Xero.NetStandard.OAuth2.Model.Payment>();

                var result = reseller.SyncXeroDate != null ? await AccountingApi.GetPaymentsAsync(accessToken, xeroTenantId, reseller.SyncXeroDate) : await AccountingApi.GetPaymentsAsync(accessToken, xeroTenantId);
                payment = result._Payments.ToList();

                if (payment != null)
                {
                    DataTable dtPayment = CreateXeroInvoicePaymentTable();
                    payment.ForEach(d =>
                    {
                        DataRow dr = dtPayment.NewRow();
                        dr["Amount"] = d.Amount;
                        dr["PaymentDate"] = d.Date;
                        dr["XeroInvoiceID"] = d.Invoice.InvoiceID.ToString();
                        dr["XeroPaymentID"] = d.PaymentID.ToString();
                        dr["IsDeleted"] = d.Status == Xero.NetStandard.OAuth2.Model.Payment.StatusEnum.AUTHORISED ? false : true;
                        dtPayment.Rows.Add(dr);
                    });

                    List<string> lstStatus = new List<string>();
                    lstStatus.Add("Voided");
                    lstStatus.Add("Deleted");
                    var result1 = reseller.SyncXeroDate != null ? await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId, reseller.SyncXeroDate, statuses: lstStatus) : await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId);
                    string STCInvoiceStatusData = string.Empty;
                    var invoice = result1._Invoices.ToList();
                    if (invoice != null)
                    {
                        List<STCInvoicePayment> lstSTCInvoice = invoice.AsEnumerable().Select(row =>
                                                                new STCInvoicePayment
                                                                {
                                                                    XeroInvoiceID = row.InvoiceID.ToString()
                                                                }).ToList();
                        if (lstSTCInvoice.Count > 0)
                        {
                            STCInvoiceStatusData = string.Join(",", lstSTCInvoice.Select(a => a.XeroInvoiceID).ToList());
                        }
                    }

                    List<Remittance> remittanceData = _stcInvoiceServiceBAL.InsertSTCInvoicePayment(dtPayment, userId, DateTime.Now, Convert.ToInt32(resellerId), userId, DateTime.Now, DateTime.UtcNow, STCInvoiceStatusData);

                    return remittanceData;
                }
                else
                {
                    return new List<Remittance>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable CreateXeroInvoicePaymentTable()
        {
            DataTable dtPayment = new DataTable();
            dtPayment.Columns.Add("Amount", typeof(decimal));
            dtPayment.Columns.Add("PaymentDate", typeof(DateTime));
            dtPayment.Columns.Add("XeroInvoiceID", typeof(string));
            dtPayment.Columns.Add("XeroPaymentID", typeof(string));
            dtPayment.Columns.Add("IsDeleted", typeof(bool));
            return dtPayment;
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// Generate remittance
        /// </summary>
        /// <param name="remittanceData"></param>
        /// <returns>ActionResult</returns>
        //public int GenerateRemittance(DataSet remittanceData, string resellerId)
        //{
        //    string Filename = "Remittance_Invoice";
        //    string ExportType = "pdf";
        //    string pathName = string.Empty;
        //    string STCInvoicePaymentID = string.Empty;
        //    int response = 0;

        //    List<STCInvoicePayment> lstFilePath = new List<Entity.STCInvoicePayment>();

        //    DataTable dt = remittanceData.Tables[0];
        //    IDictionary<string, string> pathList = new Dictionary<string, string>();

        //    //ManualResetEvent[] completionEvents = new ManualResetEvent[dt.Rows.Count];
        //    //Task[] tasks = new Task[dt.Rows.Count];

        //    //int index = 0;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        //int pageIndex = index;

        //        var serverPath = Server.MapPath("~/");
        //        //tasks[pageIndex] = new Task(() =>
        //        //{
        //        DataTable dtTest = dt.Clone();
        //        dtTest.Rows.Add(dr.ItemArray);

        //        Microsoft.Reporting.WebForms.Warning[] warnings;
        //        string[] streamIds;
        //        string mimeType = string.Empty;
        //        string encoding = string.Empty;
        //        string extension = string.Empty;
        //        ReportViewer viewer = new ReportViewer();
        //        //XmlDocument oXD = new XmlDocument();

        //        ////oXD.Load(serverPath + "Reports\\StcRemittance.rdlc");
        //        //oXD.Load(serverPath + "Reports\\StcRemittance_WholeSaler.rdlc");

        //        DataSet ds = new DataSet();
        //        int report = 1;

        //        string PaymentDate = string.Empty;
        //        string SentDate = string.Empty;
        //        string ABN = string.Empty;
        //        string InvoiceDate = string.Empty;
        //        string Reference = string.Empty;
        //        string Reference_WholeSaler = string.Empty;
        //        string InvoiceTotal = string.Empty;
        //        string AmountPaid = string.Empty;
        //        string Stillwing = string.Empty;
        //        string JobId = string.Empty;
        //        string STCInvoiceID = string.Empty;
        //        //string XeroPaymentID = string.Empty;
        //        string CompanyName = string.Empty;
        //        string LogoPath = string.Empty;
        //        string Logo = string.Empty;
        //        int resellerUserId = 0;
        //        int solarCompanyId = 0;
        //        int solarCompanyUserId = 0;
        //        int rankNum = 0;
        //        string Gst = string.Empty;
        //        decimal NoOfStc = 0;
        //        decimal StcPrice = 0;
        //        bool IsWholeSaler = false;

        //        if (dr != null)
        //        {

        //            PaymentDate = !string.IsNullOrEmpty(dr["PaymentDate"].ToString()) ? Convert.ToDateTime(dr["PaymentDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            SentDate = !string.IsNullOrEmpty(dr["SentDate"].ToString()) ? Convert.ToDateTime(dr["SentDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            ABN = !string.IsNullOrEmpty(dr["ABN"].ToString()) ? dr["ABN"].ToString() : string.Empty;
        //            InvoiceDate = !string.IsNullOrEmpty(dr["InvoiceCreatedDate"].ToString()) ? Convert.ToDateTime(dr["InvoiceCreatedDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            Reference = !string.IsNullOrEmpty(dr["Reference"].ToString()) ? dr["Reference"].ToString() : string.Empty;
        //            InvoiceTotal = !string.IsNullOrEmpty(dr["InvoiceTotal"].ToString()) ? dr["InvoiceTotal"].ToString() : string.Empty;
        //            AmountPaid = !string.IsNullOrEmpty(dr["TotalPaid"].ToString()) ? dr["TotalPaid"].ToString() : string.Empty;
        //            Stillwing = !string.IsNullOrEmpty(dr["StillOwing"].ToString()) ? dr["StillOwing"].ToString() : string.Empty;

        //            JobId = !string.IsNullOrEmpty(dr["JobId"].ToString()) ? dr["JobId"].ToString() : string.Empty;
        //            //XeroPaymentID = !string.IsNullOrEmpty(dr["XeroPaymentID"].ToString()) ? dr["XeroPaymentID"].ToString() : string.Empty;
        //            STCInvoiceID = !string.IsNullOrEmpty(dr["STCInvoiceID"].ToString()) ? dr["STCInvoiceID"].ToString() : string.Empty;
        //            CompanyName = !string.IsNullOrEmpty(dr["CompanyName"].ToString()) ? dr["CompanyName"].ToString() : string.Empty;
        //            STCInvoicePaymentID = !string.IsNullOrEmpty(dr["STCInvoicePaymentID"].ToString()) ? dr["STCInvoicePaymentID"].ToString() : string.Empty;
        //            Logo = !string.IsNullOrEmpty(dr["Logo"].ToString()) ? dr["Logo"].ToString() : string.Empty;
        //            resellerUserId = !string.IsNullOrEmpty(dr["ResellerUserId"].ToString()) ? Convert.ToInt32(dr["ResellerUserId"]) : 0;
        //            solarCompanyId = !string.IsNullOrEmpty(dr["SolarCompanyId"].ToString()) ? Convert.ToInt32(dr["SolarCompanyId"]) : 0;
        //            solarCompanyUserId = !string.IsNullOrEmpty(dr["SolarCompanyUserId"].ToString()) ? Convert.ToInt32(dr["SolarCompanyUserId"]) : 0;
        //            rankNum = !string.IsNullOrEmpty(dr["RankNumber"].ToString()) ? Convert.ToInt32(dr["RankNumber"]) : 0;
        //            Gst = dr["IsGst"].ToString() == "True" ? "+Gst" : "";
        //            NoOfStc = !string.IsNullOrEmpty(dr["CalculatedSTC"].ToString()) ? Convert.ToDecimal(dr["CalculatedSTC"]) : 0;
        //            StcPrice = !string.IsNullOrEmpty(dr["STCPrice"].ToString()) ? Convert.ToDecimal(dr["STCPrice"]) : 0;
        //            IsWholeSaler = Convert.ToBoolean(dr["IsWholeSaler"]);

        //        }
        //        if (Logo != "" && Logo != null)
        //        {
        //            var LogoP = Path.Combine(ProjectSession.UploadedDocumentPath + "UserDocuments" + "\\" + resellerUserId, Logo);
        //            LogoPath = new Uri(LogoP).AbsoluteUri;
        //        }
        //        else
        //        {
        //            LogoPath = "";
        //        }

        //        string FromAddressLine1 = string.Empty;
        //        string FromAddressLine2 = string.Empty;
        //        string FromAddressLine3 = string.Empty;
        //        string FromCompanyName = string.Empty;
        //        string ToAddressLine1 = string.Empty;
        //        string ToAddressLine2 = string.Empty;
        //        string ToAddressLine3 = string.Empty;
        //        string ToCompanyName = string.Empty;

        //        DataSet dsAddress = _stcInvoiceServiceBAL.GetSolarCompanyAndResellerAddress(solarCompanyId, string.IsNullOrEmpty(resellerId) ? 0 : Convert.ToInt32(resellerId));
        //        if (dsAddress != null && dsAddress.Tables.Count > 0 && dsAddress.Tables[0] != null && dsAddress.Tables[0].Rows.Count > 0)
        //        {
        //            ToAddressLine1 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine1"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
        //            ToAddressLine2 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine2"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
        //            ToAddressLine3 = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToAddressLine3"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
        //            ToCompanyName = !string.IsNullOrEmpty(dsAddress.Tables[0].Rows[0]["ToCompanyName"].ToString()) ? dsAddress.Tables[0].Rows[0]["ToCompanyName"].ToString() : string.Empty;
        //        }
        //        if (dsAddress != null && dsAddress.Tables.Count > 0 && dsAddress.Tables[1] != null && dsAddress.Tables[1].Rows.Count > 0)
        //        {
        //            FromAddressLine1 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline1"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline1"].ToString() : string.Empty;
        //            FromAddressLine2 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline2"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline2"].ToString() : string.Empty;
        //            FromAddressLine3 = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FormAddressline3"].ToString()) ? dsAddress.Tables[1].Rows[0]["FormAddressline3"].ToString() : string.Empty;
        //            FromCompanyName = !string.IsNullOrEmpty(dsAddress.Tables[1].Rows[0]["FromCompanyName"].ToString()) ? dsAddress.Tables[1].Rows[0]["FromCompanyName"].ToString() : string.Empty;
        //        }


        //        Reference_WholeSaler = ToCompanyName + " - " + Reference + " " + NoOfStc + "@" + StcPrice + Gst;

        //        if (IsWholeSaler)
        //        {
        //            viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance_WholeSaler.rdlc";
        //        }
        //        else
        //        {
        //            viewer.LocalReport.ReportPath = serverPath + "Reports\\StcRemittance.rdlc"; //@"Reports//StcRemittance.rdlc";
        //        }
        //        viewer.LocalReport.EnableExternalImages = true;
        //        // LocalReport.EnableExternalImages = true;
        //        ReportDataSource rds1 = new ReportDataSource("dt", dtTest);
        //        viewer.LocalReport.DataSources.Add(rds1);

        //        viewer.LocalReport.SetParameters(new ReportParameter("PaymentDate", PaymentDate));
        //        //viewer.LocalReport.SetParameters(new ReportParameter("Reference", Reference));
        //        viewer.LocalReport.SetParameters(new ReportParameter("SentDate", SentDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ABN", ABN));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceDate", InvoiceDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceTotal", InvoiceTotal));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AmountPaid", AmountPaid));
        //        viewer.LocalReport.SetParameters(new ReportParameter("Stillwing", Stillwing));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyName", CompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));

        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline1", FromAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline2", FromAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FormAddressline3", FromAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));
        //        if (IsWholeSaler)
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
        //        else
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference", Reference));

        //        viewer.LocalReport.Refresh();

        //        byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //        //Save Report

        //        pathName = ByteArrayToFile("Remittance_" + STCInvoicePaymentID, bytes, JobId);
        //        pathList.Add(STCInvoicePaymentID, pathName);

        //        if (!string.IsNullOrEmpty(STCInvoicePaymentID) && !string.IsNullOrEmpty(pathName))
        //            lstFilePath.Add(new Entity.STCInvoicePayment { STCInvoicePaymentID = Convert.ToInt64(STCInvoicePaymentID), FilePath = pathName, SolarCompanyId = solarCompanyId, SolarCompanyUserId = solarCompanyUserId, ResellerUserId = resellerUserId, RankNumber = rankNum });

        //        //completionEvents[pageIndex].Set();
        //        //});

        //        //tasks[pageIndex].Start();

        //        //index++;
        //    }

        //    //Task.WaitAll(tasks);

        //    if (lstFilePath.Count > 0)
        //    {
        //        string STCInvoicePaymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilePath);
        //        response = _stcInvoiceServiceBAL.UpdateFilePath(STCInvoicePaymentJson);

        //        //for (int i = 0; i < lstFilePath.Count; i++)
        //        //{
        //        //    string emailId = string.Empty;
        //        //    int toUserId = 0;
        //        //    int fromUserId = 0;
        //        //    //if (!string.IsNullOrEmpty(Convert.ToString(lstFilePath[i].SolarCompanyUserId)))
        //        //    //{
        //        //    //    int.TryParse(QueryString.GetValueFromQueryString(Convert.ToString(lstFilePath[i].SolarCompanyUserId), "id"), out toUserId);
        //        //    //}
        //        //    var dsToUsers = _userBAL.GetUserById(lstFilePath[i].SolarCompanyUserId);
        //        //    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

        //        //    //if (!string.IsNullOrEmpty(Convert.ToString(lstFilePath[i].ResellerUserId)))
        //        //    //{
        //        //    //    int.TryParse(QueryString.GetValueFromQueryString(Convert.ToString(lstFilePath[i].ResellerUserId), "id"), out fromUserId);
        //        //    //}
        //        //    var dsFromUsers = _userBAL.GetUserById(lstFilePath[i].ResellerUserId);
        //        //    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

        //        //    string newFileName = "Remittance_" + lstFilePath[i].RankNumber + ".pdf";
        //        //    bool mailStatus = sendRemittanceFile(lstFilePath[i].FilePath, toUserDetail.Email, FromUserDetail.Email, newFileName, ProjectSession.LoggedInName, toUserDetail.FirstName + " " + toUserDetail.LastName);
        //        //}
        //    }
        //    return response;
        //}


        /// <summary>
        /// Get file path
        /// </summary>
        /// <param name="_FileName"></param>
        /// <param name="_ByteArray"></param>
        /// <param name="jobID"></param>
        /// <returns>ActionResult</returns>
        //public String ByteArrayToFile(string _FileName, byte[] _ByteArray, string jobID)
        //{
        //    try
        //    {

        //        string physicalPath = ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;
        //        string filePath = "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName + ".pdf";
        //        if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
        //            Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
        //        // Open file for reading

        //        System.IO.File.WriteAllBytes(physicalPath + ".pdf", _ByteArray);

        //        //System.IO.FileStream _FileStream =
        //        //   new System.IO.FileStream(physicalPath, System.IO.FileMode.Create,
        //        //                            System.IO.FileAccess.Write);
        //        //// Writes a block of bytes to this stream using data from
        //        //// a byte array.
        //        //_FileStream.Write(_ByteArray, 0, _ByteArray.Length);

        //        //// close file stream
        //        //_FileStream.Close();

        //        return filePath;
        //    }
        //    catch (Exception _Exception)
        //    {
        //        // Error
        //        Console.WriteLine("Exception caught in process: {0}",
        //                          _Exception.ToString());
        //    }

        //    // error occured, return false
        //    return string.Empty;
        //}

        /// <summary>
        /// Create STC report
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="ExportType"></param>
        /// <param name="STCJobDetailsID"></param>
        /// <param name="InvoiceNo"></param>
        /// <param name="solarCompanyId"></param>
        /// <returns>ActionResult</returns>
        //public String CreateStcReport(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, bool RegenerateRemittanceFile = false)
        //{

        //    Microsoft.Reporting.WebForms.Warning[] warnings;
        //    string[] streamIds;
        //    string mimeType = string.Empty;
        //    string encoding = string.Empty;
        //    string extension = string.Empty;
        //    ReportViewer viewer = new ReportViewer();
        //    //XmlDocument oXD = new XmlDocument();
        //    //oXD.Load(Server.MapPath("/Reports/InvoiceSTC.rdlc"));
        //    STCInvoice stcinvoice = new STCInvoice();
        //    DataSet ds = new DataSet();
        //    int report = 1;
        //    string RefNumber = string.Empty;
        //    string CompanyABN = string.Empty;
        //    string CompanyABNReseller = string.Empty;
        //    string InoviceDate = string.Empty;
        //    string InvoiceNumber = string.Empty;
        //    string AmountDue = string.Empty;
        //    string Total = string.Empty;
        //    string DueDate = string.Empty;
        //    string FromAddressLine1 = string.Empty;
        //    string FromAddressLine2 = string.Empty;
        //    string FromAddressLine3 = string.Empty;
        //    string ToAddressLine1 = string.Empty;
        //    string ToAddressLine2 = string.Empty;
        //    string ToAddressLine3 = string.Empty;
        //    string LogoPath = string.Empty;
        //    string InvoiceFooter = string.Empty;
        //    string JobDescription = string.Empty;
        //    string JobDate = string.Empty;
        //    string JobAddress = string.Empty;
        //    string JobTitle = string.Empty;
        //    string Logo = string.Empty;
        //    string ItemCode = string.Empty;
        //    string jobid = string.Empty;
        //    string IsStcInvoice = string.Empty;
        //    string ToName = string.Empty;
        //    string FromName = string.Empty;
        //    string FromCompanyName = string.Empty;
        //    string ToCompanyName = string.Empty;

        //    bool IsJobDescription = false;
        //    bool IsJobAddress = false;
        //    bool IsJobDate = false;
        //    bool IsTitle = false;
        //    bool IsName = false;
        //    bool IsTaxInclusive = false;
        //    decimal? TaxRate = 0;
        //    int SettingUserId = 0;
        //    string path = string.Empty;

        //    string AccountName = string.Empty;
        //    string BSB = string.Empty;
        //    string AccountNumber = string.Empty;
        //    string Reference_WholeSaler = string.Empty;
        //    string Gst = string.Empty;
        //    decimal NoOfStc = 0;
        //    decimal StcPrice = 0;
        //    string AccountNameReseller = string.Empty;
        //    string BSBReseller = string.Empty;
        //    string AccountNumberReseller = string.Empty;
        //    string STCPVDCode = string.Empty;
        //    bool IsWholeSaler = false;
        //    string resellerId = string.Empty;

        //    Entity.Settings.Settings settings = new Entity.Settings.Settings();

        //    settings = GetSettingsData(solarCompanyId);
        //    IsJobDescription = settings.IsJobDescription;
        //    IsJobAddress = settings.IsJobAddress;
        //    IsJobDate = settings.IsJobDate;
        //    IsTitle = settings.IsTitle;
        //    IsName = settings.IsName;
        //    IsTaxInclusive = settings.IsTaxInclusive;
        //    TaxRate = settings.TaxRate;
        //    //SettingUserId = settings.UserId;

        //    //SettingUserId = 544;
        //    //Logo = "gogreen.jpg";

        //    //var invoiceSetting = _jobInvoiceDetail.getInvoiceSetting();
        //    //if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
        //    //{
        //    //    DueDate = invoiceSetting.Item1.ToString("dd MMMM yyyy");
        //    //}


        //    ds = _stcInvoiceServiceBAL.GetStcInvoice(STCJobDetailsID, IsJobAddress, IsJobDate, IsJobDescription, IsTitle, IsName, DateTime.Now, InvoiceNo);
        //    DataTable dt = new DataTable();
        //    if (ds != null && ds.Tables.Count > 0)
        //    {

        //        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            RefNumber = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["RefNumber"].ToString()) ? ds.Tables[0].Rows[0]["RefNumber"].ToString() : string.Empty;
        //        }
        //        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
        //        {
        //            CompanyABN = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABN"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABN"].ToString() : string.Empty;
        //            CompanyABNReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString() : string.Empty;
        //            Logo = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["ResellerLogo"].ToString()) ? ds.Tables[1].Rows[0]["ResellerLogo"].ToString() : string.Empty;
        //            SettingUserId = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["SettingUserId"].ToString()) ? Convert.ToInt32(ds.Tables[1].Rows[0]["SettingUserId"]) : 0;
        //            InvoiceFooter = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["InvoiceFooter"].ToString()) ? ds.Tables[1].Rows[0]["InvoiceFooter"].ToString() : string.Empty;

        //            AccountName = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountName"].ToString()) ? ds.Tables[1].Rows[0]["AccountName"].ToString() : string.Empty;
        //            BSB = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["BSB"].ToString()) ? Convert.ToString(ds.Tables[1].Rows[0]["BSB"]) : string.Empty;
        //            AccountNumber = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNumber"].ToString()) ? ds.Tables[1].Rows[0]["AccountNumber"].ToString() : string.Empty;

        //            AccountNameReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNameReseller"].ToString()) ? ds.Tables[1].Rows[0]["AccountNameReseller"].ToString() : string.Empty;
        //            BSBReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["BSBReseller"].ToString()) ? Convert.ToString(ds.Tables[1].Rows[0]["BSBReseller"]) : string.Empty;
        //            AccountNumberReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNumberReseller"].ToString()) ? ds.Tables[1].Rows[0]["AccountNumberReseller"].ToString() : string.Empty;
        //            IsWholeSaler = Convert.ToBoolean(ds.Tables[1].Rows[0]["IsWholeSaler"]);
        //            resellerId = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["ResellerID"].ToString()) ? ds.Tables[1].Rows[0]["ResellerID"].ToString() : string.Empty;

        //            if (Logo != "" && Logo != null)
        //            {
        //                var LogoP = Path.Combine(ProjectSession.UploadedDocumentPath + "\\UserDocuments" + "\\" + SettingUserId, Logo);
        //                LogoPath = new Uri(LogoP).AbsoluteUri;
        //            }
        //            else
        //            {
        //                LogoPath = "";
        //            }
        //        }
        //        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        //        {
        //            InoviceDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["InoviceDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["InoviceDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            InvoiceNumber = InvoiceNo;
        //            jobid = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["jobid"].ToString()) ? ds.Tables[2].Rows[0]["jobid"].ToString() : string.Empty;
        //            DueDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["DueDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["DueDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //        }

        //        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
        //        {
        //            Gst = ds.Tables[3].Rows[0]["IsTaxInclusive"].ToString() == "True" ? "+Gst" : "";
        //            NoOfStc = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["Quantity"].ToString()) ? Convert.ToDecimal(ds.Tables[3].Rows[0]["Quantity"]) : 0;
        //            StcPrice = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["Sale"].ToString()) ? Convert.ToDecimal(ds.Tables[3].Rows[0]["Sale"]) : 0;
        //            STCPVDCode = !string.IsNullOrEmpty(ds.Tables[3].Rows[0]["STCPVDCode"].ToString()) ? ds.Tables[3].Rows[0]["STCPVDCode"].ToString() : string.Empty;
        //            dt = ds.Tables[3];
        //        }
        //        if (ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
        //        {
        //            ToAddressLine1 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine1"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
        //            ToAddressLine2 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine2"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
        //            ToAddressLine3 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine3"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
        //            ToCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToCompanyName"].ToString() : string.Empty;

        //        }
        //        if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
        //        {
        //            FromAddressLine1 = (ds.Tables[5].Rows[0]["FormAddressline1"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline1"].ToString();
        //            FromAddressLine2 = (ds.Tables[5].Rows[0]["FormAddressline2"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline2"].ToString();
        //            FromAddressLine3 = (ds.Tables[5].Rows[0]["FormAddressline3"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline3"].ToString();
        //            FromCompanyName = (ds.Tables[5].Rows[0]["FromCompanyName"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FromCompanyName"].ToString();
        //        }
        //        if (ds.Tables[6] != null && ds.Tables[6].Rows.Count > 0)
        //        {
        //            JobDate = !string.IsNullOrEmpty(ds.Tables[6].Rows[0][0].ToString()) ? Convert.ToDateTime(ds.Tables[6].Rows[0][0]).ToString("dd MMMM yyyy") : string.Empty;
        //        }
        //        if (ds.Tables[7] != null && ds.Tables[7].Rows.Count > 0)
        //        {
        //            JobDescription = !string.IsNullOrEmpty(ds.Tables[7].Rows[0][0].ToString()) ? ds.Tables[7].Rows[0][0].ToString() : string.Empty;
        //        }

        //        if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
        //        {
        //            JobTitle = !string.IsNullOrEmpty(ds.Tables[8].Rows[0][0].ToString()) ? ds.Tables[8].Rows[0][0].ToString() : string.Empty;
        //        }
        //        if (ds.Tables[9] != null && ds.Tables[9].Rows.Count > 0)
        //        {
        //            JobAddress = !string.IsNullOrEmpty(ds.Tables[9].Rows[0][0].ToString()) ? ds.Tables[9].Rows[0][0].ToString() : string.Empty;
        //        }

        //        Reference_WholeSaler = ToCompanyName + " - " + RefNumber + " " + STCPVDCode + " " + NoOfStc + "@" + StcPrice + Gst;

        //        if (IsWholeSaler)
        //            viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC_WholeSaler.rdlc";
        //        else
        //            viewer.LocalReport.ReportPath = @"Reports//InvoiceSTC.rdlc";

        //        viewer.LocalReport.EnableExternalImages = true;
        //        ReportDataSource rds1 = new ReportDataSource("dt", dt);
        //        viewer.LocalReport.DataSources.Add(rds1);

        //        viewer.LocalReport.SetParameters(new ReportParameter("RefNumber", RefNumber));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyABN", CompanyABN));
        //        viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", CompanyABNReseller));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InoviceDate", InoviceDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceNumber", InvoiceNumber));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AmountDue", AmountDue));
        //        viewer.LocalReport.SetParameters(new ReportParameter("Total", Total));
        //        viewer.LocalReport.SetParameters(new ReportParameter("DueDate", DueDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine1", FromAddressLine1));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine2", FromAddressLine2));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine3", FromAddressLine3));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobDate", JobDate));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobDescription", JobDescription));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobTitle", JobTitle));
        //        viewer.LocalReport.SetParameters(new ReportParameter("JobAddress", JobAddress));
        //        viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));
        //        viewer.LocalReport.SetParameters(new ReportParameter("InvoiceFooter", InvoiceFooter));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToName", ToName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromName", FromName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("IsStcInvoice", IsStcInvoice));
        //        viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));

        //        viewer.LocalReport.SetParameters(new ReportParameter("AccountName", AccountName));
        //        viewer.LocalReport.SetParameters(new ReportParameter("BSB", BSB));
        //        viewer.LocalReport.SetParameters(new ReportParameter("AccountNumber", AccountNumber));
        //        if (IsWholeSaler)
        //        {
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountNameReseller", AccountNameReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("BSBReseller", BSBReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountNumberReseller", AccountNumberReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("Reference_WholeSaler", Reference_WholeSaler));
        //        }
        //        viewer.LocalReport.Refresh();
        //        byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        //        //Save Report
        //        path = ByteArrayToFile(InvoiceNo, bytes, jobid);

        //        if (RegenerateRemittanceFile)
        //        {
        //            DataSet remittanceData = _stcInvoiceServiceBAL.RegenerateRemittanceFile(Convert.ToInt32(resellerId), InvoiceNo);
        //            GenerateRemittance(remittanceData, resellerId);
        //        }

        //        // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
        //        //Response.Buffer = true;
        //        //Response.Clear();
        //        //Response.ClearHeaders();
        //        //Response.ContentType = mimeType;
        //        //Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "." + extension);
        //        //Response.BinaryWrite(bytes); // create the file
        //        //Response.End(); // send it to the client to download
        //    }

        //    return path;
        //}

        /// <summary>
        /// CreateCSV
        /// </summary>
        /// <param name="STCInvoiceID"></param>
        public void CreateCSV(String STCInvoiceID)
        {
            Server.ClearError();

            string DueDate = string.Empty;
            var invoiceSetting = _jobInvoiceDetail.GetInvoiceSetting();
            if (invoiceSetting.Item1 != null && invoiceSetting.Item1 != DateTime.MinValue)
            {
                DueDate = invoiceSetting.Item1.ToString("dd MMMM yyyy");
            }

            DataSet ds = _stcInvoiceServiceBAL.GetStcCSV(STCInvoiceID, DateTime.Now);
            DataTable dt = new DataTable();
            //Build the CSV file data as a Comma separated string.
            string csv = string.Empty;
            FileInfo objFileInfo = new FileInfo(Path.Combine(ProjectSession.ProofDocuments + "\\" + "SalesInvoice.csv"));
            if (objFileInfo.Exists)
            {
                objFileInfo.Delete();
            }
            CsvOptions objCsvOptions = new CsvOptions("STCInvoice", ',', 43);
            objCsvOptions.IncludeHeaderNames = true;
            objCsvOptions.DateFormat = "dd/MM/yyyy";
            ds.Tables[0].AsEnumerable().ToList().ForEach(a => { a["STCInvoiceId"] = QueryString.QueryStringEncode("id=" + Convert.ToString(a["STCInvoiceId"])); });
            FileHelpers.CsvEngine.DataTableToCsv(ds.Tables[0], Path.Combine(ProjectSession.ProofDocuments + "\\" + "SalesInvoice.csv"), objCsvOptions);
            ////Download the CSV file.
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.BufferOutput = true;
            Response.ContentType = "application/CSV";
            Response.AddHeader("Content-Disposition", "attachment; filename= SalesInvoice.csv");
            Response.TransmitFile(Path.Combine(ProjectSession.ProofDocuments + "\\" + "SalesInvoice.csv"));
            //Response.Flush();
            Response.End();

        }

        /// <summary>
        /// Gets the STC invoice.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult GetSTCInvoice(string id)
        {
            Int64 invoiceId = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            STCInvoice stcInVoice = _stcInvoiceServiceBAL.GetSTCInvoiceByInvoiceID(invoiceId);

            SystemEnums.STCSettlementTerm stcSet = (SystemEnums.STCSettlementTerm)Convert.ToInt32(stcInVoice.SettlementTerms);
            string description = string.Empty;
            if (stcSet != null && stcSet > 0)
            {
                FieldInfo fi = stcSet.GetType().GetField(stcSet.ToString());

                System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attributes != null &&
                    attributes.Length > 0)
                    description = attributes[0].Description;
                else
                    description = stcSet.ToString();
            }
            stcInVoice.SettlementTermDescription = description;
            ViewBag.PaymentStatus = _stcInvoiceServiceBAL.GetSTCPaymentStatus().Tables[0].AsEnumerable().Select(
                                                                                                                 t => new SelectListItem()
                                                                                                                 {
                                                                                                                     Text = t.Field<string>("PaymentStatusName"),
                                                                                                                     Value = t.Field<int>("PaymentStatusID").ToString()
                                                                                                                 }).ToList();

            Session["STCInvoice"] = stcInVoice;

            return PartialView("_AddInvoicing", stcInVoice);
        }

        /// <summary>
        /// Get settings data
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <returns>ActionResult</returns>
        //public Entity.Settings.Settings GetSettingsData(string SolarCompanyId)
        //{
        //    SettingsBAL settingsBAL = new SettingsBAL();
        //    Entity.Settings.Settings settings = new Entity.Settings.Settings();
        //    int? solarCompanyId = (!string.IsNullOrEmpty(SolarCompanyId)) ? Convert.ToInt32(SolarCompanyId) : 0;

        //    settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ProjectSession.LoggedInUserId, 4, solarCompanyId, ProjectSession.ResellerId);
        //    return settings;
        //}

        /// <summary>
        /// Saves the invoice data.
        /// </summary>
        /// <param name="stcInvoice">The STC invoice.</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult SaveInvoiceData(STCInvoice stcInvoice)
        {
            //try
            //{
            bool IsChanged = false;
            STCInvoice preStcInvoice = Session["STCInvoice"] as STCInvoice;

            if (preStcInvoice.IsGst != stcInvoice.IsGst || preStcInvoice.STCValue != stcInvoice.STCValue || preStcInvoice.STCAmount != stcInvoice.STCAmount)
            {
                IsChanged = true;
            }

            if (IsChanged)
            {
                _stcInvoiceServiceBAL.UpdateGeneratedSTCInvoice(stcInvoice.JobID, stcInvoice.STCJobDetailsID, stcInvoice.STCInvoiceID, stcInvoice.IsGst, stcInvoice.STCValue, stcInvoice.PaymentStatusID, Convert.ToInt32(stcInvoice.SettlementTerms), stcInvoice.STCAmount, stcInvoice.Notes, stcInvoice.Total);
                //CreateStcReport("FormbotStcInvoivce", "Pdf", stcInvoice.STCJobDetailsID, stcInvoice.STCInvoiceNumber, "0");
                _generateStcReportBAL.CreateStcReportNew("FormbotStcInvoivce", "Pdf", stcInvoice.STCJobDetailsID, stcInvoice.STCInvoiceNumber, "0", "4", ProjectSession.LoggedInUserId, ProjectSession.ResellerId, false);
            }
            else
            {
                _stcInvoiceServiceBAL.UpdateGeneratedSTCInvoice(stcInvoice.JobID, stcInvoice.STCJobDetailsID, stcInvoice.STCInvoiceID, stcInvoice.IsGst, stcInvoice.STCValue, stcInvoice.PaymentStatusID, Convert.ToInt32(stcInvoice.SettlementTerms), stcInvoice.STCAmount, stcInvoice.Notes, stcInvoice.Total);
            }
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Get payment status
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public JsonResult GetPaymentStatus()
        {
            List<SelectListItem> items = _stcInvoiceServiceBAL.GetPaymentStatus().Select(a => new SelectListItem { Text = a.PaymentStatusName, Value = a.PaymentStatusID.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Bulks the change payment status.
        /// </summary>
        /// <param name="paymentstatus">The paymentstatus.</param>
        /// <param name="stcinvoiceids">The stcinvoiceids.</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult BulkChangePaymentStatus(string paymentstatus, string stcinvoiceids)
        {

            int PaymentStatusID = Convert.ToInt32(paymentstatus);
            DataSet ds = _stcInvoiceServiceBAL.BulkChangePaymentStatus(PaymentStatusID, stcinvoiceids);
            List<string> StcInvoiceIds = stcinvoiceids.Split(',').ToList();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drBulkChangeStatus in ds.Tables[0].Rows)
                {
                    int JobID = Convert.ToInt32(drBulkChangeStatus["JobID"].ToString());
                    string STCInvoiceNumber = drBulkChangeStatus["STCInvoiceNumber"].ToString();
                    string status = Common.GetDescription((SystemEnums.InvoiceStatus)PaymentStatusID, "");
                    string JobHistoryMessage = "changed STC Invoice status to <b style=\"color:black\">" + status + "</b>";
                    Common.SaveJobHistorytoXML(JobID, JobHistoryMessage, "Invoicing", "BulkChangeInvoicePaymentStatus", ProjectSession.LoggedInName, false);
                }
            }

            return this.Json(new { success = true });
        }

        /// <summary>
        /// Remove Selected STC Invoice
        /// </summary>
        /// <param name="stcinvoiceids"></param>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult> RemoveSelectedSTCInvoice(string stcinvoiceids, string xeroInvoiceIdsArray)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                List<STCInvoice> lstXeroInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(xeroInvoiceIdsArray);

                bool isXeroItem = lstXeroInvoiceIds.AsEnumerable().Any(a => a.XeroInvoiceId != "0");
                List<string> lstXeroInvoiceID = lstXeroInvoiceIds.Where(x => x.XeroInvoiceId == "0").Select(x => x.XeroInvoiceId).ToList();
                //Task<XeroOAuth2Token> xeroToken = Task.Run(async () => await xeroApiHelper.CheckToken());
                //xeroToken.Wait();
                if (lstXeroInvoiceIds.Count == 0 || !isXeroItem
                    || (lstXeroInvoiceID.Count > 0 || isXeroItem))
                {
                    // remove drafts from xero
                    //var xeroApiHelper = new ApplicationSettingsTest();
                    //if (!TokenUtilities.TokenExists())
                    //{
                    //    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                    //}
                    List<string> lstNotDeletedInvoices = new List<string>();
                    List<STCInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(stcinvoiceids);
                    if (isXeroItem)
                    {
                        // remove drafts from xero
                        var xeroApiHelper = new ApplicationSettingsTest();
                        if (!TokenUtilities.TokenExists())
                        {
                            return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                        }
                        var token = TokenUtilities.GetStoredToken();
                        string accessToken = token.AccessToken;
                        string xeroTenantId = token.Tenants[0].TenantId.ToString();
                        lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);
                    }
                    //    var token = TokenUtilities.GetStoredToken();
                    //string accessToken = token.AccessToken;
                    //string xeroTenantId = token.Tenants[0].TenantId.ToString();
                    //List<string> lstNotDeletedInvoices = new List<string>();
                    //lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);
                    //List<STCInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(stcinvoiceids);
                    if (isXeroItem)
                    {
                        // remove drafts from xero
                        var xeroApiHelper = new ApplicationSettingsTest();
                        if (!TokenUtilities.TokenExists())
                        {
                            return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                        }
                        var token = TokenUtilities.GetStoredToken();
                        string accessToken = token.AccessToken;
                        string xeroTenantId = token.Tenants[0].TenantId.ToString();
                        lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);

                    }

                    if (lstNotDeletedInvoices.Count == 0)
                    {
                        var values = lstInvoiceIds.Select(x => x.STCInvoiceID).ToList();
                        _stcInvoiceServiceBAL.RemoveSelectedSTCInvoice(string.Join(",", values));
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        for (int i = 0; i < lstInvoiceIds.Count; i++)
                        {
                            jobHistory.JobID = Convert.ToInt32(lstInvoiceIds[i].JobID);
                            jobHistory.stcInvoiceNumber = Convert.ToString(lstInvoiceIds[i].STCInvoiceNumber);
                            jobHistory.HistoryMessage = "";
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.RemoveInvoiceFromGreenbot);
                            string JobHistoryMessage = "has removed <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "RemoveInvoiceFromGreenbot", ProjectSession.LoggedInName, false);
                        }
                        var lstStcJobDetailsId = lstInvoiceIds.Select(x => x.STCJobDetailsID).ToList();
                        string stcjobids = string.Join(",", lstStcJobDetailsId);
                        List<int> peakpaySTCJobIds = new List<int>();
                        DataTable dtinvoiceCount = _createJob.GetSTCInvoiceAndCreditNoteCount(stcjobids);
                        if (dtinvoiceCount.Rows.Count > 0)
                        {
                            peakpaySTCJobIds = dtinvoiceCount.AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsID")).Distinct().ToList();
                            for (int j = 0; j < dtinvoiceCount.Rows.Count; j++)
                            {
                                SortedList<string, string> data = new SortedList<string, string>();
                                string IsInvoiced = dtinvoiceCount.Rows[j]["IsInvoiced"].ToString();
                                string IsCreditNote = dtinvoiceCount.Rows[j]["IsCreditNote"].ToString();
                                string STCInvoiceCount = dtinvoiceCount.Rows[j]["STCInvoiceCount"].ToString();
                                data.Add("IsInvoiced", IsInvoiced);
                                data.Add("IsCreditNote", IsCreditNote);
                                data.Add("STCInvoiceCount", STCInvoiceCount);
                                await CommonBAL.SetCacheDataForSTCSubmission(null, Convert.ToInt32(dtinvoiceCount.Rows[j]["JobID"]), data, true, false, true);

                                //data = new SortedList<string, string>();
                                //data.Add("STCInvoiceCnt", STCInvoiceCount);
                                //data.Add("PaymentDate", Convert.ToString(dtinvoiceCount.Rows[j]["STCInvoiceCreatedDate"]));
                                //data.Add("IsInvoiced", Convert.ToString(dtinvoiceCount.Rows[j]["InvoiceStatus"]));
                                //await CommonBAL.SetCacheDataForPeakPay(null, Convert.ToInt32(dtinvoiceCount.Rows[j]["JobID"]), data);
                            }
                        }
                        
                        if(peakpaySTCJobIds.Count>0)
                            await CommonBAL.SetCacheDataForPeakPayFromJobId("", string.Join(",",peakpaySTCJobIds));
                        //foreach (var id in lstStcJobDetailsId)
                        //    CommonBAL.SetCacheDataForSTCSubmission(id, 0);

                        return this.Json(new { success = true });
                    }
                    else
                    {
                        List<string> notdeletedStcInvoiceIDs = new List<string>();
                        List<string> stcJobDetailsIds = new List<string>();
                        for (int i = 0; i < lstNotDeletedInvoices.Count; i++)
                        {
                            notdeletedStcInvoiceIDs.Add(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).STCInvoiceID.ToString());
                            stcJobDetailsIds.Add(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).STCJobDetailsID.ToString());

                            JobHistory jobHistory = new JobHistory();
                            jobHistory.Name = ProjectSession.LoggedInName;

                            jobHistory.JobID = Convert.ToInt32(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).JobID);

                            jobHistory.stcInvoiceNumber = lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).STCInvoiceNumber;
                            jobHistory.HistoryMessage = "which is not successfull because of invoice found on xero with voided, paid or approved.";
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.RemoveInvoiceFromGreenbot);
                            string JobHistoryMessage = "has removed <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + " " + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "RemoveInvoiceFromGreenbot", ProjectSession.LoggedInName, false);
                            lstInvoiceIds.Remove(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])));


                        }

                        if (lstInvoiceIds.Count > 0)
                        {
                            var values = lstInvoiceIds.Select(x => x.XeroInvoiceId).ToList();
                            _stcInvoiceServiceBAL.RemoveSelectedSTCInvoice(string.Join(",", values));
                            JobHistory jobHistory = new JobHistory();
                            jobHistory.Name = ProjectSession.LoggedInName;
                            for (int i = 0; i < lstInvoiceIds.Count; i++)
                            {
                                jobHistory.JobID = Convert.ToInt32(lstInvoiceIds.Select(x => x.JobID));

                                jobHistory.stcInvoiceNumber = lstInvoiceIds.Select(x => x.STCInvoiceNumber).ToString();
                                jobHistory.HistoryMessage = "";
                                //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.RemoveInvoiceFromGreenbot);
                                string JobHistoryMessage = "has removed <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + jobHistory.HistoryMessage;
                                Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "RemoveInvoiceFromGreenbot", ProjectSession.LoggedInName, false);
                            }
                            var lstStcJobDetailsId = lstInvoiceIds.Select(x => x.STCJobDetailsID).ToList();
                            string stcjobids = string.Join(",", lstStcJobDetailsId);
                            DataTable dtinvoiceCount = _createJob.GetSTCInvoiceAndCreditNoteCount(stcjobids);
                            List<int> peakpaySTCJobIds = new List<int>();
                            if (dtinvoiceCount.Rows.Count > 0)
                            {
                                peakpaySTCJobIds = dtinvoiceCount.AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsID")).Distinct().ToList();
                                for (int j = 0; j < dtinvoiceCount.Rows.Count; j++)
                                {
                                    SortedList<string, string> data = new SortedList<string, string>();
                                    string IsInvoiced = dtinvoiceCount.Rows[j]["IsInvoiced"].ToString();
                                    string IsCreditNote = dtinvoiceCount.Rows[j]["IsCreditNote"].ToString();
                                    string STCInvoiceCount = dtinvoiceCount.Rows[j]["STCInvoiceCount"].ToString();
                                    data.Add("IsInvoiced", IsInvoiced);
                                    data.Add("IsCreditNote", IsCreditNote);
                                    data.Add("STCInvoiceCount", STCInvoiceCount);
                                    await CommonBAL.SetCacheDataForSTCSubmission(null, Convert.ToInt32(dtinvoiceCount.Rows[j]["JobID"]), data, true, false, true);
                                }
                            }
                            if (peakpaySTCJobIds.Count > 0)
                                await CommonBAL.SetCacheDataForPeakPayFromJobId("", string.Join(",", peakpaySTCJobIds));
                            //foreach (var id in lstStcJobDetailsId)
                            //    CommonBAL.SetCacheDataForSTCSubmission(id, 0);
                        }

                        string strNotDeletedInvoices = string.Join(", ", lstNotDeletedInvoices.ToArray());
                        string strnotdeletedStcInvoiceIDs = string.Join(",", notdeletedStcInvoiceIDs.ToArray());
                        string strStcJobDetailsIds = string.Join(",", stcJobDetailsIds.ToArray());
                        return this.Json(new { success = true, InvoiceNum = strNotDeletedInvoices, count = lstNotDeletedInvoices.Count, notdeletedStcInvoiceIDs = strnotdeletedStcInvoiceIDs, StcJobDetailsIds = strStcJobDetailsIds });
                    }
                }
                else
                {
                    List<string> lstNotDeletedInvoices = new List<string>();
                    List<string> lstnotdeletedStcInvoiceIDs = new List<string>();
                    List<string> lststcJobDetailsIds = new List<string>();
                    foreach (var item in lstXeroInvoiceIds)
                    {
                        lstNotDeletedInvoices.Add(item.STCInvoiceNumber);
                        lstnotdeletedStcInvoiceIDs.Add(item.STCInvoiceID.ToString());
                        lststcJobDetailsIds.Add(item.STCJobDetailsID.ToString());
                    }
                    string strNotDeletedInvoices = string.Join(", ", lstNotDeletedInvoices.ToArray());
                    string strnotdeletedStcInvoiceIDs = string.Join(",", lstnotdeletedStcInvoiceIDs.ToArray());
                    string strStcJobDetailsIds = string.Join(",", lststcJobDetailsIds.ToArray());
                    return Json(new { success = true, InvoiceNum = strNotDeletedInvoices, count = lstXeroInvoiceIds.Count, notdeletedStcInvoiceIDs = strnotdeletedStcInvoiceIDs, StcJobDetailsIds = strStcJobDetailsIds });
                }

            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
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

        /// <summary>
        /// Mark UnMarkSelectedAsSentForPayment
        /// </summary>
        /// <param name="markbit"></param>
        /// <param name="stcinvoiceids"></param>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult MarkUnMarkSelectedAsSentForPayment(string markbit, string stcinvoiceids, string xeroInvoiceIdsArray)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                bool IsInvoiced = markbit == "0" ? false : true;

                List<string> lstNotDeletedInvoices = new List<string>();
                List<STCInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(stcinvoiceids);
                List<STCInvoice> lstXeroInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(xeroInvoiceIdsArray);
                //var xeroApiHelper = new ApplicationSettingsTest();
                //if (!TokenUtilities.TokenExists())
                //{
                //    return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                //}
                //var token = TokenUtilities.GetStoredToken();
                //string accessToken = token.AccessToken;
                //string xeroTenantId = token.Tenants[0].TenantId.ToString();

                //AccountingApi accountingApi = new AccountingApi();
                // remove drafts from xero
                //if (lstXeroInvoiceIds.Count > 0 && markbit == "0")
                //{

                //if (token != null)
                //        lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);
                //    else
                //        return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                //}
                bool isXeroItem = lstXeroInvoiceIds.AsEnumerable().Any(a => a.XeroInvoiceId != "0");

                if (isXeroItem)
                {
                    var xeroApiHelper = new ApplicationSettingsTest();
                    if (!TokenUtilities.TokenExists())
                    {
                        return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                    }
                    var token = TokenUtilities.GetStoredToken();
                    string accessToken = token.AccessToken;
                    string xeroTenantId = token.Tenants[0].TenantId.ToString();

                    AccountingApi accountingApi = new AccountingApi();
                    // remove drafts from xero
                    if (lstXeroInvoiceIds.Count > 0 && markbit == "0")
                    {

                        if (token != null)
                            lstNotDeletedInvoices = RemoveDraftsFromXero(xeroInvoiceIdsArray);
                        else
                            return Json(new { status = false, error = "specified method is not supported." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (lstNotDeletedInvoices.Count == 0)
                {
                    var values = lstInvoiceIds.Select(x => x.STCInvoiceID).ToList();
                    _stcInvoiceServiceBAL.MarkUnMarkSelectedAsSentForPayment(IsInvoiced, string.Join(",", values));

                    JobHistory jobHistory = new JobHistory();
                    jobHistory.Name = ProjectSession.LoggedInName;
                    for (int i = 0; i < lstInvoiceIds.Count; i++)
                    {
                        jobHistory.JobID = lstInvoiceIds[i].JobID;
                        jobHistory.stcInvoiceNumber = lstInvoiceIds[i].STCInvoiceNumber;
                        jobHistory.HistoryMessage = "";
                        if (IsInvoiced == false)
                        {
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                            string JobHistoryMessage = "has unmarked <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "UnmarkInvoice", ProjectSession.LoggedInName, false);
                        }
                        else
                        {
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.MarkInvoice);
                            string JobHistoryMessage = "has marked <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "MarkInvoice", ProjectSession.LoggedInName, false);
                        }
                    }
                    return this.Json(new { success = true });
                }
                else
                {

                    List<string> notdeletedStcInvoiceIDs = new List<string>();
                    for (int i = 0; i < lstNotDeletedInvoices.Count; i++)
                    {
                        notdeletedStcInvoiceIDs.Add(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).STCInvoiceID.ToString());

                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;

                        jobHistory.JobID = lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).JobID;
                        jobHistory.stcInvoiceNumber = lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])).STCInvoiceNumber;
                        jobHistory.HistoryMessage = "which is not successfull because of invoice found on xero with voided, paid or approved";
                        //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                        string JobHistoryMessage = "has unmarked <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + " " + jobHistory.HistoryMessage;
                        Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "UnmarkInvoice", ProjectSession.LoggedInName, false);
                        lstInvoiceIds.Remove(lstInvoiceIds.First(item => item.STCInvoiceNumber.Equals(lstNotDeletedInvoices[i])));

                    }


                    if (lstInvoiceIds.Count > 0)
                    {
                        var values = lstInvoiceIds.Select(x => x.STCInvoiceID).ToList();
                        _stcInvoiceServiceBAL.MarkUnMarkSelectedAsSentForPayment(IsInvoiced, string.Join(",", values));
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        for (int i = 0; i < lstInvoiceIds.Count; i++)
                        {
                            jobHistory.JobID = lstInvoiceIds[i].JobID;
                            jobHistory.stcInvoiceNumber = lstInvoiceIds[i].STCInvoiceNumber;
                            jobHistory.HistoryMessage = "";
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                            string JobHistoryMessage = "has unmarked <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + " " + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "UnmarkInvoice", ProjectSession.LoggedInName, false);
                        }
                    }

                    string strNotDeletedInvoices = string.Join(", ", lstNotDeletedInvoices.ToArray());
                    string strnotdeletedStcInvoiceIDs = string.Join(",", notdeletedStcInvoiceIDs.ToArray());
                    return this.Json(new { success = true, InvoiceNum = strNotDeletedInvoices, count = lstNotDeletedInvoices.Count, notdeletedStcInvoiceIDs = strnotdeletedStcInvoiceIDs });
                }
                //to do
                /*
                 code to update status of invoices in XERO 
                 for synchronization with our system
                */
            }
            catch (RenewTokenException e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = "RenewTokenException" }, JsonRequestBehavior.AllowGet);
            }
            catch (XeroApiException e)
            {
                //Response.Write(e.Message);
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

        /// <summary>
        /// Remove remittance file
        /// </summary>
        /// <param name="stcinvoiceid"></param>
        /// <param name="filepath"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult RemoveRemittenceFile(string stcinvoiceid, string filepath)
        {
            //try
            //{
            Int64 STCInvoiceID = Convert.ToInt64(stcinvoiceid);
            _stcInvoiceServiceBAL.RemoveRemittenceFile(STCInvoiceID, filepath);
            DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + filepath, string.Empty));
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        /// <summary>
        /// Upload remittance file
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns>ActionResult</returns>
        public JsonResult UploadRemittenceFile(string jobID)
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    uploadStatus.Add(GetFileUpload(Request.Files[i], jobID));
                }
            }
            return Json(uploadStatus);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="jobID"></param>
        /// <returns>ActionResult</returns>
        public HelperClasses.UploadStatus GetFileUpload(HttpPostedFileBase fileUpload, string jobID)
        {
            HelperClasses.UploadStatus uploadStatus = new HelperClasses.UploadStatus();
            uploadStatus.FileName = Request.Files[0].FileName;

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(fileUpload.FileName);

                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;

                if (jobID != null)
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice" + "\\" + "Report" + "\\");
                    proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice" + "\\" + "Report" + "\\";
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                try
                {
                    string path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                    if (System.IO.File.Exists(path))
                    {
                        string orignalFileName = Path.GetFileNameWithoutExtension(path);
                        string fileExtension = Path.GetExtension(path);
                        string fileDirectory = Path.GetDirectoryName(path);
                        int i = 1;
                        while (true)
                        {
                            string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                            if (System.IO.File.Exists(renameFileName))
                                i++;
                            else
                            {
                                path = renameFileName;
                                break;
                            }
                        }
                        fileName = Path.GetFileName(path);
                    }
                    fileName = fileName.Replace("%", "$");
                    string mimeType = MimeMapping.GetMimeMapping(fileName);
                    if (fileUpload.FileName.Length > 70)
                    {
                        uploadStatus.Status = false;
                        uploadStatus.Message = "BigName";

                    }
                    else if (mimeType != "application/pdf")
                    {
                        uploadStatus.Status = false;
                        uploadStatus.Message = "NotPDF";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                        uploadStatus.Status = true;
                        uploadStatus.Message = "File Uploaded Successfully.";
                        uploadStatus.FileName = fileName;
                        uploadStatus.MimeType = mimeType;
                        uploadStatus.Path = proofDocumentsFolder + uploadStatus.FileName;
                    }
                }
                catch (Exception)
                {
                    uploadStatus.Status = false;
                    uploadStatus.Message = "An error occured while uploading. Please try again later.";
                }
            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.Message = "No data received";
            }

            return uploadStatus;
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {
            if (System.IO.File.Exists(path))
            {
                ////Delete all files from the Directory
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Remove drafts from xero
        /// </summary>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        private List<string> RemoveDraftsFromXero(string xeroInvoiceIdsArray)
        {
            List<string> lstNotDeletedInvoices = new List<string>();
            List<STCInvoice> lstXeroInvoiceIds = new List<STCInvoice>();
            lstXeroInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(xeroInvoiceIdsArray);

            //if (System.Web.HttpContext.Current.Session["oauth_token"] != null)
            //    Log("oauth_token :" + System.Web.HttpContext.Current.Session["oauth_token"].ToString());
            //if (System.Web.HttpContext.Current.Session["TokenKey"] != null)
            //    Log("TokenKey :" + System.Web.HttpContext.Current.Session["TokenKey"].ToString());
            //if (System.Web.HttpContext.Current.Session["TokenSecret"] != null)
            //    Log("TokenSecret :" + System.Web.HttpContext.Current.Session["TokenSecret"].ToString());

            if (lstXeroInvoiceIds.Count > 0)
            {
                for (int i = 0; i < lstXeroInvoiceIds.Count; i++)
                {
                    string xeroInvoiceId = lstXeroInvoiceIds[i].XeroInvoiceId;


                    if (xeroInvoiceId != "0")
                    {
                        if (!TokenUtilities.TokenExists())
                        {
                            //return Json(new { status = false, error = "invalid_grant" }, JsonRequestBehavior.AllowGet);
                        }
                        var token = TokenUtilities.GetStoredToken();
                        string accessToken = token.AccessToken;
                        string xeroTenantId = token.Tenants[0].TenantId.ToString();
                        var api = new AccountingApi();
                        AccountingApi accountingApi = new AccountingApi();
                        //    if (XeroApiHelper.xeroApiHelperSession != null && xeroInvoiceId != "0")
                        //{
                        //var api = XeroApiHelper.xeroApiHelperSession.CoreApi();

                        //var invoice = api.Invoices.Where("Id = " + new Guid(xeroInvoiceId)).Find().FirstOrDefault(); // Find().Where(a => a.Id == new Guid(xeroInvoiceId)).FirstOrDefault();
                        var result = Task.Run(async () => await accountingApi.GetInvoicesAsync(accessToken, xeroTenantId));
                        result.Wait();
                        var invoices = result.Result._Invoices;
                        var invoice = invoices.Where(a => a.InvoiceID == new Guid(xeroInvoiceId)).FirstOrDefault();
                        if (invoice != null)
                        {
                            Invoice.StatusEnum status = invoice.Status;
                            List<Payment> Payments = invoice.Payments;
                            if (status == Invoice.StatusEnum.DRAFT)
                            {
                                var deleteInvoice = new Invoice { Status = Invoice.StatusEnum.DELETED, InvoiceID = invoice.InvoiceID };
                                var deletedInvoices = new Invoices();
                                deletedInvoices._Invoices = new List<Invoice>() { deleteInvoice };
                                var result1 = Task.Run(async () => await accountingApi.UpdateInvoiceAsync(accessToken, xeroTenantId, new Guid(invoice.InvoiceID.ToString()), deletedInvoices));
                                result1.Wait();
                                //api.Invoices.Update(deleteInvoice);
                            }
                            else if (status == Invoice.StatusEnum.AUTHORISED || status == Invoice.StatusEnum.PAID)
                            {
                                //if (invoice.Payments.Count > 0)
                                //{
                                //    for (int pay = 0; pay < invoice.Payments.Count; pay++)
                                //    {
                                //        //var deletePayment = new Payment { Status = 
                                //        //    PaymentStatus.Deleted, Id = invoice.Payments[pay].Id };
                                //        // api.Payments.Update(deletePayment);

                                //        var deletePayment = new Payment
                                //        {
                                //            Status = Payment.StatusEnum.DELETED,
                                //            PaymentID = invoice.Payments[pay].PaymentID
                                //        };
                                //        var payments = new Payments();
                                //        payments._Payments = new List<Payment>() { deletePayment };
                                //        var result2 = Task.Run(async () => await accountingApi.DeletePaymentAsync(accessToken, xeroTenantId, new Guid(invoice.Payments[pay].PaymentID.ToString()), payments));
                                //        result2.Wait();
                                //    }
                                //}

                                //var deleteInvoice = new Invoice { Status = Invoice.StatusEnum.VOIDED, InvoiceID = invoice.InvoiceID };
                                //var deletedInvoices = new Invoices();
                                //deletedInvoices._Invoices = new List<Invoice>() { deleteInvoice };
                                //var result3 = Task.Run(async () => await accountingApi.UpdateInvoiceAsync(accessToken, xeroTenantId, new Guid(invoice.InvoiceID.ToString()), deletedInvoices));
                                //result3.Wait();
                                //api.Invoices.Update(deleteInvoice);

                                lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                            }
                            else if (status == Invoice.StatusEnum.DELETED || status == Invoice.StatusEnum.VOIDED)
                            {
                                lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                            }
                        }
                        else
                            lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                    }
                    else
                        lstNotDeletedInvoices.Add(lstXeroInvoiceIds[i].STCInvoiceNumber);
                }
            }
            return lstNotDeletedInvoices;
        }

        /// <summary>
        /// Import CSV for payment
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="solarCompanyId"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult ImportCSV(string resellerId, string solarCompanyId)
        {
            try
            {
                var postedFile = Request.Files[0];
                DataTable dtCSV = GetCSVTable();
                var textReader = new StreamReader(postedFile.InputStream);

                using (GenericParser parser = new GenericParser())
                {
                    parser.ColumnDelimiter = ','; //".ToCharArray();
                    parser.FirstRowHasHeader = true;
                    //parser.MaxBufferSize = 20480; // 8192 
                    parser.FirstRowSetsExpectedColumnCount = true;
                    parser.SetDataSource(textReader);

                    if (parser.Read())
                    {
                        do
                        {
                            try
                            {
                                var row = dtCSV.NewRow();
                                string[] STCInvoicePaymentId = { };

                                if (parser["STCInvoiceId"].Trim() != "" && parser["STCInvoiceId"].Trim() != null)
                                {
                                    string invoicePaymentId = string.Empty;
                                    if (!string.IsNullOrEmpty(parser["STCInvoiceId"].Trim()))
                                    {
                                        invoicePaymentId = QueryString.GetValueFromQueryString(Convert.ToString(parser["STCInvoiceId"].Trim()), "id");
                                        if (!string.IsNullOrEmpty(invoicePaymentId))
                                        {
                                            STCInvoicePaymentId = invoicePaymentId.Split(',');
                                        }
                                    }
                                }

                                for (int i = 1; i <= 5; i++)
                                {
                                    row = dtCSV.NewRow();
                                    if (parser["Amount" + i.ToString()].Trim() != "" && parser["Amount" + i.ToString()].Trim() != null)
                                    {
                                        if (Convert.ToDecimal(parser["Amount" + i.ToString()].Trim()) > 0)
                                            row["Amount"] = Convert.ToDecimal(parser["Amount" + i.ToString()].Trim());
                                        else
                                            return Json(new { status = false, error = "Payment amount should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        row["Amount"] = DBNull.Value;
                                    }

                                    if (parser["PaymentDate" + i.ToString()].Trim() != "" && parser["PaymentDate" + i.ToString()].Trim() != null)
                                    {
                                        row["PaymentDate"] = Convert.ToDateTime(parser["PaymentDate" + i.ToString()].Trim());
                                    }
                                    else
                                    {
                                        row["PaymentDate"] = DBNull.Value;
                                    }

                                    row["InvoiceNumber"] = parser["InvoiceNumber"].Trim();


                                    if (parser["UnitAmount"].Trim() != "" && parser["UnitAmount"].Trim() != null)
                                    {
                                        if (Convert.ToDecimal(parser["UnitAmount"].Trim()) > 0)
                                            row["UnitAmount"] = Convert.ToDecimal(parser["UnitAmount"].Trim());
                                        else
                                            return Json(new { status = false, error = "UnitAmount should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                        return Json(new { status = false, error = "Imported csv file has not data of unit amount." }, JsonRequestBehavior.AllowGet);


                                    if (STCInvoicePaymentId.Length > 0)
                                    {
                                        row["STCInvoiceID"] = Convert.ToInt32(STCInvoicePaymentId[0]);
                                        if (STCInvoicePaymentId.Length > i)
                                        {
                                            row["STCInvoicePaymentID"] = Convert.ToInt64(STCInvoicePaymentId[i]);
                                        }
                                        else
                                        {
                                            row["STCInvoicePaymentID"] = 0;
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { status = false, error = "Imported csv file has not data of STCInvoiceID." }, JsonRequestBehavior.AllowGet);
                                    }


                                    //if (parser["STCInvoiceID"].Trim() != "" && parser["STCInvoiceID"].Trim() != null)
                                    //    row["STCInvoiceID"] = Convert.ToInt32(parser["STCInvoiceID"].Trim());
                                    //else
                                    //    return Json(new { status = false, error = "Imported csv file has not data of STCInvoiceID." }, JsonRequestBehavior.AllowGet);


                                    if (parser["TaxType"].Trim() != "" && parser["TaxType"].Trim() != null)
                                        row["ISGST"] = Convert.ToString(parser["TaxType"].Trim().ToLower()) == "gst on expenses" ? true : false;
                                    else
                                        return Json(new { status = false, error = "Imported csv file has not data of TaxType." }, JsonRequestBehavior.AllowGet);

                                    if (parser["Have invoiced?"].Trim() != "" && parser["Have invoiced?"].Trim() != null)
                                        row["IsInvoiced"] = Convert.ToString(parser["Have invoiced?"].Trim().ToLower()) == "yes" ? true : false;
                                    else
                                        return Json(new { status = false, error = "Imported csv file has not data of TaxType." }, JsonRequestBehavior.AllowGet);

                                    dtCSV.Rows.Add(row);

                                }

                                //var row = dtCSV.NewRow();

                                //if (parser["Amount"].Trim() != "" && parser["Amount"].Trim() != null)
                                //{
                                //    if (Convert.ToDecimal(parser["Amount"].Trim()) > 0)
                                //        row["Amount"] = Convert.ToDecimal(parser["Amount"].Trim());
                                //    else
                                //        return Json(new { status = false, error = "Payment amount should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                //}
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of paymentdate or amount." }, JsonRequestBehavior.AllowGet);

                                //if (parser["PaymentDate"].Trim() != "" && parser["PaymentDate"].Trim() != null)
                                //    row["PaymentDate"] = Convert.ToDateTime(parser["PaymentDate"].Trim());
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of paymentdate or amount." }, JsonRequestBehavior.AllowGet);

                                //row["InvoiceNumber"] = parser["InvoiceNumber"].Trim();


                                //if (parser["UnitAmount"].Trim() != "" && parser["UnitAmount"].Trim() != null)
                                //{
                                //    if (Convert.ToDecimal(parser["UnitAmount"].Trim()) > 0)
                                //        row["UnitAmount"] = Convert.ToDecimal(parser["UnitAmount"].Trim());
                                //    else
                                //        return Json(new { status = false, error = "UnitAmount should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                //}
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of unit amount." }, JsonRequestBehavior.AllowGet);


                                //if (parser["STCInvoiceID"].Trim() != "" && parser["STCInvoiceID"].Trim() != null)
                                //    row["STCInvoiceID"] = Convert.ToInt32(parser["STCInvoiceID"].Trim());
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of STCInvoiceID." }, JsonRequestBehavior.AllowGet);


                                //if (parser["TaxType"].Trim() != "" && parser["TaxType"].Trim() != null)
                                //    row["ISGST"] = Convert.ToString(parser["TaxType"].Trim().ToLower()) == "gst on expenses" ? true : false;
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of TaxType." }, JsonRequestBehavior.AllowGet);

                                //if (parser["Have invoiced?"].Trim() != "" && parser["Have invoiced?"].Trim() != null)
                                //    row["IsInvoiced"] = Convert.ToString(parser["Have invoiced?"].Trim().ToLower()) == "yes" ? true : false;
                                //else
                                //    return Json(new { status = false, error = "Imported csv file has not data of TaxType." }, JsonRequestBehavior.AllowGet);

                                //dtCSV.Rows.Add(row);
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            }

                        } while (parser.Read());
                    }
                    else
                        return Json(new { status = false, error = "Imported csv file has not data." }, JsonRequestBehavior.AllowGet);

                    if (dtCSV.Rows.Count > 0)
                    {
                        DataSet remittanceData = _stcInvoiceServiceBAL.ImportCSV(dtCSV, ProjectSession.LoggedInUserId, DateTime.Now, Convert.ToInt32(resellerId), Convert.ToInt32(solarCompanyId));

                        if (remittanceData != null && remittanceData.Tables.Count > 1 && remittanceData.Tables[1] != null && remittanceData.Tables[1].Rows.Count > 0)
                        {
                            for (int i = 0; i < remittanceData.Tables[1].Rows.Count; i++)
                            {
                                _generateStcReportBAL.CreateStcReportNew("FormbotSTCReport", "Pdf", Convert.ToInt32(remittanceData.Tables[1].Rows[i]["STCJobDetailsID"]), Convert.ToString(remittanceData.Tables[1].Rows[i]["STCInvoiceNumber"]), Convert.ToString(remittanceData.Tables[1].Rows[i]["SolarCompanyID"]), "4", ProjectSession.LoggedInUserId, Convert.ToInt32(resellerId), false);
                            }
                        }
                        if (remittanceData != null && remittanceData.Tables.Count > 1 && remittanceData.Tables[0] != null && remittanceData.Tables[0].Rows.Count > 0)
                        {
                            List<Remittance> remittances = remittanceData.Tables[0].ToListof<Remittance>();
                            if (remittances != null && remittances.Count > 0)
                            {
                                for (int i = 0; i < remittances.Count; i++)
                                {
                                    _generateStcReportBAL.GenerateRemittanceNew(remittances[i], resellerId);
                                }
                            }
                        }
                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                        {
                            var stcInvoiceId = !string.IsNullOrEmpty(Convert.ToString(dtCSV.Rows[i]["STCInvoiceID"])) ? Convert.ToInt32(dtCSV.Rows[i]["STCInvoiceID"]) : 0;
                        }

                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, error = "Imported csv file has not data of paymentdate or amount." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get CSV table
        /// </summary>
        /// <returns>ActionResult</returns>
        public static DataTable GetCSVTable()
        {
            DataTable dtCSVData = new DataTable();
            dtCSVData.Columns.Add("Amount", typeof(decimal));
            dtCSVData.Columns.Add("PaymentDate", typeof(DateTime));
            dtCSVData.Columns.Add("InvoiceNumber", typeof(string));
            dtCSVData.Columns.Add("ISGST", typeof(bool));
            dtCSVData.Columns.Add("UnitAmount", typeof(decimal));
            dtCSVData.Columns.Add("STCInvoiceID", typeof(Int32));
            dtCSVData.Columns.Add("IsInvoiced", typeof(Int32));

            dtCSVData.Columns.Add("STCInvoicePaymentID", typeof(Int64));


            return dtCSVData;
        }

        /// <summary>
        /// UploadStcBulk
        /// </summary>
        /// <param name="resellerId"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult UploadStcBulk(string resellerId)
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    uploadStatus.Add(GetStcBulk(Request.Files[i], Convert.ToInt32(resellerId)));
                }
            }
            return Json(uploadStatus);
        }

        /// <summary>
        /// GetStcBulk
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="resellerId"></param>
        /// <returns>UploadStatus</returns>
        public HelperClasses.UploadStatus GetStcBulk(HttpPostedFileBase fileUpload, int resellerId)
        {
            HelperClasses.UploadStatus uploadStatus = new HelperClasses.UploadStatus();
            uploadStatus.FileName = Request.Files[0].FileName;

            if (fileUpload != null)
            {
                if (fileUpload.ContentLength > 0)
                {
                    string zipPath = Path.Combine(ProjectSession.ProofDocuments + "\\" + "JobDocuments" + "\\" + fileUpload.FileName);

                    if (System.IO.File.Exists(zipPath))
                    {

                        string orignalFileName = Path.GetFileNameWithoutExtension(zipPath);
                        string fileExtension = Path.GetExtension(zipPath);
                        string fileDirectory = Path.GetDirectoryName(zipPath);
                        int i = 1;
                        while (true)
                        {
                            string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                            if (System.IO.File.Exists(renameFileName))
                                i++;
                            else
                            {
                                zipPath = renameFileName;
                                break;
                            }
                        }

                    }
                    //fileUpload.SaveAs(zipPath);

                    //string[] fileList = System.IO.Directory.GetFiles(zipPath);
                    //foreach (string file in fileList)
                    //{

                    //}
                    int reId = 0;
                    int rankNumber = 0;
                    int response = 0;
                    int jobId = 0;
                    int STCInvoiceID = 0;
                    int solarCompanyID = 0;
                    List<STCInvoicePayment> lstFilePath = new List<Entity.STCInvoicePayment>();
                    int FileCount;
                    List<string> listSuccessFile = new List<string>();
                    List<string> listErrorFile = new List<string>();
                    string msg = "";
                    using (ZipFile zip = ZipFile.Read(fileUpload.InputStream))
                    {
                        foreach (ZipEntry ezip in zip.Take(1))
                        {

                            var extpath = ezip.FileName;

                            zip.ExtractAll(zipPath);
                            Directory.CreateDirectory(zipPath);
                            FileInfo[] files = null;
                            bool bIsFile = false;
                            bool bIsDirectory = false;

                            DirectoryInfo dir = new DirectoryInfo(zipPath + "\\" + extpath);

                            try
                            {
                                files = dir.GetFiles();

                                bIsDirectory = true;
                                bIsFile = false;
                            }
                            catch (System.IO.IOException)
                            {
                                bIsDirectory = false;
                                bIsFile = true;

                            }

                            if (bIsFile == true)
                            {
                                dir = new DirectoryInfo(zipPath);
                                files = dir.GetFiles();
                                FileCount = files.Length;
                            }
                            else
                            {
                                dir = new DirectoryInfo(zipPath + "\\" + extpath);
                                files = dir.GetFiles();
                                FileCount = files.Length;
                            }

                            foreach (FileInfo e in files)
                            {
                                string FileType = MimeMapping.GetMimeMapping(e.Name);

                                string[] Number = e.Name.Split('_');
                                string stcInvoiceNumber = Number[0];
                                DataSet ds = _createJob.GetjobIdBystcInvoiceNumber(stcInvoiceNumber, ProjectSession.UserTypeId, resellerId);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    jobId = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["jobId"].ToString()) ? Convert.ToInt32(ds.Tables[0].Rows[0]["jobId"].ToString()) : 0;
                                    STCInvoiceID = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["STCInvoiceID"].ToString()) ? Convert.ToInt32(ds.Tables[0].Rows[0]["STCInvoiceID"].ToString()) : 0;
                                    solarCompanyID = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["SolarCompanyId"].ToString()) ? Convert.ToInt32(ds.Tables[0].Rows[0]["SolarCompanyId"].ToString()) : 0;
                                    reId = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["resellerId"].ToString()) ? Convert.ToInt32(ds.Tables[0].Rows[0]["resellerId"].ToString()) : 0;
                                }
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                                {
                                    rankNumber = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["rankNumber"].ToString()) ? Convert.ToInt32(ds.Tables[1].Rows[0]["rankNumber"].ToString()) : 0;

                                }
                                if (jobId != 0)
                                {
                                    string fileName = stcInvoiceNumber + ".pdf";
                                    string proofDocumentsFolder = ProjectSession.ProofDocuments;
                                    string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;

                                    if (jobId != 0)
                                    {

                                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report" + "\\");
                                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report" + "\\";
                                    }

                                    if (!Directory.Exists(proofDocumentsFolder))
                                    {
                                        Directory.CreateDirectory(proofDocumentsFolder);
                                    }

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();

                                    try
                                    {
                                        string path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                                        if (System.IO.File.Exists(path))
                                        {

                                            string orignalFileName = Path.GetFileNameWithoutExtension(path);
                                            string fileExtension = Path.GetExtension(path);
                                            string fileDirectory = Path.GetDirectoryName(path);
                                            int i = 1;
                                            while (true)
                                            {
                                                string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                                                if (System.IO.File.Exists(renameFileName))
                                                    i++;
                                                else
                                                {
                                                    path = renameFileName;
                                                    break;
                                                }
                                            }
                                            fileName = Path.GetFileName(path);
                                        }
                                        fileName = fileName.Replace("%", "$");
                                        string mimeType = MimeMapping.GetMimeMapping(fileName);

                                        //System.IO.File.Copy(e, path, true);
                                        string fileToMove = e.DirectoryName + "\\" + e;
                                        string moveTo = path;
                                        //moving file
                                        System.IO.File.Move(fileToMove, moveTo);

                                        string PathName = "JobDocuments" + "\\" + jobId + "\\" + "Invoice\\Report" + "\\" + fileName;
                                        if (!string.IsNullOrEmpty(PathName))
                                        {
                                            lstFilePath.Add(new Entity.STCInvoicePayment { FilePath = PathName, STCInvoiceID = STCInvoiceID, SolarCompanyUserId = solarCompanyID, ResellerUserId = reId, RankNumber = rankNumber });
                                        }
                                        listSuccessFile.Add(e.Name);

                                        uploadStatus.Status = true;
                                        uploadStatus.Message = "File Uploaded Successfully.";
                                        uploadStatus.FileName = fileName;
                                        uploadStatus.MimeType = mimeType;

                                        uploadStatus.Path = proofDocumentsFolder + uploadStatus.FileName;
                                    }
                                    catch (Exception)
                                    {
                                        uploadStatus.Status = false;
                                        uploadStatus.Message = "An error occured while uploading. Please try again later.";
                                    }
                                }
                                else
                                {
                                    listErrorFile.Add(e.Name);
                                }

                            }

                        }
                    }
                    DeleteZipDir(zipPath);
                    if (lstFilePath.Count > 0)
                    {
                        string STCInvoicePaymentJson = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilePath);
                        response = _stcInvoiceServiceBAL.InsertBulkUploadFiles(STCInvoicePaymentJson, DateTime.Now, DateTime.Now, ProjectSession.LoggedInUserId);

                        //for (int i = 0; i < lstFilePath.Count; i++)
                        //{
                        //    string emailId = string.Empty;
                        //    int toUserId = 0;
                        //    int fromUserId = 0;

                        //    var dsToUsers = _userBAL.GetUserById(lstFilePath[i].SolarCompanyUserId);
                        //    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

                        //    var dsFromUsers = _userBAL.GetUserById(lstFilePath[i].ResellerUserId);
                        //    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

                        //    string newFileName = "Remittance_" + lstFilePath[i].RankNumber + ".pdf";
                        //    if (dsToUsers != null && dsFromUsers != null && dsToUsers.Tables[0].Rows.Count > 0 && dsFromUsers.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(toUserDetail.Email) && !string.IsNullOrEmpty(FromUserDetail.Email))
                        //    {
                        //        bool mailStatus = sendRemittanceFile(lstFilePath[i].FilePath, toUserDetail.Email, FromUserDetail.Email, newFileName);
                        //    }

                        //}
                    }

                    if (listErrorFile.Count > 0 && listSuccessFile.Count == 0)
                    {
                        string Files = "file is";
                        uploadStatus.Status = false;

                        foreach (var file in listErrorFile)
                        {

                            if (listErrorFile.IndexOf(file) == listErrorFile.Count - 1)
                            {
                                msg += file;
                            }
                            else
                            {
                                Files = "files are";
                                msg += file + " , ";
                            }

                        }
                        uploadStatus.Message = msg + "  " + Files + " not uploaded.";
                    }
                    else if (listErrorFile.Count > 0 && listSuccessFile.Count > 0)
                    {
                        string Files = "file is";
                        uploadStatus.Status = false;

                        msg = listSuccessFile.Count + " file uploaded successfully.";
                        foreach (var file in listErrorFile)
                        {
                            if (listErrorFile.IndexOf(file) == listErrorFile.Count - 1)
                            {
                                msg += file;
                            }
                            else
                            {
                                Files = "files are";
                                msg += file + " , ";
                            }

                        }
                        uploadStatus.Message = msg + "  " + Files + " not uploaded.";
                    }
                    else if (listErrorFile.Count == 0)
                    {
                        uploadStatus.Status = true;
                        uploadStatus.Message = "All files are Successfully uploaded.";

                    }
                }

            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.Message = "No data received";
            }

            return uploadStatus;
        }

        /// <summary>
        /// ViewDownloadFile
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="JobID"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult ViewDownloadFile(string FileName, string JobID)
        {

            var path = Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + JobID + "\\" + "Invoice" + "\\" + "Report" + "\\", FileName);
            //var path=@"\\azureexamplestorage.file.core.windows.net\test\UserDocuments\27\examples.png";

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
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// DeleteFileFromFolder
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="FolderName"></param>
        /// <returns>JsonResult</returns>
        [AllowAnonymous]
        public JsonResult DeleteFileFromFolder(string fileName, string FolderName)
        {
            DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + FolderName + "\\" + "Invoice" + "\\" + "Report" + "\\", fileName));
            this.ShowMessage(SystemEnums.MessageType.Success, "File has been deleted successfully.", false);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public bool sendRemittanceFile(string filePath, string toEmail, string fromEmail, string fileName, string fromName = "", string toName = "")
        {
            //string FileName = ProjectSession.ProofDocuments + filePath;
            //string FailReason = string.Empty;
            //SMTPDetails smtpDetail = new SMTPDetails();
            //smtpDetail.MailFrom = fromEmail;
            //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
            //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
            //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
            //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
            //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;
            //EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(27, null, null);
            //bool status = MailHelper.SendMail(smtpDetail, toEmail, null, null, eTemplate.Subject, eTemplate.Content, FileName, true, ref FailReason, false, fileName);
            //return status;

            EmailInfo emailInfo = new EmailInfo();
            emailInfo.TemplateID = 27;
            emailInfo.SolarCompanyFullName = toName;
            emailInfo.ResellerFullName = fromName;
            //emailInfo.FullFileName = ProjectSession.ProofDocuments + filePath;
            //emailInfo.DisplayFileName = fileName;
            EmailBAL objEmailBAL = new EmailBAL();

            //AttachmentData attachmentData = new AttachmentData();
            //attachmentData.FileName = ProjectSession.ProofDocuments + filePath;
            //attachmentData.GeneratedName = fileName;

            List<FormBot.Email.AttachmentData> Attachments = new List<AttachmentData>();
            //Attachments.Add(attachmentData);

            IEmailBAL emailService = null;
            FormBot.BAL.Service.IUserBAL userBAL = null;
            FormBot.BAL.Service.ICreateJobBAL job = null;
            Areas.Email.Controllers.EmailController objEmail = new Areas.Email.Controllers.EmailController(emailService, userBAL, job, null, null, null);

            filePath = ProjectSession.ProofDocuments + filePath;
            var result = objEmail.UploadPhysicalFileAsAnAttachmentInEmail(Path.GetFileName(filePath), filePath);

            if (result != null)
            {
                var myJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                var fileTmpDetails = Newtonsoft.Json.Linq.JObject.Parse(myJsonString);
                var fileTmpName = fileTmpDetails["Data"]["tmp_name"].ToString();

                if (fileTmpName != null)
                {
                    AttachmentData attachmentData = new AttachmentData();
                    attachmentData.FileName = Path.GetFileName(filePath);
                    attachmentData.GeneratedName = fileTmpName;
                    Attachments.Add(attachmentData);
                }
            }

            objEmailBAL.ComposeAndSendEmail(emailInfo, toEmail, Attachments);

            return true;
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteZipDir(string path)
        {
            string directoryPath = path;
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        /// <summary>
        /// Gets the STC invoice.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult GetSTCAmountPaidDetail(string id)
        {
            int invoiceId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(id) : 0;
            STCInvoicePayment stcInvoicePayment = new STCInvoicePayment();
            stcInvoicePayment.STCInvoiceID = invoiceId;
            return PartialView("_STCAmountPaidDetail", stcInvoicePayment);
        }

        public void GetSTCAmountPaidDetailRecords(string stcinvoiceid)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            Int64 invoiceId = !string.IsNullOrEmpty(stcinvoiceid) ? Convert.ToInt64(stcinvoiceid) : 0;
            IList<STCInvoicePayment> lstSTCAmountPaid = _stcInvoiceServiceBAL.GetSTCAmountPaidDetailRecords(gridParam.SortCol, gridParam.SortDir, invoiceId);
            //if (lstSTCAmountPaid.Count > 0)
            //{
            //    gridParam.TotalDisplayRecords = lstSTCAmountPaid.FirstOrDefault().TotalRecords;
            //    gridParam.TotalRecords = lstSTCAmountPaid.FirstOrDefault().TotalRecords;
            //}

            HttpContext.Response.Write(Grid.PrepareDataSet(lstSTCAmountPaid, gridParam));
        }

        /// <summary>
        /// Updates the STC amount paid record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="payment">The payment.</param>
        /// <param name="paymentdate">The paymentdate.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSTCAmountPaidRecord(string reseller, string id, string payment, string paymentdate)
        {
            int ResellerID = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            Int64 STCInvoicePaymentID = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            decimal Payment = !string.IsNullOrEmpty(payment) ? Convert.ToDecimal(payment) : 0;
            DateTime PaymentDate = !string.IsNullOrEmpty(paymentdate) ? Convert.ToDateTime(paymentdate) : DateTime.Now;
            List<Remittance> remittanceData = _stcInvoiceServiceBAL.UpdateSTCAmountPaidRecord(ResellerID, STCInvoicePaymentID, Payment, PaymentDate);
            if (remittanceData != null && remittanceData.Count > 0)
            {
                for (int i = 0; i < remittanceData.Count; i++)
                {
                    _generateStcReportBAL.GenerateRemittanceNew(remittanceData[i], reseller);

                    var StcinvoiceId = !string.IsNullOrEmpty(Convert.ToString(remittanceData[i].STCInvoiceID)) ? Convert.ToInt32(remittanceData[i].STCInvoiceID) : 0;
                }
            }
            //GenerateRemittance(remittanceData, reseller);
            return Json(new { success = true });
        }

        /// <summary>
        /// Adds the STC amount paid record.
        /// </summary>
        /// <param name="stcinvoiceid">The stcinvoiceid.</param>
        /// <param name="payment">The payment.</param>
        /// <param name="paymentdate">The paymentdate.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSTCAmountPaidRecord(string reseller, string stcinvoiceid, string payment, string paymentdate)
        {
            int ResellerID = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            Int64 STCInvoiceID = !string.IsNullOrEmpty(stcinvoiceid) ? Convert.ToInt64(stcinvoiceid) : 0;
            decimal Payment = !string.IsNullOrEmpty(payment) ? Convert.ToDecimal(payment) : 0;
            DateTime PaymentDate = !string.IsNullOrEmpty(paymentdate) ? Convert.ToDateTime(paymentdate) : DateTime.Now;
            List<Remittance> remittanceData = _stcInvoiceServiceBAL.AddSTCAmountPaidRecord(ResellerID, ProjectSession.LoggedInUserId, DateTime.Now, STCInvoiceID, Payment, PaymentDate);

            if (remittanceData != null && remittanceData.Count > 0)
            {
                for (int i = 0; i < remittanceData.Count; i++)
                {
                    _generateStcReportBAL.GenerateRemittanceNew(remittanceData[i], reseller);
                }
            }
            //GenerateRemittance(remittanceData, reseller);
            return Json(new { success = true });
        }

        /// <summary>
        /// Deletes the STC amount paid record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSTCAmountPaidRecord(string id)
        {
            Int64 STCInvoicePaymentID = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            DataSet ds = _stcInvoiceServiceBAL.DeleteSTCAmountPaidRecord(STCInvoicePaymentID);
            DataTable dt = ds.Tables[0];
            string deletedFilePath = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["FilePath"])))
                deletedFilePath = Convert.ToString(dt.Rows[0]["FilePath"]);
            if (System.IO.File.Exists(ProjectSession.ProofDocumentsURL + "\\" + deletedFilePath))
            {
                System.IO.File.Delete(ProjectSession.ProofDocumentsURL + "\\" + deletedFilePath);
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Regenerate STC Invoice
        /// </summary>
        /// <param name="stcInvoiceData">stcInvoiceData</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult RegenerateSTCInvoice(string stcInvoiceData)
        {
            DataTable dtSTCInvoice = new DataTable();
            dtSTCInvoice.Columns.Add("STCInvoiceID", typeof(Int64));
            dtSTCInvoice.Columns.Add("STCInvoiceFilePath", typeof(string));
            List<STCInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(stcInvoiceData);
            if (lstInvoiceIds.Count > 0)
            {
                for (int i = 0; i < lstInvoiceIds.Count; i++)
                {
                    string jobId = Convert.ToString(lstInvoiceIds[i].JobID);
                    //DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice" + "\\" + "Report" + "\\", lstInvoiceIds[i].STCInvoiceNumber + ".pdf"));
                    _generateStcReportBAL.MoveDeletedDocuments(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice" + "\\" + "Report" + "\\", lstInvoiceIds[i].STCInvoiceNumber + ".pdf"), jobId);

                    //string filePath = CreateStcReport("FormbotSTCReport", "Pdf", Convert.ToInt32(lstInvoiceIds[i].STCJobDetailsID), lstInvoiceIds[i].STCInvoiceNumber, Convert.ToString(lstInvoiceIds[i].SolarCompanyId),true);
                    string filePath = _generateStcReportBAL.CreateStcReportNew("FormbotSTCReport", "Pdf", Convert.ToInt32(lstInvoiceIds[i].STCJobDetailsID), lstInvoiceIds[i].STCInvoiceNumber, Convert.ToString(lstInvoiceIds[i].SolarCompanyId), "4", ProjectSession.LoggedInUserId, ProjectSession.ResellerId, false, true);
                    dtSTCInvoice.Rows.Add(new object[] { Convert.ToInt32(lstInvoiceIds[i].STCInvoiceID), filePath });
                }
            }
            if (dtSTCInvoice.Rows.Count > 0)
                return this.Json(new { success = true });
            else
                return this.Json(new { success = false });
            //_stcInvoiceServiceBAL.UpdateRecGeneratedRCTIFilePath(dtSTCInvoice);
        }
        public FileResult DownloadSTCInvoice(string documentName, string jobId)
        {
            string fileName = documentName.Split('\\').Last();
            //documentName.Replace("\\\","");
            string path = @FormBot.Helper.ProjectSession.ProofDocumentsURL;//ProjectConfiguration. + documentName;
            string documentFullPath = path + documentName; //Path.Combine(ProjectSession.ProofDocumentsURL, path);
            string deletedirectoryPath = path + "JobDocuments\\" + jobId + "\\Invoice";


            if (System.IO.File.Exists(documentFullPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(documentFullPath);
                //string fileName = path.Split('\\').Last();
                System.IO.File.Delete(documentFullPath);
                if (System.IO.Directory.Exists(deletedirectoryPath))
                {
                    //System.IO.Directory.SetAccessControl(deletedirectoryPath,)
                    System.IO.Directory.Delete(deletedirectoryPath, true);
                }

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
                Response.Write("File Doesn't exists!");
            return null;
        }
        //[HttpPost]
        //public ActionResult AuditRegenerateSTCInvoice(string stcInvoiceData)
        //{
        //    DataTable dtSTCInvoice = new DataTable();
        //    dtSTCInvoice.Columns.Add("STCInvoiceID", typeof(Int64));
        //    dtSTCInvoice.Columns.Add("STCInvoiceFilePath", typeof(string));
        //    List<STCInvoice> lstInvoiceIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<STCInvoice>>(stcInvoiceData);
        //    if (lstInvoiceIds.Count > 0)
        //    {
        //        for (int i = 0; i < lstInvoiceIds.Count; i++)
        //        {
        //            string jobId = Convert.ToString(lstInvoiceIds[i].JobID);
        //            //DeleteDirectory(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice" + "\\" + "Report" + "\\", lstInvoiceIds[i].STCInvoiceNumber + ".pdf"));
        //            _generateStcReportBAL.MoveDeletedDocuments(Path.Combine(ProjectSession.ProofDocumentsURL + "\\" + "JobDocuments" + "\\" + jobId + "\\" + "Invoice" + "\\" + "Report" + "\\", lstInvoiceIds[i].STCInvoiceNumber + ".pdf"), jobId);

        //            //string filePath = CreateStcReport("FormbotSTCReport", "Pdf", Convert.ToInt32(lstInvoiceIds[i].STCJobDetailsID), lstInvoiceIds[i].STCInvoiceNumber, Convert.ToString(lstInvoiceIds[i].SolarCompanyId),true);
        //            string filePath = _generateStcReportBAL.CreateStcReport("FormbotSTCReport", "Pdf", Convert.ToInt32(lstInvoiceIds[i].STCJobDetailsID), lstInvoiceIds[i].STCInvoiceNumber, Convert.ToString(lstInvoiceIds[i].SolarCompanyId), "4", ProjectSession.LoggedInUserId, ProjectSession.ResellerId, false, true);
        //            dtSTCInvoice.Rows.Add(new object[] { Convert.ToInt32(lstInvoiceIds[i].STCInvoiceID), filePath });
        //            _log.Log(SystemEnums.Severity.Info, "Successfully generated invoice: " + (lstInvoiceIds[i].STCInvoiceID).ToString());
        //        }
        //    }
        //    if (dtSTCInvoice.Rows.Count > 0)
        //        return this.Json(new { success = true });
        //    else
        //        return this.Json(new { success = false });
        //    //_stcInvoiceServiceBAL.UpdateRecGeneratedRCTIFilePath(dtSTCInvoice);
        //}

        #region Get job detail from REC which is not created in greenbot system

        /// <summary>
        /// Prepare datatble for job which is not created in greenbot system
        /// </summary>
        /// <returns></returns>
        public DataTable PVDCode()
        {
            DataTable dtPVDCode = new DataTable();

            dtPVDCode.Columns.Add("Id", typeof(string));

            dtPVDCode.Rows.Add(new object[] { "PVD3076944" });
            dtPVDCode.Rows.Add(new object[] { "PVD3070733" });

            return dtPVDCode;
        }

        /// <summary>
        /// manually call this method and get job detail from REC
        /// </summary>
        public void GetRECRecord()
        {
            try
            {

                CookieContainer cookies = new CookieContainer();
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create("https://www.rec-registry.gov.au/rec-registry/cer_security_check");
                wreq.Method = "POST";
                wreq.Timeout = -1;
                wreq.CookieContainer = cookies;
                wreq.KeepAlive = true;
                wreq.ContentType = "application/x-www-form-urlencoded";
                string paramContent = "j_username=" + "" + "&j_password=" + "" + "";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(paramContent);
                wreq.ContentLength = buffer.Length;

                using (var request = wreq.GetRequestStream())
                {
                    request.Write(buffer, 0, buffer.Length);
                    request.Close();
                }
                var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                string strResult;
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    strResult = reader.ReadToEnd();
                    myHttpWebResponse.Close();
                }
                //CSRF
                Match m = Regex.Match(strResult, "name=\"_csrf\" content=\"(.*)\"/>");
                var CSRFToken = string.Empty;
                if (m.Success)
                {
                    CSRFToken = m.Groups[1].Value;

                    NameValueCollection nvc = new NameValueCollection();

                    DataTable dtPVDCode = PVDCode();
                    DataTable dtREC = dtResult();


                    for (int i = 0; i < dtPVDCode.Rows.Count; i++)
                    {
                        SearchPVDCodeResult result = SearchByAccreditationCode(cookies, CSRFToken, true, dtPVDCode.Rows[i]["Id"].ToString());
                        string inverterDetails = string.Empty;

                        if (result != null && result.result != null && result.result.installationDetails != null)
                        {
                            if (result.result.installationDetails.inverters != null && result.result.installationDetails.inverters.Length > 0)
                            {
                                for (int j = 0; j < result.result.installationDetails.inverters.Length; j++)
                                {
                                    inverterDetails = string.Join(",", result.result.installationDetails.inverters.Select(a => a.modelNumber));
                                }
                            }
                            dtREC.Rows.Add(new object[] { dtPVDCode.Rows[i]["Id"].ToString() + "-1", dtPVDCode.Rows[i]["Id"].ToString(), result.result.reference, result.result.installationDetails.retailerName, inverterDetails });
                        }

                    }

                    DataTable dt = dtREC;
                }

            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// prepare datatable for REC job result
        /// </summary>
        /// <returns></returns>
        public DataTable dtResult()
        {
            DataTable dtResult = new DataTable();

            dtResult.Columns.Add("Registration Code", typeof(string));
            dtResult.Columns.Add("Accreditation Code", typeof(string));
            dtResult.Columns.Add("Reference Number", typeof(string));
            dtResult.Columns.Add("Retailer Name", typeof(string));
            dtResult.Columns.Add("Model number", typeof(string));

            return dtResult;
        }

        /// <summary>
        /// search job detail in REC using accreditation code
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="CSRFToken"></param>
        /// <param name="IsPVDJob"></param>
        /// <param name="accreditationCode"></param>
        /// <returns></returns>
        public static SearchPVDCodeResult SearchByAccreditationCode(CookieContainer cookies, string CSRFToken, bool IsPVDJob, string accreditationCode)
        {
            try
            {

                //GET /rec-registry/app/smallunits/sgu/details?registrationCode=PVD3071142-1&_=1545046413827 HTTP/1.1

                HttpWebRequest wreq = null;
                if (IsPVDJob)
                    wreq = (HttpWebRequest)WebRequest.Create("https://www.rec-registry.gov.au/rec-registry/app/smallunits/sgu/details?registrationCode=" + accreditationCode + "-1");
                else
                    wreq = (HttpWebRequest)WebRequest.Create("https://www.rec-registry.gov.au/rec-registry/app/smallunits/swh/search");
                wreq.Method = "GET";
                wreq.Timeout = -1;
                wreq.CookieContainer = cookies;
                wreq.ContentType = "application/json; charset=UTF-8";

                wreq.Headers.Add("X-CSRF-TOKEN", CSRFToken);

                var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                string strResult;
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    strResult = reader.ReadToEnd();
                    myHttpWebResponse.Close();
                }
                //Match m1 = Regex.Match(strResult, "name=\"registrationCode\" content=\"(.*)\"/>");
                if (strResult != null)
                {
                    var model = JsonConvert.DeserializeObject<SearchPVDCodeResult>(strResult);
                    return model;
                }
                return new SearchPVDCodeResult();
            }
            catch (Exception ex)
            {
                return new SearchPVDCodeResult();
            }

        }

        /// <summary>
        /// Import Bulk upload id from CSV
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ImportBulkUploadIdCSVFromREC()
        {

            string filePath = string.Empty;
            try
            {
                bool isCreateInvoice = false;
                var postedFile = Request.Files[0];
                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                var textReader = new StreamReader(postedFile.InputStream);
                var csvTable = new DataTable();
                DataTable dtImportPVDSWHCode = DataTableImportPVDCode();
                using (var csvReader = new CsvReader(textReader, true))
                {
                    csvTable.Load(csvReader);
                    for (int i = 0; i < csvTable.Rows.Count; i++)
                    {
                        DataRow dr = dtImportPVDSWHCode.NewRow();
                        string serialnumber = Convert.ToString(csvTable.Rows[i]["Serial numbers"]);
                        dr["Serialnumber"] = serialnumber.Contains(',') ? serialnumber.Split(',')[0] : serialnumber;
                        dr["OwnerType"] = csvTable.Rows[i]["Owner type"];
                        dr["OwnerName"] = csvTable.Rows[i]["Owner name"];

                        if (csvTable.Columns.Contains("Your reference"))
                            dr["ReferenceNumber"] = csvTable.Rows[i]["Your reference"];
                        else
                            dr["ReferenceNumber"] = csvTable.Rows[i]["Reference"];

                        if (csvTable.Columns.Contains("CEC installer number"))
                            dr["CECAccreditedNumber"] = csvTable.Rows[i]["CEC installer number"];
                        else
                            dr["PVDSWHCode"] = string.Empty;

                        dr["InstallationDate"] = csvTable.Rows[i]["Installation date"];
                        dr["RECCreationDate"] = csvTable.Rows[i]["REC creation date"];
                        dr["PVDSWHCode"] = csvTable.Rows[i]["Accreditation code"];

                        dtImportPVDSWHCode.Rows.Add(dr);
                    }
                }
                DataSet dsUpdateData = _stcInvoiceServiceBAL.ImportBulkUploadData(dtImportPVDSWHCode, fileName);
                #region save stcJobHistory into xml
                if (dsUpdateData != null && dsUpdateData.Tables.Count > 0 && dsUpdateData.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow drUpdateData in dsUpdateData.Tables[2].Rows)
                    {
                        int JobID = Convert.ToInt32(drUpdateData["JobID"]);
                        int STCStatusID = Convert.ToInt32(drUpdateData["STCStatusID"]);
                        string Description = drUpdateData["Description"].ToString();
                        string CreatedByID = drUpdateData["CreatedBy"].ToString();
                        string CreatedBy = "";
                        if (CreatedByID.ToString() == "-1")
                        {
                            CreatedBy = "System";
                        }
                        else
                        {
                            CreatedBy = _createJob.GetUsernameByUserID(Convert.ToInt32(CreatedByID));
                        }
                        string JobHistoryMessage = "changed STC Status to " + _createJob.GetSTCStausNameBySTCStatusID(STCStatusID) + " <b class=\"blue-title\"> (" + JobID + ") JobRefNo </b> - ";
                        Common.SaveSTCJobHistorytoXML(JobID, JobHistoryMessage, Description, STCStatusID, "Statuses", "STCSubmission", CreatedBy, false);
                    }
                }
                #endregion
                if (dsUpdateData != null && dsUpdateData.Tables.Count > 0)
                {
                    DataTable dtUpdateData = dsUpdateData.Tables[0];
                    int count = dtUpdateData.Rows.Count;
                    List<int> lstSTCJobdetailIds = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        int STCJobDetailId = Convert.ToInt32(dtUpdateData.Rows[i]["STCJobDetailsID"]);

                        SortedList<string, string> data = new SortedList<string, string>();
                        string pvdswhcode = dtUpdateData.Rows[i]["STCPVDCode"].ToString();
                        string stcstatus = dtUpdateData.Rows[i]["STCStatus"].ToString();
                        string gbBatchRECUploadId = dtUpdateData.Rows[i]["GBBatchRECUploadId"].ToString();
                        string strStcStatus = (stcstatus != null || stcstatus != "") ? Common.GetDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";
                        string colorCode = (stcstatus != null || stcstatus != "") ? Common.GetSubDescription((SystemEnums.STCJobStatus)Convert.ToInt32(stcstatus), "") : "";
                        //string RECCreationTime = dtUpdateData.Rows[i]["RECCreationDate"].ToString();

                        int settlementterm = Convert.ToInt32(dtUpdateData.Rows[i]["STCSettlementTerm"]);
                        int customsettlementterm = Convert.ToInt32(dtUpdateData.Rows[i]["CustomSettlementTerm"]);

                        if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && settlementterm != 10) ||
                            (settlementterm == 10 && (customsettlementterm != 4 && customsettlementterm != 8 && customsettlementterm != 12)))
                        {
                            lstSTCJobdetailIds.Add(STCJobDetailId);
                        }

                        data.Add("GBBatchRECUploadId", gbBatchRECUploadId);
                        data.Add("ColorCode", colorCode);
                        data.Add("PVDSWHCode", pvdswhcode);
                        data.Add("STCStatus", strStcStatus);
                        data.Add("STCStatusId", stcstatus);
                        //data.Add("RECCreationTime", RECCreationTime);
                        await CommonBAL.SetCacheDataForSTCSubmission(STCJobDetailId, null, data);
                        Helper.Log.WriteLog(DateTime.Now + " Update cache for stcid from ImportBulkUploadIdCSVFromREC  " + (STCJobDetailId.ToString()) + " gbBatchRECUploadId: " + gbBatchRECUploadId);
                    }

                    DataTable dtResellerId = dsUpdateData.Tables[1];
                    if (dtResellerId != null && dtResellerId.Rows.Count > 0 && lstSTCJobdetailIds.Count > 0)
                    {
                        string resellerId = Convert.ToString(dtResellerId.Rows[0]["ResellerID"]);
                        isCreateInvoice = await SaveSTCInvoice(resellerId, string.Join(",", lstSTCJobdetailIds), 1, "", 1, ProjectSession.LoggedInUserId, InoviceTermID: 4);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int UserID = ProjectSession.LoggedInUserId;
                        string FileName = fileName;
                        string bulkuploadId = dtUpdateData.Rows[i]["GBBatchRECUploadId"].ToString();
                        int STCStatusID = Convert.ToInt32(dtUpdateData.Rows[i]["STCStatus"]);
                        int StcJobDetailID = Convert.ToInt32(dtUpdateData.Rows[i]["STCJobDetailsID"]);
                        string Description = "imported a file " + fileName + ".csv" + " BulkUploadID: " + bulkuploadId;
                        _createJob.SaveSTCJobHistory(StcJobDetailID, STCStatusID, ProjectSession.LoggedInUserId, Description, DateTime.Now, ProjectSession.LoggedInUserId);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
            return Json(new { status = true });
        }

        /// <summary>
        /// Create table to import data
        /// </summary>
        /// <returns></returns>
        public DataTable DataTableImportPVDCode()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Serialnumber", typeof(string));
            dataTable.Columns.Add("OwnerType", typeof(string));
            dataTable.Columns.Add("OwnerName", typeof(string));
            dataTable.Columns.Add("ReferenceNumber", typeof(string));
            dataTable.Columns.Add("CECAccreditedNumber", typeof(string));
            dataTable.Columns.Add("InstallationDate", typeof(DateTime));
            dataTable.Columns.Add("RECCreationDate", typeof(DateTime));
            dataTable.Columns.Add("PVDSWHCode", typeof(string));
            return dataTable;
        }

        #endregion

        #endregion

        [HttpPost]
        public ActionResult MarkUnmarkApprovedDeletedInvoices(string stcinvoiceids)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                bool IsInvoiced = false;

                List<string> lstNotDeletedInvoices = new List<string>();
                List<string> lstInvoiceIds = stcinvoiceids.Split(',').ToList();

                AccountingApi accountingApi = new AccountingApi();


                if (lstInvoiceIds.Count > 0)
                {
                    _stcInvoiceServiceBAL.MarkUnMarkSelectedAsSentForPayment(IsInvoiced, string.Join(",", lstInvoiceIds));

                    DataSet ds = _stcInvoiceServiceBAL.GetStcDataFromStcInvoiceIds(string.Join(",", lstInvoiceIds));
                    // List<int> jobid = new List<int>();
                    List<STCInvoice> sTCInvoices = new List<STCInvoice>();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        sTCInvoices = ConvertDataTable<STCInvoice>(ds.Tables[0]);
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        for (int i = 0; i < sTCInvoices.Count; i++)
                        {
                            jobHistory.JobID = sTCInvoices[i].JobID;
                            jobHistory.stcInvoiceNumber = sTCInvoices[i].STCInvoiceNumber;
                            jobHistory.HistoryMessage = "which is approved,paid or not existed invoice in xero";
                            //_jobHistory.LogJobHistory(jobHistory, HistoryCategory.UnmarkInvoice);
                            string JobHistoryMessage = "has unmarked <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + " " + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "UnmarkInvoice", ProjectSession.LoggedInName, false);
                        }
                    }
                }
                return this.Json(new { success = true });
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Remove Selected STC Invoice
        /// </summary>
        /// <param name="stcinvoiceids"></param>
        /// <param name="xeroInvoiceIdsArray"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult> RemoveApprovedDeletedInvoices(string stcinvoiceids, string stcJobDetailIds)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                List<string> lstNotDeletedInvoices = new List<string>();
                List<string> lstInvoiceIds = stcinvoiceids.Split(',').ToList();
                AccountingApi accountingApi = new AccountingApi();


                if (lstInvoiceIds.Count > 0)
                {
                    _stcInvoiceServiceBAL.RemoveSelectedSTCInvoice(string.Join(",", lstInvoiceIds));
                    DataSet ds = _stcInvoiceServiceBAL.GetStcDataFromStcInvoiceIds(string.Join(",", lstInvoiceIds));
                    // List<int> jobid = new List<int>();
                    List<STCInvoice> sTCInvoices = new List<STCInvoice>();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        sTCInvoices = ConvertDataTable<STCInvoice>(ds.Tables[0]);
                        JobHistory jobHistory = new JobHistory();
                        jobHistory.Name = ProjectSession.LoggedInName;
                        for (int i = 0; i < sTCInvoices.Count; i++)
                        {
                            jobHistory.JobID = sTCInvoices[i].JobID;
                            jobHistory.stcInvoiceNumber = sTCInvoices[i].STCInvoiceNumber;
                            jobHistory.HistoryMessage = "which is approved,paid or not existed invoice in xero";
                            // _jobHistory.LogJobHistory(jobHistory, HistoryCategory.RemoveInvoiceFromGreenbot);
                            string JobHistoryMessage = "has removed <b style=\"color: black\">" + jobHistory.stcInvoiceNumber + "</b>" + " " + jobHistory.HistoryMessage;
                            Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "Invoicing", "RemoveInvoiceFromGreenbot", ProjectSession.LoggedInName, false);
                        }
                    }
                }
                //for cache update in stc submission screen
                DataTable dtinvoiceCount = _createJob.GetSTCInvoiceAndCreditNoteCount(stcJobDetailIds);
                List<int> peakpaySTCJobIds = new List<int>();
                if (dtinvoiceCount.Rows.Count > 0)
                {
                    peakpaySTCJobIds = dtinvoiceCount.AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsID")).Distinct().ToList();
                    for (int j = 0; j < dtinvoiceCount.Rows.Count; j++)
                    {
                        SortedList<string, string> data = new SortedList<string, string>();
                        string IsInvoiced = dtinvoiceCount.Rows[j]["IsInvoiced"].ToString();
                        string IsCreditNote = dtinvoiceCount.Rows[j]["IsCreditNote"].ToString();
                        string STCInvoiceCount = dtinvoiceCount.Rows[j]["STCInvoiceCount"].ToString();
                        data.Add("IsInvoiced", IsInvoiced);
                        data.Add("IsCreditNote", IsCreditNote);
                        data.Add("STCInvoiceCount", STCInvoiceCount);
                        await CommonBAL.SetCacheDataForSTCSubmission(null, Convert.ToInt32(dtinvoiceCount.Rows[j]["JobID"]), data, true, false, true);
                    }
                }
                if(peakpaySTCJobIds.Count>0)
                 await CommonBAL.SetCacheDataForPeakPayFromJobId("", string.Join(",",peakpaySTCJobIds));

                return this.Json(new { success = true });
            }
            catch (Exception e)
            {
                //Response.Write(e.Message);
                return Json(new { status = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetSyncToXeroLog(int resellerId)
        {
            try
            {
                List<SyncWithXeroLog> syncWithXeroLogs = _stcInvoiceServiceBAL.GetSynxWithXeroLog(resellerId);
                for (int i = 0; i < syncWithXeroLogs.Count; i++)
                {
                    syncWithXeroLogs[i].strModifiedDate = syncWithXeroLogs[i].ModifiedDate.ToString("MM/dd/yyyy HH:mm");
                }
                return Json(new { syncWithXeroLogs = syncWithXeroLogs, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ex = ex, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        [HttpGet]
        public void GetRecStatusData(int StageId = 1, string sortBy = "CreatedDate", int ResellerId = 0, string bulkUploadId = "", DateTime? FromDate = null, DateTime? ToDate = null, string RECUserName = null, string RecName = null, string InitiatedBy = null)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            if (gridParam.SortCol == "Id")
            {
                gridParam.SortCol = "CreatedDate";
                gridParam.SortDir = "desc";
            }
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            SearchParamRec searchParam = new SearchParamRec();
            searchParam.ResellerId = ResellerId;
            searchParam.BulkUploadId = bulkUploadId;
            searchParam.DateFrom = FromDate;
            searchParam.DateTo = ToDate;
            searchParam.StageId = StageId;
            searchParam.RECName = RecName;
            searchParam.RECUsername = RECUserName;
            searchParam.InitiatedBy = InitiatedBy;
            DataSet ds = _reseller.GetRecFailureReason(searchParam, gridParam.PageSize, pageNumber, gridParam.SortCol, gridParam.SortDir);
            REC rec = new REC();
            if (ds.Tables.Count > 0)
            {
                //if (searchParam.StageId == 3)
                //{
                //    rec.lstREC = ds.Tables[0].ToListof<RECData>();
                //    //rec.lstRecSearchFailedBatchId = ds.Tables[1].ToListof<RECData>();
                //}
                //else if (searchParam.StageId == 1)
                //    rec.lstREC = ds.Tables[0].ToListof<RECData>();
                //else if (searchParam.StageId == 2)
                rec.lstREC = ds.Tables[0].ToListof<RECData>();
            }
            if (rec.lstREC.Count > 0)
            {
                gridParam.TotalDisplayRecords = rec.lstREC.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = rec.lstREC.FirstOrDefault().TotalRecords;
            }
            HttpContext.Response.Write(Grid.PrepareDataSet(rec.lstREC, gridParam));
            //return Json(ds, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRecFailedBatchDetails(string batchId, decimal totalSTC, bool isIssue, bool isFailedJob = true, bool isUnLockBtns = false)
        {
            DataSet ds = _stcInvoiceServiceBAL.GetRECFailedBatchDetails(batchId, "", isFailedJob);
            List<RECData> recData = ds.Tables[0].ToListof<RECData>();
            REC rEC = new REC();
            rEC.lstREC = recData;
            rEC.BatchId = batchId;
            rEC.TotalJob = recData.Count;
            rEC.TotalSTCs = totalSTC;
            rEC.IsIssue = isIssue;
            rEC.IsFailedJobs = isFailedJob;
            rEC.IsUnLockBtns = isUnLockBtns;
            return PartialView("/Views/Job/_RECFailureReason.cshtml", rEC);
        }
        [HttpGet]
        public ActionResult GetJobDetailsBatchWise(string batchId)
        {
            DataSet ds = _stcInvoiceServiceBAL.GetJobDetailsBatchWise(batchId);
            List<RECData> recData = ds.Tables[0].ToListof<RECData>();
            REC rEC = new REC();
            rEC.lstREC = recData;
            rEC.BatchId = recData[0].GBBatchRECUploadId;
            rEC.TotalJob = recData.Count;
            return PartialView("/Views/Job/_GetJobDetailsOfBatch.cshtml", rEC);
        }

        public JsonResult ReleaseForRecreation(string recUploadId)
        {
            try
            {
                List<string> lstJobIds = new List<string>();
                int BatchRecUploadCount = Convert.ToInt32(ProjectConfiguration.BatchRecUploadCount);
                List<string> lstBatches = new List<string>();
                DataTable dtBatches = new DataTable();
                string datetimeTickOld = "";

                string FilePath = string.Empty;
                string UploadURL = string.Empty;
                string referer = string.Empty;
                string paramname = string.Empty;
                string spvParamName = string.Empty;
                string spvFilePath = string.Empty;
                bool IsPVDJob = false;
                string sguBulkUploadDocumentsParamName = string.Empty;
                string sguBulkUploadDocumentsFilePath = string.Empty;
                string JobType = "1";

                DataSet dsOld = _reseller.GetDatetimeTickForBatch(recUploadId);
                if (dsOld != null && dsOld.Tables.Count > 0)
                {
                    if (dsOld.Tables[0].Rows.Count > 0)
                    {
                        lstJobIds = dsOld.Tables[0].Rows[0]["JobIds"].ToString().Split(',').ToList();
                        datetimeTickOld = dsOld.Tables[0].Rows[0]["datetimeticks"].ToString();
                        lstBatches = SplitList<string>(lstJobIds, BatchRecUploadCount);
                        dtBatches = ListToDataTable(lstBatches);
                        JobType = dsOld.Tables[0].Rows[0]["JobType"].ToString();
                    }
                }
                if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".csv"))
                {
                    System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".csv");
                }
                if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + "_REC.zip"))
                {
                    System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + "_REC.zip");
                }
                if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".zip"))
                {
                    System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".zip");
                }
                string dateTimeTicks = DateTime.Now.Ticks.ToString();
                foreach (var JobIds in lstBatches)
                {
                    DataSet ds = _createJob.GetJobsForRecInsertOrUpdateNew(JobIds);

                    if (ds.Tables.Count > 0)
                    {
                        // Get Reseller user REC Credentials from User table                    

                        string NotInRecSTCJobDetailsID = "";
                        if (ds.Tables[0].Rows.Count > 0 || ds.Tables[1].Rows.Count > 0)
                        {
                            //string NotInRecJobIds = string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                            //NotInRecSTCJobDetailsID = string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());
                            string NotInRecJobIds = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                            NotInRecSTCJobDetailsID = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());


                            DataSet dsCSV_JobID = new DataSet();
                            DataTable dtSPVXmlPath = new DataTable();

                            #region Generate CSV File Based on Selected Job IDs
                            // Generate CSV Files based on selected job ids and get file path to upload file into REC Registry     


                            FilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".csv";

                            if (JobType == "1") // PVD Jobs
                                _commonBulkUploadToCER.GetBulkUploadCSV_PVD(NotInRecJobIds, FilePath, ref dsCSV_JobID, ref dtSPVXmlPath, true);
                            else  // SWH Jobs
                                _commonBulkUploadToCER.GetBulkUploadSWHCSV(NotInRecJobIds, FilePath, ref dsCSV_JobID, true);

                            if (JobType == "1")
                            {
                                if (dsCSV_JobID.Tables.Count > 0 && dsCSV_JobID.Tables[0] != null)

                                    foreach (DataRow dr in dsCSV_JobID.Tables[0].Rows)
                                    {
                                        string srcPath = ProjectSession.ProofDocuments + "\\JobDocuments\\" + Convert.ToString(dr["JobId"]) + "\\" + dr["Documents Zip File"].ToString();
                                        string fileName = System.IO.Path.GetFileName(srcPath);
                                        string destFolder = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;
                                        string destPath = Path.Combine(destFolder + "\\" + fileName);

                                        if (!Directory.Exists(destFolder))
                                        {
                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();
                                            Directory.CreateDirectory(destFolder);
                                        }

                                        if (System.IO.File.Exists(srcPath))
                                        {
                                            System.IO.File.Copy(srcPath, destPath, true);
                                        }
                                    }

                                #region Creating zip file

                                string InputDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;

                                sguBulkUploadDocumentsFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + "_REC.zip";

                                using (Stream zipStream = new FileStream(Path.GetFullPath(sguBulkUploadDocumentsFilePath), FileMode.Create, FileAccess.Write))
                                using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create))
                                {
                                    bool IsAnyFileExists = false;
                                    foreach (var filePath in System.IO.Directory.GetFiles(InputDirectory, "*.*", System.IO.SearchOption.AllDirectories))
                                    {
                                        var relativePath = Path.GetFileName(filePath);//filePath.Replace(InputDirectory, string.Empty);
                                        using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                        using (Stream fileStreamInZip = archive.CreateEntry(relativePath).Open())
                                            fileStream.CopyTo(fileStreamInZip);

                                        IsAnyFileExists = true;
                                    }
                                    if (!IsAnyFileExists)
                                        sguBulkUploadDocumentsFilePath = "";
                                }
                                #endregion Creating zip file

                                #endregion
                            }

                            #region REC Regsitry call and get response from REC
                            // PVD Jobs
                            if (JobType == "1")
                            {
                                // Zip files for SPV Jobs
                                int spvDataCount = 0;
                                if (dsCSV_JobID.Tables[0] != null)
                                {
                                    spvDataCount = dsCSV_JobID.Tables[0].Select("[Signed data package] <> ''").Count();
                                }
                                if (spvDataCount > 0)
                                {
                                    spvParamName = "sguBulkUploadSdpZip";
                                    spvFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".zip";
                                    //WriteToLogFile("spvfilePath: " + spvFilePathk);
                                    _log.LogException("spvfilePath: " + spvFilePath, null);
                                    using (ZipFile zip = new ZipFile())
                                    {
                                        foreach (DataRow dr in dtSPVXmlPath.Rows)
                                        {
                                            zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, dr["Path"].ToString()), "");
                                            zip.Save(spvFilePath);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                _reseller.ReleaseRecUploadIdForRecreation(recUploadId, dateTimeTicks);
                return this.Json(new { status = true });
            }
            catch (Exception ex)
            {
                return this.Json(new { status = false });
            }
        }
        [HttpGet]
        public JsonResult GetUnknownIssues(string BulkUploadId)
        {
            List<string> lstIssue = new List<string>();
            try
            {
                DataSet ds = _stcInvoiceServiceBAL.GetUnknownIssues(BulkUploadId);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string Issue = string.Empty;
                        Issue = ds.Tables[0].Rows[i]["InternalIssueDescription"].ToString();
                        lstIssue.Add(Issue);
                    }
                }
            }
            catch (Exception ex)
            {
                //WriteToLogFile(ex.Message);
                _log.LogException("GetUnknownIssues BulkUploadId = " + BulkUploadId, ex);
            }
            return Json(new { lstIssue = lstIssue }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePvdCodeFromRec(string recUploadId)
        {
            string FilePath = string.Empty;
            string UploadURL = string.Empty;
            string referer = string.Empty;
            string paramname = string.Empty;
            //bool IsPVDJob = false;
            clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
            try
            {
                DataTable dt = _stcInvoiceServiceBAL.GetJobsForUpdatingPvdCodeFromRec(recUploadId);
                if (dt.Rows.Count > 0)
                {
                    FormBot.Entity.RECAccount objResellerUser = new Entity.RECAccount();
                    objResellerUser.CERLoginId = dt.Rows[0]["CERLoginId"].ToString();
                    objResellerUser.CERPassword = dt.Rows[0]["CERPassword"].ToString();
                    objResellerUser.RECAccName = dt.Rows[0]["RECAccName"].ToString();

                    string InRecJobIds = string.Join(",", dt.AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                    string InRecSTCJobDetailsID = string.Join(",", dt.AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());
                    string JobsForInvoice = string.Join(",", dt.Select("JobsForInvoice <> ''").AsEnumerable().Select(s => s.Field<string>("JobsForInvoice")).ToArray());
                    string BulkUploadId = dt.Rows[0]["GBBatchRECUploadId"].ToString().Split('-')[1];
                    string SerialNumber = string.Join(",", dt.AsEnumerable().Select(s => s.Field<string>("SerialNumbers")).ToArray());
                    string RefNumber = dt.Rows[0]["RefNumber"].ToString();
                    string FromDate = dt.Rows[0]["RECBulkUploadTime"].ToString();
                    string JobType = dt.Rows[0]["JobType"].ToString();
                    int resellerId = Convert.ToInt32(dt.Rows[0]["scResellerId"]);
                    DataSet dsCSV = JobType == "1" ? _createJob.GetBulkUploadForJob(InRecJobIds) : _createJob.GetSWHBulkUploadForJob(InRecJobIds);
                    RECRegistryHelper.AuthenticateUser_UploadFileForREC(ref JsonResponseObj, ref dsCSV, referer, paramname, JobType == "1" ? true : false, objResellerUser, InRecSTCJobDetailsID, ProjectSession.LoggedInUserId, SerialNumber, BulkUploadId, RefNumber, null, FromDate);

                    //// Generating Invoice after rec creation
                    List<string> lstSTCjobDetailsId = new List<string>();
                    if (!string.IsNullOrEmpty(InRecSTCJobDetailsID))
                        lstSTCjobDetailsId = InRecSTCJobDetailsID.Split(',').ToList();

                    if (JsonResponseObj.IsPVDCodeUpdated)
                    {
                        bool IsSuccess = true;
                        _stcInvoiceServiceBAL.UpdateQueuedSubmissionStatus(recUploadId, "Completed");
                        if (!string.IsNullOrEmpty(JobsForInvoice))
                        {
                            List<string> stcjobid = JobsForInvoice.Split(',').ToList();
                            List<string> lstSTCJobId = new List<string>();
                            for (int k = 0; k < stcjobid.Count; k++)
                            {
                                for (int l = 0; l < lstSTCjobDetailsId.Count; l++)
                                {
                                    if (stcjobid[k].Contains(lstSTCjobDetailsId[l] + "_"))
                                        lstSTCJobId.Add(stcjobid[k]);
                                }
                            }
                            IsSuccess = SaveSTCInvoice(resellerId.ToString(), string.Join(",", lstSTCJobId), UserTypeId: ProjectSession.UserTypeId, userId: ProjectSession.LoggedInUserId, IsBackgroundRecProcess: true).Result;
                        }
                        if (IsSuccess)
                        {
                            _log.LogException(SystemEnums.Severity.Info, "Invoice generated successfully.");
                        }
                        else
                        {
                            _log.LogException(SystemEnums.Severity.Info, "Unable to generate invoice");
                        }
                    }
                }
                return this.Json(new { status = JsonResponseObj.IsPVDCodeUpdated });
            }
            catch (Exception ex)
            {
                return this.Json(new { status = false, IsError = true });
            }
        }

        public void getFailureReasonTest()
        {
            DataTable dt = _stcInvoiceServiceBAL.GetJobsForPVDCode();
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
            Thread.Sleep(5000);
            IWebElement ele = driver.FindElement(By.Id("signInName"));
            IWebElement ele2 = driver.FindElement(By.Id("password"));
            ele.SendKeys("hus@emergingenergy.com.au");
            ele2.SendKeys("rXTQ6zZr8RPZXWE!");
            IWebElement ele1 = driver.FindElement(By.Id("next"));
            ele1.Click();
            Thread.Sleep(5000);
            ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
            IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('LAMH62759')").FirstOrDefault();

            if (eleAccount != null)
            {
                eleAccount.Click();
                Thread.Sleep(5000);
            }
            DataTable dtReason = new DataTable();
            dtReason.Clear();
            dtReason.Columns.Add("jobId");
            dtReason.Columns.Add("reason");
            dtReason.Columns.Add("completedTime");
            dtReason.Columns.Add("LastAuditedDate");
            DateTime? lastAuditedDate = null;
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                List<string> lstFailureReasons = new List<string>();


                if (!string.IsNullOrWhiteSpace(dt.Rows[i]["STCPVDCode"].ToString()))
                {
                    RECRegistryHelper.SearchByPVDCode(ref lstFailureReasons, ref lastAuditedDate, dt.Rows[i]["STCPVDCode"].ToString(), 1, "", driver);
                    foreach (var reason in lstFailureReasons)
                    {
                        dtReason.Rows.Add(Convert.ToInt32(dt.Rows[i]["JobId"]), reason, DateTime.Now.ToString("yyyy-MM-dd"));
                    }

                }
            }
            if (dtReason != null && dtReason.Rows.Count > 0)
            {
                InsertRECFailureJobReason(dtReason);
            }
        }

        public JsonResult GetFailureDescription(int jobID, int stcJobDetailsID)
        {
            try
            {
                DataTable dt = _stcInvoiceServiceBAL.GetJobDetailsForPVDCode(jobID, stcJobDetailsID);
                List<string> lstFailureReasons = new List<string>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    RECAccount objAdminUser = new RECAccount();
                    RECAccount objResellerUser = new RECAccount();
                    string pvdCode = dt.Rows[0]["STCPVDCode"].ToString();
                    objAdminUser.CERLoginId = dt.Rows[0]["CERLoginId"].ToString();
                    objAdminUser.CERPassword = dt.Rows[0]["CERPassword"].ToString();
                    objAdminUser.RECAccName = dt.Rows[0]["RECAccName"].ToString();

                    objResellerUser.CERLoginId = dt.Rows[0]["RCERLoginId"].ToString();
                    objResellerUser.CERPassword = dt.Rows[0]["RCERPassword"].ToString();
                    objResellerUser.RECAccName = dt.Rows[0]["RRECAccName"].ToString();

                    DataTable dtReason = new DataTable();
                    dtReason.Clear();
                    dtReason.Columns.Add("jobId");
                    dtReason.Columns.Add("reason");
                    dtReason.Columns.Add("completedTime");
                    dtReason.Columns.Add("LastAuditedDate");
                    DateTime? LastAuditedDate = null;
                    RECRegistryHelper.AuthenticateUser_UploadFileForREC(ref lstFailureReasons, ref LastAuditedDate, objAdminUser, pvdCode, 1, objResellerUser);
                    foreach (var reason in lstFailureReasons)
                    {
                        dtReason.Rows.Add(jobID, reason, DateTime.Now.ToString("yyyy-MM-dd"), LastAuditedDate?.ToString("yyyy-MM-dd"));
                    }
                    if (dtReason != null && dtReason.Rows.Count > 0)
                    {
                        InsertRECFailureJobReason(dtReason);
                    }
                }
                //remove auditor details
                if (lstFailureReasons.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(lstFailureReasons[0]) && lstFailureReasons[0].Contains("Auditor"))
                    {
                        lstFailureReasons[0] = lstFailureReasons[0].Remove(lstFailureReasons[0].IndexOf("Auditor"));
                    }
                    return Json(new { FailureDesc = lstFailureReasons[0] }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { FailureDesc = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { FailureDesc = ex.Message });
            }
        }

        private void InsertRECFailureJobReason(DataTable dtReason)
        {
            try
            {
                _createJob.InsertRECFailureJobReason(dtReason);
            }
            catch (Exception ex)
            {
                Common.Log("An error has occured while Insert Job Failure Reason: " + ex.Message);
            }
        }

        public static IWebElement FindElementSafe(IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public static bool Exists(IWebElement element)
        {
            if (element == null)
            { return false; }
            return true;
        }
        [HttpGet]
        public async Task<JsonResult> RunScheduler(string scheduleDate)
        {
            try
            {
                string jsonRECData = string.Empty;
                //DateTime processDate = Convert.ToDateTime(scheduleDate).AddDays(1);
                //DateTime latestDate = Convert.ToDateTime(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));


                DateTime limitDate = Convert.ToDateTime(ProjectConfiguration.RECDataLimitDate);
                DateTime processDate = Convert.ToDateTime(scheduleDate);
                //DateTime processDate = Convert.ToDateTime("2016-10-14");
                DateTime latestDate = Convert.ToDateTime(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")); 


                //while ((processDate >= limitDate))
                //{
                //    if (RECDataExistForProcessDate(processDate))
                //    {
                //        processDate = processDate.AddDays(1);
                //        break;
                //    }
                //    else
                //    {
                //        processDate = processDate.AddDays(-1);
                //        continue;
                //    }
                //}
                //string jsonRECData = "{\"status\":\"Success\",\"result\":[{\"actionType\":\"STC registered\",\"completedTime\":\"2016-08-31T21:46:57.559Z\",\"certificateRanges\":[{\"certificateType\":\"STC\",\"registeredPersonNumber\":714,\"accreditationCode\":\"OnAppTest\",\"generationYear\":2016,\"generationState\":\"QLD\",\"startSerialNumber\":1,\"endSerialNumber\":31,\"fuelSource\":\"S.G.U. - solar (deemed)\",\"ownerAccount\":\"SolarpowerRex Pty Ltd\",\"ownerAccountId\":2114,\"status\":\"Registered\"}]}]}";
                //while ((processDate >= limitDate) && !RECDataExistForProcessDate(processDate))
                while (processDate <= latestDate)
                {
                    //Common.Log("fetch data from rec-registry.gov.au start for " + processDate.ToString("yyyy-MM-dd"));
                    string url = ProjectConfiguration.RECDataURL + processDate.ToString("yyyy-MM-dd");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.Timeout = 1800000;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonRECData = reader.ReadToEnd();
                    }

                    Common.Log("fetch data from rec-registry.gov.au " + processDate.ToString("yyyy-MM-dd") + " Completed");
                    Common.Log("Prepare DataTable Start");
                    var rootObject = JsonConvert.DeserializeObject<RECDataScheduler>(jsonRECData);
                    if (rootObject.status == "Success")
                    {
                        DataTable dtRECData = createTable();
                        foreach (var result in rootObject.result)
                        {
                            foreach (var cert in result.certificateRanges)
                            {
                                DataRow dr = dtRECData.NewRow();
                                dr["actionType"] = Convert.ToString(result.actionType);
                                dr["completedTime"] = convertedDate(Convert.ToString(result.completedTime)).ToString("yyyy-MM-dd HH:mm:ss");
                                dr["registeredPersonNumber"] = Convert.ToString(cert.registeredPersonNumber);
                                dr["accreditationCode"] = Convert.ToString(cert.accreditationCode);
                                dr["generationYear"] = Convert.ToString(cert.generationYear);
                                dr["generationState"] = Convert.ToString(cert.generationState);
                                dr["startSerialNumber"] = Convert.ToString(cert.startSerialNumber);
                                dr["endSerialNumber"] = Convert.ToString(cert.endSerialNumber);
                                dr["certificateType"] = Convert.ToString(cert.certificateType);
                                dr["fuelSource"] = Convert.ToString(cert.fuelSource);
                                dr["ownerAccount"] = Convert.ToString(cert.ownerAccount);
                                dr["ownerAccountId"] = Convert.ToString(cert.ownerAccountId);
                                dr["status"] = Convert.ToString(cert.status);
                                dtRECData.Rows.Add(dr);
                            }

                        }

                        Common.Log("Prepare DataTable Completed");
                        Common.Log("Insert REC Data call for: " + processDate.ToString("yyyy-MM-dd"));
                        try
                        {
                            DataSet ds = _createJob.InsertRECData(dtRECData, processDate);
                            //Common.Log("Insert REC Data " + processDate.ToString("yyyy-MM-dd") + " Completed");

                            // Find Reason for REC Failure Job and Insert into Reason and JobReason Table
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            {
                                DataTable dtRECFailureJobDetails = ds.Tables[0];
                                DataTable dtReason = new DataTable();
                                dtReason.Clear();
                                dtReason.Columns.Add("jobId");
                                dtReason.Columns.Add("reason");
                                dtReason.Columns.Add("completedTime");
                                dtReason.Columns.Add("LastAuditedDate");
                                for (int i = 0; i < dtRECFailureJobDetails.Rows.Count; i++)
                                {
                                    try
                                    {
                                        FormBot.Entity.RECAccount objAdminUser = new RECAccount();
                                        FormBot.Entity.RECAccount objResellerUser = new RECAccount();
                                        List<string> lstFailureReasons = new List<string>();
                                        string STCPVDCode = Convert.ToString(dtRECFailureJobDetails.Rows[i]["STCPVDCode"]);
                                        objAdminUser.CERLoginId = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RecUserName"]);
                                        objAdminUser.CERPassword = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RecPassword"]);
                                        objAdminUser.RECAccName = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RECAccName"]);
                                        objResellerUser.CERLoginId = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecUserName"]);
                                        objResellerUser.CERPassword = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecPassword"]);
                                        objResellerUser.RECAccName = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRECAccName"]);
                                        int JobID = Convert.ToInt32(dtRECFailureJobDetails.Rows[i]["JobID"]);
                                        DateTime CompletedTime = Convert.ToDateTime(dtRECFailureJobDetails.Rows[i]["CompletedTime"]);
                                        DateTime? LastAuditedDate = null;
                                        int jobType = Convert.ToInt32(dtRECFailureJobDetails.Rows[i]["JobType"]);

                                        if (!string.IsNullOrEmpty(STCPVDCode) && JobID > 0 && jobType > 0)
                                        {
                                            // Get Failure reasons from REC using PVDCode 
                                            RECRegistryHelper.AuthenticateUser_UploadFileForREC(ref lstFailureReasons, ref LastAuditedDate, objAdminUser, STCPVDCode, jobType, objResellerUser);
                                           // Common.Log("Reason for JobID:" + JobID + " Failure reason count: " + lstFailureReasons.Count);
                                            //Insert into Reason and JobReason Table
                                            foreach (var reason in lstFailureReasons)
                                            {
                                                Common.Log("last audited date for jobid:" + LastAuditedDate + "------" + LastAuditedDate?.ToString("yyyy-MM-dd"));
                                                Common.Log("JobID" + JobID + ",completedTime: " + CompletedTime );
                                                dtReason.Rows.Add(JobID, reason, CompletedTime, LastAuditedDate?.ToString("yyyy-MM-dd"));
                                                Common.Log("completed insertion of dtreason for jobid:"+JobID);
                                            }

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Common.Log("Exception in insert dtreason:"+ex.Message.ToString());
                                    }

                                }

                                if (dtReason != null && dtReason.Rows.Count > 0)
                                {
                                    Common.Log("dtreason rows count :" + dtReason.Rows.Count);
                                    InsertRECFailureJobReason(dtReason);
                                    Common.Log("insert rec failure reason completed in db ");
                                }

                                Common.Log("REC Failure reasons inserted into db");
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                            {
                                _jobRules.CreateSTCInvoicePDFForRECData(ds.Tables[1], false);
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                            {
                                Common.Log("start sending email on CER failed");
                                foreach (string id in Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]).Split(','))
                                {
                                    try
                                    {
                                        
                                        int STCJobStageID = 14;
                                        SortedList<string, string> data = new SortedList<string, string>();
                                        string stcStatus = Common.GetDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
                                        string colorCode = Common.GetSubDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
                                        data.Add("STCStatus", stcStatus);
                                        data.Add("ColorCode", colorCode);
                                        data.Add("STCStatusId", STCJobStageID.ToString());
                                       await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(id), null, data, true);
                                       await CommonBAL.SetCacheDataForPeakPay(Convert.ToInt32(id), null, data);
                                    }
                                    catch (Exception ex)
                                    {
                                        Common.Log(ex.Message);
                                    }
                                }
                                _jobRules.SendMailOnCERFailed(Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]), true);
                                Common.Log("end sending email on CER failed for stcjobdetailsId: " + Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]));
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                            {
                                Common.Log(DateTime.Now + "---Enter inside GetRECData ds.tables[3]");
                                try
                                {
                                    List<int> stcJobDetailIds = new List<int>();
                                    stcJobDetailIds = ds.Tables[3].AsEnumerable().Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();

                                    string strstcjobdetailsid = stcJobDetailIds.Count > 0 ? string.Join(",", stcJobDetailIds) : string.Empty;

                                    Common.Log("stcJobdetailsId for update cache: " + strstcjobdetailsid);
                                    for (int i = 0; i < stcJobDetailIds.Count; i++)
                                    {

                                        int STCJobStageID = 22;
                                        SortedList<string, string> data = new SortedList<string, string>();
                                        string stcStatus = Common.GetDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
                                        string colorCode = Common.GetSubDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
                                        data.Add("STCStatus", stcStatus);
                                        data.Add("ColorCode", colorCode);
                                        data.Add("STCStatusId", STCJobStageID.ToString());
                                        data.Add("IsUrgentSubmission", "False");
                                        await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(stcJobDetailIds[i]),null, data, true);
                                        await CommonBAL.SetCacheDataForPeakPay(Convert.ToInt32(stcJobDetailIds[i]), null, data);

                                    }


                                }
                                catch (Exception ex)
                                {
                                    Common.Log(ex.Message);
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            Common.Log(ex.Message.ToString());
                        }

                    }
                    processDate = processDate.AddDays(1);
                }
                return Json(new { status = true, LastSyncDate = latestDate.ToString("yyyy-MM-dd") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(new { status = false });
            }
        }

        public DateTime convertedDate(string strDate)
        {
            //2016-07-31T14:31:33.498Z
            string[] formats = new string[] { "yyyy-MM-ddTHH:mm:ss.fffZ" };
            DateTime retValue;
            if (false == DateTime.TryParseExact(strDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out retValue))
            {
                retValue = DateTime.MinValue;
            }

            retValue = TimeZoneInfo.ConvertTime(retValue, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));
            return retValue;
        }

        public DataTable createTable()
        {
            DataTable dtData = new DataTable();
            DataColumn dc = new DataColumn("actionType", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("completedTime", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("certificateType", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("registeredPersonNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("accreditationCode", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("generationYear", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("generationState", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("startSerialNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("endSerialNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("fuelSource", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("ownerAccount", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("ownerAccountId", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("status", typeof(string));
            dtData.Columns.Add(dc);
            return dtData;
        }
        [HttpGet]
        public JsonResult RegenerateRemittance(string reseller, string path)
        {
            int ResellerID = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            string stcAmountPaymentId= _stcInvoiceServiceBAL.GetSTCAmountPaidId(path);
            if (!string.IsNullOrEmpty(stcAmountPaymentId))
            {
                List<Remittance> remittanceData = _stcInvoiceServiceBAL.GetRemittance(ResellerID, stcAmountPaymentId);
                if (remittanceData != null && remittanceData.Count > 0)
                {
                    for (int i = 0; i < remittanceData.Count; i++)
                    {
                        _generateStcReportBAL.GenerateRemittanceNew(remittanceData[i], reseller);
                    }
                }
            }
            
            return Json(new { success = true });
        }
    }
}
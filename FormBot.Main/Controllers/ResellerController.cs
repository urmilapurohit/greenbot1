using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL;
using FormBot.Helper;
using System.Data;
using Newtonsoft.Json;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.SPV;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Xml;
using FormBot.Entity.Job;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StackExchange.Redis;
using FormBot.Helper.Helper;
using System.Diagnostics;

namespace FormBot.Main.Controllers
{
    public class ResellerController : Controller
    {
        #region Properties
        private readonly IResellerBAL _resellerService;
        private readonly ICreateJobBAL _job;
        private readonly Logger _log;
        private readonly ISolarCompanyBAL _solarCompanyService;
        private readonly UserBAL _userBAL;
        #endregion

        #region Constructor
        public ResellerController(IResellerBAL resellerService, ICreateJobBAL job, ISolarCompanyBAL solarCompanyService)
        {
            this._resellerService = resellerService;
            this._job = job;
            this._log = new Logger();
            this._solarCompanyService = solarCompanyService;
            _userBAL = new UserBAL();
        }
        #endregion

        // GET: Reseller
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get ResellerID and ResellerName.
        /// </summary>
        /// <returns>Returns List of Reseller</returns>
        [HttpGet]
        public JsonResult GetReseller(bool IsPeakPay = false)
        {
            List<SelectListItem> items = _resellerService.GetData(null, IsPeakPay).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetResellerForPricingManager(bool IsWholeSaler)
        {
            List<SelectListItem> items = _resellerService.GetData(null).AsEnumerable().Where(a => a.IsWholeSaler == IsWholeSaler).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSAASForPricingManager()
        {
            List<SelectListItem> items = _resellerService.GetSAASUsers().Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRAMForReseller(string rId)
        {
            int resellerId = !string.IsNullOrEmpty(rId) ? Convert.ToInt32(rId) : 0;
            List<SelectListItem> items = _resellerService.GetResellerAccountManagerByResellerId(resellerId).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get ResellerID and ResellerName for FSA change user .
        /// </summary>
        /// <returns>Returns List of Reseller</returns>
        [HttpGet]
        public JsonResult GetResellerFSAChangeUser(string isTypeChange)
        {
            if (!string.IsNullOrEmpty(isTypeChange) && Convert.ToInt32(isTypeChange) == 1)
                ProjectSession.SystemReseller = null;

            if (ProjectSession.SystemReseller != null)
                return Json(ProjectSession.SystemReseller, JsonRequestBehavior.AllowGet);
            else
            {
                List<SelectListItem> items = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
                ProjectSession.SystemReseller = JsonConvert.SerializeObject(items);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetRecFailureReason()
        {
            REC rEC = new REC();
            return View(@"~\Views\Job\RECFailureReason.cshtml", rEC);
        }



        [HttpGet]
        public ActionResult GetSPVFailureReason(string refJobId = "")
        {
            //DataSet ds = _resellerService.GetSPVFailureReason(refJobId);
            //if (ds.Tables.Count > 0)
            //{
            //    if (ds.Tables[0].Rows.Count > 0)
            //        spvFailureReason.lstHistoryJobId = ds.Tables[0].ToListof<SPVFailureReason>();
            //    if (ds.Tables[1].Rows.Count > 0)
            //        spvFailureReason.lstSPVHistory = ds.Tables[1].ToListof<SPVFailureReason>();
            //}
            SPVFailureReason spvFailureReason = new SPVFailureReason();
            List<JobHistory> jobHistory = new List<JobHistory>();
            int Id = 0;

            Id = !string.IsNullOrEmpty(refJobId) ? Convert.ToInt32(refJobId) : 0;
            if (Id > 0)
            {
                DataSet objDs = _job.GetJobHistory(Id, "DESC", 0);
                if (objDs != null && objDs.Tables.Count > 0)
                {

                    jobHistory = objDs.Tables[0].AsEnumerable().Where(p => p.Field<int>("CategoryID") == 39 || p.Field<int>("CategoryID") == 40 || p.Field<int>("CategoryID") == 41 || p.Field<int>("CategoryID") == 43 || p.Field<int>("CategoryID") == 44 || p.Field<int>("CategoryID") == 47).Select(
                        p => new JobHistory
                        {
                            CategoryID = (p.Field<int>("CategoryID")),
                            HistoryCategory = (p.Field<string>("HistoryCategory")),
                            HistoryMessage = (p.Field<string>("HistoryMessage")).Replace("DownloadDocument(this)", "DownloadHistoryDocument(this)"),
                            IsSSC = (p.Field<bool>("IsSSC")),
                            JobHistoryID = (p.Field<int>("JobHistoryID")),
                            JobID = (p.Field<int>("JobID")),
                            ModifiedBy = (p.Field<int>("ModifiedBy")),
                            CreateDate = (p.Field<DateTime>("ModifiedDate")).ToString("dd/MM/yyyy hh:mmtt"),
                            ModifiedDate = (p.Field<DateTime>("ModifiedDate")),
                            Modifier = (p.Field<string>("Modifier")),
                            Title = (p.Field<string>("Title")),
                            RefNumber = (p.Field<string>("RefNumber")),
                            //currentPage = (p.Field<int>("currentPage")),
                            TotalRecords = (p.Field<int>("TotalRecords")),
                            FilterID = (p.Field<int>("FilterID")),
                            IsImportant = false

                        }).ToList();
                }

                string JobHistoryDirectoryPath = Path.Combine(ProjectConfiguration.ProofDocumentsURL, "JobDocuments", Id.ToString(), "JobHistory");
                string JobHistoryFilePath = Path.Combine(JobHistoryDirectoryPath, "JobHistory_" + Id.ToString() + ".xml");
                if (System.IO.File.Exists(JobHistoryFilePath))
                {
                    string refNumber = _job.GetRefNumberByJobId(Id);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(JobHistoryFilePath);
                    XmlNodeList History = doc.DocumentElement.SelectNodes("/JobHistory/History");

                    foreach (XmlNode node in History)
                    {
                        string Category = node.SelectSingleNode("Category").InnerText;
                        string Filter = node.SelectSingleNode("Filter").InnerText;
                        int CategoryID = !string.IsNullOrEmpty(Category) ? Convert.ToInt32((HistoryCategory)Enum.Parse(typeof(HistoryCategory), Category).GetHashCode()) : 0;
                        int FilterID = !string.IsNullOrEmpty(Filter) ? Convert.ToInt32((SystemEnums.JobHistoryFilter)Enum.Parse(typeof(SystemEnums.JobHistoryFilter), Filter).GetHashCode()) : 0;
                        JobHistory objjobhistory = new JobHistory();
                        objjobhistory.HistoryCategory = Category;
                        objjobhistory.CategoryID = CategoryID;
                        objjobhistory.HistoryMessage = node.SelectSingleNode("JobHistoryMessage").InnerText;
                        objjobhistory.JobID = Convert.ToInt32(node.SelectSingleNode("JobID").InnerText);
                        objjobhistory.CreateDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText).ToString("dd/MM/yyyy hh:mmtt");
                        objjobhistory.IsImportant = Convert.ToBoolean(node.SelectSingleNode("IsImportant").InnerText);
                        objjobhistory.ModifiedDate = Convert.ToDateTime(node.SelectSingleNode("CreatedDate").InnerText);
                        objjobhistory.Modifier = node.SelectSingleNode("CreatedBy").InnerText;
                        objjobhistory.FilterID = FilterID;
                        objjobhistory.RefNumber = refNumber;
                        jobHistory.Add(objjobhistory);
                    }
                }
                jobHistory = jobHistory.Where(m => m.CategoryID == 39 || m.CategoryID == 40 || m.CategoryID == 41 || m.CategoryID == 43 || m.CategoryID == 44 || m.CategoryID == 47).ToList();
                jobHistory = jobHistory.OrderByDescending(m => m.ModifiedDate).ToList();
                List<SPVFailureReason> lstSPVHistoryReason = new List<SPVFailureReason>();
                foreach (var item in jobHistory)
                {
                    SPVFailureReason FailureReason = new SPVFailureReason();

                    FailureReason.JobID = item.JobID;
                    FailureReason.HistoryCategory = item.HistoryCategory;
                    FailureReason.HistoryMessage = item.HistoryMessage;
                    FailureReason.ModifiedBy = item.Modifier;
                    FailureReason.CreateDate = item.CreateDate;
                    FailureReason.RefNumber = item.RefNumber;
                    lstSPVHistoryReason.Add(FailureReason);
                }
                spvFailureReason.lstSPVHistory = lstSPVHistoryReason;
                spvFailureReason.lstHistoryJobId = lstSPVHistoryReason;
            }

            return PartialView(@"~\Views\Job\_SPVFailureReason.cshtml", spvFailureReason);
        }

        public JsonResult DeleteRecFailureReason(string id)
        {
            try
            {
                _resellerService.DeleteRecFailureReason(id);
                return this.Json(new { success = true });
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false });
            }
        }

        //public JsonResult ReleaseForRecreation(string recUploadId)
        //{
        //    try
        //    {
        //        string datetimeTickOld = _resellerService.GetDatetimeTickForBatch(recUploadId);
        //        if(System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".csv"))
        //        {
        //            System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".csv");
        //        }
        //        if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + "_REC.zip"))
        //        {
        //            System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + "_REC.zip");
        //        }
        //        if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".zip"))
        //        {
        //            System.IO.File.Delete(ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeTickOld + ".zip");
        //        }
        //        long dateTimeTicks = DateTime.Now.Ticks;
        //        string FilePath = string.Empty;
        //        string UploadURL = string.Empty;
        //        string referer = string.Empty;
        //        string paramname = string.Empty;
        //        string spvParamName = string.Empty;
        //        string spvFilePath = string.Empty;
        //        bool IsPVDJob = false;
        //        string sguBulkUploadDocumentsParamName = string.Empty;
        //        string sguBulkUploadDocumentsFilePath = string.Empty;
        //        #region Creating zip file

        //        string InputDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;

        //        sguBulkUploadDocumentsFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + "_REC.zip";

        //        using (Stream zipStream = new FileStream(Path.GetFullPath(sguBulkUploadDocumentsFilePath), FileMode.Create, FileAccess.Write))
        //        using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create))
        //        {
        //            bool IsAnyFileExists = false;
        //            foreach (var filePath in System.IO.Directory.GetFiles(InputDirectory, "*.*", System.IO.SearchOption.AllDirectories))
        //            {
        //                var relativePath = Path.GetFileName(filePath);//filePath.Replace(InputDirectory, string.Empty);
        //                using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //                using (Stream fileStreamInZip = archive.CreateEntry(relativePath).Open())
        //                    fileStream.CopyTo(fileStreamInZip);

        //                IsAnyFileExists = true;
        //            }
        //            if (!IsAnyFileExists)
        //                sguBulkUploadDocumentsFilePath = "";
        //        }
        //        #endregion Creating zip file

        //        _resellerService.ReleaseRecUploadIdForRecreation(recUploadId);
        //        return this.Json(new { status = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Json(new { status = false });
        //    }
        //}

        ///// <summary>
        ///// Get ResellerID and ResellerName for FSA change user .
        ///// </summary>
        ///// <returns>Returns List of Reseller</returns>
        //[HttpGet]
        //public JsonResult GetResellerFSAChangeUser()
        //{
        //    if (ProjectSession.SystemResellerTable != null && ProjectSession.SystemResellerTable.Rows.Count > 0)
        //    {
        //        DataTable dtReseller = ProjectSession.SystemResellerTable;
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        for (int i = 0; i < dtReseller.Rows.Count; i++)
        //        {
        //            items.Add(new SelectListItem()
        //            {
        //                Text = dtReseller.Rows[i]["ResellerName"].ToString(),
        //                Value = dtReseller.Rows[i]["ResellerID"].ToString()
        //            });
        //        }
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }

        //    else
        //    {
        //        List<SelectListItem> items = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
        //        DataTable dtReseller = new DataTable();
        //        dtReseller.Columns.Add("ResellerID", typeof(string));
        //        dtReseller.Columns.Add("ResellerName", typeof(string));
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            dtReseller.Rows.Add(new object[] { items[i].Value, items[i].Text });
        //        }
        //        ProjectSession.SystemResellerTable = dtReseller;
        //        return Json(items, JsonRequestBehavior.AllowGet);
        //    }
        //    //List<SelectListItem> items = _resellerService.GetData(null).Select(a => new SelectListItem { Text = a.ResellerName, Value = a.ResellerID.ToString() }).ToList();
        //    //return Json(items, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// create cache for all reseller in job index page and stc submission 
        /// </summary>
        /// <returns></returns>
        #region Fetch SolarCompanyWise JobList Data From Cache with Reseller for jobs
        public async Task SetAllResellerDataInCacheForJobs(int year)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                List<int> lstResellerID = _resellerService.GetData(null, false).Select(a => a.ResellerID).ToList();
                foreach (var RAId in lstResellerID)
                {
                    List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByResellerID(RAId).Select(X => X.SolarCompanyId).ToList();
                    if (lstSCAID.Any())
                    {
                        DataSet dsAllColumnsData = new DataSet();
                        var lstJobsFull = new List<JobView>();
                        CommonBAL.JobsDistributedCacheHashSet(cache, lstSCAID, year, ref dsAllColumnsData, ref lstJobsFull);
                        //DataSet dsAllColumnsData = _job.GetJobList_ForCachingDataKendo(string.Join(",", lstSCAID), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                        //foreach (var scaID in lstSCAID)
                        //{
                        //    DataTable dtSolarCompData = null;
                        //    var data = dsAllColumnsData.Tables[0].AsEnumerable().Where(dr => dr.Field<int>("SolarCompanyId") == scaID).Select(dr => dr).ToList();
                        //    if (data.Any())
                        //    {
                        //        dtSolarCompData = data.CopyToDataTable();
                        //    }
                        //    CacheConfiguration.Set(RedisCacheConfiguration.dsJobIndex + "_" + scaID, dtSolarCompData);
                        //}
                    }
                    //CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(RAId);
                }
                //return RedirectToAction("JobSetting", "JobSetting");
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForJobs action", ex);
                // return RedirectToAction("JobSetting", "JobSetting");
            }
        }

        /// <summary>
        /// create cache for all reseller in job index page and stc submission New
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mainJobCache"></param>
        /// <returns></returns>
        public async Task SetAllResellerDataInCacheForJobsMissing(int year, List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string resellerIDs = string.Join(",", _resellerService.GetData(null, false).Select(a => a.ResellerID.ToString()).ToArray());
                List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                #region Call SP for those lstSolarId which are not found in CacheData

                lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();
                if (lstSolarId != null && lstSolarId.Count > 0)
                {
                    CommonBAL.JobsDistributedCacheHashSet(cache, lstSolarId, year);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForJobs action", ex);
            }
        }
        #endregion


        #region Fetch SolarCompanyWise JobList Data From Cache with Reseller for stcsubmission
        public async Task SetAllResellerDataInCacheForStcSubmission(int year)
        {
            try
            {
                List<int> lstResellerID = _resellerService.GetData(null, false).Select(a => a.ResellerID).ToList();
                foreach (var RAId in lstResellerID)
                {
                    await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(RAId, "0", year);
                }
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForStcSubmission action", ex);
            }
        }

        /// <summary>
        /// Set All Reseller Data In Cache For Stc Submission New
        /// </summary>
        /// <param name="year"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public async Task SetAllResellerDataInCacheForStcSubmissionMissing(int year, List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache)
        {
            try
            {
                string resellerIDs = string.Join(",", _resellerService.GetData(null, false).Select(a => a.ResellerID.ToString()).ToArray());
                List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                #region Call SP for those lstSolarId which are not found in CacheData

                lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();
                if (lstSolarId != null && lstSolarId.Count > 0)
                {
                    string solarIds = string.Join(",", lstSolarId.Select(s => s.ToString()).ToArray());
                    await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(0, solarIds, year);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForStcSubmission action", ex);
            }
        }

        public async Task SetAllResellerDataInCacheForPeakPay()
        {
            try
            {
                List<int> lstResellerID = _resellerService.GetData(null, true).Select(a => a.ResellerID).ToList();
                foreach (var RAId in lstResellerID)
                {
                    await CommonBAL.SetCacheDataForPeakPayFromSolarCompanyId("", RAId);
                }
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForPeakPay action", ex);
            }
        }

        /// <summary>
        /// Set All Reseller Data In Cache For PeakPay
        /// </summary>
        /// <param name="mainJobCache"></param>
        /// <returns></returns>
        public async Task SetAllResellerDataInCacheForPeakPayMissing(List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache)
        {
            try
            {
                string resellerIDs = string.Join(",", _resellerService.GetData(null, true).Select(a => a.ResellerID.ToString()).ToArray());
                List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                #region Call SP for those lstSolarId which are not found in CacheData

                lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();
                if (lstSolarId != null && lstSolarId.Count > 0)
                {
                    string solarIds = string.Join(",", lstSolarId.Select(s => s.ToString()).ToArray());
                    await CommonBAL.SetCacheDataForPeakPayFromSolarCompanyId(solarIds, 0);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForPeakPay action", ex);
            }
        }

        #endregion

        /// <summary>
        /// create cache for stc submission solar compnay wise  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCacheForSTCSubmission(List<string> resellerID, string solarCompanyId, int year = 0, bool isMissingOnly = false)
        {
            try
            {
                CommonBAL.ValidateYearForGridData(ref year);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string mainCacheKey = string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year);
                List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainCacheKey);

                var AllReseller = resellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    if (isMissingOnly)
                        await SetAllResellerDataInCacheForStcSubmissionMissing(year, mainJobCache);
                    else
                        await SetAllResellerDataInCacheForStcSubmission(year);
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        string resellerIDs = string.Join(",", resellerID.ToArray());
                        List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                        if (isMissingOnly)
                            lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();

                        if (lstSolarId != null && lstSolarId.Count > 0)
                        {
                            string solarIds = string.Join(",", lstSolarId.Select(s => s.ToString()).ToArray());
                            await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(0, solarIds, year);
                        }
                    }
                    else
                    {
                        if (!isMissingOnly || (isMissingOnly && !string.IsNullOrEmpty(solarCompanyId) && !mainJobCache.Any(R => R.PID == Convert.ToInt32(solarCompanyId))))
                            await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(0, solarCompanyId, year);
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForStcSubmission action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Create Cache For STCSubmission Non Approved
        /// </summary>
        /// <param name="lstResellerID"></param>
        /// <param name="solarCompanyId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCacheForSTCSubmissionNonApproved(List<string> lstResellerID, string solarCompanyId, int year = 0,bool isMissingOnly = false)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string mainCacheKey = string.Format(RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey, year);
                List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainCacheKey);

                var AllReseller = lstResellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    if(isMissingOnly)
                        await SetAllResellerDataInCacheForStcSubmissionMissing(year, mainJobCache);
                    else
                        await SetAllResellerDataInCacheForStcSubmission(year);
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        string resellerIDs = string.Join(",", lstResellerID.ToArray());
                        List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                        if(isMissingOnly)
                            lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();

                        if (lstSolarId != null && lstSolarId.Count > 0)
                        {
                            string solarIds = string.Join(",", lstSolarId.Select(s => s.ToString()).ToArray());
                            await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyIdNonApproved(0, solarIds, year);
                        }
                    }
                    else
                    {
                        if (!isMissingOnly || (isMissingOnly && !string.IsNullOrEmpty(solarCompanyId) && !mainJobCache.Any(R => R.PID == Convert.ToInt32(solarCompanyId))))
                        {
                            await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyIdNonApproved(0, solarCompanyId, year);
                        }
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForStcSubmission action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the cache for peak pay.
        /// </summary>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="isMissingOnly">if set to <c>true</c> [is missing only].</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCacheForPeakPay(List<string> resellerID, string solarCompanyId, bool isMissingOnly = false)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string mainCacheKey = RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey;
                List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainCacheKey);

                var AllReseller = resellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    if(isMissingOnly)
                        await SetAllResellerDataInCacheForPeakPayMissing(mainJobCache);
                    else
                        await SetAllResellerDataInCacheForPeakPay();
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        string resellerIDs = string.Join(",", resellerID.ToArray());
                        List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(resellerIDs).Select(s => s.SolarCompanyId).ToList();

                        if(isMissingOnly)
                            lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();

                        if (lstSolarId != null && lstSolarId.Count > 0)
                        {
                            string solarIds = string.Join(",", lstSolarId.Select(s => s.ToString()).ToArray());
                            await CommonBAL.SetCacheDataForPeakPayFromSolarCompanyId(solarIds, 0);
                        }
                    }
                    else
                    {
                        if (!isMissingOnly || (isMissingOnly && !string.IsNullOrEmpty(solarCompanyId) && !mainJobCache.Any(R => R.PID == Convert.ToInt32(solarCompanyId))))
                        {
                            await CommonBAL.SetCacheDataForPeakPayFromSolarCompanyId(solarCompanyId, 0);
                        }
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForPeakPay action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ClearCacheForPeakPay(string resellerID, string solarCompanyId)
        {
            try
            {
                int RAId = Convert.ToInt32(resellerID);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                if (RAId == 0)
                {
                    List<int> lstResellerID = _resellerService.GetData(null, true).Select(a => a.ResellerID).ToList();
                    foreach (var RaId in lstResellerID)
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByResellerID(RaId).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, RedisCacheConfiguration.dsPeakPayHashKey);
                            //foreach (int solarCompId in lstSCAID)
                            //{
                            //    if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsPeakPayIndex + "_" + solarCompId))
                            //        CacheConfiguration.Remove(CacheConfiguration.dsPeakPayIndex + "_" + solarCompId);
                            //}
                        }
                    }
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByResellerID(RAId).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, RedisCacheConfiguration.dsPeakPayHashKey);
                            //foreach (int solarCompId in lstSCAID)
                            //{
                            //    if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsPeakPayIndex + "_" + solarCompId))
                            //        CacheConfiguration.Remove(CacheConfiguration.dsPeakPayIndex + "_" + solarCompId);
                            //}
                        }
                    }

                    else
                    {
                        CommonBAL.RemoveHashKeys(cache, new List<int>() { Convert.ToInt32(solarCompanyId) }, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, RedisCacheConfiguration.dsPeakPayHashKey);
                        //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsPeakPayIndex + "_" + solarCompanyId))
                        //    CacheConfiguration.Remove(CacheConfiguration.dsPeakPayIndex + "_" + solarCompanyId);
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in ClearAllResellerDataInCacheForPeakPay action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyStatusPeakPay(string solarCompanyId)
        {
            try
            {
                if (solarCompanyId != "" || solarCompanyId != "0")
                {
                    IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                    List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = CommonBAL.DistributedCacheAllKeysInfoGet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);

                    if (parentCaches.Any(d => d.PID == Convert.ToInt32(solarCompanyId)))
                        return Json(new { status = true, key = "" }, JsonRequestBehavior.AllowGet);

                    //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsPeakPayIndex + "_" + solarCompanyId))
                    //{
                    //    var data = CacheConfiguration.Get<DataTable>(CacheConfiguration.dsPeakPayIndex + "_" + solarCompanyId);
                    //    return Json(new { status = true, key = CacheConfiguration.dsPeakPayIndex + "_" + solarCompanyId }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    //}
                }
                else
                {
                    //return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                //return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// clear cache for stc submission solar compnay wise  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ClearCacheForSTCSubmission(List<string> resellerID, string solarCompanyId, int year = 0)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                CommonBAL.ValidateYearForGridData(ref year);
                //int RAId = Convert.ToInt32(resellerID);
                string ResellerIds = string.Join(",", resellerID);
                var AllReseller = resellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    List<int> lstResellerID = _resellerService.GetData(null, false).Select(a => a.ResellerID).ToList();
                    foreach (var RaId in lstResellerID)
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByMultipleResellerID(RaId.ToString()).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, RedisCacheConfiguration.dsSTCHashKey, year);
                        }
                    }
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByMultipleResellerID(ResellerIds).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, RedisCacheConfiguration.dsSTCHashKey, year);
                        }
                    }

                    else
                    {
                        CommonBAL.RemoveHashKeys(cache, new List<int>() { Convert.ToInt32(solarCompanyId) }, RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, RedisCacheConfiguration.dsSTCHashKey, year);
                        //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsSTCSubmissionIndex + "_" + solarCompanyId))
                        //    CacheConfiguration.Remove(CacheConfiguration.dsSTCSubmissionIndex + "_" + solarCompanyId);
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForStcSubmission action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// create cache for job listing solar compnay wise  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCacheForJobListing(List<string> resellerID, string solarCompanyId, int year = 0, bool isMissingOnly = false)
        {
            try
            {
                CommonBAL.ValidateYearForGridData(ref year);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string mainJobCacheKey = string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year);
                List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainJobCacheKey);

                string ResellerIds = string.Join(",", resellerID);
                var AllReseller = resellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    if(isMissingOnly)
                        await SetAllResellerDataInCacheForJobsMissing(year, mainJobCache);
                    else
                        await SetAllResellerDataInCacheForJobs(year);
                }
                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        List<int> lstSolarId = _solarCompanyService.GetSolarCompanyByMultipleResellerID(ResellerIds).Select(s => s.SolarCompanyId).ToList();

                        if(isMissingOnly)
                            lstSolarId = lstSolarId.Where(X => mainJobCache == null || !mainJobCache.Any(R => R.PID == Convert.ToInt32(X))).Select(X => Convert.ToInt32(X)).ToList();

                        if (lstSolarId != null && lstSolarId.Count > 0)
                        {
                            CommonBAL.JobsDistributedCacheHashSet(cache, lstSolarId, year);
                        }
                    }
                    else
                    {
                        if (!isMissingOnly || (isMissingOnly && !string.IsNullOrEmpty(solarCompanyId) && !mainJobCache.Any(R => R.PID == Convert.ToInt32(solarCompanyId))))
                        {
                            CommonBAL.JobsDistributedCacheHashSet(cache, new List<int> { Convert.ToInt32(solarCompanyId) }, year);
                        }
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForJobListing action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// clear cache for stc submission solar compnay wise  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ClearCacheForJobListing(List<string> resellerID, string solarCompanyId, int year = 0)
        {
            try
            {
                CommonBAL.ValidateYearForGridData(ref year);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                string ResellerIds = string.Join(",", resellerID);
                var AllReseller = resellerID.Where(x => x == "0").FirstOrDefault();
                if (AllReseller != null)
                {
                    //int RAId = Convert.ToInt32(resellerID);
                    //if (RAId == 0)
                    //{
                    List<int> lstResellerID = _resellerService.GetData(null, false).Select(a => a.ResellerID).ToList();
                    foreach (var RaId in lstResellerID)
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByMultipleResellerID(RaId.ToString()).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsJobAllKeysInfoHashKey, RedisCacheConfiguration.dsJobIndex, year);
                            //foreach (int solarCompId in lstSCAID)
                            //{
                            //    if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsJobIndex + "_" + solarCompId))
                            //        CacheConfiguration.Remove(CacheConfiguration.dsJobIndex + "_" + solarCompId);
                            //}
                        }
                    }
                }

                else
                {
                    if (solarCompanyId == null || solarCompanyId == "" || solarCompanyId == "-1" || solarCompanyId == "0")
                    {
                        List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByMultipleResellerID(ResellerIds).Select(X => X.SolarCompanyId).ToList();
                        if (lstSCAID.Any())
                        {
                            CommonBAL.RemoveHashKeys(cache, lstSCAID, RedisCacheConfiguration.dsJobAllKeysInfoHashKey, RedisCacheConfiguration.dsJobIndex, year);
                            //foreach (int solarCompId in lstSCAID)
                            //{
                            //    if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsJobIndex + "_" + solarCompId))
                            //        CacheConfiguration.Remove(CacheConfiguration.dsJobIndex + "_" + solarCompId);
                            //}
                        }
                    }

                    else
                    {
                        CommonBAL.RemoveHashKeys(cache, new List<int>() { Convert.ToInt32(solarCompanyId) }, RedisCacheConfiguration.dsJobAllKeysInfoHashKey, RedisCacheConfiguration.dsJobIndex, year);
                        //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsJobIndex + "_" + solarCompanyId))
                        //    CacheConfiguration.Remove(CacheConfiguration.dsJobIndex + "_" + solarCompanyId);
                    }
                }

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in SetAllResellerDataInCacheForJobListing action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> CacheKeyStatusSTC(string solarCompanyId, int year = 0)
        {
            try
            {
                if (solarCompanyId != "" || solarCompanyId != "0")
                {
                    IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                    CommonBAL.ValidateYearForGridData(ref year);
                    string mainKey = string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year);
                    List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainKey);

                    if (parentCaches.Any(d => d.PID == Convert.ToInt32(solarCompanyId)))
                        return Json(new { status = true, key = "" }, JsonRequestBehavior.AllowGet);


                    //if (CacheConfiguration.IsContainsKey(key))
                    //{
                    //    var data = CacheConfiguration.Get<DataTable>(key);
                    //    return Json(new { status = true, key = key }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    //}
                }
                //else
                //{
                //    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                //}
            }

            catch (Exception ex)
            {
                //return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyStatus(int year, bool isSTC, bool isJob, bool isPeakPay,bool isSTCMain, bool isParent, int jobID, int solarCompanyID = 0)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                if (isParent && year > 0)
                {
                    var timer = new Stopwatch();
                    string mainRedisKey = isSTCMain ? string.Format(RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey, year) :
                                          isSTC ? string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year) :
                                          (isJob ? string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year) :
                                          isPeakPay ? RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey : string.Empty);
                    if (!string.IsNullOrEmpty(mainRedisKey))
                    {
                        timer.Start();
                        List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                        timer.Stop();
                        Log.WriteLog("CacheKeyStatus Redis Main Key -   Time taken " + timer.ElapsedMilliseconds);
                        return Json(new { status = true, key = mainRedisKey, value = parentCaches }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (!isParent && solarCompanyID > 0 && jobID > 0)
                {
                    string mainRedisKey = isSTC || isSTCMain ? string.Format(RedisCacheConfiguration.dsSTCHashKey, solarCompanyID) :
                                          (isJob ? string.Format(RedisCacheConfiguration.dsJobHashKey, solarCompanyID) :
                                          isPeakPay ? string.Format(RedisCacheConfiguration.dsPeakPayHashKey, solarCompanyID) : string.Empty);
                    if (!string.IsNullOrEmpty(mainRedisKey))
                    {
                        if (isSTC || isSTCMain)
                        {
                            var data = RedisCacheConfiguration.GetHash<STCSubmissionView>(cache, mainRedisKey, jobID);
                            return Json(new { status = true, key = mainRedisKey + ">" + jobID, value = data }, JsonRequestBehavior.AllowGet);
                        }
                        else if (isJob)
                        {
                            var data = RedisCacheConfiguration.GetHash<JobView>(cache, mainRedisKey, jobID);
                            return Json(new { status = true, key = mainRedisKey + ">" + jobID, value = data }, JsonRequestBehavior.AllowGet);
                        }
                        else if (isPeakPay)
                        {
                            var data = RedisCacheConfiguration.GetHash<PeakPayView>(cache, mainRedisKey, jobID);
                            return Json(new { status = true, key = mainRedisKey + ">" + jobID, value = data }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is use for Set Cache Status
        /// </summary>
        /// <param name="solarCompanyID"></param>
        /// <param name="jobID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SetCacheKeyStatus(int jobID, int solarCompanyID = 0)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                if (jobID > 0)
                {
                    await CommonBAL.SetCacheDataForJobID(solarCompanyID, jobID);

                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyStatusInstallerDesigner(int instDesgID)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                var data = RedisCacheConfiguration.GetHash<JobsInstallerDesignerView>(cache, RedisCacheConfiguration.dsInstallerDesignerHashKey, instDesgID);
                return Json(new { status = true, key = RedisCacheConfiguration.dsInstallerDesignerHashKey + ">" + instDesgID, value = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyStatusSolarCompany(int solarCompanyID)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                var data = RedisCacheConfiguration.GetHash<JobsSolarCompanyView>(cache, RedisCacheConfiguration.dsSolarCompanyHashKey, solarCompanyID);
                return Json(new { status = true, key = RedisCacheConfiguration.dsSolarCompanyHashKey + ">" + solarCompanyID, value = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyResetSolarCompany()
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                List<JobsSolarCompanyView> jobsSolarCompanyViews = new List<JobsSolarCompanyView>();
                CommonBAL.SolarCompanyDistributedCacheHashSet(cache, ref jobsSolarCompanyViews);
                return Json(new { status = true, key = RedisCacheConfiguration.dsSolarCompanyHashKey, value = "Key has reset successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyResetInstallerDesginer()
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                List<JobsInstallerDesignerView> jobsInstallerDesignerView = new List<JobsInstallerDesignerView>();
                CommonBAL.InstallerDesignerDistributedCacheHashSet(cache, ref jobsInstallerDesignerView);
                return Json(new { status = true, key = RedisCacheConfiguration.dsInstallerDesignerHashKey, value = "Key has reset successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CacheKeyStatusJob(string solarCompanyId, int year = 0)
        {
            try
            {
                if (solarCompanyId != "" || solarCompanyId != "0")
                {
                    IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                    CommonBAL.ValidateYearForGridData(ref year);
                    string mainKey = string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year);
                    List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainKey);

                    if (parentCaches.Any(d => d.PID == Convert.ToInt32(solarCompanyId)))
                        return Json(new { status = true, key = "" }, JsonRequestBehavior.AllowGet);

                    //string key = (CacheConfiguration.dsJobIndex + "_" + solarCompanyId);

                    //if (CacheConfiguration.IsContainsKey(key))
                    //{
                    //    var data = CacheConfiguration.Get<DataTable>(key);
                    //    return Json(new { status = true, key = key }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    //}
                }
                //else
                //{
                //    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                //}
            }

            catch (Exception ex)
            {
                //return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get REC Accounts for Given Username and Password for dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetRECAccounts(string recUsername, string recPassword)
        {
            List<RECAccount> Items = new List<RECAccount>();
            bool isAccess = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");
            IWebDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            try
            {
                driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
                Thread.Sleep(5000);
                IWebElement ele = driver.FindElement(By.Id("signInName"));
                IWebElement ele2 = driver.FindElement(By.Id("password"));
                ele.SendKeys(recUsername);
                ele2.SendKeys(recPassword);
                IWebElement ele1 = driver.FindElement(By.Id("next"));
                ele1.Click();
                Thread.Sleep(5000);

                if (Exists(FindElementSafe(driver, By.ClassName("error"))))
                {
                    IWebElement eleError = driver.FindElement(By.ClassName("error"));
                    if (eleError != null)
                    {
                        if (eleError.Displayed)
                        {
                            return Json(new { status = false, message = eleError.Text }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));

                    if (lstEle != null && lstEle.Count > 0)
                    {
                        for (int i = 0; i < lstEle.Count; i++)
                        {
                            string companyname = lstEle[i].GetAttribute("value");
                            string username = lstEle[i].GetAttribute("onclick").Replace("submitUser('", "").Replace("')", "");
                            Items.Add(new RECAccount { RECCompName = companyname, RECAccName = username, RECName = "Hus Lam" });
                        }
                    }
                    else
                    {
                        IWebElement divAcc = driver.FindElement(By.Id("top-right-box"));
                        if (divAcc != null)
                        {
                            string[] strArr = divAcc.Text.Split('|');
                            string companyname = strArr[0].Replace("Account:", "").Trim();
                            string username = strArr[1].Replace("User:", "").Trim();
                            driver.Navigate().GoToUrl(ProjectConfiguration.RECSearchURL);
                            Thread.Sleep(5000);
                            if (FindElementSafe(driver, By.LinkText("Register bulk SGU")) == null)
                            {
                                isAccess = false;
                            }
                            Items.Add(new RECAccount { RECCompName = companyname, RECAccName = "Normal User", RECName = username });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                driver.Quit();
            }
            return Json(new { status = true, Items = Items, isAccess = isAccess }, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetRECName(string recUsername, string recPassword, string RECAccName)
        {
            List<RECAccount> Items = new List<RECAccount>();
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddExcludedArgument("enable-automation");
            //options.AddArguments("headless");
            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            string RECName = "";
            bool isAccess = true;
            try
            {
                driver.Navigate().GoToUrl(ProjectConfiguration.RECAuthURL);
                Thread.Sleep(5000);
                IWebElement ele = driver.FindElement(By.Id("signInName"));
                IWebElement ele2 = driver.FindElement(By.Id("password"));
                ele.SendKeys(recUsername);
                ele2.SendKeys(recPassword);
                IWebElement ele1 = driver.FindElement(By.Id("next"));
                ele1.Click();
                Thread.Sleep(5000);

                ReadOnlyCollection<IWebElement> lstEle = driver.FindElements(By.ClassName("btn-primary"));
                IWebElement eleAccount = lstEle.Where(a => a.GetAttribute("onclick") == "submitUser('" + RECAccName + "')").FirstOrDefault();

                if (eleAccount != null)
                {
                    eleAccount.Click();
                    Thread.Sleep(2000);
                    IWebElement divAcc = driver.FindElement(By.Id("top-right-box"));
                    if (divAcc != null)
                    {
                        RECName = divAcc.Text.Split('|')[1].Replace("User:", "").Trim();
                    }
                    driver.Navigate().GoToUrl(ProjectConfiguration.RECSearchURL);
                    Thread.Sleep(5000);
                    if (FindElementSafe(driver, By.LinkText("Register bulk SGU")) == null)
                    {
                        isAccess = false;
                    }
                }
                return Json(new { status = true, RECName = RECName, isAccess = isAccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                driver.Quit();
            }
        }

        [HttpGet]
        public JsonResult CheckResellerRECCredentials(string resellerID, string JobIds)
        {
            try
            {
                DataSet ds = _job.CheckStatusOfSubmissionInRec(JobIds);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<int> lstJobIds = new List<int>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int jobid = 0;
                        if (ds.Tables[0].Rows[i]["RecStatus"].ToString() == "InProgress" || ds.Tables[0].Rows[i]["RecStatus"].ToString() == "SuccessfullyUploaded")
                        {
                            jobid = Convert.ToInt32(ds.Tables[0].Rows[i]["JobID"].ToString());
                            lstJobIds.Add(jobid);
                        }
                    }
                    if (lstJobIds.Count > 0)
                    {
                        return Json(new { status = false, lstJobIds = lstJobIds, strlstJobIds = string.Join(",", lstJobIds) }, JsonRequestBehavior.AllowGet);
                    }
                }
                int RAId = Convert.ToInt32(resellerID);
                if (RAId == 0)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<FormBot.Entity.RECAccount> objResellerUser = new List<FormBot.Entity.RECAccount>();
                    objResellerUser = _userBAL.GetAllResellerUserRECCredentialsByResellerID(Convert.ToInt32(resellerID));
                    if (objResellerUser != null)
                    {
                        return Json(new { status = true, ResellerDetails = objResellerUser }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }


}
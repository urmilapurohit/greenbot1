using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Helper;
using FormBot.Helper.Helper;
using FormBot.Main.Infrastructure;
using StackExchange.Redis;

namespace FormBot.Main.Controllers
{

    public class ServiceController : Controller
    {
        public static Object myLock = new Object();
        private readonly ILogger _log;
        private readonly ISolarCompanyBAL _solarCompanyService;

        public ServiceController(ILogger log, ISolarCompanyBAL solarCompanyService)
        {
            this._log = log;
            this._solarCompanyService = solarCompanyService;
        }

        [HttpGet]
        public async Task<bool> CheckBusinessRules_UpdateCache(int jobid)
        {
            try
            {
                //var controller = DependencyResolver.Current.GetService<JobController>();
                //controller.GetBusinessRuleStatus(jobid,true);

                //controller.GetBusinessRuleStatus = new GetBusinessRuleStatus();

                await CommonBAL.SetCacheDataForJobID(0, jobid);
                return true;
            }
            catch (Exception ex)
            {
                //RECRegistryHelper.WriteToLogFile("Error Date:" + DateTime.Now.ToString() + " Exception:" +ex.Message);
                _log.LogException("Error Date:" + DateTime.Now.ToString() + " Exception:", ex);
                return false;
            }
        }
        /// <summary>
        /// update cache of all solarcompany of reseller or specific solar company wise
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        public async Task SetCacheDataForSTCSubmissionFromSolarCompanyId(int? ResellerId, bool onlySTCAndPeakPay = false)
        {
            try
            {
                // for main screen
                //await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyIdNonApproved(ResellerId, SolarCompanyId, 0);
                //for (int year = DateTime.Now.Year; year >= ProjectConfiguration.ArchiveMinYear; year--)
                //{
                //    await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(ResellerId, SolarCompanyId, year);
                //}

                await SetCacheDataForSTCSubmissionFromResellerIds(Convert.ToString(ResellerId),onlySTCAndPeakPay);

                //ICreateJobBAL _job = new CreateJobBAL();

                //lock (myLock)
                //{
                //    DataSet dsAllColumnsData = new DataSet();
                //   // List<int> solarCompanyIds = new List<int>();
                //    List<int> solarCompanyIds = _solarCompanyService.GetSolarCompanyByResellerID(ResellerId).Select(X => X.SolarCompanyId).ToList();
                //    List<int> dssolarCompanyIds = new List<int>();
                //    if (solarCompanyIds != null && solarCompanyIds.Count > 0)
                //    {
                //        Helper.Log.WriteLog("enter in if condition 1:" + solarCompanyIds.Count);
                //            dsAllColumnsData = _job.GetJobSTCSubmissionKendo(string.Join(",", solarCompanyIds), ResellerId);
                //       // Helper.Log.WriteLog("enter in if condition 2:" +dsAllColumnsData.Tables.Count);
                //        if (dsAllColumnsData != null && dsAllColumnsData.Tables.Count > 0)
                //            {
                //                dssolarCompanyIds = dsAllColumnsData.Tables[1].AsEnumerable().Select(dr => dr.Field<int>("SolarCompanyId")).ToList();
                //            }
                //        Helper.Log.WriteLog("enter in if condition 3:" + dssolarCompanyIds.Count +"solarcompanyIDs: "+string.Join(",",dssolarCompanyIds));
                //        foreach (int solarCompId in dssolarCompanyIds)
                //        {
                //            Helper.Log.WriteLog("enter in if condition 4:" + solarCompId);
                //            //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsSTCSubmissionIndex + "_" + solarCompId))
                //            CacheConfiguration.Remove(CacheConfiguration.dsSTCSubmissionIndex + "_" + solarCompId);

                //            DataTable dtSolarCompData = dsAllColumnsData.Tables[0].AsEnumerable().Where(dr => dr.Field<int>("SolarCompanyId") == solarCompId).Select(dr => dr).CopyToDataTable();
                //            CacheConfiguration.Set(CacheConfiguration.dsSTCSubmissionIndex + "_" + solarCompId, dtSolarCompData);
                //            Helper.Log.WriteLog("enter in if condition 5 after set cache:" + solarCompId);

                //        }

                //    }

                //    //List<int> dsSolarCompanyIds = dsAllColumnsData.Tables[0].AsEnumerable().Select(dr => dr.Field<int>("SolarCompanyId")).Distinct().ToList();

                //}
            }
            catch (Exception e)
            {
                string s = e.Message;
                Helper.Log.WriteError(e, "write error in setCacheDataForSTCSUbmissionFromSOlarCOmpanyId" + e.InnerException.ToString());
                _log.Log(SystemEnums.Severity.Error, e.InnerException.Message.ToString());
            }
        }

        /// <summary>
        /// create cache for job listing solar compnay wise  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCacheFull(string key)
        {
            try
            {
                if (ProjectConfiguration.ArchiveMinYear.ToString() == key)
                {
                    ResellerBAL objResellerBAL = new ResellerBAL();
                    List<string> lstResellerIDs = new List<string>();
                    //lstResellerID = objResellerBAL.GetData(null, false, true).Select(a => a.ResellerID).ToList();
                    lstResellerIDs = objResellerBAL.GetBatchData().Select(a => a.ResellerIDs).ToList();

                    Common.Log("CreateCacheFull Started Total ResellerID Count: " + lstResellerIDs.Count);
                    foreach (var RAId in lstResellerIDs)
                    {
                        Common.Log("CreateCacheFull Started ResellerID: " + RAId);
                        await SetCacheDataForSTCSubmissionFromResellerIds(RAId.ToString());
                        Common.Log("CreateCacheFull Ended ResellerID: " + RAId);
                    }
                    Common.Log("CreateCacheFull Completed Total ResellerID Count: " + lstResellerIDs.Count);
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException("error in CreateCacheFull action", ex);
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// update cache of all solarcompany of reseller or specific solar company wise
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        public async Task SetCacheDataForSTCSubmissionFromResellerIds(string ResellerId, bool onlySTCAndPeakPay = false)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                List<int> lstSCAID = _solarCompanyService.GetSolarCompanyByMultipleResellerID(ResellerId).Select(X => X.SolarCompanyId).ToList();
                if (lstSCAID.Any())
                {
                    Common.Log("OnlySTCPeakPay: " + onlySTCAndPeakPay + "Reseller: " + ResellerId);
                    string solarCompanyIds = string.Join(",", lstSCAID.Select(n => n.ToString()).ToArray());
                    await CommonBAL.SetCacheDataForPeakPayFromSolarCompanyId(solarCompanyIds, 0);
                    CommonBAL.STCDistributedCacheHashSet(cache, solarCompanyIds, 0, 0, true);
                    for (int year = DateTime.Now.Year; year >= ProjectConfiguration.ArchiveMinYear; year--)
                    {
                        await CommonBAL.SetCacheDataForSTCSubmissionFromSolarCompanyId(0, solarCompanyIds, year);
                        if (!onlySTCAndPeakPay) 
                        {
                            CommonBAL.JobsDistributedCacheHashSet(cache, lstSCAID, year);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
                Helper.Log.WriteError(e, "write error in setCacheDataForSTCSUbmissionFromSOlarCOmpanyId" + e.InnerException.ToString());
                _log.Log(SystemEnums.Severity.Error, e.InnerException.Message.ToString());
            }
        }

        [HttpGet]
        public async Task<bool> UpdateCacheForSTCSubmission(int StcJobDetailsId)
        {
            try
            {
                await CommonBAL.SetCacheDataForSTCSubmission_VendorAPI(StcJobDetailsId);
                //RECRegistryHelper.WriteToLogFile("Update Cache successfully" + DateTime.Now.ToString());
                _log.LogException("Update Cache successfully" + DateTime.Now.ToString(), null);
                return true;
            }
            catch (Exception ex)
            {
                //RECRegistryHelper.WriteToLogFile("Error Date:" + DateTime.Now.ToString() + " Exception:" + ex.Message);
                _log.LogException("Error Date:" + DateTime.Now.ToString() + " Exception:", ex);
                return false;
            }
        }
        [HttpGet]
        public async Task<bool> UpdateCacheForSTCIds(string StcJobDetailsIds)
        {
            try
            {
                Log.WriteLog("ENter in updatecacheforSTCIds: " + StcJobDetailsIds);
                if (!string.IsNullOrEmpty(StcJobDetailsIds))
                {
                    List<string> stcjobid = new List<string>();
                    stcjobid = StcJobDetailsIds.Split(',').ToList();
                    foreach (var item in stcjobid)
                    {
                        await CommonBAL.SetCacheDataForSTCSubmission_VendorAPI(Convert.ToInt32(item));
                    }

                    //RECRegistryHelper.WriteToLogFile("Update Cache successfully" + DateTime.Now.ToString());
                    Log.WriteLog(DateTime.Now + "Update Cache successfully");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //RECRegistryHelper.WriteToLogFile("Error Date:" + DateTime.Now.ToString() + " Exception:" + ex.Message);
                _log.LogException("Error Date:" + DateTime.Now.ToString() + " Exception:", ex);
                return false;
            }
        }
        /// <summary>
        /// update cache from window service
        /// </summary>
        /// <param name="StcJobDetailsId"></param>
        /// <param name="JobId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<bool> UpdateCacheForSTCSubmissionForWindowService(int? StcJobDetailsId, int? JobId = null)
        {
            try
            {
                //_log.Log(SystemEnums.Severity.Debug, "Enter in UpdateCacheForSTCSubmissionForWindowService: " + StcJobDetailsId + " JobId: " + JobId );
                Log.WriteLog("enter in UpdateCacheForSTCSubmissionForWindowService :" + StcJobDetailsId + " Jobdid:" + JobId);
                SortedList<string, string> data = new SortedList<string, string>();
                NameValueCollection headers = Request.Headers;
                List<string> lstCacheKey = new List<string>() { "STCJobDetailsId", "SolarCompanyId", "ColorCode", "IsGst", "ComplianceBy", "STCStatusId", "IsPayment", "HasMultipleRecords", "IsPartialValidForSTCInvoice", "RamId", "JobNumber", "CustomSettlementTerm", "STCJobComplianceID", "ScoUserId", "CreatedBy", "InvoiceStatus", "IsDeleted", "STCInvoiceCount", "RefNumberOwnerName", "InstallationAddress", "STCStatus", "PVDSWHCode", "STCPrice", "STCSettlementTerm", "STCSubmissionDate", "STCSettlementDate", "ComplianceOfficer", "IsInvoiced", "AccountManager", "SolarCompany", "STC", "JobTypeId", "SystemSize", "JobID", "InstallerName", "OwnerName", "IsCreditNote", "ComplianceNotes", "OwnerCompany", "InstallationDate", "InstallationState", "InstallationTown", "GBBatchRECUploadId", "RECBulkUploadTimeDate", "IsSPVRequired", "IsRelease", "IsSPVInstallationVerified", "PanelBrand", "PanelModel", "InverterBrand", "InverterSeries", "InverterModel" };
                Log.WriteLog(DateTime.Now + "enter in UpdateCacheForSTCSubmissionForWindowService 2:" + headers.Count);
                bool isResetParentCache = false;
                for (int i = 0; i < headers.Count; i++)
                {
                    string key = headers.GetKey(i);
                    string value = headers.Get(i);
                    Log.WriteLog("satrt for loop key: " + key + ",Value: " + value);
                    if (key != "Connection" && key != "Accept-Encoding" && key != "Host")
                    {
                        if (lstCacheKey.Contains(key))
                        {
                            if (key.ToLower().Contains("date") || key.ToLower().Contains("time"))
                            {
                                Log.WriteLog("key: " + key + ",Value: " + value);
                                string strdateTime = value.Replace("AM", "").Replace("PM", "");
                                DateTime dt = DateTime.ParseExact(strdateTime, "M/d/yyyy h:m:s ", CultureInfo.InvariantCulture);
                                value = (dt.ToString());
                            }

                            Log.WriteLog("Key: " + key + " Value: " + (value));
                            data.Add(key, value);
                            if (key == "STCStatusId" || key == "IsInvoiced")
                                isResetParentCache = true;
                        }
                    }
                }

                //TODO: Lisa
                await CommonBAL.SetCacheDataForSTCSubmission(StcJobDetailsId, JobId, data, isResetParentCache, isResetParentCache);
                _log.Log(SystemEnums.Severity.Info, DateTime.Now.ToString() + " Update Cache successfully for stcid: " + StcJobDetailsId);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogException("Error in UpdateCacheForSTCSubmissionForWindowService Date:" + DateTime.Now.ToString() + " Exception:", ex);
                return false;
            }
        }

        public async Task<bool> UpdateCacheForPeakPayForWindowService(string stcJobdetailIds)
        {
            try
            {
                await CommonBAL.SetCacheDataForPeakPayFromJobId("", stcJobdetailIds);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogException("Error in UpdateCacheForPeakPayForWindowService :" + stcJobdetailIds, ex);
                return false;
            }
        }
    }
}
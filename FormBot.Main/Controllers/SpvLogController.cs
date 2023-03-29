using FormBot.BAL.Service.SPV;
using FormBot.Entity.KendoGrid;
using FormBot.Entity.SPV;
using FormBot.Helper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    //[UserAuthorization]
    public class SpvLogController : BaseController
    {
        #region Fields
        private readonly Logger _log;
        private readonly ISpvLogBAL _spvLog;
        #endregion
        #region Constructor
        public SpvLogController(ISpvLogBAL spvLog)
        {
            _log = new Logger();
            _spvLog = spvLog;
        }
        #endregion
        #region Method

        [UserAuthorization]
        // GET: SpvLog
        public ActionResult Index()
        {
            return View();
        }
        // POST: GetEndPointsList
        [HttpPost]
        public JsonResult GetEndPointList()
        {
            List<string> lstEndPoints = new List<string>();
            try
            {
                lstEndPoints = _spvLog.GetAllEndPointsNameFromSpvManufacturer().ToList();
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogController/GetEndPointList", ex);
            }
            return Json(lstEndPoints, JsonRequestBehavior.AllowGet);
        }
        // POST: GetSpvLogs
        /// <summary>
        /// Get Spv logs with filters.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNumber"></param>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <param name="ServiceAdministrator"></param>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="JobReferenceOrId"></param>
        /// <param name="PVDSWHcode"></param>
        /// <param name="SPVMethod"></param>
        /// <param name="VerificationStatus"></param>
        /// <param name="ResponseCode"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="ModelNumber"></param>
        /// <param name="SerialNumer"></param>
        /// <param name="FromRequestDate"></param>
        /// <param name="ToRequestDate"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSpvLogs(int skip, string take, string pageSize, int page,
            string ServiceAdministrator, int ResellerId, int? SolarCompanyId = null, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null, List<KendoGridSorting> sort = null)
        {
            try
            {
                string SortCol = "", SortDir = "";
                if(sort != null)
                    if(sort.Any())
                    {
                        SortCol = sort.FirstOrDefault().Field;
                        SortDir = sort.FirstOrDefault().Dir;
                    }
                //string cacheKey = string.Format(CacheConfiguration.SpvLogList, Convert.ToInt32(pageSize), page, SortCol, SortDir, ServiceAdministrator, ResellerId, SolarCompanyId, JobReferenceOrId, PVDSWHcode, SPVMethod, VerificationStatus, ResponseCode, Manufacturer, ModelNumber, SerialNumer, FromRequestDate, ToRequestDate);
                List<SpvLog> lstSpvLogs = 
                    //CacheConfiguration.Get<List<SpvLog>>(cacheKey, () =>
                    //{
                    //    return 
                        _spvLog.GetSpvLogs(Convert.ToInt32(pageSize), page, SortCol, SortDir, ServiceAdministrator, ResellerId, SolarCompanyId, JobReferenceOrId, PVDSWHcode, SPVMethod, VerificationStatus, ResponseCode, Manufacturer, ModelNumber, SerialNumer, FromRequestDate, ToRequestDate).ToList();
                    //});

                return Json(new { total = (lstSpvLogs.Any() ? lstSpvLogs.FirstOrDefault().TotalRecordCount : 0), data = lstSpvLogs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogController/GetSpvLogs", ex);
                return Json(new { total = 0, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get all spv log data with filters and export in csv .
        /// </summary>
        /// <param name="ServiceAdministrator"></param>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="JobReferenceOrId"></param>
        /// <param name="PVDSWHcode"></param>
        /// <param name="SPVMethod"></param>
        /// <param name="VerificationStatus"></param>
        /// <param name="ResponseCode"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="ModelNumber"></param>
        /// <param name="SerialNumer"></param>
        /// <param name="FromRequestDate"></param>
        /// <param name="ToRequestDate"></param>
        /// <returns></returns>
        public void ExportCSVForSPVLog(string ServiceAdministrator, int? ResellerId, int? SolarCompanyId = null, string JobReferenceOrId = null, string PVDSWHcode = null, int? SPVMethod = null, int? VerificationStatus = null, string ResponseCode = null, string Manufacturer = null, string ModelNumber = null, string SerialNumer = null, DateTime? FromRequestDate = null, DateTime? ToRequestDate = null, bool IsWithoutSCA = false, string ResellerName = null, string SolarCompanyName = null)
        {
            try
            {

                string ServiceAdministratorName = "Service Administrator:" + ServiceAdministrator;
                string SolarCompanyname = SolarCompanyId == -1 ? "All" : SolarCompanyName;
                string SpvMethod = SPVMethod == -1 ? "All" : SPVMethod == 1 ? "Product Verification" : "Installation Verification";
                string VerificationState = VerificationStatus == -1 ? "All" : VerificationStatus == 1 ? "Success" : "Fail";

                //get data for generate csv from spvlog.
                DataTable dataTable = _spvLog.GetDataForExportCSVForSpvLogs(ServiceAdministrator, ResellerId, SolarCompanyId, JobReferenceOrId, PVDSWHcode, SPVMethod, VerificationStatus, ResponseCode, Manufacturer, ModelNumber, SerialNumer, FromRequestDate, ToRequestDate);
                //generate csv of spvlog datatable.
                StringBuilder csv = new StringBuilder();
                csv.Append(Helper.Helper.Common.StringToCSVCell(ServiceAdministratorName) + ",");
                csv.Remove(csv.ToString().Length - 1, 1);
                csv.Append("\r\n");
                csv.Append(@"Reseller:" + ResellerName);
                csv.Append("\r\n");
                if (!IsWithoutSCA)
                {
                    csv.Append(@"Solar Company:" + SolarCompanyname);
                    csv.Append("\r\n");
                }
                csv.Append(@"Reference Number/Job ID Number:" + JobReferenceOrId);
                csv.Append("\r\n");
                csv.Append(@"PVD/SWH code:" + PVDSWHcode);
                csv.Append("\r\n");
                csv.Append(@"SPV Method:" + SpvMethod);
                csv.Append("\r\n");
                csv.Append(@"Verification Status:" + VerificationState);
                csv.Append("\r\n");
                csv.Append(@"Response Code:" + ResponseCode);
                csv.Append("\r\n");
                csv.Append(@"Manufacturer:" + Manufacturer);
                csv.Append("\r\n");
                csv.Append(@"Model Number:" + ModelNumber);
                csv.Append("\r\n");
                csv.Append(@"Serial Number:" + SerialNumer);
                csv.Append("\r\n");
                csv.Append(@"FromRequestDate:" + FromRequestDate);
                csv.Append("\r\n");
                csv.Append(@"ToRequestDate:" + ToRequestDate);
                csv.Append("\r\n");


                //id iswithout sca button click then remove solar company and reseller column from csv.
                if (!IsWithoutSCA)
                {
                    dataTable.Columns.Remove("SolarCompanyCode");
                    csv.Append(@"SolarCompanyName,ResellerName,JobId,SerialNumber,SpvMethod,VerificationStatus,ResponseCode,ResponseTime,ServiceAdministrator,RequestTime,Manufacturer,ModelNumber,Supllier,ResponseMessage");
                }
                else
                {
                    dataTable.Columns.Remove("CompanyName");
                    dataTable.Columns.Remove("ResellerName");
                    
                    csv.Append(@"SolarCompanyCode,JobId,SerialNumber,SpvMethod,VerificationStatus,ResponseCode,ResponseTime,ServiceAdministrator,RequestTime,Manufacturer,ModelNumber,Supllier,ResponseMessage");
                }
                csv.Append("\r\n");
                //var cnt = dataTable.Rows.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (string item in row.ItemArray)
                    {
                        csv.Append(Helper.Helper.Common.StringToCSVCell(item) + ",");
                    }

                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=SpvLogs" + DateTime.Now.ToString() + ".csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(csv.ToString());
                Response.Flush();
                Response.End();
            }

            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogController/ExportCSVForSPVLog", ex);

            }
        }

        /// <summary>
        /// Get spv log details by spv log id. And display in nested kendo grid.
        /// </summary>
        /// <param name="SpvLogId"></param>
        /// <returns></returns>
        public JsonResult GetSpvLogDetailsBySpvLogId(int SpvLogId)
        {
            try
            {
                //string cacheKey = string.Format(CacheConfiguration.SpvLogDetails, SpvLogId);
                SPVLogDetail objSpvLogDetail = 
                //CacheConfiguration.Get<SPVLogDetail>(cacheKey, () =>
                //{
                //    return 
                    _spvLog.GetSPVLogDetailBySpvLogId(SpvLogId);
                //});
                return Json(objSpvLogDetail, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, $"SpvLogController/GetEndPointList", ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
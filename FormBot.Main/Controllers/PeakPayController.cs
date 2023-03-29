using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Entity.KendoGrid;
using FormBot.Helper;
using FormBot.Helper.Helper;
using GenericParsing;
using Microsoft.VisualBasic.FileIO;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace FormBot.Main.Controllers
{
    public class PeakPayController : Controller
    {
        #region Properties
        private readonly IPeakPayBAL _peakPay;
        private readonly ICreateJobHistoryBAL _createJobHistoryBAL;
        private readonly ICreateJobBAL _createJobBAL;
        private readonly ISolarCompanyBAL _solarCompanyBAL;
        #endregion

        #region Constructor

        public PeakPayController(IPeakPayBAL peakPayBAL, ICreateJobHistoryBAL createJobHistoryBAL, ICreateJobBAL createJobBAL, ISolarCompanyBAL solarCompanyBAL)
        {
            this._peakPay = peakPayBAL;
            this._createJobHistoryBAL = createJobHistoryBAL;
            this._createJobBAL = createJobBAL;
            this._solarCompanyBAL = solarCompanyBAL;
        }
        #endregion

        // GET: PeakPay
        [UserAuthorization]
        public ActionResult Index()
        {
            PeakPay peakPay = new PeakPay();
            peakPay.UserID = ProjectSession.LoggedInUserId;
            peakPay.UserTypeID = ProjectSession.UserTypeId;
            peakPay.lstPeakPayJobStages = _peakPay.GetPeakPayJobStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 0, 0, ProjectSession.SolarCompanyId, "");

            return View("Index", peakPay);
        }

        [UserAuthorization]
        public ActionResult StaticPeakPay()
        {
            PeakPay peakPay = new PeakPay();
            peakPay.UserID = ProjectSession.LoggedInUserId;
            peakPay.UserTypeID = ProjectSession.UserTypeId;
            peakPay.lstPeakPayJobStages = _peakPay.GetPeakPayJobStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 0, 0, ProjectSession.SolarCompanyId, "");

            return View("StaticPeakPay", peakPay);
        }

        /// <summary>
        /// Gets the peak pay list.
        /// </summary>
        /// <param name="reseller">The reseller.</param>
        /// <param name="solarcompanyId">The solarcompany identifier.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="stcFromPrice">The STC from price.</param>
        /// <param name="stcToPrice">The STC to price.</param>
        /// <param name="cerApprovedFromDate">The cer approved from date.</param>
        /// <param name="cerApprovedToDate">The cer approved to date.</param>
        /// <param name="settleBeforeFromDate">The settle before from date.</param>
        /// <param name="settleBeforeToDate">The settle before to date.</param>
        /// <param name="paymentFromDate">The payment from date.</param>
        /// <param name="paymentToDate">The payment to date.</param>
        /// <param name="stcStatusId">The STC status identifier.</param>
        /// <param name="isSentInvoice">if set to <c>true</c> [is sent invoice].</param>
        /// <param name="isUnsentInvoice">if set to <c>true</c> [is unsent invoice].</param>
        /// <param name="isReadytoSTCInvoice">if set to <c>true</c> [is readyto STC invoice].</param>
        public void GetPeakPayListNew(string stageid = "", string reseller = "", string solarcompanyId = "", string searchText = "", string stcFromPrice = "", string stcToPrice = "", string cerApprovedFromDate = "", string cerApprovedToDate = "", string settleBeforeFromDate = "", string settleBeforeToDate = "", string paymentFromDate = "", string paymentToDate = "", string stcStatusId = "", bool isSentInvoice = false, bool isUnsentInvoice = true, bool isReadytoSentInvoice = true, string systSize = "", string isAllScaJobView = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int StageId = !string.IsNullOrEmpty(stageid) ? Convert.ToInt32(stageid) : 0;
            int ResellerId = 0;
            int RamId = 0;
            int SolarcompanyId = 0;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
            {
                ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
                SolarcompanyId = !string.IsNullOrEmpty(solarcompanyId) ? Convert.ToInt32(solarcompanyId) : 0;
            }
            else if (ProjectSession.UserTypeId == 2)
            {
                ResellerId = ProjectSession.ResellerId;
                SolarcompanyId = !string.IsNullOrEmpty(solarcompanyId) ? Convert.ToInt32(solarcompanyId) : 0;
            }
            else if (ProjectSession.UserTypeId == 5)
            {
                ResellerId = ProjectSession.ResellerId;
                //RamId = !string.IsNullOrEmpty(ram) ? Convert.ToInt32(ram) : 0;
                SolarcompanyId = !string.IsNullOrEmpty(solarcompanyId) ? Convert.ToInt32(solarcompanyId) : 0;
            }
            else
            {
                SolarcompanyId = ProjectSession.SolarCompanyId;
            }

            decimal StcFromPrice = 0, StcToPrice = 0;
            if (!string.IsNullOrEmpty(stcFromPrice) && !string.IsNullOrEmpty(stcToPrice))
            {
                StcFromPrice = Convert.ToDecimal(stcFromPrice);
                StcToPrice = Convert.ToDecimal(stcToPrice);
            }

            DateTime? CERApprovedFromDate = null, CERApprovedToDate = null, SettleBeforeFromDate = null, SettleBeforeToDate = null, PaymentFromDate = null, PaymentToDate = null;
            if (!string.IsNullOrEmpty(cerApprovedFromDate) && !string.IsNullOrEmpty(cerApprovedToDate))
            {
                CERApprovedFromDate = Convert.ToDateTime(cerApprovedFromDate);
                CERApprovedToDate = Convert.ToDateTime(cerApprovedToDate);
            }
            if (!string.IsNullOrEmpty(settleBeforeFromDate) && !string.IsNullOrEmpty(settleBeforeToDate))
            {
                SettleBeforeFromDate = Convert.ToDateTime(settleBeforeFromDate);
                SettleBeforeToDate = Convert.ToDateTime(settleBeforeToDate);
            }
            if (!string.IsNullOrEmpty(paymentFromDate) && !string.IsNullOrEmpty(paymentToDate))
            {
                PaymentFromDate = Convert.ToDateTime(paymentFromDate);
                PaymentToDate = Convert.ToDateTime(paymentToDate);
            }
            int StcStatusId = !string.IsNullOrEmpty(stcStatusId) ? Convert.ToInt32(stcStatusId) : 0;
            int SystemSize = !string.IsNullOrEmpty(systSize) ? Convert.ToInt32(systSize) : 0;
            decimal TotalAmount = 0;

            IList<PeakPay> lstPeakPay = _peakPay.GetPeakPayList(StageId, ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, ResellerId, SolarcompanyId, searchText, StcFromPrice, StcToPrice, CERApprovedFromDate, CERApprovedToDate, SettleBeforeFromDate, SettleBeforeToDate, PaymentFromDate, PaymentToDate, StcStatusId, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, SystemSize, isAllScaJobView);
            if (lstPeakPay.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstPeakPay.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstPeakPay.FirstOrDefault().TotalRecords;
                gridParam.TotalAmount = TotalAmount;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstPeakPay, gridParam));
        }

        /// <summary>
        /// Generates the CSV for selected jobs.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GenerateCSVForSelectedJobs(string jobs)
        {
            try
            {
                if (!string.IsNullOrEmpty(jobs))
                {
                    CreateCSV(jobs);
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message });
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Creates the CSV.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        public void CreateCSV(String JobId)
        {

            DataSet ds = _peakPay.GetPeakPayCSV(JobId);
            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
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
            Response.ClearHeaders();
            Response.ClearContent();
            Response.BufferOutput = true;
            Response.ContentType = "application/CSV";
            Response.AddHeader("content-disposition", "attachment;filename=PeakPay.csv");
            Response.Charset = "";
            Response.Output.Write(csv);
            //Response.Flush();
            Response.End();

        }

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public async Task<JsonResult> ImportCSV()
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
                    parser.FirstRowSetsExpectedColumnCount = true;
                    parser.SetDataSource(textReader);

                    if (parser.Read())
                    {
                        do
                        {
                            try
                            {
                                var row = dtCSV.NewRow();

                                if (parser["STCJobDetailsID"].Trim() != "" && parser["STCJobDetailsID"].Trim() != null)
                                    row["STCJobDetailsID"] = Convert.ToInt32(parser["STCJobDetailsID"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of STCJobDetailsID." }, JsonRequestBehavior.AllowGet);

                                if (parser["JobID"].Trim() != "" && parser["JobID"].Trim() != null)
                                    row["JobID"] = Convert.ToInt32(parser["JobID"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of JobID." }, JsonRequestBehavior.AllowGet);

                                if (parser["STCPrice"].Trim() != "" && parser["STCPrice"].Trim() != null)
                                {
                                    if (Convert.ToDecimal(parser["STCPrice"].Trim()) > 0)
                                        row["STCPrice"] = Convert.ToDecimal(parser["STCPrice"].Trim());
                                    else
                                        return Json(new { status = false, error = "STCPrice should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of STCPrice." }, JsonRequestBehavior.AllowGet);

                                if (parser["IsGst"].Trim() != "" && parser["IsGst"].Trim() != null)
                                    row["IsGst"] = Convert.ToString(parser["IsGst"].Trim().ToLower()) == "true" ? 1 : 0;
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of IsGst." }, JsonRequestBehavior.AllowGet);

                                if (parser["STCFee"].Trim() != "" && parser["STCFee"].Trim() != null)
                                {
                                    if (Convert.ToDecimal(parser["STCFee"].Trim()) > 0)
                                        row["STCFee"] = Convert.ToDecimal(parser["STCFee"].Trim());
                                    else
                                        return Json(new { status = false, error = "STCFee should be greater than 0." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of STCFee." }, JsonRequestBehavior.AllowGet);

                                if (parser["InvoiceStatus"].Trim() != "" && parser["InvoiceStatus"].Trim() != null)
                                    row["InvoiceStatus"] = Convert.ToInt32(parser["InvoiceStatus"].Trim());
                                else
                                    return Json(new { status = false, error = "Imported csv file has not data of InvoiceStatus." }, JsonRequestBehavior.AllowGet);

                                dtCSV.Rows.Add(row);

                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                            }

                        } while (parser.Read());
                    }
                    else
                        return Json(new { status = false, error = "Imported csv file has not data." }, JsonRequestBehavior.AllowGet);
                    string stcjobIds = string.Empty;
                    if (dtCSV.Rows.Count > 0)
                    {
                        DataTable dt = _peakPay.ImportPeakPayCsv(dtCSV);
                        foreach (DataRow dr in dtCSV.Rows)
                        {
                            stcjobIds = stcjobIds + (dr["STCJobDetailsID"]).ToString() + ",";
                            JobHistory jobHistory = new JobHistory();
                            jobHistory.Name = ProjectSession.LoggedInName;
                            jobHistory.FunctionalityName = "Import PeakPay CSV";
                            jobHistory.JobID = Convert.ToInt32(dr["JobID"]);
                            //bool isHistorySaved = _createJobHistoryBAL.LogJobHistory(jobHistory, HistoryCategory.ModifiedIsGst);
                            string JobHistoryMessage = "modified Gst from " + jobHistory.FunctionalityName;
                           Common.SaveJobHistorytoXML(jobHistory.JobID, JobHistoryMessage, "General", "ModifiedIsGst", ProjectSession.LoggedInName, false);
                        }
                        //stcjobIds = stcjobIds.Remove(stcjobIds.Length - 1);
                        //DataTable dt = _createJobBAL.GetSTCDetailsAndJobDataForCache(stcjobIds, null);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string isGst = dt.Rows[i]["IsGst"].ToString();
                                string stcPrice = dt.Rows[i]["STCPrice"].ToString();
                                string InvoiceStatus = dt.Rows[i]["InvoiceStatus"].ToString();
                                SortedList<string, string> data = new SortedList<string, string>();
                                data.Add("IsGst", isGst);
                                data.Add("STCPrice", stcPrice);
                                data.Add("InvoiceStatus", InvoiceStatus);
                                await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), null, data);
                                data.Remove("InvoiceStatus");
                                data.Add("IsInvoiced", InvoiceStatus);
                                data.Add("STCFee", Convert.ToString(dt.Rows[i]["StcFee"]));
                                data.Add("Total", Convert.ToString(dt.Rows[i]["Total"]));
                                await CommonBAL.SetCacheDataForPeakPay(Convert.ToInt32(dt.Rows[i]["STCJobDetailsID"]), null, data);
                            }
                        }

                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, error = "Imported csv file has not data." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the CSV table.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCSVTable()
        {
            DataTable dtCSVData = new DataTable();
            dtCSVData.Columns.Add("STCJobDetailsID", typeof(Int32));
            dtCSVData.Columns.Add("JobID", typeof(Int32));
            dtCSVData.Columns.Add("STCPrice", typeof(decimal));
            dtCSVData.Columns.Add("IsGst", typeof(bool));
            dtCSVData.Columns.Add("STCFee", typeof(decimal));
            dtCSVData.Columns.Add("InvoiceStatus", typeof(Int32));

            return dtCSVData;
        }

        [HttpPost]
        public async Task<ActionResult> PeakPaySetStcPrice(string stcJobDetailIds, decimal stcPrice)
        {
            DataSet dsPeakPay = _peakPay.SetStcPricePeakPay(stcJobDetailIds, stcPrice);
            if (dsPeakPay != null && dsPeakPay.Tables[0] != null && dsPeakPay.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsPeakPay.Tables[0].Rows.Count; i++)
                {
                    SortedList<string, string> data = new SortedList<string, string>();
                    data.Add("STCPrice", stcPrice.ToString());
                    await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dsPeakPay.Tables[0].Rows[i]["STCJobDetailsID"]), null, data);
                    data.Add("Total", Convert.ToString(dsPeakPay.Tables[0].Rows[i]["Total"]));
                    await CommonBAL.SetCacheDataForPeakPay(Convert.ToInt32(dsPeakPay.Tables[0].Rows[i]["STCJobDetailsID"]), null, data);
                }
            }
            return this.Json(new { success = true });
        }

        [HttpPost]
        public async Task<ActionResult> ChangePeakpayInvoiceStatus(string stcJobDetailIds, int invoiceStatus)
        {
            int Invoice_Status = _peakPay.ChangePeakpayInvoiceStatus(stcJobDetailIds, invoiceStatus);

            List<string> lstStcJobDetailsId = stcJobDetailIds.Split(',').ToList();
            foreach (var id in lstStcJobDetailsId)
            {
                SortedList<string, string> data = new SortedList<string, string>();
                data.Add("InvoiceStatus", invoiceStatus.ToString());
                await CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(id), null, data);
                data.Remove("InvoiceStatus");
                data.Add("IsInvoiced", invoiceStatus.ToString());
                await CommonBAL.SetCacheDataForPeakPay(Convert.ToInt32(id), null, data);
            }
            //CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(id), 0);
            return this.Json(new { success = true, InvoiceStatus = Invoice_Status });
        }

        [HttpPost]
        public ActionResult GetPeakPayJobStageCount(string reseller, string ram, string sId, string isAllScaJobView)
        {
            int ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            int RamId = !string.IsNullOrEmpty(ram) ? Convert.ToInt32(ram) : 0;
            int SolarCompanyId = !string.IsNullOrEmpty(sId) ? Convert.ToInt32(sId) : 0;
            
            if (ProjectSession.UserTypeId == 5 && RamId == 0)
                RamId = ProjectSession.LoggedInUserId;
            var lstPeakPayJobStagesCount = _peakPay.GetPeakPayJobStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ResellerId, RamId, SolarCompanyId, isAllScaJobView);
            return this.Json(new { lstPeakPayJobStagesCount, success = true });
        }

        //private DataTable FilteringAndSortingStaticPeakPayDataTable(DataTable dt, int stageId = 0, string sortCol = "", string sortDir = "", string searchText = "", string stcFromPrice = null, string stcToPrice = null, string CERApprovedFromDate = null, string CERApprovedToDate = null, string settleBeforeFromDate = null, string settleToFromDate = null,
        //    string paymentFromDate = null, string paymentToDate = null, bool isSentInvoice = false, bool isUnsentInvoice = true, bool isReadyToSentInvoice = true, string systemSize = "")
        //{
        //    #region filter section
        //    string searchQuery = "";
        //    string shortQuery = string.Empty;

        //    if (stageId != 0)
        //    {
        //        shortQuery = "(StcStatusId = " + stageId + ")";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(searchText) && int.TryParse(searchText, out int jobId))
        //    {
        //        shortQuery = "((Reference Like '%" + searchText.Trim() + "%') OR (JobID = " + searchText.Trim() + ") OR (OwnerName Like '%" + searchText.Trim() + "%' ) OR (InstallationAddress Like '%" + searchText.Trim() + "%' ))";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    else if (!string.IsNullOrEmpty(searchText))
        //    {
        //        shortQuery = "((Reference Like '%" + searchText.Trim() + "%') OR (OwnerName Like '%" + searchText.Trim() + "%' ) OR (InstallationAddress Like '%" + searchText.Trim() + "%' ))";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(stcFromPrice))
        //    {
        //        shortQuery = "(STCPrice >= " + stcFromPrice + ")";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(stcToPrice))
        //    {
        //        shortQuery = "(STCPrice <= " + stcToPrice + ")";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(CERApprovedFromDate))
        //    {
        //        shortQuery = "(CERApprovedDate >= '" + Convert.ToDateTime(CERApprovedFromDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(CERApprovedToDate))
        //    {
        //        shortQuery = "(CERApprovedDate <= '" + Convert.ToDateTime(CERApprovedToDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(settleBeforeFromDate))
        //    {
        //        shortQuery = "(SettleBefore >= '" + Convert.ToDateTime(settleBeforeFromDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(settleToFromDate))
        //    {
        //        shortQuery = "(SettleBefore <= '" + Convert.ToDateTime(settleToFromDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(paymentFromDate))
        //    {
        //        shortQuery = "(PaymentDate >= '" + Convert.ToDateTime(paymentFromDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (!string.IsNullOrEmpty(paymentToDate))
        //    {
        //        shortQuery = "(PaymentDate <= '" + Convert.ToDateTime(paymentToDate) + "')";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (isSentInvoice == true || isUnsentInvoice == true || isReadyToSentInvoice == true)
        //    {
        //        string invoiceQuery = string.Empty;
        //        if (isSentInvoice == true)
        //            invoiceQuery = "(STCInvoiceCnt > 0 OR isnull(IsInvoiced,0) = 1)";
        //        if (isUnsentInvoice == true)
        //            invoiceQuery = string.IsNullOrEmpty(invoiceQuery) ? "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 2)" : invoiceQuery + " OR " + "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 2)";
        //        if (isReadyToSentInvoice == true)
        //            invoiceQuery = string.IsNullOrEmpty(invoiceQuery) ? "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 3)" : invoiceQuery + " OR " + "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 3)";

        //        searchQuery = string.IsNullOrEmpty(searchQuery) ? invoiceQuery : searchQuery + " AND " + "(" + invoiceQuery + ")";
        //    }
        //    //if (isUnsentInvoice == true)
        //    //{
        //    //    shortQuery = "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 2)";
        //    //    if (string.IsNullOrEmpty(searchQuery))
        //    //        searchQuery += shortQuery;
        //    //    else
        //    //        searchQuery += " AND " + shortQuery;
        //    //}
        //    //if (isReadyToSentInvoice == true)
        //    //{
        //    //    shortQuery = "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 3)";
        //    //    if (string.IsNullOrEmpty(searchQuery))
        //    //        searchQuery += shortQuery;
        //    //    else
        //    //        searchQuery += " AND " + shortQuery;
        //    //}
        //    if (systemSize == "1")
        //    {
        //        shortQuery = "(SystemSize < 60)";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    if (systemSize == "2")
        //    {
        //        shortQuery = "(SystemSize > 60)";
        //        if (string.IsNullOrEmpty(searchQuery))
        //            searchQuery += shortQuery;
        //        else
        //            searchQuery += " AND " + shortQuery;
        //    }
        //    #endregion filter section

        //    //string query = "";
        //    string sortQuery = "SubmissionDate desc";

        //    if (sortCol != "" && sortCol.ToLower() == "id")
        //    {
        //        sortQuery = "SubmissionDate desc";
        //    }
        //    else if (sortCol != "" && sortDir != "")
        //    {
        //        sortQuery = sortCol + " " + sortDir;
        //    }
        //    //if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(query))
        //    //{
        //    //    query = query + " AND " + searchQuery;
        //    //}
        //    //else if (string.IsNullOrEmpty(query))
        //    //{
        //    //    query = searchQuery;
        //    //}
        //    if (sortQuery.Length > 0 || searchQuery.Length > 0)
        //    {
        //        DataRow[] dr = dt.Select(searchQuery, sortQuery);
        //        dt = dr.Length > 0 ? dr.CopyToDataTable() : new DataTable();
        //    }
        //    return dt;
        //}

        private static void PrepareFilterForPeakPay(ref KendoFilter filter, ref List<KendoSort> sort, int stageId = 0, string sortCol = "", string sortDir = "", string searchText = "", string stcFromPrice = null, string stcToPrice = null, string CERApprovedFromDate = null, string CERApprovedToDate = null, string settleBeforeFromDate = null, string settleToFromDate = null,
            string paymentFromDate = null, string paymentToDate = null, bool isSentInvoice = false, bool isUnsentInvoice = true, bool isReadyToSentInvoice = true, string systemSize = "")
        {
            #region filter section
            filter = new KendoFilter() { Field = null, Filters = null, Logic = "and", Operator = null, Value = null };
            if (stageId != 0)
            {
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "STCStatusId", Operator = "eq", Value = stageId }).ToList();
                filter.AddFilterItem("STCStatusId", KendoFilterOperator.eq.ToString(), stageId);
                //shortQuery = "(StcStatusId = " + stageId + ")";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                KendoFilter filterSearchCols = new KendoFilter() { Logic = "or", Filters = null };
                filterSearchCols.AddFilterItem("Reference", KendoFilterOperator.contains.ToString(), searchText.Trim());
                //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "Reference", Operator = "contains", Value = searchText.Trim() }).ToList();
                if (int.TryParse(searchText, out int _jobId))
                    filterSearchCols.AddFilterItem("JobID", KendoFilterOperator.eq.ToString(), searchText.Trim());
                //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "JobID", Operator = "eq", Value = searchText.Trim() }).ToList();
                filterSearchCols.AddFilterItem("OwnerName", KendoFilterOperator.contains.ToString(), searchText.Trim());
                filterSearchCols.AddFilterItem("InstallationAddress", KendoFilterOperator.contains.ToString(), searchText.Trim());
                //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "OwnerName", Operator = "contains", Value = searchText.Trim() }).ToList();
                //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "InstallationAddress", Operator = "contains", Value = searchText.Trim() }).ToList();
                if (filterSearchCols.Filters != null)
                    filter.Filters = filter.Filters.AddItem(filterSearchCols).ToList();

                //shortQuery = "((Reference Like '%" + searchText.Trim() + "%') OR (JobID = " + searchText.Trim() + ") OR (OwnerName Like '%" + searchText.Trim() + "%' ) OR (InstallationAddress Like '%" + searchText.Trim() + "%' ))";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            //else if (!string.IsNullOrEmpty(searchText))
            //{
            //    KendoFilter filterSearchCols = new KendoFilter() { Logic = "or", Filters = null };
            //    filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "Reference", Operator = "contains", Value = searchText.Trim() }).ToList();
            //    filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "JobID", Operator = "eq", Value = searchText.Trim() }).ToList();
            //    filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "OwnerName", Operator = "contains", Value = searchText.Trim() }).ToList();
            //    filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "InstallationAddress", Operator = "contains", Value = searchText.Trim() }).ToList();
            //    if (filterSearchCols.Filters != null)
            //        filter.Filters = filter.Filters.AddItem(filterSearchCols).ToList();

            //    shortQuery = "((Reference Like '%" + searchText.Trim() + "%') OR (OwnerName Like '%" + searchText.Trim() + "%' ) OR (InstallationAddress Like '%" + searchText.Trim() + "%' ))";
            //    if (string.IsNullOrEmpty(searchQuery))
            //        searchQuery += shortQuery;
            //    else
            //        searchQuery += " AND " + shortQuery;
            //}
            if (!string.IsNullOrEmpty(stcFromPrice))
            {
                filter.AddFilterItem("STCPrice", KendoFilterOperator.gte.ToString(), stcFromPrice);
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "STCPrice", Operator = "gte", Value = stcFromPrice }).ToList();
                //shortQuery = "(STCPrice >= " + stcFromPrice + ")";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(stcToPrice))
            {
                filter.AddFilterItem("STCPrice", KendoFilterOperator.lte.ToString(), stcToPrice);
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "STCPrice", Operator = "lte", Value = stcToPrice }).ToList();
                //shortQuery = "(STCPrice <= " + stcToPrice + ")";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(CERApprovedFromDate))
            {
                filter.AddFilterItem("CERApprovedDate", KendoFilterOperator.gte.ToString(), Convert.ToDateTime(CERApprovedFromDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "CERApprovedDate", Operator = "get", Value = Convert.ToDateTime(CERApprovedFromDate) }).ToList();
                //shortQuery = "(CERApprovedDate >= '" + Convert.ToDateTime(CERApprovedFromDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(CERApprovedToDate))
            {
                filter.AddFilterItem("CERApprovedDate", KendoFilterOperator.lte.ToString(), Convert.ToDateTime(CERApprovedToDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "CERApprovedDate", Operator = "lte", Value = Convert.ToDateTime(CERApprovedToDate) }).ToList();
                //shortQuery = "(CERApprovedDate <= '" + Convert.ToDateTime(CERApprovedToDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(settleBeforeFromDate))
            {
                filter.AddFilterItem("SettleBefore", KendoFilterOperator.gte.ToString(), Convert.ToDateTime(settleBeforeFromDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "SettleBefore", Operator = "gte", Value = Convert.ToDateTime(settleBeforeFromDate) }).ToList();
                //shortQuery = "(SettleBefore >= '" + Convert.ToDateTime(settleBeforeFromDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(settleToFromDate))
            {
                filter.AddFilterItem("SettleBefore", KendoFilterOperator.lte.ToString(), Convert.ToDateTime(settleToFromDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "SettleBefore", Operator = "lte", Value = Convert.ToDateTime(settleToFromDate) }).ToList();
                //shortQuery = "(SettleBefore <= '" + Convert.ToDateTime(settleToFromDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(paymentFromDate))
            {
                filter.AddFilterItem("PaymentDate", KendoFilterOperator.gte.ToString(), Convert.ToDateTime(paymentFromDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "PaymentDate", Operator = "gte", Value = Convert.ToDateTime(paymentFromDate) }).ToList();
                //shortQuery = "(PaymentDate >= '" + Convert.ToDateTime(paymentFromDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (!string.IsNullOrEmpty(paymentToDate))
            {
                filter.AddFilterItem("PaymentDate", KendoFilterOperator.lte.ToString(), Convert.ToDateTime(paymentToDate));
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "PaymentDate", Operator = "lte", Value = Convert.ToDateTime(paymentToDate) }).ToList();
                //shortQuery = "(PaymentDate <= '" + Convert.ToDateTime(paymentToDate) + "')";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (isSentInvoice == true || isUnsentInvoice == true || isReadyToSentInvoice == true)
            {
                string invoiceQuery = string.Empty;
                KendoFilter filterSearchCols = new KendoFilter() { Logic = "or", Filters = null };
                if (isSentInvoice == true)
                {
                    filterSearchCols.AddFilterItem("STCInvoiceCnt", KendoFilterOperator.gt.ToString(), 0);
                    filterSearchCols.AddFilterItem("IsInvoiced", KendoFilterOperator.eq.ToString(), 1);
                    //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "STCInvoiceCnt", Operator = "gt", Value = 0 }).ToList();
                    //filterSearchCols.Filters = filterSearchCols.Filters.AddItem(new KendoFilter() { Field = "IsInvoiced", Operator = "eq", Value = 1 }).ToList();
                }
                if (isUnsentInvoice == true)
                {
                    KendoFilter filterSearchColsSub = new KendoFilter() { Logic = "and", Filters = null };
                    filterSearchColsSub.AddFilterItem("STCInvoiceCnt", KendoFilterOperator.eq.ToString(), 0);
                    filterSearchColsSub.AddFilterItem("IsInvoiced", KendoFilterOperator.eq.ToString(), 2);
                    //filterSearchColsSub.Filters = filterSearchColsSub.Filters.AddItem(new KendoFilter() { Field = "STCInvoiceCnt", Operator = "eq", Value = 0 }).ToList();
                    //filterSearchColsSub.Filters = filterSearchColsSub.Filters.AddItem(new KendoFilter() { Field = "IsInvoiced", Operator = "eq", Value = 2 }).ToList();
                    if (filterSearchColsSub.Filters != null)
                        filterSearchCols.Filters = filterSearchCols.Filters.AddItem(filterSearchColsSub).ToList();
                }
                if (isReadyToSentInvoice == true)
                {
                    KendoFilter filterSearchColsSub = new KendoFilter() { Logic = "and", Filters = null };
                    filterSearchColsSub.AddFilterItem("STCInvoiceCnt", KendoFilterOperator.eq.ToString(), 0);
                    filterSearchColsSub.AddFilterItem("IsInvoiced", KendoFilterOperator.eq.ToString(), 3);
                    //filterSearchColsSub.Filters = filterSearchColsSub.Filters.AddItem(new KendoFilter() { Field = "STCInvoiceCnt", Operator = "eq", Value = 0 }).ToList();
                    //filterSearchColsSub.Filters = filterSearchColsSub.Filters.AddItem(new KendoFilter() { Field = "IsInvoiced", Operator = "eq", Value = 3 }).ToList();
                    if (filterSearchColsSub.Filters != null)
                        filterSearchCols.Filters = filterSearchCols.Filters.AddItem(filterSearchColsSub).ToList();
                }
                if (filterSearchCols.Filters != null)
                    filter.Filters = filter.Filters.AddItem(filterSearchCols).ToList();
                //if (isSentInvoice == true)
                //    invoiceQuery = "(STCInvoiceCnt > 0 OR isnull(IsInvoiced,0) = 1)";
                //if (isUnsentInvoice == true)
                //    invoiceQuery = string.IsNullOrEmpty(invoiceQuery) ? "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 2)" : invoiceQuery + " OR " + "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 2)";
                //if (isReadyToSentInvoice == true)
                //    invoiceQuery = string.IsNullOrEmpty(invoiceQuery) ? "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 3)" : invoiceQuery + " OR " + "(STCInvoiceCnt = 0 AND isnull(IsInvoiced,0) = 3)";

                //searchQuery = string.IsNullOrEmpty(searchQuery) ? invoiceQuery : searchQuery + " AND " + "(" + invoiceQuery + ")";
            }
            if (systemSize == "1")
            {
                filter.AddFilterItem("SystemSize", KendoFilterOperator.lt.ToString(), 60);
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "SystemSize", Operator = "lt", Value = 60}).ToList();
                //shortQuery = "(SystemSize < 60)";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            if (systemSize == "2")
            {
                filter.AddFilterItem("SystemSize", KendoFilterOperator.gt.ToString(), 60);
                //filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = "SystemSize", Operator = "gt", Value = 60 }).ToList();
                //shortQuery = "(SystemSize > 60)";
                //if (string.IsNullOrEmpty(searchQuery))
                //    searchQuery += shortQuery;
                //else
                //    searchQuery += " AND " + shortQuery;
            }
            #endregion filter section

            //string query = "";
            //string sortQuery = "SubmissionDate desc";
            
            if (sortCol != "" && sortDir != "")
            {
                //sortQuery = sortCol + " " + sortDir;
                sort = sort.AddItem(new KendoSort() { Dir = sortDir, Field = sortCol }).ToList();
            }
            else
                sort = sort.AddItem(new KendoSort() { Dir = "Desc", Field = "SubmissionDate" }).ToList();
            //ToDo
            //if (sortQuery.Length > 0 || searchQuery.Length > 0)
            //{
            //    DataRow[] dr = dt.Select(searchQuery, sortQuery);
            //    dt = dr.Length > 0 ? dr.CopyToDataTable() : new DataTable();
            //}
        }

        public async Task GetPeakPayList(string stageid = "", string reseller = "", string solarcompanyId = "", string searchText = "", string stcFromPrice = "", string stcToPrice = "", string cerApprovedFromDate = "", string cerApprovedToDate = "", string settleBeforeFromDate = "", string settleBeforeToDate = "", string paymentFromDate = "", string paymentToDate = "", string stcStatusId = "", bool isSentInvoice = false, bool isUnsentInvoice = true, bool isReadytoSentInvoice = true, string systSize = "", string isAllScaJobView = "", bool defaultGrid = false)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            List<KendoSort> sort = new List<KendoSort>();
            KendoFilter filter = new KendoFilter();
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            int StageId = !string.IsNullOrEmpty(stageid) ? Convert.ToInt32(stageid) : 0;
            int ResellerId = 0;
            int SolarcompanyId = 0;
            decimal totalAmount = 0;
            int totalRecords = 0;
            decimal totalAllAmount = 0;
            List<PeakPayView> lstPeakPayView = new List<PeakPayView>();
            List<PeakPayView> lstPeakPayViewToReturn = new List<PeakPayView>();
            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
                ResellerId = !string.IsNullOrEmpty(reseller) ? Convert.ToInt32(reseller) : 0;
            else if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                ResellerId = ProjectSession.ResellerId;

            if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                SolarcompanyId = !string.IsNullOrEmpty(solarcompanyId) ? Convert.ToInt32(solarcompanyId) : 0;
            else
                SolarcompanyId = ProjectSession.SolarCompanyId;

            decimal StcFromPrice = 0, StcToPrice = 0;
            if (!string.IsNullOrEmpty(stcFromPrice) && !string.IsNullOrEmpty(stcToPrice))
            {
                StcFromPrice = Convert.ToDecimal(stcFromPrice);
                StcToPrice = Convert.ToDecimal(stcToPrice);
            }

            DateTime? CERApprovedFromDate = null, CERApprovedToDate = null, SettleBeforeFromDate = null, SettleBeforeToDate = null, PaymentFromDate = null, PaymentToDate = null;
            if (!string.IsNullOrEmpty(cerApprovedFromDate) && !string.IsNullOrEmpty(cerApprovedToDate))
            {
                CERApprovedFromDate = Convert.ToDateTime(cerApprovedFromDate);
                CERApprovedToDate = Convert.ToDateTime(cerApprovedToDate);
            }
            if (!string.IsNullOrEmpty(settleBeforeFromDate) && !string.IsNullOrEmpty(settleBeforeToDate))
            {
                SettleBeforeFromDate = Convert.ToDateTime(settleBeforeFromDate);
                SettleBeforeToDate = Convert.ToDateTime(settleBeforeToDate);
            }
            if (!string.IsNullOrEmpty(paymentFromDate) && !string.IsNullOrEmpty(paymentToDate))
            {
                PaymentFromDate = Convert.ToDateTime(paymentFromDate);
                PaymentToDate = Convert.ToDateTime(paymentToDate);
            }
            int StcStatusId = !string.IsNullOrEmpty(stcStatusId) ? Convert.ToInt32(stcStatusId) : 0;
            int SystemSize = !string.IsNullOrEmpty(systSize) ? Convert.ToInt32(systSize) : 0;
            IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
            DataSet dsAllColumnsData = new DataSet();
            if (SolarcompanyId == 0)
            {
                #region SolarCompanyID value is "All"

                List<int> lstSolarCompanyId = _solarCompanyBAL.GetSolarCompanyByResellerID(ResellerId).Select(X => X.SolarCompanyId).ToList();

                if (ProjectSession.UserTypeId == 5 && !Convert.ToBoolean(isAllScaJobView))
                    lstSolarCompanyId = _solarCompanyBAL.GetSolarCompanyByRAMID(ProjectSession.LoggedInUserId).Select(X => X.SolarCompanyId).ToList();

                if (lstSolarCompanyId != null && lstSolarCompanyId.Count > 0)
                {
                    if (defaultGrid)
                    {
                        Decimal dStcFromPrice = String.IsNullOrEmpty(stcFromPrice) ? 0 : Convert.ToDecimal(stcFromPrice);
                        Decimal dStcToPrice = String.IsNullOrEmpty(stcToPrice) ? 0 : Convert.ToDecimal(stcToPrice);
                        CommonBAL.PeakPayDistributedWithoutCache(string.Join(",", lstSolarCompanyId), ResellerId, ref lstPeakPayViewToReturn, pageNumber, gridParam.PageSize, StageId, gridParam.SortCol, gridParam.SortDir, searchText, dStcFromPrice, dStcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                        totalAllAmount = lstPeakPayViewToReturn != null && lstPeakPayViewToReturn.Any() ? lstPeakPayViewToReturn.Select(dr => dr.Total).Sum() : 0;
                        totalRecords = lstPeakPayViewToReturn != null && lstPeakPayViewToReturn.Any() ? lstPeakPayViewToReturn.FirstOrDefault().Totalcount : 0;
                    }
                    else
                    {
                        #region Call SP for those SolarCompanyId which are not found in CacheData
                        List<DistributedCacheAllKeysInfoForHashSetView> mainPeakPayCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
                        List<int> lstSolarCompanyIdForCachingData = lstSolarCompanyId.Where(X => mainPeakPayCache == null || !mainPeakPayCache.Any(R => R.PID == X)).Select(X => X).ToList();
                        if (lstSolarCompanyIdForCachingData != null && lstSolarCompanyIdForCachingData.Count > 0)
                        {
                            CommonBAL.PeakPayDistributedCacheHashSet(cache, string.Join(",", lstSolarCompanyIdForCachingData), ResellerId, ref dsAllColumnsData, ref lstPeakPayView);
                        }

                        #endregion
                        #region Fetch SolarCompanyWise JobList Data From Cache
                        lstSolarCompanyId = lstSolarCompanyId.Except(lstSolarCompanyIdForCachingData).ToList();
                        if (dsAllColumnsData.Tables.Count == 0)
                        {
                            dsAllColumnsData.Tables.Add(new DataTable());
                        }
                        CommonBAL.PeakPayDistributedCacheHashGet(cache, lstSolarCompanyId, ref lstPeakPayView);
                        #endregion
                        totalAllAmount = lstPeakPayView != null && lstPeakPayView.Any() ? lstPeakPayView.Select(dr => dr.Total).Sum() : 0;
                        //ToDo
                        PrepareFilterForPeakPay(ref filter, ref sort, StageId, gridParam.SortCol, gridParam.SortDir, searchText, stcFromPrice, stcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                        lstPeakPayViewToReturn = lstPeakPayView.AsQueryable().ToDataSourceResultSortFilter(sort, filter);
                        //DataTable newDatatable = FilteringAndSortingStaticPeakPayDataTable(dsAllColumnsData.Tables[0], StageId, gridParam.SortCol, gridParam.SortDir, searchText, stcFromPrice, stcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                        totalRecords = lstPeakPayViewToReturn.Count;
                        if (totalRecords > 0)
                            lstPeakPayViewToReturn = lstPeakPayViewToReturn.Skip((pageNumber - 1) * gridParam.PageSize).Take(gridParam.PageSize).ToList();
                    }
                    //if (newDatatable.Rows.Count > 0)
                    //{
                    //    DataTable dataTable = newDatatable.Rows.Cast<System.Data.DataRow>().Skip((pageNumber - 1) * gridParam.PageSize).Take(gridParam.PageSize).CopyToDataTable();
                    //    dsAllColumnsData.Tables[0].Clear();
                    //    dsAllColumnsData.Tables[0].Merge(dataTable);
                    //}
                    //else
                    //{
                    //    dsAllColumnsData.Tables[0].Clear();
                    //}
                }

                #endregion
            }
            else
            {
                if (defaultGrid)
                {
                    Decimal dStcFromPrice = String.IsNullOrEmpty(stcFromPrice) ? 0 : Convert.ToDecimal(stcFromPrice);
                    Decimal dStcToPrice = String.IsNullOrEmpty(stcToPrice) ? 0 : Convert.ToDecimal(stcToPrice);
                    CommonBAL.PeakPayDistributedWithoutCache(string.Join(",", new List<int> { SolarcompanyId }), ResellerId, ref lstPeakPayViewToReturn, pageNumber, gridParam.PageSize, StageId, gridParam.SortCol, gridParam.SortDir, searchText, dStcFromPrice, dStcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                    totalAllAmount = lstPeakPayViewToReturn != null && lstPeakPayViewToReturn.Any() ? lstPeakPayViewToReturn.Select(dr => dr.Total).Sum() : 0;
                    totalRecords = lstPeakPayViewToReturn != null && lstPeakPayViewToReturn.Any() ? lstPeakPayViewToReturn.FirstOrDefault().Totalcount : 0;
                }
                else
                {
                    #region Selected specific SolarCompanyId
                    List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(new DataTable());
                    if (mainJobCache != null && mainJobCache.Any(R => R.PID == SolarcompanyId))
                    {
                        CommonBAL.PeakPayDistributedCacheHashGet(cache, new List<int> { SolarcompanyId }, ref lstPeakPayView);
                    }
                    else
                    {
                        CommonBAL.PeakPayDistributedCacheHashSet(cache, Convert.ToString(SolarcompanyId), ResellerId, ref ds, ref lstPeakPayView);
                    }
                    totalAllAmount = lstPeakPayView != null && lstPeakPayView.Any() ? lstPeakPayView.Select(dr => dr.Total).Sum() : 0;
                    PrepareFilterForPeakPay(ref filter, ref sort, StageId, gridParam.SortCol, gridParam.SortDir, searchText, stcFromPrice, stcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                    lstPeakPayViewToReturn = lstPeakPayView.AsQueryable().ToDataSourceResultSortFilter(sort, filter);
                    //DataTable newDatatable = FilteringAndSortingStaticPeakPayDataTable(ds.Tables[0], StageId, gridParam.SortCol, gridParam.SortDir, searchText, stcFromPrice, stcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
                    totalRecords = lstPeakPayViewToReturn.Count;
                    if (totalRecords > 0)
                        lstPeakPayViewToReturn = lstPeakPayViewToReturn.Skip((pageNumber - 1) * gridParam.PageSize).Take(gridParam.PageSize).ToList();
                    #endregion
                }
            }

            if (lstPeakPayViewToReturn.Count > 0)
            {
                gridParam.TotalDisplayRecords = totalRecords;
                gridParam.TotalRecords = totalRecords;
                gridParam.TotalAmount = totalAllAmount;
                HttpContext.Response.Write(Grid.PrepareDataSet(lstPeakPayViewToReturn, gridParam));
            }
            else
            {
                gridParam.TotalDisplayRecords = 0;
                gridParam.TotalRecords = 0;
                gridParam.TotalAmount = 0;
                List<dynamic> lstJobsSTCSubmission = new List<dynamic>();
                HttpContext.Response.Write(Grid.PrepareDataSet(lstJobsSTCSubmission, gridParam));
            }
        }
    }
}
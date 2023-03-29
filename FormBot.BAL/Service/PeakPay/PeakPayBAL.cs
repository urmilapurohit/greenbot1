using Dapper;
using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public class PeakPayBAL : IPeakPayBAL
    {
        /// <summary>
        /// Gets the peak pay list.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="SearchText">The search text.</param>
        /// <param name="StcFromPrice">The STC from price.</param>
        /// <param name="StcToPrice">The STC to price.</param>
        /// <param name="CERApprovedFromDate">The cer approved from date.</param>
        /// <param name="CERApprovedToDate">The cer approved to date.</param>
        /// <param name="SettlementFromDate">The settlement from date.</param>
        /// <param name="SettlementToDate">The settlement to date.</param>
        /// <param name="PaymentFromDate">The payment from date.</param>
        /// <param name="PaymentToDate">The payment to date.</param>
        /// <param name="StcStatusId">The STC status identifier.</param>
        /// <param name="IsSentInvoice">if set to <c>true</c> [is sent invoice].</param>
        /// <param name="IsUnsentInvoice">if set to <c>true</c> [is unsent invoice].</param>
        /// <param name="IsReadytoSTCInvoice">if set to <c>true</c> [is readyto STC invoice].</param>
        /// <returns></returns>
        public List<PeakPay> GetPeakPayList(int StageId,int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int SolarCompanyId, string SearchText, decimal StcFromPrice, decimal StcToPrice, DateTime? CERApprovedFromDate, DateTime? CERApprovedToDate, DateTime? SettleBeforeFromDate, DateTime? SettleBeforeToDate, DateTime? PaymentFromDate, DateTime? PaymentToDate, int StcStatusId, bool IsSentInvoice, bool IsUnsentInvoice, bool IsReadytoSTCInvoice,int SystemSize,string isAllScaJobView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, SearchText));
            sqlParameters.Add(DBClient.AddParameters("StcFromPrice", SqlDbType.Decimal, StcFromPrice));
            sqlParameters.Add(DBClient.AddParameters("StcToPrice", SqlDbType.Decimal, StcToPrice));
            sqlParameters.Add(DBClient.AddParameters("CERApprovedFromDate", SqlDbType.DateTime, CERApprovedFromDate != null ? CERApprovedFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("CERApprovedToDate", SqlDbType.DateTime, CERApprovedToDate != null ? CERApprovedToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettleBeforeFromDate", SqlDbType.DateTime, SettleBeforeFromDate != null ? SettleBeforeFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettleBeforeToDate", SqlDbType.DateTime, SettleBeforeToDate != null ? SettleBeforeToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("PaymentFromDate ", SqlDbType.DateTime, PaymentFromDate != null ? PaymentFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("PaymentToDate", SqlDbType.DateTime, PaymentToDate != null ? PaymentToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("StcStatusId", SqlDbType.Int, StcStatusId));
            sqlParameters.Add(DBClient.AddParameters("IsSentInvoice", SqlDbType.Bit, IsSentInvoice));
            sqlParameters.Add(DBClient.AddParameters("IsUnsentInvoice", SqlDbType.Bit, IsUnsentInvoice));
            sqlParameters.Add(DBClient.AddParameters("IsReadyToSentInvoice", SqlDbType.Bit, IsReadytoSTCInvoice));
            sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Int, SystemSize));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            List<PeakPay> lstPeakPay = CommonDAL.ExecuteProcedure<PeakPay>("PeakPay_GetPeakPayListNew", sqlParameters.ToArray()).ToList();

            return lstPeakPay;
        }

        /// <summary>
        /// Gets the peak pay CSV.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns></returns>
        public DataSet GetPeakPayCSV(string JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("CSV_GetPeakPay", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Imports the peak pay CSV.
        /// </summary>
        /// <param name="dtCsv">The dt CSV.</param>
        /// <returns></returns>
        public DataTable ImportPeakPayCsv(DataTable dtCsv)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCSV", SqlDbType.Structured, dtCsv));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.NVarChar, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));

            DataSet ds = CommonDAL.ExecuteDataSet("ImportCSV_PeakPay", sqlParameters.ToArray());
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public DataSet SetStcPricePeakPay(string stcJobDetailIds,decimal stcPrice)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.NVarChar, stcJobDetailIds));
            sqlParameters.Add(DBClient.AddParameters("STCPrice", SqlDbType.Decimal, stcPrice));
            sqlParameters.Add(DBClient.AddParameters("STCPriceSetBy", SqlDbType.NVarChar, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("STCPriceSetDate", SqlDbType.DateTime, DateTime.Now));

            DataSet ds = CommonDAL.ExecuteDataSet("PeakPay_SetStcPrice", sqlParameters.ToArray());
            return ds;
        }

        public int ChangePeakpayInvoiceStatus(string stcJobDetailIds, int invoiceStatus)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.NVarChar, stcJobDetailIds));
            sqlParameters.Add(DBClient.AddParameters("InvoiceStatus", SqlDbType.Int, invoiceStatus));

            object objinvoiceStatus = CommonDAL.ExecuteScalar("PeakPay_ChangeInvoiceStatus", sqlParameters.ToArray());
            return Convert.ToInt32(objinvoiceStatus);           
        }

        public List<JobStage> GetPeakPayJobStagesWithCount(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId,string isAllScaJobView)
        {
            string spName = "[PeakPay_GetSTCJobSatgesWithCountNew]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            List<JobStage> lstPeakPayJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstPeakPayJobStage;
        }

        public DataSet GetPeakPayListForCache(string SolarCompanyId, int ResellerId, string JobIds = "", string STCjobdetailIds = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.VarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.VarChar, JobIds));
            sqlParameters.Add(DBClient.AddParameters("STCJobdetailIds", SqlDbType.VarChar, STCjobdetailIds));
            return CommonDAL.ExecuteDataSet("PeakPay_GetPeakPayListNew", sqlParameters.ToArray());
        }

        public List<PeakPayView> GetPeakPayListForWithoutCache(string solarCompanyIds, int resellerId, int pageNumber, int pageSize, int stageId, string sortCol, string sortDir, string searchText, 
            Decimal stcFromPrice, Decimal stcToPrice, string cerApprovedFromDate, string cerApprovedToDate, string settleBeforeFromDate, string settleBeforeToDate, string paymentFromDate, 
            string paymentToDate, bool isSentInvoice, bool isUnsentInvoice, bool isReadytoSentInvoice, string systSize)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("SolarCompanyID", solarCompanyIds, DbType.String);
            dynamicParameters.Add("ResellerId", resellerId, DbType.Int32);
            dynamicParameters.Add("page", pageNumber, DbType.Int32);
            dynamicParameters.Add("pagesize", pageSize, DbType.Int32);
            dynamicParameters.Add("stageId", stageId, DbType.Int32);
            dynamicParameters.Add("sortCol", sortCol, DbType.String);
            dynamicParameters.Add("sortDir", sortDir, DbType.String);
            dynamicParameters.Add("searchText", searchText, DbType.String);
            dynamicParameters.Add("stcFromPrice", stcFromPrice, DbType.Decimal);
            dynamicParameters.Add("stcToPrice", stcToPrice, DbType.Decimal);
            dynamicParameters.Add("cerApprovedFromDate", cerApprovedFromDate, DbType.String);
            dynamicParameters.Add("cerApprovedToDate", cerApprovedToDate, DbType.String);
            dynamicParameters.Add("settleBeforeFromDate", settleBeforeFromDate, DbType.String);
            dynamicParameters.Add("settleBeforeToDate", settleBeforeToDate, DbType.String);
            dynamicParameters.Add("paymentFromDate", paymentFromDate, DbType.String);
            dynamicParameters.Add("paymentToDate", paymentToDate, DbType.String);
            dynamicParameters.Add("isSentInvoice", isSentInvoice, DbType.Boolean);
            dynamicParameters.Add("isUnsentInvoice", isUnsentInvoice, DbType.Boolean);
            dynamicParameters.Add("isReadytoSentInvoice", isReadytoSentInvoice, DbType.Boolean);
            dynamicParameters.Add("systSize", systSize, DbType.String);
            return CommonDAL.ExecuteProcedureDapper<PeakPayView>("PeakPay_GetPeakPayListNewStatic", dynamicParameters);
        }
    }
}

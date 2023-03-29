using FormBot.Entity;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IPeakPayBAL
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
        List<PeakPay> GetPeakPayList(int StageId,int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int SolarCompanyId, string SearchText, decimal StcFromPrice, decimal StcToPrice, DateTime? CERApprovedFromDate, DateTime? CERApprovedToDate, DateTime? SettleBeforeFromDate, DateTime? SettleBeforeToDate, DateTime? PaymentFromDate, DateTime? PaymentToDate, int StcStatusId, bool IsSentInvoice, bool IsUnsentInvoice, bool IsReadytoSTCInvoice, int SystemSize,string isAllScaJobView);

        /// <summary>
        /// Gets the peak pay CSV.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns></returns>
        DataSet GetPeakPayCSV(string JobId);

        /// <summary>
        /// Imports the peak pay CSV.
        /// </summary>
        /// <param name="dtCsv">The dt CSV.</param>
        /// <returns></returns>
        DataTable ImportPeakPayCsv(DataTable dtCsv);

        DataSet SetStcPricePeakPay(string stcJobDetailIds, decimal stcPrice);

        int ChangePeakpayInvoiceStatus(string stcJobDetailIds, int invoiceStatus);

        List<JobStage> GetPeakPayJobStagesWithCount(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId,string isAllScaJobView);

        DataSet GetPeakPayListForCache(string SolarCompanyId, int ResellerId, string JobIds = "", string STCjobdetailIds = "");

        List<PeakPayView> GetPeakPayListForWithoutCache(string solarCompanyIds, int resellerId, int pageNumber, int pageSize, int stageId, string sortCol, string sortDir, string searchText, Decimal stcFromPrice, Decimal stcToPrice, string cerApprovedFromDate, string cerApprovedToDate, string settleBeforeFromDate, string settleBeforeToDate, string paymentFromDate, string paymentToDate, bool isSentInvoice, bool isUnsentInvoice, bool isReadytoSentInvoice, string systSize);
    }
}

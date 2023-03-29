using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using FormBot.Entity.Job;
using System.Data;

namespace FormBot.BAL.Service.Job
{
    public interface IJobInvoiceDetailBAL
    {
        /// <summary>
        /// Gets the job invoice detail.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="jobInvoiceType">Type of the job invoice.</param>
        /// <param name="isInvoiced">The is invoiced.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>invoice list</returns>
        List<JobInvoiceDetail> GetJobInvoiceDetail(string jobInvoiceID, int jobInvoiceType, int isInvoiced, string sortColumn, string sortDirection);

        /// <summary>
        /// Jobs the visit data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet JobVisitData(int userId, int userTypeId, int jobId);

        /// <summary>
        /// Gets the job parts by identifier.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobPartsById(int jobInvoiceDetailID);

        /// <summary>
        /// Inserts the job invoice detail.
        /// </summary>
        /// <param name="jobInvoiceDetailID">The job invoice detail identifier.</param>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="jobPartID">The job part identifier.</param>
        /// <param name="isBillable">if set to <c>true</c> [is billable].</param>
        /// <param name="jobScheduleID">The job schedule identifier.</param>
        /// <param name="staff">The staff.</param>
        /// <param name="sale">The sale.</param>
        /// <param name="timeStart">The time start.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="purchase">The purchase.</param>
        /// <param name="margin">The margin.</param>
        /// <param name="description">The description.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="timeEnd">The time end.</param>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="PaymentAmount">The payment amount.</param>
        /// <param name="jobInvoiceType">Type of the job invoice.</param>
        /// <param name="isInvoiced">if set to <c>true</c> [is invoiced].</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="isDeleted">if set to <c>true</c> [is deleted].</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="isPart">if set to <c>true</c> [is part].</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="OwnerUsername">The owner username.</param>
        /// <param name="SendTo">The send to.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>DataSet</returns>
        DataSet InsertJobInvoiceDetail(int jobInvoiceDetailID, int? jobInvoiceID, int? jobPartID, bool isBillable, int jobScheduleID, int staff, decimal? sale, DateTime timeStart, string itemCode, decimal? quantity, decimal? purchase, decimal? margin, string description, string fileName, DateTime? timeEnd, int? paymentType, decimal? PaymentAmount, int jobInvoiceType, bool isInvoiced, int? createdBy, int? modifiedBy, bool isDeleted, int? solarCompanyId, bool isPart, string invoiceNumber, int jobId, string OwnerUsername = null, int? SendTo = null, FormBot.Entity.Settings.Settings settings = null, int? taxRateId =null, string saleAccountCode = null);

        /// <summary>
        /// Deletes the invoice detail.
        /// </summary>
        /// <param name="ids">The ids.</param>
        void DeleteInvoiceDetail(string ids);

        /// <summary>
        /// Gets the invoice detsil for report.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsJobAddress">if set to <c>true</c> [is job address].</param>
        /// <param name="IsJobDate">if set to <c>true</c> [is job date].</param>
        /// <param name="IsJobDescription">if set to <c>true</c> [is job description].</param>
        /// <param name="IsTitle">if set to <c>true</c> [is title].</param>
        /// <param name="IsName">if set to <c>true</c> [is name].</param>
        /// <returns>DataSet</returns>
        DataSet GetInvoiceDetsilForReport(int jobInvoiceID, bool IsJobAddress, bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName);

        /// <summary>
        /// Gets the invoice all detail for CSV.
        /// </summary>
        /// <param name="jobInvoiceID">The job invoice identifier.</param>
        /// <param name="IsTaxInclusive">if set to <c>true</c> [is tax inclusive].</param>
        /// <param name="DueDate">The due date.</param>
        /// <returns>DataSet</returns>
        DataSet GetInvoiceAllDetailForCSV(int jobInvoiceID, bool IsTaxInclusive, string DueDate);

        /// <summary>
        /// Gets the invoice setting.
        /// </summary>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="itemCode">itemCode.</param>
        /// <returns>Tuple</returns>
        Tuple<DateTime, string, string, decimal, string, bool, decimal> GetInvoiceSetting(int? SolarCompanyId = null, int userTypeId = 0, int? resellerId = 0, string itemCode = "");
    }

}

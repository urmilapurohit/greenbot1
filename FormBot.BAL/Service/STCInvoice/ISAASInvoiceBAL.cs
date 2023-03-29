using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;

namespace FormBot.BAL.Service
{
    public interface ISAASInvoiceBAL
    {
        /// <summary>
        /// Gets Settlement Terms for SAAS
        /// </summary>
        /// <returns>SettlementTerms List</returns>
        dynamic GetSAASSettlementTerms();

        /// <summary>
        /// Gets the SAAS invoice lists.
        /// </summary>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="reseller">The SAAS User identifier.</param>
        /// <param name="InvoiceId">The Invoice identifier.</param>
        /// <param name="createdFromDate">The Invoice Created Date</param>
        /// <param name="createdToDate">The Invoice Created To Date</param>
        /// <param name="invoiceDueFromDate">The Invoice Due From Date</param>
        /// <param name="invoiceDueToDate">The Invoice To Date</param>
        /// <param name="Owner">The Owner Name</param>
        /// <param name="BillingPeriod">The Billiing Period</param>
        /// <param name="JobID">THe Job Identifier</param>
        /// <param name="settlementTermId">The Settlement Tem Identifier</param>
        /// <param name="IsSent">if set to <c>true</c> [is sent invoice].</param>
        /// <returns>SAAS Invoice List</returns>
        DataSet GetSAASInvoiceList(int PageNumber, int pageSize, string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent, string BillingPeriod);

        /// <summary>
        /// Gets the SAAS invoice lists For Export.
        /// </summary>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="reseller">The SAAS User identifier.</param>
        /// <param name="InvoiceId">The Invoice identifier.</param>
        /// <param name="createdFromDate">The Invoice Created Date</param>
        /// <param name="createdToDate">The Invoice Created To Date</param>
        /// <param name="invoiceDueFromDate">The Invoice Due From Date</param>
        /// <param name="invoiceDueToDate">The Invoice To Date</param>
        /// <param name="Owner">The Owner Name</param>
        /// <param name="BillingPeriod">The Billiing Period</param>
        /// <param name="JobID">THe Job Identifier</param>
        /// <param name="settlementTermId">The Settlement Tem Identifier</param>
        /// <param name="IsSent">if set to <c>true</c> [is sent invoice].</param>
        /// <returns>Dataset</returns>
        DataSet GetSAASInvoiceListForExport(string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent);
        DataSet GetSAASInvoiceListForExportAll(string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent);

        // dynamic GetSAASInvoiceDetail(int InvoiceId);

        dynamic GetSAASInvoiceDetail(string strJobID, string IsInvoiced,string InvoiceId);

        /// <summary>
        /// Updates the xero invoice identifier.
        /// </summary>
        /// <param name="STCInvoiceJson">The STC invoice json.</param>
        /// <returns>invoice identifier</returns>
        int UpdateXeroInvoiceId(int SAASInvoiceID, string SAASXeroInvoiceID);

        /// <summary>
        /// Inserts the STC invoice payment.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="UTCDate">The UTC date.</param>
        /// <param name="STCInvoiceData">The STC invoice data.</param>
        /// <returns>dataset</returns>
        List<Remittance> InsertSAASInvoicePayment(DataTable dtPayment, int createdBy, DateTime createdDate, string invoiceNumbers, int modifiedBy, DateTime modifiedDate, DateTime UTCDate, string STCInvoiceData);

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <returns>DataSet</returns>
        DataSet ImportSAASCSV(DataTable dtCSV, int resellerID);

        /// <summary>
        /// Get log of sync with xero
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns>list of log of sync with xero</returns>
        List<SyncWithXeroLog> GetSynxWithXeroLog(int ResellerId);

        /// <summary>
        /// insert gb batch invoice id
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns>list of log of sync with xero</returns>
        int SAASInvoiceCreate(DataTable dtCSV, int resellerID);

        /// <summary>
        /// get selected invoice
        /// </summary>
        /// <param name="STCInvoiceIds">stc invoice</param>
        /// <param name="resellerId">reseller identifier</param>
        /// <returns>DataSet</returns>
        DataSet GetSelectdSTCInvoice(string SAASInvoiceIds);
        void DeleteSAASInvoiceByID(string invoiceIds);
        void ImportUserTypeMenuCSV(DataTable dtCSV);
        int RestoreSAASInvoiceByID(int invoiceId);

        /// <summary>
        /// Marks the un mark selected as sent for payment.
        /// </summary>
        /// <param name="IsInvoiced">if set to <c>true</c> [is invoiced].</param>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        void MarkUnMarkSelectedAsSentForPayment(bool IsInvoiced, string STCInvoiceIDs);

        void CreateNewInvoice(string ResellerID, string SettlementTermId, string Price, string UnitQTY, bool IsGST);
    }
}

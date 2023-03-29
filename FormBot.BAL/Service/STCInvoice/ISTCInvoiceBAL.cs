using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;

namespace FormBot.BAL.Service
{
    public interface ISTCInvoiceBAL
    {
        /// <summary>
        /// Gets the STC invoice list.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RamId">The ram identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="InvoiceNumber">The invoice number.</param>
        /// <param name="RefJobId">The reference job identifier.</param>
        /// <param name="ownername">The ownername.</param>
        /// <param name="installationaddress">The installationaddress.</param>
        /// <param name="SubmissionFromDate">The submission from date.</param>
        /// <param name="SubmissionToDate">The submission to date.</param>
        /// <param name="SettlementFromDate">The settlement from date.</param>
        /// <param name="SettlementToDate">The settlement to date.</param>
        /// <param name="IsSTCInvoice">if set to <c>true</c> [is STC invoice].</param>
        /// <param name="IsCreditNotes">if set to <c>true</c> [is credit notes].</param>
        /// <param name="IsSentInvoice">if set to <c>true</c> [is sent invoice].</param>
        /// <param name="IsUnSentInvoice">if set to <c>true</c> [is un sent invoice].</param>
        /// <returns>stc list</returns> 
        dynamic GetSTCInvoiceList(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int StageId, int ResellerId, int RamId, int SolarCompanyId, string InvoiceNumber, string RefJobId, string ownername, string installationaddress, DateTime? SubmissionFromDate, DateTime? SubmissionToDate, DateTime? SettlementFromDate, DateTime? SettlementToDate, bool IsSTCInvoice, bool IsCreditNotes, bool IsSentInvoice, bool IsUnSentInvoice, int SettlementTermId,bool IsAllExportCsv,string isAllScaJobView ="");

        /// <summary>
        /// Gets the STC invoice stages with count.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RamId">The ram identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>job stage</returns>
        List<JobStage> GetSTCInvoiceStagesWithCount(int UserId, int UserTypeId,int ResellerId, int RamId, int SolarCompanyId,string isAllScaJobView = "");

        /// <summary>
        /// Generates the STC invoice for selected jobs.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="IsSTCInvoice">The is STC invoice.</param>
        /// <param name="dt">The dt.</param>
        /// <returns>dataset</returns>
        DataSet GenerateSTCInvoiceForSelectedJobs(int UserId, int UserTypeId, int ResellerId, int IsSTCInvoice, DataTable dt, DateTime? STCSettlementDateForInvoiceSTC);

        /// <summary>
        /// Gets the selectd STC invoice.
        /// </summary>
        /// <param name="STCInvoiceIds">The STC invoice ids.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>dataset</returns>
        DataSet GetSelectdSTCInvoice(string STCInvoiceIds,int resellerId);

        /// <summary>
        /// Gets the invoice number by reseller wise.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>reseller identifier</returns>
        int GetInvoiceNumberByResellerWise(int ResellerId);

        /// <summary>
        /// Updates the xero invoice identifier.
        /// </summary>
        /// <param name="STCInvoiceJson">The STC invoice json.</param>
        /// <returns>invoice identifier</returns>
        //int UpdateXeroInvoiceId(string STCInvoiceJson);
        int UpdateXeroInvoiceId(int STCInvoiceID, string STCXeroInvoiceID);

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
        List<Remittance> InsertSTCInvoicePayment(DataTable dtPayment, int createdBy, DateTime createdDate, int resellerID, int modifiedBy, DateTime modifiedDate, DateTime UTCDate, string STCInvoiceData);

        /// <summary>
        /// Gets the STC invoice by invoice identifier.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>stc invoice</returns>
        STCInvoice GetSTCInvoiceByInvoiceID(Int64 invoiceId);

        
        

        /// <summary>
        /// Get STC inovice report
        /// </summary>
        /// <param name="STCJobDetailsID"></param>
        /// <param name="IsJobAddress"></param>
        /// <param name="IsJobDate"></param>
        /// <param name="IsJobDescription"></param>
        /// <param name="IsTitle"></param>
        /// <param name="IsName"></param>
        /// <param name="CreatedDate"></param>
        /// <param name="stcinvoicenumber"></param>
        /// <returns>STCInvoiceReport</returns>
        STCInvoiceReport GetStcInvoice(int STCJobDetailsID, bool IsJobAddress, bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName, DateTime CreatedDate, string stcinvoicenumber);

        /// <summary>
        /// Gets the STC payment status.
        /// </summary>
        /// <returns>dataset</returns>
        DataSet GetSTCPaymentStatus();

        /// <summary>
        /// Gets the STC CSV.
        /// </summary>
        /// <param name="STCInvoiceID">The STC job details identifier.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <returns>dataset</returns>
        DataSet GetStcCSV(string STCInvoiceID, DateTime CreatedDate);

        /// <summary>
        /// Updates the PVD code by serial numbers.
        /// </summary>
        /// <param name="dtSerials">The serial number datatable.</param>
        DataSet UpdatePVDCodeByJobID(DataTable dtSerials,int userID);

        /// <summary>
        /// Updates the generated STC invoice.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <param name="STCJobDetailsID">The STC job details identifier.</param>
        /// <param name="STCInvoiceID">The STC invoice identifier.</param>
        /// <param name="IsGst">if set to <c>true</c> [is GST].</param>
        /// <param name="STCValue">The STC value.</param>
        /// <param name="PaymentStatusID">The payment status identifier.</param>
        /// <param name="SettlementTerms">The settlement terms.</param>
        /// <param name="STCAmount">The STC amount.</param>
        /// <param name="Notes">The notes.</param>
        /// <param name="Total">The total.</param>
        void UpdateGeneratedSTCInvoice(int JobID, int STCJobDetailsID, long STCInvoiceID, bool IsGst, decimal STCValue, int PaymentStatusID, int SettlementTerms, decimal STCAmount, string Notes,decimal Total);

        /// <summary>
        /// Gets the payment status.
        /// </summary>
        /// <returns>stc list</returns>
        List<STCInvoice> GetPaymentStatus();

        /// <summary>
        /// Updates the file path.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <returns>invoice payment</returns>
        int UpdateFilePath(string STCInvoicePaymentJson);

        /// <summary>
        /// Bulks the change payment status.
        /// </summary>
        /// <param name="PaymentStatusID">The payment status identifier.</param>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        DataSet BulkChangePaymentStatus(int PaymentStatusID,string STCInvoiceIDs);

        /// <summary>
        /// Removes the selected STC invoice.
        /// </summary>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        void RemoveSelectedSTCInvoice(string STCInvoiceIDs);
       

        /// <summary>
        /// Marks the un mark selected as sent for payment.
        /// </summary>
        /// <param name="IsInvoiced">if set to <c>true</c> [is invoiced].</param>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        void MarkUnMarkSelectedAsSentForPayment(bool IsInvoiced, string STCInvoiceIDs);

        /// <summary>
        /// Removes the remittence file.
        /// </summary>
        /// <param name="STCInvoiceID">The STC invoice identifier.</param>
        /// <param name="FilePath">The file path.</param>
        void RemoveRemittenceFile(Int64 STCInvoiceID, string FilePath);

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        DataSet ImportCSV(DataTable dtCSV, int createdBy, DateTime createdDate, int resellerID, int solarCompanyId);

        /// <summary>
        /// Updates the record generated invoice file path.
        /// </summary>
        /// <param name="dt">The dt.</param>
        List<int> UpdateRecGeneratedInvoiceFilePath(DataTable dt);

        /// <summary>
        /// Inserts the bulk upload files.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="PaymentDate">The payment date.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="CreatedBy">The created by.</param>
        /// <returns>created by</returns>
        int InsertBulkUploadFiles(string STCInvoicePaymentJson, DateTime PaymentDate, DateTime CreatedDate, int CreatedBy);

        DataSet TempRecord();

        /// <summary>
        /// Gets the STC amount paid detail records.
        /// </summary>
        /// <param name="STCInvoiceId">The STC invoice identifier.</param>
        /// <returns></returns>
        List<STCInvoicePayment> GetSTCAmountPaidDetailRecords(string SortCol, string SortDir, long STCInvoiceId);

        /// <summary>
        /// Updates the STC amount paid record.
        /// </summary>
        /// <param name="STCInvoicePaymentID">The STC invoice payment identifier.</param>
        /// <param name="Payment">The payment.</param>
        /// <param name="PaymentDate">The payment date.</param>
        List<Remittance> UpdateSTCAmountPaidRecord(int ResellerID, long STCInvoicePaymentID, decimal Payment, DateTime PaymentDate);

        /// <summary>
        /// Adds the STC amount paid record.
        /// </summary>
        /// <param name="ResellerID">The reseller identifier.</param>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="STCInvoiceID">The STC invoice identifier.</param>
        /// <param name="Payment">The payment.</param>
        /// <param name="PaymentDate">The payment date.</param>
        /// <returns></returns>
        List<Remittance> AddSTCAmountPaidRecord(int ResellerID, int UserID, DateTime CreatedDate, long STCInvoiceID,decimal Payment, DateTime PaymentDate);

        /// <summary>
        /// GetRemittance
        /// </summary>
        /// <param name="ResellerID"></param>
        /// <param name="STCInvoicePaymentID"></param>
        /// <param name="SolarCompanyId"></param>
        /// <returns>remittance</returns>
        List<Remittance> GetRemittance(int ResellerID, string STCInvoicePaymentID);

        /// <summary>
        /// Deletes the STC amount paid record.
        /// </summary>
        /// <param name="STCInvoicePaymentID">The STC invoice payment identifier.</param>
        /// <returns></returns>
        DataSet DeleteSTCAmountPaidRecord(long STCInvoicePaymentID);

        DataSet GetSolarCompanyAndResellerAddress(int SolarCompanyId, int ResellerID);

        DataSet RegenerateRemittanceFile(int resellerID, string stcInvoiceNumber);


        /// <summary>
        /// Update GBBatchUpdateId by appending RecbulkUploadId
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <param name="RecBulkUploadId"></param>
        void UpdateRECUploadId(string STCJobDetailsId, int RecBulkUploadId);

        /// <summary>
        /// Update rec failed record in stc submission
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <param name="TimeOut"></param>
        void UpdateRECFailedRecord(string STCJobDetailsId, bool isRecFailed = false);

        void InsertRECEntryFailureReason(int JobId, string FailureReason, int UserId);

        
        void UpdateRecSearchFailedRecord(string STCJobDetailsId, bool IsRECSearchFailed = false);

        DataTable GetJobsForUpdatingPvdCodeFromRec(string recUploadId);

        DataSet GetAllocationRecords(int STCInvoiceId);

        /// <summary>
        /// Import Bulk upload data
        /// </summary>
        /// <param name="dtImportPVDSWHCode"></param>
        /// <param name="bulkUploadId"></param>
        /// <returns></returns>
        DataSet ImportBulkUploadData(DataTable dtImportPVDSWHCode, string bulkUploadId);

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
        void InsertGbInvoiceBatchId(DataTable dt, int resellerId);
        /// <summary>
        /// Get stc data from STCInvoiceIds
        /// </summary>
        /// <param name="STCInvoiceIDs">The STC invoice ids.</param>
        DataSet GetStcDataFromStcInvoiceIds(string STCInvoiceIDs);
 		DataSet GetRECFailedBatchDetails(string batchId,string STCJobDetailsId ="",bool isFailedJob=true);

        void UpdateQueuedSubmissionStatus(string batchId, string Status);
        /// <summary>
        /// Get job details from batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        DataSet GetJobDetailsBatchWise(string batchId);
        /// <summary>
        /// update internal issue flag in db
        /// </summary>
        /// <param name="jobIds"></param>
        /// <param name="errorMessage"></param>
        void UpdateInternalIssue(string recBulkUploadId, string errorMessage);
        /// <summary>
        /// remove from queued submission table
        /// </summary>
        /// <param name="Jobids"></param>
        void RemoveFromQueuedRecSubmission(string Jobids);
        /// <summary>
        /// get unknown issues and internal error
        /// </summary>
        /// <param name="BulkUploadId"></param>
        /// <returns></returns>
        DataSet GetUnknownIssues(string BulkUploadId);

        DataTable GetJobDetailsForPVDCode(int jobID, int stcJobDetailsID);

        /// <summary>
        /// Gets the jobids for create batch for automated process.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>reseller identifier.</returns>
        DataSet GetJobIdsForBatchByResellerWise(int ResellerId, bool IsBeforeAprilInstallation);

        DataTable GetJobsForPVDCode();
        string GetSTCAmountPaidId(string path);
    }
}

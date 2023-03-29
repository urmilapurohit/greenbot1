using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using FormBot.DAL;
using System.Globalization;
using System.Reflection;
using FormBot.Helper;

namespace FormBot.BAL.Service
{
    public class STCInvoiceBAL : ISTCInvoiceBAL
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
        /// <returns>
        /// stc list
        /// </returns>
        public dynamic GetSTCInvoiceList(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int StageId, int ResellerId, int RamId, int SolarCompanyId, string InvoiceNumber, string RefJobId, string ownername, string installationaddress, DateTime? SubmissionFromDate, DateTime? SubmissionToDate, DateTime? SettlementFromDate, DateTime? SettlementToDate, bool IsSTCInvoice, bool IsCreditNotes, bool IsSentInvoice, bool IsUnSentInvoice, int SettlementTermId, bool IsAllExportCsv, string isAllScaJobView = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            sqlParameters.Add(DBClient.AddParameters("InvoiceNumber", SqlDbType.NVarChar, InvoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("RefJobId", SqlDbType.NVarChar, RefJobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, ownername));
            sqlParameters.Add(DBClient.AddParameters("InstallationAddress", SqlDbType.NVarChar, installationaddress));
            sqlParameters.Add(DBClient.AddParameters("SubmissionFromDate", SqlDbType.DateTime, SubmissionFromDate != null ? SubmissionFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SubmissionToDate", SqlDbType.DateTime, SubmissionToDate != null ? SubmissionToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettlementFromDate", SqlDbType.DateTime, SettlementFromDate != null ? SettlementFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettlementToDate", SqlDbType.DateTime, SettlementToDate != null ? SettlementToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("IsSTCInvoice", SqlDbType.Bit, IsSTCInvoice));
            sqlParameters.Add(DBClient.AddParameters("IsCreditNotes", SqlDbType.Bit, IsCreditNotes));
            sqlParameters.Add(DBClient.AddParameters("IsSentInvoice", SqlDbType.Bit, IsSentInvoice));
            sqlParameters.Add(DBClient.AddParameters("IsUnSentInvoice", SqlDbType.Bit, IsUnSentInvoice));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.Int, SettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("IsAllExportCsv", SqlDbType.Bit, IsAllExportCsv));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            dynamic lstSTCInvoice;
            if (IsAllExportCsv)
            {
                DataSet ds = CommonDAL.ExecuteDataSet("STCInvoice_GetSTCInvoiceList", sqlParameters.ToArray());
                var myData = ds.Tables[0].AsEnumerable().Select(a => a.Field<Int64>("STCInvoiceID")).ToList();
                lstSTCInvoice = myData;

            }
            else
            {
                lstSTCInvoice = CommonDAL.ExecuteProcedure<STCInvoice>("STCInvoice_GetSTCInvoiceList", sqlParameters.ToArray()).ToList();
            }

            /*List<STCInvoice> lstInvoice = lstSTCInvoice.Cast<STCInvoice>().ToList();*/
            return lstSTCInvoice;
        }

        /// <summary>
        /// Gets the STC invoice stages with count.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RamId">The ram identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>job stage</returns>
        public List<JobStage> GetSTCInvoiceStagesWithCount(int userId, int userTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView = "")
        {
            string spName = "[STCInvoice_GetSTCInvoiceStagesWithCount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            List<JobStage> lstJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstJobStage;
        }

        /// <summary>
        /// Generates the STC invoice for selected jobs.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="IsSTCInvoice">The is STC invoice.</param>
        /// <param name="dt">The dt.</param>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GenerateSTCInvoiceForSelectedJobs(int UserId, int UserTypeId, int ResellerId, int IsSTCInvoice, DataTable dt, DateTime? STCSettlementDateForInvoiceSTC)
        {
            TimeSpan onApprovalCutOffTime = TimeSpan.Parse(ProjectConfiguration.OnApprovalSettlementCutOffTime);
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DateTime currentDate = DateTime.Now;

            //for onapproval settlement term  Traded before 3pm - same day payment. Traded after 3pm  - next day payment.
            if (currentTime > onApprovalCutOffTime)
                currentDate = currentDate.AddDays(1);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("IsSTCInvoice", SqlDbType.Int, IsSTCInvoice));
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsIDs", SqlDbType.Structured, dt));
            sqlParameters.Add(DBClient.AddParameters("STCSettlementDate", SqlDbType.DateTime, currentDate));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateAndTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("STCSettlementDateForInvoiceSTC", SqlDbType.DateTime, STCSettlementDateForInvoiceSTC));

            DataSet stcInvoice = CommonDAL.ExecuteDataSet("STCInvoice_GenerateSTCInvoiceForSelectedJobs", sqlParameters.ToArray());
            return stcInvoice;
        }

        /// <summary>
        /// Gets the job reference number by address_ installation date.
        /// </summary>
        /// <param name="Installation_streetnumber">The installation_streetnumber.</param>
        /// <param name="Installation_streetname">The installation_streetname.</param>
        /// <param name="Installation_streettype">The installation_streettype.</param>
        /// <param name="Installation_town_suburb">The installation_town_suburb.</param>
        /// <param name="Installation_state">The installation_state.</param>
        /// <param name="Installation_postcode">The installation_postcode.</param>
        /// <param name="Installation_date">The installation_date.</param>
        /// <returns>DataSet</returns>
        public DataSet GetJobRefNumberByAddress_InstallationDate(string Installation_streetnumber, string Installation_streetname, string Installation_streettype, string Installation_town_suburb, string Installation_state, string Installation_postcode, DateTime Installation_date)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Installation_streetnumber", SqlDbType.NVarChar, Installation_streetnumber));
            sqlParameters.Add(DBClient.AddParameters("Installation_streetname", SqlDbType.NVarChar, Installation_streetname));
            sqlParameters.Add(DBClient.AddParameters("Installation_streettype", SqlDbType.NVarChar, Installation_streettype));
            sqlParameters.Add(DBClient.AddParameters("Installation_town_suburb", SqlDbType.NVarChar, Installation_town_suburb));
            sqlParameters.Add(DBClient.AddParameters("Installation_state", SqlDbType.NVarChar, Installation_state));
            sqlParameters.Add(DBClient.AddParameters("Installation_postcode", SqlDbType.NVarChar, Installation_postcode));
            sqlParameters.Add(DBClient.AddParameters("Installation_date", SqlDbType.NVarChar, Installation_date));
            DataSet JobRefDetils = CommonDAL.ExecuteDataSet("[GetJobRefNumberByInstallementDetails]", sqlParameters.ToArray());
            return JobRefDetils;
        }

        /// <summary>
        /// Updates the PVD code by serial numbers.
        /// </summary>
        /// <param name="dtSerials">The serial number datatable.</param>
        public DataSet UpdatePVDCodeByJobID(DataTable dtSerials, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("tab_STCSubmissinon_PVD_JobID", SqlDbType.Structured, dtSerials));
            sqlParameters.Add(DBClient.AddParameters("RECCreationDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            DataSet ds = CommonDAL.ExecuteDataSet("[BulkUpdatePVDCode_JobID]", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// get selected invoice
        /// </summary>
        /// <param name="STCInvoiceIds">stc invoice</param>
        /// <param name="resellerId">reseller identifier</param>
        /// <returns>DataSet</returns>
        public DataSet GetSelectdSTCInvoice(string STCInvoiceIds, int resellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceIds", SqlDbType.NVarChar, STCInvoiceIds));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, resellerId));
            DataSet stcInvoice = CommonDAL.ExecuteDataSet("STCInvoice_GetSelectdSTCInvoice", sqlParameters.ToArray());
            return stcInvoice;
        }

        /// <summary>
        /// Gets the invoice number by reseller wise.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>reseller identifier.</returns>
        public int GetInvoiceNumberByResellerWise(int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            object invoiceNumber = CommonDAL.ExecuteScalar("STCInvoice_GetInvoiceNumberByResellerWise", sqlParameters.ToArray());
            if (invoiceNumber != null)
            {
                return Convert.ToInt32(Convert.ToString(invoiceNumber).Split('-').Last());
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Updates the xero invoice identifier.
        /// </summary>
        /// <param name="STCInvoiceJson">The STC invoice json.</param>
        /// <returns>invoice identifier</returns>
        //public int UpdateXeroInvoiceId(string STCInvoiceJson)
        public int UpdateXeroInvoiceId(int STCInvoiceID, string STCXeroInvoiceID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("STCInvoiceJson", SqlDbType.NVarChar, STCInvoiceJson));
            sqlParameters.Add(DBClient.AddParameters("STCXeroInvoiceID", SqlDbType.NVarChar, STCXeroInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceID", SqlDbType.Int, STCInvoiceID));
            object response = CommonDAL.ExecuteScalar("STCInvoice_UpdateXeroInvoiceId", sqlParameters.ToArray());
            return Convert.ToInt32(response);
        }

        /// <summary>
        /// invoice payment
        /// </summary>
        /// <param name="STCInvoicePaymentJson">invoice payment json</param>
        /// <param name="createdBy">created By</param>
        /// <param name="createdDate">created Date</param>
        /// <param name="resellerID">reseller ID</param>
        /// <param name="modifiedBy">modified By</param>
        /// <param name="modifiedDate">modified Date</param>
        /// <param name="UTCDate">UTC Date</param>
        /// <param name="STCInvoiceData">STC Invoice Data</param>
        /// <returns>DataSet</returns>
        public List<Remittance> InsertSTCInvoicePayment(DataTable dtPayment, int createdBy, DateTime createdDate, int resellerID, int modifiedBy, DateTime modifiedDate, DateTime UTCDate, string STCInvoiceData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtPayment", SqlDbType.Structured, dtPayment));
            sqlParameters.Add(DBClient.AddParameters("createdBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, modifiedDate));
            sqlParameters.Add(DBClient.AddParameters("UTCDate", SqlDbType.DateTime, UTCDate));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceData", SqlDbType.NVarChar, STCInvoiceData));
            List<Remittance> remittance = CommonDAL.ExecuteProcedure<Remittance>("STCInvoicePayment_InsertSTCInvoicePayment", sqlParameters.ToArray()).ToList();
            return remittance;
        }

        /// <summary>
        /// Gets the STC invoice by invoice identifier.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>STCInvoice</returns>
        public STCInvoice GetSTCInvoiceByInvoiceID(Int64 invoiceId)
        {
            STCInvoice stcInvoice = CommonDAL.SelectObject<STCInvoice>("STCInvoice_GetSTCInvoiceByInvoiceID", DBClient.AddParameters("InvoiceID", SqlDbType.BigInt, invoiceId));
            return stcInvoice;
        }

        /// <summary>
        /// Gets the STC invoice.
        /// </summary>
        /// <param name="STCJobDetailsID">The STC job details identifier.</param>
        /// <param name="IsJobAddress">if set to <c>true</c> [is job address].</param>
        /// <param name="IsJobDate">if set to <c>true</c> [is job date].</param>
        /// <param name="IsJobDescription">if set to <c>true</c> [is job description].</param>
        /// <param name="IsTitle">if set to <c>true</c> [is title].</param>
        /// <param name="IsName">if set to <c>true</c> [is name].</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="stcinvoicenumber">The stcinvoicenumber.</param>
        /// <returns>
        /// dataset
        /// </returns>
        public STCInvoiceReport GetStcInvoice(int STCJobDetailsID, bool IsJobAddress, bool IsJobDate, bool IsJobDescription, bool IsTitle, bool IsName, DateTime CreatedDate, string stcinvoicenumber = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, IsJobAddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobDate", SqlDbType.Bit, IsJobDate));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, IsJobDescription));
            sqlParameters.Add(DBClient.AddParameters("IsTitle", SqlDbType.Bit, IsTitle));
            sqlParameters.Add(DBClient.AddParameters("IsName", SqlDbType.Bit, IsName));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("stcinvoicenumber", SqlDbType.NVarChar, stcinvoicenumber));
            STCInvoiceReport sTCInvoiceReport = CommonDAL.ExecuteProcedure<STCInvoiceReport>("RDLC_GetStcInvoice", sqlParameters.ToArray()).FirstOrDefault();
            return sTCInvoiceReport;
        }




        /// <summary>
        /// Gets the STC payment status.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetSTCPaymentStatus()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet stcInvoicePaymentStatus = CommonDAL.ExecuteDataSet("GetSTCInvoicePaymentStatus", sqlParameters.ToArray());
            return stcInvoicePaymentStatus;
        }

        /// <summary>
        /// Gets the STC CSV.
        /// </summary>
        /// <param name="STCInvoiceID">The STC job details identifier.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GetStcCSV(string STCInvoiceID, DateTime CreatedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceID", SqlDbType.NVarChar, STCInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            DataSet ds = CommonDAL.ExecuteDataSet("CSV_GetStcInvoice", sqlParameters.ToArray());
            return ds;
        }

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
        public void UpdateGeneratedSTCInvoice(int JobID, int STCJobDetailsID, long STCInvoiceID, bool IsGst, decimal STCValue, int PaymentStatusID, int SettlementTerms, decimal STCAmount, string Notes, decimal Total)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceID", SqlDbType.BigInt, STCInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, IsGst));
            sqlParameters.Add(DBClient.AddParameters("STCValue", SqlDbType.Decimal, STCValue));
            sqlParameters.Add(DBClient.AddParameters("PaymentStatusID", SqlDbType.Int, PaymentStatusID));
            sqlParameters.Add(DBClient.AddParameters("SettlementTerms", SqlDbType.Int, SettlementTerms));
            sqlParameters.Add(DBClient.AddParameters("STCAmount", SqlDbType.Decimal, STCAmount));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, Notes));
            sqlParameters.Add(DBClient.AddParameters("Total", SqlDbType.Decimal, Total));
            CommonDAL.Crud("STCInvoice_UpdateGeneratedSTCInvoice", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the payment status.
        /// </summary>
        /// <returns>
        /// stc list
        /// </returns>
        public List<STCInvoice> GetPaymentStatus()
        {
            string spName = "[STCInvoicePaymentStatus_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<STCInvoice> lstPaymentStatus = CommonDAL.ExecuteProcedure<STCInvoice>(spName, sqlParameters.ToArray());
            return lstPaymentStatus.ToList();
        }

        /// <summary>
        /// Updates the file path.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <returns>
        /// invoice payment
        /// </returns>
        public int UpdateFilePath(string STCInvoicePaymentJson)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoicePaymentJson", SqlDbType.NVarChar, STCInvoicePaymentJson));
            object response = CommonDAL.ExecuteScalar("STCInvoicePayment_UpdateFilePath", sqlParameters.ToArray());
            return Convert.ToInt32(response);
        }

        /// <summary>
        /// Bulks the change payment status.
        /// </summary>
        /// <param name="PaymentStatusID">The payment status identifier.</param>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        public DataSet BulkChangePaymentStatus(int PaymentStatusID, string STCInvoiceIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PaymentStatusID", SqlDbType.Int, PaymentStatusID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceIDs", SqlDbType.NVarChar, STCInvoiceIDs));
            DataSet ds = CommonDAL.ExecuteDataSet("STCInvoice_BulkChangePaymentStatus", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Removes the selected STC invoice.
        /// </summary>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        public void RemoveSelectedSTCInvoice(string STCInvoiceIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceIDs", SqlDbType.NVarChar, STCInvoiceIDs));
            CommonDAL.Crud("STCInvoice_RemoveSelectedSTCInvoice", sqlParameters.ToArray());
        }

        /// <summary>
        /// Marks the un mark selected as sent for payment.
        /// </summary>
        /// <param name="IsInvoiced">if set to <c>true</c> [is invoiced].</param>
        /// <param name="STCInvoiceIDs">The STC invoice i ds.</param>
        public void MarkUnMarkSelectedAsSentForPayment(bool IsInvoiced, string STCInvoiceIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsInvoiced", SqlDbType.Bit, IsInvoiced));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceIDs", SqlDbType.NVarChar, STCInvoiceIDs));
            CommonDAL.Crud("STCInvoice_MarkUnMarkSelectedAsSentForPayment", sqlParameters.ToArray());
        }

        /// <summary>
        /// Removes the remittence file.
        /// </summary>
        /// <param name="STCInvoiceID">The STC invoice identifier.</param>
        /// <param name="FilePath">The file path.</param>
        public void RemoveRemittenceFile(Int64 STCInvoiceID, string FilePath)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("STCInvoiceID", SqlDbType.NVarChar, STCInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("FilePath", SqlDbType.NVarChar, FilePath));
            CommonDAL.Crud("STCInvoicePayment_RemoveRemittenceFile", sqlParameters.ToArray());
        }

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet ImportCSV(DataTable dtCSV, int createdBy, DateTime createdDate, int resellerID, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCSV", SqlDbType.Structured, dtCSV));
            sqlParameters.Add(DBClient.AddParameters("createdBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            DataSet remittance = CommonDAL.ExecuteDataSet("STCInvoicePayment_ImportCSV", sqlParameters.ToArray());
            return remittance;
        }

        /// <summary>
        /// Updates the record generated invoice file path.
        /// </summary>
        /// <param name="dt">The dt.</param>
        public List<int> UpdateRecGeneratedInvoiceFilePath(DataTable dt)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoiceFilePaths", SqlDbType.Structured, dt));
            return CommonDAL.ExecuteProcedure<int>("STCInvoice_UpdateRecGeneratedInvoiceFilePath", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Inserts the bulk upload files.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="PaymentDate">The payment date.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="CreatedBy">The created by.</param>
        /// <returns>
        /// created by
        /// </returns>
        public int InsertBulkUploadFiles(string STCInvoicePaymentJson, DateTime PaymentDate, DateTime CreatedDate, int CreatedBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoicePaymentJson", SqlDbType.NVarChar, STCInvoicePaymentJson));
            sqlParameters.Add(DBClient.AddParameters("PaymentDate", SqlDbType.DateTime, PaymentDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy));
            object response = CommonDAL.ExecuteScalar("STCInvoicePayment_InsertBulkUploadFiles", sqlParameters.ToArray());
            return Convert.ToInt32(response);
        }

        public DataSet TempRecord()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet TempRecord = CommonDAL.ExecuteDataSet("TempRecord", sqlParameters.ToArray());
            return TempRecord;
        }

        /// <summary>
        /// Gets the STC amount paid detail records.
        /// </summary>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <param name="STCInvoiceId">The STC invoice identifier.</param>
        /// <returns></returns>
        public List<STCInvoicePayment> GetSTCAmountPaidDetailRecords(string SortCol, string SortDir, long STCInvoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceId", SqlDbType.BigInt, STCInvoiceId));
            List<STCInvoicePayment> lstSTCAmountPaid = CommonDAL.ExecuteProcedure<STCInvoicePayment>("STCInvoice_GetSTCAmountPaidDetailRecords", sqlParameters.ToArray()).ToList();
            return lstSTCAmountPaid;
        }

        /// <summary>
        /// Updates the STC amount paid record.
        /// </summary>
        /// <param name="STCInvoicePaymentID">The STC invoice payment identifier.</param>
        /// <param name="Payment">The payment.</param>
        /// <param name="PaymentDate">The payment date.</param>
        public List<Remittance> UpdateSTCAmountPaidRecord(int ResellerID, long STCInvoicePaymentID, decimal Payment, DateTime PaymentDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoicePaymentID", SqlDbType.BigInt, STCInvoicePaymentID));
            sqlParameters.Add(DBClient.AddParameters("Payment", SqlDbType.Decimal, Payment));
            sqlParameters.Add(DBClient.AddParameters("PaymentDate", SqlDbType.DateTime, PaymentDate));
            List<Remittance> remittance = CommonDAL.ExecuteProcedure<Remittance>("STCInvoice_UpdateSTCAmountPaidRecord", sqlParameters.ToArray()).ToList();
            return remittance;
        }

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
        public List<Remittance> AddSTCAmountPaidRecord(int ResellerID, int UserID, DateTime CreatedDate, long STCInvoiceID, decimal Payment, DateTime PaymentDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceID", SqlDbType.BigInt, STCInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("Payment", SqlDbType.Decimal, Payment));
            sqlParameters.Add(DBClient.AddParameters("PaymentDate", SqlDbType.DateTime, PaymentDate));
            List<Remittance> remittance = CommonDAL.ExecuteProcedure<Remittance>("STCInvoice_AddSTCAmountPaidRecord", sqlParameters.ToArray()).ToList();
            return remittance;
        }

        /// <summary>
        /// Deletes the STC amount paid record.
        /// </summary>
        /// <param name="STCInvoicePaymentID">The STC invoice payment identifier.</param>
        /// <returns></returns>
        public DataSet DeleteSTCAmountPaidRecord(long STCInvoicePaymentID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoicePaymentID", SqlDbType.BigInt, STCInvoicePaymentID));
            DataSet ds = CommonDAL.ExecuteDataSet("STCInvoice_DeleteSTCAmountPaidRecord", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// GetRemittance
        /// </summary>
        /// <param name="ResellerID"></param>
        /// <param name="STCInvoicePaymentID"></param>
        /// <param name="SolarCompanyId"></param>
        /// <returns>remittance</returns>
        public List<Remittance> GetRemittance(int ResellerID, string STCInvoicePaymentID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoicePaymentID", SqlDbType.VarChar, STCInvoicePaymentID));
            List<Remittance> remittance = CommonDAL.ExecuteProcedure<Remittance>("GetRemittanceDetailsForReport", sqlParameters.ToArray()).ToList();
            return remittance;
        }

        public DataSet GetSolarCompanyAndResellerAddress(int SolarCompanyId, int ResellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.BigInt, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.BigInt, ResellerID));
            DataSet address = CommonDAL.ExecuteDataSet("GetSolarCompanyAndResellerAddress", sqlParameters.ToArray());
            return address;
        }

        public DataSet RegenerateRemittanceFile(int resellerID, string stcInvoiceNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceNumber", SqlDbType.NVarChar, stcInvoiceNumber));
            DataSet remittance = CommonDAL.ExecuteDataSet("RegenerateRemittanceFile", sqlParameters.ToArray());
            return remittance;
        }

        /// <summary>
        /// Update GBBatchUpdateId by appending RecbulkUploadId
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <param name="RecBulkUploadId"></param>
        public void UpdateRECUploadId(string STCJobDetailsId, int RecBulkUploadId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StcJobDetailsId", SqlDbType.NVarChar, STCJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("GBBatchRECUploadId", SqlDbType.Int, RecBulkUploadId));
            CommonDAL.ExecuteScalar("UpdateGBBatchRECUploadId", sqlParameters.ToArray());
        }

        /// <summary>
        /// Update REC failed record 
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        public void UpdateRECFailedRecord(string STCJobDetailsId, bool isRecFailed = false)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.NVarChar, STCJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("IsRecFailed", SqlDbType.NVarChar, isRecFailed));
            CommonDAL.ExecuteScalar("UpdateRECFailedRecord", sqlParameters.ToArray());
        }

        public void InsertRECEntryFailureReason(int JobId, string FailureReason, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("FailureReason", SqlDbType.NVarChar, FailureReason));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("InsertRECEntryFailureReason", sqlParameters.ToArray());
        }

        public void UpdateRecSearchFailedRecord(string STCJobDetailsId, bool IsRECSearchFailed = false)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.NVarChar, STCJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("IsRECSearchFailed", SqlDbType.NVarChar, IsRECSearchFailed));
            CommonDAL.ExecuteScalar("UpdateRecSearchFailedRecord", sqlParameters.ToArray());
        }

        public DataTable GetJobsForUpdatingPvdCodeFromRec(string recUploadId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RecUploadId", SqlDbType.NVarChar, recUploadId));
            DataTable dt = CommonDAL.ExecuteDataSet("GetJobsForUpdatingPvdCodeFromRec_Queued", sqlParameters.ToArray()).Tables[0];
            return dt;
        }

        public DataTable GetJobDetailsForPVDCode(int jobID, int stcJobDetailsID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, stcJobDetailsID));
            DataTable dt = CommonDAL.ExecuteDataSet("GetJobsForPVDCode", sqlParameters.ToArray()).Tables[0];
            return dt;
        }

        public DataTable GetJobsForPVDCode()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataTable dt = CommonDAL.ExecuteDataSet("GetJobForPVDCode", sqlParameters.ToArray()).Tables[0];
            return dt;
        }


        /// <summary>
        /// get allocation records against credit note or vice versa
        /// </summary>
        /// <param name="STCInvoiceId"></param>
        /// <returns></returns>
        public DataSet GetAllocationRecords(int STCInvoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcInvoiceId", SqlDbType.Int, STCInvoiceId));
            DataSet dsAllocation = CommonDAL.ExecuteDataSet("GetCreditNoteAllocationforInvoice", sqlParameters.ToArray());
            return dsAllocation;
        }

        /// <summary>
        /// Import Bulk upload data
        /// </summary>
        /// <param name="dtImportPVDSWHCode"></param>
        /// <param name="bulkUploadId"></param>
        /// <returns></returns>
        public DataSet ImportBulkUploadData(DataTable dtImportPVDSWHCode, string bulkUploadId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("importPVDSWHCode", SqlDbType.Structured, dtImportPVDSWHCode));
            sqlParameters.Add(DBClient.AddParameters("bulkUploadId", SqlDbType.VarChar, bulkUploadId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.VarChar, ProjectSession.LoggedInUserId));
            DataSet dsUpdatedData = CommonDAL.ExecuteDataSet("ImportBulkUploadData", sqlParameters.ToArray());
            return dsUpdatedData;
        }
        /// <summary>
        /// Get log of sync with xero
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns>list of log of sync with xero</returns>
        public List<SyncWithXeroLog> GetSynxWithXeroLog(int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("resellerId", SqlDbType.Int, ResellerId));
            List<SyncWithXeroLog> syncWithXeroLogs = CommonDAL.ExecuteProcedure<SyncWithXeroLog>("GetSyncWithXerolog", sqlParameters.ToArray()).ToList();
            return syncWithXeroLogs;
        }
        /// <summary>
        /// insert gb batch invoice id
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns>list of log of sync with xero</returns>
        public void InsertGbInvoiceBatchId(DataTable dt, int resellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtStcInvoiceIds", SqlDbType.Structured, dt));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("InsertGBBatchRECUploadId", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get stc data from STCInvoiceIds
        /// </summary>
        /// <param name="STCInvoiceIDs">The STC invoice ids.</param>
        public DataSet GetStcDataFromStcInvoiceIds(string STCInvoiceIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceIDs", SqlDbType.NVarChar, STCInvoiceIDs));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStcDataFromStcInvoiceIds", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// Get Failed REC Batch Details
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public DataSet GetRECFailedBatchDetails(string batchId, string STCJobDetailsId = "", bool isFailedJob = true)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("BatchId", SqlDbType.NVarChar, batchId));
            sqlParameters.Add(DBClient.AddParameters("isFailedJob", SqlDbType.Bit, isFailedJob));
            if (!string.IsNullOrWhiteSpace(STCJobDetailsId))
                sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.NVarChar, STCJobDetailsId));
            DataSet dsRecFailureReason = CommonDAL.ExecuteDataSet("GetRecFailedBatchDetails", sqlParameters.ToArray());
            return dsRecFailureReason;
        }

        public void UpdateQueuedSubmissionStatus(string batchId, string Status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("BatchId", SqlDbType.NVarChar, batchId));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.NVarChar, Status));
            CommonDAL.Crud("QueuedRECSubmission_BulkChangeSubmissionStatus", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get job details from batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public DataSet GetJobDetailsBatchWise(string batchId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("BatchId", SqlDbType.NVarChar, batchId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobDetailsBatchWise", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// update internal issue flag in db
        /// </summary>
        /// <param name="jobIds"></param>
        /// <param name="errorMessage"></param>
        public void UpdateInternalIssue(string recBulkUploadId, string errorMessage)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("recBulkUploadId", SqlDbType.NVarChar, recBulkUploadId));
            sqlParameters.Add(DBClient.AddParameters("errorMessage", SqlDbType.NVarChar, errorMessage));
            CommonDAL.Crud("UpdateInternalIssue", sqlParameters.ToArray());
        }
        /// <summary>
        /// remove batch from queued rec submission table on exception
        /// </summary>
        /// <param name="Jobids"></param>
        public void RemoveFromQueuedRecSubmission(string Jobids)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobIds", SqlDbType.NVarChar, Jobids));
            CommonDAL.Crud("RemoveFromQueuedRecSubmission", sqlParameters.ToArray());
        }
        /// <summary>
        /// get unknown issues and internal error
        /// </summary>
        /// <param name="BulkUploadId"></param>
        /// <returns></returns>
        public DataSet GetUnknownIssues(string BulkUploadId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("BulkUploadId", SqlDbType.NVarChar, BulkUploadId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetUnknownIssues", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the jobids for create batch for automated process.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>reseller identifier.</returns>
        public DataSet GetJobIdsForBatchByResellerWise(int ResellerId, bool IsBeforeAprilInstallation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("IsBeforeAprilInstallation", SqlDbType.Bit, IsBeforeAprilInstallation));
            DataSet ds = CommonDAL.ExecuteDataSet("GetCreateBatchJobIds", sqlParameters.ToArray());
            return ds;
        }
        public string GetSTCAmountPaidId(string path)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            object stcAmountPaidId = CommonDAL.ExecuteScalar("GetSTCAmountPaidId", sqlParameters.ToArray());
            return stcAmountPaidId != null ? stcAmountPaidId.ToString() : null;
        }
    }
}

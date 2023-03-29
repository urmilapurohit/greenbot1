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
    public class SAASInvoiceBAL : ISAASInvoiceBAL
    {
        /// <summary>
        /// Gets Settlement Terms for SAAS
        /// </summary>
        /// <returns>SettlementTerms List</returns>
        public dynamic GetSAASSettlementTerms()
        {
            dynamic lstSTCInvoice;
            lstSTCInvoice = CommonDAL.ExecuteProcedure<SettlementTermSAAS>("GetSAASSettlementTerm").ToList();
            return lstSTCInvoice;
        }
        /// <summary>
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
        public DataSet GetSAASInvoiceList(int PageNumber, int pageSize, string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent, string BillingPeriod)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, !string.IsNullOrWhiteSpace(reseller) ? Convert.ToInt32(reseller) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(Owner) ? Owner : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("JobRefId", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(JobID) ? JobID : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("CreatedFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdFromDate) ? Convert.ToDateTime(createdFromDate) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("CreatedToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdToDate) ? Convert.ToDateTime(createdToDate) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("InvoiceDueFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueFromDate) ? Convert.ToDateTime(invoiceDueFromDate) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("InvoiceDueToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueToDate) ? Convert.ToDateTime(invoiceDueToDate) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("InvoiceId", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(InvoiceId) ? InvoiceId : (object)DBNull.Value));
            //sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.Int, !string.IsNullOrWhiteSpace(settlementTermId) ? Convert.ToInt32(settlementTermId) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(settlementTermId) ? settlementTermId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("BillableInterval", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(BillingPeriod) ? BillingPeriod : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, IsSent));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(SortCol) ? SortCol : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SortDir) ? SortDir : (object)DBNull.Value));
            return CommonDAL.ExecuteDataSet("GetSAASInvoiceList", sqlParameters.ToArray());
        }

        /// <summary>
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
        /// <returns>Dataset</returns>
        public DataSet GetSAASInvoiceListForExport(string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent)
        {
            DataSet ds = new DataSet();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, !string.IsNullOrWhiteSpace(reseller) ? Convert.ToInt32(reseller) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(Owner) ? Owner : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("JobRefId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(JobID) ? JobID : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("CreatedFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdFromDate) ? Convert.ToDateTime(createdFromDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("CreatedToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdToDate) ? Convert.ToDateTime(createdToDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceDueFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueFromDate) ? Convert.ToDateTime(invoiceDueFromDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceDueToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueToDate) ? Convert.ToDateTime(invoiceDueToDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(InvoiceId) ? InvoiceId : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(settlementTermId) ? (settlementTermId) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, IsSent));
                sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SortCol) ? SortCol : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SortDir) ? SortDir : (object)DBNull.Value));
                ds = CommonDAL.ExecuteDataSet("GetSAASInvoiceListForExport", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        /// <summary>
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
        /// <returns>Dataset</returns>
        public DataSet GetSAASInvoiceListForExportAll(string SortCol, string SortDir, string reseller, string InvoiceId, string createdFromDate, string createdToDate, string invoiceDueFromDate, string invoiceDueToDate, string Owner, string JobID, string settlementTermId, bool IsSent)
        {
            DataSet ds = new DataSet();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, !string.IsNullOrWhiteSpace(reseller) ? Convert.ToInt32(reseller) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(Owner) ? Owner : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("JobRefId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(JobID) ? JobID : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("CreatedFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdFromDate) ? Convert.ToDateTime(createdFromDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("CreatedToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(createdToDate) ? Convert.ToDateTime(createdToDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceDueFromDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueFromDate) ? Convert.ToDateTime(invoiceDueFromDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceDueToDate", SqlDbType.DateTime, !string.IsNullOrWhiteSpace(invoiceDueToDate) ? Convert.ToDateTime(invoiceDueToDate) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("InvoiceId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(InvoiceId) ? InvoiceId : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(settlementTermId) ? (settlementTermId) : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, IsSent));
                sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SortCol) ? SortCol : (object)DBNull.Value));
                sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SortDir) ? SortDir : (object)DBNull.Value));
                ds = CommonDAL.ExecuteDataSet("GetSAASInvoiceListForExportAll", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        /*public dynamic GetSAASInvoiceDetail(int InvoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoiceId", SqlDbType.Int, InvoiceId));
            dynamic lstSTCInvoice;
            lstSTCInvoice = CommonDAL.ExecuteProcedure<SAASInvoiceDetail>("GetSAASInvoiceDetail", sqlParameters.ToArray()).ToList();
            return lstSTCInvoice;
        }*/

        public dynamic GetSAASInvoiceDetail(string strJobID, string IsInvoiced,string InvoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("strJobID", SqlDbType.NVarChar, strJobID));
            sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, Convert.ToBoolean(IsInvoiced)));
            sqlParameters.Add(DBClient.AddParameters("InvoiceId", SqlDbType.Int, Convert.ToInt32(InvoiceId)));
            dynamic lstSTCInvoice;
            lstSTCInvoice = CommonDAL.ExecuteProcedure<SAASInvoiceDetail>("GetSAASInvoiceDetail", sqlParameters.ToArray()).ToList();
            return lstSTCInvoice;
        }

        /// <summary>
        /// get selected invoice
        /// </summary>
        /// <param name="STCInvoiceIds">stc invoice</param>
        /// <param name="resellerId">reseller identifier</param>
        /// <returns>DataSet</returns>
        public DataSet GetSelectdSTCInvoice(string SAASInvoiceIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASInvoiceIds", SqlDbType.NVarChar, SAASInvoiceIds));
            DataSet stcInvoice = CommonDAL.ExecuteDataSet("SAASInvoice_GetSelectdSAASInvoice", sqlParameters.ToArray());
            return stcInvoice;
        }

        /// <summary>
        /// Updates the xero invoice identifier.
        /// </summary>
        /// <param name="STCInvoiceJson">The STC invoice json.</param>
        /// <returns>invoice identifier</returns>
        //public int UpdateXeroInvoiceId(string STCInvoiceJson)
        public int UpdateXeroInvoiceId(int SAASInvoiceID, string SAASXeroInvoiceID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("STCInvoiceJson", SqlDbType.NVarChar, STCInvoiceJson));
            sqlParameters.Add(DBClient.AddParameters("SAASXeroInvoiceID", SqlDbType.NVarChar, SAASXeroInvoiceID));
            sqlParameters.Add(DBClient.AddParameters("SAASInvoiceID", SqlDbType.Int, SAASInvoiceID));
            object response = CommonDAL.ExecuteScalar("SAASInvoice_UpdateXeroInvoiceId", sqlParameters.ToArray());
            return Convert.ToInt32(response);
        }

        /// <summary>
        /// invoice payment
        /// </summary>
        /// <param name="SAASInvoicePaymentJson">invoice payment json</param>
        /// <param name="createdBy">created By</param>
        /// <param name="createdDate">created Date</param>
        /// <param name="resellerID">reseller ID</param>
        /// <param name="modifiedBy">modified By</param>
        /// <param name="modifiedDate">modified Date</param>
        /// <param name="UTCDate">UTC Date</param>
        /// <param name="STCInvoiceData">STC Invoice Data</param>
        /// <returns>DataSet</returns>
        public List<Remittance> InsertSAASInvoicePayment(DataTable dtPayment, int createdBy, DateTime createdDate, string invoiceNumbers, int modifiedBy, DateTime modifiedDate, DateTime UTCDate, string STCInvoiceData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtPayment", SqlDbType.Structured, dtPayment));
            sqlParameters.Add(DBClient.AddParameters("createdBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("invoiceNumbers", SqlDbType.NVarChar, invoiceNumbers));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, modifiedDate));
            sqlParameters.Add(DBClient.AddParameters("UTCDate", SqlDbType.DateTime, UTCDate));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceData", SqlDbType.NVarChar, STCInvoiceData));
            List<Remittance> remittance = CommonDAL.ExecuteProcedure<Remittance>("SAASInvoicePayment_InsertSAASInvoicePayment", sqlParameters.ToArray()).ToList();
            return remittance;
        }

        /// <summary>
        /// Imports the CSV.
        /// </summary>
        /// <param name="STCInvoicePaymentJson">The STC invoice payment json.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet ImportSAASCSV(DataTable dtCSV, int resellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCSV", SqlDbType.Structured, dtCSV));
            sqlParameters.Add(DBClient.AddParameters("createdBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            DataSet remittance = CommonDAL.ExecuteDataSet("SAASInvoice_ImportCSV", sqlParameters.ToArray());
            return remittance;
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
        /// Create SAAS Invoice
        /// </summary>
        /// <param name="dtCSV">The SAAS invoice payment table.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <returns>DataSet</returns>
        public int SAASInvoiceCreate(DataTable dtCSV, int resellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCSV", SqlDbType.Structured, dtCSV));
            sqlParameters.Add(DBClient.AddParameters("createdBy", SqlDbType.Int, resellerID));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            int InvoiceId = Convert.ToInt32(CommonDAL.ExecuteScalar("SAASInvoice_GenerateSAASInvoiceForSelectedJobs", sqlParameters.ToArray()));
            return InvoiceId;
        }

        public void DeleteSAASInvoiceByID(string invoiceIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASInvoiceIDs", SqlDbType.NVarChar, invoiceIds));
            CommonDAL.ExecuteDataSet("DeleteSAASInvoiceByID", sqlParameters.ToArray());
        }
        public int RestoreSAASInvoiceByID(int invoiceId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("invoiceId", SqlDbType.Int, invoiceId));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("RestoreSAASInvoiceByID", sqlParameters.ToArray()));
        }

        public void ImportUserTypeMenuCSV(DataTable dtCSV)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCSV", SqlDbType.Structured, dtCSV));
            CommonDAL.ExecuteDataSet("UserTypeMenu_ImportCSV", sqlParameters.ToArray());

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

        public void CreateNewInvoice(string ResellerID, string SettlementTermId, string Price, string UnitQTY, bool IsGST)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SettlementTermId) ? SettlementTermId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Price", SqlDbType.Int, !string.IsNullOrWhiteSpace(Price) ? Price : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UnitQTY", SqlDbType.Int, !string.IsNullOrWhiteSpace(UnitQTY) ? UnitQTY : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("IsGST", SqlDbType.Bit, IsGST));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteDataSet("CreateNewSAASInvoice", sqlParameters.ToArray());
        }
    }
}

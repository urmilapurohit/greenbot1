using FormBot.DAL;
using FormBot.Entity;
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
    public class SAASInvoiceBuilderBAL : ISAASInvoiceBuilderBAL
    {
        public dynamic GetSAASInvoiceBuilderDetail(string InvoiceNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoiceNumber", SqlDbType.NVarChar, InvoiceNumber));
            dynamic lstSTCInvoice;
            lstSTCInvoice = CommonDAL.ExecuteProcedure<SAASInvoiceDetail>("GetSAASInvoiceBuilderDetails", sqlParameters.ToArray()).ToList();
            return lstSTCInvoice;
        }

        public dynamic GetSAASInvoiceBuilderDetailBasedOnTerms(string strJobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("strJobID", SqlDbType.NVarChar, strJobID));
            dynamic lstSTCInvoice;
            lstSTCInvoice = CommonDAL.ExecuteProcedure<SAASInvoiceDetail>("GetSAASInvoiceBuilderDetailBasedOnTerms", sqlParameters.ToArray()).ToList();
            return lstSTCInvoice;
        }

        public DataSet GetSAASInvoiceBuilderList(int PageNumber, int pageSize, string SAASUserId, string userTypeID, string UserRole)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SAASUserId) ? Convert.ToInt32(SAASUserId) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(userTypeID) ? Convert.ToInt32(userTypeID) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UserRole", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(UserRole) ? Convert.ToString(UserRole) : (object)DBNull.Value));
            return CommonDAL.ExecuteDataSet("GetSAASInvoiceBuilderList", sqlParameters.ToArray());
        }

        public DataSet GetSAASInvoiceBuilderListBasedOnTerms(int PageNumber, int pageSize, string SAASUserId, string userTypeID, string UserRole, bool IsIsArchive)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SAASUserId) ? Convert.ToInt32(SAASUserId) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UserRole", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(UserRole) ? Convert.ToString(UserRole) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("IsIsArchive", SqlDbType.Bit, IsIsArchive));
            return CommonDAL.ExecuteDataSet("GetSAASInvoiceBuilderListBasedOnTerms", sqlParameters.ToArray());
        }

        //public void SendToSAASInvoices(string invoiceIds)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    //sqlParameters.Add(DBClient.AddParameters("SAASInvoiceBuilderIDs", SqlDbType.NVarChar, invoiceIds));
        //    sqlParameters.Add(DBClient.AddParameters("strJobID", SqlDbType.NVarChar, invoiceIds));
        //    CommonDAL.ExecuteDataSet("SendToSAASInvoices", sqlParameters.ToArray());
        //}

        public void SendToSAASInvoices(SAASInvoiceBuilder objSAASInvoiceBuilder)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("strJobID", SqlDbType.NVarChar, objSAASInvoiceBuilder.strJobID));
            sqlParameters.Add(DBClient.AddParameters("strAllJobIds", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(objSAASInvoiceBuilder.strAllJobIds) ? Convert.ToString(objSAASInvoiceBuilder.strAllJobIds) : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("InvoiceNumber", SqlDbType.NVarChar, objSAASInvoiceBuilder.InvoiceID));
            sqlParameters.Add(DBClient.AddParameters("SettelmentTerm", SqlDbType.Int, objSAASInvoiceBuilder.SettelmentTerm));
            sqlParameters.Add(DBClient.AddParameters("GlobalTermId", SqlDbType.Int, objSAASInvoiceBuilder.GlobalTermId));
            //sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, objSAASInvoiceBuilder.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("Rate", SqlDbType.Decimal, objSAASInvoiceBuilder.Rate));
            sqlParameters.Add(DBClient.AddParameters("QTY", SqlDbType.Int, objSAASInvoiceBuilder.QTY));
            sqlParameters.Add(DBClient.AddParameters("InvoiceAmount", SqlDbType.Decimal, objSAASInvoiceBuilder.InvoiceAmount));
            //sqlParameters.Add(DBClient.AddParameters("BillingPeriod", SqlDbType.NVarChar, objSAASInvoiceBuilder.strJobID));
            sqlParameters.Add(DBClient.AddParameters("BillingMonth", SqlDbType.VarChar, objSAASInvoiceBuilder.BillingMonth));
            sqlParameters.Add(DBClient.AddParameters("BillingYear", SqlDbType.VarChar, objSAASInvoiceBuilder.BillingYear));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, objSAASInvoiceBuilder.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("IsGlobalGST", SqlDbType.Bit, objSAASInvoiceBuilder.IsGlobalGST));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, objSAASInvoiceBuilder.ResellerID));
            CommonDAL.ExecuteDataSet("SendToSAASInvoices", sqlParameters.ToArray());
        }

        public dynamic GetUserTypes()
        {
            dynamic lstUserTypes;
            lstUserTypes = CommonDAL.ExecuteProcedure<UserTypesSASS>("UserType_BindDropdown").ToList();
            return lstUserTypes;
        }

        public dynamic GetUserRoles(int id)
        {
            dynamic lstUserRoles;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, id));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            lstUserRoles = CommonDAL.ExecuteDataSet("Role_BindDropDown", sqlParameters.ToArray());
            return lstUserRoles;
        }

        /// <summary>
        /// Gets the SAAS User data.
        /// </summary>
        /// <returns>reseller list</returns>
        public List<Reseller> GetSAASUsers()
        {
            string spName = "[Reseller_SAAS]";
            IList<Reseller> resellerDetailList = CommonDAL.ExecuteProcedure<Reseller>(spName);
            return resellerDetailList.ToList();
        }

        public void DeleteSAASInvoiceFomBuilderByID(string jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobid));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteDataSet("DeleteSAASInvoiceFomBuilderByID", sqlParameters.ToArray());
        }

        public void CreateNewInvoice(string UserID, string ResellerID, string ResellerName, string SettelmentTerm, string Rate, string QTY, string BillingPeriod, string GlobalTermId, string IsGlobalGST, string JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("ResellerName", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(ResellerName) ? ResellerName : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettelmentTerm", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(SettelmentTerm) ? SettelmentTerm : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Rate", SqlDbType.Decimal, !string.IsNullOrWhiteSpace(Rate) ? Rate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("QTY", SqlDbType.Int, !string.IsNullOrWhiteSpace(QTY) ? QTY : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("BillingPeriod", SqlDbType.VarChar, !string.IsNullOrWhiteSpace(BillingPeriod) ? BillingPeriod : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("GlobalTermId", SqlDbType.Int, GlobalTermId));
            sqlParameters.Add(DBClient.AddParameters("IsGlobalGST", SqlDbType.Bit, IsGlobalGST));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, !string.IsNullOrWhiteSpace(JobID) ? JobID : (object)DBNull.Value));
            CommonDAL.ExecuteDataSet("CreateNewSAASInvoice", sqlParameters.ToArray());
        }

        public dynamic GetMonthAndQTYBasedOnTerms(string SettelmentTerm, string Rate, string UserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.NVarChar, SettelmentTerm));
            sqlParameters.Add(DBClient.AddParameters("Rate", SqlDbType.Decimal, Rate));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            dynamic lstBuilderInvoiceData;
            lstBuilderInvoiceData = CommonDAL.ExecuteProcedure<SAASInvoiceDetail>("GetBuilderInvoiceDataBasedOnTerm", sqlParameters.ToArray()).ToList();
            return lstBuilderInvoiceData;
        }

        public List<SAASPricingManager> GetGlobalBillingTermsList(string UserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            List<SAASPricingManager> lstTermsList = CommonDAL.ExecuteProcedure<SAASPricingManager>("BillableTerms_BindDropDown", sqlParameters.ToArray()).ToList();
            return lstTermsList;
        }
    }
}

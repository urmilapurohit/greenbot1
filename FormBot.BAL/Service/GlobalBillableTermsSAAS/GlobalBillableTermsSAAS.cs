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

namespace FormBot.BAL.Service.GlobalBillableTermsSAAS
{
    public class GlobalBillableTermsSAAS : IGlobalBillableTermsSAAS
    {
        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="GlobalBillableTerms">Pricing Options for Settlement Terms</param>
        public List<GlobalBillableTerms> SaveGlobalBillableTermSAAS(GlobalBillableTerms GlobalBillableTerms)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.Int, GlobalBillableTerms.Id));
            sqlParameters.Add(DBClient.AddParameters("TermName", SqlDbType.NVarChar, GlobalBillableTerms.TermName));
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.NVarChar, GlobalBillableTerms.TermCode));
            sqlParameters.Add(DBClient.AddParameters("TermDescription", SqlDbType.NVarChar, GlobalBillableTerms.TermDescription));
            sqlParameters.Add(DBClient.AddParameters("GlobalPrice", SqlDbType.Decimal, GlobalBillableTerms.GlobalPrice));
            sqlParameters.Add(DBClient.AddParameters("IsGlobalGST", SqlDbType.Bit, GlobalBillableTerms.IsGlobalGST));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("BillerCode", SqlDbType.NVarChar, GlobalBillableTerms.BillerCode != "" || GlobalBillableTerms.BillerCode != null ? GlobalBillableTerms.BillerCode : (object)DBNull.Value));
            List<GlobalBillableTerms> lstGlobalPrice = CommonDAL.ExecuteProcedure<GlobalBillableTerms>("InsertUpdate_GlobalPricingTermsSAAS", sqlParameters.ToArray()).ToList();
            return lstGlobalPrice;
        }

        /// <summary>
        /// Get SAAS global pricing list.
        /// </summary>
        /// <param name="TermName"></param>
        /// <param name="BillerCode"></param>
        /// <param name="TermDescription"></param>
        public List<GlobalBillableTerms> GetGlobalPricingList(string TermName, string BillerCode, string TermDescription, string TermCode, int PageNumber, int PageSize, string sortColumn, string sortDirectoin)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TermName", SqlDbType.NVarChar, TermName));
            sqlParameters.Add(DBClient.AddParameters("BillerCode", SqlDbType.NVarChar, BillerCode != "" ? BillerCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("TermDescription", SqlDbType.NVarChar, TermDescription));
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.NVarChar, TermCode != "0" ? TermCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirectoin));
            List<GlobalBillableTerms> lstGlobalPrice = CommonDAL.ExecuteProcedure<GlobalBillableTerms>("GetGlobalBillableTermsSAAS", sqlParameters.ToArray()).ToList();
            return lstGlobalPrice;
        }

        /// <summary>
        /// Deletes billable term by id
        /// </summary>
        /// <param name="Id">Billing term id</param>
        public List<GlobalBillableTerms> DeleteBillableTermByID(int Id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.Int, Id));
            List<GlobalBillableTerms> lstGlobalBillableTerms = CommonDAL.ExecuteProcedure<GlobalBillableTerms>("DeleteBillingTermByID", sqlParameters.ToArray()).ToList();
            return lstGlobalBillableTerms;
        }

        /// <summary>
        /// Restores deleted billable term.
        /// </summary>
        /// <param name="Id">Billing term id</param>
        public List<GlobalBillableTerms> RetoreBillingTermByID(int Id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.Int, Id));
            List<GlobalBillableTerms> lstGlobalBillableTerms = CommonDAL.ExecuteProcedure<GlobalBillableTerms>("RetoreBillingTermByID", sqlParameters.ToArray()).ToList();
            return lstGlobalBillableTerms;
        }
    }
}

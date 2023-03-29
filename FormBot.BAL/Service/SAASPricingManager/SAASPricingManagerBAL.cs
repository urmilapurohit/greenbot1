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

namespace FormBot.BAL.Service.SAASPricingManagerBAL
{
    public class SAASPricingManagerBAL : ISAASPricingManagerBAL
    {
        /// <summary>
        /// Get SAAS Global Billing Terms list.
        /// </summary>
        public List<SAASPricingManager> GetGlobalBillingTermsList()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, 10000));
            List<SAASPricingManager> lstSAASPrice = CommonDAL.ExecuteProcedure<SAASPricingManager>("GetGlobalBillableTermsSAAS", sqlParameters.ToArray()).ToList();
            return lstSAASPrice;
        }

        /// <summary>
        /// Get SAAS pricing list.
        /// </summary>
        public List<GlobalBillableTerms> GetGlobalPricingList(bool IsIsArchive)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsIsArchive", SqlDbType.Bit, IsIsArchive));
            List<GlobalBillableTerms> lstGlobalPrice = CommonDAL.ExecuteProcedure<GlobalBillableTerms>("GetGlobalBillableTermsSAAS", sqlParameters.ToArray()).ToList();
            return lstGlobalPrice;
        }

        /// <summary>
        /// Get SAAS pricing list.
        /// </summary>
        public List<SAASPricingManager> GetSAASPricingList(int PageNumber, int PageSize, string sortColumn, string sortDirectoin, string RoleID, string TermCode, string UserType, string BillerCode, string TermName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleID", SqlDbType.Int, RoleID != "" ? RoleID : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.VarChar, TermCode != "" ? TermCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, UserType != "" ? UserType : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("BillerCode", SqlDbType.NVarChar, BillerCode != "" ? BillerCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("TermName", SqlDbType.NVarChar, TermName != "" ? TermName : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirectoin));
            List<SAASPricingManager> lstSAASPrice = CommonDAL.ExecuteProcedure<SAASPricingManager>("GetSAASPricingList", sqlParameters.ToArray()).ToList();
            return lstSAASPrice;
        }

        /// <summary>
        /// Gets the users role wise.
        /// </summary>
        public List<SAASPricingManager> GetRoleWiseUserSAAS(int PageNumber, int PageSize, int RoleId, string UserName, string RoleName, string TermCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, RoleId));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.VarChar, UserName));
            sqlParameters.Add(DBClient.AddParameters("RoleName", SqlDbType.VarChar, RoleName));
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.VarChar, TermCode));
            List<SAASPricingManager> lstRoleWiseUsers = CommonDAL.ExecuteProcedure<SAASPricingManager>("GetRoleWiseUserSAAS", sqlParameters.ToArray()).ToList();
            return lstRoleWiseUsers;
        }

        /// <summary>
        /// Gets all saas users list .
        /// </summary>
        public List<SAASPricingManager> GetAllRoleUserList(int PageNumber, int PageSize, string sortColumn, string sortDirectoin, string RoleID, string TermCode, string UserType, string BillableCode, string ResellerId, string SolarCompany)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, sortColumn));
            sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, sortDirectoin));
            sqlParameters.Add(DBClient.AddParameters("RoleID", SqlDbType.Int, RoleID != "" ? RoleID : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("TermCode", SqlDbType.VarChar, TermCode != "" ? TermCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, UserType != "" ? UserType : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("BillerCode", SqlDbType.NVarChar, BillableCode != "" ? BillableCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerId != "" ? ResellerId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompany != "" ? SolarCompany : (object)DBNull.Value));
            List<SAASPricingManager> lstSAASPrice = CommonDAL.ExecuteProcedure<SAASPricingManager>("GetAllRoleUserList", sqlParameters.ToArray()).ToList();
            return lstSAASPrice;
        }

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="pricingManagerSAAS">Pricing Options for Settlement Terms</param>
        public void SavePriceForSAAS(int SAASPricingId, int SAASUserId, int SettlementTermId, bool IsEnable, decimal Price, bool IsGst, int BillingPeriod, int SettlementPeriod)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASPricingId", SqlDbType.Int, SAASPricingId));
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, SAASUserId));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.Int, SettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("IsEnable", SqlDbType.Bit, IsEnable));
            sqlParameters.Add(DBClient.AddParameters("Price", SqlDbType.Decimal, Price));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, IsGst));
            sqlParameters.Add(DBClient.AddParameters("BillingPeriod", SqlDbType.Int, BillingPeriod));
            sqlParameters.Add(DBClient.AddParameters("SettlementPeriod", SqlDbType.Int, SettlementPeriod));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.Crud("InsertUpdate_PricingSAAS", sqlParameters.ToArray());
        }

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="GlobalTermId"></param>
        /// <param name="Price"></param>
        public List<SAASPricingManager> SaveUserBillableSettings(int RoleId, int GlobalTermId, decimal Price, int DATAOPMODE, int UserId, int BillableSettingsId, bool IsGST)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, RoleId));
            sqlParameters.Add(DBClient.AddParameters("GlobalTermId", SqlDbType.Int, GlobalTermId));
            sqlParameters.Add(DBClient.AddParameters("Price", SqlDbType.Decimal, Price));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, true));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("DATAOPMODE", SqlDbType.Int, DATAOPMODE));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("BillableSettingsId", SqlDbType.Int, BillableSettingsId));
            //sqlParameters.Add(DBClient.AddParameters("StrUserId", SqlDbType.NVarChar, StrUserId));
            sqlParameters.Add(DBClient.AddParameters("IsGST", SqlDbType.Bit, IsGST));
            List<SAASPricingManager> lstAffectedRecords = CommonDAL.ExecuteProcedure<SAASPricingManager>("InsertUpdate_UserBillableSettings", sqlParameters.ToArray()).ToList();
            return lstAffectedRecords;
        }

        public List<SAASPricingManager> GetBillingManagerHistoryData(string GlobalTermId, string RoleId, int UserId, int DATAOPMODE, int BillableSettingsId, string strBillableSettingsId, decimal Price)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, RoleId));
            sqlParameters.Add(DBClient.AddParameters("GlobalTermId", SqlDbType.Int, GlobalTermId));
            sqlParameters.Add(DBClient.AddParameters("DATAOPMODE", SqlDbType.Int, DATAOPMODE));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("BillableSettingsId", SqlDbType.Int, BillableSettingsId));
            sqlParameters.Add(DBClient.AddParameters("strBillableSettingsId", SqlDbType.NVarChar, strBillableSettingsId));
            sqlParameters.Add(DBClient.AddParameters("Price", SqlDbType.Decimal, Price));
            List<SAASPricingManager> lstHistoryData = CommonDAL.ExecuteProcedure<SAASPricingManager>("GetBillingManagerHistory", sqlParameters.ToArray()).ToList();
            return lstHistoryData;
        }
    }
}

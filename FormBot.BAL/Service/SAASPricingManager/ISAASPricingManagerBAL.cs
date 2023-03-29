using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ISAASPricingManagerBAL
    {
        List<SAASPricingManager> GetGlobalBillingTermsList();

        /// <summary>
        /// Get saas pricinig list.
        /// </summary>
        List<GlobalBillableTerms> GetGlobalPricingList(bool IsIsArchive);

        /// <summary>
        /// Get saas pricinig list.
        /// </summary>
        List<SAASPricingManager> GetSAASPricingList(int PageNumber, int PageSize, string sortColumn, string sortDirectoin, string RoleID, string TermCode, string UserType, string BillerCode, string TermName);
        

        /// <summary>
        /// Gets the users role wise.
        /// </summary>
        List<SAASPricingManager> GetRoleWiseUserSAAS(int PageNumber, int PageSize, int RoleId, string UserName, string RoleName, string TermCode);

        /// <summary>
        /// Gets all saas users list .
        /// </summary>
        List<SAASPricingManager> GetAllRoleUserList(int PageNumber, int PageSize, string sortColumn, string sortDirectoin, string RoleID, string TermCode, string UserType, string BillableCode, string ResellerId, string SolarCompany);

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="pricingManagerSAAS">Pricing Options for Settlement Terms</param>
        void SavePriceForSAAS(int SAASPricingId, int SAASUserId, int SettlementTermId, bool IsEnable, decimal Price, bool IsGst, int BillingPeriod, int SettlementPeriod);

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="GlobalTermId"></param>
        /// <param name="Price"></param>
        List<SAASPricingManager> SaveUserBillableSettings(int RoleId, int GlobalTermId, decimal Price, int DATAOPMODE, int UserId, int BillableSettingsId, bool IsGST);

        List<SAASPricingManager> GetBillingManagerHistoryData(string GlobalTermId, string RoleId, int UserId, int DATAOPMODE, int BillableSettingsId, string strBillableSettingsId, decimal Price);
    }
}

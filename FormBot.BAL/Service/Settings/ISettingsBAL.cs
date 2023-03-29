using FormBot.Entity.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ISettingsBAL
    {
        /// <summary>
        /// Inserts the account using synchronize xero.
        /// </summary>
        /// <param name="accountJson">The account json.</param>
        /// <param name="taxRatesJson">The tax rates json.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="totalAccountIDs">The total account i ds.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <returns>DataSet</returns>
        DataSet InsertAccountUsingSyncXero(string accountJson, string taxRatesJson, int createdBy, DateTime createdDate, int? solarCompanyId, int userTypeId, string totalAccountIDs, int modifiedBy, DateTime modifiedDate);

        /// <summary>
        /// Gets the xero account list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>xero list</returns>
        List<XeroAccount> GetXeroAccountList(int userID, int userTypeId, int pageNumber, int pageSize, string sortCol, string sortDir, int solarCompanyId);

        /// <summary>
        /// Gets the charges parts payment code and settings.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>Entity Settings</returns>
        FormBot.Entity.Settings.Settings GetChargesPartsPaymentCodeAndSettings(int userID, int userTypeId, int? solarCompanyId, int? ResellerId);

        /// <summary>
        /// Updates the logo.
        /// </summary>
        /// <param name="settingsId">The settings identifier.</param>
        /// <returns>user identifier</returns>
        int UpdateLogo(int settingsId);

        /// <summary>
        /// Inserts the update invoice settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>user identifier</returns>
        int InsertUpdateInvoiceSettings(FormBot.Entity.Settings.Settings settings);

        DataSet GetSolarCompanies();

        int ChangeReseller(int SolarCompanyId,int NewResellerId);
    }
}

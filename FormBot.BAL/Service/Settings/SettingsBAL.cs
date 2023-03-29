using FormBot.DAL;
using FormBot.Entity.Settings;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FormBot.BAL.Service
{
    public class SettingsBAL : ISettingsBAL
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
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet InsertAccountUsingSyncXero(string accountJson, string taxRatesJson, int createdBy, DateTime createdDate, int? solarCompanyId, int userTypeId, string totalAccountIDs, int modifiedBy, DateTime modifiedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("AccountJson", SqlDbType.NVarChar, accountJson));
            sqlParameters.Add(DBClient.AddParameters("TaxRatesJson", SqlDbType.NVarChar, taxRatesJson));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, modifiedDate));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("TotalAccountIDs", SqlDbType.NVarChar, totalAccountIDs));
            DataSet xeroAccount = CommonDAL.ExecuteDataSet("XeroAccount_InsertAccountUsingSyncXero", sqlParameters.ToArray());
            return xeroAccount;
        }

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
        /// <returns>
        /// xero list
        /// </returns>
        public List<XeroAccount> GetXeroAccountList(int userID, int userTypeId, int pageNumber, int pageSize, string sortCol, string sortDir, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            List<XeroAccount> lstJobParts = CommonDAL.ExecuteProcedure<XeroAccount>("XeroAccount_GetAccount", sqlParameters.ToArray()).ToList();
            return lstJobParts;
        }

        /// <summary>
        /// Gets the charges parts payment code and settings.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>
        /// Entity Settings
        /// </returns>
        public FormBot.Entity.Settings.Settings GetChargesPartsPaymentCodeAndSettings(int userID, int userTypeId, int? solarCompanyId, int? ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            DataSet dsPartsPaymentCharges = CommonDAL.ExecuteDataSet("XeroAccount_GetChargesPartsPaymentCodeAndSettings", sqlParameters.ToArray());
            FormBot.Entity.Settings.Settings settings = new Entity.Settings.Settings();
            if (dsPartsPaymentCharges != null && dsPartsPaymentCharges.Tables.Count > 0)
            {
                if (dsPartsPaymentCharges.Tables[0] != null && dsPartsPaymentCharges.Tables[0].Rows.Count > 0)
                {
                    settings = dsPartsPaymentCharges.Tables[0].ToListof<FormBot.Entity.Settings.Settings>().FirstOrDefault();
                }
                else
                {
                    settings.IsJobDescription = true;
                    settings.IsJobAddress = true;
                    settings.IsJobDate = true;
                    settings.IsTitle = true;
                    settings.IsName = true;
                    settings.IsClient = true;
                    settings.IsXeroAccount = true;
                    settings.IsTaxInclusive = true;
                    settings.PartAccountTax = 0;
                }

                settings.OldLogo = settings.Logo;

                if (dsPartsPaymentCharges.Tables[1] != null)
                {
                    settings.lstXeroPartsCodeId = dsPartsPaymentCharges.Tables[1].ToListof<GeneralClass>();
                }

                if (dsPartsPaymentCharges.Tables[1] != null)
                {
                    settings.lstXeroChargesCodeId = dsPartsPaymentCharges.Tables[1].ToListof<GeneralClass>();
                }

                if (dsPartsPaymentCharges.Tables[2] != null)
                {
                    settings.lstXeroPaymentsCodeId = dsPartsPaymentCharges.Tables[2].ToListof<GeneralClass>();
                }

                if (dsPartsPaymentCharges.Tables[3] != null && dsPartsPaymentCharges.Tables[3].Rows.Count > 0 && (userTypeId == 4 || userTypeId == 6 || userTypeId == 8))
                {
                    settings.UserId = Convert.ToInt32(dsPartsPaymentCharges.Tables[3].Rows[0]["UserId"]);
                }

                if (dsPartsPaymentCharges.Tables[4] != null)
                {
                    settings.lstXeroAccountCodeId = dsPartsPaymentCharges.Tables[4].ToListof<GeneralClass>();
                }

                if (dsPartsPaymentCharges.Tables[5] != null && dsPartsPaymentCharges.Tables[5].Rows.Count > 0 && (userTypeId == 2 || userTypeId == 5))
                {
                    settings.UserId = Convert.ToInt32(dsPartsPaymentCharges.Tables[5].Rows[0]["UserId"]);
                }
            }

            DataTable dtInvoiceDueDate = Common.GetInvoiceDueDate();
            settings.lstInvoiceDueDate = dtInvoiceDueDate.ToListof<GeneralClass>();

            return settings;
        }

        /// <summary>
        /// Inserts the update invoice settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// user identifier
        /// </returns>
        public int InsertUpdateInvoiceSettings(FormBot.Entity.Settings.Settings settings)
        {

            if (settings.IsXeroAccount)
            {
                settings.PartCode = null;
                settings.PartName = null;
                settings.PartTax = 0;
                settings.PaymentCode = null;
                settings.PaymentName = null;
                settings.PaymentTax = 0;
                settings.ChargeTax = 0;
            }
            else
            {
                settings.XeroChargeCodeId = null;
                settings.XeroAccountCodeId = null;
                settings.XeroPartsCodeId = null;
                settings.XeroPaymentsCodeId = null;
            }

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SettingsId", SqlDbType.Int, settings.SettingsId));
            sqlParameters.Add(DBClient.AddParameters("InvoiceDueDateId", SqlDbType.Int, settings.InvoiceDueDateId));
            sqlParameters.Add(DBClient.AddParameters("Logo", SqlDbType.NVarChar, settings.Logo));
            sqlParameters.Add(DBClient.AddParameters("InvoiceFooter", SqlDbType.NVarChar, settings.InvoiceFooter));
            sqlParameters.Add(DBClient.AddParameters("IsXeroAccount", SqlDbType.Bit, settings.IsXeroAccount));
            sqlParameters.Add(DBClient.AddParameters("XeroPartsCodeId", SqlDbType.Int, settings.XeroPartsCodeId));
            sqlParameters.Add(DBClient.AddParameters("XeroPaymentsCodeId", SqlDbType.Int, settings.XeroPaymentsCodeId));
            sqlParameters.Add(DBClient.AddParameters("XeroAccountCodeId", SqlDbType.Int, settings.XeroAccountCodeId));
            sqlParameters.Add(DBClient.AddParameters("XeroChargeCodeId", SqlDbType.Int, settings.XeroChargeCodeId));
            sqlParameters.Add(DBClient.AddParameters("IsTaxInclusive", SqlDbType.Bit, settings.IsTaxInclusive));
            sqlParameters.Add(DBClient.AddParameters("PartCode", SqlDbType.NVarChar, settings.PartCode));
            sqlParameters.Add(DBClient.AddParameters("PartName", SqlDbType.NVarChar, settings.PartName));
            sqlParameters.Add(DBClient.AddParameters("PartTax", SqlDbType.Decimal, settings.PartTax));
            sqlParameters.Add(DBClient.AddParameters("PaymentCode", SqlDbType.NVarChar, settings.PaymentCode));
            sqlParameters.Add(DBClient.AddParameters("PaymentName", SqlDbType.NVarChar, settings.PaymentName));
            sqlParameters.Add(DBClient.AddParameters("PaymentTax", SqlDbType.Decimal, settings.PaymentTax));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, settings.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, settings.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, settings.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, settings.ModifiedDate));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, settings.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, settings.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, settings.IsJobDescription));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, settings.IsJobAddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobDate", SqlDbType.Bit, settings.IsJobDate));
            sqlParameters.Add(DBClient.AddParameters("IsName", SqlDbType.Bit, settings.IsName));
            sqlParameters.Add(DBClient.AddParameters("IsClient", SqlDbType.Bit, settings.IsClient));
            sqlParameters.Add(DBClient.AddParameters("IsTitle", SqlDbType.Bit, settings.IsTitle));
            sqlParameters.Add(DBClient.AddParameters("TaxRate", SqlDbType.Decimal, settings.TaxRate));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, settings.ResellerId));
            sqlParameters.Add(DBClient.AddParameters("ChargeTax", SqlDbType.Decimal, settings.ChargeTax));
            object settingId = CommonDAL.ExecuteScalar("Settings_InsertUpdateInvoiceSettings", sqlParameters.ToArray());
            return Convert.ToInt32(settingId);
        }

        /// <summary>
        /// Updates the logo.
        /// </summary>
        /// <param name="settingsId">The settings identifier.</param>
        /// <returns>
        /// user identifier
        /// </returns>
        public int UpdateLogo(int settingsId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SettingsId", SqlDbType.Int, settingsId));
            object settingId = CommonDAL.ExecuteScalar("Settings_UpdateLogo", sqlParameters.ToArray());
            return Convert.ToInt32(settingId);
        }

        public DataSet GetSolarCompanies() {
            return CommonDAL.ExecuteDataSet("GetSolarCompanies");
        }

        public int ChangeReseller(int SolarCompanyId, int NewResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("NewResellerId", SqlDbType.Int, NewResellerId));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("ChangeReseller", sqlParameters.ToArray()));
        }
    }
}

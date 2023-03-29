using FormBot.DAL;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.PricingManager
{
    public class PricingManagerBAL : IPricingManagerBAL
    {
        /// <summary>
        /// Gets the sca user for pricing manager.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RAMID">The ramid.</param>
        /// <param name="SolarCompany">The solar company.</param>
        /// <param name="Name">The name.</param>
        /// <returns>
        /// price list
        /// </returns>
        public List<FormBot.Entity.PricingManager> GetSCAUserForPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, string SolarCompany, string Name)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompany", SqlDbType.NVarChar, SolarCompany));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, Name));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            List<FormBot.Entity.PricingManager> lstSCAUser = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("Users_GetSCAUserForPricingManager", sqlParameters.ToArray()).ToList();
            return lstSCAUser;
        }

        /// <summary>
        /// Gets the jobs for pricing manager.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RAMID">The ramid.</param>
        /// <param name="SystemSize">Size of the system.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="JobRef">The job reference.</param>
        /// <param name="HomeOwnerName">Name of the home owner.</param>
        /// <param name="HomeOwnerAddress">The home owner address.</param>
        /// <returns>
        /// price list
        /// </returns>
        public List<FormBot.Entity.PricingManager> GetJobsForPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID,int SystemSize, int SolarCompanyId, string JobRef, string HomeOwnerName, string HomeOwnerAddress)
        {
             List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Int, SystemSize));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobRef", SqlDbType.NVarChar, JobRef));
            sqlParameters.Add(DBClient.AddParameters("HomeOwnerName", SqlDbType.NVarChar, HomeOwnerName));
            sqlParameters.Add(DBClient.AddParameters("HomeOwnerAddress", SqlDbType.NVarChar, HomeOwnerAddress));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            List<FormBot.Entity.PricingManager> lstJobs = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("Jobs_GetJobsForPricingManager", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Saves the global price for solar company.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="Day1Price">The day1 price.</param>
        /// <param name="Day3Price">The day3 price.</param>
        /// <param name="Day7Price">The day7 price.</param>
        /// <param name="OnApprovalPrice">The on approval price.</param>
        public void SaveGlobalPriceForSolarCompany(int ResellerId, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice, 
            decimal invoiceStcPrice, int customSettlementTermId, bool UnderKW, int KWvalue, bool CommercialJob, bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,
            bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval, bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc,decimal PeakPayPrice, 
            bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value,decimal FeeValue,bool IsPeakPayPrice,bool IsCustomTimePeriod_Value,int CustomTimePeriodValue,bool IsCustomFee_Value,decimal CustomFeeValue,bool IsPeakPayGst_Value,
            decimal PeakPayGst_Value,bool IsPeakPay_CommercialJob,bool IsPeakPay_NonCommercialJob,int PeakPayStcPrice_value,bool IsCustomPeakPayGst_Value,decimal CustomPeakPayGst_Value,bool IsCustomPeakPay_CommercialJob,
            bool IsCustomPeakPay_NonCommercialJob,int CustomPeakPayStcPrice_value)
        {
            string spName = "PricingGlobal_SaveGlobalPriceForSolarCompany";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("Day1Price", SqlDbType.Decimal, Day1Price));
            sqlParameters.Add(DBClient.AddParameters("Day3Price", SqlDbType.Decimal, Day3Price));
            sqlParameters.Add(DBClient.AddParameters("Day7Price", SqlDbType.Decimal, Day7Price));
            sqlParameters.Add(DBClient.AddParameters("OnApprovalPrice", SqlDbType.Decimal, OnApprovalPrice));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("RapidPayPrice", SqlDbType.Decimal, rapidPayPrice));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));
            sqlParameters.Add(DBClient.AddParameters("CommercialPrice", SqlDbType.Decimal, commercialPrice));
            sqlParameters.Add(DBClient.AddParameters("CustomPrice", SqlDbType.Decimal, customPrice));

            sqlParameters.Add(DBClient.AddParameters("InvoiceStcPrice", SqlDbType.Decimal, invoiceStcPrice));

            sqlParameters.Add(DBClient.AddParameters("CustomSettlementTermId", SqlDbType.Int, customSettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("UnderKW", SqlDbType.Bit, UnderKW));
            sqlParameters.Add(DBClient.AddParameters("KWvalue", SqlDbType.Int, KWvalue));
            sqlParameters.Add(DBClient.AddParameters("CommercialJob", SqlDbType.Bit, CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("NonCommercialJob", SqlDbType.Bit, NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomUnderKw", SqlDbType.Bit, IsCustomUnderKw));
            sqlParameters.Add(DBClient.AddParameters("CustomKWValue", SqlDbType.Int, CustomKWValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomCommercialJob", SqlDbType.Bit, IsCustomCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomNonCommercialJob", SqlDbType.Bit, IsCustomNonCommercialJob));

            sqlParameters.Add(DBClient.AddParameters("IsPriceDay1", SqlDbType.Bit, IsPriceDay1));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay3", SqlDbType.Bit, IsPriceDay3));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay7", SqlDbType.Bit, IsPriceDay7));
            sqlParameters.Add(DBClient.AddParameters("IsPriceApproval", SqlDbType.Bit, IsPriceApproval));
            sqlParameters.Add(DBClient.AddParameters("IsPriceRapidPay", SqlDbType.Bit, IsPriceRapidPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceOptiPay", SqlDbType.Bit, IsPriceOptiPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCommercial", SqlDbType.Bit, IsPriceCommercial));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCustom", SqlDbType.Bit, IsPriceCustom));
            sqlParameters.Add(DBClient.AddParameters("IsPriceInvoiceStc", SqlDbType.Bit, IsPriceInvoiceStc));

            sqlParameters.Add(DBClient.AddParameters("PeakPayPrice", SqlDbType.Decimal, PeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsTimePeriod", SqlDbType.Bit, IsTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("TimePeriod", SqlDbType.Int, TimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsFee", SqlDbType.Bit, IsFee_Value));
            sqlParameters.Add(DBClient.AddParameters("Fee", SqlDbType.Decimal, FeeValue));
            sqlParameters.Add(DBClient.AddParameters("IsPricePeakPay", SqlDbType.Bit, IsPeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsCustomTimePeriod", SqlDbType.Bit, IsCustomTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomTimePeriod", SqlDbType.Int, CustomTimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomFee", SqlDbType.Bit, IsCustomFee_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomFee", SqlDbType.Decimal, CustomFeeValue));

            sqlParameters.Add(DBClient.AddParameters("IsPeakPayGst", SqlDbType.Bit, IsPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("PeakPayGst", SqlDbType.Decimal, PeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayCommercialJob", SqlDbType.Bit, IsPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayNonCommercialJob", SqlDbType.Bit, IsPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("PeakPayStcPrice", SqlDbType.Int, PeakPayStcPrice_value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayGst", SqlDbType.Bit, IsCustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayGst", SqlDbType.Decimal, CustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayCommercialJob", SqlDbType.Bit, IsCustomPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayNonCommercialJob", SqlDbType.Bit, IsCustomPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayStcPrice", SqlDbType.Int, CustomPeakPayStcPrice_value));

            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));

            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Saves the custom price for solar company.
        /// </summary>
        /// <param name="lstSolarCompany">The LST solar company.</param>
        /// <param name="Day1Price">The day1 price.</param>
        /// <param name="Day3Price">The day3 price.</param>
        /// <param name="Day7Price">The day7 price.</param>
        /// <param name="OnApprovalPrice">The on approval price.</param>
        /// <param name="ExpiryDate">The expiry date.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        public void SaveCustomPriceForSolarCompany(List<int> lstSolarCompany, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, DateTime ExpiryDate, int ResellerId,
            decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice, decimal invoiceStcPrice, int customSettlementTermId,bool UnderKW, int KWvalue, bool CommercialJob, 
            bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval,
            bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc, decimal PeakPayPrice,bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value, 
            decimal FeeValue, bool IsPeakPayPrice,bool IsCustomTimePeriod_Value,int CustomTimePeriodValue,bool IsCustomFee_Value,decimal CustomFeeValue,bool IsPeakPayGst_Value,decimal PeakPayGst_Value,
            bool IsPeakPay_CommercialJob,bool IsPeakPay_NonCommercialJob,int PeakPayStcPrice_value,bool IsCustomPeakPayGst_Value,decimal CustomPeakPayGst_Value,bool IsCustomPeakPay_CommercialJob,
            bool IsCustomPeakPay_NonCommercialJob,int CustomPeakPayStcPrice_value)
        {
            var iDs = lstSolarCompany.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIDs", SqlDbType.NVarChar, iDs));
            sqlParameters.Add(DBClient.AddParameters("Day1Price", SqlDbType.Decimal, Day1Price));
            sqlParameters.Add(DBClient.AddParameters("Day3Price", SqlDbType.Decimal, Day3Price));
            sqlParameters.Add(DBClient.AddParameters("Day7Price", SqlDbType.Decimal, Day7Price));
            sqlParameters.Add(DBClient.AddParameters("OnApprovalPrice", SqlDbType.Decimal, OnApprovalPrice));
            sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.DateTime, ExpiryDate));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("RapidPayPrice", SqlDbType.Decimal, rapidPayPrice));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));
            sqlParameters.Add(DBClient.AddParameters("CommercialPrice", SqlDbType.Decimal, commercialPrice));
            sqlParameters.Add(DBClient.AddParameters("CustomPrice", SqlDbType.Decimal, customPrice));

            sqlParameters.Add(DBClient.AddParameters("InvoiceStcPrice", SqlDbType.Decimal, invoiceStcPrice));

            sqlParameters.Add(DBClient.AddParameters("CustomSettlementTermId", SqlDbType.Int, customSettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("UnderKW", SqlDbType.Bit, UnderKW));
            sqlParameters.Add(DBClient.AddParameters("KWvalue", SqlDbType.Int, KWvalue));
            sqlParameters.Add(DBClient.AddParameters("CommercialJob", SqlDbType.Bit, CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("NonCommercialJob", SqlDbType.Bit, NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomUnderKw", SqlDbType.Bit, IsCustomUnderKw));
            sqlParameters.Add(DBClient.AddParameters("CustomKWValue", SqlDbType.Int, CustomKWValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomCommercialJob", SqlDbType.Bit, IsCustomCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomNonCommercialJob", SqlDbType.Bit, IsCustomNonCommercialJob));

            sqlParameters.Add(DBClient.AddParameters("IsPriceDay1", SqlDbType.Bit, IsPriceDay1));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay3", SqlDbType.Bit, IsPriceDay3));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay7", SqlDbType.Bit, IsPriceDay7));
            sqlParameters.Add(DBClient.AddParameters("IsPriceApproval", SqlDbType.Bit, IsPriceApproval));
            sqlParameters.Add(DBClient.AddParameters("IsPriceRapidPay", SqlDbType.Bit, IsPriceRapidPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceOptiPay", SqlDbType.Bit, IsPriceOptiPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCommercial", SqlDbType.Bit, IsPriceCommercial));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCustom", SqlDbType.Bit, IsPriceCustom));
            sqlParameters.Add(DBClient.AddParameters("IsPriceInvoiceStc", SqlDbType.Bit, IsPriceInvoiceStc));

            sqlParameters.Add(DBClient.AddParameters("PeakPayPrice", SqlDbType.Decimal, PeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsTimePeriod", SqlDbType.Bit, IsTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("TimePeriod", SqlDbType.Int, TimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsFee", SqlDbType.Bit, IsFee_Value));
            sqlParameters.Add(DBClient.AddParameters("Fee", SqlDbType.Decimal, FeeValue));
            sqlParameters.Add(DBClient.AddParameters("IsPricePeakPay", SqlDbType.Bit, IsPeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsCustomTimePeriod", SqlDbType.Bit, IsCustomTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomTimePeriod", SqlDbType.Int, CustomTimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomFee", SqlDbType.Bit, IsCustomFee_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomFee", SqlDbType.Decimal, CustomFeeValue));

            sqlParameters.Add(DBClient.AddParameters("IsPeakPayGst", SqlDbType.Bit, IsPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("PeakPayGst", SqlDbType.Decimal, PeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayCommercialJob", SqlDbType.Bit, IsPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayNonCommercialJob", SqlDbType.Bit, IsPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("PeakPayStcPrice", SqlDbType.Int, PeakPayStcPrice_value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayGst", SqlDbType.Bit, IsCustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayGst", SqlDbType.Decimal, CustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayCommercialJob", SqlDbType.Bit, IsCustomPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayNonCommercialJob", SqlDbType.Bit, IsCustomPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayStcPrice", SqlDbType.Int, CustomPeakPayStcPrice_value));

            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));

            CommonDAL.Crud("PricingSolarCompany_SaveCustomPriceForSolarCompany", sqlParameters.ToArray());
        }

        /// <summary>
        /// Saves the custom price for solar job.
        /// </summary>
        /// <param name="lstSolarJob">The LST solar job.</param>
        /// <param name="Day1Price">The day1 price.</param>
        /// <param name="Day3Price">The day3 price.</param>
        /// <param name="Day7Price">The day7 price.</param>
        /// <param name="OnApprovalPrice">The on approval price.</param>
        /// <param name="PartialPayment">The partial payment.</param>
        /// <param name="InitialSTC">The initial STC.</param>
        /// <param name="ExpiryDate">The expiry date.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        public void SaveCustomPriceForSolarJob(List<int> lstSolarJob, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, decimal PartialPayment, decimal InitialSTC,
            DateTime ExpiryDate, int SolarCompanyId, decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice, decimal invoiceStcPrice, int customSettlementTermId,
             bool UnderKW, int KWvalue, bool CommercialJob, bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,
             bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval, bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc, 
             bool IsPricePartialPayment,decimal PeakPayPrice,bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value,decimal FeeValue, bool IsPeakPayPrice,bool IsCustomTimePeriod_Value,
             int CustomTimePeriodValue,bool IsCustomFee_Value,decimal CustomFeeValue,bool IsPeakPayGst_Value,decimal PeakPayGst_Value,bool IsPeakPay_CommercialJob,bool IsPeakPay_NonCommercialJob,
             int PeakPayStcPrice_value,bool IsCustomPeakPayGst_Value,decimal CustomPeakPayGst_Value,bool IsCustomPeakPay_CommercialJob,bool IsCustomPeakPay_NonCommercialJob,int CustomPeakPayStcPrice_value,
             bool IsUpFrontSettelmentDay, int UpFrontSettelmentDayValue, bool IsUpFrontSettelmentDate, DateTime UpFrontSettelmentDateValue)
        {
            var iDs = lstSolarJob.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIDs", SqlDbType.NVarChar, iDs));
            sqlParameters.Add(DBClient.AddParameters("Day1Price", SqlDbType.Decimal, Day1Price));
            sqlParameters.Add(DBClient.AddParameters("Day3Price", SqlDbType.Decimal, Day3Price));
            sqlParameters.Add(DBClient.AddParameters("Day7Price", SqlDbType.Decimal, Day7Price));
            sqlParameters.Add(DBClient.AddParameters("OnApprovalPrice", SqlDbType.Decimal, OnApprovalPrice));
            sqlParameters.Add(DBClient.AddParameters("PartialPayment", SqlDbType.Decimal, PartialPayment));
            sqlParameters.Add(DBClient.AddParameters("InitialSTC", SqlDbType.Decimal, InitialSTC));
            sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.DateTime, ExpiryDate));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("RapidPayPrice", SqlDbType.Decimal, rapidPayPrice));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));
            sqlParameters.Add(DBClient.AddParameters("CommercialPrice", SqlDbType.Decimal, commercialPrice));
            sqlParameters.Add(DBClient.AddParameters("CustomPrice", SqlDbType.Decimal, customPrice));

            sqlParameters.Add(DBClient.AddParameters("InvoiceStcPrice", SqlDbType.Decimal, invoiceStcPrice));

            sqlParameters.Add(DBClient.AddParameters("CustomSettlementTermId", SqlDbType.Int, customSettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("UnderKW", SqlDbType.Bit, UnderKW));
            sqlParameters.Add(DBClient.AddParameters("KWvalue", SqlDbType.Int, KWvalue));
            sqlParameters.Add(DBClient.AddParameters("CommercialJob", SqlDbType.Bit, CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("NonCommercialJob", SqlDbType.Bit, NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomUnderKw", SqlDbType.Bit, IsCustomUnderKw));
            sqlParameters.Add(DBClient.AddParameters("CustomKWValue", SqlDbType.Int, CustomKWValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomCommercialJob", SqlDbType.Bit, IsCustomCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomNonCommercialJob", SqlDbType.Bit, IsCustomNonCommercialJob));

            sqlParameters.Add(DBClient.AddParameters("IsPriceDay1", SqlDbType.Bit, IsPriceDay1));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay3", SqlDbType.Bit, IsPriceDay3));
            sqlParameters.Add(DBClient.AddParameters("IsPriceDay7", SqlDbType.Bit, IsPriceDay7));
            sqlParameters.Add(DBClient.AddParameters("IsPriceApproval", SqlDbType.Bit, IsPriceApproval));
            sqlParameters.Add(DBClient.AddParameters("IsPriceRapidPay", SqlDbType.Bit, IsPriceRapidPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceOptiPay", SqlDbType.Bit, IsPriceOptiPay));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCommercial", SqlDbType.Bit, IsPriceCommercial));
            sqlParameters.Add(DBClient.AddParameters("IsPriceCustom", SqlDbType.Bit, IsPriceCustom));
            sqlParameters.Add(DBClient.AddParameters("IsPriceInvoiceStc", SqlDbType.Bit, IsPriceInvoiceStc));
            sqlParameters.Add(DBClient.AddParameters("IsPricePartialPayment", SqlDbType.Bit, IsPricePartialPayment));   

            sqlParameters.Add(DBClient.AddParameters("PeakPayPrice", SqlDbType.Decimal, PeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsTimePeriod", SqlDbType.Bit, IsTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("TimePeriod", SqlDbType.Int, TimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsFee", SqlDbType.Bit, IsFee_Value));
            sqlParameters.Add(DBClient.AddParameters("Fee", SqlDbType.Decimal, FeeValue));
            sqlParameters.Add(DBClient.AddParameters("IsPricePeakPay", SqlDbType.Bit, IsPeakPayPrice));
            sqlParameters.Add(DBClient.AddParameters("IsCustomTimePeriod", SqlDbType.Bit, IsCustomTimePeriod_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomTimePeriod", SqlDbType.Int, CustomTimePeriodValue));
            sqlParameters.Add(DBClient.AddParameters("IsCustomFee", SqlDbType.Bit, IsCustomFee_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomFee", SqlDbType.Decimal, CustomFeeValue));

            sqlParameters.Add(DBClient.AddParameters("IsPeakPayGst", SqlDbType.Bit, IsPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("PeakPayGst", SqlDbType.Decimal, PeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayCommercialJob", SqlDbType.Bit, IsPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPayNonCommercialJob", SqlDbType.Bit, IsPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("PeakPayStcPrice", SqlDbType.Int, PeakPayStcPrice_value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayGst", SqlDbType.Bit, IsCustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayGst", SqlDbType.Decimal, CustomPeakPayGst_Value));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayCommercialJob", SqlDbType.Bit, IsCustomPeakPay_CommercialJob));
            sqlParameters.Add(DBClient.AddParameters("IsCustomPeakPayNonCommercialJob", SqlDbType.Bit, IsCustomPeakPay_NonCommercialJob));
            sqlParameters.Add(DBClient.AddParameters("CustomPeakPayStcPrice", SqlDbType.Int, CustomPeakPayStcPrice_value));

            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));

            sqlParameters.Add(DBClient.AddParameters("isUpFrontSettelmentDay", SqlDbType.Bit, IsUpFrontSettelmentDay));
            sqlParameters.Add(DBClient.AddParameters("UpFrontSettelmentDay", SqlDbType.Int, UpFrontSettelmentDayValue));
            sqlParameters.Add(DBClient.AddParameters("isUpFrontSettelmentDate", SqlDbType.Bit, IsUpFrontSettelmentDate));
            sqlParameters.Add(DBClient.AddParameters("UpFrontSettelmentDate", SqlDbType.DateTime, UpFrontSettelmentDateValue));

            CommonDAL.Crud("PricingJob_SaveCustomPriceForSolarJob", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the global price for reseller.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>
        /// price object
        /// </returns>
        public FormBot.Entity.PricingManager GetGlobalPriceForReseller(int ResellerId)
        {
            FormBot.Entity.PricingManager pricingManager = CommonDAL.SelectObject<FormBot.Entity.PricingManager>("PricingGlobal_GetGlobalPriceForReseller", DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            return pricingManager;
        }

        /// <summary>
        /// Gets the global price for SAAS.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>
        /// price object
        /// </returns>
        public List<FormBot.Entity.PricingManagerSAAS> GetGlobalPriceForSAAS(int SAASUserId, bool IsEnabled)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, SAASUserId));
            sqlParameters.Add(DBClient.AddParameters("IsEnabled", SqlDbType.Bit, IsEnabled));
            DataSet ds = CommonDAL.ExecuteDataSet("PricingGlobal_GetGlobalPriceForSAAS", sqlParameters.ToArray());
            List<FormBot.Entity.PricingManagerSAAS> pricingManager = DBClient.DataTableToList<FormBot.Entity.PricingManagerSAAS>(ds.Tables[0]);
            return pricingManager;
        }

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="pricingManagerSAAS">Pricing Options for Settlement Terms</param>
        public void SavePriceForSAAS(Entity.PricingManagerSAAS pricingManagerSAAS)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASPricingId", SqlDbType.Int, pricingManagerSAAS.SAASPricingId));
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, pricingManagerSAAS.SAASUserId));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.Int, pricingManagerSAAS.SettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("IsEnable", SqlDbType.Bit, pricingManagerSAAS.IsEnable));
            sqlParameters.Add(DBClient.AddParameters("Price", SqlDbType.Decimal, pricingManagerSAAS.Price));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, pricingManagerSAAS.IsGst));
            sqlParameters.Add(DBClient.AddParameters("BillingPeriod", SqlDbType.Int, pricingManagerSAAS.BillingPeriod));
            sqlParameters.Add(DBClient.AddParameters("SettlementPeriod", SqlDbType.Int, pricingManagerSAAS.SettlementPeriod));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.Crud("InsertUpdate_PricingSAAS", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the GST.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Status">if set to <c>true</c> [status].</param>
        public void UpdateGST(int JobId,bool Status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, Status));
            CommonDAL.Crud("JobDetails_UpdateGST", sqlParameters.ToArray());
        }
    }
}

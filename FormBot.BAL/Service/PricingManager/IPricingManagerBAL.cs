using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface IPricingManagerBAL
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
        /// <returns>price list</returns>
        List<FormBot.Entity.PricingManager> GetSCAUserForPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, string SolarCompany, string Name);

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
        /// <returns>price list</returns>
        List<FormBot.Entity.PricingManager> GetJobsForPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, int SystemSize, int SolarCompanyId, string JobRef, string HomeOwnerName, string HomeOwnerAddress);

        /// <summary>
        /// Saves the global price for solar company.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="Day1Price">The day1 price.</param>
        /// <param name="Day3Price">The day3 price.</param>
        /// <param name="Day7Price">The day7 price.</param>
        /// <param name="OnApprovalPrice">The on approval price.</param>
        void SaveGlobalPriceForSolarCompany(int ResellerId, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice,
            decimal invoiceStcPrice, int customSettlementTermId, bool UnderKW, int KWvalue, bool CommercialJob, bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,
            bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval, bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc, decimal PeakPayPrice,
            bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value, decimal FeeValue, bool IsPeakPayPrice, bool IsCustomTimePeriod_Value, int CustomTimePeriodValue, bool IsCustomFee_Value, decimal CustomFeeValue, bool IsPeakPayGst_Value,
            decimal PeakPayGst_Value, bool IsPeakPay_CommercialJob, bool IsPeakPay_NonCommercialJob, int PeakPayStcPrice_value, bool IsCustomPeakPayGst_Value, decimal CustomPeakPayGst_Value, bool IsCustomPeakPay_CommercialJob,
            bool IsCustomPeakPay_NonCommercialJob, int CustomPeakPayStcPrice_value);

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
        void SaveCustomPriceForSolarCompany(List<int> lstSolarCompany, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, DateTime ExpiryDate, int ResellerId,
            decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice, decimal invoiceStcPrice, int customSettlementTermId,bool UnderKW, int KWvalue, bool CommercialJob, 
            bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval,
            bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc, decimal PeakPayPrice,bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value, 
            decimal FeeValue, bool IsPeakPayPrice,bool IsCustomTimePeriod_Value,int CustomTimePeriodValue,bool IsCustomFee_Value,decimal CustomFeeValue,bool IsPeakPayGst_Value,decimal PeakPayGst_Value,
            bool IsPeakPay_CommercialJob,bool IsPeakPay_NonCommercialJob,int PeakPayStcPrice_value,bool IsCustomPeakPayGst_Value,decimal CustomPeakPayGst_Value,bool IsCustomPeakPay_CommercialJob,
            bool IsCustomPeakPay_NonCommercialJob,int CustomPeakPayStcPrice_value);

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
        void SaveCustomPriceForSolarJob(List<int> lstSolarJob, decimal Day1Price, decimal Day3Price, decimal Day7Price, decimal OnApprovalPrice, decimal PartialPayment, decimal InitialSTC,
             DateTime ExpiryDate, int SolarCompanyId, decimal rapidPayPrice, decimal optiPayPrice, decimal commercialPrice, decimal customPrice, decimal invoiceStcPrice, int customSettlementTermId,
              bool UnderKW, int KWvalue, bool CommercialJob, bool NonCommercialJob, bool IsCustomUnderKw, int CustomKWValue, bool IsCustomCommercialJob, bool IsCustomNonCommercialJob,
              bool IsPriceDay1, bool IsPriceDay3, bool IsPriceDay7, bool IsPriceApproval, bool IsPriceRapidPay, bool IsPriceOptiPay, bool IsPriceCommercial, bool IsPriceCustom, bool IsPriceInvoiceStc,
              bool IsPricePartialPayment, decimal PeakPayPrice, bool IsTimePeriod_Value, int TimePeriodValue, bool IsFee_Value, decimal FeeValue, bool IsPeakPayPrice, bool IsCustomTimePeriod_Value,
              int CustomTimePeriodValue, bool IsCustomFee_Value, decimal CustomFeeValue, bool IsPeakPayGst_Value, decimal PeakPayGst_Value, bool IsPeakPay_CommercialJob, bool IsPeakPay_NonCommercialJob,
              int PeakPayStcPrice_value, bool IsCustomPeakPayGst_Value, decimal CustomPeakPayGst_Value, bool IsCustomPeakPay_CommercialJob, bool IsCustomPeakPay_NonCommercialJob, int CustomPeakPayStcPrice_value,
              bool IsUpFrontSettelmentDay, int UpFrontSettelmentDayValue, bool IsUpFrontSettelmentDate, DateTime UpFrontSettelmentDateValue);

        /// <summary>
        /// Gets the global price for reseller.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>price object</returns>
        FormBot.Entity.PricingManager GetGlobalPriceForReseller(int ResellerId);

        /// <summary>
        /// Gets the global price for SAAS User.
        /// </summary>
        /// <param name="SAASUserId">The reseller identifier.</param>
        /// <returns>price object</returns>
        List<FormBot.Entity.PricingManagerSAAS> GetGlobalPriceForSAAS(int SAASUserId, bool IsEnabled);

        /// <summary>
        /// Sets the Global price for SAAS User.
        /// </summary>
        /// <param name="pricingManagerSAAS">Pricing Options for Settlement Terms</param>
        void SavePriceForSAAS(FormBot.Entity.PricingManagerSAAS pricingManagerSAAS);

        /// <summary>
        /// Updates the GST.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Status">if set to <c>true</c> [status].</param>
        void UpdateGST(int JobId, bool Status);
    }
}

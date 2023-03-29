using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FormBot.Helper;
using FormBot.Entity.Email;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class PricingManager
    {
        ////public int UserId { get; set; }

        public int ID { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        public string Name { get; set; }

        public string AccountManager { get; set; }

        [DisplayName("User Name:")]
        [NotMapped]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [DisplayName("User Type:")]
        public int UserTypeID { get; set; }

        [DisplayName("Company/Business Name:")]
        [Required(ErrorMessage = "Company/Business Name is required.")]
        public string CompanyName { get; set; }

        [DisplayName("Company Website:")]
        public string CompanyWebsite { get; set; }

        [DisplayName("Solar Company/Sub Contractor Name:")]
        [Required(ErrorMessage = "Solar Company is required.")]
        public int? SolarCompanyId { get; set; }

        [NotMapped]
        [DisplayName("Solar Company")]
        public string SolarCompany { get; set; }

        [DisplayName("Reseller Name:")]
        [Required(ErrorMessage = "Reseller Name is required.")]
        public int? ResellerID { get; set; }

        public int? RAMID { get; set; }

        public decimal TradedSTCs { get; set; }

        public decimal InProgressSTCs { get; set; }

        public decimal LastTradedPrice { get; set; }

        [Required(ErrorMessage = "24 Hour price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? Hour24 { get; set; }

        [Required(ErrorMessage = "UpFront price is required")]
        [Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? UpFront { get; set; }

        [Required(ErrorMessage = "3 Days price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? Days3 { get; set; }

        [Required(ErrorMessage = "7 Days price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? Days7 { get; set; }

        [Required(ErrorMessage = "On Approval price is required")]
        [Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? CERApproved { get; set; }

        [Required(ErrorMessage = "Partial Payment price is required")]
        [Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? PartialPayment { get; set; }

        [Required(ErrorMessage = "Initial STC is required")]
        [Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? InitialSTC { get; set; }

        [Required(ErrorMessage = "Rapid-Pay price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        //[DisplayName("Rapid-Pay:")]
        public decimal? RapidPay { get; set; }

        [Required(ErrorMessage = "Opti-Pay price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        //[DisplayName("Opti-Pay:")]
        public decimal? OptiPay { get; set; }

        [Required(ErrorMessage = "Commercial price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        //[DisplayName("Commercial:")]
        public decimal? Commercial { get; set; }

        [Required(ErrorMessage = "Custom price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        //[DisplayName("Custom:")]
        public decimal? Custom { get; set; }

        [Required(ErrorMessage = "Invoice Stc price is required")]
        [Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        [DisplayName("Invoice Stc")]
        public decimal? InvoiceStc { get; set; }


        //[Required(ErrorMessage = "Peak Pay price is required")]       
        public decimal? PeakPay { get; set; }

        public bool IsGst { get; set; }

        [Required(ErrorMessage = "Expiry date required")]
        public DateTime? OfferExpires { get; set; }

        public string strOfferExpires
        {
            get
            {
                return OfferExpires != null ? OfferExpires.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public List<SelectListItem> lstLeftSide { get; set; }

        public string[] LeftSide { get; set; }

        public List<SelectListItem> lstRightSide { get; set; }

        public string[] RightSide { get; set; }

        public int PricingType { get; set; }

        public int PricingMode { get; set; }

        public int SystemSize { get; set; }

        public decimal JobSystemSize { get; set; }

        public string RefNumber { get; set; }

        public string HomeOwnerName { get; set; }

        public string HomeOwnerAddress { get; set; }

        public decimal STCAmount { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool HaveNotCustomPrice { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.ID));
            }
        }

        public int IsSubmissionScreen { get; set; }

        public Dictionary<int, string> SettlementTermList { get; set; }

        public bool IsTradedFromJobIndex { get; set; }

        [DisplayName("WholeSaler Name:")]
        public int? WholeSalerID { get; set; }

        #region For Trade STC PopUp
        public string RefNumberOwnerName { get; set; }
        public DateTime? InstallationDate { get; set; }
        public decimal STC { get; set; }
        public int JobID { get; set; }
        public string jobIDS { get; set; }
        public bool IsGridView { get; set; }
        public bool IsShowInDashboard { get; set; }

        public int STCStatus { get; set; }
        public decimal STCPrice { get; set; }
        public int STCSettlementTerm { get; set; }
        public int STCJobDetailsID { get; set; }
        public string Status { get; set; }
        public bool IsInvoiced { get; set; }
        public string LastUpdated { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public int ApprovedJobsCount { get; set; }
        public int PendingJobsCount { get; set; }
        public decimal TotalSTCCount { get; set; }
        public decimal? TaxRate { get; set; }

        public int? CustomSettlementTerm { get; set; }

        public string CustomTermText { get; set; }
        public string CustomSubDescription { get; set; }

        public bool? UnderKW { get; set; }
        public int? KWValue { get; set; }
        public bool? CommercialJob { get; set; }
        public bool? NonCommercialJob { get; set; }

        public bool? IsCustomUnderKW { get; set; }
        public int? CustomKWValue { get; set; }
        public bool? IsCustomCommercialJob { get; set; }
        public bool? IsCustomNonCommercialJob { get; set; }

        public bool? IsTimePeriod { get; set; }
        public int? TimePeriod { get; set; }
        public bool? IsFee { get; set; }
        public decimal? Fee { get; set; }
        public bool? IsPeakPayGst { get; set; }
        public decimal? PeakPayGst { get; set; }
        public bool? IsPeakPayCommercialJob { get; set; }
        public bool? IsPeakPayNonCommercialJob { get; set; }
        public int PeakPayStcPrice { get; set; }

        public bool? IsCustomTimePeriod { get; set; }
        public int? CustomTimePeriod { get; set; }
        public bool? IsCustomFee { get; set; }
        public decimal? CustomFee { get; set; }
        public bool? IsCustomPeakPayGst { get; set; }
        public decimal? CustomPeakPayGst { get; set; }
        public bool? IsCustomPeakPayCommercialJob { get; set; }
        public bool? IsCustomPeakPayNonCommercialJob { get; set; }
        public int CustomPeakPayStcPrice { get; set; }

        public bool IsPriceDay1 { get; set; }
        public bool IsPriceDay3 { get; set; }
        public bool IsPriceDay7 { get; set; }
        public bool IsPriceApproval { get; set; }
        public bool IsPriceRapidPay { get; set; }
        public bool IsPriceOptiPay { get; set; }
        public bool IsPriceCommercial { get; set; }
        public bool IsPriceCustom { get; set; }
        public bool IsPriceInvoiceStc { get; set; }
        public bool IsPricePartialPayment { get; set; }
        public bool IsPricePeakPay { get; set; }
        
        public bool IsApproachingExpiryDate { get; set; }

        // For sca dashboard STC trade
        public bool IsTraded { get; set; }
        public bool IsReadyToTrade { get; set; }
        public decimal? PriceDay1 { get; set; }
        public decimal? PriceDay3 { get; set; }
        public decimal? PriceDay7 { get; set; }
        public decimal? PriceOnApproval { get; set; }
        public bool IsCustomPrice { get; set; }
        public string PropertyType { get; set; }
        public string GSTDocument { get; set; }
        public int IsGstSetByAdminUser { get; set; }
        public bool IsWholeSaler { get; set; }
        public bool IsSAASUser { get; set; }
        public int SAASUserId { get; set; }
        public bool IsDashboardPricing { get; set; }
        public string OwnerType { get; set; }
        public decimal? ModifiedSTCAmount { get; set; }
        public bool IsPriceDay1Up { get; set; }
        public bool IsPriceDay3Up { get; set; }
        public bool IsPriceDay7Up { get; set; }
        public bool IsPriceApprovalUp { get; set; }
        public bool IsPriceRapidPayUp { get; set; }
        public bool IsPriceOptiPayUp { get; set; }
        public bool IsPriceCommercialUp { get; set; }
        public bool IsPriceCustomUp { get; set; }
        public bool IsPriceInvoiceStcUp { get; set; }
        public bool IsPricePricePeakPayUp { get; set; }
        public bool IsForDashboardPricingWholesaler { get; set; }
        public List<PricingManagerSAAS> PricingManagerSAAS { get; set; }
        #endregion;

        [Required(ErrorMessage = "24 Hour price is required")]
        //[Range(0.01, 99999.99, ErrorMessage = "The value must be greater than 0")]
        public decimal? SAASSTCAmount { get; set; }
        public int RoleID { get; set; }
        public bool isUpFrontSettelmentDay { get; set; }
        public bool isUpFrontSettelmentDate { get; set; }
        public int UpFrontSettelmentDay { get; set; }
        public DateTime UpFrontSettelmentDate { get; set; }
    }

    public class PricingManagerSAAS
    {
        public int SAASPricingId { get; set; }
        public int SAASUserId { get; set; }
        public string SettlementTerms { get; set; }
        public int SettlementTermId { get; set; }
        public decimal Price { get; set; }
        public bool IsEnable { get; set; }
        public bool IsGst { get; set; }
        public int BillingPeriod { get; set; }
        public int SettlementPeriod { get; set; }

    }
}


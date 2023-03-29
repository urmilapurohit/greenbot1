using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class VEECPricingManager
    {
        [DisplayName("User Type:")]
        public int UserTypeID { get; set; }

        [DisplayName("Reseller Name:")]
        [Required(ErrorMessage = "Reseller Name is required.")]
        public int? ResellerID { get; set; }

        public int? RAMID { get; set; }

        [Required(ErrorMessage = "Opti-Pay price is required")]
        public decimal? OptiPay { get; set; }

        public bool? IsUnderKW { get; set; }

        public int? KWValue { get; set; }

        public bool? IsCommercialJob { get; set; }

        public bool? IsNonCommercialJob { get; set; }

        public bool IsPriceOptiPay { get; set; }

        public int PricingType { get; set; }    // 1:Global Price  2:Custom Price

        public int PricingMode { get; set; }    //1:SolarCompany    2:SolarJob

        public List<SelectListItem> lstLeftSide { get; set; }

        public string[] LeftSide { get; set; }

        public List<SelectListItem> lstRightSide { get; set; }

        public string[] RightSide { get; set; }

        public int SystemSize { get; set; }

        public Dictionary<int, string> SettlementTermList { get; set; }

        [Required(ErrorMessage = "Expiry date required")]
        public DateTime? OfferExpires { get; set; }

        public string strOfferExpires
        {
            get
            {
                return OfferExpires != null ? OfferExpires.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public int TotalRecords { get; set; }

        public int? SolarCompanyId { get; set; }

        [DisplayName("Solar Company")]
        public string SolarCompany { get; set; }

        [DisplayName("Company/Business Name:")]
        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string AccountManager { get; set; }

        public int ID { get; set; }

        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.ID));
            }
        }

        public string RefNumber { get; set; }

        public string HomeOwnerName { get; set; }

        public string HomeOwnerAddress { get; set; }

        public decimal LastTradedPrice { get; set; }

        public int? CustomSettlementTerm { get; set; }

        public string CustomTermText { get; set; }

        public string CustomSubDescription { get; set; }

    }
}

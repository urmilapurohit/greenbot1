using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Settings
{
    public class Settings
    {
        public int SettingsId { get; set; }

        [Display(Name = "Invoice Due Date:")]
        [Required(ErrorMessage = "Invoice due date is required.")]
        public int InvoiceDueDateId { get; set; }

        [NotMapped]
        public List<GeneralClass> lstInvoiceDueDate { get; set; }

        [Display(Name = "Logo:")]
        public string Logo { get; set; }

        [Display(Name = "Invoice Footer:")]
        public string InvoiceFooter { get; set; }

        [NotMapped]
        public string type { get; set; }

        public bool IsXeroAccount { get; set; }

        [Display(Name = "Parts Code:")]
        [Required(ErrorMessage = "Parts code is required.")]
        public int? XeroPartsCodeId { get; set; }

        [NotMapped]
        public List<GeneralClass> lstXeroPartsCodeId { get; set; }

        [Display(Name = "Payments Code:")]
        [Required(ErrorMessage = "Payments code is required.")]
        public int? XeroPaymentsCodeId { get; set; }

        [NotMapped]
        public List<GeneralClass> lstXeroPaymentsCodeId { get; set; }

        [Display(Name = "Charges Code:")]
        [Required(ErrorMessage = "Charges code is required.")]
        public int? XeroChargeCodeId { get; set; }

        [NotMapped]
        public List<GeneralClass> lstXeroChargesCodeId { get; set; }

        [Display(Name = "Account Code:")]
        [Required(ErrorMessage = "Account code is required.")]
        public int? XeroAccountCodeId { get; set; }

        [NotMapped]
        public List<GeneralClass> lstXeroAccountCodeId { get; set; }

        [Display(Name = "Tax Inclusive:")]
        [Required]
        public bool IsTaxInclusive { get; set; }

        [Display(Name = "Tax Rate(%):")]
        ////[Required(ErrorMessage = "Tax rate is required.")]
        public decimal? TaxRate { get; set; }

        [Display(Name = "Part Code:")]
        [Required(ErrorMessage = "Part code is required.")]
        public string PartCode { get; set; }

        [Display(Name = "Part Name:")]
        [Required(ErrorMessage = "Part name is required.")]
        public string PartName { get; set; }

        [Display(Name = "Part Tax:")]
        [Required(ErrorMessage = "Part tax is required.")]
        public decimal PartTax { get; set; }

        [Display(Name = "Payment Code:")]
        [Required(ErrorMessage = "Payment code is required.")]
        public string PaymentCode { get; set; }

        [Display(Name = "Payment Name:")]
        [Required(ErrorMessage = "Payment name is required.")]
        public string PaymentName { get; set; }

        [Display(Name = "Payment Tax:")]
        [Required(ErrorMessage = "Payment tax is required.")]
        public decimal PaymentTax { get; set; }

        [Display(Name = "Charge Tax:")]
        [Required(ErrorMessage = "Charge tax is required.")]
        public decimal ChargeTax { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public int UserId { get; set; }

        public int? SolarCompanyId { get; set; }

        [Display(Name = "Add Job Description to Invoice:")]
        public bool IsJobDescription { get; set; }

        [Display(Name = "Add Job Address to Invoice:")]
        public bool IsJobAddress { get; set; }

        [Display(Name = "Add Job Date to Invoice:")]
        public bool IsJobDate { get; set; }

        [Display(Name = "Add Job Title to Invoice:")]
        public bool IsName { get; set; }

        [Display(Name = "Add Name/Code to Charge Description:")]
        public bool IsClient { get; set; }

        [Display(Name = "Add Client Name to Invoice:")]
        public bool IsTitle { get; set; }

        [NotMapped]
        public int SyncValue { get; set; }

        [NotMapped]
        public string OldLogo { get; set; }

        public int? ResellerId { get; set; }

        public string Signature { get; set; }

        public decimal? PartAccountTax { get; set; }

    }

    public class GeneralClass
    {
        public string Name { get; set; }
        public int? Id { get; set; }
        public string Code { get; set; }
        public decimal Tax { get; set; }
        public string Status { get; set; }
        public string TaxType { get; set; }
    }
}

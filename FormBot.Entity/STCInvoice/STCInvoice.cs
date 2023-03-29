using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    [Serializable]
    public class STCInvoice
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public int? ResellerID { get; set; }
        public int? SolarCompanyId { get; set; }
        public List<JobStage> lstSTCInvoiceStages { get; set; }

        public Int64 STCInvoiceID { get; set; }
        public string STCInvoiceNumber { get; set; }
        public int JobID { get; set; }
        public int STCJobDetailsID { get; set; }
        public int PaymentStatusID { get; set; }
        public string PaymentStatusName { get; set; }
        public string STC { get; set; }
        public string SolarCompany { get; set; }
        public string SettlementTerms { get; set; }
        public string SettlementTermDescription { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string strSubmissionDate
        {
            get
            {
                return SubmissionDate != null ? SubmissionDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? SettlementDate { get; set; }
        public string strSettlementDate
        {
            get
            {
                return SettlementDate != null ? SettlementDate.Value.ToString("dd/MM/yyyy") : null;
            }

        }

        public bool IsInvoice { get; set; }
        public string InvoiceFilePath { get; set; }
        public string RemittenceFilePath { get; set; }
        public decimal Total { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string RefNumberOwnerName { get; set; }
        public string InstallationAddress { get; set; }
        public bool IsGst { get; set; }

        [Required(ErrorMessage = "STC Amount is required")]
        public decimal STCAmount { get; set; }
        [Required(ErrorMessage = "STC Value is required")]
        public decimal STCValue { get; set; }
        public int STCQty { get; set; }
        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.STCInvoiceID));
            }
        }
        [NotMapped]
        public string Job_Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }
        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public decimal? TotalAmount { get; set; }

        public string Notes { get; set; }

        public decimal? TaxRate { get; set; }
        public string STCPVDCode { get; set; }

        public int? RAMID { get; set; }
        public decimal? AmountPaid { get; set; }

        [NotMapped]
        public string FilePaths { get; set; }
        [NotMapped]
        public List<string> FileNamesCreate { get; set; }
	    public string XeroInvoiceId { get; set; }
        public bool IsCreditNote { get; set; }
        public string strAmountPaid { get; set; }

        public decimal CalculatedSTC { get; set; }

        public Dictionary<int, string> DictSettlementTerm { get; set; }

        public int? CustomSettlementTerm { get; set; }
        public string CustomTermLabel { get; set; }
        public string PeakPayInfo { get; set; }

        public string AccountManager { get; set; }

        public string AdjustmentNotes { get; set; }
        public string UserName { get; set; }
        public int InoviceTermID { get; set; }
    }

    public class STCInvoiceDetail
    {
        public Int64 Id { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public int IsTax { get; set; }
        public decimal? TaxRate { get; set; }
        public string STCPVDCode { get; set; }
        public bool IsCreditNote { get; set; }
    }

    public class STCInvoiceReport
    {
        public string RefNumber { get; set; }
        public string CompanyABN { get; set; }
        public string CompanyABNReseller { get; set; }
        public string ResellerLogo { get; set; }
        public string InvoiceFooter { get; set; }
        public string AccountNumber { get; set; }
        public string BSB { get; set; }
        public string AccountName { get; set; }
        public string AccountNumberReseller { get; set; }
        public string AccountNameReseller { get; set; }
        public string BSBReseller { get; set; }
        public bool IsWholeSaler { get; set; }
        public int ResellerID { get; set; }
        public string SolarCompanyName { get; set; }
        public int JobID { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Sale { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool IsTaxInclusive { get; set; }
        public string ToAddressLine1 { get; set; }
        public string ToAddressLine2 { get; set; }
        public string ToAddressLine3 { get; set; }
        public string fromAddressLine1 { get; set; }
        public string fromAddressLine2 { get; set; }
        public string fromAddressLine3 { get; set; }
        public string OwnerName { get; set; }
        public string ToOwnerCompanyName { get; set; }
        public string ToCompanyName { get; set; }
        public string ResellerName { get; set; }
        public string STCInvoiceNumber { get; set; }
        public string JobDescription { get; set; }
        public string JobAddress { get; set; }
        public string JobTitle { get; set; }
        public DateTime JobDate { get; set; }
        public int SettingUserId{ get; set; }
        public decimal Tax { get; set; }
        public string FromCompanyName { get; set; }
        public string STCPVDCode { get; set; }
        public bool IsPeakPay { get; set; }
        public decimal PeakPayFee { get; set; }
        public decimal PeakPayGst { get; set; }

    }
    public class SyncWithXeroLog
    {
        public int SyncXeroLogID { get; set; }
        public int ResellerID { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string strModifiedDate { get; set; }
        public string ResellerName { get; set; }
        public string UserName { get; set; }

    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.Entity.Job
{
    [Serializable]
    public class JobInvoiceDetail
    {
        public int JobInvoiceDetailID { get; set; }

        public int? JobInvoiceID { get; set; }

        public int? JobPartID { get; set; }

        [Display(Name = "Billable:")]
        public bool IsBillable { get; set; }

        [Display(Name = "Job Visit:")]
        public int JobScheduleID { get; set; }

        public int Staff { get; set; }

        [Display(Name = "Sale Price:")]
        ////[Required(ErrorMessage = "Sale Price is required.")]
        public decimal? Sale { get; set; }

        [Display(Name = "Time Added:")]
        [Required(ErrorMessage = "Time is required.")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Item Code:")]
        [Required(ErrorMessage = "Item Code is required.")]
        public string ItemCode { get; set; }

        ////[Required(ErrorMessage = "Quantity is required.")]
        [Display(Name = "Quantity:")]
        public decimal? Quantity { get; set; }

        [Display(Name = "Purchase Cost:")]
        public decimal? Purchase { get; set; }

        [Display(Name = "Margin(%):")]
        public decimal? Margin { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description:")]
        public string Description { get; set; }

        [Display(Name = "File:")]
        public string FileName { get; set; }

        [Display(Name = "Time End:")]
        public DateTime? TimeEnd { get; set; }

        [Display(Name = "Payment Type:")]
        [Required(ErrorMessage = "Payment Type is required.")]
        public byte? PaymentType { get; set; }

        [Display(Name = "Amount:")]
        [Required(ErrorMessage = "Amount is required.")]
        public decimal? PaymentAmount { get; set; }

        public byte JobInvoiceType { get; set; }

        [Display(Name = "Invoiced:")]
        public bool IsInvoiced { get; set; }

        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public int? ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string JobScheduleLabel { get; set; }

        [NotMapped]
        [Display(Name = "Staff:")]
        public string StaffName { get; set; }

        [NotMapped]
        public string Created { get; set; }

        [NotMapped]
        public string Modified { get; set; }

        [NotMapped]
        public decimal? Profit { get; set; }

        [NotMapped]
        public decimal? Tax { get; set; }

        [NotMapped]
        public decimal? TaxAmountConsider { get; set; }

        [NotMapped]
        public string TaxType { get; set; }

        [NotMapped]
        public bool IsTaxInclusive { get; set; }

        [NotMapped]
        public decimal? cost { get; set; }

        [NotMapped]
        public decimal? Total { get; set; }

        [NotMapped]
        public decimal? Payments { get; set; }

        [NotMapped]
        public decimal? Remaning { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Time start is required")]
        public string InvoiceStartDate { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Time start is required")]
        public string InvoiceStartTime { get; set; }

        [NotMapped]
        public string InvoiceEndDate { get; set; }

        [NotMapped]
        public string InvoiceEndTime { get; set; }
        [NotMapped]
        public string IsBillableImage { get; set; }

        public List<JobScheduling> JobVisit { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public string DateAdded { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        public string TimeAdded { get; set; }

        public decimal? SubTotal { get; set; }

        public string Guid { get; set; }

        public string InvoiceNumber { get; set; }

        public int JobId { get; set; }

        ////public HttpPostedFile UploadInvoiceDocument { get; set; }

        public string MimeType { get; set; }

        public string FullFileName { get; set; }

        public string OwnerUsername { get; set; }

        public int? SentTo { get; set; }

        [NotMapped]
        public string XeroId { get; set; }

        [NotMapped]
        public string IsXeroId { get; set; }

        [NotMapped]
        public string OldFileName { get; set; }

        public string Signature { get; set; }

        public int? TaxRateId { get; set; }

        [Display(Name = "Tax Rate")]
        public string TaxRate { get; set; }

        [Display(Name = "Tax Amount")]
        public decimal TaxAmount{ get; set; }

        [Display(Name = "Account Code")]
        public string SaleAccountCode { get; set; }
    }
}

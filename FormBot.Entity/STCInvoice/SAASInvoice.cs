using System;
using System.Collections.Generic;

namespace FormBot.Entity
{
    public class SAASInvoice
    {
        public int UserTypeId { get; set; }
        public int SAASUserId { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int TotalRecords { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public int SettlementTermId { get; set; }
        public string SettlementTerms { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public int Status { get; set; }
        public bool IsSent { get; set; }
        public string XeroInvoiceId { get; set; }
        public bool IsDeleted { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public bool IsInvoice { get; set; }
        public int JobId { get; set; }
        public List<SAASInvoiceDetail> lstInvoiceDetail { get; set; }
        public List<SettlementTermSAAS> lstSettlementTerm { get; set; }
    }

    public class SAASInvoiceDetail
    {
        public int InvoiceId { get; set; }
        //public int InvoiceDetailId { get; set; }
        public int JobId { get; set; }
        public int STCJobDetailId { get; set; }
        public decimal STCPrice { get; set; }
        public string RefNumber { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public string Status { get; set; }
        public DateTime? RECBulkUploadTime { get; set; }
        public string STCPVDCode { get; set; }
        public bool IsSAASInvoiced { get; set; }
        public string strJobID { get; set; }
        public int QTY { get; set; }
        public string BillingPeriod { get; set; }
    }

    public class SettlementTermSAAS
    {
        public int SettlementTermId { get; set; }
        public string SettlementTerms { get; set; }
    }
}

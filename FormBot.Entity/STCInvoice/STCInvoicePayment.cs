using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class STCInvoicePayment
    {
        public int? STCInvoiceID { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal? Amount { get; set; }
        public string XeroPaymentID { get; set; }
        [Required(ErrorMessage = "Payment date required")]
        public DateTime? PaymentDate { get; set; }
        public Int64 STCInvoicePaymentID { get; set; }
        public string XeroInvoiceID { get; set; }
        public string Reference { get; set; }
        public string CompanyName { get; set; }
        public bool IsDeleted { get; set; }
        public string FilePath { get; set; }
        public string InvoiceStatus { get; set; }
        public int JobId { get; set; }
        public int SolarCompanyId { get; set; }
        public int ResellerUserId { get; set; }
        public int SolarCompanyUserId { get; set; }
        public int RankNumber { get; set; }
        public string STCInvoiceNumber { get; set; }
        public bool IsXeroPayment { get; set; }

        public string strPaymentDate
        {
            get
            {
                return PaymentDate != null ? PaymentDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }
    }

    public class Remittance {
        public int RankNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime InvoiceCreatedDate { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal StillOwing { get; set; }

        public string Reference { get; set; }

        public int JobId { get; set; }
        public int STCInvoiceID { get; set; }

        public string CompanyName { get; set; }
        public int STCInvoicePaymentID { get; set; }

        public DateTime SentDate { get; set; }

        public string ABN { get; set; }
        public string Logo { get; set; }

        public int ResellerUserId { get; set; }
        public int SolarCompanyId { get; set; }
        public int SolarCompanyUserId { get; set; }

        public decimal STCPrice { get; set; }
        public decimal CalculatedSTC { get; set; }

        public bool IsGst { get; set; }
        public bool IsWholeSaler { get; set; }

        public string STCInvoiceNumber { get; set; }
        public string description { get; set; }
        public string STCPVDCode { get; set; }
        public string ToAddressLine1 { get; set; }
        public string ToAddressLine2 { get; set; }
        public string ToAddressLine3 { get; set; }
        public string FromCompanyName { get; set; }
        public string fromAddressLine1 { get; set; }
        public string fromAddressLine2 { get; set; }
        public string fromAddressLine3 { get; set; }
        public string ToCompanyName { get; set; }
        public bool? IsCreditNote { get; set; }
        public decimal TaxRate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobInvoice
    {
        public int JobInvoiceID { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceRefNo { get; set; }

        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public int CreatedBy { get; set; }
        public int? SendTo { get; set; }

        public byte Status { get; set; }

        public decimal? Total { get; set; }

        public decimal? AmountDue { get; set; }

        public bool Sent { get; set; }

        public string FileName { get; set; }

        public bool IsDeleted { get; set; }

        public int JobId { get; set; }

        public DateTime? InvoiceSentDate { get; set; }

        public DateTime? XeroDraftDate { get; set; }

        public DateTime? XeroApprovedDate { get; set; }

        public string Created { get; set; }

        public string Send { get; set; }

        public string OwnerUsername { get; set; }

        public string JobInvoiceIDXero { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string InvoicedTo { get; set; }

        [NotMapped]
        public decimal? InvoiceTotal { get; set; }
        [NotMapped]
        public decimal? InvoiceAmountPaid { get; set; }

        [NotMapped]
        public decimal? InvoiceAmountDue { get; set; }

        [NotMapped]
        public int FileExist { get; set; }

        public string OwnerName { get; set; }

    }

}

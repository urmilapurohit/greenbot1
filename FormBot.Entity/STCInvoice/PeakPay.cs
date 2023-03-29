using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class PeakPay
    {

        public int UserID { get; set; }

        public int UserTypeID { get; set; }
        
        public int? ResellerId { get; set; }
        
        public int? SolarCompanyId { get; set; }

        public int JobID { get; set; }

        public string Job_Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }
        
        public string Reference { get; set; }
        
        public string OwnerName { get; set; }        
        
        public string InstallationAddress { get; set; }
        
        public int STCStatusId { get; set; }
        
        public string StcStatus { get; set; }
        
        public string SolarCompany { get; set; }

        public DateTime? SubmissionDate { get; set; }
        public string strSubmissionDate
        {
            get
            {
                return SubmissionDate != null ? SubmissionDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? CERApprovedDate { get; set; }
        public string strCERApprovedDate
        {
            get
            {
                return CERApprovedDate != null ? CERApprovedDate.Value.ToString("dd/MM/yyyy") : null;
            }

        }

        public DateTime? SettleBefore { get; set; }
        public string strSettleBefore
        {
            get
            {
                return SettleBefore != null ? SettleBefore.Value.ToString("dd/MM/yyyy") : null;
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

        public int DaysLeft { get; set; }

        public DateTime? PaymentDate { get; set; }
        public string strPaymentDate
        {
            get
            {
                return PaymentDate != null ? PaymentDate.Value.ToString("dd/MM/yyyy") : null;
            }

        }

        public decimal STCAmount { get; set; }

        public decimal STCPrice { get; set; }

        public decimal STCFee { get; set; }

        public decimal Total { get; set; }

        public bool IsGst { get; set; }

        public int? IsInvoiced { get; set; }

        public int TotalRecords { get; set; }

        public decimal? TotalAmount { get; set; }

        public int STCJobDetailsID { get; set; }

        public int STCJobStageID { get; set; }

        public string STCPVDCode { get; set; }

        public int STCSettlementTerm { get; set; }

        public decimal SetSTCPrice { get; set; }

        public int SystemSize { get; set; }

        public List<JobStage> lstPeakPayJobStages { get; set; }

        public int STCInvoiceCnt { get; set; }
    }
}

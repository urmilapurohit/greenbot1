using FormBot.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class REC
    {
        public int ResellerId { get; set; }

        public string BatchId { get; set; }

        public decimal TotalSTCs { get; set; }

        public int TotalJob { get; set; }

        public List<RECData> lstREC { get; set; } 

        public List<RECData> lstRecSearchFailedBatchId { get; set; }

        public List<RECData> lstRecSuccess { get; set; }

        public List<RECData> lstRecInProgress { get; set; }

        public bool IsIssue { get; set; }

        public bool IsFailedJobs { get; set; }
        public bool IsUnLockBtns { get; set; }
    }    

    public class RECData
    {
        public int JobId { get; set; }

        public string FailureReason { get; set; }

        public string RefNumber { get; set; }
        public string STCStatus { get; set; }
        public string RecStatus { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobId));
            }
        }

        public int RecFailureReasonId { get; set; }

        public string GBBatchRECUploadId { get; set; }

        public int StcJobDetailsId { get; set; }

        public decimal TotalSTC { get; set; }

        public string RECUserName { get; set; }

        public string RECName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string InitiatedBy { get; set; }

        public string ResellerName { get; set; }
        public string RECCompanyName { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        public string CurrentStatus { get; set; }

        public bool IsChecked { get; set; }

        public bool IsIssue { get; set; }

        public string InternalIssueDescription { get; set; }

    }

    public class SearchParamRec
    {
        public string BulkUploadId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string CreatedBy { get; set; }
        public int ResellerId { get; set; }
        public int StageId { get; set; }
        public string RECUsername { get; set; }
        public string RECName { get; set; }
        public string InitiatedBy { get; set; }
    }

    public class RECDataScheduler
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("result")]
        public List<Result> result { get; set; }

        public class Result
        {
            [JsonProperty("actionType")]
            public string actionType { get; set; }
            [JsonProperty("completedTime")]
            public string completedTime { get; set; }
            [JsonProperty("certificateRanges")]
            public List<CertificateRanges> certificateRanges { get; set; }

        }

        public class CertificateRanges
        {
            [JsonProperty("certificateType")]
            public string certificateType { get; set; }
            [JsonProperty("registeredPersonNumber")]
            public string registeredPersonNumber { get; set; }
            [JsonProperty("accreditationCode")]
            public string accreditationCode { get; set; }
            [JsonProperty("generationYear")]
            public string generationYear { get; set; }
            [JsonProperty("generationState")]
            public string generationState { get; set; }
            [JsonProperty("startSerialNumber")]
            public string startSerialNumber { get; set; }
            [JsonProperty("endSerialNumber")]
            public string endSerialNumber { get; set; }
            [JsonProperty("fuelSource")]
            public string fuelSource { get; set; }
            [JsonProperty("ownerAccount")]
            public string ownerAccount { get; set; }
            [JsonProperty("ownerAccountId")]
            public string ownerAccountId { get; set; }
            [JsonProperty("status")]
            public string status { get; set; }
        }

    }
}

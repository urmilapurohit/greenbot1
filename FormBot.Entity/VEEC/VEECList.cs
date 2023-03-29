using FormBot.Entity.VEEC;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class VEECList
    {
        //public int JobID { get; set; }

        public int VEECId { get; set; }

        public string RefNumber { get; set; }

        public string ClientName { get; set; }

        public string phone { get; set; }

        public string JobAddress { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string strCreatedDate
        {
            get
            {
                return CreatedDate != null ? CreatedDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? CommencementDate { get; set; }

        public string strCommencementDate
        {
            get
            {
                return CommencementDate != null ? CommencementDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? ActivityDate { get; set; }

        public string strActivityDate
        {
            get
            {
                return ActivityDate != null ? ActivityDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECId));
            }
        }

        public string Title { get; set; }

        public int Urgent { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string StaffName { get; set; }

        public string VeecStage { get; set; }

        public int DocumentCount { get; set; }

        public bool IsLatestDocument { get; set; }

        public decimal STC { get; set; }

        public int? SSCID { get; set; }

        public int UserTypeID { get; set; }

        public int? ResellerID { get; set; }

        public int? SolarCompanyId { get; set; }

        public int? ComplianceOfficerId { get; set; }

        public List<JobStage> lstJobStages { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsPreApprovaApproved { get; set; }

        public bool IsConnectionCompleted { get; set; }

        public bool IsReadyToTrade { get; set; }

        public int JobStageID { get; set; }

        public int STCJobStageID { get; set; }

        public string JobStageName { get; set; }

        public string ColorCode { get; set; }       

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int UserID { get; set; }

        public string SSCJobID { get; set; }

        public int JobTypeId { get; set; }

        public bool IsAccept { get; set; }

        public bool IsCustomPrice { get; set; }

        public decimal SystemSize { get; set; }

        public string SerialNumbers { get; set; }

        public string SCOName { get; set; }

        public bool IsClassic { get; set; }

        public List<AdvanceSearchCategory> lstAdvanceSearchCategory { get; set; }

        public List<PreConStatus> lstPreApproval { get; set; }

        public List<PreConStatus> lstConnection { get; set; }

        public int PreApprovalStatusId { get; set; }

        public int ConnectionStatusId { get; set; }

    }
}




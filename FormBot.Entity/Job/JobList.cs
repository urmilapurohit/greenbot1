using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class JobList
    {
        public int JobID { get; set; }
        public string JobNumber { get; set;}
        public int Urgent { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string ClientName { get; set; }

        public string StaffName { get; set; }

        public string phone { get; set; }

        public string JobAddress { get; set; }

        public string JobStage { get; set; }

        public string JobType { get; set; }

        public DateTime? InstallationDate { get; set; }

        public string strInstallationDate
        {
            get
            {
                return InstallationDate != null ? InstallationDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? RECBulkUploadTimeDate { get; set; }


        public string strRECBulkUploadTime
        {
            get
            {
                return RECBulkUploadTimeDate != null ? RECBulkUploadTimeDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        ////public int CommentCount { get; set; }

        ////public bool IsLatestComment { get; set; }

        public int DocumentCount { get; set; }

        public bool IsLatestDocument { get; set; }

        public int STCInvoiceCount { get; set; }

        ////public int InvoiceCount { get; set; }

        ////public bool IsLatestInvoice { get; set; }

        public decimal STC { get; set; }

        public string InstallationState { get; set; }

        public string InstallationTown { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public string strCreatedDate
        {
            get
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }

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

        #region For Price Check
        public decimal? PriceDay1 { get; set; }

        public decimal? UpFront { get; set; }

        public decimal? PriceDay3 { get; set; }

        public decimal? PriceDay7 { get; set; }

        public decimal? PriceOnApproval { get; set; }

        public decimal? PartialPayment { get; set; }

        public decimal? RapidPay { get; set; }

        public decimal? OptiPay { get; set; }

        public decimal? Commercial { get; set; }

        public decimal? Custom { get; set; }

        public decimal? InvoiceStc { get; set; }

        public decimal? PeakPay { get; set; }

        public bool IsTraded { get; set; }

        public bool? UnderKW { get; set; }
        
        public int? KWValue { get; set; }

        public bool? IsCustomUnderKW { get; set; }

        public int? CustomKWValue { get; set; }

        #endregion

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

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

        #region Properties For STCSubmission

        public int STCJobDetailsID { get; set; }

        public int? RAMID { get; set; }

        public string RefNumber { get; set; }

        public string Ownername { get; set; }

        public string RefNumberOwnerName { get; set; }

        public string InstallationAddress { get; set; }

        public string STCStatus { get; set; }

        public string SolarCompany { get; set; }

        public decimal STCPrice { get; set; }

        public DateTime? STCSubmissionDate { get; set; }

        public string strSTCSubmissionDate
        {
            get
            {
                return STCSubmissionDate != null ? STCSubmissionDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public DateTime? STCSettlementDate { get; set; }

        public string strSTCSettlementDate
        {
            get
            {
                return STCSettlementDate != null ? STCSettlementDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

        public bool IsGst { get; set; }

        public string PVDSWHCode { get; set; }

        public string ComplianceOfficer { get; set; }

        public bool HasMultipleRecords { get; set; }

        public List<JobStage> lstSTCJobStages { get; set; }

        public int STCSettlementTerm { get; set; }
        
        public bool IsInvoiced { get; set; }

        public bool IsCreditNote { get; set; }

        public bool IsPayment { get; set; }

        public int PreApprovalStatusId { get; set; }
        public int ConnectionStatusId { get; set; }

        public List<PreConStatus> lstPreApproval { get; set; }

        public List<PreConStatus> lstConnection { get; set; }

        public bool IsPartialValidForSTCInvoice { get; set; }

        public Dictionary<int, string> DictSettlementTerm { get; set; }

        public int? CustomSettlementTerm { get; set; }
        public string CustomTermLabel { get; set; }

        public int STCStatusId { get; set; }

        public string PropertyType { get; set; }

        public string GSTDocument { get; set; }

        public string AccountManager { get; set; }

        public string InstallerName { get; set; }

        public string OwnerName { get; set; }

        public string OwnerCompany { get; set; }
        #endregion

        public bool IsClassic { get; set; }

        public List<AdvanceSearchCategory> lstAdvanceSearchCategory { get; set; }

        public string OwnerType { get; set; }

        public string ComplianceNotes { get; set; }

        public int STCJobComplianceID { get; set; }

        public string GBBatchRECUploadId { get; set; }

        public bool? IsSPVInstallationVerified { get; set; }
        public bool? IsSPVRequired { get; set; } 

        public bool IsApproachingExpiryDate { get; set; }

        public bool? IsRelease { get; set; }
        [NotMapped]
        public bool SCisAllowedSPV { get; set; }
        public string PanelBrand { get; set; }
        public string PanelModel { get; set; }
        public string InverterBrand { get; set; }
        public string InverterModel { get; set; }
        public string InverterSeries { get; set; }
        public DateTime? CERAuditedDate { get; set; }

        public string strCERAuditedDate
        {
            get
            {
                return CERAuditedDate != null ? CERAuditedDate.Value.ToString("dd/MM/yyyy") : null;
            }
        }

    }

    public class DeleteJob_Failed
    {
        public int JobID { get; set; }
        public string Reason { get; set; }
        public string Title { get; set; }
    }

    public class PreConStatus
    {
        public int PreApprovalStatusId { get; set; }
        public int ConnectionStatusId { get; set; }
        public string PreApprovalStatusName { get; set; }
        public string ConnectionStatusName { get; set; }
    }

    public class AdvanceSearchCategory
    {
        public int SearchCategoryId { get; set; }
        public string SearchCategoryName { get; set; }
        public List<AdvanceSearchSubCategory> lstAdvanceSearchSubCategory { get; set; }
        public string AllFilters { get; set; }
        public string hdnAllFilters { get; set; }
        public string hdnColName { get; set; }
    }

    public class AdvanceSearchSubCategory
    {
        public int ColumnID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}

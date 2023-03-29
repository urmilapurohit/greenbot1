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
    public class FormBotReport
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }

        public IEnumerable<SelectListItem> LstSolarCompany { get; set; }
        public List<SelectListItem> LstSolarCompanyAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstUsers { get; set; }
        public string[] FormBotSolarCompanyAssignedUser { get; set; }
        public string hdnFormBotSolarCompanyAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstReseller { get; set; }
        public List<SelectListItem> LstResellerAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstResellerUsers { get; set; }
        public string[] FormBotResellerAssignedUser { get; set; }
        public string hdnFormBotResellerAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSSC { get; set; }
        public List<SelectListItem> LstSSCAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSSCUsers { get; set; }
        public string[] FormBotSSCAssignedUser { get; set; }
        public string hdnFormBotSSCAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSC { get; set; }
        public List<SelectListItem> LstSCAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSCUsers { get; set; }
        public string[] FormBotSCAssignedUser { get; set; }
        public string hdnFormBotSCAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSE { get; set; }
        public List<SelectListItem> LstSEAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSEUsers { get; set; }
        public string[] FormBotSEAssignedUser { get; set; }
        public string hdnFormBotSEAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSWH { get; set; }
        public List<SelectListItem> LstSWHAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSWHUsers { get; set; }
        public string[] FormBotSWHAssignedUser { get; set; }
        public string hdnFormBotSWHAssignedUser { get; set; }
        
        public IEnumerable<SelectListItem> LstStatus { get; set; }
        public List<SelectListItem> LstStatusAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstStatusUsers { get; set; }
        public string[] FormBotStatusAssignedUser { get; set; }
        public string hdnFormBotStatusAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSCO { get; set; }
        public List<SelectListItem> LstSCOAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSCOUsers { get; set; }
        public string[] FormBotSCOAssignedUser { get; set; }
        public string hdnFormBotSCOAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstRAM { get; set; }
        public List<SelectListItem> LstRAMAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstRAMUsers { get; set; }
        public string[] FormBotRAMAssignedUser { get; set; }
        public string hdnFormBotRAMAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstFCO { get; set; }
        public List<SelectListItem> LstFCOAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstFCOUsers { get; set; }
        public string[] FormBotFCOAssignedUser { get; set; }
        public string hdnFormBotFCOAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstFSA { get; set; }
        public List<SelectListItem> LstFSAAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstFSAUsers { get; set; }
        public string[] FormBotFSAAssignedUser { get; set; }
        public string hdnFormBotFSAAssignedUser { get; set; }

        public int TimePeriod { get; set; }
        public int SolarCompanyId { get; set; }
        public string SolarCompanyName { get; set; }
        public int DeletedFilter { get; set; }
        public int ChartType { get; set; }
        public int RAMOption { get; set; }
        public int PreApproval { get; set; }
        public int Connection { get; set; }
        public int STCsubmission { get; set; }
        public int TotalJob { get; set; }
        public string ResellerName { get; set; }
        public string RAMName { get; set; }
        public string CompanyName { get; set; }
        public int New { get; set; }
        public int NewInstallation { get; set; }

        public string UserName { get; set; }
        public string UserTypeName { get; set; }
        public DateTime LastLogIn { get; set; }

        public int InProgress { get; set; }
        public int Complete { get; set; }
        public int STCTrade { get; set; }
        public int Aftersales { get; set; }
        public int Cancellations { get; set; }
        public int InstallationCompleted { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string InvoiceType { get; set; }
        public string InvoiceMode { get; set; }
        public string InvoiceStatus { get; set; }
        public int LoginUserType { get; set; }
        public bool IsCreditNote { get; set; }

        public IEnumerable<SelectListItem> LstPreapprovalStatus { get; set; }
        public IEnumerable<SelectListItem> LstConnectionStatus { get; set; }
        public IEnumerable<SelectListItem> LstSTCSubmissionStatus { get; set; }
        public List<SelectListItem> LstPreapprovalStatusAssigned { get; set; }
        public List<SelectListItem> LstConnectionStatusAssigned { get; set; }
        public List<SelectListItem> LstSTCSubmissionStatusAssigned { get; set; }
        public string hdnPreapprovalStatusAssigned { get; set; }
        public string hdnConnectionStatusAssigned { get; set; }
        public string hdnSTCSubmissionStatusAssigned { get; set; }
        public int PreapprovalId { get; set; }
        public string PreapprovalStatus { get; set; }
        public int ConnectionId { get; set; }
        public string ConnectionStatus { get; set; }
        public int STCSubmissioinId { get; set; }
        public string STCSubmissioinStatus { get; set; }

        public bool IsAllSelected { get; set; }
        public int IsDetail { get; set; }
        public int OtherFilter { get; set; }
        public int NoOfUsers { get; set; }
        public string RefNumber { get; set; }
        public string Creator { get; set; }
        public string StageName { get; set; }

        public int RAMId { get; set; }
        public int ResellerID { get; set; }
        public string hdnResellers { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal percentageAmount { get; set; }
        public int UserID { get; set; }
        public string Status { get; set; }
        public int AmountOfFailure { get; set; }

        public string FullName { get; set; }
        public int TotalJobs { get; set; }
        public int CompletedJobs { get; set; }
        public int OutStandingJobs { get; set; }
        public int LogginUserID { get; set; }
        public int JobStageID { get; set; }
        public string SoldBy { get; set; }
        public DateTime? SoldByDate { get; set; }

        public IEnumerable<SelectListItem> LstJobStages { get; set; }
        public List<SelectListItem> LstJobStagesAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstJobStagesUsers { get; set; }
        public string[] JobStagesAssignedUser { get; set; }
        public string hdnJobStagesAssignedUser { get; set; }

        public IEnumerable<SelectListItem> LstSalesAgent { get; set; }
        public List<SelectListItem> LstSalesAgentAssignedUser { get; set; }
        public IEnumerable<SelectListItem> LstSalesAgentUsers { get; set; }
        public string[] SalesAgentAssignedUser { get; set; }
        public string hdnSalesAgentAssignedUser { get; set; }

        public string Owner_Name { get; set; }
        public DateTime? visitdate { get; set; }
        public string InvoiceSent { get; set; }
        public List<FormBotReport> lstReport { get; set; }
        public string ClientName { get; set; }
        public decimal NoOfSTC { get; set; }
        public decimal STCPrice { get; set; }
        public decimal Total { get; set; }

        public decimal NotYetSubmitted { get; set; }
        public decimal ReSubmission { get; set; }
        public decimal ReadyToTrade { get; set; }
        public decimal AwaitingAuthorization { get; set; }
        public decimal CERFailed { get; set; }
        public decimal CannotReCreate { get; set; }
        public decimal UnderReview { get; set; }
        public decimal ComplianceIssues { get; set; }
        public decimal RequiresCallBack { get; set; }
        public decimal ReadyToCreate { get; set; }
        public decimal PendingApproval { get; set; }
        public decimal NewSubmission { get; set; }
        public decimal CERApproved { get; set; }
        public DateTime STCSubmissionDate { get; set; }
        //public string Name { get; set; } 
        public int jobId { get; set; }
        [NotMapped]
        public string ID
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.jobId));
            }
        }

        public int DeletedUsers { get; set; }
        public int PendingUsers { get; set; }

        #region Allocation report properties
        public string Account_Manager { get; set; }
        public string SolarCompanyCreationDate { get; set; }
        public string SolarCompanyStatus { get; set; }
        public string JobStage { get; set; }
        public string AllocationDate { get; set; }
        public string UserStatus { get; set; }
        public int counts { get; set; }
        public string Name { get; set; }
        #endregion

        #region Job status detail report

        public string creationDate { get; set; }
        public string Preapprovals { get; set; }
        public string Connections { get; set; }
        public string STC_Submission { get; set; }
        public int count_PreApprovals { get; set; }
        public int count_Connections { get; set; }
        public int count_STC_Submission { get; set; }
        #endregion
    }

    public class AllocationReport
    {
        public List<FormBotReport> lstTotalJobRecord { get; set; }
        public List<FormBotReport> lstJobDetailsRecord { get; set; }

        public List<FormBotReport> dsSoldByWho { get; set; }
        public List<FormBotReport> dsSoldByWhoInnerTable { get; set; }

        public List<FormBotReport> dsSSCSEJobsDetail { get; set; }
        public List<FormBotReport> dsSSCSEJobsDetailInnerTable { get; set; }

        public List<FormBotReport> dsSTCGeneralGrid { get; set; }
        public List<FormBotReport> dsSTCGeneralChart { get; set; }
        public List<FormBotReport> dsSTCGeneralDashboardChart { get; set; }

        public List<FormBotReport> lstAllocationReportChart { get; set; }
    }

    public class JobStatusDetailReport
    {
        public List<FormBotReport> lstJob_StatuswiseRecord { get; set; }
        public List<FormBotReport> lstJobDetailsRecord { get; set; }
    }

    public class SystemUserReport
    {
        public List<FormBotReport> lstRecord { get; set; }
        public List<FormBotReport> lstChartRecord { get; set; }
    }

    public class JobStagesReport
    {
        public List<FormBotReport> lstRecord { get; set; }
        public List<FormBotReport> lstChartRecord { get; set; }
    }

    public class RecFailureReasonReport
    {
        public List<FormBotReport> lstRecord { get; set; }
        public List<FormBotReport> lstChartRecord { get; set; }
    }
    public class NonTradeJobReport
    {
        public List<FormBotReport> lstRecord { get; set; }
        public List<FormBotReport> lstChartRecord { get; set; }
    }
}

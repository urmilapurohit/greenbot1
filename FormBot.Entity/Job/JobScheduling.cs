using FormBot.Entity.CheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    [Serializable]
    public class JobScheduling
    {
        public string Id { get; set; }

        [DisplayName("Assign Staff:")]
        [Required(ErrorMessage = "Eletrician is required.")]
        public List<SolarElectricianView> solarElectrician { get; set; }

        ////[DisplayName("Assigned Job:")]
        ////[Required(ErrorMessage = "Job is required.")]
        ////public List<CreateJob> job { get; set; }

        [DisplayName("Assign Job:")]
        [Required(ErrorMessage = "Job is required.")]
        public List<JobDetails> job { get; set; }

        public List<JobScheduling> jobScheduling { get; set; }

        public string jobSchedulingData { get; set; }

        ////public SolarElectricianView solarElectrician;
        ////public JobDetails job;

        public int JobSchedulingID { get; set; }

        [DisplayName("Assign Job:")]
        [Required(ErrorMessage = "Job is required.")]
        public int JobID { get; set; }
        public string VisitJobId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        public string JobTitle { get; set; }

        public string SESCName { get; set; }

        ////public int ElectricianID { get; set; }
        [DisplayName("Assign Staff:")]
        [Required(ErrorMessage = "Electrician/Contractor is required.")]
        public int UserId { get; set; }

        [DisplayName("Visit Label:")]
        public string Label { get; set; }

        ////[DisplayName("Street Type:")]
        [DisplayName("Visit Detail:")]
        public string Detail { get; set; }

        [DisplayName("Visit Start:")]
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime visitStartDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan visitStartTime { get; set; }

        [DisplayName("Visit End:")]
        //[Required(ErrorMessage = "End date is required.")]
        public DateTime? visitEndDate { get; set; }

        //[Required(ErrorMessage = "End time is required.")]
        public TimeSpan? visitEndTime { get; set; }

        public string strVisitStartDate { get; set; }
        public string strVisitEndDate { get; set; }
        public string strVisitStartTime { get; set; }
        public string strVisitEndTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public int Status { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Month { get; set; }
        public int Date { get; set; }
        public int Year { get; set; }
        public string time { get; set; }

        public string StatusName { get; set; }
        ////public bool IsAccept { get; set; }
        ////public bool IsAccept { get; set; }

        public int JobStageId { get; set; }
        public string JobStage { get; set; }

        public string StageName { get; set; }

        public int UserTypeID { get; set; }
        public int? ResellerID { get; set; }
        public int? SolarCompanyId { get; set; }

        public List<JobScheduling> lstJobSchedule { get; set; }

        public List<JobNotes> lstJobNotes { get; set; }

        public bool IsDashboard { get; set; }

        public string UniqueVisitID { get; set; }

        public Int64 VisitNum { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public int NewNotesCount { get; set; }

        public bool IsCheckListView { get; set; }

        public bool IsClassic { get; set; }

        public CheckListTemplate checkListTemplate { get; set; }

        public int DefaultCheckListTemplateId { get; set; }
        public string DefaultTemplateId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.DefaultCheckListTemplateId));
            }
        }

        public int CheckListTemplateId { get; set; }
        public string CheckListTemplateEncodedId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.CheckListTemplateId));
            }
        }

        public string TemplateId { get; set; }
        public string CheckListTemplateName { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalCompletedItemCount { get; set; }

        public int VisitStatus { get; set; }
        public DateTime CompletedDate { get; set; }
        public string strCompletedDate { get; set; }

        public bool IsDefaultSubmission { get; set; }

        public string Panels { get; set; }
        public string Inverters { get; set; }

        public string SystemSize { get; set; }
        public string InstallerName { get; set; }
        public string InstallerAddress { get; set; }
        public string DesignerName { get; set; }
        public string DesignerAddress { get; set; }
        public string ElectricianName { get; set; }
        public string ElectricianAddress { get; set; }
        public int TempJobSchedulingId { get; set; }
        public string VisitUniqueId { get; set; }
        public bool IsFromCalendarView { get; set; }
        public int JobType { get; set; }

        [DisplayName("Visit Label:")]
        public string  RefNumber { get; set; }
    }
}

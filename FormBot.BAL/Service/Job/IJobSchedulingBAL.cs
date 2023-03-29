using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.Job
{
    public interface IJobSchedulingBAL
    {

        /// <summary>
        /// Jobs the scheduling_ insert update scheduling.
        /// </summary>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="label">The label.</param>
        /// <param name="detail">The detail.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="isDeleted">if set to <c>true</c> [is deleted].</param>
        /// <param name="status">The status.</param>
        /// <param name="isNotification">if set to <c>true</c> [is notification].</param>
        /// <param name="isDrop">if set to <c>true</c> [is drop].</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="checkListTemplateId">checkListTemplateId.</param>
        /// <returns>DataSet</returns>
        DataSet JobScheduling_InsertUpdateScheduling(int jobSchedulingID, int jobID, int userId, string label, string detail, DateTime startDate, TimeSpan startTime, DateTime? endDate, TimeSpan? endTime, DateTime createdDate, int createdBy, DateTime? ModifiedDate, int? modifiedBy, bool isDeleted, int status, bool isNotification, bool isDrop, int solarCompanyId, int userTypeId, int checkListTemplateId, string VisitCheckListItemIds, Int64 tempJobSchedulingId, bool IsFromCalendarView, bool IsQuickAddvisit);

        /// <summary>
        /// Gets the job scheduling detail.
        /// </summary>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobSchedulingDetail(int jobSchedulingID);

        /// <summary>
        /// Accepts the reject schedule.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>DataSet</returns>
        DataSet AcceptRejectSchedule(int status, int jobSchedulingID);

        /// <summary>
        /// Gets all calendar data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetAllCalendarData(int userId, int userTypeId, int solarCompanyId, string solarElectricianId);

        

        /// <summary>
        /// Gets SolarElectricianForJobType Data
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        List<SolarElectricianView> GetSolarElectricianForJobType(int userId, int userTypeId, int solarCompanyId, int JobType);

        /// <summary>
        /// Gets JobsForJobType Data
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        List<JobDetails> GetJobsForJobType(int userId, int userTypeId, int solarCompanyId, int JobType);

        /// <summary>
        /// Gets the job scheduling data by job identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobSchedulingDataByJobID(int userId, int userTypeId, int solarCompanyId, int jobId);

        /// <summary>
        /// Deletes the schedule.
        /// </summary>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>DataSet</returns>
        DataSet DeleteSchedule(int jobSchedulingID);

        /// <summary>
        /// Searches the jobs for scheduler.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="PageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort column.</param>
        /// <param name="SortDir">The sort direction.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="JobStage">The job stage.</param>
        /// <param name="Unscheduled">set true to see Unscheduled jobs.</param>
        /// <param name="All">set true to see all jobs.</param>
        /// <returns>List of Jobs</returns>
        List<JobList> SearchJobsForScheduler(int UserId, int UserTypeId, int ResellerId, int SolarCompanyId, int PageIndex, int PageSize, string SortCol, string SortDir, DateTime? FromDate, DateTime? ToDate, string searchText, int JobStage, bool Unscheduled, bool All);
        List<JobList> SearchJobsForSchedulerWithSE(int UserId, int UserTypeId, int ResellerId, int SolarCompanyId, int PageIndex, int PageSize, string SortCol, string SortDir, DateTime? FromDate, DateTime? ToDate, string searchText, int JobStage, bool Unscheduled, bool All, string solarElectricianId);

        /// <summary>
        /// Get jobScheduling history detail 
        /// </summary>
        /// <param name="jobSchedulingID">jobScheduling ID</param>
        /// <returns>dataset</returns>
        DataSet GetJobSchedulingHistoryDetail(int jobSchedulingID);

        /// <summary>
        /// Gets the job scheduling by user type for API.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <returns>dataset</returns>
        DataSet GetJobSchedulingByUserTypeForAPI(int userID, int userTypeId, int solarCompanyID, int jobId);

        /// <summary>
        /// Deletes the serial number.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        void DeleteSerialNumber(int jobID, string serialNumber, bool isAll);

        /// <summary>
        /// Updates the job stage for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="Stage">The stage.</param>
        void UpdateJobStageForAPI(int jobID, string Stage);

        void MakeVisitAsDefaultSubmission(int jobId, int jobSchedulingId, bool isDefault);

        DataSet ChangeVisitStatus(int jobSchedulingId, int visitStatus, DateTime? completedDate);

        DataSet GetReferencePhotosForAPI(int jobid);

        JobScheduling GetAllSchedulingDataOfJob(string id = null, bool isCheckListView = false, bool isReloadGridView = false, ICreateJobBAL _job = null, List<JobScheduling> lstJobSchedule=null);
        JobScheduling GetAllSchedulingDataOfJobModularAjax(int jobId = 0, bool isCheckListView = false, bool isReloadGridView = false, ICreateJobBAL _job = null, List<JobScheduling> lstJobSchedule = null);
        List<User> GetSolarElectrician(int userId, int userTypeId, int solarCompanyId);
        /// <summary>
        /// get data of visit data for history log
        /// </summary>
        /// <param name="jobschedulingId"></param>
        /// <param name="checkListTemplateId"></param>
        /// <returns></returns>
        DataSet GetJobSchedulingDataForHistoryLog(int jobschedulingId, int? checkListTemplateId);
    }
}

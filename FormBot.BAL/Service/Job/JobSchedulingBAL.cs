using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.Job
{
    public class JobSchedulingBAL : IJobSchedulingBAL
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
        /// <param name="ModifiedDate">The modified date.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="isDeleted">if set to <c>true</c> [is deleted].</param>
        /// <param name="status">The status.</param>
        /// <param name="isNotification">if set to <c>true</c> [is notification].</param>
        /// <param name="isDrop">if set to <c>true</c> [is drop].</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="checkListTemplateId">check List Template Id.</param>
        /// <returns>DataSet</returns>
        public DataSet JobScheduling_InsertUpdateScheduling(int jobSchedulingID, int jobID, int userId, string label, string detail, DateTime startDate, TimeSpan startTime, DateTime? endDate, TimeSpan? endTime, DateTime createdDate, int createdBy, DateTime? ModifiedDate, int? modifiedBy, bool isDeleted, int status, bool isNotification, bool isDrop, int solarCompanyId, int userTypeId, int checkListTemplateId, string VisitCheckListItemIds, Int64 tempJobSchedulingId, bool IsFromCalendarView,bool IsQuickAddvisit)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingID", SqlDbType.BigInt, jobSchedulingID));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.BigInt, jobID));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("Label", SqlDbType.NVarChar, label));
            sqlParameters.Add(DBClient.AddParameters("detail", SqlDbType.NVarChar, detail));
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.Date, startDate));
            sqlParameters.Add(DBClient.AddParameters("startTime", SqlDbType.Time, startTime));
            sqlParameters.Add(DBClient.AddParameters("endDate", SqlDbType.Date, endDate));
            sqlParameters.Add(DBClient.AddParameters("endTime", SqlDbType.Time, endTime));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, ModifiedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.BigInt, createdBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.BigInt, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("isDeleted", SqlDbType.Bit, isDeleted));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, status));
            sqlParameters.Add(DBClient.AddParameters("isNotification", SqlDbType.Bit, isNotification));
            sqlParameters.Add(DBClient.AddParameters("isDrop", SqlDbType.Bit, isDrop));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.BigInt, checkListTemplateId));

            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemIds", SqlDbType.NVarChar, VisitCheckListItemIds));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, tempJobSchedulingId));

            sqlParameters.Add(DBClient.AddParameters("IsFromCalendarView", SqlDbType.Bit, IsFromCalendarView));
            sqlParameters.Add(DBClient.AddParameters("IsQuickAddVisit", SqlDbType.Bit, IsQuickAddvisit));
            
            DataSet jobSchedulingDetail = CommonDAL.ExecuteDataSet("JobScheduling_InsertUpdateScheduling", sqlParameters.ToArray());
            return jobSchedulingDetail;

        }

        /// <summary>
        /// Gets the job scheduling detail.
        /// </summary>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobSchedulingDetail(int jobSchedulingID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingID", SqlDbType.BigInt, jobSchedulingID));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("JobScheduleDetail_GetJobScheduleDetail", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        /// <summary>
        /// Accepts the reject schedule.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet AcceptRejectSchedule(int status, int jobSchedulingID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingID", SqlDbType.BigInt, jobSchedulingID));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.BigInt, status));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("JobSchedulingDetail_AcceptRejectSchedule", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        /// <summary>
        /// Gets all calendar data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetAllCalendarData(int userId, int userTypeId, int solarCompanyId, string solarElectricianId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("SolarElectricianUserId", SqlDbType.NVarChar, (solarElectricianId == "All" ? null : solarElectricianId)));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("Calendar_GetAllData", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }     

        /// <summary>
        /// Gets SolarElectricianForJobType Data
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        public List<SolarElectricianView> GetSolarElectricianForJobType(int userId, int userTypeId, int solarCompanyId, int JobType)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.BigInt, JobType));
            List<SolarElectricianView> lstSolarElectrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("GET_SE_SC", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }

        /// <summary>
        /// Gets SolarElectricianForJobType Data
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        public List<JobDetails> GetJobsForJobType(int userId, int userTypeId, int solarCompanyId, int JobType)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.BigInt, JobType));
            List<JobDetails> lstJobs = CommonDAL.ExecuteProcedure<JobDetails>("GetJobsForJobType", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Gets the job scheduling data by job identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobSchedulingDataByJobID(int userId, int userTypeId, int solarCompanyId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.BigInt, jobId));

            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("Job_GetSchedulingDataByJobId", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        /// <summary>
        /// Deletes the schedule.
        /// </summary>
        /// <param name="jobSchedulingID">The job scheduling identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet DeleteSchedule(int jobSchedulingID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingID", SqlDbType.BigInt, jobSchedulingID));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("JobScheduling_DeleteSchedule", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

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
        public List<JobList> SearchJobsForScheduler(int UserId, int UserTypeId, int ResellerId, int SolarCompanyId, int PageIndex, int PageSize, string SortCol, string SortDir, DateTime? FromDate, DateTime? ToDate, string searchText, int JobStage, bool Unscheduled, bool All)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PageIndex", SqlDbType.Int, PageIndex));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchText));
            sqlParameters.Add(DBClient.AddParameters("JobStageId", SqlDbType.Int, JobStage));
            sqlParameters.Add(DBClient.AddParameters("IsUnscheduled", SqlDbType.Bit, Unscheduled));
            sqlParameters.Add(DBClient.AddParameters("IsAll", SqlDbType.Bit, All));

            List<JobList> lstJobs = CommonDAL.ExecuteProcedure<JobList>("Job_SearchJobsForSchedulerWithJobId", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }


        public List<JobList> SearchJobsForSchedulerWithSE(int UserId, int UserTypeId, int ResellerId, int SolarCompanyId, int PageIndex, int PageSize, string SortCol, string SortDir, DateTime? FromDate, DateTime? ToDate, string searchText, int JobStage, bool Unscheduled, bool All, string solarElectricianId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PageIndex", SqlDbType.Int, PageIndex));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchText));
            sqlParameters.Add(DBClient.AddParameters("JobStageId", SqlDbType.Int, JobStage));
            sqlParameters.Add(DBClient.AddParameters("IsUnscheduled", SqlDbType.Bit, Unscheduled));
            sqlParameters.Add(DBClient.AddParameters("IsAll", SqlDbType.Bit, All));
            sqlParameters.Add(DBClient.AddParameters("SolarElectricianUserId", SqlDbType.NVarChar, (solarElectricianId == "All" ? null : solarElectricianId)));
            List<JobList> lstJobs = CommonDAL.ExecuteProcedure<JobList>("Job_SearchJobsForSchedulerWithJobId", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Get jobScheduling history detail 
        /// </summary>
        /// <param name="jobSchedulingID">jobScheduling ID</param>
        /// <returns>dataset</returns>
        public DataSet GetJobSchedulingHistoryDetail(int jobSchedulingID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingID", SqlDbType.BigInt, jobSchedulingID));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("JobScheduling_HistoryDetail", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        /// <summary>
        /// Gets the job scheduling by user type for API.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <returns>dataset</returns>
        public DataSet GetJobSchedulingByUserTypeForAPI(int userID, int userTypeId, int solarCompanyID, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("Get_JobSchedulingByUserTypeForAPI", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        /// <summary>
        /// Deletes the serial number.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        public void DeleteSerialNumber(int jobID, string serialNumber, bool isAll)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.NVarChar, serialNumber));
            sqlParameters.Add(DBClient.AddParameters("IsAll", SqlDbType.Bit, isAll));
            CommonDAL.Crud("DeleteSerialNumberForAPI", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the job stage for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="Stage">The stage.</param>
        public void UpdateJobStageForAPI(int jobID, string Stage)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("JobStage", SqlDbType.NVarChar, Stage));
            CommonDAL.Crud("UpdateJobStageForAPI", sqlParameters.ToArray());
        }

        public void MakeVisitAsDefaultSubmission(int jobId, int jobSchedulingId, bool isDefault)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsDefaultSubmission", SqlDbType.Bit, isDefault));
            CommonDAL.Crud("JobScheduling_MakeVisitAsDefaultSubmission", sqlParameters.ToArray());
        }

        public DataSet ChangeVisitStatus(int jobSchedulingId, int visitStatus, DateTime? completedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VisitStatus", SqlDbType.Int, visitStatus));
            sqlParameters.Add(DBClient.AddParameters("CompletedDate", SqlDbType.DateTime, completedDate));
            //CommonDAL.Crud("JobScheduling_ChangeVisitStatus", sqlParameters.ToArray());

            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("JobScheduling_ChangeVisitStatus", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        public DataSet GetReferencePhotosForAPI(int jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.BigInt, jobid));
            DataSet GetReferencePhotosForAPI = CommonDAL.ExecuteDataSet("GetReferencePhotosForAPI", sqlParameters.ToArray());
            return GetReferencePhotosForAPI;
        }


        public JobScheduling GetAllSchedulingDataOfJob(string id = null, bool isCheckListView = false, bool isReloadGridView = false, ICreateJobBAL _job = null, List<JobScheduling> lstJobSchedule=null)
        {
            int jobId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out jobId);
            }
            JobScheduling jobScheduling = new JobScheduling();
            jobScheduling.IsFromCalendarView = false;
            jobScheduling.JobID = jobId;
            if(lstJobSchedule==null)
            { 
                jobScheduling.lstJobSchedule = _job.GetJobschedulingByJobID(jobId);
            }
            else
            {
                jobScheduling.lstJobSchedule = lstJobSchedule;
            }
            jobScheduling.lstJobNotes = _job.GetJobNotesListOnVisit(jobId);

            jobScheduling.NewNotesCount = jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Count();
            string notSeenJobNotesId = string.Join(",", jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Select(a => a.JobNotesID));

            jobScheduling.IsCheckListView = isCheckListView;

            int solarCompanyId = ProjectSession.SolarCompanyId;
            int userTypeId = ProjectSession.UserTypeId;
            int userId = ProjectSession.LoggedInUserId;

            DataSet calendarData = GetJobSchedulingDataByJobID(userId, userTypeId, solarCompanyId, jobId);
            if (calendarData != null && calendarData.Tables.Count > 0)
            {
                if (calendarData.Tables[0] != null && calendarData.Tables[0].Rows.Count > 0)
                {
                    var itemsSolarElectrician = calendarData.Tables[0].ToListof<SolarElectricianView>();

                    jobScheduling.solarElectrician = itemsSolarElectrician;
                }
                else
                {
                    jobScheduling.solarElectrician = new List<SolarElectricianView>();
                }
                if (calendarData.Tables[1] != null && calendarData.Tables[1].Rows.Count > 0)
                {
                    var itemsJobDetails = calendarData.Tables[1].ToListof<JobDetails>();

                    jobScheduling.IsClassic = calendarData.Tables[1].AsEnumerable().Where(a => a.Field<Int32>("JobID") == jobId).Select(a => a.Field<bool>("IsClassic")).FirstOrDefault();

                    jobScheduling.job = itemsJobDetails;
                }
                else
                {
                    jobScheduling.job = new List<JobDetails>();

                }
                if (calendarData.Tables[2] != null && calendarData.Tables[2].Rows.Count > 0)
                {
                    jobScheduling.DefaultCheckListTemplateId = Convert.ToInt32(calendarData.Tables[2].Rows[0]["CheckListTemplateId"]);
                }

            }
            jobScheduling.IsDashboard = false;

            if (!string.IsNullOrEmpty(notSeenJobNotesId))
            {
                _job.JobNotesMarkAsSeen(notSeenJobNotesId);
            }

            return jobScheduling;
        }


        public JobScheduling GetAllSchedulingDataOfJobModularAjax(int jobId = 0, bool isCheckListView = false, bool isReloadGridView = false, ICreateJobBAL _job = null, List<JobScheduling> lstJobSchedule = null)
        {
            
            //if (!string.IsNullOrEmpty(id))
            //{
            //    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out jobId);
            //}
            JobScheduling jobScheduling = new JobScheduling();
            jobScheduling.IsFromCalendarView = false;
            jobScheduling.JobID = jobId;
            if (lstJobSchedule == null)
            {
                jobScheduling.lstJobSchedule = _job.GetJobschedulingByJobID(jobId);
            }
            else
            {
                jobScheduling.lstJobSchedule = lstJobSchedule;
            }
            jobScheduling.lstJobNotes = _job.GetJobNotesListOnVisit(jobId);

            jobScheduling.NewNotesCount = jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Count();
            string notSeenJobNotesId = string.Join(",", jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Select(a => a.JobNotesID));

            jobScheduling.IsCheckListView = isCheckListView;

            int solarCompanyId = ProjectSession.SolarCompanyId;
            int userTypeId = ProjectSession.UserTypeId;
            int userId = ProjectSession.LoggedInUserId;

            DataSet calendarData = GetJobSchedulingDataByJobID(userId, userTypeId, solarCompanyId, jobId);
            if (calendarData != null && calendarData.Tables.Count > 0)
            {
                if (calendarData.Tables[0] != null && calendarData.Tables[0].Rows.Count > 0)
                {
                    var itemsSolarElectrician = calendarData.Tables[0].ToListof<SolarElectricianView>();

                    jobScheduling.solarElectrician = itemsSolarElectrician;
                }
                else
                {
                    jobScheduling.solarElectrician = new List<SolarElectricianView>();
                }
                if (calendarData.Tables[1] != null && calendarData.Tables[1].Rows.Count > 0)
                {
                    var itemsJobDetails = calendarData.Tables[1].ToListof<JobDetails>();

                    jobScheduling.IsClassic = calendarData.Tables[1].AsEnumerable().Where(a => a.Field<Int32>("JobID") == jobId).Select(a => a.Field<bool>("IsClassic")).FirstOrDefault();

                    jobScheduling.job = itemsJobDetails;
                }
                else
                {
                    jobScheduling.job = new List<JobDetails>();

                }
                if (calendarData.Tables[2] != null && calendarData.Tables[2].Rows.Count > 0)
                {
                    jobScheduling.DefaultCheckListTemplateId = Convert.ToInt32(calendarData.Tables[2].Rows[0]["CheckListTemplateId"]);
                }

            }
            jobScheduling.IsDashboard = false;

            if (!string.IsNullOrEmpty(notSeenJobNotesId))
            {
                _job.JobNotesMarkAsSeen(notSeenJobNotesId);
            }

            return jobScheduling;
        }
        public List<User> GetSolarElectrician(int userId, int userTypeId, int solarCompanyId) {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            List<User> lstUsers = CommonDAL.ExecuteProcedure<User>("GET_SE_SC", sqlParameters.ToArray()).ToList();
            return lstUsers;
        }
        /// <summary>
        /// get data of visit data for history log
        /// </summary>
        /// <param name="jobschedulingId"></param>
        /// <param name="checkListTemplateId"></param>
        /// <returns></returns>
       public DataSet GetJobSchedulingDataForHistoryLog(int jobschedulingId, int? checkListTemplateId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobschedulingId));
            sqlParameters.Add(DBClient.AddParameters("ChecklistTemplateId", SqlDbType.Int, checkListTemplateId));
            DataSet GetReferencePhotosForAPI = CommonDAL.ExecuteDataSet("GetJobSchedulingDataForHistoryLog", sqlParameters.ToArray());
            return GetReferencePhotosForAPI;
        }
    }
}

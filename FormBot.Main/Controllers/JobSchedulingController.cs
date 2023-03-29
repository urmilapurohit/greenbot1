using FormBot.BAL;
using FormBot.BAL.Service.Job;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Helper;
using FormBot.BAL.Service.SolarElectrician;
using System.Globalization;
using FormBot.BAL.Service;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using FormBot.Entity.Email;
using System.Threading.Tasks;
using FormBot.Email;
using FormBot.BAL.Service.CommonRules;
using System.Xml;
using System.Xml.Linq;
using FormBot.Helper.Helper;

namespace FormBot.Main.Controllers
{
    public class JobSchedulingController : BaseController
    {

        #region Properties
        private readonly IJobSchedulingBAL _jobScheduling;
        //private readonly IJobDetailsBAL _jobDetails;
        //private readonly ISolarElectricianBAL _solarElectrician;
        private readonly ICreateJobBAL _jobBAL;
        private readonly ICreateJobHistoryBAL _jobHistory;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly ILogger _log;

        #endregion

        #region Constructor

        public JobSchedulingController(IJobSchedulingBAL jobScheduling, ICreateJobBAL jobBAL, ICreateJobHistoryBAL jobHistory, IUserBAL userBAL, IEmailBAL emailBAL, ILogger log)
        {
            this._jobScheduling = jobScheduling;
            //this._jobDetails = jobDetails;
            //this._solarElectrician = solarElectrician;
            this._jobBAL = jobBAL;
            this._jobHistory = jobHistory;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
            this._log = log;
        }
        #endregion

        #region Method

        /// <summary>
        /// Job Scheduling Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult JobSchedulingDetail(string id = null, bool isCheckListView = false, bool isReloadGridView = false)
        {
            //WriteToLogFile("StartTime (Get JobSchedulingDetail) :" + DateTime.Now);

            if (!string.IsNullOrEmpty(id))
            {
                JobScheduling jobScheduling = new JobScheduling();
                jobScheduling = _jobScheduling.GetAllSchedulingDataOfJob(id, isCheckListView, isReloadGridView, _jobBAL);

                //WriteToLogFile("EndTime (Get JobSchedulingDetail) :" + DateTime.Now);

                if (isCheckListView)
                {
                    if (isReloadGridView)
                        return PartialView("_VisitGridView", jobScheduling);
                    else
                        return PartialView("_JobVisit", jobScheduling);
                }
                else
                    return PartialView("_jobScheduling", jobScheduling);
            }
            else
            {
                JobScheduling jobScheduling = new JobScheduling();
                int solarCompanyId = ProjectSession.SolarCompanyId;
                int userTypeId = ProjectSession.UserTypeId;
                int userId = ProjectSession.LoggedInUserId;
                jobScheduling.IsFromCalendarView = true;
                //jobScheduling.UserId = ProjectSession.LoggedInUserId;
                //jobScheduling.UserTypeID = ProjectSession.UserTypeId;
                //jobScheduling.ResellerID = ProjectSession.ResellerId;
                //jobScheduling.SolarCompanyId = ProjectSession.SolarCompanyId;

                if ((userTypeId != 2 && userTypeId != 5 && userTypeId != 1 && userTypeId != 3))
                {
                    jobScheduling = SetCalendarData(jobScheduling, userId, userTypeId, solarCompanyId);
                }
                else
                {
                    jobScheduling.solarElectrician = new List<SolarElectricianView>();
                    jobScheduling.job = new List<JobDetails>();
                    jobScheduling.jobSchedulingData = string.Empty;
                    //jobScheduling.jobSchedulingData = new List<JobScheduling>();
                }
                jobScheduling.IsDashboard = false;

                //if (isCheckListView)
                //    return PartialView("_JobVisit", jobScheduling);
                //else
                //    return View(jobScheduling);

                return View(jobScheduling);

                //return View(jobScheduling);
            }
        }

        //public JobScheduling GetAllSchedulingDataOfJob(string id = null, bool isCheckListView = false, bool isReloadGridView = false)
        //{
        //    int jobId = 0;
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out jobId);
        //    }
        //    JobScheduling jobScheduling = new JobScheduling();
        //    jobScheduling.IsFromCalendarView = false;
        //    jobScheduling.JobID = jobId;
        //    jobScheduling.lstJobSchedule = _jobBAL.GetJobschedulingByJobID(jobId);
        //    jobScheduling.lstJobNotes = _jobBAL.GetJobNotesListOnVisit(jobId);

        //    jobScheduling.NewNotesCount = jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Count();
        //    string notSeenJobNotesId = string.Join(",", jobScheduling.lstJobNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Select(a => a.JobNotesID));

        //    jobScheduling.IsCheckListView = isCheckListView;

        //    int solarCompanyId = ProjectSession.SolarCompanyId;
        //    int userTypeId = ProjectSession.UserTypeId;
        //    int userId = ProjectSession.LoggedInUserId;

        //    DataSet calendarData = _jobScheduling.GetJobSchedulingDataByJobID(userId, userTypeId, solarCompanyId, jobId);
        //    if (calendarData != null && calendarData.Tables.Count > 0)
        //    {
        //        if (calendarData.Tables[0] != null && calendarData.Tables[0].Rows.Count > 0)
        //        {
        //            var itemsSolarElectrician = calendarData.Tables[0].ToListof<SolarElectricianView>();

        //            jobScheduling.solarElectrician = itemsSolarElectrician;
        //        }
        //        else
        //        {
        //            jobScheduling.solarElectrician = new List<SolarElectricianView>();
        //        }
        //        if (calendarData.Tables[1] != null && calendarData.Tables[1].Rows.Count > 0)
        //        {
        //            var itemsJobDetails = calendarData.Tables[1].ToListof<JobDetails>();

        //            jobScheduling.IsClassic = calendarData.Tables[1].AsEnumerable().Where(a => a.Field<Int32>("JobID") == jobId).Select(a => a.Field<bool>("IsClassic")).FirstOrDefault();

        //            jobScheduling.job = itemsJobDetails;
        //        }
        //        else
        //        {
        //            jobScheduling.job = new List<JobDetails>();

        //        }
        //        if (calendarData.Tables[2] != null && calendarData.Tables[2].Rows.Count > 0)
        //        {
        //            jobScheduling.DefaultCheckListTemplateId = Convert.ToInt32(calendarData.Tables[2].Rows[0]["CheckListTemplateId"]);
        //        }

        //    }
        //    jobScheduling.IsDashboard = false;

        //    if (!string.IsNullOrEmpty(notSeenJobNotesId))
        //    {
        //        _jobBAL.JobNotesMarkAsSeen(notSeenJobNotesId);
        //    }

        //    return jobScheduling;
        //}

        /// <summary>
        /// Set Calendar Data
        /// </summary>
        /// <param name="jobScheduling">jobScheduling</param>
        /// <param name="userId">userId</param>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="solarCompanyId">solarCompanyId</param>
        /// <returns>JobScheduling</returns>
        public JobScheduling SetCalendarData(JobScheduling jobScheduling, int userId, int userTypeId, int solarCompanyId, string solarElectricianId ="")
        {
            DataSet calendarData = _jobScheduling.GetAllCalendarData(userId, userTypeId, solarCompanyId, solarElectricianId);
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
                    //List<JobDetails> itemsJobDetails = calendarData.Tables[1].AsEnumerable().Select(row =>
                    //new JobDetails
                    //{
                    //    JobID = row.Field<int>("JobID"),
                    //    Title = row.Field<string>("Title")
                    //}).ToList();

                    var itemsJobDetails = calendarData.Tables[1].ToListof<JobDetails>();

                    jobScheduling.job = itemsJobDetails;
                }
                else
                {
                    jobScheduling.job = new List<JobDetails>();
                }

                if (calendarData.Tables[2] != null && calendarData.Tables[2].Rows.Count > 0)
                {
                    calendarData.Tables[2].Columns.Add("ID");
                    foreach (DataRow dr in calendarData.Tables[2].Rows)
                    {
                        dr["ID"] = QueryString.QueryStringEncode("id=" + dr["JobID"].ToString());
                    }
                    //List<JobScheduling> itemsJobScheduling = calendarData.Tables[2].AsEnumerable().Select(row =>
                    //new JobScheduling
                    //{
                    //    JobSchedulingID = row.Field<int>("JobSchedulingID"),
                    //    JobID = row.Field<int>("JobID"),
                    //    JobTitle = row.Field<string>("JobTitle"),
                    //    Label = row.Field<string>("Label"),
                    //    //Label = !string.IsNullOrEmpty(row.Field<string>("Label")) ? ((row.Field<string>("Label").Contains("\\") ? row.Field<string>("Label").Replace("\\", "\\\\") : row.Field<string>("Label"))) : row.Field<string>("Label"),
                    //    //Label = !string.IsNullOrEmpty(row.Field<string>("Label")) ?  row.Field<string>("Label").Replace("\\", "\\\\").Replace("","'")  : row.Field<string>("Label"),
                    //    SESCName = row.Field<string>("SESCName"),
                    //    UserId = row.Field<int>("UserId"),
                    //    strVisitStartDate = row.Field<string>("strVisitStartDate"),
                    //    strVisitStartTime = row.Field<string>("strVisitStartTime"),
                    //    strVisitEndDate = row.Field<string>("strVisitEndDate"),
                    //    strVisitEndTime = row.Field<string>("strVisitEndTime"),
                    //    Detail = row.Field<string>("Detail"),
                    //    Status = row.Field<Byte>("Status"),
                    //}).ToList();
                    //List<JobScheduling> lstjobSchedulingData = calendarData.Tables[2].DataTableToList<JobScheduling>();
                    //for(int i = 0; i < lstjobSchedulingData.Count; i++)
                    //{
                    //    lstjobSchedulingData[i].Id = lstjobSchedulingData[i].VisitUniqueId;
                    //}
                    jobScheduling.jobSchedulingData = Newtonsoft.Json.JsonConvert.SerializeObject(calendarData.Tables[2]);
                }
                else
                    //jobScheduling.jobScheduling = new List<JobScheduling>();
                    jobScheduling.jobSchedulingData = string.Empty;
            }
            return jobScheduling;
        }

        public JsonResult GetSolarElectricianForJobType(int JobType, int SolarCompanyId = 0)
        {
            int companyid = 0;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                companyid = ProjectSession.SolarCompanyId;
            else
                companyid = SolarCompanyId;

            List<SelectListItem> items = _jobScheduling.GetSolarElectricianForJobType(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, companyid, JobType).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobsForJobType(int JobType, int SolarCompanyId = 0)
        {
            int companyid = 0;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                companyid = ProjectSession.SolarCompanyId;
            else
                companyid = SolarCompanyId;

            List<SelectListItem> items = _jobScheduling.GetJobsForJobType(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, companyid, JobType).Select(a => new SelectListItem { Text = a.RefNumber, Value = a.JobID.ToString() }).ToList();
            return Serializer.GetJsonResult(items);
            //var jsonResult = Json(items, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = Int32.MaxValue;
            //return jsonResult;
        }

        /// <summary>
        /// Job Scheduling Detail
        /// </summary>
        /// <param name="jobSchedulingPopup"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [UserAuthorization]
        public async Task<JsonResult> JobSchedulingDetail(JobSchedulingPopup jobSchedulingPopup, bool isQuickAddvisit)
        {
            //WriteToLogFile("1. StartTime:" + DateTime.Now);
            try
            {
                if (ModelState.IsValid)
                {
                    string jobSchedulingAssignTime = string.Empty;
                    DataSet jobSchedulingDetail = new DataSet();
                    int createdBy = ProjectSession.LoggedInUserId;
                    int jobSchedulingId = 0;
                    string jobSchedulingData = string.Empty;
                    JobScheduling jobScheduling = new JobScheduling();

                    int solarCompanyId = 0;
                    if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                        solarCompanyId = ProjectSession.SolarCompanyId;
                    else
                        solarCompanyId = jobSchedulingPopup.SolarCompanyId;


                    int userTypeId = ProjectSession.UserTypeId;

                    int UserId = 0;
                    int JobId = 0;
                    string JobName = "";
                    string eleFirstName = string.Empty;
                    string eleLastName = string.Empty;
                    string eleEmail = string.Empty;

                    int checkListTemplateId = 0;

                    DateTime? dtEndDate = null;
                    TimeSpan? timeEndTime = null;
                    if (!string.IsNullOrEmpty(jobSchedulingPopup.strVisitEndDate))
                    {
                        dtEndDate = Convert.ToDateTime(jobSchedulingPopup.strVisitEndDate);
                    }
                    if (!string.IsNullOrEmpty(jobSchedulingPopup.strVisitEndTime))
                    {
                        timeEndTime = TimeSpan.Parse(jobSchedulingPopup.strVisitEndTime);
                    }

                    if (!string.IsNullOrEmpty(jobSchedulingPopup.TemplateId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(jobSchedulingPopup.TemplateId, "id"), out checkListTemplateId);
                    }

                    if (Convert.ToInt32(jobSchedulingPopup.JobSchedulingID) > 0)
                    {

                        if (Convert.ToBoolean(jobSchedulingPopup.isDrop))
                        {
                            //As per Discussion with Hus, Always set JobSchedulingStatus as "Approved"
                            //update job scheduling data on drag-drop
                            int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Approved);
                            jobSchedulingDetail = _jobScheduling.JobScheduling_InsertUpdateScheduling(Convert.ToInt32(jobSchedulingPopup.JobSchedulingID), Convert.ToInt32(0), Convert.ToInt32(jobSchedulingPopup.UserId),
                            null, null, Convert.ToDateTime(jobSchedulingPopup.strVisitStartDate), TimeSpan.Parse(jobSchedulingPopup.strVisitStartTime), dtEndDate, new TimeSpan(), DateTime.Now, createdBy, DateTime.Now,
                            createdBy, false, scheduleStatus, Convert.ToBoolean(jobSchedulingPopup.isNotification), Convert.ToBoolean(jobSchedulingPopup.isDrop), solarCompanyId, userTypeId, checkListTemplateId,
                            jobSchedulingPopup.VisitCheckListItemIds, jobSchedulingPopup.TempJobSchedulingId, jobSchedulingPopup.IsFromCalendarView, isQuickAddvisit);
                        }
                        else
                        {
                            //update job scheduling data
                            int scheduleStatus = Convert.ToInt32(jobSchedulingPopup.Status);
                            jobSchedulingDetail = _jobScheduling.JobScheduling_InsertUpdateScheduling(Convert.ToInt32(jobSchedulingPopup.JobSchedulingID), Convert.ToInt32(jobSchedulingPopup.JobID), Convert.ToInt32(jobSchedulingPopup.UserId),
                           !string.IsNullOrEmpty(jobSchedulingPopup.Label) ? jobSchedulingPopup.Label.Trim() : jobSchedulingPopup.Label, !string.IsNullOrEmpty(jobSchedulingPopup.Detail) ? jobSchedulingPopup.Detail.Trim() : jobSchedulingPopup.Detail,
                           Convert.ToDateTime(jobSchedulingPopup.strVisitStartDate), TimeSpan.Parse(jobSchedulingPopup.strVisitStartTime), dtEndDate, timeEndTime, DateTime.Now, createdBy, DateTime.Now, createdBy, false, scheduleStatus,
                           Convert.ToBoolean(jobSchedulingPopup.isNotification), Convert.ToBoolean(jobSchedulingPopup.isDrop), solarCompanyId, userTypeId, checkListTemplateId, jobSchedulingPopup.VisitCheckListItemIds, jobSchedulingPopup.TempJobSchedulingId,
                           jobSchedulingPopup.IsFromCalendarView, isQuickAddvisit);
                        }
                        if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0 && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]) != -1)
                            {
                                int jobschedulingId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]);
                                DataSet dataForHistoryLog = _jobScheduling.GetJobSchedulingDataForHistoryLog(jobschedulingId, checkListTemplateId);
                                string visitId = string.Empty;
                                string templateName = string.Empty;
                                string assignto = _jobBAL.GetUsernameByUserID(jobSchedulingPopup.UserId);
                                if (dataForHistoryLog != null && dataForHistoryLog.Tables.Count > 0)
                                {
                                    visitId = dataForHistoryLog.Tables[0].Rows[0]["VisitUniqueId"]
                                        .ToString();
                                    templateName = dataForHistoryLog.Tables[1].Rows[0]["CheckListTemplateName"].ToString();
                                }
                                //bool isHistorySaved = _jobHistory.LogJobHistory(jobSchedulingPopup, HistoryCategory.Rescheduled);
                                //string JobHistoryMessage = "has edited visit -<b class=\"blue-title\"> (" + jobSchedulingPopup.JobID + ") JobRefNo </b>"; 
                                string JobHistoryMessage = " has edited scheduled with "+ "Visit id: <b style=\"color:black\">" + visitId + "</b> checklist template: <b style=\"color:black\">" + templateName + "</b>, installer: <b style=\"color:black\">" + assignto+ "</b> ,Visit date: <b style=\"color:black\">" + jobSchedulingPopup.strVisitStartDate + " - " + jobSchedulingPopup.strVisitEndDate + " " + jobSchedulingPopup.strVisitStartTime + " - " + jobSchedulingPopup.strVisitEndTime + "</b> .";
                                Common.SaveJobHistorytoXML(jobSchedulingPopup.JobID, JobHistoryMessage, "Scheduling", "Rescheduled", ProjectSession.LoggedInName, false,null);
                            }
                        }
                    }
                    else
                    {
                        //As per Discussion with Hus, Always set JobSchedulingStatus as "Approved"
                        //insert
                        int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Approved);
                        jobSchedulingDetail = _jobScheduling.JobScheduling_InsertUpdateScheduling(0, Convert.ToInt32(jobSchedulingPopup.JobID), Convert.ToInt32(jobSchedulingPopup.UserId),
                        !string.IsNullOrEmpty(jobSchedulingPopup.Label) ? jobSchedulingPopup.Label.Trim() : jobSchedulingPopup.Label, !string.IsNullOrEmpty(jobSchedulingPopup.Detail) ? jobSchedulingPopup.Detail.Trim() : jobSchedulingPopup.Detail,
                        Convert.ToDateTime(jobSchedulingPopup.strVisitStartDate), TimeSpan.Parse(jobSchedulingPopup.strVisitStartTime), dtEndDate, timeEndTime, DateTime.Now, createdBy, null, null, false, scheduleStatus,
                        Convert.ToBoolean(jobSchedulingPopup.isNotification), Convert.ToBoolean(jobSchedulingPopup.isDrop), solarCompanyId, userTypeId, checkListTemplateId, jobSchedulingPopup.VisitCheckListItemIds,
                        jobSchedulingPopup.TempJobSchedulingId, jobSchedulingPopup.IsFromCalendarView, isQuickAddvisit);
                        if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0 && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]) != -1)
                            {
                                if (isQuickAddvisit)
                                {
                                    jobSchedulingPopup.UserId = Convert.ToInt32(jobSchedulingDetail.Tables[1].Rows[0]["UserId"]);
                                }
                                //bool isHistorySaved = _jobHistory.LogJobHistory(jobSchedulingPopup, HistoryCategory.CreateSchedule);
                                int jobschedulingId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]);
                                DataSet dataForHistoryLog = _jobScheduling.GetJobSchedulingDataForHistoryLog(jobschedulingId, checkListTemplateId);
                                string visitId = string.Empty;
                                string templateName = string.Empty;
                                if(dataForHistoryLog != null && dataForHistoryLog.Tables.Count > 0)
                                {
                                    visitId = dataForHistoryLog.Tables[0].Rows[0]["VisitUniqueId"]
                                        .ToString();
                                    templateName= dataForHistoryLog.Tables[1].Rows[0]["CheckListTemplateName"].ToString();
                                }
                                string assignto = _jobBAL.GetUsernameByUserID(jobSchedulingPopup.UserId);

                                //string JobHistoryMessage = "has scheduled new visit to-<b class=\"blue-title\"> (" + jobSchedulingPopup.JobID+ ") JobRefNo</b>";
                                string JobHistoryMessage = "has scheduled new visit to installer <b style=\"color:black\">" + assignto+ "</b> Visit id: <b style=\"color:black\">" + visitId + "</b> with checklist template: <b style=\"color:black\">" + templateName + "</b> on visitDate: <b style=\"color:black\">" + jobSchedulingPopup.strVisitStartDate + " - " + jobSchedulingPopup.strVisitEndDate + " " + jobSchedulingPopup.strVisitStartTime + " - " + jobSchedulingPopup.strVisitEndTime + "</b> .";
                                Common.SaveJobHistorytoXML(jobSchedulingPopup.JobID, JobHistoryMessage, "Scheduling", "CreateSchedule", ProjectSession.LoggedInName ,false,null);
                                
                            }
                        }
                    }

                    if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0)
                    {
                        if (jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        {
                            jobSchedulingId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]);

                            if (jobSchedulingDetail.Tables[1] != null && jobSchedulingDetail.Tables[1].Rows.Count > 0)
                            {
                                UserId = Convert.ToInt32(jobSchedulingDetail.Tables[1].Rows[0]["UserId"]);
                                JobId = Convert.ToInt32(jobSchedulingDetail.Tables[1].Rows[0]["JobId"]);

                                eleFirstName = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["FirstName"]);
                                eleLastName = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["LastName"]);
                                eleEmail = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["Email"]);
                            }

                            if (jobSchedulingDetail.Tables[2] != null && jobSchedulingDetail.Tables[2].Rows.Count > 0)
                            {
                                JobName = jobSchedulingDetail.Tables[2].Rows[0]["Header"].ToString();
                            }

                            //mail
                            //if (UserId > 0 && JobId > 0)
                            //{
                            //    var dsToUsers = _userBAL.GetUserById(UserId);
                            //    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

                            //    var dsFromUsers = _userBAL.GetUserById(ProjectSession.LoggedInUserId);
                            //    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

                            //    EmailInfo emailinfo = new EmailInfo();

                            //    emailinfo.JobName = JobName;
                            //    if (Convert.ToInt32(jobSchedulingPopup.JobSchedulingID) > 0)
                            //    {
                            //        emailinfo.TemplateID = 21;
                            //    }
                            //    else
                            //    {
                            //        emailinfo.TemplateID = 19;
                            //    }
                            //    if (dsToUsers != null && dsFromUsers != null && dsToUsers.Tables[0].Rows.Count > 0 && dsFromUsers.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(toUserDetail.Email) && !string.IsNullOrEmpty(FromUserDetail.Email))
                            //    {
                            //        emailinfo.FirstName = toUserDetail.FirstName;
                            //        emailinfo.LastName = toUserDetail.LastName;

                            //        var syncTask = new Task(() =>
                            //        {
                            //            _emailBAL.ComposeAndSendEmail(emailinfo, toUserDetail.Email);
                            //        });
                            //        syncTask.RunSynchronously();

                            //        // SendMailForScheduling(emailinfo, toUserDetail.Email);
                            //    }
                            //}

                            //mail
                            if (UserId > 0 && JobId > 0)
                            {
                                var dsToUsers = _userBAL.GetUserById(UserId);

                                EmailInfo emailinfo = new EmailInfo();

                                emailinfo.JobName = JobName;
                                if (Convert.ToInt32(jobSchedulingPopup.JobSchedulingID) > 0)
                                {
                                    emailinfo.TemplateID = 21;
                                }
                                else
                                {
                                    emailinfo.TemplateID = 19;
                                }

                                //if (dsToUsers != null && dsToUsers.Tables.Count > 0 && dsToUsers.Tables[0] !=null && dsToUsers.Tables[0].Rows.Count > 0)
                                //{

                                //    emailinfo.FirstName = dsToUsers.Tables[0].Rows[0]["FirstName"].ToString();
                                //    emailinfo.LastName = dsToUsers.Tables[0].Rows[0]["LastName"].ToString();

                                emailinfo.FirstName = eleFirstName;
                                emailinfo.LastName = eleLastName;


                                Account acct = System.Web.HttpContext.Current.Session[Constants.sessionAccount] as Account;

                                //WriteToLogFile("2. StartTime (ComposeAndSendEmail) :" + DateTime.Now);

                                var syncTask = new Task(() =>
                                {
                                    _emailBAL.ComposeAndSendEmail(emailinfo, dsToUsers.Tables[0].Rows[0]["Email"].ToString(), null, null, default(Guid), Convert.ToString(JobId));
                                });
                                syncTask.RunSynchronously();

                                //WriteToLogFile("3. EndTime (ComposeAndSendEmail) :" + DateTime.Now);
                            }

                            // Send notification to assign user
                            if (jobSchedulingId > 0)
                            {
                                //_pushNotificationBAL.SendPushNotification(jobSchedulingPopup.UserId, jobScheduling.JobTitle +" - has been schedule");

                                //WriteToLogFile("4. StartTime (SendPushNotification) :" + DateTime.Now);
                                var syncTask = new Task(() =>
                                {
                                    new PushNotification().SendPushNotification(jobSchedulingPopup.UserId, "Job '" + jobSchedulingPopup.JobTitle + "' has been scheduled");
                                });
                                syncTask.RunSynchronously();
                                //WriteToLogFile("5. EndTime (SendPushNotification) :" + DateTime.Now);

                            }
                        }

                        if (jobSchedulingDetail.Tables.Count > 3 && jobSchedulingDetail.Tables[3] != null && jobSchedulingDetail.Tables[3].Rows.Count > 0)
                        {
                            jobSchedulingDetail.Tables[3].Columns.Add("ID");
                            foreach (DataRow dr in jobSchedulingDetail.Tables[3].Rows)
                            {
                                dr["ID"] = QueryString.QueryStringEncode("id=" + dr["JobID"].ToString());
                            }
                            jobSchedulingData = Newtonsoft.Json.JsonConvert.SerializeObject(jobSchedulingDetail.Tables[3]);
                        }
                    }

                    await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, JobId);

                    if (JobId > 0)
                    {
                        DataTable dt = _jobBAL.GetSTCDetailsAndJobDataForCache(null, JobId.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            string spvRequired = dt.Rows[0]["IsSPVRequired"].ToString();
                            SortedList<string, string> data = new SortedList<string, string>();
                            data.Add("IsSPVRequired", spvRequired);
                            await CommonBAL.SetCacheDataForSTCSubmission(null, JobId, data);
                        }
                       
                    }
                    //WriteToLogFile("6. EndTime :" + DateTime.Now);
                    //WriteToLogFile(DateTime.Now.ToString()+" save/update job scheduling data successfully.. jobID: " + JobId + " JobSchedulingid: " + jobSchedulingId + "VisitChecklistTempId: " + checkListTemplateId + "Userid:" + ProjectSession.LoggedInUserId+"VisitchecklistItemIds: "+jobSchedulingPopup.VisitCheckListItemIds);
                    _log.LogException(DateTime.Now.ToString() + " save/update job scheduling data successfully.. jobID: " + JobId + " JobSchedulingid: " + jobSchedulingId + "VisitChecklistTempId: " + checkListTemplateId + "Userid:" + ProjectSession.LoggedInUserId + "VisitchecklistItemIds: " + jobSchedulingPopup.VisitCheckListItemIds, null);
                    return Json(Convert.ToString(jobSchedulingId) + "^" + jobSchedulingData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg = d.Errors[0].ErrorMessage;
                    });
                    return Json(0 + "^" + msg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0 + "^" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Save accept reject status for scheduling
        /// </summary>
        /// <param name="status">status</param>
        /// <param name="jobSchedulingID">jobSchedulingID</param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult AcceptRejectJobSchedule(string status, string jobSchedulingID)
        {
            JobScheduling jobScheduling = new JobScheduling();
            int jobSchedulingId = 0;
            int UserId = 0;
            if (Convert.ToInt32(jobSchedulingID) > 0)
            {
                DataSet dsSchedule = _jobScheduling.AcceptRejectSchedule(Convert.ToInt32(status), Convert.ToInt32(jobSchedulingID));
                if (dsSchedule != null && dsSchedule.Tables.Count > 0)
                {
                    if (dsSchedule.Tables[0] != null && dsSchedule.Tables[0].Rows.Count > 0)
                    {
                        jobSchedulingId = Convert.ToInt32(dsSchedule.Tables[0].Rows[0]["JobSchedulingID"]);
                    }
                    if (dsSchedule.Tables[0] != null && dsSchedule.Tables[0].Rows.Count > 0)
                    {
                        UserId = Convert.ToInt32(dsSchedule.Tables[1].Rows[0]["UserId"]);
                    }
                }
                if (Convert.ToInt32(jobSchedulingID) > 0)
                {
                    var dsToUsers = _userBAL.GetUserById(UserId);
                    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

                    var dsFromUsers = _userBAL.GetUserById(ProjectSession.LoggedInUserId);
                    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

                    EmailInfo emailinfo = new EmailInfo();
                    emailinfo.TemplateID = status == "3" ? 18 : 17;
                    //if (status == "3")
                    //{
                    //    emailinfo.TemplateID = 18;
                    //}
                    //else
                    //{
                    //    emailinfo.TemplateID = 17;
                    //}
                    if (dsToUsers != null && dsFromUsers != null && dsToUsers.Tables[0].Rows.Count > 0 && dsFromUsers.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(toUserDetail.Email) && !string.IsNullOrEmpty(FromUserDetail.Email))
                    {
                        emailinfo.FirstName = toUserDetail.FirstName;
                        emailinfo.LastName = toUserDetail.LastName;
                        emailinfo.SolarElectrician = FromUserDetail.FirstName + ' ' + FromUserDetail.LastName;
                        _emailBAL.ComposeAndSendEmail(emailinfo, toUserDetail.Email);
                    }
                }
                DataSet jobSchedulingDetail = _jobScheduling.GetJobSchedulingHistoryDetail(Convert.ToInt32(jobSchedulingID));

                //For history log.

                if (jobSchedulingDetail != null && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                {
                    jobScheduling.JobID = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["JobId"]);
                    jobScheduling.JobTitle = Convert.ToString(jobSchedulingDetail.Tables[0].Rows[0]["Title"]);

                    if (status == "3")
                    {
                        //bool isHistorySaved = _jobHistory.LogJobHistory(jobScheduling, HistoryCategory.DeclinedSchedule);
                        string Title = !string.IsNullOrEmpty(jobScheduling.JobTitle) ? jobScheduling.JobTitle : jobScheduling.RefNumber;
                        string JobHistoryMessage = "have <b>declined</b> job <b class=\"blue-title\">" +Title + "</b>";
                        Common.SaveJobHistorytoXML(jobScheduling.JobID, JobHistoryMessage, "Scheduling", "DeclinedSchedule", ProjectSession.LoggedInName, false);
                    }
                    else if (status == "2")
                    {
                        //bool isHistorySaved = _jobHistory.LogJobHistory(jobScheduling, HistoryCategory.AcceptedSchedule);
                        string Title = !string.IsNullOrEmpty(jobScheduling.JobTitle) ? jobScheduling.JobTitle : jobScheduling.RefNumber;
                        string JobHistoryMessage = "have <b>accepted</b> job <b class=\"blue-title\">" + Title + "</b>";
                        Common.SaveJobHistorytoXML(jobScheduling.JobID, JobHistoryMessage, "Scheduling", "AcceptedSchedule", ProjectSession.LoggedInName, false);
                    }
                }

            }
            return Json(jobSchedulingId, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get jobScheduling detail for particular id
        /// </summary>
        /// <param name="jobSchedulingID"></param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult JobSchedulingDetailById(string jobSchedulingID)
        {
            JobScheduling jobScheduling = new JobScheduling();
            try
            {
                if (Convert.ToInt32(jobSchedulingID) > 0)
                {
                    DataSet jobSchedulingDetail = _jobScheduling.GetJobSchedulingDetail(Convert.ToInt32(jobSchedulingID));
                    if (jobSchedulingDetail != null && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                    {
                        jobScheduling.JobID = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["JobId"]);
                        jobScheduling.JobSchedulingID = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["JobSchedulingID"]);
                        jobScheduling.UserId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["UserId"]);
                        jobScheduling.strVisitStartDate = Convert.ToString(Convert.ToDateTime(jobSchedulingDetail.Tables[0].Rows[0]["StartDate"]).ToString("yyyy/MM/dd"));
                        jobScheduling.strVisitStartTime = Convert.ToString(TimeSpan.Parse(jobSchedulingDetail.Tables[0].Rows[0]["StartTime"].ToString()));
                        //jobScheduling.strVisitEndDate = Convert.ToString(Convert.ToDateTime(jobSchedulingDetail.Tables[0].Rows[0]["EndDate"]).ToString("yyyy/MM/dd"));
                        //jobScheduling.strVisitEndTime = Convert.ToString(TimeSpan.Parse(jobSchedulingDetail.Tables[0].Rows[0]["EndTime"].ToString()));
                        jobScheduling.strVisitEndDate = DBNull.Value == jobSchedulingDetail.Tables[0].Rows[0]["EndDate"] ? null : Convert.ToString(Convert.ToDateTime(jobSchedulingDetail.Tables[0].Rows[0]["EndDate"]).ToString("yyyy/MM/dd"));
                        jobScheduling.strVisitEndTime = DBNull.Value == jobSchedulingDetail.Tables[0].Rows[0]["EndTime"] ? null : Convert.ToString(TimeSpan.Parse(jobSchedulingDetail.Tables[0].Rows[0]["EndTime"].ToString()));

                        //jobScheduling.strVisitEndDate = Convert.ToString(Convert.ToDateTime(jobSchedulingDetail.Tables[0].Rows[0]["EndDate"]).ToString("yyyy/MM/dd"));
                        //jobScheduling.strVisitEndTime = Convert.ToString(TimeSpan.Parse(jobSchedulingDetail.Tables[0].Rows[0]["EndTime"].ToString()));

                        jobScheduling.Label = Convert.ToString(jobSchedulingDetail.Tables[0].Rows[0]["Label"]);
                        jobScheduling.Detail = Convert.ToString(jobSchedulingDetail.Tables[0].Rows[0]["Detail"]);
                        jobScheduling.Status = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["Status"]);
                        //jobScheduling.TotalItemCount = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["TotalItemCount"]);
                        jobScheduling.DefaultCheckListTemplateId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["DefaultCheckListTemplateId"]);
                        //jobScheduling.CheckListTemplateId = Convert.ToInt32(!string.IsNullOrEmpty(jobSchedulingDetail.Tables[0].Rows[0]["CheckListTemplateId"] );
                        jobScheduling.RefNumber = Convert.ToString(jobSchedulingDetail.Tables[0].Rows[0]["RefNumber"]);
                        jobScheduling.CheckListTemplateId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["CheckListTemplateId"]);
                        jobScheduling.IsClassic = Convert.ToBoolean(jobSchedulingDetail.Tables[0].Rows[0]["IsClassic"]);
                        jobScheduling.JobType = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["JobType"]);

                        //jobScheduling.CheckListTemplateName = Convert.ToString(jobSchedulingDetail.Tables[0].Rows[0]["CheckListTemplateName"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Json(jobScheduling, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult ChangeAssignJobDateElectrician(string jobSchedulingID, string startDate, string endDate, string userID)
        //{
        //    int jobSchedulingId = 0;
        //    int createdBy = ProjectSession.LoggedInUserId;
        //    string jobSchedulingData = string.Empty;
        //    DataSet jobSchedulingDetail = new DataSet();

        //    //TODO
        //    //createdBy = 84;

        //    jobSchedulingDetail = _jobScheduling.ChangeAssignJobDateElectrician(Convert.ToInt32(jobSchedulingID), Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), Convert.ToInt32(userID), createdBy);

        //    if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0)
        //    {
        //        if (jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
        //        {
        //            jobSchedulingId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingId"]);
        //        }
        //        if (jobSchedulingDetail.Tables[1] != null && jobSchedulingDetail.Tables[1].Rows.Count > 0)
        //        {
        //            jobSchedulingDetail.Tables[1].AsEnumerable().ToList().ForEach(d => jobSchedulingData += (!string.IsNullOrEmpty(jobSchedulingData) ? ",{" : "{") +
        //                                                     "\"strVisitStartDate\"" + ":\"" + Convert.ToString(d.Field<string>("strVisitStartDate")) + "\"," +
        //                                                     "\"strVisitStartTime\"" + ":\"" + Convert.ToString(d.Field<string>("strVisitStartTime")) + "\"," +
        //                                                      "\"Label\"" + ":\"" + Convert.ToString(d.Field<string>("Label")) + "\"," +
        //                                                       "\"Detail\"" + ":\"" + Convert.ToString(d.Field<string>("Detail")) + "\"," +
        //                                                          "\"JobID\"" + ":\"" + Convert.ToString(d.Field<int>("JobID")) + "\"," +
        //                                                           "\"JobSchedulingID\"" + ":\"" + Convert.ToString(d.Field<int>("JobSchedulingID")) + "\"," +
        //                                                             "\"UserId\"" + ":\"" + Convert.ToString(d.Field<int>("UserId")) + "\"," +
        //                                                     "\"strVisitEndDate\"" + ":\"" + Convert.ToString(d.Field<string>("strVisitEndDate")) + "\"," +
        //                                                     "\"strVisitEndTime\"" + ":\"" + Convert.ToString(d.Field<string>("strVisitEndTime")) + "\"}");

        //            jobSchedulingData = !string.IsNullOrEmpty(jobSchedulingData) ? "[" + jobSchedulingData + "]" : "";
        //        }
        //    }

        //    return Json(Convert.ToString(jobSchedulingId) + "&" + jobSchedulingData, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// Delete Job Schedule
        /// </summary>
        /// <param name="jobSchedulingID">jobSchedulingID</param>
        /// <param name="userId">userId</param>
        /// <param name="jobTitle">jobTitle</param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [UserAuthorization]
        public JsonResult DeleteJobSchedule(string jobSchedulingID, string userId, string jobTitle)
        {
            int jobSchedulingId = 0;
            int UserId = 0;
            int JobId = 0;
            string JobName = "";
            JobSchedulingPopup jobSchedulingPopup = new JobSchedulingPopup();

            if (Convert.ToInt32(jobSchedulingID) > 0)
            {
                DataSet dsJobSchedulingDetail = _jobScheduling.DeleteSchedule(Convert.ToInt32(jobSchedulingID));
                if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables.Count > 0)
                {
                    if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[0] != null && dsJobSchedulingDetail.Tables[0].Rows.Count > 0)
                    {
                        jobSchedulingId = Convert.ToInt32(dsJobSchedulingDetail.Tables[0].Rows[0]["JobSchedulingId"]);
                    }
                    if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[1] != null && dsJobSchedulingDetail.Tables[1].Rows.Count > 0)
                    {
                        UserId = Convert.ToInt32(dsJobSchedulingDetail.Tables[1].Rows[0]["UserId"]);
                        JobId = Convert.ToInt32(dsJobSchedulingDetail.Tables[1].Rows[0]["JobId"]);
                        jobSchedulingPopup.JobID = JobId;
                    }
                    if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[2] != null && dsJobSchedulingDetail.Tables[2].Rows.Count > 0)
                    {
                        JobName = dsJobSchedulingDetail.Tables[2].Rows[0]["Header"].ToString();
                    }
                    if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[3] != null && dsJobSchedulingDetail.Tables[3].Rows.Count > 0)
                    {
                        jobSchedulingPopup.strVisitStartDate = dsJobSchedulingDetail.Tables[3].Rows[0]["StartDate"].ToString();
                        jobSchedulingPopup.strVisitStartTime = dsJobSchedulingDetail.Tables[3].Rows[0]["StartTime"].ToString();
                        jobSchedulingPopup.UserId = Convert.ToInt32(dsJobSchedulingDetail.Tables[3].Rows[0]["UserId"]);
                        jobSchedulingPopup.InstallerDesignerId = !string.IsNullOrEmpty(dsJobSchedulingDetail.Tables[3].Rows[0]["InstallerDesignerId"].ToString())?Convert.ToInt32(dsJobSchedulingDetail.Tables[3].Rows[0]["InstallerDesignerId"]):0;
                        jobSchedulingPopup.SEStatus = Convert.ToInt32(dsJobSchedulingDetail.Tables[3].Rows[0]["SEStatus"]);
                    }
                }
                //mail
                if (UserId > 0 && JobId > 0)
                {
                    var dsToUsers = _userBAL.GetUserById(UserId);
                    FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

                    var dsFromUsers = _userBAL.GetUserById(ProjectSession.LoggedInUserId);
                    FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

                    EmailInfo emailinfo = new EmailInfo();

                    emailinfo.JobName = JobName;
                    emailinfo.TemplateID = 20;

                    if (dsToUsers != null && dsFromUsers != null && dsToUsers.Tables[0].Rows.Count > 0 && dsFromUsers.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(toUserDetail.Email) && !string.IsNullOrEmpty(FromUserDetail.Email))
                    {
                        emailinfo.FirstName = toUserDetail.FirstName;
                        emailinfo.LastName = toUserDetail.LastName;

                        _emailBAL.ComposeAndSendEmail(emailinfo, toUserDetail.Email, null, null, default(Guid), Convert.ToString(JobId));
                        jobSchedulingPopup.UserName = toUserDetail.FirstName + ' ' + toUserDetail.LastName;
                    }
                }
                //Send notification to assign user
                if (Convert.ToInt32(jobSchedulingID) > 0 && Convert.ToInt32(userId) > 0)
                {
                    try
                    {
                        //_pushNotificationBAL.SendPushNotification(Convert.ToInt32(userId),jobSchedulingPopup.JobTitle+" - has been deleted.");
                        new PushNotification().SendPushNotification(Convert.ToInt32(userId), "Job '" + Convert.ToString(dsJobSchedulingDetail.Tables[3].Rows[0]["Label"]) + "' has been deleted.");
                    }
                    catch (Exception ex)
                    {
                        //WriteToLogFile(ex.Message);
                        _log.LogException("DeleteJobSchedule jobSchedulingID = " + jobSchedulingID, ex);
                        //WriteToLogFile(ex.StackTrace);
                    }
                }

                //bool isHistorySaved = _jobHistory.LogJobHistory(jobSchedulingPopup, HistoryCategory.RemovedSchedule);
                string username = _jobBAL.GetUsernameByUserID(jobSchedulingPopup.UserId);
                DataSet dataForHistoryLog = _jobScheduling.GetJobSchedulingDataForHistoryLog(jobSchedulingId, jobSchedulingPopup.CheckListTemplateId);
                string visitId = string.Empty;
                string templateName = string.Empty;
                if (dataForHistoryLog != null && dataForHistoryLog.Tables.Count > 0)
                {
                    visitId = dataForHistoryLog.Tables[0].Rows[0]["VisitUniqueId"]
                        .ToString();
                    templateName = dataForHistoryLog.Tables[1].Rows[0]["CheckListTemplateName"].ToString();
                }
                //string JobHistoryMessage = "has removed a visit - <b class=\"blue-title\"> (" + jobSchedulingPopup.JobID + ") JobRefNo </b>";
                string JobHistoryMessage =  "has removed scheduled with "+ "Visit Id: <b style=\"color:black\">" + visitId + " </b>from installer: <b style=\"color:black\">" + username + "</b>"+ "with checklist template: <b style=\"color:black\">" + templateName+ "</b>"+ " ,VisiteDate: <b style=\"color:black\">" + jobSchedulingPopup.strVisitStartDate + " - " +  jobSchedulingPopup.strVisitStartTime + "</b> .";
                Common.SaveJobHistorytoXML(jobSchedulingPopup.JobID, JobHistoryMessage, "Scheduling", "RemovedSchedule", ProjectSession.LoggedInName, false,null);
            }
            return Json(new { jobSchedulingId = jobSchedulingId, installerDesignerId = jobSchedulingPopup.InstallerDesignerId, seStatus = jobSchedulingPopup.SEStatus }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the job stages.
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetJobStages()
        {
            List<SelectListItem> items = _jobBAL.GetJobStage().Select(a => new SelectListItem { Text = a.StageName, Value = a.JobStageId.ToString() }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Search Jobs For Scheduler
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchText"></param>
        /// <param name="jobStage"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="Unscheduled"></param>
        /// <param name="All"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult SearchJobsForScheduler(string PageIndex, string PageSize, string SortCol = "Title", string SortDir = "asc", string fromDate = "", string toDate = "", string searchText = "", string jobStage = "", string solarcompanyid = "", string Unscheduled = "0", string All = "0", string solarElectricianId = "")
        {
            int pI = Convert.ToInt32(PageIndex);
            int pS = Convert.ToInt32(PageSize);
            int JobStage = !string.IsNullOrEmpty(jobStage) ? Convert.ToInt32(jobStage) : 0;
            int SolarCompanyId = 0;
            bool NotScheduled = Unscheduled == "0" ? false : true;
            bool all = All == "0" ? false : true;
            if (ProjectSession.UserTypeId == 4)
            {
                SolarCompanyId = ProjectSession.SolarCompanyId;
            }
            else
            {
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }
            DateTime? FromDate = null, ToDate = null;
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                FromDate = Convert.ToDateTime(fromDate);
                ToDate = Convert.ToDateTime(toDate);
            }

            //IList<FormBot.Entity.JobList> lstJobs = _jobScheduling.SearchJobsForScheduler(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ProjectSession.ResellerId, SolarCompanyId, pI, pS, SortCol, SortDir, FromDate, ToDate, searchText, JobStage, NotScheduled, all);
            IList<FormBot.Entity.JobList> lstJobs = _jobScheduling.SearchJobsForSchedulerWithSE(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ProjectSession.ResellerId, SolarCompanyId, pI, pS, SortCol, SortDir, FromDate, ToDate, searchText, JobStage, NotScheduled, all, solarElectricianId);

            StringBuilder sb = new StringBuilder();
            if (lstJobs != null && lstJobs.Count > 0)
            {
                foreach (JobList job in lstJobs)
                {
                    sb.Append("<li class='findJobDraggable' style='z-index:10;' jobID=\"" + job.JobID + "\" jobTitle=\"" + job.RefNumber + "\" jobType =\"" + job.JobTypeId + "\" IsClassic=\"" + job.IsClassic + "\" >");
                    sb.Append("<span class='job_status completed'>" + job.JobStage + "</span>");
                    sb.Append("<a href='#' class='mtitle'>" + job.JobID + " - " + job.RefNumber + "</a>");
                    //sb.Append("<a href='#' class='mtitle'>" + job.RefNumber + "</a>");
                    sb.Append(job.ClientName);
                    if (job.JobTypeId == 1)
                        sb.Append("<br/><span>JobType: PVD<span>");
                    else if (job.JobTypeId == 2)
                        sb.Append("<br/>JobType: SWH");

                    if (job.StartDate != null)
                    {
                        string scheduleDetails = job.StartDate.Value.ToString(ProjectConfiguration.GetDateFormat.Replace('m', 'M')) + "-" + job.StartTime.Value.ToString(@"hh\:mm");
                        if (job.EndDate != null)
                        {
                            scheduleDetails += " To " + job.EndDate.Value.ToString(ProjectConfiguration.GetDateFormat.Replace('m', 'M'));
                            if (job.EndTime != null)
                                scheduleDetails += "-" + job.EndTime.Value.ToString(@"hh\:mm");
                        }

                        sb.Append("<span class='timing'>Schedule: " + scheduleDetails + "</span>");
                    }
                    else
                    {
                        sb.Append("<span class='timing'>Schedule: Unscheduled</span>");
                    }
                    //<span class="timing">Schedule: 9:15 - 10:15  6 JUN</span>
                    sb.Append("</li>");
                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Calendar data using solar compnay id when user is (RA, RAM, FSA, FCO).
        /// </summary>
        /// <param name="solarCompanyId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetAdminData(string solarCompanyId, string solarElectricianId = "")
        {
            JobScheduling jobScheduling = new JobScheduling();

            if (!string.IsNullOrEmpty(solarCompanyId))
                jobScheduling = SetCalendarData(jobScheduling, ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, Convert.ToInt32(solarCompanyId),solarElectricianId);

            return Json(jobScheduling, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult MakeVisitAsDefaultSubmission(string jobId, string jobSchedulingId, bool isDefault)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                _jobScheduling.MakeVisitAsDefaultSubmission(!string.IsNullOrEmpty(jobId) ? Convert.ToInt32(jobId) : 0, !string.IsNullOrEmpty(jobSchedulingId) ? Convert.ToInt32(jobSchedulingId) : 0, isDefault);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult ChangeVisitStatus(string jobSchedulingId, string visitStatus)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                DateTime? completedDate = null;
                string visitCompletedDate = string.Empty;

                DataSet dsSCAInfo = _jobScheduling.ChangeVisitStatus(!string.IsNullOrEmpty(jobSchedulingId) ? Convert.ToInt32(jobSchedulingId) : 0, !string.IsNullOrEmpty(visitStatus) ? Convert.ToInt32(visitStatus) : 0, completedDate);
                int intjobschedulingId = !string.IsNullOrEmpty(jobSchedulingId) ? Convert.ToInt32(jobSchedulingId) : 0;
                DataSet dataForHistoryLog = _jobScheduling.GetJobSchedulingDataForHistoryLog(intjobschedulingId, null);
                string visitId = string.Empty;
                string JobID = string.Empty;
                if (dataForHistoryLog != null && dataForHistoryLog.Tables.Count > 0)
                {
                    visitId = dataForHistoryLog.Tables[0].Rows[0]["VisitUniqueId"]
                        .ToString();
                    JobID = dataForHistoryLog.Tables[0].Rows[0]["JobID"].ToString();
                }
                string JobHistoryMessage = string.Empty;
                string description = string.Empty;
                
                if (visitStatus == "2")
                {
                    completedDate = DateTime.Now;
                    visitCompletedDate = Convert.ToDateTime(completedDate).ToString("dd/MM/yyyy hh:mm tt");

                    if (dsSCAInfo != null && dsSCAInfo.Tables.Count > 0 && dsSCAInfo.Tables[0] != null && dsSCAInfo.Tables[0].Rows.Count > 0)
                    {
                        EmailInfo emailInfo = new EmailInfo();
                        
                        emailInfo = dsSCAInfo.Tables[0].Rows.Count > 0 ? DBClient.DataTableToList<EmailInfo>(dsSCAInfo.Tables[0])[0] : new EmailInfo();
                        emailInfo.JobDetailLink = ProjectSession.LoginLink + Url.Action("Job", "Index") + "?id=" + emailInfo.Id;
                        //live or staging
                        emailInfo.TemplateID = 38;
                        //local
                        //emailInfo.TemplateID = 1037;

                        //emailInfo.SolarCompanyDetails = Convert.ToString(dsSCAInfo.Tables[0].Rows[0]["Name"]);
                        _emailBAL.ComposeAndSendEmail(emailInfo, emailInfo.Email, null, null, default(Guid), Convert.ToString(JobID));
                    }
                    //JobHistoryMessage = "has completed a visit -<b class=\"blue-title\"> (" + JobID + ") JobRefNo</b>";
                    JobHistoryMessage = "has changed visit status from Open to Completed for Visit id: <b class=\"blue-title\">"+visitId+"</b>";
                    Common.SaveJobHistorytoXML(Convert.ToInt32(JobID), JobHistoryMessage, "Scheduling", "ChangeVisitStatus", ProjectSession.LoggedInName, false, null);
                }

                // _jobScheduling.ChangeVisitStatus(!string.IsNullOrEmpty(jobSchedulingId) ? Convert.ToInt32(jobSchedulingId) : 0, !string.IsNullOrEmpty(visitStatus) ? Convert.ToInt32(visitStatus) : 0, completedDate);
                else if(visitStatus=="1")
                {
                    JobHistoryMessage = "has changed visit status from Completed to Open for Visit id: <b class=\"blue-title\">" + visitId + "</b>";
                    Common.SaveJobHistorytoXML(Convert.ToInt32(JobID), JobHistoryMessage, "Scheduling", "ChangeVisitStatus", ProjectSession.LoggedInName, false, null);
                    //JobHistoryMessage = "has re-opened a visit -<b class=\"blue-title\"> (" + JobID + ") JobRefNo</b>";
                    //description = "Visit id: <b class=\"blue-title\">" + visitId + "</b> visit status has been changed to open.";
                    //Common.SaveJobHistorytoXML(Convert.ToInt32(JobID), JobHistoryMessage, "Scheduling", "ChangeVisitStatus", ProjectSession.LoggedInName, false, description);
                }
                return Json(new { status = true, completedDate = visitCompletedDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GenerateRandomNumber()
        {
            try
            {
                Random r = new Random();
                int rInt = r.Next(Int32.MinValue, -1);

                return Json(new { status = true, randomNumber = rInt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Solar electrician for job scheduling 
        /// </summary>
        /// <param name="resellerID"></param>
        /// <param name="solarEletrician"></param>
        /// <returns>list fo solar electrician</returns>
         [HttpGet]
        public JsonResult GetSolarElectrician(int solarCompanyID = 0)
        {
            List<SelectListItem> lstSolarElectrician = _jobScheduling.GetSolarElectrician(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, Convert.ToInt32(solarCompanyID)).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            
            return Json(lstSolarElectrician, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
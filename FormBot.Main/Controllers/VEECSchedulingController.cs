using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.BAL.Service.VEEC;
using FormBot.Email;
using FormBot.Entity;
using FormBot.Entity.Email;
using FormBot.Entity.VEEC;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class VEECSchedulingController : Controller
    {

        #region Properties
        private readonly IVEECSchedulingBAL _veecScheduling;
        //private readonly IJobDetailsBAL _jobDetails;
        //private readonly ISolarElectricianBAL _solarElectrician;
        private readonly ICreateVeecBAL _jobBAL;
        private readonly ICreateJobHistoryBAL _jobHistory;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;

        #endregion

        // GET: VEECScheduling
        public ActionResult Index()
        {
            return View();
        }

        #region Constructor

        public VEECSchedulingController(IVEECSchedulingBAL jobScheduling, ICreateVeecBAL jobBAL, ICreateJobHistoryBAL jobHistory, IUserBAL userBAL, IEmailBAL emailBAL)
        {
            this._veecScheduling = jobScheduling;
            //this._jobDetails = jobDetails;
            //this._solarElectrician = solarElectrician;
            this._jobBAL = jobBAL;
            this._jobHistory = jobHistory;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
        }
        #endregion

        [HttpGet]
        [UserAuthorization]
        public ActionResult VeecSchedulingDetail(string id = null, bool isCheckListView = true, bool isReloadGridView = true)
        {
            //WriteToLogFile("StartTime (Get JobSchedulingDetail) :" + DateTime.Now);

            if (!string.IsNullOrEmpty(id))
            {
                VEECScheduling veecScheduling = new VEECScheduling();
                veecScheduling = _veecScheduling.GetAllSchedulingDataOfVEEC(id, isCheckListView, isReloadGridView, _jobBAL);

                //WriteToLogFile("EndTime (Get JobSchedulingDetail) :" + DateTime.Now);

                if (isCheckListView)
                {
                    if (isReloadGridView)
                        return PartialView("_VisitGridView", veecScheduling);
                    else
                        return PartialView("_VeecVisit", veecScheduling);
                }
                else
                    return PartialView("_VeecScheduling", veecScheduling);
            }
            return View();
            //else
            //{
            //    JobScheduling jobScheduling = new JobScheduling();
            //    int solarCompanyId = ProjectSession.SolarCompanyId;
            //    int userTypeId = ProjectSession.UserTypeId;
            //    int userId = ProjectSession.LoggedInUserId;
            //    jobScheduling.IsFromCalendarView = true;
            //    //jobScheduling.UserId = ProjectSession.LoggedInUserId;
            //    //jobScheduling.UserTypeID = ProjectSession.UserTypeId;
            //    //jobScheduling.ResellerID = ProjectSession.ResellerId;
            //    //jobScheduling.SolarCompanyId = ProjectSession.SolarCompanyId;

            //    if ((userTypeId != 2 && userTypeId != 5 && userTypeId != 1 && userTypeId != 3))
            //    {
            //        jobScheduling = SetCalendarData(jobScheduling, userId, userTypeId, solarCompanyId);
            //    }
            //    else
            //    {
            //        jobScheduling.solarElectrician = new List<SolarElectricianView>();
            //        jobScheduling.job = new List<JobDetails>();
            //        jobScheduling.jobSchedulingData = string.Empty;
            //        //jobScheduling.jobSchedulingData = new List<JobScheduling>();
            //    }
            //    jobScheduling.IsDashboard = false;

            //    //if (isCheckListView)
            //    //    return PartialView("_JobVisit", jobScheduling);
            //    //else
            //    //    return View(jobScheduling);

            //    return View(jobScheduling);

            //    //return View(jobScheduling);
            //}
        }


        [HttpPost]
        [UserAuthorization]
        public JsonResult VeecSchedulingDetail(VEECSchedulingPopup veecSchedulingPopup)
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
                    VEECScheduling veecScheduling = new VEECScheduling();

                    int solarCompanyId = 0;
                    if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                        solarCompanyId = ProjectSession.SolarCompanyId;
                    else
                        solarCompanyId = veecSchedulingPopup.SolarCompanyId;


                    int userTypeId = ProjectSession.UserTypeId;

                    int UserId = 0;
                    int JobId = 0;
                    string JobName = "";
                    string eleFirstName = string.Empty;
                    string eleLastName = string.Empty;
                    string eleEmail = string.Empty;

                    int veecCheckListTemplateId = 0;

                    DateTime? dtEndDate = null;
                    TimeSpan? timeEndTime = null;
                    if (!string.IsNullOrEmpty(veecSchedulingPopup.strVisitEndDate))
                    {
                        dtEndDate = Convert.ToDateTime(veecSchedulingPopup.strVisitEndDate);
                    }
                    if (!string.IsNullOrEmpty(veecSchedulingPopup.strVisitEndTime))
                    {
                        timeEndTime = TimeSpan.Parse(veecSchedulingPopup.strVisitEndTime);
                    }

                    if (!string.IsNullOrEmpty(veecSchedulingPopup.TemplateId))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(veecSchedulingPopup.TemplateId, "id"), out veecCheckListTemplateId);
                    }

                    if (Convert.ToInt32(veecSchedulingPopup.VeecSchedulingID) > 0)
                    {

                        if (Convert.ToBoolean(veecSchedulingPopup.isDrop))
                        {
                            //update job scheduling data on drag-drop
                            int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Pending);
                            jobSchedulingDetail = _veecScheduling.VeecScheduling_InsertUpdateScheduling(Convert.ToInt32(veecSchedulingPopup.VeecSchedulingID), Convert.ToInt32(0), Convert.ToInt32(veecSchedulingPopup.UserId),
                            null, null, Convert.ToDateTime(veecSchedulingPopup.strVisitStartDate), TimeSpan.Parse(veecSchedulingPopup.strVisitStartTime), dtEndDate, new TimeSpan(),
                            DateTime.Now, createdBy, DateTime.Now, createdBy, false, scheduleStatus, Convert.ToBoolean(veecSchedulingPopup.isNotification), Convert.ToBoolean(veecSchedulingPopup.isDrop), solarCompanyId, userTypeId, veecCheckListTemplateId, veecSchedulingPopup.VeecVisitCheckListItemIds, veecSchedulingPopup.TempVeecSchedulingId, veecSchedulingPopup.IsFromCalendarView);
                        }
                        else
                        {
                            //update job scheduling data
                            int scheduleStatus = Convert.ToInt32(veecSchedulingPopup.Status);
                            jobSchedulingDetail = _veecScheduling.VeecScheduling_InsertUpdateScheduling(Convert.ToInt32(veecSchedulingPopup.VeecSchedulingID), Convert.ToInt32(veecSchedulingPopup.VeecID), Convert.ToInt32(veecSchedulingPopup.UserId),
                           !string.IsNullOrEmpty(veecSchedulingPopup.Label) ? veecSchedulingPopup.Label.Trim() : veecSchedulingPopup.Label, !string.IsNullOrEmpty(veecSchedulingPopup.Detail) ? veecSchedulingPopup.Detail.Trim() : veecSchedulingPopup.Detail, Convert.ToDateTime(veecSchedulingPopup.strVisitStartDate), TimeSpan.Parse(veecSchedulingPopup.strVisitStartTime), dtEndDate, timeEndTime,
                           DateTime.Now, createdBy, DateTime.Now, createdBy, false, scheduleStatus, Convert.ToBoolean(veecSchedulingPopup.isNotification), Convert.ToBoolean(veecSchedulingPopup.isDrop), solarCompanyId, userTypeId, veecCheckListTemplateId, veecSchedulingPopup.VeecVisitCheckListItemIds, veecSchedulingPopup.TempVeecSchedulingId, veecSchedulingPopup.IsFromCalendarView);
                        }
                        //if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0 && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        //{
                        //    if (Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]) != -1)
                        //    {
                        //        bool isHistorySaved = _jobHistory.LogJobHistory(veecSchedulingPopup, HistoryCategory.Rescheduled);
                        //    }
                        //}
                    }
                    else
                    {
                        //insert
                        int scheduleStatus = Convert.ToInt32(FormBot.Helper.SystemEnums.JobSchedulingStatus.Pending);
                        jobSchedulingDetail = _veecScheduling.VeecScheduling_InsertUpdateScheduling(0, Convert.ToInt32(veecSchedulingPopup.VeecID), Convert.ToInt32(veecSchedulingPopup.UserId),
                        !string.IsNullOrEmpty(veecSchedulingPopup.Label) ? veecSchedulingPopup.Label.Trim() : veecSchedulingPopup.Label, !string.IsNullOrEmpty(veecSchedulingPopup.Detail) ? veecSchedulingPopup.Detail.Trim() : veecSchedulingPopup.Detail, Convert.ToDateTime(veecSchedulingPopup.strVisitStartDate), TimeSpan.Parse(veecSchedulingPopup.strVisitStartTime), dtEndDate, timeEndTime,
                        DateTime.Now, createdBy, null, null, false, scheduleStatus, Convert.ToBoolean(veecSchedulingPopup.isNotification), Convert.ToBoolean(veecSchedulingPopup.isDrop), solarCompanyId, userTypeId, veecCheckListTemplateId, veecSchedulingPopup.VeecVisitCheckListItemIds, veecSchedulingPopup.TempVeecSchedulingId, veecSchedulingPopup.IsFromCalendarView);
                        //if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0 && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        //{
                        //    if (Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["jobSchedulingResponse"]) != -1)
                        //    {
                        //        bool isHistorySaved = _jobHistory.LogJobHistory(veecSchedulingPopup, HistoryCategory.CreateSchedule);
                        //    }
                        //}
                    }

                    if (jobSchedulingDetail != null && jobSchedulingDetail.Tables.Count > 0)
                    {
                        if (jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                        {
                            jobSchedulingId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["veecSchedulingResponse"]);

                            if (jobSchedulingDetail.Tables[1] != null && jobSchedulingDetail.Tables[1].Rows.Count > 0)
                            {
                                UserId = Convert.ToInt32(jobSchedulingDetail.Tables[1].Rows[0]["UserId"]);
                                JobId = Convert.ToInt32(jobSchedulingDetail.Tables[1].Rows[0]["VeecId"]);

                                eleFirstName = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["FirstName"]);
                                eleLastName = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["LastName"]);
                                eleEmail = Convert.ToString(jobSchedulingDetail.Tables[1].Rows[0]["Email"]);
                            }

                            //if (jobSchedulingDetail.Tables[2] != null && jobSchedulingDetail.Tables[2].Rows.Count > 0)
                            //{
                            //    JobName = jobSchedulingDetail.Tables[2].Rows[0]["Header"].ToString();
                            //}

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
                                if (Convert.ToInt32(veecSchedulingPopup.VeecSchedulingID) > 0)
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
                            //if (jobSchedulingId > 0)
                            //{
                            //    //_pushNotificationBAL.SendPushNotification(jobSchedulingPopup.UserId, jobScheduling.JobTitle +" - has been schedule");

                            //    //WriteToLogFile("4. StartTime (SendPushNotification) :" + DateTime.Now);
                            //    var syncTask = new Task(() =>
                            //    {
                            //        new PushNotification().SendPushNotification(veecSchedulingPopup.UserId, "Job '" + veecSchedulingPopup.veecTitle + "' has been scheduled");
                            //    });
                            //    syncTask.RunSynchronously();
                            //    //WriteToLogFile("5. EndTime (SendPushNotification) :" + DateTime.Now);

                            //}
                        }

                        if (jobSchedulingDetail.Tables.Count > 3 && jobSchedulingDetail.Tables[3] != null && jobSchedulingDetail.Tables[3].Rows.Count > 0)
                        {
                            jobSchedulingData = Newtonsoft.Json.JsonConvert.SerializeObject(jobSchedulingDetail.Tables[3]);
                        }
                    }

                    CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, JobId);
                    //WriteToLogFile("6. EndTime :" + DateTime.Now);
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



        public ActionResult VeecSchedulingDetailById(string veecSchedulingID)
        {
            VEECScheduling jobScheduling = new VEECScheduling();
            try
            {
                if (Convert.ToInt32(veecSchedulingID) > 0)
                {
                    DataSet jobSchedulingDetail = _veecScheduling.GetVeecSchedulingDetail(Convert.ToInt32(veecSchedulingID));
                    if (jobSchedulingDetail != null && jobSchedulingDetail.Tables[0] != null && jobSchedulingDetail.Tables[0].Rows.Count > 0)
                    {
                        jobScheduling.VeecID = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["VeecId"]);
                        jobScheduling.VeecSchedulingID = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["VeecSchedulingID"]);
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
                        jobScheduling.Status = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["VeecSchedulingStatus"]);
                        //jobScheduling.TotalItemCount = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["TotalItemCount"]);
                        jobScheduling.DefaultVEECCheckListTemplateId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["DefaultVeecCheckListTemplateId"]);
                        //jobScheduling.CheckListTemplateId = Convert.ToInt32(!string.IsNullOrEmpty(jobSchedulingDetail.Tables[0].Rows[0]["CheckListTemplateId"] );

                        jobScheduling.VEECCheckListTemplateId = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["VeecCheckListTemplateId"]);
                        //jobScheduling.IsClassic = Convert.ToBoolean(jobSchedulingDetail.Tables[0].Rows[0]["IsClassic"]);
                        //jobScheduling.JobType = Convert.ToInt32(jobSchedulingDetail.Tables[0].Rows[0]["JobType"]);

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


        public JsonResult GetSolarElectricianForVeecType(int SolarCompanyId = 0)
        {
            int companyid = 0;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 6 || ProjectSession.UserTypeId == 8)
                companyid = ProjectSession.SolarCompanyId;
            else
                companyid = SolarCompanyId;

            List<SelectListItem> items = _veecScheduling.GetSolarElectricianForVeecType(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, companyid).Select(a => new SelectListItem { Text = a.Name, Value = a.UserId.ToString() }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult MakeVisitAsDefaultSubmission(string veecId, string veecSchedulingId, bool isDefault)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                _veecScheduling.MakeVisitAsDefaultSubmission(!string.IsNullOrEmpty(veecId) ? Convert.ToInt32(veecId) : 0, !string.IsNullOrEmpty(veecSchedulingId) ? Convert.ToInt32(veecSchedulingId) : 0, isDefault);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //[UserAuthorization]
        //public JsonResult DeleteJobSchedule(string jobSchedulingID, string userId, string jobTitle)
        //{
        //    int jobSchedulingId = 0;
        //    int UserId = 0;
        //    int JobId = 0;
        //    string JobName = "";
        //    JobSchedulingPopup jobSchedulingPopup = new JobSchedulingPopup();

        //    if (Convert.ToInt32(jobSchedulingID) > 0)
        //    {
        //        DataSet dsJobSchedulingDetail = _jobScheduling.DeleteSchedule(Convert.ToInt32(jobSchedulingID));
        //        if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables.Count > 0)
        //        {
        //            if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[0] != null && dsJobSchedulingDetail.Tables[0].Rows.Count > 0)
        //            {
        //                jobSchedulingId = Convert.ToInt32(dsJobSchedulingDetail.Tables[0].Rows[0]["JobSchedulingId"]);
        //            }
        //            if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[1] != null && dsJobSchedulingDetail.Tables[1].Rows.Count > 0)
        //            {
        //                UserId = Convert.ToInt32(dsJobSchedulingDetail.Tables[1].Rows[0]["UserId"]);
        //                JobId = Convert.ToInt32(dsJobSchedulingDetail.Tables[1].Rows[0]["JobId"]);
        //                jobSchedulingPopup.JobID = JobId;
        //            }
        //            if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[2] != null && dsJobSchedulingDetail.Tables[2].Rows.Count > 0)
        //            {
        //                JobName = dsJobSchedulingDetail.Tables[2].Rows[0]["Header"].ToString();
        //            }
        //            if (dsJobSchedulingDetail != null && dsJobSchedulingDetail.Tables[3] != null && dsJobSchedulingDetail.Tables[3].Rows.Count > 0)
        //            {
        //                jobSchedulingPopup.strVisitStartDate = dsJobSchedulingDetail.Tables[3].Rows[0]["StartDate"].ToString();
        //                jobSchedulingPopup.strVisitStartTime = dsJobSchedulingDetail.Tables[3].Rows[0]["StartTime"].ToString();
        //                jobSchedulingPopup.UserId = Convert.ToInt32(dsJobSchedulingDetail.Tables[3].Rows[0]["UserId"]);
        //            }
        //        }
        //        //mail
        //        if (UserId > 0 && JobId > 0)
        //        {
        //            var dsToUsers = _userBAL.GetUserById(UserId);
        //            FormBot.Entity.User toUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsToUsers.Tables[0]).FirstOrDefault();

        //            var dsFromUsers = _userBAL.GetUserById(ProjectSession.LoggedInUserId);
        //            FormBot.Entity.User FromUserDetail = DBClient.DataTableToList<FormBot.Entity.User>(dsFromUsers.Tables[0]).FirstOrDefault();

        //            EmailInfo emailinfo = new EmailInfo();

        //            emailinfo.JobName = JobName;
        //            emailinfo.TemplateID = 20;

        //            if (dsToUsers != null && dsFromUsers != null && dsToUsers.Tables[0].Rows.Count > 0 && dsFromUsers.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(toUserDetail.Email) && !string.IsNullOrEmpty(FromUserDetail.Email))
        //            {
        //                emailinfo.FirstName = toUserDetail.FirstName;
        //                emailinfo.LastName = toUserDetail.LastName;

        //                _emailBAL.ComposeAndSendEmail(emailinfo, toUserDetail.Email);
        //                jobSchedulingPopup.UserName = toUserDetail.FirstName + ' ' + toUserDetail.LastName;
        //            }
        //        }
        //        //Send notification to assign user
        //        if (Convert.ToInt32(jobSchedulingID) > 0 && Convert.ToInt32(userId) > 0)
        //        {
        //            try
        //            {
        //                //_pushNotificationBAL.SendPushNotification(Convert.ToInt32(userId),jobSchedulingPopup.JobTitle+" - has been deleted.");
        //                new PushNotification().SendPushNotification(Convert.ToInt32(userId), "Job '" + jobTitle + "' has been deleted.");
        //            }
        //            catch (Exception ex)
        //            {
        //                WriteToLogFile(ex.Message);
        //                WriteToLogFile(ex.StackTrace);
        //            }
        //        }

        //        bool isHistorySaved = _jobHistory.LogJobHistory(jobSchedulingPopup, HistoryCategory.RemovedSchedule);
        //    }
        //    return Json(jobSchedulingId, JsonRequestBehavior.AllowGet);
        //}



        public JsonResult ChangeVisitStatus(string veecSchedulingId, string visitStatus)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                DateTime? completedDate = null;
                string visitCompletedDate = string.Empty;
				if(visitStatus == "2")
					completedDate = DateTime.Now;
				DataSet dsSCAInfo = _veecScheduling.ChangeVisitStatus(!string.IsNullOrEmpty(veecSchedulingId) ? Convert.ToInt32(veecSchedulingId) : 0, !string.IsNullOrEmpty(visitStatus) ? Convert.ToInt32(visitStatus) : 0, completedDate);

                if (visitStatus == "2")
                {
                    visitCompletedDate = Convert.ToDateTime(completedDate).ToString("dd/MM/yyyy hh:mm tt");
                    if (dsSCAInfo != null && dsSCAInfo.Tables.Count > 0 && dsSCAInfo.Tables[0] != null && dsSCAInfo.Tables[0].Rows.Count > 0)
                    {
                        EmailInfo emailInfo = new EmailInfo();
                        emailInfo.TemplateID = 37;
                        emailInfo.SolarCompanyDetails = Convert.ToString(dsSCAInfo.Tables[0].Rows[0]["Name"]);
                        _emailBAL.ComposeAndSendEmail(emailInfo, Convert.ToString(dsSCAInfo.Tables[0].Rows[0]["Email"]));
                    }
                }

                // _jobScheduling.ChangeVisitStatus(!string.IsNullOrEmpty(jobSchedulingId) ? Convert.ToInt32(jobSchedulingId) : 0, !string.IsNullOrEmpty(visitStatus) ? Convert.ToInt32(visitStatus) : 0, completedDate);

                return Json(new { status = true, completedDate = visitCompletedDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
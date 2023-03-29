using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.Entity;
using FormBot.Entity.Email;
using FormBot.Helper;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace FormBot.Main.Controllers
{
    public class AssignJobToSCOController : Controller
    {
        #region Properties

        private readonly ICreateJobBAL _job;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        private readonly ICreateJobHistoryBAL _jobHistory;
        #endregion

        #region Constructor

        public AssignJobToSCOController(ICreateJobBAL job, IUserBAL userBAL, IEmailBAL emailBAL, ICreateJobHistoryBAL jobHistory)
        {
            this._job = job;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
            this._jobHistory = jobHistory;
        }

        #endregion

        /// <summary>
        /// Assigns the SCO.
        /// </summary>
        /// <returns>view sco</returns>
        public ActionResult AssignSCO(string jobIds = null, int solarcomapnyId = 0)
        {
            var model = new AssignSCO();
            //var lstAssignedFCOGroupList = _job.GetAssignJobToSCOList(ProjectSession.LoggedInUserId, 0);
            //if (lstAssignedFCOGroupList.Count > 0)
            //{
            //    var lstUserList = _job.GetAllJobToSCO(ProjectSession.LoggedInUserId);
            //    model.LstJobSCOUser = lstUserList;
            //    model.LstJobSCOAssignedUser = lstAssignedFCOGroupList;
            //    model.JobSCOAssignedUser = lstAssignedFCOGroupList.Select(d => d.Value).ToArray();
            //}
            //else
            //{
            //    var lstUserList = _job.GetAllJobToSCO(ProjectSession.LoggedInUserId);
            //    model.LstJobSCOUser = lstUserList;
            //    model.LstJobSCOAssignedUser = new List<SelectListItem>();
            //}
            var lstUserList = _job.GetAllJobsToAssignSCO(jobIds);
            model.LstJobSCOUser = lstUserList;
            model.SolarCompanyId = solarcomapnyId;
            model.LstJobSCOAssignedUser = new List<SelectListItem>();
            return View(model);
        }

        /// <summary>
        /// Saves the job to sco.
        /// </summary>
        /// <param name="ramId">The ram identifier.</param>
        /// <param name="assignedsc">The assignedsc.</param>
        /// <returns>success or error msg.</returns>
        [HttpGet]
        public async Task<ActionResult> SaveJobToSCO(string scoId, string jobIds, int solarcompanyId)
        {
            //try
            //{
            if (Convert.ToInt32(scoId) > 0)
            {
                _job.CreateJobSCOMapping(Convert.ToInt32(scoId), jobIds);
                string[] jobs = jobIds.Split(',');
                for (int i = 0; i < jobs.Length; i++)
                {
                    await CommonBAL.SetCacheDataForJobID(solarcompanyId, Convert.ToInt32(jobs[i]));
                }
                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }

        /// <summary>
        /// Gets the sco user.
        /// </summary>
        /// <returns>Json Result</returns>
        [HttpGet]
        public JsonResult GetSCOUser(int solarcompanyId)
        {
            List<SelectListItem> Items = _job.GetSCOUser(solarcompanyId).Select(a => new SelectListItem { Text = a.FirstName + ' ' + a.LastName, Value = a.UserID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the job to sco chanage drop down.
        /// </summary>
        /// <param name="scoID">The sco identifier.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetJobToSCOChanageDropDown(string scoID)
        {
            var model = new AssignSCO();
            if (scoID != "")
            {
                model.nodeListAssigned = _job.GetAssignJobToSCOList(ProjectSession.LoggedInUserId, Convert.ToInt32(scoID));
                model.nodeList = _job.GetAllJobToSCO(ProjectSession.LoggedInUserId);
                return this.Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.nodeListAssigned = _job.GetAssignJobToSCOList(ProjectSession.LoggedInUserId, 0);
                model.nodeList = _job.GetAllJobToSCO(ProjectSession.LoggedInUserId);
                return this.Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult _AssignJobSSC()
        {
            return View();
        }

        /// <summary>
        /// Gets the ssc user.
        /// </summary>
        /// <returns>Json Result</returns>
        public JsonResult GetSSCUser()
        {
            List<SelectListItem> Items = _job.GetSSCUser().Select(a => new SelectListItem { Text = a.FirstName + ' ' + a.LastName, Value = a.UserID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the sscid.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Json Result</returns>
        public JsonResult GetSSCID(string id)
        {
            int sscJOBID = 0;
            if (!string.IsNullOrEmpty(id))
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out sscJOBID);
            List<SolarSubContractor> SSCID = _job.GetSSCID(sscJOBID);
            if (SSCID.Count > 0)
            {
                string date = SSCID[0].SSCRemoveDate.Hour >= 12 ? "PM AEST" : "AM AEST";
                SSCID[0].RequestedBy = SSCID[0].RequestedBy + " " + SSCID[0].SSCRemoveDate.ToShortDateString() + " " + SSCID[0].SSCRemoveDate.Hour + ":" + SSCID[0].SSCRemoveDate.Minute + date;
            }
            return Json(SSCID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the SSC user by jb identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Json Result</returns>
        public JsonResult GetSSCUserByJbID(string id)
        {
            int sscJOBID = 0;
            if (!string.IsNullOrEmpty(id))
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out sscJOBID);
            List<SelectListItem> Items = _job.GetSSCUserByJbID(sscJOBID, ProjectSession.SolarCompanyId).Select(a => new SelectListItem { Text = a.FirstName + ' ' + a.LastName, Value = a.UserID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the SSC user in edit.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>Json Result</returns>
        public JsonResult GetSSCUserInEdit(string jobID)
        {
            int id = Convert.ToInt32(jobID);
            List<SelectListItem> Items = _job.GetSSCUserByJbID(id, ProjectSession.SolarCompanyId).Select(a => new SelectListItem { Text = a.FirstName + ' ' + a.LastName, Value = a.UserID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the SSC user in dropdown.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>Json Result</returns>
        public JsonResult GetSSCUserInDropdown(string jobID)
        {
            int id = Convert.ToInt32(jobID);
            List<SolarSubContractor> SSCID = _job.GetSSCID(id);
            return Json(SSCID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the job to SSC.
        /// </summary>
        /// <param name="ramId">The ram identifier.</param>
        /// <param name="jobId">The Jobid.</param>
        /// <param name="SSCName">The SSC Name.</param>
        /// <returns>Json Result</returns>
        [HttpGet]
        public async Task<ActionResult> SaveJobToSSC(string ramId, string jobId, string SSCName)
        {
            try
            {
                int sscJOBID = 0;
                if (!string.IsNullOrEmpty(jobId))
                    int.TryParse(QueryString.GetValueFromQueryString(jobId, "id"), out sscJOBID);
                if (Convert.ToInt32(ramId) > 0)
                {
                    int jobID = sscJOBID;
                    _job.CreateJobSSCMapping(Convert.ToInt32(ramId), jobID);
                    ContractorHistory objContractor = new ContractorHistory()
                    {
                        JobID = sscJOBID,
                        SolarCompanyName = ProjectSession.LoginCompanyName != string.Empty ? ProjectSession.LoginCompanyName : "Solar Company",
                        SSCName = SSCName
                    };

                    await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, jobID);

                    //bool isHistorySaved = _jobHistory.LogJobHistory(objContractor, HistoryCategory.SCASentJob);
                    string SolarCompanyname = objContractor.SolarCompanyName;
                    string SSCname = objContractor.SSCName;
                    string JobHistoryMessage = "<b class=\"blue-title\">" + SolarCompanyname + "</b> sent JobID "+objContractor.JobID +" to SSC-" + SSCname + " .";
                    Common.SaveJobHistorytoXML(objContractor.JobID, JobHistoryMessage, "General", "SCASentJob", ProjectSession.LoggedInName, false);

                    return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Removes the SSC request.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="sscID">The sscID.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public async Task<ActionResult> RemoveSSCRequest(string notes, string jobId, string sscID)
        {
            //try
            //{
            int sscJOBID = 0;
            if (!string.IsNullOrEmpty(jobId))
                int.TryParse(QueryString.GetValueFromQueryString(jobId, "id"), out sscJOBID);
            int jobID = sscJOBID;
            _job.RemoveSSCRequest(notes, jobID, ProjectSession.LoggedInUserId);
            this.SendEmailOnRemoveRequest(Convert.ToInt32(sscID), jobID);

            ContractorHistory objContractor = new ContractorHistory()
            {
                JobID = sscJOBID
            };

            await CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, jobID);

            //bool isHistorySaved = _jobHistory.LogJobHistory(objContractor, HistoryCategory.RemoveSSCRequest);
            string JobHistoryMessage = "has requested to remove SSC from JobID "+objContractor.JobID+" .";
           Common.SaveJobHistorytoXML(objContractor.JobID, JobHistoryMessage, "General", "RemoveSSCRequest", ProjectSession.LoggedInName, false);
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }

        public void SendEmailOnRemoveRequest(int sscID, int JobId)
        {
            string FailReason = string.Empty;
            EmailInfo emailInfo = new EmailInfo();

            try
            {
                DataSet usr = _userBAL.GetUserById(sscID);
                string firstName = string.Empty;
                string lastName = string.Empty;
                string email = string.Empty;
                if (usr != null && usr.Tables.Count > 0)
                {
                    firstName = Convert.ToString(usr.Tables[0].Rows[0]["FirstName"]);
                    lastName = Convert.ToString(usr.Tables[0].Rows[0]["LastName"]);
                    email = Convert.ToString(usr.Tables[0].Rows[0]["Email"]);
                }
                //FormBot.Entity.User uv = DBClient.ToListof<FormBot.Entity.User>(usr.Tables[0]).FirstOrDefault();

                emailInfo.TemplateID = 23;
                emailInfo.FirstName = firstName;
                emailInfo.LastName = lastName;
                //SMTPDetails smtpDetail = new SMTPDetails();
                //smtpDetail.MailFrom = ProjectSession.MailFrom;
                //smtpDetail.SMTPUserName = ProjectSession.SMTPUserName;
                //smtpDetail.SMTPPassword = ProjectSession.SMTPPassword;
                //smtpDetail.SMTPHost = ProjectSession.SMTPHost;
                //smtpDetail.SMTPPort = Convert.ToInt32(ProjectSession.SMTPPort);
                //smtpDetail.IsSMTPEnableSsl = ProjectSession.IsSMTPEnableSsl;

                EmailTemplate eTemplate = _emailBAL.GetEmailTemplateByID(23);
                eTemplate.Content = _emailBAL.GetEmailBody(emailInfo, eTemplate);
				bool status = false;

				if (eTemplate != null && !string.IsNullOrEmpty(eTemplate.Content))
				{
					QueuedEmail objQueuedEmail = new QueuedEmail();
					objQueuedEmail.FromEmail = ProjectSession.MailFrom;
					objQueuedEmail.Body = eTemplate.Content;
					objQueuedEmail.Subject = eTemplate.Subject;
					objQueuedEmail.ToEmail = email;
					objQueuedEmail.CreatedDate = DateTime.Now;
					objQueuedEmail.ModifiedDate = DateTime.Now;
                    objQueuedEmail.JobId = Convert.ToString(JobId);
                    _emailBAL.InsertUpdateQueuedEmail(objQueuedEmail);
					status = true;
				}
				//bool status = MailHelper.SendMail(smtpDetail, email, null, null, eTemplate.Subject, eTemplate.Content, null, true, ref FailReason, false);
			}
            catch (Exception ex)
            {

            }

        }
    }
}
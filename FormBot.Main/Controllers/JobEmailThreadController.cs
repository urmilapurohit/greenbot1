using System.Collections.Generic;
using System.Web.Mvc;
using FormBot.Email;
using FormBot.Entity.Email;
using FormBot.BAL.Service;
using System;
using System.Linq;
using FormBot.Helper;
using FormBot.Entity;
using FormBot.BAL.Service.Job;
using FormBot.Entity.Documents;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Web;
using FormBot.Helper.Helper;

namespace FormBot.Main.Controllers
{
    public class JobEmailThreadController : Controller
    {
        #region Properties
        /// <summary>
        /// The _email service
        /// </summary>
        private readonly IEmailBAL _emailService;
        /// <summary>
        /// The _job details
        /// </summary>
        private readonly IJobDetailsBAL _jobDetails;
        private readonly ICreateJobHistoryBAL _jobHistory;

        protected bool errorOccured = false;
        protected string name = string.Empty;
        protected string tmp_name = string.Empty;
        protected int size = 0;
        protected string mime_type = string.Empty;
        protected string error = string.Empty;
        protected string inlineImage = "false";
        protected string url = "";
        protected bool flashUpload = true;
        #endregion

        #region Constructor

        /// <summary>
        /// JobEmailThreadController
        /// </summary>
        /// <param name="emailService">emailService</param>
        /// <param name="jobDetails">jobDetails</param>
        /// <param name="jobHistory">jobHistory</param>
        public JobEmailThreadController(IEmailBAL emailService, IJobDetailsBAL jobDetails, ICreateJobHistoryBAL jobHistory)
        {
            this._emailService = emailService;
            this._jobDetails = jobDetails;
            this._jobHistory = jobHistory;
        }

        #endregion

        #region events

        /// <summary>
        /// Loads the message tab.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Action result</returns>
        public PartialViewResult LoadMessageTab(int jobId)
        {
            string loggedUserEmail = "";
            Account acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
            CheckMail checkMail = new CheckMail();
            if (acct != null)
            {
                loggedUserEmail = acct.Email;
                checkMail.AutoCheckMailForAccount();
            }

            ViewBag.LoggedUserEmail = loggedUserEmail;
            List<EmailThread> lstEmailThread = _emailService.GetEmailThreadByJobId(jobId, string.Empty, null, null, string.Empty, false, 1, Convert.ToInt32(FormBot.Helper.ProjectConfiguration.GetPageSizeForMail), ProjectSession.LoggedInUserId);
            if (lstEmailThread.Count > 0)
            {
                foreach (var item in lstEmailThread)
                {
                    item.FullDate = checkMail.DateFormattingGetDateWithoughtTimeZone(item.ModifiedDate);
                }
            }
            ViewBag.JobId = jobId;

            List<string> lstThreadMembers = _emailService.GetThreadMembers(jobId, ProjectSession.LoggedInUserId);
            List<SelectListItem> listItems = new List<SelectListItem>();

            foreach (var item in lstThreadMembers)
            {
                listItems.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }

            ViewBag.ThreadMembers = listItems;

            //List<JobWiseUsers> lstJobWiseUsers = _emailService.GetJobWiseUsers(jobId, ProjectSession.LoggedInUserId);
            //Session["JobWiseUsers"] = lstJobWiseUsers;
            //List<SelectListItem> jobWiseUsers = new List<SelectListItem>();
            //if (lstJobWiseUsers.Count > 0)
            //{
            //    lstJobWiseUsers = lstJobWiseUsers.Where(w => w.Email != null && w.Email != "" && w.Email != acct.Email).ToList();
            //    if (lstJobWiseUsers != null)
            //    {
            //        foreach (var item in lstJobWiseUsers)
            //        {
            //            if (!string.IsNullOrEmpty(item.Email))
            //            {
            //                jobWiseUsers.Add(new SelectListItem
            //                {
            //                    Text = !string.IsNullOrEmpty(item.FullName) ? item.FullName.First().ToString().ToUpper() + item.FullName.Substring(1) : item.FullName,
            //                    Value = item.Email
            //                });
            //            }
            //        }
            //    }

            //    jobWiseUsers.Add(new SelectListItem { Text = "Other", Value = "other" });
            //}

            //ViewBag.JobWiseUsers = jobWiseUsers;
            return this.PartialView("~/Views/JobEmailThread/Messages.cshtml", lstEmailThread);
        }

        /// <summary>
        /// Gets the messages on search.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="threadMembers">The thread members.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="title">The title.</param>
        /// <param name="isArchived">if set to <c>true</c> [is archived].</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>Action result</returns>
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GetMessagesOnSearch(int jobId, string threadMembers, string fromDate, string toDate, string title, bool isArchived = false, int pageIndex = 1)
        {
            CheckMail checkMail = new CheckMail();
            DateTime? dtfromDate = null;
            DateTime? dttoDate = null;
            if (!string.IsNullOrEmpty(fromDate))
            {
                dtfromDate = Convert.ToDateTime(fromDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                dttoDate = Convert.ToDateTime(toDate);
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.Replace("[", "[[]");
            }

            List<EmailThread> lstEmailThread = _emailService.GetEmailThreadByJobId(jobId, threadMembers, dtfromDate, dttoDate, title, isArchived, pageIndex, Convert.ToInt32(FormBot.Helper.ProjectConfiguration.GetPageSizeForMail), ProjectSession.LoggedInUserId);
            if (lstEmailThread.Count > 0)
            {
                foreach (var item in lstEmailThread)
                {
                    item.FullDate = checkMail.DateFormattingGetDateWithoughtTimeZone(item.ModifiedDate);
                }
            }

            return this.PartialView("_EmailThread", lstEmailThread);
        }

        /// <summary>
        /// Archives the message.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        /// <returns>Action result</returns>
        [HttpGet]
        public ActionResult ArchiveMessage(long emailThreadId)
        {
            bool isArchived = true;
            //try
            //{
            _emailService.UpdateJobEmailThreadAsArchived(emailThreadId);
            //}
            //catch (Exception ex)
            //{
            //    isArchived = true;
            //}

            return this.Json(new { IsArchived = isArchived }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the email thread by message identifier.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        /// <returns>Action result</returns>
        public PartialViewResult GetEmailThreadByMessageId(long emailThreadId)
        {
            int id_Acct = 0;
            CheckMail checkMail = new CheckMail();
            List<EmailMessageWithAttachments> result = new List<EmailMessageWithAttachments>();
            Account acct = Session[Constants.sessionAccount] as Account;
            if (acct != null)
            {
                id_Acct = acct.ID;
            }

            List<EmailMessageWithAttachments> lstEmailThread = _emailService.GetMessagesByEmailThreadId(emailThreadId, id_Acct);
            foreach (var item in lstEmailThread)
            {
                Dictionary<string, string> shortAndFullDate = checkMail.GetShortAndFullDate(acct, item.msg_date);
                item.full_date = shortAndFullDate["FullDate"];
                ////EmailMessageWithAttachments emailMessageWithAttachments = new EmailMessageWithAttachments();
                ////if (item.has_attachments)
                ////{
                item.Attachments = new List<string>();
                MailBee.Mime.MailMessage message = checkMail.LoadMessagesByEmailThreadID(Convert.ToInt32(item.id), item.FolderName, item.email, Convert.ToInt32(item.id_acct));
                if (message != null)
                {
                    //emailMessageWithAttachments.from = item.from;
                    //emailMessageWithAttachments.to = item.to;
                    if (!string.IsNullOrEmpty(item.body_text))
                    {
                        item.body_text = message.BodyHtmlText.Replace("wmx_", "");
                    }

                    if (message.Attachments.Count > 0)
                    {
                        foreach (var attachment in message.Attachments)
                        {

                            //string attachLink = Url.Action("DownloadFileFromTheServer", "JobEmail", new { area = "Email" }) + "?_download=1&file_name=" + ((MailBee.Mime.Attachment)(attachment)).Filename + "&" + ((MailBee.Mime.Attachment)(attachment)).FilenameOriginal;
                            //string attachLink = Url.Action("DownloadFileFromTheServer", "Email", new { area = "Email" }) + "?file_name=" + ((MailBee.Mime.Attachment)(attachment)).Filename + "&fileFullPath=" + ((MailBee.Mime.Attachment)(attachment)).SavedAs;
                            string attachLink = Url.Action("DownloadFileFromTheServer", "Email", new { area = "Email" }) + "?fileFullPath=" + ((MailBee.Mime.Attachment)(attachment)).SavedAs + "&file_name=" + ((MailBee.Mime.Attachment)(attachment)).Filename;
                            string attach = "<a href=" + attachLink + "> <img src='" + Url.Content("~/images/attach_document.png") + "' alt='' class='mCS_img_loaded'/>" + ((MailBee.Mime.Attachment)(attachment)).Filename + " </a>";
                            item.Attachments.Add(attach);
                        }
                    }

                    result.Add(item);
                }
                ////}
                ////else
                ////{
                ////    item.Attachments = new List<string>();
                ////    result.Add(item);
                ////}
            }
            return this.PartialView("_EmailConversation", result);
        }

        /// <summary>
        /// Creates the job email thread.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Receiver">The receiver.</param>
        /// <param name="ReceiverEmail">The receiver email.</param>
        /// <param name="SubjectGroup">The subject group.</param>
        /// <param name="Subject">The subject.</param>
        /// <returns>Action result</returns>
        [HttpGet]
        public ActionResult CreateJobEmailThread(int JobId, string Receiver, string ReceiverEmail, string SubjectGroup, string Subject)
        {
            CheckMail checkMail = new CheckMail();
            Account acct = Session[Constants.sessionAccount] as Account;
            List<EmailThread> lstEmailThread = new List<EmailThread>();
            if (acct != null)
            {
                string Sender = ProjectSession.LoggedInName;
                string SenderEmail = acct.Email;
                List<JobWiseUsers> lstJobWiseUsers = (List<JobWiseUsers>)Session["JobWiseUsers"];
                int IsSscAndSca = 0;
                if (lstJobWiseUsers.Count > 0)
                {
                    lstJobWiseUsers = lstJobWiseUsers.Where(w => w.Email != null && w.Email != "").ToList();
                    if (lstJobWiseUsers.Count > 0)
                    {
                        var resultuserType = lstJobWiseUsers.FirstOrDefault(w => w.Email == ReceiverEmail);
                        if (ProjectSession.UserTypeId == 4 && resultuserType != null && Convert.ToString(resultuserType.UserTypeID) == "6")
                        {
                            IsSscAndSca = 2;
                        }
                        else if (ProjectSession.UserTypeId == 6 && resultuserType != null && Convert.ToString(resultuserType.UserTypeID) == "4")
                        {
                            IsSscAndSca = 2;
                        }
                    }
                }

                int index = Receiver.IndexOf("-");
                if (index > 0)
                {
                    Receiver = Receiver.Substring(0, index);
                }

                lstEmailThread = _emailService.CreateJobEmailThread(JobId, acct.ID, Sender, Receiver, SenderEmail, ReceiverEmail, SubjectGroup, Subject, ProjectSession.LoggedInUserId, IsSscAndSca);

                //Inser history
                EmailMessage objEmailMessage = new EmailMessage()
                {
                    from = Sender,
                    to = Receiver,
                    subject = Subject,
                    id = JobId
                };
                //bool isHistorySaved = _jobHistory.LogJobHistory(objEmailMessage, HistoryCategory.MessageSent);
                string to = !string.IsNullOrEmpty(objEmailMessage.to) ? objEmailMessage.to : "";
                string subject = !string.IsNullOrEmpty(objEmailMessage.subject) ? objEmailMessage.subject : "";

                string JobHistoryMessage = "sent new message to: <b class=\"blue-title\">" + to + "</b>" +
                                "<p><label>Subject: </label>" + subject + "</p>";
                Common.SaveJobHistorytoXML(Convert.ToInt32(objEmailMessage.id), JobHistoryMessage, "General","MessageSent", ProjectSession.LoggedInName, false);
                

                if (lstEmailThread.Count > 0)
                {
                    foreach (var item in lstEmailThread)
                    {
                        item.FullDate = checkMail.DateFormattingGetDateWithoughtTimeZone(item.ModifiedDate);
                    }
                }
            }

            return this.PartialView("_EmailThread", lstEmailThread);
        }

        [HttpGet]
        [ValidateInput(false)]
        public PartialViewResult GetMessages(int jobID)
        {
            List<EmailThread> lstEmailThread = _emailService.GetEmailThreadByJobId(jobID, string.Empty, null, null, string.Empty, false, 1, Convert.ToInt32(FormBot.Helper.ProjectConfiguration.GetPageSizeForMail), ProjectSession.LoggedInUserId);
            string loggedUserEmail = "";
            Account acct = Session[Constants.sessionAccount] as Account;
            if (acct != null)
            {
                loggedUserEmail = acct.Email;
                CheckMail checkMail = new CheckMail();
                checkMail.AutoCheckMailForAccount();
            }

            ViewBag.LoggedUserEmail = loggedUserEmail;
            return this.PartialView("Messages", lstEmailThread);
        }

        #endregion

        #region PreApproval Popup

        /// <summary>
        /// Applies for pre approval and connection.
        /// </summary>
        /// <param name="jobID">jobID</param>
        /// <param name="preApprOrConne">preApprOrConne</param>
        /// <param name="distributorID">distributorID</param>
        /// <returns>action result</returns>
        public PartialViewResult ApplyForPreApprovalAndConnection(int jobID, int preApprOrConne, string distributorID)
        {
            List<JobStatusForPreApprovalAndConnection> result = _emailService.GetStatusForPreApprovalAndConnection(preApprOrConne);
            if (result.Count() > 0)
            {
                List<SelectListItem> listJobStatus = new List<SelectListItem>();

                foreach (var item in result)
                {
                    listJobStatus.Add(new SelectListItem
                    {
                        Text = item.Status,
                        Value = Convert.ToString(item.Id),
                    });
                }

                ViewBag.listJobStatus = listJobStatus;
            }

            JobCommentForPreApprAndConn jobCommentForPreApprAndConn = _emailService.GetJobStatusAndComment(jobID, preApprOrConne);
            if (jobCommentForPreApprAndConn != null)
            {
                ViewBag.SelectedJobStatus = jobCommentForPreApprAndConn.JobStatusId;
                ViewBag.SelectedJobComment = jobCommentForPreApprAndConn.Comment;
            }

            string preApprovalsOrConnection = string.Empty;
            if (preApprOrConne == FormBot.Helper.SystemEnums.JobStage.PreApprovals.GetHashCode())
            {
                preApprovalsOrConnection = FormBot.Helper.SystemEnums.JobStage.PreApprovals.ToString();
            }
            else if (preApprOrConne == FormBot.Helper.SystemEnums.JobStage.Connections.GetHashCode())
            {
                preApprovalsOrConnection = FormBot.Helper.SystemEnums.JobStage.Connections.ToString();
            }

            List<FormBot.Entity.Documents.DocumentSteps> DocumentSteps = _jobDetails.GetDocumentsStepsForPreApprovalAndConn(distributorID, preApprovalsOrConnection, jobID);

            return this.PartialView("_ApplyForPreApprovalAndConnection", DocumentSteps);
        }

        /// <summary>
        /// Get EmailSendViewForPreApprovalAndConnection
        /// </summary>
        /// <param name="jobID">jobID</param>
        /// <param name="Type">Type</param>
        /// <returns>partial result</returns>
        public PartialViewResult GetEmailSendViewForPreApprovalAndConnection(int jobID, int Type, bool isClassic = true)
        {

            DocumentStepType documentStepType = new DocumentStepType();
            documentStepType.Type = Type;
            documentStepType.IsClassic = isClassic;
			ViewBag.guid = Guid.NewGuid().ToString();
            return this.PartialView("_EmailSendViewForDocumentSubmission", documentStepType);
            //return this.PartialView("_EmailSendViewForDocumentSubmission", Type);
        }

        /// <summary>
        /// Adds the update job comment for pre appr and connection.
        /// </summary>
        /// <param name="jobCommentForPreApprAndConn">The job comment for pre appr and connection.</param>
        /// <returns>Action result</returns>
        [HttpPost]
        public JsonResult AddUpdateJobCommentForPreApprAndConn(JobCommentForPreApprAndConn jobCommentForPreApprAndConn)
        {
            bool isHistorySaved = false;
            _emailService.AddUpdateJobCommentForPreApprAndConn(jobCommentForPreApprAndConn);

            //Inser history
            StatusHistory objStatusHistory = new StatusHistory()
            {
                Status = string.IsNullOrEmpty(jobCommentForPreApprAndConn.Comment) ? "to '" + jobCommentForPreApprAndConn.Status + "'" : "- '" + jobCommentForPreApprAndConn.Status + "'",
                JobID = jobCommentForPreApprAndConn.JobId,
                ChangedOn = DateTime.Now,
                Action = string.IsNullOrEmpty(jobCommentForPreApprAndConn.Comment) ? "changed" : "left a note with",
                Comment = string.IsNullOrEmpty(jobCommentForPreApprAndConn.Comment) ? "" : "<br/><label>Note: </label>" + jobCommentForPreApprAndConn.Comment
            };
            if (jobCommentForPreApprAndConn.PreApprovalAndConnectionId == 1)
            {
                //isHistorySaved = _jobHistory.LogJobHistory(objStatusHistory, HistoryCategory.PreapprovalStatus);
                string action = objStatusHistory.Action;
                string status = objStatusHistory.Status;
                string changedon = objStatusHistory.ChangedOn.ToString();
                string comment = objStatusHistory.Comment;

                string JobHistoryMessage = action + " preapproval status " +status + " " + changedon + " " + comment;
                Common.SaveJobHistorytoXML(objStatusHistory.JobID, JobHistoryMessage, "Statuses", "PreapprovalStatus", ProjectSession.LoggedInName, false);
                isHistorySaved = true;
            }
            else if (jobCommentForPreApprAndConn.PreApprovalAndConnectionId == 2)
            {
                //isHistorySaved = _jobHistory.LogJobHistory(objStatusHistory, HistoryCategory.ConnectionStatus);
                string action = objStatusHistory.Action;
                string status = objStatusHistory.Status;
                string changedon = objStatusHistory.ChangedOn.ToString();
                string comment = objStatusHistory.Comment;
                string JobHistoryMessage = action + " connection status " + status + " " + changedon + " " + comment;
                Common.SaveJobHistorytoXML(objStatusHistory.JobID, JobHistoryMessage, "Statuses", "ConnectionStatus", ProjectSession.LoggedInName, false);
                isHistorySaved = true;
            }
            return this.Json(isHistorySaved);
        }

        [HttpPost]
        public JsonResult AddUpdateJobEmailConversationForPreAndConn(JobEmailConversationForPreAndConn jobEmailConversationForPreAndConn)
        {
            _emailService.SaveJobEmailConversationForPreAndConn(jobEmailConversationForPreAndConn);

            //Inser history
            if (jobEmailConversationForPreAndConn.IsPreAprConn == 1)
            {
                //bool isHistorySaved = _jobHistory.LogJobHistory(jobEmailConversationForPreAndConn, HistoryCategory.PreapprovalDocuments);
                string distributor = jobEmailConversationForPreAndConn.Distributor;
                string JobHistoryMessage = "sent preapprovals documents to " + distributor ;
                Common.SaveJobHistorytoXML(jobEmailConversationForPreAndConn.JobId, JobHistoryMessage, "Statuses", "PreapprovalDocuments", ProjectSession.LoggedInName, false);
            }
            else if (jobEmailConversationForPreAndConn.IsPreAprConn == 2)
            {
                //bool isHistorySaved = _jobHistory.LogJobHistory(jobEmailConversationForPreAndConn, HistoryCategory.ConnectionDocuments);
                string distributor = jobEmailConversationForPreAndConn.Distributor;
                string JobHistoryMessage = "sent connection documents to " + distributor;
                Common.SaveJobHistorytoXML(jobEmailConversationForPreAndConn.JobId, JobHistoryMessage, "Statuses", "ConnectionDocuments", ProjectSession.LoggedInName, false);
            }
            return this.Json("");
        }

        /// <summary>
        /// Reads the email for job email.
        /// </summary>
        /// <param name="id_msg">The id_msg.</param>
        /// <param name="id_acct">The id_acct.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="sendOrReceiveFromEmail">The send or receive from email.</param>
        /// <returns>Action result</returns>
        [HttpGet]
        public ActionResult ReadEmailForJobEmail(int id_msg, int id_acct, string folderName, string sendOrReceiveFromEmail)
        {
            CheckMail checkMail = new CheckMail();
            List<EmailMessageWithAttachments> result = new List<EmailMessageWithAttachments>();
            MailBee.Mime.MailMessage message = checkMail.LoadMessagesByEmailThreadID(Convert.ToInt32(id_msg), folderName, sendOrReceiveFromEmail, id_acct);
            ComposeEmail composeEmail = new ComposeEmail();
            Account acct = Session[Constants.sessionAccount] as Account;
            if (message != null)
            {
                //Dictionary<string, string> shortAndFullDate = checkMail.GetShortAndFullDate(acct, message.DateReceived);
                //var _time = message.DateReceived.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                //_full = GetWeekday(message.DateReceived) + ", " + GetMonth(message.DateReceived) +
                //dt.ToString(" dd, yyyy", CultureInfo.InvariantCulture) + ", " + _time;
                var fullDate = checkMail.DateFormattingGetDateWithoughtTimeZone(message.Date);

                composeEmail.From = message.From.AsString;
                composeEmail.To = message.To.AsString;
                //composeEmail.ShortDate = shortAndFullDate["ShortDate"];
                composeEmail.FullDate = fullDate;
                composeEmail.Body = new innerBody() { body = message.BodyHtmlText.Replace("wmx_", "") };
                composeEmail.Subject = message.Subject;
                composeEmail.Cc = message.Cc.AsString;
                //emailMessageWithAttachments.from = item.from;
                //emailMessageWithAttachments.to = item.to;
                //item.body_text = message.BodyPlainText;
                var attachLink = "";
                var attach = "";
                if (message.Attachments.Count > 0)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        //string attachLink = Url.Action("DownloadFileFromTheServer", "Email", new { area = "Email" }) + "?fileFullPath=" + ((MailBee.Mime.Attachment)(attachment)).SavedAs + "&file_name=" + ((MailBee.Mime.Attachment)(attachment)).Filename;
                        //string attach = "<a href=" + attachLink + "> <img src='" + Url.Content("~/images/attach_document.png") + "' alt='' class='mCS_img_loaded'/>" + ((MailBee.Mime.Attachment)(attachment)).Filename + " </a>";
                        attachLink = Url.Action("DownloadFileFromTheServer", "Email", new { area = "Email" }) + "?fileFullPath=" + ((MailBee.Mime.Attachment)(attachment)).SavedAs + "&file_name=" + ((MailBee.Mime.Attachment)(attachment)).Filename;
                        attach += "<li> <a href=" + attachLink + "> <img src='" + Url.Content("~/images/attach_document.png") + "' alt=''/>" + ((MailBee.Mime.Attachment)(attachment)).FilenameOriginal + " </a> </li>";
                    }

                    composeEmail.Attachment = attach;
                }

                //result.Add(item);
            }

            if (acct.ID == id_acct)
            {
                ViewBag.isDisplayReplyForward = true;
            }
            else
            {
                ViewBag.isDisplayReplyForward = false;
            }

            ViewBag.UserEmailProp = acct.Email;

            return this.PartialView("_EmailTabLoadContent", composeEmail);
        }

        /// <summary>
        /// Prepares the reply for job email tab.
        /// </summary>
        /// <param name="id_msg">The id_msg.</param>
        /// <param name="id_acct">The id_acct.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="sendOrReceiveFromEmail">The send or receive from email.</param>
        /// <returns>Action result</returns>
        [HttpPost]
        public ActionResult PrepareReplyForJobEmailTab(int id_msg, int id_acct, string folderName, string sendOrReceiveFromEmail)
        {
            CheckMail checkMail = new CheckMail();
            var mail = new ComposeEmail();
            Account acct = Session[Constants.sessionAccount] as Account;
            MailBee.Mime.MailMessage message = checkMail.LoadMessagesByEmailThreadID(Convert.ToInt32(id_msg), folderName, sendOrReceiveFromEmail, id_acct);
            mail.Attachments = new List<AttachmentData>();
            var body = string.Empty;
            if (message != null)
            {
                ////Dictionary<string, string> shortAndFullDate = checkMail.GetShortAndFullDate(acct, message.Date);
                var fullDate = checkMail.DateFormattingGetDateWithoughtTimeZone(message.Date);
                ////mail.ShortDate = shortAndFullDate["ShortDate"];
                mail.FullDate = fullDate;

                mail.From = message.From.Email;
                var to = message.To.AsString;
                if (!string.IsNullOrEmpty(to))
                {
                    var splitToByComma = new List<string>();
                    foreach (var detail in to.Split(',').ToList())
                    {
                        splitToByComma.Add(checkMail.SplitNameAndEmail(detail.Trim()).Replace("&lt;", "").Replace("&gt;", "").Trim());
                    }

                    splitToByComma = splitToByComma.Distinct().ToList();
                    splitToByComma.Remove(acct.Email);
                    mail.To = string.Join(",", splitToByComma);
                }
                ////mail.To = message.To.AsString;
                mail.Cc = message.Cc.AsString;
                var cc = message.Cc.AsString;
                if (!string.IsNullOrEmpty(cc))
                {
                    var splitToByComma = new List<string>();
                    foreach (var detail in cc.Split(',').ToList())
                    {
                        splitToByComma.Add(checkMail.SplitNameAndEmail(detail.Trim().Replace("&lt;", "").Replace("&gt;", "").Trim()));
                    }

                    splitToByComma = splitToByComma.Distinct().ToList();
                    splitToByComma.Remove(acct.Email);
                    mail.Cc = string.Join(",", splitToByComma);
                }

                ////mail.Body = new innerBody() { body = message.BodyHtmlText };
                mail.Bcc = message.Bcc.AsString;

                mail.Subject = message.Subject;
                string mess = string.Empty;
                mess = @"<br/>" + acct.Signature;
                mess += "<br/>---- " + "OriginalMessage" + " ----<br/>";
                mess += "<b>" + "From" + "</b>: " + Utils.EncodeHtml(message.From.Email) + "<br/>";
                mess += "<b>" + "To" + "</b>: " + Utils.EncodeHtml(Convert.ToString(to)) + "<br/>";
                if (!string.IsNullOrEmpty(Convert.ToString(cc)))
                    mess += "<b>" + "CC" + "</b>: " + Utils.EncodeHtml(Convert.ToString(cc)) + "<br/>";
                mess += "<b>" + "Sent" + "</b>: " + Utils.EncodeHtml(mail.FullDate) + "<br/>";
                mess += "<b>" + "Subject" + "</b>: " + Utils.EncodeHtml(mail.Subject) + "<br/>";
                body = mess + message.BodyHtmlText;
                mail.Body = new innerBody() { body = body.Replace("wmx_", "") };

                foreach (var attachment in message.Attachments)
                {
                    var savedPath = ((MailBee.Mime.Attachment)(attachment)).SavedAs;
                    var splitPath = savedPath.Split('\\').ToList();
                    var tempFileName = splitPath.Last().ToString();
                    mail.Attachments.Add(new AttachmentData
                    {
                        FileName = ((MailBee.Mime.Attachment)(attachment)).FilenameOriginal,
                        GeneratedName = tempFileName
                    });
                }
            }

            var result = new { Result = mail };
            return this.Json(result);
        }

        #endregion
    }
}
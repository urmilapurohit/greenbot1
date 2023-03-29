using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using FormBot.Entity.Email;
using System;
using FormBot.Helper;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using FormBot.Email;
using FormBot.Entity.Settings;
using FormBot.Entity.Job;
using FormBot.Helper.Helper;

namespace FormBot.BAL.Service
{
    public class EmailBAL : IEmailBAL
    {
        #region Properties
        ResellerBAL objResellerBAL = new ResellerBAL();
        SolarCompanyBAL objSolarCompanyBAL = new SolarCompanyBAL();
        UserBAL objUserBAL = new UserBAL();
        UnitTypeBAL objUnitTypeBAL = new UnitTypeBAL();
        StreetTypeBAL objStreetTypeBAL = new StreetTypeBAL();
        #endregion

        #region Email

        public IList<EmailMessage> GetAllMessages(long maxMessageId, int id_acct)
        {
            string spName = "[GetLatestEmailMessage]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("maxMessageId", SqlDbType.BigInt, maxMessageId));
            sqlParameters.Add(DBClient.AddParameters("id_acct", SqlDbType.Int, id_acct));
            IList<EmailMessage> lstEmailMessage = CommonDAL.ExecuteProcedure<EmailMessage>(spName, sqlParameters.ToArray());

            return lstEmailMessage;
        }

        public IList<EmailFolder> GetFolders(int id_acct)
        {
            string spName = "[GetFoldersList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("id_acct", SqlDbType.Int, id_acct));
            IList<EmailFolder> lstEmailFolder = CommonDAL.ExecuteProcedure<EmailFolder>(spName, sqlParameters.ToArray());
            lstEmailFolder.ToList().ForEach(f => f.name = f.name.Replace("#", ""));
            return lstEmailFolder;
        }

        /// <summary>
        /// Create Email Template
        /// </summary>
        /// <param name="emailTemplate">email Template</param>
        /// <returns>The List of Email Template</returns>
        public int CreateTemplate(EmailTemplate emailTemplate)
        {
            string spName = "[EmailTemplate_InsertUpdate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TemplateID", SqlDbType.Int, emailTemplate.TemplateID));
            sqlParameters.Add(DBClient.AddParameters("TemplateName", SqlDbType.NVarChar, emailTemplate.TemplateName));
            sqlParameters.Add(DBClient.AddParameters("Subject", SqlDbType.NVarChar, emailTemplate.Subject));
            sqlParameters.Add(DBClient.AddParameters("Content", SqlDbType.NVarChar, emailTemplate.Content));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.BigInt, emailTemplate.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.BigInt, emailTemplate.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, emailTemplate.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));

            object templateID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(templateID);
        }

        /// <summary>
        /// Email Template Listing
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort column.</param>
        /// <param name="sortDir">The sort direction.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>The List of Email Template</returns>
        public List<EmailTemplate> GetEmailTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string templateName, string subject)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("templateName", SqlDbType.NVarChar, templateName));
            sqlParameters.Add(DBClient.AddParameters("subject", SqlDbType.NVarChar, subject));
            List<EmailTemplate> lstEmailTemplate = CommonDAL.ExecuteProcedure<EmailTemplate>("EmailTemplate_GetEmailTemplateList", sqlParameters.ToArray()).ToList();
            return lstEmailTemplate;
        }

        /// <summary>
        /// Delete Email Template
        /// </summary>
        /// <param name="templateID">template ID</param>
        public void DeleteEmailTemplate(int templateID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TemplateID", SqlDbType.Int, templateID));
            CommonDAL.Crud("EmailTemplate_DeleteEmailTemplate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get Email Template by TemplateID
        /// </summary>
        /// <param name="templateID">template ID</param>
        /// <returns>Get Single Email Template</returns>
        public EmailTemplate GetEmailTemplateByID(int templateID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TemplateID", SqlDbType.Int, templateID));
            return CommonDAL.SelectObject<EmailTemplate>("EmailTemplate_GetEmailTemplateByID", sqlParameters.ToArray());
        }

        public void ComposeAndSendEmail(EmailInfo emailInfo, string mailTo, List<AttachmentData> Attachments = null,List<EmialAttechment> lstAttechment = null,Guid guid = default(Guid), string JobId = null)
        {
            try
            {
				
				EmailTemplate emailTempalte = GetEmailTemplateByID(emailInfo.TemplateID);
				if (Attachments == null)
				{
					if (emailTempalte != null )
					{
						if (!string.IsNullOrEmpty(emailTempalte.Content))
						{
							QueuedEmail objQueuedEmail = new QueuedEmail();
							objQueuedEmail.FromEmail = ProjectSession.MailFrom;
							objQueuedEmail.Body = GetEmailBody(emailInfo, emailTempalte);
                            if(emailTempalte.TemplateID == 45)
                            {
                                objQueuedEmail.Subject = emailTempalte.Subject + " at " + emailInfo.InstallationAddress;
                            }
                            else
                            {
                                objQueuedEmail.Subject = emailTempalte.Subject;
                            }
							objQueuedEmail.ToEmail = mailTo;
							objQueuedEmail.CreatedDate = DateTime.Now;
							objQueuedEmail.ModifiedDate = DateTime.Now;
							if(guid != Guid.Empty)
							{
								objQueuedEmail.Guid = guid;
							}
                            objQueuedEmail.JobId = JobId;
                            
                            InsertUpdateQueuedEmail(objQueuedEmail);

							if (lstAttechment != null)
							{
								if(lstAttechment.Any())
								{
									foreach (var item in lstAttechment)
									{
										item.Guid = objQueuedEmail.Guid;
										InsertEmailAttechment(item);
									}
								}
							}
							//InsertEmailAttechment()
						}
					}
				}
				else
				{
					if (Attachments.Any())
					{
						//ComposeEmail composeEmail = new Email.ComposeEmail();
						//if (Attachments != null && Attachments.Count > 0)
						//{
						//	composeEmail.Attachments = Attachments;
						//}
						//composeEmail.Subject = emailTempalte.Subject;
						//composeEmail.To = mailTo;
						//composeEmail.Body = new innerBody();
						//composeEmail.Body.body = GetEmailBody(emailInfo, emailTempalte);
						//CheckMail checkMail = new CheckMail();
						//checkMail.SendMail(composeEmail, "send");
					}
				}
            }
            catch (Exception ex)
            {
                //Common.WriteErrorLog(ex.Message);
                Helper.Log.WriteError(ex, "Compose and send email:");
            }
        }


        public void ComposeAndSendEmailForAndroidAppUsers(EmailInfo emailInfo, string mailTo)
        {
            try
            {

                EmailTemplate emailTempalte = GetEmailTemplateByID(emailInfo.TemplateID);
                if (emailTempalte != null)
                {
                    if (!string.IsNullOrEmpty(emailTempalte.Content))
                    {
                        QueuedEmail objQueuedEmail = new QueuedEmail();
                        objQueuedEmail.FromEmail = ProjectSession.MailFrom;
                        objQueuedEmail.Body = GetEmailBody(emailInfo, emailTempalte);
                        objQueuedEmail.Subject = emailTempalte.Subject;
                        objQueuedEmail.ToEmail = mailTo;
                        objQueuedEmail.CreatedDate = DateTime.Now;
                        objQueuedEmail.ModifiedDate = DateTime.Now;
                        InsertUpdateQueuedEmailForAndrodiAppUser(objQueuedEmail);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Gets the email body.
        /// </summary>
        /// <param name="emailInfo">The email information.</param>
        /// <param name="emailTemplate">email template</param>
        /// <returns>string object</returns>
        public string GetEmailBody(EmailInfo emailInfo, EmailTemplate emailTemplate)
        {
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            string content = emailTemplate.Content;
            foreach (PropertyInfo pro in props)
            {
                if (emailTemplate.Content.Contains("{@" + pro.Name + "}"))
                {
                    string replaceFrom = string.Empty;
                    string replaceTo = "{@" + pro.Name + "}";
                    if (pro.GetValue(emailInfo) != null)
                    {
                        replaceFrom = pro.GetValue(emailInfo).ToString();
                        content = content.Replace(replaceTo, replaceFrom);
                    }
                    else
                    {
                        content = content.Replace(replaceTo, replaceFrom);
                    }
                }
            }

            return content;
        }

        /// <summary>
        /// Assigns the email to job.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Id_msg">The id_msg.</param>
        public void AssignEmailToJob(int JobId, string Id_msg)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Id_msg", SqlDbType.NVarChar, Id_msg));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("AssignEmailToJob_INSERT", sqlParameters.ToArray());
        }

        #endregion

        #region EmailTemplateMethods

        /// <summary>
        /// Generals the compose and send mail.
        /// </summary>
        /// <param name="emailTempalte">The email tempalte.</param>
        /// <param name="mailTo">The mail to.</param>
        public bool GeneralComposeAndSendMail(EmailTemplate emailTempalte, string mailTo)
        {
			string FailReason = string.Empty;
			bool status = false;
			try
			{
				if (emailTempalte != null && !string.IsNullOrEmpty(emailTempalte.Content))
				{
					QueuedEmail objQueuedEmail = new QueuedEmail();
					objQueuedEmail.FromEmail = ProjectSession.MailFrom;
					objQueuedEmail.Body = emailTempalte.Content;
					objQueuedEmail.Subject = emailTempalte.Subject;
					objQueuedEmail.ToEmail = mailTo;
					objQueuedEmail.CreatedDate = DateTime.Now;
					objQueuedEmail.ModifiedDate = DateTime.Now;
					InsertUpdateQueuedEmail(objQueuedEmail);
					status = true;
				}
			}
			catch (Exception ex)
			{
				FailReason = ex.Message;
                status = false;
            }
            return status;
		}

        /// <summary>
        /// Generals the compose and send mail.
        /// </summary>
        /// <param name="emailTempalte">The email tempalte.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="composeEmail">The compose email.</param>
        public void GeneralComposeAndSendMail(EmailTemplate emailTempalte, string mailTo, ComposeEmail composeEmail)
        {
            //ComposeEmail composeEmail = new Email.ComposeEmail();
            //composeEmail.Attachment = "";
            //composeEmail.Subject = emailTempalte.Subject;
            //composeEmail.To = mailTo;
            //composeEmail.Body = new innerBody();
            //composeEmail.Body.body = emailTempalte.Content;
			string FailReason = string.Empty;
			bool status = false;
			try
			{
				if (emailTempalte != null && !string.IsNullOrEmpty(emailTempalte.Content))
				{
					QueuedEmail objQueuedEmail = new QueuedEmail();
					objQueuedEmail.FromEmail = ProjectSession.MailFrom;
					objQueuedEmail.Body = emailTempalte.Content;
					objQueuedEmail.Subject = emailTempalte.Subject;
					objQueuedEmail.ToEmail = mailTo;
					objQueuedEmail.CreatedDate = DateTime.Now;
					objQueuedEmail.ModifiedDate = DateTime.Now;
					InsertUpdateQueuedEmail(objQueuedEmail);
					status = true;
				}
			}
			catch (Exception ex)
			{
				FailReason = ex.Message;
			}
			//CheckMail checkMail = new CheckMail();
			//checkMail.SendMail(composeEmail, "send");
		}

        #endregion

        #region EmailThread

        /// <summary>
        /// Gets the email thread by job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="senderOrReceiverName">Name of the sender or receiver.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="title">The title.</param>
        /// <param name="isArchived">if set to <c>true</c> [is archived].</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>emai list</returns>
        public List<EmailThread> GetEmailThreadByJobId(int jobId, string senderOrReceiverName, DateTime? fromDate, DateTime? toDate, string title, bool isArchived, int pageNumber, int pageSize, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("senderOrReceiverName", SqlDbType.NVarChar, senderOrReceiverName));
            sqlParameters.Add(DBClient.AddParameters("fromDate", SqlDbType.DateTime, fromDate));
            sqlParameters.Add(DBClient.AddParameters("toDate", SqlDbType.DateTime, toDate));
            sqlParameters.Add(DBClient.AddParameters("title", SqlDbType.NVarChar, title));
            sqlParameters.Add(DBClient.AddParameters("isArchived", SqlDbType.Bit, isArchived));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            List<EmailThread> lstEmailThread = CommonDAL.ExecuteProcedure<EmailThread>("EmailThread_GetEmailThreadByJobId", sqlParameters.ToArray()).ToList();
            return lstEmailThread;
        }

        /// <summary>
        /// Gets the messages by email thread identifier.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        /// <param name="id_Acct">The id_ acct.</param>
        /// <returns>email list</returns>
        public List<EmailMessageWithAttachments> GetMessagesByEmailThreadId(long emailThreadId, int id_Acct)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("EmailThreadId", SqlDbType.BigInt, emailThreadId));
            sqlParameters.Add(DBClient.AddParameters("Id_Acct", SqlDbType.Int, id_Acct));
            List<EmailMessageWithAttachments> lstEmailMessage = CommonDAL.ExecuteProcedure<EmailMessageWithAttachments>("EmailThread_GetMessagesByEmailThreadId", sqlParameters.ToArray()).ToList();
            return lstEmailMessage;
        }

        /// <summary>
        /// Gets the thread members.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// string list
        /// </returns>
        public List<string> GetThreadMembers(int jobId, int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            DataSet dsThreadMembers = CommonDAL.ExecuteDataSet("JobEmailThreadGetThreadMembers", sqlParameters.ToArray());
            List<string> lstThreadMembers = new List<string>();
            if (dsThreadMembers != null && dsThreadMembers.Tables[0] != null && dsThreadMembers.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsThreadMembers.Tables[0].Rows.Count; i++)
                {
                    lstThreadMembers.Add(Convert.ToString(dsThreadMembers.Tables[0].Rows[i][0]));
                }
            }

            return lstThreadMembers;
        }

        /// <summary>
        /// Updates the job email thread as archived.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        public void UpdateJobEmailThreadAsArchived(long emailThreadId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("EmailThreadId", SqlDbType.BigInt, emailThreadId));
            CommonDAL.Crud("JobEmailThread_UpdateThreadAsArchived", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the job email thread as archived.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Id_AcctId">The id_ acct identifier.</param>
        /// <param name="Sender">The sender.</param>
        /// <param name="Receiver">The receiver.</param>
        /// <param name="SenderEmail">The sender email.</param>
        /// <param name="ReceiverEmail">The receiver email.</param>
        /// <param name="SubjectGroup">The subject group.</param>
        /// <param name="Subject">The subject.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="IsSscAndSca">The is SSC and sca.</param>
        /// <returns>
        /// email list
        /// </returns>
        public List<EmailThread> CreateJobEmailThread(int JobId, int Id_AcctId, string Sender, string Receiver, string SenderEmail, string ReceiverEmail, string SubjectGroup, string Subject, int UserId, int IsSscAndSca)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Id_AcctId", SqlDbType.Int, Id_AcctId));
            sqlParameters.Add(DBClient.AddParameters("Sender", SqlDbType.NVarChar, Sender));
            sqlParameters.Add(DBClient.AddParameters("Receiver", SqlDbType.NVarChar, Receiver));
            sqlParameters.Add(DBClient.AddParameters("SenderEmail", SqlDbType.NVarChar, SenderEmail));
            sqlParameters.Add(DBClient.AddParameters("ReceiverEmail", SqlDbType.NVarChar, ReceiverEmail));
            sqlParameters.Add(DBClient.AddParameters("SubjectGroup", SqlDbType.NVarChar, SubjectGroup));
            sqlParameters.Add(DBClient.AddParameters("Subject", SqlDbType.NVarChar, Subject));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("IsSscAndSca", SqlDbType.Int, IsSscAndSca));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            List<EmailThread> lstEmailThread = CommonDAL.ExecuteProcedure<EmailThread>("JobEmailThread_CreateThread", sqlParameters.ToArray()).ToList();
            return lstEmailThread;
        }

        /// <summary>
        /// Gets the job wise users.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="UserId">user identifier</param>
        /// <returns>
        /// user list
        /// </returns>
        public List<JobWiseUsers> GetJobWiseUsers(int JobId, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            List<JobWiseUsers> lstJobWiseUsers = CommonDAL.ExecuteProcedure<JobWiseUsers>("JobEmailThread_GetJobWiseUsers", sqlParameters.ToArray()).ToList();
            return lstJobWiseUsers;
        }
        #endregion

        #region PreAprovalAndConnection Popup

        /// <summary>
        /// Gets the status job status for pre approval and connection.
        /// </summary>
        /// <param name="JobStage">The job stage.</param>
        /// <returns>job list</returns>
        //public List<JobStatusForPreApprovalAndConnection> GetStatusForPreApprovalAndConnection(int JobStage)
        public List<JobStatusForPreApprovalAndConnection> GetStatusForPreApprovalAndConnection(int? JobStage = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if(JobStage.HasValue)
                sqlParameters.Add(DBClient.AddParameters("JobStage", SqlDbType.Int, JobStage));
            return CommonDAL.ExecuteProcedure<JobStatusForPreApprovalAndConnection>("JobStatusForPreApprovalAndConnection_GetStatus", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Adds the job comment for pre appr and connection.
        /// </summary>
        /// <param name="jobCommentForPreApprAndConn">The job comment for pre appr and connection.</param>
        public void AddUpdateJobCommentForPreApprAndConn(JobCommentForPreApprAndConn jobCommentForPreApprAndConn)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobCommentForPreApprAndConn.JobId));
            sqlParameters.Add(DBClient.AddParameters("JobStatusId", SqlDbType.Int, jobCommentForPreApprAndConn.JobStatusId));
            sqlParameters.Add(DBClient.AddParameters("PreApprovalAndConnectionId", SqlDbType.Int, jobCommentForPreApprAndConn.PreApprovalAndConnectionId));
            sqlParameters.Add(DBClient.AddParameters("Comment", SqlDbType.NVarChar, jobCommentForPreApprAndConn.Comment));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, jobCommentForPreApprAndConn.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobCommentForPreApprAndConn_InsertUpdate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job status and comment.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="PreApprovalAndConnectionId">Pre Approval And Connection</param>
        /// <returns>
        /// job comment
        /// </returns>
        public JobCommentForPreApprAndConn GetJobStatusAndComment(int JobId, int PreApprovalAndConnectionId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("PreApprovalAndConnectionId", SqlDbType.Int, PreApprovalAndConnectionId));
            return CommonDAL.ExecuteProcedure<JobCommentForPreApprAndConn>("JobCommentForPreApprAndConn_GetComment", sqlParameters.ToArray()).FirstOrDefault();
        }

        /// <summary>
        /// Saves the job email conversation for pre and connection.
        /// </summary>
        /// <param name="jobEmailConversationForPreAndConn">The job email conversation for pre and connection.</param>
        public void SaveJobEmailConversationForPreAndConn(JobEmailConversationForPreAndConn jobEmailConversationForPreAndConn)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobEmailConversationForPreAndConn.JobId));
            sqlParameters.Add(DBClient.AddParameters("IsPreAprConn", SqlDbType.Int, jobEmailConversationForPreAndConn.IsPreAprConn));
            sqlParameters.Add(DBClient.AddParameters("StetpId", SqlDbType.Int, jobEmailConversationForPreAndConn.StetpId));
            sqlParameters.Add(DBClient.AddParameters("DocumentType", SqlDbType.Int, jobEmailConversationForPreAndConn.DocumentType));
            sqlParameters.Add(DBClient.AddParameters("Comment", SqlDbType.NVarChar, jobEmailConversationForPreAndConn.Comment));
            sqlParameters.Add(DBClient.AddParameters("SubjectCode", SqlDbType.NVarChar, jobEmailConversationForPreAndConn.SubjectCode));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobEmailConversationForPreAndConn_Insert", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the message for job email conversation for pre and connection.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>job list</returns>
        public List<JobEmailMessageInfo> GetMessageForJobEmailConversationForPreAndConn(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteProcedure<JobEmailMessageInfo>("JobEmailConversationForPreAndConn_GetMessage", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Gets the status for pre approval and connection for complaince.
        /// </summary>
        /// <param name="JobStage">The job stage.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>
        /// general class list
        /// </returns>
        public List<GeneralClass> GetStatusForPreApprovalAndConnectionForComplaince(int JobStage, int UserTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobStage", SqlDbType.Int, JobStage));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            DataSet dsComplaince = CommonDAL.ExecuteDataSet("JobStatusForPreApprovalAndConnection_GetStatusForComplaince", sqlParameters.ToArray());
            StcComplianceCheck stcComplianceCheck = new StcComplianceCheck();
            stcComplianceCheck.lstStcStatus = dsComplaince.Tables[0].ToListof<GeneralClass>();
            return stcComplianceCheck.lstStcStatus;
        }

        #endregion

        #region Email Service
        public virtual void InsertUpdateQueuedEmailForAndrodiAppUser(QueuedEmail queuedEmail, bool IsUpdate = false)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("QueuedEmailForNotifyInstallerId", SqlDbType.Int, queuedEmail.QueuedEmailForNotifyInstallerId));
            sqlParameters.Add(DBClient.AddParameters("FromEmail", SqlDbType.NVarChar, queuedEmail.FromEmail));
            sqlParameters.Add(DBClient.AddParameters("ToEmail", SqlDbType.NVarChar, queuedEmail.ToEmail));
            sqlParameters.Add(DBClient.AddParameters("CC", SqlDbType.NVarChar, queuedEmail.CC));
            sqlParameters.Add(DBClient.AddParameters("Bcc", SqlDbType.NVarChar, queuedEmail.Bcc));
            sqlParameters.Add(DBClient.AddParameters("Subject", SqlDbType.NVarChar, queuedEmail.Subject));
            sqlParameters.Add(DBClient.AddParameters("Body", SqlDbType.NVarChar, queuedEmail.Body));
            sqlParameters.Add(DBClient.AddParameters("Guid", SqlDbType.UniqueIdentifier, queuedEmail.Guid));
            sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, queuedEmail.IsSent));
            sqlParameters.Add(DBClient.AddParameters("SentTries", SqlDbType.Int, queuedEmail.SentTries));
            sqlParameters.Add(DBClient.AddParameters("SentOn", SqlDbType.DateTime, queuedEmail.SentOn));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, queuedEmail.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, queuedEmail.ModifiedDate));
            sqlParameters.Add(DBClient.AddParameters("IsUpdate ", SqlDbType.Bit, IsUpdate));
            CommonDAL.Crud("QueuedEmailForNotifyInstaller_InsertUpdate", sqlParameters.ToArray());
        }
        public virtual void InsertUpdateQueuedEmail(QueuedEmail queuedEmail,bool IsUpdate = false)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("QueuedEmailId", SqlDbType.Int, queuedEmail.QueuedEmailId));
			sqlParameters.Add(DBClient.AddParameters("FromEmail", SqlDbType.NVarChar, queuedEmail.FromEmail));
			sqlParameters.Add(DBClient.AddParameters("ToEmail", SqlDbType.NVarChar, queuedEmail.ToEmail));
			sqlParameters.Add(DBClient.AddParameters("CC", SqlDbType.NVarChar, queuedEmail.CC));
			sqlParameters.Add(DBClient.AddParameters("Bcc", SqlDbType.NVarChar, queuedEmail.Bcc));
			sqlParameters.Add(DBClient.AddParameters("Subject", SqlDbType.NVarChar, queuedEmail.Subject));
			sqlParameters.Add(DBClient.AddParameters("Body", SqlDbType.NVarChar, queuedEmail.Body));
			sqlParameters.Add(DBClient.AddParameters("Guid", SqlDbType.UniqueIdentifier, queuedEmail.Guid));
			sqlParameters.Add(DBClient.AddParameters("IsSent", SqlDbType.Bit, queuedEmail.IsSent));
			sqlParameters.Add(DBClient.AddParameters("SentTries", SqlDbType.Int, queuedEmail.SentTries));
			sqlParameters.Add(DBClient.AddParameters("SentOn", SqlDbType.DateTime, queuedEmail.SentOn));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, queuedEmail.CreatedDate));
			sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, queuedEmail.ModifiedDate));
			sqlParameters.Add(DBClient.AddParameters("IsUpdate ", SqlDbType.Bit, IsUpdate));
            sqlParameters.Add(DBClient.AddParameters("JobID ", SqlDbType.Int, queuedEmail.JobId != "" ? Convert.ToInt32(queuedEmail.JobId) : (object)DBNull.Value));
            CommonDAL.Crud("QueuedEmail_InsertUpdate", sqlParameters.ToArray());
		}
		public virtual int InsertEmailAttechment(EmialAttechment objEmialAttechment)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("QueuedEmailId", SqlDbType.Int, objEmialAttechment.QueuedEmailId));
			sqlParameters.Add(DBClient.AddParameters("FilePath", SqlDbType.NVarChar, objEmialAttechment.FilePath));
			sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, objEmialAttechment.FileName));
			sqlParameters.Add(DBClient.AddParameters("Guid", SqlDbType.UniqueIdentifier, objEmialAttechment.Guid));
			sqlParameters.Add(DBClient.AddParameters("FileMimeType", SqlDbType.NVarChar, objEmialAttechment.FileMimeType));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
			var Id = CommonDAL.ExecuteScalar("QueuedEmail_InsertAttechment", sqlParameters.ToArray());
			return Convert.ToInt32(Id);
		}
		public virtual List<EmialAttechment> GetEmailAttechmentOverGuid(Guid Guid)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("Guid", SqlDbType.UniqueIdentifier, Guid));
			return CommonDAL.ExecuteProcedure<EmialAttechment>("QueuedEmail_GetAllAttechmentOverGuid", sqlParameters.ToArray()).ToList();
		}
		public virtual void DeleteEmailAttechment(int QueuedEmailId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("QueuedEmailId", SqlDbType.Int, QueuedEmailId));
			CommonDAL.Crud("QueuedEmail_DeleteAttechment", sqlParameters.ToArray());
		}
		public List<QueuedEmail> GetAllPendingQueuedEmailList()
		{
			return CommonDAL.ExecuteProcedure<QueuedEmail>("QueuedEmail_GetAllPendingQueuedEmailList").ToList();
		}
        public List<QueuedEmail> GetAllPendingQueuedEmailListForNotifyInstaller()
        {
            return CommonDAL.ExecuteProcedure<QueuedEmail>("GetAllPendingQueuedEmailListForNotifyInstaller").ToList();
        }
        public void DeleteQueuedEmailOverQueuedEmailId(int Id)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("QueuedEmailId", SqlDbType.Int, Id));
			CommonDAL.Crud("QueuedEmail_DeleteQueuedEmailOverQueuedEmailId", sqlParameters.ToArray());
		}
		public void EmailSentSchedulerTimeMaintain(int flag)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("flag", SqlDbType.Int, flag));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("ScheduleTask_EmailSentSchedulerTimeMaintain", sqlParameters.ToArray());
		}
		public int GetEmailSentSchedulerIntervalTime()
		{
			return Convert.ToInt32(CommonDAL.ExecuteScalar("ScheduleTask_GetEmailSentSchedulerIntervalTime"));
		}
		public bool GetScheduleTaskRunningStatus(string TaskName)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("TaskName", SqlDbType.NVarChar, TaskName));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            return Convert.ToBoolean(CommonDAL.ExecuteScalar("GetScheduleTaskRunningStatus", sqlParameters.ToArray()));
		}
		#endregion
	}
}

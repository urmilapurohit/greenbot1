using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity.Email;
using FormBot.Helper;
using FormBot.Entity;
using FormBot.Entity.Settings;

namespace FormBot.BAL.Service
{
    public interface IEmailBAL
    {
        IList<EmailMessage> GetAllMessages(long maxMessageId, int id_acct);
        IList<EmailFolder> GetFolders(int id_acct);

        /// <summary>
        /// Create Template
        /// </summary>
        /// <param name="emailTemplate">email Template</param>
        /// <returns>The List Of Email Template</returns>
        int CreateTemplate(EmailTemplate emailTemplate);

        /// <summary>
        /// Email Template Listing
        /// </summary>
        /// <param name="pageNumbe">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort column.</param>
        /// <param name="sortDir">The sort direction.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>The List of Email Template</returns>
        List<EmailTemplate> GetEmailTemplateList(int pageNumbe, int pageSize, string sortCol, string sortDir, string templateName, string subject);

        /// <summary>
        /// Delete Email Template
        /// </summary>
        /// <param name="templateID">The template identifier.</param>
        void DeleteEmailTemplate(int templateID);

        /// <summary>
        /// Get Email Template by TemplateID
        /// </summary>
        /// <param name="templateID">The template identifier.</param>
        /// <returns>Get Single Email Template</returns>
        EmailTemplate GetEmailTemplateByID(int templateID);

        /// <summary>
        /// Gets the email body.
        /// </summary>
        /// <param name="emailInfo">The email information.</param>
        /// <param name="emailTemplate">The email template.</param>
        /// <returns>email string</returns>
        string GetEmailBody(EmailInfo emailInfo, EmailTemplate emailTemplate);

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
        /// <returns>emai llist</returns>
        List<EmailThread> GetEmailThreadByJobId(int jobId, string senderOrReceiverName, DateTime? fromDate, DateTime? toDate, string title, bool isArchived, int pageNumber, int pageSize, int UserId);

        /// <summary>
        /// Gets the messages by email thread identifier.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        /// <param name="id_Acct">The id_ acct.</param>
        /// <returns>message list</returns>
        List<EmailMessageWithAttachments> GetMessagesByEmailThreadId(long emailThreadId, int id_Acct);

        /// <summary>
        /// Gets the thread members.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>string list</returns>
        List<string> GetThreadMembers(int jobId, int userId);

        /// <summary>
        /// Updates the job email thread as archived.
        /// </summary>
        /// <param name="emailThreadId">The email thread identifier.</param>
        void UpdateJobEmailThreadAsArchived(long emailThreadId);

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
        /// <returns>email list</returns>
        List<EmailThread> CreateJobEmailThread(int JobId, int Id_AcctId, string Sender, string Receiver, string SenderEmail, string ReceiverEmail, string SubjectGroup, string Subject, int UserId, int IsSscAndSca);

        /// <summary>
        /// Gets the job wise users.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>user list</returns>
        List<JobWiseUsers> GetJobWiseUsers(int JobId, int UserId);

        /// <summary>
        /// Generals the compose and send mail.
        /// </summary>
        /// <param name="emailTempalte">The email tempalte.</param>
        /// <param name="mailTo">The mail to.</param>
        bool GeneralComposeAndSendMail(EmailTemplate emailTempalte, string mailTo);

        /// <summary>
        /// Generals the compose and send mail.
        /// </summary>
        /// <param name="emailTempalte">The email tempalte.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="composeEmail">The compose email.</param>
        void GeneralComposeAndSendMail(EmailTemplate emailTempalte, string mailTo, Email.ComposeEmail composeEmail);

		/// <summary>
		/// Composes the and send email.
		/// </summary>
		/// <param name="emailInfo">The email information.</param>
		/// <param name="mailTo">The mail to.</param>
		/// <param name="Attachments">The attachments.</param>
		void ComposeAndSendEmail(EmailInfo emailInfo, string mailTo, List<FormBot.Email.AttachmentData> Attachments = null, List<EmialAttechment> lstAttechment = null, Guid guid = default(Guid), string JobId = null);
        /// <summary>
		/// Composes the and send email.
		/// </summary>
		/// <param name="emailInfo">The email information.</param>
		/// <param name="mailTo">The mail to.</param>
        void ComposeAndSendEmailForAndroidAppUsers(EmailInfo emailInfo, string mailTo);
        /// <summary>
        /// Assigns the email to job.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="Id_msg">The id_msg.</param>
        void AssignEmailToJob(int JobId, string Id_msg);

        #region PreAprovalAndConnection Popup

        /// <summary>
        /// Gets the status job status for pre approval and connection.
        /// </summary>
        /// <param name="JobStage">The job stage.</param>
        /// <returns>job list</returns>
        List<JobStatusForPreApprovalAndConnection> GetStatusForPreApprovalAndConnection(int? JobStage = null);

        /// <summary>
        /// Adds the job comment for pre appr and connection.
        /// </summary>
        /// <param name="jobCommentForPreApprAndConn">The job comment for pre appr and connection.</param>
        void AddUpdateJobCommentForPreApprAndConn(JobCommentForPreApprAndConn jobCommentForPreApprAndConn);

        /// <summary>
        /// Gets the job status and comment.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="PreApprovalAndConnectionId">PreApprovalAndConnectionId.</param>
        /// <returns>job comment</returns>
        JobCommentForPreApprAndConn GetJobStatusAndComment(int JobId, int PreApprovalAndConnectionId);

        /// <summary>
        /// Saves the job email conversation for pre and connection.
        /// </summary>
        /// <param name="jobEmailConversationForPreAndConn">The job email conversation for pre and connection.</param>
        void SaveJobEmailConversationForPreAndConn(JobEmailConversationForPreAndConn jobEmailConversationForPreAndConn);

        /// <summary>
        /// Gets the message for job email conversation for pre and connection.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>email message</returns>
        List<JobEmailMessageInfo> GetMessageForJobEmailConversationForPreAndConn(int JobId);

        /// <summary>
        /// Gets the status for pre approval and connection for complaince.
        /// </summary>
        /// <param name="JobStage">The job stage.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>general class list</returns>
        List<GeneralClass> GetStatusForPreApprovalAndConnectionForComplaince(int JobStage, int UserTypeId);

		#endregion

		#region Email Service
		void InsertUpdateQueuedEmail(QueuedEmail queuedEmail, bool IsUpdate = false);
		int InsertEmailAttechment(EmialAttechment objEmialAttechment);
		List<EmialAttechment> GetEmailAttechmentOverGuid(Guid Guid);
		void DeleteEmailAttechment(int QueuedEmailId);
		List<QueuedEmail> GetAllPendingQueuedEmailList();
		void DeleteQueuedEmailOverQueuedEmailId(int Id);
		void EmailSentSchedulerTimeMaintain(int flag);
		int GetEmailSentSchedulerIntervalTime();
		#endregion

	}
}

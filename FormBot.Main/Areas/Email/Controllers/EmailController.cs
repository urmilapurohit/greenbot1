using System.Collections.Generic;
using System.Web.Mvc;
using FormBot.Email;
using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FormBot.Entity.Email;
using FormBot.BAL.Service;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using FormBot.Helper;
using FormBot.Main.Controllers;
using System.Data;
using FormBot.BAL;
using FormBot.Entity;
using FormBot.BAL.Service.Documents;
using Ionic.Zip;
using FormBot.Main.Infrastructure;

namespace FormBot.Main.Areas.Email.Controllers
{
    /// <summary>
    /// Email Controller
    /// </summary>
    [EmailOnActionAttribute]
    public partial class EmailController : BaseController
    {
        #region Properties
        /// <summary>
        /// The _email service
        /// </summary>       
        protected bool errorOccured = false;
        protected string name = string.Empty;
        protected string tmp_name = string.Empty;
        protected int size = 0;
        protected string mime_type = string.Empty;
        protected string error = string.Empty;
        protected string inlineImage = "false";
        protected string url = "";
        protected bool flashUpload = true;

        private readonly Logger _log = new Logger();
        private readonly IEmailBAL _emailService;
        private readonly IUserBAL _userBAL;
        private readonly ICreateJobBAL _job;
        private readonly IResellerBAL _reseller;
        private readonly IDocumentsBAL _documentsBAL;
        private readonly IEmailBAL _emailBAL;
        #endregion

        #region Constructor
        /// <summary>
        /// EmailController
        /// </summary>
        /// <param name="emailService"></param>
        /// <param name="userBAL"></param>
        /// <param name="job"></param>
        /// <param name="reseller"></param>
        /// <param name="documentsBAL"></param>
        /// <param name="emailBAL"></param>
        public EmailController(IEmailBAL emailService, IUserBAL userBAL, ICreateJobBAL job, IResellerBAL reseller, IDocumentsBAL documentsBAL, IEmailBAL emailBAL)
        {
            this._emailService = emailService;
            this._userBAL = userBAL;
            this._job = job;
            this._reseller = reseller;
            this._documentsBAL = documentsBAL;
            this._emailBAL = emailBAL;
        }
        #endregion

        #region Events

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Return the Index action method</returns>
        [UserAuthorization]
        public ActionResult Index()
        {
            Account acct = Session[Constants.sessionAccount] as Account;
            if (acct == null)
            {
                Response.ClearHeaders();
                return RedirectToAction("EmailSettings");
            }

            CheckEmail();

            var result = new { acct.Signature };

            ViewBag.signature = Json(result);
            ViewBag.email = acct.Email;

            var lstEmailFolder = _emailService.GetFolders(acct.ID);

            bool isarchive = false;
            string scheduletype = "";
            string jobtype = "";
            string jobpriority = "";
            string searchtext = "";
            string fromdate = "";
            string todate = "";
            bool IsGst = false;
            bool jobref = true;
            bool jobdescription = true;
            bool jobaddress = true;
            bool jobclient = true;
            bool jobstaff = false;
            //bool notesdescription = false;
            //bool nottraded = false;
            //bool title = true;
            //bool nopreapprovals = false;
            //bool noconnections = false;
            bool ACT = true;
            bool NSW = true;
            bool NT = true;
            bool QLD = true;
            bool SA = true;
            bool TAS = true;
            bool WA = true;
            bool VIC = true;
            int StageId = 0;
            string solarcompanyid = "";
            int SolarCompanyId = 0;
            int ScheduleType = !string.IsNullOrEmpty(scheduletype) ? Convert.ToInt32((SystemEnums.JobScheduleType)Enum.Parse(typeof(SystemEnums.JobScheduleType), scheduletype).GetHashCode()) : 0;
            int JobType = !string.IsNullOrEmpty(jobtype) ? Convert.ToInt32((SystemEnums.JobType)Enum.Parse(typeof(SystemEnums.JobType), jobtype).GetHashCode()) : 0;
            int JobPriority = !string.IsNullOrEmpty(jobpriority) ? Convert.ToInt32((SystemEnums.JobPriority)Enum.Parse(typeof(SystemEnums.JobPriority), jobpriority).GetHashCode()) : 0;
            if (ProjectSession.UserTypeId == 4)
            {
                SolarCompanyId = ProjectSession.SolarCompanyId;
            }
            else
            {
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;
            }

            DateTime? FromDate = null, ToDate = null;
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
            {
                FromDate = Convert.ToDateTime(fromdate);
                ToDate = Convert.ToDateTime(todate);
            }

            IList<FormBot.Entity.JobList> lstJobs = _job.GetJobList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 1, 2147483647, null, null, Convert.ToInt32(ConfigurationManager.AppSettings["UrgentJobDay"].ToString()), StageId, Convert.ToString(SolarCompanyId), isarchive, ScheduleType, JobType, JobPriority, searchtext, FromDate, ToDate,
                                                                     IsGst,jobref, jobdescription, jobaddress, jobclient, jobstaff, true, true, true, true, true, true, true, true, true, true, ACT, NSW, NT, QLD, SA, TAS, WA, VIC, 0, 0, null, null);
            List<SelectListItem> lstJobsSelectList = new List<SelectListItem>();
            if (lstJobs.Count > 0)
            {
                foreach (var item in lstJobs)
                {
                    lstJobsSelectList.Add(new SelectListItem()
                    {
                        Text = item.Title,
                        Value = Convert.ToString(item.JobID)
                    });
                }
            }

            ViewBag.JobsList = lstJobsSelectList;
            return View(lstEmailFolder);
        }

        /// <summary>
        /// Checks the new email.
        /// </summary>
        /// <param name="maxMessageId">The maximum message identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Return the Index action method</returns>
        public JsonResult CheckNewEmail(long maxMessageId, int pageIndex, int folderId, string folderName)
        {
            CheckEmail();
            Account acct = Session[Constants.sessionAccount] as Account;
            List<EmailMessage> lstWebMail = new List<EmailMessage>();
            IList<EmailMessage> resultMessages = _emailService.GetAllMessages(maxMessageId, acct.ID);
            CheckMail checkMail = new CheckMail();
            foreach (var item in resultMessages.ToList())
            {
                Dictionary<string, string> shortAndFullDate = checkMail.GetShortAndFullDate(acct, item.msg_date);
                item.from = checkMail.SplitNameAndEmail(item.from);
                item.to = checkMail.SplitNameAndEmail(item.to);
                item.short_date = shortAndFullDate["ShortDate"];
                item.full_date = shortAndFullDate["FullDate"];
                item.MessageClass = (item.flags == true) ? "opened" : "unread";
                item.has_attachments = item.has_attachments;
                if (item.has_attachments)
                {
                    string xmlBody = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages_bodies'/><folder id='" + folderId
                        + "'><full_name><![CDATA[" + folderName + "]]></full_name><message id='" + item.id + "' charset='-1'></message></folder></webmail>";

                    xmlBody = GetResult(xmlBody).OuterXml;
                }

                item.flags = item.flags;
                lstWebMail.Add(item);
            }
            ////int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
            ////lstWebMail = lstWebMail.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            string mailListHtml = RenderRazorViewToString("_MailList", lstWebMail);
            if (lstWebMail.Count() > 0)
            {
                maxMessageId = lstWebMail.Max(w => w.id);
            }

            var finalResult = new { Result = mailListHtml, MaxMessageId = maxMessageId };
            return this.Json(finalResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// _s the mail list.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="typeId">The type identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="sort_order">The sort_order.</param>
        /// <returns>
        /// Return the Index action method
        /// </returns>
        public JsonResult _MailList(string folderId, string typeId, long pageIndex, string sort_order = "0")
        {
            Account acct = Session[Constants.sessionAccount] as Account;
            string xmlText = string.Empty;
            if (typeId == "1")
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='base'/></webmail>";
            }
            else if (typeId == "10")
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages'/><param name='id_acct' value='" + acct.ID
                    + "'/><param name='page' value='1'/><param name='sort_field' value='0'/><param name='sort_order' value='0'/><folder id='" + folderId
                    + "'></folder><look_for fields='0'><![CDATA[]]></look_for></webmail>";
            }
            else
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='folders_base'/><param name='background' value='2'/></webmail>";
            }

            string resultXml = GetResult(xmlText).OuterXml;

            XDocument resultDocument = XDocument.Parse(resultXml);

            var lstMessages = resultDocument.Descendants("webmail").Elements("messages").Elements("message").Where(x => x.Element("folder").Attribute("id").Value == folderId).ToList();

            ////((@PageNumber * @PageSize) - (@PageSize - 1)) AND (@PageNumber * @PageSize) 

            List<EmailMessage> lstWebMail = new List<EmailMessage>();

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());

            CheckMail checkMail = new CheckMail();
            foreach (var item in lstMessages)
            {
                EmailMessage emailMessage = new EmailMessage()
                {
                    id = Convert.ToInt64(item.Attribute("id").Value),
                    from = checkMail.SplitNameAndEmail(item.Element("from").Value).Replace("&gt;", "").Replace("&lt;", ""),
                    to = item.Element("to").Value,
                    subject = item.Element("subject").Value,
                    full_date = item.Element("full_date").Value,
                    short_date = item.Element("short_date").Value,
                    has_attachments = item.Attribute("has_attachments").Value == "1",
                    flags = item.Attribute("flags").Value == "0" ? false : true
                };

                emailMessage.MessageClass = (emailMessage.flags == true) ? "opened" : "unread";
                lstWebMail.Add(emailMessage);
            }

            long maxMessageId = 0;
            if (lstWebMail.Count() > 0)
            {
                maxMessageId = lstWebMail.Max(w => w.id);
            }

            if (lstWebMail.Count() > 0)
            {
                if (sort_order == "0")
                {
                    if (pageIndex == 0)
                    {
                        lstWebMail = lstWebMail.OrderByDescending(o => o.id).Where(w => w.id <= maxMessageId).Take(pageSize).ToList();
                        pageIndex = lstWebMail.Min(w => w.id);
                    }
                    else
                    {
                        lstWebMail = lstWebMail.OrderByDescending(o => o.id).Where(w => w.id < pageIndex).Take(pageSize).ToList();
                        if (lstWebMail.Count() > 0)
                        {
                            pageIndex = lstWebMail.Min(w => w.id);
                        }
                    }
                }
                else
                {
                    long minMessageId = lstWebMail.Min(m => m.id);
                    if (pageIndex == 0)
                    {
                        lstWebMail = lstWebMail.OrderBy(o => o.id).Where(w => w.id >= minMessageId).Take(pageSize).ToList();
                        if (lstWebMail.Count() > 0)
                        {
                            pageIndex = lstWebMail.Max(w => w.id);
                        }
                    }
                    else
                    {
                        lstWebMail = lstWebMail.OrderBy(o => o.id).Where(w => w.id > pageIndex).Take(pageSize).ToList();
                        if (lstWebMail.Count() > 0)
                        {
                            pageIndex = lstWebMail.Max(w => w.id);
                        }
                    }
                }
            }

            string returnParitalHtml = RenderRazorViewToString("_MailList", lstWebMail);

            bool isLast = false;
            if (lstWebMail.Count() < Convert.ToInt32(FormBot.Helper.ProjectConfiguration.GetPageSizeForMail))
            {
                isLast = true;
            }

            var result = new { Result = returnParitalHtml, MaxMessageId = maxMessageId, IsLast = isLast, MessageIdForScroll = pageIndex };
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Searches the mail list.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="search">The search.</param>
        /// <param name="sort_order">The sort_order.</param>
        /// <returns>Return the Index action method</returns>
        public JsonResult SearchMailList(string folderId, string folderName, string search, string sort_order = "0")
        {
            CheckMail checkMail = new CheckMail();
            string xmlText = string.Empty;
            xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages'/><param name='id_acct' value='" +
                folderId + "' /><param name='page' value='1' /><param name='sort_field' value='0' /><param name='sort_order' value='" + sort_order + "' /><folder id='" + folderId +
                "'><full_name><![CDATA[" + folderName + "]]></full_name></folder><look_for fields='0'><![CDATA[" + search + "]]></look_for></webmail>";

            string resultXml = GetResult(xmlText).OuterXml;

            XDocument resultDocument = XDocument.Parse(resultXml);

            var lstMessages = resultDocument.Descendants("webmail").Elements("messages").Elements("message").Where(x => x.Element("folder").Attribute("id").Value == folderId).ToList();
            List<EmailMessage> lstWebMail = new List<EmailMessage>();
            foreach (var item in lstMessages)
            {
                EmailMessage emailMessage = new EmailMessage()
                {
                    id = Convert.ToInt64(item.Attribute("id").Value),
                    //from = item.Element("from").Value,
                    from = checkMail.SplitNameAndEmail(item.Element("from").Value).Replace("&gt;", "").Replace("&lt;", ""),
                    to = item.Element("to").Value,
                    subject = item.Element("subject").Value,
                    full_date = item.Element("full_date").Value,
                    short_date = item.Element("short_date").Value,
                    has_attachments = item.Attribute("has_attachments").Value == "1",
                    flags = item.Attribute("flags").Value == "0" ? false : true
                };

                emailMessage.MessageClass = (emailMessage.flags == true) ? "opened" : "unread";
                lstWebMail.Add(emailMessage);
            }

            string returnParitalHtml = RenderRazorViewToString("_MailList", lstWebMail);
            long maxMessageId = 0;
            if (lstWebMail.Count() > 0)
            {
                maxMessageId = lstWebMail.Max(w => w.id);
            }

            //int pageIndex = lstWebMail;

            var result = new { Result = returnParitalHtml, MaxMessageId = maxMessageId };
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Messages the seen.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Return the Index action method</returns>
        public JsonResult MessageSeen(string messageId, int folderId, string folderName)
        {
            string xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='operation_messages'/><param name='request' value='mark_read'/><messages><look_for fields='0'><![CDATA[]]></look_for><to_folder id
            ='-1'><full_name><![CDATA[]]></full_name></to_folder><folder id='" + folderId + "'><full_name><![CDATA[" + folderName + "]]></full_name></folder><message id='" + messageId + "' charset='-1'><folder id='" + folderId + "'><full_name><![CDATA[" + folderName + "]]></full_name></folder></message></messages></webmail>";
            var result = GetResult(xmlText);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Moves the message to folder.
        /// </summary>
        /// <param name="messageIds">The messageIds.</param>
        /// <param name="fromFolderId">From folder identifier.</param>
        /// <param name="toFolderId">To folder identifier.</param>
        /// <param name="fromFolderName">Name of from folder.</param>
        /// <param name="toFolderName">Name of to folder.</param>
        /// <param name="fromTypeId">From type identifier.</param>
        /// <param name="toTypeId">To type identifier.</param>
        /// <returns>Return the Index action method</returns>
        public JsonResult MoveMessageToFolder(string messageIds, int fromFolderId, int toFolderId, string fromFolderName, string toFolderName, string fromTypeId, string toTypeId)
        {
            string messageIdsxml = "";
            foreach (var item in messageIds.Split(','))
            {
                messageIdsxml += "<message id='" + item + "' charset='-1' size='1441'><folder id='" + fromFolderId + "'><full_name><![CDATA[" + fromFolderName + "]]></full_name></folder></message>";
            }

            string xmlText = string.Empty;
            if (fromTypeId == "4" && toTypeId == "4")
            {
                xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='operation_messages'/><param
 name='request' value='delete'/><messages><look_for fields='0'><![CDATA[]]></look_for><to_folder id='-1'><full_name/></to_folder><folder id='" + fromFolderId + "'><full_name><![CDATA[" + fromFolderName + "]]></full_name></folder>  " + messageIdsxml + "</messages></webmail>";
            }
            else
            {
                xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='operation_messages'/><param
 name='request' value='move_to_folder'/><messages><look_for fields='0'><![CDATA[]]></look_for><to_folder
 id='" + toFolderId + "'><full_name><![CDATA[" + toFolderName + "]]></full_name></to_folder><folder id='" + fromFolderId + "'><full_name><![CDATA[" + fromFolderName + "]]></full_name></folder>  " + messageIdsxml + "</messages></webmail>";
            }

            var result = GetResult(xmlText);
            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Emails the settings.
        /// </summary>
        /// <returns>Return the Index action method</returns>
        [UserAuthorization]
        public ActionResult EmailSettings()
        {
            Account acct = Session[Constants.sessionAccount] as Account;
            EmailSignup model = new EmailSignup();

            #region Email configuration not required

            //Email configuration is not required at a time of first login
            model.IsRequired = true;
            #endregion
            if (acct != null)
            {
                model.ConfigurationEmail = acct.Email;
                model.IncomingMail = acct.MailIncomingHost;
                model.IncomingMailPort = acct.MailIncomingPort;
                model.Login = acct.MailIncomingLogin;
                model.OutgoingMail = acct.MailOutgoingHost;
                model.OutgoingMailPort = acct.MailOutgoingPort;
                model.ConfigurationPassword = acct.MailIncomingPassword;
                model.EmailSignature = acct.Signature;
                var result = new { acct.Signature };

                ViewBag.Signature = Json(result);

                if (acct.UserOfAccount != null && acct.UserOfAccount.Settings != null && acct.UserOfAccount.Settings.DefaultTimeZone != null)
                {
                    model.Def_TimeZone = acct.UserOfAccount.Settings.DefaultTimeZone;
                }
            }
            else
            {
                model.IncomingMailPort = 110;
                model.OutgoingMailPort = 25;
            }

            return View(model);
        }

        /// <summary>
        /// Emails the settings.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Return the Index action method</returns>
        [HttpPost]
        [ValidateInput(false)]
        [UserAuthorization]
        public ActionResult EmailSettings(EmailSignup model)
        {
            string xmlString = string.Empty;
            ModelState.Remove("Login");
            if (ModelState.IsValid)
            {
                Session[Constants.sessionAccount] = null;
                model.Login = model.ConfigurationEmail;
                ////int accountEmailExists = _userBAL.CheckAccountEmailExists(model.ConfigurationEmail, ProjectSession.LoggedInUserId);
                ////if (accountEmailExists > 0)
                ////{
                ////    this.ShowMessage(SystemEnums.MessageType.Error, "User with same email configuration already exists.", false);
                ////    return View("EmailSettings", model);
                ////}
                xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + model.ConfigurationEmail
                    + "]]></param><param name='mail_inc_login'><![CDATA[" + model.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + model.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + model.IncomingMail
                    + "]]></param><param name='mail_inc_port' value='" + model.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + model.OutgoingMail
                    + "]]></param><param name='mail_out_port' value='" + model.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";


                //Account getAccount = Account.LoadFromDb(model.ConfigurationEmail, model.ConfigurationEmail, model.ConfigurationPassword);
                //if(!(Convert.ToInt32(getAccount.MailIncomingPort) == model.IncomingMailPort && Convert.ToInt32(getAccount.MailOutgoingPort) == model.OutgoingMailPort &&
                //   getAccount.MailIncomingHost == model.IncomingMail && getAccount.MailOutgoingHost == model.OutgoingMail))
                //{
                //    getAccount.MailIncomingPort = model.IncomingMailPort != null ? Convert.ToInt32(model.IncomingMailPort) : 0;
                //    getAccount.MailOutgoingPort = model.OutgoingMailPort != null ? Convert.ToInt32(model.OutgoingMailPort) : 0;
                //    getAccount.MailIncomingHost = model.IncomingMail;
                //    getAccount.MailOutgoingHost = model.OutgoingMail;

                //    getAccount.Update(false);
                //}

                CheckMail checkMail = new CheckMail();
                var result = checkMail.GetMessages(xmlString);
                XDocument xDocument = XDocument.Parse(result.OuterXml);
                var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
                {
                    ViewBag.ErrorMessage = error.Value;
                }
                else
                {
                    Account acct = Session[Constants.sessionAccount] as Account;

                    if (acct != null)
                    {
                        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='signature'/><param name='id_acct' value='" + acct.ID
                            + "'/><signature type='0' opt='1'><![CDATA[" + model.EmailSignature + "]]></signature></webmail>";
                        result = checkMail.GetMessages(xmlString);
                        _userBAL.EmailMappingInsertUpdate(acct.ID, ProjectSession.LoggedInUserId);

                        ////This will Update TimeZone
                        xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='update'/><param name='request' value='settings'/><param name='id_acct' value='" + acct.ID
                            + "'/><settings id_acct='" + acct.ID + "' msgs_per_page='10000' contacts_per_page='20' auto_checkmail_interval='0' allow_dhtml_editor='1' def_charset_out='65001' def_timezone='" + model.Def_TimeZone + "' time_format='1' view_mode='1'><def_skin><![CDATA[AfterLogic]]></def_skin><def_lang><![CDATA[English]]></def_lang></settings></webmail>";
                        checkMail.GetMessages(xmlString);

                        #region Email configuration not required

                        ProjectSession.IsUserEmailAccountConfigured = true;
                        #endregion
                    }

                    return RedirectToAction("Index");
                }
            }

            var resultSignature = new { Signature = model.EmailSignature };
            ViewBag.Signature = Json(resultSignature);
            return View("EmailSettings", model);
        }

        /// <summary>
        /// Adds the new folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Return the Index action method</returns>
        public JsonResult AddNewFolder(string folderName)
        {
            Account acct = Session[Constants.sessionAccount] as Account;
            EmailSignup model = new EmailSignup();
            string xmlString = string.Empty;
            string message = string.Empty;
            bool success = false;
            int newFolderId = 0;
            int newFolderType = 0;
            if (acct != null && !string.IsNullOrEmpty(folderName))
            {
                xmlString = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='new'/><param name='request' value='folder'/><param name='id_acct' value='" + acct.ID
                    + "'/><param name='id_parent' value='-1'/><param name='full_name_parent'><![CDATA[]]></param><param name='name'><![CDATA[" + folderName.Trim() + "]]></param><param name='create' value='0' /></webmail>";
                CheckMail checkMail = new CheckMail();
                var result = checkMail.GetMessages(xmlString);
                XDocument xDocument = XDocument.Parse(result.OuterXml);
                var error = xDocument.Descendants("webmail").Elements("error").FirstOrDefault();
                if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
                {
                    success = false;
                    ViewBag.ErrorMessage = error.Value;
                    message = error.Value;
                }
                else
                {
                    success = true;
                    var foldersList = xDocument.Descendants("folders_list").Descendants("folder").LastOrDefault();
                    newFolderId = Convert.ToInt32(foldersList.Attribute("id").Value);
                    newFolderType = Convert.ToInt32(foldersList.Attribute("type").Value);
                }
            }
            else
            {
                success = false;
                message = "You are not authenticated. Please login again";
            }

            return Json(new { Success = success, Message = message, id = newFolderId, type = newFolderType }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="composeMail">The compose mail.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="selectedMessageid">The selected messageid.</param>
        /// <returns>json result</returns>
        [HttpPost]
        public JsonResult SendMail(ComposeEmail composeMail, string eventType, long? selectedMessageid)
        {
            bool isSuccess = true;
            var errorMessage = string.Empty;
            CheckMail checkMail = new CheckMail();
            var result = checkMail.SendMail(composeMail, eventType, selectedMessageid);

            var updatedMessageId = 0;
            XDocument xDocumentEmail = XDocument.Parse(result.OuterXml);
            var error = xDocumentEmail.Descendants("webmail").Elements("error").FirstOrDefault();
            if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
            {
                isSuccess = false;
                errorMessage = error.Value;
            }
            else
            {
                if (eventType == "save" && selectedMessageid != null)
                {
                    try
                    {
                        XDocument xDocument = XDocument.Parse(result.OuterXml);
                        var updatedMessageInfo = xDocument.Descendants("webmail").Elements("update").FirstOrDefault();
                        updatedMessageId = Convert.ToInt32(updatedMessageInfo.Attribute("id").Value);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        errorMessage = ex.Message;
                    }
                }
            }

            return Json(new { updatedMessageId = updatedMessageId, isSuccess = isSuccess, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the mail detail.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="mailId">The mail identifier.</param>
        /// <param name="UID">The uid.</param>
        /// <param name="foldarName">Name of the foldar.</param>
        /// <returns>json result</returns>
        [HttpGet]
        public JsonResult GetMailDetail(string folderId, string mailId, string UID, string foldarName)
        {
            //this is getting only body of the mail
            var xmlRequest = @"<?xml version='1.0' encoding='utf-8'?>
                                <webmail>
                                <param name='action' value='get'/>
                                <param name='request' value='message'/>
                                <param name='mode' value='8'/>
                                <param name='background' value='1'/>
                                <param name='id' value='" + mailId + @"'/>
                                <param name='charset' value='-1'/>
                                <param name='uid'>
                                <![CDATA[" + UID + @"]]>
                                </param>
                                <folder id='" + folderId + @"'>
                                <full_name>
                                <![CDATA[" + foldarName + @"]]>
                                </full_name>
                                </folder>
                                </webmail>";

            var xml = GetResult(xmlRequest).OuterXml;
            var d = XDocument.Parse(xml);
            var lst = d.Descendants("webmail").Elements("message").Elements("reply_html").ToList();

            var body = string.Empty;
            foreach (var s in lst)
            {
                body = s.Value.Replace("wmx_src", "src");
                body = body.Replace("wmx_", "");
            }

            var filename = string.Empty;
            var download = string.Empty;
            var dynamicName = string.Empty;
            var attachLink = string.Empty;

            var attachment = d.Descendants("webmail").Elements("message").Elements("attachments").Elements("attachment").ToList();
            string attach = string.Empty;
            download = "1";
            foreach (var a in attachment)
            {
                filename = a.Element("filename").Value;
                filename = filename.Replace(' ', '+');
                var array = a.Element("download").Value.Split('&');
                dynamicName = array[2];
                dynamicName = dynamicName.Replace("tmp_filename=", "");
                attachLink = Url.Action("DownloadEmailAttachmnet", "Email", new { area = "Email" }) + "?_download=1&file_name=" + filename + "&" + dynamicName;
                attach += "<li> <a href=" + attachLink + "> <img src='" + Url.Content("~/images/attach_document.png") + "' alt=''/>" + a.Element("filename").Value + " </a> </li>";
            }

            var xmlText = string.Empty;
            xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages'/><param name='id_acct' value='" +
                folderId + "' /><param name='page' value='1' /><param name='sort_field' value='0' /><param name='sort_order' value='0' /><folder id='" + folderId +
                "'><full_name><![CDATA[" + foldarName + "]]></full_name></folder></webmail>";

            //string xml = GetResult(xmlText).OuterXml;

            //XDocument d = XDocument.Parse(xml);

            var xmlContent = GetResult(xmlText).OuterXml;

            var dContent = XDocument.Parse(xmlContent);

            var lstContent = dContent.Descendants("webmail").Elements("messages").Elements("message").Where(x => x.Attribute("id").Value == mailId).ToList();

            Account acct = Session[Constants.sessionAccount] as Account;
            var mail = new ComposeEmail();
            CheckMail c = new CheckMail();
            MailBee.Mime.MailMessage message = c.LoadMessagesByEmailThreadID(Convert.ToInt32(mailId), foldarName, acct.Email, acct.ID);
            if (message != null)
            {
                foreach (var s in lstContent)
                {
                    mail.From = message.From.AsString;
                    mail.To = message.To.AsString;
                    mail.Cc = message.Cc.AsString;
                    mail.Bcc = message.Bcc.AsString;
                    mail.Subject = string.IsNullOrEmpty(message.Subject) ? "&nbsp;&nbsp;" : message.Subject;
                    mail.FullDate = s.Element("full_date").Value;
                    mail.ShortDate = s.Element("short_date").Value;
                    mail.Body = new innerBody() { body = body };
                    mail.Attachment = attach;
                }
            }

            var result = new { Result = mail };

            return this.Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Mails the detail for reply.
        /// </summary>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="mailId">The mail identifier.</param>
        /// <param name="UID">The uid.</param>
        /// <param name="foldarName">Name of the foldar.</param>
        /// <param name="isFromDrafts">if set to <c>true</c> [is from drafts].</param>
        /// <returns>json result</returns>
        [HttpGet]
        public JsonResult MailDetailForReply(string folderId, string mailId, string UID, string foldarName, bool isFromDrafts = false)
        {
            //this is getting only body of the mail
            var xmlRequest = @"<?xml version='1.0' encoding='utf-8'?>
                                <webmail>
                                <param name='action' value='get'/>
                                <param name='request' value='message'/>
                                <param name='mode' value='8'/>
                                <param name='background' value='1'/>
                                <param name='id' value='" + mailId + @"'/>
                                <param name='charset' value='-1'/>
                                <param name='uid'>
                                <![CDATA[" + UID + @"]]>
                                </param>
                                <folder id='" + folderId + @"'>
                                <full_name>
                                <![CDATA[" + foldarName + @"]]>
                                </full_name>
                                </folder>
                                </webmail>";

            var xml = GetResult(xmlRequest).OuterXml;
            var d = XDocument.Parse(xml);
            var lst = d.Descendants("webmail").Elements("message").Elements("reply_html").ToList();

            var body = string.Empty;
            foreach (var s in lst)
            {
                body = s.Value.Replace("wmx_src", "src");
                body = body.Replace("wmx_", "");
            }

            var filename = string.Empty;
            var download = string.Empty;
            var dynamicName = string.Empty;
            var attachLink = string.Empty;

            var attachment = d.Descendants("webmail").Elements("message").Elements("attachments").Elements("attachment").ToList();
            string attach = string.Empty;
            download = "1";
            foreach (var a in attachment)
            {
                filename = a.Element("filename").Value;
                if (a.Element("download").HasElements)
                {
                    //var array = a.Element("download").Value.Split('&');
                    //dynamicName = array[5];
                    //dynamicName = dynamicName.Replace("tmp_filename=", "");
                    //attachLink = "/Email/Email/DownloadEmailAttachmnet?_download=1&file_name=" + filename + "&temp_filename=" + dynamicName + "";
                    //attach += "<li> <a href=" + attachLink + "> <img src='../images/attach_document.png' alt=''/>" + filename + " </a> </li>";

                    filename = a.Element("filename").Value;
                    filename = filename.Replace(' ', '+');
                    var array = a.Element("download").Value.Split('&');
                    dynamicName = array[2];
                    dynamicName = dynamicName.Replace("tmp_filename=", "");
                    attachLink = Url.Action("DownloadEmailAttachmnet", "Email", new { area = "Email" }) + "?_download=1&file_name=" + filename + "&" + dynamicName;
                    attach += "<li> <a href=" + attachLink + "> <img src='" + Url.Content("~/images/attach_document.png") + "' alt=''/>" + a.Element("filename").Value + " </a> </li>";
                }
            }

            Account acct = Session[Constants.sessionAccount] as Account;

            var xmlText = string.Empty;
            //xmlText = @"<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages'/><param name='id_acct' value='" +
            //    acct.ID + "' /><param name='page' value='1' /><param name='sort_field' value='0' /><param name='sort_order' value='0' /><folder id='" + folderId +
            //    "'><full_name><![CDATA[" + foldarName + "]]></full_name></folder></webmail>";
            xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages_bodies'/><folder id='" + folderId
                + "'><full_name><![CDATA[" + foldarName + "]]></full_name><message id='" + mailId + "' charset='-1'></message></folder></webmail>";

            //string xml = GetResult(xmlText).OuterXml;

            //XDocument d = XDocument.Parse(xml);

            var xmlContent = GetResult(xmlText).OuterXml;

            var dContent = XDocument.Parse(xmlContent);

            var lstContent = dContent.Descendants("webmail").Elements("message").Where(x => x.Attribute("id").Value == mailId).ToList();
            var lstAttachments = lstContent.Descendants("attachments").Descendants("attachment").ToList();
            var mail = new ComposeEmail();
            CheckMail c = new CheckMail();
            foreach (var s in lstContent)
            {
                var item = s.Element("headers");
                mail.From = c.SplitNameAndEmail(item.Element("from").Element("full").Value, false);
                var to = item.Element("to").Value;
                if (!string.IsNullOrEmpty(to) && to.Contains(","))
                {
                    var splitToByComma = new List<string>();
                    foreach (var detail in to.Split(',').ToList())
                    {
                        splitToByComma.Add(c.SplitNameAndEmail(detail.Trim()).Replace("&lt;", "").Replace("&gt;", "").Trim());
                    }

                    splitToByComma = splitToByComma.Distinct().ToList();
                    splitToByComma.Remove(acct.Email);
                    mail.To = string.Join(",", splitToByComma);
                }
                else
                    mail.To = to;

                var cc = item.Element("cc").Value;
                if (!string.IsNullOrEmpty(cc))
                {
                    var splitToByComma = new List<string>();
                    foreach (var detail in cc.Split(',').ToList())
                    {
                        splitToByComma.Add(c.SplitNameAndEmail(detail.Trim().Replace("&lt;", "").Replace("&gt;", "").Trim()));
                    }

                    splitToByComma = splitToByComma.Distinct().ToList();
                    splitToByComma.Remove(acct.Email);
                    mail.Cc = string.Join(",", splitToByComma);
                }

                ////mail.Cc = item.Element("cc").Value;
                mail.Bcc = item.Element("bcc").Value;
                mail.Subject = item.Element("subject").Value;
                mail.FullDate = item.Element("full_date").Value;
                mail.ShortDate = item.Element("short_date").Value;
                string mess = string.Empty;
                if (isFromDrafts != true)
                {
                    mess = @"<br/>" + acct.Signature;
                    mess += "<br/>---- " + "OriginalMessage" + " ----<br/>";
                    mess += "<b>" + "From" + "</b>: " + EncodeHtml(mail.From.ToString()) + "<br/>";
                    mess += "<b>" + "To" + "</b>: " + EncodeHtml(Convert.ToString(to)) + "<br/>";
                    if (!string.IsNullOrEmpty(Convert.ToString(cc)))
                        mess += "<b>" + "CC" + "</b>: " + EncodeHtml(Convert.ToString(cc)) + "<br/>";
                    mess += "<b>" + "Sent" + "</b>: " + EncodeHtml(mail.FullDate) + "<br/>";
                    mess += "<b>" + "Subject" + "</b>: " + EncodeHtml(mail.Subject) + "<br/>";
                    body = mess + body;
                    mail.Body = new innerBody() { body = body };
                }
                else
                {
                    mail.Body = new innerBody() { body = body };
                }

                mail.Attachment = attach;
            }

            mail.Attachments = new List<AttachmentData>();
            foreach (var item in lstAttachments)
            {
                try
                {
                    List<string> lstDownload = item.Element("download").Value.Split('&').ToList();
                    var selectedFile = lstDownload.Where(s => s.Contains("tmp_filename") || s.Contains("temp_filename")).FirstOrDefault();
                    if (selectedFile != null)
                    {
                        var tmp_Fileame = selectedFile.Split('=').ToList();
                        mail.Attachments.Add(new AttachmentData
                        {
                            FileName = item.Element("filename").Value,
                            GeneratedName = tmp_Fileame[1]
                        });
                    }
                }
                catch (Exception ex)
                {
                    Helper.Log.WriteError(ex);
                }
            }

            var result = new { Result = mail };

            return this.Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Assigns the message to job.
        /// </summary>
        /// <param name="Jobid">The job identifier.</param>
        /// <param name="Id_msgs">The id_msgs.</param>
        /// <returns>json result</returns>
        [HttpPost]
        public ActionResult AssignMessageToJob(string Jobid, string Id_msgs)
        {
            int JobId = Convert.ToInt32(Jobid);
            _emailService.AssignEmailToJob(JobId, Id_msgs);
            return this.Json("");
        }

        /// <summary>
        /// Downloads the email attachmnet.
        /// </summary>
        /// <param name="_download">The _download.</param>
        /// <param name="file_name">The file_name.</param>
        /// <param name="temp_filename">The temp_filename.</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult DownloadEmailAttachmnet(string _download, string file_name, string temp_filename)
        {
            string userAgent; //Client browser
            var tempfilename = temp_filename;

            if (temp_filename != null)
            {
                try
                {
                    var buffer = new byte[0];

                    object tempFolder = Utils.GetTempFolderName();
                    if (tempFolder != null)
                    {
                        var safe_temp_file_name = Path.GetFileName(temp_filename);

                        var fullPath = Path.Combine(tempFolder.ToString(), safe_temp_file_name);
                        if (System.IO.File.Exists(fullPath))
                        {
                            using (var fs = System.IO.File.OpenRead(fullPath))
                            {
                                buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    else
                    {
                        //return;
                    }

                    var filename = file_name ?? temp_filename;
                    var download = _download;
                    //*************************************************************
                    //IE with cyrillic file names
                    //*************************************************************
                    string encodedFilename;
                    userAgent = Request.UserAgent;
                    if (userAgent.IndexOf("MSIE") > -1)
                    {
                        encodedFilename = Server.UrlPathEncode(filename);
                    }
                    else
                    {
                        encodedFilename = filename;
                    }
                    //**************************************************************
                    if (download != null)
                    {
                        Response.Clear();

                        if (string.Compare(download, "1", true, CultureInfo.InvariantCulture) == 0)
                        {
                            Response.Clear();
                            Response.ClearHeaders();
                            Response.ClearContent();
                            Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                            Response.AddHeader("Accept-Ranges", "bytes");
                            Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                            Response.AddHeader("Content-Transfer-Encoding", "binary");
                            Response.ContentType = "application/octet-stream";
                            
                           
                            //Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                            //Response.AddHeader("Accept-Ranges", "bytes");
                            //Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                            //Response.AddHeader("Content-Transfer-Encoding", "binary");
                            //Response.ContentType = "application/octet-stream";
                        }
                        else
                        {
                            var ext = Path.GetExtension(filename);
                            if (!string.IsNullOrEmpty(ext))
                            {
                                ext = ext.Substring(1, ext.Length - 1); // remove first dot
                            }

                            Response.ContentType = Utils.GetAttachmentMimeTypeFromFileExtension(ext);
                        }

                    }

                    Response.BinaryWrite(buffer);
                    //Response.Flush();
                }
                catch (Exception ex)
                {
                    FormBot.Email.Log.WriteException(ex);
                }
            }
            else
            {
                if (Request.QueryString["partID"] != null)
                {
                    var buffer = new byte[0];

                    try
                    {
                        var uid = HttpUtility.UrlDecode(Request.QueryString["uid"]);
                        var full_folder_name = Request.QueryString["full_folder_name"];
                        var partID = Request.QueryString["partID"];
                        var filename = Request.QueryString["filename"];
                        var download = Request.QueryString["download"];
                        var temp_file = Request.QueryString["tmp_filename"];
                        var temp_folder = Utils.GetTempFolderName();

                        //*************************************************************
                        //IE with cyrillic file names
                        //*************************************************************
                        string encodedFilename;
                        userAgent = Request.UserAgent;
                        if (userAgent.IndexOf("MSIE") > -1)
                        {
                            encodedFilename = Server.UrlPathEncode(filename);
                        }
                        else
                        {
                            encodedFilename = filename;
                        }
                        //**************************************************************

                        if (!string.IsNullOrEmpty(temp_file) && System.IO.File.Exists(Path.Combine(temp_folder, temp_file)))
                        {
                            using (var fs = System.IO.File.OpenRead(Path.Combine(temp_folder, temp_file)))
                            {
                                buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                            }
                        }
                        else
                        {
                            var mp = new MailProcessor(DbStorageCreator.CreateDatabaseStorage(Session[Constants.sessionAccount] as Account));
                            try
                            {
                                mp.Connect();
                                var fld = mp.GetFolder(full_folder_name);
                                if (fld != null)
                                {
                                    buffer = mp.GetAttachmentPart(uid, fld, partID);
                                }
                            }
                            finally
                            {
                                mp.Disconnect();
                            }

                            var tmp_filename = Utils.CreateTempFilePath(temp_folder, filename, true);
                            System.IO.File.WriteAllBytes(tmp_filename, buffer);
                        }

                        if (download != null)
                        {
                            Response.Clear();

                            if (string.Compare(download, "1", true, CultureInfo.InvariantCulture) == 0)
                            {
                                Response.Clear();
                                Response.ClearHeaders();
                                Response.ClearContent();
                                Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                                Response.AddHeader("Accept-Ranges", "bytes");
                                Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                                Response.AddHeader("Content-Transfer-Encoding", "binary");
                                Response.ContentType = "application/octet-stream";
                            

                                //Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                                //Response.AddHeader("Accept-Ranges", "bytes");
                                //Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                                //Response.AddHeader("Content-Transfer-Encoding", "binary");
                                //Response.ContentType = "application/octet-stream";
                            }
                            else
                            {
                                var ext = Path.GetExtension(filename);
                                if (!string.IsNullOrEmpty(ext))
                                {
                                    ext = ext.Substring(1, ext.Length - 1); // remove first dot
                                }

                                Response.ContentType = Utils.GetAttachmentMimeTypeFromFileExtension(ext);
                            }
                        }

                        Response.BinaryWrite(buffer);
                        //Response.Flush();

                    }
                    catch (Exception ex)
                    {
                        FormBot.Email.Log.WriteException(ex);
                    }
                }
                else if ((Request.QueryString["id_msg"] != null)
                    && (Request.QueryString["uid"] != null)
                    && (Request.QueryString["id_folder"] != null)
                    && (Request.QueryString["folder_path"] != null))
                {
                    try
                    {
                        var id_msg = int.Parse(Request.QueryString["id_msg"], CultureInfo.InvariantCulture);
                        var id_folder = long.Parse(Request.QueryString["id_folder"], CultureInfo.InvariantCulture);

                        WebMailMessage msg = null;
                        var mp = new MailProcessor(DbStorageCreator.CreateDatabaseStorage(Session[Constants.sessionAccount] as Account));
                        try
                        {
                            mp.Connect();
                            var fld = mp.GetFolder(id_folder);
                            if (fld != null)
                            {
                                msg = mp.GetMessage((fld.SyncType != FolderSyncType.DirectMode) ? (object)id_msg : HttpUtility.UrlDecode(Request.QueryString["uid"]), fld);
                            }
                        }
                        finally
                        {
                            mp.Disconnect();
                        }

                        if (msg.MailBeeMessage != null)
                        {
                            var subj = msg.MailBeeMessage.Subject;
                            //«\», «/», «?», «|», «*», «<», «>», «:»
                            var safeSubject = string.Empty;
                            for (var i = 0; i < subj.Length; i++)
                            {
                                if (subj[i] == '\\' || subj[i] == '|' || subj[i] == '/' ||
                                    subj[i] == '?' || subj[i] == '*' || subj[i] == '<' ||
                                    subj[i] == '>' || subj[i] == ':')
                                {
                                    continue;
                                }
                                safeSubject += subj[i];
                            }
                            safeSubject = safeSubject.TrimStart();
                            if (safeSubject.Length > 30)
                            {
                                safeSubject = safeSubject.Substring(0, 30).TrimEnd();
                            }
                            safeSubject = safeSubject.TrimEnd(new char[2] { '.', ' ' });
                            if (safeSubject.Length == 0)
                            {
                                safeSubject = "message";
                            }

                            string encodedMsgFilename;
                            userAgent = Request.UserAgent;
                            if (userAgent.IndexOf("MSIE") > -1)
                            {
                                encodedMsgFilename = Server.UrlPathEncode(safeSubject);
                                Response.AddHeader("Expires", "0");
                                Response.AddHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                                Response.AddHeader("Pragma", "public");
                            }
                            else
                            {
                                encodedMsgFilename = safeSubject;
                            }
                            //**************************************************************
                            var buffer = msg.MailBeeMessage.GetMessageRawData();
                            //**************************************************************

                            Response.Clear();
                            Response.ClearHeaders();
                            Response.ClearContent();
                            Response.ContentType = "application/octet-stream";
                            Response.AddHeader("Accept-Ranges", "bytes");
                            Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                            Response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}.eml""", encodedMsgFilename));
                            Response.AddHeader("Content-Transfer-Encoding", "binary");
                            Response.BinaryWrite(buffer);
                            

                            //Response.Clear();
                            //Response.ContentType = "application/octet-stream";
                            //Response.AddHeader("Accept-Ranges", "bytes");
                            //Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                            //Response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}.eml""", encodedMsgFilename));
                            //Response.AddHeader("Content-Transfer-Encoding", "binary");
                            //Response.BinaryWrite(buffer);
                            //Response.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        FormBot.Email.Log.WriteException(ex);
                    }
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Downloads the file from the server.
        /// </summary>
        /// <param name="file_name">The file_name.</param>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>ActionResult</returns>
        public ActionResult DownloadFileFromTheServer(string file_name, string fileFullPath)
        {
            string _download = "1";
            string userAgent; //Client browser
            ////string fileFullPath = Convert.ToString(Request.QueryString["fileFullPath"]);
            try
            {
                var buffer = new byte[0];

                var fullPath = fileFullPath;
                if (System.IO.File.Exists(fullPath))
                {
                    using (var fs = System.IO.File.OpenRead(fullPath))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                    }
                }

                var filename = file_name;
                var download = _download;
                //*************************************************************
                //IE with cyrillic file names
                //*************************************************************
                string encodedFilename;
                userAgent = Request.UserAgent;
                if (userAgent.IndexOf("MSIE") > -1)
                {
                    encodedFilename = Server.UrlPathEncode(filename);
                }
                else
                {
                    encodedFilename = filename;
                }
                //**************************************************************

                Response.Clear();

                if (string.Compare(download, "1", true, CultureInfo.InvariantCulture) == 0)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                    Response.AddHeader("Accept-Ranges", "bytes");
                    Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                    Response.AddHeader("Content-Transfer-Encoding", "binary");
                    Response.ContentType = "application/octet-stream";

                    //Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
                    //Response.AddHeader("Accept-Ranges", "bytes");
                    //Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                    //Response.AddHeader("Content-Transfer-Encoding", "binary");
                    //Response.ContentType = "application/octet-stream";
                }
                else
                {
                    var ext = Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(ext))
                    {
                        ext = ext.Substring(1, ext.Length - 1); // remove first dot
                    }
                    Response.ContentType = Utils.GetAttachmentMimeTypeFromFileExtension(ext);
                }

                Response.BinaryWrite(buffer);
                //Response.Flush();
            }
            catch (Exception ex)
            {
                FormBot.Email.Log.WriteException(ex);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Uploads this instance.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Upload()
        {
            WebmailSettings settings = (new WebMailSettingsCreator()).CreateWebMailSettings();

            inlineImage = (Request["inline_image"] == "1") ? "true" : "false";
            flashUpload = (Request["flash_upload"] == "0") ? false : true;
            string filename = string.Empty;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if ((file != null))
                {
                    name = Utils.EncodeHtml(Path.GetFileName(file.FileName)).Replace(@"'", @"\'");
                    size = file.ContentLength;
                    mime_type = Utils.EncodeHtml(file.ContentType);
                    if ((file.ContentLength < settings.AttachmentSizeLimit) || (settings.EnableAttachmentSizeLimit == false))
                    {
                        try
                        {
                            byte[] buffer;
                            using (Stream uploadStream = file.InputStream)
                            {
                                buffer = new byte[uploadStream.Length];
                                long numBytesToRead = uploadStream.Length;
                                long numBytesRead = 0;
                                while (numBytesToRead > 0)
                                {
                                    int n = uploadStream.Read(buffer, (int)numBytesRead, (int)numBytesToRead);
                                    if (n == 0)
                                        break;
                                    numBytesRead += n;
                                    numBytesToRead -= n;
                                }
                            }
                            if (buffer != null)
                            {
                                ////string tempFolder = Utils.GetTempFolderName(Session);
                                string tempFolder = Utils.GetTempFolderName();
                                filename = Utils.CreateTempFilePath(tempFolder, file.FileName);
                                tmp_name = Path.GetFileName(filename);
                                url = String.Format("download-view-attachment.aspx?download=0&temp_filename={0}", tmp_name);
                                using (FileStream fs = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write))
                                {
                                    fs.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            errorOccured = true;
                            error = (new WebmailResourceManagerCreator()).CreateResourceManager().GetString("UnknownUploadError");
                            FormBot.Email.Log.WriteException(ex);
                        }
                    }
                    else
                    {
                        errorOccured = true;
                        error = (new WebmailResourceManagerCreator()).CreateResourceManager().GetString("FileIsTooBig");
                        FormBot.Email.Log.WriteLine("upload", error);
                    }
                }
                else
                {
                    errorOccured = true;
                    error = (new WebmailResourceManagerCreator()).CreateResourceManager().GetString("NoFileUploaded");
                    FormBot.Email.Log.WriteLine("upload", error);
                }
            }

            return Json(new { tmp_name = tmp_name, filenameWithFullPath = filename });
        }

        /// <summary>
        /// UploadPhysicalFileAsAnAttachmentInEmail
        /// </summary>
        /// <param name="file_name">file_name</param>
        /// <param name="fileFullPath">fileFullPath</param>
        /// <returns>ActionResult</returns>
        public ActionResult UploadPhysicalFileAsAnAttachmentInEmail(string file_name, string fileFullPath)
        {
            WebmailSettings settings = (new WebMailSettingsCreator()).CreateWebMailSettings();
            //inlineImage = (Request["inline_image"] != null && Request["inline_image"] == "1") ? "true" : "false";
            flashUpload = true;
            string filename = string.Empty;
            var buffer = new byte[0];

            var fullPath = fileFullPath;
            if (System.IO.File.Exists(fullPath))
            {
                using (var fs = System.IO.File.OpenRead(fullPath))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                }

                try
                {
                    if (buffer != null)
                    {
                        string tempFolder = Utils.GetTempFolderName();
                        filename = Utils.CreateTempFilePath(tempFolder, file_name);
                        tmp_name = Path.GetFileName(filename);
                        url = String.Format("download-view-attachment.aspx?download=0&temp_filename={0}", tmp_name);
                        using (FileStream fs = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (IOException ex)
                {
                    errorOccured = true;
                    error = (new WebmailResourceManagerCreator()).CreateResourceManager().GetString("UnknownUploadError");
                    //FormBot.Email.Log.WriteException(ex);
                    //RECRegistryHelper.WriteToLogFile(ex.Message);
                    _log.LogException("UploadPhysicalFileAsAnAttachmentInEmail = ", ex);
                }
            }
            else
            {
                tmp_name = string.Empty;
                filename = string.Empty;
            }

            return Json(new { tmp_name = tmp_name, filenameWithFullPath = filename });
        }

        /// <summary>
        /// Reads the image file.
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult ReadImageFile()
        {
            string Content = string.Empty;
            Account acct = acct = Session[Constants.sessionAccount] as Account;
            if (acct == null)
            {
                ////Response.Redirect("default.aspx", true);
                return this.Content("");
            }

            string filename = Request.QueryString["filename"];
            if (!string.IsNullOrEmpty(filename))
            {
                string safe_file_name = Path.GetFileName(filename);
                object tempFolder = Utils.GetTempFolderName();
                if (tempFolder != null)
                {
                    string fullPath = Path.Combine(tempFolder.ToString(), safe_file_name);
                    if (System.IO.File.Exists(fullPath))
                    {
                        using (FileStream fs = System.IO.File.OpenRead(fullPath))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            Response.BinaryWrite(buffer);
                        }
                    }
                }
            }

            return this.Content("");
        }

        /// <summary>
        /// Renders the razor view to string.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>Return the Index action method</returns>
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Checks the email.
        /// </summary>
        public void CheckEmail()
        {
            CheckMail checkMail = new CheckMail();
            checkMail.AutoCheckMailForAccount();
        }

        /// <summary>
        /// Encodes the HTML.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>static</returns>
        public static string EncodeHtml(string email)
        {
            Regex rChar = new Regex("[\x0-\x8\xB-\xC\xE-\x1F]+");
            email = rChar.Replace(email, " ");

            StringBuilder sb = new StringBuilder(email);
            sb.Replace(">", "&gt;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            //sb.Replace("&", "&amp;");

            return sb.ToString();
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="xmlText">The XML text.</param>
        /// <returns>Return the Index action method</returns>
        public XmlNode GetResult(string xmlText)
        {
            CheckMail checkMail = new CheckMail();
            var xml = checkMail.GetMessages(xmlText);
            return xml;
        }
        #endregion

        #region Methods

        [HttpGet]
        public JsonResult GetJobsToAssign(string searchText)
        {
            IList<FormBot.Entity.JobList> lstJobs = _job.GetJobsToAssignMessage(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, searchText);
            return Json(lstJobs, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Windows service methods

        /// <summary>
        /// Fetches the reseller new mail.
        /// </summary>
        /// <returns>ActionResult</returns>
        [AllowAnonymous]
        public ActionResult FetchResellerNewMail()
        {
            //RECRegistryHelper.WriteToLogFile("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " FetchResellerNewMail called");
            //FormBot.Helper.Helper.Common.WriteErrorLog("Account/FetchResellerNewMail called at " + Convert.ToString(DateTime.Now.Date) + "");
            bool success = true;
            int intZipSize_MB = 5; // 5 MB
            string FromEmail = ProjectConfiguration.REC_PaperWork_FromEmail;
            string ToEmail = ProjectConfiguration.REC_PaperWork_ToEmail;
            string PVD_Subject_Text = ProjectConfiguration.REC_PaperWork_From_SubjectLine_PVD;
            string SWH_Subject_Text = ProjectConfiguration.REC_Site_audit_request_for_From_SubjectLine_SWH;
            string ResellerCompanyName = string.Empty;
            try
            {
                FormBot.Email.CheckMail checkMail = new FormBot.Email.CheckMail();
                DataSet resellersEmails = _reseller.GetResellersEmailAccount();
                if (resellersEmails != null && resellersEmails.Tables[0] != null && resellersEmails.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i <= resellersEmails.Tables[0].Rows.Count - 1; i++)
                    {
                        Account acct = null;
                        Session[FormBot.Email.Constants.sessionAccount] = null;
                        string xmlString = string.Empty;
                        FormBot.Entity.Email.EmailSignup emailModel = new FormBot.Entity.Email.EmailSignup();
                        DataRow dr = (DataRow)resellersEmails.Tables[0].Rows[i];

                        emailModel.Login = dr["email"].ToString();
                        emailModel.ConfigurationEmail = dr["email"].ToString();
                        emailModel.ConfigurationPassword = FormBot.Email.Utils.DecodePassword(Convert.ToString(dr["mail_inc_pass"]));
                        emailModel.IncomingMail = dr["mail_inc_host"].ToString();
                        emailModel.IncomingMailPort = Convert.ToInt32(dr["mail_inc_port"]);
                        emailModel.Login = dr["email"].ToString();
                        emailModel.OutgoingMail = dr["mail_out_host"].ToString();
                        emailModel.OutgoingMailPort = Convert.ToInt32(dr["mail_out_port"]);
                        ResellerCompanyName = Convert.ToString(dr["ResellerCompanyName"]);

                        xmlString = "<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[" + emailModel.ConfigurationEmail
                            + "]]></param><param name='mail_inc_login'><![CDATA[" + emailModel.Login + "]]></param><param name='mail_inc_pass'><![CDATA[" + emailModel.ConfigurationPassword + "]]></param><param name='mail_inc_host'><![CDATA[" + emailModel.IncomingMail
                            + "]]></param><param name='mail_inc_port' value='" + emailModel.IncomingMailPort + "' /><param name='mail_protocol' value='0' /><param name='mail_out_host'><![CDATA[" + emailModel.OutgoingMail
                            + "]]></param><param name='mail_out_port' value='" + emailModel.OutgoingMailPort + "' /><param name='mail_out_auth' value='1' /><param name='sign_me' value='0' /><param name='language' /><param name='advanced_login' value='1' /></webmail>";

                        var result = checkMail.GetMessages(xmlString);
                        acct = Session[FormBot.Email.Constants.sessionAccount] as Account;
                        if (acct != null)
                        {
                            //checkMail.AutoCheckMailForAccount();
                            #region My Logic

                            var lstEmailFolder = _emailService.GetFolders(acct.ID);
                            foreach (var item in lstEmailFolder)
                            {
                                if (item.id_folder != 0 && (!string.IsNullOrEmpty(item.name) && item.name.ToLower().Contains("inbox")))
                                {
                                    string typeId = Convert.ToString(item.type);
                                    long folderId = item.id_folder;
                                    string folderName = item.name;

                                    // Find last inserted message ID
                                    long maxMessageId = GetMaxMessageID_Reseller(typeId, Convert.ToString(folderId));
                                    // Find all new messages after maxMessageID mail
                                    List<EmailMessage> lstNewMail = CheckNewEmail_Reseller(maxMessageId, folderId, Convert.ToString(folderName));

                                    if (lstNewMail != null && lstNewMail.Count > 0)
                                    {
                                        foreach (var emailHeaders in lstNewMail)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(emailHeaders.from) && !string.IsNullOrEmpty(emailHeaders.subject) && emailHeaders.from.Contains(FromEmail) &&
                                                    (emailHeaders.subject.Contains(PVD_Subject_Text) || emailHeaders.subject.Contains(SWH_Subject_Text)))
                                                {
                                                    string PVDorSWH_Code = string.Empty;
                                                    bool IsPVDJob = false;
                                                    int index = emailHeaders.subject.LastIndexOf(' ');
                                                    PVDorSWH_Code = emailHeaders.subject.Substring(index).Trim();
                                                    string PVDorSWHDescription = string.Empty;
                                                    if (!string.IsNullOrEmpty(PVDorSWH_Code) && PVDorSWH_Code.Contains("PVD"))
                                                    {
                                                        PVDorSWHDescription = "compliance paperwork request";
                                                        IsPVDJob = true;
                                                    }
                                                    else
                                                    {
                                                        PVDorSWHDescription = "site audit request documentation";
                                                        IsPVDJob = false;
                                                    }

                                                    DataTable dt = _reseller.GetJobIDByPVDCode(PVDorSWH_Code);
                                                    if (dt != null && dt.Rows.Count > 0)
                                                    {
                                                        int jobID = Convert.ToInt32(dt.Rows[0]["JobID"]);
                                                        int STCDocID = Convert.ToInt32(dt.Rows[0]["STCDocID"]);
                                                        int CESDocID = Convert.ToInt32(dt.Rows[0]["CESDocID"]);
                                                        string RECCreationDate = Convert.ToDateTime(dt.Rows[0]["RECCreationDate"]).ToString("dd/MM/yyyy");

                                                        if (jobID > 0)
                                                        {
                                                            // Fetch Serial numbers pics by jobID
                                                            List<UserDocument> lstSerialDocument = new List<UserDocument>();
                                                            lstSerialDocument = _job.GetJobInstallationSerialByJobID(jobID);

                                                            // Get STCDocuments and CESDoc only for PVD Job not for SWH Job
                                                            if (IsPVDJob)
                                                            {
                                                                // Fetch STC Doc path
                                                                List<FormBot.Entity.Documents.DocumentsView> lstSTCDocuments = new List<Entity.Documents.DocumentsView>();
                                                                lstSTCDocuments = _documentsBAL.GetDocument(STCDocID);
                                                                string STCForm = GetDocumentFullPath(lstSTCDocuments, jobID);

                                                                // Fetch CES Doc path
                                                                List<FormBot.Entity.Documents.DocumentsView> lstCESDocuments = new List<Entity.Documents.DocumentsView>();
                                                                lstCESDocuments = _documentsBAL.GetDocument(CESDocID);
                                                                string CESDoc = GetDocumentFullPath(lstCESDocuments, jobID);

                                                                lstSerialDocument.Add(new UserDocument { DocumentPath = STCForm });
                                                                lstSerialDocument.Add(new UserDocument { DocumentPath = CESDoc });
                                                            }

                                                            #region Create Zip Logic

                                                            int m_packageSize = 1024 * 1024 * intZipSize_MB; // 5 MB Size
                                                            long totalZipFileSize = 0;
                                                            List<string> lstSavedFilePath = new List<string>();
                                                            System.IO.Directory.CreateDirectory(ProjectSession.ProofDocuments + "RECDocuments");
                                                            string ZipFolderPath = ProjectSession.ProofDocuments + "RECDocuments/";
                                                            string picSerialDocPath = ProjectSession.ProofDocuments + "JobDocuments/" + jobID + "/";

                                                            int counter = 1;
                                                            ZipFile zip = new ZipFile();
                                                            foreach (var picSerialDoc in lstSerialDocument)
                                                            {
                                                                string picSerialDocFullPath = string.Empty;

                                                                if (picSerialDoc.DocumentPath.Contains(@"\") || picSerialDoc.DocumentPath.Contains(@"/"))
                                                                    picSerialDocFullPath = picSerialDoc.DocumentPath;
                                                                else
                                                                    picSerialDocFullPath = picSerialDocPath + picSerialDoc.DocumentPath;

                                                                if (totalZipFileSize < m_packageSize)
                                                                {
                                                                    totalZipFileSize += new System.IO.FileInfo(picSerialDocFullPath).Length;
                                                                    if (totalZipFileSize <= m_packageSize)
                                                                        zip.AddFile(picSerialDocFullPath, "");
                                                                    else
                                                                    {
                                                                        string strsavedFilePath = ZipFolderPath + DateTime.Now.Ticks + "_" + jobID + ".zip";
                                                                        zip.Save(strsavedFilePath);
                                                                        zip.Dispose();

                                                                        lstSavedFilePath.Add(strsavedFilePath);
                                                                        totalZipFileSize = 0;

                                                                        // After create new zip file and then add file into 2nd zip
                                                                        if (counter != lstSerialDocument.Count)
                                                                        {
                                                                            zip = new ZipFile();
                                                                            try
                                                                            {
                                                                                zip.AddFile(picSerialDocFullPath, "");
                                                                                totalZipFileSize += new System.IO.FileInfo(picSerialDocFullPath).Length;
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                Helper.Log.WriteError(ex);
                                                                                // Zip already contains file Nothing to do                                                                       
                                                                            }
                                                                        }
                                                                    }

                                                                    // After create new zip file and then add file into 2nd zip (This case will run for last record only)
                                                                    if (counter == lstSerialDocument.Count)
                                                                    {
                                                                        try
                                                                        {
                                                                            zip.AddFile(picSerialDocFullPath, "");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Helper.Log.WriteError(ex);
                                                                            // Zip already contains file Nothing to do                                                                       
                                                                        }
                                                                        string strsavedFilePath = ZipFolderPath + DateTime.Now.Ticks + "_" + jobID + ".zip";
                                                                        zip.Save(strsavedFilePath);
                                                                        zip.Dispose();

                                                                        lstSavedFilePath.Add(strsavedFilePath);
                                                                        totalZipFileSize = 0;
                                                                    }
                                                                }
                                                                counter++;
                                                            }
                                                            #endregion

                                                            #region Send Mail functionality
                                                            // send mail with created zip as attachment
                                                            if (lstSavedFilePath != null && lstSavedFilePath.Count > 0)
                                                            {
                                                                int indexEmail = 1;
                                                                foreach (var strFilePath in lstSavedFilePath)
                                                                {
                                                                    string eventType = "send";
                                                                    long? selectedMessageid = -1;

                                                                    string tempfolderPath = SaveUploadFiles_Reseller(Path.GetFileName(strFilePath), strFilePath);

                                                                    if (!string.IsNullOrEmpty(tempfolderPath))
                                                                    {
                                                                        EmailTemplate eTemplate = _emailBAL.GetEmailTemplateByID(25);
                                                                        EmailInfo emailInfo = new EmailInfo();
                                                                        emailInfo.PVDorSWHDescription = PVDorSWHDescription;
                                                                        emailInfo.PVDCode = PVDorSWH_Code;
                                                                        emailInfo.RECCreationDate = RECCreationDate;
                                                                        emailInfo.IndexOfEmail = Convert.ToString(indexEmail);
                                                                        emailInfo.TotalEmailCount = Convert.ToString(lstSavedFilePath.Count());
                                                                        emailInfo.RACompanyName = ResellerCompanyName;

                                                                        eTemplate.Content = _emailBAL.GetEmailBody(emailInfo, eTemplate);

                                                                        ////EmailTemplate eTemplate = _emailBAL.PrepareEmailTemplate(25, null, null, null, null, PVDorSWH_Code, RECCreationDate, indexEmail, lstSavedFilePath.Count(), ResellerCompanyName, PVDorSWHDescription);

                                                                        //string SubjectLine = "(1 of 5)Site audit request for PVD2492144 creation: 19/07/2016 ";
                                                                        ComposeEmail objComposeEmail = new ComposeEmail();
                                                                        objComposeEmail.Attachments = new List<AttachmentData>();
                                                                        objComposeEmail.Attachments.Add(new AttachmentData { FileName = Path.GetFileName(strFilePath), GeneratedName = Path.GetFileName(tempfolderPath) });
                                                                        objComposeEmail.Subject = "(" + indexEmail + " of " + lstSavedFilePath.Count() + ")Site audit request for " + PVDorSWH_Code + " creation:" + RECCreationDate;
                                                                        objComposeEmail.To = ToEmail;
                                                                        objComposeEmail.Body = new innerBody { body = eTemplate.Content };

                                                                        SendMail_Reseller(objComposeEmail, eventType, selectedMessageid);
                                                                    }
                                                                    indexEmail++;
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                // Any error occurs then skip that email and start process for new email
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            //RECRegistryHelper.WriteToLogFile("Account configuration for following email " + emailModel.ConfigurationEmail + " is invalid.");
                            _log.LogException("Account configuration for following email " + emailModel.ConfigurationEmail + " is invalid.", null);
                            //FormBot.Helper.Helper.Common.WriteErrorLog("Account configuration for following email " + emailModel.ConfigurationEmail + " is invalid.");
                        }
                    }

                    Session[FormBot.Email.Constants.sessionAccount] = null;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Helper.Log.WriteError(ex,"FetchResellerNewMail");
            }
            return null;
            //return success;
        }

        /// <summary>
        /// Get Document Full path
        /// </summary>
        /// <param name="lstJobDocuments"></param>
        /// <param name="jobID"></param>
        /// <returns>string</returns>
        public string GetDocumentFullPath(List<FormBot.Entity.Documents.DocumentsView> lstJobDocuments, int jobID)
        {
            string name = (lstJobDocuments.Count > 0 ? lstJobDocuments[0].Name : "");
            if (name.ToLower() == "ces")
                name = "ces" + (FormBot.Helper.SystemEnums.JobType.PVD.ToString().ToLower() == (lstJobDocuments.Count > 0 ? lstJobDocuments[0].ServiceProviderName.ToLower() : "") ? "pvd.pdf" : "sw.pdf");

            string documentPath = jobID + "/" + (lstJobDocuments.Count > 0 ? lstJobDocuments[0].Stage : Helper.SystemEnums.JobStage.PreApprovals.ToString()) + "/" + name;
            string documentFullPath = ProjectConfiguration.JobDocumentsToSaveFullPath + documentPath;
            return documentFullPath;
        }

        /// <summary>
        /// Get MaxMessage ID
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="folderId"></param>
        /// <returns>long</returns>
        public long GetMaxMessageID_Reseller(string typeId, string folderId)
        {
            Account acct = Session[Constants.sessionAccount] as Account;
            string xmlText = string.Empty;
            if (typeId == "1")
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='base'/></webmail>";
            }
            else if (typeId == "10")
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages'/><param name='id_acct' value='" + acct.ID
                    + "'/><param name='page' value='1'/><param name='sort_field' value='0'/><param name='sort_order' value='0'/><folder id='" + folderId
                    + "'></folder><look_for fields='0'><![CDATA[]]></look_for></webmail>";
            }
            else
            {
                xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='folders_base'/><param name='background' value='2'/></webmail>";
            }

            string resultXml = GetResult(xmlText).OuterXml;
            XDocument resultDocument = XDocument.Parse(resultXml);

            var lstMessages = resultDocument.Descendants("webmail").Elements("messages").Elements("message").Where(x => x.Element("folder").Attribute("id").Value == folderId).ToList();

            List<EmailMessage> lstWebMail = new List<EmailMessage>();

            CheckMail checkMail = new CheckMail();
            foreach (var item in lstMessages)
            {
                EmailMessage emailMessage = new EmailMessage()
                {
                    id = Convert.ToInt64(item.Attribute("id").Value),
                    from = checkMail.SplitNameAndEmail(item.Element("from").Value).Replace("&gt;", "").Replace("&lt;", ""),
                    to = item.Element("to").Value,
                    subject = item.Element("subject").Value,
                    full_date = item.Element("full_date").Value,
                    short_date = item.Element("short_date").Value,
                    has_attachments = item.Attribute("has_attachments").Value == "1",
                    flags = item.Attribute("flags").Value == "0" ? false : true
                };

                //emailMessage.MessageClass = (emailMessage.flags == true) ? "opened" : "unread";
                lstWebMail.Add(emailMessage);
            }

            long maxMessageId = 0;
            if (lstWebMail.Count() > 0)
            {
                maxMessageId = lstWebMail.Max(w => w.id);
            }
            return maxMessageId;
        }

        /// <summary>
        /// CheckNewEmail
        /// </summary>
        /// <param name="maxMessageId"></param>
        /// <param name="folderId"></param>
        /// <param name="folderName"></param>
        /// <returns>list</returns>
        public List<EmailMessage> CheckNewEmail_Reseller(long maxMessageId, long folderId, string folderName)
        {
            CheckEmail();
            Account acct = Session[Constants.sessionAccount] as Account;
            List<EmailMessage> lstWebMail = new List<EmailMessage>();
            IList<EmailMessage> resultMessages = _emailService.GetAllMessages(maxMessageId, acct.ID);
            CheckMail checkMail = new CheckMail();
            foreach (var item in resultMessages.ToList())
            {
                Dictionary<string, string> shortAndFullDate = checkMail.GetShortAndFullDate(acct, item.msg_date);
                item.from = checkMail.SplitNameAndEmail(item.from);
                item.to = checkMail.SplitNameAndEmail(item.to);
                item.short_date = shortAndFullDate["ShortDate"];
                item.full_date = shortAndFullDate["FullDate"];
                item.MessageClass = (item.flags == true) ? "opened" : "unread";
                item.has_attachments = item.has_attachments;
                if (item.has_attachments)
                {
                    string xmlBody = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='get'/><param name='request' value='messages_bodies'/><folder id='" + folderId
                        + "'><full_name><![CDATA[" + folderName + "]]></full_name><message id='" + item.id + "' charset='-1'></message></folder></webmail>";

                    xmlBody = GetResult(xmlBody).OuterXml;
                }

                item.flags = item.flags;
                lstWebMail.Add(item);
            }
            return lstWebMail;
        }

       /// <summary>
        /// Save Upload Files
       /// </summary>
        /// <param name="file_name">file_name</param>
        /// <param name="fileFullPath">fileFullPath</param>
        /// <returns>string</returns>
        public string SaveUploadFiles_Reseller(string file_name, string fileFullPath)
        {
            string filename = string.Empty;
            name = Utils.EncodeHtml(Path.GetFileName(fileFullPath)).Replace(@"'", @"\'");
            //size = file.ContentLength;
            mime_type = "application/x-zip-compressed";
            try
            {
                byte[] buffer = ReadFile(fileFullPath);
                if (buffer != null)
                {
                    string tempFolder = Utils.GetTempFolderName();
                    filename = Utils.CreateTempFilePath(tempFolder, file_name);
                    tmp_name = Path.GetFileName(filename);
                    using (FileStream fs = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
                return filename;
            }
            catch (IOException ex)
            {
                Helper.Log.WriteError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// ReadFile
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>byte array</returns>
        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        /// <summary>
        /// Send Mail and returns true or false based on mail send successfully or not
        /// </summary>
        /// <param name="composeMail"></param>
        /// <param name="eventType"></param>
        /// <param name="selectedMessageid"></param>
        /// <returns>bool value</returns>
        public bool SendMail_Reseller(ComposeEmail composeMail, string eventType, long? selectedMessageid)
        {
            bool isSuccess = true;
            var errorMessage = string.Empty;
            CheckMail checkMail = new CheckMail();
            var result = checkMail.SendMail(composeMail, eventType, selectedMessageid);

            XDocument xDocumentEmail = XDocument.Parse(result.OuterXml);
            var error = xDocumentEmail.Descendants("webmail").Elements("error").FirstOrDefault();
            if (error != null && !string.IsNullOrEmpty(Convert.ToString(error.Value)))
            {
                isSuccess = false;
                errorMessage = error.Value;
            }
            return isSuccess;
        }

        #endregion
    }
}
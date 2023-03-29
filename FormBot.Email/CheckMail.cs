using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using System.Web.Util;
using System.Xml;
using System.Configuration;
using MailBee.Mime;
using System.Text.RegularExpressions;

namespace FormBot.Email
{
    public class CheckMail
    {
        protected int Type = 0;
        protected string folderName = string.Empty;
        protected string errorDesc = string.Empty;
        protected int msgsCount = 3;
        protected int msgNumber = 0;
        protected string accountName = string.Empty;

        #region EmailRegion

        public void ValidateUser()
        {
            //string xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[bhupesh.rajput@tatvasoft.com]]></param><param name='mail_inc_login'><![CDATA[]]></param><param name='mail_inc_pass'><![CDATA[BhupeshR768]]></param><param name='mail_inc_host'><![CDATA[localhost]]></param><param name='mail_inc_port' value='110'/><param name='mail_protocol' value='0'/><param name='mail_out_host'><![CDATA[localhost]]></param><param name='mail_out_port' value='25'/><param name='mail_out_auth' value='1'/><param name='sign_me' value='0'/><param name='language'><![CDATA[]]></param><param name='advanced_login' value='0'/></webmail>";
            string xmlText = "<?xml version='1.0' encoding='utf-8'?><webmail><param name='action' value='login' /><param name='request' value='' /><param name='email'><![CDATA[pankaj.baman@tatvasoft.com]]></param><param name='mail_inc_login'><![CDATA[]]></param><param name='mail_inc_pass'><![CDATA[PankajB016]]></param><param name='mail_inc_host'><![CDATA[localhost]]></param><param name='mail_inc_port' value='110'/><param name='mail_protocol' value='0'/><param name='mail_out_host'><![CDATA[localhost]]></param><param name='mail_out_port' value='25'/><param name='mail_out_auth' value='1'/><param name='sign_me' value='0'/><param name='language'><![CDATA[]]></param><param name='advanced_login' value='0'/></webmail>";
            if (xmlText != null)
            {
                Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;

                XmlPacketManager manager = new XmlPacketManager(acct);
                XmlPacket packet = manager.ParseClientXmlText(xmlText);
                XmlDocument doc = manager.CreateServerXmlDocumentResponse(packet);

                if (HttpContext.Current.Session[Constants.sessionAccount] == null)
                {
                    if (manager.CurrentAccount != null)
                    {
                        HttpContext.Current.Session.Add(Constants.sessionAccount, manager.CurrentAccount);
                        int idUser = manager.CurrentAccount.IDUser;
                        if (HttpContext.Current.Session[Constants.sessionUserID] == null)
                        {
                            HttpContext.Current.Session.Add(Constants.sessionUserID, idUser);
                        }
                        else
                        {
                            HttpContext.Current.Session[Constants.sessionUserID] = idUser;
                        }
                    }
                }
            }
        }

        public XmlNode GetMessages(string xmlText)
        {

            if (xmlText != null)
            {
                Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;

                XmlPacketManager manager = new XmlPacketManager(acct);
                XmlPacket packet = manager.ParseClientXmlText(xmlText);
                XmlDocument doc = manager.CreateServerXmlDocumentResponse(packet);

                if (HttpContext.Current.Session[Constants.sessionAccount] == null)
                {
                    if (manager.CurrentAccount != null)
                    {
                        HttpContext.Current.Session.Add(Constants.sessionAccount, manager.CurrentAccount);
                        int idUser = manager.CurrentAccount.IDUser;
                        if (HttpContext.Current.Session[Constants.sessionUserID] == null)
                        {
                            HttpContext.Current.Session.Add(Constants.sessionUserID, idUser);
                        }
                        else
                        {
                            HttpContext.Current.Session[Constants.sessionUserID] = idUser;
                        }
                    }
                }
                else
                {
                    if (manager.CurrentAccount != null)
                    {
                        if (!manager.CurrentAccount.Equals(HttpContext.Current.Session[Constants.sessionAccount]))
                        {
                            HttpContext.Current.Session[Constants.sessionAccount] = manager.CurrentAccount;
                            if (HttpContext.Current.Session[Constants.sessionUserID] == null)
                            {
                                HttpContext.Current.Session.Add(Constants.sessionUserID, manager.CurrentAccount.IDUser);
                            }
                            else
                            {
                                HttpContext.Current.Session[Constants.sessionUserID] = manager.CurrentAccount.IDUser;
                            }
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session[Constants.sessionAccount] = null;
                    }
                }

                return doc;
            }

            return new XmlDocument();
        }

        public void AutoCheckMailForAccount()
        {
            Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;
            if (acct != null)
            {
                try
                {
                    DbStorage dbs = DbStorageCreator.CreateDatabaseStorage(acct);
                    MailProcessor mp = new MailProcessor(dbs);
                    WebmailResourceManager _resMan = (new WebmailResourceManagerCreator()).CreateResourceManager();
                    try
                    {
                        mp.MessageDownloaded += new DownloadedMessageHandler(mp_MessageDownloaded);
                        mp.Connect();

                        FolderCollection fc1 = dbs.GetFolders();
                        FolderCollection fc2 = new FolderCollection();
                        foreach (Folder fld in fc1)
                        {
                            if (fld.Type == FolderType.Inbox)
                            {
                                fc2.Add(fld);
                            }
                        }
                        Dictionary<long, string> updatedFolders = mp.Synchronize(fc2);
                        string strFolders = "";
                        foreach (KeyValuePair<long, string> kvp in updatedFolders)
                        {
                            strFolders += "{id: " + kvp.Key.ToString() + ", fullName: '" + kvp.Value + "'}, ";
                        }
                        //Response.Write(@"<script type=""text/javascript"">parent.SetUpdatedFolders([" + strFolders.TrimEnd(new char[2] { ',', ' ' }) + "], false);</script>");
                    }
                    finally
                    {
                        mp.MessageDownloaded -= new DownloadedMessageHandler(mp_MessageDownloaded);
                        mp.Disconnect();
                    }
                }
                catch (WebMailException ex)
                {
                    Log.WriteException(ex);
                    errorDesc = Utils.EncodeJsSaveString(ex.Message);
                    if (Type == 1 || Type == 2)
                    {
                        //Session.Add(Constants.sessionErrorText, errorDesc);
                    }
                }
            }
        }

        private void mp_MessageDownloaded(object sender, CheckMailEventArgs e)
        {
            folderName = e.FolderName;
            msgNumber = e.MsgsNumber;
            msgsCount = e.MsgsCount;
            //            Response.Write(@"<script type=""text/javascript"">
            //				parent.SetCheckingFolderHandler('" + folderName + "', " + msgsCount + ");</script>");
            //            Response.Flush();
            //            Response.Write(@"<script type=""text/javascript"">
            //				parent.SetRetrievingMessageHandler(" + msgNumber + ");</script>");
            //Response.Flush();
            //Log.WriteLine("mp_MessageDownloaded", string.Format("Folder: '{0}'; MessageCount: {1}; MessageNumber: {2}", folderName, msgsCount, msgNumber));
        }

        public Dictionary<string, string> GetShortAndFullDate(Account acct, DateTime dt)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DateFormatting df = new DateFormatting(acct, dt);
            result.Add("FullDate", df.FullDate);
            result.Add("ShortDate", df.ShortDate);
            return result;
        }


        public string SplitNameAndEmail(string html, bool isName = true)
        {
            Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;
            WebMailMessage webMailMessage = new WebMailMessage(acct);
            webMailMessage.FromMsg = new MailBee.Mime.EmailAddress() { AsString = html };
            if (isName)
            {
                //return webMailMessage.FromMsg.DisplayName;    
            }

            return webMailMessage.FromMsg.Email;
        }

        public XmlNode SendMail(ComposeEmail composeMail, string eventType, long? messageId = -1)
        {
            Account acct = HttpContext.Current.Session[Constants.sessionAccount] as Account;
            if (acct != null)
            {
                var xmlText = @"<?xml version='1.0' encoding='UTF-8'?><webmail><param name='action' value='" + eventType + "' /><param name='request' value='message' /><message id='" + messageId + "' from_acct_id='" + acct.ID
                        + "' sensivity='0' size='0' priority='3' save_mail='" + 1 + "'><uid /><headers><from><![CDATA[" + acct.Email + "]]></from><to><![CDATA[" + composeMail.To + "]]></to><cc><![CDATA[" + composeMail.Cc + "]]></cc><bcc><![CDATA[" +
                        composeMail.Bcc + "]]></bcc><subject><![CDATA[" + composeMail.Subject + "]]></subject><groups /></headers><body is_html='1'><![CDATA[" + composeMail.Body.body + "]]></body><attachments>";
                string attachment = string.Empty;
                if (composeMail.Attachments != null && composeMail.Attachments.Count() > 0)
                {
                    foreach (var item in composeMail.Attachments)
                    {
                        attachment += "<attachment  inline='0'><temp_name><![CDATA[" + item.GeneratedName + "]]></temp_name><name><![CDATA[" + item.FileName + "]]></name><mime_type><![CDATA[application/octet-stream]]></mime_type></attachment>";
                    }
                }

                xmlText += attachment + "</attachments></message></webmail>";
                return GetMessages(xmlText);
            }
            return new XmlDocument();
            //return GetMessages(string.Empty);
        }

        #endregion

        #region EmailThreadRegion

        public MailMessage LoadMessagesByEmailThreadID(int id_msg, string folderName, string email, int id_acct)
        {
            MailMessage msg = null;
            string path = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString();
            string fullPathWithFolderName = Path.Combine(path, "EmailManagement", "Mail", Convert.ToString(email[0]), email + "." + id_acct, folderName, Convert.ToString(id_msg) + ".eml");
            if (File.Exists(fullPathWithFolderName))
            {
                msg = new MailMessage();
                msg.LoadMessage(fullPathWithFolderName);
                msg = PrepareMessage(null, msg, fullPathWithFolderName, 0, true, true, false, email, id_acct);
            }

            return msg;
        }

        private MailMessage PrepareMessage(WebMailMessage msg, MailMessage outputMsg, string folderFullPath, byte safety, bool needToTrim, bool needToShowTrimMessage, bool body_structure, string email, int id_acct)
        ////private WebMailMessage PrepareMessage(WebMailMessage msg, MailMessage outputMsg, Folder fld, byte safety, bool needToTrim, bool needToShowTrimMessage, bool body_structure,string email,int id_acct)
        {
            //if (_acct.UserOfAccount.GetSenderSafety(outputMsg.From.Email) == 1) safety = 1;
            safety = 0;
            if (outputMsg.Attachments.Count > 0)
            {
                //string tempFolder = Utils.GetTempFolderName();
                string tempFolder = Utils.GetTempFolderNameForEmailThread(email, id_acct);

                outputMsg.Parser.WorkingFolder = tempFolder;
                string pathForDownload = System.Configuration.ConfigurationManager.AppSettings["ProofUploadFolder"];
                string projectImagePath = System.Configuration.ConfigurationManager.AppSettings["ProjectImagePath"] + "Email/Email/ReadImageFile?filename=";
                outputMsg.BodyHtmlText = outputMsg.GetHtmlAndSaveRelatedFiles(@"" + projectImagePath, VirtualMappingType.Static, MessageFolderBehavior.DoNotCreate);

                AttachmentCollection SavedAttachments = outputMsg.Attachments/*Utils.CreateDeepCopy(outputMsg.Attachments)*/;
                SaveAttachmentsToTempDirectory(SavedAttachments, tempFolder, outputMsg.Attachments);
            }

            // removing all potentially unsafe content from the HTML e-mail body
            if (!string.IsNullOrEmpty(outputMsg.BodyHtmlText))
            {
                string htmlBody = outputMsg.BodyHtmlText;
                if (needToTrim && (htmlBody.Length > Constants.BodyMaxLength))
                {
                    htmlBody = htmlBody.Substring(0, Constants.BodyMaxLength);
                    if (needToShowTrimMessage)
                    {
                        string strUid = (string.IsNullOrEmpty(msg.StrUid)) ? msg.IntUid.ToString() : msg.StrUid;
                        htmlBody += Utils.GetTrimMessage(msg.IDMsg, strUid, msg.IDFolderDB, folderFullPath, msg.OverrideCharset, 1); ;
                    }
                }

                // Ex-MailBee.Html
                string[] removeTags = new string[]
					{
						"<!doctype[^>]*>",
						"</?html[^>]*>",
						"</?body[^>]*>",
						"<link[^>]*>",
						"<base[^>]*>",
						"<head[^>]*>.*?</head>",
						"<title[^>]*>.*?</title>",
						"<style[^>]*>.*?</style>",
						"<script[^>]*>.*?</script>",
						"<object[^>]*>.*?</object>",
						"<embed[^>]*>.*?</embed>",
						"<applet[^>]*>.*?</applet>",
						"<mocha[^>]*>.*?</mocha>",
						"<meta[^>]*>"
					};

                foreach (string regEx in removeTags)
                {
                    htmlBody = Regex.Replace(htmlBody, regEx, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }

                string[] removeAttributes = new string[] { "onActivate", "onAfterPrint",
									"onBeforePrint", "onAfterUpdate", "onBeforeUpdate", "onErrorUpdate",
									"onAbort", "onBeforeDeactivate", "onDeactivate", "onBeforeCopy",
									"onBeforeCut", "onBeforeEditFocus", "onBeforePaste", "onBeforeUnload",
									"onBlur", "onBounce", "onChange", "onClick", "onControlSelect",
									"onCopy", "onCut", "onDblClick", "onDrag", "onDragEnter", "onDragLeave",
									"onDragOver", "onDragStart", "onDrop", "onFilterChange", "onDragDrop",
									"onError", "onFilterChange", "onFinish", "onFocus", "onHelp", "onKeyDown",
									"onKeyPress", "onKeyUp", "onLoad", "onLoseCapture", "onMouseDown",
									"onMouseEnter", "onMouseLeave", "onMouseMove", "onMouseOut",
									"onMouseOver", "onMouseUp", "onMove", "onPaste",
									"onPropertyChange", "onReadyStateChange", "onReset", "onResize",
									"onResizeEnd", "onResizeStart", "onScroll", "onSelectStart",
									"onSelect", "onSelectionChange", "onStart", "onStop", "onSubmit",
									"onUnload"};

                foreach (string regEx in removeAttributes)
                {
                    htmlBody = Regex.Replace(htmlBody, "(<[^>]*)(" + regEx + ")([^>]*>)", "${1}X_${2}${3}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }

                if (safety == 0)
                {
                    safety = 1; // sender not safety, but HTML may be safety

                    //BGImagesReplacement
                    htmlBody = Regex.Replace(htmlBody, "(<[^>]+style[ \t\r\n]*=[ \t\r\n]*['\"]?)url\\(", "${1}wmx_url(");
                    htmlBody = Regex.Replace(htmlBody, "(<[^>]+)background", "${1}wmx_background");

                    //img
                    if (Regex.IsMatch(htmlBody, "(<img[^>]+)src"))
                    {
                        htmlBody = Regex.Replace(htmlBody, "(<img[^>]+)src", "${1}wmx_src");
                        htmlBody = Regex.Replace(htmlBody, "(<img[^>]+)wmx_src=\"get-attachment-binary", "${1}src=\"get-attachment-binary");
                        if (htmlBody.IndexOf("wmx_src") != -1)
                        {
                            safety = 0;
                        }
                    }

                }
                ////msg.Safety = safety;

                MatchEvaluator aHrefEvaluator = new MatchEvaluator(AHrefReplaceDelegate);
                htmlBody = Regex.Replace(htmlBody, "<a[^>]+", aHrefEvaluator, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // processing of the rule
                htmlBody = Regex.Replace(htmlBody, @"(document|window).location[^=]*=\s*[""'][^""']*[""']",
                                                string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                outputMsg.BodyHtmlText = htmlBody;
            }
            else
            {
                if (msg != null)
                    msg.Safety = 1;
            }

            if (string.IsNullOrEmpty(outputMsg.BodyPlainText))
            {
                outputMsg.MakePlainBodyFromHtmlBody();
            }

            // for mailto:
            outputMsg.BodyHtmlText = outputMsg.BodyHtmlText.Replace("<a ", "<a onclick=\"return checkLinkHref(this.href);\" ");
            outputMsg.BodyPlainText = outputMsg.BodyPlainText.Replace("<a ", "<a onclick=\"return checkLinkHref(this.href);\" ");

            ////msg.MailBeeMessage = outputMsg;
            return outputMsg;
        }

        private void SaveAttachmentsToTempDirectory(AttachmentCollection Attachments, string tempFolder, AttachmentCollection refAttachments)
        {
            for (int i = 0; i < Attachments.Count; i++)
            {

                //string filename = CreateTempFilePath(tempFolder,
                //    (Attachments[i].Filename.Length > 0) ? Attachments[i].Filename : Attachments[i].Name);

                string filename = (Attachments[i].Filename.Length > 0) ? Attachments[i].Filename : Attachments[i].Name;

                string tempFilePath = string.Format("{0}{1}", Utils.GetMD5DigestHexString(filename), Path.GetExtension(filename));
                filename = Path.Combine(tempFolder, tempFilePath);

                //if (!File.Exists(filename))
                //{
                Attachments[i].Save(filename, false);
                //}
                //else
                //{
                //    Attachments[i]. = filename;
                //}

                if (Attachments[i].IsTnef)
                {
                    AttachmentCollection atCol = Attachments[i].GetAttachmentsFromTnef();
                    foreach (Attachment attach in atCol)
                    {
                        refAttachments.Add(attach);
                    }
                    SaveAttachmentsToTempDirectory(atCol, tempFolder, refAttachments);
                }
            }
        }

        ////public string CreateTempFilePath(string tempFolderName, string filename)
        ////{
        ////    string tempFilePath = string.Format("{0}{1}", Utils.GetMD5DigestHexString(filename), Path.GetExtension(filename));
        ////    tempFilePath = Path.Combine(tempFolderName, tempFilePath);
        ////    if (File.Exists(tempFilePath))
        ////    {
        ////        int i = 1;
        ////        while (File.Exists(tempFilePath))
        ////        {
        ////            tempFilePath = string.Format("{0}_{2}{1}", Utils.GetMD5DigestHexString(filename), Path.GetExtension(filename), i);
        ////            tempFilePath = Path.Combine(tempFolderName, tempFilePath);
        ////            i++;
        ////        }
        ////    }
        ////    if (tempFilePath.Length > Constants.PathMaxLength)
        ////    {
        ////        System.Diagnostics.Debug.Assert(false);
        ////    }
        ////    return tempFilePath;
        ////}

        private string AHrefReplaceDelegate(Match m)
        {
            string result = m.ToString();

            Regex re = new Regex("<a([^>]+href[ \t\r\n]*=[ \t\r\n]*['\"]?[#]{1})");

            Match mr = re.Match(result);

            if (!mr.Success)
            {
                result = "<a target=\"_blank\"" + result.Substring(2);
            }

            return result;
        }

        public string DateFormattingGetDateWithoughtTimeZone(DateTime date)
        {
            WebmailSettings settings = (new WebMailSettingsCreator()).CreateWebMailSettings();
            var _time = date.ToString("hh:mm tt", CultureInfo.InvariantCulture);

            return GetWeekdayManually(date) + ", " + GetMonthManually(date) +
                date.ToString(" dd, yyyy", CultureInfo.InvariantCulture) + ", " + _time;
        }

        private string GetWeekdayManually(DateTime dt)
        {
            WebmailResourceManager resMan = (new WebmailResourceManagerCreator()).CreateResourceManager();
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return resMan.GetString("DayToolMonday");
                case DayOfWeek.Tuesday:
                    return resMan.GetString("DayToolTuesday");
                case DayOfWeek.Wednesday:
                    return resMan.GetString("DayToolWednesday");
                case DayOfWeek.Thursday:
                    return resMan.GetString("DayToolThursday");
                case DayOfWeek.Friday:
                    return resMan.GetString("DayToolFriday");
                case DayOfWeek.Saturday:
                    return resMan.GetString("DayToolSaturday");
                default:
                    return resMan.GetString("DayToolSunday");
            }
        }

        private string GetMonthManually(DateTime dt)
        {
            WebmailResourceManager resMan = (new WebmailResourceManagerCreator()).CreateResourceManager();
            switch (dt.Month)
            {
                case 2:
                    return resMan.GetString("ShortMonthFebruary");
                case 3:
                    return resMan.GetString("ShortMonthMarch");
                case 4:
                    return resMan.GetString("ShortMonthApril");
                case 5:
                    return resMan.GetString("ShortMonthMay");
                case 6:
                    return resMan.GetString("ShortMonthJune");
                case 7:
                    return resMan.GetString("ShortMonthJuly");
                case 8:
                    return resMan.GetString("ShortMonthAugust");
                case 9:
                    return resMan.GetString("ShortMonthSeptember");
                case 10:
                    return resMan.GetString("ShortMonthOctober");
                case 11:
                    return resMan.GetString("ShortMonthNovember");
                case 12:
                    return resMan.GetString("ShortMonthDecember");
                default:
                    return resMan.GetString("ShortMonthJanuary");
            }
        }
        #endregion
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace FormBot.Helper
{
    public static class MailHelper
    {
        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="smtpDetail">The SMTP detail.</param>
        /// <param name="MailTo">The mail to.</param>
        /// <param name="MailCC">The mail cc.</param>
        /// <param name="MailBCC">The mail BCC.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="Body">The body.</param>
        /// <param name="Attachment">The attachment.</param>
        /// <param name="IsBodyHtml">if set to <c>true</c> [is body HTML].</param>
        /// <param name="FailureReason">The failure reason.</param>
        /// <param name="isAttachmentContent">if set to <c>true</c> [is attachment content].</param>
        /// <param name="newFileName">New name of the file.</param>
        /// <returns>bool</returns>
        public static bool SendMail(SMTPDetails smtpDetail, string MailTo, string MailCC, string MailBCC, string subject, string Body, string Attachment, bool IsBodyHtml, ref string FailureReason, bool isAttachmentContent = false, string newFileName = "")
        {
            MailReponse obj = new MailReponse();
            try
            {
                SmtpClient mailClient = new SmtpClient();

                char[] deliminadores = { ';' };
                string[] Direcciones;
                bool SendStatus = false;
                FailureReason = string.Empty;

                MailMessage MailMesg = new MailMessage();

                MailMesg.From = new MailAddress(smtpDetail.MailFrom);

                if (!string.IsNullOrEmpty(MailTo))
                {
                    Direcciones = MailTo.Split(deliminadores);
                    foreach (string d in Direcciones)
                    {
                        MailMesg.To.Add(new MailAddress(d));
                    }
                }

                if (!string.IsNullOrEmpty(MailCC))
                {
                    Direcciones = MailCC.Split(deliminadores);
                    foreach (string d in Direcciones)
                    {
                        MailMesg.CC.Add(new MailAddress(d));
                    }
                }

                if (!string.IsNullOrEmpty(MailBCC))
                {
                    Direcciones = MailBCC.Split(deliminadores);
                    foreach (string d in Direcciones)
                    {
                        MailMesg.Bcc.Add(new MailAddress(d));
                    }
                }

                if (!string.IsNullOrEmpty(Attachment))
                {
                    System.Net.Mail.Attachment attach = null;
                    if (isAttachmentContent)
                    {
                        attach = System.Net.Mail.Attachment.CreateAttachmentFromString("" + Attachment + "", "Faults.xls");
                    }
                    else
                    {
                        attach = new System.Net.Mail.Attachment(Attachment);
                    }

                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        attach.Name = newFileName;
                    }

                    MailMesg.Attachments.Add(attach);
                    attach = null;
                }
                MailMesg.Subject = subject;
                MailMesg.Body = Body;
                MailMesg.IsBodyHtml = IsBodyHtml;                
                System.Net.Mail.SmtpClient objSMTP = new System.Net.Mail.SmtpClient();
                objSMTP.Host = smtpDetail.SMTPHost;
                objSMTP.Port = smtpDetail.SMTPPort;
                objSMTP.EnableSsl = smtpDetail.IsSMTPEnableSsl;
                
                if (!string.IsNullOrEmpty(smtpDetail.SMTPUserName) && !string.IsNullOrEmpty(smtpDetail.SMTPPassword))
                {
                    NetworkCredential basicAuthenticationInfo = new NetworkCredential(smtpDetail.SMTPUserName, smtpDetail.SMTPPassword);
                    objSMTP.UseDefaultCredentials = false;
                    objSMTP.Credentials = basicAuthenticationInfo;
                }
                else
                {
                    objSMTP.UseDefaultCredentials = true;
                }

                try
                {
                    objSMTP.Send(MailMesg);
                    SendStatus = true;
                    objSMTP = null;
                    MailMesg.Dispose();
                    MailMesg = null;
                }
                catch (Exception ex)
                {
                    MailMesg.Dispose();
                    MailMesg = null;
                    FailureReason = ex.GetBaseException().Message.ToString();
                    //return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class MailReponse
    {
        public bool IsSent { get; set; }
        public string FailureReason { get; set; }
    }

    public class SMTPDetails
    {
        /// <summary>
        /// Gets or sets the mail from.
        /// </summary>
        /// <value>
        /// The mail from.
        /// </value>
        public string MailFrom { get; set; }
        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        /// <value>
        /// The SMTP host.
        /// </value>
        public string SMTPHost { get; set; }
        /// <summary>
        /// Gets or sets the name of the SMTP user.
        /// </summary>
        /// <value>
        /// The name of the SMTP user.
        /// </value>
        public string SMTPUserName { get; set; }
        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        /// <value>
        /// The SMTP password.
        /// </value>
        public string SMTPPassword { get; set; }
        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        /// <value>
        /// The SMTP port.
        /// </value>
        public int SMTPPort { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is SMTP enable SSL.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is SMTP enable SSL; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsSMTPEnableSsl { get; set; }

    }
}

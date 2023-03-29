using Azure.Identity;
using FormBot.Entity.Email;
using FormBot.Helper.Helper;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PriorityWindowService
{
    public class EmailGraph
    {
        public bool SendEMail(QueuedEmail objQueuedEmail)
        {
            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var tenantId = Convert.ToString(ConfigurationManager.AppSettings["TenetId"]);
                var clientId = Convert.ToString(ConfigurationManager.AppSettings["ClientId"]);
                var clientSecret = Convert.ToString(ConfigurationManager.AppSettings["ClientSecret"]);
                var SenderMailId = Convert.ToString(ConfigurationManager.AppSettings["SenderMailId"]);

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };
                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
                Message msg = new Message();
                if (!string.IsNullOrEmpty(objQueuedEmail.FromEmail))
                {
                    Recipient from = new Recipient();
                    EmailAddress fromEmail = new EmailAddress();
                    fromEmail.Address = objQueuedEmail.FromEmail;
                    fromEmail.Name = objQueuedEmail.FromEmail;
                    from.EmailAddress = fromEmail;
                    msg.From = from;
                }
                List<Recipient> ToRecipients = new List<Recipient>();
                foreach (var ToEmail in objQueuedEmail.ToEmail.Split(';'))
                {
                    Recipient recipient = new Recipient();
                    EmailAddress emailAddress = new EmailAddress();
                    emailAddress.Address = ToEmail.Trim();
                    recipient.EmailAddress = emailAddress;
                    ToRecipients.Add(recipient);
                }
                msg.ToRecipients = ToRecipients;


                MessageAttachmentsCollectionPage attachments = new MessageAttachmentsCollectionPage();
                //string linkBody = string.Empty;
                //string fileName = string.Empty;
                //if (objQueuedEmail.IsAttachment ?? false)
                //{
                //    byte[] fileStream = ReadFully(GetFiles(objQueuedEmail.Id, out fileName));
                //    attachments.Add(new FileAttachment
                //    {
                //        ODataType = "#microsoft.graph.fileAttachment",
                //        ContentBytes = fileStream,
                //        Name = fileName
                //    });
                //}
                //else if (isLinkBody)
                //{
                //    linkBody = AddLinkOnBody(objQueuedEmail);
                //}
                //objQueuedEmail.Body += linkBody;
                ItemBody body = new ItemBody();
                body.Content = objQueuedEmail.Body;
                body.ContentType = BodyType.Html;
                msg.Body = body;
                //if (objQueuedEmail.Attechments != null && objQueuedEmail.Attechments.Count > 0)
                //{
                //    foreach (var item in objQueuedEmail.Attechments)
                //    {
                //        attachments.Add(new FileAttachment
                //        {
                //            ODataType = "#microsoft.graph.fileAttachment",
                //            ContentBytes = item.Attachment,
                //            Name = item.AttachmentName
                //        });
                //    }
                //}
                //msg.Attachments = attachments;

                if (objQueuedEmail.lstAttechment != null && objQueuedEmail.lstAttechment.Count > 0)
                {
                    foreach (var item in objQueuedEmail.lstAttechment)
                    {
                        if (item.FilePath != null && item.FilePath != "")
                        {
                            string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                            Common.Log(attachmentFilename);
                            attachments.Add(new FileAttachment
                            {
                                ODataType = "#microsoft.graph.fileAttachment",
                                ContentBytes = System.IO.File.ReadAllBytes(attachmentFilename),
                                ContentType = MimeMapping.GetMimeMapping(attachmentFilename),
                                Name = Path.GetFileName(item.FilePath)
                            });

                            msg.HasAttachments = true;
                            msg.Attachments = attachments;
                        }
                    }
                }

                msg.Subject = objQueuedEmail.Subject;

                graphClient.Users[SenderMailId]
                .SendMail(msg, false)
                .Request()
                .PostAsync().GetAwaiter().GetResult();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
using FormBot.Entity.JobHistory;
using FormBot.Entity.Signature;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace FormBot.BAL.Service
{
    public class CreateJobHistoryBAL : ICreateJobHistoryBAL
    {
        /// <summary>
        /// Logs the job history.
        /// </summary>
        /// <typeparam name="T">type param</typeparam>
        /// <param name="objData">The object data.</param>
        /// <param name="HistoryCategoryID">The history category identifier.</param>
        /// <returns>bool</returns>
        public bool LogJobHistory<T>(T objData, HistoryCategory HistoryCategoryID,int UserId = 0)
        {
            try
            {
                string HTMLTemplate = string.Empty;
                HistoryTemplate historyTemplate = new HistoryTemplate(HistoryCategoryID);
                string template = historyTemplate.HTMLTemplate;

                Type objType = typeof(T);
                if (objType == typeof(CreateJob))
                {
                    CreateJob objJobAdd = objData as CreateJob;
                    HTMLTemplate = GetTemplate<CreateJob>(objJobAdd, template);
                    int result = SaveJobHistory(objJobAdd.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if(HistoryCategoryID == HistoryCategory.AddNote)
                {
                    JobNotes objjobnotes = objData as JobNotes;
                    HTMLTemplate = GetTemplate<JobNotes>(objjobnotes, template)
                                    .Replace("@Notes", objjobnotes.Notes);
                    int result = SaveJobHistory(objjobnotes.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.AddNotesWithMention)
                {
                    JobNotes objNote = objData as JobNotes;
                    HTMLTemplate = GetTemplate<JobNotes>(objNote, template)
                                    .Replace("@Tagged", objNote.TaggedUser.ToString())
                                    .Replace("@Notes", objNote.Notes);


                    int result = SaveJobHistory(objNote.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobNotes))
                {
                    JobNotes objNote = objData as JobNotes;
                    HTMLTemplate = GetTemplate<JobNotes>(objNote, template);
                    int result = SaveJobHistory(objNote.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(BasicDetails) && HistoryCategoryID == HistoryCategory.JobDeleted)
                {
                    BasicDetails objJobDetail = objData as BasicDetails;
                    HTMLTemplate = GetTemplate<BasicDetails>(objJobDetail, template);
                    int result = SaveJobHistory(objJobDetail.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobSchedulingPopup) && (HistoryCategoryID == HistoryCategory.CreateSchedule || HistoryCategoryID == HistoryCategory.Rescheduled || HistoryCategoryID == HistoryCategory.RemovedSchedule))
                {
                    JobSchedulingPopup objJobDetail = objData as JobSchedulingPopup;
                    HTMLTemplate = GetTemplate<JobSchedulingPopup>(objJobDetail, template);
                    int result = SaveJobHistory(objJobDetail.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, objJobDetail.UserId);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobScheduling) && (HistoryCategoryID == HistoryCategory.AcceptedSchedule || HistoryCategoryID == HistoryCategory.DeclinedSchedule))
                {
                    JobScheduling objJobDetail = objData as JobScheduling;
                    HTMLTemplate = GetTemplate<JobScheduling>(objJobDetail, template);
                    int result = SaveJobHistory(objJobDetail.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(EmailMessage) && HistoryCategoryID == HistoryCategory.MessageSent)
                {
                    EmailMessage objJobDetail = objData as EmailMessage;
                    HTMLTemplate = GetTemplate<EmailMessage>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.id), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobHistory) && (HistoryCategoryID == HistoryCategory.Generated || HistoryCategoryID == HistoryCategory.Deleted || HistoryCategoryID == HistoryCategory.Uploaded || HistoryCategoryID == HistoryCategory.Updated || HistoryCategoryID == HistoryCategory.TermsAndConditions ))
                {
                    JobHistory objJobDetail = objData as JobHistory;
                    HTMLTemplate = GetTemplate<JobHistory>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(ContractorHistory) && (HistoryCategoryID == HistoryCategory.SCASentJob || HistoryCategoryID == HistoryCategory.RemoveSSCRequest || HistoryCategoryID == HistoryCategory.CancelSSCRemoveRequest || HistoryCategoryID == HistoryCategory.AcceptedRejectedBySSC))
                {
                    ContractorHistory objJobDetail = objData as ContractorHistory;
                    HTMLTemplate = GetTemplate<ContractorHistory>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(StatusHistory) && (HistoryCategoryID == HistoryCategory.PreapprovalStatus || HistoryCategoryID == HistoryCategory.ConnectionStatus || HistoryCategoryID == HistoryCategory.FailedForSTC || HistoryCategoryID == HistoryCategory.SubmittedForSTC))
                {
                    StatusHistory objJobDetail = objData as StatusHistory;
                    HTMLTemplate = GetTemplate<StatusHistory>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobEmailConversationForPreAndConn) && (HistoryCategoryID == HistoryCategory.ConnectionDocuments || HistoryCategoryID == HistoryCategory.PreapprovalDocuments))
                {
                    JobEmailConversationForPreAndConn objJobDetail = objData as JobEmailConversationForPreAndConn;
                    HTMLTemplate = GetTemplate<JobEmailConversationForPreAndConn>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobId), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(STCHistory) && (HistoryCategoryID == HistoryCategory.STCSubmission))
                {
                    STCHistory objJobDetail = objData as STCHistory;
                    HTMLTemplate = GetTemplate<STCHistory>(objJobDetail, template);
                    //HTMLTemplate = HTMLTemplate +". "+ objJobDetail.Message;
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, objJobDetail.ModifiedBy, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(InvoiceHistory) && (HistoryCategoryID == HistoryCategory.InvoiceSent))
                {
                    InvoiceHistory objJobDetail = objData as InvoiceHistory;
                    HTMLTemplate = GetTemplate<InvoiceHistory>(objJobDetail, template);
                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(Signature))
                {
                    Signature objSignature = objData as Signature;
                    HTMLTemplate = GetTemplate<Signature>(objSignature, template);
                    int result = SaveJobHistory(Convert.ToInt32(objSignature.JobId), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(SignatureApproved))
                {
                    SignatureApproved objSignature = objData as SignatureApproved;
                    HTMLTemplate = GetTemplate<SignatureApproved>(objSignature, template);
                    int result = SaveJobHistory(Convert.ToInt32(objSignature.JobId), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(ModifiedStcValueHistory))
                {
                    ModifiedStcValueHistory objModifiedStcValue = objData as ModifiedStcValueHistory;
                    HTMLTemplate = objModifiedStcValue.HistoryMessage + ' ' + GetTemplate<ModifiedStcValueHistory>(objModifiedStcValue, template);
                    int result = SaveJobHistory(Convert.ToInt32(objModifiedStcValue.JobId), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobHistory) && HistoryCategoryID == HistoryCategory.ModifiedIsGst)
                {
                    JobHistory objJobHistory = objData as JobHistory;
                    HTMLTemplate = GetTemplate<JobHistory>(objJobHistory, template);
                    int result = SaveJobHistory(objJobHistory.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(STCHistory) && HistoryCategoryID == HistoryCategory.FSAStcSubmission)
                {
                    STCHistory objJobHistory = objData as STCHistory;
                    HTMLTemplate = GetTemplate<STCHistory>(objJobHistory, template);
                    int result = SaveJobHistory(objJobHistory.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(PanelCompare) && HistoryCategoryID == HistoryCategory.PanelRemoved)
                {
                    PanelCompare objPanelCompare = objData as PanelCompare;
                    HTMLTemplate = GetTemplate<PanelCompare>(objPanelCompare, template);
                    if (UserId == 0)
                        UserId = ProjectSession.LoggedInUserId;
                    int result = SaveJobHistory(objPanelCompare.JobID, HTMLTemplate, (int)HistoryCategoryID, UserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(PanelCompare) && HistoryCategoryID == HistoryCategory.PanelAdded)
                {
                    PanelCompare objPanelCompare = objData as PanelCompare;
                    HTMLTemplate = GetTemplate<PanelCompare>(objPanelCompare, template);
                    if (UserId == 0)
                        UserId = ProjectSession.LoggedInUserId;
                    int result = SaveJobHistory(objPanelCompare.JobID, HTMLTemplate, (int)HistoryCategoryID, UserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(PanelCompare) && HistoryCategoryID == HistoryCategory.PanelUpdated)
                {
                    PanelCompare objPanelCompare = objData as PanelCompare;
                    HTMLTemplate = template.Replace("@Oldbrand", objPanelCompare.Brand.Split('$')[1])
                                           .Replace("@Oldmodel", objPanelCompare.Model.Split('$')[1])
                                           .Replace("@Oldcount", objPanelCompare.Count.Split('$')[1])
                                           .Replace("@Newbrand", objPanelCompare.Brand.Split('$')[0])
                                           .Replace("@Newmodel", objPanelCompare.Model.Split('$')[0])
                                           .Replace("@Newcount", objPanelCompare.Count.Split('$')[0]);
                    if (UserId == 0)
                        UserId = ProjectSession.LoggedInUserId;
                    int result = SaveJobHistory(objPanelCompare.JobID, HTMLTemplate, (int)HistoryCategoryID, UserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if ( HistoryCategoryID == HistoryCategory.SPVInstallationVerified)
                {
                    JobHistory objJobDetail = objData as JobHistory;

                    HTMLTemplate = template.Replace("@Code", objJobDetail.ErrorCode)
                                           .Replace("@Details", objJobDetail.ErrorDetails)
                                           .Replace("@Description", objJobDetail.ErrorDescription);

                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SPVInstallationVerifiedSuccess||HistoryCategoryID==HistoryCategory.SPVInstallationAlreadyDoneButFailed||HistoryCategoryID==HistoryCategory.AllReadyInstallationVerified || HistoryCategoryID== HistoryCategory.DoProductVerificationAgain || HistoryCategoryID == HistoryCategory.ReleaseSPV || HistoryCategoryID == HistoryCategory.ResetSPV || HistoryCategoryID == HistoryCategory.InstallationVerificationAlreadyDone||HistoryCategoryID==HistoryCategory.RemovedSPVAfterXMLVerification)
                {
                    JobHistory objJobDetail = objData as JobHistory;
                    HTMLTemplate = GetTemplate<JobHistory>(objJobDetail, template);
                    int result = SaveJobHistory(objJobDetail.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SerialNumberNotValidForVerify)
                {
                    JobHistory objJobDetail = objData as JobHistory;
                    HTMLTemplate = template.Replace("@SerialNumber", objJobDetail.SerialNumbers);
                                           

                    int result = SaveJobHistory(Convert.ToInt32(objJobDetail.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (objType == typeof(JobHistory) && (HistoryCategoryID == HistoryCategory.LockSerialNumber || HistoryCategoryID == HistoryCategory.UnLockSerialNumber))
                {
                    JobHistory objJobHistory = objData as JobHistory;
                    HTMLTemplate = GetTemplate<JobHistory>(objJobHistory, template).Replace("@UserName", ProjectSession.LoggedInName);
                    int result = SaveJobHistory(objJobHistory.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.DownloadDocument)
                {
                    JobHistory objJobHistory = objData as JobHistory;
                    HTMLTemplate = template.Replace("@Type", objJobHistory.DocumentType)
                                            .Replace("@HistoryMessage",objJobHistory.HistoryMessage);


                    int result = SaveJobHistory(Convert.ToInt32(objJobHistory.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.DownloadPhoto)
                {
                    JobHistory objJobHistory = objData as JobHistory;
                    HTMLTemplate = template.Replace("@cntPhotos",objJobHistory.PhotosCount)
                                            .Replace("@visitId", objJobHistory.UniqueVisitId);


                    int result = SaveJobHistory(Convert.ToInt32(objJobHistory.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SentToXero)
                {
                    STCInvoice sTCInvoice = objData as STCInvoice;
                    HTMLTemplate = GetTemplate<STCInvoice>(sTCInvoice, template)
                                        .Replace("@STCInvoiceNumber", sTCInvoice.STCInvoiceNumber.ToString())
                                        .Replace("@STC", sTCInvoice.STC)
                                        .Replace("@CompanyName", sTCInvoice.SolarCompany)
                                        .Replace("@date", sTCInvoice.CreatedDate.ToString())
                                        .Replace("@UserName", ProjectSession.LoggedInName);
                    int result = SaveJobHistory(sTCInvoice.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SendCreditNotesToXero)
                {
                    STCInvoice sTCInvoice = objData as STCInvoice;
                    HTMLTemplate = GetTemplate<STCInvoice>(sTCInvoice, template)
                                        .Replace("@STCInvoiceNumber", sTCInvoice.JobID.ToString())
                                        .Replace("@CompanyName", sTCInvoice.SolarCompany)
                                        .Replace("@date", DateTime.Now.ToString())
                                        .Replace("@UserName", ProjectSession.LoggedInName);
                    int result = SaveJobHistory(sTCInvoice.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SyncWithXero)
                {
                    Remittance remittance = objData as Remittance;
                    HTMLTemplate = GetTemplate<Remittance>(remittance, template)
                                                        .Replace("@STCInvoiceNumber", remittance.STCInvoiceNumber)
                                                        .Replace("@date", DateTime.Now.ToString())
                                                        .Replace("@JobID", remittance.JobId.ToString());                                     
                    int result = SaveJobHistory(remittance.JobId, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.PartiallySettledWithXero)
                {
                    Remittance remittance = objData as Remittance;
                    HTMLTemplate = GetTemplate<Remittance>(remittance, template)
                                                        .Replace("@STCInvoiceNumber", remittance.STCInvoiceNumber)
                                                        .Replace("@date", DateTime.Now.ToString())
                                                        .Replace("@JobID", remittance.JobId.ToString())
                                                        .Replace("@Paid", remittance.AmountPaid.ToString());
                    int result = SaveJobHistory(remittance.JobId, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.SendToSTCInovice )
                {
                    STCInvoice objSTCInvoice = objData as STCInvoice;
                    if (objSTCInvoice.InoviceTermID == 1)
                    {
                        template += " (Generated from STC Inoive Creation for jobs)";
                    }
                    else if (objSTCInvoice.InoviceTermID == 2)
                    {
                        template += " (Generated from Inserted Entry In REC)";
                    }
                    else if (objSTCInvoice.InoviceTermID == 3)
                    {
                        template += " (Generated from Updated PVD code from REC)";
                    }
                    else 
                    {
                        template += " (Generated from Imported Bulk Upload ID From REC)";
                    }
                    HTMLTemplate = GetTemplate<STCInvoice>(objSTCInvoice, template)
                                        .Replace("@STCInvoiceNumber", objSTCInvoice.STCInvoiceNumber.ToString())
                                        .Replace("@JobID", objSTCInvoice.JobID.ToString())
                                         .Replace("@date", DateTime.Now.ToString())
                                        .Replace("@UserName", objSTCInvoice.UserName);
                    int result = SaveJobHistory(Convert.ToInt32(objSTCInvoice.JobID), HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }

                else if (HistoryCategoryID == HistoryCategory.RemoveInvoiceFromGreenbot || HistoryCategoryID == HistoryCategory.UnmarkInvoice || HistoryCategoryID == HistoryCategory.MarkInvoice)
                {
                    JobHistory objJobHistory = objData as JobHistory;
                    HTMLTemplate = GetTemplate<JobHistory>(objJobHistory, template)
                        .Replace("@STCInvoiceNumber", objJobHistory.stcInvoiceNumber)
                        .Replace("@CreatedDate", DateTime.Now.ToString())
                        .Replace("@Message", objJobHistory.HistoryMessage);
                    int result = SaveJobHistory(objJobHistory.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                else if (HistoryCategoryID == HistoryCategory.AddedSerialNumber || HistoryCategoryID == HistoryCategory.RemoveSerialNumber)
                {
                    JobSerialNoupdate objjobserialnoupdate = objData as JobSerialNoupdate;
                    HTMLTemplate = GetTemplate<JobSerialNoupdate>(objjobserialnoupdate, template)
                                    .Replace("@SerialNo", objjobserialnoupdate.serialno)
                                    .Replace("@JobID", objjobserialnoupdate.JobID.ToString())
                                    .Replace("@Username", ProjectSession.LoggedInName);
                    int result = SaveJobHistory(objjobserialnoupdate.JobID, HTMLTemplate, (int)HistoryCategoryID, ProjectSession.LoggedInUserId, null);
                    if (result == 0)
                    {
                        return false;
                    }
                }
                

            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the job history.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="historyMessage">The history message.</param>
        /// <param name="historyCategoryID">The history category identifier.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="AssignTo">The assign to.</param>
        /// <returns>history identifier</returns>
        private int SaveJobHistory(int jobID, string historyMessage, int historyCategoryID, int modifiedBy, int? AssignTo)
        {
            try
            {
                string spName = "[JobHistory_Insert]";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
                sqlParameters.Add(DBClient.AddParameters("HistoryMessage", SqlDbType.NVarChar, historyMessage));
                sqlParameters.Add(DBClient.AddParameters("HistoryCategoryID", SqlDbType.Int, historyCategoryID));
                sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, modifiedBy));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
                if (AssignTo != null)
                {
                    sqlParameters.Add(DBClient.AddParameters("AssignTo", SqlDbType.Int, AssignTo));
                }
               
                object historyID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
                return Convert.ToInt32(historyID);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return 0;
            }

        }

        /// <summary>
        /// Get html template.
        /// </summary>
        /// <typeparam name="T">Soucre object type</typeparam>
        /// <param name="obj">Data souce object</param>
        /// <param name="templateString">Initial html template string</param>
        /// <returns>Sting HTML template</returns>
        private string GetTemplate<T>(T obj, string templateString)
        {
            var props = obj.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);

            foreach (PropertyInfo pro in props)
            {
                if (pro.PropertyType.IsClass && pro.PropertyType.Namespace != "System")
                {
                    var subProps = pro.PropertyType.GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);

                    foreach (PropertyInfo subPro in subProps)
                    {
                        if (templateString.Contains("@" + subPro.Name))
                        {
                            string replaceFrom = string.Empty;
                            string replaceTo = "@" + subPro.Name;
                            replaceFrom = subPro.GetValue(pro.GetValue(obj)) != null ? subPro.GetValue(pro.GetValue(obj)).ToString() : "";
                            templateString = templateString.Replace(replaceTo, replaceFrom);
                        }
                    }
                }
                else if (templateString.Contains("@" + pro.Name))
                {
                    string replaceFrom = string.Empty;
                    string replaceTo = "@" + pro.Name;
                    replaceFrom = pro.GetValue(obj) != null ? pro.GetValue(obj).ToString() : "";
                    templateString = templateString.Replace(replaceTo, replaceFrom);
                }
            }

            return templateString;
        }
    }
}

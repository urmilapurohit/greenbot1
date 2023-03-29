using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class HistoryTemplate
    {
        private int historyType;
        private string template;

        public HistoryTemplate(HistoryCategory HistoryCategoryID)
        {
            historyType = (int)HistoryCategoryID;
        }

        public string HTMLTemplate
        {
            get
            {
                switch (historyType)
                {
                    case 1:
                        template = "added the following job: <b class=\"blue-title\">@Title</b>" +
                                      "<p><label>Job Address: </label>@AddressDisplay</p>" +
                                      "<p><label>Description: </label>@Description</p>";
                        break;
                    case 2:
                        template = "modified the job part: <b class=\"blue-title\">@Title</b>";
                        break;
                    case 3:
                        template = "left note for job <b class=\"blue-title\">@JobTitle</b>" +
                                    "<p><label>Description: </label>@Notes</p>";
                        break;
                    case 4:
                        template = "sent new message to: <b class=\"blue-title\">@to</b>" +
                                    "<p><label>Subject: </label>@subject</p>";
                        break;
                    case 5:
                        template = "changed the stage of <b class=\"blue-title\">@Title</b> from <b>@PreviousJobStage</b> to <b>@CurrentJobStage</b>";
                        break;
                    case 6:
                        template = "deleted the job: <b class=\"blue-title\">@Title</b>";
                        break;
                    case 7:
                        template = "modified the job: <b class=\"blue-title\">@Title</b";
                        break;
                    case 8:
                        template = "changed job <b class=\"blue-title\">@Title</b> priority to <b>@CurrentPriority</b>.";
                        break;
                    case 9:
                        template = "have scheduled JobID @JobID to <b>#UserName#</b> @strVisitStartDate - @strVisitEndDate @strVisitStartTime - @strVisitEndTime.";
                        break;
                    case 10:
                        template = "have removed #UserName# from JobID @JobID. @strVisitStartDate - @strVisitStartTime.";
                        break;
                    case 11:
                        template = "have rescheduled JobID @JobID to @strVisitStartDate - @strVisitEndDate @strVisitStartTime - @strVisitEndTime.";
                        break;
                    case 12:
                        template = "have <b>accepted</b> job <b class=\"blue-title\">@JobTitle</b>";
                        break;
                    case 13:
                        template = "have <b>declined</b> job <b class=\"blue-title\">@JobTitle</b>";
                        break;
                    case 14:
                        template = "has generated a @HistoryMessage.<br /><b><a href=\"javascript:void();\" onclick=\"DownloadHistoryDocument(this)\" data-name=\"@DocumentName\" data-folder=\"@DocumentPath\" class=\"blue-title\"><img src=\"/Images/attach_document.png\">&nbsp;&nbsp;Document</a></b>";
                        break;
                    case 15:
                        template = "has deleted a <b class=\"blue-title\">@HistoryMessage</b>.";
                        break;
                    case 16:
                        template = "has uploaded a @HistoryMessage.<br /><b><a href=\"javascript:void();\" onclick=\"DownloadHistoryDocument(this)\" data-name=\"@DocumentName\" data-folder=\"@DocumentPath\" class=\"blue-title\"><img src=\"/Images/attach_document.png\">&nbsp;&nbsp;Document</a></b>";
                        break;
                    case 17:
                        template = "has updated a <b class=\"blue-title\">@HistoryMessage</b>.";
                        break;
                    case 18:
                        template = "<b class=\"blue-title\">@SolarCompanyName</b> sent JobID @JobID to SSC-@SSCName.";
                        break;
                    case 19:
                        template = "has requested to remove SSC from JobID @JobID.";
                        break;
                    case 20:
                        template = "has cancelled requested to remove SSC from JobID @JobID.";
                        break;
                    case 21:
                        template = "@Action JobID @JobID from @SolarCompanyName.";
                        break;
                    case 22:
                        template = "@Action preapproval status @Status @ChangedOn @Comment";
                        break;
                    case 23:
                        template = "@Action connection status @Status @ChangedOn @Comment";
                        break;
                    case 24:
                        template = "sent preapprovals documents to @Distributor";
                        break;
                    case 25:
                        template = "sent connection documents to @Distributor";
                        break;
                    case 26:
                        template = "traded Job  with @stcAmt STCs at <b class=\"blue-title\">$@STCPrice</b> Successfully on - @DT @Message";
                        break;
                    case 27:
                        template = "has just sent invoice <b class=\"blue-title\">@InvoiceNumber</b> to @InvoiceTo";
                        break;
                    case 28:
                        template = "submitted job for STC Creation. @Comment";
                        break;
                    case 29:
                        template = "failed STC submission for JobID @JobID. @Comment";
                        break;
                    case 30:
                        template = "Sent signature request to @SinatureType through @Type - @MobileNumber";
                        break;
                    case 31:
                        template = "@SinatureType Signature request made through @Type - @MobileNumber is approved.";
                        break;
                    case 32:
                        template = "@Name agreed to the terms and conditions.";
                        break;
					case 33:
						template = "Stc value got modified from @OldStcValue to @NewStcValue.";
						break;
                    case 34:
                        template = "modified Gst from @FunctionalityName";
                        break;
                    case 35:
                        template = "traded Job with (@UserType) @stcAmt STCs at <b class=\"blue-title\">$@STCPrice</b> Successfully on - @DT @Message";
                        break;
                    case 36:
                        template = "has updated Panel Details.Old panel details {Brand : @Oldbrand , Model : @Oldmodel , No. of Panel : @Oldcount} -> New panel details {Brand : @Newbrand , Model : @Newmodel , No. of Panel : @Newcount}";
                        break;
                    case 37:
                        template = "has added Panel {Brand : @Brand , Model : @Model , No. of Panel : @Count} for Job";
                        break;
                    case 38:
                        template = "has removed Panel {Brand : @Brand , Model : @Model , No. of Panel : @Count} from job";
                        break;
                    case 39:
                        template = "has done Installation Verified <br/><b class=\"blue-title\">Code:</b> @Code <br/><b class=\"blue-title\"> Details:</b> @Details <br/><b class=\"blue-title\">Description:</b> @Description";
                        break;
                    case 40:
                        template = "has done Installation Verified and It has been performed successfully.";
                            break;
                    case 41:
                        template = "has done Installation Verified and It has been already failed earlier.";
                        break;
                    case 43:
                        template = "has done Installation Verified and It has been already Verified.";
                        break;
                    case 44:
                        template = "has done Installation Verified and SerialNumber:@SerialNumber not valid.";
                        break;
                    case 45:
                        template = "release spv successfully.";
                        break;
                    case 46:
                        template = "re-set spv successfully.";
                        break;
                    case 47:
                        template = "has done Installation Verified but in this job product verification not done yet so installation verification should failed.";
                        break;
                    case 48:
                        template = "has Locked SerialNumber";
                        break;
                    case 49:
                        template = "has UnLocked SerialNumber";
                        break;
                    case 50:
                        template = "has downloaded <b class=\"blue-title\"> @HistoryMessage</b> from @Type.";
                        break;
                    case 51:
                        template = "has downloaded <b class=\"blue-title\">@cntPhotos photo</b> from <b class=\"blue-title\"> @visitId</b>.";
                        break;
					case 52:
                        template = "Installation verification is already done for re-submission";
                        break;
                    case 53:
                        template = "has been sent invoice @STCInvoiceNumber successfully with JobID @JobID for payment to Xero (@STC) - @CompanyName on @date";
                        break;
                    case 56:
                        template = "has removed @STCInvoiceNumber on @CreatedDate @Message.";
                        break;
                    case 57:
                        template = "has marked @STCInvoiceNumber on @CreatedDate @Message.";
                        break;
                    case 58:
                        template = "has unmarked @STCInvoiceNumber on @CreatedDate @Message.";
                        break;
                    case 55:
                        template = "has try to settlement of invoice for JobID @JobID and invoice @STCInvoiceNumber has been settled and moved to PAID status successfully from Xero on @date";
                        break;
                    case 59:
                        template = "has been created invoice for JobID @JobID and STCInvoiceNumber is @STCInvoiceNumber on @date";
                        break;
                    case 60:
                        template = "has been sent credit note @STCInvoiceNumber successfully with JobID @JobID for adjustment to Xero @CompanyName on @date";
                        break;
                    case 61:
                        template = "has try to settle invoice for JobID @JobID and invoice @STCInvoiceNumber has been partially settled(@Paid) successfully from Xero on @date";
                        break;
					case 54:
                        template = "has removed SPV after verification of installation SPV XML.";
                        break;
                    case 62:
                        template = "has added serial number @SerialNo for JobID @JobID ";
                        break;
                    case 63:
                        template = "has removed serial number @SerialNo for JobID @JobID ";
                        break;
                    case 64:
                        template = "left note for <b class='tagged-users'>@Tagged</b>" +
                                    "<p><label>Description: </label>@Notes</p>";
                        break;
                    case 65:
                        template = "<p><label>Description: </label>@Notes</p>";
                        break;
                    default:
                        template = string.Empty;
                        break;
                }

                return template;
            }
        }
    }

    public enum HistoryCategory
    {
        JobAdded = 1,
        Part = 2,
        Note = 3,
        MessageSent = 4,
        Stagechanged = 5,
        JobDeleted = 6,
        JobEdited = 7,
        PriorityChanged = 8,
        CreateSchedule = 9,
        RemovedSchedule = 10,
        Rescheduled = 11,
        AcceptedSchedule = 12,
        DeclinedSchedule = 13,
        Generated = 14,
        Deleted = 15,
        Uploaded = 16,
        Updated = 17,
        SCASentJob = 18,
        RemoveSSCRequest = 19,
        CancelSSCRemoveRequest = 20,
        AcceptedRejectedBySSC = 21,
        PreapprovalStatus = 22,
        ConnectionStatus = 23,
        PreapprovalDocuments = 24,
        ConnectionDocuments = 25,
        STCSubmission = 26,
        InvoiceSent = 27,
        SubmittedForSTC = 28,
        FailedForSTC = 29,
        Signature = 30,
        SignatureApproved = 31,
        TermsAndConditions = 32,
		ModifiedStcValue = 33,
        ModifiedIsGst = 34,
        FSAStcSubmission = 35,
        PanelUpdated = 36,
        PanelAdded = 37,
        PanelRemoved = 38,
        SPVInstallationVerified=39,
        SPVInstallationVerifiedSuccess=40,
        SPVInstallationAlreadyDoneButFailed=41,
        AllReadyInstallationVerified=43,
        SerialNumberNotValidForVerify=44,
        ReleaseSPV = 45,
        ResetSPV = 46,
        DoProductVerificationAgain=47,
        LockSerialNumber = 48,
        UnLockSerialNumber = 49,
        DownloadDocument=50,
        DownloadPhoto=51,
		InstallationVerificationAlreadyDone = 52,
        SentToXero = 53,
        RemovedSPVAfterXMLVerification = 54,
        SyncWithXero = 55,
        RemoveInvoiceFromGreenbot = 56,
        MarkInvoice = 57,
        UnmarkInvoice = 58,
        SendToSTCInovice = 59,
        SendCreditNotesToXero = 60,
        PartiallySettledWithXero = 61,
        AddedSerialNumber = 62,
        RemoveSerialNumber = 63,
        AddNotesWithMention = 64,
        AddNote = 65,
        SigntureOTPRequest = 66,
        ChangeVisitStatus = 67,
        SentSignRequestForDoc=68,
        CompletedSignRequestDoc=69,
        SignCompletedByAllParties=70,
        JobOwnerDetails=71,
        JobBasicDetails=72,
        JobInstallationDetails=73,
        ModifiedSystemSize=74,
        ProductVerification=75,
        DeleteNote = 76,
        EditNote = 77,
        UploadPhoto=78,
        JobSTCDetails=79,
        BulkChangeInvoicePaymentStatus=80,
        DeletePhotos=81,
        InverterDetails=82,
        InstallerDesignerDetails=83,
        SCAChangeDetails=84
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Helper
{
    public class SystemEnums
    {
        public enum MessageType
        {
            Success = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        public enum MenuController
        {
            Home = 1
        }

        public enum CERType
        {
            [Description("Photovoltaic Modules")]
            PhotovoltaicModules = 0,
            [Description("Approved Inverters")]
            ApprovedInverters = 1,
            [Description("Accredited Installers")]
            AccreditedInstallers = 2,
            [Description("HW Brand and HW Model")]
            HWBrandModel = 3,
            [Description("Serial Numbers")]
            SerialNumbers = 4,
            [Description("Electricity Provider")]
            ElectricityProvider = 5,
            [Description("Battery Storage")]
            BatteryStorage = 6,
            [Description("SPV Manufacturer")]
            SPVManufacturer=7
        }

        public enum CERSubType
        {
            AirSource = 0,
            SolarWaterLess700L = 1,
            SolarWaterMore700L = 2,
            None = 3
        }

        public enum ViewPageId
        {
            JobView = 1,
            STCSubmissionView = 2,
            STCInvoiceView = 3,
            SAASInvoiceView = 4
        }

        public enum UserType
        {
            FormBotSuperAdmin = 1,
            ResellerAdmin = 2,
            FormBotComplianceOfficer = 3,
            SolarCompanyAdmin = 4,
            ResellerAccountManager = 5,
            SolarSubContractorAdmin = 6,
            [Description("Solar Electricians")]
            SolarElectricians = 7,
            SolarConnectionsOfficer = 8,
            SolarContractor = 9,
            [Description("SWH Users")]
            SolarElectriciansSWH = 10
        }

        /// <summary>
        /// User Status
        /// </summary>
        public enum UserStatus
        {
            Pending = 1,
            Approved = 2,
            Rejected = 3,
            Require_More_Paperwork = 4
        }

        /// <summary>
        /// Job Scheduling Status
        /// </summary>
        public enum JobSchedulingStatus
        {
            Pending = 1,
            Approved = 2,
            Rejected = 3
        }

        /// <summary>
        /// Electrician Status
        /// </summary>
        public enum ElectricianStatus
        {
            Request_Send = 1,
            Request_Accepted = 2,
            Request_Rejected = 3
        }

        public enum Theme
        {
            green = 1,
            blue = 2,
            pink = 3,
            skyblue = 4,
            yellow = 5,
            black = 6
        }

        public enum SEDesignRole
        {
            Design = 2,
            Install = 1,
            Design_Install = 3
        }

        public enum PostalAddressType
        {
            [Description("physical address")]
            Different_Physical_Address = 1,

            [Description("P.O BOX")]
            Post_Delivery_Address = 2
        }

        public enum TempDataKey
        {
            RoleMenu = 1,
            IsProfile = 2,
            IsSendRequest = 3,
            SCAComplianceCheck = 4,
            SEComplianceCheck = 5,
            RAMViewAllJob = 6,
            SCOViewAssignJob = 7,
            AutoRECUpload = 8
        }

        [Serializable]
        public enum MenuId
        {
            Roles = 11,
            RoelList = 12,
            RoleView = 43,
            RoleAdd = 13,
            RoleEdit = 14,
            RoleDelete = 15,
            Users = 21,
            UserList = 22,
            UserView = 44,
            UserAdd = 25,
            UserEdit = 26,
            UserDelete = 27,
            SCAList = 23,
            SCAView = 52,
            SCADelete = 37,
            SEList = 24,
            SEView = 53,
            SEDelete = 39,
            FCOList = 30,
            FCOAdd = 31,
            FCOEdit = 32,
            FCODelete = 33,
            profile = 35,
            EmailTemplate = 45,
            EmailTemplateList = 46,
            EmailTemplateView = 50,
            EmailTemplateAdd = 47,
            EmailTemplateEdit = 48,
            EmailTemplateDelete = 49,
            SendRequestToElectrician = 51,
            SCAComplianceChek = 36,
            SEComplianceChek = 38,
            Email = 29,
            EmailSetting = 54,
            JobView = 56,
            JobAdd = 60,
            JobEdit = 61,
            JobDelete = 62,
            JobNoteAdd = 64,
            JobNoteView = 65,
            JobMessageSend = 67,
            JobMessageView = 68,
            JobHistoryView = 70,
            JobHistoryDocumentAccess = 71,
            JobDocumentManagerView = 73,
            JobDocumentManagerGenerate = 74,
            JobDocumentManagerSave = 75,
            JobDocumentManagerUpload = 76,
            JobDocumentManagerDelete = 77,
            JobPhotoView = 79,
            JobPhotoUpload = 80,
            JobPhotoDelete = 81,
            JobSchedulingView = 83,
            JobSchedulingAddEdit = 84,
            JobSchedulingDelete = 85,
            JobAssignToSCO = 86,
            JobAssignToSSC = 87,
            SolarSubContractorView = 90,
            SolarSubContractorSendRequest = 91,
            JobNoteDelete = 92,
            STCSubmissionView = 96,
            JobInvoiceView = 103,
            JobEmailView = 109,
            STCAssignToCompliance = 112,
            STCExportCSV = 113,
            STCSendToXero = 114,
            STCCreateInRec = 115,
            STCInvoiceView = 117,
            InvoiceMarkUnMark = 118,
            InvoiceRemoveSelected = 119,
            InvoiceBulkChange = 120,
            InvoiceSendToXero = 121,
            InvoiceSyncWithXero = 122,
            RequestedSEDelete = 139,
            InvoiceBulkUploadRemittances = 144,
            InvoiceImportCSV = 145,
            InvoiceExportCSV = 146,
            InvoiceEdit = 147
            ,WholesalerSCASettlementTermView = 152   //165 (For Local); 160 (For Staging); 152 (For Live)
            , AllScaJobView = 154     //174 (For Local); 163 (For Staging); 154 (For Live)
            , ShowOnlyAssignJobsSCO = 157 // 177 (For Local); 167 (For Staging); 157(For Live)
            , IsTradableJob = 159//179(For Local); // 169 (For Staging); 159 (For Live)
             , InstallerDetails = 163
             , ScheduleInstaller = 164
              , STCStatus = 165
              , RetailerDetails = 166
              , Photos = 167
              , CESForm = 168
              , STCForm = 169
              , DocumentManager = 170
              , PanelSerialNubers = 171
              , OtherDetails = 172
              , CustomDetails = 173
              , jobActions = 174
              , History = 175
              , Message = 176
              , Printjob = 177
              , preapprovals =178
              , connections = 179
              , RECLogin = 180
              , LockUnlockSerialNo = 185
              , IsAllowAccesstoUploadGPSPhoto = 186
              , AutoRECUpload = 189
                , SaveJobAfterTrade=190
                ,UploadPhotoAfterTrade=191
                ,SaveDocAfterTrade=192
                ,UploadSignAfterTrade=193
                , GenerateFullJobPack=196
                , EditColumns=198
                , IsAllowVisibleInstallerSelfiePhoto=200,
            ChangeSCA = 202,
            PnlInvoiceform = 203,
            ElecBillform = 204,
            IsAllowVisibleTabulerViewSwitch =207
        }

        public enum JobDetails
        {

            UploadDocumentCES=1,//upload ces doc,upload doc from doc manager
            UploadDocument=2,//upload stc doc
            UploadDocumentOther=3,
            UploadSTCDocument=4,//create stc doc
            DeleteDocument=5,//delete STC doc
            DeleteDocumentNew=6,//del CES doc,Delete doc manager doc
            UploadReferencePhoto=7,
            DeleteCheckListPhotos=8,
            DownloadJobDocuments=9,//download CES doc,download  doc form doc manager 
            DownloadJobPhotos=10,
            DownloadAllDocumentsNew=11,//download all doc from doc manager
            DownloadSTCDocument=12,//download STC doc
            AddOtherDocuments=13,//create CES doc,doc manager create doc
            GenerateFullJobPack= 14//Download full job pack



        }
        /// <summary>
        /// Template Fields
        /// </summary>
        public enum TemplateFields
        {
            FirstName = 1,
            LastName = 2,
            Email = 3,
            Mobile = 4,
            LoginLink = 5,
            UserName = 6,
            Password = 7,
            Details = 8,
            SolarCompanyDetails = 9,
            SolarElectrician = 10
        }

        /// <summary>
        /// Job Type
        /// </summary>
        public enum JobType
        {
            PVD = 1,
            SWH = 2
        }

        /// <summary>
        /// Job Schedule Type
        /// </summary>
        public enum JobScheduleType
        {
            Scheduled = 1,
            Unscheduled = 2
        }

        /// <summary>
        /// Job Priority
        /// </summary>
        public enum JobPriority
        {
            High = 1,
            Normal = 2,
            Low = 3
        }

        public enum Severity
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public enum InputTypes
        {
            BUTTON = 1,
            CHECK_BOX = 2,
            RADIO_BUTTON = 3,
            TEXT_FIELD = 4,
            LIST_BOX = 5,
            COMBO_BOX = 6
        }
        public enum LampType
        {
            [Description("Non-emerging technology")]
            NonEmergingTechnology = 1,
            [Description("Emerging technology")]
            EmergingTechnology = 2,
            [Description("Retained For Before Upgrade")]
            RetainedForBeforeUpgrade = 3


        }

        public enum JobStage
        {
            [Description("PRE")]
            PreApprovals = 1,
            [Description("CES")]
            CES = 4,
            [Description("CONN")]
            Connections = 2,
            [Description("STC")]
            STC = 3,
            [Description("OTHER")]
            Other = 5,
            STCTrade = 6,
        }

        /// <summary>
        /// STC JobStatus
        /// </summary>
        public enum STCJobStatus
        {
            [Description("Not Yet Submitted")]
            [SubDescription("#708090")]
            NotYetSubmitted = 10,
            [Description("Re-submission")]
            [SubDescription("#C0C0C0")]
            Resubmission = 11,
            [Description("Submit to Trade")]
            [SubDescription("#708090")]
            SubmittoTrade = 12,
            [Description("Awaiting Authorization")]
            [SubDescription("#32CD32")]
            AwaitingAuthorization = 13,
            ////[Description("Failure due to REC Audit")]
            ////FailureduetoRECAudit = 14,
            [Description("CER Failed")]
            [SubDescription("#DC143C")]
            CERFailed = 14,
            [Description("Cannot re-create")]
            [SubDescription("#DC143C")]
            Cannotrecreate = 15,
            [Description("Under Review")]
            [SubDescription("#FFA500")]
            UnderReview = 16,
            [Description("Compliance Issues")]
            [SubDescription("#DC143C")]
            ComplianceIssues = 17,
            [Description("Requires Call Back")]
            [SubDescription("#FF0000")]
            RequiresCallBack = 18,
            ////[Description("Approved for REC Creation")]
            ////ApprovedforRECCreation = 19,
            [Description("Ready To Create")]
            [SubDescription("#006400")]
            ReadyToCreate = 19,
            ////[Description("Successfully Traded")]
            ////SuccessfullyTraded = 20,
            [Description("Pending Approval")]
            [SubDescription("#006400")]
            PendingApproval = 20,
            [Description("New Submission")]
            [SubDescription("#C0C0C0")]
            NewSubmission = 21,
            ////[Description("REC Approved")]
            ////RECApproved = 22
            [Description("CER Approved")]
            [SubDescription("#006400")]
            CERApproved = 22
        }

        /// <summary>
        /// STC Settlement Term
        /// </summary>
        public enum STCSettlementTerm : int
        {
            [Description("24 Hour")]
            [SubDescription("CER Processing")]
            Hour24 = 1,
            
            [Description("3 Days")]
            [SubDescription("CER Processing")]
            Days3 = 2,
            
            [Description("7 Days")]
            [SubDescription("CER Processing")]
            Days7 = 3,
            
            [Description("CER Approved")]
            [SubDescription("CER Approved")]
            CERApproved = 4,
            
            [Description("Partial Payment")]
            [SubDescription("CER Processing")]
            PartialPayments = 5,
            
            [Description("Upfront")]
            [SubDescription("CER Processing")]
            UpFront = 6,

            [Description("Rapid-Pay")]
            [SubDescription("(Paid in 24-48 Hours)")]
            RapidPay = 7,
            
            [Description("Opti-Pay")]
            [SubDescription("(Locked price, Settle on Approval)")]
            OptiPay = 8,
            
            [Description("Commercial")]
            [SubDescription("(Jobs < 60kW +GST)")]
            Commercial = 9,
           
            [Description("Custom")]
            [SubDescription("CER Processing")]
            Custom = 10,

            [Description("InvoiceStc")]
            //[SubDescription("CER Processing")]
            InvoiceStc = 11,
            
            [Description("PeakPay")]
            [SubDescription("Includes PeakPay Fees")]
            PeakPay = 12

        }

        /// <summary>
        /// SAAS Settlement Term
        /// </summary>
        public enum SAASSettlementTerm
        {
            [Description("STC Amount")]
            [SubDescription("")]
            STCAmount = 1,
            
            [Description("JobAmount")]
            [SubDescription("")]
            JobAmount = 2,
            
            [Description("Feature 1")]
            [SubDescription("")]
            Feature1 = 3,
        }

        /// <summary>
        /// Job Document Type
        /// </summary>
        public enum JobDocumentType
        {
            Email = 1,
            RefNo = 2,
            Submitted = 3
        }

        public enum ServiceResult
        {
            Failure,
            Success,
            UserNotActive
        }

        public enum InvoiceStatus
        {
            [Description("PAID IN FULL")]
            PaidFull = 1,
            [Description("CANCELLED")]
            Cancelled = 2,
            [Description("PARTIAL PAYMENT ")]
            PartialPayment = 3,
            [Description("OUTSTANDING")]
            OutStanding = 4
        }

        public enum JobInvoiceType
        {
            Part = 1,
            Time = 2,
            Payment = 3
        }

        public enum PaymentType
        {
            ManualCreditCard = 1,
            Cash = 2,
            Cheque = 3,
            Credit = 4,
            BankTransfer = 5,
            Eftpos = 6,
            Debit = 7

        }

        public enum DateGrouping
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Yearly = 4
        }

        public enum SWHInstallationType
        {
            [Description("New Building")]
            New_Building = 1,
            [Description("Replaced Electric Heater")]
            Replaced_Electric_Heater = 2,
            [Description("Replaced Solar Water Heater")]
            Replaced_Solar_Water_Heater = 3,
            [Description("First Solar Water Heater At Existing Building")]
            First_Solar_Water_Heater_At_Existing_Building = 4,
            [Description("Replaced Gas Water Heater")]
            Replaced_Gas_Water_Heater = 5,
            [Description("Other")]
            Other = 6,

        }

        public enum PVDInstallationType
        {
            [Description("New")]
            New = 1,
            [Description("Additional")]
            Additional = 2,
            [Description("Replacement System")]
            Replacement_System = 3

        }

        public enum ConnectionType
        {
            [Description("Connected To An Electricity Grid WithOut Battery Storage")]
            Connected_To_An_Electricity_Grid_WithOut_Battery_Storage = 1,
            [Description("Connected To An Electricity Grid With Battery Storage")]
            Connected_To_An_Electricity_Grid_With_Battery_Storage = 2,
            [Description("Stand-Alone(Not Connected To An Electricity Grid)")]
            Stand_Alone_Not_Connected_To_An_Electricity_Grid = 3
        }

        public enum MountingType
        {
            [Description("Building Or Structure")]
            Building_Or_Structure = 1,
            [Description("Ground Mounted Or Free Standing")]
            Ground_Mounted_Or_Free_Standing = 2
        }

        public enum TaxType
        {
            OUTPUT = 1, // GST on Income
            EXEMPTOUTPUT = 2 // GST Free Income
        }

        public enum TypeOfSignature
        {
			[Description("Owner_Signature")]
            Home_Owner = 1,
			[Description("Installer_Signature")]
			Installer = 2,
			[Description("Electrician_Signature")]
			Electrician = 3,
			[Description("Designer_Signature")]
			Designer = 4,
			[Description("Other_Signature")]
			Other = 5,
			[Description("SCA_Signature")]
			SolarCompnay = 6,
            [Description("Retailer_Signature")]
            Retailer = 6
        }

        public enum SerialNumberSeparatorId
        {
            Comma = 1,
            NewLine = 2,
            Colon = 3,
            SemiColon = 4
        }

        public enum CheckListPDFLocation
        {
            Document_Manager = 1,
            CES = 2
        }

        public enum VisitStatus
        {
            Open = 1,
            Completed = 2
        }

        public enum PhotoQuality
        {
            High = 1,
            Medium = 2,
            Low = 3
        }
        public enum SelfiePhotoType
        {
            [Description("Begin Installation")]
            BeginInstallation = 1,
            [Description("During Installation")]
            DuringInstallation = 2,
            [Description("Completed Installation")]
            CompletedInstallation = 3
        }

        public enum HorizontalAlignment
        {
            Left = 0,
            Center = 1,
            Right = 2
        }

        public enum VerticalAlignment
        {
            Top = 0,
            Middle = 1,
            Bottom = 2
        }

        public enum SignatureType
        {
            Draw = 0,
            Mobile = 1,
            Email = 2
        }
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }

        public class SubDescriptionAttribute : Attribute
        {
            public string SubDescription;
            public SubDescriptionAttribute(string typeText) { SubDescription = typeText; }
        }


        public enum DocumentType
        {
            CES = 1,
            STC = 2,
            OTHER = 3
        }

        public enum DocumentNameId  //for classic screen
        {
            STC_Assignment_Form  = 43,
            cespvd = 44,
            EESG_SWH_Form = 45,
            cessw = 46,
        }
                
        public enum PhotoType
        {
            Installation = 1,   // for newJob 1 = SerialNum, 2= Installation
            SerialNumber = 2
        }

        /// <summary>
        /// Job Schedule Type
        /// </summary>
        public enum TradeStatus
        {
            Traded = 1,
            ReadyToTrade = 2,
            NotTraded = 3
        }
		public static string GetDescription(Enum value)
		{
			return
				value
					.GetType()
					.GetMember(value.ToString())
					.FirstOrDefault()
					?.GetCustomAttribute<DescriptionAttribute>()
					?.Description;
		}

        public static string GetSubDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<SubDescriptionAttribute>()
                    ?.SubDescription;
        }

        public enum DocumentChangeLog
		{
			[Description("Company Admin")]
			Created = 1,
			RequestSent = 2,
			Open = 3,
			Signed = 4,
			Completed = 5,
			CompletedSent = 6
		}

		public enum DocumentUsersRoleName
		{
			[Description("Company Admin")]
			Admin = 1,
			[Description("Installer")]
			Installer = 2,
			[Description("Designer")]
			Designer = 3,
			[Description("Electrician")]
			Electrician = 4,
			[Description("System Owner")]
			Owner = 5,
			Resaller = 6,


			
		}

		public enum UserTypeForDocument
		{
			[Description("System Owner")]
			FormBotSuperAdmin = 1,
			[Description("Reseller")]
			ResellerAdmin = 2,
			[Description("Compliance Officer")]
			FormBotComplianceOfficer = 3,
			[Description("Company Admin")]
			SolarCompanyAdmin = 4,
			[Description("Account Manager")]
			ResellerAccountManager = 5,
			[Description("Contractor")]
			SolarSubContractorAdmin = 6,
			[Description(" Electricians")]
			SolarElectricians = 7,
			[Description("Connection Officer")]
			SolarConnectionsOfficer = 8,
			[Description("Contractor")]
			SolarContractor = 9,
			[Description("Electricians")]
			SolarElectriciansSWH = 10
		}

        public enum PricingType
        {
            PricingGlobal = 1,
            PricingSolarCompany = 2,
            PricingJob = 3
        }

        /// <summary>
        /// Sca Proof Document Type
        /// </summary>
        public enum ProofDocumentType
        {
            DirectorsDriversLicense = 1,
            AccreditationDocumentations = 2,
            ProofOfBusinessAddress = 3,
            PublicLiabilityInsurance = 4,
            InstallersDriversLicense = 5,
            SAAS = 6
        }


        public enum ActivityLogType
        {
            [Description("Create Job")]
            CreateJob = 1
        }

        public enum SpvServiceReponceStatus 
        {
            [Description("Invalid Signature")]
            [SubDescription("{0}")]
            SignatureNotValid = 100,

            [Description("Verification of Manufacturer not available")]
            [SubDescription("{0}")]
            VerificationOfManufacturerNotAvailable = 101,

            [Description("Serial Number not valid")]
            [SubDescription("Following Serial Numbers are invalid: {0}")]
            SerialNumberNotValid = 102,

            [Description("Mandatory field missing")]
            [SubDescription("Following fields are missing : {0}")]
            MandatoryFieldMissing = 103,

            [Description("XML not valid")]
            [SubDescription("Following tags are missing : {0}")]
            XMLNotValid = 104,

            [Description("Town/State/PostCode are not valid")]
            [SubDescription("{0}")]
            Town_State_PostCodeNotValid = 105,


            [Description("Serial Number Already Verified")]
            [SubDescription("Following Serial Numbers are already verified: {0}")]
            SerialNumberAlreadyVerified = 106,

            [Description("Internal Error")]
            [SubDescription("{0}")]
            InternalError = 500
        }


        /// <summary>
        /// Spv request name
        /// </summary>
        public enum SpvRequest
        {
            ProductVerification = 1,
            InstallationVerification = 2
        }

        /// <summary>
        /// Spv request verification status
        /// </summary>
        public enum SpvVerificationStatus
        {
            Success = 1,
            Fail = 2
        }

        public enum NotesType
        {
            All=0,
            Public = 1,
           // Private = 2,
            Compliance = 3
           // Installer =4
        }

        public enum JobHistoryFilter
        {
            General = 1,
            Interaction = 2,
            Statuses = 3,
            Notifications = 4,
            Invoicing = 5,
            Documents = 6,
            Scheduling = 7,
            Signature = 8
        }

        //public enum JobHistoryCategory
        //{
        //    JobAdded = 1,
        //    Part = 2,
        //    Note = 3,
        //    MessageSent = 4,
        //    Stagechanged = 5,
        //    JobDeleted = 6,
        //    JobEdited = 7,
        //    PriorityChanged = 8,
        //    CreateSchedule = 9,
        //    RemovedSchedule = 10,
        //    Rescheduled = 11,
        //    AcceptedSchedule = 12,
        //    DeclinedSchedule = 13,
        //    Generated = 14,
        //    Deleted = 15,
        //    Uploaded = 16,
        //    Updated = 17,
        //    SCASentJob = 18,
        //    RemoveSSCRequest = 19,
        //    CancelSSCRemoveRequest = 20,
        //    AcceptedRejectedBySSC = 21,
        //    PreapprovalStatus = 22,
        //    ConnectionStatus = 23,
        //    PreapprovalDocuments = 24,
        //    ConnectionDocuments = 25,
        //    STCSubmission = 26,
        //    InvoiceSent = 27,
        //    SubmittedForSTC = 28,
        //    FailedForSTC = 29,
        //    Signature = 30,
        //    SignatureApproved = 31,
        //    TermsAndConditions = 32,
        //    ModifiedStcValue = 33,
        //    ModifiedIsGst = 34,
        //    FSAStcSubmission = 35,
        //    PanelUpdated = 36,
        //    PanelAdded = 37,
        //    PanelRemoved = 38,
        //    SPVInstallationVerified = 39,
        //    SPVInstallationVerifiedSuccess = 40,
        //    SPVInstallationAlreadyDoneButFailed = 41,
        //    AllReadyInstallationVerified = 43,
        //    SerialNumberNotValidForVerify = 44,
        //    ReleaseSPV = 45,
        //    ResetSPV = 46,
        //    DoProductVerificationAgain = 47,
        //    LockSerialNumber = 48,
        //    UnLockSerialNumber = 49,
        //    DownloadDocument = 50,
        //    DownloadPhoto = 51,
        //    InstallationVerificationAlreadyDone = 52,
        //    SentToXero = 53,
        //    RemovedSPVAfterXMLVerification = 54,
        //    SyncWithXero = 55,
        //    RemoveInvoiceFromGreenbot = 56,
        //    MarkInvoice = 57,
        //    UnmarkInvoice = 58,
        //    SendToSTCInovice = 59,
        //    SendCreditNotesToXero = 60,
        //    PartiallySettledWithXero = 61,
        //    AddedSerialNumber = 62,
        //    RemoveSerialNumber = 63,
        //    AddNotesWithMention = 64,
        //    AddNote = 65,
        //    SigntureOTPRequest=66,
        //    ChangeVisitStatus=67

        //}

	}
}

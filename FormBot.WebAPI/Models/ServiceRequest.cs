using FormBot.Entity.Job;
using FormBot.Entity.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class LoginRequest
    {
        public string Password { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string Type { get; set; }
        public string UserID { get; set; }
        public string DeviceInfo { get; set; }
        public string ApiToken { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Username { get; set; }
    }

    public class ServiceRequest
    {
        public string UserID { get; set; }
        public string UserTypeID { get; set; }
        public string CompanyID { get; set; }
        public string JobId { get; set; }
        public string ApiToken { get; set; }
    }

    public class DocumentRequest
    {
        public string JobID { get; set; }

        public string DocumentId { get; set; }
        public string AlreadyAddedDocumentid { get; set; }

        public string UserID { get; set; }
        public string ApiToken { get; set; }

        public bool IsClassic { get; set; }
    }
    public class CaptureSignRequest
    {
        public string UserID { get; set; }
        public string ApiToken { get; set; }
        public int JobDocId { get; set; }
        public string FieldName { get; set; }
        public bool IsUpdate { get; set; }
        public string signString { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string mobileNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class ImageRequest
    {
        public string Image { get; set; }
        public string JobID { get; set; }
        public string Status { get; set; }
        public string Filename { get; set; }
        public string UserID { get; set; }
        public string ApiToken { get; set; }

        public string VisitCheckListItemId { get; set; }
        public string JobSchedulingId { get; set; }
        public bool IsClassic { get; set; }

        public string CaptureUploadImagePDFName { get; set; }

        public string PDFLocationId { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string Altitude { get; set; }
        public string Accuracy { get; set; }

        public string ImageTakenDate { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }

        public int VisitChecklistPhotoId { get; set; }
    }

    public class JobRequest
    {
        public string JobID { get; set; }
        public string UserID { get; set; }
        public string ApiToken { get; set; }
        public string JobSchedulingID { get; set; }

        public string VisitCheckListItemId { get; set; }
        public bool IsClassic { get; set; }
    }

    public class ScheduleStatusRequest
    {
        public string JobID { get; set; }
        public string Status { get; set; }
        public string JobSchedulingID { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
    }

    public class JobPhotoRequest
    {
        public string JobID { get; set; }
        public string UserID { get; set; }
        public string ImageName { get; set; }
        public string ApiToken { get; set; }
        public int IsReference { get; set; }
        public string VisitCheckListItemId { get; set; }
        public string JobSchedulingId { get; set; }
        public bool IsClassic { get; set; }
    }

    public class VisitCheckListItem
    {
        public string VisitCheckListItemId { get; set; }
        public string JobId { get; set; }
        public string SerialNumFileName { get; set; }
        public string CheckListSerialNumbers { get; set; }
        public string UserID { get; set; }
        public string ImageName { get; set; }
        public string ApiToken { get; set; }
        public bool IsClassic { get; set; }
        public bool IsCreateFile { get; set; }
        public List<JobSerialNumberRequestFromAutoSyncMethod> lstJobSerialNumbers { get; set; }
    }

    public class JobPhotoDeleteRequest
    {
        public string JobID { get; set; }
        public string UserID { get; set; }
        public string FolderName { get; set; }
        public string ApiToken { get; set; }

        public string VisitCheckListItemId { get; set; }
        public string JobSchedulingId { get; set; }
        public bool IsClassic { get; set; }
    }

    public class JobNotesRequest
    {
        public string JobID { get; set; }
        public string UserID { get; set; }
        public string Notes { get; set; }
        public string ApiToken { get; set; }
        public string JobSchedulingID { get; set; }
    }

    public class JobSerialNumberRequest
    {
        public string JobID { get; set; }
        public string SerialNumber { get; set; }
        public string IsAll { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
    }

    public class JobSerialNumberRequestFromAutoSyncMethod
    {
        public int Id { get; set; }
        public int VisitChecklistItemId { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public int JobId { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsDeleted { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }
        public string Accuracy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class JobStageRequest
    {
        public string JobID { get; set; }
        public string Stage { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
    }
    public class AutoRequestApprovedRequest
    {
        public bool IsApproved { get; set; }
        public bool IsGet { get; set; }
        public string ApiToken { get; set; }
        public int UserID { get; set; }
    }
    public class SPVSerialNumberProductVerificationRequest
    {
        public List<SPVProductVerification> lstSerialNumber { get; set; }
        public string ApiToken { get; set; }
        public int UserID { get; set; }
    }

    public class ForgotPasswordResponse
    {
        public string ForgotPasswordStatus { get; set; }
    }

    public class SignatureRequest
    {
        public string JobID { get; set; }
        public string Signature { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string SignatureDate { get; set; }
        public string Image { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
    }

    public class UpdateDocumentRequest
    {
        public string DocumentId { get; set; }
        public string JobId { get; set; }
        public string JsonData { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
        //public string lstCaptureUserSign { get; set; }
        //public List<PdfItems> pdfItems { get; set; }
        //public string Firstname { get; set; }
        //public string Lastname { get; set; }
        //public string mobileNumber { get; set; }
        //public string Fieldname { get; set; }
        //public string SignString { get; set; }
        //public string Email { get; set; }

    }

    public class ApiVersion
    {
        public string AndroidVersion { get; set; }
        public string IOSVersion { get; set; }
        public bool IsCompulsory { get; set; }
        public int LowQuality { get; set; }
        public int MediumQuality { get; set; }
        public int HighQuality { get; set; }
        public string ScandITAndroidKey { get; set; }
        public string ScandITIOSKey { get; set; }
    }

    public class SolarCompanyRequest
    {
        public string ApiToken { get; set; }
        public string UserID { get; set; }
    }

    public class SolarCompanyStatusSetBySE
    {
        public string ApiToken { get; set; }
        public string UserID { get; set; }
        public string SolarCompanyId { get; set; }
        public bool SolarCompanyStatus { get; set; }
        public string ElectricianName { get; set; }
    }

    public class VisitSignatureRequest
    {
        public string CheckListItemId { get; set; }
        public string JobSchedulingId { get; set; }
        public string JobId { get; set; }
        public string Path { get; set; }
        public string SignatureTypeId { get; set; }
        public string UserID { get; set; }

        public string Image { get; set; }

        public string ApiToken { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string SignatureDate { get; set; }
        public string VisitCheckListItemId { get; set; }
        public string IsCompleted { get; set; }
    }


    public class ErrorLog
    {
        public string Error { get; set; }

        public string UserID { get; set; }

        public string DeviceToken { get; set; }

        public string ApiToken { get; set; }

        public bool bIsSyncDone { get; set; }

        public DateTime DateTime { get; set; }
    }

    public class UpdateVisitStatusRequest
    {
        public string JobSchedulingID { get; set; }
        public string VisitStatus { get; set; }
        public string ApiToken { get; set; }
        public string UserID { get; set; }
        public string CompletedDate { get; set; }
    }

    public class GetVisitSignature
    {
        public string JobSchedulingID { get; set; }

        public string ApiToken { get; set; }
        public string UserID { get; set; }

        public string VisitCheckListItemId { get; set; }

    }

    public class UserSignature
    {
        public string UserID { get; set; }
        public string Image { get; set; }
        public string ApiToken { get; set; }
    }
    public class LogoutDevices
    {
        public string UserID { get; set; }
        public string UserDeviceId { get; set; }
        public string ApiToken { get; set; }
        public bool IsLogOutAll { get; set; }
    }
    public class SCAndGlobalLevalSPVConfiguration
    {
        public string UserId { get; set; }
        public string ApiToken { get; set; }
       public string SolarCompanyId { get; set; }
        public string JobId { get; set; }
        
    }
    public class VisitchecklistPhotoIds
    {
        public string UserId { get; set; }
        public string ApiToken { get; set; }
        public List<int> VisitChecklistPhotoIds { get; set; }

    }
}
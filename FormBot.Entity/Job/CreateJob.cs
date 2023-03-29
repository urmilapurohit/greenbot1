using FormBot.Entity.Documents;
using FormBot.Entity.Job;
using FormBot.Entity.Settings;
using FormBot.Entity.SolarElectrician;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class CreateJob
    {
        public BasicDetails BasicDetails { get; set; }
        public JobOwnerDetails JobOwnerDetails { get; set; }
        public JobElectricians JobElectricians { get; set; }
        public JobInstallationDetails JobInstallationDetails { get; set; }
        public JobInverterDetails JobInverterDetails { get; set; }
        public JobNotes JobNotes { get; set; }
        public JobPanelDetails JobPanelDetails { get; set; }
        public JobBatteryManufacturer JobBatteryManufacturer { get; set; }

        public List<JobSerialNumbers> JobSerialNumbers { get; set; }

        public List<JobInverterSerialNumber> JobInverterSerialNumbers { get; set; }
        public JobSTCDetails JobSTCDetails { get; set; }
        public JobSystemDetails JobSystemDetails { get; set; }
        public JobInstallerDetails JobInstallerDetails { get; set; }
        public InstallerDesignerView InstallerView { get; set; }
        public InstallerDesignerView DesignerView { get; set; }

        public InstallerDesignerView InstallerDesignerView { get; set; }

        public List<JobPanelDetails> lstJobPanelDetails { get; set; }
        public List<JobInverterDetails> lstJobInverterDetails { get; set; }
        public List<JobBatteryManufacturer> lstJobBatteryManufacturer { get; set; }
        public DocObject docObject { get; set; }

        public List<STCJobHistory> lstSTCJobHistory { get; set; }
        [NotMapped]
        public List<PVModules> lstPVModules { get; set; }
        [NotMapped]
        public List<Inverters> lstInverters { get; set; }
        [NotMapped]
        public List<BatteryStorage> lstBatteryStorage { get; set; }

        public string Guid { get; set; }
        public string Signature { get; set; }
        public string panelXml { get; set; }
        public string inverterXml { get; set; }

        [NotMapped]
        public string FileName { get; set; }

        [NotMapped]
        public List<string> FileNamesCreate { get; set; }

        [NotMapped]
        public List<UserDocument> lstUserDocument { get; set; }

        [NotMapped]
        public List<InstallerDeviceAudit> InstallerDeviceAudits { get; set; }

        public int JobID { get; set; }

        public int UserType { get; set; }

        public string VendorJobId { get; set; }

        public bool IsVendorApi { get; set; }

        //public string InstallerCECAccreditationNumber { get; set; }

        //public string DesignerCECAccreditationNumber { get; set; }

        //public string ElectricianCECAccreditationNumber { get; set; }

        public int STCStatus { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        ////public List<JobScheduling> lstJobSchedule { get; set; }

        public List<JobScheduling> lstJobSchedule { get; set; }

        [NotMapped]
        public List<UserDocument> lstSerialDocument { get; set; }
        public List<UserDocument> lstJobOtherDocument { get; set; }
        public string Header { get; set; }

        public string OwnerSignature { get; set; }
        public string OldElectricianSignature { get; set; }

        public List<UserDocument> lstEmailDocuments { get; set; }

        public string ProfileSignatureID { get; set; }
        public string ProfileSignature { get; set; }

        public JobScheduling JobScheduling { get; set; }

        public string InstallerSignature { get; set; }
        public string DesignerSignature { get; set; }
        public string ElectricianSignature { get; set; }
        //public bool isClassic { get; set; }

        public List<FormBot.Entity.Documents.DocumentSteps> lstPreApprovalDocumentSteps { get; set; }
        public List<FormBot.Entity.Documents.DocumentSteps> lstConnectionDocumentSteps { get; set; }

        public STCDetailsModel STCDetailsModel { get; set; }
        public chkPhotos chkPhotosAll { get; set; }

        public JobDocuments JobDocumnts { get; set; }

        public bool IsInstallerSignUpload { get; set; }
        public bool IsDesigerSignUpload { get; set; }
        public bool IsElectricianSignUpload { get; set; }
        public bool IsOwnerSignUpload { get; set; }

        public List<CommonSerialNumber> CommonSerialNumbers { get; set; }
        public List<CommonInverterSerialNumber> CommonInverterSerialNumbers { get; set; }
        public List<CustomField> lstCustomField { get; set; }

        public string Type { get; set; }

        public int? liLength { get; set; }

        public bool isTabular { get; set; }

        public List<CheckList.CheckListItem> lstCheckListItem { get; set; }

        public JobCustomDetails JobCustomDetails { get; set; }

        public List<CustomDetail> lstCustomDetails { get; set; }

        public List<Panel> panel { get; set; }

        public List<Inverter> inverter { get; set; }

        public List<JobNotes> lstJobNotes { get; set; }

        public string NewlyAddedSerialNumber { get; set; }
        public string DeletedSerialNumber { get; set; }
        public JsonResult DocumentJson { get; set; }

        public List<CommonJobsWithSameInstallDateAndInstaller> CommonJobsWithSameInstallDateAndInstaller { get; set; }

        public List<CommonJobsWithSameInstallDateAndInstaller> CommonJobsWithSameInstallDateAndInstallerAndFailedStcStatus { get; set; }
        public List<CommonJobsWithSameInstallationAddress> CommonJobsWithSameInstallationAddress { get; set; }
        [NotMapped]
        public bool GlobalisAllowedSPV { get; set; }

        public DataTable ElectricianData { get; set; }

        public string SMSOrMail { get; set; }

        public JsonResult SerialNumberwithPhotosAvaibilityList { get; set; }
        public RetailerAutoSetting RetailerAutoSetting { get; set; }
        public JobRetailerSetting JobRetailerSetting { get; set; }

        public DataTable RetailerAutoSettingForSignature { get; set; }
        [NotMapped]
        public string UserDocuments { get; set; }

        public string OwnerSignatureSelfie { get; set; }
        public string DesignerSignatureSelfie { get; set; }
        public string ElectritionSignatureSelfie { get; set; }
        public string InstallerSignatureSelfie { get; set; }
        public int JobUserComplianceID { get; set; }
        public DocumentsAndPhotosCount DocumentsAndPhotosCount { get; set;}
        public int IsPanelInvoice { get; set; }
        public int IsElectricityBill{ get; set; }
        public JobStage STCJobStages { get; set; }
        public int STCSettlementTerm { get; set; }
        public bool IsPartialValidForSTCInvoice { get; set; }
        public bool IsInvoiced { get; set; }
    }

    public class DocumentsAndPhotosCount
    {
        public int DocumentsCount { get; set; }
        public int PhotosCount { get; set; }
        public int PhotosTotalCount { get; set; }
        public int SerialNumberCount { get; set; }
        public int SerialNumberTotalCount { get; set; }
    }
    public class chkPhotos
    {
        public int jobId { get; set; }
        public List<JobSchedulingPhotos> chkJobPhotos { get; set; }
        public List<Photo> ReferencePhotos { get; set; }
        public List<Photo> InstallationPhotos { get; set; }
        public List<Photo> SerialPhotos { get; set; }
    }

    public class JobSchedulingPhotos
    {
        public string UniqueVisitID { get; set; }
        public int jobSchedulingId { get; set; }
        public int jobId { get; set; }
        public List<Photo> chkSerial { get; set; }

        public string ItemName { get; set; }
        public List<VisitCheckListItems> lstVisitCheckListItem { get; set; }
        public List<Photo> chkPhotos { get; set; }
        public List<Photo> chkCapturePhoto { get; set; }
        public List<Photo> chkSignature { get; set; }
        public List<Photo> chkCustom { get; set; }
        public bool IsDefaultSubmission { get; set; }
        public bool IsDeleted { get; set; }
        public int serialNumTotalCount { get; set; }
        public int capturePhotoTotalCount { get; set; }
        public int signatureTotalCount { get; set; }
    }

    public class Photo
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string VisitCheckListPhotoId { get; set; }
        public string VisitSignatureId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CreatedDate { get; set; }
        public string IsUnderInstallationArea { get; set; } 
    }

    public class JobDocuments
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int DocumentId { get; set; }
    }
   
    public class VisitCheckListItems
    {
        public string VisitCheckListItemId { get; set; }
        public string FolderName { get; set; }
        public List<Photo> lstCheckListPhoto { get; set; }
        public int TotalCount { get; set; }
        public int VisitedCount { get; set; }
        public int CheckListClassTypeId { get; set; }
        public string PDFLocationId { get; set; }
        public string CaptureUploadImagePDFName { get; set; }
        public bool Isdeleted { get; set; }
        public int CheckListPhotoTypeId { get; set; }
    }

    public class CommonSerialNumber
    {
        public int JobId { get; set; }
        public string RefNumber { get; set; }
        public string SerialNumber { get; set; }
        public string CompanyName { get; set; }
        public string ResellerName { get; set; }
        public int SolarCompanyId { get; set; }
        public int ResellerId { get; set; }
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(JobId));
            }
        }
    }
    public class CommonInverterSerialNumber
    {
        public int JobId { get; set; }
        public string RefNumber { get; set; }
        public string SerialNumber { get; set; }
        public string CompanyName { get; set; }
        public string ResellerName { get; set; }
        public int SolarCompanyId { get; set; }
        public int ResellerId { get; set; }
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(JobId));
            }
        }
    }

    public class CustomField
    {
        public int VisitCheckListItemId { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public int JobCustomFieldId { get; set; }
    }

    public class CustomDetail
    {
        public int JobCustomFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public int SeparatorId { get; set; }

        //[Required(ErrorMessage = "VendorJobCustomFieldId is required.")]
        public string VendorJobCustomFieldId { get; set; }
    }

    public class Panel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int NoOfPanel { get; set; }
    }

    public class Inverter
    {
        public string Brand { get; set; }
        public string Series { get; set; }
        public string Model { get; set; }
    }

    public class PanelModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
    }

    public class SystemBrandModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
    }

    public class JobListModel
    {
        public BasicDetailsVendorAPI BasicDetails { get; set; }
        public JobOwnerDetailsVendorAPI JobOwnerDetails { get; set; }
        public JobElectriciansVendorAPI JobElectricians { get; set; }
        public JobInstallationDetails JobInstallationDetails { get; set; }

        public JobSTCDetailsVendorAPI JobSTCDetails { get; set; }
        public JobSystemDetailsVendorAPI JobSystemDetails { get; set; }
        public JobInstallerDetails JobInstallerDetails { get; set; }
        public InstallerDesignerViewVendorAPI InstallerView { get; set; }
        public InstallerDesignerViewVendorAPI DesignerView { get; set; }

        public List<JobPanelDetails> lstJobPanelDetails { get; set; }
        public List<JobInverterDetails> lstJobInverterDetails { get; set; }
        public List<CustomDetail> lstCustomDetails { get; set; }

        public String STCStatus { get; set; }
    }
    //<Inverters><inverter><Brand>ABB Oy Power Conversion</Brand><Model>PRO-33.0-TL-OUTD-400</Model><Series>String Inverter</Series></inverter></Inverters>



    public class VendorCustomField
    {
        public string FieldName { get; set; }
        public string VendorJobCustomFieldId { get; set; }
    }

    public class VendorJobPhotoList
    {
        //public string VendorJobId { get; set; }
        public string ImageBase64 { get; set; }
        public int PhotoType { get; set; }
        public string Filename { get; set; }
        public string VendorJobPhotoId { get; set; }
        public bool IsDefault { get; set; }
    }

    //public class InstallerDesigner
    //{
    //    public string Text { get; set; }
    //    public string Value { get; set; }
    //    public string IsSystemUser { get; set; }
    //}

    public class ElectricianList
    {
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsCustomElectrician { get; set; }
        public int UserId { get; set; }
        public int SEStatus { get; set; }
    }


    public class JobSerialNumbers
    {
        public string SerialNumber { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsSPVInstallationVerified { get; set; }
    }
    public class CommonJobsWithSameInstallDateAndInstaller
    {
        public int JobID { get; set; }
        public string RefNumber { get; set; }
        public string CompanyName { get; set; }
        public string ResellerName { get; set; }
        public int SolarCompanyId { get; set; }
        public int ResellerId { get; set; }

        public string InstallerName { get; set; }
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(JobID));
            }
        }

     
    }
    public class CommonJobsWithSameInstallationAddress
    {
        public int JobID { get; set; }
        public string RefNumber { get; set; }
        public string CompanyName { get; set; }
        public string ResellerName { get; set; }
        public int SolarCompanyId { get; set; }
        public int ResellerId { get; set; }
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(JobID));
            }
        }
        public string Status { get; set; }
}
    public class XMLProduct { 
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ResponsibleSupplier { get; set; }
    }

    public class XMLModel {
        public string ModelNumber { get; set; }
        public string Manufacturer { get; set; }
        public string wattage { get; set; }
    }

    public class JobInverterSerialNumber
    {
        public string InverterSerialNumber { get; set; }
    }

}

using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEECCheckList
{
    public class VEECCheckListItem
    {

        [DisplayName("CheckList Item:")]
        public int? VEECCheckListItemId { get; set; }

        [DisplayName("CheckList Item:")]
        public int? CheckListItemSelectedId { get; set; }

        [DisplayName("CheckList Template:")]
        public int VEECCheckListTemplateId { get; set; }

        [DisplayName("Class Type:")]
        [Required(ErrorMessage = "Class Type is required.")]
        public int VEECCheckListClassTypeId { get; set; }

        [DisplayName("Item Name:")]
        [Required(ErrorMessage = "Item Name is required.")]
        public string ItemName { get; set; }

        [DisplayName("Folder Name:")]
        [Required(ErrorMessage = "Folder Name is required.")]
        public string FolderName { get; set; }

        [Required(ErrorMessage = "Total Number is required.")]
        [DisplayName("Total Number:")]
        public int TotalNumber { get; set; }

        [DisplayName("Same As Total Panel Amount:")]
        public bool IsSameAsTotalPanelAmount { get; set; }

        public bool IsAtLeastOne { get; set; }

        public int NumberOptions { get; set; }

        [DisplayName("Is Custom Serial Number Field:")]
        public bool IsCustomSerialNumField { get; set; }

        [Required(ErrorMessage = "Field Name is required.")]
        [DisplayName("Map Captured Serials:")]
        public int? JobFieldId { get; set; }

        //[Required(ErrorMessage = "Field Name is required.")]
        [DisplayName("Serial Field Name:")]
        public string SerialFieldName { get; set; }

        [Required(ErrorMessage = "Custom Field is required.")]
        [DisplayName("Custom Field:")]
        public int? CustomFieldId { get; set; }

        [DisplayName("Separator:")]
        [Required(ErrorMessage = "Separator is required.")]
        public int? SeparatorId { get; set; }

        [DisplayName("Scanned Title Header:")]
        //[Required(ErrorMessage = "Title is required.")]
        public string SerialNumTitle { get; set; }

        [DisplayName("Save a copy to .TXT?:")]
        public bool IsSaveCopyofSerialNum { get; set; }

        [DisplayName("Serial Number File Name:")]
        [Required(ErrorMessage = "File Name is required.")]
        public string SerialNumFileName { get; set; }

        [DisplayName("Is Owner Signature:")]
        public bool IsOwnerSignature { get; set; }

        [DisplayName("Is Installer Signature:")]
        public bool IsInstallerSignature { get; set; }

        [DisplayName("Is Designer Signature:")]
        public bool IsDesignerSignature { get; set; }

        [DisplayName("Is Electrician Signature:")]
        public bool IsElectricianSignature { get; set; }

        [DisplayName("Is Other Signature:")]
        public bool IsOtherSignature { get; set; }

        [DisplayName("Other Signature Name:")]
        [Required(ErrorMessage = "Signature Name is required.")]
        public string OtherSignName { get; set; }

        [DisplayName("Other Signature Label:")]
        [Required(ErrorMessage = "Signature Label is required.")]
        public string OtherSignLabel { get; set; }

        [DisplayName("PDF Name:")]
        [Required(ErrorMessage = "PDF Name is required.")]
        public string CaptureUploadImagePDFName { get; set; }

        [DisplayName("PDF Location:")]
        [Required(ErrorMessage = "PDF Location is required.")]
        public int? PDFLocationId { get; set; }

        [DisplayName("Is Link To Document:")]
        public bool IsLinkToDocument { get; set; }

        [DisplayName("Linked Document:")]
        [Required(ErrorMessage = "Document is required.")]
        public int LinkedDocumentId { get; set; }

        [DisplayName("Is Completed Custom CheckList Item:")]
        public bool IsCompletedCustomCheckListItem { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public bool IsCompleted { get; set; }

        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECCheckListItemId));
            }
        }

        public string TemplateId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECCheckListTemplateId));
            }
        }

        public int OrderNumber { get; set; }

        public bool isSetFromSetting { get; set; }

        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }

        public bool IsDefaultTemplateItem { get; set; }

        public int VEECVisitCheckListItemId { get; set; }

        public int? JobSchedulingId { get; set; }
        public int JobId { get; set; }

        public int VisitedCount { get; set; }

        public Int64 TempJobSchedulingId { get; set; }

        [Required(ErrorMessage = "Separator is required.")]
        public int SerialNumberSeparator { get; set; }

        [DisplayName("Photo Quality:")]
        [Required(ErrorMessage = "Photo Quality is required.")]
        public int PhotoQualityId { get; set; }
    }
}

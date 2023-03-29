using FormBot.Entity.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class StcComplianceCheck
    {
        public int STCJobComplianceID { get; set; }

        public int STCJobDetailsID { get; set; }

        public string IsNameCorrect { get; set; }
        public string IsOrganisationNameCorrect { get; set; }
        public string OrganisationName { get; set; }
        public string IsAddressCorrect { get; set; }

        public string IsInstallerSignatureVisible { get; set; }

        public string IsDesignerSignatureVisible { get; set; }

        public string IsElectriciandetailsvisible { get; set; }

        public string IsSerialNumbersMatch { get; set; }

        public string IsSTCAmountMatch { get; set; }

        public string IsOwnerDetailsMatch { get; set; }

        public string IsDescriptionCES { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public int? NoOfPanel { get; set; }

        public string Notes { get; set; }

        public DateTime? CallDateTime { get; set; }

        ////public string FileName { get; set; }

        public int? StcDocumentId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public byte Status { get; set; }

        public bool IsRequestedAuthorize { get; set; }

        public string CallMadeBy { get; set; }
        public string CallMadeUserId { get; set; }

        public DateTime STCLastUpdatedDate { get; set; }  
        public string CallDate
        {
            get
            {
                return Convert.ToDateTime(CallDateTime).ToString("yyyy/MM/dd");
            }

        }

        public string CallTime
        {
            get
            {
                return Convert.ToDateTime(CallDateTime).ToString("HH:mm");
            }
        }

        public string Guid { get; set; }
         public string FileName { get; set; }
        public List<string> FileNamesCreate { get; set; }
        public List<GeneralClass> lstUploadFile { get; set; }

        public List<GeneralClass> lstSubmission { get; set; }
        public List<GeneralClass> lstStcStatus { get; set; }
        public List<string> lstStcDocuments { get; set; }
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string FileUploadJson { get; set; }

        public string Description { get; set; }

        public int? InstallationType { get; set; }
        public int? ConnectionType { get; set; }
        public int? MountingType { get; set; }
        public String AdditionalInformation { get; set; }

        public string ExplanatoryNotes { get; set; }

        public int JobId { get; set; }
        public byte JobType { get; set; }
        public List<UserDocument> lstUserDocument { get; set; }
        public int UserTypeId { get; set; }
    }

    public class Document
    {
        public string FileName { get; set; }
        public string MineType { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class JobSTCDetails
    {
        public int STCDetailsID { get; set; }
        public int JobId { get; set; }

        [Display(Name = "Type of connection to the electricity grid:")]
        public string TypeOfConnection { get; set; }

        [Display(Name = "System mounting type:")]
        public string SystemMountingType { get; set; }

        ////[Display(Name = "Are you installing a complete unit (adding capacity to an existing system is not considered a complete unit)?:")]
        public string InstallingCompleteUnit { get; set; }

        [Display(Name = "If this system is additional capacity to an existing system please provide detailed information on the position of the new panels and inverter (if applicable). System upgrades without a note explaining new panel locations will be failed by the Clean Energy Regulator:")]
        ////[StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required.")]
        public string AdditionalCapacityNotes { get; set; }

        [Display(Name = "For what period would you like to create RECs:")]
        public string DeemingPeriod { get; set; }

        [Display(Name = "Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?:")]
        public string CertificateCreated { get; set; }

        [Display(Name = "Failed accreditation code:")]
        public string FailedAccreditationCode { get; set; }

        [Display(Name = "Explanatory notes for re-creating certificates previously failed:")]
        public string FailedReason { get; set; }

        ////[Display(Name = "Explanatory notes for re-creating certificates previously failed:")]
        ////public string ExplanatoryNotes { get; set; }

        public string CECAccreditationStatement { get; set; }
        public string GovernmentSitingApproval { get; set; }
        public string ElectricalSafetyDocumentation { get; set; }
        public string AustralianNewZealandStandardStatement { get; set; }
        public string StandAloneGridSelected { get; set; }

        [Display(Name = "Is there more than one SGU at this address?:")]
        public string MultipleSGUAddress { get; set; }

        [Display(Name = "Where is the location of this system in relation to the existing system?:")]
        public string Location { get; set; }

        [Display(Name = "Where is this system located?:")]
        public string SGUSystemLocated { get; set; }

        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

         [Display(Name = "Is the volumetric capacity of this installation greater than 700L (Y/N)?:")]
                  public string VolumetricCapacity { get; set; }

         [Display(Name = "Statutory declarations sent (Y/N) (has to be yes to submit from Rec Registry logic)?:")]
                 public string StatutoryDeclarations { get; set; }

         [Display(Name = "Is your water heater second hand  (Y/N) ( has to be No to successfully submit, else will fail)?:")]  
        public string SecondhandWaterHeater { get; set; }

        [Display(Name = "What is the location of the new system in relation to the existing system?")]  
         public string AdditionalLocationInformation { get; set; }

        [AllowHtml]
        public string AdditionalSystemInformation { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        [Display(Name = "Is the battery system part of an aggregated control (Y/N)?:")]
        public string batterySystemPartOfAnAggregatedControl { get; set; }

        [Display(Name = "Has the installer changed default manufacturer setting of the battery storage system (Y/N)?:")]
        public string changedSettingOfBatteryStorageSystem { get; set; }
        public string PreviousSystemMountingType { get; set; }
        public string PreviousConnectionType { get; set; }
        public string PreviousMultipleSGUAddress { get; set; }
        public string PreviousDeemingPeriod { get; set; }
        public string PreviousCertificateCreated { get; set; }
        public string PreviousFailedAccreditationCode { get; set; }
        public string PreviousFailedReason { get; set; }
        public string PreviousLocation { get; set; }
        public string PreviousAdditionalLocationInformation { get; set; }
        public string PreviousVolumetricCapacity { get; set; }
        public string PreviousStatutoryDeclarations { get; set; }
        public string PreviousSecondhandWaterHeater { get; set; }
        public string PreviousLatitude { get; set; }
        public string PreviousLongitude { get; set; }
        [AllowHtml]
        public string PreviousAdditionalSystemInformation { get; set; }
    }

    public class StcObject
    {
        public JobInstallationDetails JobInstallationDetails { get; set; }
        public JobSTCDetails JobSTCDetails { get; set; }
        public JobSystemDetails JobSystemDetails { get; set; }
    }
    public class JobSTCDetailsVendorAPI
    {
        public string TypeOfConnection { get; set; }
        public string SystemMountingType { get; set; }
        public string InstallingCompleteUnit { get; set; }
        public string AdditionalCapacityNotes { get; set; }
        public string DeemingPeriod { get; set; }
        public string CertificateCreated { get; set; }
        public string FailedAccreditationCode { get; set; }
        public string FailedReason { get; set; }
        public string MultipleSGUAddress { get; set; }
        public string Location { get; set; }
        public string SGUSystemLocated { get; set; }
        public string VolumetricCapacity { get; set; }
        public string StatutoryDeclarations { get; set; }
        public string SecondhandWaterHeater { get; set; }
        public string AdditionalLocationInformation { get; set; }
        public string AdditionalSystemInformation { get; set; }
       
    }
}

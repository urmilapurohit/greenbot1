using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECDetail
    {
        public int VEECId { get; set; }

        [Display(Name = "Schedule Activity Type:")]
        public int ScheduleActivityType { get; set; }

        [Display(Name = "Reference Number:")]        
        //[Required(ErrorMessage = "Reference number is required.")]        
        public string RefNumber { get; set; }

        public string RefNumber_Prefix { get; set; }

        [Required(ErrorMessage = "Reference number is required.")]      
        public string RefNumber_Suffix { get; set; }

        [Display(Name = "Title:")]
        [Required(ErrorMessage = "Title is required.")]        
        public string Title { get; set; }

        [Display(Name = "Schedule Activity Premises:")]
        //[Required(ErrorMessage = "Schedule Activity Premises is required.")]        
        public string ScheduledActivityPremises { get; set; }

        [Display(Name = "Description:")]
        [Required(ErrorMessage = "Description is required.")]        
        public string Description { get; set; }

        [Display(Name = "Commencement Date:")]
        [Required(ErrorMessage = "Commencement Date is required.")]        
        public DateTime CommencementDate { get; set; }

        public string CommencementDateStr
        {
            get
            {
                return CommencementDate.ToString("dd/MM/yyyy");
            }
        }

        [Display(Name = "Activity Date:")]
        [Required(ErrorMessage = "Activity Date is required.")]
        public DateTime ActivityDate { get; set; }

            public string ActivityDateStr
        {
            get
            {
                return ActivityDate.ToString("dd/MM/yyyy");
            }
        }

        public int SolarCompanyId { get; set; }

        public int ResellerId { get; set; }

        public int? VEECInstallerId { get; set; }

        public string VEECInstallerSignature { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
		
		 public int? ScoIDVEEC { get; set; }

        [Display(Name = "Lighting Design Method")]
        [Required(ErrorMessage = "Lighting Design Method is Required")]
        public int LightingDesignMethodId { get; set; }

        [Display(Name = "Contractual Arrangement")]
        [Required(ErrorMessage = "Contractual Arrangement is Required")]
        public int ContractualArrangementId { get; set; }

        [Display(Name = "Contractual Arrangement")]
        [Required(ErrorMessage = "Contractual Arrangement is Required")]
        public string ContractualDetails { get; set; }

        [Display(Name = "Light Level Verification")]
        [Required(ErrorMessage = "Light Level Verification is Required")]
        public int LightLevelVerificationId { get; set; }

        [Display(Name = "Qualification Of Light Level Verifier")]
        [Required(ErrorMessage = "Qualification Of Light Level Verifier is Required")]
        public int? QualificationOfLightLevelVerifierId { get; set; }

        [Display(Name = "Qualification Of Lighting Designer")]
        [Required(ErrorMessage = "Qualification Of Lighting Designer is Required")]
        public int? QualificationsOfLightingDesignerId { get; set; }

        [Display(Name = "Verifier Qualification Detail")]
        [Required(ErrorMessage = "Verifier Qualification Detail is Required")]
        public string VerifierQualificationDetails { get; set; }

        //[Display(Name = "Form Of Benefit provided to Energy Consumer")]
        //[Required(ErrorMessage = "Form Of Benefit provided to Energy Consumer is required")]
        //public string BenefitEnergyConsumer1680 { get; set; }

        [Display(Name = "Unrecognised Address Justification")]
        //[Required(ErrorMessage = "Unrecognised Address Justification is required")]
        public string UnrecognisedAddressJustification { get; set; }

        [Display(Name = "Internal Duplicate Justification")]
        //[Required(ErrorMessage = "Internal Duplicate Justification is required")]
        public string InternalDuplicateJustification { get; set; }

        [Display(Name = "External Duplication Justification")]
        //[Required(ErrorMessage = "External Duplication Justification is required")]
        public string ExternalDuplicationJustification { get; set; }

        [Display(Name = "Designer Qualification Details")]
        [Required(ErrorMessage = "Designer Qualification Details is required")]
        public string DesignerQualificationDetails { get; set; }
        
        public bool UpgradeManagerDeclartionChkbox1 { get; set; }

        public bool UpgradeManagerDeclartionChkbox2 { get; set; }

        public bool UpgradeManagerDeclartionChkbox3 { get; set; }

    }
}

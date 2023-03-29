using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class BasicDetailsVEEC
    {
        public int VeecID { get; set; }

        [Display(Name = "Reference Number:")]
        [Required(ErrorMessage = "Reference number is required.")]
        public string RefNumber { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title:")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description:")]
        public string Description { get; set; }

        [Display(Name = "Job Number:")]
        [Required(ErrorMessage = "Job number is required.")]
        public string VeecNumber { get; set; }

        [Display(Name = "Job Stage:")]
        [Required(ErrorMessage = "Job stage is required.")]
        public int VeecStage { get; set; }

        //[Display(Name = "Job Type:")]
        //[Required(ErrorMessage = "Job type is required.")]
        //public int JobType { get; set; }

        [Display(Name = "Installer:")]
        public int? InstallerID { get; set; }

        [Display(Name = "Designer:")]
        public int? DesignerID { get; set; }

        [Display(Name = "Installation Date:")]
        ////[Required(ErrorMessage = "Installation date is required.")]
        public DateTime? InstallationDate { get; set; }

        [Display(Name = "Installation Date:")]
        ////[Required(ErrorMessage = "Installation date is required.")]
        public string strInstallationDate { get; set; }

        public string strInstallationDateTemp { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        [Display(Name = "Priority:")]
        public int Priority { get; set; }
        public int VeecElectricianID { get; set; }
        public string CompanyName { get; set; }
        public string ShortCompanyName { get; set; }
        public int CompanyCounter { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        //[Required(ErrorMessage = "Solar-Sub Contractor is required.")]
        public int UserTypeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserID { get; set; }

        public int? SSCID { get; set; }
        public int? ScoIDVEEC { get; set; }

        [Required(ErrorMessage = "Notes is required.")]
        public string Notes { get; set; }

        [Display(Name = "Sold By:")]
        public string SoldBy { get; set; }

        public string CurrentVeecStage { get; set; }
        public string PreviousJobStage { get; set; }
        public string PreviousPriority { get; set; }
        public string CurrentPriority { get; set; }
        public int? PreviousSSCID { get; set; }
        public string SSCName { get; set; }

        [Display(Name = "Sold By Date:")]
        ////[Required(ErrorMessage = "Installation date is required.")]
        public DateTime? SoldByDate { get; set; }

        [Display(Name = "Sold By Date:")]
        ////[Required(ErrorMessage = "Installation date is required.")]
        public string strSoldByDate { get; set; }
        public string VeecSoldBy { get; set; }
        public string VeecSoldByText { get; set; }
        public int SolarCompanyId { get; set; }

        // Job Print detail
        public string StageName { get; set; }
        public string installerName { get; set; }
        public string designerName { get; set; }
        public string electricianName { get; set; }
        public string SolarCompanyPhone { get; set; }
        public string SolarCompanyMobile { get; set; }
        public string CompanyUserName { get; set; }
        public string RemainingAmount { get; set; }

        [Display(Name = "Claiming GST:")]
        public bool IsGst { get; set; }

        [Display(Name = "GSTDocument:")]
        public string GSTDocument { get; set; }

        public string MimeType { get; set; }

        public string CompanyABN { get; set; }
        public bool IsRegisteredWithGST { get; set; }
        public int? IsGSTSetByAdminUser { get; set; }
        public int STCInvoiceCount { get; set; }

        public string InstallerSignature { get; set; }
        public string DesignerSignature { get; set; }
        public string ElectricianSignature { get; set; }
        public string OwnerSignature { get; set; }
        public bool IsClassic { get; set; }

        public string ContactName { get; set; }
        public string Email { get; set; }

        //public List<KeyValuePair<string, string>> lstCustomFields { get; set; }

        public string Reseller { get; set; }

        public string ResellerABN { get; set; }

        public string ResellerName { get; set; }

        public string InstallerLatLong { get; set; }

        public string DesignerLatLong { get; set; }

        public string ElectricianLatLong { get; set; }

        public string OwnerLatLong { get; set; }
        public bool IsAllowTrade { get; set; }

        public string GstFileBase64 { get; set; }
        public bool IsWholeSaler { get; set; }

        public int ResellerId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FormBot.Helper;
using FormBot.Entity.Email;
using Newtonsoft.Json;


namespace FormBot.Entity
{
    public class JobInstallationDetails
    {
        [JsonIgnore]
        public int JobInstallationID { get; set; }
        [JsonIgnore]
        public int JobID { get; set; }

        [DisplayName("Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int? UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string StreetName { get; set; }

        [DisplayName("Town:")]
        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("NMI:")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Minimum 10 character required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        //[Required(ErrorMessage = "NMI is required.")]
        public string NMI { get; set; }

        [DisplayName("Installation property type:")]
        public string PropertyType { get; set; }

        [DisplayName("Installation property name:")]
        public string PropertyName { get; set; }

        [DisplayName("Single or multi-story:")]
        public string SingleMultipleStory { get; set; }

        [DisplayName("Are you installing new, replacing, extending or adding panels to a system?:")]
        public string InstallingNewPanel { get; set; }

        [DisplayName("Meter Number:")]
        public string MeterNumber { get; set; }

        [DisplayName("Phase Property:")]
        public string PhaseProperty { get; set; }

        [DisplayName("Electricity Provider:")]
        public int? ElectricityProviderID { get; set; }

        [DisplayName("Additional Notes:")]
        public string AdditionalNotes { get; set; }

        [DisplayName("Does the home owner have an existing system on the property?:")]
        public bool ExistingSystem { get; set; }

        [DisplayName("Existing System Size:")]
        public decimal? ExistingSystemSize { get; set; }

        [DisplayName("Number Of Panels:")]
        public int? NoOfPanels { get; set; }

        [DisplayName("System Location:")]
        public string SystemLocation { get; set; }

        [DisplayName("System Size:")]
        public decimal? SystemSize { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string PostalDeliveryNumber { get; set; }

        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Address:")]
        [Required(ErrorMessage = "Postal Address is required.")]
        public int PostalAddressID { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int TotalRecords { get; set; }

        public int AddressID { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        [JsonIgnore]
        public string AddressDisplay { get; set; }

        [DisplayName("What side facing roof are these panels located:")]
        public string Location { get; set; }

        [DisplayName("Distributor:")]
        public int? DistributorID { get; set; }

        [Display(Name = "Additional installation information:")]
        public string AdditionalInstallationInformation { get; set; }

        [JsonIgnore]
        public bool IsSameAsOwnerAddress { get; set; }
        [JsonIgnore]
        public string ElectricityProvider { get; set; }
        [JsonIgnore]
        public string DistributorName { get; set; }
        [JsonIgnore]
        public List<CustomDetail> lstCustomDetails { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public bool? IsInstallationAddressValid { get; set; }
        public string PreviousNMI { get; set; }
        public string oldInstallationAddress { get; set; }
        public string PreviousSingleMultipleStory{get;set;}
        public string PreviousInstallingNewPanel { get; set; }

        public string PreviousLocation { get; set; }
    }
}

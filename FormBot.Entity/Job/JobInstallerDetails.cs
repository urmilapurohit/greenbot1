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
    public class JobInstallerDetails
    {
        [JsonIgnore]
        public int JobInstallerId { get; set; }
        [JsonIgnore]
        public int JobID { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Surname:")]
        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; }

        [DisplayName("Phone:")]
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for phone.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for mobile.")]
        public string Mobile { get; set; }

        [DisplayName("Email:")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        [DisplayName("Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int? UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }

        [DisplayName("Town:")]
        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "Post Code is required.")]
        public string PostCode { get; set; }

        [DisplayName("Postal Address:")]
        [Required(ErrorMessage = "Postal Address is required.")]
        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        [Required(ErrorMessage = "Postal Delivery Type is required.")]
        public int PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }
        
        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int TotalRecords { get; set; }

        public int AddressID { get; set; }

        [DisplayName("Installer:")]
        public int ElectricianID { get; set; }

        [DisplayName("License Number:")]
        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }
        public int SWHInstallerDesignerId { get; set; }
        public string SESignature { get; set; }
        public int SolarCompanyId { get; set; }

        public int? AccreditedInstallerId { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }        
        public string AccreditationNumber { get; set; }
        public string InstallerStatus { get; set; }
        public DateTime? InstallerExpiryDate { get; set; }

        public bool IsSystemUser { get; set; }        
        public bool IsVisitScheduled { get; set; }
        public bool IsPVDUser { get; set; }
        public bool IsSWHUser { get; set; }
        public int UserId { get; set; }
        public int SEStatus { get; set; }
    }
}

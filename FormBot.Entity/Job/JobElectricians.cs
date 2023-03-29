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

namespace FormBot.Entity
{
    public class JobElectricians
    {
        public int JobElectricianID { get; set; }        

        public int SolarCompanyID { get; set; }

        [DisplayName("Company Name:")]
        public string CompanyName { get; set; }

        [DisplayName("First Name:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string LastName { get; set; }

        [DisplayName("Phone:")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for phone.")]
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for mobile.")]
        public string Mobile { get; set; }

        [DisplayName("Email:")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        public string BasicAddress { get; set; }
        [DisplayName("Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int? UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
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
        //[Required(ErrorMessage = "Postal Delivery Type is required.")]
        public int PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string PostalDeliveryNumber { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        public int AddressID { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [DisplayName("License Number:")]
        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }
        [DisplayName("Signature:")]
        public string Signature { get; set; }

        [Display(Name = "Installer:")]
        public int? InstallerID { get; set; }

        [Display(Name = "Electrician:")]
        public int? ElectricianID { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        public DateTime SignatureDate { get; set; }

        //for electrician dropdown
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsCustomElectrician { get; set; }
    }

    public class JobElectriciansVendorAPI
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string LicenseNumber { get; set; }
        public int AddressID { get; set; }
        public int? UnitTypeID { get; set; }
        public string UnitNumber { get; set; }
        public bool IsPostalAddress { get; set; }
        public int PostalAddressID { get; set; }
        public string PostalDeliveryNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public int? StreetTypeID { get; set; }
        public string Town { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
    }
}

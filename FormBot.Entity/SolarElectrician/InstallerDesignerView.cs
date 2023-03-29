using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FormBot.Entity.SolarElectrician
{
    [Serializable]
    [JsonObject]
    public class InstallerDesignerView
    {
        public int InstallerDesignerId { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        //[RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        //[RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string LastName { get; set; }


        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FindInstallerDesignerFirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string FindInstallerDesignerLastName { get; set; }

        [DisplayName("Unit Type:")]
        public int UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string StreetName { get; set; }

        [DisplayName("Street Type:")]
        public int StreetTypeID { get; set; }

        [DisplayName("Town/Suburb:")]
        [Required(ErrorMessage = "Town/Suburb is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("PostCode:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("CEC Accreditation Number:")]
        [Required(ErrorMessage = "CEC Accreditation Number is required.")]
        public string CECAccreditationNumber { get; set; }

        [DisplayName("CEC Designer Number:")]
        public string CECDesignerNumber { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public int? UserId { get; set; }

        public int SolarCompanyId { get; set; }

        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        public int PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string PostalDeliveryNumber { get; set; }

        public int AddressID { get; set; }

        [NotMapped]
        public string UnitTypeName { get; set; }

        [NotMapped]
        public string StreetTypeName { get; set; }

        [NotMapped]
        public string Code { get; set; }

        [NotMapped]
        public string PostalDeliveryType { get; set; }

        [NotMapped]
        public bool IsActiveDiv { get; set; }
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email.")]
        [DisplayName("Email:")]        
        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "SE Role is required.")]
        public int SEDesignRoleId { get; set; }

        public string SERoleId { get; set; }

        public int SERole { get; set; }

        [DisplayName("Licensed Electrician Number:")]        
        public string ElectricalContractorsLicenseNumber { get; set; }

        [DisplayName("License Number:")]
        [Required(ErrorMessage = "SWH License Number is required.")]
        public string SWHLicenseNumber { get; set; }

        public int SolarElectricianId { get; set; }

        public bool IsSendRequest { get; set; }

        public int SendBy { get; set; }

        public string SESignature { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [DisplayName("Phone:")]
        [StringLength(16, MinimumLength = 6)]
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone No. is not valid, Please enter only Numbers.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        [StringLength(16, MinimumLength = 6)]
        [RegularExpression("^[0-9,+]*$", ErrorMessage = "Mobile No. is not valid, Please enter only Numbers.")]
        public string Mobile { get; set; }

        public bool IsSubContractor { get; set; }

        public int ResellerID { get; set; }

        public int InstallerCount { get; set; }

        public int DesignerCount { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        public DateTime SignatureDate { get; set; }
        public string SignBase64 { get; set; }

        public bool IsCECAccreditationNumberExist { get; set; }
        public bool IsSWHUser { get; set; }
        public bool IsPVDUser { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsSolarElectrician { get; set; }
        public bool IsVisitScheduled { get; set; }

        public int? SWHInstallerId { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public bool IsAutoRequest { get; set; }
        public int SEStatus { get; set; }

        public string GridType { get; set; }
        public string SPS { get; set; }
    }

    public class InstallerDesignerViewVendorAPI
    {
        public string CECAccreditationNumber { get; set; }

        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public int UnitTypeID { get; set; }
        //public string UnitNumber { get; set; }
        //public string StreetNumber { get; set; }
        //public string StreetName { get; set; }
        //public int StreetTypeID { get; set; }
        //public string Town { get; set; }
        //public string State { get; set; }
        //public string PostCode { get; set; }
        //public string CECAccreditationNumber { get; set; }
        //public string CECDesignerNumber { get; set; }
        //public bool IsPostalAddress { get; set; }
        //public int PostalAddressID { get; set; }
        //public string PostalDeliveryNumber { get; set; }
        //public int AddressID { get; set; }
        //public string Email { get; set; }
        //public int SEDesignRoleId { get; set; }
        //public string ElectricalContractorsLicenseNumber { get; set; }
        //public string Phone { get; set; }
        //public string Mobile { get; set; }
    }
}

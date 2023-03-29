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
    public class JobOwnerDetails
    {
        public int JobOwnerID { get; set; }
        public int JobID { get; set; }

        [DisplayName("Owner Type:")]
        [Required(ErrorMessage = "Owner Type is required.")]
        public string OwnerType { get; set; }

        [DisplayName("Organisation Name:")]
        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [DisplayName("Company ABN:")]
        //[Required(ErrorMessage = "Company ABN is required.")]
        public string CompanyABN { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string LastName { get; set; }

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
        [Required(ErrorMessage = "Stree tName is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
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

        [DisplayName("Phone:")]
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for phone.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone No. is not valid, Please enter only Numbers.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required for mobile.")]
        [RegularExpression("^[0-9,+]*$", ErrorMessage = "Mobile No. is not valid, Please enter only Numbers.")]
        public string Mobile { get; set; }

        [DisplayName("Email:")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        [DisplayName("Postal Address:")]
        [Required(ErrorMessage = "Postal Address is required.")]
        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        [Required(ErrorMessage = "Postal Delivery Type is required.")]
        public string PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only printable English characters")]
        public string PostalDeliveryNumber { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        public int AddressID { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        public string OwnerSignature { get; set; }

        public string OwnerBaseSignature { get; set; }

        public string OwnerFinalSignature { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }

        public DateTime? SignatureDate { get; set; }

        public string Fullname { get; set; }

        public string OwnerAddress { get; set; }

        public bool IsOwnerRegisteredWithGST { get; set; }
        public string OldABNNumber { get; set; }
        public bool? IsOwnerAddressValid { get; set; }
        public string PreviousFirstName { get; set; }
        public string PreviousLastName { get; set; }
        public string PreviousEmail { get; set; }
        public string PreviousMobile { get; set; }
        public string PreviousPhone { get; set; }
        public string PreviousOwnerType { get; set; }
        public string oldOwnerAddress { get; set; }
        public string DisplayOwnerFullAddress { get; set; }
        public string NewOwnerAddress { get; set; }
        public string PreviousCompanyName { get; set; }
    }
    public class JobOwnerDetailsVendorAPI
    {
        public string OwnerType { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UnitTypeID { get; set; }
        public string UnitNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string Town { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsPostalAddress { get; set; }
        public string PostalAddressID { get; set; }
        public string PostalDeliveryNumber { get; set; }
        public int AddressID { get; set; }
        public int? StreetTypeID { get; set; }
    }
}

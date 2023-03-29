using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECInstaller
    {
        public int VEECInstallerId { get; set; }

        public int SolarCompanyId { get; set; }

        [DisplayName("Company Name:")]        
        public string CompanyName { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [DisplayName("Email:")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        public string Signature { get; set; }

        [DisplayName("Phone:")]
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone No. is not valid, Please enter only Numbers.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Minimum 6 character required.")]
        [RegularExpression("^[0-9,+]*$", ErrorMessage = "Mobile No. is not valid, Please enter only Numbers.")]
        public string Mobile { get; set; }

        [DisplayName("Unit Type:")]
        //[Required(ErrorMessage = "Unit Type is required.")]
        public int? UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        //[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        [DisplayName("Town:")]
        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("Postal Address:")]
        [Required(ErrorMessage = "Postal Address is required.")]
        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        [Required(ErrorMessage = "Postal Delivery Type is required.")]
        public string PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }

        [DisplayName("Electrician License Number:")]
        [Required(ErrorMessage = "Electrician License Number is required.")]
        public string ElectricalContractorsLicenseNumber { get; set; }

        [DisplayName("CEC Accreditation Number:")]
        [Required(ErrorMessage = "CEC Accreditation Number is required.")]
        public string CECAccreditationNumber { get; set; }

        [DisplayName("CEC Designer Number:")]
        public string CECDesignerNumber { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string IpAddress { get; set; }

        public string Location { get; set; }

        public DateTime SignatureDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Electrical Compliance Number")]
        [Required(ErrorMessage = "Electrical Compliance Number is Required")]
        public string ElectricalComplienceNumber { get; set; }
        

        public int UserId { get; set; }


        public string Name { get; set; }

        public bool IsSysUser { get; set; }
    }
}

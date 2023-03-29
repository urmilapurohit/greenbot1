using FormBot.Entity.Email;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SpvUser
    {
        public int SpvUserId { get; set; }
        public string AspNetUserId { get; set; }
        public string Guid { get; set; }
        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email.")]
        //// [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Email is not valid")]
        [Required(ErrorMessage = "Email is required.")]
        [DisplayName("Email:")]
        [NotMapped]
        public string Email { get; set; }

        [DisplayName("Phone:")]
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        public string Mobile { get; set; }

        [DisplayName("User Name:")]
        [NotMapped]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [DisplayName("Password:")]
        [NotMapped]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        public string PasswordHash { get; set; }


        [DisplayName("Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int StreetTypeID { get; set; }

        [DisplayName("Town/Suburb:")]
        [Required(ErrorMessage = "Town/Suburb is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("Company Website:")]
        public string CompanyWebsite { get; set; }

        [DisplayName("Signature:")]
        public string Signature { get; set; }

        [DisplayName("User Type:")]
        public int SpvUserTypeId { get; set; }
        

        [DisplayName("Is Active:")]
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string strCreatedDate
        {
            get
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }

        public string strModifiedDate
        {
            get
            {
                return ModifiedDate.ToString("dd/MM/yyyy");
            }
        }

        public bool IsDeleted { get; set; }

        [DisplayName("Status:")]
        public byte? Status { get; set; }

        public DateTime? LastLogIn { get; set; }

        public string strLastLogIn
        {
            get
            {
                return LastLogIn != null ? Convert.ToDateTime(LastLogIn).ToString("dd/MM/yyyy hh:mm:ss") : "";
            }
        }

        public int AddressId { get; set; }

        [DisplayName("Postal Delivery Type:")]
        [Required(ErrorMessage = "Postal Delivery Type is required.")]
        public int PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }
        [DisplayName("Logo:")]
        public string Logo { get; set; }

        public int Theme { get; set; }
        public bool IsPostalAddress { get; set; }
        public EmailSignup EmailSignup { get; set; }
        //[DisplayName("Manufacturer Name:")]
        //[Required(ErrorMessage = "Manufacturer Name is required.")]
        public string ManufacturerName { get; set; }
        [DisplayName("Role:")]
       
        public int SpvRoleId { get; set; }

        public bool IsResetPwd { get; set; }
        public string UserTypeName { get; set; }
       // public string PostalDeliveryType { get; set; }

        public int TotalRecordCount { get; set; }

        public string Fullname { get; set; }
    }

    public class SpvUserType
    {
        public int SpvUserTypeId { get; set; }
        public string UserTypeName { get; set; }

    }
}

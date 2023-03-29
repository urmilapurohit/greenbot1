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
    public class XeroContact
    {
        public string XeroContactId { get; set; }
        public int UserId { get; set; }

        [DisplayName("First Name:")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email.")]
        [DisplayName("Email:")]
        [NotMapped]
        public string Email { get; set; }

        [DisplayName("Phone:")]
        public string Phone { get; set; }

        [DisplayName("Mobile:")]
        public string Mobile { get; set; }

        [DisplayName("BSB:")]
        public string BSB { get; set; }

        [DisplayName("Company Name:")]
        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [DisplayName("Company ABN:")]
        public string CompanyABN { get; set; }

        [DisplayName("Client Number:")]
        public string ClientNumber { get; set; }
		[DisplayName("Unique Company Number:")]
		public string UniqueCompanyNumber { get; set; }

		[DisplayName("Company Website:")]
        public string CompanyWebsite { get; set; }

        [DisplayName("AddressLine1:")]
        public string AddressLine1 { get; set; }

        [DisplayName("AddressLine2:")]
        public string AddressLine2 { get; set; }

        [DisplayName("AddressLine3:")]
        public string AddressLine3 { get; set; }

        [DisplayName("AddressLine4:")]
        public string AddressLine4 { get; set; }

        [DisplayName("Town/Suburb:")]
        public string Town { get; set; }

        [DisplayName("State:")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        public string PostCode { get; set; }

        [DisplayName("Account Number:")]
        public string BankAccountDetails { get; set; }

        //[DisplayName("Trader:")]
        //public string Trader { get; set; }

        public string purchasesTrackingCategoryName { get; set; }

         [DisplayName("Trader:")]
        public string purchasesTrackingCategoryOption { get; set; }

         public bool IsPostalAddress { get; set; }

         public bool IsActive { get; set; }

    }
}

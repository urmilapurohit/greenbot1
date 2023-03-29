using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FormBot.Main.Models
{
	public class JobDocumentSignatureModels
	{
		public int GroupId { get; set; }
		public string EncryptedGroupId { get; set; }
		public string GroupName { get; set; }
		public string referenceNumber { get; set; }


		[Display(Name = "First Name")]
		[Required(ErrorMessage = "First Name is Required.")]
		public string Firstname { get; set; }

		[Display(Name = "Last Name:")]
		[Required(ErrorMessage = "Last Name is required.")]
		public string Lastname { get; set; }

		[RegularExpression(@"^[0-9]+$", ErrorMessage = "Mobile Number should be in digit")]
		[Display(Name = "Mobile Number:")]
		[Required(ErrorMessage = "Mobile Number is required.")]
		public string mobileNumber { get; set; }

		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[Display(Name = "Email :")]
		[Required(ErrorMessage = "Email is required.")]
		public string Email { get; set; }
	}
}
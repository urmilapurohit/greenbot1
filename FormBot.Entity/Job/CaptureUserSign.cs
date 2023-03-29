using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class CaptureUserSign
    {

        public int id { get; set; }

        public int jobDocId { get; set; }

        public string signString { get; set; }

        public bool isImg { get; set; }


        public string fieldName { get; set; }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECUpgradeManagerDetail
    {
        public int VEECUpgradeManagerDetailId { get; set; }

        [DisplayName("Upgrade Manager Company Name:")]
        [Required(ErrorMessage = "Upgrade Manager Company Name is required.")]
        public string CompanyName { get; set; }

        [DisplayName("Upgrade Manager First Name:")]
        [Required(ErrorMessage = "Upgrade Manager First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Upgrade Manager Last Name:")]
        [Required(ErrorMessage = "Upgrade Manager Last Name is required.")]
        public string LastName { get; set; }

        [DisplayName("Upgrade Manager Phone Number:")]
        [Required(ErrorMessage = "Upgrade Manager Phone is required.")]
        [StringLength(16, MinimumLength = 10, ErrorMessage = "Minimum 10 Digit required.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone No. is not valid, Please enter only Numbers.")]
        public string Phone { get; set; }

        [DisplayName("Company ABN :")]
        [Required(ErrorMessage = "Company ABN is required.")]
        public string CompanyABN { get; set; }



        public bool IsSysUser { get; set; }

        public int SolarCompanyId { get; set; }

        public bool IsDeleted { get; set; }

    }
}

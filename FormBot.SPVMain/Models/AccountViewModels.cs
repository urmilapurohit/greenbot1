using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FormBot.SPVMain.Models
{
        public class LoginViewModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            public string CompanyName { get; set; }
            public string Theme { get; set; }
            public string Logo { get; set; }
            public int isActiveDiv { get; set; }
            public int isDisableSignUp { get; set; }
            public string DefaultLoginCompanyName { get; set; }
        }
    
}
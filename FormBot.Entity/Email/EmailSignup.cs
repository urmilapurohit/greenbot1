using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FormBot.Entity.Email
{
    [Serializable]
    public class EmailSignup
    {
        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "ConfigurationEmail is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string ConfigurationEmail { get; set; }

        [Required]
        [EmailAddress]
        public string Login { get; set; }

        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [AllowHtml]
        public string ConfigurationPassword { get; set; }

        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Incoming Mail is required")]
        [Display(Name = "Incoming Mail")]
        public string IncomingMail { get; set; }

        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Incoming Mail Port is required")]
        [Display(Name = "Port")]
        public int? IncomingMailPort { get; set; }

        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Outgoing Mail is required")]
        [Display(Name = "Outgoing Mail")]
        public string OutgoingMail { get; set; }

        [RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Outgoing Mail Port is required")]
        [Display(Name = "Port")]
        public int? OutgoingMailPort { get; set; }

        //[RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "ConfigurationEmail is required")]
        [Required]
        public bool SMTP_Authenctication { get; set; }

        [AllowHtml]
        [Display(Name = "Signature")]
        public string EmailSignature { get; set; }

        [Display(Name = "Type")]
        public int ProtocolType { get; set; }

        [Display(Name = "Default Timezone")]
        public int Def_TimeZone { get; set; }

        public bool IsRequired { get; set; }
    }

    [Serializable]
    public class RecEmailSignup
    {
        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "ConfigurationEmail is required")]
        [EmailAddress]
        [Display(Name = "Rec Email")]
        public string RecConfigurationEmail { get; set; }

        [Required]
        [EmailAddress]
        public string RecLogin { get; set; }

        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Password is required")]
        [Display(Name = "Rec Password")]
        [AllowHtml]
        public string RecConfigurationPassword { get; set; }

        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Incoming Mail is required")]
        [Display(Name = "Incoming Mail")]
        public string RecIncomingMail { get; set; }

        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Incoming Mail Port is required")]
        [Display(Name = "Port")]
        public int? RecIncomingMailPort { get; set; }

        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Outgoing Mail is required")]
        [Display(Name = "Outgoing Mail")]
        public string RecOutgoingMail { get; set; }

        [RequiredIf("RecIsRequired", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "Outgoing Mail Port is required")]
        [Display(Name = "Port")]
        public int? RecOutgoingMailPort { get; set; }

        //[RequiredIf("IsRequired", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "ConfigurationEmail is required")]
        [Required]
        public bool RecSMTP_Authenctication { get; set; }

        [AllowHtml]
        [Display(Name = "Signature")]
        public string RecEmailSignature { get; set; }

        [Display(Name = "Type")]
        public int RecProtocolType { get; set; }

        [Display(Name = "Default Timezone")]
        public int RecDef_TimeZone { get; set; }

        public bool RecIsRequired { get; set; }
    }
}

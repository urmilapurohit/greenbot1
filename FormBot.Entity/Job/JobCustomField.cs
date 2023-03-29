using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobCustomField
    {
        public int JobCustomFieldId { get; set; }

        [Required(ErrorMessage = "Custom Field is required.")]
        [Display(Name = "Custom Field:")]
        public string CustomField { get; set; }

        public int SolarCompanyId { get; set; }

        public int TotalRecords { get; set; }
    }

    public class DefaultSetting
    {
        public bool IsPreapproval { get; set; }
        public bool IsConnection { get; set; }

        public bool IsAllowTrade { get; set; }

        public bool IsCreateJobNotification { get; set; }

        public string JobId { get; set; }
        public bool GlobalisAllowedSPV { get; set; }
         public bool IsAllowedAccessToAllUsers { get; set; }

        public string LastSyncDate { get; set; }
    }
}

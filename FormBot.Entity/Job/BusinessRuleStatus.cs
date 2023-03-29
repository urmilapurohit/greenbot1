using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class BusinessRuleStatus
    {
        public bool IsSuccess { get; set; }
        public string ValidationSummary { get; set; }
        public string IsEMailNotification { get; set; }
        public string EMailList { get; set; }
        public string STCStatusId { get; set; }
        public string STCStatusName { get; set; }
        public string STCDescription { get; set; }
        public bool IsError { get; set; }
        public int ErrorLength { get; set; }
        public bool IsValidSystemSize { get; set; }
        public string IsSpvInstallationVerified { get; set; }
        public bool IsSPVRequired { get; set; }


    }
}

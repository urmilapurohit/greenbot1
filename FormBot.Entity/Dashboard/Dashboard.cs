using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Dashboard
{
    public class Dashboard
    {
        public int JobSchedulingID { get; set; }
        public int JobID { get; set; }
        public string strVisitStartDate { get; set; }
        public string CompanyName { get; set; }
        public string ClientDetails { get; set; }
        public string VisitDescription { get; set; }
        public string JobStage { get; set; }
        public int status { get; set; }
        public int TotalRecords { get; set; }
        public DateTime VisitStartDate { get; set; }
        public int UserId { get; set; }
    }

    public class DashboardSTCJobStaus
    {
        public int ResellerId { get; set; }
        public string ResellerName { get; set; }

        public int SolarCompanyId { get; set; }
        public string CompanyName { get; set; }

        public int NewSubmission { get; set; }
        public int ReSubmission { get; set; }
        public int UnderReview { get; set; }
        public int ComplianceIssues { get; set; }
        public int AwaitingAuthorization { get; set; }
        public int CERFailed { get; set; }
        public int ReadyToCreate { get; set; }

    }
}

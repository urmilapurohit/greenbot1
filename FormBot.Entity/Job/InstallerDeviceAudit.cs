using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class InstallerDeviceAudit
    {
        public int UserID { get; set; }
        public string InstallerName { get; set; }
        public string AccreditationNumber { get; set; }
        public List<InstallerVisit> InstallerVisits { get; set; }
    }
    public class InstallerVisit
    {
        public int VisitID { get; set; }
        public int DeviceID { get; set; }
        public DateTime RegisteredOTP { get; set; }
        public DateTime LastUsed { get; set; }
        public DateTime MostRecentSignedIn { get; set; }
        public string FileLocation { get; set; }
        public List<InstallerVisitDateTime> InstallerVisitDateTimes { get; set; }
    }
    public class InstallerVisitDateTime
    {
        public DateTime VisitDate { get; set; }
        public string LengthOfTime { get; set; }
    }

    public class InstallerAuditDetails
    {
        public int UserID { get; set; }
        public string InstallerName { get; set; }
        public string AccreditationNumber { get; set; }
        public DateTime RegisteredOTP { get; set; }
        public DateTime LastUsed { get; set; }
        public DateTime MostRecentSignedIn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class InstallerDeviceAuditViewModel
    {
        public int VisitID { get; set; }
        public int DeviceID { get; set; }
        public List<AuditDetails> Details { get; set; }
    }

    public class AuditDetails
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Date { get; set; }
    }
}

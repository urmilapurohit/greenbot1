using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobDetails
    {
        public int JobID { get; set; }
        public string RefNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string JobNumber { get; set; }
        public int JobStage { get; set; }
        public int JobType { get; set; }
        public int InstallerID { get; set; }
        public int DesignerID { get; set; }
        public int JobElectricianID { get; set; }
        public DateTime InstallationDate { get; set; }
        public int Priority { get; set; }
    }
}

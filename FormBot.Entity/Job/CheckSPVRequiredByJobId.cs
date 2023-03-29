using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
   public class CheckSPVRequiredByJobId
    {
        public string UserID { get; set; }
        public string ApiToken { get; set; }
        public int STCStatus { get; set; }
        public bool IsSPVRequired { get; set; }
        public string Status { get; set; }
        public bool IsUpload { get; set; }
        public int JobId { get; set; }
    }
}

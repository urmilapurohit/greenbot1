using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.ActivityLog
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }
        public int UserId { get; set; }
        public string LogMessage { get; set; }
        public DateTime CreatedDate { get; set; }

        public string strCreatedDate {
            get
            {
                return CreatedDate != null ? CreatedDate.ToString("dd/MM/yyyy") : null;
            }
        }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string strModifiedDate
        {
            get
            {
                return ModifiedDate != null ? ModifiedDate.ToString("dd/MM/yyyy") : null;
            }
        }

        public int ModifiedBy { get; set; }
        public string IpAddress  { get; set; }
        public string ActivityTypeName { get; set; }

        public int ActvityLogTypeId { get; set; }
        public string UserFullName { get; set; }

        public int TotalRecords { get; set; }
    }
    
}

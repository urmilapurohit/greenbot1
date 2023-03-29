using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobCustomDetails
    {
        public int JobCustomDetailsId { get; set; }

        public int JobID { get; set; }

        public string CustomField { get; set; }

        public int JobCustomFieldId { get; set; }

        public string CustomValue { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}

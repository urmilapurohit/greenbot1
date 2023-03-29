using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class STCJobHistory
    {
        public long id { get; set; }

        public int JobId { get; set; }

        public int StcStatusId { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public string Status { get; set; }

        public string Color { get; set; }

        public List<STCJobHistory> lstSTCJobHistory { get; set; }

        public string Desc { get; set; }
        public string changedBy { get; set; }
    }
}

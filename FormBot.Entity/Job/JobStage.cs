using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class JobStage
    {
        public int JobStageId { get; set; }
        public string StageName { get; set; }

        public string ColorCode { get; set; }
        public int jobCount { get; set; }
    }
}

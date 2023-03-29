using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobStatus
    {
        public bool insert { get; set; }
        public bool update { get; set; }
        public bool error { get; set; }
        public bool success { get; set; }
        public string id { get; set; }
        public decimal? STCValue { get; set; }
        public bool? IsRecUp { get; set; }
        public bool? IsSPVRequired { get; set; }

    }
}

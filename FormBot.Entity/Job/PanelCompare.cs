using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class PanelCompare
    {
        public string ID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Count { get; set; }

        public int JobID { get; set; }
    }
    public class InverterCompare
    {
        public string ID { get; set; }
        public string Brand { get; set; }
        public string Series { get; set; }
        public string Model { get; set; }
        public string Count { get; set; }

        public int JobID { get; set; }
    }
}

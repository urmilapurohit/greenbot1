using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class StatusHistory
    {
        public int JobID { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public DateTime ChangedOn { get; set; }
    }
}

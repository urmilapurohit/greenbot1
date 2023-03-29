using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class CERLog
    {
        public int CERLogID { get; set; }
        public string Version { get; set; }
        public int CERType { get; set; }
        public int SubType { get; set; }
        public string CERText { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}

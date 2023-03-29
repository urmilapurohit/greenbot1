using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.KendoGrid
{
    public class KendoGridFilter : KendoGridData
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class STCHistory
    {
        public int JobID { get; set; }
        public string STCPrice { get; set; }

        public string Message { get; set; }

        public string stcAmt { get; set; }

        public string IsGst { get; set; }

        public string DT { get; set; }

        public int ModifiedBy { get; set; }

        public string UserType { get; set; }
    }
}

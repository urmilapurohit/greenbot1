using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Dashboard
{
    public class SCATradeReport
    {
        public int TradedCount { get; set; }

        public string STCSubmissionDateMonth { get; set; }

        public string STCSubmissionDateYear { get; set;}

        public int Month { get; set; }

        public string STCSubmissionDateWeek { get; set; }

        public int Week { get; set; }
    }
}

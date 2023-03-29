using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Dashboard
{
    public class SCAStatusReport
    {
        public decimal StatusCount { get; set; }

        public string Status { get; set; }

        public string MonthName { get; set; }
        public int Month { get; set; }

        public int Year { get; set; }

        public string WeekName { get; set; }

        public int Week { get; set; }
    }
}

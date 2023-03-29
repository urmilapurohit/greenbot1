using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.JobHistory
{
	public class ModifiedStcValueHistory
	{
		public decimal OldStcValue { get; set; }
		public decimal NewStcValue { get; set; }
		public string Name { get; set; }
		public int JobId { get; set; }
        public string HistoryMessage { get; set; }
	}
}

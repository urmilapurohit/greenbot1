using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Task
{
	public partial class ScheduleTask
	{
		public int ScheduleTaskId { get; set; }
		public string Name { get; set; }
		public string SystemName { get; set; }
		public int Seconds { get; set; }
		public bool Enabled { get; set; }
		public int SetSentTries { get; set; }
		public bool StopOnError { get; set; }
		public DateTime? LastStartOn { get; set; }
		public DateTime? LastEndOn { get; set; }
		public DateTime? LastSuccessOn { get; set; }
	}
}

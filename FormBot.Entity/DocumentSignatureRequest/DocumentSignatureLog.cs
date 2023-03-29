using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{
	public class DocumentSignatureLog
	{
		public int DocumentSignatureLogId { get; set; }
		public int JobDocId { get; set; }
		public string UserTypeName { get; set; }
		public string UserFullName { get; set; }
		public int MessageType { get; set; }
		public string Message { get; set; }
		public DateTime CreatedOn { get; set; }
		public string IpAddress { get; set; }
	}
}

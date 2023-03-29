using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{
	public class JobDocumentMessage
	{
		public int JobDocumentMessageId { get; set; }
		public int JobDocId { get; set; }
		public string TypeId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Message { get; set; }
		public int MessageCategory { get; set; }
		public DateTime CreatedOn { get; set; }
		public int CreatedBy { get; set; }
		public bool IsDeleted { get; set; }
	}
}

using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{
	public class DocumentSignatureOption
	{
		public int DocumentSignatureOptionId { get; set; }
		public int Type { get; set; }
		public bool SendCopy { get; set; }
		public bool SameAsInstaller { get; set; }
		public int BulkUploadDocumentSignatureId { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedBy { get; set; }
		public bool IsDeleted { get; set; }

	}
	public class DocumentSignatureOptionCustom : DocumentSignatureOption
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int JobDocId { get; set; }
		public int JobId { get; set; }
		public string CustomMessage { get; set; }
	}
}

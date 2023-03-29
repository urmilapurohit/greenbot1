using System;

namespace FormBot.Entity.DocumentSignatureRequest
{
	public class DocumentUserEmail
	{
		public int DocumentUserEmailId { get; set; }
		public int BulkUploadDocumentSignatureId { get; set; }
		public string EmailId { get; set; }
		public int Type { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}

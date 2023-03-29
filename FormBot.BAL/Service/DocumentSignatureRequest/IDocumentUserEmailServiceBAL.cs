using FormBot.Entity.DocumentSignatureRequest;
using System.Collections.Generic;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public interface IDocumentUserEmailServiceBAL
	{
		DocumentUserEmail Insert(DocumentUserEmail oDocumentUserEmail);
		List<DocumentUserEmail> GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId);
	}
}

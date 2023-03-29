using FormBot.Entity.DocumentSignatureRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public interface IDocumentSignatureOptionBAL
	{
		DocumentSignatureOption InsertUpdateDelete(DocumentSignatureOption objDocumentSignatureoption, int action, string CustomMessage = "");
		List<DocumentSignatureOption> GetAll();
		List<DocumentSignatureOptionCustom> GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId);
	}
}

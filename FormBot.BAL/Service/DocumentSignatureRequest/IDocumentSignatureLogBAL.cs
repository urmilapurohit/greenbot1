using FormBot.Entity.DocumentSignatureRequest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public interface IDocumentSignatureLogBAL
	{
		DocumentSignatureLog Insert(DocumentSignatureLog oDocumentSignatureLog);
		List<DocumentSignatureLog> GetByJobDocId(int JobDocId);
        /// <summary>
        /// get job doc path from jobdocid
        /// </summary>
        /// <param name="JobDocId"></param>
        /// <returns></returns>
        DataSet GetDocumentPathAndJobIdByJobDocumentId(int JobDocId);

    }
}

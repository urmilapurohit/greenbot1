using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FormBot.Entity.DocumentSignatureRequest;
using FormBot.DAL;
using System.Data;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public class DocumentUserEmailServiceBAL : IDocumentUserEmailServiceBAL
	{
		public DocumentUserEmail Insert(DocumentUserEmail oDocumentUserEmail)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentSignatureId", SqlDbType.Int, oDocumentUserEmail.BulkUploadDocumentSignatureId));
			sqlParameters.Add(DBClient.AddParameters("EmailId", SqlDbType.NVarChar, oDocumentUserEmail.EmailId));
			sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.Int, oDocumentUserEmail.Type));
			sqlParameters.Add(DBClient.AddParameters("CreatedOn", SqlDbType.DateTime, DateTime.Now));
			sqlParameters.Add(DBClient.AddParameters("UpdatedOn", SqlDbType.DateTime, DateTime.Now));
			objDocumentUserEmail = CommonDAL.ExecuteProcedure<DocumentUserEmail>("InsertUpdate_DocumentUserEmail", sqlParameters.ToArray()).ToList().FirstOrDefault();
			return objDocumentUserEmail;
		}
		public List<DocumentUserEmail> GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentSignatureId", SqlDbType.Int, BulkUploadDocumentSignatureId));
			List<DocumentUserEmail> lstDocumentUserEmail = CommonDAL.ExecuteProcedure<DocumentUserEmail>("GetByBulkUploadDocumentSignatureId_DocumentUserEmail", sqlParameters.ToArray()).ToList();
			return lstDocumentUserEmail;
		}
	}
}

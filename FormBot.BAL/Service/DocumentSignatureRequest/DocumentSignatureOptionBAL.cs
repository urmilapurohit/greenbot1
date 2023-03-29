using FormBot.DAL;
using FormBot.Entity.DocumentSignatureRequest;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FormBot.Helper.SystemEnums;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public class DocumentSignatureOptionBAL : IDocumentSignatureOptionBAL
	{
		private readonly Logger _logger;
		public DocumentSignatureOptionBAL()
		{
			_logger = new Logger();
		}
		public DocumentSignatureOption InsertUpdateDelete(DocumentSignatureOption objDocumentSignatureoption,int action, string CustomMessage = "")
		{
			try
			{
				List<SqlParameter> sqlParameters = new List<SqlParameter>();
				sqlParameters.Add(DBClient.AddParameters("Action", SqlDbType.Int, action));
				sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.Int, objDocumentSignatureoption.Type));
				sqlParameters.Add(DBClient.AddParameters("SendCopy", SqlDbType.Bit, objDocumentSignatureoption.SendCopy));
				sqlParameters.Add(DBClient.AddParameters("SameAsInstaller", SqlDbType.Bit, objDocumentSignatureoption.SameAsInstaller));
				sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentSignatureId", SqlDbType.Int, objDocumentSignatureoption.BulkUploadDocumentSignatureId));
				sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, objDocumentSignatureoption.CreatedDate));
				sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, objDocumentSignatureoption.CreatedBy));
				sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, objDocumentSignatureoption.ModifiedDate));
				sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, objDocumentSignatureoption.ModifiedBy));
				sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, objDocumentSignatureoption.IsDeleted));
				sqlParameters.Add(DBClient.AddParameters("DocumentSignatureOptionId", SqlDbType.Int, objDocumentSignatureoption.DocumentSignatureOptionId));
				sqlParameters.Add(DBClient.AddParameters("CustomMessage", SqlDbType.NVarChar, CustomMessage));
				objDocumentSignatureoption = CommonDAL.ExecuteProcedure<DocumentSignatureOption>("InsertUpdateDelete_DocumentSignatureOption", sqlParameters.ToArray()).ToList().FirstOrDefault();
				return objDocumentSignatureoption;
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(),ex);
				return null;
			}
		}
		public List<DocumentSignatureOption> GetAll()
		{
			try
			{
				List<DocumentSignatureOption> lstDocumentSignatureOption = CommonDAL.ExecuteProcedure<DocumentSignatureOption>("GetAll_DocumentSignatureOption").ToList();
				return lstDocumentSignatureOption;
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(), ex);
				return null;
			}
		}
		public List<DocumentSignatureOptionCustom> GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId)
		{
			try
			{
				List<SqlParameter> sqlParameters = new List<SqlParameter>();
				sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentSignatureId", SqlDbType.Int, BulkUploadDocumentSignatureId));
				List<DocumentSignatureOptionCustom> lstDocumentSignatureOption = CommonDAL.ExecuteProcedure<DocumentSignatureOptionCustom>("GetByBulkUploadDocumentSignatureId_DocumentSignatureOption", sqlParameters.ToArray()).ToList();
				return lstDocumentSignatureOption;
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(), ex);
				return null;
			}
		}
	}
}

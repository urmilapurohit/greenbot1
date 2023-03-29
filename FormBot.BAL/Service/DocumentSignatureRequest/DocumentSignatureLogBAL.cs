using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.DAL;
using FormBot.Entity.DocumentSignatureRequest;
namespace FormBot.BAL.Service.DocumentSignatureRequest
{
	public class DocumentSignatureLogBAL : IDocumentSignatureLogBAL
	{
		public DocumentSignatureLog Insert(DocumentSignatureLog oDocumentSignatureLog)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			DocumentSignatureLog objDocumentSignatureLog = new DocumentSignatureLog();
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, oDocumentSignatureLog.JobDocId));
			sqlParameters.Add(DBClient.AddParameters("UserTypeName", SqlDbType.NVarChar, oDocumentSignatureLog.UserTypeName));
			sqlParameters.Add(DBClient.AddParameters("UserFullName", SqlDbType.NVarChar, oDocumentSignatureLog.UserFullName));
			sqlParameters.Add(DBClient.AddParameters("Message", SqlDbType.NVarChar, oDocumentSignatureLog.Message));
			sqlParameters.Add(DBClient.AddParameters("MessageType", SqlDbType.Int, oDocumentSignatureLog.MessageType));
			sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, oDocumentSignatureLog.IpAddress));
			sqlParameters.Add(DBClient.AddParameters("CreatedOn", SqlDbType.DateTime, oDocumentSignatureLog.CreatedOn));
			objDocumentSignatureLog = CommonDAL.ExecuteProcedure<DocumentSignatureLog>("Insert_DocumentSignatureLog", sqlParameters.ToArray()).ToList().FirstOrDefault();
			return objDocumentSignatureLog;
		}
		public List<DocumentSignatureLog> GetByJobDocId(int JobDocId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, JobDocId));
			List <DocumentSignatureLog> lstDocumentSignatureLog = CommonDAL.ExecuteProcedure<DocumentSignatureLog>("GetByJobDocId_DocumentSignatureLog", sqlParameters.ToArray()).ToList();
			return lstDocumentSignatureLog;
		}
        /// <summary>
        /// get job doc path from jobdocid
        /// </summary>
        /// <param name="JobDocId"></param>
        /// <returns></returns>
        public DataSet GetDocumentPathAndJobIdByJobDocumentId(int JobDocId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetDocumentPathAndJobIdByJobDocumentId", sqlParameters.ToArray());
            return ds;
        }

    }
}

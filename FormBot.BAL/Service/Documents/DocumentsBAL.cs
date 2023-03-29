using FormBot.DAL;
using FormBot.Entity.Documents;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FormBot.BAL.Service.Documents
{
    /// <summary>
    /// DocumentsBAL
    /// </summary>
    public class DocumentsBAL : IDocumentsBAL
    {
        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <param name="distributorID">The distributor ID.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns> List</returns>
        public DataSet GetDocuments(string distributorID, string stage, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NMI", SqlDbType.NVarChar, distributorID));
            sqlParameters.Add(DBClient.AddParameters("Stage", SqlDbType.NVarChar, stage));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet dsDocument = CommonDAL.ExecuteDataSet("Document_GetByNMI", sqlParameters.ToArray());
            return dsDocument;
            //List<DocumentsView> lstSolarElectrician = CommonDAL.ExecuteProcedure<DocumentsView>("Document_GetByNMI", sqlParameters.ToArray()).ToList();
            //return lstSolarElectrician;
        }

        /// <returns> List</returns>
        public DataSet GetDocumentsAll(string distributorID, string stage, int jobId,bool IsJsonData = true)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NMI", SqlDbType.NVarChar, distributorID));
            sqlParameters.Add(DBClient.AddParameters("Stage", SqlDbType.NVarChar, stage));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("IsJsonData", SqlDbType.Bit, IsJsonData));
            DataSet dsDocument = CommonDAL.ExecuteDataSet("DocumentAll_GetByNMI", sqlParameters.ToArray());
            return dsDocument;
            //List<DocumentsView> lstSolarElectrician = CommonDAL.ExecuteProcedure<DocumentsView>("Document_GetByNMI", sqlParameters.ToArray()).ToList();
            //return lstSolarElectrician;
        }

        public DataSet GetDocumentsByStateId(int stateId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StateId", SqlDbType.Int, stateId));
            return CommonDAL.ExecuteDataSet("GetDocumentsByStateId", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get all global Document Template in Document Signature Request 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public DataSet GetDocumentTemplateByStateId(int stateId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StateId", SqlDbType.Int, stateId));
            return CommonDAL.ExecuteDataSet("GetDocumentTemplateByStateId", sqlParameters.ToArray());
        }
        public DataSet GetStateList()
        {
            return CommonDAL.ExecuteDataSet("GetStateList");
        }

        /// <summary>
        /// Gets the documents by job identifier.
        /// </summary>
        /// <param name="distributorID">The distributor ID.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns> List</returns>
        public List<DocumentsView> GetDocumentsByJobId(string distributorID, string stage, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NMI", SqlDbType.NVarChar, distributorID));
            sqlParameters.Add(DBClient.AddParameters("Stage", SqlDbType.NVarChar, stage));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            List<DocumentsView> lstSolarElectrician = CommonDAL.ExecuteProcedure<DocumentsView>("JobDocument_GetByJobId", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns> List</returns>
        public List<DocumentsView> GetDocument(int documentId,bool IsClassic)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DocumentId", SqlDbType.Int, documentId));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, IsClassic));
            List<DocumentsView> lstSolarElectrician = CommonDAL.ExecuteProcedure<DocumentsView>("Document_GetByDocumentId", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }


        public DataTable GetJobDocumentPath(int JobDocumentId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobDocumentPath", sqlParameters.ToArray());
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return (ds.Tables[0]);
            }
            return null;
        }

        public List<JobDocumentResponce> GetDocumentsByJobIdForApi(int jobId, string JobDocumentId = "",string AlreadyAddedDocumentid = "")
        {
            string distributorID = new CreateJobBAL().GetNMIByJobID(jobId);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));

            //if(JobDocumentId > 0)
            //    sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));

			if (!string.IsNullOrEmpty(JobDocumentId))
				sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.NVarChar, JobDocumentId));
			if (!string.IsNullOrEmpty(AlreadyAddedDocumentid))
				sqlParameters.Add(DBClient.AddParameters("AlreadyAddedDocumentid", SqlDbType.NVarChar, AlreadyAddedDocumentid));

			List<JobDocumentResponce> lstSolarElectrician = CommonDAL.ExecuteProcedure<JobDocumentResponce>("Document_GetForAPI", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }
		public List<CaptureUserSign> GetCapturedSignByJobIdForApi(int jobId,string JobDocId = "")
		{
			string distributorID = new CreateJobBAL().GetNMIByJobID(jobId);
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
			sqlParameters.Add(DBClient.AddParameters("JobDocIds", SqlDbType.NVarChar, JobDocId));
			List<CaptureUserSign> lstCapturedSign = CommonDAL.ExecuteProcedure<CaptureUserSign>("CapturedSignByJobId_GetForAPI", sqlParameters.ToArray()).ToList();
			return lstCapturedSign;
		}
		public List<CaptureUserSign> GetCapturedSignatureByJobIdDeletedListForApi(int jobId)
		{
			string distributorID = new CreateJobBAL().GetNMIByJobID(jobId);
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
			List<CaptureUserSign> lstCapturedSign = CommonDAL.ExecuteProcedure<CaptureUserSign>("CapturedSignByJobIdForDeleteList_GetForAPI", sqlParameters.ToArray()).ToList();
			return lstCapturedSign;
		}
		public void GetCapturedSignByJobIdUploadLiveForApi(int jobDocId, string fieldName, string SignString, string Firstname, string Lastname, string mobileNumber, string Email, DateTime CreatedDate, DateTime ModifiedDate, bool isUpdate = true)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, jobDocId));
			sqlParameters.Add(DBClient.AddParameters("FieldName", SqlDbType.NVarChar, fieldName));
			sqlParameters.Add(DBClient.AddParameters("IsUpdate", SqlDbType.Bit, isUpdate));
			sqlParameters.Add(DBClient.AddParameters("signString", SqlDbType.NVarChar, SignString));
			sqlParameters.Add(DBClient.AddParameters("Firstname", SqlDbType.NVarChar, Firstname));
			sqlParameters.Add(DBClient.AddParameters("Lastname", SqlDbType.NVarChar, Lastname));
			sqlParameters.Add(DBClient.AddParameters("mobileNumber", SqlDbType.NVarChar, mobileNumber));
			sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, Email));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
			sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, ModifiedDate));
			CommonDAL.ExecuteProcedure<CaptureUserSign>("CapturedSignByJobIdForUpload_GetForAPI", sqlParameters.ToArray());
		}
	/// <summary>
	/// Gets the documents by job identifier.
	/// </summary>
	/// <param name="distributorID">The distributor ID.</param>
	/// <param name="stage">The stage.</param>
	/// <param name="jobId">The job identifier.</param>
	/// <returns> List</returns>
	public DataSet DownloadAllAndActiveTabDocument(string distributorID, string stage, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NMI", SqlDbType.NVarChar, distributorID));
            sqlParameters.Add(DBClient.AddParameters("Stage", SqlDbType.NVarChar, stage));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet dsDocument = CommonDAL.ExecuteDataSet("JobDocument_GetByJobId", sqlParameters.ToArray());
            return dsDocument;
            //List<DocumentsView> lstSolarElectrician = CommonDAL.ExecuteProcedure<DocumentsView>("JobDocument_GetByJobId", sqlParameters.ToArray()).ToList();
            //return lstSolarElectrician;
        }

		/// <summary>
		/// Update Job Document
		/// </summary>
		/// <param name="documentId">documentId</param>
		/// <param name="jobId">jobId</param>
		/// <param name="jsonData">jsonData</param>
		public DataSet UpdateJobDocument(int documentId, int jobId, string jsonData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DocumentId", SqlDbType.Int, documentId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("JsonData", SqlDbType.NVarChar, jsonData));
			return CommonDAL.ExecuteDataSet("JobDocument_UpdateFromAPI", sqlParameters.ToArray());
        }

        public List<DocumentsView> GetDocumentForCheckListItem(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            List<DocumentsView> lstDocuments = CommonDAL.ExecuteProcedure<DocumentsView>("JobDocument_GetDocumentForCheckListItem", sqlParameters.ToArray()).ToList();
            if (lstDocuments.Count > 0)
            {
                for (int i = 0; i < lstDocuments.Count; i++)
                {
                    lstDocuments[i].FileName = System.IO.Path.GetFileName(lstDocuments[i].FileName);
                }
            }

            return lstDocuments;
        }

    }
	//customeReponce Classes
	public class JobDocumentResponce:DocumentsView
	{
		public int? Flag { get; set; }
	}
}
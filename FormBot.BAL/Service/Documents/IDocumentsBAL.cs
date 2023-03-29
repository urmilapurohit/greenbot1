using FormBot.Entity.Documents;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.Documents
{
    /// <summary>
    /// IDocumentsBAL
    /// </summary>
    public interface IDocumentsBAL
    {
        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <param name="distributorID">The distributor ID.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>List</returns>
        //List<DocumentsView> GetDocuments(string distributorID, string stage, int jobId);
        DataSet GetDocuments(string distributorID, string stage, int jobId);

        DataSet GetDocumentsAll(string distributorID, string stage, int jobId, bool IsJsonData = true);

        DataSet GetStateList();


        DataSet GetDocumentsByStateId(int stateId);

        /// <summary>
        /// Get all global Document Template in Document Signature Request 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        DataSet GetDocumentTemplateByStateId(int stateId); 
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>List</returns>
        List<DocumentsView> GetDocument(int documentId,bool IsClassic = true);

        DataTable GetJobDocumentPath(int JobDocumentId);

        /// <summary>
        /// Gets the documents by job identifier.
        /// </summary>
        /// <param name="nmi">The nmi.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>List</returns>
        List<DocumentsView> GetDocumentsByJobId(string nmi, string stage, int jobId);

		/// <summary>
		/// Gets the documents by job identifier for API.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <returns>List</returns>
		List<JobDocumentResponce> GetDocumentsByJobIdForApi(int jobId, string JobDocumentId = "", string AlreadyAddedDocumentid = "");
		List<CaptureUserSign> GetCapturedSignByJobIdForApi(int jobId, string JobDocId = "");
		List<CaptureUserSign> GetCapturedSignatureByJobIdDeletedListForApi(int jobId);
		void GetCapturedSignByJobIdUploadLiveForApi(int jobDocId, string fieldName, string SignString, string Firstname, string Lastname, string mobileNumber, string Email, DateTime CreatedDate, DateTime ModifiedDate, bool isUpdate = true);
		/// <summary>
		/// Gets the documents by job identifier.
		/// </summary>
		/// <param name="distributorID">The distributor ID.</param>
		/// <param name="stage">The stage.</param>
		/// <param name="jobId">The job identifier.</param>
		/// <returns> List</returns>
		DataSet DownloadAllAndActiveTabDocument(string distributorID, string stage, int jobId);

        /// <summary>
        /// Update Job Document
        /// </summary>
        /// <param name="documentId">documentId</param>
        /// <param name="jobId">jobId</param>
        /// <param name="jsonData">jsonData</param>
        DataSet UpdateJobDocument(int documentId, int jobId, string jsonData);

        List<DocumentsView> GetDocumentForCheckListItem(int jobId);
    }
}

using FormBot.Entity.DocumentSignatureRequest;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.DocumentSignatureRequest
{
    public interface IDocumentSignatureRequestBAL
    {
		BulkUploadDocumentSignature GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId);
		BulkUploadDocumentSignature GetByJobDocumentId(int JobDocumentId,int GroupId = 0);

		int InsertUpdateGroupName(string GroupName, int BulkUploadDocumentGroupId,string groupDocumentPath = "");

        IList<BulkUploadDocumentGroup> GroupNameList(int pageNumber, int pageSize, string sortCol, string sortDir, string groupName);

        void DeleteGroupName(string bulkDocumentGroupId);

        BulkUploadDocumentGroup GetBulkDocumentGroupName(int bulkDocumentGroupId);

        List<BulkUploadDocumentGroup> GetBulkUploadDocumentGroupNameList();

        List<EmailTemplate> GetEmailTemplateList();

        //List<DocumentWiseSignatureDetails> GetBulkJobListForSendEmail(int pageNumber, int pageSize, string sortCol, string sortDir, string referenceNumber, string solarCompanyId, string resellerId, bool InstallerSignatureStatus, bool HomeOwnerSignatureStatus, bool SolarCompanySignatureStatus, bool ElectricianSignatureStatus, bool DesignerSignatureStatus);
        List<DocumentWiseSignatureDetails> GetBulkJobListForSendEmail(int pageNumber, int pageSize, string sortCol, string sortDir,int bulkUploadDocumentGroupId);
		CaptureUserSign GetUserBasicDetailsBySignatureTypeOfJobDocument(int JobType, int JobId);
		List<DocumentWiseSignatureDetails> GetBulkJobListForAddGroup(int pageNumber, int pageSize, string sortCol, string sortDir, string resellerId, string referenceNumber = "", string solarCompanyId = "", int JobId = 0, int JobType = 0,string PVDSWHCode = "");


		List<DocumentWiseSignatureDetails> ShowJobInGroup(int pageNumber, int pageSize, string sortCol, string sortDir, int bulkUploadDocumentGroupId, string refNumber);

		DataSet GetDocumentPathFromDocumentTemplateId(int DocumentTemplateId);

        int InsertUpdateBulkJobForSignatureRequest(int BulkUploadDocumentGroupId,int jobId,string installerSignatureStatus, string DesignerSignatureStatus, string ElectricianSignatureStatus, string HomeOwnerSignatureStatus, string SolarCompanySignatureStatus);

        void AddJobtoGroup(string jobId,string PVDSWHCode, int BulkUploadDocumentGroupId, string InstallerSignatureStatus, string HomeOwnerSignatureStatus, string SolarCompanySignatureStatus, string ElectricianSignatureStatus, string DesignerSignatureStatus);

        void DeleteJobInGroup(int bulkDocumentGroupId, string JobId);
		List<DocumentSignatureStatusWithEmailResponce> GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(int DocumentGroupId, int JobId, bool isMessageEmail =  false, int JobDocId = 0);
		void UpdateSentMailStatusForBulkUploadSignature(int bulkDocumentGroupId, int JobId, int EmailStatus, int JobDocumentId = 0);
		int AddMessageForJobDocument(JobDocumentMessage objJobDocumentMessage);
		List<JobDocumentMessage> GetAllMessageByJobDocumentIdWise(int JobDocId);
		DocumentWiseSignatureDetails UpdateSignatureStatusInBulkUploadSignatureRequest(int Type, int JobDocumentId, int BulkUploadDocumentGroupId,int JobId);
		int GetSignatureCompletedStatus(int JobDocId,int DocumentGroupId);
		string GetDocumentPathFromGroupId(int bulkUploadDocumentGroupId = 0, int JobDocumentId = 0);
		List<DocumentWiseSignatureDetails> GetInsertOrUpdateBulkSendDocumentSignatureRequest(int ActionPerformed,
			DateTime? CreatedDate = null, int CreatedBy = 0, DateTime? ModifiedDate = null, int ModifiedBy = 0, bool IsDelete = false, int SentMailStatus = 0, int JobDocumentId = 0
			, DocumentWiseSignatureDetails objDocumentWiseSignatureDetails = null);
		EmailStatusResponce GetEmailStatusByJobDocumentId(int JobDocId);
	}
}

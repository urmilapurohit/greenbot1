using FormBot.BAL.Service.Documents;
using FormBot.DAL;
using FormBot.Entity.Documents;
using FormBot.Entity.DocumentSignatureRequest;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
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
	public class DocumentSignatureRequestBAL : IDocumentSignatureRequestBAL
	{

		public BulkUploadDocumentSignature GetByBulkUploadDocumentSignatureId(int BulkUploadDocumentSignatureId)
		{
			try
			{
				List<SqlParameter> sqlParameters = new List<SqlParameter>();
				sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentSignatureId", SqlDbType.Int, BulkUploadDocumentSignatureId));
				return CommonDAL.ExecuteProcedure<BulkUploadDocumentSignature>("GetById_BulkUploadDocumentSignature", sqlParameters.ToArray()).FirstOrDefault();
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public BulkUploadDocumentSignature GetByJobDocumentId(int JobDocumentId,int GroupId = 0)
		{
			try
			{
				List<SqlParameter> sqlParameters = new List<SqlParameter>();
				sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
				sqlParameters.Add(DBClient.AddParameters("GroupId", SqlDbType.Int, GroupId));
				return CommonDAL.ExecuteProcedure<BulkUploadDocumentSignature>("GetByJobDocumentId_BulkUploadDocumentSignature", sqlParameters.ToArray()).FirstOrDefault();
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		/// <summary>
		/// Insert / Update Groupname and document inside groupname 
		/// </summary>
		/// <param name="GroupName"></param>
		/// <param name="DocumentTemplateId"></param>
		/// <param name="BulkUploadDocumentGroupId"></param>
		/// <returns></returns>
		public int InsertUpdateGroupName(string GroupName, int BulkUploadDocumentGroupId, string groupDocumentPath = "")
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.NVarChar, GroupName));
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentGroupId", SqlDbType.Int, BulkUploadDocumentGroupId));
            sqlParameters.Add(DBClient.AddParameters("GroupDocumentPath", SqlDbType.NVarChar, string.IsNullOrEmpty(groupDocumentPath)? DBNull.Value.ToString() : groupDocumentPath));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
			sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
			sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
			int id = Convert.ToInt32(CommonDAL.ExecuteScalar("InsertUpdateGroupName", sqlParameters.ToArray()));
			return id;
		}


		/// <summary>
		/// List group name in grid and search by group name
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="sortCol"></param>
		/// <param name="sortDir"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public IList<BulkUploadDocumentGroup> GroupNameList(int pageNumber, int pageSize, string sortCol, string sortDir, string groupName)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.NVarChar, string.IsNullOrEmpty(groupName) ? null : groupName));
			sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
			sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
			sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
			sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
			IList<BulkUploadDocumentGroup> lstBulkUploadDocumentGroup = CommonDAL.ExecuteProcedure<BulkUploadDocumentGroup>("GetBulkUploadDocumentGroupList", sqlParameters.ToArray()).ToList();
			return lstBulkUploadDocumentGroup;
		}

		public void DeleteGroupName(string bulkDocumentGroupId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkDocumentGroupId", SqlDbType.NVarChar, bulkDocumentGroupId));
			CommonDAL.ExecuteScalar("BulkDocumentGroupNameDelete", sqlParameters.ToArray());
		}

		public BulkUploadDocumentGroup GetBulkDocumentGroupName(int bulkDocumentGroupId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("bulkDocumentGroupId", SqlDbType.Int, bulkDocumentGroupId));
			BulkUploadDocumentGroup bulkUploadDocumentGroup = CommonDAL.SelectObject<BulkUploadDocumentGroup>("GetBulkDocumentGroupName", sqlParameters.ToArray());
			return bulkUploadDocumentGroup;
		}

        public string GetDocumentPathFromGroupId(int bulkUploadDocumentGroupId = 0,int JobDocumentId = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentGroupId", SqlDbType.Int, bulkUploadDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
			string path = Convert.ToString(CommonDAL.ExecuteScalar("GetDocumentPathFromGroupId", sqlParameters.ToArray()));
            return path;
        }
        public List<BulkUploadDocumentGroup> GetBulkUploadDocumentGroupNameList()
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<BulkUploadDocumentGroup> lstbulkUploadDocumentGroup = CommonDAL.ExecuteProcedure<BulkUploadDocumentGroup>("GetBulkUploadDocumentGroupNameList", sqlParameters.ToArray()).ToList();
			return lstbulkUploadDocumentGroup;
		}

		public List<EmailTemplate> GetEmailTemplateList()
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<EmailTemplate> lstEmailTemplate = CommonDAL.ExecuteProcedure<EmailTemplate>("GetEmailTemplateList", sqlParameters.ToArray()).ToList();
			return lstEmailTemplate;
		}

		public List<DocumentWiseSignatureDetails> GetBulkJobListForSendEmail(int pageNumber, int pageSize, string sortCol, string sortDir, int bulkUploadDocumentGroupId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<DocumentWiseSignatureDetails> lstDocumentWiseSignatureDetails = new List<DocumentWiseSignatureDetails>();
			sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
			sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
			sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
			sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.NVarChar, sortDir));
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentGroupId", SqlDbType.Int, bulkUploadDocumentGroupId));
			lstDocumentWiseSignatureDetails = CommonDAL.ExecuteProcedure<DocumentWiseSignatureDetails>("GetBulkJobListForSendEmail", sqlParameters.ToArray()).ToList();
			return lstDocumentWiseSignatureDetails; 
		}
		public CaptureUserSign GetUserBasicDetailsBySignatureTypeOfJobDocument(int JobType, int JobId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<CaptureUserSign> lstCaptureUserSign = new List<CaptureUserSign>();
			sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
			lstCaptureUserSign = CommonDAL.ExecuteProcedure<CaptureUserSign>("GetUserBasicDetailsBySignatureTypeOfJobDocument", sqlParameters.ToArray()).ToList();
			return lstCaptureUserSign.Any() ? lstCaptureUserSign.FirstOrDefault() : null;
		}

		public List<DocumentWiseSignatureDetails> GetBulkJobListForAddGroup(int pageNumber, int pageSize, string sortCol, string sortDir, string resellerId, string referenceNumber = "", string solarCompanyId = "", int JobId = 0, int JobType = 0,string PVDSWHCode = "")
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<DocumentWiseSignatureDetails> lstDocumentWiseSignatureDetails = new List<DocumentWiseSignatureDetails>();
			sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
			sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
			sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
			sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.NVarChar, sortDir));
			sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, Convert.ToInt32(string.IsNullOrEmpty(solarCompanyId) ? "-1" : solarCompanyId)));
			sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, Convert.ToInt32(resellerId)));
			sqlParameters.Add(DBClient.AddParameters("JobRefNumber", SqlDbType.NVarChar, referenceNumber));
			sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
			sqlParameters.Add(DBClient.AddParameters("PVDSWHCode", SqlDbType.NVarChar, PVDSWHCode));
			lstDocumentWiseSignatureDetails = CommonDAL.ExecuteProcedure<DocumentWiseSignatureDetails>("GetBulkJobListForAddGroup", sqlParameters.ToArray()).ToList();
			return lstDocumentWiseSignatureDetails;
		}

		public DataSet GetDocumentPathFromDocumentTemplateId(int DocumentTemplateId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("DocumentTemplateId", SqlDbType.Int, DocumentTemplateId));
			return CommonDAL.ExecuteDataSet("GetDocumentPathFromDocumentTemplateId", sqlParameters.ToArray());
		}

		public List<DocumentWiseSignatureDetails> ShowJobInGroup(int pageNumber, int pageSize, string sortCol, string sortDir, int bulkUploadDocumentGroupId, string refNumber)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<DocumentWiseSignatureDetails> lstDocumentWiseSignatureDetails = new List<DocumentWiseSignatureDetails>();
			sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
			sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
			sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
			sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.NVarChar, sortDir));
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentGroupId", SqlDbType.Int, bulkUploadDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, refNumber));
			lstDocumentWiseSignatureDetails = CommonDAL.ExecuteProcedure<DocumentWiseSignatureDetails>("ShowJobInGroup", sqlParameters.ToArray()).ToList();
			return lstDocumentWiseSignatureDetails;
		}

		public int InsertUpdateBulkJobForSignatureRequest(int BulkUploadDocumentGroupId, int jobId, string installerSignatureStatus, string DesignerSignatureStatus, string ElectricianSignatureStatus, string HomeOwnerSignatureStatus, string SolarCompanySignatureStatus)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkDocumentGroupId", SqlDbType.Int, BulkUploadDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
			sqlParameters.Add(DBClient.AddParameters("InstallerSignatureStatus", SqlDbType.Bit, installerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("DesignerSignatureStatus", SqlDbType.Bit, DesignerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("ElectricianSignatureStatus", SqlDbType.Bit, ElectricianSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("HomeOwnerSignatureStatus", SqlDbType.Bit, HomeOwnerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("SolarCompanySignatureStatus", SqlDbType.Bit, SolarCompanySignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
			sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
			sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
			int id = Convert.ToInt32(CommonDAL.ExecuteScalar("InsertUpdateBulkJobForSignatureRequest", sqlParameters.ToArray()));
			return id;
		}
		public void AddJobtoGroup(string jobId,string PVDSWHCode, int BulkUploadDocumentGroupId, string InstallerSignatureStatus, string HomeOwnerSignatureStatus, string SolarCompanySignatureStatus, string ElectricianSignatureStatus, string DesignerSignatureStatus)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkDocumentGroupId", SqlDbType.Int, BulkUploadDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, jobId));
			sqlParameters.Add(DBClient.AddParameters("PVDSWHCode", SqlDbType.NVarChar, PVDSWHCode));
			sqlParameters.Add(DBClient.AddParameters("DesignerSignatureStatus", SqlDbType.NVarChar, DesignerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("ElectricianSignatureStatus", SqlDbType.NVarChar, ElectricianSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("HomeOwnerSignatureStatus", SqlDbType.NVarChar, HomeOwnerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("SolarCompanySignatureStatus", SqlDbType.NVarChar, SolarCompanySignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("InstallerSignatureStatus", SqlDbType.NVarChar, InstallerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
			sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
			CommonDAL.ExecuteScalar("AddJobtoGroup", sqlParameters.ToArray());
		}
		public void DeleteJobInGroup(int bulkDocumentGroupId, string JobId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkDocumentGroupId", SqlDbType.Int, bulkDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, JobId));
			CommonDAL.ExecuteScalar("DeleteJobInGroup", sqlParameters.ToArray());
		}
		public List<DocumentSignatureStatusWithEmailResponce> GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(int DocumentGroupId, int JobId , bool isMessageEmail,int JobDocId = 0)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<DocumentSignatureStatusWithEmailResponce> lstDocumentSignatureStatusWithEmailResponce = new List<DocumentSignatureStatusWithEmailResponce>();
			sqlParameters.Add(DBClient.AddParameters("DocumentGroupId", SqlDbType.Int, DocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("IsMessageEmail", SqlDbType.Bit, isMessageEmail));
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, JobDocId));
			lstDocumentSignatureStatusWithEmailResponce = CommonDAL.ExecuteProcedure<DocumentSignatureStatusWithEmailResponce>("GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId", sqlParameters.ToArray()).ToList();
			return lstDocumentSignatureStatusWithEmailResponce;
		}
		public void UpdateSentMailStatusForBulkUploadSignature(int bulkDocumentGroupId, int JobId, int EmailStatus,int JobDocumentId = 0)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("BulkDocumentGroupId", SqlDbType.Int, bulkDocumentGroupId));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
			sqlParameters.Add(DBClient.AddParameters("SentEmailStatus", SqlDbType.Int, EmailStatus));
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
			CommonDAL.ExecuteScalar("UpdateSentMailStatusForBulkUploadSignature", sqlParameters.ToArray());
		}
		public int AddMessageForJobDocument(JobDocumentMessage objJobDocumentMessage)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, objJobDocumentMessage.JobDocId));
			sqlParameters.Add(DBClient.AddParameters("TypeId", SqlDbType.NVarChar, objJobDocumentMessage.TypeId));
			sqlParameters.Add(DBClient.AddParameters("Message", SqlDbType.NVarChar, objJobDocumentMessage.Message));
			sqlParameters.Add(DBClient.AddParameters("MessageCategory", SqlDbType.Int, objJobDocumentMessage.MessageCategory));
			sqlParameters.Add(DBClient.AddParameters("CreatedOn", SqlDbType.DateTime, DateTime.Now));
			sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, 0));
			int id = Convert.ToInt32(CommonDAL.ExecuteScalar("AddMessageForJobDocument", sqlParameters.ToArray()));
			return id;
		}
		public List<JobDocumentMessage> GetAllMessageByJobDocumentIdWise(int JobDocId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<JobDocumentMessage> lstJobDocumentMessage = new List<JobDocumentMessage>();
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocId));
			lstJobDocumentMessage = CommonDAL.ExecuteProcedure<JobDocumentMessage>("GetAllMessageByJobDocumentIdWise", sqlParameters.ToArray()).ToList();
			return lstJobDocumentMessage;
		}
		public DocumentWiseSignatureDetails UpdateSignatureStatusInBulkUploadSignatureRequest(int Type, int JobDocumentId, int BulkUploadDocumentGroupId, int JobId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, (Type == TypeOfSignature.Installer.GetHashCode() ? true : false)));
			sqlParameters.Add(DBClient.AddParameters("IsElectrician", SqlDbType.Bit, (Type == TypeOfSignature.Electrician.GetHashCode() ? true : false)));
			sqlParameters.Add(DBClient.AddParameters("IsDesigner", SqlDbType.Bit, (Type == TypeOfSignature.Designer.GetHashCode() ? true : false)));
			sqlParameters.Add(DBClient.AddParameters("IsHomeOwner", SqlDbType.Bit, (Type == TypeOfSignature.Home_Owner.GetHashCode() ? true : false)));
			sqlParameters.Add(DBClient.AddParameters("IsSolarCompnay", SqlDbType.Bit, (Type == TypeOfSignature.SolarCompnay.GetHashCode() ? true : false)));
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
			//sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
			sqlParameters.Add(DBClient.AddParameters("BulkUploadDocumentGroupId", SqlDbType.Int, BulkUploadDocumentGroupId));
			return CommonDAL.ExecuteProcedure<DocumentWiseSignatureDetails>("UpdateSignatureStatusInBulkUploadSignatureRequest", sqlParameters.ToArray()).FirstOrDefault();
		}
		public int GetSignatureCompletedStatus(int JobDocId,int DocumentGroupId = 0)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<JobDocumentMessage> lstJobDocumentMessage = new List<JobDocumentMessage>();
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocId));
			sqlParameters.Add(DBClient.AddParameters("DocumentGroupId", SqlDbType.Int, DocumentGroupId));
			return Convert.ToInt32(CommonDAL.ExecuteScalar("GetSignatureCompletedStatus", sqlParameters.ToArray()));
		}
		public List<DocumentWiseSignatureDetails> GetInsertOrUpdateBulkSendDocumentSignatureRequest(int ActionPerformed,
			DateTime? CreatedDate = null, int CreatedBy = 0, DateTime? ModifiedDate = null, int ModifiedBy = 0,bool IsDelete = false,int SentMailStatus = 0,int JobDocumentId = 0
			, DocumentWiseSignatureDetails objDocumentWiseSignatureDetails = null)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<DocumentWiseSignatureDetails> lstJobDocumentMessage = new List<DocumentWiseSignatureDetails>();
			sqlParameters.Add(DBClient.AddParameters("ActionPerformed", SqlDbType.Int, ActionPerformed));
			sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, objDocumentWiseSignatureDetails?.JobId));
			sqlParameters.Add(DBClient.AddParameters("InstallerSignatureStatus", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.InstallerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("DesignerSignatureStatus", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.DesignerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("ElectricianSignatureStatus", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.ElectricianSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("HomeOwnerSignatureStatus", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.HomeOwnerSignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("SolarCompanySignatureStatus", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.SolarCompanySignatureStatus));
			sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
			sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy));
			sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, ModifiedDate));
			sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ModifiedBy));
			sqlParameters.Add(DBClient.AddParameters("IsDelete", SqlDbType.Bit, IsDelete));
			sqlParameters.Add(DBClient.AddParameters("IsApplicable", SqlDbType.Bit, objDocumentWiseSignatureDetails?.IsApplicable));
			sqlParameters.Add(DBClient.AddParameters("SentMailStatus", SqlDbType.Int, SentMailStatus));
			sqlParameters.Add(DBClient.AddParameters("PVDSWHCode", SqlDbType.NVarChar, objDocumentWiseSignatureDetails?.PVDSWHCode));
			sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
			lstJobDocumentMessage = CommonDAL.ExecuteProcedure<DocumentWiseSignatureDetails>("GetInsertOrUpdateBulkSendDocumentSignatureRequest", sqlParameters.ToArray()).ToList();
			return lstJobDocumentMessage;
		}
		public EmailStatusResponce GetEmailStatusByJobDocumentId(int JobDocId)
		{
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("JobDocId", SqlDbType.Int, JobDocId));
			EmailStatusResponce objEmailStatusResponce = CommonDAL.SelectObject<EmailStatusResponce>("GetEmailStatusByJobDocumentId", sqlParameters.ToArray());
			return objEmailStatusResponce;
		}

	}
	#region Custom Classes
	public class JobDocumentSignatureDetails : CaptureUserSign
	{
		public JobDocumentSignatureDetails()
		{
			lstJobDocumentMessage = new List<JobDocumentMessage>();
			lsttype = new List<int>();
		}
		public int jobid { get; set; }
		public int jobDocumentId { get; set; }
		public string jobDocumentPath { get; set; }
		public int type { get; set; }
		public string Base64 { get; set; }
		public bool IsImage { get; set; }
		public List<int> lsttype { get; set; }
		public List<JobDocumentMessage> lstJobDocumentMessage { get; set; }
	}
	#endregion
}

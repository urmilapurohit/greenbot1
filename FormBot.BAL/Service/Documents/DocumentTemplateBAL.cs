using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.DAL;
using FormBot.Entity.Documents;
using System.Web.Mvc;
using FormBot.Entity.Job;
using FormBot.Helper;

namespace FormBot.BAL.Service.Documents
{
    /// <summary>
    /// Document Template Class
    /// </summary>
    public class DocumentTemplateBAL : IDocumentTemplateBAL
    {
        public IList<DocumentTemplate> DocumentTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string docTemplateName, string companyIds, int userTypeId, int jobType = 0, int folderTypeId = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("DocTemplateName", SqlDbType.NVarChar, string.IsNullOrEmpty(docTemplateName) ? null : docTemplateName));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, companyIds));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, jobType));
            sqlParameters.Add(DBClient.AddParameters("FolderTypeId", SqlDbType.Int, folderTypeId));
            IList<DocumentTemplate> lstDocumentTemplate = CommonDAL.ExecuteProcedure<DocumentTemplate>("DocumentTemplate_ListBySolarCompanyId", sqlParameters.ToArray()).ToList();
            return lstDocumentTemplate;
        }


        public IList<DocumentTemplate> DocumentTemplateList(int solarCompanyID, int userTypeId, int jobType = 0, int folderTypeId = 0) {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, jobType));
            sqlParameters.Add(DBClient.AddParameters("FolderTypeId", SqlDbType.Int, folderTypeId));
            IList<DocumentTemplate> lstDocumentTemplate = CommonDAL.ExecuteProcedure<DocumentTemplate>("DocumentTemplate_ListBySolarCompanyId_ForCreateJob", sqlParameters.ToArray()).ToList();
            return lstDocumentTemplate;
        }

        /// <summary>
        /// Documents the template insert update.
        /// </summary>
        /// <param name="docTemplate">The document template.</param>
        /// <returns></returns>
        public int DocumentTemplateInsertUpdate(DocumentTemplate docTemplate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DocumentTemplateId", SqlDbType.Int, docTemplate.DocumentTemplateId));
            sqlParameters.Add(DBClient.AddParameters("DocumentTemplateName", SqlDbType.NVarChar, docTemplate.DocumentTemplateName));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, docTemplate.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, docTemplate.Path));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, docTemplate.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, docTemplate.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, docTemplate.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, docTemplate.ModifiedDate));
            sqlParameters.Add(DBClient.AddParameters("StateID", SqlDbType.Int, docTemplate.StateID));
            sqlParameters.Add(DBClient.AddParameters("JobTypeID", SqlDbType.Int, docTemplate.JobTypeID));            
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, docTemplate.IsDefault));
            //sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, solarCompanyIds));

            object documentTemplateID = CommonDAL.ExecuteScalar("DocumentTemplate_InsertUpdate_With_IsDefault", sqlParameters.ToArray());
            return Convert.ToInt32(documentTemplateID);
        }

        /// <summary>
        /// Gets the document template.
        /// </summary>
        /// <param name="docTemplateID">The document template identifier.</param>
        /// <returns></returns>
        public DocumentTemplate GetDocumentTemplate(int docTemplateID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DocumentTemplateId", SqlDbType.Int, docTemplateID));
            DocumentTemplate documentTemplate = CommonDAL.SelectObject<DocumentTemplate>("DocumentTemplate_GetDocumentTemplate", sqlParameters.ToArray());
            return documentTemplate;
        }

        /// <summary>
        /// Deletes the document template.
        /// </summary>
        /// <param name="docTemplateIds">The document template ids.</param>
        public void DeleteDocumentTemplate(string docTemplateIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("docTemplateIds", SqlDbType.NVarChar, docTemplateIds));
            CommonDAL.Crud("DocumentTemplate_Delete", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets all document template.
        /// </summary>
        /// <param name="jobTypeId">The job type identifier.</param>
        /// <returns></returns>
        public List<DocumentTemplate> GetAllDocumentTemplate(int jobTypeId, int solarCompanyId, bool isSTC, bool isCES)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobTypeId", SqlDbType.Int, jobTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsSTC", SqlDbType.Bit, isSTC));
            sqlParameters.Add(DBClient.AddParameters("IsCES", SqlDbType.Bit, isCES));
            List<DocumentTemplate> lstDocumentTemplate = CommonDAL.ExecuteProcedure<DocumentTemplate>("GetAllDocumentTemplate", sqlParameters.ToArray()).ToList();
            return lstDocumentTemplate;
        }

        /// <summary>
        /// Gets the export value by job field identifier.
        /// </summary>
        /// <param name="propertyName">The job field value.</param>
        /// <returns></returns>
        public List<JobFieldExportValue> GetExportValueByJobFieldId(string propertyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PropertyName", SqlDbType.NVarChar, propertyName));
            List<JobFieldExportValue> lstExportValue = CommonDAL.ExecuteProcedure<JobFieldExportValue>("GetExportValueByJobFieldId", sqlParameters.ToArray()).ToList();
            return lstExportValue;
        }

        public void RenameDocumentTemplateName(int docTemplateId, string filename)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DocTemplateId", SqlDbType.Int, docTemplateId));
            sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, filename));
            CommonDAL.ExecuteScalar("RenameDocumentTemplateName", sqlParameters.ToArray());

        }
    }
}

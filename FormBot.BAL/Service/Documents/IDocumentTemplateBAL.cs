using FormBot.Entity.Documents;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.BAL.Service.Documents
{
    /// <summary>
    /// Document Template Interface
    /// </summary>
    public interface IDocumentTemplateBAL
    {
        /// <summary>
        /// Documents the template list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="docTemplateName">Name of the document template.</param>
        /// <param name="solarCompanyId">The company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <returns></returns>
        IList<DocumentTemplate> DocumentTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string docTemplateName, string companyIds, int userTypeId, int jobType = 0, int folderTypeId = 0);

        /// <summary>
        /// Get document for createjob
        /// </summary>
        /// <param name="solarCompanyID"></param>
        /// <param name="userTypeId"></param>
        /// <param name="jobType"></param>
        /// <param name="folderTypeId"></param>
        /// <returns>list of document template</returns>
        IList<DocumentTemplate> DocumentTemplateList(int solarCompanyID, int userTypeId, int jobType = 0, int folderTypeId = 0);

        /// <summary>
        /// Documents the template insert update.
        /// </summary>
        /// <param name="docTemplate">The document template.</param>
        /// <returns></returns>
        int DocumentTemplateInsertUpdate(DocumentTemplate docTemplate);
        ////int DocumentTemplateInsertUpdate(DocumentTemplate docTemplate, int userTypeId, string solarCompanyIds);

        /// <summary>
        /// Gets the document template.
        /// </summary>
        /// <param name="docTemplateID">The document template identifier.</param>
        /// <returns></returns>
        DocumentTemplate GetDocumentTemplate(int docTemplateID);

        /// <summary>
        /// Deletes the document template.
        /// </summary>
        /// <param name="docTemplateIds">The document template ids.</param>
        void DeleteDocumentTemplate(string docTemplateIds);

        /// <summary>
        /// Gets all document template.
        /// </summary>
        /// <param name="jobTypeId">The job type identifier.</param>
        /// <returns></returns>
        List<DocumentTemplate> GetAllDocumentTemplate(int jobTypeId, int solarCompanyId, bool isSTC, bool isCES);

        /// <summary>
        /// Gets the export value by job field identifier.
        /// </summary>
        /// <param name="propertyName">The job field value.</param>
        /// <returns></returns>
        List<JobFieldExportValue> GetExportValueByJobFieldId(string propertyName);

        /// <summary>
        /// Rename document template name
        /// </summary>
        /// <param name="docTemplateId"></param>
        /// <param name="filename"></param>
        void RenameDocumentTemplateName(int docTemplateId, string filename);
    }
}

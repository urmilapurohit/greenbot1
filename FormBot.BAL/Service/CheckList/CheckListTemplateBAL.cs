using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity.CheckList;
using System.Linq;
using System;
using FormBot.Helper;

namespace FormBot.BAL.Service.CheckList
{
    public class CheckListTemplateBAL : ICheckListTemplateBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        public List<CheckListTemplate> GetData(int? solarCompanyId, int userTypeId, int JobType)
        {
            string spName = "[CheckListTemplate_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            IList<CheckListTemplate> checkListTemplate = CommonDAL.ExecuteProcedure<CheckListTemplate>(spName, sqlParameters.ToArray());
            return checkListTemplate.ToList();
        }

        public int CheckListTemplateInsertUpdate(CheckListTemplate checkListTemplate, int CopyOfCheckListTemplateId, int deletedItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, checkListTemplate.CheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("CopyOfCheckListTemplateId", SqlDbType.Int, CopyOfCheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("DeletedItemId", SqlDbType.Int, deletedItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateName", SqlDbType.NVarChar, checkListTemplate.CheckListTemplateName));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, checkListTemplate.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, checkListTemplate.IsDefault));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemIds", SqlDbType.NVarChar, checkListTemplate.VisitCheckListItemIds));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, checkListTemplate.isSetFromSetting));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.TinyInt, checkListTemplate.JobType));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            object checkListTemplateId = CommonDAL.ExecuteScalar("CheckListTemplate_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(checkListTemplateId);
        }

        public void CheckListTemplateDelete(string templateIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateIds", SqlDbType.NVarChar, templateIds));
            CommonDAL.Crud("CheckListTemplate_Delete", sqlParameters.ToArray());
        }

        public IList<CheckListTemplate> CheckListTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int solarCompanyId, int userTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            IList<CheckListTemplate> lstCheckListTemplate = CommonDAL.ExecuteProcedure<CheckListTemplate>("CheckListTemplate_ListBySolarCompanyId", sqlParameters.ToArray()).ToList();
            return lstCheckListTemplate;
        }   

        public CheckListTemplate GetCheckListTemplate(int checkListTemplateId)
        {
            CheckListTemplate checkListTemplate = new CheckListTemplate();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            DataSet dscheckListTemplate = CommonDAL.ExecuteDataSet("GetCheckListTemplate", sqlParameters.ToArray());
            if (dscheckListTemplate != null && dscheckListTemplate.Tables.Count > 0 && dscheckListTemplate.Tables[0] != null && dscheckListTemplate.Tables[0].Rows.Count > 0)
            {
                checkListTemplate = DBClient.DataTableToList<CheckListTemplate>(dscheckListTemplate.Tables[0])[0];
            }
            return checkListTemplate;
        }
    }
}

using FormBot.DAL;
using FormBot.Entity.VEECCheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public class VEECCheckListTemplateBAL : IVEECCheckListTemplateBAL
    {
        public IList<VEECCheckListTemplate> CheckListTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int? solarCompanyId, int userTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            IList<VEECCheckListTemplate> lstCheckListTemplate = CommonDAL.ExecuteProcedure<VEECCheckListTemplate>("VEECCheckListTemplate_ListBySolarCompanyId", sqlParameters.ToArray()).ToList();
            return lstCheckListTemplate;
        }

        public VEECCheckListTemplate GetCheckListTemplate(int checkListTemplateId)
        {
            VEECCheckListTemplate checkListTemplate = new VEECCheckListTemplate();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            DataSet dscheckListTemplate = CommonDAL.ExecuteDataSet("GetVEECCheckListTemplate", sqlParameters.ToArray());
            if (dscheckListTemplate != null && dscheckListTemplate.Tables.Count > 0 && dscheckListTemplate.Tables[0] != null && dscheckListTemplate.Tables[0].Rows.Count > 0)
            {
                checkListTemplate = DBClient.DataTableToList<VEECCheckListTemplate>(dscheckListTemplate.Tables[0])[0];
            }
            return checkListTemplate;
        }


        public void VEECCheckListTemplateDelete(string templateIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateIds", SqlDbType.NVarChar, templateIds));
            CommonDAL.Crud("VEECCheckListTemplate_Delete", sqlParameters.ToArray());
        }

        public int VEECCheckListTemplateInsertUpdate(VEECCheckListTemplate checkListTemplate, int CopyOfCheckListTemplateId, int deletedItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, checkListTemplate.VEECCheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("CopyOfCheckListTemplateId", SqlDbType.Int, CopyOfCheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("DeletedItemId", SqlDbType.Int, deletedItemId));
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateName", SqlDbType.NVarChar, checkListTemplate.VEECCheckListTemplateName));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, checkListTemplate.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, checkListTemplate.IsDefault));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemIds", SqlDbType.NVarChar, checkListTemplate.VisitCheckListItemIds));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, checkListTemplate.isSetFromSetting));           

            object checkListTemplateId = CommonDAL.ExecuteScalar("VEECCheckListTemplate_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(checkListTemplateId);
        }

        public List<VEECCheckListTemplate> GetData(int? solarCompanyId, int userTypeId)
        {
            string spName = "[VEECCheckListTemplate_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            IList<VEECCheckListTemplate> checkListTemplate = CommonDAL.ExecuteProcedure<VEECCheckListTemplate>(spName, sqlParameters.ToArray());
            return checkListTemplate.ToList();
        }
    }
}

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
    public class CheckListItemBAL : ICheckListItemBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        public List<CheckListItem> GetData(int checkListTemplateId)
        {
            string spName = "[CheckListItem_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            IList<CheckListItem> checkListItem = CommonDAL.ExecuteProcedure<CheckListItem>(spName, sqlParameters.ToArray());
            return checkListItem.ToList();
        }

        public CheckListTemplate GetCheckListItemByTemplateId(int templateId, int jobSchedulingId, bool isSetFromSetting, bool isTemplateChange, Int64 tempJobSchedulingId, int jobId, string visitCheckListIdsString)
        {
            CheckListTemplate checkListTemplate = new CheckListTemplate();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, templateId));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            sqlParameters.Add(DBClient.AddParameters("IsTemplateChange", SqlDbType.Bit, isTemplateChange));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, tempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListIdsString", SqlDbType.NVarChar, visitCheckListIdsString));            

            DataSet dscheckListTemplate = CommonDAL.ExecuteDataSet("GetCheckListItemByTemplateId", sqlParameters.ToArray());
            if (dscheckListTemplate != null && dscheckListTemplate.Tables.Count > 0)
            {
                if (dscheckListTemplate.Tables[1] != null && dscheckListTemplate.Tables[1].Rows.Count > 0)
                {
                    checkListTemplate = dscheckListTemplate.Tables[1].ToListof<CheckListTemplate>().FirstOrDefault();
                }
                if (dscheckListTemplate.Tables[0] != null && dscheckListTemplate.Tables[0].Rows.Count > 0)
                {
                    checkListTemplate.lstCheckListItem = dscheckListTemplate.Tables[0].ToListof<CheckListItem>();
                }
            }
            return checkListTemplate;
        }

        public int CheckListItemInsertUpdate(CheckListItem checkListItem, bool isSetFromSetting, int? jobSchedulingId, int jobId, bool isTempItemAdd)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("IsTempItemAdd", SqlDbType.Bit, isTempItemAdd));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, checkListItem.TempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, checkListItem.VisitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, checkListItem.CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, checkListItem.CheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("CheckListClassTypeId", SqlDbType.Int, checkListItem.CheckListClassTypeId));
            sqlParameters.Add(DBClient.AddParameters("ItemName", SqlDbType.NVarChar, checkListItem.ItemName));
            sqlParameters.Add(DBClient.AddParameters("FolderName", SqlDbType.NVarChar, checkListItem.FolderName));
            sqlParameters.Add(DBClient.AddParameters("TotalNumber", SqlDbType.Int, checkListItem.TotalNumber));
            sqlParameters.Add(DBClient.AddParameters("IsSameAsTotalPanelAmount", SqlDbType.Bit, checkListItem.IsSameAsTotalPanelAmount));
            sqlParameters.Add(DBClient.AddParameters("IsAtLeastOne", SqlDbType.Bit, checkListItem.IsAtLeastOne));

            sqlParameters.Add(DBClient.AddParameters("IsCustomSerialNumField", SqlDbType.Bit, checkListItem.IsCustomSerialNumField));
            sqlParameters.Add(DBClient.AddParameters("IsNoneFieldMap", SqlDbType.Bit, checkListItem.IsNoneFieldMap));
            sqlParameters.Add(DBClient.AddParameters("JobFieldId", SqlDbType.Int, checkListItem.JobFieldId));
            //sqlParameters.Add(DBClient.AddParameters("SerialFieldName", SqlDbType.NVarChar, checkListItem.SerialFieldName));
            sqlParameters.Add(DBClient.AddParameters("CustomFieldId", SqlDbType.Int, checkListItem.CustomFieldId));
            sqlParameters.Add(DBClient.AddParameters("SeparatorId", SqlDbType.Int, checkListItem.SeparatorId));
            sqlParameters.Add(DBClient.AddParameters("SerialNumTitle", SqlDbType.NVarChar, checkListItem.SerialNumTitle));
            sqlParameters.Add(DBClient.AddParameters("IsSaveCopyofSerialNum", SqlDbType.Bit, checkListItem.IsSaveCopyofSerialNum));
            sqlParameters.Add(DBClient.AddParameters("SerialNumFileName", SqlDbType.NVarChar, checkListItem.SerialNumFileName));

            sqlParameters.Add(DBClient.AddParameters("IsOwnerSignature", SqlDbType.Bit, checkListItem.IsOwnerSignature));
            sqlParameters.Add(DBClient.AddParameters("IsInstallerSignature", SqlDbType.Bit, checkListItem.IsInstallerSignature));
            sqlParameters.Add(DBClient.AddParameters("IsDesignerSignature", SqlDbType.Bit, checkListItem.IsDesignerSignature));
            sqlParameters.Add(DBClient.AddParameters("IsElectricianSignature", SqlDbType.Bit, checkListItem.IsElectricianSignature));
            sqlParameters.Add(DBClient.AddParameters("IsOtherSignature", SqlDbType.Bit, checkListItem.IsOtherSignature));
            sqlParameters.Add(DBClient.AddParameters("OtherSignName", SqlDbType.NVarChar, checkListItem.OtherSignName));
            sqlParameters.Add(DBClient.AddParameters("OtherSignLabel", SqlDbType.NVarChar, checkListItem.OtherSignLabel));

            sqlParameters.Add(DBClient.AddParameters("CaptureUploadImagePDFName", SqlDbType.NVarChar, checkListItem.CaptureUploadImagePDFName));
            sqlParameters.Add(DBClient.AddParameters("PDFLocationId", SqlDbType.Int, checkListItem.PDFLocationId));
            sqlParameters.Add(DBClient.AddParameters("IsLinkToDocument", SqlDbType.Bit, checkListItem.IsLinkToDocument));
            sqlParameters.Add(DBClient.AddParameters("LinkedDocumentId", SqlDbType.Int, checkListItem.LinkedDocumentId));
            sqlParameters.Add(DBClient.AddParameters("IsCompletedCustomCheckListItem", SqlDbType.Bit, checkListItem.IsCompletedCustomCheckListItem));
            sqlParameters.Add(DBClient.AddParameters("PhotoQualityId", SqlDbType.Int, checkListItem.PhotoQualityId));

            sqlParameters.Add(DBClient.AddParameters("OrderNumber", SqlDbType.Int, checkListItem.OrderNumber));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CheckListPhotoTypeId", SqlDbType.Int, checkListItem.CheckListPhotoTypeId));
            sqlParameters.Add(DBClient.AddParameters("AllowUploadPhotoFromGallary", SqlDbType.Bit, checkListItem.AllowUploadPhotoFromGallary));
            sqlParameters.Add(DBClient.AddParameters("SelfieTypeId", SqlDbType.Int, checkListItem.SelfieTypeId));
            object checkListItemId = CommonDAL.ExecuteScalar("CheckListItem_InsertUpdate", sqlParameters.ToArray());

            return Convert.ToInt32(checkListItemId);
        }

        public CheckListItem GetCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, visitCheckListItemId));
            CheckListItem checkListItem = CommonDAL.ExecuteProcedure<CheckListItem>("GetCheckListItemByItemId", sqlParameters.ToArray()).FirstOrDefault();
            return checkListItem;
        }

        public void DeleteCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, visitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.Crud("DeleteCheckListItemByItemId", sqlParameters.ToArray());
        }

        public void MarkUnMarkCheckListItem(int CheckListItemId, bool isCompleted, int jobSchedulingId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("IsCompleted", SqlDbType.Bit, isCompleted));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            CommonDAL.Crud("CheckListItem_MarkUnMarkAsCompleted", sqlParameters.ToArray());
        }

        public void ChangeOrderOfCheckListItem(int CheckListItemId, int CheckListTemplateId, int sourceOrder, int targetOrder)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, CheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("SourceOrder", SqlDbType.Int, sourceOrder));
            sqlParameters.Add(DBClient.AddParameters("TargerOrder", SqlDbType.Int, targetOrder));
            CommonDAL.Crud("CheckListItem_ChangeOrder", sqlParameters.ToArray());
        }

        public void MoveUPAndDownOrderOfCheckListItem(int CheckListTemplateId, bool isMoveUp)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, CheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("IsMoveUp", SqlDbType.Bit, isMoveUp));
            CommonDAL.Crud("CheckListItem_UpDownOrder", sqlParameters.ToArray());
        }


        public void MoveUPAndDownOrderOfCheckListItemNew(string strData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("strData", SqlDbType.VarChar, strData));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("CheckListItem_UpDownOrder_New", sqlParameters.ToArray());
        }

        public DataSet GetCheckListItemsByJobScheduleId(string Id)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.VarChar, Id));
            return CommonDAL.ExecuteDataSet("GetCheckListItemsByJobScheduleId", sqlParameters.ToArray());

        }

        public DataSet TempCheckListTemplateItemAdd(int checkListTemplateId, Int64 tempJobSchedulingId, int? jobSchedulingId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, tempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            return CommonDAL.ExecuteDataSet("TempCheckListTemplateItemAdd", sqlParameters.ToArray());

        }

        public void DeleteTempVisitCheckListItem()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            CommonDAL.Crud("DeleteTempVisitCheckListItem", sqlParameters.ToArray());
        }

        public DataSet GetDefaultCheckListTemplateId(int JobType,int SolarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            DataSet ds = CommonDAL.ExecuteDataSet("CheckListItem_GetDefaultCheckListTemplateId", sqlParameters.ToArray());
            return ds;
            //object checkListItemId = CommonDAL.ExecuteScalar("CheckListItem_GetDefaultCheckListTemplateId", sqlParameters.ToArray());
            //return Convert.ToString(checkListItemId);
        }

        public void SaveDefaultChecklistTemplate(bool isDefault, int jobTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, jobTypeId));
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, isDefault));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteScalar("SaveDefaultChecklistTemplate", sqlParameters.ToArray());
        }
    }
}

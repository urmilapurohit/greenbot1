using FormBot.DAL;
using FormBot.Entity.CheckList;
using FormBot.Entity.VEECCheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECCheckList
{
    class VECCCheckListItemBAL : IVEECCheckListItemBAL
    {
        public List<VEECCheckListItem> GetData(int checkListTemplateId)
        {
            string spName = "[VEECCheckListItem_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            IList<VEECCheckListItem> checkListItem = CommonDAL.ExecuteProcedure<VEECCheckListItem>(spName, sqlParameters.ToArray());
            return checkListItem.ToList();
        }


        public DataSet TempVEECCheckListTemplateItemAdd(int checkListTemplateId, Int64 tempJobSchedulingId, int? jobSchedulingId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, checkListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("TempVEECSchedulingId", SqlDbType.BigInt, tempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VEECSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            return CommonDAL.ExecuteDataSet("TempVEECCheckListTemplateItemAdd", sqlParameters.ToArray());

        }
        public VEECCheckListTemplate GetCheckListItemByTemplateId(int templateId, int jobSchedulingId, bool isSetFromSetting, bool isTemplateChange, Int64 tempJobSchedulingId, int veecid, string visitCheckListIdsString)
        {
            VEECCheckListTemplate checkListTemplate = new VEECCheckListTemplate();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, templateId));
            sqlParameters.Add(DBClient.AddParameters("VEECJobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            sqlParameters.Add(DBClient.AddParameters("IsTemplateChange", SqlDbType.Bit, isTemplateChange));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, tempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, veecid));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListIdsString", SqlDbType.NVarChar, visitCheckListIdsString));

            DataSet dscheckListTemplate = CommonDAL.ExecuteDataSet("GetVEECCheckListItemByTemplateId", sqlParameters.ToArray());
            if (dscheckListTemplate != null && dscheckListTemplate.Tables.Count > 0)
            {
                if (dscheckListTemplate.Tables[1] != null && dscheckListTemplate.Tables[1].Rows.Count > 0)
                {
                    checkListTemplate = dscheckListTemplate.Tables[1].ToListof<VEECCheckListTemplate>().FirstOrDefault();
                }
                if (dscheckListTemplate.Tables[0] != null && dscheckListTemplate.Tables[0].Rows.Count > 0)
                {
                    checkListTemplate.lstCheckListItem = dscheckListTemplate.Tables[0].ToListof<VEECCheckListItem>();
                }
            }
            return checkListTemplate;
        }


        public void MoveUPAndDownOrderOfCheckListItem(int CheckListTemplateId, bool isMoveUp)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, CheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("IsMoveUp", SqlDbType.Bit, isMoveUp));
            CommonDAL.Crud("VEECCheckListItem_UpDownOrder", sqlParameters.ToArray());
        }


        public void MoveUPAndDownOrderOfCheckListItemNew(string strData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("strData", SqlDbType.VarChar, strData));

            CommonDAL.Crud("VEECCheckListItem_UpDownOrder_New", sqlParameters.ToArray());
        }

        public VEECCheckListItem GetCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, visitCheckListItemId));
            VEECCheckListItem checkListItem = CommonDAL.ExecuteProcedure<VEECCheckListItem>("GetVEECCheckListItemByItemId", sqlParameters.ToArray()).FirstOrDefault();
            return checkListItem;
        }

        public void DeleteCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListItemId", SqlDbType.Int, CheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VEECVisitCheckListItemId", SqlDbType.Int, visitCheckListItemId));
            CommonDAL.Crud("DeleteVEECCheckListItemByItemId", sqlParameters.ToArray());
        }

        public int VEECCheckListItemInsertUpdate(VEECCheckListItem checkListItem, bool isSetFromSetting, int? jobSchedulingId, int jobId, bool isTempItemAdd)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("IsTempItemAdd", SqlDbType.Bit, isTempItemAdd));
            sqlParameters.Add(DBClient.AddParameters("TempJobSchedulingId", SqlDbType.BigInt, checkListItem.TempJobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("VEECVisitCheckListItemId", SqlDbType.Int, checkListItem.VEECVisitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListItemId", SqlDbType.Int, checkListItem.VEECCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListTemplateId", SqlDbType.Int, checkListItem.VEECCheckListTemplateId));
            sqlParameters.Add(DBClient.AddParameters("VEECCheckListClassTypeId", SqlDbType.Int, checkListItem.VEECCheckListClassTypeId));
            sqlParameters.Add(DBClient.AddParameters("ItemName", SqlDbType.NVarChar, checkListItem.ItemName));
            sqlParameters.Add(DBClient.AddParameters("FolderName", SqlDbType.NVarChar, checkListItem.FolderName));
            sqlParameters.Add(DBClient.AddParameters("TotalNumber", SqlDbType.Int, checkListItem.TotalNumber));
            sqlParameters.Add(DBClient.AddParameters("IsSameAsTotalPanelAmount", SqlDbType.Bit, checkListItem.IsSameAsTotalPanelAmount));
            sqlParameters.Add(DBClient.AddParameters("IsAtLeastOne", SqlDbType.Bit, checkListItem.IsAtLeastOne));

            sqlParameters.Add(DBClient.AddParameters("IsCustomSerialNumField", SqlDbType.Bit, checkListItem.IsCustomSerialNumField));
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
            object checkListItemId = CommonDAL.ExecuteScalar("VEECCheckListItem_InsertUpdate", sqlParameters.ToArray());

            return Convert.ToInt32(checkListItemId);
        }
    }
}

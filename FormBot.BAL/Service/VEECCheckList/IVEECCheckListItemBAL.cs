using FormBot.BAL.Service.VEECCheckList;
using System;
using System.Collections.Generic;
using FormBot.Entity.VEECCheckList;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity.CheckList;
using System.Data;

namespace FormBot.BAL.Service.VEECCheckList
{
    public interface IVEECCheckListItemBAL
    {
        List<VEECCheckListItem> GetData(int checkListTemplateId);

        VEECCheckListTemplate GetCheckListItemByTemplateId(int templateId, int jobSchedulingId, bool isSetFromSetting, bool isTemplateChange, Int64 tempJobSchedulingId, int jobId, string visitCheckListIdsString);

        VEECCheckListItem GetCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId);

        int VEECCheckListItemInsertUpdate(VEECCheckListItem checkListItem, bool isSetFromSetting, int? jobSchedulingId, int jobId, bool isTempItemAdd);

        void DeleteCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId);

        void MoveUPAndDownOrderOfCheckListItem(int CheckListTemplateId, bool isMoveUp);

        void MoveUPAndDownOrderOfCheckListItemNew(string strData);

        DataSet TempVEECCheckListTemplateItemAdd(int checkListTemplateId, Int64 tempJobSchedulingId, int? jobSchedulingId, int jobId);
    }
}

using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity.CheckList;
using System.Linq;
using System;

namespace FormBot.BAL.Service.CheckList
{
    public interface ICheckListItemBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        List<CheckListItem> GetData(int checkListTemplateId);

        CheckListTemplate GetCheckListItemByTemplateId(int templateId, int jobSchedulingId, bool isSetFromSetting, bool isTemplateChange, Int64 tempJobSchedulingId, int jobId, string visitCheckListIdsString);

        int CheckListItemInsertUpdate(CheckListItem checkListItem, bool isSetFromSetting, int? jobSchedulingId, int jobId, bool isTempItemAdd);

        CheckListItem GetCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId);

        void DeleteCheckListItemByItemId(int CheckListItemId, int visitCheckListItemId);

        void MarkUnMarkCheckListItem(int CheckListItemId, bool isCompleted, int jobSchedulingId);

        void ChangeOrderOfCheckListItem(int CheckListItemId, int CheckListTemplateId, int sourceOrder, int targetOrder);

        void MoveUPAndDownOrderOfCheckListItem(int CheckListTemplateId, bool isMoveUp);

        void MoveUPAndDownOrderOfCheckListItemNew(string strData);

        DataSet GetCheckListItemsByJobScheduleId(string Id);

        DataSet TempCheckListTemplateItemAdd(int checkListTemplateId, Int64 tempJobSchedulingId, int? jobSchedulingId, int jobId);

        void DeleteTempVisitCheckListItem();

        DataSet GetDefaultCheckListTemplateId(int JobType , int SolarCompanyId);

        /// <summary>
        /// Save default checklisttemplate for SCO users 
        /// </summary>
        /// <param name="isDefault"></param>
        /// <param name="jobTypeId"></param>
        void SaveDefaultChecklistTemplate(bool isDefault, int jobTypeId);
    }
}

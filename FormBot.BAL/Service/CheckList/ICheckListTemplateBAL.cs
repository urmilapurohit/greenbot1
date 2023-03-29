using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity.CheckList;
using System.Linq;
using System;


namespace FormBot.BAL.Service.CheckList
{
    public interface ICheckListTemplateBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        List<CheckListTemplate> GetData(int? solarCompanyId,int userTypeId, int JobType);

        int CheckListTemplateInsertUpdate(CheckListTemplate checkListTemplate, int CopyOfCheckListTemplateId, int deletedItemId);

        void CheckListTemplateDelete(string templateIds);

        IList<CheckListTemplate> CheckListTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int solarCompanyId, int userTypeId);

        CheckListTemplate GetCheckListTemplate(int checkListTemplateId);
    }
}

using FormBot.Entity.CheckList;
using FormBot.Entity.VEECCheckList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IVEECCheckListTemplateBAL
    {
        IList<VEECCheckListTemplate> CheckListTemplateList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int? solarCompanyId, int userTypeId);

        void VEECCheckListTemplateDelete(string templateIds);
        int VEECCheckListTemplateInsertUpdate(VEECCheckListTemplate checkListTemplate, int CopyOfCheckListTemplateId, int deletedItemId);

        VEECCheckListTemplate GetCheckListTemplate(int checkListTemplateId);

        List<VEECCheckListTemplate> GetData(int? solarCompanyId, int userTypeId);
    }
}

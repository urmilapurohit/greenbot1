using FormBot.Entity.VEECCheckList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECCheckList
{
    public interface IVECCCheckListClassTypeBAL
    {
        List<VECCCheckListClassType> GetData(bool isSetFromSetting);
    }
}

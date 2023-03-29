using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ICommonRECMethodsBAL
    {
        void ProcessRecData(DataSet ds, DataTable dtReason, bool IsWindowService = false);
    }
}

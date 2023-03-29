using FormBot.DAL;
using FormBot.Entity.VEECCheckList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECCheckList
{
    public class VECCCheckListClassTYpeBAL : IVECCCheckListClassTypeBAL
    {

        public List<VECCCheckListClassType> GetData(bool isSetFromSetting)
        {
            string spName = "[VEECCheckListClassType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            IList<VECCCheckListClassType> checkListClassType = CommonDAL.ExecuteProcedure<VECCCheckListClassType>(spName, sqlParameters.ToArray());
            return checkListClassType.ToList();
        }
    }
}

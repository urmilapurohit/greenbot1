using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity.CheckList;
using System.Linq;
using System;

namespace FormBot.BAL.Service.CheckList
{
    public class CheckListClassTypeBAL : ICheckListClassTypeBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        public List<CheckListClassType> GetData(bool isSetFromSetting)
        {
            string spName = "[CheckListClassType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsSetFromSetting", SqlDbType.Bit, isSetFromSetting));
            IList<CheckListClassType> checkListClassType = CommonDAL.ExecuteProcedure<CheckListClassType>(spName, sqlParameters.ToArray());
            return checkListClassType.ToList();
        }
        /// <summary>
        /// get checklist photo type
        /// </summary>
        /// <returns></returns>
        public List<CheckListPhotoType> GetPhototype()
        {
            string spName = "[CheckListPhotoType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<CheckListPhotoType> CheckListPhotoType = CommonDAL.ExecuteProcedure<CheckListPhotoType>(spName, sqlParameters.ToArray());
            return CheckListPhotoType.ToList();
        }
    }
}

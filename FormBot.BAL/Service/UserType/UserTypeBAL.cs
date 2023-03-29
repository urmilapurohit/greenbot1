using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;

namespace FormBot.BAL.Service
{
    public class UserTypeBAL : IUserTypeBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>user list</returns>
        public List<UserType> GetData()
        {
            string spName = "[UserType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<UserType> userTypeList = CommonDAL.ExecuteProcedure<UserType>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }

    }
}

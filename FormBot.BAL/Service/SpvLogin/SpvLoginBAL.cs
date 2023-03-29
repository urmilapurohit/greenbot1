using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.SpvLogin
{
    public class SpvLoginBAL : ISpvLoginBAL
    {
        /// <summary>
        /// Gets the menu action by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSpvMenuActionByRoleId(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("SpvLogin_GetMenuActionByRoleId", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Updates the last login date.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void UpdateLastLoginDate(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("LoginDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("SpvUser_UpdateLoginDate", sqlParameters.ToArray());
        }
    }
}

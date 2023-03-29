using FormBot.DAL;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public class SpvRoleBAL : ISpvRoleBAL
    {
        /// <summary>
        /// Dynamics the spvmenu binding.
        /// </summary>
        /// <param name="spvroleId">The spvrole identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet DynamicSpvMenuBinding(int spvroleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SpvRoleId", SqlDbType.Int, spvroleId));
            var dsSpvMenus = CommonDAL.ExecuteDataSet("Role_DynamicSpvMenuBinding", sqlParameters.ToArray());
            return dsSpvMenus;
        }

        /// <summary>
        /// Customs the authorization.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <returns>DataSet</returns>
        public DataSet CustomAuthorization(int roleId, string controller, string action)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            sqlParameters.Add(DBClient.AddParameters("Controller", SqlDbType.NVarChar, controller));
            sqlParameters.Add(DBClient.AddParameters("Action", SqlDbType.NVarChar, action));
            var dsMenus = CommonDAL.ExecuteDataSet("SpvRole_CustomAuthorization", sqlParameters.ToArray());
            return dsMenus;
        }
    }
}

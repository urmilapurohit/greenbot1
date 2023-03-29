using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FormBot.Entity;
using FormBot.BAL;

namespace FormBot.BAL.Service
{
    public class FCOUserGroupBAL : IFCOUserGroupBAL
    {
        /// <summary>
        /// Gets the fco user group list.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>
        /// select list
        /// </returns>
        public List<SelectListItem> GetFCOUserGroupList(int FCOGroupId)
        {
            string spName = "[FCOUserGroup_GetAssignedFCOUserListFromUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.Int, FCOGroupId));
            var FCOUserGroup = CommonDAL.ExecuteProcedure<FCOUserGroup>(spName, sqlParameters.ToArray())
            .Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.UserID),
                Text = d.FirstName + " " + d.LastName
            }).ToList();
            return FCOUserGroup;
        }

        /// <summary>
        /// Deletes the specified fco group identifier.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        public void Delete(int FCOGroupId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.Int, FCOGroupId));
            CommonDAL.Crud("FCOUserGroup_Delete", sqlParameters.ToArray());
        }
    }
}

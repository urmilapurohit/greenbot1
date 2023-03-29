using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System.Web.Mvc;
using System;
using FormBot.Helper;

namespace FormBot.BAL.Service
{
    public class FCOGroupBAL : IFCOGroupBAL
    {
        /// <summary>
        /// Creates the specified fco group view.
        /// </summary>
        /// <param name="fcoGroupView">The fco group view.</param>
        /// <returns>group view</returns>
        public int Create(FCOGroup fcoGroupView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupID", SqlDbType.Int, fcoGroupView.FCOGroupId));
            sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.VarChar, fcoGroupView.GroupName));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, fcoGroupView.IsDeleted));
            object lstFCOGroup = CommonDAL.ExecuteScalar("FCOGroup_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(lstFCOGroup);
        }

        /// <summary>
        /// Updates the specified fco group view.
        /// </summary>
        /// <param name="FCOGroupView">The fco group view.</param>
        /// <returns>
        /// group list
        /// </returns>
        public int Update(FCOGroup FCOGroupView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.VarChar, FCOGroupView.GroupName));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, FCOGroupView.IsDeleted));
            object lstFCOGroup = CommonDAL.ExecuteScalar("FCOGroup_Update", sqlParameters.ToArray());
            return Convert.ToInt32(lstFCOGroup);
        }

        /// <summary>
        /// Get FCOGroup By Id
        /// </summary>
        /// <param name="FCOGroupId">FCOGroupId</param>
        /// <returns>DataSet</returns>
        public DataSet GetFCOGroupById(int FCOGroupId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.Int, FCOGroupId));
            DataSet dsFCOGroup = CommonDAL.ExecuteDataSet("FCOGroup_GetFCOGroupById", sqlParameters.ToArray());
            return dsFCOGroup;
        }

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetUserList(int FCOGroupId)
        {
            string spName = "[FCOUserGroup_GetAllUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.NVarChar, FCOGroupId));
            IList<FCOGroup> FCOGroup = CommonDAL.ExecuteProcedure<FCOGroup>(spName, sqlParameters.ToArray());
            return FCOGroup.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.UserID),
                Text = d.FirstName + " " + d.LastName
            }).ToList();
        }

        /// <summary>
        /// Checks the group name exists.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>
        /// object type
        /// </returns>
        public object CheckGroupNameExists(string groupName, int? FCOGroupId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.NVarChar, groupName));
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.NVarChar, FCOGroupId));
            object lstUser = CommonDAL.ExecuteScalar("FCO_CheckGroupNameExist", sqlParameters.ToArray());
            return lstUser;
        }

        /// <summary>
        /// Creates the fco group user.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>
        /// object type
        /// </returns>
        public object CreateFCOGroupUser(int UserID, int FCOGroupId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.NVarChar, UserID));
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.NVarChar, FCOGroupId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, 0));
            CommonDAL.Crud("FCOUserGroup_Insert", sqlParameters.ToArray());
            return null;
        }

        /// <summary>
        /// Gets the fco user group by identifier.
        /// </summary>
        /// <param name="FCOUserGroupId">The fco user group identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetFCOUserGroupById(int FCOUserGroupId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOUserGroupId", SqlDbType.Int, FCOUserGroupId));
            DataSet dsFCOUserGroup = CommonDAL.ExecuteDataSet("FCOUserGroup_GetFCOUserGroupById", sqlParameters.ToArray());
            return dsFCOUserGroup;
        }

        /// <summary>
        /// Fcoes the group list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="AssignedUser">The assigned user.</param>
        /// <returns>
        /// group list
        /// </returns>
        public List<FCOGroup> FCOGroupList(int pageNumber, int pageSize, string sortCol, string sortDir, string groupName, string AssignedUser)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("GroupName", SqlDbType.NVarChar, string.IsNullOrEmpty(groupName) ? null : groupName));
            sqlParameters.Add(DBClient.AddParameters("AssignFCOUser", SqlDbType.NVarChar, string.IsNullOrEmpty(AssignedUser) ? null : AssignedUser));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            var lstRole = CommonDAL.ExecuteProcedure<FCOGroup>("FCOGroup_GetFCOGrouprList_Test", sqlParameters.ToArray()).ToList();
            return lstRole;
        }

        /// <summary>
        /// Gets the fco group drop down.
        /// </summary>
        /// <returns>
        /// group list
        /// </returns>
        public List<FCOGroup> GetFCOGroupDropDown()
        {
            string spName = "[FCOGroup_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<FCOGroup> fcoGroupList = CommonDAL.ExecuteProcedure<FCOGroup>(spName, sqlParameters.ToArray());
            return fcoGroupList.ToList();
        }

        /// <summary>
        /// Delete FCO Group
        /// </summary>
        /// <param name="FCOGroupId">fco id</param>
        public void DeleteFCOGroup(int FCOGroupId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.Int, FCOGroupId));
            CommonDAL.Crud("FCOGroup_UpdateForDelete", sqlParameters.ToArray());
        }
    }
}
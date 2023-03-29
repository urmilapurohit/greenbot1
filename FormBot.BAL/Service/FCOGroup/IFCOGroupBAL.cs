using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public interface IFCOGroupBAL
    {
        /// <summary>
        /// Creates the specified fco group view.
        /// </summary>
        /// <param name="FCOGroupView">The fco group view.</param>
        /// <returns>fco identifier</returns>
        int Create(FCOGroup FCOGroupView);

        /// <summary>
        /// Gets the fco group by identifier.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetFCOGroupById(int FCOGroupId);

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetUserList(int FCOGroupId);

        /// <summary>
        /// Checks the group name exists.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>object type</returns>
        object CheckGroupNameExists(string groupName, int? FCOGroupId = null);

        /// <summary>
        /// Creates the fco group user.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>object type</returns>
        object CreateFCOGroupUser(int UserID, int FCOGroupId);

        /// <summary>
        /// Gets the fco user group by identifier.
        /// </summary>
        /// <param name="FCOUserGroupId">The fco user group identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetFCOUserGroupById(int FCOUserGroupId);

        /// <summary>
        /// Fcoes the group list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="AssignedUser">The assigned user.</param>
        /// <returns>group list</returns>
        List<FCOGroup> FCOGroupList(int pageNumber, int pageSize, string sortCol, string sortDir, string groupName, string AssignedUser);

        /// <summary>
        /// Gets the fco group drop down.
        /// </summary>
        /// <returns>group list</returns>
        List<FCOGroup> GetFCOGroupDropDown();

        /// <summary>
        /// Delete FCO Group
        /// </summary>
        /// <param name="FCOGroupId">fco id</param>
        void DeleteFCOGroup(int FCOGroupId);

        /// <summary>
        /// Updates the specified fco group view.
        /// </summary>
        /// <param name="FCOGroupView">The fco group view.</param>
        /// <returns>group list</returns>
        int Update(FCOGroup FCOGroupView);
    }
}

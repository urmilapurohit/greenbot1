using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.SpvLogin
{
    public interface ISpvLoginBAL
    {
        /// <summary>
        /// Gets the menu action by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetSpvMenuActionByRoleId(int roleId);

        /// Updates the lat login date.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        void UpdateLastLoginDate(int UserId);

    }
}

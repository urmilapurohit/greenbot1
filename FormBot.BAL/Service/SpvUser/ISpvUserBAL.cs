using FormBot.Entity;
using FormBot.Entity.Role;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ISpvUserBAL
    {
        List<SpvUserType> GetData();
        DataSet GetDataForStreetTypeUnitTypeDropdown(List<CommonData> cdata);
       // object CheckUserExists(string userName);
        int CreateSpvUser(SpvUser userView);
        DataSet GetSpvUserById(int? userID);


        List<SpvUser> GetAllSpvUserKendo(int pageNumber, int pageSize, string sortCol = "", string sortDir = "", string Name = "", string UserName = "", string Email = "", int SpvUserTypeId = 0, bool? IsActive = null,int SpvRoleId= 0);
        List<SpvRoleView> GetSpvRole(int? userType = null);

        /// <summary>
        /// Gets the user by aspnet user identifier.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user object</returns>
        SpvUser GetSpvUserByAspnetUserId(string aspnetUserId);

        /// <summary>
        /// Logins the user email details.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>data set</returns>
        DataSet LoginSpvUserEmailDetails(int userID);
        /// <summary>
        /// Checks the email exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// bool type
        /// </returns>
        bool CheckEmailExists(string userName, int? spvUserId = null);
        void DeleteUser(int SpvuserID);

        /// <summary>
        /// Gets the spv user of user type.
        /// </summary>
        /// <returns>user list</returns>
        List<SpvUser> GetSpvUserByUserType(int spvUserTypeId);

        /// <summary>
        /// Get SPV Users By FSA
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        SpvUser GetSPVUsersByFSA(int UserID);
    }
}

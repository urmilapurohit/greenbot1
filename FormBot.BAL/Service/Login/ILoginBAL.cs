using System.Collections.Generic;
using System.Data;

namespace FormBot.BAL.Service
{
    public interface ILoginBAL
    {

        /// <summary>
        /// Gets the menu action by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetMenuActionByRoleId(int roleId);

        /// <summary>
        /// Updates the lat login date.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        void UpdateLatLoginDate(int UserId);

        /// <summary>
        /// Checks the logged in user status.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UsertypeId">The usertype identifier.</param>
        /// <returns>string</returns>
        string CheckLoggedInUserStatus(int UserId, int UsertypeId);

        /// <summary>
        /// Gets the reseller theme logo.
        /// </summary>
        /// <param name="CompanyName">Name of the company.</param>
        /// <returns>DataSet</returns>
        DataSet GetResellerThemeLogo(string CompanyName);

        /// <summary>
        /// Gets the reseller theme logo forget password.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetResellerThemeLogoForgetPassword(string UserId);

        /// <summary>
        /// Checks the reseller delete.
        /// </summary>
        /// <param name="CompanyName">Name of the company.</param>
        /// <returns>object</returns>
        object CheckResellerDelete(string CompanyName);

        /// <summary>
        /// Gets the default name of the login company.
        /// </summary>
        /// <returns>string</returns>
        string GetDefaultLoginCompanyName();
        void InsertOtpData(int UserId,string UserName,string Phone,string IP,int OtpCode);
        int GetOTPData(int UserId, string UserName, string phone,int? OtpCode);
        string GetAspnetUserId(int UserId);
        DataSet UpdateFlagResetPassword();
        bool GetResetFlag(int UserId);
        /// <summary>
        /// Get last three password to check in reset password 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        List<string> GetOldPasswordHistory(string Id);

        /// <summary>
        /// Insert password in password history
        /// </summary>
        /// <param name="Id"></param>
        void InsertPasswordHistory(string Id);

    }
}

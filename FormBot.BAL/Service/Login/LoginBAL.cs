using FormBot.DAL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FormBot.Entity;
using FormBot.Helper;
using System.Linq;
using System;
using System.Configuration;

namespace FormBot.BAL.Service
{
    public class LoginBAL : ILoginBAL
    {
        /// <summary>
        /// Gets the menu action by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetMenuActionByRoleId(int roleId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleId", SqlDbType.Int, roleId));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("Login_GetMenuActionByRoleId", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Updates the lat login date.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void UpdateLatLoginDate(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("LoginDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("User_UpdateLoginDate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the logged in user status.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UsertypeId">The usertype identifier.</param>
        /// <returns>
        /// string
        /// </returns>
        public string CheckLoggedInUserStatus(int UserId, int UsertypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UsertypeId", SqlDbType.Int, UsertypeId));
            var status = CommonDAL.ExecuteScalar("Login_CheckUserStatus", sqlParameters.ToArray());
            return status.ToString();
        }

        /// <summary>
        /// Gets the reseller theme logo.
        /// </summary>
        /// <param name="CompanyName">Name of the company.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetResellerThemeLogo(string CompanyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, CompanyName));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("Reseller_ThemeLogo", sqlParameters.ToArray());
            return dsRoles;
        }

        /// <summary>
        /// Gets the reseller theme logo forget password.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetResellerThemeLogoForgetPassword(string userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("AspNetUserId", SqlDbType.NVarChar, userID));
            DataSet dsRoles = CommonDAL.ExecuteDataSet("Reseller_ThemeLogoForgetPassword", sqlParameters.ToArray());
            return dsRoles;
        }
        public void InsertOtpData(int UserId, string UserName, string Phone, string IP, int OtpCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.VarChar, UserName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, Phone));
            sqlParameters.Add(DBClient.AddParameters("Ip", SqlDbType.VarChar, IP));
            sqlParameters.Add(DBClient.AddParameters("OtpCode", SqlDbType.Int, OtpCode));
            sqlParameters.Add(DBClient.AddParameters("SendOtpDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("InsertOTPData", sqlParameters.ToArray());
        }
        public int GetOTPData(int UserId, string UserName, string Phone,int? OtpCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.VarChar, UserName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, Phone));
            sqlParameters.Add(DBClient.AddParameters("OtpCode", SqlDbType.Int, OtpCode));
            var otp = CommonDAL.ExecuteScalar("GetOtpData", sqlParameters.ToArray());
            return Convert.ToInt32(otp);
        }
        public string GetAspnetUserId(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            var AspnetId= CommonDAL.ExecuteScalar("GetAspNetUserId", sqlParameters.ToArray());
            return AspnetId.ToString();
        }
        public DataSet UpdateFlagResetPassword()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("timeIntervalforReset", SqlDbType.Int, ConfigurationManager.AppSettings["TimeintervalForResetPassword"]));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateFlagResetPassword", sqlParameters.ToArray());
            return ds;
        }
        public bool GetResetFlag(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            var resetpwdflag = CommonDAL.ExecuteScalar("GetResetFlag", sqlParameters.ToArray());
            return Convert.ToBoolean(resetpwdflag);
        }
        /// <summary>
        /// Checks the reseller delete.
        /// </summary>
        /// <param name="CompanyName">Name of the company.</param>
        /// <returns>
        /// object
        /// </returns>
        public object CheckResellerDelete(string CompanyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, CompanyName));
            object resellerDeleted = CommonDAL.ExecuteScalar("ResellerDetails_CheckResellerDelete", sqlParameters.ToArray());
            return resellerDeleted;
        }

        /// <summary>
        /// Gets the default name of the login company.
        /// </summary>
        /// <returns>
        /// string
        /// </returns>
        public string GetDefaultLoginCompanyName()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            return Convert.ToString(CommonDAL.ExecuteScalar("ResellerDetails_GetDefaultLoginCompanyName", sqlParameters.ToArray()));
        }

        /// <summary>
        /// Get last three password to check in reset password
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<string> GetOldPasswordHistory(string Id)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.VarChar, Id));
                DataSet ds = CommonDAL.ExecuteDataSet("GetOldPasswordHistory", sqlParameters.ToArray());
                return ds.Tables[0].AsEnumerable().Select(x => x.Field<string>("PasswordHash")).ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Insert password in password history
        /// </summary>
        /// <param name="Id"></param>
        public void InsertPasswordHistory(string Id)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.VarChar, Id));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
                CommonDAL.ExecuteScalar("InsertPasswordHistory", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {

            }
        }

    }
}

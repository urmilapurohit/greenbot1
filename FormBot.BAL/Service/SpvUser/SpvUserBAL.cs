using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.Role;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public class SpvUserBAL : ISpvUserBAL
    {
        // <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>user list</returns>
        public List<SpvUserType> GetData()
        {
            string spName = "[SpvUserType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<SpvUserType> userTypeList = CommonDAL.ExecuteProcedure<SpvUserType>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }
        public DataSet GetDataForStreetTypeUnitTypeDropdown(List<CommonData> cData)
        {
            DataSet ds = new DataSet();
            foreach (CommonData obj in cData)
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                DataTable dt = CommonDAL.ExecuteDataSet(obj.proc, sqlParameters.ToArray()).Tables[0].Copy();
                dt.TableName = obj.id;
                ds.Tables.Add(dt);
            }
            return ds;
        }
        //public object CheckUserExists(string userName, int? userId = null)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, userName));
        //    sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
        //    object lstUser = CommonDAL.ExecuteScalar("AspNetUser_CheckUserNameExist", sqlParameters.ToArray());
        //    return lstUser;
        //}
        public int CreateSpvUser(SpvUser userView)
        {
            string spName = "[SpvUsers_InsertUpdate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
           sqlParameters.Add(DBClient.AddParameters("SpvUserID", SqlDbType.Int, userView.SpvUserId));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserId", SqlDbType.NVarChar, userView.AspNetUserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, userView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, userView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("ManufacturerName", SqlDbType.NVarChar, userView.ManufacturerName));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            //sqlParameters.Add(DBClient.AddParameters("CompanyWebsite", SqlDbType.NVarChar, userView.CompanyWebsite));
            sqlParameters.Add(DBClient.AddParameters("SpvUserTypeID", SqlDbType.Int, userView.SpvUserTypeId));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, userView.IsActive));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, userView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("Logo", SqlDbType.NVarChar, userView.Logo));
            sqlParameters.Add(DBClient.AddParameters("Theme", SqlDbType.Int, userView.Theme));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, userView.Status));
            sqlParameters.Add(DBClient.AddParameters("SpvRoleId", SqlDbType.Int, userView.SpvRoleId));
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());

            return Convert.ToInt32(userID);
        }
        public DataSet GetSpvUserById(int? userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("SpvUsers_GetUserById", sqlParameters.ToArray());
            return dsUsers;
        }
        public List<SpvUser> GetAllSpvUserKendo(int pageNumber, int pageSize, string sortCol ="", string sortDir = "",string Name = "", string UserName = "", string Email = "", int SpvUserTypeId = 0, bool? IsActive = null,int SpvRoleId= 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, Name));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, UserName));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, Email));
            sqlParameters.Add(DBClient.AddParameters("SpvUserTypeId", SqlDbType.Int, SpvUserTypeId));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, IsActive));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SpvRoleId", SqlDbType.Int, SpvRoleId));
            //return CommonDAL.ExecuteProcedure<SpvUser>("GetAllSpvUserKendo", sqlParameters.ToArray()).ToList();

            List<SpvUser> lstUser = CommonDAL.ExecuteProcedure<SpvUser>("GetAllSpvUserKendo", sqlParameters.ToArray()).ToList();
            return lstUser;
        }
		public List<SpvRoleView> GetSpvRole(int? userType = null)
        {
            string spName = "[SpvRole_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, userType));
            IList<SpvRoleView> roleList = CommonDAL.ExecuteProcedure<SpvRoleView>(spName, sqlParameters.ToArray());
            return roleList.ToList();
        }
        /// <summary>
        /// Gets the spv user by aspnet user identifier.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user identifier</returns>
        public SpvUser GetSpvUserByAspnetUserId(string aspnetUserId)
        {
            SpvUser user = CommonDAL.SelectObject<SpvUser>("SpvUsers_GetUserByAspnetUserId", DBClient.AddParameters("Aspnetuserid", SqlDbType.NVarChar, aspnetUserId));
            return user;
        }

        /// <summary>
        /// Logins the spv user email details.
        /// </summary>
        /// <param name="userID">The spv user identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet LoginSpvUserEmailDetails(int spvUserID)
        {
            string spName = "[EmailMapping_GetLoginEmailDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, spvUserID));
            DataSet emailDetails = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray()); ////CommonDAL.ExecuteProcedure<Entity.Email.EmailSignup>(spName, sqlParameters.ToArray());
            return emailDetails;
        }
        /// <summary>
        /// Checks the email exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// object type
        /// </returns>
        public bool CheckEmailExists(string userName, int? spvUserId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, userName));
            sqlParameters.Add(DBClient.AddParameters("SpvUserId", SqlDbType.Int, spvUserId));
            bool lstUser = Convert.ToBoolean(CommonDAL.ExecuteScalar("AspNetUser_CheckSpvUserNameExist", sqlParameters.ToArray()));
            return lstUser;
        }
      public void DeleteUser(int SpvuserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SpvUserID", SqlDbType.Int, SpvuserID));
            sqlParameters.Add(DBClient.AddParameters("LoginUserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("SpvUsers_DeleteUser", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the user of user type.
        /// </summary>
        /// <returns>user list</returns>
        public List<SpvUser> GetSpvUserByUserType(int userTypeId)
        {
            string spName = "[SpvUser_GetUserByUserType]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            IList<SpvUser> userList = CommonDAL.ExecuteProcedure<SpvUser>(spName, sqlParameters.ToArray());
            return userList.ToList();
        }

        /// <summary>
        /// Gets the user by FSA.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user identifier</returns>
        public SpvUser GetSPVUsersByFSA(int UserID)
        {
            SpvUser user = CommonDAL.SelectObject<SpvUser>("SpvUsers_GetUsersByFSA", DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            return user;
        }

    }
}

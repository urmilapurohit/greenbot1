using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System;
using FormBot.Helper;
using System.Text;
using FormBot.Entity.SolarElectrician;
using FormBot.Entity.VEEC;
using FormBot.Entity.KendoGrid;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service
{
    public class UserBAL : IUserBAL
    {

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="userView">user table value</param>
        /// <returns>Returns the UserID</returns>
        public int Create(User userView)
        {
            string spName = "[Users_InsertUpdate_invoicer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserId", SqlDbType.NVarChar, userView.AspNetUserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, userView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, userView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, userView.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, userView.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("CompanyWebsite", SqlDbType.NVarChar, userView.CompanyWebsite));
            sqlParameters.Add(DBClient.AddParameters("RecUserName", SqlDbType.NVarChar, userView.RecUserName));
            sqlParameters.Add(DBClient.AddParameters("RecPassword", SqlDbType.NVarChar, userView.RecPassword));
            sqlParameters.Add(DBClient.AddParameters("BSB", SqlDbType.NVarChar, userView.BSB));
            sqlParameters.Add(DBClient.AddParameters("AccountNumber", SqlDbType.NVarChar, userView.AccountNumber));
            sqlParameters.Add(DBClient.AddParameters("AccountName", SqlDbType.NVarChar, userView.AccountName));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, userView.ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, userView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("CECDesignerNumber", SqlDbType.NVarChar, userView.CECDesignerNumber));
            sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.NVarChar, userView.Signature));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, userView.UserTypeID));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, userView.IsActive));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, userView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, userView.ResellerID));
            //sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, (userView.SolarSubContractorID != null ? userView.SolarSubContractorID : userView.SolarCompanyId)));
            sqlParameters.Add(DBClient.AddParameters("Logo", SqlDbType.NVarChar, userView.Logo));
            sqlParameters.Add(DBClient.AddParameters("Theme", SqlDbType.Int, userView.Theme));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, userView.FromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, userView.ToDate));
            sqlParameters.Add(DBClient.AddParameters("IsFirstLogin", SqlDbType.Bit, userView.IsFirstLogin));
            sqlParameters.Add(DBClient.AddParameters("RoleID", SqlDbType.Int, userView.RoleID));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, userView.LoginCompanyName));
            sqlParameters.Add(DBClient.AddParameters("SEDesigner", SqlDbType.Int, userView.SEDesigner));
            sqlParameters.Add(DBClient.AddParameters("IsSTC", SqlDbType.Bit, userView.IsSTC));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserIdSE", SqlDbType.NVarChar, userView.AspNetUserIdSE));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ClientCodePrefix", SqlDbType.NVarChar, userView.ClientCodePrefix));
            sqlParameters.Add(DBClient.AddParameters("ClientNumber", SqlDbType.NVarChar, userView.ClientNumber));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.NVarChar, userView.RAMId));
            sqlParameters.Add(DBClient.AddParameters("IsWholeSaler", SqlDbType.Bit, userView.IsWholeSaler));
            sqlParameters.Add(DBClient.AddParameters("CustomCompanyName", SqlDbType.NVarChar, userView.CustomCompanyName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerFirstName", SqlDbType.NVarChar, userView.WholesalerFirstName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerLastName", SqlDbType.NVarChar, userView.WholesalerLastName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerEmail", SqlDbType.NVarChar, userView.WholesalerEmail));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPhone", SqlDbType.NVarChar, userView.WholesalerPhone));
            sqlParameters.Add(DBClient.AddParameters("WholesalerMobile", SqlDbType.NVarChar, userView.WholesalerMobile));
            sqlParameters.Add(DBClient.AddParameters("WholesalerCompanyABN", SqlDbType.NVarChar, userView.WholesalerCompanyABN));
            sqlParameters.Add(DBClient.AddParameters("WholesalerCompanyName", SqlDbType.NVarChar, userView.WholesalerCompanyName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerIsPostalAddress", SqlDbType.Bit, Convert.ToBoolean(userView.WholesalerIsPostalAddress)));
            sqlParameters.Add(DBClient.AddParameters("WholesalerUnitTypeID", SqlDbType.Int, userView.WholesalerUnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerUnitNumber", SqlDbType.NVarChar, userView.WholesalerUnitNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetNumber", SqlDbType.NVarChar, userView.WholesalerStreetNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetName", SqlDbType.NVarChar, userView.WholesalerStreetName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetTypeID", SqlDbType.Int, userView.WholesalerStreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostalAddressID", SqlDbType.Int, userView.WholesalerPostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostalDeliveryNumber", SqlDbType.NVarChar, userView.WholesalerPostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerTown", SqlDbType.NVarChar, userView.WholesalerTown));
            sqlParameters.Add(DBClient.AddParameters("WholesalerState", SqlDbType.NVarChar, userView.WholesalerState));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostCode", SqlDbType.NVarChar, userView.WholesalerPostCode));
            sqlParameters.Add(DBClient.AddParameters("WholesalerBSB", SqlDbType.NVarChar, userView.WholesalerBSB));
            sqlParameters.Add(DBClient.AddParameters("WholesalerAccountNumber", SqlDbType.NVarChar, userView.WholesalerAccountNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerAccountName", SqlDbType.NVarChar, userView.WholesalerAccountName));

            //Invoicer Details
            sqlParameters.Add(DBClient.AddParameters("UsageType", SqlDbType.Int, userView.UsageType));
            sqlParameters.Add(DBClient.AddParameters("IsSAASUser", SqlDbType.Bit, userView.IsSAASUser));
            sqlParameters.Add(DBClient.AddParameters("Invoicer", SqlDbType.Int, userView.Invoicer));
            sqlParameters.Add(DBClient.AddParameters("UniqueContactID", SqlDbType.NVarChar, userView.UniqueContactId));
            sqlParameters.Add(DBClient.AddParameters("ContractPath", SqlDbType.NVarChar, userView.ContractPath));

            //sqlParameters.Add(DBClient.AddParameters("IsAllowedMasking", SqlDbType.Bit, userView.isAllowedMasking));

            if (userView.IsSTC == true && userView.UserTypeID == 4)
                userView.IsPVDUser = true;

            sqlParameters.Add(DBClient.AddParameters("IsPVDUser", SqlDbType.Bit, userView.IsPVDUser));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, userView.IsSWHUser));
            sqlParameters.Add(DBClient.AddParameters("IsVEECUser", SqlDbType.Bit, userView.IsVEECUser));

            if (userView.AccreditedInstallerId > 0)
            {
                sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, userView.Status));
                sqlParameters.Add(DBClient.AddParameters("Note", SqlDbType.NVarChar, userView.Note));
                sqlParameters.Add(DBClient.AddParameters("ComplainBy", SqlDbType.Int, userView.ComplainBy));
            }
            sqlParameters.Add(DBClient.AddParameters("IsAutoRequest", SqlDbType.Bit, userView.IsAutoRequest));

            if (userView.UserTypeID == 4)
                sqlParameters.Add(DBClient.AddParameters("SCisAllowedSPV", SqlDbType.Bit, userView.SCisAllowedSPV));

            if (userView.UserTypeID == 7 || userView.UserTypeID==9)
            {
                sqlParameters.Add(DBClient.AddParameters("SESelfie", SqlDbType.NVarChar, userView.SESelfie));
                sqlParameters.Add(DBClient.AddParameters("Reason", SqlDbType.NVarChar, userView.Reason));
            }
            sqlParameters.Add(DBClient.AddParameters("IsVerified", SqlDbType.Int, userView.IsVerified));
            //sqlParameters.Add(DBClient.AddParameters("EntityName", SqlDbType.VarChar, userView.EntityName));
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());

            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Gets the user by aspnet user identifier.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user identifier</returns>
        public User GetUserByAspnetUserId(string aspnetUserId)
        {
            User user = CommonDAL.SelectObject<User>("Users_GetUserByAspnetUserId", DBClient.AddParameters("Aspnetuserid", SqlDbType.NVarChar, aspnetUserId));
            return user;
        }

        public User GetUserByUserName(string Username)
        {
            User user = CommonDAL.SelectObject<User>("Users_GetUserByAspnetUserId", DBClient.AddParameters("Username", SqlDbType.NVarChar, Username));
            return user;
        }


        public UserVendor GetUserByAspnetUserId_VendorAPI(string aspnetUserId)
        {
            UserVendor user = CommonDAL.SelectObject<UserVendor>("Users_GetUserByAspnetUserId", DBClient.AddParameters("Aspnetuserid", SqlDbType.NVarChar, aspnetUserId));
            return user;
        }
        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="selectedUserTypeId">The selected user type identifier.</param>
        /// <param name="view">The view.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="reseller">The reseller.</param>
        /// <param name="solarCompany">The solar company.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="roleID">The role identifier.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="userstatus">The userstatus.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="designernumber">The designernumber.</param>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        /// <returns>user list</returns>
        public List<User> GetUserList(int userID, int pageNumber, int pageSize, string sortCol, string sortDir, string selectedUserTypeId, int view, string name, string username, string email, int userType, int reseller, int solarCompany, bool active, int roleID, string companyname, string companyabn, int userstatus, string accreditationnumber, string designernumber, string licensenumber, int electricianstatus, int ramId, string isAllScaJobView = "", string State = "", int? isallowspv = null, string mobile = "", string IsSaasUser = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("View", SqlDbType.TinyInt, view));
            sqlParameters.Add(DBClient.AddParameters("SelectedUserTypeId", SqlDbType.NVarChar, selectedUserTypeId));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, username));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.NVarChar, email));
            sqlParameters.Add(DBClient.AddParameters("userType", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("reseller", SqlDbType.Int, reseller));
            sqlParameters.Add(DBClient.AddParameters("solarcompany", SqlDbType.Int, solarCompany));
            sqlParameters.Add(DBClient.AddParameters("active", SqlDbType.Bit, active));
            sqlParameters.Add(DBClient.AddParameters("roleID", SqlDbType.Int, roleID));
            sqlParameters.Add(DBClient.AddParameters("companyname", SqlDbType.NVarChar, companyname));
            sqlParameters.Add(DBClient.AddParameters("companyabn", SqlDbType.NVarChar, companyabn));
            sqlParameters.Add(DBClient.AddParameters("userstatus", SqlDbType.TinyInt, userstatus));
            sqlParameters.Add(DBClient.AddParameters("accreditationnumber", SqlDbType.NVarChar, accreditationnumber));
            sqlParameters.Add(DBClient.AddParameters("designernumber", SqlDbType.NVarChar, designernumber));
            sqlParameters.Add(DBClient.AddParameters("licensenumber", SqlDbType.NVarChar, licensenumber));
            sqlParameters.Add(DBClient.AddParameters("electricianstatus", SqlDbType.TinyInt, electricianstatus));
            sqlParameters.Add(DBClient.AddParameters("ramId", SqlDbType.Int, ramId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.VarChar, State));
            sqlParameters.Add(DBClient.AddParameters("isallowspv", SqlDbType.Bit, isallowspv));
            sqlParameters.Add(DBClient.AddParameters("mobile", SqlDbType.VarChar, mobile));
            sqlParameters.Add(DBClient.AddParameters("IsSaasUser", SqlDbType.VarChar, IsSaasUser == "1" ? "1" : IsSaasUser == "0" ? "0" : "1,0"));
            List<User> lstUser = CommonDAL.ExecuteProcedure<User>("Users_GetUsersList", sqlParameters.ToArray()).ToList();
            return lstUser;
        }

        /// <summary>
        /// Gets the solar electrician list.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="designernumber">The designernumber.</param>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        /// <returns>
        /// user list
        /// </returns>
        public List<User> GetSolarElectricianList(int userType, int pageSize, int pageNumber, string sortCol, string sortDir, string name, string username, string email, int solarCompanyId, string accreditationnumber, string designernumber, string licensenumber, int electricianstatus, string requestedusertype, int RoleId, int? IsVerified, string mobile, string IsSaasUser)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, username));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.NVarChar, email));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("accreditationnumber", SqlDbType.NVarChar, accreditationnumber));
            sqlParameters.Add(DBClient.AddParameters("designernumber", SqlDbType.NVarChar, designernumber));
            sqlParameters.Add(DBClient.AddParameters("licensenumber", SqlDbType.NVarChar, licensenumber));
            sqlParameters.Add(DBClient.AddParameters("electricianstatus", SqlDbType.TinyInt, electricianstatus));
            sqlParameters.Add(DBClient.AddParameters("requestedusertype", SqlDbType.NVarChar, requestedusertype));
            sqlParameters.Add(DBClient.AddParameters("roleID", SqlDbType.Int, RoleId));
            sqlParameters.Add(DBClient.AddParameters("IsVerified", SqlDbType.Int, IsVerified));
            sqlParameters.Add(DBClient.AddParameters("mobile", SqlDbType.NVarChar, mobile));
            sqlParameters.Add(DBClient.AddParameters("IsSaasUser", SqlDbType.VarChar, IsSaasUser == "1" ? "1" : IsSaasUser == "0" ? "0" : "1,0"));
            List<User> lstSolarElectrician = CommonDAL.ExecuteProcedure<User>("Users_GetSolarElectricianList", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }


        public List<User> GetAllUserForRestoreList(int userType, int pageSize, int pageNumber, string sortCol, string sortDir, string name, string username, string email, string accreditationnumber, string licensenumber, string companyabn, string companyName, string resellerId, string companyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, username));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.NVarChar, email));
            sqlParameters.Add(DBClient.AddParameters("accreditationnumber", SqlDbType.NVarChar, accreditationnumber));
            sqlParameters.Add(DBClient.AddParameters("licensenumber", SqlDbType.NVarChar, licensenumber));
            sqlParameters.Add(DBClient.AddParameters("companyabn", SqlDbType.NVarChar, companyabn));
            sqlParameters.Add(DBClient.AddParameters("companyName", SqlDbType.NVarChar, companyName));
            if (!(resellerId == ""))
            {
                sqlParameters.Add(DBClient.AddParameters("resellerId", SqlDbType.Int, Convert.ToInt32(resellerId)));
            }
            if (!(companyId == ""))
            {
                sqlParameters.Add(DBClient.AddParameters("companyId", SqlDbType.Int, Convert.ToInt32(companyId)));
            }

            List<User> lstSolarElectrician = CommonDAL.ExecuteProcedure<User>("User_GetRestoreUserList", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }



        public List<User> GetRequestedSolarElectricianList(int UserId, int userType, int pageSize, int pageNumber, string sortCol, string sortDir, int solarCompanyId, string name, string accreditationnumber, string licensenumber, int electricianstatus, string requestedusertype)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.VarChar, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("accreditationnumber", SqlDbType.NVarChar, accreditationnumber));
            sqlParameters.Add(DBClient.AddParameters("licensenumber", SqlDbType.NVarChar, licensenumber));
            sqlParameters.Add(DBClient.AddParameters("electricianstatus", SqlDbType.TinyInt, electricianstatus));
            //sqlParameters.Add(DBClient.AddParameters("requestedusertypeid", SqlDbType.Int, requestedusertypeid));
            sqlParameters.Add(DBClient.AddParameters("requestedusertype", SqlDbType.NVarChar, requestedusertype));

            List<User> lstSolarElectrician = CommonDAL.ExecuteProcedure<User>("Users_GetRequestedSolarElectricianList", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }

        /// <summary>
        /// Gets the requesting sc afor SSC.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="status">The status.</param>
        /// <returns>user list</returns>
        public List<User> GetRequestingSCAforSSC(int userID, int pageNumber, int pageSize, string sortCol, string sortDir, string name, string email, string companyname, string companyabn, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.NVarChar, email));
            sqlParameters.Add(DBClient.AddParameters("companyname", SqlDbType.NVarChar, companyname));
            sqlParameters.Add(DBClient.AddParameters("companyabn", SqlDbType.NVarChar, companyabn));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, status));
            List<User> lstUser = CommonDAL.ExecuteProcedure<User>("Users_GetRequestingSCAforSSC", sqlParameters.ToArray()).ToList();
            return lstUser;
        }

        /// <summary>
        /// Gets the requested SSC for SCA.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort column.</param>
        /// <param name="SortDir">The sort direction.</param>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="status">The status.</param>
        /// <returns>Returns List of Requested SSC for SCA</returns>
        public List<User> GetRequestedSSCforSCA(int UserId, int SolarCompanyId, int PageNumber, int PageSize, string SortCol, string SortDir, string name, string email, string companyname, string companyabn, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.NVarChar, email));
            sqlParameters.Add(DBClient.AddParameters("companyname", SqlDbType.NVarChar, companyname));
            sqlParameters.Add(DBClient.AddParameters("companyabn", SqlDbType.NVarChar, companyabn));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, status));
            List<User> lstUser = CommonDAL.ExecuteProcedure<User>("Users_GetRequestedSSCforSCA", sqlParameters.ToArray()).ToList();
            return lstUser;
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        public void DeleteUser(int userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("LoginUserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("Users_DeleteUser", sqlParameters.ToArray());
        }

        /// <summary>
        /// Change status of SE
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="status">The status.</param>
        public void SEAcceptRejectSolarCompanyRequest(int userID, int solarCompanyId, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, status));
            CommonDAL.Crud("Users_SEAcceptRejectSolarCompanyRequest", sqlParameters.ToArray());
        }

        public void SSCAcceptRejectSolarCompanyRequest(int userID, int solarCompanyId, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, status));
            CommonDAL.Crud("Users_SCAAcceptRejectSolarCompanyRequest", sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the email exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// object type
        /// </returns>
        public object CheckEmailExists(string userName, int? userId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, userName));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            object lstUser = CommonDAL.ExecuteScalar("AspNetUser_CheckUserNameExist", sqlParameters.ToArray());
            return lstUser;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetUserById(int? userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("Users_GetUserById", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Inserts the status note.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="Note">The note.</param>
        /// <param name="ComplainBy">The complain by.</param>
        /// <param name="IsSCDetailConfirm">if set to <c>true</c> [is sc detail confirm].</param>
        /// <param name="IsInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="IsSEDetailConfirm">if set to <c>true</c> [is se detail confirm].</param>
        public void InsertStatusNote(int UserId, byte? Status, string Note, int ComplainBy, bool IsSCDetailConfirm, bool IsInstaller, bool IsSEDetailConfirm, int? IsGSTSetByAdminUser = null)
        {
            string spName = "[Users_InsertStatusNote]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, Status));
            sqlParameters.Add(DBClient.AddParameters("Note", SqlDbType.NVarChar, Note));
            sqlParameters.Add(DBClient.AddParameters("ComplainBy", SqlDbType.NVarChar, ComplainBy));
            sqlParameters.Add(DBClient.AddParameters("IsSCDetailConfirm", SqlDbType.Bit, IsSCDetailConfirm));
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, IsInstaller));
            sqlParameters.Add(DBClient.AddParameters("IsSEDetailConfirm", SqlDbType.Bit, IsSEDetailConfirm));
            sqlParameters.Add(DBClient.AddParameters("IsGSTSetByAdminUser", SqlDbType.Int, IsGSTSetByAdminUser));
            sqlParameters.Add(DBClient.AddParameters("ComplainDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        public void InsertUpdateResellerRECLoginDetails(int UserId, string ResellerName, string LoginType, string CERLoginId, string CERPassword, string RECCompName, string UserName, string RECName, int UpdatedBy, bool IsDefault)
        {
            string spName = "[Reseller_InsertRECLoginDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("ResellerName", SqlDbType.NVarChar, ResellerName));
            sqlParameters.Add(DBClient.AddParameters("LoginType", SqlDbType.NVarChar, LoginType));
            sqlParameters.Add(DBClient.AddParameters("CERLoginId", SqlDbType.NVarChar, CERLoginId));
            sqlParameters.Add(DBClient.AddParameters("CERPassword", SqlDbType.NVarChar, CERPassword));
            sqlParameters.Add(DBClient.AddParameters("RECCompName", SqlDbType.NVarChar, RECCompName));
            sqlParameters.Add(DBClient.AddParameters("RECUserName", SqlDbType.NVarChar, UserName));
            sqlParameters.Add(DBClient.AddParameters("RECName", SqlDbType.NVarChar, RECName));
            sqlParameters.Add(DBClient.AddParameters("UpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("UpdatedBy", SqlDbType.Int, UpdatedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, IsDefault));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        public void InsertUpdateDocVerification(int UserId, bool IsSignatureVerified, bool IsSelfieVerified, bool IsDriverLicVerified, bool IsOtherDocVerified)
        {
            string spName = "[User_InsertUpdateDocVerification]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("IsSignatureVerified", SqlDbType.Bit, IsSignatureVerified));
            sqlParameters.Add(DBClient.AddParameters("IsSelfieVerified", SqlDbType.Bit, IsSelfieVerified));
            sqlParameters.Add(DBClient.AddParameters("IsDriverLicVerified", SqlDbType.Bit, IsDriverLicVerified));
            sqlParameters.Add(DBClient.AddParameters("IsOtherDocVerified", SqlDbType.Bit, IsOtherDocVerified));
            sqlParameters.Add(DBClient.AddParameters("VerifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("LastVerifiedOn", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>
        /// company identifier
        /// </returns>
        public int CheckCompanyABN(string companyABN, int? userID = null, int? UserTypeId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, companyABN));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeId));
            object lstABN = CommonDAL.ExecuteScalar("ResellerDetails_CheckCompanyABNExist", sqlParameters.ToArray());
            return Convert.ToInt32(lstABN);
        }

        /// <summary>
        /// Inserts the user document.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="DocumentPath">The document path.</param>
        public void InsertUserDocument(int UserId, string DocumentPath, int DocumentType = 0, int DocLoc = 0)
        {
            string spName = "[UserDocument_Insert]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("DocumentPath", SqlDbType.NVarChar, DocumentPath));
            sqlParameters.Add(DBClient.AddParameters("DocumentType", SqlDbType.Int, DocumentType));
            if (DocLoc != 0)
                sqlParameters.Add(DBClient.AddParameters("DocLoc", SqlDbType.Int, DocLoc));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the user document.
        /// </summary>
        /// <param name="UserDocumentID">The user document identifier.</param>
        public void DeleteUserDocument(int UserDocumentID)
        {
            string spName = "[UserDocument_Delete]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserDocumentID", SqlDbType.Int, UserDocumentID));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the user users document by user identifier.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns>user list</returns>
        public List<UserDocument> GetUserUsersDocumentByUserId(int UserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("UsersDocument_GetUserUsersDocumentByUserId", sqlParameters.ToArray()).ToList();
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                int type1 = 0, type2 = 0, type3 = 0, type4 = 0, type5 = 0;
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }

                    if (lstUserDocument[i].DocumentType == 1)
                    {
                        type1++;
                        lstUserDocument[i].index = type1;
                    }
                    else if (lstUserDocument[i].DocumentType == 2)
                    {
                        type2++;
                        lstUserDocument[i].index = type2;
                    }
                    else if (lstUserDocument[i].DocumentType == 3)
                    {
                        type3++;
                        lstUserDocument[i].index = type3;
                    }
                    else if (lstUserDocument[i].DocumentType == 4)
                    {
                        type4++;
                        lstUserDocument[i].index = type4;
                    }
                    else if (lstUserDocument[i].DocumentType == 5)
                    {
                        type5++;
                        lstUserDocument[i].index = type5;
                    }
                    else
                        lstUserDocument[i].index = i + 1;
                }
            }
            return lstUserDocument;
        }

        /// <summary>
        /// Creates the send request.
        /// </summary>
        /// <param name="solarElectricianView">The solar electrician view.</param>
        /// <returns>
        /// electrician view
        /// </returns>
        public int? CreateSendRequest(SolarElectricianView solarElectricianView)
        {
            string spName = "[SolarElectrician_InsertSendRequest]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, solarElectricianView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, solarElectricianView.LastName));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, solarElectricianView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, solarElectricianView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, solarElectricianView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, solarElectricianView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, solarElectricianView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, solarElectricianView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, solarElectricianView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, solarElectricianView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, solarElectricianView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("CECDesignerNumber", SqlDbType.NVarChar, solarElectricianView.CECDesignerNumber));
            //sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, solarElectricianView.CreatedBy));
            //sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, solarElectricianView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, solarElectricianView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, solarElectricianView.UserId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarElectricianView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, solarElectricianView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, solarElectricianView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, solarElectricianView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("SERole", SqlDbType.Int, solarElectricianView.SEDesignRoleId));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, solarElectricianView.Email));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, solarElectricianView.ElectricalContractorsLicenseNumber));
            //sqlParameters.Add(DBClient.AddParameters("IsInstallerDesignerAddByProfile", SqlDbType.Bit, solarElectricianView.IsInstallerDesignerAddByProfile));
            //sqlParameters.Add(DBClient.AddParameters("SESignature", SqlDbType.NVarChar, solarElectricianView.SESignature));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, solarElectricianView.UserTypeID));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, solarElectricianView.ElectricianStatusId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            if (Convert.ToString(userID) == string.Empty)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(userID);
            }
        }

        /// <summary>
        /// Checks the accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <returns>
        /// Accreditation Number
        /// </returns>
        //public int CheckAccreditationNumber(string accreditationNumber, int UserTypeID)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
        //    sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeID));
        //    object userID = CommonDAL.ExecuteScalar("User_CheckAccreditationNumber", sqlParameters.ToArray());
        //    return Convert.ToInt32(userID);

        //    //if (Convert.ToString(userID) == string.Empty)
        //    //{
        //    //    return 0;
        //    //}
        //    //else
        //    //{
        //    //    return Convert.ToInt32(userID);
        //    //}
        //}

        public SpResponce_CheckAccreditationNumber CheckAccreditationNumber(string CECAccreditationNumber, string ElectricalContractorsLicenseNumber, string Email, bool IsPVD, bool IsSWH, bool IsSendRequet = false, int UserIdNotInclude = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPVD", SqlDbType.Bit, IsPVD));
            sqlParameters.Add(DBClient.AddParameters("IsSWH", SqlDbType.Bit, IsSWH));
            sqlParameters.Add(DBClient.AddParameters("IsSendRequet", SqlDbType.Bit, IsSendRequet));
            sqlParameters.Add(DBClient.AddParameters("UserIdNotInclude", SqlDbType.Int, UserIdNotInclude));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, Email));
            SpResponce_CheckAccreditationNumber oSpResponce_CheckAccreditationNumber = CommonDAL.ExecuteProcedure<SpResponce_CheckAccreditationNumber>("User_CheckAccreditationNumber", sqlParameters.ToArray()).FirstOrDefault();
            return oSpResponce_CheckAccreditationNumber;
            //if (Convert.ToString(userID) == string.Empty)
            //{
            //	return 0;
            //}
            //else
            //{
            //	return Convert.ToInt32(userID);
            //}
        }
        /// <summary>
        /// Forms the bot sign up.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>
        /// user identifier
        /// </returns>
        public int FormBotSignUp(User userView)
        {
            string spName = "[Users_SignUpForSE]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserId", SqlDbType.NVarChar, userView.AspNetUserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, userView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, userView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, userView.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, userView.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("CompanyWebsite", SqlDbType.NVarChar, userView.CompanyWebsite));
            sqlParameters.Add(DBClient.AddParameters("RecUserName", SqlDbType.NVarChar, userView.RecUserName));
            sqlParameters.Add(DBClient.AddParameters("RecPassword", SqlDbType.NVarChar, userView.RecPassword));
            sqlParameters.Add(DBClient.AddParameters("BSB", SqlDbType.NVarChar, userView.BSB));
            sqlParameters.Add(DBClient.AddParameters("AccountNumber", SqlDbType.NVarChar, userView.AccountNumber));
            sqlParameters.Add(DBClient.AddParameters("AccountName", SqlDbType.NVarChar, userView.AccountName));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, userView.ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, userView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("CECDesignerNumber", SqlDbType.NVarChar, userView.CECDesignerNumber));
            sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.NVarChar, userView.Signature));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, userView.UserTypeID));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, userView.IsActive));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, userView.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, userView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, userView.ResellerID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Logo", SqlDbType.NVarChar, userView.Logo));
            sqlParameters.Add(DBClient.AddParameters("Theme", SqlDbType.Int, userView.Theme));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, userView.FromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, userView.ToDate));
            sqlParameters.Add(DBClient.AddParameters("IsFirstLogin", SqlDbType.Bit, userView.IsFirstLogin));
            sqlParameters.Add(DBClient.AddParameters("SEDesigner", SqlDbType.Int, userView.SEDesigner));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("IsSTC", SqlDbType.Bit, userView.IsSTC));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserIdSE", SqlDbType.NVarChar, userView.AspNetUserIdSE));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, userView.RAMId));

            if (userView.IsSTC == true && userView.UserTypeID == 4)
                userView.IsPVDUser = true;

            sqlParameters.Add(DBClient.AddParameters("IsPVDUser", SqlDbType.Bit, userView.IsPVDUser));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, userView.IsSWHUser));
            sqlParameters.Add(DBClient.AddParameters("IsVEECUser", SqlDbType.Bit, userView.IsVEECUser));
            sqlParameters.Add(DBClient.AddParameters("IsAutoRequest", SqlDbType.Bit, userView.IsAutoRequest));
            sqlParameters.Add(DBClient.AddParameters("SESelfie", SqlDbType.NVarChar, userView.SESelfie));

            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Checks the accreditation number exist.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// object result
        /// </returns>
        public object CheckAccreditationNumberExist(string accreditationNumber, int? userId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.NVarChar, userId));
            object lstaccreditationNumber = CommonDAL.ExecuteScalar("Users_CheckAccreditationNumberExist", sqlParameters.ToArray());
            return lstaccreditationNumber;
        }

        /// <summary>
        /// Checks the accreditation number exists in accredited installers.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <returns>
        /// Accredited Installers list
        /// </returns>
        public List<AccreditedInstallers> CheckAccreditationNumberExistsInAccreditedInstallers(string accreditationNumber = "", string LicenseNumber = "", bool IsPvdUser = false, bool IsSWHUser = false, bool IsFindSWHInstaller = false)
        {
            string spName = "[AccreditedInstallers_CheckAccreditationNumberExist]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("LicensedElectricianNumber", SqlDbType.NVarChar, LicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPVDUser", SqlDbType.Bit, IsPvdUser));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, IsSWHUser));
            sqlParameters.Add(DBClient.AddParameters("IsFindSWHInstaller", SqlDbType.Bit, IsFindSWHInstaller));
            IList<AccreditedInstallers> usersList = CommonDAL.ExecuteProcedure<AccreditedInstallers>(spName, sqlParameters.ToArray());
            return usersList.ToList();
        }

        /// <summary>
        /// Gets the multiple fco group drop down.
        /// </summary>
        /// <returns>
        /// group list
        /// </returns>
        public List<FCOGroup1> GetMultipleFCOGroupDropDown()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<FCOGroup1> lstFCOUserGroup = CommonDAL.ExecuteProcedure<FCOGroup1>("FCOGroup_BindDropdown", sqlParameters.ToArray()).ToList();
            if (lstFCOUserGroup == null)
            {
                lstFCOUserGroup = new List<FCOGroup1>();
            }
            return lstFCOUserGroup;
        }

        /// <summary>
        /// Inserts the selected fco group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        public void InsertSelectedFCOGroup(int UserId, string FCOGroupId)
        {
            string spName = "[FCOUserGroup_InsertFCOGroupID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, false));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.NVarChar, FCOGroupId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the multiple fco group drop down by user identifier.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="userTypeID">The user type identifier.</param>
        /// <returns>
        /// group list
        /// </returns>
        public List<FCOGroup1> GetMultipleFCOGroupDropDownByUserId(int UserId, int userTypeID)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeID));
            List<FCOGroup1> lstFCOUserGroup = CommonDAL.ExecuteProcedure<FCOGroup1>("FCOUserGroup_GetFCOGroupBYUserID", sqlParameters.ToArray()).ToList();

            return lstFCOUserGroup;
        }

        /// <summary>
        /// Deletes the selected fco group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void DeleteSelectedFCOGroup(int UserId)
        {
            string spName = "[FCOUserGroup_DeleteFCOGroupBYUserID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the postal address.
        /// </summary>
        /// <returns>
        /// address list
        /// </returns>
        public List<PostalAddressView> GetPostalAddress()
        {
            string spName = "[PostalAddress_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<PostalAddressView> postTypeList = CommonDAL.ExecuteProcedure<PostalAddressView>(spName, sqlParameters.ToArray());
            return postTypeList.ToList();
        }

        /// <summary>
        /// Emails the mapping insert update.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        public void EmailMappingInsertUpdate(int id, int loggedInUserId)
        {
            string spName = "[EmailMapping_InsertUpdate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("id_acct", SqlDbType.Int, id));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, loggedInUserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Emails the mapping insert update.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        public void Rec_EmailMappingInsertUpdate(string email, string mail_inc_host, string mail_inc_login, string mail_inc_pass, int? mail_inc_port, string mail_out_host, int? mail_out_port, string signature, string signature_opt, long mailbox_size, int loggedInUserId, int def_timezone)
        {
            string spName = "[Rec_EmailMapping_InsertUpdate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, loggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.VarChar, email));
            sqlParameters.Add(DBClient.AddParameters("mail_inc_host", SqlDbType.VarChar, mail_inc_host));
            sqlParameters.Add(DBClient.AddParameters("mail_inc_login", SqlDbType.VarChar, mail_inc_login));
            sqlParameters.Add(DBClient.AddParameters("mail_inc_pass", SqlDbType.VarChar, mail_inc_pass));
            sqlParameters.Add(DBClient.AddParameters("mail_inc_port", SqlDbType.Int, mail_inc_port));
            sqlParameters.Add(DBClient.AddParameters("mail_out_host", SqlDbType.VarChar, mail_out_host));
            sqlParameters.Add(DBClient.AddParameters("mail_out_port", SqlDbType.Int, mail_out_port));
            //sqlParameters.Add(DBClient.AddParameters("signature", SqlDbType.Text, signature));
            //sqlParameters.Add(DBClient.AddParameters("signature_opt", SqlDbType.SmallInt, signature_opt));
            sqlParameters.Add(DBClient.AddParameters("mailbox_size", SqlDbType.BigInt, mailbox_size));
            sqlParameters.Add(DBClient.AddParameters("def_timezone", SqlDbType.BigInt, def_timezone));

            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the account email exists.
        /// </summary>
        /// <param name="configurationEmail">The configuration email.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns>
        /// user identifier
        /// </returns>
        public int CheckAccountEmailExists(string configurationEmail, int loggedInUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ConfigurationEmail", SqlDbType.NVarChar, configurationEmail));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, loggedInUserId));
            object lstUser = CommonDAL.ExecuteScalar("awm_accounts_CheckAccountEmailExists", sqlParameters.ToArray());
            return Convert.ToInt32(lstUser);
        }

        /// <summary>
        /// Gets the name of the reseller identifier by login company.
        /// </summary>
        /// <param name="loginCompanyName">Name of the login company.</param>
        /// <returns>
        /// reseller identifier
        /// </returns>
        public int GetResellerIdByLoginCompanyName(string loginCompanyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, loginCompanyName));
            object lstUser = CommonDAL.ExecuteScalar("Resellerdetails_GetResellerID", sqlParameters.ToArray());
            return Convert.ToInt32(lstUser);
        }

        public List<VEECInstaller> CheckVEECInstallerExists(string LicenseNumber)
        {
            string spName = "[VEECInstallers_CheckLiecenseNumberExist]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LicensedElectricianNumber", SqlDbType.NVarChar, LicenseNumber));
            List<VEECInstaller> usersList = CommonDAL.ExecuteProcedure<VEECInstaller>(spName, sqlParameters.ToArray()).ToList();
            return usersList.ToList();
        }

        /// <summary>
        /// Gets the postal address detail.
        /// </summary>
        /// <param name="postalAddressID">The postal address identifier.</param>
        /// <returns>
        /// address list
        /// </returns>
        public List<PostalAddressView> GetPostalAddressDetail(int postalAddressID)
        {
            string spName = "[PostalAddress_GetPostalAddressDetail]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, postalAddressID));
            IList<PostalAddressView> postTypeList = CommonDAL.ExecuteProcedure<PostalAddressView>(spName, sqlParameters.ToArray());
            return postTypeList.ToList();
        }

        /// <summary>
        /// Logins the user email details.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet LoginUserEmailDetails(int userID)
        {
            string spName = "[EmailMapping_GetLoginEmailDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet emailDetails = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray()); ////CommonDAL.ExecuteProcedure<Entity.Email.EmailSignup>(spName, sqlParameters.ToArray());
            return emailDetails;
        }

        /// <summary>
        /// Rec Email details
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataSet RecUserEmailDetails(int userID)
        {
            string spName = "[RecEmailMapping_GetRecEmailDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet emailDetails = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray()); ////CommonDAL.ExecuteProcedure<Entity.Email.EmailSignup>(spName, sqlParameters.ToArray());
            return emailDetails;
        }

        /// <summary>
        /// Checks the login company name exists.
        /// </summary>
        /// <param name="loginCompanyName">Name of the login company.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <returns>
        /// company identifier
        /// </returns>
        public int CheckLoginCompanyNameExists(string loginCompanyName, int? resellerID = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, loginCompanyName));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerID));
            object lstUser = CommonDAL.ExecuteScalar("ResellerDetails_CheckLoginCompanyNameExist", sqlParameters.ToArray());
            return Convert.ToInt32(lstUser);
        }

        /// <summary>
        /// Gets the user by solar company identifier.
        /// </summary>
        /// <param name="scID">The sc identifier.</param>
        /// <returns>
        /// user object
        /// </returns>
        public User GetUserBySolarCompanyId(int? scID)
        {
            scID = scID != null && scID > 0 ? scID : 0;
            User user = CommonDAL.SelectObject<User>("Users_GetUserBySolarCompanyId", DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, scID));
            return user;
        }

        /// <summary>
        /// Gets the postal delivery name by identifier.
        /// </summary>
        /// <param name="postalAddressID">The postal address identifier.</param>
        /// <returns>
        /// delivery name
        /// </returns>
        public string GetPostalDeliveryNameByID(int postalAddressID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, postalAddressID));
            return Convert.ToString(CommonDAL.ExecuteScalar("PostalAddress_GetPostalDeliveryNameByID", sqlParameters.ToArray()));
        }

        public string GetStreetTypeNameByID(int StreetTypeID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StreeTypeID", SqlDbType.Int, StreetTypeID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetStreetTypeNameByID", sqlParameters.ToArray()));
        }

        public string GetUnitTypeNameByID(int UnitTypeID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, UnitTypeID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetUnitTypeNameByID", sqlParameters.ToArray()));
        }

        public string GetRoleNameByID(int RoleID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RoleID", SqlDbType.Int, RoleID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetRoleNameByID", sqlParameters.ToArray()));
        }

        public string GetResellerNamebyID(int ResellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetResellerNameByID", sqlParameters.ToArray()));
        }

        public string GetRAMNameByID(int RAMID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetRAMNameByID", sqlParameters.ToArray()));
        }

        public string GetSolarCompanyNamebyID(int SolarCompanyID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetSolarCompanyNameByID", sqlParameters.ToArray()));
        }

        public string GetFCOGroupNameByGroupID(int FCOGroupID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOGroupID", SqlDbType.Int, FCOGroupID));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetFCOGroupNameByGroupID", sqlParameters.ToArray()));
        }
        /// <summary>
        /// Creates the send request for solar sub contractor.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>create solar sub contracter id</returns>
        public int CreateSendRequestForSolarSubContractor(User userView)
        {
            string spName = "[SCASSCMapping_SendRequest]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SSCID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Gets the user identifier by company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <returns>
        /// company abn
        /// </returns>
        public int GetUserIdByCompanyABN(string companyABN)
        {
            string spName = "[SCASSCMapping_GetSSCID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, companyABN.Replace(" ", "")));
            object sscID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(sscID);
        }

        /// <summary>
        /// Gets the user identifier by company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <returns>
        /// company name
        /// </returns>
        public string GetComapnyNameByABN(string companyABN)
        {
            string spName = "[Users_GetComapnyNameByABN]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, companyABN.Replace(" ", "")));
            object lstUser = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(lstUser);
        }

        /// <summary>
        /// Sscids the exist inscassc mapping.
        /// </summary>
        /// <param name="sscID">The SSC identifier.</param>
        /// <param name="loginID">The login identifier.</param>
        /// <returns>login identifier</returns>
        public object SSCIDExistINSCASSCMapping(int sscID, int loginID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, sscID));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, loginID));
            object lstUser = CommonDAL.ExecuteScalar("SCASSCMapping_SSCIDExist", sqlParameters.ToArray());
            return lstUser;
        }

        /// <summary>
        /// Inserts the status.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void InsertStatus(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            CommonDAL.Crud("Users_InsertStatus", sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the exist accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// accreditation number
        /// </returns>
        public int CheckExistAccreditationNumber(string accreditationNumber, int solarCompanyId, int UserTypeID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("AccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeID));
            object userID = CommonDAL.ExecuteScalar("SolarElectrician_CheckExistAccreditationNumber", sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }
        //public bool SolarElectricianRequestIsExists(string AccreditationNumber, string LicenseNumber)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("AccreditationNumber", SqlDbType.NVarChar, AccreditationNumber));
        //    sqlParameters.Add(DBClient.AddParameters("LicenseNumber", SqlDbType.NVarChar, LicenseNumber));
        //    object IsExists = CommonDAL.ExecuteScalar("SolarElectricianRequestIsExists", sqlParameters.ToArray());
        //    return Convert.ToBoolean(IsExists);
        //}
        public bool SolarElectricianRequestIsExists(int solarCompanyId, int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyid", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Userid", SqlDbType.Int, userId));
            object IsExists = CommonDAL.ExecuteScalar("SolarElectricianRequestIsExists", sqlParameters.ToArray());
            return Convert.ToBoolean(IsExists);
        }
        public bool CheckAutoRequestStatusOverUserId(int userId, bool IsApproved, bool IsGet)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("userid", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("IsApproved", SqlDbType.Bit, IsApproved));
            sqlParameters.Add(DBClient.AddParameters("IsGet", SqlDbType.Bit, IsGet));
            object IsExists = CommonDAL.ExecuteScalar("CheckAutoRequestStatusOverUserId", sqlParameters.ToArray());
            return Convert.ToBoolean(IsExists);
        }


        /// <summary>
        /// Scases the insert.
        /// </summary>
        /// <param name="userView">The user view.</param>
        public void SCASEInsert(User userView)
        {
            string spName = "[SolarElectrician_SCASEInsert]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, userView.ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, userView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("CECDesignerNumber", SqlDbType.NVarChar, userView.CECDesignerNumber));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, userView.IsActive));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, userView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("SEDesigner", SqlDbType.Int, userView.SEDesigner));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, userView.Email));
            sqlParameters.Add(DBClient.AddParameters("Note", SqlDbType.NVarChar, userView.Note));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Inserts the sc aas se.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>
        /// sca to se
        /// </returns>
        public int InsertSCAasSE(User userView)
        {
            string spName = "[User_SCASEInsert]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("AspNetUserId", SqlDbType.NVarChar, userView.AspNetUserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, userView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, userView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, userView.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, userView.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("CompanyWebsite", SqlDbType.NVarChar, userView.CompanyWebsite));
            sqlParameters.Add(DBClient.AddParameters("RecUserName", SqlDbType.NVarChar, userView.RecUserName));
            sqlParameters.Add(DBClient.AddParameters("RecPassword", SqlDbType.NVarChar, userView.RecPassword));
            sqlParameters.Add(DBClient.AddParameters("BSB", SqlDbType.NVarChar, userView.BSB));
            sqlParameters.Add(DBClient.AddParameters("AccountNumber", SqlDbType.NVarChar, userView.AccountNumber));
            sqlParameters.Add(DBClient.AddParameters("AccountName", SqlDbType.NVarChar, userView.AccountName));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, userView.ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, userView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("CECDesignerNumber", SqlDbType.NVarChar, userView.CECDesignerNumber));
            sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.NVarChar, userView.Signature));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, userView.UserTypeID));
            sqlParameters.Add(DBClient.AddParameters("IsActive", SqlDbType.Bit, userView.IsActive));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, userView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, userView.ResellerID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Logo", SqlDbType.NVarChar, userView.Logo));
            sqlParameters.Add(DBClient.AddParameters("Theme", SqlDbType.Int, userView.Theme));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, userView.FromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, userView.ToDate));
            sqlParameters.Add(DBClient.AddParameters("IsFirstLogin", SqlDbType.Bit, userView.IsFirstLogin));
            sqlParameters.Add(DBClient.AddParameters("RoleID", SqlDbType.Int, userView.RoleID));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, userView.LoginCompanyName));
            sqlParameters.Add(DBClient.AddParameters("SEDesigner", SqlDbType.Int, userView.SEDesigner));
            sqlParameters.Add(DBClient.AddParameters("IsSTC", SqlDbType.Bit, userView.IsSTC));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            //sqlParameters.Add(DBClient.AddParameters("SESelfie", SqlDbType.NVarChar, userView.SESelfie));
            object seID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(seID);
        }

        /// <summary>
        /// Gets the isst cfor sca.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ssc to sca</returns>
        public int GetISSTCforSCA(int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.NVarChar, userId));
            object lstUser = CommonDAL.ExecuteScalar("Users_chkExistsSCAisSTC", sqlParameters.ToArray());
            return Convert.ToInt32(lstUser);
        }

        /// <summary>
        /// Gets the solar electrician by identifier.
        /// </summary>
        /// <param name="solarElectricianId">The solar electrician identifier.</param>
        /// <returns>
        /// electrician identifier
        /// </returns>
        public DataSet GetSolarElectricianById(int solarElectricianId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarElectricianId", SqlDbType.Int, solarElectricianId));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("SolarElectrician_GetSolarElectricianById", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Inserts the selected reseller fco group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        public void InsertSelectedResellerFCOGroup(int UserId, string FCOGroupId)
        {
            string spName = "[ResellersFCOGroup_InsertFCOGroupID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("FCOGroupId", SqlDbType.NVarChar, FCOGroupId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the selected reseller group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void DeleteSelectedResellerGroup(int UserId)
        {
            string spName = "[ResellersFCOGroup_DeleteFCOGroupBYUserID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the logo.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void DeleteLogo(int UserId)
        {
            string spName = "[ResellerDetails_DeleteLogo]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the signature.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void DeleteSignature(int UserId)
        {
            string spName = "[Users_DeleteSingature]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the Selfie.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        public void DeleteSelfie(int UserId)
        {
            string spName = "[Users_DeleteSelfie]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the fco by reseller identifier.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>
        /// reseller identifier
        /// </returns>
        public List<User> GetFCOByResellerId(int ResellerId)
        {
            string spName = "[User_GetResellerWiseFCOUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            IList<User> lstFcoUsers = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return lstFcoUsers.ToList();
        }

        /// <summary>
        /// Gets the reseller user record credentials by reseller identifier.
        /// </summary>
        /// <param name="ResellerID">The reseller identifier.</param>
        /// <returns>
        /// user object
        /// </returns>
        public RECAccount GetResellerUserRECCredentialsByResellerID(int ResellerID)
        {
            RECAccount user = CommonDAL.SelectObject<RECAccount>("[GetResellerUserRECCredentialsByResellerID]", DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            return user;
        }

        /// <summary>
        /// Gets the reseller user record credentials by reseller identifier.
        /// </summary>
        /// <param name="ResellerID">The reseller identifier.</param>
        /// <returns>
        /// user object
        /// </returns>
        public List<RECAccount> GetAllResellerUserRECCredentialsByResellerID(int ResellerID)
        {
            List<RECAccount> user = new List<RECAccount>();
            DataSet ds = CommonDAL.ExecuteDataSet("[GetResellerAllRECCredentialsByResellerID]", DBClient.AddParameters("ResellerID", SqlDbType.Int, ResellerID));
            if (ds != null && ds.Tables.Count > 0)
            {
                user = CommonDAL.DataTableToList<RECAccount>(ds.Tables[0]);
            }
            return user;
        }

        /// <summary>
        /// Updates the device token.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// string type
        /// </returns>
        public string UpdateDeviceToken(int userID, string accessToken, string type, string DeviceInfo = "")
        {
            string spName = "[UpdateDeviceToken]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("AccessToken", SqlDbType.NVarChar, accessToken));
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.NVarChar, type));
            sqlParameters.Add(DBClient.AddParameters("DeviceInfo", SqlDbType.NVarChar, DeviceInfo));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataset != null && dataset.Tables.Count > 0)
            {
                return Convert.ToString(dataset.Tables[0].Rows[0]["apiToken"]);
            }
            return null;
        }
        /// <summary>
        /// Updates the device token.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// string type
        /// </returns>
        public string UpdateDeviceAllowAccessTimmer(int userID, string accessToken, string DeviceInfo = "")
        {
            string spName = "[api_UpdateLastAccessOverToken]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("userid", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("token", SqlDbType.NVarChar, accessToken));
            sqlParameters.Add(DBClient.AddParameters("deviceinfo", SqlDbType.NVarChar, DeviceInfo));
            sqlParameters.Add(DBClient.AddParameters("lastaccess", SqlDbType.DateTime, DateTime.Now));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataset != null && dataset.Tables.Count > 0)
            {
                return Convert.ToString(dataset.Tables[0].Rows[0]["apiToken"]);
            }
            return null;
        }
        /// <summary>
        /// Determines whether [is valid token] [the specified user identifier].
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiToken">The API token.</param>
        /// <returns>boolean result</returns>
        public bool IsValidToken(int userID, string apiToken)
        {
            string spName = "[CheckUserToken]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("ApiToken", SqlDbType.NVarChar, apiToken));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataset.Tables.Count > 0 ? true : false;
        }

        /// <summary>
        /// Gets the reseller identifier by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user identifier.</returns>
        public int GetResellerIdByUserId(int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.NVarChar, userId));
            object resellerId = CommonDAL.ExecuteScalar("GetResellerIdByUserId", sqlParameters.ToArray());
            return Convert.ToInt32(resellerId);
        }

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="ElectricianID">The electrician identifier.</param>
        /// <param name="SolarCompanyID">The solar company identifier.</param>
        public void DeleteRequestToSE(int ElectricianID, int SolarCompanyID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.NVarChar, ElectricianID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyID));
            CommonDAL.Crud("User_DeleteRequestToSE", sqlParameters.ToArray());
        }

        public List<User> GetSolarSubContractor()
        {
            string spName = "[User_BindSolarSubConractorDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<User> lstSolarSubContractor = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return lstSolarSubContractor.ToList();
        }

        /// <summary>
        /// Gets the raram email address.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public string GetRARAMEmailAddress(int SolarCompanyId)
        {
            string spName = "[Users_GetRARAMEmailAddress]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            object lstUser = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(lstUser);
        }

        /// <summary>
        /// Gets the fsafco email addresses.
        /// </summary>
        /// <returns></returns>
        public string GetFSAFCOEmailAddresses()
        {
            string spName = "[Users_GetFSAFCOEmailAddresses]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            object lstUser = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(lstUser);
        }

        public string GetFSAEmailAddresses()
        {
            string spName = "[Users_GetFSAEmailAddresses]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            object lstUser = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(lstUser);
        }

        /// <summary>
        /// Gets the ra email address.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns></returns>
        public string GetRAEmailAddress(int ResellerId)
        {
            string spName = "[Users_GetRAEmailAddress]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            object lstUser = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(lstUser);
        }

        public List<InstallerDesignerView> GetInstallerDesignerAddByProfile(int UserId, int userType, int pageSize, int pageNumber, string sortCol, string sortDir, int solarCompanyId, string name, string accreditationnumber, int SERole, int SendBy, string LicenseNumber, bool IsSWHUser)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.VarChar, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            sqlParameters.Add(DBClient.AddParameters("accreditationnumber", SqlDbType.NVarChar, accreditationnumber));
            sqlParameters.Add(DBClient.AddParameters("SERole", SqlDbType.TinyInt, SERole));
            sqlParameters.Add(DBClient.AddParameters("SendBy", SqlDbType.TinyInt, SendBy));
            sqlParameters.Add(DBClient.AddParameters("LicenseNumber", SqlDbType.NVarChar, LicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, IsSWHUser));
            List<InstallerDesignerView> lstInstallerDesigner = CommonDAL.ExecuteProcedure<InstallerDesignerView>("Users_InstallerDesignerAddByProfile", sqlParameters.ToArray()).ToList();
            return lstInstallerDesigner;
        }

        /// <summary>
        /// Add installer designer.
        /// </summary>
        /// <param name="installerDesignerView">Add installer designer.</param>
        /// <returns>
        /// installer designer view
        /// </returns>
        public int AddEditInstallerDesigner(InstallerDesignerView installerDesignerView, int jobId = 0, int profileType = 0, string signPath = null, int accreditedInstallerId = 0, int userId = 0)
        {
            string spName = "[InstallerDesigner_AddInstallerDesigner]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("InstallerDesignerId", SqlDbType.Int, installerDesignerView.InstallerDesignerId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, installerDesignerView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, installerDesignerView.LastName));
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.NVarChar, installerDesignerView.CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, installerDesignerView.Email));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, installerDesignerView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, installerDesignerView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, installerDesignerView.ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("SERole", SqlDbType.Int, installerDesignerView.SEDesignRoleId));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, installerDesignerView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, installerDesignerView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, installerDesignerView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, installerDesignerView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, installerDesignerView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, installerDesignerView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, installerDesignerView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, installerDesignerView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, installerDesignerView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, installerDesignerView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, installerDesignerView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("SESignature", SqlDbType.NVarChar, installerDesignerView.SESignature));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, installerDesignerView.CreatedBy == 0 ? ProjectSession.LoggedInUserId : installerDesignerView.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, installerDesignerView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, installerDesignerView.IsDeleted));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, installerDesignerView.SolarCompanyId));

            sqlParameters.Add(DBClient.AddParameters("BasicJobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("ProfileType", SqlDbType.Int, profileType));
            sqlParameters.Add(DBClient.AddParameters("JobSignPath", SqlDbType.NVarChar, signPath));
            sqlParameters.Add(DBClient.AddParameters("IsCECAccreditationNumberExist", SqlDbType.Bit, installerDesignerView.IsCECAccreditationNumberExist));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, installerDesignerView.IsSWHUser));
            sqlParameters.Add(DBClient.AddParameters("IsPVDUser", SqlDbType.Bit, installerDesignerView.IsPVDUser));

            sqlParameters.Add(DBClient.AddParameters("AccreditedInstallerId", SqlDbType.Int, accreditedInstallerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));

            object InstallerDesignerId = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(InstallerDesignerId);
        }

        public InstallerDesignerView GetInstallerDesigner(int InstallerDesignerId, int? JobId)
        {
            try
            {
                InstallerDesignerView installerDesignerView = CommonDAL.SelectObject<InstallerDesignerView>("InstallerDesigner_GetInstallerDesigner", DBClient.AddParameters("InstallerDesignerId", SqlDbType.Int, InstallerDesignerId), DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
                return installerDesignerView;
            }
            catch (Exception ex)
            {
                return new InstallerDesignerView();
            }
        }

        /// <summary>
        /// Checks the exist accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// accreditation number
        /// </returns>
        public int CheckExistAccreditationNumberForInstallerDesigner(string accreditationNumber, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("AccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            object userID = CommonDAL.ExecuteScalar("InstallerDesigner_CheckExistAccreditationNumber", sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="ElectricianID">The electrician identifier.</param>
        /// <param name="SolarCompanyID">The solar company identifier.</param>
        public void DeleteRequestToSEForInstallerDesigner(int ElectricianID, int SolarCompanyID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.NVarChar, ElectricianID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyID));
            CommonDAL.Crud("InstallerDesigner_DeleteRequestToSE", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the se user With Status.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>Solar Electrician View</returns>
        public List<InstallerDesignerView> GetInstallerDesignerWithStatus(bool isInstaller, int companyId, int jobId, bool IsSubContractor)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, isInstaller));
            sqlParameters.Add(DBClient.AddParameters("CompanyId", SqlDbType.Int, companyId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("IsSubContractor", SqlDbType.Bit, IsSubContractor));
            List<InstallerDesignerView> lstInstallerDesignerView = CommonDAL.ExecuteProcedure<InstallerDesignerView>("InstallerDesigner_GetInstallerDesignerListInJob", sqlParameters.ToArray()).ToList();
            return lstInstallerDesignerView;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetInstallerDesignerById(int InstallerDesignerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InstallerDesignerId", SqlDbType.Int, InstallerDesignerId));
            DataSet dsInstallerDesigner = CommonDAL.ExecuteDataSet("InstallerDEsigner_GetInstallerDesignerById", sqlParameters.ToArray());
            return dsInstallerDesigner;
        }

        /// <summary>
        /// Gets the user of user type.
        /// </summary>
        /// <returns>user list</returns>
        public List<User> GetUserByUserType(int userTypeId)
        {
            string spName = "[User_GetUserByUserType]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            IList<User> userList = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return userList.ToList();
        }

        /// <summary>
        /// Gets the user by FSA.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user identifier</returns>
        public User GetUsersByFSA(int UserID)
        {
            User user = CommonDAL.SelectObject<User>("Users_GetUsersByFSA", DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            return user;
        }

        /// <summary>
        /// Gets the user of reseller.
        /// </summary>
        /// <returns>user list</returns>
        public List<User> GetUserByResellerId(int userTypeId, int resellerId)
        {
            string spName = "[User_GetUserByResellerId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            IList<User> userList = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return userList.ToList();
        }

        /// <summary>
        /// Gets the user of solar company.
        /// </summary>
        /// <returns>user list</returns>
        public List<User> GetUserBySolarCompanyId(int userTypeId, int solarCompanyId)
        {
            string spName = "[User_GetUserBySolarCompanyId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            IList<User> userList = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return userList.ToList();
        }

        /// <summary>
        /// Check Client Code Prefix
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <param name="clientCodePrefix">clientCodePrefix</param>
        /// <returns>Dataset</returns>
        public DataSet CheckClientCodePrefix(int userTypeId, int resellerId, int userId, string clientCodePrefix)
        {
            string spName = "[RARAM_CheckClientCodePrefix]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("ClientCodePrefix", SqlDbType.NVarChar, clientCodePrefix));
            DataSet dsCount = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsCount;
        }

        /// <summary>
        /// Get Client Number
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <returns>DataSet</returns>
        public DataSet GetClientNumber(int userTypeId, int resellerId, int userId, string existClientNumber, int solarCompanyId)
        {
            string spName = "[SolarCompany_GetClientNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("ClientNumber", SqlDbType.NVarChar, existClientNumber));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            DataSet dsClientNumber = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsClientNumber;
        }


        /// <summary>
        /// Update SolarCompany Users CheckInXero
        /// </summary>
        /// <param name="userView">user table value</param>
        /// <returns>Returns the UserID</returns>
        public int UpdateSolarCompanyUsersCheckInXero(User userView)
        {
            string spName = "[Users_UpdateSolarCompanyUsersCheckInXero]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userView.UserId));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, userView.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, userView.LastName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, userView.Phone));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.NVarChar, userView.Mobile));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, userView.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, userView.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, userView.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.NVarChar, userView.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.NVarChar, userView.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.NVarChar, userView.StreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, userView.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.NVarChar, userView.Town));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.NVarChar, userView.State));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.NVarChar, userView.PostCode));
            sqlParameters.Add(DBClient.AddParameters("CompanyWebsite", SqlDbType.NVarChar, userView.CompanyWebsite));
            sqlParameters.Add(DBClient.AddParameters("BSB", SqlDbType.NVarChar, userView.BSB));
            sqlParameters.Add(DBClient.AddParameters("AccountNumber", SqlDbType.NVarChar, userView.AccountNumber));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, userView.ModifiedBy));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, userView.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, userView.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.NVarChar, userView.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPostalAddress", SqlDbType.Bit, userView.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ClientNumber", SqlDbType.NVarChar, userView.ClientNumber));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.NVarChar, userView.RAMId));
            sqlParameters.Add(DBClient.AddParameters("CustomCompanyName", SqlDbType.NVarChar, userView.CustomCompanyName));
            sqlParameters.Add(DBClient.AddParameters("IsWholeSaler", SqlDbType.NVarChar, userView.IsWholeSaler));
            sqlParameters.Add(DBClient.AddParameters("WholesalerFirstName", SqlDbType.NVarChar, userView.WholesalerFirstName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerLastName", SqlDbType.NVarChar, userView.WholesalerLastName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerEmail", SqlDbType.NVarChar, userView.WholesalerEmail));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPhone", SqlDbType.NVarChar, userView.WholesalerPhone));
            sqlParameters.Add(DBClient.AddParameters("WholesalerMobile", SqlDbType.NVarChar, userView.WholesalerMobile));
            sqlParameters.Add(DBClient.AddParameters("WholesalerCompanyABN", SqlDbType.NVarChar, userView.WholesalerCompanyABN));
            sqlParameters.Add(DBClient.AddParameters("WholesalerCompanyName", SqlDbType.NVarChar, userView.WholesalerCompanyName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerIsPostalAddress", SqlDbType.Bit, Convert.ToBoolean(userView.WholesalerIsPostalAddress)));
            sqlParameters.Add(DBClient.AddParameters("WholesalerUnitTypeID", SqlDbType.Int, userView.WholesalerUnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerUnitNumber", SqlDbType.NVarChar, userView.WholesalerUnitNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetNumber", SqlDbType.NVarChar, userView.WholesalerStreetNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetName", SqlDbType.NVarChar, userView.WholesalerStreetName));
            sqlParameters.Add(DBClient.AddParameters("WholesalerStreetTypeID", SqlDbType.Int, userView.WholesalerStreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostalAddressID", SqlDbType.Int, userView.WholesalerPostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostalDeliveryNumber", SqlDbType.NVarChar, userView.WholesalerPostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerTown", SqlDbType.NVarChar, userView.WholesalerTown));
            sqlParameters.Add(DBClient.AddParameters("WholesalerState", SqlDbType.NVarChar, userView.WholesalerState));
            sqlParameters.Add(DBClient.AddParameters("WholesalerPostCode", SqlDbType.NVarChar, userView.WholesalerPostCode));
            sqlParameters.Add(DBClient.AddParameters("WholesalerBSB", SqlDbType.NVarChar, userView.WholesalerBSB));
            sqlParameters.Add(DBClient.AddParameters("WholesalerAccountNumber", SqlDbType.NVarChar, userView.WholesalerAccountNumber));
            sqlParameters.Add(DBClient.AddParameters("WholesalerAccountName", SqlDbType.NVarChar, userView.WholesalerAccountName));

            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());

            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Get All SolarCompany Details
        /// </summary>
        /// <param name="solarCompanyIds">solarCompanyIds</param>
        /// <returns>DataSet</returns>
        public DataSet GetAllSolarCompanyDetails(string solarCompanyIds)
        {
            string spName = "[GetAllSolarCompanyDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, solarCompanyIds));
            DataSet dsClientNumber = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsClientNumber;
        }

        /// <summary>
        /// GetApiVersion
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetApiVersion()
        {
            string spName = "[GetApiVersion]";
            DataSet dsVersion = CommonDAL.ExecuteDataSet(spName, null);
            return dsVersion;
        }

        public DataSet GetDetailsForTermsAndCondition(int UserTypeID, int SolarCompanyId)
        {
            string spName = "[GetDetailsForTermsAndCondition]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            DataSet dsUserDetailsForTAndC = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsUserDetailsForTAndC;
        }

        public List<AccreditedInstallers> CheckExistAccreditationNumber_VendorApi(string accreditationNumber, string FirstName, string LastName)
        {
            string spName = "[CheckExistAccreditationNumber_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("AccreditationNumber", SqlDbType.NVarChar, accreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, LastName));
            IList<AccreditedInstallers> usersList = CommonDAL.ExecuteProcedure<AccreditedInstallers>(spName, sqlParameters.ToArray());
            return usersList.ToList();
        }

        /// <summary>
        /// Checks the exist accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// accreditation number
        /// </returns>
        public int CheckExistLicenseNumberForSWHInstaller(string licenseNumber, int solarCompanyId, string email)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LicenseNumber", SqlDbType.NVarChar, licenseNumber));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, email));
            object userID = CommonDAL.ExecuteScalar("SWHInstaller_CheckExistLicenseNumber", sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Restores the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int RestoreUser(string userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.NVarChar, userId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            object result = CommonDAL.ExecuteScalar("RestoreUser", sqlParameters.ToArray());
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Updates the job installer designer identifier.
        /// </summary>
        /// <param name="installerDesignerId">The installer designer identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="profileType">Type of the profile.</param>
        /// <param name="IsSWHUser">if set to <c>true</c> [is SWH user].</param>
        /// <returns></returns>
        public InstallerDesignerView UpdateJob_InstallerDesignerId(int installerDesignerId, int jobId, int profileType, bool IsSWHUser, int userId = 0)
        {
            InstallerDesignerView installerDesignerView = CommonDAL.SelectObject<InstallerDesignerView>("UpdateJobDetails_InstallerDesigner",
                DBClient.AddParameters("InstallerDesignerId", SqlDbType.Int, installerDesignerId),
                DBClient.AddParameters("BasicJobID", SqlDbType.Int, jobId),
                DBClient.AddParameters("ProfileType", SqlDbType.Int, profileType),
                DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, IsSWHUser),
                DBClient.AddParameters("UserId", SqlDbType.Int, userId)
                );
            return installerDesignerView;
        }

        /// <summary>
        /// Gets the electrician list.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        public List<JobElectricians> GetElectricianList(int companyId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, companyId));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            List<JobElectricians> lstJobElectricians = CommonDAL.ExecuteProcedure<JobElectricians>("GetElectricianListInJob", sqlParameters.ToArray()).ToList();
            return lstJobElectricians;
        }

        /// <summary>
        /// Deletes the custom electrician.
        /// </summary>
        /// <param name="jobElectricianId">The job electrician identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        public void DeleteCustomElectrician(int jobElectricianId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobElectricianId", SqlDbType.Int, jobElectricianId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            CommonDAL.Crud("Job_DeleteCustomElectrician", sqlParameters.ToArray());
        }

        public IList<InstallerDesignerView> SWHInstallerList_ByLicenseNumber(string licenseNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LicenseNumber", SqlDbType.NVarChar, licenseNumber));
            IList<InstallerDesignerView> lstSWHIntsaller = CommonDAL.ExecuteProcedure<InstallerDesignerView>("SWHInstaller_ListByLicenseNumber", sqlParameters.ToArray()).ToList();
            return lstSWHIntsaller;
        }

        public DataSet UpdateJob_JobElectricianId(int jobId, int solarCompanyId, int jobElectricianId, bool isCustomElectrician, int CreatedBy = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobElectricianId", SqlDbType.Int, jobElectricianId));
            sqlParameters.Add(DBClient.AddParameters("IsCustomElectrician", SqlDbType.Bit, isCustomElectrician));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy > 0 ? CreatedBy : ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateJob_JobElectricianId", sqlParameters.ToArray());
            return ds;
        }
        public void UpdateUsers_SaveIsNewViewerUserWise(bool IsNewViewer, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsNewViewer", SqlDbType.Bit, IsNewViewer));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            CommonDAL.Crud("UpdateUsers_SaveIsNewViewerUserWise", sqlParameters.ToArray());
        }
        public void User_UpdateApiTokenFromResetPassword(string Id)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.NVarChar, Id));
                CommonDAL.Crud("User_UpdateApiTokenFromResetPassword", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
            }
        }

        public void UploadProfilePhoto(int solarCompanyId, string profilePhoto)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ProfilePhoto", SqlDbType.NVarChar, profilePhoto));
            CommonDAL.Crud("UploadProfilePhoto", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get Userwise  grid details 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<UserWiseGridConfiguration> GetUserWiseGridConfiguration(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            return CommonDAL.ExecuteProcedure<UserWiseGridConfiguration>("GetUserWiseGridConfiguration", sqlParameters.ToArray()).ToList();
        }


        /// <summary>
        /// Insert / update userwise grid configuartion
        /// </summary>
        /// <param name="userWiseGridConfiguration"></param>
        /// <returns></returns>
        public int InsertUpdateUserWiseGridConfiguration(UserWiseGridConfiguration userWiseGridConfiguration)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsKendoGrid", SqlDbType.Bit, userWiseGridConfiguration.IsKendoGrid));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, userWiseGridConfiguration.PageSize));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userWiseGridConfiguration.UserId));
            sqlParameters.Add(DBClient.AddParameters("UserWiseGridConfigurationId", SqlDbType.Int, userWiseGridConfiguration.UserWiseGridConfigurationId));
            sqlParameters.Add(DBClient.AddParameters("ViewPageId", SqlDbType.Int, userWiseGridConfiguration.ViewPageId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("InsertUpdateUserWiseGridConfiguration", sqlParameters.ToArray()));
        }

        /// <summary>
        /// Update user wise tabular view 
        /// </summary>
        /// <param name="isTabularView"></param>
        public void UpdateTabularViewConfiguration(bool isTabularView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsTabularView", SqlDbType.Bit, isTabularView));
            CommonDAL.ExecuteScalar("UpdateTabularViewConfiguration", sqlParameters.ToArray());
        }

        public List<UserDevice> GetUserDeviceInfo(int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            List<UserDevice> lstUserDevice = CommonDAL.ExecuteProcedure<UserDevice>("GetUserDeviceDetails", sqlParameters.ToArray()).ToList();
            return lstUserDevice;
        }

        public void DeviceLogout(int id, bool isLogoutAll)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.Int, id));
            sqlParameters.Add(DBClient.AddParameters("IsLogoutAll", SqlDbType.Bit, isLogoutAll));
            CommonDAL.ExecuteScalar("DeviceLogout", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get all sca data for export excel
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="state">The state.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="status">The status.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RamId">The ram identifier.</param>
        /// <param name="mobile">The mobile.</param>
        /// <returns></returns>
        public DataSet ExportSCA(string name = "", string username = "", string state = "", string companyname = "", string email = "", string companyabn = "", int status = 0, int ResellerId = 0, int RamId = 0, string mobile = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.VarChar, name));
            sqlParameters.Add(DBClient.AddParameters("Username", SqlDbType.VarChar, username));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.VarChar, state));
            sqlParameters.Add(DBClient.AddParameters("Companyname", SqlDbType.VarChar, companyname));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.VarChar, email));
            sqlParameters.Add(DBClient.AddParameters("Companyabn", SqlDbType.VarChar, companyabn));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, status));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("Mobile", SqlDbType.VarChar, mobile));
            return CommonDAL.ExecuteDataSet("GetDataForExportSCA", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get rec Username and password from userid
        /// </summary>
        /// <param name="UserId"></param>
        public DataTable GetRecDataFromUserId(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            return CommonDAL.ExecuteDataSet("GetRecDataFromUserId", sqlParameters.ToArray()).Tables[0];
        }
        /// <summary>
        /// Get all android mobile app users details. 
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllAndroidMobileAppUsers()
        {
            return CommonDAL.ExecuteDataSet("GetAllAndroidMobileAppUsers", null).Tables[0];
        }

        public List<User> GetAllApprovedElectricianList()
        {
            List<User> lstSolarElectricians = CommonDAL.ExecuteProcedure<User>("GetApprovedElectricianList").ToList();
            return lstSolarElectricians;
        }
        /// <summary>
        ///insert logs for update contact details in xero
        /// </summary>
        /// <param name="historyMessages"></param>
        /// <param name="userid"></param>
        public void InsertLogForUpdateContact(string historyMessages, int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("historyMessages", SqlDbType.VarChar, historyMessages));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("InsertLogForUpdateContact", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get logs details about update contact details in xero. 
        /// </summary>
        /// <returns></returns>
        public List<UpdateContactXeroLog> GetLogsForUpdateContact(int userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            return CommonDAL.ExecuteProcedure<UpdateContactXeroLog>("GetLogsForUpdateContact", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Save masking value
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="isAllowedMAsking"></param>
        public void SaveMaskingValue(int resellerId, bool isAllowedMAsking)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("resellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("isAllowedMasking", SqlDbType.Bit, isAllowedMAsking));
            CommonDAL.Crud("SaveMaskingValue", sqlParameters.ToArray());
        }


        public bool GetAutoRECUploadAccess(int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            object isAccess = CommonDAL.ExecuteScalar("GetAutoRECUploadRole", sqlParameters.ToArray());
            if (isAccess != null)
                return Convert.ToBoolean(isAccess);
            else
                return false;
        }
        /// <summary>
        /// Inserts the status except note.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="Note">The note.</param>
        /// <param name="ComplainBy">The complain by.</param>
        /// <param name="IsSCDetailConfirm">if set to <c>true</c> [is sc detail confirm].</param>
        /// <param name="IsInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="IsSEDetailConfirm">if set to <c>true</c> [is se detail confirm].</param>
        public void InsertStatusWithoutNote(int UserId, byte? Status, int ComplainBy, bool IsSCDetailConfirm, bool IsInstaller, bool IsSEDetailConfirm, int? IsGSTSetByAdminUser = null)
        {
            string spName = "[Users_InsertStatusWithoutNotes]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, Status));
            sqlParameters.Add(DBClient.AddParameters("ComplainBy", SqlDbType.NVarChar, ComplainBy));
            sqlParameters.Add(DBClient.AddParameters("IsSCDetailConfirm", SqlDbType.Bit, IsSCDetailConfirm));
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, IsInstaller));
            sqlParameters.Add(DBClient.AddParameters("IsSEDetailConfirm", SqlDbType.Bit, IsSEDetailConfirm));
            sqlParameters.Add(DBClient.AddParameters("IsGSTSetByAdminUser", SqlDbType.Int, IsGSTSetByAdminUser));
            sqlParameters.Add(DBClient.AddParameters("ComplainDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }
        /// <summary>
        /// get user by userid
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet GetUserByUserId(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetUserByUserId", sqlParameters.ToArray());
            return ds;
        }

		public DataTable GetAcccountDetailByEmail(string emailID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("email", SqlDbType.VarChar, emailID));
            return CommonDAL.ExecuteDataSet("GetAcccountDetailByEmail", sqlParameters.ToArray()).Tables[0];
        }

        public DataSet GetAllUsersWithABN()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dsUsers = CommonDAL.ExecuteDataSet("GetAllUsersWithABN", sqlParameters.ToArray());
            return dsUsers;
        }

        public void UpdateEntityName(int UserId, string EntityName)
        {
            string spName = "UpdateEntityName";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("EntityName", SqlDbType.VarChar, EntityName));
            CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
        }
        /// <summary>
        /// get userid from cec accerdiationNumber
        /// </summary>
        /// <param name="CECAccreditationNumber"></param>
        /// <returns></returns>
        public string GetUserIdFromCECNumber(string CECAccreditationNumber,string ElectricalContractorsLicenseNumber, bool isPvdUser,bool isSWHUser)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CECAccreditationNumber", SqlDbType.VarChar, CECAccreditationNumber));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.VarChar, ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("IsPVDUser", SqlDbType.Bit, isPvdUser));
            sqlParameters.Add(DBClient.AddParameters("IsSWHUser", SqlDbType.Bit, isSWHUser));
            object Userid = CommonDAL.ExecuteScalar("GetUserIdFromCECAccreditationNumber", sqlParameters.ToArray());
            return Userid!=null? Userid.ToString():null;
           
        }
 public DataSet ChangeSCA(int jobID, string gbSCACode, int resellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("GB_SCACode", SqlDbType.VarChar, gbSCACode));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerID));
            return CommonDAL.ExecuteDataSet("ChangeSCAInJob", sqlParameters.ToArray());
        }

        public DataSet GetCECAccreditationNumberForInstallerDesignerElectrician(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            return CommonDAL.ExecuteDataSet("GetCECAccreditationNumberForInstallerDesignerElectrician", sqlParameters.ToArray());
        }
        /// <summary>
        /// Save SAAS user notes
        /// </summary>
        /// <param name="UserID">UserID</param>
        /// <param name="Notes">Notes</param>
        /// <param name="CreatedBy">Created By</param>
        /// <param name="CreatedDate">Created Date</param>
        public void SaveSAASUserNote(int UserID, string Notes, int CreatedBy, DateTime CreatedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SAASUserId", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("Note", SqlDbType.VarChar, Notes));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            CommonDAL.Crud("InserSAASUserNote", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get SAAS Userdetails
        /// </summary>
        /// <param name="SAASUserID"></param>
        /// <returns></returns>
        public DataSet GetSAASUserDetailsbyInvoicerID(int SAASUserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoicerID", SqlDbType.Int, SAASUserID));
            return CommonDAL.ExecuteDataSet("Users_GetSAASUserDetailbyInvoicerID", sqlParameters.ToArray());
        }
        /// <summary>
        /// Delete ContractFile from db
        /// </summary>
        /// <param name="InvoicerID"></param>
        /// <param name="DocumentPath"></param>
        public void DeleteContractFile(int InvoicerID, string DocumentPath)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoicerID", SqlDbType.Int, InvoicerID));
            sqlParameters.Add(DBClient.AddParameters("DocumentPath", SqlDbType.VarChar, DocumentPath));
            CommonDAL.Crud("DeleteContractFile", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get SAAS users Count
        /// </summary>
        /// <returns></returns>
        public int GetSAASUserCount()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            object SAASUsers = CommonDAL.ExecuteScalar("GetSAASUserCount", sqlParameters.ToArray());
            return Convert.ToInt32(SAASUsers);
        }

        public InstallerAuditDetails GetInstallerAuditDetails(int deviceID)
        {
            InstallerAuditDetails installerAuditDetails = CommonDAL.SelectObject<InstallerAuditDetails>("GetInstallerAuditDetails", DBClient.AddParameters("DeviceID", SqlDbType.Int, deviceID));
            return installerAuditDetails;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetinvoicerDetailsForSaasUsers(int? userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("GetInvoicerDetailsBy_UserId", sqlParameters.ToArray());
            return dsUsers;
        }

        public List<User> GetInvoicerDetailsList()
        {
            List<User> lstInvoicerDetails = CommonDAL.ExecuteProcedure<User>("Invoicer_BindDropdown").ToList();
            return lstInvoicerDetails;
        }

        public void AddNotesNotificaion(int receiverUserId, int jobId, string jobDetailLink, string notes, int sendBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("receiverUserId", SqlDbType.Int, receiverUserId));
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("jobDetailLink", SqlDbType.NVarChar, jobDetailLink));
            sqlParameters.Add(DBClient.AddParameters("notes", SqlDbType.NVarChar, notes));
            sqlParameters.Add(DBClient.AddParameters("sendBy", SqlDbType.Int, sendBy));
            CommonDAL.ExecuteScalar("AddNotesNotification", sqlParameters.ToArray());
        }

        public int GetNotificationCount(int loggedInUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("loggedInUserId", SqlDbType.Int, loggedInUserId));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("GetNotificationCount", sqlParameters.ToArray()));
        }

        public List<NotificationViewModel> GetNotificationList(int loggedInUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("loggedInUserId", SqlDbType.Int, loggedInUserId));
            return CommonDAL.ExecuteProcedure<NotificationViewModel>("GetNotificationList", sqlParameters.ToArray()).ToList();
        }

        public void RemoveNotification(string notificationIds, int loggedInUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("notificationIds", SqlDbType.NVarChar, notificationIds));
            sqlParameters.Add(DBClient.AddParameters("loggedInUserId", SqlDbType.Int, loggedInUserId));
            CommonDAL.ExecuteScalar("RemoveNotification", sqlParameters.ToArray());
        }
    }
}

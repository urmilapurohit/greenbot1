using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using FormBot.Entity.SolarElectrician;
using FormBot.Entity.VEEC;
using FormBot.Entity.KendoGrid;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service
{
    public interface IUserBAL
    {
        /// <summary>
        /// Gets the user by aspnet user identifier.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns>user object</returns>
        User GetUserByAspnetUserId(string aspnetUserId);

        /// <summary>
        /// Creates the specified user view.
        /// </summary>
        /// <param name="userView">user View</param>
        /// <returns>The UserId</returns>        
        int Create(User userView);

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirectoin">The sort directoin.</param>
        /// <param name="selectedUserTypeId">The selected user type identifier.</param>
        /// <param name="view">The view.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="reseller">The reseller.</param>
        /// <param name="solarCompany">The solar company.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="role">The role.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="userstatus">The userstatus.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="designernumber">The designernumber.</param>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        /// <returns>user list</returns>
        List<User> GetUserList(int userId, int pageNumber, int pageSize, string sortColumn, string sortDirectoin, string selectedUserTypeId, int view, string name, string username, string email, int userType, int reseller, int solarCompany, bool active, int role, string companyname, string companyabn, int userstatus, string accreditationnumber, string designernumber, string licensenumber, int electricianstatus, int ramId, string isAllScaJobView = "", string state = "", int? isallowspv = null, string mobile = "", string IsSaasUser = "");

        /// <summary>
        /// Gets the requesting sc afor SSC.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyname">The companyname.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="status">The status.</param>
        /// <returns>user list</returns>
        List<User> GetRequestingSCAforSSC(int userId, int pageNumber, int pageSize, string sortColumn, string sortDirection, string name, string email, string companyname, string companyabn, int status);

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
        List<User> GetRequestedSSCforSCA(int UserId, int SolarCompanyId, int PageNumber, int PageSize, string SortCol, string SortDir, string name, string email, string companyname, string companyabn, int status);

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
        /// <returns>user list</returns>
        List<User> GetSolarElectricianList(int userType, int pageSize, int pageNumber, string sortCol, string sortDir, string name, string username, string email, int solarCompanyId, string accreditationnumber, string designernumber, string licensenumber, int electricianstatus, string requestedusertype,int RoleId,int? isVerified, string mobile, string IsSaasUser);

        /// <summary>
        /// Gets the requested solar electrician list.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="electricianstatus">The electricianstatus.</param>
        /// <returns></returns>
        List<User> GetRequestedSolarElectricianList(int UserId, int userType, int pageSize, int pageNumber, string sortCol, string sortDir, int solarCompanyId, string name, string accreditationnumber, string licensenumber, int electricianstatus, string requestedusertype);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        void DeleteUser(int userID);

        /// <summary>
        /// Accepts the reject solar company request.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="Status">The status.</param>
        void SEAcceptRejectSolarCompanyRequest(int UserId, int SolarCompanyId, int Status);

        /// <summary>
        /// Sca the accept reject solar company request.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="Status">The status.</param>
        void SSCAcceptRejectSolarCompanyRequest(int UserId, int SolarCompanyId, int Status);

        /// <summary>
        /// Checks the email exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>object type</returns>
        object CheckEmailExists(string userName, int? userId = null);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>data set</returns>
        DataSet GetUserById(int? userID);

        /// <summary>
        /// Inserts the status note.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="Note">The note.</param>
        /// <param name="complainBy">The complain by.</param>
        /// <param name="IsSCDetailConfirm">if set to <c>true</c> [is sc detail confirm].</param>
        /// <param name="IsInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="IsSEDetailConfirm">if set to <c>true</c> [is se detail confirm].</param>
        void InsertStatusNote(int UserId, byte? Status, string Note, int complainBy, bool IsSCDetailConfirm, bool IsInstaller, bool IsSEDetailConfirm, int? IsGSTSetByAdminUser = null);

        /// <summary>
        /// Checks the company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>company identifier</returns>
        int CheckCompanyABN(string companyABN, int? userID = null, int? UserTypeId = null);

        /// <summary>
        /// Inserts the user document.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="DocumentPath">The document path.</param>
        void InsertUserDocument(int UserId, string DocumentPath, int DocumentType = 0,int DocLoc = 0);

        void InsertUpdateDocVerification(int UserId, bool IsSignatureVerified, bool IsSelfieVerified, bool IsDriverLicVerified, bool IsOtherDocVerified);

        /// <summary>
        /// Deletes the user document.
        /// </summary>
        /// <param name="UserDocumentID">The user document identifier.</param>
        void DeleteUserDocument(int UserDocumentID);

        /// <summary>
        /// Gets the user users document by user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>user list</returns>
        List<UserDocument> GetUserUsersDocumentByUserId(int userID);

        /// <summary>
        /// Gets the multiple fco group drop down.
        /// </summary>
        /// <returns>group list</returns>
        List<FCOGroup1> GetMultipleFCOGroupDropDown();

        /// <summary>
        /// Creates the send request.
        /// </summary>
        /// <param name="solarElectricianView">The solar electrician view.</param>
        /// <returns>electrician view</returns>
        int? CreateSendRequest(SolarElectricianView solarElectricianView);

        /// <summary>
        /// Checks the accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <returns>Accreditation Number</returns>
        //int CheckAccreditationNumber(string accreditationNumber, int UserTypeID);
        SpResponce_CheckAccreditationNumber CheckAccreditationNumber(string CECAccreditationNumber, string ElectricalContractorsLicenseNumber, string Email, bool IsPVD, bool IsSWH, bool IsSendRequet = false, int UserIdNotInclude = 0);

        /// <summary>
        /// Forms the bot sign up.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>user identifier</returns>
        int FormBotSignUp(User userView);

        /// <summary>
        /// Checks the accreditation number exist.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>object result</returns>
        object CheckAccreditationNumberExist(string accreditationNumber, int? userId = null);

        /// <summary>
        /// Checks the accreditation number exists in accredited installers.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <returns>Accredited Installers list</returns>
        List<AccreditedInstallers> CheckAccreditationNumberExistsInAccreditedInstallers(string accreditationNumber = "", string LicenseNumber = "", bool IsPvdUser = false, bool IsSWHUser = false, bool IsFindSWHInstaller = false);

        /// <summary>
        /// Inserts the selected fco group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        void InsertSelectedFCOGroup(int UserId, string FCOGroupId);

        /// <summary>
        /// Gets the multiple fco group drop down by user identifier.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="userTypeID">The user type identifier.</param>
        /// <returns>group list</returns>
        List<FCOGroup1> GetMultipleFCOGroupDropDownByUserId(int UserId, int userTypeID);

        /// <summary>
        /// Deletes the selected fco group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        void DeleteSelectedFCOGroup(int UserId);

        /// <summary>
        /// Gets the postal address.
        /// </summary>
        /// <returns>address list</returns>
        List<PostalAddressView> GetPostalAddress();

        /// <summary>
        /// Emails the mapping insert update.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        void EmailMappingInsertUpdate(int id, int loggedInUserId);

        /// <summary>
        /// Checks the account email exists.
        /// </summary>
        /// <param name="configurationEmail">The configuration email.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns>user identifier</returns>
        int CheckAccountEmailExists(string configurationEmail, int loggedInUserId);

        /// <summary>
        /// Gets the name of the reseller identifier by login company.
        /// </summary>
        /// <param name="loginCompanyName">Name of the login company.</param>
        /// <returns>reseller identifier</returns>
        int GetResellerIdByLoginCompanyName(string loginCompanyName);

        /// <summary>
        /// Gets the postal address detail.
        /// </summary>
        /// <param name="postalAddressID">The postal address identifier.</param>
        /// <returns>address list</returns>
        List<PostalAddressView> GetPostalAddressDetail(int postalAddressID);

        /// <summary>
        /// Logins the user email details.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>data set</returns>
        DataSet LoginUserEmailDetails(int userID);

        /// <summary>
        /// Checks the login company name exists.
        /// </summary>
        /// <param name="loginCompanyName">Name of the login company.</param>
        /// <param name="resellerID">The reseller identifier.</param>
        /// <returns>company identifier</returns>
        int CheckLoginCompanyNameExists(string loginCompanyName, int? resellerID = null);

        /// <summary>
        /// Gets the user by solar company identifier.
        /// </summary>
        /// <param name="scID">The sc identifier.</param>
        /// <returns>user object</returns>
        User GetUserBySolarCompanyId(int? scID);

        /// <summary>
        /// Gets the postal delivery name by identifier.
        /// </summary>
        /// <param name="postalAddressID">The postal address identifier.</param>
        /// <returns>delivery name</returns>
        string GetPostalDeliveryNameByID(int postalAddressID);

        /// <summary>
        /// Gets the Streettype name.
        /// </summary>
        /// <param name="StreetTypeID">The StreetType identifier.</param>
        /// <returns>StreetType name</returns>
        string GetStreetTypeNameByID(int StreetTypeID);

        /// <summary>
        /// Gets the Unittype name.
        /// </summary>
        /// <param name="UnitTypeID">The UnitType identifier.</param>
        /// <returns>UnitType name</returns>
        string GetUnitTypeNameByID(int UnitTypeID);

        /// <summary>
        /// Gets the Role name.
        /// </summary>
        /// <param name="RoleID">The Role identifier.</param>
        /// <returns>Role name</returns>
        string GetRoleNameByID(int RoleID);

        /// <summary>
        /// Gets the Reseller name.
        /// </summary>
        /// <param name="ResellerID">The Reseller identifier.</param>
        /// <returns>Reseller name</returns>
        string GetResellerNamebyID(int ResellerID);

        /// <summary>
        /// Gets the RAM name.
        /// </summary>
        /// <param name="RAMID">The RAM identifier.</param>
        /// <returns>RAM name</returns>
        string GetRAMNameByID(int RAMID);

        /// <summary>
        /// Gets the SolarCompany name.
        /// </summary>
        /// <param name="SolarCompanyID">The SolarCompany identifier.</param>
        /// <returns>SolarCompany name</returns>
        string GetSolarCompanyNamebyID(int SolarCompanyID);

        /// <summary>
        /// Gets the FCO group name.
        /// </summary>
        /// <param name="FCOGroupID">The FCO group identifier.</param>
        /// <returns>SolarCompany name</returns>
        string GetFCOGroupNameByGroupID(int FCOGroupID);

        /// <summary>
        /// Creates the send request for solar sub contractor.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>sub contractor identifier</returns>
        int CreateSendRequestForSolarSubContractor(User userView);

        /// <summary>
        /// Gets the user identifier by company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <returns>company abn</returns>
        int GetUserIdByCompanyABN(string companyABN);

        /// <summary>
        /// Gets the user identifier by company abn.
        /// </summary>
        /// <param name="companyABN">The company abn.</param>
        /// <returns>company name</returns>
        string GetComapnyNameByABN(string companyABN);

        /// <summary>
        /// Sscids the exist inscassc mapping.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>object result</returns>
        object SSCIDExistINSCASSCMapping(int p1, int p2);

        /// <summary>
        /// Gets the isst cfor sca.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>ssc to sca</returns>
        int GetISSTCforSCA(int userID);

        /// <summary>
        /// Checks the exist accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>accreditation number</returns>
        int CheckExistAccreditationNumber(string accreditationNumber, int solarCompanyId, int UserTypeID);

        bool SolarElectricianRequestIsExists(int solarCompanyId, int userId);

        bool CheckAutoRequestStatusOverUserId(int userId, bool IsApproved, bool IsGet);
        /// <summary>
        /// Scases the insert.
        /// </summary>
        /// <param name="userView">The user view.</param>
        void SCASEInsert(User userView);

        /// <summary>
        /// Inserts the sc aas se.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns>sca to se</returns>
        int InsertSCAasSE(User userView);

        /// <summary>
        /// Inserts the status.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        void InsertStatus(int UserId);

        /// <summary>
        /// Gets the solar electrician by identifier.
        /// </summary>
        /// <param name="solarElectricianId">The solar electrician identifier.</param>
        /// <returns>electrician identifier</returns>
        DataSet GetSolarElectricianById(int solarElectricianId);

        /// <summary>
        /// Gets the fco by reseller identifier.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>reseller identifier</returns>
        List<User> GetFCOByResellerId(int ResellerId);

        /// <summary>
        /// Inserts the selected reseller fco group.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="strFCOGroupSelected">The string fco group selected.</param>
        void InsertSelectedResellerFCOGroup(int userID, string strFCOGroupSelected);

        /// <summary>
        /// Deletes the selected reseller group.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        void DeleteSelectedResellerGroup(int UserId);

        /// <summary>
        /// Deletes the logo.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        void DeleteLogo(int userID);

        /// <summary>
        /// Deletes the signature.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        void DeleteSignature(int userID);

        /// <summary>
        /// Deletes the selfie.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        void DeleteSelfie(int userID);

        /// <summary>
        /// Gets the reseller user record credentials by reseller identifier.
        /// </summary>
        /// <param name="ResellerID">The reseller identifier.</param>
        /// <returns>user object</returns>
        RECAccount GetResellerUserRECCredentialsByResellerID(int ResellerID);

        /// <summary>
        /// Gets the reseller user record credentials by reseller identifier.
        /// </summary>
        /// <param name="ResellerID">The reseller identifier.</param>
        /// <returns>
        /// user object
        /// </returns>
        List<RECAccount> GetAllResellerUserRECCredentialsByResellerID(int ResellerID);

        /// <summary>
        /// Updates the device token.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="type">The type.</param>
        /// <returns>string type</returns>
        string UpdateDeviceToken(int userID, string accessToken, string type, string DeviceInfo = "");
		/// <summary>
		/// Updates the device token.
		/// </summary>
		/// <param name="userID">The user identifier.</param>
		/// <param name="accessToken">The access token.</param>
		/// <param name="type">The type.</param>
		/// <returns>
		/// string type
		/// </returns>
		string UpdateDeviceAllowAccessTimmer(int userID, string accessToken, string DeviceInfo = "");

		/// <summary>
		/// Gets the reseller identifier by user identifier.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <returns>user identifier</returns>
		int GetResellerIdByUserId(int userId);

        /// <summary>
        /// Determines whether [is valid token] [the specified user identifier].
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="apiToken">The API token.</param>
        /// <returns>boolean result</returns>
        bool IsValidToken(int userID, string apiToken);

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="ElectricianID">The electrician identifier.</param>
        /// <param name="SolarCompanyID">The solar company identifier.</param>
        void DeleteRequestToSE(int ElectricianID, int SolarCompanyID);

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        List<User> GetSolarSubContractor();

        /// <summary>
        /// Rec_Email Mapping InsertUpdate
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loggedInUserId"></param>
        void Rec_EmailMappingInsertUpdate(string email, string mail_inc_host, string mail_inc_login, string mail_inc_pass, int? mail_inc_port, string mail_out_host, int? mail_out_port, string signature,
            string signature_opt, long mailbox_size, int loggedInUserId, int def_timezone);

        /// <summary>
        /// Rec UserEmailDetails
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        DataSet RecUserEmailDetails(int userID);

        /// <summary>
        /// Gets the raram email address.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        string GetRARAMEmailAddress(int SolarCompanyId);

        /// <summary>
        /// Gets the fsafco email addresses.
        /// </summary>
        /// <returns></returns>
        string GetFSAFCOEmailAddresses();

        /// <summary>
        /// Gets the fsa email addresses.
        /// </summary>
        /// <returns></returns>
        string GetFSAEmailAddresses();

        /// <summary>
        /// Gets the ra email address.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns></returns>
        string GetRAEmailAddress(int ResellerId);

        List<InstallerDesignerView> GetInstallerDesignerAddByProfile(int UserId, int userType, int pageSize, int pageNumber, string sortCol, string sortDir, int solarCompanyId, string name, string accreditationnumber, int SERole, int SendBy, string LicenseNumber, bool IsSWHUser);

        int AddEditInstallerDesigner(InstallerDesignerView installerDesignerView, int jobId = 0, int profileType = 0, string signPath = null, int accreditedInstallerId = 0, int userId = 0);

        InstallerDesignerView GetInstallerDesigner(int InstallerDesignerId, int? JobId);

        /// <summary>
        /// Checks the exist accreditation number.
        /// </summary>
        /// <param name="accreditationNumber">The accreditation number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>accreditation number</returns>
        int CheckExistAccreditationNumberForInstallerDesigner(string accreditationNumber, int solarCompanyId);

        /// <summary>
        /// Deletes the request to se.
        /// </summary>
        /// <param name="ElectricianID">The electrician identifier.</param>
        /// <param name="SolarCompanyID">The solar company identifier.</param>
        void DeleteRequestToSEForInstallerDesigner(int ElectricianID, int SolarCompanyID);

        /// <summary>
        /// Gets the se user With Status.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>solar view</returns>
        List<InstallerDesignerView> GetInstallerDesignerWithStatus(bool isInstaller, int companyId, int jobId, bool IsSubContractor);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>data set</returns>
        DataSet GetInstallerDesignerById(int InstallerDesignerId);

        /// <summary>
        /// Gets the user of user type.
        /// </summary>
        /// <returns>user list</returns>
        List<User> GetUserByUserType(int userTypeId);

        /// <summary>
        /// Get Users By FSA
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        User GetUsersByFSA(int UserID);

        /// <summary>
        /// Gets the user of reseller.
        /// </summary>
        /// <returns>user list</returns>
        List<User> GetUserByResellerId(int userTypeId, int resellerId);

        /// <summary>
        /// Gets the user of solar company.
        /// </summary>
        /// <returns>user list</returns>
        List<User> GetUserBySolarCompanyId(int userTypeId, int solarCompanyId);

        /// <summary>
        /// Check Client Code Prefix
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <param name="clientCodePrefix">clientCodePrefix</param>
        /// <returns>Dataset</returns>
        DataSet CheckClientCodePrefix(int userTypeId, int resellerId, int userId, string clientCodePrefix);

        /// <summary>
        /// Get Client Number
        /// </summary>
        /// <param name="userTypeId">userTypeId</param>
        /// <param name="resellerId">resellerId</param>
        /// <param name="userId">userId</param>
        /// <returns>DataSet</returns>
        DataSet GetClientNumber(int userTypeId, int resellerId, int userId, string existClientNumber, int solarCompanyId);

        /// <summary>
        /// Update SolarCompany Users CheckInXero
        /// </summary>
        /// <param name="userView">user table value</param>
        /// <returns>Returns the UserID</returns>
        int UpdateSolarCompanyUsersCheckInXero(User userView);

        /// <summary>
        /// GetAllSolarCompanyDetails
        /// </summary>
        /// <param name="solarCompanyIds"></param>
        /// <returns>DataSet</returns>
        DataSet GetAllSolarCompanyDetails(string solarCompanyIds);

        /// <summary>
        /// GetApiVersion
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetApiVersion();

        /// <summary>
        /// GetDetailsForTermsAndCondition
        /// </summary>
        /// <param name="UserTypeID"></param>
        /// <param name="SolarCompanyId"></param>
        /// <returns></returns>
        DataSet GetDetailsForTermsAndCondition(int UserTypeID, int SolarCompanyId);

        /// <summary>
        /// Gets the user by aspnet user id_ vendor API.
        /// </summary>
        /// <param name="aspnetUserId">The aspnet user identifier.</param>
        /// <returns></returns>
        UserVendor GetUserByAspnetUserId_VendorAPI(string aspnetUserId);

        /// <summary>
        /// Gets the name of the user by user.
        /// </summary>
        /// <param name="Username">The username.</param>
        /// <returns></returns>
        User GetUserByUserName(string Username);

        List<AccreditedInstallers> CheckExistAccreditationNumber_VendorApi(string accreditationNumber, string FirstName, string LastName);

        /// <summary>
        /// Checks the exist License number.
        /// </summary>
        /// <param name="LicenseNumber">The License number.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>accreditation number</returns>
        int CheckExistLicenseNumberForSWHInstaller(string LicenseNumber, int solarCompanyId, string email);

        /// <summary>
        /// Gets all user for restore list.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="accreditationnumber">The accreditationnumber.</param>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <param name="companyabn">The companyabn.</param>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <returns></returns>
        List<User> GetAllUserForRestoreList(int userType, int pageSize, int pageNumber, string sortCol, string sortDir, string name, string username, string email, string accreditationnumber, string licensenumber, string companyabn, string companyName, string resellerId, string companyId);

        /// <summary>
        /// Restores the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        int RestoreUser(string userId);

        /// <summary>
        /// Updates the job installer designer identifier.
        /// </summary>
        /// <param name="installerDesignerId">The installer designer identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="profileType">Type of the profile.</param>
        /// <param name="IsSWHUser">if set to <c>true</c> [is SWH user].</param>
        /// <returns></returns>
        InstallerDesignerView UpdateJob_InstallerDesignerId(int installerDesignerId, int jobId, int profileType, bool IsSWHUser, int userId = 0);

        /// <summary>
        /// Gets the electrician list.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        List<JobElectricians> GetElectricianList(int companyId, int jobId);

        /// <summary>
        /// Deletes the custom electrician.
        /// </summary>
        /// <param name="jobElectricianId">The job electrician identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        void DeleteCustomElectrician(int jobElectricianId, int jobId);

        IList<InstallerDesignerView> SWHInstallerList_ByLicenseNumber(string licenseNumber);

        List<VEECInstaller> CheckVEECInstallerExists(string LicenseNumber);

        DataSet UpdateJob_JobElectricianId(int jobId, int solarCompanyId, int jobElectricianId, bool isCustomElectrician, int CreatedBy = 0);
        void UpdateUsers_SaveIsNewViewerUserWise(bool IsNewViewer, int UserId);
        void User_UpdateApiTokenFromResetPassword(string Id);
		void UploadProfilePhoto(int solarCompanyId, string profilePhoto);

        /// <summary>
        /// Get GridConfiguartion of all pages based on userid
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<UserWiseGridConfiguration> GetUserWiseGridConfiguration(int UserId);

        /// <summary>
        /// Insert Grid Configuration user wise
        /// </summary>
        /// <param name="userWiseGridConfiguration"></param>
        int InsertUpdateUserWiseGridConfiguration(UserWiseGridConfiguration userWiseGridConfiguration);

        /// <summary>
        /// Update tabular view or default user wise
        /// </summary>
        /// <param name="isTabularView"></param>
        void UpdateTabularViewConfiguration(bool isTabularView);


        List<UserDevice> GetUserDeviceInfo(int userId);

        void DeviceLogout(int id, bool isLogoutAll);


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
        DataSet ExportSCA(string name = "", string username = "", string state = "", string companyname = "", string email = "", string companyabn = "", int status = 0,int ResellerId=0,int RamId=0, string mobile = "");

        /// <summary>
        /// Get rec Username and password from userid
        /// </summary>
        /// <param name="UserId"></param>
        DataTable GetRecDataFromUserId(int UserId);
        /// <summary>
        /// Get all android mobile app users details. 
        /// </summary>
        /// <returns></returns>
        DataTable GetAllAndroidMobileAppUsers();

        List<User> GetAllApprovedElectricianList();
        /// <summary>
        ///insert logs for update contact details in xero
        /// </summary>
        /// <param name="historyMessages"></param>
        /// <param name="UserId"></param>
        void InsertLogForUpdateContact(string historyMessages,int UserId);
        /// <summary>
        /// Get logs details about update contact details in xero. 
        /// </summary>
        /// <returns></returns>
        List<UpdateContactXeroLog> GetLogsForUpdateContact(int userId);
		void InsertUpdateResellerRECLoginDetails(int UserId, string ResellerName, string LoginType, string CERLoginId, string CERPassword, string RECCompName, string UserName, string RECName, int UpdatedBy, bool IsDefault);


        /// <summary>
        /// Save masking value
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="isAllowedMAsking"></param>
        void SaveMaskingValue(int resellerId, bool isAllowedMAsking);

        /// <summary>
        /// Get Access Role value for Auto REC Upload
        /// </summary>
        /// <param name="resellerId"></param>
        bool GetAutoRECUploadAccess(int resellerId);
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
        void InsertStatusWithoutNote(int UserId, byte? Status, int ComplainBy, bool IsSCDetailConfirm, bool IsInstaller, bool IsSEDetailConfirm, int? IsGSTSetByAdminUser = null);
        /// <summary>
        /// get user by userid
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        DataSet GetUserByUserId(int UserId);
 		DataTable GetAcccountDetailByEmail(string emailID);
        DataSet GetAllUsersWithABN();
        void UpdateEntityName(int UserId, string EntityName);

        DataSet ChangeSCA(int jobId, string GBSCACode, int resellerID);

        /// <summary>
        /// get userid from cec accerdiationNumber
        /// </summary>
        /// <param name="CECAccreditationNumber"></param>
        /// <returns></returns>
        string GetUserIdFromCECNumber(string CECAccreditationNumber,string ElectricalContractorsLicenseNumber, bool isPvdUser, bool isSWHUser);
 /// <summary>
        /// Save SAAS user notes
        /// </summary>
        /// <param name="UserID">UserID</param>
        /// <param name="Notes">Notes</param>
        /// <param name="CreatedBy">Created By</param>
        /// <param name="CreatedDate">Created Date</param>
        void SaveSAASUserNote(int UserID, string Notes, int CreatedBy, DateTime CreatedDate);

        /// <summary>
        /// Get SAAS Userdetails
        /// </summary>
        /// <param name="SAASUserID"></param>
        /// <returns></returns>
        DataSet GetSAASUserDetailsbyInvoicerID(int SAASUserID);
        /// <summary>
        /// Delete ContractFile from db
        /// </summary>
        /// <param name="InvoicerID"></param>
        /// <param name="DocumentPath"></param>
        void DeleteContractFile(int InvoicerID, string DocumentPath);

        /// <summary>
        /// Get SAAS users Count
        /// </summary>
        /// <returns></returns>
        int GetSAASUserCount();
        InstallerAuditDetails GetInstallerAuditDetails(int deviceID);
DataSet GetCECAccreditationNumberForInstallerDesignerElectrician(int jobId);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>data set</returns>
        DataSet GetinvoicerDetailsForSaasUsers(int? userID);

        List<User> GetInvoicerDetailsList();
        void AddNotesNotificaion(int receiverUserId, int jobId, string jobDetailLink, string notes, int loggedInUserId);
        int GetNotificationCount(int loggedInUserId);
        List<NotificationViewModel> GetNotificationList(int loggedInUserId);
        void RemoveNotification(string notificationIds, int loggedInUserId);
    }
}

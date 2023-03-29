using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System.Web.Mvc;
using System;
using FormBot.Helper;

namespace FormBot.BAL.Service.RAMSolarCompanyMapping
{
    public class RAMSolarCompanyMappingBAL : IRAMSolarCompanyMappingBAL
    {
        /// <summary>
        /// Get GetResellerAccountManager
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Get Single Reseller</returns>
        public List<User> GetResellerAccountManager(int userId)
        {
            string spName = "[GetResellerAccountManager]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            IList<User> getResellerAccountManagerList = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return getResellerAccountManagerList.ToList();
        }

        /// <summary>
        /// RAMSolarCompany List
        /// </summary>
        /// <param name="userID">The UserId</param>
        /// <returns>Get List of Solar Company</returns>
        public IEnumerable<SelectListItem> RAMSolarCompanyList(int userID)
        {
            string spName = "[GetResellerAccountManager_SolarComapny]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            IList<RAMSolarCompany> fCOGroup = CommonDAL.ExecuteProcedure<RAMSolarCompany>(spName, sqlParameters.ToArray());
            return fCOGroup.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.SolarCompanyId),
                Text = d.CompanyName
            }).ToList();

        }

        /// <summary>
        /// insert record into  RAMSolarCompanyMapping
        /// </summary>
        /// <param name="rAMID">The r amid.</param>
        /// <param name="solarCompanyIDs">The solar company i ds.</param>
        /// <returns>Returns List of Solar Company Assignment</returns>
        public object CreateRAMSolarCompanyMapping(int rAMID, string solarCompanyIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, rAMID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.NVarChar, solarCompanyIDs));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, 0));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("RAMSolarCompanyMapping_Insert", sqlParameters.ToArray());
            return null;
        }

        /// <summary>
        /// Gets the assign ram solar company list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="ramID">The ram identifier.</param>
        /// <returns>
        /// select list
        /// </returns>
        public List<SelectListItem> GetAssignRAMSolarCompanyList(int userID,int ramID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, ramID));
            IList<RAMSolarCompany> fCOGroup = CommonDAL.ExecuteProcedure<RAMSolarCompany>("GetResellerAccountManager_AssignSolarCompany", sqlParameters.ToArray());
            return fCOGroup.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.SolarCompanyId),
                Text = d.CompanyName
            }).ToList();
        }

        /// <summary>
        /// Gets all ram solar company list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetAllRAMSolarCompanyList(int userID)
        {
            string spName = "[GetResellerAccountManager_SolarComapnyList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            IList<RAMSolarCompany> fCOGroup = CommonDAL.ExecuteProcedure<RAMSolarCompany>(spName, sqlParameters.ToArray());
            return fCOGroup.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.SolarCompanyId),
                Text = d.CompanyName
            }).ToList();
        }

        /// <summary>
        /// Gets the ram by user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// user identifier
        /// </returns>
        public int GetRAMByUserId(int userID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            object ramID = CommonDAL.ExecuteScalar("RAMSolarCompanyMapping_GetRAMByUserId", sqlParameters.ToArray());
            return Convert.ToInt32(ramID);
        }

        /// <summary>
        /// Gets the assigned solar company to ram.
        /// </summary>
        /// <param name="id">The RAM identifier.</param>
        /// <returns>List Of SolarCompanies assigned to this RAM</returns>
        public List<SolarCompany> GetAssignedSolarCompanyToRAM(int id)
        {
            string spName = "[RAMSolarCompanyMapping_GetAssignedSolarCompanyToRAM]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// get GetRAMByReseller
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Get RAM</returns>
        public List<User> GetRAMByReseller(int resellerId)
        {
            string spName = "[GetRAMByReseller]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            IList<User> getRAMByReseller = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return getRAMByReseller.ToList();
        }
    }
}

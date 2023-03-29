using FormBot.DAL;
using FormBot.Entity.Job;
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
    public class JobPartsBAL : IJobPartsBAL
    {
        /// <summary>
        /// Inserts the job parts using synchronize xero.
        /// </summary>
        /// <param name="JobPartsJson">The job parts json.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="totalJobPartsIds">The total job parts ids.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet InsertJobPartsUsingSyncXero(string JobPartsJson, int createdBy, DateTime createdDate, int? solarCompanyId, int userTypeId, string totalJobPartsIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobPartsJson", SqlDbType.NVarChar, JobPartsJson));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("TotalJobPartsIDs", SqlDbType.NVarChar, totalJobPartsIds));
            DataSet jobParts = CommonDAL.ExecuteDataSet("JobParts_InsertJobPartsUsingSyncXero", sqlParameters.ToArray());
            return jobParts;
        }

        /// <summary>
        /// Gets the job parts list.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="itemCodeOrDescription">The item code or description.</param>
        /// <param name="isXeroParts">if set to <c>true</c> [is xero parts].</param>
        /// <returns>
        /// job parts
        /// </returns>
        public List<JobParts> GetJobPartsList(int userID, int userTypeId, int pageNumber, int pageSize, string sortCol, string sortDir, int solarCompanyId, string itemCodeOrDescription, bool isXeroParts)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("ItemCodeOrDescription", SqlDbType.NVarChar, itemCodeOrDescription));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsXeroParts", SqlDbType.Bit, isXeroParts));
            List<JobParts> lstJobParts = CommonDAL.ExecuteProcedure<JobParts>("JobParts_GetJobParts", sqlParameters.ToArray()).ToList();
            return lstJobParts;
        }

        /// <summary>
        /// Gets the job parts by identifier.
        /// </summary>
        /// <param name="jobPartId">The job part identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobPartsById(int jobPartId)
        {
            int? solarCompanyId = 0;
            int? resellerId = 0;

            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                solarCompanyId = null;
            else
                solarCompanyId = ProjectSession.SolarCompanyId;

            if (ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                resellerId = ProjectSession.ResellerId;
            else
                resellerId = 0;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobPartId", SqlDbType.BigInt, jobPartId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.BigInt, resellerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, ProjectSession.LoggedInUserId));

            DataSet jobParts = CommonDAL.ExecuteDataSet("JobParts_GetJobPartsById", sqlParameters.ToArray());
            return jobParts;
        }
    }
}

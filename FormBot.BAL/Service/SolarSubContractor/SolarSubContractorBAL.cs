using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System;

namespace FormBot.BAL.Service
{
    public class SolarSubContractorBAL : ISolarSubContractorBAL
    {
        /// <summary>
        /// Gets the solar sub contractor request list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCol">The sort col.</param>
        /// <param name="sortDir">The sort dir.</param>
        /// <param name="refNumber">The reference number.</param>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns>solar list</returns>
        public List<SolarSubContractor> GetSolarSubContractorRequestList(int pageNumber, int pageSize, string sortCol, string sortDir, string refNumber, string companyName, DateTime? fromDate, DateTime? toDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, refNumber));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, companyName));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, fromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, toDate));
            List<SolarSubContractor> lstSolarSubContractorRequest = CommonDAL.ExecuteProcedure<SolarSubContractor>("SSC_RemoveRequestList", sqlParameters.ToArray()).ToList();
            return lstSolarSubContractorRequest;
        }

        /// <summary>
        /// Deletes the remove SSC request.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        public void SuccessfullRemoveSSCRequest(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            CommonDAL.Crud("JobDetails_SuccessfullRemoveSSCRequest", sqlParameters.ToArray());
        }
    }
}

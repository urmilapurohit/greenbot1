using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IJobPartsBAL
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
        /// <returns>DataSet</returns>
        DataSet InsertJobPartsUsingSyncXero(string JobPartsJson, int createdBy, DateTime createdDate, int? solarCompanyId, int userTypeId, string totalJobPartsIds);

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
        /// <returns>job parts</returns>
        List<JobParts> GetJobPartsList(int userID, int userTypeId, int pageNumber, int pageSize, string sortCol, string sortDir, int solarCompanyId, string itemCodeOrDescription, bool isXeroParts);

        /// <summary>
        /// Gets the job parts by identifier.
        /// </summary>
        /// <param name="jobPartId">The job part identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobPartsById(int jobPartId);
    }
}

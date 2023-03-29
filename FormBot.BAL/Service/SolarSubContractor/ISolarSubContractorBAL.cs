using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface ISolarSubContractorBAL
    {
        /// <summary>
        /// Gets the solar sub contractor request list.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="todate">The todate.</param>
        /// <returns>solar list</returns>
        List<SolarSubContractor> GetSolarSubContractorRequestList(int pageNumber, int PageSize, string SortCol, string SortDir, string templateName, string subject,DateTime? fromdate, DateTime? todate);

        /// <summary>
        /// Successfulls the remove SSC request.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        void SuccessfullRemoveSSCRequest(int jobID);
    }
}

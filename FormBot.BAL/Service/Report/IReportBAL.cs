using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Web.Mvc;
using System.Data;

namespace FormBot.BAL.Service
{
    public interface IReportBAL
    {

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="reportID">The report identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetReport(int? reportID);

        /// <summary>
        /// Gets the owner account.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetOwnerAccount();

        /// <summary>
        /// Gets the type of the action.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetActionType();

        /// <summary>
        /// Gets the type of the status.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetStatusType();

        /// <summary>
        /// Gets the fuel source.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetFuelSource();

        /// <summary>
        /// Gets the report DRP.
        /// </summary>
        /// <returns>report list</returns>
        List<Report> GetReportDrp();

        /// <summary>
        /// Gets the report list.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        /// <returns>report list</returns>
        List<Report> GetReportList(Report reportModel);

        /// <summary>
        /// Gets the excel data.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetExcelData(Report reportModel);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Web.Mvc;
using System.Data;
using FormBot.Entity.Dashboard;

namespace FormBot.BAL.Service
{
    public interface IFormBotReportBAL
    {

        /// <summary>
        /// Gets the form bot report DRP.
        /// </summary>
        /// <returns>report list</returns>
        List<FormBotReport> GetFormBotReportDrp();

        /// <summary>
        /// Gets the solar company.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetSolarCompany(string isAllScaJobView = "" ,string hdnResellers = "");

        /// <summary>
        /// Gets the preapproval status.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetPreapprovalStatus();

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetConnectionStatus();

        /// <summary>
        /// Gets the STC submission status.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetSTCSubmissionStatus();

        /// <summary>
        /// Gets the job status bar total reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>report list</returns>
        List<FormBotReport> GetJobStatusBarTotalReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the total active users report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>report list</returns>
        List<FormBotReport> GetTotalActiveUsersReport(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the job stages report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>report list</returns>
        DataSet GetJobStagesReport(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the job status detail reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>report list</returns>
        DataSet GetJobStatusDetailReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the form bot user.
        /// </summary>
        /// <param name="UserType">Type of the user.</param>
        /// <returns>report list</returns>
        IEnumerable<SelectListItem> GetFormBotUser(int UserType, string hdnSolarCompanies = "", int? UserId = 0);

        /// <summary>
        /// Gets the system user reports reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>report list</returns>
        DataSet GetSystemUserReportsReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the total job list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>slected list item</returns>
        List<FormBotReport> GetTotalJobList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the allocation report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetAllocationReportList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the job invoice report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>dataset</returns>
        DataSet GetJobInvoiceReport(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the form bot ram user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetFormBotRAMUser(FormBotReport model);

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>formbot report</returns>
        DataSet GetRECFailureReasonsReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>formbot report</returns>
        List<FormBotReport> GetRECFailureReasonsDashboardReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the se user level reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>formbot report</returns>
        List<FormBotReport> GetSEUserLevelReportsList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the job stages.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetJobStages();

        /// <summary>
        /// Gets the sales agent.
        /// </summary>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetSalesAgent(string hdnSolarCompanies = "");

        /// <summary>
        /// Gets the sold by who report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetSoldByWhoReportList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the sscse jobs detail report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetSSCSEJobsDetailReportList(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the STC general report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetSTCGeneralReport(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the STC general report for ram.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetSTCGeneralReportForRAM(FormBotReport formbotReportModel);

        /// <summary>
        /// Gets the STC general report for dashboard.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>DataSet</returns>
        DataSet GetSTCGeneralDashboardReport(FormBotReport formbotReportModel);

        /// <summary>
        ///  Get Trade data for SCA dashboard trade chart
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        List<SCATradeReport> GetSCATradedChartData(string StartDate, string EndDate, int Type);

        /// <summary>
        /// Get SCA Status Chart Data
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        List<SCAStatusReport> GetSCAStatusChartData(int Type);
        DataSet GetNonTradeJobReport(FormBotReport formbotReportModel);
    }
}

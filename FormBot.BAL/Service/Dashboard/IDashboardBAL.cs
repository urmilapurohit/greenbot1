using FormBot.Entity.Dashboard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.BAL.Service.Dashboard
{
    public interface IDashboardBAL
    {
        /// <summary>
        /// Get News Feed List
        /// </summary>
        /// <param name="categoryID">category ID</param>
        /// <param name="fromDate">from Date</param>
        /// <param name="toDate">to Date</param>
        /// <returns>Data Set</returns>
        DataSet GetNewsFeedList(int? categoryID, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize);

        /// <summary>
        /// Get Dashboard Job STC Price
        /// </summary>
        /// <returns>Data Set</returns>
        FormBot.Entity.PricingManager GetDashboardJobSTCPrice();

        /// <summary>
        /// Get SCA Dashboard Details
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetSCADashboardDetails();

        /// <summary>
        /// Get SSC Dashboard Details
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetSSCDashboardDetails();

        /// <summary>
        /// Get SESC Dashboard Details
        /// </summary>
        /// <param name="UserId">User Id</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="sortCol">sort Col</param>
        /// <param name="sortDir">sort Dir</param>
        /// <returns>Data Set</returns>
        DataSet GetSESCDashboardDetails(int UserId, int pageSize, int pageNumber, string sortCol, string sortDir);

        /// <summary>
        /// Get New Job Request List for SSC
        /// </summary>
        /// <param name="UserId">User Id</param>
        /// <param name="sortCol">sort Col</param>
        /// <param name="sortDir">sort Dir</param>
        /// <returns>Data Set</returns>
        //DataSet GetNewJobRequestList(int UserId, string sortCol, string sortDir);
		DataSet GetNewJobRequestList(int UserId, string sortCol, string sortDir, int PageIndex = 1, int PageSize = 10);
		/// <summary>
		/// Get SE/SC dashboard details
		/// </summary>
		/// <returns>Data Set</returns>
		DataSet GetSESCDashboard();

        /// <summary>
        /// Gets the fsa dashboard details.
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetFSADashboardDetails();

        /// <summary>
        /// Gets the ra dashboard details.
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetRADashboardDetails();

        /// <summary>
        /// Get FSA/FCO STC job status.
        /// </summary>
        /// <param name="resellerIds">reseller Ids</param>
        /// <param name="isAllReseller">is All Reseller</param>
        /// <returns>list</returns>
        List<DashboardSTCJobStaus> GetFSAFCO_STCJobStaus(string resellerIds, bool isAllReseller);

        /// <summary>
        /// Get FCO by reseller.
        /// </summary>
        /// <returns>SelectListItem</returns>
        IEnumerable<SelectListItem> GetFCOByReseller();

        /// <summary>
        /// Get reseller by FCO.
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns>List</returns>
        List<FormBot.Entity.Reseller> GetResellerByFCO(int userId);

        /// <summary>
        /// Gets the compliance team.
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetComplianceTeam();

        /// <summary>
        /// Get STC job parts for RA/RAM  users.
        /// </summary>
        /// <param name="solarCompanyIds">solar Company Ids</param>
        /// <param name="sortCol">sort Col</param>
        /// <returns>Dashboard STC Job Staus</returns>
        List<DashboardSTCJobStaus> GetRARAM_STCJobStaus(string solarCompanyIds, string sortCol);

        /// <summary>
        /// Gets the database link last update.
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetDatabaseLinkUpdate();

        /// <summary>
        /// Get solar company by RAM
        /// </summary>
        /// <param name="ramIds">ram Ids</param>
        /// <returns>list</returns>
        List<FormBot.Entity.SolarCompany> GetSolarCompanyByRAM(string ramIds,string isAllScaJobView);

        /// <summary>
        /// Get all RAM by RA
        /// </summary>
        /// <param name="resellerId">reseller Id</param>
        /// <returns>SelectList Item</returns>
        IEnumerable<SelectListItem> GetAllRAMByRA(int resellerId);

        /// <summary>
        /// Get payment for RA/RAM
        /// </summary>
        /// <param name="resellerId">resellerId</param>
        /// <param name="ramId">ramId</param>
        /// <returns>dataset</returns>
        DataSet GetPaymentForRARAM(int resellerId, int ramId);

        /// <summary>
        /// Gets the requesting sc aand SSC for s eand sc.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <returns>List</returns>
        List<FormBot.Entity.User> GetRequestingSCAandSSCForSEandSC(int UserId, int UserTypeId, string SortCol, string SortDir);

        FormBot.Entity.PricingManager GetDashboardJobCERApproved();

        DataSet GetNotifications(DateTime dt);
        DataSet GetNotificationsById(string SnackbarId, DateTime date);
        /// <summary>
        /// Get Dashboard Job STC Price for RA
        /// </summary>
        /// <returns>Entity</returns>
        Entity.PricingManager GetDashboardJobSTCPriceForRA();

    }
}

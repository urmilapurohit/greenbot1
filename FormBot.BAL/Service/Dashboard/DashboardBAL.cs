using FormBot.DAL;
using FormBot.Entity.Dashboard;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.BAL.Service.Dashboard
{
    public class DashboardBAL : IDashboardBAL
    {
        /// <summary>
        /// Get News Feed List
        /// </summary>
        /// <param name="categoryID">category ID</param>
        /// <param name="fromDate">from Date</param>
        /// <param name="toDate">to Date</param>
        /// <returns>Data Set</returns>
        public DataSet GetNewsFeedList(int? categoryID, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize)
        {
            string spName = "[GetDashboardNewsFeed]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("CategoryID", SqlDbType.Int, categoryID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("IsSubContractor", SqlDbType.Bit, ProjectSession.IsSubContractor));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, fromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, toDate));

            sqlParameters.Add(DBClient.AddParameters("PageIndex", SqlDbType.Int, pageIndex));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));

            DataSet dsNewsFeed = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsNewsFeed;
        }

        /// <summary>
        /// Get Dashboard Job STC Price
        /// </summary>
        /// <returns>Entity</returns>
        public FormBot.Entity.PricingManager GetDashboardJobSTCPrice()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDate", SqlDbType.DateTime, DateTime.Now));
            var result = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("GetDashboardJobSTCPrice", sqlParameters.ToArray()).FirstOrDefault();

            if (result == null)
            {
                FormBot.Entity.PricingManager pricing = new Entity.PricingManager();
                pricing.Hour24 = 0;
                pricing.Days3 = 0;
                pricing.Days7 = 0;
                pricing.CERApproved = 0;
                pricing.STCAmount = 0;
                return pricing;
            }

            return result;
        }
        /// <summary>
        /// Get Dashboard Job STC Price for RA
        /// </summary>
        /// <returns>Entity</returns>
        public Entity.PricingManager GetDashboardJobSTCPriceForRA()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            //sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ProjectSession.ResellerId));
            var result = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("GetDashboardJobSTCPriceForRA", sqlParameters.ToArray()).FirstOrDefault();

            if (result == null)
            {
                FormBot.Entity.PricingManager pricing = new Entity.PricingManager();
                pricing.Hour24 = 0;
                pricing.Days3 = 0;
                pricing.Days7 = 0;
                pricing.CERApproved = 0;
                pricing.STCAmount = 0;
                return pricing;
            }

            return result;
        }
        /// <summary>
        /// Get Account Manager Details
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetSCADashboardDetails()
        {
            string spName = "[GetSCADashboardDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get SSC Dashboard Details
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetSSCDashboardDetails()
        {
            string spName = "[GetSSCDashboardDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("IsSubContractor", SqlDbType.Bit, ProjectSession.IsSubContractor));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, ProjectSession.SolarCompanyId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get SESC Dashboard Details
        /// </summary>
        /// <param name="UserId">User Id</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="pageNumber">page Number</param>
        /// <param name="sortCol">sort Col</param>
        /// <param name="sortDir">sort Dir</param>
        /// <returns>Data Set</returns>
        public DataSet GetSESCDashboardDetails(int UserId, int pageSize, int pageNumber, string sortCol, string sortDir)
        {
            string spName = "[SESCDashboard]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get New Job Request List for SSC
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <param name="sortCol">sortCol</param>
        /// <param name="sortDir">sortDir</param>
        /// <returns>Data Set</returns>
        public DataSet GetNewJobRequestList(int UserId, string sortCol, string sortDir,int PageIndex = 1, int PageSize = 10)
        {
            string spName = "[GetNewJobRequestList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
			sqlParameters.Add(DBClient.AddParameters("PageIndex", SqlDbType.Int, PageIndex));
			sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
			DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get SE/SC dashboard details
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetSESCDashboard()
        {
            string spName = "[GetSESCDashboardDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the FSA dashboard details.
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetFSADashboardDetails()
        {
            string spName = "[GetFSADashboardDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the RA dashboard details.
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetRADashboardDetails()
        {
            string spName = "[GetRADashboardDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, ProjectSession.ResellerId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get FSA/FCO STC job status.
        /// </summary>
        /// <param name="resellerIds">reseller Ids</param>
        /// <param name="isAllReseller">is All Reseller</param>
        /// <returns>List</returns>
        public List<DashboardSTCJobStaus> GetFSAFCO_STCJobStaus(string resellerIds, bool isAllReseller)
        {
            string spName = "[FSAFCO_STCJobStaus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.NVarChar, resellerIds));
            //sqlParameters.Add(DBClient.AddParameters("ComplianceOfficerIds", SqlDbType.NVarChar, complianceOfficeIds));
            sqlParameters.Add(DBClient.AddParameters("IsAllReseller", SqlDbType.Bit, isAllReseller));
            //sqlParameters.Add(DBClient.AddParameters("IsAllComplianceOfficer", SqlDbType.Bit, isAllComplianceOfficer));

            List<DashboardSTCJobStaus> lstSTCJobStatus = CommonDAL.ExecuteProcedure<DashboardSTCJobStaus>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStatus;
        }

        /// <summary>
        /// Gets the compliance team.
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetComplianceTeam()
        {
            string spName = "[ComplianceTeamForDashboard]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ProjectSession.ResellerId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get FCO By Reseller
        /// </summary>
        /// <returns>SelectListItem</returns>
        public IEnumerable<SelectListItem> GetFCOByReseller()
        {
            string spName = "[GetFCOByReseller]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.NVarChar, resellerIds));
            //IList<Dashboard> lstFCOUsers = CommonDAL.ExecuteProcedure<FormBot.Entity.User>(spName, sqlParameters.ToArray()).ToList();
            return CommonDAL.ExecuteProcedure<FormBot.Entity.User>(spName, sqlParameters.ToArray()).ToList().Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.UserId),
                Text = Convert.ToString(d.Name)
            }).ToList();
        }

        /// <summary>
        /// Get reseller by FCO.
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns>List</returns>
        public List<FormBot.Entity.Reseller> GetResellerByFCO(int userId)
        {
            string spName = "[GetResellerByFCO]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.NVarChar, userId));
            List<FormBot.Entity.Reseller> lstReseller = CommonDAL.ExecuteProcedure<FormBot.Entity.Reseller>(spName, sqlParameters.ToArray()).ToList();
            return lstReseller;
        }

        /// <summary>
        /// Get STC job parts for RA/RAM  users.
        /// </summary>
        /// <param name="solarCompanyIds">solarCompanyIds</param>
        /// <param name="sortCol">sortCol</param>
        /// <returns>List</returns>
        public List<DashboardSTCJobStaus> GetRARAM_STCJobStaus(string solarCompanyIds, string sortCol)
        {
            string spName = "[RARAM_STCJobStaus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, solarCompanyIds));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));

            List<DashboardSTCJobStaus> lstSTCJobStatus = CommonDAL.ExecuteProcedure<DashboardSTCJobStaus>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStatus;
        }

        /// <summary>
        /// Gets the database link last update.
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetDatabaseLinkUpdate()
        {
            string spName = "[GetDatabaseLinkUpdates]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get solar company by RAM
        /// </summary>
        /// <param name="ramIds">ram Ids</param>
        /// <returns> Get Solar Company By RAM</returns>
        public List<FormBot.Entity.SolarCompany> GetSolarCompanyByRAM(string ramIds, string isAllScaJobView)
        {
            string spName = "[GetSolarCompanyByRAM]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RAMIds", SqlDbType.NVarChar, ramIds));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, !string.IsNullOrEmpty(isAllScaJobView) ? Convert.ToBoolean(isAllScaJobView) : false));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ProjectSession.ResellerId));
            List<FormBot.Entity.SolarCompany> lstSolarCompany = CommonDAL.ExecuteProcedure<FormBot.Entity.SolarCompany>(spName, sqlParameters.ToArray()).ToList();
            return lstSolarCompany;
        }

        /// <summary>
        /// Get all RAM by RA
        /// </summary>
        /// <param name="resellerId">reseller Id</param>
        /// <returns>Select List Item</returns>
        public IEnumerable<SelectListItem> GetAllRAMByRA(int resellerId)
        {
            string spName = "[GetAllRAMByRA]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, resellerId));
            return CommonDAL.ExecuteProcedure<FormBot.Entity.User>(spName, sqlParameters.ToArray()).ToList().Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.UserId),
                Text = Convert.ToString(d.Name)
            }).ToList();
        }

        /// <summary>
        /// Get payment for RA/RAM
        /// </summary>
        /// <param name="resellerId">resellerId</param>
        /// <param name="ramId">ramId</param>
        /// <returns>dataset</returns>
        public DataSet GetPaymentForRARAM(int resellerId, int ramId)
        {
            string spName = "[GetPaymentForRARAM]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, ramId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the requesting sc aand SSC for s eand sc.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <returns>List</returns>
        public List<FormBot.Entity.User> GetRequestingSCAandSSCForSEandSC(int UserId, int UserTypeId, string SortCol, string SortDir)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            List<FormBot.Entity.User> lstUser = CommonDAL.ExecuteProcedure<FormBot.Entity.User>("Dashboard_GetRequestingSCAandSSCForSEandSC", sqlParameters.ToArray()).ToList();
            return lstUser;
        }

        public FormBot.Entity.PricingManager GetDashboardJobCERApproved()
        {
            FormBot.Entity.PricingManager result = new Entity.PricingManager();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, ProjectSession.SolarCompanyId));
                sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
                result = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("GetDashboardJobCERApproved", sqlParameters.ToArray()).FirstOrDefault();

                if (result == null)
                {
                    FormBot.Entity.PricingManager pricing = new Entity.PricingManager();
                    pricing.Hour24 = 0;
                    pricing.Days3 = 0;
                    pricing.Days7 = 0;
                    pricing.CERApproved = 0;
                    pricing.STCAmount = 0;
                    return pricing;
                }

                if (result.LastUpdatedDate != null)
                {
                    result.LastUpdated = Convert.ToDateTime(result.LastUpdatedDate).ToString("dd/MM/yyy hh:mm tt");
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
        public DataSet GetNotifications(DateTime dt)
        {
            string spName = "[GetNotifications]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dt", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetNotificationsById(string SnackbarId, DateTime date)
        {
            string spName = "[GetNotificationsById]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.NVarChar, SnackbarId));

            sqlParameters.Add(DBClient.AddParameters("dt", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

    }
}

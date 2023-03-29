using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System.Web.Mvc;
using System;

namespace FormBot.BAL.Service
{
    public class ReportBAL : IReportBAL
    {

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="reportID">The report identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetReport(int? reportID)
        {
            string spName = "[Report_GetReportBindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ReportID", SqlDbType.Int, reportID));
            DataSet dsReportList = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsReportList;
        }

        /// <summary>
        /// Gets the report DRP.
        /// </summary>
        /// <returns>
        /// report list
        /// </returns>
        public List<Report> GetReportDrp()
        {
            string spName = "[Report_GetReportBindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ReportID", SqlDbType.Int, null));
            IList<Report> reportList = CommonDAL.ExecuteProcedure<Report>(spName, sqlParameters.ToArray());
            return reportList.ToList();
        }

        /// <summary>
        /// Gets the owner account.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetOwnerAccount()
        {
            string spName = "[RECOwnerAccount_OwnerAccount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<Report> ownerList = CommonDAL.ExecuteProcedure<Report>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.RECOwnerAccountId),
                Text = Convert.ToString(d.OwnerAccount)
            }).ToList();
        }

        /// <summary>
        /// Gets the type of the action.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetActionType()
        {
            string spName = "[RECActionType_ActionType]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<Report> ownerList = CommonDAL.ExecuteProcedure<Report>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.RECActionTypeId),
                Text = Convert.ToString(d.ActionType)
            }).ToList();
        }

        /// <summary>
        /// Gets the type of the status.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetStatusType()
        {
            string spName = "[RECStatus_StatusType]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<Report> ownerList = CommonDAL.ExecuteProcedure<Report>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.RECStatusId),
                Text = Convert.ToString(d.Status)
            }).ToList();
        }

        /// <summary>
        /// Gets the fuel source.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetFuelSource()
        {
            string spName = "[RECFuelSource_FuelSource]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<Report> ownerList = CommonDAL.ExecuteProcedure<Report>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.RECFuelSourceId),
                Text = Convert.ToString(d.FuelSource)
            }).ToList();
        }

        /// <summary>
        /// Gets the report list.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        public List<Report> GetReportList(Report reportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ReportType", SqlDbType.Int, reportModel.ReportID));
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.DateTime, reportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.DateTime, reportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("ActionTypes", SqlDbType.NVarChar, reportModel.hdnRECActionTypeAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("OwnerAccounts", SqlDbType.NVarChar, reportModel.hdnRECOwnerAccountAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("StatusType", SqlDbType.NVarChar, reportModel.hdnRECStatusTypeAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("FuelSourceType", SqlDbType.NVarChar, reportModel.hdnRECFuelSourceAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("GroupingType", SqlDbType.Int, reportModel.DateGrouping));
            List<Report> lstReport = CommonDAL.ExecuteProcedure<Report>("RECReport_Select", sqlParameters.ToArray()).ToList();
            return lstReport;
        }

        /// <summary>
        /// Gets the excel data.
        /// </summary>
        /// <param name="reportModel">The report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetExcelData(Report reportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ReportType", SqlDbType.Int, reportModel.ReportID));
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.DateTime, reportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.DateTime, reportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("ActionTypes", SqlDbType.NVarChar, reportModel.hdnRECActionTypeAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("OwnerAccounts", SqlDbType.NVarChar, reportModel.hdnRECOwnerAccountAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("StatusType", SqlDbType.NVarChar, reportModel.hdnRECStatusTypeAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("FuelSourceType", SqlDbType.NVarChar, reportModel.hdnRECFuelSourceAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("GroupingType", SqlDbType.Int, reportModel.DateGrouping));
            DataSet lstReport = CommonDAL.ExecuteDataSet("RECReport_Select", sqlParameters.ToArray());
            return lstReport;
        }

    }
}

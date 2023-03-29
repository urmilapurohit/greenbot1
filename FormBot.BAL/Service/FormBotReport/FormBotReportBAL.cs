using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System.Web.Mvc;
using System;
using FormBot.Helper;
using FormBot.Entity.Dashboard;

namespace FormBot.BAL.Service
{
    public class FormBotReportBAL : IFormBotReportBAL
    {
        /// <summary>
        /// Gets the form bot report DRP.
        /// </summary>
        /// <returns>
        /// report list
        /// </returns>
        public List<FormBotReport> GetFormBotReportDrp()
        {
            string spName = "[FormBotReports_GetFormBotReportBindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, ProjectSession.UserTypeId));
            IList<FormBotReport> reportList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return reportList.ToList();
        }

        /// <summary>
        /// Gets the solar company.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetSolarCompany(string isAllScaJobView = "" ,string hdnResellers = "")
        {
            string spName = "[GetSolarCompany]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LoggedInUserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerIDs", SqlDbType.NVarChar, hdnResellers));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ProjectSession.ResellerId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, !string.IsNullOrEmpty(isAllScaJobView) ? Convert.ToBoolean(isAllScaJobView) : false));
            IList<FormBotReport> ownerList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.SolarCompanyId),
                Text = Convert.ToString(d.SolarCompanyName)
            }).ToList();
        }

        /// <summary>
        /// Gets the preapproval status.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetPreapprovalStatus()
        {
            string spName = "[GetPreapprovalStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<FormBotReport> preapprovalStatusList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return preapprovalStatusList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.PreapprovalId),
                Text = Convert.ToString(d.PreapprovalStatus)
            }).ToList();
        }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetConnectionStatus()
        {
            string spName = "[GetConnectionStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<FormBotReport> preapprovalStatusList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return preapprovalStatusList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.ConnectionId),
                Text = Convert.ToString(d.ConnectionStatus)
            }).ToList();
        }

        /// <summary>
        /// Gets the STC submission status.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetSTCSubmissionStatus()
        {
            string spName = "[GetSTCSubmissionStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<FormBotReport> preapprovalStatusList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return preapprovalStatusList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.STCSubmissioinId),
                Text = Convert.ToString(d.STCSubmissioinStatus)
            }).ToList();
        }

        /// <summary>
        /// Gets the job status bar total reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        public List<FormBotReport> GetJobStatusBarTotalReportsList(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, formbotReportModel.DeletedFilter));
            sqlParameters.Add(DBClient.AddParameters("SCAIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>("GetJobStatusBarTotalReports", sqlParameters.ToArray()).ToList();
            return lstReport;
        }

        /// <summary>
        /// Gets the total active users report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        public List<FormBotReport> GetTotalActiveUsersReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TimePeriod", SqlDbType.Int, formbotReportModel.TimePeriod));
            sqlParameters.Add(DBClient.AddParameters("SCAIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("FSAId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotFSAAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("FCOId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotFCOAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RAMId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCOId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCOAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SSCId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SEId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("DeletedUsers", SqlDbType.Int, formbotReportModel.DeletedUsers));
            sqlParameters.Add(DBClient.AddParameters("PendingUsers", SqlDbType.Int, formbotReportModel.PendingUsers));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>("Report_GetActiveUsers", sqlParameters.ToArray()).ToList();
            return lstReport;
        }

        /// <summary>
        /// Gets the job status detail reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        //public List<FormBotReport> GetJobStatusDetailReportsList(FormBotReport formbotReportModel)
        public DataSet GetJobStatusDetailReportsList(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, formbotReportModel.LogginUserID));
            //sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, formbotReportModel.LoginUserType));
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, formbotReportModel.DeletedFilter));
            sqlParameters.Add(DBClient.AddParameters("SCAIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("PreapprovalId", SqlDbType.NVarChar, formbotReportModel.hdnPreapprovalStatusAssigned));
            sqlParameters.Add(DBClient.AddParameters("ConnectionId", SqlDbType.NVarChar, formbotReportModel.hdnConnectionStatusAssigned));
            sqlParameters.Add(DBClient.AddParameters("STCsubmission", SqlDbType.NVarChar, formbotReportModel.hdnSTCSubmissionStatusAssigned));
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.DateTime, formbotReportModel.EndDate));

            DataSet dsReport = CommonDAL.ExecuteDataSet("[GetJobStatusDetailReport_New]", sqlParameters.ToArray());
            return dsReport;
            //List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>("GetJobStatusDetailReport", sqlParameters.ToArray()).ToList();
            //return lstReport;
        }

        /// <summary>
        /// Gets the job stages report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        public DataSet GetJobStagesReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, formbotReportModel.LogginUserID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, formbotReportModel.LoginUserType));
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, formbotReportModel.DeletedFilter));
            sqlParameters.Add(DBClient.AddParameters("SCAIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            DataSet dsReport = CommonDAL.ExecuteDataSet("[GetJobStagesReport]", sqlParameters.ToArray());
            return dsReport;
            //List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>("GetJobStagesReport", sqlParameters.ToArray()).ToList();
            //return lstReport;
        }

        /// <summary>
        /// Gets the total job list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// slected list item
        /// </returns>
        //public List<SelectListItem> GetTotalJobList(FormBotReport formbotReportModel)
        //{
        //    string spName = "[GetTotalJobReport]";
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("Reseller", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
        //    sqlParameters.Add(DBClient.AddParameters("SoalarCompany", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
        //    sqlParameters.Add(DBClient.AddParameters("SSC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
        //    sqlParameters.Add(DBClient.AddParameters("SC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCAssignedUser));
        //    sqlParameters.Add(DBClient.AddParameters("SE", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
        //    sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, Convert.ToInt32(formbotReportModel.DeletedFilter)));
        //    sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, Convert.ToInt32(ProjectSession.ResellerId)));
        //    DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        //    if (dataset != null && dataset.Tables.Count > 0)
        //    {
        //        return dataset.Tables[0].AsEnumerable().Select(d => new SelectListItem()
        //        {
        //            Value = d.Field<string>("NAME"),
        //            Text = Convert.ToString(d.Field<Int32>("Total"))
        //        }).ToList();
        //    }
        //    return null;
        //}

        /// <summary>
        /// Gets the total job list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// slected list item
        /// </returns>
        public List<FormBotReport> GetTotalJobList(FormBotReport formbotReportModel)
        {
            string spName = "[GetTotalJobReport]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Reseller", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SoalarCompany", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SSC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SE", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, Convert.ToInt32(formbotReportModel.DeletedFilter)));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, Convert.ToInt32(ProjectSession.ResellerId)));
            List<FormBotReport> lstjobs = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray()).ToList();
            return lstjobs;
        }

        /// <summary>
        /// Gets the form bot user.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns>select list</returns>
        public IEnumerable<SelectListItem> GetFormBotUser(int userType, string hdnSolarCompanies = "", int? UserId = 0)
        {
            string spName = "[GetUserForReport]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserType", SqlDbType.Int, userType));
            sqlParameters.Add(DBClient.AddParameters("solarCompanies", SqlDbType.NVarChar, hdnSolarCompanies));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataset != null && dataset.Tables.Count > 0)
            {
                IEnumerable<SelectListItem> lst = new List<SelectListItem>();
                lst = dataset.Tables[0].AsEnumerable().Select(d => new SelectListItem()
                {
                    Value = Convert.ToString(d.Field<int>("UserId")),
                    Text = d.Field<string>("Name")

                });
                return lst;
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// Gets the system user reports reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// report list
        /// </returns>
        public DataSet GetSystemUserReportsReportsList(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("IsAllSelected", SqlDbType.Bit, formbotReportModel.IsAllSelected));
            sqlParameters.Add(DBClient.AddParameters("OtherFilter", SqlDbType.Int, formbotReportModel.OtherFilter));
            //List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>("FBSystemUserReports", sqlParameters.ToArray()).ToList();
            DataSet dsReport = CommonDAL.ExecuteDataSet("FBSystemUserReports", sqlParameters.ToArray());
            //return lstReport;
            return dsReport;
        }

        /// <summary>
        /// Gets the allocation report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetAllocationReportList(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("LoggedinUserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("Fromdate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("FCO_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotFCOAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RAM_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("Reseller_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCA_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCO_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCOAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SSC_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SE_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SC_AssignedUser", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("OtherFilter", SqlDbType.NVarChar, formbotReportModel.OtherFilter));
            DataSet dsReport = CommonDAL.ExecuteDataSet("FBAllocationReports", sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the job invoice report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GetJobInvoiceReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Reseller", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SoalarCompany", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SSC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SC", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SE", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("Fromdate", SqlDbType.Date, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.Date, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("LoggedInUserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("InvoiceType", SqlDbType.Int, formbotReportModel.InvoiceType));
            sqlParameters.Add(DBClient.AddParameters("InvoiceMode", SqlDbType.Int, formbotReportModel.InvoiceMode));
            sqlParameters.Add(DBClient.AddParameters("InvoiceStatus", SqlDbType.NVarChar, formbotReportModel.hdnFormBotStatusAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("IsCreditNote", SqlDbType.NVarChar, formbotReportModel.IsCreditNote));
            DataSet dsReport = CommonDAL.ExecuteDataSet("JobInvoiceReport", sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the form bot ram user.
        /// </summary>
        /// <param name="formBotReportModel">The form bot report model.</param>
        /// <returns>select list</returns>
        public IEnumerable<SelectListItem> GetFormBotRAMUser(FormBotReport formBotReportModel)
        {
            string spName = "[GetFormBotRAMUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, formBotReportModel.ResellerID));
            sqlParameters.Add(DBClient.AddParameters("hdnReseller", SqlDbType.NVarChar, formBotReportModel.hdnResellers));
            IList<FormBotReport> ownerList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.UserID),
                Text = Convert.ToString(d.UserName)
            }).ToList();
        }

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// formbot report
        /// </returns>
        public DataSet GetRECFailureReasonsReportsList(FormBotReport formbotReportModel)
        {
            string spName = "[FB_RECFailure_Reasons_Reports]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("RAIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RAMIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            //List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray()).ToList();
            //DataSet ds = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray()).ToList();
            DataSet dsReport = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsReport;
            //return lstReport;            
        }

        /// <summary>
        /// Gets the record failure reasons reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// formbot report
        /// </returns>
        public List<FormBotReport> GetRECFailureReasonsDashboardReportsList(FormBotReport formbotReportModel)
        {
            string spName = "[FB_RECFailure_Reasons_Dashboard_Reports]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("RAIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RAMIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray()).ToList();
            return lstReport;
        }

        /// <summary>
        /// Gets the se user level reports list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// formbot report
        /// </returns>
        public List<FormBotReport> GetSEUserLevelReportsList(FormBotReport formbotReportModel)
        {
            string spName = "[FB_SE_User_Level_Reports]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.DateTime, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("GroupingType", SqlDbType.Int, formbotReportModel.TimePeriod));
            sqlParameters.Add(DBClient.AddParameters("LogginUserID", SqlDbType.NVarChar, formbotReportModel.LogginUserID));
            List<FormBotReport> lstReport = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray()).ToList();
            return lstReport;
        }

        /// <summary>
        /// Gets the job stages.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetJobStages()
        {
            string spName = "[GetJobStages]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<FormBotReport> ownerList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.JobStageID),
                Text = Convert.ToString(d.StageName)
            }).ToList();
        }

        /// <summary>
        /// Gets the sales agent.
        /// </summary>
        /// <returns>
        /// select list
        /// </returns>
        public IEnumerable<SelectListItem> GetSalesAgent(string hdnSolarCompanies = "")
        {
            string spName = "[GetSalesAgent]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SCAIDs", SqlDbType.NVarChar, hdnSolarCompanies));
            IList<FormBotReport> ownerList = CommonDAL.ExecuteProcedure<FormBotReport>(spName, sqlParameters.ToArray());
            return ownerList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.SoldBy),
                Text = Convert.ToString(d.SoldBy)
            }).ToList();
        }

        /// <summary>
        /// Gets the sold by who report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSoldByWhoReportList(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.DateTime, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("RAIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCAIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("JobStages", SqlDbType.NVarChar, formbotReportModel.hdnJobStagesAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SoldBy", SqlDbType.NVarChar, formbotReportModel.hdnSalesAgentAssignedUser));
            DataSet dsReport = CommonDAL.ExecuteDataSet("FB_Sold_By_Who_Report", sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the sscse jobs detail report list.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSSCSEJobsDetailReportList(FormBotReport formbotReportModel)
        {
            string spName = "[FB_SSC_SE_JobsDetail_Report]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SSCIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSSCAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SEIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSEAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCAIDs", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("JobStatus", SqlDbType.NVarChar, Convert.ToString(formbotReportModel.JobStageID)));
            DataSet dsReport = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the STC general report.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSTCGeneralReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, formbotReportModel.DeletedFilter));
            sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RamIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.Date, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.Date, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("JobStageIds", SqlDbType.NVarChar, formbotReportModel.hdnSTCSubmissionStatusAssigned));
            sqlParameters.Add(DBClient.AddParameters("isAll", SqlDbType.Bit, formbotReportModel.IsAllSelected));
            DataSet dsReport = CommonDAL.ExecuteDataSet("STCGeneralReport", sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the STC general report for RAM.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSTCGeneralReportForRAM(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DeletedFilter", SqlDbType.Int, formbotReportModel.DeletedFilter));
            sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RamIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.Date, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.Date, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("JobStageIds", SqlDbType.NVarChar, formbotReportModel.hdnSTCSubmissionStatusAssigned));
            sqlParameters.Add(DBClient.AddParameters("isDetail", SqlDbType.Int, formbotReportModel.IsDetail));
            DataSet dsReport = CommonDAL.ExecuteDataSet("STCGeneralReportRAM", sqlParameters.ToArray());
            return dsReport;
        }

        /// <summary>
        /// Gets the STC general report for dashboard.
        /// </summary>
        /// <param name="formbotReportModel">The formbot report model.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetSTCGeneralDashboardReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("RamIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotRAMAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.Date, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.Date, formbotReportModel.EndDate));
            sqlParameters.Add(DBClient.AddParameters("JobStageIds", SqlDbType.NVarChar, formbotReportModel.hdnSTCSubmissionStatusAssigned));
            DataSet dsReport = CommonDAL.ExecuteDataSet("STCGeneralDashboardReport", sqlParameters.ToArray());
            return dsReport;
        }


        /// <summary>
        /// Get Trade Data count for SCA dashboard display in chart
        /// </summary>
        /// <param name="StratDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<SCATradeReport> GetSCATradedChartData(string StartDate, string EndDate, int Type)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.NVarChar, !string.IsNullOrEmpty(StartDate) ? StartDate : DateTime.Now.ToString("yyyy-MM-dd")));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.NVarChar, !string.IsNullOrEmpty(EndDate) ? EndDate: DateTime.Now.ToString("yyyy-MM-dd")));
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.Int, Type));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            return CommonDAL.ExecuteProcedure<SCATradeReport>("GetSCATradedChartData", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Get SCA Status Chart Data
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<SCAStatusReport> GetSCAStatusChartData(int Type)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.Int, Type));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            return CommonDAL.ExecuteProcedure<SCAStatusReport>("GetSCAStatusChartData", sqlParameters.ToArray()).ToList();
        }

        public DataSet GetNonTradeJobReport(FormBotReport formbotReportModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.NVarChar, formbotReportModel.hdnFormBotResellerAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("SCAIds", SqlDbType.NVarChar, formbotReportModel.hdnFormBotSolarCompanyAssignedUser));
            sqlParameters.Add(DBClient.AddParameters("datStart", SqlDbType.DateTime, formbotReportModel.StartDate));
            sqlParameters.Add(DBClient.AddParameters("datEnd", SqlDbType.DateTime, formbotReportModel.EndDate));
            DataSet dsReport = CommonDAL.ExecuteDataSet("GetSolarCompanyListByStcStatus", sqlParameters.ToArray());
            return dsReport;
        }
    }
}

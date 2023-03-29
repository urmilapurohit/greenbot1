using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data.SqlClient;
using FormBot.DAL;
using System.Data;
using FormBot.Helper;

namespace FormBot.BAL
{
    public class ResellerBAL : IResellerBAL
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="FCOId">The fco identifier.</param>
        /// <returns>
        /// reseller list
        /// </returns>
        public List<Reseller> GetData(int? FCOId, bool IsPeakPay = false,bool isFromwindowService=false)
        {
            string spName = "[Reseller_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FCOId", SqlDbType.Int, FCOId));
            sqlParameters.Add(DBClient.AddParameters("IsPeakPay", SqlDbType.Bit, IsPeakPay));
            sqlParameters.Add(DBClient.AddParameters("isFromwindowService", SqlDbType.Bit, isFromwindowService));
            IList <Reseller> resellerDetailList = CommonDAL.ExecuteProcedure<Reseller>(spName, sqlParameters.ToArray());
            return resellerDetailList.ToList();
        }

        /// <summary>
        /// Gets the SAAS User data.
        /// </summary>
        /// <returns>reseller list</returns>
        public List<Reseller> GetSAASUsers()
        {
            string spName = "[Reseller_SAAS]";
            IList<Reseller> resellerDetailList = CommonDAL.ExecuteProcedure<Reseller>(spName);
            return resellerDetailList.ToList();
        }

 /// <summary>
        /// Get reseller batch data
        /// </summary>
        /// <param name="FCOId"></param>
        /// <param name="IsPeakPay"></param>
        /// <param name="isFromwindowService"></param>
        /// <returns></returns>
        public List<ResellerBatch> GetBatchData()
        {
            string spName = "[ResellerBatch_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<ResellerBatch> resellerDetailList = CommonDAL.ExecuteProcedure<ResellerBatch>(spName, sqlParameters.ToArray());
            return resellerDetailList.ToList();
        }
        
public List<Reseller> GetAllResellersForREC()
        {
            string spName = "[GetAllResellersForREC]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<Reseller> resellerDetailList = CommonDAL.ExecuteProcedure<Reseller>(spName, sqlParameters.ToArray());
            return resellerDetailList.ToList();
        }

        /// <summary>
        /// Fetch reseller details by login company name
        /// </summary>
        /// <param name="companyName">Name of the company.</param>
        /// <returns>Returns Reseller By Login Company Name</returns>
        public DataSet GetResellerByLoginCompanyName(string companyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("LoginCompanyName", SqlDbType.NVarChar, companyName));
            DataSet dsReseller = CommonDAL.ExecuteDataSet("Reseller_GetResellerByResellerId", sqlParameters.ToArray());
            return dsReseller;
        }

        /// <summary>
        /// Gets the job identifier by PVD code.
        /// </summary>
        /// <param name="PVDCode">The PVD code.</param>
        /// <returns>
        /// DataTable
        /// </returns>
        public DataTable GetJobIDByPVDCode(string PVDCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PVDCode", SqlDbType.NVarChar, PVDCode));
            DataTable dtReseller = CommonDAL.ExecuteDataSet("[GetJobIDByPVDCode]", sqlParameters.ToArray()).Tables[0];
            return dtReseller;
        }

        /// <summary>
        /// Check user belongs to reseller company
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="username">The username.</param>
        /// <returns>Returns User Belongs To Reseller Company</returns>
        public bool CheckUserBelogsToResellerCompany(int resellerId, string username)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserName", SqlDbType.NVarChar, username));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            var isExists = CommonDAL.ExecuteScalar("User_ExistsInResellerCompany", sqlParameters.ToArray());
            return isExists.ToString() == "1" ? true : false;
        }

        /// <summary>
        /// Gets the reseller by reseller identifier.
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>reseller object</returns>
        public Reseller GetResellerByResellerID(int? resellerId)
        {
            resellerId = resellerId != null && resellerId > 0 ? resellerId : 0;
            Reseller reseller = CommonDAL.SelectObject<Reseller>("ResellerDetails_GetResellerByResellerID", DBClient.AddParameters("ResellerID", SqlDbType.Int, resellerId));
            return reseller;
        }

        /// <summary>
        /// Gets the reseller account manager by reseller identifier.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>user list</returns>
        public List<User> GetResellerAccountManagerByResellerId(int ResellerId)
        {
            string spName = "[GetResellerAccountManagerByResellerId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            IList<User> lstAccountManagers = CommonDAL.ExecuteProcedure<User>(spName, sqlParameters.ToArray());
            return lstAccountManagers.ToList();
        }

        /// <summary>
        /// Gets the resellers email account.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetResellersEmailAccount()
        {
            string spName = "[awmAccounts_GetResselersAccount]";
            return CommonDAL.ExecuteDataSet(spName, null);
        }

        //public List<REC> GetRecFailureReason1(int ResellerId)
        //{
        //    string spName = "[GetRecFailureReason]";
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
        //    IList<REC> lstRec = CommonDAL.ExecuteProcedure<REC>(spName, sqlParameters.ToArray());
        //    return lstRec.ToList();
        //}

        public DataSet GetRecFailureReason(SearchParamRec searchParam,int pagesize,int pageNumber,string sortCol,string sortDir)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (searchParam.ResellerId != 0)
                sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, searchParam.ResellerId));
            if (!string.IsNullOrWhiteSpace(searchParam.BulkUploadId))
                sqlParameters.Add(DBClient.AddParameters("BulkUploadId", SqlDbType.NVarChar, searchParam.BulkUploadId));
            if (!string.IsNullOrWhiteSpace(searchParam.RECUsername))
                sqlParameters.Add(DBClient.AddParameters("RECUsername", SqlDbType.VarChar, searchParam.RECUsername));
            if (!string.IsNullOrWhiteSpace(searchParam.RECName))
                sqlParameters.Add(DBClient.AddParameters("RECName", SqlDbType.VarChar, searchParam.RECName));
            if (!string.IsNullOrWhiteSpace(searchParam.InitiatedBy))
                sqlParameters.Add(DBClient.AddParameters("InitiatedBy", SqlDbType.VarChar, searchParam.InitiatedBy));
            if (searchParam.DateFrom != null)
                sqlParameters.Add(DBClient.AddParameters("DateFrom", SqlDbType.DateTime, searchParam.DateFrom));
            if (searchParam.DateTo != null)
                sqlParameters.Add(DBClient.AddParameters("DateTo", SqlDbType.DateTime, searchParam.DateTo));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, searchParam.StageId));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pagesize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.NVarChar, sortDir));
            DataSet dsRecFailureReason = CommonDAL.ExecuteDataSet("GetRecStatus_FailureReason", sqlParameters.ToArray());
            return dsRecFailureReason;
        }

        public void DeleteRecFailureReason(string id)
        {
            string spName = "[DeleteRecFailureReason]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Id", SqlDbType.NVarChar, id));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
        }

        public void ReleaseRecUploadIdForRecreation(string recUploadId,string dateTimeTicks)
        {
            string spName = "[ReleaseRecUploadIdForRecreation]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RecUploadId", SqlDbType.NVarChar, recUploadId));
            sqlParameters.Add(DBClient.AddParameters("dateTimeTicks", SqlDbType.VarChar, dateTimeTicks));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
        }

        public DataSet GetDatetimeTickForBatch(string recUploadId)
        {
            string spName = "[GetDatetimeTickForBatchId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RecUploadId", SqlDbType.NVarChar, recUploadId));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }

        public DataSet GetSPVFailureReason(string refJobId = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RefJobId", SqlDbType.VarChar, refJobId));
            DataSet dsRecFailureReason = CommonDAL.ExecuteDataSet("GetSPVfailureReasons", sqlParameters.ToArray());
            return dsRecFailureReason;
        }
        public DataTable GetDataForNullFilePath_rid()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            DataTable dt = CommonDAL.ExecuteDataSet("GetDataForNullFilePath_rid", sqlParameters.ToArray()).Tables[0];
            return dt;

        }
        /// <summary>
        /// insert log for sync wit xero
        /// </summary>
        /// <param name="resellerID"></param>
        public void SyncToXeroWithReseller(int resellerID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("resellerId", SqlDbType.Int, resellerID));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteScalar("InsertSyncToXeroLog", sqlParameters.ToArray());
        }


    }
}

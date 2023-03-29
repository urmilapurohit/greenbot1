using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;

namespace FormBot.BAL
{
    public interface IResellerBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="FCOId">The fco identifier.</param>
        /// <returns>reseller list</returns>
        List<Reseller> GetData(int? FCOId, bool IsPeakPay = false,bool isFromwindowService=false);

        /// <summary>
        /// Get the reseller batch data
        /// </summary>
        /// <param name="FCOId"></param>
        /// <param name="IsPeakPay"></param>
        /// <param name="isFromwindowService"></param>
        /// <returns></returns>
        List<ResellerBatch> GetBatchData();

        /// <summary>
        /// Gets the SAAS User data.
        /// </summary>
        /// <returns>reseller list</returns>
        List<Reseller> GetSAASUsers();

        /// <summary>
        /// Gets the name of the reseller by login company.
        /// </summary>
        /// <param name="companyName">Name of the company.</param>
        /// <returns>DataSet row</returns>
        DataSet GetResellerByLoginCompanyName(string companyName);

        /// <summary>
        /// Checks the user belogs to reseller company.
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="username">The username.</param>
        /// <returns>true false value</returns>
        bool CheckUserBelogsToResellerCompany(int resellerId, string username);

        /// <summary>
        /// Gets the reseller by reseller identifier.
        /// </summary>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>Reseller details</returns>
        Reseller GetResellerByResellerID(int? resellerId);

        /// <summary>
        /// Gets the reseller account manager by reseller identifier.
        /// </summary>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns>user list</returns>
        List<User> GetResellerAccountManagerByResellerId(int ResellerId);

        /// <summary>
        /// Gets the resellers email account.
        /// </summary>
        /// <returns>DataSet</returns>
        DataSet GetResellersEmailAccount();

        /// <summary>
        /// Gets the job identifier by PVD code.
        /// </summary>
        /// <param name="PVDCode">The PVD code.</param>
        /// <returns>DataTable</returns>
        DataTable GetJobIDByPVDCode(string PVDCode);

        //List<REC> GetRecFailureReason(int ResellerId);

        DataSet GetRecFailureReason(SearchParamRec searchParam,int pageSize,int PageNumber,string sortCol,string sortDir);

        void DeleteRecFailureReason(string id);

        void ReleaseRecUploadIdForRecreation(string recUploadId,string datetimeTicks);
        DataSet GetSPVFailureReason(string refJobId = "");
        DataTable GetDataForNullFilePath_rid();
        /// <summary>
        /// insert log for sync wit xero
        /// </summary>
        /// <param name="resellerID"></param>
        void SyncToXeroWithReseller(int resellerID);

        /// <summary>
        /// Get Old Datetimetick for batchId
        /// </summary>
        /// <param name="recUploadId"></param>
        /// <returns></returns>
        DataSet GetDatetimeTickForBatch(string recUploadId);

    }
}

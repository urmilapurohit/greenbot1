using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface ICreateJobHistoryBAL
    {
        /// <summary>
        /// Logs the job history.
        /// </summary>
        /// <typeparam name="T">type name</typeparam>
        /// <param name="objJob">The object job.</param>
        /// <param name="HistoryCategoryID">The history category identifier.</param>
        /// <param name="UserId">UserId (VendorApi).</param>
        /// <returns>Job History</returns>
        bool LogJobHistory<T>(T objJob, HistoryCategory HistoryCategoryID,int UserId = 0);
    }
}

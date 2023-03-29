using FormBot.DAL;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.ActivityLog
{
    public class ActivityLogBAL : IActivityLogBAL
    {
        public List<Entity.ActivityLog.ActivityLog> GetActivityLogs(int UserId, int ActivityTypeId, string StartDate, string EndDate, int Page, int Pagesize)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("ActivityTypeId", SqlDbType.Int, ActivityTypeId));
            sqlParameters.Add(DBClient.AddParameters("StartDate", SqlDbType.NVarChar, StartDate));
            sqlParameters.Add(DBClient.AddParameters("EndDate", SqlDbType.NVarChar, EndDate));
            sqlParameters.Add(DBClient.AddParameters("Page", SqlDbType.Int, Page));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, Pagesize));
            return CommonDAL.ExecuteProcedure<Entity.ActivityLog.ActivityLog>("GetActivityLogsByUserId", sqlParameters.ToArray()).ToList();
        }

        public void InsertActivityLog(Entity.ActivityLog.ActivityLog objActivityLog)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int,objActivityLog.UserId));
            sqlParameters.Add(DBClient.AddParameters("ActivityLogType", SqlDbType.Int, objActivityLog.ActvityLogTypeId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("LogMessage", SqlDbType.NVarChar, objActivityLog.LogMessage));
            sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, objActivityLog.IpAddress));
            CommonDAL.ExecuteScalar("InsertActivityLog", sqlParameters.ToArray());
        }
    }
}

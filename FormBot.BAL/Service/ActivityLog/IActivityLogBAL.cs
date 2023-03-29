using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.ActivityLog
{
    public interface IActivityLogBAL
    {
        List<Entity.ActivityLog.ActivityLog> GetActivityLogs(int UserId, int ActivityTypeId, string StartDate, string EndDate,int Page,int Pagesize);

        void InsertActivityLog(Entity.ActivityLog.ActivityLog objActivityLog);
    }
}

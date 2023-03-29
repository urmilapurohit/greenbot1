using FormBot.DAL;
using FormBot.Entity.Settings;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FormBot.BAL.Service
{
    public class GBsettingsBAL : IGBsettingsBAL
    {
        public GBsetting GetGBsettingValueByKeyName(string KeyName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("KeyName", SqlDbType.NVarChar, KeyName));
            GBsetting objGBsetting = CommonDAL.ExecuteProcedure<GBsetting>("GetGBsettingValueByKeyName", sqlParameters.ToArray()).FirstOrDefault();
            return objGBsetting;
        }
        public void SetGBsettingValueByKeyName(string KeyName,string Value, int UserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("KeyName", SqlDbType.NVarChar, KeyName));
            sqlParameters.Add(DBClient.AddParameters("Value", SqlDbType.NVarChar, Value));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("SetGBsettingValueByKeyNameWithLog", sqlParameters.ToArray());
        }
    }
}

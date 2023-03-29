using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;

namespace FormBot.BAL.Service
{
    public class ResellerBAL : IResellerDetailBAL
    {
        public List<ResellerDetailView> GetData()
        {
            string spName = "[ResellerDetail_BindDropdown]";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.BigInt, 8));

            // DataSet dsUserData = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            // List<Title> usersList = DBClient.DataTableToList<Title>(dsUserData.Tables[0]);
            IList<ResellerDetailView> resellerList = CommonDAL.ExecuteProcedure<ResellerDetailView>(spName, sqlParameters.ToArray());

            return resellerList.ToList();
        }
    }
}

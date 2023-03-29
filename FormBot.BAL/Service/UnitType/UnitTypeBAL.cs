using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System;

namespace FormBot.BAL.Service
{
    public class UnitTypeBAL : IUnitTypeBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>unit list</returns>
        public List<UnitType> GetData()
        {
            string spName = "[UnitType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<UnitType> unitTypeList = CommonDAL.ExecuteProcedure<UnitType>(spName, sqlParameters.ToArray());
            return unitTypeList.ToList();
        }

        /// <summary>
        /// Gets the unitname by unit identifier.
        /// </summary>
        /// <param name="unitId">The unit identifier.</param>
        /// <returns>Returns Unit Name</returns>
        public string GetUnitnameByUnitId(int unitId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UnitID", SqlDbType.Int, unitId));
            return Convert.ToString(CommonDAL.ExecuteScalar("UnitType_GetUnitNameByUnitId", sqlParameters.ToArray()));
        }
    }
}

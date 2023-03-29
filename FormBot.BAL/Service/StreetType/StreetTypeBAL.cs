using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity;
using System.Linq;
using System;

namespace FormBot.BAL.Service
{
    public class StreetTypeBAL : IStreetTypeBAL
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>street list</returns>
        public List<StreetType> GetData()
        {
            string spName = "[StreetType_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<StreetType> streetTypeList = CommonDAL.ExecuteProcedure<StreetType>(spName, sqlParameters.ToArray());
            return streetTypeList.ToList();
        }

        /// <summary>
        /// Gets the street type name by street type identifier.
        /// </summary>
        /// <param name="streetTypeId">The street type identifier.</param>
        /// <returns>Returns Street type name</returns>
        public string GetStreetTypeNameByStreetTypeId(int streetTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("streetTypeID", SqlDbType.Int, streetTypeId));
            return Convert.ToString(CommonDAL.ExecuteScalar("StreetType_GetStreetTypeNameByStreetTypeId", sqlParameters.ToArray()));
        }
    }
}

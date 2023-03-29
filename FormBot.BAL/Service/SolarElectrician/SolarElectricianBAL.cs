using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service.SolarElectrician
{
    public class SolarElectricianBAL : ISolarElectricianBAL
    {
        //public DataSet GetAllElectrician(bool isInstaller, int solarCompanyId)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, isInstaller));
        //    sqlParameters.Add(DBClient.AddParameters("CompanyId", SqlDbType.BigInt, solarCompanyId));
        //    DataSet dsJobAndScheduling = CommonDAL.ExecuteDataSet("Job_GetSEUser", sqlParameters.ToArray());
        //    return dsJobAndScheduling;
        //}

        public List<SolarElectricianView> GetAllElectrician(bool isInstaller, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, isInstaller));
            sqlParameters.Add(DBClient.AddParameters("CompanyId", SqlDbType.BigInt, solarCompanyId));
            IList<SolarElectricianView> electrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("Job_GetSEUser", sqlParameters.ToArray());
            return electrician.ToList();
        }
    }
}

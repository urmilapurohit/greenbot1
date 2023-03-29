using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FormBot.Helper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECLightingComponent
{
    public class VEECLightingComponentBAL : IVEECLightingComponentBAL
    {
        public int SaveLightingComponentTemplate(string veecLightingComponentId, string veecLightingComponentName,string path)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECLightingComponentId", SqlDbType.Int, string.IsNullOrEmpty(veecLightingComponentId)? 0 : Convert.ToInt32(veecLightingComponentId)));
            sqlParameters.Add(DBClient.AddParameters("VEECLightingComponentName", SqlDbType.NVarChar, veecLightingComponentName));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            int id = Convert.ToInt32(CommonDAL.ExecuteScalar("SaveLightingComponentTemplate", sqlParameters.ToArray()));
            return id;
        }

        public IList<FormBot.Entity.VEECLightingComponent> GetLightingComponentList(int pageNumber, int pageSize, string sortCol, string sortDir, string lightingComponentName)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("LightingComponentName", SqlDbType.NVarChar, string.IsNullOrEmpty(lightingComponentName) ? null : lightingComponentName));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIds", SqlDbType.NVarChar, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            IList<FormBot.Entity.VEECLightingComponent> lstVEECLightingComponent = CommonDAL.ExecuteProcedure<FormBot.Entity.VEECLightingComponent>("LightingComponent_ListBySolarCompanyId", sqlParameters.ToArray()).ToList();
            return lstVEECLightingComponent;
        }

        public void DeleteLightingComponent(string deleteLightingComponentIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DeleteLightingComponentId", SqlDbType.NVarChar, deleteLightingComponentIds));
            CommonDAL.ExecuteScalar("DeleteLightingComponent", sqlParameters.ToArray());
        }

        public string GetLightingComponentFilePath(int VEECLightingComponentId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECLightingComponentId", SqlDbType.Int, VEECLightingComponentId));
            return Convert.ToString(CommonDAL.ExecuteScalar("GetLightingComponentFilePath", sqlParameters.ToArray()));
        }
    }
}

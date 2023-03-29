using FormBot.DAL;
using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECPricingManager
{
    public class VEECPricingManagerBAL : IVEECPricingManagerBAL
    {
        public FormBot.Entity.VEECPricingManager GetVEECGlobalPriceForReseller(int ResellerId)
        {
            FormBot.Entity.VEECPricingManager veecPricingManager = CommonDAL.SelectObject<FormBot.Entity.VEECPricingManager>("VEECPricingGlobal_GetVEECGlobalPriceForReseller", DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            return veecPricingManager;
        }

        public List<FormBot.Entity.VEECPricingManager> GetSCAUserForVEECPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, string SolarCompany, string Name)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompany", SqlDbType.NVarChar, SolarCompany));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, Name));
            List<FormBot.Entity.VEECPricingManager> lstSCAUser = CommonDAL.ExecuteProcedure<FormBot.Entity.VEECPricingManager>("Users_GetSCAUserForVEECPricingManager", sqlParameters.ToArray()).ToList();
            return lstSCAUser;
        }

        public List<FormBot.Entity.VEECPricingManager> GetVEECsForVEECPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, int SolarCompanyId, string veecRef, string HomeOwnerName, string VeecInstallationAddress)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("veecRef", SqlDbType.NVarChar, veecRef));
            sqlParameters.Add(DBClient.AddParameters("HomeOwnerName", SqlDbType.NVarChar, HomeOwnerName));
            sqlParameters.Add(DBClient.AddParameters("VeecInstallationAddress", SqlDbType.NVarChar, VeecInstallationAddress));
            List<FormBot.Entity.VEECPricingManager> lstVEEC = CommonDAL.ExecuteProcedure<FormBot.Entity.VEECPricingManager>("VEECs_GetVeecForVEECPricingManager", sqlParameters.ToArray()).ToList();
            return lstVEEC;
        }

        public void SaveVEECGlobalPriceForSolarCompany(int ResellerId, decimal optiPayPrice)
        {
            string spName = "VEECPricingGlobal_SaveVEECGlobalPriceForSolarCompany";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));

            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        public void SaveVEECCustomPriceForSolarCompany(List<int> lstSolarCompany, DateTime ExpiryDate, int ResellerId, decimal optiPayPrice)
        {
            var iDs = lstSolarCompany.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyIDs", SqlDbType.NVarChar, iDs));
            sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.DateTime, ExpiryDate));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));

            CommonDAL.Crud("VEECPricingSolarCompany_SaveVEECCustomPriceForSolarCompany", sqlParameters.ToArray());
        }

        public void SaveVEECCustomPriceForVeec(List<int> lstVEEC, DateTime ExpiryDate, int SolarCompanyId,decimal optiPayPrice)
        {
            var iDs = lstVEEC.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecIDs", SqlDbType.NVarChar, iDs));
            sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.DateTime, ExpiryDate));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("LastUpdatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("OptiPayPrice", SqlDbType.Decimal, optiPayPrice));

            CommonDAL.Crud("VEECPricingVeec_SaveVEECCustomPriceForVeec", sqlParameters.ToArray());
        }

        public List<VEECList> GetVEECsForCustomPricing(int SolarCompanyId, string OwnerName, string InstallationAddress, string RefNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, OwnerName));
            sqlParameters.Add(DBClient.AddParameters("InstallationAddress", SqlDbType.NVarChar, InstallationAddress));
            sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, RefNumber));
            List<VEECList> lstVEEC = CommonDAL.ExecuteProcedure<VEECList>("GetVEECsForCustomPricing", sqlParameters.ToArray()).ToList();
            return lstVEEC;
        }
    }
}

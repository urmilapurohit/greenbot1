using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data.SqlClient;
using FormBot.DAL;
using System.Data;
using FormBot.Helper;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service
{
    public class SolarCompanyBAL : ISolarCompanyBAL
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>solar list</returns>
        public List<SolarCompany> GetData()
        {
            string spName = "[SolarCompany_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the sub contractor data.
        /// </summary>
        /// <returns>solar list</returns>
        public List<SolarCompany> GetSubContractorData()
        {
            string spName = "[SolarCompany_SubContractorBindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<SolarCompany> subContractorList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return subContractorList.ToList();
        }

        /// <summary>
        /// Gets the solar company by reseller identifier.
        /// </summary>
        /// <param name="id">The Reseller identifier.</param>
        /// <returns>List of SolarCompanies under this Reseller</returns>
        public List<SolarCompany> GetSolarCompanyByResellerID(int id)
        {
            string spName = "[SolarCompanyByResellerID_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the solar companies by multiple reseller identifier.
        /// </summary>
        /// <param name="id">The Reseller identifier.</param>
        /// <returns>List of SolarCompanies under multiple Reseller</returns>
        public List<SolarCompany> GetSolarCompanyByMultipleResellerID(string id)
        {
            string spName = "[SolarCompanyByMultipleResellerID_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerIDs", SqlDbType.VarChar, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the solar company by ramid.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// solar list
        /// </returns>
        public List<SolarCompany> GetSolarCompanyByRAMID(int id)
        {
            string spName = "[SolarCompany_GetSolarCompanyByRAMID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the solar company by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>solar object</returns>
        public SolarCompany GetSolarCompanyBySolarCompanyID(int? solarCompanyId)
        {
            solarCompanyId = solarCompanyId != null && solarCompanyId > 0 ? solarCompanyId : 0;
            SolarCompany solarCompany = CommonDAL.SelectObject<SolarCompany>("SolarCompany_GetSolarCompanyBySolarCompanyID", DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyId));
            return solarCompany;
        }

        /// <summary>
        /// Gets the solar company for global pricing.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="PricingType">Type of the pricing.</param>
        /// <returns>
        /// solar list
        /// </returns>
        public List<SolarCompany> GetSolarCompanyForGlobalPricing(int UserId, int UserTypeId, int ResellerId, int PricingType)
        {
            string spName = "[GetSolarCompanyForGlobalPricing]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("PricingType", SqlDbType.Int, PricingType));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the solar company for custom pricing.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="SolarCompany">The solar company.</param>
        /// <param name="RAMID">The ramid.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// solar list
        /// </returns>
        public List<SolarCompany> GetSolarCompanyForCustomPricing(int UserId, int UserTypeId, int ResellerId, string SolarCompany, int RAMID, string name)
        {
            string spName = "[GetSolarCompanyForCustomPricing]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompany", SqlDbType.NVarChar, SolarCompany));
            sqlParameters.Add(DBClient.AddParameters("RAMID", SqlDbType.Int, RAMID));
            sqlParameters.Add(DBClient.AddParameters("name", SqlDbType.NVarChar, name));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Gets the requested solar company to SSC.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// solar list
        /// </returns>
        public List<SolarCompany> GetRequestedSolarCompanyToSSC(int id)
        {
            string spName = "[SolarCompany_GetRequestedSolarCompanyToSSC]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SSCID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        public List<SolarCompany> GetSolarCompaanyFromSE(int id)
        {
            string spName = "[SolarCompany_GetSolarCompaanyFromSE]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        public List<SolarCompany> GetSolarCompany_IsSSCByResellerID(int id, bool IsSSC)
        {
            string spName = "[SolarCompany_IsSSCByResellerID_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, id));
            sqlParameters.Add(DBClient.AddParameters("IsSSC", SqlDbType.Bit, IsSSC));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        //public List<SolarCompany> GetSolarCompany_IsSSCByResellerID(bool IsSSC)
        //{
        //    string spName = "[SolarCompany_IsSSCByResellerID_BindDropDown]";
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    //sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, id));
        //    sqlParameters.Add(DBClient.AddParameters("IsSSC", SqlDbType.Bit, IsSSC));
        //    IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
        //    return solarCompanyList.ToList();
        //}

        public List<SolarCompany> GetSolarCompanyByWholeSalerID(int id)
        {
            string spName = "[SolarCompanyByWholeSalerID_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("WholeSalerID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        //public bool CheckIsWholeSaler_ByResellerId(int id)
        //{
        //    string spName = "[CheckIsWholeSaler_ByResellerId]";
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, id));
        //    return Convert.ToBoolean(CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray()));
        //}

        public DataSet CheckIsWholeSaler_ByResellerId(int id)
        {
            string spName = "[CheckIsWholeSaler_ByResellerId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, id));
            DataSet dsCheckIsWholeSaler = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsCheckIsWholeSaler;
        }

        public List<SolarCompany> GetSolarCompanyForVEECGlobalPricing(int UserId, int UserTypeId, int ResellerId, int PricingType)
        {
            string spName = "[GetSolarCompanyForVEECGlobalPricing]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("PricingType", SqlDbType.Int, PricingType));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        public void UpdateSolarCompanyAllowedSPV(string solarCompayIds, int isSPVAllowedSPV)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.VarChar, solarCompayIds));
            sqlParameters.Add(DBClient.AddParameters("SCisAllowedSPV", SqlDbType.Bit, isSPVAllowedSPV));
            CommonDAL.ExecuteScalar("SolarCompany_UpdateAllowedSPV", sqlParameters.ToArray());
        }
        /// <summary>
        /// Gets Representative for auto sign
        /// </summary>
        /// <returns>List of SolarCompanies/SCO </returns>
        public List<Representative> GetRepresentativeForAutoSign(int jobId=0)
        {
            string spName = "[GetRepresentativeForAutoSign]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int,ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int,jobId));
            IList<Representative> solarCompanyList = CommonDAL.ExecuteProcedure<Representative>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }
        public DataSet GetAutoSignSettingsData(int UserId,bool isForDefaultSelection)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("isForDefaultSelection", SqlDbType.Bit, isForDefaultSelection));
            DataSet ds = CommonDAL.ExecuteDataSet("GetAutoSignSettingsData", sqlParameters.ToArray());
            return ds;
        }
        
        public void SaveAutoSignSettingsData(int UserId,string Position,bool isSubcontractor,bool isEmployee,string signature,bool isChangedDesign,int JobId,string latitude,string longitude)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Position", SqlDbType.VarChar, Position));
            sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.VarChar, signature));
            sqlParameters.Add(DBClient.AddParameters("isSubcontractor", SqlDbType.Bit, isSubcontractor));
            sqlParameters.Add(DBClient.AddParameters("isEmployee", SqlDbType.Bit, isEmployee));
            sqlParameters.Add(DBClient.AddParameters("isChangedDesign", SqlDbType.Bit, isChangedDesign));
            sqlParameters.Add(DBClient.AddParameters("SignedBy", SqlDbType.VarChar, ProjectSession.LoggedInName));
            sqlParameters.Add(DBClient.AddParameters("SignedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar,latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, longitude));
            CommonDAL.ExecuteScalar("SaveAutoSignSettingsData", sqlParameters.ToArray());
        }

        public void UpdateInstallerWrittenStatementSetting(int jobId, bool isEmployee, bool isChangedDesign)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("isEmployee", SqlDbType.Bit, isEmployee));
            sqlParameters.Add(DBClient.AddParameters("isChangedDesign", SqlDbType.Bit, isChangedDesign));
            CommonDAL.ExecuteScalar("UpdateInstallerWrittenStatementSetting", sqlParameters.ToArray());
        }
    }
}
